Imports System
Imports System.Web
Imports MySql.Data.MySqlClient

'Valida y completa el contexto de preview en una sesión Gestión ya autenticada.
'No es un feature gate y no acepta datos del navegador.
Public NotInheritable Class WorkflowPreviewSessionContextGate
    Public Function AsegurarContexto() As ResultadoContextoSesionWorkflow
        Dim resultado As New ResultadoContextoSesionWorkflow With {
            .Contexto = New ContextoModuloWorkflow()
        }
        Dim requestContext As HttpContext = HttpContext.Current
        If requestContext Is Nothing OrElse requestContext.Session Is Nothing Then Return resultado

        Dim contexto As ContextoModuloWorkflow = CrearContexto(requestContext)
        If Not EsSesionGestionAutenticada(requestContext) Then
            If contexto.EsValido() Then
                resultado.Contexto = contexto
                resultado.CadenaConexionWorkflow = CrearCadenaConexion(requestContext)
                If String.IsNullOrWhiteSpace(resultado.CadenaConexionWorkflow) Then
                    resultado.Contexto = New ContextoModuloWorkflow()
                End If
            End If
            Return resultado
        End If

        Dim idUsuarioGestion As Integer = 0
        Dim idUsuarioWorkflowRelacionado As Integer = 0
        Dim idUsuarioDocuarchi As Integer = 0
        Dim idUsuarioRadicacion As Integer = 0
        Dim loginWorkflow As String = String.Empty
        Dim loginDocuarchi As String = String.Empty
        Dim loginRadicacion As String = String.Empty
        Dim loginGestion As String = Convert.ToString(requestContext.Session.Item("GA_LOGINUSUARIOGESTION")).Trim()

        Dim gestion As New ClassGestionDocumental()
        Dim consultaRelacion As String = gestion.SolicitaDatosUsuarioGestionLogin(loginGestion,
            idUsuarioGestion,
            loginWorkflow,
            idUsuarioWorkflowRelacionado,
            loginDocuarchi,
            idUsuarioDocuarchi,
            loginRadicacion,
            idUsuarioRadicacion)
        If Not String.Equals(consultaRelacion, "YES", StringComparison.OrdinalIgnoreCase) OrElse
           idUsuarioWorkflowRelacionado <= 0 OrElse String.IsNullOrWhiteSpace(loginWorkflow) Then
            Return resultado
        End If

        'Esta consulta legacy solo obtiene usuario, ruta y grupo; no registra log ni carga permisos.
        Dim inicioWorkflow As New InicioWorkflow()
        Dim consultaContexto As String = inicioWorkflow.SolicitaIdUsuarIdRutaGrupoWorkflow(loginWorkflow)
        If Not String.Equals(consultaContexto, "YES", StringComparison.OrdinalIgnoreCase) Then Return resultado

        contexto = CrearContexto(requestContext)
        If Not contexto.EsValido() OrElse contexto.IdUsuarioWorkflow <> idUsuarioWorkflowRelacionado Then
            LimpiarContextoWorkflow(requestContext)
            Return resultado
        End If

        requestContext.Session.Item("Login_Usuario_Workfow") = loginWorkflow
        resultado.CadenaConexionWorkflow = CrearCadenaConexion(requestContext)
        If String.IsNullOrWhiteSpace(resultado.CadenaConexionWorkflow) Then
            LimpiarContextoWorkflow(requestContext)
            Return resultado
        End If
        resultado.CadenaConexionDocuarchi = CrearCadenaConexion(requestContext, "DA_")

        resultado.Contexto = contexto
        Return resultado
    End Function

    Private Shared Function EsSesionGestionAutenticada(ByVal requestContext As HttpContext) As Boolean
        Return String.Equals(Convert.ToString(requestContext.Session.Item("TIPOMODULO")).Trim(),
                             "GESTOR DOCUMENTAL",
                             StringComparison.OrdinalIgnoreCase) AndAlso
               Not String.IsNullOrWhiteSpace(Convert.ToString(requestContext.Session.Item("GA_LOGINUSUARIOGESTION")))
    End Function

    Private Shared Function CrearContexto(ByVal requestContext As HttpContext) As ContextoModuloWorkflow
        Dim contexto As New ContextoModuloWorkflow()
        Integer.TryParse(Convert.ToString(requestContext.Session.Item("Id_Usuario_Workflow")), contexto.IdUsuarioWorkflow)
        Integer.TryParse(Convert.ToString(requestContext.Session.Item("Id_Grupo_Workflow")), contexto.IdGrupoWorkflow)
        contexto.LoginUsuario = Convert.ToString(requestContext.Session.Item("Login_Usuario_Workfow")).Trim()
        Return contexto
    End Function

    Private Shared Function CrearCadenaConexion(ByVal requestContext As HttpContext,
                                                Optional ByVal prefijoSesion As String = "") As String
        Dim servidor As String = Convert.ToString(requestContext.Session.Item(prefijoSesion & "IP_SERVER_MODULO")).Trim()
        Dim baseDatos As String = Convert.ToString(requestContext.Session.Item(prefijoSesion & "DB_NAME_MODULO")).Trim()
        Dim usuario As String = Convert.ToString(requestContext.Session.Item(prefijoSesion & "USER_DBMS_MODULO")).Trim()
        Dim clave As String = Convert.ToString(requestContext.Session.Item(prefijoSesion & "PASW_DBMS_MODULO"))
        Dim tipo As String = Convert.ToString(requestContext.Session.Item(prefijoSesion & "TYPE_DBMS_MODULO")).Trim()
        If Not String.Equals(tipo, "mysql", StringComparison.OrdinalIgnoreCase) OrElse
           String.IsNullOrWhiteSpace(servidor) OrElse String.IsNullOrWhiteSpace(baseDatos) OrElse
           String.IsNullOrWhiteSpace(usuario) OrElse String.IsNullOrWhiteSpace(clave) Then
            Return String.Empty
        End If

        Dim builder As New MySqlConnectionStringBuilder With {
            .Server = servidor,
            .Database = baseDatos,
            .UserID = usuario,
            .Password = clave,
            .Pooling = EsHabilitado(requestContext.Session.Item(prefijoSesion & "ACTIVA_POOL_DBMS"))
        }
        Dim maximoPool As Integer = 0
        If Integer.TryParse(Convert.ToString(requestContext.Session.Item(prefijoSesion & "NUMERO_DBMS_CONEX")), maximoPool) AndAlso maximoPool > 0 Then
            builder.MaximumPoolSize = maximoPool
        End If
        Return builder.ConnectionString
    End Function

    Private Shared Function EsHabilitado(ByVal valor As Object) As Boolean
        Dim texto As String = Convert.ToString(valor).Trim()
        Return texto = "1" OrElse String.Equals(texto, "true", StringComparison.OrdinalIgnoreCase) OrElse
               String.Equals(texto, "yes", StringComparison.OrdinalIgnoreCase)
    End Function

    Private Shared Sub LimpiarContextoWorkflow(ByVal requestContext As HttpContext)
        requestContext.Session.Remove("Id_Usuario_Workflow")
        requestContext.Session.Remove("Id_Grupo_Workflow")
        requestContext.Session.Remove("Id_Ruta_Workflow")
        requestContext.Session.Remove("Login_Usuario_Workfow")
    End Sub
End Class

Public NotInheritable Class ResultadoContextoSesionWorkflow
    Public Property Contexto As ContextoModuloWorkflow
    Public Property CadenaConexionWorkflow As String
    Public Property CadenaConexionDocuarchi As String
End Class
