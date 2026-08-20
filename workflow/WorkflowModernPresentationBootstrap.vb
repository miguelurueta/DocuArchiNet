Imports System
Imports System.Web

'Bootstrap de Presentation: valida el contexto de sesion y expone la politica oficial
'que usan los contratos ASMX para enlazar la experiencia moderna.
Public NotInheritable Class WorkflowModernPresentationBootstrap
    Private Const ClaveSolicitudActiva As String = "WorkflowModernPresentationBootstrap.Activa"
    Private ReadOnly _contextGate As WorkflowPreviewSessionContextGate
    Private ReadOnly _featureGate As IWorkflowModernFeatureGate

    Public Sub New()
        Me.New(New WorkflowPreviewSessionContextGate(), New ConfiguracionWorkflowModernFeatureGate())
    End Sub

    Friend Sub New(ByVal contextGate As WorkflowPreviewSessionContextGate,
                   ByVal featureGate As IWorkflowModernFeatureGate)
        _contextGate = contextGate
        _featureGate = featureGate
    End Sub

    Public Shared Function EstaActivaParaSolicitudActual() As Boolean
        Dim solicitud As HttpContext = HttpContext.Current
        If solicitud Is Nothing Then
            Return False
        End If

        Dim valorCacheado As Object = solicitud.Items(ClaveSolicitudActiva)
        If TypeOf valorCacheado Is Boolean Then
            Return CBool(valorCacheado)
        End If

        Dim activa As Boolean = New WorkflowModernPresentationBootstrap().EstaActiva()
        solicitud.Items(ClaveSolicitudActiva) = activa
        Return activa
    End Function

    Public Shared Function ValorAtributoActivacionSolicitudActual() As String
        Return If(EstaActivaParaSolicitudActual(), "true", "false")
    End Function

    Public Function EstaActiva() As Boolean
        If _contextGate Is Nothing OrElse _featureGate Is Nothing Then
            Return False
        End If

        Try
            Dim resultadoContexto As ResultadoContextoSesionWorkflow = _contextGate.AsegurarContexto()
            If resultadoContexto Is Nothing OrElse resultadoContexto.Contexto Is Nothing OrElse
               Not resultadoContexto.Contexto.EsValido() Then
                Return False
            End If

            Dim habilitacion As HabilitacionWorkflowModern = _featureGate.Evaluar(resultadoContexto.Contexto)
            Return habilitacion IsNot Nothing AndAlso habilitacion.EstaActiva
        Catch
            'La ausencia de contexto no enlaza operaciones y no revela detalles internos.
            Return False
        End Try
    End Function
End Class
