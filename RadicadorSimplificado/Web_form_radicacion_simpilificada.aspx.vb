Public Class Web_form_radicacion_simpilificada
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'Dim class_gestor_sesion As New GestorModuleSesion.Gestor_conexion
        'Dim Result = class_gestor_sesion.Asigna_detalle_inicio_confirmacion("VILLAVIVIENDA")
        'If Result <> "YES" Then
        '    Context.Response.Write(Result)
        '    Exit Sub
        'End If
        'HttpContext.Current.Session.Item("ip_host_name") = "localhost"
        'HttpContext.Current.Session.Item("DA_Login_Usuario") = "LUZ.AGUILERA"
        'Session.Item("GA_LOGINUSUARIOGESTION") = "LUZ.AGUILERA"
        'Session.Item("Id_Usuario_Workflow") = 137
        'Session.Item("Id_actividad_Workflow") = 91
        'Session.Item("Id_Grupo_Workflow") = 44
        'Session.Item("Id_Ruta_Workflow") = 9
        'Session.Item("RA_ID_USUARIO") = 27
        'HttpContext.Current.Session("WF_RUTAWORKFLOW") = "CONTABILIDADCCV"
        'HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") = 141
        'Session.Item("UTIL_MIGRA_FORMATO_ARCHIVO") = 1
        'Session.Item("UTIL_MIGRA_LOAD_FORMATO_ARCHIVO") = 1
        'Session.Item("UTIL_MIGRA_UPDATE_TIPOLOGIA") = 1
        'HttpContext.Current.Session.Item("UTIL_VER_MIG_RESTAURA_VERSION_DOCUMENTO_GABINETE") = 1
        'Session.Item("ID_USUARIO_DOCUARCHI") = 1
        'HttpContext.Current.Session.Item("UTIL_MIGRA_REMPLAZA_VERSION_DOCUMENTO") = 1
        'HttpContext.Current.Session.Item("UTIL_VER_MIG_ELIMINA_VERSION_DOCUMENTO") = 1
        'HttpContext.Current.Session.Item("UTIL_VER_MIG_REMPLAZA_VERSION_DOCUMENTO") = 1
        'HttpContext.Current.Session.Item("UTIL_VER_MASTER_REMPLAZA_VERSION_DOCUMENTO") = 1
        'HttpContext.Current.Session.Item("UTIL_MIG_AUTO_VINCULA_DOC_EXPEDIENTE") = 1
        'HttpContext.Current.Session.Item("UTIL_MIGRA_UPDATE_INDICE_BATCH") = 1
        'HttpContext.Current.Session.Item("GA_IDEMPRESA") = 2
        'Session.Item("UTIL_VER_CON_MIGRA_ELIMINA_VERSION_DOCUMENTO") = 1
        'HttpContext.Current.Session.Item("RA_VALIDACION_AGREGAR") = 1
        'HttpContext.Current.Session.Item("RA_VALIDACION_EDITAR") = 1
        'HttpContext.Current.Session.Item("RA_VALIDACION_ELIMINAR") = 1
        Dim cs As ClientScriptManager = Page.ClientScript
        Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", "$(document).ready(function () {$().inicio();});"))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
        End If
    End Sub

End Class