Public Class Class_parram_ClassConsultaExpedienteSIIMercantil
	Public codigoerror As String
	Public Property codigoerror_() As String
		Get
			Return codigoerror
		End Get
		Set(ByVal value As String)
			codigoerror = value
		End Set
	End Property
	Public mensajeerror As String
	Public Property mensajeerror_() As String
		Get
			Return mensajeerror
		End Get
		Set(ByVal value As String)
			mensajeerror = value
		End Set
	End Property
	'Public ClassConsultaExpedienteSIIMercantil As ClassConsultaExpedienteSIIMercantil()
	'Public Property ClassConsultaExpedienteSIIMercantil_() As ClassConsultaExpedienteSIIMercantil()
	'	Get
	'		Return ClassConsultaExpedienteSIIMercantil
	'	End Get
	'	Set(ByVal value As ClassConsultaExpedienteSIIMercantil())
	'		ClassConsultaExpedienteSIIMercantil = value
	'	End Set
	'End Property
End Class
Public Class ClassConsultaExpedienteSIIMercantil
	Public codigoerror As String
	Public Property codigoerror_() As String
		Get
			Return codigoerror
		End Get
		Set(ByVal value As String)
			codigoerror = value
		End Set
	End Property
	Public mensajeerror As String
	Public Property mensajeerror_() As String
		Get
			Return mensajeerror
		End Get
		Set(ByVal value As String)
			mensajeerror = value
		End Set
	End Property
	Public matricula As String
	Public Property matricula_() As String
		Get
			Return matricula
		End Get
		Set(ByVal value As String)
			matricula = value
		End Set
	End Property
	Public nombre As String
	Public Property nombre_() As String
		Get
			Return nombre
		End Get
		Set(ByVal value As String)
			nombre = value
		End Set
	End Property
	Public identificacion As String
	Public Property identificacion_() As String
		Get
			Return identificacion
		End Get
		Set(ByVal value As String)
			identificacion = value
		End Set
	End Property
	Public nit As String
	Public Property nit_() As String
		Get
			Return nit
		End Get
		Set(ByVal value As String)
			nit = value
		End Set
	End Property
	Public matriculapro As String
	Public Property matriculapro_() As String
		Get
			Return matriculapro
		End Get
		Set(ByVal value As String)
			matriculapro = value
		End Set
	End Property
	Public nombrepro As String
	Public Property nombrepro_() As String
		Get
			Return nombrepro
		End Get
		Set(ByVal value As String)
			nombrepro = value
		End Set
	End Property
	Public identificacionpro As String
	Public Property identificacionpro_() As String
		Get
			Return identificacionpro
		End Get
		Set(ByVal value As String)
			identificacionpro = value
		End Set
	End Property
	Public establecimiento As establecimientos()
	Public Property establecimientos_() As establecimientos()
		Get
			Return establecimiento
		End Get
		Set(ByVal value As establecimientos())
			establecimiento = value
		End Set
	End Property
	Public Structure establecimientos
		Dim categoria As String
		Dim matricula As String
		Dim nombre As String
		Dim fechamatricula As String
		Dim fecharenovacion As String
		Dim valorestablecimiento As String
		Dim latitud As String
		Dim longitud As String
		Dim fechacenso As String
		Dim censo As String
		Dim infografia1 As String
		Dim infografia2 As String
	End Structure
