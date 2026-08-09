
Public Structure PLANTILLA_VALIDACION_CAMPOS_ESTATICOS
    Dim id_Radicado As Long
    Dim Destinatario_Externo_id_Dest_Ext As Integer
    Dim Remit_Dest_Interno_id_Remit_Dest_Int As Integer
    Dim Usuario_Radicador_id_usuario As Integer
    Dim System_Plantilla_Radicado_id_Plantilla As Integer
    Dim Consecutivo_Rad As String
    Dim Consecutivo_CodBarra As String
    Dim Fecha_Radicado As String
    Dim Fecha_Documento As String
    Dim Descripcion_Documento As String
    Dim Numero_Folios As Integer
    Dim Destinatario_Cor As String
    Dim Remitente_Cor As String
    Dim Anexos_Cor As String
    Dim Asunto As String
    Dim Expediente As String
    Dim id_Expediente As Integer
    Dim CITARADICADO As String
    Dim FECHALIMITERESPUESTA As String
    Dim Id_area_remit_dest_interno As Integer
    Dim Area_remit_dest_interno As String
    Dim CARGO_DESTINATARIO As String
    Dim CARGO_REMITENTE As String
    Dim IDENTIFICACION_REMITENTE As String
    Dim IDENTIFICACION_DESTINATARIO As String
    Dim tipo_doc_entrante_id_tipo_doc_entrante As Integer
    Dim id_tipo_flujo_workflow As Integer
    Dim Flag_Flow As Integer
