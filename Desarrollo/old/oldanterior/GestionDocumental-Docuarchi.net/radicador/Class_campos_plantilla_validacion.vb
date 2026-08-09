Public Structure CAMPOS_PLANTILLA_VALIDACION_PQR
    Dim ID_CAMPO As Integer
    Dim Nombre_Campo As String
    Dim Tipo_Campo As String
    Dim Unico_campo As Integer
    Dim Aloja_null_campo As Integer
    Dim Visible_Campo As Integer
    Dim Obligatorio_Campo As Integer
    Dim Campo_Iidenti_pqr As Integer
    Dim Campo_anualidad_pqr As Integer
    Dim Campo_nombre_pqr As Integer
    Dim Aleas_Campo_pqr As String
    Dim Campo_correo_electrnico_pqr As Integer
    Dim Orden_Campos As Integer
    Dim IDENTI_CAMPO As Integer
    Dim TEXTO_CAMPO As String
    Dim TEXTO_CAMPO_MODIFICADO As String
    Dim valida_capital_text As Integer
    Dim Campo_clasifcacion_identificacion_pqr As Integer
    Dim Campo_clasificacion_identificacion As Integer
    Dim Campo_clasificacion_poblacion_pqr As Integer
    Dim Campo_clasificacion_poblacion As Integer
    Dim Campo_clasificacion_sexo_pqr As Integer
    Dim Campo_clasificacion_sexo As Integer
    Dim tipo_iteractua_campo As Integer
    Dim tipo_agrupacion_campo As Integer
