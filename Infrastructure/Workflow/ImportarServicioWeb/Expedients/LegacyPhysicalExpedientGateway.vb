Imports System
Imports System.Collections.Generic
Imports System.Reflection
Imports System.IO
Imports System.Xml

' Gateway físico moderno: recibe todo el contexto de autoridad de forma explícita.
' La función AutoRegistraExpedienteTramite permanece disponible para consumidores legacy.
Public NotInheritable Class ModernPhysicalExpedientGateway
    Implements IPhysicalExpedientGateway

    Private ReadOnly _identityLookup As IPhysicalExpedientIdentityLookup

    Public Sub New(ByVal identityLookup As IPhysicalExpedientIdentityLookup)
        If identityLookup Is Nothing Then Throw New ArgumentNullException("identityLookup")
        _identityLookup = identityLookup
    End Sub

    Public Function Localizar(ByVal contexto As ContextoImportacionServicio,
                              ByVal nombreGabinete As String,
                              ByVal valorConsulta As String) As IList(Of ExpedienteFisicoImportacion) Implements IPhysicalExpedientGateway.Localizar
        ValidateContext(contexto)
        Dim values As New List(Of ExpedienteFisicoImportacion)()
        Dim ids = _identityLookup.BuscarPrincipal(contexto, nombreGabinete, If(valorConsulta, String.Empty))
        For Each id In ids
            Dim physical = Obtener(contexto, id)
            If physical IsNot Nothing Then values.Add(physical)
        Next
        Return values
    End Function

    Public Function Crear(ByVal contexto As ContextoImportacionServicio,
                          ByVal inscripcion As InscripcionImportacion,
                          ByVal configuracion As ConfiguracionExpedienteImportacion,
                          ByVal identidad As IdentidadExpedienteNormalizada) As ResultadoCreacionExpedienteFisico Implements IPhysicalExpedientGateway.Crear
        ValidateContext(contexto)
        If contexto.IdUsuarioGestion <= 0 OrElse contexto.IdEmpresaGestion <= 0 Then Throw New InvalidOperationException("EXPEDIENT_MANAGEMENT_CONTEXT_INVALID")
        If inscripcion Is Nothing OrElse configuracion Is Nothing OrElse identidad Is Nothing OrElse Not identidad.Valida Then Throw New InvalidOperationException("EXPEDIENT_CREATE_INPUT_INVALID")
        Dim legacy As New CIncripcionSII With {.LIBRO_SII = inscripcion.Libro, .REGISTRO_SII = inscripcion.Registro,
            .MATRICULA_SII = identidad.ValorPersistencia, .PROPONENTE_SII = If(String.Equals(identidad.NombreGabinete, "RUP", StringComparison.OrdinalIgnoreCase), identidad.ValorPersistencia, inscripcion.Proponente),
            .NIT_SII = inscripcion.IdentificacionSujeto, .RSOCIAL_SII = inscripcion.RazonSocial,
            .RADICADO_SII = inscripcion.RadicadoSii, .COD_BARRA_SII = inscripcion.RadicadoSii}
        Dim id As Integer = 0, name As String = Nothing
        Try
            Using creationLock = _identityLookup.Adquirir(contexto, configuracion)
                If creationLock Is Nothing Then Return New ResultadoCreacionExpedienteFisico With {.RespuestaRecibida = False, .Aceptada = False, .Codigo = "EXPEDIENT_CREATE_LOCK_UNAVAILABLE"}
                Dim existing = _identityLookup.Buscar(contexto, configuracion)
                If existing Is Nothing Then Return New ResultadoCreacionExpedienteFisico With {.RespuestaRecibida = False, .Aceptada = False, .Codigo = "EXPEDIENT_PRECHECK_UNKNOWN"}
                If existing.Count > 1 Then Return New ResultadoCreacionExpedienteFisico With {.RespuestaRecibida = True, .Aceptada = False, .Codigo = "EXPEDIENT_IDENTITY_CONFLICT"}
                If existing.Count = 1 Then
                    Return New ResultadoCreacionExpedienteFisico With {.RespuestaRecibida = True, .Aceptada = True, .IdExpediente = existing(0), .Codigo = "EXPEDIENT_ALREADY_EXISTS"}
                End If
                Dim response = New ModernExpedientMutationBridge().RegistrarExpedienteTramiteConContexto(
                    contexto.IdTramite, legacy, contexto.IdTarea, 0, contexto.IdUsuarioGestion,
                    contexto.IdEmpresaGestion, id, name)
                Return New ResultadoCreacionExpedienteFisico With {.RespuestaRecibida = True, .Aceptada = String.Equals(response, "YES", StringComparison.OrdinalIgnoreCase), .IdExpediente = If(id > 0, New Nullable(Of Long)(id), Nothing), .Codigo = If(String.Equals(response, "YES", StringComparison.OrdinalIgnoreCase), "EXPEDIENT_CREATED", "EXPEDIENT_CREATE_REJECTED")}
            End Using
        Catch
            Try
                Dim recovered = _identityLookup.Buscar(contexto, configuracion)
                If recovered IsNot Nothing AndAlso recovered.Count = 1 Then Return New ResultadoCreacionExpedienteFisico With {.RespuestaRecibida = False, .Aceptada = True, .IdExpediente = recovered(0), .Codigo = "EXPEDIENT_CREATE_RESPONSE_LOST_RECOVERED"}
                If recovered IsNot Nothing AndAlso recovered.Count > 1 Then Return New ResultadoCreacionExpedienteFisico With {.RespuestaRecibida = False, .Aceptada = False, .Codigo = "EXPEDIENT_IDENTITY_CONFLICT"}
            Catch
                ' La postconsulta también fue incierta; el repositorio conservará ResultadoIncierto.
            End Try
            Return New ResultadoCreacionExpedienteFisico With {.RespuestaRecibida = False, .Aceptada = False, .Codigo = "EXPEDIENT_CREATE_RESULT_UNKNOWN"}
        End Try
    End Function

    Public Function Obtener(ByVal contexto As ContextoImportacionServicio,
                            ByVal idExpediente As Long) As ExpedienteFisicoImportacion Implements IPhysicalExpedientGateway.Obtener
        ValidateContext(contexto)
        If idExpediente <= 0 OrElse idExpediente > Integer.MaxValue Then Return Nothing
        Dim records() As expediente_conservacion = Nothing
        Dim legacy As New ClassGaExpediente()
        Dim response = legacy.SolicitaDatosEstructuraExpediente(CInt(idExpediente), records)
        If Not String.Equals(response, "YES", StringComparison.OrdinalIgnoreCase) OrElse records Is Nothing OrElse records.Length <> 1 Then Return Nothing
        Dim cabinet As String = Nothing
        response = legacy.SolicitaGabineteProducionExpediente(CInt(idExpediente), cabinet)
        If Not String.Equals(response, "YES", StringComparison.OrdinalIgnoreCase) Then Return Nothing
        If Not ConfirmXmlIndex(records(0), idExpediente) Then Return Nothing
        Dim snapshot As New ExpedienteFisicoImportacion With {.IdExpediente = idExpediente, .NombreGabinete = cabinet, .ValorIdentidad = Convert.ToString(records(0).CODIGO_UNICO)}
        CopyPublicMembers(records(0), snapshot.Campos)
        AddLegacyFieldAliases(records(0), snapshot.Campos)
        Return snapshot
    End Function

    Private Shared Sub AddLegacyFieldAliases(ByVal record As expediente_conservacion,
                                             ByVal target As IDictionary(Of String, String))
        ' SolicitaDatosEstructuraExpediente proyecta estas columnas con nombres históricos distintos.
        target("NOMBRE_AREA_TRD") = Convert.ToString(record.NOMBRE_AREA).Trim()
        target("CODIGO_SERIE_TRD") = Convert.ToString(record.CODIGO_SERIE).Trim()
        target("NOMBRE_SERIE_TRD") = Convert.ToString(record.NOMBRE_SERIE).Trim()
        target("CODIGO_SUB_SERIE_TRD") = Convert.ToString(record.CODIGO_SUBSERIE).Trim()
        target("NOMBRE_SUBSERIE_TRD") = Convert.ToString(record.NOMBRE_SUBSERIE).Trim()
    End Sub

    Private Shared Sub CopyPublicMembers(ByVal source As Object, ByVal target As IDictionary(Of String, String))
        For Each field In source.GetType().GetFields(BindingFlags.Instance Or BindingFlags.Public)
            target(field.Name) = Convert.ToString(field.GetValue(source)).Trim()
        Next
        For Each propertyInfo In source.GetType().GetProperties(BindingFlags.Instance Or BindingFlags.Public)
            If propertyInfo.CanRead AndAlso propertyInfo.GetIndexParameters().Length = 0 Then target(propertyInfo.Name) = Convert.ToString(propertyInfo.GetValue(source, Nothing)).Trim()
        Next
    End Sub

    Private Shared Function ConfirmXmlIndex(ByVal record As expediente_conservacion,
                                            ByVal idExpediente As Long) As Boolean
        If record.ID_DISCO <= 0 OrElse idExpediente <= 0 Then Return False
        Dim route As stru_ruta_expediente = Nothing
        Dim response = New Class_ra_ruta_expediente().Solicita_datos_estructura_ruta_expediente(route)
        If Not String.Equals(response, "YES", StringComparison.OrdinalIgnoreCase) OrElse String.IsNullOrWhiteSpace(route.RUTA) Then Return False
        Dim disk = record.ID_DISCO.ToString().PadLeft(9, "0"c)
        Dim fileName = idExpediente.ToString().PadLeft(9, "0"c) & ".xml"
        Dim xmlPath = Path.Combine(route.RUTA.Replace("/", "\"), disk, fileName)
        If Not File.Exists(xmlPath) Then Return False
        Try
            Dim document As New XmlDocument()
            document.Load(xmlPath)
            Dim identityNodes = document.GetElementsByTagName("identicacionexpediente")
            If identityNodes Is Nothing OrElse identityNodes.Count <> 1 Then Return False
            Dim persistedId As Long
            Return Long.TryParse(identityNodes(0).InnerText, persistedId) AndAlso persistedId = idExpediente
        Catch
            Return False
        End Try
    End Function

    Private Shared Sub ValidateContext(ByVal contexto As ContextoImportacionServicio)
        If contexto Is Nothing OrElse contexto.IdTarea <= 0 OrElse contexto.IdTramite <= 0 Then Throw New InvalidOperationException("EXPEDIENT_CONTEXT_INVALID")
    End Sub

End Class
