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



Public Class ClassWorkflowReportes
    Private w As StreamWriter
    Private ruta As String
    Public xpath As String = ""
    Function Convertir_tif_pdf(ByVal Matri_documentos() As String, _
                               ByRef ruta_pdf As String, _
                               ByVal Ajusta As String, _
                               ByVal lectura As Integer, _
                               ByVal pas_word As String) As String

        Dim refile As New FileInfo(Matri_documentos(0))
        Dim Rut_pdf As String = refile.DirectoryName
        Dim Nombre_Archivo As String = refile.Name
        Nombre_Archivo = Nombre_Archivo.Replace(UCase(refile.Extension), ".PDF")
        ruta_pdf = ruta_pdf & Nombre_Archivo
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
            Convertir_tif_pdf = "YES"
        Catch ex As Exception
            Convertir_tif_pdf = "Inconsistencia general funcion Convertir_tif_pdf " & ex.Message
        Finally
            If oPdfDoc.IsOpen = True Then
                oPdfDoc.Close()
            End If
        End Try
    End Function
    Function Convertir_tif_pdf_correo(ByVal Matri_documentos() As String, _
                                      ByRef ruta_pdf As String, _
                                      ByVal Ajusta As String, _
                                      ByVal lectura As Integer, _
                                      ByVal pas_word As String, _
                                      ByVal contador As Integer) As String
        Dim refile As New FileInfo(Matri_documentos(0))
        Dim Rut_pdf As String = refile.DirectoryName
        Dim Nombre_Archivo As String = refile.Name
        Nombre_Archivo = Nombre_Archivo.Replace(UCase(refile.Extension), ".PDF")
        ruta_pdf = ruta_pdf & contador & "-" & Nombre_Archivo
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
            Convertir_tif_pdf_correo = "YES"
        Catch ex As Exception
            Convertir_tif_pdf_correo = "Inconsistencia general funcion Convertir_tif_pdf_correo " & ex.Message
        Finally
            If oPdfDoc.IsOpen = True Then
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
    Public Function Export(ByVal titulos As ArrayList, _
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
            Export = "Inconsistencia general funcion  Export " & ex.Message
        End Try

    End Function
    Public Function genera_xls(ByRef Datgridvi As GridView, _
                               ByRef tiparchivo As String, _
                               ByVal ruta_archivo As String, _
                               ByRef spli_header As String) As String
        Try
            Dim Result As String = ""
            Dim titulos As New ArrayList()
            Dim datosTabla As New DataTable()
            xpath = ruta_archivo
            ruta = ruta_archivo
            Dim ob = Datgridvi.DataSource
            Dim spli_header_matr() As String = spli_header.Split("|")
            'obtenemos los titulos del grid y creamos las columnas de la tabla  
            For gricolun As Integer = 0 To Datgridvi.HeaderRow.Cells.Count - 1
                titulos.Add(Datgridvi.HeaderRow.Cells(gricolun).Text)
                datosTabla.Columns.Add()
            Next
            'se crean los renglones de la tabla   
            Dim incre As Integer = 1
            For Each item As Object In Datgridvi.Rows
                Dim rowx As DataRow = datosTabla.NewRow()
                datosTabla.Rows.Add(rowx)
                incre = incre + 1
            Next
            For Each rowcell As GridViewRow In Datgridvi.Rows
                Dim i As Integer = 0
                For cell As Integer = 0 To rowcell.Cells.Count - 1
                    datosTabla.Rows(rowcell.RowIndex).Item(cell) = rowcell.Cells(cell).Text
                    i = i + i
                Next
            Next
            If tiparchivo = "test.csv" Then
                If Result <> "YES" Then
                    genera_xls = Result
                    Exit Function
                End If
            Else
                Result = Export(titulos, datosTabla)
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
    Function Listar_Reportes_Grupos_Treview(ByRef Tre_v2 As TreeView, _
                                            ByVal id_usuario As String, _
                                            ByVal Conectio_Documnent As String) As String
        Try
            Dim Matri_Rutas() As String
            Dim Matri_sPLIT() As String
            Dim i As Integer = 0
            Dim z As Integer = 0
            Dim Tre_v As New TreeNode
            Tre_v.ChildNodes.Clear()
            Tre_v.Text = "REPORTES DISPONIBLES"
            '*****************************
            'Solicitando listado de grupos
            '*****************************
            Dim Class_worflow_rutas As New Class_worflow_rutas
            Erase Matri_Rutas
            Dim Resultado As String = Class_worflow_rutas.Listar_Rutas_Workflow(Matri_Rutas)
            If Resultado <> "YES" Then
                Listar_Reportes_Grupos_Treview = Resultado
                Return Listar_Reportes_Grupos_Treview
            End If
            If Matri_Rutas Is Nothing Then
                Listar_Reportes_Grupos_Treview = "YES"
                Return Listar_Reportes_Grupos_Treview
            End If
            For i = 0 To UBound(Matri_Rutas)
                Erase Matri_sPLIT
                Matri_sPLIT = Split(Matri_Rutas(i), "|")
                Dim attrNodeGru As New TreeNode()
                attrNodeGru.Text = Matri_sPLIT(1)
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
                Resultado_User = Listar_Reportes_por_ruta(Matri_ReportesPar, _
                                                          Matri_sPLIT(0), _
                                                          id_usuario, _
                                                          Conectio_Documnent)
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
            Next
            Tre_v2.EnableViewState = True
            Tre_v2.Nodes.Add(Tre_v)
            Listar_Reportes_Grupos_Treview = "YES"
        Catch ex As Exception
            Listar_Reportes_Grupos_Treview = ex.ToString
        End Try
    End Function
    Function Listar_Reportes_por_ruta(ByRef Matri_Reportes() As String, _
                                      ByVal Id_Ruta As String, _
                                      ByVal id_usuario As String, _
                                      ByVal Conectio_Documnent As String) As String
        Try
            Dim Parametro_COnsulta As String = ""
            Parametro_COnsulta = "select id_reporte,nombre_reporte from  relacion_usuarios_workflow_reporte  as ruwr " & _
            "inner join reportes_workflow as rw on (ruwr.Reportes_Workflow_Id_Reporte=rw.ID_Reporte and rw.rutas_workflow_id_ruta=" & Id_Ruta & ") " & _
            " where ruwr.Usuarios_Reportes_ID_Usuario_Reporte =" & id_usuario
            Dim ref As New conect.Dbase_Conction_Mysql
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
            If scripma Is Nothing Then
                Resultado_consulta = "Imposible encontrar datagrid GridView_val_radicacion"
                Exit Function
            End If
            Dim updat As UpdatePanel = page1.Page.FindControl("UpdatePanel_conenido")
            If updat Is Nothing Then
                Resultado_consulta = "Imposible encontrar el control  UpdatePanel_conenido"
                Exit Function
            End If
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("desxt")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Resultado_consulta = "Imposible Encontrar datos para listar " & Result
                labetitle.Text = "Se encontro " & "0" & " registro(s) en el reporte " & nombre_reporte
                scripma.DataSource = Nothing
                scripma.DataBind()
                updat.Update()
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
                Resultado_consulta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Resultado_consulta = "Inconsistencia general función Resultado_consulta " & ex.Message
        End Try
    End Function
    Function Resultado_Consulta(ByRef Table_Ref As Table, _
                                ByVal ConectionC As String, _
                                ByVal Consulta As String) As String
        Try
            Table_Ref.Rows.Clear()
            Dim RegisTro As New TableRow
            Dim CellDa As New TableCell
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql
            'Dim Dat_set As New DataSet
            Dim Datset As New DataSet
            Dim ini As Integer = 0
            Dim i As Integer = 0
            Dim Conta_Registro As Integer = 0
            Dim Nomb_Colum() As String
            Dim Matri_Read() As String
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Consulta, Datset)
            If Result <> "YES" Then
                Resultado_Consulta = " Error consultando datos del reporte " & Consulta
                Return Resultado_Consulta
                Exit Function
            Else
                'verifica la existencia de record con el idruta
                If Datset.Tables(0).Rows.Count = 0 Then
                    Resultado_Consulta = "YES"
                    Return Resultado_Consulta
                    Exit Function

                Else
                    'Dat_reader.Read()
                    ini = Datset.Tables(0).Columns.Count
                    '*****************************************
                    'Agregar los titulos al array
                    '*****************************************
                    Erase Nomb_Colum
                    For i = 0 To ini - 1
                        ReDim Preserve Nomb_Colum(i)
                        Nomb_Colum(i) = Datset.Tables(0).Columns(i).ColumnName
                    Next
                    '********************************************************
                    'Agregar registros al array temporal
                    '********************************************************
                    Erase Matri_Read
                    Conta_Registro = 0
                    For i1 As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                        ReDim Preserve Matri_Read(Conta_Registro)
                        For i2 = 0 To ini
                            'Dim das As String = Matri_Read(Conta_Registro)
                            Try
                            
                                Dim Tempovalor As Object = Datset.Tables(0).Rows(i1).Item(i2)
                                If IsDBNull(Tempovalor) Then

                                    Matri_Read(Conta_Registro) = Matri_Read(Conta_Registro) & "NULL" & "+"
                                Else
                                    Matri_Read(Conta_Registro) = Matri_Read(Conta_Registro) & Tempovalor & "+"
                                End If
                            Catch ex As Exception
                                Matri_Read(Conta_Registro) = Matri_Read(Conta_Registro) & "ERROR" & "+"
                                'i = i + 1

                            End Try
                        Next
                        Conta_Registro = Conta_Registro + 1
                    Next
                End If
            End If
            
            '******************************************
            'Agregar los titulos de la coleccion field
            '******************************************
            Dim ica As Integer
            For ica = 0 To UBound(Nomb_Colum)
                CellDa = New TableCell
                CellDa.Text = Nomb_Colum(ica)
                CellDa.ID = Nomb_Colum(ica) & "8"
                CellDa.BorderStyle = BorderStyle.Outset
                CellDa.Wrap = False
                CellDa.BorderWidth = 1
                CellDa.ForeColor = Color.White
                CellDa.BackColor = Color.Navy
                RegisTro.Cells.Add(CellDa)
                Table_Ref.Rows.Add(RegisTro)

            Next
            '********************************************************
            'agregar los registros a las celdas
            '********************************************************
            RegisTro = New TableRow
            CellDa = New TableCell
            Dim BotnLin As Button
            For i = 0 To UBound(Matri_Read)
                'While reader.Read
                Dim icono As Integer
                RegisTro = New TableRow
                CellDa = New TableCell
                'CellDa.Wrap = False
                CellDa.BorderWidth = 1
                '*******CellDa.EnableViewState = True
                BotnLin = New Button
                BotnLin.EnableViewState = True
                BotnLin.ForeColor = Color.White
                BotnLin.BackColor = Color.MidnightBlue
                BotnLin.BorderStyle = BorderStyle.Ridge
                RegisTro.EnableViewState = True
                RegisTro.Width = Table_Ref.Width
                CellDa.Controls.Clear()

                '********************************************************
                'agregar los valores de los campo ocultos a la itentidads
                'del botton
                '********************************************************
                Dim Matri_Celd() As String
                Erase Matri_Celd
                Matri_Celd = Split(Matri_Read(i), "+")
                For ica = 0 To ini - 1
                    CellDa = New TableCell
                    CellDa.EnableViewState = True
                    CellDa.Text = Matri_Celd(ica)
                    CellDa.Font.Size = 10
                    RegisTro.Cells.Add(CellDa)
                Next
                Table_Ref.Rows.Add(RegisTro)
            Next
            Resultado_Consulta = "YES"
        Catch ex As Exception
            Resultado_Consulta = "Inconsistencia general función Resultado_Consulta " & ex.Message
        End Try
    End Function
    Function Sql_consulta(ByVal Conection_conectro_C As String, _
                          ByVal id_Reporte As String, _
                          ByRef Datos_Sql As String) As String
        Try
            Dim Parametro_Consulta = "select SQL_REPORTE FROM REPORTES_WORKFLOW WHERE " & _
            "ID_REPORTE =" & id_Reporte & " AND Estado_Reporte=1"
            Dim ref As New conect.Dbase_Conction_Mysql
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
