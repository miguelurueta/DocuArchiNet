Option Strict On
Option Compare Binary
Imports System
Imports System.Globalization
Imports System.Linq

Public Partial Class MySqlImportacionCarpetasRepository
    Public Function ObtenerIntentoAlmacenamiento(contexto As ContextoImportacionCarpetas, solicitud As ReclamacionImportacionCarpetas) As IntentoImportacionCarpetas
        AutorizarOperacion(contexto) : ValidarSolicitudDocumental(solicitud)
        If solicitud.Fase <> "ALMACENAR" Then Throw SolicitudInvalida()
        Return Operar(Function(conexion, tx)
                          If BuscarLote(conexion, tx, contexto, "lote_id", IdOperacion(solicitud.LoteId)) Is Nothing Then Throw AccesoDenegado()
                          Dim archivo = CargarArchivoRecepcion(conexion, tx, contexto, solicitud.LoteId, solicitud.RecursoId, False)
                          Return CargarRepeticionDocumental(conexion, tx, solicitud, archivo.RaizId)
                      End Function)
    End Function

    Public Function IniciarAlmacenamientoDocumento(comando As ComandoAlmacenamientoCarpetas, datos As DatosAlmacenamientoImportacion) As Boolean
        If comando Is Nothing Then Throw SolicitudInvalida()
        If comando.EsVerificacion Then Throw SolicitudInvalida()
        comando.ValidarDatos(datos)
        AutorizarOperacion(comando.Contexto)
        Return Operar(Function(conexion, tx)
                          Dim lote = BloquearLoteRecepcion(conexion, tx, comando.Contexto, comando.Raiz.LoteId)
                          Dim raiz = CargarRaizControl(conexion, tx, lote.LoteId, comando.Raiz.RaizId)
                          Dim archivo = CargarArchivoRecepcion(conexion, tx, comando.Contexto, lote.LoteId, comando.Archivo.ArchivoId, True)
                          ValidarInicioDocumento(comando, lote, raiz, archivo)
                          Dim plan = CargarPlan(conexion, tx, comando.Contexto, IdOperacion(lote.PlanId), False)
                          Dim vista = LeerVistaBarreraCreacion(lote, plan, raiz, Function(sql, parametros) LeerTablaConsulta(conexion, tx, sql, parametros))
                          If Not CoordinadorImportacionCarpetas.EvaluarBarreraAlmacenamiento(vista, archivo, comando.Intento.IntentoId) Then Return False
                          If Not LeerClasificacionExpedienteConocido(comando.Contexto, raiz.IdentidadExpediente, lote.Snapshot.Configuracion, Function(sql, parametros) LeerTablaConsulta(conexion, tx, sql, parametros)) Then Throw ConflictoOperacion()
                          Dim intento = comando.Intento, instante = Ahora()
                          Dim evidencia = EvidenciaInicialDocumento(comando)
                          Using cmd = ComandoOperacion(conexion, tx, "UPDATE ra_integracion_intento SET estado='EN_CURSO',ultimo_contacto_utc=@ahora,evidencia_doc=@evidencia WHERE intento_id=@intento AND lote_id=@lote AND raiz_id=@raiz AND archivo_id=@archivo AND fase='ALMACENAR' AND estado='RESERVADO' AND evidencia_doc IS NULL AND token_reclamacion=@token AND solicitud_hash=@hash AND fin_utc IS NULL", "@ahora", instante, "@evidencia", PerfilImportacionJson.Escribir(evidencia), "@intento", IdOperacion(intento.IntentoId), "@lote", IdOperacion(lote.LoteId), "@raiz", IdOperacion(raiz.RaizId), "@archivo", IdOperacion(archivo.ArchivoId), "@token", intento.TokenReclamacion, "@hash", BytesHuella(intento.SolicitudHash))
                              Dim afectadas = cmd.ExecuteNonQuery()
                              If afectadas = 0 Then Return False
                              UnaFila(afectadas)
                          End Using
                          Using cmd = ComandoOperacion(conexion, tx, "UPDATE ra_integracion_lote SET revision=revision+1,actualizado_utc=@ahora WHERE lote_id=@lote AND revision=@revision AND estado='EN_CURSO' AND detencion_solicitada=0", "@ahora", instante, "@lote", IdOperacion(lote.LoteId), "@revision", lote.Revision)
                              UnaFila(cmd.ExecuteNonQuery())
                          End Using
                          RegistrarEventoControl(conexion, tx, comando.Contexto, lote.LoteId, "ARCHIVO", archivo.ArchivoId, intento.IntentoId, "INICIAR_ALMACENAMIENTO", "ALMACENAMIENTO_PREPARADO", instante, evidencia)
                          Return True
                      End Function)
    End Function

    Friend Shared Sub ValidarInicioDocumento(comando As ComandoAlmacenamientoCarpetas, lote As LoteImportacionCarpetas, raiz As RaizImportacionCarpetas, archivo As ArchivoImportacionCarpetas)
        If comando Is Nothing OrElse lote Is Nothing OrElse raiz Is Nothing OrElse archivo Is Nothing OrElse lote.LoteId <> comando.Raiz.LoteId OrElse lote.Estado <> "EN_CURSO" OrElse lote.DetencionSolicitada OrElse lote.Revision = 0 OrElse lote.Revision >= UInteger.MaxValue - 1UI Then Throw ConflictoOperacion()
        If PerfilImportacionJson.Escribir(lote.Contexto) <> PerfilImportacionJson.Escribir(comando.Contexto) OrElse PerfilImportacionJson.Escribir(lote.Snapshot) <> PerfilImportacionJson.Escribir(comando.Snapshot) OrElse PerfilImportacionJson.Escribir(raiz) <> PerfilImportacionJson.Escribir(comando.Raiz) OrElse PerfilImportacionJson.Escribir(archivo) <> PerfilImportacionJson.Escribir(comando.Archivo) Then Throw ConflictoOperacion()
    End Sub
    Friend Shared Function EvidenciaInicialDocumento(comando As ComandoAlmacenamientoCarpetas) As ResultadoEfectoImportacion
        Return New ResultadoEfectoImportacion With {.Estado = "EN_CURSO", .Codigo = "ALMACENAMIENTO_PREPARADO", .Mensaje = "Inicio registrado; no acredita resultado documental.", .Expediente = comando.Raiz.IdentidadExpediente, .OrigenGabinete = comando.Snapshot.Configuracion.OrigenGabinete, .GabineteId = comando.Snapshot.Configuracion.GabineteId.Value.ToString(CultureInfo.InvariantCulture), .ValoresEfectivos = comando.Archivo.ValoresEfectivos}
    End Function
    Public Function RegistrarResultadoDocumento(comando As ComandoAlmacenamientoCarpetas, resultado As ResultadoEfectoImportacion) As ResultadoEfectoImportacion
        If comando Is Nothing OrElse comando.EsVerificacion Then Throw SolicitudInvalida()
        Dim normal = NormalizarResultadoDocumento(comando, resultado)
        AutorizarOperacion(comando.Contexto)
        Return Operar(Function(conexion, tx)
                          Dim lote = BloquearLoteRecepcion(conexion, tx, comando.Contexto, comando.Raiz.LoteId)
                          Dim raiz = CargarRaizControl(conexion, tx, lote.LoteId, comando.Raiz.RaizId)
                          Dim archivo = CargarArchivoRecepcion(conexion, tx, comando.Contexto, lote.LoteId, comando.Archivo.ArchivoId, True)
                          Dim intento As IntentoImportacionCarpetas
                          Using cmd = ComandoOperacion(conexion, tx, "SELECT * FROM ra_integracion_intento WHERE intento_id=@intento AND lote_id=@lote AND raiz_id=@raiz AND archivo_id=@archivo AND fase='ALMACENAR' FOR UPDATE", "@intento", IdOperacion(comando.Intento.IntentoId), "@lote", IdOperacion(lote.LoteId), "@raiz", IdOperacion(raiz.RaizId), "@archivo", IdOperacion(archivo.ArchivoId))
                              Using reader = cmd.ExecuteReader()
                                  If Not reader.Read() Then Throw ConflictoOperacion()
                                  intento = LeerIntentoDocumental(reader)
                              End Using
                          End Using
                          Dim instante = Ahora()
                          Dim persistido = PersistirResultadoDocumento(comando, lote, raiz, archivo, intento, normal, instante,
                              Function(sql, parametros)
                                  Using cmd = ComandoOperacion(conexion, tx, sql, parametros)
                                      Return cmd.ExecuteNonQuery()
                                  End Using
                              End Function,
                              Sub() RegistrarEventoControl(conexion, tx, comando.Contexto, lote.LoteId, "ARCHIVO", archivo.ArchivoId, intento.IntentoId, "RESULTADO_ALMACENAMIENTO", normal.Codigo, instante, normal))
                          If Not {"CONFIRMADO", "INCIERTO"}.Contains(intento.Estado) Then ConsolidarEnTransaccion(conexion, tx, comando.Contexto, lote.LoteId, instante)
                          Return persistido
                      End Function)
    End Function
    Friend Shared Function NormalizarResultadoDocumento(comando As ComandoAlmacenamientoCarpetas, original As ResultadoEfectoImportacion) As ResultadoEfectoImportacion
        If comando Is Nothing OrElse original Is Nothing OrElse Not {"CONFIRMADO", "INCIERTO"}.Contains(original.Estado) OrElse original.Creacion IsNot Nothing OrElse original.Recepcion IsNot Nothing Then Throw SolicitudInvalida()
        Dim esperado = EvidenciaInicialDocumento(comando)
        If PerfilImportacionJson.Escribir(original.Expediente) <> PerfilImportacionJson.Escribir(esperado.Expediente) OrElse original.OrigenGabinete <> esperado.OrigenGabinete OrElse original.GabineteId <> esperado.GabineteId OrElse PerfilImportacionJson.Escribir(original.ValoresEfectivos) <> PerfilImportacionJson.Escribir(esperado.ValoresEfectivos) Then Throw SolicitudInvalida()
        For Each id In New String() {original.DocumentoId, original.InventarioId}
            Dim numero As Long
            If id IsNot Nothing AndAlso (Not Long.TryParse(id, NumberStyles.None, CultureInfo.InvariantCulture, numero) OrElse numero <= 0 OrElse numero.ToString(CultureInfo.InvariantCulture) <> id) Then Throw SolicitudInvalida()
        Next
        If original.Estado = "CONFIRMADO" AndAlso (original.DocumentoId Is Nothing OrElse original.InventarioId Is Nothing) Then Throw SolicitudInvalida()
        Dim normal = PerfilImportacionJson.Leer(Of ResultadoEfectoImportacion)(PerfilImportacionJson.Escribir(original))
        normal.Codigo = If(normal.Estado = "CONFIRMADO", "DOCUMENTO_CONFIRMADO", "RESULTADO_INCIERTO")
        normal.Mensaje = If(normal.Estado = "CONFIRMADO", "Documento, gabinete e incorporacion comprobados en el expediente confirmado.", "El almacenamiento requiere comprobacion antes de cualquier repeticion.")
        Return normal
    End Function

    Friend Shared Function PersistirResultadoDocumento(comando As ComandoAlmacenamientoCarpetas, lote As LoteImportacionCarpetas, raiz As RaizImportacionCarpetas, archivo As ArchivoImportacionCarpetas, intento As IntentoImportacionCarpetas, resultado As ResultadoEfectoImportacion, instante As DateTime, ejecutar As Func(Of String, Object(), Integer), evento As Action) As ResultadoEfectoImportacion
        Dim normal = NormalizarResultadoDocumento(comando, resultado), original = comando.Intento
        If lote Is Nothing OrElse raiz Is Nothing OrElse archivo Is Nothing OrElse intento Is Nothing OrElse ejecutar Is Nothing OrElse evento Is Nothing OrElse intento.IntentoId <> original.IntentoId OrElse intento.LoteId <> original.LoteId OrElse intento.RaizId <> original.RaizId OrElse intento.ArchivoId <> original.ArchivoId OrElse intento.Fase <> "ALMACENAR" OrElse intento.TokenReclamacion <> original.TokenReclamacion OrElse intento.SolicitudHash <> original.SolicitudHash OrElse lote.LoteId <> original.LoteId OrElse raiz.RaizId <> original.RaizId OrElse raiz.LoteId <> lote.LoteId OrElse archivo.ArchivoId <> original.ArchivoId OrElse archivo.RaizId <> raiz.RaizId Then Throw ConflictoOperacion()
        If PerfilImportacionJson.Escribir(lote.Contexto) <> PerfilImportacionJson.Escribir(comando.Contexto) OrElse PerfilImportacionJson.Escribir(lote.Snapshot) <> PerfilImportacionJson.Escribir(comando.Snapshot) OrElse PerfilImportacionJson.Escribir(raiz.IdentidadExpediente) <> PerfilImportacionJson.Escribir(comando.Raiz.IdentidadExpediente) Then Throw ConflictoOperacion()
        If {"CONFIRMADO", "INCIERTO"}.Contains(intento.Estado) Then
            If intento.Estado <> normal.Estado OrElse PerfilImportacionJson.Escribir(intento.Resultado) <> PerfilImportacionJson.Escribir(normal) OrElse PerfilImportacionJson.Escribir(archivo.Resultado) <> PerfilImportacionJson.Escribir(normal) OrElse archivo.EstadoEfecto <> If(normal.Estado = "CONFIRMADO", "COMPLETADO", "INCIERTO") OrElse (normal.Estado = "CONFIRMADO" AndAlso (intento.FinUtc Is Nothing OrElse archivo.IntentoActivoId IsNot Nothing)) OrElse (normal.Estado = "INCIERTO" AndAlso (intento.FinUtc IsNot Nothing OrElse archivo.IntentoActivoId <> intento.IntentoId)) Then Throw ConflictoOperacion()
            Return normal
        End If
        'El checkpoint conserva una señal de detencion posterior al inicio.
        If lote.Estado <> "EN_CURSO" OrElse lote.Revision = 0 OrElse lote.Revision >= UInteger.MaxValue - 1UI OrElse intento.Estado <> "EN_CURSO" OrElse intento.FinUtc IsNot Nothing OrElse PerfilImportacionJson.Escribir(intento.Resultado) <> PerfilImportacionJson.Escribir(EvidenciaInicialDocumento(comando)) OrElse PerfilImportacionJson.Escribir(archivo) <> PerfilImportacionJson.Escribir(comando.Archivo) OrElse PerfilImportacionJson.Escribir(raiz) <> PerfilImportacionJson.Escribir(comando.Raiz) Then Throw ConflictoOperacion()
        Dim estadoArchivo = If(normal.Estado = "CONFIRMADO", "COMPLETADO", "INCIERTO")
        CoordinadorImportacionCarpetas.ValidarTransicion("EFECTO", archivo.EstadoEfecto, estadoArchivo)
        CoordinadorImportacionCarpetas.ValidarTransicion("INTENTO", intento.Estado, normal.Estado)
        Dim estadoLote = If(normal.Estado = "CONFIRMADO", lote.Estado, "PENDIENTE_VERIFICACION")
        UnaFila(ejecutar("UPDATE ra_integracion_intento SET estado=@estado,evidencia_doc=@evidencia,fin_utc=@fin,ultimo_contacto_utc=@ahora WHERE intento_id=@intento AND lote_id=@lote AND raiz_id=@raiz AND archivo_id=@archivo AND fase='ALMACENAR' AND estado='EN_CURSO' AND token_reclamacion=@token AND solicitud_hash=@hash AND fin_utc IS NULL", New Object() {"@estado", normal.Estado, "@evidencia", PerfilImportacionJson.Escribir(normal), "@fin", If(normal.Estado = "CONFIRMADO", DirectCast(instante, Object), DBNull.Value), "@ahora", instante, "@intento", IdOperacion(intento.IntentoId), "@lote", IdOperacion(lote.LoteId), "@raiz", IdOperacion(raiz.RaizId), "@archivo", IdOperacion(archivo.ArchivoId), "@token", intento.TokenReclamacion, "@hash", BytesHuella(intento.SolicitudHash)}))
        UnaFila(ejecutar("UPDATE ra_integracion_archivo SET estado_efecto=@estado,resultado_doc=@resultado,intento_activo_id=@activo,revision=revision+1,actualizado_utc=@ahora WHERE archivo_id=@archivo AND raiz_id=@raiz AND revision=@revision AND estado_efecto='EN_CURSO' AND intento_activo_id=@intento", New Object() {"@estado", estadoArchivo, "@resultado", PerfilImportacionJson.Escribir(normal), "@activo", If(normal.Estado = "CONFIRMADO", DirectCast(DBNull.Value, Object), IdOperacion(intento.IntentoId)), "@ahora", instante, "@archivo", IdOperacion(archivo.ArchivoId), "@raiz", IdOperacion(raiz.RaizId), "@revision", archivo.Revision, "@intento", IdOperacion(intento.IntentoId)}))
        UnaFila(ejecutar("UPDATE ra_integracion_lote SET estado=@estado,revision=revision+1,actualizado_utc=@ahora WHERE lote_id=@lote AND revision=@revision", New Object() {"@estado", estadoLote, "@ahora", instante, "@lote", IdOperacion(lote.LoteId), "@revision", lote.Revision}))
        evento()
        Return normal
    End Function
End Class