End Class
Public Structure ClassConsultaExpedienteSIIMercantil_
	Dim codigoerror As String
	Dim mensajeerror As String
	Dim matricula As String
	Dim nombre As String
	Dim nombre1 As String
	Dim nombre2 As String
	Dim apellido1 As String
	Dim apellido2 As String
	Dim sigla As String
	Dim idclase As String
	Dim identificacion As String
	Dim genero As String
	Dim nit As String
	Dim emprendimientosocial As String
	Dim organizacion As String
	Dim categoria As String
	Dim estado As String
	Dim fechamatricula As String
	Dim fecharenovacion As String
	Dim ultanorenovado As String
	Dim fechacancelacion As String
	Dim dircom As String
	Dim idbarriocom As String
	Dim barriocom As String
	Dim muncom As String
	Dim telcom1 As String
	Dim telcom2 As String
	Dim telcom3 As String
	Dim emailcom As String
	Dim urlcom As String
	Dim dirnot As String
	Dim idbarrionot As String
	Dim barrionot As String
	Dim munnot As String
	Dim telnot1 As String
	Dim telnot2 As String
	Dim telnot3 As String
	Dim emailnot As String
	Dim autorizacionemailsms As String
	Dim ciiu1 As String
	Dim ciiu2 As String
	Dim ciiu3 As String
	Dim ciiu4 As String
	Dim afiliado As String
	Dim saldoafiliado As String
	Dim anodatos As String
	Dim fechadatos As String
	Dim activos As String
	Dim pasivos As String
	Dim patrimonio As String
	Dim ingresos As String
	Dim gastos As String
	Dim utilidad As String
	Dim personal As String
	Dim beneficio1429 As String
	Dim beneficio1780 As String
	Dim fechainicioactividades As String
	Dim regimentributario As String
	Dim idclaserl As String
	Dim identificacionrl As String
	Dim nombrerl As String
	Dim idclasepro As String
	Dim identificacionpro As String
	Dim nombrepro As String
	Dim matriculapro As String
	Dim camarapro As String
	Dim renovacionappaltoimpacto As String
	Dim renovacionappnocomercial As String
	Dim renovacionapp1780 As String
	Dim renovacionappmultavencida As String
	Dim cantidadmujeres As String
	Dim cantidadmujerescargosdirectivos As String
	Dim participacionmujeres As Integer
	Dim tamanoempresa As Integer
	Dim ciiutamanoempresarial As String
	Dim ingresostamanoempresarial As String
	Dim anodatostamanoempresarial As String
	Dim fechadatostamanoempresarial As String
	Dim pagos() As Pagos
	Dim actcte As String
	Dim actnocte As String
	Dim acttot As String
	Dim pascte As String
	Dim paslar As String
	Dim pastot As String
	Dim pattot As String
	Dim paspat As String
	Dim balsoc As String
	Dim ingope As String
	Dim ingnoope As String
	Dim gtoven As String
	Dim gtoadm As String
	Dim cosven As String
	Dim gasint As String
	Dim gasimp As String
	Dim utiope As String
	Dim utinet As String
	Dim actvin As String
End Structure
Public Structure Pagos
	Dim numerorecibo As String
	Dim fecoperacion As String
	Dim fecharenovacionaplicable As String
	Dim ctranulacion As String
	Dim servicio As String
	Dim nombreservicio As String
	Dim valor As String
	Dim anorenovacion As String
	Dim reliquidacion As String
