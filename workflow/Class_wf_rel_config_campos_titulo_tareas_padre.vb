Imports Newtonsoft.Json

Public Class CdConfigCamposTituloTareasPadre
    Property IdwfRelConfigCampoTituloTareaPadre As Integer
    Property ConfiguracionListadoRutaIdConfiguracion As Integer
    Property CampoAgrupaCondicion As Integer
    Property CampoLista As Integer
    Property OrdenCampo As Integer
    Property FechaRegistro As String
    Property OrderbyCampo As Integer
    Property AleasCampo As String
    Property NombreCampo As String
    Property CampoAtriButo As Integer
    Property CampoVisible As Integer
End Class

Public Class Class_wf_rel_config_campos_titulo_tareas_padre
    Function SolicitaEstructuraCamposTituloTareasPadre(ByRef CdConfigCamposTituloTareasPadre As List(Of CdConfigCamposTituloTareasPadre)) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita lista estructura campos titulos tareas padres de agrupación
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        '
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'CdConfigCamposTituloTareasPadre  : Retorna la estructura con los campos para la lista
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-06-05
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = "Select wrctp.idwf_rel_config_campos_agrupa_padre,wrctp.configuracion_listado_ruta_Id_Configuracion," &
                "wrctp.CampoAgrupaCondicion,wrctp.CampoLista,wrctp.AleasCampo,wrctp.OrdenCampo,wrctp.FechaRegistro,wrctp.OrderbyCampo," &
                "wrctp.CampoAtriButo,wrctp.CampoVisible,clr.Nombre_Campo" &
                " from wf_rel_config_campos_titulo_tareas_padre as wrctp " &
                " INNER JOIN configuracion_listado_ruta AS clr on (clr.Id_Configuracion=wrctp.configuracion_listado_ruta_Id_Configuracion)" &
                " order by wrctp.OrdenCampo "
            Dim SqloConsulta As String = ""
            Dim ConectDataBase As New conect.Dbase_Conction_Mysql
            Dim DataSet As DataSet = New DataSet("wf_rel_config_campos_titulo_tareas_padre")
            Result = ConectDataBase.SELECTION_SELECT_FIELD(SqloConsulta, DataSet)
            If Result <> "YES" Then
                SolicitaEstructuraCamposTituloTareasPadre = "Inconsistencia detectada en la función SolicitaEstructuraCamposTituloTareasPadre : " & Result
                Exit Function
            End If
            If DataSet.Tables(0).Rows.Count = 0 Then
                SolicitaEstructuraCamposTituloTareasPadre = "No fue posible encontrar los campos de configuración correspondientes a la lista de tareas padres agrupadas."
                Exit Function
            Else
                Dim ItemTituloPadre As CdConfigCamposTituloTareasPadre
                For i As Integer = 0 To DataSet.Tables(0).Rows.Count - 1
                    ItemTituloPadre = New CdConfigCamposTituloTareasPadre
                    ItemTituloPadre.IdwfRelConfigCampoTituloTareaPadre = DataSet.Tables(0).Rows(i).Item(0)
                    ItemTituloPadre.ConfiguracionListadoRutaIdConfiguracion = DataSet.Tables(0).Rows(i).Item(1)
                    ItemTituloPadre.CampoAgrupaCondicion = DataSet.Tables(0).Rows(i).Item(2)
                    ItemTituloPadre.CampoLista = DataSet.Tables(0).Rows(i).Item(3)
                    ItemTituloPadre.AleasCampo = DataSet.Tables(0).Rows(i).Item(4)
                    ItemTituloPadre.OrdenCampo = DataSet.Tables(0).Rows(i).Item(5)
                    ItemTituloPadre.FechaRegistro = DataSet.Tables(0).Rows(i).Item(5)
                    ItemTituloPadre.OrderbyCampo = DataSet.Tables(0).Rows(i).Item(6)
                    ItemTituloPadre.CampoAtriButo = DataSet.Tables(0).Rows(i).Item(7)
                    ItemTituloPadre.CampoVisible = DataSet.Tables(0).Rows(i).Item(8)
                    ItemTituloPadre.NombreCampo = DataSet.Tables(0).Rows(i).Item(9)
                    CdConfigCamposTituloTareasPadre.Add(ItemTituloPadre)
                Next
            End If
        Catch ex As Exception
            SolicitaEstructuraCamposTituloTareasPadre = "Inconsistencia general funcion SolicitaEstructuraCamposTituloTareasPadre " & ex.Message
        End Try
    End Function
    Function SolicitaCamposListaTareasPadre(ByVal CdConfigCamposTituloTareasPadre As List(Of CdConfigCamposTituloTareasPadre),
                                            ByRef class_campos_table_bostra_table As List(Of class_campos_table_bostra_table)) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la estructura de campos lista tareas padres agrupadas
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'CdConfigCamposTituloTareasPadre   : Representa la estructura de campos 
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'class_campos_table_bostra_table  : Retorna la estructura de los campos para BootTable
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-05-06
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim ItemBooTable As New class_campos_table_bostra_table
            ItemBooTable = New class_campos_table_bostra_table
            ItemBooTable.title = "id_tarea_tarea_workflow_padre"
            ItemBooTable.field = "id_tarea_tarea_workflow_padre"
            ItemBooTable.visible = False
            ItemBooTable.viisble_sql = 1
            ItemBooTable.visible_like_sql = 1
            class_campos_table_bostra_table.Add(ItemBooTable)
            ItemBooTable = New class_campos_table_bostra_table
            ItemBooTable.title = "IdTareaWorkflowAgrupa"
            ItemBooTable.field = "IdTareaWorkflowAgrupa"
            ItemBooTable.visible = False
            ItemBooTable.viisble_sql = 1
            ItemBooTable.visible_like_sql = 1
            class_campos_table_bostra_table.Add(ItemBooTable)
            ItemBooTable = New class_campos_table_bostra_table
            ItemBooTable.title = "CANTIDAD"
            ItemBooTable.field = "CANTIDAD"
            ItemBooTable.visible = False
            ItemBooTable.viisble_sql = 1
            ItemBooTable.visible_like_sql = 1
            class_campos_table_bostra_table.Add(ItemBooTable)
            '//-------Agrega campos dinamicos-------// 
            For i As Integer = 0 To CdConfigCamposTituloTareasPadre.Count - 1
                ItemBooTable = New class_campos_table_bostra_table
                If CdConfigCamposTituloTareasPadre.Item(i).CampoVisible = 1 Then
                    ItemBooTable.visible = True
                Else
                    ItemBooTable.visible = False
                End If
                ItemBooTable.viisble_sql = 1
                ItemBooTable.visible_like_sql = 1
                ItemBooTable.title = CdConfigCamposTituloTareasPadre.Item(i).AleasCampo
                ItemBooTable.field = CdConfigCamposTituloTareasPadre.Item(i).NombreCampo
                class_campos_table_bostra_table.Add(ItemBooTable)
            Next
            ItemBooTable = New class_campos_table_bostra_table
            ItemBooTable.field = "operate"
            ItemBooTable.title = ""
            ItemBooTable.checkbox = False
            ItemBooTable.visible = True
            ItemBooTable.viisble_sql = 0
            ItemBooTable.clickToSelect = False
            ItemBooTable.visible_like_sql = 0
            ItemBooTable.align = "center"
            ItemBooTable.events = "window.operateEventsAgrupadosPadre"
            ItemBooTable.formatter = "operateFormattertablebootAgrupadosPadre"
            class_campos_table_bostra_table.Add(ItemBooTable)
        Catch ex As Exception
            SolicitaCamposListaTareasPadre = "Inconsistencia general funcion SolicitaCamposListaTareasPadre " & ex.Message
        End Try
    End Function
    Function SolicitaSqlConsultaTareasPadre(ByVal NameTableRuta As String,
                                            ByVal IdTareaWorkflowPadre As Long,
                                            ByVal NameCampoAgrupacion As String,
                                            ByVal class_campos_table_bostra_table As List(Of class_campos_table_bostra_table),
                                            ByRef SqlConsulta As String) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita el sql de consulta tareas padre agrupadas
        '         
        '          
        '         
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'NameTableRuta           : Representa el nombre de la tabla de DAT de la ruta
        'IdTareaWorkflowPadre    : Representa la identificación de la tarea workflow
        'NameCampoAgrupacion     : Representa el nombre del campo agrupación
        'class_campos_table_bostra_table : Representa la estructura de los campos tipo
        'TableBot 
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'SqlConsulta  : Retorna comando sql de consulta
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2025-06-05
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim CondicionSql As String = " where "
            Dim SelecCampos As String = "select COUNT(wrrta. id_tarea_tarea_workflow_child) as CANTIDAD," &
                "wrrta.id_tarea_tarea_workflow_padre,wrrta.IdTareaWorkflowAgrupa,"
            For i As Integer = 0 To class_campos_table_bostra_table.Count - 1
                SelecCampos = SelecCampos & "," & "dat." & class_campos_table_bostra_table(i).field
            Next
            CondicionSql = CondicionSql & " wrrta.id_tarea_tarea_workflow_padre " & "=" & IdTareaWorkflowPadre
            Dim Sqlfrom As String = " From " & NameTableRuta & " as wrrta "
            Dim InnerJoin As String = " inner join " & NameTableRuta & " as dat on (wrrta.id_tarea_tarea_workflow_child=dat.INICIO_TAREAS_WORKFLOW_ID_TAREA) "
            Dim GroupBy As String = ""
            Dim OrderBy As String = ""
            If NameCampoAgrupacion <> "" Then
                GroupBy = " group by dat." & NameCampoAgrupacion
                OrderBy = " order by dat." & NameCampoAgrupacion
            End If
            SqlConsulta = SelecCampos & " " & Sqlfrom & InnerJoin & CondicionSql & GroupBy & OrderBy
            SolicitaSqlConsultaTareasPadre = "YES"
        Catch ex As Exception
            SolicitaSqlConsultaTareasPadre = "Inconsistencia general funcion SolicitaSqlConsultaTareasPadre " & ex.Message
        End Try
    End Function
    Function SolicitaRowTableConsultaTareasPadre(ByVal SqlConsulta As String,
                                                 ByRef Obj_ilist_row_generic As Object) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita la estructura generica con los datos de la consulta
        '          de las tareas padres agrupadas
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'SqlConsulta               : Representa la consulta en comando SQL
        '                        
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'Obj_ilist_row_generic  : Retorna la estructura de datos de la consulta
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2025-06-05
        'Elabora               : Miguel Angel Urueta Miranda
        '---------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("table")
            Result = ref.SELECTION_SELECT_FIELDA(SqlConsulta, Datset)
            If Result <> "YES" Then
                SolicitaRowTableConsultaTareasPadre = "Inconsistencia detectada en la función  SolicitaRowTableConsultaTareasPadre : " & Result
                Exit Function
            End If
            Obj_ilist_row_generic = JsonConvert.SerializeObject(Datset.Tables(0))
            SolicitaRowTableConsultaTareasPadre = "YES"
            Exit Function
        Catch ex As Exception
            SolicitaRowTableConsultaTareasPadre = "Inconsistencia general funcion SolicitaRowTableConsultaTareasPadre " & ex.Message
        End Try
    End Function
End Class
