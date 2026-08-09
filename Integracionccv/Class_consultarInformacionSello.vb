Imports Newtonsoft.Json

Public Structure imagenes_sello
    Dim url As String
    Dim idanexo As String
    Dim tipo As String
    Dim tiposirep As String
    Dim tipodigitalizacion As String
    Dim identificador As String
    Dim formato As String
    Dim identificacion As String
    Dim nombre As String
    Dim matricula As String
    Dim proponente As String
    Dim fechadocumento As String
    Dim origen As String
    Dim observaciones As String
End Structure
Public Structure consultarInformacionSello
    Dim codigoerror As String
    Dim mensajeerror As String
    Dim inscripciones() As InscripcionesInformacionSello
End Structure
Public Structure InscripcionesInformacionSello
    Dim libro As String
    Dim registro As String
    Dim fecha As String
    Dim hora As String
    Dim usuario As String
    Dim usuariosii As String
    Dim tipoidentificacion As String
    Dim identificacion As String
    Dim nombre As String
    Dim matricula As String
    Dim proponente As String
    Dim acto As String
    Dim nacto As String
    Dim noticia As String
    Dim tipolibro As String
    Dim paginainicial As String
    Dim numeropaginas As String
    Dim imagenes() As imagenes_sello

End Structure
Public Class [Class_lista_inscripcioes_sello]
    Private structList As List(Of Class_lista_inscripcioes_sello)
    Public Property libro As String
    Public Property registro As String
    Public Property fecha As String
    Public Property hora As String
    Public Property nacto As String
    Public Property identificacion As String
    Public Property nombre As String
    Public Property matricula As String
    Public Property proponente As String
    Public Property acto As String
    Public Property noticia As String
    Public Property url As String
    Public Property Recibo As String
    Public Property CodigoBarras As String
    Public Sub New()

    End Sub

    Public Function returnStruct() As List(Of Class_lista_inscripcioes_sello)
        Return structList
    End Function
