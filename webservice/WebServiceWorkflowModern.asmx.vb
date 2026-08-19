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

    <WebMethod(EnableSession:=True)>
    <System.Web.Script.Services.ScriptMethod(ResponseFormat:=System.Web.Script.Services.ResponseFormat.Json)>
    Public Function EjecutarEnvioTarea(ByVal idTarea As Long,
                                       ByVal idConector As Integer,
                                       ByVal tokenVersion As String) As ResultadoTransicionDto
        Try
            Dim resultadoSesion As ResultadoContextoSesionWorkflow = New WorkflowPreviewSessionContextGate().AsegurarContextoEjecucion()
            If resultadoSesion.Contexto Is Nothing OrElse Not resultadoSesion.Contexto.EsValido() OrElse
               String.IsNullOrWhiteSpace(resultadoSesion.CadenaConexionWorkflow) Then
                Return CrearResultadoEjecucionBloqueado(CodigosBloqueoPrevisualizacion.ContextoInvalido,
                                                        "No fue posible validar la sesion de la tarea.")
            End If

            Dim factory As New WorkflowModuleConnectionFactory(resultadoSesion.CadenaConexionWorkflow)
            Dim docuarchiFactory As IModuleConnectionFactory = Nothing
            If Not String.IsNullOrWhiteSpace(resultadoSesion.CadenaConexionDocuarchi) Then
                docuarchiFactory = New DocuarchiModuleConnectionFactory(resultadoSesion.CadenaConexionDocuarchi)
            End If
            Dim dataExecutor As New AdoNetDataExecutor()
            Dim servicio As New ServicioTransicionTarea(
                New MySqlTareaWorkflowRepository(factory, dataExecutor),
                New MySqlTransicionFlujoRepository(factory, dataExecutor),
                New MySqlTransicionRutaRepository(factory, docuarchiFactory, dataExecutor),
                New MySqlTransicionEjecucionRepository(factory, docuarchiFactory, dataExecutor),
                New WorkflowLegacyRequisitosAdapter(),
                New WorkflowLegacyAuditoriaAdapter(),
                New MySqlTransicionConcurrencyGuard(factory, dataExecutor),
                New ConfiguracionWorkflowModernFeatureGate(),
                New ValidadorTransicionTarea(),
                New EjecutorTransicionTarea(New WorkflowLegacyExecutorAdapter()))

            Return servicio.Ejecutar(resultadoSesion.Contexto, New SolicitudTransicionWorkflow With {
                .IdTarea = idTarea,
                .IdConector = idConector,
                .TokenVersion = tokenVersion
            })
        Catch
            Return CrearResultadoEjecucionBloqueado(CodigosBloqueoPrevisualizacion.TransicionNoDisponible,
                                                    "No fue posible enviar la tarea.")
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    <System.Web.Script.Services.ScriptMethod(ResponseFormat:=System.Web.Script.Services.ResponseFormat.Json)>
    Public Function PreviewEnviarGrupo(ByVal idTarea As Long) As PrevisualizacionEnvioGrupoDto
        Try
            Dim resultadoSesion As ResultadoContextoSesionWorkflow = New WorkflowPreviewSessionContextGate().AsegurarContextoEnvioGrupo()
            If resultadoSesion Is Nothing OrElse resultadoSesion.Contexto Is Nothing OrElse Not resultadoSesion.Contexto.EsValido() OrElse
               String.IsNullOrWhiteSpace(resultadoSesion.CadenaConexionWorkflow) Then
                Return CrearRespuestaGrupoSegura(idTarea, CodigosBloqueoPrevisualizacion.ContextoInvalido,
                                                 "No fue posible validar la sesion de la tarea.")
            End If

            Dim factory As New WorkflowModuleConnectionFactory(resultadoSesion.CadenaConexionWorkflow)
            Dim docuarchiFactory As IModuleConnectionFactory = Nothing
            If Not String.IsNullOrWhiteSpace(resultadoSesion.CadenaConexionDocuarchi) Then
                docuarchiFactory = New DocuarchiModuleConnectionFactory(resultadoSesion.CadenaConexionDocuarchi)
            End If
            Dim dataExecutor As New AdoNetDataExecutor()
            Dim servicio As New ServicioEnvioGrupoTarea(
                New MySqlTareaWorkflowRepository(factory, dataExecutor),
                New MySqlEnvioGrupoRepository(factory, docuarchiFactory, dataExecutor),
                New ConfiguracionWorkflowModernFeatureGate(),
                New ValidadorEnvioGrupoTarea())
            Return servicio.Previsualizar(resultadoSesion.Contexto, idTarea)
        Catch
            Return CrearRespuestaGrupoSegura(idTarea, CodigosBloqueoPrevisualizacion.TransicionNoDisponible,
                                             "No fue posible consultar los destinos de la tarea.")
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    <System.Web.Script.Services.ScriptMethod(ResponseFormat:=System.Web.Script.Services.ResponseFormat.Json)>
    Public Function EjecutarEnvioGrupo(ByVal idTarea As Long,
                                       ByVal idActividadDestino As Integer,
                                       ByVal tokenVersion As String) As ResultadoEnvioGrupoDto
        Try
            Dim resultadoSesion As ResultadoContextoSesionWorkflow = New WorkflowPreviewSessionContextGate().AsegurarContextoEnvioGrupo(True)
            If resultadoSesion Is Nothing OrElse resultadoSesion.Contexto Is Nothing OrElse Not resultadoSesion.Contexto.EsValido() OrElse
               String.IsNullOrWhiteSpace(resultadoSesion.CadenaConexionWorkflow) Then
                Return CrearResultadoEnvioGrupoBloqueado(CodigosBloqueoPrevisualizacion.ContextoInvalido,
                                                         "No fue posible validar la sesion de la tarea.")
            End If

            Dim factory As New WorkflowModuleConnectionFactory(resultadoSesion.CadenaConexionWorkflow)
            Dim docuarchiFactory As IModuleConnectionFactory = Nothing
            If Not String.IsNullOrWhiteSpace(resultadoSesion.CadenaConexionDocuarchi) Then
                docuarchiFactory = New DocuarchiModuleConnectionFactory(resultadoSesion.CadenaConexionDocuarchi)
            End If
            Dim dataExecutor As New AdoNetDataExecutor()
            Dim repositorioGrupo As New MySqlEnvioGrupoRepository(factory, docuarchiFactory, dataExecutor)
            Dim servicio As New ServicioEnvioGrupoTarea(
                New MySqlTareaWorkflowRepository(factory, dataExecutor),
                repositorioGrupo,
                repositorioGrupo,
                New WorkflowLegacyEnvioGrupoRequisitosAdapter(),
                New WorkflowLegacyAuditoriaAdapter(),
                New MySqlTransicionConcurrencyGuard(factory, dataExecutor),
                New ConfiguracionWorkflowModernFeatureGate(),
                New WorkflowLegacyEnvioGrupoExecutorAdapter(),
                New ValidadorEnvioGrupoTarea())
            Return servicio.Ejecutar(resultadoSesion.Contexto, New SolicitudEnvioGrupoWorkflow With {
                .IdTarea = idTarea,
                .IdActividadDestino = idActividadDestino,
                .TokenVersion = tokenVersion
            })
        Catch
            Return CrearResultadoEnvioGrupoBloqueado(CodigosBloqueoPrevisualizacion.TransicionNoDisponible,
                                                     "No fue posible enviar la tarea.")
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

    Private Shared Function CrearRespuestaGrupoSegura(ByVal idTarea As Long,
                                                       ByVal codigo As String,
                                                       ByVal mensaje As String) As PrevisualizacionEnvioGrupoDto
        Return New PrevisualizacionEnvioGrupoDto With {
            .IdTarea = idTarea,
            .[Error] = New ErrorTransicionDto With {
                .Codigo = codigo,
                .MensajeVisible = mensaje,
                .ReferenciaTrazabilidad = String.Empty
            }
        }
    End Function

    Private Shared Function CrearResultadoEjecucionBloqueado(ByVal codigo As String,
                                                              ByVal mensaje As String) As ResultadoTransicionDto
        Return New ResultadoTransicionDto With {
            .Exito = False,
            .EstadoFinal = "bloqueado",
            .CodigoBloqueo = codigo,
            .MensajeFuncional = mensaje,
            .EsReintentable = False,
            .[Error] = New ErrorTransicionDto With {
                .Codigo = codigo,
                .MensajeVisible = mensaje,
                .ReferenciaTrazabilidad = String.Empty
            }
        }
    End Function

    Private Shared Function CrearResultadoEnvioGrupoBloqueado(ByVal codigo As String,
                                                               ByVal mensaje As String) As ResultadoEnvioGrupoDto
        Return New ResultadoEnvioGrupoDto With {
            .Exito = False,
            .EstadoFinal = "bloqueado",
            .CodigoBloqueo = codigo,
            .MensajeFuncional = mensaje,
            .EsReintentable = False,
            .[Error] = New ErrorTransicionDto With {
                .Codigo = codigo,
                .MensajeVisible = mensaje,
                .ReferenciaTrazabilidad = String.Empty
            }
        }
    End Function
End Class
