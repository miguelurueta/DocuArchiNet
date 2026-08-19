Imports System
Imports System.Collections.Generic

'Conserva el único requisito previo del envío legacy a grupo: no tener solicitudes de aprobación sin decisión.
Public Class WorkflowLegacyEnvioGrupoRequisitosAdapter
    Implements IEnvioGrupoRequisitosRepository

    Public Function Evaluar(ByVal contexto As ContextoModuloWorkflow,
                            ByVal tarea As TareaWorkflow,
                            ByVal destino As DestinoEnvioGrupoWorkflow) As ResultadoRequisitosEnvioGrupo Implements IEnvioGrupoRequisitosRepository.Evaluar
        If contexto Is Nothing OrElse Not contexto.EsValido() OrElse contexto.IdUsuarioGestion <= 0 OrElse
           tarea Is Nothing OrElse Not tarea.EstaActiva OrElse destino Is Nothing OrElse destino.IdActividadDestino <= 0 Then
            Return Bloquear(CodigosBloqueoPrevisualizacion.ContextoInvalido,
                            "No fue posible validar los requisitos de la tarea.",
                            "WORKFLOW_CONTEXT_VALID")
        End If

        Try
            Dim estadoAprobacion As String = String.Empty
            Dim resultadoAprobacion As String = New ClassRaSolicitudesAprobacion().Verifica_solicitudes_de_aprobacion_sin_desicion(
                CInt(tarea.IdTarea),
                estadoAprobacion,
                contexto.IdUsuarioGestion)
            If Not String.Equals(resultadoAprobacion, "YES", StringComparison.OrdinalIgnoreCase) Then
                Return Bloquear(CodigosBloqueoPrevisualizacion.TransicionNoDisponible,
                                "No fue posible validar las solicitudes de aprobacion de la tarea.",
                                "WORKFLOW_APPROVAL_AVAILABLE")
            End If
            If String.Equals(estadoAprobacion, "YES", StringComparison.OrdinalIgnoreCase) Then
                Return Bloquear(CodigosBloqueoPrevisualizacion.AprobacionPendiente,
                                "La tarea tiene solicitudes de aprobacion pendientes.",
                                "WORKFLOW_APPROVAL_REQUIRED")
            End If

            Return New ResultadoRequisitosEnvioGrupo With {
                .Cumple = True,
                .Requisitos = New List(Of RequisitoTransicion) From {
                    New RequisitoTransicion With {
                        .Codigo = "WORKFLOW_APPROVAL_REQUIRED",
                        .Descripcion = "No existen solicitudes de aprobacion pendientes.",
                        .Obligatorio = True,
                        .Satisfecho = True
                    }}
            }
        Catch
            Return Bloquear(CodigosBloqueoPrevisualizacion.TransicionNoDisponible,
                            "No fue posible validar los requisitos de la tarea.",
                            "WORKFLOW_REQUIREMENTS_AVAILABLE")
        End Try
    End Function

    Private Shared Function Bloquear(ByVal codigo As String,
                                     ByVal mensaje As String,
                                     ByVal codigoRequisito As String) As ResultadoRequisitosEnvioGrupo
        Return New ResultadoRequisitosEnvioGrupo With {
            .Cumple = False,
            .CodigoBloqueo = codigo,
            .MensajeFuncional = mensaje,
            .Requisitos = New List(Of RequisitoTransicion) From {
                New RequisitoTransicion With {
                    .Codigo = codigoRequisito,
                    .Descripcion = mensaje,
                    .Obligatorio = True,
                    .Satisfecho = False
                }}
        }
    End Function
End Class
