Imports System.IO


Public Class ClassInicioRadicador
    Function Cambiar_Contraseña_Ra(ByVal Pawsuno As String,
                                   ByVal paswdos As String) As String
        Try
            '*****************************************************
            'Verifica que los campos contraseña no esten vacios
            '*****************************************************
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("CONFIGURACION_GABINETE")
            Dim Result As String = ""
            If Pawsuno = "" Then
                Cambiar_Contraseña_Ra = "La primera contraseña debe informarse"
                Exit Function
            End If
            If paswdos = "" Then
                Cambiar_Contraseña_Ra = "La segunda contraseña debe informarse"
                Exit Function
            End If
            '******************************************************
            'La contraseña debe tener mas de ocho caracteres 
            '*****************************************************
            If Pawsuno.Length < 8 Then
                Cambiar_Contraseña_Ra = "La contraseña debe tener mínimo 8 caracteres"
                Exit Function
            End If
            '******************************************************
            'Compara las contraseña entre minusculas y mayusculas
            '******************************************************
            Dim compara As Integer = -2
            Dim Srcomuno As String = Pawsuno
            Dim Srcomdos As String = paswdos
            compara = StrComp(Srcomuno, Srcomdos,
             CompareMethod.Binary)
            If compara = 0 Then
            Else
                Cambiar_Contraseña_Ra = "Las contraseñas no coinciden, tenga en cuenta que el sistema diferencia entre minúsculas y mayusculas"
                Exit Function
            End If
            '******************************************************
            'Encriptacion de contraseñas
            '******************************************************
            Dim Contraseña_Encript As String = Pawsuno
            Result = Encrip_Value(Contraseña_Encript)
            If Result <> "YES" Then
                Cambiar_Contraseña_Ra = "Imposible Encriptar la contraseña " & Result
                Exit Function
            End If
            '*******************************************************
            'Actualizacion de la base de datos
            '*******************************************************
            Dim Sqlupdate As String = "Update usuario_radicador   " &
               "set Pasw_Usuario='" & Pawsuno & "'" &
               ", PASW_ENCRIPT='" & Contraseña_Encript & "'" &
               " where id_usuario=" & HttpContext.Current.Session.Item("RA_ID_USUARIO")
            Dim Resultado_Insertar As String = ref.SELECTION_INSERT_COMMAND(Sqlupdate)
            If Resultado_Insertar <> "YES" Then
                Cambiar_Contraseña_Ra = "Funcion update Error : " & Resultado_Insertar
                Exit Function
            End If
            Cambiar_Contraseña_Ra = "YES"
        Catch ex As Exception
            Cambiar_Contraseña_Ra = "Inconsistencia General Función Cambiar_Contraseña_Ra " & ex.Message
        End Try
    End Function
    Function Inicializa_Radicador(ByVal login_usuario As String,
                                  ByRef id_usuario As Integer,
                                  ByRef id_usuario_gestion As Integer,
                                  ByRef id_usuario_login As String) As String
        Try
            Dim Result As String = ""
            Result = SolicitaIdUsuarioRadicadorLoginGestionIdUsuarioGestion(login_usuario,
                                           id_usuario,
                                           id_usuario_gestion,
                                           id_usuario_login)
            If Result <> "YES" Then
                Inicializa_Radicador = Result
                Exit Function
            End If
            Inicializa_Radicador = "YES"
        Catch ex As Exception
            Inicializa_Radicador = "Inconsistencia general funcion Inicializa_Radicador " & ex.Message
        End Try
    End Function
    Function Tri_View(ByRef Tre_v As TreeView,
                      ByVal Permiso_Plantilla_Matri() As plantillas,
                      ByVal matri_plantilla_externas() As String,
                      Optional ByVal lista_gestion As Integer = 0) As String
        'Genera los permisos de radicacion
        '************************************
        'Funcion : Tri_View
        'Ing : Miguel Angel Urueta Miranda
        'Descripcion : Dibuja en tiempo de
        'ejecucion el listado de opciones del
        'cliente dependiendo de los pdermisos
        'asignados en la base de datos
        '*************************************
        Try
            Dim atrrnodeprincipal As New TreeNode
            atrrnodeprincipal.Text = "Correspondencia"
            If HttpContext.Current.Session("RA_PERMISO_RADICADO") = 1 Then
                Dim attrNode As New TreeNode
                attrNode.Text = "Radicar correspondencia"
                If Not Permiso_Plantilla_Matri Is Nothing Then
                    For z2 As Integer = 0 To UBound(Permiso_Plantilla_Matri)
                        'If Permiso_Plantilla_Matri(z2).Permiso_Radicado = 1 Then
                        Dim attrNode1 As New TreeNode
                        attrNode1.Text = Permiso_Plantilla_Matri(z2).nombre_plantilla
                        'attrNode1.SelectedImageIndex = 1
                        'attrNode1.ImageKey = "usuario copia.gif"
                        If Permiso_Plantilla_Matri(z2).tipo_plantilla = "RADICACION ENTRANTE" Then
                            attrNode1.Value = "RADICACION|" & Permiso_Plantilla_Matri(z2).id_plantilla.ToString & "|" & Permiso_Plantilla_Matri(z2).tipo_plantilla & "|" & z2.ToString
                            attrNode1.ShowCheckBox = False
                            attrNode1.ToolTip = "Plantilla de radicación entrante"
                            attrNode1.ImageUrl = "../radicador/imagenes/inbox-in-light.png"
                            attrNode.ChildNodes.Add(attrNode1)
                        End If
                        If Permiso_Plantilla_Matri(z2).tipo_plantilla = "RADICACION SALIENTE" Then
                            attrNode1.Value = "RADICACION|" & Permiso_Plantilla_Matri(z2).id_plantilla.ToString & "|" & Permiso_Plantilla_Matri(z2).tipo_plantilla & "|" & z2.ToString
                            attrNode1.ShowCheckBox = False
                            attrNode1.ToolTip = "Plantilla de radicación saliente"
                            attrNode1.ImageUrl = "../radicador/imagenes/inbox-out-light.png"
                            attrNode.ChildNodes.Add(attrNode1)
                        End If
                        If Permiso_Plantilla_Matri(z2).tipo_plantilla = "RADICACION GUIA" Then
                            'attrNode1.Value = "RADICACION|" & Permiso_Plantilla_Matri(z2).id_plantilla.ToString & "|" & Permiso_Plantilla_Matri(z2).tipo_plantilla & "|" & z2.ToString
                            'AtrrNodeGuia.ChildNodes.Add(attrNode1)
                        End If
                        'End If
                    Next
                    atrrnodeprincipal.ChildNodes.Add(attrNode)
                End If

                'Tre_v.Nodes.Add(attrNode)
            End If
            If HttpContext.Current.Session("RA_PERMISO_RADICADO") = 1 Then
                Dim attrNode As New TreeNode
                attrNode.Text = "Consulta correspondencia"
                attrNode.ToolTip = "Lista las plantillas de consulta entrante y saliente"
                'attrNode.SelectedImageIndex = 1
                'attrNode.ImageKey = "usuario copia.gif
                Dim AtrrNodeEntrante As New TreeNode
                AtrrNodeEntrante.Text = "(Entrante)"
                Dim AtrrNodeSaliente As New TreeNode
                AtrrNodeSaliente.Text = "(Saliente)"
                If Not Permiso_Plantilla_Matri Is Nothing Then
                    For z2 As Integer = 0 To UBound(Permiso_Plantilla_Matri)
                        'If Permiso_Plantilla_Matri(z2).Permiso_Consulta = 1 Then
                        Dim attrNode1 As New TreeNode
                        attrNode1.Text = Permiso_Plantilla_Matri(z2).nombre_plantilla
                        'attrNode1.SelectedImageIndex = 1
                        'attrNode1.ImageKey = "usuario copia.gif"
                        If Permiso_Plantilla_Matri(z2).tipo_plantilla = "RADICACION ENTRANTE" Then
                            attrNode1.Value = "CONSULTA|" & Permiso_Plantilla_Matri(z2).id_plantilla.ToString & "|" & Permiso_Plantilla_Matri(z2).tipo_plantilla & "|" & z2.ToString
                            attrNode1.ToolTip = "Plantilla de radicación entrante"
                            attrNode1.ImageUrl = "../radicador/imagenes/inbox-in-light.png"
                            attrNode.ChildNodes.Add(attrNode1)
                        End If
                        If Permiso_Plantilla_Matri(z2).tipo_plantilla = "RADICACION SALIENTE" Then
                            attrNode1.Value = "CONSULTA|" & Permiso_Plantilla_Matri(z2).id_plantilla.ToString & "|" & Permiso_Plantilla_Matri(z2).tipo_plantilla & "|" & z2.ToString
                            attrNode1.ImageUrl = "../radicador/imagenes/inbox-out-light.png"
                            attrNode1.ToolTip = "Plantilla de radicación saliente"
                            attrNode.ChildNodes.Add(attrNode1)
                        End If
                        If Permiso_Plantilla_Matri(z2).tipo_plantilla = "RADICACION GUIA" Then
                            'attrNode1.Value = "CONSULTA|" & Permiso_Plantilla_Matri(z2).id_plantilla.ToString & "|" & Permiso_Plantilla_Matri(z2).tipo_plantilla & "|" & z2.ToString
                            'AtrrNodeGuia.ChildNodes.Add(attrNode1)
                        End If

                        'End If
                    Next
                    atrrnodeprincipal.ChildNodes.Add(attrNode)
                End If


            End If
            If HttpContext.Current.Session("RA_PERMISO_GESTION_RESPUESTA") <> 0 Then
                Dim attrNode_envio As New TreeNode
                attrNode_envio.Text = "Gestión de respuestas"
                Dim AtrrNodeEntrante_envio As New TreeNode
                AtrrNodeEntrante_envio.Text = "Pendientes por enviar"
                AtrrNodeEntrante_envio.Value = "ENVIOS|" & "PORENVIAR" & "|" & "" & "|" & ""
                AtrrNodeEntrante_envio.ImageUrl = "../radicador/imagenes/shipping-fast-light.png"
                AtrrNodeEntrante_envio.ToolTip = "Listado de correspondencia pendiente por enviar"
                'AtrrNodeEntrante_envio.ImageUrl = "../workflow/imageneswf/iten_list_select.png"
                attrNode_envio.ChildNodes.Add(AtrrNodeEntrante_envio)
                Dim AtrrNodeSaliente_envio As New TreeNode
                AtrrNodeSaliente_envio.Text = "Pendientes por confirmar envío"
                AtrrNodeSaliente_envio.ToolTip = "Listado de corresponencia pendiente por confirmar envío"
                AtrrNodeSaliente_envio.ImageUrl = "../radicador/imagenes/check-light.png"
                AtrrNodeSaliente_envio.Value = "ENVIOS|" & "PORARCHIVAR" & "|" & "" & "|" & ""
                'AtrrNodeSaliente_envio.ImageUrl = "../workflow/imageneswf/iten_list_select.png"
                attrNode_envio.ChildNodes.Add(AtrrNodeSaliente_envio)
                atrrnodeprincipal.ChildNodes.Add(attrNode_envio)
            End If
            'Tre_v.Nodes.Add(attrNode_envio)
            If HttpContext.Current.Session("RA_PERMISO_GESTION_CORRESPONDENCIA") <> 0 Then
                Dim attrNode_gestion_corresp As New TreeNode
                attrNode_gestion_corresp.Text = "Remisión de correspondencia"
                Dim attrNode_remisio As New TreeNode
                attrNode_remisio.Text = "Remisión de correspondencia"
                attrNode_remisio.Value = "REMISION|" & "REMISIONCORRESPONDENCIA" & "|" & "" & "|" & ""
                attrNode_remisio.ImageUrl = "../radicador/imagenes/share-square-light.png"
                attrNode_gestion_corresp.ChildNodes.Add(attrNode_remisio)
                atrrnodeprincipal.ChildNodes.Add(attrNode_gestion_corresp)
            End If
            'Gestion guías
            Dim attrNode_gestion_guias As New TreeNode
            attrNode_gestion_guias.Text = "Gestión guías de envío"
            Dim attrNode_genera_guia As New TreeNode
            attrNode_genera_guia.Text = "Registrar guías"
            attrNode_genera_guia.Value = "GUIAS|" & "REGISTRARGUIA" & "|" & "" & "|" & ""
            attrNode_genera_guia.ImageUrl = "../radicador/imagenes/file-plus-light.png"
            attrNode_gestion_guias.ChildNodes.Add(attrNode_genera_guia)
            Dim attrNode_gestionar_guia As New TreeNode
            attrNode_gestionar_guia.Text = "Gestión de guías"
            attrNode_gestionar_guia.Value = "GUIAS|" & "GESTIONARGUIA" & "|" & "" & "|" & ""
            attrNode_gestionar_guia.ImageUrl = "../radicador/imagenes/file-edit-light.png"
            attrNode_gestion_guias.ChildNodes.Add(attrNode_gestionar_guia)
            Dim attrNode_consulta_guia As New TreeNode
            attrNode_consulta_guia.Text = "Consultar guías"
            attrNode_consulta_guia.Value = "GUIAS|" & "CONSULTAGUIA" & "|" & "" & "|" & ""
            attrNode_consulta_guia.ImageUrl = "../radicador/imagenes/search-regular.png"
            attrNode_gestion_guias.ChildNodes.Add(attrNode_consulta_guia)
            atrrnodeprincipal.ChildNodes.Add(attrNode_gestion_guias)
            'GESTION DE PLANTILLAS EXTERNAS
            Dim attrNode_gestion_Plantillas As New TreeNode
            attrNode_gestion_Plantillas.Text = "Gestión remitentes"
            If Not matri_plantilla_externas Is Nothing Then
                For i As Integer = 0 To matri_plantilla_externas.Length - 1
                    Dim attrNode_plantilla As New TreeNode
                    Dim spli_plantilla() As String = matri_plantilla_externas(i).ToString.Split("¬")
                    attrNode_plantilla.Text = UCase(spli_plantilla(1))
                    attrNode_plantilla.Value = "PLANTILLA|" & spli_plantilla(0) & "|" & spli_plantilla(1) & "|" & ""
                    attrNode_plantilla.ImageUrl = "../radicador/imagenes/file-edit-light.png"
                    attrNode_gestion_Plantillas.ChildNodes.Add(attrNode_plantilla)
                Next
                atrrnodeprincipal.ChildNodes.Add(attrNode_gestion_Plantillas)
            End If
            Dim attrNode_usuario As New TreeNode
            attrNode_usuario.Text = "Usuario"
            Dim attrNode_pasword As New TreeNode
            attrNode_pasword.Text = "Contraseña usuario"
            attrNode_pasword.Value = "USUARIO|" & "CONTRASEÑA" & "|" & "" & "|" & ""
            If lista_gestion = 0 Then
                attrNode_usuario.ChildNodes.Add(attrNode_pasword)
                atrrnodeprincipal.ChildNodes.Add(attrNode_usuario)
            End If
            Dim attrNode_reporte As New TreeNode
            attrNode_reporte.Text = "Reportes"
            Dim attrNode_reporte_usuario As New TreeNode
            attrNode_reporte_usuario.Text = "Reportes gestión"
            attrNode_reporte_usuario.Value = "REPROTES|" & "USUARIO" & "|" & "" & "|" & ""
            attrNode_reporte_usuario.ImageUrl = "../radicador/imagenes/chart-line-light.png"
            attrNode_reporte.ChildNodes.Add(attrNode_reporte_usuario)
            atrrnodeprincipal.ChildNodes.Add(attrNode_reporte)
            Tre_v.Nodes.Add(atrrnodeprincipal)
            Tri_View = "YES"
        Catch ex As Exception
            Tri_View = "Error General Funcion Tri_View : " & ex.Message
        End Try
    End Function
    Function RegistraLogSesionUsuarioRadicador(ByVal id_usuario As Integer,
                                                 ByVal direcion_ip As String,
                                                 ByRef codigo_transaccion As Integer) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim refclas_gestion_fechas As New ClassGestionFechas
            Dim date1al As String = Date.Now
            Dim Result As String = ""
            Result = refclas_gestion_fechas.Formatea_Fecha_Almacenamiento_Time(date1al)
            If Result <> "YES" Then
                RegistraLogSesionUsuarioRadicador = "Imposible formatear fecha " & Result
                Exit Function
            End If
            Dim sql_insert As String = "Insert into log_usuario_radicado (Usuario_Radicador_id_usuario,Fecha_Logueo,Nombre_Equipo) values (" &
                id_usuario & ",'" & date1al & "','" & direcion_ip & "')"
            Result = ref.SELECTION_LAST_INSERT_COMMAND(sql_insert, codigo_transaccion)
            If Result <> "YES" Then
                RegistraLogSesionUsuarioRadicador = Result
                Exit Function
            Else
                RegistraLogSesionUsuarioRadicador = "YES"
                Exit Function
            End If
        Catch ex As Exception
            RegistraLogSesionUsuarioRadicador = "Inconsistencia general función RegistraLogSesionUsuarioRadicador" & ex.Message
        End Try
    End Function
    Function update_log_sesion_usuario_radicador(ByVal id_log As Integer) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim refclas_gestion_fechas As New ClassGestionFechas
            Dim date1al As String = Date.Now
            Dim Result As String = ""
            Result = refclas_gestion_fechas.Formatea_Fecha_Almacenamiento_Time(date1al)
            If Result <> "YES" Then
                update_log_sesion_usuario_radicador = "Imposible formatear fecha " & Result
                Exit Function
            End If
            Dim sql_insert As String = "update log_usuario_radicado set Fecha_Salida=" &
                 "'" & date1al & "'" & " where id_log_usuario=" & id_log
            Result = ref.SELECTION_INSERT_COMMAND(sql_insert)
            If Result <> "YES" Then
                update_log_sesion_usuario_radicador = Result
                Exit Function
            Else
                update_log_sesion_usuario_radicador = "YES"
                Exit Function
            End If
        Catch ex As Exception
            update_log_sesion_usuario_radicador = "Inconsistencia general función update_log_sesion_usuario_radicador " & ex.Message
        End Try
    End Function
    Function SolicitaIdUsuarioRadicadorLoginGestionIdUsuarioGestion(ByVal Login_User As String,
                                   ByRef id_user As Integer,
                                   ByRef id_usuario_gestion As Integer,
                                   ByRef id_usuario_login As String) As String
        '-----------------------------------------------------------------------
        'Funcion : Retorna el id del usuario de radicacion con el parametro
        'login de usuario
        'Fecha : 2014-04-03
        'Ing : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------
        Dim refra As New conect.Dbase_Conction_Mysql_RA
        Dim Result As String = ""
        Try
            Dim Datset As DataSet = New DataSet("radic_")
            Dim Sqlstext As String = "Select id_usuario,Relacion_Gestion,Relacion_Gestion_Login from usuario_radicador where login_usuario='" &
            Login_User & "'"
            Result = refra.SELECTION_SELECT_FIELD(Sqlstext, Datset)
            If Result <> "YES" Then
                SolicitaIdUsuarioRadicadorLoginGestionIdUsuarioGestion = "Funcion SolicitaIdUsuarioRadicadorLoginGestionIdUsuarioGestion ClassInicioworkflow " & Result.ToString
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaIdUsuarioRadicadorLoginGestionIdUsuarioGestion = "Imposible encontrar el id del usuario de radicación"
                Exit Function
            Else
                id_user = Datset.Tables(0).Rows(0).Item(0)
                id_usuario_gestion = Datset.Tables(0).Rows(0).Item(1)
                id_usuario_login = Datset.Tables(0).Rows(0).Item(2)
                HttpContext.Current.Session.Item("RA_LOGIN_USER") = UCase(Login_User)
                SolicitaIdUsuarioRadicadorLoginGestionIdUsuarioGestion = "YES"
                Exit Function
            End If

            SolicitaIdUsuarioRadicadorLoginGestionIdUsuarioGestion = "YES"
        Catch ex As Exception
            SolicitaIdUsuarioRadicadorLoginGestionIdUsuarioGestion = "La funcion SolicitaIdUsuarioRadicadorLoginGestionIdUsuarioGestion retorna " & ex.Message
        End Try
    End Function
    Public Function Crea_Dir_Temporal_ra() As String
        Try
            Dim Ruttempo As String = HttpContext.Current.Server.MapPath(HttpContext.Current.Session.Item("RA_RUTA_TEMPO") + HttpContext.Current.Session.Item("RA_ID_USUARIO").ToString)
            If Directory.Exists(Ruttempo) = False Then
                Directory.CreateDirectory(Ruttempo)
            End If
            '--------------------------------
            'Crea ruta escaneo Session.Item("RA_RUTA_TEMPO") = "../Temp_Impre/"
            '--------------------------------
            Dim ruta_escaner As String = Ruttempo & "\ESCANER"
            If Directory.Exists(ruta_escaner) = False Then
                Directory.CreateDirectory(ruta_escaner)
            End If
            Dim ruta_impresioin As String = Ruttempo & "\IMPRESION"
            If Directory.Exists(ruta_impresioin) = False Then
                Directory.CreateDirectory(ruta_impresioin)
            End If
            Dim ruta_descarga As String = Ruttempo & "\DESCARGA"
            If Directory.Exists(ruta_descarga) = False Then
                Directory.CreateDirectory(ruta_descarga)
            End If
            HttpContext.Current.Session.Item("RA_RUTA_TEMPO_IMPRESION_DESCARGA_ROTULO") = HttpContext.Current.Session.Item("RA_RUTA_TEMPO") + HttpContext.Current.Session.Item("RA_ID_USUARIO").ToString & "/IMPRESION/"
            HttpContext.Current.Session.Item("RA_RUTA_TEMPO_IMPRESION") = ruta_impresioin
            HttpContext.Current.Session.Item("RA_RUTA_TEMPO_ESCANER") = ruta_escaner
            HttpContext.Current.Session.Item("RA_RUTA_TEMPO_DESCARGA") = ruta_descarga
            Crea_Dir_Temporal_ra = "YES"
        Catch EX As Exception
            Crea_Dir_Temporal_ra = EX.Message
        End Try
    End Function

    Function Retorna_tipo_Impresion(ByVal ID_USER_RADICADO As Integer) As String
        '**************************************************************
        'Funcion : Retorna el tipo de impresion para el usuario con 
        'el parametro de id de usuario, asigna a la variable global
        'el valor
        'Ingeniero Miguel Angel Urueta
        'Fecha : 2014-05-17
        '***************************************************************
        Try
            Dim SQL As String = "Select TIPO_IMPRESION from usuario_radicador where id_usuario=" & ID_USER_RADICADO
            Dim refra As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("usuario_radicador")
            Dim Result As String = ""
            Result = refra.SELECTION_SELECT_FIELD(SQL, Datset)
            If Result <> "YES" Then
                Retorna_tipo_Impresion = "La funcion Retorna_tipo_Impresion Cod:1 Dice: " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_tipo_Impresion = "La funcion Retorna_tipo_Impresion Cod:2 Dice: no se encontraron resultados para el tipo de impresion"
                Exit Function
            End If
            HttpContext.Current.Session("RA_TIPO_IMPRESION") = Datset.Tables(0).Rows(0).Item(0)
            Retorna_tipo_Impresion = "YES"
        Catch ex As Exception
            Retorna_tipo_Impresion = "Inconsistencia general Funcion Retorna_tipo_Impresion " & ex.Message
        End Try
    End Function
End Class
