Public Class ClassRaImpresion
    Function Imprimir_da_documentos_visor(ByRef pag As Page) As String
        Try
            Dim Result As String = ""
            Dim refclasgabinete As New ClassDaGabinete
            Dim sid2 As HtmlInputHidden = pag.FindControl("sid2")
            Dim Hidden_respuesta As HtmlInputHidden = pag.FindControl("Hidden_respuesta")
            Dim RadioButton_rango As RadioButton = pag.FindControl("RadioButton_rango")
            Dim RadioButton_seleccion As RadioButton = pag.FindControl("RadioButton_seleccion")
            Dim RadioButton_todo As RadioButton = pag.FindControl("RadioButton_todo")
            Dim hideextension As HtmlInputHidden = pag.FindControl("hideextension")
            Dim TextBox_ini As TextBox = pag.FindControl("TextBox_ini")
            Dim CheckBox_pdf_convert As CheckBox = pag.FindControl("CheckBox_pdf_convert")
            Dim CheckBox_marca_agua As CheckBox = pag.FindControl("CheckBox_marca_agua")
            sid2.Value = ""
            Hidden_respuesta.Value = "NO"
            If HttpContext.Current.Session.Item("RA_RUTA_IMPRESION_FINAL") = "OJO" Or HttpContext.Current.Session.Item("RA_RUTA_IMPRESION_FINAL") = "" Then
                Imprimir_da_documentos_visor = "Imposible imprimir matriz vacia, es posible que el reguest este bloqueado"
                Exit Function
            End If
            Dim spliter() As String = HttpContext.Current.Session.Item("RA_RUTA_IMPRESION_FINAL").ToString.Split(",")
            Dim archivo_extenxion As String = ""
            If Not spliter Is Nothing Then
                archivo_extenxion = spliter(0)
            Else
                archivo_extenxion = HttpContext.Current.Session.Item("RA_RUTA_IMPRESION_FINAL").ToString
            End If
            Dim fileinf As New IO.FileInfo(archivo_extenxion)
            hideextension.Value = UCase(fileinf.Extension)
            If RadioButton_seleccion.Checked = True Then
                If HttpContext.Current.Session.Item("RA_RUTA_IMPRESION_FINAL_DOC_ACTUAL") = "" Then
                    Imprimir_da_documentos_visor = "Imposible imprimir documento actual vacio, es posible que el reguest este bloqueado"
                    Exit Function
                End If
                sid2.Value = HttpContext.Current.Session.Item("RA_RUTA_IMPRESION_FINAL_DOC_ACTUAL")
                Hidden_respuesta.Value = "YES"
            End If
            If RadioButton_todo.Checked = True Then
                sid2.Value = HttpContext.Current.Session.Item("RA_RUTA_IMPRESION_FINAL")
                Hidden_respuesta.Value = "YES"
            End If
            If RadioButton_rango.Checked = True Then
                Dim splits() As String = HttpContext.Current.Session.Item("RA_RUTA_IMPRESION_FINAL").ToString.Split(",")
                Dim matri_selec() As String
                Erase matri_selec
                If Not splits Is Nothing Then
                    If splits.Length > 1 Then
                        Result = refclasgabinete.Generando_Matriz_Imagenes_Sleccionadas(splits, TextBox_ini.Text, matri_selec)
                        If Result <> "YES" Then
                            Imprimir_da_documentos_visor = Result
                            Hidden_respuesta.Value = "NO"
                            Exit Function
                        Else
                            If Not matri_selec Is Nothing Then
                                sid2.Value = ""
                                For i As Integer = 0 To matri_selec.Length - 1
                                    If sid2.Value = "" Then
                                        sid2.Value = matri_selec(i)
                                    Else
                                        sid2.Value = sid2.Value & "," & matri_selec(i)
                                    End If
                                Next
                                Hidden_respuesta.Value = "YES"
                            Else
                                Imprimir_da_documentos_visor = "La selección no cumple los criterios, imposible imprimir"
                                Hidden_respuesta.Value = "NO"
                                Exit Function
                            End If
                        End If
                    Else
                        sid2.Value = HttpContext.Current.Session.Item("RA_RUTA_IMPRESION_FINAL")
                        Hidden_respuesta.Value = "YES"
                    End If

                End If
            End If
            '--------------------------------------------
            'Convierte pdf documento tif
            '--------------------------------------------
            If CheckBox_pdf_convert.Checked = True Then
                If UCase(hideextension.Value) = ".TIF" Or UCase(hideextension.Value) = ".BMP" Or UCase(hideextension.Value) = ".JPG" Then
                    Dim option_marcar As Integer = 0
                    Dim Refclas_report As New Class_ItexShare
                    Dim matri_archivo() As String
                    Dim file_out() As String
                    Erase file_out
                    If CheckBox_marca_agua.Checked = True Then
                        option_marcar = 1

                    End If
                    ReDim Preserve matri_archivo(0)
                    Dim matri_envio() As String = sid2.Value.ToString.Split(",")
                    matri_archivo(0) = spliter(0)
                    Result = Refclas_report.stamp_pdf_seguridad(matri_archivo, file_out, option_marcar, matri_envio)
                    If Result <> "YES" Then
                        Imprimir_da_documentos_visor = Result.Replace("/", "-")
                        Exit Function
                    End If
                    For i3 As Integer = 0 To file_out.Length - 1
                        If i3 = 0 Then
                            sid2.Value = file_out(i3)
                        Else
                            sid2.Value = sid2.Value & "," & file_out(i3)
                        End If
                    Next
                    hideextension.Value = ".PDF"
                End If
            End If
            Dim datos_log As String = ""
            Result = refclasgabinete.Retorna_Datos_Auditoria_Gabinete(HttpContext.Current.Session.Item("DA_ID_IMAGEN_IMPRESION"), HttpContext.Current.Session.Item("DA_GABINETE_IMPRESION"), datos_log)
            If Result <> "YES" Then
                Imprimir_da_documentos_visor = "Imposible encontrar datos log " & Result
                Hidden_respuesta.Value = "NO"
                Exit Function
            End If
            Dim selecion As String = ""
            If RadioButton_todo.Checked = True Then
                selecion = "Todas las páginas (" & spliter.Length.ToString & ")"
            End If
            If RadioButton_seleccion.Checked = True Then
                selecion = "Pagina actual (1)" & sid2.Value
            End If
            If RadioButton_rango.Checked = True Then
                selecion = "Rango de selección " & TextBox_ini.Text
            End If
            Result = refclasgabinete.Registra_Auditoria_Eventos(HttpContext.Current.Session.Item("DA_GABINETE_IMPRESION"), selecion & " Imagen Principal " & spliter(0), HttpContext.Current.Session.Item("DA_ID_IMAGEN_IMPRESION"), datos_log, "Imprimir")
            If Result <> "YES" Then
                Imprimir_da_documentos_visor = "Imposible registrar datos log " & Result
                Hidden_respuesta.Value = "NO"
                Exit Function
            End If
            sid2.Value = sid2.Value & "|" & UCase(hideextension.Value)
            Imprimir_da_documentos_visor = "YES"
        Catch ex As Exception
            Imprimir_da_documentos_visor = "Inconsistencia función Imprimir_da_documentos_visor " & ex.Message
        End Try
    End Function
End Class
