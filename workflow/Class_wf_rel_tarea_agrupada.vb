Public Class CdTareaListaAgrupaPadre
    Public Obj_ilist_row_generic As Object     'Seralizado DATA-SET
    Public Obj_ilist_fileds_generic As Object  'class_campos_table_bostra_table
    Public NameTabla As Object
End Class
Public Class CdTareaListaTareaAgrupa
    Public Obj_ilist_row_generic As Object     'Seralizado DATA-SET
    Public Obj_ilist_fileds_generic As Object  'class_campos_table_bostra_table
    Public NameTabla As Object
    Public NombreCampoRadicado As Object
End Class
Public Class Class_wf_rel_tarea_agrupada
    Function SolicitaListaEstructuraTareaAgrupadaPadre(ByVal IdTareaWorkflowPadre As Long,
                                                       ByVal NombreRutaWorkflow As String,
                                                       ByRef CdTareaListaAgrupaPadre As CdTareaListaAgrupaPadre) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la estructura con la lista de tareas agrupadas padre de una tarea
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdTareaWorkflowPadre : Representa la identificación de la tarea padre 
        'NombreRutaWorkflow   : Representa el nombre de la ruta workflow
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'CdTareaListaAgrupaPadre  : Retorna la estructura de la lista de tareas padres agrupadas
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-06-06
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Class_campos_titulo_tareas_padre As New Class_wf_rel_config_campos_titulo_tareas_padre
            Dim CdConfigCamposTituloTareasPadre As New List(Of CdConfigCamposTituloTareasPadre)
            Result = Class_campos_titulo_tareas_padre.SolicitaEstructuraCamposTituloTareasPadre(CdConfigCamposTituloTareasPadre)
            If Result <> "YES" Then
                SolicitaListaEstructuraTareaAgrupadaPadre = Result
                Exit Function
            End If
            Dim class_campos_table_bostra_table As New List(Of class_campos_table_bostra_table)
            Result = Class_campos_titulo_tareas_padre.SolicitaCamposListaTareasPadre(CdConfigCamposTituloTareasPadre,
                                                                                     class_campos_table_bostra_table)
            If Result <> "YES" Then
                SolicitaListaEstructuraTareaAgrupadaPadre = Result
                Exit Function
            End If
            CdTareaListaAgrupaPadre.Obj_ilist_fileds_generic = class_campos_table_bostra_table
            Dim NameTableRuta As String = "DAT_ADIC_TAR" & NombreRutaWorkflow
            Dim NameCampoAgrupacion As String = ""
            Dim SqlConsulta As String = ""
            For i As Integer = 0 To CdConfigCamposTituloTareasPadre.Count - 1
                If CdConfigCamposTituloTareasPadre.Item(i).CampoAgrupaCondicion = 1 Then
                    NameCampoAgrupacion = CdConfigCamposTituloTareasPadre.Item(i).NombreCampo
                    Exit For
                End If
            Next
            If NameCampoAgrupacion = "" Then
                SolicitaListaEstructuraTareaAgrupadaPadre = "Es necesario definir un campo de agrupación en la configuración de agrupamiento."
                Exit Function
            End If
            Result = Class_campos_titulo_tareas_padre.SolicitaSqlConsultaTareasPadre(NameTableRuta,
                                                                                     IdTareaWorkflowPadre,
                                                                                     NameCampoAgrupacion,
                                                                                     class_campos_table_bostra_table,
                                                                                     SqlConsulta)
            If Result <> "YES" Then
                SolicitaListaEstructuraTareaAgrupadaPadre = Result
                Exit Function
            End If
            Result = Class_campos_titulo_tareas_padre.SolicitaRowTableConsultaTareasPadre(SqlConsulta,
                                                                                          CdTareaListaAgrupaPadre.Obj_ilist_row_generic)
            If Result <> "YES" Then
                SolicitaListaEstructuraTareaAgrupadaPadre = Result
                Exit Function
            End If
            SolicitaListaEstructuraTareaAgrupadaPadre = "YES"
        Catch ex As Exception
            SolicitaListaEstructuraTareaAgrupadaPadre = "Inconsistencia general funcion SolicitaListaEstructuraTareaAgrupadaPadre " & ex.Message
        End Try
    End Function
    Function SolicitaListaEstructuraTareaAgrupada(ByVal IdTareaWorkflowPadre As Long,
                                                  ByVal IdTareaGrupada As Long,
                                                  ByVal IdRutaWorkflow As Integer,
                                                  ByVal NombreRutaWorkflow As String,
                                                  ByRef CdTareaListaTareaAgrupa As CdTareaListaTareaAgrupa) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la estructura con la lista de tareas agrupadas 
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdTareaWorkflowPadre : Representa la identificación de la tarea padre 
        'NombreRutaWorkflow   : Representa el nombre de la ruta workflow
        'IdTareaGrupada       : Representa la identificación de la tarea agrupada
        'IdRutaWorkflow       : Representa la identificación de la ruta
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'CdTareaListaTareaAgrupa  : Retorna la estructura de la lista de tareas agrupadas
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-06-13
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Class_wf_rel_config_campo_titulo_tarea_agrupada As New Class_wf_rel_config_campo_titulo_tarea_agrupada
            Dim CdConfigCamposTituloTareaAgrupada As New List(Of CdConfigCamposTituloTareaAgrupada)
            Result = Class_wf_rel_config_campo_titulo_tarea_agrupada.SolicitaEstructuraCamposTituloTareaAgrupada(CdConfigCamposTituloTareaAgrupada)
            If Result <> "YES" Then
                SolicitaListaEstructuraTareaAgrupada = Result
                Exit Function
            End If
            Dim NombreCampoRadicado As String = ""
            Dim Class_configuracion_listado_ruta As New Class_configuracion_listado_ruta
            Result = Class_configuracion_listado_ruta.SolicitaNombreCampoRadicadoRuta(IdRutaWorkflow,
                                                                                      NombreCampoRadicado)
            If Result <> "YES" Then
                SolicitaListaEstructuraTareaAgrupada = Result
                Exit Function
            End If
            CdTareaListaTareaAgrupa.NombreCampoRadicado = NombreCampoRadicado
            Dim class_campos_table_bostra_table As New List(Of class_campos_table_bostra_table)
            Result = Class_wf_rel_config_campo_titulo_tarea_agrupada.SolicitaCamposListaTareaAgrupada(CdConfigCamposTituloTareaAgrupada,
                                                                                                      NombreCampoRadicado,
                                                                                                      class_campos_table_bostra_table)
            If Result <> "YES" Then
                SolicitaListaEstructuraTareaAgrupada = Result
                Exit Function
            End If
            Dim NombreCampoOrdena As String = ""
            Dim NameTableRuta As String = "DAT_ADIC_TAR" & NombreRutaWorkflow
            CdTareaListaTareaAgrupa.Obj_ilist_fileds_generic = class_campos_table_bostra_table
            Dim SqlConsulta As String = ""
            For i As Integer = 0 To CdConfigCamposTituloTareaAgrupada.Count - 1
                If CdConfigCamposTituloTareaAgrupada.Item(i).OrderbyCampo = 1 Then
                    If NombreCampoOrdena = "" Then
                        NombreCampoOrdena = CdConfigCamposTituloTareaAgrupada.Item(i).NombreCampo
                    Else
                        NombreCampoOrdena = "," & CdConfigCamposTituloTareaAgrupada.Item(i).NombreCampo
                    End If

                End If
            Next
            Result = Class_wf_rel_config_campo_titulo_tarea_agrupada.SolicitaSqlConsultaTareasAgrupadas(NameTableRuta,
                                                                                                        IdTareaWorkflowPadre,
                                                                                                        IdTareaGrupada,
                                                                                                        NombreCampoOrdena,
                                                                                                        class_campos_table_bostra_table,
                                                                                                        SqlConsulta)
            If Result <> "YES" Then
                SolicitaListaEstructuraTareaAgrupada = Result
                Exit Function
            End If
            Result = Class_wf_rel_config_campo_titulo_tarea_agrupada.SolicitaRowTableConsultaTareasAgrupadas(SqlConsulta,
                                                                                                             CdTareaListaTareaAgrupa.Obj_ilist_row_generic)
            If Result <> "YES" Then
                SolicitaListaEstructuraTareaAgrupada = Result
                Exit Function
            End If
            SolicitaListaEstructuraTareaAgrupada = "YES"
        Catch ex As Exception
            SolicitaListaEstructuraTareaAgrupada = "Inconsistencia general funcion SolicitaListaEstructuraTareaAgrupada " & ex.Message
        End Try
    End Function
End Class