End Structure
Public Class consultarExpedienteProponente
	Public codigoerror As String
	Public Property codigoerror_() As String
		Get
			Return codigoerror
		End Get
		Set(ByVal value As String)
			codigoerror = value
		End Set
	End Property
	Public mensajeerror As String
	Public Property mensajeerror_() As String
		Get
			Return mensajeerror
		End Get
		Set(ByVal value As String)
			mensajeerror = value
		End Set
	End Property
	Public matricula As String
	Public Property matricula_() As String
		Get
			Return matricula
		End Get
		Set(ByVal value As String)
			matricula = value
		End Set
	End Property
	Public nombre As String
	Public Property nombre_() As String
		Get
			Return nombre
		End Get
		Set(ByVal value As String)
			nombre = value
		End Set
	End Property
	Public identificacion As String
	Public Property identificacion_() As String
		Get
			Return identificacion
		End Get
		Set(ByVal value As String)
			identificacion = value
		End Set
	End Property
	Public nit As String
	Public Property nit_() As String
		Get
			Return nit
		End Get
		Set(ByVal value As String)
			nit = value
		End Set
	End Property
	Public matriculapro As String
	Public Property matriculapro_() As String
		Get
			Return matriculapro
		End Get
		Set(ByVal value As String)
			matriculapro = value
		End Set
	End Property
	Public nombrepro As String
	Public Property nombrepro_() As String
		Get
			Return nombrepro
		End Get
		Set(ByVal value As String)
			nombrepro = value
		End Set
	End Property
	Public identificacionpro As String
	Public Property identificacionpro_() As String
		Get
			Return identificacionpro
		End Get
		Set(ByVal value As String)
			identificacionpro = value
		End Set
	End Property
	Public establecimiento As establecimientos()
	Public Property establecimientos_() As establecimientos()
		Get
			Return establecimiento
		End Get
		Set(ByVal value As establecimientos())
			establecimiento = value
		End Set
	End Property
	Public Structure establecimientos
		Dim categoria As String
		Dim matricula As String
		Dim nombre As String
		Dim fechamatricula As String
		Dim fecharenovacion As String
		Dim valorestablecimiento As String
		Dim latitud As String
		Dim longitud As String
		Dim fechacenso As String
		Dim censo As String
		Dim infografia1 As String
		Dim infografia2 As String
	End Structure
End Class
Public Structure consultarExpedienteProponente_
	Dim codigoerror As String
	Dim mensajeerror As String
	Dim proponente As String
	Dim matricula As String
	Dim nombre As String
	Dim sigla As String
	Dim idclase As String
	Dim identificacion As String
	Dim nit As String
	Dim organizacion As String
	Dim estado As String
	Dim fechainscripcion As String
	Dim fecharenovacion As String
	Dim fechacancelacion As String
	Dim dircom As String
	Dim muncom As String
	Dim telcom1 As String
	Dim telcom2 As String
	Dim telcom3 As String
	Dim emailcom As String
	Dim urlcom As String
	Dim dirnot As String
	Dim munnot As String
	Dim telnot1 As String
	Dim telnot2 As String
	Dim telnot3 As String
	Dim emailnot As String
	Dim idclaserl As String
	Dim identificacionrl As String
	Dim nombrerl As String
	Dim inffin1510_fechacorte As String
	Dim inffin1510_actcte As String
	Dim inffin1510_actnocte As String
	Dim inffin1510_acttot As String
	Dim inffin1510_pascte As String
	Dim inffin1510_paslar As String
	Dim inffin1510_pastot As String
	Dim inffin1510_patnet As String
	Dim inffin1510_paspat As String
	Dim inffin1510_balsoc As String
	Dim inffin1510_ingope As String
	Dim inffin1510_ingnoope As String
	Dim inffin1510_gasope As String
	Dim inffin1510_gasnoope As String
	Dim inffin1510_cosven As String
	Dim inffin1510_utinet As String
	Dim inffin1510_utiope As String
	Dim inffin1510_gasint As String
	Dim inffin1510_gasimp As String
	Dim inffin1510_indliq As String
	Dim inffin1510_nivend As String
	Dim inffin1510_razcob As String
	Dim inffin1510_renpat As String
	Dim inffin1510_renact As String
	Dim inffin1510_gruponiif As String
End Structure

