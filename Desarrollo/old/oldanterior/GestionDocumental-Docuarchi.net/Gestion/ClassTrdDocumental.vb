Imports System.IO
Imports MySql.Data
Imports MySql.Data.MySqlClient

Public Structure sub_series_t
    Dim id_sub_serie As Integer
    Dim consecutivo_subserie As Integer
End Structure
Public Structure Disposicion_Final
    Dim CONSECUTIVO_SUBSERIE As Integer
    Dim CONSECUTIVO_TIP_DOC As Integer
    Dim TIEMPO_RET_ARCH_CENTRAL As Integer
    Dim TIEMPO_RET_ARCH_GESTION As Integer
    Dim CONSERVACION_TOTAL As Integer
    Dim ELIMINACION As Integer
    Dim MICROFILM As Integer
    Dim SELECCION As Integer
    Dim OBSERVACIONES As String
    Dim ESTADO_DESICION As Integer

End Structure
Public Structure areas_depart_radicacion
    Dim Codigo_Area As Integer
    Dim Nombre_Area As String
    Dim matri_series() As Serie_documental
End Structure
Public Structure Serie_documental
    Dim Id_Series As Integer
    Dim Nombre_Serie As String
    Dim matri_sub_serie() As subseries_documentales
    Dim matri_tipo_doc_series() As tipo_doc_series
End Structure
Public Structure subseries_documentales
    Dim Id_SubSeries As Integer
    Dim Nombre_Subserie As String
    Dim matri_tipo_doc_series() As tipo_doc_series
End Structure
Public Structure tipo_doc_series
    Dim Id_Tipo_Doc_Series As Integer
    Dim Descripcion_Documento As String
End Structure
Public Structure stru_serie_documental
    Dim Id_Series As Integer
    Dim Areas_Depart_Radicacion_Codigo_Area As Integer
    Dim Consecutivo_Serie As Integer
    Dim Nombre_Serie As String
    Dim Estado_Serie As Integer
    Dim Consecutivo_subserie As Integer
    Dim Consecutivo_Tip_Doc As Integer
    Dim Tiempo_Ret_Arch_Central As Integer
    Dim Tiempo_Ret_Archivo_Historico As Integer
    Dim Conservacion_Total As Integer
    Dim Eliminacion As Integer
    Dim Microfilm As Integer
    Dim Seleccion As Integer
    Dim observaciones As String
    Dim Tiempo_Ret_Arch_Gestion As Integer
    Dim ESTADO_DESICION As Integer
    Dim Estado_Publico_Serie As Integer
    Dim Proceso As String
    Dim Procedimiento As String
    Dim Medio_soporte As String
    Dim Codigo_Arbitrario As String
    Dim Ra_registro_instrumento_archivistico_id_instrumento As Integer
End Structure
Public Structure stru_sub_serie_documental
    Dim Id_SubSeries As Integer
    Dim Series_Documentales_Id_Series As Integer
    Dim Nombre_Subserie As String
    Dim Consecutivo_Subserie As Integer
    Dim Estado_SubSerie As Integer
    Dim Consecutivo_Tip_Doc As Integer
    Dim TIEMPO_RET_ARCH_CENTRAL As Integer
    Dim TIEMPO_RET_ARCH_HISTORICO As Integer
    Dim TIEMPO_RET_ARCH_GESTION As Integer
    Dim ELIMINACION As Integer
    Dim MICROFILM As Integer
    Dim DIGITALIZACION As Integer
    Dim SELECCION As Integer
    Dim observaciones As String
    Dim ESTADO_DESICION As String
    Dim CONSERVACION_TOTAL As Integer
    Dim Estado_Publico_Sub_Serie As Integer
    Dim Proceso As String
    Dim Procedimiento As String
    Dim Medio_soporte As String
    Dim Codigo_Arbitrario As String
    Dim Ra_registro_instrumento_archivistico_id_instrumento As Integer
End Structure
Public Structure stru_tipo_documental
    Dim Series_Documentales_Id_Series As Integer
    Dim Consecutivo_Tip_Doc As String
    Dim Descripcion_Documento As String
    Dim Fecha_Creacion As String
    Dim Estado_Tipo As Integer
    Dim PLANTILLA As String
    Dim EXTENSION_ARCHIVO As String
    Dim sub_serie_id_serie As Integer
    Dim codigo_documento As String
    Dim id_instrumento As Integer
    Dim tipo_doc_trasversal As Integer
