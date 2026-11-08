Option Strict On
Option Compare Binary
Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization
Imports System.Linq
Imports MySql.Data.MySqlClient

Friend Class VistaPersistenciaDocumentoV2Importacion
    Public Property Lote As LoteImportacionCarpetas
    Public Property Raiz As RaizImportacionCarpetas
    Public Property Archivo As ArchivoImportacionCarpetas
    Public Property Intento As IntentoImportacionCarpetas
    Public Property Evidencia As String
    Public Property Diario As DataTable
End Class

Public Partial Class MySqlImportacionCarpetasRepository
    Private Const InicioV2 As String = "INICIAR_ALMACENAMIENTO_V2"
    Private Const GateV2 As String = "AUTORIZAR_INVOCACION_DOCUMENTAL"
    Private Const RetornoV2 As String = "OBSERVAR_RETORNO_DOCUMENTAL"
    Private Const ResultadoV2 As String = "RESULTADO_ALMACENAMIENTO_V2"

    Friend Function CrearPersistenciaDocumentoV2() As PersistenciaDocumentoV2Importacion
        Return New PersistenciaDocumentoV2Importacion(
            Function(c, d, documento)
                c.ValidarDatos(d)
                Dim texto = EjecutarPasoDocumentoV2(c, documento, "INICIO", Nothing)
                'Operar ya confirmó antes de devolver; nunca emitir dentro del tx.
                Return New AcuseDocumentoV2Importacion(DocumentosAlmacenamientoV2Importacion.Leer(texto))
            End Function,
            Function(c, documento)
                Dim texto = EjecutarPasoDocumentoV2(c, documento, "AUTORIZAR", Nothing)
                Return New AcuseDocumentoV2Importacion(DocumentosAlmacenamientoV2Importacion.Leer(texto))
            End Function,
            Function(c, documento)
                Dim texto = EjecutarPasoDocumentoV2(c, documento, "OBSERVAR", Nothing)
                Return New AcuseDocumentoV2Importacion(DocumentosAlmacenamientoV2Importacion.Leer(texto))
            End Function,
            Function(c, documento, resultado) PerfilImportacionJson.Leer(Of ResultadoEfectoImportacion)(EjecutarPasoDocumentoV2(c, documento, "RESULTADO", resultado)))
    End Function

    Private Function EjecutarPasoDocumentoV2(comando As ComandoAlmacenamientoCarpetas, documento As DocumentoAlmacenamientoV2Importacion, paso As String, resultado As ResultadoEfectoImportacion) As String
        If comando Is Nothing OrElse comando.EsVerificacion Then Throw SolicitudInvalida()
        DocumentosAlmacenamientoV2Importacion.Escribir(documento)
        AutorizarOperacion(comando.Contexto)
        Return Operar(Function(conexion, tx)
                          Dim vista = CargarPersistenciaDocumentoV2(conexion, tx, comando)
                          If paso = "INICIO" OrElse paso = "AUTORIZAR" Then
                              Dim plan = CargarPlan(conexion, tx, comando.Contexto, IdOperacion(vista.Lote.PlanId), False)
                              Dim barrera = LeerVistaBarreraCreacion(vista.Lote, plan, vista.Raiz, Function(sql, parametros) LeerTablaConsulta(conexion, tx, sql, parametros))
                              If Not CoordinadorImportacionCarpetas.EvaluarBarreraAlmacenamiento(barrera, vista.Archivo, comando.Intento.IntentoId) Then Throw ConflictoOperacion()
                          End If
                          If Not LeerClasificacionExpedienteConocido(comando.Contexto, vista.Raiz.IdentidadExpediente, vista.Lote.Snapshot.Configuracion, Function(sql, parametros) LeerTablaConsulta(conexion, tx, sql, parametros)) Then Throw AccesoDenegado()
                          Return PersistirPasoDocumentoV2(comando, vista, documento, resultado, paso, Ahora(),
                              Function(sql, parametros)
                                  Using cmd = ComandoOperacion(conexion, tx, sql, parametros)
                                      Return cmd.ExecuteNonQuery()
                                  End Using
                              End Function,
                              Sub() ConsolidarEnTransaccion(conexion, tx, comando.Contexto, vista.Lote.LoteId, Ahora()))
                      End Function)
    End Function

    Private Shared Function CargarPersistenciaDocumentoV2(conexion As MySqlConnection, tx As MySqlTransaction, comando As ComandoAlmacenamientoCarpetas) As VistaPersistenciaDocumentoV2Importacion
        Dim lote = BloquearLoteRecepcion(conexion, tx, comando.Contexto, comando.Intento.LoteId)
        Dim raiz = CargarRaizControl(conexion, tx, lote.LoteId, comando.Intento.RaizId)
        Dim archivo = CargarArchivoRecepcion(conexion, tx, comando.Contexto, lote.LoteId, comando.Intento.ArchivoId, True)
        Dim vista As New VistaPersistenciaDocumentoV2Importacion With {.Lote = lote, .Raiz = raiz, .Archivo = archivo}
        Using cmd = ComandoOperacion(conexion, tx, "SELECT * FROM ra_integracion_intento WHERE intento_id=@intento AND lote_id=@lote AND raiz_id=@raiz AND archivo_id=@archivo AND fase='ALMACENAR' FOR UPDATE", "@intento", IdOperacion(comando.Intento.IntentoId), "@lote", IdOperacion(lote.LoteId), "@raiz", IdOperacion(raiz.RaizId), "@archivo", IdOperacion(archivo.ArchivoId))
            Using reader = cmd.ExecuteReader()
                If Not reader.Read() Then Throw AccesoDenegado()
                vista.Intento = LeerIntentoDocumental(reader)
                If Not reader.IsDBNull(reader.GetOrdinal("evidencia_doc")) Then vista.Evidencia = CStr(reader("evidencia_doc"))
            End Using
        End Using
        vista.Diario = LeerTablaConsulta(conexion, tx, "SELECT accion,codigo,actor_ref,recurso_tipo,recurso_id,lote_id,intento_id,detalle_doc FROM ra_integracion_evento WHERE lote_id=@lote AND intento_id=@intento ORDER BY evento_id LIMIT 6 FOR UPDATE", New Object() {"@lote", IdOperacion(lote.LoteId), "@intento", IdOperacion(vista.Intento.IntentoId)})
        Return vista
    End Function

    'Núcleo usado por la transacción real y por el harness SQL local.
    Friend Shared Function PersistirPasoDocumentoV2(comando As ComandoAlmacenamientoCarpetas, vista As VistaPersistenciaDocumentoV2Importacion, documento As DocumentoAlmacenamientoV2Importacion, resultado As ResultadoEfectoImportacion, paso As String, instante As DateTime, ejecutar As Func(Of String, Object(), Integer), consolidar As Action) As String
        If comando Is Nothing OrElse comando.EsVerificacion OrElse vista Is Nothing OrElse vista.Lote Is Nothing OrElse vista.Raiz Is Nothing OrElse vista.Archivo Is Nothing OrElse vista.Intento Is Nothing OrElse vista.Diario Is Nothing OrElse ejecutar Is Nothing OrElse consolidar Is Nothing Then Throw SolicitudInvalida()
        Dim nuevo = DocumentosAlmacenamientoV2Importacion.Leer(DocumentosAlmacenamientoV2Importacion.Escribir(documento))
        Dim l = vista.Lote, r = vista.Raiz, a = vista.Archivo, i = vista.Intento, original = comando.Intento
        AutorizarOperacion(comando.Contexto)
        If i.IntentoId <> original.IntentoId OrElse i.LoteId <> original.LoteId OrElse i.RaizId <> original.RaizId OrElse i.ArchivoId <> original.ArchivoId OrElse i.Fase <> "ALMACENAR" OrElse i.EjecutorRef <> "WEB_DOCUMENTO" OrElse i.TokenReclamacion <> original.TokenReclamacion OrElse i.SolicitudHash <> original.SolicitudHash OrElse l.LoteId <> i.LoteId OrElse r.LoteId <> l.LoteId OrElse r.RaizId <> i.RaizId OrElse a.RaizId <> r.RaizId OrElse a.ArchivoId <> i.ArchivoId Then Throw ConflictoOperacion()
        If PerfilImportacionJson.Escribir(l.Contexto) <> PerfilImportacionJson.Escribir(comando.Contexto) OrElse PerfilImportacionJson.Escribir(l.Snapshot) <> PerfilImportacionJson.Escribir(comando.Snapshot) OrElse PerfilImportacionJson.Escribir(r.IdentidadExpediente) <> PerfilImportacionJson.Escribir(comando.Raiz.IdentidadExpediente) Then Throw ConflictoOperacion()
        Dim preparado As New DocumentoAlmacenamientoV2Importacion With {.Version = 2, .Origen = "SERVIDOR", .Fase = "PREPARADO", .Inicio = nuevo.Inicio}
        Dim esperado = DocumentosAlmacenamientoV2Importacion.Preparar(comando, nuevo.Inicio.Original, nuevo.Inicio.RevisionLote)
        If DocumentosAlmacenamientoV2Importacion.Escribir(preparado) <> DocumentosAlmacenamientoV2Importacion.Escribir(esperado) OrElse nuevo.Inicio.SolicitudHash <> i.SolicitudHash Then Throw ConflictoOperacion()
        Dim normal As ResultadoEfectoImportacion = Nothing
        If paso = "RESULTADO" Then
            If nuevo.Fase <> "RETORNO_OBSERVADO" Then Throw ConflictoOperacion()
            normal = NormalizarResultadoDocumento(comando, resultado)
            If normal.DocumentoId <> nuevo.Retorno.DocumentoId OrElse normal.InventarioId <> nuevo.Retorno.InventarioId Then Throw ConflictoOperacion()
        ElseIf resultado IsNot Nothing Then
            Throw SolicitudInvalida()
        End If
        Dim terminal = paso = "RESULTADO" AndAlso (i.Estado = "CONFIRMADO" OrElse i.Estado = "INCIERTO")
        Dim cantidades = ValidarDiarioDocumentoV2(vista, nuevo, If(terminal, normal, Nothing))
        Dim inicioJson = DocumentosAlmacenamientoV2Importacion.Escribir(preparado)
        If terminal Then
            If a.Resultado Is Nothing OrElse i.Resultado Is Nothing OrElse PerfilImportacionJson.Escribir(i.Resultado) <> PerfilImportacionJson.Escribir(normal) Then Throw ConflictoOperacion()
            If cantidades(1) <> 1 OrElse cantidades(2) <> 1 OrElse cantidades(3) <> 1 OrElse cantidades(4) <> 1 OrElse vista.Evidencia <> PerfilImportacionJson.Escribir(normal) OrElse PerfilImportacionJson.Escribir(a.Resultado) <> PerfilImportacionJson.Escribir(normal) OrElse i.Estado <> normal.Estado OrElse a.EstadoEfecto <> If(normal.Estado = "CONFIRMADO", "COMPLETADO", "INCIERTO") OrElse (normal.Estado = "CONFIRMADO" AndAlso (i.FinUtc Is Nothing OrElse a.IntentoActivoId IsNot Nothing)) OrElse (normal.Estado = "INCIERTO" AndAlso (i.FinUtc IsNot Nothing OrElse a.IntentoActivoId <> i.IntentoId)) Then Throw ConflictoOperacion()
            Return PerfilImportacionJson.Escribir(normal)
        End If
        If l.Estado <> "EN_CURSO" OrElse l.Revision = 0 OrElse l.Revision >= UInteger.MaxValue - 1UI OrElse i.FinUtc IsNot Nothing OrElse cantidades(4) <> 0 OrElse PerfilImportacionJson.Escribir(r) <> PerfilImportacionJson.Escribir(comando.Raiz) OrElse PerfilImportacionJson.Escribir(a) <> PerfilImportacionJson.Escribir(comando.Archivo) Then Throw ConflictoOperacion()
        Dim accion As String, codigo As String, contenido As String
        Select Case paso
            Case "INICIO"
                If l.DetencionSolicitada OrElse i.Estado <> "RESERVADO" OrElse vista.Evidencia IsNot Nothing OrElse nuevo.Fase <> "PREPARADO" OrElse nuevo.Inicio.RevisionLote <> l.Revision OrElse cantidades(1) <> 0 OrElse cantidades(2) <> 0 OrElse cantidades(3) <> 0 Then Throw ConflictoOperacion()
                accion = InicioV2 : codigo = "ALMACENAMIENTO_PREPARADO_V2"
            Case "AUTORIZAR"
                'Nunca devolver un gate anterior como un permiso nuevo.
                If l.DetencionSolicitada OrElse i.Estado <> "EN_CURSO" OrElse nuevo.Fase <> "PREPARADO" OrElse vista.Evidencia <> inicioJson OrElse cantidades(1) <> 1 OrElse cantidades(2) <> 0 OrElse cantidades(3) <> 0 Then Throw ConflictoOperacion()
                nuevo = DocumentosAlmacenamientoV2Importacion.ConAutorizacion(nuevo, Guid.NewGuid().ToString("N"), l.Revision)
                accion = GateV2 : codigo = "INVOCACION_DOCUMENTAL_AUTORIZADA"
            Case "OBSERVAR"
                If i.Estado <> "EN_CURSO" OrElse nuevo.Fase <> "RETORNO_OBSERVADO" OrElse cantidades(1) <> 1 OrElse cantidades(2) <> 1 OrElse cantidades(3) <> 0 Then Throw ConflictoOperacion()
                Dim anterior = PerfilImportacionJson.Leer(Of DocumentoAlmacenamientoV2Importacion)(DocumentosAlmacenamientoV2Importacion.Escribir(nuevo))
                anterior.Fase = "AUTORIZADO" : anterior.Retorno = Nothing
                If vista.Evidencia <> DocumentosAlmacenamientoV2Importacion.Escribir(anterior) Then Throw ConflictoOperacion()
                accion = RetornoV2 : codigo = "RETORNO_DOCUMENTAL_OBSERVADO"
            Case "RESULTADO"
                If i.Estado <> "EN_CURSO" OrElse vista.Evidencia <> DocumentosAlmacenamientoV2Importacion.Escribir(nuevo) OrElse cantidades(1) <> 1 OrElse cantidades(2) <> 1 OrElse cantidades(3) <> 1 Then Throw ConflictoOperacion()
                accion = ResultadoV2 : codigo = normal.Codigo
                CoordinadorImportacionCarpetas.ValidarTransicion("INTENTO", i.Estado, normal.Estado)
                CoordinadorImportacionCarpetas.ValidarTransicion("EFECTO", a.EstadoEfecto, If(normal.Estado = "CONFIRMADO", "COMPLETADO", "INCIERTO"))
            Case Else
                Throw SolicitudInvalida()
        End Select
        contenido = If(normal Is Nothing, DocumentosAlmacenamientoV2Importacion.Escribir(nuevo), PerfilImportacionJson.Escribir(normal))
        Dim estado = If(normal Is Nothing, "EN_CURSO", normal.Estado)
        UnaFila(ejecutar("UPDATE ra_integracion_intento SET estado=@estado,evidencia_doc=@evidencia,fin_utc=@fin,ultimo_contacto_utc=@ahora WHERE intento_id=@intento AND lote_id=@lote AND raiz_id=@raiz AND archivo_id=@archivo AND fase='ALMACENAR' AND estado=@anteriorEstado AND fin_utc IS NULL AND token_reclamacion=@token AND solicitud_hash=@hash AND evidencia_doc <=> @anterior", New Object() {"@estado", estado, "@evidencia", contenido, "@fin", If(estado = "CONFIRMADO", DirectCast(instante, Object), DBNull.Value), "@ahora", instante, "@intento", IdOperacion(i.IntentoId), "@lote", IdOperacion(l.LoteId), "@raiz", IdOperacion(r.RaizId), "@archivo", IdOperacion(a.ArchivoId), "@anteriorEstado", i.Estado, "@token", i.TokenReclamacion, "@hash", BytesHuella(i.SolicitudHash), "@anterior", If(vista.Evidencia, DirectCast(DBNull.Value, Object))}))
        If normal IsNot Nothing Then
            UnaFila(ejecutar("UPDATE ra_integracion_archivo SET estado_efecto=@estado,resultado_doc=@resultado,intento_activo_id=@activo,revision=revision+1,actualizado_utc=@ahora WHERE archivo_id=@archivo AND raiz_id=@raiz AND revision=@revision AND estado_efecto='EN_CURSO' AND intento_activo_id=@intento AND resultado_doc IS NULL", New Object() {"@estado", If(normal.Estado = "CONFIRMADO", "COMPLETADO", "INCIERTO"), "@resultado", contenido, "@activo", If(normal.Estado = "CONFIRMADO", DirectCast(DBNull.Value, Object), IdOperacion(i.IntentoId)), "@ahora", instante, "@archivo", IdOperacion(a.ArchivoId), "@raiz", IdOperacion(r.RaizId), "@revision", a.Revision, "@intento", IdOperacion(i.IntentoId)}))
        End If
        UnaFila(ejecutar("UPDATE ra_integracion_lote SET estado=@estado,revision=revision+1,actualizado_utc=@ahora WHERE lote_id=@lote AND revision=@revision AND estado='EN_CURSO'" & If(paso = "INICIO" OrElse paso = "AUTORIZAR", " AND detencion_solicitada=0", ""), New Object() {"@estado", If(normal IsNot Nothing AndAlso normal.Estado = "INCIERTO", "PENDIENTE_VERIFICACION", "EN_CURSO"), "@ahora", instante, "@lote", IdOperacion(l.LoteId), "@revision", l.Revision}))
        UnaFila(ejecutar("INSERT INTO ra_integracion_evento (lote_id,intento_id,recurso_tipo,recurso_id,accion,codigo,actor_ref,ocurrido_utc,detalle_doc) VALUES (@lote,@intento,'ARCHIVO',@archivo,@accion,@codigo,@actor,@ahora,@detalle)", New Object() {"@lote", IdOperacion(l.LoteId), "@intento", IdOperacion(i.IntentoId), "@archivo", IdOperacion(a.ArchivoId), "@accion", accion, "@codigo", codigo, "@actor", comando.Contexto.UsuarioRef, "@ahora", instante, "@detalle", contenido}))
        If normal IsNot Nothing Then consolidar()
        Return contenido
    End Function

    Private Shared Function ValidarDiarioDocumentoV2(vista As VistaPersistenciaDocumentoV2Importacion, documento As DocumentoAlmacenamientoV2Importacion, resultado As ResultadoEfectoImportacion) As Integer()
        Dim acciones = New String() {"RESERVAR_INTENTO", InicioV2, GateV2, RetornoV2, ResultadoV2}
        Dim cantidades(4) As Integer
        Dim ultimo As Integer = -1
        If vista.Diario.Rows.Count > 5 Then Throw ConflictoOperacion()
        For Each fila As DataRow In vista.Diario.Rows
            Dim accion = Texto(fila, "accion"), posicion = Array.IndexOf(acciones, accion)
            If posicion <= ultimo Then Throw ConflictoOperacion()
            ultimo = posicion
            If posicion < 0 OrElse Texto(fila, "lote_id") <> vista.Lote.LoteId OrElse Texto(fila, "intento_id") <> vista.Intento.IntentoId OrElse Texto(fila, "actor_ref") <> vista.Lote.Contexto.UsuarioRef OrElse Texto(fila, "recurso_tipo") <> "ARCHIVO" OrElse Texto(fila, "recurso_id") <> vista.Archivo.ArchivoId Then Throw ConflictoOperacion()
            cantidades(posicion) += 1
            If cantidades(posicion) <> 1 Then Throw ConflictoOperacion()
            If posicion = 0 Then
                If Texto(fila, "codigo") <> "INTENTO_RESERVADO" Then Throw ConflictoOperacion()
            ElseIf posicion = 4 Then
                If resultado Is Nothing OrElse Texto(fila, "codigo") <> resultado.Codigo OrElse Texto(fila, "detalle_doc") <> PerfilImportacionJson.Escribir(resultado) Then Throw ConflictoOperacion()
            Else
                Dim guardado = DocumentosAlmacenamientoV2Importacion.Leer(Texto(fila, "detalle_doc"))
                If Texto(fila, "detalle_doc") <> DocumentosAlmacenamientoV2Importacion.Escribir(guardado) OrElse PerfilImportacionJson.Escribir(guardado.Inicio) <> PerfilImportacionJson.Escribir(documento.Inicio) Then Throw ConflictoOperacion()
                Dim fases = New String() {"", "PREPARADO", "AUTORIZADO", "RETORNO_OBSERVADO"}
                Dim codigos = New String() {"", "ALMACENAMIENTO_PREPARADO_V2", "INVOCACION_DOCUMENTAL_AUTORIZADA", "RETORNO_DOCUMENTAL_OBSERVADO"}
                If guardado.Fase <> fases(posicion) OrElse Texto(fila, "codigo") <> codigos(posicion) Then Throw ConflictoOperacion()
                If posicion >= 2 AndAlso (documento.Autorizacion Is Nothing OrElse PerfilImportacionJson.Escribir(guardado.Autorizacion) <> PerfilImportacionJson.Escribir(documento.Autorizacion)) Then Throw ConflictoOperacion()
                If posicion = 3 AndAlso DocumentosAlmacenamientoV2Importacion.Escribir(guardado) <> DocumentosAlmacenamientoV2Importacion.Escribir(documento) Then Throw ConflictoOperacion()
            End If
        Next
        If cantidades(0) <> 1 Then Throw ConflictoOperacion()
        Return cantidades
    End Function

    Private Shared Function LeerEvidenciaDocumentalCompatible(texto As String, intento As IntentoImportacionCarpetas) As ResultadoEfectoImportacion
        'El discriminador solo elige parser; no concede elegibilidad ni permiso.
        Try
            If texto Is Nothing OrElse texto.Length > PerfilImportacionJson.MaximoCaracteres Then Throw NoDisponible()
            Dim token = Newtonsoft.Json.Linq.JObject.Parse(texto)
            If token.Property("Version", StringComparison.Ordinal) Is Nothing Then Return LeerPersistido(Of ResultadoEfectoImportacion)(texto)
            Dim documento = DocumentosAlmacenamientoV2Importacion.Leer(texto), inicio = documento.Inicio
            If intento.Fase <> "ALMACENAR" OrElse intento.Estado <> "EN_CURSO" OrElse intento.FinUtc IsNot Nothing OrElse inicio.IntentoId <> intento.IntentoId OrElse inicio.LoteId <> intento.LoteId OrElse inicio.RaizId <> intento.RaizId OrElse inicio.ArchivoId <> intento.ArchivoId OrElse inicio.SolicitudHash <> intento.SolicitudHash Then Throw NoDisponible()
            Return New ResultadoEfectoImportacion With {.Estado = "EN_CURSO", .Codigo = "ALMACENAMIENTO_PREPARADO_V2", .Mensaje = "Protocolo V2 activo; no acredita un resultado documental.", .Expediente = New IdentidadExpedienteImportacion With {.OrigenLogico = "RA", .ExpedienteId = inicio.ExpedienteId, .RelacionNivelId = inicio.RelacionNivelId, .NivelRef = inicio.NivelRef}, .OrigenGabinete = inicio.OrigenGabinete, .GabineteId = inicio.GabineteId, .DocumentoId = If(documento.Retorno Is Nothing, Nothing, documento.Retorno.DocumentoId), .InventarioId = If(documento.Retorno Is Nothing, Nothing, documento.Retorno.InventarioId)}
        Catch ex As Newtonsoft.Json.JsonException
            Throw NoDisponible()
        Catch ex As PerfilImportacionException
            Throw NoDisponible()
        End Try
    End Function
End Class
