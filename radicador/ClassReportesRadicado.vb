Imports System.Math
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.OleDb
Imports System.Data.Odbc
Imports MySql.Data.MySqlClient
Imports Microsoft.VisualBasic
Imports System
Imports System.Text
Imports System.CodeDom.Compiler
Imports System.Reflection
Imports System.Collections.Specialized
Imports System.Drawing
Imports iTextSharp.text
Imports iTextSharp.text.pdf
Imports System.IO
Public Class ClassReportesRadicado
    Private w As StreamWriter
    Private ruta As String
    Public xpath As String = ""
    Function Crear_Parametro_consulta(ByVal Matri_Parameros() As String, _
                                      ByRef pag As Page) As String
        Try
            Dim Tableparametro As Table = pag.FindControl("Tableparametro")
            If Tableparametro Is Nothing Then
                Crear_Parametro_consulta = "Imposible encontrar el control (Crear_Parametro_consulta )"
                Exit Function
            End If
            Dim Panel_parametros_consulta As Panel = pag.FindControl("Panel_parametros_consulta")
            If Panel_parametros_consulta Is Nothing Then
                Crear_Parametro_consulta = "Imposible encontrar el control (Panel_parametros_consulta )"
                Exit Function
            End If
            Dim Hidden_parametro_sel As Object = pag.FindControl("Hidden_parametro_sel")
            If Hidden_parametro_sel Is Nothing Then
                Crear_Parametro_consulta = "Imposible encontrar el control (Hidden_parametro_sel )"
                Exit Function
            End If
            Dim Button_reporte As Button = pag.FindControl("Button_reporte")
            If Button_reporte Is Nothing Then
                Crear_Parametro_consulta = "Imposible encontrar el control (Button_reporte )"
                Exit Function
            End If
            Dim UpdatePanel_parametros As UpdatePanel = pag.FindControl("UpdatePanel_parametros")
            If UpdatePanel_parametros Is Nothing Then
                Crear_Parametro_consulta = "Imposible encontrar el control (UpdatePanel_parametros )"
                Exit Function
            End If
            Tableparametro.Controls.Clear()
            Dim i As Integer = 0
            Dim RegisTro = New TableRow
            Dim CellDa = New TableCell
            Dim Lebol = New Label
            Dim Result As String = ""
            Dim refclas As New ClassRadicador
            Dim refclas_rad As New ClassRadicador
            Lebol.Text = "Por Favor Ingrese Los Parametros de Consulta "
            Lebol.Font.Size = 9
            Lebol.ForeColor = Color.DarkBlue
            CellDa.Controls.Add(Lebol)
            RegisTro.Controls.Add(CellDa)
            CellDa = New TableCell
            RegisTro.Controls.Add(CellDa)
            Tableparametro.Controls.Add(RegisTro)
            For i = 0 To UBound(Matri_Parameros) - 1
                RegisTro = New TableRow
                CellDa = New TableCell
                Dim TexboxC As New TextBox
                Dim Labelc As New Label
                Labelc.ID = "i_ca_ser_" & i
                Dim textolabel As String = ""
                Dim nombre_tabla As String = ""
                Dim nombre_campo As String = ""
                Dim estado_campo_fecha As String = ""
                If InStr(Matri_Parameros(i), "$") > 0 Then
                    textolabel = Matri_Parameros(i).Split("$")(0)
                    nombre_campo = Matri_Parameros(i).Split("$")(1)
                    nombre_tabla = Matri_Parameros(i).Split("$")(2)
                    estado_campo_fecha = Matri_Parameros(i).Split("$")(3)
                    Matri_Parameros(i) = textolabel
                    If nombre_campo <> "" And nombre_tabla <> "" Then
                        Result = refclas.agregar_auto_complete(Matri_Parameros(i), _
                                                               Panel_parametros_consulta, _
                                                               "GetGuiaRadicaconasp", _
                                                               nombre_tabla, nombre_campo)
                        If Result <> "YES" Then
                            Crear_Parametro_consulta = Result
                            Exit Function
                        End If
                    End If

                Else
                    textolabel = Matri_Parameros(i)
                End If
                Labelc.Style.Add("float", "left")
                TexboxC.Style.Add("float", "left")
                TexboxC.ID = Matri_Parameros(i)
                Labelc.Text = textolabel
                Labelc.Attributes.Add("Class", "h6 font-weight-light")
                CellDa.Controls.Add(Labelc)
                RegisTro.Controls.Add(CellDa)
                CellDa = New TableCell
                CellDa.Controls.Add(TexboxC)
                If estado_campo_fecha = "DATE" Then
                    TexboxC.MaxLength = 10
                    TexboxC.Attributes.Add("onkeypress", "GetChar (event);")
                    TexboxC.Attributes.Add("onkeypress", "return validate_fecha(event,this);")
                    TexboxC.Attributes.Add("placeholder", "yyyy mm dd")
                    TexboxC.Width = 95
                    TexboxC.Attributes.Add("class", "ml-0 form-control")
                    Dim bhtml As New HtmlControls.HtmlGenericControl("button")
                    bhtml.ID = "Fecha_ela_" & TexboxC.ID
                    bhtml.Attributes.Add("class", "ml-1 btn btn-success border-0")
                    bhtml.Attributes.Add("font-size", "10px")
                    bhtml.Attributes.Add("title", "formato aaaa mm dd")
                    Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ihtml.Attributes.Add("class", "fad fa-calendar-alt fa-1x")
                    bhtml.Controls.Add(ihtml)
                    CellDa.Controls.Add(bhtml)
                    Result = refclas_rad.Agregar_Calendar(bhtml.ID, TexboxC.ID, Panel_parametros_consulta)
                    If Result <> "YES" Then
                       
                    End If
                Else
                    TexboxC.Style.Add("width", "90%")
                    TexboxC.Attributes.Add("Class", "form-control m-2")
                End If
                RegisTro.Controls.Add(CellDa)
                Tableparametro.Controls.Add(RegisTro)
            Next
           
            RegisTro = New TableRow
            CellDa = New TableCell
            RegisTro.Controls.Add(CellDa)
            CellDa = New TableCell
            Dim Butonc As New Button
            Butonc.Attributes.Add("Class", "btn btn-success mt-5")
            Butonc.Attributes.Add("float", "rigth")
            Butonc.Text = "Consultar"
            AddHandler Butonc.Click, AddressOf Button_Click
            CellDa.Controls.Add(Butonc)
            RegisTro.Controls.Add(CellDa)
            Tableparametro.Controls.Add(RegisTro)
            'Asignar matris de parametros al boton
            Butonc.ID = "consulta_parametro_index"
            'Butonc.CssClass = "boton_blanco"
            Hidden_parametro_sel.Value = "1|"
            For i = 0 To UBound(Matri_Parameros)
                Hidden_parametro_sel.Value = Hidden_parametro_sel.Value & Matri_Parameros(i) & "|"
            Next
            Button_reporte.Visible = False
            UpdatePanel_parametros.Update()
            Crear_Parametro_consulta = "YES"
            Exit Function
        Catch ex As Exception
            Crear_Parametro_consulta = "Inconsistencia función Crear_Parametro_consulta " & ex.Message
        End Try
    End Function
    Sub Button_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim clasjava As New Classscrripjava
        Dim UpdatePanel_parametros As UpdatePanel = sender.page.findcontrol("UpdatePanel_parametros")
        Try
            If UpdatePanel_parametros Is Nothing Then
                clasjava.Showscripman("Imposible encontrar el control (UpdatePanel_parametros)", UpdatePanel_parametros)
            End If
            Dim Hidden_parametro_sel As Object = sender.page.findcontrol("Hidden_parametro_sel")
            If Hidden_parametro_sel Is Nothing Then
                clasjava.Showscripman("Imposible encontrar el control (Hidden_parametro_sel)", UpdatePanel_parametros)
            End If
            Dim TreeView1 As TreeView = sender.page.findcontrol("TreeView1")
            If TreeView1 Is Nothing Then
                clasjava.Showscripman("Imposible encontrar el control (TreeView1)", UpdatePanel_parametros)
            End If
            Dim update_tre_principal As UpdatePanel = sender.page.findcontrol("update_tre_principal")
            If update_tre_principal Is Nothing Then
                clasjava.Showscripman("Imposible encontrar el control (update_tre_principal)", UpdatePanel_parametros)
            End If
            Dim Matri_Parametro() As String
            Dim RefDato_Sql_Consulta As String = Trim(HttpContext.Current.Session.Item("Dato_Sql_Consulta_ra"))
            Dim ref As New ClassReportesRadicado
            Dim Result As String = ""
            Erase Matri_Parametro
            Matri_Parametro = Split(Hidden_parametro_sel.Value, "|")
            Dim i As Integer = 0
            Dim Darodoc As String
            Dim Matri_Nodo() As String
            Erase Matri_Nodo
            Dim Datos_Nodo As String = ""
            Result = ref.NodoChild_Selecionado(TreeView1, Datos_Nodo)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, update_tre_principal)
                Exit Sub
            Else
                Matri_Nodo = Split(Datos_Nodo, "|")
            End If
            'Remplazar parametros en la cosnulta
            If Not Matri_Parametro Is Nothing Then
                For i = 1 To UBound(Matri_Parametro)
                    If InStr(Matri_Parametro(i), "$") Then
                        Dim matri_list() As String = Matri_Parametro(i).Split("$")
                        Matri_Parametro(i) = matri_list(0)
                    End If
                    Darodoc = "#" & Matri_Parametro(i)
                    If InStr(RefDato_Sql_Consulta, Darodoc) Then
                        Dim ValorParametro As String = ""
                        Result = obtener_valor_parametro(Matri_Parametro(i), _
                                                         ValorParametro, _
                                                         sender.page)
                        If Result <> "YES" Then
                            clasjava.Showscripman(Result, UpdatePanel_parametros)
                            Exit Sub
                        End If
                        RefDato_Sql_Consulta = RefDato_Sql_Consulta.Replace(Darodoc, "'" & ValorParametro & "' ")
                    End If
                Next
                Result = ref.Resultado_consulta(sender.page, RefDato_Sql_Consulta, Matri_Nodo(1))
                If Result <> "YES" Then
                    clasjava.Showscripman(Result, UpdatePanel_parametros)
                    Exit Sub
                End If
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, UpdatePanel_parametros)
        End Try
    End Sub
    Function obtener_valor_parametro(ByVal Parametro As String, _
                                     ByRef Valor_Dato As String, _
                                     ByRef pag As Page) As String
        Dim clasjava As New Classscrripjava
        Try
            Dim i As Integer = 0
            Dim i2 As Integer = 0
            Dim RefCel As New TableCell
            Dim Reftextbox As New Object
            Dim texto_box As String = ""
            Reftextbox = Nothing
            Reftextbox = pag.FindControl(Parametro)
            If Reftextbox Is Nothing Then
                obtener_valor_parametro = "Imposible encontrar el control " & Parametro
                Exit Function
            Else
                Valor_Dato = Reftextbox.Text
                obtener_valor_parametro = "YES"
                Exit Function
            End If

        Catch ex As Exception
            obtener_valor_parametro = "Inconsistencia general función " & ex.Message
        End Try
    End Function
    Function Consulta_Reporte(ByVal Dato_Sql As String, _
                              ByVal nombre_reporte As String, _
                              ByRef pag As Page) As String
        Try
            Dim UpdatePanel_parametros As UpdatePanel = pag.FindControl("UpdatePanel_parametros")
            If UpdatePanel_parametros Is Nothing Then
                Consulta_Reporte = "Imposible encontrar el control (UpdatePanel_parametros)"
                Exit Function
            End If
            Dim Tableparametro As Table = pag.FindControl("Tableparametro")
            If Tableparametro Is Nothing Then
                Consulta_Reporte = "Imposible encontrar el control (Tableparametro)"
                Exit Function
            End If
            Dim MatriSql() As String = Split(Dato_Sql, "//")
            Dim MatriParametros() As String
            Dim Ref As New ClassReportesRadicado
            Dim Result As String = ""
            Erase MatriParametros
            'verifica que tenga parametros la consulta
            If Not MatriSql Is Nothing And UBound(MatriSql) > 0 Then
                MatriParametros = Split(MatriSql(1), "#")
            End If
            'Verifica parametros de consulta
            Dim i As Integer = 0
            Dim Darodoc As String = ""
            Dim CodigoSQL2 As String = ""
            If Not MatriParametros Is Nothing Then
                If MatriParametros(0) <> "" Then
                    Result = Ref.Crear_Parametro_consulta(MatriParametros, _
                                                          pag)
                    If Result <> "YES" Then
                        Consulta_Reporte = Result
                        Exit Function
                    End If
                Else
                    Tableparametro.Controls.Clear()
                    UpdatePanel_parametros.Update()
                    Result = Ref.Resultado_consulta(pag, Trim(MatriSql(0)), nombre_reporte)
                    If Result <> "YES" Then
                        Consulta_Reporte = Result
                        Exit Function
                    End If
                End If
                Consulta_Reporte = "YES"
                Exit Function
            Else
                Consulta_Reporte = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Consulta_Reporte = "Función Consulta_Reporte inconsistencia general " & ex.Message
        End Try

    End Function
    Function Convertir_tif_pdf(ByVal Matri_documentos() As String,
                               ByRef ruta_pdf As String,
                               ByVal Ajusta As String,
                               ByVal lectura As Integer,
                               ByVal pas_word As String) As String

        Dim refile As New FileInfo(Matri_documentos(0))
        Dim Rut_pdf As String = refile.DirectoryName
        Dim Nombre_Archivo As String = refile.Name
        If UCase(refile.Extension) = ".TIF" Or
           UCase(refile.Extension) = ".JPG" Or
           UCase(refile.Extension) = ".BMP" Then
        End If
        Nombre_Archivo = Nombre_Archivo.Replace(UCase(refile.Extension), ".PDF")
        ruta_pdf = ruta_pdf & Nombre_Archivo
        If File.Exists(ruta_pdf) Then
            'Kill(ruta_pdf)
            'Dim i As Integer = 0
        End If
        Dim oPdfDoc As New iTextSharp.text.Document()
        Dim oPdfWriter As PdfWriter = PdfWriter.GetInstance(oPdfDoc, New FileStream(ruta_pdf, FileMode.Create))
        Try
            If lectura = 1 Then
                Dim matri_permisos() As Integer = {PdfWriter.ALLOW_SCREENREADERS}
                oPdfWriter.SetEncryption(True, "123", "123", matri_permisos(0))
            End If
            oPdfDoc.Open()
            For k As Integer = 0 To Matri_documentos.Length - 1
                Dim oImage As iTextSharp.text.Image = iTextSharp.text.Image.GetInstance(Matri_documentos(k))
                Dim iWidth As Single = oImage.Width
                Dim iHeight As Single = oImage.Height
                If Ajusta = "YES" Then
                    oImage.SetAbsolutePosition(1, 1)
                    oPdfDoc.SetPageSize(New iTextSharp.text.RectangleReadOnly(iWidth, iHeight))
                    oPdfDoc.NewPage()
                    'oPdfDoc.Add(New iTextSharp.text.RectangleReadOnly(iWidth, iHeight))
                    oPdfDoc.Add(oImage)
                Else
                    Dim iAspectRatio As Double = iWidth / iHeight
                    Dim iWidthPage As Single = iTextSharp.text.PageSize.LETTER.Width
                    Dim iHeightPage As Single = iTextSharp.text.PageSize.LETTER.Height
                    Dim iPageAspectRatio As Double = iWidthPage / iHeightPage
                    Dim iWidthGoal As Single = 0
                    Dim iHeightGoal As Single = 0
                    If iWidth < iWidthPage And iHeight < iHeightPage Then
                        'Image fits within the page
                        iWidthGoal = iWidth
                        iHeightGoal = iHeight

                    ElseIf iAspectRatio > iPageAspectRatio Then
                        'Width is too big
                        iWidthGoal = iWidthPage
                        iHeightGoal = iWidthPage * (iHeight / iWidth)

                    Else
                        'Height is too big
                        iWidthGoal = iHeightPage * (iWidth / iHeight)
                        iHeightGoal = iHeightPage
                    End If
                    oImage.SetAbsolutePosition(1, 1)
                    oPdfDoc.NewPage()
                    oPdfDoc.Add(oImage)

                End If
            Next

            oPdfDoc.Close()
            If lectura = 1 Then
                'Dim reader As PdfReader = New PdfReader(ruta_pdf)
                'Dim MS As MemoryStream = New MemoryStream()
                'PdfEncryptor.Encrypt(reader, MS, True, "123", "123", PdfWriter.ALLOW_MODIFY_CONTENTS)
                'Dim stamper As PdfStamper = New PdfStamper(reader, MS, "4")
                'stamper.FormFlattening = True
                ''Dim pdfFormFields As AcroFields = pdfStampDocument.AcroFields
                ''Dim newtext As New TextField(stamper, New iTextSharp.text.Rectangle(590, 660, 470, 690), "txtfield")
                ''newtext.Options = TextField.READ_ONLY
                ''Dim field As PdfFormField = newtext.GetTextField()
                ''stamper.AddAnnotation(field, 1)
                ''stamper.Close()
                ''MS.Position = 0
                ''MS.Flush()
                ''reader.Close()
                'Dim fields() As String = stamper.AcroFields.Fields.[Select](Function(x) x.Key).ToArray()
                'For key As Integer = 0 To fields.Count - 1
                '    stamper.AcroFields.SetFieldProperty(fields(key), "setfflags", PdfFormField.FF_READ_ONLY, Nothing)
                'Next
                'stamper.Writer.CloseStream = False
                'stamper.Close()
                'MS.Position = 0
                ''fileBytes = MS.ToArray()
                'MS.Flush()
                'reader.Close()
            End If
            Convertir_tif_pdf = "YES"
        Catch ex As Exception
            Convertir_tif_pdf = "Inconsistencia general funcion Convertir_tif_pdf " & ex.Message
        End Try
    End Function
    Function Convertir_tif_pdf_Sello(ByVal Matri_documentos() As String,
                                     ByRef ruta_pdf As String,
                                     ByVal Ajusta As String,
                                     ByVal lectura As Integer,
                                     ByVal pas_word As String) As String

        Dim refile As New FileInfo(Matri_documentos(0))
        Dim Rut_pdf As String = refile.DirectoryName
        Dim Nombre_Archivo As String = refile.Name
        If UCase(refile.Extension) = ".TIF" Or
           UCase(refile.Extension) = ".JPG" Or
           UCase(refile.Extension) = ".BMP" Then
        End If
        'Nombre_Archivo = Nombre_Archivo.Replace(UCase(refile.Extension), ".PDF")
        ruta_pdf = ruta_pdf & "sello_pdf.PDF"
        If File.Exists(ruta_pdf) Then
            'Kill(ruta_pdf)
            'Dim i As Integer = 0
        End If
        Dim oPdfDoc As New iTextSharp.text.Document()
        Dim oPdfWriter As PdfWriter = PdfWriter.GetInstance(oPdfDoc, New FileStream(ruta_pdf, FileMode.Create))

        Try
            If lectura = 1 Then
                Dim matri_permisos() As Integer = {PdfWriter.ALLOW_SCREENREADERS}
                oPdfWriter.SetEncryption(True, "123", "123", matri_permisos(0))
            End If
            oPdfDoc.Open()
            For k As Integer = 0 To Matri_documentos.Length - 1
                Dim oImage As iTextSharp.text.Image = iTextSharp.text.Image.GetInstance(Matri_documentos(k))
                Dim iWidth As Single = oImage.Width
                Dim iHeight As Single = oImage.Height
                If Ajusta = "YES" Then
                    oImage.SetAbsolutePosition(1, 1)
                    oPdfDoc.SetPageSize(New iTextSharp.text.RectangleReadOnly(iWidth, iHeight))
                    oPdfDoc.NewPage()
                    'oPdfDoc.Add(New iTextSharp.text.RectangleReadOnly(iWidth, iHeight))
                    oPdfDoc.Add(oImage)
                Else
                    Dim iAspectRatio As Double = iWidth / iHeight
                    Dim iWidthPage As Single = iTextSharp.text.PageSize.LETTER.Width
                    Dim iHeightPage As Single = iTextSharp.text.PageSize.LETTER.Height
                    Dim iPageAspectRatio As Double = iWidthPage / iHeightPage
                    Dim iWidthGoal As Single = 0
                    Dim iHeightGoal As Single = 0
                    If iWidth < iWidthPage And iHeight < iHeightPage Then
                        'Image fits within the page
                        iWidthGoal = iWidth
                        iHeightGoal = iHeight

                    ElseIf iAspectRatio > iPageAspectRatio Then
                        'Width is too big
                        iWidthGoal = iWidthPage
                        iHeightGoal = iWidthPage * (iHeight / iWidth)

                    Else
                        'Height is too big
                        iWidthGoal = iHeightPage * (iWidth / iHeight)
                        iHeightGoal = iHeightPage
                    End If
                    oImage.SetAbsolutePosition(1, 1)
                    oPdfDoc.NewPage()
                    oPdfDoc.Add(oImage)

                End If
            Next
            Convertir_tif_pdf_Sello = "YES"
        Catch ex As Exception
            Convertir_tif_pdf_Sello = "Inconsistencia general funcion Convertir_tif_pdf_Sello " & ex.Message
        Finally
            If oPdfDoc.IsOpen Then
                oPdfDoc.Close()
            End If
        End Try
    End Function
    Function ExportToExcel(ByRef GridView1 As GridView)
        Try
            HttpContext.Current.Response.Clear()
            HttpContext.Current.Response.Buffer = True
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=GridViewExport.xls")
            HttpContext.Current.Response.Charset = ""
            HttpContext.Current.Response.ContentType = "application/vnd.ms-excel"
            Using sw As New StringWriter()
                Dim hw As New HtmlTextWriter(sw)
                GridView1.HeaderRow.BackColor = Color.White
                For Each cell As TableCell In GridView1.HeaderRow.Cells
                    cell.BackColor = GridView1.HeaderStyle.BackColor
                Next
                For Each row As GridViewRow In GridView1.Rows
                    row.BackColor = Color.White
                    For Each cell As TableCell In row.Cells
                        If row.RowIndex Mod 2 = 0 Then
                            cell.BackColor = GridView1.AlternatingRowStyle.BackColor
                        Else
                            cell.BackColor = GridView1.RowStyle.BackColor
                        End If
                        cell.CssClass = "textmode"
                    Next
                Next
                GridView1.RenderControl(hw)
                Dim style As String = "<style> .textmode { } </style>"
                HttpContext.Current.Response.Write(style)
                HttpContext.Current.Response.Output.Write(sw.ToString())
                HttpContext.Current.Response.Flush()
                HttpContext.Current.Response.[End]()
            End Using
            ExportToExcel = "YES"
        Catch EX As Exception
            ExportToExcel = "Inconsistencia función ExportToExcel " & EX.Message
        End Try
    End Function
    Function Export(ByVal titulos As ArrayList, _
                    ByVal datos As DataTable) As String
        Try
            Dim fs As New FileStream(ruta, FileMode.Create, FileAccess.ReadWrite)
            w = New StreamWriter(fs)
            Dim comillas As String = Char.ConvertFromUtf32(34)
            Dim html As New StringBuilder()
            html.Append("<!DOCTYPE html PUBLIC" + comillas + "-//W3C//DTD XHTML 1.0 Transitional//EN" + comillas + " " _
            + comillas + "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd" + comillas + ">")
            html.Append("<html xmlns=" + comillas + "http://www.w3.org/1999/xhtml" + comillas + ">")
            html.Append("<head>")
            html.Append("<meta http-equiv=" + comillas + "Content-Type" + comillas + "content=" + comillas + "text/html charset=utf-8" + comillas + "/>")
            html.Append("<title>Untitled Document</title>")
            html.Append("</head>")
            html.Append("<body>")
            'Generando encabezados del archivo         
            '(aquí podemos dar el formato como a una tabla de HTML)        
            html.Append("<table WIDTH=730 CELLSPACING=0 CELLPADDING=10 border=8 BORDERCOLOR=" + comillas + "#333366" + comillas + " bgcolor=" + comillas + "#FFFFFF" + comillas + ">")
            html.Append("<tr> <b>")
            For Each item As Object In titulos
                html.Append("<th>" + item.ToString() + "</th>")
            Next
            html.Append("</b> </tr>")
            'Generando datos del archivo        
            For i As Integer = 0 To datos.Rows.Count - 1
                html.Append("<tr>")
                For j As Integer = 0 To datos.Columns.Count - 1
                    html.Append("<td>" + datos.Rows.Item(i).Item(j).ToString + "</td>")
                Next
                html.Append("</tr>")
            Next
            html.Append("</body>")
            html.Append("</html>")
            w.Write(html.ToString())
            w.Close()
            Export = "YES"
        Catch ex As Exception
            Export = "Inconsistencia general funcion " & ex.ToString
        End Try

    End Function
    Function Export_html5(ByVal titulos As ArrayList, _
                          ByVal datos As DataTable, _
                          ByVal nombre_reporte As String, _
                          ByVal nombre_usuario As String) As String
        Try
            Dim fs As New FileStream(ruta, FileMode.Create, FileAccess.ReadWrite)
            w = New StreamWriter(fs)
            Dim comillas As String = Char.ConvertFromUtf32(34)
            Dim html As New StringBuilder()
            html.Append("!DOCTYPE HTML>")
            'html.Append("<html xmlns=" + comillas + "http://www.w3.org/1999/xhtml" + comillas + ">")
            html.Append("<head>")
            html.Append("<meta http-equiv=" + comillas + "Content-Type" + comillas + "content=" + comillas + "text/html charset=utf-8" + comillas + "/>")
            html.Append("<title>Untitled Document</title>")
            html.Append("</head>")
            html.Append("<body>")
            'Generando encabezados del archivo         
            '(aquí podemos dar el formato como a una tabla de HTML)  
            Dim colp As Integer = titulos.Count - 1
            Dim registro As Integer = datos.Rows.Count
            Dim uri_split() As String = HttpContext.Current.Request.Url.ToString.Split("/")
            Dim name_page As String = uri_split(uri_split.Length - 1)
            Dim base_url As String = ""
            For i As Integer = 0 To uri_split.Length - 2
                If i = 0 Then
                    base_url = uri_split(i)
                Else
                    base_url = base_url & "/" & uri_split(i)
                End If
            Next
            base_url = uri_split(0) & "//" & uri_split(2) & "/" & uri_split(3) & "/" & "imagera/logo_trd.png"
            Dim mg = "<img src=" + base_url + comillas + " alt=" + comillas + "Smiley face" + comillas + "width=" + comillas + "80" + comillas + _
                      "height =" + comillas + "80" + comillas + " >"
            html.Append("<table WIDTH=730 CELLSPACING=0 CELLPADDING=10 border=1 BORDERCOLOR=" + comillas + "#333366" + comillas + " bgcolor=" + comillas + "#FFFFFF" + comillas + ">")
            html.Append(" <tr> " & _
                              "<td rowspan=" + comillas + "5" + comillas + "; colspan=" + comillas + "0" + comillas + "> " & mg & " </td>" & _
                         "</tr>")
            html.Append(" <tr> " & _
                        "<td colspan=" + comillas + colp.ToString + comillas + " ; align=" + comillas + "middle" + comillas + "> Reporte : " & nombre_reporte & " </td>" & _
                      "</tr>")
            html.Append(" <tr> " & _
                       "<td colspan=" + comillas + colp.ToString + comillas + " ; align=" + comillas + "middle" + comillas + "> Genera : " & nombre_usuario & " </td>" & _
                     "</tr>")
            html.Append(" <tr> " & _
                      "<td colspan=" + comillas + colp.ToString + comillas + " ; align=" + comillas + "middle" + comillas + "> Fecha : " & Trim(CStr(Now.ToString("yyyy'-'MM'-'dd HH':'mm':'ss"))) & " </td>" & _
                    "</tr>")
            html.Append(" <tr> " & _
                     "<td colspan=" + comillas + colp.ToString + comillas + " ; align=" + comillas + "middle" + comillas + "> Registros : " & registro.ToString & " </td>" & _
                   "</tr>")
            html.Append(" <tr border=0> " & _
                     "<td colspan=" + comillas + (colp + 1).ToString + comillas + " ; align=" + comillas + "middle" + comillas + "> " & "" & " </td>" & _
                   "</tr>")
            html.Append("<tr> <b>")
            For Each item As Object In titulos
                html.Append("<th bgcolor=" + comillas + "#E7EDF5" + comillas + ">" + item.ToString() + "</th>")
            Next
            html.Append("</b> </tr>")
            'Generando datos del archivo        
            For i As Integer = 0 To datos.Rows.Count - 1
                html.Append("<tr>")
                For j As Integer = 0 To datos.Columns.Count - 1
                    html.Append("<td>" + datos.Rows.Item(i).Item(j).ToString + "</td>")
                Next
                html.Append("</tr>")
            Next
            html.Append("</body>")
            html.Append("</html>")
            w.Write(html.ToString())
            w.Close()
            Export_html5 = "YES"
        Catch ex As Exception
            Export_html5 = "Inconsistencia general funcion Export_html5 " & ex.ToString
        End Try

    End Function
    Function genera_xls(ByRef Datgridvi As GridView, _
                        ByRef tiparchivo As String, _
                        ByVal ruta_archivo As String, _
                        ByRef spli_header As String, _
                        ByVal nombre_reporte As String, _
                        ByVal nombre_usuario As String) As String
        Try
            Dim Result As String = ""
            Dim titulos As New ArrayList()
            Dim datosTabla As New DataTable()
            'Especificar ruta del archivo con extensión de EXCEL.   
            'Dim reclas As Classreportes = New Classreportes(Application.StartupPath + tiparchivo)
            xpath = ruta_archivo
            ruta = ruta_archivo
            'Dim ob = Datgridvi.DataSource
            Dim spli_header_matr() As String = spli_header.Split("|")
            'obtenemos los titulos del grid y creamos las columnas de la tabla  
            For item As Integer = 0 To spli_header_matr.Length - 2
                titulos.Add(spli_header_matr(item))
                datosTabla.Columns.Add()
            Next
            Dim incre As Integer = 1
            For Each item As GridViewRow In Datgridvi.Rows
                Dim rowx As DataRow = datosTabla.NewRow()
                If item.Visible = True Then
                    datosTabla.Rows.Add(rowx)
                    incre = incre + 1
                End If
            Next
            For Each rowcell As GridViewRow In Datgridvi.Rows
                Dim i As Integer = 0
                Dim difer_hiden As Integer = rowcell.Cells.Count - datosTabla.Columns.Count
                For cell As Integer = difer_hiden To rowcell.Cells.Count - 1
                    If rowcell.Cells.Item(cell).Visible = True Then
                        datosTabla.Rows(rowcell.RowIndex).Item(i) = rowcell.Cells(cell).Text
                        i = i + 1
                    End If

                Next
            Next
            If tiparchivo = "test.csv" Then
                'Result = ExportCSV(titulos, datosTabla)
                If Result <> "YES" Then
                    genera_xls = Result
                    Exit Function
                End If
            Else
                Result = Export_html5(titulos, datosTabla, nombre_reporte, nombre_usuario)
                If Result <> "YES" Then
                    genera_xls = Result
                    Exit Function
                End If
            End If
            genera_xls = "YES"
        Catch ex As Exception
            genera_xls = "Inconsistencia general funcion genera_xls " & ex.Message
        End Try

    End Function
    Function SolicitaReporteEcxelGredView(ByVal DataSet As DataSet,
                                          ByVal NombreReporte As String,
                                          ByVal NombreUsuario As String,
                                          ByVal RutaArchivo As String) As String
        Try
            Dim result As String = ""
            Dim TitleArray As New ArrayList()
            Dim DatosTabla As New DataTable()
            xpath = RutaArchivo
            ruta = RutaArchivo
            For i As Integer = 0 To DataSet.Tables(0).Columns.Count - 1
                TitleArray.Add(DataSet.Tables(0).Columns(i).ColumnName)
                DatosTabla.Columns.Add()
            Next
            Dim GridView As GridView = New GridView
            GridView.DataSource = DataSet
            GridView.DataBind()
            For Each item As GridViewRow In GridView.Rows
                Dim rowx As DataRow = DatosTabla.NewRow()
                If item.Visible = True Then
                    DatosTabla.Rows.Add(rowx)
                End If
            Next
            For Each rowcell As GridViewRow In GridView.Rows
                Dim i As Integer = 0
                Dim difer_hiden As Integer = rowcell.Cells.Count - DatosTabla.Columns.Count
                For cell As Integer = difer_hiden To rowcell.Cells.Count - 1
                    If rowcell.Cells.Item(cell).Visible = True Then
                        DatosTabla.Rows(rowcell.RowIndex).Item(i) = rowcell.Cells(cell).Text
                        i = i + 1
                    End If
                Next
            Next
            result = Export_html5(TitleArray, DatosTabla, NombreReporte, NombreUsuario)
            SolicitaReporteEcxelGredView = result
            Exit Function
        Catch ex As Exception
            SolicitaReporteEcxelGredView = "Inconsistencia general funcion SolicitaReporteEcxelGredView " & ex.Message
        End Try
    End Function
    Function genera_xls_paginacion(ByRef Datgridvi As GridView,
                                   ByRef tiparchivo As String,
                                   ByVal ruta_archivo As String,
                                   ByRef spli_header As String,
                                   ByVal nombre_reporte As String,
                                   ByVal nombre_usuario As String,
                                   ByVal ref_dat_set As Object) As String
        Try
            Dim Result As String = ""
            Dim titulos As New ArrayList()
            Dim datosTabla As New DataTable()
            xpath = ruta_archivo
            ruta = ruta_archivo
            Dim Ref_datgridvi As GridView = New GridView
            Ref_datgridvi.DataSource = ref_dat_set
            Ref_datgridvi.DataBind()
            Ref_datgridvi.EnableViewState = False
            Dim spli_header_matr() As String = spli_header.Split("|")
            For item As Integer = 0 To spli_header_matr.Length - 2
                titulos.Add(spli_header_matr(item))
                datosTabla.Columns.Add()
            Next
            Dim incre As Integer = 1
            For Each item As GridViewRow In Ref_datgridvi.Rows
                Dim rowx As DataRow = datosTabla.NewRow()
                If item.Visible = True Then
                    datosTabla.Rows.Add(rowx)
                    incre = incre + 1
                End If
            Next
            For Each rowcell As GridViewRow In Ref_datgridvi.Rows
                Dim i As Integer = 0
                Dim difer_hiden As Integer = rowcell.Cells.Count - datosTabla.Columns.Count
                For cell As Integer = difer_hiden To rowcell.Cells.Count - 1
                    If rowcell.Cells.Item(cell).Visible = True Then
                        datosTabla.Rows(rowcell.RowIndex).Item(i) = rowcell.Cells(cell).Text
                        i = i + 1
                    End If

                Next
            Next
            If tiparchivo = "test.csv" Then
                'Result = ExportCSV(titulos, datosTabla)
                If Result <> "YES" Then
                    genera_xls_paginacion = Result
                    Exit Function
                End If
            Else
                Result = Export_html5(titulos, datosTabla, nombre_reporte, nombre_usuario)
                If Result <> "YES" Then
                    genera_xls_paginacion = Result
                    Exit Function
                End If
            End If
            genera_xls_paginacion = "YES"
            Exit Function
        Catch ex As Exception
            genera_xls_paginacion = "Inconsistencia general funcion genera_xls_paginacion " & ex.Message
        End Try

    End Function
    Function genera_xls_buton(ByRef Datgridvi_ As GridView, _
                              ByRef tiparchivo As String, _
                              ByVal ruta_archivo As String, _
                              ByRef spli_header As String, _
                              ByVal nombre_reporte As String, _
                              ByVal nombre_usuario As String, _
                              ByVal Datgridvi As Object) As String
        Try
            Dim Result As String = ""
            Dim titulos As New ArrayList()
            Dim datosTabla As New DataTable()
            'Especificar ruta del archivo con extensión de EXCEL.   
            'Dim reclas As Classreportes = New Classreportes(Application.StartupPath + tiparchivo)
            xpath = ruta_archivo
            ruta = ruta_archivo
            'Dim Datgridvi As Object = HttpContext.Current.Session.Item("DATA_SET_SESION_TRAZA_RAD")
            Dim ob = Datgridvi.Columns.Count
            Dim spli_header_matr() As String = spli_header.Split("|")
            'obtenemos los titulos del grid y creamos las columnas de la tabla  
            Dim item_tab As Integer = 1
            For item As Integer = 1 To spli_header_matr.Length - 1
                If spli_header_matr(item) <> "" And spli_header_matr(item) <> " " Then
                    titulos.Add(spli_header_matr(item))
                    datosTabla.Columns.Add()
                    item_tab = item_tab + 1
                End If
            Next
            'se crean los renglones de la tabla   
            Dim incre As Integer = 1
            Dim r = Datgridvi.Rows
            For Each item As Object In Datgridvi.Rows
                Dim rowx As DataRow = datosTabla.NewRow()
                datosTabla.Rows.Add(rowx)
                incre = incre + 1
            Next
            For Each rowcell As GridViewRow In Datgridvi.Rows
                Dim i As Integer = 0
                For cell As Integer = 2 To rowcell.Cells.Count - 1
                    datosTabla.Rows(rowcell.RowIndex).Item(i) = rowcell.Cells(cell).Text
                    i = i + 1
                Next
            Next
            If tiparchivo = "test.csv" Then
                'Result = ExportCSV(titulos, datosTabla)
                If Result <> "YES" Then
                    genera_xls_buton = Result
                    Exit Function
                End If
            Else
                Result = Export_html5(titulos, datosTabla, nombre_reporte, nombre_usuario)
                If Result <> "YES" Then
                    genera_xls_buton = Result
                    Exit Function
                End If
            End If
            genera_xls_buton = "YES"
        Catch ex As Exception
            genera_xls_buton = "Inconsistencia general funcion genera_xls_buton " & ex.Message
        End Try

    End Function
    Function Listar_Reportes_Grupos_Treview(ByRef Tre_v2 As TreeView, _
                                            ByVal id_usuario As String, _
                                            ByVal Conectio_Documnent As String) As String
        Try

            Dim i As Integer = 0
            Dim z As Integer = 0
            Dim Tre_v As New TreeNode
            Tre_v.ChildNodes.Clear()
            Tre_v.Text = "REPORTES DISPONIBLES"
            Dim attrNodeGru As New TreeNode()
            attrNodeGru.Text = "REPORTES GESTION"
            attrNodeGru.SelectAction = TreeNodeSelectAction.Expand
            attrNodeGru.PopulateOnDemand = False
            Tre_v.ChildNodes.Add(attrNodeGru)
            Dim Matri_ReportesPar() As String
            Dim Resultado_User As String = ""
            Erase Matri_ReportesPar
            Dim Matri_Reportes() As String
            '***********************************
            'Lista los reportes por cada ruta
            '***********************************
            Resultado_User = Listar_Reportes_por_ruta(Matri_ReportesPar, id_usuario)
            If Resultado_User = "YES" Then
                If Not (Matri_ReportesPar) Is Nothing Then
                    For z = 0 To UBound(Matri_ReportesPar)
                        Erase Matri_Reportes
                        Matri_Reportes = Split(Matri_ReportesPar(z), "|")
                        Dim attrNode1Gru As New TreeNode
                        attrNode1Gru.Text = Matri_Reportes(1)
                        attrNode1Gru.Value = Matri_Reportes(0)
                        attrNode1Gru.ImageUrl = "../workflow/imageneswf/chart-line-light.png"
                        attrNodeGru.ChildNodes.Add(attrNode1Gru)

                    Next
                End If
            End If
            'Next

            Tre_v2.EnableViewState = True
            Tre_v2.Nodes.Add(Tre_v)
            Listar_Reportes_Grupos_Treview = "YES"
        Catch ex As Exception
            Listar_Reportes_Grupos_Treview = ex.ToString
        End Try
    End Function
    Function Listar_Reportes_por_ruta(ByRef Matri_Reportes() As String, ByVal id_usuario As Object) As String
        Try
            Dim Parametro_COnsulta As String = "Select id_reporte,nombre_reporte " & _
            "from reportes_workflow " & _
            "where Estado_Reporte=1"
            Parametro_COnsulta = "select id_reporte,nombre_reporte from  relacion_usuarios_reporte  as ruwr " & _
            "inner join reportes_workflow as rw on (ruwr.Reportes_Workflow_Id_Reporte=rw.ID_Reporte ) " & _
            " where ruwr.Usuarios_da_Clave_Usuario =" & id_usuario
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Parametro_COnsulta, Datset)
            If Result <> "YES" Then
                Listar_Reportes_por_ruta = "Imposible listar rutas workflow" & Result
                Exit Function
            End If
            If Datset Is Nothing Or Datset.Tables.Count = 0 Then
                Listar_Reportes_por_ruta = "YES"
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Listar_Reportes_por_ruta = " Imposible Encontrar id ruta no record tabla (0)"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve Matri_Reportes(i)
                    Matri_Reportes(i) = Datset.Tables(0).Rows(i).Item(0).ToString & "|" & Datset.Tables(0).Rows(i).Item(1).ToString
                Next
                Listar_Reportes_por_ruta = "YES"
                Exit Function
            End If
            Listar_Reportes_por_ruta = "YES"
        Catch ex As Exception
            Listar_Reportes_por_ruta = "Inconsistencia General Funcion Listar_Reportes_por_ruta " & ex.Message
        End Try

    End Function
    
    Function Datos_Sql_Reporte(ByVal Conection_conectro_C As String, ByVal id_Reporte As String, _
    ByRef Datos_Sql As String) As String
        Try
            Dim Parametro_Consulta = "select SQL_REPORTE FROM REPORTES_WORKFLOW WHERE " & _
            "ID_REPORTE =" & id_Reporte & " AND Estado_Reporte=1"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Datos_Sql_Reporte = "Imposible listar el reporte" & Result
                Exit Function
            End If
            If Datset Is Nothing Or Datset.Tables.Count = 0 Then
                Datos_Sql_Reporte = " Imposible listal tabla reportes tabla (0) "
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Datos_Sql_Reporte = "YES"
                Exit Function
            Else
                Datos_Sql = Trim(Datset.Tables(0).Rows(0).Item(0).ToString)
                Datos_Sql_Reporte = "YES"
                Exit Function
            End If
            Datos_Sql_Reporte = "YES"
        Catch ex As Exception
            Datos_Sql_Reporte = ex.Message
        End Try

    End Function
    Function Limpiar_Resultado_consulta(ByVal page1 As Page) As String
        Try
            Dim scripma As GridView = page1.FindControl("GridView_val_radicacion")
            If scripma Is Nothing Then
                Limpiar_Resultado_consulta = "YES"
                Exit Function
            End If
            Dim updat As UpdatePanel = page1.Page.FindControl("UpdatePanel_conenido")
            If updat Is Nothing Then
                Limpiar_Resultado_consulta = "Imposible encontrar el control  UpdatePanel_conenido"
                Exit Function
            End If
            Dim labetitle As Label = page1.FindControl("Label_resultado")
            labetitle.Text = "Se encontro " & "0" & " registro(s) en el reporte "
            scripma.DataSource = Nothing
            'scripma.DataKeyNames = DataKey
            scripma.DataBind()
            updat.Update()
            Limpiar_Resultado_consulta = "YES"
            Exit Function
        Catch ex As Exception
            Limpiar_Resultado_consulta = "Inconsistencia general función Limpiar_Resultado_consulta " & ex.Message
        End Try
    End Function
    Function Resultado_consulta(ByVal page1 As Page, _
                                ByRef sql_consulta As String, _
                                ByVal nombre_reporte As String) As String
        Try
            Dim scripma As GridView = page1.FindControl("GridView_val_radicacion")
            Dim labetitle As Label = page1.FindControl("Label_resultado")
            'Dim updatelabel As UpdatePanel = page1.FindControl(prefijocampo & "UpdatePanelabel_validacion")
            If scripma Is Nothing Then
                Resultado_consulta = "Imposible encontrar datagrid   GridView_val_radicacion"
                Exit Function
            End If
            Dim updat As UpdatePanel = page1.Page.FindControl("UpdatePanel_conenido")
            If updat Is Nothing Then
                Resultado_consulta = "Imposible encontrar el control  UpdatePanel_conenido"
                Exit Function
            End If
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("desxt")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Resultado_consulta = "Imposible Encontrar datos para listar " & Result
                labetitle.Text = "Se encontro " & "0" & " registro(s) en el reporte " & nombre_reporte
                scripma.DataSource = Nothing
                'scripma.DataKeyNames = DataKey
                scripma.DataBind()
                updat.Update()
                'updatelabel.Update()
                Exit Function

            End If
            If Datset.Tables(0).Rows.Count = 0 Then
               labetitle.Text = "Se encontro " & Datset.Tables(0).Rows.Count & " registro(s) en el reporte (" & nombre_reporte & ")"
                scripma.DataSource = Nothing
                scripma.DataBind()
                updat.Update()
                Resultado_consulta = "No se econtraron registros para el reporte"
                Exit Function
            Else
                labetitle.Text = "Se encontro " & Datset.Tables(0).Rows.Count & " registro(s) en el reporte (" & nombre_reporte & ")"
                scripma.DataSource = Datset
                scripma.DataBind()
                updat.Update()
                For i As Integer = 0 To scripma.Rows.Count - 1
                    For z As Integer = 0 To scripma.Rows(i).Cells.Count - 1
                        If z > 0 Then
                            scripma.Rows(i).Cells(z).Attributes.Add("Class", "GridviewScrollItem_line_cort_tr_flex")
                            scripma.Rows(i).Cells(z).Attributes.Add("onclick", "prevent_scrol(event,this);")
                        End If
                    Next
                Next
                updat.Update()
                Resultado_consulta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Resultado_consulta = "Inconsistencia general función Resultado_consulta " & ex.Message
        End Try
    End Function
   
    Function Sql_consulta(ByVal Conection_conectro_C As String, _
                          ByVal id_Reporte As String, _
                          ByRef Datos_Sql As String) As String
        Try
            Dim Parametro_Consulta = "select SQL_REPORTE FROM REPORTES_WORKFLOW WHERE " & _
            "ID_REPORTE =" & id_Reporte & " AND Estado_Reporte=1"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Sql_consulta = "Imposible listar el reporte" & Result
                Exit Function
            End If
            If Datset Is Nothing Or Datset.Tables.Count = 0 Then
                Sql_consulta = " Imposible listal tabla reportes tabla (0) "
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Sql_consulta = "YES"
                Exit Function
            Else
                Datos_Sql = Trim(Datset.Tables(0).Rows(0).Item(0).ToString)
                Sql_consulta = "YES"
                Exit Function
            End If
            Sql_consulta = "YES"
        Catch ex As Exception
            Sql_consulta = ex.Message
        End Try

    End Function
    Public Function NodoChild_Selecionado(ByRef Tre_vie As TreeView, ByRef Datos_Nodo As String) As String
        Try
            Datos_Nodo = ""
            Dim Result As String = ""
            If Tre_vie.Nodes.Count > 0 Then
                Dim i As Integer = 0
                For i = 0 To Tre_vie.Nodes.Count - 1
                    Result = Nod_CHILD(Tre_vie.Nodes(i), Datos_Nodo)
                    If Datos_Nodo <> "" Then
                        NodoChild_Selecionado = "YES"
                        Return NodoChild_Selecionado
                    End If
                Next
            End If
            NodoChild_Selecionado = "YES"
        Catch ex As Exception
            NodoChild_Selecionado = ex.ToString
        End Try
    End Function
    Public Function Nod_CHILD(ByVal NodeC As TreeNode, ByRef Datos_Nodo As String) As String
        Try
            Dim i As Integer = 0
            For i = 0 To NodeC.ChildNodes.Count - 1
                If NodeC.ChildNodes(i).Selected Then
                    Datos_Nodo = NodeC.ChildNodes(i).Value & "|" & NodeC.ChildNodes(i).Text
                    Nod_CHILD = "YES"
                    Return Nod_CHILD
                End If
                Nod_CHILD(NodeC.ChildNodes(i), Datos_Nodo)
            Next
            Nod_CHILD = "YES"
        Catch ex As Exception
            Nod_CHILD = ex.ToString
        End Try

    End Function
End Class
