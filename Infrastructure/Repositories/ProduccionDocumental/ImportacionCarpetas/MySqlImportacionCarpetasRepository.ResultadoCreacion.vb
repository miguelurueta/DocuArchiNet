Option Strict On
Option Compare Binary
Imports System
Imports System.Data
Imports System.Linq
Imports MySql.Data.MySqlClient

'Checkpoint del resultado ya producido por el adaptador; no ejecuta ni
'reconcilia efectos. El mismo repositorio conserva raiz, intento y evento.
Public Partial Class MySqlImportacionCarpetasRepository
    Public Function RegistrarResultadoCreacion(comando As ComandoCreacionExpedienteImportacion, resultado As ResultadoEfectoImportacion) As ResultadoEfectoImportacion
        If comando Is Nothing OrElse comando.EsVerificacion Then Throw SolicitudInvalida()
        Dim normal = NormalizarResultadoCreacion(comando, resultado)
        Dim contexto = comando.Contexto
        AutorizarOperacion(contexto)
        Return Operar(Function(conexion, tx)
                          Dim lote = BloquearLoteRecepcion(conexion, tx, contexto, comando.LoteId)
                          Dim raiz = CargarRaizControl(conexion, tx, lote.LoteId, comando.RaizId)
                          Dim intento As IntentoImportacionCarpetas
                          Using cmd = ComandoOperacion(conexion, tx, "SELECT * FROM ra_integracion_intento WHERE intento_id=@intento AND lote_id=@lote AND raiz_id=@raiz AND archivo_id IS NULL AND fase='CREAR_EXPEDIENTE' FOR UPDATE", "@intento", IdOperacion(comando.IntentoId), "@lote", IdOperacion(comando.LoteId), "@raiz", IdOperacion(comando.RaizId))
                              Using reader = cmd.ExecuteReader()
                                  If Not reader.Read() Then Throw ConflictoOperacion()
                                  intento = LeerIntentoDocumental(reader)
                                  If reader.Read() Then Throw ConflictoOperacion()
                              End Using
                          End Using
                          Dim instante = Ahora()
                          Dim persistido = PersistirResultadoCreacion(comando, lote, raiz, intento, normal, instante,
                              Function(sql, parametros)
                                  Using cmd = ComandoOperacion(conexion, tx, sql, parametros)
                                      Return cmd.ExecuteNonQuery()
                                  End Using
                              End Function,
                              Sub()
                                  If _observadorConfirmacion IsNot Nothing Then _observadorConfirmacion("CREACION_RESULTADO_ANTES_EVENTO", conexion, tx)
                                  Using cmd = ComandoOperacion(conexion, tx, "INSERT INTO ra_integracion_evento (lote_id,intento_id,recurso_tipo,recurso_id,accion,codigo,actor_ref,ocurrido_utc,detalle_doc) VALUES (@lote,@intento,'RAIZ',@raiz,'RESULTADO_CREACION',@codigo,@actor,@ahora,@detalle)", "@lote", IdOperacion(comando.LoteId), "@intento", IdOperacion(comando.IntentoId), "@raiz", IdOperacion(comando.RaizId), "@codigo", normal.Codigo, "@actor", contexto.UsuarioRef, "@ahora", instante, "@detalle", PerfilImportacionJson.Escribir(normal))
                                      UnaFila(cmd.ExecuteNonQuery())
                                  End Using
                              End Sub)
                          If Not {"CONFIRMADO", "INCIERTO"}.Contains(intento.Estado) Then ConsolidarEnTransaccion(conexion, tx, contexto, lote.LoteId, instante)
                          Return persistido
                      End Function)
    End Function

    Friend Shared Function NormalizarResultadoCreacion(comando As ComandoCreacionExpedienteImportacion, original As ResultadoEfectoImportacion) As ResultadoEfectoImportacion
        If comando Is Nothing OrElse original Is Nothing OrElse Not {"CONFIRMADO", "INCIERTO"}.Contains(original.Estado) OrElse original.Creacion Is Nothing OrElse original.Creacion.IntentoId <> comando.IntentoId OrElse Not ValidadorPerfilImportacion.ReferenciaValida(original.Creacion.CicloArchivoId) OrElse original.Creacion.CicloArchivoNombre <> comando.CicloArchivoNombre Then Throw SolicitudInvalida()
        If original.Recepcion IsNot Nothing OrElse original.DocumentoId IsNot Nothing OrElse original.InventarioId IsNot Nothing OrElse original.GabineteId IsNot Nothing OrElse original.OrigenGabinete IsNot Nothing OrElse original.ValoresEfectivos Is Nothing OrElse original.ValoresEfectivos.Count <> 0 Then Throw SolicitudInvalida()
        Dim identidad = original.Expediente
        If identidad IsNot Nothing Then
            If identidad.OrigenLogico <> "RA" OrElse identidad.NivelRef <> comando.Contexto.NivelRef OrElse (identidad.ExpedienteId Is Nothing AndAlso identidad.RelacionNivelId Is Nothing) Then Throw SolicitudInvalida()
            For Each valor In New String() {identidad.ExpedienteId, identidad.RelacionNivelId}
                If valor IsNot Nothing AndAlso Not ValidadorPerfilImportacion.ReferenciaValida(valor) Then Throw SolicitudInvalida()
            Next
        End If
        If original.Estado = "CONFIRMADO" AndAlso (identidad Is Nothing OrElse identidad.ExpedienteId Is Nothing OrElse identidad.RelacionNivelId Is Nothing) Then Throw SolicitudInvalida()
        Dim normal = PerfilImportacionJson.Leer(Of ResultadoEfectoImportacion)(PerfilImportacionJson.Escribir(original))
        normal.Codigo = If(normal.Estado = "CONFIRMADO", "EXPEDIENTE_CONFIRMADO", "RESULTADO_INCIERTO")
        normal.Mensaje = If(normal.Estado = "CONFIRMADO", "Expediente y relación comprobados en el destino confirmado.", "La creación requiere comprobación antes de cualquier repetición.")
        Return normal
    End Function

    Friend Shared Function PersistirResultadoCreacion(comando As ComandoCreacionExpedienteImportacion, lote As LoteImportacionCarpetas, raiz As RaizImportacionCarpetas, intento As IntentoImportacionCarpetas, resultado As ResultadoEfectoImportacion, instante As DateTime, ejecutar As Func(Of String, Object(), Integer), registrarEvento As Action) As ResultadoEfectoImportacion
        Dim normal = NormalizarResultadoCreacion(comando, resultado)
        If lote Is Nothing OrElse raiz Is Nothing OrElse intento Is Nothing OrElse ejecutar Is Nothing OrElse registrarEvento Is Nothing OrElse Not EstadosImportacionCarpetas.EsLote(lote.Estado) OrElse lote.LoteId <> comando.LoteId OrElse raiz.LoteId <> lote.LoteId OrElse raiz.RaizId <> comando.RaizId OrElse intento.IntentoId <> comando.IntentoId OrElse intento.LoteId <> lote.LoteId OrElse intento.RaizId <> raiz.RaizId OrElse intento.ArchivoId IsNot Nothing OrElse intento.Fase <> "CREAR_EXPEDIENTE" OrElse intento.TokenReclamacion <> comando.TokenReclamacion OrElse intento.SolicitudHash <> comando.SolicitudHash Then Throw ConflictoOperacion()
        If PerfilImportacionJson.Escribir(lote.Contexto) <> PerfilImportacionJson.Escribir(comando.Contexto) OrElse PerfilImportacionJson.Escribir(lote.Snapshot) <> PerfilImportacionJson.Escribir(comando.Snapshot) Then Throw ConflictoOperacion()
        If intento.Resultado Is Nothing OrElse intento.Resultado.Creacion Is Nothing OrElse PerfilImportacionJson.Escribir(intento.Resultado.Creacion) <> PerfilImportacionJson.Escribir(normal.Creacion) Then Throw ConflictoOperacion()
        If {"CONFIRMADO", "INCIERTO"}.Contains(intento.Estado) Then
            If intento.Estado <> normal.Estado OrElse (intento.Estado = "CONFIRMADO" AndAlso intento.FinUtc Is Nothing) OrElse (intento.Estado = "INCIERTO" AndAlso intento.FinUtc IsNot Nothing) Then Throw ConflictoOperacion()
            If PerfilImportacionJson.Escribir(intento.Resultado) <> PerfilImportacionJson.Escribir(normal) Then Throw ConflictoOperacion()
            If intento.Estado = "CONFIRMADO" Then
                If raiz.IdentidadExpediente Is Nothing OrElse PerfilImportacionJson.Escribir(raiz.IdentidadExpediente) <> PerfilImportacionJson.Escribir(normal.Expediente) Then Throw ConflictoOperacion()
            ElseIf raiz.Estado <> "INCIERTO" OrElse raiz.IntentoActivoId <> intento.IntentoId Then
                Throw ConflictoOperacion()
            End If
            Return normal
        End If
        If intento.Estado <> "EN_CURSO" OrElse intento.Resultado.Estado <> "EN_CURSO" OrElse intento.FinUtc IsNot Nothing OrElse raiz.Estado <> "CREANDO" OrElse raiz.IntentoActivoId <> intento.IntentoId OrElse raiz.Revision = 0 OrElse raiz.Revision <> comando.RevisionRaiz OrElse raiz.IdentidadExpediente IsNot Nothing OrElse raiz.Revision >= UInteger.MaxValue - 1UI OrElse lote.Revision = 0 OrElse lote.Revision >= UInteger.MaxValue - 1UI OrElse {"COMPLETADO", "FALLIDO"}.Contains(lote.Estado) Then Throw ConflictoOperacion()
        Dim conocida = intento.Resultado.Expediente, recibida = normal.Expediente
        If conocida IsNot Nothing Then
            If recibida Is Nothing OrElse conocida.OrigenLogico <> recibida.OrigenLogico OrElse conocida.NivelRef <> recibida.NivelRef OrElse (conocida.ExpedienteId IsNot Nothing AndAlso conocida.ExpedienteId <> recibida.ExpedienteId) OrElse (conocida.RelacionNivelId IsNot Nothing AndAlso conocida.RelacionNivelId <> recibida.RelacionNivelId) Then Throw ConflictoOperacion()
        End If
        CoordinadorImportacionCarpetas.ValidarTransicion("INTENTO", intento.Estado, normal.Estado)
        Dim estadoRaiz = If(normal.Estado = "CONFIRMADO", "EXPEDIENTE_CONFIRMADO", "INCIERTO")
        CoordinadorImportacionCarpetas.ValidarTransicion("RAIZ", raiz.Estado, estadoRaiz)
        Dim estadoLote = If(normal.Estado = "INCIERTO", "PENDIENTE_VERIFICACION", lote.Estado)
        If estadoLote <> lote.Estado Then CoordinadorImportacionCarpetas.ValidarTransicion("LOTE", lote.Estado, estadoLote)
        UnaFila(ejecutar("UPDATE ra_integracion_intento SET estado=@estado,evidencia_doc=@evidencia,fin_utc=@fin,ultimo_contacto_utc=@ahora WHERE intento_id=@intento AND lote_id=@lote AND raiz_id=@raiz AND fase='CREAR_EXPEDIENTE' AND archivo_id IS NULL AND token_reclamacion=@token AND solicitud_hash=@hash AND estado='EN_CURSO' AND fin_utc IS NULL", New Object() {"@estado", normal.Estado, "@evidencia", PerfilImportacionJson.Escribir(normal), "@fin", If(normal.Estado = "CONFIRMADO", DirectCast(instante, Object), DBNull.Value), "@ahora", instante, "@intento", IdOperacion(intento.IntentoId), "@lote", IdOperacion(lote.LoteId), "@raiz", IdOperacion(raiz.RaizId), "@token", comando.TokenReclamacion, "@hash", BytesHuella(comando.SolicitudHash)}))
        UnaFila(ejecutar("UPDATE ra_integracion_raiz SET estado=@estado,identidad_expediente_doc=@identidad,intento_activo_id=@activo,revision=revision+1,actualizado_utc=@ahora WHERE raiz_id=@raiz AND lote_id=@lote AND revision=@revision AND estado='CREANDO' AND intento_activo_id=@intento AND identidad_expediente_doc IS NULL", New Object() {"@estado", estadoRaiz, "@identidad", If(normal.Estado = "CONFIRMADO", DirectCast(PerfilImportacionJson.Escribir(normal.Expediente), Object), DBNull.Value), "@activo", If(normal.Estado = "CONFIRMADO", DirectCast(DBNull.Value, Object), IdOperacion(intento.IntentoId)), "@ahora", instante, "@raiz", IdOperacion(raiz.RaizId), "@lote", IdOperacion(lote.LoteId), "@revision", raiz.Revision, "@intento", IdOperacion(intento.IntentoId)}))
        UnaFila(ejecutar("UPDATE ra_integracion_lote SET estado=@estado,revision=revision+1,actualizado_utc=@ahora WHERE lote_id=@lote AND revision=@revision", New Object() {"@estado", estadoLote, "@ahora", instante, "@lote", IdOperacion(lote.LoteId), "@revision", lote.Revision}))
        registrarEvento()
        Return normal
    End Function

    Private Shared Function LeerIntentoDocumental(reader As IDataRecord) As IntentoImportacionCarpetas
        Dim intento As New IntentoImportacionCarpetas With {.IntentoId = Texto(reader("intento_id")), .LoteId = Texto(reader("lote_id")), .RaizId = Texto(reader("raiz_id")), .ArchivoId = If(reader.IsDBNull(reader.GetOrdinal("archivo_id")), Nothing, Texto(reader("archivo_id"))), .Fase = Texto(reader("fase")), .Estado = Texto(reader("estado")), .EjecutorRef = Texto(reader("ejecutor_ref")), .TokenReclamacion = Texto(reader("token_reclamacion")), .SolicitudHash = Hexadecimal(DirectCast(reader("solicitud_hash"), Byte())), .InicioUtc = TextoUtc(CDate(reader("inicio_utc"))), .UltimoContactoUtc = TextoUtc(CDate(reader("ultimo_contacto_utc")))}
        If Not reader.IsDBNull(reader.GetOrdinal("fin_utc")) Then intento.FinUtc = TextoUtc(CDate(reader("fin_utc")))
        If Not reader.IsDBNull(reader.GetOrdinal("evidencia_doc")) Then intento.Resultado = LeerPersistido(Of ResultadoEfectoImportacion)(CStr(reader("evidencia_doc")))
        If Not EstadosImportacionCarpetas.EsIntento(intento.Estado) OrElse Not HexExacto(intento.TokenReclamacion, 32) Then Throw NoDisponible()
        Return intento
    End Function
End Class
