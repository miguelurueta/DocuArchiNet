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
' Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente.
<System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
Public Class WebServiceInicioGestor
    Inherits System.Web.Services.WebService
    Public Class ArrayItem
        Public Text_node As String
        Public value_node As String
        Public Toltip_node As String
        Public url_node As String
        Public visible_node As Integer
        Public nodo_plantilla_radicado As String
        Public tipo_plantilla As String
        Public id_plantilla As Integer
        Public error_funcion As String
        Public url_externa As String
        Public url_content As String
        Public value_content As String
        Public value_card As String
        Public value_card_conten As String
        Public tipo_modulo As String
    End Class
    <WebMethod(EnableSession:=True)> _
  <Script.Services.ScriptMethod()> _
    Public Function web_service_lista_item_menu(ByVal DName As String)
        Dim country As New List(Of ArrayItem)
        Try
            Dim Ref_classGestorSesion As New ClassGestorSesion
            Dim stru_inicio_workflow() As stru_inicio_menu = Nothing
            Dim Result As String = ""
            Result = Ref_classGestorSesion.Inicializa_menu_principal()
            If Result <> "YES" Then
                Dim item As New ArrayItem
                item.error_funcion = Result
                country.Add(item)
                Return country
            End If
            Result = Ref_classGestorSesion.Solicita_items_modulos_workflow(HttpContext.Current.Session("TIPOMODULO"), _
                                                                           stru_inicio_workflow)
            If Result <> "YES" Then
                Dim item As New ArrayItem
                item.error_funcion = Result
                country.Add(item)
                Return country
            End If
            Result = Ref_classGestorSesion.Solicita_item_modulo_docuarchi(HttpContext.Current.Session("TIPOMODULO"), _
                                                                          stru_inicio_workflow)
            If Result <> "YES" Then
                Dim item As New ArrayItem
                item.error_funcion = Result
                country.Add(item)
                Return country
            End If
            Result = Ref_classGestorSesion.Solicita_items_modulo_correspondencia(HttpContext.Current.Session("TIPOMODULO"), _
                                                                                 stru_inicio_workflow)
            If Result <> "YES" Then
                Dim item As New ArrayItem
                item.error_funcion = Result
                country.Add(item)
                Return country
            End If
            Result = Ref_classGestorSesion.Solicita_items_modulo_gestion(HttpContext.Current.Session("TIPOMODULO"), _
                                                                         stru_inicio_workflow)
            If Result <> "YES" Then
                Dim item As New ArrayItem
                item.error_funcion = Result
                country.Add(item)
                Return country
            End If
            If stru_inicio_workflow Is Nothing Then
                Dim item As New ArrayItem
                item.error_funcion = Result
                country.Add(item)
                Return country
            Else
                For i As Integer = 0 To stru_inicio_workflow.Length - 1
                    Dim item As New ArrayItem
                    item.error_funcion = Result
                    item.id_plantilla = stru_inicio_workflow(i).id_plantilla
                    item.nodo_plantilla_radicado = stru_inicio_workflow(i).nodo_plantilla_radicado
                    item.Text_node = stru_inicio_workflow(i).Text_node
                    item.tipo_plantilla = stru_inicio_workflow(i).tipo_plantilla
                    item.Toltip_node = stru_inicio_workflow(i).Toltip_node
                    item.url_node = stru_inicio_workflow(i).url_node
                    item.value_node = stru_inicio_workflow(i).value_node
                    item.visible_node = stru_inicio_workflow(i).visible_node
                    item.url_externa = stru_inicio_workflow(i).url_externa
                    item.url_content = stru_inicio_workflow(i).url_content
                    item.value_content = stru_inicio_workflow(i).value_content
                    item.value_card = stru_inicio_workflow(i).value_card
                    item.value_card_conten = stru_inicio_workflow(i).value_card_conten
                    item.tipo_modulo = stru_inicio_workflow(i).tipo_modulo
                    country.Add(item)
                Next
                Return country
            End If

        Catch ex As Exception
            Dim item As New ArrayItem
            item.error_funcion = "Inconsistencia general funcion web_service_lista_item_menu : " & ex.Message
            country.Add(item)
            Return country
        End Try
    End Function

    <WebMethod(EnableSession:=True)> _
<Script.Services.ScriptMethod()> _
    Public Function web_service_inicializa_menu_principal(ByVal DName As String)
        Dim country As New List(Of ArrayItem)
        Try
            Dim Ref_classGestorSesion As New ClassGestorSesion
            Dim Result As String = ""
            Result = Ref_classGestorSesion.Inicializa_menu_principal()
            If Result <> "YES" Then
                Return Result
            Else
                Return "YES"
            End If
        Catch ex As Exception
            Return "Inconsistencia general funcion web_service_inicializa_menu_principal : " & ex.Message
        End Try
    End Function
    <WebMethod(EnableSession:=True)> _
