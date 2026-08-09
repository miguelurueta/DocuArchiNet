Imports System.ComponentModel
Imports System.IO
Imports System.Web.Script.Serialization
Imports System.Web.Services
Imports GestionDocumental_Docuarchi.net.Class_config_general_service
Imports MySql.Data.MySqlClient
Imports Newtonsoft.Json

' Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class WebServiceDocuarchi
    Inherits System.Web.Services.WebService
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function ServiceSolicitaPermisosSessionGabinete(ByVal Parameter As List(Of CDParamenterGabinete)) As Object
        '---------------------------------------------------------------------------
        'Funcion : Servicio que lista los permisos de gabionete
        '       
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'Parameter                              : Opcional
        '---------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------
        'ListControlGeneralDrowLista        : Retorna la lista de gabinetes 
        '                     value: identificación del gabinete
        '                      text: Nombre del gabinete  
        'error_sistema             : Retorna el resultado del consumo del servicio
        '-----------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------
        'Fecha                 : 2024-05-21
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------
        Dim ListCDaGabinete = New List(Of CDaGabinete)()
        Dim ItemCDaGabinete As New CDaGabinete
        Try
            Dim Result As String = ""
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim CDpersmisosGabinete As New CDpersmisosGabinete
            Dim CDParamenterGabinete As New List(Of CDParamenterGabinete)
            CDParamenterGabinete = Parameter
            ItemCDaGabinete.AppError = ClassDaGabinete.SolicitaPermisosSessionGabinete(CDParamenterGabinete(0).NombreGabinete,
                                                                                       HttpContext.Current.Session.Item("DA_gruposusu"),
                                                                                       HttpContext.Current.Session.Item("ID_USUARIO_DOCUARCHI"),
                                                                                       CDpersmisosGabinete)
            ItemCDaGabinete.CDpersmisosGabinete = CDpersmisosGabinete
            ListCDaGabinete.Add(ItemCDaGabinete)
            Return ListCDaGabinete
        Catch ex As Exception
            ItemCDaGabinete.AppError = "Función  ServiceSolicitaPermisosSessionGabinete " & ex.Message
            ListCDaGabinete.Add(ItemCDaGabinete)
            Return ListCDaGabinete
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function ServiceSolicitaListaGabinetesPermitidos(ByVal Parameter As Object) As IEnumerable(Of control_general_drow_lista)
        '---------------------------------------------------------------------------
        'Funcion : Servicio que lista los gabinetes permitidos usuario y grupo
        '       
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'Parameter                              : Opcional
        '---------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------
        'ListControlGeneralDrowLista        : Retorna la lista de gabinetes 
        '                     value: identificación del gabinete
        '                      text: Nombre del gabinete  
        'error_sistema             : Retorna el resultado del consumo del servicio
        '-----------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------
        'Fecha                 : 2024-05-21
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------
        Dim ListControlGeneralDrowLista = New List(Of control_general_drow_lista)()
        Dim ItemControlGeneralDrowLista As New control_general_drow_lista
        Dim listcontrolDrowLista As New List(Of control_drow_lista)
        Try
            Dim Result As String = ""
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim CDGabinetesPermitidos As New List(Of CDGabinetesPermitidos)
            Result = ClassDaGabinete.SolicitaListaGabinetesPermitidos(HttpContext.Current.Session.Item("DA_gruposusu"),
                                                                      HttpContext.Current.Session.Item("ID_USUARIO_DOCUARCHI"),
                                                                      CDGabinetesPermitidos)
            If Result <> "YES" Then
                ItemControlGeneralDrowLista.error_sistema = Result
                ListControlGeneralDrowLista.Add(ItemControlGeneralDrowLista)
                Return ListControlGeneralDrowLista
            Else
                Dim _item As control_drow_lista
                _item = New control_drow_lista
                _item.value = "0"
                _item.text = "Seleccione"
                listcontrolDrowLista.Add(_item)
                For i As Integer = 0 To CDGabinetesPermitidos.Count - 1
                    _item = New control_drow_lista
                    _item.value = CDGabinetesPermitidos(i).IdGabinete
                    _item.text = CDGabinetesPermitidos(i).NombreGabinete
                    listcontrolDrowLista.Add(_item)
                Next
                ItemControlGeneralDrowLista.error_sistema = Result
                ItemControlGeneralDrowLista.item_sistema = listcontrolDrowLista
                ListControlGeneralDrowLista.Add(ItemControlGeneralDrowLista)
                Return ListControlGeneralDrowLista
            End If
        Catch ex As Exception
            ItemControlGeneralDrowLista.error_sistema = "Función ServiceSolicitaListaGabinetesPermitidos " & ex.Message
            ItemControlGeneralDrowLista.item_sistema = listcontrolDrowLista
            ListControlGeneralDrowLista.Add(ItemControlGeneralDrowLista)
            Return ListControlGeneralDrowLista
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function ServiceSolicitaDocumentoConsultaRue(ByVal IdImagen As Object,
                                                        ByVal Gabinete As Object,
                                                        ByVal Matricula As Object) As Object
        '--------------------------------------------------------------------------------------
        'Funcion :Sevicio que Solicita el tipo de archivo a visualuizar y retorna
        'la url de visualización
        '         
        '--------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '--------------------------------------------------------------------------------------
        'id_imagen                    : Representa la identiifcación de la imagen
        'gabinete                     : Representa el nombre del gabinete                           
        'matricula                    : Representa la matricula del matricualdo
        '                               
        '                             : 
        '                             : 
        '-------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------
        'class_stru_visor_migracion : Retorna la estructura con los datos de visualizacion
        '-----------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------
        'Fecha                 : 2025-05-08
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------
        Dim resultList = New List(Of class_stru_visor_migracion)()
        Dim item_ilist As class_stru_visor_migracion = New class_stru_visor_migracion
        Try
            Dim ClassRues As New ClassRues
            item_ilist.Error_result = ClassRues.SolicitaDocumentoConsultaRue(IdImagen,
                                                                             Gabinete,
                                                                             Matricula,
                                                                             item_ilist)
            If item_ilist.Error_result <> "YES" Then
                resultList.Add(item_ilist)
                Return resultList
            Else
                resultList.Add(item_ilist)
                Return resultList
            End If
        Catch ex As Exception
            item_ilist.Error_result = ex.Message
            resultList.Add(item_ilist)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_solicita_estructura_configuracion_gabinete(ByVal gabinete As Object) As Object
        '---------------------------------------------------------------------------
        'Funcion : Servicio que expone el archivo a visualizar
        '
        '         
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'parameter                    : Retorna los parametros del documento
        '                               
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'class_stru_visor_migracion : Retorna la estructura con los datos de visualizacion
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-09-09
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Dim resultList = New List(Of Class_system1_detalle_gabinete)()
        Dim item_ilist As Class_system1_detalle_gabinete = New Class_system1_detalle_gabinete
        Try
            Dim Result As String = ""
            Dim Class_system1 As New Class_system1
            item_ilist.Error_result = Class_system1.Solicita_estructura_configuracion_gabinete(gabinete,
                                                                                               item_ilist)
            resultList.Add(item_ilist)
            Return resultList
        Catch ex As Exception
            item_ilist.Error_result = ex.Message
            resultList.Add(item_ilist)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_Lista_documento_matriculado(ByVal parameter As Object) As Object
        '---------------------------------------------------------------------------
        'Funcion : Servicio que expone el archivo a visualizar
        '
        '         
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'parameter                    : Retorna los parametros del documento
        '                               
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'class_stru_visor_migracion : Retorna la estructura con los datos de visualizacion
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-09-09
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Dim resultList = New List(Of class_stru_visor_migracion)()
        Dim item_ilist As class_stru_visor_migracion = New class_stru_visor_migracion
        Try
            Dim Result As String = ""
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim class_parameter_visualiza_documento As List(Of class_parameter_visualiza_documento) = Nothing
            If Not parameter Is Nothing Then
                class_parameter_visualiza_documento = JsonConvert.DeserializeObject(Of List(Of class_parameter_visualiza_documento))(parameter)
                If class_parameter_visualiza_documento Is Nothing Then
                    item_ilist.Error_result = "Imposible deserealizar los parametros de configuracion"
                    resultList.Add(item_ilist)
                    Return resultList
                End If
            End If
            Dim Class_ra_con_registros_publicos As New Class_ra_con_registros_publicos
            item_ilist.Error_result = Class_ra_con_registros_publicos.Lista_documento_consulta_publica_expediente(class_parameter_visualiza_documento.Item(0).id_imagen,
                                                                                                                  class_parameter_visualiza_documento.Item(0).id_registro_publico,
                                                                                                                  class_parameter_visualiza_documento.Item(0).id_usuario_registro_publico,
                                                                                                                  class_parameter_visualiza_documento.Item(0).matricula,
                                                                                                                  item_ilist)
            If item_ilist.Error_result <> "YES" Then
                resultList.Add(item_ilist)
                Return resultList
            Else
                resultList.Add(item_ilist)
                Return resultList
            End If
        Catch ex As Exception
            item_ilist.Error_result = ex.Message
            resultList.Add(item_ilist)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_Lista_documento_consulta_publica_expediente(ByVal parameter As Object) As Object
        '---------------------------------------------------------------------------
        'Funcion : Servicio que expone el archivo a visualizar y retorna la url
        '
        '         
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'parameter                    : Retorna los parametros del documento
        '                               
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'class_stru_visor_migracion : Retorna la estructura con los datos de visualizacion
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-09-09
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Dim resultList = New List(Of class_stru_visor_migracion)()
        Dim item_ilist As class_stru_visor_migracion = New class_stru_visor_migracion
        Try
            Dim Result As String = ""
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim class_parameter_visualiza_documento As List(Of class_parameter_visualiza_documento) = Nothing
            If Not parameter Is Nothing Then
                class_parameter_visualiza_documento = JsonConvert.DeserializeObject(Of List(Of class_parameter_visualiza_documento))(parameter)
                If class_parameter_visualiza_documento Is Nothing Then
                    item_ilist.Error_result = "Imposible deserealizar los parametros de configuracion"
                    resultList.Add(item_ilist)
                    Return resultList
                End If
            End If
            Dim Class_ra_con_registros_publicos As New Class_ra_con_registros_publicos
            item_ilist.Error_result = Class_ra_con_registros_publicos.Lista_documento_consulta_publica_expediente(class_parameter_visualiza_documento.Item(0).id_imagen,
                                                                                                                  class_parameter_visualiza_documento.Item(0).id_registro_publico,
                                                                                                                  class_parameter_visualiza_documento.Item(0).id_usuario_registro_publico,
                                                                                                                  class_parameter_visualiza_documento.Item(0).matricula,
                                                                                                                  item_ilist)
            If item_ilist.Error_result <> "YES" Then
                resultList.Add(item_ilist)
                Return resultList
            Else
                resultList.Add(item_ilist)
                Return resultList
            End If
        Catch ex As Exception
            item_ilist.Error_result = ex.Message
            resultList.Add(item_ilist)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_solicita_url_documento_soporte_documental_rad_simple(ByVal parameter As Object) As Object
        '------------------------------------------------------------------------------------
        'Funcion : Servicio que expone el archivo a visualizar y retorna la url
        '          para el modulo de radicación simple
        '
        '         
        '------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '------------------------------------------------------------------------------------
        'parameter                    : Representa los parametros del documento
        '                               
        '
        '------------------------------------------------------------------------------------
        '                           RETORNO
        '------------------------------------------------------------------------------------
        'class_stru_visor_migracion : Retorna la estructura con los datos de visualizacion
        '------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '------------------------------------------------------------------------------------
        'Fecha                 : 2024-11-01
        'Elabora               : Miguel Angel Urueta Miranda
        '-------------------------------------------------------------------------------------
        Dim resultList = New List(Of class_stru_visor_migracion)()
        Dim item_ilist As class_stru_visor_migracion = New class_stru_visor_migracion
        Try
            Dim Result As String = ""
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim class_parameter_visualiza_documento As List(Of class_image_gabinete_visor_rad_simple) = Nothing
            If Not parameter Is Nothing Then
                class_parameter_visualiza_documento = JsonConvert.DeserializeObject(Of List(Of class_image_gabinete_visor_rad_simple))(parameter)
                If class_parameter_visualiza_documento Is Nothing Then
                    item_ilist.Error_result = "Imposible deserealizar los parametros de configuracion"
                    resultList.Add(item_ilist)
                    Return resultList
                End If
            End If
            Dim url As String = ""
            item_ilist.Error_result = ClassDaGabinete.Solicita_url_documento_soporte_documental_rad_simple(class_parameter_visualiza_documento(0).id_imagen,
                                                                                                           class_parameter_visualiza_documento(0).gabinete,
                                                                                                           class_parameter_visualiza_documento(0).id_tarea_workflow,
                                                                                                           class_parameter_visualiza_documento(0).radicado,
                                                                                                           item_ilist.url_iframe,
                                                                                                           item_ilist.name_file)
            resultList.Add(item_ilist)
            Return resultList
        Catch ex As Exception
            item_ilist.Error_result = ex.Message
            resultList.Add(item_ilist)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_Lista_documentos_visor_a_migrar(ByVal id_imagen As Object, ByVal gabinete As Object)
        '---------------------------------------------------------------------------
        'Funcion : Servicio que Solicita el tipo de archivo a visualizar y retorna
        'la url de visualización
        '         
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_imagen                    : Representa la identiifcación de la imagen
        '                               
        'gabinete                     : Representa el nombre del gabinete
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'class_stru_visor_migracion : Retorna la estructura con los datos de visualizacion
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-06-19
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Dim resultList = New List(Of class_stru_visor_migracion)()
        Dim item_ilist As class_stru_visor_migracion = New class_stru_visor_migracion
        Try
            Dim Result As String = ""
            Dim ClassDaGabinete As New ClassDaGabinete
            item_ilist.Error_result = ClassDaGabinete.Lista_documentos_visor_a_migrar(id_imagen,
                                                                                      gabinete,
                                                                                      item_ilist)

            If item_ilist.Error_result <> "YES" Then
                resultList.Add(item_ilist)
                Return resultList
            Else
                resultList.Add(item_ilist)
                Return resultList
            End If
        Catch ex As Exception
            item_ilist.Error_result = ex.Message
            resultList.Add(item_ilist)
            Return resultList

        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function ServiceSolicitaUrlVisorConsulta(ByVal Parameter As List(Of CDParamenterGabinete))
        '---------------------------------------------------------------------------
        'Funcion : Servicio que Solicita el tipo de archivo a visualizar y retorna
        'la url de visualización
        '         
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_imagen                    : Representa la identiifcación de la imagen
        '                               
        'gabinete                     : Representa el nombre del gabinete
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'class_stru_visor_migracion : Retorna la estructura con los datos de visualizacion
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-06-19
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Dim resultList = New List(Of class_stru_visor_migracion)()
        Dim item_ilist As class_stru_visor_migracion = New class_stru_visor_migracion
        Try
            Dim Result As String = ""
            Dim ClassDaGabinete As New ClassDaGabinete
            item_ilist.Error_result = ClassDaGabinete.SolicitaUrlVisorConsulta(Parameter(0).IdImagen,
                                                                               Parameter(0).NombreGabinete,
                                                                               item_ilist)

            resultList.Add(item_ilist)
            Return resultList
        Catch ex As Exception
            item_ilist.Error_result = ex.Message
            resultList.Add(item_ilist)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function ServiceAutoCompleteConsultaGabinete(ByVal parameter As AutoCompleteRequest)
        '---------------------------------------------------------------------------
        'Funcion : Servicio que solicita la estructura con los registro de auto AutoCompleteRequest
        '          de auto complete de una gabinete para la cosulta de migración
        '         
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'parameter            : Representa la estructura contenedora de los parametros
        '                       nombre de tabla name_table_auto y el dbms de conuslta
        '                       name_dbs_auto.
        'value                : Representa el valor de consulta sobre la tabla
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'class_stru_auto_complete_migracion  : Retorna la estructura con los registtros
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-06-15
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Dim resultList = New List(Of class_stru_auto_complete_migracion)()
        Dim item_ilist As class_stru_auto_complete_migracion = New class_stru_auto_complete_migracion
        Try
            Dim ClassDaGabinete As New ClassDaGabinete
            item_ilist.Error_result = ClassDaGabinete.SolicitaDatosAutoCompleteConsultaGabinete(parameter,
                                                                                                item_ilist.country)
            resultList.Add(item_ilist)
            Return resultList
        Catch ex As Exception
            item_ilist.Error_result = ex.Message
            resultList.Add(item_ilist)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function ServiceSolicitaAutoCompleteCampoGabinete(ByVal parameter As AutoCompleteRequest)
        '---------------------------------------------------------------------------
        'Funcion : Servicio que solicita la estructura con los registro de auto 
        '          complete para un campo de gabinete
        '         
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'parameter            : Representa la estructura contenedora de los parametros
        '                       nombre de tabla name_table_auto y el dbms de conuslta
        '                       name_dbs_auto.
        'value                : Representa el valor de consulta sobre la tabla
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'class_stru_auto_complete_migracion  : Retorna la estructura con los registtros
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-06-15
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Dim resultList = New List(Of class_stru_auto_complete_migracion)()
        Dim item_ilist As class_stru_auto_complete_migracion = New class_stru_auto_complete_migracion
        Try
            Dim ClassDaGabinete As New ClassDaGabinete
            item_ilist.Error_result = ClassDaGabinete.SolicitaAutoCompleteCampoGabinete(parameter,
                                                                                        item_ilist.country)
            resultList.Add(item_ilist)
            Return resultList
        Catch ex As Exception
            item_ilist.Error_result = ex.Message
            resultList.Add(item_ilist)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_auto_complete_gabinete_migracion(ByVal parameter As Object, ByVal value As Object)
        '---------------------------------------------------------------------------
        'Funcion : Servicio que solicita la estructura con los registro de auto 
        '          de auto complete de una gabinete para la cosulta de migración
        '         
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'parameter            : Representa la estructura contenedora de los parametros
        '                       nombre de tabla name_table_auto y el dbms de conuslta
        '                       name_dbs_auto.
        'value                : Representa el valor de consulta sobre la tabla
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'class_stru_auto_complete_migracion  : Retorna la estructura con los registtros
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-06-15
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Dim resultList = New List(Of class_stru_auto_complete_migracion)()
        Dim item_ilist As class_stru_auto_complete_migracion = New class_stru_auto_complete_migracion
        Try
            Dim deserialize_parameter = Nothing
            deserialize_parameter = JsonConvert.DeserializeObject(Of List(Of Class_config_general_service_auto_complete))(parameter)
            Dim Result As String = ""
            Dim name_dbs_auto As String = deserialize_parameter(0).name_dbs_auto
            Dim name_table_auto As String = deserialize_parameter(0).name_table_auto
            Dim name_campo_auto As String = deserialize_parameter(0).name_campo_auto
            Dim value_auto As String = value
            Dim ClassDaGabinete As New ClassDaGabinete
            item_ilist.Error_result = ClassDaGabinete.Solicita_datos_auto_complete_gabinete_migracion(name_dbs_auto,
                                                                                                      name_table_auto,
                                                                                                      value_auto,
                                                                                                      item_ilist.country)
            If item_ilist.Error_result <> "YES" Then
                resultList.Add(item_ilist)
                Return resultList
            Else
                resultList.Add(item_ilist)
                Return resultList
            End If
        Catch ex As Exception
            item_ilist.Error_result = ex.Message
            resultList.Add(item_ilist)
            Return resultList

        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_estructura_campos_dynamic_migracion(ByVal id_gabinete As Object)
        '---------------------------------------------------------------------------
        'Funcion : Solicita la estructura de campos de gabinetes de migracion para la
        '         tabla dinamica boot
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_gabinete            : Representa la estructura del indice extraidos
        '                        de la interface
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'class_campos_table_bostra_table  : Retorna la estructura de campos de tabla
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-06-04
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Dim resultList = New List(Of class_stru_Row_Gabinete_Generic)
        Dim item_ilist As class_stru_Row_Gabinete_Generic = New class_stru_Row_Gabinete_Generic
        Try
            Dim Class_DETALLE_GABIENETE As New Class_DETALLE_GABIENETE
            item_ilist.Error_result = Class_DETALLE_GABIENETE.Solicita_estructura_campos_dynamic_migracion(id_gabinete,
                                                                                                           item_ilist.Obj_ilist_fileds_generic)
            If item_ilist.Error_result <> "YES" Then
                resultList.Add(item_ilist)
                Return resultList
            End If
            resultList.Add(item_ilist)
            Return resultList
        Catch ex As Exception
            item_ilist.Error_result = ex.Message
            resultList.Add(item_ilist)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function ServiceConsultaGabinete(ByVal Parameter As List(Of CDParamenterGabinete))
        '-----------------------------------------------------------------------------------
        'Funcion : Servicio web Solicita la consulta sobre gabinetes de migración
        '         
        '-----------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------
        'parameter             : Representa la estructura del indice extraidos
        '                        de la interface
        'tipo_consulta         : Tipo de consulta de gabinete migracion 1 - consulta
        '                        campos  2- Tipo de consulta general todos los campos
        'valor_consulta        : Valor de consulta para tipo de consulta 2
        'id_gabinete           : Representa identificacion del gabiete
        '-----------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------
        'class_stru_date_Gabinete_Generic : Retorna la estructura de datos de la consulta
        '-----------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------
        'Fecha                 : 2024-06-15
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------
        Dim ListClassRowGabinete = New List(Of class_stru_Row_Gabinete_Generic)
        Dim IListClassRowGabinete As class_stru_Row_Gabinete_Generic = New class_stru_Row_Gabinete_Generic
        Try
            Dim ClassDaGabinete As New ClassDaGabinete
            IListClassRowGabinete.Error_result = ClassDaGabinete.ConsultaGabinete(Parameter.Item(0).TipoConsulta,
                                                                                  Parameter.Item(0).ValorConsulta,
                                                                                  Parameter.Item(0).IdGabinete,
                                                                                  Parameter.Item(0).ClassConfigGeneralService,
                                                                                  IListClassRowGabinete)
            ListClassRowGabinete.Add(IListClassRowGabinete)
            Return ListClassRowGabinete
        Catch ex As Exception
            IListClassRowGabinete.Error_result = ex.Message
            ListClassRowGabinete.Add(IListClassRowGabinete)
            Return ListClassRowGabinete
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_consulta_gabinete_migracion(ByVal parameter As Object,
                                                        ByVal tipo_consulta As Object,
                                                        ByVal valor_consulta As String,
                                                        ByVal id_gabinete As Object)
        '-----------------------------------------------------------------------------------
        'Funcion : Servicio web Solicita la consulta sobre gabinetes de migración
        '         
        '-----------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------
        'parameter             : Representa la estructura del indice extraidos
        '                        de la interface
        'tipo_consulta         : Tipo de consulta de gabinete migracion 1 - consulta
        '                        campos  2- Tipo de consulta general todos los campos
        'valor_consulta        : Valor de consulta para tipo de consulta 2
        'id_gabinete           : Representa identificacion del gabiete
        '-----------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------
        'class_stru_date_Gabinete_Generic : Retorna la estructura de datos de la consulta
        '-----------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------
        'Fecha                 : 2024-06-15
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------
        Dim resultList = New List(Of class_stru_Row_Gabinete_Generic)
        Dim iList_class_stru_Row_Gabinete_Generic As class_stru_Row_Gabinete_Generic = New class_stru_Row_Gabinete_Generic
        Try
            Dim serializer = New JavaScriptSerializer()
            Dim Class_config_general_service_ = Nothing
            Dim Result As String = ""
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim Class_system1 As New Class_system1
            Dim Class_DETALLE_GABIENETE As New Class_DETALLE_GABIENETE
            Class_config_general_service_ = JsonConvert.DeserializeObject(Of List(Of Class_config_general_service))(parameter)
            If Class_config_general_service_ Is Nothing Then
                iList_class_stru_Row_Gabinete_Generic.Error_result = "Imposible deserealizar los parametros de configuracion"
                resultList.Add(iList_class_stru_Row_Gabinete_Generic)
                Return resultList
            End If
            iList_class_stru_Row_Gabinete_Generic.Error_result = ClassDaGabinete.Consulta_gabinete_migracion(tipo_consulta,
                                                                                                              valor_consulta,
                                                                                                              id_gabinete,
                                                                                                              Class_config_general_service_,
                                                                                                              iList_class_stru_Row_Gabinete_Generic)
            resultList.Add(iList_class_stru_Row_Gabinete_Generic)
            Return resultList
        Catch ex As Exception
            iList_class_stru_Row_Gabinete_Generic.Error_result = ex.Message
            resultList.Add(iList_class_stru_Row_Gabinete_Generic)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function ServiceListaInterfaceBusquedaGabinete(ByVal Parameter As List(Of CDParamenterGabinete))
        '---------------------------------------------------------------------------
        'Funcion : Servicio que lista la estructura de los campos para la interfaz
        '        : dinamica de documentos  
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_gabinete                : Indentificación del gabinete
        'aplica_campo_date          : Valida si muestra en campo date en la consulta 
        'aplica_campo_id            : Valida si muestra en campo identificación de la
        '                           : imagen en la consulta
        '---------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------
        'Class_config_general_service : Retorna la estructura general del gabinete en  
        '                             : la estructura generica de contorles
        '                             
        'error_gestion             : Retorna el resultado del consumo del servicio
        '-----------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------
        'Fecha                 : 2024-05-21
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------
        Dim resultList = New List(Of Class_config_general_service)()
        Try

            Dim Result As String = ""
            Dim ClassDaGabinete As New ClassDaGabinete
            Result = ClassDaGabinete.SolicitaEstructurainterfaceBusquedaGabinete(Parameter(0).IdGabinete,
                                                                                 1,
                                                                                 1,
                                                                                 resultList)

            resultList(0).error_gestion = Result
            Return resultList
        Catch ex As Exception
            resultList(0).error_gestion = ex.Message
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_lista_interface_busqueda_gabinete(ByVal id_gabinete As Object,
                                                              ByVal aplica_campo_date As Object,
                                                              ByVal aplica_campo_id As Object) As IEnumerable(Of Class_config_general_service)
        '---------------------------------------------------------------------------
        'Funcion : Servicio que lista la estructura de los campos para la interfaz
        '        : dinamica de documentos  
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_gabinete                : Indentificación del gabinete
        'aplica_campo_date          : Valida si muestra en campo date en la consulta 
        'aplica_campo_id            : Valida si muestra en campo identificación de la
        '                           : imagen en la consulta
        '---------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------
        'Class_config_general_service : Retorna la estructura general del gabinete en  
        '                             : la estructura generica de contorles
        '                             
        'error_gestion             : Retorna el resultado del consumo del servicio
        '-----------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------
        'Fecha                 : 2024-05-21
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------
        Dim resultList = New List(Of Class_config_general_service)()
        Try

            Dim Result As String = ""
            Dim ClassDaGabinete As New ClassDaGabinete
            Result = ClassDaGabinete.SolicitaEstructurainterfaceBusquedaGabinete(id_gabinete,
                                                                                 aplica_campo_date,
                                                                                 CInt(aplica_campo_id),
                                                                                 resultList)

            resultList(0).error_gestion = Result
            Return resultList
        Catch ex As Exception
            resultList(0).error_gestion = ex.Message
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_solicita_gabinetes_migracion(ByVal id As Object) As IEnumerable(Of control_general_drow_lista)
        '---------------------------------------------------------------------------
        'Funcion : Servicio que lista los gabinetes permitidos para migración
        '       
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id                               : Opcional
        '---------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------
        'control_drow_lista        : Retorna la lista de gabinetes 
        '                     value: identificación del gabinete
        '                      text: Nombre del gabinete  
        'error_sistema             : Retorna el resultado del consumo del servicio
        '-----------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------
        'Fecha                 : 2024-05-21
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------
        Dim resul_service = New List(Of control_general_drow_lista)()
        Dim item As New control_general_drow_lista
        Dim lista_item_drow As New List(Of control_drow_lista)
        Try
            Dim Result As String = ""
            Dim Class_system1 As New Class_system1
            Result = Class_system1.Solicita_gabinetes_migracion(lista_item_drow)
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
            item.error_sistema = "Función Service_solicita_gabinetes_migracion " & ex.Message
            item.item_sistema = lista_item_drow
            resul_service.Add(item)
            Return resul_service
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_elimina_documento_relacionado_consulta_radicado(ByVal parameter As Object) As IEnumerable(Of class_image_gabinete)
        '---------------------------------------------------------------------------
        'Funcion : Servicio que elimina documentos relacionados en la ventana
        '          de consulta radicado
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'parameter                        : Representa el id de la imagen
        'RA_ID_REGISTRO_RADICADO          : Representa la identificación del registro
        '                                   del radicado seleccionado
        'MASTER_ELIMINA_GABINETE_WORKFLOW : Indentifica si el usuario puede eliminar
        '                                   documentos almacenados por otros usuarios
        '---------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------
        'id_imagen                 : Retorna la identificacion de la imagen eliminada
        'limpia_visor              : Retorna si el usuario esta visualizando la imagen
        '                            para determinar si inicializa el visor 1-0
        'error_gestion             : Retorna el resultado del consumo del servicio
        '-----------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------
        'Fecha                 : 2023-06-28
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------
        Dim resultList = New List(Of class_image_gabinete)()
        Dim parameter_gestion As class_image_gabinete = New class_image_gabinete()
        Try
            Dim Result As String = ""

            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim nombre_gabinete As String = ""
            Result = Class_DAT_ADIC_TAR.SolicitaNombreGabineteImagenTareaWorkflow(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                 nombre_gabinete,
                                                                 0)
            If Result <> "YES" Then
                parameter_gestion.error_gestion = Result
                resultList.Add(parameter_gestion)
                Return resultList
            End If
            Dim ClassDaGabinete As New ClassDaGabinete
            Result = ClassDaGabinete.Eliminar_documento_relacionado_consulta_radicado(nombre_gabinete,
                                                                                                      parameter,
                                                                                                      parameter,
                                                                                                      1,
                                                                                                      Session.Item("MASTER_ELIMINA_GABINETE_WORKFLOW"),
                                                                                                      HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"))
            If Result = "YES" Then
                If HttpContext.Current.Session.Item("DA_IMAGEN") = parameter Then
                    parameter_gestion.limpia_visor = 1
                Else
                    parameter_gestion.limpia_visor = 0
                End If
            End If
            parameter_gestion.error_gestion = Result
            parameter_gestion.id_imagen = parameter
            resultList.Add(parameter_gestion)
            Return resultList
        Catch ex As Exception
            parameter_gestion.error_gestion = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_elimina_documento_enlace_radicado_workflow(ByVal parameter As Object) As IEnumerable(Of class_image_gabinete)
        '---------------------------------------------------------------------------
        'Funcion : Servicio que elimina documentos relacionados en la ventana
        '          de enlace del modulo radicado
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'parameter                        : Representa el id de la imagen
        'RA_ID_REGISTRO_RADICADO          : Representa la identificación del registro
        '                                   del radicado seleccionado
        'MASTER_ELIMINA_GABINETE_WORKFLOW : Indentifica si el usuario puede eliminar
        '                                   documentos almacenados por otros usuarios
        '---------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------
        'id_imagen                 : Retorna la identificacion de la imagen eliminada
        'limpia_visor              : Retorna si el usuario esta visualizando la imagen
        '                            para determinar si inicializa el visor 1-0
        'error_gestion             : Retorna el resultado del consumo del servicio
        '-----------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------
        'Fecha                 : 2023-06-27
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------
        Dim resultList = New List(Of class_image_gabinete)()
        Dim parameter_gestion As class_image_gabinete = New class_image_gabinete()
        Try
            Dim Result As String = ""
            Dim Class_ra_rad_estados_modulo_radicacion As New Class_ra_rad_estados_modulo_radicacion
            Dim stru_registro As stru_registro_estado = Nothing
            Result = Class_ra_rad_estados_modulo_radicacion.SolicitaDatosEstructuraEstadoRadicado(HttpContext.Current.Session.Item("RA_ID_REGISTRO_RADICADO"),
                                                                                                      stru_registro)
            If Result <> "YES" Then
                parameter_gestion.error_gestion = Result
                resultList.Add(parameter_gestion)
                Return resultList
            End If
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim nombre_gabinete As String = ""
            Result = Class_DAT_ADIC_TAR.SolicitaNombreGabineteImagenTareaWorkflow(stru_registro.id_tarea_workflow,
                                                                 nombre_gabinete,
                                                                 0)
            If Result <> "YES" Then
                parameter_gestion.error_gestion = Result
                resultList.Add(parameter_gestion)
                Return resultList
            End If

            Dim ClassDaGabinete As New ClassDaGabinete
            Result = ClassDaGabinete.Eliminar_documento_relacionado_enlace_radicado(nombre_gabinete,
                                                                                    parameter,
                                                                                    parameter,
                                                                                    1,
                                                                                    Session.Item("MASTER_ELIMINA_GABINETE_WORKFLOW"),
                                                                                    stru_registro.id_tarea_workflow,
                                                                                    stru_registro.consecutivo_radicado)
            If Result = "YES" Then
                If HttpContext.Current.Session.Item("DA_IMAGEN") = parameter Then
                    parameter_gestion.limpia_visor = 1
                Else
                    parameter_gestion.limpia_visor = 0
                End If
            End If
            parameter_gestion.error_gestion = Result
            parameter_gestion.id_imagen = parameter
            resultList.Add(parameter_gestion)
            Return resultList
        Catch ex As Exception
            parameter_gestion.error_gestion = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_elimina_documento_enlace_workflow(ByVal parameter As Object) As IEnumerable(Of class_image_gabinete)
        '---------------------------------------------------------------------------
        'Funcion : Servicio que elimina documentos relacionados en la ventana
        '          de enlace del modulo workflow
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'parameter                        : Representa el id de la imagen
        'ID_TAREA_SELECCIONDA_ENLACE      : Representa la identificación de la tarea
        '                                  seleccionada en la variable sesión
        'MASTER_ELIMINA_GABINETE_WORKFLOW : Indentifica si el usuario puede eliminar
        '                                   documentos almacenados por otros usuarios
        '---------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------
        'id_imagen                 : Retorna la identificacion de la imagen eliminada
        'limpia_visor              : Retorna si el usuario esta visualizando la imagen
        '                            para determinar si inicializa el visor 1-0
        'error_gestion             : Retorna el resultado del consumo del servicio
        '-----------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------
        'Fecha                 : 2023-06-26
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------
        Dim resultList = New List(Of class_image_gabinete)()
        Dim parameter_gestion As class_image_gabinete = New class_image_gabinete()
        Try
            Dim Result As String = ""
            Dim id_tarea As Long = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA_ENLACE")
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim nombre_gabinete As String = ""
            Result = Class_DAT_ADIC_TAR.SolicitaNombreGabineteImagenTareaWorkflow(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA_ENLACE"),
                                                                 nombre_gabinete,
                                                                 0)
            If Result <> "YES" Then
                parameter_gestion.error_gestion = Result
                resultList.Add(parameter_gestion)
                Return resultList
            End If
            Dim ClassDaGabinete As New ClassDaGabinete
            Result = ClassDaGabinete.Eliminar_documento_relcionado_workflow(nombre_gabinete,
                                                                            parameter,
                                                                            parameter,
                                                                            1,
                                                                            Session.Item("MASTER_ELIMINA_GABINETE_WORKFLOW"),
                                                                            id_tarea)
            If Result = "YES" Then
                If HttpContext.Current.Session.Item("DA_IMAGEN") = parameter Then
                    parameter_gestion.limpia_visor = 1
                Else
                    parameter_gestion.limpia_visor = 0
                End If
            End If
            parameter_gestion.error_gestion = Result
            parameter_gestion.id_imagen = parameter
            resultList.Add(parameter_gestion)
            Return resultList
        Catch ex As Exception
            parameter_gestion.error_gestion = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_elimina_documento_relacionado_workflow(ByVal parameter As Object) As IEnumerable(Of class_image_gabinete)
        '---------------------------------------------------------------------------
        'Funcion : Servicio que elimina documentos relacionados en la ventana
        '          de selecion de modulo workflow
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'parameter                        : Representa el id de la imagen
        'ID_TAREA_SELECCIONDA             : Representa la identificación de la tarea
        '                                  seleccionada en la variable sesión
        'MASTER_ELIMINA_GABINETE_WORKFLOW : Indentifica si el usuario puede eliminar
        '                                   documentos almacenados por otros usuarios
        '---------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------
        'id_imagen                 : Retorna la identificacion de la imagen eliminada
        'limpia_visor              : Retorna si el usuario esta visualizando la imagen
        '                            para determinar si inicializa el visor 1-0
        'error_gestion             : Retorna el resultado del consumo del servicio
        '-----------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------
        'Fecha                 : 2023-06-26
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------
        Dim resultList = New List(Of class_image_gabinete)()
        Dim parameter_gestion As class_image_gabinete = New class_image_gabinete()
        Try
            Dim Result As String = ""
            Dim id_tarea As Long = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim nombre_gabinete As String = ""
            Result = Class_DAT_ADIC_TAR.SolicitaNombreGabineteImagenTareaWorkflow(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                 nombre_gabinete,
                                                                 0)
            If Result <> "YES" Then
                parameter_gestion.error_gestion = Result
                resultList.Add(parameter_gestion)
                Return resultList
            End If
            Dim ClassDaGabinete As New ClassDaGabinete
            'Thread.Sleep(1000)
            Result = ClassDaGabinete.Eliminar_documento_relcionado_workflow(nombre_gabinete,
                                                                            parameter,
                                                                            parameter,
                                                                            1,
                                                                            Session.Item("MASTER_ELIMINA_GABINETE_WORKFLOW"),
                                                                            id_tarea)
            If Result = "YES" Then
                If HttpContext.Current.Session.Item("WF_ID_DOCUMENTO_SELECCIONADO") = parameter Then
                    HttpContext.Current.Session.Item("WF_ID_DOCUMENTO_SELECCIONADO") = 0
                    HttpContext.Current.Session.Item("WF_GABINETE_SELECCIONADO") = ""
                    HttpContext.Current.Session.Item("WF_ID_GABINETE_SELECCIONADO") = ""
                    parameter_gestion.limpia_visor = 1
                Else
                    parameter_gestion.limpia_visor = 0
                End If
            End If
            parameter_gestion.error_gestion = Result
            parameter_gestion.id_imagen = parameter
            resultList.Add(parameter_gestion)
            Return resultList
        Catch ex As Exception
            parameter_gestion.error_gestion = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_crea_interface_indice_workflow(ByVal parameter As Object) As IEnumerable(Of Class_config_general_service)
        Dim resultList = New List(Of Class_config_general_service)()
        Dim parameter_gestion As Class_config_general_service = New Class_config_general_service()
        Try
            If HttpContext.Current.Session.Item("WF_ACTUALIZA_INDICE_BATCH_WF") = 0 Then
                parameter_gestion.error_gestion = "Usuario sin permiso para actualizar multiplex indices"
                resultList.Add(parameter_gestion)
                Return resultList
            End If
            Dim Result As String = ""
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim nombre_gabinete As String = ""
            Result = Class_DAT_ADIC_TAR.SolicitaNombreGabineteImagenTareaWorkflow(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                 nombre_gabinete,
                                                                 0)
            If Result <> "YES" Then
                parameter_gestion.error_gestion = Result
                resultList.Add(parameter_gestion)
                Return resultList
            End If
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim id_documento As Integer = parameter
            Dim estructura_gabinete() As estructura_gabinete = Nothing
            Dim NameEspaceControl As String = "form_control_indice_docuarchi"
            Result = ClassDaGabinete.SolicitaEstructuraValoresCamposIndice(id_documento,
                                                                           nombre_gabinete,
                                                                           1,
                                                                           NameEspaceControl,
                                                                           estructura_gabinete,
                                                                           resultList)
            If Result <> "YES" Then
                parameter_gestion.error_gestion = Result
                resultList.Add(parameter_gestion)
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
    Public Function Service_crea_interface_indice_workflow_enlace(ByVal parameter As Object) As IEnumerable(Of Class_config_general_service)
        Dim resultList = New List(Of Class_config_general_service)()
        Dim parameter_gestion As Class_config_general_service = New Class_config_general_service()
        Try
            If HttpContext.Current.Session.Item("WF_ACTUALIZA_INDICE_BATCH_WF") = 0 Then
                parameter_gestion.error_gestion = "Usuario sin permiso para actualizar multiplex indices"
                resultList.Add(parameter_gestion)
                Return resultList
            End If
            Dim Result As String = ""
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim nombre_gabinete As String = ""
            Result = Class_DAT_ADIC_TAR.SolicitaNombreGabineteImagenTareaWorkflow(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA_ENLACE"),
                                                                 nombre_gabinete,
                                                                 0)
            If Result <> "YES" Then
                parameter_gestion.error_gestion = Result
                resultList.Add(parameter_gestion)
                Return resultList
            End If
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim id_documento As Integer = parameter
            Dim estructura_gabinete() As estructura_gabinete = Nothing
            Dim NameEspaceControl As String = "form_control_indice_docuarchi"
            Result = ClassDaGabinete.SolicitaEstructuraValoresCamposIndice(id_documento,
                                                                           nombre_gabinete,
                                                                           1,
                                                                           NameEspaceControl,
                                                                           estructura_gabinete,
                                                                           resultList)
            parameter_gestion.error_gestion = Result
            Return resultList
        Catch ex As Exception
            parameter_gestion.error_gestion = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_crea_interface_indice_migracion(ByVal parameter As Object) As Object
        Dim resultList = New List(Of Class_config_general_service)()
        Dim parameter_gestion As Class_config_general_service = New Class_config_general_service()
        Try

            Dim Result As String = ""
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim estructura_gabinete() As estructura_gabinete = Nothing
            Dim des_parmeter_interface_show = New List(Of class_config_general_parmeter_interface_show)()
            des_parmeter_interface_show = JsonConvert.DeserializeObject(Of List(Of class_config_general_parmeter_interface_show))(parameter)
            Dim NameEspaceControl As String = "form_control_indice_docuarchi"
            Result = ClassDaGabinete.SolicitaEstructuraValoresCamposIndice(des_parmeter_interface_show(0).id_registro,
                                                                           des_parmeter_interface_show(0).name_table,
                                                                           1,
                                                                           NameEspaceControl,
                                                                           estructura_gabinete,
                                                                           resultList)
            If Result <> "YES" Then
                resultList.Clear()
                parameter_gestion.error_gestion = Result
                resultList.Add(parameter_gestion)
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
    Public Function ServiceCreaInterfazindiceGabinete(ByVal Parameter As List(Of CDParamenterGabinete)) As Object
        Dim resultList = New List(Of Class_config_general_service)()
        Dim parameter_gestion As Class_config_general_service = New Class_config_general_service()
        Try
            Dim Result As String = ""
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim EstructuraGabinete() As estructura_gabinete = Nothing
            Result = ClassDaGabinete.SolicitaEstructuraValoresCamposIndice(Parameter(0).IdImagen,
                                                                           Parameter(0).NombreGabinete,
                                                                           1,
                                                                           Parameter(0).NameEspaceControl,
                                                                           EstructuraGabinete,
                                                                           resultList)
            If Result <> "YES" Then
                resultList.Clear()
                parameter_gestion.error_gestion = Result
                resultList.Add(parameter_gestion)
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
    Public Function Service_crea_interface_indice_produccion(ByVal parameter As Object) As IEnumerable(Of Class_config_general_service)
        Dim resultList = New List(Of Class_config_general_service)()
        Dim parameter_gestion As Class_config_general_service = New Class_config_general_service()
        Try

            Dim Result As String = ""
            Dim ClassGaProducionDocumental As New ClassGaProducionDocumental
            Dim stru_produccion_indice As stru_produccion_indice = Nothing
            stru_produccion_indice.ID_DOCUMENTO_DOCUARCHI_ALMACEN = -1
            Result = ClassGaProducionDocumental.Solicita_estructura_id_registro_produccion(parameter,
                                                                                           stru_produccion_indice)
            If Result <> "YES" Then
                parameter_gestion.error_gestion = Result
                resultList.Add(parameter_gestion)
                Return resultList
            End If
            If stru_produccion_indice.ID_DOCUMENTO_DOCUARCHI_ALMACEN = -1 Then
                parameter_gestion.error_gestion = "Imposible encontrar la produción del documento (" & parameter & ")"
                resultList.Add(parameter_gestion)
                Return resultList
            End If
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim id_documento As Integer = stru_produccion_indice.ID_DOCUMENTO_DOCUARCHI_ALMACEN
            Dim estructura_gabinete() As estructura_gabinete = Nothing
            Dim NameEspaceControl As String = "form_control_indice_docuarchi"
            Result = ClassDaGabinete.SolicitaEstructuraValoresCamposIndice(id_documento,
                                                                            stru_produccion_indice.NOMBRE_GABINETE,
                                                                            1,
                                                                            NameEspaceControl,
                                                                            estructura_gabinete,
                                                                            resultList)
            If Result <> "YES" Then
                parameter_gestion.error_gestion = Result
                resultList.Add(parameter_gestion)
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
    Public Function GetLista_consulta_gabinetes(ByVal DName As String)
        Dim response As String = ""
        Dim country As List(Of String) = New List(Of String)()
        Try
            Dim refcconect As New conect.Dbase_Conction_Mysql_RA
            Dim datset As New DataSet
            Dim Sql_condicion As String = ""
            Dim ref_Class_DETALLE_GABIENETE As New Class_DETALLE_GABIENETE
            Dim stru_campo_detalle() As stru_campo_detalle = Nothing
            Dim Result As String = ""
            If HttpContext.Current.Session("TIPOMODULO") = "PUBLICO" Then
                Result = ref_Class_DETALLE_GABIENETE.Solicita_detalle_campos_gabinete_publico(Session.Item("DA_GABINETE_CONSULTA"),
                                                                                              stru_campo_detalle)
            Else
                Result = ref_Class_DETALLE_GABIENETE.SolicitaDetalleCamposGabinete(Session.Item("DA_GABINETE_CONSULTA"),
                                                                                               stru_campo_detalle)
            End If

            If Result <> "YES" Then
                country.Add(Result)
                Return country
            End If
            If stru_campo_detalle Is Nothing Then
                country.Add("Impsible encontrar campos gabinete (" & Session.Item("DA_GABINETE_CONSULTA") & ")")
                Return country
            End If
            Dim seleccampos As String = "Select "
            Dim campo_clase_documento As String = ""
            Dim sqlfrom As String = " From " & Session.Item("DA_GABINETE_CONSULTA")
            For i As Integer = 0 To stru_campo_detalle.Length - 1
                If stru_campo_detalle(i).nombre_campo = "TIPODOCUMENTO" Then
                    campo_clase_documento = "TIPODOCUMENTO"
                    Exit For
                End If
            Next
            For i As Integer = 0 To stru_campo_detalle.Length - 1
                Dim refcampo As String = stru_campo_detalle(i).nombre_campo
                If stru_campo_detalle(i).tipo_campo = "DATE" Then
                    refcampo = "CAST(" & stru_campo_detalle(i).nombre_campo & " AS DATE) AS " & stru_campo_detalle(i).nombre_campo
                End If
                If seleccampos = "Select " Then
                    If campo_clase_documento <> "" Then
                        seleccampos = seleccampos & campo_clase_documento & "," & refcampo
                    Else
                        seleccampos = seleccampos & refcampo
                    End If
                Else
                    If stru_campo_detalle(i).nombre_campo <> campo_clase_documento Then
                        seleccampos = seleccampos & "," & refcampo
                    End If
                End If
            Next
            Dim condicionsql As String = " where "
            For i As Integer = 0 To stru_campo_detalle.Length - 1
                Dim likeigual As String = " Like"
                Dim campo_plantilla As String = stru_campo_detalle(i).nombre_campo
                If condicionsql = " where " Then
                    condicionsql = condicionsql & "(" & stru_campo_detalle(i).nombre_campo & likeigual & "'%" & DName & "%'"
                Else
                    condicionsql = condicionsql & " or " & stru_campo_detalle(i).nombre_campo & likeigual & "'%" & DName & "%'"
                End If
            Next
            If condicionsql = " where " Then
                Return country
                Exit Function
            End If
            Dim sqlconsult = seleccampos & " " & sqlfrom & " " & condicionsql & ") AND DBT <=1   " & " LIMIT 50"
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
            country.Add(ex.Message)
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
    Public Function GetPosiblesDatosGabinete(ByVal prefixText As String, ByVal count As Integer, ByVal contextKey As String) As String()
        Dim result As New List(Of String)()
        Try
            Dim refcconect As New conect.Dbase_Conction_Mysql_DA
            Dim refclas As New ClassRadicador
            Dim datset As New DataSet
            Dim response As String = ""
            Dim split() As String = contextKey.Split("|")
            Dim sqlconsult As String = ""
            If prefixText = "*." Then
                sqlconsult = "Select distinct " & split(0) & " from " & split(1) & "  LIMIT 1000  "
            Else
                sqlconsult = "Select distinct " & split(0) & " from " & split(1) & " where " & split(0) & " like '%" & prefixText & "%' LIMIT 50  "
            End If
            response = SELECTION_SELECT_FIELD(sqlconsult, datset)
            If response <> "YES" Then
                GetPosiblesDatosGabinete = result.ToArray
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
                GetPosiblesDatosGabinete = result.ToArray
            Else
                GetPosiblesDatosGabinete = result.ToArray
            End If
        Catch ex As Exception
            result.Add(ex.Message)
            GetPosiblesDatosGabinete = result.ToArray
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    Public Function ServiceEliminaDocumentoGabinete(ByVal Parameter As List(Of CDParamenterGabinete)) As List(Of CDaGabinete)
        '---------------------------------------------------------------------------
        'Funcion : Elimina documento desde la consulta docuarchi
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'Parameter             : Respresenta los parametros
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2025-09-01
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Dim ListCDaGabinete As New List(Of CDaGabinete)
        Dim IlistCDaGabinete As New CDaGabinete
        Try
            Dim Refclass As New ClassEliminarDocListResult
            Dim Result As String = ""
            If HttpContext.Current.Session.Item("ELIMINAR_REGISTRO") = 0 Then
                IlistCDaGabinete.AppError = "El usuario no tiene permisos para eliminar registros del gabinete (" & Parameter(0).NombreGabinete & ")"
                ListCDaGabinete.Add(IlistCDaGabinete)
                Return ListCDaGabinete
            End If
            IlistCDaGabinete.AppError = Refclass.EliminarDocumentosGabinete(Parameter(0).IdImagen,
                                                                                  0,
                                                                                  Parameter(0).NombreGabinete,
                                                                                  1,
                                                                                  1,
                                                                                  Session.Item("MASTER_ELIMINAR_REGISTRO"),
                                                                                  -1,
                                                                                  Parameter(0).NombreModulo)
            IlistCDaGabinete.CDParamenterGabinete = Parameter(0)
            ListCDaGabinete.Add(IlistCDaGabinete)
            Return ListCDaGabinete
        Catch ex As Exception
            IlistCDaGabinete.AppError = ex.Message
            ListCDaGabinete.Add(IlistCDaGabinete)
            Return ListCDaGabinete
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    Public Function Get_elimina_registro_service(ByVal id_imagen As String) As String
        '---------------------------------------------------------------------------
        'Funcion : Elimina documento desde la consulta docuarchi
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_imagen             : Respresenta el nombre de plantilla del radicado
        'radicado              : Representa el consecutivo de radicado
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'id_usuario_radicador  : Retorna la idnetificación del usuario radicador
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-04-01
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Refclass As New ClassEliminarDocListResult
            Dim Result As String = ""
            If HttpContext.Current.Session.Item("ELIMINAR_REGISTRO") = 0 Then
                Get_elimina_registro_service = "El usuario no tiene permisos para eliminar registros del gabinete (" & Session.Item("DA_GABINETE_CONSULTA") & ")"
                Exit Function
            End If
            Result = Refclass.EliminarDocumentosGabinete(id_imagen,
                                                               0,
                                                               Session.Item("DA_GABINETE_CONSULTA"),
                                                               1,
                                                               1,
                                                               Session.Item("MASTER_ELIMINAR_REGISTRO"),
                                                               -1,
                                                               "DOCUARCHI")
            If Result <> "YES" Then
                Get_elimina_registro_service = Result
                Exit Function
            Else
                Get_elimina_registro_service = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Get_elimina_registro_service = ex.Message
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Get_elimina_registro_producion_service(ByVal id_imagen As String)
        Try
            Dim Refclass As New ClassGaProducionDocumental
            Dim Result As String = ""
            Result = Refclass.Activa_eliminar_documento_producion_documental(Session.Item("GA_IDUSUARIOGESTION"),
                                                                             Val(id_imagen))
            If Result <> "YES" Then
                Return Result
                Exit Function
            Else
                Return "YES"
                Exit Function
            End If
        Catch ex As Exception
            Return "Inconsistencia general función Get_elimina_registro_producion_service " + ex.Message
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    Public Function Get_solicita_documento_seleccionado(ByVal id_imagen As String) As String
        Try
            Dim Ref_clas As New ClassVisualisaDocumento
            Dim Result As String = ""
            If HttpContext.Current.Session.Item("DG_SELECION_TREE") = "" Then
                Return "NO|Debe seleccionar una imagen para editar"
                'Get_solicita_documento_seleccionado = "NO|Debe seleccionar una imagen para editar"
                'Exit Function
            End If
            Dim split_seleccion() As String = HttpContext.Current.Session.Item("DG_SELECION_TREE").ToString.Split("|")
            Dim Matri_documentos() As String = Nothing
            Result = Ref_clas.Genera_Matris_Documentos_Almacenados(split_seleccion(1),
                                                                   split_seleccion(0),
                                                                   Matri_documentos)
            If Result <> "YES" Then
                Return "ERROR|" & Result
                'Get_solicita_documento_seleccionado = "ERROR|" & Result
                'Exit Function
            End If
            Dim fileinf As New FileInfo(Matri_documentos(1))
            If fileinf.Extension <> ".PDF" Then
                Return "ERROR|El tipo de archivo (" & fileinf.Extension & ") no se puede editar en la interface"
                'Get_solicita_documento_seleccionado = "ERROR|El tipo de archivo (" & fileinf.Extension & ") no se puede editar en la interface"
                'Exit Function
            End If
            Dim uri As String = HttpContext.Current.Request.Url.ToString.Replace("/webservice/WebServiceDocuarchi.asmx/Get_solicita_documento_seleccionado", "")
            Dim ruta_copia As String = HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ESCANER") & "\"
            File.Copy(Matri_documentos(1), ruta_copia & "DOC_COPIA_XXXX.PDF", True)
            uri = HttpContext.Current.Request.Url.Scheme & System.Uri.SchemeDelimiter & HttpContext.Current.Request.Url.Host & HttpContext.Current.Request.ApplicationPath & "/workflow/Handler_file_pdf.ashx?rut_image="
            Return "YES|" & ruta_copia & "DOC_COPIA_XXXX.PDF" & "|" & uri
        Catch ex As Exception
            Return "ERROR|" & ex.Message
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    Public Function Set_documento_seleccionado(ByVal id_imagen As String, ByVal nombre_gabinete As String) As String
        Try
            HttpContext.Current.Session.Item("DG_SELECION_TREE") = nombre_gabinete & "|" & id_imagen
            Return "YES"
        Catch ex As Exception
            Return "ERROR|" & ex.Message
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Set_actualiza_indice_docuarchi(ByVal parameter As Object, ByVal tipo_indice_actualiza As Integer)
        Try
            Dim deserialize_parameter = Nothing
            Dim serializer = New JavaScriptSerializer()
            deserialize_parameter = serializer.Deserialize(Of List(Of stru_campos_docuarchi))(parameter)
            If deserialize_parameter Is Nothing Then
                Return "Imposible deserealizar los parametros de configuracion"
                Exit Function
            End If
            Dim stru_campos_docuarchi() As stru_campos_docuarchi = Nothing
            For i As Integer = 0 To deserialize_parameter.count - 1
                ReDim Preserve stru_campos_docuarchi(i)
                stru_campos_docuarchi(i).nombre_campo = deserialize_parameter(i).nombre_campo
                stru_campos_docuarchi(i).valor_campo = deserialize_parameter(i).valor_campo
                stru_campos_docuarchi(i).tipo_campo = deserialize_parameter(i).tipo_campo
            Next
            Dim Result As String = ""
            Dim id_tarea_wf As Long = -1
            Dim radicado As String = ""
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            If tipo_indice_actualiza = 1 Then
                id_tarea_wf = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")
                Result = Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(id_tarea_wf,
                                                                                    radicado)
                If Result <> "YES" Then
                    Return Result
                End If
            End If
            Dim Refclas_ClassWorkflowIndiceDA As New ClassWorkflowIndiceDA
            Result = Refclas_ClassWorkflowIndiceDA.Actualiza_Indice_Imagen_service(HttpContext.Current.Session.Item("WF_ID_GABINETE_SELECCIONADO"),
                                                                                   HttpContext.Current.Session.Item("WF_GABINETE_SELECCIONADO"),
                                                                                   stru_campos_docuarchi,
                                                                                   "",
                                                                                   id_tarea_wf,
                                                                                   radicado)
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
    Public Function Service_actualiza_indice_batch_wf(ByVal parameter As Object,
                                                      ByVal id_parameter As Object,
                                                      ByVal tipo_indice_actualiza As Integer)
        '---------------------------------------------------------------------------
        'Funcion : Actualiza idice de documento desde la ventana de documentos 
        '          asignados       
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'parameter             : Representa la estructura del indice extraidos
        '                        de la interface
        'id_parameter          : Representa el identificador del imagen
        'tipo_indice_actualiza : Representa rl tipo de acutalización para que el sistema
        '                        registre en el sistema de log 
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'id_usuario_radicador  : Retorna la idnetificación del usuario radicador
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-04-01
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim serializer = New JavaScriptSerializer()
            Dim deserialize_parameter = Nothing
            Dim Result As String = ""
            deserialize_parameter = JsonConvert.DeserializeObject(Of List(Of Class_config_general_service))(parameter)
            If deserialize_parameter Is Nothing Then
                Return "Imposible deserealizar los parametros de configuracion"
                Exit Function
            End If
            Dim stru_campos_docuarchi() As stru_campos_docuarchi = Nothing
            Dim iConta As Integer = 0
            For i As Integer = 0 To deserialize_parameter.count - 1
                If deserialize_parameter(i).atrib_chek = 1 Then
                    ReDim Preserve stru_campos_docuarchi(iConta)
                    stru_campos_docuarchi(iConta).nombre_campo = deserialize_parameter(i).name_campo
                    stru_campos_docuarchi(iConta).valor_campo = deserialize_parameter(i).texto_campo
                    stru_campos_docuarchi(iConta).tipo_campo = deserialize_parameter(i).tipo_campo
                    iConta = iConta + 1
                End If
            Next
            If stru_campos_docuarchi Is Nothing Then
                Result = "Debe chequear el campo a cambiar"
                Return Result
            End If
            Dim id_tarea_wf As Long = -1
            Dim radicado As String = ""
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            If tipo_indice_actualiza = 1 Then
                id_tarea_wf = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")
                Result = Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(id_tarea_wf,
                                                                                    radicado)
                If Result <> "YES" Then
                    Return Result
                End If
            End If
            Dim ClassDaGabinete As New ClassDaGabinete
            Result = ClassDaGabinete.ActualizaIndiceDocumentoGabinete(id_parameter,
                                                                      deserialize_parameter(0).tbl_control,
                                                                      stru_campos_docuarchi,
                                                                      "WORKFLOW",
                                                                      HttpContext.Current.Session.Item("Login_Usuario_Workfow"),
                                                                      "",
                                                                      id_tarea_wf,
                                                                      radicado)
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
    Public Function ServiceActualizaIndiceBatchGabinete(ByVal Parameter As List(Of CDParamenterGabinete))
        '---------------------------------------------------------------------------
        'Funcion : Servicio que expone la funcion de  Actualiza idice de documento 
        '          desde la ventana de migracion  documental      
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'parameter             : Representa la estructura del indice extraidos
        '                        de la interface
        '
        '
        '                        
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'Class_config_general_service  : Retorna la estructura de datos
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-08-21
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Dim ListClassConfigGeneralService = New List(Of Class_config_general_service)()
        Dim ItemClassConfigGeneralService As Class_config_general_service = New Class_config_general_service()
        Try
            Dim StruCamposDocuarchi() As stru_campos_docuarchi = Nothing
            Dim iConta As Integer = 0
            Dim ConfigGeneralService As New List(Of Class_config_general_service)
            ConfigGeneralService = Parameter(0).ClassConfigGeneralService
            Dim CamposUpdateIndiceBach As New List(Of CDCamposUpdateIndiceBach)
            For i As Integer = 0 To ConfigGeneralService.Count - 1
                If ConfigGeneralService(i).atrib_chek = 1 Then
                    ReDim Preserve StruCamposDocuarchi(iConta)
                    StruCamposDocuarchi(iConta).nombre_campo = ConfigGeneralService(i).name_campo
                    StruCamposDocuarchi(iConta).valor_campo = ConfigGeneralService(i).texto_campo
                    StruCamposDocuarchi(iConta).tipo_campo = ConfigGeneralService(i).tipo_campo
                    Dim ItemCamposUpdateIndiceBach As New CDCamposUpdateIndiceBach
                    ItemCamposUpdateIndiceBach.NombreCampo = ConfigGeneralService(i).name_campo
                    ItemCamposUpdateIndiceBach.ValorCampo = ConfigGeneralService(i).texto_campo
                    ItemCamposUpdateIndiceBach.TipoCampo = ConfigGeneralService(i).tipo_campo
                    CamposUpdateIndiceBach.Add(ItemCamposUpdateIndiceBach)
                    iConta += 1
                End If
            Next
            ItemClassConfigGeneralService.CamposUpdateIndiceBach = CamposUpdateIndiceBach
            If StruCamposDocuarchi Is Nothing Then
                ItemClassConfigGeneralService.error_gestion = "Debe chequear el campo a cambiar"
                ListClassConfigGeneralService.Add(ItemClassConfigGeneralService)
                Return ListClassConfigGeneralService
            End If
            Dim IdTareaWorkflow As Long = -1
            Dim radicado As String = ""
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim IdImagen As Integer = Parameter(0).IdImagen
            Dim NameTable As String = Parameter(0).NombreGabinete
            Dim NombreModulo As String = Parameter(0).NombreModulo
            ItemClassConfigGeneralService.error_gestion = ClassDaGabinete.ActualizaIndiceDocumentoGabinete(IdImagen,
                                                                                                           NameTable,
                                                                                                           StruCamposDocuarchi,
                                                                                                           NombreModulo,
                                                                                                           HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                                           "",
                                                                                                           IdTareaWorkflow,
                                                                                                           radicado)
            ListClassConfigGeneralService.Add(ItemClassConfigGeneralService)
            Return ListClassConfigGeneralService
        Catch ex As Exception
            ItemClassConfigGeneralService.error_gestion = ex.Message
            ListClassConfigGeneralService.Add(ItemClassConfigGeneralService)
            Return ListClassConfigGeneralService
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_actualiza_indice_batch_migracion_gabinete(ByVal parameter As Object, ByVal id_imagen As Object, ByVal name_table As Object)
        '---------------------------------------------------------------------------
        'Funcion : Servicio que expone la funcion de  Actualiza idice de documento 
        '          desde la ventana de migracion  documental      
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'parameter             : Representa la estructura del indice extraidos
        '                        de la interface
        '
        '
        '                        
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'Class_config_general_service  : Retorna la estructura de datos
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-08-21
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Dim resultList = New List(Of Class_config_general_service)()
        Dim parameter_gestion As Class_config_general_service = New Class_config_general_service()
        Try

            Dim serializer = New JavaScriptSerializer()
            Dim deserialize_parameter As New List(Of Class_config_general_service)()
            Dim Result As String = ""
            deserialize_parameter = JsonConvert.DeserializeObject(Of List(Of Class_config_general_service))(parameter)
            If deserialize_parameter Is Nothing Then
                parameter_gestion.error_gestion = "Imposible deserealizar los parametros de configuracion"
                resultList.Add(parameter_gestion)
                Return resultList
            End If
            If Session.Item("UTIL_MIGRA_UPDATE_INDICE_BATCH") = 0 Then
                parameter_gestion.error_gestion = "El usuario no tiene permiso para actualizar indices"
                resultList.Add(parameter_gestion)
                Return resultList
            End If
            Dim stru_campos_docuarchi() As stru_campos_docuarchi = Nothing
            Dim iConta As Integer = 0
            For i As Integer = 0 To deserialize_parameter.Count - 1
                If deserialize_parameter(i).atrib_chek = 1 Then
                    ReDim Preserve stru_campos_docuarchi(iConta)
                    stru_campos_docuarchi(iConta).nombre_campo = deserialize_parameter(i).name_campo
                    stru_campos_docuarchi(iConta).valor_campo = deserialize_parameter(i).texto_campo
                    stru_campos_docuarchi(iConta).tipo_campo = deserialize_parameter(i).tipo_campo
                    iConta = iConta + 1
                End If
            Next
            If stru_campos_docuarchi Is Nothing Then
                parameter_gestion.error_gestion = "Debe chequear el campo a cambiar"
                resultList.Add(parameter_gestion)
                Return resultList
            End If
            Dim id_tarea_wf As Long = -1
            Dim radicado As String = ""
            Dim ClassDaGabinete As New ClassDaGabinete
            deserialize_parameter(0).tbl_control = name_table
            deserialize_parameter(0).dms_id_registro = id_imagen
            Result = ClassDaGabinete.ActualizaIndiceDocumentoGabinete(id_imagen,
                                                                      name_table,
                                                                      stru_campos_docuarchi,
                                                                      "MIGRACION",
                                                                      HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                      "",
                                                                      id_tarea_wf,
                                                                      radicado)
            deserialize_parameter(0).error_gestion = Result
            Return deserialize_parameter
        Catch ex As Exception
            parameter_gestion.error_gestion = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_actualiza_indice_batch_production(ByVal parameter As Object,
                                                              ByVal id_parameter As Object,
                                                              ByVal tipo_indice_actualiza As Integer)
        '---------------------------------------------------------------------------
        'Funcion : Actualiza idice de documento desde la ventana de producción 
        '          documental      
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'parameter             : Representa la estructura del indice extraidos
        '                        de la interface
        'id_parameter          : Representa el identificador del registro de produccion
        'tipo_indice_actualiza : Representa rl tipo de acutalización para que el sistema
        '                        registre en el sistema de log 
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'id_usuario_radicador  : Retorna la idnetificación del usuario radicador
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-11-24
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim serializer = New JavaScriptSerializer()
            Dim deserialize_parameter = Nothing
            Dim Result As String = ""
            deserialize_parameter = JsonConvert.DeserializeObject(Of List(Of Class_config_general_service))(parameter)
            If deserialize_parameter Is Nothing Then
                Return "Imposible deserealizar los parametros de configuracion"
                Exit Function
            End If
            Dim stru_campos_docuarchi() As stru_campos_docuarchi = Nothing
            Dim iConta As Integer = 0
            For i As Integer = 0 To deserialize_parameter.count - 1
                If deserialize_parameter(i).atrib_chek = 1 Then
                    ReDim Preserve stru_campos_docuarchi(iConta)
                    stru_campos_docuarchi(iConta).nombre_campo = deserialize_parameter(i).name_campo
                    stru_campos_docuarchi(iConta).valor_campo = deserialize_parameter(i).texto_campo
                    stru_campos_docuarchi(iConta).tipo_campo = deserialize_parameter(i).tipo_campo
                    iConta = iConta + 1
                End If
            Next
            If stru_campos_docuarchi Is Nothing Then
                Result = "Debe chequear el campo a cambiar"
                Return Result
            End If
            Dim id_tarea_wf As Long = -1
            Dim radicado As String = ""
            Dim ClassGaProducionDocumental As New ClassGaProducionDocumental
            Dim stru_produccion_indice As stru_produccion_indice = Nothing
            stru_produccion_indice.ID_DOCUMENTO_DOCUARCHI_ALMACEN = -1
            Result = ClassGaProducionDocumental.Solicita_estructura_id_registro_produccion(id_parameter,
                                                                                           stru_produccion_indice)
            If Result <> "YES" Then
                Return Result
            End If
            If stru_produccion_indice.ID_DOCUMENTO_DOCUARCHI_ALMACEN = -1 Then
                Return "Imposible encontrar el registro de producción del parametro (" & id_parameter & ")"
            End If
            Dim ClassDaGabinete As New ClassDaGabinete
            Result = ClassDaGabinete.ActualizaIndiceDocumentoGabinete(stru_produccion_indice.ID_DOCUMENTO_DOCUARCHI_ALMACEN,
                                                                      stru_produccion_indice.NOMBRE_GABINETE,
                                                                      stru_campos_docuarchi,
                                                                      "PRODUCCION",
                                                                      HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                      "",
                                                                      id_tarea_wf,
                                                                      radicado)
            Return Result
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_actualiza_indice_batch_wf_enlace(ByVal parameter As Object,
                                                             ByVal id_parameter As Object,
                                                             ByVal tipo_indice_actualiza As Integer)
        '---------------------------------------------------------------------------
        'Funcion : Actualiza idice de documento desde la ventana de enlace de 
        '          documentos
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'parameter             : Representa la estructura del indice extraidos
        '                        de la interface
        'id_parameter          : Representa el identificador del imagen
        'tipo_indice_actualiza : Representa rl tipo de acutalización para que el sistema
        '                        registre en el sistema de log 
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'id_usuario_radicador  : Retorna la idnetificación del usuario radicador
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-04-01
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim serializer = New JavaScriptSerializer()
            Dim deserialize_parameter = Nothing
            Dim Result As String = ""
            deserialize_parameter = JsonConvert.DeserializeObject(Of List(Of Class_config_general_service))(parameter)
            If deserialize_parameter Is Nothing Then
                Return "Imposible deserealizar los parametros de configuracion"
                Exit Function
            End If
            Dim stru_campos_docuarchi() As stru_campos_docuarchi = Nothing
            Dim iConta As Integer = 0
            For i As Integer = 0 To deserialize_parameter.count - 1
                If deserialize_parameter(i).atrib_chek = 1 Then
                    ReDim Preserve stru_campos_docuarchi(iConta)
                    stru_campos_docuarchi(iConta).nombre_campo = deserialize_parameter(i).name_campo
                    stru_campos_docuarchi(iConta).valor_campo = deserialize_parameter(i).texto_campo
                    stru_campos_docuarchi(iConta).tipo_campo = deserialize_parameter(i).tipo_campo
                    iConta = iConta + 1
                End If
            Next
            If stru_campos_docuarchi Is Nothing Then
                Result = "Debe chequear el campo a cambiar para campos enlace"
                Return Result
            End If
            Dim id_tarea_wf As Long = -1
            Dim radicado As String = ""
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            If tipo_indice_actualiza = 1 Then
                id_tarea_wf = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA_ENLACE")
                Result = Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(id_tarea_wf,
                                                                                    radicado)
                If Result <> "YES" Then
                    Return Result
                End If
            End If
            Dim ClassDaGabinete As New ClassDaGabinete
            Result = ClassDaGabinete.ActualizaIndiceDocumentoGabinete(id_parameter,
                                                                      deserialize_parameter(0).tbl_control,
                                                                      stru_campos_docuarchi,
                                                                      "WORKFLOW",
                                                                       HttpContext.Current.Session.Item("Login_Usuario_Workfow"),
                                                                      "",
                                                                      id_tarea_wf,
                                                                      radicado)
            If Result <> "YES" Then
                Return Result
            Else
                Return "YES"
            End If
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function
    Private MYSQL_SELECT_COMMAND As MySqlCommand
    Private Function Returna_Conexion_Mysql(ByRef CconectionMysql As MySql.Data.MySqlClient.MySqlConnection) As String
        Dim poltrue As String = "False"
        If HttpContext.Current.Session.Item("DA_ACTIVA_POOL_DBMS") = "1" Then
            poltrue = "True"
        Else
            poltrue = "False"
        End If
        Dim Contenido_Config As String = "Persist Security Info=" _
          & True & ";database=" & HttpContext.Current.Session("DA_DB_NAME_MODULO").ToString _
          & ";server=" & HttpContext.Current.Session("DA_IP_SERVER_MODULO").ToString _
         & ";user id=" & HttpContext.Current.Session("DA_USER_DBMS_MODULO").ToString _
         & ";pwd=" & HttpContext.Current.Session("DA_PASW_DBMS_MODULO").ToString _
         & ";Pooling=" & poltrue & ";Min Pool Size=0;Max Pool Size=" &
         HttpContext.Current.Session.Item("DA_NUMERO_DBMS_CONEX")


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
        If HttpContext.Current.Session("DA_TYPE_DBMS_MODULO").ToString = "mysql" Then
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
End Class