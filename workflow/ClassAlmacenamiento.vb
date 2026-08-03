Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.OleDb
Imports System.Data.Odbc
Imports MySql.Data.MySqlClient
Imports System.IO
Imports System.IO.IsolatedStorage
Imports System.Xml
Imports iTextSharp.text
Imports iTextSharp.text.pdf

Public Structure estructure_gestion
    Dim ID_AREA As Integer
    Dim ID_SERIE As Integer
    Dim ID_SUB_SERIE As Integer
    Dim ID_TIPODOCUMENTO As Integer
    Dim ID_USUARIO_GESTION As Integer
    Dim USUARIO_GESTION As String
    Dim EXPEDIENTE As String
    Dim ID_EXPEDIENTE As Integer
    Dim ID_TIPO_EXPEDIENTE As Integer
    Dim UNIDAD_CONSERVACION As String
    Dim ID_UNIDAD_CONSERVACION As Integer
    Dim ID_TIPO_UNIDAD_CONSERVACION As Integer
    Dim TIPO_UNIDAD_DOCUMENTAL As Integer
    Dim CLASE_DOCUMENTO As String
    Dim ID_CLASE_DOCUMENTO As Integer
    Dim FECHA_ELABORACION As String
    Dim NOMBRE_SERIE As String
    Dim NOMBRE_SUB_SERIE As String
    Dim TIPODOCUMENTO As String
    Dim ASUNTO As String
    Dim TEMA As String
    Dim INDICE_GESTION As String
End Structure
Public Structure estructura_gabinete
    Dim IDENTI As Integer
    Dim TIPO As String
    Dim CAMPO As String
    Dim VISIBLE As Integer
    Dim SISTEMA As Integer
    Dim ESTADO As Integer
    Dim INFOCAMPO As String
    Dim CAMPOPUBLICO As Integer
    Dim CAMPOUNICO As Integer
    Dim VALORCAMPO As String
    Dim CAMPO_RADICADO As Integer
    Dim ALEAS_CAMPO As String
    Dim CAMPO_ENABLE_DISABLE As Integer
End Structure
Public Structure stru_datos_image_lista
    Dim nombre_gabinete As String
    Dim id_imagen As Integer
    Dim DBT As Object
    Dim radicado As String
    Dim tipodocumental As String
    Dim notipodocumento As String
    Dim extension As String
    Dim id_tarea_workflow As Long
    Dim estado_firma_digital As Integer
    Dim icono_icono_awe_some As String
    Dim fecha As String
    Dim aleas As String
    Dim id_registro As Long
    Dim nombre_archivo As String
End Structure
Public Class class_stru_datos_image_lista
    Property nombre_gabinete As String
    Property id_imagen As Integer
    Property DBT As Object
    Property radicado As String
    Property tipodocumental As String
    Property notipodocumento As String
    Property extension As String
    Property id_tarea_workflow As Long
    Property estado_firma_digital As Integer
    Property icono_icono_awe_some As String
    Property fecha As String
    Property aleas As String
    Property id_registro As Long
    Property nombre_archivo As String
    Property error_sistema As String
End Class
Public Class CDcamposAsignaAlmacenamiento
    Property NombreCampoGabinete As String
    Property ValorCampoGabinete As String
End Class
Public Class ClassAlmacenamiento
    Function Almacenamiento_documentos_adjuntos_digitalizados_rad_simplificada(ByVal DG_tipo_digitalizacion As String,
                                                                               ByVal id_tarea_workflow As Long,
                                                                               ByVal gabienete As String,
                                                                               ByVal radicado As String,
                                                                               ByRef class_stru_datos_image_lista As class_stru_datos_image_lista) As String
        Try
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim Result As String = ""
            Dim evalua_flujo_ruta As Integer = 0
            If id_tarea_workflow <> 0 Then
                evalua_flujo_ruta = 1
            End If
            Dim ID_DOCUMENTO As Integer = 0
            Dim TIPO_DOCUMENTO As Integer = 0
            Dim stru_datos_image_lista As stru_datos_image_lista = Nothing
            Result = ClassDaGabinete.Almacenamiento_documentos_adjuntos_digitalizados_modulo_radicado("",
                                                                                                     ID_DOCUMENTO,
                                                                                                     TIPO_DOCUMENTO,
                                                                                                     stru_datos_image_lista,
                                                                                                     DG_tipo_digitalizacion,
                                                                                                     id_tarea_workflow,
                                                                                                     gabienete,
                                                                                                     radicado,
                                                                                                     evalua_flujo_ruta,
                                                                                                     0)
            If Result <> "YES" Then
                Almacenamiento_documentos_adjuntos_digitalizados_rad_simplificada = Result
                Exit Function
            Else
                class_stru_datos_image_lista.id_imagen = stru_datos_image_lista.id_imagen
                class_stru_datos_image_lista.icono_icono_awe_some = stru_datos_image_lista.icono_icono_awe_some
                class_stru_datos_image_lista.estado_firma_digital = stru_datos_image_lista.estado_firma_digital
                class_stru_datos_image_lista.notipodocumento = stru_datos_image_lista.notipodocumento
                class_stru_datos_image_lista.DBT = stru_datos_image_lista.DBT
                Almacenamiento_documentos_adjuntos_digitalizados_rad_simplificada = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Almacenamiento_documentos_adjuntos_digitalizados_rad_simplificada = "Inconsistencia general funcion  Almacenamiento_documentos_adjuntos_digitalizados_rad_simplificada " & ex.Message
            Exit Function
        End Try
    End Function

    Function Retorna_estructura_campos_gabinete_visible(ByVal nombre_gabinete As String, _
                                                        ByRef estructura_gabinete() As estructura_gabinete) As String
        Try
            Dim Parametro_Consulta = "SELECT IDENTI,TIPO,CAMPO,VISIBLE,SISTEMA,ESTADO,INFOCAMPO,CAMPO_PUBLICO,CAMPO_UNICO FROM " & _
                            "DETALLE_GABIENETE " & _
                            "WHERE GABINETE='" & nombre_gabinete & "' AND VISIBLE=1  ORDER BY IDENTI"
            Dim Result As String = ""
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_relacion_plantilla_radicado_gabinete")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_estructura_campos_gabinete_visible = "Función Retorna_estructura_campos_gabinete_visible dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_estructura_campos_gabinete_visible = "Imposible encontrar los campos para gabinete en la tabla gabinete detalle"
                Exit Function
            Else

                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve estructura_gabinete(i)
                    estructura_gabinete(i).IDENTI = Datset.Tables(0).Rows(i).Item(0)
                    estructura_gabinete(i).TIPO = Datset.Tables(0).Rows(i).Item(1)
                    estructura_gabinete(i).CAMPO = Datset.Tables(0).Rows(i).Item(2)
                    estructura_gabinete(i).VISIBLE = Datset.Tables(0).Rows(i).Item(3)
                    estructura_gabinete(i).SISTEMA = Datset.Tables(0).Rows(i).Item(4)
                    estructura_gabinete(i).ESTADO = Datset.Tables(0).Rows(i).Item(5)
                    estructura_gabinete(i).INFOCAMPO = Datset.Tables(0).Rows(i).Item(6)
                    estructura_gabinete(i).CAMPOPUBLICO = Datset.Tables(0).Rows(i).Item(7)
                    estructura_gabinete(i).CAMPOUNICO = Datset.Tables(0).Rows(i).Item(8)
                Next
                Retorna_estructura_campos_gabinete_visible = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Retorna_estructura_campos_gabinete_visible = "Inconsistencia general función Retorna_estructura_campos_gabinete_visible " & ex.Message
        End Try
    End Function
    Function Solicita_datos_unidad_conservacion_estructura_base_datos(ByRef matri_gestion As estructure_gestion,
                                                                      ByVal nombre_gabinete As String,
                                                                      ByVal id_imagen As Integer) As String
        '*********************************************************
        'Funcion : Asigna datos unidad de conservación de la 
        'base de datos a la estructura
        'Fecha : 2015-16-16
        'Ing : Miguel Angel Urueta Miranda
        '*********************************************************
        Try
            Dim refclastrd As New ClassTrdDocumental
            Dim Result As String = ""
            Dim Parametro_Consulta = "SELECT ID_UNIDAD_CONSERVACION,ID_TIPO_UNIDAD_CONSERVACION,UNIDADCONSERVA FROM " & nombre_gabinete &
                    " where ID=" & id_imagen
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet(nombre_gabinete)
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_datos_unidad_conservacion_estructura_base_datos = "Funcion Asigna_datos_unidad_conservacion_estructura_base_datos dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_datos_unidad_conservacion_estructura_base_datos = "Imposible encontrar el id " & id_imagen & " de la imagen en la tabla " & nombre_gabinete
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = False Then
                    matri_gestion.ID_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(0).Item(0)
                Else
                    matri_gestion.ID_UNIDAD_CONSERVACION = 0
                End If
                If Datset.Tables(0).Rows(0).IsNull(1) = False Then
                    matri_gestion.ID_TIPO_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(0).Item(1)
                Else
                    matri_gestion.ID_TIPO_UNIDAD_CONSERVACION = 0
                End If
                If Datset.Tables(0).Rows(0).IsNull(2) = False Then
                    matri_gestion.UNIDAD_CONSERVACION = Datset.Tables(0).Rows(0).Item(2)
                Else
                    matri_gestion.UNIDAD_CONSERVACION = ""
                End If
                Solicita_datos_unidad_conservacion_estructura_base_datos = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_datos_unidad_conservacion_estructura_base_datos = "Inconsistencia función Asigna_datos_unidad_conservacion_estructura_base_datos " & ex.Message
        End Try
    End Function
    Function Solicita_datos_tipo_documental_estructura_base_datos(ByRef matri_gestion As estructure_gestion, _
                                                                  ByVal nombre_gabinete As String, _
                                                                  ByVal id_imagen As Integer) As String
        '*********************************************************
        'Funcion : Asigna datos tipo documental documental de la 
        'la base de datos
        'Fecha : 2015-02-13
        'Ing : Miguel Angel Urueta Miranda
        '*********************************************************
        Try
            Dim refclastrd As New ClassTrdDocumental
            Dim Result As String = ""
            Dim Parametro_Consulta = "SELECT FECHAELABORACION,CLASEDOCUMENTO,ID_CLASE_DOCUMENTO FROM " & nombre_gabinete & _
              " where ID=" & id_imagen
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet(nombre_gabinete)
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_datos_tipo_documental_estructura_base_datos = "Funcion Asigna_datos_tipo_documental_estructura_base_datos dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_datos_tipo_documental_estructura_base_datos = "Imposible encontrar el id " & id_imagen & " de la imagen en la tabla " & nombre_gabinete
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = False Then
                    Dim SplitWf() As String = Left(Datset.Tables(0).Rows(0).Item(0).ToString, 10).Split("/")
                    If Not SplitWf Is Nothing Then
                        matri_gestion.FECHA_ELABORACION = SplitWf(2) & "/" & SplitWf(1) & "/" & SplitWf(0)
                    Else
                        matri_gestion.FECHA_ELABORACION = "#Error"
                    End If
                Else
                    matri_gestion.FECHA_ELABORACION = ""
                End If
                If Datset.Tables(0).Rows(0).IsNull(1) = False Then
                    matri_gestion.CLASE_DOCUMENTO = Datset.Tables(0).Rows(0).Item(1)
                Else
                    matri_gestion.CLASE_DOCUMENTO = ""
                End If
                If Datset.Tables(0).Rows(0).IsNull(2) = False Then
                    matri_gestion.ID_CLASE_DOCUMENTO = Datset.Tables(0).Rows(0).Item(2)
                Else
                    matri_gestion.ID_CLASE_DOCUMENTO = 0
                End If
                Solicita_datos_tipo_documental_estructura_base_datos = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Solicita_datos_tipo_documental_estructura_base_datos = "Inconsistencia función Asigna_datos_tipo_documental_estructura_base_datos " & ex.Message
        End Try
    End Function
    Function Solicita_datos_gestion_estructura_base_datos(ByRef matri_gestion As estructure_gestion, _
                                                        ByVal nombre_gabinete As String, _
                                                        ByVal id_imagen As Integer) As String
        '***************************************************
        'Funcion : Asigna datos gestion documental de la 
        'base de la tabla gabinete a la estructura
        'Fecha : 2015-12-13
        'Ing : Miguel Angel Urueta Miranda
        '***************************************************
        Try
            Dim refclastrd As New ClassTrdDocumental
            Dim Result As String = ""
            'Dim opt_tabla_retencion As Integer = 0
            'Result = refclastrd.Verifica_opcion_aplicar_tabla_retencion(opt_tabla_retencion, nombre_gabinete)
            'If Result <> "YES" Then
            '    Asigna_datos_gestion_estructura_base_datos = Result
            '    Exit Function
            'End If
            Dim Parametro_Consulta = "SELECT ID_AREA,ID_SERIE,ID_SUB_SERIE,ID_TIPODOCUMENTO,NOMBRESERIE,NOMBRESUBSERIE,TIPODOCUMENTO FROM " & nombre_gabinete & _
            " where ID=" & id_imagen
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet(nombre_gabinete)
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_datos_gestion_estructura_base_datos = "Funcion Asigna_datos_gestion_estructura_base_datos dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_datos_gestion_estructura_base_datos = "Imposible encontrar el id " & id_imagen & " de la imagen en la tabla " & nombre_gabinete
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = False Then
                    matri_gestion.ID_AREA = Datset.Tables(0).Rows(0).Item(0)
                Else
                    matri_gestion.ID_AREA = 0
                End If
                If Datset.Tables(0).Rows(0).IsNull(1) = False Then
                    matri_gestion.ID_SERIE = Datset.Tables(0).Rows(0).Item(1)
                Else
                    matri_gestion.ID_SERIE = 0
                End If
                If Datset.Tables(0).Rows(0).IsNull(2) = False Then
                    matri_gestion.ID_SUB_SERIE = Datset.Tables(0).Rows(0).Item(2)
                Else
                    matri_gestion.ID_SUB_SERIE = 0
                End If
                If Datset.Tables(0).Rows(0).IsNull(3) = False Then
                    matri_gestion.ID_TIPODOCUMENTO = Datset.Tables(0).Rows(0).Item(3)
                Else
                    matri_gestion.ID_TIPODOCUMENTO = 0
                End If
                If Datset.Tables(0).Rows(0).IsNull(4) = False Then
                    matri_gestion.NOMBRE_SERIE = Datset.Tables(0).Rows(0).Item(4)
                Else
                    matri_gestion.NOMBRE_SERIE = ""
                End If
                If Datset.Tables(0).Rows(0).IsNull(5) = False Then
                    matri_gestion.NOMBRE_SUB_SERIE = Datset.Tables(0).Rows(0).Item(5)
                Else
                    matri_gestion.NOMBRE_SUB_SERIE = ""
                End If
                If Datset.Tables(0).Rows(0).IsNull(6) = False Then
                    matri_gestion.TIPODOCUMENTO = Datset.Tables(0).Rows(0).Item(6)
                Else
                    matri_gestion.TIPODOCUMENTO = ""
                End If
                Solicita_datos_gestion_estructura_base_datos = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Solicita_datos_gestion_estructura_base_datos = "Inconsistencia función Asigna_datos_gestion_estructura_base_datos " & ex.Message
        End Try
    End Function

    Function Leyendo_Archivo_Preindex_Xml_id_imagen(ByVal Xml As XmlNode,
     ByRef Id_imagen_id As Integer) As String
        Try
            Dim Valor_Matri As Integer = 0
            Dim Xml2 As XmlNodeList
            Id_imagen_id = -1
            Xml2 = Xml.SelectNodes("/Gabinetes/Gabinete")
            Dim xmlAttr As XmlNode
            For Each xmlAttr In Xml2
                With xmlAttr.Attributes
                    If .Count > 0 Then
                        If Not .GetNamedItem("ID_ALMACEN_LINK") Is Nothing Then
                            Id_imagen_id = .GetNamedItem("ID_ALMACEN_LINK").Value()
                        End If

                    End If
                End With
            Next
            Leyendo_Archivo_Preindex_Xml_id_imagen = "YES"
        Catch ex As Exception
            Leyendo_Archivo_Preindex_Xml_id_imagen = "Error General Funcion : Leyendo_Archivo_Preindex_Xml Error : " & ex.ToString
        End Try

    End Function
    Public Function Leyendo_Archivo_Preindex_Xml(ByVal Xml As XmlNode, _
    ByVal Matri_Campos() As String, ByRef Matri_Datos() As String, ByRef Id_imagen_id As Integer) As String
        Try
            Dim Valor_Matri As Integer = 0
            Dim Xml2 As XmlNodeList
            Xml2 = Xml.SelectNodes("/Gabinetes/Gabinete")
            Dim xmlAttr As XmlNode
            Dim Contador_Nodo As Integer = 0
            For Each xmlAttr In Xml2
                '= UBound(Matri_Reg_Carpeta) + 1
                With xmlAttr.Attributes
                    For z2 As Integer = 0 To .Count - 1
                        For iz As Integer = 0 To UBound(Matri_Campos)
                            Dim Matri_Campos_Temp() As String = Matri_Campos(iz).Split("|")
                            If Matri_Campos_Temp(0) = .Item(z2).Name Then
                                ReDim Preserve Matri_Datos(z2)
                                If .Item(z2).Value <> "null" Then
                                    Matri_Datos(iz) = .Item(z2).Value
                                Else
                                    Matri_Datos(iz) = ""
                                    Exit For
                                End If
                            End If

                        Next
                    Next
                End With
                Contador_Nodo = Contador_Nodo + 1
            Next
            Leyendo_Archivo_Preindex_Xml = "YES"
        Catch ex As Exception
            Leyendo_Archivo_Preindex_Xml = "Error General Funcion : Leyendo_Archivo_Preindex_Xml Error : " & ex.ToString
        End Try

    End Function

    Public Function Arma_Nombre_Archivo_Preindex(ByVal Docu_Selecion As String,
                                                 ByRef Doc_Xml As String) As String
        Try
            Dim Refile As New FileInfo(Docu_Selecion)
            Dim Nombre_Documento As String = Refile.Name.Replace(Refile.Extension, ".xml")
            Nombre_Documento = Nombre_Documento.Replace("DIG", "FXL")
            Doc_Xml = Nombre_Documento
            Arma_Nombre_Archivo_Preindex = "YES"
        Catch ex As Exception
            Arma_Nombre_Archivo_Preindex = "Error generando nombre archivo preindex " & ex.ToString
        End Try
    End Function

    Public Function Retorna_Id_imagen_Archivo(ByVal Nombre_Archivo As String, _
                                              ByRef Id_Link As Integer) As String
        '************************************************
        'Funcion : Debuelve el codigo de la imagen
        'almacenada que se graba en el archivo preindex
        'Ing Miguel Angel Urueta Miranda
        'Feha 2012-08-01
        '************************************************
        Try
            Dim Nombre_Preindex As String = ""
            Dim Result = ""
            Dim Refclas As New ClassAlmacenamiento
            Result = Refclas.Arma_Nombre_Archivo_Preindex(Nombre_Archivo, Nombre_Preindex)
            If Result <> "YES" Then
                Retorna_Id_imagen_Archivo = "Imposible generar nombre archivo preindex"
                Exit Function
            End If
            Dim Refileinfes As New FileInfo(Nombre_Archivo)
            Nombre_Preindex = Refileinfes.DirectoryName.ToString & "\" & Nombre_Preindex
            Dim bo As Boolean = System.IO.File.Exists(Nombre_Preindex)
            If bo = False Then
                Retorna_Id_imagen_Archivo = "El documento no tiene archivo preindex para almacenar un link"
                Exit Function
            End If
            Dim Xml2 = New XmlDocument()
            Xml2.Load(Nombre_Preindex)
            Result = ""
            Result = Leyendo_Archivo_Preindex_Xml_id_imagen(Xml2, Id_Link)
            If Result <> "YES" Then
                Retorna_Id_imagen_Archivo = "Error Leyendo archivo Preindex   " & Result
                Exit Function
            End If
            Retorna_Id_imagen_Archivo = "YES"
        Catch ex As Exception
            Retorna_Id_imagen_Archivo = "Error general funcion : Retorna_Id_imagen_Archivo " & ex.ToString
        End Try
    End Function
    Function Calcula_Tamaño_Archivo(ByVal Fil As String,
                                    ByRef TamFile As Integer) As String
        Try
            Dim filearchivo As New FileInfo(Fil)
            If filearchivo.Exists = False Then
                Calcula_Tamaño_Archivo = "Imposible encontrar el archivo " & Fil
                Exit Function
            End If
            TamFile = Math.Round((Fil.Length / 1024) / 50)
            If TamFile = 0 Then
                TamFile = 1
            End If
            Calcula_Tamaño_Archivo = "YES"
        Catch ex As Exception
            Calcula_Tamaño_Archivo = "inconsistencia general funcion Calcula_Tamaño_Archivo  " & ex.Message
        End Try
    End Function


    Public Function Almacenamiento_simple(ByVal _Ruta_Carpeta As String,
    ByVal _Nombre_Documento As String, ByVal _Nombre_Gabienete As String,
    ByVal _Estado_Elimina As Integer, ByRef _Matri_Datos() As String,
    ByVal _Tipo_Alamcenamiento As Integer, ByVal Numero_Pag As Integer,
    ByVal Tipo_Doc As Integer, ByRef Matri_Dcoumentos() As Object, ByVal Evalua_Campo_Obli As Integer,
    ByRef Id_Almacen As Integer, ByVal Tipo_Doc_Añade As Integer, ByVal Login_Usuario As String) As String
        Dim Result As String = ""
        Dim _Ruta_Almacenamiento As String = ""
        'System.Windows.Forms.Application.DoEvents()
        '****************************************
        'Consulta ruta almacenamiento
        '****************************************
        Result = ""
        Dim ref_Class_SYSTEM1RUT As New Class_SYSTEM1RUT
        Result = ref_Class_SYSTEM1RUT.Consulta_Ruta_Almacenamiento(_Ruta_Almacenamiento,
                                                                   _Nombre_Gabienete)
        If Result <> "YES" Then
            Almacenamiento_simple = Result
            Exit Function
        End If
        '*************************************
        'Verifica que la imagen se encuentre
        'en la ruta    _Ruta_Carpeta
        '*************************************
        'If File.Exists(_Ruta_Carpeta & _Nombre_Documento) = False Then
        'Almacenamiento = " Error -2 Funcion Alamcenamiento No se encontro imagen para almacenar " & _Ruta_Carpeta & _Nombre_Documento
        'Exit Function
        'End If
        '**************************************************************
        'Contruye archivo preindex para eviar al cliente que almacena
        '**************************************************************
        Dim Ref_Nombre_Documento As String = ""
        Dim Ceros_Documento As String = ""
        'Result = Contruye_Nombre_Archvio_Index(_Ruta_Carpeta, _Nombre_Documento, Ceros_Documento, Ref_Nombre_Documento)
        'If Result <> "YES" Then
        '    Almacenamiento = "Error Construyendo nombre del archivo preindex Archivo preindex " & Result
        '    Exit Function
        'End If
        '****************************************
        'Verifica la existencia del archivo 
        'Preindex si es almacenamineto por bach
        'se evalua tipo almacenamiento variable
        '_Tipo_Alamcenamiento si es uno almacena
        'por bach y es necesario leer el archivo
        'para los datos
        '*****************************************
        Dim Nombre_Archivo_Preindex As String = ""
        Dim Tipo_Archivo As String = ""
        '*********************************
        'Determina si se lee archivo 
        'Preindex
        '*********************************
        If _Tipo_Alamcenamiento = 1 Then
            If File.Exists(_Ruta_Carpeta & Ceros_Documento & Ref_Nombre_Documento & ".xmls") = True Then
                Nombre_Archivo_Preindex = _Ruta_Carpeta & Ceros_Documento & Ref_Nombre_Documento & ".xmls"
                Tipo_Archivo = ".xmls"
            End If
            If File.Exists(_Ruta_Carpeta & Ceros_Documento & Ref_Nombre_Documento & ".txt") = True Then
                Nombre_Archivo_Preindex = _Ruta_Carpeta & Ceros_Documento & Ref_Nombre_Documento & ".txt"
                Tipo_Archivo = ".txt"
            End If
            If Nombre_Archivo_Preindex = "" Then
                Almacenamiento_simple = "Documento sin preindexacion"
                Exit Function
            End If
            '****************************
            'Lee archivo Preindex (txt)
            '****************************
            If Tipo_Archivo = ".txt" Then
                Result = Leer_Archivo_Preindex(Nombre_Archivo_Preindex, _Matri_Datos)
                If Result <> "YES" Then
                    Almacenamiento_simple = "Imposible Leer Archivo Preindex " & Nombre_Archivo_Preindex & " Error " & Result
                    Exit Function
                End If
            End If
        End If
        '**************************************************************
        'codigo que permite verificar que los datos de los campos
        'obligatorios contengan la informacion del archivo
        '**************************************************************
        Dim Matri_Campos_Obli() As String
        Erase Matri_Campos_Obli
        Result = ""
        Dim ref_Class_DETALLE_GABIENETE As New Class_DETALLE_GABIENETE
        Result = ref_Class_DETALLE_GABIENETE.Consulta_Campos_Obligatorio(_Nombre_Gabienete,
                                                                         Matri_Campos_Obli)
        If Result <> "YES" Then
            Almacenamiento_simple = "Imposible Encontrar datos para campos obligatorios Error " & Result
            Exit Function
        End If
        If Matri_Campos_Obli Is Nothing Then
            Almacenamiento_simple = "Matri Campos es nula consulte gabinete_detalle"
            Exit Function
        End If
        If UBound(Matri_Campos_Obli) <> UBound(_Matri_Datos) Then
            Almacenamiento_simple = "Las matrices de datos y campos no son iguales es posible que el preindex pertenezca a otro gabinete "
            Exit Function
        End If
        '**************************************
        'Determina si evalua los campos obli
        'gatorios
        '**************************************
        If Evalua_Campo_Obli = 1 Then
            For z As Integer = 0 To UBound(Matri_Campos_Obli)
                Dim Matri_Tempo() As String
                Erase Matri_Tempo
                Matri_Tempo = Matri_Campos_Obli(z).Split("|")
                If Matri_Tempo(0) = 1 And _Matri_Datos(z) = "" Then
                    Almacenamiento_simple = "El Campo " & Matri_Tempo(1).ToString & " es obligatorio "
                    Exit Function
                End If
            Next
        End If
        '*****************************************************************
        'consulta la base de datos system para traer los datos del sistema
        'esta consulta bloquea la base de datos y bloquea este registro
        'para que los demas usuarios no lo modifiquen al tiempo
        '*****************************************************************
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Dim carpealma As String = ""
        Dim numcarpvar As Integer = 0
        Dim Numero_Pagina_Carp As Integer = 0
        Dim tandiscvar As Long = 0
        Dim idal As Integer = 0
        Dim disc As Integer = 0
        Dim useral As String = Login_Usuario
        Dim pagi As Integer = Numero_Pag
        Dim indexal As Integer = Tipo_Doc
        Dim date1al As String = Date.Today
        Dim time1al As String = Date.Now.ToString
        ref_ClassGestionFechas.Formatea_Fecha_Almacenamiento_Time(time1al)
        Dim TempoFecha As String = Left(time1al, 10)
        time1al = Trim(time1al.Replace(TempoFecha, ""))
        Result = ""
        Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
        If Result <> "YES" Then
            Almacenamiento_simple = "Error formateando fecha almacenamiento Funcion: Formatea_Fecha_Almacenamiento " & Result
            Exit Function
        End If
        Dim mySqldatReader As MySqlDataReader
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_DA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Try
            'myConnection.Open()
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            Dim Parametro_Select_System1 As String = " SELECT disco,proxid,tamdisc,numcarp,NUMPAG_CARP FROM system1 where nombre = " _
            & "'" & _Nombre_Gabienete & "' " & "for update"
            myCommand.CommandText = Parametro_Select_System1
            mySqldatReader = myCommand.ExecuteReader()
            If mySqldatReader Is Nothing Then
                Almacenamiento_simple = "Imposible Encontrar Registro En Tabla Systema Error Conexion"
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            If mySqldatReader.HasRows = False Then
                Almacenamiento_simple = "Imposible Encontrar Registro En Tabla Systema"
                'myTrans.Rollback()
                mySqldatReader.Close()
                myConnection.Close()
                Exit Function

            End If
            '*******************************************************
            'Valores recuperados de la consulta de la tabla system1
            '********************************************************
            mySqldatReader.Read()
            numcarpvar = mySqldatReader.Item("NUMCARP")
            tandiscvar = mySqldatReader.Item("TAMDISC")
            idal = mySqldatReader.Item("PROXID")
            Numero_Pagina_Carp = mySqldatReader.Item("NUMPAG_CARP")
            idal = idal + 1
            disc = mySqldatReader.Item("DISCO")
            '***************************************************
            'validacion del tamaño de disco valor tabal system
            '***************************************************
            mySqldatReader.Close()
            If tandiscvar = 572523149 Or tandiscvar = 4310948432 Then
            Else
                Almacenamiento_simple = "Tamaño de disco incorrecto Consulte su amnistrador valor : " & tandiscvar
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '****************************************************
            'validar que la carpeta sea diferente de cero
            '***************************************************
            If numcarpvar = 0 Then
                Almacenamiento_simple = "Valor incorrecto de carpeta Consulte su amnistrador valor : " & numcarpvar
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '**************************************************
            'validar que el numero del disco sea valido
            '**************************************************
            If disc = 0 Then
                Almacenamiento_simple = "Valor incorrecto de disco Consulte su amnistrador valor : " & disc
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '****************************************************
            'Consulta que el usuario este registrado en el sistema
            '****************************************************
            If useral = "" Then
                Almacenamiento_simple = "Usuario no valido"
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '*****************************************************
            'consulta que el ide sea diferente de ""
            '*****************************************************
            If idal = 0 Then
                Almacenamiento_simple = "Valor incorrecto de identidad de imagen Consulte su amnistrador valor : " & idal
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '******************************************************
            'consulta que le numero de paginas sea diferente de "0"
            '******************************************************
            If pagi = 0 Then
                Almacenamiento_simple = "Valor incorrecto de paginas Consulte su amnistrador valor : " & pagi
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '**************************************
            'Consulta que el disco tenga espacion
            'donde se guardan las imagenes SL
            '**************************************
            Dim EstadoDisco As String = ""
            Dim ResulDisco As String = ""
            Dim ref_Class_disco_detalle As New Class_disco_detalle
            ResulDisco = ref_Class_disco_detalle.Numero_Imagenes(_Nombre_Gabienete,
                                                                 tandiscvar,
                                                                 disc,
                                                                 EstadoDisco)
            If ResulDisco = "YES" Then
                If EstadoDisco = "SL" Then
                    Almacenamiento_simple = "Disco  " & disc.ToString & " Sobrepaso el limite de capacidad"
                    'myConnection.Close()
                    'myTrans.Rollback()
                    myConnection.Close()
                    Exit Function

                End If
            Else
                'myTrans.Rollback()
                myConnection.Close()
                Almacenamiento_simple = ResulDisco
                Exit Function
            End If
            '*****************************************************
            'Actualizacion para determinar el numero de imagenes
            'por base de datos para evitar contar los archivos
            'del directorio y agilizar la funcion
            '****************************************************
            Dim Valor_Suma_Imagen As Integer = Numero_Pag + Numero_Pagina_Carp
            If Valor_Suma_Imagen > 230 Then
                numcarpvar = numcarpvar + 1
                Numero_Pagina_Carp = Numero_Pag
            Else
                Numero_Pagina_Carp = Valor_Suma_Imagen

            End If
            Dim Parametro_Actualiza_System1 As String = "update system1 set proxid = " & "'" & idal & "' ," &
            " numcarp = " & " '" & numcarpvar & "', NUMPAG_CARP=" & Numero_Pagina_Carp & " where nombre =" & "'" &
            _Nombre_Gabienete & "'" & "and proxid <> " & "'" & idal & "'"
            myCommand.CommandText = Parametro_Actualiza_System1
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            '*********************************
            'Determina si se actualizo
            'el nuevo id de la base de datos
            '*********************************
            If Switc = 0 Then
                Almacenamiento_simple = "Imposible actualizar la tabla System  : " & Parametro_Actualiza_System1
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myTrans.Commit()
        Catch e As Exception
            Try

            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    myTrans.Rollback()
                    myConnection.Close()
                    Almacenamiento_simple = "An exception of type " + ex.GetType().ToString() +
                                      " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try
            If Not myTrans Is Nothing Then
                myTrans.Rollback()
            End If
            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Almacenamiento_simple = "Error General " & e.ToString
            Exit Function
        End Try

        '*****************************************************
        'verifica existencia de carpeta y crea nueva carpeta
        '*****************************************************
        Dim rut2000 As String = ""
        Dim Result_Carpeta As String
        Dim RutaCarpet As String
        RutaCarpet = _Ruta_Almacenamiento & _Nombre_Gabienete & disc
        Result_Carpeta = Solicita_Carpeta_almacenamiento(carpealma, numcarpvar, rut2000, RutaCarpet)
        If Result_Carpeta <> "YES" Then
            Almacenamiento_simple = "Imposible calcular tamaño carpeta Consulte su administrador  : " & Result_Carpeta
            'myTrans.Rollback()
            'myConnection.Close()

            Exit Function
        End If
        carpealma = carpealma & numcarpvar

        Try

            '*************************************
            'Arma sql de almacenamiento
            '*************************************
            Dim Matri_Xml() As String
            Erase Matri_Xml
            Dim Campos_Insert As String = "ID,DISC,PAG,DBT,IDEX,USER,DATE1,TIME1"
            Dim Datos_Insert As String = idal & "," & " " & disc & " " & "," & "'" & pagi & "'" & "," & "'" & Tipo_Doc & "'" & "," & "'" & numcarpvar & "'" & ","
            Datos_Insert = Datos_Insert & "'" & useral & "'" & "," & "'" & date1al & "'" & "," & "'" & time1al & "'"
            For z2 As Integer = 0 To UBound(Matri_Campos_Obli)
                Dim Matri_Tempo() As String
                Erase Matri_Tempo
                Matri_Tempo = Matri_Campos_Obli(z2).Split("|")
                Campos_Insert = Campos_Insert & "," & Matri_Tempo(1).ToString
            Next
            For z3 As Integer = 0 To UBound(_Matri_Datos)
                If _Matri_Datos(z3) = "" Then
                    Datos_Insert = Datos_Insert & "," & "null"
                Else
                    Datos_Insert = Datos_Insert & ",'" & _Matri_Datos(z3) & "'"
                End If

            Next
            Dim Switc2 As Integer = 0
            Dim Parametro_Insert_Registro As String = "Insert into " & _Nombre_Gabienete & "( " & Campos_Insert & " )" & " Values " & "( " & Datos_Insert & " )"
            Dim myCommand2 As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand2.Connection = myConnection
            myCommand2.Transaction = myTrans
            myCommand2.CommandText = Parametro_Insert_Registro
            Switc2 = myCommand2.ExecuteNonQuery()
            '************************************
            'Determina si se inserto el registro
            'el nuevo id de la base de datos
            '*************************************
            If Switc2 = 0 Then
                Almacenamiento_simple = "Imposible agregar registro  : " & Parametro_Insert_Registro
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '***********************************
            'Crea archivo xml para imagen
            '***********************************
            ReDim Preserve Matri_Xml(0)
            Matri_Xml(0) = "ID¬" & idal
            ReDim Preserve Matri_Xml(1)
            Matri_Xml(1) = "DISC¬" & disc
            ReDim Preserve Matri_Xml(2)
            Matri_Xml(2) = "PAG¬" & pagi
            ReDim Preserve Matri_Xml(3)
            Matri_Xml(3) = "DBT¬" & Tipo_Doc
            ReDim Preserve Matri_Xml(4)
            Matri_Xml(4) = "IDEX¬" & numcarpvar
            ReDim Preserve Matri_Xml(5)
            Matri_Xml(5) = "USER¬" & useral
            ReDim Preserve Matri_Xml(6)
            Matri_Xml(6) = "DATE1¬" & date1al
            ReDim Preserve Matri_Xml(7)
            Matri_Xml(7) = "TIME1¬" & time1al
            Dim IncreMat As Integer = 7
            For z2 As Integer = 0 To UBound(Matri_Campos_Obli)
                IncreMat = IncreMat + 1
                Dim Matri_Tempo() As String
                Erase Matri_Tempo
                Matri_Tempo = Matri_Campos_Obli(z2).Split("|")
                Campos_Insert = Campos_Insert & "," & Matri_Tempo(1).ToString
                ReDim Preserve Matri_Xml(IncreMat)
                Matri_Xml(IncreMat) = Matri_Tempo(1).ToString & "¬"
                If _Matri_Datos(z2) = "" Then
                    Matri_Xml(IncreMat) = Matri_Xml(IncreMat) & "null"
                Else
                    Matri_Xml(IncreMat) = Matri_Xml(IncreMat) & _Matri_Datos(z2)
                End If
            Next
            Dim Ruta_Alamce_Image As String = _Ruta_Almacenamiento & _Nombre_Gabienete & disc & "\" & carpealma & "\"
            Result = ""
            Result = Generando_Archivo_Dat_Xml(Ruta_Alamce_Image,
                                               idal,
                                               Matri_Xml,
                                               _Ruta_Carpeta)
            If Result <> "YES" Then
                myTrans.Rollback()
                myConnection.Close()
                Almacenamiento_simple = "Error Generando xml" & Result
                Exit Function
            End If
            Result = ""
            Result = Copia_Imagen_Almacenada_local(Ruta_Alamce_Image,
                                                   idal,
                                                   Matri_Dcoumentos,
                                                   _Ruta_Carpeta,
                                                   Tipo_Doc_Añade)
            If Result <> "YES" Then
                myTrans.Rollback()
                If Not myConnection Is Nothing Then
                    myConnection.Close()
                End If
                Almacenamiento_simple = "Error Copiando Imagenes " & Result
                Exit Function
            End If
            '**************************************
            'Actualiza numero imagenes en la tabla
            'disco detalle
            '**************************************
            Dim Parametro_A As String = "select NUMERO_IMAGENES  from disco_detalle  where disco = '" & disc & "'" &
            " and gabinete ='" & _Nombre_Gabienete & "'" & " for update "
            myCommand2.CommandText = Parametro_A
            mySqldatReader = myCommand2.ExecuteReader()
            If mySqldatReader Is Nothing Then
                Almacenamiento_simple = "Error sql para encontrar disco commando " & Parametro_A
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            If mySqldatReader.HasRows = False Then
                Almacenamiento_simple = "Imposible Encontrar disco en la tabla disco detalle"
                myTrans.Rollback()
                mySqldatReader.Close()
                myConnection.Close()
                Exit Function
            End If
            mySqldatReader.Read()
            Dim IncreNumPage As Integer = mySqldatReader.Item(0)
            IncreNumPage = IncreNumPage + Val(pagi)
            mySqldatReader.Close()
            Dim SqlActualiza As String = "Update disco_detalle set NUMERO_IMAGENES=" & IncreNumPage & " , NUMPAG_CARP=" & Numero_Pagina_Carp & " where disco = '" & disc & "'" &
            " and gabinete ='" & _Nombre_Gabienete & "'"
            myCommand2.CommandText = SqlActualiza
            myCommand2.ExecuteNonQuery()
            myTrans.Commit()
            '******************************************************
            'Se agreag esta linea para devolber imagen indexada
            '******************************************************
            Id_Almacen = idal
        Catch e As Exception
            Try
                If Not mySqldatReader Is Nothing Then
                    mySqldatReader.Close()
                End If
                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Almacenamiento_simple = "An exception of type " + ex.GetType().ToString() +
                                      " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Almacenamiento_simple = "Error General " & e.ToString
            Exit Function
        End Try
        myConnection.Close()
        '*********************************************
        'Determina si elimina la imagen de la carpeta
        '*********************************************
        Almacenamiento_simple = "YES"
    End Function
    Function Almacena_documento_nueva_version(ByVal id_gabinete As Integer,
                                              ByVal nombre_gabinete As String,
                                              ByVal id_imagen As Integer,
                                              ByVal matri_documentos() As String,
                                              ByVal id_usuario_gestion As Integer,
                                              ByVal id_usuario_da As Integer,
                                              ByVal logui_usuario_gestion As String,
                                              ByVal logui_usuario_da As String,
                                              ByVal option_remplaza_gabinete As Integer,
                                              ByVal option_elimina_archivo As Integer,
                                              ByRef Extension_documento As String,
                                              ByRef item_list As class_list_detalle_version_document) As String
        '--------------------------------------------------------------------------------
        'Funcion : función que almacena documento nueva version y con la opción de remplazar
        '          en gabinete
        '          
        '--------------------------------------------------------------------------------
        '                           PARAMETROS  
        '--------------------------------------------------------------------------------
        'id_imagen                 : Representa la identificación de la imagen dentro del
        '                            gabinete.
        'gabinete                  : Representa el nombre del gabinete.
        'id_usuario_da             : Representa la identificación del usuario docuarchi
        'Stru_registro_migracion   : Representa la estructura del registro de migración
        'logui_usuario_gestion     : Representa el login del usuario de gestion
        'logui_usuario_da          : Representa el logion del usuaario docuarchi
        'matri_documemtos_gabinete : Representa la matris de documentos migrados
        'option_remplaza_gabinete  : Representa la opcion si remplaza versión en el gabine
        '                            te
        'option_elimina_archivo    : Representa la opción si elimina los documentos tempora
        '                            les de almacenamiento
        '--------------------------------------------------------------------------------
        '                           RETORNO
        '--------------------------------------------------------------------------------
        'Extension                 : Retorna la extensión del archivo migrado
        'item_list                 : Representa la estructura con los detalles del nuevo documento
        '                               de version                                   
        '--------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '--------------------------------------------------------------------------------
        'Fecha                 : 2024-07-19
        'Elabora               : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------------------------
        Dim Result As String = ""
        Dim Class_SYSTEM1RUT As New Class_SYSTEM1RUT
        Dim _Ruta_Almacenamiento As String = ""
        Result = Class_SYSTEM1RUT.Consulta_Ruta_Almacenamiento(_Ruta_Almacenamiento,
                                                               nombre_gabinete)
        If Result <> "YES" Then
            Almacena_documento_nueva_version = Result
            Exit Function
        End If
        Dim Class_fyle_system As New Class_fyle_system
        Dim peso_archivo As String = ""
        Dim Numero_Pag = matri_documentos.Length  'Obtien el numero de archivos a guardar en el disco
        Result = Class_fyle_system.Solicita_peso_matriz_documentos(matri_documentos,
                                                                   peso_archivo)
        If Result <> "YES" Then
            Almacena_documento_nueva_version = Result
            Exit Function
        End If

        Dim Class_ItexShare As New Class_ItexShare
        Dim file_inf As New FileInfo(matri_documentos(0))
        If UCase(file_inf.Extension) = ".PDF" Then
            Result = Class_ItexShare.Retorna_numero_paginas_documentos_pdf(matri_documentos(0),
                                                                           Numero_Pag)
            If Result <> "YES" Then
                Almacena_documento_nueva_version = Result
                Exit Function
            End If
        End If
        Dim ClassGestionFechas As New ClassGestionFechas
        Dim time1al As String = Date.Now.ToString
        ClassGestionFechas.Formatea_Fecha_Almacenamiento_Time(time1al)
        Dim fecha_registro As String = time1al
        Dim TempoFecha As String = Left(time1al, 10)
        time1al = Trim(time1al.Replace(TempoFecha, ""))
        '--------Parametros de registro de nueva version de documentos
        Dim SQL_registro_version_nuevo As String = ""
        Dim ID_new_version As Integer = 0
        Dim DISC_new_version As Integer = 0
        Dim PAG_new_version As Integer = 0
        Dim DBT_new_version As Integer = 0
        Dim IDEX_new_version As Integer = 0
        Dim USER_DA_new_version As String = ""
        Dim CTRL_ACES_new_version As Integer = 0
        Dim PESO_DOCUMENTO_new_version As String = ""
        Dim TIPO_ARCHIVO_new_version As String = ""
        Dim ID_REG_MIGRA_new_version As Long = 0
        Dim PAGINA_DOCUMENT_new_version As Integer = 0
        Dim ESTADO_FIRMA_DIGITAL_new_version As Integer = 0
        PAG_new_version = Numero_Pag
        PAGINA_DOCUMENT_new_version = Numero_Pag
        ID_new_version = id_imagen
        ID_REG_MIGRA_new_version = 0
        USER_DA_new_version = logui_usuario_da
        CTRL_ACES_new_version = 0
        PESO_DOCUMENTO_new_version = peso_archivo
        ESTADO_FIRMA_DIGITAL_new_version = 0
        TIPO_ARCHIVO_new_version = UCase(file_inf.Extension)
        '-----------Solicita el tipo de documento de la nueva version
        Dim Class_da_extension As New Class_da_extension
        Dim tipo_documento_gabinete As Integer = 0
        Result = Class_da_extension.SolicitaTipoArchivoDocuarchiExtension(UCase(file_inf.Extension),
                                                                           DBT_new_version)
        If Result <> "YES" Then
            Almacena_documento_nueva_version = Result
            Exit Function
        End If
        Dim ClassDaGabinete As New ClassDaGabinete
        '-----------Solicita el tipo de icono
        ClassDaGabinete.Agrega_icono_image_fownt_extension_cort(UCase(file_inf.Extension), item_list.IconoAsome)
        '--------Parametros de registro  version documento remplazo
        Dim SQL_registro_version_remplazo As String = ""
        Dim ID_rem_version As Integer = 0
        Dim DISC_rem_version As Integer = 0
        Dim PAG_rem_version As Integer = 0
        Dim DBT_rem_version As Integer = 0
        Dim IDEX_rem_version As Integer = 0
        Dim USER_DA_rem_version As String = ""
        Dim CTRL_ACES_rem_version As Integer = 0
        Dim PESO_DOCUMENTO_rem_version As String = ""
        Dim TIPO_ARCHIVO_rem_version As String = ""
        Dim ID_REG_MIGRA_rem_version As Long = 0
        Dim ID_REGISTRO_VERSION_rem_version As Integer = 0
        Dim ID_VERSION_DOC_rem_version As Integer = 0
        Dim PAGINA_DOCUMENT_rem_version As Integer = 0
        Dim ESTADO_FIRMA_DIGITAL_rem_version As Integer = 0
        Dim DATE_rem_version As String = ""
        Dim datetemp As String = ""
        Dim Stru_paramter_image As stru_paramter_image = Nothing
        '---------------------Solicita los datos de registro o actualzación para la imagen de remplazo en el gabinete---------OPTIONAL
        If option_remplaza_gabinete = 1 Then
            Result = ClassDaGabinete.SolicitaEtructuraImagenGabinete(nombre_gabinete,
                                                                     id_imagen,
                                                                     Stru_paramter_image,
                                                                     1,
                                                                     1,
                                                                     1,
                                                                     1,
                                                                     1)
            If Result <> "YES" Then
                Almacena_documento_nueva_version = Result
                Exit Function
            End If
            datetemp = Stru_paramter_image.DATE1 & " " & Stru_paramter_image.TIME1
            ClassGestionFechas.Formatea_fecha_time_db(datetemp, DATE_rem_version)
            ID_rem_version = id_imagen
            DISC_rem_version = Stru_paramter_image.DISC
            PAG_rem_version = Stru_paramter_image.PAG
            DBT_rem_version = Stru_paramter_image.DBT_TIPO_IMAGEN
            IDEX_rem_version = Stru_paramter_image.IDEX
            USER_DA_rem_version = Stru_paramter_image.USER
            CTRL_ACES_rem_version = Stru_paramter_image.CTRL_ACES
            ID_REGISTRO_VERSION_rem_version = Stru_paramter_image.ID_REGISTRO_VERSION
            ID_VERSION_DOC_rem_version = Stru_paramter_image.ID_VERSION_DOC
            ESTADO_FIRMA_DIGITAL_rem_version = Stru_paramter_image.ESTADO_FIRMA_DIGITAL
            Dim matri_documemtos_gabinete() As String = Nothing
            Result = ClassDaGabinete.Solicita_matriz_documentos_almacenados_gabinete(id_imagen,
                                                                                     nombre_gabinete,
                                                                                     matri_documemtos_gabinete)
            If Result <> "YES" Then
                Almacena_documento_nueva_version = Result
                Exit Function
            End If
            Dim ref_matri_documento() As String = Nothing
            Dim Icont As Integer = 0
            For i As Integer = 1 To matri_documemtos_gabinete.Length - 1
                ReDim Preserve ref_matri_documento(Icont)
                ref_matri_documento(Icont) = matri_documemtos_gabinete(i)
                Icont = Icont + 1
            Next
            '----------Solicita peso documento gabinete a remplazar
            Result = Class_fyle_system.Solicita_peso_matriz_documentos(ref_matri_documento,
                                                                       PESO_DOCUMENTO_rem_version)
            If Result <> "YES" Then
                Almacena_documento_nueva_version = Result
                Exit Function
            End If
            If Stru_paramter_image.DBT_TIPO_IMAGEN = -10 Then
                PAGINA_DOCUMENT_rem_version = ref_matri_documento.Length - 1
            Else
                PAGINA_DOCUMENT_rem_version = Stru_paramter_image.PAG
            End If
            '----------Solicita la extension o tipo documento del gabinete
            Dim file_inf_rem As New FileInfo(ref_matri_documento(0))
            TIPO_ARCHIVO_rem_version = UCase(file_inf_rem.Extension)
            '//--------Solicita registro de firma del 
        End If
        '-------------------Solicita el registro de produccion si debe remplazar la version del documento en el gabinete---OPTIONAL
        Dim ClassGaProducionDocumental As New ClassGaProducionDocumental
        Dim id_registro_producion As Long = 0
        If option_remplaza_gabinete = 1 Then
            Result = ClassGaProducionDocumental.Solicita_id_registro_producion_documental(id_imagen,
                                                                                          nombre_gabinete,
                                                                                          id_registro_producion)
            If Result <> "YES" Then
                Almacena_documento_nueva_version = Result
                Exit Function
            End If
        End If
        Dim Stru_produccion_indice As stru_produccion_indice = Nothing
        Dim Stru_expediente() As expediente_conservacion = Nothing
        Dim Ruta_archivo_xml As String = ""
        Dim ClassGaExpediente As New ClassGaExpediente
        '-------------------Solicita los datos del expediente y la ruta del expediente electrónico si esta activa la opcion de remplazar documento ---OPTIONAL
        If id_registro_producion <> 0 Then
            Result = ClassGaProducionDocumental.Solicita_estructura_id_registro_produccion(id_registro_producion,
                                                                                           Stru_produccion_indice)
            If Result <> "YES" Then
                Almacena_documento_nueva_version = Result
                Exit Function
            End If
            If Stru_produccion_indice.EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE <> 0 Then
                Result = ClassGaExpediente.SolicitaDatosEstructuraExpediente(Stru_produccion_indice.EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE,
                                                                                           Stru_expediente)
                If Result <> "YES" Then
                    Almacena_documento_nueva_version = Result
                    Exit Function
                End If
                If Stru_expediente(0).estado_expediente_electronico = 2 Then
                    Result = ClassGaExpediente.Solicita_archivo_indice_expediente(Stru_produccion_indice.EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE,
                                                                                  Ruta_archivo_xml)
                    If Result <> "YES" Then
                        Almacena_documento_nueva_version = Result
                        Exit Function
                    End If
                    If File.Exists(Ruta_archivo_xml) = False Then
                        Almacena_documento_nueva_version = "Imposible encontrar el archivo indice del expediente (" & Ruta_archivo_xml & ")"
                        Exit Function
                    End If
                End If
            End If
        End If
        Dim Class_system1 As New Class_system1
        Dim Stru_datos_configuracion_gabinete As stru_datos_configuracion_gabinete = Nothing
        Result = Class_system1.Solicita_datos_configuracion_gabinete(id_gabinete,
                                                                     Stru_datos_configuracion_gabinete)
        If Result <> "YES" Then
            Almacena_documento_nueva_version = Result
            Exit Function
        End If
        DISC_new_version = Stru_datos_configuracion_gabinete.disco
        '---------------------Valida estado del disco--------OBLI
        Dim Class_disco_detalle As New Class_disco_detalle
        Dim estado_disco As String = ""
        Result = Class_disco_detalle.Numero_Imagenes(nombre_gabinete,
                                                     Stru_datos_configuracion_gabinete.tamdisc,
                                                     Stru_datos_configuracion_gabinete.disco,
                                                     estado_disco)
        If Result <> "YES" Then
            Almacena_documento_nueva_version = Result
            Exit Function
        End If
        If estado_disco = "SL" Then
            Almacena_documento_nueva_version = "El disco (" & Stru_datos_configuracion_gabinete.disco & ") sobre paso el limite de capacidad"
            Exit Function
        End If
        '-----------------Solicita el registro de control de versión para determinar si lo registra--------OBLI 
        Dim Class_ra_ver_control_version_documento As New Class_ra_ver_control_version_documento
        Dim id_control_version_documento As Long = 0
        Result = Class_ra_ver_control_version_documento.Solicita_id_registro_control_version(id_gabinete,
                                                                                             id_imagen,
                                                                                             id_control_version_documento)
        If Result <> "YES" Then
            Almacena_documento_nueva_version = Result
            Exit Function
        End If
        '------------------Registra el control de versión ------------------ OBLI
        If id_control_version_documento = 0 Then
            Result = Class_ra_ver_control_version_documento.Registra_control_version_documento(id_gabinete,
                                                                                               id_imagen,
                                                                                               fecha_registro,
                                                                                               id_control_version_documento)
            If Result <> "YES" Then
                Almacena_documento_nueva_version = Result
                Exit Function
            End If
        End If
        '--------Solicita los discos de ublicación de cada una de las versiones de documentos-------OBLI
        Dim Class_ra_ver_version_documento As New Class_ra_ver_version_documento
        Dim Stru_registro_version_documento() As stru_registro_version_documento = Nothing
        Result = Class_ra_ver_version_documento.Solicita_lista_discos_carpetas(id_imagen,
                                                                               id_gabinete,
                                                                               Stru_registro_version_documento)
        If Result <> "YES" Then
            Almacena_documento_nueva_version = Result
            Exit Function
        End If
        '--------Solicita registro activo en el gabinete en el registro de versiones------OBLI
        Dim id_registro_version_activo As Integer = 0
        Result = Class_ra_ver_version_documento.Solicita_registro_activo_gabinete(id_imagen,
                                                                                 id_gabinete,
                                                                                 id_registro_version_activo)
        If Result <> "YES" Then
            Almacena_documento_nueva_version = Result
            Exit Function
        End If
        '--------Crea el nombre del nuevo archivo con Zero Fill
        Dim ZeroFillImagen As String = ""
        Dim NameNEWarchivo As String = ""
        Result = Ceros_Imagen_Almacenada(ZeroFillImagen,
                                         id_imagen)
        If Result <> "YES" Then
            Almacena_documento_nueva_version = "Error generando ZerroFill imagen "
            Exit Function
        End If
        NameNEWarchivo = "DIG" & ZeroFillImagen & id_imagen & TIPO_ARCHIVO_new_version
        Dim mySqldatReader As MySqlDataReader
        Dim mySqldatReade_CONTROL As MySqlDataReader
        Dim myConnection As New MySqlConnection
        Dim myTrans As MySqlTransaction
        Dim ref As New conect.Dbase_Conction_Mysql_DA
        Result = ref.Returna_Conexion_Mysql(myConnection)
        If Result <> "YES" Then
            Almacena_documento_nueva_version = Result
            Exit Function
        End If
        Try
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            Dim Parametro_Select_System1 As String = " SELECT disco,proxid,tamdisc,numcarp,NUMPAG_CARP FROM system1 where nombre = " _
                & "'" & nombre_gabinete & "' " & "for update"
            myCommand.CommandText = Parametro_Select_System1
            mySqldatReader = myCommand.ExecuteReader()
            If mySqldatReader Is Nothing Then
                Almacena_documento_nueva_version = "Imposible Encontrar Registro En Tabla Systema Error Conexion"
                myConnection.Close()
                Exit Function
            End If
            If mySqldatReader.HasRows = False Then
                Almacena_documento_nueva_version = "Imposible Encontrar Registro En Tabla Systema"
                mySqldatReader.Close()
                myConnection.Close()
                Exit Function
            End If
            mySqldatReader.Read()
            Dim numcarpvar = mySqldatReader.Item("NUMCARP")
            Dim nuncarpvarREF = mySqldatReader.Item("NUMCARP")
            Dim Numero_Pagina_Carp = mySqldatReader.Item("NUMPAG_CARP")
            DISC_new_version = mySqldatReader.Item("disco")
            mySqldatReader.Close()
            '*****************************************************
            'Actualizacion para determinar el numero de imagenes
            'por base de datos para evitar contar los archivos
            'del directorio y agilizar la funcion
            '****************************************************
            Dim Valor_Suma_Imagen As Integer = Numero_Pag + Numero_Pagina_Carp
            If Valor_Suma_Imagen > 230 Then
                numcarpvar = numcarpvar + 1
                Numero_Pagina_Carp = Numero_Pag
            Else
                Numero_Pagina_Carp = Valor_Suma_Imagen
            End If
            '---------------------------------------------------------
            'Valida que los documemtos no esten en las misma carpeta
            'e incrementa el numero de la carpeta a almacenar
            '---------------------------------------------------------
            If IDEX_rem_version = numcarpvar Or id_registro_version_activo = 0 Then
                numcarpvar = numcarpvar + 1
            End If
            If Not Stru_registro_version_documento Is Nothing Then
                For i As Integer = 0 To Stru_registro_version_documento.Length - 1
                    If Stru_registro_version_documento(i).DISC = DISC_new_version And Stru_registro_version_documento(i).IDEX = numcarpvar Then
                        numcarpvar = numcarpvar + 1
                    End If
                Next
            End If
            Dim Parametro_Actualiza_System1 As String = "update system1 set numcarp = " & "'" & numcarpvar & "', NUMPAG_CARP=" & Numero_Pagina_Carp & " where nombre =" & "'" &
                    nombre_gabinete & "'"
            myCommand.CommandText = Parametro_Actualiza_System1
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Almacena_documento_nueva_version = "Imposible actualizar la tabla System  : " & Parametro_Actualiza_System1
                myConnection.Close()
                Exit Function
            End If
            Dim ruta_almacenamiento As String = ""
            Dim RutaDisco As String = ""
            Dim carpealma As String = ""
            '--------Consolida ruta de alamacenamiento disco carpeta-----------
            RutaDisco = _Ruta_Almacenamiento & nombre_gabinete & DISC_new_version
            Result = Solicita_Carpeta_almacenamiento(carpealma,
                                                     numcarpvar,
                                                     ruta_almacenamiento,
                                                     RutaDisco)
            If Result <> "YES" Then
                Almacena_documento_nueva_version = Result
                myConnection.Close()
                Exit Function
            End If
            IDEX_new_version = numcarpvar
            Dim SQL_control_version As String = "SELECT  control_version " &
            " FROM ra_ver_control_version_documento where id_control_version_documento=" & id_control_version_documento &
            " for update"
            myCommand.CommandText = SQL_control_version
            mySqldatReade_CONTROL = myCommand.ExecuteReader()
            If mySqldatReade_CONTROL Is Nothing Then
                Almacena_documento_nueva_version = "Error funcion Almacena_documento_migrado_nueva_version  conexión fallida"
                myConnection.Close()
                Exit Function
            End If
            If mySqldatReade_CONTROL.HasRows = False Then
                Almacena_documento_nueva_version = "Imposible Encontrar el registro de control de migracion"
                mySqldatReade_CONTROL.Close()
                myConnection.Close()
                Exit Function
            End If
            mySqldatReade_CONTROL.Read()
            Dim control_version = mySqldatReade_CONTROL.Item("control_version")
            '------Si la versión del gabinete no esta registrada como versión principal aumenta el control de version en 1 para dejar el espacio de la primera versión
            If id_registro_version_activo = 0 And control_version = 0 Then
                control_version = control_version + 1
            End If
            mySqldatReade_CONTROL.Close()
            'Inactiva version activa de gabinete en el registro de version si esta activa la opcion de remplazo
            If option_remplaza_gabinete = 1 Then
                Dim SQL_inactiva_version_gabinete As String = "UPDATE ra_ver_version_documento SET ESTADO_ACTIVO_GABINETE=0 " &
                 " WHERE id_registro_version=" & id_registro_version_activo
                If id_registro_version_activo <> 0 Then
                    myCommand.CommandText = SQL_inactiva_version_gabinete
                    Switc = myCommand.ExecuteNonQuery()
                    If Switc = 0 Then
                        Almacena_documento_nueva_version = "Imposible inactivar las activas en el gabinete  : " & SQL_inactiva_version_gabinete
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    End If
                End If
            End If
            'Registra la versión del documento a remplazar
            Dim id_registro_version_doc_remplazo As Integer = 0
            Dim id_registro_version_db_remplazo As Long = 0
            If id_registro_version_activo = 0 And option_remplaza_gabinete = 1 Then
                id_registro_version_doc_remplazo = control_version
                Dim SQL_insert_version_remplazo As String = "Insert into ra_ver_version_documento (system1_id_gabinete,ra_mig_reg_mig_id_registro_migracion," &
               "fecha_registro_version,nombre_gabinete,id_version_doc,id_usuario_da,id_usuario_gestion,ID,DISC,PAG,DBT,IDEX,USER_DA,CTRL_ACES," &
               "PESO_DOCUMENTO,TIPO_ARCHIVO,PAGINAS_DOCUMENT,ESTADO_FIRMA_DIGITAL) VALUES (" & id_gabinete & "," & "0" & ",'" & DATE_rem_version & "','" & nombre_gabinete &
               "'," & control_version & "," & id_usuario_da & "," & id_usuario_gestion & "," & ID_rem_version & "," & DISC_rem_version & "," &
               PAG_rem_version & "," & DBT_rem_version & "," & IDEX_rem_version & ",'" & USER_DA_rem_version & "'," & CTRL_ACES_rem_version &
               ",'" & PESO_DOCUMENTO_rem_version & "','" & TIPO_ARCHIVO_rem_version & "', " & PAGINA_DOCUMENT_rem_version & "," & ESTADO_FIRMA_DIGITAL_rem_version & ")"
                myCommand.CommandText = SQL_insert_version_remplazo
                Switc = myCommand.ExecuteNonQuery()
                If Switc = 0 Then
                    Almacena_documento_nueva_version = "Imposible registrar la version del documento a remplazar  : " & SQL_insert_version_remplazo
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
                id_registro_version_db_remplazo = myCommand.LastInsertedId
            Else
                id_registro_version_doc_remplazo = ID_VERSION_DOC_rem_version
                id_registro_version_db_remplazo = ID_REGISTRO_VERSION_rem_version
            End If
            'Registra nueva version documento 
            Dim id_registro_version_doc_new As Integer = 0
            Dim id_registro_version_db_new As Long = 0
            control_version = control_version + 1
            id_registro_version_doc_new = control_version
            Dim SQL_insert_version_new As String = "Insert into ra_ver_version_documento (system1_id_gabinete,ra_mig_reg_mig_id_registro_migracion," &
               "fecha_registro_version,nombre_gabinete,id_version_doc,id_usuario_da,id_usuario_gestion,ID,DISC,PAG,DBT,IDEX,USER_DA,CTRL_ACES," &
               "PESO_DOCUMENTO,TIPO_ARCHIVO,PAGINAS_DOCUMENT,ESTADO_FIRMA_DIGITAL,ESTADO_ACTIVO_GABINETE) VALUES (" & id_gabinete & "," & "0" & ",'" & fecha_registro & "','" & nombre_gabinete &
               "'," & control_version & "," & id_usuario_da & "," & id_usuario_gestion & "," & ID_new_version & "," & DISC_new_version & "," &
               PAG_new_version & "," & DBT_new_version & "," & IDEX_new_version & ",'" & USER_DA_new_version & "'," & CTRL_ACES_new_version &
               ",'" & PESO_DOCUMENTO_new_version & "','" & TIPO_ARCHIVO_new_version & "'," & PAGINA_DOCUMENT_new_version & "," & ESTADO_FIRMA_DIGITAL_new_version & "," & option_remplaza_gabinete & ")"
            myCommand.CommandText = SQL_insert_version_new
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Almacena_documento_nueva_version = "Imposible registrar la version del documento a remplazante  : " & SQL_insert_version_new
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '----Devuelve los datos para registro en la interface de version
            item_list.id_registro_version = myCommand.LastInsertedId
            item_list.id_version_doc = control_version
            item_list.fecha_registro_version = fecha_registro
            item_list.error_sistema = "YES"
            If option_remplaza_gabinete = 0 Then
                item_list.ESTADO_ACTIVO_GABINETE = ""
            Else
                item_list.ESTADO_ACTIVO_GABINETE = "SI"
            End If
            item_list.id_registro_version_old = id_registro_version_activo
            item_list.TIPO_ARCHIVO = TIPO_ARCHIVO_new_version
            item_list.ID = id_imagen
            item_list.DBT = DBT_new_version
            Extension_documento = TIPO_ARCHIVO_new_version
            id_registro_version_db_new = myCommand.LastInsertedId
            'Actualiza el registro de control de version del documento
            Dim SQL_update_control_version As String = "update ra_ver_control_version_documento set control_version=" & control_version &
                " where id_control_version_documento=" & id_control_version_documento
            myCommand.CommandText = SQL_update_control_version
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Almacena_documento_nueva_version = "Imposible actualizar el control de version del documento   : " & SQL_update_control_version
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            'Actualiza registro de gabinete
            If option_remplaza_gabinete = 1 Then
                Dim SQL_update_registro_gabinete As String = "update " & nombre_gabinete & " set DISC=" & DISC_new_version & " , PAG=" & PAG_new_version &
               " , DBT=" & DBT_new_version & " , IDEX=" & IDEX_new_version & " , DA_MIG=" & "0" &
               " , ID_REGISTRO_VERSION=" & id_registro_version_db_new & " ,ID_VERSION_DOC=" & id_registro_version_doc_new &
               " , ESTADO_FIRMA_DIGITAL=" & ESTADO_FIRMA_DIGITAL_new_version &
               " where ID=" & id_imagen
                myCommand.CommandText = SQL_update_registro_gabinete
                Switc = myCommand.ExecuteNonQuery()
                If Switc = 0 Then
                    Almacena_documento_nueva_version = "Imposible actualizar el registro de gabinete   : " & SQL_update_registro_gabinete
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            '//--------------Actualiza versión en el registro de firma digital----////  
            If Stru_paramter_image.ESTADO_FIRMA_DIGITAL = 1 And Stru_paramter_image.ID_REGISTRO_VERSION = 0 And option_remplaza_gabinete = 1 Then
                Dim SQLpdate = "Update ra_cert_registro_certificado_archivo set id_registro_version=" & id_registro_version_db_remplazo &
                 "  where registro_producion_documental_ID_REGISTRO=" & id_registro_producion &
                " And id_registro_version=" & Stru_paramter_image.ID_REGISTRO_VERSION
                myCommand.CommandText = SQLpdate
                Switc = myCommand.ExecuteNonQuery()
                If Switc = 0 Then
                    Almacena_documento_nueva_version = "Imposible actualizar el registro de estado firma   : " & SQLpdate
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            'Actualiza registro produccion
            If id_registro_producion <> 0 Then
                Dim SQL_actualiza_registro_produccion As String = "update registro_producion_documental Set FORMATO='" & TIPO_ARCHIVO_new_version & "' , " &
               "TAMANO='" & PESO_DOCUMENTO_new_version & "', ESTADO_FIRMA_DIGITAL=" & ESTADO_FIRMA_DIGITAL_new_version & " where ID_REGISTRO_PRODUCION_DOCUMENTAL=" & id_registro_producion
                myCommand.CommandText = SQL_actualiza_registro_produccion
                Switc = myCommand.ExecuteNonQuery()
                If Switc = 0 Then
                    Almacena_documento_nueva_version = "Imposible actualizar el registro de produccion documental   : " & SQL_actualiza_registro_produccion
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            Dim Ruta_Alamce_Image As String = ruta_almacenamiento & "\"
            'Actualiza indice base de datos expediente
            Dim SQL_update_actualiza_indice_expediente As String = ""
            Dim Ruta_indice_documento As String = Ruta_Alamce_Image & NameNEWarchivo
            Dim file_inf_name As New FileInfo(Ruta_indice_documento)
            Ruta_indice_documento = Ruta_indice_documento.Replace("\", "/")
            If id_registro_producion <> 0 Then
                If Not Stru_expediente Is Nothing Then
                    If Stru_expediente(0).estado_expediente_electronico = 2 Then
                        SQL_update_actualiza_indice_expediente = "update ra_cert_indice_expediente set formato='" & TIPO_ARCHIVO_new_version & "' , " &
                        "dimension_kb='" & PESO_DOCUMENTO_new_version & "' , ruta_documento='" & Ruta_indice_documento & "' " &
                        " , Nombre_documento='" & file_inf_name.Name & "' " &
                        " where registro_producion_documental_ID_REGISTRO_PRODUCION_DOCUMENTAL=" & id_registro_producion
                        myCommand.CommandText = SQL_update_actualiza_indice_expediente
                        Switc = myCommand.ExecuteNonQuery()
                        If Switc = 0 Then
                            Almacena_documento_nueva_version = "Imposible actualizar el registro del indice del expediente   : " & SQL_update_actualiza_indice_expediente
                            myTrans.Rollback()
                            myConnection.Close()
                            Exit Function
                        End If
                        Dim Stru_values_cambio_indice() As ClassGaExpediente.stru_values_cambio_indice
                        ReDim Preserve Stru_values_cambio_indice(0)
                        Stru_values_cambio_indice(0).clave_index = "Formato"
                        Stru_values_cambio_indice(0).value_index = TIPO_ARCHIVO_new_version
                        ReDim Preserve Stru_values_cambio_indice(1)
                        Stru_values_cambio_indice(1).clave_index = "Tamano"
                        Stru_values_cambio_indice(1).value_index = PESO_DOCUMENTO_new_version
                        ReDim Preserve Stru_values_cambio_indice(2)
                        Stru_values_cambio_indice(2).clave_index = "Nombre_Documento"
                        Stru_values_cambio_indice(2).value_index = file_inf_name.Name
                        Result = ClassGaExpediente.Actualiza_campos_indice_expediente_xml_expediente(Ruta_archivo_xml,
                                                                                                     id_registro_producion,
                                                                                                     Stru_values_cambio_indice)
                        If Result <> "YES" Then
                            Almacena_documento_nueva_version = "Imposible actualizar el archivo del indice del expediente   : " & Result
                            myTrans.Rollback()
                            myConnection.Close()
                            Exit Function
                        End If
                    End If
                End If
            End If
            Result = Copia_imagen_version_documento(Ruta_Alamce_Image,
                                                    id_imagen,
                                                    matri_documentos,
                                                    DBT_new_version,
                                                    option_elimina_archivo)
            If Result <> "YES" Then
                Almacena_documento_nueva_version = "Imposible copiar los documentos " & Result
                Exit Function
            End If
            myTrans.Commit()
            Almacena_documento_nueva_version = "YES"
        Catch e As Exception
            Try

            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    myTrans.Rollback()
                    myConnection.Close()
                    Almacena_documento_nueva_version = "An exception of type " + ex.GetType().ToString() +
                                              " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try
            If Not myTrans Is Nothing Then
                myTrans.Rollback()
            End If
            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Almacena_documento_nueva_version = "Error General " & e.Message
            Exit Function
        End Try
    End Function

    Function Almacena_documento_migrado_nueva_version(ByVal Stru_registro_migracion As stru_registro_migracion,
                                                      ByVal matri_documemtos_gabinete() As String,
                                                      ByVal id_usuario_gestion As Integer,
                                                      ByVal id_usuario_da As Integer,
                                                      ByVal logui_usuario_gestion As String,
                                                      ByVal logui_usuario_da As String,
                                                      ByRef Extension_documento As String) As String
        '--------------------------------------------------------------------------------
        'Funcion : función que almacena documento de remplazo de migración
        '          
        '--------------------------------------------------------------------------------
        '                           PARAMETROS  
        '--------------------------------------------------------------------------------
        'id_imagen                 : Representa la identificación de la imagen dentro del
        '                            gabinete.
        'gabinete                  : Representa el nombre del gabinete.
        'id_usuario_da             : Representa la identificación del usuario docuarchi
        'Stru_registro_migracion   : Representa la estructura del registro de migración
        'logui_usuario_gestion     : Representa el login del usuario de gestion
        'logui_usuario_da          : Representa el logion del usuaario docuarchi
        'matri_documemtos_gabinete : Representa la matris de documentos migrados
        '--------------------------------------------------------------------------------
        '                           RETORNO
        '--------------------------------------------------------------------------------
        'Extension             : Retorna la extensión del archivo migrado
        '                                      
        '--------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '--------------------------------------------------------------------------------
        'Fecha                 : 2024-07-01
        'Elabora               : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------------------------
        Dim Result As String = ""
        Dim Class_SYSTEM1RUT As New Class_SYSTEM1RUT
        Dim _Ruta_Almacenamiento As String = ""
        Result = Class_SYSTEM1RUT.Consulta_Ruta_Almacenamiento(_Ruta_Almacenamiento,
                                                               Stru_registro_migracion.nombre_gabinete)
        If Result <> "YES" Then
            Almacena_documento_migrado_nueva_version = Result
            Exit Function
        End If
        Dim Class_fyle_system As New Class_fyle_system
        Dim ruta_documento As String = Stru_registro_migracion.ruta_documento.Replace("/", "\")
        Dim peso_archivo As String = ""
        Dim matri_documentos_() As String = Nothing
        ReDim Preserve matri_documentos_(0)
        matri_documentos_(0) = ruta_documento
        Dim Numero_Pag = matri_documentos_.Length  'Obtien el numero de archivos a guardar en el disco
        Result = Class_fyle_system.Solicita_peso_matriz_documentos(matri_documentos_,
                                                                   peso_archivo)
        If Result <> "YES" Then
            Almacena_documento_migrado_nueva_version = Result
            Exit Function
        End If
        Dim ClassGestionFechas As New ClassGestionFechas
        Dim time1al As String = Date.Now.ToString
        ClassGestionFechas.Formatea_Fecha_Almacenamiento_Time(time1al)
        Dim fecha_registro As String = time1al
        Dim TempoFecha As String = Left(time1al, 10)
        time1al = Trim(time1al.Replace(TempoFecha, ""))
        Dim ClassDaGabinete As New ClassDaGabinete
        Dim gabinete As String = Stru_registro_migracion.nombre_gabinete
        Dim id_imagen As Integer = Stru_registro_migracion.id_imagen
        Dim id_gabinete As Integer = Stru_registro_migracion.system1_id_gabinete
        Dim id_registro_migracion As Integer = Stru_registro_migracion.id_registro_migracion
        Dim tipo_migracion As Integer = Stru_registro_migracion.tipo_migracion
        Dim Stru_paramter_image As stru_paramter_image = Nothing
        Result = ClassDaGabinete.SolicitaEtructuraImagenGabinete(gabinete,
                                                                 id_imagen,
                                                                 Stru_paramter_image,
                                                                 1,
                                                                 1,
                                                                 1)
        If Result <> "YES" Then
            Almacena_documento_migrado_nueva_version = Result
            Exit Function
        End If
        Dim Class_DETALLE_GABIENETE As New Class_DETALLE_GABIENETE
        Dim nombre_campo_valor_gabinete As String = ""
        Result = Class_DETALLE_GABIENETE.Solicita_nombre_campo_valor_gabinete(gabinete,
                                                                              nombre_campo_valor_gabinete)
        If Result <> "YES" Then
            Almacena_documento_migrado_nueva_version = Result
            Exit Function
        End If
        Dim valor_campo_gabinete As String = ""
        Dim ref_valor_campo_gabinete As String = "null"
        Result = ClassDaGabinete.Solicita_valor_campo_gebinete(id_imagen,
                                                               gabinete,
                                                               nombre_campo_valor_gabinete,
                                                               valor_campo_gabinete)
        If Result <> "YES" Then
            Almacena_documento_migrado_nueva_version = Result
            Exit Function
        End If
        If valor_campo_gabinete <> "" Then
            ref_valor_campo_gabinete = "'" & valor_campo_gabinete & "'"
        End If

        '--------Parametros de registro de nueva version de documentos
        Dim SQL_registro_version_nuevo As String = ""
        Dim ID_new_version As Integer = 0
        Dim DISC_new_version As Integer = 0
        Dim PAG_new_version As Integer = 0
        Dim DBT_new_version As Integer = 0
        Dim IDEX_new_version As Integer = 0
        Dim USER_DA_new_version As String = ""
        Dim CTRL_ACES_new_version As Integer = 0
        Dim PESO_DOCUMENTO_new_version As String = ""
        Dim TIPO_ARCHIVO_new_version As String = ""
        Dim ID_REG_MIGRA_new_version As Long = 0
        Dim PAGINA_DOCUMENT_new_version As Integer = 0
        Dim ESTADO_FIRMA_DIGITAL_new_version As Integer = 0
        PAG_new_version = Stru_registro_migracion.num_page_nuevo
        PAGINA_DOCUMENT_new_version = Stru_registro_migracion.num_page_nuevo
        ID_new_version = Stru_registro_migracion.id_imagen
        ID_REG_MIGRA_new_version = Stru_registro_migracion.id_registro_migracion
        USER_DA_new_version = logui_usuario_da
        CTRL_ACES_new_version = Stru_paramter_image.CTRL_ACES
        PESO_DOCUMENTO_new_version = Stru_registro_migracion.leng_file
        ESTADO_FIRMA_DIGITAL_new_version = Stru_registro_migracion.ESTADO_FIRMA_DIGITAL
        '-----------Solicita el tipo de documento del nuevo documento migrado
        Dim Class_da_extension As New Class_da_extension
        Dim File_inf As New FileInfo(ruta_documento)
        Dim tipo_documento_gabinete As Integer = 0
        Result = Class_da_extension.SolicitaTipoArchivoDocuarchiExtension(UCase(File_inf.Extension),
                                                                      DBT_new_version)
        If Result <> "YES" Then
            Almacena_documento_migrado_nueva_version = Result
            Exit Function
        End If
        TIPO_ARCHIVO_new_version = UCase(File_inf.Extension)
        '--------Parametros de registro  version documento remplazo
        Dim SQL_registro_version_remplazo As String = ""
        Dim ID_rem_version As Integer = 0
        Dim DISC_rem_version As Integer = 0
        Dim PAG_rem_version As Integer = 0
        Dim DBT_rem_version As Integer = 0
        Dim IDEX_rem_version As Integer = 0
        Dim USER_DA_rem_version As String = ""
        Dim CTRL_ACES_rem_version As Integer = 0
        Dim PESO_DOCUMENTO_rem_version As String = ""
        Dim TIPO_ARCHIVO_rem_version As String = ""
        Dim ID_REG_MIGRA_rem_version As Long = 0
        Dim ID_REGISTRO_VERSION_rem_version As Integer = 0
        Dim ID_VERSION_DOC_rem_version As Integer = 0
        Dim PAGINA_DOCUMENT_rem_version As Integer = 0
        Dim ESTADO_FIRMA_DIGITAL_rem_version As Integer = 0
        Dim DATE_rem_version As String = ""
        Dim datetemp As String = Stru_paramter_image.DATE1 & " " & Stru_paramter_image.TIME1
        ClassGestionFechas.Formatea_fecha_time_db(datetemp, DATE_rem_version)
        ID_rem_version = id_imagen
        DISC_rem_version = Stru_paramter_image.DISC
        PAG_rem_version = Stru_paramter_image.PAG
        DBT_rem_version = Stru_paramter_image.DBT_TIPO_IMAGEN
        IDEX_rem_version = Stru_paramter_image.IDEX
        USER_DA_rem_version = Stru_paramter_image.USER
        CTRL_ACES_rem_version = Stru_paramter_image.CTRL_ACES
        ID_REGISTRO_VERSION_rem_version = Stru_paramter_image.ID_REGISTRO_VERSION
        ID_VERSION_DOC_rem_version = Stru_paramter_image.ID_VERSION_DOC
        ESTADO_FIRMA_DIGITAL_rem_version = Stru_paramter_image.ESTADO_FIRMA_DIGITAL
        Dim ref_matri_documento() As String = Nothing
        Dim Icont As Integer = 0
        For i As Integer = 1 To matri_documemtos_gabinete.Length - 1
            ReDim Preserve ref_matri_documento(Icont)
            ref_matri_documento(Icont) = matri_documemtos_gabinete(i)
            Icont = Icont + 1
        Next
        '----------Solicita perso documento gabinete a remplazar
        Result = Class_fyle_system.Solicita_peso_matriz_documentos(ref_matri_documento,
                                                                   PESO_DOCUMENTO_rem_version)
        If Result <> "YES" Then
            Almacena_documento_migrado_nueva_version = Result
            Exit Function
        End If
        If Stru_paramter_image.DBT_TIPO_IMAGEN = -10 Then
            PAGINA_DOCUMENT_rem_version = ref_matri_documento.Length - 1
        Else
            PAGINA_DOCUMENT_rem_version = Stru_paramter_image.PAG
        End If
        '----------Solicita la extension o tipo documento del gabinete
        Dim file_inf_rem As New FileInfo(ref_matri_documento(0))
        TIPO_ARCHIVO_rem_version = UCase(file_inf_rem.Extension)
        Dim ClassGaProducionDocumental As New ClassGaProducionDocumental
        Dim id_registro_producion As Long = 0
        Result = ClassGaProducionDocumental.Solicita_id_registro_producion_documental(id_imagen,
                                                                                      gabinete,
                                                                                      id_registro_producion)
        If Result <> "YES" Then
            Almacena_documento_migrado_nueva_version = Result
            Exit Function
        End If
        Dim Stru_produccion_indice As stru_produccion_indice = Nothing
        Dim Stru_expediente() As expediente_conservacion = Nothing
        Dim Ruta_archivo_xml As String = ""
        Dim ClassGaExpediente As New ClassGaExpediente
        If id_registro_producion <> 0 Then
            Result = ClassGaProducionDocumental.Solicita_estructura_id_registro_produccion(id_registro_producion,
                                                                                           Stru_produccion_indice)
            If Result <> "YES" Then
                Almacena_documento_migrado_nueva_version = Result
                Exit Function
            End If
            If Stru_produccion_indice.EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE <> 0 Then
                Result = ClassGaExpediente.SolicitaDatosEstructuraExpediente(Stru_produccion_indice.EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE,
                                                                                           Stru_expediente)
                If Result <> "YES" Then
                    Almacena_documento_migrado_nueva_version = Result
                    Exit Function
                End If
                Result = ClassGaExpediente.Solicita_archivo_indice_expediente(Stru_produccion_indice.EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE,
                                                                              Ruta_archivo_xml)
                If Result <> "YES" Then
                    Almacena_documento_migrado_nueva_version = Result
                    Exit Function
                End If
                If File.Exists(Ruta_archivo_xml) = False Then
                    Almacena_documento_migrado_nueva_version = "Imposible encontrar el archivo indice del expediente (" & Ruta_archivo_xml & ")"
                    Exit Function
                End If
            End If
        End If
        Dim Class_system1 As New Class_system1
        Dim Stru_datos_configuracion_gabinete As stru_datos_configuracion_gabinete = Nothing
        Result = Class_system1.Solicita_datos_configuracion_gabinete(id_gabinete,
                                                                     Stru_datos_configuracion_gabinete)
        If Result <> "YES" Then
            Almacena_documento_migrado_nueva_version = Result
            Exit Function
        End If
        DISC_new_version = Stru_datos_configuracion_gabinete.disco
        Dim Class_disco_detalle As New Class_disco_detalle
        Dim estado_disco As String = ""
        Result = Class_disco_detalle.Numero_Imagenes(gabinete,
                                                     Stru_datos_configuracion_gabinete.tamdisc,
                                                     Stru_datos_configuracion_gabinete.disco,
                                                     estado_disco)
        If Result <> "YES" Then
            Almacena_documento_migrado_nueva_version = Result
            Exit Function
        End If
        If estado_disco = "SL" Then
            Almacena_documento_migrado_nueva_version = "El disco (" & Stru_datos_configuracion_gabinete.disco & ") sobre paso el limite de capacidad"
            Exit Function
        End If
        Dim Class_ra_ver_control_version_documento As New Class_ra_ver_control_version_documento
        Dim id_control_version_documento As Long = 0
        Result = Class_ra_ver_control_version_documento.Solicita_id_registro_control_version(id_gabinete,
                                                                                             id_imagen,
                                                                                             id_control_version_documento)
        If Result <> "YES" Then
            Almacena_documento_migrado_nueva_version = Result
            Exit Function
        End If
        If id_control_version_documento = 0 Then
            Result = Class_ra_ver_control_version_documento.Registra_control_version_documento(id_gabinete,
                                                                                               id_imagen,
                                                                                               fecha_registro,
                                                                                               id_control_version_documento)
            If Result <> "YES" Then
                Almacena_documento_migrado_nueva_version = Result
                Exit Function
            End If
        End If
        Dim ZeroFillImagen As String = ""
        Dim NameNEWarchivo As String = ""
        Result = Ceros_Imagen_Almacenada(ZeroFillImagen,
                                         id_imagen)
        If Result <> "YES" Then
            Almacena_documento_migrado_nueva_version = "Error generando ZerroFill imagen "
            Exit Function
        End If
        NameNEWarchivo = "DIG" & ZeroFillImagen & id_imagen & TIPO_ARCHIVO_new_version
        Dim Class_ra_ver_version_documento As New Class_ra_ver_version_documento
        Dim Stru_registro_version_documento() As stru_registro_version_documento = Nothing
        Result = Class_ra_ver_version_documento.Solicita_lista_discos_carpetas(id_imagen,
                                                                               id_gabinete,
                                                                               Stru_registro_version_documento)
        If Result <> "YES" Then
            Almacena_documento_migrado_nueva_version = Result
            Exit Function
        End If
        Dim id_registro_version As Long = 0
        Result = Class_ra_ver_version_documento.Solicita_registro_activo_gabinete(id_imagen,
                                                                                  id_gabinete,
                                                                                  id_registro_version)
        If Result <> "YES" Then
            Almacena_documento_migrado_nueva_version = Result
            Exit Function
        End If
        Dim mySqldatReader As MySqlDataReader
        Dim mySqldatReade_CONTROL As MySqlDataReader
        Dim myConnection As New MySqlConnection
        Dim myTrans As MySqlTransaction
        Dim ref As New conect.Dbase_Conction_Mysql_DA
        Result = ref.Returna_Conexion_Mysql(myConnection)
        If Result <> "YES" Then
            Almacena_documento_migrado_nueva_version = Result
            Exit Function
        End If
        Try
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            Dim Parametro_Select_System1 As String = " SELECT disco,proxid,tamdisc,numcarp,NUMPAG_CARP FROM system1 where nombre = " _
                & "'" & gabinete & "' " & "for update"
            myCommand.CommandText = Parametro_Select_System1
            mySqldatReader = myCommand.ExecuteReader()
            If mySqldatReader Is Nothing Then
                Almacena_documento_migrado_nueva_version = "Imposible Encontrar Registro En Tabla Systema Error Conexion"
                myConnection.Close()
                Exit Function
            End If
            If mySqldatReader.HasRows = False Then
                Almacena_documento_migrado_nueva_version = "Imposible Encontrar Registro En Tabla Systema"
                mySqldatReader.Close()
                myConnection.Close()
                Exit Function
            End If
            mySqldatReader.Read()
            Dim numcarpvar = mySqldatReader.Item("NUMCARP")
            Dim nuncarpvarREF = mySqldatReader.Item("NUMCARP")
            Dim Numero_Pagina_Carp = mySqldatReader.Item("NUMPAG_CARP")
            DISC_new_version = mySqldatReader.Item("disco")
            mySqldatReader.Close()
            '*****************************************************
            'Actualizacion para determinar el numero de imagenes
            'por base de datos para evitar contar los archivos
            'del directorio y agilizar la funcion
            '****************************************************
            Dim Valor_Suma_Imagen As Integer = Numero_Pag + Numero_Pagina_Carp
            If Valor_Suma_Imagen > 230 Then
                numcarpvar = numcarpvar + 1
                Numero_Pagina_Carp = Numero_Pag
            Else
                Numero_Pagina_Carp = Valor_Suma_Imagen
            End If
            '---------------------------------------------------------
            'Valida que los documemtos no esten en las misma carpeta
            'e incrementa el numero de la carpeta a almacenar
            '---------------------------------------------------------
            If IDEX_rem_version = numcarpvar Then
                numcarpvar = numcarpvar + 1
            End If
            If Not Stru_registro_version_documento Is Nothing Then
                For i As Integer = 0 To Stru_registro_version_documento.Length - 1
                    If Stru_registro_version_documento(i).DISC = DISC_new_version And Stru_registro_version_documento(i).IDEX = numcarpvar Then
                        numcarpvar = numcarpvar + 1
                    End If
                Next
            End If
            Dim Parametro_Actualiza_System1 As String = "update system1 set numcarp = " & "'" & numcarpvar & "', NUMPAG_CARP=" & Numero_Pagina_Carp & " where nombre =" & "'" &
                    gabinete & "'"
            myCommand.CommandText = Parametro_Actualiza_System1
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Almacena_documento_migrado_nueva_version = "Imposible actualizar la tabla System  : " & Parametro_Actualiza_System1
                myConnection.Close()
                Exit Function
            End If
            Dim ruta_almacenamiento As String = ""
            Dim RutaDisco As String = ""
            Dim carpealma As String = ""
            '--------Consolida ruta de carpeta de almacenamiento
            RutaDisco = _Ruta_Almacenamiento & gabinete & DISC_new_version
            Result = Solicita_Carpeta_almacenamiento(carpealma,
                                                     numcarpvar,
                                                     ruta_almacenamiento,
                                                     RutaDisco)
            If Result <> "YES" Then
                Almacena_documento_migrado_nueva_version = Result
                myConnection.Close()
                Exit Function
            End If
            IDEX_new_version = numcarpvar
            Dim SQL_control_version As String = "SELECT  control_version " &
            " FROM ra_ver_control_version_documento where id_control_version_documento=" & id_control_version_documento &
            " for update"
            myCommand.CommandText = SQL_control_version
            mySqldatReade_CONTROL = myCommand.ExecuteReader()
            If mySqldatReade_CONTROL Is Nothing Then
                Almacena_documento_migrado_nueva_version = "Error funcion Almacena_documento_migrado_nueva_version  conexión fallida"
                myConnection.Close()
                Exit Function
            End If
            If mySqldatReade_CONTROL.HasRows = False Then
                Almacena_documento_migrado_nueva_version = "Imposible Encontrar el registro de control de migracion"
                mySqldatReade_CONTROL.Close()
                myConnection.Close()
                Exit Function
            End If
            mySqldatReade_CONTROL.Read()
            Dim control_version = mySqldatReade_CONTROL.Item("control_version")
            mySqldatReade_CONTROL.Close()
            'Inactiva version activa de gabinete en el registro de version
            Dim SQL_inactiva_version_gabinete As String = "UPDATE ra_ver_version_documento SET ESTADO_ACTIVO_GABINETE=0 " &
                  " WHERE id_registro_version=" & id_registro_version
            If id_registro_version <> 0 Then
                myCommand.CommandText = SQL_inactiva_version_gabinete
                Switc = myCommand.ExecuteNonQuery()
                If Switc = 0 Then
                    Almacena_documento_migrado_nueva_version = "Imposible inactivar las activas en el gabinete  : " & SQL_inactiva_version_gabinete
                    'myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            'Registra la versión del documento remplazado
            Dim id_registro_version_doc_remplazo As Integer = 0
            Dim id_registro_version_db_remplazo As Long = 0
            If control_version = 0 Then
                control_version = control_version + 1
                id_registro_version_doc_remplazo = control_version
                Dim SQL_insert_version_remplazo As String = "Insert into ra_ver_version_documento (system1_id_gabinete,ra_mig_reg_mig_id_registro_migracion," &
               "fecha_registro_version,nombre_gabinete,id_version_doc,id_usuario_da,id_usuario_gestion,ID,DISC,PAG,DBT,IDEX,USER_DA,CTRL_ACES," &
               "PESO_DOCUMENTO,TIPO_ARCHIVO,PAGINAS_DOCUMENT,ESTADO_FIRMA_DIGITAL) VALUES (" & id_gabinete & "," & id_registro_migracion & ",'" & DATE_rem_version & "','" & gabinete &
               "'," & control_version & "," & id_usuario_da & "," & id_usuario_gestion & "," & ID_rem_version & "," & DISC_rem_version & "," &
               PAG_rem_version & "," & DBT_rem_version & "," & IDEX_rem_version & ",'" & USER_DA_rem_version & "'," & CTRL_ACES_rem_version &
               ",'" & PESO_DOCUMENTO_rem_version & "','" & TIPO_ARCHIVO_rem_version & "', " & PAGINA_DOCUMENT_rem_version & "," & ESTADO_FIRMA_DIGITAL_rem_version & ")"
                myCommand.CommandText = SQL_insert_version_remplazo
                Switc = myCommand.ExecuteNonQuery()
                If Switc = 0 Then
                    Almacena_documento_migrado_nueva_version = "Imposible registrar la version del documento a remplazar  : " & SQL_insert_version_remplazo
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
                id_registro_version_db_remplazo = myCommand.LastInsertedId
            Else
                id_registro_version_doc_remplazo = ID_VERSION_DOC_rem_version
                id_registro_version_db_remplazo = ID_REGISTRO_VERSION_rem_version
            End If
            'Registra version documento remplazante
            Dim id_registro_version_doc_new As Integer = 0
            Dim id_registro_version_db_new As Long = 0
            control_version = control_version + 1
            id_registro_version_doc_new = control_version
            Dim SQL_insert_version_new As String = "Insert into ra_ver_version_documento (system1_id_gabinete,ra_mig_reg_mig_id_registro_migracion," &
               "fecha_registro_version,nombre_gabinete,id_version_doc,id_usuario_da,id_usuario_gestion,ID,DISC,PAG,DBT,IDEX,USER_DA,CTRL_ACES," &
               "PESO_DOCUMENTO,TIPO_ARCHIVO,PAGINAS_DOCUMENT,ESTADO_FIRMA_DIGITAL,ESTADO_ACTIVO_GABINETE) VALUES (" & id_gabinete & "," & id_registro_migracion & ",'" & fecha_registro & "','" & gabinete &
               "'," & control_version & "," & id_usuario_da & "," & id_usuario_gestion & "," & ID_new_version & "," & DISC_new_version & "," &
               PAG_new_version & "," & DBT_new_version & "," & IDEX_new_version & ",'" & USER_DA_new_version & "'," & CTRL_ACES_new_version &
               ",'" & PESO_DOCUMENTO_new_version & "','" & TIPO_ARCHIVO_new_version & "'," & PAGINA_DOCUMENT_new_version & "," & ESTADO_FIRMA_DIGITAL_new_version & ",1)"
            myCommand.CommandText = SQL_insert_version_new
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Almacena_documento_migrado_nueva_version = "Imposible registrar la version del documento a remplazante  : " & SQL_insert_version_new
                'mySqldatReade_CONTROL.Close()
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            Extension_documento = TIPO_ARCHIVO_new_version
            id_registro_version_db_new = myCommand.LastInsertedId
            'Actualiza el registro de control de version del documento
            Dim SQL_update_control_version As String = "update ra_ver_control_version_documento set control_version=" & control_version &
                " where id_control_version_documento=" & id_control_version_documento
            myCommand.CommandText = SQL_update_control_version
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Almacena_documento_migrado_nueva_version = "Imposible actualizar el control de version del documento   : " & SQL_update_control_version
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            'Actualiza el registro de migracion 
            Dim SQL_update_registro_migracion As String = "update ra_mig_registro_migracion set id_registro_version_nuevo=" & id_registro_version_db_new &
                " , id_registro_version_anterior=" & id_registro_version_db_remplazo & " , estado_eliminado=1 , " & " fecha_registro_eliminado='" & fecha_registro &
                "', id_usuario_gestion_elimina=" & id_usuario_gestion & ", valor_campo_gabinete=" & ref_valor_campo_gabinete & " where id_registro_migracion=" & id_registro_migracion
            myCommand.CommandText = SQL_update_registro_migracion
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Almacena_documento_migrado_nueva_version = "Imposible actualizar el registro de migración   : " & SQL_update_registro_migracion
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            'Actualiza registro de gabinete
            Dim SQL_update_registro_gabinete As String = "update " & gabinete & " set DISC=" & DISC_new_version & " , PAG=" & PAG_new_version &
                " , DBT=" & DBT_new_version & " , IDEX=" & IDEX_new_version & " , DA_MIG=" & tipo_migracion &
                " , ID_REGISTRO_VERSION=" & id_registro_version_db_new & " ,ID_VERSION_DOC=" & id_registro_version_doc_new &
                " , ESTADO_FIRMA_DIGITAL=" & ESTADO_FIRMA_DIGITAL_new_version &
                " where ID=" & id_imagen
            myCommand.CommandText = SQL_update_registro_gabinete
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Almacena_documento_migrado_nueva_version = "Imposible actualizar el registro de gabinete   : " & SQL_update_registro_gabinete
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            'Actualiza registro produccion
            If id_registro_producion <> 0 Then
                Dim SQL_actualiza_registro_produccion As String = "update registro_producion_documental set FORMATO='" & TIPO_ARCHIVO_new_version & "' , " &
               "TAMANO='" & PESO_DOCUMENTO_new_version & "', ESTADO_FIRMA_DIGITAL=" & ESTADO_FIRMA_DIGITAL_new_version & " where ID_REGISTRO_PRODUCION_DOCUMENTAL=" & id_registro_producion
                myCommand.CommandText = SQL_actualiza_registro_produccion
                Switc = myCommand.ExecuteNonQuery()
                If Switc = 0 Then
                    Almacena_documento_migrado_nueva_version = "Imposible actualizar el registro de produccion documental   : " & SQL_actualiza_registro_produccion
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            Dim Ruta_Alamce_Image As String = ruta_almacenamiento & "\" & NameNEWarchivo
            Dim file_inf_name As New FileInfo(Ruta_Alamce_Image)
            'Actualiza indice base de datos expediente
            Dim SQL_update_actualiza_indice_expediente As String = ""
            Dim Ruta_indice_documento As String = Ruta_Alamce_Image
            Ruta_indice_documento = Ruta_indice_documento.Replace("\", "/")
            If id_registro_producion <> 0 Then
                If Not Stru_expediente Is Nothing Then
                    If Stru_expediente(0).estado_expediente_electronico = 2 Then
                        SQL_update_actualiza_indice_expediente = "update ra_cert_indice_expediente set formato='" & TIPO_ARCHIVO_new_version & "' , " &
                        "dimension_kb='" & PESO_DOCUMENTO_new_version & "' , ruta_documento='" & Ruta_indice_documento & "' " &
                        " , Nombre_documento='" & file_inf_name.Name & "' " &
                        " where registro_producion_documental_ID_REGISTRO_PRODUCION_DOCUMENTAL=" & id_registro_producion
                        myCommand.CommandText = SQL_update_actualiza_indice_expediente
                        Switc = myCommand.ExecuteNonQuery()
                        If Switc = 0 Then
                            Almacena_documento_migrado_nueva_version = "Imposible actualizar el registro del indice del expediente   : " & SQL_update_actualiza_indice_expediente
                            myTrans.Rollback()
                            myConnection.Close()
                            Exit Function
                        End If
                        Dim Stru_values_cambio_indice() As ClassGaExpediente.stru_values_cambio_indice
                        ReDim Preserve Stru_values_cambio_indice(0)
                        Stru_values_cambio_indice(0).clave_index = "Formato"
                        Stru_values_cambio_indice(0).value_index = TIPO_ARCHIVO_new_version
                        ReDim Preserve Stru_values_cambio_indice(1)
                        Stru_values_cambio_indice(1).clave_index = "Tamano"
                        Stru_values_cambio_indice(1).value_index = PESO_DOCUMENTO_new_version
                        ReDim Preserve Stru_values_cambio_indice(2)
                        Stru_values_cambio_indice(2).clave_index = "Nombre_Documento"
                        Stru_values_cambio_indice(2).value_index = file_inf_name.Name
                        Result = ClassGaExpediente.Actualiza_campos_indice_expediente_xml_expediente(Ruta_archivo_xml,
                                                                                                     id_registro_producion,
                                                                                                     Stru_values_cambio_indice)
                        If Result <> "YES" Then
                            Almacena_documento_migrado_nueva_version = "Imposible actualizar el archivo del indice del expediente   : " & Result
                            myTrans.Rollback()
                            myConnection.Close()
                            Exit Function
                        End If
                    End If
                End If
            End If
            File.Copy(ruta_documento, Ruta_Alamce_Image)
            File.Delete(ruta_documento)
            myTrans.Commit()
            Almacena_documento_migrado_nueva_version = "YES"
        Catch e As Exception
            Try

            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    myTrans.Rollback()
                    myConnection.Close()
                    Almacena_documento_migrado_nueva_version = "An exception of type " + ex.GetType().ToString() +
                                              " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try
            If Not myTrans Is Nothing Then
                myTrans.Rollback()
            End If
            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Almacena_documento_migrado_nueva_version = "Error General " & e.Message
            Exit Function
        End Try
    End Function
    Function Copia_imagen_version_documento(ByVal Ruta_alamcenamiento As String,
                                            ByVal id_imagen As Integer,
                                            ByVal Matri_Dcoumentos() As String,
                                            ByVal dbt_tipoDocumento As Integer,
                                            ByVal OpTionElimina As Integer) As String
        Try
            Dim Ceros_Cuerpo_Imag As String = ""
            Dim Result As String = ""
            Dim i As Integer = 0
            Dim i2 As Integer = 0
            Dim Ceros_Ext As String = ""
            Dim Imagen_Principal As String = ""
            Result = Ceros_Imagen_Almacenada(Ceros_Cuerpo_Imag,
                                             id_imagen)
            If Result <> "YES" Then
                Copia_imagen_version_documento = "Error generando zero fill de imagen " & Result
                Exit Function
            End If
            Dim Extension As String = ""
            Dim visor As String = ""
            Dim Estado_doc As String = ""
            Imagen_Principal = "DIG" & Ceros_Cuerpo_Imag & id_imagen
            Dim Class_da_extension As New Class_da_extension
            Result = Class_da_extension.RetornaExtensionTipoDocumento(dbt_tipoDocumento,
                                                                      Extension)
            If Result <> "YES" Then
                Copia_imagen_version_documento = "Error determinando tipo documento " & Result
                Exit Function
            End If
            File.Copy(Matri_Dcoumentos(0), Ruta_alamcenamiento & Imagen_Principal & Extension)
            If Matri_Dcoumentos.Length > 1 Then
                For i = 1 To UBound(Matri_Dcoumentos)
                    Result = ""
                    Ceros_Ext = ""
                    Result = Ceros_Imagen_Alamacenada_ext(i2, Ceros_Ext)
                    If Result <> "YES" Then
                        Copia_imagen_version_documento = "Error generando ceros extension imagen "
                        Exit Function
                    End If
                    If Not Matri_Dcoumentos(i) Is Nothing Then
                        File.Copy(Matri_Dcoumentos(i), Ruta_alamcenamiento & Imagen_Principal & "." & Ceros_Ext & i2)
                    End If
                    i2 = i2 + 1
                Next
            End If
            If OpTionElimina = 1 Then
                For i = 0 To Matri_Dcoumentos.Length - 1
                    If File.Exists(Matri_Dcoumentos(i)) Then
                        Kill(Matri_Dcoumentos(i))
                    End If
                Next
            End If
            Copia_imagen_version_documento = "YES"
        Catch ex As Exception
            Copia_imagen_version_documento = "Inconsistencia general funcion Copia_imagen_version_documento " & ex.Message
        End Try
    End Function
    Function Almacenamiento(ByVal _Ruta_Carpeta As String,
    ByVal _Nombre_Documento As String, ByVal _Nombre_Gabienete As String,
    ByVal _Estado_Elimina As Integer, ByRef _Matri_Datos() As String,
    ByVal _Tipo_Alamcenamiento As Integer, ByVal Numero_Pag As Integer,
    ByVal Tipo_Doc As Integer, ByVal Matri_Dcoumentos() As String, ByVal Evalua_Campo_Obli As Integer,
    ByRef Id_Almacen As Integer, ByVal Tipo_Doc_Añade As Integer, ByVal id_empresa As Integer, Optional ByVal id_usuario_gestion As Integer = 0 _
    , Optional ByVal id_area As Integer = 0, Optional ByVal id_serie As Integer = 0,
    Optional ByVal id_sub_serie As Integer = 0, Optional ByVal id_tipo_documento As Integer = 0,
    Optional ByVal id_expediente As Integer = 0,
    Optional ByVal id_tipo_expediente As Integer = 0, Optional ByVal id_unidad_conservacion As Integer = 0,
    Optional ByVal id_tipo_unidad_conservacion As Integer = 0,
    Optional ByVal id_clase_documento As Integer = 0, Optional ByVal expediente As String = "", Optional ByVal nombre_serie As String = "",
    Optional ByVal nombre_sub_serie As String = "", Optional ByVal tipo_documento As String = "",
    Optional ByVal unidad_conserva As String = "", Optional ByVal clase_documento As String = "", Optional ByVal fecha_elaboracion As String = "",
    Optional ByVal radicado As String = "", Optional ByVal sugundo_nombre_documento As String = "", Optional ByRef id_registro_producion As Object = 0,
    Optional tipo_archivo_producion As Integer = 0, Optional ByRef estado_firma_digtal As Integer = 0, Optional ByVal id_tarea_workflow As Long = 0,
                            Optional ByVal id_ruta_workflow As Integer = 0)
        Dim Result As String = ""
        Dim _Ruta_Almacenamiento As String = ""
        Result = ""
        'If ESTADOFILESERVER = 1 Then
        Dim ref_Class_SYSTEM1RUT As New Class_SYSTEM1RUT
        Result = ref_Class_SYSTEM1RUT.Consulta_Ruta_Almacenamiento(_Ruta_Almacenamiento,
                                                                   _Nombre_Gabienete)
        If Result <> "YES" Then
            Almacenamiento = Result
            Exit Function
        End If
        '****************************************
        'Verifica la existencia del archivo 
        'Preindex si es almacenamineto por bach
        'se evalua tipo almacenamiento variable
        '_Tipo_Alamcenamiento si es uno almacena
        'por bach y es necesario leer el archivo
        'para los datos
        '*****************************************
        Dim Nombre_Archivo_Preindex As String = ""
        Dim Tipo_Archivo As String = ""
        '*********************************
        'Determina si se lee archivo 
        'Preindex
        '*********************************
        If _Tipo_Alamcenamiento = 1 Then
            '*************************************
            'Contruye nombre archivo Preindex
            '*************************************
            Dim Ref_Nombre_Documento As String = ""
            Dim Ceros_Documento As String = ""
            Result = Contruye_Nombre_Archvio_Index(_Ruta_Carpeta,
                                                   _Nombre_Documento,
                                                   Ceros_Documento,
                                                   Ref_Nombre_Documento)
            If Result <> "YES" Then
                Almacenamiento = "Error Construyendo nombre del archivo preindex Archivo preindex " & Result
                Exit Function
            End If
            If File.Exists(_Ruta_Carpeta & Ceros_Documento & Ref_Nombre_Documento & ".xmls") = True Then
                Nombre_Archivo_Preindex = _Ruta_Carpeta & Ceros_Documento & Ref_Nombre_Documento & ".xmls"
                Tipo_Archivo = ".xmls"
            End If
            If File.Exists(_Ruta_Carpeta & Ceros_Documento & Ref_Nombre_Documento & ".txt") = True Then
                Nombre_Archivo_Preindex = _Ruta_Carpeta & Ceros_Documento & Ref_Nombre_Documento & ".txt"
                Tipo_Archivo = ".txt"
            End If
            If Nombre_Archivo_Preindex = "" Then
                Almacenamiento = "Documento sin preindexacion"
                Exit Function
            End If
            '****************************
            'Lee archivo Preindex (txt)
            '****************************
            If Tipo_Archivo = ".txt" Then
                Result = Leer_Archivo_Preindex(Nombre_Archivo_Preindex,
                                               _Matri_Datos)
                If Result <> "YES" Then
                    Almacenamiento = "Imposible Leer Archivo Preindex " & Nombre_Archivo_Preindex & " Error " & Result
                    Exit Function
                End If
            End If
        End If
        '**************************************************************
        'codigo que permite verificar que los datos de los campos
        'obligatorios contengan la informacion del archivo
        '**************************************************************
        Dim Matri_Campos_Obli() As String
        Erase Matri_Campos_Obli
        Result = ""
        Dim ref_Class_DETALLE_GABIENETE As New Class_DETALLE_GABIENETE
        Result = ref_Class_DETALLE_GABIENETE.Consulta_Campos_Obligatorio(_Nombre_Gabienete,
                                                                         Matri_Campos_Obli)
        If Result <> "YES" Then
            Almacenamiento = "Imposible Encontrar datos para campos obligatorios Error " & Result
            Exit Function
        End If
        If Matri_Campos_Obli Is Nothing Then
            Almacenamiento = "Matri Campos es nula consulte gabinete_detalle"
            Exit Function
        End If
        If UBound(Matri_Campos_Obli) <> UBound(_Matri_Datos) Then
            Almacenamiento = "Las matrices de datos y campos no son iguales es posible que el preindex pertenezca a otro gabinete "
            Exit Function
        End If
        '**************************************
        'Determina si evalua los campos obli
        'gatorios
        '**************************************
        If Evalua_Campo_Obli = 1 Then
            For z As Integer = 0 To UBound(Matri_Campos_Obli)
                Dim Matri_Tempo() As String
                Erase Matri_Tempo
                Matri_Tempo = Matri_Campos_Obli(z).Split("|")
                If Matri_Tempo(0) = 1 And _Matri_Datos(z) = "" Then
                    Almacenamiento = "El Campo " & Matri_Tempo(1).ToString & " es obligatorio "
                    Exit Function
                End If
            Next
        End If
        '*****************************************************************
        'consulta la base de datos system para traer los datos del sistema
        'esta consulta bloquea la base de datos y bloquea este registro
        'para que los demas usuarios no lo modifiquen al tiempo
        '*****************************************************************
        Dim carpealma As String = ""
        Dim numcarpvar As Integer = 0
        Dim Numero_Pagina_Carp As Integer = 0
        Dim tandiscvar As Long = 0
        Dim idal As Integer = 0
        Dim disc As Integer = 0
        'If HttpContext.Current.Session.Item("DA_Login_Usuario") = "" Then
        '    Almacenamiento = "Debe relacionar el usuario de plataforma contendora docuarchi.net  "
        '    Exit Function
        'End If
        Dim useral As String = UCase(HttpContext.Current.Session.Item("DA_Login_Usuario"))
        '****************************************************
        'Consulta que el usuario este registrado en el sistema
        '****************************************************
        If useral = "" Then
            useral = "consultapublico"
        End If

        '-------------------------------------------------
        'Determina el tamaño del archivo principal
        '-------------------------------------------------
        Dim tamano As String = ""
        Dim tipo As String = ""
        Dim tam_archivo As Object = 1024
        For i As Integer = 0 To Matri_Dcoumentos.Length - 1
            Dim fi As New FileInfo(Matri_Dcoumentos(i))
            If fi.Exists Then
                tam_archivo = tam_archivo + fi.Length
            End If
        Next
        If (tam_archivo / 1024) > 1024 Then
            tamano = Math.Round(((tam_archivo / 1024) / 1024), 2).ToString() & " Mb"
        Else
            tamano = Math.Round((tam_archivo / 1024), 2).ToString() & " Kb"
        End If
        Dim f2 As New FileInfo(Matri_Dcoumentos(0))
        tipo = UCase(f2.Extension)
        '-------------------------------------------------
        'Detecta el numero de paaginas cundo el documento
        'es diferente a TIF, BMP, JPG
        '-------------------------------------------------
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Dim numero_pagina As Integer = -1
        Dim Class_ItexShare As New Class_ItexShare
        Result = Class_ItexShare.Retorna_numero_paginas_documentos_unificados(Matri_Dcoumentos(0),
                                                                              numero_pagina)
        If Result <> "YES" Then
            Almacenamiento = Result
            Exit Function
        End If
        Dim pagi As Integer = Numero_Pag
        If numero_pagina <> -1 Then
            pagi = numero_pagina
        End If
        Dim indexal As Integer = Tipo_Doc
        Dim date1al As String = Date.Today
        Dim time1al As String = Date.Now.ToString
        ref_ClassGestionFechas.Formatea_Fecha_Almacenamiento_Time(time1al)
        Dim TempoFecha As String = Left(time1al, 10)
        time1al = Trim(time1al.Replace(TempoFecha, ""))
        '********************************************************************
        'Adiciona las validaciones para gestiòn documental
        'Verfica la opcion aplicar inventario documental
        '********************************************************************
        Dim opcion_inventario As Integer = 0
        Dim refclastrd As New ClassTrdDocumental
        Dim ref_Class_system1 As New Class_system1
        Result = ref_Class_system1.VerificaOpcionAplicarInventarioDocumental(opcion_inventario,
                                                                                 _Nombre_Gabienete)
        If Result <> "YES" Then
            Almacenamiento = "Inconsistencia verficando opción registrar inventario documental codigo:  " & Result
            Exit Function
        End If
        If opcion_inventario = 1 Then
            If id_usuario_gestion = 0 Then
                Almacenamiento = "El usuario docuarchi.net debe estar asociado a un usuario de gestión  "
                Exit Function
            End If
            If id_empresa = 0 Then
                Almacenamiento = "El usuario docuarchi.net asociado no tiene una empresa asociada  "
                Exit Function
            End If
        End If
        '--------------------------------------------------------------------
        'Verfica opción aplica tabla de retencion documental
        '--------------------------------------------------------------------
        Dim option_aplica_trd As Integer = 0
        Result = ref_Class_system1.VerificaOpcionAplicarTablaRetencion(option_aplica_trd,
                                                                           _Nombre_Gabienete)
        If Result <> "YES" Then
            Almacenamiento = "Inconsistencia verficando opción asignación trd codigo:  " & Result
            Exit Function
        End If
        '-------------------------------------------------------------------
        'Restricion valores minimos trd
        '-------------------------------------------------------------------
        Dim nombre_area As String = ""
        If option_aplica_trd = 1 Then
            Dim ref_Class_areas_depart_radicacion As New Class_areas_depart_radicacion
            If id_area <> 0 Then
                Result = ref_Class_areas_depart_radicacion.Retorna_nombre_area_por_id_area(id_area,
                                                                                           nombre_area)
                If Result <> "YES" Then
                    Almacenamiento = Result
                    Exit Function
                End If
            End If
        End If
        '--------------------------------------------------------------------
        'Verfica la opcion aplica unidad de conservación
        '--------------------------------------------------------------------
        Dim id_tipo_unidad_documental As Integer = 0
        Dim option_unidad_conservacion As Integer = 0
        Result = ref_Class_system1.Verfica_opcion_seleccion_unidad(option_unidad_conservacion,
                                                                   _Nombre_Gabienete)
        If Result <> "YES" Then
            Almacenamiento = "Inconsistencia verficando opción asignación unidad y expediente codigo:  " & Result
            Exit Function
        End If

        Dim estru_unidad_conservacion() As expediente_conservacion = Nothing
        Dim estado_expediente_electronico As Integer = 0
        Dim unidad_conserva_tipo As String = ""
        '--------------------------------------------------------------------
        'Restriccion unidad de conservacion expediente
        '--------------------------------------------------------------------
        Dim ref_clas_expediente As New ClassGaExpediente
        If option_unidad_conservacion = 1 Then
            'If id_expediente <> 0 And id_unidad_conservacion <> 0 Then
            '    Almacenamiento = "El sistema encontro ambiguedad en la asignación de la unidad de conserevación y expediente"
            '    Exit Function
            'End If
            If id_expediente <> 0 Then
                If id_clase_documento = 0 Then
                    Almacenamiento = "Por favor seleccione la clase de documento si quiere asignar el expediente"
                    Exit Function
                End If
                '---------------------------------------
                'Verifica el expediente no este cerrado
                '---------------------------------------
                Result = ref_clas_expediente.SolicitaDatosEstructuraExpediente(id_expediente,
                                                                               estru_unidad_conservacion)
                If Result <> "YES" Then
                    Almacenamiento = "Inconsistencia verficando propiedades del expediente  " & Result
                    Exit Function
                End If
                If estru_unidad_conservacion(0).ESTADO_EXPEDIENTE <> 1 Then
                    Almacenamiento = "El expediente no esta disponible, debido a que puede estar cerrado o descartado  "
                    Exit Function
                End If
                estado_expediente_electronico = estru_unidad_conservacion(0).estado_expediente_electronico
                '---------------------------------------------------
                'Retorna unidad de tipo documento
                '---------------------------------------------------
                Result = refclastrd.Retorna_unidad_conserva_tipo_documento(id_clase_documento,
                                                                           unidad_conserva_tipo)
                If Result <> "YES" Then
                    Almacenamiento = Result
                    Exit Function
                End If
            End If
            If id_unidad_conservacion <> 0 Then
                If id_clase_documento = 0 Then
                    Almacenamiento = "Por favor seleccione la clase de documento si quiere asignar la unidad de conservación"
                    Exit Function
                End If
            End If
        End If
        '--------------------------------------------------------------------
        'Solicita ruta indice expediente
        '--------------------------------------------------------------------
        Dim stru_produccion_indice As stru_produccion_indice = Nothing
        Dim ClassGaExpediente As New ClassGaExpediente
        Dim Ruta_archivo_xml As String = ""
        If id_expediente <> 0 And estado_expediente_electronico = 2 Then
            Result = ClassGaExpediente.Solicita_archivo_indice_expediente(id_expediente,
                                                                          Ruta_archivo_xml)
            If Result <> "YES" Then
                Almacenamiento = Result
                Exit Function
            End If
        End If
        Result = ""
        Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
        If Result <> "YES" Then
            Almacenamiento = "Error formateando fecha almacenamiento Funcion: Formatea_Fecha_Almacenamiento " & Result
            Exit Function
        End If
        Dim peso_archivo_byte As Long = 0
        peso_archivo_byte = FileLen(Matri_Dcoumentos(0))
        Dim mySqldatReader As MySqlDataReader
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_DA
        Result = ref.Returna_Conexion_Mysql(myConnection)
        If Result <> "YES" Then
            Almacenamiento = Result
            Exit Function
        End If
        Dim myTrans As MySqlTransaction
        Try
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            Dim Parametro_Select_System1 As String = " SELECT disco,proxid,tamdisc,numcarp,NUMPAG_CARP FROM system1 where nombre = " _
            & "'" & _Nombre_Gabienete & "' " & "for update"
            myCommand.CommandText = Parametro_Select_System1
            mySqldatReader = myCommand.ExecuteReader()
            If mySqldatReader Is Nothing Then
                Almacenamiento = "Imposible Encontrar Registro En Tabla Systema Error Conexion"
                myConnection.Close()
                Exit Function
            End If
            If mySqldatReader.HasRows = False Then
                Almacenamiento = "Imposible Encontrar Registro En Tabla Systema"
                mySqldatReader.Close()
                myConnection.Close()
                Exit Function
            End If
            '*******************************************************
            'Valores recuperados de la consulta de la tabla system1
            '********************************************************
            mySqldatReader.Read()
            numcarpvar = mySqldatReader.Item("NUMCARP")
            tandiscvar = mySqldatReader.Item("TAMDISC")
            idal = mySqldatReader.Item("PROXID")
            Numero_Pagina_Carp = mySqldatReader.Item("NUMPAG_CARP")
            idal = idal + 1
            disc = mySqldatReader.Item("DISCO")
            '***************************************************
            'validacion del tamaño de disco valor tabal system
            '***************************************************
            mySqldatReader.Close()
            If tandiscvar = 572523149 Or tandiscvar = 4310948432 Then
            Else
                Almacenamiento = "Tamaño de disco incorrecto Consulte su amnistrador valor : " & tandiscvar
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '****************************************************
            'validar que la carpeta sea diferente de cero
            '***************************************************
            If numcarpvar = 0 Then
                Almacenamiento = "Valor incorrecto de carpeta Consulte su amnistrador valor : " & numcarpvar
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '**************************************************
            'validar que el numero del disco sea valido
            '**************************************************
            If disc = 0 Then
                Almacenamiento = "Valor incorrecto de disco Consulte su amnistrador valor : " & disc
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If

            '*****************************************************
            'consulta que el ide sea diferente de "0"
            '*****************************************************
            If idal = 0 Then
                Almacenamiento = "Valor incorrecto de identidad de imagen Consulte su amnistrador valor : " & idal
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '******************************************************
            'consulta que le numero de paginas sea diferente de "0"
            '******************************************************
            If pagi = 0 Then
                Almacenamiento = "Valor incorrecto de paginas Consulte su amnistrador valor : " & pagi
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '**************************************
            'Consulta que el disco tenga espacion
            'donde se guardan las imagenes SL
            '**************************************
            Dim EstadoDisco As String = ""
            Dim ResulDisco As String = ""
            Dim ref_Class_disco_detalle As New Class_disco_detalle
            ResulDisco = ref_Class_disco_detalle.Numero_Imagenes(_Nombre_Gabienete,
                                                                 tandiscvar,
                                                                 disc,
                                                                 EstadoDisco)
            If ResulDisco = "YES" Then
                If EstadoDisco = "SL" Then
                    Almacenamiento = "Disco  " & disc.ToString & " Sobrepaso el limite de capacidad"
                    'myConnection.Close()
                    'myTrans.Rollback()
                    myConnection.Close()
                    Exit Function

                End If
            Else
                'myTrans.Rollback()
                myConnection.Close()
                Almacenamiento = ResulDisco
                Exit Function
            End If
            '*****************************************************
            'Actualizacion para determinar el numero de imagenes
            'por base de datos para evitar contar los archivos
            'del directorio y agilizar la funcion
            '****************************************************
            Dim Valor_Suma_Imagen As Integer = Numero_Pag + Numero_Pagina_Carp
            If Valor_Suma_Imagen > 230 Then
                numcarpvar = numcarpvar + 1
                Numero_Pagina_Carp = Numero_Pag
            Else
                Numero_Pagina_Carp = Valor_Suma_Imagen

            End If
            Dim Parametro_Actualiza_System1 As String = "update system1 set proxid = " & "'" & idal & "' ," &
            " numcarp = " & " '" & numcarpvar & "', NUMPAG_CARP=" & Numero_Pagina_Carp & " where nombre =" & "'" &
            _Nombre_Gabienete & "'" & "and proxid <> " & "'" & idal & "'"
            myCommand.CommandText = Parametro_Actualiza_System1
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            '*********************************
            'Determina si se actualizo
            'el nuevo id de la base de datos
            '*********************************
            If Switc = 0 Then
                Almacenamiento = "Imposible actualizar la tabla System  : " & Parametro_Actualiza_System1
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myTrans.Commit()
        Catch e As Exception
            Try

            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    myTrans.Rollback()
                    myConnection.Close()
                    Almacenamiento = "An exception of type " + ex.GetType().ToString() +
                                      " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try
            If Not myTrans Is Nothing Then
                myTrans.Rollback()
            End If
            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Almacenamiento = "Error General " & e.Message
            Exit Function
        End Try

        '*****************************************************
        'verifica existencia de carpeta y crea nueva carpeta
        '*****************************************************
        Dim rut2000 As String = ""
        Dim Result_Carpeta As String
        Dim RutaCarpet As String
        '*****************************************************
        'Cambio para almacenamiento via web service
        '*****************************************************
        RutaCarpet = _Ruta_Almacenamiento & _Nombre_Gabienete & disc
        Result_Carpeta = Solicita_Carpeta_almacenamiento(carpealma,
                                                         numcarpvar,
                                                         rut2000,
                                                         RutaCarpet)
        If Result_Carpeta <> "YES" Then
            Almacenamiento = "Imposible calcular tamaño carpeta Consulte su administrador  : " & Result_Carpeta
            Exit Function
        End If
        carpealma = carpealma & numcarpvar
        Try
            Dim Switc2 As Integer = 0
            Dim myCommand2 As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand2.Connection = myConnection
            myCommand2.Transaction = myTrans

            '******************************************
            'Arma sql de almacenamiento en el gabinete
            '******************************************
            Dim Matri_Xml() As String
            Erase Matri_Xml
            Dim Campos_Insert As String = "ID,DISC,PAG,DBT,IDEX,USER,DATE1,TIME1"
            Dim Datos_Insert As String = idal & "," & " " & disc & " " & "," & "'" & pagi & "'" & "," & "'" & Tipo_Doc & "'" & "," & "'" & numcarpvar & "'" & ","
            Datos_Insert = Datos_Insert & "'" & useral & "'" & "," & "'" & date1al & "'" & "," & "'" & time1al & "'"
            '----------------------------------------------------------
            'Agrega los datos y los campos de las tablas de retención
            '----------------------------------------------------------
            If option_aplica_trd = 1 Then
                Campos_Insert = Campos_Insert & ",ID_AREA,ID_SERIE,ID_SUB_SERIE,ID_TIPODOCUMENTO"
                If id_area = 0 Then
                    Datos_Insert = Datos_Insert & ",null"
                Else
                    Datos_Insert = Datos_Insert & "," & id_area
                End If
                If id_serie = 0 Then
                    Datos_Insert = Datos_Insert & ",null"
                Else
                    Datos_Insert = Datos_Insert & "," & id_serie
                End If
                If id_sub_serie = 0 Then
                    Datos_Insert = Datos_Insert & ",null"
                Else
                    Datos_Insert = Datos_Insert & "," & id_sub_serie
                End If
                If id_tipo_documento = 0 Then
                    Datos_Insert = Datos_Insert & ",null"
                Else
                    Datos_Insert = Datos_Insert & "," & id_tipo_documento
                End If
                '------------------------------------------------------------
                'Retorna nombre del area
                '------------------------------------------------------------

            End If
            '------------------------------------------------------------
            'Agrega los campos de unidad de conservacion y asignación
            '------------------------------------------------------------
            If option_unidad_conservacion = 1 Then
                Campos_Insert = Campos_Insert & ",ID_EXPEDIENTE,ID_TIPO_EXPEDIENTE,ID_UNIDAD_CONSERVACION," &
                "ID_TIPO_UNIDAD_CONSERVACION,ID_CLASE_DOCUMENTO"
                If id_expediente = 0 Then
                    Datos_Insert = Datos_Insert & ",null"
                Else
                    Datos_Insert = Datos_Insert & "," & id_expediente
                End If
                If id_tipo_expediente = 0 Then
                    Datos_Insert = Datos_Insert & ",null"
                Else
                    Datos_Insert = Datos_Insert & "," & id_tipo_expediente
                End If
                If id_unidad_conservacion = 0 Then
                    Datos_Insert = Datos_Insert & ",null"
                Else
                    Datos_Insert = Datos_Insert & "," & id_unidad_conservacion
                End If
                If id_tipo_unidad_conservacion = 0 Then
                    Datos_Insert = Datos_Insert & ",null"
                Else
                    Datos_Insert = Datos_Insert & "," & id_tipo_unidad_conservacion
                End If
                If id_clase_documento = 0 Then
                    Datos_Insert = Datos_Insert & ",null"
                Else
                    Datos_Insert = Datos_Insert & "," & id_clase_documento
                End If
                If id_expediente <> 0 Then
                    id_tipo_unidad_documental = 2
                End If
                If id_unidad_conservacion <> 0 Then
                    id_tipo_unidad_documental = 1
                End If
                Dim refa_id_tipo_unidad_documental As String = "null"
                Campos_Insert = Campos_Insert & ",ID_TIPO_UNIDAD_DOCUMENTAL"
                If id_tipo_unidad_documental = 0 Then
                    Datos_Insert = Datos_Insert & "," & refa_id_tipo_unidad_documental
                Else
                    Datos_Insert = Datos_Insert & "," & id_tipo_unidad_documental
                End If

                '-------------------------------------------------------------
                'Actualiza el numero de folios electrónicos o digitalizados 
                'expediente
                '-------------------------------------------------------------
                If id_expediente <> 0 Then
                    Dim Numero_Digitalizado_contenido As Integer = 0
                    Dim Numero_Electronico_contenido As Integer = 0
                    Dim Parametro_Select_System1 As String = " SELECT NUMERO_DIGITALIZADO_CONTENIDO,NUMERO_ELECTRONICO_CONTENIDO" &
                    " FROM expediente_archivo where ID_EXPEDIENTE = " _
                   & "'" & id_expediente & "' " & "for update"
                    myCommand2.CommandText = Parametro_Select_System1
                    mySqldatReader = myCommand2.ExecuteReader()
                    If mySqldatReader Is Nothing Then
                        Almacenamiento = "Imposible encontrar la identificación del expediente "
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    End If
                    If mySqldatReader.HasRows = False Then
                        Almacenamiento = "Imposible Encontrar el registro del expediente"
                        mySqldatReader.Close()
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    Else
                        mySqldatReader.Read()
                        Numero_Digitalizado_contenido = mySqldatReader.Item(0)
                        Numero_Electronico_contenido = mySqldatReader.Item(1)
                        mySqldatReader.Close()
                    End If
                    Dim update_sql As String = ""
                    Numero_Electronico_contenido = Numero_Electronico_contenido + pagi
                    update_sql = "update expediente_archivo Set NUMERO_ELECTRONICO_CONTENIDO=" & Numero_Electronico_contenido &
                     ",NUMERO_DIGITALIZADO_CONTENIDO=" & Numero_Electronico_contenido & " where ID_EXPEDIENTE = " & "'" & id_expediente & "' "
                    myCommand2.CommandText = update_sql
                    Switc2 = myCommand2.ExecuteNonQuery()
                    If Switc2 = 0 Then
                        Almacenamiento = "Imposible Actualizar numero de folios del expediente "
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    End If
                End If
                '---------------------------------------------------------------
                'Actualiza el numero de folios electronicos o digitalizados
                'de la unidad de conservacion
                '---------------------------------------------------------------
                If id_unidad_conservacion <> 0 Then
                    Dim Numero_Digitalizado_contenido As Integer = 0
                    Dim Numero_Electronico_contenido As Integer = 0
                    Dim Parametro_Select_System1 As String = " SELECT NUMERO_DIGITALIZADO_CONTENIDO,NUMERO_ELECTRONICO_CONTENIDO" &
                    " FROM unidad_conservacion where ID_UNIDAD_CONSERVACION = " _
                   & "'" & id_unidad_conservacion & "' " & "for update"
                    myCommand2.CommandText = Parametro_Select_System1
                    mySqldatReader = myCommand2.ExecuteReader()
                    If mySqldatReader Is Nothing Then
                        Almacenamiento = "Imposible encontrar la identificación de la unidad de conservación "
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    End If
                    If mySqldatReader.HasRows = False Then
                        Almacenamiento = "Imposible Encontrar el registro de la unidad de conservación"
                        mySqldatReader.Close()
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    Else
                        mySqldatReader.Read()
                        Numero_Digitalizado_contenido = mySqldatReader.Item(0)
                        Numero_Electronico_contenido = mySqldatReader.Item(1)
                        mySqldatReader.Close()
                    End If
                    Dim update_sql As String = ""
                    If unidad_conserva_tipo = "DIGITALIZADO" Then
                        Numero_Digitalizado_contenido = Numero_Digitalizado_contenido + pagi
                        update_sql = "update unidad_conservacion Set NUMERO_DIGITALIZADO_CONTENIDO=" & Numero_Digitalizado_contenido &
                        " where ID_UNIDAD_CONSERVACION = " & "'" & id_unidad_conservacion & "' "
                    End If
                    If unidad_conserva_tipo = "ELECTRONICO" Then
                        Numero_Electronico_contenido = Numero_Electronico_contenido + pagi
                        update_sql = "update unidad_conservacion Set NUMERO_ELECTRONICO_CONTENIDO=" & Numero_Electronico_contenido &
                        " where ID_UNIDAD_CONSERVACION = " & "'" & id_unidad_conservacion & "' "
                    End If
                    myCommand2.CommandText = update_sql
                    Switc2 = myCommand2.ExecuteNonQuery()
                    If Switc2 = 0 Then
                        Almacenamiento = "Imposible Actualizar numero de folios de la unidad de conservación "
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    End If
                End If
            End If
            '---------------------------------------------
            'Construye registro de inventario documental
            '---------------------------------------------
            Dim ref_expediente As String = "null"
            Dim ref_nombre_serie As String = "null"
            Dim ref_nombre_sub_serie As String = "null"
            Dim ref_tipo_documento As String = "null"
            Dim ref_unidad_conserva As String = "null"
            Dim ref_clase_documento As String = "null"
            Dim ref_fecha_elaboracion As String = "null"
            Dim ref_id_expediente As String = "null"
            Dim ref_id_unidad_conservacion As String = "null"
            Dim ref_id_area As String = "null"
            Dim ref_id_serie As String = "null"
            Dim ref_id_tipo_unidad_conservacion As String = "null"
            Dim ref_id_clase_documento As String = "null"
            Dim ref_nombre_area As String = "null"
            Dim ref_id_sub_serie As String = "null"
            Dim ref_id_tipo_documento As String = "null"
            Dim ref_id_tipo_expediente As String = "null"
            Dim ref_id_tipo_unidad_documental As String = "null"
            Dim ref_radicado As String = "null"
            If radicado <> "" Then
                ref_radicado = "'" & radicado & "'"
            End If
            If id_tipo_unidad_documental <> 0 Then
                ref_id_tipo_unidad_documental = id_tipo_unidad_documental
            End If
            If id_tipo_expediente <> 0 Then
                ref_id_tipo_expediente = id_tipo_expediente
            End If
            If id_tipo_documento <> 0 Then
                ref_id_tipo_documento = id_tipo_documento
            End If
            If id_sub_serie <> 0 Then
                ref_id_sub_serie = id_sub_serie
            End If
            If nombre_area <> "" Then
                ref_nombre_area = "'" & nombre_area & "'"
            End If
            If id_clase_documento <> 0 Then
                ref_id_clase_documento = id_clase_documento
            End If
            If id_tipo_unidad_conservacion <> 0 Then
                ref_id_tipo_unidad_conservacion = id_tipo_unidad_conservacion
            End If
            If id_serie <> 0 Then
                ref_id_serie = id_serie
            End If
            If id_area <> 0 Then
                ref_id_area = id_area
            End If
            If id_expediente <> 0 Then
                ref_id_expediente = id_expediente
            End If
            If id_unidad_conservacion <> 0 Then
                ref_id_unidad_conservacion = id_unidad_conservacion
            End If
            If expediente <> "" Then
                ref_expediente = "'" & expediente & "'"
            End If
            If nombre_serie <> "" Then
                ref_nombre_serie = "'" & nombre_serie & "'"
            End If
            If nombre_sub_serie <> "" Then
                ref_nombre_sub_serie = "'" & nombre_sub_serie & "'"
            End If
            If tipo_documento <> "" Then
                ref_tipo_documento = "'" & tipo_documento & "'"
            End If
            If unidad_conserva <> "" Then
                ref_unidad_conserva = "'" & unidad_conserva & "'"
            End If
            If clase_documento <> "" Then
                ref_clase_documento = "'" & clase_documento & "'"
            End If
            If fecha_elaboracion <> "" Then
                ref_fecha_elaboracion = "'" & fecha_elaboracion & "'"
            End If
            Dim ref_sugundo_nombre_documento As String = ""
            If sugundo_nombre_documento <> "" Then
                ref_sugundo_nombre_documento = "'" & sugundo_nombre_documento & "'"
            Else
                Dim Ceros_Cuerpo_Imag As String = "DIG"
                Dim file_inf As New FileInfo(Matri_Dcoumentos(0))
                Result = Ceros_Imagen_Almacenada(Ceros_Cuerpo_Imag, idal)
                ref_sugundo_nombre_documento = "'DIG" & Ceros_Cuerpo_Imag & idal & file_inf.Extension & "'"
            End If
            Dim fultex As String = ""
            Dim fultex_log As String = ""
            For z3 As Integer = 0 To UBound(_Matri_Datos)
                If _Matri_Datos(z3) <> "" Then
                    fultex = fultex & vbCrLf & _Matri_Datos(z3)
                    fultex_log = fultex_log & "|" & _Matri_Datos(z3)
                End If
            Next

            Dim estado_archivo As Integer = 0
            If id_expediente <> 0 Or id_unidad_conservacion <> 0 Then
                estado_archivo = 0
            End If
            Dim datos_insert_inventario As String = ""
            Dim sqlinventario As String = ""
            Dim xmlArchivo As New XmlDocument
            Dim estado_exml_archivo As String = ""
            If opcion_inventario = 1 Then
                sqlinventario = "insert into registro_producion_documental (remit_dest_interno_idremit_dest_interno," &
                "ID_USUARIO_GESTION,FECHA_DOCUMENTO,ID_AREA_DEPARTAMENTO,ID_SERIE_DOCUMENTO,SERIE_DOCUMENTO," &
                "ID_SUBSERIE_DOCUMENTO,SUBSERIE_DOCUMENTO,ID_TIPO_DOCUMENTO,DESCRIPCION_TIPO_DOCUMENTO,FULTEXT_DOCUMENTO," &
                "ID_DOCUMENTO_DOCUARCHI_ALMACEN,ESTADO_DOCUMENTO_ARCHIVO,NOMBRE_GABINETE,NUMERO_FOLIOS," &
                "EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE,EXPEDIENTE,ID_TIPO_EXPEDIENTE,ID_TIPO_UNIDAD_CONSERVACION," &
                "ID_UNIDAD_CONSERVACION,ID_CLASE_DOCUMENTO,CLASEDOCUMENTO," &
                "FECHA_ELABORACION,UNIDADCONSERVA,NOMBRE_AREA_DEPARTAMENTO,ID_TIPO_UNIDAD_DOCUMENTAL,ID_EMPRESA_DOCUMENTO," &
                "RADICADO_DOCUMENTO,SEGUNDO_NOMBRE_DOCUMENTO,DOCUMENTO_PRODUCION_DOCUMENTAL,TAMANO,FORMATO) values "
                datos_insert_inventario = "(" & id_usuario_gestion & "," & id_usuario_gestion & ",'" & date1al & "'," &
                ref_id_area & "," & ref_id_serie & "," & ref_nombre_serie & "," & ref_id_sub_serie & "," & ref_nombre_sub_serie &
                "," & ref_id_tipo_documento & "," & ref_tipo_documento & ",'" & fultex & "'," & idal & "," &
                estado_archivo & ",'" & _Nombre_Gabienete & "'," & pagi & "," & ref_id_expediente & "," & ref_expediente & "," & ref_id_tipo_expediente &
                "," & ref_id_tipo_unidad_conservacion & "," & ref_id_unidad_conservacion & "," & ref_id_clase_documento & "," &
                ref_clase_documento & "," & ref_fecha_elaboracion & "," & ref_unidad_conserva & "," & ref_nombre_area & "," & ref_id_tipo_unidad_documental &
                "," & id_empresa & "," & ref_radicado & "," & ref_sugundo_nombre_documento & "," & tipo_archivo_producion & ",'" & tamano & "','" & tipo & "')"
                sqlinventario = sqlinventario & datos_insert_inventario
                '-----------------------------------------------
                'Registra inventario documental
                '-----------------------------------------------
                myCommand2.CommandText = sqlinventario
                Switc2 = myCommand2.ExecuteNonQuery()
                If Switc2 = 0 Then
                    Almacenamiento = "Imposible agregar registro inventario general  "
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
                Dim oblast As Object = myCommand2.LastInsertedId
                id_registro_producion = myCommand2.LastInsertedId
                Campos_Insert = Campos_Insert & ",ID_INVENTARIO_DOCUMENTAL"
                Datos_Insert = Datos_Insert & "," & oblast
                Campos_Insert = Campos_Insert & ",ID_USUARIO_GESTION"
                Datos_Insert = Datos_Insert & "," & id_usuario_gestion
                Dim Ceros_Cuerpo_Imag As String = ""
                Dim nombre_docuarchi As String = ""
                '------------------------------------------------------
                'Crea indice documento en el expediente
                '------------------------------------------------------
                If id_expediente <> 0 And estado_expediente_electronico = 2 Then
                    'Nombre documento docuarchi
                    Result = Ceros_Imagen_Almacenada(Ceros_Cuerpo_Imag,
                                                     idal)
                    If Result <> "YES" Then
                        Almacenamiento = "Error generando ceros de imagen "
                        Exit Function
                    End If
                    Dim Extension As String = ""
                    Dim visor As String = ""
                    Dim Estado_doc As String = ""
                    stru_produccion_indice.SEGUNDO_NOMBRE_DOCUMENTO = ref_sugundo_nombre_documento
                    stru_produccion_indice.ID_REGISTRO_PRODUCION_DOCUMENTAL = id_registro_producion
                    stru_produccion_indice.NOMBRE_DOCUARCHI = "DIG" & Ceros_Cuerpo_Imag & idal & tipo
                    Dim valor_ingreso_hueya As String = stru_produccion_indice.ID_REGISTRO_PRODUCION_DOCUMENTAL
                    encriptacion.encript_md5(valor_ingreso_hueya,
                                                  "7894561230!",
                                                   stru_produccion_indice.VALOR_HUELLA)
                    stru_produccion_indice.FUCION_RESUMEN = "MD5"
                    Dim ClassGestionFechas As New ClassGestionFechas
                    Dim fecha_incorporacion As String = ""
                    stru_produccion_indice.FECHA_ELABORACION = date1al
                    stru_produccion_indice.FECHA_DOCUMENTO = date1al
                    stru_produccion_indice.FORMATO = tipo
                    stru_produccion_indice.TAMANO = tamano
                    stru_produccion_indice.CLASEDOCUMENTO = ref_clase_documento
                    stru_produccion_indice.RUTA_ARCHIVO = _Ruta_Almacenamiento & _Nombre_Gabienete & disc & "/" & carpealma & "/" & "DIG" & Ceros_Cuerpo_Imag & idal & tipo
                    stru_produccion_indice.RUTA_ARCHIVO = stru_produccion_indice.RUTA_ARCHIVO.Replace("/", "\")
                    stru_produccion_indice.NUMERO_FOLIOS = pagi
                    If tipo_documento = "" Then
                        stru_produccion_indice.DESCRIPCION_TIPO_DOCUMENTO = "NA"
                    Else
                        stru_produccion_indice.DESCRIPCION_TIPO_DOCUMENTO = tipo_documento
                    End If
                    stru_produccion_indice.CLASEDOCUMENTO = ref_clase_documento
                    Dim ORDEN_INDICE As Integer = 0
                    Dim ULTIMA_PAGINA_INDICE As Integer = 0
                    Dim Parametro_orden_indice As String = " SELECT ORDEN_INDICE,ULTIMA_PAGINA_INDICE" &
                    " FROM expediente_archivo where ID_EXPEDIENTE = " _
                     & id_expediente & " " & " for update"
                    myCommand2.CommandText = Parametro_orden_indice
                    mySqldatReader = myCommand2.ExecuteReader()
                    If mySqldatReader Is Nothing Then
                        Almacenamiento = "Imposible encontrar la identificación de la unidad de conservación "
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    End If
                    If mySqldatReader.HasRows = False Then
                        Almacenamiento = "Imposible Encontrar el registro de la unidad de conservación"
                        mySqldatReader.Close()
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    Else
                        mySqldatReader.Read()
                        ORDEN_INDICE = mySqldatReader.Item(0)
                        ULTIMA_PAGINA_INDICE = mySqldatReader.Item(1)
                        mySqldatReader.Close()
                    End If
                    ORDEN_INDICE = ORDEN_INDICE + 1
                    Dim PAGINA_INICIAL As Integer = ULTIMA_PAGINA_INDICE + 1
                    ULTIMA_PAGINA_INDICE = ULTIMA_PAGINA_INDICE + pagi
                    stru_produccion_indice.ORDEN_EN_EXPEDIENTE = ORDEN_INDICE
                    stru_produccion_indice.PAGINA_INICIO = PAGINA_INICIAL
                    stru_produccion_indice.PAGINA_FINAL = ULTIMA_PAGINA_INDICE
                    Dim sql_insert As String = "insert into  ra_cert_indice_expediente (registro_producion_documental_ID_REGISTRO_PRODUCION_DOCUMENTAL," &
                    "expediente_archivo_ID_EXPEDIENTE,Nombre_documento,Tipologia_documental,fecha_declaracion_documento,fecha_incorporacion_documento," &
                    "valor_huella,Funcion_resumen,orden_documento_expedicion,pagina_inicial,pagina_final,formato,dimension_kb,origen,ruta_documento,numero_folios, segundo_nombre) values (" &
                    stru_produccion_indice.ID_REGISTRO_PRODUCION_DOCUMENTAL & "," & id_expediente & ",'" & stru_produccion_indice.NOMBRE_DOCUARCHI & "','" &
                    stru_produccion_indice.DESCRIPCION_TIPO_DOCUMENTO & "','" & stru_produccion_indice.FECHA_DOCUMENTO & "','" & stru_produccion_indice.FECHA_ELABORACION & "','" &
                    stru_produccion_indice.VALOR_HUELLA & "','" & stru_produccion_indice.FUCION_RESUMEN & "'," & ORDEN_INDICE & "," & PAGINA_INICIAL &
                    "," & ULTIMA_PAGINA_INDICE & ",'" & stru_produccion_indice.FORMATO & "','" & stru_produccion_indice.TAMANO & "'," & stru_produccion_indice.CLASEDOCUMENTO & ",'" &
                    stru_produccion_indice.RUTA_ARCHIVO & "'," & stru_produccion_indice.NUMERO_FOLIOS & "," & stru_produccion_indice.SEGUNDO_NOMBRE_DOCUMENTO & ")"
                    myCommand2.CommandText = sql_insert
                    Switc2 = myCommand2.ExecuteNonQuery()
                    If Switc2 = 0 Then
                        Almacenamiento = "Imposible crear indice documento "
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    End If
                    Dim last_insert_indice As Object = myCommand2.LastInsertedId
                    Dim update_orden_ultima_pagina As String = " UPDATE expediente_archivo " &
                        " SET ORDEN_INDICE=" & ORDEN_INDICE & " , ULTIMA_PAGINA_INDICE=" & ULTIMA_PAGINA_INDICE &
                        "  where ID_EXPEDIENTE = " _
                        & id_expediente
                    myCommand2.CommandText = update_orden_ultima_pagina
                    Switc2 = myCommand2.ExecuteNonQuery()
                    If Switc2 = 0 Then
                        Almacenamiento = "Imposible actualizar el orden del indice en el expediente "
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    End If
                    stru_produccion_indice.SEGUNDO_NOMBRE_DOCUMENTO = stru_produccion_indice.SEGUNDO_NOMBRE_DOCUMENTO.Replace("'", "")
                    stru_produccion_indice.SEGUNDO_NOMBRE_DOCUMENTO = stru_produccion_indice.SEGUNDO_NOMBRE_DOCUMENTO.Replace("'", "")
                    Result = ref_clas_expediente.Actualiza_archivo_xml_indice_expediente(Ruta_archivo_xml,
                                                                                         stru_produccion_indice,
                                                                                         xmlArchivo)
                    If Result <> "YES" Then
                        myTrans.Rollback()
                        If Not myConnection Is Nothing Then
                            myConnection.Close()
                        End If
                        Almacenamiento = "Error actualizando archivo xml indice " & Result
                        Exit Function
                    End If
                    estado_exml_archivo = "YES"
                End If
            End If
            '--------------------------------------------------
            'Carga los datos de insercion de los campos de
            'digitacion para el gabinete
            '--------------------------------------------------
            For z2 As Integer = 0 To UBound(Matri_Campos_Obli)
                Dim Matri_Tempo() As String
                Erase Matri_Tempo
                Matri_Tempo = Matri_Campos_Obli(z2).Split("|")
                Campos_Insert = Campos_Insert & "," & Matri_Tempo(1).ToString
            Next
            For z3 As Integer = 0 To UBound(_Matri_Datos)
                If _Matri_Datos(z3) = "" Then
                    Datos_Insert = Datos_Insert & "," & "null"
                Else
                    Datos_Insert = Datos_Insert & ",'" & _Matri_Datos(z3) & "'"
                End If

            Next
            Dim Parametro_Insert_Registro As String = "Insert into " & _Nombre_Gabienete & "( " & Campos_Insert & " )" & " Values " & "( " & Datos_Insert & " )"
            myCommand2.CommandText = Parametro_Insert_Registro
            Switc2 = myCommand2.ExecuteNonQuery()
            '************************************
            'Determina si se inserto el registro
            'el nuevo id de la base de datos
            '*************************************
            If Switc2 = 0 Then
                Almacenamiento = "Imposible agregar registro  : " & Parametro_Insert_Registro
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '----------------------------------
            'registra log worlflow docuarchi
            '----------------------------------
            Dim Ceros_Cuerpo_Imag_ As String = "DIG"
            Dim file_inf_ As New FileInfo(Matri_Dcoumentos(0))
            Result = Ceros_Imagen_Almacenada(Ceros_Cuerpo_Imag_, idal)
            Dim noombre_log = "DIG" & Ceros_Cuerpo_Imag_ & idal & file_inf_.Extension
            Dim Ruta_Alamce_Image_ As String = _Ruta_Almacenamiento & _Nombre_Gabienete & disc & "/" & carpealma & "/" & noombre_log
            Ruta_Alamce_Image_ = Ruta_Alamce_Image_.Replace("\", "/")
            Dim hor As New System.DateTime
            hor = Date.Now
            Dim hora As String = hor.Hour.ToString & ":" & hor.Minute.ToString & ":" & hor.Second.ToString
            Dim SqlTransac As String = " INSERT INTO logdocuarchi (id_tran,desc_op,USER_OPER,DATE_TRANS," _
            & "RUT_DOCU,MODULO_REGISTRO,GABINETE,CAMPOS,IP_TRANS,HORA_REGISTRO,RADICADO,ID_TAREA_WF,ID_RUTA_WF,USER_PROPIETARIO,TIPOLOGIA_DOCUMENTAL) VALUES ( "
            SqlTransac = SqlTransac & "'" & idal & "',"
            SqlTransac = SqlTransac & "'" & "Registra" & "',"
            SqlTransac = SqlTransac & "'" & useral & "',"
            SqlTransac = SqlTransac & "'" & date1al & "',"
            SqlTransac = SqlTransac & "'" & Ruta_Alamce_Image_ & "',"
            SqlTransac = SqlTransac & "'" & "WORKFLOW" & "',"
            SqlTransac = SqlTransac & "'" & _Nombre_Gabienete & "',"
            SqlTransac = SqlTransac & "'" & fultex_log & "','" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hora & "'," & ref_radicado & "," &
                id_tarea_workflow & "," & HttpContext.Current.Session.Item("Id_Ruta_Workflow") & ",'" & useral & "'," & ref_tipo_documento & ")"
            If id_tarea_workflow <> 0 Then
                myCommand2.CommandText = SqlTransac
                Switc2 = myCommand2.ExecuteNonQuery()
                If Switc2 = 0 Then
                    Almacenamiento = "Imposible registrar log  : " & SqlTransac
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            '***********************************
            'Crea archivo xml para imagen
            '***********************************
            ReDim Preserve Matri_Xml(0)
            Matri_Xml(0) = "ID¬" & idal
            ReDim Preserve Matri_Xml(1)
            Matri_Xml(1) = "DISC¬" & disc
            ReDim Preserve Matri_Xml(2)
            Matri_Xml(2) = "PAG¬" & pagi
            ReDim Preserve Matri_Xml(3)
            Matri_Xml(3) = "DBT¬" & Tipo_Doc
            ReDim Preserve Matri_Xml(4)
            Matri_Xml(4) = "IDEX¬" & numcarpvar
            ReDim Preserve Matri_Xml(5)
            Matri_Xml(5) = "USER¬" & useral
            ReDim Preserve Matri_Xml(6)
            Matri_Xml(6) = "DATE1¬" & date1al
            ReDim Preserve Matri_Xml(7)
            Matri_Xml(7) = "TIME1¬" & time1al
            Dim IncreMat As Integer = 7
            For z2 As Integer = 0 To UBound(Matri_Campos_Obli)
                IncreMat = IncreMat + 1
                Dim Matri_Tempo() As String
                Erase Matri_Tempo
                Matri_Tempo = Matri_Campos_Obli(z2).Split("|")
                Campos_Insert = Campos_Insert & "," & Matri_Tempo(1).ToString
                ReDim Preserve Matri_Xml(IncreMat)
                Matri_Xml(IncreMat) = Matri_Tempo(1).ToString & "¬"
                If _Matri_Datos(z2) = "" Then
                    Matri_Xml(IncreMat) = Matri_Xml(IncreMat) & "null"
                Else
                    Matri_Xml(IncreMat) = Matri_Xml(IncreMat) & _Matri_Datos(z2)
                End If
            Next
            IncreMat = IncreMat + 1
            ReDim Preserve Matri_Xml(IncreMat)
            If ref_id_area = "null" Then
                Matri_Xml(IncreMat) = "ID_AREA¬" & ref_id_area
            Else
                Matri_Xml(IncreMat) = "ID_AREA¬" & id_area
            End If
            IncreMat = IncreMat + 1
            ReDim Preserve Matri_Xml(IncreMat)
            If ref_id_serie = "null" Then
                Matri_Xml(IncreMat) = "ID_SERIE¬" & ref_id_serie
            Else
                Matri_Xml(IncreMat) = "ID_SERIE¬" & id_serie
            End If
            IncreMat = IncreMat + 1
            ReDim Preserve Matri_Xml(IncreMat)
            If ref_id_sub_serie = "null" Then
                Matri_Xml(IncreMat) = "ID_SUB_SERIE¬" & ref_id_sub_serie
            Else
                Matri_Xml(IncreMat) = "ID_SUB_SERIE¬" & id_sub_serie
            End If
            IncreMat = IncreMat + 1
            ReDim Preserve Matri_Xml(IncreMat)
            If ref_id_tipo_documento = "null" Then
                Matri_Xml(IncreMat) = "ID_TIPODOCUMENTO¬" & ref_id_tipo_documento
            Else
                Matri_Xml(IncreMat) = "ID_TIPODOCUMENTO¬" & id_tipo_documento
            End If
            IncreMat = IncreMat + 1
            ReDim Preserve Matri_Xml(IncreMat)
            Matri_Xml(IncreMat) = "ID_USUARIO_GESTION¬" & id_usuario_gestion

            IncreMat = IncreMat + 1
            ReDim Preserve Matri_Xml(IncreMat)
            If ref_id_expediente = "null" Then
                Matri_Xml(IncreMat) = "ID_EXPEDIENTE¬" & ref_id_expediente
            Else
                Matri_Xml(IncreMat) = "ID_EXPEDIENTE¬" & id_expediente
            End If
            IncreMat = IncreMat + 1
            ReDim Preserve Matri_Xml(IncreMat)
            If ref_id_tipo_expediente = "null" Then
                Matri_Xml(IncreMat) = "ID_TIPO_EXPEDIENTE¬" & ref_id_expediente
            Else
                Matri_Xml(IncreMat) = "ID_TIPO_EXPEDIENTE¬" & id_expediente
            End If

            IncreMat = IncreMat + 1
            ReDim Preserve Matri_Xml(IncreMat)
            If ref_id_unidad_conservacion = "null" Then
                Matri_Xml(IncreMat) = "ID_UNIDAD_CONSERVACION¬" & ref_id_unidad_conservacion
            Else
                Matri_Xml(IncreMat) = "ID_UNIDAD_CONSERVACION¬" & id_unidad_conservacion
            End If

            IncreMat = IncreMat + 1
            ReDim Preserve Matri_Xml(IncreMat)
            If ref_id_tipo_unidad_conservacion = "null" Then
                Matri_Xml(IncreMat) = "ID_TIPO_UNIDAD_CONSERVACION¬" & ref_id_tipo_unidad_conservacion
            Else
                Matri_Xml(IncreMat) = "ID_TIPO_UNIDAD_CONSERVACION¬" & id_tipo_unidad_conservacion
            End If

            IncreMat = IncreMat + 1
            ReDim Preserve Matri_Xml(IncreMat)
            If ref_id_tipo_unidad_documental = "null" Then
                Matri_Xml(IncreMat) = "ID_TIPO_UNIDAD_DOCUMENTAL¬" & ref_id_tipo_unidad_documental
            Else
                Matri_Xml(IncreMat) = "ID_TIPO_UNIDAD_DOCUMENTAL¬" & id_tipo_unidad_documental
            End If

            IncreMat = IncreMat + 1
            ReDim Preserve Matri_Xml(IncreMat)
            If ref_id_clase_documento = "null" Then
                Matri_Xml(IncreMat) = "ID_CLASE_DOCUMENTO¬" & ref_id_clase_documento
            Else
                Matri_Xml(IncreMat) = "ID_CLASE_DOCUMENTO¬" & id_clase_documento
            End If
            Dim Ruta_Alamce_Image As String = _Ruta_Almacenamiento & _Nombre_Gabienete & disc & "\" & carpealma & "\"
            Result = ""

            Result = Generando_Archivo_Dat_Xml(Ruta_Alamce_Image,
                                               idal,
                                               Matri_Xml,
                                               _Ruta_Carpeta)

            If Result <> "YES" Then
                myTrans.Rollback()
                myConnection.Close()
                Almacenamiento = "Error Generando xml" & Result
                Exit Function
            End If
            Result = ""
            Result = Copia_Imagen_Almacenada_local(Ruta_Alamce_Image,
                                                   idal,
                                                   Matri_Dcoumentos,
                                                   _Ruta_Carpeta,
                                                   Tipo_Doc_Añade)
            If Result <> "YES" Then
                myTrans.Rollback()
                If Not myConnection Is Nothing Then
                    myConnection.Close()
                End If
                Almacenamiento = "Error Copiando Imagenes " & Result
                Exit Function

            End If
            '**************************************
            'Actualiza numero imagenes en la tabla
            'Disco detalle
            '**************************************
            Dim Parametro_A As String = "select NUMERO_IMAGENES  from disco_detalle  where disco = '" & disc & "'" &
            " and gabinete ='" & _Nombre_Gabienete & "'" & " for update "
            myCommand2.CommandText = Parametro_A
            mySqldatReader = myCommand2.ExecuteReader()
            If mySqldatReader Is Nothing Then
                Almacenamiento = "Error sql para encontrar disco commando " & Parametro_A
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            If mySqldatReader.HasRows = False Then
                Almacenamiento = "Imposible Encontrar disco en la tabla disco detalle"
                myTrans.Rollback()
                mySqldatReader.Close()
                myConnection.Close()
                Exit Function

            End If
            mySqldatReader.Read()
            Dim IncreNumPage As Integer = mySqldatReader.Item(0)
            IncreNumPage = IncreNumPage + Val(pagi)
            mySqldatReader.Close()
            Dim SqlActualiza As String = "Update disco_detalle set NUMERO_IMAGENES=" & IncreNumPage & " , NUMPAG_CARP=" & Numero_Pagina_Carp & " where disco = '" & disc & "'" &
            " and gabinete ='" & _Nombre_Gabienete & "'"
            myCommand2.CommandText = SqlActualiza
            myCommand2.ExecuteNonQuery()
            myTrans.Commit()
            myConnection.Close()
            '******************************************************
            'Se agreag esta linea para devolber imagen indexada
            '******************************************************
            Id_Almacen = idal
            If estado_exml_archivo = "YES" Then
                xmlArchivo.Save(Ruta_archivo_xml)
            End If
        Catch e As Exception
            Try
                If Not mySqldatReader Is Nothing Then
                    mySqldatReader.Close()
                End If

            Catch ex As MySqlException
                myTrans.Rollback()
                If Not myTrans.Connection Is Nothing Then
                    Almacenamiento = "An exception of type " + ex.GetType().ToString() +
                                      " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Almacenamiento = "Error General " & e.Message
            Exit Function
        End Try
        myConnection.Close()
        '*********************************************
        'Determina si elimina la imagen de la carpeta
        '*********************************************
        Almacenamiento = "YES"
    End Function
    Public Function Almacenamiento_local(ByVal _Ruta_Carpeta As String,
    ByVal _Nombre_Documento As String, ByVal _Nombre_Gabienete As String,
    ByVal _Estado_Elimina As Integer, ByRef _Matri_Datos() As String,
    ByVal _Tipo_Alamcenamiento As Integer, ByVal Numero_Pag As Integer,
    ByVal Tipo_Doc As Integer, ByRef Matri_Dcoumentos() As String, ByVal Evalua_Campo_Obli As Integer,
    ByRef Id_Almacen As Integer, ByVal Tipo_Doc_Añade As Integer, ByVal Login_Usuario As String) As String
        Dim Result As String = ""
        'Dim RefClaasConsultaGabinete As New ClassConsultaGabinete
        Dim _Ruta_Almacenamiento As String = ""
        'System.Windows.Forms.Application.DoEvents()
        '****************************************
        'Consulta ruta almacenamiento
        '****************************************
        Result = ""
        Dim ref_Class_SYSTEM1RUT As New Class_SYSTEM1RUT
        Result = ref_Class_SYSTEM1RUT.Consulta_Ruta_Almacenamiento(_Ruta_Almacenamiento,
                                                                   _Nombre_Gabienete)
        If Result <> "YES" Then
            Almacenamiento_local = Result
            Exit Function
        End If

        '*************************************
        'Verifica que la imagen se encuentre
        'en la ruta    _Ruta_Carpeta
        '*************************************
        'If File.Exists(_Ruta_Carpeta & _Nombre_Documento) = False Then
        'Almacenamiento = " Error -2 Funcion Alamcenamiento No se encontro imagen para almacenar " & _Ruta_Carpeta & _Nombre_Documento
        'Exit Function
        'End If
        '**************************************************************
        'Contruye archivo preindex para eviar al cliente que almacena
        '**************************************************************
        Dim Ref_Nombre_Documento As String = ""
        Dim Ceros_Documento As String = ""
        'Result = Contruye_Nombre_Archvio_Index(_Ruta_Carpeta, _Nombre_Documento, Ceros_Documento, Ref_Nombre_Documento)
        'If Result <> "YES" Then
        '    Almacenamiento = "Error Construyendo nombre del archivo preindex Archivo preindex " & Result
        '    Exit Function
        'End If
        '****************************************
        'Verifica la existencia del archivo 
        'Preindex si es almacenamineto por bach
        'se evalua tipo almacenamiento variable
        '_Tipo_Alamcenamiento si es uno almacena
        'por bach y es necesario leer el archivo
        'para los datos
        '*****************************************
        Dim Nombre_Archivo_Preindex As String = ""
        Dim Tipo_Archivo As String = ""
        '*********************************
        'Determina si se lee archivo 
        'Preindex
        '*********************************
        If _Tipo_Alamcenamiento = 1 Then
            If File.Exists(_Ruta_Carpeta & Ceros_Documento & Ref_Nombre_Documento & ".xmls") = True Then
                Nombre_Archivo_Preindex = _Ruta_Carpeta & Ceros_Documento & Ref_Nombre_Documento & ".xmls"
                Tipo_Archivo = ".xmls"
            End If
            If File.Exists(_Ruta_Carpeta & Ceros_Documento & Ref_Nombre_Documento & ".txt") = True Then
                Nombre_Archivo_Preindex = _Ruta_Carpeta & Ceros_Documento & Ref_Nombre_Documento & ".txt"
                Tipo_Archivo = ".txt"
            End If
            If Nombre_Archivo_Preindex = "" Then
                Almacenamiento_local = "Documento sin preindexacion"
                Exit Function
            End If
            '****************************
            'Lee archivo Preindex (txt)
            '****************************
            If Tipo_Archivo = ".txt" Then
                Result = Leer_Archivo_Preindex(Nombre_Archivo_Preindex, _Matri_Datos)
                If Result <> "YES" Then
                    Almacenamiento_local = "Imposible Leer Archivo Preindex " & Nombre_Archivo_Preindex & " Error " & Result
                    Exit Function
                End If
            End If
        End If
        '**************************************************************
        'codigo que permite verificar que los datos de los campos
        'obligatorios contengan la informacion del archivo
        '**************************************************************
        Dim Matri_Campos_Obli() As String
        Erase Matri_Campos_Obli
        Result = ""
        Dim ref_Class_DETALLE_GABIENETE As New Class_DETALLE_GABIENETE
        Result = ref_Class_DETALLE_GABIENETE.Consulta_Campos_Obligatorio(_Nombre_Gabienete,
                                                                         Matri_Campos_Obli)
        If Result <> "YES" Then
            Almacenamiento_local = "Imposible Encontrar datos para campos obligatorios Error " & Result
            Exit Function
        End If
        If Matri_Campos_Obli Is Nothing Then
            Almacenamiento_local = "Matri Campos es nula consulte gabinete_detalle"
            Exit Function
        End If
        If UBound(Matri_Campos_Obli) <> UBound(_Matri_Datos) Then
            Almacenamiento_local = "Las matrices de datos y campos no son iguales es posible que el preindex pertenezca a otro gabinete "
            Exit Function
        End If
        '**************************************
        'Determina si evalua los campos obli
        'gatorios
        '**************************************
        If Evalua_Campo_Obli = 1 Then
            For z As Integer = 0 To UBound(Matri_Campos_Obli)
                Dim Matri_Tempo() As String
                Erase Matri_Tempo
                Matri_Tempo = Matri_Campos_Obli(z).Split("|")
                If Matri_Tempo(0) = 1 And _Matri_Datos(z) = "" Then
                    Almacenamiento_local = "El Campo " & Matri_Tempo(1).ToString & " es obligatorio "
                    Exit Function
                End If
            Next
        End If
        '*****************************************************************
        'consulta la base de datos system para traer los datos del sistema
        'esta consulta bloquea la base de datos y bloquea este registro
        'para que los demas usuarios no lo modifiquen al tiempo
        '*****************************************************************
        Dim carpealma As String = ""
        Dim numcarpvar As Integer = 0
        Dim Numero_Pagina_Carp As Integer = 0
        Dim tandiscvar As Long = 0
        Dim idal As Integer = 0
        Dim disc As Integer = 0
        Dim useral As String = Login_Usuario
        Dim pagi As Integer = Numero_Pag
        Dim indexal As Integer = Tipo_Doc
        Dim time1al As String = Date.Now.ToString
        Dim date1al As String = Date.Today
        Dim TempoFecha As String = Left(time1al, 10)
        time1al = Trim(time1al.Replace(TempoFecha, ""))
        Result = ""
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
        If Result <> "YES" Then
            Almacenamiento_local = "Error formateando fecha almacenamiento Funcion: Formatea_Fecha_Almacenamiento " & Result
            Exit Function
        End If
        Dim mySqldatReader As MySqlDataReader
        'Dim myConnection As New Conect.vb.Dbase_Conction_Mysql
        Dim ref As New ClassAlmacenamiento
        Dim Parametro_Conexion As String = ""
        ref.Funcion_Lee_Archivo_Configuracion(Parametro_Conexion)
        Dim myConnection As New MySqlConnection(Parametro_Conexion)
        Dim myTrans As MySqlTransaction
        Try
            myConnection.Open()
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            Dim Parametro_Select_System1 As String = " SELECT disco,proxid,tamdisc,numcarp,NUMPAG_CARP FROM system1 where nombre = " _
            & "'" & _Nombre_Gabienete & "' " & "for update"
            myCommand.CommandText = Parametro_Select_System1
            mySqldatReader = myCommand.ExecuteReader()
            If mySqldatReader Is Nothing Then
                Almacenamiento_local = "Imposible Encontrar Registro En Tabla Systema Error Conexion"
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            If mySqldatReader.HasRows = False Then
                Almacenamiento_local = "Imposible Encontrar Registro En Tabla Sistema"
                'myTrans.Rollback()
                mySqldatReader.Close()
                myConnection.Close()
                Exit Function

            End If
            '*******************************************************
            'Valores recuperados de la consulta de la tabla system1
            '********************************************************
            mySqldatReader.Read()
            numcarpvar = mySqldatReader.Item("NUMCARP")
            tandiscvar = mySqldatReader.Item("TAMDISC")
            idal = mySqldatReader.Item("PROXID")
            Numero_Pagina_Carp = mySqldatReader.Item("NUMPAG_CARP")
            idal = idal + 1
            disc = mySqldatReader.Item("DISCO")
            '***************************************************
            'validacion del tamaño de disco valor tabal system
            '***************************************************
            mySqldatReader.Close()
            If tandiscvar = 572523149 Or tandiscvar = 4310948432 Then
            Else
                Almacenamiento_local = "Tamaño de disco incorrecto Consulte su amnistrador valor : " & tandiscvar
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '****************************************************
            'validar que la carpeta sea diferente de cero
            '***************************************************
            If numcarpvar = 0 Then
                Almacenamiento_local = "Valor incorrecto de carpeta Consulte su amnistrador valor : " & numcarpvar
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '**************************************************
            'validar que el numero del disco sea valido
            '**************************************************
            If disc = 0 Then
                Almacenamiento_local = "Valor incorrecto de disco Consulte su amnistrador valor : " & disc
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '****************************************************
            'Consulta que el usuario este registrado en el sistema
            '****************************************************
            If useral = "" Then
                Almacenamiento_local = "Usuario no valido"
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '*****************************************************
            'consulta que el ide sea diferente de ""
            '*****************************************************
            If idal = 0 Then
                Almacenamiento_local = "Valor incorrecto de identidad de imagen Consulte su amnistrador valor : " & idal
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '******************************************************
            'consulta que le numero de paginas sea diferente de "0"
            '******************************************************
            If pagi = 0 Then
                Almacenamiento_local = "Valor incorrecto de paginas Consulte su amnistrador valor : " & pagi
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '**************************************
            'Consulta que el disco tenga espacion
            'donde se guardan las imagenes SL
            '**************************************
            Dim EstadoDisco As String = ""
            Dim ResulDisco As String = ""
            Dim ref_Class_disco_detalle As New Class_disco_detalle
            ResulDisco = ref_Class_disco_detalle.Numero_Imagenes(_Nombre_Gabienete,
                                                                 tandiscvar,
                                                                 disc,
                                                                 EstadoDisco)
            If ResulDisco = "YES" Then
                If EstadoDisco = "SL" Then
                    Almacenamiento_local = "Disco  " & disc.ToString & " Sobrepaso el limite de capacidad"
                    'myConnection.Close()
                    'myTrans.Rollback()
                    myConnection.Close()
                    Exit Function

                End If
            Else
                'myTrans.Rollback()
                myConnection.Close()
                Almacenamiento_local = ResulDisco
                Exit Function
            End If
            '*****************************************************
            'Actualizacion para determinar el numero de imagenes
            'por base de datos para evitar contar los archivos
            'del directorio y agilizar la funcion
            '****************************************************
            Dim Valor_Suma_Imagen As Integer = Numero_Pag + Numero_Pagina_Carp
            If Valor_Suma_Imagen > 230 Then
                numcarpvar = numcarpvar + 1
                Numero_Pagina_Carp = Numero_Pag
            Else
                Numero_Pagina_Carp = Valor_Suma_Imagen

            End If
            Dim Parametro_Actualiza_System1 As String = "update system1 set proxid = " & "'" & idal & "' ," &
            " numcarp = " & " '" & numcarpvar & "', NUMPAG_CARP=" & Numero_Pagina_Carp & " where nombre =" & "'" &
            _Nombre_Gabienete & "'" & "and proxid <> " & "'" & idal & "'"
            myCommand.CommandText = Parametro_Actualiza_System1
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            '*********************************
            'Determina si se actualizo
            'el nuevo id de la base de datos
            '*********************************
            If Switc = 0 Then
                Almacenamiento_local = "Imposible actualizar la tabla System  : " & Parametro_Actualiza_System1
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myTrans.Commit()
        Catch e As Exception
            Try

            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    myTrans.Rollback()
                    myConnection.Close()
                    Almacenamiento_local = "An exception of type " + ex.GetType().ToString() +
                                      " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try
            If Not myTrans Is Nothing Then
                myTrans.Rollback()
            End If
            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Almacenamiento_local = "Error General " & e.ToString
            Exit Function
        End Try

        '*****************************************************
        'verifica existencia de carpeta y crea nueva carpeta
        '*****************************************************
        Dim rut2000 As String = ""
        Dim Result_Carpeta As String
        Dim RutaCarpet As String
        RutaCarpet = _Ruta_Almacenamiento & _Nombre_Gabienete & disc
        Result_Carpeta = Solicita_Carpeta_almacenamiento(carpealma, numcarpvar, rut2000, RutaCarpet)
        If Result_Carpeta <> "YES" Then
            Almacenamiento_local = "Imposible calcular tamaño carpeta Consulte su administrador  : " & Result_Carpeta
            'myTrans.Rollback()
            'myConnection.Close()

            Exit Function
        End If
        carpealma = carpealma & numcarpvar

        Try

            '*************************************
            'Arma sql de almacenamiento
            '*************************************
            Dim Matri_Xml() As String
            Erase Matri_Xml
            Dim Campos_Insert As String = "ID,DISC,PAG,DBT,IDEX,USER,DATE1,TIME1"
            Dim Datos_Insert As String = idal & "," & " " & disc & " " & "," & "'" & pagi & "'" & "," & "'" & Tipo_Doc & "'" & "," & "'" & numcarpvar & "'" & ","
            Datos_Insert = Datos_Insert & "'" & useral & "'" & "," & "'" & date1al & "'" & "," & "'" & time1al & "'"
            For z2 As Integer = 0 To UBound(Matri_Campos_Obli)
                Dim Matri_Tempo() As String
                Erase Matri_Tempo
                Matri_Tempo = Matri_Campos_Obli(z2).Split("|")
                Campos_Insert = Campos_Insert & "," & Matri_Tempo(1).ToString
            Next
            For z3 As Integer = 0 To UBound(_Matri_Datos)
                If _Matri_Datos(z3) = "" Then
                    Datos_Insert = Datos_Insert & "," & "null"
                Else
                    Datos_Insert = Datos_Insert & ",'" & _Matri_Datos(z3) & "'"
                End If

            Next
            Dim Switc2 As Integer = 0
            Dim Parametro_Insert_Registro As String = "Insert into " & _Nombre_Gabienete & "( " & Campos_Insert & " )" & " Values " & "( " & Datos_Insert & " )"
            Dim myCommand2 As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand2.Connection = myConnection
            myCommand2.Transaction = myTrans
            myCommand2.CommandText = Parametro_Insert_Registro
            Switc2 = myCommand2.ExecuteNonQuery()
            '************************************
            'Determina si se inserto el registro
            'el nuevo id de la base de datos
            '*************************************
            If Switc2 = 0 Then
                Almacenamiento_local = "Imposible agregar registro  : " & Parametro_Insert_Registro
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '***********************************
            'Crea archivo xml para imagen
            '***********************************
            ReDim Preserve Matri_Xml(0)
            Matri_Xml(0) = "ID¬" & idal
            ReDim Preserve Matri_Xml(1)
            Matri_Xml(1) = "DISC¬" & disc
            ReDim Preserve Matri_Xml(2)
            Matri_Xml(2) = "PAG¬" & pagi
            ReDim Preserve Matri_Xml(3)
            Matri_Xml(3) = "DBT¬" & Tipo_Doc
            ReDim Preserve Matri_Xml(4)
            Matri_Xml(4) = "IDEX¬" & numcarpvar
            ReDim Preserve Matri_Xml(5)
            Matri_Xml(5) = "USER¬" & useral
            ReDim Preserve Matri_Xml(6)
            Matri_Xml(6) = "DATE1¬" & date1al
            ReDim Preserve Matri_Xml(7)
            Matri_Xml(7) = "TIME1¬" & time1al
            Dim IncreMat As Integer = 7
            For z2 As Integer = 0 To UBound(Matri_Campos_Obli)
                IncreMat = IncreMat + 1
                Dim Matri_Tempo() As String
                Erase Matri_Tempo
                Matri_Tempo = Matri_Campos_Obli(z2).Split("|")
                Campos_Insert = Campos_Insert & "," & Matri_Tempo(1).ToString
                ReDim Preserve Matri_Xml(IncreMat)
                Matri_Xml(IncreMat) = Matri_Tempo(1).ToString & "¬"
                If _Matri_Datos(z2) = "" Then
                    Matri_Xml(IncreMat) = Matri_Xml(IncreMat) & "null"
                Else
                    Matri_Xml(IncreMat) = Matri_Xml(IncreMat) & _Matri_Datos(z2)
                End If
            Next
            Dim Ruta_Alamce_Image As String = _Ruta_Almacenamiento & _Nombre_Gabienete & disc & "\" & carpealma & "\"
            Result = ""
            Result = Generando_Archivo_Dat_Xml(Ruta_Alamce_Image, idal, Matri_Xml, _Ruta_Carpeta)
            If Result <> "YES" Then
                myTrans.Rollback()
                myConnection.Close()
                Almacenamiento_local = "Error Generando xml" & Result
                Exit Function
            End If
            Result = ""
            Result = Copia_Imagen_Almacenada_local(Ruta_Alamce_Image, idal, Matri_Dcoumentos, _Ruta_Carpeta, Tipo_Doc_Añade)
            If Result <> "YES" Then
                myTrans.Rollback()
                If Not myConnection Is Nothing Then
                    myConnection.Close()
                End If
                Almacenamiento_local = "Error Copiando Imagenes " & Result
                Exit Function

            End If
            '**************************************
            'Actualiza numero imagenes en la tabla
            'disco detalle
            '**************************************
            Dim Parametro_A As String = "select NUMERO_IMAGENES  from disco_detalle  where disco = '" & disc & "'" &
            " and gabinete ='" & _Nombre_Gabienete & "'" & " for update "
            myCommand2.CommandText = Parametro_A
            mySqldatReader = myCommand2.ExecuteReader()
            If mySqldatReader Is Nothing Then
                Almacenamiento_local = "Error sql para encontrar disco commando " & Parametro_A
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            If mySqldatReader.HasRows = False Then
                Almacenamiento_local = "Imposible Encontrar disco en la tabla disco detalle"
                myTrans.Rollback()
                mySqldatReader.Close()
                myConnection.Close()
                Exit Function

            End If
            mySqldatReader.Read()
            Dim IncreNumPage As Integer = mySqldatReader.Item(0)
            IncreNumPage = IncreNumPage + Val(pagi)
            mySqldatReader.Close()
            Dim SqlActualiza As String = "Update disco_detalle set NUMERO_IMAGENES=" & IncreNumPage & " , NUMPAG_CARP=" & Numero_Pagina_Carp & " where disco = '" & disc & "'" &
            " and gabinete ='" & _Nombre_Gabienete & "'"
            myCommand2.CommandText = SqlActualiza
            myCommand2.ExecuteNonQuery()
            myTrans.Commit()
            '******************************************************
            'Se agreag esta linea para devolber imagen indexada
            '******************************************************
            Id_Almacen = idal
        Catch e As Exception
            Try
                If Not mySqldatReader Is Nothing Then
                    mySqldatReader.Close()
                End If
                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Almacenamiento_local = "An exception of type " + ex.GetType().ToString() +
                                      " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Almacenamiento_local = "Error General " & e.ToString
            Exit Function
        End Try
        myConnection.Close()
        '*********************************************
        'Determina si elimina la imagen de la carpeta
        '*********************************************
        Almacenamiento_local = "YES"
    End Function
    Public Function AlmacenamientoLINK(ByVal _Ruta_Carpeta As String,
   ByVal _Nombre_Documento As String, ByVal _Nombre_Gabienete As String,
   ByVal _Estado_Elimina As Integer, ByRef _Matri_Datos() As String,
   ByVal _Tipo_Alamcenamiento As Integer, ByVal Numero_Pag As Integer,
   ByVal Tipo_Doc As Integer, ByVal Matri_Dcoumentos() As String, ByVal Evalua_Campo_Obli As Integer, ByVal Id_Almacen As Integer, ByVal Login_Usuario As String)
        '***************************************************
        'Funcion: Funcion que permite crear un almacena
        'miento utilizando una imagen almacenada esta
        'funcion alamacena en el campo DISC el id del docu
        'mento principal como negativo 
        'FECHA 2012-07-31
        'Ing Miguel Angel Urueta Miranda
        '****************************************************
        If Id_Almacen = -1 Then
            AlmacenamientoLINK = "El preindex no permite crear link de imagen"
            Exit Function
        End If
        '**************************************************************
        'Codigo que permite verificar que los datos de los campos
        'obligatorios contengan la informacion del archivo
        '**************************************************************
        Dim Matri_Campos_Obli() As String
        Erase Matri_Campos_Obli
        Dim Result = ""
        Dim ref_Class_DETALLE_GABIENETE As New Class_DETALLE_GABIENETE
        Result = ref_Class_DETALLE_GABIENETE.Consulta_Campos_Obligatorio(_Nombre_Gabienete,
                                                                         Matri_Campos_Obli)
        If Result <> "YES" Then
            AlmacenamientoLINK = "Imposible Encontrar datos para campos obligatorios Error " & Result
            Exit Function
        End If
        If Matri_Campos_Obli Is Nothing Then
            AlmacenamientoLINK = "Matri Campos es nula consulte gabinete_detalle "
            Exit Function
        End If
        If UBound(Matri_Campos_Obli) <> UBound(_Matri_Datos) Then
            AlmacenamientoLINK = "Las matrices de datos y campos no son iguales es posible que el preindex pertenezca a otro gabinete "
            Exit Function
        End If
        '**************************************
        'Determina si evalua los campos obli
        'gatorios
        '**************************************
        If Evalua_Campo_Obli = 1 Then
            For z As Integer = 0 To UBound(Matri_Campos_Obli)
                Dim Matri_Tempo() As String
                Erase Matri_Tempo
                Matri_Tempo = Matri_Campos_Obli(z).Split("|")
                If Matri_Tempo(0) = 1 And _Matri_Datos(z) = "" Then
                    AlmacenamientoLINK = "El Campo " & Matri_Tempo(1).ToString & " es obligatorio "
                    Exit Function
                End If
            Next
        End If

        '*****************************************************************
        'consulta la base de datos system para traer los datos del sistema
        'esta consulta bloquea la base de datos y bloquea este registro
        'para que los demas usuarios no lo modifiquen al tiempo
        '*****************************************************************
        Dim carpealma As String = ""
        Dim numcarpvar As Integer = 0
        Dim Numero_Pagina_Carp As Integer = 0
        Dim tandiscvar As Long = 0
        Dim idal As Integer = 0
        Dim disc As Integer = 0
        Dim useral As String = Login_Usuario
        Dim pagi As Integer = Numero_Pag
        Dim indexal As Integer = Tipo_Doc
        Dim time1al As String = Date.Now.ToString
        Dim date1al As String = Date.Today
        Dim TempoFecha As String = Left(time1al, 10)
        time1al = Trim(time1al.Replace(TempoFecha, ""))
        Result = ""
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
        If Result <> "YES" Then
            AlmacenamientoLINK = "Error formateando fecha almacenamiento Funcion: Formatea_Fecha_Almacenamiento " & Result
            Exit Function
        End If
        Dim myConnection As New MySqlConnection("")
        Dim mySqldatReader As MySqlDataReader
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Dim myTrans As MySqlTransaction
        Dim Parametro_Select_System1 As String = " SELECT disco,proxid,tamdisc,numcarp,NUMPAG_CARP FROM system1 where nombre = " _
            & "'" & _Nombre_Gabienete & "' " & "for update"
        Try
            myConnection.Open()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = Parametro_Select_System1
            mySqldatReader = myCommand.ExecuteReader()
            If mySqldatReader Is Nothing Then
                AlmacenamientoLINK = "Imposible Encontrar Registro En Tabla Systema Error Conexion"
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            If mySqldatReader.HasRows = False Then
                AlmacenamientoLINK = "Imposible Encontrar Registro En Tabla Systema"
                'myTrans.Rollback()
                mySqldatReader.Close()
                myConnection.Close()
                Exit Function

            End If
            '*******************************************************
            'Valores recuperados de la consulta de la tabla system1
            '********************************************************
            mySqldatReader.Read()
            numcarpvar = mySqldatReader.Item("NUMCARP")
            tandiscvar = mySqldatReader.Item("TAMDISC")
            idal = mySqldatReader.Item("PROXID")
            Numero_Pagina_Carp = mySqldatReader.Item("NUMPAG_CARP")
            idal = idal + 1
            disc = mySqldatReader.Item("DISCO")
            mySqldatReader.Close()
            '****************************************************
            'validar que la carpeta sea diferente de cero
            '***************************************************
            If numcarpvar = 0 Then
                AlmacenamientoLINK = "Valor incorrecto de carpeta Consulte su amnistrador valor : " & numcarpvar
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '**************************************************
            'validar que el numero del disco sea valido
            '**************************************************
            If disc = 0 Then
                AlmacenamientoLINK = "Valor incorrecto de disco Consulte su amnistrador valor : " & disc
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '****************************************************
            'Consulta que el usuario este registrado en el sistema
            '****************************************************
            If useral = "" Then
                AlmacenamientoLINK = "Usuario no valido"
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '*****************************************************
            'consulta que el ide sea diferente de ""
            '*****************************************************
            If idal = 0 Then
                AlmacenamientoLINK = "Valor incorrecto de identidad de imagen Consulte su amnistrador valor : " & idal
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            Dim Parametro_Actualiza_System1 As String = "update system1 set proxid = " & "'" & idal & "' ," &
                   " numcarp = " & " '" & numcarpvar & "', NUMPAG_CARP=" & Numero_Pagina_Carp & " where nombre =" & "'" &
                   _Nombre_Gabienete & "'" & "and proxid <> " & "'" & idal & "'"
            myCommand.CommandText = Parametro_Actualiza_System1
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            '*********************************
            'Determina si se actualizo
            'el nuevo id de la base de datos
            '*********************************
            If Switc = 0 Then
                AlmacenamientoLINK = "Imposible actualizar la tabla System  : " & Parametro_Actualiza_System1
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myTrans.Commit()
            'AGREGA FUNCION CONSULTA DATOS SISTEMA IMAGEN NUEVA FUNCION
            Dim tipdoc As Integer = 1
            Result = Consulta_Datos_Sistema_Imagen(Id_Almacen, _Nombre_Gabienete, Tipo_Doc,
            disc, pagi, tipdoc, numcarpvar)
            If Result <> "YES" Then
                AlmacenamientoLINK = "No hay imagen para hacer link " & Result
                Exit Function
            End If
            disc = "-" & Id_Almacen
            '*************************************
            'Arma sql de almacenamiento
            '*************************************
            Dim Matri_Xml() As String
            Erase Matri_Xml
            Dim Campos_Insert As String = "ID,DISC,PAG,DBT,IDEX,USER,DATE1,TIME1"
            Dim Datos_Insert As String = idal & "," & " " & disc & " " & "," & "'" & 0 & "'" & "," & "'" & Tipo_Doc & "'" & "," & "'" & numcarpvar & "'" & ","
            Datos_Insert = Datos_Insert & "'" & useral & "'" & "," & "'" & date1al & "'" & "," & "'" & time1al & "'"
            For z2 As Integer = 0 To UBound(Matri_Campos_Obli)
                Dim Matri_Tempo() As String
                Erase Matri_Tempo
                Matri_Tempo = Matri_Campos_Obli(z2).Split("|")
                Campos_Insert = Campos_Insert & "," & Matri_Tempo(1).ToString
            Next
            For z3 As Integer = 0 To UBound(_Matri_Datos)
                If _Matri_Datos(z3) = "" Then
                    Datos_Insert = Datos_Insert & "," & "null"
                Else
                    Datos_Insert = Datos_Insert & ",'" & _Matri_Datos(z3) & "'"
                End If
            Next
            Dim Switc2 As Integer = 0
            Dim Parametro_Insert_Registro As String = "Insert into " & _Nombre_Gabienete & "( " & Campos_Insert & " )" & " Values " & "( " & Datos_Insert & " )"
            Dim myCommand2 As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand2.Connection = myConnection
            myCommand2.Transaction = myTrans
            myCommand2.CommandText = Parametro_Insert_Registro
            Switc2 = myCommand2.ExecuteNonQuery()
            '************************************
            'Determina si se inserto el registro
            'el nuevo id de la base de datos
            '*************************************
            If Switc2 = 0 Then
                AlmacenamientoLINK = "Imposible agregar registro  : " & Parametro_Insert_Registro
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            Else
                myTrans.Commit()
                myConnection.Close()
            End If

            AlmacenamientoLINK = "YES"
        Catch e As Exception
            Try

            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    myTrans.Rollback()
                    myConnection.Close()
                    AlmacenamientoLINK = "An exception of type " + ex.GetType().ToString() +
                                      " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try
            If Not myTrans Is Nothing Then
                myTrans.Rollback()
            End If
            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            AlmacenamientoLINK = "Error General " & e.ToString
            Exit Function
        End Try
    End Function
    Function Generando_Archivo_Preindex(ByVal Matri_Datos() As String,
                                               ByVal _Ruta_Carpeta As String,
                                               ByVal Ident_Arch_xml_t As String,
                                               ByVal Matri_Campos() As String,
                                               ByVal Id_Almacen As Integer) As String
        '***************************************************
        'Funcion : Generando_Archivo_Preindex
        'Fecha : 2011-02-02
        'Ing : Miguel Angel Urueta Miranda
        'Descripcion : Funcion que guarda archivo preindex
        'en la digitacion de documentos
        '****************************************************
        Dim RefFyleSystem As New FileInfo(Ident_Arch_xml_t)
        Dim Ident_Arch_xml As String = ""
        Dim Result As String = ""
        Result = Arma_Nombre_Archivo_Preindex(Ident_Arch_xml_t, Ident_Arch_xml)
        If Result <> "YES" Then
            Generando_Archivo_Preindex = "Imposible Generar Nombre Archivo xml"
            Exit Function
        End If
        Ident_Arch_xml = _Ruta_Carpeta & Ident_Arch_xml

        If File.Exists(Ident_Arch_xml) = True Then
            Kill(Ident_Arch_xml)
        End If
        If Matri_Datos Is Nothing Then
            Generando_Archivo_Preindex = "Imposible encontrar datos para archivo XML"
            Exit Function
        End If
        Dim myXmlTextWriter As XmlTextWriter = New XmlTextWriter(Ident_Arch_xml, System.Text.Encoding.UTF8)
        Try
            myXmlTextWriter.Formatting = System.Xml.Formatting.Indented
            myXmlTextWriter.WriteStartDocument(False)
            myXmlTextWriter.WriteStartElement("Gabinetes")
            myXmlTextWriter.WriteStartElement("Gabinete")
            For i As Integer = 0 To UBound(Matri_Datos)
                'myXmlTextWriter.WriteComment("Esto es un comentario")
                Dim Matri_Split() As String = Matri_Campos(i).Split("|")
                myXmlTextWriter.WriteAttributeString(Matri_Split(0), Matri_Datos(i))
            Next
            myXmlTextWriter.WriteAttributeString("ID_ALMACEN_LINK", Id_Almacen)
            myXmlTextWriter.WriteEndElement()
            myXmlTextWriter.WriteEndElement()
            myXmlTextWriter.Flush()
            myXmlTextWriter.Close()
            Generando_Archivo_Preindex = "YES"
        Catch ex As Exception
            If Not myXmlTextWriter Is Nothing Then
                myXmlTextWriter.Close()
            End If
            Generando_Archivo_Preindex = "Error general Registrando carpeta en archivo de registro " & ex.ToString
        End Try
        Generando_Archivo_Preindex = "YES"
    End Function
    Function Generando_Archivo_Preindex_cache(ByVal Matri_Datos() As String,
                                                     ByVal _Ruta_Carpeta As String,
                                                     ByRef Ident_Arch_xml_t As String,
                                                     ByVal Matri_Campos() As String,
                                                     ByVal Id_Almacen As Integer) As String
        '***************************************************
        'Funcion : Generando_Archivo_Preindex_cache
        'Fecha : 2011-02-02
        'Ing : Miguel Angel Urueta Miranda
        'Descripcion : Funcion que guarda archivo preindex
        'en la digitacion de documentos
        '****************************************************
        Ident_Arch_xml_t = _Ruta_Carpeta & Ident_Arch_xml_t
        Dim RefFyleSystem As New FileInfo(Ident_Arch_xml_t)
        Dim Ident_Arch_xml As String = ""
        Dim Result As String = ""
        If File.Exists(Ident_Arch_xml_t) = True Then
            Kill(Ident_Arch_xml_t)
        End If
        If Matri_Datos Is Nothing Then
            Generando_Archivo_Preindex_cache = "Imposible encontrar datos para archivo XML"
            Exit Function
        End If
        Dim myXmlTextWriter As XmlTextWriter = New XmlTextWriter(Ident_Arch_xml_t, System.Text.Encoding.UTF8)
        Try
            myXmlTextWriter.Formatting = System.Xml.Formatting.Indented
            myXmlTextWriter.WriteStartDocument(False)
            myXmlTextWriter.WriteStartElement("Gabinetes")
            myXmlTextWriter.WriteStartElement("Gabinete")
            For i As Integer = 0 To UBound(Matri_Datos)
                'myXmlTextWriter.WriteComment("Esto es un comentario")
                Dim Matri_Split() As String = Matri_Campos(i).Split("|")
                myXmlTextWriter.WriteAttributeString(Matri_Split(0), Matri_Datos(i))
            Next
            myXmlTextWriter.WriteAttributeString("ID_ALMACEN_LINK", Id_Almacen)
            myXmlTextWriter.WriteEndElement()
            myXmlTextWriter.WriteEndElement()
            myXmlTextWriter.Flush()
            myXmlTextWriter.Close()
            Generando_Archivo_Preindex_cache = "YES"
        Catch ex As Exception
            If Not myXmlTextWriter Is Nothing Then
                myXmlTextWriter.Close()
            End If
            Generando_Archivo_Preindex_cache = "Error general Registrando carpeta en archivo de registro " & ex.ToString
        End Try
        Generando_Archivo_Preindex_cache = "YES"
    End Function

    Function Generando_Archivo_Dat_Xml(ByVal Ruta_Alamce_Image As String,
                                              ByVal idal As Long,
                                              ByVal Matri_Datos() As String,
                                              ByVal _Ruta_Carpeta As String) As String
        '***********************************
        'Function: Genera archivo xml para
        'recuperacion de datos 
        'Fecha: 2010-10-26
        'Ing: Miguel Angel Urueta Mirandao
        '************************************
        Dim Ceros_Cuerpo_Imag As String = ""
        Dim Result As String = ""
        Dim Ident_Arch_xml As String = ""
        Result = Ceros_Imagen_Almacenada(Ceros_Cuerpo_Imag, idal)
        If Result <> "YES" Then
            Generando_Archivo_Dat_Xml = "Error generando ceros archivo xmls "
            Exit Function
        End If
        Ident_Arch_xml = Ruta_Alamce_Image & "FXL" & Ceros_Cuerpo_Imag & idal & ".xml"
        'Elimina el archivo si la matriz en nula
        If File.Exists(Ident_Arch_xml) = True Then
            Kill(Ident_Arch_xml)
        End If
        If Matri_Datos Is Nothing Then
            Generando_Archivo_Dat_Xml = "Imposible encontrar datos para archivo XML"
            Exit Function
        End If
        Dim myXmlTextWriter As XmlTextWriter = New XmlTextWriter(Ident_Arch_xml,
                                                                 System.Text.Encoding.UTF8)
        Try
            myXmlTextWriter.Formatting = System.Xml.Formatting.Indented
            myXmlTextWriter.WriteStartDocument(False)
            myXmlTextWriter.WriteStartElement("Gabinetes")
            myXmlTextWriter.WriteStartElement("Gabinete")
            For i As Integer = 0 To UBound(Matri_Datos)
                'myXmlTextWriter.WriteComment("Esto es un comentario")
                Dim Matri_Split() As String
                Erase Matri_Split
                If Not Matri_Datos(i) Is Nothing Then
                    Matri_Split = Matri_Datos(i).Split("¬")
                    myXmlTextWriter.WriteAttributeString(Matri_Split(0).ToString, Matri_Split(1).ToString)
                End If
            Next
            myXmlTextWriter.WriteEndElement()
            myXmlTextWriter.WriteEndElement()
            myXmlTextWriter.Flush()
            myXmlTextWriter.Close()
            Generando_Archivo_Dat_Xml = "YES"
        Catch ex As Exception
            If Not myXmlTextWriter Is Nothing Then
                myXmlTextWriter.Close()
            End If
            Generando_Archivo_Dat_Xml = "Error general Registrando carpeta en archivo de registro " & ex.ToString
        End Try

    End Function
    Function copyfile(ByVal VirtualPath As String,
                      ByVal Name As String,
                      ByRef Content As Object) As String
        Dim objFile As File, objStream As StreamWriter, objFstream As FileStream
        Try

            objFstream = File.Open(VirtualPath & Name, FileMode.Create, FileAccess.Write)
            Dim lngLen As Long = Content.Length
            objFstream.Write(Content, 0, CInt(lngLen))
            objFstream.Flush()
            objFstream.Close()

            copyfile = "YES"
        Catch exc As System.UnauthorizedAccessException
            copyfile = "No autorizado para pegar"
        Catch exc As Exception
            copyfile = exc.ToString
            Return exc.ToString
        Finally
            If Not objFstream Is Nothing Then
                objFstream.Close()
            End If
        End Try
    End Function
    Function Funcion_Lee_Archivo_Configuracion(ByRef conexion As String) As String
        '*****************************************
        'leer archivo configuracion docuarchi
        '*****************************************
        Try
            Dim RutaArchivoConfiguracion As String = ""
            'Dim rat As String = ConfigurationManager.AppSettings

            Dim RutaArchivoConfig As String = HttpContext.Current.Server.MapPath("./config/DocuArchiNetconfig.ini")
            'Dim ContenidoStreeam As New System.IO.StreamReader(RutaArchivoConfig)
            'Dim RutaArchivoConfig As String = "c:\Archivos de Programa\workflowDocuarchi\SetupWorkflow\workflow.ini"
            'FileOpen(1, RutaArchivoConfig, OpenMode.Input)
            'While Not EOF(1)
            '    Dim DatoConfig() As String = Split(Trim(LineInput(1)), "=")
            '    If Not DatoConfig Is Nothing Then
            '        RutaArchivoConfiguracion = DatoConfig(1)
            '    End If

            'End While
            'FileClose(1)
            'If RutaArchivoConfiguracion = "" Then
            '    MsgBox("Imposible Encontrar ruta archivo configuracion" + vbCrLf _
            '    & RutaArchivoConfig)
            '    'FormRef.Dispose()
            '    FormRef.Close()
            '    Application.Exit()
            '    Funcion_Lee_Archivo_Configuracion = "Imposible leer archivo de configuracion : " & Application.StartupPath & "\workflow.ini"

            '    Exit Function
            'End If
            Dim Datasource, Userconf, Pasw, Database, Max_Size_Pool, Ruta_Archivo_Licent_Dat As String
            FileOpen(1, RutaArchivoConfig, OpenMode.Input)
            While Not EOF(1)
                Dim DatoConfig() As String = Split(Trim(LineInput(1)), "=")
                If Not DatoConfig Is Nothing Then
                    Select Case DatoConfig(0)
                        Case "Datasource"
                            Datasource = DatoConfig(1)
                        Case "USER"
                            Userconf = Trim(DatoConfig(1))
                        Case "Version"
                            Pasw = DatoConfig(1)
                        Case "Database"
                            Database = DatoConfig(1)
                        Case "Max Size Pool"
                            Max_Size_Pool = DatoConfig(1)

                        Case "RUTA_lICENT"
                            Ruta_Archivo_Licent_Dat = DatoConfig(1)

                    End Select
                End If
            End While
            FileClose(1)
            Dim Persistencia As Boolean = True
            conexion = "Persist Security Info=" _
                          & Persistencia & ";database=" & Database & ";server=" & Datasource _
                          & ";Connect Timeout=100;user id=" & Userconf & ";pwd=" & Pasw _
                          & ";Pooling=false;Min Pool Size=0;Max Pool Size=" & Max_Size_Pool
            Funcion_Lee_Archivo_Configuracion = "YES"
        Catch ex As Exception
            Funcion_Lee_Archivo_Configuracion = ex.ToString
            'FormRef.Dispose()
            'FormRef.Close()
        End Try

    End Function

    Function Copia_Imagen_Almacenada_local(ByVal Ruta_Alamce_Image As String,
                                           ByVal idal As Integer,
                                           ByRef Matri_Dcoumentos() As Object,
                                           ByVal _Ruta_Carpeta As String,
                                           ByVal Tipo_Doc As Integer) As String
        '************************************
        'Funcion: copia imagen
        'Fecha : 2010-11-16
        'Ing : Miguel Angel Urueta Miranda
        'Descripcion: Copia las imagenes
        'almacenada en la base de datos
        '************************************
        Dim Ceros_Cuerpo_Imag As String = ""
        Dim Result As String = ""
        Dim i As Integer = 0
        Dim i2 As Integer = 0
        Dim Ceros_Ext As String
        Dim Imagen_Principal As String = ""
        Try
            Result = Ceros_Imagen_Almacenada(Ceros_Cuerpo_Imag, idal)
            If Result <> "YES" Then
                Copia_Imagen_Almacenada_local = "Error generando ceros de imagen "
                Exit Function
            End If
            Dim Extension As String = ""
            Dim visor As String = ""
            Dim Estado_doc As String = ""
            Imagen_Principal = "DIG" & Ceros_Cuerpo_Imag & idal
            Dim Class_da_extension As New Class_da_extension
            Result = Class_da_extension.RetornaExtensionTipoDocumento(Tipo_Doc,
                                                                      Extension)
            If Result <> "YES" Then
                Copia_Imagen_Almacenada_local = "Error determinando tipo documento " & Result
                Exit Function
            End If
            'If Tipo_Doc >= -1 Then
            'Extension = ".TIF"
            'End If
            'If Tipo_Doc = -2 Then
            'Extension = ".PDF"
            'End If
            'If Tipo_Doc = -3 Then
            'Extension = ".JPG"
            'End If
            'If Tipo_Doc = -4 Then
            'Extension = ".BMP"
            'End If

            'Result = copyfile(Ruta_Alamce_Image, Imagen_Principal & Extension, Matri_Dcoumentos(0))
            File.Copy(Matri_Dcoumentos(0), Ruta_Alamce_Image & Imagen_Principal & Extension)
            'If Result <> "YES" Then
            'Copia_Imagen_Almacenada_local = Result
            'Exit Function
            'End If
            'Copia_Imagen_Almacenada = "llEGO AQUI"
            'Exit Function
            If Matri_Dcoumentos.Length > 1 Then
                For i = 1 To UBound(Matri_Dcoumentos)
                    Result = ""
                    Ceros_Ext = ""
                    Result = Ceros_Imagen_Alamacenada_ext(i2, Ceros_Ext)
                    If Result <> "YES" Then
                        Copia_Imagen_Almacenada_local = "Error generando ceros extension imagen "
                        Exit Function
                    End If
                    If Not Matri_Dcoumentos(i) Is Nothing Then
                        File.Copy(Matri_Dcoumentos(i), Ruta_Alamce_Image & Imagen_Principal & "." & Ceros_Ext & i2)
                        'copyfile(Ruta_Alamce_Image, Imagen_Principal & "." & Ceros_Ext & i2, Matri_Dcoumentos(i))
                        'If Result <> "YES" Then
                        'Copia_Imagen_Almacenada_local = Result
                        'Exit Function
                        'End If
                    End If
                    i2 = i2 + 1
                Next
            End If
            Copia_Imagen_Almacenada_local = "YES"
        Catch ex As Exception
            Copia_Imagen_Almacenada_local = "Error General Funcion Copia_Imagen_Almacenada Error : " & ex.Message
        End Try
    End Function
    Function Copia_Imagen_Almacenada(ByVal Ruta_Alamce_Image As String,
                                            ByVal idal As Integer,
                                            ByRef Matri_Dcoumentos() As Object,
                                            ByVal _Ruta_Carpeta As String,
                                            ByVal Tipo_Doc As Integer) As String
        '************************************
        'Funcion: copia imagen
        'Fecha : 2010-11-16
        'Ing : Miguel Angel Urueta Miranda
        'Descripcion: Copia las imagenes
        'almacenada en la base de datos
        '************************************
        Dim Ceros_Cuerpo_Imag As String = ""
        Dim Result As String = ""
        Dim i As Integer = 0
        Dim i2 As Integer = 0
        Dim Ceros_Ext As String
        Dim Imagen_Principal As String = ""
        Try
            Result = Ceros_Imagen_Almacenada(Ceros_Cuerpo_Imag, idal)
            If Result <> "YES" Then
                Copia_Imagen_Almacenada = "Error generando ceros de imagen "
                Exit Function
            End If
            Dim Extension As String = ""
            Dim visor As String = ""
            Dim Estado_doc As String = ""
            Imagen_Principal = "DIG" & Ceros_Cuerpo_Imag & idal
            Dim Class_da_extension As New Class_da_extension
            Result = Class_da_extension.RetornaExtensionTipoDocumento(Tipo_Doc,
                                                                      Extension)
            If Result <> "YES" Then
                Copia_Imagen_Almacenada = "Error determinando tipo documento " & Result
                Exit Function
            End If
            'If Tipo_Doc >= -1 Then
            'Extension = ".TIF"
            'End If
            'If Tipo_Doc = -2 Then
            'Extension = ".PDF"
            'End If
            'If Tipo_Doc = -3 Then
            'Extension = ".JPG"
            'End If
            'If Tipo_Doc = -4 Then
            'Extension = ".BMP"
            'End If

            Result = copyfile(Ruta_Alamce_Image, Imagen_Principal & Extension, Matri_Dcoumentos(0))
            'File.Copy(Matri_Dcoumentos(0), Ruta_Alamce_Image & Imagen_Principal & Extension)
            If Result <> "YES" Then
                Copia_Imagen_Almacenada = Result
                Exit Function
            End If
            'Copia_Imagen_Almacenada = "llEGO AQUI"
            'Exit Function
            If Matri_Dcoumentos.Length > 1 Then
                For i = 1 To UBound(Matri_Dcoumentos)
                    Result = ""
                    Ceros_Ext = ""
                    Result = Ceros_Imagen_Alamacenada_ext(i2, Ceros_Ext)
                    If Result <> "YES" Then
                        Copia_Imagen_Almacenada = "Error generando ceros extension imagen "
                        Exit Function
                    End If
                    If Not Matri_Dcoumentos(i) Is Nothing Then
                        copyfile(Ruta_Alamce_Image, Imagen_Principal & "." & Ceros_Ext & i2, Matri_Dcoumentos(i))
                        If Result <> "YES" Then
                            Copia_Imagen_Almacenada = Result
                            Exit Function
                        End If
                    End If
                    i2 = i2 + 1
                Next
            End If
            Copia_Imagen_Almacenada = "YES"
        Catch ex As Exception
            Copia_Imagen_Almacenada = "Error General Funcion Copia_Imagen_Almacenada Error : " & ex.Message
        End Try
    End Function
    Function Ceros_Imagen_Alamacenada_ext(ByVal Val_Ext As Integer, ByRef Ceros_Ext As String) As String
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
    Public Function Ceros_Imagen_Carpeta(ByVal Valor_Imagen As String, ByRef Ceros_Imagen As String) As String
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
    Function Ceros_Imagen_Almacenada(ByRef Ceros_Cuerpo_Imag As String, ByVal Idal As Long) As String
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
    Function Solicita_Carpeta_almacenamiento(ByRef carpealma As Object,
                                             ByVal numcarpvar As Object,
                                             ByRef rut2000 As String,
                                             ByVal RutaCarpet As String) As String
        '****************************************************
        'Ing : Miguel Angel Urueta
        'Modificado : 2010-09-01
        'Calcula el tamaño de la carpeta
        '***************************************************
        'Arma ruta carpeta almacenamiento
        '**************************************************
        Dim Result As String = ""
        Dim Tama_Archivo As Long = 0
        Try
            rut2000 = ""
            carpealma = ""
            Result = Ceros_Carpeta_Almacenamiento(carpealma,
                                                  numcarpvar)
            If Result <> "YES" Then
                Solicita_Carpeta_almacenamiento = "Error generando carpeta almacenamiento " & Result
                Exit Function
            End If
            '*******************************
            'Verifica la existencia de la
            'ruta del documento sin la
            'carpeta
            '*******************************
            If Directory.Exists(RutaCarpet) = False Then
                Solicita_Carpeta_almacenamiento = "Imposible encontrar ruta alamacenamiento " & RutaCarpet
                Exit Function
            End If
            rut2000 = RutaCarpet & "\" & carpealma & numcarpvar
            '*****************************************
            'Verifica la existencia de la ruta de la
            'carpeta para calcular el temaño
            '*****************************************
            If Directory.Exists(rut2000) = True Then
                'Result = ""
                'Result = GetFolderSize(rut2000, False, Tama_Archivo)
                'If Result <> "YES" Then
                'Tamaño_Carpeta = "Error Consultando tamaño disco " & Result
                'Exit Function
                'End If
                'If Tama_Archivo >= 8422054 Then
                '*********************************
                'Incrementa a la nueva carpeta
                '*********************************
                'numcarpvar = Val(numcarpvar) + 1
                'carpealma = ""
                'Result = Ceros_Carpeta_Almacenamiento(carpealma, numcarpvar)
                'If Result <> "YES" Then
                'Tamaño_Carpeta = Result
                'Exit Function
                'End If
                '***************************************************
                'reconsulta que la carpeta nueva a instalar exista
                '***************************************************
                'If Directory.Exists(RutaCarpet & "\" & carpealma & numcarpvar) = True Then
                'Else
                '   Directory.CreateDirectory(RutaCarpet & "\" & carpealma & numcarpvar)
                'End If
                'End If 'cierra if de tamaño de disco
            Else
                Directory.CreateDirectory(rut2000)
            End If
            Solicita_Carpeta_almacenamiento = "YES"
        Catch ex As Exception
            Solicita_Carpeta_almacenamiento = "Inconsistencia general funcion Solicita_ruta_Carpeta " & ex.Message
            Exit Function
        End Try
    End Function
    Function GetFolderSize(ByVal DirPath As String,
                           ByVal includeSubFolders As Boolean,
                           ByRef ZiseRef As Long) As String
        Try
            Dim size As Long = 0
            Dim diBase As New DirectoryInfo(DirPath)
            Dim files() As FileInfo
            If includeSubFolders Then
                files = diBase.GetFiles("*", SearchOption.AllDirectories)
            Else
                files = diBase.GetFiles("*", SearchOption.TopDirectoryOnly)
            End If
            Dim ie As IEnumerator = files.GetEnumerator
            While ie.MoveNext
                size += DirectCast(ie.Current, FileInfo).Length
            End While
            ZiseRef = size
            GetFolderSize = "YES"
        Catch ex As Exception
            GetFolderSize = ex.ToString
        End Try
    End Function
    Function Ceros_Carpeta_Almacenamiento(ByRef carpealma As String,
                                          ByVal numcarpvar As Integer) As String
        '*********************************
        'Funcion : Arma los ceros
        'de la carpeta de alamcenamieto
        'Ing : Miguel Angel Urueta Miranda 
        'Fecha : 2010-09-01
        '*********************************
        Try
            Select Case Len(numcarpvar.ToString)
                Case "1"
                    carpealma = carpealma & "0000"
                Case "2"
                    carpealma = carpealma & "000"
                Case "3"
                    carpealma = carpealma & "00"
                Case "4"
                    carpealma = carpealma & "0"
                Case "5"
                    carpealma = carpealma & ""
            End Select
            Ceros_Carpeta_Almacenamiento = "YES"
        Catch ex As Exception
            Ceros_Carpeta_Almacenamiento = ex.ToString
        End Try
    End Function

    Function Leer_Archivo_Preindex(ByVal Nombre_Archivo_Preindex As String,
                                   ByRef Matri_Datos_index() As String) As String
        '**************************************
        'Funcion : Leer Archivo Preindex
        'Descripcion : funcion que lee archivo
        'preindes y genera una matriz de datos
        '**************************************
        Try
            File.SetAttributes(Nombre_Archivo_Preindex, FileAttributes.Normal)
            FileOpen(1, Nombre_Archivo_Preindex, OpenMode.Input)
            Dim IdentiArchvo As String = ""
            Dim i As Integer = 0
            Do While Not EOF(1)

                IdentiArchvo = Trim(LineInput(1))
                If IdentiArchvo <> "#fin#" Then
                    ReDim Preserve Matri_Datos_index(i)
                    Matri_Datos_index(i) = IdentiArchvo
                End If
                i = i + 1
            Loop
            FileClose(1)
            File.SetAttributes(Nombre_Archivo_Preindex, FileAttributes.Hidden)
            Leer_Archivo_Preindex = "YES"
        Catch ex As Exception
            FileClose(1)
            Leer_Archivo_Preindex = ex.ToString
        End Try
    End Function
    Function Contruye_Nombre_Archvio_Index(ByVal _Ruta_Carpeta As String,
                                           ByVal _Nombre_Documento As String,
                                           ByRef Ceros_Documento As String,
                                           ByRef Ref_Nombre_Documento As String) As String
        '***************************************
        'Function : Genera el nombre del 
        'archivo de preindesacion de imagenes
        'desde el nombre del archivo y la gene
        'racion de ceros
        'Ing : Miguel Angel Urueta 
        'Fecha : 2010-09-02
        '***************************************
        Dim Result As String = ""
        Try
            Ref_Nombre_Documento = _Nombre_Documento
            Ref_Nombre_Documento = Left(Ref_Nombre_Documento, 8)
            Ref_Nombre_Documento = Ref_Nombre_Documento.Replace("DIG", "")
            Ref_Nombre_Documento = Val(Ref_Nombre_Documento)
            '*****************************
            'Genera ceros del index txt
            '*****************************
            Result = Lee_Archivo_Txt_Index_Ceros_Txt(Ref_Nombre_Documento,
                                                     Ceros_Documento)
            If Result <> "YES" Then
                Contruye_Nombre_Archvio_Index = "Error Generando ceros del archivo index Descri Error " &
                Result & " Documento Relacionado " & _Nombre_Documento
                Exit Function

            End If
            Contruye_Nombre_Archvio_Index = "YES"
        Catch ex As Exception
            Contruye_Nombre_Archvio_Index = "Inconsistencia general función Contruye_Nombre_Archvio_Index " & ex.Message
        End Try
    End Function
    Function Lee_Archivo_Txt_Index_Ceros_Txt(ByVal Numero_Doc As String,
                                             ByRef Ceros_Documento As String) As String
        '*********************************************************
        'Ing : Miguel Angle Urueta
        'Fecha : 2010-08-29
        'Descripcion : Fucion genera los ceros para el archivo
        'idenx txt de la version 3.1b 
        '*********************************************************
        Try
            Select Case Len(Numero_Doc)
                Case "1"
                    Ceros_Documento = Ceros_Documento & "0000000"
                Case "2"
                    Ceros_Documento = Ceros_Documento & "000000"
                Case "3"
                    Ceros_Documento = Ceros_Documento & "00000"
                Case "4"
                    Ceros_Documento = Ceros_Documento & "0000"
                Case "5"
                    Ceros_Documento = Ceros_Documento & "000"
                Case "6"
                    Ceros_Documento = Ceros_Documento & "00"
                Case "7"
                    Ceros_Documento = Ceros_Documento & "0"
                Case "8"
                    Ceros_Documento = ""
            End Select
            Lee_Archivo_Txt_Index_Ceros_Txt = "YES"
        Catch ex As Exception
            Lee_Archivo_Txt_Index_Ceros_Txt = ex.ToString
        End Try
    End Function
    Function Consulta_Datos_Sistema_Imagen(ByVal Id_imagen As String,
                                                  ByVal Nombre_Gabinete As String,
                                                  ByVal Tipo_Imagen As Integer,
                                                  ByRef DISC As Integer,
                                                  ByRef PAG As Integer,
                                                  ByRef DBT As Integer,
                                                  ByRef IDEX As Integer) As String
        '***************************************************************
        'Funcion Consulta_Datos_Sistema_Imagen
        'Fecha 2013-08-09
        'Ingeniero : Miguel Angel Urueta Miranda
        'Funcion modifcada para cumplir con los parametros de conexion
        'del sistema web 
        '****************************************************************
        Try
            Dim tipoprinc As Integer = -1
            Dim tipoprincañade As Integer = -1
            '****************************************
            'Determina el tipo documento principal
            '****************************************
            '---caso tif
            If Tipo_Imagen = -11 Then
                tipoprinc = -1
                tipoprincañade = -10
            End If
            '---caso pdf
            If Tipo_Imagen = -22 Then
                tipoprinc = -2
                tipoprincañade = -20
            End If
            '---caso jpg
            If Tipo_Imagen = -33 Then
                tipoprinc = -3
                tipoprincañade = -30
            End If
            '---caso bmp
            If Tipo_Imagen = -44 Then
                tipoprinc = -4
                tipoprincañade = -40
            End If

            Dim Sql_consulta = "SELECT DISC,PAG,DBT,IDEX FROM " &
                     Nombre_Gabinete &
                     " WHERE ID=" & Id_imagen
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("DETALLE_GABIENETE")
            Dim result As String = ref.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If result <> "YES" Then
                Consulta_Datos_Sistema_Imagen = "Error Consultando en tabla " & Nombre_Gabinete = "Error Consultando en tabla 36 " & " " & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Consulta_Datos_Sistema_Imagen = "Imposible datos en el sistema para este id imagen" & Id_imagen
                Exit Function
            Else
                DISC = Datset.Tables(0).Rows(0).Item(0)
                PAG = Datset.Tables(0).Rows(0).Item(1)
                DBT = Datset.Tables(0).Rows(0).Item(2)
                IDEX = Datset.Tables(0).Rows(0).Item(3)
            End If

            Consulta_Datos_Sistema_Imagen = "YES"
        Catch ex As Exception
            Consulta_Datos_Sistema_Imagen = "Inconsistencia general funcion Consulta_Datos_Sistema_Imagen " & ex.Message
        End Try
    End Function
    Function Adjunta_documento_parte_documento_automatico(ByRef pag As Page,
                                                          ByVal ruta_documento As String,
                                                          ByVal id_documento_seleccionado As Integer) As String
        Try
            '----------------------------------------------------
            'Funcion : Adjunta documento al documento seleccionado
            'Fecha : 2018-03-21
            'Ing :Miguel Angel Urueta Miranda
            '----------------------------------------------------
            If (File.Exists(ruta_documento)) Then
                Dim refclas As New ClassNeodynamic
                Dim Matri_Doc() As String
                Erase Matri_Doc
                Dim clasñade As New ClassAñadirDocumento
                Dim Result As String = ""
                Dim clasvis As New ClassRaEnvioCorrespondencia
                Dim content As Object = Nothing
                Result = refclas.Extraer_Documento_de_Multitif_fisico(ruta_documento,
                                                                      Matri_Doc,
                                                                      HttpContext.Current.Session.Item("WF_RUTA_TEMPO_FINAL") & "\")
                If Result <> "YES" Then
                    Adjunta_documento_parte_documento_automatico = "Imposible extraer documento Multi tif "
                    Exit Function
                End If
                If Matri_Doc Is Nothing Then
                    Adjunta_documento_parte_documento_automatico = "La matriz de multi tif esta nothing "
                    Exit Function
                End If
                Dim document As String = Matri_Doc(0)
                Result = clasñade.Añadir_Documentos_tif(id_documento_seleccionado,
                                                        HttpContext.Current.Session.Item("WF_GABINETE_SELECCIONADO"),
                                                        document,
                                                        Matri_Doc)
                If Result <> "YES" Then
                    Adjunta_documento_parte_documento_automatico = "Imposible añadir el documento " & Result
                    Exit Function
                Else
                    Dim Refclas1 As New ClassAñadirDocumento
                    Dim Result1 As String = ""
                    Dim Matrep() As String
                    Erase Matrep
                    Result1 = Refclas1.Actualiza_Interface_Documento_añadido(Matrep,
                                                                             pag)
                    If Result1 <> "YES" Then
                        Adjunta_documento_parte_documento_automatico = Result1
                    Else
                        Adjunta_documento_parte_documento_automatico = "YES"
                    End If
                    Kill(ruta_documento)
                    Exit Function
                End If
            Else
                Adjunta_documento_parte_documento_automatico = "Imposible encontrar el archivo " & ruta_documento
                Exit Function
            End If
        Catch ex As Exception
            Adjunta_documento_parte_documento_automatico = "Inconsistencia general función Adjunta_documento_parte_documento_automatico " & ex.Message
        End Try
    End Function
    Function upload_adjunta_image_parte_documento(ByRef cotntador As String) As String
        Try
            If (File.Exists(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA"))) Then
                Dim refclas As New ClassNeodynamic
                Dim Matri_Doc() As String
                Erase Matri_Doc
                Dim clasñade As New ClassAñadirDocumento
                Dim Result As String = ""
                Dim clasvis As New ClassRaEnvioCorrespondencia
                Dim content As Object = Nothing
                Result = refclas.Extraer_Documento_de_Multitif_fisico(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA"),
                                                                      Matri_Doc,
                                                                      HttpContext.Current.Session.Item("WF_RUTA_TEMPO_FINAL") & "\")
                If Result <> "YES" Then
                    upload_adjunta_image_parte_documento = "Imposible extraer documento Multi tif "
                    Exit Function
                End If
                If Matri_Doc Is Nothing Then
                    upload_adjunta_image_parte_documento = "La matriz de multi tif esta nothing "
                    Exit Function
                End If
                Dim document As String = Matri_Doc(0)
                Result = clasñade.Añadir_Documentos_tif(HttpContext.Current.Session.Item("WF_ID_DOCUMENTO_SELECCIONADO"),
                                                        HttpContext.Current.Session.Item("WF_GABINETE_SELECCIONADO"),
                                                        document,
                                                        Matri_Doc)
                If Result <> "YES" Then
                    upload_adjunta_image_parte_documento = "Imposible añadir el documento " & Result
                    Exit Function
                Else
                    Dim Refclas1 As New ClassAñadirDocumento
                    Dim Result1 As String = ""
                    Dim Matrep() As String
                    Erase Matrep
                    Result1 = Refclas1.upload_inicializa_contador_visor(Matrep,
                                                                        cotntador)
                    If Result1 <> "YES" Then
                        upload_adjunta_image_parte_documento = Result1
                    Else
                        upload_adjunta_image_parte_documento = "YES"
                    End If
                    Kill(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA"))
                    HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA") = ""
                    HttpContext.Current.Session.Item("WF_ERROR_RESPUESTA") = ""
                    Exit Function
                End If
            Else
                upload_adjunta_image_parte_documento = "Imposible encontrar el archivo " & HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA")
                Exit Function
            End If
        Catch ex As Exception
            upload_adjunta_image_parte_documento = "Inconsistencia general funcion upload_adjunta_image_parte_documento " & ex.Message
        End Try
    End Function
    Function Adjunta_documento_parte_documento(ByRef pag As Page, Optional ByRef num_pag As Integer = 0) As String
        '----------------------------------------------------
        'Funcion : Adjunta documento al documento seleccionado
        'Fecha : 2017-01-27
        'ing :Miguel Angel Urueta Miranda
        '----------------------------------------------------

        Try

            If (File.Exists(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA"))) Then
                Dim refclas As New ClassNeodynamic
                Dim Matri_Doc() As String
                Erase Matri_Doc
                Dim clasñade As New ClassAñadirDocumento
                Dim Result As String = ""
                Dim clasvis As New ClassRaEnvioCorrespondencia
                Dim content As Object = Nothing
                Result = refclas.Extraer_Documento_de_Multitif_fisico(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA"),
                                                                      Matri_Doc,
                                                                      HttpContext.Current.Session.Item("WF_RUTA_TEMPO_FINAL") & "\")
                If Result <> "YES" Then
                    Adjunta_documento_parte_documento = "Imposible extraer documento Multi tif "
                    Exit Function
                End If
                If Matri_Doc Is Nothing Then
                    Adjunta_documento_parte_documento = "La matriz de multi tif esta nothing "
                    Exit Function
                End If
                Dim document As String = Matri_Doc(0)
                Result = clasñade.Añadir_Documentos_tif(HttpContext.Current.Session.Item("WF_ID_DOCUMENTO_SELECCIONADO"),
                                                        HttpContext.Current.Session.Item("WF_GABINETE_SELECCIONADO"),
                                                        document,
                                                        Matri_Doc)
                If Result <> "YES" Then
                    Adjunta_documento_parte_documento = "Imposible añadir el documento " & Result
                    Exit Function
                Else
                    Dim Refclas1 As New ClassAñadirDocumento
                    Dim Result1 As String = ""
                    Dim Matrep() As String
                    Erase Matrep
                    Result1 = Refclas1.Actualiza_Interface_Documento_añadido(Matrep, pag)
                    If Result1 <> "YES" Then
                        Adjunta_documento_parte_documento = Result1
                    Else
                        Adjunta_documento_parte_documento = "YES"
                    End If
                    num_pag = Matrep.Length - 1
                    Kill(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA"))
                    HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA") = ""
                    HttpContext.Current.Session.Item("WF_ERROR_RESPUESTA") = ""
                    Exit Function
                End If
            Else
                Adjunta_documento_parte_documento = "Imposible encontrar el archivo " & HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA")
                Exit Function
            End If
        Catch ex As Exception
            Adjunta_documento_parte_documento = "Inconsistencia general función Adjunta_documento_parte_documento " & ex.Message
        End Try
    End Function
    Function Retorna_matriz_documentos_adjuntos_workflow(ByRef Matri_Doc() As String) As String
        Try
            Erase Matri_Doc
            If HttpContext.Current.Session.Item("WF_ERROR_RESPUESTA") <> "" Then
                Retorna_matriz_documentos_adjuntos_workflow = HttpContext.Current.Session.Item("WF_ERROR_RESPUESTA")
                Exit Function
            End If
            If (File.Exists(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA"))) Then
                Dim refclas As New ClassNeodynamic
                Dim clasñade As New ClassAñadirDocumento
                Dim Result As String = ""
                Dim clasvis As New ClassRaEnvioCorrespondencia
                Dim content As Object = Nothing
                Dim fil_info As New FileInfo(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA"))
                If UCase(fil_info.Extension) = ".TIF" Then

                    Result = refclas.Extraer_Documento_de_Multitif_fisico(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA"),
                                                                          Matri_Doc,
                                                                          HttpContext.Current.Session.Item("WF_RUTA_TEMPO_FINAL") & "\")
                    If Result <> "YES" Then
                        Retorna_matriz_documentos_adjuntos_workflow = "Imposible extraer documento Multif " & Left(Result, 20)
                        Exit Function
                    End If
                    If Matri_Doc Is Nothing Then
                        Retorna_matriz_documentos_adjuntos_workflow = "La matriz de multi tif esta nothing "
                        Exit Function
                    End If
                    Retorna_matriz_documentos_adjuntos_workflow = "YES"
                    Exit Function
                Else
                    ReDim Preserve Matri_Doc(0)
                    Matri_Doc(0) = HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA")
                    Retorna_matriz_documentos_adjuntos_workflow = "YES"
                    Exit Function
                End If
            Else
                Retorna_matriz_documentos_adjuntos_workflow = "Imposible encontrar el archivo " & HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA")
                Exit Function
            End If
        Catch ex As Exception
            Retorna_matriz_documentos_adjuntos_workflow = "Inconsistencia general función Retorna_matriz_documentos_adjuntos_workflow " & ex.Message
        End Try
    End Function
    Function Adjunta_documento_relacionado_automatico(ByRef pag As Page,
                                                      ByRef id_imagen As Integer,
                                                      ByVal ruta_documento As String,
                                                      ByVal id_tipo_lista_chequeo As Integer,
                                                      ByVal option_agrega_item_listview As Integer,
                                                      ByVal id_tarea_wf As Long,
                                                      ByVal radicado As String,
                                                      ByRef stru_datos_image_lista As stru_datos_image_lista) As String
        Try
            If (File.Exists(ruta_documento)) Then
                Dim refclas As New ClassNeodynamic
                Dim Matri_Doc() As String
                Erase Matri_Doc
                Dim clasñade As New ClassAñadirDocumento
                Dim Result As String = ""
                Dim clasvis As New ClassRaEnvioCorrespondencia
                Dim content As Object = Nothing
                Dim file As New FileInfo(ruta_documento)
                If UCase(file.Extension) = ".TIF" Then
                    Result = refclas.Extraer_Documento_de_Multitif_fisico(ruta_documento,
                                                                          Matri_Doc,
                                                                          HttpContext.Current.Session.Item("WF_RUTA_TEMPO_FINAL") & "\")
                    If Result <> "YES" Then
                        Adjunta_documento_relacionado_automatico = "Imposible extraer documento Multi tif "
                        Exit Function
                    End If
                    If Matri_Doc Is Nothing Then
                        Adjunta_documento_relacionado_automatico = "La matriz de multi tif esta nothing "
                        Exit Function
                    End If
                Else
                    ReDim Preserve Matri_Doc(0)
                    Matri_Doc(0) = ruta_documento
                End If
                If id_tipo_lista_chequeo = -1 Or id_tipo_lista_chequeo = 0 Then
                    Result = Me.Guardar_Documento_adjunto_relacionado_con_parametro_imagen_previa(HttpContext.Current.Session.Item("WF_GABINETE_SELECCIONADO"),
                                                                                                  HttpContext.Current.Session.Item("WF_ID_DOCUMENTO_SELECCIONADO"),
                                                                                                  Matri_Doc,
                                                                                                  id_imagen,
                                                                                                  1,
                                                                                                  id_tarea_wf,
                                                                                                  radicado,
                                                                                                  stru_datos_image_lista)
                    If Result <> "YES" Then
                        Adjunta_documento_relacionado_automatico = "Imposible guardar el documento relacionado " & Result
                        Exit Function
                    End If
                Else

                    Result = Me.Guardar_Documento_adjunto_relacionado_tipificado(HttpContext.Current.Session.Item("WF_GABINETE_SELECCIONADO"),
                                                                                 HttpContext.Current.Session.Item("WF_ID_DOCUMENTO_SELECCIONADO"),
                                                                                 Matri_Doc,
                                                                                 id_tarea_wf,
                                                                                 radicado,
                                                                                 id_imagen,
                                                                                 id_tipo_lista_chequeo,
                                                                                 stru_datos_image_lista)
                    If Result <> "YES" Then
                        Adjunta_documento_relacionado_automatico = "Imposible guardar el documento relacionado tipificado " & Result
                        Exit Function
                    End If
                End If
                If option_agrega_item_listview = 1 Then
                    Dim structure_datos_tarea_workflow As structure_datos_tarea_workflow = Nothing
                    Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
                    Result = ref_Class_DAT_ADIC_TAR.SolicitaDatosEstructuraBasicaTareaWorkflow(HttpContext.Current.Session("WF_RUTAWORKFLOW"),
                                                                                                    HttpContext.Current.Session("ID_TAREA_SELECCIONDA"),
                                                                                                    structure_datos_tarea_workflow)
                    If Result <> "YES" Then
                        Adjunta_documento_relacionado_automatico = Result
                        Exit Function
                    End If
                    Dim Refclas_trd As New ClassTrdDocumental
                    Dim option_aplica_trd As Integer = 0
                    Dim ref_Class_system1 As New Class_system1
                    Result = ref_Class_system1.VerificaOpcionAplicarTablaRetencion(option_aplica_trd,
                                                                                       stru_datos_image_lista.nombre_gabinete)
                    If Result <> "YES" Then
                        Adjunta_documento_relacionado_automatico = "#31 SELECCIONA-WF Imposible encontrar opción aplicar trd gabinete (" + stru_datos_image_lista.nombre_gabinete + ")"
                        Exit Function
                    End If
                    Dim ref_Class_configuracion_gabinete As New Class_configuracion_gabinete
                    Dim structure_gabinete_workflow As structure_gabinete_workflow = Nothing
                    Result = ref_Class_configuracion_gabinete.SolicitaDatosEstructuraGabineteWorkflow(structure_datos_tarea_workflow.ID_GABINETE,
                                                                                                          structure_gabinete_workflow)
                    If Result <> "YES" Then
                        Adjunta_documento_relacionado_automatico = "#28 SELECCIONA-WF " & Result
                        Exit Function
                    End If
                    Dim Valor_Ceros_Imagen As String = "DIG"
                    Dim Valor_Ceros_Carpeta_Imagen As String = ""
                    Dim Valor_Disco_Imagen As String = ""
                    Dim Ruta_Imagen As String = ""
                    '----------------------------------------------
                    'Obteniendo la identidad de la imagen 
                    '----------------------------------------------
                    Result = Obtener_Ceros_Imagen(stru_datos_image_lista.id_imagen.ToString,
                                                  Valor_Ceros_Imagen)
                    If Result <> "YES" Then
                        Adjunta_documento_relacionado_automatico = "#38 SELECCIONA-WF Error En la funcion Obtener ceros para la imagen=" + structure_datos_tarea_workflow.ID_IMAGEN + Result
                        Exit Function
                    End If
                    Valor_Ceros_Imagen = Valor_Ceros_Imagen & stru_datos_image_lista.extension
                    Dim ref_ClassDaGabinete As New ClassDaGabinete
                    Dim stru_paramter_image As stru_paramter_image = Nothing
                    Result = ref_ClassDaGabinete.SolicitaEtructuraImagenGabinete(structure_gabinete_workflow.NOMBRE_GABINETE,
                                                                                 stru_datos_image_lista.id_imagen,
                                                                                 stru_paramter_image,
                                                                                 option_aplica_trd)
                    If Result <> "YES" Then
                        Adjunta_documento_relacionado_automatico = Result
                        Exit Function
                    End If
                    '--------------------------------------------------
                    'obtener la identidad de la carpeta
                    '--------------------------------------------------
                    Result = Obtener_Ceros_Carpeta_Imagen(stru_paramter_image.IDEX,
                                                          Valor_Ceros_Carpeta_Imagen)
                    If Result <> "YES" Then
                        Adjunta_documento_relacionado_automatico = "Error en la funcion obtener ceros de la carpeta =" + stru_paramter_image.DISC + Result
                        Exit Function
                    End If
                    Dim Cod_Visor As String = ""
                    Dim Extension As String = ""
                    Dim Estado_Documento As String = ""
                    Dim Refclasvis As New Classactualizacionvisor
                    Dim ref_Class_da_extension As New Class_da_extension
                    Result = ref_Class_da_extension.Determina_tipo_documento_list(stru_paramter_image.DBT_TIPO_IMAGEN,
                                                                                  Cod_Visor,
                                                                                  Extension,
                                                                                  Estado_Documento)
                    If Result <> "YES" Then
                        Adjunta_documento_relacionado_automatico = "#36 SELECCIONA-WF Error En la funcion determina_tipo_documento_list=" + Result
                        Exit Function
                    End If
                    '-----------------------------------------
                    'Consulta los id de la imagenes enlazadas
                    'al documento
                    '-----------------------------------------
                    Dim stru_paramter_image_enlace() As stru_paramter_image = Nothing
                    Result = ref_ClassDaGabinete.Solicita_lista_imagenes_enlzadas_gabinete(structure_gabinete_workflow.NOMBRE_GABINETE,
                                                                                           stru_paramter_image.ENLACE,
                                                                                           stru_paramter_image_enlace,
                                                                                           -1,
                                                                                           option_aplica_trd)
                    If Result <> "YES" Then
                        Adjunta_documento_relacionado_automatico = Result
                        Exit Function
                    End If
                    '---------------------------------------------------
                    'Obtener carpeta cntenedora imagen enlace
                    '---------------------------------------------------
                    Valor_Disco_Imagen = structure_gabinete_workflow.NOMBRE_GABINETE & stru_paramter_image.DISC
                    Ruta_Imagen = structure_gabinete_workflow.RUTA_BUSQUEDA_IMAGEN & Valor_Disco_Imagen & "\" & Valor_Ceros_Carpeta_Imagen & "\" & Valor_Ceros_Imagen
                    Dim Treview As TreeView = pag.FindControl("TreeViewseleccion")
                    Dim up_date As UpdatePanel = pag.FindControl("UpdatePanelseleccion")
                    Dim attrNodeGru1 As New TreeNode
                    attrNodeGru1.Value = Ruta_Imagen & "|" & stru_paramter_image.PAG & "|" & stru_paramter_image.ID & "|" & Extension & "|" & Estado_Documento & "|" & stru_datos_image_lista.nombre_gabinete
                    attrNodeGru1.ToolTip = Ruta_Imagen & "|" & stru_paramter_image.PAG & "|" & stru_paramter_image.ID & "|" & Extension & "|" & Estado_Documento & "|" & stru_datos_image_lista.nombre_gabinete
                    attrNodeGru1.PopulateOnDemand = False
                    Dim refclas_seleccion As New Classselecciotarea
                    If Not Treview Is Nothing Then
                        Result = refclas_seleccion.Agrega_icono_image_tre_view(stru_datos_image_lista.extension,
                                                                               attrNodeGru1)
                        If stru_datos_image_lista.tipodocumental = "" Then
                            attrNodeGru1.Text = "Documento(" & stru_paramter_image_enlace.Length & ")"
                        Else
                            attrNodeGru1.Text = stru_datos_image_lista.tipodocumental
                        End If
                        Treview.Nodes.Add(attrNodeGru1)
                    End If
                    If Not up_date Is Nothing Then
                        up_date.Update()
                    End If
                    Dim ref_Label_docu_relacionado_wf As Label = pag.FindControl("Label_docu_relacionado_wf")
                    Dim ref_UpdatePanel_label_seleccion As UpdatePanel = pag.FindControl("UpdatePanel_label_seleccion")
                    ref_Label_docu_relacionado_wf.Text = "Documentos relacionados (" & stru_paramter_image_enlace.Length & ")"
                    ref_UpdatePanel_label_seleccion.Update()
                End If
                If Result <> "YES" Then
                    Adjunta_documento_relacionado_automatico = "Imposible añadir el documento " & Result
                    Exit Function
                Else
                    If file.Exists() Then
                        Kill(ruta_documento)
                    End If
                    Adjunta_documento_relacionado_automatico = "YES"
                    Exit Function
                End If

            Else
                Adjunta_documento_relacionado_automatico = "Imposible encontrar el archivo " & ruta_documento
                Exit Function
            End If
        Catch ex As Exception
            Adjunta_documento_relacionado_automatico = "Inconsistencia general función Adjunta_documento_relacionado_automatico " & ex.Message
        End Try
    End Function

    Function Adjunta_imagen_default(ByVal option_se As Integer,
                                   ByRef pag As Page,
                                   ByVal option_pdf As Integer,
                                   ByVal id_tipo_lista_chequeo As Integer,
                                   ByVal option_agrega_item_listview As Integer,
                                   ByVal id_documento_seleccionado As Integer,
                                   ByRef stru_datos_image_lista As stru_datos_image_lista) As String
        Try
            Dim MatriId() As String
            Erase MatriId
            Dim Id_Activida As String = ""
            Dim Refclas As New ClassListandoTareas
            Dim Result As String = ""
            Dim Ruta_archivo_final As String = ""
            Dim Refclas_Tif As New Class_TiffDLL200
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim radicado As String = ""
            If id_documento_seleccionado = 0 Then
                Adjunta_imagen_default = "Documento no seleccionado por favor habrá el documento"
                Exit Function
            End If
            Dim ref_Class_grupos_workflow As New Class_grupos_workflow
            Result = ref_Class_grupos_workflow.Solicita_id_actividad_usuario_workflow(Id_Activida,
                                                                                      HttpContext.Current.Session.Item("Id_Grupo_Workflow"))
            If Result <> "YES" Then
                Adjunta_imagen_default = Result
                Exit Function
            End If
            If Id_Activida = "" Then
                Adjunta_imagen_default = "Imposible encontrar la id actividad"
                Exit Function
            End If
            Dim ob_iddocumento As Object = id_documento_seleccionado.ToString
            Dim Conection_conectro_C As String = "Persist Security Info=" _
                 & True & ";database=" & HttpContext.Current.Session("DB_NAME_MODULO").ToString _
                 & ";server=" & HttpContext.Current.Session("IP_SERVER_MODULO").ToString _
                & ";user id=" & HttpContext.Current.Session("USER_DBMS_MODULO").ToString _
                & ";pwd=" & HttpContext.Current.Session("PASW_DBMS_MODULO").ToString
            Dim mParamT() As Object = {Conection_conectro_C, HttpContext.Current.Session("Id_Usuario_Workflow").ToString,
                                       HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA").ToString, Id_Activida, ob_iddocumento}
            If HttpContext.Current.Session.Item("ADJUNTOS") <> "" Then
                Dim refcla As New ClassEdtiScript
                Dim ResultadoComp As String = ""
                Dim Resultado4 As String = refcla.Compila_Evalua(ResultadoComp,
                                                                 HttpContext.Current.Session("ADJUNTOS"),
                                                                 "ADJUNTOS",
                                                                 mParamT)
                If Resultado4 <> "YES" Then
                    Adjunta_imagen_default = Resultado4
                    Exit Function
                End If
                If InStr(ResultadoComp, "lPOSITIVOQL_") Then
                    MatriId = ResultadoComp.Split("|")
                    If MatriId Is Nothing Then
                        Adjunta_imagen_default = "Error Ejecutando Función ADJUNTOS " & ResultadoComp
                        Exit Function
                    End If
                Else
                    Adjunta_imagen_default = "El servicio web no devuelve ningun archivo, mensaje relacionado(" & ResultadoComp & ")"
                    Exit Function
                End If
                Result = Refclas_Tif.Sello_camara_tif(MatriId(1),
                                                      Ruta_archivo_final)
                If Result <> "YES" Then
                    Adjunta_imagen_default = Result
                    Exit Function
                End If
                Dim Matri_documentos() As String = Nothing
                ReDim Preserve Matri_documentos(0)
                Matri_documentos(0) = Ruta_archivo_final
                Dim Refclas_reportes As New ClassReportesRadicado
                Dim Ruttempo As String = HttpContext.Current.Server.MapPath(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_WF") + HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString)
                Dim ruta_descarga As String = Ruttempo & "\DONWLOAD\"
                If option_pdf = 1 Then
                    Result = Refclas_reportes.Convertir_tif_pdf_Sello(Matri_documentos,
                                                                      ruta_descarga,
                                                                      "YES",
                                                                      0,
                                                                      "")
                    If Result <> "YES" Then
                        Adjunta_imagen_default = Result
                        Exit Function
                    End If
                    Ruta_archivo_final = ruta_descarga
                End If
                Dim Refclas_almacenamiento As New ClassAlmacenamiento
                If option_se = 1 Then
                    Result = Refclas_almacenamiento.Adjunta_documento_parte_documento_automatico(pag,
                                                                                                 Ruta_archivo_final,
                                                                                                 id_documento_seleccionado)
                    If Result <> "YES" Then
                        Adjunta_imagen_default = Result
                        Exit Function
                    End If
                    If File.Exists(Matri_documentos(0)) Then
                        Kill(Matri_documentos(0))
                    End If
                End If
                Dim id_imagen As Object = Nothing
                If option_se = 2 Then
                    Result = Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"), radicado)
                    Result = Refclas_almacenamiento.Adjunta_documento_relacionado_automatico(pag,
                                                                                             id_imagen,
                                                                                             Ruta_archivo_final,
                                                                                             id_tipo_lista_chequeo,
                                                                                             option_agrega_item_listview,
                                                                                             HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                             radicado,
                                                                                             stru_datos_image_lista)
                    If Result <> "YES" Then
                        Adjunta_imagen_default = Result
                        Exit Function
                    End If
                End If
                If File.Exists(Matri_documentos(0)) Then
                    Kill(Matri_documentos(0))
                End If
                Adjunta_imagen_default = "YES"
                Exit Function
            Else
                Adjunta_imagen_default = "Evento dinámico sin configurar imposible agregar archivo default"
                Exit Function
            End If
        Catch ex As Exception
            Adjunta_imagen_default = "Inconsistencia general función Adjunta_imagen_default " & ex.Message
        End Try
    End Function
    Function Guarda_documento_digitalizado_relacionado_tipificado(ByVal nombre_gabinete As String,
                                                                  ByVal id_documento_origen As Integer,
                                                                  ByVal matri_documentos() As String,
                                                                  ByRef id_imagen As Integer,
                                                                  ByVal id_tipo_documental_lista As Integer,
                                                                  ByVal id_tarea_wf As Long,
                                                                  ByVal radicado As String,
                                                                  ByRef datos_image As stru_datos_image_lista) As String
        Try
            Dim id_tipo_documento As Integer = 0
            Dim id_area As Integer = 0
            Dim id_serie As Integer = 0
            Dim id_sub_serie As Integer = 0
            Dim descripcion_tipo_documento As String = ""
            Dim nombre_area As String = ""
            Dim nombre_serie As String = ""
            Dim nombre_sub_serie As String = ""
            Dim nombre_tipo_documento As String = ""
            Dim Result As String = ""
            Dim class_workflow_digitalizacion As New ClassWorkflowDigitalizacion
            Dim reflcas_tipo_documento As New ClassGaTipoDocumental
            Dim id_clase_documento As Integer = 0
            Dim stru As stru_tipo_lista_chequeo = Nothing
            Dim Ref_Class_ra_dig_tipos_docum_lista_chequeo As New ra_dig_tipos_docum_lista_chequeo
            Result = Ref_Class_ra_dig_tipos_docum_lista_chequeo.SolicitaDatosTipoDocumentalListaChequeo(id_tipo_documental_lista,
                                                                                                             stru)
            If Result <> "YES" Then
                Guarda_documento_digitalizado_relacionado_tipificado = Result
                Exit Function
            End If
            If stru.subseries_documentales_Id_SubSeries <> 0 Then
                id_tipo_documento = stru.tipos_doc_subseries_Id_Tipos_Doc_SubSerie
            Else
                id_tipo_documento = stru.tipo_doc_series_Id_Tipo_Doc_Series
            End If
            '-----------------------------------------------
            'Retorna serie y sub serie tipo documento
            '-----------------------------------------------
            Dim stru_tipo As stru_tipo_documental = Nothing
            Dim ref_clas_trd As New ClassTrdDocumental
            Result = ref_clas_trd.Solicita_datos_estructura_tipo_documento(id_tipo_documento, stru_tipo)
            If Result <> "YES" Then
                Guarda_documento_digitalizado_relacionado_tipificado = Result
                Exit Function
            End If
            id_serie = stru_tipo.Series_Documentales_Id_Series
            id_sub_serie = stru_tipo.sub_serie_id_serie
            Dim ref_Class_series_documentales As New Class_series_documentales
            Result = ref_Class_series_documentales.Retorna_id_area_serie_documental(id_serie,
                                                                                    id_area)
            If Result <> "YES" Then
                Guarda_documento_digitalizado_relacionado_tipificado = Result
                Exit Function
            End If
            Dim ref_Class_ra_tipo_doc_series As New Class_ra_tipo_doc_series
            If id_tipo_documento <> 0 Then
                Result = ref_Class_ra_tipo_doc_series.Retorna_nombre_tipo_documental(id_serie,
                                                                                     id_sub_serie,
                                                                                     id_tipo_documento,
                                                                                     descripcion_tipo_documento)
                If Result <> "YES" Then
                    Guarda_documento_digitalizado_relacionado_tipificado = Result
                    Exit Function
                End If
            End If
            If nombre_tipo_documento = "" Then
                Result = reflcas_tipo_documento.SolicitaIdTipoFormatoDocumento("DOCUMENTO DIGITALIZADO",
                                                                               id_clase_documento)
                If Result <> "YES" Then
                    Guarda_documento_digitalizado_relacionado_tipificado = Result
                    Exit Function
                Else
                    nombre_tipo_documento = "DOCUMENTO DIGITALIZADO"
                End If
            End If
            Dim ref_Class_areas_depart_radicacion As New Class_areas_depart_radicacion
            If id_area <> 0 Then
                Result = ref_Class_areas_depart_radicacion.Retorna_nombre_area_por_id_area(id_area,
                                                                                           nombre_area)
                If Result <> "YES" Then
                    Guarda_documento_digitalizado_relacionado_tipificado = Result
                    Exit Function
                End If
            End If

            If id_serie <> 0 Then
                Result = ref_Class_series_documentales.Retorna_nombre_serie_id_serie(id_serie,
                                                                                     nombre_serie)
                If Result <> "YES" Then
                    Guarda_documento_digitalizado_relacionado_tipificado = Result
                    Exit Function
                End If
            End If
            Dim Class_subseries_documentales As New Class_subseries_documentales
            If id_sub_serie <> 0 Then
                Result = Class_subseries_documentales.Retorna_nombre_sub_serie(id_sub_serie,
                                                                                nombre_sub_serie)
                If Result <> "YES" Then
                    Guarda_documento_digitalizado_relacionado_tipificado = Result
                    Exit Function
                End If
            End If
            Dim Ref_producion As New ClassGaProducionDocumental
            Dim inventario_documental As Integer = 0
            Dim aplica_trd As Integer = 0
            Dim asigna_unidad As Integer = 0
            Dim Ref_Class_system1 As New Class_system1
            Result = Ref_Class_system1.Verifica_opcion_aplicar_trd_inventario_unidad_documental_gabinete(nombre_gabinete,
                                                                                                        inventario_documental,
                                                                                                        aplica_trd,
                                                                                                        asigna_unidad)
            If Result <> "YES" Then
                Guarda_documento_digitalizado_relacionado_tipificado = Result
                Exit Function
            End If
            If aplica_trd <> 1 Then
                Guarda_documento_digitalizado_relacionado_tipificado = "Debe configurar la aplicación de tablas de retención en el gabinete (" & nombre_gabinete & ")"
                Exit Function
            End If
            'Dim radicado As String = ""
            Dim matri_datos_almacen() As String = Nothing
            Dim matri_gestion As estructure_gestion = Nothing
            Result = Me.Retorna_parametros_almacenamiento_documento_relacionado(id_documento_origen,
                                                                                matri_datos_almacen,
                                                                                matri_gestion,
                                                                                nombre_gabinete)
            If Result <> "YES" Then
                Guarda_documento_digitalizado_relacionado_tipificado = Result
                Exit Function
            End If
            '--------------------------------------------------------------
            'Retorna datos del a estructura del gabinete para remplazar 
            'el campo tipo documento
            '--------------------------------------------------------------
            Dim estructura_gabinete() As estructura_gabinete = Nothing
            Dim index_campo As Integer = -1
            Result = Me.Retorna_estructura_campos_gabinete_visible(nombre_gabinete,
                                                                   estructura_gabinete)
            If Result <> "YES" Then
                Guarda_documento_digitalizado_relacionado_tipificado = Result
                Exit Function
            End If
            For i As Integer = 0 To estructura_gabinete.Length - 1
                If estructura_gabinete(i).CAMPO = "TIPODOCUMENTO" Then
                    index_campo = i
                    Exit For
                End If
            Next
            If index_campo <> -1 Then
                matri_datos_almacen(index_campo) = descripcion_tipo_documento
            End If
            Dim fecha_elaboracion As String = ""
            Dim Refclasalmacena As New ClassAlmacenamiento
            Dim date1al As String = Date.Today
            Result = ""
            Dim ref_ClassGestionFechas As New ClassGestionFechas
            Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
            If Result <> "YES" Then
                Guarda_documento_digitalizado_relacionado_tipificado = "Error formateando fecha almacenamiento Funcion: Guarda_documento_digitalizado_relacionado_tipificado " & Result
                Exit Function
            Else
                fecha_elaboracion = date1al
            End If
            matri_gestion.CLASE_DOCUMENTO = nombre_tipo_documento
            matri_gestion.ID_AREA = id_area
            matri_gestion.ID_CLASE_DOCUMENTO = id_clase_documento
            matri_gestion.ID_SERIE = id_serie
            matri_gestion.ID_SUB_SERIE = id_sub_serie
            matri_gestion.ID_TIPO_UNIDAD_CONSERVACION = 0
            matri_gestion.ID_TIPODOCUMENTO = id_tipo_documento
            matri_gestion.ID_UNIDAD_CONSERVACION = 0
            matri_gestion.ID_USUARIO_GESTION = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
            matri_gestion.TIPO_UNIDAD_DOCUMENTAL = 0
            matri_gestion.UNIDAD_CONSERVACION = ""
            matri_gestion.FECHA_ELABORACION = fecha_elaboracion
            matri_gestion.TIPODOCUMENTO = descripcion_tipo_documento
            matri_gestion.NOMBRE_SERIE = nombre_serie
            matri_gestion.NOMBRE_SUB_SERIE = nombre_sub_serie
            Dim Tipo_Doc_int As Integer = -1
            Dim Filein As New FileInfo(matri_documentos(0))
            Dim Refalmacena As New ClassAlmacenamiento
            Dim Class_da_extension As New Class_da_extension
            Result = Class_da_extension.SolicitaTipoArchivoDocuarchiExtension(UCase(Filein.Extension),
                                                                          Tipo_Doc_int)
            If Result <> "YES" Then
                Guarda_documento_digitalizado_relacionado_tipificado = "Imposible determinar el tipo de documento " & Result
                Exit Function
            End If
            id_imagen = Tipo_Doc_int
            Dim estado_firma_digital As Integer = 0
            Result = Refalmacena.Almacenamiento("", "", nombre_gabinete, 0, matri_datos_almacen,
             2, matri_documentos.Length, Tipo_Doc_int, matri_documentos, 0, id_imagen, Tipo_Doc_int,
             HttpContext.Current.Session.Item("GA_IDEMPRESA"), HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
             matri_gestion.ID_AREA, matri_gestion.ID_SERIE, matri_gestion.ID_SUB_SERIE,
            matri_gestion.ID_TIPODOCUMENTO, matri_gestion.ID_EXPEDIENTE, matri_gestion.ID_TIPO_EXPEDIENTE,
            matri_gestion.ID_UNIDAD_CONSERVACION, matri_gestion.ID_TIPO_UNIDAD_CONSERVACION,
            matri_gestion.ID_CLASE_DOCUMENTO, matri_gestion.EXPEDIENTE, matri_gestion.NOMBRE_SERIE,
            matri_gestion.NOMBRE_SUB_SERIE, matri_gestion.TIPODOCUMENTO, matri_gestion.UNIDAD_CONSERVACION,
            matri_gestion.CLASE_DOCUMENTO, matri_gestion.FECHA_ELABORACION, radicado, "", 0, 0, 0, id_tarea_wf,
            HttpContext.Current.Session.Item("Id_Ruta_Workflow"))
            If Result <> "YES" Then
                Guarda_documento_digitalizado_relacionado_tipificado = "Guarda_documento_digitalizado_relacionado_tipificado  dice " & Result
                Exit Function
            Else
                Dim icono As String = ""
                Dim classdagabinete As New ClassDaGabinete
                classdagabinete.SolicitaIconoImageFownt(Tipo_Doc_int,
                                        icono)
                Dim val_documento As String = matri_gestion.TIPODOCUMENTO
                If val_documento = "" Then
                    val_documento = "Documento"
                End If
                datos_image.nombre_gabinete = nombre_gabinete
                datos_image.id_imagen = id_imagen
                datos_image.radicado = radicado
                datos_image.tipodocumental = descripcion_tipo_documento
                datos_image.notipodocumento = val_documento
                datos_image.extension = UCase(Filein.Extension)
                datos_image.estado_firma_digital = estado_firma_digital
                datos_image.icono_icono_awe_some = icono
                For i As Integer = 0 To matri_documentos.Length - 1
                    Kill(matri_documentos(i))
                Next
                Guarda_documento_digitalizado_relacionado_tipificado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Guarda_documento_digitalizado_relacionado_tipificado = "Inconsistencia general función Guarda_documento_digitalizado_relacionado_tipificado " & ex.Message
        End Try
    End Function
    Function UploadSaveFileScan(ByVal CasoDgitalizacion As String,
                                ByRef StruDatosIImageLista As stru_datos_image_lista) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Activa guardar documentos desde vis escaneo
        '          
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'CasoDgitalizacion        : Representa el caso de digitalización
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'StruDatosImageLista      : Retorna la estructura para la interfaz de lista
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                    : 2025-07-23
        'Elabora                  : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Selection As String = HttpContext.Current.Session.Item("SELECCIONTEMPORAL")
            Dim SplSpliter() As String = Selection.Split("|")
            Dim ClassWorkflowDigitalizacion As New ClassWorkflowDigitalizacion
            StruDatosIImageLista = Nothing
            Dim IdImagenAlmacenada As Integer = 0
            Dim IdTareaWorkflow As Long = 0
            Dim RutaArchivoDigitalizado As String = ""
            Dim NombreClaseDocumento As String = "DOCUMENTO DIGITALIZADO"
            Select Case CasoDgitalizacion
                '///---------Adjunta documento digitalizado desde el enlace workflow-------///
                Case "TRAMITE"
                    Dim IdTipoTramite As Integer = HttpContext.Current.Session.Item("DG_ID_TRAMITE")
                    If SplSpliter(3) = "ENLASE" Then
                        IdTareaWorkflow = HttpContext.Current.Session("ID_TAREA_SELECCIONDA_ENLACE")
                    Else
                        IdTareaWorkflow = HttpContext.Current.Session("ID_TAREA_SELECCIONDA")
                    End If
                    If IdTareaWorkflow = 0 Then
                        If Result <> "YES" Then
                            UploadSaveFileScan = "El sistema no pudo identificar el siguiete estado de digitalzación (" & SplSpliter(3) & ")"
                            Exit Function
                        End If
                    End If
                    Result = ClassWorkflowDigitalizacion.SolicitaRutaDocumentoDigitalizado(Val(SplSpliter(0)),
                                                                                           HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ESCANER"),
                                                                                           RutaArchivoDigitalizado)
                    If Result <> "YES" Then
                        UploadSaveFileScan = Result
                        Exit Function
                    End If

                    Result = Me.PreAlmacenaDocumentoTareaWorkflow(HttpContext.Current.Session.Item("DG_RIPO_DOCUMENTAL_LISTA_CHEQUEO"),
                                                                  HttpContext.Current.Session.Item("DG_LISTA_CHEQUEO"),
                                                                  RutaArchivoDigitalizado,
                                                                  IdTareaWorkflow,
                                                                  IdTipoTramite,
                                                                  NombreClaseDocumento,
                                                                  IdImagenAlmacenada,
                                                                  StruDatosIImageLista)
                    If Result <> "YES" Then
                        UploadSaveFileScan = Result
                        Exit Function
                    End If
                    '////-----Adjunta documento digitializado desde la interfaz de documento seleccionado------///
                Case "TRAMITE_ADJUNTOWORKFLOW"
                    Dim IdTipoTramite As Integer = HttpContext.Current.Session.Item("DG_ID_TRAMITE")
                    If SplSpliter(3) = "ENLASE" Then
                        IdTareaWorkflow = HttpContext.Current.Session("ID_TAREA_SELECCIONDA_ENLACE")
                    Else
                        IdTareaWorkflow = HttpContext.Current.Session("ID_TAREA_SELECCIONDA")
                    End If
                    If IdTareaWorkflow = 0 Then
                        If Result <> "YES" Then
                            UploadSaveFileScan = "El sistema no pudo identificar el siguiete estado de digitalzación (" & SplSpliter(3) & ")"
                            Exit Function
                        End If
                    End If
                    Result = ClassWorkflowDigitalizacion.SolicitaRutaDocumentoDigitalizado(Val(SplSpliter(0)),
                                                                                           HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ESCANER"),
                                                                                           RutaArchivoDigitalizado)
                    If Result <> "YES" Then
                        UploadSaveFileScan = Result
                        Exit Function
                    End If
                    Result = Me.PreAlmacenaDocumentoTareaWorkflow(HttpContext.Current.Session.Item("DG_RIPO_DOCUMENTAL_LISTA_CHEQUEO"),
                                                                  HttpContext.Current.Session.Item("DG_LISTA_CHEQUEO"),
                                                                  RutaArchivoDigitalizado,
                                                                  IdTareaWorkflow,
                                                                  IdTipoTramite,
                                                                  NombreClaseDocumento,
                                                                  IdImagenAlmacenada,
                                                                  StruDatosIImageLista)
                    If Result <> "YES" Then
                        UploadSaveFileScan = Result
                        Exit Function
                    End If
                Case Else
                    UploadSaveFileScan = "El sistema no ha identificado el tipo de digitalización (" & CasoDgitalizacion & ") no se encuentra previamente registrado o configurado en los catálogos o parámetros del sistema. "
                    Exit Function
            End Select
            UploadSaveFileScan = "YES"
            Exit Function
        Catch ex As Exception
            UploadSaveFileScan = "Inconsistencia general funcion  UploadSaveFileScan " & ex.Message
        End Try
    End Function
    Function UploadSaveFile(ByVal IdExpediente As Integer,
                            ByVal IdTipoChek As Integer,
                            ByVal DescripcionTipoDocumento As String,
                            ByVal EstadoChekAdjuntoAnexo As Integer,
                            ByVal EstadoChekRelacionado As Integer,
                            ByVal NumeroDocRelacionado As Integer,
                            ByVal FechaCarga As String,
                            ByRef StruDatosImageLista As stru_datos_image_lista,
                            ByRef IdTareaWorkflow As Long,
                            ByRef Contador As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Activa guardar documentos que se carga desde dispositivos por la interfaz de carga
        '          de java script
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdExpediente             : Representa la identificación de un expediente
        'IdTipoChek               : Representa la identificación de la lista chek list
        'DescripcionTipoDocumento : Representa la descripcion de la tipologia documental
        'EstadoChekAdjuntoAnexo   : Representa el estado del chequeo de documento adjunto
        'EstadoChekRelacionado    : Representa el estado del chequeo de documento relacionado
        'NumeroDocRelacionado     : Represental el numero de documento relacionado
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'StruDatosImageLista      : Retorna la estructura para la interfaz de lista
        'IdTareaWorkflow          : Retorna la tarea workflow
        'Contador                 : Retorna el contador de imagenes para docuentos adjuntos desde el visor
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-07-23
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim ClassAlmacenamiento As New ClassAlmacenamiento
            Dim ob_page As Object = Nothing
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim radicado As String = ""
            Dim NombreClaseDocumento As String = "DOCUMENTO ELECTRONICO"
            If HttpContext.Current.Session("WF_TIPO_ADJUNTA") = "ESCANER" Then
                If IdTipoChek = 0 Then
                    HttpContext.Current.Session("DG_LISTA_CHEQUEO") = "-1"
                Else
                    HttpContext.Current.Session("DG_LISTA_CHEQUEO") = IdTipoChek
                End If
                Dim ID_DOCUMENTO As Integer = 0
                Dim TIPO_DOCUMENTO As Integer = 0
                Dim datos_image As String = ""
                Dim ref_ClassDaGabinete As New ClassDaGabinete
                Dim ref_ClassAlmacenamiento As New ClassAlmacenamiento
                Dim ob As Object = Nothing
                Result = ref_ClassAlmacenamiento.Almacenamiento_Documentos_Digitalizados("",
                                                                                        ID_DOCUMENTO,
                                                                                        TIPO_DOCUMENTO,
                                                                                        ob,
                                                                                        StruDatosImageLista,
                                                                                        HttpContext.Current.Session("DG_TIPODIGITALIZACION"),
                                                                                        1)
                If Result <> "YES" Then
                    UploadSaveFile = Result
                    Exit Function
                End If
            End If
            If HttpContext.Current.Session.Item("WF_TIPO_ADJUNTA") = "ENLACE" Then
                If IdTipoChek = 0 Or IdTipoChek = -1 Then
                    HttpContext.Current.Session.Item("DG_LISTA_CHEQUEO") = "-1"
                Else
                    HttpContext.Current.Session.Item("DG_LISTA_CHEQUEO") = IdTipoChek
                End If
                Dim IdImagenAlmacenada As Integer = 0
                Dim IdTipoTramite As Integer = HttpContext.Current.Session.Item("DG_ID_TRAMITE")
                Result = ClassAlmacenamiento.PreAlmacenaDocumentoTareaWorkflow(DescripcionTipoDocumento,
                                                                               HttpContext.Current.Session.Item("DG_LISTA_CHEQUEO"),
                                                                               HttpContext.Current.Session("WF_RUTA_TEMPO_ADJUNTA"),
                                                                               HttpContext.Current.Session("ID_TAREA_SELECCIONDA_ENLACE"),
                                                                               IdTipoTramite,
                                                                               NombreClaseDocumento,
                                                                               IdImagenAlmacenada,
                                                                               StruDatosImageLista)
                If Result <> "YES" Then
                    HttpContext.Current.Session.Item("DG_LISTA_CHEQUEO") = -1
                    UploadSaveFile = Result
                    Exit Function
                Else
                    HttpContext.Current.Session.Item("DG_LISTA_CHEQUEO") = -1
                    If StruDatosImageLista.notipodocumento = "" Or StruDatosImageLista.notipodocumento = "Documento" Then
                        Dim nun_doc As Object = Val(NumeroDocRelacionado) + 1
                        StruDatosImageLista.notipodocumento = "D-" & StruDatosImageLista.id_imagen
                    End If
                    IdTareaWorkflow = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA_ENLACE")
                    UploadSaveFile = "YES"
                    Exit Function
                End If
            End If
            '----------------------------------------------------
            'Adjunta doocumento modulo workflow tarea asgiganda
            '----------------------------------------------------
            If HttpContext.Current.Session("WF_TIPO_ADJUNTA") = "LISTA" Then
                If IdTipoChek = 0 Or IdTipoChek = -1 Then
                    HttpContext.Current.Session.Item("DG_LISTA_CHEQUEO") = "-1"
                Else
                    HttpContext.Current.Session.Item("DG_LISTA_CHEQUEO") = IdTipoChek
                End If
                Dim id_imagen As Integer = 0
                Dim IdTipoTramite As Integer = HttpContext.Current.Session.Item("DG_ID_TRAMITE")
                Result = PreAlmacenaDocumentoTareaWorkflow(DescripcionTipoDocumento,
                                                           HttpContext.Current.Session.Item("DG_LISTA_CHEQUEO"),
                                                           HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA"),
                                                           HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                           IdTipoTramite,
                                                           NombreClaseDocumento,
                                                           id_imagen,
                                                           StruDatosImageLista)
                If Result <> "YES" Then
                    UploadSaveFile = Result
                    Exit Function
                Else
                    If StruDatosImageLista.notipodocumento = "" Or StruDatosImageLista.notipodocumento = "Documento" Then
                        StruDatosImageLista.notipodocumento = "D-" & StruDatosImageLista.id_imagen
                    End If
                    IdTareaWorkflow = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")
                    UploadSaveFile = "YES"
                    Exit Function
                End If
            End If
            If HttpContext.Current.Session.Item("WF_TIPO_ADJUNTA") = "VISOR" Then
                If EstadoChekAdjuntoAnexo = 1 Then
                    Result = ClassAlmacenamiento.upload_adjunta_image_parte_documento(Contador)
                    If Result <> "YES" Then
                        UploadSaveFile = Result
                        Exit Function
                    Else
                        IdTareaWorkflow = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")
                        UploadSaveFile = "YES"
                        Exit Function
                    End If
                End If
                If EstadoChekRelacionado = 1 Then
                    Result = Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"), radicado)
                    Dim id_imagen As Integer = 0
                    If IdTipoChek = -1 Or IdTipoChek = 0 Then
                        Result = ClassAlmacenamiento.Adjunta_donumento_relacionado(ob_page,
                                                                       id_imagen,
                                                                       HttpContext.Current.Session.Item("WF_GABINETE_SELECCIONADO"),
                                                                       HttpContext.Current.Session.Item("WF_ID_DOCUMENTO_SELECCIONADO"),
                                                                       IdTipoChek,
                                                                       HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                       radicado,
                                                                       StruDatosImageLista,
                                                                       0)
                        If Result <> "YES" Then
                            UploadSaveFile = Result
                            Exit Function
                        Else
                            If StruDatosImageLista.notipodocumento = "" Or StruDatosImageLista.notipodocumento = "Documento" Then
                                StruDatosImageLista.notipodocumento = "D-" & StruDatosImageLista.id_imagen
                            End If
                            IdTareaWorkflow = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")
                            UploadSaveFile = "YES"
                            Exit Function
                        End If

                    Else
                        Result = ClassAlmacenamiento.Adjunta_donumento_relacionado(ob_page,
                                                                       id_imagen,
                                                                       HttpContext.Current.Session.Item("WF_GABINETE_SELECCIONADO"),
                                                                       HttpContext.Current.Session.Item("WF_ID_DOCUMENTO_SELECCIONADO"),
                                                                       IdTipoChek,
                                                                       HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                       radicado,
                                                                       StruDatosImageLista,
                                                                       0)
                        If Result <> "YES" Then
                            HttpContext.Current.Session.Item("DG_LISTA_CHEQUEO") = -1
                            UploadSaveFile = Result
                            Exit Function
                        Else
                            HttpContext.Current.Session.Item("DG_LISTA_CHEQUEO") = -1
                            If StruDatosImageLista.notipodocumento = "" Then
                                Dim nun_doc As Object = Val(NumeroDocRelacionado) + 1
                                StruDatosImageLista.notipodocumento = "D-" & StruDatosImageLista.id_imagen
                            End If
                            IdTareaWorkflow = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")
                            UploadSaveFile = "YES"
                            Exit Function
                        End If

                    End If
                End If
            End If
            If HttpContext.Current.Session.Item("WF_TIPO_ADJUNTA") = "CONSULTA_RADICADO" Then
                Dim Class_plantillas_radicacion As New Class_plantillas_radicacion
                Dim plantilla_validacion_campos_estaticos As PLANTILLA_VALIDACION_CAMPOS_ESTATICOS = Nothing
                Result = Class_plantillas_radicacion.retorna_datos_radicacion_estructura(HttpContext.Current.Session.Item("RA_TIPO_PLANTILLA_CONSULTA"),
                                                                                         HttpContext.Current.Session.Item("RA_RADICADO_CONSULTA"),
                                                                                         HttpContext.Current.Session.Item("RA_PLANTILLA_CONSULTA"),
                                                                                         plantilla_validacion_campos_estaticos)
                If Result <> "YES" Then
                    UploadSaveFile = Result
                    Exit Function
                End If
                Dim id_gabiente As Integer = 0
                Dim nombre_gabinete As String = ""
                Dim Class_tipo_doc_entrante As New Class_tipo_doc_entrante
                Result = Class_tipo_doc_entrante.Retorna_id_nombre_gabinete_tipo_tramite(plantilla_validacion_campos_estaticos.System_Plantilla_Radicado_id_Plantilla,
                                                                                         plantilla_validacion_campos_estaticos.Descripcion_Documento,
                                                                                         id_gabiente,
                                                                                         nombre_gabinete)
                If Result <> "YES" Then
                    UploadSaveFile = Result
                    Exit Function
                End If
                Dim id_tarea_workflow As Long = 0
                Dim id_ruta As Integer = 0
                Dim nombre_ruta As String = ""
                Dim nombre_campo_radicado_ruta As String = ""
                Dim Class_worflow_rutas As New Class_worflow_rutas
                Dim Class_configuracion_listado_ruta As New Class_configuracion_listado_ruta
                If plantilla_validacion_campos_estaticos.Flag_Flow = 1 Then
                    Result = Class_worflow_rutas.Retorna_nombre_ruta_workflow(nombre_ruta)
                    If Result <> "YES" Then
                        UploadSaveFile = Result
                        Exit Function
                    End If
                    Result = Class_worflow_rutas.Retorna_id_ruta_workflow(nombre_ruta,
                                                                          id_ruta)
                    If Result <> "YES" Then
                        UploadSaveFile = Result
                        Exit Function
                    End If
                    Result = Class_configuracion_listado_ruta.SolicitaNombreCampoRadicadoRuta(id_ruta,
                                                                                              nombre_campo_radicado_ruta)
                    If Result <> "YES" Then
                        UploadSaveFile = Result
                        Exit Function
                    End If
                    Result = Class_DAT_ADIC_TAR.Solicita_id_tarea_radicado(HttpContext.Current.Session.Item("RA_RADICADO_CONSULTA"),
                                                                           nombre_ruta,
                                                                           nombre_campo_radicado_ruta,
                                                                           id_tarea_workflow,
                                                                           1)
                    If Result <> "YES" Then
                        UploadSaveFile = Result
                        Exit Function
                    End If

                End If
                If IdTipoChek = 0 Or IdTipoChek = -1 Then
                    HttpContext.Current.Session.Item("DG_LISTA_CHEQUEO") = "-1"
                Else
                    HttpContext.Current.Session.Item("DG_LISTA_CHEQUEO") = IdTipoChek
                End If
                Dim evalua_flujo_ruta As Integer = 0
                If id_tarea_workflow <> 0 Then
                    evalua_flujo_ruta = 1
                End If
                Dim Refclas_dagabinete As New ClassDaGabinete
                Dim ID_DOCUMENTO As Integer = 0
                Dim TIPO_DOCUMENTO As Integer = 0
                Result = Refclas_dagabinete.Almacenamiento_documentos_adjuntos_digitalizados_modulo_radicado("",
                                                                                                             ID_DOCUMENTO,
                                                                                                             TIPO_DOCUMENTO,
                                                                                                             StruDatosImageLista,
                                                                                                             IdTipoChek,
                                                                                                             id_tarea_workflow,
                                                                                                             nombre_gabinete,
                                                                                                             HttpContext.Current.Session.Item("RA_RADICADO_CONSULTA"),
                                                                                                             evalua_flujo_ruta,
                                                                                                             2, 2)
                If Result <> "YES" Then
                    UploadSaveFile = Result
                    Exit Function
                Else
                    If StruDatosImageLista.notipodocumento = "" Or StruDatosImageLista.notipodocumento = "Documento" Then
                        Dim nun_doc As Object = Val(NumeroDocRelacionado) + 1
                        StruDatosImageLista.notipodocumento = "D-" & StruDatosImageLista.id_imagen
                    End If
                    UploadSaveFile = "YES"
                    Exit Function
                End If
            End If
            If HttpContext.Current.Session.Item("WF_TIPO_ADJUNTA") = "ENLACE_RADICADO" Then
                Dim Class_ra_rad_estados_modulo_radicacion As New Class_ra_rad_estados_modulo_radicacion
                Dim stru_ As stru_registro_estado = Nothing
                Result = Class_ra_rad_estados_modulo_radicacion.SolicitaDatosEstructuraEstadoRadicado(HttpContext.Current.Session.Item("RA_ID_REGISTRO_RADICADO"),
                                                                                                          stru_)

                If Result <> "YES" Then
                    UploadSaveFile = Result
                    Exit Function
                End If
                If IdTipoChek = 0 Or IdTipoChek = -1 Then
                    HttpContext.Current.Session.Item("DG_LISTA_CHEQUEO") = "-1"
                Else
                    HttpContext.Current.Session.Item("DG_LISTA_CHEQUEO") = IdTipoChek
                End If
                Dim evalua_flujo_ruta As Integer = 0
                If stru_.id_tarea_workflow <> 0 Then
                    evalua_flujo_ruta = 1
                End If
                Dim Refclas_dagabinete As New ClassDaGabinete
                Dim ID_DOCUMENTO As Integer = 0
                Dim TIPO_DOCUMENTO As Integer = 0
                Result = Refclas_dagabinete.Almacenamiento_documentos_adjuntos_digitalizados_modulo_radicado("",
                                                                                                             ID_DOCUMENTO,
                                                                                                             TIPO_DOCUMENTO,
                                                                                                             StruDatosImageLista,
                                                                                                             IdTipoChek,
                                                                                                             stru_.id_tarea_workflow,
                                                                                                             HttpContext.Current.Session.Item("DG_NOMBRE_GABINETE"),
                                                                                                             stru_.consecutivo_radicado,
                                                                                                             evalua_flujo_ruta,
                                                                                                             2,
                                                                                                             2)
                If Result <> "YES" Then
                    UploadSaveFile = Result
                    Exit Function
                Else
                    If StruDatosImageLista.notipodocumento = "" Or StruDatosImageLista.notipodocumento = "Documento" Then
                        Dim nun_doc As Object = Val(NumeroDocRelacionado) + 1
                        StruDatosImageLista.notipodocumento = "D-" & StruDatosImageLista.id_imagen
                    End If
                    UploadSaveFile = "YES"
                    Exit Function
                End If
            End If
            If HttpContext.Current.Session.Item("WF_TIPO_ADJUNTA") = "ADJUNTARADICACION" Then
                Dim Class_ra_rad_estados_modulo_radicacion As New Class_ra_rad_estados_modulo_radicacion
                Dim StruRegistroEstado As stru_registro_estado = Nothing
                Result = Class_ra_rad_estados_modulo_radicacion.SolicitaDatosEstructuraEstadoRadicado(HttpContext.Current.Session.Item("RA_ID_REGISTRO_RADICADO"),
                                                                                                      StruRegistroEstado)

                If Result <> "YES" Then
                    Return Result
                End If
                If IdTipoChek = 0 Or IdTipoChek = -1 Then
                    HttpContext.Current.Session.Item("DG_LISTA_CHEQUEO") = "-1"
                Else
                    HttpContext.Current.Session.Item("DG_LISTA_CHEQUEO") = IdTipoChek
                End If
                Dim EvaluaActualizaImagenWorkflow As Integer = 0
                If StruRegistroEstado.id_tarea_workflow <> 0 Then
                    EvaluaActualizaImagenWorkflow = 1
                End If
                Dim IdImagenAlmacenada As Integer = 0
                Result = PreAlmacenaDocumentosRadicacion(DescripcionTipoDocumento,
                                                         IdTipoChek,
                                                         "",
                                                         StruRegistroEstado.id_tarea_workflow,
                                                         StruRegistroEstado.tipo_doc_entrante_id_Tipo_Doc_Entrante,
                                                         NombreClaseDocumento,
                                                         EvaluaActualizaImagenWorkflow,
                                                         2,
                                                         0,
                                                         "",
                                                         IdImagenAlmacenada,
                                                         StruDatosImageLista)
                If Result <> "YES" Then
                    UploadSaveFile = Result
                    Exit Function
                Else
                    If StruDatosImageLista.notipodocumento = "" Or StruDatosImageLista.notipodocumento = "Documento" Then
                        Dim nun_doc As Object = Val(NumeroDocRelacionado) + 1
                        StruDatosImageLista.notipodocumento = "D-" & StruDatosImageLista.id_imagen
                    End If
                    UploadSaveFile = "YES"
                    Exit Function
                End If
            End If
            If HttpContext.Current.Session.Item("WF_TIPO_ADJUNTA") = "PRODUCCION" Then
                Result = ClassAlmacenamiento.PreAlmacenaDocumentoProduccion(IdExpediente,
                                                                            HttpContext.Current.Session("WF_RUTA_TEMPO_ADJUNTA"),
                                                                            DescripcionTipoDocumento,
                                                                            IdTipoChek,
                                                                            NombreClaseDocumento,
                                                                            0,
                                                                            FechaCarga,
                                                                            0,
                                                                            StruDatosImageLista)
                If Result <> "YES" Then
                    UploadSaveFile = Result
                    Exit Function
                Else
                    If StruDatosImageLista.notipodocumento = "" Or StruDatosImageLista.notipodocumento = "Documento" Then
                        Dim nun_doc As Object = Val(NumeroDocRelacionado) + 1
                        StruDatosImageLista.notipodocumento = "D-" & StruDatosImageLista.id_imagen
                    End If
                    UploadSaveFile = "YES"
                    Exit Function
                End If
            End If
            UploadSaveFile = "No hay seleccion, funcion upload_save_file_gestion_respuesta"
        Catch ex As Exception
            UploadSaveFile = "Inconsistencia general funcion upload_save_file_gestion_respuesta " & ex.Message
        End Try
    End Function
    Function Solicita_datos_estructura_tipo_documento_lista_chequeo(ByVal id_tipo_documental_lista As Integer,
                                                                    ByVal descripcion_clase_documento As String,
                                                                    ByRef matri_gestion As estructure_gestion) As String
        Try
            Dim id_tipo_documento As Integer = 0
            Dim id_area As Integer = 0
            Dim id_serie As Integer = 0
            Dim id_sub_serie As Integer = 0
            Dim descripcion_tipo_documento As String = ""
            Dim nombre_area As String = ""
            Dim nombre_serie As String = ""
            Dim nombre_sub_serie As String = ""
            Dim nombre_tipo_documento As String = ""
            Dim stru As stru_tipo_lista_chequeo = Nothing
            Dim class_workflow_digitalizacion As New ClassWorkflowDigitalizacion
            Dim reflcas_tipo_documento As New ClassGaTipoDocumental
            Dim id_clase_documento As Integer = 0
            Dim Result As String = ""
            Dim Ref_Class_ra_dig_tipos_docum_lista_chequeo As New ra_dig_tipos_docum_lista_chequeo
            Result = Ref_Class_ra_dig_tipos_docum_lista_chequeo.SolicitaDatosTipoDocumentalListaChequeo(id_tipo_documental_lista,
                                                                                                             stru)
            If Result <> "YES" Then
                Solicita_datos_estructura_tipo_documento_lista_chequeo = Result
                Exit Function
            End If
            If stru.subseries_documentales_Id_SubSeries <> 0 Then
                id_tipo_documento = stru.tipos_doc_subseries_Id_Tipos_Doc_SubSerie
            Else
                id_tipo_documento = stru.tipo_doc_series_Id_Tipo_Doc_Series
            End If
            '-----------------------------------------------
            'Retorna serie y sub serie tipo documento
            '-----------------------------------------------
            Dim stru_tipo As stru_tipo_documental = Nothing
            Dim ref_clas_trd As New ClassTrdDocumental
            Result = ref_clas_trd.Solicita_datos_estructura_tipo_documento(id_tipo_documento,
                                                                           stru_tipo)
            If Result <> "YES" Then
                Solicita_datos_estructura_tipo_documento_lista_chequeo = Result
                Exit Function
            End If
            id_serie = stru_tipo.Series_Documentales_Id_Series
            id_sub_serie = stru_tipo.sub_serie_id_serie
            Dim ref_Class_series_documentales As New Class_series_documentales
            Result = ref_Class_series_documentales.Retorna_id_area_serie_documental(id_serie,
                                                                                    id_area)
            If Result <> "YES" Then
                Solicita_datos_estructura_tipo_documento_lista_chequeo = Result
                Exit Function
            End If
            Dim ref_Class_ra_tipo_doc_series As New Class_ra_tipo_doc_series
            If id_tipo_documento <> 0 Then
                Result = ref_Class_ra_tipo_doc_series.Retorna_nombre_tipo_documental(id_serie,
                                                                                     id_sub_serie,
                                                                                     id_tipo_documento,
                                                                                     descripcion_tipo_documento)
                If Result <> "YES" Then
                    Solicita_datos_estructura_tipo_documento_lista_chequeo = Result
                    Exit Function
                End If
            End If
            If nombre_tipo_documento = "" Then
                Result = reflcas_tipo_documento.SolicitaIdTipoFormatoDocumento(descripcion_clase_documento,
                                                                               id_clase_documento)
                If Result <> "YES" Then
                    Solicita_datos_estructura_tipo_documento_lista_chequeo = Result
                    Exit Function
                Else
                    nombre_tipo_documento = descripcion_clase_documento
                End If
            End If
            Dim ref_Class_areas_depart_radicacion As New Class_areas_depart_radicacion
            If id_area <> 0 Then
                Result = ref_Class_areas_depart_radicacion.Retorna_nombre_area_por_id_area(id_area,
                                                                                       nombre_area)
                If Result <> "YES" Then
                    Solicita_datos_estructura_tipo_documento_lista_chequeo = Result
                    Exit Function
                End If
            End If

            If id_serie <> 0 Then
                Result = ref_Class_series_documentales.Retorna_nombre_serie_id_serie(id_serie,
                                                                                     nombre_serie)
                If Result <> "YES" Then
                    Solicita_datos_estructura_tipo_documento_lista_chequeo = Result
                    Exit Function
                End If
            End If
            Dim Class_subseries_documentales As New Class_subseries_documentales
            If id_sub_serie <> 0 Then
                Result = Class_subseries_documentales.Retorna_nombre_sub_serie(id_sub_serie,
                                                                                nombre_sub_serie)
                If Result <> "YES" Then
                    Solicita_datos_estructura_tipo_documento_lista_chequeo = Result
                    Exit Function
                End If
            End If
            Dim fecha_elaboracion As String = ""
            Dim ref_ClassGestionFechas As New ClassGestionFechas
            Dim date1al As String = Date.Today
            Result = ""
            Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
            If Result <> "YES" Then
                Solicita_datos_estructura_tipo_documento_lista_chequeo = "Error formateando fecha almacenamiento Funcion: Solicita_datos_estructura_tipo_documento_lista_chequeo " & Result
                Exit Function
            Else
                fecha_elaboracion = date1al
            End If
            matri_gestion.CLASE_DOCUMENTO = nombre_tipo_documento
            matri_gestion.ID_AREA = id_area
            matri_gestion.ID_CLASE_DOCUMENTO = id_clase_documento
            matri_gestion.ID_SERIE = id_serie
            matri_gestion.ID_SUB_SERIE = id_sub_serie
            matri_gestion.ID_TIPO_UNIDAD_CONSERVACION = 0
            matri_gestion.ID_TIPODOCUMENTO = id_tipo_documento
            matri_gestion.ID_UNIDAD_CONSERVACION = 0
            matri_gestion.ID_USUARIO_GESTION = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
            matri_gestion.TIPO_UNIDAD_DOCUMENTAL = 0
            matri_gestion.UNIDAD_CONSERVACION = ""
            matri_gestion.FECHA_ELABORACION = fecha_elaboracion
            matri_gestion.TIPODOCUMENTO = descripcion_tipo_documento
            matri_gestion.NOMBRE_SERIE = nombre_serie
            matri_gestion.NOMBRE_SUB_SERIE = nombre_sub_serie
            Solicita_datos_estructura_tipo_documento_lista_chequeo = "YES"
        Catch ex As Exception
            Solicita_datos_estructura_tipo_documento_lista_chequeo = "Inconsistencia general función Solicita_datos_estructura_tipo_documento_lista_chequeo " & ex.Message
        End Try
    End Function
    Function Guardar_Documento_adjunto_relacionado_tipificado(ByVal nombre_gabinete As String,
                                                              ByVal id_documento_origen As Integer,
                                                              ByVal matri_documentos() As String,
                                                              ByVal id_tarea_wf As Long,
                                                              ByVal radicado As String,
                                                              ByRef id_imagen As Integer,
                                                              ByVal id_tipo_documental_lista As Integer,
                                                              ByRef datos_image As stru_datos_image_lista) As String
        Try
            Dim id_tipo_documento As Integer = 0
            Dim id_area As Integer = 0
            Dim id_serie As Integer = 0
            Dim id_sub_serie As Integer = 0
            Dim descripcion_tipo_documento As String = ""
            Dim nombre_area As String = ""
            Dim nombre_serie As String = ""
            Dim nombre_sub_serie As String = ""
            Dim nombre_tipo_documento As String = ""
            Dim Result As String = ""
            Dim class_workflow_digitalizacion As New ClassWorkflowDigitalizacion
            Dim reflcas_tipo_documento As New ClassGaTipoDocumental
            Dim id_clase_documento As Integer = 0
            Dim stru As stru_tipo_lista_chequeo = Nothing
            Dim Ref_Class_ra_dig_tipos_docum_lista_chequeo As New ra_dig_tipos_docum_lista_chequeo
            Result = Ref_Class_ra_dig_tipos_docum_lista_chequeo.SolicitaDatosTipoDocumentalListaChequeo(id_tipo_documental_lista,
                                                                                                             stru)
            If Result <> "YES" Then
                Guardar_Documento_adjunto_relacionado_tipificado = Result
                Exit Function
            End If
            If stru.subseries_documentales_Id_SubSeries <> 0 Then
                id_tipo_documento = stru.tipos_doc_subseries_Id_Tipos_Doc_SubSerie
            Else
                id_tipo_documento = stru.tipo_doc_series_Id_Tipo_Doc_Series
            End If
            '-----------------------------------------------
            'Retorna serie y sub serie tipo documento
            '-----------------------------------------------
            Dim stru_tipo As stru_tipo_documental = Nothing
            Dim ref_clas_trd As New ClassTrdDocumental
            Result = ref_clas_trd.Solicita_datos_estructura_tipo_documento(id_tipo_documento,
                                                                           stru_tipo)
            If Result <> "YES" Then
                Guardar_Documento_adjunto_relacionado_tipificado = Result
                Exit Function
            End If
            id_serie = stru_tipo.Series_Documentales_Id_Series
            id_sub_serie = stru_tipo.sub_serie_id_serie
            Dim ref_Class_series_documentales As New Class_series_documentales
            Result = ref_Class_series_documentales.Retorna_id_area_serie_documental(id_serie,
                                                                                    id_area)
            If Result <> "YES" Then
                Guardar_Documento_adjunto_relacionado_tipificado = Result
                Exit Function
            End If
            Dim ref_Class_ra_tipo_doc_series As New Class_ra_tipo_doc_series
            If id_tipo_documento <> 0 Then
                Result = ref_Class_ra_tipo_doc_series.Retorna_nombre_tipo_documental(id_serie,
                                                                                     id_sub_serie,
                                                                                     id_tipo_documento,
                                                                                     descripcion_tipo_documento)
                If Result <> "YES" Then
                    Guardar_Documento_adjunto_relacionado_tipificado = Result
                    Exit Function
                End If
            End If
            If nombre_tipo_documento = "" Then
                Result = reflcas_tipo_documento.SolicitaIdTipoFormatoDocumento("DOCUMENTO DIGITALIZADO",
                                                                               id_clase_documento)
                If Result <> "YES" Then
                    Guardar_Documento_adjunto_relacionado_tipificado = Result
                    Exit Function
                Else
                    nombre_tipo_documento = "DOCUMENTO DIGITALIZADO"
                End If
            End If
            Dim ref_Class_areas_depart_radicacion As New Class_areas_depart_radicacion
            If id_area <> 0 Then
                Result = ref_Class_areas_depart_radicacion.Retorna_nombre_area_por_id_area(id_area,
                                                                                       nombre_area)
                If Result <> "YES" Then
                    Guardar_Documento_adjunto_relacionado_tipificado = Result
                    Exit Function
                End If
            End If

            If id_serie <> 0 Then
                Result = ref_Class_series_documentales.Retorna_nombre_serie_id_serie(id_serie,
                                                                                     nombre_serie)
                If Result <> "YES" Then
                    Guardar_Documento_adjunto_relacionado_tipificado = Result
                    Exit Function
                End If
            End If
            Dim Class_subseries_documentales As New Class_subseries_documentales
            If id_sub_serie <> 0 Then
                Result = Class_subseries_documentales.Retorna_nombre_sub_serie(id_sub_serie,
                                                                                nombre_sub_serie)
                If Result <> "YES" Then
                    Guardar_Documento_adjunto_relacionado_tipificado = Result
                    Exit Function
                End If
            End If
            Dim Ref_producion As New ClassGaProducionDocumental
            Dim inventario_documental As Integer = 0
            Dim aplica_trd As Integer = 0
            Dim asigna_unidad As Integer = 0
            Dim Ref_Class_system1 As New Class_system1
            Result = Ref_Class_system1.Verifica_opcion_aplicar_trd_inventario_unidad_documental_gabinete(nombre_gabinete,
                                                                                                        inventario_documental,
                                                                                                        aplica_trd,
                                                                                                        asigna_unidad)
            If Result <> "YES" Then
                Guardar_Documento_adjunto_relacionado_tipificado = Result
                Exit Function
            End If
            If aplica_trd <> 1 Then
                Guardar_Documento_adjunto_relacionado_tipificado = "Debe configurar la aplicación de tablas de retención en el gabinete (" & nombre_gabinete & ")"
                Exit Function
            End If
            Dim radicado_ As String = ""
            Dim matri_datos_almacen() As String = Nothing
            Dim matri_gestion As estructure_gestion = Nothing
            Result = Me.Retorna_parametros_almacenamiento_documento_relacionado(id_documento_origen,
                                                                                matri_datos_almacen,
                                                                                matri_gestion,
                                                                                nombre_gabinete)
            If Result <> "YES" Then
                Guardar_Documento_adjunto_relacionado_tipificado = Result
                Exit Function
            End If
            '--------------------------------------------------------------
            'Retorna datos del a estructura del gabinete para remplazar 
            'el campo tipo documento
            '--------------------------------------------------------------
            Dim estructura_gabinete() As estructura_gabinete = Nothing
            Dim index_campo As Integer = -1
            Dim index_campo_radicado As Integer = -1
            Dim Valor_radicado As String = "RAD-ANEXO"
            Result = Me.Retorna_estructura_campos_gabinete_visible(nombre_gabinete,
                                                                   estructura_gabinete)
            If Result <> "YES" Then
                Guardar_Documento_adjunto_relacionado_tipificado = Result
                Exit Function
            End If
            For i As Integer = 0 To estructura_gabinete.Length - 1
                If estructura_gabinete(i).CAMPO = "TIPODOCUMENTO" Then
                    index_campo = i
                    'Exit For
                End If
                If estructura_gabinete(i).CAMPO = "ENLASE" Then
                    index_campo_radicado = i
                    'Exit For
                End If
                If estructura_gabinete(i).CAMPO = "NUMERORADICA" Then
                    index_campo_radicado = i
                    'Exit For
                End If

            Next
            If index_campo <> -1 Then
                matri_datos_almacen(index_campo) = descripcion_tipo_documento
            End If
            If index_campo_radicado <> -1 Then
                Valor_radicado = "RAD-" & matri_datos_almacen(index_campo_radicado)
            End If
            Dim fecha_elaboracion As String = ""
            Dim Refclasalmacena As New ClassAlmacenamiento
            Dim ref_ClassGestionFechas As New ClassGestionFechas
            Dim date1al As String = Date.Today
            Result = ""
            Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
            If Result <> "YES" Then
                Guardar_Documento_adjunto_relacionado_tipificado = "Error formateando fecha almacenamiento Funcion: Guardar_Documento_adjunto_relacionado_tipificado " & Result
                Exit Function
            Else
                fecha_elaboracion = date1al
            End If
            matri_gestion.CLASE_DOCUMENTO = nombre_tipo_documento
            matri_gestion.ID_AREA = id_area
            matri_gestion.ID_CLASE_DOCUMENTO = id_clase_documento
            matri_gestion.ID_SERIE = id_serie
            matri_gestion.ID_SUB_SERIE = id_sub_serie
            matri_gestion.ID_TIPO_UNIDAD_CONSERVACION = 0
            matri_gestion.ID_TIPODOCUMENTO = id_tipo_documento
            matri_gestion.ID_UNIDAD_CONSERVACION = 0
            matri_gestion.ID_USUARIO_GESTION = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
            matri_gestion.TIPO_UNIDAD_DOCUMENTAL = 0
            matri_gestion.UNIDAD_CONSERVACION = ""
            matri_gestion.FECHA_ELABORACION = fecha_elaboracion
            matri_gestion.TIPODOCUMENTO = descripcion_tipo_documento
            matri_gestion.NOMBRE_SERIE = nombre_serie
            matri_gestion.NOMBRE_SUB_SERIE = nombre_sub_serie
            Dim Tipo_Doc_int As Integer = -1
            Dim Filein As New FileInfo(matri_documentos(0))
            Dim cl As New Classactualizacionvisor
            Dim Refalmacena As New ClassAlmacenamiento
            Dim Class_da_extension As New Class_da_extension
            Result = Class_da_extension.SolicitaTipoArchivoDocuarchiExtension(UCase(Filein.Extension),
                                                                          Tipo_Doc_int)
            If Result <> "YES" Then
                Guardar_Documento_adjunto_relacionado_tipificado = "Imposible determinar el tipo de documento " & Result
                Exit Function
            End If
            '-------------------------------------------------------
            'Asigna valores del expediente a relacionar el documento
            '-------------------------------------------------------
            Dim ClassGaExpediente As New ClassGaExpediente
            Result = ClassGaExpediente.SolicitaEstructuraExpedienteDocumentoVinculante(matri_gestion,
                                                                                       nombre_gabinete,
                                                                                       id_imagen,
                                                                                       id_tarea_wf,
                                                                                       radicado,
                                                                                       HttpContext.Current.Session("WF_RUTAWORKFLOW"),
                                                                                       HttpContext.Current.Session("Id_Ruta_Workflow"),
                                                                                       "")
            If Result <> "YES" Then
                Guardar_Documento_adjunto_relacionado_tipificado = Result
                Exit Function
            End If
            id_imagen = Tipo_Doc_int
            Dim estado_firma_digital As Integer = 0
            Result = Refalmacena.Almacenamiento("", "", nombre_gabinete, 0, matri_datos_almacen,
            2, matri_documentos.Length, Tipo_Doc_int, matri_documentos, 0, id_imagen, Tipo_Doc_int,
            HttpContext.Current.Session.Item("GA_IDEMPRESA"), HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
            matri_gestion.ID_AREA, matri_gestion.ID_SERIE, matri_gestion.ID_SUB_SERIE,
            matri_gestion.ID_TIPODOCUMENTO, matri_gestion.ID_EXPEDIENTE, matri_gestion.ID_TIPO_EXPEDIENTE,
            matri_gestion.ID_UNIDAD_CONSERVACION, matri_gestion.ID_TIPO_UNIDAD_CONSERVACION,
            matri_gestion.ID_CLASE_DOCUMENTO, matri_gestion.EXPEDIENTE, matri_gestion.NOMBRE_SERIE,
            matri_gestion.NOMBRE_SUB_SERIE, matri_gestion.TIPODOCUMENTO, matri_gestion.UNIDAD_CONSERVACION,
            matri_gestion.CLASE_DOCUMENTO, matri_gestion.FECHA_ELABORACION, radicado_, Valor_radicado, estado_firma_digital, id_tarea_wf,
            HttpContext.Current.Session.Item("Id_Ruta_Workflow"))
            If Result <> "YES" Then
                Guardar_Documento_adjunto_relacionado_tipificado = "Guardar_Documento_adjunto_relacionado_tipificado  dice " & Result
                Exit Function
            Else
                Dim val_documento As String = matri_gestion.TIPODOCUMENTO
                datos_image.nombre_gabinete = nombre_gabinete
                datos_image.id_imagen = id_imagen
                datos_image.radicado = radicado_
                datos_image.tipodocumental = descripcion_tipo_documento
                datos_image.notipodocumento = val_documento
                datos_image.extension = UCase(Filein.Extension)
                datos_image.estado_firma_digital = estado_firma_digital
                For i As Integer = 0 To matri_documentos.Length - 1
                    Kill(matri_documentos(i))
                Next
                Guardar_Documento_adjunto_relacionado_tipificado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Guardar_Documento_adjunto_relacionado_tipificado = "Inconsistencia función Guardar_Documento_adjunto_relacionado_tipificado " & ex.Message
        End Try
    End Function
    Function Guardar_Documento_adjunto_relacionado_con_parametro_imagen_previa(ByVal nombre_gabinete As String,
                                                                               ByVal id_documento_origen As Integer,
                                                                               ByVal matri_documentos() As String,
                                                                               ByRef id_imagen As Integer,
                                                                               ByVal tipo_documental_asignado As Integer,
                                                                               ByVal id_tarea_wf As Long,
                                                                               ByVal radicado As String,
                                                                               ByRef datos_image As stru_datos_image_lista) As String
        Try
            Dim radicado_ As String = ""
            Dim matri_datos_almacen() As String = Nothing
            Dim matri_gestion As estructure_gestion = Nothing
            Dim result As String = ""
            result = Me.Retorna_parametros_almacenamiento_documento_relacionado(id_documento_origen,
                                                                                matri_datos_almacen,
                                                                                matri_gestion,
                                                                                nombre_gabinete)
            If result <> "YES" Then
                Guardar_Documento_adjunto_relacionado_con_parametro_imagen_previa = result
                Exit Function
            End If
            '--------------------------------------------------------------
            'Retorna datos del a estructura del gabinete para remplazar 
            'el campo tipo documento
            '--------------------------------------------------------------
            Dim estructura_gabinete() As estructura_gabinete = Nothing
            Dim index_campo As Integer = -1
            Dim index_campo_radicado As Integer = -1
            Dim idex_campo_tipologia_documental As Integer = -1
            Dim Valor_radicado As String = "RAD-ANEXO"
            result = Me.Retorna_estructura_campos_gabinete_visible(nombre_gabinete,
                                                                   estructura_gabinete)
            If result <> "YES" Then
                Guardar_Documento_adjunto_relacionado_con_parametro_imagen_previa = result
                Exit Function
            End If
            For i As Integer = 0 To estructura_gabinete.Length - 1
                If estructura_gabinete(i).CAMPO = "ENLASE" Then
                    index_campo_radicado = i
                End If
                If estructura_gabinete(i).CAMPO = "NUMERORADICA" Then
                    index_campo_radicado = i
                End If
                If estructura_gabinete(i).CAMPO = "TIPODOCUMENTO" Then
                    idex_campo_tipologia_documental = i
                End If
            Next
            If index_campo_radicado <> -1 Then
                Valor_radicado = "RAD-" & matri_datos_almacen(index_campo_radicado)
            End If
            If tipo_documental_asignado = -1 And idex_campo_tipologia_documental <> -1 Then
                matri_datos_almacen(idex_campo_tipologia_documental) = ""
            End If
            Dim Tipo_Doc_int As Integer = -1
            Dim Filein As New FileInfo(matri_documentos(0))
            Dim cl As New Classactualizacionvisor
            Dim Refalmacena As New ClassAlmacenamiento
            Dim Class_da_extension As New Class_da_extension
            result = Class_da_extension.SolicitaTipoArchivoDocuarchiExtension(UCase(Filein.Extension),
                                                                          Tipo_Doc_int)
            If result <> "YES" Then
                Guardar_Documento_adjunto_relacionado_con_parametro_imagen_previa = "Imposible determinar el tipo de documento " & result
                Exit Function
            End If
            If tipo_documental_asignado = -1 Then
                matri_gestion.TIPODOCUMENTO = ""
                matri_gestion.ID_TIPODOCUMENTO = 0
            End If
            '-------------------------------------------------------
            'Asigna valores del expediente a relacionar el documento
            '-------------------------------------------------------
            Dim ClassGaExpediente As New ClassGaExpediente
            result = ClassGaExpediente.SolicitaEstructuraExpedienteDocumentoVinculante(matri_gestion,
                                                                                       nombre_gabinete,
                                                                                       id_imagen,
                                                                                       id_tarea_wf,
                                                                                       radicado,
                                                                                       HttpContext.Current.Session("WF_RUTAWORKFLOW"),
                                                                                       HttpContext.Current.Session("Id_Ruta_Workflow"),
                                                                                       "")
            If result <> "YES" Then
                Guardar_Documento_adjunto_relacionado_con_parametro_imagen_previa = result
                Exit Function
            End If
            id_imagen = Tipo_Doc_int
            Dim estado_firma_digital As Integer = 0
            result = Refalmacena.Almacenamiento("", "", nombre_gabinete, 0, matri_datos_almacen,
            2, matri_documentos.Length, Tipo_Doc_int, matri_documentos, 0, id_imagen, Tipo_Doc_int,
            HttpContext.Current.Session.Item("GA_IDEMPRESA"), HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
            matri_gestion.ID_AREA, matri_gestion.ID_SERIE, matri_gestion.ID_SUB_SERIE,
            matri_gestion.ID_TIPODOCUMENTO, matri_gestion.ID_EXPEDIENTE, matri_gestion.ID_TIPO_EXPEDIENTE,
            matri_gestion.ID_UNIDAD_CONSERVACION, matri_gestion.ID_TIPO_UNIDAD_CONSERVACION,
            matri_gestion.ID_CLASE_DOCUMENTO, matri_gestion.EXPEDIENTE, matri_gestion.NOMBRE_SERIE,
            matri_gestion.NOMBRE_SUB_SERIE, matri_gestion.TIPODOCUMENTO, matri_gestion.UNIDAD_CONSERVACION,
            matri_gestion.CLASE_DOCUMENTO, matri_gestion.FECHA_ELABORACION, radicado_, Valor_radicado, estado_firma_digital, id_tarea_wf, HttpContext.Current.Session.Item("Id_Ruta_Workflow"))
            If result <> "YES" Then
                Guardar_Documento_adjunto_relacionado_con_parametro_imagen_previa = "Guardar_Documento_adjunto_relacionado  dice " & result
                Exit Function
            Else
                Dim icono As String = ""
                Dim classdagabinete As New ClassDaGabinete
                classdagabinete.SolicitaIconoImageFownt(Tipo_Doc_int,
                                                         icono)
                Dim val_documento As String = matri_gestion.TIPODOCUMENTO
                datos_image.nombre_gabinete = nombre_gabinete
                datos_image.id_imagen = id_imagen
                datos_image.radicado = radicado
                datos_image.tipodocumental = val_documento
                datos_image.notipodocumento = val_documento
                datos_image.extension = UCase(Filein.Extension)
                datos_image.estado_firma_digital = estado_firma_digital
                datos_image.icono_icono_awe_some = icono
                For i As Integer = 0 To matri_documentos.Length - 1
                    Kill(matri_documentos(i))
                Next
                Guardar_Documento_adjunto_relacionado_con_parametro_imagen_previa = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Guardar_Documento_adjunto_relacionado_con_parametro_imagen_previa = "Inconsistencia función Guardar_Documento_adjunto_relacionado_con_parametro_imagen_previa " & ex.Message
        End Try
    End Function
    Function Retorna_parametros_almacenamiento_documento_relacionado(ByVal id_imagen_seleccionada As Integer,
                                                                     ByRef matri_datos_almacen() As String,
                                                                     ByRef matri_gestion As estructure_gestion,
                                                                     ByRef nombre_gabinete As String) As String
        Try
            Dim Result As String = ""
            Dim Refeclasaladir As New ClassAñadirDocumento
            Erase matri_datos_almacen
            Result = Refeclasaladir.Obtiene_Valores_Campos_Documentos_a_Duplicar(matri_datos_almacen,
                                                                                 nombre_gabinete,
                                                                                 id_imagen_seleccionada)
            If Result <> "YES" Then
                Retorna_parametros_almacenamiento_documento_relacionado = " Funcion Retorna_parametros_almacenamiento_documento_relacionado dice Imposible buscar campos del documento " & Result
                Exit Function
            End If
            '--------------------------------------------------------------
            'Retorna datos del a estructura del gabinete respuesta
            '--------------------------------------------------------------
            Dim estructura_gabinete() As estructura_gabinete = Nothing
            Dim Class_DETALLE_GABIENETE As New Class_DETALLE_GABIENETE
            Result = Class_DETALLE_GABIENETE.SolicitaEstructuraCamposGabinete(nombre_gabinete,
                                                                                 estructura_gabinete)
            If Result <> "YES" Then
                Retorna_parametros_almacenamiento_documento_relacionado = Result
                Exit Function
            End If
            For i As Integer = 0 To estructura_gabinete.Length - 1
                estructura_gabinete(i).VALORCAMPO = ""
            Next
            '-------------------------------------------
            'Asigna datos gestion
            '-------------------------------------------
            matri_gestion.CLASE_DOCUMENTO = ""
            matri_gestion.EXPEDIENTE = ""
            matri_gestion.ID_AREA = 0
            matri_gestion.ID_CLASE_DOCUMENTO = 0
            matri_gestion.ID_EXPEDIENTE = 0
            matri_gestion.ID_SERIE = 0
            matri_gestion.ID_SUB_SERIE = 0
            matri_gestion.ID_TIPO_EXPEDIENTE = 0
            matri_gestion.ID_TIPO_UNIDAD_CONSERVACION = 0
            matri_gestion.ID_TIPODOCUMENTO = 0
            matri_gestion.ID_UNIDAD_CONSERVACION = 0
            matri_gestion.ID_USUARIO_GESTION = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
            matri_gestion.TIPO_UNIDAD_DOCUMENTAL = 0
            matri_gestion.UNIDAD_CONSERVACION = ""
            matri_gestion.FECHA_ELABORACION = ""
            Dim reflclasalalma As New ClassAlmacenamiento
            Dim ClassDaGabinete As New ClassDaGabinete
            If id_imagen_seleccionada <> 0 Then
                Result = ClassDaGabinete.Solicita_datos_expediente_relacion_gabinete(id_imagen_seleccionada,
                                                                                     nombre_gabinete,
                                                                                     matri_gestion)
                If Result <> "YES" Then
                    Retorna_parametros_almacenamiento_documento_relacionado = Result
                    Exit Function
                End If
                Result = reflclasalalma.Solicita_datos_gestion_estructura_base_datos(matri_gestion,
                                                                                     nombre_gabinete,
                                                                                     id_imagen_seleccionada)
                If Result <> "YES" Then
                    Retorna_parametros_almacenamiento_documento_relacionado = Result
                    Exit Function
                End If
                Result = reflclasalalma.Solicita_datos_tipo_documental_estructura_base_datos(matri_gestion,
                                                                                             nombre_gabinete,
                                                                                             id_imagen_seleccionada)
                If Result <> "YES" Then
                    Retorna_parametros_almacenamiento_documento_relacionado = Result
                    Exit Function
                End If
                Result = reflclasalalma.Solicita_datos_unidad_conservacion_estructura_base_datos(matri_gestion,
                                                                                                 nombre_gabinete,
                                                                                                 id_imagen_seleccionada)
                If Result <> "YES" Then
                    Retorna_parametros_almacenamiento_documento_relacionado = Result
                    Exit Function
                End If

            End If
            '------------------------------------------
            'Retorna el id tipo documento
            '------------------------------------------
            Dim clase_documento As String = "DOCUMENTO ELECTRONICO"
            Dim id_clase_documento As Integer = 0
            Dim Class_ra_tipo_documento As New Class_ra_tipo_documento
            Result = Class_ra_tipo_documento.Solicita_id_clase_documento(clase_documento,
                                                                         id_clase_documento)
            If Result <> "YES" Then
                Retorna_parametros_almacenamiento_documento_relacionado = Result
                Exit Function
            End If
            matri_gestion.CLASE_DOCUMENTO = clase_documento
            matri_gestion.ID_CLASE_DOCUMENTO = id_clase_documento
            Dim date1al As String = Date.Today
            Dim ref_ClassGestionFechas As New ClassGestionFechas
            Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
            If Result <> "YES" Then
                Retorna_parametros_almacenamiento_documento_relacionado = "Error formateando fecha almacenamiento Funcion: Formatea_Fecha_Almacenamiento " & Result
                Exit Function
            End If
            matri_gestion.FECHA_ELABORACION = date1al
            '---------------------------------------------
            'Asigna los datos de gestion a la estructura
            '---------------------------------------------
            For i As Integer = 0 To estructura_gabinete.Length - 1
                If estructura_gabinete(i).CAMPO = "FECHAELABORACION" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.FECHA_ELABORACION
                End If
                If estructura_gabinete(i).CAMPO = "ID_AREA" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_AREA
                End If
                If estructura_gabinete(i).CAMPO = "ID_SERIE" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_SERIE
                End If
                If estructura_gabinete(i).CAMPO = "ID_SUB_SERIE" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_SUB_SERIE
                End If
                If estructura_gabinete(i).CAMPO = "ID_TIPODOCUMENTO" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_TIPODOCUMENTO
                End If
                If estructura_gabinete(i).CAMPO = "ID_USUARIO_GESTION" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_USUARIO_GESTION
                End If
                If estructura_gabinete(i).CAMPO = "NOMBRESERIE" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.NOMBRE_SERIE
                End If
                If estructura_gabinete(i).CAMPO = "NOMBRESUBSERIE" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.NOMBRE_SUB_SERIE
                End If
                If estructura_gabinete(i).CAMPO = "ID_EXPEDIENTE" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_EXPEDIENTE
                End If
                If estructura_gabinete(i).CAMPO = "ID_TIPO_EXPEDIENTE" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_EXPEDIENTE
                End If
                If estructura_gabinete(i).CAMPO = "ID_UNIDAD_CONSERVACION" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_UNIDAD_CONSERVACION
                End If
                If estructura_gabinete(i).CAMPO = "ID_TIPO_UNIDAD_CONSERVACION" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_UNIDAD_CONSERVACION
                End If
                If estructura_gabinete(i).CAMPO = "ID_TIPO_UNIDAD_DOCUMENTAL" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_UNIDAD_CONSERVACION
                End If
                If estructura_gabinete(i).CAMPO = "ID_CLASE_DOCUMENTO" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_CLASE_DOCUMENTO
                End If
                If estructura_gabinete(i).CAMPO = "EXPEDIENTE" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.EXPEDIENTE
                End If
                If estructura_gabinete(i).CAMPO = "UNIDADCONSERVA" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.UNIDAD_CONSERVACION
                End If
                If estructura_gabinete(i).CAMPO = "CLASEDOCUMENTO" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.CLASE_DOCUMENTO
                End If
            Next
            Retorna_parametros_almacenamiento_documento_relacionado = "YES"
        Catch ex As Exception
            Retorna_parametros_almacenamiento_documento_relacionado = "Inconsistencia general función Retorna_parametros_almacenamiento_documento_permanente_almacenado " & ex.Message
        End Try
    End Function

    Function Retorna_parametros_almacenamiento_documento_relacionado_digitalizado(
                                                                                  ByVal id_imagen_seleccionada As Integer,
                                                                                  ByRef matri_datos_almacen() As String,
                                                                                  ByRef matri_gestion As estructure_gestion,
                                                                                  ByRef nombre_gabinete As String) As String
        Try
            Dim Result As String = ""
            Dim Refeclasaladir As New ClassAñadirDocumento
            'Dim MatriDatosAlmacen() As String
            Erase matri_datos_almacen
            Result = Refeclasaladir.Obtiene_Valores_Campos_Documentos_a_Duplicar(matri_datos_almacen,
                                                                                 nombre_gabinete,
                                                                                 id_imagen_seleccionada)
            If Result <> "YES" Then
                Retorna_parametros_almacenamiento_documento_relacionado_digitalizado = "Funcion Retorna_parametros_almacenamiento_documento_relacionado_digitalizado dice Imposible buscar campos del documento " & Result
                Exit Function
            End If
            '--------------------------------------------------------------
            'Retorna datos del a estructura del gabinete respuesta
            '--------------------------------------------------------------
            Dim Refclasalmacenamiento As New ClassAlmacenamiento
            Dim estructura_gabinete() As estructura_gabinete = Nothing
            Dim Class_DETALLE_GABIENETE As New Class_DETALLE_GABIENETE
            Result = Class_DETALLE_GABIENETE.SolicitaEstructuraCamposGabinete(nombre_gabinete,
                                                                                 estructura_gabinete)
            If Result <> "YES" Then
                Retorna_parametros_almacenamiento_documento_relacionado_digitalizado = Result
                Exit Function
            End If
            For i As Integer = 0 To estructura_gabinete.Length - 1
                estructura_gabinete(i).VALORCAMPO = ""
            Next
            '-------------------------------------------
            'Asigna datos gestion
            '-------------------------------------------
            matri_gestion.CLASE_DOCUMENTO = ""
            matri_gestion.EXPEDIENTE = ""
            matri_gestion.ID_AREA = 0
            matri_gestion.ID_CLASE_DOCUMENTO = 0
            matri_gestion.ID_EXPEDIENTE = 0
            matri_gestion.ID_SERIE = 0
            matri_gestion.ID_SUB_SERIE = 0
            matri_gestion.ID_TIPO_EXPEDIENTE = 0
            matri_gestion.ID_TIPO_UNIDAD_CONSERVACION = 0
            matri_gestion.ID_TIPODOCUMENTO = 0
            matri_gestion.ID_UNIDAD_CONSERVACION = 0
            matri_gestion.ID_USUARIO_GESTION = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
            matri_gestion.TIPO_UNIDAD_DOCUMENTAL = 0
            matri_gestion.UNIDAD_CONSERVACION = ""
            matri_gestion.FECHA_ELABORACION = ""
            Dim reflclasalalma As New ClassAlmacenamiento
            Dim ClassDaGabinete As New ClassDaGabinete
            If id_imagen_seleccionada <> 0 Then
                Result = ClassDaGabinete.Solicita_datos_expediente_relacion_gabinete(id_imagen_seleccionada,
                                                                                     nombre_gabinete,
                                                                                     matri_gestion)
                If Result <> "YES" Then
                    Retorna_parametros_almacenamiento_documento_relacionado_digitalizado = Result
                    Exit Function
                End If
                Result = reflclasalalma.Solicita_datos_gestion_estructura_base_datos(matri_gestion,
                                                                                     nombre_gabinete,
                                                                                     id_imagen_seleccionada)
                If Result <> "YES" Then
                    Retorna_parametros_almacenamiento_documento_relacionado_digitalizado = Result
                    Exit Function
                End If
                Result = reflclasalalma.Solicita_datos_tipo_documental_estructura_base_datos(matri_gestion,
                                                                                             nombre_gabinete,
                                                                                             id_imagen_seleccionada)
                If Result <> "YES" Then
                    Retorna_parametros_almacenamiento_documento_relacionado_digitalizado = Result
                    Exit Function
                End If
                Result = reflclasalalma.Solicita_datos_unidad_conservacion_estructura_base_datos(matri_gestion,
                                                                                                 nombre_gabinete,
                                                                                                 id_imagen_seleccionada)
                If Result <> "YES" Then
                    Retorna_parametros_almacenamiento_documento_relacionado_digitalizado = Result
                    Exit Function
                End If
            End If
            '------------------------------------------
            'Retorna el id tipo documento
            '------------------------------------------
            Dim clase_documento As String = "DOCUMENTO ELECTRONICO"
            Dim id_clase_documento As Integer = 0
            Dim Class_ra_tipo_documento As New Class_ra_tipo_documento
            Result = Class_ra_tipo_documento.Solicita_id_clase_documento(clase_documento,
                                                                         id_clase_documento)
            If Result <> "YES" Then
                Retorna_parametros_almacenamiento_documento_relacionado_digitalizado = Result
                Exit Function
            End If
            matri_gestion.CLASE_DOCUMENTO = clase_documento
            matri_gestion.ID_CLASE_DOCUMENTO = id_clase_documento
            Dim date1al As String = Date.Today
            Dim ref_ClassGestionFechas As New ClassGestionFechas
            Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
            If Result <> "YES" Then
                Retorna_parametros_almacenamiento_documento_relacionado_digitalizado = "Error formateando fecha almacenamiento Funcion: Formatea_Fecha_Almacenamiento " & Result
                Exit Function
            End If
            matri_gestion.FECHA_ELABORACION = date1al
            '---------------------------------------------
            'Asigna los datos de gestion a la estructura
            '---------------------------------------------
            For i As Integer = 0 To estructura_gabinete.Length - 1
                If estructura_gabinete(i).CAMPO = "FECHAELABORACION" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.FECHA_ELABORACION
                End If
                If estructura_gabinete(i).CAMPO = "ID_AREA" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_AREA
                End If
                If estructura_gabinete(i).CAMPO = "ID_SERIE" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_SERIE
                End If
                If estructura_gabinete(i).CAMPO = "ID_SUB_SERIE" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_SUB_SERIE
                End If
                If estructura_gabinete(i).CAMPO = "ID_TIPODOCUMENTO" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_TIPODOCUMENTO
                End If
                If estructura_gabinete(i).CAMPO = "ID_USUARIO_GESTION" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_USUARIO_GESTION
                End If
                If estructura_gabinete(i).CAMPO = "NOMBRESERIE" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.NOMBRE_SERIE
                End If
                If estructura_gabinete(i).CAMPO = "NOMBRESUBSERIE" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.NOMBRE_SUB_SERIE
                End If
                If estructura_gabinete(i).CAMPO = "ID_EXPEDIENTE" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_EXPEDIENTE
                End If
                If estructura_gabinete(i).CAMPO = "ID_TIPO_EXPEDIENTE" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_EXPEDIENTE
                End If
                If estructura_gabinete(i).CAMPO = "ID_UNIDAD_CONSERVACION" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_UNIDAD_CONSERVACION
                End If
                If estructura_gabinete(i).CAMPO = "ID_TIPO_UNIDAD_CONSERVACION" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_UNIDAD_CONSERVACION
                End If
                If estructura_gabinete(i).CAMPO = "ID_TIPO_UNIDAD_DOCUMENTAL" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_UNIDAD_CONSERVACION
                End If
                If estructura_gabinete(i).CAMPO = "ID_CLASE_DOCUMENTO" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_CLASE_DOCUMENTO
                End If
                If estructura_gabinete(i).CAMPO = "EXPEDIENTE" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.EXPEDIENTE
                End If
                If estructura_gabinete(i).CAMPO = "UNIDADCONSERVA" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.UNIDAD_CONSERVACION
                End If
                If estructura_gabinete(i).CAMPO = "CLASEDOCUMENTO" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.CLASE_DOCUMENTO
                End If
            Next
            Retorna_parametros_almacenamiento_documento_relacionado_digitalizado = "YES"
        Catch ex As Exception
            Retorna_parametros_almacenamiento_documento_relacionado_digitalizado = "Inconsistencia general función Retorna_parametros_almacenamiento_documento_relacionado_digitalizado " & ex.Message
        End Try
    End Function
    'Function Almacenamiento_digitalizados_a_flujo_trabajo(ByVal id_tarea As Long,
    '                                                      ByRef ID_ALMACEN As Long, ByVal f As Integer,
    '                                                      ByRef datos_image As stru_datos_image_lista) As String
    '    Try
    '        Dim Result As String = ""
    '        Dim RefclasDigitaliza As New ClassWorkflowDigitalizacion
    '        Dim Selection As String = HttpContext.Current.Session.Item("SELECCIONTEMPORAL")
    '        Dim spl() As String = Selection.Split("|")
    '        Dim Matri_Documentos_Final() As String
    '        Erase Matri_Documentos_Final
    '        '--------------------------------------------------------
    '        'Retorna los documentos almacenados en el file system
    '        '--------------------------------------------------------
    '        Result = RefclasDigitaliza.SolicitaMatrizDocumentosDigitalizados(Val(spl(0)),
    '                                                                         HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ESCANER"),
    '                                                                         Matri_Documentos_Final)
    '        If Result <> "YES" Then
    '            Almacenamiento_digitalizados_a_flujo_trabajo = Result
    '            Exit Function
    '        End If
    '        '----------------------------------------------------
    '        'Obtiene los datos de almacenamiento
    '        '----------------------------------------------------
    '        Dim matri_datos() As Datos_Almacenamiento
    '        Dim Gabinete As String = ""
    '        Dim Radicado As String = ""
    '        Dim Campo_Radicado As String = ""
    '        Gabinete = HttpContext.Current.Session.Item("DG_NOMBRE_GABINETE")
    '        Dim id_imagen_copia As Long = 0
    '        Dim Ref_clas_seleccion As New Classselecciotarea
    '        Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
    '        Result = Class_DAT_ADIC_TAR.SolicitaIdImagenRelacionadaTareaworkflowIdRuta(HttpContext.Current.Session.Item("Id_Ruta_Workflow"),
    '                                                                                   id_tarea,
    '                                                                                   id_imagen_copia)
    '        If Result <> "YES" Then
    '            Almacenamiento_digitalizados_a_flujo_trabajo = Result
    '            Exit Function
    '        End If
    '        Dim Refeclasaladir As New ClassAñadirDocumento
    '        Dim matri_datos_almacen() As String
    '        Erase matri_datos_almacen
    '        Result = Refeclasaladir.Obtiene_Valores_Campos_Documentos_a_Duplicar(matri_datos_almacen,
    '                                                                             Gabinete,
    '                                                                             id_imagen_copia)
    '        If Result <> "YES" Then
    '            Almacenamiento_digitalizados_a_flujo_trabajo = "Imposible encontrar campos para la imagen (" & id_imagen_copia & ") en el gabinete " & Gabinete & "Error relacionado : " & Result
    '            Exit Function
    '        End If

    '        Result = Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(id_tarea,
    '                                                                            Radicado)
    '        If Result <> "YES" Then
    '            Almacenamiento_digitalizados_a_flujo_trabajo = Result
    '            Exit Function
    '        End If
    '        Dim matri_gestion As estructure_gestion = Nothing
    '        Dim ClassGaExpediente As New ClassGaExpediente
    '        Result = ClassGaExpediente.SolicitaEstructuraExpedienteDocumentoVinculante(matri_gestion,
    '                                                                                   Gabinete,
    '                                                                                   id_imagen_copia,
    '                                                                                   id_tarea,
    '                                                                                   Radicado,
    '                                                                                   HttpContext.Current.Session("WF_RUTAWORKFLOW"),
    '                                                                                   HttpContext.Current.Session("Id_Ruta_Workflow"),
    '                                                                                   "")
    '        If Result <> "YES" Then
    '            Almacenamiento_digitalizados_a_flujo_trabajo = Result
    '            Exit Function
    '        End If
    '        '----------------------------------------------
    '        'Configura la radicacion tipo tramite
    '        '----------------------------------------------
    '        Dim id_tipo_documento As Integer = 0
    '        Dim id_area As Integer = 0
    '        Dim id_serie As Integer = 0
    '        Dim id_sub_serie As Integer = 0
    '        Dim descripcion_tipo_documento As String = ""
    '        Dim nombre_area As String = ""
    '        Dim nombre_serie As String = ""
    '        Dim nombre_sub_serie As String = ""
    '        Dim nombre_tipo_documento As String = ""
    '        Dim id_clase_documento As Integer = 0
    '        Dim fecha_elaboracion As String = ""
    '        Dim reflcas_tipo_documento As New ClassGaTipoDocumental
    '        Dim Refclas_almacenamiento As New ClassAlmacenamiento
    '        Dim Ref_Class_ra_dig_tipos_docum_lista_chequeo As New ra_dig_tipos_docum_lista_chequeo
    '        If HttpContext.Current.Session.Item("DG_LISTA_CHEQUEO") <> -1 Then
    '            Dim stru As stru_tipo_lista_chequeo = Nothing
    '            Result = Ref_Class_ra_dig_tipos_docum_lista_chequeo.Solicita_datos_tipo_documental_lista_chequeo(HttpContext.Current.Session.Item("DG_LISTA_CHEQUEO"),
    '                                                                                                             stru)
    '            If Result <> "YES" Then
    '                Almacenamiento_digitalizados_a_flujo_trabajo = Result
    '                Exit Function
    '            End If
    '            If stru.subseries_documentales_Id_SubSeries <> 0 Then
    '                id_tipo_documento = stru.tipos_doc_subseries_Id_Tipos_Doc_SubSerie
    '            Else
    '                id_tipo_documento = stru.tipo_doc_series_Id_Tipo_Doc_Series
    '            End If
    '            id_serie = stru.series_documentales_Id_Series
    '            id_sub_serie = stru.subseries_documentales_Id_SubSeries
    '            Dim ref_Class_series_documentales As New Class_series_documentales
    '            Result = ref_Class_series_documentales.Retorna_id_area_serie_documental(id_serie,
    '                                                                                    id_area)
    '            If Result <> "YES" Then
    '                Almacenamiento_digitalizados_a_flujo_trabajo = Result
    '                Exit Function
    '            End If
    '            Dim ref_Class_ra_tipo_doc_series As New Class_ra_tipo_doc_series
    '            If id_tipo_documento <> 0 Then
    '                Result = ref_Class_ra_tipo_doc_series.Retorna_nombre_tipo_documental(id_serie,
    '                                                                                     id_sub_serie,
    '                                                                                     id_tipo_documento,
    '                                                                                     descripcion_tipo_documento)
    '                If Result <> "YES" Then
    '                    Almacenamiento_digitalizados_a_flujo_trabajo = Result
    '                    Exit Function
    '                End If
    '            End If
    '            nombre_tipo_documento = "DOCUMENTO DIGITALIZADO"
    '            Result = reflcas_tipo_documento.Retorna_id_tipo_documento("DOCUMENTO DIGITALIZADO", id_clase_documento)
    '            If Result <> "YES" Then
    '                Almacenamiento_digitalizados_a_flujo_trabajo = Result
    '                Exit Function
    '            End If
    '            Dim ref_Class_areas_depart_radicacion As New Class_areas_depart_radicacion
    '            If id_area <> 0 Then
    '                Result = ref_Class_areas_depart_radicacion.Retorna_nombre_area_por_id_area(id_area,
    '                                                                                           nombre_area)
    '                If Result <> "YES" Then
    '                    Almacenamiento_digitalizados_a_flujo_trabajo = Result
    '                    Exit Function
    '                End If
    '            End If

    '            If id_serie <> 0 Then
    '                Result = ref_Class_series_documentales.Retorna_nombre_serie_id_serie(id_serie,
    '                                                                                     nombre_serie)
    '                If Result <> "YES" Then
    '                    Almacenamiento_digitalizados_a_flujo_trabajo = Result
    '                    Exit Function
    '                End If
    '            End If
    '            Dim Class_subseries_documentales As New Class_subseries_documentales
    '            If id_sub_serie <> 0 Then
    '                Result = Class_subseries_documentales.Retorna_nombre_sub_serie(id_sub_serie,
    '                                                                                nombre_sub_serie)
    '                If Result <> "YES" Then
    '                    Almacenamiento_digitalizados_a_flujo_trabajo = Result
    '                    Exit Function
    '                End If
    '            End If
    '            Dim date1al As String = Date.Today
    '            Result = ""
    '            Dim ref_ClassGestionFechas As New ClassGestionFechas
    '            Result = ref_ClassGestionFechas.Formatea_Fecha_Almacenamiento(date1al)
    '            If Result <> "YES" Then
    '                Almacenamiento_digitalizados_a_flujo_trabajo = "Error formateando fecha almacenamiento Funcion: Almacenamiento_Documentos_Digitalizados " & Result
    '                Exit Function
    '            Else
    '                fecha_elaboracion = date1al
    '            End If
    '        End If
    '        Dim Ref_producion As New ClassGaProducionDocumental
    '        Dim inventario_documental As Integer = 0
    '        Dim aplica_trd As Integer = 0
    '        Dim asigna_unidad As Integer = 0
    '        Dim Ref_Class_system1 As New Class_system1
    '        Result = Ref_Class_system1.Verifica_opcion_aplicar_trd_inventario_unidad_documental_gabinete(Gabinete,
    '                                                                                                     inventario_documental,
    '                                                                                                     aplica_trd,
    '                                                                                                     asigna_unidad)
    '        If Result <> "YES" Then
    '            Almacenamiento_digitalizados_a_flujo_trabajo = Result
    '            Exit Function
    '        End If
    '        If aplica_trd = 1 Then
    '            If nombre_tipo_documento = "" Then
    '                Result = reflcas_tipo_documento.Retorna_id_tipo_documento("DOCUMENTO DIGITALIZADO", id_clase_documento)
    '                If Result <> "YES" Then
    '                    Almacenamiento_digitalizados_a_flujo_trabajo = Result
    '                    Exit Function
    '                Else
    '                    nombre_tipo_documento = "DOCUMENTO DIGITALIZADO"
    '                End If
    '            End If
    '        End If
    '        matri_gestion.CLASE_DOCUMENTO = nombre_tipo_documento
    '        matri_gestion.ID_AREA = id_area
    '        matri_gestion.ID_CLASE_DOCUMENTO = id_clase_documento
    '        matri_gestion.ID_SERIE = id_serie
    '        matri_gestion.ID_SUB_SERIE = id_sub_serie
    '        matri_gestion.ID_TIPO_UNIDAD_CONSERVACION = 0
    '        matri_gestion.ID_TIPODOCUMENTO = id_tipo_documento
    '        matri_gestion.ID_UNIDAD_CONSERVACION = 0
    '        matri_gestion.ID_USUARIO_GESTION = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
    '        matri_gestion.TIPO_UNIDAD_DOCUMENTAL = 0
    '        matri_gestion.UNIDAD_CONSERVACION = ""
    '        matri_gestion.FECHA_ELABORACION = fecha_elaboracion
    '        matri_gestion.TIPODOCUMENTO = descripcion_tipo_documento
    '        matri_gestion.NOMBRE_SERIE = nombre_serie
    '        matri_gestion.NOMBRE_SUB_SERIE = nombre_sub_serie
    '        ReDim Preserve matri_datos(0)
    '        matri_datos(0).nombre_campo = "CLASEDOCUMENTO"
    '        matri_datos(0).valor_campo = nombre_tipo_documento
    '        ReDim Preserve matri_datos(1)
    '        matri_datos(1).nombre_campo = "FECHAELABORACION"
    '        matri_datos(1).valor_campo = fecha_elaboracion
    '        ReDim Preserve matri_datos(2)
    '        matri_datos(2).nombre_campo = "TIPODOCUMENTO"
    '        matri_datos(2).valor_campo = descripcion_tipo_documento
    '        ReDim Preserve matri_datos(3)
    '        matri_datos(3).nombre_campo = "NOMBRESERIE"
    '        matri_datos(3).valor_campo = nombre_serie
    '        ReDim Preserve matri_datos(4)
    '        matri_datos(4).nombre_campo = "NOMBRESUBSERIE"
    '        matri_datos(4).valor_campo = nombre_sub_serie
    '        '----------------------------------------------
    '        'Genera la matriz de datos de almacenamiento
    '        '----------------------------------------------
    '        Dim Refclas_Class_DETALLE_GABIENETE As New Class_DETALLE_GABIENETE
    '        Result = Refclas_Class_DETALLE_GABIENETE.Actualiza_Valores_Campos_Almacenamiento(matri_datos_almacen,
    '                                                                                         Gabinete,
    '                                                                                         matri_datos)
    '        If Result <> "YES" Then
    '            Almacenamiento_digitalizados_a_flujo_trabajo = "Imposible general mariz datos almacenamiento " & Result
    '            Exit Function
    '        End If
    '        If matri_datos_almacen Is Nothing Then
    '            Almacenamiento_digitalizados_a_flujo_trabajo = "Matriz de datos de almacenamiento es nothing "
    '            Exit Function
    '        End If
    '        '----------------------------------------------
    '        'Obtiene el tipo documento 
    '        '----------------------------------------------
    '        Dim Tipo_Documento As Integer = -1
    '        Dim Refclasvisor As New Classactualizacionvisor
    '        Result = ""
    '        Dim filinf As New FileInfo(Matri_Documentos_Final(0))
    '        Dim Class_da_extension As New Class_da_extension
    '        Result = Class_da_extension.Solicita_Tipo_Documento_Extension(filinf.Extension,
    '                                                                      Tipo_Documento)
    '        If Result <> "YES" Then
    '            Almacenamiento_digitalizados_a_flujo_trabajo = Result
    '            Exit Function
    '        End If
    '        Dim ClassDaGabinete As New ClassDaGabinete
    '        Dim icono As String = ""
    '        ClassDaGabinete.Agrega_icono_image_fownt_java(Tipo_Documento.ToString,
    '                                                      icono)
    '        '-----------------------------------------------
    '        'Almacena documento en la base docuarchi.net
    '        '-----------------------------------------------
    '        Dim UserWf As String = "WF-" & HttpContext.Current.Session.Item("Login_Usuario_Workfow")
    '        Result = ""
    '        Dim estado_firma_digital As Integer = 0
    '        Result = Refclas_almacenamiento.Almacenamiento("", "", Gabinete, 0, matri_datos_almacen, 2,
    '        Matri_Documentos_Final.Length, Tipo_Documento, Matri_Documentos_Final, 0, ID_ALMACEN,
    '        Tipo_Documento, HttpContext.Current.Session.Item("GA_IDEMPRESA"),
    '        HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
    '        matri_gestion.ID_AREA, matri_gestion.ID_SERIE, matri_gestion.ID_SUB_SERIE,
    '        matri_gestion.ID_TIPODOCUMENTO, matri_gestion.ID_EXPEDIENTE, matri_gestion.ID_TIPO_EXPEDIENTE,
    '        matri_gestion.ID_UNIDAD_CONSERVACION, matri_gestion.ID_TIPO_UNIDAD_CONSERVACION,
    '        matri_gestion.ID_CLASE_DOCUMENTO, matri_gestion.EXPEDIENTE, matri_gestion.NOMBRE_SERIE,
    '        matri_gestion.NOMBRE_SUB_SERIE, matri_gestion.TIPODOCUMENTO, matri_gestion.UNIDAD_CONSERVACION,
    '        matri_gestion.CLASE_DOCUMENTO, matri_gestion.FECHA_ELABORACION, Radicado, "", 0, 0, estado_firma_digital)
    '        If Result <> "YES" Then
    '            Almacenamiento_digitalizados_a_flujo_trabajo = Result
    '            Exit Function
    '        End If
    '        Dim val_documento As String = matri_gestion.TIPODOCUMENTO
    '        If val_documento = "" Then
    '            val_documento = "D-" & ID_ALMACEN
    '        End If
    '        If descripcion_tipo_documento = "" Then
    '            descripcion_tipo_documento = val_documento
    '        End If
    '        datos_image.nombre_gabinete = Gabinete
    '        datos_image.id_imagen = ID_ALMACEN
    '        datos_image.radicado = Radicado
    '        datos_image.tipodocumental = descripcion_tipo_documento
    '        datos_image.notipodocumento = val_documento
    '        datos_image.extension = UCase(filinf.Extension)
    '        datos_image.icono_icono_awe_some = icono
    '        datos_image.estado_firma_digital = estado_firma_digital
    '        Almacenamiento_digitalizados_a_flujo_trabajo = "YES"
    '    Catch ex As Exception
    '        Almacenamiento_digitalizados_a_flujo_trabajo = "Inconsistencia función Almacenamiento_digitalizados_a_flujo_trabajo " & ex.Message
    '    End Try
    'End Function
    Function PreAlmacenaDocumentoAnexosEnlaceIntegracionSII(ByVal IdTipoChekLista As Integer,
                                                            ByVal DescripcionTipo As String,
                                                            ByVal IdTipoTaramite As Integer,
                                                            ByVal MultiAnexos As Integer,
                                                            ByVal Gabinete As String,
                                                            ByVal CodigoBarras As String,
                                                            ByVal ReciboSII As String,
                                                            ByVal CDlistaAnexosSII As CDlistaAnexosSII,
                                                            ByVal NombreClaseDocumento As String,
                                                            ByRef IdImagenAlamacenada As Integer,
                                                            ByRef EstructuraDatosImagen As stru_datos_image_lista) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Prepara la estructura para el almacenamineto de los anexos de integración SII
        '          
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------                             
        'Gabinete                   : Representa el nombre del gabinete de almacenamiento
        'ReciboSII                  : Representa el consecutivo de recibo SII el cual representa el
        '                             consecutivo de radicado
        'CodigoBarras               : Representa el codigo de barras de la integfracón con el sistema SII
        'RutaArchivoAlmacenar       : Representa la ruta del archivo de almacenamiento
        'DescripcionTipo            : Representa la identificación literal del tipo documental
        'IdTipoChekLista            : Representa la identificación del tramite en la lista de chequeo
        'MultiAnexos                : Representa si se almacenan multiplex anexos donde el sistema debe
        '                             determinar la tipologia por relación
        'CDlistaAnexosSII           : Representa las estructura de los datos del anexo 
        'IdTipoTaramite             : Representa la identificación del tipo de tramite de la tabla 
        '                             tipo_doc_entrante
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'IdImagenAlamacenada  : Retorna la identificación de del documento alamcenado en el gabinete
        'EstructuraDatosImagen : Retorna la estrucutura de la imagen para el registro en la interfaz
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-07-15
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Class_ra_dig_config_digitalizacion As New Class_ra_dig_config_digitalizacion
            Dim StruConfigDigitalizacion As Stru_config_digitalizacion = Nothing
            Dim NombreRutaWorflow As String = HttpContext.Current.Session("WF_RUTAWORKFLOW")
            Dim IdRutaWorkflow As Integer = HttpContext.Current.Session("Id_Ruta_Workflow")
            Dim IdTareaWorkflow As Integer = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA_ENLACE")
            If MultiAnexos = 0 Then
                Result = Class_ra_dig_config_digitalizacion.SolicitaDatosConfiguracionDigitalizacionPorTramite(IdTipoTaramite,
                                                                                                                    StruConfigDigitalizacion)
                If Result <> "YES" Then
                    PreAlmacenaDocumentoAnexosEnlaceIntegracionSII = Result
                    Exit Function
                End If
                If StruConfigDigitalizacion.OBLIGA_LISTA_CHEQUEO = 1 And (IdTipoChekLista = -1 Or IdTipoChekLista = 0) Then
                    PreAlmacenaDocumentoAnexosEnlaceIntegracionSII = "Por favor, indique el tipo documental correspondiente al documento anexo.​"
                    Exit Function
                End If
            Else
                '////Debe agregar la logica para buscar la relación del tipo tramite en la relación SII
            End If
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim StructureDatosTareaWorkflow As structure_datos_tarea_workflow = Nothing
            Result = Class_DAT_ADIC_TAR.SolicitaDatosEstructuraBasicaTareaWorkflow(NombreRutaWorflow,
                                                                                   IdTareaWorkflow,
                                                                                   StructureDatosTareaWorkflow)
            If Result <> "YES" Then
                PreAlmacenaDocumentoAnexosEnlaceIntegracionSII = Result
                Exit Function
            End If
            Dim stru_datos_image_lista As stru_datos_image_lista = Nothing
            Dim RutaVirtual As String = "../Temp_Image/" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION").ToString & "/DONWLOAD/"
            Dim RutaFisica As String = HttpContext.Current.Server.MapPath(RutaVirtual)
            If Directory.Exists(RutaFisica) = False Then
                Directory.CreateDirectory(RutaFisica)
            End If
            Dim FormatoFile As String = ""
            If CDlistaAnexosSII.formato = "" Then
                FormatoFile = ".PDF"
            Else
                If InStr(CDlistaAnexosSII.formato, ".") = 0 Then
                    FormatoFile = "." & CDlistaAnexosSII.formato
                Else
                    FormatoFile = CDlistaAnexosSII.formato
                End If
            End If
            Dim Archivo As String = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION").ToString & "_doc_adjunto_" & FormatoFile
            Dim ArchivoDonwload As String = RutaFisica & Archivo
            If File.Exists(ArchivoDonwload) Then
                Kill(ArchivoDonwload)
            End If
            Dim ObJectElement As Object = Nothing
            Dim Class_file_byte As New Class_file_byte
            Result = Class_file_byte.DownloadFileViaRestAPI(CDlistaAnexosSII.url,
                                                            ObJectElement,
                                                            "MyDocumentLib",
                                                            Archivo,
                                                            RutaFisica)
            If Result <> "YES" Then
                PreAlmacenaDocumentoAnexosEnlaceIntegracionSII = "Imposible descargar el anexo desde el servicio web SII. La operación no pudo completarse debido al siguiente mensaje:" & Result
                Exit Function
            End If
            HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA") = ArchivoDonwload
            '////------------------------Asigna valores y campos ----------------------///
            Dim CDcamposAsignaAlmacenamiento As New List(Of CDcamposAsignaAlmacenamiento)
            Dim IlistCDcamposAsignaAlmacenamiento As New CDcamposAsignaAlmacenamiento
            If CDlistaAnexosSII.proponente Is Nothing Then
                CDlistaAnexosSII.proponente = ""
            End If
            If CDlistaAnexosSII.matricula Is Nothing Then
                CDlistaAnexosSII.matricula = ""
            End If
            '///---------Valida caso SO para las esal------///
            Dim Matricula As String = CDlistaAnexosSII.matricula
            If Matricula <> "" Then
                Matricula = Matricula.Replace("S0", "")
            End If
            Dim MatriculaProponente = CDlistaAnexosSII.proponente
            '///--------Remplaza caracteres no validos para el campo matricula SII----------////
            Dim ClassCarateres As New ClassCarateres
            Dim CDcarateres As New List(Of CDcarateres)
            Result = ClassCarateres.SolicitaEstructuraCarateres(2,
                                                                CDcarateres)
            If Result <> "YES" Then
                PreAlmacenaDocumentoAnexosEnlaceIntegracionSII = Result
                Exit Function
            End If
            '----////Remplaza caracteres matricula mercantil y esal---------////
            If Matricula <> "" And Not CDcarateres Is Nothing Then
                ClassCarateres.RemplazaCaracteresNoValidos(CDcarateres, Matricula)
            End If
            '----////Remplaza caracteres matricula rup---------////
            If MatriculaProponente <> "" And Not CDcarateres Is Nothing Then
                ClassCarateres.RemplazaCaracteresNoValidos(CDcarateres, MatriculaProponente)
            End If
            '///-------Solicita el valor del campo matricula del código de barras SII si no existe en los datos del anexo----///
            Dim Class_ConsultarRadicado_sii As New Class_ConsultarRadicado_sii
            Dim ConsultarRadicado_sii As ConsultarRadicado_sii = Nothing
            If Matricula = "" And (Gabinete = "MERCANTIL" Or Gabinete = "ESAL") Then
                Result = Class_ConsultarRadicado_sii.ConSultarRadicado(CodigoBarras,
                                                                       ConsultarRadicado_sii)
                If Result <> "YES" Then
                    PreAlmacenaDocumentoAnexosEnlaceIntegracionSII = Result
                    Exit Function
                End If
                Matricula = ConsultarRadicado_sii.matricula
                MatriculaProponente = ConsultarRadicado_sii.proponente
                If Matricula <> "" And Not CDcarateres Is Nothing Then
                    ClassCarateres.RemplazaCaracteresNoValidos(CDcarateres, Matricula)
                End If
                If MatriculaProponente <> "" And Not CDcarateres Is Nothing Then
                    ClassCarateres.RemplazaCaracteresNoValidos(CDcarateres, MatriculaProponente)
                End If
            End If
            If MatriculaProponente = "" And Gabinete = "RUP" Then
                Result = Class_ConsultarRadicado_sii.ConSultarRadicado(CodigoBarras,
                                                                       ConsultarRadicado_sii)
                If Result <> "YES" Then
                    PreAlmacenaDocumentoAnexosEnlaceIntegracionSII = Result
                    Exit Function
                End If
                Matricula = ConsultarRadicado_sii.matricula
                MatriculaProponente = ConsultarRadicado_sii.proponente
                If Matricula <> "" And Not CDcarateres Is Nothing Then
                    ClassCarateres.RemplazaCaracteresNoValidos(CDcarateres, Matricula)
                End If
                If MatriculaProponente <> "" And Not CDcarateres Is Nothing Then
                    ClassCarateres.RemplazaCaracteresNoValidos(CDcarateres, MatriculaProponente)
                End If
            End If
            If CDlistaAnexosSII.nombre Is Nothing Then
                CDlistaAnexosSII.nombre = ""
            End If
            If CDlistaAnexosSII.identificacion Is Nothing Then
                CDlistaAnexosSII.identificacion = ""
            End If
            Dim RazonSocial As String = CDlistaAnexosSII.nombre
            Dim Identificacion As String = CDlistaAnexosSII.identificacion

            '///--extrayendo los datos nombre y nit desde el expediente SII-----------------------////
            Dim ClassConsultaExpedienteSII As New ClassConsultaExpedienteSII
            Dim StruSiiCahcheInscripcion As New StruSiiCahcheInscripcion
            If MatriculaProponente <> "" Or Matricula <> "" Then
                Result = ClassConsultaExpedienteSII.SolicitaEstructuraExpedienteSII(Matricula,
                                                                                    MatriculaProponente,
                                                                                    Gabinete,
                                                                                    StruSiiCahcheInscripcion)
                If Result <> "YES" Then
                    PreAlmacenaDocumentoAnexosEnlaceIntegracionSII = Result & ". Contenido de la matricula (" & Matricula & MatriculaProponente & ")."
                    Exit Function
                End If
                RazonSocial = StruSiiCahcheInscripcion.Rsocial
                Identificacion = StruSiiCahcheInscripcion.NitIdentificacion
            End If
            If RazonSocial <> "" Then
                RazonSocial = Left(RazonSocial, 40)
                RazonSocial = RazonSocial.Replace("/", "-")
                RazonSocial = RazonSocial.Replace("\", "-")
            End If
            ClassCarateres.RemplazaCaracteresNoValidos(HttpContext.Current.Session.Item("DG_CDCARACTERES"), RazonSocial)
            IlistCDcamposAsignaAlmacenamiento = New CDcamposAsignaAlmacenamiento
            IlistCDcamposAsignaAlmacenamiento.NombreCampoGabinete = "CODBARRAS"
            IlistCDcamposAsignaAlmacenamiento.ValorCampoGabinete = CodigoBarras
            CDcamposAsignaAlmacenamiento.Add(IlistCDcamposAsignaAlmacenamiento)
            IlistCDcamposAsignaAlmacenamiento = New CDcamposAsignaAlmacenamiento
            IlistCDcamposAsignaAlmacenamiento.NombreCampoGabinete = "ENLASE"
            IlistCDcamposAsignaAlmacenamiento.ValorCampoGabinete = ReciboSII
            CDcamposAsignaAlmacenamiento.Add(IlistCDcamposAsignaAlmacenamiento)
            Select Case Gabinete
                Case "MERCANTIL"
                    IlistCDcamposAsignaAlmacenamiento = New CDcamposAsignaAlmacenamiento
                    IlistCDcamposAsignaAlmacenamiento.NombreCampoGabinete = "MATRICULA"
                    IlistCDcamposAsignaAlmacenamiento.ValorCampoGabinete = Matricula
                    CDcamposAsignaAlmacenamiento.Add(IlistCDcamposAsignaAlmacenamiento)
                    IlistCDcamposAsignaAlmacenamiento = New CDcamposAsignaAlmacenamiento
                    IlistCDcamposAsignaAlmacenamiento.NombreCampoGabinete = "RAZONSOCIAL"
                    IlistCDcamposAsignaAlmacenamiento.ValorCampoGabinete = RazonSocial
                    CDcamposAsignaAlmacenamiento.Add(IlistCDcamposAsignaAlmacenamiento)
                    IlistCDcamposAsignaAlmacenamiento = New CDcamposAsignaAlmacenamiento
                    IlistCDcamposAsignaAlmacenamiento.NombreCampoGabinete = "NITCEDULA"
                    IlistCDcamposAsignaAlmacenamiento.ValorCampoGabinete = Identificacion
                    CDcamposAsignaAlmacenamiento.Add(IlistCDcamposAsignaAlmacenamiento)
                Case "ESAL"
                    IlistCDcamposAsignaAlmacenamiento = New CDcamposAsignaAlmacenamiento
                    IlistCDcamposAsignaAlmacenamiento.NombreCampoGabinete = "MATRICULA"
                    IlistCDcamposAsignaAlmacenamiento.ValorCampoGabinete = Matricula
                    CDcamposAsignaAlmacenamiento.Add(IlistCDcamposAsignaAlmacenamiento)
                    IlistCDcamposAsignaAlmacenamiento = New CDcamposAsignaAlmacenamiento
                    IlistCDcamposAsignaAlmacenamiento.NombreCampoGabinete = "RAZONSOCIAL"
                    IlistCDcamposAsignaAlmacenamiento.ValorCampoGabinete = RazonSocial
                    CDcamposAsignaAlmacenamiento.Add(IlistCDcamposAsignaAlmacenamiento)
                    IlistCDcamposAsignaAlmacenamiento = New CDcamposAsignaAlmacenamiento
                    IlistCDcamposAsignaAlmacenamiento.NombreCampoGabinete = "NITCEDULA"
                    IlistCDcamposAsignaAlmacenamiento.ValorCampoGabinete = Identificacion
                    CDcamposAsignaAlmacenamiento.Add(IlistCDcamposAsignaAlmacenamiento)
                Case "RUP"
                    Matricula = MatriculaProponente
                    IlistCDcamposAsignaAlmacenamiento = New CDcamposAsignaAlmacenamiento
                    IlistCDcamposAsignaAlmacenamiento.NombreCampoGabinete = "MATRICULA"
                    IlistCDcamposAsignaAlmacenamiento.ValorCampoGabinete = MatriculaProponente
                    CDcamposAsignaAlmacenamiento.Add(IlistCDcamposAsignaAlmacenamiento)
                    IlistCDcamposAsignaAlmacenamiento = New CDcamposAsignaAlmacenamiento
                    IlistCDcamposAsignaAlmacenamiento.NombreCampoGabinete = "RAZONSOCIAL"
                    IlistCDcamposAsignaAlmacenamiento.ValorCampoGabinete = RazonSocial
                    CDcamposAsignaAlmacenamiento.Add(IlistCDcamposAsignaAlmacenamiento)
                    IlistCDcamposAsignaAlmacenamiento = New CDcamposAsignaAlmacenamiento
                    IlistCDcamposAsignaAlmacenamiento.NombreCampoGabinete = "NITCEDULA"
                    IlistCDcamposAsignaAlmacenamiento.ValorCampoGabinete = Identificacion
                    CDcamposAsignaAlmacenamiento.Add(IlistCDcamposAsignaAlmacenamiento)
            End Select
            Result = AlmacenaDocumentoTareaWorkflow(1,
                                                    Gabinete,
                                                    ReciboSII,
                                                    ArchivoDonwload,
                                                    NombreRutaWorflow,
                                                    IdRutaWorkflow,
                                                    IdTareaWorkflow,
                                                    DescripcionTipo,
                                                    IdTipoChekLista,
                                                    2,
                                                    CDcamposAsignaAlmacenamiento,
                                                    Matricula,
                                                    "SII",
                                                    NombreClaseDocumento,
                                                    IdImagenAlamacenada,
                                                    EstructuraDatosImagen)
            If Result <> "YES" Then
                PreAlmacenaDocumentoAnexosEnlaceIntegracionSII = Result
                Exit Function
            End If
            If StructureDatosTareaWorkflow.ID_IMAGEN = 0 Then
                Result = Class_DAT_ADIC_TAR.AcualizaIdImagenTareaWorkflow(IdTareaWorkflow,
                                                                          StructureDatosTareaWorkflow.ID_GABINETE,
                                                                          NombreRutaWorflow,
                                                                          IdImagenAlamacenada)
                If Result <> "YES" Then
                    PreAlmacenaDocumentoAnexosEnlaceIntegracionSII = Result
                    Exit Function
                End If
            End If
            PreAlmacenaDocumentoAnexosEnlaceIntegracionSII = "YES"
            Exit Function
        Catch ex As Exception
            PreAlmacenaDocumentoAnexosEnlaceIntegracionSII = "Inconsistencia general función PreAlmacenaDocumentoAnexosEnlaceIntegracionSII " & ex.Message
        End Try
    End Function
    Function PreAlmacenaConstanciaIsncripcionsSII(ByVal IdTipoChekLista As Integer,
                                                  ByVal DescripcionTipo As String,
                                                  ByVal IdTipoTaramite As Integer,
                                                  ByVal NombreClaseDocumento As String,
                                                  ByVal CIncripcionSII As CIncripcionSII,
                                                  ByRef IdImagenAlmacenada As Integer,
                                                  ByRef EstructuraDatosImagen As stru_datos_image_lista) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Prepara la estructura para el almacenamiento constancia inscripción integración SII
        '          
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------                             
        'DescripcionTipo            : Representa la identificación literal del tipo documental
        'IdTipoChekLista            : Representa la identificación del tramite en la lista de chequeo
        'CIncripcionSII             : Representa las estructura de los datos del anexo 
        'IdTipoTaramite             : Representa la identificación del tipo de tramite de la tabla 
        '                             tipo_doc_entrante
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'IdImagenAlamacenada  : Retorna la identificación de del documento alamcenado en el gabinete
        'EstructuraDatosImagen : Retorna la estrucutura de la imagen para el registro en la interfaz
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-07-28
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try
            Dim Result As String = ""
            Dim Class_ra_dig_config_digitalizacion As New Class_ra_dig_config_digitalizacion
            Dim StruConfigDigitalizacion As Stru_config_digitalizacion = Nothing
            Dim NombreRutaWorflow As String = HttpContext.Current.Session("WF_RUTAWORKFLOW")
            Dim IdRutaWorkflow As Integer = HttpContext.Current.Session("Id_Ruta_Workflow")
            Dim IdTareaWorkflow As Integer = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")
            Result = Class_ra_dig_config_digitalizacion.SolicitaDatosConfiguracionDigitalizacionPorTramite(IdTipoTaramite,
                                                                                                           StruConfigDigitalizacion)
            If Result <> "YES" Then
                PreAlmacenaConstanciaIsncripcionsSII = Result
                Exit Function
            End If
            If StruConfigDigitalizacion.OBLIGA_LISTA_CHEQUEO = 1 And (IdTipoChekLista = -1 Or IdTipoChekLista = 0) Then
                PreAlmacenaConstanciaIsncripcionsSII = "Por favor, indique el tipo documental correspondiente a la constancia de inscripción.​"
                Exit Function
            End If
            Dim RutaVirtual As String = "../Temp_Image/" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION").ToString & "/DONWLOAD/"
            Dim RutaFisica As String = HttpContext.Current.Server.MapPath(RutaVirtual)
            If Directory.Exists(RutaFisica) = False Then
                Directory.CreateDirectory(RutaFisica)
            End If
            Dim Archivo As String = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION").ToString & "-" & CIncripcionSII.COD_BARRA_SII & "-" & CIncripcionSII.LIBRO_SII & "-" & CIncripcionSII.REGISTRO_SII & ".PDF"
            Dim ArchivoDonwload As String = RutaFisica & Archivo
            If File.Exists(ArchivoDonwload) Then
                Kill(ArchivoDonwload)
            End If
            Dim Gabinete As String = ""
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = Class_DAT_ADIC_TAR.SolicitaNombreGabneteTareaWokflow(NombreRutaWorflow,
                                                                          IdTareaWorkflow,
                                                                          Gabinete)
            If Result <> "YES" Then
                PreAlmacenaConstanciaIsncripcionsSII = Result
                Exit Function
            End If
            Dim ResgistroPublico As String = ""
            If UCase(Gabinete) = "MERCANTIL" Then
                ResgistroPublico = " MERCANTIL"
            End If
            If UCase(Gabinete) = "ESAL" Then
                ResgistroPublico = " ENTIDADES SIN ANIMO DE LUCRO"
            End If
            If UCase(Gabinete) = "RUP" Then
                ResgistroPublico = " DE PROPONENTES"
            End If
            Dim ClassDaGabinete As New ClassDaGabinete
            '//------------Solicita la estrucutura del cache de regitro de inscripción-----------//
            Dim ClassRaSiiCahcheInscripcion As New ClassRaSiiCahcheInscripcion
            Dim StruSiiCahcheInscripcion As StruSiiCahcheInscripcion = Nothing
            Result = ClassRaSiiCahcheInscripcion.SolicitaEstructuraCacheInscripcionRadicado(CIncripcionSII.RADICADO_SII,
                                                                                            StruSiiCahcheInscripcion)
            If Result <> "YES" Then
                PreAlmacenaConstanciaIsncripcionsSII = Result
                Exit Function
            End If
            Dim ClassConsultaExpedienteSII As New ClassConsultaExpedienteSII
            '//------------Actualiza el campo nombre si el registro del sello no trae el nombre del matriculado------------//
            If CIncripcionSII.RSOCIAL_SII = "" Then
                Result = ClassConsultaExpedienteSII.SolicitaEstructuraExpedienteSII(CIncripcionSII.MATRICULA_SII,
                                                                                    CIncripcionSII.PROPONENTE_SII,
                                                                                    Gabinete,
                                                                                    StruSiiCahcheInscripcion)
                If Result <> "YES" Then
                    PreAlmacenaConstanciaIsncripcionsSII = Result
                    Exit Function
                End If
                CIncripcionSII.RSOCIAL_SII = StruSiiCahcheInscripcion.Rsocial
                CIncripcionSII.NIT_SII = StruSiiCahcheInscripcion.NitIdentificacion
            End If
            Dim Class_ItexShare As New Class_ItexShare
            Result = Class_ItexShare.ItexConstanciaIsncripcionSII(CIncripcionSII,
                                                                  ArchivoDonwload,
                                                                  ResgistroPublico)
            If Result <> "YES" Then
                PreAlmacenaConstanciaIsncripcionsSII = Result
                Exit Function
            End If
            Dim CDcamposAsignaAlmacenamiento As New List(Of CDcamposAsignaAlmacenamiento)
            Dim IlistCDcamposAsignaAlmacenamiento As New CDcamposAsignaAlmacenamiento
            If CIncripcionSII.PROPONENTE_SII Is Nothing Then
                CIncripcionSII.PROPONENTE_SII = ""
            End If
            If CIncripcionSII.MATRICULA_SII Is Nothing Then
                CIncripcionSII.MATRICULA_SII = ""
            End If
            '///---------Valida caso SO para las esal------///
            Dim Matricula As String = CIncripcionSII.MATRICULA_SII
            If Matricula <> "" Then
                Matricula = Matricula.Replace("S0", "")
            End If
            Dim MatriculaProponente = CIncripcionSII.PROPONENTE_SII
            '///--------Remplaza caracteres no validos para el campo matricula SII----------////
            Dim ClassCarateres As New ClassCarateres
            Dim CDcarateres As New List(Of CDcarateres)
            Result = ClassCarateres.SolicitaEstructuraCarateres(2,
                                                                CDcarateres)
            If Result <> "YES" Then
                PreAlmacenaConstanciaIsncripcionsSII = Result
                Exit Function
            End If
            '----////Remplaza caracteres matricula mercantil y esal---------////
            If Matricula <> "" And Not CDcarateres Is Nothing Then
                ClassCarateres.RemplazaCaracteresNoValidos(CDcarateres, Matricula)
            End If
            '----////Remplaza caracteres matricula rup---------////
            If MatriculaProponente <> "" And Not CDcarateres Is Nothing Then
                ClassCarateres.RemplazaCaracteresNoValidos(CDcarateres, MatriculaProponente)
            End If
            ClassCarateres.RemplazaCaracteresNoValidos(HttpContext.Current.Session.Item("DG_CDCARACTERES"), CIncripcionSII.RSOCIAL_SII)
            If CIncripcionSII.ToString <> "" Then
                CIncripcionSII.RSOCIAL_SII = Left(CIncripcionSII.RSOCIAL_SII, 50)
            End If
            IlistCDcamposAsignaAlmacenamiento = New CDcamposAsignaAlmacenamiento
            IlistCDcamposAsignaAlmacenamiento.NombreCampoGabinete = "CODBARRAS"
            IlistCDcamposAsignaAlmacenamiento.ValorCampoGabinete = CIncripcionSII.COD_BARRA_SII
            CDcamposAsignaAlmacenamiento.Add(IlistCDcamposAsignaAlmacenamiento)
            IlistCDcamposAsignaAlmacenamiento = New CDcamposAsignaAlmacenamiento
            IlistCDcamposAsignaAlmacenamiento.NombreCampoGabinete = "ENLASE"
            IlistCDcamposAsignaAlmacenamiento.ValorCampoGabinete = CIncripcionSII.RADICADO_SII
            CDcamposAsignaAlmacenamiento.Add(IlistCDcamposAsignaAlmacenamiento)
            IlistCDcamposAsignaAlmacenamiento = New CDcamposAsignaAlmacenamiento
            IlistCDcamposAsignaAlmacenamiento.NombreCampoGabinete = "MATRICULA"
            IlistCDcamposAsignaAlmacenamiento.ValorCampoGabinete = CIncripcionSII.MATRICULA_SII
            CDcamposAsignaAlmacenamiento.Add(IlistCDcamposAsignaAlmacenamiento)
            IlistCDcamposAsignaAlmacenamiento = New CDcamposAsignaAlmacenamiento
            IlistCDcamposAsignaAlmacenamiento.NombreCampoGabinete = "RAZONSOCIAL"
            IlistCDcamposAsignaAlmacenamiento.ValorCampoGabinete = CIncripcionSII.RSOCIAL_SII
            CDcamposAsignaAlmacenamiento.Add(IlistCDcamposAsignaAlmacenamiento)
            IlistCDcamposAsignaAlmacenamiento = New CDcamposAsignaAlmacenamiento
            IlistCDcamposAsignaAlmacenamiento.NombreCampoGabinete = "NITCEDULA"
            IlistCDcamposAsignaAlmacenamiento.ValorCampoGabinete = CIncripcionSII.NIT_SII
            CDcamposAsignaAlmacenamiento.Add(IlistCDcamposAsignaAlmacenamiento)
            IlistCDcamposAsignaAlmacenamiento = New CDcamposAsignaAlmacenamiento
            IlistCDcamposAsignaAlmacenamiento.NombreCampoGabinete = "LIBRO"
            IlistCDcamposAsignaAlmacenamiento.ValorCampoGabinete = CIncripcionSII.LIBRO_SII
            CDcamposAsignaAlmacenamiento.Add(IlistCDcamposAsignaAlmacenamiento)
            IlistCDcamposAsignaAlmacenamiento = New CDcamposAsignaAlmacenamiento
            IlistCDcamposAsignaAlmacenamiento.NombreCampoGabinete = "INSCRIPCION"
            IlistCDcamposAsignaAlmacenamiento.ValorCampoGabinete = CIncripcionSII.REGISTRO_SII
            CDcamposAsignaAlmacenamiento.Add(IlistCDcamposAsignaAlmacenamiento)
            IlistCDcamposAsignaAlmacenamiento = New CDcamposAsignaAlmacenamiento
            IlistCDcamposAsignaAlmacenamiento.NombreCampoGabinete = "RECIBOCAJA"
            IlistCDcamposAsignaAlmacenamiento.ValorCampoGabinete = CIncripcionSII.RADICADO_SII
            CDcamposAsignaAlmacenamiento.Add(IlistCDcamposAsignaAlmacenamiento)
            Select Case UCase(Gabinete)
                Case "ESAL"
                    IlistCDcamposAsignaAlmacenamiento = New CDcamposAsignaAlmacenamiento
                    IlistCDcamposAsignaAlmacenamiento.NombreCampoGabinete = "FECHAINSCRIP"
                    IlistCDcamposAsignaAlmacenamiento.ValorCampoGabinete = CIncripcionSII.FECHA_SII
                    CDcamposAsignaAlmacenamiento.Add(IlistCDcamposAsignaAlmacenamiento)
                    IlistCDcamposAsignaAlmacenamiento = New CDcamposAsignaAlmacenamiento
                    IlistCDcamposAsignaAlmacenamiento.NombreCampoGabinete = "ACTO"
                    IlistCDcamposAsignaAlmacenamiento.ValorCampoGabinete = CIncripcionSII.ACTO_SII
                    CDcamposAsignaAlmacenamiento.Add(IlistCDcamposAsignaAlmacenamiento)
                    IlistCDcamposAsignaAlmacenamiento = New CDcamposAsignaAlmacenamiento
                    IlistCDcamposAsignaAlmacenamiento.NombreCampoGabinete = "DESCRIPCIONA"
                    IlistCDcamposAsignaAlmacenamiento.ValorCampoGabinete = CIncripcionSII.NACTO_SII
                    CDcamposAsignaAlmacenamiento.Add(IlistCDcamposAsignaAlmacenamiento)
                Case "MERCANTIL"
                    IlistCDcamposAsignaAlmacenamiento = New CDcamposAsignaAlmacenamiento
                    IlistCDcamposAsignaAlmacenamiento.NombreCampoGabinete = "FECHAREGISTR"
                    IlistCDcamposAsignaAlmacenamiento.ValorCampoGabinete = CIncripcionSII.FECHA_SII
                    CDcamposAsignaAlmacenamiento.Add(IlistCDcamposAsignaAlmacenamiento)
                    IlistCDcamposAsignaAlmacenamiento = New CDcamposAsignaAlmacenamiento
                    IlistCDcamposAsignaAlmacenamiento.NombreCampoGabinete = "ACTO"
                    IlistCDcamposAsignaAlmacenamiento.ValorCampoGabinete = CIncripcionSII.ACTO_SII
                    CDcamposAsignaAlmacenamiento.Add(IlistCDcamposAsignaAlmacenamiento)
                    IlistCDcamposAsignaAlmacenamiento = New CDcamposAsignaAlmacenamiento
                    IlistCDcamposAsignaAlmacenamiento.NombreCampoGabinete = "DESCRIACTO"
                    IlistCDcamposAsignaAlmacenamiento.ValorCampoGabinete = CIncripcionSII.NACTO_SII
                    CDcamposAsignaAlmacenamiento.Add(IlistCDcamposAsignaAlmacenamiento)
                Case "RUP"
                    IlistCDcamposAsignaAlmacenamiento = New CDcamposAsignaAlmacenamiento
                    IlistCDcamposAsignaAlmacenamiento.NombreCampoGabinete = "FECHAREGISTR"
                    IlistCDcamposAsignaAlmacenamiento.ValorCampoGabinete = CIncripcionSII.FECHA_SII
                    CDcamposAsignaAlmacenamiento.Add(IlistCDcamposAsignaAlmacenamiento)
                    IlistCDcamposAsignaAlmacenamiento = New CDcamposAsignaAlmacenamiento
                    IlistCDcamposAsignaAlmacenamiento.NombreCampoGabinete = "ACTO"
                    IlistCDcamposAsignaAlmacenamiento.ValorCampoGabinete = CIncripcionSII.ACTO_SII
                    CDcamposAsignaAlmacenamiento.Add(IlistCDcamposAsignaAlmacenamiento)
                    IlistCDcamposAsignaAlmacenamiento = New CDcamposAsignaAlmacenamiento
                    IlistCDcamposAsignaAlmacenamiento.NombreCampoGabinete = "DESCRIACTO"
                    IlistCDcamposAsignaAlmacenamiento.ValorCampoGabinete = CIncripcionSII.NACTO_SII
                    CDcamposAsignaAlmacenamiento.Add(IlistCDcamposAsignaAlmacenamiento)
            End Select
            Result = AlmacenaDocumentoTareaWorkflow(1,
                                                    Gabinete,
                                                    CIncripcionSII.RADICADO_SII,
                                                    ArchivoDonwload,
                                                    NombreRutaWorflow,
                                                    IdRutaWorkflow,
                                                    IdTareaWorkflow,
                                                    DescripcionTipo,
                                                    IdTipoChekLista,
                                                    2,
                                                    CDcamposAsignaAlmacenamiento,
                                                    Matricula,
                                                    "SII",
                                                    NombreClaseDocumento,
                                                    IdImagenAlmacenada,
                                                    EstructuraDatosImagen)
            If Result <> "YES" Then
                PreAlmacenaConstanciaIsncripcionsSII = Result
                Exit Function
            End If
            PreAlmacenaConstanciaIsncripcionsSII = "YES"
            Exit Function
        Catch ex As Exception
            PreAlmacenaConstanciaIsncripcionsSII = "Inconsistencia general funcion PreAlmacenaConstanciaIsncripcionsSII " & ex.Message
        End Try
    End Function
    Function PreAlmacenaDocumentoTareaWorkflow(ByVal DescripcionTipo As String,
                                               ByVal IdTipoChekLista As Integer,
                                               ByVal RutaArchivo As String,
                                               ByVal IdTareaWorkflow As Long,
                                               ByVal IdTipoTramite As Integer,
                                               ByVal NombreClaseDocumento As String,
                                               ByRef IdImagenAlamacenada As Integer,
                                               ByRef EstructuraDatosImagen As stru_datos_image_lista) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Alista los datos para el almacenamiento de un documento
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'DescripcionTipo     : Representa la descripción de la tipologia documental
        'IdTipoChekLista     : Representa la identificación de la lista de chequeo
        'RutaArchivo         : Representa la ruta del archivo de almacenamiento
        'IdTareaWorkflow     : Representa la identificación de la tarea workflow
        'IdTipoTramite       : Representa la identificación del tipo tramite
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'IdImagenAlamacenada   : Retorna la identificación de la imagen almacenada
        'EstructuraDatosImagen : Retorna la estructura del documento almacenado
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-07-23
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try
            Dim Result As String = ""
            Dim Gabinete As String = ""
            Dim IdRutaWorkflow As Integer = HttpContext.Current.Session("Id_Ruta_Workflow")
            Dim NombreRutaWorflow As String = HttpContext.Current.Session("WF_RUTAWORKFLOW")
            Dim Class_ra_dig_config_digitalizacion As New Class_ra_dig_config_digitalizacion
            Dim StruConfigDigitalizacion As Stru_config_digitalizacion = Nothing
            Result = Class_ra_dig_config_digitalizacion.SolicitaDatosConfiguracionDigitalizacionPorTramite(IdTipoTramite,
                                                                                                           StruConfigDigitalizacion)
            If Result <> "YES" Then
                PreAlmacenaDocumentoTareaWorkflow = Result
                Exit Function
            End If
            If StruConfigDigitalizacion.OBLIGA_LISTA_CHEQUEO = 1 And (IdTipoChekLista = -1 Or IdTipoChekLista = 0) Then
                PreAlmacenaDocumentoTareaWorkflow = "Por favor, indique el tipo documental correspondiente al documento que desea adjuntar.​"
                Exit Function
            End If
            '//------------------Solicita estructuta tipo tramite---------------///
            Dim Class_tipo_doc_entrante As New Class_tipo_doc_entrante
            Dim CTipoDocEntrante As New CTipoDocEntrante
            If IdTipoTramite <> 0 And IdTipoTramite <> -1 Then
                Result = Class_tipo_doc_entrante.SolicitaEstructuraTramite(IdTipoTramite,
                                                                           CTipoDocEntrante)
                If Result <> "YES" Then
                    PreAlmacenaDocumentoTareaWorkflow = Result
                    Exit Function
                End If
            End If
            '//-----------------Solicita servicio de integración-----------------------//
            Dim Class_ra_ser_servicioIntegracion As New Class_ra_ser_servicioIntegracion
            Dim RaSerServicioInteracion As New RaSerServicioInteracion
            If Not CTipoDocEntrante Is Nothing Then
                Result = Class_ra_ser_servicioIntegracion.SolicitaEstructuraServicioIntegracion(CTipoDocEntrante.Id_ser_servicioIntegracion,
                                                                                                RaSerServicioInteracion)
                If Result <> "YES" Then
                    PreAlmacenaDocumentoTareaWorkflow = Result
                    Exit Function
                End If
            End If
            '//---------------Solicita la estructura de datos basico de la tarea workflow----------//
            Dim NombreServicioIntegracion As String = RaSerServicioInteracion.NombreServicio
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim StructureDatosTareaWorkflow As structure_datos_tarea_workflow = Nothing
            Result = Class_DAT_ADIC_TAR.SolicitaDatosEstructuraBasicaTareaWorkflow(NombreRutaWorflow,
                                                                                   IdTareaWorkflow,
                                                                                   StructureDatosTareaWorkflow)
            If Result <> "YES" Then
                PreAlmacenaDocumentoTareaWorkflow = Result
                Exit Function
            End If
            If StructureDatosTareaWorkflow.ID_GABINETE = 0 Then
                PreAlmacenaDocumentoTareaWorkflow = "Imposible encontrar el identificador del gabinete asociado a la tarea de Workflow (" & IdTareaWorkflow & ")."
                Exit Function
            End If
            '////------------------------Solicita nombre gabinete ----------------------///
            Dim Class_configuracion_gabinete As New Class_configuracion_gabinete
            Result = Class_configuracion_gabinete.SolicitanombreGabineteWorkflow(StructureDatosTareaWorkflow.ID_GABINETE,
                                                                                 Gabinete)
            If Result <> "YES" Then
                PreAlmacenaDocumentoTareaWorkflow = Result
                Exit Function
            End If
            Dim TipoCaso As String = ""
            '////------------------------Asigna valores y campos para indice de gabinete----------------------///
            Dim CDcamposAsignaAlmacenamiento As New List(Of CDcamposAsignaAlmacenamiento)
            Dim Radicado As String = ""
            Dim CDParameterValoresCamposIndiceGabinete As New CDParameterValoresCamposIndiceGabinete
            Select Case StructureDatosTareaWorkflow.FLUJO_INTERNO_WF
                '///------------------------Caso flujo externo---------------///
                Case 2
                    If NombreServicioIntegracion = "INTEGRACIONSII" Then
                        TipoCaso = "SII"
                    End If
                    '-----Prametros SII Integración---------- "INTEGRACIONSII"
                    CDParameterValoresCamposIndiceGabinete.CDParmeterValoresCamposGabineteSII = New CDParmeterValoresCamposGabineteSII
                    CDParameterValoresCamposIndiceGabinete.CDParmeterValoresCamposGabineteSII.IdTareaWorkflow = IdTareaWorkflow
                    CDParameterValoresCamposIndiceGabinete.CDParmeterValoresCamposGabineteSII.IdRutaWorkflow = IdRutaWorkflow
                    CDParameterValoresCamposIndiceGabinete.CDParmeterValoresCamposGabineteSII.NombreRutaWorkflow = NombreRutaWorflow
                    CDParameterValoresCamposIndiceGabinete.CDParmeterValoresCamposGabineteSII.Gabinete = Gabinete
                    '-----Parameter default-------------------
                    CDParameterValoresCamposIndiceGabinete.CDParmeterValoresCamposGabineteDatAdicTar = New CDParmeterValoresCamposGabineteDatAdicTar
                    CDParameterValoresCamposIndiceGabinete.CDParmeterValoresCamposGabineteDatAdicTar.IdTareaWorkflow = IdTareaWorkflow
                    CDParameterValoresCamposIndiceGabinete.CDParmeterValoresCamposGabineteDatAdicTar.IdRutaWorkflow = IdRutaWorkflow
                    CDParameterValoresCamposIndiceGabinete.CDParmeterValoresCamposGabineteDatAdicTar.NombreRutaWorkflow = NombreRutaWorflow
                    CDParameterValoresCamposIndiceGabinete.CDParmeterValoresCamposGabineteDatAdicTar.Gabinete = Gabinete
                    Result = Class_ra_ser_servicioIntegracion.SolicitaDatosCamposIndiceGabineteIntegracion(CDParameterValoresCamposIndiceGabinete,
                                                                                                           NombreServicioIntegracion,
                                                                                                           Radicado,
                                                                                                           CDcamposAsignaAlmacenamiento)
                    If Result <> "YES" Then
                        PreAlmacenaDocumentoTareaWorkflow = Result
                        Exit Function
                    End If
                     '///------------------------Caso flujo Interno---------------///
                Case 1
                    '///-----Prametros relación gabinete plantilla radicación----------///
                    CDParameterValoresCamposIndiceGabinete.CDParmeterValoresCamposGabinete = New CDParmeterValoresCamposGabinete
                    CDParameterValoresCamposIndiceGabinete.CDParmeterValoresCamposGabinete.IdTareaWorkflow = IdTareaWorkflow
                    CDParameterValoresCamposIndiceGabinete.CDParmeterValoresCamposGabinete.IdRutaWorkflow = IdRutaWorkflow
                    CDParameterValoresCamposIndiceGabinete.CDParmeterValoresCamposGabinete.NombreRutaWorkflow = NombreRutaWorflow
                    CDParameterValoresCamposIndiceGabinete.CDParmeterValoresCamposGabinete.Gabinete = Gabinete
                    Dim ClassDaGabinete As New ClassDaGabinete
                    Result = ClassDaGabinete.SolicitaDatosCamposIndiceGabinete(CDParameterValoresCamposIndiceGabinete.CDParmeterValoresCamposGabinete,
                                                                               Radicado,
                                                                               CDcamposAsignaAlmacenamiento)
                    If Result <> "YES" Then
                        PreAlmacenaDocumentoTareaWorkflow = Result
                        Exit Function
                    End If
                Case Else
                    PreAlmacenaDocumentoTareaWorkflow = "Imposible determinar si el tipo de tarea Workflow es interna o externa en relación con el número de tarea Workflow (" & StructureDatosTareaWorkflow.FLUJO_INTERNO_WF & ")"
                    Exit Function
            End Select
            Result = AlmacenaDocumentoTareaWorkflow(1,
                                                    Gabinete,
                                                    Radicado,
                                                    RutaArchivo,
                                                    NombreRutaWorflow,
                                                    IdRutaWorkflow,
                                                    IdTareaWorkflow,
                                                    DescripcionTipo,
                                                    IdTipoChekLista,
                                                    2,
                                                    CDcamposAsignaAlmacenamiento,
                                                    "",
                                                    TipoCaso,
                                                    NombreClaseDocumento,
                                                    IdImagenAlamacenada,
                                                    EstructuraDatosImagen)
            If Result <> "YES" Then
                PreAlmacenaDocumentoTareaWorkflow = Result
                Exit Function
            End If
            If StructureDatosTareaWorkflow.ID_IMAGEN = 0 Then
                Result = Class_DAT_ADIC_TAR.AcualizaIdImagenTareaWorkflow(IdTareaWorkflow,
                                                                          StructureDatosTareaWorkflow.ID_GABINETE,
                                                                          NombreRutaWorflow,
                                                                          IdImagenAlamacenada)
                If Result <> "YES" Then
                    PreAlmacenaDocumentoTareaWorkflow = Result
                    Exit Function
                End If
            End If
            PreAlmacenaDocumentoTareaWorkflow = "YES"
            Exit Function
        Catch ex As Exception
            PreAlmacenaDocumentoTareaWorkflow = "Inconsistencia general funcion PreAlmacenaDocumentoTareaWorkflow " & ex.Message
        End Try
    End Function
    Function PreAlmacenaDocumentosRadicacion(ByVal DescripcionTipo As String,
                                             ByVal IdTipoChekLista As Integer,
                                             ByVal RutaArchivo As String,
                                             ByVal IdTareaWorkflow As Long,
                                             ByVal IdTipoTramite As Integer,
                                             ByVal NombreClaseDocumento As String,
                                             ByVal EvaluaActualizaImagenWorkflow As Integer,
                                             ByVal TipoAlmacenamiento As Integer,
                                             ByVal TipoAlmacen As Integer,
                                             ByVal DatosEnlaceScript As String,
                                             ByRef IdImagenAlamacenada As Integer,
                                             ByRef EstructuraDatosImagen As stru_datos_image_lista) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Alista los datos de pre almacenamiento para documentos que se adjuntan dsde el radicado
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'DescripcionTipo      : Representa la descripción de la tipologia documental
        'IdTipoChekLista      : Representa la identificación de la lista de chequeo
        'RutaArchivo          : Representa la ruta del archivo de almacenamiento
        'IdTareaWorkflow      : Representa la identificación de la tarea workflow
        'IdTipoTramite        : Representa la identificación del tipo tramite
        'NombreClaseDocumento : Representa la clase de documento
        'EvaluaActualizaImagenWorkflow : Deterina si actualiza el documento en workflow
        'TipoAlmacenamiento   : Representa el tipo de almacenamiento  0- Documento digitalizado
        '                       1- Documento adjunto  2- Documento adjunto desde dispositivo
        'TipoAlmacen          : Representa el tipo de almacenamiento para servidor de script
        'DatosEnlaceScript    : Reprentan los datos de servidor de script
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'IdImagenAlamacenada   : Retorna la identificación de la imagen almacenada
        'EstructuraDatosImagen : Retorna la estructura del documento almacenado
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-08-22
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Gabinete As String = ""
            Dim IdRutaWorkflow As Integer = HttpContext.Current.Session("Id_Ruta_Workflow")
            Dim NombreRutaWorflow As String = HttpContext.Current.Session("WF_RUTAWORKFLOW")
            Dim Class_ra_dig_config_digitalizacion As New Class_ra_dig_config_digitalizacion
            Dim StruConfigDigitalizacion As Stru_config_digitalizacion = Nothing
            Result = Class_ra_dig_config_digitalizacion.SolicitaDatosConfiguracionDigitalizacionPorTramite(IdTipoTramite,
                                                                                                           StruConfigDigitalizacion)
            If Result <> "YES" Then
                Return Result
            End If
            If StruConfigDigitalizacion.OBLIGA_LISTA_CHEQUEO = 1 And (IdTipoChekLista = -1 Or IdTipoChekLista = 0) Then
                Return "Por favor, indique el tipo documental correspondiente al documento que desea adjuntar.​"
            End If
            '//------------------Solicita estructuta tipo tramite---------------///
            Dim Class_tipo_doc_entrante As New Class_tipo_doc_entrante
            Dim CTipoDocEntrante As New CTipoDocEntrante
            If IdTipoTramite <> 0 And IdTipoTramite <> -1 Then
                Result = Class_tipo_doc_entrante.SolicitaEstructuraTramite(IdTipoTramite,
                                                                           CTipoDocEntrante)
                If Result <> "YES" Then
                    Return Result
                End If
            End If
            '//-----------------Solicita servicio de integración-----------------------//
            Dim Class_ra_ser_servicioIntegracion As New Class_ra_ser_servicioIntegracion
            Dim RaSerServicioInteracion As New RaSerServicioInteracion
            If Not CTipoDocEntrante Is Nothing Then
                Result = Class_ra_ser_servicioIntegracion.SolicitaEstructuraServicioIntegracion(CTipoDocEntrante.Id_ser_servicioIntegracion,
                                                                                                RaSerServicioInteracion)
                If Result <> "YES" Then
                    Return Result
                End If
            End If
            '//---------------Solicita la estructura de datos basico de la tarea workflow----------//
            Dim NombreServicioIntegracion As String = RaSerServicioInteracion.NombreServicio
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim StructureDatosTareaWorkflow As structure_datos_tarea_workflow = Nothing
            Result = Class_DAT_ADIC_TAR.SolicitaDatosEstructuraBasicaTareaWorkflow(NombreRutaWorflow,
                                                                                   IdTareaWorkflow,
                                                                                   StructureDatosTareaWorkflow)
            If Result <> "YES" Then
                Return Result
            End If
            If StructureDatosTareaWorkflow.ID_GABINETE = 0 Then
                Return "Imposible encontrar el identificador del gabinete asociado a la tarea de Workflow (" & IdTareaWorkflow & ")."
            End If
            '////------------------------Solicita nombre gabinete ----------------------///
            Dim Class_configuracion_gabinete As New Class_configuracion_gabinete
            Result = Class_configuracion_gabinete.SolicitanombreGabineteWorkflow(StructureDatosTareaWorkflow.ID_GABINETE,
                                                                                 Gabinete)
            If Result <> "YES" Then
                Return Result
            End If
            Dim TipoCaso As String = ""
            '////------------------------Asigna valores y campos para indice de gabinete----------------------///
            Dim CDcamposAsignaAlmacenamiento As New List(Of CDcamposAsignaAlmacenamiento)
            Dim Radicado As String = ""
            Dim CDParameterValoresCamposIndiceGabinete As New CDParameterValoresCamposIndiceGabinete
            Select Case StructureDatosTareaWorkflow.FLUJO_INTERNO_WF
                '///------------------------Caso flujo externo---------------///
                Case 2
                    If NombreServicioIntegracion = "INTEGRACIONSII" Then
                        TipoCaso = "SII"
                    End If
                    '-----Prametros SII Integración---------- "INTEGRACIONSII"
                    CDParameterValoresCamposIndiceGabinete.CDParmeterValoresCamposGabineteSII = New CDParmeterValoresCamposGabineteSII
                    CDParameterValoresCamposIndiceGabinete.CDParmeterValoresCamposGabineteSII.IdTareaWorkflow = IdTareaWorkflow
                    CDParameterValoresCamposIndiceGabinete.CDParmeterValoresCamposGabineteSII.IdRutaWorkflow = IdRutaWorkflow
                    CDParameterValoresCamposIndiceGabinete.CDParmeterValoresCamposGabineteSII.NombreRutaWorkflow = NombreRutaWorflow
                    CDParameterValoresCamposIndiceGabinete.CDParmeterValoresCamposGabineteSII.Gabinete = Gabinete
                    '-----Parameter default-------------------
                    CDParameterValoresCamposIndiceGabinete.CDParmeterValoresCamposGabineteDatAdicTar = New CDParmeterValoresCamposGabineteDatAdicTar
                    CDParameterValoresCamposIndiceGabinete.CDParmeterValoresCamposGabineteDatAdicTar.IdTareaWorkflow = IdTareaWorkflow
                    CDParameterValoresCamposIndiceGabinete.CDParmeterValoresCamposGabineteDatAdicTar.IdRutaWorkflow = IdRutaWorkflow
                    CDParameterValoresCamposIndiceGabinete.CDParmeterValoresCamposGabineteDatAdicTar.NombreRutaWorkflow = NombreRutaWorflow
                    CDParameterValoresCamposIndiceGabinete.CDParmeterValoresCamposGabineteDatAdicTar.Gabinete = Gabinete
                    Result = Class_ra_ser_servicioIntegracion.SolicitaDatosCamposIndiceGabineteIntegracion(CDParameterValoresCamposIndiceGabinete,
                                                                                                           NombreServicioIntegracion,
                                                                                                           Radicado,
                                                                                                           CDcamposAsignaAlmacenamiento)
                    If Result <> "YES" Then
                        Return Result
                    End If
                     '///------------------------Caso flujo Interno---------------///
                Case 1
                    '///-----Prametros relación gabinete plantilla radicación----------///
                    CDParameterValoresCamposIndiceGabinete.CDParmeterValoresCamposGabinete = New CDParmeterValoresCamposGabinete
                    CDParameterValoresCamposIndiceGabinete.CDParmeterValoresCamposGabinete.IdTareaWorkflow = IdTareaWorkflow
                    CDParameterValoresCamposIndiceGabinete.CDParmeterValoresCamposGabinete.IdRutaWorkflow = IdRutaWorkflow
                    CDParameterValoresCamposIndiceGabinete.CDParmeterValoresCamposGabinete.NombreRutaWorkflow = NombreRutaWorflow
                    CDParameterValoresCamposIndiceGabinete.CDParmeterValoresCamposGabinete.Gabinete = Gabinete
                    Dim ClassDaGabinete As New ClassDaGabinete
                    Result = ClassDaGabinete.SolicitaDatosCamposIndiceGabinete(CDParameterValoresCamposIndiceGabinete.CDParmeterValoresCamposGabinete,
                                                                               Radicado,
                                                                               CDcamposAsignaAlmacenamiento)
                    If Result <> "YES" Then
                        Return Result
                    End If
                Case Else
                    Return "Imposible determinar si el tipo de tarea Workflow es interna o externa en relación con el número de tarea Workflow (" & StructureDatosTareaWorkflow.FLUJO_INTERNO_WF & ")"
            End Select
            Result = AlmacenaDocumentosRadicacion(EvaluaActualizaImagenWorkflow,
                                                  Gabinete,
                                                  Radicado,
                                                  RutaArchivo,
                                                  NombreRutaWorflow,
                                                  IdRutaWorkflow,
                                                  IdTareaWorkflow,
                                                  DescripcionTipo,
                                                  IdTipoChekLista,
                                                  TipoAlmacenamiento,
                                                  CDcamposAsignaAlmacenamiento,
                                                  "",
                                                  TipoCaso,
                                                  NombreClaseDocumento,
                                                  TipoAlmacen,
                                                  DatosEnlaceScript,
                                                  StructureDatosTareaWorkflow.ID_IMAGEN,
                                                  IdImagenAlamacenada,
                                                  EstructuraDatosImagen)
            Return Result
        Catch ex As Exception
            Return "Inconsistencia general funcion AlmacenaDocumentosRadicacion " & ex.Message
        End Try
    End Function
    Function PreAlmacenaDocumentoProduccion(ByVal IdExpediente As Integer,
                                            ByVal RutaArchivoAlmacenar As String,
                                            ByVal DescripcionTipo As String,
                                            ByVal IdTipoLogia As Integer,
                                            ByVal NombreClaseFormatoDocumento As String,
                                            ByVal ObligaViculoExpeGabinete As Integer,
                                            ByVal FechaCarga As String,
                                            ByRef IdImagenAlamacenada As Integer,
                                            ByRef EstructuraDatosImagen As stru_datos_image_lista) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Asgina los datos de almacenamiento de un documento en la producción documental
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdExpediente                : Representa la identificación del expediente
        'RutaArchivoAlmacenar        : Representa la ruta del archivo a alamacenar
        'DescripcionTipo             : Representa la descripcion del tipo documental de trd
        'IdTipoLogia                 : Representa la identificación del tipo documental de trd
        'NombreClaseFormatoDocumento : Representa el nombre de formato de documento
        'ObligaViculoExpeGabinete    : Representa la obligatoriedad de relación indice documento expediente
        'FechaCarga                  : Rapresenta la fecha de carga de los archivos desde la interfaz
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'IdImagenAlamacenada         : Retorna la identificación la imagen almacenada
        'EstructuraDatosImagen       : Retorna la estructura para guardar el archivo en la interfaz
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-08-07
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim NombreGabinete As String = "PRODUCIONDOC"
            Dim ClassGaExpediente As New ClassGaExpediente
            Result = ClassGaExpediente.SolicitaGabineteProducionExpediente(IdExpediente,
                                                                           NombreGabinete)
            If Result <> "YES" Then
                PreAlmacenaDocumentoProduccion = Result
                Exit Function
            End If
            Dim IdGabineteDocuarchi As Integer = 0
            Dim Class_system1 As New Class_system1
            Result = Class_system1.SolicitaIdGabineteDocuarchi(NombreGabinete,
                                                               IdGabineteDocuarchi)
            If Result <> "YES" Then
                PreAlmacenaDocumentoProduccion = Result
                Exit Function
            End If
            '////------------------------Asigna valores y campos para indice de gabinete heredados del expediente----------------------///
            Dim CDcamposAsignaAlmacenamiento As New List(Of CDcamposAsignaAlmacenamiento)
            Dim Class_Ra_gabexp_relacion_index_gabinete_expediente As New Class_Ra_gabexp_relacion_index_gabinete_expediente
            Dim StruRelExpGabinete() As stru_rel_exp_gabinete = Nothing
            Result = Class_Ra_gabexp_relacion_index_gabinete_expediente.SolicitaValoresCampoExpedienteParaCampoIndiceGabinete(IdExpediente,
                                                                                                                              IdGabineteDocuarchi,
                                                                                                                              ObligaViculoExpeGabinete,
                                                                                                                              StruRelExpGabinete)
            If Result <> "YES" Then
                PreAlmacenaDocumentoProduccion = Result
                Exit Function
            End If
            Dim IlistCDcamposAsignaAlmacenamiento As New CDcamposAsignaAlmacenamiento
            If Not StruRelExpGabinete Is Nothing Then
                For i As Integer = 0 To StruRelExpGabinete.Length - 1
                    IlistCDcamposAsignaAlmacenamiento = New CDcamposAsignaAlmacenamiento
                    IlistCDcamposAsignaAlmacenamiento.NombreCampoGabinete = StruRelExpGabinete(i).CAMPO
                    IlistCDcamposAsignaAlmacenamiento.ValorCampoGabinete = StruRelExpGabinete(i).valor_campo_gabinete
                    CDcamposAsignaAlmacenamiento.Add(IlistCDcamposAsignaAlmacenamiento)
                Next
            End If
            '///------------------------Solicita la relación de campos fecha del gabinete para adjuntar la fecha de carga-------///
            Dim CDCamposFechaGabinetePro As New List(Of CDCamposFechaGabinetePro)
            Dim ClassRaCamposFechaGabineteProduccion As New ClassRaCamposFechaGabineteProduccion
            Result = ClassRaCamposFechaGabineteProduccion.SolicitaCamposFechaGabineteProduccion(IdGabineteDocuarchi,
                                                                                                CDCamposFechaGabinetePro)
            If Result <> "YES" Then
                PreAlmacenaDocumentoProduccion = Result
                Exit Function
            End If
            If Not CDCamposFechaGabinetePro Is Nothing Then
                For i As Integer = 0 To CDCamposFechaGabinetePro.Count - 1
                    If CDCamposFechaGabinetePro.Item(i).Tipo <> "DATE" Then
                        PreAlmacenaDocumentoProduccion = "No es posible actualizar el campo (" & CDCamposFechaGabinetePro.Item(i).Campo & ") con la fecha de carga, ya que no corresponde a un tipo de dato fecha."
                        Exit Function
                    End If
                    IlistCDcamposAsignaAlmacenamiento = New CDcamposAsignaAlmacenamiento
                    IlistCDcamposAsignaAlmacenamiento.NombreCampoGabinete = CDCamposFechaGabinetePro.Item(i).Campo
                    IlistCDcamposAsignaAlmacenamiento.ValorCampoGabinete = FechaCarga
                    CDcamposAsignaAlmacenamiento.Add(IlistCDcamposAsignaAlmacenamiento)
                    If CDcamposAsignaAlmacenamiento.Count > 0 Then
                        For k = 0 To CDcamposAsignaAlmacenamiento.Count - 1
                            If CDCamposFechaGabinetePro.Item(i).Campo = CDcamposAsignaAlmacenamiento.Item(k).NombreCampoGabinete And FechaCarga <> "" Then
                                CDcamposAsignaAlmacenamiento.Item(k).ValorCampoGabinete = FechaCarga
                            End If
                        Next
                    End If
                Next
            End If
            Dim TipoAlmacenamiento As Integer = 2
            Result = AlmacenamientoDocumentoProduccionDocumental(NombreGabinete,
                                                                 RutaArchivoAlmacenar,
                                                                 DescripcionTipo,
                                                                 IdExpediente,
                                                                 DescripcionTipo,
                                                                 IdTipoLogia,
                                                                 TipoAlmacenamiento,
                                                                 CDcamposAsignaAlmacenamiento,
                                                                 "",
                                                                 "",
                                                                 NombreClaseFormatoDocumento,
                                                                 IdImagenAlamacenada,
                                                                 EstructuraDatosImagen)
            PreAlmacenaDocumentoProduccion = Result
            Exit Function
        Catch ex As Exception
            PreAlmacenaDocumentoProduccion = "Inconsistencia general funcion PreAlmacenaDocumentoProduccion " & ex.Message
        End Try
    End Function
    Function AlmacenaDocumentoTareaWorkflow(ByVal ActivaGuardaValorRadicado As Integer,
                                            ByVal NombreGabinete As String,
                                            ByVal Radicado As String,
                                            ByVal RutaArchivoAlmacenar As String,
                                            ByVal NombreRutaWorkflow As String,
                                            ByVal IdRutaWorkflow As Integer,
                                            ByVal IdTareaWorkflow As Long,
                                            ByVal DescripcionTipo As String,
                                            ByVal IdTipoListaChek As Integer,
                                            ByVal TipoAlmacenamiento As Integer,
                                            ByVal CDcamposAsignaAlmacenamiento As List(Of CDcamposAsignaAlmacenamiento),
                                            ByVal ValorObjeto As Object,
                                            ByVal NombreCaso As String,
                                            ByVal NombreClaseFormatoDocumento As String,
                                            ByRef IdImagenAlamacenada As Integer,
                                            ByRef EstructuraDatosImagen As stru_datos_image_lista) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Prepara la estructura para el almacenamineto de documentos desde los eventos
        '          1. Enlace workflow
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'ActivaGuardaValorRadicado  : Representa la validación si guarda el valor del campo radicado 
        '                             en el gabinete
        'NombreGabinete             : Representa el nombre del gabinete de almacenamiento
        'Radicado                   : Representa el consecutivo de radicado
        'RutaArchivoAlmacenar       : Representa la ruta del archivo de almacenamiento
        'NombreRutaWorkflow         : Representa el nombre de la ruta workflow
        'IdRutaWorkflow             : Representa la identiifcación de la ruta workflow
        'IdTareaWorkflow            : Representa la identificación de la tarea worfflow
        'DescripcionTipo            : Representa la identificación literal del tipo documental
        'IdTipoListaChek            : Representa la identificación del tramite en la lista de chequeo
        'TipoAlmacenamiento         : Representa el tipo de almacenamiento determinado si elimina la imagen
        '                             valores 1-Elimina los documentos temprales de almacenaminento
        'CDcamposAsignaAlmacenamiento : Representa la estructura con los campos y valores de almacenami
        '                               ento para el documento de alamacenar, debe tener un el nombre de 
        '                               campos de gabinete valido de lo contrato no guarda los datos
        'ValorObjeto                : Representa un valor tipo objeto adaptable para futuras adaptaciones
        'NombreCaso                 : Representa el caso de alamacenamiento Poiblees valore
        '                             1- SII caso integración SII remplaza el valor SO del campo matricula
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'IdImagenAlamacenada  : Retorna la identificación de del documento alamcenado en el gabinete
        'EstructuraDatosImagen : Retorna la estrucutura de la imagen para el registro en la interfaz
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-07-10
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim ClassNeodynamic As New ClassNeodynamic
            Dim MatrizDocumentosFinal() As String
            Erase MatrizDocumentosFinal
            Dim File As New FileInfo(RutaArchivoAlmacenar)
            If File.Exists = False Then
                AlmacenaDocumentoTareaWorkflow = "Lamentamos informarle que no fue posible acceder al archivo solicitado (" & File.FullName & ")"
                Exit Function
            End If
            If UCase(File.Extension) = ".TIF" Then
                Result = ClassNeodynamic.Extraer_Documento_de_Multitif_fisico(RutaArchivoAlmacenar,
                                                                              MatrizDocumentosFinal,
                                                                              HttpContext.Current.Session.Item("WF_RUTA_TEMPO_FINAL") & "\")
                If Result <> "YES" Then
                    AlmacenaDocumentoTareaWorkflow = "Imposible extraer el documento Multi-TIFF. Se ha presentado la siguiente advertencia que impide continuar con el proceso (" & Result & ") Por favor, verifique el archivo y vuelva a intentarlo."
                    Exit Function
                End If
                If MatrizDocumentosFinal Is Nothing Then
                    AlmacenaDocumentoTareaWorkflow = "Imposible extraer los documentos del archivo Multi-TIFF. La operación fue detenida debido a la siguiente advertencia:  La matriz de documentos está vacía"
                    Exit Function
                End If
            Else
                ReDim Preserve MatrizDocumentosFinal(0)
                MatrizDocumentosFinal(0) = RutaArchivoAlmacenar
            End If
            Dim EstructuraDatosPrevioAlmacenamineto() As Datos_Almacenamiento = Nothing
            ReDim Preserve EstructuraDatosPrevioAlmacenamineto(0)
            Dim NombreCampoRadicadoGabinete As String = ""
            Dim Class_DETALLE_GABIENETE As New Class_DETALLE_GABIENETE
            Result = Class_DETALLE_GABIENETE.SolicitaNombreCampoRadicadoGabinete(NombreGabinete,
                                                                                 NombreCampoRadicadoGabinete)
            If Result <> "YES" Then
                AlmacenaDocumentoTareaWorkflow = Result
                Exit Function
            End If
            EstructuraDatosPrevioAlmacenamineto(0).nombre_campo = NombreCampoRadicadoGabinete
            If ActivaGuardaValorRadicado = 1 Then
                EstructuraDatosPrevioAlmacenamineto(0).valor_campo = Radicado
            Else
                EstructuraDatosPrevioAlmacenamineto(0).valor_campo = ""
            End If
            ReDim Preserve EstructuraDatosPrevioAlmacenamineto(1)
            EstructuraDatosPrevioAlmacenamineto(1).nombre_campo = "ENLASE"
            EstructuraDatosPrevioAlmacenamineto(1).valor_campo = Radicado
            Dim IdImagen As Integer = 0
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = Class_DAT_ADIC_TAR.SolicitaidImagenTareaworkflow(IdTareaWorkflow,
                                                                      NombreRutaWorkflow,
                                                                      IdImagen)
            If Result <> "YES" Then
                AlmacenaDocumentoTareaWorkflow = Result
                Exit Function
            End If
            Dim NombreTipoDocumento As String = ""
            Dim IdClaseFormatoDocumento As Integer = 0
            Dim FechaElaboracion As String = ""
            Dim ClassGaTipoDocumental As New ClassGaTipoDocumental
            Result = ClassGaTipoDocumental.SolicitaIdTipoFormatoDocumento(NombreClaseFormatoDocumento,
                                                                          IdClaseFormatoDocumento)
            If Result <> "YES" Then
                AlmacenaDocumentoTareaWorkflow = Result
                Exit Function
            End If
            NombreTipoDocumento = NombreClaseFormatoDocumento
            Dim date1al As String = Date.Today
            Result = ""
            Dim ClassGestionFechas As New ClassGestionFechas
            Result = ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
            If Result <> "YES" Then
                AlmacenaDocumentoTareaWorkflow = Result
                Exit Function
            End If
            FechaElaboracion = date1al
            '//-----------------------Asigna los datos de la matricula del Sistema SII----------//
            Select Case NombreCaso
                Case "SII"
                    If ValorObjeto <> "" Then
                        ValorObjeto = ValorObjeto.Replace("S0", "")
                    End If
            End Select
            '///------------Solicita los datos del expediente a relacionar el documento---------// 
            Dim EstructuraGestion As estructure_gestion = Nothing
            Dim ClassGaExpediente As New ClassGaExpediente
            Result = ClassGaExpediente.SolicitaEstructuraExpedienteDocumentoVinculante(EstructuraGestion,
                                                                                       NombreGabinete,
                                                                                       IdImagen,
                                                                                       IdTareaWorkflow,
                                                                                       Radicado,
                                                                                       NombreRutaWorkflow,
                                                                                       IdRutaWorkflow,
                                                                                       ValorObjeto)
            If Result <> "YES" Then
                AlmacenaDocumentoTareaWorkflow = Result
                Exit Function
            End If
            Dim ClassRaTipoDocSeries As New Class_ra_tipo_doc_series
            Dim EsctructuraExpediente() As expediente_conservacion = Nothing
            Dim CDclasificacionTipoDocumental As New CDclasificacionTipoDocumental
            CDclasificacionTipoDocumental.DescripcionTipoDocumento = ""
            CDclasificacionTipoDocumental.IdSerie = 0
            CDclasificacionTipoDocumental.IdArea = 0
            CDclasificacionTipoDocumental.NombreSerie = ""
            CDclasificacionTipoDocumental.NombreSubSerie = ""
            CDclasificacionTipoDocumental.NombreArea = ""
            '//------------Asigna los datos de clasificación documental heredado del expediente----------////
            If EstructuraGestion.ID_EXPEDIENTE <> 0 Then
                Result = ClassGaExpediente.SolicitaDatosEstructuraExpediente(EstructuraGestion.ID_EXPEDIENTE,
                                                                             EsctructuraExpediente)
                If Result <> "YES" Then
                    AlmacenaDocumentoTareaWorkflow = Result
                    Exit Function
                End If
                Dim StruTipoListaChequeo As stru_tipo_lista_chequeo = Nothing
                Dim ra_dig_tipos_docum_lista_chequeo As New ra_dig_tipos_docum_lista_chequeo
                If IdTipoListaChek <> 0 And IdTipoListaChek <> -1 Then
                    Result = ra_dig_tipos_docum_lista_chequeo.SolicitaDatosTipoDocumentalListaChequeo(IdTipoListaChek,
                                                                                                      StruTipoListaChequeo)
                    If Result <> "YES" Then
                        AlmacenaDocumentoTareaWorkflow = Result
                        Exit Function
                    End If
                    If StruTipoListaChequeo.subseries_documentales_Id_SubSeries <> 0 Then
                        CDclasificacionTipoDocumental.IdTipoDocumento = StruTipoListaChequeo.tipos_doc_subseries_Id_Tipos_Doc_SubSerie
                    Else
                        CDclasificacionTipoDocumental.IdTipoDocumento = StruTipoListaChequeo.tipo_doc_series_Id_Tipo_Doc_Series
                    End If
                    Result = ClassRaTipoDocSeries.SolicitaNombreTipoDocumentalSerieSubSerie(CDclasificacionTipoDocumental.IdTipoDocumento,
                                                                                            DescripcionTipo)
                    If Result <> "YES" Then
                        AlmacenaDocumentoTareaWorkflow = Result
                        Exit Function
                    End If
                End If
                CDclasificacionTipoDocumental.DescripcionTipoDocumento = DescripcionTipo
                CDclasificacionTipoDocumental.IdSerie = EsctructuraExpediente(0).CODIGO_SERIE
                CDclasificacionTipoDocumental.IdSubSerie = EsctructuraExpediente(0).CODIGO_SUBSERIE
                CDclasificacionTipoDocumental.IdArea = EsctructuraExpediente(0).CODIGO_AREA_TRD
                CDclasificacionTipoDocumental.NombreSerie = EsctructuraExpediente(0).NOMBRE_SERIE
                CDclasificacionTipoDocumental.NombreSubSerie = EsctructuraExpediente(0).NOMBRE_SUBSERIE
                CDclasificacionTipoDocumental.NombreArea = EsctructuraExpediente(0).NOMBRE_AREA
            End If
            '//------------Asigna los datos de clasificación documental heredado de la lista de chequeo----------////
            If EstructuraGestion.ID_EXPEDIENTE = 0 And IdTipoListaChek <> 0 And IdTipoListaChek <> -1 Then
                Result = ClassGaTipoDocumental.SolicitaEstructuraClasificacionTipoDocumento(IdTipoListaChek,
                                                                                            CDclasificacionTipoDocumental)
                If Result <> "YES" Then
                    AlmacenaDocumentoTareaWorkflow = Result
                    Exit Function
                End If
            End If
            '//--------Asigna datos gestión al documento----------------////
            Dim EstructuraGestionAlmacenamiento As estructure_gestion = Nothing
            EstructuraGestionAlmacenamiento.CLASE_DOCUMENTO = CDclasificacionTipoDocumental.DescripcionTipoDocumento
            EstructuraGestionAlmacenamiento.ID_AREA = CDclasificacionTipoDocumental.IdArea
            EstructuraGestionAlmacenamiento.ID_CLASE_DOCUMENTO = IdClaseFormatoDocumento
            EstructuraGestionAlmacenamiento.ID_SERIE = CDclasificacionTipoDocumental.IdSerie
            EstructuraGestionAlmacenamiento.ID_SUB_SERIE = CDclasificacionTipoDocumental.IdSubSerie
            EstructuraGestionAlmacenamiento.ID_TIPO_UNIDAD_CONSERVACION = 0
            EstructuraGestionAlmacenamiento.ID_TIPODOCUMENTO = CDclasificacionTipoDocumental.IdTipoDocumento
            EstructuraGestionAlmacenamiento.ID_UNIDAD_CONSERVACION = 0
            EstructuraGestionAlmacenamiento.ID_USUARIO_GESTION = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
            EstructuraGestionAlmacenamiento.TIPO_UNIDAD_DOCUMENTAL = 0
            EstructuraGestionAlmacenamiento.UNIDAD_CONSERVACION = ""
            EstructuraGestionAlmacenamiento.FECHA_ELABORACION = FechaElaboracion
            EstructuraGestionAlmacenamiento.TIPODOCUMENTO = CDclasificacionTipoDocumental.DescripcionTipoDocumento
            EstructuraGestionAlmacenamiento.NOMBRE_SERIE = CDclasificacionTipoDocumental.NombreSerie
            EstructuraGestion.NOMBRE_SUB_SERIE = CDclasificacionTipoDocumental.NombreSubSerie
            ReDim Preserve EstructuraDatosPrevioAlmacenamineto(2)
            EstructuraDatosPrevioAlmacenamineto(2).nombre_campo = "EXPEDIENTE"
            EstructuraDatosPrevioAlmacenamineto(2).valor_campo = EstructuraGestion.EXPEDIENTE
            ReDim Preserve EstructuraDatosPrevioAlmacenamineto(3)
            EstructuraDatosPrevioAlmacenamineto(3).nombre_campo = "CLASEDOCUMENTO"
            EstructuraDatosPrevioAlmacenamineto(3).valor_campo = NombreTipoDocumento
            ReDim Preserve EstructuraDatosPrevioAlmacenamineto(4)
            EstructuraDatosPrevioAlmacenamineto(4).nombre_campo = "FECHAELABORACION"
            EstructuraDatosPrevioAlmacenamineto(4).valor_campo = FechaElaboracion
            ReDim Preserve EstructuraDatosPrevioAlmacenamineto(5)
            EstructuraDatosPrevioAlmacenamineto(5).nombre_campo = "TIPODOCUMENTO"
            EstructuraDatosPrevioAlmacenamineto(5).valor_campo = CDclasificacionTipoDocumental.DescripcionTipoDocumento
            ReDim Preserve EstructuraDatosPrevioAlmacenamineto(6)
            EstructuraDatosPrevioAlmacenamineto(6).nombre_campo = "NOMBRESERIE"
            EstructuraDatosPrevioAlmacenamineto(6).valor_campo = CDclasificacionTipoDocumental.NombreSerie
            ReDim Preserve EstructuraDatosPrevioAlmacenamineto(7)
            EstructuraDatosPrevioAlmacenamineto(7).nombre_campo = "NOMBRESUBSERIE"
            EstructuraDatosPrevioAlmacenamineto(7).valor_campo = CDclasificacionTipoDocumental.NombreSubSerie
            Dim Icount As Integer = 7
            '///---------------Asignamos los valores de campos de gabinete a la estructura -------------/////
            If CDcamposAsignaAlmacenamiento.Count > 0 Then
                For i As Integer = 0 To CDcamposAsignaAlmacenamiento.Count - 1
                    If CDcamposAsignaAlmacenamiento.Item(i).ValorCampoGabinete <> "" Then
                        Icount += 1
                        ReDim Preserve EstructuraDatosPrevioAlmacenamineto(Icount)
                        EstructuraDatosPrevioAlmacenamineto(Icount).valor_campo = CDcamposAsignaAlmacenamiento.Item(i).ValorCampoGabinete
                        EstructuraDatosPrevioAlmacenamineto(Icount).nombre_campo = CDcamposAsignaAlmacenamiento.Item(i).NombreCampoGabinete
                    End If
                Next
            End If
            '----------------------------------------------
            'Genera la matriz de datos de almacenamiento
            '----------------------------------------------
            Dim EstructuraDatosAlmacenamineto() As String
            Erase EstructuraDatosAlmacenamineto
            Result = Class_DETALLE_GABIENETE.SolicitaValoresCamposDocumentoGabinete(EstructuraDatosAlmacenamineto,
                                                                                    NombreGabinete,
                                                                                    EstructuraDatosPrevioAlmacenamineto)
            If Result <> "YES" Then
                AlmacenaDocumentoTareaWorkflow = Result
                Exit Function
            End If
            Dim TipoArchivoDocuarchi As Object = 0
            Dim Class_da_extension As New Class_da_extension
            Result = Class_da_extension.SolicitaTipoArchivoDocuarchiExtension(File.Extension,
                                                                              TipoArchivoDocuarchi)
            If Result <> "YES" Then
                AlmacenaDocumentoTareaWorkflow = Result
                Exit Function
            End If
            TipoArchivoDocuarchi = CInt(TipoArchivoDocuarchi)
            '-----------------------------------------------
            'Almacena documento en la base docuarchi.net
            '-----------------------------------------------
            Dim UserWf As String = "WF-" & HttpContext.Current.Session.Item("Login_Usuario_Workfow")
            Result = Me.Almacenamiento("", "", NombreGabinete, 0, EstructuraDatosAlmacenamineto, 2,
            MatrizDocumentosFinal.Length, TipoArchivoDocuarchi, MatrizDocumentosFinal, 0, IdImagenAlamacenada,
            TipoArchivoDocuarchi, HttpContext.Current.Session.Item("GA_IDEMPRESA"),
            HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), EstructuraGestionAlmacenamiento.ID_AREA,
            EstructuraGestionAlmacenamiento.ID_SERIE, EstructuraGestionAlmacenamiento.ID_SUB_SERIE,
            EstructuraGestionAlmacenamiento.ID_TIPODOCUMENTO, EstructuraGestionAlmacenamiento.ID_EXPEDIENTE,
            EstructuraGestionAlmacenamiento.ID_TIPO_EXPEDIENTE,
            EstructuraGestionAlmacenamiento.ID_UNIDAD_CONSERVACION, EstructuraGestionAlmacenamiento.ID_TIPO_UNIDAD_CONSERVACION,
            EstructuraGestionAlmacenamiento.ID_CLASE_DOCUMENTO, EstructuraGestionAlmacenamiento.EXPEDIENTE, EstructuraGestionAlmacenamiento.NOMBRE_SERIE,
            EstructuraGestionAlmacenamiento.NOMBRE_SUB_SERIE, EstructuraGestionAlmacenamiento.TIPODOCUMENTO, EstructuraGestionAlmacenamiento.UNIDAD_CONSERVACION,
            EstructuraGestionAlmacenamiento.CLASE_DOCUMENTO, EstructuraGestionAlmacenamiento.FECHA_ELABORACION, Radicado, "RAD-" & Radicado, 0, 0, 0, IdTareaWorkflow,
            IdRutaWorkflow)
            If Result <> "YES" Then
                AlmacenaDocumentoTareaWorkflow = Result
                Exit Function
            End If
            Dim attrNodeGru1 As New TreeNode
            Dim val_documento As String = EstructuraGestionAlmacenamiento.TIPODOCUMENTO
            If val_documento = "" Then
                val_documento = "D-" & IdImagenAlamacenada
            End If
            Dim DescripcionTipoDocumento As String = ""
            If CDclasificacionTipoDocumental.DescripcionTipoDocumento = "" Then
                DescripcionTipoDocumento = val_documento
            End If
            Dim classgabinete As New ClassDaGabinete
            Dim icono As String = ""
            classgabinete.SolicitaIconoImageFownt(TipoArchivoDocuarchi.ToString,
                                                  icono)
            EstructuraDatosImagen.nombre_gabinete = NombreGabinete
            EstructuraDatosImagen.id_imagen = IdImagenAlamacenada
            EstructuraDatosImagen.radicado = Radicado
            EstructuraDatosImagen.tipodocumental = DescripcionTipoDocumento
            EstructuraDatosImagen.notipodocumento = val_documento
            EstructuraDatosImagen.extension = UCase(File.Extension)
            EstructuraDatosImagen.icono_icono_awe_some = icono
            '-----------------------------------------------
            'Elimina los documentos almacenados  
            '-----------------------------------------------
            For k As Integer = 0 To MatrizDocumentosFinal.Length - 1
                If System.IO.File.Exists(MatrizDocumentosFinal(k)) = True Then
                    System.IO.File.Delete(MatrizDocumentosFinal(k))
                End If
            Next
            If TipoAlmacenamiento <> 0 Then
                If System.IO.File.Exists(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA")) = True Then
                    Kill(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA"))
                End If
                HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA") = ""
                HttpContext.Current.Session.Item("WF_ERROR_RESPUESTA") = ""
            End If
            AlmacenaDocumentoTareaWorkflow = "YES"
            Exit Function
        Catch ex As Exception
            AlmacenaDocumentoTareaWorkflow = "Inconsistencia general función AlmacenaDocumentoTareaWorkflow " & ex.Message
        End Try
    End Function
    Function AlmacenamientoDocumentoProduccionDocumental(ByVal NombreGabinete As String,
                                                         ByVal RutaArchivoAlmacenar As String,
                                                         ByVal NombreDocumento As String,
                                                         ByVal IdExpediente As Integer,
                                                         ByVal DescripcionTipo As String,
                                                         ByVal IdTipoLogia As Integer,
                                                         ByVal TipoAlmacenamiento As Integer,
                                                         ByVal CDcamposAsignaAlmacenamiento As List(Of CDcamposAsignaAlmacenamiento),
                                                         ByVal ValorObjeto As Object,
                                                         ByVal NombreCaso As String,
                                                         ByVal NombreClaseFormatoDocumento As String,
                                                         ByRef IdImagenAlamacenada As Integer,
                                                         ByRef EstructuraDatosImagen As stru_datos_image_lista) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Almacenamiento de un documento en la producción documental
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdExpediente                 : Representa la identificación del expediente
        'RutaArchivoAlmacenar         : Representa la ruta del archivo a alamacenar
        'DescripcionTipo              : Representa la descripcion del tipo documental de trd
        'IdTipoLogia                  : Representa la identificación del tipo documental de trd
        'NombreClaseFormatoDocumento  : Representa el nombre de formato de documento
        'CDcamposAsignaAlmacenamiento : Representa la estructura de campos de almacenamiento
        'ValorObjeto                  : 
        'NombreCaso                   :
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'IdImagenAlamacenada         : Retorna la identificación la imagen almacenada
        'EstructuraDatosImagen       : Retorna la estructura para guardar el archivo en la interfaz
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-08-07
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim ClassNeodynamic As New ClassNeodynamic
            Dim MatrizDocumentosFinal() As String
            Erase MatrizDocumentosFinal
            Dim File As New FileInfo(RutaArchivoAlmacenar)
            Dim Result As String = ""
            If File.Exists = False Then
                AlmacenamientoDocumentoProduccionDocumental = "Lamentamos informarle que no fue posible acceder al archivo solicitado (" & File.FullName & ")"
                Exit Function
            End If
            If UCase(File.Extension) = ".TIF" Then
                Result = ClassNeodynamic.Extraer_Documento_de_Multitif_fisico(RutaArchivoAlmacenar,
                                                                              MatrizDocumentosFinal,
                                                                              HttpContext.Current.Session.Item("WF_RUTA_TEMPO_FINAL") & "\")
                If Result <> "YES" Then
                    AlmacenamientoDocumentoProduccionDocumental = "Imposible extraer el documento Multi-TIFF. Se ha presentado la siguiente advertencia que impide continuar con el proceso (" & Result & ") Por favor, verifique el archivo y vuelva a intentarlo."
                    Exit Function
                End If
                If MatrizDocumentosFinal Is Nothing Then
                    AlmacenamientoDocumentoProduccionDocumental = "Imposible extraer los documentos del archivo Multi-TIFF. La operación fue detenida debido a la siguiente advertencia:  La matriz de documentos está vacía"
                    Exit Function
                End If
            Else
                ReDim Preserve MatrizDocumentosFinal(0)
                MatrizDocumentosFinal(0) = RutaArchivoAlmacenar
            End If
            Dim Class_DETALLE_GABIENETE As New Class_DETALLE_GABIENETE
            Dim EstructuraDatosPrevioAlmacenamineto() As Datos_Almacenamiento = Nothing
            ReDim Preserve EstructuraDatosPrevioAlmacenamineto(0)
            Dim NombreCampoRadicadoGabinete As String = ""
            ReDim Preserve EstructuraDatosPrevioAlmacenamineto(0)
            EstructuraDatosPrevioAlmacenamineto(0).nombre_campo = "NOMBRE"
            EstructuraDatosPrevioAlmacenamineto(0).valor_campo = NombreDocumento
            Dim ClassGaExpediente As New ClassGaExpediente
            Dim EsctructuraExpediente() As expediente_conservacion = Nothing
            Dim CDclasificacionTipoDocumental As New CDclasificacionTipoDocumental
            CDclasificacionTipoDocumental.DescripcionTipoDocumento = ""
            CDclasificacionTipoDocumental.IdSerie = 0
            CDclasificacionTipoDocumental.IdArea = 0
            CDclasificacionTipoDocumental.NombreSerie = ""
            CDclasificacionTipoDocumental.NombreSubSerie = ""
            CDclasificacionTipoDocumental.NombreArea = ""
            CDclasificacionTipoDocumental.IdTipoDocumento = 0
            Result = ClassGaExpediente.SolicitaDatosEstructuraExpediente(IdExpediente,
                                                                         EsctructuraExpediente)
            If Result <> "YES" Then
                AlmacenamientoDocumentoProduccionDocumental = Result
                Exit Function
            End If
            CDclasificacionTipoDocumental.DescripcionTipoDocumento = DescripcionTipo
            CDclasificacionTipoDocumental.IdSerie = EsctructuraExpediente(0).CODIGO_SERIE
            CDclasificacionTipoDocumental.IdSubSerie = EsctructuraExpediente(0).CODIGO_SUBSERIE
            CDclasificacionTipoDocumental.IdArea = EsctructuraExpediente(0).CODIGO_AREA_TRD
            CDclasificacionTipoDocumental.NombreSerie = EsctructuraExpediente(0).NOMBRE_SERIE
            CDclasificacionTipoDocumental.NombreSubSerie = EsctructuraExpediente(0).NOMBRE_SUBSERIE
            CDclasificacionTipoDocumental.NombreArea = EsctructuraExpediente(0).NOMBRE_AREA
            CDclasificacionTipoDocumental.IdTipoDocumento = IdTipoLogia
            Dim NombreTipoDocumento As String = ""
            Dim IdClaseFormatoDocumento As Integer = 0
            Dim FechaElaboracion As String = ""
            Dim ClassGaTipoDocumental As New ClassGaTipoDocumental
            Result = ClassGaTipoDocumental.SolicitaIdTipoFormatoDocumento(NombreClaseFormatoDocumento,
                                                                          IdClaseFormatoDocumento)
            If Result <> "YES" Then
                AlmacenamientoDocumentoProduccionDocumental = Result
                Exit Function
            End If
            NombreTipoDocumento = NombreClaseFormatoDocumento
            Dim DateTyme As String = Date.Today
            Result = ""
            Dim ClassGestionFechas As New ClassGestionFechas
            Result = ClassGestionFechas.FormateaFechaAlmacenamiento(DateTyme)
            If Result <> "YES" Then
                AlmacenamientoDocumentoProduccionDocumental = Result
                Exit Function
            End If
            FechaElaboracion = DateTyme
            Dim Class_system1 As New Class_system1

            Dim EstructuraGestionAlmacenamiento As estructure_gestion = Nothing
            EstructuraGestionAlmacenamiento.CLASE_DOCUMENTO = NombreClaseFormatoDocumento
            EstructuraGestionAlmacenamiento.ID_AREA = CDclasificacionTipoDocumental.IdArea
            EstructuraGestionAlmacenamiento.ID_CLASE_DOCUMENTO = IdClaseFormatoDocumento
            EstructuraGestionAlmacenamiento.ID_SERIE = CDclasificacionTipoDocumental.IdSerie
            EstructuraGestionAlmacenamiento.ID_SUB_SERIE = CDclasificacionTipoDocumental.IdSubSerie
            EstructuraGestionAlmacenamiento.ID_TIPO_UNIDAD_CONSERVACION = 0
            EstructuraGestionAlmacenamiento.ID_TIPODOCUMENTO = CDclasificacionTipoDocumental.IdTipoDocumento
            EstructuraGestionAlmacenamiento.ID_UNIDAD_CONSERVACION = 0
            EstructuraGestionAlmacenamiento.ID_USUARIO_GESTION = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
            EstructuraGestionAlmacenamiento.TIPO_UNIDAD_DOCUMENTAL = 0
            EstructuraGestionAlmacenamiento.UNIDAD_CONSERVACION = ""
            EstructuraGestionAlmacenamiento.FECHA_ELABORACION = FechaElaboracion
            EstructuraGestionAlmacenamiento.TIPODOCUMENTO = CDclasificacionTipoDocumental.DescripcionTipoDocumento
            EstructuraGestionAlmacenamiento.NOMBRE_SERIE = CDclasificacionTipoDocumental.NombreSerie
            EstructuraGestionAlmacenamiento.ID_EXPEDIENTE = IdExpediente

            ReDim Preserve EstructuraDatosPrevioAlmacenamineto(1)
            EstructuraDatosPrevioAlmacenamineto(1).nombre_campo = "EXPEDIENTE"
            EstructuraDatosPrevioAlmacenamineto(1).valor_campo = EsctructuraExpediente(0).CODIGO_UNICO
            ReDim Preserve EstructuraDatosPrevioAlmacenamineto(2)
            EstructuraDatosPrevioAlmacenamineto(2).nombre_campo = "CLASEDOCUMENTO"
            EstructuraDatosPrevioAlmacenamineto(2).valor_campo = NombreTipoDocumento
            ReDim Preserve EstructuraDatosPrevioAlmacenamineto(3)
            EstructuraDatosPrevioAlmacenamineto(3).nombre_campo = "FECHAELABORACION"
            EstructuraDatosPrevioAlmacenamineto(3).valor_campo = FechaElaboracion
            ReDim Preserve EstructuraDatosPrevioAlmacenamineto(4)
            EstructuraDatosPrevioAlmacenamineto(4).nombre_campo = "TIPODOCUMENTO"
            EstructuraDatosPrevioAlmacenamineto(4).valor_campo = CDclasificacionTipoDocumental.DescripcionTipoDocumento
            ReDim Preserve EstructuraDatosPrevioAlmacenamineto(5)
            EstructuraDatosPrevioAlmacenamineto(5).nombre_campo = "NOMBRESERIE"
            EstructuraDatosPrevioAlmacenamineto(5).valor_campo = CDclasificacionTipoDocumental.NombreSerie
            ReDim Preserve EstructuraDatosPrevioAlmacenamineto(6)
            EstructuraDatosPrevioAlmacenamineto(6).nombre_campo = "NOMBRESUBSERIE"
            EstructuraDatosPrevioAlmacenamineto(6).valor_campo = CDclasificacionTipoDocumental.NombreSubSerie
            Dim Icount As Integer = 6
            '///---------------Asignamos los valores externos para estructura del indice de gabinetes-------------/////
            If CDcamposAsignaAlmacenamiento.Count > 0 Then
                For i As Integer = 0 To CDcamposAsignaAlmacenamiento.Count - 1
                    If CDcamposAsignaAlmacenamiento.Item(i).ValorCampoGabinete <> "" Then
                        Icount += 1
                        ReDim Preserve EstructuraDatosPrevioAlmacenamineto(Icount)
                        EstructuraDatosPrevioAlmacenamineto(Icount).valor_campo = CDcamposAsignaAlmacenamiento.Item(i).ValorCampoGabinete
                        EstructuraDatosPrevioAlmacenamineto(Icount).nombre_campo = CDcamposAsignaAlmacenamiento.Item(i).NombreCampoGabinete
                    End If
                Next
            End If
            '----------------------------------------------
            'Genera la matriz de datos de almacenamiento
            '----------------------------------------------
            Dim EstructuraDatosAlmacenamineto() As String
            Erase EstructuraDatosAlmacenamineto
            Result = Class_DETALLE_GABIENETE.SolicitaValoresCamposDocumentoGabinete(EstructuraDatosAlmacenamineto,
                                                                                    NombreGabinete,
                                                                                    EstructuraDatosPrevioAlmacenamineto)
            If Result <> "YES" Then
                AlmacenamientoDocumentoProduccionDocumental = Result
                Exit Function
            End If
            Dim TipoArchivoDocuarchi As String = ""
            Dim Class_da_extension As New Class_da_extension
            Result = Class_da_extension.SolicitaTipoArchivoDocuarchiExtension(File.Extension,
                                                                              TipoArchivoDocuarchi)
            If Result <> "YES" Then
                AlmacenamientoDocumentoProduccionDocumental = Result
                Exit Function
            End If
            '-----------------------------------------------
            'Almacena documento en la base docuarchi.net
            '-----------------------------------------------
            Dim IdRegistro As Integer = 0
            Dim EstadoFirmaDigital As Integer = 0
            Dim NombreArchivo As String = File.Name.Replace("/", "-")
            Result = Me.Almacenamiento("", "", NombreGabinete, 0, EstructuraDatosAlmacenamineto, 2,
            MatrizDocumentosFinal.Length, TipoArchivoDocuarchi, MatrizDocumentosFinal, 0, IdImagenAlamacenada,
            TipoArchivoDocuarchi, HttpContext.Current.Session.Item("GA_IDEMPRESA"),
            HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), EstructuraGestionAlmacenamiento.ID_AREA,
            EstructuraGestionAlmacenamiento.ID_SERIE, EstructuraGestionAlmacenamiento.ID_SUB_SERIE,
            EstructuraGestionAlmacenamiento.ID_TIPODOCUMENTO, EstructuraGestionAlmacenamiento.ID_EXPEDIENTE,
            EstructuraGestionAlmacenamiento.ID_TIPO_EXPEDIENTE,
            EstructuraGestionAlmacenamiento.ID_UNIDAD_CONSERVACION, EstructuraGestionAlmacenamiento.ID_TIPO_UNIDAD_CONSERVACION,
            EstructuraGestionAlmacenamiento.ID_CLASE_DOCUMENTO, EstructuraGestionAlmacenamiento.EXPEDIENTE, EstructuraGestionAlmacenamiento.NOMBRE_SERIE,
            EstructuraGestionAlmacenamiento.NOMBRE_SUB_SERIE, EstructuraGestionAlmacenamiento.TIPODOCUMENTO, EstructuraGestionAlmacenamiento.UNIDAD_CONSERVACION,
            EstructuraGestionAlmacenamiento.CLASE_DOCUMENTO, EstructuraGestionAlmacenamiento.FECHA_ELABORACION, "", NombreArchivo, IdRegistro, 1, EstadoFirmaDigital, 0,
            0)
            If Result <> "YES" Then
                AlmacenamientoDocumentoProduccionDocumental = Result
                Exit Function
            End If
            Dim classgabinete As New ClassDaGabinete
            Dim IconoFownt As String = ""
            classgabinete.SolicitaIconoImageFownt(TipoArchivoDocuarchi.ToString,
                                                  IconoFownt)
            '-----------------------------------------------
            'Elimina los documentos almacenados  
            '-----------------------------------------------
            Dim FechaTempo As String = ""
            ClassGestionFechas.FormateaFechaTimeDbDefault(DateTyme,
                                                          FechaTempo)
            FechaTempo = Left(FechaTempo, 10)
            EstructuraDatosImagen.nombre_gabinete = NombreGabinete
            EstructuraDatosImagen.tipodocumental = DescripcionTipo
            EstructuraDatosImagen.id_registro = IdRegistro
            EstructuraDatosImagen.fecha = FechaTempo
            EstructuraDatosImagen.aleas = EsctructuraExpediente(0).ALEAS_EXPEDIENTE
            EstructuraDatosImagen.nombre_archivo = NombreArchivo.Replace("|", "")
            EstructuraDatosImagen.icono_icono_awe_some = IconoFownt
            EstructuraDatosImagen.estado_firma_digital = EstadoFirmaDigital
            EstructuraDatosImagen.id_imagen = IdImagenAlamacenada
            For k As Integer = 0 To MatrizDocumentosFinal.Length - 1
                If System.IO.File.Exists(MatrizDocumentosFinal(k)) = True Then
                    System.IO.File.Delete(MatrizDocumentosFinal(k))
                End If
            Next
            If TipoAlmacenamiento <> 0 Then
                If System.IO.File.Exists(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA")) = True Then
                    Kill(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA"))
                End If
                HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA") = ""
                HttpContext.Current.Session.Item("WF_ERROR_RESPUESTA") = ""
            End If
            AlmacenamientoDocumentoProduccionDocumental = "YES"
            Exit Function
        Catch ex As Exception
            AlmacenamientoDocumentoProduccionDocumental = "Incosistencia general funcion AlmacenamientoDocumentoProduccionDocumental " & ex.Message
        End Try
    End Function
    Function AlmacenaDocumentosRadicacion(ByVal EvaluaActualizaImagenWorkflow As Integer,
                                          ByVal GabineteRadicado As String,
                                          ByVal ConsecutivoRadicado As String,
                                          ByVal RutaArchivoAlmacenar As String,
                                          ByVal NombreRutaWorkflow As String,
                                          ByVal IdRutaWorkflow As Integer,
                                          ByVal IdTareaWorkflow As Long,
                                          ByVal DescripcionTipo As String,
                                          ByVal IdTipoListaChek As Integer,
                                          ByVal TipoAlmacenamiento As Integer,
                                          ByVal CDcamposAsignaAlmacenamiento As List(Of CDcamposAsignaAlmacenamiento),
                                          ByVal ValorObjeto As Object,
                                          ByVal NombreCaso As String,
                                          ByVal NombreClaseFormatoDocumento As String,
                                          ByVal TipoAlmacen As Integer,
                                          ByVal DatosEnlaceScript As String,
                                          ByVal IdImagenWorkflow As Integer,
                                          ByRef IdImagenAlamacenada As Integer,
                                          ByRef EstructuraDatosImagen As stru_datos_image_lista) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Prepara la estructura para el almacenamineto de documentos desde los eventos
        '          1. Ventanilla radicacion
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'EvaluaActualizaImagenWorkflow  : Representa la validación si guarda el valor del campo radicado 
        '                             en el gabinete
        'GabineteRadicado             : Representa el nombre del gabinete de almacenamiento
        'ConsecutivoRadicado                   : Representa el consecutivo de radicado
        'RutaArchivoAlmacenar       : Representa la ruta del archivo de almacenamiento
        'NombreRutaWorkflow         : Representa el nombre de la ruta workflow
        'IdRutaWorkflow             : Representa la identiifcación de la ruta workflow
        'IdTareaWorkflow            : Representa la identificación de la tarea worfflow
        'DescripcionTipo            : Representa la identificación literal del tipo documental
        'IdTipoListaChek            : Representa la identificación del tramite en la lista de chequeo
        'TipoAlmacenamiento         : Representa el tipo de almacenamiento determinado 
        '                             valores 0-Para documento digitalizados   1-Para documentos adjuntos
        '                             para añadir   2- Para documentos que se adjuntan desde dispositivos
        'CDcamposAsignaAlmacenamiento : Representa la estructura con los campos y valores de almacenami
        '                               ento para el documento de alamacenar, debe tener un el nombre de 
        '                               campos de gabinete valido de lo contrato no guarda los datos
        'ValorObjeto                : Representa un valor tipo objeto adaptable para futuras adaptaciones
        'NombreCaso                 : Representa el caso de alamacenamiento Poiblees valore
        '                             1- SII caso integración SII remplaza el valor SO del campo matricula
        'TipoAlmacen                : Representa el tipo de almacenamiento para script de enlace de 
        '                             documentos
        'DatosEnlaceScript          : Representan los datos del servidor de script
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'IdImagenAlamacenada  : Retorna la identificación de del documento alamcenado en el gabinete
        'EstructuraDatosImagen : Retorna la estrucutura de la imagen para el registro en la interfaz
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-08-22
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim ClassWorkflowDigitalizacion As New ClassWorkflowDigitalizacion
            Dim MatrizDocumentosFinal() As String
            Erase MatrizDocumentosFinal
            '///-----------------------Solicita estructura de archivos desde documentos digitalizados------------/////
            If TipoAlmacenamiento = 0 Then
                Result = ClassWorkflowDigitalizacion.SolicitaMatrizDocumentosDigitalizados(IdTareaWorkflow,
                                                                                           HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ESCANER"),
                                                                                           MatrizDocumentosFinal)
                If Result <> "YES" Then
                    Return Result
                End If
            End If
            '///-----------------------Solicita estructura de archivos para adjuntar------------/////
            If TipoAlmacenamiento = 1 Then
                Dim Refclas_almacenamiento As New ClassAlmacenamiento
                Result = Refclas_almacenamiento.Retorna_matriz_documentos_adjuntos_workflow(MatrizDocumentosFinal)
                If Result <> "YES" Then
                    Return Result
                End If
            End If
            Dim ClassNeodynamic As New ClassNeodynamic
            '///-----------------------Solicita estructura de archivos para adjuntar desde dispositivos------------/////
            If TipoAlmacenamiento = 2 Then
                Dim file_ As New FileInfo(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA"))
                If UCase(file_.Extension) = ".TIF" Then
                    Result = ClassNeodynamic.Extraer_Documento_de_Multitif_fisico(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA"),
                                                                                  MatrizDocumentosFinal,
                                                                                  HttpContext.Current.Session.Item("WF_RUTA_TEMPO_FINAL") & "\")
                    If Result <> "YES" Then
                        Return Result
                    End If
                    If MatrizDocumentosFinal Is Nothing Then
                        Return "Matriz sin documentos"
                    End If
                Else
                    ReDim Preserve MatrizDocumentosFinal(0)
                    MatrizDocumentosFinal(0) = HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA")
                End If
            End If
            Dim EstructuraDatosPrevioAlmacenamineto() As Datos_Almacenamiento
            Dim NombreGabinete As String = ""
            Dim Radicado As String = ""
            Dim CampoRadicado As String = ""
            If DatosEnlaceScript <> "" Then
                Dim DatosEnlaceReturn As String = Trim(DatosEnlaceScript)
                If DatosEnlaceReturn = "" Then
                    Return "Enlace sin datos imposible almacenar"
                End If
                If InStr(DatosEnlaceReturn, "POSITIVOQL_") < 1 Then
                    Return "Enlace sin datos correctos "
                End If
                Dim dat As String = DatosEnlaceReturn.Replace("POSITIVOQL_", "<!#>")
                Dim Splipositvol() As String = dat.Split("<!#>")
                Dim SpliDATOS() As String = Splipositvol(1).Split("|")
                Dim SpliCAMPOS() As String = Splipositvol(2).Split("|")
                NombreGabinete = SpliDATOS(1)
                Radicado = SpliDATOS(2)
                CampoRadicado = SpliCAMPOS(2)
                '---------------------------------------------
                'Remplaza las R000 del numero de radicado
                '---------------------------------------------
                Dim RadicTemporal As String = ""
                RadicTemporal = Radicado.Replace("R", "")
                Radicado = Val(RadicTemporal)
                ReDim Preserve EstructuraDatosPrevioAlmacenamineto(0)
                EstructuraDatosPrevioAlmacenamineto(0).nombre_campo = CampoRadicado
                If TipoAlmacen = 1 Then
                    EstructuraDatosPrevioAlmacenamineto(0).valor_campo = Radicado
                Else
                    EstructuraDatosPrevioAlmacenamineto(0).valor_campo = ""
                End If
                ReDim Preserve EstructuraDatosPrevioAlmacenamineto(1)
                EstructuraDatosPrevioAlmacenamineto(1).nombre_campo = "ENLASE"
                EstructuraDatosPrevioAlmacenamineto(1).valor_campo = Radicado
            Else
                NombreGabinete = GabineteRadicado
                Radicado = ConsecutivoRadicado
                ReDim Preserve EstructuraDatosPrevioAlmacenamineto(0)
                Dim nombre_campo_radicado_gabinete As String = ""
                Dim Ref_Class_DETALLE_GABIENETE As New Class_DETALLE_GABIENETE
                Result = Ref_Class_DETALLE_GABIENETE.SolicitaNombreCampoRadicadoGabinete(NombreGabinete,
                                                                                         nombre_campo_radicado_gabinete)
                If Result <> "YES" Then
                    Return Result
                End If
                EstructuraDatosPrevioAlmacenamineto(0).nombre_campo = nombre_campo_radicado_gabinete
                EstructuraDatosPrevioAlmacenamineto(0).valor_campo = Radicado
                ReDim Preserve EstructuraDatosPrevioAlmacenamineto(1)
                EstructuraDatosPrevioAlmacenamineto(1).nombre_campo = "ENLASE"
                EstructuraDatosPrevioAlmacenamineto(1).valor_campo = Radicado
            End If
            Dim Refclas_workflow As New ClassWorkflow
            Dim Ref_class_ruta As New Class_worflow_rutas

            '------------------------------------------------
            'Retorna el nombre de la tabla de radicación
            'si el flujo se genero internamente desde
            'desde el radicador
            '-----------------------------------------------
            Dim ClassAlmacenamiento As New ClassAlmacenamiento
            Dim NombrePlantillaRadicado As String = ""
            Dim Class_ra_registro_general_radicacion As New Class_ra_registro_general_radicacion
            Result = Class_ra_registro_general_radicacion.SolicitaNombrePlantillaRadicado(Radicado,
                                                                                          NombrePlantillaRadicado)
            If Result <> "YES" Then
                Return Result
            End If
            Dim NombreTipoDocumento As String = ""
            Dim IdClaseFormatoDocumento As Integer = 0
            Dim FechaElaboracion As String = ""
            Dim ClassGaTipoDocumental As New ClassGaTipoDocumental
            Result = ClassGaTipoDocumental.SolicitaIdTipoFormatoDocumento(NombreClaseFormatoDocumento,
                                                                          IdClaseFormatoDocumento)
            If Result <> "YES" Then
                Return Result
            End If
            NombreTipoDocumento = NombreClaseFormatoDocumento
            Dim DateNow As String = Date.Today
            Result = ""
            Dim ClassGestionFechas As New ClassGestionFechas
            Result = ClassGestionFechas.FormateaFechaAlmacenamiento(DateNow)
            If Result <> "YES" Then
                Return Result
            End If
            FechaElaboracion = DateNow
            '-------------------------------------------------------
            'Solicita relación campos radicado plantilla gabinete
            '-------------------------------------------------------
            Dim IdPlantillaRadicado As Integer = 0
            Dim Class_system_plantilla_radicado As New Class_system_plantilla_radicado
            Result = Class_system_plantilla_radicado.SolicitaIdPlantillaRadicado(IdPlantillaRadicado,
                                                                                 NombrePlantillaRadicado)
            If Result <> "YES" Then
                Return Result
            End If
            Dim IdGabinete As Integer = 0
            Dim Class_system1 As New Class_system1
            Result = Class_system1.SolicitaIdGabineteDocuarchi(NombreGabinete,
                                                               IdGabinete)
            If Result <> "YES" Then
                Return Result
            End If
            Dim StruCamposPlantillaGabinete() As csfc_structure_relacion_campos_plantilla_ruta = Nothing
            Dim Class_ra_relacion_plantilla_gabinete As New Class_ra_relacion_plantilla_gabinete
            Result = Class_ra_relacion_plantilla_gabinete.SolicitaCamposRelacionPlantillaGabinete(IdPlantillaRadicado,
                                                                                                  IdGabinete,
                                                                                                  StruCamposPlantillaGabinete)
            If Result <> "YES" Then
                Return Result
            End If
            '------------------------------------------------------
            'Asigna los datos a la estructura de relación plantilla
            'gabinete
            '-------------------------------------------------------
            Dim Class_plantillas_radicacion As New Class_plantillas_radicacion
            Result = Class_plantillas_radicacion.AsignaDatosCamposPlantillaRadicadoGabinete(StruCamposPlantillaGabinete,
                                                                                            Radicado,
                                                                                            NombrePlantillaRadicado)
            If Result <> "YES" Then
                Return Result
            End If
            '--------------------------------------------------------
            'Formatea campos tipo date  y date time
            '--------------------------------------------------------
            Dim refclas_ClassGestionFechas As New ClassGestionFechas
            For i As Integer = 0 To StruCamposPlantillaGabinete.Length - 1
                If StruCamposPlantillaGabinete(i).tipo_campo_plantilla = "DATE" Then
                    If Not StruCamposPlantillaGabinete(i).dato_campo_plantilla Is Nothing And StruCamposPlantillaGabinete(i).dato_campo_plantilla <> "" Then
                        Result = refclas_ClassGestionFechas.csfc_Formatea_Fecha_Time_MYSQL_Fecha_Inicio(StruCamposPlantillaGabinete(i).dato_campo_plantilla)
                        If Result <> "YES" Then
                            Return Result
                        End If
                    End If

                End If
                If StruCamposPlantillaGabinete(i).tipo_campo_plantilla = "DATETIME" Then
                    If Not StruCamposPlantillaGabinete(i).dato_campo_plantilla Is Nothing And StruCamposPlantillaGabinete(i).dato_campo_plantilla <> "" Then
                        Result = refclas_ClassGestionFechas.csfc_Formatea_Fecha_Almacenamiento_Time_bsd(StruCamposPlantillaGabinete(i).dato_campo_plantilla)
                        If Result <> "YES" Then
                            Return Result
                        End If
                    End If

                End If
            Next
            Dim EstructuraGestion As estructure_gestion = Nothing
            Dim ClassGaExpediente As New ClassGaExpediente
            Result = ClassGaExpediente.SolicitaEstructuraExpedienteDocumentoVinculante(EstructuraGestion,
                                                                                       NombreGabinete,
                                                                                       IdImagenWorkflow,
                                                                                       IdTareaWorkflow,
                                                                                       Radicado,
                                                                                       NombreRutaWorkflow,
                                                                                       IdRutaWorkflow,
                                                                                       ValorObjeto)
            If Result <> "YES" Then
                Return Result
            End If
            Dim ClassRaTipoDocSeries As New Class_ra_tipo_doc_series
            Dim EsctructuraExpediente() As expediente_conservacion = Nothing
            Dim CDclasificacionTipoDocumental As New CDclasificacionTipoDocumental
            CDclasificacionTipoDocumental.DescripcionTipoDocumento = ""
            CDclasificacionTipoDocumental.IdSerie = 0
            CDclasificacionTipoDocumental.IdArea = 0
            CDclasificacionTipoDocumental.NombreSerie = ""
            CDclasificacionTipoDocumental.NombreSubSerie = ""
            CDclasificacionTipoDocumental.NombreArea = ""
            '//------------Asigna los datos de clasificación documental heredado del expediente----------////
            If EstructuraGestion.ID_EXPEDIENTE <> 0 Then
                Result = ClassGaExpediente.SolicitaDatosEstructuraExpediente(EstructuraGestion.ID_EXPEDIENTE,
                                                                             EsctructuraExpediente)
                If Result <> "YES" Then
                    Return Result
                End If
                Dim StruTipoListaChequeo As stru_tipo_lista_chequeo = Nothing
                Dim ra_dig_tipos_docum_lista_chequeo As New ra_dig_tipos_docum_lista_chequeo
                If IdTipoListaChek <> 0 And IdTipoListaChek <> -1 Then
                    Result = ra_dig_tipos_docum_lista_chequeo.SolicitaDatosTipoDocumentalListaChequeo(IdTipoListaChek,
                                                                                                      StruTipoListaChequeo)
                    If Result <> "YES" Then
                        Return Result
                    End If
                    If StruTipoListaChequeo.subseries_documentales_Id_SubSeries <> 0 Then
                        CDclasificacionTipoDocumental.IdTipoDocumento = StruTipoListaChequeo.tipos_doc_subseries_Id_Tipos_Doc_SubSerie
                    Else
                        CDclasificacionTipoDocumental.IdTipoDocumento = StruTipoListaChequeo.tipo_doc_series_Id_Tipo_Doc_Series
                    End If
                    Result = ClassRaTipoDocSeries.SolicitaNombreTipoDocumentalSerieSubSerie(CDclasificacionTipoDocumental.IdTipoDocumento,
                                                                                            DescripcionTipo)
                    If Result <> "YES" Then
                        Return Result
                    End If
                End If
                CDclasificacionTipoDocumental.DescripcionTipoDocumento = DescripcionTipo
                CDclasificacionTipoDocumental.IdSerie = EsctructuraExpediente(0).CODIGO_SERIE
                CDclasificacionTipoDocumental.IdSubSerie = EsctructuraExpediente(0).CODIGO_SUBSERIE
                CDclasificacionTipoDocumental.IdArea = EsctructuraExpediente(0).CODIGO_AREA_TRD
                CDclasificacionTipoDocumental.NombreSerie = EsctructuraExpediente(0).NOMBRE_SERIE
                CDclasificacionTipoDocumental.NombreSubSerie = EsctructuraExpediente(0).NOMBRE_SUBSERIE
                CDclasificacionTipoDocumental.NombreArea = EsctructuraExpediente(0).NOMBRE_AREA
            End If
            '//------------Asigna los datos de clasificación documental heredado de la lista de chequeo----------////
            If EstructuraGestion.ID_EXPEDIENTE = 0 And IdTipoListaChek <> 0 And IdTipoListaChek <> -1 Then
                Result = ClassGaTipoDocumental.SolicitaEstructuraClasificacionTipoDocumento(IdTipoListaChek,
                                                                                            CDclasificacionTipoDocumental)
                If Result <> "YES" Then
                    Return Result
                End If
            End If
            '//--------Asigna datos gestión al documento----------------////
            Dim EstructuraGestionAlmacenamiento As estructure_gestion = Nothing
            EstructuraGestionAlmacenamiento.CLASE_DOCUMENTO = CDclasificacionTipoDocumental.DescripcionTipoDocumento
            EstructuraGestionAlmacenamiento.ID_AREA = CDclasificacionTipoDocumental.IdArea
            EstructuraGestionAlmacenamiento.ID_CLASE_DOCUMENTO = IdClaseFormatoDocumento
            EstructuraGestionAlmacenamiento.ID_SERIE = CDclasificacionTipoDocumental.IdSerie
            EstructuraGestionAlmacenamiento.ID_SUB_SERIE = CDclasificacionTipoDocumental.IdSubSerie
            EstructuraGestionAlmacenamiento.ID_TIPO_UNIDAD_CONSERVACION = 0
            EstructuraGestionAlmacenamiento.ID_TIPODOCUMENTO = CDclasificacionTipoDocumental.IdTipoDocumento
            EstructuraGestionAlmacenamiento.ID_UNIDAD_CONSERVACION = 0
            EstructuraGestionAlmacenamiento.ID_USUARIO_GESTION = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
            EstructuraGestionAlmacenamiento.TIPO_UNIDAD_DOCUMENTAL = 0
            EstructuraGestionAlmacenamiento.UNIDAD_CONSERVACION = ""
            EstructuraGestionAlmacenamiento.FECHA_ELABORACION = FechaElaboracion
            EstructuraGestionAlmacenamiento.TIPODOCUMENTO = CDclasificacionTipoDocumental.DescripcionTipoDocumento
            EstructuraGestionAlmacenamiento.NOMBRE_SERIE = CDclasificacionTipoDocumental.NombreSerie
            EstructuraGestion.NOMBRE_SUB_SERIE = CDclasificacionTipoDocumental.NombreSubSerie
            ReDim Preserve EstructuraDatosPrevioAlmacenamineto(2)
            EstructuraDatosPrevioAlmacenamineto(2).nombre_campo = "EXPEDIENTE"
            EstructuraDatosPrevioAlmacenamineto(2).valor_campo = EstructuraGestion.EXPEDIENTE
            ReDim Preserve EstructuraDatosPrevioAlmacenamineto(3)
            EstructuraDatosPrevioAlmacenamineto(3).nombre_campo = "CLASEDOCUMENTO"
            EstructuraDatosPrevioAlmacenamineto(3).valor_campo = NombreTipoDocumento
            ReDim Preserve EstructuraDatosPrevioAlmacenamineto(4)
            EstructuraDatosPrevioAlmacenamineto(4).nombre_campo = "FECHAELABORACION"
            EstructuraDatosPrevioAlmacenamineto(4).valor_campo = FechaElaboracion
            ReDim Preserve EstructuraDatosPrevioAlmacenamineto(5)
            EstructuraDatosPrevioAlmacenamineto(5).nombre_campo = "TIPODOCUMENTO"
            EstructuraDatosPrevioAlmacenamineto(5).valor_campo = CDclasificacionTipoDocumental.DescripcionTipoDocumento
            ReDim Preserve EstructuraDatosPrevioAlmacenamineto(6)
            EstructuraDatosPrevioAlmacenamineto(6).nombre_campo = "NOMBRESERIE"
            EstructuraDatosPrevioAlmacenamineto(6).valor_campo = CDclasificacionTipoDocumental.NombreSerie
            ReDim Preserve EstructuraDatosPrevioAlmacenamineto(7)
            EstructuraDatosPrevioAlmacenamineto(7).nombre_campo = "NOMBRESUBSERIE"
            EstructuraDatosPrevioAlmacenamineto(7).valor_campo = CDclasificacionTipoDocumental.NombreSubSerie
            Dim Icount As Integer = 7
            '----------------------------------------------
            'Genera la matriz de datos de almacenamiento
            '----------------------------------------------
            Dim Refclas_Class_DETALLE_GABIENETE As New Class_DETALLE_GABIENETE
            Dim EstructuraDatosAlmacenamineto() As String
            Erase EstructuraDatosAlmacenamineto
            Result = Refclas_Class_DETALLE_GABIENETE.SolicitaEstructuraCamposGabinete(NombreGabinete,
                                                                                      EstructuraDatosAlmacenamineto,
                                                                                      EstructuraDatosPrevioAlmacenamineto)
            If Result <> "YES" Then
                Return Result
            End If
            If EstructuraDatosAlmacenamineto Is Nothing Then
                Return "Matriz de datos de almacenamiento es nothing "
            End If
            '///---------------Asignamos los valores de la relación plantilla radicado gabinete para llenar los indices -------------/////
            Dim Iconta As Integer = 0
            For i As Integer = 8 To EstructuraDatosPrevioAlmacenamineto.Length - 1
                For z As Integer = 0 To StruCamposPlantillaGabinete.Length - 1
                    If EstructuraDatosPrevioAlmacenamineto(i).nombre_campo = StruCamposPlantillaGabinete(z).nombre_campo_ruta Then
                        EstructuraDatosAlmacenamineto(Iconta) = StruCamposPlantillaGabinete(z).dato_campo_plantilla
                        EstructuraDatosPrevioAlmacenamineto(i).valor_campo = StruCamposPlantillaGabinete(z).dato_campo_plantilla
                    End If
                Next
                Iconta = Iconta + 1
            Next
            '///---------------Asignamos los valores externos a los indices de gabinete -------------/////
            Iconta = 0
            If CDcamposAsignaAlmacenamiento.Count > 0 Then
                For i As Integer = 8 To EstructuraDatosPrevioAlmacenamineto.Length - 1
                    For z As Integer = 0 To CDcamposAsignaAlmacenamiento.Count - 1
                        If EstructuraDatosPrevioAlmacenamineto(i).nombre_campo = CDcamposAsignaAlmacenamiento.Item(z).NombreCampoGabinete Then
                            EstructuraDatosAlmacenamineto(Iconta) = CDcamposAsignaAlmacenamiento.Item(z).ValorCampoGabinete
                            EstructuraDatosPrevioAlmacenamineto(i).valor_campo = CDcamposAsignaAlmacenamiento.Item(z).ValorCampoGabinete
                        End If
                    Next
                    Iconta = Iconta + 1
                Next
            End If
            Dim TipoArchivoDocuarchi As Object = 0
            Dim Class_da_extension As New Class_da_extension
            Dim File As New FileInfo(MatrizDocumentosFinal(0))
            Result = Class_da_extension.SolicitaTipoArchivoDocuarchiExtension(File.Extension,
                                                                              TipoArchivoDocuarchi)
            If Result <> "YES" Then
                Return Result
            End If
            TipoArchivoDocuarchi = CInt(TipoArchivoDocuarchi)
            '-----------------------------------------------
            'Almacena documento en la base docuarchi.net
            '-----------------------------------------------
            Dim UserWf As String = "WF-" & HttpContext.Current.Session.Item("Login_Usuario_Workfow")
            Result = Me.Almacenamiento("", "", NombreGabinete, 0, EstructuraDatosAlmacenamineto, 2,
            MatrizDocumentosFinal.Length, TipoArchivoDocuarchi, MatrizDocumentosFinal, 0, IdImagenAlamacenada,
            TipoArchivoDocuarchi, HttpContext.Current.Session.Item("GA_IDEMPRESA"),
            HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), EstructuraGestionAlmacenamiento.ID_AREA,
            EstructuraGestionAlmacenamiento.ID_SERIE, EstructuraGestionAlmacenamiento.ID_SUB_SERIE,
            EstructuraGestionAlmacenamiento.ID_TIPODOCUMENTO, EstructuraGestionAlmacenamiento.ID_EXPEDIENTE,
            EstructuraGestionAlmacenamiento.ID_TIPO_EXPEDIENTE,
            EstructuraGestionAlmacenamiento.ID_UNIDAD_CONSERVACION, EstructuraGestionAlmacenamiento.ID_TIPO_UNIDAD_CONSERVACION,
            EstructuraGestionAlmacenamiento.ID_CLASE_DOCUMENTO, EstructuraGestionAlmacenamiento.EXPEDIENTE, EstructuraGestionAlmacenamiento.NOMBRE_SERIE,
            EstructuraGestionAlmacenamiento.NOMBRE_SUB_SERIE, EstructuraGestionAlmacenamiento.TIPODOCUMENTO, EstructuraGestionAlmacenamiento.UNIDAD_CONSERVACION,
            EstructuraGestionAlmacenamiento.CLASE_DOCUMENTO, EstructuraGestionAlmacenamiento.FECHA_ELABORACION, Radicado, "RAD-" & Radicado, 0, 0, 0, IdTareaWorkflow,
            IdRutaWorkflow)
            If Result <> "YES" Then
                Return Result
            End If
            Dim attrNodeGru1 As New TreeNode
            Dim val_documento As String = EstructuraGestionAlmacenamiento.TIPODOCUMENTO
            If val_documento = "" Then
                val_documento = "D-" & IdImagenAlamacenada
            End If
            Dim DescripcionTipoDocumento As String = ""
            If CDclasificacionTipoDocumental.DescripcionTipoDocumento = "" Then
                DescripcionTipoDocumento = val_documento
            End If
            Dim classgabinete As New ClassDaGabinete
            Dim icono As String = ""
            classgabinete.SolicitaIconoImageFownt(TipoArchivoDocuarchi.ToString,
                                                  icono)
            EstructuraDatosImagen.nombre_gabinete = NombreGabinete
            EstructuraDatosImagen.id_imagen = IdImagenAlamacenada
            EstructuraDatosImagen.radicado = Radicado
            EstructuraDatosImagen.tipodocumental = DescripcionTipoDocumento
            EstructuraDatosImagen.notipodocumento = val_documento
            EstructuraDatosImagen.extension = UCase(File.Extension)
            EstructuraDatosImagen.icono_icono_awe_some = icono
            EstructuraDatosImagen.DBT = TipoArchivoDocuarchi
            EstructuraDatosImagen.estado_firma_digital = 0
            '-----------------------------------------------
            'Elimina los documentos almacenados  
            '-----------------------------------------------
            For k As Integer = 0 To MatrizDocumentosFinal.Length - 1
                If System.IO.File.Exists(MatrizDocumentosFinal(k)) = True Then
                    System.IO.File.Delete(MatrizDocumentosFinal(k))
                End If
            Next
            If TipoAlmacenamiento <> 0 Then
                If System.IO.File.Exists(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA")) = True Then
                    Kill(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA"))
                End If
                HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA") = ""
                HttpContext.Current.Session.Item("WF_ERROR_RESPUESTA") = ""
            End If
            'Actualiza documento en el flujo
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            If EvaluaActualizaImagenWorkflow = 1 And IdImagenWorkflow = 0 Then
                Result = Class_DAT_ADIC_TAR.ActualizaIdImagenTareaWorkflow(NombreRutaWorkflow,
                                                                           IdTareaWorkflow,
                                                                           IdImagenAlamacenada)
                If Result <> "YES" Then
                    Return Result
                End If
            End If
            Return "YES"
        Catch ex As Exception
            Return "Inonsistencia general funcion AlmacenaDocumentosRadicacion " & ex.Message
        End Try
    End Function
    Function Almacenamiento_Documentos_Digitalizados(ByVal Datos As String,
                                                     ByRef ID_ALMACEN As Integer,
                                                     ByVal Tipo_Amacen As Integer,
                                                     ByRef Treview As TreeView,
                                                     ByRef datos_image As stru_datos_image_lista,
                                                     ByVal tipo_digitalizacion As String,
                                                     Optional tipo_almacenamiento As Integer = 0,
                                                     Optional agrega_item_listview As Integer = 1) As String
        '-----------------------------------------------------------
        'Funcion : Almacena los documentos digitalizados
        'Fecha : 2014-02-24
        'Ingeniero : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim RefclasDigitaliza As New ClassWorkflowDigitalizacion
            Dim Selection As String = HttpContext.Current.Session.Item("SELECCIONTEMPORAL")
            Dim spl() As String = Selection.Split("|")
            Dim L = HttpContext.Current.Session.Item("DG_LISTA_CHEQUEO")
            Dim kc = HttpContext.Current.Session.Item("DG_RIPO_DOCUMENTAL_LISTA_CHEQUEO")
            Dim Matri_Documentos_Final() As String
            Erase Matri_Documentos_Final
            If tipo_almacenamiento = 0 Then
                '--------------------------------------------------------
                'Retorna los documentos almacenados en el file system
                '--------------------------------------------------------
                Result = RefclasDigitaliza.SolicitaMatrizDocumentosDigitalizados(spl(0),
                                                                                HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ESCANER"),
                                                                                Matri_Documentos_Final)
                If Result <> "YES" Then
                    Almacenamiento_Documentos_Digitalizados = "Imposible encontrar documentos " & Result
                    Exit Function
                End If
            Else
                '-------------------------------------------------------
                'Retorma matriz de documentos almacenados adjuntos
                '-------------------------------------------------------
                Dim Refclas_almacenamiento As New ClassAlmacenamiento
                Result = Refclas_almacenamiento.Retorna_matriz_documentos_adjuntos_workflow(Matri_Documentos_Final)
                If Result <> "YES" Then
                    Almacenamiento_Documentos_Digitalizados = Result
                    Exit Function
                End If
            End If
            '----------------------------------------------------
            'Obtiene los datos de almacenamiento
            '----------------------------------------------------
            Dim matri_datos() As Datos_Almacenamiento
            Dim Gabinete As String = ""
            Dim Radicado As String = ""
            Dim Campo_Radicado As String = ""
            If Datos <> "" Then
                Dim datos_enlace As String = Trim(Datos)
                If datos_enlace = "" Then
                    Almacenamiento_Documentos_Digitalizados = "Enlace sin datos imposible almacenar"
                    Exit Function
                End If
                If InStr(datos_enlace, "POSITIVOQL_") < 1 Then
                    Almacenamiento_Documentos_Digitalizados = "Enlace sin datos correctos"
                    Exit Function
                End If
                Dim dat As String = datos_enlace.Replace("POSITIVOQL_", "<!#>")
                Dim Splipositvol() As String = dat.Split("<!#>")
                Dim SpliDATOS() As String = Splipositvol(1).Split("|")
                Dim SpliCAMPOS() As String = Splipositvol(2).Split("|")
                Gabinete = SpliDATOS(1)
                Radicado = SpliDATOS(2)
                Campo_Radicado = SpliCAMPOS(2)
                '---------------------------------------------
                'Remplaza las R000 del numero de radicado
                '---------------------------------------------
                Dim RadicTemporal As String = ""
                RadicTemporal = Radicado.Replace("R", "")
                Radicado = Val(RadicTemporal)
                ReDim Preserve matri_datos(0)
                matri_datos(0).nombre_campo = Campo_Radicado
                If Tipo_Amacen = 1 Then
                    matri_datos(0).valor_campo = Radicado
                Else
                    matri_datos(0).valor_campo = ""
                End If
                ReDim Preserve matri_datos(1)
                matri_datos(1).nombre_campo = "ENLASE"
                matri_datos(1).valor_campo = Radicado
            Else
                Gabinete = HttpContext.Current.Session.Item("DG_NOMBRE_GABINETE")
                Radicado = HttpContext.Current.Session.Item("DG_RADICADO")
                ReDim Preserve matri_datos(0)
                Dim nombre_campo_radicado_gabinete As String = ""
                Dim Ref_Class_DETALLE_GABIENETE As New Class_DETALLE_GABIENETE
                Result = Ref_Class_DETALLE_GABIENETE.SolicitaNombreCampoRadicadoGabinete(Gabinete,
                                                                                        nombre_campo_radicado_gabinete)
                If Result <> "YES" Then
                    Almacenamiento_Documentos_Digitalizados = Result
                    Exit Function
                End If
                matri_datos(0).nombre_campo = nombre_campo_radicado_gabinete
                If Tipo_Amacen = 1 Then
                    matri_datos(0).valor_campo = Radicado
                Else
                    matri_datos(0).valor_campo = ""
                End If
                ReDim Preserve matri_datos(1)
                matri_datos(1).nombre_campo = "ENLASE"
                matri_datos(1).valor_campo = Radicado
            End If

            Dim nombre_tipo_documento As String = ""
            Dim id_clase_documento As Integer = 0
            Dim fecha_elaboracion As String = ""
            Dim reflcas_tipo_documento As New ClassGaTipoDocumental
            Result = reflcas_tipo_documento.SolicitaIdTipoFormatoDocumento("DOCUMENTO DIGITALIZADO",
                                                                          id_clase_documento)
            If Result <> "YES" Then
                Almacenamiento_Documentos_Digitalizados = Result
                Exit Function
            Else
                nombre_tipo_documento = "DOCUMENTO DIGITALIZADO"
            End If
            Dim date1al As String = Date.Today
            Result = ""
            Dim ref_ClassGestionFechas As New ClassGestionFechas
            Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
            If Result <> "YES" Then
                Almacenamiento_Documentos_Digitalizados = "Error formateando fecha almacenamiento Funcion: Almacenamiento_Documentos_Digitalizados " & Result
                Exit Function
            Else
                fecha_elaboracion = date1al

            End If
            '----------------------------------------------
            'Configura  tipo tramite
            '----------------------------------------------
            Dim id_tipo_documento As Integer = 0
            Dim id_area As Integer = 0
            Dim id_serie As Integer = 0
            Dim id_sub_serie As Integer = 0
            Dim descripcion_tipo_documento As String = ""
            Dim nombre_area As String = ""
            Dim nombre_serie As String = ""
            Dim nombre_sub_serie As String = ""
            Dim Ref_Class_ra_dig_tipos_docum_lista_chequeo As New ra_dig_tipos_docum_lista_chequeo
            If HttpContext.Current.Session.Item("DG_LISTA_CHEQUEO") <> -1 Then
                Dim stru As stru_tipo_lista_chequeo = Nothing
                Result = Ref_Class_ra_dig_tipos_docum_lista_chequeo.SolicitaDatosTipoDocumentalListaChequeo(HttpContext.Current.Session.Item("DG_LISTA_CHEQUEO"),
                                                                                                                 stru)
                If Result <> "YES" Then
                    Almacenamiento_Documentos_Digitalizados = Result
                    Exit Function
                End If
                If stru.subseries_documentales_Id_SubSeries <> 0 Then
                    id_tipo_documento = stru.tipos_doc_subseries_Id_Tipos_Doc_SubSerie
                Else
                    id_tipo_documento = stru.tipo_doc_series_Id_Tipo_Doc_Series
                End If
                '-----------------------------------------------
                'Retorna serie y sub serie tipo documento
                '-----------------------------------------------
                Dim stru_tipo As stru_tipo_documental = Nothing
                Dim ref_clas_trd As New ClassTrdDocumental
                Result = ref_clas_trd.Solicita_datos_estructura_tipo_documento(id_tipo_documento,
                                                                               stru_tipo)
                If Result <> "YES" Then
                    Almacenamiento_Documentos_Digitalizados = Result
                    Exit Function
                End If
                id_serie = stru_tipo.Series_Documentales_Id_Series
                id_sub_serie = stru_tipo.sub_serie_id_serie
                Dim ref_Class_series_documentales As New Class_series_documentales
                Result = ref_Class_series_documentales.Retorna_id_area_serie_documental(id_serie,
                                                                                        id_area)
                If Result <> "YES" Then
                    Almacenamiento_Documentos_Digitalizados = Result
                    Exit Function
                End If
                Dim ref_Class_ra_tipo_doc_series As New Class_ra_tipo_doc_series
                If id_tipo_documento <> 0 Then
                    Result = ref_Class_ra_tipo_doc_series.Retorna_nombre_tipo_documental(id_serie,
                                                                                         id_sub_serie,
                                                                                         id_tipo_documento,
                                                                                         descripcion_tipo_documento)
                    If Result <> "YES" Then
                        Almacenamiento_Documentos_Digitalizados = Result
                        Exit Function
                    End If
                End If
                If nombre_tipo_documento = "" Then
                    Result = reflcas_tipo_documento.SolicitaIdTipoFormatoDocumento("DOCUMENTO DIGITALIZADO", id_clase_documento)
                    If Result <> "YES" Then
                        Almacenamiento_Documentos_Digitalizados = Result
                        Exit Function
                    Else
                        nombre_tipo_documento = "DOCUMENTO DIGITALIZADO"
                    End If
                End If
                Dim ref_Class_areas_depart_radicacion As New Class_areas_depart_radicacion
                If id_area <> 0 Then
                    Result = ref_Class_areas_depart_radicacion.Retorna_nombre_area_por_id_area(id_area,
                                                                                               nombre_area)
                    If Result <> "YES" Then
                        Almacenamiento_Documentos_Digitalizados = Result
                        Exit Function
                    End If
                End If

                If id_serie <> 0 Then
                    Result = ref_Class_series_documentales.Retorna_nombre_serie_id_serie(id_serie,
                                                                                         nombre_serie)
                    If Result <> "YES" Then
                        Almacenamiento_Documentos_Digitalizados = Result
                        Exit Function
                    End If
                End If
                Dim Class_subseries_documentales As New Class_subseries_documentales
                If id_sub_serie <> 0 Then
                    Result = Class_subseries_documentales.Retorna_nombre_sub_serie(id_sub_serie,
                                                                                    nombre_sub_serie)
                    If Result <> "YES" Then
                        Almacenamiento_Documentos_Digitalizados = Result
                        Exit Function
                    End If
                End If


            End If
            Dim Ref_producion As New ClassGaProducionDocumental
            Dim inventario_documental As Integer = 0
            Dim aplica_trd As Integer = 0
            Dim asigna_unidad As Integer = 0
            Dim Ref_Class_system1 As New Class_system1
            Result = Ref_Class_system1.Verifica_opcion_aplicar_trd_inventario_unidad_documental_gabinete(Gabinete,
                                                                                                         inventario_documental,
                                                                                                         aplica_trd,
                                                                                                         asigna_unidad)
            If Result <> "YES" Then
                Almacenamiento_Documentos_Digitalizados = Result
                Exit Function
            End If
            If aplica_trd = 1 Then
                If nombre_tipo_documento = "" Then
                    Result = reflcas_tipo_documento.SolicitaIdTipoFormatoDocumento("DOCUMENTO DIGITALIZADO",
                                                                                  id_clase_documento)
                    If Result <> "YES" Then
                        Almacenamiento_Documentos_Digitalizados = Result
                        Exit Function
                    Else
                        nombre_tipo_documento = "DOCUMENTO DIGITALIZADO"
                    End If
                End If
            End If
            Dim matri_gestion As estructure_gestion = Nothing
            Dim ClassGaExpediente As New ClassGaExpediente
            Result = ClassGaExpediente.SolicitaEstructuraExpedienteDocumentoVinculante(matri_gestion,
                                                                                       Gabinete,
                                                                                       0,
                                                                                       Val(spl(0)),
                                                                                       Radicado,
                                                                                       HttpContext.Current.Session("WF_RUTAWORKFLOW"),
                                                                                       HttpContext.Current.Session("Id_Ruta_Workflow"),
                                                                                       "")
            If Result <> "YES" Then
                Almacenamiento_Documentos_Digitalizados = Result
                Exit Function
            End If
            '-------------------------------------------
            'Asigna datos gestion
            '-------------------------------------------

            matri_gestion.CLASE_DOCUMENTO = nombre_tipo_documento
            'matri_gestion.EXPEDIENTE = nombre_expediente
            matri_gestion.ID_AREA = id_area
            matri_gestion.ID_CLASE_DOCUMENTO = id_clase_documento
            'matri_gestion.ID_EXPEDIENTE = id_expediente
            matri_gestion.ID_SERIE = id_serie
            matri_gestion.ID_SUB_SERIE = id_sub_serie
            'matri_gestion.ID_TIPO_EXPEDIENTE = id_tipo_expediente
            matri_gestion.ID_TIPO_UNIDAD_CONSERVACION = 0
            matri_gestion.ID_TIPODOCUMENTO = id_tipo_documento
            matri_gestion.ID_UNIDAD_CONSERVACION = 0
            matri_gestion.ID_USUARIO_GESTION = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
            matri_gestion.TIPO_UNIDAD_DOCUMENTAL = 0
            matri_gestion.UNIDAD_CONSERVACION = ""
            matri_gestion.FECHA_ELABORACION = fecha_elaboracion
            matri_gestion.TIPODOCUMENTO = descripcion_tipo_documento
            matri_gestion.NOMBRE_SERIE = nombre_serie
            matri_gestion.NOMBRE_SUB_SERIE = nombre_sub_serie
            ReDim Preserve matri_datos(2)
            matri_datos(2).nombre_campo = "EXPEDIENTE"
            matri_datos(2).valor_campo = matri_gestion.EXPEDIENTE
            ReDim Preserve matri_datos(3)
            matri_datos(3).nombre_campo = "CLASEDOCUMENTO"
            matri_datos(3).valor_campo = nombre_tipo_documento
            ReDim Preserve matri_datos(4)
            matri_datos(4).nombre_campo = "FECHAELABORACION"
            matri_datos(4).valor_campo = fecha_elaboracion
            ReDim Preserve matri_datos(5)
            matri_datos(5).nombre_campo = "TIPODOCUMENTO"
            matri_datos(5).valor_campo = descripcion_tipo_documento
            ReDim Preserve matri_datos(6)
            matri_datos(6).nombre_campo = "NOMBRESERIE"
            matri_datos(6).valor_campo = nombre_serie
            ReDim Preserve matri_datos(7)
            matri_datos(7).nombre_campo = "NOMBRESUBSERIE"
            matri_datos(7).valor_campo = nombre_sub_serie
            '----------------------------------------------
            'Genera la matriz de datos de almacenamiento
            '----------------------------------------------
            Dim Refclas_Class_DETALLE_GABIENETE As New Class_DETALLE_GABIENETE
            Dim Matri_Datos_Almacen() As String
            Erase Matri_Datos_Almacen
            Result = Refclas_Class_DETALLE_GABIENETE.SolicitaValoresCamposDocumentoGabinete(Matri_Datos_Almacen,
                                                                                            Gabinete,
                                                                                            matri_datos)
            If Result <> "YES" Then
                Almacenamiento_Documentos_Digitalizados = "Imposible general mariz datos almacenamiento "
                Exit Function
            End If
            If Matri_Datos_Almacen Is Nothing Then
                Almacenamiento_Documentos_Digitalizados = "Matriz de datos de almacenamiento es nothing "
                Exit Function
            End If
            '----------------------------------------------
            'Obtiene el tipo documento 
            '----------------------------------------------
            Dim Tipo_Documento As Integer = -1
            Dim Refclasvisor As New Classactualizacionvisor
            Result = ""
            Dim filinf As New FileInfo(Matri_Documentos_Final(0))
            Dim Class_da_extension As New Class_da_extension
            Result = Class_da_extension.SolicitaTipoArchivoDocuarchiExtension(filinf.Extension,
                                                                          Tipo_Documento)
            If Result <> "YES" Then
                Almacenamiento_Documentos_Digitalizados = Result
                Exit Function
            End If
            Dim ClassDaGabinete As New ClassDaGabinete
            '-----------------------------------------------
            'Almacena documento en la base docuarchi.net
            '-----------------------------------------------
            Dim UserWf As String = "WF-" & HttpContext.Current.Session.Item("Login_Usuario_Workfow")
            Result = ""
            Dim estado_firma_digital As Integer = 0
            Result = Me.Almacenamiento("", "", Gabinete, 0, Matri_Datos_Almacen, 2,
            Matri_Documentos_Final.Length, Tipo_Documento, Matri_Documentos_Final, 0, ID_ALMACEN,
            Tipo_Documento, HttpContext.Current.Session.Item("GA_IDEMPRESA"),
            HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), matri_gestion.ID_AREA,
            matri_gestion.ID_SERIE, matri_gestion.ID_SUB_SERIE,
            matri_gestion.ID_TIPODOCUMENTO, matri_gestion.ID_EXPEDIENTE, matri_gestion.ID_TIPO_EXPEDIENTE,
            matri_gestion.ID_UNIDAD_CONSERVACION, matri_gestion.ID_TIPO_UNIDAD_CONSERVACION,
            matri_gestion.ID_CLASE_DOCUMENTO, matri_gestion.EXPEDIENTE, matri_gestion.NOMBRE_SERIE,
            matri_gestion.NOMBRE_SUB_SERIE, matri_gestion.TIPODOCUMENTO, matri_gestion.UNIDAD_CONSERVACION,
            matri_gestion.CLASE_DOCUMENTO, matri_gestion.FECHA_ELABORACION, Radicado, "RAD-" & Radicado, "", 0, estado_firma_digital,
            Val(spl(0)),
            HttpContext.Current.Session("Id_Ruta_Workflow"))
            If Result <> "YES" Then
                Almacenamiento_Documentos_Digitalizados = Result
                Exit Function
            End If
            Dim attrNodeGru1 As New TreeNode
            Dim val_documento As String = matri_gestion.TIPODOCUMENTO
            If val_documento = "" Then
                val_documento = "D-" & ID_ALMACEN
            End If
            Dim classgabinete As New ClassDaGabinete
            Dim icono As String = ""
            classgabinete.Agrega_icono_image_fownt_java(Tipo_Documento.ToString,
                                                               icono)
            If descripcion_tipo_documento = "" Then
                descripcion_tipo_documento = val_documento
            End If
            datos_image.nombre_gabinete = Gabinete
            datos_image.id_imagen = ID_ALMACEN
            datos_image.radicado = Radicado
            datos_image.tipodocumental = descripcion_tipo_documento
            datos_image.notipodocumento = val_documento
            datos_image.extension = UCase(filinf.Extension)
            datos_image.icono_icono_awe_some = icono
            datos_image.estado_firma_digital = estado_firma_digital
            If agrega_item_listview = 1 Then
                attrNodeGru1.Value = Gabinete & "|" & ID_ALMACEN & "|" & Radicado & "|" & Tipo_Documento
                attrNodeGru1.PopulateOnDemand = False
                Dim refclas_seleccion As New Classselecciotarea
                If Not Treview Is Nothing Then
                    Result = refclas_seleccion.Agrega_icono_image_tre_view(Matri_Documentos_Final(0),
                                                                           attrNodeGru1)
                    If descripcion_tipo_documento = "" Then
                        attrNodeGru1.Text = "Documento(" & Treview.Nodes.Count & ")"
                    Else
                        attrNodeGru1.Text = descripcion_tipo_documento
                    End If
                    Treview.Nodes.Add(attrNodeGru1)
                End If
            End If
            '-----------------------------------------------
            'Elimina los documentos almacenados  
            '-----------------------------------------------
            For k As Integer = 0 To Matri_Documentos_Final.Length - 1
                If File.Exists(Matri_Documentos_Final(k)) Then
                    File.Delete(Matri_Documentos_Final(k))
                End If
            Next
            If tipo_almacenamiento <> 0 Then
                If File.Exists(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA")) = True Then
                    Kill(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA"))
                End If
                HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA") = ""
                HttpContext.Current.Session.Item("WF_ERROR_RESPUESTA") = ""
            End If
            Almacenamiento_Documentos_Digitalizados = "YES"
        Catch ex As Exception
            Almacenamiento_Documentos_Digitalizados = "Funcion Almacenamiento_Documentos_Digitalizados : " & ex.Message
        End Try
    End Function
    Function Adjunta_donumento_relacionado(ByRef pag As Page,
                                           ByRef id_imagen As Integer,
                                           ByVal nombre_gabinete As String,
                                           ByVal id_documento_workflow_origen As Integer,
                                           ByVal tipo_documental As Integer,
                                           ByVal id_tarea_wf As Long,
                                           ByVal radicado As String,
                                           ByRef stru_datos_image_lista As stru_datos_image_lista,
                                           ByVal option_agrega_item_listview As Integer) As String
        Try

            If (File.Exists(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA"))) Then
                Dim refclas As New ClassNeodynamic
                Dim Matri_Doc() As String
                Erase Matri_Doc
                Dim clasñade As New ClassAñadirDocumento
                Dim Result As String = ""
                Dim clasvis As New ClassRaEnvioCorrespondencia
                Dim content As Object = Nothing
                Dim file As New FileInfo(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA"))
                If UCase(file.Extension) = ".TIF" Then
                    Result = refclas.Extraer_Documento_de_Multitif_fisico(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA"),
                                                                          Matri_Doc,
                                                                          HttpContext.Current.Session.Item("WF_RUTA_TEMPO_FINAL") & "\")
                    If Result <> "YES" Then
                        Adjunta_donumento_relacionado = "Imposible extraer documento Multi tif "
                        Exit Function
                    End If
                    If Matri_Doc Is Nothing Then
                        Adjunta_donumento_relacionado = "La matriz de multi tif es nothing "
                        Exit Function
                    End If
                Else
                    ReDim Preserve Matri_Doc(0)
                    Matri_Doc(0) = HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA")
                End If

                If tipo_documental = -1 Or tipo_documental = 0 Then
                    Result = Me.Guardar_Documento_adjunto_relacionado_con_parametro_imagen_previa(nombre_gabinete,
                                                                                                  id_documento_workflow_origen,
                                                                                                  Matri_Doc,
                                                                                                  id_imagen,
                                                                                                  tipo_documental,
                                                                                                  id_tarea_wf,
                                                                                                  radicado,
                                                                                                  stru_datos_image_lista)
                    If Result <> "YES" Then
                        Adjunta_donumento_relacionado = "Imposible guardar el documento relacionado " & Result
                        Exit Function
                    End If
                Else
                    Result = Me.Guarda_documento_digitalizado_relacionado_tipificado(nombre_gabinete,
                                                                                     id_documento_workflow_origen,
                                                                                     Matri_Doc,
                                                                                     id_imagen,
                                                                                     tipo_documental,
                                                                                     id_tarea_wf,
                                                                                     radicado,
                                                                                     stru_datos_image_lista)
                    If Result <> "YES" Then
                        Adjunta_donumento_relacionado = "Imposible guardar el documento relacionado tipificado " & Result
                        Exit Function
                    End If
                End If

                If option_agrega_item_listview = 1 Then
                    Dim structure_datos_tarea_workflow As structure_datos_tarea_workflow = Nothing
                    Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
                    Result = ref_Class_DAT_ADIC_TAR.SolicitaDatosEstructuraBasicaTareaWorkflow(HttpContext.Current.Session("WF_RUTAWORKFLOW"),
                                                                                                    HttpContext.Current.Session("ID_TAREA_SELECCIONDA"),
                                                                                                    structure_datos_tarea_workflow)
                    If Result <> "YES" Then
                        Adjunta_donumento_relacionado = Result
                        Exit Function
                    End If
                    Dim Refclas_trd As New ClassTrdDocumental
                    Dim option_aplica_trd As Integer = 0
                    Dim ref_Class_system1 As New Class_system1
                    Result = ref_Class_system1.VerificaOpcionAplicarTablaRetencion(option_aplica_trd,
                                                                                       stru_datos_image_lista.nombre_gabinete)
                    If Result <> "YES" Then
                        Adjunta_donumento_relacionado = "#31 SELECCIONA-WF Imposible encontrar opción aplicar trd gabinete (" + stru_datos_image_lista.nombre_gabinete + ")"
                        Exit Function
                    End If
                    Dim ref_Class_configuracion_gabinete As New Class_configuracion_gabinete
                    Dim structure_gabinete_workflow As structure_gabinete_workflow = Nothing
                    Result = ref_Class_configuracion_gabinete.SolicitaDatosEstructuraGabineteWorkflow(structure_datos_tarea_workflow.ID_GABINETE,
                                                                                                          structure_gabinete_workflow)
                    If Result <> "YES" Then
                        Adjunta_donumento_relacionado = "#28 SELECCIONA-WF " & Result
                        Exit Function
                    End If
                    Dim Valor_Ceros_Imagen As String = "DIG"
                    Dim Valor_Ceros_Carpeta_Imagen As String = ""
                    Dim Valor_Disco_Imagen As String = ""
                    Dim Ruta_Imagen As String = ""
                    '----------------------------------------------
                    'Obteniendo la identidad de la imagen 
                    '----------------------------------------------
                    Result = Obtener_Ceros_Imagen(stru_datos_image_lista.id_imagen.ToString,
                                                  Valor_Ceros_Imagen)
                    If Result <> "YES" Then
                        Adjunta_donumento_relacionado = "#38 SELECCIONA-WF Error En la funcion Obtener ceros para la imagen=" + structure_datos_tarea_workflow.ID_IMAGEN + Result
                        Exit Function
                    End If
                    Valor_Ceros_Imagen = Valor_Ceros_Imagen & stru_datos_image_lista.extension
                    Dim ref_ClassDaGabinete As New ClassDaGabinete
                    Dim stru_paramter_image As stru_paramter_image = Nothing
                    Result = ref_ClassDaGabinete.SolicitaEtructuraImagenGabinete(structure_gabinete_workflow.NOMBRE_GABINETE,
                                                                                 stru_datos_image_lista.id_imagen,
                                                                                 stru_paramter_image,
                                                                                 option_aplica_trd)
                    If Result <> "YES" Then
                        Adjunta_donumento_relacionado = Result
                        Exit Function
                    End If
                    '--------------------------------------------------
                    'obtener la identidad de la carpeta
                    '--------------------------------------------------
                    Result = Obtener_Ceros_Carpeta_Imagen(stru_paramter_image.IDEX,
                                                          Valor_Ceros_Carpeta_Imagen)
                    If Result <> "YES" Then
                        Adjunta_donumento_relacionado = "Error en la funcion obtener ceros de la carpeta =" + stru_paramter_image.DISC + Result
                        Exit Function
                    End If
                    Dim Cod_Visor As String = ""
                    Dim Extension As String = ""
                    Dim Estado_Documento As String = ""
                    Dim Refclasvis As New Classactualizacionvisor
                    Dim ref_Class_da_extension As New Class_da_extension
                    Result = ref_Class_da_extension.Determina_tipo_documento_list(stru_paramter_image.DBT_TIPO_IMAGEN,
                                                                                  Cod_Visor,
                                                                                  Extension,
                                                                                  Estado_Documento)
                    If Result <> "YES" Then
                        Adjunta_donumento_relacionado = "#36 SELECCIONA-WF Error En la funcion determina_tipo_documento_list=" + Result
                        Exit Function
                    End If
                    '-----------------------------------------
                    'Consulta los id de la imagenes enlazadas
                    'al documento
                    '-----------------------------------------
                    Dim stru_paramter_image_enlace() As stru_paramter_image = Nothing
                    Result = ref_ClassDaGabinete.Solicita_lista_imagenes_enlzadas_gabinete(structure_gabinete_workflow.NOMBRE_GABINETE,
                                                                                           stru_paramter_image.ENLACE,
                                                                                           stru_paramter_image_enlace,
                                                                                           -1,
                                                                                           option_aplica_trd)
                    If Result <> "YES" Then
                        Adjunta_donumento_relacionado = Result
                        Exit Function
                    End If
                    '---------------------------------------------------
                    'Obtener carpeta cntenedora imagen enlace
                    '---------------------------------------------------
                    Valor_Disco_Imagen = structure_gabinete_workflow.NOMBRE_GABINETE & stru_paramter_image.DISC
                    Ruta_Imagen = structure_gabinete_workflow.RUTA_BUSQUEDA_IMAGEN & Valor_Disco_Imagen & "\" & Valor_Ceros_Carpeta_Imagen & "\" & Valor_Ceros_Imagen

                End If
                If file.Exists() Then
                    Kill(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA"))
                End If
                HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA") = ""
                HttpContext.Current.Session.Item("WF_ERROR_RESPUESTA") = ""
                Adjunta_donumento_relacionado = "YES"
                Exit Function
            Else
                Adjunta_donumento_relacionado = "Imposible encontrar el archivo " & HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA")
                Exit Function
            End If
        Catch ex As Exception
            Adjunta_donumento_relacionado = "Inconsistencia general función Adjunta_doumento_relacionado " & ex.Message
        End Try
    End Function
    Function Almacenamiento_documentos_load_enlace(ByVal datos As String,
                                                   ByVal tipo_almacenamiento_guarda_campo_radicado As Integer,
                                                   ByVal ruta_archivo As String,
                                                   ByVal id_tarea_seleecionada_enlace As Long,
                                                   ByVal tipo_almacenamiento As Integer,
                                                   ByRef id_imagen_almacen As Integer,
                                                   ByRef datos_image As stru_datos_image_lista) As String
        Try
            Dim Result As String = ""
            Dim refclas_ClassNeodynamic As New ClassNeodynamic
            Dim Matri_Documentos_Final() As String
            Erase Matri_Documentos_Final
            Dim file As New FileInfo(ruta_archivo)
            If UCase(file.Extension) = ".TIF" Then
                Result = refclas_ClassNeodynamic.Extraer_Documento_de_Multitif_fisico(ruta_archivo,
                                                                                      Matri_Documentos_Final,
                                                                                      HttpContext.Current.Session.Item("WF_RUTA_TEMPO_FINAL") & "\")
                If Result <> "YES" Then
                    Almacenamiento_documentos_load_enlace = "function Almacenamiento_documentos_load_enlace dice (Imposible extraer documento Multi tif " & Result & ")"
                    Exit Function
                End If
                If Matri_Documentos_Final Is Nothing Then
                    Almacenamiento_documentos_load_enlace = "function Almacenamiento_documentos_load_enlace dice (La matriz de multi tif es nothing) "
                    Exit Function
                End If
            Else
                ReDim Preserve Matri_Documentos_Final(0)
                Matri_Documentos_Final(0) = HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA")
            End If
            '----------------------------------------------------
            'Obtiene los datos de almacenamiento
            '----------------------------------------------------
            Dim matri_datos() As Datos_Almacenamiento
            Dim Gabinete As String = ""
            Dim Radicado As String = ""
            Dim Campo_Radicado As String = ""
            If datos <> "" Then
                Dim datos_enlace As String = Trim(datos)
                If datos_enlace = "" Then
                    Almacenamiento_documentos_load_enlace = "Enlace sin datos imposible almacenar"
                    Exit Function
                End If
                If InStr(datos_enlace, "POSITIVOQL_") < 1 Then
                    Almacenamiento_documentos_load_enlace = "Enlace sin datos correctos"
                    Exit Function
                End If
                Dim dat As String = datos_enlace.Replace("POSITIVOQL_", "<!#>")
                Dim Splipositvol() As String = dat.Split("<!#>")
                Dim SpliDATOS() As String = Splipositvol(1).Split("|")
                Dim SpliCAMPOS() As String = Splipositvol(2).Split("|")
                Gabinete = SpliDATOS(1)
                Radicado = SpliDATOS(2)
                Campo_Radicado = SpliCAMPOS(2)
                '---------------------------------------------
                'Remplaza las R000 del numero de radicado
                '---------------------------------------------
                Dim RadicTemporal As String = ""
                RadicTemporal = Radicado.Replace("R", "")
                Radicado = Val(RadicTemporal)
                ReDim Preserve matri_datos(0)
                matri_datos(0).nombre_campo = Campo_Radicado
                If tipo_almacenamiento_guarda_campo_radicado = 1 Then
                    matri_datos(0).valor_campo = Radicado
                Else
                    matri_datos(0).valor_campo = ""
                End If
                ReDim Preserve matri_datos(1)
                matri_datos(1).nombre_campo = "ENLASE"
                matri_datos(1).valor_campo = Radicado
            Else
                Gabinete = HttpContext.Current.Session.Item("DG_NOMBRE_GABINETE")
                Radicado = HttpContext.Current.Session.Item("DG_RADICADO")
                ReDim Preserve matri_datos(0)
                Dim nombre_campo_radicado_gabinete As String = ""
                Dim Ref_Class_DETALLE_GABIENETE As New Class_DETALLE_GABIENETE
                Result = Ref_Class_DETALLE_GABIENETE.SolicitaNombreCampoRadicadoGabinete(Gabinete,
                                                                                         nombre_campo_radicado_gabinete)
                If Result <> "YES" Then
                    Almacenamiento_documentos_load_enlace = Result
                    Exit Function
                End If
                matri_datos(0).nombre_campo = nombre_campo_radicado_gabinete
                If tipo_almacenamiento_guarda_campo_radicado = 1 Then
                    matri_datos(0).valor_campo = Radicado
                Else
                    matri_datos(0).valor_campo = ""
                End If
                ReDim Preserve matri_datos(1)
                matri_datos(1).nombre_campo = "ENLASE"
                matri_datos(1).valor_campo = Radicado
            End If
            '-----------------------------------------------
            'Retorna nombre ruta tarea
            '-----------------------------------------------
            Dim NombreRuta As String = ""
            Dim Refclas_workflow As New ClassWorkflow
            Dim Ref_class_ruta As New Class_worflow_rutas
            NombreRuta = HttpContext.Current.Session.Item("WF_RUTAWORKFLOW")
            '------------------------------------------------
            'Retorna si el tipo de tarea workflow es externa
            'Valores 1. Tarea interna    2. Tarea externa
            '------------------------------------------------
            Dim IdImagen As Integer = 0
            Dim Ref_dat_adic As New Class_DAT_ADIC_TAR
            Result = Ref_dat_adic.SolicitaidImagenTareaworkflow(id_tarea_seleecionada_enlace,
                                                                NombreRuta,
                                                                IdImagen)
            If Result <> "YES" Then
                Almacenamiento_documentos_load_enlace = Result
                Exit Function
            End If
            '------------------------------------------------
            'Retorna el nombre de la tabla de radicación
            'si el flujo se genero internamente desde
            'desde el radicador
            '-----------------------------------------------
            'Dim Refclasalmacena As New ClassAlmacenamiento
            Dim Nombre_plantilla_radicado As String = ""
            Dim id_expediente As Integer = 0
            Dim id_tipo_expediente As Integer = 0
            Dim nombre_expediente As String = ""
            Dim nombre_tipo_documento As String = ""
            Dim id_clase_documento As Integer = 0
            Dim fecha_elaboracion As String = ""
            Dim Refclas_radicado As New ClassRadicador
            Dim reflcas_tipo_documento As New ClassGaTipoDocumental
            Result = reflcas_tipo_documento.SolicitaIdTipoFormatoDocumento("DOCUMENTO DIGITALIZADO",
                                                                           id_clase_documento)
            If Result <> "YES" Then
                Almacenamiento_documentos_load_enlace = Result
                Exit Function
            Else
                nombre_tipo_documento = "DOCUMENTO DIGITALIZADO"
            End If
            Dim date1al As String = Date.Today
            Result = ""
            Dim ref_ClassGestionFechas As New ClassGestionFechas
            Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
            If Result <> "YES" Then
                Almacenamiento_documentos_load_enlace = "Error formateando fecha almacenamiento Funcion: Almacenamiento_documentos_load_enlace " & Result
                Exit Function
            Else
                fecha_elaboracion = date1al
            End If
            '------------------------------------------------
            'Asigna los datos de clasificación del documento
            '------------------------------------------------
            Dim IdTipoDocumento As Integer = 0
            Dim IdArea As Integer = 0
            Dim IdSerie As Integer = 0
            Dim IdSubSerie As Integer = 0
            Dim DescripcionTipoDocumento As String = ""
            Dim NombreArea As String = ""
            Dim NombreSerie As String = ""
            Dim NombreSubSerie As String = ""
            Dim Ref_ra_dig_tipos_docum_lista_chequeo As New ra_dig_tipos_docum_lista_chequeo
            If HttpContext.Current.Session.Item("DG_LISTA_CHEQUEO") <> -1 Then
                Dim stru As stru_tipo_lista_chequeo = Nothing
                Result = Ref_ra_dig_tipos_docum_lista_chequeo.SolicitaDatosTipoDocumentalListaChequeo(HttpContext.Current.Session.Item("DG_LISTA_CHEQUEO"),
                                                                                                           stru)
                If Result <> "YES" Then
                    Almacenamiento_documentos_load_enlace = Result
                    Exit Function
                End If
                If stru.subseries_documentales_Id_SubSeries <> 0 Then
                    IdTipoDocumento = stru.tipos_doc_subseries_Id_Tipos_Doc_SubSerie
                Else
                    IdTipoDocumento = stru.tipo_doc_series_Id_Tipo_Doc_Series
                End If
                '-----------------------------------------------
                'Retorna serie y sub serie tipo documento
                '-----------------------------------------------
                Dim stru_tipo As stru_tipo_documental = Nothing
                Dim ref_clas_trd As New ClassTrdDocumental
                Result = ref_clas_trd.Solicita_datos_estructura_tipo_documento(IdTipoDocumento,
                                                                               stru_tipo)
                If Result <> "YES" Then
                    Almacenamiento_documentos_load_enlace = Result
                    Exit Function
                End If
                IdSerie = stru_tipo.Series_Documentales_Id_Series
                IdSubSerie = stru_tipo.sub_serie_id_serie
                Dim ref_Class_series_documentales As New Class_series_documentales
                Result = ref_Class_series_documentales.Retorna_id_area_serie_documental(IdSerie,
                                                                                        IdArea)
                If Result <> "YES" Then
                    Almacenamiento_documentos_load_enlace = Result
                    Exit Function
                End If
                Dim ref_Class_ra_tipo_doc_series As New Class_ra_tipo_doc_series
                If IdTipoDocumento <> 0 Then
                    Result = ref_Class_ra_tipo_doc_series.Retorna_nombre_tipo_documental(IdSerie,
                                                                                         IdSubSerie,
                                                                                         IdTipoDocumento,
                                                                                         DescripcionTipoDocumento)
                    If Result <> "YES" Then
                        Almacenamiento_documentos_load_enlace = Result
                        Exit Function
                    End If
                End If
                If nombre_tipo_documento = "" Then
                    Result = reflcas_tipo_documento.SolicitaIdTipoFormatoDocumento("DOCUMENTO DIGITALIZADO",
                                                                                   id_clase_documento)
                    If Result <> "YES" Then
                        Almacenamiento_documentos_load_enlace = Result
                        Exit Function
                    Else
                        nombre_tipo_documento = "DOCUMENTO DIGITALIZADO"
                    End If
                End If
                Dim ref_Class_areas_depart_radicacion As New Class_areas_depart_radicacion
                If IdArea <> 0 Then
                    Result = ref_Class_areas_depart_radicacion.Retorna_nombre_area_por_id_area(IdArea,
                                                                                               NombreArea)
                    If Result <> "YES" Then
                        Almacenamiento_documentos_load_enlace = Result
                        Exit Function
                    End If
                End If
                If IdSerie <> 0 Then
                    Result = ref_Class_series_documentales.Retorna_nombre_serie_id_serie(IdSerie,
                                                                                         NombreSerie)
                    If Result <> "YES" Then
                        Almacenamiento_documentos_load_enlace = Result
                        Exit Function
                    End If
                End If
                Dim Class_subseries_documentales As New Class_subseries_documentales
                If IdSubSerie <> 0 Then
                    Result = Class_subseries_documentales.Retorna_nombre_sub_serie(IdSubSerie,
                                                                                    NombreSubSerie)
                    If Result <> "YES" Then
                        Almacenamiento_documentos_load_enlace = Result
                        Exit Function
                    End If
                End If
            End If
            Dim Ref_producion As New ClassGaProducionDocumental
            Dim inventario_documental As Integer = 0
            Dim aplica_trd As Integer = 0
            Dim asigna_unidad As Integer = 0
            Dim Ref_Class_system1 As New Class_system1
            Result = Ref_Class_system1.Verifica_opcion_aplicar_trd_inventario_unidad_documental_gabinete(Gabinete,
                                                                                                         inventario_documental,
                                                                                                         aplica_trd,
                                                                                                         asigna_unidad)
            If Result <> "YES" Then
                Almacenamiento_documentos_load_enlace = Result
                Exit Function
            End If
            If aplica_trd = 1 Then
                If nombre_tipo_documento = "" Then
                    Result = reflcas_tipo_documento.SolicitaIdTipoFormatoDocumento("DOCUMENTO DIGITALIZADO",
                                                                                   id_clase_documento)
                    If Result <> "YES" Then
                        Almacenamiento_documentos_load_enlace = Result
                        Exit Function
                    Else
                        nombre_tipo_documento = "DOCUMENTO DIGITALIZADO"
                    End If
                End If
            End If
            Dim matri_gestion As estructure_gestion = Nothing
            Dim ClassGaExpediente As New ClassGaExpediente
            Result = ClassGaExpediente.SolicitaEstructuraExpedienteDocumentoVinculante(matri_gestion,
                                                                                       Gabinete,
                                                                                       IdImagen,
                                                                                       id_tarea_seleecionada_enlace,
                                                                                       Radicado,
                                                                                       HttpContext.Current.Session("WF_RUTAWORKFLOW"),
                                                                                       HttpContext.Current.Session("Id_Ruta_Workflow"),
                                                                                       "")
            If Result <> "YES" Then
                Almacenamiento_documentos_load_enlace = Result
                Exit Function
            End If
            '-------------------------------------------
            'Asigna datos gestion
            '-------------------------------------------
            'Dim matri_gestion As estructure_gestion
            matri_gestion.CLASE_DOCUMENTO = nombre_tipo_documento
            'matri_gestion.EXPEDIENTE = nombre_expediente
            matri_gestion.ID_AREA = IdArea
            matri_gestion.ID_CLASE_DOCUMENTO = id_clase_documento
            'matri_gestion.ID_EXPEDIENTE = id_expediente
            matri_gestion.ID_SERIE = IdSerie
            matri_gestion.ID_SUB_SERIE = IdSubSerie
            'matri_gestion.ID_TIPO_EXPEDIENTE = id_tipo_expediente
            matri_gestion.ID_TIPO_UNIDAD_CONSERVACION = 0
            matri_gestion.ID_TIPODOCUMENTO = IdTipoDocumento
            matri_gestion.ID_UNIDAD_CONSERVACION = 0
            matri_gestion.ID_USUARIO_GESTION = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
            matri_gestion.TIPO_UNIDAD_DOCUMENTAL = 0
            matri_gestion.UNIDAD_CONSERVACION = ""
            matri_gestion.FECHA_ELABORACION = fecha_elaboracion
            matri_gestion.TIPODOCUMENTO = DescripcionTipoDocumento
            matri_gestion.NOMBRE_SERIE = NombreSerie
            matri_gestion.NOMBRE_SUB_SERIE = NombreSubSerie
            ReDim Preserve matri_datos(2)
            matri_datos(2).nombre_campo = "EXPEDIENTE"
            matri_datos(2).valor_campo = matri_gestion.EXPEDIENTE
            ReDim Preserve matri_datos(3)
            matri_datos(3).nombre_campo = "CLASEDOCUMENTO"
            matri_datos(3).valor_campo = nombre_tipo_documento
            ReDim Preserve matri_datos(4)
            matri_datos(4).nombre_campo = "FECHAELABORACION"
            matri_datos(4).valor_campo = fecha_elaboracion
            ReDim Preserve matri_datos(5)
            matri_datos(5).nombre_campo = "TIPODOCUMENTO"
            matri_datos(5).valor_campo = DescripcionTipoDocumento
            ReDim Preserve matri_datos(6)
            matri_datos(6).nombre_campo = "NOMBRESERIE"
            matri_datos(6).valor_campo = NombreSerie
            ReDim Preserve matri_datos(7)
            matri_datos(7).nombre_campo = "NOMBRESUBSERIE"
            matri_datos(7).valor_campo = NombreSubSerie
            '----------------------------------------------
            'Genera la matriz de datos de almacenamiento
            '----------------------------------------------
            Dim Refclas_Class_DETALLE_GABIENETE As New Class_DETALLE_GABIENETE
            Dim Matri_Datos_Almacen() As String
            Erase Matri_Datos_Almacen
            Result = Refclas_Class_DETALLE_GABIENETE.SolicitaValoresCamposDocumentoGabinete(Matri_Datos_Almacen,
                                                                                            Gabinete,
                                                                                            matri_datos)
            If Result <> "YES" Then
                Almacenamiento_documentos_load_enlace = "Imposible general mariz datos almacenamiento "
                Exit Function
            End If
            If Matri_Datos_Almacen Is Nothing Then
                Almacenamiento_documentos_load_enlace = "Matriz de datos de almacenamiento es nothing "
                Exit Function
            End If
            '----------------------------------------------
            'Obtiene el tipo documento 
            '----------------------------------------------
            Dim Tipo_Documento As Integer = -1
            Dim Refclasvisor As New Classactualizacionvisor
            Result = ""
            Dim filinf As New FileInfo(Matri_Documentos_Final(0))
            Dim Class_da_extension As New Class_da_extension
            Result = Class_da_extension.SolicitaTipoArchivoDocuarchiExtension(filinf.Extension,
                                                                          Tipo_Documento)
            If Result <> "YES" Then
                Almacenamiento_documentos_load_enlace = Result
                Exit Function
            End If
            '-----------------------------------------------
            'Almacena documento en la base docuarchi.net
            '-----------------------------------------------
            Dim UserWf As String = "WF-" & HttpContext.Current.Session.Item("Login_Usuario_Workfow")
            Result = ""
            Result = Me.Almacenamiento("", "", Gabinete, 0, Matri_Datos_Almacen, 2,
            Matri_Documentos_Final.Length, Tipo_Documento, Matri_Documentos_Final, 0, id_imagen_almacen,
            Tipo_Documento, HttpContext.Current.Session.Item("GA_IDEMPRESA"),
            HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), matri_gestion.ID_AREA,
            matri_gestion.ID_SERIE, matri_gestion.ID_SUB_SERIE,
            matri_gestion.ID_TIPODOCUMENTO, matri_gestion.ID_EXPEDIENTE, matri_gestion.ID_TIPO_EXPEDIENTE,
            matri_gestion.ID_UNIDAD_CONSERVACION, matri_gestion.ID_TIPO_UNIDAD_CONSERVACION,
            matri_gestion.ID_CLASE_DOCUMENTO, matri_gestion.EXPEDIENTE, matri_gestion.NOMBRE_SERIE,
            matri_gestion.NOMBRE_SUB_SERIE, matri_gestion.TIPODOCUMENTO, matri_gestion.UNIDAD_CONSERVACION,
            matri_gestion.CLASE_DOCUMENTO, matri_gestion.FECHA_ELABORACION, Radicado, "RAD-" & Radicado, 0, 0, 0, id_tarea_seleecionada_enlace,
            HttpContext.Current.Session.Item("Id_Ruta_Workflow"))
            If Result <> "YES" Then
                Almacenamiento_documentos_load_enlace = Result
                Exit Function
            End If
            Dim attrNodeGru1 As New TreeNode
            Dim val_documento As String = matri_gestion.TIPODOCUMENTO
            If val_documento = "" Then
                val_documento = "D-" & id_imagen_almacen
            End If
            If DescripcionTipoDocumento = "" Then
                DescripcionTipoDocumento = val_documento
            End If
            Dim classgabinete As New ClassDaGabinete
            Dim icono As String = ""
            classgabinete.SolicitaIconoImageFownt(Tipo_Documento.ToString,
                                                   icono)
            datos_image.nombre_gabinete = Gabinete
            datos_image.id_imagen = id_imagen_almacen
            datos_image.radicado = Radicado
            datos_image.tipodocumental = DescripcionTipoDocumento
            datos_image.notipodocumento = val_documento
            datos_image.extension = UCase(filinf.Extension)
            datos_image.icono_icono_awe_some = icono
            '-----------------------------------------------
            'Elimina los documentos almacenados  
            '-----------------------------------------------
            For k As Integer = 0 To Matri_Documentos_Final.Length - 1
                If System.IO.File.Exists(Matri_Documentos_Final(k)) = True Then
                    System.IO.File.Delete(Matri_Documentos_Final(k))
                End If
            Next
            If tipo_almacenamiento <> 0 Then
                If System.IO.File.Exists(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA")) = True Then
                    Kill(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA"))
                End If
                HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA") = ""
                HttpContext.Current.Session.Item("WF_ERROR_RESPUESTA") = ""
            End If
            Almacenamiento_documentos_load_enlace = "YES"
        Catch ex As Exception
            Almacenamiento_documentos_load_enlace = "Inconsistencia general funcion Almacenamiento_documentos_load_enlace (" & ex.Message & ")"
        End Try
    End Function

    Function Almacenamiento_migra_sii(ByVal gabinete As String,
                                      ByVal matricula As String,
                                      ByVal recibo As String,
                                      ByVal codigo_sii As String,
                                      ByVal libro_sii As String,
                                      ByVal inscripcion As String,
                                      ByVal razon_social As String,
                                      ByVal descripcion As String,
                                      ByVal nit_identificacion As String,
                                      ByVal fecha_documento As String,
                                      ByVal ruta_documento As String,
                                      ByRef id_imagen As Integer) As String
        Try
            Dim Result As String = ""
            Dim refclas_ClassNeodynamic As New ClassNeodynamic
            Dim Matri_Documentos_Final() As String
            Erase Matri_Documentos_Final
            Dim file As New FileInfo(ruta_documento)
            If UCase(file.Extension) = ".TIF" Then
                Result = refclas_ClassNeodynamic.Extraer_Documento_de_Multitif_fisico(ruta_documento,
                                                                                      Matri_Documentos_Final,
                                                                                      HttpContext.Current.Session.Item("WF_RUTA_TEMPO_FINAL") & "\")
                If Result <> "YES" Then
                    Almacenamiento_migra_sii = "function Almacenamiento_migra_sii dice (Imposible extraer documento Multi tif " & Result & ")"
                    Exit Function
                End If
                If Matri_Documentos_Final Is Nothing Then
                    Almacenamiento_migra_sii = "function Almacenamiento_documentos_load_enlace dice (La matriz de multi tif es nothing) "
                    Exit Function
                End If
            Else
                ReDim Preserve Matri_Documentos_Final(0)
                Matri_Documentos_Final(0) = ruta_documento
            End If
            Dim matri_datos() As Datos_Almacenamiento
            Dim inventario_documental As Integer = 0
            Dim aplica_trd As Integer = 0
            Dim asigna_unidad As Integer = 0
            Dim Ref_Class_system1 As New Class_system1
            Dim id_clase_documento As Integer = 0
            Dim nombre_tipo_documento As String = ""
            Dim id_tipo_documento As Integer = 0
            Dim id_area As Integer = 0
            Dim id_serie As Integer = 0
            Dim id_sub_serie As Integer = 0
            Dim descripcion_tipo_documento As String = ""
            Dim nombre_area As String = ""
            Dim nombre_serie As String = ""
            Dim nombre_sub_serie As String = ""
            Dim Refclasalmacena As New ClassAlmacenamiento
            Dim Nombre_plantilla_radicado As String = ""
            Dim id_expediente As Integer = 0
            Dim id_tipo_expediente As Integer = 0
            Dim nombre_expediente As String = ""
            Dim fecha_elaboracion As String = ""
            Dim Refclas_radicado As New ClassRadicador
            Dim reflcas_tipo_documento As New ClassGaTipoDocumental
            Result = Ref_Class_system1.Verifica_opcion_aplicar_trd_inventario_unidad_documental_gabinete(gabinete,
                                                                                                         inventario_documental,
                                                                                                         aplica_trd,
                                                                                                         asigna_unidad)
            If Result <> "YES" Then
                Almacenamiento_migra_sii = Result
                Exit Function
            End If
            If aplica_trd = 1 Then
                If nombre_tipo_documento = "" Then
                    Result = reflcas_tipo_documento.SolicitaIdTipoFormatoDocumento("DOCUMENTO DIGITALIZADO",
                                                                                  id_clase_documento)
                    If Result <> "YES" Then
                        Almacenamiento_migra_sii = Result
                        Exit Function
                    Else
                        nombre_tipo_documento = "DOCUMENTO DIGITALIZADO"
                    End If
                End If
            End If

            '-------------------------------------------
            'Asigna datos gestion
            '-------------------------------------------
            Dim matri_gestion As estructure_gestion
            matri_gestion.CLASE_DOCUMENTO = nombre_tipo_documento
            matri_gestion.EXPEDIENTE = nombre_expediente
            matri_gestion.ID_AREA = id_area
            matri_gestion.ID_CLASE_DOCUMENTO = id_clase_documento
            matri_gestion.ID_EXPEDIENTE = id_expediente
            matri_gestion.ID_SERIE = id_serie
            matri_gestion.ID_SUB_SERIE = id_sub_serie
            matri_gestion.ID_TIPO_EXPEDIENTE = id_tipo_expediente
            matri_gestion.ID_TIPO_UNIDAD_CONSERVACION = 0
            matri_gestion.ID_TIPODOCUMENTO = id_tipo_documento
            matri_gestion.ID_UNIDAD_CONSERVACION = 0
            matri_gestion.ID_USUARIO_GESTION = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
            matri_gestion.TIPO_UNIDAD_DOCUMENTAL = 0
            matri_gestion.UNIDAD_CONSERVACION = ""
            matri_gestion.FECHA_ELABORACION = fecha_elaboracion
            matri_gestion.TIPODOCUMENTO = descripcion_tipo_documento
            matri_gestion.NOMBRE_SERIE = nombre_serie
            matri_gestion.NOMBRE_SUB_SERIE = nombre_sub_serie
            ReDim Preserve matri_datos(0)
            matri_datos(0).nombre_campo = "MATRICULA"
            matri_datos(0).valor_campo = matricula
            ReDim Preserve matri_datos(1)
            matri_datos(1).nombre_campo = "RAZONSOCIAL"
            matri_datos(1).valor_campo = razon_social
            ReDim Preserve matri_datos(2)
            matri_datos(2).nombre_campo = "NITCEDULA"
            matri_datos(2).valor_campo = nit_identificacion
            ReDim Preserve matri_datos(3)
            matri_datos(3).nombre_campo = "LIBRO"
            matri_datos(3).valor_campo = libro_sii
            ReDim Preserve matri_datos(4)
            matri_datos(4).nombre_campo = "INSCRIPCION"
            matri_datos(4).valor_campo = inscripcion
            ReDim Preserve matri_datos(5)
            matri_datos(5).nombre_campo = "RECIBOCAJA"
            matri_datos(5).valor_campo = recibo
            ReDim Preserve matri_datos(6)
            matri_datos(6).nombre_campo = "CODBARRAS"
            matri_datos(6).valor_campo = codigo_sii
            ReDim Preserve matri_datos(7)
            matri_datos(7).nombre_campo = "EXPEDIENTE"
            matri_datos(7).valor_campo = nombre_expediente
            ReDim Preserve matri_datos(8)
            matri_datos(8).nombre_campo = "CLASEDOCUMENTO"
            matri_datos(8).valor_campo = nombre_tipo_documento
            ReDim Preserve matri_datos(9)
            matri_datos(9).nombre_campo = "FECHAELABORACION"
            matri_datos(9).valor_campo = fecha_elaboracion
            ReDim Preserve matri_datos(10)
            matri_datos(10).nombre_campo = "TIPODOCUMENTO"
            matri_datos(10).valor_campo = descripcion_tipo_documento
            ReDim Preserve matri_datos(11)
            matri_datos(11).nombre_campo = "NOMBRESERIE"
            matri_datos(11).valor_campo = nombre_serie
            ReDim Preserve matri_datos(12)
            matri_datos(12).nombre_campo = "NOMBRESUBSERIE"
            matri_datos(12).valor_campo = nombre_sub_serie
            ReDim Preserve matri_datos(13)
            matri_datos(13).nombre_campo = "DESCRIPCION"
            matri_datos(13).valor_campo = descripcion
            If gabinete = "ESAL" Then
                matri_datos(13).nombre_campo = "DESCRIPCIONT"
            End If
            If gabinete = "RUP" Then
                matri_datos(13).nombre_campo = "DESCRIPCIONT"
            End If
            '----------------------------------------------
            'Genera la matriz de datos de almacenamiento
            '----------------------------------------------
            Dim Refclas_Class_DETALLE_GABIENETE As New Class_DETALLE_GABIENETE
            Dim Matri_Datos_Almacen() As String
            Erase Matri_Datos_Almacen
            Result = Refclas_Class_DETALLE_GABIENETE.SolicitaValoresCamposDocumentoGabinete(Matri_Datos_Almacen,
                                                                                                gabinete,
                                                                                                matri_datos)
            If Result <> "YES" Then
                Almacenamiento_migra_sii = "Imposible general mariz datos almacenamiento "
                Exit Function
            End If
            If Matri_Datos_Almacen Is Nothing Then
                Almacenamiento_migra_sii = "Matriz de datos de almacenamiento es nothing "
                Exit Function
            End If
            '----------------------------------------------
            'Obtiene el tipo documento 
            '----------------------------------------------
            Dim Tipo_Documento As Integer = -1
            Dim Refclasvisor As New Classactualizacionvisor
            Result = ""
            Dim filinf As New FileInfo(Matri_Documentos_Final(0))
            Dim Class_da_extension As New Class_da_extension
            Result = Class_da_extension.SolicitaTipoArchivoDocuarchiExtension(filinf.Extension,
                                                                          Tipo_Documento)
            If Result <> "YES" Then
                Almacenamiento_migra_sii = Result
                Exit Function
            End If

            '-----------------------------------------------
            'Almacena documento en la base docuarchi.net
            '-----------------------------------------------
            Result = ""
            Result = Refclasalmacena.Almacenamiento("", "", gabinete, 0, Matri_Datos_Almacen, 2,
            Matri_Documentos_Final.Length, Tipo_Documento, Matri_Documentos_Final, 0, id_imagen,
            Tipo_Documento, HttpContext.Current.Session.Item("GA_IDEMPRESA"),
            HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), matri_gestion.ID_AREA,
            matri_gestion.ID_SERIE, matri_gestion.ID_SUB_SERIE,
            matri_gestion.ID_TIPODOCUMENTO, matri_gestion.ID_EXPEDIENTE, matri_gestion.ID_TIPO_EXPEDIENTE,
            matri_gestion.ID_UNIDAD_CONSERVACION, matri_gestion.ID_TIPO_UNIDAD_CONSERVACION,
            matri_gestion.ID_CLASE_DOCUMENTO, matri_gestion.EXPEDIENTE, matri_gestion.NOMBRE_SERIE,
            matri_gestion.NOMBRE_SUB_SERIE, matri_gestion.TIPODOCUMENTO, matri_gestion.UNIDAD_CONSERVACION,
            matri_gestion.CLASE_DOCUMENTO, matri_gestion.FECHA_ELABORACION)
            If Result <> "YES" Then
                Almacenamiento_migra_sii = Result
                Exit Function
            End If
            '-----------------------------------------------
            'Elimina los documentos almacenados  
            '-----------------------------------------------
            For k As Integer = 0 To Matri_Documentos_Final.Length - 1
                If System.IO.File.Exists(Matri_Documentos_Final(k)) = True Then
                    System.IO.File.Delete(Matri_Documentos_Final(k))
                End If
            Next
            Almacenamiento_migra_sii = "YES"
            Exit Function
        Catch ex As Exception
            Almacenamiento_migra_sii = "Inconsistencia general funcion Almacenamiento_migra_sii " & ex.Message
        End Try
    End Function
    Function SolicitaEstructuraExpedienteVinculanteIntegracionSII(ByVal NombreGabinete As String,
                                                                  ByVal IdTareaWorkflow As Long,
                                                                  ByVal Radicado As String,
                                                                  ByVal IdRutaWorkflow As Integer,
                                                                  ByVal Matricula As String,
                                                                  ByRef EstructuraGestion As estructure_gestion) As String
        Try
            Dim Result As String = ""
            Dim ClassRaRelacionRadicadoExternoExpediente As New ClassRaRelacionRadicadoExternoExpediente
            Dim Class_ra_rel_copia_wf_produccion As New Class_ra_rel_copia_wf_produccion
            Dim ClassGaExpediente As New ClassGaExpediente
            Dim ClassRaSIiCacheExpediente As New ClassRaSIiCacheExpediente
            EstructuraGestion.ID_EXPEDIENTE = 0
            EstructuraGestion.ID_TIPO_EXPEDIENTE = 0
            EstructuraGestion.EXPEDIENTE = ""
            Dim IdTipoTarea As Integer = 2 '//-----------Tarea externa-----------//
            Dim IdExpediente As Integer = 0
            Dim CStruSiiCahcheExpediente As New CStruSiiCahcheExpediente
            Result = ClassRaSIiCacheExpediente.SolicitaCacheCreacionExpedienteSII(Matricula,
                                                                                  NombreGabinete,
                                                                                  CStruSiiCahcheExpediente)
            If Result <> "YES" Then
                SolicitaEstructuraExpedienteVinculanteIntegracionSII = Result
                Exit Function
            End If
            If Not CStruSiiCahcheExpediente.RadicadoSII Is Nothing Then
                IdExpediente = CStruSiiCahcheExpediente.IdExpediente
            Else
                Result = ClassRaRelacionRadicadoExternoExpediente.SolicitaExpedienteRadicadoExterno(Radicado,
                                                                                                    IdExpediente)
                If Result <> "YES" Then
                    SolicitaEstructuraExpedienteVinculanteIntegracionSII = Result
                    Exit Function
                End If
                If IdExpediente <> 0 Then
                    Result = Class_ra_rel_copia_wf_produccion.SolicitaUltimaRelacionExpedienteIdTareaWorkflow(IdTareaWorkflow,
                                                                                                              IdRutaWorkflow,
                                                                                                              IdExpediente)
                    If Result <> "YES" Then
                        SolicitaEstructuraExpedienteVinculanteIntegracionSII = Result
                        Exit Function
                    End If
                End If
            End If
            If IdExpediente <> 0 Then
                Result = ClassGaExpediente.Solicita_datos_expediente_relacion(IdExpediente,
                                                                              EstructuraGestion)
                If Result <> "YES" Then
                    SolicitaEstructuraExpedienteVinculanteIntegracionSII = Result
                    Exit Function
                End If
            End If
            SolicitaEstructuraExpedienteVinculanteIntegracionSII = "YES"
        Catch ex As Exception
            SolicitaEstructuraExpedienteVinculanteIntegracionSII = "Inconsistencia general funcion SolicitaEstructuraExpedienteVinculanteIntegracionSII " & ex.Message
        End Try
    End Function

    Function Almacenar_documento_respuesta(ByVal id_registro_respuesta As Integer,
                                           ByVal file_firma As String,
                                           ByVal opcion_firma_digital As String,
                                           ByVal pasword_firma_digital As String,
                                           ByVal file_digital_archivo As String,
                                           ByVal estru As stru_envio,
                                           ByVal id_usuario_firma_respuesta As Integer,
                                           ByVal id_usuario_gestion As Integer,
                                           ByRef matri_documento_almacenar() As String) As String
        Try
            Dim Ref_class_config_listado As New Class_configuracion_listado_ruta
            Dim Ref_calss_wf_ruta As New Class_worflow_rutas
            Dim rfclas_gembox As New ClassGaGembox
            Dim Refalmacena As New ClassAlmacenamiento
            Dim Class_da_extension As New Class_da_extension
            Dim Refcals_dat_ruta As New Class_DAT_ADIC_TAR
            Dim ref_almacenaminento As New ClassVisualisaDocumento
            Dim Result As String = ""
            If estru.ID_IMAGEN_RESPUESTA <> 0 Then
                Almacenar_documento_respuesta = "El documento ya tiene una respuesta permanente, imposible continuar"
                Exit Function
            End If
            If estru.FECHA_RESPUETA = "" And estru.RADICADO_RESPUESTA = "" Then
                Almacenar_documento_respuesta = "Debe general un radicado de respuesta para guardar el documento de respuesta permanente"
                Exit Function
            End If
            If estru.ID_IMAGEN = 0 Then
                Almacenar_documento_respuesta = "Debe cargar el documento para dar respuesta a la solicitud"
                Exit Function
            End If
            Dim matridatos_almacenamiento() As String = Nothing
            Dim matri_gestion As estructure_gestion = Nothing
            'Dim matri_documentos_almacenar() As String = Nothing
            Dim Id_imagen_padre As Integer = 0
            Dim nombre_gabinete_padre As String = ""
            Dim NombreRuta As String = ""
            Result = Ref_calss_wf_ruta.Solicita_nombre_ruta_workflow(HttpContext.Current.Session.Item("Id_Ruta_Workflow").ToString,
                                                                     NombreRuta)
            If Result <> "YES" Then
                Almacenar_documento_respuesta = Result
                Exit Function
            End If
            If NombreRuta = "" Then
                Almacenar_documento_respuesta = "Imposible Econtrar Nombre de la ruta " & Result
                Exit Function
            End If
            Dim campo_radicado As String = ""
            Result = Ref_class_config_listado.SolicitaNombreCampoRadicadoRuta(HttpContext.Current.Session.Item("Id_Ruta_Workflow"),
                                                                              campo_radicado)
            If Result <> "YES" Then
                Almacenar_documento_respuesta = Result
                Exit Function
            End If
            Result = Refcals_dat_ruta.Solicita_id_imagen_gabinete_seleccionada(estru.RADICADO,
                                                                              HttpContext.Current.Session.Item("Id_Ruta_Workflow"),
                                                                              campo_radicado,
                                                                               NombreRuta,
                                                                              Id_imagen_padre,
                                                                              nombre_gabinete_padre)
            If Result <> "YES" Then
                Almacenar_documento_respuesta = Result
                Exit Function
            End If
            Dim matri_documentos_almacenados() As String = Nothing
            Result = ref_almacenaminento.Genera_Matris_Documentos_Almacenados(estru.ID_IMAGEN,
                                                                              "IMP03GESTIONTMP",
                                                                              matri_documentos_almacenados)
            If Result <> "YES" Then
                Almacenar_documento_respuesta = Result
                Exit Function
            End If
            If File.Exists(matri_documentos_almacenados(1)) = False Then
                Almacenar_documento_respuesta = "Imposible econtrar el documento respuesta en el gabinete " & matri_documentos_almacenados(1)
                Exit Function
            End If
            Dim documento_respuesta As String = ""
            Result = ""
            Dim archivo_formato_respuesta As String = ""
            If estru.ID_TIPO_DOC_RESPUESTA = 1 Then
                '--------------------------------------------------------------
                'Remplaza el contenido del documento en la plantilla
                'del fotter archivo plantillalibre_web_contendor_Footers.docx
                '--------------------------------------------------------------
                Result = rfclas_gembox.Solicita_formato_respuesta_con_Footers(id_registro_respuesta,
                                                                              matri_documentos_almacenados(1),
                                                                              documento_respuesta)
                If Result <> "YES" Then
                    Almacenar_documento_respuesta = Result
                    Exit Function
                End If
                Result = rfclas_gembox.Firma_documento_formato_respuesta(documento_respuesta,
                                                                         opcion_firma_digital,
                                                                         pasword_firma_digital,
                                                                         file_digital_archivo,
                                                                         id_usuario_firma_respuesta,
                                                                         id_usuario_gestion,
                                                                         1,
                                                                         "PDF",
                                                                         archivo_formato_respuesta)
                If Result <> "YES" Then
                    Almacenar_documento_respuesta = Result
                    Exit Function
                End If
                matri_documentos_almacenados(1) = archivo_formato_respuesta
            End If
            Dim Classgestionrespuesta As New Classgestionrespuesta
            Result = Classgestionrespuesta.Solicita_parametros_almacenamiento_documento_enexo_rad_respuesta(Id_imagen_padre,
                                                                                                            nombre_gabinete_padre,
                                                                                                            estru,
                                                                                                            "RESPUESTA",
                                                                                                            matri_gestion,
                                                                                                            matridatos_almacenamiento)

            If Result <> "YES" Then
                Almacenar_documento_respuesta = Result
                Exit Function
            End If
            Erase matri_documento_almacenar
            For i As Integer = 1 To matri_documentos_almacenados.Length - 1
                ReDim Preserve matri_documento_almacenar(i - 1)
                matri_documento_almacenar(i - 1) = matri_documentos_almacenados(i)
            Next

            Dim Tipo_Doc_int As Integer = -1
            Dim Filein As New FileInfo(matri_documento_almacenar(0))
            Result = Class_da_extension.SolicitaTipoArchivoDocuarchiExtension(UCase(Filein.Extension),
                                                                         Tipo_Doc_int)
            If Result <> "YES" Then
                Almacenar_documento_respuesta = "Imposible determinar el tipo de documento " & Result
                Exit Function
            End If
            Dim ClassGaExpediente As New ClassGaExpediente
            Result = ClassGaExpediente.SolicitaEstructuraExpedienteDocumentoVinculante(matri_gestion,
                                                                                       nombre_gabinete_padre,
                                                                                       Id_imagen_padre,
                                                                                       HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                       estru.RADICADO,
                                                                                       HttpContext.Current.Session("WF_RUTAWORKFLOW"),
                                                                                       HttpContext.Current.Session("Id_Ruta_Workflow"),
                                                                                       "")
            If Result <> "YES" Then
                Almacenar_documento_respuesta = Result
                Exit Function
            End If
            Dim id_imagen = 0
            Result = Me.Almacenamiento("", "", nombre_gabinete_padre, 0, matridatos_almacenamiento,
            2, matri_documento_almacenar.Length, Tipo_Doc_int, matri_documento_almacenar, 0, id_imagen, Tipo_Doc_int,
            HttpContext.Current.Session.Item("GA_IDEMPRESA"), HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
            matri_gestion.ID_AREA, matri_gestion.ID_SERIE, matri_gestion.ID_SUB_SERIE,
            matri_gestion.ID_TIPODOCUMENTO, matri_gestion.ID_EXPEDIENTE, matri_gestion.ID_TIPO_EXPEDIENTE,
            matri_gestion.ID_UNIDAD_CONSERVACION, matri_gestion.ID_TIPO_UNIDAD_CONSERVACION,
            matri_gestion.ID_CLASE_DOCUMENTO, matri_gestion.EXPEDIENTE, matri_gestion.NOMBRE_SERIE,
            matri_gestion.NOMBRE_SUB_SERIE, matri_gestion.TIPODOCUMENTO, matri_gestion.UNIDAD_CONSERVACION,
            matri_gestion.CLASE_DOCUMENTO, matri_gestion.FECHA_ELABORACION, estru.RADICADO)
            If Result <> "YES" Then
                Almacenar_documento_respuesta = Result
                Exit Function
            Else
                '----------------------------------------------------------
                'Actualiza el estado del codigo del documento docuarchi
                '----------------------------------------------------------
                Dim SQL As String = "Update ra_respuesta_radicado set ID_IMAGEN_RESPUESTA=" & id_imagen &
                " where ID_RESPUESTA_RADICADO=" & id_registro_respuesta
                Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
                Result = Ref_Car_Conec.SELECTION_INSERT_COMMAND(SQL)
                If Result <> "YES" Then
                    Almacenar_documento_respuesta = "Inconsistencia actualizando id documento " & Result
                    Exit Function
                End If
                Dim ClassDaGabinete As New ClassDaGabinete
                Dim ruta_archivo As String = ""
                Result = ClassDaGabinete.Solicita_ruta_documento_gabinete(id_imagen,
                                                                          nombre_gabinete_padre,
                                                                          ruta_archivo)
                If Result <> "YES" Then
                    Almacenar_documento_respuesta = Result
                    Exit Function
                End If
                Erase matri_documento_almacenar
                ReDim Preserve matri_documento_almacenar(0)
                matri_documento_almacenar(0) = ruta_archivo
                Almacenar_documento_respuesta = "YES"
                Exit Function
            End If
            Almacenar_documento_respuesta = "YES"
            Exit Function
        Catch ex As Exception
            Almacenar_documento_respuesta = "Inconsistencia función Almacena_documento_respuesta_permanente " & ex.Message
        End Try
    End Function
End Class
