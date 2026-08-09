Imports System.IO
Imports System
Imports TiffDLL200
Imports System.Drawing

Public Class Class_TiffDLL200
    Function Sello_camara_tif(ByVal cuerpo_sello As String,
                              ByRef ruta_archivo_sello As String) As String
        Dim obj As New TiffDLL200.TiffDLL()
        Try
            obj.init.Licensecode = "YIDSTAIZ5961110300711262"
            Dim ruta_plantilla As String = HttpContext.Current.Server.MapPath("../Temp_Workflow/Plantilla.tif")
            Dim Ruttempo As String = HttpContext.Current.Server.MapPath(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_WF") + HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString)
            Dim ruta_descarga As String = Ruttempo & "\DONWLOAD\"
            Dim lvFileName As String = "file_sello.tif"
            If Directory.Exists(ruta_descarga) = False Then
                Directory.CreateDirectory(ruta_descarga)
            End If
            If File.Exists(ruta_plantilla) = False Then
                Sello_camara_tif = "Imposible encontrar el archivo plantilla  " & ruta_plantilla
                Exit Function
            End If
            If File.Exists(ruta_descarga & "Plantilla.tif") Then
                Kill(ruta_descarga & "Plantilla.tif")
            End If
            File.Copy(HttpContext.Current.Server.MapPath("../Temp_Workflow/Plantilla.tif"), ruta_descarga & "Plantilla.tif")
            ruta_plantilla = ruta_descarga & "Plantilla.tif"
            Dim Ruta_firma As String = HttpContext.Current.Server.MapPath(HttpContext.Current.Session("WF_RUTA_FIRMA_FINAL"))
            If File.Exists(Ruta_firma) = False Then
                Sello_camara_tif = "Imposible encontrar el archivo de firma para el documento  "
                Exit Function
            End If
            obj.op.MultilineAnnotation.Font1 = New Font("Arial", 10, FontStyle.Regular)
            obj.op.MultilineAnnotation.Font2 = New Font("Arial", 12, FontStyle.Regular)
            obj.op.MultilineAnnotation.Font3 = New Font("Arial", 14, FontStyle.Regular)
            obj.init.File_in = ruta_plantilla
            Dim split_cuerpo_sello() As String = cuerpo_sello.ToString.Split("¬")
            For i As Integer = 0 To split_cuerpo_sello.Length - 1
                obj.Specs(i) = split_cuerpo_sello(i)
            Next
            If InStr(cuerpo_sello, "¬") <= 0 Then
                Sello_camara_tif = "Sello sin información " & cuerpo_sello.ToString
                Exit Function
            End If
            obj.op.Image.File = Ruta_firma
            obj.op.Image.PointF.X = 2.5
            obj.op.Image.PointF.Y = 6.5
            obj.op.Image.SizeF.Width = 1.5F
            obj.op.Image.SizeF.Height = 1.5F
            obj.op.Image.MakeWhiteTransparent = True
            Dim err As Integer = 0
            obj.init.File_out = (ruta_descarga & "NEW" & lvFileName)
            obj.init.OverwriteFile = True
            obj.init.Format.TiffCompression = TiffCompression.CCITT4_Group4_mono
            err = obj.run
            If err <> 0 Then
                Sello_camara_tif = err.ToString + " " + obj.info.lasterror
                Exit Function
            End If
            IO.File.Delete(ruta_descarga & lvFileName)
            lvFileName = "NEW" & lvFileName
            GC.Collect()
            ruta_archivo_sello = ruta_descarga & lvFileName
            Sello_camara_tif = "YES"
        Catch ex As Exception
            Sello_camara_tif = "Inconsistencia general función Sello_camara_tif " & ex.Message
        Finally
            obj = Nothing
        End Try
    End Function
End Class