Public Class ClassConsultaExpedienteSII

	Function SolicitaEstructuraExpedienteSII(ByVal Matricula As String,
											 ByVal Proponente As String,
											 ByVal Gabinete As String,
											 ByRef StruSiiCahcheInscripcion As StruSiiCahcheInscripcion) As String
		'-----------------------------------------------------------------------------------------------
		'Funcion : Solcita la estructura del expediente SII para la actualización de indices
		'-----------------------------------------------------------------------------------------------
		'                           PARAMETROS  
		'-----------------------------------------------------------------------------------------------
		'Matricula         : Representa el número de matricula del matricualdo SII mercantil y esal
		'                    las maticulas ESAL empiezan con S0
		'Proponente        : Represnta el número del proponente
		'Gabinete          : Representa el nombre del gabinete del tramite (MERCANTIL, EAL Y PROPONENTE)
		'id_plantilla_radicacion : 
		'-----------------------------------------------------------------------------------------------
		'                           RETORNO
		'-----------------------------------------------------------------------------------------------
		'StruSiiCahcheInscripcion  : Retorna la esctrucctura con el registro de inscripción
		'-----------------------------------------------------------------------------------------------
		'                         CARACTERIZACIÓN
		'-----------------------------------------------------------------------------------------------
		'Fecha                 : 2025-04-01
		'Elabora               : Miguel Angel Urueta Miranda
		'------------------------------------------------------------------------------------------------
		Try
			Dim ClassConsultaExpedienteSIIMercantil As New ClassConsultaExpedienteSIIMercantil
			Dim ConsultarExpedienteProponente As New consultarExpedienteProponente
			Dim Result As String = ""
			Select Case UCase(Gabinete)
				Case "MERCANTIL"
					Result = Me.ConsultaExpedienteMercantilEsal(Matricula,
																ClassConsultaExpedienteSIIMercantil)
					If Result <> "YES" Then
						SolicitaEstructuraExpedienteSII = Result
						Exit Function
					Else
						StruSiiCahcheInscripcion.NitIdentificacion = ClassConsultaExpedienteSIIMercantil.nit
						StruSiiCahcheInscripcion.Rsocial = ClassConsultaExpedienteSIIMercantil.nombre
						StruSiiCahcheInscripcion.Matricula = Matricula
						StruSiiCahcheInscripcion.NombrePropietario = ClassConsultaExpedienteSIIMercantil.nombrepro
						StruSiiCahcheInscripcion.Identificacionpro = ClassConsultaExpedienteSIIMercantil.identificacionpro
						StruSiiCahcheInscripcion.MatriculaPropietario = ClassConsultaExpedienteSIIMercantil.matriculapro
						SolicitaEstructuraExpedienteSII = "YES"
						Exit Function
					End If
				Case "ESAL"
					Matricula = Matricula.Replace("9000", "")
					Dim MatriculaEsal As String = "S0" & Matricula
					Result = Me.ConsultaExpedienteMercantilEsal(MatriculaEsal,
																ClassConsultaExpedienteSIIMercantil)
					If Result <> "YES" Then
						SolicitaEstructuraExpedienteSII = Result
						Exit Function
					Else
						StruSiiCahcheInscripcion.NitIdentificacion = ClassConsultaExpedienteSIIMercantil.nit
						StruSiiCahcheInscripcion.Rsocial = ClassConsultaExpedienteSIIMercantil.nombre
						StruSiiCahcheInscripcion.Matricula = Matricula
						StruSiiCahcheInscripcion.NombrePropietario = ClassConsultaExpedienteSIIMercantil.nombrepro
						StruSiiCahcheInscripcion.Identificacionpro = ClassConsultaExpedienteSIIMercantil.identificacionpro
						StruSiiCahcheInscripcion.MatriculaPropietario = ClassConsultaExpedienteSIIMercantil.matriculapro
						SolicitaEstructuraExpedienteSII = "YES"
						Exit Function
					End If
				Case "RUP"
					Result = Me.ConsultaEexpedienteProponente(Val(Proponente),
															  ConsultarExpedienteProponente)
					If Result <> "YES" Then
						SolicitaEstructuraExpedienteSII = Result
						Exit Function
					Else
						StruSiiCahcheInscripcion.NitIdentificacion = ConsultarExpedienteProponente.nit
						StruSiiCahcheInscripcion.Rsocial = ConsultarExpedienteProponente.nombre
						StruSiiCahcheInscripcion.Matricula = Proponente
						StruSiiCahcheInscripcion.NombrePropietario = ClassConsultaExpedienteSIIMercantil.nombrepro
						StruSiiCahcheInscripcion.Identificacionpro = ClassConsultaExpedienteSIIMercantil.identificacionpro
						StruSiiCahcheInscripcion.MatriculaPropietario = ClassConsultaExpedienteSIIMercantil.matriculapro
						SolicitaEstructuraExpedienteSII = "YES"
						Exit Function
					End If
				Case Else
					SolicitaEstructuraExpedienteSII = "No se pudo homologar el gabinete (" & Gabinete & ") en la consulta de expedientes SII"
					Exit Function
			End Select
		Catch ex As Exception
			SolicitaEstructuraExpedienteSII = "Inconsistencia general funcion SolicitaEstructuraExpedienteSII " & ex.Message
		End Try
	End Function
	Function ConsultaEexpedienteProponente(ByVal MatriculaProponente As String,
										   ByRef ConsultarExpedienteProponente As consultarExpedienteProponente) As String
		'-----------------------------------------------------------------------------------------------
		'Funcion : Realiza consulta del expediente del registro unico de proponentes
		''         
		'-----------------------------------------------------------------------------------------------
		'                           PARAMETROS  
		'-----------------------------------------------------------------------------------------------
		'MatriculaMercantilEsal : Representa el numero de matricula de intengración con el SII
		'
		'
		'-----------------------------------------------------------------------------------------------
		'                           RETORNO
		'-----------------------------------------------------------------------------------------------
		'ClassConsultaExpedienteSIIMercantil  : Retorna la estructura con el expediente SII
		'-----------------------------------------------------------------------------------------------
		'                         CARACTERIZACIÓN
		'-----------------------------------------------------------------------------------------------
		'Fecha                 : 2025-04-01
		'Elabora               : Miguel Angel Urueta Miranda
		'------------------------------------------------------------------------------------------------
		Try
			Dim Result As String = ""
			Dim usuario_sii As String = ""
			Dim clave_usuario_sii As String = ""
			Dim UrlBase As String = ""
			Dim codigo_empresa As String = ""
			Dim Class_ws_usuarioworkflowsii As New Class_ws_usuarioworkflowsii
			Result = Class_ws_usuarioworkflowsii.solicita_usuario_validacion_sii(codigo_empresa,
																					 usuario_sii,
																					 clave_usuario_sii)
			If Result <> "YES" Then
				ConsultaEexpedienteProponente = Result
				Exit Function
			End If
			Result = Class_ws_usuarioworkflowsii.Solicita_url_nombrefuncion_restfull(UrlBase,
																					"solicitarToken")
			If Result <> "YES" Then
				ConsultaEexpedienteProponente = Result
				Exit Function
			End If
			Dim stru_token As SolicitaToken = Nothing
			Dim Class_ClassResfull As New Class_ClassResfull
			Result = Class_ClassResfull.Solicitar_token_general(codigo_empresa,
																   usuario_sii,
																   clave_usuario_sii,
																   UrlBase & "solicitarToken",
																   stru_token)
			If Result <> "YES" Then
				ConsultaEexpedienteProponente = Result
				Exit Function
			End If
			If stru_token.mensajeerror <> "" Then
				ConsultaEexpedienteProponente = stru_token.mensajeerror
				Exit Function
			End If
			Dim Parametros As Dictionary(Of String, String) = New Dictionary(Of String, String)()
			Parametros.Add("codigoempresa", codigo_empresa)
			Parametros.Add("usuariows", usuario_sii)
			Parametros.Add("token", stru_token.token)
			Parametros.Add("proponente", MatriculaProponente)
			Dim Class_Desserializacion As New Class_Desserializacion
			Dim respuestaServidor As String = ""
			Result = Class_ClassResfull.GetResponse(UrlBase & "consultarExpedienteProponente",
														Parametros,
														"POST",
														respuestaServidor)
			If Result <> "YES" Then
				ConsultaEexpedienteProponente = Result
				Exit Function
			End If
			Result = Class_Desserializacion.DesSerializacion_ConsultaExpedienteSIIProponente(respuestaServidor,
																							 ConsultarExpedienteProponente)
			If Result <> "YES" Then
				ConsultaEexpedienteProponente = Result
				Exit Function
			End If
			If ConsultarExpedienteProponente.mensajeerror <> "" Or ConsultarExpedienteProponente.mensajeerror Is Nothing Then
				If ConsultarExpedienteProponente.codigoerror = "9999" Or ConsultarExpedienteProponente.codigoerror Is Nothing Then
					ConsultaEexpedienteProponente = "Lamento que esté experimentando dificultades para encontrar el expediente de la matrícula del proponete (" & MatriculaProponente & ") en el SII; por favor, contacte a su administrador."
					Exit Function
				End If
				ConsultaEexpedienteProponente = "Lamento que esté experimentando dificultades para encontrar el expediente de la matrícula del proponete (" & MatriculaProponente & ") en el SII; por favor, contacte a su administrador Error : (" & ConsultarExpedienteProponente.mensajeerror & ") "
				Exit Function
			Else
				Dim Nombre As String = ConsultarExpedienteProponente.nombre
				If Nombre <> "" Then
					Nombre = Nombre.Replace("'", "")
					Nombre = Nombre.Replace("/", "")
					Nombre = Nombre.Replace("\", "")
					Nombre = Nombre.Replace("""", "")
					ConsultarExpedienteProponente.nombre = Nombre
				End If
				Dim NombreP As String = ConsultarExpedienteProponente.nombrepro
				If NombreP <> "" Then
					NombreP = NombreP.Replace("'", "")
					NombreP = NombreP.Replace("/", "")
					NombreP = NombreP.Replace("\", "")
					NombreP = NombreP.Replace("""", "")
					ConsultarExpedienteProponente.nombrepro = NombreP
				End If
				ConsultaEexpedienteProponente = "YES"
			End If
		Catch ex As Exception
			ConsultaEexpedienteProponente = "Inconsistencia general funcion ConsultaEexpedienteProponente " & ex.Message
		End Try
	End Function
	Function ConsultaExpedienteMercantilEsal(ByVal MatriculaMercantilEsal As String,
											 ByRef ClassConsultaExpedienteSIIMercantil As ClassConsultaExpedienteSIIMercantil) As String
		'-----------------------------------------------------------------------------------------------
		'Funcion : Realiza consulta del expediente del registro mercantil y entidades sin animo de 
		''         lucro
		'-----------------------------------------------------------------------------------------------
		'                           PARAMETROS  
		'-----------------------------------------------------------------------------------------------
		'MatriculaMercantilEsal : Representa el numero de matricula de intengración con el SII
		'
		'
		'-----------------------------------------------------------------------------------------------
		'                           RETORNO
		'-----------------------------------------------------------------------------------------------
		'ClassConsultaExpedienteSIIMercantil  : Retorna la estructura con el expediente SII
		'-----------------------------------------------------------------------------------------------
		'                         CARACTERIZACIÓN
		'-----------------------------------------------------------------------------------------------
		'Fecha                 : 2025-04-01
		'Elabora               : Miguel Angel Urueta Miranda
		'------------------------------------------------------------------------------------------------

		Try
			Dim Result As String = ""
			Dim usuario_sii As String = ""
			Dim clave_usuario_sii As String = ""
			Dim UrlBase As String = ""
			Dim codigo_empresa As String = ""
			Dim Class_ws_usuarioworkflowsii As New Class_ws_usuarioworkflowsii
			Result = Class_ws_usuarioworkflowsii.solicita_usuario_validacion_sii(codigo_empresa,
																				 usuario_sii,
																				 clave_usuario_sii)
			If Result <> "YES" Then
				ConsultaExpedienteMercantilEsal = Result
				Exit Function
			End If
			Result = Class_ws_usuarioworkflowsii.Solicita_url_nombrefuncion_restfull(UrlBase,
																					 "solicitarToken")
			If Result <> "YES" Then
				ConsultaExpedienteMercantilEsal = Result
				Exit Function
			End If
			Dim stru_token As SolicitaToken = Nothing
			Dim Class_ClassResfull As New Class_ClassResfull
			Result = Class_ClassResfull.Solicitar_token_general(codigo_empresa,
															   usuario_sii,
															   clave_usuario_sii,
															   UrlBase & "solicitarToken",
															   stru_token)
			If Result <> "YES" Then
				ConsultaExpedienteMercantilEsal = Result
				Exit Function
			End If
			If stru_token.mensajeerror <> "" Then
				ConsultaExpedienteMercantilEsal = stru_token.mensajeerror
				Exit Function
			End If
			Dim Parametros As Dictionary(Of String, String) = New Dictionary(Of String, String)()
			Parametros.Add("codigoempresa", codigo_empresa)
			Parametros.Add("usuariows", usuario_sii)
			Parametros.Add("token", stru_token.token)
			Parametros.Add("matricula", MatriculaMercantilEsal)
			Dim Class_Desserializacion As New Class_Desserializacion
			Dim respuestaServidor As String = ""
			Result = Class_ClassResfull.GetResponse(UrlBase & "consultarExpedienteMercantil",
													Parametros,
													"POST",
													respuestaServidor)
			If Result <> "YES" Then
				ConsultaExpedienteMercantilEsal = Result
				Exit Function
			End If
			ClassConsultaExpedienteSIIMercantil = New ClassConsultaExpedienteSIIMercantil
			Result = Class_Desserializacion.DesSerializacion_ConsultaExpedienteSIIMercantil(respuestaServidor,
																							ClassConsultaExpedienteSIIMercantil)
			If Result <> "YES" Then
				ConsultaExpedienteMercantilEsal = Result
				Exit Function
			End If
			If ClassConsultaExpedienteSIIMercantil.mensajeerror <> "" Or ClassConsultaExpedienteSIIMercantil.mensajeerror Is Nothing Then
				If ClassConsultaExpedienteSIIMercantil.codigoerror = "9999" Or ClassConsultaExpedienteSIIMercantil.codigoerror Is Nothing Then
					ConsultaExpedienteMercantilEsal = "Lamento que esté experimentando dificultades para encontrar el expediente de la matrícula (" & MatriculaMercantilEsal & ") en el SII; por favor, contacte a su administrador."
					Exit Function
				End If
				ConsultaExpedienteMercantilEsal = "Lamento que esté experimentando dificultades para encontrar el expediente de la matrícula (" & MatriculaMercantilEsal & ") en el SII; por favor, contacte a su administrador Error : (" & ClassConsultaExpedienteSIIMercantil.mensajeerror & ") "
				Exit Function
			Else
				Dim Nombre As String = ClassConsultaExpedienteSIIMercantil.nombre
				If Nombre <> "" Then
					Nombre = Nombre.Replace("'", "")
					Nombre = Nombre.Replace("/", "")
					Nombre = Nombre.Replace("\", "")
					Nombre = Nombre.Replace("""", "")
					ClassConsultaExpedienteSIIMercantil.nombre = Nombre
				End If
				Dim NombreP As String = ClassConsultaExpedienteSIIMercantil.nombrepro
				If NombreP <> "" Then
					NombreP = NombreP.Replace("'", "")
					NombreP = NombreP.Replace("/", "")
					NombreP = NombreP.Replace("\", "")
					NombreP = NombreP.Replace("""", "")
					ClassConsultaExpedienteSIIMercantil.nombrepro = NombreP
				End If
				ConsultaExpedienteMercantilEsal = "YES"
			End If
		Catch ex As Exception
			ConsultaExpedienteMercantilEsal = "Inconsistencia general función ConsultaExpedienteMercantilEsal " & ex.Message
		End Try
	End Function
End Class
