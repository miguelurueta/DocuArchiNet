Imports Neodynamic.WebControls.ImageDraw
Imports System.IO
Imports MySql
Imports Dynamsoft


Public Class ClassNeodynamic
    
    Public Function Extraer_Documento_de_Multitif(ByRef pag As Page, ByRef fileby() As Byte, ByRef Matri_img() As String, ByVal Rutempo As String) As String
        Try
            ImageDraw.LicenseOwner = "Miguel Angel Urueta Miranda-Developer License"
            ImageDraw.LicenseKey = "28Q48MH26VEUUW84A4FH9YV8Q33LJ7PC6WF84EZF3AMC93SVP2FQ"
            Dim img As New ImageElement
            img.Source = Neodynamic.WebControls.ImageDraw.ImageSource.Binary
            img.SourceBinary = fileby
            'img.UseSourceDpi = True
            img.Dpi = 50      
            Dim noamis As New ImageDraw
            Erase Matri_img
            'FileUpload1.FileBytes()
            noamis.Elements.Add(img)
            If img.MultiPageCount > 0 Then

                For i As Integer = 0 To img.MultiPageCount - 1
                    img.MultiPageIndex = i
                    noamis.OutputImageName = "Page" + i.ToString()
                    If File.Exists(Rutempo + "\" + i.ToString + ".TIF") Then
                        Kill(Rutempo + "\" + i.ToString + ".TIF")
                    End If
                    noamis.Save(Rutempo + "\" + i.ToString + ".TIF", System.Drawing.Imaging.ImageFormat.Tiff)
                    ReDim Preserve Matri_img(i)
                    Matri_img(i) = Rutempo + "\" + i.ToString + ".TIF"
                Next
            Else
                noamis.OutputImageName = Rutempo + "\" + "Page0" + ".TIF"
                If File.Exists(Rutempo + "\" + "Page0" + ".TIF") Then
                    Kill(Rutempo + "\" + "Page0" + ".TIF")
                End If
                noamis.Save(Rutempo + "\" + "Page0" + ".TIF", System.Drawing.Imaging.ImageFormat.Tiff)
                ReDim Preserve Matri_img(0)
                Matri_img(0) = Rutempo + "\" + "Page0" + ".TIF"
            End If
            'img.MultiPageIndex(0)

            Extraer_Documento_de_Multitif = "YES"
        Catch ex As Exception
            Extraer_Documento_de_Multitif = "Extraer documento tif dice : " & ex.Message
        End Try
    End Function
    Public Function Extraer_Documento_de_Multitif_fisico(ByVal fil As String, _
                                                         ByRef Matri_img() As String, _
                                                         ByVal Rutempo As String) As String
        Try
            ImageDraw.LicenseOwner = "Miguel Angel Urueta Miranda-Developer License"
            ImageDraw.LicenseKey = "28Q48MH26VEUUW84A4FH9YV8Q33LJ7PC6WF84EZF3AMC93SVP2FQ"
            Dim REFDYNA As New Dynamsoft.DotNet.TWAIN.DynamicDotNetTwain
            REFDYNA.LicenseKeys = "8ED0684F604FDB0F79F98FA1456A8B16"
            Erase Matri_img
            For Each Archivo In My.Computer.FileSystem.GetFiles( _
                   Rutempo, _
                    FileIO.SearchOption.SearchTopLevelOnly, "*.*")
                Kill(Archivo)
            Next
            REFDYNA.RemoveAllImages()
            REFDYNA.MaxImagesInBuffer = 2000
            REFDYNA.LoadImage(fil)
            Dim b = REFDYNA.HowManyImagesInBuffer
            For i As Integer = 0 To b - 1
                'If File.Exists(Rutempo + "" + i.ToString + ".TIF") Then
                '    Kill(Rutempo + "" + i.ToString + ".TIF")
                'End If
                REFDYNA.SaveAsTIFF(Rutempo + "" + i.ToString + ".TIF", i)
                ReDim Preserve Matri_img(i)
                Matri_img(i) = Rutempo + "" + i.ToString + ".TIF"
            Next
            REFDYNA.RemoveAllImages()
            Extraer_Documento_de_Multitif_fisico = "YES"
        Catch ex As Exception
            Extraer_Documento_de_Multitif_fisico = "Fucion Extraer_Documento_de_Multitif_fisico Extraer documento tif dice : " & ex.Message
        End Try
    End Function
   
    Function Shape_Firma(ByVal Ruta_Firma As String, _
                         ByVal fx As Object, _
                         ByVal fy As Object, _
                         ByVal fwidth As Object, _
                         ByVal fheight As Object, _
                         ByVal Matri_Doc_Visual() As String, _
                         ByRef pag As Page, _
                         ByVal topcontenido As Integer, _
                         ByVal tamimagescrol As Object, _
                         ByVal tamimagen As String, _
                         ByVal heigimageor As String, _
                         ByVal witimageor As String) As String
        Try
            ImageDraw.LicenseOwner = "Miguel Angel Urueta Miranda-Developer License"
            ImageDraw.LicenseKey = "28Q48MH26VEUUW84A4FH9YV8Q33LJ7PC6WF84EZF3AMC93SVP2FQ"
            Dim callout As New CalloutShapeElement()
            Dim noami As ImageDraw = pag.FindControl("noaming")
            Dim noamiclon As New ImageDraw
            Dim updatevisor As UpdatePanel = pag.FindControl("UpdatePanelvisor")
            Dim ruta_fisica_firma As String = HttpContext.Current.Server.MapPath(HttpContext.Current.Session.Item("WF_RUTA_FIRMA_FINAL"))
            Dim shaopefirma As New ImageElement
            shaopefirma.SourceFile = ruta_fisica_firma
            'shaopefirma.SourceFile = HttpContext.Current.Session.Item("WF_RUTA_FIRMA_FINAL")
            'Dim escaleheig As Integer = HttpContext.Current.Session.Item("WF_IMAGE_HEIHG") / 10
            'Dim escalewith As Integer = HttpContext.Current.Session.Item("WF_IMAGE_WITH") / 10
            Dim Siz As New System.Drawing.Size
            Siz = noami.Elements(0).GetSize()
            Dim SizAnt As New System.Drawing.Size
            SizAnt = noami.Elements(0).GetSizeAfterActions
            Dim DiferenciaHEIG As String = Siz.Height / SizAnt.Height
            Dim DiferenciaWITH As String = Siz.Width / SizAnt.Width
            shaopefirma.Width = fwidth * DiferenciaHEIG
            shaopefirma.Height = fheight * DiferenciaWITH
            Dim taimaref As Integer = Val(tamimagen)
            shaopefirma.Y = (Val(fy) * DiferenciaHEIG)
            shaopefirma.X = (Val(fx) * DiferenciaWITH)
            'shaopefirma.X = Val(fx) + (Val(fwidth) * 2)
            'shaopefirma.Y = Val(fy)
            'shaopefirma.Y = taimaref + shaopefirma.Height
            'shaopefirma.Y = taimaref + (img_.Height)
            '**********************************************
            'Create an instance of MakeTransparent class
            '*********************************************
            Dim makeTransparent As New Neodynamic.WebControls.ImageDraw.MakeTransparent()
            makeTransparent.UseFirstPixel = True
            noami.Elements(0).Actions.RemoveAt(0)
            makeTransparent.UseFirstPixel = True
            shaopefirma.Actions.Add(makeTransparent)
            noami.Elements.Add(shaopefirma)
            noami.Save(Ruta_Firma)

            noami.Elements.RemoveAt(0)
            noami.Elements.Remove(shaopefirma)
            Dim img As New ImageElement
            img.SourceFile = Ruta_Firma
            noami.Elements.Add(img)
            Dim scal As New Neodynamic.WebControls.ImageDraw.Scale
            scal.HeightPercentage = HttpContext.Current.Session.Item("WF_IMAGE_HEIHG")
            scal.WidthPercentage = HttpContext.Current.Session.Item("WF_IMAGE_WITH")
            'Escala la imagen para que el usuario la pueda ver en un tamaño razonable
            noami.Elements(0).Actions.Add(scal)
            updatevisor.Update()
            Dim UpdatePanel_noaming As UpdatePanel = pag.FindControl("UpdatePanel_noaming")
            If Not UpdatePanel_noaming Is Nothing Then
                UpdatePanel_noaming.Update()
            End If
            Shape_Firma = "YES"
        Catch ex As Exception
            Shape_Firma = "Inconsistencia función Shape_Firma " & ex.Message
        End Try
    End Function
    Public Function Firma_transparente() As String
        Try
            ImageDraw.LicenseOwner = "Miguel Angel Urueta Miranda-Developer License"
            ImageDraw.LicenseKey = "28Q48MH26VEUUW84A4FH9YV8Q33LJ7PC6WF84EZF3AMC93SVP2FQ"
            Dim shaopefirma As New ImageElement
            shaopefirma.SourceFile = HttpContext.Current.Session("WF_RUTA_FIRMA")
            Dim makeTransparent As New Neodynamic.WebControls.ImageDraw.MakeTransparent()
            makeTransparent.UseFirstPixel = True
            'Apply the action on the ImageElement
            shaopefirma.Actions.Add(makeTransparent)
            Dim imgDraw As New Neodynamic.WebControls.ImageDraw.ImageDraw()
            imgDraw.Elements.Add(shaopefirma)
            imgDraw.Save(HttpContext.Current.Session("WF_RUTA_FIRMA"))
            Firma_transparente = "YES"
        Catch ex As Exception
            Firma_transparente = "Inconsistencia función Firma_transparente " & ex.Message
        End Try
    End Function
    Function Bytes_VSS(ByVal Bin As Byte(), _
                       ByVal Nombre_Ruta As String) As String
        Try
            ImageDraw.LicenseOwner = "Miguel Angel Urueta Miranda-Developer License"
            ImageDraw.LicenseKey = "28Q48MH26VEUUW84A4FH9YV8Q33LJ7PC6WF84EZF3AMC93SVP2FQ"
            'Dim oFileStream As FileStream
            Dim pathTemporal As String = HttpContext.Current.Server.MapPath(HttpContext.Current.Session("WF_RUTA_FIRMA") & Nombre_Ruta)
            If File.Exists(pathTemporal) Then File.Delete(pathTemporal)
            'oFileStream = New FileStream(pathTemporal, FileMode.CreateNew)
            'oFileStream.Write(Bin, 0, Bin.Length)
            'oFileStream.Close()
            'oFileStream = Nothing

            Dim imgDraw As New Neodynamic.WebControls.ImageDraw.ImageDraw()
            'Create an instance of ImageElement class
            Dim imgElem As New Neodynamic.WebControls.ImageDraw.ImageElement
            'Set the source property
            imgElem.Source = Neodynamic.WebControls.ImageDraw.ImageSource.Binary
            'Set the binary content of the image
            imgElem.SourceBinary = Bin
            imgDraw.Elements.Add(imgElem)
            imgDraw.Save(pathTemporal)
            Bytes_VSS = "YES"
        Catch ex As Exception
            Bytes_VSS = ex.Message.ToString
        End Try
        'AR1.LoadFile(pathTemporal)
        'If File.Exists(pathTemporal) Then File.Delete(pathTemporal)
    End Function

    Function Bajar_Firma_Plantilla_Wf(ByVal Parametro_Consulta As String, _
                                      ByVal Extension As String, _
                                      ByVal Mensaje As String)
        '--------------------------------------------------------------------
        'Funcion : Bajar archivo firma de usuario
        'Descripcion : Funcion que baja de la base
        'de datos los diferentes archivos y los guarda en  la base de datos
        'Ingeniero : Miguel Angel Urueta Miranda
        'Fecha : 2010-04-16
        '---------------------------------------------------------------------
        Try
            Dim bDatos() As Byte
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("FIRMAS WORKFLOW")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Bajar_Firma_Plantilla_Wf = "Error Consultando en tabla " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Bajar_Firma_Plantilla_Wf = "Imposible encontrar la firma del usuario "
                Exit Function
            Else
                Dim Tempvalor As Object = Datset.Tables(0).Rows(0).Item(0)
                If IsDBNull(Tempvalor) Then
                    Bajar_Firma_Plantilla_Wf = "Usuario sin " & Mensaje & " Registrada en la base de datos"
                    Exit Function
                Else
                    bDatos = CType(Datset.Tables(0).Rows(0).Item(0), Byte())
                End If
            End If

            '********************************************
            'Bajando el archivo firma  de mysql
            '********************************************
            Dim Result_Temporal As String = Bytes_VSS(bDatos, HttpContext.Current.Session("Id_Usuario_Workflow") & Extension)
            If Result_Temporal <> "YES" Then
                Bajar_Firma_Plantilla_Wf = Result_Temporal
                Exit Function
            End If
            Bajar_Firma_Plantilla_Wf = "YES"
            Exit Function
        Catch ex As Exception
            Bajar_Firma_Plantilla_Wf = "Error General funcion firma " & ex.Message
        End Try

    End Function
End Class
