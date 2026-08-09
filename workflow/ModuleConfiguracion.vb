Module ModuleConfiguracion
    
    Public Function Obtener_Ceros_Imagen(ByVal Id_Imagen As String, _
                                         ByRef Valor_Ceros_Imagen As String) As String
        Try
            Dim ceros3 As String
            ceros3 = ""
            Select Case Len(Id_Imagen)
                Case "1"
                    ceros3 = ceros3 & "0000000"
                Case "2"
                    ceros3 = ceros3 & "000000"
                Case "3"
                    ceros3 = ceros3 & "00000"
                Case "4"
                    ceros3 = ceros3 & "0000"
                Case "5"
                    ceros3 = "000"
                Case "6"
                    ceros3 = "00"
                Case "7"
                    ceros3 = "0"
                Case "8"
                    ceros3 = ""
            End Select
            Valor_Ceros_Imagen = Valor_Ceros_Imagen & ceros3 & Id_Imagen
            Obtener_Ceros_Imagen = "YES"
        Catch ex As Exception
            Obtener_Ceros_Imagen = ex.ToString
        End Try

    End Function
    Public Function Obtener_Ceros_Carpeta_Imagen(ByVal id_Carpeta As String, _
                                                 ByRef Datos_Carpeta As String) As String
        Try
            Dim SiguiVIs2 As String = ""
            Select Case Len(id_Carpeta)
                Case "1"
                    SiguiVIs2 = SiguiVIs2 & "0000"
                Case "2"
                    SiguiVIs2 = SiguiVIs2 & "000"
                Case "3"
                    SiguiVIs2 = SiguiVIs2 & "00"
                Case "4"
                    SiguiVIs2 = SiguiVIs2 & "0"
                Case "5"
                    'ceros33 = ceros33
            End Select
            Datos_Carpeta = SiguiVIs2 & id_Carpeta
            Obtener_Ceros_Carpeta_Imagen = "YES"
        Catch ex As Exception
            Obtener_Ceros_Carpeta_Imagen = ex.ToString
        End Try
    End Function
    Public Function Obtener_Ceros_Numero_Imagen(ByVal id_Carpeta As String, _
                                                ByRef Datos_Carpeta As String) As String
        Try
            Dim SiguiVIs2 As String = ""
            Select Case Len(id_Carpeta)
                Case "1"
                    SiguiVIs2 = SiguiVIs2 & "0000"
                Case "2"
                    SiguiVIs2 = SiguiVIs2 & "000"
                Case "3"
                    SiguiVIs2 = SiguiVIs2 & "00"
                Case "4"
                    SiguiVIs2 = SiguiVIs2 & "0"
                Case "5"
                    'SiguiVIs2 = SiguiVIs2 & "0"
            End Select
            Datos_Carpeta = SiguiVIs2 & id_Carpeta
            Obtener_Ceros_Numero_Imagen = "YES"
        Catch ex As Exception
            Obtener_Ceros_Numero_Imagen = ex.ToString
        End Try
    End Function
    Public Function Obtener_Ceros_Nueva_Imagen(ByVal Numero_Pagina As String, _
                                               ByRef CerosImag As String) As String
        Try
            Dim consco41 As String = ""
            If Numero_Pagina = 2 Then consco41 = 0
            If Numero_Pagina > 2 Then consco41 = Val(Numero_Pagina) - 2
            CerosImag = ""
            Select Case Len(consco41)
                Case "1"
                    CerosImag = CerosImag & "0000"
                Case "2"
                    CerosImag = CerosImag & "000"
                Case "3"
                    CerosImag = CerosImag & "00"
                Case "4"
                    CerosImag = CerosImag & "0"
                Case "5"
                    CerosImag = CerosImag & ""
            End Select
            CerosImag = CerosImag & consco41
            Obtener_Ceros_Nueva_Imagen = "YES"
        Catch ex As Exception
            Obtener_Ceros_Nueva_Imagen = ex.ToString
        End Try
    End Function

    Function Ceros_Imagen_Alamacenada_ext(ByVal Val_Ext As Integer, _
                                          ByRef Ceros_Ext As String) As String
        Ceros_Ext = ""
        Try
            Select Case Len(Val_Ext.ToString)
                Case "1"
                    Ceros_Ext = Ceros_Ext & "0000"
                Case "2"
                    Ceros_Ext = Ceros_Ext & "000"
                Case "3"
                    Ceros_Ext = Ceros_Ext & "00"
                Case "4"
                    Ceros_Ext = Ceros_Ext & "0"
                Case "5"
                    Ceros_Ext = ""
            End Select
            Ceros_Imagen_Alamacenada_ext = "YES"
        Catch ex As Exception
            Ceros_Imagen_Alamacenada_ext = ex.ToString
        End Try
    End Function
    Public Function Ceros_Imagen_Carpeta(ByVal Valor_Imagen As String, _
                                         ByRef Ceros_Imagen As String) As String
        Try
            Ceros_Imagen = ""
            Select Case Len(Valor_Imagen)
                Case "1"
                    Ceros_Imagen = Ceros_Imagen & "0000"
                Case "2"
                    Ceros_Imagen = Ceros_Imagen & "000"
                Case "3"
                    Ceros_Imagen = Ceros_Imagen & "00"
                Case "4"
                    Ceros_Imagen = Ceros_Imagen & "0"
                Case "5"
                    Ceros_Imagen = ""
            End Select
            Ceros_Imagen_Carpeta = "YES"
            Exit Function
        Catch ex As Exception
            Ceros_Imagen_Carpeta = ex.ToString()
        End Try
    End Function
    Function Ceros_Imagen_Almacenada(ByRef Ceros_Cuerpo_Imag As String, _
                                     ByVal Idal As Long) As String
        Try
            Ceros_Cuerpo_Imag = ""
            Select Case Len(Idal.ToString)
                Case "1"
                    Ceros_Cuerpo_Imag = Ceros_Cuerpo_Imag & "0000000"
                Case "2"
                    Ceros_Cuerpo_Imag = Ceros_Cuerpo_Imag & "000000"
                Case "3"
                    Ceros_Cuerpo_Imag = Ceros_Cuerpo_Imag & "00000"
                Case "4"
                    Ceros_Cuerpo_Imag = Ceros_Cuerpo_Imag & "0000"
                Case "5"
                    Ceros_Cuerpo_Imag = Ceros_Cuerpo_Imag & "000"
                Case "6"
                    Ceros_Cuerpo_Imag = Ceros_Cuerpo_Imag & "00"
                Case "7"
                    Ceros_Cuerpo_Imag = Ceros_Cuerpo_Imag & "0"
                Case "8"
                    Ceros_Cuerpo_Imag = ""
            End Select
            Ceros_Imagen_Almacenada = "YES"
        Catch ex As Exception
            Ceros_Imagen_Almacenada = ex.ToString
        End Try
    End Function
    Public Function Codigo_Nueva_Image(ByVal matri_doc As String) As String
        Try
            Dim Numero_Aleatorio As Integer = intNumeroaleatorio()
            Dim MAUM As String
            Dim MAS As DateTime
            MAS = Now
            MAUM = MAS
            MAUM = Replace(MAUM, "/", "")
            MAUM = Replace(MAUM, ":", "")
            MAUM = Replace(MAUM, " ", "")
            MAUM = Replace(MAUM, "PM", "")
            MAUM = Replace(MAUM, "AM", "")
            MAUM = Replace(MAUM, "pm", "")
            MAUM = Replace(MAUM, "am", "")
            MAUM = Replace(MAUM, ".", "")
            Codigo_Nueva_Image = MAUM & Numero_Aleatorio & matri_doc & ".jpeg"
        Catch EX As Exception
            Codigo_Nueva_Image = EX.ToString
        End Try
    End Function
    Public Function intNumeroaleatorio() As Integer
        Dim Time_Dat As DateTime
        Time_Dat = Now
        intNumeroaleatorio = Time_Dat.Millisecond()


    End Function
End Module
