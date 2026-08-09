Public Class CdTareasWorkflow
    Property IdTareaWorkflow As Long
    Property Radicado As String
    Property Tramite As String
    Property NombreRuta As String
    Property BeneficiarioTarea As String
    Property EstadoFlujoRuta As Integer
    Property NombreFlujo As String
    Property IdFlujoTrabajo As Integer
    Property EstadoOptionEnviarUsuario As Integer
    Property EstadoOptionEnviarGrupo As Integer
    Property EstadoOpionAutoTerminar As Integer
    Property EstadoOptionChekAutoriza As Integer
End Class
Public Class Class_TareasWorkflow
    Function SolicitaDaosTareaAsignadaWorkflow(ByVal IdTareaWorkflow As Long,
                                               ByVal IdRutaWorkflow As Integer,
                                               ByVal IdUsuarioWorkflow As Integer,
                                               ByVal NombreRutaWorkflow As String,
                                               ByVal IdActividadWorkflow As Integer,
                                               ByVal RadicadoTarea As String,
                                               ByVal IdFlujoTarea As Integer,
                                               ByRef CdTareasWorkflow As CdTareasWorkflow) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita datos de aignción de una tarea workflow
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdTareaWorkflow     : Representa la identificación de la tarea
        'IdRutaWorkflow      : Representa la identificación de la ruta workflow
        'NombreRutaWorkflow  : Representa el nombre de la ruta workflow
        'IdActividadWorkflow : Rpresenta la identificacion de actividad del usuario workflow
        'RadicadoTarea       : Repreenta la identificación del consecutvo de de radicado
        'IdFlujoTarea        : Representa la identificación de un flujo de taarea 
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'CdTareasWorkflow  : Retorna la estructura de una tarea asignada
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-06-09
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try
            Dim Result As String = ""
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim Class_flujo_trabajo_workflow As New Class_flujo_trabajo_workflow
            Dim Class_worflow_rutas As New Class_worflow_rutas
            If IdFlujoTarea = 0 Then
                Result = Class_DAT_ADIC_TAR.SolicitaIdFlujoTrabajoIdTareaRutaWorkflow(NombreRutaWorkflow,
                                                                                      IdTareaWorkflow,
                                                                                      IdFlujoTarea)
                If Result <> "YES" Then
                    SolicitaDaosTareaAsignadaWorkflow = Result
                    Exit Function
                End If
            End If
            If IdFlujoTarea <> 0 Then
                Result = Class_flujo_trabajo_workflow.SolicitaEstadoAbiertoCerradoFlujoDocumental(IdFlujoTarea,
                                                                                                   CdTareasWorkflow.EstadoFlujoRuta)
                If Result <> "YES" Then
                    SolicitaDaosTareaAsignadaWorkflow = Result
                    Exit Function
                End If
                Result = Class_flujo_trabajo_workflow.SolicitaNombreFlujoTrabajoPorIdFlujo(IdFlujoTarea,
                                                                                           CdTareasWorkflow.NombreFlujo)
                If Result <> "YES" Then
                    SolicitaDaosTareaAsignadaWorkflow = Result
                    Exit Function
                End If
                If CdTareasWorkflow.EstadoFlujoRuta = 1 Then
                    CdTareasWorkflow.EstadoOptionEnviarUsuario = 0
                    CdTareasWorkflow.EstadoOptionEnviarGrupo = 0
                    CdTareasWorkflow.EstadoOpionAutoTerminar = 0
                Else
                    CdTareasWorkflow.EstadoOptionEnviarUsuario = 1
                    CdTareasWorkflow.EstadoOptionEnviarGrupo = 1
                    CdTareasWorkflow.EstadoOpionAutoTerminar = 0
                End If
            Else
                Result = Class_worflow_rutas.SolicitaEstadoRutaCerradoAbierto(IdTareaWorkflow,
                                                                             IdRutaWorkflow,
                                                                             RadicadoTarea,
                                                                             NombreRutaWorkflow,
                                                                             CdTareasWorkflow.EstadoFlujoRuta,
                                                                             CdTareasWorkflow.Tramite)
                If Result <> "YES" Then
                    SolicitaDaosTareaAsignadaWorkflow = Result
                    Exit Function
                End If
                CdTareasWorkflow.NombreRuta = NombreRutaWorkflow
                If CdTareasWorkflow.EstadoFlujoRuta = 1 Then
                    CdTareasWorkflow.EstadoOptionEnviarUsuario = 0
                    CdTareasWorkflow.EstadoOptionEnviarGrupo = 0
                    CdTareasWorkflow.EstadoOpionAutoTerminar = 0
                Else
                    CdTareasWorkflow.EstadoOptionEnviarUsuario = 1
                    CdTareasWorkflow.EstadoOptionEnviarGrupo = 1
                    CdTareasWorkflow.EstadoOpionAutoTerminar = 0
                End If
            End If
            If HttpContext.Current.Session("CAMBIO_USUARIO") = 0 Then
                CdTareasWorkflow.EstadoOptionEnviarUsuario = 0
            End If
            If HttpContext.Current.Session("Cambio_Ruta") = 0 Then
                CdTareasWorkflow.EstadoOptionEnviarGrupo = 0
            End If
            Dim Class_autoriza_tarea_worklfow As New Class_autoriza_tarea_worklfow
            Dim ExistenciaAutorizacion As String = ""
            Result = Class_autoriza_tarea_worklfow.SolicitaExistenciaAutorizacion(IdTareaWorkflow,
                                                                                  IdActividadWorkflow,
                                                                                  IdUsuarioWorkflow,
                                                                                  ExistenciaAutorizacion)
            If Result <> "YES" Then
                SolicitaDaosTareaAsignadaWorkflow = Result
                Exit Function
            End If
            If ExistenciaAutorizacion = "YES" Then
                CdTareasWorkflow.EstadoOptionChekAutoriza = 1
            Else
                CdTareasWorkflow.EstadoOptionChekAutoriza = 0
            End If
            '---------------------------------------------
            '----Asigna los datos del radicado asignado
            '---------------------------------------------
            Dim Class_configuracion_listado_ruta As New Class_configuracion_listado_ruta
            Dim NombreCampoBeneficiario As String = ""
            Result = Class_configuracion_listado_ruta.SolicitaNombreCampoBenificiarioRuta(IdRutaWorkflow,
                                                                                          NombreCampoBeneficiario)
            If Result <> "YES" Then
                SolicitaDaosTareaAsignadaWorkflow = Result
                Exit Function
            End If
            If NombreCampoBeneficiario <> "" Then
                Result = Class_DAT_ADIC_TAR.SolicitaBeneficiarioTareaWorkflow(NombreRutaWorkflow,
                                                                              NombreCampoBeneficiario,
                                                                              IdTareaWorkflow,
                                                                              CdTareasWorkflow.BeneficiarioTarea)

                If Result <> "YES" Then
                    SolicitaDaosTareaAsignadaWorkflow = Result
                    Exit Function
                End If
            End If
            Dim NombreCampoTramite As String = ""
            If CdTareasWorkflow.Tramite = "" Then
                Result = Class_configuracion_listado_ruta.SolicitaNombreCampoTramiteRuta(IdRutaWorkflow,
                                                                                         NombreCampoTramite)
                If Result <> "YES" Then
                    SolicitaDaosTareaAsignadaWorkflow = Result
                    Exit Function
                End If

                Result = Class_DAT_ADIC_TAR.SolicitaTramiteFlujoWorkflow(IdTareaWorkflow,
                                                                         IdRutaWorkflow,
                                                                         NombreCampoTramite,
                                                                         NombreRutaWorkflow,
                                                                         CdTareasWorkflow.Tramite,
                                                                         0)

                If Result <> "YES" Then
                    SolicitaDaosTareaAsignadaWorkflow = Result
                    Exit Function
                End If
            End If
            CdTareasWorkflow.Radicado = RadicadoTarea
            SolicitaDaosTareaAsignadaWorkflow = "YES"
            Exit Function
        Catch ex As Exception
            SolicitaDaosTareaAsignadaWorkflow = "Inconsistencia general funcion SolicitaDaosTareaAsignadaWorkflow " & ex.Message
        End Try
    End Function
End Class
