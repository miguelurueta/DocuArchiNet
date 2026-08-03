Public Class Class_relacion_script_plantilla
    Function retorna_campo_compracion_plantilla(ByVal id_script As Integer,
                                                ByRef campo_compracion As String,
                                                ByRef nombre_plantilla_validacion As String,
                                                ByRef id_plantilla_validacion As Integer) As String
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "SELECT Campo_busqueda_plantilla,pv.Id_Plantilla_Validacion,pv.Nombre_Plantilla " &
            " from relacion_script_plantilla  " &
            " inner join plantilla_validacion as pv on (pv.Id_Plantilla_Validacion=plantilla_validacion_Id_Plantilla_Validacion) " &
            " where script_actividades_Id_Script=" & id_script
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                retorna_campo_compracion_plantilla = " la función retorna_campo_compracion_plantilla dice   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                retorna_campo_compracion_plantilla = "Función retorna_campo_compracion_plantilla imposible encontrar el campo compración del escrip"
                Exit Function
            Else
                campo_compracion = Datset.Tables(0).Rows(0).Item(0)
                id_plantilla_validacion = Datset.Tables(0).Rows(0).Item(1)
                nombre_plantilla_validacion = Datset.Tables(0).Rows(0).Item(2)
                retorna_campo_compracion_plantilla = "YES"
            End If
        Catch ex As Exception
            retorna_campo_compracion_plantilla = "Inconsistencia función retorna_campo_compracion_plantilla " & ex.Message
        End Try
    End Function
    Function Solicta_campos_relacion_remitente_usuario_gestion(ByVal id_script As Integer,
                                                               ByRef Matri_Datos() As CAMPOS_PLANTILLA_VALIDACION_RAD_INTERNO) As String
        '--------------------------------------------------------------
        'Funcion : Lista los campos y el detalle plantilla validacion
        'con los parametors id de la plantillas y retorna una
        'matriz con la estructura 
        'Fecha : 2017-12-01
        'Ing : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select  cpv.Nombre_Campo,cpv.Tipo_Campo,cpv.Unico_campo,cpv.Aloja_null_campo,cpv.Visible_Campo,cpv.Obligatorio_Campo" &
             " ,cpv.Orden_Campos,cpv.Campo_Primari_Key,cpv.CAMPO_RELACION_RAD_INTERNA from relacion_script_plantilla as rsp inner join campos_plantilla_validacion as cpv on " &
             " ( cpv.Plantilla_Validacion_Id_Plantilla_Validacion=rsp.Plantilla_Validacion_Id_Plantilla_Validacion)" &
            " where script_actividades_id_script = " & id_script & " order by cpv.Orden_Campos"
            Dim Dat_reader As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Solicta_campos_relacion_remitente_usuario_gestion = "Función Solicta_campos_relacion_remitente_usuario_gestion dice  " & Result
                Exit Function
            End If
            Dim Iconta2 As Integer = 0
            If Dat_reader.Tables(0).Rows.Count = 0 Then
                Solicta_campos_relacion_remitente_usuario_gestion = "No se encontraron campos validación relacionados en la plantilla de validacion con el usuario de gestión"
                Exit Function
            Else
                For Iconta As Integer = 0 To Dat_reader.Tables(0).Rows.Count - 1
                    ReDim Preserve Matri_Datos(Iconta)
                    If Dat_reader.Tables(0).Rows(Iconta).IsNull(0) = False Then
                        Matri_Datos(Iconta).Nombre_Campo = Dat_reader.Tables(0).Rows(Iconta).Item(0).ToString
                    Else
                        Matri_Datos(Iconta).Nombre_Campo = ""
                    End If
                    If Dat_reader.Tables(0).Rows(Iconta).IsNull(1) = False Then
                        Matri_Datos(Iconta).Tipo_Campo = Dat_reader.Tables(0).Rows(Iconta).Item(1).ToString
                    Else
                        Matri_Datos(Iconta).Tipo_Campo = ""
                    End If

                    If Dat_reader.Tables(0).Rows(Iconta).IsNull(2) = False Then
                        Matri_Datos(Iconta).Unico_campo = Dat_reader.Tables(0).Rows(Iconta).Item(2).ToString
                    Else
                        Matri_Datos(Iconta).Unico_campo = 0
                    End If
                    If Dat_reader.Tables(0).Rows(Iconta).IsNull(3) = False Then
                        Matri_Datos(Iconta).Aloja_null_campo = Dat_reader.Tables(0).Rows(Iconta).Item(3).ToString
                    Else
                        Matri_Datos(Iconta).Aloja_null_campo = 0
                    End If
                    If Dat_reader.Tables(0).Rows(Iconta).IsNull(4) = False Then
                        Matri_Datos(Iconta).Visible_Campo = Dat_reader.Tables(0).Rows(Iconta).Item(4).ToString
                    Else
                        Matri_Datos(Iconta).Visible_Campo = 0
                    End If
                    If Dat_reader.Tables(0).Rows(Iconta).IsNull(5) = False Then
                        Matri_Datos(Iconta).Obligatorio_Campo = Dat_reader.Tables(0).Rows(Iconta).Item(5).ToString
                    Else
                        Matri_Datos(Iconta).Obligatorio_Campo = 0
                    End If
                    If Dat_reader.Tables(0).Rows(Iconta).IsNull(6) = False Then
                        Matri_Datos(Iconta).Orden_Campos = Dat_reader.Tables(0).Rows(Iconta).Item(6).ToString
                    Else
                        Matri_Datos(Iconta).Orden_Campos = 0
                    End If
                    If Dat_reader.Tables(0).Rows(Iconta).IsNull(7) = False Then
                        Matri_Datos(Iconta).IDENTI_CAMPO = Dat_reader.Tables(0).Rows(Iconta).Item(7).ToString
                    Else
                        Matri_Datos(Iconta).IDENTI_CAMPO = 0
                    End If

                    If Dat_reader.Tables(0).Rows(Iconta).IsNull(8) = False Then
                        Matri_Datos(Iconta).Aleas_Campo_rad_interno = Dat_reader.Tables(0).Rows(Iconta).Item(8).ToString
                    Else
                        Matri_Datos(Iconta).Aleas_Campo_rad_interno = ""
                    End If
                    Matri_Datos(Iconta).TEXTO_CAMPO = ""
                    Matri_Datos(Iconta).TEXTO_CAMPO_MODIFICADO = ""
                    Iconta2 = Iconta2 + 1
                Next
                Solicta_campos_relacion_remitente_usuario_gestion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicta_campos_relacion_remitente_usuario_gestion = "Incosistencia función Solicta_campos_relacion_remitente_usuario_gestion " & ex.Message
        End Try
    End Function
End Class
