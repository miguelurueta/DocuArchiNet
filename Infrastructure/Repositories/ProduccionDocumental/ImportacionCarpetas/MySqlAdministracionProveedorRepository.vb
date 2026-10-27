Option Strict On
Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports MySql.Data.MySqlClient

Public Class MySqlAdministracionProveedorRepository
    Implements IAdministracionProveedorRepository
    Private ReadOnly _ra As String
    Private Const Seleccion As String = "SELECT p.proveedor_id,p.codigo,p.nombre,p.activo,p.revision AS revision_proveedor,v.version_id,v.numero,v.estado,v.revision,v.conexion_ref,v.tabla_nombre,v.clave_columna,v.clave_tipo,v.empresa_columna,v.propiedades_doc FROM ra_integracion_proveedor p INNER JOIN ra_integracion_proveedor_version v ON v.proveedor_id=p.proveedor_id "
    Public Sub New(ra As String)
        _ra = ra
    End Sub
    Public Function Listar(c As ContextoPerfilImportacion, s As SolicitudProveedorAdmin) As List(Of DocumentoProveedorAdmin) Implements IAdministracionProveedorRepository.Listar
        Autorizar(c)
        If s Is Nothing OrElse s.Pagina < 1 OrElse s.Pagina > 1000000 OrElse Not ValidadorPerfilImportacion.TextoValido(s.Termino, 160, True) Then Throw ErrorNegocio("SOLICITUD_INVALIDA")
        If s.ProveedorId IsNot Nothing AndAlso Not ValidadorPerfilImportacion.ReferenciaValida(s.ProveedorId) Then Throw ErrorNegocio("SOLICITUD_INVALIDA")
        Try
            Using cn As New MySqlConnection(_ra)
                cn.Open()
                Using cmd = Comando(cn, Nothing, Seleccion & "WHERE p.ambito_ref=@ambito AND (@proveedor IS NULL OR p.proveedor_id=@proveedor) AND (@termino='' OR LOCATE(@termino,p.codigo)>0 OR LOCATE(@termino,p.nombre)>0) ORDER BY p.proveedor_id DESC,v.numero DESC LIMIT 25 OFFSET @offset", "@ambito", c.AmbitoRef, "@proveedor", s.ProveedorId, "@termino", If(s.Termino, ""), "@offset", (s.Pagina - 1) * 25)
                    Dim salida As New List(Of DocumentoProveedorAdmin)()
                    Using r = cmd.ExecuteReader()
                        While r.Read()
                            salida.Add(Mapear(r))
                        End While
                    End Using
                    Return salida
                End Using
            End Using
        Catch ex As MySqlException
            Throw ErrorNegocio("CAPACIDAD_NO_DISPONIBLE")
        End Try
    End Function
    Public Function Obtener(c As ContextoPerfilImportacion, proveedorId As String, versionId As String) As DocumentoProveedorAdmin Implements IAdministracionProveedorRepository.Obtener
        Autorizar(c) : Id(proveedorId) : Id(versionId)
        Try
            Using cn As New MySqlConnection(_ra)
                cn.Open()
                Return Leer(cn, Nothing, c, proveedorId, versionId, False)
            End Using
        Catch ex As MySqlException
            Throw ErrorNegocio("CAPACIDAD_NO_DISPONIBLE")
        End Try
    End Function
    Public Function Mutar(c As ContextoPerfilImportacion, accion As String, s As SolicitudProveedorAdmin, huella As Byte()) As DocumentoProveedorAdmin Implements IAdministracionProveedorRepository.Mutar
        Autorizar(c)
        Try
            Using cn As New MySqlConnection(_ra)
                cn.Open()
                Using tx = cn.BeginTransaction()
                    Dim proveedorId = s.ProveedorId, versionId = s.VersionId
                    If accion = "CREAR" Then
                        Using cmd = Comando(cn, tx, "INSERT INTO ra_integracion_proveedor(ambito_ref,codigo,nombre,activo,revision,creado_por,creado_utc) VALUES(@ambito,@codigo,@nombre,0,1,@usuario,@utc)", "@ambito", c.AmbitoRef, "@codigo", s.Codigo, "@nombre", s.Nombre, "@usuario", c.UsuarioRef, "@utc", DateTime.UtcNow)
                            cmd.ExecuteNonQuery() : proveedorId = cmd.LastInsertedId.ToString(CultureInfo.InvariantCulture)
                        End Using
                        versionId = Insertar(cn, tx, proveedorId, 1UI, New ContratoProveedorIndices(), c.UsuarioRef)
                    Else
                        Id(proveedorId) : Id(versionId)
                        Dim actual = Leer(cn, tx, c, proveedorId, versionId, True)
                        If actual.Revision <> s.Revision OrElse actual.RevisionProveedor <> s.RevisionProveedor Then Throw ErrorNegocio("CONFLICTO_REVISION")
                        Select Case accion
                            Case "GUARDAR"
                                If actual.Estado <> "BORRADOR" Then Throw ErrorNegocio("CONFLICTO_REVISION")
                                Dim contrato = s.Contrato
                                Using cmd = Comando(cn, tx, "UPDATE ra_integracion_proveedor_version SET conexion_ref=@conexion,tabla_nombre=@tabla,clave_columna=@clave,clave_tipo=@tipo,empresa_columna=@empresa,propiedades_doc=@doc,revision=revision+1,actualizado_por=@usuario,actualizado_utc=@utc WHERE version_id=@version", "@conexion", If(contrato.ConexionRef, ""), "@tabla", If(contrato.Tabla, ""), "@clave", If(contrato.ColumnaClave, ""), "@tipo", If(contrato.TipoClave, "TEXTO"), "@empresa", If(contrato.ColumnaEmpresa, ""), "@doc", PerfilImportacionJson.Escribir(contrato.Propiedades), "@usuario", c.UsuarioRef, "@utc", DateTime.UtcNow, "@version", versionId)
                                    cmd.ExecuteNonQuery()
                                End Using
                            Case "PUBLICAR"
                                If actual.Estado <> "BORRADOR" OrElse huella Is Nothing OrElse huella.Length <> 32 Then Throw ErrorNegocio("CONFLICTO_REVISION")
                                Using cmd = Comando(cn, tx, "UPDATE ra_integracion_proveedor_version SET estado='PUBLICADA',revision=revision+1,publicado_por=@usuario,publicado_utc=@utc,contrato_hash=@hash WHERE version_id=@version", "@usuario", c.UsuarioRef, "@utc", DateTime.UtcNow, "@hash", huella, "@version", versionId)
                                    cmd.ExecuteNonQuery()
                                End Using
                            Case "VERSION"
                                If actual.Estado <> "PUBLICADA" Then Throw ErrorNegocio("CONFLICTO_REVISION")
                                Dim numero As UInteger
                                Using cmd = Comando(cn, tx, "SELECT MAX(numero) FROM ra_integracion_proveedor_version WHERE proveedor_id=@proveedor", "@proveedor", proveedorId)
                                    numero = Convert.ToUInt32(cmd.ExecuteScalar()) + 1UI
                                End Using
                                versionId = Insertar(cn, tx, proveedorId, numero, actual.Contrato, c.UsuarioRef)
                            Case "DISPONIBILIDAD"
                                Using cmd = Comando(cn, tx, "UPDATE ra_integracion_proveedor SET activo=@activo WHERE proveedor_id=@proveedor", "@activo", If(s.Activo, 1, 0), "@proveedor", proveedorId)
                                    cmd.ExecuteNonQuery()
                                End Using
                            Case Else
                                Throw ErrorNegocio("SOLICITUD_INVALIDA")
                        End Select
                        Using cmd = Comando(cn, tx, "UPDATE ra_integracion_proveedor SET revision=revision+1,actualizado_por=@usuario,actualizado_utc=@utc WHERE proveedor_id=@proveedor", "@usuario", c.UsuarioRef, "@utc", DateTime.UtcNow, "@proveedor", proveedorId)
                            cmd.ExecuteNonQuery()
                        End Using
                    End If
                    Dim resultado = Leer(cn, tx, c, proveedorId, versionId, False)
                    tx.Commit()
                    Return resultado
                End Using
            End Using
        Catch ex As MySqlException
            Throw ErrorNegocio(If(ex.Number = 1062 OrElse ex.Number = 1213 OrElse ex.Number = 1205, "CONFLICTO_REVISION", "CAPACIDAD_NO_DISPONIBLE"))
        End Try
    End Function
    Private Shared Function Insertar(cn As MySqlConnection, tx As MySqlTransaction, proveedor As String, numero As UInteger, c As ContratoProveedorIndices, usuario As String) As String
        Using cmd = Comando(cn, tx, "INSERT INTO ra_integracion_proveedor_version(proveedor_id,numero,tipo,conexion_ref,tabla_nombre,clave_columna,clave_tipo,empresa_columna,esquema_config,propiedades_doc,creado_por,creado_utc,estado,revision) VALUES(@proveedor,@numero,'TABLA',@conexion,@tabla,@clave,@tipo,@empresa,1,@doc,@usuario,@utc,'BORRADOR',1)", "@proveedor", proveedor, "@numero", numero, "@conexion", If(c.ConexionRef, ""), "@tabla", If(c.Tabla, ""), "@clave", If(c.ColumnaClave, ""), "@tipo", If(c.TipoClave, "TEXTO"), "@empresa", If(c.ColumnaEmpresa, ""), "@doc", PerfilImportacionJson.Escribir(c.Propiedades), "@usuario", usuario, "@utc", DateTime.UtcNow)
            cmd.ExecuteNonQuery()
            Return cmd.LastInsertedId.ToString(CultureInfo.InvariantCulture)
        End Using
    End Function
    Private Shared Function Leer(cn As MySqlConnection, tx As MySqlTransaction, c As ContextoPerfilImportacion, proveedor As String, version As String, bloquear As Boolean) As DocumentoProveedorAdmin
        If bloquear Then
            'Orden uniforme: cabecera primero, version despues. Serializa nuevos numeros.
            Using cmd = Comando(cn, tx, "SELECT proveedor_id FROM ra_integracion_proveedor WHERE proveedor_id=@proveedor AND ambito_ref=@ambito FOR UPDATE", "@proveedor", proveedor, "@ambito", c.AmbitoRef)
                If cmd.ExecuteScalar() Is Nothing Then Throw ErrorNegocio("ACCESO_DENEGADO")
            End Using
        End If
        Using cmd = Comando(cn, tx, Seleccion & "WHERE p.proveedor_id=@proveedor AND v.version_id=@version AND p.ambito_ref=@ambito" & If(bloquear, " FOR UPDATE", ""), "@proveedor", proveedor, "@version", version, "@ambito", c.AmbitoRef)
            Using r = cmd.ExecuteReader()
                If Not r.Read() Then Throw ErrorNegocio("ACCESO_DENEGADO")
                Return Mapear(r)
            End Using
        End Using
    End Function
    Private Shared Function Mapear(r As MySqlDataReader) As DocumentoProveedorAdmin
        Return New DocumentoProveedorAdmin With {.ProveedorId = Convert.ToString(r("proveedor_id"), CultureInfo.InvariantCulture), .VersionId = Convert.ToString(r("version_id"), CultureInfo.InvariantCulture), .Codigo = CStr(r("codigo")), .Nombre = CStr(r("nombre")), .Activo = Convert.ToInt32(r("activo")) = 1, .RevisionProveedor = Convert.ToUInt32(r("revision_proveedor")), .Numero = Convert.ToUInt32(r("numero")), .Estado = CStr(r("estado")), .Revision = Convert.ToUInt32(r("revision")), .Contrato = New ContratoProveedorIndices With {.ConexionRef = CStr(r("conexion_ref")), .Tabla = CStr(r("tabla_nombre")), .ColumnaClave = CStr(r("clave_columna")), .TipoClave = CStr(r("clave_tipo")), .ColumnaEmpresa = CStr(r("empresa_columna")), .Propiedades = PerfilImportacionJson.Leer(Of List(Of PropiedadProveedorIndices))(CStr(r("propiedades_doc")))}}
    End Function
    Private Shared Function Comando(cn As MySqlConnection, tx As MySqlTransaction, sql As String, ParamArray p As Object()) As MySqlCommand
        Dim cmd As New MySqlCommand(sql, cn, tx) With {.CommandTimeout = 5}
        For i = 0 To p.Length - 1 Step 2
            cmd.Parameters.AddWithValue(CStr(p(i)), If(p(i + 1), DBNull.Value))
        Next
        Return cmd
    End Function
    Private Shared Sub Autorizar(c As ContextoPerfilImportacion)
        If c Is Nothing OrElse Not c.PuedeAdministrarProveedores OrElse Not c.PuedeAdministrar OrElse Not ValidadorPerfilImportacion.ReferenciaValida(c.UsuarioRef) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(c.AmbitoRef) Then Throw ErrorNegocio("ACCESO_DENEGADO")
    End Sub
    Private Shared Sub Id(valor As String)
        If Not ValidadorPerfilImportacion.ReferenciaValida(valor) Then Throw ErrorNegocio("SOLICITUD_INVALIDA")
    End Sub
    Private Shared Function ErrorNegocio(codigo As String) As PerfilImportacionException
        Return New PerfilImportacionException(codigo, "No se pudo completar la operacion del proveedor. Consulte su estado antes de reintentar una escritura.")
    End Function
End Class
