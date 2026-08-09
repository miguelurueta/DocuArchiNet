Public Structure actividades_generales_workflow
    Dim Id_Actividad_General As Integer
    Dim Tipo_Actividad As String
    Dim Descripcion_Actividad As String
    Dim Agrupacion_actividad As Integer
    Dim Nombre_tipo_actividad As String
End Structure
Public Class Class_actividades_generales_workflow
    Function Solicita_estructura_tipo_actividad_workflow(ByVal id_tipo_actividad As Integer,
                                                         ByRef actividades_generales_workflow_ As actividades_generales_workflow) As String
        Try
            Dim Result As String = ""
            Dim sql_consulta As String = "Select Id_Actividad_General,Tipo_Actividad,Descripcion_Actividad,Agrupacion_actividad,Nombre_tipo_actividad from actividades_generales_workflow where  Id_Actividad_General=" & id_tipo_actividad
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("actividades_generales_workflow")
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_estructura_tipo_actividad_workflow = "Error Función Solicita_estructura_tipo_actividad_workflow dice  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_estructura_tipo_actividad_workflow = "Imposible encontrar el tipo de actividad del tipo (" & id_tipo_actividad & ")"
                Exit Function
            Else
                actividades_generales_workflow_.Id_Actividad_General = Datset.Tables(0).Rows(0).Item(0)
                actividades_generales_workflow_.Tipo_Actividad = Datset.Tables(0).Rows(0).Item(1)
                actividades_generales_workflow_.Descripcion_Actividad = Datset.Tables(0).Rows(0).Item(2)
                actividades_generales_workflow_.Agrupacion_actividad = Datset.Tables(0).Rows(0).Item(3)
                actividades_generales_workflow_.Nombre_tipo_actividad = Datset.Tables(0).Rows(0).Item(4)
                Solicita_estructura_tipo_actividad_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estructura_tipo_actividad_workflow = "Inconsistencia general funcion Solicita_estructura_tipo_actividad_workflow " & ex.Message
        End Try
    End Function
    Function Solicita_tipos_actividades_grupo_drowlist(ByVal id_tipo_actividad As Integer,
                                                       ByRef LisRef As DropDownList) As String
        Try
            LisRef.Items.Clear()
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql
            Dim Parametro_Consulta As String = "Select id_actividad_general,Nombre_tipo_actividad from actividades_generales_workflow where " &
            " Agrupacion_actividad=" & id_tipo_actividad
            Dim Dat_reader As New DataSet
            Dim result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If result <> "YES" Then
                Solicita_tipos_actividades_grupo_drowlist = " Error función Solicita_tipos_actividades_grupo_drowlist   " & result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To Dat_reader.Tables(0).Rows.Count - 1
                    Dim ilist_ As New ListItem
                    ilist_.Value = Dat_reader.Tables(0).Rows(i).Item(0)
                    ilist_.Text = Dat_reader.Tables(0).Rows(i).Item(1)
                    LisRef.Items.Add(ilist_)
                Next
                Solicita_tipos_actividades_grupo_drowlist = "YES"
                Exit Function
            Else
                Solicita_tipos_actividades_grupo_drowlist = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_tipos_actividades_grupo_drowlist = "Inconsistencia general funcion Solicita_tipos_actividades_grupo_drowlis " & ex.Message
        End Try
    End Function
End Class