End Structure
Public Class ClassTrdDocumental  
    

    Function Retorna_id_organigrama_empresa_gestion(ByVal id_empresa As Integer, _
                                                    ByRef id_organigrama As Integer) As String
        '*************************************************
        'Funcion : Retorna organigrama usuario gestion
        'Ing : Miguel Angel Urueta Miranda
        'Fecha : 2014-12-26
        '*************************************************
        Try

            Dim Parametro_Consulta As String = "SELECT ID_ORGANIGRAMA FROM registro_organigrama WHERE EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA=" & _
            id_empresa & " AND ESTADO_ORGANIGRAMA=1 "
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("system1")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_id_organigrama_empresa_gestion = "Funcion  Retorna_id_organigrama_empresa_gestion dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_id_organigrama_empresa_gestion = "Imposible Encontrar datos del organigrama"
                Exit Function
            Else
                id_organigrama = Datset.Tables(0).Rows(0).Item(0)
                Retorna_id_organigrama_empresa_gestion = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Retorna_id_organigrama_empresa_gestion = "Inconsistencia general funcion Retorna_id_organigrama_empresa_gestion " & ex.Message
        End Try
    End Function

    Function Retorna_id_area_por_organigrama_nombrearea(ByVal id_organigrama As Integer, _
                                                        ByVal nombre_area As String, _
                                                        ByRef id_area As Integer) As String
        '******************************************************
        'Funcion : retorna el id del area con el organigrama
        'y el nombre del area
        'Ingeniero : Miguel Angel Urueta Miranda
        'Fecha : 2015-12-26
        '******************************************************
        Try
            Dim Parametro_Consulta As String = "Select Codigo_Area from areas_depart_radicacion where Nombre_Area='" & nombre_area & "'" & _
            " and Registro_Organigrama_Id_Organigrama=" & id_organigrama
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("system1")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_id_area_por_organigrama_nombrearea = "Funcion  Retorna_id_area_por_organigrama_nombrearea dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_id_area_por_organigrama_nombrearea = "Imposible encontrar el codigo del area seleccionada"
                Exit Function
            Else
                id_area = Datset.Tables(0).Rows(0).Item(0)
                Retorna_id_area_por_organigrama_nombrearea = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Retorna_id_area_por_organigrama_nombrearea = "Inconsistencia general funcion Retorna_id_area_por_organigrama_nombrearea " & ex.Message
        End Try
    End Function
    Function solicita_areas_de_gestion_organigrama(ByVal id_organigrama As Integer, _
                                                   ByRef combo_ref As DropDownList)
        '*************************************************************
        'Funcion : Lista las areas registradas en el organigrama
        'Fecha : 2014-12-26
        'Ing : Miguel Angel Urueta Miranda
        '*************************************************************
        Try

            Dim Parametro_Consulta As String = "SELECT Nombre_Area FROM areas_depart_radicacion WHERE REGISTRO_ORGANIGRAMA_ID_ORGANIGRAMA=" & _
            id_organigrama & " AND Estado_Area=1 "
            combo_ref.Items.Clear()
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("system1")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                solicita_areas_de_gestion_organigrama = "Funcion  solicita_areas_de_gestion_organigrama dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                solicita_areas_de_gestion_organigrama = "YES"
                Exit Function
            Else
                combo_ref.Items.Add("Seleccione Área o departamento")
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    combo_ref.Items.Add(Datset.Tables(0).Rows(i).Item(0))
                Next
                solicita_areas_de_gestion_organigrama = "YES"
                Exit Function
            End If

        Catch ex As Exception
            solicita_areas_de_gestion_organigrama = "Inconsistencia general funcion solicita_areas_de_gestion_organigrama " & ex.Message
        End Try
    End Function

    Function Retorna_Datos_Datos_Area(ByVal id_user_wf As Integer, _
                                      ByRef cod_area As Integer, _
                                      ByRef nombre_area As String, _
                                      ByRef Razon_Social As String, _
                                      ByRef id_empresa As Integer) As String
        Try

            Dim Parametro_Consulta As String = "SELECT adr.Codigo_Area,adr.nombre_area,egd.RAZON_SOCIAL_EMPRESA,egd.ID_EMPRESA " & _
            " FROM remit_dest_interno  as  rdi" & _
            " inner join areas_depart_radicacion as adr  on " & _
            " (adr.Codigo_Area=rdi.areas_dep_radicacion_id_Areas_Dep) " & _
            " inner join empresa_gestion_documental as egd on " & _
            " (egd.ID_EMPRESA = rdi.Empresa_Gestion_Documental_id_empresa) " & _
            " where id_remit_dest_int=" & id_user_wf
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("remit_dest_interno")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_Datos_Datos_Area = "Funcion  Retorna_Datos_Datos_Area dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_Datos_Datos_Area = "Imposible Encontrar datos del área"
                Exit Function
            Else
                cod_area = Datset.Tables(0).Rows(0).Item(0)
                nombre_area = Datset.Tables(0).Rows(0).Item(1)
                Razon_Social = Datset.Tables(0).Rows(0).Item(2)
                id_empresa = Datset.Tables(0).Rows(0).Item(3)
                Retorna_Datos_Datos_Area = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Retorna_Datos_Datos_Area = "Inconsistencia General Funcion  Retorna_Datos_Datos_Area " & ex.Message
        End Try
    End Function

    Function Retorna_Datos_Datos_Area(ByVal id_user_wf As Integer, _
                                      ByRef cod_area As Integer, _
                                      ByRef nombre_area As String, _
                                      ByRef Razon_Social As String) As String
        Try

            Dim Parametro_Consulta As String = "SELECT adr.Codigo_Area,adr.nombre_area,egd.RAZON_SOCIAL_EMPRESA " & _
            " FROM remit_dest_interno  as  rdi" & _
            " inner join areas_depart_radicacion as adr  on " & _
            " (adr.Codigo_Area=rdi.areas_dep_radicacion_id_Areas_Dep) " & _
            " inner join empresa_gestion_documental as egd on " & _
            " (egd.ID_EMPRESA = rdi.Empresa_Gestion_Documental_id_empresa) " & _
            " where relacion_workflow=" & id_user_wf
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("remit_dest_interno")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_Datos_Datos_Area = "Funcion  Retorna_Datos_Datos_Area 2 dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_Datos_Datos_Area = "Imposible Encontrar datos del área"
                Exit Function
            Else
                cod_area = Datset.Tables(0).Rows(0).Item(0)
                nombre_area = Datset.Tables(0).Rows(0).Item(1)
                Razon_Social = Datset.Tables(0).Rows(0).Item(2)
                Retorna_Datos_Datos_Area = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Retorna_Datos_Datos_Area = "Inconsistencia General Funcion  Retorna_Datos_Datos_Area " & ex.Message
        End Try
    End Function

    Function Retorna_Codigo_Arbitrario_SerieDoc(ByVal Id_AreaDep As String, _
                                                ByRef Codigo_arbitrario As String) As String
        Try

            Dim Parametro_Consulta As String = "Select CODIGO_ARBITRARIO " & _
            " from AREAS_DEPART_RADICACION where CODIGO_AREA=" & Id_AreaDep
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("AREAS_DEPART_RADICACION")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_Codigo_Arbitrario_SerieDoc = "Funcion  Retorna_Codigo_Arbitrario_SerieDoc dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_Codigo_Arbitrario_SerieDoc = "Imposible Encontrar código arbitrario"
                Exit Function
            Else
                Codigo_arbitrario = Datset.Tables(0).Rows(0).Item(0)
                Retorna_Codigo_Arbitrario_SerieDoc = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_Codigo_Arbitrario_SerieDoc = ex.Message
        End Try
    End Function
    Function lista_series_sub_series_tipo(ByVal id_instrumento As Integer, _
                                          ByVal id_area As Integer, _
                                          ByVal id_serie As Integer, _
                                          ByVal id_sub_serie As Integer, _
                                          ByVal nombre_tipo As String, _
                                          ByRef scripma As GridView, _
                                          ByRef update As UpdatePanel) As String
        '***********************************************
        'Funcion : Lista las series, sub seires y tipos
        'de un area especifica con el id del area como
        'parametro
        'Fecha 2014-12-26
        'Ing : Miguel Angel Urueta Miranda
        '***********************************************
        Try
            Dim Parametro_consulta As String = ""
            '--------Consulta por area
            If id_area <> -1 And id_serie = -1 And id_sub_serie = -1 And id_instrumento = -1 And nombre_tipo = "" Then
                Parametro_consulta = "select  sd.id_series,sbd.id_subseries,tds.Id_Tipo_Doc_Series,sd.nombre_serie as NOMBRE_SERIE,sbd.Nombre_Subserie AS NOMBRE_SUB_SERIE,tds.Descripcion_Documento AS TIPO_DOCUMENTAL" & _
                ",sd.Tiempo_Ret_Arch_Gestion as TIEMPO_RT_ARCH_GESTION_SERIE,sd.Tiempo_Ret_Arch_Central as TIEMPO_RT_ARCH_CENTRAL_SERIE,sbd.TIEMPO_RET_ARCH_GESTION AS TIEMPO_RT_ARCH_GESTION_SUB_SERIE," & _
                "sbd.TIEMPO_RET_ARCH_CENTRAL as TIEMPO_RT_ARCH_CENTRAL_SUB_SERIE"
                Parametro_consulta = Parametro_consulta & _
               " from tipo_doc_series as tds  " & _
               " left outer join series_documentales as sd   on (tds.series_documentales_id_series=sd.Id_Series) " & _
               " left outer  join subseries_documentales as sbd on (tds.sub_serie_id_serie=sbd.Id_SubSeries) " & _
               "   where   Areas_Depart_Radicacion_Codigo_Area=" & id_area & " and Estado_Tipo=1 order by nombre_serie,Nombre_Subserie "
            End If
            If nombre_tipo <> "" And id_instrumento <> -1 Then
                Parametro_consulta = "select  sd.id_series,sbd.id_subseries,tds.Id_Tipo_Doc_Series,sd.nombre_serie as NOMBRE_SERIE,sbd.Nombre_Subserie AS NOMBRE_SUB_SERIE,tds.Descripcion_Documento AS TIPO_DOCUMENTAL" & _
              ",sd.Tiempo_Ret_Arch_Gestion as TIEMPO_RT_ARCH_GESTION_SERIE,sd.Tiempo_Ret_Arch_Central as TIEMPO_RT_ARCH_CENTRAL_SERIE,sbd.TIEMPO_RET_ARCH_GESTION AS TIEMPO_RT_ARCH_GESTION_SUB_SERIE," & _
              "sbd.TIEMPO_RET_ARCH_CENTRAL as TIEMPO_RT_ARCH_CENTRAL_SUB_SERIE"
                Parametro_consulta = Parametro_consulta & _
               " from series_documentales as sd " & _
               "  left outer join tipo_doc_series as tds   on (tds.series_documentales_id_series=sd.Id_Series) " & _
               "  left outer  join subseries_documentales as sbd on (tds.sub_serie_id_serie=sbd.Id_SubSeries) " & _
               "   where   Descripcion_Documento like '%" & nombre_tipo & "%' and id_instrumento=" & id_instrumento & " and Estado_Tipo=1 order by nombre_serie,Nombre_Subserie "
            End If
            If nombre_tipo <> "" And id_instrumento <> -1 And id_serie <> -1 Then
                Parametro_consulta = "select  sd.id_series,sbd.id_subseries,tds.Id_Tipo_Doc_Series,sd.nombre_serie as NOMBRE_SERIE,sbd.Nombre_Subserie AS NOMBRE_SUB_SERIE,tds.Descripcion_Documento AS TIPO_DOCUMENTAL" & _
              ",sd.Tiempo_Ret_Arch_Gestion as TIEMPO_RT_ARCH_GESTION_SERIE,sd.Tiempo_Ret_Arch_Central as TIEMPO_RT_ARCH_CENTRAL_SERIE,sbd.TIEMPO_RET_ARCH_GESTION AS TIEMPO_RT_ARCH_GESTION_SUB_SERIE," & _
              "sbd.TIEMPO_RET_ARCH_CENTRAL as TIEMPO_RT_ARCH_CENTRAL_SUB_SERIE"
                Parametro_consulta = Parametro_consulta & _
               " from series_documentales as sd " & _
               "  left outer join tipo_doc_series as tds   on (tds.series_documentales_id_series=sd.Id_Series) " & _
               "  left outer  join subseries_documentales as sbd on (tds.sub_serie_id_serie=sbd.Id_SubSeries) " & _
               "   where   Descripcion_Documento like '%" & nombre_tipo & "%' and id_instrumento=" & id_instrumento & _
               " and Estado_Tipo=1 " & " and Id_Series=" & id_serie & " and sub_serie_id_serie is null order by nombre_serie,Nombre_Subserie "
            End If
            If nombre_tipo = "" And id_instrumento <> -1 And id_serie <> -1 Then
                Parametro_consulta = "select  sd.id_series,sbd.id_subseries,tds.Id_Tipo_Doc_Series,sd.nombre_serie as NOMBRE_SERIE,sbd.Nombre_Subserie AS NOMBRE_SUB_SERIE,tds.Descripcion_Documento AS TIPO_DOCUMENTAL" & _
              ",sd.Tiempo_Ret_Arch_Gestion as TIEMPO_RT_ARCH_GESTION_SERIE,sd.Tiempo_Ret_Arch_Central as TIEMPO_RT_ARCH_CENTRAL_SERIE,sbd.TIEMPO_RET_ARCH_GESTION AS TIEMPO_RT_ARCH_GESTION_SUB_SERIE," & _
              "sbd.TIEMPO_RET_ARCH_CENTRAL as TIEMPO_RT_ARCH_CENTRAL_SUB_SERIE"
                Parametro_consulta = Parametro_consulta & _
               " from series_documentales as sd " & _
               "  left outer join tipo_doc_series as tds   on (tds.series_documentales_id_series=sd.Id_Series) " & _
               "  left outer  join subseries_documentales as sbd on (tds.sub_serie_id_serie=sbd.Id_SubSeries) " & _
               "   where  id_instrumento=" & id_instrumento & _
               " and Estado_Tipo=1 " & " and Id_Series=" & id_serie & "  and sub_serie_id_serie is null order by nombre_serie,Nombre_Subserie "
            End If
            If nombre_tipo <> "" And id_instrumento <> -1 And id_serie <> -1 And id_sub_serie <> -1 Then
                Parametro_consulta = "select  sd.id_series,sbd.id_subseries,tds.Id_Tipo_Doc_Series,sd.nombre_serie as NOMBRE_SERIE,sbd.Nombre_Subserie AS NOMBRE_SUB_SERIE,tds.Descripcion_Documento AS TIPO_DOCUMENTAL" & _
              ",sd.Tiempo_Ret_Arch_Gestion as TIEMPO_RT_ARCH_GESTION_SERIE,sd.Tiempo_Ret_Arch_Central as TIEMPO_RT_ARCH_CENTRAL_SERIE,sbd.TIEMPO_RET_ARCH_GESTION AS TIEMPO_RT_ARCH_GESTION_SUB_SERIE," & _
              "sbd.TIEMPO_RET_ARCH_CENTRAL as TIEMPO_RT_ARCH_CENTRAL_SUB_SERIE"
                Parametro_consulta = Parametro_consulta & _
               " from  subseries_documentales as sbd" & _
                "  left outer join series_documentales as sd  on (sd.Id_Series=sbd.Series_Documentales_Id_Series) " & _
               "  left outer join tipo_doc_series as tds   on (tds.sub_serie_id_serie=sbd.Id_SubSeries) " & _
               "  where   Descripcion_Documento like '%" & nombre_tipo & "%' and id_instrumento=" & id_instrumento & _
               " and Estado_Tipo=1 " & " and Id_SubSeries=" & id_sub_serie & " order by nombre_serie,Nombre_Subserie "
            End If
            If nombre_tipo = "" And id_instrumento <> -1 And id_serie <> -1 And id_sub_serie <> -1 Then
                Parametro_consulta = "select  sd.id_series,sbd.id_subseries,tds.Id_Tipo_Doc_Series,sd.nombre_serie as NOMBRE_SERIE,sbd.Nombre_Subserie AS NOMBRE_SUB_SERIE,tds.Descripcion_Documento AS TIPO_DOCUMENTAL" & _
               ",sd.Tiempo_Ret_Arch_Gestion as TIEMPO_RT_ARCH_GESTION_SERIE,sd.Tiempo_Ret_Arch_Central as TIEMPO_RT_ARCH_CENTRAL_SERIE,sbd.TIEMPO_RET_ARCH_GESTION AS TIEMPO_RT_ARCH_GESTION_SUB_SERIE," & _
               "sbd.TIEMPO_RET_ARCH_CENTRAL as TIEMPO_RT_ARCH_CENTRAL_SUB_SERIE"
                Parametro_consulta = Parametro_consulta & _
               " from  subseries_documentales as sbd" & _
                "  left outer join series_documentales as sd  on (sd.Id_Series=sbd.Series_Documentales_Id_Series) " & _
               "  left outer join tipo_doc_series as tds   on (tds.sub_serie_id_serie=sbd.Id_SubSeries) " & _
               "  where  id_instrumento=" & id_instrumento & _
               " and Estado_Tipo=1 " & " and Id_SubSeries=" & id_sub_serie & " order by nombre_serie,Nombre_Subserie "
            End If
            If Parametro_consulta = "" Then
                scripma.DataSource = Nothing
                scripma.DataBind()
                update.Update()
                lista_series_sub_series_tipo = "YES"
                Exit Function
            End If
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("series_documentales")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Parametro_consulta, Datset)
            If Result <> "YES" Then
                lista_series_sub_series_tipo = "Función dice lista_series_sub_series_tipo " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                scripma.DataSource = Datset
                scripma.DataBind()
                update.Update()
                lista_series_sub_series_tipo = "YES"
                Exit Function
            Else
                scripma.DataSource = Datset
                scripma.DataBind()
                For i As Integer = 0 To scripma.Rows.Count - 1
                    Dim celtext As String = "0"
                    Dim celtexext2 As String = "0"
                    If Trim(scripma.Rows(i).Cells(3).Text.ToString) <> "&nbsp;" Then
                        celtext = scripma.Rows(i).Cells(3).Text
                    End If
                    If Trim(scripma.Rows(i).Cells(2).Text.ToString) <> "&nbsp;" Then
                        celtexext2 = scripma.Rows(i).Cells(2).Text
                    End If
                    Dim tex As String = scripma.Rows(i).Cells(1).Text & "-" & celtexext2 & "-" & celtext
                    scripma.Rows(i).Attributes.Add("id", tex)
                    For z As Integer = 0 To scripma.Rows(i).Cells.Count - 1
                        'If z > 0 Then
                        scripma.Rows(i).Cells(z).Attributes.Add("Class", "GridviewScrollItem_line_cort_tr_flex")
                        scripma.Rows(i).Cells(z).Attributes.Add("onclick", "prevent_scrol(event,this);")
                        'End If
                    Next
                Next
                update.Update()
                lista_series_sub_series_tipo = "YES"
            End If

        Catch ex As Exception
            lista_series_sub_series_tipo = "Inconsistencia general funcion lista_series_sub_series_tipo " & ex.Message
        End Try
    End Function
    Function Listar_Series_Documentales(ByVal Id_Areadep As String, _
        ByRef stru_serie() As stru_serie_documental) As String
        Try
            Dim Parametro_Consulta = "select Areas_Depart_Radicacion_Codigo_Area,Consecutivo_Serie,Nombre_Serie," & _
             "Estado_Serie,Consecutivo_subserie,Consecutivo_Tip_Doc,Tiempo_Ret_Arch_Central,Tiempo_Ret_Archivo_Historico," & _
             "Conservacion_Total,Eliminacion,Microfilm,Seleccion,observaciones,Tiempo_Ret_Arch_Gestion,ESTADO_DESICION," & _
             "Estado_Publico_Serie,Proceso,Procedimiento,Medio_soporte,Codigo_Arbitrario,Ra_registro_instrumento_archivistico_id_instrumento,Id_Series " & _
             "from series_documentales" & _
             " WHERE Areas_Depart_Radicacion_Codigo_Area=" & Id_Areadep
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("SERIES_DOCUMENTALES")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Listar_Series_Documentales = "Funcion  Listar_Series_Documentales dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Listar_Series_Documentales = "Imposible encontrar los datos de la serie documental"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru_serie(i)
                    stru_serie(i).Areas_Depart_Radicacion_Codigo_Area = Datset.Tables(0).Rows(i).Item(0)
                    stru_serie(i).Consecutivo_Serie = Datset.Tables(0).Rows(i).Item(1)
                    stru_serie(i).Nombre_Serie = Datset.Tables(0).Rows(i).Item(2)
                    stru_serie(i).Estado_Serie = Datset.Tables(0).Rows(i).Item(3)
                    stru_serie(i).Consecutivo_subserie = Datset.Tables(0).Rows(i).Item(4)
                    stru_serie(i).Consecutivo_Tip_Doc = Datset.Tables(0).Rows(i).Item(5)
                    stru_serie(i).Tiempo_Ret_Arch_Central = Datset.Tables(0).Rows(i).Item(6)
                    stru_serie(i).Tiempo_Ret_Archivo_Historico = Datset.Tables(0).Rows(i).Item(7)
                    stru_serie(i).Conservacion_Total = Datset.Tables(0).Rows(i).Item(8)
                    stru_serie(i).Eliminacion = Datset.Tables(0).Rows(i).Item(9)
                    stru_serie(i).Microfilm = Datset.Tables(0).Rows(i).Item(10)
                    stru_serie(i).Seleccion = Datset.Tables(0).Rows(i).Item(11)
                    If Datset.Tables(0).Rows(i).IsNull(12) Then
                        stru_serie(i).observaciones = ""
                    Else
                        stru_serie(i).observaciones = Datset.Tables(0).Rows(i).Item(12)
                    End If
                    stru_serie(i).Tiempo_Ret_Arch_Gestion = Datset.Tables(0).Rows(i).Item(13)
                    stru_serie(i).ESTADO_DESICION = Datset.Tables(0).Rows(i).Item(14)
                    stru_serie(i).Estado_Publico_Serie = Datset.Tables(0).Rows(i).Item(15)
                    If Datset.Tables(0).Rows(i).IsNull(16) Then
                        stru_serie(i).Proceso = ""
                    Else
                        stru_serie(i).Proceso = Datset.Tables(0).Rows(i).Item(16)
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(17) Then
                        stru_serie(i).Procedimiento = ""
                    Else
                        stru_serie(i).Procedimiento = Datset.Tables(0).Rows(i).Item(17)
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(18) Then
                        stru_serie(i).Medio_soporte = ""
                    Else
                        stru_serie(i).Medio_soporte = Datset.Tables(0).Rows(i).Item(18)
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(19) Then
                        stru_serie(i).Codigo_Arbitrario = ""
                    Else
                        stru_serie(i).Codigo_Arbitrario = Datset.Tables(0).Rows(i).Item(19)
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(20) Then
                        stru_serie(i).Ra_registro_instrumento_archivistico_id_instrumento = 0
                    Else
                        stru_serie(i).Ra_registro_instrumento_archivistico_id_instrumento = Datset.Tables(0).Rows(i).Item(20)
                    End If
                    stru_serie(i).Id_Series = Datset.Tables(0).Rows(i).Item(21)
                Next
                
                Listar_Series_Documentales = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Listar_Series_Documentales = "Inconsistencia General Funcion Listar_Series_Documentales " & ex.Message
        End Try
    End Function

    Function Listar_Tipdoc_Series(ByVal Id_Series As String, _
    ByRef Matri_Tip_Doc() As String) As String
        Try
            Dim Parametro_Consulta = "select Id_Tipo_Doc_Series,Descripcion_Documento FROM TIPO_DOC_SERIES " & _
            " WHERE Series_Documentales_Id_Series=" & Id_Series & " and sub_serie_id_serie is null"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("SERIES_DOCUMENTALES")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Listar_Tipdoc_Series = "Funcion  Listar_Tipdoc_Series dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Listar_Tipdoc_Series = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve Matri_Tip_Doc(i)
                    Matri_Tip_Doc(i) = Datset.Tables(0).Rows(i).Item(0).ToString & "|" & Datset.Tables(0).Rows(i).Item(1).ToString
                Next
                Listar_Tipdoc_Series = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Listar_Tipdoc_Series = "Inconsistencia función Listar_Tipdoc_Subseries " & ex.Message
        End Try
    End Function

    Function Listar_Tipdoc_Subseries(ByVal Id_subseries As String, _
   ByRef Matri_Tip_Doc() As String) As String
        Try
            Dim Parametro_Consulta = "select Id_Tipo_Doc_Series,Descripcion_Documento FROM  tipo_doc_series" & _
            " WHERE sub_serie_id_serie=" & Id_subseries
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("SERIES_DOCUMENTALES")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Listar_Tipdoc_Subseries = "Funcion  Listar_Tipdoc_Subseries dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Listar_Tipdoc_Subseries = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve Matri_Tip_Doc(i)
                    Matri_Tip_Doc(i) = Datset.Tables(0).Rows(i).Item(0).ToString & "|" & Datset.Tables(0).Rows(i).Item(1).ToString
                Next
                Listar_Tipdoc_Subseries = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Listar_Tipdoc_Subseries = "Inconsistencia Listar_Tipdoc_Subseries " & ex.Message
        End Try
    End Function

    Function Retorna_Datos_Disposicion_SubSerie(ByVal Id_Serie As Integer, ByRef Datos_Disp As Disposicion_Final) As String
        Try
            Dim Parametro_Consulta = "select CONSECUTIVO_SUBSERIE,CONSECUTIVO_TIP_DOC,TIEMPO_RET_ARCH_CENTRAL,TIEMPO_RET_ARCH_GESTION " & _
            ",CONSERVACION_TOTAL,ELIMINACION,MICROFILM,SELECCION,OBSERVACIONES,ESTADO_DESICION FROM SUBSERIES_DOCUMENTALES " & _
               " WHERE ID_SUBSERIES=" & Id_Serie
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("AREAS_DEPART_RADICACION")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_Datos_Disposicion_SubSerie = "Funcion  Retorna_Datos_Disposicion_SubSerie dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_Datos_Disposicion_SubSerie = "Imposible encontrar datos de disposición de la sub serie"
                Exit Function
            Else
                Datos_Disp.CONSECUTIVO_SUBSERIE = Datset.Tables(0).Rows(0).Item(0)
                Datos_Disp.CONSECUTIVO_TIP_DOC = Datset.Tables(0).Rows(0).Item(1)
                Datos_Disp.TIEMPO_RET_ARCH_CENTRAL = Datset.Tables(0).Rows(0).Item(2)
                Datos_Disp.TIEMPO_RET_ARCH_GESTION = Datset.Tables(0).Rows(0).Item(3)
                Datos_Disp.CONSERVACION_TOTAL = Datset.Tables(0).Rows(0).Item(4)
                Datos_Disp.ELIMINACION = Datset.Tables(0).Rows(0).Item(5)
                Datos_Disp.MICROFILM = Datset.Tables(0).Rows(0).Item(6)
                Datos_Disp.SELECCION = Datset.Tables(0).Rows(0).Item(7)
                Datos_Disp.OBSERVACIONES = Datset.Tables(0).Rows(0).Item(8)
                Datos_Disp.ESTADO_DESICION = Datset.Tables(0).Rows(0).Item(9)
                Retorna_Datos_Disposicion_SubSerie = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Retorna_Datos_Disposicion_SubSerie = "Inconsistencia General Funcion Retorna_Datos_Disposicion_SubSerie : " & ex.Message
        End Try
    End Function

    Function Listar_SubSeries_Documentales(ByVal id_SerieDoc As Integer _
    , ByRef stru_serie() As stru_sub_serie_documental) As String
        Try
            Dim Parametro_Consulta = "select Series_Documentales_Id_Series,Consecutivo_Subserie,Nombre_Subserie," & _
            "Estado_SubSerie,Consecutivo_Tip_Doc,TIEMPO_RET_ARCH_CENTRAL,TIEMPO_RET_ARCH_HISTORICO," & _
            "CONSERVACION_TOTAL,ELIMINACION,MICROFILM,SELECCION,observaciones,TIEMPO_RET_ARCH_GESTION,ESTADO_DESICION," & _
            "Estado_Publico_Sub_Serie,Proceso,Procedimiento,Medio_soporte,Codigo_Arbitrario,Ra_registro_instrumento_archivistico_id_instrumento,Id_SubSeries " & _
            "from subseries_documentales" & _
            " WHERE Series_Documentales_Id_Series=" & id_SerieDoc
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("SUBSERIES_DOCUMENTALES")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Listar_SubSeries_Documentales = "Funcion  Listar_SubSeries_Documentales dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Listar_SubSeries_Documentales = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru_serie(i)
                    stru_serie(i).Id_SubSeries = Datset.Tables(0).Rows(i).Item(0)
                    stru_serie(i).Series_Documentales_Id_Series = Datset.Tables(0).Rows(i).Item(0)
                    stru_serie(i).Consecutivo_Subserie = Datset.Tables(0).Rows(i).Item(1)
                    stru_serie(i).Nombre_Subserie = Datset.Tables(0).Rows(i).Item(2)
                    stru_serie(i).Estado_SubSerie = Datset.Tables(0).Rows(i).Item(3)
                    stru_serie(i).Consecutivo_Tip_Doc = Datset.Tables(0).Rows(i).Item(4)
                    stru_serie(i).TIEMPO_RET_ARCH_CENTRAL = Datset.Tables(0).Rows(i).Item(5)
                    stru_serie(i).TIEMPO_RET_ARCH_HISTORICO = Datset.Tables(0).Rows(i).Item(6)
                    stru_serie(i).CONSERVACION_TOTAL = Datset.Tables(0).Rows(i).Item(7)
                    stru_serie(i).ELIMINACION = Datset.Tables(0).Rows(i).Item(8)
                    stru_serie(i).MICROFILM = Datset.Tables(0).Rows(i).Item(9)
                    stru_serie(i).SELECCION = Datset.Tables(0).Rows(i).Item(10)
                    If Datset.Tables(0).Rows(i).IsNull(11) Then
                        stru_serie(i).observaciones = ""
                    Else
                        stru_serie(i).observaciones = Datset.Tables(0).Rows(i).Item(11)
                    End If
                    stru_serie(i).TIEMPO_RET_ARCH_GESTION = Datset.Tables(0).Rows(i).Item(12)
                    stru_serie(i).ESTADO_DESICION = Datset.Tables(0).Rows(i).Item(13)
                    stru_serie(i).Estado_Publico_Sub_Serie = Datset.Tables(0).Rows(i).Item(14)
                    If Datset.Tables(0).Rows(i).IsNull(15) Then
                        stru_serie(i).Proceso = ""
                    Else
                        stru_serie(i).Proceso = Datset.Tables(0).Rows(i).Item(15)
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(16) Then
                        stru_serie(i).Procedimiento = ""
                    Else
                        stru_serie(i).Procedimiento = Datset.Tables(0).Rows(i).Item(16)
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(17) Then
                        stru_serie(i).Medio_soporte = ""
                    Else
                        stru_serie(i).Medio_soporte = Datset.Tables(0).Rows(i).Item(17)
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(18) Then
                        stru_serie(i).Codigo_Arbitrario = ""
                    Else
                        stru_serie(i).Codigo_Arbitrario = Datset.Tables(0).Rows(i).Item(18)
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(19) Then
                        stru_serie(i).Ra_registro_instrumento_archivistico_id_instrumento = 0
                    Else
                        stru_serie(i).Ra_registro_instrumento_archivistico_id_instrumento = Datset.Tables(0).Rows(i).Item(19)
                    End If
                    stru_serie(i).Id_SubSeries = Datset.Tables(0).Rows(i).Item(20)
                Next
                Listar_SubSeries_Documentales = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Listar_SubSeries_Documentales = "Inconsistencia general función Listar_SubSeries_Documentales " & ex.Message
        End Try

    End Function

    Function Retorna_Datos_Disposicion_Serie(ByVal Id_Areadep As Integer, ByRef Datos_Disp As Disposicion_Final) As String
        Try
            Dim Parametro_Consulta = "select CONSECUTIVO_SUBSERIE,CONSECUTIVO_TIP_DOC,TIEMPO_RET_ARCH_CENTRAL,TIEMPO_RET_ARCH_GESTION " & _
            ",CONSERVACION_TOTAL,ELIMINACION,MICROFILM,SELECCION,OBSERVACIONES,ESTADO_DESICION FROM SERIES_DOCUMENTALES " & _
               " WHERE ID_SERIES=" & Id_Areadep
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("AREAS_DEPART_RADICACION")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_Datos_Disposicion_Serie = "Funcion  Retorna_Datos_Disposicion_Serie dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_Datos_Disposicion_Serie = "Imposible encontrar datos de disposición de la serie"
                Exit Function
            Else
                Datos_Disp.CONSECUTIVO_SUBSERIE = Datset.Tables(0).Rows(0).Item(0)
                Datos_Disp.CONSECUTIVO_TIP_DOC = Datset.Tables(0).Rows(0).Item(1)
                Datos_Disp.TIEMPO_RET_ARCH_CENTRAL = Datset.Tables(0).Rows(0).Item(2)
                Datos_Disp.TIEMPO_RET_ARCH_GESTION = Datset.Tables(0).Rows(0).Item(3)
                Datos_Disp.CONSERVACION_TOTAL = Datset.Tables(0).Rows(0).Item(4)
                Datos_Disp.ELIMINACION = Datset.Tables(0).Rows(0).Item(5)
                Datos_Disp.MICROFILM = Datset.Tables(0).Rows(0).Item(6)
                Datos_Disp.SELECCION = Datset.Tables(0).Rows(0).Item(7)
                Datos_Disp.OBSERVACIONES = Datset.Tables(0).Rows(0).Item(8)
                Datos_Disp.ESTADO_DESICION = Datset.Tables(0).Rows(0).Item(9)
                Retorna_Datos_Disposicion_Serie = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_Datos_Disposicion_Serie = "Inconsistencia General Funcion Retorna_Datos_Disposicion_Serie : " & ex.Message
        End Try
    End Function
   
    Function Retorna_unidad_conserva_tipo_documento(ByVal id_tipo_documento As Integer, _
                                                    ByRef unidad_conserva As String) As String
        '***********************************************************
        'Función : Retorna la unidad conservacion en la esta 
        'reperesntado el tipo de documento
        'Fecha : 2015-02-11
        'Ingeniero : Miguel Angel Urueta Miranda
        '***********************************************************
        Try
            If id_tipo_documento = 0 Then
                unidad_conserva = ""
                Retorna_unidad_conserva_tipo_documento = "YES"
                Exit Function
            End If
            Dim Parametro_Consulta As String = "Select UNIDAD_CONSERVA from ra_tipo_documento where ID_TIPO_DOCUMENTO='" & id_tipo_documento & "'"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("AREAS_DEPART_RADICACION")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_unidad_conserva_tipo_documento = "Funcion  Retorna_unidad_conserva_tipo_documento dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_unidad_conserva_tipo_documento = "Imposible encontrar el tipo de conservación"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    Retorna_unidad_conserva_tipo_documento = "Imposible encontrar el tipo de conservación es null"
                    Exit Function
                Else
                    unidad_conserva = Datset.Tables(0).Rows(0).Item(0)
                    If unidad_conserva <> "FISICO" And unidad_conserva <> "ELECTRONICO" _
                    And unidad_conserva <> "DIGITALIZADO" Then
                        Retorna_unidad_conserva_tipo_documento = "El sistema encontro icnonsistencia con el tipo de conservación " & unidad_conserva
                        Exit Function
                    End If
                    Retorna_unidad_conserva_tipo_documento = "YES"
                    Exit Function
                End If
            End If

        Catch ex As Exception
            Retorna_unidad_conserva_tipo_documento = "Inconsistencia función Retorna_unidad_conserva_tipo_documento " & ex.Message
        End Try
    End Function

    Function Suma_dias_fecha(ByVal fecha As Object, ByVal numero_año As Integer, ByRef fecha_result As String) As String
        Try
            If numero_año = 0 Then
                Suma_dias_fecha = "YES"
                Exit Function
            End If
            Dim numero_dias As Integer = 0
            If numero_año > 0 Then

                numero_dias = numero_año * 365
            Else

                numero_dias = (Math.Abs(numero_año)) * 30
            End If
            fecha_result = DateAdd(DateInterval.Day, numero_dias, fecha)
            Suma_dias_fecha = "YES"
        Catch ex As Exception
            Suma_dias_fecha = "Funcion Suma_dias_fecha =" & ex.Message
        End Try
    End Function
    Function Lista_series_lista(ByVal Id_Areadep As Integer, _
                                ByVal id_instrumento As Integer, _
                                ByRef stru_serie() As Serie_documental) As String
        Try
            Dim Parametro_Consulta = "select ID_SERIES,NOMBRE_SERIE,CONSECUTIVO_SERIE,ESTADO_DESICION FROM SERIES_DOCUMENTALES " & _
           " WHERE Areas_Depart_Radicacion_Codigo_Area=" & Id_Areadep & _
           " and Ra_registro_instrumento_archivistico_id_instrumento=" & id_instrumento
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("SERIES_DOCUMENTALES")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Lista_series_lista = "Funcion  Lista_series_lista dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Lista_series_lista = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru_serie(i)
                    stru_serie(i).Id_Series = Datset.Tables(0).Rows(i).Item(0)
                    stru_serie(i).Nombre_Serie = Datset.Tables(0).Rows(i).Item(1)
                Next
                Lista_series_lista = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_series_lista = "Inconsistencia general función Lista_series_lista " & ex.Message
        End Try

    End Function
    Function Listar_SubSeries_Documentales_lista(ByRef Stru_Serie As Serie_documental) As String
        Try
            Dim Parametro_Consulta = "select ID_SUBSERIES,NOMBRE_SUBSERIE,CONSECUTIVO_SUBSERIE,ESTADO_DESICION FROM SUBSERIES_DOCUMENTALES " & _
            " WHERE Series_Documentales_Id_Series=" & Stru_Serie.Id_Series
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("SERIES_DOCUMENTALES")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Listar_SubSeries_Documentales_lista = "Funcion  Listar_SubSeries_Documentales_lista dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Listar_SubSeries_Documentales_lista = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve Stru_Serie.matri_sub_serie(i)
                    Stru_Serie.matri_sub_serie(i).Id_SubSeries = Datset.Tables(0).Rows(i).Item(0)
                    Stru_Serie.matri_sub_serie(i).Nombre_Subserie = Datset.Tables(0).Rows(i).Item(1)
                Next
                Listar_SubSeries_Documentales_lista = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Listar_SubSeries_Documentales_lista = ex.ToString
        End Try
    End Function
    Function Listar_Tipdoc_Series_lista(ByRef Stru_Serie As Serie_documental) As String
        Try
            Dim Parametro_Consulta = "select Id_Tipo_Doc_Series,Descripcion_Documento FROM TIPO_DOC_SERIES " & _
            " WHERE Series_Documentales_Id_Series=" & Stru_Serie.Id_Series & " and sub_serie_id_serie is null"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("SERIES_DOCUMENTALES")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Listar_Tipdoc_Series_lista = "Funcion  Listar_Tipdoc_Series_lista dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Listar_Tipdoc_Series_lista = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve Stru_Serie.matri_tipo_doc_series(i)
                    Stru_Serie.matri_tipo_doc_series(i).Id_Tipo_Doc_Series = Datset.Tables(0).Rows(i).Item(0)
                    Stru_Serie.matri_tipo_doc_series(i).Descripcion_Documento = Datset.Tables(0).Rows(i).Item(1)
                Next
                Listar_Tipdoc_Series_lista = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Listar_Tipdoc_Series_lista = "Inconsistencia función Listar_Tipdoc_Series_lista " & ex.Message
        End Try
    End Function
    Function Listar_Tipdoc_Subseries_lista(ByRef Stru_Sub_Serie As subseries_documentales) As String
        Try
            Dim Parametro_Consulta = "select Id_Tipo_Doc_Series,Descripcion_Documento FROM tipo_doc_series " & _
            " WHERE sub_serie_id_serie=" & Stru_Sub_Serie.Id_SubSeries
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("SERIES_DOCUMENTALES")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Listar_Tipdoc_Subseries_lista = "Funcion  Listar_Tipdoc_Subseries_lista dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Listar_Tipdoc_Subseries_lista = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve Stru_Sub_Serie.matri_tipo_doc_series(i)
                    Stru_Sub_Serie.matri_tipo_doc_series(i).Id_Tipo_Doc_Series = Datset.Tables(0).Rows(i).Item(0)
                    Stru_Sub_Serie.matri_tipo_doc_series(i).Descripcion_Documento = Datset.Tables(0).Rows(i).Item(1)
                Next
                Listar_Tipdoc_Subseries_lista = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Listar_Tipdoc_Subseries_lista = "Inconsistencia Listar_Tipdoc_Subseries_lista " & ex.Message
        End Try
    End Function
    Function Lista_instrumentos_por_area(ByVal id_area As Integer, _
                                         ByVal id_instrumento As Integer, _
                                        ByRef stru_serie() As Serie_documental) As String

        Try
            Dim Result As String = ""
            Erase stru_serie
            Result = Me.Lista_series_lista(id_area, id_instrumento, stru_serie)
            If Result <> "YES" Then
                Lista_instrumentos_por_area = Result
                Exit Function
            End If
            If stru_serie Is Nothing Then
                Lista_instrumentos_por_area = "YES"
                Exit Function
            End If
            For i As Integer = 0 To stru_serie.Length - 1
                Result = Me.Listar_Tipdoc_Series_lista(stru_serie(i))
                If Result <> "YES" Then
                    Lista_instrumentos_por_area = Result
                    Exit Function
                End If
            Next
            For i As Integer = 0 To stru_serie.Length - 1
                Result = Me.Listar_SubSeries_Documentales_lista(stru_serie(i))
                If Result <> "YES" Then
                    Lista_instrumentos_por_area = Result
                    Exit Function
                End If
            Next
            For i As Integer = 0 To stru_serie.Length - 1
                If Not stru_serie(i).matri_sub_serie Is Nothing Then
                    For z As Integer = 0 To stru_serie(i).matri_sub_serie.Length - 1
                        Result = Me.Listar_Tipdoc_Subseries_lista(stru_serie(i).matri_sub_serie(z))
                        If Result <> "YES" Then
                            Lista_instrumentos_por_area = Result
                            Exit Function
                        End If
                    Next
                   
                End If
            Next
            HttpContext.Current.Session.Item("TR_SERIES_CACHE") = stru_serie
            Lista_instrumentos_por_area = "YES"
        Catch ex As Exception
            Lista_instrumentos_por_area = "Inconsistencia general función Lista_instrumentos_por_area " & ex.Message
        End Try

    End Function
    Function Lista_instrumento_interface_por_area(ByVal stru_serie() As Serie_documental, _
                                                  ByVal nombre_area As String, _
                                                  ByRef ref_teview As TreeView, _
                                                  ByRef updat As UpdatePanel, _
                                                  ByVal tipo_trenode As Integer) As String
        Try
            Dim Result As String = ""
            ref_teview.Nodes.Clear()
            Dim attrNode_principal As New TreeNode
            attrNode_principal.Text = nombre_area & " (Area - Departamento)"
            attrNode_principal.Value = ""
            ref_teview.Nodes.Add(attrNode_principal)
            HttpContext.Current.Session.Item("TRD_CONTADOR") = 0
            If stru_serie Is Nothing Then
                Lista_instrumento_interface_por_area = "YES"
                Exit Function
            End If
            Dim valor_() As String = {"NOMBRE SERIE", "PROCESO", "PROCEDIMIENTO", "MEDIO", "ARCHIVO_GESTION", "ARCHIVO CENTRAL", "CT", "MT", "S"}
            Dim id_tempo_nodo As String = ""
            For i As Integer = 0 To stru_serie.Length - 1
                HttpContext.Current.Session.Item("TRD_CONTADOR") = HttpContext.Current.Session.Item("TRD_CONTADOR") + 1
                id_tempo_nodo = HttpContext.Current.Session.Item("TRD_CONTADOR") & "|" & stru_serie(i).Id_Series & "|" & "1" '& "|" & stru_serie(i).Nombre_Serie
                Dim valor_tr() As String = {id_tempo_nodo, stru_serie(i).Nombre_Serie, "NA", "NA", "NA", "NA", "NA", "NA", "NA", "NA"}
                Dim attrNode_serie As Object = Nothing
                If tipo_trenode = 1 Then
                    attrNode_serie = New TreeNode
                    attrNode_serie.Value = id_tempo_nodo
                    attrNode_serie.text = stru_serie(i).Nombre_Serie
                End If
                If tipo_trenode = 2 Then
                    attrNode_serie = CreateNode_table("", "table_tre", valor_, valor_tr)
                    attrNode_serie.Value = id_tempo_nodo
                    attrNode_serie.SelectAction = TreeNodeSelectAction.None
                End If
                Result = Me.Agrega_tipos_documentales_serie_treview(stru_serie(i), attrNode_serie, tipo_trenode)
                If Result <> "YES" Then
                    Lista_instrumento_interface_por_area = Result
                    Exit Function
                End If
                Result = Me.Agrega_sub_series_documentales(stru_serie(i), attrNode_serie, tipo_trenode)
                If Result <> "YES" Then
                    Lista_instrumento_interface_por_area = Result
                    Exit Function
                End If
                attrNode_principal.ChildNodes.Add(attrNode_serie)
            Next
            Lista_instrumento_interface_por_area = "YES"
        Catch ex As Exception
            Lista_instrumento_interface_por_area = "Inconsistencia general función Lista_instrumento_interface_por_area " & ex.Message
        Finally
            updat.Update()
        End Try
    End Function
    Function Agrega_tipos_documentales_serie_treview(stru_serie As Serie_documental, _
                                                     ByRef tred_nod As TreeNode, _
                                                     ByVal tipo_trenode As Integer) As String
        Try
            If stru_serie.matri_tipo_doc_series Is Nothing Then
                Agrega_tipos_documentales_serie_treview = "YES"
                Exit Function
            End If
            For i As Integer = 0 To stru_serie.matri_tipo_doc_series.Length - 1
                HttpContext.Current.Session.Item("TRD_CONTADOR") = HttpContext.Current.Session.Item("TRD_CONTADOR") + 1
                Dim id_tempo_nodo = HttpContext.Current.Session.Item("TRD_CONTADOR") & "|" & stru_serie.matri_tipo_doc_series(i).Id_Tipo_Doc_Series & "|" & "3" '& "|" & stru_serie.matri_tipo_doc_series(i).Descripcion_Documento
                Dim valor() As String = {"TIPO DOCUMENTO SERIE"}
                Dim valor_() As String = {id_tempo_nodo, stru_serie.matri_tipo_doc_series(i).Descripcion_Documento}
                Dim attrNode_tipos As Object = Nothing
                If tipo_trenode = 2 Then
                    attrNode_tipos = CreateNode_table("", "table_tre", valor, valor_)
                    attrNode_tipos.Text = stru_serie.matri_tipo_doc_series(i).Descripcion_Documento
                    attrNode_tipos.Value = id_tempo_nodo
                    attrNode_tipos.SelectAction = TreeNodeSelectAction.None
                End If
                If tipo_trenode = 1 Then
                    attrNode_tipos = New TreeNode
                    attrNode_tipos.Value = id_tempo_nodo
                    attrNode_tipos.text = stru_serie.matri_tipo_doc_series(i).Descripcion_Documento
                    attrNode_tipos.ImageUrl = "../workflow/imageneswf/lista_tipo_documento.png"
                End If
                tred_nod.ChildNodes.Add(attrNode_tipos)
            Next
            Agrega_tipos_documentales_serie_treview = "YES"
        Catch ex As Exception
            Agrega_tipos_documentales_serie_treview = "Inconsistencia general función Agrega_tipos_documentales_serie_treview " & ex.Message
        End Try
    End Function
    Function Agrega_sub_series_documentales(stru_serie As Serie_documental, _
                                            ByRef tred_nod As TreeNode, _
                                            ByVal tipo_trenode As Integer) As String
        Try
            Dim Result As String = ""
            If stru_serie.matri_sub_serie Is Nothing Then
                Agrega_sub_series_documentales = "YES"
                Exit Function
            End If
            For i As Integer = 0 To stru_serie.matri_sub_serie.Length - 1
                HttpContext.Current.Session.Item("TRD_CONTADOR") = HttpContext.Current.Session.Item("TRD_CONTADOR") + 1
                Dim id_tempo_nodo As String = HttpContext.Current.Session.Item("TRD_CONTADOR") & "|" & stru_serie.matri_sub_serie(i).Id_SubSeries & "|" & "2" '& "|" & stru_serie.matri_sub_serie(i).Nombre_Subserie
                Dim valor() As String = {"NOMBRE SUB SERIE", "PROCESO", "PROCEDIMIENTO", "MEDIO", "ARCHIVO_GESTION", "ARCHIVO CENTRAL", "CT", "MT", "S"}
                Dim valor_() As String = {id_tempo_nodo, stru_serie.matri_sub_serie(i).Nombre_Subserie, "NA", "NA", "NA", "NA", "NA", "NA", "NA", "NA"}
                Dim attrNode_sub_series As Object = Nothing
                If tipo_trenode = 2 Then
                    attrNode_sub_series = CreateNode_table("", "table_tre", valor, valor_)
                    attrNode_sub_series.Value = id_tempo_nodo
                    attrNode_sub_series.SelectAction = TreeNodeSelectAction.None
                End If
                If tipo_trenode = 1 Then
                    Dim c As TreeNode
                    attrNode_sub_series = New TreeNode
                    attrNode_sub_series.Value = id_tempo_nodo
                    attrNode_sub_series.text = stru_serie.matri_sub_serie(i).Nombre_Subserie
                    attrNode_sub_series.ImageUrl = "../workflow/imageneswf/lista_sub_serie.png"
                End If
                Result = Me.Agrega_tipo_documento_sub_series_documentales(stru_serie.matri_sub_serie(i), attrNode_sub_series, tipo_trenode)
                If Result <> "YES" Then
                    Agrega_sub_series_documentales = Result
                    Exit Function
                End If
                tred_nod.ChildNodes.Add(attrNode_sub_series)
            Next
            Agrega_sub_series_documentales = "YES"
        Catch ex As Exception
            Agrega_sub_series_documentales = "Inconsistencia general función Agrega_sub_series_documentales " & ex.Message
        End Try
    End Function
    Function Agrega_tipo_documento_sub_series_documentales(stru_serie As subseries_documentales, _
                                                           ByRef tred_nod As TreeNode, _
                                                           ByVal tipo_trenode As Integer) As String
        Try
            If stru_serie.matri_tipo_doc_series Is Nothing Then
                Agrega_tipo_documento_sub_series_documentales = "YES"
                Exit Function
            End If
            For i As Integer = 0 To stru_serie.matri_tipo_doc_series.Length - 1
                HttpContext.Current.Session.Item("TRD_CONTADOR") = HttpContext.Current.Session.Item("TRD_CONTADOR") + 1
                Dim id_tempo_nodo As String = HttpContext.Current.Session.Item("TRD_CONTADOR") & "|" & stru_serie.matri_tipo_doc_series(i).Id_Tipo_Doc_Series & "|" & "4" '& "|" & stru_serie.matri_tipo_doc_series(i).Descripcion_Documento
                Dim valor() As String = {"TIPO DOCUMENTO"}
                Dim valor_() As String = {stru_serie.matri_tipo_doc_series(i).Descripcion_Documento}
                Dim attrNode_tipos As Object = Nothing
                If tipo_trenode = 2 Then
                    attrNode_tipos = CreateNode_table("", "table_tre", valor, valor_)
                End If
                If tipo_trenode = 1 Then
                    attrNode_tipos = New TreeNode
                    attrNode_tipos.Value = id_tempo_nodo
                    attrNode_tipos.Text = stru_serie.matri_tipo_doc_series(i).Descripcion_Documento
                    attrNode_tipos.ImageUrl = "../workflow/imageneswf/lista_tipo_documento.png"
                End If

                If tipo_trenode = 2 Then
                    attrNode_tipos.SelectAction = TreeNodeSelectAction.None
                End If
                tred_nod.ChildNodes.Add(attrNode_tipos)
            Next
            Agrega_tipo_documento_sub_series_documentales = "YES"
        Catch ex As Exception
            Agrega_tipo_documento_sub_series_documentales = "Inconsistencia general función Agrega_tipo_documento_sub_series_documentales " & ex.Message
        End Try
    End Function
    Function CreateNode(ByVal NodeText As String, ByVal NodeStyle As String) As CustomTreeNode
        Dim oNode As CustomTreeNode = New CustomTreeNode()
        oNode.Text = NodeText
        oNode.cssClass = NodeStyle
        Return oNode
    End Function
    Function CreateNode_table(ByVal NodeText As String, _
                              ByVal NodeStyle As String, _
                              ByVal valore() As String, _
                              ByVal valore_tr() As String) As CustomTreeNodeTable
        Dim oNode As CustomTreeNodeTable = New CustomTreeNodeTable()
        'oNode.Text = NodeText
        oNode.cssClass = NodeStyle
        oNode.valores_table = valore
        oNode.valores_table_tr = valore_tr
        Return oNode
    End Function
    Function NodoChild_Selecioa_nodo(ByRef Tre_vie As TreeView, _
                                   ByVal Datos_Nodo As String, _
                       ByRef trenode As TreeNode) As String
        Try
            Dim Result As String = ""
            trenode = Nothing
            If Tre_vie.Nodes.Count > 0 Then
                For i = 0 To Tre_vie.Nodes.Count - 1
                    trenode = Nod_CHILD(Tre_vie.Nodes(i), Datos_Nodo)
                    If Not trenode Is Nothing Then
                        NodoChild_Selecioa_nodo = "YES"
                        Return NodoChild_Selecioa_nodo
                    End If
                Next
            End If
            NodoChild_Selecioa_nodo = "YES"
        Catch ex As Exception
            NodoChild_Selecioa_nodo = ex.Message
        End Try
    End Function
    Function Nod_CHILD(ByVal NodeC As TreeNode, ByVal Datos_Nodo As String) As Object
        Try
            Nod_CHILD = Nothing
            Dim i As Integer = 0
            For i = 0 To NodeC.ChildNodes.Count - 1
                If NodeC.ChildNodes(i).Value = Datos_Nodo Then
                    'NodeC.ChildNodes(i).Selected = True
                    Nod_CHILD = NodeC.ChildNodes(i)
                    Return Nod_CHILD
                    Exit Function
                End If
                Nod_CHILD(NodeC.ChildNodes(i), Datos_Nodo)
            Next
            Return Nod_CHILD
        Catch ex As Exception
            Nod_CHILD = ex.Message
        End Try

    End Function
    Function RecorrerTreeView(ByRef Nodos As TreeNodeCollection _
                              , ByVal Datos_Nodo As String)
        Try
            For Each Nodo As TreeNode In Nodos
                If Nodo.ChildNodes.Count = 0 Then
                    If Nodo.Value = Datos_Nodo Then
                        Nodo.Selected = True
                        RecorrerTreeView = "YES"
                        Exit For
                    End If
                Else
                    If Nodo.Value = Datos_Nodo Then
                        Nodo.Selected = True
                        RecorrerTreeView = "YES"
                        Exit For
                    End If
                    RecorrerTreeView_aplica_style(Nodo.ChildNodes, Datos_Nodo)
                End If
            Next
            RecorrerTreeView = "YES"
        Catch ex As Exception
            RecorrerTreeView = "Inconsistecia general " & ex.Message
        End Try
    End Function
    Function RecorrerTreeView_aplica_style(ByRef Nodos As TreeNodeCollection _
                              , ByVal Datos_Nodo As String)
        Try
            Dim Result As String = ""
            For Each Nodo As TreeNode In Nodos
                If Nodo.ChildNodes.Count = 0 Then
                    Nodo = CType(Nodo, CustomTreeNodeTable)
                    Result = Aplica_style_nodes_recursive(Nodo, Datos_Nodo)
                    If Result <> "YES" Then
                        RecorrerTreeView_aplica_style = Result
                        Exit Function
                    End If
                Else
                    Nodo = CType(Nodo, CustomTreeNodeTable)
                    Result = Aplica_style_nodes_recursive(Nodo, Datos_Nodo)
                    If Result <> "YES" Then
                        RecorrerTreeView_aplica_style = Result
                        Exit Function
                    End If
                    'RecorrerTreeView_aplica_style(Nodo.ChildNodes, Datos_Nodo)
                End If
            Next
            RecorrerTreeView_aplica_style = "YES"
        Catch ex As Exception
            RecorrerTreeView_aplica_style = "Inconsistecia general " & ex.Message
        End Try
    End Function
    Function Aplica_style_nodes_recursive(ByRef NodeC As Object, _
                                          ByVal Datos_Nodo As String) As String
        Try

            Dim Result As String = ""
            Dim splinode_value() As String = NodeC.Value.ToString.Split("|")
            If Not splinode_value Is Nothing Then
                If splinode_value(2) = 1 Then
                    Dim stru_serie As Serie_documental = Nothing
                    Result = Me.Retorna_valores_nodo_estructura(HttpContext.Current.Session.Item("TR_SERIES_CACHE"), _
                                                              Val(splinode_value(1)), Val(splinode_value(2)), stru_serie)
                    If Result <> "YES" Then
                        Aplica_style_nodes_recursive = Result
                        Exit Function
                    End If
                    Dim valor_() As String = {"NOMBRE SERIE", "PROCESO", "PROCEDIMIENTO", "MEDIO", "ARCHIVO_GESTION", "ARCHIVO CENTRAL", "CT", "MT", "S"}
                    Dim valor_tr() As String = {Val(splinode_value(1)), stru_serie.Nombre_Serie, "NA", "NA", "NA", "NA", "NA", "NA", "NA", "NA"}

                    NodeC.valores_table_tr = valor_tr
                End If
                If splinode_value(2) = 2 Then
                    Dim stru_serie As subseries_documentales = Nothing
                    Result = Me.Retorna_valores_nodo_estructura(HttpContext.Current.Session.Item("TR_SERIES_CACHE"), _
                                                              Val(splinode_value(1)), Val(splinode_value(2)), stru_serie)
                    If Result <> "YES" Then
                        Aplica_style_nodes_recursive = Result
                        Exit Function
                    End If
                    Dim valor_() As String = {"NOMBRE SERIE", "PROCESO", "PROCEDIMIENTO", "MEDIO", "ARCHIVO_GESTION", "ARCHIVO CENTRAL", "CT", "MT", "S"}
                    Dim valor_tr() As String = {Val(splinode_value(1)), stru_serie.Nombre_Subserie, "NA", "NA", "NA", "NA", "NA", "NA", "NA", "NA"}
                    NodeC.valores_table = valor_
                    NodeC.valores_table_tr = valor_tr
                End If
            End If
            Aplica_style_nodes_recursive = "YES"
        Catch ex As Exception
            Aplica_style_nodes_recursive = ex.Message
        End Try
    End Function
    Function Retorna_valores_nodo_estructura(ByVal stru_serie() As Serie_documental, _
                                             ByVal id_ As Integer, _
                                             ByVal id_tipo As Integer, _
                                             ByRef stru_objet As Object) As String
        Try
            If id_tipo = 1 Then
                For i As Integer = 0 To stru_serie.Length - 1
                    If stru_serie(i).Id_Series = id_ Then
                        stru_objet = stru_serie(i)
                        Retorna_valores_nodo_estructura = "YES"
                        Exit Function
                    End If
                Next
            End If
            If id_tipo = 2 Then
                For i As Integer = 0 To stru_serie.Length - 1
                    If Not stru_serie(i).matri_sub_serie Is Nothing Then
                        For z As Integer = 0 To stru_serie(i).matri_sub_serie.Length - 1
                            If stru_serie(i).matri_sub_serie(z).Id_SubSeries = id_ Then
                                stru_objet = stru_serie(i).matri_sub_serie(z)
                                Retorna_valores_nodo_estructura = "YES"
                                Exit Function
                            End If
                        Next
                    End If

                Next
            End If
            If id_tipo = 3 Then
                For i As Integer = 0 To stru_serie.Length - 1
                    If Not stru_serie(i).matri_tipo_doc_series Is Nothing Then
                        For z As Integer = 0 To stru_serie(i).matri_tipo_doc_series.Length - 1
                            If stru_serie(i).matri_tipo_doc_series(z).Id_Tipo_Doc_Series = id_ Then
                                stru_objet = stru_serie(i).matri_tipo_doc_series(z)
                                Retorna_valores_nodo_estructura = "YES"
                                Exit Function
                            End If
                        Next
                    End If

                Next
            End If
            If id_tipo = 4 Then
                For i As Integer = 0 To stru_serie.Length - 1
                    If Not stru_serie(i).matri_sub_serie Is Nothing Then
                        For z As Integer = 0 To stru_serie(i).matri_sub_serie.Length - 1
                            If stru_serie(i).matri_sub_serie(z).matri_tipo_doc_series Is Nothing Then
                                For k As Integer = 0 To stru_serie(i).matri_sub_serie(z).matri_tipo_doc_series.Length - 1
                                    If stru_serie(i).matri_sub_serie(z).matri_tipo_doc_series(k).Id_Tipo_Doc_Series = id_ Then
                                        stru_objet = stru_serie(i).matri_sub_serie(z).matri_tipo_doc_series(k)
                                        Retorna_valores_nodo_estructura = "YES"
                                        Exit Function
                                    End If
                                Next
                            End If
                        Next
                    End If

                Next
            End If
            Retorna_valores_nodo_estructura = "YES"
        Catch ex As Exception
            Retorna_valores_nodo_estructura = "Inconsistencia general función Retorna_valores_nodo_estructura " & ex.Message
        End Try
    End Function
    Function Verifica_existencia_nombre_serie_documental_en_area(ByVal nombre_serie As String, _
                                                                ByVal id_area As Integer, _
                                                                ByVal id_instrumento As Integer, _
                                                                ByRef existencia As String) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("series_documentales")
            Dim sql_consulta As String = "Select Id_Series from series_documentales " & _
                " where Ra_registro_instrumento_archivistico_id_instrumento=" & id_instrumento & _
                " and Areas_Depart_Radicacion_Codigo_Area=" & id_area & _
                " and Nombre_Serie='" & nombre_serie & "'"
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(sql_consulta, Datset)
            If Result <> "YES" Then
                Verifica_existencia_nombre_serie_documental_en_area = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                existencia = "NO"
                Verifica_existencia_nombre_serie_documental_en_area = "YES"
                Exit Function
            Else
                existencia = "YES"
                Verifica_existencia_nombre_serie_documental_en_area = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_existencia_nombre_serie_documental_en_area = "Inconsistencia general función Verifica_existencia_nombre_serie_documental_en_area " & ex.Message
        End Try
    End Function
    Function Verifica_existencia_codigo_serie_area_departamento(ByVal codigo_serie As String, _
                                                                ByVal id_area As Integer, _
                                                                ByVal id_instrumento As Integer, _
                                                                ByRef existencia As String) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("series_documentales")
            Dim sql_consulta As String = "Select Id_Series from series_documentales " & _
                " where Ra_registro_instrumento_archivistico_id_instrumento=" & id_instrumento & _
                " and Areas_Depart_Radicacion_Codigo_Area=" & id_area & _
                " and Codigo_Arbitrario='" & codigo_serie & "'"
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(sql_consulta, Datset)
            If Result <> "YES" Then
                Verifica_existencia_codigo_serie_area_departamento = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                existencia = "NO"
                Verifica_existencia_codigo_serie_area_departamento = "YES"
                Exit Function
            Else
                existencia = "YES"
                Verifica_existencia_codigo_serie_area_departamento = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_existencia_codigo_serie_area_departamento = "Inconsistencia general función Verifica_existencia_codigo_serie_area_departamento " & ex.Message
        End Try
    End Function
    Function Agregar_serie_documental(ByVal id_instrumento As Integer, _
                                      ByVal id_area_departamento As Integer, _
                                      ByVal nombre_serie As String, _
                                      ByVal observaciones As String, _
                                      ByVal proceso_serie As String, _
                                      ByVal procedimiento As String, _
                                      ByVal codigo_serie As String, _
                                      ByVal medio As String, _
                                      ByVal estado_desicion As Integer, _
                                      ByVal tiempo_gestion As Integer, _
                                      ByVal tiempo_central As Integer, _
                                      ByVal conservacion_total As Integer, _
                                      ByVal eliminacion As Integer, _
                                      ByVal digitalizacion As Integer, _
                                      ByVal public_serie As Integer, _
                                      ByVal seleccion As Integer, _
                                      ByRef trenode As TreeNode, _
                                      ByRef update As UpdatePanel) As String

        If nombre_serie = "" Then
            Agregar_serie_documental = "Debe informar el nombre de la serie "
            Exit Function
        End If
        Dim existencia As String = ""
        Dim Result As String = ""
        Dim id_tipo_instrumento As Integer = 0
        Dim Ref_class_gestion_instrumento As New ClassGaGestionInstrumento
        Dim Ref_class_registro_instrumento_archivistico As New Class_ra_registro_instrumento_archivistico
        Result = Ref_class_registro_instrumento_archivistico.Retorna_id_tipo_instrumento(id_instrumento, _
                                                                                         id_tipo_instrumento)
        If Result <> "YES" Then
            Agregar_serie_documental = Result
            Exit Function
        End If
        Result = Me.Verifica_existencia_nombre_serie_documental_en_area(UCase(nombre_serie), _
                                                                        id_area_departamento, _
                                                                        id_instrumento, _
                                                                        existencia)
        If Result <> "YES" Then
            Agregar_serie_documental = Result
            Exit Function
        End If
        If existencia = "YES" Then
            Agregar_serie_documental = "Ya se ecnuentra registrada una serie con el nombre " & nombre_serie
            Exit Function
        End If
        If codigo_serie <> "" Then
            Result = Me.Verifica_existencia_codigo_serie_area_departamento(codigo_serie, _
                                                                           id_area_departamento, _
                                                                           id_instrumento, _
                                                                           existencia)
            If Result <> "YES" Then
                Agregar_serie_documental = Result
                Exit Function
            End If
            If existencia = "YES" Then
                Agregar_serie_documental = "Ya se ecnuentra registrada una serie con el código " & codigo_serie
                Exit Function
            End If
        End If
        If estado_desicion = 1 And id_tipo_instrumento = 1 Then
            If tiempo_gestion = 0 Then
                Agregar_serie_documental = "Debe informar el tiempo en archivo gestión  "
                Exit Function
            End If
            'If tiempo_central = 0 Then
            '    Agregar_serie_documental = "Debe informar el tiempo en archivo central  "
            '    Exit Function
            'End If
            If medio = "" Then
                Agregar_serie_documental = "Debe informar el medio de la serie  "
                Exit Function
            End If
            If conservacion_total = 0 And eliminacion = 0 _
                Then
                Agregar_serie_documental = "Debe seleccionar la disposición final  "
                Exit Function
            End If
        End If
        If estado_desicion = 1 And id_tipo_instrumento = 2 Then
            tiempo_gestion = 0
            If tiempo_central = 0 Then
                Agregar_serie_documental = "Debe informar el tiempo en archivo central  "
                Exit Function
            End If
            If medio = "" Then
                Agregar_serie_documental = "Debe informar el medio de la serie  "
                Exit Function
            End If
            If conservacion_total = 0 And eliminacion = 0 _
                Then
                Agregar_serie_documental = "Debe seleccionar la disposición final  "
                Exit Function
            End If
        End If
        Dim ref_observaciones As String = "Null"
        If observaciones <> "" Then
            ref_observaciones = "'" & observaciones & "'"
        End If
        Dim ref_proceso_serie As String = "Null"
        If proceso_serie <> "" Then
            ref_proceso_serie = "'" & proceso_serie & "'"
        End If
        Dim ref_procedimiento As String = "Null"
        If procedimiento <> "" Then
            ref_procedimiento = "'" & procedimiento & "'"
        End If
        Dim ref_medio As String = "Null"
        If medio <> "" Then
            ref_medio = "'" & medio & "'"
        End If

        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_DA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim mySqldatReader As MySqlDataReader
        Dim sqlresultinsert As Integer = 0
        Agregar_serie_documental = ""
        Try
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            Dim Parametro_Select_System1 As String = " SELECT Consecutivo_Serie FROM areas_depart_radicacion " & _
            " where Codigo_Area=" & id_area_departamento & " for update"
            myCommand.CommandText = Parametro_Select_System1
            mySqldatReader = myCommand.ExecuteReader()
            If mySqldatReader Is Nothing Then
                Agregar_serie_documental = "Imposible encontrar el registro del consecutivo código serie del área " & id_area_departamento & " error de conexión"
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            If mySqldatReader.HasRows = False Then
                Agregar_serie_documental = "Imposible encontrar el registro del consecutivo código serie del área " & id_area_departamento
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            Dim consecutivo_serie As Integer = 0
            mySqldatReader.Read()
            consecutivo_serie = mySqldatReader.Item(0)
            consecutivo_serie = consecutivo_serie + 1
            mySqldatReader.Close()
            Dim update_consecutivo_serie As String = "Update  areas_depart_radicacion set Consecutivo_Serie=" & consecutivo_serie & _
            " where Codigo_Area=" & id_area_departamento
            myCommand.CommandText = update_consecutivo_serie
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Agregar_serie_documental = "Imposible actualizar el consecutivo de la serie en el área "
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            Dim SqlInsert As String = "Insert Into Series_Documentales (Areas_Depart_Radicacion_Codigo_Area," & _
           "Consecutivo_Serie,Nombre_Serie,Estado_Serie,Consecutivo_subserie,Consecutivo_Tip_Doc," & _
           "Tiempo_Ret_Arch_Central,Tiempo_Ret_Arch_Gestion,Conservacion_Total,Eliminacion," & _
           "Microfilm,Seleccion,OBSERVACIONES,Tiempo_Ret_Archivo_historico,ESTADO_DESICION,Estado_Publico_Serie," & _
           "Proceso,Procedimiento,Medio_soporte,Codigo_Arbitrario,Ra_registro_instrumento_archivistico_id_instrumento) values "
            If codigo_serie = "" Then
                codigo_serie = consecutivo_serie.ToString
            End If
            Dim sql_values As String = "(" & id_area_departamento & "," & "0,'" & UCase(nombre_serie) & "',1,0,0," & tiempo_central & "," & tiempo_gestion & _
                "," & conservacion_total & "," & eliminacion & "," & digitalizacion & "," & seleccion & "," & ref_observaciones & "," & "0," & estado_desicion & _
                  "," & public_serie & "," & ref_proceso_serie & "," & ref_procedimiento & "," & ref_medio & ",'" & codigo_serie & "'," & _
                   id_instrumento & ")"
            myCommand.CommandText = SqlInsert & sql_values
            sqlresultinsert = myCommand.ExecuteNonQuery
            If sqlresultinsert = 0 Then
                Agregar_serie_documental = "Imposible registrar la serie documental "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            Dim id_serie As Object = myCommand.LastInsertedId
            HttpContext.Current.Session.Item("TRD_CONTADOR") = HttpContext.Current.Session.Item("TRD_CONTADOR") + 1
            Dim id_tempo_nodo = HttpContext.Current.Session.Item("TRD_CONTADOR") & "|" & id_serie & "|" & "1" '& "|" & UCase(nombre_serie)
            Dim attrNode_serie As New TreeNode
            attrNode_serie.Value = id_tempo_nodo
            attrNode_serie.Text = UCase(nombre_serie)
            trenode.ChildNodes.Add(attrNode_serie)
            update.Update()
            myTrans.Commit()
            myConnection.Close()
            Agregar_serie_documental = "YES"
        Catch ex As Exception
            If myConnection.State = ConnectionState.Open Then
                myTrans.Rollback()
                myConnection.Close()
                Agregar_serie_documental = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally
            If myConnection.State = ConnectionState.Open Then
                myConnection.Close()
            End If
            Agregar_serie_documental = Agregar_serie_documental
        End Try
    End Function
    Function Activa_editar_serie_documental(ByVal id_serie As Integer, _
                                            ByRef pag As Page) As String

        Try
            Dim nombre_serie As TextBox = pag.FindControl("TextBox_nombre_serie")
            Dim observaciones As TextBox = pag.FindControl("TextBox_observaciones_serie")
            Dim proceso_serie As TextBox = pag.FindControl("TextBoxProceso")
            Dim procedimiento As TextBox = pag.FindControl("TextBoxProcedimiento")
            Dim codigo_serie As TextBox = pag.FindControl("TextBoxCodigoSerie")
            Dim medio As DropDownList = pag.FindControl("DropDownListMedio")
            Dim estado_desicion As CheckBox = pag.FindControl("CheckBoxDiposicion")
            Dim tiempo_gestion As DropDownList = pag.FindControl("DropDownList_tiempo_retencion_gestion")
            Dim tiempo_central As DropDownList = pag.FindControl("DropDownList_tiempo_retencion_central")
            Dim conservacion_total As CheckBox = pag.FindControl("CheckBoxConservTotal")
            Dim eliminacion As CheckBox = pag.FindControl("CheckBoxSerieEliminacion")
            Dim digitalizacion As CheckBox = pag.FindControl("CheckBoxSerieDigitalizacion")
            Dim publi_serie As CheckBox = pag.FindControl("CheckBox_public_serie")
            Dim seleccion As CheckBox = pag.FindControl("CheckBoxSerieSeleccion")
            Dim update As UpdatePanel = pag.FindControl("UpdatePanel_agregar_serie")
            Dim Label_title_agregar_serie As Label = pag.FindControl("Label_title_agregar_serie")
            Dim UpdatePanel_title_agregar_serie As UpdatePanel = pag.FindControl("UpdatePanel_title_agregar_serie")
            Dim ModalPopupExtender_agregar_serie As AjaxControlToolkit.ModalPopupExtender = pag.FindControl("ModalPopupExtender_agregar_serie")
            Dim stru_serie As stru_serie_documental = Nothing
            Dim Result As String = ""
            Result = Me.Solicita_estructura_serie_documental(id_serie, stru_serie)
            If Result <> "YES" Then
                Activa_editar_serie_documental = Result
                Exit Function
            End If
            Result = Me.Asigna_datos_intrface_serie_documental(stru_serie, _
                                                                nombre_serie, _
                                                                observaciones, _
                                                                proceso_serie, _
                                                                procedimiento, _
                                                                codigo_serie, _
                                                                medio, _
                                                                estado_desicion, _
                                                                tiempo_gestion, _
                                                                tiempo_central, _
                                                                conservacion_total, _
                                                                eliminacion, _
                                                                digitalizacion, _
                                                                publi_serie, _
                                                                seleccion, _
                                                                update)
            If Result <> "YES" Then
                Activa_editar_serie_documental = Result
                Exit Function
            End If
            Label_title_agregar_serie.Text = "Editar serie documental"
            UpdatePanel_title_agregar_serie.Update()
            update.Update()
            ModalPopupExtender_agregar_serie.Show()
            Activa_editar_serie_documental = "YES"
            Exit Function
        Catch ex As Exception
            Activa_editar_serie_documental = "Inconsistencia general función Activa_editar_serie_documental " & ex.Message
        End Try
    End Function
    Function Solicita_estructura_serie_documental(ByVal id_serie As Integer, _
                                                  ByRef stru_serie As stru_serie_documental) As String
        Try
            Dim Parametro_Consulta = "select Areas_Depart_Radicacion_Codigo_Area,Consecutivo_Serie,Nombre_Serie," & _
            "Estado_Serie,Consecutivo_subserie,Consecutivo_Tip_Doc,Tiempo_Ret_Arch_Central,Tiempo_Ret_Archivo_Historico," & _
            "Conservacion_Total,Eliminacion,Microfilm,Seleccion,observaciones,Tiempo_Ret_Arch_Gestion,ESTADO_DESICION," & _
            "Estado_Publico_Serie,Proceso,Procedimiento,Medio_soporte,Codigo_Arbitrario,Ra_registro_instrumento_archivistico_id_instrumento " & _
            "from series_documentales" & _
          " WHERE Id_Series=" & id_serie
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("SERIES_DOCUMENTALES")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_estructura_serie_documental = "Funcion  Solicita_estructura_serie_documental dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_estructura_serie_documental = "Imposible encontrar los datos de la serie documental"
                Exit Function
            Else
                stru_serie.Areas_Depart_Radicacion_Codigo_Area = Datset.Tables(0).Rows(0).Item(0)
                stru_serie.Consecutivo_Serie = Datset.Tables(0).Rows(0).Item(1)
                stru_serie.Nombre_Serie = Datset.Tables(0).Rows(0).Item(2)
                stru_serie.Estado_Serie = Datset.Tables(0).Rows(0).Item(3)
                stru_serie.Consecutivo_subserie = Datset.Tables(0).Rows(0).Item(4)
                stru_serie.Consecutivo_Tip_Doc = Datset.Tables(0).Rows(0).Item(5)
                stru_serie.Tiempo_Ret_Arch_Central = Datset.Tables(0).Rows(0).Item(6)
                stru_serie.Tiempo_Ret_Archivo_Historico = Datset.Tables(0).Rows(0).Item(7)
                stru_serie.Conservacion_Total = Datset.Tables(0).Rows(0).Item(8)
                stru_serie.Eliminacion = Datset.Tables(0).Rows(0).Item(9)
                stru_serie.Microfilm = Datset.Tables(0).Rows(0).Item(10)
                stru_serie.Seleccion = Datset.Tables(0).Rows(0).Item(11)
                If Datset.Tables(0).Rows(0).IsNull(12) Then
                    stru_serie.observaciones = ""
                Else
                    stru_serie.observaciones = Datset.Tables(0).Rows(0).Item(12)
                End If
                stru_serie.Tiempo_Ret_Arch_Gestion = Datset.Tables(0).Rows(0).Item(13)
                stru_serie.ESTADO_DESICION = Datset.Tables(0).Rows(0).Item(14)
                stru_serie.Estado_Publico_Serie = Datset.Tables(0).Rows(0).Item(15)
                If Datset.Tables(0).Rows(0).IsNull(16) Then
                    stru_serie.Proceso = ""
                Else
                    stru_serie.Proceso = Datset.Tables(0).Rows(0).Item(16)
                End If
                If Datset.Tables(0).Rows(0).IsNull(17) Then
                    stru_serie.Procedimiento = ""
                Else
                    stru_serie.Procedimiento = Datset.Tables(0).Rows(0).Item(17)
                End If
                If Datset.Tables(0).Rows(0).IsNull(18) Then
                    stru_serie.Medio_soporte = ""
                Else
                    stru_serie.Medio_soporte = Datset.Tables(0).Rows(0).Item(18)
                End If
                If Datset.Tables(0).Rows(0).IsNull(19) Then
                    stru_serie.Codigo_Arbitrario = ""
                Else
                    stru_serie.Codigo_Arbitrario = Datset.Tables(0).Rows(0).Item(19)
                End If
                If Datset.Tables(0).Rows(0).IsNull(20) Then
                    stru_serie.Ra_registro_instrumento_archivistico_id_instrumento = 0
                Else
                    stru_serie.Ra_registro_instrumento_archivistico_id_instrumento = Datset.Tables(0).Rows(0).Item(20)
                End If
                Solicita_estructura_serie_documental = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estructura_serie_documental = "Inconsistencia general función Solicita_estructura_serie_documental " & ex.Message
        End Try
    End Function
    Function Asigna_datos_intrface_serie_documental(ByVal stru_serie As stru_serie_documental, _
                                                     ByRef nombre_serie As TextBox, _
                                                     ByRef observaciones As TextBox, _
                                                     ByRef proceso_serie As TextBox, _
                                                     ByRef procedimiento As TextBox, _
                                                     ByRef codigo_serie As TextBox, _
                                                     ByRef medio As DropDownList, _
                                                     ByRef estado_desicion As CheckBox, _
                                                     ByRef tiempo_gestion As DropDownList, _
                                                     ByRef tiempo_central As DropDownList, _
                                                     ByRef conservacion_total As CheckBox, _
                                                     ByRef eliminacion As CheckBox, _
                                                     ByRef digitalizacion As CheckBox, _
                                                     ByRef public_serie As CheckBox, _
                                                     ByRef seleccion As CheckBox, _
                                                     ByRef update As UpdatePanel) As String
        Try
            nombre_serie.Text = stru_serie.Nombre_Serie
            observaciones.Text = stru_serie.observaciones
            proceso_serie.Text = stru_serie.Proceso
            procedimiento.Text = stru_serie.Procedimiento
            codigo_serie.Text = stru_serie.Codigo_Arbitrario
            medio.Items.Clear()
            medio.Items.Add("")
            medio.Items.Add("Físico")
            medio.Items.Add("Digital")
            medio.Items.Add("Físico-Digital")
            For i As Integer = 0 To medio.Items.Count - 1
                If medio.Items(i).Text = stru_serie.Medio_soporte Then
                    medio.SelectedValue = medio.Items(i).Text
                    Exit For
                End If
            Next
            If stru_serie.ESTADO_DESICION = 1 Then
                estado_desicion.Checked = True
            Else
                estado_desicion.Checked = False
            End If
            tiempo_gestion.Items.Clear()
            tiempo_central.Items.Clear()
            For i As Integer = 0 To 100
                tiempo_gestion.Items.Add(i)
                tiempo_central.Items.Add(i)
            Next
            For i As Integer = 0 To tiempo_gestion.Items.Count - 1
                If tiempo_gestion.Items(i).Text = stru_serie.Tiempo_Ret_Arch_Gestion Then
                    tiempo_gestion.SelectedValue = tiempo_gestion.Items(i).Text
                    Exit For
                End If
            Next
            For i As Integer = 0 To tiempo_central.Items.Count - 1
                If tiempo_central.Items(i).Text = stru_serie.Tiempo_Ret_Arch_Central Then
                    tiempo_central.SelectedValue = tiempo_central.Items(i).Text
                    Exit For
                End If
            Next
            If stru_serie.Conservacion_Total = 1 Then
                conservacion_total.Checked = True
            Else
                conservacion_total.Checked = False
            End If
            If stru_serie.Eliminacion = 1 Then
                eliminacion.Checked = True
            Else
                eliminacion.Checked = False
            End If
            If stru_serie.Microfilm = 1 Then
                digitalizacion.Checked = True
            Else
                digitalizacion.Checked = False
            End If
            If stru_serie.Estado_Publico_Serie = 1 Then
                public_serie.Checked = True
            Else
                public_serie.Checked = False
            End If
            If stru_serie.Seleccion = 1 Then
                seleccion.Checked = True
            Else
                seleccion.Checked = False
            End If
            Asigna_datos_intrface_serie_documental = "YES"
            Exit Function
        Catch ex As Exception
            Asigna_datos_intrface_serie_documental = "Inconsistencia general función Asigna_datos_intrface_serie_documental " & ex.Message
        End Try
    End Function
    Function Actualiza_serie_documental(ByVal id_instrumento As Integer, _
                                      ByVal id_serie As Integer, _
                                      ByVal id_area_departamento As Integer, _
                                      ByVal nombre_serie As String, _
                                      ByVal observaciones As String, _
                                      ByVal proceso_serie As String, _
                                      ByVal procedimiento As String, _
                                      ByVal codigo_serie As String, _
                                      ByVal medio As String, _
                                      ByVal estado_desicion As Integer, _
                                      ByVal tiempo_gestion As Integer, _
                                      ByVal tiempo_central As Integer, _
                                      ByVal conservacion_total As Integer, _
                                      ByVal eliminacion As Integer, _
                                      ByVal digitalizacion As Integer, _
                                      ByVal public_serie As Integer, _
                                      ByVal seleccion As Integer, _
                                      ByRef treview As TreeView, _
                                      ByRef update As UpdatePanel) As String
        Dim sql_actualiza_nombre_serie_exp As String = ""
        Dim sql_actualiza_nombre_serie_uni As String = ""
        If nombre_serie = "" Then
            Actualiza_serie_documental = "Debe informar el nombre de la serie "
            Exit Function
        End If
        If codigo_serie = "" Then
            Actualiza_serie_documental = "Debe informar el código de la serie "
            Exit Function
        End If
        Dim id_tipo_instrumento As Integer = 0
        Dim Ref_class_gestion_instrumento As New ClassGaGestionInstrumento
        Dim Ref_class_registro_instrumento_archivistico As New Class_ra_registro_instrumento_archivistico
        Dim Result As String = Ref_class_registro_instrumento_archivistico.Retorna_id_tipo_instrumento(id_instrumento, _
                                                                                                       id_tipo_instrumento)
        If Result <> "YES" Then
            Actualiza_serie_documental = Result
            Exit Function
        End If
        If estado_desicion = 1 And id_tipo_instrumento = 1 Then
            If tiempo_gestion = 0 Then
                Actualiza_serie_documental = "Debe informar el tiempo en archivo gestión  "
                Exit Function
            End If
            'If tiempo_central = 0 Then
            '    Actualiza_serie_documental = "Debe informar el tiempo en archivo central  "
            '    Exit Function
            'End If
            If medio = "" Then
                Actualiza_serie_documental = "Debe informar el medio de la serie  "
                Exit Function
            End If
            If conservacion_total = 0 And eliminacion = 0 _
                 Then
                Actualiza_serie_documental = "Debe seleccionar la disposición final  "
                Exit Function
            End If
        End If
        If estado_desicion = 1 And id_tipo_instrumento = 2 Then
            If tiempo_central = 0 Then
                Actualiza_serie_documental = "Debe informar el tiempo en archivo central  "
                Exit Function
            End If
            If medio = "" Then
                Actualiza_serie_documental = "Debe informar el medio de la serie  "
                Exit Function
            End If
            If conservacion_total = 0 And eliminacion = 0 _
                 Then
                Actualiza_serie_documental = "Debe seleccionar la disposición final  "
                Exit Function
            End If
        End If
        Dim confirm As Boolean = True
        Dim Cambios As String = ""
        Dim existencia As String = ""
        Dim update_registro As String = "Update series_documentales "
        Dim stru_serie As stru_serie_documental = Nothing
        Dim sql_actualiza_nombre_serie_producion_documental As String = ""
        Result = Me.Solicita_estructura_serie_documental(id_serie, stru_serie)
        If Result <> "YES" Then
            Actualiza_serie_documental = Result
            Exit Function
        End If
        If UCase(nombre_serie) <> stru_serie.Nombre_Serie Then
            Result = Me.Verifica_existencia_nombre_serie_documental_en_area(UCase(nombre_serie), id_area_departamento, _
                                                                           id_instrumento, existencia)
            If Result <> "YES" Then
                Actualiza_serie_documental = Result
                Exit Function
            End If
            If existencia = "YES" Then
                Actualiza_serie_documental = "Ya se ecnuentra registrada una serie con el nombre " & nombre_serie
                Exit Function
            End If
            Cambios = Cambios & " Cambio nombre serie documental " & stru_serie.Nombre_Serie & " Nuevo valor " & UCase(nombre_serie)
            If update_registro = "Update series_documentales " Then
                update_registro = update_registro & " set Nombre_Serie='" & UCase(nombre_serie) & "'"
            Else
                update_registro = update_registro & " , Nombre_Serie='" & UCase(nombre_serie) & "'"
            End If
            Result = Me.verifica_existencia_serie_relacionda_expediente(id_serie, existencia)
            If Result <> "YES" Then
                Actualiza_serie_documental = Result
                Exit Function
            End If
            If existencia = "YES" Then
                sql_actualiza_nombre_serie_exp = "Update expediente_archivo set NOMBRE_SERIE_TRD='" & nombre_serie & "'" & _
                    " where CODIGO_SERIE_TRD=" & id_serie
                Cambios = Cambios & " (*Cambio de nombres de series en expedientes relacionados*) "
            End If
            Result = Me.verifica_existencia_serie_relacionada_unidad_conservacion(id_serie, existencia)
            If Result <> "YES" Then
                Actualiza_serie_documental = Result
                Exit Function
            End If
            If existencia = "YES" Then
                sql_actualiza_nombre_serie_uni = "Update unidad_conservacion set NOMBRE_SERIE='" & nombre_serie & "'" &
                    " where CODIGO_SERIE=" & id_serie
                Cambios = Cambios & " (*Cambio de nombres de series en unidades de conservación relacionadas*) "
            End If
            Dim ClassGaTipoDocumental As New ClassGaProducionDocumental
            existencia = ""
            Result = ClassGaTipoDocumental.Solicita_existencia_serie_produccion_documental(id_serie,
                                                                                               existencia)
            If Result <> "YES" Then
                Actualiza_serie_documental = Result
                Exit Function
            End If
            If existencia = "YES" Then
                sql_actualiza_nombre_serie_producion_documental = "update registro_producion_documental set SERIE_DOCUMENTO='" & nombre_serie & "'" &
                    " where ID_SERIE_DOCUMENTO=" & id_serie
                Cambios = Cambios & " (*Cambio de nombres de series en documentos relacionados en la producción documental*) "
            End If
        End If
        If observaciones <> stru_serie.observaciones Then
            Cambios = Cambios & " Cambio observaciones " & stru_serie.observaciones & " Nuevo valor " & observaciones
            If update_registro = "Update series_documentales " Then
                update_registro = update_registro & " set observaciones='" & observaciones & "'"
            Else
                update_registro = update_registro & " , observaciones='" & observaciones & "'"
            End If
        End If
        If proceso_serie <> stru_serie.Proceso Then
            Cambios = Cambios & " Cambio proceso " & stru_serie.Proceso & " Nuevo valor " & proceso_serie
            If update_registro = "Update series_documentales " Then
                update_registro = update_registro & " set Proceso='" & proceso_serie & "'"
            Else
                update_registro = update_registro & " , Proceso='" & proceso_serie & "'"
            End If
        End If
        If procedimiento <> stru_serie.Procedimiento Then
            Cambios = Cambios & " Cambio procedimiento " & stru_serie.Procedimiento & " Nuevo valor " & procedimiento
            If update_registro = "Update series_documentales " Then
                update_registro = update_registro & " set Procedimiento='" & procedimiento & "'"
            Else
                update_registro = update_registro & " , Procedimiento='" & procedimiento & "'"
            End If
        End If
        If codigo_serie <> stru_serie.Codigo_Arbitrario Then
            Result = Me.Verifica_existencia_codigo_serie_area_departamento(codigo_serie, id_area_departamento, _
                                                                          id_instrumento, existencia)
            If Result <> "YES" Then
                Actualiza_serie_documental = Result
                Exit Function
            End If
            If existencia = "YES" Then
                Actualiza_serie_documental = "Ya se ecnuentra registrada una serie con el código " & codigo_serie
                Exit Function
            End If
            Cambios = Cambios & " Cambio código serie " & stru_serie.Codigo_Arbitrario & " Nuevo valor " & codigo_serie
            If update_registro = "Update series_documentales " Then
                update_registro = update_registro & " set Codigo_Arbitrario='" & codigo_serie & "'"
            Else
                update_registro = update_registro & " , Codigo_Arbitrario='" & codigo_serie & "'"
            End If
        End If
        If public_serie <> stru_serie.Estado_Publico_Serie Then
            Cambios = Cambios & " Cambio estado público serie " & stru_serie.Estado_Publico_Serie & " Nuevo valor " & public_serie
            If update_registro = "Update series_documentales " Then
                update_registro = update_registro & " set Estado_Publico_Serie='" & public_serie & "'"
            Else
                update_registro = update_registro & " , Estado_Publico_Serie='" & public_serie & "'"
            End If
        End If
        If estado_desicion <> stru_serie.ESTADO_DESICION Then
            If estado_desicion = 1 Then
                If tiempo_gestion = 0 Then
                    Actualiza_serie_documental = "Debe informar el tiempo en archivo gestión  "
                    Exit Function
                End If
                If tiempo_central = 0 Then
                    Actualiza_serie_documental = "Debe informar el tiempo en archivo central  "
                    Exit Function
                End If
                If medio = "" Then
                    Actualiza_serie_documental = "Debe informar el medio de la serie  "
                    Exit Function
                End If
                If conservacion_total = 0 And eliminacion = 0 _
                    And digitalizacion = 0 And seleccion = 0 Then
                    Actualiza_serie_documental = "Debe seleccionar por lo menos un item de disposición final  "
                    Exit Function
                End If
            End If
            Cambios = Cambios & " Cambio estado de disposición final " & stru_serie.ESTADO_DESICION & " Nuevo valor " & estado_desicion
            If update_registro = "Update series_documentales " Then
                update_registro = update_registro & " set ESTADO_DESICION='" & estado_desicion & "'"
            Else
                update_registro = update_registro & " , ESTADO_DESICION='" & estado_desicion & "'"
            End If
        End If
        If tiempo_gestion <> stru_serie.Tiempo_Ret_Arch_Gestion Then
            Cambios = Cambios & " Cambio tiempo archivo de gestión " & stru_serie.Tiempo_Ret_Arch_Gestion & " Nuevo valor " & tiempo_gestion
            If update_registro = "Update series_documentales " Then
                update_registro = update_registro & " set Tiempo_Ret_Arch_Gestion='" & tiempo_gestion & "'"
            Else
                update_registro = update_registro & " , Tiempo_Ret_Arch_Gestion='" & tiempo_gestion & "'"
            End If
        End If
        If tiempo_central <> stru_serie.Tiempo_Ret_Arch_Central Then
            Cambios = Cambios & " Cambio tiempo de archivo central  " & stru_serie.Tiempo_Ret_Arch_Central & " Nuevo valor " & tiempo_central
            If update_registro = "Update series_documentales " Then
                update_registro = update_registro & " set Tiempo_Ret_Arch_Central='" & tiempo_central & "'"
            Else
                update_registro = update_registro & " , Tiempo_Ret_Arch_Central='" & tiempo_central & "'"
            End If
        End If
        If medio <> stru_serie.Medio_soporte Then
            Cambios = Cambios & " Cambio de medio  " & stru_serie.Medio_soporte & " Nuevo valor " & medio
            If update_registro = "Update series_documentales " Then
                update_registro = update_registro & " set Medio_soporte='" & medio & "'"
            Else
                update_registro = update_registro & " , Medio_soporte='" & medio & "'"
            End If
        End If
        If conservacion_total <> stru_serie.Conservacion_Total Then
            Cambios = Cambios & " Cambio estado conservación total  " & stru_serie.Conservacion_Total & " Nuevo valor " & conservacion_total
            If update_registro = "Update series_documentales " Then
                update_registro = update_registro & " set Conservacion_Total='" & conservacion_total & "'"
            Else
                update_registro = update_registro & " , Conservacion_Total='" & conservacion_total & "'"
            End If
        End If
        If eliminacion <> stru_serie.Eliminacion Then
            Cambios = Cambios & " Cambio estado eliminación  " & stru_serie.Eliminacion & " Nuevo valor " & eliminacion
            If update_registro = "Update series_documentales " Then
                update_registro = update_registro & " set Eliminacion='" & eliminacion & "'"
            Else
                update_registro = update_registro & " , Eliminacion='" & eliminacion & "'"
            End If
        End If
        If digitalizacion <> stru_serie.Microfilm Then
            Cambios = Cambios & " Cambio estado tecnológico  " & stru_serie.Microfilm & " Nuevo valor " & digitalizacion
            If update_registro = "Update series_documentales " Then
                update_registro = update_registro & " set Microfilm='" & digitalizacion & "'"
            Else
                update_registro = update_registro & " , Microfilm='" & digitalizacion & "'"
            End If
        End If
        If seleccion <> stru_serie.Seleccion Then
            Cambios = Cambios & " Cambio estado selección  " & stru_serie.Seleccion & " Nuevo valor " & seleccion
            If update_registro = "Update series_documentales " Then
                update_registro = update_registro & " set Seleccion='" & seleccion & "'"
            Else
                update_registro = update_registro & " , Seleccion='" & seleccion & "'"
            End If
        End If
        If update_registro = "Update series_documentales " Then
            Actualiza_serie_documental = "No se detectaron cambios para actualizar en la serie"
            Exit Function
        Else
            update_registro = update_registro & " where Id_Series=" & id_serie
        End If
        If update_registro = "Update series_documentales " Then
            Actualiza_serie_documental = "YES"
            Exit Function
        End If
        Dim date_time As String = ""
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Result = ref_ClassGestionFechas.Retorna_fecha_registro(date_time)
        If Result <> "YES" Then
            Actualiza_serie_documental = Result
            Exit Function
        End If
        Dim iphost As String = HttpContext.Current.Session.Item("ip_host_name")
        Dim id_user As String = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
        Dim logi_user As String = HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION")
        Dim hor As String = Now
        Dim detalle_trans As String = "EDITA SERIE"
        Dim campos_trans As String = ""
        Dim isert_datos As String = ""
        campos_trans = "EDITA SERIE " & id_serie & "-" & stru_serie.Nombre_Serie & "  (" & _
        " EDITA SERIE " & Cambios & ")"
        isert_datos = isert_datos & "(" & id_instrumento & _
            ",'" & detalle_trans & "','" & logi_user & "','" & id_user & "','" & date_time & "'," & _
                    "'" & iphost & "','GESTOR-WEB','" & campos_trans & "')"

        Dim update_gestion = "INSERT INTO ra_log_instrumentos_archivisticos (Ra_registro_instrumento_archivistico_id_instrumento,desc_op,USER_OPER,ID_USER,DATE_TRANS" & _
                                             ",IP_TRANS,MODULO_REGISTRO,CAMPOS) values " & _
                                             isert_datos
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Actualiza_serie_documental = ""
        Try
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = update_registro
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Actualiza_serie_documental = "Imposible registrar cambios en la serie"
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myCommand.CommandText = update_gestion
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Actualiza_serie_documental = "Imposible registrar log del instrumento"
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            If sql_actualiza_nombre_serie_exp <> "" Then
                myCommand.CommandText = sql_actualiza_nombre_serie_exp
                Switc = myCommand.ExecuteNonQuery()
                If Switc = 0 Then
                    Actualiza_serie_documental = "Imposible actualizar nombres de serie en expedientes relacionados"
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            If sql_actualiza_nombre_serie_uni <> "" Then
                myCommand.CommandText = sql_actualiza_nombre_serie_uni
                Switc = myCommand.ExecuteNonQuery()
                If Switc = 0 Then
                    Actualiza_serie_documental = "Imposible actualizar nombres de  serie en unidades de conservación relacionadas"
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            If sql_actualiza_nombre_serie_producion_documental <> "" Then
                myCommand.CommandText = sql_actualiza_nombre_serie_producion_documental
                Switc = myCommand.ExecuteNonQuery()
                If Switc = 0 Then
                    Actualiza_serie_documental = "Imposible actualizar la  serie en la producion"
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            If UCase(nombre_serie) <> stru_serie.Nombre_Serie Then
                If Not treview.SelectedNode Is Nothing Then
                    treview.SelectedNode.Text = UCase(nombre_serie)
                    update.Update()
                End If
            End If
            myTrans.Commit()
            Actualiza_serie_documental = "YES"
            Exit Function
        Catch ex As MySqlException
            If myConnection.State = ConnectionState.Open Then
                myTrans.Rollback()
                myConnection.Close()
                Actualiza_serie_documental = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function


            End If
        Finally
            If myConnection.State = ConnectionState.Open Then
                myConnection.Close()
            End If
            Actualiza_serie_documental = Actualiza_serie_documental
        End Try
    End Function
    Function Eliminar_serie_documental(ByVal id_serie As Integer, _
                                       ByVal id_instrumento As Integer, _
                                       ByRef treview As TreeView, _
                                       ByRef update As UpdatePanel) As String

        Dim Result As String = ""
        Dim Existencia As String = ""
        Result = Me.Verifica_Existencia_Subserie_Doc(id_serie, Existencia)
        If Result <> "YES" Then
            Eliminar_serie_documental = Result
            Exit Function
        End If
        If Existencia = "YES" Then
            Eliminar_serie_documental = "La serie tiene sub series relacionadas imposible eliminar"
            Exit Function
        End If
        Result = Me.Verifica_Tipdocment_TipDoc_en_serie(id_serie, Existencia)
        If Result <> "YES" Then
            Eliminar_serie_documental = Result
            Exit Function
        End If
        If Existencia = "YES" Then
            Eliminar_serie_documental = "La serie tiene tipos documentales relacionadas imposible eliminar"
            Exit Function
        End If
        Result = Me.verifica_existencia_serie_relacionda_expediente(id_serie, Existencia)
        If Result <> "YES" Then
            Eliminar_serie_documental = Result
            Exit Function
        End If
        If Existencia = "YES" Then
            Eliminar_serie_documental = "La serie tiene expedientes relacionadas imposible eliminar"
            Exit Function
        End If
        Result = Me.verifica_existencia_serie_relacionada_unidad_conservacion(id_serie, Existencia)
        If Result <> "YES" Then
            Eliminar_serie_documental = Result
            Exit Function
        End If
        If Existencia = "YES" Then
            Eliminar_serie_documental = "La serie tiene unidades de conservación relacionadas imposible eliminar"
            Exit Function
        End If
        Dim date_time As String = ""
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Result = ref_ClassGestionFechas.Retorna_fecha_registro(date_time)
        If Result <> "YES" Then
            Eliminar_serie_documental = Result
            Exit Function
        End If
        Dim iphost As String = HttpContext.Current.Session.Item("ip_host_name")
        Dim id_user As String = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
        Dim logi_user As String = HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION")
        Dim hor As String = Now
        Dim detalle_trans As String = "ELIMINA SERIE"
        Dim campos_trans As String = ""
        Dim isert_datos As String = ""
        campos_trans = "ELIMINA SERIE " & id_serie & "-" & treview.SelectedNode.Text & "  (" & _
        " ELIMINA SERIE " & ")"
        isert_datos = isert_datos & "(" & id_instrumento & _
            ",'" & detalle_trans & "','" & logi_user & "','" & id_user & "','" & date_time & "'," & _
                    "'" & iphost & "','GESTOR-WEB','" & campos_trans & "')"
        Dim update_gestion = "INSERT INTO ra_log_instrumentos_archivisticos (Ra_registro_instrumento_archivistico_id_instrumento,desc_op,USER_OPER,ID_USER,DATE_TRANS" & _
                                             ",IP_TRANS,MODULO_REGISTRO,CAMPOS) values " & _
                                             isert_datos
        Dim elimna_registro As String = "delete from series_documentales where Id_Series=" & id_serie
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Eliminar_serie_documental = ""
        Try
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = elimna_registro
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Eliminar_serie_documental = "Imposible eliminar la serie"
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myCommand.CommandText = update_gestion
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Eliminar_serie_documental = "Imposible registrar log del instrumento"
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            If Not treview.SelectedNode Is Nothing Then
                treview.Nodes.Remove(treview.SelectedNode)
                Dim sNodo As TreeNode = treview.SelectedNode
                Dim pNodo As TreeNode = sNodo.Parent
                pNodo.ChildNodes.Remove(sNodo)
                update.Update()
            End If
            myTrans.Commit()
            Eliminar_serie_documental = "YES"
            Exit Function
        Catch ex As MySqlException
            If myConnection.State = ConnectionState.Open Then
                myTrans.Rollback()
                myConnection.Close()
                Eliminar_serie_documental = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally
            If myConnection.State = ConnectionState.Open Then
                myConnection.Close()
            End If
            Eliminar_serie_documental = Eliminar_serie_documental
        End Try
    End Function
    Function Verifica_Existencia_Subserie_Doc(ByVal Id_Serie As Integer, _
                                        ByRef Existencia As String) As String
        Try
            Dim Parametro_Consulta As String = "Select * " & _
            " from subseries_documentales where Series_Documentales_id_Series=" & Id_Serie
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("SERIES_DOCUMENTALES")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verifica_Existencia_Subserie_Doc = "Función  Verifica_Existencia_Subserie_Doc dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Existencia = "NO"
                Verifica_Existencia_Subserie_Doc = "YES"
                Exit Function
            Else
                Existencia = "YES"
                Verifica_Existencia_Subserie_Doc = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_Existencia_Subserie_Doc = "Inconsistencia general función Verifica_Existencia_Subserie_Doc " & ex.Message
        End Try
    End Function
    Function Verifica_Tipdocment_TipDoc_en_serie(ByVal Id_Serie As Integer, _
                                                 ByRef Existencia As String) As String
        Try
            Dim Parametro_Consulta As String = "Select * " & _
            " from tipo_doc_series where Series_Documentales_Id_Series=" & Id_Serie
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipo_doc_series")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verifica_Tipdocment_TipDoc_en_serie = "Función  Verifica_Tipdocment_TipDoc_en_serie dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Existencia = "NO"
                Verifica_Tipdocment_TipDoc_en_serie = "YES"
                Exit Function
            Else
                Existencia = "YES"
                Verifica_Tipdocment_TipDoc_en_serie = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_Tipdocment_TipDoc_en_serie = "Inconsistencia general función tipo_doc_series " & ex.Message
        End Try
    End Function
    Function Solicita_estado_serie_documental(ByVal id_serie As Integer, _
                                              ByRef estado_serie As Integer) As String
        Try
            Dim Parametro_Consulta As String = "Select Estado_Serie " & _
            " from series_documentales where Id_Series=" & id_serie
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("SERIES_DOCUMENTALES")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_estado_serie_documental = "Función  Solicita_estado_serie_documental dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_estado_serie_documental = "Imposible encontrar el estado de la serie documental " & id_serie
                Exit Function
            Else
                estado_serie = Datset.Tables(0).Rows(0).Item(0)
                Solicita_estado_serie_documental = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estado_serie_documental = "Inconsistencia general función Solicita_estado_serie_documental " & ex.Message
        End Try
    End Function
    Function Asigna_cambio_estado_elemento(ByVal estado_elemento As Integer, _
                                              ByRef Check_activa_instrumento As CheckBox, _
                                              ByRef CheckBox_inactiva_instrumento As CheckBox, _
                                              ByRef ModalPopupExtender_activar_inactivar As  _
                                              AjaxControlToolkit.ModalPopupExtender, _
                                              ByRef UpdatePanel_activar_inactivar As UpdatePanel) As String
        '---------------------------------------------------------------
        'Función : Asigna cambio de estado del elemento seleccionado
        'a la interface
        'Fecha : 2018-06-29
        'Ing : Miguel Angel Urueta Miranda
        '---------------------------------------------------------------
        Try

            If estado_elemento = 1 Then
                Check_activa_instrumento.Checked = True
                CheckBox_inactiva_instrumento.Checked = False
            Else
                Check_activa_instrumento.Checked = False
                CheckBox_inactiva_instrumento.Checked = True
            End If
            UpdatePanel_activar_inactivar.Update()
            ModalPopupExtender_activar_inactivar.Show()
            Asigna_cambio_estado_elemento = "YES"
            Exit Function

        Catch ex As Exception
            Asigna_cambio_estado_elemento = "Inconsistencia general función Asigna_cambio_estado_elemento " & ex.Message
        End Try
    End Function
    Function Cambia_estado_serie_documental(ByVal id_serie As Integer, _
                                            ByVal estado_serie As Integer, _
                                            ByVal nombre_serie As String, _
                                            ByVal id_instrumento As Integer) As String

        Dim Result As String = ""
        Dim estado_actual As Integer = 0
        Result = Me.Solicita_estado_serie_documental(id_serie, estado_actual)
        If Result <> "YES" Then
            Cambia_estado_serie_documental = Result
            Exit Function
        End If
        If estado_actual = estado_serie Then
            Cambia_estado_serie_documental = "YES"
            Exit Function
        End If
        Dim date_time As String = ""
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Result = ref_ClassGestionFechas.Retorna_fecha_registro(date_time)
        If Result <> "YES" Then
            Cambia_estado_serie_documental = Result
            Exit Function
        End If
        Dim iphost As String = HttpContext.Current.Session.Item("ip_host_name")
        Dim id_user As String = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
        Dim logi_user As String = HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION")
        Dim hor As String = Now
        Dim detalle_trans As String = "CAMBIA ESTADO SERIE"
        Dim campos_trans As String = ""
        Dim isert_datos As String = ""
        campos_trans = "CAMBIA ESTADO SERIE " & id_serie & "-" & nombre_serie & "  (" & _
        " CAMBIA ESTADO SERIE " & estado_actual & " Nuevo Valor " & estado_serie & ")"
        isert_datos = isert_datos & "(" & id_instrumento & _
            ",'" & detalle_trans & "','" & logi_user & "','" & id_user & "','" & date_time & "'," & _
                    "'" & iphost & "','GESTOR-WEB','" & campos_trans & "')"
        Dim update_gestion = "INSERT INTO ra_log_instrumentos_archivisticos (Ra_registro_instrumento_archivistico_id_instrumento,desc_op,USER_OPER,ID_USER,DATE_TRANS" & _
                                             ",IP_TRANS,MODULO_REGISTRO,CAMPOS) values " & _
                                             isert_datos
        Dim elimna_registro As String = "Update series_documentales set Estado_Serie=" & estado_serie & " where Id_Series=" & id_serie
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Cambia_estado_serie_documental = ""
        Try
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = elimna_registro
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Cambia_estado_serie_documental = "Imposible eliminar la serie"
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myCommand.CommandText = update_gestion
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Cambia_estado_serie_documental = "Imposible registrar log del instrumento"
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If

            myTrans.Commit()
            Cambia_estado_serie_documental = "YES"
            Exit Function
        Catch ex As MySqlException
            If myConnection.State = ConnectionState.Open Then
                myTrans.Rollback()
                myConnection.Close()
                Cambia_estado_serie_documental = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally
            If myConnection.State = ConnectionState.Open Then
                myConnection.Close()
            End If
            Cambia_estado_serie_documental = Cambia_estado_serie_documental
        End Try
    End Function
    Function Agregar_tipo_documental_a_serie(ByVal id_serie As Integer, _
                                             ByVal id_instrumento As Integer, _
                                             ByVal nombre_documento As String, _
                                             ByVal ruta_documento As String, _
                                             ByVal codigo_documento As String, _
                                             ByRef Trenod As TreeNode, _
                                             ByRef update As UpdatePanel, _
                                             ByVal chek As CheckBox) As String

        Dim Existencia As String = ""
        Dim Result As String = ""
        If nombre_documento = "" Then
            Agregar_tipo_documental_a_serie = "Debe informar el nombre del documento "
            Exit Function
        End If
        Result = Me.Formato_sub_serie(nombre_documento, _
                                      nombre_documento)
        If Result <> "YES" Then
            Agregar_tipo_documental_a_serie = Result
            Exit Function
        End If
        Result = Me.Verifica_existencia_documento_en_serie(id_serie, nombre_documento, Existencia)
        If Result <> "YES" Then
            Agregar_tipo_documental_a_serie = Result
            Exit Function
        End If
        If Existencia = "YES" Then
            Agregar_tipo_documental_a_serie = "Esta intentado agregar un tipo de documento que existe en la serie"
            Exit Function
        End If
        Dim date_time As String = ""
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Result = ref_ClassGestionFechas.Retorna_fecha_registro(date_time)
        If Result <> "YES" Then
            Agregar_tipo_documental_a_serie = Result
            Exit Function
        End If
        Dim ref_ruta_documento As String = "Null"
        If ruta_documento <> "" Then
            ref_ruta_documento = "'" & ruta_documento & "'"
        End If
        Dim Ref_codigo_documento As String = "Null"
        If codigo_documento <> "" Then
            Ref_codigo_documento = "'" & codigo_documento & "'"
        End If
        Dim estado_trasversal As Integer = 0
        If chek.Checked = True Then
            estado_trasversal = 1
        End If
        Dim elimna_registro As String = "insert into tipo_doc_series (Series_Documentales_Id_Series,Consecutivo_Tip_Doc,Descripcion_Documento," & _
            "Fecha_Creacion,Estado_Tipo,PLANTILLA,id_instrumento,codigo_documento,tipo_doc_trasversal) values (" & _
            id_serie & ",1,'" & nombre_documento & "','" & date_time & "',1," & ref_ruta_documento & "," _
            & id_instrumento & "," & Ref_codigo_documento & "," & estado_trasversal & ")"
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Agregar_tipo_documental_a_serie = ""
        Try
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = elimna_registro
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Agregar_tipo_documental_a_serie = "Imposible registrar el documento a la serie"
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            HttpContext.Current.Session.Item("TRD_CONTADOR") = HttpContext.Current.Session.Item("TRD_CONTADOR") + 1
            Dim id_tempo_nodo = HttpContext.Current.Session.Item("TRD_CONTADOR") & "|" & myCommand.LastInsertedId & "|" & "3" '& "|" & nombre_documento
            Dim attrNode_tipos As TreeNode = New TreeNode
            attrNode_tipos.Value = id_tempo_nodo
            attrNode_tipos.Text = nombre_documento
            attrNode_tipos.ImageUrl = "../workflow/imageneswf/lista_tipo_documento.png"
            Trenod.ChildNodes.Add(attrNode_tipos)
            update.Update()
            myTrans.Commit()
            Agregar_tipo_documental_a_serie = "YES"
            Exit Function
        Catch ex As MySqlException
            If myConnection.State = ConnectionState.Open Then
                myTrans.Rollback()
                myConnection.Close()
                Agregar_tipo_documental_a_serie = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally
            If myConnection.State = ConnectionState.Open Then
                myConnection.Close()
            End If
            Agregar_tipo_documental_a_serie = Agregar_tipo_documental_a_serie
        End Try
    End Function
    Function Verifica_existencia_documento_en_serie(ByVal id_serie As Integer, _
                                                    ByVal nombre_documento As String, _
                                                    ByRef Existencia As String) As String
        Try
            Dim Parametro_Consulta As String = "Select * " & _
           " from tipo_doc_series where Series_Documentales_Id_Series=" & id_serie & _
           " and Descripcion_Documento='" & nombre_documento & "'"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipo_doc_series")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verifica_existencia_documento_en_serie = "Función  Verifica_existencia_documento_en_serie dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Existencia = "NO"
                Verifica_existencia_documento_en_serie = "YES"
                Exit Function
            Else
                Existencia = "YES"
                Verifica_existencia_documento_en_serie = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_existencia_documento_en_serie = "Inconsistencia general función Verifica_existencia_documento_en_serie " & ex.Message
        End Try
    End Function
    Function Verifica_Existencia_nombre_en_sub_serie(ByVal Id_serie As Integer, _
                                              ByVal Nombre_Serie As String, _
                                              ByRef Existencia As String) As String

        Try
            Dim Parametro_Consulta As String = "Select * " & _
            " from subseries_documentales where Series_Documentales_id_Series=" & Id_serie & _
            " and Nombre_subSerie='" & Nombre_Serie & "'"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("subseries_documentales")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verifica_Existencia_nombre_en_sub_serie = "Función  Verifica_Existencia_nombre_en_sub_serie dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Existencia = "NO"
                Verifica_Existencia_nombre_en_sub_serie = "YES"
                Exit Function
            Else
                Existencia = "YES"
                Verifica_Existencia_nombre_en_sub_serie = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_Existencia_nombre_en_sub_serie = "Inconsistencia general función Verifica_Existencia_nombre_en_sub_serie " & ex.Message
        End Try
    End Function
    Function Verfica_existecia_codigo_sub_serie_en_la_serie(ByVal Id_serie As Integer, _
                                              ByVal codigo_serie As String, _
                                              ByRef Existencia As String) As String
        Try
            Dim Parametro_Consulta As String = "Select * " & _
            " from subseries_documentales where Series_Documentales_id_Series=" & Id_serie & _
            " and Codigo_Arbitrario='" & codigo_serie & "'"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("subseries_documentales")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verfica_existecia_codigo_sub_serie_en_la_serie = "Función  Verfica_existecia_codigo_sub_serie_en_la_serie dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Existencia = "NO"
                Verfica_existecia_codigo_sub_serie_en_la_serie = "YES"
                Exit Function
            Else
                Existencia = "YES"
                Verfica_existecia_codigo_sub_serie_en_la_serie = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verfica_existecia_codigo_sub_serie_en_la_serie = "Inconsistencia general función Verfica_existecia_codigo_sub_serie_en_la_serie " & ex.Message
        End Try
    End Function
    Function Agregar_sub_serie_documental(ByVal id_instrumento As Integer, _
                                      ByVal id_serie As Integer, _
                                      ByVal nombre_sub_serie As String, _
                                      ByVal observaciones As String, _
                                      ByVal proceso_sub_serie As String, _
                                      ByVal procedimiento As String, _
                                      ByVal codigo_sub_serie As String, _
                                      ByVal medio As String, _
                                      ByVal estado_desicion As Integer, _
                                      ByVal tiempo_gestion As Integer, _
                                      ByVal tiempo_central As Integer, _
                                      ByVal conservacion_total As Integer, _
                                      ByVal eliminacion As Integer, _
                                      ByVal digitalizacion As Integer, _
                                      ByVal public_sub_serie As Integer, _
                                      ByVal seleccion As Integer, _
                                      ByRef trenode As TreeNode, _
                                      ByRef update As UpdatePanel) As String

        If nombre_sub_serie = "" Then
            Agregar_sub_serie_documental = "Debe informar el nombre de la sub serie "
            Exit Function
        End If
        Dim existencia As String = ""
        Dim Result As String = ""
        Result = Me.Formato_sub_serie(nombre_sub_serie, nombre_sub_serie)
        If Result <> "YES" Then
            Agregar_sub_serie_documental = Result
            Exit Function
        End If
        Result = Me.Verifica_Existencia_nombre_en_sub_serie(id_serie, _
                                                            nombre_sub_serie, _
                                                            existencia)
        If Result <> "YES" Then
            Agregar_sub_serie_documental = Result
            Exit Function
        End If
        If existencia = "YES" Then
            Agregar_sub_serie_documental = "Ya se ecnuentra registrada una sub serie con el nombre " & nombre_sub_serie
            Exit Function
        End If

        If codigo_sub_serie <> "" Then
            Result = Me.Verfica_existecia_codigo_sub_serie_en_la_serie(id_serie, codigo_sub_serie, _
                                                                       existencia)
            If Result <> "YES" Then
                Agregar_sub_serie_documental = Result
                Exit Function
            End If
            If existencia = "YES" Then
                Agregar_sub_serie_documental = "Ya se ecnuentra registrada una sub serie con el código " & codigo_sub_serie
                Exit Function
            End If
        End If
        Dim id_tipo_instrumento As Integer = 0
        Dim Ref_class_gestion_instrumento As New ClassGaGestionInstrumento
        Dim Ref_class_registro_instrumento_archivistico As New Class_ra_registro_instrumento_archivistico
        Result = Ref_class_registro_instrumento_archivistico.Retorna_id_tipo_instrumento(id_instrumento, _
                                                                                         id_tipo_instrumento)
        If Result <> "YES" Then
            Agregar_sub_serie_documental = Result
            Exit Function
        End If
        If estado_desicion = 0 Then
            Agregar_sub_serie_documental = "Debe marcar la disposición final de la sub serie  "
            Exit Function
        End If
        If estado_desicion = 1 And id_tipo_instrumento = 1 Then
            If tiempo_gestion = 0 Then
                Agregar_sub_serie_documental = "Debe informar el tiempo en archivo gestión  "
                Exit Function
            End If
            'If tiempo_central = 0 Then
            '    Agregar_sub_serie_documental = "Debe informar el tiempo en archivo central  "
            '    Exit Function
            'End If
            If medio = "" Then
                Agregar_sub_serie_documental = "Debe informar el medio de la serie  "
                Exit Function
            End If
            If conservacion_total = 0 And eliminacion = 0 _
                Then
                Agregar_sub_serie_documental = "Debe seleccionar la disposición final  "
                Exit Function
            End If
        End If
        If estado_desicion = 1 And id_tipo_instrumento = 2 Then
            If tiempo_central = 0 Then
                Agregar_sub_serie_documental = "Debe informar el tiempo en archivo central  "
                Exit Function
            End If
            If medio = "" Then
                Agregar_sub_serie_documental = "Debe informar el medio de la serie  "
                Exit Function
            End If
            If conservacion_total = 0 And eliminacion = 0 _
                Then
                Agregar_sub_serie_documental = "Debe seleccionar la disposición final  "
                Exit Function
            End If
        End If
        Dim ref_observaciones As String = "Null"
        If observaciones <> "" Then
            ref_observaciones = "'" & observaciones & "'"
        End If
        Dim ref_proceso_sub_serie As String = "Null"
        If proceso_sub_serie <> "" Then
            ref_proceso_sub_serie = "'" & proceso_sub_serie & "'"
        End If
        Dim ref_procedimiento As String = "Null"
        If procedimiento <> "" Then
            ref_procedimiento = "'" & procedimiento & "'"
        End If
        Dim ref_medio As String = "Null"
        If medio <> "" Then
            ref_medio = "'" & medio & "'"
        End If
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_DA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim mySqldatReader As MySqlDataReader
        Dim sqlresultinsert As Integer = 0
        Agregar_sub_serie_documental = ""
        Try
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            Dim Parametro_Select_System1 As String = " SELECT Consecutivo_subserie FROM series_documentales " & _
            " where Id_Series=" & id_serie & " for update"
            myCommand.CommandText = Parametro_Select_System1
            mySqldatReader = myCommand.ExecuteReader()
            If mySqldatReader Is Nothing Then
                Agregar_sub_serie_documental = "Imposible encontrar el registro del consecutivo de serie de la serie " & id_serie & " error de conexión"
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            If mySqldatReader.HasRows = False Then
                Agregar_sub_serie_documental = "Imposible encontrar el registro del consecutivo de serie de la serie " & id_serie
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            Dim consecutivo_serie As Integer = 0
            mySqldatReader.Read()
            consecutivo_serie = mySqldatReader.Item(0)
            consecutivo_serie = consecutivo_serie + 1
            mySqldatReader.Close()
            Dim update_consecutivo_serie As String = "Update series_documentales set Consecutivo_subserie=" & consecutivo_serie & _
            " where Id_Series=" & id_serie
            myCommand.CommandText = update_consecutivo_serie
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Agregar_sub_serie_documental = "Imposible actualizar el consecutivo de la sub serie en el área "
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            Dim SqlInsert As String = "Insert Into subseries_documentales (Series_Documentales_Id_Series," & _
           "Consecutivo_Subserie,Nombre_Subserie,Estado_SubSerie,Consecutivo_Tip_Doc," & _
           "TIEMPO_RET_ARCH_CENTRAL,TIEMPO_RET_ARCH_GESTION,CONSERVACION_TOTAL,ELIMINACION," & _
           "MICROFILM,SELECCION,observaciones,TIEMPO_RET_ARCH_HISTORICO,ESTADO_DESICION,Estado_Publico_Sub_Serie," & _
           "Proceso,Procedimiento,Medio_soporte,Codigo_Arbitrario,Ra_registro_instrumento_archivistico_id_instrumento) values "
            If codigo_sub_serie = "" Then
                codigo_sub_serie = consecutivo_serie.ToString
            End If
            Dim sql_values As String = "(" & id_serie & "," & "0,'" & nombre_sub_serie & "',1,0," & tiempo_central & "," & tiempo_gestion & _
                "," & conservacion_total & "," & eliminacion & "," & digitalizacion & "," & seleccion & "," & ref_observaciones & "," & "0," & estado_desicion & _
                  "," & public_sub_serie & "," & ref_proceso_sub_serie & "," & ref_procedimiento & "," & ref_medio & ",'" & codigo_sub_serie & "'," & _
                   id_instrumento & ")"
            myCommand.CommandText = SqlInsert & sql_values
            sqlresultinsert = myCommand.ExecuteNonQuery
            If sqlresultinsert = 0 Then
                Agregar_sub_serie_documental = "Imposible registrar la sub serie documental "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            Dim id_sub_serie As Object = myCommand.LastInsertedId
            HttpContext.Current.Session.Item("TRD_CONTADOR") = HttpContext.Current.Session.Item("TRD_CONTADOR") + 1
            Dim id_tempo_nodo = HttpContext.Current.Session.Item("TRD_CONTADOR") & "|" & id_sub_serie & "|" & "2" '& "|" & nombre_sub_serie
            Dim attrNode_serie As New TreeNode
            attrNode_serie.Value = id_tempo_nodo
            attrNode_serie.Text = nombre_sub_serie
            attrNode_serie.ImageUrl = "../workflow/imageneswf/lista_sub_serie.png"
            trenode.ChildNodes.Add(attrNode_serie)
            update.Update()
            myTrans.Commit()
            myConnection.Close()
            Agregar_sub_serie_documental = "YES"
        Catch ex As Exception
            If myConnection.State = ConnectionState.Open Then
                myTrans.Rollback()
                myConnection.Close()
                Agregar_sub_serie_documental = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally
            If myConnection.State = ConnectionState.Open Then
                myConnection.Close()
            End If
            Agregar_sub_serie_documental = Agregar_sub_serie_documental
        End Try
    End Function
    Function Formato_sub_serie(ByVal Frase_formmat As String, _
                               ByRef Salida_Formato As String) As String
        Try
            Salida_Formato = ""
            Dim Frasetemp As String = LCase(Frase_formmat)
            Dim Espacios As String() = Frasetemp.Split(New [Char]() {" "c, ","c, "."c, ":"c, CChar(vbTab)})
            For i As Integer = 0 To Espacios.Length - 1
                If Espacios(i) <> "" Then
                    Dim Letracapital As String = UCase(Left(Espacios(i), 1))
                    Dim Otro_Texto As String = Right(Espacios(i), Espacios(i).Length - 1)
                    If i = 0 Then
                        Salida_Formato = Salida_Formato & Letracapital & Otro_Texto
                    Else
                        Salida_Formato = Salida_Formato & " " & Letracapital & Otro_Texto
                    End If
                End If
            Next
            Formato_sub_serie = "YES"
        Catch ex As Exception
            Formato_sub_serie = "Inconsistencia General Funcion Formato_sub_serie " & ex.Message
        End Try
    End Function
    Function Activa_editar_sub_serie_documental(ByVal id_sub_serie As Integer, _
                                            ByRef pag As Page) As String

        Try
            Dim TextBox_nombre_sub_serie As TextBox = pag.FindControl("TextBox_nombre_sub_serie")
            Dim observaciones As TextBox = pag.FindControl("TextBox_observaciones_sub_serie")
            Dim proceso_serie As TextBox = pag.FindControl("TextBoxProceso_sub")
            Dim procedimiento As TextBox = pag.FindControl("TextBoxProcedimiento_sub")
            Dim codigo_serie As TextBox = pag.FindControl("TextBoxCodigo_sub_Serie")
            Dim medio As DropDownList = pag.FindControl("DropDownListMedio_sub_serie")
            Dim estado_desicion As CheckBox = pag.FindControl("CheckBoxDiposicion_sub_serie")
            Dim tiempo_gestion As DropDownList = pag.FindControl("DropDownList_tiempo_retencion_gestion_sub_serie")
            Dim tiempo_central As DropDownList = pag.FindControl("DropDownList_tiempo_retencion_central_sub_serie")
            Dim conservacion_total As CheckBox = pag.FindControl("CheckBoxConservTotal_sub_serie")
            Dim eliminacion As CheckBox = pag.FindControl("CheckBoxSerieEliminacion_sub_serie")
            Dim digitalizacion As CheckBox = pag.FindControl("CheckBoxSerieDigitalizacion_sub_serie")
            Dim publi_serie As CheckBox = pag.FindControl("CheckBox_public_sub_serie")
            Dim seleccion As CheckBox = pag.FindControl("CheckBoxSerieSeleccion_sub_serie")
            Dim update As UpdatePanel = pag.FindControl("UpdatePanel_agregar_sub_serie")
            Dim Label_title_agregar_sub_serie As Label = pag.FindControl("Label_title_agregar_sub_serie")
            Dim UpdatePanel_title_agregar_sub_serie As UpdatePanel = pag.FindControl("UpdatePanel_title_agregar_sub_serie")
            Dim ModalPopupExtender_agregar_sub_serie As AjaxControlToolkit.ModalPopupExtender = pag.FindControl("ModalPopupExtender_agregar_sub_serie")
            Dim stru_sub_serie As stru_sub_serie_documental = Nothing
            Dim Result As String = ""
            Result = Me.Solicita_estructura_sub_serie_documental(id_sub_serie, stru_sub_serie)
            If Result <> "YES" Then
                Activa_editar_sub_serie_documental = Result
                Exit Function
            End If
            Result = Me.Asigna_datos_interface_sub_serie_documental(stru_sub_serie, _
                                                                TextBox_nombre_sub_serie, _
                                                                observaciones, _
                                                                proceso_serie, _
                                                                procedimiento, _
                                                                codigo_serie, _
                                                                medio, _
                                                                estado_desicion, _
                                                                tiempo_gestion, _
                                                                tiempo_central, _
                                                                conservacion_total, _
                                                                eliminacion, _
                                                                digitalizacion, _
                                                                publi_serie, _
                                                                seleccion, _
                                                                update)
            If Result <> "YES" Then
                Activa_editar_sub_serie_documental = Result
                Exit Function
            End If
            Label_title_agregar_sub_serie.Text = "Editar sub serie documental"
            UpdatePanel_title_agregar_sub_serie.Update()
            update.Update()
            ModalPopupExtender_agregar_sub_serie.Show()
            Activa_editar_sub_serie_documental = "YES"
            Exit Function
        Catch ex As Exception
            Activa_editar_sub_serie_documental = "Inconsistencia general función Activa_editar_serie_documental " & ex.Message
        End Try
    End Function
    Function Solicita_estructura_sub_serie_documental(ByVal id_sub_serie As Integer, _
                                                  ByRef stru_serie As stru_sub_serie_documental) As String
        Try
            Dim Parametro_Consulta = "select Series_Documentales_Id_Series,Consecutivo_Subserie,Nombre_Subserie," & _
            "Estado_SubSerie,Consecutivo_Tip_Doc,TIEMPO_RET_ARCH_CENTRAL,TIEMPO_RET_ARCH_HISTORICO," & _
            "CONSERVACION_TOTAL,ELIMINACION,MICROFILM,SELECCION,observaciones,TIEMPO_RET_ARCH_GESTION,ESTADO_DESICION," & _
            "Estado_Publico_Sub_Serie,Proceso,Procedimiento,Medio_soporte,Codigo_Arbitrario,Ra_registro_instrumento_archivistico_id_instrumento " & _
            "from subseries_documentales" & _
          " WHERE Id_SubSeries=" & id_sub_serie
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("SERIES_DOCUMENTALES")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_estructura_sub_serie_documental = "Función Solicita_estructura_sub_serie_documental dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_estructura_sub_serie_documental = "Imposible encontrar los datos de la sub serie documental"
                Exit Function
            Else
                stru_serie.Series_Documentales_Id_Series = Datset.Tables(0).Rows(0).Item(0)
                stru_serie.Consecutivo_Subserie = Datset.Tables(0).Rows(0).Item(1)
                stru_serie.Nombre_Subserie = Datset.Tables(0).Rows(0).Item(2)
                stru_serie.Estado_SubSerie = Datset.Tables(0).Rows(0).Item(3)
                stru_serie.Consecutivo_Tip_Doc = Datset.Tables(0).Rows(0).Item(4)
                stru_serie.TIEMPO_RET_ARCH_CENTRAL = Datset.Tables(0).Rows(0).Item(5)
                stru_serie.TIEMPO_RET_ARCH_HISTORICO = Datset.Tables(0).Rows(0).Item(6)
                stru_serie.CONSERVACION_TOTAL = Datset.Tables(0).Rows(0).Item(7)
                stru_serie.ELIMINACION = Datset.Tables(0).Rows(0).Item(8)
                stru_serie.MICROFILM = Datset.Tables(0).Rows(0).Item(9)
                stru_serie.SELECCION = Datset.Tables(0).Rows(0).Item(10)
                If Datset.Tables(0).Rows(0).IsNull(11) Then
                    stru_serie.observaciones = ""
                Else
                    stru_serie.observaciones = Datset.Tables(0).Rows(0).Item(11)
                End If
                stru_serie.TIEMPO_RET_ARCH_GESTION = Datset.Tables(0).Rows(0).Item(12)
                stru_serie.ESTADO_DESICION = Datset.Tables(0).Rows(0).Item(13)
                stru_serie.Estado_Publico_Sub_Serie = Datset.Tables(0).Rows(0).Item(14)
                If Datset.Tables(0).Rows(0).IsNull(15) Then
                    stru_serie.Proceso = ""
                Else
                    stru_serie.Proceso = Datset.Tables(0).Rows(0).Item(15)
                End If
                If Datset.Tables(0).Rows(0).IsNull(16) Then
                    stru_serie.Procedimiento = ""
                Else
                    stru_serie.Procedimiento = Datset.Tables(0).Rows(0).Item(16)
                End If
                If Datset.Tables(0).Rows(0).IsNull(17) Then
                    stru_serie.Medio_soporte = ""
                Else
                    stru_serie.Medio_soporte = Datset.Tables(0).Rows(0).Item(17)
                End If
                If Datset.Tables(0).Rows(0).IsNull(18) Then
                    stru_serie.Codigo_Arbitrario = ""
                Else
                    stru_serie.Codigo_Arbitrario = Datset.Tables(0).Rows(0).Item(18)
                End If
                If Datset.Tables(0).Rows(0).IsNull(19) Then
                    stru_serie.Ra_registro_instrumento_archivistico_id_instrumento = 0
                Else
                    stru_serie.Ra_registro_instrumento_archivistico_id_instrumento = Datset.Tables(0).Rows(0).Item(19)
                End If
                Solicita_estructura_sub_serie_documental = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estructura_sub_serie_documental = "Inconsistencia general función Solicita_estructura_sub_serrie " & ex.Message
        End Try
    End Function
    Function Asigna_datos_interface_sub_serie_documental(ByVal stru_sub_serie As stru_sub_serie_documental, _
                                                     ByRef nombre_sub_serie As TextBox, _
                                                     ByRef observaciones As TextBox, _
                                                     ByRef proceso_serie As TextBox, _
                                                     ByRef procedimiento As TextBox, _
                                                     ByRef codigo_serie As TextBox, _
                                                     ByRef medio As DropDownList, _
                                                     ByRef estado_desicion As CheckBox, _
                                                     ByRef tiempo_gestion As DropDownList, _
                                                     ByRef tiempo_central As DropDownList, _
                                                     ByRef conservacion_total As CheckBox, _
                                                     ByRef eliminacion As CheckBox, _
                                                     ByRef digitalizacion As CheckBox, _
                                                     ByRef public_serie As CheckBox, _
                                                     ByRef seleccion As CheckBox, _
                                                     ByRef update As UpdatePanel) As String
        Try
            nombre_sub_serie.Text = stru_sub_serie.Nombre_Subserie
            observaciones.Text = stru_sub_serie.observaciones
            proceso_serie.Text = stru_sub_serie.Proceso
            procedimiento.Text = stru_sub_serie.Procedimiento
            codigo_serie.Text = stru_sub_serie.Codigo_Arbitrario
            medio.Items.Clear()
            medio.Items.Add("")
            medio.Items.Add("Físico")
            medio.Items.Add("Digital")
            medio.Items.Add("Físico-Digital")
            For i As Integer = 0 To medio.Items.Count - 1
                If medio.Items(i).Text = stru_sub_serie.Medio_soporte Then
                    medio.SelectedValue = medio.Items(i).Text
                    Exit For
                End If
            Next
            If stru_sub_serie.ESTADO_DESICION = 1 Then
                estado_desicion.Checked = True
            Else
                estado_desicion.Checked = False
            End If
            tiempo_gestion.Items.Clear()
            tiempo_central.Items.Clear()
            For i As Integer = 0 To 100
                tiempo_gestion.Items.Add(i)
                tiempo_central.Items.Add(i)
            Next
            For i As Integer = 0 To tiempo_gestion.Items.Count - 1
                If tiempo_gestion.Items(i).Text = stru_sub_serie.Tiempo_Ret_Arch_Gestion Then
                    tiempo_gestion.SelectedValue = tiempo_gestion.Items(i).Text
                    Exit For
                End If
            Next
            For i As Integer = 0 To tiempo_central.Items.Count - 1
                If tiempo_central.Items(i).Text = stru_sub_serie.Tiempo_Ret_Arch_Central Then
                    tiempo_central.SelectedValue = tiempo_central.Items(i).Text
                    Exit For
                End If
            Next
            If stru_sub_serie.Conservacion_Total = 1 Then
                conservacion_total.Checked = True
            Else
                conservacion_total.Checked = False
            End If
            If stru_sub_serie.Eliminacion = 1 Then
                eliminacion.Checked = True
            Else
                eliminacion.Checked = False
            End If
            If stru_sub_serie.Microfilm = 1 Then
                digitalizacion.Checked = True
            Else
                digitalizacion.Checked = False
            End If
            If stru_sub_serie.Estado_Publico_Sub_Serie = 1 Then
                public_serie.Checked = True
            Else
                public_serie.Checked = False
            End If
            If stru_sub_serie.Seleccion = 1 Then
                seleccion.Checked = True
            Else
                seleccion.Checked = False
            End If
            Asigna_datos_interface_sub_serie_documental = "YES"
            Exit Function
        Catch ex As Exception
            Asigna_datos_interface_sub_serie_documental = "Inconsistencia general función Asigna_datos_interface_sub_serie_documental " & ex.Message
        End Try
    End Function
    Function Actualiza_sub_serie_documental(ByVal id_instrumento As Integer, _
                                      ByVal id_sub_serie As Integer, _
                                      ByRef nombre_sub_serie As String, _
                                      ByVal observaciones As String, _
                                      ByVal proceso_serie As String, _
                                      ByVal procedimiento As String, _
                                      ByVal codigo_serie As String, _
                                      ByVal medio As String, _
                                      ByVal estado_desicion As Integer, _
                                      ByVal tiempo_gestion As Integer, _
                                      ByVal tiempo_central As Integer, _
                                      ByVal conservacion_total As Integer, _
                                      ByVal eliminacion As Integer, _
                                      ByVal digitalizacion As Integer, _
                                      ByVal public_serie As Integer, _
                                      ByVal seleccion As Integer, _
                                      ByRef treview As TreeView, _
                                      ByRef update As UpdatePanel) As String
        Dim sql_actualiza_nombre_sub_serie_exp As String = ""
        Dim sql_actualiza_nombre_sub_serie_uni As String = ""
        Dim sql_actualiza_nombre_sub_serie_producion_documental As String = ""
        If nombre_sub_serie = "" Then
            Actualiza_sub_serie_documental = "Debe informar el nombre de la sub serie "
            Exit Function
        End If
        If codigo_serie = "" Then
            Actualiza_sub_serie_documental = "Debe informar el código de la sub serie "
            Exit Function
        End If
        If estado_desicion = 0 Then
            Actualiza_sub_serie_documental = "Debe marcar el estado de disposición final de la sub serie  "
            Exit Function
        End If
        Dim Result As String = ""
        Dim id_tipo_instrumento As Integer = 0
        Dim Ref_class_gestion_instrumento As New ClassGaGestionInstrumento
        Dim Ref_class_registro_instrumento_archivistico As New Class_ra_registro_instrumento_archivistico
        Result = Ref_class_registro_instrumento_archivistico.Retorna_id_tipo_instrumento(id_instrumento, _
                                                                                         id_tipo_instrumento)
        If Result <> "YES" Then
            Actualiza_sub_serie_documental = Result
            Exit Function
        End If
        If estado_desicion = 1 And id_instrumento = 1 Then
            If tiempo_gestion = 0 Then
                Actualiza_sub_serie_documental = "Debe informar el tiempo en archivo gestión  "
                Exit Function
            End If
            If tiempo_central = 0 Then
                Actualiza_sub_serie_documental = "Debe informar el tiempo en archivo central  "
                Exit Function
            End If
            If medio = "" Then
                Actualiza_sub_serie_documental = "Debe informar el medio de la serie  "
                Exit Function
            End If
            If conservacion_total = 0 And eliminacion = 0 _
                 Then
                Actualiza_sub_serie_documental = "Debe seleccionar la disposición final  "
                Exit Function
            End If
        End If
        If estado_desicion = 1 And id_instrumento = 2 Then
            If tiempo_central = 0 Then
                Actualiza_sub_serie_documental = "Debe informar el tiempo en archivo central  "
                Exit Function
            End If
            If medio = "" Then
                Actualiza_sub_serie_documental = "Debe informar el medio de la serie  "
                Exit Function
            End If
            If conservacion_total = 0 And eliminacion = 0 _
                 Then
                Actualiza_sub_serie_documental = "Debe seleccionar la disposición final  "
                Exit Function
            End If
        End If
        Dim confirm As Boolean = True
        Dim Cambios As String = ""
        Dim existencia As String = ""
        Dim update_registro As String = "Update subseries_documentales "
        Dim stru_sub_serie As stru_sub_serie_documental = Nothing
        Result = Me.Solicita_estructura_sub_serie_documental(id_sub_serie, stru_sub_serie)
        If Result <> "YES" Then
            Actualiza_sub_serie_documental = Result
            Exit Function
        End If
        If nombre_sub_serie <> stru_sub_serie.Nombre_Subserie Then
            Result = Me.Formato_sub_serie(nombre_sub_serie, nombre_sub_serie)
            If Result <> "YES" Then
                Actualiza_sub_serie_documental = Result
                Exit Function
            End If
            Result = Me.Verifica_Existencia_nombre_en_sub_serie(id_instrumento, nombre_sub_serie _
                                                                           , existencia)
            If Result <> "YES" Then
                Actualiza_sub_serie_documental = Result
                Exit Function
            End If
            If existencia = "YES" Then
                Actualiza_sub_serie_documental = "Ya se ecnuentra registrada una sub serie con el nombre " & nombre_sub_serie
                Exit Function
            End If
            Cambios = Cambios & " Cambio nombre sub serie documental " & stru_sub_serie.Nombre_Subserie & " Nuevo valor " & nombre_sub_serie
            If update_registro = "Update subseries_documentales " Then
                update_registro = update_registro & " set Nombre_Subserie='" & nombre_sub_serie & "'"
            Else
                update_registro = update_registro & " , Nombre_Subserie='" & nombre_sub_serie & "'"
            End If
            Result = Me.verifica_existencia_sub_serie_relacionada_expediente(id_sub_serie, existencia)
            If Result <> "YES" Then
                Actualiza_sub_serie_documental = Result
                Exit Function
            End If
            If existencia = "YES" Then
                sql_actualiza_nombre_sub_serie_exp = "Update expediente_archivo set NOMBRE_SUBSERIE_TRD='" & nombre_sub_serie & "'" & _
                    " where CODIGO_SUB_SERIE_TRD=" & id_sub_serie
                Cambios = Cambios & " (*Cambio de nombres de sub series en expedientes relacionados*) "
            End If
            Result = Me.verifica_existencia_sub_serie_relacionada_unidad_conservacion(id_sub_serie,
                                                                                      existencia)
            If Result <> "YES" Then
                Actualiza_sub_serie_documental = Result
                Exit Function
            End If
            If existencia = "YES" Then
                sql_actualiza_nombre_sub_serie_uni = "Update unidad_conservacion set NOMBRE_SUBSERIE='" & nombre_sub_serie & "'" &
                    " where CODIGO_SUBSERIE=" & id_sub_serie
                Cambios = Cambios & " (*Cambio de nombres de sub series en unidades de conservación relacionadas*) "
            End If
            '-----Detecta cambio de la serie en producón documental
            Dim ClassGaTipoDocumental As New ClassGaProducionDocumental
            existencia = ""
            Result = ClassGaTipoDocumental.Solicita_existencia_sub_serie_produccion_documental(id_sub_serie,
                                                                                               existencia)
            If Result <> "YES" Then
                Actualiza_sub_serie_documental = Result
                Exit Function
            End If
            If existencia = "YES" Then
                sql_actualiza_nombre_sub_serie_producion_documental = "update registro_producion_documental set SUBSERIE_DOCUMENTO='" & nombre_sub_serie & "'" &
                    " where ID_SUBSERIE_DOCUMENTO=" & id_sub_serie
                Cambios = Cambios & " (*Cambio de nombres de sub series en documentos relacionados en la producción documental*) "
            End If
        End If
        If observaciones <> stru_sub_serie.observaciones Then
            Cambios = Cambios & " Cambio observaciones " & stru_sub_serie.observaciones & " Nuevo valor " & observaciones
            If update_registro = "Update subseries_documentales " Then
                update_registro = update_registro & " set observaciones='" & observaciones & "'"
            Else
                update_registro = update_registro & " , observaciones='" & observaciones & "'"
            End If
        End If
        If proceso_serie <> stru_sub_serie.Proceso Then
            Cambios = Cambios & " Cambio proceso " & stru_sub_serie.Proceso & " Nuevo valor " & proceso_serie
            If update_registro = "Update subseries_documentales " Then
                update_registro = update_registro & " set Proceso='" & proceso_serie & "'"
            Else
                update_registro = update_registro & " , Proceso='" & proceso_serie & "'"
            End If
        End If
        If procedimiento <> stru_sub_serie.Procedimiento Then
            Cambios = Cambios & " Cambio procedimiento " & stru_sub_serie.Procedimiento & " Nuevo valor " & procedimiento
            If update_registro = "Update subseries_documentales " Then
                update_registro = update_registro & " set Procedimiento='" & procedimiento & "'"
            Else
                update_registro = update_registro & " , Procedimiento='" & procedimiento & "'"
            End If
        End If
        If codigo_serie <> stru_sub_serie.Codigo_Arbitrario Then
            Result = Me.Verfica_existecia_codigo_sub_serie_en_la_serie(stru_sub_serie.Series_Documentales_Id_Series,
                                                                      codigo_serie,
                                                                      existencia)
            If Result <> "YES" Then
                Actualiza_sub_serie_documental = Result
                Exit Function
            End If
            If existencia = "YES" Then
                Actualiza_sub_serie_documental = "Ya se ecnuentra registrada una sub serie con el código " & codigo_serie
                Exit Function
            End If
            Cambios = Cambios & " Cambio código sub serie " & stru_sub_serie.Codigo_Arbitrario & " Nuevo valor " & codigo_serie
            If update_registro = "Update subseries_documentales " Then
                update_registro = update_registro & " set Codigo_Arbitrario='" & codigo_serie & "'"
            Else
                update_registro = update_registro & " , Codigo_Arbitrario='" & codigo_serie & "'"
            End If
        End If
        If public_serie <> stru_sub_serie.Estado_Publico_Sub_Serie Then
            Cambios = Cambios & " Cambio estado público sub serie " & stru_sub_serie.Estado_Publico_Sub_Serie & " Nuevo valor " & public_serie
            If update_registro = "Update subseries_documentales " Then
                update_registro = update_registro & " set Estado_Publico_Sub_Serie='" & public_serie & "'"
            Else
                update_registro = update_registro & " , Estado_Publico_Sub_Serie='" & public_serie & "'"
            End If
        End If
        If estado_desicion <> stru_sub_serie.ESTADO_DESICION Then
            If estado_desicion = 1 Then
                If tiempo_gestion = 0 Then
                    Actualiza_sub_serie_documental = "Debe informar el tiempo en archivo gestión  "
                    Exit Function
                End If
                'If tiempo_central = 0 Then
                '    Actualiza_sub_serie_documental = "Debe informar el tiempo en archivo central  "
                '    Exit Function
                'End If
                If medio = "" Then
                    Actualiza_sub_serie_documental = "Debe informar el medio de la serie  "
                    Exit Function
                End If
                If conservacion_total = 0 And eliminacion = 0 _
                    And digitalizacion = 0 And seleccion = 0 Then
                    Actualiza_sub_serie_documental = "Debe seleccionar por lo menos un item de disposición final  "
                    Exit Function
                End If
            End If
            Cambios = Cambios & " Cambio estado de disposición final " & stru_sub_serie.ESTADO_DESICION & " Nuevo valor " & estado_desicion
            If update_registro = "Update subseries_documentales " Then
                update_registro = update_registro & " set ESTADO_DESICION='" & estado_desicion & "'"
            Else
                update_registro = update_registro & " , ESTADO_DESICION='" & estado_desicion & "'"
            End If
        End If
        If tiempo_gestion <> stru_sub_serie.TIEMPO_RET_ARCH_GESTION Then
            Cambios = Cambios & " Cambio tiempo archivo de gestión " & stru_sub_serie.TIEMPO_RET_ARCH_GESTION & " Nuevo valor " & tiempo_gestion
            If update_registro = "Update subseries_documentales " Then
                update_registro = update_registro & " set TIEMPO_RET_ARCH_GESTION='" & tiempo_gestion & "'"
            Else
                update_registro = update_registro & " , TIEMPO_RET_ARCH_GESTION='" & tiempo_gestion & "'"
            End If
        End If
        If tiempo_central <> stru_sub_serie.TIEMPO_RET_ARCH_CENTRAL Then
            Cambios = Cambios & " Cambio tiempo de archivo central  " & stru_sub_serie.TIEMPO_RET_ARCH_CENTRAL & " Nuevo valor " & tiempo_central
            If update_registro = "Update subseries_documentales " Then
                update_registro = update_registro & " set TIEMPO_RET_ARCH_CENTRAL='" & tiempo_central & "'"
            Else
                update_registro = update_registro & " , TIEMPO_RET_ARCH_CENTRAL='" & tiempo_central & "'"
            End If
        End If
        If medio <> stru_sub_serie.Medio_soporte Then
            Cambios = Cambios & " Cambio de medio  " & stru_sub_serie.Medio_soporte & " Nuevo valor " & medio
            If update_registro = "Update subseries_documentales " Then
                update_registro = update_registro & " set Medio_soporte='" & medio & "'"
            Else
                update_registro = update_registro & " , Medio_soporte='" & medio & "'"
            End If
        End If
        If conservacion_total <> stru_sub_serie.CONSERVACION_TOTAL Then
            Cambios = Cambios & " Cambio estado conservación total  " & stru_sub_serie.CONSERVACION_TOTAL & " Nuevo valor " & conservacion_total
            If update_registro = "Update subseries_documentales " Then
                update_registro = update_registro & " set CONSERVACION_TOTAL='" & conservacion_total & "'"
            Else
                update_registro = update_registro & " , CONSERVACION_TOTAL='" & conservacion_total & "'"
            End If
        End If
        If eliminacion <> stru_sub_serie.ELIMINACION Then
            Cambios = Cambios & " Cambio estado eliminación  " & stru_sub_serie.ELIMINACION & " Nuevo valor " & eliminacion
            If update_registro = "Update subseries_documentales " Then
                update_registro = update_registro & " set ELIMINACION='" & eliminacion & "'"
            Else
                update_registro = update_registro & " , ELIMINACION='" & eliminacion & "'"
            End If
        End If
        If digitalizacion <> stru_sub_serie.MICROFILM Then
            Cambios = Cambios & " Cambio estado tecnológico  " & stru_sub_serie.MICROFILM & " Nuevo valor " & digitalizacion
            If update_registro = "Update subseries_documentales " Then
                update_registro = update_registro & " set Microfilm='" & digitalizacion & "'"
            Else
                update_registro = update_registro & " , Microfilm='" & digitalizacion & "'"
            End If
        End If
        If seleccion <> stru_sub_serie.SELECCION Then
            Cambios = Cambios & " Cambio estado selección  " & stru_sub_serie.SELECCION & " Nuevo valor " & seleccion
            If update_registro = "Update subseries_documentales " Then
                update_registro = update_registro & " set SELECCION='" & seleccion & "'"
            Else
                update_registro = update_registro & " , SELECCION='" & seleccion & "'"
            End If
        End If
        If update_registro = "Update subseries_documentales " Then
            Actualiza_sub_serie_documental = "No se detectaron cambios para actualizar en la sub serie"
            Exit Function
        Else
            update_registro = update_registro & " where Id_SubSeries=" & id_sub_serie
        End If
        If update_registro = "Update subseries_documentales " Then
            Actualiza_sub_serie_documental = "YES"
            Exit Function
        End If
        Dim date_time As String = ""
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Result = ref_ClassGestionFechas.Retorna_fecha_registro(date_time)
        If Result <> "YES" Then
            Actualiza_sub_serie_documental = Result
            Exit Function
        End If
        Dim iphost As String = HttpContext.Current.Session.Item("ip_host_name")
        Dim id_user As String = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
        Dim logi_user As String = HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION")
        Dim hor As String = Now
        Dim detalle_trans As String = "EDITA SUB SERIE"
        Dim campos_trans As String = ""
        Dim isert_datos As String = ""
        campos_trans = "EDITA SUB SERIE " & id_sub_serie & "-" & stru_sub_serie.Nombre_Subserie & "  (" & _
        " EDITA SUB SERIE " & Cambios & ")"
        isert_datos = isert_datos & "(" & id_instrumento & _
            ",'" & detalle_trans & "','" & logi_user & "','" & id_user & "','" & date_time & "'," & _
                    "'" & iphost & "','GESTOR-WEB','" & campos_trans & "')"

        Dim update_gestion = "INSERT INTO ra_log_instrumentos_archivisticos (Ra_registro_instrumento_archivistico_id_instrumento,desc_op,USER_OPER,ID_USER,DATE_TRANS" &
                                             ",IP_TRANS,MODULO_REGISTRO,CAMPOS) values " &
                                             isert_datos
        Dim update_actualiza_producion As String = "update registro_producion_documental set SUBSERIE_DOCUMENTO='" & nombre_sub_serie
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Actualiza_sub_serie_documental = ""
        Try
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = update_registro
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Actualiza_sub_serie_documental = "Imposible registrar cambios en la sub serie"
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            If sql_actualiza_nombre_sub_serie_exp <> "" Then
                myCommand.CommandText = sql_actualiza_nombre_sub_serie_exp
                Switc = myCommand.ExecuteNonQuery()
                If Switc = 0 Then
                    Actualiza_sub_serie_documental = "Imposible actualizar nombres de sub serie en expedientes relacionados"
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            If sql_actualiza_nombre_sub_serie_uni <> "" Then
                myCommand.CommandText = sql_actualiza_nombre_sub_serie_uni
                Switc = myCommand.ExecuteNonQuery()
                If Switc = 0 Then
                    Actualiza_sub_serie_documental = "Imposible actualizar nombres de sub serie en unidades de conservación relacionadas"
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            If sql_actualiza_nombre_sub_serie_producion_documental <> "" Then
                myCommand.CommandText = sql_actualiza_nombre_sub_serie_producion_documental
                Switc = myCommand.ExecuteNonQuery()
                If Switc = 0 Then
                    Actualiza_sub_serie_documental = "Imposible actualizar la sub serie en la producion"
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            myCommand.CommandText = update_gestion
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Actualiza_sub_serie_documental = "Imposible registrar log del instrumento"
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            If nombre_sub_serie <> stru_sub_serie.Nombre_Subserie Then
                If Not treview.SelectedNode Is Nothing Then
                    treview.SelectedNode.Text = nombre_sub_serie
                    update.Update()
                End If
            End If
            myTrans.Commit()
            Actualiza_sub_serie_documental = "YES"
            Exit Function
        Catch ex As MySqlException
            If myConnection.State = ConnectionState.Open Then
                myTrans.Rollback()
                myConnection.Close()
                Actualiza_sub_serie_documental = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function


            End If
        Finally
            If myConnection.State = ConnectionState.Open Then
                myConnection.Close()
            End If
            Actualiza_sub_serie_documental = Actualiza_sub_serie_documental
        End Try
    End Function
    Function Verifica_Tipdocment_TipDoc_en_sub_serie(ByVal Id_sub_Serie As Integer, _
                                                 ByRef Existencia As String) As String
        Try
            Dim Parametro_Consulta As String = "Select * " & _
            " from tipo_doc_series where sub_serie_id_serie=" & Id_sub_Serie
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipo_doc_series")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verifica_Tipdocment_TipDoc_en_sub_serie = "Función  Verifica_Tipdocment_TipDoc_en_sub_serie dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Existencia = "NO"
                Verifica_Tipdocment_TipDoc_en_sub_serie = "YES"
                Exit Function
            Else
                Existencia = "YES"
                Verifica_Tipdocment_TipDoc_en_sub_serie = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_Tipdocment_TipDoc_en_sub_serie = "Inconsistencia general función Verifica_Tipdocment_TipDoc_en_sub_serie " & ex.Message
        End Try
    End Function
    Function verifica_existencia_sub_serie_relacionada_expediente(ByVal id_sub_serie As Integer, _
                                                                  ByRef existencia As String) As String
        Try
            Dim Parametro_Consulta As String = "Select * " & _
           " from expediente_archivo where CODIGO_SUB_SERIE_TRD=" & id_sub_serie
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("expediente_archivo")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                verifica_existencia_sub_serie_relacionada_expediente = "Función  verifica_existencia_sub_serie_relacionada_expediente dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                existencia = "NO"
                verifica_existencia_sub_serie_relacionada_expediente = "YES"
                Exit Function
            Else
                existencia = "YES"
                verifica_existencia_sub_serie_relacionada_expediente = "YES"
                Exit Function
            End If
        Catch ex As Exception
            verifica_existencia_sub_serie_relacionada_expediente = "Inconsistencia general función verifica_existencia_sub_serie_relacionada_expediente " & ex.Message
        End Try
    End Function
    Function verifica_existencia_serie_relacionda_expediente(ByVal id_serie As Integer, _
                                                            ByRef existencia As String) As String
        Try
            Dim Parametro_Consulta As String = "Select * " & _
            " from expediente_archivo where CODIGO_SERIE_TRD=" & id_serie
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("expediente_archivo")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                verifica_existencia_serie_relacionda_expediente = "Función  verifica_existencia_serie_relacionda_expediente dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                existencia = "NO"
                verifica_existencia_serie_relacionda_expediente = "YES"
                Exit Function
            Else
                existencia = "YES"
                verifica_existencia_serie_relacionda_expediente = "YES"
                Exit Function
            End If
        Catch ex As Exception
            verifica_existencia_serie_relacionda_expediente = "Inconsistencia general función verifica_existencia_serie_relacionda_expediente " & ex.Message
        End Try
    End Function
    Function verifica_existencia_sub_serie_relacionada_unidad_conservacion(ByVal id_sub_serie As Integer, _
                                                                           ByVal existencia As String) As String
        Try
            Dim Parametro_Consulta As String = "Select * " & _
            " from unidad_conservacion where CODIGO_SUBSERIE=" & id_sub_serie
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("expediente_archivo")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                verifica_existencia_sub_serie_relacionada_unidad_conservacion = "Función  verifica_existencia_sub_serie_relacionada_unidad_conservacion dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                existencia = "NO"
                verifica_existencia_sub_serie_relacionada_unidad_conservacion = "YES"
                Exit Function
            Else
                existencia = "YES"
                verifica_existencia_sub_serie_relacionada_unidad_conservacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            verifica_existencia_sub_serie_relacionada_unidad_conservacion = "Inconsistencia general función verifica_existencia_sub_serie_relacionada_unidad_conservacion " & _
                ex.Message
        End Try
    End Function
    Function verifica_existencia_serie_relacionada_unidad_conservacion(ByVal id_serie As Integer, _
                                                                       ByRef existencia As String) As String
        Try
            Dim Parametro_Consulta As String = "Select * " & _
            " from unidad_conservacion where CODIGO_SERIE=" & id_serie
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("expediente_archivo")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                verifica_existencia_serie_relacionada_unidad_conservacion = "Función  verifica_existencia_serie_relacionada_unidad_conservacion dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                existencia = "NO"
                verifica_existencia_serie_relacionada_unidad_conservacion = "YES"
                Exit Function
            Else
                existencia = "YES"
                verifica_existencia_serie_relacionada_unidad_conservacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            verifica_existencia_serie_relacionada_unidad_conservacion = "Inconsistencia general función verifica_existencia_serie_relacionada_unidad_conservacion " & ex.Message
        End Try
    End Function
    Function Eliminar_sub_serie_documental(ByVal id_sub_serie As Integer, _
                                       ByVal id_instrumento As Integer, _
                                       ByRef treview As TreeView, _
                                       ByRef update As UpdatePanel) As String

        Dim Result As String = ""
        Dim Existencia As String = ""
        Result = Me.Verifica_Existencia_Subserie_Doc(id_sub_serie, Existencia)
        If Result <> "YES" Then
            Eliminar_sub_serie_documental = Result
            Exit Function
        End If
        If Existencia = "YES" Then
            Eliminar_sub_serie_documental = "La sub serie tiene tipos documentales relacionadas imposible eliminar"
            Exit Function
        End If
        Result = Me.Verifica_Tipdocment_TipDoc_en_serie(id_sub_serie, Existencia)
        If Result <> "YES" Then
            Eliminar_sub_serie_documental = Result
            Exit Function
        End If
        If Existencia = "YES" Then
            Eliminar_sub_serie_documental = "La sub serie tiene tipos documentales relacionadas imposible eliminar"
            Exit Function
        End If
        Result = Me.verifica_existencia_sub_serie_relacionada_expediente(id_sub_serie, Existencia)
        If Result <> "YES" Then
            Eliminar_sub_serie_documental = Result
            Exit Function
        End If
        If Existencia = "YES" Then
            Eliminar_sub_serie_documental = "La sub serie tiene expedientes relacionadas imposible eliminar"
            Exit Function
        End If
        Result = Me.verifica_existencia_sub_serie_relacionada_unidad_conservacion(id_sub_serie, Existencia)
        If Result <> "YES" Then
            Eliminar_sub_serie_documental = Result
            Exit Function
        End If
        If Existencia = "YES" Then
            Eliminar_sub_serie_documental = "La sub serie tiene unidades de conservación relacionadas imposible eliminar"
            Exit Function
        End If
        Dim date_time As String = ""
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Result = ref_ClassGestionFechas.Retorna_fecha_registro(date_time)
        If Result <> "YES" Then
            Eliminar_sub_serie_documental = Result
            Exit Function
        End If
        Dim iphost As String = HttpContext.Current.Session.Item("ip_host_name")
        Dim id_user As String = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
        Dim logi_user As String = HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION")
        Dim hor As String = Now
        Dim detalle_trans As String = "ELIMINA SUB SERIE"
        Dim campos_trans As String = ""
        Dim isert_datos As String = ""
        campos_trans = "ELIMINA SUB SERIE " & id_sub_serie & "-" & treview.SelectedNode.Text & "  (" & _
        " ELIMINA SUB SERIE " & ")"
        isert_datos = isert_datos & "(" & id_instrumento & _
            ",'" & detalle_trans & "','" & logi_user & "','" & id_user & "','" & date_time & "'," & _
                    "'" & iphost & "','GESTOR-WEB','" & campos_trans & "')"
        Dim update_gestion = "INSERT INTO ra_log_instrumentos_archivisticos (Ra_registro_instrumento_archivistico_id_instrumento,desc_op,USER_OPER,ID_USER,DATE_TRANS" & _
                                             ",IP_TRANS,MODULO_REGISTRO,CAMPOS) values " & _
                                             isert_datos
        Dim elimna_registro As String = "delete from subseries_documentales where Id_SubSeries=" & id_sub_serie
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Eliminar_sub_serie_documental = ""
        Try
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = elimna_registro
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Eliminar_sub_serie_documental = "Imposible eliminar la sub serie"
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myCommand.CommandText = update_gestion
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Eliminar_sub_serie_documental = "Imposible registrar log del instrumento"
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            If Not treview.SelectedNode Is Nothing Then
                treview.Nodes.Remove(treview.SelectedNode)
                Dim sNodo As TreeNode = treview.SelectedNode
                Dim pNodo As TreeNode = sNodo.Parent
                pNodo.ChildNodes.Remove(sNodo)
                update.Update()
            End If
            myTrans.Commit()
            Eliminar_sub_serie_documental = "YES"
            Exit Function
        Catch ex As MySqlException
            If myConnection.State = ConnectionState.Open Then
                myTrans.Rollback()
                myConnection.Close()
                Eliminar_sub_serie_documental = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally
            If myConnection.State = ConnectionState.Open Then
                myConnection.Close()
            End If
            Eliminar_sub_serie_documental = Eliminar_sub_serie_documental
        End Try
    End Function
    Function Solicita_estado_sub_serie_documental(ByVal id_sub_serie As Integer, _
                                                  ByRef estado_sub_serie As Integer) As String
        Try
            Dim Parametro_Consulta As String = "Select Estado_SubSerie " & _
           " from subseries_documentales where Id_SubSeries=" & id_sub_serie
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("expediente_archivo")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_estado_sub_serie_documental = "Función  Solicita_estado_sub_serie_documental dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado_sub_serie = 0
                Solicita_estado_sub_serie_documental = "Imposible encontra información del estado de la sub serie (" & id_sub_serie & ")"
                Exit Function
            Else
                estado_sub_serie = Datset.Tables(0).Rows(0).Item(0)
                Solicita_estado_sub_serie_documental = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estado_sub_serie_documental = "Inconsistencia general función Solicita_estado_sub_serie_documental " & ex.Message
        End Try
    End Function

    Function Cambia_estado_sub_serie_documental(ByVal id_sub_serie As Integer, _
                                            ByVal estado_serie As Integer, _
                                            ByVal nombre_sub_serie As String, _
                                            ByVal id_instrumento As Integer) As String

        Dim Result As String = ""
        Dim estado_actual As Integer = 0
        Result = Me.Solicita_estado_sub_serie_documental(id_sub_serie, estado_actual)
        If Result <> "YES" Then
            Cambia_estado_sub_serie_documental = Result
            Exit Function
        End If
        If estado_actual = estado_serie Then
            Cambia_estado_sub_serie_documental = "YES"
            Exit Function
        End If
        Dim date_time As String = ""
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Result = ref_ClassGestionFechas.Retorna_fecha_registro(date_time)
        If Result <> "YES" Then
            Cambia_estado_sub_serie_documental = Result
            Exit Function
        End If
        Dim iphost As String = HttpContext.Current.Session.Item("ip_host_name")
        Dim id_user As String = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
        Dim logi_user As String = HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION")
        Dim hor As String = Now
        Dim detalle_trans As String = "CAMBIA ESTADO SUB SERIE"
        Dim campos_trans As String = ""
        Dim isert_datos As String = ""
        campos_trans = "CAMBIA ESTADO SUB SERIE " & id_sub_serie & "-" & nombre_sub_serie & "  (" & _
        " CAMBIA ESTADO SUB SERIE " & estado_actual & " Nuevo Valor " & estado_serie & ")"
        isert_datos = isert_datos & "(" & id_instrumento & _
            ",'" & detalle_trans & "','" & logi_user & "','" & id_user & "','" & date_time & "'," & _
                    "'" & iphost & "','GESTOR-WEB','" & campos_trans & "')"
        Dim update_gestion = "INSERT INTO ra_log_instrumentos_archivisticos (Ra_registro_instrumento_archivistico_id_instrumento,desc_op,USER_OPER,ID_USER,DATE_TRANS" & _
                                             ",IP_TRANS,MODULO_REGISTRO,CAMPOS) values " & _
                                             isert_datos
        Dim elimna_registro As String = "Update subseries_documentales set Estado_SubSerie=" & estado_serie & " where Id_SubSeries=" & id_sub_serie
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Cambia_estado_sub_serie_documental = ""
        Try
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = elimna_registro
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Cambia_estado_sub_serie_documental = "Imposible eliminar la serie"
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myCommand.CommandText = update_gestion
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Cambia_estado_sub_serie_documental = "Imposible registrar log del instrumento"
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If

            myTrans.Commit()
            Cambia_estado_sub_serie_documental = "YES"
            Exit Function
        Catch ex As MySqlException
            If myConnection.State = ConnectionState.Open Then
                myTrans.Rollback()
                myConnection.Close()
                Cambia_estado_sub_serie_documental = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally
            If myConnection.State = ConnectionState.Open Then
                myConnection.Close()
            End If
            Cambia_estado_sub_serie_documental = Cambia_estado_sub_serie_documental
        End Try
    End Function
    Function Agregar_tipo_documental_a_sub_serie(ByVal id_sub_serie As Integer, _
                                                 ByVal id_instrumento As Integer, _
                                                 ByVal nombre_documento As String, _
                                                 ByVal ruta_documento As String, _
                                                 ByVal codigo_documento As String, _
                                                 ByRef Trenod As TreeNode, _
                                                 ByRef update As UpdatePanel, _
                                                 ByVal chek As CheckBox) As String

        Dim Existencia As String = ""
        Dim Result As String = ""
        If nombre_documento = "" Then
            Agregar_tipo_documental_a_sub_serie = "Debe informar el nombre del documento "
            Exit Function
        End If
        Result = Me.Formato_sub_serie(nombre_documento, nombre_documento)
        If Result <> "YES" Then
            Agregar_tipo_documental_a_sub_serie = Result
            Exit Function
        End If
        Dim estado_trasversal As Integer = 0
        If chek.Checked = True Then
            estado_trasversal = 1
        End If
        Dim stru_sub_serie As stru_sub_serie_documental = Nothing
        Result = Me.Solicita_estructura_sub_serie_documental(id_sub_serie, stru_sub_serie)
        If Result <> "YES" Then
            Agregar_tipo_documental_a_sub_serie = Result
            Exit Function
        End If
        Result = Me.Verifica_existencia_tipdoc_en_sub_serie(id_sub_serie, nombre_documento, Existencia)
        If Result <> "YES" Then
            Agregar_tipo_documental_a_sub_serie = Result
            Exit Function
        End If
        If Existencia = "YES" Then
            Agregar_tipo_documental_a_sub_serie = "Esta intentado agregar un tipo de documento que existe en la sub serie"
            Exit Function
        End If
        Dim date_time As String = ""
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Result = ref_ClassGestionFechas.Retorna_fecha_registro(date_time)
        If Result <> "YES" Then
            Agregar_tipo_documental_a_sub_serie = Result
            Exit Function
        End If
        Dim ref_ruta_documento As String = "Null"
        If ruta_documento <> "" Then
            ref_ruta_documento = "'" & ruta_documento & "'"
        End If
        Dim Ref_codigo_documento As String = "Null"
        If codigo_documento <> "" Then
            Ref_codigo_documento = "'" & codigo_documento & "'"
        End If
        Dim elimna_registro As String = "insert into tipo_doc_series (sub_serie_id_serie,Consecutivo_Tip_Doc,Descripcion_Documento," & _
            "Fecha_Creacion,Estado_Tipo,PLANTILLA,id_instrumento,codigo_documento,Series_Documentales_Id_Series,tipo_doc_trasversal) values (" & _
            id_sub_serie & ",1,'" & nombre_documento & "','" & date_time & "',1," & ref_ruta_documento & "," & id_instrumento & "," _
            & Ref_codigo_documento & "," & stru_sub_serie.Series_Documentales_Id_Series & "," & estado_trasversal & ")"
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Agregar_tipo_documental_a_sub_serie = ""
        Try
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = elimna_registro
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Agregar_tipo_documental_a_sub_serie = "Imposible registrar el documento a la serie"
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            HttpContext.Current.Session.Item("TRD_CONTADOR") = HttpContext.Current.Session.Item("TRD_CONTADOR") + 1
            Dim id_tempo_nodo = HttpContext.Current.Session.Item("TRD_CONTADOR") & "|" & myCommand.LastInsertedId & "|" & "4" '& "|" & nombre_documento
            Dim attrNode_tipos As TreeNode = New TreeNode
            attrNode_tipos.Value = id_tempo_nodo
            attrNode_tipos.Text = nombre_documento
            attrNode_tipos.ImageUrl = "../workflow/imageneswf/lista_tipo_documento.png"
            Trenod.ChildNodes.Add(attrNode_tipos)
            update.Update()
            myTrans.Commit()
            Agregar_tipo_documental_a_sub_serie = "YES"
            Exit Function
        Catch ex As MySqlException
            If myConnection.State = ConnectionState.Open Then
                myTrans.Rollback()
                myConnection.Close()
                Agregar_tipo_documental_a_sub_serie = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally
            If myConnection.State = ConnectionState.Open Then
                myConnection.Close()
            End If
            Agregar_tipo_documental_a_sub_serie = Agregar_tipo_documental_a_sub_serie
        End Try
    End Function
    Function Verifica_existencia_tipdoc_en_sub_serie(ByVal id_sub_serie As Integer, _
                                                     ByVal nombre_documento As String, _
                                                     ByRef existencia As String) As String
        Try
            Dim Parametro_Consulta As String = "Select Id_Tipo_Doc_Series " & _
          " from tipo_doc_series where sub_serie_id_serie=" & id_sub_serie & _
          " and Descripcion_Documento='" & nombre_documento & "'"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipo_doc_series")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verifica_existencia_tipdoc_en_sub_serie = "Función  Verifica_existencia_tipdoc_en_sub_serie dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                existencia = "NO"
                Verifica_existencia_tipdoc_en_sub_serie = "YES"
                Exit Function
            Else
                existencia = "YES"
                Verifica_existencia_tipdoc_en_sub_serie = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_existencia_tipdoc_en_sub_serie = "Inconsistencia general función Verifica_existencia_tipdoc_en_sub_serie " & ex.Message
        End Try
    End Function
    Function Activa_editar_tipo_documento(ByVal id_tipo_documento As Integer, _
                                          ByVal nombre_docu As String, _
                                          ByVal ruta_document As String, _
                                          ByVal codigo_document As String, _
                                          ByVal nombre_update As String, _
                                          ByVal modal_documento As String, _
                                          ByVal nombre_label As String, _
                                          ByVal texto_label As String, _
                                          ByVal nombre_update_label As String, _
                                          ByRef pag As Page, _
                                          ByVal nombre_chek As String) As String
        Try
            Dim nombre_documento As TextBox = pag.FindControl(nombre_docu)
            Dim ruta_documento As TextBox = pag.FindControl(ruta_document)
            Dim codigo_documento As TextBox = pag.FindControl(codigo_document)
            Dim update As UpdatePanel = pag.FindControl(nombre_update)
            Dim modal As AjaxControlToolkit.ModalPopupExtender = pag.FindControl(modal_documento)
            Dim label_title As Label = pag.FindControl(nombre_label)
            Dim update_label As UpdatePanel = pag.FindControl(nombre_update_label)
            Dim chek As CheckBox = pag.FindControl(nombre_chek)
            Dim Result As String = ""
            Dim stru As stru_tipo_documental = Nothing

            Result = Me.Solicita_datos_estructura_tipo_documento(id_tipo_documento, _
                                                                 stru)
            If Result <> "YES" Then
                Activa_editar_tipo_documento = Result
                Exit Function
            End If

            Result = Me.Asigna_datos_interface_tipo_documento(stru, nombre_documento, _
                                                              ruta_documento, _
                                                              codigo_documento, _
                                                              update, _
                                                              chek)
            If Result <> "YES" Then
                Activa_editar_tipo_documento = Result
                Exit Function
            End If
            label_title.Text = texto_label
            update_label.Update()
            modal.Show()
            Activa_editar_tipo_documento = "YES"
            Exit Function
        Catch ex As Exception
            Activa_editar_tipo_documento = "Inconsistencia general función Activa_editar_tipo_documento " & ex.Message
        End Try
    End Function
    Function Solicita_datos_estructura_tipo_documento(ByVal id_tipo_documento As Integer, _
                                                      ByRef stru As stru_tipo_documental) As String
        Try
            Dim Parametro_Consulta As String = "Select Series_Documentales_Id_Series,Consecutivo_Tip_Doc, " & _
                "Descripcion_Documento,Fecha_Creacion,Estado_Tipo,PLANTILLA,EXTENSION_ARCHIVO,sub_serie_id_serie,codigo_documento,id_instrumento,tipo_doc_trasversal" & _
               " from tipo_doc_series where Id_Tipo_Doc_Series=" & id_tipo_documento
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipo_doc_series")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_datos_estructura_tipo_documento = "Función  Solicita_datos_estructura_tipo_documento dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_datos_estructura_tipo_documento = "Imposible encontrar datos del tipo documental (" & id_tipo_documento & ")"
                Exit Function
            Else
                stru.Series_Documentales_Id_Series = Datset.Tables(0).Rows(0).Item(0)
                stru.Consecutivo_Tip_Doc = Datset.Tables(0).Rows(0).Item(1)
                stru.Descripcion_Documento = Datset.Tables(0).Rows(0).Item(2)
                stru.Fecha_Creacion = Datset.Tables(0).Rows(0).Item(3)
                stru.Estado_Tipo = Datset.Tables(0).Rows(0).Item(4)
                If Datset.Tables(0).Rows(0).IsNull(5) Then
                    stru.PLANTILLA = ""
                Else
                    stru.PLANTILLA = Datset.Tables(0).Rows(0).Item(5)
                End If
                stru.EXTENSION_ARCHIVO = Datset.Tables(0).Rows(0).Item(6)
                If Datset.Tables(0).Rows(0).IsNull(7) Then
                    stru.sub_serie_id_serie = 0
                Else
                    stru.sub_serie_id_serie = Datset.Tables(0).Rows(0).Item(7)
                End If
                If Datset.Tables(0).Rows(0).IsNull(8) Then
                    stru.codigo_documento = ""
                Else
                    stru.codigo_documento = Datset.Tables(0).Rows(0).Item(8)
                End If
                If Datset.Tables(0).Rows(0).IsNull(9) Then
                    stru.id_instrumento = 0
                Else
                    stru.id_instrumento = Datset.Tables(0).Rows(0).Item(9)
                End If
                If Datset.Tables(0).Rows(0).IsNull(10) Then
                    stru.tipo_doc_trasversal = 0
                Else
                    stru.tipo_doc_trasversal = Datset.Tables(0).Rows(0).Item(10)
                End If
                Solicita_datos_estructura_tipo_documento = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_datos_estructura_tipo_documento = "Inconsistencia general función Solicita_datos_estructura_tipo_documento " & ex.Message
        End Try
    End Function
    Function Asigna_datos_interface_tipo_documento(ByVal stru As stru_tipo_documental, _
                                                  ByRef nombre_documento As TextBox, _
                                                   ByRef ruta_documento As TextBox, _
                                                   ByRef codigo_documento As TextBox, _
                                                   ByRef update As UpdatePanel, _
                                                   ByRef chek As CheckBox) As String
        Try
            nombre_documento.Text = stru.Descripcion_Documento
            ruta_documento.Text = stru.PLANTILLA
            codigo_documento.Text = stru.codigo_documento
            If stru.tipo_doc_trasversal = 1 Then
                chek.Checked = True
            Else
                chek.Checked = False
            End If
            update.Update()
            Asigna_datos_interface_tipo_documento = "YES"
            Exit Function
        Catch ex As Exception
            Asigna_datos_interface_tipo_documento = "Inconsistencia general función Asigna_datos_interface_tipo_documento " & ex.Message
        End Try
    End Function
    Function Edita_tipo_documental(ByVal id_tipo_documento As Integer,
                                   ByVal id_instrumento As Integer, _
                                   ByVal nombre_documento As String, _
                                   ByVal ruta_documento As String, _
                                   ByVal codigo_documento As String, _
                                   ByRef Trenod As TreeNode, _
                                   ByRef update As UpdatePanel, _
                                   ByVal chek As CheckBox) As String
        Dim Existencia As String = ""
        Dim Result As String = ""
        Dim Cambios As String = ""
        Dim update_registro As String = "Update tipo_doc_series "
        Dim update_tipo_producion As String = ""
        Dim stru As stru_tipo_documental = Nothing
        If nombre_documento = "" Then
            Edita_tipo_documental = "Debe informar el nombre del documento "
            Exit Function
        End If
        Result = Me.Formato_sub_serie(nombre_documento, _
                                      nombre_documento)
        If Result <> "YES" Then
            Edita_tipo_documental = Result
            Exit Function
        End If
        Result = Me.Solicita_datos_estructura_tipo_documento(id_tipo_documento, _
                                                             stru)
        If Result <> "YES" Then
            Edita_tipo_documental = Result
            Exit Function
        End If
        If nombre_documento <> stru.Descripcion_Documento Then
            If stru.sub_serie_id_serie <> 0 Then
                Result = Me.Verifica_existencia_tipdoc_en_sub_serie(stru.sub_serie_id_serie, nombre_documento, Existencia)
                If Result <> "YES" Then
                    Edita_tipo_documental = Result
                    Exit Function
                End If
            Else
                Result = Me.Verifica_existencia_documento_en_serie(stru.Series_Documentales_Id_Series, nombre_documento, Existencia)
                If Result <> "YES" Then
                    Edita_tipo_documental = Result
                    Exit Function
                End If
            End If
            If Existencia = "YES" Then
                Edita_tipo_documental = "Esta intentado editar con un nombre de tipo de documental que existe en la sub serie"
                Exit Function
            End If
            If stru.sub_serie_id_serie <> 0 Then
                Result = Me.Verifica_existencia_tipo_documental_producion_sub_serie(id_tipo_documento, stru.sub_serie_id_serie, Existencia)
                If Result <> "YES" Then
                    Edita_tipo_documental = Result
                    Exit Function
                End If
                If Existencia = "YES" Then
                    update_tipo_producion = "update registro_producion_documental set DESCRIPCION_TIPO_DOCUMENTO='" & nombre_documento & "'" & _
                        " where ID_TIPO_DOCUMENTO=" & id_tipo_documento & " and ID_SUBSERIE_DOCUMENTO=" & stru.sub_serie_id_serie
                End If
                Cambios = Cambios & " Cambio de nombre tipo documental " & stru.Descripcion_Documento & " Nuevo valor " & nombre_documento
                If update_registro = "Update tipo_doc_series " Then
                    update_registro = update_registro & " set Descripcion_Documento='" & nombre_documento & "'"
                Else
                    update_registro = update_registro & " , Descripcion_Documento='" & nombre_documento & "'"
                End If
            Else
                Result = Me.Verifica_existencia_tipo_documental_producion_serie(id_tipo_documento, _
                                                                                stru.Series_Documentales_Id_Series, _
                                                                                Existencia)
                If Result <> "YES" Then
                    Edita_tipo_documental = Result
                    Exit Function
                End If
                If Existencia = "YES" Then
                    update_tipo_producion = "update registro_producion_documental set DESCRIPCION_TIPO_DOCUMENTO='" & nombre_documento & "'" & _
                        " where ID_TIPO_DOCUMENTO=" & id_tipo_documento & " and ID_SERIE_DOCUMENTO=" & stru.Series_Documentales_Id_Series
                End If
                Cambios = Cambios & " Cambio de nombre tipo documental " & stru.Descripcion_Documento & " Nuevo valor " & nombre_documento
                If update_registro = "Update tipo_doc_series " Then
                    update_registro = update_registro & " set Descripcion_Documento='" & nombre_documento & "'"
                Else
                    update_registro = update_registro & " , Descripcion_Documento='" & nombre_documento & "'"
                End If
            End If

        End If
        Dim ref_ruta_documento As String = "Null"
        If ruta_documento <> "" Then
            ref_ruta_documento = "'" & ruta_documento & "'"
        End If
        If ruta_documento <> stru.PLANTILLA Then
            Cambios = Cambios & " Cambio de ruta tipo documento " & stru.PLANTILLA & " Nuevo valor " & ruta_documento
            If update_registro = "Update tipo_doc_series " Then
                update_registro = update_registro & " set PLANTILLA=" & ref_ruta_documento
            Else
                update_registro = update_registro & " , PLANTILLA=" & ref_ruta_documento
            End If
        End If
        Dim ref_codigo_documento As String = "Null"
        If codigo_documento <> "" Then
            ref_codigo_documento = "'" & codigo_documento & "'"
        End If
        If codigo_documento <> stru.codigo_documento Then
            Cambios = Cambios & " Cambio código de documento " & stru.codigo_documento & " Nuevo valor " & codigo_documento
            If update_registro = "Update tipo_doc_series " Then
                update_registro = update_registro & " set codigo_documento=" & ref_codigo_documento
            Else
                update_registro = update_registro & " , codigo_documento=" & ref_codigo_documento
            End If
        End If
        Dim estado_trasversal As Integer = 0
        If chek.Checked = True Then
            estado_trasversal = 1
        End If
        If stru.tipo_doc_trasversal <> estado_trasversal Then
            If update_registro = "Update tipo_doc_series " Then
                update_registro = update_registro & " set tipo_doc_trasversal=" & estado_trasversal
            Else
                update_registro = update_registro & " , tipo_doc_trasversal=" & estado_trasversal
            End If
        End If
        If update_registro <> "Update tipo_doc_series " Then
            update_registro = update_registro & " where Id_Tipo_Doc_Series=" & id_tipo_documento
        End If  
        Dim date_time As String = ""
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Result = ref_ClassGestionFechas.Retorna_fecha_registro(date_time)
        If Result <> "YES" Then
            Edita_tipo_documental = Result
            Exit Function
        End If
        Dim iphost As String = HttpContext.Current.Session.Item("ip_host_name")
        Dim id_user As String = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
        Dim logi_user As String = HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION")
        Dim hor As String = Now
        Dim detalle_trans As String = "EDITA TIPO DOCUMENTO"
        Dim campos_trans As String = ""
        Dim isert_datos As String = ""
        campos_trans = "EDITA TIPO DOCUMENTO " & id_tipo_documento & "-" & stru.Descripcion_Documento & "  (" & _
        " EDITA TIPO DOCUMENTO " & Cambios & ")"
        isert_datos = isert_datos & "(" & id_instrumento & _
            ",'" & detalle_trans & "','" & logi_user & "','" & id_user & "','" & date_time & "'," & _
                    "'" & iphost & "','GESTOR-WEB','" & campos_trans & "')"

        Dim update_gestion = "INSERT INTO ra_log_instrumentos_archivisticos (Ra_registro_instrumento_archivistico_id_instrumento,desc_op,USER_OPER,ID_USER,DATE_TRANS" & _
                                             ",IP_TRANS,MODULO_REGISTRO,CAMPOS) values " & _
                                             isert_datos
        If update_registro = "Update tipo_doc_series " Then
            Edita_tipo_documental = "YES"
            Exit Function
        End If
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Edita_tipo_documental = ""
        Try
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = update_registro
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Edita_tipo_documental = "Imposible registrar cambios en el tipo documental"
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            If update_tipo_producion <> "" Then
                myCommand.CommandText = update_tipo_producion
                Switc = myCommand.ExecuteNonQuery()
                If Switc = 0 Then
                    Edita_tipo_documental = "Imposible actualizar documentos relacionados"
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If

            myCommand.CommandText = update_gestion
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Edita_tipo_documental = "Imposible registrar log del instrumento"
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            If nombre_documento <> stru.Descripcion_Documento Then
                Trenod.Text = nombre_documento
                update.Update()
            End If
            myTrans.Commit()
            Edita_tipo_documental = "YES"
            Exit Function
        Catch ex As MySqlException
            If myConnection.State = ConnectionState.Open Then
                myTrans.Rollback()
                myConnection.Close()
                Edita_tipo_documental = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function


            End If
        Finally
            If myConnection.State = ConnectionState.Open Then
                myConnection.Close()
            End If
            Edita_tipo_documental = Edita_tipo_documental
        End Try

    End Function

    Function Verifica_existencia_tipo_documental_producion_serie(ByVal id_tipo_documento As Integer, _
                                                                 ByVal id_serie As Integer, _
                                                                 ByRef existencia As String) As String
        Try
            Dim Parametro_Consulta As String = "Select ID_TIPO_DOCUMENTO " & _
            " from registro_producion_documental where ID_TIPO_DOCUMENTO=" & id_tipo_documento & _
            " and SERIE_DOCUMENTO=" & id_serie
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("registro_producion_documental")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verifica_existencia_tipo_documental_producion_serie = "Función  Verifica_existencia_tipo_documental_producion dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                existencia = "NO"
                Verifica_existencia_tipo_documental_producion_serie = "YES"
                Exit Function
            Else
                existencia = "YES"
                Verifica_existencia_tipo_documental_producion_serie = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_existencia_tipo_documental_producion_serie = "Inconsistencia general función Verifica_existencia_tipo_documental_producion_serie " & ex.Message
        End Try
    End Function
    Function Verifica_existencia_tipo_documental_producion_sub_serie(ByVal id_tipo_documento As Integer, _
                                                                 ByVal id_sub_serie As Integer, _
                                                                 ByRef existencia As String) As String
        Try
            Dim Parametro_Consulta As String = "Select ID_TIPO_DOCUMENTO " & _
            " from registro_producion_documental where ID_TIPO_DOCUMENTO=" & id_tipo_documento & _
            " and ID_SUBSERIE_DOCUMENTO=" & id_sub_serie
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("registro_producion_documental")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verifica_existencia_tipo_documental_producion_sub_serie = "Función  Verifica_existencia_tipo_documental_producion_sub_serie dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                existencia = "NO"
                Verifica_existencia_tipo_documental_producion_sub_serie = "YES"
                Exit Function
            Else
                existencia = "YES"
                Verifica_existencia_tipo_documental_producion_sub_serie = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_existencia_tipo_documental_producion_sub_serie = "Inconsistencia general función Verifica_existencia_tipo_documental_producion_sub_serie " & ex.Message
        End Try
    End Function

    Function Eliminar_tipo_documental_serie_sub_serie(ByVal id_tipo_documento As Integer, _
                                                      ByVal id_instrumento As Integer, _
                                                      ByRef treview As TreeView, _
                                                      ByRef update As UpdatePanel) As String

        Dim Result As String = ""
        Dim existencia As String = ""
        Dim stru As stru_tipo_documental = Nothing
        Result = Me.Solicita_datos_estructura_tipo_documento(id_tipo_documento, stru)
        If Result <> "YES" Then
            Eliminar_tipo_documental_serie_sub_serie = Result
            Exit Function
        End If
        If stru.sub_serie_id_serie <> 0 Then
            Result = Me.Verifica_existencia_tipo_documental_producion_sub_serie(id_tipo_documento, stru.sub_serie_id_serie, existencia)
            If Result <> "YES" Then
                Eliminar_tipo_documental_serie_sub_serie = Result
                Exit Function
            End If
        Else
            Result = Me.Verifica_existencia_tipo_documental_producion_serie(id_tipo_documento, stru.Series_Documentales_Id_Series, existencia)
            If Result <> "YES" Then
                Eliminar_tipo_documental_serie_sub_serie = Result
                Exit Function
            End If
        End If       
        If existencia = "YES" Then
            Eliminar_tipo_documental_serie_sub_serie = "Imposible elimnar el tipo documental, por que tiene documentos en produción relacionados"
            Exit Function
        End If
        '--------------------------------------------------------------
        'Verifica existencia tipo documental relacionado lista chequeo
        '--------------------------------------------------------------
        Dim existencia_tipodocumental_lista_chueq As String = ""
        Result = Me.Solicita_existencia_tipos_documentales_relacionados_a_lista_chequeo(id_tipo_documento, _
                                                                                        existencia_tipodocumental_lista_chueq)
        If Result <> "YES" Then
            Eliminar_tipo_documental_serie_sub_serie = Result
            Exit Function
        End If
        If existencia_tipodocumental_lista_chueq = "YES" Then
            Eliminar_tipo_documental_serie_sub_serie = "Tipo documental relacionado a lista de chequeo, imposible eliminar"
            Exit Function
        End If
        Dim date_time As String = ""
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Result = ref_ClassGestionFechas.Retorna_fecha_registro(date_time)
        If Result <> "YES" Then
            Eliminar_tipo_documental_serie_sub_serie = Result
            Exit Function
        End If
        Dim iphost As String = HttpContext.Current.Session.Item("ip_host_name")
        Dim id_user As String = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
        Dim logi_user As String = HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION")
        Dim hor As String = Now
        Dim detalle_trans As String = "ELIMINA TIPO DOCUMENTO"
        Dim campos_trans As String = ""
        Dim isert_datos As String = ""
        campos_trans = "ELIMINA TIPO DOCUMENTO " & id_tipo_documento & "-" & treview.SelectedNode.Text & "  (" & _
        " ELIMINA TIPO DOCUMENTO " & ")"
        isert_datos = isert_datos & "(" & id_instrumento & _
            ",'" & detalle_trans & "','" & logi_user & "','" & id_user & "','" & date_time & "'," & _
                    "'" & iphost & "','GESTOR-WEB','" & campos_trans & "')"
        Dim update_gestion = "INSERT INTO ra_log_instrumentos_archivisticos (Ra_registro_instrumento_archivistico_id_instrumento,desc_op,USER_OPER,ID_USER,DATE_TRANS" & _
                                             ",IP_TRANS,MODULO_REGISTRO,CAMPOS) values " & _
                                             isert_datos
        Dim elimna_registro As String = "delete from tipo_doc_series where Id_Tipo_Doc_Series=" & id_tipo_documento
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Eliminar_tipo_documental_serie_sub_serie = ""
        Try
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = elimna_registro
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Eliminar_tipo_documental_serie_sub_serie = "Imposible eliminar el tipo documental"
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myCommand.CommandText = update_gestion
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Eliminar_tipo_documental_serie_sub_serie = "Imposible registrar log del instrumento"
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            If Not treview.SelectedNode Is Nothing Then
                treview.Nodes.Remove(treview.SelectedNode)
                Dim sNodo As TreeNode = treview.SelectedNode
                Dim pNodo As TreeNode = sNodo.Parent
                pNodo.ChildNodes.Remove(sNodo)
                update.Update()
            End If
            myTrans.Commit()
            Eliminar_tipo_documental_serie_sub_serie = "YES"
            Exit Function
        Catch ex As MySqlException
            If myConnection.State = ConnectionState.Open Then
                myTrans.Rollback()
                myConnection.Close()
                Eliminar_tipo_documental_serie_sub_serie = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally
            If myConnection.State = ConnectionState.Open Then
                myConnection.Close()
            End If
            Eliminar_tipo_documental_serie_sub_serie = Eliminar_tipo_documental_serie_sub_serie
        End Try
    End Function
    Function Solicita_estado_tipo_documento(ByVal id_tipo_documento As Integer, _
                                                   ByRef estado As Integer) As String
        Try
            Dim Parametro_Consulta As String = "Select Estado_Tipo " & _
            " from tipo_doc_series where Id_Tipo_Doc_Series=" & id_tipo_documento
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipo_doc_series")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_estado_tipo_documento = "Función Solicita_estado_tipo_documento dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado = 0
                Solicita_estado_tipo_documento = "Imposible encontra información del estado del tipo documento (" & id_tipo_documento & ")"
                Exit Function
            Else
                estado = Datset.Tables(0).Rows(0).Item(0)
                Solicita_estado_tipo_documento = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estado_tipo_documento = "Inconsistencia general función Solicita_estado_tipo_documento " & ex.Message
        End Try
    End Function
    Function Solicita_existencia_tipos_documentales_relacionados_a_lista_chequeo(ByVal id_tipo_dcoumental As Integer, _
                                                                                 ByRef existencia_tipodocumental_lista_chueq As String) As String
        Try
            Dim Parametro_Consulta As String = "Select tipo_doc_series_Id_Tipo_Doc_Series " & _
           " from ra_dig_tipos_docum_lista_chequeo where tipo_doc_series_Id_Tipo_Doc_Series=" & id_tipo_dcoumental
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_dig_tipos_docum_lista_chequeo")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_existencia_tipos_documentales_relacionados_a_lista_chequeo = "Función Solicita_existencia_tipos_documentales_relacionados_a_lista_chequeo dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                existencia_tipodocumental_lista_chueq = "NO"
                Solicita_existencia_tipos_documentales_relacionados_a_lista_chequeo = "YES"
                Exit Function
            Else
                existencia_tipodocumental_lista_chueq = "YES"
                Solicita_existencia_tipos_documentales_relacionados_a_lista_chequeo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_existencia_tipos_documentales_relacionados_a_lista_chequeo = "Inconsistencia general función Solicita_existencia_tipos_documentales_relacionados_a_lista_chequeo " & ex.Message
        End Try
    End Function
    Function Cambia_estado_tipo_documento(ByVal id_tipo_documento As Integer, _
                                          ByVal estado As Integer, _
                                          ByVal nombre_tipo As String, _
                                          ByVal id_instrumento As Integer) As String
        Dim Result As String = ""
        Dim estado_actual As Integer = 0
        Result = Me.Solicita_estado_tipo_documento(id_tipo_documento, estado_actual)
        If Result <> "YES" Then
            Cambia_estado_tipo_documento = Result
            Exit Function
        End If
        If estado_actual = estado Then
            Cambia_estado_tipo_documento = "YES"
            Exit Function
        End If
        Dim date_time As String = ""
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Result = ref_ClassGestionFechas.Retorna_fecha_registro(date_time)
        If Result <> "YES" Then
            Cambia_estado_tipo_documento = Result
            Exit Function
        End If
        Dim iphost As String = HttpContext.Current.Session.Item("ip_host_name")
        Dim id_user As String = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
        Dim logi_user As String = HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION")
        Dim hor As String = Now
        Dim detalle_trans As String = "CAMBIA ESTADO TIPO DOCUMENTO"
        Dim campos_trans As String = ""
        Dim isert_datos As String = ""
        campos_trans = "CAMBIA ESTADO TIPO DOCUMENTO " & id_tipo_documento & "-" & nombre_tipo & "  (" & _
        " CAMBIA ESTADO TIPO DOCUMENTO " & estado_actual & " Nuevo Valor " & estado & ")"
        isert_datos = isert_datos & "(" & id_instrumento & _
            ",'" & detalle_trans & "','" & logi_user & "','" & id_user & "','" & date_time & "'," & _
                    "'" & iphost & "','GESTOR-WEB','" & campos_trans & "')"
        Dim update_gestion = "INSERT INTO ra_log_instrumentos_archivisticos (Ra_registro_instrumento_archivistico_id_instrumento,desc_op,USER_OPER,ID_USER,DATE_TRANS" & _
                                             ",IP_TRANS,MODULO_REGISTRO,CAMPOS) values " & _
                                             isert_datos
        Dim elimna_registro As String = "Update tipo_doc_series set Estado_Tipo=" & estado & " where Id_Tipo_Doc_Series=" & id_tipo_documento
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Cambia_estado_tipo_documento = ""
        Try
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = elimna_registro
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Cambia_estado_tipo_documento = "Imposible eliminar la serie"
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myCommand.CommandText = update_gestion
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Cambia_estado_tipo_documento = "Imposible registrar log del instrumento"
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If

            myTrans.Commit()
            Cambia_estado_tipo_documento = "YES"
            Exit Function
        Catch ex As MySqlException
            If myConnection.State = ConnectionState.Open Then
                myTrans.Rollback()
                myConnection.Close()
                Cambia_estado_tipo_documento = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally
            If myConnection.State = ConnectionState.Open Then
                myConnection.Close()
            End If
            Cambia_estado_tipo_documento = Cambia_estado_tipo_documento
        End Try
    End Function
    Function Export_Serie(ByVal Id_Areadep As Object, _
                          ByRef consecutivo_serie As Object, _
                          ByVal nombre_Area As String, _
                          ByRef pag As Page) As String
        Try
            Dim Refclas As New ClassTrdDocumental
            Dim html As New StringBuilder()
            Dim w As StreamWriter = Nothing
            Dim Result As String = ""
            Dim codigo_depart As String = ""
            Result = Me.Solicita_codigo_area_departamento(Id_Areadep, codigo_depart)
            If Result <> "YES" Then
                Export_Serie = Result
                Exit Function
            End If
            Dim Ruta As String = HttpContext.Current.Session.Item("GA_RUTA_TEMPO_DESCARGA") & "\table_instrumento.xls"
            Result = Export(Id_Areadep, consecutivo_serie, codigo_depart, _
                            HttpContext.Current.Session.Item("EMPRESA_GESTION"), _
                            nombre_Area, Ruta, w, html, pag)
            If Result <> "YES" Then
                Export_Serie = Result
                Exit Function
            End If
            Export_Serie = "YES"
        Catch ex As Exception
            Export_Serie = "Inconsistencia general función Export_Serie " & ex.Message
        End Try

    End Function
    Function Export(ByVal Id_Areadep As Integer, _
                    ByVal consecutivo_serie As Integer, _
                    ByVal area_depart As String, _
                    ByVal nombre_entidad As String, _
                    ByVal ofic_product As String, _
                    ByVal ruta As String, _
                    ByVal w As StreamWriter, _
                    ByRef html As StringBuilder, _
                    ByRef pag As Page) As String

        Try
            Dim Hidden_ruta_archivo As Object = pag.FindControl("Hidden_ruta_archivo")
            Dim ifmExcel As Object = pag.FindControl("ifmExcel_")
            Dim updatapanel_iframe As UpdatePanel = pag.FindControl("updatapanel_iframe")
            Dim fs As New FileStream(ruta, FileMode.Create, FileAccess.ReadWrite)
            w = New StreamWriter(fs)
            Dim comillas As String = Char.ConvertFromUtf32(34)
            html.Append("<!DOCTYPE html PUBLIC" + comillas + "-//W3C//DTD XHTML 1.0 Transitional//EN" + comillas + " " _
           + comillas + "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd" + comillas + ">")
            html.Append("<html xmlns=" + comillas + "http://www.w3.org/1999/xhtml" + comillas + ">")
            html.Append("<head>")
            html.Append("<meta http-equiv=" + comillas + "Content-Type" + comillas + "content=" + comillas + "text/html charset=utf-8" + comillas + "/>")
            html.Append("<title>Untitled Document</title>")
            html.Append("</head>")
            html.Append("<body>")
            html.Append("<br>")
            html.Append("<br>")
            Dim mg = "<img src=" + comillas + HttpContext.Current.Server.MapPath("../imagera/logo_trd.png") + comillas + " alt=" + comillas + "Smiley face" + comillas + "width=" + comillas + "80" + comillas + _
                      "height =" + comillas + "80" + comillas + " >"
            html.Append(" <table  CELLSPACING=0 CELLPADDING=1 border=1> " & _
                         " <tr> " & _
                              "<td rowspan=" + comillas + "5" + comillas + "; colspan=" + comillas + "2" + comillas + "> " & mg & " </td>" & _
                         "</tr>" & _
                       " <tr> " & _
                        "<td colspan=" + comillas + "12" + comillas + " ; align=" + comillas + "middle" + comillas + "> " & "TABLA DE RETENCION DOCUMENTAL" & " </td>" & _
                      "</tr>" & _
                       " <tr> " & _
                        "<td> " & "CODIGO" & " </td>" & _
                        "<td colspan=" + comillas + "11" + comillas + "> " & " </td>" & _
                      "</tr>" & _
                     " <tr> " & _
                        "<td> ENTIDAD PRODUCTORA " & " </td>" & _
                        "<td colspan=" + comillas + "11" + comillas + "> " & nombre_entidad & " </td>" & _
                      "</tr>" & _
                      " <tr> " & _
                        "<td> OFICINA PRODUCTORA " & " </td>" & _
                        "<td colspan=" + comillas + "11" + comillas + "> " & ofic_product & " </td>" & _
                      "</tr>" & _
                       " </table>")
            html.Append("<table CELLSPACING=0 CELLPADDING=1 border=1> " & _
    " <tr> " & _
     "  <td width=" + comillas & "" + comillas + "; colspan=" + comillas & "3" + comillas + " ; align=" + comillas + "middle" + comillas + "> CODIGO </td> " & _
     " <td rowspan=" + comillas + "2" + comillas + " ; align=" + comillas + "middle" + comillas + "> NOMBRE DE SERIE Y SUB SERIES <br> Y TIPOS DOCUMENTALES </td>" & _
     " <td colspan=" + comillas + "2" + comillas + " ; align=" + comillas + "middle" + comillas + "> SIG <br>  </td>" & _
     " <td colspan=" + comillas + "1" + comillas + " ; align=" + comillas + "middle" + comillas + "> MEDIO <br>  </td>" & _
     " <td colspan=" + comillas + "2" + comillas + " ; align=" + comillas + "middle" + comillas + "> RETENCION <br> EN AÑOS </td> " & _
     " <td colspan=" + comillas + "4" + comillas + " ; align=" + comillas + " middle" + comillas + "> DISPOSICION <br> FINAL </td> " & _
     " <td width=" + comillas + "" + comillas + " ; rowspan=" + comillas + "2" + comillas + " ;  align=" + comillas + "middle" + comillas + "> OBSERVACIONES </td> " & _
    " </tr> " & _
    " <tr> " & _
     "  <td align=" + comillas + "middle" + comillas + "> DEP. </td> " & _
     "  <td align=" + comillas + "middle" + comillas + "> SERIE </td> " & _
     "  <td align=" + comillas + "middle" + comillas + "> SUB SERIE </td> " & _
      "  <td align=" + comillas + "middle" + comillas + "> PROCESO <br>  </td>" & _
     "  <td align=" + comillas + "middle" + comillas + "> PROCEDIMIENTO  </td>" & _
     "  <td align=" + comillas + "middle" + comillas + "> PAPEL/ELECTRONIC  </td>" & _
     "  <td align=" + comillas + "middle" + comillas + "> ARCHIVO <br> GESTION </td>" & _
     "  <td align=" + comillas + "middle" + comillas + "> ARCHIVO <br> CENTRAL </td>" & _
     "  <td align=" + comillas + "middle" + comillas + "> CT </td>" & _
     "  <td align=" + comillas + "middle" + comillas + "> E </td> " & _
     "  <td align=" + comillas + "middle" + comillas + "> M </td> " & _
     "  <td align=" + comillas + "middle" + comillas + "> S </td> " & _
     "  </tr> ")

            Contenido_Serie_Subserie(Id_Areadep, consecutivo_serie, area_depart, html)
            html.Append("<tr>" & "<td colspan=" + comillas + "14" + comillas + " ; align=" + comillas + "middle" + comillas + "> " & "CONVENCIONES" & " </td>" _
                       & "  </tr> " & _
                       "<tr>" & "<td colspan=" + comillas + "3" + comillas + " ; align=" + comillas + "middle" + comillas + "> " & "CT:Conservación Total <br> E:Eliminación <br> MT: Medios Tecnólogicos <br> S: Selección " & " </td>" _
                       & "<td colspan=" + comillas + "11" + comillas + " ; align=" + comillas + "middle" + comillas + "> " & _
                       "NOMBRE RESPONSABLE  DE LA DEPENDENCIA: ________________________  &nbsp&nbsp&nbsp&nbsp FIRMA RESPONSABLE  DE LA DEPENDENCIA:_______________________" & _
                       "<br> RESPONSABLE DE LA SUBDIRECCIÓN DE GESTIÓN DOCUMENTAL: ________________________ &nbsp&nbsp&nbsp&nbsp RESPONSABLE DE LA SUBDIRECCIÓN DE GESTIÓN DOCUMENTAL:_______________________" & " </td> </tr>")
            html.Append("</table>")
            html.Append("</body>")
            html.Append("</html>")
            w.Write(html.ToString())
            w.Close()
            Hidden_ruta_archivo.Value = HttpContext.Current.Session.Item("GA_RUTA_TEMPO") & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "/DESCARGA/" & "table_instrumento.xls"
            ifmExcel.Attributes.Add("src", "../radicador/WebFormDescargaRadicado.aspx")
            updatapanel_iframe.Update()
            Export = "YES"
        Catch ex As Exception
            Export = "Inconsistencia general funcion " & ex.Message
        Finally
            If Not w Is Nothing Then
                w.Close()
            End If
        End Try
    End Function
    Function Contenido_Serie_Subserie(ByVal Id_Areadep As Integer, _
                                      ByVal consecutivo_serie As Integer, _
                                      ByVal codigo_depart As String, _
                                      ByRef html As StringBuilder) As String
        Try
            Dim Matri_stru_series() As stru_serie_documental
            Dim Result As String = ""
            Dim Refclas As New ClassTrdDocumental
            Dim i As Integer = 0
            Erase Matri_stru_series
            Dim htmlCodigoSubserie As New StringBuilder()
            Dim htmlNombreSubserie As New StringBuilder()
            Dim comillas As String = Char.ConvertFromUtf32(34)
            '**********************************************
            'Lista series documentales
            '**********************************************
            Result = Refclas.Listar_Series_Documentales(Id_Areadep, Matri_stru_series)
            If Result <> "YES" Then
                Contenido_Serie_Subserie = Result
                Exit Function
            End If
            If Matri_stru_series Is Nothing Then
                Contenido_Serie_Subserie = "YES"
                Exit Function
            End If

            For i = 0 To Matri_stru_series.Length - 1

                '*****************************
                'Agrega codigo de Serie
                '*****************************
                html.Append("<tr>")
                '------Codigo departamento
                html.Append("<td valign=" + comillas + "TOP" + comillas + "; align=" + comillas + "center" + comillas + ">" & "<b>" & codigo_depart & "/" & "</b> </td> ")
                '------Serie documental
                html.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "center" + comillas + ">" & "<b>" & Matri_stru_series(i).Codigo_Arbitrario & "." & "</b> </td> ")

                '*******************************
                'Busca tipos documentales para 
                'la serie documental
                '*******************************
                Dim ResultA_s As String = ""
                Dim Matri_Tipo_doc_S() As String
                Erase Matri_Tipo_doc_S
                Dim Tipos As String = ""
                '*************************************
                'Agrega tipos documentales
                '*************************************
                ResultA_s = Refclas.Listar_Tipdoc_Series(Matri_stru_series(i).Id_Series, Matri_Tipo_doc_S)
                If Not Matri_Tipo_doc_S Is Nothing Then
                    Dim Matri_Tempo_Tipodoc() As String
                    For k As Integer = 0 To Matri_Tipo_doc_S.Length - 1
                        Erase Matri_Tempo_Tipodoc
                        Matri_Tempo_Tipodoc = Matri_Tipo_doc_S(k).Split("|")
                        Tipos = Tipos & "-" & Matri_Tempo_Tipodoc(1) & "<br>"
                    Next
                    '-------------- Agrega nombre serie documental con tipos documentales 
                    html.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "center" + comillas + ">  </td>")
                    html.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "left" + comillas + ">" & "<b>" & Matri_stru_series(i).Nombre_Serie & "</b>" & "<br>" & Tipos)
                    html.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "center" + comillas + ">  </td>")
                    'html.Append("<td align=" + comillas + "middle" + comillas + ">" & Matri_sPLIT(1) & "<br>" & Tipos)
                Else
                    'html.Append("<td align=" + comillas + "middle" + comillas + ">" & Matri_sPLIT(1))
                    html.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "center" + comillas + ">  </td>")
                    html.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "left" + comillas + ">" & "<b>" & Matri_stru_series(i).Nombre_Serie & "</b>")
                    html.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "center" + comillas + ">  </td>")
                End If
                html.Append("</td>")
                '-------------------------------------
                'Agrega disposición final de la serie
                '-------------------------------------
                If Matri_stru_series(i).ESTADO_DESICION = 1 Then
                    html.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "center" + comillas + ">" & "<b>" & Matri_stru_series(i).Proceso & " </b> </td> ")
                    html.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "center" + comillas + ">" & "<b>" & Matri_stru_series(i).Procedimiento & " </b> </td> ")
                    html.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "center" + comillas + ">" & "<b>" & Matri_stru_series(i).Medio_soporte & " </b> </td> ")
                    html.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "center" + comillas + ">" & "<b>" & Matri_stru_series(i).Tiempo_Ret_Arch_Gestion & "Años </b>  </td> ")
                    html.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "center" + comillas + ">" & "<b>" & Matri_stru_series(i).Tiempo_Ret_Arch_Central & "Años </b> </td> ")

                    If Matri_stru_series(i).Conservacion_Total = 1 Then
                        html.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "center" + comillas + ">" & "<b>" & "X" & " </b> </td> ")
                    Else
                        html.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "center" + comillas + ">  </td>")
                    End If
                    If Matri_stru_series(i).Eliminacion = 1 Then
                        html.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "center" + comillas + ">" & "<b>" & "X" & " </b> </td> ")
                    Else
                        html.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "center" + comillas + ">  </td>")
                    End If
                    If Matri_stru_series(i).Microfilm = 1 Then
                        html.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "center" + comillas + ">" & "<b>" & "X" & " </b> </td> ")
                    Else
                        html.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "center" + comillas + ">  </td>")
                    End If
                    If Matri_stru_series(i).Seleccion = 1 Then
                        html.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "center" + comillas + ">" & "<b>" & "X" & " </b> </td> ")
                    Else
                        html.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "center" + comillas + ">  </td>")
                    End If
                    If Matri_stru_series(i).observaciones <> "" Then
                        html.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "left" + comillas + ">" & "<b>" & Matri_stru_series(i).observaciones & "</b> </td> ")
                    Else
                        html.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "left" + comillas + ">  </td>")
                    End If
                Else
                    html.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "center" + comillas + ">  </td>")
                    html.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "center" + comillas + ">  </td>")
                    html.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "center" + comillas + ">  </td>")
                    html.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "center" + comillas + ">  </td>")
                    html.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "center" + comillas + ">  </td>")
                    html.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "center" + comillas + ">  </td>")
                    html.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "center" + comillas + ">  </td>")
                    html.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "center" + comillas + ">  </td>")
                    html.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "center" + comillas + ">  </td>")
                End If
                html.Append("</tr>")
                'html.Append("<tr>")
                Contenido_Sub_Series_Tipos_Documentales(Matri_stru_series(i).Id_Series, html, Matri_stru_series(i).Codigo_Arbitrario, codigo_depart)
                'html.Append("</tr>")
            Next
            Contenido_Serie_Subserie = "YES"
        Catch ex As Exception
            Contenido_Serie_Subserie = "Inconsistencia general función Contenido_Serie_Subserie " & ex.Message
        End Try
    End Function
    Function Contenido_Sub_Series_Tipos_Documentales(ByVal Id_Serie As Integer, _
                                                     ByRef htmlNombreSubserie As StringBuilder, _
                                                     ByVal Consecutivo_Serie As String, _
                                                     ByVal cod_departamento As String) As String
        Try
            Dim matri_stru_sub_serie() As stru_sub_serie_documental
            Dim Resultado_User As String = ""
            Erase matri_stru_sub_serie
            Dim Result As String = ""
            Dim Refclas As New ClassTrdDocumental
            Dim comillas As String = Char.ConvertFromUtf32(34)
            '***********************************
            'Lista sub series documentales
            '***********************************
            Resultado_User = Refclas.Listar_SubSeries_Documentales(Id_Serie, matri_stru_sub_serie)
            If Resultado_User = "YES" Then
                If Not (matri_stru_sub_serie) Is Nothing Then
                    For z As Integer = 0 To matri_stru_sub_serie.Length - 1
                        htmlNombreSubserie.Append("<tr>")
                        htmlNombreSubserie.Append("<td valign=" + comillas + "TOP" + comillas + "; align=" + comillas + "center" + comillas + ">" & "<b>" & cod_departamento & "/" & "</b> </td> ")
                        htmlNombreSubserie.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "center" + comillas + ">" & "<b>" & Consecutivo_Serie & "</b> </td> ")
                        htmlNombreSubserie.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "center" + comillas + ">" & "<b>" & matri_stru_sub_serie(z).Codigo_Arbitrario & "</b> </td> ")
                        'html.Append("<td align=" + comillas + "middle" + comillas + ">" & Matri_Reportes(1) & " </td> ")
                        htmlNombreSubserie.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "center " + comillas + ">" + "<b>" & matri_stru_sub_serie(z).Nombre_Subserie & "</b>")

                        '****************************************
                        'Busca tipos documentales
                        '****************************************
                        Dim ResultA As String = ""
                        Dim Matri_Tipo_doc() As String
                        Erase Matri_Tipo_doc
                        ResultA = Refclas.Listar_Tipdoc_Subseries(matri_stru_sub_serie(z).Id_SubSeries, Matri_Tipo_doc)
                        If ResultA <> "YES" Then
                            Contenido_Sub_Series_Tipos_Documentales = ResultA
                            Exit Function
                        End If
                        If Not Matri_Tipo_doc Is Nothing Then
                            Dim Matri_Tempo_Tipodoc() As String
                            For k As Integer = 0 To Matri_Tipo_doc.Length - 1
                                Erase Matri_Tempo_Tipodoc
                                Matri_Tempo_Tipodoc = Matri_Tipo_doc(k).Split("|")
                                htmlNombreSubserie.Append("<br>" & "-" & Matri_Tempo_Tipodoc(1))
                            Next
                        End If
                        htmlNombreSubserie.Append("</td>")
                        '-------------------------------------
                        'Agrega disposición final de la serie
                        '-------------------------------------
                        If matri_stru_sub_serie(z).ESTADO_DESICION = 1 Then
                            htmlNombreSubserie.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "center" + comillas + ">" & "<b>" & matri_stru_sub_serie(z).Proceso & " </b> </td> ")
                            htmlNombreSubserie.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "center" + comillas + ">" & "<b>" & matri_stru_sub_serie(z).Procedimiento & " </b> </td> ")
                            htmlNombreSubserie.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "center" + comillas + ">" & "<b>" & matri_stru_sub_serie(z).Medio_soporte & " </b> </td> ")
                            htmlNombreSubserie.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "center" + comillas + " >" & "<b>" & matri_stru_sub_serie(z).TIEMPO_RET_ARCH_GESTION & "Años </b> </td> ")
                            htmlNombreSubserie.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "center" + comillas + " >" & "<b>" & matri_stru_sub_serie(z).TIEMPO_RET_ARCH_CENTRAL & "Años </b> </td> ")
                            If matri_stru_sub_serie(z).CONSERVACION_TOTAL = 1 Then
                                htmlNombreSubserie.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "center" + comillas + " >" & "<b>" & "X" & " </b> </td> ")
                            Else
                                htmlNombreSubserie.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "center" + comillas + " >  </td>")
                            End If
                            If matri_stru_sub_serie(z).ELIMINACION = 1 Then
                                htmlNombreSubserie.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "center" + comillas + " >" & "<b>" & "X" & "</b> </td> ")
                            Else
                                htmlNombreSubserie.Append("<td valign=" + comillas + "TOP" + comillas + "; align=" + comillas + "center" + comillas + " >  </td>")
                            End If
                            If matri_stru_sub_serie(z).MICROFILM = 1 Then
                                htmlNombreSubserie.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "center" + comillas + " >" & "<b>" & "X" & "</b></td> ")
                            Else
                                htmlNombreSubserie.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "center" + comillas + " >  </td>")
                            End If
                            If matri_stru_sub_serie(z).SELECCION = 1 Then
                                htmlNombreSubserie.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "center" + comillas + " >" & "<b>" & "X" & "</b></td> ")
                            Else
                                htmlNombreSubserie.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "center" + comillas + " >  </td>")
                            End If
                            If matri_stru_sub_serie(z).observaciones <> "" Then
                                htmlNombreSubserie.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "left" + comillas + " >" & "<b>" & matri_stru_sub_serie(z).observaciones & "</b> </td> ")
                            Else
                                htmlNombreSubserie.Append("<td " + ">  </td>")
                            End If
                        Else
                            htmlNombreSubserie.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "center" + comillas + " >  </td>")
                            htmlNombreSubserie.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "center" + comillas + " >  </td>")
                            htmlNombreSubserie.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "center" + comillas + " >  </td>")
                            htmlNombreSubserie.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "center" + comillas + " >  </td>")
                            htmlNombreSubserie.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "center" + comillas + " >  </td>")
                            htmlNombreSubserie.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "center" + comillas + " >  </td>")
                            htmlNombreSubserie.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "center" + comillas + " >  </td>")
                            htmlNombreSubserie.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "center" + comillas + " >  </td>")
                            htmlNombreSubserie.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "center" + comillas + " >  </td>")
                            'htmlNombreSubserie.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "center" + comillas + " >  </td>")
                            'htmlNombreSubserie.Append("<td valign=" + comillas + "TOP" + comillas + " ; align=" + comillas + "center" + comillas + " >  </td>")
                        End If
                        htmlNombreSubserie.Append("</tr>")

                    Next
                End If
            End If
            Contenido_Sub_Series_Tipos_Documentales = "YES"
        Catch ex As Exception
            Contenido_Sub_Series_Tipos_Documentales = "Inconsistencia general función Contenido_Sub_Series_Tipos_Documentales " & ex.Message
        End Try
    End Function
    Function Solicita_codigo_area_departamento(ByVal id_area As Integer, _
                                               ByRef Codigo_area As String) As String
        Try
            Dim Parametro_Consulta As String = "Select Codigo_Arbitrario " & _
        " from areas_depart_radicacion where Codigo_Area=" & id_area
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipo_doc_series")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_codigo_area_departamento = "Función Solicita_codigo_area_departamento dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Codigo_area = 0
                Solicita_codigo_area_departamento = "Imposible encontrar el código del area (" & id_area & ")"
                Exit Function
            Else
                Codigo_area = Datset.Tables(0).Rows(0).Item(0)
                Solicita_codigo_area_departamento = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_codigo_area_departamento = "Inconsistencia general función Solicita_codigo_area_departamento " & ex.Message
        End Try
    End Function
    Function Selecciona_elemento_treview_tabla(ByRef ref_TreeView1 As TreeView, _
                                               ByRef ref_update As UpdatePanel, _
                                               ByVal texto_busqueda As String) As String
        Try

            If ref_TreeView1 Is Nothing Then
                Selecciona_elemento_treview_tabla = "Imposible encontrar control TreeView1 "
                Exit Function
            End If
            Dim Result As String = ""
            Dim Ref As New ClassGestorSesion
            Dim Matri_Nodo() As String
            Erase Matri_Nodo
            Dim Datos_Nodo As String = ""
            'consulta dato de nodo seleccionado
            If texto_busqueda = "" Then
                Result = Ref.NodoChild_Selecionado(ref_TreeView1, _
                                                   Datos_Nodo)
                If Result <> "YES" Then
                    Selecciona_elemento_treview_tabla = Result
                    Exit Function
                Else
                    Matri_Nodo = Split(Datos_Nodo, "|")
                End If
            Else
                Result = Ref.NodoChild_Selecionado_busqueda(ref_TreeView1, _
                                                            Datos_Nodo, _
                                                            texto_busqueda)
                If Result <> "YES" Then
                    Selecciona_elemento_treview_tabla = Result
                    Exit Function
                Else
                    Matri_Nodo = Split(Datos_Nodo, "|")
                End If
            End If

            Selecciona_elemento_treview_tabla = "YES"
        Catch ex As Exception
            Selecciona_elemento_treview_tabla = "Inconsistencia general función Selecciona_elemento_treview_tabla " & ex.Message
        Finally
            ref_update.Update()
        End Try
    End Function
    Function Solicita_id_instrumento_serie_documental(ByVal id_serie As Integer, _
                                                      ByRef id_instrumento As Integer) As String
        Try
            Dim Parametro_Consulta As String = "Select Ra_registro_instrumento_archivistico_id_instrumento " & _
            " from series_documentales where Id_Series=" & id_serie
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("SERIES_DOCUMENTALES")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_instrumento_serie_documental = "Función  Solicita_id_instrumento_serie_documental dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_id_instrumento_serie_documental = "Imposible encontrar el registro de la serie documental (" & id_serie & ")"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) Then
                    Solicita_id_instrumento_serie_documental = "El registro del instrumento de la serie documental (" & id_serie & ") es null"
                    Exit Function
                Else
                    id_instrumento = Datset.Tables(0).Rows(0).Item(0)
                    Solicita_id_instrumento_serie_documental = "YES"
                    Exit Function
                End If
            End If
        Catch ex As Exception
            Solicita_id_instrumento_serie_documental = "Inconsistencia general función Solicita_id_instrumento_serie_documental " & ex.Message
        End Try
    End Function
  
    
End Class
