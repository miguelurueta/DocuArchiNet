
Public Class CdefRues
    Property ErrorAppp As String
    Property CdRues As CdRues
    Property CdRuesCaracterizacion As CdRuesCaracterizacion
    Property NombreEmpresa As String
    Property Gabinete As String
End Class
Public Class CdRues
    Property codigo_camara As String
    Property emailUsuario As String
    Property identificacionusuario As String
    Property nombreUsuario As String
    Property nitEntidad As String
    Property nombreEntidad As String
    Property municipioEntidad As String
    Property tipoRegistro As String
    Property expediente As String
End Class
Public Class CdRuesCaracterizacion
    Property NitIdentificacion As String
    Property Rsocial As String
    Property Matricula As String
    Property TipoRegistro As String
End Class
Public Class ClassRues
    Function SolicitaDocumentoConsultaRue(ByVal id_imagen As Integer,
                                          ByVal gabinete As String,
                                          ByVal matricula As String,
                                          ByRef class_stru_visor_migracion As class_stru_visor_migracion) As String
        '--------------------------------------------------------------------------------------
        'Funcion : Solicita el tipo de archivo a visualuizar y retorna la url de visualización
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
        Try

            Dim Result As String = ""
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim id_tipo_imagen As Integer = 0
            Result = ClassDaGabinete.SolicitaIdTipoImagen(id_imagen,
                                                            gabinete,
                                                            id_tipo_imagen)
            If Result <> "YES" Then
                SolicitaDocumentoConsultaRue = Result
                Exit Function
            End If
            Dim ClassDaExtension As New Class_da_extension
            Result = ClassDaExtension.SolicitaExtensionArchivoGabineteTipoImagen(id_tipo_imagen,
                                                                                 class_stru_visor_migracion.tipo_file)
            If Result <> "YES" Then
                SolicitaDocumentoConsultaRue = Result
                Exit Function
            End If
            If class_stru_visor_migracion.tipo_file = ".TIF" Or class_stru_visor_migracion.tipo_file = ".JPG" Or class_stru_visor_migracion.tipo_file = ".BMP" Then
                class_stru_visor_migracion.url_iframe = "../Docuarchi/WebFormDaVisorVersionPublico.aspx"
            Else
                class_stru_visor_migracion.url_iframe = "../Docuarchi/WebFormDaVisorExternoPublico.aspx"
            End If
            HttpContext.Current.Session.Item("DA_IMAGEN") = id_imagen
            HttpContext.Current.Session.Item("DA_GABINETE_CONSULTA") = gabinete
            SolicitaDocumentoConsultaRue = "YES"
        Catch ex As Exception
            SolicitaDocumentoConsultaRue = "Inconsistencia general funcion SolicitaDocumentoConsultaRue " & ex.Message
        End Try
    End Function
    Function IniciaConsultaRue(ByVal ParramRue As String,
                               ByVal CodigoCamara As String,
                               ByRef CdRues As CdRues,
                               ByRef NombreEmpresa As String) As String
        '---------------------------------------------------------------------------
        'Funcion : Inicia la consulta RUE 
        '
        '         
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'ParramRue                    : Representa la data encriptada del rue
        '                               
        'CodigoCamara                 : Representa el código de la camara
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'CdRues : Retorna parram RUE
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2025-05-06
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim TextoDesencript As String = ""
            Result = DesencriptaParamRue(ParramRue,
                                         TextoDesencript)
            If Result <> "YES" Then
                IniciaConsultaRue = Result
                Exit Function
            End If
            CdRues = New CdRues
            CdRues = Newtonsoft.Json.JsonConvert.DeserializeObject(Of CdRues)(TextoDesencript)
            Dim LetfTipoEsal As String = ""
            If CdRues.expediente <> "" Then
                LetfTipoEsal = Left(CdRues.expediente, 4)
                If LetfTipoEsal = "9000" Then
                    CdRues.tipoRegistro = "ES"
                End If
            End If
            Dim GestorModuleSesion As New GestorModuleSesion.Gestor_conexion
            Result = GestorModuleSesion.Retorna_nombre_empresa(CodigoCamara,
                                                               NombreEmpresa)
            If Result <> "YES" Then
                IniciaConsultaRue = "Lamentamos que esté presentando problemas con su consulta. El código de cámara informado por el RUE no es válido.  (" & CodigoCamara & ")  inconsistencia presentada (" & Result & ")"
                Exit Function
            End If
            Result = GestorModuleSesion.Inicializa_conexiones_publicas_si_sesion(NombreEmpresa)
            If Result <> "YES" Then
                IniciaConsultaRue = "Lamentamos que esté presentando problemas con su consulta. Estamos presentando inconvenientes con la conexión (" & Result & ")"
                Exit Function
            End If
            IniciaConsultaRue = "YES"
            Exit Function
        Catch ex As Exception
            IniciaConsultaRue = "Inconsistencia general funcion IniciaConsultaRue " & ex.Message
        End Try
    End Function
    Function ConsultaGabineteRue(ByVal CdRues As CdRues,
                                 ByVal NombreEmpresa As String,
                                 ByRef class_stru_Row_Gabinete_Generic As class_stru_Row_Gabinete_Generic,
                                 ByRef NombreGabinete As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Realiza la consulta de documentos relacionados a una matricula
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'CdRues              : Representa la estructura del reue para consulta
        'NombreEmpresa       : Representa el nombre de la empresa
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'class_stru_Row_Gabinete_Generic : Retorna la estructura con la lista de registros
        'NombreGabinete                  : Retorna el nombre del gabinete
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-05-09
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            '---Solicita id del gabinete rue y el nombre de gabinete docuarchi
            Dim Result As String = ""
            Dim IdGabinete As Integer = 0
            Dim ClassRaTiporueGabinete As New ClassRaTiporueGabinete
            Result = ClassRaTiporueGabinete.SolicitaNombreGabineteConCodigoGabineteTipoRue(CdRues.tipoRegistro,
                                                                                           IdGabinete,
                                                                                           NombreGabinete)
            If Result <> "YES" Then
                ConsultaGabineteRue = Result
                Exit Function
            End If
            HttpContext.Current.Session.Item("DA_GABINETE_CONSULTA") = NombreGabinete
            Dim ClassRaCamposConsultaRueGabinete As New ClassRaCamposConsultaRueGabinete
            Dim CampoConsultaGabinete As String = ""
            Result = ClassRaCamposConsultaRueGabinete.SolicitaNombreCampoConsultaGabinete(IdGabinete,
                                                                                          CampoConsultaGabinete)
            If Result <> "YES" Then
                ConsultaGabineteRue = Result
                Exit Function
            End If
            Dim structure_campo_aleas() As SturcturaCampoConsultaAleasRue = Nothing
            Dim ClassRaCamposDocuarchiAleasRue As New ClassRaCamposDocuarchiAleasRue
            Result = ClassRaCamposDocuarchiAleasRue.SolicitaNombresDocuarchiAleasRue(IdGabinete,
                                                                                     structure_campo_aleas)
            If Result <> "YES" Then
                ConsultaGabineteRue = Result
                Exit Function
            End If
            HttpContext.Current.Session.Item("DA_Login_Usuario") = "CONSULTAPUBLICO"
            HttpContext.Current.Session.Item("TIPOMODULO") = "PUBLICO"
            HttpContext.Current.Session.Item("EMPRESA_GESTION") = NombreEmpresa
            Dim class_campos_table_bostra_table As New List(Of class_campos_table_bostra_table)()
            Result = ClassRaCamposDocuarchiAleasRue.SolicitaCamposGabinetesRues(structure_campo_aleas,
                                                                                NombreGabinete,
                                                                                class_campos_table_bostra_table)
            If Result <> "YES" Then
                ConsultaGabineteRue = Result
                Exit Function
            End If
            Dim ClassRaNotificaEmailModulos As New ClassRaNotificaEmailModulos
            Dim EmailNotificacion As String = ""
            Result = ClassRaNotificaEmailModulos.SolicitaEmailNofiticacionModulo("RUECONSULTA",
                                                                                 EmailNotificacion)
            If Result <> "YES" Then
                ConsultaGabineteRue = Result
                Exit Function
            End If
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim ValorCampo As String = CdRues.expediente
            If ValorCampo <> "" Then
                ValorCampo = ValorCampo.Replace("SO", "")
                ValorCampo = ValorCampo.Replace("9000", "")
                ValorCampo = Trim(ValorCampo)
            End If
            Dim Consulta As String = ""
            Result = ClassDaGabinete.SolicitaSqlConsultaRue(class_campos_table_bostra_table,
                                                            CampoConsultaGabinete,
                                                            ValorCampo,
                                                            NombreGabinete,
                                                            Consulta)
            If Result <> "YES" Then
                ConsultaGabineteRue = Result
                Exit Function
            End If
            class_stru_Row_Gabinete_Generic.Obj_ilist_fileds_generic = class_campos_table_bostra_table
            Result = ClassDaGabinete.SolicitaRowTableConsultaRue(Consulta,
                                                                 class_stru_Row_Gabinete_Generic.Obj_ilist_row_generic)
            If Result <> "YES" Then
                ConsultaGabineteRue = Result
                Exit Function
            End If
            Dim ClassLogUsuarioRue As New ClassLogUsuarioRue
            Dim ClassCorreo As New ClassCorreo
            If class_stru_Row_Gabinete_Generic.Obj_ilist_row_generic = "[]" Then
                ClassLogUsuarioRue.RegistraLogUsuarioRue(CdRues, "MATRICULA NO ENCONTRADA", "0")
                Result = ClassCorreo.EnviaCorreoNotificacionResultadoConsultaRue(CdRues, "MATRICULA NO ENCONTRADA", "Lamentablemente, no encontramos resultados para la matrícula (" & ValorCampo & "), hemos notificado al administrador de la cámara responsable a través de un correo electrónico la incosistencia", EmailNotificacion)
                ConsultaGabineteRue = "Lamentablemente, no encontramos resultados para la matrícula (" & ValorCampo & "), hemos notificado al administrador de la cámara responsable a través de un correo electrónico la incosistencia"
                Exit Function
            Else
                ClassLogUsuarioRue.RegistraLogUsuarioRue(CdRues, "CONSULTA RUE", "0")
                ConsultaGabineteRue = "YES"
                Exit Function
            End If

        Catch ex As Exception
            ConsultaGabineteRue = "Inconsistencia general función ConsultaGabineteRue " & ex.Message
        End Try
    End Function
    Function SolicitaCatractrizacionExpedienteRue(Matricula As String,
                                                  ByVal Gabinete As String,
                                                  ByRef CdRuesCaracterizacion As CdRuesCaracterizacion) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita los datos de caracteriación de un expediente
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'Matricula           : Representa la mtricula del matriculado 
        'Gabinete            : Representa el nombre del gabinete Docuarchi
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'CdRuesCaracterizacion  : Retorna los datos de caracterización de la matricula RUE
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-05-08
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim ClassConsultaExpedienteSII As New ClassConsultaExpedienteSII
            Dim Result As String = ""
            Dim Proponete As String = ""
            Select Case UCase(Gabinete)
                Case "RUP"
                    Proponete = Matricula
                Case "ESAL"
                    Dim LetfTipoEsal = Left(Matricula, 4)
                    If LetfTipoEsal = "9000" Then
                        Matricula = Matricula.Replace("9000", "")
                    End If
                    Matricula = Matricula.Replace("S0", "")
            End Select
            Dim StruSiiCahcheInscripcion As StruSiiCahcheInscripcion = Nothing
            Result = ClassConsultaExpedienteSII.SolicitaEstructuraExpedienteSII(Matricula,
                                                                                Proponete,
                                                                                Gabinete,
                                                                                StruSiiCahcheInscripcion)
            If Result <> "YES" Then
                SolicitaCatractrizacionExpedienteRue = Result
                Exit Function
            Else
                CdRuesCaracterizacion = New CdRuesCaracterizacion
                CdRuesCaracterizacion.Matricula = StruSiiCahcheInscripcion.Matricula
                CdRuesCaracterizacion.Rsocial = StruSiiCahcheInscripcion.Rsocial
                Select Case UCase(Gabinete)
                    Case "RUP"
                        CdRuesCaracterizacion.TipoRegistro = "Registro único de proponentes - RUP"
                    Case "ESAL"
                        CdRuesCaracterizacion.TipoRegistro = "Registro Entidades sin ánimo de lucro - ESAL"
                    Case "MERCANTIL"
                        CdRuesCaracterizacion.TipoRegistro = "Registro mercantil - MERCANTIL"
                End Select
                SolicitaCatractrizacionExpedienteRue = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaCatractrizacionExpedienteRue = "Inconsistencia general funcion SolicitaCatractrizacionExpedienteRue " & ex.Message
        End Try
    End Function
End Class
