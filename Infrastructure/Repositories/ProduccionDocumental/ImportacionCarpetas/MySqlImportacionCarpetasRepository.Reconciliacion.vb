Option Strict On
Option Compare Binary
Imports System
Imports System.Linq
Imports MySql.Data.MySqlClient

Public Partial Class MySqlImportacionCarpetasRepository
    Public Function CerrarReservaAlmacenamientoSinInicio(esperada As VistaReconciliacionImportacion) As ResultadoEfectoImportacion
        If esperada Is Nothing OrElse esperada.Lote Is Nothing OrElse esperada.Intento Is Nothing Then Throw SolicitudInvalida()
        Dim contexto = esperada.Lote.Contexto
        AutorizarOperacion(contexto)
        Return Operar(Function(conexion, tx)
                          'Mismo mutex de lote y orden de recursos que IniciarAlmacenamientoDocumento.
                          Dim actual = CargarVistaReconciliacion(conexion, tx, contexto, esperada.Lote.LoteId, esperada.Intento.IntentoId)
                          Dim reservadas = 0, cerradas = 0, otras = 0
                          Using cmd = ComandoOperacion(conexion, tx, "SELECT accion,codigo,recurso_tipo,recurso_id FROM ra_integracion_evento WHERE lote_id=@lote AND intento_id=@intento", "@lote", IdOperacion(actual.Lote.LoteId), "@intento", IdOperacion(actual.Intento.IntentoId))
                              Using reader = cmd.ExecuteReader()
                                  While reader.Read()
                                      If Texto(reader("recurso_tipo")) <> "ARCHIVO" OrElse Texto(reader("recurso_id")) <> actual.Intento.ArchivoId Then
                                          otras += 1
                                      ElseIf Texto(reader("accion")) = "RESERVAR_INTENTO" AndAlso Texto(reader("codigo")) = "INTENTO_RESERVADO" Then
                                          reservadas += 1
                                      ElseIf Texto(reader("accion")) = "CERRAR_RESERVA_SIN_INICIO" AndAlso Texto(reader("codigo")) = "RESERVA_CERRADA_SIN_INICIO" Then
                                          cerradas += 1
                                      Else
                                          otras += 1
                                      End If
                                  End While
                              End Using
                          End Using
                          Return PersistirCierreReservaSinInicio(esperada, actual, reservadas, cerradas, otras, Ahora(),
                              Function(sql, parametros)
                                  Using cmd = ComandoOperacion(conexion, tx, sql, parametros)
                                      Return cmd.ExecuteNonQuery()
                                  End Using
                              End Function,
                              Sub(r) RegistrarEventoControl(conexion, tx, contexto, actual.Lote.LoteId, "ARCHIVO", actual.Archivo.ArchivoId, actual.Intento.IntentoId, "CERRAR_RESERVA_SIN_INICIO", r.Codigo, Ahora(), r))
                      End Function)
    End Function

    Friend Shared Function PersistirCierreReservaSinInicio(esperada As VistaReconciliacionImportacion, actual As VistaReconciliacionImportacion, reservadas As Integer, cerradas As Integer, otras As Integer, instante As DateTime, ejecutar As Func(Of String, Object(), Integer), evento As Action(Of ResultadoEfectoImportacion)) As ResultadoEfectoImportacion
        If esperada Is Nothing OrElse esperada.Lote Is Nothing OrElse esperada.Intento Is Nothing OrElse actual Is Nothing OrElse ejecutar Is Nothing OrElse evento Is Nothing Then Throw SolicitudInvalida()
        CoordinadorImportacionCarpetas.ValidarVistaReconciliacion(esperada, esperada.Lote.Contexto, esperada.Lote.LoteId, esperada.Intento.IntentoId)
        CoordinadorImportacionCarpetas.ValidarVistaReconciliacion(actual, esperada.Lote.Contexto, esperada.Lote.LoteId, esperada.Intento.IntentoId)
        Dim i = actual.Intento, a = actual.Archivo, r = actual.Raiz, l = actual.Lote
        'EjecutorRef identifica el canal tecnico; la propiedad/autorizacion del
        'usuario se comprueba mediante el contexto persistido y la vista.
        If i.Fase <> "ALMACENAR" OrElse a Is Nothing OrElse i.EjecutorRef <> "WEB_DOCUMENTO" OrElse reservadas <> 1 OrElse otras <> 0 Then Throw ConflictoOperacion()
        'Una repeticion solo devuelve el cierre persistido. No toca una reserva posterior.
        If i.Estado = "RECHAZADO" AndAlso i.FinUtc IsNot Nothing AndAlso i.Resultado IsNot Nothing AndAlso i.Resultado.Estado = "RECHAZADO" AndAlso i.Resultado.Codigo = "RESERVA_CERRADA_SIN_INICIO" AndAlso i.Resultado.DocumentoId Is Nothing AndAlso i.Resultado.InventarioId Is Nothing AndAlso cerradas = 1 Then Return i.Resultado
        If cerradas <> 0 OrElse PerfilImportacionJson.Escribir(esperada) <> PerfilImportacionJson.Escribir(actual) OrElse i.Estado <> "RESERVADO" OrElse i.Resultado IsNot Nothing OrElse i.FinUtc IsNot Nothing OrElse i.EsNuevaReserva Then Throw ConflictoOperacion()
        If l.Estado <> "EN_CURSO" OrElse l.DetencionSolicitada OrElse l.Revision >= UInteger.MaxValue - 1UI OrElse r.Estado <> "EXPEDIENTE_CONFIRMADO" OrElse r.IntentoActivoId IsNot Nothing OrElse r.IdentidadExpediente Is Nothing OrElse a.EstadoRecepcion <> "RECIBIDO" OrElse a.EstadoEfecto <> "EN_CURSO" OrElse a.IntentoActivoId <> i.IntentoId OrElse a.Resultado IsNot Nothing OrElse Not a.ValoresEfectivosFijados OrElse a.Revision >= UInteger.MaxValue - 1UI Then Throw ConflictoOperacion()
        Dim resultado As New ResultadoEfectoImportacion With {.Estado = "RECHAZADO", .Codigo = "RESERVA_CERRADA_SIN_INICIO", .Mensaje = "Reserva cerrada antes del checkpoint de inicio; no acredita un documento.", .Expediente = r.IdentidadExpediente, .OrigenGabinete = l.Snapshot.Configuracion.OrigenGabinete, .GabineteId = l.Snapshot.Configuracion.GabineteId.Value.ToString(Globalization.CultureInfo.InvariantCulture), .ValoresEfectivos = a.ValoresEfectivos}
        'Excepcion acotada previa al inicio, no una transicion general de efectos inciertos.
        UnaFila(ejecutar("UPDATE ra_integracion_intento SET estado='RECHAZADO',evidencia_doc=@evidencia,fin_utc=@ahora,ultimo_contacto_utc=@ahora WHERE intento_id=@intento AND lote_id=@lote AND raiz_id=@raiz AND archivo_id=@archivo AND fase='ALMACENAR' AND estado='RESERVADO' AND evidencia_doc IS NULL AND fin_utc IS NULL AND token_reclamacion=@token AND solicitud_hash=@hash", New Object() {"@evidencia", PerfilImportacionJson.Escribir(resultado), "@ahora", instante, "@intento", IdOperacion(i.IntentoId), "@lote", IdOperacion(l.LoteId), "@raiz", IdOperacion(r.RaizId), "@archivo", IdOperacion(a.ArchivoId), "@token", i.TokenReclamacion, "@hash", BytesHuella(i.SolicitudHash)}))
        UnaFila(ejecutar("UPDATE ra_integracion_archivo SET estado_efecto='NO_INICIADO',intento_activo_id=NULL,revision=revision+1,actualizado_utc=@ahora WHERE archivo_id=@archivo AND raiz_id=@raiz AND revision=@revision AND estado_efecto='EN_CURSO' AND intento_activo_id=@intento AND resultado_doc IS NULL", New Object() {"@ahora", instante, "@archivo", IdOperacion(a.ArchivoId), "@raiz", IdOperacion(r.RaizId), "@revision", a.Revision, "@intento", IdOperacion(i.IntentoId)}))
        UnaFila(ejecutar("UPDATE ra_integracion_lote SET revision=revision+1,actualizado_utc=@ahora WHERE lote_id=@lote AND revision=@revision AND estado='EN_CURSO' AND detencion_solicitada=0", New Object() {"@ahora", instante, "@lote", IdOperacion(l.LoteId), "@revision", l.Revision}))
        evento(resultado)
        Return resultado
    End Function

    Public Function ObtenerVistaReconciliacion(contexto As ContextoImportacionCarpetas, loteId As String, intentoId As String) As VistaReconciliacionImportacion
        AutorizarOperacion(contexto) : IdOperacion(loteId) : IdOperacion(intentoId)
        Return Operar(Function(conexion, tx) CargarVistaReconciliacion(conexion, tx, contexto, loteId, intentoId))
    End Function
    Private Shared Function CargarVistaReconciliacion(conexion As MySqlConnection, tx As MySqlTransaction, contexto As ContextoImportacionCarpetas, loteId As String, intentoId As String) As VistaReconciliacionImportacion
        Dim lote = BloquearLoteRecepcion(conexion, tx, contexto, loteId)
        Dim intento As IntentoImportacionCarpetas
        Using cmd = ComandoOperacion(conexion, tx, "SELECT * FROM ra_integracion_intento WHERE intento_id=@intento AND lote_id=@lote", "@intento", IdOperacion(intentoId), "@lote", IdOperacion(loteId))
            Using reader = cmd.ExecuteReader()
                If Not reader.Read() Then Throw AccesoDenegado()
                intento = LeerIntentoDocumental(reader)
            End Using
        End Using
        Dim raiz = CargarRaizControl(conexion, tx, loteId, intento.RaizId)
        Dim archivo = If(intento.ArchivoId Is Nothing, Nothing, CargarArchivoRecepcion(conexion, tx, contexto, loteId, intento.ArchivoId, True))
        Dim plan = CargarPlan(conexion, tx, contexto, IdOperacion(lote.PlanId), False)
        Dim vista As New VistaReconciliacionImportacion With {.Lote = lote, .Plan = plan, .Raiz = raiz, .Archivo = archivo, .Intento = intento}
        CoordinadorImportacionCarpetas.ValidarVistaReconciliacion(vista, contexto, loteId, intentoId)
        Return vista
    End Function
    Public Function GuardarReconciliacion(esperada As VistaReconciliacionImportacion, resultado As ResultadoEfectoImportacion) As ResultadoEfectoImportacion
        If esperada Is Nothing OrElse esperada.Lote Is Nothing Then Throw SolicitudInvalida()
        Dim contexto = esperada.Lote.Contexto
        AutorizarOperacion(contexto)
        Return Operar(Function(conexion, tx)
                          Dim actual = CargarVistaReconciliacion(conexion, tx, contexto, esperada.Lote.LoteId, esperada.Intento.IntentoId)
                          Dim instante = Ahora(), nueva = actual.Intento.Estado <> "CONFIRMADO"
                          Dim normal = PersistirReconciliacion(esperada, actual, resultado, instante,
                              Function(sql, parametros)
                                  Using cmd = ComandoOperacion(conexion, tx, sql, parametros)
                                      Return cmd.ExecuteNonQuery()
                                  End Using
                              End Function,
                              Sub(r) RegistrarEventoControl(conexion, tx, contexto, actual.Lote.LoteId, If(actual.Archivo Is Nothing, "RAIZ", "ARCHIVO"), If(actual.Archivo Is Nothing, actual.Raiz.RaizId, actual.Archivo.ArchivoId), actual.Intento.IntentoId, "RECONCILIAR_RESULTADO", r.Codigo, instante, r))
                          If nueva Then ConsolidarEnTransaccion(conexion, tx, contexto, actual.Lote.LoteId, instante)
                          Return normal
                      End Function)
    End Function

    Friend Shared Function ResultadoConfirmadoReconciliacion(vista As VistaReconciliacionImportacion) As ResultadoEfectoImportacion
        If Not CoordinadorImportacionCarpetas.PuedeReconciliar(vista) Then Throw SolicitudInvalida()
        Dim normal = PerfilImportacionJson.Leer(Of ResultadoEfectoImportacion)(PerfilImportacionJson.Escribir(vista.Intento.Resultado))
        normal.Estado = "CONFIRMADO"
        If vista.Intento.Fase = "CREAR_EXPEDIENTE" Then
            Dim comando = ComandoCreacionExpedienteImportacion.PrepararVerificacion(vista.Lote.Contexto, vista.Lote, vista.Raiz, vista.Intento)
            Return NormalizarResultadoCreacion(comando, normal)
        End If
        Dim documento = ComandoAlmacenamientoCarpetas.PrepararVerificacion(vista.Lote.Contexto, vista.Raiz, vista.Archivo, vista.Lote.Snapshot, vista.Intento)
        Return NormalizarResultadoDocumento(documento, normal)
    End Function

    Friend Shared Function PersistirReconciliacion(esperada As VistaReconciliacionImportacion, actual As VistaReconciliacionImportacion, resultado As ResultadoEfectoImportacion, instante As DateTime, ejecutar As Func(Of String, Object(), Integer), evento As Action(Of ResultadoEfectoImportacion)) As ResultadoEfectoImportacion
        If esperada Is Nothing OrElse esperada.Lote Is Nothing OrElse esperada.Intento Is Nothing OrElse actual Is Nothing OrElse resultado Is Nothing OrElse ejecutar Is Nothing OrElse evento Is Nothing Then Throw SolicitudInvalida()
        CoordinadorImportacionCarpetas.ValidarVistaReconciliacion(esperada, esperada.Lote.Contexto, esperada.Lote.LoteId, esperada.Intento.IntentoId)
        CoordinadorImportacionCarpetas.ValidarVistaReconciliacion(actual, esperada.Lote.Contexto, esperada.Lote.LoteId, esperada.Intento.IntentoId)
        Dim normal = ResultadoConfirmadoReconciliacion(esperada)
        If PerfilImportacionJson.Escribir(resultado) <> PerfilImportacionJson.Escribir(normal) Then Throw ConflictoOperacion()
        If CoordinadorImportacionCarpetas.ConfirmacionReconciliacionPersistida(actual) Then
            If PerfilImportacionJson.Escribir(actual.Intento.Resultado) <> PerfilImportacionJson.Escribir(normal) Then Throw ConflictoOperacion()
            Return normal
        End If
        If PerfilImportacionJson.Escribir(esperada) <> PerfilImportacionJson.Escribir(actual) OrElse Not CoordinadorImportacionCarpetas.PuedeReconciliar(actual) OrElse actual.Lote.Revision >= UInteger.MaxValue - 1UI OrElse actual.Raiz.Revision >= UInteger.MaxValue - 1UI OrElse (actual.Archivo IsNot Nothing AndAlso actual.Archivo.Revision >= UInteger.MaxValue - 1UI) Then Throw ConflictoOperacion()
        Dim i = actual.Intento, raiz = actual.Raiz, archivo = actual.Archivo
        CoordinadorImportacionCarpetas.ValidarTransicion("INTENTO", i.Estado, "CONFIRMADO")
        UnaFila(ejecutar("UPDATE ra_integracion_intento SET estado='CONFIRMADO',evidencia_doc=@evidencia,fin_utc=@ahora,ultimo_contacto_utc=@ahora WHERE intento_id=@intento AND lote_id=@lote AND raiz_id=@raiz AND fase=@fase AND token_reclamacion=@token AND solicitud_hash=@hash AND estado='INCIERTO' AND fin_utc IS NULL", New Object() {"@evidencia", PerfilImportacionJson.Escribir(normal), "@ahora", instante, "@intento", IdOperacion(i.IntentoId), "@lote", IdOperacion(i.LoteId), "@raiz", IdOperacion(i.RaizId), "@fase", i.Fase, "@token", i.TokenReclamacion, "@hash", BytesHuella(i.SolicitudHash)}))
        If archivo Is Nothing Then
            CoordinadorImportacionCarpetas.ValidarTransicion("RAIZ", raiz.Estado, "EXPEDIENTE_CONFIRMADO")
            UnaFila(ejecutar("UPDATE ra_integracion_raiz SET estado='EXPEDIENTE_CONFIRMADO',identidad_expediente_doc=@identidad,intento_activo_id=NULL,revision=revision+1,actualizado_utc=@ahora WHERE raiz_id=@raiz AND lote_id=@lote AND revision=@revision AND estado='INCIERTO' AND intento_activo_id=@intento AND identidad_expediente_doc IS NULL", New Object() {"@identidad", PerfilImportacionJson.Escribir(normal.Expediente), "@ahora", instante, "@raiz", IdOperacion(raiz.RaizId), "@lote", IdOperacion(i.LoteId), "@revision", raiz.Revision, "@intento", IdOperacion(i.IntentoId)}))
        Else
            CoordinadorImportacionCarpetas.ValidarTransicion("EFECTO", archivo.EstadoEfecto, "COMPLETADO")
            UnaFila(ejecutar("UPDATE ra_integracion_archivo SET estado_efecto='COMPLETADO',resultado_doc=@resultado,intento_activo_id=NULL,revision=revision+1,actualizado_utc=@ahora WHERE archivo_id=@archivo AND raiz_id=@raiz AND revision=@revision AND estado_efecto='INCIERTO' AND intento_activo_id=@intento", New Object() {"@resultado", PerfilImportacionJson.Escribir(normal), "@ahora", instante, "@archivo", IdOperacion(archivo.ArchivoId), "@raiz", IdOperacion(raiz.RaizId), "@revision", archivo.Revision, "@intento", IdOperacion(i.IntentoId)}))
            If raiz.Estado = "INCIERTO" Then
                CoordinadorImportacionCarpetas.ValidarTransicion("RAIZ", raiz.Estado, "EXPEDIENTE_CONFIRMADO")
                UnaFila(ejecutar("UPDATE ra_integracion_raiz SET estado='EXPEDIENTE_CONFIRMADO',revision=revision+1,actualizado_utc=@ahora WHERE raiz_id=@raiz AND lote_id=@lote AND revision=@revision AND estado='INCIERTO' AND intento_activo_id IS NULL", New Object() {"@ahora", instante, "@raiz", IdOperacion(raiz.RaizId), "@lote", IdOperacion(i.LoteId), "@revision", raiz.Revision}))
            End If
        End If
        'Esto habilita solo el calculo de agregados, nunca una nueva invocacion.
        'Otras incertidumbres volveran a conservar PENDIENTE_VERIFICACION.
        Dim estadoLote = If(actual.Lote.DetencionSolicitada OrElse actual.Lote.Estado = "DETENIDO", "DETENIDO", "LISTO")
        If estadoLote <> actual.Lote.Estado Then CoordinadorImportacionCarpetas.ValidarTransicion("LOTE", actual.Lote.Estado, estadoLote)
        UnaFila(ejecutar("UPDATE ra_integracion_lote SET estado=@estado,revision=revision+1,actualizado_utc=@ahora WHERE lote_id=@lote AND revision=@revision AND estado=@anterior", New Object() {"@estado", estadoLote, "@ahora", instante, "@lote", IdOperacion(i.LoteId), "@revision", actual.Lote.Revision, "@anterior", actual.Lote.Estado}))
        evento(normal)
        Return normal
    End Function
End Class
