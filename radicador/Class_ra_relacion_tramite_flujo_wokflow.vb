Imports GestionDocumental_Docuarchi.net.Class_config_general_service

Public Class Class_ra_relacion_tramite_flujo_wokflow
    Function Solicita_id_flujo_relacion_flujo_tramite(ByVal id_tipo_tramite As Integer,
                                                      ByRef id_flujo_trabajo As Integer) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita la existencia de flujo de trabajo relacionado al tramte
        '          
        '          
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_tipo_tramite    : Representa la identificación del tipo de tramite
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'id_flujo_trabajo  : Retorna la identificación del flujo de trabajo
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-11-15
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim SQL_consulta As String = "SELECT ID_WF_FLUJO_TRABAJO FROM ra_relacion_tramite_flujo_wokflow " &
          " where tipo_doc_entrante_id_Tipo_Doc_Entrante=" & id_tipo_tramite
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_relacion_tramite_flujo_wokflow")
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(SQL_consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_flujo_relacion_flujo_tramite = "Función Solicita_id_flujo_relacion_flujo_tramite dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_flujo_trabajo = 0
                Solicita_id_flujo_relacion_flujo_tramite = "YES"
                Exit Function
            Else
                id_flujo_trabajo = Datset.Tables(0).Rows(0).Item(0)
                Solicita_id_flujo_relacion_flujo_tramite = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_flujo_relacion_flujo_tramite = "Inconsistencia general funcion Solicita_id_flujo_relacion_flujo_tramite " & ex.Message
        End Try
    End Function
    Function Solicita_existencia_relacion_flujo_tramite(ByVal id_tipo_tramite As Integer,
                                                        ByRef estado_existencia As Integer) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita la existencia de la relación de un tipo tramite con 
        '          flujos de trabajo
        '          
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_tipo_tramite    : Representa la identificación del tipo de tramite
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'estado_existencia  : Retorna el estado de existencia 1 - existe    0- no existe 
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-08-05
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim SQL_consulta As String = "SELECT ID_WF_FLUJO_TRABAJO FROM ra_relacion_tramite_flujo_wokflow " &
          " where tipo_doc_entrante_id_Tipo_Doc_Entrante=" & id_tipo_tramite
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_relacion_tramite_flujo_wokflow")
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(SQL_consulta, Datset)
            If Result <> "YES" Then
                Solicita_existencia_relacion_flujo_tramite = "Función Solicita_relaciones_flujo_trabajo_tramite dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado_existencia = 0
                Solicita_existencia_relacion_flujo_tramite = "YES"
                Exit Function
            Else
                estado_existencia = 1
                Solicita_existencia_relacion_flujo_tramite = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_existencia_relacion_flujo_tramite = "Inconsistencia general funcion Solicita_existencia_relacion_flujo_tramite " & ex.Message
        End Try
    End Function
    Function Solicita_relaciones_flujo_trabajo_tramite(ByVal id_tipo_tramite As Integer,
                                                       ByVal opton_blank As Integer,
                                                       ByRef Class_service_ilist_drowlist As IList(Of Class_service_ilist_drowlist)) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Asigna la estructura de las actividades de ala ruta workflow
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        '
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'Class_service_ilist_drowlist  : Retorna la estructura generica de los flujos de trabajo
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2024-12-04
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Datset As DataSet = New DataSet("LISTADO_ACTIVIDADES_WORKFLOW")
            Dim Parametro_Consulta As String = "SELECT ID_WF_FLUJO_TRABAJO,NOMBRE_FLUJO_TRABAJO FROM ra_relacion_tramite_flujo_wokflow " &
           " where tipo_doc_entrante_id_Tipo_Doc_Entrante=" & id_tipo_tramite
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_relaciones_flujo_trabajo_tramite = "Función Solicita_relaciones_flujo_trabajo_tramite dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_relaciones_flujo_trabajo_tramite = "YES"
                Exit Function
            Else
                Dim Item As New Class_service_ilist_drowlist
                Item.value_campo = ""
                Item.id_value = "0"
                If opton_blank = 1 Then
                    Class_service_ilist_drowlist.Add(Item)
                End If
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    Item = New Class_service_ilist_drowlist
                    Item.value_campo = Datset.Tables(0).Rows(i).Item(1)
                    Item.id_value = Datset.Tables(0).Rows(i).Item(0)
                    Class_service_ilist_drowlist.Add(Item)
                Next
                Solicita_relaciones_flujo_trabajo_tramite = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_relaciones_flujo_trabajo_tramite = "Inconistencia general función  Solicita_class_actividades_workflow_ruta " & ex.Message
        End Try
    End Function
    Function Solicita_relaciones_flujo_trabajo_tramite(ByVal id_tipo_tramite As Integer,
                                                       ByVal opton_blank As Integer,
                                                       ByRef drop_list As DropDownList) As String
        Try
            drop_list.Items.Clear()
            Dim Parametro_Consulta As String = "SELECT ID_WF_FLUJO_TRABAJO,NOMBRE_FLUJO_TRABAJO FROM ra_relacion_tramite_flujo_wokflow " &
           " where tipo_doc_entrante_id_Tipo_Doc_Entrante=" & id_tipo_tramite
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_relacion_tramite_flujo_wokflow")
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_relaciones_flujo_trabajo_tramite = "Función Solicita_relaciones_flujo_trabajo_tramite dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                drop_list.Items.Clear()
                Solicita_relaciones_flujo_trabajo_tramite = "YES"
                Exit Function
            Else
                drop_list.Items.Clear()
                Dim ilist As New ListItem
                ilist.Value = 0
                ilist.Text = ""
                If opton_blank = 1 Then
                    drop_list.Items.Add(ilist)
                End If
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ilist = New ListItem
                    ilist.Value = Datset.Tables(0).Rows(i).Item(0)
                    ilist.Text = Datset.Tables(0).Rows(i).Item(1)
                    drop_list.Items.Add(ilist)
                Next
                Solicita_relaciones_flujo_trabajo_tramite = "YES"
                Exit Function
            End If
            Solicita_relaciones_flujo_trabajo_tramite = "YES"
            Exit Function
        Catch ex As Exception
            Solicita_relaciones_flujo_trabajo_tramite = "Inconsistencia general función Solicita_relaciones_flujo_trabajo_tramite " & ex.Message
        End Try
    End Function
    Function Solicita_relaciones_flujo_trabajo_tramite_default(ByVal id_tipo_tramite As Integer,
                                                               ByVal id_flujo_trabajo As Integer,
                                                               ByRef drop_list As DropDownList) As String
        Try
            drop_list.Items.Clear()
            Dim Parametro_Consulta As String = "SELECT ID_WF_FLUJO_TRABAJO,NOMBRE_FLUJO_TRABAJO FROM ra_relacion_tramite_flujo_wokflow " &
           " where tipo_doc_entrante_id_Tipo_Doc_Entrante=" & id_tipo_tramite
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_relacion_tramite_flujo_wokflow")
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_relaciones_flujo_trabajo_tramite_default = "Función Solicita_relaciones_flujo_trabajo_tramite_default dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                drop_list.Items.Clear()
                Solicita_relaciones_flujo_trabajo_tramite_default = "YES"
                Exit Function
            Else
                drop_list.Items.Clear()
                Dim ilist As New ListItem
                ilist.Value = 0
                ilist.Text = ""
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ilist = New ListItem
                    ilist.Value = Datset.Tables(0).Rows(i).Item(0)
                    ilist.Text = Datset.Tables(0).Rows(i).Item(1)
                    drop_list.Items.Add(ilist)
                Next
                For i As Integer = 0 To drop_list.Items.Count - 1
                    If drop_list.Items(i).Value = id_flujo_trabajo Then
                        drop_list.Items(i).Selected = True
                        Exit For
                    End If
                Next
                Solicita_relaciones_flujo_trabajo_tramite_default = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_relaciones_flujo_trabajo_tramite_default = "Inconsistencia general función Solicita_relaciones_flujo_trabajo_tramite_default " & ex.Message
        End Try
    End Function

End Class
