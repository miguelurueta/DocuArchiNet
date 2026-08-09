Imports System.IO
Imports MySql.Data
Public Class CDclasificacionTipoDocumental
    Property IdTipoDocumento As Integer
    Property IdArea As Integer = 0
    Property IdSerie As Integer = 0
    Property IdSubSerie As Integer = 0
    Property DescripcionTipoDocumento As String
    Property NombreArea As String
    Property NombreSerie As String
    Property NombreSubSerie As String
End Class
Public Class ClassGaTipoDocumental
    Function SolicitaEstructuraClasificacionTipoDocumento(ByVal IdTipoListaCheck As Integer,
                                                          ByRef CDclasificacionTipoDocumental As CDclasificacionTipoDocumental) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la estructura de clasificacación de un tipo docimental de lista de 
        '          chequeo o lista de tipologias
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdTipoListaCheck    : Representa la identificación del tipo documento lista de chequeo
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'CDclasificacionTipoDocumental  : Retorna la estructura de clasifcación
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-07-09
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim StruTipoListaChequeo As stru_tipo_lista_chequeo = Nothing
            CDclasificacionTipoDocumental = New CDclasificacionTipoDocumental
            Dim ra_dig_tipos_docum_lista_chequeo As New ra_dig_tipos_docum_lista_chequeo
            Result = ra_dig_tipos_docum_lista_chequeo.SolicitaDatosTipoDocumentalListaChequeo(IdTipoListaCheck,
                                                                                                   StruTipoListaChequeo)
            If Result <> "YES" Then
                SolicitaEstructuraClasificacionTipoDocumento = Result
                Exit Function
            End If
            If StruTipoListaChequeo.subseries_documentales_Id_SubSeries <> 0 Then
                CDclasificacionTipoDocumental.IdTipoDocumento = StruTipoListaChequeo.tipos_doc_subseries_Id_Tipos_Doc_SubSerie
            Else
                CDclasificacionTipoDocumental.IdTipoDocumento = StruTipoListaChequeo.tipo_doc_series_Id_Tipo_Doc_Series
            End If

            '//-----Retorna serie y sub serie tipo documento-----//
            Dim StruTipoDocumental As stru_tipo_documental = Nothing
            Dim ClassTrdDocumental As New ClassTrdDocumental
            Result = ClassTrdDocumental.Solicita_datos_estructura_tipo_documento(CDclasificacionTipoDocumental.IdTipoDocumento,
                                                                                 StruTipoDocumental)
            If Result <> "YES" Then
                SolicitaEstructuraClasificacionTipoDocumento = Result
                Exit Function
            End If
            CDclasificacionTipoDocumental.IdSerie = StruTipoDocumental.Series_Documentales_Id_Series
            CDclasificacionTipoDocumental.IdSubSerie = StruTipoDocumental.sub_serie_id_serie
            Dim ref_Class_series_documentales As New Class_series_documentales
            '//-----Retorna la identificación del area-----//
            Result = ref_Class_series_documentales.Retorna_id_area_serie_documental(CDclasificacionTipoDocumental.IdSerie,
                                                                                    CDclasificacionTipoDocumental.IdArea)
            If Result <> "YES" Then
                SolicitaEstructuraClasificacionTipoDocumento = Result
                Exit Function
            End If
            '//-----Retorna la descripción del documento-----//
            Dim Class_ra_tipo_doc_series As New Class_ra_tipo_doc_series
            If CDclasificacionTipoDocumental.IdTipoDocumento <> 0 Then
                Result = Class_ra_tipo_doc_series.Retorna_nombre_tipo_documental(CDclasificacionTipoDocumental.IdSerie,
                                                                                 CDclasificacionTipoDocumental.IdSubSerie,
                                                                                 CDclasificacionTipoDocumental.IdTipoDocumento,
                                                                                 CDclasificacionTipoDocumental.DescripcionTipoDocumento)
                If Result <> "YES" Then
                    SolicitaEstructuraClasificacionTipoDocumento = Result
                    Exit Function
                End If
            End If
            '//-----Retorna el nombre del area-----//
            Dim Class_areas_depart_radicacion As New Class_areas_depart_radicacion
            If CDclasificacionTipoDocumental.IdArea <> 0 Then
                Result = Class_areas_depart_radicacion.Retorna_nombre_area_por_id_area(CDclasificacionTipoDocumental.IdArea,
                                                                                       CDclasificacionTipoDocumental.NombreArea)
                If Result <> "YES" Then
                    SolicitaEstructuraClasificacionTipoDocumento = Result
                    Exit Function
                End If
            End If
            '//-----Retorna el nombre de la serie-----//
            If CDclasificacionTipoDocumental.IdSerie <> 0 Then
                Result = ref_Class_series_documentales.Retorna_nombre_serie_id_serie(CDclasificacionTipoDocumental.IdSerie,
                                                                                     CDclasificacionTipoDocumental.NombreSerie)
                If Result <> "YES" Then
                    SolicitaEstructuraClasificacionTipoDocumento = Result
                    Exit Function
                End If
            End If
            '//-----Retorna el nombre de la sub serie-----//
            Dim Class_subseries_documentales As New Class_subseries_documentales
            If CDclasificacionTipoDocumental.IdSubSerie <> 0 Then
                Result = Class_subseries_documentales.Retorna_nombre_sub_serie(CDclasificacionTipoDocumental.IdSubSerie,
                                                                                CDclasificacionTipoDocumental.NombreSubSerie)
                If Result <> "YES" Then
                    SolicitaEstructuraClasificacionTipoDocumento = Result
                    Exit Function
                End If
            End If
            SolicitaEstructuraClasificacionTipoDocumento = "YES"
            Exit Function
        Catch ex As Exception
            SolicitaEstructuraClasificacionTipoDocumento = "Inconsistencia general funcion SolicitaEstructuraClasificacionTipoDocumento " & ex.Message
        End Try
    End Function

    Function Solicita_tipos_documentales_combo_excluyentes(ByRef refcombo As DropDownList, _
                                                           ByVal matri_unidad() As String, _
                                                           ByVal nombre_tipo As String, _
                                                           ByRef update As UpdatePanel) As String
        '******************************************************
        'Funcion : Lista los tipos de documentos disponibles
        'para clasificacion 
        'Ing : Miguel Angel Urueta Miranda
        'Fecha : 2015-01-05
        '******************************************************
        Try
            refcombo.Items.Clear()
            'Dim Ref_Car_Conec As New Conect.vb.Dbase_Conction_Mysql
            Dim Parametro_Consulta As String = "SELECT DOCUMENTO " & _
            " FROM ra_tipo_documento " & _
            " where ESTADO_DOCUMENTO=1"
            If Not matri_unidad Is Nothing Then
                For i As Integer = 0 To matri_unidad.Length - 1
                    If i = 0 Then
                        Parametro_Consulta = Parametro_Consulta & " and UNIDAD_CONSERVA='" & _
                                            matri_unidad(i) & "'"
                    Else
                        Parametro_Consulta = Parametro_Consulta & " or UNIDAD_CONSERVA='" & _
                        matri_unidad(i) & "'"
                    End If

                Next
            End If
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_tipo_documento")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                update.Update()
                Solicita_tipos_documentales_combo_excluyentes = "Funcion Solicita_tipos_documentales_combo_excluyentes dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                update.Update()
                Solicita_tipos_documentales_combo_excluyentes = "YES"
                Exit Function
            Else
                refcombo.Items.Add("")
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    refcombo.Items.Add(Datset.Tables(0).Rows(i).Item(0))
                Next
                If nombre_tipo <> "" Then
                    refcombo.Text = nombre_tipo
                End If
                update.Update()
                Solicita_tipos_documentales_combo_excluyentes = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Solicita_tipos_documentales_combo_excluyentes = "Inconsistencia Solicita_tipos_documentales_combo_excluyentes " & ex.Message
        End Try
    End Function

    

    Function Solicita_ayuda_tipo_documento(ByVal nombre_documento As String, ByRef tex As String) As String
        '*****************************************************************
        'Funcion : solicita la ayuda de un tipo documentos especifico
        'ing : Miguel Angel Urueta Miranda
        'Fecha : 2015-01-05
        '*****************************************************************
        Try
            tex = ""
            Dim Parametro_Consulta As String = "SELECT AYUDA_DOCUMENTO " & _
            " FROM ra_tipo_documento " & _
            " where DOCUMENTO='" & nombre_documento & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_tipo_documento")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_ayuda_tipo_documento = "Funcion Solicita_ayuda_tipo_documento dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_ayuda_tipo_documento = "Imposible encontrar ayuda para la clase documento " & nombre_documento
                Exit Function
            Else
                tex = Datset.Tables(0).Rows(0).Item(0)
                Solicita_ayuda_tipo_documento = "YES"
                Exit Function
            End If
            
        Catch ex As Exception
            Solicita_ayuda_tipo_documento = "Inconsistencia funcion Solicita_ayuda_tipo_documento " & ex.Message
        End Try
    End Function

    Function SolicitaIdTipoFormatoDocumento(ByVal NombreFormatoDocumento As String,
                                            ByRef IdTipoClaseDocumento As Integer) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la identificación del tipo de formato de documento
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'NombreFormatoDocumento  : Representa nombre del tipo de formato de documento
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'IdTipoClaseDocumento  : Retorna la identificación del tipo de formato de documento
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-07-25
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try

            Dim Parametro_Consulta As String = "SELECT ID_TIPO_DOCUMENTO " &
            " FROM ra_tipo_documento " &
             " where DOCUMENTO='" & NombreFormatoDocumento & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_tipo_documento")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                SolicitaIdTipoFormatoDocumento = "Funcion SolicitaIdTipoFormatoDocumento dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaIdTipoFormatoDocumento = "Imposible encontrar la identificación del formado de documento (" & NombreFormatoDocumento & ")"
                Exit Function
            Else
                IdTipoClaseDocumento = Datset.Tables(0).Rows(0).Item(0)
                SolicitaIdTipoFormatoDocumento = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaIdTipoFormatoDocumento = "Inconsistencia funcion SolicitaIdTipoFormatoDocumento " & ex.Message
        End Try
    End Function

    Function Asigna_datos_tipo_documental_estructura(ByVal page1 As Page, _
                                                     ByRef matri_gestion As estructure_gestion, _
                                                     ByVal nombre_gabinete As String) As String
        '*********************************************************
        'Funcion : Asigna datos tipo documental documental de la 
        'interface de almacenamiento a la estructura
        'Fecha : 2015-01-05 Modificado para web 2015-06-24
        'Ing : Miguel Angel Urueta Miranda
        '*********************************************************
        Try
            '***********************************
            'Asigna datos trd estructura
            '***********************************
            '********************************************
            'Consulta opcion aplica trd
            '*******************************************
            Dim ref_Class_system1 As New Class_system1
            Dim Result As String = ""
            Dim opt_tipo_documental As Integer = 0
            Result = ref_Class_system1.Verfica_opcion_seleccion_unidad(opt_tipo_documental, _
                                                                       nombre_gabinete)
            If Result <> "YES" Then
                Asigna_datos_tipo_documental_estructura = Result
                Exit Function
            End If
            If opt_tipo_documental = 0 Then
                Asigna_datos_tipo_documental_estructura = "YES"
                Exit Function
            End If
            Dim FECHAELABORACION As Object = Nothing
            Dim CLASEDOCUMENTO As Object = Nothing
            Dim Hidden_id_tipo As Object = Nothing
            FECHAELABORACION = page1.FindControl("FECHAELABORACION")
            If FECHAELABORACION Is Nothing Then
                Asigna_datos_tipo_documental_estructura = "Función Asigna_datos_tipo_documental_estructura dice : imposible encontrar el control FECHAELABORACION "
                Exit Function
            End If
            CLASEDOCUMENTO = page1.FindControl("CLASEDOCUMENTO")
            If CLASEDOCUMENTO Is Nothing Then
                Asigna_datos_tipo_documental_estructura = "Función Asigna_datos_tipo_documental_estructura dice : imposible encontrar el control CLASEDOCUMENTO "
                Exit Function
            End If
            Hidden_id_tipo = page1.FindControl("Hidden_id_tipo")
            If Hidden_id_tipo Is Nothing Then
                Asigna_datos_tipo_documental_estructura = "Función Asigna_datos_tipo_documental_estructura dice : imposible encontrar el control Hidden_id_tipo "
                Exit Function
            End If
            matri_gestion.FECHA_ELABORACION = FECHAELABORACION.text
            matri_gestion.CLASE_DOCUMENTO = CLASEDOCUMENTO.text
            matri_gestion.ID_CLASE_DOCUMENTO = Hidden_id_tipo.value
            Asigna_datos_tipo_documental_estructura = "YES"
        Catch ex As Exception
            Asigna_datos_tipo_documental_estructura = "Inconsistencia general funcion Asigna_datos_tipo_documental_estructura " & ex.Message
        End Try
    End Function
    Function Asigna_datos_tipo_documental_estructura(ByVal stru_campos_docuarchi() As stru_campos_docuarchi, _
                                                     ByRef matri_gestion As estructure_gestion, _
                                                     ByVal nombre_gabinete As String) As String
        '*********************************************************
        'Funcion : Asigna datos tipo documental documental de la 
        'interface de almacenamiento a la estructura
        'Fecha : 2015-01-05 Modificado para web 2015-06-24
        'Ing : Miguel Angel Urueta Miranda
        '*********************************************************
        Try
           
            Dim ref_Class_system1 As New Class_system1
            Dim Result As String = ""
            Dim opt_tipo_documental As Integer = 0
            Result = ref_Class_system1.Verfica_opcion_seleccion_unidad(opt_tipo_documental, _
                                                                       nombre_gabinete)
            If Result <> "YES" Then
                Asigna_datos_tipo_documental_estructura = Result
                Exit Function
            End If
            If opt_tipo_documental = 0 Then
                Asigna_datos_tipo_documental_estructura = "YES"
                Exit Function
            End If
            Dim ref_ClassWorkflowIndiceDA As New ClassWorkflowIndiceDA
            Dim valor_campo As String = ""
            Result = ref_ClassWorkflowIndiceDA.Solicita_valor_campo_estructura_docuarchi(stru_campos_docuarchi, _
                                                                                        "FECHAELABORACION", _
                                                                                         valor_campo)
            If Result <> "YES" Then
                Asigna_datos_tipo_documental_estructura = Result
                Exit Function
            End If
            matri_gestion.FECHA_ELABORACION = valor_campo
            Result = ref_ClassWorkflowIndiceDA.Solicita_valor_campo_estructura_docuarchi(stru_campos_docuarchi, _
                                                                                        "CLASEDOCUMENTO", _
                                                                                         valor_campo)
            If Result <> "YES" Then
                Asigna_datos_tipo_documental_estructura = Result
                Exit Function
            End If
            matri_gestion.CLASE_DOCUMENTO = valor_campo
            Result = ref_ClassWorkflowIndiceDA.Solicita_valor_campo_estructura_docuarchi(stru_campos_docuarchi, _
                                                                                        "Hidden_id_tipo", _
                                                                                         valor_campo)
            If Result <> "YES" Then
                Asigna_datos_tipo_documental_estructura = Result
                Exit Function
            End If
            matri_gestion.ID_CLASE_DOCUMENTO = valor_campo
            Asigna_datos_tipo_documental_estructura = "YES"
        Catch ex As Exception
            Asigna_datos_tipo_documental_estructura = "Inconsistencia general funcion Asigna_datos_tipo_documental_estructura " & ex.Message
        End Try
    End Function
End Class
