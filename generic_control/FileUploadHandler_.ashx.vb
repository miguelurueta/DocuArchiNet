Imports System.Web
Imports System.Web.Services
Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports Newtonsoft.Json
Public Class UploadFilesResult
    Public Property name As String
    Public Property size As Long
    Public Property type As String
    Public Property url As String
    Public Property ruta As String
    Public Property deleteUrl As String
    Public Property thumbnailUrl As String
    Public Property deleteType As String
    Public Property name_gabinete As String
    Public Property id_image As Long
    Public Property radicado As String
    Public Property tipodocumental As String
    Public Property notitipodocumental As String
    Public Property id_tarea_workflow As Long
    Public Property error_sistema As String
    Public Property estado_firma_digital As Integer
    Public Property contador_paginas As String
    Public Property id_tipo_envio_respuesta As Integer
    Public Property url_image_semaforo As String
    Public Property id_anexo As Integer
    Public Property nombre_anexo As String
    Public Property icono_icono_awe_some As String
    Public Property fecha As String
    Public Property aleas As String
    Public Property id_registro As Long
    Public Property nombre_archivo As String
    Public Property ruta_archivo As String
    Public Property Class_list_detalle_version_document As New List(Of class_list_detalle_version_document)
    Public Property row_table_boot As Object
    Public Property obj_field_boot_table As Object
    Public Property name_modulo As String
    'Public Property Class_list_detalle_version_document As class_list_detalle_version_document
End Class
Public Class JsonFiles
    Public files As UploadFilesResult()
    Public Property TempFolder As String

    Public Sub New(ByVal filesList As List(Of UploadFilesResult))
        files = New UploadFilesResult(filesList.Count - 1) {}
        For i As Integer = 0 To filesList.Count - 1
            files(i) = filesList.ElementAt(i)
        Next
    End Sub
