Option Strict On
Imports System
Imports System.Configuration
Imports System.Web
Imports System.Web.Configuration

'Capacidad tecnica del transporte existente. Las reglas funcionales siguen en
'el perfil publicado; no introduce cuotas o claves de habilitacion adicionales.
Public NotInheritable Class LimitesImportacionCarpetas

    Public ReadOnly Property MaximoRaices As Integer
    Public ReadOnly Property MaximoArchivos As Integer
    Public ReadOnly Property MaximoBytesArchivo As ULong
    Public ReadOnly Property MaximoBytesLote As ULong
    Public ReadOnly Property MaximoBytesManifiesto As Integer
    Public ReadOnly Property DuracionPlan As TimeSpan
    Public ReadOnly Property RetencionStaging As TimeSpan

    Public Sub New(maximoRaices As Integer, maximoArchivos As Integer,
                   maximoBytesArchivo As ULong, maximoBytesLote As ULong,
                   maximoBytesManifiesto As Integer, duracionPlan As TimeSpan,
                   retencionStaging As TimeSpan)
        If maximoRaices <= 0 OrElse maximoArchivos <= 0 OrElse maximoBytesArchivo = 0UL OrElse
            maximoBytesLote = 0UL OrElse maximoBytesManifiesto <= 0 OrElse duracionPlan <= TimeSpan.Zero OrElse
            retencionStaging <= TimeSpan.Zero Then Throw NoConfigurada()
        Me.MaximoRaices = maximoRaices
        Me.MaximoArchivos = maximoArchivos
        Me.MaximoBytesArchivo = maximoBytesArchivo
        Me.MaximoBytesLote = maximoBytesLote
        Me.MaximoBytesManifiesto = maximoBytesManifiesto
        Me.DuracionPlan = duracionPlan
        Me.RetencionStaging = retencionStaging
    End Sub

    Public Shared Sub Exigir(politica As LimitesImportacionCarpetas)
        If politica Is Nothing Then Throw NoConfigurada()
    End Sub

    Public Shared Function DesdeContexto(http As HttpContext) As LimitesImportacionCarpetas
        If http Is Nothing OrElse http.Session Is Nothing Then Throw New PerfilImportacionException("SESION_REQUERIDA", "Se requiere una sesion vigente.")
        Dim transporte = TryCast(ConfigurationManager.GetSection("system.web/httpRuntime"), HttpRuntimeSection)
        If transporte Is Nothing OrElse transporte.MaxRequestLength <= 0 Then Throw NoConfigurada()
        Return DesdeTransporte(CULng(transporte.MaxRequestLength) * 1024UL, TimeSpan.FromMinutes(http.Session.Timeout))
    End Function

    Public Shared Function DesdeTransporte(maximoBytesSolicitud As ULong, vigenciaSesion As TimeSpan) As LimitesImportacionCarpetas
        If maximoBytesSolicitud = 0UL OrElse vigenciaSesion <= TimeSpan.Zero Then Throw NoConfigurada()
        'La cantidad de elementos queda acotada por el JSON existente. El lote
        'se recibe archivo a archivo: no se inventa un cupo acumulado nuevo.
        Dim manifiesto = CInt(Math.Min(maximoBytesSolicitud, CULng(PerfilImportacionJson.MaximoCaracteres) * 3UL))
        Return New LimitesImportacionCarpetas(PerfilImportacionJson.MaximoCaracteres, PerfilImportacionJson.MaximoCaracteres,
            maximoBytesSolicitud, ULong.MaxValue, manifiesto, vigenciaSesion, TimeSpan.MaxValue)
    End Function

    Private Shared Function NoConfigurada() As PerfilImportacionException
        Return New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "No se pudo comprobar la capacidad del transporte existente.")
    End Function
End Class
