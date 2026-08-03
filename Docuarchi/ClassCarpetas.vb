Public Class ClassCarpetas
    Function Exportar_Documentos_Gabinete(ByVal Docu_Copia As String, _
                                          ByVal Tipo_Exportacion As String, _
                                          ByVal archivo_pdf As String, _
                                          ByVal Resiz As String, _
                                          ByVal matri_documentos_selec() As String) As String
        Try
            If Tipo_Exportacion = "PDF" Then
                Dim Refclas As New Class_ItexShare
                Dim Result As String = ""
                Result = Refclas.Convertir_tif_pdf_gabinete(matri_documentos_selec, _
                                                            archivo_pdf, _
                                                            Resiz)
                If Result <> "YES" Then
                    Exportar_Documentos_Gabinete = Result
                    Exit Function
                End If
            End If
            Exportar_Documentos_Gabinete = "YES"
        Catch ex As Exception
            Exportar_Documentos_Gabinete = "Inconsistencia general función Exportar_Documentos_Gabinete " & ex.Message
        End Try

    End Function
    Function Exportar_Documentos_Carpeta(ByVal Docu_Copia As String,
                                         ByVal Tipo_Exportacion As String,
                                         ByVal Ruta_Exportacion As String,
                                         ByVal Resiz As String) As String
        Try
            Dim Result As String = ""
            'Dim RwfClasEportGab As New ClassExportaGabinete
            '******************************************
            'Genera el cuerpo del documentos almacenado
            '******************************************
            Dim Archivo As New System.IO.FileInfo(Docu_Copia)
            '**************************************
            'Extrae Matriz Docuarchi ".000001"
            '**************************************
            Dim Nombre_Archivo As String
            Dim Matri_Docuarchi_Doc() As String
            Erase Matri_Docuarchi_Doc
            Dim Matri_Incre As Integer = 0
            Nombre_Archivo = Archivo.Name.Replace(Archivo.Extension, "")
            '**//Determina si el archivo esta en formato tif
            If UCase(Archivo.Extension) = UCase(".tif") Or
            UCase(Archivo.Extension) = UCase(".jpg") Or
            UCase(Archivo.Extension) = UCase(".bmp") Or
                UCase(Archivo.Extension) = UCase(".tiff") Then
                ReDim Preserve Matri_Docuarchi_Doc(Matri_Incre)
                Matri_Docuarchi_Doc(Matri_Incre) = Docu_Copia
                Matri_Incre = Matri_Incre + 1
                For Each Archivoif As Object In My.Computer.FileSystem.GetFiles(
                                       Archivo.DirectoryName,
                                        FileIO.SearchOption.SearchTopLevelOnly, Nombre_Archivo & ".*")
                    If Docu_Copia <> Archivoif Then
                        ReDim Preserve Matri_Docuarchi_Doc(Matri_Incre)
                        Matri_Docuarchi_Doc(Matri_Incre) = Archivoif
                        Matri_Incre = Matri_Incre + 1
                    End If
                Next
                '***************************************
                'Extrae tif multi pagina por determinar
                'el componente de extracion
                '***************************************
                'Dim ResultmAT As String = Ordena_Matriz_Doc(Matri_Docuarchi_Doc)
                'If ResultmAT <> "YES" Then
                'Exportar_Documentos_Carpeta = "Error ordenando matriz documentos " & ResultmAT
                'Exit Function
                'End If
            Else
                ReDim Preserve Matri_Docuarchi_Doc(0)
                Matri_Docuarchi_Doc(0) = Docu_Copia
            End If
            '****************************************
            'Copia las imagenes en formato docuarchi
            '****************************************
            'Dim Nombre_Documentos As String = Archivo.Name
            'If Tipo_Exportacion = "TIFDOCUARCHI" Then
            '    Result = ""
            '    Result = RwfClasEportGab.Export_Formato_Docuarchi(Nombre_Documentos, Matri_Docuarchi_Doc, _
            '    Ruta_Exportacion)
            '    If Result <> "YES" Then
            '        Exportar_Documentos_Carpeta = Result
            '        Exit Function
            '    End If
            'End If
            '***************************************
            'Copia las imagenes en formato DW
            '***************************************
            Dim Id_Imagen As String = Archivo.Name.Replace(Archivo.Extension, "")
            Id_Imagen = Val(Id_Imagen.Replace("DIG", ""))
            'If Tipo_Exportacion = "TIFDW" Then
            '    If UCase(Archivo.Extension) = ".JPG" Or UCase(Archivo.Extension) = ".TIF" Or _
            '    UCase(Archivo.Extension) = ".BMP" Then
            '        Result = RwfClasEportGab.Export_Formato_DW(Id_Imagen, Matri_Docuarchi_Doc, Ruta_Exportacion)
            '        If Result <> "YES" Then
            '            Exportar_Documentos_Carpeta = Result
            '            Exit Function
            '        End If
            '    Else
            '        Result = ""
            '        Result = RwfClasEportGab.Export_Formato_Docuarchi(Nombre_Documentos, Matri_Docuarchi_Doc, _
            '        Ruta_Exportacion)
            '        If Result <> "YES" Then
            '            Exportar_Documentos_Carpeta = Result
            '            Exit Function
            '        End If
            '    End If
            'End If
            '**************************************
            'Copia las imagenes en formato tif
            'universal
            '**************************************
            'If Tipo_Exportacion = "TIFUNIVERSAL" And UCase(Archivo.Extension) = ".TIF" Then
            '    'Exportar_Documentos_Carpeta = "Funcion en Desarrollo"
            '    'Exit Function
            '    Dim Refclas As New ClassExportaGabinete
            '    If UCase(Archivo.Extension) = ".TIF" Then
            '        Result = ""
            '        Result = Refclas.Crea_Documento_MultiTif(Matri_Docuarchi_Doc, Ruta_Exportacion)
            '        If Result <> "YES" Then
            '            Exportar_Documentos_Carpeta = Result
            '            Exit Function
            '        End If
            '    Else
            '        Result = ""
            '        Result = RwfClasEportGab.Export_Formato_Docuarchi(Nombre_Documentos, Matri_Docuarchi_Doc, _
            '        Ruta_Exportacion)
            '        If Result <> "YES" Then
            '            Exportar_Documentos_Carpeta = Result
            '            Exit Function
            '        End If
            '    End If

            'End If
            '**************************************
            'Copia las imagenes en formato PDF
            '**************************************
            If Tipo_Exportacion = "PDF" Then
                Dim Refclas As New Class_ItexShare
                If UCase(Archivo.Extension) = ".JPG" Or UCase(Archivo.Extension) = ".TIF" Or
                    UCase(Archivo.Extension) = ".BMP" Then
                    Result = ""
                    Result = Refclas.Convertir_tif_pdf_gabinete(Matri_Docuarchi_Doc, Ruta_Exportacion, Resiz)
                    If Result <> "YES" Then
                        Exportar_Documentos_Carpeta = Result
                        Exit Function
                    End If
                Else
                    'Result = ""
                    'Result = RwfClasEportGab.Export_Formato_Docuarchi(Nombre_Documentos, Matri_Docuarchi_Doc, _
                    'Ruta_Exportacion)
                    'If Result <> "YES" Then
                    '    Exportar_Documentos_Carpeta = Result
                    '    Exit Function
                    'End If

                End If

            End If
            Exportar_Documentos_Carpeta = "YES"
        Catch ex As Exception
            Exportar_Documentos_Carpeta = "Error general funcion Exportar_Documentos_Carpeta Error " & ex.ToString
        End Try
    End Function
End Class
