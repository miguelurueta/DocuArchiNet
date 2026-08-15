Imports System
Imports System.ComponentModel
Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols

'Endpoint paralelo y exclusivamente de lectura para la previsualizacion del envio.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class WebServiceWorkflowModern
    Inherits System.Web.Services.WebService

    <WebMethod(EnableSession:=True)>
    <System.Web.Script.Services.ScriptMethod(ResponseFormat:=System.Web.Script.Services.ResponseFormat.Json)>
    Public Function PreviewEnviarTarea(ByVal idTarea As Long) As PrevisualizacionTransicionDto
        Try
            Dim resultadoSesion As ResultadoContextoSesionWorkflow = New WorkflowPreviewSessionContextGate().AsegurarContexto()
            If resultadoSesion.Contexto Is Nothing OrElse Not resultadoSesion.Contexto.EsValido() OrElse
               String.IsNullOrWhiteSpace(resultadoSesion.CadenaConexionWorkflow) Then
                Return CrearServicioSinConexion().Previsualizar(New ContextoModuloWorkflow(), idTarea)
            End If

            Dim factory As New WorkflowModuleConnectionFactory(resultadoSesion.CadenaConexionWorkflow)
            Dim docuarchiFactory As IModuleConnectionFactory = Nothing
            If Not String.IsNullOrWhiteSpace(resultadoSesion.CadenaConexionDocuarchi) Then
                docuarchiFactory = New DocuarchiModuleConnectionFactory(resultadoSesion.CadenaConexionDocuarchi)
            End If
            Dim servicio As New ServicioTransicionTarea(
                New MySqlTareaWorkflowRepository(factory, New AdoNetDataExecutor()),
                New MySqlTransicionFlujoRepository(factory, New AdoNetDataExecutor()),
                New MySqlTransicionRutaRepository(factory, docuarchiFactory, New AdoNetDataExecutor()),
                New ConfiguracionWorkflowModernFeatureGate(),
                New ValidadorTransicionTarea())

            Return servicio.Previsualizar(resultadoSesion.Contexto, idTarea)
        Catch ex As Exception
            Return CrearRespuestaSegura(idTarea)
        End Try
    End Function

    Private Shared Function CrearServicioSinConexion() As ServicioTransicionTarea
        Return New ServicioTransicionTarea(
            New MySqlTareaWorkflowRepository(),
            New MySqlTransicionFlujoRepository(),
            New MySqlTransicionRutaRepository(),
            New ConfiguracionWorkflowModernFeatureGate(),
            New ValidadorTransicionTarea())
    End Function

    Private Shared Function CrearRespuestaSegura(ByVal idTarea As Long) As PrevisualizacionTransicionDto
        Return New PrevisualizacionTransicionDto With {
            .IdTarea = idTarea,
            .[Error] = New ErrorTransicionDto With {
                .Codigo = CodigosBloqueoPrevisualizacion.TransicionInconsistente,
                .MensajeVisible = "No fue posible consultar los destinos de la tarea.",
                .ReferenciaTrazabilidad = String.Empty
            }
        }
    End Function
End Class
