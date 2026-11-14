Option Strict On
Option Compare Binary
Imports System
Imports System.Data
Imports System.Linq
Imports System.Collections.Generic
Imports MySql.Data.MySqlClient

Public Partial Class MySqlImportacionCarpetasRepository
    Private Shared Function SinOtrosReclamosRetomar() As String
        Return "NOT EXISTS(SELECT 1 FROM ra_integracion_intento oi WHERE oi.lote_id=l.lote_id AND oi.intento_id<>i.intento_id AND oi.estado IN ('RESERVADO','EN_CURSO','INCIERTO')) AND NOT EXISTS(SELECT 1 FROM ra_integracion_raiz otras WHERE otras.lote_id=l.lote_id AND otras.raiz_id<>r.raiz_id AND (otras.intento_activo_id IS NOT NULL OR otras.estado IN ('CREANDO','INCIERTO'))) AND NOT EXISTS(SELECT 1 FROM ra_integracion_archivo af INNER JOIN ra_integracion_raiz ar ON ar.raiz_id=af.raiz_id WHERE ar.lote_id=l.lote_id AND (af.intento_activo_id IS NOT NULL OR af.estado_recepcion IN ('RECIBIENDO','INCIERTO') OR af.estado_efecto IN ('EN_CURSO','INCIERTO')))"
    End Function

    Private Shared Function EventoRetomarValido() As String
        Return "e.lote_id=l.lote_id AND e.recurso_tipo='RAIZ' AND e.recurso_id=r.raiz_id AND e.actor_ref=l.solicitante_ref AND ((e.accion='RESERVAR_INTENTO' AND e.codigo='INTENTO_RESERVADO') OR (e.accion='RETOMAR_CREACION' AND e.codigo='RESERVA_REANUDADA'))"
    End Function

    Friend Shared Function PredicadoRetomarCreacion() As String
        Return "l.estado='EN_CURSO' AND l.detencion_solicitada IN (0,1) AND l.revision BETWEEN 1 AND 4294967290 AND r.revision BETWEEN 1 AND 4294967292 AND r.estado='CREANDO' AND r.identidad_expediente_doc IS NULL AND (r.valores_efectivos_doc IS NULL OR r.valores_efectivos_doc='[]') AND r.intento_activo_id=i.intento_id AND i.lote_id=l.lote_id AND i.raiz_id=r.raiz_id AND i.archivo_id IS NULL AND i.fase='CREAR_EXPEDIENTE' AND i.ejecutor_ref='WEB_CREACION' AND i.estado='RESERVADO' AND i.fin_utc IS NULL AND i.evidencia_doc IS NULL AND (SELECT COUNT(*) FROM ra_integracion_evento e WHERE e.intento_id=i.intento_id AND e.accion='RESERVAR_INTENTO' AND e.codigo='INTENTO_RESERVADO' AND e.lote_id=l.lote_id AND e.recurso_tipo='RAIZ' AND e.recurso_id=r.raiz_id AND e.actor_ref=l.solicitante_ref)=1 AND NOT EXISTS(SELECT 1 FROM ra_integracion_evento e WHERE e.intento_id=i.intento_id AND NOT(COALESCE((" & EventoRetomarValido() & "),0))) AND " & SinOtrosReclamosRetomar()
    End Function

    Friend Shared Sub CompletarRetomarCreacionConsulta(lote As LoteImportacionCarpetas, pagina As PaginaRecursosImportacion(Of RaizImportacionCarpetas), consultar As Func(Of String, Object(), DataTable))
        If lote Is Nothing OrElse lote.Contexto Is Nothing OrElse pagina Is Nothing OrElse consultar Is Nothing Then Throw NoDisponible()
        For Each raiz In pagina.Elementos
            raiz.PuedeRetomarCreacion = False
        Next
        If lote.Estado <> "EN_CURSO" Then Return
        Dim candidatas = pagina.Elementos.Where(Function(r) r.Estado = "CREANDO" AndAlso r.IntentoActivoId IsNot Nothing AndAlso r.IdentidadExpediente Is Nothing).ToList()
        If candidatas.Count = 0 Then Return
        Dim parametros As New List(Of Object) From {"@lote", IdOperacion(lote.LoteId), "@usuario", lote.Contexto.UsuarioRef, "@ambito", lote.Contexto.AmbitoRef, "@nivel", lote.Contexto.NivelRef}
        Dim nombres As New List(Of String)()
        For Each raiz In candidatas
            Dim nombre = "@r" & nombres.Count.ToString(Globalization.CultureInfo.InvariantCulture)
            nombres.Add(nombre) : parametros.AddRange(New Object() {nombre, IdOperacion(raiz.RaizId)})
        Next
        Dim sql = "SELECT r.raiz_id,CASE WHEN " & PredicadoRetomarCreacion() & " THEN 1 ELSE 0 END AS elegible FROM ra_integracion_lote l INNER JOIN ra_integracion_raiz r ON r.lote_id=l.lote_id LEFT JOIN ra_integracion_intento i ON i.intento_id=r.intento_activo_id WHERE l.lote_id=@lote AND l.solicitante_ref=@usuario AND l.ambito_ref=@ambito AND l.nivel_ref=@nivel AND r.raiz_id IN (" & String.Join(",", nombres) & ") ORDER BY r.raiz_id"
        Dim datos = consultar(sql, parametros.ToArray())
        If datos Is Nothing OrElse datos.Rows.Count <> candidatas.Count OrElse Not datos.Columns.Contains("raiz_id") OrElse Not datos.Columns.Contains("elegible") Then Throw NoDisponible()
        Dim vistos As New HashSet(Of String)(StringComparer.Ordinal)
        For Each fila As DataRow In datos.Rows
            Dim id = Texto(fila("raiz_id")), raiz = candidatas.SingleOrDefault(Function(r) r.RaizId = id)
            If raiz Is Nothing OrElse Not vistos.Add(id) OrElse fila.IsNull("elegible") Then Throw NoDisponible()
            raiz.PuedeRetomarCreacion = Convert.ToBoolean(fila("elegible"), Globalization.CultureInfo.InvariantCulture)
        Next
    End Sub

    Public Function ReanudarReservaCreacion(esperada As VistaReconciliacionImportacion, nuevoToken As String) As IntentoImportacionCarpetas
        If esperada Is Nothing OrElse esperada.Lote Is Nothing OrElse esperada.Raiz Is Nothing OrElse esperada.Intento Is Nothing Then Throw SolicitudInvalida()
        Dim contexto = esperada.Lote.Contexto
        AutorizarOperacion(contexto)
        Return Operar(Function(conexion, tx)
                          'Orden de locks común: lote, raíz, intento. La lectura
                          'del diario/otros reclamos ocurre después del lock del lote.
                          Dim lote = BloquearLoteRecepcion(conexion, tx, contexto, esperada.Lote.LoteId)
                          Dim raiz = CargarRaizControl(conexion, tx, lote.LoteId, esperada.Raiz.RaizId)
                          Dim intento As IntentoImportacionCarpetas
                          Using cmd = ComandoOperacion(conexion, tx, "SELECT * FROM ra_integracion_intento WHERE intento_id=@intento AND lote_id=@lote AND raiz_id=@raiz AND archivo_id IS NULL AND fase='CREAR_EXPEDIENTE' FOR UPDATE", "@intento", IdOperacion(esperada.Intento.IntentoId), "@lote", IdOperacion(lote.LoteId), "@raiz", IdOperacion(raiz.RaizId))
                              Using reader = cmd.ExecuteReader()
                                  If Not reader.Read() Then Throw ConflictoOperacion()
                                  intento = LeerIntentoDocumental(reader)
                                  If reader.Read() Then Throw ConflictoOperacion()
                              End Using
                          End Using
                          Dim actual As New VistaReconciliacionImportacion With {.Lote = lote, .Raiz = raiz, .Intento = intento, .Plan = CargarPlan(conexion, tx, contexto, IdOperacion(lote.PlanId), False)}
                          Dim parametros As Object() = {"@lote", IdOperacion(lote.LoteId), "@raiz", IdOperacion(raiz.RaizId), "@intento", IdOperacion(intento.IntentoId)}
                          Dim diario = LeerTablaConsulta(conexion, tx, "SELECT COALESCE(SUM(CASE WHEN e.accion='RESERVAR_INTENTO' AND e.codigo='INTENTO_RESERVADO' AND " & EventoRetomarValido() & " THEN 1 ELSE 0 END),0) AS reservas,COALESCE(SUM(CASE WHEN " & EventoRetomarValido() & " THEN 0 ELSE 1 END),0) AS otras FROM ra_integracion_evento e INNER JOIN ra_integracion_intento i ON i.intento_id=e.intento_id INNER JOIN ra_integracion_raiz r ON r.raiz_id=i.raiz_id INNER JOIN ra_integracion_lote l ON l.lote_id=i.lote_id WHERE i.intento_id=@intento AND i.lote_id=@lote AND i.raiz_id=@raiz", parametros)
                          Dim reclamos = LeerTablaConsulta(conexion, tx, "SELECT CASE WHEN " & SinOtrosReclamosRetomar() & " THEN 0 ELSE 1 END AS otros FROM ra_integracion_lote l INNER JOIN ra_integracion_raiz r ON r.lote_id=l.lote_id INNER JOIN ra_integracion_intento i ON i.intento_id=r.intento_activo_id WHERE l.lote_id=@lote AND r.raiz_id=@raiz AND i.intento_id=@intento", parametros)
                          If diario.Rows.Count <> 1 OrElse reclamos.Rows.Count <> 1 Then Throw ConflictoOperacion()
                          Dim instante = Ahora()
                          Return PersistirReanudacionCreacion(esperada, actual, nuevoToken, Convert.ToInt32(diario.Rows(0)("reservas")), Convert.ToInt32(diario.Rows(0)("otras")), Convert.ToBoolean(reclamos.Rows(0)("otros")), instante,
                              Function(sql, argumentos)
                                  Using cmd = ComandoOperacion(conexion, tx, sql, argumentos)
                                      Return cmd.ExecuteNonQuery()
                                  End Using
                              End Function,
                              Sub() RegistrarEventoControl(conexion, tx, contexto, lote.LoteId, "RAIZ", raiz.RaizId, intento.IntentoId, "RETOMAR_CREACION", "RESERVA_REANUDADA", instante))
                      End Function)
    End Function

    Friend Shared Function PersistirReanudacionCreacion(esperada As VistaReconciliacionImportacion, actual As VistaReconciliacionImportacion, token As String, reservas As Integer, otras As Integer, otrosReclamos As Boolean, instante As DateTime, ejecutar As Func(Of String, Object(), Integer), evento As Action) As IntentoImportacionCarpetas
        If esperada Is Nothing OrElse esperada.Lote Is Nothing OrElse esperada.Intento Is Nothing OrElse actual Is Nothing OrElse ejecutar Is Nothing OrElse evento Is Nothing Then Throw SolicitudInvalida()
        Dim contexto = esperada.Lote.Contexto
        CoordinadorImportacionCarpetas.ValidarVistaReconciliacion(esperada, contexto, esperada.Lote.LoteId, esperada.Intento.IntentoId)
        CoordinadorImportacionCarpetas.ValidarVistaReconciliacion(actual, contexto, esperada.Lote.LoteId, esperada.Intento.IntentoId)
        CoordinadorImportacionCarpetas.ExigirReservaRetomable(esperada) : CoordinadorImportacionCarpetas.ExigirReservaRetomable(actual)
        If PerfilImportacionJson.Escribir(esperada) <> PerfilImportacionJson.Escribir(actual) OrElse reservas <> 1 OrElse otras <> 0 OrElse otrosReclamos OrElse Not HexExacto(token, 32) OrElse token = actual.Intento.TokenReclamacion Then Throw ConflictoOperacion()
        Dim l = actual.Lote, r = actual.Raiz, i = actual.Intento
        UnaFila(ejecutar("UPDATE ra_integracion_intento SET token_reclamacion=@nuevo,ultimo_contacto_utc=@ahora WHERE intento_id=@intento AND lote_id=@lote AND raiz_id=@raiz AND archivo_id IS NULL AND fase='CREAR_EXPEDIENTE' AND ejecutor_ref='WEB_CREACION' AND estado='RESERVADO' AND fin_utc IS NULL AND evidencia_doc IS NULL AND token_reclamacion=@previo AND solicitud_hash=@hash", New Object() {"@nuevo", token, "@ahora", instante, "@intento", IdOperacion(i.IntentoId), "@lote", IdOperacion(l.LoteId), "@raiz", IdOperacion(r.RaizId), "@previo", i.TokenReclamacion, "@hash", BytesHuella(i.SolicitudHash)}))
        UnaFila(ejecutar("UPDATE ra_integracion_raiz SET revision=revision+1,actualizado_utc=@ahora WHERE raiz_id=@raiz AND lote_id=@lote AND revision=@revision AND estado='CREANDO' AND intento_activo_id=@intento AND identidad_expediente_doc IS NULL", New Object() {"@ahora", instante, "@raiz", IdOperacion(r.RaizId), "@lote", IdOperacion(l.LoteId), "@revision", r.Revision, "@intento", IdOperacion(i.IntentoId)}))
        UnaFila(ejecutar("UPDATE ra_integracion_lote SET detencion_solicitada=0,revision=revision+1,actualizado_utc=@ahora WHERE lote_id=@lote AND solicitante_ref=@usuario AND ambito_ref=@ambito AND revision=@revision AND estado='EN_CURSO' AND detencion_solicitada=@detencion", New Object() {"@ahora", instante, "@lote", IdOperacion(l.LoteId), "@usuario", contexto.UsuarioRef, "@ambito", contexto.AmbitoRef, "@revision", l.Revision, "@detencion", If(l.DetencionSolicitada, 1, 0)}))
        evento()
        Dim salida = PerfilImportacionJson.Leer(Of IntentoImportacionCarpetas)(PerfilImportacionJson.Escribir(i))
        salida.TokenReclamacion = token : salida.UltimoContactoUtc = TextoUtc(instante) : salida.EsNuevaReserva = True
        Return salida
    End Function
End Class
