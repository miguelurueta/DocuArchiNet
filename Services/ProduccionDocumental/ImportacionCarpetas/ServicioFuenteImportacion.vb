Option Strict On
Imports System

Public Class ServicioFuenteImportacion
    Private ReadOnly _repo As IFuenteImportacionRepository
    Private ReadOnly _metadatos As Action(Of ContextoPerfilImportacion, DocumentoFuenteImportacion)
    Public Sub New(repo As IFuenteImportacionRepository, metadatos As Action(Of ContextoPerfilImportacion, DocumentoFuenteImportacion))
        _repo = repo : _metadatos = metadatos
    End Sub
    Public Function Ejecutar(c As ContextoPerfilImportacion, accion As String, s As SolicitudFuenteImportacion) As Object
        ReglasFuenteImportacion.Autorizar(c)
        If s Is Nothing Then Throw New PerfilImportacionException("SOLICITUD_INVALIDA", "Solicitud requerida.")
        Select Case accion
            Case "LISTAR" : Return _repo.Listar(c, s.Pagina, s.Termino)
            Case "OBTENER" : Return _repo.Obtener(c, s.FuenteId)
            Case "HISTORIAL" : Return _repo.Historial(c, s.FuenteId, s.Pagina)
        End Select
        ReglasFuenteImportacion.Validar(accion, s.Cambio)
        Dim d = s.Cambio.Documento
        If accion <> "ALTA" Then
            Dim actual = _repo.Obtener(c, d.FuenteId)
            If actual.Revision <> d.Revision Then Throw New PerfilImportacionException("CONFLICTO_REVISION", "La fuente cambio. Consulte su estado.")
            If accion = "EDITAR" AndAlso (actual.ConexionRef <> d.ConexionRef OrElse actual.Tabla <> d.Tabla) Then Throw New PerfilImportacionException("IDENTIDAD_INMUTABLE", "Conexion y tabla no se pueden cambiar.")
            If accion = "ACTIVAR" Then _metadatos(c, actual)
        End If
        If accion = "ALTA" OrElse accion = "EDITAR" Then _metadatos(c, d)
        'El repositorio vuelve a comprobar revision bajo bloqueo para cerrar la carrera.
        Return _repo.Guardar(c, accion, s.Cambio)
    End Function
End Class