<Script.Services.ScriptMethod()> _
    Public Function web_service_sesion_user(ByVal DName As String)
        Dim country As New List(Of ArrayItem)
        Try
            Dim Ref_classGestorSesion As New ClassGestorSesion
            Dim Result As String = ""
            Dim nombre_usuario_radicador As String = ""
            Dim cargo_usuario_radicador As String = ""
            Dim sede_empresa As String = ""
            Dim Class_usuario_radicador As New Class_usuario_radicador
            Dim Class_remit_dest_interno As New Class_remit_dest_interno
            If HttpContext.Current.Session("TIPOMODULO") = "RADICACION DOCUMENTAL" Then
                Result = Class_usuario_radicador.Solicita_caraterizacion_usuario_radicador_gestion(HttpContext.Current.Session.Item("RA_ID_USUARIO"), _
                                                                                                  nombre_usuario_radicador, _
                                                                                                  cargo_usuario_radicador, _
                                                                                                  sede_empresa)
                If Result <> "YES" Then
                    Return Result
                Else
                    Return UCase(HttpContext.Current.Session.Item("RA_LOGIN_USER")) & " (" & cargo_usuario_radicador & ")"
                End If
            End If
            If HttpContext.Current.Session("TIPOMODULO") = "DOCUARCHI CONTENEDOR" Then
                Return HttpContext.Current.Session.Item("DA_Login_Usuario")
            End If
            If HttpContext.Current.Session("TIPOMODULO") = "GESTOR DOCUMENTAL" Then
                Result = Class_remit_dest_interno.Solicita_detalle_usuario_gestion(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                   nombre_usuario_radicador,
                                                                                   cargo_usuario_radicador,
                                                                                   sede_empresa)
                If Result <> "YES" Then
                    Return ""
                Else
                    Return UCase(HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION")) & " (" & cargo_usuario_radicador & ")"
                End If
            End If
            Dim ClassWorkflowUsuario As New ClassWorkflowUsuario
            If HttpContext.Current.Session("TIPOMODULO") = "WORKFLOW DOCUMENTAL" Then
                Result = ClassWorkflowUsuario.Solicita_nombre_cargo_usuario_workflow(HttpContext.Current.Session.Item("Id_Usuario_Workflow"), nombre_usuario_radicador, cargo_usuario_radicador)
                If Result <> "YES" Then
                    Return ""
                Else
                    Return UCase(HttpContext.Current.Session("Login_Usuario_Workfow")) & " (" & cargo_usuario_radicador & ")"
                End If

            End If
            Return "Loguin sin identificar"
        Catch ex As Exception
            Return "Inconsistencia general funcion web_service_inicializa_menu_principal : " & ex.Message
        End Try
    End Function
    <WebMethod(EnableSession:=True)> _
<Script.Services.ScriptMethod()> _
    Public Function web_service_loguin_user(ByVal DName As String)
        Dim country As New List(Of ArrayItem)
        Try
            Dim Ref_classGestorSesion As New ClassGestorSesion
            Dim Result As String = ""
            Dim nombre_usuario_radicador As String = ""
            Dim cargo_usuario_radicador As String = ""
            Dim sede_empresa As String = ""
            Dim Class_usuario_radicador As New Class_usuario_radicador
            Dim Class_remit_dest_interno As New Class_remit_dest_interno
            If HttpContext.Current.Session("TIPOMODULO") = "RADICACION DOCUMENTAL" Then
                Result = Class_usuario_radicador.Solicita_caraterizacion_usuario_radicador_gestion(HttpContext.Current.Session.Item("RA_ID_USUARIO"), _
                                                                                                  nombre_usuario_radicador, _
                                                                                                  cargo_usuario_radicador, _
                                                                                                  sede_empresa)
                If Result <> "YES" Then
                    Return Result
                Else
                    Return HttpContext.Current.Session.Item("RA_LOGIN_USER")
                End If
            End If
            If HttpContext.Current.Session("TIPOMODULO") = "DOCUARCHI CONTENEDOR" Then
                Return HttpContext.Current.Session.Item("DA_Login_Usuario")
            End If
            If HttpContext.Current.Session("TIPOMODULO") = "GESTOR DOCUMENTAL" Then
                Result = Class_remit_dest_interno.Solicita_detalle_usuario_gestion(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                      nombre_usuario_radicador,
                                                                                      cargo_usuario_radicador,
                                                                                      sede_empresa)
                If Result <> "YES" Then
                    Return Result
                Else
                    Return HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION")
                End If
            End If
            If HttpContext.Current.Session("TIPOMODULO") = "WORKFLOW DOCUMENTAL" Then
                Return HttpContext.Current.Session("Login_Usuario_Workfow")
            End If
            Return "Loguin sin identificar"
        Catch ex As Exception
            Return "Inconsistencia general funcion web_service_loguin_user : " & ex.Message
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function web_service_update_radicado_consulta(ByVal DName As String)
        Dim country As New List(Of ArrayItem)
        Try
            Session.Item("RA_MODULO_SELECCIONADO") = DName
            Return "YES"
        Catch ex As Exception
            Return "Inconsistencia general funcion web_service_inicializa_menu_principal : " & ex.Message
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function web_service_validate_sesion_active(ByVal DName As Object)
        Try
            Dim ClassGestorSesion As New ClassGestorSesion
            Dim ValueSesion = ClassGestorSesion.IsSessionTimedOut
            If ValueSesion = False Then
                Return "YES"
            Else
                Return "FALSE"
            End If
        Catch ex As Exception
            Return "FALSE"
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function web_service_sesion_end(ByVal DName As String)
        Try
            Session.Abandon()
            Return "YES"
        Catch ex As Exception
            Return "FALSE"
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_sesion_return_timeout(ByVal DName As String)
        Dim tiempo As String = ""
        Try
            Dim ClassGestorSesion As New ClassGestorSesion
            ClassGestorSesion.Tiempo_sesion(Session.Timeout, tiempo)
            Return tiempo
        Catch ex As Exception
            Return tiempo
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_inicializa_conexion_consulta_publica(ByVal DName As String)

        Try
            Dim Gestor_conexion As New Gestor_conexion
            Dim Result As String = ""
            Result = Gestor_conexion.Inicializa_conexion_consulta_publica(DName)
            Return Result
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function
End Class