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
Imports Newtonsoft.Json


' Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class WebService_Config_Digitalizacion
    Inherits System.Web.Services.WebService
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function ServiceSolicitaEstructuraConfiguracion(ByVal IdTipoTramite As Object)
        '-----------------------------------------------------------------------------------------------
        'Funcion : Servicio que expone la estructura con la configuración de la digitalziación 
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdTipoTramite       : Representa la identificación del tipo tramite
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'interface_config_digitaliza  : Retorna la estructura con la configuracio de la digitalziacion
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-04-21
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Dim resultList = New List(Of interface_config_digitaliza)()
        Dim parameter_gestion As interface_config_digitaliza = New interface_config_digitaliza()
        Try
            Dim Class_ra_dig_config_digitalizacion As New Class_ra_dig_config_digitalizacion
            parameter_gestion.error_gestion = Class_ra_dig_config_digitalizacion.Solicita_datos_estructura_config_tramite(IdTipoTramite,
                                                                                                                          parameter_gestion)
            resultList.Add(parameter_gestion)
            Return resultList
        Catch ex As Exception
            parameter_gestion.error_gestion = "Inconsistencia general funcion ServiceRESTsolicitaEstructuraConfiguracion " & ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_inicializa_dgitalizacion(ByVal parameter As Object)
        Dim resultList = New List(Of interface_config_digitaliza)()
        Dim parameter_gestion As interface_config_digitaliza = New interface_config_digitaliza()
        '-----------------------------------------------------------------------------------------------
        'Funcion : Servicio que expone la asignación de la función de digitalizacion 
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'parameter           : Representa el nombre de la función de digitalización
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        '
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-01-28
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            HttpContext.Current.Session.Item("DG_TIPODIGITALIZACION") = parameter
            parameter_gestion.error_gestion = "YES"
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
    Public Function Service_solicita_estructura_interface_digitalizacion(ByVal radicado As Object) As IEnumerable(Of interface_config_digitaliza)
        Dim resultList = New List(Of interface_config_digitaliza)()
        Dim parameter_gestion As interface_config_digitaliza = New interface_config_digitaliza()
        Try
            Dim Result As String = ""
            Dim Class_tipo_doc_entrante As New Class_tipo_doc_entrante
            Dim Class_ra_dig_config_digitalizacion As New Class_ra_dig_config_digitalizacion
            Dim Class_ra_config_digitalizacion_user_gestion As New Class_ra_config_digitalizacion_user_gestion
            '---------------------------------------------------------
            'Solicita configuración tramite para digitalización
            '---------------------------------------------------------
            If Session.Item("DG_TRAMITE_DIGITAIZACION") <> -1 Then
                parameter_gestion.Id_Ra_Config = Session.Item("DG_TRAMITE_DIGITAIZACION")
                Result = Class_ra_dig_config_digitalizacion.Solicita_datos_estructura_config_tramite(Session.Item("DG_TRAMITE_DIGITAIZACION"),
                                                                                                     parameter_gestion)
                If Result <> "YES" Then
                    parameter_gestion.error_gestion = Result
                    resultList.Add(parameter_gestion)
                    Return resultList
                End If
            Else
                parameter_gestion.Id_Ra_Config = -1
            End If
            If parameter_gestion.Id_Ra_Config = -1 Then
                Dim id_tramite As Integer = 0
                Result = Class_tipo_doc_entrante.Solicita_tramite_default_digitalizacion(id_tramite)
                If Result <> "YES" Then
                    parameter_gestion.error_gestion = Result
                    resultList.Add(parameter_gestion)
                    Return resultList
                End If
                Result = Class_ra_dig_config_digitalizacion.Solicita_datos_estructura_config_tramite(id_tramite,
                                                                                                     parameter_gestion)
                If Result <> "YES" Then
                    parameter_gestion.error_gestion = Result
                    resultList.Add(parameter_gestion)
                    Return resultList
                End If
            End If
            '----------------------------------------------------------
            'Solicita configuración escaner del usuario de gestion
            'aqui guarda los parametros
            '----------------------------------------------------------
            Result = Class_ra_config_digitalizacion_user_gestion.Solicita_existencia_configuracion_interface_digitalizacion(Session.Item("GA_IDUSUARIOGESTION"),
                                                                                                                            parameter_gestion)
            If Result <> "YES" Then
                parameter_gestion.error_gestion = Result
                resultList.Add(parameter_gestion)
                Return resultList
            End If
            If parameter_gestion.id_config_digitalizacion = -1 Then
                Result = Class_ra_config_digitalizacion_user_gestion.Registro_configuracion_interface_user_escaner(Session.Item("GA_IDUSUARIOGESTION"),
                                                                                                                   parameter_gestion)
                If Result <> "YES" Then
                    parameter_gestion.error_gestion = Result
                    resultList.Add(parameter_gestion)
                    Return resultList
                Else
                    parameter_gestion.error_gestion = Result
                    resultList.Add(parameter_gestion)
                    Return resultList
                End If
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
    Public Function Service_actualiza_estructura_interface_digitalizacion(ByVal parameter As Object) As IEnumerable(Of interface_config_digitaliza)
        Dim resultList = New List(Of interface_config_digitaliza)()
        Dim parameter_gestion As interface_config_digitaliza = New interface_config_digitaliza()
        Try
            Dim Result As String = ""
            Dim Class_ra_config_digitalizacion_user_gestion As New Class_ra_config_digitalizacion_user_gestion
            Dim deserialize_parameter = Nothing
            deserialize_parameter = JsonConvert.DeserializeObject(Of interface_config_digitaliza())(parameter)
            Result = Class_ra_config_digitalizacion_user_gestion.Update_configuracion_interface_user_escaner(Session.Item("GA_IDUSUARIOGESTION"),
                                                                                                                deserialize_parameter(0))
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
End Class