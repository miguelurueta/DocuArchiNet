Option Strict On
Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Data.Common
Imports System.Globalization
Imports System.Linq
Imports System.Text

Public Class MySqlPlantillaExcelRepository
    Implements IPlantillaExcelRepository
    Private ReadOnly _fabrica As Func(Of DbConnection)
    Private ReadOnly _conexionId As UInteger
    Private Const Cabecera As String = "ID_PLANTILLAIMP,IMP01_XMLCONEXION_ID_XMLCONEXION,NOMBRE_PLANTILLA,CONEXION_FUENTE,LIBRO_EXCEL,RUTA_ARCHIVO_FUENTE,ELIMINA_TABLA,ELIMINA_INSERTA_RECORD,INSERTA_RECORD,ESTADO_PLANTILLA,CAMPO_UNICO,REVISION_CARGA"
    Private Const Campos As String = "ID_CAMPOS_PLANTILLA,IMP01_PLANTILLAIMP_ID_PLANTILLAIMP,CAMPO_FUENTE,CAMPO_DESTINO,TIPO_CAMPO,REMPLAZA_COMA,REMPLAZA_PUNTO,OPCION_SPLITER,CARACTER_SPLITER,NUMERO_MATRIZ_EXPLITER"
    Public Sub New(fabrica As Func(Of DbConnection), conexionId As UInteger)
        If fabrica Is Nothing OrElse conexionId = 0 Then Throw ServicioPlantillasExcel.ErrorControlado("CAPACIDAD_NO_DISPONIBLE")
        _fabrica = fabrica
        _conexionId = conexionId
    End Sub
    Private Function Abrir() As DbConnection
        Dim cn = _fabrica()
        Try
            cn.Open()
            Return cn
        Catch ex As DbException
            cn.Dispose()
            Throw ServicioPlantillasExcel.ErrorControlado("CAPACIDAD_NO_DISPONIBLE")
        End Try
    End Function
    Private Shared Function Cmd(cn As DbConnection, tx As DbTransaction, sql As String, ParamArray args() As Object) As DbCommand
        Dim command = cn.CreateCommand()
        command.Transaction = tx
        command.CommandText = sql
        command.CommandTimeout = 10
        For i = 0 To args.Length - 1 Step 2
            ImportacionExcelContextAdapter.Parametro(command, CStr(args(i)), args(i + 1))
        Next
        Return command
    End Function
    Public Function Listar(termino As String, pagina As Integer, tamano As Integer) As PaginaPlantillasExcel Implements IPlantillaExcelRepository.Listar
        Dim result As New PaginaPlantillasExcel With {.Pagina = pagina, .TamanoPagina = tamano}
        'Escape explícito con ! para que %, _ y barra no amplíen la búsqueda.
        Dim filtro = "%" & termino.Replace("!", "!!").Replace("%", "!%").Replace("_", "!_") & "%"
        Try
            Using cn = Abrir()
                Using q = Cmd(cn, Nothing, "SELECT COUNT(*) FROM imp01_plantillaimp WHERE IMP01_XMLCONEXION_ID_XMLCONEXION=@conexion AND LEFT(BINARY NOMBRE_PLANTILLA,7)='imp_02_' AND CHAR_LENGTH(NOMBRE_PLANTILLA)>7 AND NOMBRE_PLANTILLA LIKE @termino ESCAPE '!'", "@conexion", _conexionId, "@termino", filtro)
                    result.Total = Convert.ToInt32(q.ExecuteScalar(), CultureInfo.InvariantCulture)
                End Using
                Using q = Cmd(cn, Nothing, "SELECT ID_PLANTILLAIMP,NOMBRE_PLANTILLA,REVISION_CARGA,ESTADO_PLANTILLA FROM imp01_plantillaimp WHERE IMP01_XMLCONEXION_ID_XMLCONEXION=@conexion AND LEFT(BINARY NOMBRE_PLANTILLA,7)='imp_02_' AND CHAR_LENGTH(NOMBRE_PLANTILLA)>7 AND NOMBRE_PLANTILLA LIKE @termino ESCAPE '!' ORDER BY ID_PLANTILLAIMP LIMIT @limite OFFSET @offset", "@conexion", _conexionId, "@termino", filtro, "@limite", tamano, "@offset", (pagina - 1) * tamano)
                    Using r = q.ExecuteReader()
                        While r.Read()
                            result.Plantillas.Add(New ResumenPlantillaExcel With {.PlantillaId = UInt(r, 0), .Nombre = Texto(r, 1), .Revision = UInt(r, 2), .Activa = UInt(r, 3) = 1})
                        End While
                    End Using
                End Using
            End Using
        Catch ex As DbException
            Throw ServicioPlantillasExcel.ErrorControlado("CAPACIDAD_NO_DISPONIBLE")
        End Try
        Return result
    End Function
    Public Function Obtener(id As UInteger) As PlantillaExcel Implements IPlantillaExcelRepository.Obtener
        Try
            Using cn = Abrir()
                'Una lectura consistente de cabecera/campos incluso mientras se guarda.
                Using tx = cn.BeginTransaction(IsolationLevel.RepeatableRead)
                    Dim p = Leer(cn, tx, id, False)
                    tx.Commit()
                    Return p
                End Using
            End Using
        Catch ex As DbException
            Throw ServicioPlantillasExcel.ErrorControlado("CAPACIDAD_NO_DISPONIBLE")
        End Try
    End Function
    Private Function Leer(cn As DbConnection, tx As DbTransaction, id As UInteger, bloquear As Boolean) As PlantillaExcel
        Return LeerCarga(cn, tx, id, _conexionId, bloquear)
    End Function
    Public Shared Function LeerCarga(cn As DbConnection, tx As DbTransaction, id As UInteger, conexionId As UInteger, bloquear As Boolean) As PlantillaExcel
        Dim p As PlantillaExcel = Nothing
        Using q = Cmd(cn, tx, "SELECT " & Cabecera & " FROM imp01_plantillaimp WHERE ID_PLANTILLAIMP=@id AND IMP01_XMLCONEXION_ID_XMLCONEXION=@conexion" & If(bloquear, " FOR UPDATE", ""), "@id", id, "@conexion", conexionId)
            Using r = q.ExecuteReader()
                If Not r.Read() Then Return Nothing
                p = New PlantillaExcel With {.ID_PLANTILLAIMP = UInt(r, 0), .IMP01_XMLCONEXION_ID_XMLCONEXION = UInt(r, 1), .NOMBRE_PLANTILLA = Texto(r, 2), .CONEXION_FUENTE = Texto(r, 3), .LIBRO_EXCEL = Texto(r, 4), .RUTA_ARCHIVO_FUENTE = Texto(r, 5), .ELIMINA_TABLA = UInt(r, 6), .ELIMINA_INSERTA_RECORD = UInt(r, 7), .INSERTA_RECORD = UInt(r, 8), .ESTADO_PLANTILLA = UInt(r, 9), .CAMPO_UNICO = Texto(r, 10), .REVISION_CARGA = UInt(r, 11)}
                If r.Read() Then Throw ServicioPlantillasExcel.ErrorControlado("CAPACIDAD_NO_DISPONIBLE")
            End Using
        End Using
        Using q = Cmd(cn, tx, "SELECT " & Campos & " FROM imp01_campos_plantilla WHERE IMP01_PLANTILLAIMP_ID_PLANTILLAIMP=@id ORDER BY ID_CAMPOS_PLANTILLA" & If(bloquear, " FOR UPDATE", ""), "@id", id)
            Using r = q.ExecuteReader()
                While r.Read()
                    p.Campos.Add(New CampoPlantillaExcel With {.ID_CAMPOS_PLANTILLA = UInt(r, 0), .CAMPO_FUENTE = Texto(r, 2), .CAMPO_DESTINO = Texto(r, 3), .TIPO_CAMPO = Texto(r, 4), .REMPLAZA_COMA = UIntNulo(r, 5), .REMPLAZA_PUNTO = UIntNulo(r, 6), .OPCION_SPLITER = UIntNulo(r, 7), .CARACTER_SPLITER = Texto(r, 8), .NUMERO_MATRIZ_EXPLITER = UIntNulo(r, 9)})
                End While
            End Using
        End Using
        Return p
    End Function
    Public Function Destino(tabla As String) As DestinoPlantillaExcel Implements IPlantillaExcelRepository.Destino
        If Not ServicioPlantillasExcel.TablaValida(tabla) Then Throw ServicioPlantillasExcel.ErrorControlado("DESTINO_INVALIDO")
        Try
            Using cn = Abrir()
                Return LeerDestino(cn, Nothing, tabla)
            End Using
        Catch ex As DbException
            Throw ServicioPlantillasExcel.ErrorControlado("CAPACIDAD_NO_DISPONIBLE")
        End Try
    End Function
    Private Shared Function LeerDestino(cn As DbConnection, tx As DbTransaction, tabla As String) As DestinoPlantillaExcel
        Dim d As New DestinoPlantillaExcel
        Using q = Cmd(cn, tx, "SELECT ENGINE FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME=@tabla AND TABLE_TYPE='BASE TABLE'", "@tabla", tabla)
            Dim motor = q.ExecuteScalar()
            If motor Is Nothing OrElse Convert.IsDBNull(motor) Then Return d
            d.Existe = True
            d.Motor = Convert.ToString(motor, CultureInfo.InvariantCulture)
        End Using
        Using q = Cmd(cn, tx, "SELECT COLUMN_NAME,DATA_TYPE,COLUMN_TYPE,CHARACTER_MAXIMUM_LENGTH,IS_NULLABLE,COLUMN_DEFAULT,CHARACTER_SET_NAME,COLLATION_NAME,EXTRA FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME=@tabla ORDER BY ORDINAL_POSITION", "@tabla", tabla)
            Using r = q.ExecuteReader()
                While r.Read()
                    d.Columnas.Add(New ColumnaDestinoExcel With {.Nombre = Texto(r, 0), .Tipo = Texto(r, 1), .TipoCompleto = Texto(r, 2), .Longitud = If(r.IsDBNull(3), New Nullable(Of Long)(), New Nullable(Of Long)(Convert.ToInt64(r(3), CultureInfo.InvariantCulture))), .Nulable = Texto(r, 4) = "YES", .Predeterminado = Texto(r, 5), .Charset = Texto(r, 6), .Collation = Texto(r, 7), .Extra = Texto(r, 8)})
                End While
            End Using
        End Using
        Return d
    End Function
    Public Function Guardar(p As PlantillaExcel, revisionEsperada As UInteger, huellaEsperada As String) As PlantillaExcel Implements IPlantillaExcelRepository.Guardar
        If p Is Nothing OrElse p.IMP01_XMLCONEXION_ID_XMLCONEXION <> _conexionId OrElse Not ServicioPlantillasExcel.TablaValida(p.NOMBRE_PLANTILLA) Then Throw ServicioPlantillasExcel.ErrorControlado("ACCESO_DENEGADO")
        Try
            Using cn = Abrir()
                VerificarEsquema(cn)
                Using tx = cn.BeginTransaction(IsolationLevel.ReadCommitted)
                    Try
                        Dim anterior As PlantillaExcel = Nothing
                        If p.ID_PLANTILLAIMP <> 0 Then
                            anterior = Leer(cn, tx, p.ID_PLANTILLAIMP, True)
                            If anterior Is Nothing Then Throw ServicioPlantillasExcel.ErrorControlado("NO_ENCONTRADO")
                            If anterior.REVISION_CARGA <> revisionEsperada OrElse Not String.Equals(ServicioPlantillasExcel.Huella(anterior), huellaEsperada, StringComparison.Ordinal) Then Throw ServicioPlantillasExcel.ErrorControlado("CONFLICTO_REVISION")
                            ServicioPlantillasExcel.ModoHistorico(anterior)
                            'Edición no mueve una plantilla a otro destino ni conexión.
                            If anterior.NOMBRE_PLANTILLA <> p.NOMBRE_PLANTILLA Then Throw ServicioPlantillasExcel.ErrorControlado("DESTINO_INVALIDO")
                            p.RUTA_ARCHIVO_FUENTE = anterior.RUTA_ARCHIVO_FUENTE
                            p.REVISION_CARGA = anterior.REVISION_CARGA + 1UI
                        Else
                            If revisionEsperada <> 0 OrElse p.Campos.Any(Function(c) c.ID_CAMPOS_PLANTILLA <> 0) Then Throw ServicioPlantillasExcel.ErrorControlado("SOLICITUD_INVALIDA")
                            p.REVISION_CARGA = 1
                        End If
                        ServicioPlantillasExcel.ValidarDestino(p, LeerDestino(cn, tx, p.NOMBRE_PLANTILLA))
                        Dim valores = ValoresCabecera(p)
                        ValidarTextos(cn, tx, "imp01_plantillaimp", valores)
                        'Validar todos los campos antes de escribir la cabecera o un prefijo de filas.
                        Dim errores As New List(Of DetalleConfiguracionExcel)
                        For i = 0 To p.Campos.Count - 1
                            ValidarTextos(cn, tx, "imp01_campos_plantilla", ValoresCampo(p, p.Campos(i)), errores, i + 1, p.Campos(i).CAMPO_DESTINO)
                        Next
                        If errores.Count > 0 Then
                            Dim rechazo = ServicioPlantillasExcel.ErrorControlado("CONFIGURACION_INVALIDA")
                            rechazo.Detalles = errores : Throw rechazo
                        End If
                        Dim columnas = Cabecera.Split(","c).Skip(1).ToArray()
                        Dim sql As String
                        If anterior Is Nothing Then
                            sql = "INSERT INTO imp01_plantillaimp (" & String.Join(",", columnas) & ") VALUES (" & String.Join(",", columnas.Select(Function(c) "@" & c)) & ")"
                        Else
                            sql = "UPDATE imp01_plantillaimp SET " & String.Join(",", columnas.Select(Function(c) c & "=@" & c)) & " WHERE ID_PLANTILLAIMP=@id AND IMP01_XMLCONEXION_ID_XMLCONEXION=@conexion AND REVISION_CARGA=@revision"
                        End If
                        Using q = Cmd(cn, tx, sql)
                            For Each c In columnas
                                ImportacionExcelContextAdapter.Parametro(q, "@" & c, valores(c))
                            Next
                            If anterior IsNot Nothing Then
                                ImportacionExcelContextAdapter.Parametro(q, "@id", p.ID_PLANTILLAIMP)
                                ImportacionExcelContextAdapter.Parametro(q, "@conexion", _conexionId)
                                ImportacionExcelContextAdapter.Parametro(q, "@revision", revisionEsperada)
                            End If
                            If q.ExecuteNonQuery() <> 1 Then Throw ServicioPlantillasExcel.ErrorControlado("CONFLICTO_REVISION")
                        End Using
                        If anterior Is Nothing Then
                            Using q = Cmd(cn, tx, "SELECT LAST_INSERT_ID()")
                                p.ID_PLANTILLAIMP = Convert.ToUInt32(q.ExecuteScalar(), CultureInfo.InvariantCulture)
                            End Using
                        End If
                        GuardarCampos(cn, tx, p, anterior)
                        'Leer antes de commit para devolver exactamente los valores persistidos.
                        Dim guardada = Leer(cn, tx, p.ID_PLANTILLAIMP, False)
                        tx.Commit()
                        Return guardada
                    Catch
                        Try
                            tx.Rollback()
                        Catch ex As DbException
                            'Preservar el fallo original; nunca reintentar una escritura automáticamente.
                        End Try
                        Throw
                    End Try
                End Using
            End Using
        Catch ex As DbException
            'No filtrar SQL, endpoint, valores, credenciales o mensajes del proveedor.
            Throw ServicioPlantillasExcel.ErrorControlado("GUARDADO_NO_COMPLETADO")
        End Try
    End Function
    Private Shared Sub VerificarEsquema(cn As DbConnection)
        Using q = Cmd(cn, Nothing, "SELECT COUNT(*) FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME IN ('imp01_plantillaimp','imp01_campos_plantilla') AND ENGINE='InnoDB'")
            If Convert.ToInt32(q.ExecuteScalar(), CultureInfo.InvariantCulture) <> 2 Then Throw ServicioPlantillasExcel.ErrorControlado("CAPACIDAD_NO_DISPONIBLE")
        End Using
    End Sub
    Private Shared Sub GuardarCampos(cn As DbConnection, tx As DbTransaction, p As PlantillaExcel, anterior As PlantillaExcel)
        Dim existentes = If(anterior Is Nothing, New List(Of CampoPlantillaExcel), anterior.Campos)
        Dim recibidos As New HashSet(Of UInteger)
        For Each c In p.Campos
            If c.ID_CAMPOS_PLANTILLA <> 0 AndAlso (Not recibidos.Add(c.ID_CAMPOS_PLANTILLA) OrElse Not existentes.Any(Function(x) x.ID_CAMPOS_PLANTILLA = c.ID_CAMPOS_PLANTILLA)) Then Throw ServicioPlantillasExcel.ErrorControlado("SOLICITUD_INVALIDA")
            Dim valores = ValoresCampo(p, c)
            Dim columnas = Campos.Split(","c).Skip(1).ToArray()
            Dim sql = If(c.ID_CAMPOS_PLANTILLA = 0, "INSERT INTO imp01_campos_plantilla (" & String.Join(",", columnas) & ") VALUES (" & String.Join(",", columnas.Select(Function(x) "@" & x)) & ")", "UPDATE imp01_campos_plantilla SET " & String.Join(",", columnas.Select(Function(x) x & "=@" & x)) & " WHERE ID_CAMPOS_PLANTILLA=@campoId AND IMP01_PLANTILLAIMP_ID_PLANTILLAIMP=@plantillaId")
            Using q = Cmd(cn, tx, sql)
                For Each col In columnas
                    ImportacionExcelContextAdapter.Parametro(q, "@" & col, valores(col))
                Next
                If c.ID_CAMPOS_PLANTILLA <> 0 Then
                    ImportacionExcelContextAdapter.Parametro(q, "@campoId", c.ID_CAMPOS_PLANTILLA)
                    ImportacionExcelContextAdapter.Parametro(q, "@plantillaId", p.ID_PLANTILLAIMP)
                End If
                'UPDATE sin cambios puede devolver cero con CLIENT_FOUND_ROWS deshabilitado.
                Dim afectadas = q.ExecuteNonQuery()
                If c.ID_CAMPOS_PLANTILLA = 0 AndAlso afectadas <> 1 Then Throw ServicioPlantillasExcel.ErrorControlado("GUARDADO_NO_COMPLETADO")
            End Using
        Next
        For Each viejo In existentes.Where(Function(c) Not recibidos.Contains(c.ID_CAMPOS_PLANTILLA))
            Using q = Cmd(cn, tx, "DELETE FROM imp01_campos_plantilla WHERE ID_CAMPOS_PLANTILLA=@campo AND IMP01_PLANTILLAIMP_ID_PLANTILLAIMP=@plantilla", "@campo", viejo.ID_CAMPOS_PLANTILLA, "@plantilla", p.ID_PLANTILLAIMP)
                If q.ExecuteNonQuery() <> 1 Then Throw ServicioPlantillasExcel.ErrorControlado("CONFLICTO_REVISION")
            End Using
        Next
    End Sub
    Private Shared Function ValoresCabecera(p As PlantillaExcel) As Dictionary(Of String, Object)
        Return New Dictionary(Of String, Object)(StringComparer.Ordinal) From {
            {"IMP01_XMLCONEXION_ID_XMLCONEXION", p.IMP01_XMLCONEXION_ID_XMLCONEXION}, {"NOMBRE_PLANTILLA", p.NOMBRE_PLANTILLA}, {"CONEXION_FUENTE", p.CONEXION_FUENTE}, {"LIBRO_EXCEL", p.LIBRO_EXCEL}, {"RUTA_ARCHIVO_FUENTE", p.RUTA_ARCHIVO_FUENTE}, {"ELIMINA_TABLA", p.ELIMINA_TABLA}, {"ELIMINA_INSERTA_RECORD", p.ELIMINA_INSERTA_RECORD}, {"INSERTA_RECORD", p.INSERTA_RECORD}, {"ESTADO_PLANTILLA", p.ESTADO_PLANTILLA}, {"CAMPO_UNICO", p.CAMPO_UNICO}, {"REVISION_CARGA", p.REVISION_CARGA}}
    End Function
    Private Shared Function ValoresCampo(p As PlantillaExcel, c As CampoPlantillaExcel) As Dictionary(Of String, Object)
        Return New Dictionary(Of String, Object)(StringComparer.Ordinal) From {
            {"IMP01_PLANTILLAIMP_ID_PLANTILLAIMP", p.ID_PLANTILLAIMP}, {"CAMPO_FUENTE", c.CAMPO_FUENTE}, {"CAMPO_DESTINO", c.CAMPO_DESTINO}, {"TIPO_CAMPO", c.TIPO_CAMPO}, {"REMPLAZA_COMA", c.REMPLAZA_COMA}, {"REMPLAZA_PUNTO", c.REMPLAZA_PUNTO}, {"OPCION_SPLITER", c.OPCION_SPLITER}, {"CARACTER_SPLITER", c.CARACTER_SPLITER}, {"NUMERO_MATRIZ_EXPLITER", c.NUMERO_MATRIZ_EXPLITER}}
    End Function
    Private Shared Sub ValidarTextos(cn As DbConnection, tx As DbTransaction, tabla As String, valores As Dictionary(Of String, Object), Optional compartidos As List(Of DetalleConfiguracionExcel) = Nothing, Optional fila As Nullable(Of Integer) = Nothing, Optional destino As String = Nothing)
        Dim errores = If(compartidos, New List(Of DetalleConfiguracionExcel))
        Using q = Cmd(cn, tx, "SELECT COLUMN_NAME,CHARACTER_MAXIMUM_LENGTH,IS_NULLABLE,CHARACTER_SET_NAME FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME=@tabla", "@tabla", tabla)
            Dim vistas As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
            Using r = q.ExecuteReader()
                While r.Read()
                    Dim nombre = Texto(r, 0)
                    If Not valores.ContainsKey(nombre) Then Continue While
                    vistas.Add(nombre)
                    Dim value = valores(nombre)
                    If value Is Nothing Then
                        If Texto(r, 2) <> "YES" Then errores.AddRange(ServicioPlantillasExcel.ErrorConfiguracion("CONFIGURACION_INVALIDA", nombre, "REQUERIDO", fila, destino).Detalles)
                        Continue While
                    End If
                    If Not TypeOf value Is String Then Continue While
                    Dim contenido = CStr(value)
                    If Not r.IsDBNull(1) AndAlso contenido.Length > Convert.ToInt64(r(1), CultureInfo.InvariantCulture) Then errores.AddRange(ServicioPlantillasExcel.ErrorConfiguracion("CONFIGURACION_INVALIDA", nombre, "LONGITUD_EXCEDIDA", fila, destino, CInt(Math.Min(Integer.MaxValue, Convert.ToInt64(r(1), CultureInfo.InvariantCulture))), contenido.Length).Detalles)
                    Dim charset = Texto(r, 3)
                    Try
                        ValidarCharset(contenido, charset)
                    Catch ex As ImportacionExcelException When ex.Codigo = "CONFIGURACION_INVALIDA"
                        errores.AddRange(ServicioPlantillasExcel.ErrorConfiguracion("CONFIGURACION_INVALIDA", nombre, "CARACTERES_INCOMPATIBLES", fila, destino).Detalles)
                    End Try
                End While
            End Using
            If valores.Keys.Any(Function(k) Not vistas.Contains(k)) Then Throw ServicioPlantillasExcel.ErrorControlado("CAPACIDAD_NO_DISPONIBLE")
        End Using
        If compartidos Is Nothing AndAlso errores.Count > 0 Then
            Dim rechazo = ServicioPlantillasExcel.ErrorControlado("CONFIGURACION_INVALIDA")
            rechazo.Detalles = errores : Throw rechazo
        End If
    End Sub
    Private Shared Function Texto(r As DbDataReader, i As Integer) As String
        Return If(r.IsDBNull(i), Nothing, Convert.ToString(r(i), CultureInfo.InvariantCulture))
    End Function
    Private Shared Sub ValidarCharset(contenido As String, charset As String)
        Try
            Dim utf8Estricto As New UTF8Encoding(False, True)
            Select Case charset
                Case "ascii"
                    Encoding.GetEncoding("us-ascii", EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback).GetBytes(contenido)
                Case "latin1"
                    'MySQL latin1 corresponde a Windows-1252, incluido el signo euro.
                    Encoding.GetEncoding(1252, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback).GetBytes(contenido)
                Case "utf8"
                    If contenido.Any(Function(c) Char.IsSurrogate(c)) Then Throw ServicioPlantillasExcel.ErrorControlado("CONFIGURACION_INVALIDA")
                    utf8Estricto.GetBytes(contenido)
                Case "utf8mb4"
                    utf8Estricto.GetBytes(contenido)
                Case Else
                    Throw ServicioPlantillasExcel.ErrorControlado("CAPACIDAD_NO_DISPONIBLE")
            End Select
        Catch ex As EncoderFallbackException
            Throw ServicioPlantillasExcel.ErrorControlado("CONFIGURACION_INVALIDA")
        End Try
    End Sub
    Private Shared Function UInt(r As DbDataReader, i As Integer) As UInteger
        Return Convert.ToUInt32(r(i), CultureInfo.InvariantCulture)
    End Function
    Private Shared Function UIntNulo(r As DbDataReader, i As Integer) As Nullable(Of UInteger)
        If r.IsDBNull(i) Then Return Nothing
        Return UInt(r, i)
    End Function
End Class
