Option Strict On
Imports System
Imports System.Collections.Generic
Imports System.Linq

Public Class ServicioPerfilImportacion
    Private ReadOnly _repositorio As IPerfilImportacionRepository
    Private ReadOnly _catalogos As IPerfilImportacionCatalogos
    Private ReadOnly _validador As New ValidadorPerfilImportacion()
    Private ReadOnly _resolvedor As New ResolvedorReglasPerfilImportacion()
    Private ReadOnly _proveedores As CoordinadorProveedorIndices

    Public Sub New(repositorio As IPerfilImportacionRepository, catalogos As IPerfilImportacionCatalogos, Optional proveedores As CoordinadorProveedorIndices = Nothing)
        If repositorio Is Nothing OrElse catalogos Is Nothing Then Throw New ArgumentNullException()
        _repositorio = repositorio
        _catalogos = catalogos
        _proveedores = proveedores
    End Sub

    Private Sub Autorizar(contexto As ContextoPerfilImportacion)
        If contexto Is Nothing OrElse Not contexto.PuedeAdministrar OrElse Not ValidadorPerfilImportacion.ReferenciaValida(contexto.UsuarioRef) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(contexto.AmbitoRef) Then Throw New PerfilImportacionException("ACCESO_DENEGADO", "Operación no autorizada.")
        _repositorio.VerificarEsquema()
    End Sub

    Public Function Listar(contexto As ContextoPerfilImportacion, filtro As FiltroPerfilesImportacion) As PaginaPerfilesImportacion
        Autorizar(contexto)
        If filtro Is Nothing OrElse Not ValidadorPerfilImportacion.TextoValido(filtro.Termino, 160, True) OrElse Not ValidadorPerfilImportacion.TextoValido(filtro.Nombre, 160, True) OrElse Not ValidadorPerfilImportacion.TextoValido(filtro.Codigo, 64, True) Then Throw SolicitudInvalida()
        Return _repositorio.Listar(contexto, filtro)
    End Function
    Public Function ListarProveedores(contexto As ContextoPerfilImportacion, pagina As Integer, tamano As Integer, proveedorId As String, versionId As String) As List(Of OpcionProveedorIndices)
        Autorizar(contexto)
        If _proveedores Is Nothing Then Throw New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "Proveedores no disponibles.")
        Return _proveedores.Listar(contexto, pagina, tamano, proveedorId, versionId)
    End Function

    Public Function Obtener(contexto As ContextoPerfilImportacion, perfilId As String, versionId As String) As PerfilImportacion
        Autorizar(contexto)
        ExigirId(perfilId)
        If Not String.IsNullOrEmpty(versionId) Then ExigirId(versionId)
        Return _repositorio.Obtener(contexto, perfilId, versionId)
    End Function

    Public Function Historial(contexto As ContextoPerfilImportacion, perfilId As String, pagina As Integer, tamano As Integer) As List(Of VersionPerfilImportacion)
        Autorizar(contexto)
        ExigirId(perfilId)
        _repositorio.Obtener(contexto, perfilId, Nothing)
        Return _repositorio.Historial(contexto, perfilId, pagina, tamano)
    End Function

    Public Function Crear(contexto As ContextoPerfilImportacion, codigo As String, nombre As String, descripcion As String) As PerfilImportacion
        Autorizar(contexto)
        If String.IsNullOrWhiteSpace(codigo) OrElse codigo.Length > 64 OrElse codigo.Any(Function(c) AscW(c) < 33 OrElse AscW(c) > 126) Then Throw SolicitudInvalida()
        ValidarCabecera(nombre, descripcion)
        Return _repositorio.Crear(contexto, codigo, nombre, descripcion)
    End Function

    Public Function Guardar(contexto As ContextoPerfilImportacion, perfilId As String, version As VersionPerfilImportacion) As PerfilImportacion
        Autorizar(contexto)
        ExigirId(perfilId)
        If version Is Nothing Then Throw SolicitudInvalida()
        ExigirId(version.VersionId)
        Dim actual = _repositorio.Obtener(contexto, perfilId, version.VersionId)
        If actual.Version.Estado <> "BORRADOR" OrElse actual.Version.Revision <> version.Revision Then Throw Conflicto()
        ExigirConfiguracion(contexto, version, False)
        Return _repositorio.Guardar(contexto, perfilId, version)
    End Function

    Public Function Ensayar(contexto As ContextoPerfilImportacion, perfilId As String, version As VersionPerfilImportacion, ejemplos As List(Of EjemploPerfilImportacion)) As List(Of ResultadoEnsayoPerfilImportacion)
        Autorizar(contexto)
        ExigirId(perfilId)
        If version Is Nothing OrElse ejemplos Is Nothing OrElse ejemplos.Count < 1 OrElse ejemplos.Count > 20 Then Throw SolicitudInvalida()
        ExigirId(version.VersionId)
        Dim actual = _repositorio.Obtener(contexto, perfilId, version.VersionId)
        'Una publicación se ensaya desde su snapshot persistido, nunca desde reglas reenviadas.
        If actual.Version.Estado = "PUBLICADA" Then version = actual.Version
        ExigirConfiguracion(contexto, version, False)
        Dim heredadas = version.Reglas.Where(Function(r) (r.FuenteTipo = "EXPEDIENTE" AndAlso ResolvedorReglasPerfilImportacion.PropiedadesClasificacion.Contains(r.PropiedadExpediente)) OrElse
            (r.FuenteTipo = "PROVEEDOR" AndAlso r.Proveedor.Clave.FuenteTipo = "EXPEDIENTE" AndAlso ResolvedorReglasPerfilImportacion.PropiedadesClasificacion.Contains(r.Proveedor.Clave.PropiedadExpediente))).ToList()
        If heredadas.Count > 0 Then
            Dim origen = TryCast(_catalogos, IClasificacionEnsayoPerfil)
            If origen Is Nothing Then Throw New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "No se puede resolver la clasificacion del perfil.")
            Dim propuesta = origen.ClasificacionPropuesta(contexto, version.Configuracion)
            Dim preparados As New List(Of EjemploPerfilImportacion)()
            For Each ejemplo In ejemplos
                If ejemplo Is Nothing OrElse ejemplo.Expediente Is Nothing Then Throw SolicitudInvalida()
                Dim copia As New EjemploPerfilImportacion With {.NombreCarpeta = ejemplo.NombreCarpeta, .NombreArchivo = ejemplo.NombreArchivo, .Expediente = New Dictionary(Of String, String)(ejemplo.Expediente, StringComparer.Ordinal)}
                'El cliente no puede sustituir la clasificacion autorizada por un supuesto expediente confirmado.
                For Each propiedad In ResolvedorReglasPerfilImportacion.PropiedadesClasificacion
                    copia.Expediente.Remove(propiedad)
                Next
                For Each par In propuesta
                    copia.Expediente.Add(par.Key, par.Value)
                Next
                preparados.Add(copia)
            Next
            ejemplos = preparados
        End If
        Dim resultados As List(Of ResultadoEnsayoPerfilImportacion)
        If version.Reglas.Any(Function(r) r.FuenteTipo = "PROVEEDOR") Then
            resultados = _proveedores.Ensayar(contexto, version.Reglas, ejemplos)
        Else
            resultados = ejemplos.Select(Function(e) _resolvedor.Resolver(version.Reglas, e)).ToList()
        End If
        For Each resultado In resultados
            For Each valor In resultado.Valores
                If heredadas.Any(Function(r) r.DestinoTipo = valor.DestinoTipo AndAlso r.DestinoRef = valor.DestinoRef) Then valor.Procedencia = "PROPUESTA_PERFIL"
            Next
            If resultado.Incidencias.Count = 0 Then resultado.Incidencias.AddRange(_catalogos.ValidarValores(contexto, version.Configuracion, resultado.Valores))
        Next
        Return resultados
    End Function

    Public Function Publicar(contexto As ContextoPerfilImportacion, perfilId As String, versionId As String, revisionPerfil As UInteger, revisionVersion As UInteger) As PerfilImportacion
        Dim actual = Obtener(contexto, perfilId, versionId)
        If actual.Revision <> revisionPerfil OrElse actual.Version.Revision <> revisionVersion OrElse actual.Version.Estado <> "BORRADOR" Then Throw Conflicto()
        ExigirConfiguracion(contexto, actual.Version, True)
        Return _repositorio.Publicar(contexto, perfilId, versionId, revisionPerfil, revisionVersion, PerfilImportacionJson.Huella(actual.Version))
    End Function

    Public Function CrearVersion(contexto As ContextoPerfilImportacion, perfilId As String, origenId As String, revisionPerfil As UInteger, revisionOrigen As UInteger) As PerfilImportacion
        Autorizar(contexto)
        ExigirId(perfilId)
        ExigirId(origenId)
        Return _repositorio.CrearVersion(contexto, perfilId, origenId, revisionPerfil, revisionOrigen)
    End Function

    Public Function CambiarDisponibilidad(contexto As ContextoPerfilImportacion, perfilId As String, revision As UInteger, activo As Boolean) As PerfilImportacion
        Autorizar(contexto)
        ExigirId(perfilId)
        Return _repositorio.CambiarDisponibilidad(contexto, perfilId, revision, activo)
    End Function

    Public Function ActualizarCabecera(contexto As ContextoPerfilImportacion, perfilId As String, revision As UInteger, nombre As String, descripcion As String) As PerfilImportacion
        Autorizar(contexto)
        ExigirId(perfilId)
        ValidarCabecera(nombre, descripcion)
        Return _repositorio.ActualizarCabecera(contexto, perfilId, revision, nombre, descripcion)
    End Function

    Public Function Seleccionar(contexto As ContextoPerfilImportacion, perfilId As String, versionId As String, nivelRef As String, proposito As String) As PerfilImportacion
        Autorizar(contexto)
        If proposito <> "ADMINISTRACION" Then Throw New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "La selección operativa pertenece a la entrega de importación.")
        ExigirId(versionId)
        Dim actual = Obtener(contexto, perfilId, versionId)
        If Not actual.Activo OrElse actual.Version.Estado <> "PUBLICADA" Then Throw New PerfilImportacionException("ACCESO_DENEGADO", "La versión no está disponible para selección.")
        ExigirConfiguracion(contexto, actual.Version, True)
        If Not ValidadorPerfilImportacion.ReferenciaValida(nivelRef) OrElse Not _catalogos.NivelCompatible(contexto, actual.Version.Configuracion, nivelRef) Then Throw New PerfilImportacionException("ACCESO_DENEGADO", "El nivel no es compatible o no está autorizado.")
        Return actual
    End Function

    Private Sub ExigirConfiguracion(contexto As ContextoPerfilImportacion, version As VersionPerfilImportacion, publicar As Boolean)
        Dim errores = _validador.Validar(version.Configuracion, version.Reglas, publicar)
        If errores.Count = 0 AndAlso version.Reglas.Any(Function(r) r.FuenteTipo = "PROVEEDOR") Then
            If _proveedores Is Nothing Then Throw New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "La fuente de proveedor no esta disponible.")
            errores.AddRange(_proveedores.Validar(contexto, version.Reglas))
        End If
        If errores.Count = 0 Then errores.AddRange(_catalogos.Validar(contexto, version.Configuracion, version.Reglas, publicar))
        If errores.Count > 0 Then Throw New ConfiguracionPerfilImportacionException(errores)
        PerfilImportacionJson.Escribir(version.Configuracion)
        PerfilImportacionJson.Escribir(version.Reglas)
    End Sub

    Private Shared Sub ExigirId(id As String)
        If Not ValidadorPerfilImportacion.ReferenciaValida(id) Then Throw SolicitudInvalida()
    End Sub
    Private Shared Sub ValidarCabecera(nombre As String, descripcion As String)
        '16000 BMP caracteres caben en TEXT utf8 (máximo 48000 bytes).
        If Not ValidadorPerfilImportacion.TextoValido(nombre, 160, False) OrElse Not ValidadorPerfilImportacion.TextoValido(descripcion, 16000, True) Then Throw SolicitudInvalida()
    End Sub
    Private Shared Function SolicitudInvalida() As PerfilImportacionException
        Return New PerfilImportacionException("SOLICITUD_INVALIDA", "Revise los datos de la solicitud.")
    End Function
    Private Shared Function Conflicto() As PerfilImportacionException
        Return New PerfilImportacionException("CONFLICTO_REVISION", "La versión cambió o ya está publicada.")
    End Function
End Class

Public Class ConfiguracionPerfilImportacionException
    Inherits PerfilImportacionException
    Public ReadOnly Property Incidencias As List(Of IncidenciaPerfilImportacion)
    Public Sub New(incidencias As List(Of IncidenciaPerfilImportacion))
        MyBase.New("CONFIGURACION_INVALIDA", "La configuración contiene errores.")
        Me.Incidencias = incidencias
    End Sub
End Class