End Class
Public Class Class_consultarInformacionSello
    Function SolicitaCamposBootConstanciasIncripcionSII(ByRef class_campos_table_bostra_table As List(Of class_campos_table_bostra_table)) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita los campos bootstraf para listar las constancias de inscripción
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        '
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'class_campos_table_bostra_table  : Retorna la estructura 
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-04-01
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
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
            item.events = "window.operateEventsConstanciasInscripcion"
            item.formatter = "operateFormattertablebootListaConstancia"
            class_campos_table_bostra_table.Add(item)
            item = New class_campos_table_bostra_table
            item.title = "Libro"
            item.field = "libro"
            item.field_destino = "libro"
            item.visible = True
            item.viisble_sql = 1
            item.visible_like_sql = 1
            class_campos_table_bostra_table.Add(item)
            item = New class_campos_table_bostra_table
            item.title = "Incripción"
            item.field = "registro"
            item.field_destino = "registro"
            item.visible = True
            item.viisble_sql = 1
            item.visible_like_sql = 1
            class_campos_table_bostra_table.Add(item)
            item = New class_campos_table_bostra_table
            item.title = "fecha"
            item.field = "fecha"
            item.field_destino = "fecha"
            item.visible = True
            item.viisble_sql = 1
            item.visible_like_sql = 1
            class_campos_table_bostra_table.Add(item)
            item = New class_campos_table_bostra_table
            item.title = "hora"
            item.field = "hora"
            item.field_destino = "hora"
            item.visible = True
            item.viisble_sql = 1
            item.visible_like_sql = 1
            class_campos_table_bostra_table.Add(item)
            item = New class_campos_table_bostra_table
            item.title = "nacto"
            item.field = "nacto"
            item.field_destino = "nacto"
            item.visible = True
            item.viisble_sql = 1
            item.visible_like_sql = 1
            class_campos_table_bostra_table.Add(item)
            item = New class_campos_table_bostra_table
            item.title = "acto"
            item.field = "acto"
            item.field_destino = "acto"
            item.visible = True
            item.viisble_sql = 1
            item.visible_like_sql = 1
            class_campos_table_bostra_table.Add(item)
            item = New class_campos_table_bostra_table
            item.title = "noticia"
            item.field = "noticia"
            item.field_destino = "noticia"
            item.visible = False
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
            item = New class_campos_table_bostra_table
            item.title = "identificacion"
            item.field = "identificacion"
            item.field_destino = "identificacion"
            item.visible = False
            item.viisble_sql = 0
            item.visible_like_sql = 0
            class_campos_table_bostra_table.Add(item)
            item = New class_campos_table_bostra_table
            item.title = "nombre"
            item.field = "nombre"
            item.field_destino = "nombre"
            item.visible = False
            item.viisble_sql = 0
            item.visible_like_sql = 0
            class_campos_table_bostra_table.Add(item)
            item = New class_campos_table_bostra_table
            item.title = "matricula"
            item.field = "matricula"
            item.field_destino = "matricula"
            item.visible = False
            item.viisble_sql = 0
            item.visible_like_sql = 0
            class_campos_table_bostra_table.Add(item)
            item = New class_campos_table_bostra_table
            item.title = "proponente"
            item.field = "proponente"
            item.field_destino = "proponente"
            item.visible = False
            item.viisble_sql = 0
            item.visible_like_sql = 0
            class_campos_table_bostra_table.Add(item)
            item = New class_campos_table_bostra_table
            item.title = "Recibo"
            item.field = "Recibo"
            item.field_destino = "Recibo"
            item.visible = False
            item.viisble_sql = 0
            item.visible_like_sql = 0
            class_campos_table_bostra_table.Add(item)
            item = New class_campos_table_bostra_table
            item.title = "CodigoBarras"
            item.field = "CodigoBarras"
            item.field_destino = "CodigoBarras"
            item.visible = False
            item.viisble_sql = 0
            item.visible_like_sql = 0
            class_campos_table_bostra_table.Add(item)
            SolicitaCamposBootConstanciasIncripcionSII = "YES"
            Exit Function
        Catch ex As Exception
            SolicitaCamposBootConstanciasIncripcionSII = "Inconsistencia general funcion SolicitaCamposBootConstanciasIncripcionSII " & ex.Message
        End Try
    End Function
    Function SolicitaListaConstanciasIncripcionSII(ByRef Class_bostra_table_row As Object,
                                                   ByRef class_campos_table_bostra_table As List(Of class_campos_table_bostra_table)) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la lista de constancias de inscripcion
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        '
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'Class_bostra_table_row  : Retorna la estrucutura de los registro tipo bootstraf
        'class_campos_table_bostra_table : Retorna la estructura tablas boot
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-07-05
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            If HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "-1" Or HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "0" Then
                SolicitaListaConstanciasIncripcionSII = "Es necesario seleccionar una tarea para poder adjuntar un documento desde los servicios web."
                Exit Function
            End If
            If HttpContext.Current.Session.Item("ADJUNTAR_IMAGENES_PREDETERMINADA") = 0 Then
                SolicitaListaConstanciasIncripcionSII = "Este usuario no cuenta con los permisos necesarios para adjuntar documentos a través de los servicios de integración. "
                Exit Function
            End If
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = ref_Class_DAT_ADIC_TAR.SolicitaCodigoBarrasIdTareaWorflow(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                               HttpContext.Current.Session.Item("SII_COD_BARRAS"))
            If Result <> "YES" Then
                SolicitaListaConstanciasIncripcionSII = Result
                Exit Function
            End If
            Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                    HttpContext.Current.Session.Item("SII_RECIBO"))

            If Result <> "YES" Then
                SolicitaListaConstanciasIncripcionSII = Result
                Exit Function
            End If
            Dim myStructList As List(Of Class_lista_inscripcioes_sello) = Nothing
            Dim stru_consulta_sello As consultarInformacionSello = Nothing
            Result = ConSultarInformacionSello(HttpContext.Current.Session.Item("SII_COD_BARRAS"),
                                               stru_consulta_sello)
            If Result <> "YES" Then
                SolicitaListaConstanciasIncripcionSII = Result
                Exit Function
            End If
            If Not stru_consulta_sello.inscripciones Is Nothing Then
                Result = Me.ConsolidaListaConstanciasIscripcionesSII(stru_consulta_sello.inscripciones,
                                                                     HttpContext.Current.Session.Item("SII_RECIBO"),
                                                                     HttpContext.Current.Session.Item("SII_COD_BARRAS"),
                                                                     myStructList)
                If Result <> "YES" Then
                    SolicitaListaConstanciasIncripcionSII = Result
                    Exit Function
                End If
                HttpContext.Current.Session.Item("GA_DATO_CONSULTA_PUBLICO_BJEC") = myStructList
            Else
                SolicitaListaConstanciasIncripcionSII = "Imposible encontras datos de inscripción para el radicado (" & HttpContext.Current.Session.Item("SII_COD_BARRAS") & ")"
                Exit Function
            End If
            class_campos_table_bostra_table = New List(Of class_campos_table_bostra_table)
            Result = SolicitaCamposBootConstanciasIncripcionSII(class_campos_table_bostra_table)
            If Result <> "YES" Then
                SolicitaListaConstanciasIncripcionSII = Result
                Exit Function
            End If
            Class_bostra_table_row = JsonConvert.SerializeObject(myStructList)
            SolicitaListaConstanciasIncripcionSII = "YES"
            Exit Function
        Catch ex As Exception
            SolicitaListaConstanciasIncripcionSII = "Inconsistencia general funcion SolicitaListaConstanciasIncripcionSII " & ex.Message
        End Try
    End Function
    Function Lista_inscripciones_radicado_sii(ByVal radicado_codigo_barra As String,
                                              ByVal recibo_sii As String,
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
            Dim myStructList As List(Of Class_lista_inscripcioes_sello) = Nothing
            Dim stru_consulta_sello As consultarInformacionSello = Nothing
            If tipo_consulta = 1 Then
                Result = Me.ConSultarInformacionSello(radicado_codigo_barra,
                                                      stru_consulta_sello)
                If Result <> "YES" Then
                    Lista_inscripciones_radicado_sii = Result
                    Exit Function
                End If
                If Not stru_consulta_sello.inscripciones Is Nothing Then
                    Result = Me.ConsolidaListaConstanciasIscripcionesSII(stru_consulta_sello.inscripciones,
                                                                         recibo_sii,
                                                                         radicado_codigo_barra,
                                                                         myStructList)
                    If Result <> "YES" Then
                        Lista_inscripciones_radicado_sii = Result
                        Exit Function
                    End If
                    HttpContext.Current.Session.Item("GA_DATO_CONSULTA_PUBLICO_BJEC") = myStructList
                End If
            End If
            If tipo_consulta = 2 Then
                myStructList = HttpContext.Current.Session.Item("GA_DATO_CONSULTA_PUBLICO_BJEC")
            End If
            HttpContext.Current.Session.Item("Sort_matri_colum_compartido") = {"OPCIONES", "LIBRO",
                                                                                   "INSCRIPCION", "FECHA",
                                                                                   "ACTO", "URL"}
            HttpContext.Current.Session.Item("SortExpression_publico") = colum_order_name
            HttpContext.Current.Session.Item("SortDirection_publico") = order_colum
            HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_PUBLICO") = tipo_consulta
            If myStructList Is Nothing Then
                labetitle.Text = "Se encontro " & 0 & " registro(s) "
                scripma.DataSource = myStructList
                hideselecion.Value = "-1"
                scripma.DataBind()
                updat.Update()
                Lista_inscripciones_radicado_sii = "YES"
                Exit Function
            Else
                labetitle.Text = "Se encontro " & myStructList.Count & " registro(s)  "
                scripma.DataSource = myStructList
                hideselecion.Value = "-1"
                scripma.DataBind()
                updat.Update()
                For i As Integer = 0 To scripma.Rows.Count - 1
                    scripma.Rows(i).Attributes.Add("id", i)
                    Dim divhtml As New HtmlControls.HtmlGenericControl("div")
                    Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fad fa-arrow-to-bottom")
                    ihtml.Style.Add("color", "white")
                    Dim ahtml As New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-success btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent_list_archivo(event,this);")
                    ahtml.Attributes.Add("title", "Descarga arhivo")
                    ahtml.Attributes.Add("idd", myStructList(i).registro.ToString())
                    ahtml.Attributes.Add("ur", myStructList(i).url.ToString())
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
                    ahtml.Attributes.Add("idd", myStructList(i).registro.ToString())
                    ahtml.Attributes.Add("ur", myStructList(i).url.ToString())
                    ahtml.Attributes.Add("lib_", myStructList(i).libro.ToString())
                    ahtml.Attributes.Add("inscrip_", myStructList(i).registro.ToString())
                    ahtml.Attributes.Add("fecha_", myStructList(i).fecha.ToString())
                    ahtml.Attributes.Add("hora_", myStructList(i).hora.ToString())
                    ahtml.Attributes.Add("ident_", myStructList(i).identificacion.ToString())
                    ahtml.Attributes.Add("nombre_", myStructList(i).nombre.ToString())
                    ahtml.Attributes.Add("matri_", myStructList(i).matricula.ToString())
                    ahtml.Attributes.Add("prop_", myStructList(i).proponente.ToString())
                    ahtml.Attributes.Add("acto_", myStructList(i).acto.ToString())
                    ahtml.Attributes.Add("noticia_", myStructList(i).noticia.ToString())
                    ahtml.Attributes.Add("recib_", recibo_sii)
                    ahtml.Attributes.Add("cod_", Val(radicado_codigo_barra))
                    ahtml.Attributes.Add("nacto_", myStructList(i).nacto.ToString())
                    ahtml.Attributes.Add("tip_event", "guar_dar_archivo_inscripcion")
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
                    Lista_inscripciones_radicado_sii = "Error add clase funcion  Lista_imagenes_sii " & Result
                    Exit Function
                End If
            End If
            Lista_inscripciones_radicado_sii = "YES"
        Catch ex As Exception
            Lista_inscripciones_radicado_sii = "Inconsistencia general función Lista_inscripciones_radicado_sii " & ex.Message
        End Try
    End Function
    Function ConsolidaListaConstanciasIscripcionesSII(ByVal InscripcionesInformacionSello() As InscripcionesInformacionSello,
                                                      ByVal Recibo As String,
                                                      ByVal CodigoBarras As String,
                                                      ByRef Class_lista_inscripcioes_sello As List(Of Class_lista_inscripcioes_sello)) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Consu¿olida la lista de las constancias de inscripción a formato tablas boot
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'InscripcionesInformacionSello  : Representa la estructura de las constancias SII
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'Class_lista_inscripcioes_sello  : Retorna la estructura de constancias consolidadas
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-07-05
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Class_lista_inscripcioes_sello = New List(Of Class_lista_inscripcioes_sello)()
            Dim ClassCarateres As New ClassCarateres
            For i As Integer = 0 To InscripcionesInformacionSello.Length - 1
                Dim Class_lista_imagenes_sii As New Class_lista_inscripcioes_sello
                If InscripcionesInformacionSello(i).libro <> "" Then
                    Class_lista_imagenes_sii.libro = InscripcionesInformacionSello(i).libro
                    Class_lista_imagenes_sii.libro = Class_lista_imagenes_sii.libro.Replace("RM", "")
                    Class_lista_imagenes_sii.libro = Class_lista_imagenes_sii.libro.Replace("RE", "")
                    Class_lista_imagenes_sii.libro = Class_lista_imagenes_sii.libro.Replace("RP", "")
                Else
                    Class_lista_imagenes_sii.libro = ""
                End If
                If InscripcionesInformacionSello(i).fecha <> "" Then
                    Dim anualidad As String = InscripcionesInformacionSello(i).fecha.Substring(0, 4)
                    Dim mes As String = InscripcionesInformacionSello(i).fecha.Substring(4, 2)
                    Dim dia As String = InscripcionesInformacionSello(i).fecha.Substring(6, 2)
                    Class_lista_imagenes_sii.fecha = anualidad & "-" & mes & "-" & dia
                Else
                    Class_lista_imagenes_sii.fecha = ""
                End If
                Class_lista_imagenes_sii.identificacion = InscripcionesInformacionSello(i).identificacion
                Class_lista_imagenes_sii.nombre = InscripcionesInformacionSello(i).nombre.Replace("'", "")

                If Class_lista_imagenes_sii.nombre <> "" Then
                    If Class_lista_imagenes_sii.nombre.Length > 40 Then
                        Class_lista_imagenes_sii.nombre = Left(Class_lista_imagenes_sii.nombre, 40)
                    End If
                End If
                ClassCarateres.RemplazaCaracteresNoValidos(HttpContext.Current.Session.Item("DG_CDCARACTERES"), Class_lista_imagenes_sii.nombre)
                If InscripcionesInformacionSello(i).matricula <> "" Then
                    Class_lista_imagenes_sii.matricula = InscripcionesInformacionSello(i).matricula.Replace("S0", "")
                Else
                    Class_lista_imagenes_sii.matricula = InscripcionesInformacionSello(i).matricula
                End If
                Class_lista_imagenes_sii.proponente = InscripcionesInformacionSello(i).proponente
                Class_lista_imagenes_sii.nacto = InscripcionesInformacionSello(i).nacto
                If Class_lista_imagenes_sii.nacto <> "" Then
                    If Class_lista_imagenes_sii.nacto.Length > 40 Then
                        Class_lista_imagenes_sii.nacto = Left(Class_lista_imagenes_sii.nacto, 40)
                    End If
                End If
                Class_lista_imagenes_sii.nacto = Class_lista_imagenes_sii.nacto.Replace("'", "")
                Class_lista_imagenes_sii.nacto = Class_lista_imagenes_sii.nacto.Replace("/", "")
                Class_lista_imagenes_sii.nacto = Class_lista_imagenes_sii.nacto.Replace("\", "")
                Class_lista_imagenes_sii.url = InscripcionesInformacionSello(i).imagenes(0).url
                Class_lista_imagenes_sii.registro = InscripcionesInformacionSello(i).registro
                Class_lista_imagenes_sii.acto = InscripcionesInformacionSello(i).acto
                Class_lista_imagenes_sii.noticia = InscripcionesInformacionSello(i).noticia
                Class_lista_imagenes_sii.noticia = Class_lista_imagenes_sii.noticia.Replace("'", "")
                Class_lista_imagenes_sii.noticia = Class_lista_imagenes_sii.noticia.Replace("/", "")
                Class_lista_imagenes_sii.noticia = Class_lista_imagenes_sii.noticia.Replace("\", "")
                Class_lista_imagenes_sii.hora = InscripcionesInformacionSello(i).hora
                Class_lista_imagenes_sii.identificacion = InscripcionesInformacionSello(i).identificacion
                Class_lista_imagenes_sii.CodigoBarras = CodigoBarras
                Class_lista_imagenes_sii.Recibo = Recibo
                Class_lista_inscripcioes_sello.Add(Class_lista_imagenes_sii)
            Next
            ConsolidaListaConstanciasIscripcionesSII = "YES"
        Catch ex As Exception
            ConsolidaListaConstanciasIscripcionesSII = "Inconsistencia general función SolicitaConstanciasIscripcionesSII " & ex.Message
        End Try
    End Function
    Function ConSultarInformacionSello(ByVal radicado As String,
                                       ByRef stru_consulta_sello As consultarInformacionSello) As String
        Try

            Dim Result As String = ""
            Dim usuario_sii As String = ""
            Dim clave_usuario_sii As String = ""
            Dim UrlBase As String = ""
            Dim codigo_empresa As String = ""
            Dim Class_ws_usuarioworkflowsii As New Class_ws_usuarioworkflowsii
            Result = Class_ws_usuarioworkflowsii.solicita_usuario_validacion_sii(codigo_empresa,
                                                                                 usuario_sii,
                                                                                 clave_usuario_sii)
            If Result <> "YES" Then
                ConSultarInformacionSello = Result
                Exit Function
            End If
            Result = Class_ws_usuarioworkflowsii.Solicita_url_nombrefuncion_restfull(UrlBase,
                                                                                     "solicitarToken")
            If Result <> "YES" Then
                ConSultarInformacionSello = Result
                Exit Function
            End If
            Dim stru_token As SolicitaToken = Nothing
            Dim Class_ClassResfull As New Class_ClassResfull
            Result = Class_ClassResfull.Solicitar_token_general(codigo_empresa,
                                                               usuario_sii,
                                                               clave_usuario_sii,
                                                               UrlBase & "solicitarToken",
                                                               stru_token)
            If Result <> "YES" Then
                ConSultarInformacionSello = Result
                Exit Function
            End If
            If stru_token.mensajeerror <> "" Then
                ConSultarInformacionSello = stru_token.mensajeerror
                Exit Function
            End If
            Dim Parametros As Dictionary(Of String, String) = New Dictionary(Of String, String)()
            Parametros.Add("codigoempresa", codigo_empresa)
            Parametros.Add("usuariows", usuario_sii)
            Parametros.Add("token", stru_token.token)
            Parametros.Add("radicado", radicado)
            Dim Class_Desserializacion As New Class_Desserializacion
            Dim respuestaServidor As String = ""
            Result = Class_ClassResfull.GetResponse(UrlBase & "consultarInformacionSello",
                                                    Parametros,
                                                    "POST",
                                                    respuestaServidor)
            If Result <> "YES" Then
                ConSultarInformacionSello = Result
                Exit Function
            End If
            Result = Class_Desserializacion.DesSerializacion_consultarInformacionSello(respuestaServidor,
                                                                                       stru_consulta_sello)
            If Result <> "YES" Then
                ConSultarInformacionSello = Result
                Exit Function
            End If
            If stru_consulta_sello.mensajeerror <> "" Then
                ConSultarInformacionSello = "La funcion ConSultarInformacionSello del SII genero el siguiente error : (" & stru_consulta_sello.mensajeerror & ") codigo error (" & stru_consulta_sello.codigoerror & ") Radicado (" & radicado & ")"
            Else
                ConSultarInformacionSello = "YES"
            End If
        Catch ex As Exception
            ConSultarInformacionSello = "Inconsistencia general función ConSultarInformacionSello " & ex.Message
        End Try
    End Function
End Class