End Structure
Public Class Class_plantillas_radicacion
    Function Solicita_id_nombre_tipo_tramite_plantilla_radicado(ByVal nombre_plantilla As String, _
                                                                ByVal radicado As String, _
                                                                ByRef id_tipo_tramite As Integer, _
                                                                ByRef descripcion_tramite As String) As String
        Try
            Dim Parametro_Consulta As String = "select Descripcion_Documento,tipo_doc_entrante_id_tipo_doc_entrante from  " & nombre_plantilla & _
                               " where Consecutivo_Rad=" & radicado
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("ra_respuesta_radicado")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_nombre_tipo_tramite_plantilla_radicado = "Función Retorna_tipo_tramite_plantilla_radicado dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_id_nombre_tipo_tramite_plantilla_radicado = "Imposible encontrar datos del consecutivo de radicacion " & radicado & " en la plantilla " & nombre_plantilla
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) Then
                    descripcion_tramite = ""
                Else
                    descripcion_tramite = Datset.Tables(0).Rows(0).Item(0)
                End If
                If Datset.Tables(0).Rows(0).IsNull(1) Then
                    id_tipo_tramite = -1
                Else
                    id_tipo_tramite = Datset.Tables(0).Rows(0).Item(1)
                End If
                Solicita_id_nombre_tipo_tramite_plantilla_radicado = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Solicita_id_nombre_tipo_tramite_plantilla_radicado = "Inconsistencia función Retorna_tipo_tramite_plantilla_radicado " & ex.Message
        End Try
    End Function
    Function Verifica_campos_unicos(ByVal stru() As estru_campos_unicos,
                                    ByVal nombre_plantilla_radicacion As String) As String
        Try
            If stru Is Nothing Then
                Verifica_campos_unicos = "YES"
                Exit Function
            End If
            Dim campo_condicion As String = ""
            Dim campos_obligatorios_condicion As String = ""
            For i As Integer = 0 To stru.Length - 1
                If stru(i).campo_valida_unico = 1 Then
                    If campos_obligatorios_condicion = "" Then
                        If stru(i).valor_Campo <> "" Then
                            campos_obligatorios_condicion = campos_obligatorios_condicion & " where " & stru(i).Campo_Plantilla & "='" & stru(i).valor_Campo & "'"
                            campo_condicion = campo_condicion & stru(i).Campo_Plantilla
                        End If
                    Else
                        If stru(i).valor_Campo <> "" Then
                            campos_obligatorios_condicion = campos_obligatorios_condicion & " and " & stru(i).Campo_Plantilla & "='" & stru(i).valor_Campo & "'"
                            campo_condicion = campo_condicion & "," & stru(i).Campo_Plantilla
                        End If
                    End If
                End If
            Next
            If campos_obligatorios_condicion = "" Then
                Verifica_campos_unicos = "YES"
                Exit Function
            End If
            Dim conext As New conect.Dbase_Conction_Mysql_RA
            Dim datset As New DataSet
            Dim Parametro_Consulta As String = "Select * from " & nombre_plantilla_radicacion & " " & campos_obligatorios_condicion
            Dim Result As String = conext.SELECTION_SELECT_FIELD(Parametro_Consulta, datset)
            If Result <> "YES" Then
                Verifica_campos_unicos = "Inconsistencia listando campos obligatorios y unicos " & Result
                Exit Function
            End If
            If datset.Tables(0).Rows.Count = 0 Then
                Verifica_campos_unicos = "YES"
                Exit Function
            Else
                Verifica_campos_unicos = "Los campos " & campo_condicion & " tienen valores registrados en la plantilla " & nombre_plantilla_radicacion & " no se permite duplicidad"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_campos_unicos = "Inconsistencia función Verifica_campos_unicos " & ex.Message
        End Try
    End Function
    Function retorna_tipo_documental_radicado(ByVal radicado As String, _
                                              ByVal nombre_plantilla As String, _
                                              ByRef tipo_tramite_documento As String) As String
        '**************************************************************
        'Función : retorna el tipo documnto o tramite seleccionado por
        'el numero de radicado y la plantilla radicado
        'Feecha 2015-05-19
        'Ingemiero : Miguel Angel Urueta Miranda
        '**************************************************************
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select Descripcion_Documento from " & nombre_plantilla & " where Consecutivo_Rad='" & radicado & "'"
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                retorna_tipo_documental_radicado = "Función retorna_tipo_documental_radicado dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                retorna_tipo_documental_radicado = "Función retorna_tipo_documental_radicado dice imposible encontrar tipo clase documento o tramite"
                Exit Function
            Else
                tipo_tramite_documento = Datset.Tables(0).Rows(0).Item(0)
                retorna_tipo_documental_radicado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            retorna_tipo_documental_radicado = "Función retorna_tipo_documental_radicado dice " & ex.Message
        End Try
    End Function
    Function Retorna_id_flujo_trabajo_radicado(ByVal nombre_plantilla As String, _
                                               ByVal radicado As String, _
                                               ByRef id_flujo_trabajo As Integer) As String
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select id_tipo_flujo_workflow from " & nombre_plantilla & " where Consecutivo_Rad='" & radicado & "'"
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_id_flujo_trabajo_radicado = "Función retorna_tipo_documental_radicado dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_id_flujo_trabajo_radicado = "Función Retorna_id_flujo_trabajo_radicado dice imposible encontrar el tipo flujo del radicado (" & radicado & _
                    ") en la plantilla ( " & nombre_plantilla & " )"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    id_flujo_trabajo = 0
                Else
                    id_flujo_trabajo = Datset.Tables(0).Rows(0).Item(0)
                End If
                Retorna_id_flujo_trabajo_radicado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_id_flujo_trabajo_radicado = "Inconsistencia general funcion Retorna_id_flujo_trabajo_radicado " & ex.Message
        End Try
    End Function
    Function Solicita_remitente_destinatario_fecha_radicado(ByVal nombre_plantilla As String, _
                                                            ByVal radicado As String, _
                                                            ByRef codigo_destinatario As Integer, _
                                                            ByRef codigo_dest_externo As Integer, _
                                                            ByRef Fecha_Radicado As String) As String
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select Destinatario_Externo_id_Dest_Ext,Remit_Dest_Interno_id_Remit_Dest_Int,Fecha_Radicado from " & _
                nombre_plantilla & " where Consecutivo_Rad='" & radicado & "'"
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_remitente_destinatario_fecha_radicado = "Función Solicita_remitente_destinatario_fecha_radicado dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_remitente_destinatario_fecha_radicado = "Función Solicita_remitente_destinatario_fecha_radicado dice imposible encontrar detalle del radicado (" & radicado & _
                    ") en la plantilla ( " & nombre_plantilla & " )"
                Exit Function
            Else
                codigo_destinatario = Datset.Tables(0).Rows(0).Item(0)
                codigo_dest_externo = Datset.Tables(0).Rows(0).Item(1)
                Fecha_Radicado = Datset.Tables(0).Rows(0).Item(2)
                Solicita_remitente_destinatario_fecha_radicado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_remitente_destinatario_fecha_radicado = "Inconsistencia general funcion Solicita_remitente_destinatario_fecha_radicado " & ex.Message
        End Try
    End Function
    Function AsignaDatosCamposPlantillaRadicadoGabinete(ByRef stru_campos_plantilla_ruta() As csfc_structure_relacion_campos_plantilla_ruta,
                                                        ByVal Radicado As String,
                                                        ByVal NombrePlantillaRadicado As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Asgina datos de la plnatilla del radicado para actualizar en el gabinete
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'Radicado                   : Representa el consecutivo del radicado
        'NombrePlantillaRadicado    : Representa el nombre de la plantilla de radicado
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'stru_campos_plantilla_ruta  : Retorna la estructura con los datos
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-04-01
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim select_ As String = ""
            For i As Integer = 0 To stru_campos_plantilla_ruta.Length - 1
                If i = 0 Then
                    select_ = stru_campos_plantilla_ruta(i).nombre_campo_plantilla
                Else
                    select_ = select_ & "," & stru_campos_plantilla_ruta(i).nombre_campo_plantilla
                End If
            Next
            Dim Sql_consulta As String = "Select " & select_ & "  from " & NombrePlantillaRadicado & " where Consecutivo_Rad='" & Radicado & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet(NombrePlantillaRadicado)
            Dim result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If result <> "YES" Then
                AsignaDatosCamposPlantillaRadicadoGabinete = "Función  AsignaDatosCamposPlantillaRadicadoGabinete dice " & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                AsignaDatosCamposPlantillaRadicadoGabinete = "Imposible encontrar datos para el consecutivo radicado " & Radicado
                Exit Function
            Else
                For z As Integer = 0 To stru_campos_plantilla_ruta.Length - 1
                    If Datset.Tables(0).Rows(0).IsNull(z) = False Then
                        stru_campos_plantilla_ruta(z).dato_campo_plantilla = Left(Datset.Tables(0).Rows(0).Item(z),
                                                                                  stru_campos_plantilla_ruta(z).dimension_campo_ruta)
                    End If
                Next
                AsignaDatosCamposPlantillaRadicadoGabinete = "YES"
                Exit Function
            End If

        Catch ex As Exception
            AsignaDatosCamposPlantillaRadicadoGabinete = "Inconsistencia general función AsignaDatosCamposPlantillaRadicadoGabinete " & ex.Message
        End Try
    End Function
    Function retorna_datos_radicacion_estructura(ByVal tipo_radicacion As String,
                                                 ByVal consecutivo_radicacion As String,
                                                 ByVal nombre_plantilla As String,
                                                 ByRef sctru_radicado As PLANTILLA_VALIDACION_CAMPOS_ESTATICOS) As String
        '*************************************************************************************
        'Función : asigna los datos de la estructura de la plantilla de validación
        'con el parametro de consecutivo de radicación
        'Fecha : 2015-05-11
        'Ingeniero : Miguel Angel Urueta Miranda
        '*************************************************************************************
        Try
            Dim ref_class_gestionfechas As New ClassGestionFechas
            Dim campos_select As String = ""
            If tipo_radicacion = "RADICACION ENTRANTE" Then
                campos_select = ",IDENTIFICACION_REMITENTE,CARGO_DESTINATARIO"
            Else
                campos_select = ",IDENTIFICACION_DESTINATARIO,CARGO_REMITENTE"
            End If
            Dim Parametro_Consulta As String = "Select Destinatario_Externo_id_Dest_Ext,Remit_Dest_Interno_id_Remit_Dest_Int, " &
                 "Usuario_Radicador_id_usuario,System_Plantilla_Radicado_id_Plantilla,Consecutivo_Rad,Consecutivo_CodBarra, " &
                 "Fecha_Radicado,Fecha_Documento,Descripcion_Documento,Numero_Folios,Destinatario_Cor,Remitente_Cor, " &
                 "Anexos_Cor,Asunto,Expediente,id_Expediente,CITARADICADO,FECHALIMITERESPUESTA, " &
                 "Id_area_remit_dest_interno,Area_remit_dest_interno,tipo_doc_entrante_id_tipo_doc_entrante,id_tipo_flujo_workflow,Flag_Flow " & campos_select &
                 " from " & nombre_plantilla & " where Consecutivo_Rad='" & consecutivo_radicacion & "'"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Dat_reader As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                retorna_datos_radicacion_estructura = " Función retorna_datos_radicacion_estructura dice   " & Result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count = 0 Then
                retorna_datos_radicacion_estructura = "Función retorna_datos_radicacion_estructura Imposible encontrar radicado " & consecutivo_radicacion
                Exit Function
            Else
                If tipo_radicacion = "RADICACION ENTRANTE" Then
                    If Dat_reader.Tables(0).Rows(0).IsNull("IDENTIFICACION_REMITENTE") = True Then
                        sctru_radicado.IDENTIFICACION_REMITENTE = ""
                    Else
                        sctru_radicado.IDENTIFICACION_REMITENTE = Dat_reader.Tables(0).Rows(0).Item("IDENTIFICACION_REMITENTE")
                    End If
                    If Dat_reader.Tables(0).Rows(0).IsNull("CARGO_DESTINATARIO") = True Then
                        sctru_radicado.CARGO_DESTINATARIO = ""
                    Else
                        sctru_radicado.CARGO_DESTINATARIO = Dat_reader.Tables(0).Rows(0).Item("CARGO_DESTINATARIO")
                    End If
                Else
                    If Dat_reader.Tables(0).Rows(0).IsNull("IDENTIFICACION_DESTINATARIO") = True Then
                        sctru_radicado.IDENTIFICACION_DESTINATARIO = ""
                    Else
                        sctru_radicado.IDENTIFICACION_DESTINATARIO = Dat_reader.Tables(0).Rows(0).Item("IDENTIFICACION_DESTINATARIO")
                    End If
                    If Dat_reader.Tables(0).Rows(0).IsNull("CARGO_REMITENTE") = True Then
                        sctru_radicado.CARGO_REMITENTE = ""
                    Else
                        sctru_radicado.CARGO_REMITENTE = Dat_reader.Tables(0).Rows(0).Item("CARGO_REMITENTE")
                    End If
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull("Area_remit_dest_interno") = True Then
                    sctru_radicado.Area_remit_dest_interno = ""
                Else
                    sctru_radicado.Area_remit_dest_interno = Dat_reader.Tables(0).Rows(0).Item("Area_remit_dest_interno")
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull("Id_area_remit_dest_interno") = True Then
                    sctru_radicado.Id_area_remit_dest_interno = 0
                Else
                    sctru_radicado.Id_area_remit_dest_interno = Dat_reader.Tables(0).Rows(0).Item("Id_area_remit_dest_interno")
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull("FECHALIMITERESPUESTA") = True Then
                    sctru_radicado.FECHALIMITERESPUESTA = ""
                Else
                    sctru_radicado.FECHALIMITERESPUESTA = Dat_reader.Tables(0).Rows(0).Item("FECHALIMITERESPUESTA")
                    ref_class_gestionfechas.formato_fecha_estructura(sctru_radicado.FECHALIMITERESPUESTA)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull("CITARADICADO") = True Then
                    sctru_radicado.CITARADICADO = ""
                Else
                    sctru_radicado.CITARADICADO = Dat_reader.Tables(0).Rows(0).Item("CITARADICADO")
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull("id_Expediente") = True Then
                    sctru_radicado.id_Expediente = 0
                Else
                    sctru_radicado.id_Expediente = Dat_reader.Tables(0).Rows(0).Item("id_Expediente")
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull("Expediente") = True Then
                    sctru_radicado.Expediente = ""
                Else
                    sctru_radicado.Expediente = Dat_reader.Tables(0).Rows(0).Item("Expediente")
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull("Asunto") = True Then
                    sctru_radicado.Asunto = ""
                Else
                    sctru_radicado.Asunto = Dat_reader.Tables(0).Rows(0).Item("Asunto")
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull("Anexos_Cor") = True Then
                    sctru_radicado.Anexos_Cor = ""
                Else
                    sctru_radicado.Anexos_Cor = Dat_reader.Tables(0).Rows(0).Item("Anexos_Cor")
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull("Remitente_Cor") = True Then
                    sctru_radicado.Remitente_Cor = ""
                Else
                    sctru_radicado.Remitente_Cor = Dat_reader.Tables(0).Rows(0).Item("Remitente_Cor")
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull("Destinatario_Cor") = True Then
                    sctru_radicado.Destinatario_Cor = ""
                Else
                    sctru_radicado.Destinatario_Cor = Dat_reader.Tables(0).Rows(0).Item("Destinatario_Cor")
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull("Numero_Folios") = True Then
                    sctru_radicado.Numero_Folios = ""
                Else
                    sctru_radicado.Numero_Folios = Dat_reader.Tables(0).Rows(0).Item("Numero_Folios")
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull("Descripcion_Documento") = True Then
                    sctru_radicado.Descripcion_Documento = ""
                Else
                    sctru_radicado.Descripcion_Documento = Dat_reader.Tables(0).Rows(0).Item("Descripcion_Documento")
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull("Fecha_Documento") = True Then
                    sctru_radicado.Fecha_Documento = ""
                Else
                    sctru_radicado.Fecha_Documento = Dat_reader.Tables(0).Rows(0).Item("Fecha_Documento")
                    ref_class_gestionfechas.formato_fecha_estructura(sctru_radicado.Fecha_Documento)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull("Fecha_Radicado") = True Then
                    sctru_radicado.Fecha_Radicado = ""
                Else
                    sctru_radicado.Fecha_Radicado = Dat_reader.Tables(0).Rows(0).Item("Fecha_Radicado")
                    ref_class_gestionfechas.formato_fecha_estructura(sctru_radicado.Fecha_Radicado)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull("Consecutivo_CodBarra") = True Then
                    sctru_radicado.Consecutivo_CodBarra = 0
                Else
                    sctru_radicado.Consecutivo_CodBarra = Dat_reader.Tables(0).Rows(0).Item("Consecutivo_CodBarra")
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull("Consecutivo_Rad") = True Then
                    sctru_radicado.Consecutivo_Rad = 0
                Else
                    sctru_radicado.Consecutivo_Rad = Dat_reader.Tables(0).Rows(0).Item("Consecutivo_Rad")
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull("System_Plantilla_Radicado_id_Plantilla") = True Then
                    sctru_radicado.System_Plantilla_Radicado_id_Plantilla = 0
                Else
                    sctru_radicado.System_Plantilla_Radicado_id_Plantilla = Dat_reader.Tables(0).Rows(0).Item("System_Plantilla_Radicado_id_Plantilla")
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull("Usuario_Radicador_id_usuario") = True Then
                    sctru_radicado.Usuario_Radicador_id_usuario = 0
                Else
                    sctru_radicado.Usuario_Radicador_id_usuario = Dat_reader.Tables(0).Rows(0).Item("Usuario_Radicador_id_usuario")
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull("Remit_Dest_Interno_id_Remit_Dest_Int") = True Then
                    sctru_radicado.Remit_Dest_Interno_id_Remit_Dest_Int = 0
                Else
                    sctru_radicado.Remit_Dest_Interno_id_Remit_Dest_Int = Dat_reader.Tables(0).Rows(0).Item("Remit_Dest_Interno_id_Remit_Dest_Int")
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull("Destinatario_Externo_id_Dest_Ext") = True Then
                    sctru_radicado.Destinatario_Externo_id_Dest_Ext = 0
                Else
                    sctru_radicado.Destinatario_Externo_id_Dest_Ext = Dat_reader.Tables(0).Rows(0).Item("Destinatario_Externo_id_Dest_Ext")
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull("tipo_doc_entrante_id_tipo_doc_entrante") = True Then
                    sctru_radicado.tipo_doc_entrante_id_tipo_doc_entrante = 0
                Else
                    sctru_radicado.tipo_doc_entrante_id_tipo_doc_entrante = Dat_reader.Tables(0).Rows(0).Item("tipo_doc_entrante_id_tipo_doc_entrante")
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull("tipo_doc_entrante_id_tipo_doc_entrante") = True Then
                    sctru_radicado.tipo_doc_entrante_id_tipo_doc_entrante = 0
                Else
                    sctru_radicado.tipo_doc_entrante_id_tipo_doc_entrante = Dat_reader.Tables(0).Rows(0).Item("tipo_doc_entrante_id_tipo_doc_entrante")
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull("id_tipo_flujo_workflow") = True Then
                    sctru_radicado.id_tipo_flujo_workflow = 0
                Else
                    sctru_radicado.id_tipo_flujo_workflow = Dat_reader.Tables(0).Rows(0).Item("id_tipo_flujo_workflow")
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull("Flag_Flow") = True Then
                    sctru_radicado.Flag_Flow = 0
                Else
                    sctru_radicado.Flag_Flow = Dat_reader.Tables(0).Rows(0).Item("Flag_Flow")
                End If
                retorna_datos_radicacion_estructura = "YES"
                Exit Function
            End If
        Catch ex As Exception
            retorna_datos_radicacion_estructura = "Inconsistencia función retorna_datos_radicacion_estructura " & ex.Message
        End Try
    End Function
    Function Solicita_id_usuario_radicacion_plantilla_radicado(ByVal nombre_plantilla As String,
                                                               ByVal radicado As String,
                                                               ByRef id_usuario_radicador As Integer) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita el usuario radicador de un radicado en una plantilla
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'Nombre_plantilla      : Respresenta el nombre de plantilla del radicado
        'radicado              : Representa el consecutivo de radicado
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'id_usuario_radicador  : Retorna la idnetificación del usuario radicador
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-04-01
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "Select Usuario_Radicador_id_usuario " &
               " from " & nombre_plantilla & " where Consecutivo_Rad='" & radicado & "'"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Dat_reader As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Solicita_id_usuario_radicacion_plantilla_radicado = " Función Solicita_id_usuario_radicacion_plantilla_radicado dice   " & Result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count = 0 Then
                Solicita_id_usuario_radicacion_plantilla_radicado = "Imposible encontrar la identificación del radicador del radicado (" & radicado & ") plantilla (" & nombre_plantilla & ")"
                Exit Function
            Else
                id_usuario_radicador = Dat_reader.Tables(0).Rows(0).Item(0)
                Solicita_id_usuario_radicacion_plantilla_radicado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_usuario_radicacion_plantilla_radicado = "Inconistencia general función Solicita_id_usuario_radicacion_plantilla_radicado " & ex.Message
        End Try
    End Function
End Class