End Class
Public Class FileUploadHandler_
    Implements System.Web.IHttpHandler, System.Web.SessionState.IRequiresSessionState
    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        Dim resultList = New List(Of UploadFilesResult)()
        Dim jFilesJson As String = ""
        Dim uploadFiles As UploadFilesResult = New UploadFilesResult()
        Try
            If context.Request.Files.Count > 0 Then
                Dim file As HttpPostedFile = context.Request.Files(0)
                Dim stru_datos_image_lista As stru_datos_image_lista = Nothing
                Dim ref_calssAlamacenamiento As New ClassAlmacenamiento
                Dim Classgestionrespuesta As New Classgestionrespuesta
                Dim Result As String = ""
                Dim name_funcion As String = context.Request("funcion")
                Dim nombre_gabinete As String = context.Request("gabinete")
                Dim id_tipo_documento As Integer = context.Request("id_tipo_documento")
                Dim nombre_tipo_documento As String = context.Request("nombre_tipo_documento")
                Dim estado_adjunta_relacionado As Integer = context.Request("chek_adjunta_relacionado")
                Dim estado_adjunta_anexo As Integer = context.Request("chek_adjunta_anexo")
                Dim evento_adjunta As String = context.Request("evento_adjunta")
                Dim numero_documento_relacionado As String = context.Request("num_docu_relacion")
                Dim id_respuesta As String = context.Request("id_respuesta")
                Dim tipo_adjunta As Integer = context.Request("tipo_adjunta")
                Dim id_expediente As Integer = context.Request("id_respuesta")
                Dim name_modulo As String = context.Request("name_modulo")
                Dim FechaCarga As String = context.Request("FechaCarga")
                Dim id_imagen As Integer = 0
                If id_expediente = 0 Then
                    id_expediente = HttpContext.Current.Session.Item("PG_SELECCION_ID_EXPEIDENTE")
                End If
                Dim pat_user As String = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
                Dim path As String = context.Server.MapPath("../Temp_Image/upload_file/" & pat_user & "/")
                If Directory.Exists(path) = False Then
                    Directory.CreateDirectory(path)
                End If
                Dim path_temp As String = context.Server.MapPath("../Temp_Image/upload_file_tiif/" & pat_user & "/")
                If Directory.Exists(path_temp) = False Then
                    Directory.CreateDirectory(path_temp)
                End If
                file.SaveAs(path & file.FileName)
                uploadFiles.name = file.FileName
                uploadFiles.size = file.ContentLength
                uploadFiles.type = "image/jpeg"
                uploadFiles.url = "/Temp_Image/upload_file/" & file.FileName
                uploadFiles.deleteUrl = "/FileUploadHandler.ashx?file=" & file.FileName
                uploadFiles.thumbnailUrl = "/Temp_Image/upload_file/" & file.FileName
                uploadFiles.deleteType = "GET"
                uploadFiles.error_sistema = "Evento no registrado " & evento_adjunta
                uploadFiles.ruta_archivo = path & file.FileName
                context.Response.ContentType = "application/json"
                Dim id_tarea_workflow As Long = 0
                Dim contador As String = ""
                If evento_adjunta = "GESTION_PQRS" Then
                    uploadFiles.error_sistema = "YES"
                    uploadFiles.ruta_archivo = uploadFiles.ruta_archivo.Replace("\", "/")
                End If
                Dim Class_ra_ver_version_documento As New Class_ra_ver_version_documento
                If evento_adjunta = "ADJUNTAVERSION" Then
                    Dim modulo_adjunta_version As String = name_funcion
                    Dim optiom_replza_version As Integer = tipo_adjunta
                    id_imagen = context.Request("id_image")
                    Dim item_ilist As class_list_detalle_version_document
                    item_ilist = New class_list_detalle_version_document
                    Dim Resultado As String = ""
                    Resultado = Class_ra_ver_version_documento.AdjuntaVersionDocumento(modulo_adjunta_version,
                                                                                         optiom_replza_version,
                                                                                         nombre_gabinete,
                                                                                         id_imagen,
                                                                                         uploadFiles.ruta_archivo,
                                                                                         HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                         HttpContext.Current.Session.Item("ID_USUARIO_DOCUARCHI"),
                                                                                         HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION"),
                                                                                         HttpContext.Current.Session.Item("DA_Login_Usuario"),
                                                                                         "",
                                                                                         item_ilist)


                    item_ilist.error_sistema = Resultado
                    uploadFiles.Class_list_detalle_version_document.Add(item_ilist)
                    uploadFiles.error_sistema = Resultado

                End If
                If evento_adjunta = "REMPLAZAVERSION" Then
                    Dim modulo_adjunta_version As String = name_funcion
                    Dim optiom_replza_version As Integer = tipo_adjunta
                    Dim name_modulo_ As String = name_modulo
                    id_imagen = context.Request("id_image")
                    Dim item_ilist As class_list_detalle_version_document
                    item_ilist = New class_list_detalle_version_document
                    Dim Resultado As String = ""
                    Resultado = Class_ra_ver_version_documento.AdjuntaVersionDocumento(name_modulo_,
                                                                                         optiom_replza_version,
                                                                                         nombre_gabinete,
                                                                                         id_imagen,
                                                                                         uploadFiles.ruta_archivo,
                                                                                         HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                         HttpContext.Current.Session.Item("ID_USUARIO_DOCUARCHI"),
                                                                                         HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION"),
                                                                                         HttpContext.Current.Session.Item("DA_Login_Usuario"),
                                                                                         "",
                                                                                         item_ilist)


                    item_ilist.error_sistema = Resultado
                    uploadFiles.Class_list_detalle_version_document.Add(item_ilist)
                    uploadFiles.error_sistema = Resultado
                End If
                Dim Class_ra_mig_registro_migracion As New Class_ra_mig_registro_migracion
                Dim class_stru_list_vew_migra_documento As class_stru_list_vew_migra_documento = Nothing

                If evento_adjunta = "INTRUESII" Then
                    Dim Class_Integracion_SII As New Class_Integracion_SII
                    Dim Class_general_data As Object = Nothing
                    Result = Class_Integracion_SII.Solicita_lista_archivo_sii_rue(uploadFiles.ruta_archivo,
                                                                                  "INTRUESII",
                                                                                  1,
                                                                                  uploadFiles.row_table_boot,
                                                                                  uploadFiles.obj_field_boot_table)
                    If Result <> "YES" Then
                        uploadFiles.error_sistema = Result
                    Else
                        uploadFiles.error_sistema = "YES"
                        uploadFiles.name_gabinete = ""
                        uploadFiles.id_image = 0
                        uploadFiles.id_registro = 0
                        uploadFiles.url = ""
                        uploadFiles.ruta = uploadFiles.ruta_archivo
                    End If
                End If
                If evento_adjunta = "INTVIRTUALSII" Then
                    Dim Class_Integracion_SII As New Class_Integracion_SII
                    Dim Class_general_data As Object = Nothing
                    Result = Class_Integracion_SII.Solicita_lista_archivo_virtual_sii(uploadFiles.ruta_archivo,
                                                                                     "INTVIRTUALSII",
                                                                                     1,
                                                                                     uploadFiles.row_table_boot,
                                                                                     uploadFiles.obj_field_boot_table)
                    If Result <> "YES" Then
                        uploadFiles.error_sistema = Result
                    Else
                        uploadFiles.error_sistema = "YES"
                        uploadFiles.name_gabinete = ""
                        uploadFiles.id_image = 0
                        uploadFiles.id_registro = 0
                        uploadFiles.url = ""
                        uploadFiles.ruta = uploadFiles.ruta_archivo
                    End If
                End If
                If evento_adjunta = "MIGRACION" Then
                    id_imagen = context.Request("id_image")
                    Result = Class_ra_mig_registro_migracion.Adjunta_documento_migracion(id_imagen,
                                                                                         nombre_gabinete,
                                                                                         uploadFiles.ruta_archivo,
                                                                                         HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                         HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION"),
                                                                                         class_stru_list_vew_migra_documento)
                    If Result <> "YES" Then
                        uploadFiles.error_sistema = Result
                    Else
                        uploadFiles.error_sistema = "YES"
                        uploadFiles.name_gabinete = nombre_gabinete
                        uploadFiles.id_image = id_imagen
                        uploadFiles.id_registro = class_stru_list_vew_migra_documento.id_registro_migracion
                        uploadFiles.url = class_stru_list_vew_migra_documento.url_ruta_documento
                        uploadFiles.ruta = class_stru_list_vew_migra_documento.ruta_documento
                    End If
                End If
                If evento_adjunta = "GESTION_RESPUESTA" Then
                    HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA") = path & file.FileName
                    Result = ref_calssAlamacenamiento.UploadSaveFile(id_expediente,
                                                                     id_tipo_documento,
                                                                     nombre_tipo_documento,
                                                                     estado_adjunta_anexo,
                                                                     estado_adjunta_relacionado,
                                                                     numero_documento_relacionado,
                                                                     FechaCarga,
                                                                     stru_datos_image_lista,
                                                                     id_tarea_workflow,
                                                                     contador)
                    If Result <> "YES" Then
                        uploadFiles.error_sistema = Result
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
                    End If
                End If
                If evento_adjunta = "WORKFLOWSELECCION" Then
                    HttpContext.Current.Session.Item("WF_TIPO_ADJUNTA") = "LISTA"
                    HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA") = path & file.FileName
                    Result = ref_calssAlamacenamiento.UploadSaveFile(id_expediente,
                                                                     id_tipo_documento,
                                                                     nombre_tipo_documento,
                                                                     estado_adjunta_anexo,
                                                                     estado_adjunta_relacionado,
                                                                     numero_documento_relacionado,
                                                                     FechaCarga,
                                                                     stru_datos_image_lista,
                                                                     id_tarea_workflow,
                                                                     contador)
                    If Result <> "YES" Then
                        uploadFiles.error_sistema = Result
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
                    End If
                End If
                If evento_adjunta = "WORKFLOWENLACE" Then
                    HttpContext.Current.Session.Item("WF_TIPO_ADJUNTA") = "ENLACE"
                    HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA") = path & file.FileName
                    Result = ref_calssAlamacenamiento.UploadSaveFile(id_expediente,
                                                                     id_tipo_documento,
                                                                     nombre_tipo_documento,
                                                                     estado_adjunta_anexo,
                                                                     estado_adjunta_relacionado,
                                                                     numero_documento_relacionado,
                                                                     FechaCarga,
                                                                     stru_datos_image_lista,
                                                                     id_tarea_workflow,
                                                                     contador)
                    If Result <> "YES" Then
                        uploadFiles.error_sistema = Result
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
                    End If
                End If
                If evento_adjunta = "ADJUNTARADICACION" Then
                    HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA") = path & file.FileName
                    HttpContext.Current.Session.Item("WF_TIPO_ADJUNTA") = "ADJUNTARADICACION"
                    Result = ref_calssAlamacenamiento.UploadSaveFile(id_expediente,
                                                                     id_tipo_documento,
                                                                     nombre_tipo_documento,
                                                                     estado_adjunta_anexo,
                                                                     estado_adjunta_relacionado,
                                                                     numero_documento_relacionado,
                                                                     FechaCarga,
                                                                     stru_datos_image_lista,
                                                                     id_tarea_workflow,
                                                                     contador)
                    If Result <> "YES" Then
                        uploadFiles.error_sistema = Result
                    Else
                        Dim item_ilist As class_list_detalle_version_document
                        item_ilist = New class_list_detalle_version_document
                        item_ilist.DBT = stru_datos_image_lista.DBT
                        item_ilist.ESTADO_FIRMA_DIGITAL = stru_datos_image_lista.estado_firma_digital
                        item_ilist.IconoAsome = stru_datos_image_lista.icono_icono_awe_some
                        item_ilist.TIPO_ARCHIVO = stru_datos_image_lista.tipodocumental
                        uploadFiles.Class_list_detalle_version_document.Add(item_ilist)
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
                        uploadFiles.error_sistema = "YES"
                    End If
                End If
                If evento_adjunta = "PRODUCCION" Then
                    HttpContext.Current.Session.Item("WF_TIPO_ADJUNTA") = "PRODUCCION"
                    HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA") = path & file.FileName
                    Result = ref_calssAlamacenamiento.UploadSaveFile(id_expediente,
                                                                     id_tipo_documento,
                                                                     nombre_tipo_documento,
                                                                     estado_adjunta_anexo,
                                                                     estado_adjunta_relacionado,
                                                                     numero_documento_relacionado,
                                                                     FechaCarga,
                                                                     stru_datos_image_lista,
                                                                     id_tarea_workflow,
                                                                     contador)
                    If Result <> "YES" Then
                        uploadFiles.error_sistema = Result
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
                    End If
                End If
                Dim radicado As String = ""
                Dim id_tipo_envio_respuesta As Integer = 0
                Dim img_url As String = ""
                If evento_adjunta = "SUBE_RESPUESTA" Then
                    If tipo_adjunta = 1 Then
                        HttpContext.Current.Session.Item("EXTENSION_ARCHIVO_ADJUNTA") = path & file.FileName
                        Result = Classgestionrespuesta.upload_sube_formato_respuesta_radicado(id_respuesta,
                                                                                              HttpContext.Current.Session.Item("EXTENSION_ARCHIVO_ADJUNTA"),
                                                                                              radicado,
                                                                                              id_imagen,
                                                                                              id_tipo_envio_respuesta,
                                                                                              img_url)
                        If Result <> "YES" Then
                            uploadFiles.error_sistema = Result
                        Else
                            uploadFiles.error_sistema = "YES"
                            uploadFiles.url_image_semaforo = img_url
                            uploadFiles.id_image = id_imagen
                            uploadFiles.radicado = radicado
                        End If
                    End If
                    'Guarda formato respuesta libre
                    If tipo_adjunta = 2 Then
                        HttpContext.Current.Session.Item("EXTENSION_ARCHIVO_ADJUNTA") = path & file.FileName
                        Result = Classgestionrespuesta.upload_subir_respuesta_radicado(id_respuesta,
                                                                                       HttpContext.Current.Session.Item("EXTENSION_ARCHIVO_ADJUNTA"),
                                                                                       radicado,
                                                                                       id_imagen,
                                                                                       id_tipo_envio_respuesta,
                                                                                       img_url)
                        If Result <> "YES" Then
                            uploadFiles.error_sistema = Result
                        Else
                            uploadFiles.error_sistema = "YES"
                            uploadFiles.url_image_semaforo = img_url
                            uploadFiles.id_image = id_imagen
                            uploadFiles.radicado = radicado
                        End If
                    End If

                End If
                'Guarda anexo respuesta  
                If evento_adjunta = "SUBE_ANEXO" Then
                    Dim id_anexo As Integer = 0
                    Dim nombre_aenxo As String = ""
                    HttpContext.Current.Session.Item("EXTENSION_ARCHIVO_ADJUNTA") = path & file.FileName
                    Result = Classgestionrespuesta.upload_subir_anexo_a_la_respuesta(id_respuesta,
                                                                                     HttpContext.Current.Session.Item("EXTENSION_ARCHIVO_ADJUNTA"),
                                                                                     path_temp,
                                                                                     id_anexo,
                                                                                     nombre_aenxo)
                    If Result <> "YES" Then
                        uploadFiles.error_sistema = Result
                    Else
                        uploadFiles.error_sistema = "YES"
                        uploadFiles.id_anexo = id_anexo
                        uploadFiles.nombre_anexo = nombre_aenxo
                    End If
                End If
                If evento_adjunta = "RADICA_WORKFLOW" Then
                    HttpContext.Current.Session.Item("WF_TIPO_ADJUNTA") = "ENLACE_RADICADO"
                    HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA") = path & file.FileName
                    Result = ref_calssAlamacenamiento.UploadSaveFile(id_expediente,
                                                                     id_tipo_documento,
                                                                     nombre_tipo_documento,
                                                                     estado_adjunta_anexo,
                                                                     estado_adjunta_relacionado,
                                                                     numero_documento_relacionado,
                                                                     FechaCarga,
                                                                     stru_datos_image_lista,
                                                                     id_tarea_workflow,
                                                                     contador)
                    If Result <> "YES" Then
                        uploadFiles.error_sistema = Result
                    Else
                        Dim item_ilist As class_list_detalle_version_document
                        item_ilist = New class_list_detalle_version_document
                        item_ilist.DBT = stru_datos_image_lista.DBT
                        item_ilist.ESTADO_FIRMA_DIGITAL = stru_datos_image_lista.estado_firma_digital
                        item_ilist.IconoAsome = stru_datos_image_lista.icono_icono_awe_some
                        item_ilist.TIPO_ARCHIVO = stru_datos_image_lista.tipodocumental
                        uploadFiles.Class_list_detalle_version_document.Add(item_ilist)
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
                        uploadFiles.error_sistema = "YES"
                    End If
                    HttpContext.Current.Session.Item("WF_TIPO_ADJUNTA") = ""
                End If
                resultList.Add(uploadFiles)
                    jFilesJson = JsonConvert.SerializeObject(resultList)
                    context.Response.Write(jFilesJson)
                Else
                    uploadFiles.error_sistema = "Send sin archivo "
                resultList.Add(uploadFiles)
                jFilesJson = JsonConvert.SerializeObject(resultList)
                context.Response.Write(jFilesJson)
            End If
        Catch ex As Exception
            uploadFiles.error_sistema = ex.Message
            resultList.Add(uploadFiles)
            jFilesJson = JsonConvert.SerializeObject(resultList)
            context.Response.Write(jFilesJson)
        End Try
    End Sub

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class
