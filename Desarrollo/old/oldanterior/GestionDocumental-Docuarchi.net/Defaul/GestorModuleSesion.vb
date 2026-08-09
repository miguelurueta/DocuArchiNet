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

Module GestorModuleSesion
    Public Class Gestor_conexion
        Dim connectionString As String
        Sub New()
            Dim conf As New NameValueCollection
            Initialize("OdbcMembershipProvider", conf)

        End Sub
        Function Inicializa_conexion_consulta_publica(ByVal nombre_empresa As String) As String
            Dim conn As OdbcConnection = New OdbcConnection(connectionString)
            Try
                conn.Open()
                Dim Result = Me.AsignaAtributosConexionModuloRadicacion("GESTOR DOCUMENTAL",
                                                                nombre_empresa,
                                                                conn)
                If Result <> "YES" Then
                    Inicializa_conexion_consulta_publica = Result
                    conn.Close()
                    Exit Function
                End If
                Result = Me.Gestor_Asigna_detalle_Modulo_Public(
                                                                nombre_empresa,
                                                                "WORKFLOW DOCUMENTAL",
                                                                conn)
                If Result <> "YES" Then
                    Inicializa_conexion_consulta_publica = Result
                    conn.Close()
                    Exit Function
                End If
                Result = Me.AsignaAtributosConexionModuloDocuarchi("DOCUARCHI CONTENEDOR",
                                                            nombre_empresa,
                                                            conn)
                If Result <> "YES" Then
                    Inicializa_conexion_consulta_publica = Result
                    conn.Close()
                    Exit Function
                End If
                Dim ClassGestorSesion As New ClassGestorSesion
                ClassGestorSesion.Asigna_ip_host_cliente()
                Inicializa_conexion_consulta_publica = "YES"
            Catch ex As Exception
                Inicializa_conexion_consulta_publica = "Inconsistencia función Inicializa_conexion_consulta_publica " & ex.Message
            Finally
                conn.Close()
            End Try
        End Function
        Public Sub Initialize(ByVal name As String, ByVal config As NameValueCollection)
            Dim rootWebConfig As System.Configuration.Configuration
            rootWebConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(HttpContext.Current.Request.ApplicationPath)
            Dim connString As System.Configuration.ConnectionStringSettings
            If (0 < rootWebConfig.ConnectionStrings.ConnectionStrings.Count) Then
                connString = rootWebConfig.ConnectionStrings.ConnectionStrings("OdbcServicesGestor")
                If Not (Nothing = connString.ConnectionString) Then
                    connectionString = connString.ConnectionString

                Else
                    Throw New ArgumentNullException("config")
                End If
            End If
        End Sub
        Function InicializaconexionesModulos(ByVal nombre_empresa As String,
                                             ByVal nombre_modulo As String) As String
            Dim conn As OdbcConnection = New OdbcConnection(connectionString)
            Try
                conn.Open()
                Dim Result As String = ""
                Dim Tipo_modulo As String = ""
                Result = Retorna_tipo_modulo(nombre_modulo,
                                             nombre_empresa,
                                             Tipo_modulo,
                                             HttpContext.Current.Session.Item("VALIDA_VISOR_EXPRES"))
                If Result <> "YES" Then
                    InicializaconexionesModulos = Result
                    conn.Close()
                    Exit Function
                End If
                Result = Me.AsignaAtributosConexionModuloRadicacion("GESTOR DOCUMENTAL",
                                                            nombre_empresa,
                                                            conn)
                If Result <> "YES" Then
                    InicializaconexionesModulos = Result
                    conn.Close()
                    Exit Function
                End If
                If Tipo_modulo = "GESTOR DOCUMENTAL" Then
                    Dim id_empresa As Integer = 0
                    Result = Me.Gestor_Retorna_id_empresa(nombre_empresa,
                                                          conn,
                                                          id_empresa)
                    If Result <> "YES" Then
                        InicializaconexionesModulos = Result
                        conn.Close()
                        Exit Function
                    End If
                    Dim id_modulo As Integer = 0
                    Result = Me.Gestor_retorna_id_modulo(nombre_modulo,
                                                         id_empresa,
                                                         Tipo_modulo,
                                                         conn,
                                                         id_modulo)
                    If Result <> "YES" Then
                        InicializaconexionesModulos = Result
                        conn.Close()
                        Exit Function
                    End If
                    Result = Me.AsignaAtributosConexionModuloWorkflowDefaultGestor(nombre_empresa,
                                                                                  "WORKFLOW DOCUMENTAL",
                                                                                  id_modulo,
                                                                                  conn)
                    If Result <> "YES" Then
                        InicializaconexionesModulos = Result
                        conn.Close()
                        Exit Function
                    End If
                Else
                    Result = Me.AsignaAtibutosConexionModuloWorkflow(nombre_modulo,
                                                             nombre_empresa,
                                                             "WORKFLOW DOCUMENTAL",
                                                             conn)
                    If Result <> "YES" Then
                        InicializaconexionesModulos = Result
                        conn.Close()
                        Exit Function
                    End If
                End If

                Result = Me.AsignaAtributosConexionModuloDocuarchi("DOCUARCHI CONTENEDOR",
                                                            nombre_empresa,
                                                            conn)
                If Result <> "YES" Then
                    InicializaconexionesModulos = Result
                    conn.Close()
                    Exit Function
                End If
                conn.Close()
                InicializaconexionesModulos = "YES"
            Catch ex As Exception
                InicializaconexionesModulos = "Inconsistencia función InicializaconexionesModulos " & ex.Message
            Finally
                conn.Close()
            End Try
        End Function
        Function inicializa_conexiones_modulos_recupera_pasw(ByVal nombre_empresa As String,
                                                             ByVal nombre_modulo As String) As String
            Dim conn As OdbcConnection = New OdbcConnection(connectionString)
            Try
                conn.Open()
                Dim Result As String = ""
                Dim Tipo_modulo As String = ""
                Result = Retorna_tipo_modulo(nombre_modulo,
                                             nombre_empresa,
                                             Tipo_modulo,
                                             HttpContext.Current.Session("VALIDA_VISOR_EXPRES"))
                If Result <> "YES" Then
                    inicializa_conexiones_modulos_recupera_pasw = Result
                    conn.Close()
                    Exit Function
                End If
                Result = Me.AsignaAtributosConexionModuloRadicacion("GESTOR DOCUMENTAL",
                                                            nombre_empresa,
                                                            conn)
                If Result <> "YES" Then
                    inicializa_conexiones_modulos_recupera_pasw = Result
                    conn.Close()
                    Exit Function
                End If
                If Tipo_modulo = "GESTOR DOCUMENTAL" Then
                    Dim id_empresa As Integer = 0
                    Result = Me.Gestor_Retorna_id_empresa(nombre_empresa,
                                                          conn,
                                                          id_empresa)
                    If Result <> "YES" Then
                        inicializa_conexiones_modulos_recupera_pasw = Result
                        conn.Close()
                        Exit Function
                    End If
                    Dim id_modulo As Integer = 0
                    Result = Me.Gestor_retorna_id_modulo(nombre_modulo,
                                                         id_empresa,
                                                        Tipo_modulo,
                                                        conn,
                                                        id_modulo)
                    If Result <> "YES" Then
                        inicializa_conexiones_modulos_recupera_pasw = Result
                        conn.Close()
                        Exit Function
                    End If
                    Result = Me.AsignaAtributosConexionModuloWorkflowDefaultGestor(
                                                                              nombre_empresa,
                                                                              "WORKFLOW DOCUMENTAL",
                                                                              id_modulo,
                                                                              conn)
                    If Result <> "YES" Then
                        inicializa_conexiones_modulos_recupera_pasw = Result
                        conn.Close()
                        Exit Function
                    End If
                Else
                    Result = Me.Gestor_Asigna_detalle_Modulo_recupera_pasw(nombre_modulo,
                                                                           nombre_empresa,
                                                                           "WORKFLOW DOCUMENTAL",
                                                                            conn)
                    If Result <> "YES" Then
                        inicializa_conexiones_modulos_recupera_pasw = Result
                        conn.Close()
                        Exit Function
                    End If
                End If

                Result = Me.AsignaAtributosConexionModuloDocuarchi("DOCUARCHI CONTENEDOR",
                                                             nombre_empresa,
                                                             conn)
                If Result <> "YES" Then
                    inicializa_conexiones_modulos_recupera_pasw = Result
                    conn.Close()
                    Exit Function
                End If
                conn.Close()
                inicializa_conexiones_modulos_recupera_pasw = "YES"
            Catch ex As Exception
                inicializa_conexiones_modulos_recupera_pasw = "Inconsistencia función inicializa_conexiones_modulos_recupera_pasw " & ex.Message
            Finally
                conn.Close()
            End Try
        End Function
        Function inicializa_conexiones_modulos_publico() As String
            Dim conn As OdbcConnection = New OdbcConnection(connectionString)
            Try
                conn.Open()
                Dim Result = Me.AsignaAtributosConexionModuloRadicacion("GESTOR DOCUMENTAL",
                                                                HttpContext.Current.Session.Item("EMPRESA_GESTION"),
                                                                conn)
                If Result <> "YES" Then
                    inicializa_conexiones_modulos_publico = Result
                    conn.Close()
                    Exit Function
                End If

                Result = Me.Gestor_Asigna_detalle_Modulo_Public(
                                                                HttpContext.Current.Session.Item("EMPRESA_GESTION"),
                                                                "WORKFLOW DOCUMENTAL", conn)
                If Result <> "YES" Then
                    inicializa_conexiones_modulos_publico = Result
                    conn.Close()
                    Exit Function
                End If
                Result = Me.AsignaAtributosConexionModuloDocuarchi("DOCUARCHI CONTENEDOR",
                                                             HttpContext.Current.Session.Item("EMPRESA_GESTION"),
                                                             conn)
                If Result <> "YES" Then
                    inicializa_conexiones_modulos_publico = Result
                    conn.Close()
                    Exit Function
                End If
                inicializa_conexiones_modulos_publico = "YES"
            Catch ex As Exception
                inicializa_conexiones_modulos_publico = "Inconsistencia función InicializaconexionesModulos " & ex.Message
            Finally
                conn.Close()
            End Try
        End Function
        Function Asigna_detalle_inicio_confirmacion(ByVal nombre_empresa As String) As String
            Dim conn As OdbcConnection = New OdbcConnection(connectionString)
            Try
                conn.Open()
                Dim Result = Me.AsignaAtributosConexionModuloRadicacion("GESTOR DOCUMENTAL",
                                                                nombre_empresa, conn)
                If Result <> "YES" Then
                    Asigna_detalle_inicio_confirmacion = Result
                    conn.Close()
                    Exit Function
                End If
                Result = Me.Gestor_Asigna_detalle_Modulo_Public(
                                                                nombre_empresa,
                                                                "WORKFLOW DOCUMENTAL", conn)
                If Result <> "YES" Then
                    Asigna_detalle_inicio_confirmacion = Result
                    conn.Close()
                    Exit Function
                End If
                Result = Me.AsignaAtributosConexionModuloDocuarchi("DOCUARCHI CONTENEDOR",
                                                            nombre_empresa, conn)
                If Result <> "YES" Then
                    Asigna_detalle_inicio_confirmacion = Result
                    conn.Close()
                    Exit Function
                End If
                Asigna_detalle_inicio_confirmacion = "YES"
            Catch ex As Exception
                Asigna_detalle_inicio_confirmacion = "Inconsistencia función Asigna_detalle_inicio_confirmacion " & ex.Message
            Finally
                conn.Close()
            End Try
        End Function



        Function Inicializa_conexiones_publicas_si_sesion(ByVal nombre_empresa As String) As String
            '-------------------------------------------------------------
            'Función : Inicializa las variables de conexión de base datos
            'Ingeniero : Miguel Angel Urueta Miranda
            'Fecha : 2017-08-15
            '-------------------------------------------------------------
            Dim conn As OdbcConnection = New OdbcConnection(connectionString)
            Try
                Dim Result As String = ""
                conn.Open()
                Result = Me.AsignaAtributosConexionModuloDocuarchi("DOCUARCHI CONTENEDOR",
                                                            nombre_empresa,
                                                            conn)
                If Result <> "YES" Then
                    Inicializa_conexiones_publicas_si_sesion = Result
                    conn.Close()
                    Exit Function
                End If
                Result = Me.Gestor_Asigna_detalle_Modulo_por_db_workflow(nombre_empresa,
                                                                         "WORKFLOW DOCUMENTAL",
                                                                         "workflowdocument",
                                                                         conn)
                If Result <> "YES" Then
                    Inicializa_conexiones_publicas_si_sesion = Result
                    conn.Close()
                    Exit Function
                End If
                Inicializa_conexiones_publicas_si_sesion = "YES"
            Catch ex As Exception
                Inicializa_conexiones_publicas_si_sesion = "Inconsistencia función Inicializa_conexiones_publicas_si_sesion " & ex.Message
            Finally
                conn.Close()
            End Try
        End Function
        Function Gestor_Asigna_detalle_Modulo_por_db_workflow(ByVal Nombre_Empresa As String,
                                                              ByVal Tipo_Modulo As String,
                                                              ByVal db_workflow As String,
                                                              ByVal conn As Object) As String

            Dim cmd As OdbcCommand = New OdbcCommand("SELECT gm.ID_MODULO," &
            "gm.EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA,gm.NOMBRE_MODULO,gm.IP_SERVER_MODULO," &
            "gm.DB_NAME_MODULO,gm.USER_DBMS_MODULO,gm.PASW_DBMS_MODULO," &
            "gm.TYPE_DBMS_MODULO,gm.ESTADO_MODULO,gm.DESCRIPCION_MODULO," &
            "gm.NUMERO_DBMS_CONEX, gm.ACTIVA_POOL_DBMS, gm.VERSION_MODULO, gm.ENCRIPT_PASW, " &
            "gm.ACTIVA_WEB_SERVICE, gm.URL_WEB_SERVICE, gm.USER_WEB_SERVICE, gm.PASW_WEB_SERVICE " &
            "FROM empresa_gestion_documental as ge " &
            "INNER JOIN gestor_modulos AS gm on (gm.EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA=" &
            "ge.ID_EMPRESA and gm.TIPO_MODULO='" & Tipo_Modulo & "' and " &
            " DB_NAME_MODULO='" & db_workflow & "'" &
            ") where RAZON_SOCIAL_EMPRESA='" & Nombre_Empresa & "'", conn)
            Dim reader As OdbcDataReader = Nothing
            Try
                reader = cmd.ExecuteReader
                If reader.HasRows = True Then
                    reader.Read()
                    HttpContext.Current.Session("ID_MODULO") = reader.Item("ID_MODULO").ToString
                    HttpContext.Current.Session("ID_EMPRESA") = reader.Item("EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA").ToString
                    HttpContext.Current.Session("NOMBRE_MODULO") = reader.Item("NOMBRE_MODULO").ToString
                    HttpContext.Current.Session("IP_SERVER_MODULO") = reader.Item("IP_SERVER_MODULO").ToString
                    HttpContext.Current.Session("DB_NAME_MODULO") = reader.Item("DB_NAME_MODULO").ToString
                    HttpContext.Current.Session("USER_DBMS_MODULO") = reader.Item("USER_DBMS_MODULO").ToString
                    HttpContext.Current.Session("PASW_DBMS_MODULO") = reader.Item("PASW_DBMS_MODULO").ToString
                    HttpContext.Current.Session("TYPE_DBMS_MODULO") = reader.Item("TYPE_DBMS_MODULO").ToString
                    HttpContext.Current.Session("NUMERO_DBMS_CONEX") = reader.Item("NUMERO_DBMS_CONEX").ToString
                    HttpContext.Current.Session("ACTIVA_POOL_DBMS") = reader.Item("ACTIVA_POOL_DBMS").ToString
                    HttpContext.Current.Session("ENCRIPT_PASW") = reader.Item("ENCRIPT_PASW").ToString
                    HttpContext.Current.Session("ACTIVA_WEB_SERVICE") = reader.Item("ACTIVA_WEB_SERVICE").ToString
                    HttpContext.Current.Session("URL_WEB_SERVICE") = reader.Item("URL_WEB_SERVICE").ToString
                    HttpContext.Current.Session("USER_WEB_SERVICE") = reader.Item("USER_WEB_SERVICE").ToString
                    HttpContext.Current.Session("PASW_WEB_SERVICE") = reader.Item("PASW_WEB_SERVICE").ToString
                Else
                    Gestor_Asigna_detalle_Modulo_por_db_workflow = "Imposible encontrar el módulo workflow por DB, contacte a su administrador"
                    If Not reader Is Nothing Then reader.Close()
                    Exit Function
                End If
                Gestor_Asigna_detalle_Modulo_por_db_workflow = "YES"
            Catch e As OdbcException
                Gestor_Asigna_detalle_Modulo_por_db_workflow = "Funcion Gestor_Asigna_detalle_Modulo_por_db_workflow " + e.Message
            Finally
                If Not reader Is Nothing Then reader.Close()

            End Try
        End Function
        Function Retorna_nombre_empresa(ByVal codigo_empresa As String,
                                        ByRef nombre_empresa As String) As String
            '-----------------------------------------------------------
            'Función : Retorna el nombre de la empresa con el código de 
            'la empresa 
            'Ing : Miguel Angel Urueta Miranda
            'Fecha : 2017-08-15
            '-----------------------------------------------------------
            Dim reader As OdbcDataReader = Nothing
            Dim conn As OdbcConnection = New OdbcConnection(connectionString)
            Try
                Dim cmd As OdbcCommand = New OdbcCommand("SELECT RAZON_SOCIAL_EMPRESA FROM empresa_gestion_documental  " &
                                                  "WHERE CODIGO_CAMARA = ?", conn)
                cmd.Parameters.Add("@CODIGO_CAMARA", OdbcType.VarChar, 10).Value = codigo_empresa
                conn.Open()
                reader = cmd.ExecuteReader
                reader.Read()
                nombre_empresa = reader.Item(0).ToString
                Retorna_nombre_empresa = "YES"
            Catch ex As Exception
                Retorna_nombre_empresa = "Inconsistencia Retorna_nombre_empresa " & ex.Message
            Finally
                If Not reader Is Nothing Then reader.Close()
                conn.Close()
            End Try
        End Function
        Function Gestor_Retorna_id_empresa(ByVal nombre_empresa As String,
                                           ByRef conn As Object,
                                           ByRef id_empresa As String) As String
            '-----------------------------------------------------------
            'Función : Retorna el id de la empresa con el nombre de 
            'la empresa 
            'Ing : Miguel Angel Urueta Miranda
            'Fecha : 2018-03-08
            '-----------------------------------------------------------
            Dim reader As OdbcDataReader = Nothing
            Try

                Dim cmd As OdbcCommand = New OdbcCommand("SELECT ID_EMPRESA FROM empresa_gestion_documental  " &
                                                  "WHERE RAZON_SOCIAL_EMPRESA = ?", conn)
                cmd.Parameters.Add("@RAZON_SOCIAL_EMPRESA", OdbcType.VarChar, 45).Value = nombre_empresa
                reader = cmd.ExecuteReader
                reader.Read()
                id_empresa = reader.Item(0)
                Gestor_Retorna_id_empresa = "YES"
            Catch ex As Exception
                Gestor_Retorna_id_empresa = "Inconsistencia Gestor_Retorna_id_empresa " & ex.Message
            Finally
                If Not reader Is Nothing Then reader.Close()
            End Try
        End Function

        Function Gestor_Retorna_Empresas(ByRef Gestor_Empresas_colection() As String) _
                                  As String

            Dim conn As OdbcConnection = New OdbcConnection(connectionString)
            Dim cmd As OdbcCommand = New OdbcCommand("SELECT RAZON_SOCIAL_EMPRESA FROM empresa_gestion_documental  " &
                                              "WHERE ESTADO_EMPRESA = ?", conn)
            cmd.Parameters.Add("@ESTADO_EMPRESA", OdbcType.Int, 255).Value = 1
            Dim reader As OdbcDataReader = Nothing
            Erase Gestor_Empresas_colection

            Try
                conn.Open()
                reader = cmd.ExecuteReader
                Dim Iconta As Integer = 0
                While reader.Read
                    ReDim Preserve Gestor_Empresas_colection(Iconta)
                    Gestor_Empresas_colection(Iconta) = reader.Item(0).ToString
                    Iconta = Iconta + 1
                End While
                Gestor_Retorna_Empresas = "YES"
            Catch e As OdbcException
                Gestor_Retorna_Empresas = "funct Gestor_Retorna_Empresas" + e.Message
            Finally
                If Not reader Is Nothing Then reader.Close()
                conn.Close()
            End Try


        End Function
        Function Gestor_Retorna_Empresa(ByRef empresa_gestion As String) As String
            Dim conn As OdbcConnection = New OdbcConnection(connectionString)
            Dim cmd As OdbcCommand = New OdbcCommand("SELECT RAZON_SOCIAL_EMPRESA FROM empresa_gestion_documental  " &
                                              "WHERE ESTADO_EMPRESA = ?", conn)
            cmd.Parameters.Add("@ESTADO_EMPRESA", OdbcType.Int, 255).Value = 1
            Dim reader As OdbcDataReader = Nothing
            Try
                conn.Open()
                reader = cmd.ExecuteReader
                reader.Read()
                If reader.HasRows Then
                    empresa_gestion = reader.Item(0).ToString
                    Gestor_Retorna_Empresa = "YES"
                Else
                    empresa_gestion = ""
                    Gestor_Retorna_Empresa = "Contacte al administrador no empresa activa"
                End If
            Catch e As OdbcException
                Gestor_Retorna_Empresa = "funct Gestor_Retorna_Empresa" + e.Message
            Finally
                If Not reader Is Nothing Then reader.Close()
                conn.Close()
            End Try


        End Function
        Function SolicitaModulosEmpresa(ByVal Nombre_Empresa As String,
                                                        ByRef Gestor_Empresas_modulos() As String) As String
            Dim conn As OdbcConnection = New OdbcConnection(connectionString)
            Dim cmd As OdbcCommand = New OdbcCommand("SELECT gm.NOMBRE_MODULO FROM empresa_gestion_documental ge  " &
            "INNER JOIN gestor_modulos AS gm on (gm.EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA=ge.ID_EMPRESA and gm.ESTADO_MODULO=1)" &
            "WHERE RAZON_SOCIAL_EMPRESA = ? ORDER BY gm.DEFAULT_MODULO DESC", conn)
            cmd.Parameters.Add("@RAZON_SOCIAL_EMPRESA", OdbcType.VarChar, 255).Value = Nombre_Empresa
            Dim reader As OdbcDataReader = Nothing
            Erase Gestor_Empresas_modulos
            Try
                conn.Open()
                reader = cmd.ExecuteReader
                Dim Iconta As Integer = 0
                While reader.Read
                    ReDim Preserve Gestor_Empresas_modulos(Iconta)
                    Gestor_Empresas_modulos(Iconta) = reader.Item(0).ToString
                    Iconta = Iconta + 1
                End While
                SolicitaModulosEmpresa = "YES"
            Catch e As OdbcException
                SolicitaModulosEmpresa = "Funcion SolicitaModulosEmpresa " & e.Message
            Finally
                If Not reader Is Nothing Then reader.Close()
                conn.Close()
            End Try

        End Function
        Function Retorna_tipo_modulo(ByVal nombre_modulo As String,
                                     ByVal Nombre_Empresa As String,
                                     ByRef Tipo_Modulo As String,
                                     ByRef activa_visor As Integer) As String

            Dim conn As OdbcConnection = New OdbcConnection(connectionString)
            Dim cmd As OdbcCommand = New OdbcCommand("SELECT gm.TIPO_MODULO,ge.VALIDA_VISOR_EXPRES " &
            "FROM empresa_gestion_documental as ge " &
            "INNER JOIN gestor_modulos AS gm on (gm.EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA=" &
            "ge.ID_EMPRESA and gm.NOMBRE_MODULO='" & nombre_modulo & "')" &
            "where RAZON_SOCIAL_EMPRESA='" & Nombre_Empresa & "'", conn)
            Dim reader As OdbcDataReader = Nothing
            Try
                conn.Open()
                reader = cmd.ExecuteReader
                If reader.HasRows = True Then
                    reader.Read()
                    Tipo_Modulo = reader.Item("TIPO_MODULO").ToString
                    activa_visor = reader.Item("VALIDA_VISOR_EXPRES")
                Else
                    Retorna_tipo_modulo = "El sistema no puede acceder al modulo (" & nombre_modulo & ") de la empresa gestión (" & Nombre_Empresa & "), caducó la sesión por favor actualice la pagina"
                    If Not reader Is Nothing Then reader.Close()
                    conn.Close()
                    Exit Function
                End If
                Retorna_tipo_modulo = "YES"
            Catch e As OdbcException
                Retorna_tipo_modulo = "Funcion Retorna_tipo_modulo " + e.Message
            Finally
                If Not reader Is Nothing Then reader.Close()
                conn.Close()
            End Try
        End Function
        Function AsignaAtibutosConexionModuloWorkflow(ByVal nombre_modulo As String,
                                                      ByVal Nombre_Empresa As String,
                                                      ByVal Tipo_Modulo As String,
                                                      ByRef conn As Object) As String

            Dim cmd As OdbcCommand = New OdbcCommand("SELECT gm.ID_MODULO," &
            "gm.EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA,gm.NOMBRE_MODULO,gm.IP_SERVER_MODULO," &
            "gm.DB_NAME_MODULO,gm.USER_DBMS_MODULO,gm.PASW_DBMS_MODULO," &
            "gm.TYPE_DBMS_MODULO,gm.ESTADO_MODULO,gm.DESCRIPCION_MODULO," &
            "gm.NUMERO_DBMS_CONEX, gm.ACTIVA_POOL_DBMS, gm.VERSION_MODULO, gm.ENCRIPT_PASW, " &
            "gm.ACTIVA_WEB_SERVICE, gm.URL_WEB_SERVICE, gm.USER_WEB_SERVICE, gm.PASW_WEB_SERVICE " &
            "FROM empresa_gestion_documental as ge " &
            "INNER JOIN gestor_modulos AS gm on (gm.EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA=" &
            "ge.ID_EMPRESA and gm.TIPO_MODULO='" & Tipo_Modulo & "' and gm.WF_DEFAULT_GESTOR ='" & nombre_modulo & "')" &
            " where RAZON_SOCIAL_EMPRESA='" & Nombre_Empresa & "'", conn)
            Dim reader As OdbcDataReader = Nothing
            Try
                reader = cmd.ExecuteReader
                If reader.HasRows = True Then
                    reader.Read()
                    HttpContext.Current.Session("ID_MODULO") = reader.Item("ID_MODULO").ToString
                    HttpContext.Current.Session("ID_EMPRESA") = reader.Item("EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA").ToString
                    HttpContext.Current.Session("NOMBRE_MODULO") = reader.Item("NOMBRE_MODULO").ToString
                    HttpContext.Current.Session("IP_SERVER_MODULO") = reader.Item("IP_SERVER_MODULO").ToString
                    HttpContext.Current.Session("DB_NAME_MODULO") = reader.Item("DB_NAME_MODULO").ToString
                    HttpContext.Current.Session("USER_DBMS_MODULO") = reader.Item("USER_DBMS_MODULO").ToString
                    HttpContext.Current.Session("PASW_DBMS_MODULO") = reader.Item("PASW_DBMS_MODULO").ToString
                    HttpContext.Current.Session("TYPE_DBMS_MODULO") = reader.Item("TYPE_DBMS_MODULO").ToString
                    HttpContext.Current.Session("NUMERO_DBMS_CONEX") = reader.Item("NUMERO_DBMS_CONEX").ToString
                    HttpContext.Current.Session("ACTIVA_POOL_DBMS") = reader.Item("ACTIVA_POOL_DBMS").ToString
                    HttpContext.Current.Session("ENCRIPT_PASW") = reader.Item("ENCRIPT_PASW").ToString
                    HttpContext.Current.Session("ACTIVA_WEB_SERVICE") = reader.Item("ACTIVA_WEB_SERVICE").ToString
                    HttpContext.Current.Session("URL_WEB_SERVICE") = reader.Item("URL_WEB_SERVICE").ToString
                    HttpContext.Current.Session("USER_WEB_SERVICE") = reader.Item("USER_WEB_SERVICE").ToString
                    HttpContext.Current.Session("PASW_WEB_SERVICE") = reader.Item("PASW_WEB_SERVICE").ToString

                Else
                    AsignaAtibutosConexionModuloWorkflow = "YES"
                    If Not reader Is Nothing Then reader.Close()
                    Exit Function
                End If
                AsignaAtibutosConexionModuloWorkflow = "YES"
            Catch e As OdbcException
                AsignaAtibutosConexionModuloWorkflow = "Funcion AsignaAtibutosConexionModuloWorkflow" + e.Message
            Finally
                If Not reader Is Nothing Then reader.Close()
            End Try
        End Function
        Function Gestor_Asigna_detalle_Modulo_recupera_pasw(ByVal nombre_modulo As String,
                                                            ByVal Nombre_Empresa As String,
                                                            ByVal Tipo_Modulo As String,
                                                            ByRef conn As Object) As String
            'Dim conn As OdbcConnection = New OdbcConnection(connectionString)
            Dim cmd As OdbcCommand = New OdbcCommand("SELECT gm.ID_MODULO," &
            "gm.EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA,gm.NOMBRE_MODULO,gm.IP_SERVER_MODULO," &
            "gm.DB_NAME_MODULO,gm.USER_DBMS_MODULO,gm.PASW_DBMS_MODULO," &
            "gm.TYPE_DBMS_MODULO,gm.ESTADO_MODULO,gm.DESCRIPCION_MODULO," &
            "gm.NUMERO_DBMS_CONEX, gm.ACTIVA_POOL_DBMS, gm.VERSION_MODULO, gm.ENCRIPT_PASW, " &
            "gm.ACTIVA_WEB_SERVICE, gm.URL_WEB_SERVICE, gm.USER_WEB_SERVICE, gm.PASW_WEB_SERVICE " &
            "FROM empresa_gestion_documental as ge " &
            "INNER JOIN gestor_modulos AS gm on (gm.EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA=" &
            "ge.ID_EMPRESA and gm.TIPO_MODULO='" & Tipo_Modulo & "' and gm.NOMBRE_MODULO ='" & nombre_modulo & "')" &
            " where RAZON_SOCIAL_EMPRESA='" & Nombre_Empresa & "'", conn)
            'cmd.Parameters.Add("@NOMBRE_EMPRESA", OdbcType.VarChar, 255).Value = NombreEmpresa
            Dim reader As OdbcDataReader = Nothing
            'Erase Gestor_Empresas_modulos

            Try
                'conn.Open()
                reader = cmd.ExecuteReader
                If reader.HasRows = True Then
                    reader.Read()

                    HttpContext.Current.Session("ID_MODULO") = reader.Item("ID_MODULO").ToString
                    HttpContext.Current.Session("ID_EMPRESA") = reader.Item("EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA").ToString
                    HttpContext.Current.Session("NOMBRE_MODULO") = reader.Item("NOMBRE_MODULO").ToString
                    HttpContext.Current.Session("IP_SERVER_MODULO") = reader.Item("IP_SERVER_MODULO").ToString
                    HttpContext.Current.Session("DB_NAME_MODULO") = reader.Item("DB_NAME_MODULO").ToString
                    HttpContext.Current.Session("USER_DBMS_MODULO") = reader.Item("USER_DBMS_MODULO").ToString
                    HttpContext.Current.Session("PASW_DBMS_MODULO") = reader.Item("PASW_DBMS_MODULO").ToString
                    HttpContext.Current.Session("TYPE_DBMS_MODULO") = reader.Item("TYPE_DBMS_MODULO").ToString
                    HttpContext.Current.Session("NUMERO_DBMS_CONEX") = reader.Item("NUMERO_DBMS_CONEX").ToString
                    HttpContext.Current.Session("ACTIVA_POOL_DBMS") = reader.Item("ACTIVA_POOL_DBMS").ToString
                    HttpContext.Current.Session("ENCRIPT_PASW") = reader.Item("ENCRIPT_PASW").ToString
                    HttpContext.Current.Session("ACTIVA_WEB_SERVICE") = reader.Item("ACTIVA_WEB_SERVICE").ToString
                    HttpContext.Current.Session("URL_WEB_SERVICE") = reader.Item("URL_WEB_SERVICE").ToString
                    HttpContext.Current.Session("USER_WEB_SERVICE") = reader.Item("USER_WEB_SERVICE").ToString
                    HttpContext.Current.Session("PASW_WEB_SERVICE") = reader.Item("PASW_WEB_SERVICE").ToString

                Else
                    Gestor_Asigna_detalle_Modulo_recupera_pasw = "YES"
                    If Not reader Is Nothing Then reader.Close()
                    Exit Function
                End If
                Gestor_Asigna_detalle_Modulo_recupera_pasw = "YES"
            Catch e As OdbcException
                Gestor_Asigna_detalle_Modulo_recupera_pasw = "Funcion  Gestor_Asigna_detalle_Modulo_recupera_pasw " + e.Message
            Finally
                If Not reader Is Nothing Then reader.Close()

            End Try

        End Function
        Function Gestor_Asigna_detalle_Modulo_Public(
                                                     ByVal Nombre_Empresa As String,
                                                     ByVal Tipo_Modulo As String,
                                                     ByRef conn As Object) As String
            'Dim conn As OdbcConnection = New OdbcConnection(connectionString)
            Dim cmd As OdbcCommand = New OdbcCommand("SELECT gm.ID_MODULO," &
            "gm.EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA,gm.NOMBRE_MODULO,gm.IP_SERVER_MODULO," &
            "gm.DB_NAME_MODULO,gm.USER_DBMS_MODULO,gm.PASW_DBMS_MODULO," &
            "gm.TYPE_DBMS_MODULO,gm.ESTADO_MODULO,gm.DESCRIPCION_MODULO," &
            "gm.NUMERO_DBMS_CONEX, gm.ACTIVA_POOL_DBMS, gm.VERSION_MODULO, gm.ENCRIPT_PASW, " &
            "gm.ACTIVA_WEB_SERVICE, gm.URL_WEB_SERVICE, gm.USER_WEB_SERVICE, gm.PASW_WEB_SERVICE " &
            "FROM empresa_gestion_documental as ge " &
            "INNER JOIN gestor_modulos AS gm on (gm.EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA=" &
            "ge.ID_EMPRESA and gm.TIPO_MODULO='" & Tipo_Modulo & "' and gm.WF_DEFAULT_GESTOR <> " & 0 & ")" &
            " where RAZON_SOCIAL_EMPRESA='" & Nombre_Empresa & "'", conn)
            'cmd.Parameters.Add("@NOMBRE_EMPRESA", OdbcType.VarChar, 255).Value = NombreEmpresa
            Dim reader As OdbcDataReader = Nothing
            'Erase Gestor_Empresas_modulos

            Try
                'conn.Open()
                reader = cmd.ExecuteReader
                If reader.HasRows = True Then
                    reader.Read()

                    HttpContext.Current.Session("ID_MODULO") = reader.Item("ID_MODULO").ToString
                    HttpContext.Current.Session("ID_EMPRESA") = reader.Item("EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA").ToString
                    HttpContext.Current.Session("NOMBRE_MODULO") = reader.Item("NOMBRE_MODULO").ToString
                    HttpContext.Current.Session("IP_SERVER_MODULO") = reader.Item("IP_SERVER_MODULO").ToString
                    HttpContext.Current.Session("DB_NAME_MODULO") = reader.Item("DB_NAME_MODULO").ToString
                    HttpContext.Current.Session("USER_DBMS_MODULO") = reader.Item("USER_DBMS_MODULO").ToString
                    HttpContext.Current.Session("PASW_DBMS_MODULO") = reader.Item("PASW_DBMS_MODULO").ToString
                    HttpContext.Current.Session("TYPE_DBMS_MODULO") = reader.Item("TYPE_DBMS_MODULO").ToString
                    HttpContext.Current.Session("NUMERO_DBMS_CONEX") = reader.Item("NUMERO_DBMS_CONEX").ToString
                    HttpContext.Current.Session("ACTIVA_POOL_DBMS") = reader.Item("ACTIVA_POOL_DBMS").ToString
                    HttpContext.Current.Session("ENCRIPT_PASW") = reader.Item("ENCRIPT_PASW").ToString
                    HttpContext.Current.Session("ACTIVA_WEB_SERVICE") = reader.Item("ACTIVA_WEB_SERVICE").ToString
                    HttpContext.Current.Session("URL_WEB_SERVICE") = reader.Item("URL_WEB_SERVICE").ToString
                    HttpContext.Current.Session("USER_WEB_SERVICE") = reader.Item("USER_WEB_SERVICE").ToString
                    HttpContext.Current.Session("PASW_WEB_SERVICE") = reader.Item("PASW_WEB_SERVICE").ToString

                Else
                    Gestor_Asigna_detalle_Modulo_Public = "YES"
                    If Not reader Is Nothing Then reader.Close()
                    Exit Function
                End If
                Gestor_Asigna_detalle_Modulo_Public = "YES"
            Catch e As OdbcException
                Gestor_Asigna_detalle_Modulo_Public = "Funcion AsignaAtibutosConexionModuloWorkflow" + e.Message
            Finally
                If Not reader Is Nothing Then reader.Close()
            End Try
        End Function
        Function AsignaAtributosConexionModuloWorkflowDefaultGestor(ByVal NombreEmpresa As String,
                                                                    ByVal TipoModulo As String,
                                                                    ByVal ModuloGestorRelacionadoModuloWorkflow As Integer,
                                                                    ByVal conn As Object) As String
            '-----------------------------------------------------------------------------------------------
            'Funcion : Asgina los atributos de conexión del modulo workflow default relacinado al modulo
            '          gestor documental
            '-----------------------------------------------------------------------------------------------
            '                           PARAMETROS  
            '-----------------------------------------------------------------------------------------------
            'NombreEmpresa      : Representa le nombre de la empresa
            'TipoModulo         : Representa el tipo modulo (WORKFLOW DOCUMENTAL-RADICACION DOCUMENTAL
            '                      DOCUARCHI CONTENEDOR-GESTOR DOCUMENTAL-ADMINISTRADOR WORFLOW)
            'ModuloGestorRelacionadoModuloWorkflow : Rerpreseta el identificador del modulo gestor relacionado
            '                                        al modulo workflow en la campos WF_DEFAULT_GESTOR EN LA
            '                                        tabla gestor modulos
            '                                        
            '-----------------------------------------------------------------------------------------------
            '                           RETORNO
            '-----------------------------------------------------------------------------------------------
            '  : Retorna las variables sesion
            '-----------------------------------------------------------------------------------------------
            '                         CARACTERIZACIÓN
            '-----------------------------------------------------------------------------------------------
            'Fecha                 : 2025-04-01
            'Elabora               : Miguel Angel Urueta Miranda
            '------------------------------------------------------------------------------------------------

            Dim cmd As OdbcCommand = New OdbcCommand("SELECT gm.ID_MODULO," &
            "gm.EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA,gm.NOMBRE_MODULO,gm.IP_SERVER_MODULO," &
            "gm.DB_NAME_MODULO,gm.USER_DBMS_MODULO,gm.PASW_DBMS_MODULO," &
            "gm.TYPE_DBMS_MODULO,gm.ESTADO_MODULO,gm.DESCRIPCION_MODULO," &
            "gm.NUMERO_DBMS_CONEX, gm.ACTIVA_POOL_DBMS, gm.VERSION_MODULO, gm.ENCRIPT_PASW, " &
            "gm.ACTIVA_WEB_SERVICE, gm.URL_WEB_SERVICE, gm.USER_WEB_SERVICE, gm.PASW_WEB_SERVICE " &
            "FROM empresa_gestion_documental as ge " &
            "INNER JOIN gestor_modulos AS gm on (gm.EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA=" &
            "ge.ID_EMPRESA and gm.TIPO_MODULO='" & TipoModulo & "' and " &
            " WF_DEFAULT_GESTOR=" & ModuloGestorRelacionadoModuloWorkflow &
            ") where RAZON_SOCIAL_EMPRESA='" & NombreEmpresa & "'", conn)
            Dim reader As OdbcDataReader = Nothing
            Try
                reader = cmd.ExecuteReader
                If reader.HasRows = True Then
                    reader.Read()
                    HttpContext.Current.Session("ID_MODULO") = reader.Item("ID_MODULO").ToString
                    HttpContext.Current.Session("ID_EMPRESA") = reader.Item("EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA").ToString
                    HttpContext.Current.Session("NOMBRE_MODULO") = reader.Item("NOMBRE_MODULO").ToString
                    HttpContext.Current.Session("IP_SERVER_MODULO") = reader.Item("IP_SERVER_MODULO").ToString
                    HttpContext.Current.Session("DB_NAME_MODULO") = reader.Item("DB_NAME_MODULO").ToString
                    HttpContext.Current.Session("USER_DBMS_MODULO") = reader.Item("USER_DBMS_MODULO").ToString
                    HttpContext.Current.Session("PASW_DBMS_MODULO") = reader.Item("PASW_DBMS_MODULO").ToString
                    HttpContext.Current.Session("TYPE_DBMS_MODULO") = reader.Item("TYPE_DBMS_MODULO").ToString
                    HttpContext.Current.Session("NUMERO_DBMS_CONEX") = reader.Item("NUMERO_DBMS_CONEX").ToString
                    HttpContext.Current.Session("ACTIVA_POOL_DBMS") = reader.Item("ACTIVA_POOL_DBMS").ToString
                    HttpContext.Current.Session("ENCRIPT_PASW") = reader.Item("ENCRIPT_PASW").ToString
                    HttpContext.Current.Session("ACTIVA_WEB_SERVICE") = reader.Item("ACTIVA_WEB_SERVICE").ToString
                    HttpContext.Current.Session("URL_WEB_SERVICE") = reader.Item("URL_WEB_SERVICE").ToString
                    HttpContext.Current.Session("USER_WEB_SERVICE") = reader.Item("USER_WEB_SERVICE").ToString
                    HttpContext.Current.Session("PASW_WEB_SERVICE") = reader.Item("PASW_WEB_SERVICE").ToString

                Else
                    AsignaAtributosConexionModuloWorkflowDefaultGestor = "Imposible encontrar el módulo workflow default, contacte a su administrador"
                    If Not reader Is Nothing Then reader.Close()
                    Exit Function
                End If
                AsignaAtributosConexionModuloWorkflowDefaultGestor = "YES"
            Catch e As OdbcException
                AsignaAtributosConexionModuloWorkflowDefaultGestor = "Funcion AsignaAtibutosConexionModuloWorkflow" + e.Message
            Finally
                If Not reader Is Nothing Then reader.Close()

            End Try
        End Function
        Function Gestor_retorna_id_modulo(ByVal nombre_modulo As String,
                                           ByVal id_empresa As Integer,
                                           ByVal tipo_modulo As String,
                                           ByRef conn As Object,
                                           ByRef id_modulo As Integer) As String
            Dim reader As OdbcDataReader = Nothing
            Try
                Dim cmd As OdbcCommand = New OdbcCommand
                cmd.CommandText = "Select ID_MODULO from gestor_modulos where EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA=" & id_empresa &
                    " and NOMBRE_MODULO='" & nombre_modulo & "' and TIPO_MODULO='" & tipo_modulo & "'"
                cmd.Connection = conn
                reader = cmd.ExecuteReader
                If reader.HasRows = True Then
                    reader.Read()
                    id_modulo = reader.Item(0)
                    Gestor_retorna_id_modulo = "YES"
                    Exit Function
                Else
                    Gestor_retorna_id_modulo = "Imposible encontrar la identificación del modulo " & nombre_modulo
                    Exit Function
                End If
            Catch e As OdbcException
                Gestor_retorna_id_modulo = "Funcion Gestor_reotorna_id_modulo" + e.Message
            Finally
                If Not reader Is Nothing Then reader.Close()

            End Try

        End Function
        Public Function AsignaAtributosConexionModuloDocuarchi(ByVal nombre_modulo As String,
                                                        ByVal Nombre_Empresa As String,
                                                        ByRef conn As Object) As String
            'Dim conn As OdbcConnection = New OdbcConnection(connectionString)
            Dim cmd As OdbcCommand = New OdbcCommand("SELECT gm.ID_MODULO," &
            "gm.EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA,gm.NOMBRE_MODULO,gm.IP_SERVER_MODULO," &
            "gm.DB_NAME_MODULO,gm.USER_DBMS_MODULO,gm.PASW_DBMS_MODULO," &
            "gm.TYPE_DBMS_MODULO,gm.ESTADO_MODULO,gm.DESCRIPCION_MODULO," &
            "gm.NUMERO_DBMS_CONEX, gm.ACTIVA_POOL_DBMS, gm.VERSION_MODULO, gm.ENCRIPT_PASW " &
            "FROM empresa_gestion_documental as ge " &
            "INNER JOIN gestor_modulos AS gm on (gm.EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA=" &
            "ge.ID_EMPRESA and gm.TIPO_MODULO='" & nombre_modulo & "')" &
            " where RAZON_SOCIAL_EMPRESA='" & Nombre_Empresa & "'", conn)
            'cmd.Parameters.Add("@NOMBRE_EMPRESA", OdbcType.VarChar, 255).Value = NombreEmpresa
            Dim reader As OdbcDataReader = Nothing
            'Erase Gestor_Empresas_modulos

            Try
                'conn.Open()
                reader = cmd.ExecuteReader

                If reader.HasRows = True Then
                    reader.Read()
                    HttpContext.Current.Session("DA_ID_MODULO") = reader.Item("ID_MODULO").ToString
                    HttpContext.Current.Session("DA_ID_EMPRESA") = reader.Item("EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA").ToString
                    HttpContext.Current.Session("DA_NOMBRE_MODULO") = reader.Item("NOMBRE_MODULO").ToString
                    HttpContext.Current.Session("DA_IP_SERVER_MODULO") = reader.Item("IP_SERVER_MODULO").ToString
                    HttpContext.Current.Session("DA_DB_NAME_MODULO") = reader.Item("DB_NAME_MODULO").ToString
                    HttpContext.Current.Session("DA_USER_DBMS_MODULO") = reader.Item("USER_DBMS_MODULO").ToString
                    HttpContext.Current.Session("DA_PASW_DBMS_MODULO") = reader.Item("PASW_DBMS_MODULO").ToString
                    HttpContext.Current.Session("DA_TYPE_DBMS_MODULO") = reader.Item("TYPE_DBMS_MODULO").ToString
                    HttpContext.Current.Session("DA_NUMERO_DBMS_CONEX") = reader.Item("NUMERO_DBMS_CONEX").ToString
                    HttpContext.Current.Session("DA_ACTIVA_POOL_DBMS") = reader.Item("ACTIVA_POOL_DBMS").ToString
                    HttpContext.Current.Session("DA_ENCRIPT_PASW") = reader.Item("ENCRIPT_PASW").ToString
                Else
                    AsignaAtributosConexionModuloDocuarchi = "YES"
                    If Not reader Is Nothing Then reader.Close()
                    'conn.Close()
                    Exit Function
                End If
                AsignaAtributosConexionModuloDocuarchi = "YES"
                If Not reader Is Nothing Then reader.Close()
                'conn.Close()
            Catch e As OdbcException
                AsignaAtributosConexionModuloDocuarchi = "Funcion AsignaAtibutosConexionModuloWorkflow" + e.Message
            Finally
                If Not reader Is Nothing Then reader.Close()
                'conn.Close()
            End Try


        End Function

        Function AsignaAtributosConexionModuloRadicacion(ByVal nombre_modulo As String,
                                                 ByVal Nombre_Empresa As String,
                                                 ByRef conn As Object) As String
            'Dim conn1 As OdbcConnection = New OdbcConnection(connectionString)
            Dim cmd As OdbcCommand = New OdbcCommand("SELECT gm.ID_MODULO," &
            "gm.EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA,gm.NOMBRE_MODULO,gm.IP_SERVER_MODULO," &
            "gm.DB_NAME_MODULO,gm.USER_DBMS_MODULO,gm.PASW_DBMS_MODULO," &
            "gm.TYPE_DBMS_MODULO,gm.ESTADO_MODULO,gm.DESCRIPCION_MODULO," &
            "gm.NUMERO_DBMS_CONEX, gm.ACTIVA_POOL_DBMS, gm.VERSION_MODULO, gm.ENCRIPT_PASW " &
            "FROM empresa_gestion_documental as ge " &
            "INNER JOIN gestor_modulos AS gm on (gm.EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA=" &
            "ge.ID_EMPRESA and gm.TIPO_MODULO='" & nombre_modulo & "')" &
            " where RAZON_SOCIAL_EMPRESA='" & Nombre_Empresa & "'", conn)
            'cmd.Parameters.Add("@NOMBRE_EMPRESA", OdbcType.VarChar, 255).Value = NombreEmpresa
            'Dim cmd As OdbcCommand = New OdbcCommand("Select * from gestor_empresa", conn1)
            Dim reader1 As OdbcDataReader = Nothing
            'Erase Gestor_Empresas_modulos

            Try
                'conn1.Open()
                reader1 = cmd.ExecuteReader

                If reader1.HasRows = True Then

                    reader1.Read()
                    HttpContext.Current.Session("RA_ID_MODULO") = reader1.Item("ID_MODULO").ToString
                    HttpContext.Current.Session("RA_ID_EMPRESA") = reader1.Item("EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA").ToString
                    HttpContext.Current.Session("RA_NOMBRE_MODULO") = reader1.Item("NOMBRE_MODULO").ToString
                    HttpContext.Current.Session("RA_IP_SERVER_MODULO") = reader1.Item("IP_SERVER_MODULO").ToString
                    HttpContext.Current.Session("RA_DB_NAME_MODULO") = reader1.Item("DB_NAME_MODULO").ToString
                    HttpContext.Current.Session("RA_USER_DBMS_MODULO") = reader1.Item("USER_DBMS_MODULO").ToString
                    HttpContext.Current.Session("RA_PASW_DBMS_MODULO") = reader1.Item("PASW_DBMS_MODULO").ToString
                    HttpContext.Current.Session("RA_TYPE_DBMS_MODULO") = reader1.Item("TYPE_DBMS_MODULO").ToString
                    HttpContext.Current.Session("RA_NUMERO_DBMS_CONEX") = reader1.Item("NUMERO_DBMS_CONEX").ToString
                    HttpContext.Current.Session("RA_ACTIVA_POOL_DBMS") = reader1.Item("ACTIVA_POOL_DBMS").ToString
                    HttpContext.Current.Session("RA_ENCRIPT_PASW") = reader1.Item("ENCRIPT_PASW").ToString
                Else
                    AsignaAtributosConexionModuloRadicacion = "YES"
                    If Not reader1 Is Nothing Then reader1.Close()
                    'conn1.Close()
                    Exit Function
                End If
                If Not reader1 Is Nothing Then reader1.Close()
                'conn1.Close()
                AsignaAtributosConexionModuloRadicacion = "YES"
            Catch e As OdbcException
                AsignaAtributosConexionModuloRadicacion = "Funcion AsignaAtibutosConexionModuloWorkflow" + e.Message
            Finally
                If Not reader1 Is Nothing Then reader1.Close()
                'conn1.Close()
            End Try

        End Function
    End Class



End Module
