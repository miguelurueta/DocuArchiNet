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
Imports System.Xml
Imports GemBox.Document.Tables
Imports Ionic.Zip
Imports System.Collections.Generic
Imports GemBox.Document
Imports Newtonsoft.Json
Imports GestionDocumental_Docuarchi.net.Class_config_general_service

Public Class Gestion_respuesta
    Public Property id_respuesta As Integer
    Public Property radicado As String
    Public Property id_remitente_interno As Integer
    Public Property id_remitente_externo As Integer
    Public Property estado_obligatorio As Integer 'Estados 1. obligatoria respuesta   2. Solo confirmacion
    Public Property estado_envio As Integer
    Public Property item_anexos As Object
    Public Property id_anexos As Integer
    Public Property error_gestion As String
    Public Property title As String
    Public Property resultado_label As String
    Public Property url_image As String
    Public Property url_image_electronica As String
    Public Property result_envio_correo As String
    Public Property correo_electronico_envio As String
    Public Property resultado_terminar_tarea As String
    Public Property fecha_limite As String
    Public Property estado_tramite As String
    Public Property name_file As String
    Public Property valor_actualiza As String
End Class
Public Class Gestion_respuesta_enexos
    Public Property id_anexo As Integer
    Public Property nombre_anexo As String
End Class
' Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class WebService_gestion_correspondencia
    Inherits System.Web.Services.WebService
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_solicita_descripcion_gestion_respuesta(ByVal parameter As Object) As IEnumerable(Of Class_config_general_service)
        Dim resultList = New List(Of Class_config_general_service)()
        Dim parameter_gestion As Class_config_general_service = New Class_config_general_service()
        Try
            Dim Result As String = ""
            Dim Class_ra_log_respuesta_radicado As New Class_ra_log_respuesta_radicado
            Result = Class_ra_log_respuesta_radicado.Solicita_datos_gestion_solicitud(parameter, resultList)
            If Result <> "YES" Then
                Return resultList
            Else
                Return resultList
            End If
        Catch ex As Exception
            parameter_gestion.error_gestion = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_Actualiza_datos_gestion_solicitud(ByVal parameter As Object, ByVal id_parameter As Object) As IEnumerable(Of Gestion_respuesta)
        Dim resultList = New List(Of Gestion_respuesta)()
        Dim parameter_gestion As Gestion_respuesta = New Gestion_respuesta()
        Try
            Dim deserialize_parameter = Nothing
            deserialize_parameter = JsonConvert.DeserializeObject(Of List(Of Class_config_general_service))(parameter)
            Dim Result As String = ""
            Dim Class_ra_log_respuesta_radicado As New Class_ra_log_respuesta_radicado
            Result = Class_ra_log_respuesta_radicado.Actualiza_datos_gestion_solicitud(id_parameter, deserialize_parameter)
            If Result <> "YES" Then
                parameter_gestion.error_gestion = Result
                parameter_gestion.resultado_terminar_tarea = ""
                parameter_gestion.result_envio_correo = ""
                resultList.Add(parameter_gestion)
                Return resultList
            Else
                parameter_gestion.error_gestion = "YES"
                parameter_gestion.resultado_terminar_tarea = ""
                parameter_gestion.result_envio_correo = ""
                resultList.Add(parameter_gestion)
                Return resultList
            End If
        Catch ex As Exception
            parameter_gestion.error_gestion = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_eliminar_registra_gestion_respuesta(ByVal parameter As Object) As IEnumerable(Of Gestion_respuesta)
        Dim resultList = New List(Of Gestion_respuesta)()
        Dim parameter_gestion As Gestion_respuesta = New Gestion_respuesta()
        Try
            Dim deserialize_parameter = Nothing
            deserialize_parameter = JsonConvert.DeserializeObject(Of List(Of Class_config_general_service_delete))(parameter)
            Dim Result As String = ""
            Dim Class_ra_log_respuesta_radicado As New Class_ra_log_respuesta_radicado
            Result = Class_ra_log_respuesta_radicado.Elimina_registro_gestion_solicitud(deserialize_parameter)
            If Result <> "YES" Then
                parameter_gestion.error_gestion = Result
                parameter_gestion.resultado_terminar_tarea = ""
                parameter_gestion.result_envio_correo = ""
                resultList.Add(parameter_gestion)
                Return resultList
            Else
                parameter_gestion.error_gestion = "YES"
                parameter_gestion.resultado_terminar_tarea = ""
                parameter_gestion.result_envio_correo = ""
                resultList.Add(parameter_gestion)
                Return resultList
            End If
        Catch ex As Exception
            parameter_gestion.error_gestion = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_registra_gestion_respuesta(ByVal parameter As Object) As IEnumerable(Of Gestion_respuesta)
        Dim resultList = New List(Of Gestion_respuesta)()
        Dim parameter_gestion As Gestion_respuesta = New Gestion_respuesta()
        Try
            Dim Class_config_general_service As New Class_config_general_service
            Dim deserialize_parameter = Nothing
            deserialize_parameter = JsonConvert.DeserializeObject(Of List(Of Class_config_general_service))(parameter)
            Dim Result As String = ""
            Dim Class_ra_log_respuesta_radicado As New Class_ra_log_respuesta_radicado
            Result = Class_ra_log_respuesta_radicado.Registra_gestion_respuesta(deserialize_parameter)
            If Result <> "YES" Then
                parameter_gestion.error_gestion = Result
                parameter_gestion.resultado_terminar_tarea = ""
                parameter_gestion.result_envio_correo = ""
                resultList.Add(parameter_gestion)
                Return resultList
            Else
                parameter_gestion.error_gestion = "YES"
                parameter_gestion.resultado_terminar_tarea = ""
                parameter_gestion.result_envio_correo = ""
                resultList.Add(parameter_gestion)
                Return resultList
            End If
        Catch ex As Exception
            parameter_gestion.error_gestion = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_redirecciona_solicitud_a_entidades(ByVal parameter As Object) As IEnumerable(Of Gestion_respuesta)
        Dim resultList = New List(Of Gestion_respuesta)()
        Dim parameter_gestion As Gestion_respuesta = New Gestion_respuesta()
        Try
            Dim Class_config_general_service As New Class_config_general_service
            Dim deserialize_parameter = Nothing
            deserialize_parameter = JsonConvert.DeserializeObject(Of List(Of Class_config_general_service))(parameter)
            Dim Result As String = ""
            Dim Re_classgestionrespuesta As New Classgestionrespuesta
            Dim resultado_terminar_tarea As String = ""
            Dim estado_envio_correo As String = ""
            Result = Re_classgestionrespuesta.Redirecciona_solicitud_entidad_externa(deserialize_parameter,
                                                                                     resultado_terminar_tarea,
                                                                                     estado_envio_correo)
            If Result <> "YES" Then
                parameter_gestion.error_gestion = Result
                parameter_gestion.resultado_terminar_tarea = ""
                parameter_gestion.result_envio_correo = ""
                resultList.Add(parameter_gestion)
                Return resultList
            Else
                parameter_gestion.error_gestion = "YES"
                parameter_gestion.resultado_terminar_tarea = resultado_terminar_tarea
                parameter_gestion.result_envio_correo = estado_envio_correo
                resultList.Add(parameter_gestion)
                Return resultList
            End If
        Catch ex As Exception
            parameter_gestion.error_gestion = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_start_inicio_gestio_correspondencia(ByVal id_tarea As Object) As IEnumerable(Of Gestion_respuesta)
        Dim resultList = New List(Of Gestion_respuesta)()
        Dim parameter_gestion As Gestion_respuesta = New Gestion_respuesta()
        Try
            Dim Result As String = ""
            Dim Re_classgestionrespuesta As New Classgestionrespuesta
            Result = Re_classgestionrespuesta.inicio_gestion_correspondencia(id_tarea,
                                                                             parameter_gestion)
            If Result <> "YES" Then
                parameter_gestion.error_gestion = Result
                resultList.Add(parameter_gestion)
                Return resultList
            Else
                parameter_gestion.error_gestion = "YES"
                resultList.Add(parameter_gestion)
                Return resultList
            End If
        Catch ex As Exception
            parameter_gestion.error_gestion = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_elimina_documento_respuesta(ByVal id_respuesta As Object) As IEnumerable(Of Gestion_respuesta)
        Dim resultList = New List(Of Gestion_respuesta)()
        Dim parameter_gestion As Gestion_respuesta = New Gestion_respuesta()
        Try
            Dim Result As String = ""
            Dim Re_classgestionrespuesta As New Classgestionrespuesta
            Dim Url_image As String = ""
            Result = Re_classgestionrespuesta.Elimina_documento_respuesta(id_respuesta,
                                                                          Url_image)
            If Result <> "YES" Then
                parameter_gestion.error_gestion = Result
                resultList.Add(parameter_gestion)
                Return resultList
            Else
                parameter_gestion.error_gestion = "YES"
                parameter_gestion.url_image = Url_image
                resultList.Add(parameter_gestion)
                Return resultList
            End If
        Catch ex As Exception
            parameter_gestion.error_gestion = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_elimina_anexo_respuesta(ByVal id_respuesta As Object, ByVal id_anexo As Object) As IEnumerable(Of Gestion_respuesta)
        Dim resultList = New List(Of Gestion_respuesta)()
        Dim parameter_gestion As Gestion_respuesta = New Gestion_respuesta()
        Try
            Dim Result As String = ""
            Dim Re_classgestionrespuesta As New Classgestionrespuesta
            Dim Url_image As String = ""
            Result = Re_classgestionrespuesta.Eliminar_anexo_documento_respuesta(id_anexo,
                                                                                 id_respuesta)
            If Result <> "YES" Then
                parameter_gestion.error_gestion = Result
                resultList.Add(parameter_gestion)
                Return resultList
            Else
                parameter_gestion.error_gestion = "YES"
                parameter_gestion.id_anexos = id_anexo
                resultList.Add(parameter_gestion)
                Return resultList
            End If
        Catch ex As Exception
            parameter_gestion.error_gestion = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_url_anexo_respuesta(ByVal id_anexo As Object) As IEnumerable(Of Gestion_respuesta)
        Dim resultList = New List(Of Gestion_respuesta)()
        Dim parameter_gestion As Gestion_respuesta = New Gestion_respuesta()
        Try
            Dim Result As String = ""
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim Url_image As String = ""
            Dim name_file As String = ""
            Result = ClassDaGabinete.Solicita_url_descarga_anexo_respuesta(id_anexo,
                                                                           Url_image,
                                                                           name_file)
            If Result <> "YES" Then
                parameter_gestion.error_gestion = Result
                parameter_gestion.name_file = name_file
                parameter_gestion.url_image = Url_image
                resultList.Add(parameter_gestion)
                Return resultList
            Else
                parameter_gestion.error_gestion = "YES"
                parameter_gestion.url_image = Url_image
                parameter_gestion.name_file = name_file
                resultList.Add(parameter_gestion)
                Return resultList
            End If
        Catch ex As Exception
            parameter_gestion.error_gestion = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_url_documento_respuesta(ByVal id_respuesta_radicado As Object, ByVal formato As Object, ByVal estado_firma As Object) As IEnumerable(Of Gestion_respuesta)
        Dim resultList = New List(Of Gestion_respuesta)()
        Dim parameter_gestion As Gestion_respuesta = New Gestion_respuesta()
        Try
            Dim Result As String = ""
            Dim ClassDaGabinete As New Classgestionrespuesta
            Dim Url_image As String = ""
            Dim name_file As String = ""
            Result = ClassDaGabinete.Descarga_documento_respuesta(id_respuesta_radicado,
                                                                 formato,
                                                                 estado_firma,
                                                                 Url_image,
                                                                 name_file)
            If Result <> "YES" Then
                parameter_gestion.error_gestion = Result
                parameter_gestion.url_image = ""
                parameter_gestion.name_file = name_file
                resultList.Add(parameter_gestion)
                Return resultList
            Else
                parameter_gestion.error_gestion = "YES"
                parameter_gestion.url_image = Url_image
                parameter_gestion.name_file = name_file
                resultList.Add(parameter_gestion)
                Return resultList
            End If
        Catch ex As Exception
            parameter_gestion.error_gestion = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_archiva_traite_solicitud(ByVal correo_token As Object,
                                                     ByVal confirma_correo As Object) As IEnumerable(Of Gestion_respuesta)
        Dim resultList = New List(Of Gestion_respuesta)()
        Dim parameter_gestion As Gestion_respuesta = New Gestion_respuesta()
        Try
            Dim Result As String = ""
            Dim Classgestionrespuesta As New Classgestionrespuesta
            Dim Resultado_correo_envio As String = ""
            Dim Resultado_terminar As String = ""
            Result = Classgestionrespuesta.Archiva_tramite_de_la_solicitud(correo_token,
                                                                           Val(confirma_correo),
                                                                           Resultado_correo_envio,
                                                                           Resultado_terminar)
            If Result <> "YES" Then
                parameter_gestion.error_gestion = Result
                resultList.Add(parameter_gestion)
                Return resultList
            Else
                parameter_gestion.error_gestion = Result
                parameter_gestion.result_envio_correo = Resultado_correo_envio
                parameter_gestion.resultado_terminar_tarea = Resultado_terminar
                resultList.Add(parameter_gestion)
                Return resultList
            End If
        Catch ex As Exception
            parameter_gestion.error_gestion = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_Responder_a_la_solicitud(ByVal id_respuesta_radicado As Object,
                                                     ByVal estado_envia_ventanilla As Object,
                                                     ByVal estado_envia_correo_electronico As Object,
                                                     ByVal estado_firma_digital As Object,
                                                     ByVal id_usuario_gestion_firma As Object,
                                                     ByVal correo_electronico_envio As Object,
                                                     ByVal tipo_respuesta As String) As IEnumerable(Of Gestion_respuesta)
        Dim resultList = New List(Of Gestion_respuesta)()
        Dim parameter_gestion As Gestion_respuesta = New Gestion_respuesta()
        Try
            Dim Result As String = ""
            Dim Classgestionrespuesta As New Classgestionrespuesta
            Dim Resultado_correo As String = ""
            Dim Url_image As String = ""
            Dim Url_image_electronica As String = ""
            ComponentInfo.SetLicense("DTFX-JTBY-6RJK-Y101")
            Result = Classgestionrespuesta.Responder_a_la_solicitud(id_respuesta_radicado,
                                                                    estado_envia_ventanilla,
                                                                    estado_envia_correo_electronico,
                                                                    estado_firma_digital,
                                                                    id_usuario_gestion_firma,
                                                                    correo_electronico_envio,
                                                                    tipo_respuesta,
                                                                    Resultado_correo,
                                                                    Url_image,
                                                                    Url_image_electronica)
            If Result <> "YES" Then
                parameter_gestion.error_gestion = Result
                resultList.Add(parameter_gestion)
                Return resultList
            Else
                parameter_gestion.error_gestion = Result
                parameter_gestion.result_envio_correo = Resultado_correo
                parameter_gestion.url_image = Url_image
                parameter_gestion.url_image_electronica = Url_image_electronica
                resultList.Add(parameter_gestion)
                Return resultList
            End If
        Catch ex As Exception
            parameter_gestion.error_gestion = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_finalizar_tramite(ByVal id_tarea As Object) As IEnumerable(Of Gestion_respuesta)
        Dim resultList = New List(Of Gestion_respuesta)()
        Dim parameter_gestion As Gestion_respuesta = New Gestion_respuesta()
        Try
            Dim Result As String = ""
            Dim Classgestionrespuesta As New Classgestionrespuesta
            Result = Classgestionrespuesta.Finalizar_tramite(id_tarea)
            If Result <> "YES" Then
                parameter_gestion.error_gestion = Result
                resultList.Add(parameter_gestion)
                Return resultList
            Else
                parameter_gestion.error_gestion = Result
                resultList.Add(parameter_gestion)
                Return resultList
            End If
        Catch ex As Exception
            parameter_gestion.error_gestion = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_reasigna_tramite(ByVal id_tarea As Object,
                                             ByVal usuario_tokenize As Object) As IEnumerable(Of Gestion_respuesta)
        Dim resultList = New List(Of Gestion_respuesta)()
        Dim parameter_gestion As Gestion_respuesta = New Gestion_respuesta()
        Try
            Dim Result As String = ""
            Dim Class_gestion_correspondencia As New Class_gestion_correspondencia
            Dim Resultado_correo As String = "YES"
            If HttpContext.Current.Session.Item("REASIGNA_RESPUESTA_TRAMITE") = 0 Then
                parameter_gestion.error_gestion = "El usuario no tiene permiso para reasignar el tramite"
                resultList.Add(parameter_gestion)
                Return resultList
            End If
            If Val(HttpContext.Current.Session.Item("Id_Usuario_Workflow")) = Val(usuario_tokenize) Then
                parameter_gestion.error_gestion = "El usuario no se puede reasignar el tramite así mismo "
                resultList.Add(parameter_gestion)
                Return resultList
            End If
            Result = Class_gestion_correspondencia.Pre_reasigna_tarea_gestion_correspondencia(id_tarea,
                                                                                              usuario_tokenize,
                                                                                              0,
                                                                                              0,
                                                                                              0,
                                                                                              "sin autorizacion usuario permitido",
                                                                                              Resultado_correo)
            If Result <> "YES" Then
                parameter_gestion.error_gestion = Result
                resultList.Add(parameter_gestion)
                Return resultList
            Else
                parameter_gestion.error_gestion = Result
                parameter_gestion.result_envio_correo = Resultado_correo
                resultList.Add(parameter_gestion)
                Return resultList
            End If
        Catch ex As Exception
            parameter_gestion.error_gestion = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_Confirma_recibido_de_la_solicitud(ByVal id_respuesta_radicado As Object,
                                                              ByVal estado_envia_correo_electronico As Object,
                                                              ByVal nota_confirma As Object,
                                                              ByVal correo_electronico_envio As Object,
                                                              ByVal tipo_respuesta As Object) As IEnumerable(Of Gestion_respuesta)
        Dim resultList = New List(Of Gestion_respuesta)()
        Dim parameter_gestion As Gestion_respuesta = New Gestion_respuesta()
        Try
            Dim Result As String = ""
            Dim ClassDaGabinete As New Classgestionrespuesta
            Dim Resultado_correo As String = "YES"
            Dim Url_image As String = ""
            Dim Url_image_electronica As String = ""
            Result = ClassDaGabinete.Confirma_recibido_de_la_solicitud(id_respuesta_radicado,
                                                                       estado_envia_correo_electronico,
                                                                       nota_confirma,
                                                                       correo_electronico_envio,
                                                                       tipo_respuesta,
                                                                       Url_image,
                                                                       Url_image_electronica,
                                                                       Resultado_correo,
                                                                       nota_confirma)
            If Result <> "YES" Then
                parameter_gestion.error_gestion = Result
                resultList.Add(parameter_gestion)
                Return resultList
            Else
                parameter_gestion.error_gestion = Result
                parameter_gestion.result_envio_correo = Resultado_correo
                parameter_gestion.url_image = Url_image
                parameter_gestion.url_image_electronica = Url_image_electronica
                resultList.Add(parameter_gestion)
                Return resultList
            End If
        Catch ex As Exception
            parameter_gestion.error_gestion = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_Reversa_gestion_tramite_usuario_autorizado(ByVal id_respuesta_radicado As Object,
                                                                       ByVal login_usuario As Object,
                                                                       ByVal pasword_usuario As Object,
                                                                       ByVal valid As Object) As IEnumerable(Of Gestion_respuesta)
        Dim resultList = New List(Of Gestion_respuesta)()
        Dim parameter_gestion As Gestion_respuesta = New Gestion_respuesta()
        Try
            Dim Result As String = ""
            Dim ClassDaGabinete As New Classgestionrespuesta
            Dim Resultado_correo As String = ""
            Dim Url_image As String = ""
            Dim Url_image_electronica As String = ""
            Result = ClassDaGabinete.Reversa_gestion_tramite_usuario_autorizado(id_respuesta_radicado,
                                                                                login_usuario,
                                                                                pasword_usuario,
                                                                                valid,
                                                                                Url_image,
                                                                                Url_image_electronica)
            If Result <> "YES" Then
                parameter_gestion.error_gestion = Result
                resultList.Add(parameter_gestion)
                Return resultList
            Else
                parameter_gestion.error_gestion = Result
                parameter_gestion.result_envio_correo = Resultado_correo
                parameter_gestion.url_image = Url_image
                parameter_gestion.url_image_electronica = Url_image_electronica
                resultList.Add(parameter_gestion)
                Return resultList
            End If
        Catch ex As Exception
            parameter_gestion.error_gestion = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_notifica_gestion_correo_electronico(ByVal id_respuesta_radicado As Object,
                                                                ByVal correo_electronico_envio As Object,
                                                                ByVal estado_anexo As Object) As IEnumerable(Of Gestion_respuesta)
        Dim resultList = New List(Of Gestion_respuesta)()
        Dim parameter_gestion As Gestion_respuesta = New Gestion_respuesta()
        Try
            Dim Result As String = ""
            Dim ClassDaGabinete As New Classgestionrespuesta
            Dim Refclas_ra_respuesta As New Class_ra_respuesta_radicado
            Dim Resultado_correo As String = ""
            Dim Url_image As String = ""
            Dim Url_image_electronica As String = ""
            Dim stru_envi As stru_envio = Nothing
            Result = Refclas_ra_respuesta.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta_radicado,
                                                                                           stru_envi)
            If Result <> "YES" Then
                parameter_gestion.error_gestion = Result
                resultList.Add(parameter_gestion)
                Return resultList
            End If
            If stru_envi.TIPO_RESPUESTA_ELAB_USUARIO = "1" Then
                Result = ClassDaGabinete.Confirma_respuesta_al_correo_con_radicado(id_respuesta_radicado,
                                                                                   correo_electronico_envio,
                                                                                   estado_anexo,
                                                                                   Url_image,
                                                                                   Url_image_electronica)
                If Result <> "YES" Then
                    parameter_gestion.error_gestion = Result
                    resultList.Add(parameter_gestion)
                    Return resultList
                Else
                    parameter_gestion.error_gestion = Result
                    parameter_gestion.result_envio_correo = Resultado_correo
                    parameter_gestion.url_image = Url_image
                    parameter_gestion.url_image_electronica = Url_image_electronica
                    resultList.Add(parameter_gestion)
                    Return resultList
                End If
            Else
                Result = ClassDaGabinete.Confirma_respuesta_al_correo_con_sin_radicado(id_respuesta_radicado,
                                                                                       correo_electronico_envio,
                                                                                       estado_anexo,
                                                                                       Url_image,
                                                                                       Url_image_electronica)
                If Result <> "YES" Then
                    parameter_gestion.error_gestion = Result
                    resultList.Add(parameter_gestion)
                    Return resultList
                Else
                    parameter_gestion.error_gestion = Result
                    parameter_gestion.result_envio_correo = Resultado_correo
                    parameter_gestion.url_image = Url_image
                    parameter_gestion.url_image_electronica = Url_image_electronica
                    resultList.Add(parameter_gestion)
                    Return resultList
                End If
            End If

        Catch ex As Exception
            parameter_gestion.error_gestion = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_activa_soicitud_aprobacion(ByVal id_respuesta_radicado As Object) As IEnumerable(Of Gestion_respuesta)
        Dim resultList = New List(Of Gestion_respuesta)()
        Dim parameter_gestion As Gestion_respuesta = New Gestion_respuesta()
        Try
            Dim Result As String = ""
            Dim ClassDaGabinete As New Classgestionrespuesta
            Dim ClassRaSolicitudesAprobacion As New ClassRaSolicitudesAprobacion
            Dim fecha_limite As String = ""
            Result = ClassRaSolicitudesAprobacion.Verfica_viabilidad_solicitud_aprobacion_respuesta(id_respuesta_radicado,
                                                                                                    fecha_limite)
            If Result <> "YES" Then
                parameter_gestion.error_gestion = Result
                parameter_gestion.fecha_limite = fecha_limite
                resultList.Add(parameter_gestion)
                Return resultList
            Else
                parameter_gestion.error_gestion = Result
                parameter_gestion.fecha_limite = fecha_limite
                resultList.Add(parameter_gestion)
                Return resultList
            End If
        Catch ex As Exception
            parameter_gestion.error_gestion = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_solicita_estado_tramite_tarea_workflow(ByVal id_tarea As Object) As IEnumerable(Of Gestion_respuesta)
        Dim resultList = New List(Of Gestion_respuesta)()
        Dim parameter_gestion As Gestion_respuesta = New Gestion_respuesta()
        Try
            Dim Result As String = ""
            Dim Refclass As New Class_DAT_ADIC_TAR
            Dim estado_tramite As String = ""
            Result = Refclass.Solicita_estado_tramite_tarea_workflow(Session.Item("WF_RUTAWORKFLOW"),
                                                                     id_tarea,
                                                                     estado_tramite)
            If Result <> "YES" Then
                parameter_gestion.error_gestion = Result
                parameter_gestion.estado_tramite = estado_tramite
                resultList.Add(parameter_gestion)
                Return resultList
            Else
                parameter_gestion.error_gestion = Result
                parameter_gestion.estado_tramite = estado_tramite
                resultList.Add(parameter_gestion)
                Return resultList
            End If
        Catch ex As Exception
            parameter_gestion.error_gestion = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
End Class