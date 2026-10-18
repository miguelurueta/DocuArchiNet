Imports iTextSharp.text
Imports iTextSharp.text.pdf
Imports iTextSharp.pdfa
Imports System.IO
Imports System.Text
Imports System.Xml
Imports iTextSharp.xmp


Public Structure stru_water_market
    Dim x As Single
    Dim y As Single
    Dim rotate_stamp As Single
    Dim ruta_archivo As String
    Dim scale_heit As Integer
    Dim scale_with As Integer
    Dim estado As Integer
    Dim blok_archivo As Object
End Structure
Public Class Class_itext_anotate
    Public pdf_file_src As Object
    Public pdf_file_page As Object
    Public pdf_file_width As Object
    Public pdf_file_heigth As Object
    Public anotate_file_src As Object
    Public anotate_file_bit As Object
    Public anotate_width As Object
    Public anotate_heigth As Object
    Public anotate_x As Object
    Public anotate_y As Object
    Public anotate_scale As Object
    Public anotate_type As String
    Public anotate_id_imagen As Object
    Public anotate_cabinete_imagen As Object
    Public anotate_radicado As Object
    Public anotate_id_workflow As Object
    Public anotate_desc_transacion As String
    Public num_tranaparent As String
    Public aplica_transparente As String
    Public rotation As String
End Class

