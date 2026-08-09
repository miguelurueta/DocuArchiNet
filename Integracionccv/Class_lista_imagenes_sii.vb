Imports Newtonsoft.Json

Public Structure imagenes_lis_sii
    Dim idanexo As String
    Dim tipo As String
    Dim observaciones As String
    Dim formato As String
    Dim url As String
End Structure

Public Class Class_lista_imagenes_sii
    Public Class [Class_lista_imagenes_sii]
        Private myStruct() As imagenes
        Private structList As List(Of Class_lista_imagenes_sii)
        Public Property idanexo As String
        Public Property formato As String
        Public Property tipo As String
        Public Property tipoanexo As String
        Public Property observaciones As String
        Public Property url As String
        Public Property tiposirep As String
        Public Property tipodigitalizacion As String
        Public Property identificador As String
        Public Property identificacion As String
        Public Property nombre As String
        Public Property matricula As String
        Public Property proponente As String
        Public Property fechadocumento As String
        Public Property origen As String

        Public Sub New()

        End Sub

        Public Function returnStruct() As List(Of Class_lista_imagenes_sii)
            Return structList
        End Function
    End Class
    Function SolicitaEstructuraCamposDynamicArchivosImagenesSII(ByRef class_campos_table_bostra_table As List(Of class_campos_table_bostra_table)) As String
        '--------------------------------------------------------------------------------
        'Funcion : Solicita la estructura de campos del registro  de imagens SII
        '          
        '        
        '--------------------------------------------------------------------------------
        '                           PARAMETROS  
        '--------------------------------------------------------------------------------
        '
        '                             
        '
        '
        '
        '---------------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------------
        'class_campos_table_bostra_table :Representa la estructura de campos  
        '---------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------------
        'Fecha                 : 2024-12-17
        'Modifica              : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------------
        Try
            class_campos_table_bostra_table = New List(Of class_campos_table_bostra_table)
            Dim item As New class_campos_table_bostra_table
            item = New class_campos_table_bostra_table
            item.field = "operate"
            item.title = "OPERATION"
            item.checkbox = False
            item.visible = True
            item.viisble_sql = 1
            item.clickToSelect = False
            item.visible_like_sql = 1
            item.align = "center"
            item.events = "window.operateEvents"
            item.formatter = "operateFormatter_image_sii"
            class_campos_table_bostra_table.Add(item)
            item = New class_campos_table_bostra_table
            item.title = "idanexo"
            item.field = "idanexo"
            item.field_destino = "idanexo"
            item.visible = False
            item.viisble_sql = 1
            item.visible_like_sql = 1
            class_campos_table_bostra_table.Add(item)
            item = New class_campos_table_bostra_table
            item.title = "formato"
            item.field = "formato"
            item.field_destino = "formato"
            item.visible = False
            item.viisble_sql = 1
            item.visible_like_sql = 1
            class_campos_table_bostra_table.Add(item)
            item = New class_campos_table_bostra_table
            item.title = "tipo"
            item.field = "tipo"
            item.field_destino = "tipo"
            item.visible = False
            item.viisble_sql = 1
            item.visible_like_sql = 1
            class_campos_table_bostra_table.Add(item)
            item = New class_campos_table_bostra_table
            item.title = "DOCUMENTO"
            item.field = "observaciones"
            item.field_destino = "observaciones"
            item.visible = True
            item.viisble_sql = 1
            item.visible_like_sql = 1
            class_campos_table_bostra_table.Add(item)
            item = New class_campos_table_bostra_table
            item.title = "url"
            item.field = "url"
            item.field_destino = "url"
            item.visible = False
            item.viisble_sql = 1
            item.visible_like_sql = 1
            class_campos_table_bostra_table.Add(item)
            SolicitaEstructuraCamposDynamicArchivosImagenesSII = "YES"
        Catch ex As Exception
            SolicitaEstructuraCamposDynamicArchivosImagenesSII = "Inconsistencia general funcion SolicitaEstructuraCamposDynamicArchivosImagenesSII " & ex.Message
        End Try
    End Function
    Function SolicitaArchivosRelacionadosRadicadoSII(ByVal RadicadoSII As String,
                                                     ByRef Class_bostra_table_row As Object,
                                                     ByRef class_campos_table_bostra_table As List(Of class_campos_table_bostra_table)) As String
        '--------------------------------------------------------------------------------
        'Funcion : Solicita la lista de archivos anexos a un radicado en la integración
        'con el sistema SII
        '          
        '        
        '--------------------------------------------------------------------------------
        '                           PARAMETROS  
        '--------------------------------------------------------------------------------
        'RadicadoSII : Rpresenta el consecutivo de radicado SII
        '                             
        '
        '
        '
        '---------------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------------
        'class_campos_table_bostra_table  :Representa la estructura de campos
        'class_campos_table_bostra_table :Representa la estructura de registros
        '---------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------------
        'Fecha                 : 2024-12-17
        'Modifica              : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Class_ConsultarRadicado_sii As New Class_ConsultarRadicado_sii
            Dim ConsultarRadicado_sii As ConsultarRadicado_sii = Nothing
            Dim list_image_si As New Class_lista_imagenes_sii
            Dim Class_lista_imagenes_sii As List(Of Class_lista_imagenes_sii) = Nothing
            Result = Class_ConsultarRadicado_sii.ConSultarRadicado(RadicadoSII,
                                                                   ConsultarRadicado_sii)
            If Result <> "YES" Then
                SolicitaArchivosRelacionadosRadicadoSII = "Error integración SII (" & Result & ")"
                Exit Function
            End If
            If ConsultarRadicado_sii.imagenes Is Nothing Then
                SolicitaArchivosRelacionadosRadicadoSII = "El radicado SII (" & RadicadoSII & ") , no tiene documentos SII relacionados "
                Exit Function
            End If
            Dim imagenes_sii_lista As New imagenes_sii_lista
            Result = imagenes_sii_lista.SolicitalistaImagenesAnexosSII(ConsultarRadicado_sii.imagenes,
                                                                       Class_lista_imagenes_sii)
            If Result <> "YES" Then
                SolicitaArchivosRelacionadosRadicadoSII = Result
                Exit Function
            End If
            class_campos_table_bostra_table = New List(Of class_campos_table_bostra_table)
            Result = SolicitaEstructuraCamposDynamicArchivosImagenesSII(class_campos_table_bostra_table)
            If Result <> "YES" Then
                SolicitaArchivosRelacionadosRadicadoSII = Result
                Exit Function
            End If
            Class_bostra_table_row = JsonConvert.SerializeObject(Class_lista_imagenes_sii)
            SolicitaArchivosRelacionadosRadicadoSII = "YES"
            Exit Function
        Catch ex As Exception
            SolicitaArchivosRelacionadosRadicadoSII = "Inconsistencia general función SolicitaArchivosRelacionadosRadicadoSII " & ex.Message
        End Try
    End Function
    Public Class imagenes_sii_lista
        Function SolicitalistaImagenesAnexosSII(ByVal Imagenes() As imagenes,
                                                ByRef Class_lista_imagenes_sii_ As List(Of Class_lista_imagenes_sii)) As String
            '--------------------------------------------------------------------------------
            'Funcion : Realiza la conversión de la lista de imagenes de la estructura SII
            '          a la clase lista de imagenes
            '
            '          
            '        
            '--------------------------------------------------------------------------------
            '                           PARAMETROS  
            '--------------------------------------------------------------------------------
            'Imagenes : Rpresenta la estructura con la lista de imagees
            '                             
            '
            '
            '
            '---------------------------------------------------------------------------------
            '                           RETORNO
            '---------------------------------------------------------------------------------
            'Class_lista_imagenes_sii_  :Representa la estructura con la lista de enexos
            '---------------------------------------------------------------------------------
            '                         CARACTERIZACIÓN
            '---------------------------------------------------------------------------------
            'Fecha                 : 2024-12-17
            'Modifica              : Miguel Angel Urueta Miranda
            '----------------------------------------------------------------------------------
            Try
                Class_lista_imagenes_sii_ = New List(Of Class_lista_imagenes_sii)()
                Dim ClassCarateres As New ClassCarateres
                For i As Integer = 0 To Imagenes.Length - 1
                    Dim Class_lista_imagenes_sii As New Class_lista_imagenes_sii
                    Class_lista_imagenes_sii.formato = Imagenes(i).formato
                    Class_lista_imagenes_sii.idanexo = Imagenes(i).idanexo
                    Class_lista_imagenes_sii.tipo = Imagenes(i).tipo
                    Class_lista_imagenes_sii.tipoanexo = Imagenes(i).tipoanexo
                    Class_lista_imagenes_sii.observaciones = Imagenes(i).observaciones
                    Class_lista_imagenes_sii.url = Imagenes(i).url
                    Class_lista_imagenes_sii.tiposirep = Imagenes(i).tiposirep
                    Class_lista_imagenes_sii.tipodigitalizacion = Imagenes(i).tipodigitalizacion
                    Class_lista_imagenes_sii.identificador = Imagenes(i).identificador
                    Class_lista_imagenes_sii.identificacion = Imagenes(i).identificacion
                    Class_lista_imagenes_sii.nombre = Imagenes(i).nombre
                    ClassCarateres.RemplazaCaracteresNoValidos(HttpContext.Current.Session.Item("DG_CDCARACTERES"), Class_lista_imagenes_sii.nombre)
                    Class_lista_imagenes_sii.matricula = Imagenes(i).matricula
                    Class_lista_imagenes_sii.proponente = Imagenes(i).proponente
                    Class_lista_imagenes_sii.fechadocumento = Imagenes(i).fechadocumento
                    Class_lista_imagenes_sii.origen = Imagenes(i).origen
                    Class_lista_imagenes_sii_.Add(Class_lista_imagenes_sii)
                Next
                SolicitalistaImagenesAnexosSII = "YES"
            Catch ex As Exception
                SolicitalistaImagenesAnexosSII = "Inconsistencia general función SolicitalistaImagenesAnexosSII " & ex.Message
            End Try
        End Function
        Function Lista_imagenes_sii(ByVal radicado_sii As String,
                                    ByVal tipo_consulta As Integer,
                                    ByVal valor_consulta As String,
                                    ByRef colum_order_name As String,
                                    ByRef order_colum As String,
                                    ByRef labetitle As Label,
                                    ByRef scripma As GridView,
                                    ByRef hideselecion As HtmlInputHidden,
                                    ByRef updat As UpdatePanel) As String
            Try
                Dim Result As String = ""
                Dim Class_ConsultarRadicado_sii As New Class_ConsultarRadicado_sii
                Dim stru_consulta_radicado As ConsultarRadicado_sii = Nothing
                Dim Class_lista_imagenes_sii As List(Of Class_lista_imagenes_sii) = Nothing
                If tipo_consulta = 1 Then
                    Result = Class_ConsultarRadicado_sii.ConSultarRadicado(radicado_sii,
                                                                           stru_consulta_radicado)
                    If Result <> "YES" Then
                        Lista_imagenes_sii = Result
                        Exit Function
                    End If
                    If Not stru_consulta_radicado.imagenes Is Nothing Then
                        Result = Me.SolicitalistaImagenesAnexosSII(stru_consulta_radicado.imagenes,
                                                            Class_lista_imagenes_sii)
                        If Result <> "YES" Then
                            Lista_imagenes_sii = Result
                            Exit Function
                        End If
                        HttpContext.Current.Session.Item("GA_DATO_CONSULTA_PUBLICO_BJEC") = Class_lista_imagenes_sii
                    End If

                End If
                If tipo_consulta = 2 Then
                    Class_lista_imagenes_sii = HttpContext.Current.Session.Item("GA_DATO_CONSULTA_PUBLICO_BJEC")
                End If
                HttpContext.Current.Session.Item("Sort_matri_colum_compartido") = {"OPCIONES", "idanexo",
                                                                                   "formato", "tipo",
                                                                                   "observaciones", "url"}
                HttpContext.Current.Session.Item("SortExpression_publico") = colum_order_name
                HttpContext.Current.Session.Item("SortDirection_publico") = order_colum
                HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_PUBLICO") = tipo_consulta
                If Class_lista_imagenes_sii Is Nothing Then
                    labetitle.Text = "Se encontro " & 0 & " registro(s) "
                    scripma.DataSource = Class_lista_imagenes_sii
                    hideselecion.Value = "-1"
                    scripma.DataBind()
                    updat.Update()
                    Lista_imagenes_sii = "YES"
                    Exit Function
                Else
                    labetitle.Text = "Se encontro " & Class_lista_imagenes_sii.Count & " registro(s)  "
                    scripma.DataSource = Class_lista_imagenes_sii
                    hideselecion.Value = "-1"
                    scripma.DataBind()
                    updat.Update()
                    For i As Integer = 0 To scripma.Rows.Count - 1
                        scripma.Rows(i).Attributes.Add("id", scripma.Rows(i).Cells(1).Text.ToString())
                        Dim divhtml As New HtmlControls.HtmlGenericControl("div")
                        Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                        ihtml.Style.Add("color", "white")
                        ihtml.Attributes.Add("class", "fad fa-arrow-to-bottom")
                        ihtml.Style.Add("color", "white")
                        Dim ahtml As New HtmlControls.HtmlGenericControl("a")
                        ahtml.Attributes.Add("Class", "btn btn-success btn-sm")
                        ahtml.Attributes.Add("onclick", "prevent_list_archivo(event,this);")
                        ahtml.Attributes.Add("title", "Descarga arhivo")
                        'ahtml.Attributes.Add("idd", myStructList(i).idanexo.ToString())
                        'ahtml.Attributes.Add("ur", myStructList(i).url.ToString())
                        'ahtml.Attributes.Add("ext", myStructList(i).formato.ToString)
                        ahtml.Attributes.Add("idd", scripma.Rows(i).Cells(1).Text.ToString())
                        ahtml.Attributes.Add("ur", scripma.Rows(i).Cells(5).Text.ToString())
                        ahtml.Attributes.Add("ext", scripma.Rows(i).Cells(2).Text.ToString())
                        ahtml.Attributes.Add("tip_event", "des_car_archivo")
                        ahtml.Style.Add("margin-left", "3px")
                        ahtml.Controls.Add(ihtml)
                        divhtml.Controls.Add(ahtml)

                        ihtml = New HtmlControls.HtmlGenericControl("i")
                        ihtml.Style.Add("color", "white")
                        ihtml.Attributes.Add("class", "fad fas fa-save")
                        ihtml.Style.Add("color", "white")

                        ahtml = New HtmlControls.HtmlGenericControl("a")
                        ahtml.Attributes.Add("Class", "btn btn-success btn-sm")
                        ahtml.Attributes.Add("onclick", "prevent_list_archivo(event,this);")
                        ahtml.Attributes.Add("title", "Guardar arhivo")
                        'ahtml.Attributes.Add("idd", myStructList(i).idanexo.ToString())
                        'ahtml.Attributes.Add("ur", myStructList(i).url.ToString())
                        'ahtml.Attributes.Add("ext", myStructList(i).formato.ToString)
                        ahtml.Attributes.Add("idd", scripma.Rows(i).Cells(1).Text.ToString())
                        ahtml.Attributes.Add("ur", scripma.Rows(i).Cells(5).Text.ToString())
                        ahtml.Attributes.Add("ext", scripma.Rows(i).Cells(2).Text.ToString())
                        ahtml.Attributes.Add("tip_event", "guar_dar_archivo")
                        ahtml.Style.Add("margin-left", "3px")
                        ahtml.Controls.Add(ihtml)
                        divhtml.Controls.Add(ahtml)
                        divhtml.Style.Add("display", "inline-flex")
                        scripma.Rows(i).Cells(0).Controls.Add(divhtml)
                        For z As Integer = 0 To scripma.Rows(i).Cells.Count - 1
                            If z > 0 Then
                                scripma.Rows(i).Cells(z).Attributes.Add("Class", "GridviewScrollItem_line_cort_tr_flex")
                                scripma.Rows(i).Cells(z).Attributes.Add("onclick", "prevent_scrol(event,this);")
                            End If
                        Next
                    Next
                    Dim Refclas As New ClassGredview
                    Result = Refclas.add_clase_acender_decender(colum_order_name,
                                                                HttpContext.Current.Session.Item("Sort_matri_colum_compartido"),
                                                                order_colum,
                                                                scripma)
                    If Result <> "YES" Then
                        Lista_imagenes_sii = "Error add clase funcion  Lista_imagenes_sii " & Result
                        Exit Function
                    End If
                End If
                Lista_imagenes_sii = "YES"
            Catch ex As Exception
                Lista_imagenes_sii = "Inconsistencia general función Lista_imagenes_sii " & ex.Message
            End Try
        End Function
    End Class
End Class

