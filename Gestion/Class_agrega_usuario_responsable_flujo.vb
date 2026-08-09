Public Class Class_agrega_usuario_responsable_flujo
    Function Agrega_usuario_responsable_flujo(ByVal id_flujo_trabajo As Integer, _
                                              ByRef drop_list As DropDownList, _
                                              ByRef up_dat As UpdatePanel, _
                                              ByRef drop_user As DropDownList) As String
        Try
            drop_user.Items.Clear()
            Dim Result As String = ""
            Dim id_usuario_gestion As Integer = 0
            Dim refclas_ra_usu As New Class_ra_usuario_gestion_responsable_flujo
            Result = refclas_ra_usu.Solicita_usuario_responsable_flujo(id_flujo_trabajo, _
                                                                      id_usuario_gestion)
            If Result <> "YES" Then
                Agrega_usuario_responsable_flujo = Result
                Exit Function
            End If
            If id_usuario_gestion <> 0 Then
                Agrega_usuario_responsable_flujo = "El sistema detecta un usuario como responsable del flujo imposible registrar"
                Exit Function
            End If
            Dim id_empresa_gestion As Integer = 0
            Dim Ref_class_empresa As New Class_empresa_gestion_documental
            Result = Ref_class_empresa.Solicita_id_empresa_gestion(id_empresa_gestion)
            If Result <> "YES" Then
                Agrega_usuario_responsable_flujo = Result
                Exit Function
            End If
            Dim ref_organigramas As New ClassGaOrganigrama
            Dim id_organigrama As Integer = 0
            Result = ref_organigramas.Solicita_id_organigramas_default(id_empresa_gestion, _
                                                                     id_organigrama)
            If Result <> "YES" Then
                Agrega_usuario_responsable_flujo = Result
                Exit Function
            End If
            Dim ref_class_areas_depart As New Class_areas_depart_radicacion
            Result = ref_class_areas_depart.Solicita_areas_departamento_organigrama(id_organigrama, _
                                                                                  drop_list, _
                                                                                  up_dat)
            Agrega_usuario_responsable_flujo = "YES"
            Exit Function
        Catch ex As Exception
            Agrega_usuario_responsable_flujo = "Inconsistencia general función Agrega_usuario_responsable_flujo " & ex.Message
        Finally
            up_dat.Update()
        End Try
    End Function
End Class
