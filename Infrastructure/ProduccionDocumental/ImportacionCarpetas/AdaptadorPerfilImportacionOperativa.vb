Option Strict On
Imports System
Imports System.Collections.Generic
Imports System.Linq

'Compone solo puertos de lectura operativa. No conoce guardar/publicar ni eleva
'un ContextoPerfilImportacion para reutilizar su autorizacion administrativa.
Public Class AdaptadorPerfilImportacionOperativa
    Private ReadOnly _perfiles As IPerfilImportacionLecturaOperativa
    Private ReadOnly _catalogos As IPerfilImportacionCatalogosOperativos
    Private ReadOnly _proveedores As Func(Of ContextoImportacionCarpetas, List(Of ReglaPerfilImportacion), List(Of IncidenciaPerfilImportacion))

    Public Sub New(perfiles As IPerfilImportacionLecturaOperativa, catalogos As IPerfilImportacionCatalogosOperativos, Optional proveedores As Func(Of ContextoImportacionCarpetas, List(Of ReglaPerfilImportacion), List(Of IncidenciaPerfilImportacion)) = Nothing)
        If perfiles Is Nothing OrElse catalogos Is Nothing Then Throw New ArgumentNullException()
        _perfiles = perfiles : _catalogos = catalogos : _proveedores = proveedores
    End Sub

    Public Function Seleccionar(contexto As ContextoImportacionCarpetas, versionId As String) As SnapshotPerfilImportacionOperativa
        Autorizar(contexto)
        If Not ValidadorPerfilImportacion.ReferenciaValida(versionId) Then Throw Denegado()
        _perfiles.VerificarEsquema()
        Return ValidarSnapshot(contexto, _perfiles.ObtenerPublicacion(contexto, versionId), versionId)
    End Function

    Public Function Listar(contexto As ContextoImportacionCarpetas, pagina As Integer, tamano As Integer) As List(Of SnapshotPerfilImportacionOperativa)
        Return ListarPagina(contexto, pagina, tamano).perfiles
    End Function

    Public Function ListarPagina(contexto As ContextoImportacionCarpetas, pagina As Integer, tamano As Integer) As PaginaPerfilesImportacionOperativa
        Autorizar(contexto)
        If pagina < 1 OrElse pagina > 1000000 OrElse tamano < 1 OrElse tamano > 50 Then Throw New PerfilImportacionException("SOLICITUD_INVALIDA", "Paginación inválida.")
        _perfiles.VerificarEsquema()
        Dim salida As New List(Of SnapshotPerfilImportacionOperativa)()
        Dim originales = _perfiles.ListarPublicaciones(contexto, pagina, tamano)
        If originales Is Nothing OrElse originales.Count > tamano Then Throw NoDisponible()
        For Each snapshot In originales
            If snapshot Is Nothing Then Throw NoDisponible()
            Try
                salida.Add(ValidarSnapshot(contexto, snapshot, snapshot.VersionId))
            Catch ex As PerfilImportacionException When ex.Codigo = "ACCESO_DENEGADO" OrElse ex.Codigo = "CONFIGURACION_INVALIDA"
                'No se anuncia una publicacion incompatible. Un origen no
                'disponible no se disfraza como una pagina vacia.
            End Try
        Next
        Return New PaginaPerfilesImportacionOperativa With {.pagina = pagina, .tamanoPagina = tamano, .puedeHaberMas = originales.Count = tamano, .perfiles = salida}
    End Function

    Private Function ValidarSnapshot(contexto As ContextoImportacionCarpetas, original As SnapshotPerfilImportacionOperativa, versionId As String) As SnapshotPerfilImportacionOperativa
        If original Is Nothing OrElse original.VersionId <> versionId Then Throw Denegado()
        Dim snapshot = PerfilImportacionJson.Leer(Of SnapshotPerfilImportacionOperativa)(PerfilImportacionJson.Escribir(original))
        If snapshot.Configuracion Is Nothing OrElse snapshot.Reglas Is Nothing OrElse snapshot.Reglas.Any(Function(r) r Is Nothing) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(snapshot.PerfilId) OrElse snapshot.NumeroVersion = 0 Then Throw NoDisponible()
        If Not _catalogos.NivelCompatibleOperativo(contexto, snapshot.Configuracion, contexto.NivelRef) Then Throw Denegado()
        Dim errores = _catalogos.ValidarOperativo(contexto, snapshot.Configuracion, snapshot.Reglas)
        If errores Is Nothing Then Throw NoDisponible()
        If snapshot.Reglas.Any(Function(r) r.FuenteTipo = "PROVEEDOR") Then
            If _proveedores Is Nothing Then Throw NoDisponible()
            Dim fuentes = _proveedores(contexto, snapshot.Reglas)
            If fuentes Is Nothing Then Throw NoDisponible()
            errores.AddRange(fuentes)
        End If
        If errores.Count <> 0 Then Throw New PerfilImportacionException("CONFIGURACION_INVALIDA", "La publicación no es compatible con el destino o sus fuentes vigentes.")
        Dim actual = PerfilImportacionJson.Huella(New VersionPerfilImportacion With {.Configuracion = snapshot.Configuracion, .Reglas = snapshot.Reglas})
        If BitConverter.ToString(actual).Replace("-", "").ToLowerInvariant() <> snapshot.Huella Then Throw NoDisponible()
        Return snapshot
    End Function

    Private Shared Sub Autorizar(contexto As ContextoImportacionCarpetas)
        If contexto Is Nothing OrElse Not contexto.PuedeImportar OrElse Not ValidadorPerfilImportacion.ReferenciaValida(contexto.UsuarioRef) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(contexto.AmbitoRef) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(contexto.NivelRef) OrElse String.IsNullOrWhiteSpace(contexto.RutaCanonica) OrElse String.IsNullOrWhiteSpace(contexto.SelloDestino) Then Throw Denegado()
    End Sub
    Private Shared Function Denegado() As PerfilImportacionException
        Return New PerfilImportacionException("ACCESO_DENEGADO", "La publicación no está disponible para este destino.")
    End Function
    Private Shared Function NoDisponible() As PerfilImportacionException
        Return New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "No se pudo comprobar la publicación operativa.")
    End Function
End Class
