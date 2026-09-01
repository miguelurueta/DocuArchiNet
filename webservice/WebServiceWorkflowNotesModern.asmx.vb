Imports System
Imports System.ComponentModel
Imports System.Web.Services

'Endpoint especializado de Notas Workflow. Las mutaciones se delegan al servicio y repositorio modernos.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class WebServiceWorkflowNotesModern
    Inherits System.Web.Services.WebService

    <WebMethod(EnableSession:=True)>
    <System.Web.Script.Services.ScriptMethod(ResponseFormat:=System.Web.Script.Services.ResponseFormat.Json)>
    Public Function ListarNotas(ByVal idTarea As Long,
                                ByVal cursor As String,
                                ByVal tamanoPagina As Integer) As ResultadoNotasDto
        Try
            Dim resultadoSesion As ResultadoContextoSesionWorkflow = New WorkflowPreviewSessionContextGate().AsegurarContextoNotas()
            If Not ContextoDisponible(resultadoSesion) Then Return RespuestaBloqueada()
            Dim resultado As ResultadoNotasWorkflow = CrearServicio(resultadoSesion).Listar(resultadoSesion.Contexto,
                New SolicitudListarNotasWorkflow With {.IdTarea = idTarea, .Cursor = cursor, .TamanoPagina = tamanoPagina})
            Return Mapear(resultado)
        Catch
            Return RespuestaBloqueada()
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    <System.Web.Script.Services.ScriptMethod(ResponseFormat:=System.Web.Script.Services.ResponseFormat.Json)>
    Public Function ConsultarNota(ByVal idTarea As Long,
                                  ByVal idNota As Long) As ResultadoNotasDto
        Try
            Dim resultadoSesion As ResultadoContextoSesionWorkflow = New WorkflowPreviewSessionContextGate().AsegurarContextoNotas()
            If Not ContextoDisponible(resultadoSesion) Then Return RespuestaBloqueada()
            Dim resultado As ResultadoNotasWorkflow = CrearServicio(resultadoSesion).Consultar(resultadoSesion.Contexto,
                New SolicitudConsultarNotaWorkflow With {.IdTarea = idTarea, .IdNota = idNota})
            Return Mapear(resultado)
        Catch
            Return RespuestaBloqueada()
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    <System.Web.Script.Services.ScriptMethod(ResponseFormat:=System.Web.Script.Services.ResponseFormat.Json)>
    Public Function ContarNotas(ByVal idTarea As Long) As ResultadoNotasDto
        Try
            Dim resultadoSesion As ResultadoContextoSesionWorkflow = New WorkflowPreviewSessionContextGate().AsegurarContextoNotas()
            If Not ContextoDisponible(resultadoSesion) Then Return RespuestaBloqueada()
            Dim resultado As ResultadoNotasWorkflow = CrearServicio(resultadoSesion).Contar(resultadoSesion.Contexto,
                New SolicitudContarNotasWorkflow With {.IdTarea = idTarea})
            Return Mapear(resultado)
        Catch
            Return RespuestaBloqueada()
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    <System.Web.Script.Services.ScriptMethod(ResponseFormat:=System.Web.Script.Services.ResponseFormat.Json)>
    Public Function CrearNota(ByVal idTarea As Long,
                              ByVal contenido As String,
                              ByVal clientRequestId As String) As ResultadoNotasDto
        Try
            Dim resultadoSesion As ResultadoContextoSesionWorkflow = New WorkflowPreviewSessionContextGate().AsegurarContextoNotas()
            If Not ContextoDisponible(resultadoSesion) Then Return RespuestaBloqueada()
            Dim resultado As ResultadoNotasWorkflow = CrearServicio(resultadoSesion).Crear(resultadoSesion.Contexto,
                New SolicitudCrearNotaWorkflow With {.IdTarea = idTarea, .Contenido = contenido, .IdSolicitudCliente = clientRequestId})
            Return Mapear(resultado)
        Catch
            Return RespuestaBloqueada()
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    <System.Web.Script.Services.ScriptMethod(ResponseFormat:=System.Web.Script.Services.ResponseFormat.Json)>
    Public Function ActualizarNota(ByVal idTarea As Long,
                                   ByVal idNota As Long,
                                   ByVal contenido As String,
                                   ByVal version As String) As ResultadoNotasDto
        Try
            Dim resultadoSesion As ResultadoContextoSesionWorkflow = New WorkflowPreviewSessionContextGate().AsegurarContextoNotas()
            If Not ContextoDisponible(resultadoSesion) Then Return RespuestaBloqueada()
            Dim resultado As ResultadoNotasWorkflow = CrearServicio(resultadoSesion).Actualizar(resultadoSesion.Contexto,
                New SolicitudActualizarNotaWorkflow With {.IdTarea = idTarea, .IdNota = idNota, .Contenido = contenido, .Version = version})
            Return Mapear(resultado)
        Catch
            Return RespuestaBloqueada()
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    <System.Web.Script.Services.ScriptMethod(ResponseFormat:=System.Web.Script.Services.ResponseFormat.Json)>
    Public Function EliminarNota(ByVal idTarea As Long,
                                 ByVal idNota As Long,
                                 ByVal version As String) As ResultadoNotasDto
        Try
            Dim resultadoSesion As ResultadoContextoSesionWorkflow = New WorkflowPreviewSessionContextGate().AsegurarContextoNotas()
            If Not ContextoDisponible(resultadoSesion) Then Return RespuestaBloqueada()
            Dim resultado As ResultadoNotasWorkflow = CrearServicio(resultadoSesion).Eliminar(resultadoSesion.Contexto,
                New SolicitudEliminarNotaWorkflow With {.IdTarea = idTarea, .IdNota = idNota, .Version = version})
            Return Mapear(resultado)
        Catch
            Return RespuestaBloqueada()
        End Try
    End Function

    Private Shared Function ContextoDisponible(ByVal resultadoSesion As ResultadoContextoSesionWorkflow) As Boolean
        Return resultadoSesion IsNot Nothing AndAlso resultadoSesion.Contexto IsNot Nothing AndAlso
               resultadoSesion.Contexto.EsValido() AndAlso Not String.IsNullOrWhiteSpace(resultadoSesion.CadenaConexionWorkflow)
    End Function

    Private Shared Function CrearServicio(ByVal resultadoSesion As ResultadoContextoSesionWorkflow) As ServicioNotasWorkflow
        Dim factory As New WorkflowModuleConnectionFactory(resultadoSesion.CadenaConexionWorkflow)
        Dim executor As New AdoNetDataExecutor()
        Return New ServicioNotasWorkflow(
            New MySqlTareaWorkflowRepository(factory, executor),
            New MySqlNotasWorkflowRepository(factory, executor),
            New NotasWorkflowCursorCodec())
    End Function

    Private Shared Function Mapear(ByVal resultado As ResultadoNotasWorkflow) As ResultadoNotasDto
        If resultado Is Nothing Then Return RespuestaBloqueada()

        Dim dto As New ResultadoNotasDto With {
            .Exito = resultado.EsExitoso,
            .Codigo = resultado.Codigo,
            .CodigoBloqueo = If(resultado.EsExitoso, String.Empty, resultado.Codigo),
            .MensajeFuncional = resultado.MensajeFuncional,
            .Contador = resultado.Contador,
            .CursorSiguiente = If(resultado.CursorSiguiente, String.Empty),
            .TieneMas = resultado.TieneMas
        }
        If resultado.Notas IsNot Nothing Then
            For Each nota As NotaWorkflow In resultado.Notas
                dto.Notas.Add(MapearNota(nota))
            Next
        End If
        dto.Nota = MapearNota(resultado.Nota)
        Return dto
    End Function

    Private Shared Function MapearNota(ByVal nota As NotaWorkflow) As NotaWorkflowDto
        If nota Is Nothing Then Return Nothing
        Return New NotaWorkflowDto With {
            .IdNota = nota.IdNota,
            .IdTarea = nota.IdTarea,
            .Contenido = If(nota.Contenido, String.Empty),
            .Version = If(nota.Version, String.Empty),
            .FechaCreacionUtc = nota.FechaCreacionUtc
        }
    End Function

    Private Shared Function RespuestaBloqueada() As ResultadoNotasDto
        Return New ResultadoNotasDto With {
            .Exito = False,
            .Codigo = CodigosResultadoNotasWorkflow.Unavailable,
            .CodigoBloqueo = CodigosResultadoNotasWorkflow.Unavailable,
            .MensajeFuncional = "No fue posible consultar las notas de la tarea."
        }
    End Function
End Class
