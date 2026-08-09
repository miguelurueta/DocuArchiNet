Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports GestionDocumental_Docuarchi.net.conect
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.OleDb
Imports System.Data.Odbc
Imports MySql.Data.MySqlClient
Imports System.Web.Http
Imports System.Web.Script.Serialization
Imports Dynamsoft.DotNet.TWAIN.Barcode
Imports Newtonsoft.Json
Imports System.IO

Public Class class_valida_peticionario
    Public Error_valida As String
    Public nombre_campo_error As String
    Public primary_peticionario As Integer
    Public valor_campo_error As String
End Class
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
Public Class WebServiceRadicacion

    Inherits System.Web.Services.WebService
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function ServiceSolicitaEstructuraTramiteAsignado(ByVal Parameter As Object) As Object
        '-----------------------------------------------------------------------------------------------
        'Funcion : Servicio que expone la estrucutura de un tramite de radicación cuando esta asignado
        '          a gestión de documentos
        '          
        '          
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        '-----------------------------------------------------------------------------------------------
        'IdTipoTramite       : Representa la identificación del tipo tramite
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        '
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-08-18
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------------------
        Dim ListCDRadicacion = New List(Of CDRadicacion)
        Dim CDRadicacion As CDRadicacion = New CDRadicacion()
        Try
            Dim Class_ra_dig_config_digitalizacion As New Class_ra_dig_config_digitalizacion
            Dim Result As String = ""
            Dim CDRAsginaGestionDocumento = New CDRAsginaGestionDocumentos

            CDRadicacion.AppError = Class_ra_dig_config_digitalizacion.Solicita_id_configuracion_digitalizacion(Session.Item("DG_ID_TRAMITE"),
                                                                                                                Session.Item("DG_ID_TRAMITE"),
                                                                                                                Session.Item("DG_ID_CONFIG_DIGITALIZACION"),
                                                                                                                0)
            CDRAsginaGestionDocumento.IdTipoTramite = Session.Item("DG_ID_TRAMITE")
            CDRAsginaGestionDocumento.TipoPlantillaTramite = Session.Item("DG_ID_TRAMITE")
            CDRAsginaGestionDocumento.IconfigDigitalizacion = Session.Item("DG_ID_CONFIG_DIGITALIZACION")
            CDRadicacion.CDRAsginaGestionDocumentos.Add(CDRAsginaGestionDocumento)
            ListCDRadicacion.Add(CDRadicacion)
            Return ListCDRadicacion
        Catch ex As Exception
            CDRadicacion.AppError = ex.Message
            ListCDRadicacion.Add(CDRadicacion)
            Return ListCDRadicacion
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_Valida_exitencia_usuario_peticionario(ByVal parameter As Object)
        Dim return_function As New List(Of class_valida_peticionario)
        Try
            Dim deserialize_parameter = Nothing
            Dim serializer = New JavaScriptSerializer()
            deserialize_parameter = serializer.Deserialize(Of List(Of CAMPOS_PLANTILLA_VALIDACION_PQR))(parameter)
            If deserialize_parameter Is Nothing Then
                Return "Imposible deserealizar los parametros de configuracion"
                Exit Function
            End If
            Dim stru_campos_docuarchi() As CAMPOS_PLANTILLA_VALIDACION_PQR = Nothing
            For i As Integer = 0 To deserialize_parameter.count - 1
                ReDim Preserve stru_campos_docuarchi(i)
                stru_campos_docuarchi(i).Nombre_Campo = deserialize_parameter(i).Nombre_Campo
                stru_campos_docuarchi(i).Tipo_Campo = deserialize_parameter(i).Tipo_Campo
                stru_campos_docuarchi(i).TEXTO_CAMPO_MODIFICADO = deserialize_parameter(i).TEXTO_CAMPO
            Next
            Dim Result As String = ""
            Dim nombre_campo_error As String = ""
            Dim primary_peticionario As Integer = 0
            Dim valor_campo_error As String = ""
            Dim Class_plantilla_validacion As New Class_plantilla_validacion
            Result = Class_plantilla_validacion.Valida_exitencia_usuario_peticionario(HttpContext.Current.Session.Item("PQRS_CODIGO_SCRIPT_PLANTILLA"),
                                                                                      stru_campos_docuarchi,
                                                                                      primary_peticionario,
                                                                                      nombre_campo_error,
                                                                                      valor_campo_error)
            If Result <> "YES" Then
                Dim item As New class_valida_peticionario
                item.Error_valida = Result
                item.nombre_campo_error = nombre_campo_error
                item.primary_peticionario = primary_peticionario
                Session.Item("PQRS_ID_USUARIO_PQRS") = primary_peticionario
                item.valor_campo_error = valor_campo_error
                return_function.Add(item)
                Return return_function
            Else
                Dim item As New class_valida_peticionario
                item.Error_valida = Result
                item.nombre_campo_error = nombre_campo_error
                item.primary_peticionario = primary_peticionario
                Session.Item("PQRS_ID_USUARIO_PQRS") = primary_peticionario
                item.valor_campo_error = valor_campo_error
                return_function.Add(item)
                Return return_function
            End If
        Catch ex As Exception
            Dim item As New class_valida_peticionario
            item.Error_valida = ex.Message
            item.nombre_campo_error = ""
            item.primary_peticionario = 0
            Session.Item("PQRS_ID_USUARIO_PQRS") = 0
            return_function.Add(item)
            Return return_function
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_registra_actualiza_plantilla_usuario_externo(ByVal parameter As Object)
        Try
            Dim deserialize_parameter = Nothing
            Dim serializer = New JavaScriptSerializer()
            deserialize_parameter = serializer.Deserialize(Of List(Of CAMPOS_PLANTILLA_VALIDACION_PQR))(parameter)
            If deserialize_parameter Is Nothing Then
                Return "Imposible deserealizar los parametros de configuracion"
                Exit Function
            End If
            Dim stru_campos_docuarchi() As CAMPOS_PLANTILLA_VALIDACION_PQR = Nothing
            For i As Integer = 0 To deserialize_parameter.count - 1
                ReDim Preserve stru_campos_docuarchi(i)
                stru_campos_docuarchi(i).Nombre_Campo = deserialize_parameter(i).Nombre_Campo
                stru_campos_docuarchi(i).Tipo_Campo = deserialize_parameter(i).Tipo_Campo
                stru_campos_docuarchi(i).TEXTO_CAMPO_MODIFICADO = deserialize_parameter(i).TEXTO_CAMPO
            Next
            Dim Result As String = ""
            Dim Class_plantilla_validacion As New Class_plantilla_validacion
            Result = Class_plantilla_validacion.Registra_actualiza_usuario_pqr(Session.Item("PQRS_CODIGO_SCRIPT_PLANTILLA"),
                                                                               Session.Item("PQRS_ID_USUARIO_PQRS"),
                                                                               stru_campos_docuarchi)
            If Result <> "YES" Then
                Return Result
            Else
                Return "YES"
            End If
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Get_lista_Tramites(ByVal DName As String)
        Dim response As String = ""
        Dim country As List(Of String) = New List(Of String)()
        Try
            Dim estado_existencia As String = ""
            Dim Sql_consulta = "Select law.Descripcion_Doc As NOMBRE_TRAMITE from tipo_doc_entrante as law " &
               " inner join system_plantilla_radicado as spr on (spr.id_Plantilla=law.system_plantilla_radicado_id_plantilla and spr.Tipo_Plantilla='RADICACION ENTRANTE') " &
               " where  ( " &
               "  law.Descripcion_Doc like '%" & DName & "%'" &
               " ) and flow_tipo=1 order by   law.Descripcion_Doc"
            Dim refcconect As New conect.Dbase_Conction_Mysql
            Dim refclas As New ClassRadicador
            Dim datset As New DataSet
            Dim sqlconsult As String = Sql_consulta
            response = SELECTION_SELECT_FIELD(sqlconsult, datset)
            If response <> "YES" Then
                country.Add(response)
                Return country
                Exit Function
            End If
            If datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To datset.Tables(0).Rows.Count - 1
                    For z As Integer = 0 To datset.Tables(0).Columns.Count - 1
                        If datset.Tables(0).Rows(i).IsNull(z) = False Then
                            Dim obsgetipe As Object = datset.Tables(0).Rows(i).Item(z).GetType.ToString
                            If obsgetipe = "System.DateTime" Then
                                Dim subtrin As String = datset.Tables(0).Rows(i).Item(z).ToString()
                                Dim splitsubtrin() As String = subtrin.Substring(0, 10).Split("/")

                                Me.existencia_item(splitsubtrin(2) & "-" & splitsubtrin(1) & "-" & splitsubtrin(0),
                                                   country,
                                                   estado_existencia)
                                If estado_existencia = "NO" Then
                                    country.Add(splitsubtrin(2) & "-" & splitsubtrin(1) & "-" & splitsubtrin(0))
                                End If

                            Else
                                Me.existencia_item(datset.Tables(0).Rows(i).Item(z).ToString(),
                                                  country,
                                                  estado_existencia)
                                If estado_existencia = "NO" Then
                                    country.Add(datset.Tables(0).Rows(i).Item(z).ToString())
                                End If

                            End If
                        End If
                    Next

                Next
                Return country
            Else
                Return country
            End If
        Catch ex As Exception
            country.Add(ex.Message)
            Return country
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function GetLista_respuestas_radicado(ByVal DName As String)
        Dim response As String = ""
        Dim country As List(Of String) = New List(Of String)()
        Try
            Dim refcconect As New conect.Dbase_Conction_Mysql_RA
            Dim datset As New DataSet
            Dim Sql_condicion As String = ""
            If Session.Item("RA_RADICADO_CONSULTA_RESPUESTA_TODAS") = 0 Then
                Sql_condicion = "ID_REMIT_DEST_INT = " & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & " and  "
            End If
            Dim sqlconsult As String = "SELECT ID_RESPUESTA_RADICADO as ID,TRAMITE_DOCUMENTO,RADICADO,RADICADO_RESPUESTA,FECHA_REGISTRO AS FECHA_RADICACION,FECHA_VENCE," &
                 "FECHA_RESPUETA,DESTINATARIO,USUARIO_RESPONSABLE,ASUNTO " &
                 " FROM ra_respuesta_radicado  where " & Sql_condicion &
                    " ( DESTINATARIO like '%" & DName & "%'" &
                    " or ASUNTO like '%" & DName & "%'" &
                    " or RADICADO like '%" & DName & "%'" &
                    " or RADICADO_RESPUESTA like '%" & DName & "%'" &
                    " or USUARIO_RESPONSABLE like '%" & DName & "%'" &
                    " or TRAMITE_DOCUMENTO like '%" & DName & "%'" &
                    " or AREA_RESPONSABLE like '%" & DName & "%'" &
                    " or ID_RESPUESTA_RADICADO like '%" & DName & "%'" &
                    " or FECHA_REGISTRO like '%" & DName & "%'" &
                    " or FECHA_VENCE like '%" & DName & "%'" &
                    " or FECHA_RESPUETA like '%" & DName & "%'" &
                    " or FECHA_ENVIO like '%" & DName & "%'" &
                    "  ) " & "LIMIT 50"
            response = SELECTION_SELECT_FIELD(sqlconsult, datset)
            If response <> "YES" Then
                Return country
                Exit Function
            End If
            If datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To datset.Tables(0).Rows.Count - 1
                    For z As Integer = 0 To datset.Tables(0).Columns.Count - 1
                        If datset.Tables(0).Rows(i).IsNull(z) = False Then
                            Dim obsgetipe As Object = datset.Tables(0).Rows(i).Item(z).GetType.ToString
                            Dim estado_exit As String = "NO"
                            If obsgetipe = "System.DateTime" Then
                                Dim subtrin As String = datset.Tables(0).Rows(i).Item(z).ToString()
                                Dim splitsubtrin() As String = subtrin.Substring(0, 10).Split("/")
                                Dim tempo_fecha As String = splitsubtrin(2) & "-" & splitsubtrin(1) & "-" & splitsubtrin(0)
                                Veri_existe_regitro(country,
                                          tempo_fecha,
                                          estado_exit)
                                If estado_exit = "NO" Then
                                    country.Add(tempo_fecha)
                                End If
                            Else
                                Veri_existe_regitro(country,
                                         datset.Tables(0).Rows(i).Item(z).ToString(),
                                         estado_exit)
                                If estado_exit = "NO" Then
                                    country.Add(datset.Tables(0).Rows(i).Item(z).ToString())
                                End If
                            End If
                        End If
                    Next
                Next
                Return country
            Else
                Return country
            End If
        Catch ex As Exception
            Return country
        End Try
    End Function
    Function existencia_item(ByVal valor_item As String,
                            ByVal country As Object,
                            ByRef estado_existencia As String) As String
        Try
            estado_existencia = "NO"
            For i As Integer = 0 To country.Count - 1
                If Trim(country(i).ToString) = Trim(valor_item) Then
                    estado_existencia = "YES"
                    Exit For
                    Exit Function
                End If
            Next
            existencia_item = "YES"
            Exit Function
        Catch ex As Exception
            existencia_item = "Inconsistencia general función existencia_item " & ex.Message
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Set_compartir_nivel(ByVal item_user As Object,
                                        ByVal parameter As Object)
        Try
            Dim deserialize_parameter = Nothing
            Dim serializer = New JavaScriptSerializer()
            deserialize_parameter = serializer.Deserialize(Of List(Of stru_permiso_nivel))(parameter)
            If deserialize_parameter Is Nothing Then
                Return "Imposible deserealizar los parametros de configuracion"
                Exit Function
            End If
            Dim Result As String = ""
            Dim Refclas As New Class_niveles_organizacion
            Result = Refclas.Compartir_nivel_organizacion_usuario_gestion(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                         Val(item_user),
                                                                         Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION"),
                                                                         deserialize_parameter(0))
            If Result <> "YES" Then
                Return Result
            Else
                Return "YES"
            End If
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function GetEmpresa(ByVal DName As String) As List(Of String)
        Try

            Dim result As New List(Of String)()
            Dim refcconect As New conect.Dbase_Conction_Mysql_RA
            Dim refclas As New ClassRadicador
            Dim datset As New DataSet
            Dim response As String = ""
            Dim sqlconsult As String = "Select distinct entidad_empresa from destinatario_externo where entidad_empresa like '%" & DName & "%' LIMIT 100"
            response = SELECTION_SELECT_FIELD(sqlconsult, datset)
            If response <> "YES" Then
                GetEmpresa = result
                Exit Function
            End If
            If datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To datset.Tables(0).Rows.Count - 1
                    result.Add(datset.Tables(0).Rows(i).Item(0).ToString())
                Next
                GetEmpresa = result
            Else
                GetEmpresa = result
            End If
        Catch ex As Exception
            GetEmpresa = Nothing
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function GetLista_usuarios_gestion(ByVal DName As String)

        Dim response As String = ""
        Dim country As List(Of String) = New List(Of String)()
        Try
            Dim split_coma() As String = Nothing
            If InStr(DName, ",") > 0 Then
                split_coma = DName.Split(",")

            Else
                ReDim Preserve split_coma(0)
                split_coma(0) = DName
            End If
            If Trim(split_coma(split_coma.Length - 1)) = "" Then
                Return country
                Exit Function
            End If
            Dim refcconect As New conect.Dbase_Conction_Mysql_RA
            Dim refclas As New ClassRadicador
            Dim datset As New DataSet
            Dim sqlconsult As String = "Select id_Remit_Dest_Int,Login_Usuario,Nombre_Remitente,Cargo_Remite from remit_dest_interno where Nombre_Remitente like '%" & Trim(split_coma(split_coma.Length - 1)) & "%' or Cargo_Remite like'%" & Trim(split_coma(split_coma.Length - 1)) & "%' and Estado_Usuario=1 and estado_usuario_para_gestion_respuesta=1 LIMIT 100"
            response = SELECTION_SELECT_FIELD(sqlconsult, datset)
            If response <> "YES" Then
                Return country
                Exit Function
            End If
            If datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To datset.Tables(0).Rows.Count - 1
                    Dim tempo_record As String = "<" & datset.Tables(0).Rows(i).Item(0).ToString() & "> " & datset.Tables(0).Rows(i).Item(2).ToString() & " (" & datset.Tables(0).Rows(i).Item(3).ToString() & ")"
                    tempo_record = tempo_record.Replace(",", "")
                    country.Add(tempo_record)
                Next

                For i As Integer = 0 To country.Count - 1
                    For z As Integer = 0 To split_coma.Length - 1
                        If Trim(country(i).ToString) = Trim(split_coma(z)) Then
                            country.RemoveAt(i)
                            'Return country
                            'Exit Function
                        End If
                    Next
                Next
                Return country
            Else
                Return country
            End If
        Catch ex As Exception
            Return country
        End Try
    End Function
    Public Class ArrayItem
        Public text As String
        Public value As String
    End Class
    Public Class ServiceRadicado
        Public Property error_sistema As String
        Public Property url_documento As String
        Public Property radicado_documento As String
        Public Property ra_log_radicado As New List(Of ra_log_error_pqr_publico_)
    End Class
    Public Class service_rad_drow_lista
        Public Property error_sistema As String
        Public Property item_sistema As List(Of rad_drow_lista)
    End Class
    Public Class rad_drow_lista
        Public Property value As String
        Public Property text As String
    End Class
    Public Class rotulo_parameter_file
        Public Property id_expediente As Integer
        Public Property id_tipo_documento As Integer
        Public Property nombre_tipo_documento As String
        Public Property estado_adjunta_anexo As Integer
        Public Property estado_adjunta_relacionado As Integer
        Public Property numero_documento_relacionado As Integer
    End Class

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function service_lista_paises(ByVal id As Object) As IEnumerable(Of service_rad_drow_lista)
        Dim resul_service = New List(Of service_rad_drow_lista)()
        Dim item As New service_rad_drow_lista
        Dim lista_item_drow As New List(Of rad_drow_lista)
        Try

            Dim Result As String = ""
            Dim Class_pais_radicacion As New Class_pais_radicacion
            Result = Class_pais_radicacion.Service_lista_Paises(lista_item_drow)
            If Result <> "YES" Then
                item.error_sistema = Result
                resul_service.Add(item)
                Return resul_service
            Else
                item.error_sistema = Result
                item.item_sistema = lista_item_drow
                resul_service.Add(item)
                Return resul_service
            End If
        Catch ex As Exception
            item.error_sistema = "Función service_lista_departamentos " & ex.Message
            item.item_sistema = lista_item_drow
            resul_service.Add(item)
            Return resul_service
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function service_solicita_lista_departamentos(ByVal id As Object) As IEnumerable(Of service_rad_drow_lista)
        Dim resul_service = New List(Of service_rad_drow_lista)()
        Dim item As New service_rad_drow_lista
        Dim lista_item_drow As New List(Of rad_drow_lista)
        Try

            Dim Result As String = ""
            Dim Class_depart_radicacion As New Class_depart_radicacion
            Result = Class_depart_radicacion.Service_lista_departamento_Paises(id,
                                                                               lista_item_drow)
            If Result <> "YES" Then
                item.error_sistema = Result
                resul_service.Add(item)
                Return resul_service
            Else
                item.error_sistema = Result
                item.item_sistema = lista_item_drow
                resul_service.Add(item)
                Return resul_service
            End If
        Catch ex As Exception
            item.error_sistema = "Función service_solicita_lista_departamentos " & ex.Message
            item.item_sistema = lista_item_drow
            resul_service.Add(item)
            Return resul_service
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function service_source_list_item_control_general_documento_radicado(ByVal id As Object) As IEnumerable(Of control_general_drow_lista)
        Dim resul_service = New List(Of control_general_drow_lista)()
        Dim item As New control_general_drow_lista
        Dim lista_item_drow As New List(Of control_drow_lista)
        Try
            Dim Refclas_config As New Class_ra_dig_config_digitalizacion
            Dim Result As String = ""
            Result = Refclas_config.Solicita_id_configuracion_digitalizacion(Session.Item("DG_ID_TRAMITE"),
                                                                             Session.Item("DG_TIPO_TRAMITE"),
                                                                             Session.Item("DG_ID_CONFIG_DIGITALIZACION"),
                                                                             0)
            If Result <> "YES" Then
                item.error_sistema = Result
                resul_service.Add(item)
                Return resul_service
            End If
            Dim Refclas_list_cheg As New ra_dig_tipos_docum_lista_chequeo
            Dim estado_resultado As String = ""
            Result = Refclas_list_cheg.Solicita_listar_tipos_documentales_relacionados_edita_tramite_radicado_service(Session.Item("DG_ID_TRAMITE"),
                                                                                                                      Session.Item("DG_TIPO_TRAMITE"),
                                                                                                                       "",
                                                                                                                      lista_item_drow,
                                                                                                                      estado_resultado)
            If Result <> "YES" Then
                item.error_sistema = Result
                resul_service.Add(item)
                Return resul_service
            Else
                item.error_sistema = Result
                item.item_sistema = lista_item_drow
                resul_service.Add(item)
                Return resul_service
            End If
        Catch ex As Exception
            item.error_sistema = "Función service_source_list_item_control_general_documento_radicado " & ex.Message
            item.item_sistema = lista_item_drow
            resul_service.Add(item)
            Return resul_service
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function service_save_file_save_rotulo_radicado(ByVal parameter As Object) As IEnumerable(Of UploadFilesResult)
        Dim resultList = New List(Of UploadFilesResult)()
        Dim uploadFiles As UploadFilesResult = New UploadFilesResult()
        Try
            Dim Result As String = ""
            Dim deserialize_parameter = Nothing
            deserialize_parameter = JsonConvert.DeserializeObject(Of List(Of rotulo_parameter_file))(parameter)
            Dim ref_calssAlamacenamiento As New ClassAlmacenamiento
            Dim stru_datos_image_lista As stru_datos_image_lista = Nothing
            Dim id_tarea_workflow As Long = 0
            Dim contador As Integer = 0
            Dim ClassRaConsultaRadicados As New ClassRaConsultaRadicados
            Dim ruta_archivo As String = ""
            Dim split = HttpContext.Current.Session.Item("RA_RADICADO_REGISTRO")
            Result = ClassRaConsultaRadicados.Solicita_rotulo_radicado(HttpContext.Current.Session.Item("RA_RADICADO_REGISTRO"),
                                                                       ruta_archivo)
            If Result <> "YES" Then
                uploadFiles.error_sistema = Result
                resultList.Add(uploadFiles)
                Return resultList
            End If
            If File.Exists(ruta_archivo) = False Then
                uploadFiles.error_sistema = "Imposible encontrar el archivo (" & ruta_archivo & ")"
                resultList.Add(uploadFiles)
                Return resultList
            End If
            HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA") = ruta_archivo
            HttpContext.Current.Session.Item("WF_TIPO_ADJUNTA") = "ENLACE_RADICADO"
            Result = ref_calssAlamacenamiento.UploadSaveFile(deserialize_parameter(0).id_expediente,
                                                               deserialize_parameter(0).id_tipo_documento,
                                                               deserialize_parameter(0).nombre_tipo_documento,
                                                               deserialize_parameter(0).estado_adjunta_anexo,
                                                               deserialize_parameter(0).estado_adjunta_relacionado,
                                                               deserialize_parameter(0).numero_documento_relacionado,
                                                               "",
                                                               stru_datos_image_lista,
                                                               id_tarea_workflow,
                                                               contador)
            If Result <> "YES" Then
                uploadFiles.error_sistema = Result
                resultList.Add(uploadFiles)
                Return resultList
            Else
                uploadFiles.error_sistema = "YES"
                uploadFiles.name_gabinete = stru_datos_image_lista.nombre_gabinete
                uploadFiles.id_image = stru_datos_image_lista.id_imagen
                uploadFiles.radicado = stru_datos_image_lista.radicado
                uploadFiles.tipodocumental = stru_datos_image_lista.tipodocumental
                uploadFiles.notitipodocumental = stru_datos_image_lista.notipodocumento
                uploadFiles.id_tarea_workflow = id_tarea_workflow
                uploadFiles.estado_firma_digital = stru_datos_image_lista.estado_firma_digital
                uploadFiles.contador_paginas = contador
                uploadFiles.icono_icono_awe_some = stru_datos_image_lista.icono_icono_awe_some
                uploadFiles.id_registro = stru_datos_image_lista.id_registro
                uploadFiles.fecha = stru_datos_image_lista.fecha
                uploadFiles.aleas = stru_datos_image_lista.aleas
                uploadFiles.nombre_archivo = stru_datos_image_lista.nombre_archivo
                resultList.Add(uploadFiles)
                Return resultList
            End If
        Catch ex As Exception
            uploadFiles.error_sistema = "Función service_save_file_save_rotulo_radicado " & ex.Message
            resultList.Add(uploadFiles)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function service_solicita_lista_municipio(ByVal id As Object) As IEnumerable(Of service_rad_drow_lista)
        Dim resul_service = New List(Of service_rad_drow_lista)()
        Dim item As New service_rad_drow_lista
        Dim lista_item_drow As New List(Of rad_drow_lista)
        Try

            Dim Result As String = ""
            Dim Class_municipio_radicacion As New Class_municipio_radicacion
            Result = Class_municipio_radicacion.Service_lista_municipio_departamento(id,
                                                                                     lista_item_drow)
            If Result <> "YES" Then
                item.error_sistema = Result
                resul_service.Add(item)
                Return resul_service
            Else
                item.error_sistema = Result
                item.item_sistema = lista_item_drow
                resul_service.Add(item)
                Return resul_service
            End If
        Catch ex As Exception
            item.error_sistema = "Función service_solicita_lista_departamentos " & ex.Message
            item.item_sistema = lista_item_drow
            resul_service.Add(item)
            Return resul_service
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_lista_tipo_respuesta(ByVal id As Object) As IEnumerable(Of service_rad_drow_lista)
        Dim resul_service = New List(Of service_rad_drow_lista)()
        Dim item As New service_rad_drow_lista
        Dim lista_item_drow As New List(Of rad_drow_lista)
        Try

            Dim Result As String = ""
            Dim Class_ra_respuesta_tipo As New Class_ra_respuesta_tipo
            Result = Class_ra_respuesta_tipo.Service_lista_tipo_respuesta(lista_item_drow)
            If Result <> "YES" Then
                item.error_sistema = Result
                resul_service.Add(item)
                Return resul_service
            Else
                item.error_sistema = Result
                item.item_sistema = lista_item_drow
                resul_service.Add(item)
                Return resul_service
            End If
        Catch ex As Exception
            item.error_sistema = "Función Service_lista_tipo_respuesta " & ex.Message
            item.item_sistema = lista_item_drow
            resul_service.Add(item)
            Return resul_service
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function GetLista_usuarios_gestion_tokenize(ByVal DName As String)
        Dim response As String = ""
        Dim country As New List(Of ArrayItem)
        Try
            Dim refcconect As New conect.Dbase_Conction_Mysql_RA
            Dim refclas As New ClassRadicador
            Dim datset As New DataSet
            Dim sqlconsult As String = "Select id_Remit_Dest_Int,Login_Usuario,Nombre_Remitente,Cargo_Remite from remit_dest_interno where Nombre_Remitente like '%" & Trim(DName) & "%' or Cargo_Remite like'%" & Trim(DName) & "%' and Estado_Usuario=1 and estado_usuario_para_gestion_respuesta=1 LIMIT 100"
            response = SELECTION_SELECT_FIELD(sqlconsult, datset)
            If response <> "YES" Then
                Return country
                Exit Function
            End If
            If datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To datset.Tables(0).Rows.Count - 1
                    Dim items As New ArrayItem
                    items.text = datset.Tables(0).Rows(i).Item(2).ToString() & " (" & datset.Tables(0).Rows(i).Item(3).ToString() & ")"
                    items.value = datset.Tables(0).Rows(i).Item(0).ToString()
                    country.Add(items)
                Next
                Return country
            Else
                Return country
            End If
        Catch ex As Exception
            Return country
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function GetLista_correos_usuarios_gestion_tokenize(ByVal DName As String)
        Dim response As String = ""
        Dim country As New List(Of ArrayItem)
        Try
            Dim refcconect As New conect.Dbase_Conction_Mysql_RA
            Dim datset As New DataSet
            Dim sqlconsult As String = "(Select  Correo_Electronico from remit_dest_interno where  Correo_Electronico like '%" & Trim(DName) & "%') UNION  DISTINCT" &
                "(Select  Correo_Electronico from ra_ca_cache_correo_envio where ( Correo_Electronico like '%" & Trim(DName) & "%') and id_Remit_Dest_Int=" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & ")"
            response = SELECTION_SELECT_FIELD(sqlconsult, datset)
            If response <> "YES" Then
                Return country
                Exit Function
            End If
            If datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To datset.Tables(0).Rows.Count - 1
                    If datset.Tables(0).Rows(i).IsNull(0) = False Then
                        Dim items As New ArrayItem
                        items.text = datset.Tables(0).Rows(i).Item(0).ToString()
                        items.value = datset.Tables(0).Rows(i).Item(0).ToString()
                        country.Add(items)
                    End If
                Next
                Return country
            Else
                Return country
            End If
        Catch ex As Exception
            Return country
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function GetLista_correos_respuesta_documento_tokenize(ByVal DName As String)
        Dim response As String = ""
        Dim country As New List(Of ArrayItem)
        Try
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Dim ref_clasradicador As New ClassRadicador
            Dim stru_envio As stru_envio = Nothing
            Dim correo_electronico As String = ""
            Dim Result As String = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(Val(DName),
                                                                                                      stru_envio)
            If Result <> "YES" Then
                Return Result
                Exit Function
            End If
            Result = ref_clasradicador.Solicta_Correo_Electronico_remitente_por_radicado(stru_envio.codigo_dest_externo,
                                                                                        correo_electronico,
                                                                                        stru_envio.system_plantilla_radicado_id_plantilla)
            If Result <> "YES" Then
                Return Result
                Exit Function
            End If
            Return correo_electronico
        Catch ex As Exception
            Return ""
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Add_correos_cache_tokenize(ByVal DName As String)
        Dim response As String = ""
        Try
            Dim datset As New DataSet
            Dim sqlconsult As String = "Select  Correo_Electronico from ra_ca_cache_correo_envio where  Correo_Electronico = '" & Trim(DName) & "' and id_Remit_Dest_Int=" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
            response = SELECTION_SELECT_FIELD(sqlconsult, datset)
            If response <> "YES" Then
                Return response

            End If
            If datset.Tables(0).Rows.Count > 0 Then
                Return "YES"

            End If
            Dim sql_inset = "Insert into ra_ca_cache_correo_envio (id_Remit_Dest_Int,Correo_Electronico) values (" &
            HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & ",'" & Trim(DName) & "')"
            Dim refcconect As New conect.Dbase_Conction_Mysql_RA
            response = refcconect.SELECTION_INSERT_COMMAND(sql_inset)
            If response <> "YES" Then
                Return response
            Else
                response = "YES"
                Return response
            End If
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function GetLista_documentos_produccion(ByVal DName As String)
        Dim response As String = ""
        Dim country As List(Of String) = New List(Of String)()
        Try

            Dim refcconect As New conect.Dbase_Conction_Mysql_RA
            Dim refclas As New ClassRadicador
            Dim datset As New DataSet
            Dim sqlconsult As String = "(Select SEGUNDO_NOMBRE_DOCUMENTO as DOCUMENTO,FECHA_DOCUMENTO,DESCRIPCION_TIPO_DOCUMENTO AS TIPODOCUMENTAL " _
                                    & " from registro_producion_documental as rpd " &
                                    " inner join  ra_pro_niveles_has_expediente_archivo as rpnhea on (rpnhea.expediente_archivo_ID_EXPEDIENTE=rpd.EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE )  " &
                                    " inner join ra_pro_niveles as rppn on (rppn.id_nivel=rpnhea.ra_pro_niveles_id_nivel and  rppn.remit_dest_interno_id_Remit_Dest_Int=" & Session.Item("GA_IDUSUARIOGESTION") & " )" &
                                    " where rpd.SEGUNDO_NOMBRE_DOCUMENTO like '%" & DName & "%' or FECHA_DOCUMENTO like " & "'%" & DName & "%'" & " or rpd.DESCRIPCION_TIPO_DOCUMENTO like '%" & DName & "%' LIMIT 50)" & " union " &
                                    " (Select SEGUNDO_NOMBRE_DOCUMENTO as DOCUMENTO,FECHA_DOCUMENTO,DESCRIPCION_TIPO_DOCUMENTO AS TIPODOCUMENTAL" &
                                    " from  registro_producion_documental as rpd " &
                                    " inner join  ra_pro_niveles_has_expediente_archivo as rpnhea on (rpnhea.expediente_archivo_ID_EXPEDIENTE=rpd.EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE )  " &
                                    " inner join ra_pro_permisos_niveles as rppn on (rppn.ra_pro_niveles_id_nivel=rpnhea.ra_pro_niveles_id_nivel and  rppn.remit_dest_interno_id_Remit_Dest_Int=" & Session.Item("GA_IDUSUARIOGESTION") & " )" &
                                    " where rpd.SEGUNDO_NOMBRE_DOCUMENTO like '%" & DName & "%' or FECHA_DOCUMENTO like " & "'%" & DName & "%'" & " or rpd.DESCRIPCION_TIPO_DOCUMENTO like '%" & DName & "%' LIMIT 50)"
            response = SELECTION_SELECT_FIELD(sqlconsult, datset)
            If response <> "YES" Then
                Return country
                Exit Function
            End If
            If datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To datset.Tables(0).Rows.Count - 1
                    For z As Integer = 0 To datset.Tables(0).Columns.Count - 1
                        If datset.Tables(0).Rows(i).IsNull(z) = False Then
                            Dim obsgetipe As Object = datset.Tables(0).Rows(i).Item(z).GetType.ToString
                            Dim estado_exit As String = "NO"
                            If obsgetipe = "System.DateTime" Then
                                Dim subtrin As String = datset.Tables(0).Rows(i).Item(z).ToString()
                                Dim splitsubtrin() As String = subtrin.Substring(0, 10).Split("/")
                                Dim tempo_fecha As String = splitsubtrin(2) & "-" & splitsubtrin(1) & "-" & splitsubtrin(0)
                                Veri_existe_regitro(country,
                                          tempo_fecha,
                                          estado_exit)
                                If estado_exit = "NO" Then
                                    country.Add(tempo_fecha)
                                End If
                            Else
                                Veri_existe_regitro(country,
                                         datset.Tables(0).Rows(i).Item(z).ToString(),
                                         estado_exit)
                                If estado_exit = "NO" Then
                                    country.Add(datset.Tables(0).Rows(i).Item(z).ToString())
                                End If

                            End If
                        End If
                    Next
                Next
                Return country
            Else
                Return country
            End If
        Catch ex As Exception
            Return country
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function GetLista_documentos_compartidos_otros_usuarios(ByVal DName As String)

        Dim response As String = ""
        Dim country As List(Of String) = New List(Of String)()
        Try

            Dim refcconect As New conect.Dbase_Conction_Mysql_RA
            Dim refclas As New ClassRadicador
            Dim datset As New DataSet
            Dim sqlconsult As String = "SELECT ID_RA_CD_DOCUMENTOS_COMPARTIDOS AS NUMERO,ESTADO_PRIORIDAD," &
                   "rdi.Nombre_Remitente as COMPARTE,rdi.Cargo_Remite  " _
                  & "as CARGO_COMPARTE,ASUNTO_DOCUMENTO as ASUNTO,rcs.DESCRIPCION_TIPO_COMPARTIDO AS TIPO,rcs.DESCRIPCION_ESTADO_APROBACION as ESTADO,rcs.ESTADO_ELIMINADO,rcs.RADICADO_RELACIONADO AS RADICADO,rcs.FECHA_REGISTRO_SOLICITUD " &
                  "as FECHA_REGISTRO,rcs.FECHA_LIMITE_RESPUESTA as FECHA_LIMITE from ra_cd_documentos_compartidos AS rcs " &
                  " INNER JOIN remit_dest_interno AS rdi on (rdi.id_Remit_Dest_Int=rcs.Remit_Dest_Interno_id_remit_dest_Int)  where " &
                    " (" &
                  "  ID_RA_CD_DOCUMENTOS_COMPARTIDOS like '%" & DName & "%'" &
                  " or rdi.Nombre_Remitente like '%" & DName & "%'" &
                  " or rdi.Cargo_Remite like '%" & DName & "%'" &
                  " or ASUNTO_DOCUMENTO like '%" & DName & "%'" &
                  " or rcs.DESCRIPCION_TIPO_COMPARTIDO like '%" & DName & "%'" &
                  " or rcs.FECHA_REGISTRO_SOLICITUD like '%" & DName & "%'" &
                  " or rcs.FECHA_LIMITE_RESPUESTA like '%" & DName & "%'" &
                  " or rcs.RADICADO_RELACIONADO like '%" & DName & "%' )" &
                  " and rcs.Remit_Dest_Interno_id_remit_dest_Int=" & Session.Item("GA_IDUSUARIOGESTION") & " LIMIT 50"
            response = SELECTION_SELECT_FIELD(sqlconsult, datset)
            If response <> "YES" Then
                Return country
                Exit Function
            End If
            If datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To datset.Tables(0).Rows.Count - 1
                    For z As Integer = 0 To datset.Tables(0).Columns.Count - 1
                        If datset.Tables(0).Rows(i).IsNull(z) = False Then
                            Dim obsgetipe As Object = datset.Tables(0).Rows(i).Item(z).GetType.ToString
                            Dim estado_exit As String = "NO"
                            If obsgetipe = "System.DateTime" Then
                                Dim subtrin As String = datset.Tables(0).Rows(i).Item(z).ToString()
                                Dim splitsubtrin() As String = subtrin.Substring(0, 10).Split("/")
                                Dim tempo_fecha As String = splitsubtrin(2) & "-" & splitsubtrin(1) & "-" & splitsubtrin(0)
                                Veri_existe_regitro(country,
                                          tempo_fecha,
                                          estado_exit)
                                If estado_exit = "NO" Then
                                    country.Add(tempo_fecha)
                                End If
                            Else
                                Veri_existe_regitro(country,
                                         datset.Tables(0).Rows(i).Item(z).ToString(),
                                         estado_exit)
                                If estado_exit = "NO" Then
                                    country.Add(datset.Tables(0).Rows(i).Item(z).ToString())
                                End If

                            End If
                        End If
                    Next
                Next
                Return country
            Else
                Return country
            End If
        Catch ex As Exception
            Return country
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function GetLista_documentos_compartidos_revision(ByVal DName As String)

        Dim response As String = ""
        Dim country As List(Of String) = New List(Of String)()
        Try

            Dim refcconect As New conect.Dbase_Conction_Mysql_RA
            Dim refclas As New ClassRadicador
            Dim datset As New DataSet
            Dim sqlconsult As String = "SELECT ID_USUARIOS_DOCUMENTOS_COMPARTIDOS,rcsd.ESTADO_PRIORIDAD,rcs.ESTADO_VISTO_SOLICITANTE,rcs.ESTADO_ELIMINADO," &
                    "rcsd.ID_RA_CD_DOCUMENTOS_COMPARTIDOS AS NUMERO,rdi.Nombre_Remitente as COMPARTE,rdi.Cargo_Remite as CARGO," _
                    & "rcsd.ASUNTO_DOCUMENTO AS ASUNTO,rcs.DESCRIPCION_TIPO_COMPARTIDO AS TIPO,rcs.DESCRIPCION_ESTADO_RESPUESTA as ESTADO,rcs.FECHA_LIMITE_RESPUESTA as FECHA_LIMITE,rcsd.RADICADO_RELACIONADO as RADICADO, " &
                    " rcsd.FECHA_REGISTRO_SOLICITUD as FECHA from ra_cd_usuarios_documentos_compartidos AS rcs " &
                    " INNER JOIN ra_cd_documentos_compartidos AS rcsd on (rcsd.ID_RA_CD_DOCUMENTOS_COMPARTIDOS=rcs.ID_RA_CD_DOCUMENTOS_COMPARTIDOS)" &
                    " INNER JOIN remit_dest_interno AS rdi on (rdi.id_Remit_Dest_Int=rcsd.Remit_Dest_Interno_id_remit_dest_Int) " &
                    " where (" &
                    "  rcsd.ID_RA_CD_DOCUMENTOS_COMPARTIDOS like '%" & DName & "%'" &
                    " or rdi.Nombre_Remitente like '%" & DName & "%'" &
                    " or rdi.Cargo_Remite like '%" & DName & "%'" &
                    " or rcsd.ASUNTO_DOCUMENTO like '%" & DName & "%'" &
                    " or rcs.DESCRIPCION_TIPO_COMPARTIDO like '%" & DName & "%'" &
                    " or rcsd.FECHA_REGISTRO_SOLICITUD like '%" & DName & "%'" &
                    " or rcs.FECHA_LIMITE_RESPUESTA like '%" & DName & "%'" &
                    " or rcsd.RADICADO_RELACIONADO like '%" & DName & "%' )" &
                    " and rcs.Remit_Dest_Interno_id_remit_dest_Int=" & Session.Item("GA_IDUSUARIOGESTION") & " LIMIT 50"
            response = SELECTION_SELECT_FIELD(sqlconsult, datset)
            If response <> "YES" Then
                Return country
                Exit Function
            End If
            If datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To datset.Tables(0).Rows.Count - 1
                    For z As Integer = 0 To datset.Tables(0).Columns.Count - 1
                        If datset.Tables(0).Rows(i).IsNull(z) = False Then
                            Dim obsgetipe As Object = datset.Tables(0).Rows(i).Item(z).GetType.ToString
                            Dim estado_exit As String = "NO"
                            If obsgetipe = "System.DateTime" Then
                                Dim subtrin As String = datset.Tables(0).Rows(i).Item(z).ToString()
                                Dim splitsubtrin() As String = subtrin.Substring(0, 10).Split("/")
                                Dim tempo_fecha As String = splitsubtrin(2) & "-" & splitsubtrin(1) & "-" & splitsubtrin(0)
                                Veri_existe_regitro(country,
                                          tempo_fecha,
                                          estado_exit)
                                If estado_exit = "NO" Then
                                    country.Add(tempo_fecha)
                                End If
                            Else
                                Veri_existe_regitro(country,
                                         datset.Tables(0).Rows(i).Item(z).ToString(),
                                         estado_exit)
                                If estado_exit = "NO" Then
                                    country.Add(datset.Tables(0).Rows(i).Item(z).ToString())
                                End If

                            End If
                        End If
                    Next
                Next
                Return country
            Else
                Return country
            End If
        Catch ex As Exception
            Return country
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function GetLista_documentos_compartidos_mi_aprobacion(ByVal DName As String)

        Dim response As String = ""
        Dim country As List(Of String) = New List(Of String)()
        Try

            Dim refcconect As New conect.Dbase_Conction_Mysql_RA
            Dim refclas As New ClassRadicador
            Dim datset As New DataSet
            Dim sqlconsult As String = "SELECT ID_CD_USUARIOS_SOLICITUDES_APROBACION,ESTADO_PRIORIDAD,rcs.ESTADO_VISTO_SOLICITANTE,rcs.FECHA_LIMITE_RESPUESTA as FECHA_LIMITE," &
                   "rcs.DESCRIPCION_ESTADO_RESPUESTA AS ESTADO,rdi.Nombre_Remitente as SOLICITANTE ,rdi.Cargo_Remite as CARGO_SOLICITANTE,rcsd.ID_SOLICITUDES_APROBACION " &
                   " AS SOLICITUD,rrr.RADICADO as RADICADO,rrr.DESTINATARIO as PETICIONARIO, rcs.FECHA_REGISTRO_SOLICITUD as FECHA from ra_cd_usuarios_solicitudes_aprobacion AS rcs " &
                   " INNER JOIN ra_cd_solicitudes_aprobacion AS rcsd on (rcsd.ID_SOLICITUDES_APROBACION=rcs.RA_CD_SOLICITUDES_APROBACION_ID_SOLICITUDES_APROBACION)" &
                   " INNER JOIN ra_respuesta_radicado AS rrr on (rrr.ID_RESPUESTA_RADICADO=rcsd.Ra_Respuesta_Radicado_ID_RESPUESTA_RADICADO)" &
                   " INNER JOIN remit_dest_interno AS rdi on (rdi.id_Remit_Dest_Int=rcsd.Remit_Dest_Interno_id_remit_dest_Int)  where (" &
                   "  rcs.DESCRIPCION_ESTADO_RESPUESTA like '%" & DName & "%'" &
                   " or rrr.RADICADO like '%" & DName & "%'" &
                    " or rrr.DESTINATARIO like '%" & DName & "%'" &
                    " or  rdi.Nombre_Remitente like '%" & DName & "%'" &
                    " or rdi.Cargo_Remite like '%" & DName & "%'" &
                    " or rcs.FECHA_LIMITE_RESPUESTA like '%" & DName & "%'" &
                    " or rcs.FECHA_REGISTRO_SOLICITUD like '%" & DName & "%') and rcs.Remit_Dest_Interno_id_remit_dest_Int=" & Session.Item("GA_IDUSUARIOGESTION") & " LIMIT 50"
            response = SELECTION_SELECT_FIELD(sqlconsult, datset)
            If response <> "YES" Then
                Return country
                Exit Function
            End If
            If datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To datset.Tables(0).Rows.Count - 1
                    For z As Integer = 0 To datset.Tables(0).Columns.Count - 1
                        If datset.Tables(0).Rows(i).IsNull(z) = False Then
                            Dim obsgetipe As Object = datset.Tables(0).Rows(i).Item(z).GetType.ToString
                            Dim estado_exit As String = "NO"
                            If obsgetipe = "System.DateTime" Then
                                Dim subtrin As String = datset.Tables(0).Rows(i).Item(z).ToString()
                                Dim splitsubtrin() As String = subtrin.Substring(0, 10).Split("/")
                                Dim tempo_fecha As String = splitsubtrin(2) & "-" & splitsubtrin(1) & "-" & splitsubtrin(0)
                                Veri_existe_regitro(country,
                                          tempo_fecha,
                                          estado_exit)
                                If estado_exit = "NO" Then
                                    country.Add(tempo_fecha)
                                End If
                            Else
                                Veri_existe_regitro(country,
                                         datset.Tables(0).Rows(i).Item(z).ToString(),
                                         estado_exit)
                                If estado_exit = "NO" Then
                                    country.Add(datset.Tables(0).Rows(i).Item(z).ToString())
                                End If

                            End If
                        End If
                    Next
                Next
                Return country
            Else
                Return country
            End If
        Catch ex As Exception
            Return country
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function GetLista_documentos_clasificacion(ByVal DName As String)

        Dim response As String = ""
        Dim country As List(Of String) = New List(Of String)()
        Try

            Dim refcconect As New conect.Dbase_Conction_Mysql_RA
            Dim refclas As New ClassRadicador
            Dim datset As New DataSet
            Dim sqlconsult As String = ""
            sqlconsult = "SELECT ID_DOCUMENTO_DOCUARCHI_ALMACEN AS ID_DOCUMENTO," &
                  "NOMBRE_GABINETE AS CONTENEDOR,CLASEDOCUMENTO,FECHA_DOCUMENTO,SEGUNDO_NOMBRE_DOCUMENTO as NOMBRE,NOMBRE_AREA_DEPARTAMENTO " &
                  "AS SECCION,SERIE_DOCUMENTO,SUBSERIE_DOCUMENTO,DESCRIPCION_TIPO_DOCUMENTO AS TIPO_DOCUMENTO " &
                  " from registro_producion_documental where EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE=" & Session.Item("GA_DATO_CONSULTA_doc_id_unidad_clasificacion") &
                   " and (" &
                    "  ID_DOCUMENTO_DOCUARCHI_ALMACEN like '%" & DName & "%'" &
                    "  or  NOMBRE_GABINETE like '%" & DName & "%'" &
                    "  or  CLASEDOCUMENTO like '%" & DName & "%'" &
                    "  or  FECHA_DOCUMENTO like '%" & DName & "%'" &
                    "  or  SEGUNDO_NOMBRE_DOCUMENTO like '%" & DName & "%'" &
                    "  or  NOMBRE_AREA_DEPARTAMENTO like '%" & DName & "%'" &
                    "  or  SERIE_DOCUMENTO like '%" & DName & "%'" &
                    "  or  DESCRIPCION_TIPO_DOCUMENTO like '%" & DName & "%') LIMIT 50"
            response = SELECTION_SELECT_FIELD(sqlconsult, datset)
            If response <> "YES" Then
                Return country
                Exit Function
            End If
            If datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To datset.Tables(0).Rows.Count - 1
                    For z As Integer = 0 To datset.Tables(0).Columns.Count - 1
                        If datset.Tables(0).Rows(i).IsNull(z) = False Then
                            Dim obsgetipe As Object = datset.Tables(0).Rows(i).Item(z).GetType.ToString
                            Dim estado_exit As String = "NO"
                            If obsgetipe = "System.DateTime" Then
                                Dim subtrin As String = datset.Tables(0).Rows(i).Item(z).ToString()
                                Dim splitsubtrin() As String = subtrin.Substring(0, 10).Split("/")
                                Dim tempo_fecha As String = splitsubtrin(2) & "-" & splitsubtrin(1) & "-" & splitsubtrin(0)
                                Veri_existe_regitro(country,
                                          tempo_fecha,
                                          estado_exit)
                                If estado_exit = "NO" Then
                                    country.Add(tempo_fecha)
                                End If
                            Else
                                Veri_existe_regitro(country,
                                         datset.Tables(0).Rows(i).Item(z).ToString(),
                                         estado_exit)
                                If estado_exit = "NO" Then
                                    country.Add(datset.Tables(0).Rows(i).Item(z).ToString())
                                End If

                            End If
                        End If
                    Next
                Next
                Return country
            Else
                Return country
            End If
        Catch ex As Exception
            Return country
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function GetLista_radicados_general(ByVal DName As String)

        Dim response As String = ""
        Dim country As List(Of String) = New List(Of String)()
        Try

            Dim refcconect As New conect.Dbase_Conction_Mysql_RA
            Dim refclas As New ClassRadicador
            Dim datset As New DataSet
            Dim sqlconsult As String = ""
            Dim split() As String = Session.Item("RA_MODULO_SELECCIONADO").ToString.Split("|")
            Dim ref_destinatario As String = ""
            If split(2) = "RADICACION SALIENTE" Then
                ref_destinatario = "CARGO_REMITENTE"
            Else
                ref_destinatario = "CARGO_DESTINATARIO"
            End If
            sqlconsult = "SELECT DISTINCT Consecutivo_Rad," &
                  "Consecutivo_CodBarra,Fecha_Radicado,REMITENTE_COR,Destinatario_Cor," & ref_destinatario &
                  ",Area_remit_dest_interno,Expediente,Descripcion_Documento,ASUNTO,Fecha_Radicado  " &
                  " from " & split(4) & "   "
            If HttpContext.Current.Session.Item("RA_TIPO_CONSULTA_RADICADO") = "PRODUCCION" Then
                sqlconsult = sqlconsult & " where Usuario_Radicador_id_usuario=" & HttpContext.Current.Session.Item("RA_ID_USUARIO") & " and Flag_Flow=2 and  (" &
                    "  Consecutivo_Rad like '%" & DName & "%'" &
                    "  or  Consecutivo_CodBarra like '%" & DName & "%'" &
                    "  or  REMITENTE_COR like '%" & DName & "%'" &
                    "  or  Destinatario_Cor like '%" & DName & "%'" &
                    "  or  " & ref_destinatario & " like '%" & DName & "%'" &
                    "  or  Expediente like '%" & DName & "%'" &
                    "  or  ASUNTO like '%" & DName & "%'" &
                    "  or  Fecha_Radicado like '%" & DName & "%'" &
                    "  or  Descripcion_Documento like '%" & DName & "%') LIMIT 50"
            Else
                sqlconsult = sqlconsult & " where  Consecutivo_Rad like '%" & DName & "%'" &
                   "  or  Consecutivo_CodBarra like '%" & DName & "%'" &
                   "  or  REMITENTE_COR like '%" & DName & "%'" &
                   "  or  Destinatario_Cor like '%" & DName & "%'" &
                   "  or  " & ref_destinatario & " like '%" & DName & "%'" &
                   "  or  Expediente like '%" & DName & "%'" &
                   "  or  ASUNTO like '%" & DName & "%'" &
                   "  or  Fecha_Radicado like '%" & DName & "%'" &
                   "  or  Descripcion_Documento like '%" & DName & "%'  LIMIT 50"
            End If
            response = SELECTION_SELECT_FIELD(sqlconsult, datset)
            If response <> "YES" Then
                Return country
                Exit Function
            End If
            If datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To datset.Tables(0).Rows.Count - 1
                    For z As Integer = 0 To datset.Tables(0).Columns.Count - 1
                        If datset.Tables(0).Rows(i).IsNull(z) = False Then
                            Dim obsgetipe As Object = datset.Tables(0).Rows(i).Item(z).GetType.ToString
                            Dim estado_exit As String = "NO"
                            If obsgetipe = "System.DateTime" Then
                                Dim subtrin As String = datset.Tables(0).Rows(i).Item(z).ToString()
                                Dim splitsubtrin() As String = subtrin.Substring(0, 10).Split("/")
                                Dim tempo_fecha As String = splitsubtrin(2) & "-" & splitsubtrin(1) & "-" & splitsubtrin(0)
                                Veri_existe_regitro(country,
                                          tempo_fecha,
                                          estado_exit)
                                If estado_exit = "NO" Then
                                    country.Add(tempo_fecha)
                                End If
                            Else
                                Veri_existe_regitro(country,
                                         datset.Tables(0).Rows(i).Item(z).ToString(),
                                         estado_exit)
                                If estado_exit = "NO" Then
                                    country.Add(datset.Tables(0).Rows(i).Item(z).ToString())
                                End If

                            End If
                        End If
                    Next
                Next
                Return country
            Else
                Return country
            End If
        Catch ex As Exception
            Return country
        End Try
    End Function
    Function Veri_existe_regitro(ByVal country As Object,
                                 ByVal valor As String,
                                 ByRef estado_exist As String) As String
        Try
            Veri_existe_regitro = "NO"
            For i As Integer = 0 To country.Count - 1
                If Trim(country(i).ToString) = Trim(valor) Then
                    estado_exist = "YES"
                    Veri_existe_regitro = "YES"
                    Exit Function
                End If
            Next
            Veri_existe_regitro = "YES"
        Catch ex As Exception
            Veri_existe_regitro = "Inconcistencia general función Veri_existe_regitro " & ex.Message
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function GetLista_expedientes_clasificacion(ByVal DName As String)

        Dim response As String = ""
        Dim country As List(Of String) = New List(Of String)()
        Try

            Dim refcconect As New conect.Dbase_Conction_Mysql_RA
            Dim refclas As New ClassRadicador
            Dim datset As New DataSet
            Dim sqlconsult As String = "Select SEGUNDO_NOMBRE_DOCUMENTO as DOCUMENTO,FECHA_DOCUMENTO,DESCRIPCION_TIPO_DOCUMENTO AS TIPODOCUMENTAL " _
                                    & " from registro_producion_documental as rpd " &
                                    " inner join  ra_pro_niveles_has_expediente_archivo as rpnhea on (rpnhea.expediente_archivo_ID_EXPEDIENTE=rpd.EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE )  " &
                                    " inner join ra_pro_niveles as rppn on (rppn.id_nivel=rpnhea.ra_pro_niveles_id_nivel and  rppn.remit_dest_interno_id_Remit_Dest_Int=" & Session.Item("GA_IDUSUARIOGESTION") & " )" &
                                    " where rpd.SEGUNDO_NOMBRE_DOCUMENTO like '%" & DName & "%' or FECHA_DOCUMENTO like " & "'%" & DName & "%'" & " or rpd.DESCRIPCION_TIPO_DOCUMENTO like '%" & DName & "%'" & " union " &
                                    " Select SEGUNDO_NOMBRE_DOCUMENTO as DOCUMENTO,FECHA_DOCUMENTO,DESCRIPCION_TIPO_DOCUMENTO AS TIPODOCUMENTAL" &
                                    " from  registro_producion_documental as rpd " &
                                    " inner join  ra_pro_niveles_has_expediente_archivo as rpnhea on (rpnhea.expediente_archivo_ID_EXPEDIENTE=rpd.EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE )  " &
                                    " inner join ra_pro_permisos_niveles as rppn on (rppn.ra_pro_niveles_id_nivel=rpnhea.ra_pro_niveles_id_nivel and  rppn.remit_dest_interno_id_Remit_Dest_Int=" & Session.Item("GA_IDUSUARIOGESTION") & " )" &
                                    " where rpd.SEGUNDO_NOMBRE_DOCUMENTO like '%" & DName & "%' or FECHA_DOCUMENTO like " & "'%" & DName & "%'" & " or rpd.DESCRIPCION_TIPO_DOCUMENTO like '%" & DName & "%'"
            If HttpContext.Current.Session.Item("nivel_expe_clasificacion") = "" Then
                Return country
                Exit Function
            End If
            Dim sql_condicion As String = ""
            If HttpContext.Current.Session.Item("nivel_expe_clasificacion") = "Serie" Then
                sql_condicion = " and CODIGO_SERIE_TRD=" & HttpContext.Current.Session.Item("serie_expe_clasificacion") '& " AND Estado_Publico_Sub_Expediente=1 "
            End If
            If HttpContext.Current.Session.Item("nivel_expe_clasificacion") = "Sección" Then
                sql_condicion = " and CODIGO_AREA_TRD=" & HttpContext.Current.Session.Item("serie_expe_clasificacion") & " or ID_SUB_AREA=" & HttpContext.Current.Session.Item("serie_expe_clasificacion") '& " AND Estado_Publico_Sub_Expediente=1 "
            End If
            sqlconsult = "SELECT ID_EXPEDIENTE AS CODIGO,CODIGO_UNICO AS NOMBRE_UNIDAD,ALEAS_EXPEDIENTE as ALEAS," &
                 "TEMA_EXPEDIENTE AS TEMA,TIPO_UNIDAD_CONSERVACION AS UNIDAD," &
                 "FECHA_EXTREMA_INICIAL AS FECHA_INI,FECHA_EXTREMA_FINAL AS FECHA_FIN,rte.NOMBRE_TIPO_EXPEDIENTE AS TIPO,NOMBRE_TIPO_UNIDAD_DOCUMENTAL AS CLASE_UNIDAD,NUMERO_FOLIOS_CONTENIDOS as FOLIO_FISICO,NUMERO_ELECTRONICO_CONTENIDO" _
                 & " AS FOLIO_ELECTRONICO,NUMERO_DIGITALIZADO_CONTENIDO AS FOLIO_DIGITALIZADO,NOMBRE_SERIE_TRD,NOMBRE_SUBSERIE_TRD from expediente_archivo " &
                 " left outer join ra_tipo_expediente as rte on (rte.ID_TIPO_EXPEDIENTE=RA_TIP_EXPE_ID_TIPO_EXPEDIENTE) " &
                  " where (" &
                  "  ID_EXPEDIENTE like '%" & DName & "%'" &
                  "  or  CODIGO_UNICO like '%" & DName & "%'" &
                  "  or  TEMA_EXPEDIENTE like '%" & DName & "%'" &
                  "  or  TIPO_UNIDAD_CONSERVACION like '%" & DName & "%'" &
                  "  or  FECHA_EXTREMA_INICIAL like '%" & DName & "%'" &
                  "  or  FECHA_EXTREMA_FINAL like '%" & DName & "%'" &
                  "  or  NOMBRE_TIPO_EXPEDIENTE like '%" & DName & "%'" &
                  "  or  NOMBRE_TIPO_UNIDAD_DOCUMENTAL like '%" & DName & "%'" &
                  "  or  NUMERO_FOLIOS_CONTENIDOS like '%" & DName & "%'" &
                  "  or  NUMERO_ELECTRONICO_CONTENIDO like '%" & DName & "%') " &
                   sql_condicion
            response = SELECTION_SELECT_FIELD(sqlconsult, datset)
            If response <> "YES" Then
                Return country
                Exit Function
            End If
            If datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To datset.Tables(0).Rows.Count - 1
                    For z As Integer = 0 To datset.Tables(0).Columns.Count - 1
                        If datset.Tables(0).Rows(i).IsNull(z) = False Then
                            Dim obsgetipe As Object = datset.Tables(0).Rows(i).Item(z).GetType.ToString
                            Dim estado_exit As String = "NO"
                            If obsgetipe = "System.DateTime" Then
                                Dim subtrin As String = datset.Tables(0).Rows(i).Item(z).ToString()
                                Dim splitsubtrin() As String = subtrin.Substring(0, 10).Split("/")
                                Dim tempo_fecha As String = splitsubtrin(2) & "-" & splitsubtrin(1) & "-" & splitsubtrin(0)
                                Veri_existe_regitro(country,
                                          tempo_fecha,
                                          estado_exit)
                                If estado_exit = "NO" Then
                                    country.Add(tempo_fecha)
                                End If
                            Else
                                Veri_existe_regitro(country,
                                         datset.Tables(0).Rows(i).Item(z).ToString(),
                                         estado_exit)
                                If estado_exit = "NO" Then
                                    country.Add(datset.Tables(0).Rows(i).Item(z).ToString())
                                End If

                            End If
                        End If
                    Next
                Next
                Return country
            Else
                Return country
            End If
        Catch ex As Exception
            Return country
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function GetLista_correos_usuarios_gestion(ByVal DName As String)

        Dim response As String = ""
        Dim country As List(Of String) = New List(Of String)()
        Try
            Dim split_coma() As String = Nothing
            If InStr(DName, ",") > 0 Then
                split_coma = DName.Split(",")

            Else
                ReDim Preserve split_coma(0)
                split_coma(0) = DName
            End If
            If Trim(split_coma(split_coma.Length - 1)) = "" Then
                Return country
                Exit Function
            End If
            Dim refcconect As New conect.Dbase_Conction_Mysql_RA
            Dim refclas As New ClassRadicador
            Dim datset As New DataSet
            Dim sqlconsult As String = "Select Correo_Electronico,Nombre_Remitente,Cargo_Remite from remit_dest_interno where Nombre_Remitente like '%" & Trim(split_coma(split_coma.Length - 1)) & "%' or Cargo_Remite like'%" & Trim(split_coma(split_coma.Length - 1)) & "%' or Correo_Electronico like '%" & Trim(split_coma(split_coma.Length - 1)) & "%'  LIMIT 100"
            response = SELECTION_SELECT_FIELD(sqlconsult, datset)
            If response <> "YES" Then
                Return country
                Exit Function
            End If
            If datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To datset.Tables(0).Rows.Count - 1
                    If datset.Tables(0).Rows(i).IsNull(0) = False Then
                        Dim ref_nombre As String = ""
                        If datset.Tables(0).Rows(i).IsNull(1) = True Then
                        Else
                            ref_nombre = datset.Tables(0).Rows(i).Item(1)
                        End If
                        Dim ref_cargo As String = ""
                        If datset.Tables(0).Rows(i).IsNull(2) = True Then
                        Else
                            ref_cargo = datset.Tables(0).Rows(i).Item(2)
                        End If
                        Dim tempo_record As String = datset.Tables(0).Rows(i).Item(0).ToString() & "|(" & ref_nombre & "  " & ref_cargo & ")"
                        tempo_record = tempo_record.Replace(",", "")
                        country.Add(tempo_record)
                    End If

                Next

                For i As Integer = 0 To country.Count - 1
                    For z As Integer = 0 To split_coma.Length - 1
                        If Trim(country(i).ToString) = Trim(split_coma(z)) Then
                            country.RemoveAt(i)
                            'Return country
                            'Exit Function
                        End If
                    Next
                Next
                Return country
            Else
                Return country
            End If
        Catch ex As Exception
            Return country
        End Try
    End Function
    Public Function Deserialize(Of T)(context As String) As T
        Dim jsonData As String = context

        'cast to specified objectType
        Dim obj = DirectCast(New JavaScriptSerializer().Deserialize(Of T)(jsonData), T)
        Return obj
    End Function
    Public Function Deserialize_string(context As String) As Object
        Dim jsonData As String = context

        'cast to specified objectType
        Dim obj = DirectCast(New JavaScriptSerializer().Deserialize(Of String)(jsonData), Object)
        Return obj
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function GetEmpresa2(ByVal prefixText As String, ByVal count As Integer) As String()
        Try

            Dim lista As New List(Of String)
            Dim refcconect As New conect.Dbase_Conction_Mysql_RA
            Dim refclas As New ClassRadicador
            Dim datset As New DataSet
            Dim response As String = ""
            Dim sqlconsult As String = "Select distinct entidad_empresa from destinatario_externo where entidad_empresa like '%" & prefixText & "%' LIMIT 100"
            response = SELECTION_SELECT_FIELD(sqlconsult, datset)
            If response <> "YES" Then
                GetEmpresa2 = lista.ToArray
                Exit Function
            End If
            If datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To datset.Tables(0).Rows.Count - 1
                    lista.Add(datset.Tables(0).Rows(i).Item(0).ToString())
                Next
                GetEmpresa2 = lista.ToArray
            Else
                GetEmpresa2 = lista.ToArray
            End If
        Catch ex As Exception
            GetEmpresa2 = Nothing
        End Try
    End Function
    <WebMethod()>
    Public Function HelloWorld() As String
        Return "Hola a todos"
    End Function
    <WebMethod(EnableSession:=True)>
    Public Function GetData(ByVal DName As String) As List(Of String)
        Try

            Dim result As New List(Of String)()
            Dim refcconect As New conect.Dbase_Conction_Mysql_RA
            Dim refclas As New ClassRadicador
            Dim datset As New DataSet
            Dim response As String = ""
            Dim sqlconsult As String = "Select distinct Nombre_Remitente from destinatario_externo where Nombre_Remitente like '%" & DName & "%' LIMIT 100"
            response = SELECTION_SELECT_FIELD(sqlconsult, datset)
            If response <> "YES" Then
                GetData = result
                Exit Function
            End If
            If datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To datset.Tables(0).Rows.Count - 1
                    result.Add(datset.Tables(0).Rows(i).Item(0).ToString())
                Next
                GetData = result
            Else
                GetData = result
            End If
        Catch ex As Exception
            GetData = Nothing
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    Public Function update_radic_plantilla_radicado(ByVal update As String) As String
        Dim refup As String = update.Replace("|", "'")
        update_radic_plantilla_radicado = MYSQL_INSERT_COMMNAD(refup)
        Return update_radic_plantilla_radicado
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function GetGuiaRadicacon(ByVal DName As String, ByVal DAcampo As String, ByVal DNtable As String) As List(Of String)
        Try

            Dim result As New List(Of String)()
            Dim refcconect As New conect.Dbase_Conction_Mysql_RA
            Dim refclas As New ClassRadicador
            Dim datset As New DataSet
            Dim response As String = ""
            If DName = "" Then
                GetGuiaRadicacon = result
                Exit Function
            End If
            Dim split() As String = DAcampo.Split("|")
            Dim sqlconsult As String = "Select distinct " & split(0) & " from " & DNtable & " where " & split(0) & " like '%" & DName & "%' LIMIT 50"
            response = SELECTION_SELECT_FIELD(sqlconsult, datset)
            If response <> "YES" Then
                GetGuiaRadicacon = result
                Exit Function
            End If
            If datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To datset.Tables(0).Rows.Count - 1
                    Dim obsgetipe As Object = datset.Tables(0).Rows(i).Item(0).GetType.ToString
                    If obsgetipe = "System.DateTime" Then
                        Dim subtrin As String = datset.Tables(0).Rows(i).Item(0).ToString()
                        Dim splitsubtrin() As String = subtrin.Substring(0, 10).Split("/")
                        result.Add(splitsubtrin(2) & "/" & splitsubtrin(1) & "/" & splitsubtrin(0))
                    Else
                        result.Add(datset.Tables(0).Rows(i).Item(0).ToString())
                    End If

                Next
                GetGuiaRadicacon = result
            Else
                GetGuiaRadicacon = result
            End If
        Catch ex As Exception
            GetGuiaRadicacon = Nothing
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    Public Function Getactualiza_service(ByVal DAcampoCompara As String,
                                         ByVal DAcampoActualiza As String,
                                         ByVal DNtable As String,
                                         ByVal DNvalues As String,
                                         ByVal DNvalues_compara As String,
                                         ByVal DAcaponivel As String) As String
        Try

            Dim result As New List(Of String)()
            Dim Result_ As String = ""
            Dim response As String = ""
            Dim refcconect As New conect.Dbase_Conction_Mysql_RA
            Dim Refclas_ra_pro_permisos As New Class_ra_pro_permisos_niveles
            Dim id_usuario_permiso As Integer = 0
            Result_ = Refclas_ra_pro_permisos.Solicita_id_usuario_id_permiso(Val(DNvalues_compara),
                                                                           id_usuario_permiso)
            If Result_ <> "YES" Then
                Return Result_
                Exit Function
            End If
            Dim Refclas As New Class_ra_pro_niveles
            Dim stru_niveles_hijo_() As stru_niveles_hijo = Nothing
            Result_ = Refclas.Solicita_niveles_relacionados_padre_recursive(Val(DAcaponivel),
                                                                           stru_niveles_hijo_,
                                                                           id_usuario_permiso)
            If Result_ <> "YES" Then
                Return Result_
                Exit Function
            End If
            Dim sqlconsult As String = "Update  " & DNtable & " set " & DAcampoActualiza & "='" & DNvalues & "' where " &
                DAcampoCompara & "='" & DNvalues_compara & "'"
            If Not stru_niveles_hijo_ Is Nothing Then
                For i As Integer = 0 To stru_niveles_hijo_.Length - 1
                    If stru_niveles_hijo_(i).estado_repetido <> 0 Then
                        sqlconsult = sqlconsult & " ; " & "Update  " & DNtable & " set " & DAcampoActualiza & "='" & DNvalues & "' where " &
                        DAcampoCompara & "='" & stru_niveles_hijo_(i).estado_repetido & "'"
                    End If
                Next
            End If
            response = refcconect.SELECTION_INSERT_COMMAND(sqlconsult)
            If response <> "YES" Then
                Return Result_
                Exit Function
            Else
                Return "YES"
                Exit Function
            End If

        Catch ex As Exception
            Return "Inconsistencia general funcion  Getactualiza_service " & ex.Message
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function GetGuiaRadicaconasp(ByVal prefixText As String, ByVal count As Integer, ByVal contextKey As String) As String()
        Try
            Dim result As New List(Of String)()
            Dim refcconect As New conect.Dbase_Conction_Mysql_RA
            Dim refclas As New ClassRadicador
            Dim datset As New DataSet
            Dim response As String = ""
            Dim split() As String = contextKey.Split("|")
            Dim sqlconsult As String = ""
            If prefixText = "*." Then
                sqlconsult = "Select distinct " & split(0) & " from " & split(1) & "  LIMIT 100  "
            Else
                sqlconsult = "Select distinct " & split(0) & " from " & split(1) & " where " & split(0) & " like '%" & prefixText & "%' LIMIT 50  "
            End If

            response = SELECTION_SELECT_FIELD(sqlconsult, datset)
            If response <> "YES" Then
                GetGuiaRadicaconasp = result.ToArray
                Exit Function
            End If
            If datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To datset.Tables(0).Rows.Count - 1
                    If datset.Tables(0).Rows(i).IsNull(0) = False Then
                        Dim obsgetipe As Object = datset.Tables(0).Rows(i).Item(0).GetType.ToString
                        If obsgetipe = "System.DateTime" Then
                            Dim subtrin As String = datset.Tables(0).Rows(i).Item(0).ToString()
                            Dim splitsubtrin() As String = subtrin.Substring(0, 10).Split("/")
                            result.Add(splitsubtrin(2) & "/" & splitsubtrin(1) & "/" & splitsubtrin(0))
                        Else
                            result.Add(datset.Tables(0).Rows(i).Item(0).ToString())
                        End If
                    End If
                Next
                GetGuiaRadicaconasp = result.ToArray
            Else
                GetGuiaRadicaconasp = result.ToArray
            End If
        Catch ex As Exception
            GetGuiaRadicaconasp = Nothing
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function GetPosiblesTipos(ByVal prefixText As String, ByVal count As Integer, ByVal contextKey As String) As String()
        Try

            Dim result As New List(Of String)()
            Dim refcconect As New conect.Dbase_Conction_Mysql_RA
            Dim refclas As New ClassRadicador
            Dim datset As New DataSet
            Dim response As String = ""
            'Dim split() As String = contextKey.Split("|")
            Dim sqlconsult As String = ""
            If prefixText = "*." Then
                sqlconsult = "Select distinct Descripcion_Documento from tipo_doc_series  where id_instrumento=" & contextKey & " and Estado_Tipo=1  LIMIT 100  "
            Else
                sqlconsult = "Select distinct Descripcion_Documento from tipo_doc_series where Descripcion_Documento like '%" & prefixText & "%' and id_instrumento=" & contextKey & " and Estado_Tipo=1  LIMIT 50  "
            End If
            response = SELECTION_SELECT_FIELD(sqlconsult, datset)
            If response <> "YES" Then
                GetPosiblesTipos = result.ToArray
                Exit Function
            End If
            If datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To datset.Tables(0).Rows.Count - 1
                    If datset.Tables(0).Rows(i).IsNull(0) = False Then
                        Dim obsgetipe As Object = datset.Tables(0).Rows(i).Item(0).GetType.ToString
                        If obsgetipe = "System.DateTime" Then
                            Dim subtrin As String = datset.Tables(0).Rows(i).Item(0).ToString()
                            Dim splitsubtrin() As String = subtrin.Substring(0, 10).Split("/")
                            result.Add(splitsubtrin(2) & "/" & splitsubtrin(1) & "/" & splitsubtrin(0))
                        Else
                            result.Add(datset.Tables(0).Rows(i).Item(0).ToString())
                        End If
                    End If
                Next
                GetPosiblesTipos = result.ToArray
            Else
                GetPosiblesTipos = result.ToArray
            End If
        Catch ex As Exception
            GetPosiblesTipos = Nothing
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function GetPosiblesTipos_serie_sub_series(ByVal prefixText As String, ByVal count As Integer, ByVal contextKey As String) As String()
        Try

            Dim result As New List(Of String)()
            Dim refcconect As New conect.Dbase_Conction_Mysql_RA
            Dim refclas As New ClassRadicador
            Dim datset As New DataSet
            Dim response As String = ""
            Dim split() As String = contextKey.Split("|")
            Dim condicion As String = ""
            '------Solo tipos documentales de serie
            If split(0) <> "-1" And split(1) = "-1" Then
                condicion = " and Series_Documentales_Id_Series=" & split(0) & " and sub_serie_id_serie is null "
            End If
            If split(1) <> "-1" And split(0) <> "-1" Then
                condicion = " and sub_serie_id_serie=" & split(1) & " and Series_Documentales_Id_Series=" & split(0)
            End If
            Dim sqlconsult As String = ""
            If prefixText = "*." Then
                sqlconsult = "Select distinct Descripcion_Documento from tipo_doc_series  where id_instrumento=" & split(2) & condicion & " and Estado_Tipo=1  LIMIT 100  "
            Else
                sqlconsult = "Select distinct Descripcion_Documento from tipo_doc_series where Descripcion_Documento like '%" & prefixText & "%'" & condicion & " and id_instrumento=" & split(2) & " and Estado_Tipo=1  LIMIT 50  "
            End If
            response = SELECTION_SELECT_FIELD(sqlconsult, datset)
            If response <> "YES" Then
                GetPosiblesTipos_serie_sub_series = result.ToArray
                Exit Function
            End If
            If datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To datset.Tables(0).Rows.Count - 1
                    If datset.Tables(0).Rows(i).IsNull(0) = False Then
                        Dim obsgetipe As Object = datset.Tables(0).Rows(i).Item(0).GetType.ToString
                        If obsgetipe = "System.DateTime" Then
                            Dim subtrin As String = datset.Tables(0).Rows(i).Item(0).ToString()
                            Dim splitsubtrin() As String = subtrin.Substring(0, 10).Split("/")
                            result.Add(splitsubtrin(2) & "/" & splitsubtrin(1) & "/" & splitsubtrin(0))
                        Else
                            result.Add(datset.Tables(0).Rows(i).Item(0).ToString())
                        End If
                    End If
                Next
                GetPosiblesTipos_serie_sub_series = result.ToArray
            Else
                GetPosiblesTipos_serie_sub_series = result.ToArray
            End If
        Catch ex As Exception
            GetPosiblesTipos_serie_sub_series = Nothing
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function GetGuiaRadicacon_interna(ByVal prefixText As String,
                                             ByVal count As Integer,
                                             ByVal contextKey As String) As String()
        Try

            Dim result As New List(Of String)()
            Dim refcconect As New conect.Dbase_Conction_Mysql_RA
            Dim refclas As New ClassRadicador
            Dim datset As New DataSet
            Dim response As String = ""
            Dim split() As String = contextKey.Split("|")
            Dim sqlconsult As String = ""
            If prefixText = "*." Then
                sqlconsult = "Select distinct " & split(0) & " from " & split(1) & " where Usuario_Radicador_id_usuario=" & HttpContext.Current.Session.Item("RA_ID_USUARIO") & " and Flag_Flow=2  LIMIT 100  "
            Else
                sqlconsult = "Select distinct " & split(0) & " from " & split(1) & " where " & split(0) & " like '%" & prefixText & "%'" & " and Usuario_Radicador_id_usuario=" & HttpContext.Current.Session.Item("RA_ID_USUARIO") & " and Flag_Flow=2  LIMIT 100  "
            End If
            response = SELECTION_SELECT_FIELD(sqlconsult, datset)
            If response <> "YES" Then
                GetGuiaRadicacon_interna = result.ToArray
                Exit Function
            End If
            If datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To datset.Tables(0).Rows.Count - 1
                    If datset.Tables(0).Rows(i).IsNull(0) = False Then
                        Dim obsgetipe As Object = datset.Tables(0).Rows(i).Item(0).GetType.ToString
                        If obsgetipe = "System.DateTime" Then
                            Dim subtrin As String = datset.Tables(0).Rows(i).Item(0).ToString()
                            Dim splitsubtrin() As String = subtrin.Substring(0, 10).Split("/")
                            result.Add(splitsubtrin(2) & "/" & splitsubtrin(1) & "/" & splitsubtrin(0))
                        Else
                            result.Add(datset.Tables(0).Rows(i).Item(0).ToString())
                        End If
                    End If
                Next
                GetGuiaRadicacon_interna = result.ToArray
            Else
                GetGuiaRadicacon_interna = result.ToArray
            End If
        Catch ex As Exception
            GetGuiaRadicacon_interna = Nothing
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function GetGuiaRadicaconasp_flow(ByVal prefixText As String, ByVal count As Integer, ByVal contextKey As String) As String()
        Try

            Dim result As New List(Of String)()
            Dim refcconect As New conect.Dbase_Conction_Mysql_RA
            Dim refclas As New ClassRadicador
            Dim datset As New DataSet
            Dim response As String = ""
            Dim split() As String = contextKey.Split("|")
            Dim sqlconsult As String = ""
            If prefixText = "*." Then
                sqlconsult = "Select distinct " & split(0) & " from " & split(1) & " where Flag_Flow=777  LIMIT 100  "
            Else
                sqlconsult = "Select distinct " & split(0) & " from " & split(1) & " where " & split(0) & " like '%" & prefixText & "%' and Flag_Flow=777 LIMIT 50  "
            End If

            response = SELECTION_SELECT_FIELD(sqlconsult, datset)
            If response <> "YES" Then
                GetGuiaRadicaconasp_flow = result.ToArray
                Exit Function
            End If
            If datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To datset.Tables(0).Rows.Count - 1
                    If datset.Tables(0).Rows(i).IsNull(0) = False Then
                        Dim obsgetipe As Object = datset.Tables(0).Rows(i).Item(0).GetType.ToString
                        If obsgetipe = "System.DateTime" Then
                            Dim subtrin As String = datset.Tables(0).Rows(i).Item(0).ToString()
                            Dim splitsubtrin() As String = subtrin.Substring(0, 10).Split("/")
                            result.Add(splitsubtrin(2) & "/" & splitsubtrin(1) & "/" & splitsubtrin(0))
                        Else
                            result.Add(datset.Tables(0).Rows(i).Item(0).ToString())
                        End If
                    End If
                Next
                GetGuiaRadicaconasp_flow = result.ToArray
            Else
                GetGuiaRadicaconasp_flow = result.ToArray
            End If
        Catch ex As Exception
            GetGuiaRadicaconasp_flow = Nothing
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function verifica_cheklis_documentos_radicados(ByVal item_chek As Object,
                                                          ByVal id_tipo_tramite As Object)
        Dim response As String = ""
        Try
            If Session.Item("RA_MODULO_SELECCIONADO") = "" Then
                Return "Imposible establecer el modulo seleciconado|"
                Exit Function
            End If
            Dim split() As String = Session.Item("RA_MODULO_SELECCIONADO").ToString.Split("|")
            Dim result As String = ""
            If split.Length < 3 Then
                Return "La estructura de la variable RA_MODULO_SELECCIONADO no es correcta|"
                Exit Function
            End If
            Dim tipo_plantilla_tramite As String = split(2)
            Dim Ref_class_ra_tipos As New ra_dig_tipos_docum_lista_chequeo
            Dim stru_chek_lista_tramite_() As stru_chek_lista_tramite = Nothing
            result = Ref_class_ra_tipos.Solicita_listar_tipos_documentales_relacion_tramite_radicacion_lista_obligatorio(id_tipo_tramite,
                                                                                                                         tipo_plantilla_tramite,
                                                                                                                         stru_chek_lista_tramite_)
            If result <> "YES" Then
                Return "La estructura de la variable RA_MODULO_SELECCIONADO no es correcta|"
                Exit Function
            End If
            If stru_chek_lista_tramite_ Is Nothing Then
                Return "YES|"
                Exit Function
            End If
            Dim parram() As stru_chek_lista_tramite
            Dim serializer = New JavaScriptSerializer()
            Dim deserialize_user = Nothing
            Dim deserialize_parameter = Nothing
            deserialize_user = serializer.Deserialize(Of List(Of ArrayItem))(item_chek)
            If deserialize_user Is Nothing Then
                Return "Imposible deserealizar los parametros de usuario|"
                Exit Function
            End If
            Dim ids As String = ""
            If deserialize_user.Count = 0 Then
                For i As Integer = 0 To stru_chek_lista_tramite_.Length - 1
                    If i = 0 Then
                        ids = stru_chek_lista_tramite_(i).ID_TIPO_DOCUMENTAL_CHEQUEO
                    Else
                        ids = ids & "," & stru_chek_lista_tramite_(i).ID_TIPO_DOCUMENTAL_CHEQUEO
                    End If
                Next
                Return "YES|" & ids
                Exit Function
            Else
                For z As Integer = 0 To deserialize_user.Count - 1
                    ReDim Preserve parram(z)
                    parram(z).ID_TIPO_DOCUMENTAL_CHEQUEO = deserialize_user(z).value
                    parram(z).estado_cumple = deserialize_user(z).text
                Next
                For i As Integer = 0 To stru_chek_lista_tramite_.Length - 1
                    For k As Integer = 0 To parram.Length - 1
                        If parram(k).ID_TIPO_DOCUMENTAL_CHEQUEO = stru_chek_lista_tramite_(i).ID_TIPO_DOCUMENTAL_CHEQUEO Then
                            stru_chek_lista_tramite_(i).estado_cumple = 1
                        End If
                    Next
                Next
                ids = ""
                For i As Integer = 0 To stru_chek_lista_tramite_.Length - 1
                    If stru_chek_lista_tramite_(i).estado_cumple = 0 Then
                        ids = ids & stru_chek_lista_tramite_(i).ID_TIPO_DOCUMENTAL_CHEQUEO & ","
                    End If
                Next
                Return "YES|" & ids
                Exit Function
            End If
        Catch ex As Exception
            Return ex.Message & "|"
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function GetLista_posibles_destinatarios(ByVal DName As String,
                                                    ByVal nombre_area As String)

        Dim response As String = ""
        Dim country As List(Of String) = New List(Of String)()
        Try

            Dim refcconect As New conect.Dbase_Conction_Mysql_RA
            Dim refclas As New ClassRadicador
            Dim datset As New DataSet
            Dim nombre_ordena As String = "Nombre_Remitente"
            Dim Result As String = ""
            Dim sqlconsulta As String = ""
            If nombre_area = "TODAS LAS AREAS" Or nombre_area = "SELECCIONE" Then
                sqlconsulta = UCase("Select Nombre_Remitente as Nombre,Cargo_Remite as Cargo,adr.Nombre_Area,Correo_Electronico," &
                "se.NOMBRE_SEDE as Nombre_Sede,se.TELEFONOS_SEDE as Telefono_Sede from remit_dest_interno as rdi") &
                " left outer  join sedes_empresa as se on (se.ID_SEDES_EMPRESA=rdi.ID_SEDES_EMPRESA)" &
                " left outer join areas_depart_radicacion as adr on (adr.Codigo_Area=rdi.Areas_Dep_Radicacion_id_Areas_Dep) " &
                " where rdi.Empresa_Gestion_Documental_id_empresa=" & HttpContext.Current.Session.Item("RA_ID_EMPRESA") &
                " and Estado_Usuario=1 order by " & nombre_ordena
            Else
                Dim Id_area_usuario_gestion As Integer = -1
                Dim ref_Class_areas_depart_radicacion As New Class_areas_depart_radicacion
                Result = ref_Class_areas_depart_radicacion.Retorna_id_area_usuario_gestion(HttpContext.Current.Session.Item("RA_ID_ORGANIGRAMA"),
                                                                                           nombre_area,
                                                                                           Id_area_usuario_gestion)
                If Result <> "YES" Then
                    Return country
                End If
                sqlconsulta = "Select Nombre_Remitente as Nombre,Cargo_Remite as Cargo,adr.Nombre_Area,Correo_Electronico," &
                    "se.NOMBRE_SEDE,se.TELEFONOS_SEDE from remit_dest_interno as rdi " &
                    " left outer join sedes_empresa as se on (se.ID_SEDES_EMPRESA=rdi.ID_SEDES_EMPRESA)" &
                    " left outer join areas_depart_radicacion as adr on (adr.Codigo_Area=rdi.Areas_Dep_Radicacion_id_Areas_Dep) " &
                    " where Areas_Dep_Radicacion_id_Areas_Dep=" & Id_area_usuario_gestion &
                    " and Estado_Usuario=1 order by " & nombre_ordena
            End If
            response = SELECTION_SELECT_FIELD(sqlconsulta, datset)
            If response <> "YES" Then
                Return country

            End If
            If datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To datset.Tables(0).Rows.Count - 1
                    For z As Integer = 0 To datset.Tables(0).Columns.Count - 1
                        If datset.Tables(0).Rows(i).IsNull(z) = False Then
                            Dim obsgetipe As Object = datset.Tables(0).Rows(i).Item(z).GetType.ToString
                            Dim estado_exit As String = "NO"
                            If obsgetipe = "System.DateTime" Then
                                Dim subtrin As String = datset.Tables(0).Rows(i).Item(z).ToString()
                                Dim splitsubtrin() As String = subtrin.Substring(0, 10).Split("/")
                                Dim tempo_fecha As String = splitsubtrin(2) & "-" & splitsubtrin(1) & "-" & splitsubtrin(0)
                                Veri_existe_regitro(country,
                                          tempo_fecha,
                                          estado_exit)
                                If estado_exit = "NO" Then
                                    country.Add(tempo_fecha)
                                End If
                            Else
                                Veri_existe_regitro(country,
                                         datset.Tables(0).Rows(i).Item(z).ToString(),
                                         estado_exit)
                                If estado_exit = "NO" Then
                                    country.Add(datset.Tables(0).Rows(i).Item(z).ToString())
                                End If

                            End If
                        End If
                    Next
                Next
                Return country
            Else
                Return country
            End If
        Catch ex As Exception
            Return country
        End Try
    End Function
    Private MYSQL_SELECT_COMMAND As MySqlCommand
    Private MYSQL_INSERT_COMMAND As MySqlCommand
    Private Function MYSQL_INSERT_COMMNAD(ByVal Sql_String As String) As String

        Dim Command_Base As New MySqlCommand(Sql_String)
        Dim conectmyslq As New MySql.Data.MySqlClient.MySqlConnection
        Dim Result As String = Returna_Conexion_Mysql(conectmyslq)
        If Result <> "YES" Then
            MYSQL_INSERT_COMMNAD = "Imposible conectar con la base de datos " & Result
        End If
        Me.MYSQL_INSERT_COMMAND = Command_Base
        Try
            Dim command As New MySqlCommand(Me.MYSQL_INSERT_COMMAND.CommandText, conectmyslq)
            If command.ExecuteNonQuery <> 0 Then
                MYSQL_INSERT_COMMNAD = "YES"
                Return MYSQL_INSERT_COMMNAD
            Else
                MYSQL_INSERT_COMMNAD = "NO"

                Return MYSQL_INSERT_COMMNAD
            End If
            MYSQL_INSERT_COMMNAD = "YES"
        Catch ex As MySqlException
            MYSQL_INSERT_COMMNAD = ex.Message
        Finally
            conectmyslq.Close()
        End Try
    End Function
    Private Function Returna_Conexion_Mysql(ByRef CconectionMysql As MySql.Data.MySqlClient.MySqlConnection) As String
        Dim poltrue As String = "False"
        If HttpContext.Current.Session.Item("RA_ACTIVA_POOL_DBMS") = "1" Then
            poltrue = "True"
        Else
            poltrue = "False"
        End If
        Dim Contenido_Config As String = "Persist Security Info=" _
          & True & ";database=" & HttpContext.Current.Session("RA_DB_NAME_MODULO").ToString _
          & ";server=" & HttpContext.Current.Session("RA_IP_SERVER_MODULO").ToString _
         & ";user id=" & HttpContext.Current.Session("RA_USER_DBMS_MODULO").ToString _
         & ";pwd=" & HttpContext.Current.Session("RA_PASW_DBMS_MODULO").ToString _
         & ";Pooling=" & poltrue & ";Min Pool Size=0;Max Pool Size=" &
         HttpContext.Current.Session.Item("RA_NUMERO_DBMS_CONEX")


        Try
            CconectionMysql = New MySql.Data.MySqlClient.MySqlConnection(Contenido_Config)
            If Not CconectionMysql Is Nothing Then
                CconectionMysql.Open()
            Else
                Returna_Conexion_Mysql = "Imposible conectar en la base de datos"
                Exit Function
            End If
            Returna_Conexion_Mysql = "YES"
        Catch ex As MySqlException
            Returna_Conexion_Mysql = ex.Message
        Finally
            'CconectionMysql = Nothing
        End Try
    End Function
    Public Function SELECTION_SELECT_FIELD(ByVal Sql_String As String, ByRef objet As Object) As String
        Dim Result As String = ""
        SELECTION_SELECT_FIELD = "SELECTION_SELECT_FIELD NO RECONOCE EL DBMS"
        If HttpContext.Current.Session("RA_TYPE_DBMS_MODULO").ToString = "mysql" Then
            Result = MYSQL_SELECT_FIELD(Sql_String, objet)
            If Result <> "YES" Then
                SELECTION_SELECT_FIELD = "Inconsistencia en la funcion SELECTION_SELEC_FIELD LLAMANDO A MYSQL FIELD " & Result
                Exit Function
            Else
                SELECTION_SELECT_FIELD = "YES"
                Exit Function
            End If

        End If

    End Function
    Private Function MYSQL_SELECT_FIELD(ByVal Sql_String As String, ByRef Mysqldatacet As System.Data.DataSet) As String
        Dim Result As String = ""
        MYSQL_SELECT_FIELD = "YES"
        Mysqldatacet = New DataSet
        Dim conectmyslq As New MySql.Data.MySqlClient.MySqlConnection
        Result = Returna_Conexion_Mysql(conectmyslq)
        If Result <> "YES" Then
            MYSQL_SELECT_FIELD = "Imposible conectar con la base de datos " & Result
            Exit Function
        End If
        MYSQL_SELECT_COMMAND = New MySqlCommand(Sql_String)
        Dim DatMysqlAdpter As MySql.Data.MySqlClient.MySqlDataAdapter =
            New MySql.Data.MySqlClient.MySqlDataAdapter(MYSQL_SELECT_COMMAND.CommandText, conectmyslq)
        Try
            DatMysqlAdpter.Fill(Mysqldatacet)
        Catch ex As MySqlException
            MYSQL_SELECT_FIELD = ex.Message
        Finally
            conectmyslq.Close()
        End Try


    End Function
    Public Function desarcaga_archivo_respuesta(ByVal data As Object) As String
        If HttpContext.Current.Request.Files.AllKeys.Any() Then
            Dim httpPostedFile = data
            Dim ruta_virtual As String = "../Temp_Image/" & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & "/DESCARGA/1111111.docx"
            Dim ruta_fisica As String = HttpContext.Current.Server.MapPath(ruta_virtual)
            httpPostedFile.Item(0).SaveAs(ruta_fisica)
        End If
        desarcaga_archivo_respuesta = "YES"
    End Function


    <WebMethod(EnableSession:=True)>
    Public Function Service_radicacion_pqrsd(ByVal tramite As String,
                                             ByVal documento_anexo As String,
                                             ByVal area As String,
                                             ByVal asunto As String,
                                             ByVal descripcion As String,
                                             ByVal correo_copia As String) As IEnumerable(Of ServiceRadicado)
        Dim resultList = New List(Of ServiceRadicado)()
        Dim serviceRadicado As ServiceRadicado = New ServiceRadicado()
        Try
            Dim Result As String = ""
            Dim ClassRadicador As New ClassRadicador
            Dim Refclas As New ClassPqrs
            Dim nombre As String = ""
            Dim nit As String = ""
            Dim anualidad As String = ""
            Result = Refclas.Lista_campos_nit_nombre_usuario_pqr(Session.Item("PQRS_CODIGO_SCRIPT_PLANTILLA"),
                                                                 Session.Item("PQRS_ID_USUARIO_PQRS"),
                                                                 nombre,
                                                                 nit,
                                                                 anualidad)
            If Result <> "YES" Then
                serviceRadicado.error_sistema = Result
                resultList.Add(serviceRadicado)
                Return resultList
            End If
            Dim ruta_virtual_anexo As String = "../Temp_Image/" & "/adjuntos_pqr/" & Session.Item("PQRS_ID_USUARIO_PQRS") & "/"
            Dim ruta_fisica_anexo As String = Server.MapPath(ruta_virtual_anexo)
            If System.IO.Directory.Exists(ruta_fisica_anexo) = False Then
                System.IO.Directory.CreateDirectory(ruta_fisica_anexo)
            End If
            If System.IO.Directory.Exists(ruta_fisica_anexo) = False Then
                serviceRadicado.error_sistema = "Imposible encontrar el directorio (" & ruta_fisica_anexo & ")"
                resultList.Add(serviceRadicado)
                Return resultList
            End If
            Dim fil_name As String = Session.Item("PQRS_ID_USUARIO_PQRS") & ".pdf"
            Dim ruta_documento_detalle As String = ruta_fisica_anexo
            Dim ruta_documento_anexo As String = ""
            If documento_anexo = "" Then
                ruta_documento_anexo = ""
            Else
                ruta_documento_anexo = documento_anexo.Replace("/", "\")
            End If
            Dim refclas_radicado As New ClassRadicador
            Dim resultado_correo As String = ""
            Dim consecutivo_radicado As String = ""
            Dim errores_post_radicacion() As ra_log_error_pqr_publico = Nothing
            Result = ClassRadicador.Registra_radicado_pqr(HttpContext.Current.Session.Item("PQRS_CODIGO_PLANTILLA_RADICADO"),
                                                          "RADICACION ENTRANTE",
                                                          tramite,
                                                          HttpContext.Current.Session.Item("PQRS_NOMBRE_PLANTILLA_RADICADO"),
                                                          nombre,
                                                          "",
                                                          area,
                                                          HttpContext.Current.Session.Item("PQRS_ID_USUARIO_PQRS"),
                                                          asunto,
                                                          ruta_documento_anexo,
                                                          descripcion,
                                                          ruta_documento_detalle,
                                                          resultado_correo,
                                                          correo_copia,
                                                          consecutivo_radicado,
                                                          errores_post_radicacion)
            If Result <> "YES" Then
                serviceRadicado.error_sistema = Result
                serviceRadicado.url_documento = ""
                serviceRadicado.radicado_documento = consecutivo_radicado
                resultList.Add(serviceRadicado)
                Return resultList
            Else
                serviceRadicado.error_sistema = "YES"
                serviceRadicado.url_documento = ruta_virtual_anexo & consecutivo_radicado & ".PDF"
                serviceRadicado.radicado_documento = consecutivo_radicado
                resultList.Add(serviceRadicado)
                Return resultList
            End If
        Catch ex As Exception
            serviceRadicado.error_sistema = ex.Message
            serviceRadicado.url_documento = ""
            serviceRadicado.radicado_documento = ""
            resultList.Add(serviceRadicado)
            Return resultList
        End Try
    End Function
End Class