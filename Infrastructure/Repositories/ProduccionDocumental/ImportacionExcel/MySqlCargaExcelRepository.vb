Option Strict On
Imports System
Imports System.Data
Imports System.Data.Common
Imports System.Globalization
Imports System.IO
Imports MySql.Data.MySqlClient

Public Class MySqlCargaExcelRepository
    Implements ICargaExcelRepository
    Private ReadOnly _fabrica As Func(Of DbConnection)
    Private ReadOnly _politica As Func(Of String, PoliticaDestinoCargaExcel)
    Private ReadOnly _descartar As Action(Of DbConnection)
    Private Const Columnas As String = "id_carga,id_plantilla,ambito_ref,usuario_ref,conexion_ref,tabla_destino,archivo_nombre,archivo_ref,archivo_bytes,archivo_hash,modo,revision_plantilla,configuracion_doc,configuracion_hash,esquema_hash,estado,filas_previstas,codigo_incidencia"
    Public Sub New(fabrica As Func(Of DbConnection), politica As Func(Of String, PoliticaDestinoCargaExcel), Optional descartar As Action(Of DbConnection) = Nothing)
        _fabrica = fabrica : _politica = politica
        _descartar = If(descartar, AddressOf DescartarConexion)
    End Sub
    Private Shared Sub DescartarConexion(cn As DbConnection)
        If Not TypeOf cn Is MySqlConnection Then Throw ReglasCargaExcel.Fallo("CAPACIDAD_NO_DISPONIBLE")
        MySqlConnection.ClearPool(DirectCast(cn, MySqlConnection))
    End Sub
    Public Function Abrir(c As ContextoCargaExcel, id As String) As ISesionCargaExcel Implements ICargaExcelRepository.Abrir
        ReglasCargaExcel.Autorizar(c)
        If Not ReglasCargaExcel.IdValido(id) Then Throw ReglasCargaExcel.Fallo("REFERENCIA_NO_DISPONIBLE")
        Return New Sesion(_fabrica, _politica, _descartar, c, id)
    End Function
    Public Function Consultar(c As ContextoCargaExcel, id As String) As CargaExcelEjecucion Implements ICargaExcelRepository.Consultar
        ReglasCargaExcel.Autorizar(c)
        If Not ReglasCargaExcel.IdValido(id) Then Throw ReglasCargaExcel.Fallo("REFERENCIA_NO_DISPONIBLE")
        Using cn = _fabrica()
            cn.Open()
            Return Leer(cn, Nothing, c, id, False)
        End Using
    End Function
    Private Shared Function Leer(cn As DbConnection, tx As DbTransaction, c As ContextoCargaExcel, id As String, bloquear As Boolean) As CargaExcelEjecucion
        Using q = SqlCargaExcel.Cmd(cn, tx, "SELECT " & Columnas & " FROM imp01_carga_ejecucion WHERE id_carga=@id AND ambito_ref=@empresa AND usuario_ref=@usuario AND conexion_ref=@conexion" & If(bloquear, " FOR UPDATE", ""), "@id", id, "@empresa", c.EmpresaId.ToString(CultureInfo.InvariantCulture), "@usuario", c.UsuarioId.ToString(CultureInfo.InvariantCulture), "@conexion", c.ConexionRef)
            Using r = q.ExecuteReader()
                If Not r.Read() Then Return Nothing
                Dim e As New CargaExcelEjecucion With {.Id = SqlCargaExcel.Texto(r, 0), .PlantillaId = Convert.ToUInt32(r(1), CultureInfo.InvariantCulture), .EmpresaId = c.EmpresaId, .UsuarioId = c.UsuarioId, .ConexionRef = SqlCargaExcel.Texto(r, 4), .Tabla = SqlCargaExcel.Texto(r, 5), .Archivo = New ArchivoRecibidoExcel With {.Nombre = SqlCargaExcel.Texto(r, 6), .Referencia = SqlCargaExcel.Texto(r, 7), .Bytes = Convert.ToInt64(r(8), CultureInfo.InvariantCulture), .Hash = SqlCargaExcel.Huella(r, 9)}, .Modo = SqlCargaExcel.Texto(r, 10), .RevisionPlantilla = If(r.IsDBNull(11), New Nullable(Of UInteger), New Nullable(Of UInteger)(Convert.ToUInt32(r(11), CultureInfo.InvariantCulture))), .ConfiguracionDoc = SqlCargaExcel.Texto(r, 12), .ConfiguracionHash = SqlCargaExcel.Huella(r, 13), .EsquemaHash = SqlCargaExcel.Huella(r, 14), .Estado = SqlCargaExcel.Texto(r, 15), .FilasPrevistas = If(r.IsDBNull(16), New Nullable(Of Integer), New Nullable(Of Integer)(Convert.ToInt32(r(16), CultureInfo.InvariantCulture))), .CodigoIncidencia = SqlCargaExcel.Texto(r, 17)}
                e.Archivo.Formato = Path.GetExtension(e.Archivo.Nombre).TrimStart("."c).ToUpperInvariant()
                Return e
            End Using
        End Using
    End Function
    Private Class Sesion
        Implements ISesionCargaExcel
        Private ReadOnly _cn As DbConnection
        Private ReadOnly _ctx As ContextoCargaExcel
        Private ReadOnly _id As String
        Private ReadOnly _validador As MySqlPreparacionExcelValidator
        Private ReadOnly _descartar As Action(Of DbConnection)
        Private _lock As String
        Private _cerrada As Boolean
        Public Sub New(fabrica As Func(Of DbConnection), politica As Func(Of String, PoliticaDestinoCargaExcel), descartar As Action(Of DbConnection), c As ContextoCargaExcel, id As String)
            _ctx = c : _id = id : _cn = fabrica() : _descartar = descartar
            Try
                _cn.Open()
                Dim fisica = MySqlPreparacionExcelValidator.Identidad(_cn)
                _lock = "dx" & ReglasCargaExcel.HashDocumento(New With {.Fisica = fisica, .Id = id}).Substring(0, 62)
                Using q = SqlCargaExcel.Cmd(_cn, Nothing, "SELECT GET_LOCK(@nombre,0)", "@nombre", _lock)
                    Dim adquirido = q.ExecuteScalar()
                    If adquirido Is Nothing OrElse Convert.IsDBNull(adquirido) OrElse Convert.ToInt32(adquirido, CultureInfo.InvariantCulture) <> 1 Then
                        _lock = Nothing
                        Throw ReglasCargaExcel.Fallo("OPERACION_EN_CURSO")
                    End If
                End Using
                _validador = New MySqlPreparacionExcelValidator(_cn, _ctx, politica)
            Catch
                Dispose() : Throw
            End Try
        End Sub
        Public Function Obtener() As CargaExcelEjecucion Implements ISesionCargaExcel.Obtener
            Return Leer(_cn, Nothing, _ctx, _id, False)
        End Function
        Public Function RegistrarRecibida(e As CargaExcelEjecucion) As CargaExcelEjecucion Implements ISesionCargaExcel.RegistrarRecibida
            ReglasCargaExcel.Propietario(_ctx, e)
            If e.Id <> _id OrElse e.Archivo Is Nothing OrElse e.Archivo.Bytes <= 0 OrElse Not ReglasCargaExcel.HashValido(e.Archivo.Hash) Then Throw ReglasCargaExcel.Fallo("ARCHIVO_INVALIDO")
            Try
                Using tx = _cn.BeginTransaction(IsolationLevel.ReadCommitted)
                    Dim anterior = Leer(_cn, tx, _ctx, _id, True)
                    If anterior IsNot Nothing Then
                        Equivalente(anterior, e) : tx.Commit() : Return anterior
                    End If
                    Using q = SqlCargaExcel.Cmd(_cn, tx, "INSERT INTO imp01_carga_ejecucion (id_carga,id_plantilla,ambito_ref,usuario_ref,conexion_ref,tabla_destino,archivo_nombre,archivo_ref,archivo_bytes,archivo_hash,estado,filas_confirmadas,creado_utc,actualizado_utc) VALUES (@id,@plantilla,@empresa,@usuario,@conexion,@tabla,@nombre,@ref,@bytes,@hash,'RECIBIDA',0,UTC_TIMESTAMP(),UTC_TIMESTAMP())", "@id", _id, "@plantilla", e.PlantillaId, "@empresa", _ctx.EmpresaId.ToString(CultureInfo.InvariantCulture), "@usuario", _ctx.UsuarioId.ToString(CultureInfo.InvariantCulture), "@conexion", _ctx.ConexionRef, "@tabla", e.Tabla, "@nombre", e.Archivo.Nombre, "@ref", e.Archivo.Referencia, "@bytes", e.Archivo.Bytes, "@hash", SqlCargaExcel.Binario(e.Archivo.Hash))
                        If q.ExecuteNonQuery() <> 1 Then Throw ReglasCargaExcel.Fallo("RESULTADO_INCIERTO")
                    End Using
                    tx.Commit() : e.Estado = "RECIBIDA" : Return e
                End Using
            Catch ex As ImportacionExcelException
                Throw
            Catch
                Throw ReglasCargaExcel.Fallo("RESULTADO_INCIERTO")
            End Try
        End Function
        Private Shared Sub Equivalente(a As CargaExcelEjecucion, b As CargaExcelEjecucion)
            If a.Id <> b.Id OrElse a.PlantillaId <> b.PlantillaId OrElse a.Tabla <> b.Tabla OrElse a.Archivo.Hash <> b.Archivo.Hash OrElse a.Archivo.Bytes <> b.Archivo.Bytes OrElse a.Archivo.Nombre <> b.Archivo.Nombre OrElse a.Archivo.Referencia <> b.Archivo.Referencia Then Throw ReglasCargaExcel.Fallo("CONFLICTO_RECEPCION")
        End Sub
        Public Function Inspeccionar(p As PlantillaExcel, modo As String) As EsquemaCargaExcel Implements ISesionCargaExcel.Inspeccionar
            Return _validador.Inspeccionar(p, modo)
        End Function
        Public Function ValidarClaves(p As PlantillaExcel, e As EsquemaCargaExcel, filas As Collections.Generic.List(Of FilaNormalizadaExcel), modo As String) As IncidenciasClavesExcel Implements ISesionCargaExcel.ValidarClaves
            Return _validador.Validar(p, e, filas, modo)
        End Function
        Public Function Publicar(e As CargaExcelEjecucion, hashEsperado As String) As CargaExcelEjecucion Implements ISesionCargaExcel.Publicar
            ReglasCargaExcel.Propietario(_ctx, e)
            If e.Id <> _id OrElse e.Estado <> "PREPARADA" OrElse Not ReglasCargaExcel.HashValido(e.ConfiguracionHash) OrElse Not ReglasCargaExcel.HashValido(e.EsquemaHash) OrElse Not e.FilasPrevistas.HasValue OrElse e.FilasPrevistas.Value < 1 Then Throw ReglasCargaExcel.Fallo("CONFIGURACION_INVALIDA")
            Try
                Using tx = _cn.BeginTransaction(IsolationLevel.ReadCommitted)
                    Dim anterior = Leer(_cn, tx, _ctx, _id, True)
                    If anterior Is Nothing Then Throw ReglasCargaExcel.Fallo("REFERENCIA_NO_DISPONIBLE")
                    Equivalente(anterior, e)
                    If anterior.Estado <> "RECIBIDA" AndAlso anterior.Estado <> "PREPARADA" Then Throw ReglasCargaExcel.Fallo("ESTADO_NO_PREPARABLE")
                    If anterior.Estado = "PREPARADA" AndAlso anterior.ConfiguracionHash <> e.ConfiguracionHash AndAlso anterior.ConfiguracionHash <> hashEsperado Then Throw ReglasCargaExcel.Fallo("CONFLICTO_PREPARACION")
                    Using q = SqlCargaExcel.Cmd(_cn, tx, "UPDATE imp01_carga_ejecucion SET modo=@modo,revision_plantilla=@revision,configuracion_doc=@doc,configuracion_hash=@config,esquema_hash=@esquema,estado='PREPARADA',filas_previstas=@filas,codigo_incidencia=NULL,actualizado_utc=UTC_TIMESTAMP() WHERE id_carga=@id AND ambito_ref=@empresa AND usuario_ref=@usuario AND conexion_ref=@conexion AND estado=@estado AND archivo_hash=@archivo", "@modo", e.Modo, "@revision", e.RevisionPlantilla.Value, "@doc", e.ConfiguracionDoc, "@config", SqlCargaExcel.Binario(e.ConfiguracionHash), "@esquema", SqlCargaExcel.Binario(e.EsquemaHash), "@filas", e.FilasPrevistas.Value, "@id", _id, "@empresa", _ctx.EmpresaId.ToString(CultureInfo.InvariantCulture), "@usuario", _ctx.UsuarioId.ToString(CultureInfo.InvariantCulture), "@conexion", _ctx.ConexionRef, "@estado", anterior.Estado, "@archivo", SqlCargaExcel.Binario(e.Archivo.Hash))
                        If q.ExecuteNonQuery() <> 1 Then Throw ReglasCargaExcel.Fallo("CONFLICTO_PREPARACION")
                    End Using
                    tx.Commit() : Return e
                End Using
            Catch ex As ImportacionExcelException
                Throw
            Catch
                Throw ReglasCargaExcel.Fallo("RESULTADO_INCIERTO")
            End Try
        End Function
        Public Sub Rechazar(hashEsperado As String, codigo As String) Implements ISesionCargaExcel.Rechazar
            Try
                Using tx = _cn.BeginTransaction(IsolationLevel.ReadCommitted)
                    Dim anterior = Leer(_cn, tx, _ctx, _id, True)
                    If anterior Is Nothing OrElse (anterior.Estado <> "RECIBIDA" AndAlso anterior.Estado <> "PREPARADA") OrElse anterior.ConfiguracionHash <> hashEsperado Then Throw ReglasCargaExcel.Fallo("CONFLICTO_PREPARACION")
                    Using q = SqlCargaExcel.Cmd(_cn, tx, "UPDATE imp01_carga_ejecucion SET estado='RECIBIDA',modo=NULL,revision_plantilla=NULL,configuracion_doc=NULL,configuracion_hash=NULL,esquema_hash=NULL,filas_previstas=NULL,codigo_incidencia=@codigo,actualizado_utc=UTC_TIMESTAMP() WHERE id_carga=@id AND ambito_ref=@empresa AND usuario_ref=@usuario AND conexion_ref=@conexion AND estado=@estado", "@codigo", codigo, "@id", _id, "@empresa", _ctx.EmpresaId.ToString(CultureInfo.InvariantCulture), "@usuario", _ctx.UsuarioId.ToString(CultureInfo.InvariantCulture), "@conexion", _ctx.ConexionRef, "@estado", anterior.Estado)
                        If q.ExecuteNonQuery() <> 1 Then Throw ReglasCargaExcel.Fallo("CONFLICTO_PREPARACION")
                    End Using
                    tx.Commit()
                End Using
            Catch ex As ImportacionExcelException
                Throw
            Catch
                Throw ReglasCargaExcel.Fallo("RESULTADO_INCIERTO")
            End Try
        End Sub
        Public Sub Dispose() Implements IDisposable.Dispose
            If _cerrada Then Return
            _cerrada = True
            Dim limpia = True
            Try
                If _validador IsNot Nothing Then limpia = _validador.Cerrar()
                If _lock IsNot Nothing Then
                    Using q = SqlCargaExcel.Cmd(_cn, Nothing, "SELECT RELEASE_LOCK(@nombre)", "@nombre", _lock)
                        Dim resultado = q.ExecuteScalar()
                        limpia = limpia AndAlso resultado IsNot Nothing AndAlso Not Convert.IsDBNull(resultado) AndAlso Convert.ToInt32(resultado, CultureInfo.InvariantCulture) = 1
                    End Using
                End If
            Catch
                limpia = False
            Finally
                Try
                    If Not limpia Then _descartar(_cn)
                Catch
                    Throw ReglasCargaExcel.Fallo("RESULTADO_INCIERTO")
                Finally
                    _cn.Dispose()
                End Try
            End Try
        End Sub
    End Class
End Class
