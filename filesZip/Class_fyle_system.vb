Imports System.IO
Imports Ionic.Zip
Imports System.Collections.Generic
Public Class file_system
    Public Property name_file_tipo_logia As String
    Public Property name_file As String
    Public Property source_file
End Class
Public Structure stru_file_system
    Dim name_file_tipo_logia As String
    Dim name_file As String
    Dim source_file
    Dim stru_files_da() As stru_file_system_da
End Structure
Public Structure stru_file_system_da
    Dim name_file As String
    Dim source_file
End Structure
Public Class Class_fyle_system
    Function Add_zip_version_documento(ByVal id_registro_version As Long,
                                       ByVal Matriz_documentos() As String,
                                       ByVal ruta_temporal As String,
                                       ByVal ruta_tem_fuera As String,
                                       ByRef Ruta_file_zip_version As String) As String
        '---------------------------------------------------------------------------
        'Funcion : Retorna zip con documentos de descarga de version de documentos
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_registro_version   : Representa la identificación del registro del docu
        '                        mneto
        'Matriz_documentos     : Representa la matriz de documentos
        'ruta_temporal         : Ruta temporal donde se general el zip y se copian los
        '                        archivos
        'ruta_tem_fuera        : Representa la ruta donde se genera el zip
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'Ruta_file_zip_version: Retorna la ruta del documento ZIP
        '
        '
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-07-13
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            If Directory.Exists(ruta_temporal) = False Then
                Directory.CreateDirectory(ruta_temporal)
            End If
            For Each deleteFile In Directory.GetFiles(ruta_temporal, "*.*", SearchOption.TopDirectoryOnly)
                File.Delete(deleteFile)
            Next
            Dim matri_documento_copia() As String = Nothing
            For i As Integer = 0 To Matriz_documentos.Length - 1
                ReDim Preserve matri_documento_copia(i)
                Dim file_inf As New FileInfo(Matriz_documentos(i))
                matri_documento_copia(i) = ruta_temporal & file_inf.Name
                File.Copy(Matriz_documentos(i), matri_documento_copia(i))
            Next
            Using zip As New ZipFile()
                zip.AddDirectory(ruta_temporal)
                Dim zipName As String = "file_version_" & id_registro_version & ".zip"
                Ruta_file_zip_version = ruta_tem_fuera & zipName
                If File.Exists(Ruta_file_zip_version) = True Then
                    File.Delete(Ruta_file_zip_version)
                End If
                zip.Save(Ruta_file_zip_version)
                For Each deleteFile In Directory.GetFiles(ruta_temporal, "*.*", SearchOption.TopDirectoryOnly)
                    File.Delete(deleteFile)
                Next
                Directory.Delete(ruta_temporal)
            End Using
            Add_zip_version_documento = "YES"
        Catch ex As Exception
            Add_zip_version_documento = "Inconsistencia general funcion Add_zip_version_documento " & ex.Message
        End Try
    End Function
    Function Add_zip_expediente_files(ByVal id_expediente As Integer,
                                      ByVal rut_file As String,
                                      ByVal url_source_file_zip As String,
                                      ByVal stru_file_system() As stru_file_system,
                                      ByRef rut_file_zip As String,
                                      ByRef url_file_zip As String,
                                      ByRef name_document As String) As String
        '---------------------------------------------------------------------------
        'Funcion : Retorna zip con documentos de descarga
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_expediente         : Representa la identificación del expediente
        'rut_file              : Representa la ruta del zil
        'stru_file_system      : Representa la estrucutura de expedientes
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'rut_file_zip         : Retorna la ruta del documento ZIP
        'url_file_zip         : Retorna la url para la descarga del ZIP
        'name_document        : Retorna el nombre del documento a descargar
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-12-09
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            If Directory.Exists(rut_file) = False Then
                Directory.CreateDirectory(rut_file)
            End If
            For Each deleteFile In Directory.GetFiles(rut_file, "*.*", SearchOption.TopDirectoryOnly)
                File.Delete(deleteFile)
            Next
            Dim files_temp() As String = Nothing
            Dim iCount_files As Integer = 0
            Using zip As New ZipFile()
                For i As Integer = 0 To stru_file_system.Length - 1
                    Dim name_file As String = ""
                    If stru_file_system(i).name_file_tipo_logia = "" Then
                        name_file = stru_file_system(i).name_file
                    Else
                        name_file = stru_file_system(i).name_file_tipo_logia
                    End If
                    If stru_file_system(i).stru_files_da Is Nothing Then
                        zip.AddFile(stru_file_system(i).source_file, name_file)
                    Else
                        Dim out_source_zip As String = ""
                        Dim unzip As Object = Nothing
                        Add_zip_expediente_files_aux(name_file, rut_file, i, stru_file_system(i).stru_files_da, out_source_zip, unzip)
                        zip.AddFile(out_source_zip, name_file)
                        ReDim Preserve files_temp(iCount_files)
                        files_temp(iCount_files) = out_source_zip
                        iCount_files = iCount_files + 1
                    End If
                Next
                Dim zipName As String = "file_expe_" & id_expediente & ".zip"
                name_document = zipName
                rut_file_zip = rut_file & "\" & zipName
                If File.Exists(rut_file_zip) = True Then
                    File.Delete(rut_file_zip)
                End If
                zip.Save(rut_file_zip)
                url_file_zip = url_source_file_zip & zipName
                If Not files_temp Is Nothing Then
                    For i As Integer = 0 To files_temp.Length - 1
                        If File.Exists(files_temp(i)) = True Then
                            File.Delete(files_temp(i))
                        End If
                    Next
                End If
            End Using
            Add_zip_expediente_files = "YES"
        Catch ex As Exception
            Add_zip_expediente_files = "Inconsistencia general fucion Add_zip_expediente_files (" & ex.Message & ")"
        End Try
    End Function
    Function Add_zip_expediente_files_aux(ByVal name_file As String,
                                          ByVal rut_file As String,
                                          ByVal icunt_file As Integer,
                                          ByVal stru_file_system() As stru_file_system_da,
                                          ByRef rut_file_zip As String,
                                          ByRef unzip As Object) As String
        '---------------------------------------------------------------------------
        'Funcion : Retorna zip con documentos de descarga
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_expediente         : Representa la identificación del expediente
        'rut_file              : Representa la ruta del zil
        'stru_file_system      : Representa la estrucutura de expedientes
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'rut_file_zip         : Retorna la ruta del documento ZIP
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-12-09
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            If Directory.Exists(rut_file) = False Then
                Directory.CreateDirectory(rut_file)
            End If
            Using zip As New ZipFile()
                For i As Integer = 0 To stru_file_system.Length - 1
                    zip.AddFile(stru_file_system(i).source_file, name_file)
                Next
                Dim zipName As String = "file_anex" & icunt_file & name_file & ".zip"
                rut_file_zip = rut_file & "\" & zipName
                If File.Exists(rut_file_zip) = True Then
                    File.Delete(rut_file_zip)
                End If
                zip.Save(rut_file_zip)
            End Using
            Add_zip_expediente_files_aux = "YES"
        Catch ex As Exception
            Add_zip_expediente_files_aux = "Inconsistencia general fucion Add_zip_expediente_files_aux (" & ex.Message & ")"
        End Try
    End Function
    Function Delete_directory(ByVal path_files As String) As String
        Try
            For Each deleteFile In Directory.GetFiles(path_files, "*.*", SearchOption.TopDirectoryOnly)
                File.Delete(deleteFile)
            Next
            Delete_directory = "YES"
        Catch ex As Exception
            Delete_directory = " Error  Delete_directory " & ex.Message
        End Try
    End Function
    Function Solicita_peso_matriz_documentos(ByVal matri_documentos() As String,
                                             ByRef peso_documento As String) As String
        '---------------------------------------------------------------------------
        'Funcion : Retorna el peso de los documentos relacionados en la matriz
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'matri_documentos      : Representa la estructura de documentos relacionados
        '                      : y la ruta file sistem de los documentos
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'peso_documento        : Retorna el tamaño del peso de los archivos en MB 
        '                         y en KB
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-07-02
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim tamano As String = ""
            Dim tam_archivo As Object = 1024
            For i As Integer = 0 To matri_documentos.Length - 1
                Dim fi As New FileInfo(matri_documentos(i))
                If fi.Exists Then
                    tam_archivo = tam_archivo + fi.Length
                End If
            Next
            If (tam_archivo / 1024) > 1024 Then
                tamano = Math.Round(((tam_archivo / 1024) / 1024), 2).ToString() & " Mb"
            Else
                tamano = Math.Round((tam_archivo / 1024), 2).ToString() & " Kb"
            End If
            peso_documento = tamano
            Solicita_peso_matriz_documentos = "YES"
        Catch ex As Exception
            Solicita_peso_matriz_documentos = "Inconsistencia general funcion Solicita_peso_matriz_documentos " & ex.Message
        End Try
    End Function

End Class
