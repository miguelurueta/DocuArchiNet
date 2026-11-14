Option Strict On
Option Compare Binary
Imports System
Imports System.Data
Imports System.Globalization
Imports System.Linq
Imports MySql.Data.MySqlClient

'Parte del unico repositorio operativo. Reclamacion, inicio y señal de detencion;
'sin invocaciones legacy, cambios de configuracion ni recuperacion por lease.
Public Partial Class MySqlImportacionCarpetasRepository
    Public Function ObtenerIntentoCreacion(contexto As ContextoImportacionCarpetas, solicitud As ReclamacionImportacionCarpetas) As IntentoImportacionCarpetas
        AutorizarOperacion(contexto) : ValidarSolicitudDocumental(solicitud)
        If solicitud.Fase <> "CREAR_EXPEDIENTE" Then Throw SolicitudInvalida()
        Return Operar(Function(conexion, tx)
                          If BuscarLote(conexion, tx, contexto, "lote_id", IdOperacion(solicitud.LoteId)) Is Nothing Then Throw AccesoDenegado()
                          Dim intento = CargarRepeticionDocumental(conexion, tx, solicitud, solicitud.RecursoId)
                          If intento IsNot Nothing Then intento.EsNuevaReserva = False
                          Return intento
                      End Function)
    End Function

    Public Function IniciarCreacionExpediente(comando As ComandoCreacionExpedienteImportacion, datos As Collections.Generic.Dictionary(Of String, String)) As Boolean
        If comando Is Nothing Then Throw SolicitudInvalida()
        If comando.EsVerificacion OrElse comando.EsPreflightReanudacion Then Throw SolicitudInvalida()
        comando.ValidarEntradasLegacy() : comando.ValidarDatosLegacy(datos)
        Dim contexto = comando.Contexto
        AutorizarOperacion(contexto)
        Return Operar(Function(conexion, tx)
                          Dim lote = BloquearLoteRecepcion(conexion, tx, contexto, comando.LoteId)
                          Dim raiz = CargarRaizControl(conexion, tx, lote.LoteId, comando.RaizId)
                          ValidarInicioCreacion(comando, lote, raiz)
                          Dim plan = CargarPlan(conexion, tx, contexto, IdOperacion(lote.PlanId), False)
                          If Not LeerBarreraCreacion(lote, plan, raiz, Function(sql, parametros) LeerTablaConsulta(conexion, tx, sql, parametros)) Then Return False
                          Dim instante = Ahora()
                          Dim evidencia As New ResultadoEfectoImportacion With {.Estado = "EN_CURSO", .Codigo = "CREACION_PREPARADA", .Mensaje = "Inicio registrado; no acredita resultado documental.", .Creacion = New EvidenciaCreacionExpedienteImportacion With {.IntentoId = comando.IntentoId, .CicloArchivoId = datos("CICLO_ARCHIVO_ID"), .CicloArchivoNombre = datos("CICLO_ARCHIVO_NOMBRE")}}
                          Using cmd = ComandoOperacion(conexion, tx, "UPDATE ra_integracion_intento SET estado='EN_CURSO',ultimo_contacto_utc=@ahora,evidencia_doc=@evidencia WHERE intento_id=@intento AND lote_id=@lote AND raiz_id=@raiz AND archivo_id IS NULL AND fase='CREAR_EXPEDIENTE' AND estado='RESERVADO' AND token_reclamacion=@token AND solicitud_hash=@hash AND fin_utc IS NULL", "@ahora", instante, "@evidencia", PerfilImportacionJson.Escribir(evidencia), "@intento", IdOperacion(comando.IntentoId), "@lote", IdOperacion(comando.LoteId), "@raiz", IdOperacion(comando.RaizId), "@token", comando.TokenReclamacion, "@hash", BytesHuella(comando.SolicitudHash))
                              Dim afectadas = cmd.ExecuteNonQuery()
                              If afectadas = 0 Then Return False
                              UnaFila(afectadas)
                          End Using
                          Using cmd = ComandoOperacion(conexion, tx, "UPDATE ra_integracion_lote SET revision=revision+1,actualizado_utc=@ahora WHERE lote_id=@lote AND revision=@revision AND detencion_solicitada=0", "@ahora", instante, "@lote", IdOperacion(lote.LoteId), "@revision", lote.Revision)
                              UnaFila(cmd.ExecuteNonQuery())
                          End Using
                          RegistrarEventoControl(conexion, tx, contexto, lote.LoteId, "RAIZ", raiz.RaizId, comando.IntentoId, "INICIAR_CREACION", "CREACION_PREPARADA", instante)
                          Return True
                      End Function)
    End Function

    Friend Shared Sub ValidarInicioCreacion(comando As ComandoCreacionExpedienteImportacion, lote As LoteImportacionCarpetas, raiz As RaizImportacionCarpetas)
        If comando IsNot Nothing AndAlso comando.EsPreflightReanudacion Then Throw SolicitudInvalida()
        If comando Is Nothing OrElse lote Is Nothing OrElse raiz Is Nothing OrElse lote.Contexto Is Nothing OrElse lote.LoteId <> comando.LoteId OrElse lote.DetencionSolicitada OrElse lote.Estado <> "EN_CURSO" OrElse lote.Revision = 0 OrElse lote.Revision >= UInteger.MaxValue - 1UI OrElse raiz.Revision >= UInteger.MaxValue - 1UI Then Throw ConflictoOperacion()
        If raiz.LoteId <> lote.LoteId OrElse raiz.RaizId <> comando.RaizId OrElse raiz.Revision <> comando.RevisionRaiz OrElse raiz.Estado <> "CREANDO" OrElse raiz.IntentoActivoId <> comando.IntentoId OrElse raiz.IdentidadExpediente IsNot Nothing OrElse raiz.ValoresEfectivos Is Nothing OrElse raiz.ValoresEfectivos.Count <> 0 Then Throw ConflictoOperacion()
        If PerfilImportacionJson.Escribir(lote.Contexto) <> PerfilImportacionJson.Escribir(comando.Contexto) OrElse PerfilImportacionJson.Escribir(lote.Snapshot) <> PerfilImportacionJson.Escribir(comando.Snapshot) OrElse PerfilImportacionJson.Escribir(raiz.ValoresPlan) <> PerfilImportacionJson.Escribir(comando.Valores) Then Throw ConflictoOperacion()
    End Sub

    Private Function ReservarIntentoDocumental(contexto As ContextoImportacionCarpetas, solicitud As ReclamacionImportacionCarpetas) As IntentoImportacionCarpetas
        ValidarSolicitudDocumental(solicitud)
        Return Operar(Function(conexion, tx)
                          'Orden comun con recepcion/detencion: lote, raiz, archivo.
                          Dim lote = BloquearLoteRecepcion(conexion, tx, contexto, solicitud.LoteId)
                          Dim archivo As ArchivoImportacionCarpetas = Nothing
                          Dim raizId = solicitud.RecursoId
                          If solicitud.RecursoTipo = "ARCHIVO" Then
                              'Resolver pertenencia antes de bloquear en el orden comun.
                              archivo = CargarArchivoRecepcion(conexion, tx, contexto, lote.LoteId, solicitud.RecursoId, False)
                              raizId = archivo.RaizId
                          End If
                          Dim raiz = CargarRaizControl(conexion, tx, lote.LoteId, raizId)
                          If archivo IsNot Nothing Then archivo = CargarArchivoRecepcion(conexion, tx, contexto, lote.LoteId, archivo.ArchivoId, True)
                          Dim previo = CargarRepeticionDocumental(conexion, tx, solicitud, raiz.RaizId)
                          If previo IsNot Nothing Then Return previo
                          ValidarReservaDocumental(lote, raiz, archivo, solicitud, HayReclamacionDocumental(conexion, tx, lote.LoteId))
                          If solicitud.Fase = "ALMACENAR" Then
                              Dim plan = CargarPlan(conexion, tx, contexto, IdOperacion(lote.PlanId), False)
                              Dim vista = LeerVistaBarreraCreacion(lote, plan, raiz, Function(sql, parametros) LeerTablaConsulta(conexion, tx, sql, parametros))
                              If Not CoordinadorImportacionCarpetas.EvaluarBarreraAlmacenamiento(vista, archivo) Then Throw ConflictoOperacion()
                          End If
                          Dim instante = Ahora(), intentoId As String
                          Using cmd = ComandoOperacion(conexion, tx, "INSERT INTO ra_integracion_intento (lote_id,raiz_id,archivo_id,fase,estado,ejecutor_ref,token_reclamacion,solicitud_hash,inicio_utc,ultimo_contacto_utc) VALUES (@lote,@raiz,@archivo,@fase,'RESERVADO',@ejecutor,@token,@hash,@ahora,@ahora)",
                              "@lote", IdOperacion(lote.LoteId), "@raiz", IdOperacion(raiz.RaizId), "@archivo", If(archivo Is Nothing, DirectCast(DBNull.Value, Object), IdOperacion(solicitud.RecursoId)), "@fase", solicitud.Fase, "@ejecutor", solicitud.EjecutorRef, "@token", solicitud.TokenReclamacion, "@hash", BytesHuella(solicitud.SolicitudHash), "@ahora", instante)
                              UnaFila(cmd.ExecuteNonQuery()) : intentoId = IdentidadInsertada(cmd)
                          End Using
                          If archivo Is Nothing Then
                              Using cmd = ComandoOperacion(conexion, tx, "UPDATE ra_integracion_raiz SET estado='CREANDO',intento_activo_id=@intento,revision=revision+1,actualizado_utc=@ahora WHERE raiz_id=@id AND lote_id=@lote AND revision=@revision AND intento_activo_id IS NULL AND estado='PENDIENTE' AND identidad_expediente_doc IS NULL", "@intento", IdOperacion(intentoId), "@ahora", instante, "@id", IdOperacion(raiz.RaizId), "@lote", IdOperacion(lote.LoteId), "@revision", raiz.Revision)
                                  UnaFila(cmd.ExecuteNonQuery())
                              End Using
                          Else
                              Using cmd = ComandoOperacion(conexion, tx, "UPDATE ra_integracion_archivo SET estado_efecto='EN_CURSO',intento_activo_id=@intento,revision=revision+1,actualizado_utc=@ahora WHERE archivo_id=@id AND raiz_id=@raiz AND revision=@revision AND intento_activo_id IS NULL AND estado_recepcion='RECIBIDO' AND estado_efecto=@estado", "@intento", IdOperacion(intentoId), "@ahora", instante, "@id", IdOperacion(archivo.ArchivoId), "@raiz", IdOperacion(raiz.RaizId), "@revision", archivo.Revision, "@estado", archivo.EstadoEfecto)
                                  UnaFila(cmd.ExecuteNonQuery())
                              End Using
                          End If
                          If _observadorConfirmacion IsNot Nothing Then _observadorConfirmacion("RECLAMACION_RECURSO_RESERVADO", conexion, tx)
                          Using cmd = ComandoOperacion(conexion, tx, "UPDATE ra_integracion_lote SET estado='EN_CURSO',revision=revision+1,actualizado_utc=@ahora WHERE lote_id=@lote AND revision=@revision AND detencion_solicitada=0", "@ahora", instante, "@lote", IdOperacion(lote.LoteId), "@revision", lote.Revision)
                              UnaFila(cmd.ExecuteNonQuery())
                          End Using
                          RegistrarEventoControl(conexion, tx, contexto, lote.LoteId, solicitud.RecursoTipo, solicitud.RecursoId, intentoId, "RESERVAR_INTENTO", "INTENTO_RESERVADO", instante)
                          Dim reservado = CargarRepeticionDocumental(conexion, tx, solicitud, raiz.RaizId)
                          If reservado Is Nothing Then Throw NoDisponible()
                          reservado.EsNuevaReserva = True
                          Return reservado
                      End Function)
    End Function

    Friend Shared Sub ValidarSolicitudDocumental(solicitud As ReclamacionImportacionCarpetas)
        If solicitud Is Nothing OrElse Not {"CREAR_EXPEDIENTE", "ALMACENAR", "INCORPORAR"}.Contains(solicitud.Fase) OrElse solicitud.RecursoTipo <> If(solicitud.Fase = "CREAR_EXPEDIENTE", "RAIZ", "ARCHIVO") OrElse Not ValidadorPerfilImportacion.ReferenciaValida(solicitud.LoteId) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(solicitud.RecursoId) OrElse solicitud.RevisionEsperada = 0 OrElse solicitud.RevisionEsperada >= UInteger.MaxValue - 1UI OrElse Not HexExacto(solicitud.TokenReclamacion, 32) OrElse Not HexExacto(solicitud.SolicitudHash, 64) OrElse Not CorrelacionValida(solicitud.EjecutorRef) Then Throw SolicitudInvalida()
    End Sub

    Friend Shared Sub ValidarReservaDocumental(lote As LoteImportacionCarpetas, raiz As RaizImportacionCarpetas, archivo As ArchivoImportacionCarpetas, solicitud As ReclamacionImportacionCarpetas, hayReclamacion As Boolean)
        ValidarSolicitudDocumental(solicitud)
        If lote Is Nothing OrElse raiz Is Nothing OrElse lote.LoteId <> solicitud.LoteId OrElse raiz.LoteId <> lote.LoteId OrElse lote.Revision = 0 OrElse lote.Revision >= UInteger.MaxValue - 1UI OrElse lote.DetencionSolicitada OrElse Not {"RECIBIENDO", "LISTO", "EN_CURSO", "PARCIAL"}.Contains(lote.Estado) Then Throw ConflictoOperacion()
        If hayReclamacion OrElse Not String.IsNullOrEmpty(raiz.IntentoActivoId) OrElse {"CREANDO", "INCIERTO"}.Contains(raiz.Estado) Then Throw New PerfilImportacionException("OPERACION_EN_CURSO", "Existe un intento documental pendiente de verificación.")
        If solicitud.Fase = "CREAR_EXPEDIENTE" Then
            If archivo IsNot Nothing OrElse raiz.RaizId <> solicitud.RecursoId OrElse raiz.Revision <> solicitud.RevisionEsperada OrElse raiz.Estado <> "PENDIENTE" OrElse raiz.IdentidadExpediente IsNot Nothing Then Throw ConflictoOperacion()
            CoordinadorImportacionCarpetas.ValidarTransicion("RAIZ", raiz.Estado, "CREANDO")
        Else
            If archivo Is Nothing OrElse archivo.ArchivoId <> solicitud.RecursoId OrElse archivo.RaizId <> raiz.RaizId OrElse archivo.Revision <> solicitud.RevisionEsperada OrElse raiz.Estado <> "EXPEDIENTE_CONFIRMADO" OrElse Not IdentidadRaizValida(lote, raiz.IdentidadExpediente) Then Throw ConflictoOperacion()
            If Not String.IsNullOrEmpty(archivo.IntentoActivoId) OrElse archivo.EstadoEfecto = "INCIERTO" OrElse archivo.EstadoEfecto = "EN_CURSO" Then Throw New PerfilImportacionException("OPERACION_EN_CURSO", "El archivo tiene un intento pendiente de verificación.")
            If archivo.EstadoRecepcion <> "RECIBIDO" OrElse archivo.BytesRecibidos <> archivo.BytesPrevistos OrElse BytesDecimales(archivo.BytesRecibidos) = 0UL OrElse Not HexExacto(archivo.ContenidoHash, 64) OrElse Not HexExacto(archivo.StagingRef, 32) Then Throw ConflictoOperacion()
            If solicitud.Fase = "ALMACENAR" Then
                If archivo.EstadoEfecto <> "NO_INICIADO" OrElse archivo.Resultado IsNot Nothing OrElse Not archivo.ValoresEfectivosFijados Then Throw ConflictoOperacion()
                CoordinadorImportacionCarpetas.ValidarVistaValores(lote.Contexto, New VistaValoresImportacion With {.Lote = lote, .Raiz = raiz, .Archivo = archivo})
            Else
                Dim resultado = archivo.Resultado
                If archivo.EstadoEfecto <> "ALMACENADO" OrElse resultado Is Nothing OrElse Not ValidadorPerfilImportacion.ReferenciaValida(resultado.DocumentoId) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(resultado.GabineteId) OrElse String.IsNullOrWhiteSpace(resultado.OrigenGabinete) OrElse PerfilImportacionJson.Escribir(resultado.Expediente) <> PerfilImportacionJson.Escribir(raiz.IdentidadExpediente) Then Throw ConflictoOperacion()
                If lote.Snapshot Is Nothing OrElse lote.Snapshot.Configuracion Is Nothing OrElse Not lote.Snapshot.Configuracion.GabineteId.HasValue OrElse lote.Snapshot.Configuracion.GabineteId.Value.ToString(CultureInfo.InvariantCulture) <> resultado.GabineteId OrElse lote.Snapshot.Configuracion.OrigenGabinete <> resultado.OrigenGabinete Then Throw ConflictoOperacion()
            End If
            CoordinadorImportacionCarpetas.ValidarTransicion("EFECTO", archivo.EstadoEfecto, "EN_CURSO")
        End If
        If lote.Estado <> "EN_CURSO" Then CoordinadorImportacionCarpetas.ValidarTransicion("LOTE", lote.Estado, "EN_CURSO")
    End Sub

    Private Shared Function IdentidadRaizValida(lote As LoteImportacionCarpetas, identidad As IdentidadExpedienteImportacion) As Boolean
        Return identidad IsNot Nothing AndAlso lote.Contexto IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(identidad.OrigenLogico) AndAlso ValidadorPerfilImportacion.ReferenciaValida(identidad.ExpedienteId) AndAlso ValidadorPerfilImportacion.ReferenciaValida(identidad.RelacionNivelId) AndAlso identidad.NivelRef = lote.Contexto.NivelRef
    End Function

    Private Shared Function HayReclamacionDocumental(conexion As MySqlConnection, tx As MySqlTransaction, loteId As String) As Boolean
        'Tambien bloquea marcadores huerfanos. Ninguna fecha/lease da permiso.
        Using cmd = ComandoOperacion(conexion, tx, "SELECT EXISTS(SELECT 1 FROM ra_integracion_intento WHERE lote_id=@lote AND fase<>'RECEPCION' AND estado IN ('RESERVADO','EN_CURSO','INCIERTO')) OR EXISTS(SELECT 1 FROM ra_integracion_raiz WHERE lote_id=@lote AND (intento_activo_id IS NOT NULL OR estado IN ('CREANDO','INCIERTO'))) OR EXISTS(SELECT 1 FROM ra_integracion_archivo a INNER JOIN ra_integracion_raiz r ON r.raiz_id=a.raiz_id WHERE r.lote_id=@lote AND (a.estado_efecto IN ('EN_CURSO','INCIERTO') OR (a.intento_activo_id IS NOT NULL AND a.estado_recepcion='RECIBIDO')))", "@lote", IdOperacion(loteId))
            Return Convert.ToBoolean(cmd.ExecuteScalar())
        End Using
    End Function

    Private Shared Function CargarRaizControl(conexion As MySqlConnection, tx As MySqlTransaction, loteId As String, raizId As String) As RaizImportacionCarpetas
        Using cmd = ComandoOperacion(conexion, tx, "SELECT * FROM ra_integracion_raiz WHERE raiz_id=@raiz AND lote_id=@lote FOR UPDATE", "@raiz", IdOperacion(raizId), "@lote", IdOperacion(loteId))
            Using reader = cmd.ExecuteReader()
                If Not reader.Read() Then Throw AccesoDenegado()
                Return LeerRaizControl(reader)
            End Using
        End Using
    End Function

    Friend Shared Function LeerRaizControl(reader As IDataRecord) As RaizImportacionCarpetas
        Dim datos = LeerPersistido(Of DocumentoRaizPersistida)(CStr(reader("datos_doc")))
        Dim raiz As New RaizImportacionCarpetas With {.RaizId = Texto(reader("raiz_id")), .LoteId = Texto(reader("lote_id")), .ClienteRaizId = Texto(reader("cliente_raiz_id")), .NombreOriginal = datos.NombreOriginal, .Estado = Texto(reader("estado")), .Revision = Convert.ToUInt32(reader("revision")), .ValoresPlan = datos.ValoresPlan,
            .IntentoActivoId = If(reader.IsDBNull(reader.GetOrdinal("intento_activo_id")), Nothing, Texto(reader("intento_activo_id")))}
        If datos.Esquema <> 1 OrElse datos.IdCliente <> raiz.ClienteRaizId OrElse datos.ValoresPlan Is Nothing OrElse raiz.Revision = 0 OrElse Not EstadosImportacionCarpetas.EsRaiz(raiz.Estado) Then Throw NoDisponible()
        If Not reader.IsDBNull(reader.GetOrdinal("identidad_expediente_doc")) Then raiz.IdentidadExpediente = LeerPersistido(Of IdentidadExpedienteImportacion)(CStr(reader("identidad_expediente_doc")))
        If Not reader.IsDBNull(reader.GetOrdinal("valores_efectivos_doc")) Then raiz.ValoresEfectivos = LeerPersistido(Of System.Collections.Generic.List(Of ValorReglaPerfilImportacion))(CStr(reader("valores_efectivos_doc")))
        Return raiz
    End Function

    Private Shared Function CargarRepeticionDocumental(conexion As MySqlConnection, tx As MySqlTransaction, solicitud As ReclamacionImportacionCarpetas, raizId As String) As IntentoImportacionCarpetas
        Using cmd = ComandoOperacion(conexion, tx, "SELECT * FROM ra_integracion_intento WHERE lote_id=@lote AND raiz_id=@raiz AND " & If(solicitud.RecursoTipo = "RAIZ", "archivo_id IS NULL", "archivo_id=@archivo") & " AND fase=@fase AND solicitud_hash=@hash ORDER BY intento_id DESC LIMIT 1", "@lote", IdOperacion(solicitud.LoteId), "@raiz", IdOperacion(raizId), "@archivo", If(solicitud.RecursoTipo = "RAIZ", DirectCast(DBNull.Value, Object), IdOperacion(solicitud.RecursoId)), "@fase", solicitud.Fase, "@hash", BytesHuella(solicitud.SolicitudHash))
            Using reader = cmd.ExecuteReader()
                If Not reader.Read() Then Return Nothing
                Return LeerIntentoDocumental(reader)
            End Using
        End Using
    End Function

    Public Function SolicitarDetencion(contexto As ContextoImportacionCarpetas, loteId As String, revisionEsperada As UInteger) As LoteImportacionCarpetas Implements IControlLoteImportacionRepository.SolicitarDetencion
        AutorizarOperacion(contexto) : IdOperacion(loteId)
        If revisionEsperada = 0 OrElse revisionEsperada = UInteger.MaxValue Then Throw SolicitudInvalida()
        Return Operar(Function(conexion, tx)
                          Dim lote = BloquearLoteRecepcion(conexion, tx, contexto, loteId)
                          ValidarDetencion(lote, revisionEsperada)
                          If lote.DetencionSolicitada OrElse {"COMPLETADO", "FALLIDO"}.Contains(lote.Estado) Then Return lote
                          Dim instante = Ahora()
                          'Solo señal. El estado agregado y los intentos siguen visibles;
                          'no declara DETENIDO mientras una llamada pueda seguir viva.
                          Using cmd = ComandoOperacion(conexion, tx, "UPDATE ra_integracion_lote SET detencion_solicitada=1,revision=revision+1,actualizado_utc=@ahora WHERE lote_id=@lote AND revision=@revision AND detencion_solicitada=0", "@ahora", instante, "@lote", IdOperacion(loteId), "@revision", revisionEsperada)
                              UnaFila(cmd.ExecuteNonQuery())
                          End Using
                          RegistrarEventoControl(conexion, tx, contexto, loteId, "LOTE", loteId, Nothing, "SOLICITAR_DETENCION", "DETENCION_SOLICITADA", instante)
                          ConsolidarEnTransaccion(conexion, tx, contexto, loteId, instante)
                          Return BuscarLote(conexion, tx, contexto, "lote_id", IdOperacion(loteId))
                      End Function)
    End Function

    Friend Shared Sub ValidarDetencion(lote As LoteImportacionCarpetas, revisionEsperada As UInteger)
        If revisionEsperada = 0 OrElse revisionEsperada = UInteger.MaxValue Then Throw SolicitudInvalida()
        If lote Is Nothing OrElse lote.Revision = 0 OrElse Not EstadosImportacionCarpetas.EsLote(lote.Estado) Then Throw ConflictoOperacion()
        'Repetir una señal ya registrada no cambia nada, aun con revision anterior.
        If lote.DetencionSolicitada Then Return
        If lote.Revision <> revisionEsperada Then Throw ConflictoOperacion()
    End Sub

    Private Sub RegistrarEventoControl(conexion As MySqlConnection, tx As MySqlTransaction, contexto As ContextoImportacionCarpetas, loteId As String, recursoTipo As String, recursoId As String, intentoId As String, accion As String, codigo As String, instante As DateTime, Optional resultado As ResultadoEfectoImportacion = Nothing)
        If _observadorConfirmacion IsNot Nothing Then _observadorConfirmacion("CONTROL_ANTES_EVENTO", conexion, tx)
        Dim estadoEvento = If(intentoId Is Nothing, "SOLICITADA", If(codigo = "CREACION_PREPARADA", "EN_CURSO", "RESERVADO"))
        Using cmd = ComandoOperacion(conexion, tx, "INSERT INTO ra_integracion_evento (lote_id,intento_id,recurso_tipo,recurso_id,accion,codigo,actor_ref,ocurrido_utc,detalle_doc) VALUES (@lote,@intento,@tipo,@recurso,@accion,@codigo,@actor,@ahora,@detalle)", "@lote", IdOperacion(loteId), "@intento", If(intentoId Is Nothing, DirectCast(DBNull.Value, Object), IdOperacion(intentoId)), "@tipo", recursoTipo, "@recurso", IdOperacion(recursoId), "@accion", accion, "@codigo", codigo, "@actor", contexto.UsuarioRef, "@ahora", instante, "@detalle", PerfilImportacionJson.Escribir(If(resultado, New ResultadoEfectoImportacion With {.Estado = estadoEvento, .Codigo = codigo, .Mensaje = "Control operativo registrado; no certifica un efecto documental."})))
            UnaFila(cmd.ExecuteNonQuery())
        End Using
    End Sub
End Class
