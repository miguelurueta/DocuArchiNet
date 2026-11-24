Option Strict On
Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Data.Common
Imports System.Globalization
Imports System.Linq
Imports System.Text
Imports MySql.Data.MySqlClient

'DML funcional, nunca instalación. Todas las agrupaciones comparten la transacción.
Public Class MySqlConfirmacionExcelWriter
    Private ReadOnly _cn As DbConnection
    Private ReadOnly _tx As DbTransaction
    Public Sub New(cn As DbConnection, tx As DbTransaction)
        _cn = cn : _tx = tx
    End Sub
    Public Function Agrupar(p As PlantillaExcel, filas As List(Of FilaNormalizadaExcel)) As List(Of List(Of FilaNormalizadaExcel))
        Dim columnas = p.Campos.OrderBy(Function(c) c.CAMPO_DESTINO, StringComparer.Ordinal).Select(Function(c) c.CAMPO_DESTINO).ToArray()
        If columnas.Length = 0 OrElse columnas.Length > 128 OrElse columnas.Distinct(StringComparer.Ordinal).Count() <> columnas.Length OrElse filas Is Nothing OrElse filas.Count = 0 Then Throw ReglasCargaExcel.Fallo("CONFIGURACION_INVALIDA")
        Dim prefijo = "INSERT INTO " & SqlCargaExcel.Identificador(p.NOMBRE_PLANTILLA) & " (" & String.Join(",", columnas.Select(AddressOf SqlCargaExcel.Identificador)) & ") VALUES "
        Dim maximo As Long
        Using q = SqlCargaExcel.Cmd(_cn, _tx, "SELECT @@max_allowed_packet")
            maximo = Math.Min(65536L, Convert.ToInt64(q.ExecuteScalar(), CultureInfo.InvariantCulture) \ 2)
        End Using
        Dim inicial = CLng(Encoding.UTF8.GetByteCount(prefijo)) + 128L, bytes = inicial, parametros = 0
        Dim resultado As New List(Of List(Of FilaNormalizadaExcel)), grupo As New List(Of FilaNormalizadaExcel)
        For Each fila In filas
            If fila.Valores.Count <> columnas.Length Then Throw ReglasCargaExcel.Fallo("LECTURA_INVALIDA")
            Dim costo As Long = 16
            For Each columna In columnas
                Dim v = fila.Valores.SingleOrDefault(Function(x) x.CampoDestino = columna)
                If v Is Nothing Then Throw ReglasCargaExcel.Fallo("LECTURA_INVALIDA")
                costo += CLng(Encoding.UTF8.GetByteCount(If(v.Texto, If(v.NumeroCanonico, "")))) * 4L + 128L
            Next
            If inicial + costo > maximo Then Throw ReglasCargaExcel.Fallo("LIMITE_PAQUETE_EXCEDIDO")
            If parametros + columnas.Length > 128 OrElse bytes + costo > maximo Then
                resultado.Add(grupo) : grupo = New List(Of FilaNormalizadaExcel) : bytes = inicial : parametros = 0
            End If
            grupo.Add(fila) : bytes += costo : parametros += columnas.Length
        Next
        If grupo.Count > 0 Then resultado.Add(grupo)
        Return resultado
    End Function
    Public Sub Escribir(c As ContextoCargaExcel, p As PlantillaExcel, esquema As EsquemaCargaExcel, modo As String, grupos As List(Of List(Of FilaNormalizadaExcel)))
        If modo <> "CREAR" AndAlso modo <> "AGREGAR" AndAlso modo <> "REEMPLAZAR" Then Throw ReglasCargaExcel.Fallo("MODO_INVALIDO")
        If modo = "REEMPLAZAR" Then
            If esquema.Politica.Modo <> "CONEXION" AndAlso esquema.Politica.Modo <> "COLUMNA" Then Throw ReglasCargaExcel.Fallo("POLITICA_DESTINO_NO_DEFINIDA")
            Dim filtro = If(esquema.Politica.Modo = "COLUMNA", " WHERE " & SqlCargaExcel.Identificador(esquema.Politica.ColumnaEmpresa) & "=@empresa", "")
            Using q = SqlCargaExcel.Cmd(_cn, _tx, "DELETE FROM " & SqlCargaExcel.Identificador(p.NOMBRE_PLANTILLA) & filtro)
                If filtro.Length > 0 Then ImportacionExcelContextAdapter.Parametro(q, "@empresa", c.EmpresaId.ToString(CultureInfo.InvariantCulture))
                q.ExecuteNonQuery() : SinAdvertencias()
            End Using
        End If
        Dim columnas = p.Campos.OrderBy(Function(x) x.CAMPO_DESTINO, StringComparer.Ordinal).Select(Function(x) x.CAMPO_DESTINO).ToArray()
        For Each grupo In grupos
            Using q = SqlCargaExcel.Cmd(_cn, _tx, "")
                Dim tuplas As New List(Of String)
                For Each fila In grupo
                    Dim args As New List(Of String)
                    For Each columna In columnas
                        Dim v = fila.Valores.Single(Function(x) x.CampoDestino = columna), nombre = "@p" & q.Parameters.Count.ToString(CultureInfo.InvariantCulture)
                        Dim dato As Object = DBNull.Value
                        If Not v.EsNulo Then
                            Select Case v.Tipo
                                Case "TEXTO" : dato = v.Texto
                                Case "DATE" : dato = v.Fecha.Value
                                Case "ENTERO", "DECIMAL" : dato = v.NumeroCanonico
                                Case Else : Throw ReglasCargaExcel.Fallo("LECTURA_INVALIDA")
                            End Select
                        End If
                        ImportacionExcelContextAdapter.Parametro(q, nombre, dato) : args.Add(nombre)
                    Next
                    tuplas.Add("(" & String.Join(",", args) & ")")
                Next
                q.CommandText = "INSERT INTO " & SqlCargaExcel.Identificador(p.NOMBRE_PLANTILLA) & " (" & String.Join(",", columnas.Select(AddressOf SqlCargaExcel.Identificador)) & ") VALUES " & String.Join(",", tuplas)
                If q.ExecuteNonQuery() <> grupo.Count Then Throw ReglasCargaExcel.Fallo("ESCRITURA_RECHAZADA")
                SinAdvertencias()
            End Using
        Next
    End Sub
    Private Sub SinAdvertencias()
        Using q = SqlCargaExcel.Cmd(_cn, _tx, "SHOW COUNT(*) WARNINGS")
            If Convert.ToInt64(q.ExecuteScalar(), CultureInfo.InvariantCulture) <> 0 Then Throw ReglasCargaExcel.Fallo("CONVERSION_MYSQL_INVALIDA")
        End Using
    End Sub
