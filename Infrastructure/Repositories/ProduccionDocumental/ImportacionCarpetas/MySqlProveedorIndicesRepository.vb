Option Strict On
Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports MySql.Data.MySqlClient

'Lectura del catalogo autorizado en RA. Registro tecnico separado del editor de perfiles.
Public Class MySqlProveedorIndicesRepository
    Implements IProveedorIndicesRepository
    Private ReadOnly _ra As String
    Public Sub New(conexionRA As String)
        _ra = conexionRA
    End Sub
    Public Function Listar(contexto As ContextoPerfilImportacion, pagina As Integer, tamano As Integer, proveedorId As String, versionId As String) As List(Of OpcionProveedorIndices) Implements IProveedorIndicesRepository.Listar
        If contexto Is Nothing OrElse Not contexto.PuedeAdministrar OrElse Not ValidadorPerfilImportacion.ReferenciaValida(contexto.UsuarioRef) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(contexto.AmbitoRef) Then Throw Denegado()
        If pagina < 1 OrElse pagina > 1000000 OrElse tamano < 1 OrElse tamano > 50 OrElse
            ((proveedorId Is Nothing) <> (versionId Is Nothing)) OrElse
            (proveedorId IsNot Nothing AndAlso (Not ValidadorPerfilImportacion.ReferenciaValida(proveedorId) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(versionId))) Then Throw New PerfilImportacionException("SOLICITUD_INVALIDA", "Filtro de proveedores invalido.")
        Dim resultado As New List(Of OpcionProveedorIndices)()
        Try
            Using conexion As New MySqlConnection(_ra)
                conexion.Open()
                Using cmd As New MySqlCommand("SELECT p.proveedor_id,p.nombre,v.version_id,v.numero,v.clave_tipo,v.propiedades_doc FROM ra_integracion_proveedor p INNER JOIN ra_integracion_proveedor_version v ON v.proveedor_id=p.proveedor_id WHERE p.ambito_ref=@ambito AND p.activo=1 AND v.estado='PUBLICADA' AND v.tipo='TABLA' AND v.esquema_config=1 AND (@proveedor IS NULL OR (p.proveedor_id=@proveedor AND v.version_id=@version)) ORDER BY p.proveedor_id,v.numero DESC LIMIT @limite OFFSET @offset", conexion)
                    cmd.CommandTimeout = 5
                    cmd.Parameters.AddWithValue("@ambito", contexto.AmbitoRef)
                    cmd.Parameters.AddWithValue("@proveedor", If(CType(proveedorId, Object), DBNull.Value))
                    cmd.Parameters.AddWithValue("@version", If(CType(versionId, Object), DBNull.Value))
                    cmd.Parameters.AddWithValue("@limite", tamano)
                    cmd.Parameters.AddWithValue("@offset", (pagina - 1) * tamano)
                    Using r = cmd.ExecuteReader()
                        While r.Read()
                            Dim opcion As New OpcionProveedorIndices With {.ProveedorId = Convert.ToString(r("proveedor_id"), CultureInfo.InvariantCulture), .VersionId = Convert.ToString(r("version_id"), CultureInfo.InvariantCulture), .Nombre = CStr(r("nombre")), .Numero = Convert.ToUInt32(r("numero")), .TipoClave = CStr(r("clave_tipo"))}
                            Dim propiedades = PerfilImportacionJson.Leer(Of List(Of PropiedadProveedorIndices))(CStr(r("propiedades_doc")))
                            If propiedades Is Nothing OrElse propiedades.Count > 128 Then Throw New PerfilImportacionException("CONFIGURACION_INVALIDA", "Contrato del proveedor invalido.")
                            For Each p In propiedades
                                If p Is Nothing Then Throw New PerfilImportacionException("CONFIGURACION_INVALIDA", "Contrato del proveedor invalido.")
                                opcion.Propiedades.Add(New OpcionPropiedadProveedorIndices With {.Nombre = p.Nombre, .Tipo = p.Tipo})
                            Next
                            resultado.Add(opcion)
                        End While
                    End Using
                End Using
            End Using
        Catch ex As MySqlException
            Throw New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "No se pudo consultar el catalogo de proveedores.")
        End Try
        Return resultado
    End Function
    Public Function Obtener(contexto As ContextoPerfilImportacion, proveedorId As String, versionId As String) As ProveedorIndicesVersion Implements IProveedorIndicesRepository.Obtener
        If contexto Is Nothing OrElse Not contexto.PuedeAdministrar OrElse
            Not ValidadorPerfilImportacion.ReferenciaValida(contexto.UsuarioRef) OrElse
            Not ValidadorPerfilImportacion.ReferenciaValida(contexto.AmbitoRef) OrElse
            Not ValidadorPerfilImportacion.ReferenciaValida(proveedorId) OrElse
            Not ValidadorPerfilImportacion.ReferenciaValida(versionId) Then Throw Denegado()
        Try
            Using conexion As New MySqlConnection(_ra)
                conexion.Open()
                Using cmd As New MySqlCommand("SELECT p.ambito_ref,v.tipo,v.conexion_ref,v.tabla_nombre,v.clave_columna,v.clave_tipo,v.empresa_columna,v.estado,v.revision,v.esquema_config,v.propiedades_doc FROM ra_integracion_proveedor p INNER JOIN ra_integracion_proveedor_version v ON v.proveedor_id=p.proveedor_id WHERE p.proveedor_id=@proveedor AND v.version_id=@version AND p.ambito_ref=@ambito AND p.activo=1 AND v.estado='PUBLICADA' LIMIT 2", conexion)
                    cmd.CommandTimeout = 5
                    cmd.Parameters.AddWithValue("@proveedor", proveedorId)
                    cmd.Parameters.AddWithValue("@version", versionId)
                    cmd.Parameters.AddWithValue("@ambito", contexto.AmbitoRef)
                    Using reader = cmd.ExecuteReader()
                        If Not reader.Read() Then Throw Denegado()
                        If Convert.ToInt32(reader("esquema_config"), CultureInfo.InvariantCulture) <> 1 Then Throw New PerfilImportacionException("CONFIGURACION_INVALIDA", "Esquema del proveedor no compatible.")
                        Dim v As New ProveedorIndicesVersion With {
                            .Estado = CStr(reader("estado")), .Revision = Convert.ToUInt32(reader("revision")), .ProveedorId = proveedorId, .VersionId = versionId, .AmbitoRef = CStr(reader("ambito_ref")), .Activo = True,
                            .Tipo = CStr(reader("tipo")), .ConexionRef = CStr(reader("conexion_ref")), .Tabla = CStr(reader("tabla_nombre")),
                            .ColumnaClave = CStr(reader("clave_columna")), .TipoClave = CStr(reader("clave_tipo")), .ColumnaEmpresa = CStr(reader("empresa_columna")),
                            .Propiedades = PerfilImportacionJson.Leer(Of List(Of PropiedadProveedorIndices))(CStr(reader("propiedades_doc")))}
                        If reader.Read() OrElse Not MySqlProveedorDatosIndices.ConfiguracionValida(v) Then Throw New PerfilImportacionException("CONFIGURACION_INVALIDA", "Configuracion del proveedor no valida.")
                        Return v
                    End Using
                End Using
            End Using
        Catch ex As MySqlException
            Throw New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "No se pudo consultar el catalogo de proveedores.")
        End Try
    End Function
    Private Shared Function Denegado() As PerfilImportacionException
        Return New PerfilImportacionException("ACCESO_DENEGADO", "Proveedor no disponible para el contexto.")
    End Function
End Class
