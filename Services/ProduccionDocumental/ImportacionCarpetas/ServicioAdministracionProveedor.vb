Option Strict On
Imports System
Imports System.Security.Cryptography
Imports System.Text

Public Class ServicioAdministracionProveedor
    Private ReadOnly _repo As IAdministracionProveedorRepository
    Private ReadOnly _validar As Action(Of ContextoPerfilImportacion, ContratoProveedorIndices, Boolean)
    Private ReadOnly _consultar As IProveedorDatosIndices
    Public Sub New(repo As IAdministracionProveedorRepository, validar As Action(Of ContextoPerfilImportacion, ContratoProveedorIndices, Boolean), consultar As IProveedorDatosIndices)
        _repo = repo : _validar = validar : _consultar = consultar
    End Sub
    Public Function Ejecutar(c As ContextoPerfilImportacion, accion As String, s As SolicitudProveedorAdmin) As Object
        If c Is Nothing OrElse Not c.PuedeAdministrar OrElse Not c.PuedeAdministrarProveedores OrElse Not ValidadorPerfilImportacion.ReferenciaValida(c.UsuarioRef) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(c.AmbitoRef) Then Throw Fallo("ACCESO_DENEGADO")
        If s Is Nothing Then Throw Fallo("SOLICITUD_INVALIDA")
        If accion = "LISTAR" Then Return _repo.Listar(c, s)
        If accion = "CREAR" Then
            If Not ValidadorPerfilImportacion.TextoValido(s.Codigo, 64, False) OrElse Not System.Text.RegularExpressions.Regex.IsMatch(s.Codigo, "\A[A-Za-z0-9_-]+\z") OrElse Not ValidadorPerfilImportacion.TextoValido(s.Nombre, 160, False) Then Throw Fallo("SOLICITUD_INVALIDA")
            Return _repo.Mutar(c, accion, s, Nothing)
        End If
        If Not ValidadorPerfilImportacion.ReferenciaValida(s.ProveedorId) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(s.VersionId) Then Throw Fallo("SOLICITUD_INVALIDA")
        Dim actual = _repo.Obtener(c, s.ProveedorId, s.VersionId)
        If accion = "OBTENER" Then Return actual
        If actual.Revision <> s.Revision OrElse actual.RevisionProveedor <> s.RevisionProveedor Then Throw Fallo("CONFLICTO_REVISION")
        Select Case accion
            Case "GUARDAR"
                If actual.Estado <> "BORRADOR" Then Throw Fallo("CONFLICTO_REVISION")
                _validar(c, s.Contrato, False)
                Return _repo.Mutar(c, accion, s, Nothing)
            Case "VALIDAR", "PUBLICAR", "ENSAYAR"
                If accion = "PUBLICAR" AndAlso actual.Estado <> "BORRADOR" Then Throw Fallo("CONFLICTO_REVISION")
                _validar(c, actual.Contrato, True)
                If accion = "VALIDAR" Then Return actual
                If accion = "ENSAYAR" Then
                    If Not ValidadorPerfilImportacion.TextoValido(s.ClaveEjemplo, 4096, False) Then Throw Fallo("SOLICITUD_INVALIDA")
                    Dim v = VersionConsulta(c, actual)
                    Return _consultar.Consultar(c, v, s.ClaveEjemplo)
                End If
                Using sha = SHA256.Create()
                    Return _repo.Mutar(c, accion, s, sha.ComputeHash(Encoding.ASCII.GetBytes(PerfilImportacionJson.Escribir(actual.Contrato))))
                End Using
            Case "VERSION"
                If actual.Estado <> "PUBLICADA" Then Throw Fallo("CONFLICTO_REVISION")
                Return _repo.Mutar(c, accion, s, Nothing)
            Case "DISPONIBILIDAD"
                Return _repo.Mutar(c, accion, s, Nothing)
            Case Else
                Throw Fallo("SOLICITUD_INVALIDA")
        End Select
    End Function
    Public Shared Function VersionConsulta(c As ContextoPerfilImportacion, d As DocumentoProveedorAdmin) As ProveedorIndicesVersion
        'El ensayo administrativo puede probar borrador/inactivo; no lo publica ni habilita para perfiles.
        Return New ProveedorIndicesVersion With {.ProveedorId = d.ProveedorId, .VersionId = d.VersionId, .AmbitoRef = c.AmbitoRef, .Activo = True, .Estado = d.Estado, .ConexionRef = d.Contrato.ConexionRef, .Tabla = d.Contrato.Tabla, .ColumnaClave = d.Contrato.ColumnaClave, .TipoClave = d.Contrato.TipoClave, .ColumnaEmpresa = d.Contrato.ColumnaEmpresa, .Propiedades = d.Contrato.Propiedades}
    End Function
    Private Shared Function Fallo(codigo As String) As PerfilImportacionException
        Return New PerfilImportacionException(codigo, "Revise el estado y la configuracion del proveedor.")
    End Function
End Class
