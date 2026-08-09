Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.OleDb
Imports System.Data.Odbc
Imports MySql.Data.MySqlClient
Imports System.IO
Imports System.IO.IsolatedStorage
Imports System.Xml
Public Structure Datos_Almacenamiento
    Dim nombre_campo As String
    Dim valor_campo As String
End Structure
Public Class ClassAñadirDocumento
    Function upload_inicializa_contador_visor(ByRef MatriDocRef() As String,
                                              ByRef cotntador As String) As String
        Try
            Dim Resultado As String = ""
            Dim sel As String = HttpContext.Current.Session("WF_TAGSELECCION")
            Dim Result As String = ""
            Dim Tag_Seleccion() As String
            Erase Tag_Seleccion
            Dim Matri_Img_Temp() As String
            Erase Matri_Img_Temp
            Tag_Seleccion = sel.Split("|")
            If Tag_Seleccion Is Nothing Then
                upload_inicializa_contador_visor = "Imagen sin datos adjuntos"
                Exit Function
            End If
            Dim Matri_dat_gabi() As String
            Erase Matri_dat_gabi
            Dim ClassDaGabinete As New ClassDaGabinete
            If Tag_Seleccion(3) = ".TIF" Or Tag_Seleccion(3) = ".JPG" Or Tag_Seleccion(3) = ".BMP" Then
                Resultado = ClassDaGabinete.Solicita_matriz_documentos_almacenados_gabinete(Tag_Seleccion(2),
                                                                                            Tag_Seleccion(5),
                                                                                            Matri_Img_Temp)
                If Resultado <> "YES" Then
                    upload_inicializa_contador_visor = Resultado
                    Exit Function
                End If
                If Matri_Img_Temp Is Nothing Then
                    upload_inicializa_contador_visor = "Imposible encontrar matris documentos"
                    Exit Function
                End If
                Erase MatriDocRef
                For i2 As Integer = 0 To UBound(Matri_Img_Temp)
                    ReDim Preserve MatriDocRef(i2)
                    MatriDocRef(i2) = Matri_Img_Temp(i2)
                Next
            End If
            HttpContext.Current.Session("WF_MATRI_IMAGE") = ""
            For i As Integer = 0 To MatriDocRef.Length - 1
                HttpContext.Current.Session("WF_MATRI_IMAGE") = HttpContext.Current.Session("WF_MATRI_IMAGE") & MatriDocRef(i) & "|"
            Next
            cotntador = HttpContext.Current.Session.Item("WF_DOC_ACTUAL") & "/" & MatriDocRef.Length - 1
            upload_inicializa_contador_visor = "YES"
        Catch ex As Exception
            upload_inicializa_contador_visor = "Inconsistencia general funcion upload_inicializa_contador_visor " & ex.Message
        End Try
    End Function
    Function Actualiza_Interface_Documento_añadido(ByRef MatriDocRef() As String,
                                                   ByRef Pag As Page) As String
        Try
            Dim Resultado As String = ""
            Dim ite As New Object
            Dim sel As String = HttpContext.Current.Session("WF_TAGSELECCION")
            Dim Result As String = ""
            Dim Tag_Seleccion() As String
            Erase Tag_Seleccion
            Dim Matri_Img_Temp() As String
            Erase Matri_Img_Temp
            Tag_Seleccion = sel.Split("|")
            If Tag_Seleccion Is Nothing Then
                Actualiza_Interface_Documento_añadido = "Imagen sin datos adjuntos"
                Exit Function
            End If
            Dim Matri_dat_gabi() As String
            Erase Matri_dat_gabi
            Dim ClassDaGabinete As New ClassDaGabinete
            If Tag_Seleccion(3) = ".TIF" Or Tag_Seleccion(3) = ".JPG" Or Tag_Seleccion(3) = ".BMP" Then
                Resultado = ClassDaGabinete.Solicita_matriz_documentos_almacenados_gabinete(Tag_Seleccion(2),
                                                                                            Tag_Seleccion(5),
                                                                                            Matri_Img_Temp)
                If Resultado <> "YES" Then
                    Actualiza_Interface_Documento_añadido = Resultado
                    Exit Function
                End If
                If Matri_Img_Temp Is Nothing Then
                    Actualiza_Interface_Documento_añadido = "Imposible encontrar matris documentos"
                    Exit Function
                End If
                Erase MatriDocRef
                For i2 As Integer = 0 To UBound(Matri_Img_Temp)
                    ReDim Preserve MatriDocRef(i2)
                    MatriDocRef(i2) = Matri_Img_Temp(i2)
                Next
            End If
            HttpContext.Current.Session("WF_MATRI_IMAGE") = ""
            For i As Integer = 0 To MatriDocRef.Length - 1
                HttpContext.Current.Session("WF_MATRI_IMAGE") = HttpContext.Current.Session("WF_MATRI_IMAGE") & MatriDocRef(i) & "|"
            Next
            Dim lab As TextBox = Pag.FindControl("LabelConteo")
            If Not lab Is Nothing Then
                lab.Text = HttpContext.Current.Session.Item("WF_DOC_ACTUAL") & "/" & MatriDocRef.Length - 1
            End If
            Dim up As UpdatePanel = Pag.FindControl("UpdatePanel_conte_bot")
            If Not up Is Nothing Then
                up.Update()
            End If
            Actualiza_Interface_Documento_añadido = "YES"
        Catch ex As Exception
            Actualiza_Interface_Documento_añadido = "Inconsistencia general funcion  Actualiza_Interface_Documento_añadido " & ex.Message
        End Try
    End Function
    Function Añade_documento_digitalizado(ByVal Id_Documento As Integer, _
                                          ByVal nombre_gabinete As String) As String
        Try
            Dim Result As String = ""
            Dim Refclas_digitalizacion As New ClassWorkflowDigitalizacion
            Dim Matri_Documentos_Final() As String
            Erase Matri_Documentos_Final
            Dim Selection As String = HttpContext.Current.Session.Item("SELECCIONTEMPORAL")
            Dim spl() As String = Selection.Split("|")
            Dim ruta_ As String = HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ESCANER")
            Result = Refclas_digitalizacion.SolicitaMatrizDocumentosDigitalizados(spl(0),
                                                                                  HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ESCANER"),
                                                                                  Matri_Documentos_Final)
            If Result <> "YES" Then
                Añade_documento_digitalizado = Result
                Exit Function
            End If
            Dim Numero_Paginas As Integer = 0
            Dim Tipo_Doc As Integer = 0
            Dim Valor_TipoDoc_Actualizar As Integer = 0
            Dim Ref_class_gabinete As New ClassDaGabinete
            Result = Ref_class_gabinete.Solicita_Datos_Gabinete(nombre_gabinete, _
                                                                Id_Documento, _
                                                                Numero_Paginas, _
                                                                Tipo_Doc)
            If Result <> "YES" Then
                Añade_documento_digitalizado = Result
                Exit Function
            End If
            Dim class_visualiza As New ClassVisualisaDocumento
            Dim matri_almacenados() As String = Nothing
            If Val(Tipo_Doc) = -2 Or Val(Tipo_Doc) = -20 Then
                Result = class_visualiza.Genera_Matris_Documentos_Almacenados(Id_Documento, _
                                                                              nombre_gabinete, _
                                                                              matri_almacenados)
                If Result <> "YES" Then
                    Añade_documento_digitalizado = Result
                    Exit Function
                End If
                Result = Me.Añadir_documento_pdf(Id_Documento, _
                                                 nombre_gabinete, _
                                                 matri_almacenados(1), _
                                                 Matri_Documentos_Final(0), _
                                                 ruta_)
                If Result <> "YES" Then
                    Añade_documento_digitalizado = "Imposible añadir documentos pdf " & Result
                    Exit Function
                End If
            End If
            If Val(Tipo_Doc) = -1 Or Val(Tipo_Doc) = -10 Then
                Result = Me.Añadir_Documentos_tif(Id_Documento, _
                                                  nombre_gabinete, _
                                                  Matri_Documentos_Final(0), _
                                                  Matri_Documentos_Final)
                If Result <> "YES" Then
                    Añade_documento_digitalizado = "Imposible añadir documentos tif " & Result
                    Exit Function
                End If
            End If
            Añade_documento_digitalizado = "YES"
        Catch ex As Exception
            Añade_documento_digitalizado = "Inconsistenncia general función Añade_documento_digitalizado " & ex.Message
        End Try
    End Function
    Function Añadir_documento_pdf(ByRef Id_Documento As Long, _
                                  ByVal Nombre_Gabinete As String, _
                                  ByVal ruta_archivo_pdf_fuente As String, _
                                  ByVal ruta_archivo_pdf_agregar As String, _
                                  ByVal ruta_temporal As String) As String
        Try
            Dim Valor_TipoDoc_Actualizar As Integer = -20
            Dim Numero_Paginas_documento As Integer = 0
            Dim Result As String = ""
            Dim Ref_class_reportes As New Class_ItexShare
            Result = Ref_class_reportes.UnirArchivoPdf(ruta_archivo_pdf_fuente,
                                                       ruta_archivo_pdf_agregar,
                                                       ruta_temporal,
                                                       Numero_Paginas_documento)
            If Result <> "YES" Then
                Añadir_documento_pdf = Result
                Exit Function
            End If
            Dim Actualiza_Datos_Gabinete As String = "Update " & Nombre_Gabinete & _
                    " set Dbt=" & Valor_TipoDoc_Actualizar & ",PAG=" & Numero_Paginas_documento & _
                    " where id=" & "'" & Id_Documento & "'"

            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Result = ref.SELECTION_INSERT_COMMAND(Actualiza_Datos_Gabinete)
            If Result <> "YES" Then
                Añadir_documento_pdf = Result
                Exit Function
            End If
            Añadir_documento_pdf = "YES"
            Exit Function
        Catch ex As Exception
            Añadir_documento_pdf = "Inconsistencia general función Añadir_documento_pdf " & ex.Message
        End Try
    End Function
    Function Añadir_Documentos_tif(ByRef Id_Documento As Long, _
                                   ByVal Nombre_Gabinete As String, _
                                   ByVal Documento_Adic As String, _
                                   ByVal Matri_Documentos() As String) As String
        '**********************************************
        'Funcion: Añadir_Documentos  
        'Fecha : 2010-10-07
        'Ing : Miguel Angel Urueta Miranda
        'Descripcion : Esta funcion añade un nuevo
        'documento a un registro almacenado en la base
        'de datos dependiendo del tipo documento para
        'esta primera version para los formatos tif
        'Modificacion 2013-08-09 para el modulo web
        'de workflow se realizan adaptaciones para
        'cumplir con el nuevo modelo de conexion
        '***********************************************
        '*********************************************
        'Consulta datos de la imagen en el gabienete
        '*********************************************
        Dim Numero_Paginas As Integer = 0
        Dim Tipo_Doc As Integer = 0
        Dim Valor_TipoDoc_Actualizar As Integer = 0
        Dim Result As String = ""
        Dim Ref_class_gabinete As New ClassDaGabinete
        Result = Ref_class_gabinete.Solicita_Datos_Gabinete(Nombre_Gabinete, _
                                                            Id_Documento, _
                                                            Numero_Paginas, _
                                                            Tipo_Doc)
        If Result <> "YES" Then
            Añadir_Documentos_tif = Result
            Exit Function
        End If
        '************************************************
        'Verifica si el tipo de documento se puede añadir
        'un documento despues de almacenado solo se
        'cumple para los documetos tif los tipo mayores 
        'que menos uno
        '************************************************
        If Tipo_Doc = -1 Or Tipo_Doc = -10 Then
        Else
            Añadir_Documentos_tif = "Para este tipo de documento no esta permitido adjuntar "
            Exit Function
        End If
        '******************************************************
        'determina el tipo documento añadido
        '******************************************************
        Dim Extencion_Documento As String = ""
        Dim Numero_Documento As String = ""
        Dim Cantidad_Documentos As Integer = 0
        Dim Documento_Sin_Exte As String = ""
        Dim ArchivoFile As New FileInfo(Documento_Adic)
        'Dim Ruta_Carpeta As String = Rut_carpt
        Dim Nombre_Documento As String = ArchivoFile.Name
        '**************************************************
        'Determina el tipo documento añadir
        '**************************************************
        If UCase(ArchivoFile.Extension) <> ".TIF" Then
            Añadir_Documentos_tif = "El tipo de archivo " & ArchivoFile.Extension & " no puede ser añadido"
            Exit Function
        End If
        '**************************************************
        'Retorna las partes de los documentos
        '**************************************************
        Result = Retorna_Numero_Documento(Nombre_Documento, _
                                          Numero_Documento, _
                                          Extencion_Documento, _
                                          Documento_Sin_Exte)
        If Result <> "YES" Then
            Añadir_Documentos_tif = Result
            Exit Function
        End If
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_DA
        ref.Returna_Conexion_Mysql(myConnection)
        Cantidad_Documentos = Matri_Documentos.Length
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Dim myTrans As MySqlTransaction
        myTrans = myConnection.BeginTransaction()
        myCommand.Connection = myConnection
        myCommand.Transaction = myTrans
        '**********************************
        'Determina el valor a actualizar
        '**********************************
        'Tipo tif
        If Val(Tipo_Doc) = -1 Or Val(Tipo_Doc) = -10 Then
            Valor_TipoDoc_Actualizar = -10
        End If
        'Tipo pdf
        If Val(Tipo_Doc) = -2 Or Val(Tipo_Doc) = -20 Then
            Valor_TipoDoc_Actualizar = -20
        End If
        'Tipo jpg
        If Val(Tipo_Doc) = -3 Or Val(Tipo_Doc) = -30 Then
            Valor_TipoDoc_Actualizar = -30
        End If
        'Tipo bmp
        If Val(Tipo_Doc) = -4 Or Val(Tipo_Doc) = -40 Then
            Valor_TipoDoc_Actualizar = -40
        End If
        Dim Actualiza_Datos_Gabinete As String = "Update " & Nombre_Gabinete & _
                " set Dbt=" & Valor_TipoDoc_Actualizar & " where id=" & "'" & Id_Documento & "'"
        myCommand.CommandText = Actualiza_Datos_Gabinete
        Try
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            '*********************************
            'Determina si se actualizo
            'el el estado del documento
            'en la base de datos
            '*********************************
            If Switc = 0 Then
                Añadir_Documentos_tif = "Imposible actualizar la tabla del gabinete  : " & Actualiza_Datos_Gabinete
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '*************************************
            'Genera datos campo documento añadido
            '*************************************
            Dim Matri_Datos_Almacen() As String
            Erase Matri_Datos_Almacen
            Result = ""
            Result = Obtiene_Valores_Campos_Documentos_Añadidos(Matri_Datos_Almacen, _
                                                                Nombre_Gabinete)
            If Result <> "YES" Then
                Añadir_Documentos_tif = "Imposibe contar campos de la tabla " & Result
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            If Matri_Datos_Almacen Is Nothing Then
                Añadir_Documentos_tif = "La matriz de datos de documentos añadisos es nothing "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '************************************************
            'Almacena el nuevo documento añadido
            '************************************************
            Result = ""
            Dim Reflcaslamcen As New ClassAlmacenamiento
            Dim id_img As Integer = 0
            Result = Reflcaslamcen.Almacenamiento_simple("", Nombre_Documento, _
                                                    Nombre_Gabinete, _
                                                    0, _
                                                    Matri_Datos_Almacen, _
                                                    2, _
                                                    Cantidad_Documentos, _
                                                    Id_Documento, _
                                                    Matri_Documentos, _
                                                    0, _
                                                    id_img _
                                                    , Valor_TipoDoc_Actualizar, _
                                                    "WF-" & HttpContext.Current.Session.Item("Login_Usuario_Workfow"))


            If Result <> "YES" Then
                myTrans.Rollback()
                myConnection.Close()
                Añadir_Documentos_tif = Result
                Exit Function
            Else
                myTrans.Commit()
                myConnection.Close()
                For i As Integer = 0 To Matri_Documentos.Length - 1
                    Kill(Matri_Documentos(i))
                Next
                Añadir_Documentos_tif = "YES"
                Exit Function
            End If
            Añadir_Documentos_tif = "YES"
            Exit Function
        Catch e As Exception
            Try
                myTrans.Rollback()
                myConnection.Close()
                Añadir_Documentos_tif = "Error Actualizando  " & e.Message

            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Añadir_Documentos_tif = "An exception of type " & ex.GetType().ToString() & _
                                      " was encountered while attempting to roll back the transaction."
                    myConnection.Close()
                    Exit Function
                End If
            End Try

        End Try

    End Function
    Function Obtiene_Valores_Campos_Documentos_a_Duplicar(ByRef Matri_Datos_Almacen() As String, _
                                                          ByVal Nombre_Tabla As String, _
                                                          ByVal id_image As Integer) As String
        '-------------------------------------------------------------
        'Funcion : Retorna la matriz de los campos del un gabinete
        'especifico
        'Ing : Miguel Angel Urueta Miranda
        'Fecha : 2016-01-26
        '--------------------------------------------------------
        Try
            Dim Matri_campos() As String
            Erase Matri_campos
            Dim Result As String = ""
            Result = Me.Obtiene_Valores_Campos_Documentos_Añadidos_NOMBRE(Matri_campos, Nombre_Tabla)
            If Result <> "YES" Then
                Obtiene_Valores_Campos_Documentos_a_Duplicar = Result
                Exit Function
            End If
            If Matri_campos Is Nothing Then
                Obtiene_Valores_Campos_Documentos_a_Duplicar = "Imposible encontrar campos para gabinete " & Result
                Exit Function
            End If
            Dim select_campos As String = " "
            For k As Integer = 0 To Matri_campos.Length - 1
                If k = 0 Then
                    select_campos = Matri_campos(k)
                Else
                    select_campos = select_campos & "," & Matri_campos(k)
                End If
            Next
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Parametro_Consulta As String = "SELECT " & select_campos & " FROM " & Nombre_Tabla & " WHERE id" & _
               "=" & id_image
            Dim Datset As DataSet = New DataSet("DETALLE_GABIENETE")
            Result = ref.SELECTION_SELECT_FIELDA(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Obtiene_Valores_Campos_Documentos_a_Duplicar = "Imposible conectar a la base de datos funcion  Obtiene_Valores_Campos_Documentos_a_Duplicar " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                For z As Integer = 0 To Matri_campos.Length - 1
                    'Dim columnOrdinal As Integer = Datset.Tables(0).Rows(0).Item(Matri_campos(z))
                    ReDim Preserve Matri_Datos_Almacen(z)
                    Dim red = Datset.Tables(0).Rows(0).Item(z).GetType
                    If Datset.Tables(0).Rows(0).IsNull(z) = True Then
                        Matri_Datos_Almacen(z) = ""
                    Else
                        If red.FullName <> "System.DateTime" Then
                            Matri_Datos_Almacen(z) = Datset.Tables(0).Rows(0).Item(Matri_campos(z)).ToString.Replace("'", "")
                        Else
                            Dim SplitWf() As String = Left(Datset.Tables(0).Rows(0).Item(Matri_campos(z)).ToString, 10).Split("/")
                            If Not SplitWf Is Nothing Then
                                Matri_Datos_Almacen(z) = SplitWf(2) & "/" & SplitWf(1) & "/" & SplitWf(0)
                            Else
                                Matri_Datos_Almacen(z) = ""
                            End If

                        End If
                    End If
                Next
                Obtiene_Valores_Campos_Documentos_a_Duplicar = "YES"
                Exit Function
            Else
                Obtiene_Valores_Campos_Documentos_a_Duplicar = "funcion  Obtiene_Valores_Campos_Documentos_a_Duplicar dice : obtener matriz de campos del gabinete  " & Nombre_Tabla
                Exit Function
            End If

        Catch ex As Exception
            Obtiene_Valores_Campos_Documentos_a_Duplicar = "Error General Funcion Obtiene_Valores_Campos_Documentos_Añadidos Error :" & ex.Message
        End Try
    End Function
    Function Obtiene_Valores_Campos_Documentos_Añadidos_NOMBRE(ByRef Matri_Datos_Almacen() As String, _
                                                               ByVal Nombre_Tabla As String) As String
        '----------------------------------------------------------
        'Funcion Retorna la estructura de los campos de un gabinete
        'Fecha : 2017-01-26 Modificada para web
        'Ing : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Sql_consulta = "SELECT CAMPO FROM DETALLE_GABIENETE WHERE GABINETE" & _
                "='" & Nombre_Tabla & "' AND VISIBLE=1 order by IDENTI"
            Dim Datset As DataSet = New DataSet("DETALLE_GABIENETE")
            Dim result As String = ref.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If result <> "YES" Then
                Obtiene_Valores_Campos_Documentos_Añadidos_NOMBRE = "Imposible conectar a la base de datos funcion  Obtiene_Valores_Campos_Documentos_Añadidos_NOMBRE " & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                Erase Matri_Datos_Almacen
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve Matri_Datos_Almacen(i)
                    Matri_Datos_Almacen(i) = Datset.Tables(0).Rows(i).Item(0).ToString
                Next
                Obtiene_Valores_Campos_Documentos_Añadidos_NOMBRE = "YES"
                Exit Function
            Else
                Obtiene_Valores_Campos_Documentos_Añadidos_NOMBRE = "Función Obtiene_Valores_Campos_Documentos_Añadidos_NOMBRE dice : Imposible Encontrar numero de campos  Consulta "
                Exit Function
            End If
        Catch ex As Exception
            Obtiene_Valores_Campos_Documentos_Añadidos_NOMBRE = "Error General Funcion Obtiene_Valores_Campos_Documentos_Añadidos Error :" & ex.Message
        End Try
    End Function
    

    Public Function Retorna_Numero_Documento(ByVal Nombre_Documento As String, _
                                             ByRef Numero_Documento As String, _
                                             ByRef Ext_Documento As String, _
                                             ByRef Documento_Sin_Exte As String) As String
        '********************************************************
        'Funcion : Retorna numero documento
        'Fecha : 2013-08-09
        'Ing miguel Angel urueta miranda
        '********************************************************
        Try
            Dim Matri_Puntos() As String
            Erase Matri_Puntos
            Matri_Puntos = Split(Nombre_Documento, ".")
            If Matri_Puntos Is Nothing Then
                Retorna_Numero_Documento = "Estructura incorecta de archivo " & Nombre_Documento
                Exit Function
            End If
            Ext_Documento = Matri_Puntos(UBound(Matri_Puntos))
            Numero_Documento = (Matri_Puntos(LBound(Matri_Puntos)))
            Documento_Sin_Exte = (Matri_Puntos(LBound(Matri_Puntos)))
            Numero_Documento = Replace(Numero_Documento, "DIG", "")
            Numero_Documento = Val(Numero_Documento)
            Retorna_Numero_Documento = "YES"
        Catch ex As Exception
            Retorna_Numero_Documento = ex.Message
        End Try
    End Function
   
    
    Public Function Obtiene_Valores_Campos_Documentos_Añadidos(ByRef Matri_Datos_Almacen() As String, _
                                                               ByVal Nombre_Tabla As String) As String
        '******************************************************************
        'Funcion : Obtiene valores vacios de campo para añadir
        'ingeniero Miguel Angel Urueta Miranda
        'Fecha : 2013-08-09
        'Funcion extraida del workflow cliente y modificada para el modulo
        'web
        '*******************************************************************
        Try
            Dim Numero_Colum As Integer = 0
            Dim Parametro_Consulta As String = "SELECT * FROM DETALLE_GABIENETE WHERE GABINETE" & _
                "='" & Nombre_Tabla & "' AND VISIBLE=1 ORDER BY IDENTI"
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("DETALLE_GABIENETE")
            Dim result As String = ref.SELECTION_SELECT_FIELDA(Parametro_Consulta, Datset)
            If result <> "YES" Then
                Obtiene_Valores_Campos_Documentos_Añadidos = "Error Consultando en tabla 33 " & " " & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Obtiene_Valores_Campos_Documentos_Añadidos = "Imposible Encontrar numero de campos  Consulta " & Parametro_Consulta
                Exit Function
            Else
                Erase Matri_Datos_Almacen
                Dim icont As Integer = 0
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve Matri_Datos_Almacen(icont)
                    Matri_Datos_Almacen(icont) = ""
                    icont = icont + 1
                Next
            End If
            Obtiene_Valores_Campos_Documentos_Añadidos = "YES"
        Catch ex As Exception
            Obtiene_Valores_Campos_Documentos_Añadidos = "Error General Funcion Obtiene_Valores_Campos_Documentos_Añadidos Error :" & ex.ToString
        End Try
    End Function
    Function Obtener_Datos_Gabinete_conexion(ByRef Matri_Datos_Gabinete() As String)
        '------------------------------------------------------
        'Función : Reotrna matri datos conexión del gabinete
        'Ing : Miguel Angel Urueta Miranda
        'Fecha : 2017-0-27 Modificado para web
        '------------------------------------------------------
        Try
            Dim ref As New ClassListandoTareas
            Dim NombreRutak As String = ""
            '--------------------------------------------
            'Solicita el nombre de la ruta
            '--------------------------------------------
            Dim Result As String = ""
            Dim I2 As Integer = 0
            Dim Ref_calss_wf_ruta As New Class_worflow_rutas
            Result = Ref_calss_wf_ruta.Solicita_nombre_ruta_workflow(HttpContext.Current.Session.Item("Id_Ruta_Workflow").ToString, _
                                                                     NombreRutak)
            If Result <> "YES" Then
                Obtener_Datos_Gabinete_conexion = "Función Obtener_Datos_Gabinete_conexion Error listando Ruta " + Result
                Exit Function
            End If
            If NombreRutak = "" Then
                Obtener_Datos_Gabinete_conexion = "Función Obtener_Datos_Gabinete_conexion Imposible econtrar nombre de la ruta " + Result
                Exit Function
            End If
            '------------------------------------------
            'Consulta los datos del gabiente
            '------------------------------------------
            'Matri_Datos_Gabinete(0)=NOMBRE_GABINETE
            'Matri_Datos_Gabinete(1)=RUTA_FISICA_GABINETE
            'Matri_Datos_Gabinete(2)=RUTA_ALMACENA_IMAGEN
            'Matri_Datos_Gabinete(3)=RUTA_BUSQUEDA_IMAGEN
            'Matri_Datos_Gabinete(4)=BASE_DATOS
            'Matri_Datos_Gabinete(5)=MOTOR_BASE
            'Matri_Datos_Gabinete(6)=ODBC_BASE
            'Matri_Datos_Gabinete(7)=USUARIO_BASE
            'Matri_Datos_Gabinete(8)=PASWORD_BASE
            Erase Matri_Datos_Gabinete
            Dim Sql_consulta As String = "SELECT CG.NOMBRE_GABINETE,CG.RUTA_FISICA_GABINETE," & _
                    "CG.RUTA_ALMACENA_IMAGEN,CG.RUTA_BUSQUEDA_IMAGEN" & _
                    ",CG.BASE_DATOS," & _
                    "CG.MOTOR_BASE,CG.ODBC_BASE,CG.USUARIO_BASE," & _
                    "CG.PASWORD_BASE " & _
                    "FROM DAT_ADIC_TAR" & NombreRutak & " AS DAT " & _
                    "INNER JOIN CONFIGURACION_GABINETE AS CG " & _
                    "ON (DAT.ID_GABINETE=CG.ID_GABINETE) " & _
                    "WHERE INICIO_TAREAS_WORKFLOW_ID_TAREA=" & HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")
            Dim ref_conect As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("DETALLE_GABIENETE")
            Result = ref_conect.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Obtener_Datos_Gabinete_conexion = "Error Función Obtener_Datos_Gabinete_conexion Consultando en tabla  " & " " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then

                Obtener_Datos_Gabinete_conexion = "Imposible encontrar datos de Configuracion para gabinete"
                Exit Function
            Else

                For I2 = 0 To Datset.Tables(0).Columns.Count - 1
                    ReDim Preserve Matri_Datos_Gabinete(I2)
                    If Datset.Tables(0).Rows(0).IsNull(I2) = False Then
                        Matri_Datos_Gabinete(I2) = Datset.Tables(0).Rows(0).Item(I2).ToString
                    Else
                        Matri_Datos_Gabinete(I2) = ""
                    End If
                Next
            End If
            If Matri_Datos_Gabinete Is Nothing Then
                Obtener_Datos_Gabinete_conexion = "Matris datos de Docuemntos sin datos"
                Exit Function
            End If
            Obtener_Datos_Gabinete_conexion = "YES"
        Catch ex As Exception
            Obtener_Datos_Gabinete_conexion = "Error General Funcion Obtener_Datos_Gabinete_conecion Cod Error " & ex.ToString
        End Try
    End Function
    Function Elimina_Documentos_Adjuntos(ByVal id_imagen_documento As Integer) As String

        Dim Resultado As String = ""
        Dim ite As New Object
        Dim sel As String = ""
        Dim Result As String = ""
        Dim Matri_Img_Temp() As String
        Dim Nombre_Gab As String = ""
        Erase Matri_Img_Temp
        Try

            Dim Refclas As New ClassVisualisaDocumento
            Dim Matri_Documentos_wf() As String
            Erase Matri_Documentos_wf
            Dim Reclaswf As New ClassWorkflow
            Dim ID_TAR As Integer = Val(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"))
            Dim Id_imagen As Integer = 0
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = Class_DAT_ADIC_TAR.SolicitaNombreGabineteImagenTareaWorkflow(ID_TAR,
                                                                 Nombre_Gab,
                                                                 Id_imagen)
            If Result <> "YES" Then
                Elimina_Documentos_Adjuntos = "Error consultado  nombre gabinete " & Result
                Exit Function
            End If
            Dim Matri_Datos_Gabinete_Connex_Wf() As String
            Erase Matri_Datos_Gabinete_Connex_Wf
            Dim Refclasañadir As New ClassAñadirDocumento
            Result = ""
            Result = Refclasañadir.Obtener_Datos_Gabinete_conexion(Matri_Datos_Gabinete_Connex_Wf)
            If Result <> "YES" Then
                Elimina_Documentos_Adjuntos = Result
                Exit Function
            End If
            If Matri_Datos_Gabinete_Connex_Wf Is Nothing Then
                Elimina_Documentos_Adjuntos = Result
                Exit Function
            End If
            Dim Persistencia As String = "False"
            Dim Refclasvisual As New ClassVisualisaDocumento
            Dim Matridat_Gabi() As Datos_Registro
            Erase Matridat_Gabi
            Dim Usuarioadjunto As String = "WF-" & UCase(HttpContext.Current.Session.Item("Login_Usuario_Workfow"))
            Result = Refclasvisual.Consulta_Documentos_Añadidos(id_imagen_documento,
                                                                Nombre_Gab,
                                                                Matridat_Gabi,
                                                                Usuarioadjunto)
            If Result <> "YES" Then
                Elimina_Documentos_Adjuntos = Result
                Exit Function
            End If
            If Matridat_Gabi Is Nothing Then
                Elimina_Documentos_Adjuntos = "No se encontraron imagenes adjuntas para eliminar "
                Exit Function
            End If
            '*************************************
            'Elimina documentos añadidos de la 
            'base de datos
            '*************************************
            Dim Sql_consulta As String = ""
            Sql_consulta = "Delete from " & Nombre_Gab & " where id =" & Matridat_Gabi(0).Id
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("DETALLE_GABIENETE")
            Dim Resultado_Eliinar As String = ref.SELECTION_INSERT_COMMAND(Sql_consulta)
            If Result <> "YES" Then
                Elimina_Documentos_Adjuntos = "Error Consultando Función Eliminando_Documentos " & Result
                Exit Function
            End If
            If Resultado_Eliinar <> "YES" Then
                Elimina_Documentos_Adjuntos = "Error eliminando páginas " & Resultado_Eliinar
                Exit Function
            Else
                Elimina_Documentos_Adjuntos = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Elimina_Documentos_Adjuntos = "Error General Funcion Eliminando_Documentos Error " & ex.ToString
        End Try

    End Function
End Class
