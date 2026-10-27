Option Strict On
Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Linq
Imports MySql.Data.MySqlClient

Public Class MySqlFuenteImportacionRepository
    Implements IFuenteImportacionRepository
    Private ReadOnly _ra As String
    Private Const Cabecera As String = "SELECT fuente_id,conexion_ref,tabla_nombre,modo_aislamiento,columna_empresa,activo,revision FROM ra_integracion_fuente "
    Public Sub New(ra As String)
        _ra = ra
    End Sub
    Public Function CargarPolitica(c As ContextoPerfilImportacion, Optional tablas As RestriccionTablasFuente = Nothing) As PoliticaFuentesProveedor
        If c Is Nothing OrElse Not c.PuedeAdministrar OrElse Not ValidadorPerfilImportacion.ReferenciaValida(c.UsuarioRef) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(c.AmbitoRef) Then Throw Fallo("ACCESO_DENEGADO")
        Return Transaccion(Function(cn, tx)
                               Dim fuentes As New Dictionary(Of String, FuenteProveedorAutorizada)(StringComparer.Ordinal)
                               Using cmd = Comando(cn, tx, "SELECT f.fuente_id,f.conexion_ref,f.tabla_nombre,f.modo_aislamiento,f.columna_empresa,c.nombre FROM ra_integracion_fuente f LEFT JOIN ra_integracion_fuente_columna c ON c.fuente_id=f.fuente_id WHERE f.ambito_ref=@ambito AND f.activo=1 ORDER BY f.fuente_id,c.nombre", "@ambito", c.AmbitoRef)
                                   Using r = cmd.ExecuteReader()
                                       While r.Read()
                                           Dim id = Convert.ToString(r(0), CultureInfo.InvariantCulture)
                                           If Not fuentes.ContainsKey(id) Then
                                               Dim modo = CStr(r(3))
                                               If modo <> "CONEXION" AndAlso modo <> "COLUMNA" Then Throw Fallo("CONFIGURACION_INVALIDA")
                                               fuentes.Add(id, New FuenteProveedorAutorizada With {.AmbitoRef = c.AmbitoRef, .ConexionRef = CStr(r(1)), .Tabla = CStr(r(2)), .ExclusivaEmpresa = modo = "CONEXION", .ColumnaEmpresaAutorizada = CStr(r(4))})
                                               If fuentes.Count > 256 Then Throw Fallo("CAPACIDAD_NO_DISPONIBLE")
                                           End If
                                           If Not r.IsDBNull(5) Then fuentes(id).Columnas.Add(CStr(r(5)))
                                           If fuentes(id).Columnas.Count > 128 Then Throw Fallo("CONFIGURACION_INVALIDA")
                                       End While
                                   End Using
                               End Using
                               Return New PoliticaFuentesProveedor(PerfilImportacionJson.Escribir(fuentes.Values.ToList()), tablas)
                           End Function)
    End Function
    Public Function Listar(c As ContextoPerfilImportacion, pagina As Integer, termino As String) As List(Of DocumentoFuenteImportacion) Implements IFuenteImportacionRepository.Listar
        ReglasFuenteImportacion.Autorizar(c) : ReglasFuenteImportacion.Pagina(pagina)
        If Not ValidadorPerfilImportacion.TextoValido(termino, 64, True) Then Throw Fallo("SOLICITUD_INVALIDA")
        Return Transaccion(Function(cn, tx)
                               Dim lista As New List(Of DocumentoFuenteImportacion)()
                               Using cmd = Comando(cn, tx, Cabecera & "WHERE ambito_ref=@ambito AND (@termino='' OR LOCATE(@termino,tabla_nombre)>0) ORDER BY fuente_id DESC LIMIT 25 OFFSET @offset", "@ambito", c.AmbitoRef, "@termino", If(termino, ""), "@offset", (pagina - 1) * 25)
                                   Using r = cmd.ExecuteReader()
                                       While r.Read()
                                           lista.Add(Mapear(r))
                                       End While
                                   End Using
                               End Using
                               For Each d In lista
                                   Columnas(cn, tx, d)
                               Next
                               Return lista
                           End Function)
    End Function
    Public Function Obtener(c As ContextoPerfilImportacion, fuenteId As String) As DocumentoFuenteImportacion Implements IFuenteImportacionRepository.Obtener
        ReglasFuenteImportacion.Autorizar(c) : ReglasFuenteImportacion.Id(fuenteId)
        Return Transaccion(Function(cn, tx) Leer(cn, tx, c, fuenteId, False))
    End Function
    Public Function Guardar(c As ContextoPerfilImportacion, accion As String, cambio As CambioFuenteImportacion) As DocumentoFuenteImportacion Implements IFuenteImportacionRepository.Guardar
        ReglasFuenteImportacion.Autorizar(c) : ReglasFuenteImportacion.Validar(accion, cambio)
        Return Transaccion(Function(cn, tx)
                               'Comprobar historial antes de la primera escritura: fallo conocido, no resultado incierto.
                               Try
                                   Using cmd = Comando(cn, tx, "SELECT fuente_id,revision,accion,actor_ref,fecha_utc,motivo,estado_doc FROM ra_integracion_fuente_historial WHERE 1=0")
                                       Using r = cmd.ExecuteReader()
                                       End Using
                                   End Using
                               Catch ex As MySqlException
                                   If ex.Number = 1146 OrElse ex.Number = 1054 Then Throw New PerfilImportacionException("ESQUEMA_FUENTES_INCOMPLETO", "No se guardo la fuente. Falta completar la tabla de historial del SQL 07 en RA. Solicite completar la instalacion antes de volver a guardar.")
                                   Throw
                               End Try
                               Dim d = cambio.Documento, id = d.FuenteId, ahora = DateTime.UtcNow
                               If accion = "ALTA" Then
                                   Using cmd = Comando(cn, tx, "INSERT INTO ra_integracion_fuente(ambito_ref,conexion_ref,tabla_nombre,modo_aislamiento,columna_empresa,activo,revision,creado_por,creado_utc) VALUES(@ambito,@conexion,@tabla,@modo,@empresa,0,1,@actor,@utc)", "@ambito", c.AmbitoRef, "@conexion", d.ConexionRef, "@tabla", d.Tabla, "@modo", d.ModoAislamiento, "@empresa", If(d.ColumnaEmpresa, ""), "@actor", c.UsuarioRef, "@utc", ahora)
                                       cmd.ExecuteNonQuery() : id = cmd.LastInsertedId.ToString(CultureInfo.InvariantCulture)
                                   End Using
                               Else
                                   Dim actual = Leer(cn, tx, c, id, True)
                                   If actual.Revision <> d.Revision Then Throw Fallo("CONFLICTO_REVISION")
                                   If accion = "EDITAR" AndAlso (actual.ConexionRef <> d.ConexionRef OrElse actual.Tabla <> d.Tabla) Then Throw Fallo("IDENTIDAD_INMUTABLE")
                                   If accion = "EDITAR" Then
                                       Using cmd = Comando(cn, tx, "UPDATE ra_integracion_fuente SET modo_aislamiento=@modo,columna_empresa=@empresa WHERE fuente_id=@id", "@modo", d.ModoAislamiento, "@empresa", If(d.ColumnaEmpresa, ""), "@id", id)
                                           cmd.ExecuteNonQuery()
                                       End Using
                                   Else
                                       Using cmd = Comando(cn, tx, "UPDATE ra_integracion_fuente SET activo=@activo WHERE fuente_id=@id", "@activo", If(accion = "ACTIVAR", 1, 0), "@id", id)
                                           cmd.ExecuteNonQuery()
                                       End Using
                                   End If
                                   Using cmd = Comando(cn, tx, "UPDATE ra_integracion_fuente SET revision=revision+1,actualizado_por=@actor,actualizado_utc=@utc WHERE fuente_id=@id", "@actor", c.UsuarioRef, "@utc", ahora, "@id", id)
                                       cmd.ExecuteNonQuery()
                                   End Using
                               End If
                               If accion = "ALTA" OrElse accion = "EDITAR" Then
                                   Using cmd = Comando(cn, tx, "DELETE FROM ra_integracion_fuente_columna WHERE fuente_id=@id", "@id", id)
                                       cmd.ExecuteNonQuery()
                                   End Using
                                   For Each nombre In d.Columnas
                                       Using cmd = Comando(cn, tx, "INSERT INTO ra_integracion_fuente_columna(fuente_id,nombre) VALUES(@id,@nombre)", "@id", id, "@nombre", nombre)
                                           cmd.ExecuteNonQuery()
                                       End Using
                                   Next
                               End If
                               Dim resultado = Leer(cn, tx, c, id, False)
                               Using cmd = Comando(cn, tx, "INSERT INTO ra_integracion_fuente_historial(fuente_id,revision,accion,actor_ref,fecha_utc,motivo,estado_doc) VALUES(@id,@revision,@accion,@actor,@utc,@motivo,@doc)", "@id", id, "@revision", resultado.Revision, "@accion", accion, "@actor", c.UsuarioRef, "@utc", ahora, "@motivo", cambio.Motivo, "@doc", PerfilImportacionJson.Escribir(resultado))
                                   cmd.ExecuteNonQuery()
                               End Using
                               Return resultado
                           End Function)
    End Function
    Public Function Historial(c As ContextoPerfilImportacion, fuenteId As String, pagina As Integer) As List(Of HistorialFuenteImportacion) Implements IFuenteImportacionRepository.Historial
        ReglasFuenteImportacion.Autorizar(c) : ReglasFuenteImportacion.Id(fuenteId) : ReglasFuenteImportacion.Pagina(pagina)
        Return Transaccion(Function(cn, tx)
                               Leer(cn, tx, c, fuenteId, False)
                               Dim lista As New List(Of HistorialFuenteImportacion)()
                               Using cmd = Comando(cn, tx, "SELECT historial_id,revision,accion,actor_ref,fecha_utc,motivo,estado_doc FROM ra_integracion_fuente_historial WHERE fuente_id=@id ORDER BY revision DESC LIMIT 25 OFFSET @offset", "@id", fuenteId, "@offset", (pagina - 1) * 25)
                                   Using r = cmd.ExecuteReader()
                                       While r.Read()
                                           lista.Add(New HistorialFuenteImportacion With {.HistorialId = Convert.ToString(r("historial_id"), CultureInfo.InvariantCulture), .Revision = Convert.ToUInt32(r("revision")), .Accion = CStr(r("accion")), .ActorRef = CStr(r("actor_ref")), .FechaUtc = DateTime.SpecifyKind(Convert.ToDateTime(r("fecha_utc")), DateTimeKind.Utc), .Motivo = CStr(r("motivo")), .Estado = PerfilImportacionJson.Leer(Of DocumentoFuenteImportacion)(CStr(r("estado_doc")))})
                                       End While
                                   End Using
                               End Using
                               Return lista
                           End Function)
    End Function
    Private Function Transaccion(Of T)(accion As Func(Of MySqlConnection, MySqlTransaction, T)) As T
        Try
            Using cn As New MySqlConnection(_ra)
                cn.Open()
                Using tx = cn.BeginTransaction(System.Data.IsolationLevel.RepeatableRead)
                    Dim resultado = accion(cn, tx)
                    tx.Commit()
                    Return resultado
                End Using
            End Using
        Catch ex As MySqlException
            Throw Fallo(If({1062, 1205, 1213}.Contains(ex.Number), "CONFLICTO_REVISION", "CAPACIDAD_NO_DISPONIBLE"))
        End Try
    End Function
    Private Shared Function Leer(cn As MySqlConnection, tx As MySqlTransaction, c As ContextoPerfilImportacion, id As String, bloquear As Boolean) As DocumentoFuenteImportacion
        Dim d As DocumentoFuenteImportacion
        Using cmd = Comando(cn, tx, Cabecera & "WHERE fuente_id=@id AND ambito_ref=@ambito" & If(bloquear, " FOR UPDATE", ""), "@id", id, "@ambito", c.AmbitoRef)
            Using r = cmd.ExecuteReader()
                If Not r.Read() Then Throw Fallo("ACCESO_DENEGADO")
                d = Mapear(r)
            End Using
        End Using
        Columnas(cn, tx, d)
        Return d
    End Function
    Private Shared Sub Columnas(cn As MySqlConnection, tx As MySqlTransaction, d As DocumentoFuenteImportacion)
        Using cmd = Comando(cn, tx, "SELECT nombre FROM ra_integracion_fuente_columna WHERE fuente_id=@id ORDER BY nombre", "@id", d.FuenteId)
            Using r = cmd.ExecuteReader()
                While r.Read()
                    d.Columnas.Add(CStr(r(0)))
                End While
            End Using
        End Using
    End Sub
    Private Shared Function Mapear(r As MySqlDataReader) As DocumentoFuenteImportacion
        Return New DocumentoFuenteImportacion With {.FuenteId = Convert.ToString(r("fuente_id"), CultureInfo.InvariantCulture), .ConexionRef = CStr(r("conexion_ref")), .Tabla = CStr(r("tabla_nombre")), .ModoAislamiento = CStr(r("modo_aislamiento")), .ColumnaEmpresa = CStr(r("columna_empresa")), .Activo = Convert.ToInt32(r("activo")) = 1, .Revision = Convert.ToUInt32(r("revision"))}
    End Function
    Private Shared Function Comando(cn As MySqlConnection, tx As MySqlTransaction, sql As String, ParamArray p As Object()) As MySqlCommand
        Dim cmd As New MySqlCommand(sql, cn, tx) With {.CommandTimeout = 5}
        For i = 0 To p.Length - 1 Step 2
            cmd.Parameters.AddWithValue(CStr(p(i)), If(p(i + 1), DBNull.Value))
        Next
        Return cmd
    End Function
    Private Shared Function Fallo(codigo As String) As PerfilImportacionException
        Return New PerfilImportacionException(codigo, "No se pudo completar la operacion de fuente. Consulte el estado antes de repetir una escritura.")
    End Function
End Class
