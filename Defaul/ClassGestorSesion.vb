Imports System.Web.Profile
Imports System.Configuration.Provider
Imports System.Collections.Specialized
Imports System
Imports System.Data
Imports System.Data.Odbc
Imports System.Configuration
Imports System.Diagnostics
Imports System.Web
Imports System.Collections
Imports Microsoft.VisualBasic
Public Structure stru_inicio_menu
    Dim Text_node As String
    Dim value_node As String
    Dim Toltip_node As String
    Dim url_node As String
    Dim visible_node As Integer
    Dim nodo_plantilla_radicado As String
    Dim tipo_plantilla As String
    Dim id_plantilla As Integer
    Dim url_externa As String
    Dim url_content As String
    Dim value_content As String
    Dim value_card As String
    Public value_card_conten As String
    Public tipo_modulo As String
End Structure
Public Class ClassGestorSesion
    Public Structure stru_detalle_web_service
        Dim id_modulo As Integer
        Dim ACTIVA_WEB_SERVICE As Integer
        Dim URL_WEB_SERVICE As String
        Dim USER_WEB_SERVICE As String
        Dim PASW_WEB_SERVICE As String
        Dim TIPO_MODULO As String
    End Structure
    Function Tiempo_sesion(ByVal tiempo As Object, ByRef p As String) As String
        Try
            Dim dtmyTime As DateTime
            dtmyTime = DateTime.Now
            dtmyTime = dtmyTime.AddMinutes(tiempo)
            p = dtmyTime.ToString
            Tiempo_sesion = "YES"
        Catch ex As Exception
            Tiempo_sesion = ex.Message
        End Try
    End Function
    Function Asigna_ip_host_cliente() As String
        Try
            Dim ClientIP, Forwaded, RealIP
            RealIP = ""
            ClientIP = HttpContext.Current.Request.ServerVariables("REMOTE_HOST")
            RealIP = HttpContext.Current.Request.ServerVariables("REMOTE_ADDR")
            Forwaded = HttpContext.Current.Request.ServerVariables("HTTP_X-Forwarded-For")
            HttpContext.Current.Session.Item("ip_host_name") = "IP clinet " & ClientIP & " Dns Cliente " & RealIP & " Forward " & Forwaded
            Asigna_ip_host_cliente = "YES"
        Catch ex As Exception
            Asigna_ip_host_cliente = "Inconsistencia general funcion Asigna_ip_host_cliente " & ex.Message
        End Try
    End Function
    Public Function IsSessionTimedOut() As Boolean
        Dim ctx As HttpContext = HttpContext.Current
        Dim ctx_sesion As Object = HttpContext.Current.Session
        If HttpContext.Current.Session.Item("USER_DBMS_MODULO") Is Nothing Then
            Return True
        End If
        If HttpContext.Current.Session.Item("USER_DBMS_MODULO") = "" Then
            Return True
        End If
        If HttpContext.Current.Session.IsNewSession = True Then
            Return True
        End If
        If ctx Is Nothing Then
            'Throw New Exception("Este método sólo se puede usar en una aplicación Web")
            Return True
        End If
        'Comprobamos que haya sesión en primer lugar 
        '(por ejemplo si por ejemplo EnableSessionState=false)
        If ctx.Session Is Nothing Then
            Return False 'Si no hay sesión, no puede caducar
        End If

        'Se comprueba si se ha generado una nueva sesión en esta petición
        If Not ctx.Session.IsNewSession Then
            Return False 'Si no es una nueva sesión es que no ha caducado
        End If

        Dim objCookie As HttpCookie = ctx.Request.Cookies("ASP.NET_SessionId")
        'Esto en teoría es imposible que pase porque si hay una 
        'nueva sesión debería existir la cookie, pero lo compruebo porque
        'IsNewSession puede dar True sin ser cierto (más en el post)
        If objCookie Is Nothing Then
            Return False
        End If

        'Si hay un valor en la cookie es que hay un valor de sesión previo, pero como la sesión 
        'es nueva no debería estar, por lo que deducimos que la sesión anterior ha caducado
        If Not String.IsNullOrEmpty(objCookie.Value) Then
            Return True
        Else
            Return False
        End If
    End Function
    Function NodoChild_Selecionado_busqueda(ByRef Tre_vie As TreeView,
                                            ByRef Datos_Nodo As String,
                                            ByVal texto_nodo As String) As String
        Try
            Datos_Nodo = ""
            Dim Result As String = ""
            If Tre_vie.Nodes.Count > 0 Then
                Dim i As Integer = 0
                For i = 0 To Tre_vie.Nodes.Count - 1
                    Result = Nod_CHILD_busqueda(Tre_vie.Nodes(i), Datos_Nodo, texto_nodo)
                    If Datos_Nodo <> "" Then
                        NodoChild_Selecionado_busqueda = "YES"
                        Return NodoChild_Selecionado_busqueda
                    End If
                Next
            End If
            NodoChild_Selecionado_busqueda = "YES"
        Catch ex As Exception
            NodoChild_Selecionado_busqueda = ex.Message
        End Try
    End Function
    Function Nod_CHILD_busqueda(ByVal NodeC As TreeNode,
                                ByRef Datos_Nodo As String,
                                ByVal texto_nodo As String) As String
        Try
            Dim i As Integer = 0
            For i = 0 To NodeC.ChildNodes.Count - 1
                If NodeC.ChildNodes(i).Text = texto_nodo Then
                    Datos_Nodo = NodeC.ChildNodes(i).Value & "|" & NodeC.ChildNodes(i).Text
                    'NodeC.ChildNodes(i).Parent.ExpandAll()
                    node_expand_recursive(NodeC.ChildNodes(i))
                    NodeC.ChildNodes(i).Selected = True
                    Nod_CHILD_busqueda = "YES"
                    Return Nod_CHILD_busqueda
                End If
                Nod_CHILD_busqueda(NodeC.ChildNodes(i), Datos_Nodo, texto_nodo)
            Next
            Nod_CHILD_busqueda = "YES"
        Catch ex As Exception
            Nod_CHILD_busqueda = ex.Message
        End Try

    End Function
    Function node_expand_recursive(ByRef NodeC As TreeNode) As String
        Try
            Dim Result As String = ""
            If NodeC.Expanded = True Then
                node_expand_recursive = "YES"
                Exit Function
            End If
            NodeC.Expand()
            node_expand_recursive(NodeC.Parent)
            node_expand_recursive = "YES"
        Catch ex As Exception
            node_expand_recursive = ex.Message
        End Try
    End Function
    Function NodoChild_Selecionado(ByRef Tre_vie As TreeView,
                                   ByRef Datos_Nodo As String) As String
        Try
            Datos_Nodo = ""
            Dim Result As String = ""
            If Tre_vie.Nodes.Count > 0 Then
                Dim i As Integer = 0
                For i = 0 To Tre_vie.Nodes.Count - 1
                    Result = Nod_CHILD(Tre_vie.Nodes(i), Datos_Nodo)
                    If Datos_Nodo <> "" Then
                        NodoChild_Selecionado = "YES"
                        Return NodoChild_Selecionado
                    End If
                Next
            End If
            NodoChild_Selecionado = "YES"
        Catch ex As Exception
            NodoChild_Selecionado = ex.Message
        End Try
    End Function
    Function Nod_CHILD(ByVal NodeC As TreeNode, ByRef Datos_Nodo As String) As String
        Try
            Dim i As Integer = 0
            For i = 0 To NodeC.ChildNodes.Count - 1
                If NodeC.ChildNodes(i).Selected Then
                    Datos_Nodo = NodeC.ChildNodes(i).Value & "|" & NodeC.ChildNodes(i).Text
                    Nod_CHILD = "YES"
                    Return Nod_CHILD
                End If
                Nod_CHILD(NodeC.ChildNodes(i), Datos_Nodo)
            Next
            Nod_CHILD = "YES"
        Catch ex As Exception
            Nod_CHILD = ex.Message
        End Try

    End Function
    Function Busca_nodo_treview(ByVal Treeview As TreeView,
                                ByVal clave_nodo As String,
                                ByRef trenod As TreeNode) As String
        Try
            trenod = Treeview.FindNode(clave_nodo)
            If Not trenod Is Nothing Then
                Busca_nodo_treview = "YES"
                Exit Function
            Else
                Busca_nodo_treview = "Imposible encontrar el nodo " & clave_nodo
                Exit Function
            End If
        Catch ex As Exception
            Busca_nodo_treview = "Inconsistencia general función Busca_nodo_treview " & ex.Message
        End Try
    End Function
    Function Proced_Listar_Modulos_Empesas(
                                           ByVal Nombre_Empresa As String,
                                           ByRef refdropmodulo As DropDownList) As String
        '***************************************************
        'Funcion : Lista los modulos que tiene licenciada
        'la empresa 
        'Fecha: 2012-08-16
        'Ingeniero Miguel Angel Urueta Miranda
        '***************************************************
        Try
            Dim Refclas As New Gestor_conexion
            Dim Result As String = ""
            Dim Modulo_colect() As String
            Erase Modulo_colect
            Result = Refclas.SolicitaModulosEmpresa(Nombre_Empresa,
                                                             Modulo_colect)
            If Result <> "YES" Then
                Proced_Listar_Modulos_Empesas = Result
                Exit Function
            End If
            refdropmodulo.Items.Clear()
            If Not Modulo_colect Is Nothing Then
                For i As Integer = 0 To Modulo_colect.Length - 1
                    refdropmodulo.Items.Add(Modulo_colect(i).ToString)
                Next
            End If
            Proced_Listar_Modulos_Empesas = "YES"
        Catch ex As Exception
            Proced_Listar_Modulos_Empesas = "Inconsistencia general funcion (Proced_Listar_Modulos_Empesas) " & ex.Message
        End Try
    End Function
    Function Proced_Listar_empresas(ByRef Refer As Object,
                                    ByRef Empresas_gestion As String,
                                    ByRef refdropmodulo As DropDownList) As String
        '***************************************************
        'Funcion : Lista las empresas que tiene licenciada
        'la empresa y los modulos que tiene cada empresa
        'Fecha: 2012-08-15
        'Ingeniero Miguel Angel Urueta Miranda
        '***************************************************
        Try
            Dim Refclas As New Gestor_conexion
            Dim Result As String = ""
            Dim Empresas_colect() As String
            Erase Empresas_colect
            Result = Refclas.Gestor_Retorna_Empresa(Empresas_gestion)
            If Result <> "YES" Then
                Proced_Listar_empresas = "Imposible cargar las empresas de gestion " & Result
                Exit Function
            End If
            refdropmodulo.Items.Clear()
            If Empresas_gestion <> "" Then
                Dim Modulo_colect() As String
                Erase Modulo_colect
                Result = Refclas.SolicitaModulosEmpresa(Empresas_gestion,
                                                        Modulo_colect)
                If Result <> "YES" Then
                    Proced_Listar_empresas = "Imposible cargar modulos de empresa " & Result
                    Exit Function
                End If
                If Not Modulo_colect Is Nothing Then
                    For i As Integer = 0 To Modulo_colect.Length - 1
                        refdropmodulo.Items.Add(Modulo_colect(i))
                    Next
                End If
            End If
            Proced_Listar_empresas = "YES"
        Catch ex As Exception
            Proced_Listar_empresas = "Inconsistencia general en la funcion (Proced_Listar_empresas)" & ex.Message
        End Try
    End Function
    Function selecciona_treview_general(ByRef page1 As Page,
                                        ByVal texto_busqueda As String,
                                        ByRef nombre_treview As String) As String
        Try
            Dim ref_TreeView1 As TreeView = page1.FindControl(nombre_treview)
            If ref_TreeView1 Is Nothing Then
                selecciona_treview_general = "Imposible encontrar control  " & nombre_treview
                Exit Function
            End If
            Dim Matri_Nodo() As String
            Erase Matri_Nodo
            Dim Datos_Nodo As String = ""
            Dim Result As String = ""
            'consulta dato de nodo seleccionado
            If texto_busqueda = "" Then
                Result = Me.NodoChild_Selecionado(ref_TreeView1,
                                                  Datos_Nodo)
                If Result <> "YES" Then
                    selecciona_treview_general = Result
                    Exit Function
                Else
                    Matri_Nodo = Split(Datos_Nodo, "|")
                End If
            Else
                Result = Me.NodoChild_Selecionado_busqueda(ref_TreeView1,
                                                           Datos_Nodo,
                                                           texto_busqueda)
                If Result <> "YES" Then
                    selecciona_treview_general = Result
                    Exit Function
                Else
                    Matri_Nodo = Split(Datos_Nodo, "|")
                End If
            End If
            selecciona_treview_general = "YES"
        Catch ex As Exception
            selecciona_treview_general = "Inconsistencia general función selecciona_treview_general " & ex.Message
        End Try

    End Function
    Function Inicializa_menu_principal() As String
        Try
            Dim refclasiniciowf As New InicioWorkflow
            Dim refclassradicado As New ClassRadicador
            Dim refclas_inicio_publico As New Classincipublico
            Dim Refclas As New ClassInicioRadicador
            Dim ref_Class_perfilar_usuario_radicador As New Class_perfilar_usuario_radicador
            Dim Result As String = ""
            If HttpContext.Current.Session("TIPOMODULO") = "RADICACION DOCUMENTAL" Then
                Result = ref_Class_perfilar_usuario_radicador.AsignaPermisosSesionUsuarioRadicador(HttpContext.Current.Session.Item("RA_ID_USUARIO"))
                If Result <> "YES" Then
                    Inicializa_menu_principal = "Imposible cargar permisos de radicacion " & Result
                    Exit Function
                End If
                Dim matri_platilla() As plantillas
                Erase matri_platilla
                Dim ref_Class_permisos_plantilla As New Class_permisos_plantilla
                Result = ref_Class_permisos_plantilla.Solicita_plantillas_radicado_permitidas_usuario_radicador(HttpContext.Current.Session.Item("RA_ID_USUARIO"),
                                                                                                                matri_platilla)
                If Result <> "YES" Then
                    Inicializa_menu_principal = "Imposible cargar matriz de plantillas " & Result
                    Exit Function
                End If
                Dim matri_plantillas_validacion() As String = Nothing
                Result = refclassradicado.Lista_plantillas_validacion_externa(matri_plantillas_validacion)
                If Result <> "YES" Then
                    Inicializa_menu_principal = "Imposible cargar validacion externa " & Result
                    Exit Function
                End If
                Result = Refclas.Retorna_tipo_Impresion(HttpContext.Current.Session.Item("RA_ID_USUARIO"))
                If Result <> "YES" Then
                    Inicializa_menu_principal = "Imposible retornar tipo impresón " & Result
                    Exit Function
                End If
            End If
            If HttpContext.Current.Session("TIPOMODULO") = "GESTOR DOCUMENTAL" Then
                If HttpContext.Current.Session.Item("RA_ID_USUARIO") <> 0 Then
                    Result = ref_Class_perfilar_usuario_radicador.AsignaPermisosSesionUsuarioRadicador(HttpContext.Current.Session.Item("RA_ID_USUARIO"))
                    If Result <> "YES" Then
                        Inicializa_menu_principal = "Imposible cargar permisos radicacion " & Result
                        Exit Function
                    End If
                    Dim matri_platilla() As plantillas
                    Erase matri_platilla
                    Dim ref_Class_permisos_plantilla As New Class_permisos_plantilla
                    Result = ref_Class_permisos_plantilla.Solicita_plantillas_radicado_permitidas_usuario_radicador(HttpContext.Current.Session.Item("RA_ID_USUARIO"),
                                                                                                                    matri_platilla)
                    If Result <> "YES" Then
                        Inicializa_menu_principal = "Imposible cargar matriz de plantillas " & Result
                        Exit Function
                    End If
                    Dim matri_plantillas_validacion() As String = Nothing
                    Result = refclassradicado.Lista_plantillas_validacion_externa(matri_plantillas_validacion)
                    If Result <> "YES" Then
                        Inicializa_menu_principal = "Imposible cargar validacion externa " & Result
                        Exit Function
                    End If
                    Result = Refclas.Retorna_tipo_Impresion(HttpContext.Current.Session.Item("RA_ID_USUARIO"))
                    If Result <> "YES" Then
                        Inicializa_menu_principal = "Imposible cargar interface plantilla " & Result
                        Exit Function
                    End If
                End If
            End If
            Inicializa_menu_principal = "YES"
        Catch ex As Exception
            Inicializa_menu_principal = "Inconsistencia general funcion Inicializa_menu_principal " & ex.Message
        End Try
    End Function
    Function selecciona_treview_aplicacion_web_gestion(ByRef page1 As Page,
                                                       ByRef ref_update As UpdatePanel,
                                                       ByRef ref_label As Label,
                                                       ByVal texto_busqueda As String) As String
        Dim Result As String = ""
        Dim Ref As New ClassGestorSesion
        Dim reclas As New Classscrripjava
        Dim refclasiniciowf As New InicioWorkflow
        Dim refclassradicado As New ClassRadicador
        Dim refclas_inicio_publico As New Classincipublico
        Dim ref_Class_perfilar_usuario_radicador As New Class_perfilar_usuario_radicador
        Try
            Dim ref_selecion_label As Label = page1.FindControl("Label_estado")
            Dim ref_TreeView1 As TreeView = ref_update.FindControl("TreeView1")
            If ref_TreeView1 Is Nothing Then
                selecciona_treview_aplicacion_web_gestion = "Imposible encontrar control TreeView1 "
                ref_label.Text = ref_label.Text & "|Imposible encontrar control TreeView1 "
                Exit Function
            End If
            Dim ref_ifrm_ds As Object = ref_update.FindControl("ifrm_ds_")
            If ref_ifrm_ds Is Nothing Then
                selecciona_treview_aplicacion_web_gestion = "Imposible encontrar control ifrm_ds_ "
                ref_label.Text = ref_label.Text & "|Imposible encontrar control ifrm_ds_ "
                Exit Function
            End If
            Dim ref_HiddenHeigth As Object = ref_update.FindControl("HiddenHeigth")
            If ref_HiddenHeigth Is Nothing Then
                selecciona_treview_aplicacion_web_gestion = "Imposible encontrar control HiddenHeigth "
                ref_label.Text = ref_label.Text & "|Imposible encontrar control HiddenHeigth "
                Exit Function
            End If
            If Not page1.IsPostBack Then
                ref_TreeView1.Nodes.Clear()
                If HttpContext.Current.Session("TIPOMODULO") = "PUBLICO" Then
                    ref_label.Text = ref_label.Text & "Usuario público "
                    Result = refclas_inicio_publico.Listar_Treeview_Publico(ref_TreeView1)
                    If Result <> "YES" Then
                        selecciona_treview_aplicacion_web_gestion = "Imposible cargar opciones publicas " & Result
                        ref_label.Text = ref_label.Text & "|Imposible cargar opciones publicas " & Result
                        Exit Function
                    End If
                End If
                If HttpContext.Current.Session("TIPOMODULO") = "WORKFLOW DOCUMENTAL" Then
                    ref_label.Text = ref_label.Text & "Modulo Workflow usuario " & HttpContext.Current.Session("Login_Usuario_Workfow")
                    Result = refclasiniciowf.Listar_Treeview_Workflow(ref_TreeView1)
                    If Result <> "YES" Then
                        selecciona_treview_aplicacion_web_gestion = "Imposible cargar opciones workflow " & Result
                        ref_label.Text = ref_label.Text & "|Imposible cargar opciones workflow " & Result
                        Exit Function
                    End If
                End If

                If HttpContext.Current.Session("TIPOMODULO") = "RADICACION DOCUMENTAL" Then
                    ref_label.Text = ref_label.Text & "Modulo Radicación usuario " & HttpContext.Current.Session.Item("RA_LOGIN_USER")
                    Dim Refclas As New ClassInicioRadicador
                    Result = ref_Class_perfilar_usuario_radicador.AsignaPermisosSesionUsuarioRadicador(HttpContext.Current.Session.Item("RA_ID_USUARIO"))
                    If Result <> "YES" Then
                        selecciona_treview_aplicacion_web_gestion = "Imposible cargar permisos radicacion " & Result
                        ref_label.Text = ref_label.Text & "|Imposible cargar permisos radicacion " & Result
                        Exit Function
                    End If
                    Dim matri_platilla() As plantillas
                    Erase matri_platilla
                    Dim ref_Class_permisos_plantilla As New Class_permisos_plantilla
                    Result = ref_Class_permisos_plantilla.Solicita_plantillas_radicado_permitidas_usuario_radicador(HttpContext.Current.Session.Item("RA_ID_USUARIO"),
                                                                                                                    matri_platilla)
                    If Result <> "YES" Then
                        selecciona_treview_aplicacion_web_gestion = "Imposible cargar matriz de plantillas " & Result
                        ref_label.Text = ref_label.Text & "|Imposible cargar matriz de plantillas " & Result
                        Exit Function
                    End If
                    Dim matri_plantillas_validacion() As String = Nothing
                    Result = refclassradicado.Lista_plantillas_validacion_externa(matri_plantillas_validacion)
                    If Result <> "YES" Then
                        selecciona_treview_aplicacion_web_gestion = "Imposible cargar validacion externa " & Result
                        ref_label.Text = ref_label.Text & "|Imposible cargar plantillas validacion externa " & Result
                        Exit Function
                    End If
                    Result = Refclas.Tri_View(ref_TreeView1,
                                              matri_platilla,
                                              matri_plantillas_validacion)
                    If Result <> "YES" Then
                        selecciona_treview_aplicacion_web_gestion = "Imposible cargar interface plantilla " & Result
                        ref_label.Text = ref_label.Text & "|Imposible cargar interface plantilla " & Result
                        Exit Function
                    End If
                    Result = Refclas.Retorna_tipo_Impresion(HttpContext.Current.Session.Item("RA_ID_USUARIO"))
                    If Result <> "YES" Then
                        selecciona_treview_aplicacion_web_gestion = "Imposible retornar tipo impresón " & Result
                        ref_label.Text = ref_label.Text & "|Imposible retornar tipo impresón " & Result
                        Exit Function
                    End If
                End If
                If HttpContext.Current.Session("TIPOMODULO") = "DOCUARCHI CONTENEDOR" Then
                    ref_label.Text = ref_label.Text & "Modulo Docuarchi" & HttpContext.Current.Session.Item("DA_Login_Usuario")
                    Dim refclas_da As New ClassDaIncioDocuarchi
                    Result = refclas_da.Listar_Treeview_docuarchi(ref_TreeView1)
                    If Result <> "YES" Then
                        selecciona_treview_aplicacion_web_gestion = "Imposible cargar opciones Docuarchi.net " & Result
                        ref_label.Text = ref_label.Text & "|Imposible cargar opciones Docuarchi.net " & Result
                        Exit Function
                    End If
                End If
                '------------------------------------------------
                'Lista modulo gestión documental
                '------------------------------------------------
                If HttpContext.Current.Session("TIPOMODULO") = "GESTOR DOCUMENTAL" Then
                    ref_label.Text = ref_label.Text & "Modulo Gestor usuario " & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION")
                    If HttpContext.Current.Session.Item("Id_Usuario_Workflow") <> 0 Then
                        Result = refclasiniciowf.Listar_Treeview_Workflow(ref_TreeView1)
                        If Result <> "YES" Then
                            selecciona_treview_aplicacion_web_gestion = "Imposible cargar opciones workflow " & Result
                            ref_label.Text = ref_label.Text & "|Imposible cargar opciones workflow " & Result
                            Exit Function
                        End If
                    End If
                    If HttpContext.Current.Session.Item("ID_USUARIO_DOCUARCHI") <> 0 Then
                        Dim refclas_da As New ClassDaIncioDocuarchi
                        Result = refclas_da.Listar_Treeview_docuarchi(ref_TreeView1, 1)
                        If Result <> "YES" Then
                            selecciona_treview_aplicacion_web_gestion = "Imposible cargar opciones Docuarchi.net " & Result
                            ref_label.Text = ref_label.Text & "|Imposible cargar opciones Docuarchi.net " & Result
                            Exit Function
                        End If
                    End If
                    If HttpContext.Current.Session.Item("RA_ID_USUARIO") <> 0 Then
                        Dim Refclas As New ClassInicioRadicador
                        Result = ref_Class_perfilar_usuario_radicador.AsignaPermisosSesionUsuarioRadicador(HttpContext.Current.Session.Item("RA_ID_USUARIO"))
                        If Result <> "YES" Then
                            selecciona_treview_aplicacion_web_gestion = "Imposible cargar permisos radicacion " & Result
                            ref_label.Text = ref_label.Text & "|Imposible cargar permisos radicacion " & Result
                            Exit Function
                        End If
                        Dim matri_platilla() As plantillas
                        Erase matri_platilla
                        Dim ref_Class_permisos_plantilla As New Class_permisos_plantilla
                        Result = ref_Class_permisos_plantilla.Solicita_plantillas_radicado_permitidas_usuario_radicador(HttpContext.Current.Session.Item("RA_ID_USUARIO"),
                                                                                                                        matri_platilla)
                        If Result <> "YES" Then
                            selecciona_treview_aplicacion_web_gestion = "Imposible cargar matriz de plantillas " & Result
                            ref_label.Text = ref_label.Text & "|Imposible cargar matriz de plantillas " & Result
                            Exit Function
                        End If
                        Dim matri_plantillas_validacion() As String = Nothing
                        Result = refclassradicado.Lista_plantillas_validacion_externa(matri_plantillas_validacion)
                        If Result <> "YES" Then
                            selecciona_treview_aplicacion_web_gestion = "Imposible cargar validacion externa " & Result
                            ref_label.Text = ref_label.Text & "|Imposible cargar plantillas validacion externa " & Result
                            Exit Function
                        End If
                        Result = Refclas.Tri_View(ref_TreeView1,
                                                  matri_platilla,
                                                  matri_plantillas_validacion,
                                                  1)
                        If Result <> "YES" Then
                            selecciona_treview_aplicacion_web_gestion = "Imposible cargar interface plantilla " & Result
                            ref_label.Text = ref_label.Text & "|Imposible cargar interface plantilla " & Result
                            Exit Function
                        End If
                        Result = Refclas.Retorna_tipo_Impresion(HttpContext.Current.Session.Item("RA_ID_USUARIO"))
                        If Result <> "YES" Then
                            selecciona_treview_aplicacion_web_gestion = "Imposible cargar interface plantilla " & Result
                            ref_label.Text = ref_label.Text & "|Imposible cargar interface plantilla " & Result
                            Exit Function
                        End If
                    End If
                    '--------------------------------------------------
                    'Lista modulo gestion documental
                    '--------------------------------------------------
                    Dim refclas_inicio_gestion As New ClassGagestorInicio
                    Result = refclas_inicio_gestion.Tri_View_gestion(ref_TreeView1)
                    If Result <> "YES" Then
                        selecciona_treview_aplicacion_web_gestion = "Imposible cargar interface gestion " & Result
                        ref_label.Text = ref_label.Text & "|Imposible cargar interface gestion " & Result
                        Exit Function
                    End If
                End If
            End If

            If page1.IsPostBack Then
                Dim Matri_Nodo() As String
                Erase Matri_Nodo
                Dim Datos_Nodo As String = ""
                'consulta dato de nodo seleccionado
                If texto_busqueda = "" Then
                    Result = Ref.NodoChild_Selecionado(ref_TreeView1, Datos_Nodo)
                    If Result <> "YES" Then
                        selecciona_treview_aplicacion_web_gestion = Result
                        Exit Function
                    Else
                        Matri_Nodo = Split(Datos_Nodo, "|")
                    End If
                Else
                    Result = Ref.NodoChild_Selecionado_busqueda(ref_TreeView1, Datos_Nodo, texto_busqueda)
                    If Result <> "YES" Then
                        selecciona_treview_aplicacion_web_gestion = Result
                        Exit Function
                    Else
                        Matri_Nodo = Split(Datos_Nodo, "|")
                    End If
                End If

                Result = ""
                If Not Matri_Nodo Is Nothing Then
                    If HttpContext.Current.Session.Item("TIPOMODULO") = "WORKFLOW DOCUMENTAL" Then
                        If Matri_Nodo(0) = "WF-CL-01" Then
                            ref_ifrm_ds.Attributes("SRC") = "../workflow/Webworkflow.aspx"
                            ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                            ref_update.Update()
                        End If
                        If Matri_Nodo(0) = "CR-GT-01" Then
                            ref_ifrm_ds.Attributes("SRC") = "../Gestion_correspondencia/WebForm_interface_gestion_tramite.aspx"
                            ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                            ref_update.Update()
                        End If
                        If Matri_Nodo(0) = "CR-HR-02" Then
                            ref_ifrm_ds.Attributes("SRC") = "../Gestion/FormGaHistorialRespuesta.aspx"
                            ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                            ref_update.Update()
                        End If
                        If Matri_Nodo(0) = "WF-SPA-06" Then
                            ref_ifrm_ds.Attributes("SRC") = "../radicador/WebFormListaSolicitudesPorMiAprobacion.aspx"
                            ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                            ref_update.Update()
                        End If
                        If Matri_Nodo(0) = "WF-DR-06" Then
                            HttpContext.Current.Session.Item("DR_RUTASELECCION_DIAGRAMA") = ""
                            ref_ifrm_ds.Attributes("SRC") = "../workflow/WebWorkflowDigramaRuta.aspx"
                            ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                            ref_update.Update()
                        End If
                        If Matri_Nodo(0) = "WF-DF-07" Then
                            ref_ifrm_ds.Attributes("SRC") = "../workflow/WebFormDiagramadorFlujoTrabajo.aspx"
                            ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                            ref_update.Update()
                        End If

                        If Matri_Nodo(0) = "WF-CMPASW-08" Then
                            ref_ifrm_ds.Attributes("SRC") = "../workflow/WebFormCambiarPasword.aspx"
                            ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                            ref_update.Update()
                        End If
                        If Matri_Nodo(0) = "WF-TR-02" Then
                            ref_ifrm_ds.Attributes("SRC") = "../workflow/WebFormConsultaTareasWorkflow.aspx"
                            ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                            ref_update.Update()
                        End If
                    End If
                    If HttpContext.Current.Session.Item("TIPOMODULO") = "PUBLICO" Then
                        If Matri_Nodo(0) = "OP-CR-01" Then
                            ref_ifrm_ds.Attributes("SRC") = "../Publico/WebFormConsultaRadicadoPublico.aspx"
                            ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                            ref_update.Update()
                        End If
                        'OP-CP-02
                        If Matri_Nodo(0) = "OP-CP-02" Then
                            ref_ifrm_ds.Attributes("SRC") = "../Publico/WebFormConsultaPublico.aspx"
                            ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                            ref_update.Update()
                        End If
                        If Matri_Nodo(0) = "OP-CEO-02" Then
                            ref_ifrm_ds.Attributes("SRC") = "../Publico/WebFormConsultaOficiales.aspx"
                            ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                            ref_update.Update()
                        End If
                    End If
                    If HttpContext.Current.Session.Item("TIPOMODULO") = "RADICACION DOCUMENTAL" Then
                        HttpContext.Current.Session.Item("RA_MODULO_SELECCIONADO") = Datos_Nodo
                        If Matri_Nodo.Length > 2 Then
                            If Matri_Nodo(0) = "RADICACION" Then
                                If Matri_Nodo(2) = "RADICACION ENTRANTE" Then
                                    ref_ifrm_ds.Attributes("SRC") = "../radicador/WebFormRadicacionEntrante.aspx"
                                    ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                                    ref_update.Update()
                                End If
                                If Matri_Nodo(2) = "RADICACION SALIENTE" Then
                                    ref_ifrm_ds.Attributes("SRC") = "../radicador/WebFormRadicacionEntrante.aspx"
                                    ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                                    ref_update.Update()
                                End If
                                If Matri_Nodo(2) = "RADICACION GUIA" Then
                                    ref_ifrm_ds.Attributes("SRC") = "../radicador/WebFormRadicacion.aspx"
                                    ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                                    ref_update.Update()
                                End If
                            End If

                            If Matri_Nodo(0) = "CONSULTA" Then
                                HttpContext.Current.Session.Item("RA_TIPO_CONSULTA_RADICADO") = "NORMAL"
                                ref_ifrm_ds.Attributes("SRC") = "../radicador/WebFormConsultaRadicacion.aspx"
                                ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                                'System.Threading.Thread.Sleep(2000)
                                ref_update.Update()

                            End If
                            If Matri_Nodo(0) = "ENVIOS" Then
                                If Matri_Nodo(1) = "PORENVIAR" Then
                                    ref_ifrm_ds.Attributes("SRC") = "../radicador/WebFormCorespondenciaporenviar.aspx"
                                    ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                                    'System.Threading.Thread.Sleep(2000)
                                    ref_update.Update()
                                End If
                                If Matri_Nodo(1) = "PORARCHIVAR" Then
                                    ref_ifrm_ds.Attributes("SRC") = "../radicador/WebFormArchivaenviocorrespo.aspx"
                                    ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                                    'System.Threading.Thread.Sleep(2000)
                                    ref_update.Update()
                                End If

                            End If
                            If Matri_Nodo(0) = "REMISION" Then
                                If Matri_Nodo(1) = "REMISIONCORRESPONDENCIA" Then
                                    ref_ifrm_ds.Attributes("SRC") = "../radicador/WebFormRemisionCorrespondencia.aspx"
                                    ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                                    'System.Threading.Thread.Sleep(2000)
                                    ref_update.Update()
                                End If
                            End If
                            If Matri_Nodo(0) = "USUARIO" Then
                                If Matri_Nodo(1) = "CONTRASEÑA" Then
                                    ref_ifrm_ds.Attributes("SRC") = "../radicador/WebFormPaswordRadicacion.aspx"
                                    ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                                    'System.Threading.Thread.Sleep(2000)
                                    ref_update.Update()
                                End If
                            End If
                            If Matri_Nodo(0) = "GUIAS" Then
                                If Matri_Nodo(1) = "CONSULTAGUIA" Then
                                    ref_ifrm_ds.Attributes("SRC") = "../radicador/WebFormRaConsultaGuias.aspx"
                                    ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                                    'System.Threading.Thread.Sleep(2000)
                                    ref_update.Update()
                                End If
                            End If
                            If Matri_Nodo(0) = "GUIAS" Then
                                If Matri_Nodo(1) = "GESTIONARGUIA" Then
                                    ref_ifrm_ds.Attributes("SRC") = "../radicador/WebFormRaGestionarGuias.aspx"
                                    ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                                    'System.Threading.Thread.Sleep(2000)
                                    ref_update.Update()
                                End If
                            End If
                            If Matri_Nodo(0) = "GUIAS" Then
                                If Matri_Nodo(1) = "REGISTRARGUIA" Then
                                    ref_ifrm_ds.Attributes("SRC") = "../radicador/WebFormRaRegistrarguia.aspx"
                                    ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                                    'System.Threading.Thread.Sleep(2000)
                                    ref_update.Update()
                                End If
                            End If
                            If Matri_Nodo(0) = "PLANTILLA" Then
                                'If Matri_Nodo(1) = "REGISTRARGUIA" Then
                                ref_ifrm_ds.Attributes("SRC") = "../radicador/WebFormGestionPlantillasvalidacion.aspx"
                                ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                                Dim refclasrad As New ClassRadicador
                                Dim id_script As Integer = 0
                                Result = refclasrad.Retorna_id_scrip_plantilla_validacion(Matri_Nodo(1), id_script)
                                If Result <> "YES" Then

                                Else
                                    HttpContext.Current.Session.Item("SESIONITERCAMBIOPLANTILLAVALIDACION") = id_script
                                    ref_update.Update()
                                End If

                                'End If
                            End If
                        End If
                    End If
                    If HttpContext.Current.Session("TIPOMODULO") = "DOCUARCHI CONTENEDOR" Then
                        If Matri_Nodo(0) = "DA-CLI-01" Then
                            ref_ifrm_ds.Attributes("SRC") = "../Docuarchi/WebFormDaPrincipal.aspx"
                            ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                            ref_update.Update()
                        End If
                        If Matri_Nodo(0) = "DA-CLI-02" Then
                            ref_ifrm_ds.Attributes("SRC") = "../Docuarchi/WebFormDaCambiarPaswordDa.aspx"
                            ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                            ref_update.Update()
                        End If
                    End If
                    If HttpContext.Current.Session.Item("TIPOMODULO") = "GESTOR DOCUMENTAL" And Matri_Nodo.Length > 1 Then
                        If Matri_Nodo(0) = "WF-CL-01" Then
                            ref_ifrm_ds.Attributes("SRC") = "../workflow/Webworkflow.aspx"
                            ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                            ref_update.Update()
                        End If
                        If Matri_Nodo(0) = "CR-GT-01" Then
                            ref_ifrm_ds.Attributes("SRC") = "../Gestion_correspondencia/WebForm_interface_gestion_tramite.aspx"
                            ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                            ref_update.Update()
                        End If
                        If Matri_Nodo(0) = "CR-HR-02" Then
                            ref_ifrm_ds.Attributes("SRC") = "../Gestion/FormGaHistorialRespuesta.aspx"
                            ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                            ref_update.Update()
                        End If
                        If Matri_Nodo(0) = "DA-CLI-01" Then
                            ref_ifrm_ds.Attributes("SRC") = "../Docuarchi/WebFormDaPrincipal.aspx"
                            ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                            ref_update.Update()
                        End If
                        If Matri_Nodo(0) = "DA-CLI-02" Then
                            ref_ifrm_ds.Attributes("SRC") = "../Docuarchi/WebFormDaCambiarPaswordDa.aspx"
                            ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                            ref_update.Update()
                        End If
                        If Matri_Nodo(0) = "WF-SPA-06" Then
                            ref_ifrm_ds.attributes("src") = "../radicador/webformlistasolicitudespormiaprobacion.aspx"
                            ref_ifrm_ds.style.add("height", ref_HiddenHeigth.value & "px")
                            ref_update.Update()
                        End If
                        If Matri_Nodo(0) = "WF-DRW-06" Then
                            HttpContext.Current.Session.Item("DR_RUTASELECCION_DIAGRAMA") = ""
                            ref_ifrm_ds.Attributes("SRC") = "../workflow/WebWorkflowDigramaRuta.aspx"
                            ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                            ref_update.Update()
                        End If
                        If Matri_Nodo(0) = "WF-DF-07" Then
                            ref_ifrm_ds.Attributes("SRC") = "../workflow/WebFormDiagramadorFlujoTrabajo.aspx"
                            ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                            ref_update.Update()
                        End If
                        If Matri_Nodo(0) = "WF-TR-02" Then
                            ref_ifrm_ds.Attributes("SRC") = "../workflow/WebFormConsultaTareasWorkflow.aspx"
                            ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                            ref_update.Update()
                        End If
                        If Matri_Nodo.Length = 3 Then
                            If Matri_Nodo(0) = "GESTION" Then
                                '--------------------------------
                                'Registra expediente
                                '-------------------------------
                                If Matri_Nodo(1) = "REGISTRAEXP" Then
                                    ref_ifrm_ds.Attributes("SRC") = "../Gestion/FormGaAgregarExpediente.aspx"
                                    ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                                    ref_update.Update()
                                End If
                                '-------------------------------
                                'Consulta expediente 
                                '-------------------------------
                                If Matri_Nodo(1) = "GESTIONEXP" Then
                                    ref_ifrm_ds.Attributes("SRC") = "../Gestion/WebFormGaGestionExpediente.aspx"
                                    ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                                    ref_update.Update()
                                End If
                                '-------------------------------
                                'Consulta toponimica
                                '-------------------------------
                                If Matri_Nodo(1) = "TOPONIMICA" Then
                                    ref_ifrm_ds.Attributes("SRC") = "../Gestion/WebFormGatoponimica.aspx"
                                    ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                                    ref_update.Update()
                                End If
                                '-------------------------------
                                'Cambia loguin usuario gestion
                                '-------------------------------
                                If Matri_Nodo(1) = "CONTRASEÑA" Then
                                    ref_ifrm_ds.Attributes("SRC") = "../Gestion/WebFormPaswordGestion.aspx"
                                    ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                                    ref_update.Update()
                                End If
                                '--------------------------------
                                'Consulta unidad de conservacion
                                '--------------------------------
                                If Matri_Nodo(1) = "CONSERVACION" Then
                                    ref_ifrm_ds.Attributes("SRC") = "../Gestion/WebFormGaGestionUnidadConservacion.aspx"
                                    ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                                    ref_update.Update()
                                End If
                                '-------------------------------
                                'Administración clasificación
                                '-------------------------------
                                If Matri_Nodo(1) = "ORGANIZACIONDOCUMENTAL_ADMINISTRACION" Then
                                    ref_ifrm_ds.Attributes("SRC") = "../Gestion/WebFormGaadmonclasificacion.aspx"
                                    ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                                    ref_update.Update()
                                End If
                                '--------------------------------
                                'Consulta clasficiación
                                '--------------------------------
                                If Matri_Nodo(1) = "ORGANIZACIONDOCUMENTAL_CONSULTA" Then
                                    ref_ifrm_ds.Attributes("SRC") = "../Gestion/WebFormGaconsultaclasificacion.aspx"
                                    ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                                    ref_update.Update()
                                End If
                                '---------------------------------
                                'Consulta tabla de retencion
                                '--------------------------------
                                If Matri_Nodo(1) = "TABLARETENCIONDOCUMENTAL_CONSULTA" Then
                                    ref_ifrm_ds.Attributes("SRC") = "../Gestion/WebFormGaAplicarTrd.aspx"
                                    ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                                    ref_update.Update()
                                End If
                                '-----------------------------------
                                'Gestión organigrama 
                                '-----------------------------------
                                If Matri_Nodo(1) = "GESTIONORGANIGRAMA_TRD" Then
                                    ref_ifrm_ds.Attributes("SRC") = "../Gestion/WebFormGestionOrganigrama.aspx"
                                    ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                                    ref_update.Update()
                                End If
                                '-----------------------------------
                                'Gestión instrumentos archivisiticos
                                '-----------------------------------
                                If Matri_Nodo(1) = "GESTIONINSTRUMENTOS_ARCHIVI" Then
                                    ref_ifrm_ds.Attributes("SRC") = "../Gestion/WebFormGaGestionInstrumentos.aspx"
                                    ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                                    ref_update.Update()
                                End If
                                '----------------------------------
                                'Produccion documental
                                '----------------------------------
                                If Matri_Nodo(1) = "PRODUCCIONDOCUMENTAL_DOCUMENTOS" Then
                                    ref_ifrm_ds.Attributes("SRC") = "../Gestion/WebFormProducionDocumental.aspx"
                                    ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                                    ref_update.Update()
                                End If
                                '------------------------------------------
                                'Lista documentos compatidos para revisión
                                '------------------------------------------
                                If Matri_Nodo(1) = "COMPARTIDO_PENDIENTE_REVISION" Then
                                    Dim obhiden As Object = ref_update.FindControl("Hidden_resultado_compartido_por_revision")
                                    obhiden.value = HttpContext.Current.Session.Item("GA_STRU_DOCUMENTO_PENDIENTE_REVISION")
                                    ref_ifrm_ds.Attributes("SRC") = "../Gestion/WebFormDocumentosCompartidosRevision.aspx"
                                    ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                                    ref_update.Update()
                                End If
                                '-----------------------------------------
                                'Lista mis documentos compartidos
                                '-----------------------------------------
                                If Matri_Nodo(1) = "COMPARTIDO_OTROS_USUARIOS" Then
                                    ref_ifrm_ds.Attributes("SRC") = "../Gestion/WebFormDocumentoCompartidoOtrosUsuarios.aspx"
                                    ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                                    ref_update.Update()
                                End If
                            End If
                        End If
                        If Matri_Nodo.Length = 6 Then
                            If Matri_Nodo(0) = "CONSULTAGESTION" Then
                                Dim Replace As String = Datos_Nodo.Replace("CONSULTAGESTION", "CONSULTA")
                                HttpContext.Current.Session.Item("RA_MODULO_SELECCIONADO") = Replace
                                Datos_Nodo = Replace
                                HttpContext.Current.Session.Item("RA_TIPO_CONSULTA_RADICADO") = "PRODUCCION"
                                ref_ifrm_ds.Attributes("SRC") = "../radicador/WebFormConsultaRadicacion.aspx"
                                ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                                ref_update.Update()
                            End If
                            If Matri_Nodo(0) = "GESTIONPENDIENTES" Then
                                Dim Replace As String = Datos_Nodo.Replace("GESTIONPENDIENTES", "CONSULTA")
                                HttpContext.Current.Session.Item("RA_MODULO_SELECCIONADO") = Replace
                                Datos_Nodo = Replace
                                ref_ifrm_ds.Attributes("SRC") = "../radicador/WebFormRadicadosPendientesPorEnviar.aspx"
                                ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                                ref_update.Update()
                            End If
                        End If
                        If Matri_Nodo.Length > 2 Then
                            HttpContext.Current.Session.Item("RA_MODULO_SELECCIONADO") = Datos_Nodo
                            If Matri_Nodo(0) = "RADICACION" Then
                                If Matri_Nodo(2) = "RADICACION ENTRANTE" Then
                                    ref_ifrm_ds.Attributes("SRC") = "../radicador/WebFormRadicacionEntrante.aspx"
                                    ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                                    ref_update.Update()
                                End If
                                If Matri_Nodo(2) = "RADICACION SALIENTE" Then
                                    ref_ifrm_ds.Attributes("SRC") = "../radicador/WebFormRadicacionEntrante.aspx"
                                    ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                                    ref_update.Update()
                                End If
                                If Matri_Nodo(2) = "RADICACION GUIA" Then
                                    HttpContext.Current.Session.Item("RA_MODULO_SELECCIONADO") = Datos_Nodo
                                    ref_ifrm_ds.Attributes("SRC") = "../radicador/WebFormRadicacion.aspx"
                                    ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                                    ref_update.Update()
                                End If
                            End If
                            If Matri_Nodo(0) = "CONSULTA" Then
                                HttpContext.Current.Session.Item("RA_TIPO_CONSULTA_RADICADO") = "NORMAL"
                                ref_ifrm_ds.Attributes("SRC") = "../radicador/WebFormConsultaRadicacion.aspx"
                                ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                                ref_update.Update()
                            End If
                            If Matri_Nodo(0) = "ENVIOS" Then
                                If Matri_Nodo(1) = "PORENVIAR" Then
                                    ref_ifrm_ds.Attributes("SRC") = "../radicador/WebFormCorespondenciaporenviar.aspx"
                                    ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                                    'System.Threading.Thread.Sleep(2000)
                                    ref_update.Update()
                                End If
                                If Matri_Nodo(1) = "PORARCHIVAR" Then
                                    ref_ifrm_ds.Attributes("SRC") = "../radicador/WebFormArchivaenviocorrespo.aspx"
                                    ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                                    'System.Threading.Thread.Sleep(2000)
                                    ref_update.Update()
                                End If

                            End If
                            If Matri_Nodo(0) = "REMISION" Then
                                If Matri_Nodo(1) = "REMISIONCORRESPONDENCIA" Then
                                    ref_ifrm_ds.Attributes("SRC") = "../radicador/WebFormRemisionCorrespondencia.aspx"
                                    ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                                    'System.Threading.Thread.Sleep(2000)
                                    ref_update.Update()
                                End If
                            End If
                            If Matri_Nodo(0) = "USUARIO" Then
                                If Matri_Nodo(1) = "CONTRASEÑA" Then
                                    ref_ifrm_ds.Attributes("SRC") = "../radicador/WebFormPaswordRadicacion.aspx"
                                    ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                                    'System.Threading.Thread.Sleep(2000)
                                    ref_update.Update()
                                End If
                            End If
                            If Matri_Nodo(0) = "GUIAS" Then
                                If Matri_Nodo(1) = "CONSULTAGUIA" Then
                                    ref_ifrm_ds.Attributes("SRC") = "../radicador/WebFormRaConsultaGuias.aspx"
                                    ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                                    'System.Threading.Thread.Sleep(2000)
                                    ref_update.Update()
                                End If
                            End If
                            If Matri_Nodo(1) = "GESTIONARGUIA" Then
                                ref_ifrm_ds.Attributes("SRC") = "../radicador/WebFormRaGestionarGuias.aspx"
                                ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                                'System.Threading.Thread.Sleep(2000)
                                ref_update.Update()
                            End If
                            If Matri_Nodo(0) = "GUIAS" Then
                                If Matri_Nodo(1) = "REGISTRARGUIA" Then
                                    ref_ifrm_ds.Attributes("SRC") = "../radicador/WebFormRaRegistrarguia.aspx"
                                    ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                                    'System.Threading.Thread.Sleep(2000)
                                    ref_update.Update()
                                End If
                            End If
                            If Matri_Nodo(0) = "PLANTILLA" Then
                                'If Matri_Nodo(1) = "REGISTRARGUIA" Then
                                ref_ifrm_ds.Attributes("SRC") = "../radicador/WebFormGestionPlantillasvalidacion.aspx"
                                ref_ifrm_ds.Style.Add("Height", ref_HiddenHeigth.Value & "px")
                                Dim refclasrad As New ClassRadicador
                                Dim id_script As Integer = 0
                                Result = refclasrad.Retorna_id_scrip_plantilla_validacion(Matri_Nodo(1), id_script)
                                If Result <> "YES" Then

                                Else
                                    HttpContext.Current.Session.Item("SESIONITERCAMBIOPLANTILLAVALIDACION") = id_script
                                    ref_update.Update()
                                End If

                            End If
                        End If
                    End If
                End If
            End If
            selecciona_treview_aplicacion_web_gestion = "YES"
        Catch ex As Exception
            selecciona_treview_aplicacion_web_gestion = "Inconsistencia función selecciona_treview_aplicacion_web_gestion " & ex.Message
        End Try
    End Function
    Function Solicita_items_modulos_workflow(ByVal tipo_modulo As String,
                                             ByRef stru_inicio() As stru_inicio_menu) As String
        Try
            Dim leng_stru As Integer = 0
            If stru_inicio Is Nothing Then
                ReDim Preserve stru_inicio(leng_stru)
            Else
                leng_stru = leng_stru + 1
                ReDim Preserve stru_inicio(leng_stru)
            End If
            stru_inicio(leng_stru).Text_node = "Workflow "
            stru_inicio(leng_stru).value_node = "WF-WF-01"
            stru_inicio(leng_stru).value_card_conten = ""
            stru_inicio(leng_stru).url_node = ""
            If tipo_modulo = "WORKFLOW DOCUMENTAL" Or tipo_modulo = "GESTOR DOCUMENTAL" Then
                stru_inicio(leng_stru).visible_node = 1
            Else
                stru_inicio(leng_stru).visible_node = 0
            End If
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            stru_inicio(leng_stru).Text_node = "Flujos y tareas"
            stru_inicio(leng_stru).value_node = "WF-CL-01"
            stru_inicio(leng_stru).value_card = ""
            stru_inicio(leng_stru).value_card_conten = ""
            stru_inicio(leng_stru).url_node = "../workflow/Webworkflow.aspx"
            stru_inicio(leng_stru).visible_node = 1
            stru_inicio(leng_stru).tipo_modulo = "WF"
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            stru_inicio(leng_stru).Text_node = "Flujos y tareas"
            stru_inicio(leng_stru).value_node = "WF-CL-01_"
            stru_inicio(leng_stru).value_card = "WF-CL-01_card_boton"
            stru_inicio(leng_stru).value_card_conten = "WF-CL-01_card_content"
            stru_inicio(leng_stru).tipo_modulo = "WF"
            stru_inicio(leng_stru).url_node = "../workflow/Webworkflow.aspx"
            stru_inicio(leng_stru).visible_node = 1
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            stru_inicio(leng_stru).Text_node = "Consulta de flujos y tareas"
            stru_inicio(leng_stru).value_node = "WF-TR-02"
            stru_inicio(leng_stru).value_card = "WF-TR-02_card_boton"
            stru_inicio(leng_stru).value_card_conten = ""
            stru_inicio(leng_stru).tipo_modulo = "WF"
            stru_inicio(leng_stru).value_card_conten = ""
            stru_inicio(leng_stru).url_node = "../workflow/WebFormConsultaTareasWorkflow.aspx"
            stru_inicio(leng_stru).visible_node = 1
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            stru_inicio(leng_stru).Text_node = "Reportes de flujos y tareas"
            stru_inicio(leng_stru).value_node = "WF-RW-03"
            stru_inicio(leng_stru).value_card = "WF-RW-03_card_boton"
            stru_inicio(leng_stru).value_card_conten = ""
            stru_inicio(leng_stru).tipo_modulo = "WF"
            stru_inicio(leng_stru).url_externa = "YES"
            stru_inicio(leng_stru).url_node = "../workflow/WebFormReportesWorkflow.aspx"
            stru_inicio(leng_stru).url_content = "../Workflow/WebFormContenedorPageWF.aspx"
            stru_inicio(leng_stru).value_content = "REPORTES WORKFLOW"
            stru_inicio(leng_stru).visible_node = 1
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            stru_inicio(leng_stru).Text_node = "Consultar documentos"
            stru_inicio(leng_stru).value_node = "WF-CD-05"
            stru_inicio(leng_stru).value_card = "WF-CD-05_card_boton"
            stru_inicio(leng_stru).value_card_conten = ""
            stru_inicio(leng_stru).tipo_modulo = "WF"
            stru_inicio(leng_stru).url_externa = "YES"
            stru_inicio(leng_stru).url_node = "Docuarchi/WebFormDaPrincipal.aspx"
            stru_inicio(leng_stru).url_content = "../Workflow/WebFormContenedorPageWF.aspx"
            stru_inicio(leng_stru).value_content = "CONSULTA DOCUMENTOS"
            stru_inicio(leng_stru).visible_node = 1
            '-------------------------------------------------
            'Genera interface workflow ruta y diagramador
            '-------------------------------------------------
            Dim Result As String = ""
            Dim refclas_ruta As New Class_worflow_rutas
            Result = refclas_ruta.SolicitaperfilDiagramadorUsuarioWorkflow(HttpContext.Current.Session.Item("Id_Usuario_Workflow"),
                                                                              HttpContext.Current.Session.Item("WF_IMPORTADOR_RUTA"),
                                                                              HttpContext.Current.Session.Item("WF_CREA_FLUJO_TRABAJO"),
                                                                              HttpContext.Current.Session.Item("WF_AGREGA_ACTIVIDAD"),
                                                                              HttpContext.Current.Session.Item("WF_CONECTA_ACTIVIDAD"),
                                                                              HttpContext.Current.Session.Item("WF_ELIMINA_ACTIVIDAD"),
                                                                              HttpContext.Current.Session.Item("WF_ELIMINA_CONECTOR"),
                                                                              HttpContext.Current.Session.Item("WF_DIAGRAMADOR"),
                                                                              HttpContext.Current.Session.Item("WF_MIGRACION"))
            If Result <> "YES" Then
                Solicita_items_modulos_workflow = Result
                Exit Function
            End If
            Dim estado_visible As Integer = 0
            If HttpContext.Current.Session.Item("WF_DIAGRAMADOR") <> 0 Then
                estado_visible = 1
            Else
                estado_visible = 0
            End If
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            stru_inicio(leng_stru).Text_node = "Administración de rutas"
            stru_inicio(leng_stru).value_node = "WF-DR-06"
            stru_inicio(leng_stru).value_card = "WF-DR-06_card_boton"
            stru_inicio(leng_stru).value_card_conten = ""
            stru_inicio(leng_stru).tipo_modulo = "WF"
            stru_inicio(leng_stru).url_node = "../workflow/WebWorkflowDigramaRuta.aspx"
            stru_inicio(leng_stru).visible_node = estado_visible
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            stru_inicio(leng_stru).Text_node = "Administración de flujos"
            stru_inicio(leng_stru).value_node = "WF-DF-07"
            stru_inicio(leng_stru).value_card = "WF-DF-07_card_boton"
            stru_inicio(leng_stru).value_card_conten = ""
            stru_inicio(leng_stru).tipo_modulo = "WF"
            stru_inicio(leng_stru).url_node = "../workflow/WebFormDiagramadorFlujoTrabajo.aspx"
            stru_inicio(leng_stru).visible_node = estado_visible
            If HttpContext.Current.Session.Item("GESTION_FLUJOS_TRABAJO") <> 0 Then
                estado_visible = 1
            Else
                estado_visible = 0
            End If
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            stru_inicio(leng_stru).Text_node = "Gestión de flujos y tareas"
            stru_inicio(leng_stru).value_node = "WF-GF-04"
            stru_inicio(leng_stru).value_card = "WF-GF-04_card_boton"
            stru_inicio(leng_stru).value_card_conten = ""
            stru_inicio(leng_stru).tipo_modulo = "WF"
            stru_inicio(leng_stru).url_externa = "YES"
            stru_inicio(leng_stru).url_node = "../workflow/WebFormGestionFlujoTrabajoCamaras.aspx"
            stru_inicio(leng_stru).url_content = "../Workflow/WebFormContenedorPageWF.aspx"
            stru_inicio(leng_stru).value_content = "GESTION FLUJOS"
            stru_inicio(leng_stru).visible_node = estado_visible
            If HttpContext.Current.Session.Item("WF_MIGRACION") <> 0 Then
                estado_visible = 1
            Else
                estado_visible = 0
            End If
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            stru_inicio(leng_stru).Text_node = "Migración SII"
            stru_inicio(leng_stru).value_node = "WF-GF-05"
            stru_inicio(leng_stru).value_card = "WF-GF-05_card_boton"
            stru_inicio(leng_stru).value_card_conten = ""
            stru_inicio(leng_stru).tipo_modulo = "WF"
            stru_inicio(leng_stru).url_node = "../integracionccv/WebForm_migra_tramite_sii.aspx"
            stru_inicio(leng_stru).value_content = "MIGRACION"
            stru_inicio(leng_stru).visible_node = estado_visible
            If tipo_modulo = "WORKFLOW DOCUMENTAL" Then
                estado_visible = 1
            Else
                estado_visible = 0
            End If
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            stru_inicio(leng_stru).Text_node = "Autenticación"
            stru_inicio(leng_stru).value_node = "WF-PC-09"
            stru_inicio(leng_stru).value_card = "WF-PC-09_card_boton"
            stru_inicio(leng_stru).value_card_conten = ""
            stru_inicio(leng_stru).tipo_modulo = "WF"
            stru_inicio(leng_stru).url_node = "../workflow/WebWorkflowCambiarPasword.aspx"
            stru_inicio(leng_stru).visible_node = estado_visible
            Solicita_items_modulos_workflow = "YES"
        Catch ex As Exception
            Solicita_items_modulos_workflow = "Inconsistencia general función Solicita_items_modulo_workflow " & ex.Message
        End Try
    End Function
    Function Solicita_item_modulo_docuarchi(ByVal tipo_modulo As String,
                                            ByRef stru_inicio() As stru_inicio_menu) As String
        Try
            Dim visible_general As Integer = 0
            Dim leng_stru As Integer = 0
            Dim estado_visible As Integer = 0
            If stru_inicio Is Nothing Then
                ReDim Preserve stru_inicio(leng_stru)
            Else
                leng_stru = stru_inicio.Length
                ReDim Preserve stru_inicio(leng_stru)
            End If
            stru_inicio(leng_stru).Text_node = "DocuArchi Contenedor"
            stru_inicio(leng_stru).value_node = "DA-PR-00"
            stru_inicio(leng_stru).value_card_conten = ""
            stru_inicio(leng_stru).url_node = ""
            If tipo_modulo = "DOCUARCHI CONTENEDOR" Or tipo_modulo = "GESTOR DOCUMENTAL" Then
                stru_inicio(leng_stru).visible_node = 1
                visible_general = 1
            Else
                stru_inicio(leng_stru).visible_node = 0
                visible_general = 0
            End If
            If visible_general = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            stru_inicio(leng_stru).Text_node = "Contenedor de documentos"
            stru_inicio(leng_stru).value_node = "DA-CLI-01"
            stru_inicio(leng_stru).value_card = "DA-CLI-01_card_boton"
            stru_inicio(leng_stru).value_card_conten = ""
            stru_inicio(leng_stru).tipo_modulo = "DA"
            stru_inicio(leng_stru).url_node = "../Docuarchi/WebFormDaPrincipal.aspx"
            stru_inicio(leng_stru).visible_node = 1
            If tipo_modulo = "DOCUARCHI CONTENEDOR" Then
                estado_visible = 1
            Else
                estado_visible = 0
            End If
            If visible_general = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            stru_inicio(leng_stru).Text_node = "Autenticación"
            stru_inicio(leng_stru).value_node = "DA-CLI-02"
            stru_inicio(leng_stru).value_card = "DA-CLI-02_card_boton"
            stru_inicio(leng_stru).value_card_conten = ""
            stru_inicio(leng_stru).tipo_modulo = "DA"
            stru_inicio(leng_stru).url_node = "../Docuarchi/WebFormDaCambiarPaswordDa.aspx"
            stru_inicio(leng_stru).visible_node = estado_visible
            If visible_general = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            Solicita_item_modulo_docuarchi = "YES"
        Catch ex As Exception
            Solicita_item_modulo_docuarchi = "Inconsistencia general funcion Solicita_item_modulo_docuarchi " & ex.Message
        End Try
    End Function
    Function Solicita_items_modulo_correspondencia(ByVal tipo_modulo As String,
                                                   ByRef stru_inicio() As stru_inicio_menu) As String
        Try
            Dim visible_general As Integer = 0
            Dim leng_stru As Integer = 0
            Dim estado_visible As Integer = 0
            If stru_inicio Is Nothing Then
                ReDim Preserve stru_inicio(leng_stru)
            Else
                leng_stru = stru_inicio.Length
                ReDim Preserve stru_inicio(leng_stru)
            End If
            stru_inicio(leng_stru).Text_node = "Corespondencia"
            stru_inicio(leng_stru).value_node = "CR-PR-00"
            stru_inicio(leng_stru).value_card = "CR-PR-00_card_boton"
            stru_inicio(leng_stru).value_card_conten = ""
            stru_inicio(leng_stru).tipo_modulo = "RD"
            stru_inicio(leng_stru).nodo_plantilla_radicado = ""
            stru_inicio(leng_stru).url_node = ""
            If tipo_modulo = "RADICACION DOCUMENTAL" Or tipo_modulo = "GESTOR DOCUMENTAL" Then
                stru_inicio(leng_stru).visible_node = 1
                visible_general = 1
            Else
                stru_inicio(leng_stru).visible_node = 0
                visible_general = 0
            End If
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            stru_inicio(leng_stru).Text_node = "Gestión de correspondencia"
            stru_inicio(leng_stru).value_node = "CR-GT-01"
            stru_inicio(leng_stru).value_card = "CR-GT-01_card_boton"
            stru_inicio(leng_stru).value_card_conten = "CR-GT-01_card_content"
            stru_inicio(leng_stru).tipo_modulo = "RD"
            stru_inicio(leng_stru).nodo_plantilla_radicado = ""
            stru_inicio(leng_stru).url_node = "../Gestion_correspondencia/WebForm_interface_gestion_tramite.aspx"
            stru_inicio(leng_stru).visible_node = 1
            If visible_general = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            stru_inicio(leng_stru).Text_node = "Gestión de correspondencia"
            stru_inicio(leng_stru).value_node = "CR-GT-01_"
            stru_inicio(leng_stru).value_card = ""
            stru_inicio(leng_stru).value_card_conten = ""
            stru_inicio(leng_stru).tipo_modulo = "RD"
            stru_inicio(leng_stru).nodo_plantilla_radicado = ""
            stru_inicio(leng_stru).url_node = "../Gestion_correspondencia/WebForm_interface_gestion_tramite.aspx"
            stru_inicio(leng_stru).visible_node = 1
            If visible_general = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            '//-----Agrega la opción de correspodencia simple----//
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            stru_inicio(leng_stru).Text_node = "Radicación simplificada"
            stru_inicio(leng_stru).value_node = "CR-HR-300"
            stru_inicio(leng_stru).value_card = "CR-HR-300_card_boton"
            stru_inicio(leng_stru).value_card_conten = "CR-HR-300_card_content"
            stru_inicio(leng_stru).tipo_modulo = "RD"
            stru_inicio(leng_stru).url_node = "../RadicadorSimplificado/Web_form_radicacion_simpilificada.aspx"
            stru_inicio(leng_stru).visible_node = HttpContext.Current.Session.Item("RA_PERMISO_GESTION_CORRESPONDENCIA_SIMPLE")
            If visible_general = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            stru_inicio(leng_stru).Text_node = "Historial de correspondencia"
            stru_inicio(leng_stru).value_node = "CR-HR-02"
            stru_inicio(leng_stru).value_card = "CR-HR-02_card_boton"
            stru_inicio(leng_stru).value_card_conten = ""
            stru_inicio(leng_stru).tipo_modulo = "RD"
            stru_inicio(leng_stru).nodo_plantilla_radicado = ""
            stru_inicio(leng_stru).url_node = "../Gestion/FormGaHistorialRespuesta.aspx"
            stru_inicio(leng_stru).visible_node = 1
            If visible_general = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            stru_inicio(leng_stru).Text_node = "Remisión interna de correspondencia"
            stru_inicio(leng_stru).value_node = "CR-CR-07"
            stru_inicio(leng_stru).value_card = "CR-CR-07_card_boton"
            stru_inicio(leng_stru).value_card_conten = ""
            stru_inicio(leng_stru).tipo_modulo = "RD"
            stru_inicio(leng_stru).nodo_plantilla_radicado = ""
            stru_inicio(leng_stru).url_node = "../radicador/WebFormRemisionCorrespondencia.aspx"
            stru_inicio(leng_stru).visible_node = HttpContext.Current.Session.Item("RA_PERMISO_REMISION_CORRESPONDENCIA_INTERNA")
            If visible_general = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            stru_inicio(leng_stru).Text_node = "Respuestas pendientes por aprobar"
            stru_inicio(leng_stru).value_node = "CR-RP-03"
            stru_inicio(leng_stru).value_card = "CR-RP-03_card_boton"
            stru_inicio(leng_stru).value_card_conten = "CR-RP-03_card_content"
            stru_inicio(leng_stru).tipo_modulo = "RD"
            stru_inicio(leng_stru).nodo_plantilla_radicado = ""
            stru_inicio(leng_stru).url_node = "../radicador/webformlistasolicitudespormiaprobacion.aspx"
            stru_inicio(leng_stru).visible_node = 1
            If visible_general = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            stru_inicio(leng_stru).Text_node = "Respuestas a correo pendientes por confirmar"
            stru_inicio(leng_stru).value_node = "CR-RE-04"
            stru_inicio(leng_stru).value_card = "CR-RE-04_card_boton"
            stru_inicio(leng_stru).value_card_conten = ""
            stru_inicio(leng_stru).tipo_modulo = "RD"
            stru_inicio(leng_stru).nodo_plantilla_radicado = ""
            stru_inicio(leng_stru).url_node = ""
            stru_inicio(leng_stru).visible_node = 0
            If HttpContext.Current.Session("RA_PERMISO_GESTION_RESPUESTA") = 0 Then
                estado_visible = 0
            Else
                estado_visible = 1
            End If
            If visible_general = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            stru_inicio(leng_stru).Text_node = "Gestión de respuestas físicas"
            stru_inicio(leng_stru).value_node = "CR-PR-02"
            stru_inicio(leng_stru).value_card = "CR-PR-02_card_boton"
            stru_inicio(leng_stru).value_card_conten = ""
            stru_inicio(leng_stru).tipo_modulo = "RD"
            stru_inicio(leng_stru).nodo_plantilla_radicado = ""
            stru_inicio(leng_stru).url_node = ""
            stru_inicio(leng_stru).visible_node = estado_visible
            If visible_general = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            stru_inicio(leng_stru).Text_node = "Pendientes por enviar"
            stru_inicio(leng_stru).value_node = "CR-RE-05"
            stru_inicio(leng_stru).value_card = "CR-RE-05_card_boton"
            stru_inicio(leng_stru).value_card_conten = ""
            stru_inicio(leng_stru).tipo_modulo = "RD"
            stru_inicio(leng_stru).nodo_plantilla_radicado = ""
            stru_inicio(leng_stru).url_node = "../radicador/WebFormCorespondenciaporenviar.aspx"
            stru_inicio(leng_stru).visible_node = estado_visible
            If visible_general = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            stru_inicio(leng_stru).Text_node = "Pendientes por confirmar"
            stru_inicio(leng_stru).value_node = "CR-RC-06"
            stru_inicio(leng_stru).value_card = "CR-RC-06_card_boton"
            stru_inicio(leng_stru).value_card_conten = ""
            stru_inicio(leng_stru).tipo_modulo = "RD"
            stru_inicio(leng_stru).nodo_plantilla_radicado = ""
            stru_inicio(leng_stru).url_node = "../radicador/WebFormArchivaenviocorrespo.aspx"
            stru_inicio(leng_stru).visible_node = estado_visible
            If HttpContext.Current.Session("RA_PERMISO_GESTION_CORRESPONDENCIA") = 0 Then
                estado_visible = 0
            Else
                estado_visible = 1
            End If
            If visible_general = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            stru_inicio(leng_stru).Text_node = "Gestión de guías"
            stru_inicio(leng_stru).value_node = "CR-PR-03"
            stru_inicio(leng_stru).value_card = "CR-PR-03_card_boton"
            stru_inicio(leng_stru).value_card_conten = ""
            stru_inicio(leng_stru).tipo_modulo = "RD"
            stru_inicio(leng_stru).nodo_plantilla_radicado = ""
            stru_inicio(leng_stru).url_node = ""
            stru_inicio(leng_stru).visible_node = estado_visible
            If visible_general = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            stru_inicio(leng_stru).Text_node = "Registrar guías de envío"
            stru_inicio(leng_stru).value_node = "CR-RG-08"
            stru_inicio(leng_stru).value_card = "CR-RG-08_card_boton"
            stru_inicio(leng_stru).value_card_conten = ""
            stru_inicio(leng_stru).tipo_modulo = "RD"
            stru_inicio(leng_stru).nodo_plantilla_radicado = ""
            stru_inicio(leng_stru).url_node = "../radicador/WebFormRaRegistrarguia.aspx"
            stru_inicio(leng_stru).visible_node = 1
            If visible_general = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            stru_inicio(leng_stru).Text_node = "Gestionar guías de envío"
            stru_inicio(leng_stru).value_node = "CR-GG-09"
            stru_inicio(leng_stru).value_card = "CR-GG-09_card_boton"
            stru_inicio(leng_stru).value_card_conten = ""
            stru_inicio(leng_stru).tipo_modulo = "RD"
            stru_inicio(leng_stru).nodo_plantilla_radicado = ""
            stru_inicio(leng_stru).url_node = "../radicador/WebFormRaGestionarGuias.aspx"
            stru_inicio(leng_stru).visible_node = 1
            If visible_general = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            stru_inicio(leng_stru).Text_node = "Consultar guías de envío"
            stru_inicio(leng_stru).value_node = "CR-CG-10"
            stru_inicio(leng_stru).value_card = "CR-CG-10_card_boton"
            stru_inicio(leng_stru).value_card_conten = ""
            stru_inicio(leng_stru).tipo_modulo = "RD"
            stru_inicio(leng_stru).nodo_plantilla_radicado = ""
            stru_inicio(leng_stru).url_node = "../radicador/WebFormRaConsultaGuias.aspx"
            stru_inicio(leng_stru).visible_node = 1
            If visible_general = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            Dim Result As String = ""
            Dim matri_platilla_radicado() As plantillas
            Erase matri_platilla_radicado
            Dim ref_Class_permisos_plantilla As New Class_permisos_plantilla
            Result = ref_Class_permisos_plantilla.Solicita_plantillas_radicado_permitidas_usuario_radicador(HttpContext.Current.Session.Item("RA_ID_USUARIO"),
                                                                                                            matri_platilla_radicado)
            If Result <> "YES" Then
                Solicita_items_modulo_correspondencia = Result
                Exit Function
            End If
            Dim estado_radicado As Integer = 0
            Dim estado_consulta As Integer = 0
            If Not matri_platilla_radicado Is Nothing Then
                For i As Integer = 0 To matri_platilla_radicado.Length - 1
                    If matri_platilla_radicado(i).permiso_radicado = 1 Then
                        leng_stru = leng_stru + 1
                        estado_radicado = 1
                        ReDim Preserve stru_inicio(leng_stru)
                        stru_inicio(leng_stru).Text_node = UCase(matri_platilla_radicado(i).nombre_plantilla)
                        stru_inicio(leng_stru).nodo_plantilla_radicado = "yes"
                        stru_inicio(leng_stru).id_plantilla = matri_platilla_radicado(i).id_plantilla
                        stru_inicio(leng_stru).tipo_plantilla = matri_platilla_radicado(i).tipo_plantilla
                        stru_inicio(leng_stru).value_node = "CR-PR-11-1" & matri_platilla_radicado(i).id_plantilla & matri_platilla_radicado(i).tipo_plantilla
                        stru_inicio(leng_stru).value_card = "CR-PR-11-1_card_boton" & matri_platilla_radicado(i).id_plantilla & matri_platilla_radicado(i).tipo_plantilla
                        stru_inicio(leng_stru).value_card_conten = ""
                        stru_inicio(leng_stru).tipo_modulo = "RD"
                        stru_inicio(leng_stru).url_node = "../radicador/WebFormRadicacionEntrante.aspx"
                        stru_inicio(leng_stru).visible_node = 1
                        If visible_general = 0 Then
                            stru_inicio(leng_stru).visible_node = 0
                        End If
                    End If
                Next
                For i As Integer = 0 To matri_platilla_radicado.Length - 1
                    If matri_platilla_radicado(i).permiso_consulta = 1 Then
                        leng_stru = leng_stru + 1
                        estado_consulta = 1
                        ReDim Preserve stru_inicio(leng_stru)
                        stru_inicio(leng_stru).Text_node = UCase(matri_platilla_radicado(i).nombre_plantilla)
                        stru_inicio(leng_stru).nodo_plantilla_radicado = "consulta"
                        stru_inicio(leng_stru).id_plantilla = matri_platilla_radicado(i).id_plantilla
                        stru_inicio(leng_stru).tipo_plantilla = matri_platilla_radicado(i).tipo_plantilla
                        stru_inicio(leng_stru).value_node = "CR-PR-11-2" & matri_platilla_radicado(i).id_plantilla & matri_platilla_radicado(i).tipo_plantilla
                        stru_inicio(leng_stru).value_card = "CR-PR-11-2_card_boton" & matri_platilla_radicado(i).id_plantilla & matri_platilla_radicado(i).tipo_plantilla
                        stru_inicio(leng_stru).value_card_conten = ""
                        stru_inicio(leng_stru).tipo_modulo = "RD"
                        stru_inicio(leng_stru).url_node = "../radicador/WebFormConsultaRadicacion.aspx"
                        stru_inicio(leng_stru).visible_node = 1
                        If visible_general = 0 Then
                            stru_inicio(leng_stru).visible_node = 0
                        End If
                    End If
                Next
            End If
            If HttpContext.Current.Session("RA_PERMISO_RADICADO") = "0" Then
                estado_radicado = 0
            End If
            If HttpContext.Current.Session("RA_PERMISO_CONSULTA") = "0" Then
                estado_consulta = 0
            End If
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            stru_inicio(leng_stru).Text_node = "Radicación de correspondencia"
            stru_inicio(leng_stru).value_node = "CR-PR-11"
            stru_inicio(leng_stru).value_card = "CR-PR-11_card_boton"
            stru_inicio(leng_stru).value_card_conten = ""
            stru_inicio(leng_stru).tipo_modulo = "RD"
            stru_inicio(leng_stru).nodo_plantilla_radicado = ""
            stru_inicio(leng_stru).url_node = ""
            stru_inicio(leng_stru).visible_node = estado_radicado
            If visible_general = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            stru_inicio(leng_stru).Text_node = "Consulta de correspondencia"
            stru_inicio(leng_stru).value_node = "CR-PR-12"
            stru_inicio(leng_stru).value_card = "CR-PR-12_card_boton"
            stru_inicio(leng_stru).value_card_conten = ""
            stru_inicio(leng_stru).tipo_modulo = "RD"
            stru_inicio(leng_stru).nodo_plantilla_radicado = ""
            stru_inicio(leng_stru).url_node = ""
            stru_inicio(leng_stru).visible_node = estado_consulta
            If tipo_modulo = "RADICACION DOCUMENTAL" Then
                estado_visible = 1
            Else
                estado_visible = 0
            End If
            If visible_general = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            stru_inicio(leng_stru).Text_node = "Autenticación"
            stru_inicio(leng_stru).value_node = "CR-PR-13"
            stru_inicio(leng_stru).value_card = "CR-PR-13_card_boton"
            stru_inicio(leng_stru).value_card_conten = ""
            stru_inicio(leng_stru).tipo_modulo = "RD"
            stru_inicio(leng_stru).nodo_plantilla_radicado = ""
            stru_inicio(leng_stru).url_node = ""
            stru_inicio(leng_stru).visible_node = estado_visible
            If visible_general = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            Solicita_items_modulo_correspondencia = "YES"
        Catch ex As Exception
            Solicita_items_modulo_correspondencia = "Inconsistencia general función Solicita_items_modulo_correspondencia " & ex.Message
        End Try
    End Function
    Function Solicita_items_modulo_gestion(ByVal tipo_modulo As String,
                                           ByRef stru_inicio() As stru_inicio_menu) As String
        Try
            Dim visible_general As Integer = 0
            Dim leng_stru As Integer = 0
            Dim estado_visible As Integer = 0
            If stru_inicio Is Nothing Then
                ReDim Preserve stru_inicio(leng_stru)
            Else
                leng_stru = stru_inicio.Length
                ReDim Preserve stru_inicio(leng_stru)
            End If
            stru_inicio(leng_stru).Text_node = "Gestión documental"
            stru_inicio(leng_stru).value_node = "GD-PR-00"
            stru_inicio(leng_stru).value_card = ""
            stru_inicio(leng_stru).value_card_conten = ""
            stru_inicio(leng_stru).tipo_modulo = "GD"
            stru_inicio(leng_stru).url_node = ""
            If tipo_modulo = "GESTOR DOCUMENTAL" Then
                stru_inicio(leng_stru).visible_node = 1
                visible_general = 1
            Else
                stru_inicio(leng_stru).visible_node = 0
                visible_general = 0
            End If
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            Dim estado_gestion_expediente As Integer = 0
            stru_inicio(leng_stru).Text_node = "Gestión de expedientes"
            stru_inicio(leng_stru).value_node = "GD-UD-01"
            stru_inicio(leng_stru).value_card = "GD-UD-01_card_boton"
            stru_inicio(leng_stru).value_card_conten = ""
            stru_inicio(leng_stru).tipo_modulo = "GD"
            stru_inicio(leng_stru).url_node = ""
            If HttpContext.Current.Session("GESTION_EXPEDIENTE") = 0 Then
                estado_gestion_expediente = 0
                stru_inicio(leng_stru).visible_node = 0
            Else
                estado_gestion_expediente = 1
                stru_inicio(leng_stru).visible_node = 1
            End If
            If estado_gestion_expediente = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            If visible_general = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            stru_inicio(leng_stru).Text_node = "Registro de expedientes"
            stru_inicio(leng_stru).value_node = "GD-RU-02"
            stru_inicio(leng_stru).value_card = "GD-RU-02_card_boton"
            stru_inicio(leng_stru).value_card_conten = ""
            stru_inicio(leng_stru).tipo_modulo = "GD"
            stru_inicio(leng_stru).url_node = "../Gestion/FormGaAgregarExpediente.aspx"
            If HttpContext.Current.Session("GA_REGISTRA_EXPEDIENTES") = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            Else
                stru_inicio(leng_stru).visible_node = 1
            End If
            If estado_gestion_expediente = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            If visible_general = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            stru_inicio(leng_stru).Text_node = "Consulta de expedientes"
            stru_inicio(leng_stru).value_node = "GD-CE-03"
            stru_inicio(leng_stru).value_card = "GD-CE-03_card_boton"
            stru_inicio(leng_stru).value_card_conten = "GD-CE-03_card_content"
            stru_inicio(leng_stru).tipo_modulo = "GD"
            stru_inicio(leng_stru).url_node = "../Gestion/WebFormGaGestionExpediente.aspx"
            If HttpContext.Current.Session("CONSULTA_EXPEDIENTE") = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            Else
                stru_inicio(leng_stru).visible_node = 1
            End If
            If estado_gestion_expediente = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            If visible_general = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            leng_stru += 1
            ReDim Preserve stru_inicio(leng_stru)
            Dim estado_gestion_fisica As Integer = 0
            stru_inicio(leng_stru).Text_node = "Gestión física"
            stru_inicio(leng_stru).value_node = "GD-ALM-03"
            stru_inicio(leng_stru).value_card = ""
            stru_inicio(leng_stru).value_card_conten = "GD-ALM-03_card_content"
            stru_inicio(leng_stru).url_node = ""
            stru_inicio(leng_stru).tipo_modulo = "GD"
            If HttpContext.Current.Session("GESTION_FISICA") = 0 Then
                estado_gestion_fisica = 0
                stru_inicio(leng_stru).visible_node = 0
            Else
                estado_gestion_fisica = 1
                stru_inicio(leng_stru).visible_node = 1
            End If

            If estado_gestion_fisica = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            If visible_general = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            'If HttpContext.Current.Session("GESTION_FISICA") = 1 And HttpContext.Current.Session("GA_ADMINISTRACION_ESTRUCTURA_ARCHIVO") = 0 _
            '     And HttpContext.Current.Session("GESTION_UNIDAD_CONSERVACION") = 0 Then
            '    stru_inicio(leng_stru).visible_node = 0
            'End If
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            stru_inicio(leng_stru).Text_node = "Gestión de unidades"
            stru_inicio(leng_stru).value_node = "GD-CU-04"
            stru_inicio(leng_stru).value_card = "GD-CU-04_card_boton"
            stru_inicio(leng_stru).value_card_conten = ""
            stru_inicio(leng_stru).tipo_modulo = "GD"
            stru_inicio(leng_stru).url_node = "../Gestion/WebFormGaGestionUnidadConservacion.aspx"
            If HttpContext.Current.Session("GESTION_UNIDAD_CONSERVACION") = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            Else
                stru_inicio(leng_stru).visible_node = 1
            End If
            If estado_gestion_fisica = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            If visible_general = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            stru_inicio(leng_stru).Text_node = "Gestión topografica"
            stru_inicio(leng_stru).value_node = "GD-GT-05"
            stru_inicio(leng_stru).value_card = "GD-GT-05_card_boton"
            stru_inicio(leng_stru).value_card_conten = ""
            stru_inicio(leng_stru).tipo_modulo = "GD"
            stru_inicio(leng_stru).url_node = "../Gestion/WebFormGatoponimica.aspx"
            If HttpContext.Current.Session("GA_ADMINISTRACION_ESTRUCTURA_ARCHIVO") = 0 Then
                estado_gestion_fisica = 0
                stru_inicio(leng_stru).visible_node = 0
            Else
                estado_gestion_fisica = 1
                stru_inicio(leng_stru).visible_node = 1
            End If
            If estado_gestion_fisica = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            If visible_general = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If

            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            Dim estado_gestion_intrumentos As Integer = 0
            stru_inicio(leng_stru).Text_node = "Gestión de instrumentos"
            stru_inicio(leng_stru).value_node = "GD-GI-06"
            stru_inicio(leng_stru).url_node = ""
            stru_inicio(leng_stru).tipo_modulo = "GD"
            If HttpContext.Current.Session("GA_ADMINISTRACION_INSTRUMENTO") = 0 Then
                estado_gestion_intrumentos = 0
                stru_inicio(leng_stru).visible_node = 0
            Else
                estado_gestion_intrumentos = 1
                stru_inicio(leng_stru).visible_node = 1
            End If
            'If HttpContext.Current.Session("GA_ADMINISTRACION_INSTRUMENTO") = 1 And HttpContext.Current.Session("GA_ADMINISTRACION_CCD") = 0 _
            '     And HttpContext.Current.Session("GA_CONSULTA_CUADRO_CLASIFICACION") = 0 And HttpContext.Current.Session("GA_ADMINISTRACION_TRD") = 0 _
            '     And HttpContext.Current.Session("GA_CONSULTA_TABLA_RETENCION") = 0 Then
            '    stru_inicio(leng_stru).visible_node = 0
            'End If
            If estado_gestion_intrumentos = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            If visible_general = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            stru_inicio(leng_stru).Text_node = "Gestión de cuadros de clasificación"
            stru_inicio(leng_stru).value_node = "GD-AC-06"
            stru_inicio(leng_stru).value_card = "GD-AC-06_card_boton"
            stru_inicio(leng_stru).value_card_conten = ""
            stru_inicio(leng_stru).tipo_modulo = "GD"
            stru_inicio(leng_stru).url_node = "../Gestion/WebFormGaadmonclasificacion.aspx"
            If HttpContext.Current.Session("GA_ADMINISTRACION_CCD") = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            Else
                stru_inicio(leng_stru).visible_node = 1
            End If
            If estado_gestion_intrumentos = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            If visible_general = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            stru_inicio(leng_stru).Text_node = "Consulta  cuadros de clasificación"
            stru_inicio(leng_stru).value_node = "GD-CC-07"
            stru_inicio(leng_stru).value_card = "GD-CC-07_card_boton"
            stru_inicio(leng_stru).value_card_conten = ""
            stru_inicio(leng_stru).tipo_modulo = "GD"
            stru_inicio(leng_stru).url_node = "../Gestion/WebFormGaconsultaclasificacion.aspx"
            If HttpContext.Current.Session("GA_CONSULTA_CUADRO_CLASIFICACION") = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            Else
                stru_inicio(leng_stru).visible_node = 1
            End If
            If estado_gestion_intrumentos = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            If visible_general = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            stru_inicio(leng_stru).Text_node = "Gestión de tablas de retención y valoración"
            stru_inicio(leng_stru).value_node = "GD-AI-08"
            stru_inicio(leng_stru).value_card = "GD-AI-08_card_boton"
            stru_inicio(leng_stru).value_card_conten = ""
            stru_inicio(leng_stru).tipo_modulo = "GD"
            stru_inicio(leng_stru).url_node = "../Gestion/WebFormGaGestionInstrumentos.aspx"
            If HttpContext.Current.Session("GA_ADMINISTRACION_TRD") = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            Else
                stru_inicio(leng_stru).visible_node = 1
            End If
            If estado_gestion_intrumentos = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            If visible_general = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            stru_inicio(leng_stru).Text_node = "Consulta de tablas de retención"
            stru_inicio(leng_stru).value_node = "GD-CR-09"
            stru_inicio(leng_stru).value_card = "GD-CR-09_card_boton"
            stru_inicio(leng_stru).value_card_conten = ""
            stru_inicio(leng_stru).tipo_modulo = "GD"
            stru_inicio(leng_stru).url_node = "../Gestion/WebFormGaAplicarTrd.aspx"
            If HttpContext.Current.Session("GA_CONSULTA_TABLA_RETENCION") = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            Else
                stru_inicio(leng_stru).visible_node = 1
            End If
            If estado_gestion_intrumentos = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            If visible_general = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            Dim estado_estructura_organica As Integer = 0
            stru_inicio(leng_stru).Text_node = "Estructura organica"
            stru_inicio(leng_stru).value_node = "GD-PR-11"
            stru_inicio(leng_stru).value_card = "GD-PR-11_card_boton"
            stru_inicio(leng_stru).value_card_conten = ""
            stru_inicio(leng_stru).tipo_modulo = "GD"
            stru_inicio(leng_stru).url_node = ""
            If HttpContext.Current.Session("GA_ADMINISTRACION_ORGANICA") = 0 Then
                estado_estructura_organica = 0
                stru_inicio(leng_stru).visible_node = 0
            Else
                estado_estructura_organica = 1
                stru_inicio(leng_stru).visible_node = 1
            End If
            If estado_estructura_organica = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            If visible_general = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            stru_inicio(leng_stru).Text_node = "Gestión de estructura"
            stru_inicio(leng_stru).value_node = "GD-AE-12"
            stru_inicio(leng_stru).value_card = "GD-AE-12_card_boton"
            stru_inicio(leng_stru).value_card_conten = ""
            stru_inicio(leng_stru).tipo_modulo = "GD"
            stru_inicio(leng_stru).url_node = "../Gestion/WebFormGestionOrganigrama.aspx"
            stru_inicio(leng_stru).visible_node = 1
            If estado_estructura_organica = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            If visible_general = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            stru_inicio(leng_stru).Text_node = "Gestión de documentos"
            stru_inicio(leng_stru).value_node = "GD-PR-13"
            stru_inicio(leng_stru).value_card = ""
            stru_inicio(leng_stru).value_card_conten = ""
            stru_inicio(leng_stru).tipo_modulo = "GD"
            stru_inicio(leng_stru).url_node = ""
            stru_inicio(leng_stru).visible_node = 1
            If visible_general = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            stru_inicio(leng_stru).Text_node = "Gestión de documentos electrónicos"
            stru_inicio(leng_stru).value_node = "GD-GD-14"
            stru_inicio(leng_stru).value_card = "GD-GD-14_card_boton"
            stru_inicio(leng_stru).value_card_conten = "GD-GD-14_card_content"
            stru_inicio(leng_stru).tipo_modulo = "GD"
            stru_inicio(leng_stru).url_node = "../Gestion/WebFormProducionDocumental.aspx"
            stru_inicio(leng_stru).visible_node = 1
            If visible_general = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            stru_inicio(leng_stru).Text_node = "Mis documentos compartidos"
            stru_inicio(leng_stru).value_node = "GD-MD-15"
            stru_inicio(leng_stru).value_card = "GD-MD-15_card_boton"
            stru_inicio(leng_stru).value_card_conten = ""
            stru_inicio(leng_stru).tipo_modulo = "GD"
            stru_inicio(leng_stru).url_node = "../Gestion/WebFormDocumentoCompartidoOtrosUsuarios.aspx"
            stru_inicio(leng_stru).visible_node = 1
            If visible_general = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            stru_inicio(leng_stru).Text_node = "Mis radicaciones internas"
            stru_inicio(leng_stru).value_node = "GD-MR-16"
            stru_inicio(leng_stru).value_card = "GD-MR-16_card_boton"
            stru_inicio(leng_stru).value_card_conten = ""
            stru_inicio(leng_stru).tipo_modulo = "GD"
            stru_inicio(leng_stru).url_node = "../radicador/WebFormConsultaRadicacion.aspx"
            stru_inicio(leng_stru).visible_node = 1
            If visible_general = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            stru_inicio(leng_stru).Text_node = "Mis radicaciones pendientes por asignar"
            stru_inicio(leng_stru).value_node = "GD-MP-17"
            stru_inicio(leng_stru).value_card = "GD-MP-17_card_boton"
            stru_inicio(leng_stru).value_card_conten = ""
            stru_inicio(leng_stru).tipo_modulo = "GD"
            stru_inicio(leng_stru).url_node = "../radicador/WebFormRadicadosPendientesPorEnviar.aspx"
            stru_inicio(leng_stru).visible_node = 1
            If visible_general = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            stru_inicio(leng_stru).Text_node = "Notificaciones y solicitudes"
            stru_inicio(leng_stru).value_node = "GD-PR-18"
            stru_inicio(leng_stru).value_card = "GD-PR-18_card_boton"
            stru_inicio(leng_stru).value_card_conten = ""
            stru_inicio(leng_stru).tipo_modulo = "GD"
            stru_inicio(leng_stru).url_node = ""
            stru_inicio(leng_stru).visible_node = 1
            If visible_general = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            stru_inicio(leng_stru).Text_node = "Documentos pendientes por revisar"
            stru_inicio(leng_stru).value_node = "GD-DC-19"
            stru_inicio(leng_stru).value_card = "GD-DC-19_card_boton"
            stru_inicio(leng_stru).value_card_conten = "GD-DC-19_card_content"
            stru_inicio(leng_stru).tipo_modulo = "GD"
            stru_inicio(leng_stru).url_node = "../Gestion/WebFormDocumentosCompartidosRevision.aspx"
            stru_inicio(leng_stru).visible_node = 1
            If visible_general = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            stru_inicio(leng_stru).Text_node = "Documentos compartidos pendientes por mi revisión"
            stru_inicio(leng_stru).value_node = "GD-DC-19_"
            stru_inicio(leng_stru).value_card = ""
            stru_inicio(leng_stru).value_card_conten = ""
            stru_inicio(leng_stru).tipo_modulo = "GD"
            stru_inicio(leng_stru).url_node = "../Gestion/WebFormDocumentosCompartidosRevision.aspx"
            stru_inicio(leng_stru).visible_node = 1
            If visible_general = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            If tipo_modulo = "GESTOR DOCUMENTAL" Then
                estado_visible = 1
            Else
                estado_visible = 0
            End If
            If visible_general = 0 Then
                stru_inicio(leng_stru).visible_node = 0
                estado_visible = 0
            End If
            '///---------------------Migración de documentos----------//
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            stru_inicio(leng_stru).Text_node = "Gestión y migración"
            stru_inicio(leng_stru).value_node = "GD-MR-15"
            stru_inicio(leng_stru).value_card = ""
            stru_inicio(leng_stru).value_card_conten = ""
            stru_inicio(leng_stru).tipo_modulo = "GD"
            stru_inicio(leng_stru).url_node = ""
            stru_inicio(leng_stru).visible_node = 1
            If visible_general = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            If HttpContext.Current.Session("UTIL_MODULO_MIGRA_FORMATO_ARCHIVO") = 0 And HttpContext.Current.Session("UTIL_MODULO_CONSULTA_MIGRA_FORMATO_ARCHIVO") = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            stru_inicio(leng_stru).Text_node = "Migración de documentos"
            stru_inicio(leng_stru).value_node = "GD-MR-99"
            stru_inicio(leng_stru).value_card = "GD-MR-99_card_boton"
            stru_inicio(leng_stru).value_card_conten = "GD-MR-16_card_content"
            stru_inicio(leng_stru).tipo_modulo = "GD"
            stru_inicio(leng_stru).url_node = "../Gestion_migracion/Web_form_gestion_migracion_documento.aspx"
            stru_inicio(leng_stru).visible_node = 1
            If visible_general = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            If HttpContext.Current.Session("UTIL_MODULO_MIGRA_FORMATO_ARCHIVO") = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            stru_inicio(leng_stru).Text_node = "Consulta documentos migrados"
            stru_inicio(leng_stru).value_node = "GD-MR-17"
            stru_inicio(leng_stru).value_card = "GD-MR-17_card_boton"
            stru_inicio(leng_stru).value_card_conten = "GD-MR-16_card_content"
            stru_inicio(leng_stru).tipo_modulo = "GD"
            stru_inicio(leng_stru).url_node = "../Gestion_migracion/Web_form_consulta_documentos_migrados.aspx"
            stru_inicio(leng_stru).visible_node = 1
            If visible_general = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            If HttpContext.Current.Session("UTIL_MODULO_CONSULTA_MIGRA_FORMATO_ARCHIVO") = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            stru_inicio(leng_stru).Text_node = "Reportes"
            stru_inicio(leng_stru).value_node = "GD-PR-06"
            stru_inicio(leng_stru).value_card = "GD-PR-06_card_boton"
            stru_inicio(leng_stru).value_card_conten = ""
            stru_inicio(leng_stru).tipo_modulo = "GD"
            stru_inicio(leng_stru).url_node = ""
            stru_inicio(leng_stru).visible_node = 1
            If visible_general = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            stru_inicio(leng_stru).Text_node = "Reportes de gestión"
            stru_inicio(leng_stru).value_node = "GD-RG-07"
            stru_inicio(leng_stru).value_card = "GD-RG-07_card_boton"
            stru_inicio(leng_stru).value_card_conten = ""
            stru_inicio(leng_stru).tipo_modulo = "GD"
            stru_inicio(leng_stru).url_externa = "YES"
            stru_inicio(leng_stru).url_node = "../radicador/WebFormReportesRadicacion.aspx"
            stru_inicio(leng_stru).url_content = "../radicador/WebFormRadicadoExterno.aspx"
            stru_inicio(leng_stru).value_content = "REPORTES DE GESTION"
            stru_inicio(leng_stru).visible_node = 1
            If visible_general = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            stru_inicio(leng_stru).Text_node = "Inicio aplicacion"
            stru_inicio(leng_stru).value_node = "CI-AP-001"
            stru_inicio(leng_stru).value_card = ""
            stru_inicio(leng_stru).value_card_conten = ""
            stru_inicio(leng_stru).tipo_modulo = "GD"
            stru_inicio(leng_stru).url_node = "../workflow/WebFormDefaultSitio.aspx"
            stru_inicio(leng_stru).visible_node = 1
            If visible_general = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            leng_stru = leng_stru + 1
            ReDim Preserve stru_inicio(leng_stru)
            stru_inicio(leng_stru).Text_node = "Autenticación"
            stru_inicio(leng_stru).value_node = "GD-PR-20"
            stru_inicio(leng_stru).value_card = "GD-PR-20_card_boton"
            stru_inicio(leng_stru).value_card_conten = ""
            stru_inicio(leng_stru).tipo_modulo = "GD"
            stru_inicio(leng_stru).url_node = "../Gestion/WebFormPaswordGestion.aspx"
            stru_inicio(leng_stru).visible_node = estado_visible
            If visible_general = 0 Then
                stru_inicio(leng_stru).visible_node = 0
            End If
            Solicita_items_modulo_gestion = "YES"
        Catch ex As Exception
            Solicita_items_modulo_gestion = "Inconsistencia general función Solicita_items_modulo_gestion " & ex.Message
        End Try
    End Function
    Function inicio_adplicacion_web_gestion_publico() As String
        Try
            Dim Result As String = ""
            Dim Class_remit_dest_interno As New Class_remit_dest_interno
            Dim existencia_loguin As String = "NO"
            Result = Class_remit_dest_interno.Verifica_nombre_usuario_gestion("CONSULTAPUBLICO",
                                                                              existencia_loguin)
            If Result <> "YES" Then
                inicio_adplicacion_web_gestion_publico = Result
                Exit Function
            End If
            If existencia_loguin = "NO" Then
                inicio_adplicacion_web_gestion_publico = "El usuario (CONSULTAPUBLICO) no se encuentra registrado, por favor contacte a su administrador."
                Exit Function
            End If
            Dim stru_service() As stru_detalle_web_service = Nothing
            'Result = Me.Gestor_Retorna_Detalle_webserice(HttpContext.Current.Session.Item("EMPRESA_GESTION"),
            '                                             stru_service)
            'If Result <> "YES" Then
            '    inicio_adplicacion_web_gestion_publico = Result
            '    Exit Function
            'End If
            Dim refclasiniciowf As New InicioWorkflow
            Dim refclasgestiondocumental As New ClassGestionDocumental
            Dim id_usuario_gestion As Integer = 0
            Dim login_usuario_workflow As String = ""
            Dim id_usuario_workflow As Integer = 0
            Dim login_usuario_docuarchi As String = ""
            Dim id_usuario_docuarchi As Integer = 0
            Dim login_usuario_radicacion As String = ""
            Dim id_usuario_radicacion As Integer = 0
            '---------------------------------------
            'Asigna los datos de relacion del
            'perfil de usuario gestion
            'con los modulos workflow, docuarchi,
            'y radicacion
            '----------------------------------------
            Result = refclasgestiondocumental.SolicitaDatosUsuarioGestionLogin("CONSULTAPUBLICO",
                                                                                  id_usuario_gestion,
                                                                                  login_usuario_workflow,
                                                                                  id_usuario_workflow,
                                                                                  login_usuario_docuarchi,
                                                                                  id_usuario_docuarchi,
                                                                                  login_usuario_radicacion,
                                                                                  id_usuario_radicacion)
            If Result <> "YES" Then
                inicio_adplicacion_web_gestion_publico = Result
                Exit Function
            End If
            HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") = "CONSULTAPUBLICO"
            '-----------------------------------
            'Asigna perfil gestiòn documental
            '-----------------------------------
            Dim ref_Class_remit_dest_interno_perfil_produccion As New Class_remit_dest_interno_perfil_produccion
            Result = ref_Class_remit_dest_interno_perfil_produccion.AsignaPermisosPerfilUsuarioGestion(id_usuario_gestion)
            If Result <> "YES" Then
                inicio_adplicacion_web_gestion_publico = Result
                Exit Function
            End If
            '-------------------------------------
            'Retorna id empresa usuario gestión
            '-------------------------------------
            Dim ref_gestion As New ClassAdmonEmpresa
            Result = ref_gestion.Retorna_id_empresa_usuario_gestion(HttpContext.Current.Session.Item("GA_IDEMPRESA"),
                                                                    id_usuario_gestion)
            If Result <> "YES" Then
                inicio_adplicacion_web_gestion_publico = Result
                Exit Function
            End If
            '----------------------------------------------
            'Asigna perfil radicación
            '----------------------------------------------
            If id_usuario_radicacion <> 0 Then
                HttpContext.Current.Session.Item("RA_ID_USUARIO") = id_usuario_radicacion
            End If
            HttpContext.Current.Session.Item("ID_USUARIO_DOCUARCHI") = id_usuario_docuarchi
            '-----------------------------------
            'Retorna login usuario docuarchi
            '-----------------------------------
            Dim Refclasda As New ClassDaIncioDocuarchi
            Result = Refclasda.SolicitaloginUsuarioDocuarchi(HttpContext.Current.Session.Item("ID_USUARIO_DOCUARCHI"),
                                                               HttpContext.Current.Session.Item("DA_Login_Usuario"))
            If Result <> "YES" Then
                inicio_adplicacion_web_gestion_publico = Result
                Exit Function
            End If
            '-----------------------------------
            'Retorna grupo usuario docuarchi
            '-----------------------------------
            Dim Refclasinicio As New Class_relacion_usu_grup
            Result = Refclasinicio.SolicitaGrupoRelacionadousuarioDocuarchi(HttpContext.Current.Session.Item("ID_USUARIO_DOCUARCHI"),
                                                                   HttpContext.Current.Session.Item("DA_gruposusu"))
            If Result <> "YES" Then
                inicio_adplicacion_web_gestion_publico = Result
                Exit Function
            End If
            '---------------------------------------------
            'Inicializa workflow
            '---------------------------------------------
            If id_usuario_workflow <> 0 Then
                Result = refclasiniciowf.InicializaSesionModuloWorkflow(login_usuario_workflow)
                If Result <> "YES" Then
                    inicio_adplicacion_web_gestion_publico = Result
                    Exit Function
                End If
            End If
            '--------------------------------------------------
            'Inserta log de usuario
            '--------------------------------------------------
            Dim ref_Class_log_usuario_gestion As New Class_log_usuario_gestion
            Result = ref_Class_log_usuario_gestion.RegistroSesionLogusuarioGestionDocumental(id_usuario_gestion,
                                                                                                HttpContext.Current.Session.Item("ip_host_name"),
                                                                                                HttpContext.Current.Session.Item("id_registro_sesion_log_gd"))
            If Result <> "YES" Then
                inicio_adplicacion_web_gestion_publico = Result
                Exit Function
            End If

            If Not stru_service Is Nothing Then
                For i As Integer = 0 To stru_service.Length - 1
                    Select Case stru_service(i).TIPO_MODULO
                        Case "WORKFLOW DOCUMENTAL"
                            HttpContext.Current.Session.Item("ACTIVA_WEB_SERVICE") = stru_service(i).ACTIVA_WEB_SERVICE
                            HttpContext.Current.Session.Item("URL_WEB_SERVICE") = stru_service(i).URL_WEB_SERVICE
                            HttpContext.Current.Session.Item("USER_WEB_SERVICE") = stru_service(i).USER_WEB_SERVICE
                            HttpContext.Current.Session.Item("PASW_WEB_SERVICE") = stru_service(i).PASW_WEB_SERVICE
                        Case "RADICACION DOCUMENTAL"
                            HttpContext.Current.Session.Item("RA_ACTIVA_WEB_SERVICE") = stru_service(i).ACTIVA_WEB_SERVICE
                            HttpContext.Current.Session.Item("RA_URL_WEB_SERVICE") = stru_service(i).URL_WEB_SERVICE
                            HttpContext.Current.Session.Item("RA_USER_WEB_SERVICE") = stru_service(i).USER_WEB_SERVICE
                            HttpContext.Current.Session.Item("RA_PASW_WEB_SERVICE") = stru_service(i).PASW_WEB_SERVICE
                        Case "DOCUARCHI CONTENEDOR"
                            HttpContext.Current.Session.Item("DA_ACTIVA_WEB_SERVICE") = stru_service(i).ACTIVA_WEB_SERVICE
                            HttpContext.Current.Session.Item("DA_URL_WEB_SERVICE") = stru_service(i).URL_WEB_SERVICE
                            HttpContext.Current.Session.Item("DA_USER_WEB_SERVICE") = stru_service(i).USER_WEB_SERVICE
                            HttpContext.Current.Session.Item("DA_PASW_WEB_SERVICE") = stru_service(i).PASW_WEB_SERVICE
                        Case "GESTOR DOCUMENTAL"
                            HttpContext.Current.Session.Item("GA_ACTIVA_WEB_SERVICE") = stru_service(i).ACTIVA_WEB_SERVICE
                            HttpContext.Current.Session.Item("GA_URL_WEB_SERVICE") = stru_service(i).URL_WEB_SERVICE
                            HttpContext.Current.Session.Item("GA_USER_WEB_SERVICE") = stru_service(i).USER_WEB_SERVICE
                            HttpContext.Current.Session.Item("GA_PASW_WEB_SERVICE") = stru_service(i).PASW_WEB_SERVICE
                    End Select
                Next

            End If
            inicio_adplicacion_web_gestion_publico = "YES"
        Catch ex As Exception
            inicio_adplicacion_web_gestion_publico = "Inconsistencia función  inicio_adplicacion_web_gestion_publico " & ex.Message
        End Try
    End Function
    Function Cerrar_sesion_aplicacion_web() As String
        Try
            Dim Result As String = ""
            Dim Refclas_inicio_gestion As New ClassGagestorInicio
            Dim refclas_inicio_workflow As New InicioWorkflow
            Dim refclas_inicio_radicado As New ClassInicioRadicador
            Dim refclas_inicio_docuarchi As New ClassDaIncioDocuarchi
            If HttpContext.Current.Session.Item("TIPOMODULO") = "GESTOR DOCUMENTAL" Then
                If HttpContext.Current.Session.Item("id_registro_sesion_log_gd") <> "0" Then
                    Dim ref_Class_log_usuario_gestion As New Class_log_usuario_gestion
                    Result = ref_Class_log_usuario_gestion.Actualiza_log_sesion_usuario_gestion_documental(HttpContext.Current.Session.Item("id_registro_sesion_log_gd"))
                    If Result <> "YES" Then
                        Cerrar_sesion_aplicacion_web = Result
                        Exit Function
                    End If
                    If HttpContext.Current.Session.Item("id_registro_sesion_log_wf") <> "0" Then
                        Result = refclas_inicio_workflow.update_log_sesion_usuario_workflow(HttpContext.Current.Session.Item("id_registro_sesion_log_wf"))
                        If Result <> "YES" Then
                            Cerrar_sesion_aplicacion_web = Result
                            Exit Function
                        End If
                    End If

                End If
            End If
            If HttpContext.Current.Session.Item("TIPOMODULO") = "WORKFLOW DOCUMENTAL" Then
                If HttpContext.Current.Session.Item("id_registro_sesion_log_wf") <> "0" Then
                    Result = refclas_inicio_workflow.update_log_sesion_usuario_workflow(HttpContext.Current.Session.Item("id_registro_sesion_log_wf"))
                    If Result <> "YES" Then
                        Cerrar_sesion_aplicacion_web = Result
                        Exit Function
                    End If
                End If
            End If
            If HttpContext.Current.Session.Item("TIPOMODULO") = "RADICACION DOCUMENTAL" Then
                If HttpContext.Current.Session.Item("id_registro_sesion_log_ra") <> "0" Then
                    Result = refclas_inicio_radicado.update_log_sesion_usuario_radicador(HttpContext.Current.Session.Item("id_registro_sesion_log_ra"))
                    If Result <> "YES" Then
                        Cerrar_sesion_aplicacion_web = Result
                        Exit Function
                    End If
                End If
            End If
            If HttpContext.Current.Session.Item("TIPOMODULO") = "DOCUARCHI CONTENEDOR" Then
                If HttpContext.Current.Session.Item("id_registro_sesion_log") <> "0" Then
                    Result = refclas_inicio_docuarchi.update_log_sesion_usuario_docuarchi(HttpContext.Current.Session.Item("id_registro_sesion_log"))
                    If Result <> "YES" Then
                        Cerrar_sesion_aplicacion_web = Result
                        Exit Function
                    End If
                End If
            End If
            Cerrar_sesion_aplicacion_web = "YES"
        Catch ex As Exception
            Cerrar_sesion_aplicacion_web = "Inconsistencia general función Cerrar_sesion_aplicacion_web " & ex.Message
        End Try
    End Function

    Function InicioAplicacionWebGestorDocumental(ByVal modulo As String,
                                           ByVal user As String,
                                           ByVal passs As String,
                                           ByVal nombre_empresa As String) As String
        Dim Refclas As New GestorModuleSesion.Gestor_conexion
        Dim Mesaje As New Classscrripjava
        Dim Result As String = ""
        Dim reclas As New Classscrripjava
        Dim refclasiniciowf As New InicioWorkflow
        Dim refclasgestiondocumental As New ClassGestionDocumental
        Try
            If modulo = "" Then
                InicioAplicacionWebGestorDocumental = "Por favor seleccione el módulo de aplicación"
                Exit Function
            End If
            If user = "" Then
                InicioAplicacionWebGestorDocumental = "Por favor digite el usuario"
                Exit Function
            End If
            If passs = "" Then
                InicioAplicacionWebGestorDocumental = "Por favor digite la contraseña"
                Exit Function
            End If
            Dim Refclas_ As New GestorModuleSesion.Gestor_conexion
            HttpContext.Current.Session("SESION_STATE") = modulo
            Result = Refclas_.InicializaconexionesModulos(nombre_empresa,
                                                            modulo)
            If Result <> "YES" Then
                InicioAplicacionWebGestorDocumental = Result
                Exit Function
            End If
            HttpContext.Current.Session.Item("EMPRESA_GESTION") = nombre_empresa
            ''--------------------------------------------------------------------------
            ''Asigna el tipo modulo seleccionado
            ''--------------------------------------------------------------------------
            Dim Modulestr As String = ""
            Result = Refclas.Retorna_tipo_modulo(modulo,
                                                 nombre_empresa,
                                                 Modulestr,
                                                 HttpContext.Current.Session("VALIDA_VISOR_EXPRES"))
            If Result <> "YES" Then
                InicioAplicacionWebGestorDocumental = Result
                Exit Function
            End If
            '-------------------------------------------------------------------------
            'Valida usuario de aplicacion
            '-------------------------------------------------------------------------
            Dim ref As New ClassGestorSesion
            HttpContext.Current.Session.Item("TIPOMODULO") = Modulestr
            Dim id_usuario_da As Integer = 0
            Result = ref.ValidaUserAplicacion(user,
                                              passs,
                                              Modulestr,
                                              id_usuario_da)
            If Result <> "YES" Then
                InicioAplicacionWebGestorDocumental = Result
                Exit Function
            End If

            If Modulestr = "DOCUARCHI CONTENEDOR" Then
                '-----------------------------------
                'Retorna login usuario docuarchi
                '-----------------------------------
                Dim Refclasda As New ClassDaIncioDocuarchi
                Result = Refclasda.SolicitaloginUsuarioDocuarchi(id_usuario_da,
                                                                   HttpContext.Current.Session.Item("DA_Login_Usuario"))
                If Result <> "YES" Then
                    InicioAplicacionWebGestorDocumental = Result
                    Exit Function
                End If
                '-----------------------------------
                'Retorna id usuario gestion
                '-----------------------------------
                HttpContext.Current.Session.Item("ID_USUARIO_DOCUARCHI") = id_usuario_da
                '-----------------------------------
                'Retorna grupo usuario docuarchi
                '-----------------------------------
                Dim Refclasinicio As New Class_relacion_usu_grup
                Result = Refclasinicio.SolicitaGrupoRelacionadousuarioDocuarchi(HttpContext.Current.Session.Item("ID_USUARIO_DOCUARCHI"),
                                                                                 HttpContext.Current.Session.Item("DA_gruposusu"))
                If Result <> "YES" Then
                    InicioAplicacionWebGestorDocumental = Result
                    Exit Function
                End If
                Dim id_user_gestion_da As Integer = 0
                Dim Refclasgestor As New ClassGestorDocumental
                Dim id_usuario_gestion_wf As Integer = 0
                Result = Refclasgestor.SolicitaIdUsuarioGestionRelacionUsuarioDocuarchi(id_usuario_da,
                                                                                        id_user_gestion_da)
                If Result <> "YES" Then
                    'InicioAplicacionWebGestorDocumental = Result
                    'Exit Function
                End If

                '-----------------------------------
                'Asigna perfil gestiòn documental
                '-----------------------------------
                Dim ref_Class_remit_dest_interno_perfil_produccion As New Class_remit_dest_interno_perfil_produccion
                If id_user_gestion_da <> 0 Then
                    Result = ref_Class_remit_dest_interno_perfil_produccion.AsignaPermisosPerfilUsuarioGestion(id_user_gestion_da)
                    If Result <> "YES" Then
                        InicioAplicacionWebGestorDocumental = Result
                        Exit Function
                    End If
                    '-------------------------------------
                    'Retorna id empresa usuario gestión
                    '-------------------------------------
                    Dim ref_gestion As New ClassAdmonEmpresa
                    Result = ref_gestion.Retorna_id_empresa_usuario_gestion(HttpContext.Current.Session.Item("GA_IDEMPRESA"),
                                                                            id_user_gestion_da)
                    If Result <> "YES" Then
                        InicioAplicacionWebGestorDocumental = Result
                        Exit Function
                    End If
                    '--------------------------------------
                    'Retorna login usuario de gestion
                    '--------------------------------------
                    Dim ref_gestor_documental As New ClassGestorDocumental
                    Result = ref_gestor_documental.SolicitaLoginUsuarioGestion(id_user_gestion_da,
                                                                                 HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION"))
                    If Result <> "YES" Then
                        InicioAplicacionWebGestorDocumental = Result
                        Exit Function
                    End If
                End If

                '--------------------------------------------------
                'Inserta log de usuario
                '--------------------------------------------------
                Result = Refclasda.RegtraLogSesionUsuarioDocuarchi(id_usuario_da,
                                                                       HttpContext.Current.Session.Item("ip_host_name"),
                                                                       HttpContext.Current.Session.Item("id_registro_sesion_log"))
                If Result <> "YES" Then
                    InicioAplicacionWebGestorDocumental = Result
                    Exit Function
                End If
            End If
            Dim Refclas_inicio As New Class_inicializa_gestion_correspondencia
            Dim mEval As New ClassEdtiScript
            Dim Reflcas As New InicioWorkflow
            If Modulestr = "WORKFLOW DOCUMENTAL" Then
                Result = refclasiniciowf.InicializaSesionModuloWorkflow(user)
                If Result <> "YES" Then
                    InicioAplicacionWebGestorDocumental = Result
                    Exit Function
                End If
                '--------------------------------------------------
                'Compila escript
                '--------------------------------------------------
                HttpContext.Current.Session("SESIONCOMPILAR") = Reflcas.CompilaScriptUsuario(HttpContext.Current.Session("Id_Grupo_Workflow"),
                                                                                               mEval)
                '-----------------------------------
                'Retorna id usuario gestion
                '-----------------------------------
                Dim Refclasgestor As New ClassGestorDocumental
                Dim id_usuario_gestion_wf As Integer = 0
                Result = Refclasgestor.SolicitaIdUsuarioGestionRelacionadoUsuarioWorkflow(HttpContext.Current.Session.Item("Id_Usuario_Workflow"),
                                                                                         id_usuario_gestion_wf)
                If Result <> "YES" Then
                    InicioAplicacionWebGestorDocumental = "YES"
                    Exit Function

                End If
                '-----------------------------------------------------
                'Retorna id usuario docuarchi de workflow relacionado 
                '-----------------------------------------------------
                Result = Refclasgestor.SolicitaIdUsuarioDocuarchiRelacionadoUsuarioGestion(id_usuario_gestion_wf,
                                                                                           HttpContext.Current.Session.Item("ID_USUARIO_DOCUARCHI"))
                If Result <> "YES" Then
                    InicioAplicacionWebGestorDocumental = Result
                    'Exit Function
                End If
                '-----------------------------------
                'Retorna login usuario docuarchi
                '-----------------------------------
                Dim Refclasda As New ClassDaIncioDocuarchi
                Result = Refclasda.SolicitaloginUsuarioDocuarchi(HttpContext.Current.Session.Item("ID_USUARIO_DOCUARCHI"),
                                                                   HttpContext.Current.Session.Item("DA_Login_Usuario"))
                If Result <> "YES" Then
                    InicioAplicacionWebGestorDocumental = Result
                    Exit Function
                End If
                '-----------------------------------
                'Retorna grupo usuario docuarchi
                '-----------------------------------
                Dim Refclasinicio As New Class_relacion_usu_grup
                Result = Refclasinicio.SolicitaGrupoRelacionadousuarioDocuarchi(HttpContext.Current.Session.Item("ID_USUARIO_DOCUARCHI"),
                                                                                HttpContext.Current.Session.Item("DA_gruposusu"))
                If Result <> "YES" Then
                    InicioAplicacionWebGestorDocumental = Result
                    Exit Function
                End If
                '-----------------------------------------------------
                'Retrona la identificacion de usuario de gestion
                '-----------------------------------------------------
                Result = Refclasgestor.SolicitaIdUsuarioRadicadorRelacionadoUsuarioGestion(id_usuario_gestion_wf,
                                                                                           HttpContext.Current.Session.Item("RA_ID_USUARIO"))
                If Result <> "YES" Then
                    InicioAplicacionWebGestorDocumental = Result
                    'Exit Function
                End If
                '-----------------------------------
                'Asigna perfil gestiòn documental
                '-----------------------------------
                Dim ref_Class_remit_dest_interno_perfil_produccion As New Class_remit_dest_interno_perfil_produccion
                If id_usuario_gestion_wf <> 0 Then
                    Result = ref_Class_remit_dest_interno_perfil_produccion.AsignaPermisosPerfilUsuarioGestion(id_usuario_gestion_wf)
                    If Result <> "YES" Then
                        InicioAplicacionWebGestorDocumental = Result
                        Exit Function
                    End If
                    '-------------------------------------
                    'Retorna id empresa usuario gestión
                    '-------------------------------------
                    Dim ref_gestion As New ClassAdmonEmpresa
                    Result = ref_gestion.Retorna_id_empresa_usuario_gestion(HttpContext.Current.Session.Item("GA_IDEMPRESA"),
                                                                            id_usuario_gestion_wf)
                    If Result <> "YES" Then
                        InicioAplicacionWebGestorDocumental = Result
                        Exit Function
                    End If
                    '--------------------------------------
                    'Retorna login usuario de gestion
                    '--------------------------------------
                    Dim ref_gestor_documental As New ClassGestorDocumental
                    Result = ref_gestor_documental.SolicitaLoginUsuarioGestion(id_usuario_gestion_wf,
                                                                                 HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION"))
                    If Result <> "YES" Then
                        InicioAplicacionWebGestorDocumental = Result
                        Exit Function
                    End If
                End If

                Result = Refclas_inicio.Inicializa_gestion_correspondencia(HttpContext.Current.Session.Item("Id_Usuario_Workflow"),
                                                                           HttpContext.Current.Session.Item("Id_Ruta_Workflow"),
                                                                           HttpContext.Current.Session.Item("Id_Grupo_Workflow"))
                If Result <> "YES" Then
                    'InicioAplicacionWebGestorDocumental = Result
                    'Exit Function
                End If
            End If

            If Modulestr = "RADICACION DOCUMENTAL" Then
                Dim id_usuario_gestion_ra As Integer = 0
                Dim id_usuario_login As String = ""
                Dim refclas2 As New ClassInicioRadicador
                Result = refclas2.Inicializa_Radicador(user, HttpContext.Current.Session.Item("RA_ID_USUARIO"), id_usuario_gestion_ra, id_usuario_login)
                If Result <> "YES" Then
                    InicioAplicacionWebGestorDocumental = "YES"
                    HttpContext.Current.Session.Item("DETALLE_SESION") = HttpContext.Current.Session.Item("DETALLE_SESION") & "Usuario gestión=" & Result & vbCrLf
                    Exit Function
                End If
                Dim refclas_gestor_documental As New ClassGestorDocumental
                Result = refclas_gestor_documental.SolicitaIdUsuarioDocuarchiRelacionadoUsuarioGestion(id_usuario_gestion_ra, HttpContext.Current.Session.Item("ID_USUARIO_DOCUARCHI"))
                If Result <> "YES" Then
                    InicioAplicacionWebGestorDocumental = "YES"
                    HttpContext.Current.Session.Item("DETALLE_SESION") = HttpContext.Current.Session.Item("DETALLE_SESION") & "Usuario gestión=" & Result & vbCrLf
                    Exit Function
                End If
                HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") = id_usuario_gestion_ra
                '-------------------------------------
                'Retorna id empresa usuario gestión
                '-------------------------------------
                Dim ref_gestion As New ClassAdmonEmpresa
                Result = ref_gestion.Retorna_id_empresa_usuario_gestion(HttpContext.Current.Session.Item("GA_IDEMPRESA"), id_usuario_gestion_ra)
                If Result <> "YES" Then
                    InicioAplicacionWebGestorDocumental = Result
                    Exit Function
                End If

                '--------------------------------------
                'Retorna login usuario de gestion
                '--------------------------------------
                Dim ref_gestor_documental As New ClassGestorDocumental
                Result = ref_gestor_documental.SolicitaLoginUsuarioGestion(id_usuario_gestion_ra, HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION"))
                If Result <> "YES" Then
                    InicioAplicacionWebGestorDocumental = Result
                    Exit Function
                End If
                '-----------------------------------
                'Asigna perfil gestiòn documental
                '-----------------------------------
                Dim ref_Class_remit_dest_interno_perfil_produccion As New Class_remit_dest_interno_perfil_produccion
                Result = ref_Class_remit_dest_interno_perfil_produccion.AsignaPermisosPerfilUsuarioGestion(id_usuario_gestion_ra)
                If Result <> "YES" Then
                    InicioAplicacionWebGestorDocumental = Result
                    Exit Function
                End If
                '-----------------------------------------------------
                'Retorna id usuario docuarchi de workflow relacionado 
                '-----------------------------------------------------
                Result = refclas_gestor_documental.SolicitaIdUsuarioDocuarchiRelacionadoUsuarioGestion(id_usuario_gestion_ra, HttpContext.Current.Session.Item("ID_USUARIO_DOCUARCHI"))
                If Result <> "YES" Then
                    InicioAplicacionWebGestorDocumental = Result
                    'Exit Function
                End If
                '-----------------------------------
                'Retorna login usuario docuarchi
                '-----------------------------------
                Dim Refclasda As New ClassDaIncioDocuarchi
                Result = Refclasda.SolicitaloginUsuarioDocuarchi(HttpContext.Current.Session.Item("ID_USUARIO_DOCUARCHI"), HttpContext.Current.Session.Item("DA_Login_Usuario"))
                If Result <> "YES" Then
                    InicioAplicacionWebGestorDocumental = Result
                    Exit Function
                End If
                '-----------------------------------
                'Retorna grupo usuario docuarchi
                '-----------------------------------
                Dim Refclasinicio As New Class_relacion_usu_grup
                Result = Refclasinicio.SolicitaGrupoRelacionadousuarioDocuarchi(HttpContext.Current.Session.Item("ID_USUARIO_DOCUARCHI"),
                                                                       HttpContext.Current.Session.Item("DA_gruposusu"))
                If Result <> "YES" Then
                    InicioAplicacionWebGestorDocumental = Result
                    Exit Function
                End If
                '--------------------------------------------------
                'Inserta log de usuario
                '--------------------------------------------------
                Result = refclas2.RegistraLogSesionUsuarioRadicador(HttpContext.Current.Session.Item("RA_ID_USUARIO"), HttpContext.Current.Session.Item("ip_host_name"), HttpContext.Current.Session.Item("id_registro_sesion_log_ra"))
                If Result <> "YES" Then
                    InicioAplicacionWebGestorDocumental = Result
                    Exit Function
                End If
            End If

            Dim id_usuario_gestion As Integer = 0
            Dim login_usuario_workflow As String = ""
            Dim id_usuario_workflow As Integer = 0
            Dim login_usuario_docuarchi As String = ""
            Dim id_usuario_docuarchi As Integer = 0
            Dim login_usuario_radicacion As String = ""
            Dim id_usuario_radicacion As Integer = 0
            If Modulestr = "GESTOR DOCUMENTAL" Then
                '---------------------------------
                'Asigna los datos de relacion del
                'perfil de usuario gestion
                'con los modulos workflow, docuarchi,
                'y radicacion
                '---------------------------------
                Result = refclasgestiondocumental.SolicitaDatosUsuarioGestionLogin(user,
                                                                                      id_usuario_gestion,
                                                                                      login_usuario_workflow,
                                                                                      id_usuario_workflow,
                                                                                      login_usuario_docuarchi,
                                                                                      id_usuario_docuarchi,
                                                                                      login_usuario_radicacion,
                                                                                      id_usuario_radicacion)
                If Result <> "YES" Then
                    InicioAplicacionWebGestorDocumental = Result
                    Exit Function
                End If
                HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") = UCase(user)
                '-----------------------------------
                'Asigna perfil gestiòn documental
                '-----------------------------------
                Dim ref_Class_remit_dest_interno_perfil_produccion As New Class_remit_dest_interno_perfil_produccion
                Result = ref_Class_remit_dest_interno_perfil_produccion.AsignaPermisosPerfilUsuarioGestion(id_usuario_gestion)
                If Result <> "YES" Then
                    InicioAplicacionWebGestorDocumental = Result
                    Exit Function
                End If
                '-------------------------------------
                'Retorna id empresa usuario gestión
                '-------------------------------------
                Dim ref_gestion As New ClassAdmonEmpresa
                Result = ref_gestion.Retorna_id_empresa_usuario_gestion(HttpContext.Current.Session.Item("GA_IDEMPRESA"),
                                                                        id_usuario_gestion)
                If Result <> "YES" Then
                    InicioAplicacionWebGestorDocumental = Result
                    Exit Function
                End If
                '----------------------------------------------
                'Asigna perfil radicación
                '----------------------------------------------
                If id_usuario_radicacion <> 0 Then
                    HttpContext.Current.Session.Item("RA_ID_USUARIO") = id_usuario_radicacion
                End If
                HttpContext.Current.Session.Item("ID_USUARIO_DOCUARCHI") = id_usuario_docuarchi
                '-----------------------------------
                'Retorna login usuario docuarchi
                '-----------------------------------
                Dim Refclasda As New ClassDaIncioDocuarchi
                Result = Refclasda.SolicitaloginUsuarioDocuarchi(HttpContext.Current.Session.Item("ID_USUARIO_DOCUARCHI"),
                                                                   HttpContext.Current.Session.Item("DA_Login_Usuario"))
                If Result <> "YES" Then
                    InicioAplicacionWebGestorDocumental = Result
                    Exit Function
                End If
                '-----------------------------------
                'Retorna grupo usuario docuarchi
                '-----------------------------------
                Dim Refclasinicio As New Class_relacion_usu_grup
                Result = Refclasinicio.SolicitaGrupoRelacionadousuarioDocuarchi(HttpContext.Current.Session.Item("ID_USUARIO_DOCUARCHI"),
                                                                                HttpContext.Current.Session.Item("DA_gruposusu"))
                If Result <> "YES" Then
                    InicioAplicacionWebGestorDocumental = Result
                    Exit Function
                End If
                '---------------------------------------------
                'Inicializa workflow
                '---------------------------------------------
                If id_usuario_workflow <> 0 Then
                    Result = refclasiniciowf.InicializaSesionModuloWorkflow(login_usuario_workflow)
                    If Result <> "YES" Then
                        InicioAplicacionWebGestorDocumental = Result
                        Exit Function
                    End If
                    '--------------------------------------------------
                    'Compila escript
                    '--------------------------------------------------
                    HttpContext.Current.Session("SESIONCOMPILAR") = Reflcas.CompilaScriptUsuario(HttpContext.Current.Session("Id_Grupo_Workflow"),
                                                                                                mEval)
                End If

                '--------------------------------------------------
                'Inserta log de usuario
                '--------------------------------------------------
                Dim ref_Class_log_usuario_gestion As New Class_log_usuario_gestion
                Result = ref_Class_log_usuario_gestion.RegistroSesionLogusuarioGestionDocumental(id_usuario_gestion,
                                                                                                 HttpContext.Current.Session.Item("ip_host_name"),
                                                                                                 HttpContext.Current.Session.Item("id_registro_sesion_log_gd"))
                If Result <> "YES" Then
                    InicioAplicacionWebGestorDocumental = Result
                    Exit Function
                End If
                Result = Refclas_inicio.Inicializa_gestion_correspondencia(HttpContext.Current.Session.Item("Id_Usuario_Workflow"),
                                                                           HttpContext.Current.Session.Item("Id_Ruta_Workflow"),
                                                                           HttpContext.Current.Session.Item("Id_Grupo_Workflow"))
                If Result <> "YES" Then
                    'InicioAplicacionWebGestorDocumental = Result
                    'Exit Function
                End If
            End If
            Dim stru_service() As stru_detalle_web_service = Nothing
            'Result = Me.Gestor_Retorna_Detalle_webserice(HttpContext.Current.Session.Item("EMPRESA_GESTION"), stru_service)
            'If Result <> "YES" Then
            '    InicioAplicacionWebGestorDocumental = Result
            '    Exit Function
            'End If
            If Not stru_service Is Nothing Then
                For i As Integer = 0 To stru_service.Length - 1
                    Select Case stru_service(i).TIPO_MODULO
                        Case "WORKFLOW DOCUMENTAL"
                            HttpContext.Current.Session.Item("ACTIVA_WEB_SERVICE") = stru_service(i).ACTIVA_WEB_SERVICE
                            HttpContext.Current.Session.Item("URL_WEB_SERVICE") = stru_service(i).URL_WEB_SERVICE
                            HttpContext.Current.Session.Item("USER_WEB_SERVICE") = stru_service(i).USER_WEB_SERVICE
                            HttpContext.Current.Session.Item("PASW_WEB_SERVICE") = stru_service(i).PASW_WEB_SERVICE
                        Case "RADICACION DOCUMENTAL"
                            HttpContext.Current.Session.Item("RA_ACTIVA_WEB_SERVICE") = stru_service(i).ACTIVA_WEB_SERVICE
                            HttpContext.Current.Session.Item("RA_URL_WEB_SERVICE") = stru_service(i).URL_WEB_SERVICE
                            HttpContext.Current.Session.Item("RA_USER_WEB_SERVICE") = stru_service(i).USER_WEB_SERVICE
                            HttpContext.Current.Session.Item("RA_PASW_WEB_SERVICE") = stru_service(i).PASW_WEB_SERVICE
                        Case "DOCUARCHI CONTENEDOR"
                            HttpContext.Current.Session.Item("DA_ACTIVA_WEB_SERVICE") = stru_service(i).ACTIVA_WEB_SERVICE
                            HttpContext.Current.Session.Item("DA_URL_WEB_SERVICE") = stru_service(i).URL_WEB_SERVICE
                            HttpContext.Current.Session.Item("DA_USER_WEB_SERVICE") = stru_service(i).USER_WEB_SERVICE
                            HttpContext.Current.Session.Item("DA_PASW_WEB_SERVICE") = stru_service(i).PASW_WEB_SERVICE
                        Case "GESTOR DOCUMENTAL"
                            HttpContext.Current.Session.Item("GA_ACTIVA_WEB_SERVICE") = stru_service(i).ACTIVA_WEB_SERVICE
                            HttpContext.Current.Session.Item("GA_URL_WEB_SERVICE") = stru_service(i).URL_WEB_SERVICE
                            HttpContext.Current.Session.Item("GA_USER_WEB_SERVICE") = stru_service(i).USER_WEB_SERVICE
                            HttpContext.Current.Session.Item("GA_PASW_WEB_SERVICE") = stru_service(i).PASW_WEB_SERVICE
                    End Select
                Next
            End If
            FormsAuthentication.RedirectFromLoginPage(user, False)
            InicioAplicacionWebGestorDocumental = "YES"
        Catch ex As Exception
            InicioAplicacionWebGestorDocumental = "Inconsistencia función InicioAplicacionWebGestorDocumental " & ex.Message
        End Try
    End Function
    Function Recuperar_pasword_usuario(ByVal user As String,
                                       ByRef correo_electronico_usuario As String,
                                       ByVal Nombre_Aplication As String,
                                       ByVal nombre_empresa As String) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim refra As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("workflow_user")
            Dim Sqlstext As String = ""
            Dim refclas_correo As New ClassCorreo
            Dim tipo_modulo As String = ""
            If nombre_empresa = "" Then
                Recuperar_pasword_usuario = "Por favor seleccione la empresa"
                Exit Function
            End If
            If Nombre_Aplication = "" Then
                Recuperar_pasword_usuario = "Por favor seleccione el módulo de aplicación "
                Exit Function
            End If
            If user = "" Then
                Recuperar_pasword_usuario = "Por favor digite el nombre del usuario "
                Exit Function
            End If

            If correo_electronico_usuario = "" Then
                Recuperar_pasword_usuario = "Por favor informe el correo electrónico del usuario "
                Exit Function
            End If
            Dim Refclas As New GestorModuleSesion.Gestor_conexion
            ''--------------------------------------------------------------------------
            ''Asigna el tipo modulo seleccionado
            ''--------------------------------------------------------------------------
            Dim Modulestr As String = ""
            Result = Refclas.Retorna_tipo_modulo(Nombre_Aplication,
                                                 nombre_empresa,
                                                 tipo_modulo,
                                                 HttpContext.Current.Session("VALIDA_VISOR_EXPRES"))
            If Result <> "YES" Then
                Recuperar_pasword_usuario = Result
                Exit Function
            End If
            Result = Refclas.inicializa_conexiones_modulos_recupera_pasw(nombre_empresa,
                                                                        Nombre_Aplication)
            If Result <> "YES" Then
                Recuperar_pasword_usuario = Result
                Exit Function
            End If
            '------------------------------------------------------------------
            'Valida usuario docuarchi.net DOCUARCHI CONTENEDOR
            '------------------------------------------------------------------
            If tipo_modulo = "DOCUARCHI CONTENEDOR" Then
                Sqlstext = "select pasw_encript,Clave_Usuario,correo from usuarios_da where idusuario = '" & user &
                "'"
                Result = refra.SELECTION_SELECT_FIELD(Sqlstext, Datset)
                If Result <> "YES" Then
                    Recuperar_pasword_usuario = "Función Recuperar_pasword_usuario " & Result.ToString
                    Exit Function
                End If
                '***************************************
                'Envia el correo al usuario
                '***************************************
                If Datset.Tables(0).Rows.Count = 0 Then
                    Recuperar_pasword_usuario = "El usuario informado no se cuentra registrado en el modulo (" & Nombre_Aplication & ") , imposible recuperar la contraseña "
                    Exit Function
                Else
                    Dim correo_db As String = ""
                    If Datset.Tables(0).Rows(0).IsNull(2) Then
                        Recuperar_pasword_usuario = "El usuario informado no tiene correo electrónico registrado, imposible recuperar su contraseña "
                        Exit Function
                    Else
                        correo_db = Datset.Tables(0).Rows(0).Item(2)
                    End If
                    If correo_db <> correo_electronico_usuario Then
                        Recuperar_pasword_usuario = "El correo electrónico informado no corresponde al usuario informado, imposible recuperar su contraseña "
                        Exit Function
                    End If
                    Dim paswBsedat As String = Datset.Tables(0).Rows(0).Item(0).ToString
                    Desc_Encript_Value(paswBsedat)
                    Dim matri_mensaje() As String = {"Hemos recuperado su contraseña con los siguientes datos : ", "Usuario : " & user,
                    "Empresa gestión : " & nombre_empresa, "Módulo del sistema : " & Nombre_Aplication, "Contraseña recuperada : " & paswBsedat}
                    Dim subyect As String = "Recuperación de contraseña " & Nombre_Aplication
                    Result = refclas_correo.Envio_Correo_recuperacion_pasword(matri_mensaje, correo_electronico_usuario, subyect)
                    If Result <> "YES" Then
                        Recuperar_pasword_usuario = "Se genero la siguiente inconsistencia enviando a su correo electrónico la contraseña recuperada " & Result.ToString
                        Exit Function
                    End If
                    Recuperar_pasword_usuario = "YES"
                    Exit Function
                End If
            End If
            '-------------------------------------------------------------------
            'Valida usuario gestión
            '-------------------------------------------------------------------
            If tipo_modulo = "GESTOR DOCUMENTAL" Then
                Sqlstext = "select PASW_ENCRIPT,Correo_Electronico from remit_dest_interno where Login_Usuario = '" & user &
                "'"
                Result = refra.SELECTION_SELECT_FIELD(Sqlstext, Datset)
                If Result <> "YES" Then
                    Recuperar_pasword_usuario = "Función Recuperar_pasword_usuario " & Result.ToString
                    Exit Function
                End If
                '***************************************
                'Envia el correo al usuario
                '***************************************
                If Datset.Tables(0).Rows.Count = 0 Then
                    Recuperar_pasword_usuario = "El usuario informado no se cuentra registrado en el modulo (" & Nombre_Aplication & ") , imposible recuperar la contraseña "
                    Exit Function
                Else
                    Dim correo_db As String = ""
                    If Datset.Tables(0).Rows(0).IsNull(1) Then
                        Recuperar_pasword_usuario = "El usuario informado no tiene correo electrónico registrado, imposible recuperar su contraseña "
                        Exit Function
                    Else
                        correo_db = Datset.Tables(0).Rows(0).Item(1)
                    End If
                    If correo_db <> correo_electronico_usuario Then
                        Recuperar_pasword_usuario = "El correo electrónico informado no corresponde al usuario informado, imposible recuperar su contraseña "
                        Exit Function
                    End If
                    Dim paswBsedat As String = Datset.Tables(0).Rows(0).Item(0).ToString
                    Desc_Encript_Value(paswBsedat)
                    Dim matri_mensaje() As String = {"Hemos recuperado su contraseña con los siguientes datos : ", "Usuario : " & user,
                    "Empresa gestión : " & nombre_empresa, "Módulo del sistema : " & Nombre_Aplication, "Contraseña recuperada : " & paswBsedat}
                    Dim subyect As String = "Recuperación de contraseña " & Nombre_Aplication
                    Result = refclas_correo.Envio_Correo_recuperacion_pasword(matri_mensaje, correo_electronico_usuario, subyect)
                    If Result <> "YES" Then
                        Recuperar_pasword_usuario = "Se genero la siguiente inconsistencia enviando a su correo electrónico la contraseña recuperada " & Result.ToString
                        Exit Function
                    End If
                    Recuperar_pasword_usuario = "YES"
                    Exit Function
                End If
            End If
            '-------------------------------------------------------------------
            'Valida usuario radicacion
            '-------------------------------------------------------------------
            If tipo_modulo = "RADICACION DOCUMENTAL" Then
                Sqlstext = "select PASW_ENCRIPT,Correo_Usuario from usuario_radicador where Login_usuario = '" & user &
                "'"
                Result = refra.SELECTION_SELECT_FIELD(Sqlstext, Datset)
                If Result <> "YES" Then
                    Recuperar_pasword_usuario = "Función Recuperar_pasword_usuario " & Result.ToString
                    Exit Function
                End If
                '***************************************
                'Envia el correo al usuario
                '***************************************
                If Datset.Tables(0).Rows.Count = 0 Then
                    Recuperar_pasword_usuario = "El usuario informado no se cuentra registrado en el modulo (" & Nombre_Aplication & ") , imposible recuperar la contraseña "
                    Exit Function
                Else
                    Dim correo_db As String = ""
                    If Datset.Tables(0).Rows(0).IsNull(1) Then
                        Recuperar_pasword_usuario = "El usuario informado no tiene correo electrónico registrado, imposible recuperar su contraseña "
                        Exit Function
                    Else
                        correo_db = Datset.Tables(0).Rows(0).Item(1)
                    End If
                    If correo_db <> correo_electronico_usuario Then
                        Recuperar_pasword_usuario = "El correo electrónico informado no corresponde al usuario informado, imposible recuperar su contraseña "
                        Exit Function
                    End If
                    Dim paswBsedat As String = Datset.Tables(0).Rows(0).Item(0).ToString
                    Desc_Encript_Value(paswBsedat)
                    Dim matri_mensaje() As String = {"Hemos recuperado su contraseña con los siguientes datos : ", "Usuario : " & user,
                    "Empresa gestión : " & nombre_empresa, "Módulo del sistema : " & Nombre_Aplication, "Contraseña recuperada : " & paswBsedat}
                    Dim subyect As String = "Recuperación de contraseña " & Nombre_Aplication
                    Result = refclas_correo.Envio_Correo_recuperacion_pasword(matri_mensaje, correo_electronico_usuario, subyect)
                    If Result <> "YES" Then
                        Recuperar_pasword_usuario = "Se genero la siguiente inconsistencia enviando a su correo electrónico la contraseña recuperada " & Result.ToString
                        Exit Function
                    End If
                    Recuperar_pasword_usuario = "YES"
                    Exit Function
                End If
            End If

            '--------------------------------------------------------------------
            'Valida usuario workflow
            '-------------------------------------------------------------------
            If tipo_modulo = "WORKFLOW DOCUMENTAL" Then
                Sqlstext = "Select pasw_encript,Correo_Usuario from usuario_workflow where login_Usuario='" & user & "'" &
                " and Correo_Usuario='" & correo_electronico_usuario & "'"
                Result = ref.SELECTION_SELECT_FIELD(Sqlstext, Datset)
                If Result <> "YES" Then
                    Recuperar_pasword_usuario = "Función Recuperar_pasword_usuario " & Result.ToString
                    Exit Function
                End If
                '***************************************
                'Envia el correo al usuario
                '***************************************
                If Datset.Tables(0).Rows.Count = 0 Then
                    Recuperar_pasword_usuario = "El usuario informado no se cuentra registrado en el modulo (" & Nombre_Aplication & ") , imposible recuperar la contraseña "
                    Exit Function
                Else
                    Dim correo_db As String = ""
                    If Datset.Tables(0).Rows(0).IsNull(1) Then
                        Recuperar_pasword_usuario = "El usuario informado no tiene correo electrónico registrado, imposible recuperar su contraseña "
                        Exit Function
                    Else
                        correo_db = Datset.Tables(0).Rows(0).Item(1)
                    End If
                    If correo_db <> correo_electronico_usuario Then
                        Recuperar_pasword_usuario = "El correo electrónico informado no corresponde al usuario informado, imposible recuperar su contraseña "
                        Exit Function
                    End If
                    Dim paswBsedat As String = Datset.Tables(0).Rows(0).Item(0).ToString
                    Desc_Encript_Value(paswBsedat)
                    Dim matri_mensaje() As String = {"Hemos recuperado su contraseña con los siguientes datos : ", "Usuario : " & user,
                    "Empresa gestión : " & nombre_empresa, "Módulo del sistema : " & Nombre_Aplication, "Contraseña recuperada : " & paswBsedat}
                    Dim subyect As String = "Recuperación de contraseña " & Nombre_Aplication
                    Result = refclas_correo.Envio_Correo_recuperacion_pasword(matri_mensaje, correo_electronico_usuario, subyect)
                    If Result <> "YES" Then
                        Recuperar_pasword_usuario = "Se genero la siguiente inconsistencia enviando a su correo electrónico la contraseña recuperada " & Result.ToString
                        Exit Function
                    End If
                    Recuperar_pasword_usuario = "YES"
                    Exit Function
                End If
            End If
            If Sqlstext = "" Then
                Recuperar_pasword_usuario = "La aplicacion seleccionada no existe dentro de las licenciadas"
                Exit Function
            End If
            Recuperar_pasword_usuario = "YES"
        Catch ex As Exception
            Recuperar_pasword_usuario = "Inconsistencia General Funcion ValidaUserAplicacion " & ex.Message
        End Try
    End Function
    Function ValidaUserAplicacion(ByVal user As String,
                                  ByRef pasw As String,
                                  ByVal Nombre_Aplication As String,
                                  ByRef id_user As Integer) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim refra As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("workflow_user")
            Dim Sqlstext As String = ""
            '------------------------------------------------------------------
            'Valida usuario docuarchi.net DOCUARCHI CONTENEDOR DA_ENCRIPT_PASW
            '------------------------------------------------------------------
            If Nombre_Aplication = "DOCUARCHI CONTENEDOR" Then
                If HttpContext.Current.Session("DA_ENCRIPT_PASW") = 1 Then
                    Sqlstext = "select pasw_encript,Clave_Usuario from usuarios_da where idusuario = '" & user &
                    "'"
                    Result = refra.SELECTION_SELECT_FIELD(Sqlstext, Datset)
                    If Result <> "YES" Then
                        ValidaUserAplicacion = "Función ValidaUserAplicacion ClassGestorSesion " & Result.ToString
                        Exit Function
                    End If
                    '***************************************
                    'Determina si existe el usuario
                    '***************************************
                    If Datset.Tables(0).Rows.Count = 0 Then
                        ValidaUserAplicacion = "El usuario  no coincide con el módulo " & Nombre_Aplication
                        Exit Function
                    Else
                        Dim paswBsedat As String = Datset.Tables(0).Rows(0).Item(0).ToString
                        id_user = Datset.Tables(0).Rows(0).Item(1).ToString
                        'Desc_Encript_Value(pasw)
                        Desc_Encript_Value(paswBsedat)
                        If pasw = paswBsedat Then
                            ValidaUserAplicacion = "YES"
                            Exit Function
                        Else
                            ValidaUserAplicacion = "La contraseña  no es valida para el módulo " & Nombre_Aplication & " tipo ecripta"
                            Exit Function
                        End If

                    End If
                Else
                    Sqlstext = "select pasword,Clave_Usuario from usuarios_da where idusuario = '" & user &
"'"
                    Result = refra.SELECTION_SELECT_FIELD(Sqlstext, Datset)
                    If Result <> "YES" Then
                        ValidaUserAplicacion = "Función ValidaUserAplicacion ClassGestorSesion " & Result.ToString
                        Exit Function
                    End If
                    '***************************************
                    'Determina si existe el usuario
                    '***************************************
                    If Datset.Tables(0).Rows.Count = 0 Then
                        ValidaUserAplicacion = "El usuario  no coincide con el módulo " & Nombre_Aplication
                        Exit Function
                    Else
                        Dim paswBsedat As String = ""
                        paswBsedat = System.Text.ASCIIEncoding.ASCII.GetString(Datset.Tables(0).Rows(0).Item(0))
                        id_user = Datset.Tables(0).Rows(0).Item(1).ToString
                        If pasw = paswBsedat Then
                            ValidaUserAplicacion = "YES"
                            Exit Function
                        Else
                            ValidaUserAplicacion = "La contraseña  no es valida para el módulo " & Nombre_Aplication
                            Exit Function
                        End If

                    End If
                End If
            End If
            '-------------------------------------------------------------------
            'Valida usuario gestión
            '-------------------------------------------------------------------
            If Nombre_Aplication = "GESTOR DOCUMENTAL" Then
                If HttpContext.Current.Session("RA_ENCRIPT_PASW") = 1 Then
                    Sqlstext = "select PASW_ENCRIPT,Estado_Usuario from remit_dest_interno where Login_Usuario = '" & user &
                    "'"
                    Result = refra.SELECTION_SELECT_FIELD(Sqlstext, Datset)
                    If Result <> "YES" Then
                        ValidaUserAplicacion = "Función ValidaUserAplicacion ClassGestorSesion " & Result.ToString
                        Exit Function
                    End If
                    '***************************************
                    'Determina si existe el usuario
                    '***************************************
                    If Datset.Tables(0).Rows.Count = 0 Then
                        ValidaUserAplicacion = "El usuario  no coincide con el módulo " & Nombre_Aplication
                        Exit Function
                    Else
                        Dim paswBsedat As String = Datset.Tables(0).Rows(0).Item(0).ToString
                        Dim estado_usuario As Integer = Datset.Tables(0).Rows(0).Item(1)
                        If estado_usuario <> 1 Then
                            ValidaUserAplicacion = "El usuario esta bloqueado en la aplicación " & Nombre_Aplication
                            Exit Function
                        End If
                        'Desc_Encript_Value(pasw)
                        Desc_Encript_Value(paswBsedat)
                        If pasw = paswBsedat Then
                            ValidaUserAplicacion = "YES"
                            Exit Function
                        Else
                            ValidaUserAplicacion = "La contraseña  no es valida para el módulo " & Nombre_Aplication
                            Exit Function
                        End If

                    End If
                Else
                    '----------------------------------------------------
                    'Los usuarios se validan sin contraseña encriptada
                    '---------------------------------------------------
                    'Desc_Encript_Value(pasw)
                    Sqlstext = "select Pasw_Usuario,Estado_Usuario from remit_dest_interno where Login_usuario = '" & user & "'"
                    Result = refra.SELECTION_SELECT_FIELD(Sqlstext, Datset)
                    If Result <> "YES" Then
                        ValidaUserAplicacion = "Función ValidaUserAplicacion ClassGestorSeesion dice " & Result.ToString
                        Exit Function
                    End If
                    If Datset.Tables(0).Rows.Count = 0 Then
                        ValidaUserAplicacion = "El usuario no coincide con el módulo " & Nombre_Aplication
                        Exit Function
                    Else
                        Dim paswBsedat As String = Datset.Tables(0).Rows(0).Item(0).ToString
                        Dim estado_usuario As Integer = Datset.Tables(0).Rows(0).Item(1)
                        If estado_usuario <> 1 Then
                            ValidaUserAplicacion = "El usuario esta bloqueado en la aplicación " & Nombre_Aplication
                            Exit Function
                        End If
                        If pasw = paswBsedat Then
                            ValidaUserAplicacion = "YES"
                            Exit Function
                        Else
                            ValidaUserAplicacion = "La contraseña  no coincide con el módulo " & Nombre_Aplication
                            Exit Function
                        End If

                    End If
                End If
            End If
            '-------------------------------------------------------------------
            'Valida usuario radicacion
            '-------------------------------------------------------------------
            If Nombre_Aplication = "RADICACION DOCUMENTAL" Then
                If HttpContext.Current.Session("RA_ENCRIPT_PASW") = 1 Then
                    Sqlstext = "select PASW_ENCRIPT from usuario_radicador where Login_usuario = '" & user &
                    "'"
                    Result = refra.SELECTION_SELECT_FIELD(Sqlstext, Datset)
                    If Result <> "YES" Then
                        ValidaUserAplicacion = "Funcion ValidaUserAplicacion ClassGestorSeesion " & Result.ToString
                        Exit Function
                    End If
                    '***************************************
                    'Determina si existe el usuario
                    '***************************************
                    If Datset.Tables(0).Rows.Count = 0 Then
                        ValidaUserAplicacion = "El usuario  no coincide con el modulo " & Nombre_Aplication
                        Exit Function
                    Else
                        Dim paswBsedat As String = Datset.Tables(0).Rows(0).Item(0).ToString
                        'Desc_Encript_Value(pasw)
                        Desc_Encript_Value(paswBsedat)
                        If pasw = paswBsedat Then
                            ValidaUserAplicacion = "YES"
                            Exit Function
                        Else
                            ValidaUserAplicacion = "La contraseña  no coinciden con el modulo " & Nombre_Aplication
                            Exit Function
                        End If

                    End If
                Else
                    '----------------------------------------------------
                    'Los usuarios se validan con el campo pasw_usuarios
                    '---------------------------------------------------
                    Desc_Encript_Value(pasw)
                    Sqlstext = "select Pasw_Usuario from usuario_radicador where Login_usuario = '" & user & "'"
                    Result = refra.SELECTION_SELECT_FIELD(Sqlstext, Datset)
                    If Result <> "YES" Then
                        ValidaUserAplicacion = "Funcion ValidaUserAplicacion ClassGestorSeesion " & Result.ToString
                        Exit Function
                    End If
                    '***************************************
                    'Determina si existe el usuario
                    '***************************************
                    If Datset.Tables(0).Rows.Count = 0 Then
                        ValidaUserAplicacion = "El usuario  no coincide con el modulo " & Nombre_Aplication
                        Exit Function
                    Else
                        Dim paswBsedat As String = Datset.Tables(0).Rows(0).Item(0).ToString
                        'Desc_Encript_Value(pasw)
                        Desc_Encript_Value(paswBsedat)
                        If pasw = paswBsedat Then
                            ValidaUserAplicacion = "YES"
                            Exit Function
                        Else
                            ValidaUserAplicacion = "La contraseña  no coinciden con el modulo " & Nombre_Aplication
                            Exit Function
                        End If

                    End If
                End If


            End If
            '--------------------------------------------------------------------
            'Valida usuario workflow
            '-------------------------------------------------------------------
            If Nombre_Aplication = "WORKFLOW DOCUMENTAL" Then
                If HttpContext.Current.Session("ENCRIPT_PASW") = 1 Then
                    '**************************************
                    'Consulta el pasword encriptado
                    '**************************************
                    Sqlstext = "Select pasw_encript,ESTADO_USUARIO from usuario_workflow where login_Usuario='" & user & "'"
                    Result = ref.SELECTION_SELECT_FIELD(Sqlstext, Datset)
                    If Result <> "YES" Then
                        ValidaUserAplicacion = "Funcion ValidaUserAplicacion ClassGestorSeesion " & Result.ToString
                        Exit Function
                    End If
                    '***************************************
                    'Determina si existe el usuario
                    '***************************************
                    If Datset.Tables(0).Rows.Count = 0 Then
                        ValidaUserAplicacion = "El usuario  no coincide con el modulo " & Nombre_Aplication
                        Exit Function
                    Else
                        Dim estado_usuario As Integer = Datset.Tables(0).Rows(0).Item(1)
                        If estado_usuario <> 1 Then
                            ValidaUserAplicacion = "El usuario esta bloqueado en la aplicación " & Nombre_Aplication
                            Exit Function
                        End If
                        Dim paswBsedat As String = Datset.Tables(0).Rows(0).Item(0).ToString
                        'Desc_Encript_Value(pasw)
                        Desc_Encript_Value(paswBsedat)
                        If pasw = paswBsedat Then
                            ValidaUserAplicacion = "YES"
                            Exit Function
                        Else
                            ValidaUserAplicacion = "La contraseña  no coincide con el modulo " & Nombre_Aplication
                            Exit Function
                        End If

                    End If

                Else
                    Desc_Encript_Value(pasw)
                    Sqlstext = "Select Pasword_Usuario,ESTADO_USUARIO from usuario_workflow where login_Usuario='" & user & "'"
                    Result = ref.SELECTION_SELECT_FIELD(Sqlstext, Datset)
                    If Result = "YES" Then
                        If Datset.Tables(0).Rows.Count = 0 Then
                            ValidaUserAplicacion = "El usuario  no coincide con el modulo " & Nombre_Aplication
                            Exit Function
                        Else
                            Dim estado_usuario As Integer = Datset.Tables(0).Rows(0).Item(1)
                            If estado_usuario <> 1 Then
                                ValidaUserAplicacion = "El usuario esta bloqueado en la aplicación " & Nombre_Aplication
                                Exit Function
                            End If
                            Dim paswBsedat As String = Datset.Tables(0).Rows(0).Item(0).ToString
                            'Desc_Encript_Value(pasw)
                            Desc_Encript_Value(paswBsedat)
                            If pasw = paswBsedat Then
                                ValidaUserAplicacion = "YES"
                                Exit Function
                            Else
                                ValidaUserAplicacion = "La contraseña  no coincide con el modulo " & Nombre_Aplication
                                Exit Function
                            End If
                        End If
                    Else
                        ValidaUserAplicacion = "Funcion ValidaUserAplicacion ClassGestorSeesion " & Result.ToString
                        Exit Function
                    End If
                End If

            End If
            If Sqlstext = "" Then
                ValidaUserAplicacion = "La aplicacion seleccionada no concide con la registrada en la base de datos"
                Exit Function
            End If
            ValidaUserAplicacion = "YES"
        Catch ex As Exception
            ValidaUserAplicacion = "Inconsistencia General Funcion ValidaUserAplicacion " & ex.Message
        End Try
    End Function
    Function Gestor_Retorna_Detalle_webserice(ByVal Nombre_Empresa As String,
                                              ByRef Detalle_modulos() As stru_detalle_web_service) As String
        '------------------------------------------------------
        'Funcion : Retorna el detalle de los módulos para los
        'servicios web integrados
        'Fecha : 2016-04-22
        'Ing . Miguel Angel Urueta Miranda
        '------------------------------------------------------
        Dim sqlconsulta As String = "SELECT gm.ID_MODULO,gm.ACTIVA_WEB_SERVICE,gm.URL_WEB_SERVICE,gm.USER_WEB_SERVICE,gm.PASW_WEB_SERVICE,gm.TIPO_MODULO FROM empresa_gestion_documental ge  " &
        "INNER JOIN gestor_modulos AS gm on (gm.EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA=ge.ID_EMPRESA)" &
        " WHERE RAZON_SOCIAL_EMPRESA ='" & Nombre_Empresa & "'"
        Dim refra As New conect.Dbase_Conction_Mysql_RA
        Dim Result As String = ""
        Try
            Dim data_set As New DataSet("gestor modulos")
            Result = refra.SELECTION_SELECT_FIELD(sqlconsulta, data_set)
            If Result <> "YES" Then
                Gestor_Retorna_Detalle_webserice = Result
            Else
                If data_set.Tables(0).Rows.Count = 0 Then
                    Gestor_Retorna_Detalle_webserice = "Imposible encontrar la confirguracion web service de los modulos de empresa (" & Nombre_Empresa & ")"
                    Exit Function
                Else
                    For i As Integer = 0 To data_set.Tables(0).Rows.Count - 1
                        ReDim Preserve Detalle_modulos(i)
                        Detalle_modulos(i).id_modulo = data_set.Tables(0).Rows(i).Item(0)
                        Detalle_modulos(i).ACTIVA_WEB_SERVICE = data_set.Tables(0).Rows(i).Item(1)
                        If data_set.Tables(0).Rows(i).IsNull(2) = True Then
                            Detalle_modulos(i).URL_WEB_SERVICE = ""
                        Else
                            Detalle_modulos(i).URL_WEB_SERVICE = data_set.Tables(0).Rows(i).Item(2)
                        End If
                        If data_set.Tables(0).Rows(i).IsNull(3) = True Then
                            Detalle_modulos(i).USER_WEB_SERVICE = ""
                        Else
                            Detalle_modulos(i).USER_WEB_SERVICE = data_set.Tables(0).Rows(i).Item(3)
                        End If
                        If data_set.Tables(0).Rows(i).IsNull(4) = True Then
                            Detalle_modulos(i).PASW_WEB_SERVICE = ""
                        Else
                            Detalle_modulos(i).PASW_WEB_SERVICE = data_set.Tables(0).Rows(i).Item(4)
                        End If
                        If data_set.Tables(0).Rows(i).IsNull(5) = True Then
                            Detalle_modulos(i).TIPO_MODULO = ""
                        Else
                            Detalle_modulos(i).TIPO_MODULO = data_set.Tables(0).Rows(i).Item(5)
                        End If

                    Next
                    Gestor_Retorna_Detalle_webserice = "YES"
                    Exit Function
                End If
            End If

        Catch e As Exception
            Gestor_Retorna_Detalle_webserice = "Funcion SolicitaModulosEmpresa " & e.Message

        End Try

    End Function
End Class
