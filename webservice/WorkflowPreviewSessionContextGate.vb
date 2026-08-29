Imports System
Imports System.Web
Imports MySql.Data.MySqlClient

'Valida y completa el contexto de preview en una sesión Gestión ya autenticada.
'No es un feature gate y no acepta datos del navegador.
Public NotInheritable Class WorkflowPreviewSessionContextGate
    'Resuelve el permiso de envio directo a usuario sin aceptar identidad, destino ni permiso del navegador.
    Public Function AsegurarContextoEnvioUsuario(Optional ByVal prepararEjecucion As Boolean = False) As ResultadoContextoSesionWorkflow
        Dim resultado As ResultadoContextoSesionWorkflow = If(prepararEjecucion,
                                                              AsegurarContextoEjecucion(),
                                                              AsegurarContexto())
        If resultado Is Nothing OrElse resultado.Contexto Is Nothing OrElse Not resultado.Contexto.EsValido() Then
            Return If(resultado, New ResultadoContextoSesionWorkflow With {.Contexto = New ContextoModuloWorkflow()})
        End If

        Try
            Dim permisos As String() = Nothing
            Dim resultadoPermisos As String = New Class_permisos_usuarios_workflow().SolicitaPermisosUsuarioWorkflow(
                resultado.Contexto.IdUsuarioWorkflow,
                permisos)
            resultado.Contexto.PuedeCambioUsuario = String.Equals(resultadoPermisos, "YES", StringComparison.OrdinalIgnoreCase) AndAlso
                                                   permisos IsNot Nothing AndAlso permisos.Length > 18 AndAlso
                                                   String.Equals(permisos(18), "1", StringComparison.OrdinalIgnoreCase)
        Catch
            resultado.Contexto.PuedeCambioUsuario = False
        End Try
        Return resultado
    End Function

    'Resuelve el permiso efectivo para la operación directa sin aceptar identidad ni permisos del navegador.
    Public Function AsegurarContextoEnvioGrupo(Optional ByVal prepararEjecucion As Boolean = False) As ResultadoContextoSesionWorkflow
        Dim resultado As ResultadoContextoSesionWorkflow = If(prepararEjecucion,
                                                              AsegurarContextoEjecucion(),
                                                              AsegurarContexto())
        If resultado Is Nothing OrElse resultado.Contexto Is Nothing OrElse Not resultado.Contexto.EsValido() Then
            Return If(resultado, New ResultadoContextoSesionWorkflow With {.Contexto = New ContextoModuloWorkflow()})
        End If

        Try
            Dim permisos As String() = Nothing
            Dim resultadoPermisos As String = New Class_permisos_usuarios_workflow().SolicitaPermisosUsuarioWorkflow(
                resultado.Contexto.IdUsuarioWorkflow,
                permisos)
            resultado.Contexto.PuedeCambioRuta = String.Equals(resultadoPermisos, "YES", StringComparison.OrdinalIgnoreCase) AndAlso
                                               permisos IsNot Nothing AndAlso permisos.Length > 8 AndAlso
                                               String.Equals(permisos(8), "1", StringComparison.OrdinalIgnoreCase)
        Catch
            resultado.Contexto.PuedeCambioRuta = False
        End Try
        Return resultado
    End Function

    'Resuelve el permiso de Notas solo desde la sesión autenticada. No acepta autor, grupo, permiso, tarea ni ruta desde el navegador.
    Public Function AsegurarContextoNotas() As ResultadoContextoSesionWorkflow
        Dim resultado As ResultadoContextoSesionWorkflow = AsegurarContexto()
        If resultado Is Nothing OrElse resultado.Contexto Is Nothing OrElse Not resultado.Contexto.EsValido() Then
            Return If(resultado, New ResultadoContextoSesionWorkflow With {.Contexto = New ContextoModuloWorkflow()})
        End If

        Try
            Dim permisos As String() = Nothing
            Dim resultadoPermisos As String = New Class_permisos_usuarios_workflow().SolicitaPermisosUsuarioWorkflow(
                resultado.Contexto.IdUsuarioWorkflow,
                permisos)
            resultado.Contexto.PuedeInteractuarAnotaciones = String.Equals(resultadoPermisos, "YES", StringComparison.OrdinalIgnoreCase) AndAlso
                                                       permisos IsNot Nothing AndAlso permisos.Length > 9 AndAlso
                                                       String.Equals(permisos(9), "1", StringComparison.OrdinalIgnoreCase)
        Catch
            resultado.Contexto.PuedeInteractuarAnotaciones = False
        End Try
        Return resultado
    End Function

    'Resuelve el permiso efectivo de devolución en el servidor, sin aceptar permisos del navegador.
    Public Function AsegurarContextoDevolverActividad(Optional ByVal prepararEjecucion As Boolean = False) As ResultadoContextoSesionWorkflow
        Dim resultado As ResultadoContextoSesionWorkflow = If(prepararEjecucion,
                                                              AsegurarContextoEjecucion(),
                                                              AsegurarContexto())
        If resultado Is Nothing OrElse resultado.Contexto Is Nothing OrElse Not resultado.Contexto.EsValido() Then
            Return If(resultado, New ResultadoContextoSesionWorkflow With {.Contexto = New ContextoModuloWorkflow()})
        End If

        Try
            Dim permisos As String() = Nothing
            Dim resultadoPermisos As String = New Class_permisos_usuarios_workflow().SolicitaPermisosUsuarioWorkflow(
                resultado.Contexto.IdUsuarioWorkflow,
                permisos)
            resultado.Contexto.PuedeDevolverActividad = String.Equals(resultadoPermisos, "YES", StringComparison.OrdinalIgnoreCase) AndAlso
                                                         permisos IsNot Nothing AndAlso permisos.Length > 43 AndAlso
                                                         String.Equals(permisos(43), "1", StringComparison.OrdinalIgnoreCase)
        Catch
            resultado.Contexto.PuedeDevolverActividad = False
        End Try
        Return resultado
    End Function

    'Resuelve el permiso específico de Usuario anterior de forma fail-closed; no reutiliza permisos de ruta ni datos del navegador.
    Public Function AsegurarContextoDevolverUsuarioAnterior(Optional ByVal prepararEjecucion As Boolean = False) As ResultadoContextoSesionWorkflow
        Dim resultado As ResultadoContextoSesionWorkflow = If(prepararEjecucion,
                                                              AsegurarContextoEjecucion(),
                                                              AsegurarContexto())
        If resultado Is Nothing OrElse resultado.Contexto Is Nothing OrElse Not resultado.Contexto.EsValido() Then
            Return If(resultado, New ResultadoContextoSesionWorkflow With {.Contexto = New ContextoModuloWorkflow()})
        End If
        Try
            Dim permisos As String() = Nothing
            Dim resultadoPermisos As String = New Class_permisos_usuarios_workflow().SolicitaPermisosUsuarioWorkflow(
                resultado.Contexto.IdUsuarioWorkflow,
                permisos)
            resultado.Contexto.PuedeDevolverUsuarioAnterior = String.Equals(resultadoPermisos, "YES", StringComparison.OrdinalIgnoreCase) AndAlso
                                                                 permisos IsNot Nothing AndAlso permisos.Length > 43 AndAlso
                                                                 String.Equals(permisos(43), "1", StringComparison.OrdinalIgnoreCase)
        Catch
            resultado.Contexto.PuedeDevolverUsuarioAnterior = False
        End Try
        Return resultado
    End Function

    Public Function AsegurarContextoEjecucion() As ResultadoContextoSesionWorkflow
        Dim resultado As New ResultadoContextoSesionWorkflow With {
            .Contexto = New ContextoModuloWorkflow()
        }
        Dim requestContext As HttpContext = HttpContext.Current
        If requestContext Is Nothing OrElse requestContext.Session Is Nothing OrElse Not EsSesionGestionAutenticada(requestContext) Then
            Return resultado
        End If

        resultado = AsegurarContexto()
        If resultado.Contexto Is Nothing OrElse Not resultado.Contexto.EsValido() OrElse
           String.IsNullOrWhiteSpace(resultado.CadenaConexionWorkflow) Then
            Return resultado
        End If

        Try
            Dim permisos As String() = Nothing
            Dim resultadoPermisos As String = New Class_permisos_usuarios_workflow().SolicitaPermisosUsuarioWorkflow(
                resultado.Contexto.IdUsuarioWorkflow,
                permisos)
            If Not String.Equals(resultadoPermisos, "YES", StringComparison.OrdinalIgnoreCase) OrElse permisos Is Nothing Then
                LimpiarContextoWorkflow(requestContext)
                Return New ResultadoContextoSesionWorkflow With {.Contexto = New ContextoModuloWorkflow()}
            End If

            Dim nombreRuta As String = String.Empty
            Dim resultadoRuta As String = New Class_worflow_rutas().Solicita_nombre_ruta_por_id_ruta(
                resultado.Contexto.IdRutaWorkflow,
                nombreRuta)
            If Not String.Equals(resultadoRuta, "YES", StringComparison.OrdinalIgnoreCase) OrElse
               Not EsNombreRutaSeguro(nombreRuta) Then
                LimpiarContextoWorkflow(requestContext)
                Return New ResultadoContextoSesionWorkflow With {.Contexto = New ContextoModuloWorkflow()}
            End If
            requestContext.Session.Item("WF_RUTAWORKFLOW") = nombreRuta

            'El login Gestión ya ejecuta InicializaSesionModuloWorkflow y deja el resultado de la compilación.
            'Solo se compila como recuperación de una sesión incompleta; no se reemplazan eventos ya cargados.
            If requestContext.Session.Item("SESIONCOMPILAR") Is Nothing Then
                Dim inicioWorkflow As New InicioWorkflow()
                Dim resultadoEventos As String = inicioWorkflow.CompilaScriptUsuario(
                    resultado.Contexto.IdGrupoWorkflow,
                    inicioWorkflow.mEval)
                If Not String.Equals(resultadoEventos, "YES", StringComparison.OrdinalIgnoreCase) AndAlso
                   Not EsGrupoSinScripts(resultadoEventos) Then
                    LimpiarContextoWorkflow(requestContext)
                    Return New ResultadoContextoSesionWorkflow With {.Contexto = New ContextoModuloWorkflow()}
                End If
                If EsGrupoSinScripts(resultadoEventos) Then
                    requestContext.Session.Item("PRETERMINARACTIVIAD") = String.Empty
                    requestContext.Session.Item("TERMINARACTIVIDAD") = String.Empty
                End If
            End If

            Return resultado
        Catch
            LimpiarContextoWorkflow(requestContext)
            Return New ResultadoContextoSesionWorkflow With {.Contexto = New ContextoModuloWorkflow()}
        End Try
    End Function

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
        contexto.IdUsuarioGestion = idUsuarioGestion
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
        Integer.TryParse(Convert.ToString(requestContext.Session.Item("Id_Ruta_Workflow")), contexto.IdRutaWorkflow)
        Integer.TryParse(Convert.ToString(requestContext.Session.Item("GA_IDUSUARIOGESTION")), contexto.IdUsuarioGestion)
        contexto.LoginUsuario = Convert.ToString(requestContext.Session.Item("Login_Usuario_Workfow")).Trim()
        Return contexto
    End Function

    Private Shared Function EsNombreRutaSeguro(ByVal nombreRuta As String) As Boolean
        Return Not String.IsNullOrWhiteSpace(nombreRuta) AndAlso
               System.Text.RegularExpressions.Regex.IsMatch(nombreRuta, "^[A-Za-z0-9_]+$")
    End Function

    Private Shared Function EsGrupoSinScripts(ByVal resultado As String) As Boolean
        Return Not String.IsNullOrWhiteSpace(resultado) AndAlso
               resultado.IndexOf("Usuario sin script registrados", StringComparison.OrdinalIgnoreCase) >= 0
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