End Class

Partial Public Class MySqlCargaExcelRepository
    Public Function AbrirConfirmacion(c As ContextoCargaExcel, id As String) As ISesionConfirmacionExcel Implements IConfirmacionExcelRepository.AbrirConfirmacion
        ReglasCargaExcel.Autorizar(c)
        If Not ReglasCargaExcel.IdValido(id) Then Throw ReglasCargaExcel.Fallo("SOLICITUD_INVALIDA")
        Return New Sesion(_fabrica, _politica, _descartar, c, id)
    End Function
    Public Function ConsultarEstructura(c As ContextoCargaExcel, e As CargaExcelEjecucion, p As PlantillaExcel) As String Implements IConfirmacionExcelRepository.ConsultarEstructura
        ReglasCargaExcel.Propietario(c, e)
        Using cn = _fabrica()
            cn.Open()
            Return New MySqlEstructuraExcel(cn, New MySqlPreparacionExcelValidator(cn, c, _politica)).ConsultarPropia(p, e)
        End Using
    End Function
    Public Shared Function NombreLockDestino(servidor As String, puerto As UInteger, base As String, tabla As String, minusculas As Integer) As String
        If Not ServicioPlantillasExcel.TablaValida(tabla) OrElse minusculas < 0 OrElse minusculas > 2 Then Throw ReglasCargaExcel.Fallo("DESTINO_INVALIDO")
        Return "dt" & ReglasCargaExcel.HashDocumento(New With {.Servidor = servidor, .Puerto = puerto, .Base = If(minusculas = 0, base, base.ToLowerInvariant()), .Tabla = If(minusculas = 0, tabla, tabla.ToLowerInvariant())}).Substring(0, 62)
    End Function
    Partial Private Class Sesion
        Implements ISesionConfirmacionExcel
        Private ReadOnly _abrirDestino As Func(Of DbConnection)
        Private _destino As DbConnection
        Private _lockDestino As String
        Private _tablaBloqueada As String
        Private _estructuraConfirmacion As MySqlEstructuraExcel
        Private Function EstructuraActual(Optional tx As DbTransaction = Nothing) As MySqlEstructuraExcel
            If _estructuraConfirmacion Is Nothing Then _estructuraConfirmacion = New MySqlEstructuraExcel(_cn, _validador)
            _estructuraConfirmacion.TransaccionActual(tx)
            Return _estructuraConfirmacion
        End Function
        Public Sub BloquearDestino(tabla As String) Implements ISesionConfirmacionExcel.BloquearDestino
            If _destino IsNot Nothing Then Throw ReglasCargaExcel.Fallo("CONFIGURACION_INVALIDA")
            Using q = SqlCargaExcel.Cmd(_cn, Nothing, "SELECT @@hostname,@@port,DATABASE(),@@lower_case_table_names"), r = q.ExecuteReader()
                If Not r.Read() Then Throw ReglasCargaExcel.Fallo("CAPACIDAD_NO_DISPONIBLE")
                _lockDestino = NombreLockDestino(SqlCargaExcel.Texto(r, 0), Convert.ToUInt32(r(1), CultureInfo.InvariantCulture), SqlCargaExcel.Texto(r, 2), tabla, Convert.ToInt32(r(3), CultureInfo.InvariantCulture))
            End Using
            _destino = _abrirDestino() : _destino.Open()
            If MySqlPreparacionExcelValidator.Identidad(_destino) <> MySqlPreparacionExcelValidator.Identidad(_cn) Then Throw ReglasCargaExcel.Fallo("ACCESO_DENEGADO")
            Using q = SqlCargaExcel.Cmd(_destino, Nothing, "SELECT GET_LOCK(@nombre,0)", "@nombre", _lockDestino)
                Dim r = q.ExecuteScalar()
                If r Is Nothing OrElse Convert.IsDBNull(r) OrElse Convert.ToInt32(r, CultureInfo.InvariantCulture) <> 1 Then
                    _lockDestino = Nothing
                    Throw ReglasCargaExcel.Fallo("OPERACION_EN_CURSO")
                End If
            End Using
            _tablaBloqueada = tabla
        End Sub
        Private Sub VerificarLocks(Optional tx As DbTransaction = Nothing)
            If _destino Is Nothing OrElse _tablaBloqueada Is Nothing Then Throw ReglasCargaExcel.Fallo("CONFIGURACION_INVALIDA")
            For Each par In {Tuple.Create(_cn, _lock, tx), Tuple.Create(_destino, _lockDestino, CType(Nothing, DbTransaction))}
                Using q = SqlCargaExcel.Cmd(par.Item1, par.Item3, "SELECT IS_USED_LOCK(@nombre)=CONNECTION_ID()", "@nombre", par.Item2)
                    Dim r = q.ExecuteScalar()
                    If r Is Nothing OrElse Convert.IsDBNull(r) OrElse Convert.ToInt32(r, CultureInfo.InvariantCulture) <> 1 Then Throw ReglasCargaExcel.Fallo("RESULTADO_INCIERTO")
                End Using
            Next
        End Sub
        Public Function LeerPlantilla(id As UInteger) As PlantillaExcel Implements ISesionConfirmacionExcel.LeerPlantilla
            Return MySqlPlantillaExcelRepository.LeerCarga(_cn, Nothing, id, _ctx.ConexionId, False)
        End Function
        Public Function Inspeccionar(p As PlantillaExcel, e As CargaExcelEjecucion) As EsquemaCargaExcel Implements ISesionConfirmacionExcel.Inspeccionar
            VerificarLocks()
            If e.Tabla <> _tablaBloqueada Then Throw ReglasCargaExcel.Fallo("CONFLICTO_CONFIRMACION")
            Return EstructuraActual().Inspeccionar(p, e)
        End Function
        Public Sub AsegurarEstructura(p As PlantillaExcel, e As CargaExcelEjecucion) Implements ISesionConfirmacionExcel.AsegurarEstructura
            VerificarLocks()
            If e.Tabla <> _tablaBloqueada Then Throw ReglasCargaExcel.Fallo("CONFLICTO_CONFIRMACION")
            Dim estructura = EstructuraActual()
            estructura.Asegurar(p, e)
        End Sub
        Public Function Ejecutar(e As CargaExcelEjecucion, filas As List(Of FilaNormalizadaExcel), validar As Action(Of PlantillaExcel, EsquemaCargaExcel)) As CargaExcelEjecucion Implements ISesionConfirmacionExcel.Ejecutar
            VerificarLocks()
            If e.Tabla <> _tablaBloqueada OrElse filas Is Nothing OrElse filas.Count <> e.FilasPrevistas.GetValueOrDefault() OrElse filas.Count < 1 Then Throw ReglasCargaExcel.Fallo("CONFLICTO_CONFIRMACION")
            Dim tx As DbTransaction = Nothing, intentoCommit = False, rollbackConocido = False
            Dim resultado As CargaExcelEjecucion = Nothing, fallo As ImportacionExcelException = Nothing
            Try
                tx = _cn.BeginTransaction(IsolationLevel.ReadCommitted)
                _validador.TransaccionActual(tx)
                Dim actual = Leer(_cn, tx, _ctx, _id, True)
                ReglasCargaExcel.Propietario(_ctx, actual) : Equivalente(actual, e)
                If actual.ConfiguracionHash <> e.ConfiguracionHash OrElse actual.EsquemaHash <> e.EsquemaHash Then Throw ReglasCargaExcel.Fallo("CONFLICTO_CONFIRMACION")
                If actual.Estado = "CONFIRMADA" Then
                    resultado = actual : tx.Rollback()
                Else
                    If actual.Estado <> "PREPARADA" AndAlso actual.Estado <> "FALLIDA" Then Throw ReglasCargaExcel.Fallo("ESTADO_NO_CONFIRMABLE")
                    Dim p = MySqlPlantillaExcelRepository.LeerCarga(_cn, tx, e.PlantillaId, _ctx.ConexionId, True)
                    Dim estructura = EstructuraActual(tx)
                    Dim esquema = estructura.Inspeccionar(p, e)
                    validar(p, esquema)
                    _validador.ActivarModoEstricto()
                    Dim escritor As New MySqlConfirmacionExcelWriter(_cn, tx), grupos = escritor.Agrupar(p, filas)
                    If _validador.Validar(p, esquema, filas, e.Modo).Total <> 0 Then Throw ReglasCargaExcel.Fallo("CLAVES_INVALIDAS")
                    If e.Modo = "CREAR" Then estructura.ExigirVacia(e.Tabla)
                    VerificarLocks(tx)
                    escritor.Escribir(_ctx, p, esquema, e.Modo, grupos)
                    validar(p, estructura.Inspeccionar(p, e))
                    VerificarLocks(tx)
                    Using q = SqlCargaExcel.Cmd(_cn, tx, "UPDATE imp01_carga_ejecucion SET estado='CONFIRMADA',filas_confirmadas=@filas,codigo_incidencia=NULL,confirmado_utc=UTC_TIMESTAMP(),actualizado_utc=UTC_TIMESTAMP() WHERE id_carga=@id AND ambito_ref=@empresa AND usuario_ref=@usuario AND conexion_ref=@conexion AND estado=@estado AND configuracion_hash=@config AND esquema_hash=@esquema AND archivo_hash=@archivo", "@filas", filas.Count, "@id", _id, "@empresa", _ctx.EmpresaId.ToString(CultureInfo.InvariantCulture), "@usuario", _ctx.UsuarioId.ToString(CultureInfo.InvariantCulture), "@conexion", _ctx.ConexionRef, "@estado", actual.Estado, "@config", SqlCargaExcel.Binario(e.ConfiguracionHash), "@esquema", SqlCargaExcel.Binario(e.EsquemaHash), "@archivo", SqlCargaExcel.Binario(e.Archivo.Hash))
                        If q.ExecuteNonQuery() <> 1 Then Throw ReglasCargaExcel.Fallo("CONFLICTO_CONFIRMACION")
                    End Using
                    resultado = Leer(_cn, tx, _ctx, _id, False)
                    intentoCommit = True
                    tx.Commit()
                End If
            Catch ex As Exception
                If Not intentoCommit AndAlso tx IsNot Nothing Then
                    Try
                        tx.Rollback() : rollbackConocido = True
                    Catch
                        rollbackConocido = False
                    End Try
                End If
                If intentoCommit OrElse Not rollbackConocido Then
                    fallo = ReglasCargaExcel.Fallo("RESULTADO_INCIERTO")
                ElseIf TypeOf ex Is ImportacionExcelException Then
                    fallo = DirectCast(ex, ImportacionExcelException)
                ElseIf TypeOf ex Is MySqlException AndAlso DirectCast(ex, MySqlException).Number = 1062 Then
                    fallo = ReglasCargaExcel.Fallo("CONFLICTO_CLAVE")
                Else
                    fallo = ReglasCargaExcel.Fallo("ESCRITURA_RECHAZADA")
                End If
            Finally
                _validador.TransaccionActual(Nothing)
                If _estructuraConfirmacion IsNot Nothing Then _estructuraConfirmacion.TransaccionActual(Nothing)
                If tx IsNot Nothing Then
                    Try
                        tx.Dispose()
                    Catch
                        fallo = ReglasCargaExcel.Fallo("RESULTADO_INCIERTO")
                    End Try
                End If
            End Try
            If fallo IsNot Nothing Then
                If rollbackConocido AndAlso Not intentoCommit AndAlso fallo.Codigo <> "RESULTADO_INCIERTO" AndAlso fallo.Codigo <> "ACCESO_DENEGADO" AndAlso fallo.Codigo <> "SOLICITUD_NO_VERIFICADA" Then RegistrarFallo(e, fallo.Codigo)
                Throw fallo
            End If
            Return resultado
        End Function
        Private Sub RegistrarFallo(e As CargaExcelEjecucion, codigo As String)
            Try
                VerificarLocks()
                Using tx = _cn.BeginTransaction(IsolationLevel.ReadCommitted)
                    Using q = SqlCargaExcel.Cmd(_cn, tx, "UPDATE imp01_carga_ejecucion SET estado='FALLIDA',codigo_incidencia=@codigo,actualizado_utc=UTC_TIMESTAMP() WHERE id_carga=@id AND ambito_ref=@empresa AND usuario_ref=@usuario AND conexion_ref=@conexion AND estado IN ('PREPARADA','FALLIDA') AND configuracion_hash=@config AND esquema_hash=@esquema AND archivo_hash=@archivo", "@codigo", codigo, "@id", _id, "@empresa", _ctx.EmpresaId.ToString(CultureInfo.InvariantCulture), "@usuario", _ctx.UsuarioId.ToString(CultureInfo.InvariantCulture), "@conexion", _ctx.ConexionRef, "@config", SqlCargaExcel.Binario(e.ConfiguracionHash), "@esquema", SqlCargaExcel.Binario(e.EsquemaHash), "@archivo", SqlCargaExcel.Binario(e.Archivo.Hash))
                        If q.ExecuteNonQuery() <> 1 Then Throw ReglasCargaExcel.Fallo("RESULTADO_INCIERTO")
                    End Using
                    tx.Commit()
                End Using
            Catch
                Throw ReglasCargaExcel.Fallo("RESULTADO_INCIERTO")
            End Try
        End Sub
        Private Sub CerrarDestino()
            If _destino Is Nothing Then Return
            Dim limpia = True
            Try
                If _lockDestino IsNot Nothing Then
                    Using q = SqlCargaExcel.Cmd(_destino, Nothing, "SELECT RELEASE_LOCK(@nombre)", "@nombre", _lockDestino)
                        Dim r = q.ExecuteScalar()
                        limpia = r IsNot Nothing AndAlso Not Convert.IsDBNull(r) AndAlso Convert.ToInt32(r, CultureInfo.InvariantCulture) = 1
                    End Using
                End If
            Catch
                limpia = False
            Finally
                Try
                    If Not limpia Then _descartar(_destino)
                Finally
                    _destino.Dispose() : _destino = Nothing
                End Try
            End Try
            If Not limpia Then Throw ReglasCargaExcel.Fallo("RESULTADO_INCIERTO")
        End Sub
    End Class
End Class
