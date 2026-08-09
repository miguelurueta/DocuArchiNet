Public Class ClassGestionFechas
    Function Solicita_fecha_limite_tramite(ByVal id_plantilla As Integer,
                                           ByVal nombre_tramite As String,
                                           ByRef fecha_vence As String) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita la fecha limite de una tramite para responder
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'nombre_tramite        : Representa el nombre del tramite
        'id_plantilla          : Representa la identiifcación de la plantilla
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'fecha_vence           : Retorna la fecha de vencimiento
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-10-25
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------

        Try
            Dim Result As String = ""
            fecha_vence = ""
            Dim ClassRadicador As New ClassRadicador
            Dim numero_dias As Integer = 0
            Result = ClassRadicador.Retorna_Dias_Vencimiento_tramite(id_plantilla,
                                                                     nombre_tramite,
                                                                     numero_dias)
            If Result <> "YES" Then
                Solicita_fecha_limite_tramite = Result
                Exit Function
            End If
            If numero_dias = 0 Then
                Solicita_fecha_limite_tramite = "YES"
                Exit Function
            End If
            Dim ob As Object
            Result = ClassRadicador.DateAgregarLaborales(numero_dias,
                                                         Date.Now,
                                                         ob)
            If Result <> "YES" Then
                Solicita_fecha_limite_tramite = Result
                Exit Function
            Else
                Dim feha As String = ob
                Result = Me.formata_fecha_tipo_date(feha)
                If Result <> "YES" Then
                    Solicita_fecha_limite_tramite = Result
                    Exit Function
                Else
                    feha = Left(feha, 10)
                    fecha_vence = feha.Replace("/", "-")
                    Solicita_fecha_limite_tramite = "YES"
                    Exit Function
                End If
            End If
        Catch ex As Exception
            Solicita_fecha_limite_tramite = "Inconsistencia general funcion Solicita_fecha_limite_tramite " & ex.Message
        End Try
    End Function
    'Function Resta_fechas(ByVal fecha_ini As String,
    '                      ByVal fecha_fin As String,
    '                      ByRef min As Long) As String
    '    '-------------------------------------------
    '    'Función : Resta las fechas en los formatos
    '    ' yyyy-mm-dd-MM HH:mm:ss con AM O PM
    '    'Fecha : 2018-10-16
    '    '--------------------------------------------
    '    Try
    '        If fecha_ini = "" Then
    '            Resta_fechas = "La fecha inicial para sacar la diferencia esta vacia"
    '            Exit Function
    '        End If
    '        If fecha_fin = "" Then
    '            Resta_fechas = "La fecha final para sacar la diferencia esta vacia"
    '            Exit Function
    '        End If
    '        Dim inicio As DateTime = New DateTime(CInt(fecha_ini.Substring(0, 4)), CInt(fecha_ini.Substring(5, 2)), CInt(fecha_ini.Substring(8, 2)) _
    '         , CInt(fecha_ini.Substring(11, 2)), CInt(fecha_ini.Substring(14, 2)), CInt((fecha_ini.Substring(17, 2))))
    '        Dim fin As DateTime = New DateTime(CInt(fecha_fin.Substring(0, 4)), CInt(fecha_fin.Substring(5, 2)), CInt(fecha_fin.Substring(8, 2)) _
    '             , CInt(fecha_fin.Substring(11, 2)), CInt(fecha_fin.Substring(14, 2)), CInt((fecha_fin.Substring(17, 2))))
    '        Dim duracion As TimeSpan = fin - inicio
    '        min = CInt(CLng(duracion.TotalMinutes))
    '        Resta_fechas = "YES"
    '    Catch ex As Exception
    '        Resta_fechas = "Inconsistencia general función Resta_fechas " & ex.Message
    '    End Try
    'End Function
    Function Resta_fechas_db(ByVal fecha_ini As String,
                             ByVal fecha_fin As String,
                             ByRef min As Long) As String
        '-------------------------------------------
        'Función : Resta las fechas en los formatos
        ' yyyy-mm-dd-MM HH:mm:ss sin AM O PM
        'Fecha : 2018-10-16
        '--------------------------------------------
        Try
            If fecha_ini = "" Then
                Resta_fechas_db = "La fecha inicial para sacar la diferencia esta vacia"
                Exit Function
            End If
            If fecha_fin = "" Then
                Resta_fechas_db = "La fecha final para sacar la diferencia esta vacia"
                Exit Function
            End If
            Dim inicio As DateTime = New DateTime(CInt(fecha_ini.Substring(0, 4)), CInt(fecha_ini.Substring(5, 2)), CInt(fecha_ini.Substring(8, 2)) _
             , CInt(fecha_ini.Substring(11, 2)), CInt(fecha_ini.Substring(14, 2)), CInt((fecha_ini.Substring(17, 2))))
            Dim fin As DateTime = New DateTime(CInt(fecha_fin.Substring(0, 4)), CInt(fecha_fin.Substring(5, 2)), CInt(fecha_fin.Substring(8, 2)) _
                 , CInt(fecha_fin.Substring(11, 2)), CInt(fecha_fin.Substring(14, 2)), CInt((fecha_fin.Substring(17, 2))))
            Dim duracion As TimeSpan = fin - inicio
            min = CInt(CLng(duracion.TotalMinutes))
            Resta_fechas_db = "YES"
        Catch ex As Exception
            Resta_fechas_db = "Inconsistencia general función Resta_fechas_db " & ex.Message
        End Try
    End Function
    Function Formatea_fecha_time_base_mysql(ByVal Fecha_Date As String,
                                            ByRef fecha_format As String) As String
        Try
            Dim Fecha_String As String = Fecha_Date
            fecha_format = Fecha_String.Substring(6, 4) & "-" & Fecha_String.Substring(3, 2) & "-" & Fecha_String.Substring(0, 2) _
            & "" & Fecha_String.Substring(10, 9)
            Formatea_fecha_time_base_mysql = "YES"
        Catch ex As Exception
            Return ex.Message.ToString & "Error 3 Fromateando Fecha Funcion Formatea_Fecha_Time_MYSQL"
        End Try
    End Function
    Function Formatea_fecha_date_base_mysql(ByVal Fecha_Date As String,
                                            ByRef fecha_format As String) As String
        Try
            Dim Fecha_String As String = Fecha_Date
            fecha_format = Fecha_String.Substring(6, 4) & "-" & Fecha_String.Substring(3, 2) & "-" & Fecha_String.Substring(0, 2)
            Formatea_fecha_date_base_mysql = "YES"
        Catch ex As Exception
            Return ex.Message.ToString & "Error 3 Fromateando Fecha Funcion Formatea_fecha_date_base_mysql"
        End Try
    End Function
    Function Formatea_fecha_time_xmls_indice(ByRef fecha_format As String) As String
        Try

            Dim spli As String = ""
            If InStr(fecha_format, "-") > 0 Then
                spli = "-"
            End If
            If InStr(fecha_format, "/") Then
                spli = "/"
            End If
            Dim split() As String = fecha_format.Split(spli)
            fecha_format = split(2) & "/" & split(1) & "/" & split(0)
            Formatea_fecha_time_xmls_indice = "YES"
        Catch ex As Exception
            Return ex.Message.ToString & "Error 4 Fromateando Fecha Formatea_fecha_time_xmls_indice"
        End Try
    End Function
    Function Formatea_fecha_time_framework(ByVal Fecha_Date As DateTime,
                                           ByRef fecha_format As String) As String
        Try
            fecha_format = Trim(CStr(Now.ToString("yyyy'-'MM'-'dd HH:mm:ss")))
            Formatea_fecha_time_framework = "YES"
        Catch ex As Exception
            Formatea_fecha_time_framework = "Formatea_fecha_time_framework dice " & ex.Message
        End Try
    End Function
    Function Formatea_fecha_time_db(ByVal Fecha_Date As Date,
                                    ByRef fecha_format As String) As String
        Try
            fecha_format = Trim(CStr(Fecha_Date.ToString("yyyy'-'MM'-'dd HH:mm:ss")))
            Formatea_fecha_time_db = "YES"
        Catch ex As Exception
            Formatea_fecha_time_db = "Formatea_fecha_time_framework dice " & ex.Message
        End Try
    End Function
    Function FormateaFechaTimeDbDefault(ByVal Fecha_Date As Date,
                                        ByRef FechaFormat As String) As String
        Try
            FechaFormat = Trim(CStr(Fecha_Date.ToString("dd'/'MM'/'yyyy HH:mm:ss")))
            FormateaFechaTimeDbDefault = "YES"
        Catch ex As Exception
            FormateaFechaTimeDbDefault = "Formatea_fecha_time_db_default dice " & ex.Message
        End Try
    End Function

    Function Solicita_tiempo_respuesta_tramite(ByVal fecha_registro_tramite As String,
                                              ByRef tiempo_respuesta_dias As Object,
                                              ByRef tiempo_respuesta_hora As Object,
                                              ByRef tiempo_respuesta_minuto As Object,
                                              ByRef dias_calendario As Object,
                                              ByRef dias_no_habiles As Object,
                                              Optional fecha_respuestas As String = "") As String
        Try
            Dim Result As String = ""
            '------------------------------------------------------
            'Formula el numero de dias transcurridos entre la fecha
            'de inicial y la fecha actual
            '------------------------------------------------------

            Dim Ref_fecha_registro_tramite As String = fecha_registro_tramite
            Result = Me.formata_fecha_tipo_date(Ref_fecha_registro_tramite)
            If Result <> "YES" Then
                Solicita_tiempo_respuesta_tramite = "Imposible formatear fecha " & Result
                Exit Function
            End If
            Dim DateCreate As String = ""
            Dim fecha_actual As String = ""
            Dim fecha_actual_no_habil As String = ""
            If fecha_respuestas = "" Then
                Result = Formatea_fecha_time_framework(Date.Now,
                                                       DateCreate)
                If Result <> "YES" Then
                    Solicita_tiempo_respuesta_tramite = "Imposible formatear fecha " & Result
                    Exit Function
                End If
                fecha_actual_no_habil = DateCreate
                fecha_actual = DateCreate.ToString
            Else
                fecha_actual_no_habil = fecha_respuestas
                Result = formata_fecha_tipo_date(fecha_respuestas)
                If Result <> "YES" Then
                    Solicita_tiempo_respuesta_tramite = "Imposible formatear fecha " & Result
                    Exit Function
                End If
                fecha_actual = fecha_respuestas
            End If
            tiempo_respuesta_dias = DateDiff("d", Ref_fecha_registro_tramite, fecha_actual)
            dias_calendario = tiempo_respuesta_dias
            Dim numero_dias_no_habil As Integer = 0
            Result = Me.Retorna_numero_dias_no_habiles(fecha_registro_tramite,
                                                       fecha_actual_no_habil,
                                                       numero_dias_no_habil)
            If Result <> "YES" Then
                Solicita_tiempo_respuesta_tramite = Result
                Exit Function
            End If
            dias_no_habiles = numero_dias_no_habil
            If numero_dias_no_habil > 0 Then
                tiempo_respuesta_dias = Val(tiempo_respuesta_dias) - numero_dias_no_habil
            End If
            If tiempo_respuesta_dias < 0 Then
                tiempo_respuesta_dias = 0
            End If
            '-----------------------------------------------------
            'Retorna la hora
            '----------------------------------------------------
            tiempo_respuesta_hora = (DateDiff("s", Ref_fecha_registro_tramite, fecha_actual) \ 3600) Mod 24
            '----------------------------------------------------
            'Retorna segundos  
            '----------------------------------------------------
            tiempo_respuesta_minuto = (DateDiff("s", Ref_fecha_registro_tramite, fecha_actual) \ 60) Mod 60
            Solicita_tiempo_respuesta_tramite = "YES"
        Catch ex As Exception
            Solicita_tiempo_respuesta_tramite = "Inconsistencia general función Solicita_tiempo_respuesta_tramite " & ex.Message
        End Try
    End Function
    Function formata_fecha_tipo_date(ByRef date1al As String) As String

        Try
            If date1al = "" Then
                formata_fecha_tipo_date = "No se puede formatear una fecha vacia"
                Exit Function
            End If
            Dim separador As String = ""
            If InStr(date1al, "/") > 0 Then
                separador = "/"
            End If
            If InStr(date1al, "-") > 0 Then
                separador = "-"
            End If
            If separador = "" Then
                formata_fecha_tipo_date = "No se puede formatear una fecha sin separador valido"
                Exit Function
            End If
            Dim SplitWf() As String = Left(date1al, 10).Split(separador)
            Dim Time As String = date1al.Replace(Left(date1al, 10), "")
            Time = Time.Replace("AM", "")
            Time = Time.Replace("PM", "")
            Time = Time.Replace("a", "")
            Time = Time.Replace("m", "")
            Time = Time.Replace("p", "")
            Time = Time.Replace(".", "")
            Time = Time.Replace(" ", "")
            If Not SplitWf Is Nothing Then
                date1al = SplitWf(2) & "/" & SplitWf(1) & "/" & SplitWf(0) & " " & Time
                formata_fecha_tipo_date = "YES"
                Exit Function
            Else
                date1al = ""
                formata_fecha_tipo_date = "#Error funcion formata_fecha_tipo_date "
                Exit Function
            End If
        Catch ex As Exception
            formata_fecha_tipo_date = "Inconsistencia general funcion formata_fecha_tipo_date " & ex.Message
        End Try
    End Function
    Function Formatea_Fecha_Almacenamiento_Time(ByRef date1al As String) As String
        '****************************************
        'Funcion : Formatea la fecha al formato
        'año-mes-dia
        'Fecha : 2010-09-02
        '*****************************************
        Try
            Dim Fecha As String = Trim(CStr(Now.ToString("yyyy'-'MM'-'dd HH':'mm':'ss")))
            Dim SplitWf() As String = Left(Fecha, 10).Split("-")
            Dim Time As String = Fecha.Replace(Left(Fecha, 10), "")
            Time = Time.Replace("AM", "")
            Time = Time.Replace("PM", "")
            Time = Time.Replace("a", "")
            Time = Time.Replace("m", "")
            Time = Time.Replace("p", "")
            Time = Time.Replace(".", "")
            Time = Time.Replace(" ", "")
            If Not SplitWf Is Nothing Then
                date1al = SplitWf(0) & "/" & SplitWf(1) & "/" & SplitWf(2) & " " & Time
                Formatea_Fecha_Almacenamiento_Time = "YES"
            Else
                date1al = ""
                Formatea_Fecha_Almacenamiento_Time = "#Error formatenado fecha"
                Exit Function
            End If
        Catch ex As Exception
            Formatea_Fecha_Almacenamiento_Time = ex.Message
        End Try
    End Function
    Function FormateaFechaAlmacenamiento(ByRef date1al As String) As String
        '****************************************
        'Funcion : Formatea la fecha al formato
        'año-mes-dia
        'Fecha : 2010-09-02
        '*****************************************
        Try
            Dim Fecha As String = Trim(CStr(Now.ToString("yyyy'-'MM'-'dd")))
            Dim SplitWf() As String = Left(Fecha, 10).Split("-")
            If Not SplitWf Is Nothing Then
                date1al = SplitWf(0) & "/" & SplitWf(1) & "/" & SplitWf(2)
            Else
                date1al = ""
                FormateaFechaAlmacenamiento = "#Error formatenado fecha"
                Exit Function
            End If

            FormateaFechaAlmacenamiento = "YES"
        Catch ex As Exception
            FormateaFechaAlmacenamiento = ex.ToString
        End Try
    End Function
    Function Formatea_Fecha_Almacenamiento_guion(ByRef date1al As String) As String
        '****************************************
        'Funcion : Formatea la fecha al formato
        'año-mes-dia
        'Fecha : 2010-09-02
        '*****************************************
        Try
            Dim Fecha As String = Trim(CStr(Now.ToString("yyyy'-'MM'-'dd")))
            Dim SplitWf() As String = Left(Fecha, 10).Split("-")
            If Not SplitWf Is Nothing Then
                date1al = SplitWf(0) & "-" & SplitWf(1) & "-" & SplitWf(1)
            Else
                date1al = ""
                Formatea_Fecha_Almacenamiento_guion = "#Error formatenado fecha"
                Exit Function
            End If

            Formatea_Fecha_Almacenamiento_guion = "YES"
        Catch ex As Exception
            Formatea_Fecha_Almacenamiento_guion = ex.ToString
        End Try
    End Function
    'Function Formatea_Fecha_date_now(ByRef date1al As String) As String
    '    '****************************************
    '    'Funcion : Formatea la fecha al formato
    '    'año-mes-dia
    '    'Fecha : 2010-09-02
    '    '*****************************************
    '    Try
    '        Dim SplitWf() As String = Left(date1al, 10).Split("/")
    '        If Not SplitWf Is Nothing Then
    '            date1al = SplitWf(2) & "-" & SplitWf(1) & "-" & SplitWf(0)
    '        Else
    '            date1al = ""
    '            Formatea_Fecha_date_now = "#Error formatenado fecha"
    '            Exit Function
    '        End If
    '        Formatea_Fecha_date_now = "YES"
    '    Catch ex As Exception
    '        Formatea_Fecha_date_now = ex.ToString
    '    End Try
    'End Function
    Function Retorna_fecha_registro(ByRef date1al As String) As String
        '-----------------------------------------
        'Funcion : Formatea la fecha al formato
        'año-mes-dia
        'Fecha : 2018-06-27
        'Ingeniero : Miguel Angel Urueta Miranda
        '-----------------------------------------
        Try
            Dim Fecha As String = Trim(CStr(Now.ToString("yyyy'-'MM'-'dd HH':'mm':'ss")))
            'Dim Fecha As String = date1al
            Dim SplitWf() As String = Left(Fecha, 10).Split("-")
            Dim Time As String = Fecha.Replace(Left(Fecha, 10), "")
            Time = Time.Replace("AM", "")
            Time = Time.Replace("PM", "")
            Time = Time.Replace("a", "")
            Time = Time.Replace("m", "")
            Time = Time.Replace("p", "")
            Time = Time.Replace(".", "")
            Time = Time.Replace(" ", "")
            If Not SplitWf Is Nothing Then
                date1al = SplitWf(0) & "/" & SplitWf(1) & "/" & SplitWf(2) & " " & Time
                Retorna_fecha_registro = "YES"
                Exit Function
            Else
                date1al = ""
                Retorna_fecha_registro = "#Error formatendo fecha date time, función Retorna_fecha_registro "
                Exit Function
            End If

        Catch ex As Exception
            Retorna_fecha_registro = "Inconsistencia general función Retorna_fecha_registro " & ex.Message
        End Try
    End Function
    Function Retorna_numero_dias_no_habiles(ByVal FechaInicial As Object,
                                            ByVal fecha_salida As Object,
                                            ByRef Numero_Dia As Integer) As String
        Try
            Dim Result As String = ""
            Dim Matri_festivos() As String
            Erase Matri_festivos
            Result = Retorna_dias_festivos(FechaInicial,
                                           fecha_salida,
                                           Matri_festivos)
            If Result <> "YES" Then
                Retorna_numero_dias_no_habiles = Result
                Exit Function
            End If
            Dim strin As String = FechaInicial.ToString
            Dim fechafin As Date = DateTime.Parse(strin)
            Dim diassabdomingo As Integer = 0
            Dim sTiempo As Object = DateDiff("d", FechaInicial, fecha_salida)
            Dim i As Integer = 1
            Dim fech As Object = Nothing
            Dim fecha_comul As Object = Nothing
            While i <= Val(sTiempo)
                fech = fechafin.AddDays(i)
                If fech.DayOfWeek = 6 Or fech.DayOfWeek = 0 Then
                    If fech.DayOfWeek = 6 Then

                        diassabdomingo = diassabdomingo + 1
                    End If
                    If fech.DayOfWeek = 0 Then

                        diassabdomingo = diassabdomingo + 1
                    End If
                End If
                i = i + 1
            End While
            If Not Matri_festivos Is Nothing Then
                Numero_Dia = Numero_Dia + Matri_festivos.Length
            End If
            Numero_Dia = Numero_Dia + diassabdomingo
            Retorna_numero_dias_no_habiles = "YES"
        Catch ex As Exception
            Retorna_numero_dias_no_habiles = "Inconsistencia función Retorna_numero_dias_no_habiles " & ex.Message
        End Try
    End Function
    Function Retorna_dias_festivos(ByVal Fi As Object,
                                   ByVal Ff As Object,
                                   ByRef libres() As String) As String

        Try
            Dim refclas_gestion_fechas As New ClassGestionFechas
            Dim date1fi As String = ""
            Dim date1ff As String = ""
            Dim Result As String = ""
            Dim obfei = Fi
            date1fi = obfei
            Result = refclas_gestion_fechas.formata_fecha_tipo_date(date1fi)
            If Result <> "YES" Then
                Retorna_dias_festivos = Result
                Exit Function
            End If
            date1fi = Left(date1fi, 10)
            Dim obff = Ff
            date1ff = obff
            Result = refclas_gestion_fechas.formata_fecha_tipo_date(date1ff)
            If Result <> "YES" Then
                Retorna_dias_festivos = Result
                Exit Function
            End If
            date1ff = Left(date1ff, 10)
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "Select FECHA_FERIADO " &
              " from REA_001_FERIADOS where ESTADO_DIA=1 and FECHA_FERIADO BETWEEN '" & date1fi & "' AND '" & date1ff & "' ORDER BY  FECHA_FERIADO"
            Dim Dat_reader As New DataSet
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Retorna_dias_festivos = " Error consultando dias no laborales  " & Result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count = 0 Then

                Retorna_dias_festivos = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Dat_reader.Tables(0).Rows.Count - 1
                    ReDim Preserve libres(i)
                    libres(i) = Dat_reader.Tables(0).Rows(i).Item(0)
                Next

                Retorna_dias_festivos = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_dias_festivos = "Inconsistencia General Funcion Retorna_dias_festivos " & ex.Message
        End Try
    End Function
    Function csfc_Formatea_Fecha_Time_MYSQL_Fecha_Inicio(ByRef Fecha_Radic As String) As String
        Dim Fecha_String As String = Left(Trim(CType(Fecha_Radic, String)), 10)
        Try
            Dim Fecha As String = Fecha_Radic.Replace("-", "")
            Dim Dia As String = Mid(Fecha, 1, 2)
            Dim Mes As String = Mid(Fecha, 4, 2)
            Dim YY As String = Mid(Fecha, 7, 4)
            Fecha_Radic = YY & "-" & Mes & "-" & Dia
            csfc_Formatea_Fecha_Time_MYSQL_Fecha_Inicio = "YES"
        Catch ex As Exception
            csfc_Formatea_Fecha_Time_MYSQL_Fecha_Inicio = ex.Message.ToString & "Error csfc_Formatea_Fecha_Time_MYSQL_Fecha_Inicio " & Fecha_Radic
        End Try
    End Function

    Function csfc_Formatea_Fecha_Almacenamiento_Time_bsd(ByRef date1al As String) As String
        Try
            Dim fecha As String = Left(date1al.ToString, 10)
            Dim Dia As String = Mid(fecha, 1, 2)
            Dim Mes As String = Mid(fecha, 4, 2)
            Dim YY As String = Mid(fecha, 7, 4)
            Dim Time As String = date1al.Replace(Left(date1al, 10), "")
            Dim t = Time.Length
            Time = Time.Replace("AM", "")
            Time = Time.Replace("PM", "")
            Time = Time.Replace("a", "")
            Time = Time.Replace("m", "")
            Time = Time.Replace("p", "")
            Time = Time.Replace(".", "")
            Time = Time.Replace(" ", "")
            date1al = YY & "-" & Mes & "-" & Dia & " " & Time
            csfc_Formatea_Fecha_Almacenamiento_Time_bsd = "YES"
        Catch ex As Exception
            csfc_Formatea_Fecha_Almacenamiento_Time_bsd = ex.Message & "Error csfc_Formatea_Fecha_Almacenamiento_Time_bsd " & date1al
        End Try
    End Function
    Function formato_fecha_estructura(ByRef fecha As String) As String
        Try
            Dim fecha_ref As String() = Left(fecha, 10).ToString.Split("/")
            fecha = fecha_ref(2) & "-" & fecha_ref(1) & "-" & fecha_ref(0)
            formato_fecha_estructura = "YES"
        Catch ex As Exception
            fecha = "Inconsistencia"
            formato_fecha_estructura = "YES"
        End Try
    End Function
    Function Verifi_campo_fecha(ByRef Fecha As String) As String
        '*************************************
        'Nombre Verifi_campo_fecha_Form6
        'Retornos CI=CARACTER INSUFICINETE
        'ED=ERROR EN EL DIA DE LA FECHA
        'Creacion Miguel Angel Urueta Miranda
        '*************************************
        Dim z As Integer = 0
        Dim BisestA As Integer
        Dim Numero_Caracter_fecha As Integer
        Dim Año_F, Mes_f, Dia_f, tip As String
        Try
            'Retirar carateres de formato
            Numero_Caracter_fecha = Len(Fecha)
            'validacion del numero de caracteres es cero
            If Numero_Caracter_fecha = 0 Then
                Verifi_campo_fecha = "La fecha esta sin datos"
                Exit Function
            End If
            'validacion del numero de caracteres es direnete de ocho
            If Numero_Caracter_fecha = 10 Or Numero_Caracter_fecha = 8 Then
            Else
                Verifi_campo_fecha = "El nmero de catacteres de l fecha es inconrecto"
                Exit Function
            End If
            If Numero_Caracter_fecha = 10 Then
                Año_F = Left(Fecha, 4)
                Dia_f = Right(Fecha, 2)
                Mes_f = Left(Fecha, 7)
                Mes_f = Right(Mes_f, 2)
            Else
                Año_F = Left(Fecha, 4)
                Dia_f = Right(Fecha, 2)
                Mes_f = Left(Fecha, 6)
                Mes_f = Right(Mes_f, 2)
            End If

            'Verifica el formato del dia
            If Val(Dia_f) > 31 Or Val(Dia_f) = 0 Then
                Verifi_campo_fecha = "ED_" & Año_F & "" & Mes_f & "(" & Dia_f & ")"
                Exit Function
            End If
            'Verifica el formato del mes
            If Val(Mes_f) > 12 Or Val(Mes_f) = 0 Then
                Verifi_campo_fecha = "EM_" & Año_F & "(" & Mes_f & ")" & Dia_f
                Exit Function
            End If
            Select Case Val(Mes_f)
                Case 1
                    If Val(Dia_f) > 31 Then
                        Verifi_campo_fecha = "ED_" & Año_F & "" & Mes_f & "(" & Dia_f & ")"
                        Exit Function
                    End If
                Case 2
                    'funcion año bisiesto
                    If (Val(Año_F)) Mod 4 = 0 Then
                        BisestA = 29
                    Else
                        BisestA = 28
                    End If
                    If Val(Dia_f) > BisestA Then
                        Verifi_campo_fecha = "ED_" & Año_F & "" & Mes_f & "(" & Dia_f & ")"
                        Exit Function
                    End If

                Case 3
                    If Val(Dia_f) > 31 Then
                        Verifi_campo_fecha = "ED_" & Año_F & "" & Mes_f & "(" & Dia_f & ")"
                        Exit Function
                    End If
                Case 4
                    If Val(Dia_f) > 30 Then
                        Verifi_campo_fecha = "ED_" & Año_F & "" & Mes_f & "(" & Dia_f & ")"
                        Exit Function
                    End If
                Case 5
                    If Val(Dia_f) > 31 Then
                        Verifi_campo_fecha = "ED_" & Año_F & "" & Mes_f & "(" & Dia_f & ")"
                        Exit Function
                    End If
                Case 6
                    If Val(Dia_f) > 30 Then
                        Verifi_campo_fecha = "ED_" & Año_F & "" & Mes_f & "(" & Dia_f & ")"
                        Exit Function
                    End If
                Case 7
                    If Val(Dia_f) > 31 Then
                        Verifi_campo_fecha = "ED_" & Año_F & "" & Mes_f & "(" & Dia_f & ")"
                        Exit Function
                    End If
                Case 8
                    If Val(Dia_f) > 31 Then
                        Verifi_campo_fecha = "ED_" & Año_F & "" & Mes_f & "(" & Dia_f & ")"
                        Exit Function
                    End If
                Case 9
                    If Val(Dia_f) > 30 Then
                        Verifi_campo_fecha = "ED_" & Año_F & "" & Mes_f & "(" & Dia_f & ")"
                        Exit Function
                    End If
                Case 10
                    If Val(Dia_f) > 31 Then
                        Verifi_campo_fecha = "ED_" & Año_F & "" & Mes_f & "(" & Dia_f & ")"
                        Exit Function
                    End If
                Case 11
                    If Val(Dia_f) > 30 Then
                        Verifi_campo_fecha = "ED_" & Año_F & "" & Mes_f & "(" & Dia_f & ")"
                        Exit Function
                    End If
                Case 12
                    If Val(Dia_f) > 31 Then
                        Verifi_campo_fecha = "ED_" & Año_F & "" & Mes_f & "(" & Dia_f & ")"
                        Exit Function
                    End If
            End Select
            If Numero_Caracter_fecha = 8 Then
                Fecha = Año_F & "-" & Mes_f & "-" & Dia_f
            End If
            Verifi_campo_fecha = "YES"
        Catch ex As Exception
            Verifi_campo_fecha = ex.Message
        End Try

    End Function
    Function Solicita_fecha_hora_sii(ByRef fecha As String,
                                     ByRef hora As String) As String
        Try
            fecha = Trim(CStr(Date.Now.ToString("yyyy'-'MM'-'dd HH:mm:ss")))
            fecha = fecha.Replace("-", "")
            fecha = fecha.Replace(":", "")
            hora = Right(fecha, 6)
            fecha = Left(fecha, 8)
            Solicita_fecha_hora_sii = "YES"
            Exit Function
        Catch ex As Exception
            Solicita_fecha_hora_sii = "Inconsistencia general funcion Solicita_fecha_hora_sii " & ex.Message
        End Try
    End Function
    Function Verifi_campo_fecha_Form6(ByVal consult1 As String) As String
        '*************************************
        'Nombre Verifi_campo_fecha_Form6
        'Retornos CI=CARACTER INSUFICINETE
        'ED=ERROR EN EL DIA DE LA FECHA
        'Creacion Miguel Angel Urueta Miranda
        '*************************************
        Dim z As Integer
        Dim BisestA As Integer
        Dim Numero_Caracter_fecha As Integer
        Dim Año_F, Mes_f, Dia_f, tip As String
        Try

            'Retirar carateres de formato
            Numero_Caracter_fecha = Len(consult1)
            'validacion del numero de caracteres es cero
            If Numero_Caracter_fecha = 0 Then
                Verifi_campo_fecha_Form6 = ""
                Exit Function
            End If
            'validacion del numero de caracteres es direnete de ocho
            If Numero_Caracter_fecha = 10 Or Numero_Caracter_fecha = 8 Then
            Else
                Verifi_campo_fecha_Form6 = "CI_" & consult1
                Exit Function
            End If
            If Numero_Caracter_fecha = 10 Then
                Año_F = Left(consult1, 4)
                Dia_f = Right(consult1, 2)
                Mes_f = Left(consult1, 7)
                Mes_f = Right(Mes_f, 2)
            Else
                Año_F = Left(consult1, 4)
                Dia_f = Right(consult1, 2)
                Mes_f = Left(consult1, 6)
                Mes_f = Right(Mes_f, 2)
            End If

            'Verifica el formato del dia
            If Val(Dia_f) > 31 Or Val(Dia_f) = 0 Then
                Verifi_campo_fecha_Form6 = "ED_" & Año_F & "" & Mes_f & "(" & Dia_f & ")"
                Exit Function
            End If
            'Verifica el formato del mes
            If Val(Mes_f) > 12 Or Val(Mes_f) = 0 Then
                Verifi_campo_fecha_Form6 = "EM_" & Año_F & "(" & Mes_f & ")" & Dia_f
                Exit Function
            End If
            Select Case Val(Mes_f)
                Case 1
                    If Val(Dia_f) > 31 Then
                        Verifi_campo_fecha_Form6 = "ED_" & Año_F & "" & Mes_f & "(" & Dia_f & ")"
                        Exit Function
                    End If
                Case 2
                    'funcion año bisiesto
                    If (Val(Año_F)) Mod 4 = 0 Then
                        BisestA = 29
                    Else
                        BisestA = 28
                    End If
                    If Val(Dia_f) > BisestA Then
                        Verifi_campo_fecha_Form6 = "ED_" & Año_F & "" & Mes_f & "(" & Dia_f & ")"
                        Exit Function
                    End If

                Case 3
                    If Val(Dia_f) > 31 Then
                        Verifi_campo_fecha_Form6 = "ED_" & Año_F & "" & Mes_f & "(" & Dia_f & ")"
                        Exit Function
                    End If
                Case 4
                    If Val(Dia_f) > 30 Then
                        Verifi_campo_fecha_Form6 = "ED_" & Año_F & "" & Mes_f & "(" & Dia_f & ")"
                        Exit Function
                    End If
                Case 5
                    If Val(Dia_f) > 31 Then
                        Verifi_campo_fecha_Form6 = "ED_" & Año_F & "" & Mes_f & "(" & Dia_f & ")"
                        Exit Function
                    End If
                Case 6
                    If Val(Dia_f) > 30 Then
                        Verifi_campo_fecha_Form6 = "ED_" & Año_F & "" & Mes_f & "(" & Dia_f & ")"
                        Exit Function
                    End If
                Case 7
                    If Val(Dia_f) > 31 Then
                        Verifi_campo_fecha_Form6 = "ED_" & Año_F & "" & Mes_f & "(" & Dia_f & ")"
                        Exit Function
                    End If
                Case 8
                    If Val(Dia_f) > 31 Then
                        Verifi_campo_fecha_Form6 = "ED_" & Año_F & "" & Mes_f & "(" & Dia_f & ")"
                        Exit Function
                    End If
                Case 9
                    If Val(Dia_f) > 30 Then
                        Verifi_campo_fecha_Form6 = "ED_" & Año_F & "" & Mes_f & "(" & Dia_f & ")"
                        Exit Function
                    End If
                Case 10
                    If Val(Dia_f) > 31 Then
                        Verifi_campo_fecha_Form6 = "ED_" & Año_F & "" & Mes_f & "(" & Dia_f & ")"
                        Exit Function
                    End If
                Case 11
                    If Val(Dia_f) > 30 Then
                        Verifi_campo_fecha_Form6 = "ED_" & Año_F & "" & Mes_f & "(" & Dia_f & ")"
                        Exit Function
                    End If
                Case 12
                    If Val(Dia_f) > 31 Then
                        Verifi_campo_fecha_Form6 = "ED_" & Año_F & "" & Mes_f & "(" & Dia_f & ")"
                        Exit Function
                    End If
            End Select
            If Numero_Caracter_fecha = 8 Then
                Verifi_campo_fecha_Form6 = Año_F & "-" & Mes_f & "-" & Dia_f & "_"
                Exit Function
            End If
        Catch ex As Exception
            Verifi_campo_fecha_Form6 = ex.ToString
        End Try
    End Function
End Class