End Structure
Public Class Class_campos_plantilla_validacion
    Function Asigna_datos_estructutura_plantilla_validacion_usuario_gestion(ByVal id_usuario_gestion As String,
                                                                            ByVal nombre_usuario_gestion As String,
                                                                            ByVal correo_electronico As String,
                                                                            ByVal telefono As String,
                                                                            ByVal identificacion As String,
                                                                            ByVal direccion As String,
                                                                            ByVal id_interno_radicado As Integer,
                                                                            ByRef matri_campos_relacion_getion_remitente() As CAMPOS_PLANTILLA_VALIDACION_RAD_INTERNO) As String
        '-----------------------------------------------------------
        'Función : Asigna datos del usuario de gestion en la 
        'matriz de campos de la plantilla de validación
        'Fecha : 2017-12-01
        'Ing : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------
        Try
            For i As Integer = 0 To matri_campos_relacion_getion_remitente.Length - 1
                If UCase(matri_campos_relacion_getion_remitente(i).Aleas_Campo_rad_interno) = UCase("Nombre_Remitente") Then
                    matri_campos_relacion_getion_remitente(i).TEXTO_CAMPO = nombre_usuario_gestion
                End If
                If UCase(matri_campos_relacion_getion_remitente(i).Aleas_Campo_rad_interno) = UCase("Correo_Electronico") Then
                    matri_campos_relacion_getion_remitente(i).TEXTO_CAMPO = correo_electronico
                End If
                If UCase(matri_campos_relacion_getion_remitente(i).Aleas_Campo_rad_interno) = UCase("TELEFONO_USUARIO") Then
                    matri_campos_relacion_getion_remitente(i).TEXTO_CAMPO = telefono
                End If
                If UCase(matri_campos_relacion_getion_remitente(i).Aleas_Campo_rad_interno) = UCase("IDENTIFICACION") Then
                    matri_campos_relacion_getion_remitente(i).TEXTO_CAMPO = identificacion
                End If
                If UCase(matri_campos_relacion_getion_remitente(i).Aleas_Campo_rad_interno) = UCase("DIRECCION") Then
                    matri_campos_relacion_getion_remitente(i).TEXTO_CAMPO = direccion
                End If
                If UCase(matri_campos_relacion_getion_remitente(i).Nombre_Campo) = UCase("id_interno_radicado") Then
                    matri_campos_relacion_getion_remitente(i).TEXTO_CAMPO = id_interno_radicado
                End If
            Next
            Asigna_datos_estructutura_plantilla_validacion_usuario_gestion = "YES"
        Catch ex As Exception
            Asigna_datos_estructutura_plantilla_validacion_usuario_gestion = "Inconsistencia general función Asigna_datos_estructutura_plantilla_validacion_usuario_gestion " & ex.Message
        End Try
    End Function
    Function Solicita_nombre_campo_remitente_relacion_usuario_gestion_usuario_remitente(ByVal nombre_campo_usuario_gestion As String,
                                                                                        ByVal Matri_relacion_gestion_remitente() As CAMPOS_PLANTILLA_VALIDACION_RAD_INTERNO,
                                                                                        ByRef nombre_campo_remitente As String) As String
        '------------------------------------------------------
        'Función : Solicita el nombre del campo del usuario
        'remitente de la plantilla de validación con el 
        'parametro de la estructura de los campos relacionados
        'Ing : Miguel Angel Urueta Miranda
        'Fecha : 2017-12-01
        '------------------------------------------------------
        Try
            nombre_campo_remitente = ""
            If Matri_relacion_gestion_remitente Is Nothing Then
                Solicita_nombre_campo_remitente_relacion_usuario_gestion_usuario_remitente = "YES"
                Exit Function
            End If
            For i As Integer = 0 To Matri_relacion_gestion_remitente.Length - 1
                If UCase(Matri_relacion_gestion_remitente(i).Aleas_Campo_rad_interno) = UCase(nombre_campo_usuario_gestion) Then
                    nombre_campo_remitente = Matri_relacion_gestion_remitente(i).Nombre_Campo
                    Exit For
                End If
            Next
            Solicita_nombre_campo_remitente_relacion_usuario_gestion_usuario_remitente = "YES"
            Exit Function
        Catch ex As Exception
            Solicita_nombre_campo_remitente_relacion_usuario_gestion_usuario_remitente = "Inconsistencia general función Solicita_nombre_campo_remitente_relacion_usuario_gestion_usuario_remitente " & ex.Message
        End Try
    End Function
    Function Solicita_Campos_Plantilla_Validacion_pqr(ByVal id_script As Integer,
                                                   ByRef Matri_Datos() As CAMPOS_PLANTILLA_VALIDACION_PQR) As String
        '--------------------------------------------------------------
        'Funcion : Lista los campos y el detalle plantilla validacion
        'con los parametors id de la plantillas y retorna una
        'matriz con la estructura 
        'Fecha : 2014-07-24
        'Ing : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select  cpv.Nombre_Campo,cpv.Tipo_Campo,cpv.Unico_campo_pqr,cpv.Aloja_null_campo_pqr,cpv.Visible_Campo_pqr" &
             ",cpv.Obligatorio_Campo_pqr,cpv.Orden_Campos,cpv.Campo_Primari_Key,Campo_Iidenti_pqr,Campo_anualidad_pqr,Campo_nombre_pqr," &
             "Aleas_Campo_pqr,Campo_correo_electrnico_pqr,cpv.valida_capital_text,cpv.Campo_clasifcacion_identificacion_pqr, " &
             "cpv.Campo_clasificacion_identificacion,cpv.Campo_clasificacion_poblacion_pqr,cpv.Campo_clasificacion_poblacion," &
             "cpv.Campo_clasificacion_sexo_pqr, cpv.Campo_clasificacion_sexo,cpv.tipo_iteractua_campo,cpv.tipo_agrupacion_campo" &
             " from relacion_script_plantilla as rsp" &
             " inner join campos_plantilla_validacion as cpv on " &
             " ( cpv.Plantilla_Validacion_Id_Plantilla_Validacion=rsp.Plantilla_Validacion_Id_Plantilla_Validacion)" &
            " where script_actividades_id_script = " & id_script & " order by cpv.Orden_Campos"
            Dim Dat_reader As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Solicita_Campos_Plantilla_Validacion_pqr = " Error listando campos validación pqrs  " & Result
                Exit Function
            End If
            Dim Iconta2 As Integer = 0
            If Dat_reader.Tables(0).Rows.Count = 0 Then
                Solicita_Campos_Plantilla_Validacion_pqr = "No se encontraron campos validación"
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
                        Matri_Datos(Iconta).Campo_Iidenti_pqr = Dat_reader.Tables(0).Rows(Iconta).Item(8).ToString
                    Else
                        Matri_Datos(Iconta).Campo_Iidenti_pqr = 0
                    End If
                    If Dat_reader.Tables(0).Rows(Iconta).IsNull(9) = False Then
                        Matri_Datos(Iconta).Campo_anualidad_pqr = Dat_reader.Tables(0).Rows(Iconta).Item(9).ToString
                    Else
                        Matri_Datos(Iconta).Campo_anualidad_pqr = 0
                    End If
                    If Dat_reader.Tables(0).Rows(Iconta).IsNull(10) = False Then
                        Matri_Datos(Iconta).Campo_nombre_pqr = Dat_reader.Tables(0).Rows(Iconta).Item(10).ToString
                    Else
                        Matri_Datos(Iconta).Campo_nombre_pqr = 0
                    End If
                    If Dat_reader.Tables(0).Rows(Iconta).IsNull(11) = False Then
                        Matri_Datos(Iconta).Aleas_Campo_pqr = Dat_reader.Tables(0).Rows(Iconta).Item(11).ToString
                    Else
                        Matri_Datos(Iconta).Aleas_Campo_pqr = ""
                    End If
                    If Dat_reader.Tables(0).Rows(Iconta).IsNull(12) = False Then
                        Matri_Datos(Iconta).Campo_correo_electrnico_pqr = Dat_reader.Tables(0).Rows(Iconta).Item(12).ToString
                    Else
                        Matri_Datos(Iconta).Campo_correo_electrnico_pqr = 0
                    End If
                    If Dat_reader.Tables(0).Rows(Iconta).IsNull(13) = False Then
                        Matri_Datos(Iconta).valida_capital_text = Dat_reader.Tables(0).Rows(Iconta).Item(13).ToString
                    Else
                        Matri_Datos(Iconta).valida_capital_text = 0
                    End If
                    Matri_Datos(Iconta).Campo_clasifcacion_identificacion_pqr = Dat_reader.Tables(0).Rows(Iconta).Item(14)
                    Matri_Datos(Iconta).Campo_clasificacion_identificacion = Dat_reader.Tables(0).Rows(Iconta).Item(15)
                    Matri_Datos(Iconta).Campo_clasificacion_poblacion_pqr = Dat_reader.Tables(0).Rows(Iconta).Item(16)
                    Matri_Datos(Iconta).Campo_clasificacion_poblacion = Dat_reader.Tables(0).Rows(Iconta).Item(17)
                    Matri_Datos(Iconta).Campo_clasificacion_sexo_pqr = Dat_reader.Tables(0).Rows(Iconta).Item(18)
                    Matri_Datos(Iconta).Campo_clasificacion_sexo = Dat_reader.Tables(0).Rows(Iconta).Item(19)
                    Matri_Datos(Iconta).tipo_iteractua_campo = Dat_reader.Tables(0).Rows(Iconta).Item(20)
                    Matri_Datos(Iconta).tipo_agrupacion_campo = Dat_reader.Tables(0).Rows(Iconta).Item(21)
                    Iconta2 = Iconta2 + 1
                Next
                Solicita_Campos_Plantilla_Validacion_pqr = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_Campos_Plantilla_Validacion_pqr = "Incosistencia función Solicita_Campos_Plantilla_Validacion_pqr " & ex.Message
        End Try
    End Function
    Function Verifica_existencia_campo_identificacion_usuario_gestion_plantilla(ByVal id_plantilla_validacion As Integer,
                                                                                ByRef estado_existencia_plantilla As String) As String
        '----------------------------------------------------------
        'Función : Verifica la existencia del campo identifcacion
        'del usuario internos como remitente de correspondencia
        'Fecha : 2017-11-30
        'ingeniero :Miguel Angel Urueta Miranda
        '----------------------------------------------------------
        Try
            Dim Parametro_Consulta = "select Id_campo_Plantilla " &
         " from campos_plantilla_validacion where Plantilla_Validacion_Id_Plantilla_Validacion=" & id_plantilla_validacion &
         " and Nombre_Campo='id_interno_radicado'"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("campos_plantilla_validacion")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verifica_existencia_campo_identificacion_usuario_gestion_plantilla = "Funcion  Verifica_existencia_campo_identificacion_usuario_gestion_plantilla dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado_existencia_plantilla = "NO"
                Verifica_existencia_campo_identificacion_usuario_gestion_plantilla = "YES"
                Exit Function
            Else
                estado_existencia_plantilla = "YES"
                Verifica_existencia_campo_identificacion_usuario_gestion_plantilla = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_existencia_campo_identificacion_usuario_gestion_plantilla = "Inconsistencia general función Verifica_existencia_campo_identificacion_usuario_gestion_plantilla " & ex.Message
        End Try
    End Function
    Function Lista_Campos_Plantilla_Validacion(ByVal id_script As Integer,
                                               ByRef Matri_Datos() As CAMPOS_PLANTILLA_VALIDACION) As String
        '--------------------------------------------------------------
        'Funcion : Lista los campos y el detalle plantilla validacion
        'con los parametors id de la plantillas y retorna una
        'matriz con la estructura 
        'Fecha : 2014-07-24
        'Ing : Miguel Angel Urueta Miranda  Campo_correo_electrnico_pqr
        '--------------------------------------------------------------
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select  cpv.Nombre_Campo,cpv.Tipo_Campo,cpv.Unico_campo,cpv.Aloja_null_campo,cpv.Visible_Campo " &
             " ,cpv.Obligatorio_Campo,cpv.Orden_Campos,cpv.Campo_Primari_Key,cpv.valida_capital_text,cpv.Campo_nombre_pqr,cpv.Campo_correo_electrnico_pqr,cpv.Aleas_Campo_pqr from relacion_script_plantilla as rsp inner join campos_plantilla_validacion as cpv on " &
             " ( cpv.Plantilla_Validacion_Id_Plantilla_Validacion=rsp.Plantilla_Validacion_Id_Plantilla_Validacion)" &
            " where script_actividades_id_script = " & id_script & " order by cpv.Orden_Campos"
            Dim Dat_reader As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Lista_Campos_Plantilla_Validacion = " Error listando campos validacion   " & Result
                Exit Function
            End If
            Dim Iconta2 As Integer = 0
            If Dat_reader.Tables(0).Rows.Count = 0 Then
                Lista_Campos_Plantilla_Validacion = "No se encontraron campos validación"
                Exit Function
            Else
                Dim valor_nombre As String = ""
                For Iconta As Integer = 0 To Dat_reader.Tables(0).Rows.Count - 1
                    ReDim Preserve Matri_Datos(Iconta)
                    If Dat_reader.Tables(0).Rows(Iconta).IsNull(0) = False Then
                        Matri_Datos(Iconta).Nombre_Campo = Dat_reader.Tables(0).Rows(Iconta).Item(0).ToString
                        valor_nombre = Dat_reader.Tables(0).Rows(Iconta).Item(0).ToString
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
                        Matri_Datos(Iconta).valida_capital_text = Dat_reader.Tables(0).Rows(Iconta).Item(8).ToString
                    Else
                        Matri_Datos(Iconta).valida_capital_text = 0
                    End If
                    If Dat_reader.Tables(0).Rows(Iconta).IsNull(9) = False Then
                        Matri_Datos(Iconta).Campo_nombre_pqr = Dat_reader.Tables(0).Rows(Iconta).Item(9).ToString
                    Else
                        Matri_Datos(Iconta).Campo_nombre_pqr = 0
                    End If
                    If Dat_reader.Tables(0).Rows(Iconta).IsNull(10) = False Then
                        Matri_Datos(Iconta).Campo_correo_electrnico_pqr = Dat_reader.Tables(0).Rows(Iconta).Item(10).ToString
                    Else
                        Matri_Datos(Iconta).Campo_correo_electrnico_pqr = 0
                    End If
                    If Dat_reader.Tables(0).Rows(Iconta).IsNull(11) = False Then
                        Matri_Datos(Iconta).Aleas_Campo_pqr = Dat_reader.Tables(0).Rows(Iconta).Item(11).ToString
                    Else
                        Matri_Datos(Iconta).Aleas_Campo_pqr = valor_nombre
                    End If
                    'Aleas_Campo_pqr
                    Iconta2 = Iconta2 + 1
                Next
                Lista_Campos_Plantilla_Validacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_Campos_Plantilla_Validacion = "Incosist funcion Lista_Campos_Plantilla_Validacion " & ex.Message
        End Try
    End Function


    Function Retorna_Campo_Primary_key_plantilla_validacion(ByVal id_plantilla As Integer,
                                                           ByRef nombre_campo As String) As String
        '**************************************************************************************
        'Funcion : Retorna nombre campo primary key plantilla de validacion con el parametro
        'de id plantilla
        'Fecha : 2014-08-04
        'Ingeniero : Miguel Angel Urueta Miranda
        '**************************************************************************************
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "SELECT Nombre_Campo FROM campos_plantilla_validacion " &
            " where plantilla_validacion_id_plantilla_validacion='" & id_plantilla & "' and Campo_Primari_key=1"
            Dim Dat_reader As New DataSet
            Dim Result As String = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Retorna_Campo_Primary_key_plantilla_validacion = " Error Listando nombre campo Primary_key_plantilla_validacion" & Result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count = 0 Then
                Retorna_Campo_Primary_key_plantilla_validacion = "Imposible encontrar campo Primary_key_plantilla_validacion"
                Exit Function
            Else
                nombre_campo = Dat_reader.Tables(0).Rows(0).Item(0)
            End If
            Retorna_Campo_Primary_key_plantilla_validacion = "YES"
        Catch ex As Exception
            Retorna_Campo_Primary_key_plantilla_validacion = "Inconsistencia funcion Retorna_Campo_Primary_key_plantilla_validacion " & ex.Message
        End Try
    End Function
End Class
