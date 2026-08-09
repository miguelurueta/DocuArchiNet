Imports Antlr.Runtime
Imports GestionDocumental_Docuarchi.net.WebServiceRadicacion

Public Structure stru_chek_lista_tramite
    Dim ID_TIPO_DOCUMENTAL_CHEQUEO As Integer
    Dim Descripcion_Documento As String
    Dim OBLIGATORIO As Integer
    Dim estado_cumple As Integer
End Structure
Public Structure stru_tipo_lista_chequeo
    Dim tipo_doc_entrante_id_Tipo_Doc_Entrante As Integer
    Dim tipo_doc_saliente_id_Tip_Doc_Saliente As Integer
    Dim series_documentales_Id_Series As Integer
    Dim tipo_doc_series_Id_Tipo_Doc_Series As Integer
    Dim subseries_documentales_Id_SubSeries As Integer
    Dim tipos_doc_subseries_Id_Tipos_Doc_SubSerie As Integer
    Dim TIPO_TRAMITE As Integer
    Dim OBLIGATORIO As Integer
    Dim UNICO As Integer
    Dim ORDEN_LISTA As Integer
    Dim nombre_tipo As String
End Structure
Public Structure StruTiposExpedienteSegundarioSII
    Dim IdTipo As Integer
End Structure
Public Class ra_dig_tipos_docum_lista_chequeo
    Function SolicitaListaTiposExpedienteSegundarioSii(ByVal IdTipoTramite As Integer,
                                                       ByRef StruTiposExpedienteSegundarioSII() As StruTiposExpedienteSegundarioSII) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita tipos documentales de expedientes seegudarios SII
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdTipoTramite       : Representa la identificación tramite
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'StruTiposExpedienteSegundarioSII  : Retorna la estructura de los tipos
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-06-20
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim SqlConsulta As String = "Select tipo_doc_series_Id_Tipo_Doc_Series from  ra_dig_tipos_docum_lista_chequeo " &
                " where tipo_doc_entrante_id_Tipo_Doc_Entrante=" & IdTipoTramite & " and UtilExpedienteRelacionado=1"
            Dim DataBaseConexion As New conect.Dbase_Conction_Mysql_RA
            Dim DataSet As New DataSet
            Result = DataBaseConexion.SELECTION_SELECT_FIELD(SqlConsulta,
                                                             DataSet)
            If Result <> "YES" Then
                SolicitaListaTiposExpedienteSegundarioSii = " Función Solicita_id_lista_chequeo dice  " & Result
                Exit Function
            End If
            If DataSet.Tables(0).Rows.Count = 0 Then
                StruTiposExpedienteSegundarioSII = Nothing
                SolicitaListaTiposExpedienteSegundarioSii = "YES"
                Exit Function
            Else
                For i As Integer = 0 To DataSet.Tables(0).Rows.Count - 1
                    ReDim Preserve StruTiposExpedienteSegundarioSII(i)
                    StruTiposExpedienteSegundarioSII(i).IdTipo = DataSet.Tables(0).Rows(i).Item(0)
                Next
                SolicitaListaTiposExpedienteSegundarioSii = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaListaTiposExpedienteSegundarioSii = "Inconsistencia general fucnción SolicitaListaTiposExpedienteSegundarioSii " & ex.Message
        End Try
    End Function
    Function Solicita_id_lista_chequeo_default_radicado(ByVal id_tipo_tramite As Integer,
                                                         ByRef id_lista_cheq As Integer) As String
        '-------------------------------------------------------------------
        'Funcion : Solicita la identificación del documento de la lista de
        'chequeo predeterminado para radicacion
        'Fecha : 2019-02-02
        'Ingeniero : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------------
        Try

            Dim Parametro_Consulta As String = " SELECT ID_TIPO_DOCUMENTAL_CHEQUEO " &
           " FROM  ra_dig_tipos_docum_lista_chequeo " &
           " where tipo_doc_entrante_id_Tipo_Doc_Entrante=" & id_tipo_tramite &
           " and DEFAULT_DOC_RADICADO=1"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Dat_set As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta,
                                                          Dat_set)
            If Result <> "YES" Then
                Solicita_id_lista_chequeo_default_radicado = " Función Solicita_id_lista_chequeo dice  " & Result
                Exit Function
            End If
            If Dat_set.Tables(0).Rows.Count = 0 Then
                Solicita_id_lista_chequeo_default_radicado = "YES"
                id_lista_cheq = 0
                Exit Function
            Else
                Solicita_id_lista_chequeo_default_radicado = "YES"
                id_lista_cheq = Dat_set.Tables(0).Rows(0).Item(0)
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_lista_chequeo_default_radicado = "Incosistencia general función Solicita_datos_tipo_documento_tramite " & ex.Message
        End Try
    End Function
    Function Solicita_id_lista_chequeo_default_respuesta(ByVal id_tipo_tramite As Integer,
                                                         ByRef id_lista_cheq As Integer) As String
        '-------------------------------------------------------------------
        'Funcion : Solicita la identificación del documento de la lista de
        'chequeo predeterminado para dar respuesta
        'Fecha : 2019-02-04
        'Ingeniero : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------------
        Try

            Dim Parametro_Consulta As String = " SELECT ID_TIPO_DOCUMENTAL_CHEQUEO " &
           " FROM  ra_dig_tipos_docum_lista_chequeo " &
           " where tipo_doc_entrante_id_Tipo_Doc_Entrante=" & id_tipo_tramite &
           " and DEFAULT_DOC_RESPUESTA=1"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Dat_set As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta,
                                                          Dat_set)
            If Result <> "YES" Then
                Solicita_id_lista_chequeo_default_respuesta = " Función Solicita_id_lista_chequeo_default_respuesta dice  " & Result
                Exit Function
            End If
            If Dat_set.Tables(0).Rows.Count = 0 Then
                Solicita_id_lista_chequeo_default_respuesta = "YES"
                id_lista_cheq = 0
                Exit Function
            Else
                Solicita_id_lista_chequeo_default_respuesta = "YES"
                id_lista_cheq = Dat_set.Tables(0).Rows(0).Item(0)
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_lista_chequeo_default_respuesta = "Incosistencia general función Solicita_id_lista_chequeo_default_respuesta " & ex.Message
        End Try
    End Function
    Function Solicita_id_tipologia_lista_chequeo_rotulo_radicado(ByVal id_tipo_tramite As Integer,
                                                                 ByRef tipo_doc_entrante_id_Tipo_Doc_Entrante As Integer) As String
        '-------------------------------------------------------------------
        'Funcion : Solicita la identificación del documento de la lista de
        'chequeo predeterminado como rotulo radicado
        'Fecha : 2021-04-28
        'Ingeniero : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------------
        Try

            Dim Parametro_Consulta As String = " SELECT tipo_doc_series_Id_Tipo_Doc_Series " &
           " FROM  ra_dig_tipos_docum_lista_chequeo " &
           " where tipo_doc_entrante_id_Tipo_Doc_Entrante=" & id_tipo_tramite &
           " and TIPO_CAMPO_RADICADO=1"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Dat_set As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta,
                                                          Dat_set)
            If Result <> "YES" Then
                Solicita_id_tipologia_lista_chequeo_rotulo_radicado = " Función Solicita_id_tipologia_lista_chequeo_rotulo_radicado dice  " & Result
                Exit Function
            End If
            If Dat_set.Tables(0).Rows.Count = 0 Then
                Solicita_id_tipologia_lista_chequeo_rotulo_radicado = "YES"
                tipo_doc_entrante_id_Tipo_Doc_Entrante = 0
                Exit Function
            Else
                Solicita_id_tipologia_lista_chequeo_rotulo_radicado = "YES"
                tipo_doc_entrante_id_Tipo_Doc_Entrante = Dat_set.Tables(0).Rows(0).Item(0)
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_tipologia_lista_chequeo_rotulo_radicado = "Incosistencia general función Solicita_id_tipologia_lista_chequeo_rotulo_radicado " & ex.Message
        End Try
    End Function
    Function Solicita_listar_tipos_documentales_relacionados_edita_tramite_lista_adjunta(
                                                                                         ByVal id_tipo_tramite As Integer, ByVal _
                                                                                         tipo_plantilla_tramite_radicado As String,
                                                                                         ByRef page1 As Page) As String
        Try
            Dim Campo As String = ""
            Dim tipo_plantilla_tramite As String = ""
            If tipo_plantilla_tramite_radicado = "RADICACION ENTRANTE" Or tipo_plantilla_tramite_radicado = "" Then
                tipo_plantilla_tramite = "tipo_doc_entrante_id_Tipo_Doc_Entrante"
            Else
                tipo_plantilla_tramite = "tipo_doc_saliente_id_Tip_Doc_Saliente"
            End If
            Dim Result As String = ""
            Dim Parametro_Consulta As String = " SELECT rdt.ID_TIPO_DOCUMENTAL_CHEQUEO,tds.Descripcion_Documento " &
            " FROM  ra_dig_tipos_docum_lista_chequeo as  rdt" &
            " inner join tipo_doc_series as tds on (tds.Id_Tipo_Doc_Series=rdt.tipo_doc_series_Id_Tipo_Doc_Series)" &
            " where " & tipo_plantilla_tramite & "=" & id_tipo_tramite &
            " order by rdt.ORDEN_LISTA "
            Dim scripma As GridView = page1.FindControl("data_grid_chequeo_actualiza")
            Dim ref_UpdateGeneral_documentos As UpdatePanel = page1.FindControl("UpdateGeneral_actualiza")
            Dim hideselecion As Object = page1.FindControl("Hidden_0003")
            Dim Datset_consulta As DataSet = New DataSet("ra_dig_tipos_docum_lista_chequeo")
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset_consulta)
            HttpContext.Current.Session.Item("DATA_SET_SESION") = Datset_consulta
            If Result <> "YES" Then
                Solicita_listar_tipos_documentales_relacionados_edita_tramite_lista_adjunta = "Error listando datos lista de chequeo" & Result
                Exit Function
            End If
            If Datset_consulta.Tables(0).Rows.Count = 0 Then
                scripma.DataSource = Datset_consulta
                hideselecion.value = "-1"
                scripma.DataBind()
                ref_UpdateGeneral_documentos.Update()
                Solicita_listar_tipos_documentales_relacionados_edita_tramite_lista_adjunta = "YES"
                Exit Function
            Else
                scripma.DataSource = Datset_consulta
                hideselecion.value = "-1"
                scripma.DataBind()
                For i As Integer = 0 To scripma.Rows.Count - 1
                    scripma.Rows(i).Attributes.Add("id", scripma.Rows(i).Cells(0).Text.ToString())
                Next
                ref_UpdateGeneral_documentos.Update()
                Solicita_listar_tipos_documentales_relacionados_edita_tramite_lista_adjunta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_listar_tipos_documentales_relacionados_edita_tramite_lista_adjunta = "Inconsistencia general función Solicita_listar_tipos_documentales_relacionados_edita_tramite_lista_adjunta " & ex.Message
        End Try
    End Function
    Function SolicitaListaTiposDocumentalesTramiteListaAdjunta(ByVal IdTipoTramite As Integer,
                                                               ByVal TipoPlantillaTramiteRadicado As String,
                                                               ByRef control_drow_lista As List(Of control_drow_lista)) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita lista de tipos documentales relacionados a un tipo 
        '          tramite
        '        
        '
        '       
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'IdTipoTramite          : Representa la identificación del tipo tramite
        '---------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------
        'control_drow_lista        : Retorna la lista tipos documentales
        '                     value: identificación del tipo documento
        '                      text: Nombre del tipo documento
        '
        '-----------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------
        'Fecha                 : 2025-04-18
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim tipo_plantilla_tramite As String = ""
            If TipoPlantillaTramiteRadicado = "RADICACION ENTRANTE" Or TipoPlantillaTramiteRadicado = "" Then
                tipo_plantilla_tramite = "tipo_doc_entrante_id_Tipo_Doc_Entrante"
            Else
                tipo_plantilla_tramite = "tipo_doc_saliente_id_Tip_Doc_Saliente"
            End If
            Dim SQLConsulta As String = " SELECT rdt.ID_TIPO_DOCUMENTAL_CHEQUEO,tds.Descripcion_Documento " &
            " FROM  ra_dig_tipos_docum_lista_chequeo as  rdt" &
            " inner join tipo_doc_series as tds on (tds.Id_Tipo_Doc_Series=rdt.tipo_doc_series_Id_Tipo_Doc_Series)" &
            " where " & tipo_plantilla_tramite & "=" & IdTipoTramite &
            " order by tds.Descripcion_Documento "
            Dim DataSet As DataSet = New DataSet("ra_dig_tipos_docum_lista_chequeo")
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Result = ref.SELECTION_SELECT_FIELD(SQLConsulta, DataSet)
            If Result <> "YES" Then
                SolicitaListaTiposDocumentalesTramiteListaAdjunta = "Error listando datos lista de chequeo (" & Result & ")"
                Exit Function
            End If
            Dim item As control_drow_lista
            If DataSet.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To DataSet.Tables(0).Rows.Count - 1
                    item = New control_drow_lista
                    item.value = DataSet.Tables(0).Rows(i).Item(0)
                    item.text = DataSet.Tables(0).Rows(i).Item(1)
                    control_drow_lista.Add(item)
                Next
                SolicitaListaTiposDocumentalesTramiteListaAdjunta = "YES"
                Exit Function
            Else
                SolicitaListaTiposDocumentalesTramiteListaAdjunta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaListaTiposDocumentalesTramiteListaAdjunta = "Inconsistencia general funcion SolicitaListaTiposDocumentalesTramiteListaAdjunta " & ex.Message
        End Try
    End Function
    Function Solicita_listar_tipos_documentales_relacionados_edita_tramite_lista_adjunta_drowlist(ByVal id_tipo_tramite As Integer,
                                                                                                  ByVal tipo_plantilla_tramite_radicado As String,
                                                                                                  ByVal nombre As String,
                                                                                                  ByRef Droplist As DropDownList,
                                                                                                  ByRef update As UpdatePanel,
                                                                                                  ByRef estado_resultado As String) As String
        Try
            Dim Campo As String = ""
            Droplist.Items.Clear()
            Dim tipo_plantilla_tramite As String = ""
            If tipo_plantilla_tramite_radicado = "RADICACION ENTRANTE" Or tipo_plantilla_tramite_radicado = "" Then
                tipo_plantilla_tramite = "tipo_doc_entrante_id_Tipo_Doc_Entrante"
            Else
                tipo_plantilla_tramite = "tipo_doc_saliente_id_Tip_Doc_Saliente"
            End If
            Dim Result As String = ""
            Dim Parametro_Consulta As String = " SELECT rdt.ID_TIPO_DOCUMENTAL_CHEQUEO,tds.Descripcion_Documento " &
            " FROM  ra_dig_tipos_docum_lista_chequeo as  rdt" &
            " inner join tipo_doc_series as tds on (tds.Id_Tipo_Doc_Series=rdt.tipo_doc_series_Id_Tipo_Doc_Series)" &
            " where " & tipo_plantilla_tramite & "=" & id_tipo_tramite &
            " order by tds.Descripcion_Documento "
            Dim Datset_consulta As DataSet = New DataSet("ra_dig_tipos_docum_lista_chequeo")
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset_consulta)
            HttpContext.Current.Session.Item("DATA_SET_SESION") = Datset_consulta
            If Result <> "YES" Then
                Solicita_listar_tipos_documentales_relacionados_edita_tramite_lista_adjunta_drowlist = "Error listando datos lista de chequeo" & Result
                Exit Function
            End If
            If Datset_consulta.Tables(0).Rows.Count = 0 Then
                Droplist.Items.Clear()
                update.Update()
                estado_resultado = "NO"
                Solicita_listar_tipos_documentales_relacionados_edita_tramite_lista_adjunta_drowlist = "YES"
                Exit Function
            Else
                estado_resultado = "YES"
                Dim ilis_ As System.Web.UI.WebControls.ListItem
                ilis_ = New System.Web.UI.WebControls.ListItem
                ilis_.Text = ""
                ilis_.Value = -1
                Droplist.Items.Add(ilis_)
                Dim ref_class_gabinete As New ClassDaGabinete
                For i As Integer = 0 To Datset_consulta.Tables(0).Rows.Count - 1
                    ilis_ = New System.Web.UI.WebControls.ListItem
                    Dim value_documento As String = ""
                    Result = ref_class_gabinete.RemoveDiacritics(Datset_consulta.Tables(0).Rows(i).Item(1),
                                                                 value_documento)
                    ilis_.Text = value_documento
                    ilis_.Value = Datset_consulta.Tables(0).Rows(i).Item(0)
                    Droplist.Items.Add(ilis_)
                Next
                For i As Integer = 0 To Droplist.Items.Count - 1
                    If Droplist.Items(i).Text = nombre Then
                        Droplist.Items(i).Selected = True
                    End If
                Next
                update.Update()
                Solicita_listar_tipos_documentales_relacionados_edita_tramite_lista_adjunta_drowlist = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_listar_tipos_documentales_relacionados_edita_tramite_lista_adjunta_drowlist = "Inconsistencia general función Solicita_listar_tipos_documentales_relacionados_edita_tramite_lista_adjunta_drowlist " & ex.Message
        End Try
    End Function
    Function Solicita_listar_tipos_documentales_relacionados_edita_tramite_radicado_service(ByVal id_tipo_tramite As Integer,
                                                                                            ByVal tipo_plantilla_tramite_radicado As String,
                                                                                            ByVal nombre As String,
                                                                                            ByRef rad_drow_lista_ As List(Of control_drow_lista),
                                                                                            ByRef estado_resultado As String) As String
        Try
            Dim Campo As String = ""
            Dim tipo_plantilla_tramite As String = ""
            If tipo_plantilla_tramite_radicado = "RADICACION ENTRANTE" Or tipo_plantilla_tramite_radicado = "" Then
                tipo_plantilla_tramite = "tipo_doc_entrante_id_Tipo_Doc_Entrante"
            Else
                tipo_plantilla_tramite = "tipo_doc_saliente_id_Tip_Doc_Saliente"
            End If
            Dim Result As String = ""
            Dim Parametro_Consulta As String = " SELECT rdt.ID_TIPO_DOCUMENTAL_CHEQUEO,tds.Descripcion_Documento " &
            " FROM  ra_dig_tipos_docum_lista_chequeo as  rdt" &
            " inner join tipo_doc_series as tds on (tds.Id_Tipo_Doc_Series=rdt.tipo_doc_series_Id_Tipo_Doc_Series)" &
            " where " & tipo_plantilla_tramite & "=" & id_tipo_tramite &
            " order by tds.Descripcion_Documento "
            Dim Datset_consulta As DataSet = New DataSet("ra_dig_tipos_docum_lista_chequeo")
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset_consulta)
            HttpContext.Current.Session.Item("DATA_SET_SESION") = Datset_consulta
            If Result <> "YES" Then
                Solicita_listar_tipos_documentales_relacionados_edita_tramite_radicado_service = "Error listando datos lista de chequeo " & Result
                Exit Function
            End If
            If Datset_consulta.Tables(0).Rows.Count = 0 Then

                estado_resultado = "NO"
                Solicita_listar_tipos_documentales_relacionados_edita_tramite_radicado_service = "YES"
                Exit Function
            Else
                estado_resultado = "YES"
                Dim item As control_drow_lista
                If Datset_consulta.Tables(0).Rows.Count > 0 Then
                    item = New control_drow_lista
                    item.value = "0"
                    item.text = ""
                    rad_drow_lista_.Add(item)
                    For i As Integer = 0 To Datset_consulta.Tables(0).Rows.Count - 1
                        item = New control_drow_lista
                        item.value = Datset_consulta.Tables(0).Rows(i).Item(0)
                        item.text = Datset_consulta.Tables(0).Rows(i).Item(1)
                        rad_drow_lista_.Add(item)
                    Next
                    Solicita_listar_tipos_documentales_relacionados_edita_tramite_radicado_service = "YES"
                    Exit Function
                Else
                    Solicita_listar_tipos_documentales_relacionados_edita_tramite_radicado_service = "YES"
                    Exit Function
                End If

                Solicita_listar_tipos_documentales_relacionados_edita_tramite_radicado_service = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_listar_tipos_documentales_relacionados_edita_tramite_radicado_service = "Inconsistencia general función Solicita_listar_tipos_documentales_relacionados_edita_tramite_radicado_service " & ex.Message
        End Try
    End Function
    Function Solicita_listar_tipos_documentales_relacion_tramite_radicacion(ByVal id_tipo_tramite As Integer,
                                                                            ByVal tipo_plantilla_tramite_radicado As String,
                                                                            ByRef page1 As Page) As String
        Try
            Dim Campo As String = ""
            Dim tipo_plantilla_tramite As String = ""
            If tipo_plantilla_tramite_radicado = "RADICACION ENTRANTE" Or tipo_plantilla_tramite_radicado = "" Then
                tipo_plantilla_tramite = "tipo_doc_entrante_id_Tipo_Doc_Entrante"
            Else
                tipo_plantilla_tramite = "tipo_doc_saliente_id_Tip_Doc_Saliente"
            End If
            Dim Result As String = ""
            Dim Parametro_Consulta As String = " SELECT rdt.ID_TIPO_DOCUMENTAL_CHEQUEO,tds.Descripcion_Documento as TIPO,rdt.OBLIGATORIO " &
            " FROM  ra_dig_tipos_docum_lista_chequeo as  rdt" &
            " inner join tipo_doc_series as tds on (tds.Id_Tipo_Doc_Series=rdt.tipo_doc_series_Id_Tipo_Doc_Series)" &
            " where " & tipo_plantilla_tramite & "=" & id_tipo_tramite &
            " order by rdt.ORDEN_LISTA,rdt.OBLIGATORIO desc"
            Dim scripma As GridView = page1.FindControl("data_grid_chequeo_actualiza")
            Dim ref_UpdateGeneral_documentos As UpdatePanel = page1.FindControl("UpdateGeneral_actualiza")
            Dim hideselecion As Object = page1.FindControl("Hidden_0003")
            Dim Datset_consulta As DataSet = New DataSet("ra_dig_tipos_docum_lista_chequeo")
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset_consulta)
            If Result <> "YES" Then
                Solicita_listar_tipos_documentales_relacion_tramite_radicacion = "Error listando datos lista de chequeo" & Result
                Exit Function
            End If
            If Datset_consulta.Tables(0).Rows.Count = 0 Then
                scripma.DataSource = Datset_consulta
                hideselecion.value = "-1"
                scripma.DataBind()
                ref_UpdateGeneral_documentos.Update()
                Solicita_listar_tipos_documentales_relacion_tramite_radicacion = "YES"
                Exit Function
            Else
                scripma.DataSource = Datset_consulta
                hideselecion.value = "-1"
                scripma.DataBind()
                For i As Integer = 0 To scripma.Rows.Count - 1
                    scripma.Rows(i).Attributes.Add("id", scripma.Rows(i).Cells(1).Text.ToString())
                    If scripma.Rows(i).Cells(0).Controls.Count > 2 Then
                        Dim p As Object = scripma.Rows(i).Cells(0).Controls(1)
                        p.Attributes.Add("idd", scripma.Rows(i).Cells(1).Text.ToString())
                        p.CssClass = "r_p_check_box"
                    End If
                    If scripma.Rows(i).Cells(3).Text.ToString = "1" Then
                        scripma.Rows(i).Cells(3).Text = "SI"
                        scripma.Rows(i).ForeColor = Drawing.Color.Red
                    Else
                        scripma.Rows(i).Cells(3).Text = "NO"
                    End If
                Next
                ref_UpdateGeneral_documentos.Update()
                Solicita_listar_tipos_documentales_relacion_tramite_radicacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_listar_tipos_documentales_relacion_tramite_radicacion = "Inconsistencia general función Solicita_listar_tipos_documentales_relacion_tramite_radicacion " & ex.Message
        End Try
    End Function
    Function Solicita_listar_tipos_documentales_relacionados_al_tipo_tramite(
                                                                             ByVal id_tipo_tramite As Integer, ByVal _
                                                                             tipo_plantilla_tramite_radicado As String,
                                                                             ByRef page1 As Page) As String
        Try
            Dim Campo As String = ""
            Dim tipo_plantilla_tramite As String = ""
            If tipo_plantilla_tramite_radicado = "RADICACION ENTRANTE" Or tipo_plantilla_tramite_radicado = "" Then
                tipo_plantilla_tramite = "tipo_doc_entrante_id_Tipo_Doc_Entrante"
            Else
                tipo_plantilla_tramite = "tipo_doc_saliente_id_Tip_Doc_Saliente"
            End If
            Dim Result As String = ""
            Dim Parametro_Consulta As String = " SELECT rdt.ID_TIPO_DOCUMENTAL_CHEQUEO,tds.Descripcion_Documento As TIPO_DOCUMENTAL" &
            " FROM  ra_dig_tipos_docum_lista_chequeo as  rdt" &
            " inner join tipo_doc_series as tds on (tds.Id_Tipo_Doc_Series=rdt.tipo_doc_series_Id_Tipo_Doc_Series)" &
            " where " & tipo_plantilla_tramite & "=" & id_tipo_tramite &
            " order by tds.Descripcion_Documento  "
            Dim scripma As GridView = page1.FindControl("data_grid")
            Dim ref_UpdateGeneral_documentos As UpdatePanel = page1.FindControl("UpdateGeneral")
            Dim hideselecion As Object = page1.FindControl("hdnEmailID")
            Dim Datset_consulta As DataSet = New DataSet("ra_dig_tipos_docum_lista_chequeo")
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset_consulta)
            If Result <> "YES" Then
                Solicita_listar_tipos_documentales_relacionados_al_tipo_tramite = "Error listando datos " & Result
                Exit Function
            End If
            If Datset_consulta.Tables(0).Rows.Count = 0 Then
                scripma.DataSource = Datset_consulta
                hideselecion.value = "-1"
                scripma.DataBind()
                ref_UpdateGeneral_documentos.Update()
                Solicita_listar_tipos_documentales_relacionados_al_tipo_tramite = "YES"
                Exit Function
            Else
                scripma.DataSource = Datset_consulta
                hideselecion.value = "-1"
                scripma.DataBind()
                For i As Integer = 0 To scripma.Rows.Count - 1
                    scripma.Rows(i).Attributes.Add("id", scripma.Rows(i).Cells(0).Text.ToString())
                    scripma.Rows(i).Attributes.Add("name_tipo", scripma.Rows(i).Cells(1).Text.ToString())
                Next
                ref_UpdateGeneral_documentos.Update()
                Solicita_listar_tipos_documentales_relacionados_al_tipo_tramite = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_listar_tipos_documentales_relacionados_al_tipo_tramite = "Inconsistencia general función Solicita_listar_tipos_documentales_relacionados_al_tipo_tramite " & ex.Message
        End Try
    End Function

    Function Solicita_listar_tipos_documentales_relacion_tramite_radicacion_lista_obligatorio(ByVal id_tipo_tramite As Integer,
                                                                                              ByVal tipo_plantilla_tramite_radicado As String,
                                                                                              ByRef lista_tipos() As stru_chek_lista_tramite) As String
        Try
            Erase lista_tipos
            Dim Campo As String = ""
            Dim tipo_plantilla_tramite As String = ""
            If tipo_plantilla_tramite_radicado = "RADICACION ENTRANTE" Or tipo_plantilla_tramite_radicado = "" Then
                tipo_plantilla_tramite = "tipo_doc_entrante_id_Tipo_Doc_Entrante"
            Else
                tipo_plantilla_tramite = "tipo_doc_saliente_id_Tip_Doc_Saliente"
            End If
            Dim Result As String = ""
            Dim Parametro_Consulta As String = " SELECT rdt.ID_TIPO_DOCUMENTAL_CHEQUEO,tds.Descripcion_Documento as TIPO,rdt.OBLIGATORIO " &
            " FROM  ra_dig_tipos_docum_lista_chequeo as  rdt" &
            " inner join tipo_doc_series as tds on (tds.Id_Tipo_Doc_Series=rdt.tipo_doc_series_Id_Tipo_Doc_Series)" &
            " where " & tipo_plantilla_tramite & "=" & id_tipo_tramite & " and rdt.OBLIGATORIO=1" &
            " order by rdt.ORDEN_LISTA,rdt.OBLIGATORIO desc"
            Dim Datset_consulta As DataSet = New DataSet("ra_dig_tipos_docum_lista_chequeo")
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset_consulta)
            If Result <> "YES" Then
                Solicita_listar_tipos_documentales_relacion_tramite_radicacion_lista_obligatorio = "Error listando datos lista de chequeo " & Result
                Exit Function
            End If
            If Datset_consulta.Tables(0).Rows.Count = 0 Then
                Solicita_listar_tipos_documentales_relacion_tramite_radicacion_lista_obligatorio = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset_consulta.Tables(0).Rows.Count - 1
                    ReDim Preserve lista_tipos(i)
                    lista_tipos(i).ID_TIPO_DOCUMENTAL_CHEQUEO = Datset_consulta.Tables(0).Rows(i).Item(0)
                    lista_tipos(i).Descripcion_Documento = Datset_consulta.Tables(0).Rows(i).Item(1)
                    lista_tipos(i).OBLIGATORIO = Datset_consulta.Tables(0).Rows(i).Item(2)
                    lista_tipos(i).estado_cumple = 0
                Next
                Solicita_listar_tipos_documentales_relacion_tramite_radicacion_lista_obligatorio = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_listar_tipos_documentales_relacion_tramite_radicacion_lista_obligatorio = "Inconsistencia general función Solicita_listar_tipos_documentales_relacion_tramite_radicacion_lista " & ex.Message
        End Try
    End Function
    Function Solicita_listar_tipos_documentales_relacionados_al_tipo_tramite_lista_adjunta(
                                                                                           ByVal id_tipo_tramite As Integer, ByVal _
                                                                                           tipo_plantilla_tramite_radicado As String,
                                                                                           ByRef page1 As Page,
                                                                                           ByRef estado_resultado As String) As String
        Try
            Dim Campo As String = ""
            Dim tipo_plantilla_tramite As String = ""
            If tipo_plantilla_tramite_radicado = "RADICACION ENTRANTE" Then
                tipo_plantilla_tramite = "tipo_doc_entrante_id_Tipo_Doc_Entrante"
            Else
                tipo_plantilla_tramite = "tipo_doc_saliente_id_Tip_Doc_Saliente"
            End If
            Dim Result As String = ""
            Dim Parametro_Consulta As String = " SELECT rdt.ID_TIPO_DOCUMENTAL_CHEQUEO,tds.Descripcion_Documento " &
            " FROM  ra_dig_tipos_docum_lista_chequeo as  rdt" &
            " inner join tipo_doc_series as tds on (tds.Id_Tipo_Doc_Series=rdt.tipo_doc_series_Id_Tipo_Doc_Series)" &
            " where " & tipo_plantilla_tramite & "=" & id_tipo_tramite &
            " order by rdt.ORDEN_LISTA"
            Dim scripma As GridView = page1.FindControl("data_grid_chequeo")
            Dim ref_UpdateGeneral_documentos As UpdatePanel = page1.FindControl("UpdateGeneral")
            Dim hideselecion As Object = page1.FindControl("Hidden_0002")
            Dim Datset_consulta As DataSet = New DataSet("ra_dig_tipos_docum_lista_chequeo")
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset_consulta)
            HttpContext.Current.Session.Item("DATA_SET_SESION") = Datset_consulta
            If Result <> "YES" Then
                Solicita_listar_tipos_documentales_relacionados_al_tipo_tramite_lista_adjunta = "Error listando datos " & Result
                Exit Function
            End If
            If Datset_consulta.Tables(0).Rows.Count = 0 Then
                scripma.DataSource = Nothing
                hideselecion.value = "-1"
                scripma.DataBind()
                ref_UpdateGeneral_documentos.Update()
                estado_resultado = "NO"
                Solicita_listar_tipos_documentales_relacionados_al_tipo_tramite_lista_adjunta = "YES"
                Exit Function
            Else
                estado_resultado = "YES"
                scripma.DataSource = Datset_consulta
                hideselecion.value = "-1"
                scripma.DataBind()
                For i As Integer = 0 To scripma.Rows.Count - 1
                    scripma.Rows(i).Attributes.Add("id", scripma.Rows(i).Cells(0).Text.ToString())
                Next
                ref_UpdateGeneral_documentos.Update()
                Solicita_listar_tipos_documentales_relacionados_al_tipo_tramite_lista_adjunta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_listar_tipos_documentales_relacionados_al_tipo_tramite_lista_adjunta = "Inconsistencia general función Solicita_listar_tipos_documentales_relacionados_al_tipo_tramite_lista_adjunta " & ex.Message
        End Try
    End Function

    Function Asigna_datos_lista_chequeo_adjunta(ByVal id_tarea As Long, ByRef estado_lista As String) As String
        Try
            Dim id_tipo_flujo As Integer = 0
            Dim Result As String = ""
            Dim refclas_dat_adit As New Class_DAT_ADIC_TAR
            Dim Ref_class_workflow As New ClassWorkflowDigitalizacion
            Result = refclas_dat_adit.SolicitaIdTipoFlujoTareaWorkflow(id_tarea,
                                                            HttpContext.Current.Session.Item("WF_RUTAWORKFLOW"),
                                                            id_tipo_flujo)
            If Result <> "YES" Then
                Asigna_datos_lista_chequeo_adjunta = Result
                Exit Function
            End If
            If id_tipo_flujo = 1 Then
                Result = Ref_class_workflow.SolicitaParametrosParaListartiposDocumentalesTareaWorkflowInterna(HttpContext.Current.Session.Item("Id_Ruta_Workflow"),
                                                                             id_tarea,
                                                                             HttpContext.Current.Session.Item("DG_TIPO_TRAMITE"),
                                                                             HttpContext.Current.Session.Item("DG_ID_TRAMITE"),
                                                                             HttpContext.Current.Session.Item("DG_ID_GABINETE"),
                                                                             HttpContext.Current.Session.Item("DG_NOMBRE_GABINETE"),
                                                                             HttpContext.Current.Session.Item("DG_ID_CONFIG_DIGITALIZACION"),
                                                                             HttpContext.Current.Session.Item("DG_RADICADO"),
                                                                             HttpContext.Current.Session("DG_NOMBRE_TRAMITE"))
                HttpContext.Current.Session.Item("DG_TIPODIGITALIZACION") = "TRAMITE"
                If Result <> "YES" Then
                    estado_lista = "NO"
                    Asigna_datos_lista_chequeo_adjunta = "YES"
                    Exit Function
                Else
                    estado_lista = "YES"
                    Asigna_datos_lista_chequeo_adjunta = "YES"
                    Exit Function
                End If
            Else
                Result = Ref_class_workflow.SolicitaParametrosParaListartiposDocumentalesTareaWorkflowExterna(HttpContext.Current.Session.Item("Id_Ruta_Workflow"),
                                                                                     id_tarea,
                                                                                     HttpContext.Current.Session.Item("WF_RUTAWORKFLOW"),
                                                                                     HttpContext.Current.Session.Item("DG_TIPO_TRAMITE"),
                                                                                     HttpContext.Current.Session.Item("DG_ID_GABINETE"),
                                                                                     HttpContext.Current.Session.Item("DG_NOMBRE_GABINETE"),
                                                                                     HttpContext.Current.Session.Item("DG_RADICADO"),
                                                                                     HttpContext.Current.Session.Item("DG_NOMBRE_TRAMITE"))
                If Result <> "YES" Then
                    estado_lista = "NO"
                    Asigna_datos_lista_chequeo_adjunta = "YES"
                    Exit Function
                End If
                Dim ref_class_tipo_doc_entrante As New Class_tipo_doc_entrante
                Result = ref_class_tipo_doc_entrante.RetornaIdTipoTramitePorNombreTipo(HttpContext.Current.Session.Item("DG_NOMBRE_TRAMITE"),
                                                                                       HttpContext.Current.Session.Item("DG_ID_TRAMITE"))
                If Result <> "YES" Then
                    estado_lista = "NO"
                    Asigna_datos_lista_chequeo_adjunta = "YES"
                    Exit Function
                End If
                Dim Refclas_config As New Class_ra_dig_config_digitalizacion
                Result = Refclas_config.Solicita_id_configuracion_digitalizacion(HttpContext.Current.Session.Item("DG_ID_TRAMITE"),
                                                                                 "RADICACION ENTRANTE",
                                                                                 HttpContext.Current.Session.Item("DG_ID_CONFIG_DIGITALIZACION"))
                HttpContext.Current.Session.Item("DG_TIPODIGITALIZACION") = "TRAMITE"
                If Result <> "YES" Then
                    estado_lista = "NO"
                    Asigna_datos_lista_chequeo_adjunta = "YES"
                    Exit Function
                Else
                    estado_lista = "YES"
                    Asigna_datos_lista_chequeo_adjunta = "YES"
                    Exit Function
                End If

            End If
        Catch ex As Exception
            Asigna_datos_lista_chequeo_adjunta = "Inconsistencia general función Asigna_datos_lista_chequeo_adjunta " & ex.Message
        End Try
    End Function
    Function SolicitaDatosTipoDocumentalListaChequeo(ByVal IdTipoDocumentoListaChequeo As Integer,
                                                     ByRef StruTipoListaChequeo As stru_tipo_lista_chequeo) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita estructura lista de chequeo
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdTipoDocumentoListaChequeo  : Representa la identificación de la lista de chequeo
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'StruTipoListaChequeo  : Retorna la estructura de la lista de chequeo
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-04-01
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim ParametroConsulta As String = " SELECT  tipo_doc_entrante_id_Tipo_Doc_Entrante,tipo_doc_saliente_id_Tip_Doc_Saliente," &
                "series_documentales_Id_Series,tipo_doc_series_Id_Tipo_Doc_Series,subseries_documentales_Id_SubSeries," &
                "tipos_doc_subseries_Id_Tipos_Doc_SubSerie,TIPO_TRAMITE,OBLIGATORIO,UNICO,ORDEN_LISTA" &
                " from ra_dig_tipos_docum_lista_chequeo where ID_TIPO_DOCUMENTAL_CHEQUEO=" &
                 IdTipoDocumentoListaChequeo
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_dig_tipos_docum_lista_chequeo")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(ParametroConsulta, Datset)
            If Result <> "YES" Then
                SolicitaDatosTipoDocumentalListaChequeo = "Función SolicitaDatosTipoDocumentalListaChequeo dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaDatosTipoDocumentalListaChequeo = "Imposible encontrar los datos del tipo documental en la lista de chequeo con el identificador (" & IdTipoDocumentoListaChequeo & ") "
                Exit Function
            Else

                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    StruTipoListaChequeo.tipo_doc_entrante_id_Tipo_Doc_Entrante = 0
                Else
                    StruTipoListaChequeo.tipo_doc_entrante_id_Tipo_Doc_Entrante = Datset.Tables(0).Rows(0).Item(0)
                End If
                If Datset.Tables(0).Rows(0).IsNull(1) = True Then
                    StruTipoListaChequeo.tipo_doc_saliente_id_Tip_Doc_Saliente = 0
                Else
                    StruTipoListaChequeo.tipo_doc_saliente_id_Tip_Doc_Saliente = Datset.Tables(0).Rows(0).Item(1)
                End If
                If Datset.Tables(0).Rows(0).IsNull(2) = True Then
                    StruTipoListaChequeo.series_documentales_Id_Series = 0
                Else
                    StruTipoListaChequeo.series_documentales_Id_Series = Datset.Tables(0).Rows(0).Item(2)
                End If
                If Datset.Tables(0).Rows(0).IsNull(3) = True Then
                    StruTipoListaChequeo.tipo_doc_series_Id_Tipo_Doc_Series = 0
                Else
                    StruTipoListaChequeo.tipo_doc_series_Id_Tipo_Doc_Series = Datset.Tables(0).Rows(0).Item(3)
                End If
                If Datset.Tables(0).Rows(0).IsNull(4) = True Then
                    StruTipoListaChequeo.subseries_documentales_Id_SubSeries = 0
                Else
                    StruTipoListaChequeo.subseries_documentales_Id_SubSeries = Datset.Tables(0).Rows(0).Item(4)
                End If
                If Datset.Tables(0).Rows(0).IsNull(5) = True Then
                    StruTipoListaChequeo.tipos_doc_subseries_Id_Tipos_Doc_SubSerie = 0
                Else
                    StruTipoListaChequeo.tipos_doc_subseries_Id_Tipos_Doc_SubSerie = Datset.Tables(0).Rows(0).Item(5)
                End If
                If Datset.Tables(0).Rows(0).IsNull(6) = True Then
                    StruTipoListaChequeo.TIPO_TRAMITE = 0
                Else
                    StruTipoListaChequeo.TIPO_TRAMITE = Datset.Tables(0).Rows(0).Item(6)
                End If
                If Datset.Tables(0).Rows(0).IsNull(7) = True Then
                    StruTipoListaChequeo.OBLIGATORIO = 0
                Else
                    StruTipoListaChequeo.OBLIGATORIO = Datset.Tables(0).Rows(0).Item(7)
                End If
                If Datset.Tables(0).Rows(0).IsNull(8) = True Then
                    StruTipoListaChequeo.UNICO = 0
                Else
                    StruTipoListaChequeo.UNICO = Datset.Tables(0).Rows(0).Item(8)
                End If
                If Datset.Tables(0).Rows(0).IsNull(9) = True Then
                    StruTipoListaChequeo.ORDEN_LISTA = 0
                Else
                    StruTipoListaChequeo.ORDEN_LISTA = Datset.Tables(0).Rows(0).Item(9)
                End If
                SolicitaDatosTipoDocumentalListaChequeo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaDatosTipoDocumentalListaChequeo = "Inconsistencia general función SolicitaDatosTipoDocumentalListaChequeo " & ex.Message
        End Try
    End Function
    Function Lista_tipos_documentales_obligatorios_tramite(ByVal id_tipo_tramite As Integer,
                                                           ByRef stru() As stru_tipo_lista_chequeo) As String
        Try
            Dim Result As String = ""
            Dim Parametro_Consulta As String = " SELECT  tipo_doc_entrante_id_Tipo_Doc_Entrante,tipo_doc_saliente_id_Tip_Doc_Saliente," &
                "series_documentales_Id_Series,tipo_doc_series_Id_Tipo_Doc_Series,subseries_documentales_Id_SubSeries," &
                "tipos_doc_subseries_Id_Tipos_Doc_SubSerie,TIPO_TRAMITE,OBLIGATORIO,UNICO,ORDEN_LISTA" &
                " from ra_dig_tipos_docum_lista_chequeo as radt where tipo_doc_entrante_id_Tipo_Doc_Entrante=" &
                id_tipo_tramite & " and OBLIGATORIO=1"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_dig_tipos_docum_lista_chequeo")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Lista_tipos_documentales_obligatorios_tramite = "Función Lista_tipos_documentales_obligatorios_tramite dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Lista_tipos_documentales_obligatorios_tramite = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru(i)
                    If Datset.Tables(0).Rows(i).IsNull(0) = True Then
                        stru(i).tipo_doc_entrante_id_Tipo_Doc_Entrante = 0
                    Else
                        stru(i).tipo_doc_entrante_id_Tipo_Doc_Entrante = Datset.Tables(0).Rows(i).Item(0)
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(1) = True Then
                        stru(i).tipo_doc_saliente_id_Tip_Doc_Saliente = 0
                    Else
                        stru(i).tipo_doc_saliente_id_Tip_Doc_Saliente = Datset.Tables(0).Rows(i).Item(1)
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(2) = True Then
                        stru(i).series_documentales_Id_Series = 0
                    Else
                        stru(i).series_documentales_Id_Series = Datset.Tables(0).Rows(i).Item(2)
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(3) = True Then
                        stru(i).tipo_doc_series_Id_Tipo_Doc_Series = 0
                    Else
                        stru(i).tipo_doc_series_Id_Tipo_Doc_Series = Datset.Tables(0).Rows(i).Item(3)
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(4) = True Then
                        stru(i).subseries_documentales_Id_SubSeries = 0
                    Else
                        stru(i).subseries_documentales_Id_SubSeries = Datset.Tables(0).Rows(i).Item(4)
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(5) = True Then
                        stru(i).tipos_doc_subseries_Id_Tipos_Doc_SubSerie = 0
                    Else
                        stru(i).tipos_doc_subseries_Id_Tipos_Doc_SubSerie = Datset.Tables(0).Rows(i).Item(5)
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(6) = True Then
                        stru(i).TIPO_TRAMITE = 0
                    Else
                        stru(i).TIPO_TRAMITE = Datset.Tables(0).Rows(i).Item(6)
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(7) = True Then
                        stru(i).OBLIGATORIO = 0
                    Else
                        stru(i).OBLIGATORIO = Datset.Tables(0).Rows(i).Item(7)
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(8) = True Then
                        stru(i).UNICO = 0
                    Else
                        stru(i).UNICO = Datset.Tables(0).Rows(i).Item(8)
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(9) = True Then
                        stru(i).ORDEN_LISTA = 0
                    Else
                        stru(i).ORDEN_LISTA = Datset.Tables(0).Rows(i).Item(9)
                    End If

                Next
                Lista_tipos_documentales_obligatorios_tramite = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_tipos_documentales_obligatorios_tramite = "Inconsistencia general función Lista_tipos_documentales_obligatorios_tramite " & ex.Message
        End Try
    End Function
End Class