Public Class Class_ItexShare
    Function Retorna_numero_paginas_documentos_unificados(ByVal ruta_documento As String,
                                                          ByRef numero_paginas As Integer) As String
        '-----------------------------------------------
        'Función : Retorna numero de paginas documentos
        'que no pertenecen a matrices de documentos
        'Ing : Miguel Angel Urueta Miranda
        '-----------------------------------------------
        Try
            numero_paginas = -1
            Dim Result As String = ""
            Dim file_inf As New FileInfo(ruta_documento)
            If UCase(file_inf.Extension) = ".PDF" Then
                Result = Retorna_numero_paginas_documentos_pdf(ruta_documento, numero_paginas)
                If Result <> "YES" Then
                    Retorna_numero_paginas_documentos_unificados = "Inconsistencia contando el numero de paginas del documento " & ruta_documento & " error " & Result
                End If
            End If
            Retorna_numero_paginas_documentos_unificados = "YES"
        Catch ex As Exception
            Retorna_numero_paginas_documentos_unificados = "Inconsistencia general función Retorna_numero_paginas_documentos_unificados " & ex.Message
        End Try
    End Function
    Function Retorna_numero_paginas_documentos_pdf(ByVal ruta_documento As String,
                                                   ByRef numero_paginas As Integer) As String
        '------------------------------------------------
        'Función Retorna el numero de paginas de un pdf
        'Ing Miguel Angel Urueta Miranda
        'Fecha : 2017-04-21
        '------------------------------------------------
        Dim rd As PdfReader
        Try
            Dim file_inf As New FileInfo(ruta_documento)
            If file_inf.Exists = False Then
                Retorna_numero_paginas_documentos_pdf = "El documento con la ruta relacionada no existe "
                Exit Function
            Else
                rd = New PdfReader(ruta_documento)
                numero_paginas = rd.NumberOfPages
                rd.Close()
                Retorna_numero_paginas_documentos_pdf = "YES"
                Exit Function
            End If
        Catch ex As Exception
            If Not rd Is Nothing Then
                rd.Close()
            End If
            Retorna_numero_paginas_documentos_pdf = "Inconsistencia general función Retorna_numero_paginas_documentos_pdf " & ex.Message
        Finally

        End Try
    End Function
    Function download_pdf_visor_plus(ByVal ur_pdf As String,
                                     ByRef url_pdf_download As String,
                                     ByRef url_path As String,
                                     ByRef name_file As String) As String
        Try
            Dim pat_user As String = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
            If pat_user = "" Or pat_user Is Nothing Then
                pat_user = "user_none"
            End If
            Dim path As String = HttpContext.Current.Server.MapPath("../Temp_Image/upload_file/" & pat_user & "/")
            If Directory.Exists(path) = False Then
                Directory.CreateDirectory(path)
            End If
            Dim pdf_file_src As String = ""
            Dim Result As String = Solicita_ruta_imagen(ur_pdf,
                                                       pdf_file_src)
            If Result <> "YES" Then
                download_pdf_visor_plus = Result
                Exit Function
            End If
            Dim fil As New FileInfo(pdf_file_src)
            name_file = fil.Name
            Dim pdf_file_copia As String = path & name_file & ".pdf"
            For Each deleteFile In Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly)
                File.Delete(deleteFile)
            Next
            File.Copy(pdf_file_src, pdf_file_copia)
            url_pdf_download = "../../Temp_Image/upload_file/" & pat_user & "/" & name_file & ".pdf"
            url_path = HttpContext.Current.Request.ApplicationPath & "/Temp_Image/upload_file/" & pat_user & "/" & name_file & ".pdf"
            download_pdf_visor_plus = "YES"
        Catch ex As Exception
            download_pdf_visor_plus = "Inconsistencia general funcion download_pdf_visor_plus " & ex.Message
        End Try
    End Function
    Private Const MaxGraphImageBytes As Integer = 10 * 1024 * 1024
    Private Const MaxGraphImageDimension As Integer = 4096

    Private NotInheritable Class PdfGraphOperationContext
        Public Property OperationId As String
        Public Property OperationDirectory As String
        Public Property SourceImagePath As String
        Public Property PreparedImagePath As String
        Public Property StagedPdfPath As String
        Public Property BackupPdfPath As String
        Public Property FailedPdfPath As String
        Public Property LockPath As String
    End Class

    Function add_anotate_image_pdf(ByVal itext_anotate As Class_itext_anotate,
                                   ByRef top As String,
                                   ByRef url_pagina As String) As String
        Dim loadingMode As String = EstrategiaCargaPdf.ModoStreamingCompleto
        Return add_anotate_image_pdf(itext_anotate, top, url_pagina, loadingMode)
    End Function

    Function add_anotate_image_pdf(ByVal itext_anotate As Class_itext_anotate,
                                   ByRef top As String,
                                   ByRef url_pagina As String,
                                   ByRef loadingMode As String) As String
        Dim operation As PdfGraphOperationContext = Nothing
        Dim documentLock As FileStream = Nothing
        Dim preserveBackup As Boolean = False
        Try
            Dim pdfFileSource As String = ""
            If Solicita_ruta_imagen(Convert.ToString(itext_anotate.pdf_file_src), pdfFileSource) <> "YES" OrElse
               Not File.Exists(pdfFileSource) Then
                Return "No fue posible localizar el documento"
            End If

            Dim documentInfo As New FileInfo(pdfFileSource)
            operation = CreateGraphOperation(documentInfo)
            Try
                documentLock = New FileStream(operation.LockPath,
                                              FileMode.OpenOrCreate,
                                              FileAccess.ReadWrite,
                                              FileShare.None)
            Catch ex As IOException
                Return "El documento está siendo actualizado"
            End Try

            Directory.CreateDirectory(operation.OperationDirectory)
            Dim preparedImage As String = ""
            Dim prepareResult As String = PrepareGraphImage(itext_anotate, operation, preparedImage)
            If prepareResult <> "YES" Then Return prepareResult

            Dim pageNumber As Integer = Convert.ToInt32(itext_anotate.pdf_file_page)
            Dim rotation As Integer = Convert.ToInt32(itext_anotate.rotation)
            Dim canvasHeight As Double = Convert.ToDouble(itext_anotate.pdf_file_heigth)
            Dim canvasWidth As Double = Convert.ToDouble(itext_anotate.pdf_file_width)
            Dim annotationX As Double = Convert.ToDouble(itext_anotate.anotate_x)
            Dim annotationY As Double = Convert.ToDouble(itext_anotate.anotate_y)
            Dim annotationHeight As Double = Convert.ToDouble(itext_anotate.anotate_heigth)
            Dim originalPageCount As Integer = 0
            Dim reader As PdfReader = Nothing
            Dim stamper As PdfStamper = Nothing
            Dim outputStream As FileStream = Nothing

            Try
                reader = New PdfReader(pdfFileSource)
                originalPageCount = reader.NumberOfPages
                If pageNumber < 1 OrElse pageNumber > originalPageCount Then
                    Return "La página solicitada no es válida"
                End If

                Dim pageSize = reader.GetPageSize(pageNumber)
                Dim crop = reader.GetCropBox(pageNumber)
                Dim matrix = GeometriaGrafoPdf.Matriz(
                    Math.Max(pageSize.Left, crop.Left), Math.Max(pageSize.Bottom, crop.Bottom),
                    Math.Min(pageSize.Right, crop.Right), Math.Min(pageSize.Top, crop.Top),
                    rotation, canvasWidth, canvasHeight, annotationX, annotationY,
                    Convert.ToDouble(itext_anotate.anotate_width), annotationHeight)

                outputStream = New FileStream(operation.StagedPdfPath,
                                              FileMode.CreateNew,
                                              FileAccess.Write,
                                              FileShare.None)
                stamper = New PdfStamper(reader, outputStream)
                ' PDF.js usa rotation explícita: no aplicar además el /Rotate del documento.
                stamper.RotateContents = False
                Dim graphImage As iTextSharp.text.Image = iTextSharp.text.Image.GetInstance(preparedImage)
                stamper.GetOverContent(pageNumber).AddImage(graphImage,
                    CSng(matrix(0)), CSng(matrix(1)), CSng(matrix(2)),
                    CSng(matrix(3)), CSng(matrix(4)), CSng(matrix(5)))
                stamper.Close()
                stamper = Nothing
                outputStream = Nothing
                reader.Close()
                reader = Nothing
            Finally
                If stamper IsNot Nothing Then stamper.Close()
                If outputStream IsNot Nothing Then outputStream.Dispose()
                If reader IsNot Nothing Then reader.Close()
            End Try

            If Not ValidatePdf(operation.StagedPdfPath, originalPageCount, pageNumber) Then
                Return "No fue posible validar el documento actualizado"
            End If
            If Not ReplacePdfSafely(operation,
                                    pdfFileSource,
                                    originalPageCount,
                                    pageNumber,
                                    preserveBackup) Then
                Return "No fue posible confirmar el documento actualizado"
            End If

            loadingMode = EstrategiaCargaPdf.Seleccionar(pdfFileSource)
            url_pagina = BuildRefreshUrl(Convert.ToString(itext_anotate.pdf_file_src), operation.OperationId)
            top = "YES"
            Return "YES"
        Catch
            Return "No fue posible actualizar el documento"
        Finally
            If documentLock IsNot Nothing Then documentLock.Dispose()
            CleanupGraphOperation(operation, preserveBackup)
        End Try
    End Function

    Private Function CreateGraphOperation(ByVal documentInfo As FileInfo) As PdfGraphOperationContext
        Dim operationId As String = Guid.NewGuid().ToString("N")
        Dim userId As String = Convert.ToString(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
        If String.IsNullOrWhiteSpace(userId) Then userId = "user_none"
        Dim operationDirectory As String = HttpContext.Current.Server.MapPath(
            "../Temp_Image/upload_file/" & userId & "/" & operationId & "/")
        Dim prefix As String = "." & documentInfo.Name & "." & operationId
        Return New PdfGraphOperationContext With {
            .OperationId = operationId,
            .OperationDirectory = operationDirectory,
            .SourceImagePath = Path.Combine(operationDirectory, "source.png"),
            .PreparedImagePath = Path.Combine(operationDirectory, "prepared.png"),
            .StagedPdfPath = Path.Combine(documentInfo.DirectoryName, prefix & ".staged.pdf"),
            .BackupPdfPath = Path.Combine(documentInfo.DirectoryName, prefix & ".backup.pdf"),
            .FailedPdfPath = Path.Combine(documentInfo.DirectoryName, prefix & ".failed.pdf"),
            .LockPath = Path.Combine(documentInfo.DirectoryName, "." & documentInfo.Name & ".graph.lock")
        }
    End Function

    Private Function PrepareGraphImage(ByVal annotation As Class_itext_anotate,
                                       ByVal operation As PdfGraphOperationContext,
                                       ByRef preparedImage As String) As String
        Dim fileSource As String = Convert.ToString(annotation.anotate_file_src)
        Dim encodedSource As String = Convert.ToString(annotation.anotate_file_bit)
        Dim hasFile As Boolean = Not String.IsNullOrWhiteSpace(fileSource)
        Dim hasBase64 As Boolean = Not String.IsNullOrWhiteSpace(encodedSource)
        If hasFile = hasBase64 Then Return "Debe proporcionar una única imagen válida"

        Dim sourcePath As String = ""
        If hasFile Then
            Dim relativeSource As String = fileSource.Replace("../", "")
            sourcePath = HttpContext.Current.Server.MapPath("../" & relativeSource)
            If Not File.Exists(sourcePath) Then Return "No fue posible localizar la imagen"
        Else
            Const prefix As String = "data:image/png;base64,"
            If encodedSource.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) Then
                encodedSource = encodedSource.Substring(prefix.Length)
            End If
            If encodedSource.Length > ((MaxGraphImageBytes + 2) \ 3) * 4 Then
                Return "La imagen supera el tamaño permitido"
            End If
            Dim imageBytes As Byte()
            Try
                imageBytes = Convert.FromBase64String(encodedSource)
            Catch
                Return "La imagen codificada no es válida"
            End Try
            If imageBytes.Length = 0 OrElse imageBytes.Length > MaxGraphImageBytes Then
                Return "La imagen supera el tamaño permitido"
            End If
            File.WriteAllBytes(operation.SourceImagePath, imageBytes)
            sourcePath = operation.SourceImagePath
        End If

        Try
            Using sourceBitmap As New System.Drawing.Bitmap(sourcePath)
                If sourceBitmap.Width < 1 OrElse sourceBitmap.Height < 1 OrElse
                   sourceBitmap.Width > MaxGraphImageDimension OrElse
                   sourceBitmap.Height > MaxGraphImageDimension Then
                    Return "Las dimensiones de la imagen no son válidas"
                End If

                Dim outputBitmap As System.Drawing.Bitmap = Nothing
                Try
                    If Convert.ToInt32(annotation.aplica_transparente) = 1 Then
                        Dim transparent As New Class_transparent
                        Dim transparentResult As String = ""
                        outputBitmap = transparent.Transparent(sourceBitmap,
                                                               System.Drawing.Color.White,
                                                               Convert.ToInt32(annotation.num_tranaparent),
                                                               transparentResult)
                        If transparentResult <> "YES" OrElse outputBitmap Is Nothing Then
                            Return "No fue posible aplicar transparencia"
                        End If
                    Else
                        outputBitmap = New System.Drawing.Bitmap(sourceBitmap)
                    End If
                    outputBitmap.Save(operation.PreparedImagePath,
                                      System.Drawing.Imaging.ImageFormat.Png)
                Finally
                    If outputBitmap IsNot Nothing Then outputBitmap.Dispose()
                End Try
            End Using
        Catch
            Return "La imagen no tiene un formato válido"
        End Try

        preparedImage = operation.PreparedImagePath
        Return "YES"
    End Function

    Private Function ValidatePdf(ByVal filePath As String,
                                 ByVal expectedPages As Integer,
                                 ByVal targetPage As Integer) As Boolean
        If Not File.Exists(filePath) OrElse New FileInfo(filePath).Length = 0 Then Return False
        Dim reader As PdfReader = Nothing
        Try
            reader = New PdfReader(filePath)
            Return reader.NumberOfPages = expectedPages AndAlso
                   targetPage >= 1 AndAlso targetPage <= reader.NumberOfPages
        Catch
            Return False
        Finally
            If reader IsNot Nothing Then reader.Close()
        End Try
    End Function

    Private Function ReplacePdfSafely(ByVal operation As PdfGraphOperationContext,
                                      ByVal originalPath As String,
                                      ByVal expectedPages As Integer,
                                      ByVal targetPage As Integer,
                                      ByRef preserveBackup As Boolean) As Boolean
        Try
            File.Replace(operation.StagedPdfPath, originalPath, operation.BackupPdfPath, True)
        Catch
            Return False
        End Try

        If ValidatePdf(originalPath, expectedPages, targetPage) Then
            Try
                If File.Exists(operation.BackupPdfPath) Then File.Delete(operation.BackupPdfPath)
            Catch
                ' El PDF ya fue confirmado; la limpieza final volverá a intentar borrar el backup propio.
            End Try
            Return True
        End If

        Try
            File.Replace(operation.BackupPdfPath, originalPath, operation.FailedPdfPath, True)
            If File.Exists(operation.FailedPdfPath) Then File.Delete(operation.FailedPdfPath)
        Catch
            preserveBackup = File.Exists(operation.BackupPdfPath)
        End Try
        Return False
    End Function

    Private Function BuildRefreshUrl(ByVal sourceUrl As String, ByVal operationId As String) As String
        Dim separator As String = If(sourceUrl.IndexOf("?", StringComparison.Ordinal) >= 0, "&", "?")
        Return sourceUrl & separator & "doc104_refresh=" & operationId
    End Function

    Private Sub CleanupGraphOperation(ByVal operation As PdfGraphOperationContext,
                                      ByVal preserveBackup As Boolean)
        If operation Is Nothing Then Return
        Try
            If Directory.Exists(operation.OperationDirectory) Then
                Directory.Delete(operation.OperationDirectory, True)
            End If
        Catch
        End Try
        For Each ownedPath As String In New String() {operation.StagedPdfPath, operation.FailedPdfPath}
            Try
                If File.Exists(ownedPath) Then File.Delete(ownedPath)
            Catch
            End Try
        Next
        If Not preserveBackup Then
            Try
                If File.Exists(operation.BackupPdfPath) Then File.Delete(operation.BackupPdfPath)
            Catch
            End Try
        End If
        Try
            If File.Exists(operation.LockPath) Then File.Delete(operation.LockPath)
        Catch
        End Try
    End Sub
    Function Solicita_ruta_imagen(ByVal src_source_file As String,
                                  ByRef src_opout_file As String) As String
        Try
            If InStr(src_source_file, "=") > 0 Then
                Dim split_ruta = src_source_file.Split("=")
                src_opout_file = split_ruta(1).Replace("|", "\")
                Solicita_ruta_imagen = "YES"
                Exit Function
            Else
                src_opout_file = src_source_file.Replace("|", "\")
                src_opout_file = HttpContext.Current.Server.MapPath(src_opout_file)
                Solicita_ruta_imagen = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_ruta_imagen = "Inconsistencia general funcion Solicita_ruta_imagen " & ex.Message
        End Try
    End Function
    Function Create_file_pagina(ByVal url_file As String,
                                ByVal ur_temp As String,
                                ByVal user As String,
                                ByVal page As Integer,
                                ByRef ur_file_resul As String) As String
        Dim reader_source As iTextSharp.text.pdf.PdfReader = Nothing
        Dim oPdfDoc_destino As PdfCopy = Nothing
        Dim document As Document = Nothing
        Try
            Dim file_path As String = ur_temp & user & ".pdf"
            If File.Exists(file_path) Then
                File.Delete(file_path)
            End If
            reader_source = New iTextSharp.text.pdf.PdfReader(url_file)
            document = New Document
            oPdfDoc_destino = New PdfCopy(document, New FileStream(file_path, FileMode.Create))
            document.Open()
            Dim page_ As PdfImportedPage = Nothing
            page_ = oPdfDoc_destino.GetImportedPage(reader_source, page)
            oPdfDoc_destino.AddPage(page_)
            oPdfDoc_destino.Close()
            reader_source.Close()
            ur_file_resul = file_path
            document.Close()
            Create_file_pagina = "YES"
        Catch ex As Exception
            If Not oPdfDoc_destino Is Nothing Then
                oPdfDoc_destino.Close()
            End If
            If Not reader_source Is Nothing Then
                reader_source.Close()
            End If
            If Not document Is Nothing Then
                document.Close()
            End If
            Create_file_pagina = "Inconsistencia general funcion Create_file_pagina " & ex.Message
        End Try
    End Function
    Function AddWatermarkImage(ByVal sourceFile As String,
                               ByVal outputFile As String,
                               ByVal watermarkImage As String) As String
        Dim reader As iTextSharp.text.pdf.PdfReader = Nothing
        Dim stamper As iTextSharp.text.pdf.PdfStamper = Nothing
        Dim img As iTextSharp.text.Image = Nothing
        Dim underContent As iTextSharp.text.pdf.PdfContentByte = Nothing
        Dim rect As iTextSharp.text.Rectangle = Nothing
        Dim X, Y As Single
        Dim pageCount As Integer = 0
        Try
            If System.IO.File.Exists(sourceFile) Then
                reader = New iTextSharp.text.pdf.PdfReader(sourceFile)
                rect = reader.GetPageSizeWithRotation(1)
                stamper = New iTextSharp.text.pdf.PdfStamper(reader, New System.IO.FileStream(outputFile, System.IO.FileMode.Create))
                img = iTextSharp.text.Image.GetInstance(watermarkImage)
                If img.Width > rect.Width OrElse img.Height > rect.Height Then
                    img.ScaleToFit(rect.Width, rect.Height)
                    X = (rect.Width - img.ScaledWidth) / 2
                    Y = (rect.Height - img.ScaledHeight) / 2
                Else
                    X = (rect.Width - img.Width) / 2
                    Y = (rect.Height - img.Height) / 2
                End If
                X = rect.Width - img.Width
                'Y = img.Height / 10
                Y = -2
                img.SetAbsolutePosition(X, Y)
                pageCount = reader.NumberOfPages()
                For i As Integer = 1 To pageCount
                    underContent = stamper.GetOverContent(i)
                    underContent.AddImage(img)
                Next
                stamper.Close()
                reader.Close()
                AddWatermarkImage = "YES"
                Exit Function
            Else
                AddWatermarkImage = "File Does Not Exist Missing File"
            End If
        Catch ex As Exception
            AddWatermarkImage = ex.Message
        End Try
    End Function

    Function Lee_Archivo_Cachexml_Stamp(ByRef Leer_Archivo() As stru_water_market,
                                        ByVal Ruta_Carpeta As String,
                                        ByVal ruta_archivo_image As String) As String
        '***************************************************
        'Funcion : Lee_Archivo_Cachexml_Carpeta
        'Descripcion : le archivo cache carpeta para listar
        'los documentos en el listview
        '***************************************************
        Try
            Dim Xml = New XmlDocument()
            Dim Resul As String = ""
            If System.IO.File.Exists(Ruta_Carpeta) = True Then
                Xml.Load(Ruta_Carpeta)
            Else
                Lee_Archivo_Cachexml_Stamp = "Imposible encontrar el archivo " & Ruta_Carpeta & "TempoRegistro.xml"
                Exit Function
            End If
            Dim Valor_Matri As Integer = 0
            Dim Xml2 As XmlNodeList
            Xml2 = Xml.SelectNodes("/STAMP/Tarea")
            Dim xmlAttr As XmlNode
            For Each xmlAttr In Xml2
                With xmlAttr.Attributes
                    ReDim Preserve Leer_Archivo(Valor_Matri)
                    If .Count > 0 Then
                        Leer_Archivo(Valor_Matri).ruta_archivo = ruta_archivo_image & .GetNamedItem("Ruta_archivo").Value()
                        Leer_Archivo(Valor_Matri).x = .GetNamedItem("X").Value()
                        Leer_Archivo(Valor_Matri).y = .GetNamedItem("Y").Value()
                        Leer_Archivo(Valor_Matri).rotate_stamp = .GetNamedItem("rotate_stamp").Value()
                        Leer_Archivo(Valor_Matri).estado = .GetNamedItem("estado_stamp").Value()
                        Valor_Matri = Valor_Matri + 1
                    End If
                End With
            Next
            Lee_Archivo_Cachexml_Stamp = "YES"
        Catch ex As Exception
            Lee_Archivo_Cachexml_Stamp = "Error General Funcion Lee_Archivo_Cachexml_Stamp " & ex.Message
        End Try
    End Function
    Function AddWatermarkImage(ByVal sourceFile As String,
                               ByVal outputFile As String,
                               ByVal watermarkImage() As stru_water_market) As String
        Dim reader As iTextSharp.text.pdf.PdfReader = Nothing
        Dim stamper As iTextSharp.text.pdf.PdfStamper = Nothing
        Dim img() As iTextSharp.text.Image = Nothing
        Dim underContent As iTextSharp.text.pdf.PdfContentByte = Nothing
        Dim rect As iTextSharp.text.Rectangle = Nothing
        Dim X, Y As Single
        Dim pageCount As Integer = 0
        Try
            If System.IO.File.Exists(sourceFile) Then
                reader = New iTextSharp.text.pdf.PdfReader(sourceFile)
                rect = reader.GetPageSizeWithRotation(1)
                stamper = New iTextSharp.text.pdf.PdfStamper(reader, New System.IO.FileStream(outputFile, System.IO.FileMode.Create))
                '****************************************************
                'Agrega las imagenes de stamp
                '****************************************************
                Dim incre As Integer = 0
                For i2 As Integer = 0 To watermarkImage.Length - 1
                    If watermarkImage(i2).estado = 1 Then
                        ReDim Preserve img(incre)
                        img(incre) = iTextSharp.text.Image.GetInstance(watermarkImage(i2).ruta_archivo)
                        If img(incre).Width > rect.Width OrElse img(incre).Height > rect.Height Then
                            img(incre).ScaleToFit(rect.Width, rect.Height)

                            'X = (rect.Width - img(i2).ScaledWidth) / 2
                            'Y = (rect.Height - img(i2).ScaledHeight) / 2
                        Else
                            'X = (rect.Width - img(i2).Width) / 2
                            'Y = (rect.Height - img(i2).Height) / 2
                        End If
                        'X = rect.Width - img(i2).Width
                        ''Y = img.Height / 10
                        'Y = -2
                        If watermarkImage(i2).scale_heit > 0 Then
                            img(incre).ScaleAbsoluteHeight(watermarkImage(i2).scale_heit)
                        End If
                        If watermarkImage(i2).scale_with > 0 Then
                            img(incre).ScaleAbsoluteWidth(watermarkImage(i2).scale_with)
                        End If
                        img(incre).RotationDegrees = watermarkImage(i2).rotate_stamp
                        img(incre).SetAbsolutePosition(watermarkImage(i2).x, watermarkImage(i2).y)
                        incre = incre + 1
                    End If
                Next
                pageCount = reader.NumberOfPages()
                For i As Integer = 1 To pageCount
                    underContent = stamper.GetOverContent(i)
                    For i3 As Integer = 0 To img.Length - 1
                        underContent.AddImage(img(i3))
                    Next
                Next
                stamper.Close()
                reader.Close()
                AddWatermarkImage = "YES"
                Exit Function
            Else
                AddWatermarkImage = "File Does Not Exist Missing File"
            End If
        Catch ex As Exception
            AddWatermarkImage = ex.Message
        End Try
    End Function
    Function AddWatermarkImagePdf(ByVal sourceFile As String,
                                  ByVal outputFile As String,
                                  ByVal watermarkImage() As stru_water_market) As String
        '---------------------------------------------------------------------------
        'Funcion : Adiciona al documento PDF una imagen en la posición predeterminada
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'Source_File           : Representa el archivo pdf fuenta a stampar
        'watermarkImage        : Estructura del posicionamiento de una imagen
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'outputFile            : Retorna el archivo con la estampa de la imagen
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-12-15
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------

        Dim reader As iTextSharp.text.pdf.PdfReader = Nothing
        Dim stamper As iTextSharp.text.pdf.PdfStamper = Nothing
        Dim img() As iTextSharp.text.Image = Nothing
        Dim underContent As iTextSharp.text.pdf.PdfContentByte = Nothing
        Dim rect As iTextSharp.text.Rectangle = Nothing
        Dim X, Y As Single
        Dim pageCount As Integer = 0
        Try
            If System.IO.File.Exists(sourceFile) Then
                reader = New iTextSharp.text.pdf.PdfReader(sourceFile)
                rect = reader.GetPageSizeWithRotation(1)
                stamper = New iTextSharp.text.pdf.PdfStamper(reader, New System.IO.FileStream(outputFile, System.IO.FileMode.Create))
                '****************************************************
                'Agrega las imagenes de stamp
                '****************************************************
                Dim incre As Integer = 0
                For i2 As Integer = 0 To watermarkImage.Length - 1
                    If watermarkImage(i2).estado = 1 Then
                        ReDim Preserve img(incre)
                        img(incre) = iTextSharp.text.Image.GetInstance(watermarkImage(i2).ruta_archivo)
                        If img(incre).Width > rect.Width OrElse img(incre).Height > rect.Height Then
                            img(incre).ScaleToFit(rect.Width, rect.Height)
                            'X = (rect.Width - img(i2).ScaledWidth) / 2
                            'Y = (rect.Height - img(i2).ScaledHeight) / 2
                        Else
                            'X = (rect.Width - img(i2).Width) / 2
                            'Y = (rect.Height - img(i2).Height) / 2
                        End If
                        'X = rect.Width - img(i2).Width
                        ''Y = img.Height / 10
                        'Y = -2
                        If watermarkImage(i2).scale_heit > 0 Then
                            img(incre).ScaleAbsoluteHeight(watermarkImage(i2).scale_heit)
                        End If
                        If watermarkImage(i2).scale_with > 0 Then
                            img(incre).ScaleAbsoluteWidth(watermarkImage(i2).scale_with)
                        End If
                        img(incre).RotationDegrees = watermarkImage(i2).rotate_stamp
                        img(incre).SetAbsolutePosition(watermarkImage(i2).x, watermarkImage(i2).y)
                        incre = incre + 1
                    End If
                Next
                pageCount = reader.NumberOfPages()
                For i As Integer = 1 To pageCount
                    underContent = stamper.GetOverContent(i)
                    For i3 As Integer = 0 To img.Length - 1
                        underContent.AddImage(img(i3))
                    Next
                Next
                stamper.Close()
                reader.Close()
                AddWatermarkImagePdf = "YES"
                Exit Function
            Else
                AddWatermarkImagePdf = "File Does Not Exist Missing File"
            End If
        Catch ex As Exception
            AddWatermarkImagePdf = "Inconsistencia general funcion AddWatermarkImagePdf (" & ex.Message & ")"
        End Try
    End Function
    Function AddWaterPdf_mutiple(ByVal Rutafile() As String,
                                  ByVal Rutafileout() As String,
                                  ByVal Ruta_Original() As String,
                                  ByVal watermarkImage() As stru_water_market,
                                  ByVal optio_marcar As Integer,
                                  ByVal matri_archivos_selec() As String) As String
        '****************************************************
        'Funcion : Genera marca de agua por encima con el
        'proposito de validar la informacion impresa
        'Fecha:2013-03-15
        'Ing : Miguel Angel Urueta Miranda
        '****************************************************
        Try
            Dim Refclascarpeta As New ClassCarpetas
            If optio_marcar = 1 Then
                If watermarkImage Is Nothing Then
                    AddWaterPdf_mutiple = "Imposible encontrar stamp cargados "
                    Exit Function
                End If
                Dim pase_watrer As Integer = 0
                For iz As Integer = 0 To watermarkImage.Length - 1
                    If watermarkImage(iz).estado = 1 Then
                        pase_watrer = 1
                    End If
                Next
                If pase_watrer = 0 Then
                    AddWaterPdf_mutiple = "Usted esta tratando de colocar un estamp, y el sistema no tiene ningún estamp activo"
                    Exit Function
                End If
            End If
            Dim Result As String = ""
            Dim Refclas As New ClassReportesGestor
            For z As Integer = 0 To Rutafileout.Length - 1
                Result = Refclascarpeta.Exportar_Documentos_Gabinete(Ruta_Original(z),
                                                                     "PDF",
                                                                     Rutafile(z),
                                                                     "NONE",
                                                                     matri_archivos_selec)
                If Result <> "YES" Then
                    AddWaterPdf_mutiple = Result
                    Exit Function
                End If
                If optio_marcar = 1 Then
                    Result = AddWatermarkImage(Rutafile(z),
                                               Rutafileout(z),
                                               watermarkImage)
                    If Result <> "YES" Then
                        AddWaterPdf_mutiple = Result
                        Exit Function
                    End If
                Else
                    Rutafileout(z) = Rutafile(z)
                End If
            Next
            AddWaterPdf_mutiple = "YES"
        Catch ex As Exception
            AddWaterPdf_mutiple = "Inconsistencia general funcion AddWaterPadf Mensage : " & ex.Message
        End Try
    End Function
    Function AddWater_image_Pdf(ByVal Source_file_pdf As String,
                                ByVal watermarkImage() As stru_water_market,
                                ByVal optio_marcar As Integer,
                                ByRef Oup_File_pdf As String) As String
        '---------------------------------------------------------------------------
        'Funcion : Stampa imagen a pdf existente en la posición
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'Source_File           : Representa el archivo pdf fuenta a stampar
        'watermarkImage        : Representa la estructura de la marca para posesionar
        '                        la imagen dentro del pdf
        'optio_marcar          : Representa si marca el pdf
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'Oup_File_pdf          : Retorna el archivo marcado
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-12-15
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            If optio_marcar = 1 Then
                If watermarkImage Is Nothing Then
                    AddWater_image_Pdf = "Imposible encontrar stamp cargados "
                    Exit Function
                End If
                Dim pase_watrer As Integer = 0
                For iz As Integer = 0 To watermarkImage.Length - 1
                    If watermarkImage(iz).estado = 1 Then
                        pase_watrer = 1
                    End If
                Next
                If pase_watrer = 0 Then
                    AddWater_image_Pdf = "Usted esta tratando de colocar un estamp, y el sistema no tiene ningún estamp activo"
                    Exit Function
                End If
            End If
            If optio_marcar = 1 Then
                Result = AddWatermarkImagePdf(Source_file_pdf,
                                              Oup_File_pdf,
                                              watermarkImage)
                If Result <> "YES" Then
                    AddWater_image_Pdf = Result
                    Exit Function
                End If
            End If
            AddWater_image_Pdf = "YES"
        Catch ex As Exception
            AddWater_image_Pdf = "Inconsistencia general función AddWater_Pdf (" & ex.Message & ")"
        End Try
    End Function
    Function Stamp_image_pdf_seguridad(ByVal Source_File As String,
                                     ByRef Fileout_final As String,
                                     ByVal optio_marcar As Integer) As String
        '---------------------------------------------------------------------------
        'Funcion : Stampa imagen a pdf existente en la posición
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'Source_File           : Representa el archivo pdf fuenta a stampar
        'Fileout_final         : 
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'id_usuario_radicador  : Retorna la idnetificación del usuario radicador
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-12-15
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try

            Dim i As Integer = 0
            Dim Result As String = ""
            Dim stru_water_market() As stru_water_market
            Erase stru_water_market
            Dim ruta_carpeta_user As String = HttpContext.Current.Server.MapPath("../Docuarchi/usuarios/" + HttpContext.Current.Session.Item("DA_Login_Usuario"))
            If System.IO.Directory.Exists(ruta_carpeta_user) = False Then
                System.IO.Directory.CreateDirectory(ruta_carpeta_user)
            End If
            Dim ruta_archivo_confing As String = HttpContext.Current.Server.MapPath("../Docuarchi/configuraciongeneral/Sellos_Temp/")
            Result = Me.Lee_Archivo_Cachexml_Stamp(stru_water_market,
                                                   HttpContext.Current.Server.MapPath("../Docuarchi/configuraciongeneral/Sellos_Temp/TempoRegistroPdf.xml"),
                                                   ruta_archivo_confing)
            If Result <> "YES" Then
                Stamp_image_pdf_seguridad = Result
                Exit Function
            End If
            Dim fil As New System.IO.FileInfo(Source_File)
            Dim Filname As String = fil.Name
            Fileout_final = ruta_carpeta_user & "\" & fil.Name
            Dim Class_fyle_system As New Class_fyle_system
            Class_fyle_system.Delete_directory(ruta_carpeta_user)
            'For Each deleteFile In Directory.GetFiles(ruta_carpeta_user, "*.*", SearchOption.TopDirectoryOnly)
            '    File.Delete(deleteFile)
            'Next
            Result = Me.AddWater_image_Pdf(Source_File,
                                           stru_water_market,
                                           optio_marcar,
                                           Fileout_final)

            If Result <> "YES" Then
                Stamp_image_pdf_seguridad = Result
                Exit Function
            End If
            Stamp_image_pdf_seguridad = "YES"
        Catch ex As Exception
            Stamp_image_pdf_seguridad = "Inconsistencia general función  Stamp_image_pdf_seguridad (" & ex.Message & ")"
        End Try
    End Function
    Function stamp_pdf_seguridad(ByVal matri_archivos() As String,
                                 ByRef Fileout_final() As String,
                                 ByVal optio_marcar As Integer,
                                 ByVal matri_archivos_selec() As String) As String
        Try
            Dim Fileout() As String
            Dim Ruta_Original() As String
            Erase Ruta_Original
            Erase Fileout
            Erase Fileout_final
            Dim i As Integer = 0
            Dim Result As String = ""
            Dim stru() As stru_water_market
            Erase stru
            Dim ruta_carpeta_user As String = HttpContext.Current.Server.MapPath("../Docuarchi/usuarios/" + HttpContext.Current.Session.Item("DA_Login_Usuario"))
            If System.IO.Directory.Exists(ruta_carpeta_user) = False Then
                System.IO.Directory.CreateDirectory(ruta_carpeta_user)
            End If
            Dim ruta_archivo_confing As String = HttpContext.Current.Server.MapPath("../Docuarchi/configuraciongeneral/Sellos_Temp/")
            Result = Me.Lee_Archivo_Cachexml_Stamp(stru,
                                                   HttpContext.Current.Server.MapPath("../Docuarchi/configuraciongeneral/Sellos_Temp/TempoRegistro.xml"),
                                                   ruta_archivo_confing)
            If Result <> "YES" Then
                stamp_pdf_seguridad = Result
                Exit Function
            End If
            For i2 As Integer = 0 To matri_archivos.Length - 1
                Dim fil As New System.IO.FileInfo(matri_archivos(i2))
                Dim Filname As String = fil.Name
                Filname = Filname.Replace(fil.Extension, "")
                Dim Fileext As String = fil.Extension
                ReDim Preserve Fileout(i)
                ReDim Preserve Fileout_final(i)
                ReDim Preserve Ruta_Original(i)
                Ruta_Original(i) = matri_archivos(i2)
                Fileout(i) = ruta_carpeta_user & "\" & Filname & ".PDF"
                Fileout_final(i) = ruta_carpeta_user & "\" & Filname & "1.PDF"
                If System.IO.File.Exists(Fileout(i)) = True Then
                    Kill(Fileout(i))
                End If
                If System.IO.File.Exists(Fileout_final(i)) = True Then
                    Kill(Fileout_final(i))
                End If
                i = i + 1
            Next
            Dim Class_fyle_system As New Class_fyle_system
            Class_fyle_system.Delete_directory(ruta_carpeta_user)
            'For Each deleteFile In Directory.GetFiles(ruta_carpeta_user, "*.*", SearchOption.TopDirectoryOnly)
            '    File.Delete(deleteFile)
            'Next
            Result = Me.AddWaterPdf_mutiple(Fileout,
                                            Fileout_final,
                                            Ruta_Original,
                                            stru,
                                            optio_marcar,
                                            matri_archivos_selec)
            If Result <> "YES" Then
                stamp_pdf_seguridad = Result
                Exit Function
            End If
            stamp_pdf_seguridad = "YES"
        Catch ex As Exception
            stamp_pdf_seguridad = "Inconsistencia general función stamp_pdf_seguridad " & ex.Message
        End Try
    End Function
    Function Migra_formato_a_pdf(ByVal Matri_documentos() As String,
                                 ByVal archivo_pdf As String,
                                 ByVal Ajusta As String,
                                 ByVal version_pdf As String,
                                 ByRef num_page As Integer) As String
        Dim file As New System.IO.FileStream(archivo_pdf,
                                             System.IO.FileMode.OpenOrCreate,
                                             System.IO.FileAccess.ReadWrite,
                                             System.IO.FileShare.ReadWrite)

        Dim oPdfDoc As New iTextSharp.text.Document()
        Dim oPdfWriter As PdfWriter = PdfWriter.GetInstance(oPdfDoc, file)
        Try
            Select Case version_pdf
                Case "PDF_A_1A"
                    oPdfWriter.PDFXConformance = PdfAConformanceLevel.PDF_A_1A

                Case "PDF_A_1B"
                    oPdfWriter.PDFXConformance = PdfAConformanceLevel.PDF_A_1B
                Case "PDF_A_2A"
                    oPdfWriter.PDFXConformance = PdfAConformanceLevel.PDF_A_2A
                Case "PDF_A_2B"
                    oPdfWriter.PDFXConformance = PdfAConformanceLevel.PDF_A_2B
                Case "PDF_A_2U"
                    oPdfWriter.PDFXConformance = PdfAConformanceLevel.PDF_A_2U
                Case "PDF_A_3A"
                    oPdfWriter.PDFXConformance = PdfAConformanceLevel.PDF_A_3A
                Case "PDF_A_3B"
                    oPdfWriter.PDFXConformance = PdfAConformanceLevel.PDF_A_3B
                Case "PDF_A_3U"
                    oPdfWriter.PDFXConformance = PdfAConformanceLevel.PDF_A_3U
            End Select
            If version_pdf = "PDF_A_1A" Then
                oPdfWriter.SetTagged()
            End If

            oPdfDoc.Open()

            'xmpMeta.AppendArrayItem(XmpConst.NS_PDFA_EXTENSION, "par", "4")
            'oPdfWriter.XmpWriter.XmpMeta = xmpMeta
            'Dim p = setXmpMetada(xmpMeta)
            'oPdfWriter.XmpWriter.XmpMeta.AppendArrayItem(XmpConst.NS_PDFA_EXTENSION, "par", "4")
            'oPdfWriter.XmpWriter.XmpMeta.AppendArrayItem("pdfaid", "rev", "2020")
            'oPdfWriter.XmpWriter.XmpMeta.AppendArrayItem("pdfaid", "amd", "")
            'oPdfWriter.XmpWriter.XmpMeta.AppendArrayItem("pdfaid", "cor", "")
            'oPdfWriter.XmpWriter.XmpMeta.AppendArrayItem("pdfaid", "conformance", "")

            'oPdfWriter.CreateXmpMetadata()
            Dim ruta As String = HttpContext.Current.Server.MapPath("../itextshare/sRGB_CS_profile.icm")
            Dim ICC As ICC_Profile = ICC_Profile.GetInstance(ruta)
            oPdfWriter.SetOutputIntents("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", ICC)
            oPdfWriter.ExtraCatalog.Put(New PdfName("MarkInfo"), New PdfBoolean(True))
            'writer.ExtraCatalog.Put(New PdfName("AF"), Array)
            'writer.ExtraCatalog.Put(New PdfName("MarkInfo"), New PdfBoolean(True))
            For k As Integer = 0 To Matri_documentos.Length - 1
                Dim oDirectContent As iTextSharp.text.pdf.PdfContentByte = oPdfWriter.DirectContent
                Dim oImage As iTextSharp.text.Image = iTextSharp.text.Image.GetInstance(Matri_documentos(k))
                Dim iWidth As Single = oImage.Width
                Dim iHeight As Single = oImage.Height
                If Ajusta = "YES" Then
                    oImage.SetAbsolutePosition(1, 1)
                    oPdfDoc.SetPageSize(New iTextSharp.text.Rectangle(iWidth, iHeight))
                    oPdfDoc.NewPage()
                    oDirectContent.AddImage(oImage)
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
                    oPdfDoc.SetPageSize(iTextSharp.text.PageSize.LETTER)
                    oPdfDoc.NewPage()
                    oImage.ScaleAbsolute(iWidthGoal, iHeightGoal)
                    oDirectContent.AddImage(oImage)
                End If
            Next
            'Dim xmpMeta As IXmpMeta = XmpMetaFactory.Create()
            'oPdfWriter.XmpWriter.XmpMeta.AppendArrayItem("pdfaid", "par", "4")
            num_page = Matri_documentos.Length
            Migra_formato_a_pdf = "YES"
        Catch ex As Exception
            Migra_formato_a_pdf = "Inconsistencia general fucion Migra_formato_a_pdf " & ex.ToString
        Finally
            If Not oPdfDoc Is Nothing Then
                oPdfDoc.Close()
            End If
            If Not oPdfWriter Is Nothing Then
                oPdfWriter.Close()
            End If
        End Try
    End Function
    Function Convertir_tif_pdf_gabinete(ByVal Matri_documentos() As String,
                                        ByVal archivo_pdf As String,
                                        ByVal Ajusta As String) As String


        'Dim oPdfDoc As iTextSharp.text.Document = New iTextSharp.text.Document(iTextSharp.text.PageSize.LETTER, 0, 0, 0, 0)
        Dim file As New System.IO.FileStream(archivo_pdf,
                                             System.IO.FileMode.OpenOrCreate,
                                             System.IO.FileAccess.ReadWrite,
                                             System.IO.FileShare.ReadWrite)
        'Dim oPdfWriter As PdfWriter = PdfWriter.GetInstance(oPdfDoc, file)
        Dim oPdfDoc As New iTextSharp.text.Document()
        'Dim oPdfDoc As New iTextSharp.pdfa.iTextS
        Dim oPdfWriter As PdfWriter = PdfWriter.GetInstance(oPdfDoc, file)
        Try
            'oPdfWriter.PDFXConformance = PdfAConformanceLevel.PDF_A_3A
            oPdfDoc.Open()
            For k As Integer = 0 To Matri_documentos.Length - 1
                Dim oDirectContent As iTextSharp.text.pdf.PdfContentByte = oPdfWriter.DirectContent
                Dim oImage As iTextSharp.text.Image = iTextSharp.text.Image.GetInstance(Matri_documentos(k))
                Dim iWidth As Single = oImage.Width
                Dim iHeight As Single = oImage.Height
                If Ajusta = "YES" Then
                    oImage.SetAbsolutePosition(1, 1)
                    oPdfDoc.SetPageSize(New iTextSharp.text.Rectangle(iWidth, iHeight))
                    oPdfDoc.NewPage()
                    oDirectContent.AddImage(oImage)
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
                    oPdfDoc.SetPageSize(iTextSharp.text.PageSize.LETTER)

                    oPdfDoc.NewPage()
                    oImage.ScaleAbsolute(iWidthGoal, iHeightGoal)
                    oDirectContent.AddImage(oImage)
                End If
            Next
            'oPdfDoc.Close()
            'oPdfWriter.Close()
            'Dim icc As ICC_Profile = ICC_Profile.GetInstance("./sRGB Color Space Profile.icm")
            'oPdfWriter.SetOutputIntents("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", icc)
            'Dim fonts = FontFactory.GetFont("./FreeSansBold.ttf", BaseFont.WINANSI, BaseFont.EMBEDDED, 10)
            'Dim normal9 As Font = FontFactory.GetFont("./FreeSans.ttf", BaseFont.WINANSI, BaseFont.EMBEDDED, 9)
            'Dim bold9 As Font = FontFactory.GetFont("./FreeSansBold.ttf", BaseFont.WINANSI, BaseFont.EMBEDDED, 9)
            'Dim normal8 As Font = FontFactory.GetFont("./FreeSans.ttf", BaseFont.WINANSI, BaseFont.EMBEDDED, 8)
            'oPdfWriter.CreateXmpMetadata()
            Convertir_tif_pdf_gabinete = "YES"
        Catch ex As Exception
            Convertir_tif_pdf_gabinete = "Inconsistencia general funcion Convertir_tif_pdf_gabinete " & ex.Message
        Finally
            If Not oPdfDoc Is Nothing Then
                oPdfDoc.Close()
            End If
            If Not oPdfWriter Is Nothing Then
                oPdfWriter.Close()
            End If
        End Try
    End Function
    Function UnirArchivoPdf(ByVal ruta_archivo_pdf_fuente As String,
                            ByVal ruta_archivo_pdf_agregar As String,
                            ByVal ruta_temporal As String,
                            ByRef numero_paginas_total As Integer) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Funcion que une archivos pdf
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'ruta_archivo_pdf_fuente    : Representa la ruta del archivo fuente de PDF
        'ruta_archivo_pdf_agregar   : Representa la ruta del archivo a unir de PDF
        'ruta_temporal              : Representa la ruta temporal de unión del archivo
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'numero_paginas_total  : Retorna numero de paginas
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha  Modificicacion : 2025-05-14
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        If File.Exists(ruta_archivo_pdf_agregar) = False Then
            UnirArchivoPdf = "Imposible encontrar el archivo (" & ruta_archivo_pdf_agregar & ") para adjuntar"
            Exit Function
        End If
        Dim page As PdfImportedPage = Nothing
        Dim reader_agregar As iTextSharp.text.pdf.PdfReader = Nothing
        Dim reader_fuente As iTextSharp.text.pdf.PdfReader = Nothing
        Dim oPdfDoc_destino As PdfCopy = Nothing
        Dim document = New Document()
        Try
            ruta_temporal = ruta_temporal & "\pdf_tempo_adjunta.pdf"
            If File.Exists(ruta_temporal) = True Then
                Kill(ruta_temporal)
            End If
            reader_agregar = New iTextSharp.text.pdf.PdfReader(ruta_archivo_pdf_agregar)
            reader_fuente = New iTextSharp.text.pdf.PdfReader(ruta_archivo_pdf_fuente)
            oPdfDoc_destino = New PdfCopy(document, New FileStream(ruta_temporal, FileMode.Create))
            document.Open()
            Dim nuM_pag As Integer = reader_fuente.NumberOfPages
            For i As Integer = 1 To nuM_pag
                page = oPdfDoc_destino.GetImportedPage(reader_fuente, i)
                oPdfDoc_destino.AddPage(page)
            Next
            nuM_pag = reader_agregar.NumberOfPages
            For i As Integer = 1 To nuM_pag
                page = oPdfDoc_destino.GetImportedPage(reader_agregar, i)
                oPdfDoc_destino.AddPage(page)
            Next

            numero_paginas_total = oPdfDoc_destino.PageNumber
            If Not oPdfDoc_destino Is Nothing Then
                oPdfDoc_destino.Close()
            End If
            If Not reader_agregar Is Nothing Then
                reader_agregar.Close()
            End If
            If Not reader_agregar Is Nothing Then
                reader_agregar.Close()
            End If
            If Not reader_fuente Is Nothing Then
                reader_fuente.Close()
            End If
            If Not document Is Nothing Then
                document.Close()
            End If
            File.Copy(ruta_temporal, ruta_archivo_pdf_fuente, True)
            Kill(ruta_temporal)
            UnirArchivoPdf = "YES"
        Catch ex As Exception
            UnirArchivoPdf = "Iconsistencia general función UnirArchivoPdf " & ex.Message
        Finally
            If Not oPdfDoc_destino Is Nothing Then
                oPdfDoc_destino.Close()
            End If
            If Not reader_agregar Is Nothing Then
                reader_agregar.Close()
            End If
            If Not reader_agregar Is Nothing Then
                reader_agregar.Close()
            End If
            If Not reader_fuente Is Nothing Then
                reader_fuente.Close()
            End If
            If Not document Is Nothing Then
                document.Close()
            End If
        End Try
    End Function

    Function Guardar_documento_solicitud_pqr(ByVal archivo_salida_formato As String, ByVal nombre_solicitante As String,
        ByVal identificacion_solicitante As String, ByVal direccion As String, ByVal correo_electronico As String,
        ByVal telfono_solicitante As String, ByVal tipo_solicitud As String, ByVal fecha_recibo As String,
        ByVal texto_peticionario As String, ByVal archivo_pdf As String) As String
        Dim doc As iTextSharp.text.Document = Nothing
        Dim writer As iTextSharp.text.pdf.PdfWriter = Nothing
        Guardar_documento_solicitud_pqr = "YES"
        Try
            doc = New iTextSharp.text.Document(iTextSharp.text.PageSize.LETTER)
            doc.SetPageSize(iTextSharp.text.PageSize.LETTER.Rotate())
            writer = iTextSharp.text.pdf.PdfWriter.GetInstance(doc,
                              New FileStream(archivo_pdf, FileMode.Create))
            writer.AddViewerPreference(iTextSharp.text.pdf.PdfName.PICKTRAYBYPDFSIZE, iTextSharp.text.pdf.PdfBoolean.PDFTRUE)
            'writer.PDFXConformance = PdfAConformanceLevel.PDF_A_3A
            doc.Open()
            Dim _standardFont As iTextSharp.text.Font = New iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA,
                   12, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK)
            '----------------------------------------
            'IDENTIFICACION PETICIONARIO
            '----------------------------------------
            Dim paragraf As New iTextSharp.text.Paragraph
            Dim tblrdatos As iTextSharp.text.pdf.PdfPTable = New iTextSharp.text.pdf.PdfPTable(2)
            tblrdatos.WidthPercentage = 100
            Dim cltitem As iTextSharp.text.pdf.PdfPCell = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase("Identificación", _standardFont))
            cltitem.BorderWidth = 1
            Dim cltdetalle As iTextSharp.text.pdf.PdfPCell = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase(identificacion_solicitante, _standardFont))
            cltdetalle.BorderWidth = 1
            tblrdatos.AddCell(cltitem)
            tblrdatos.AddCell(cltdetalle)
            '----------------------------------------
            'NOMBRE PETICIONARIO
            '----------------------------------------
            Dim cltitem_peticionario As iTextSharp.text.pdf.PdfPCell = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase("Nombre peticionario", _standardFont))
            tblrdatos.AddCell(cltitem_peticionario)
            Dim cltdetalle_peticionario As iTextSharp.text.pdf.PdfPCell = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase(nombre_solicitante, _standardFont))
            tblrdatos.AddCell(cltdetalle_peticionario)
            '-----------------------------------------
            'DIRECCION DEL PETICIONARIO
            '-----------------------------------------
            Dim cltitem_direccion As iTextSharp.text.pdf.PdfPCell = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase("Dirección", _standardFont))
            tblrdatos.AddCell(cltitem_peticionario)
            Dim cltdetalle_direccion As iTextSharp.text.pdf.PdfPCell = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase(direccion, _standardFont))
            tblrdatos.AddCell(cltdetalle_direccion)

            '-----------------------------------------
            'CORREO ELECTRONICO
            '-----------------------------------------
            Dim cltitem_electronico As iTextSharp.text.pdf.PdfPCell = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase("Correo electrónico", _standardFont))
            tblrdatos.AddCell(cltitem_peticionario)
            Dim cltdetalle_electronico As iTextSharp.text.pdf.PdfPCell = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase(correo_electronico, _standardFont))
            tblrdatos.AddCell(cltdetalle_electronico)

            '-----------------------------------------
            'TELEFONO 
            '-----------------------------------------
            Dim cltitem_telefono As iTextSharp.text.pdf.PdfPCell = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase("Teléfono", _standardFont))
            tblrdatos.AddCell(cltitem_peticionario)
            Dim cltdetalle_telefono As iTextSharp.text.pdf.PdfPCell = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase(telfono_solicitante, _standardFont))
            tblrdatos.AddCell(cltdetalle_telefono)

            '-----------------------------------------
            'Texto de la petición
            '-----------------------------------------
            Dim cltitem_texto As iTextSharp.text.pdf.PdfPCell = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase("Texto", _standardFont))
            cltitem_texto.Colspan = 2
            tblrdatos.AddCell(cltitem_texto)
            Dim cltdetalle_texto As iTextSharp.text.pdf.PdfPCell = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase(texto_peticionario, _standardFont))
            cltdetalle_texto.Colspan = 2
            tblrdatos.AddCell(cltdetalle_texto)
            doc.Add(tblrdatos)
            doc.Close()
            writer.Close()
        Catch ex As Exception
            doc.Close()
            writer.Close()
            Guardar_documento_solicitud_pqr = "Inconsistencia general función Guardar_documento_solicitud_pqr " & ex.Message
        End Try
    End Function
    Function Genera_documento_solicitud_pqr_matri_dato_detalle(ByVal archivo_salida_formato As String,
                                                                ByVal nombre_solicitante As String,
                                                                ByVal identificacion_solicitante As String,
                                                                ByVal texto_peticionario As String,
                                                                ByVal Tramite As String,
                                                                ByVal asunto As String,
                                                                ByVal radicado As String,
                                                                ByVal fecha_radicado As String,
                                                                ByVal matri_datos() As String) As String
        Dim doc As iTextSharp.text.Document = Nothing
        Dim writer As iTextSharp.text.pdf.PdfWriter = Nothing

        Try
            doc = New iTextSharp.text.Document(iTextSharp.text.PageSize.LETTER)
            doc.SetPageSize(iTextSharp.text.PageSize.LETTER.Rotate())
            writer = iTextSharp.text.pdf.PdfWriter.GetInstance(doc,
                              New FileStream(archivo_salida_formato, FileMode.Create))
            writer.AddViewerPreference(iTextSharp.text.pdf.PdfName.PICKTRAYBYPDFSIZE, iTextSharp.text.pdf.PdfBoolean.PDFTRUE)
            doc.Open()
            Dim _standardFont As iTextSharp.text.Font = New iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA,
                   12, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK)
            Dim _standardFont_ttle As iTextSharp.text.Font = New iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA,
                  10, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK)
            Dim paragraf As New iTextSharp.text.Paragraph
            Dim tblrdatos As iTextSharp.text.pdf.PdfPTable = New iTextSharp.text.pdf.PdfPTable(2)
            tblrdatos.WidthPercentage = 100
            '----------------------------------------
            'RADICADO DE LA PETICION
            '----------------------------------------
            Dim cltitem_radicado As iTextSharp.text.pdf.PdfPCell = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase("RADICADO", _standardFont_ttle))
            tblrdatos.AddCell(cltitem_radicado)
            Dim cltdetalle_radicado As iTextSharp.text.pdf.PdfPCell = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase(radicado, _standardFont_ttle))
            tblrdatos.AddCell(cltdetalle_radicado)
            '----------------------------------------
            'IDENTIFICACION PETICIONARIO
            '----------------------------------------
            Dim cltitem_fecha As iTextSharp.text.pdf.PdfPCell = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase("FECHA RADICADO", _standardFont_ttle))
            tblrdatos.AddCell(cltitem_fecha)
            Dim cltdetalle_fecha As iTextSharp.text.pdf.PdfPCell = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase(fecha_radicado, _standardFont_ttle))
            tblrdatos.AddCell(cltdetalle_fecha)

            Dim cltitem As iTextSharp.text.pdf.PdfPCell = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase(UCase("Identificación"), _standardFont_ttle))
            cltitem.BorderWidth = 1
            Dim cltdetalle As iTextSharp.text.pdf.PdfPCell = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase(identificacion_solicitante, _standardFont_ttle))
            cltdetalle.BorderWidth = 1
            tblrdatos.AddCell(cltitem)
            tblrdatos.AddCell(cltdetalle)
            '----------------------------------------
            'NOMBRE PETICIONARIO
            '----------------------------------------
            For i As Integer = 0 To matri_datos.Length - 1
                Dim spli() As String = matri_datos(i).Split("|")
                Dim cltitem_electronico As iTextSharp.text.pdf.PdfPCell = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase(spli(0), _standardFont_ttle))
                tblrdatos.AddCell(cltitem_electronico)
                Dim cltdetalle_electronico As iTextSharp.text.pdf.PdfPCell = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase(spli(1), _standardFont_ttle))
                tblrdatos.AddCell(cltdetalle_electronico)
            Next
            '-----------------------------------------
            'ASUNTO
            '-----------------------------------------
            Dim cltitem_asunto As iTextSharp.text.pdf.PdfPCell = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase(UCase("Asunto"), _standardFont_ttle))
            tblrdatos.AddCell(cltitem_asunto)
            Dim cltdetalle_asunto As iTextSharp.text.pdf.PdfPCell = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase(asunto, _standardFont_ttle))
            tblrdatos.AddCell(cltdetalle_asunto)

            '-----------------------------------------
            'TRAMITE
            '-----------------------------------------
            Dim cltitem_telefono As iTextSharp.text.pdf.PdfPCell = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase(UCase("Tipo solicitud "), _standardFont_ttle))
            tblrdatos.AddCell(cltitem_telefono)
            Dim cltdetalle_telefono As iTextSharp.text.pdf.PdfPCell = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase(Tramite, _standardFont_ttle))
            tblrdatos.AddCell(cltdetalle_telefono)
            '-----------------------------------------
            'Texto de la petición
            '-----------------------------------------
            Dim cltitem_texto As iTextSharp.text.pdf.PdfPCell = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase("CONTENIDO DEL TEXTO DEL PQR ", _standardFont))
            cltitem_texto.Colspan = 2
            cltitem_texto.HorizontalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_CENTER
            tblrdatos.AddCell(cltitem_texto)
            Dim cltdetalle_texto As iTextSharp.text.pdf.PdfPCell = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase(texto_peticionario, _standardFont))
            cltdetalle_texto.Colspan = 2
            tblrdatos.AddCell(cltdetalle_texto)
            doc.Add(tblrdatos)
            doc.Close()
            writer.Close()
            Genera_documento_solicitud_pqr_matri_dato_detalle = "YES"
            Exit Function
        Catch ex As Exception
            If Not doc Is Nothing Then
                doc.Close()
            End If
            If Not writer Is Nothing Then
                writer.Close()
            End If
            Genera_documento_solicitud_pqr_matri_dato_detalle = "Inconsistencia general función Genera_documento_solicitud_pqr_matri_dato_detalle " & ex.Message
        End Try
    End Function
    Function ItexConstanciaIsncripcionSII(ByVal CIncripcionSII As CIncripcionSII,
                                          ByVal RutaArchivo As String,
                                          ByVal RegistroPublico As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita el sello de inscripción SII en formato PDF
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'CIncripcionSII      : Representa la estructura con la constancia de inscripción
        'RutaArchivo         : Representa la ruta del archivo
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        '
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-07-28
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Dim DocumentoItex As iTextSharp.text.Document = Nothing
        Dim writer As iTextSharp.text.pdf.PdfWriter = Nothing
        Try
            Dim Class_Entero_Romano As New Class_Entero_Romano
            Dim ReferecniaLibroRomano As String = Class_Entero_Romano.Numero_A_Romano(Val(CIncripcionSII.LIBRO_SII))
            DocumentoItex = New iTextSharp.text.Document(iTextSharp.text.PageSize.LETTER)
            DocumentoItex.SetPageSize(iTextSharp.text.PageSize.LETTER)
            writer = iTextSharp.text.pdf.PdfWriter.GetInstance(DocumentoItex,
                              New FileStream(RutaArchivo, FileMode.Create))
            writer.AddViewerPreference(iTextSharp.text.pdf.PdfName.PICKTRAYBYPDFSIZE, iTextSharp.text.pdf.PdfBoolean.PDFTRUE)
            writer.PDFXConformance = PdfAConformanceLevel.PDF_A_1A
            DocumentoItex.Open()
            Dim url_log As String = HttpContext.Current.Server.MapPath("../imagera/logo_trd.jpg")
            Dim imgag As iTextSharp.text.Image = iTextSharp.text.Image.GetInstance(url_log)
            imgag.ScaleToFit(140.0F, 120.0F)
            imgag.SpacingBefore = 10.0F
            imgag.SpacingAfter = 1.0F
            imgag.Alignment = Element.ALIGN_LEFT
            DocumentoItex.Add(imgag)
            Dim _standardFont As iTextSharp.text.Font = New iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN,
                   12, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK)
            '----------------------------------------
            'ASINGANCION DEL NOMBRE DE LA CCV
            '----------------------------------------
            Dim paragraf As New iTextSharp.text.Paragraph
            paragraf.Alignment = Element.ALIGN_CENTER
            paragraf.Add(New iTextSharp.text.Phrase(UCase("CAMARA DE COMERCIO DE VILLAVICENCIO"), _standardFont))
            DocumentoItex.Add(paragraf)
            paragraf = New iTextSharp.text.Paragraph
            paragraf.Alignment = Element.ALIGN_CENTER
            paragraf.Add(New iTextSharp.text.Phrase(UCase("892.000.102-1"), _standardFont))
            DocumentoItex.Add(paragraf)
            DocumentoItex.Add(Chunk.NEWLINE)
            DocumentoItex.Add(Chunk.NEWLINE)
            paragraf = New iTextSharp.text.Paragraph
            paragraf.Alignment = Element.ALIGN_CENTER
            paragraf.Add(New iTextSharp.text.Phrase("DEL REGISTRO " & UCase(RegistroPublico), _standardFont))
            DocumentoItex.Add(paragraf)
            DocumentoItex.Add(Chunk.NEWLINE)
            DocumentoItex.Add(Chunk.NEWLINE)
            paragraf = New iTextSharp.text.Paragraph
            paragraf.Alignment = Element.ALIGN_CENTER
            paragraf.Add(New iTextSharp.text.Phrase("Libro " & ReferecniaLibroRomano, _standardFont))
            DocumentoItex.Add(paragraf)
            paragraf = New iTextSharp.text.Paragraph
            paragraf.Alignment = Element.ALIGN_CENTER
            paragraf.Add(New iTextSharp.text.Phrase("Numero registro " & CIncripcionSII.REGISTRO_SII, _standardFont))
            DocumentoItex.Add(paragraf)
            paragraf = New iTextSharp.text.Paragraph
            paragraf.Alignment = Element.ALIGN_CENTER
            paragraf.Add(New iTextSharp.text.Phrase("Fecha " & CIncripcionSII.FECHA_SII, _standardFont))
            DocumentoItex.Add(paragraf)

            paragraf = New iTextSharp.text.Paragraph
            paragraf.Alignment = Element.ALIGN_CENTER
            paragraf.Add(New iTextSharp.text.Phrase("Hora " & CIncripcionSII.HORA_SII, _standardFont))
            DocumentoItex.Add(paragraf)
            DocumentoItex.Add(Chunk.NEWLINE)
            paragraf = New iTextSharp.text.Paragraph
            paragraf.Alignment = Element.ALIGN_CENTER
            paragraf.Add(New iTextSharp.text.Phrase("Expediente " & CIncripcionSII.MATRICULA_SII, _standardFont))
            DocumentoItex.Add(paragraf)
            paragraf = New iTextSharp.text.Paragraph
            paragraf.Alignment = Element.ALIGN_CENTER
            paragraf.Add(New iTextSharp.text.Phrase("Identificación " & CIncripcionSII.NIT_SII, _standardFont))
            DocumentoItex.Add(paragraf)
            paragraf = New iTextSharp.text.Paragraph
            paragraf.Alignment = Element.ALIGN_CENTER
            paragraf.Add(New iTextSharp.text.Phrase("Nombre " & UCase(CIncripcionSII.RSOCIAL_SII), _standardFont))
            DocumentoItex.Add(paragraf)
            DocumentoItex.Add(Chunk.NEWLINE)
            DocumentoItex.Add(Chunk.NEWLINE)
            paragraf = New iTextSharp.text.Paragraph
            paragraf.Alignment = Element.ALIGN_CENTER
            paragraf.Add(New iTextSharp.text.Phrase("Acto " & UCase(CIncripcionSII.ACTO_SII) & " " & UCase(CIncripcionSII.NACTO_SII), _standardFont))
            DocumentoItex.Add(paragraf)
            DocumentoItex.Add(Chunk.NEWLINE)
            paragraf = New iTextSharp.text.Paragraph
            paragraf.Alignment = Element.ALIGN_CENTER
            paragraf.Add(New iTextSharp.text.Phrase("Noticia " & UCase(CIncripcionSII.NOTICIA_SII), _standardFont))
            DocumentoItex.Add(paragraf)
            paragraf = New iTextSharp.text.Paragraph
            paragraf.Alignment = Element.ALIGN_CENTER
            paragraf.Add(New iTextSharp.text.Phrase("Scretario (Delegado) ", _standardFont))
            DocumentoItex.Add(paragraf)
            Dim Ruta_firma As String = HttpContext.Current.Server.MapPath(HttpContext.Current.Session("WF_RUTA_FIRMA_FINAL"))
            If File.Exists(Ruta_firma) = True Then
                Dim imgag_ As iTextSharp.text.Image = iTextSharp.text.Image.GetInstance(Ruta_firma)
                imgag_.ScaleToFit(70.0F, 70.0F)
                imgag_.Alignment = Element.ALIGN_CENTER
                DocumentoItex.Add(imgag_)
            End If
            DocumentoItex.Close()
            writer.Close()
            ItexConstanciaIsncripcionSII = "YES"
        Catch ex As Exception
            DocumentoItex.Close()
            writer.Close()
            ItexConstanciaIsncripcionSII = "Inconsistencia general función ItexConstanciaIsncripcionSII " & ex.Message
        End Try

    End Function
End Class
