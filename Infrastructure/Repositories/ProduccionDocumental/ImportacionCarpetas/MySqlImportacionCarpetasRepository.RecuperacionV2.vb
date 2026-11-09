Option Strict On
Option Compare Binary
Imports System
Imports System.Data
Imports System.Globalization
Imports System.Linq
Imports System.Security.Cryptography
Imports System.Text
Imports MySql.Data.MySqlClient

Public Partial Class MySqlImportacionCarpetasRepository
    Private Const RecuperarV2 As String = "RECUPERAR_DOCUMENTO_V2"

    Friend Function CrearRecuperacionDocumentoV2(staging As Func(Of LoteImportacionCarpetas, ArchivoImportacionCarpetas, IAlmacenStagingImportacion), catalogos As PerfilImportacionCatalogAdapter, autorizar As Action(Of ContextoImportacionCarpetas, ConfiguracionPerfilImportacion)) As RecuperacionDocumentoV2Importacion
        If staging Is Nothing OrElse catalogos Is Nothing OrElse autorizar Is Nothing Then Throw New ArgumentNullException()
        Return New RecuperacionDocumentoV2Importacion(
            Function(contexto, lote, intento)
                AutorizarOperacion(contexto) : IdOperacion(lote) : IdOperacion(intento)
                Return Operar(Function(conexion, tx) CargarRecuperacionDocumentoV2(conexion, tx, contexto, lote, intento))
            End Function,
            Function(vista, documento)
                Dim proveedor = TryCast(staging(vista.Operacion.Lote, vista.Operacion.Archivo), IAlmacenStagingV2Importacion)
                If proveedor Is Nothing Then Throw NoDisponible()
                Return proveedor.AbrirGuardiaV2(vista.Operacion.Lote.Contexto, documento)
            End Function,
            Function(comando) catalogos.ComprobarAusenciaRecuperacionV2(comando, catalogos.PrepararDatosAlmacenamiento(comando)),
            Function(comando, resultado) catalogos.ComprobarAlmacenamiento(comando, catalogos.PrepararDatosAlmacenamiento(comando), resultado),
            Function(vista, resultado, guardia) GuardarRecuperacionDocumentoV2(vista, resultado, guardia, autorizar))
    End Function

    Private Shared Function CargarRecuperacionDocumentoV2(conexion As MySqlConnection, tx As MySqlTransaction, contexto As ContextoImportacionCarpetas, loteId As String, intentoId As String) As VistaRecuperacionDocumentoV2Importacion
        'La lectura inicial ya retiene el lote; el intento se bloquea después
        'de raíz/archivo mediante el cargador común, sin invertir los locks.
        Dim base = CargarVistaReconciliacion(conexion, tx, contexto, loteId, intentoId)
        If base.Archivo Is Nothing OrElse base.Intento.Fase <> "ALMACENAR" Then Throw ConflictoOperacion()
        Return New VistaRecuperacionDocumentoV2Importacion With {.Plan = base.Plan, .Operacion = CargarPersistenciaDocumentoV2(conexion, tx, contexto, loteId, base.Raiz.RaizId, base.Archivo.ArchivoId, intentoId)}
    End Function

    Private Function GuardarRecuperacionDocumentoV2(esperada As VistaRecuperacionDocumentoV2Importacion, resultado As ResultadoEfectoImportacion, guardia As IGuardiaOriginalV2Importacion, autorizar As Action(Of ContextoImportacionCarpetas, ConfiguracionPerfilImportacion)) As ResultadoEfectoImportacion
        If guardia Is Nothing OrElse autorizar Is Nothing Then Throw SolicitudInvalida()
        Dim copia = CoordinadorImportacionCarpetas.CopiarVistaRecuperacionV2(esperada)
        Dim contexto = copia.Operacion.Lote.Contexto
        AutorizarOperacion(contexto)
        Dim salida As ResultadoEfectoImportacion = Nothing
        guardia.ConservarDurante(Sub()
                                    salida = Operar(Function(conexion, tx)
                                                        Dim actual = CargarRecuperacionDocumentoV2(conexion, tx, contexto, copia.Operacion.Lote.LoteId, copia.Operacion.Intento.IntentoId)
                                                        autorizar(actual.Operacion.Lote.Contexto, actual.Operacion.Lote.Snapshot.Configuracion)
                                                        If Not LeerClasificacionExpedienteConocido(contexto, actual.Operacion.Raiz.IdentidadExpediente, actual.Operacion.Lote.Snapshot.Configuracion, Function(sql, parametros) LeerTablaConsulta(conexion, tx, sql, parametros)) Then Throw AccesoDenegado()
                                                        Return PersistirRecuperacionDocumentoV2(copia, actual, resultado, guardia, Ahora(),
                                                            Function(sql, parametros)
                                                                Using cmd = ComandoOperacion(conexion, tx, sql, parametros)
                                                                    Return cmd.ExecuteNonQuery()
                                                                End Using
                                                            End Function,
                                                            Sub() ConsolidarEnTransaccion(conexion, tx, contexto, actual.Operacion.Lote.LoteId, Ahora()))
                                                    End Function)
                                End Sub)
        Return salida
    End Function

    Friend Shared Function ClasificarRecuperacionV2(vista As VistaRecuperacionDocumentoV2Importacion) As DocumentoAlmacenamientoV2Importacion
        Dim o = vista.Operacion, i = o.Intento
        CoordinadorImportacionCarpetas.ValidarVistaRecuperacionV2(vista, o.Lote.Contexto, o.Lote.LoteId, i.IntentoId)
        If i.Estado <> "EN_CURSO" OrElse i.Fase <> "ALMACENAR" OrElse i.EjecutorRef <> "WEB_DOCUMENTO" OrElse i.FinUtc IsNot Nothing OrElse i.EsNuevaReserva OrElse o.Lote.Estado <> "EN_CURSO" OrElse o.Archivo.IntentoActivoId <> i.IntentoId OrElse o.Archivo.EstadoEfecto <> "EN_CURSO" OrElse o.Archivo.Resultado IsNot Nothing Then Throw ConflictoOperacion()
        Dim documento = DocumentosAlmacenamientoV2Importacion.Leer(o.Evidencia)
        If o.Evidencia <> DocumentosAlmacenamientoV2Importacion.Escribir(documento) Then Throw ConflictoOperacion()
        VincularRecuperacionV2(vista, documento)
        ComandoAlmacenamientoCarpetas.PrepararRecuperacionV2(o.Lote.Contexto, o.Raiz, o.Archivo, o.Lote.Snapshot, i, documento)
        Dim cantidades = ValidarDiarioDocumentoV2(o, documento, Nothing)
        If cantidades(1) <> 1 OrElse cantidades(4) <> 0 Then Throw ConflictoOperacion()
        If documento.Fase = "PREPARADO" Then
            If o.Lote.DetencionSolicitada OrElse cantidades(2) <> 0 OrElse cantidades(3) <> 0 Then Throw ConflictoOperacion()
        ElseIf documento.Fase = "RETORNO_OBSERVADO" Then
            If cantidades(2) <> 1 OrElse cantidades(3) <> 1 OrElse Not ValidadorPerfilImportacion.ReferenciaValida(documento.Retorno.DocumentoId) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(documento.Retorno.InventarioId) OrElse documento.Autorizacion.RevisionLote >= o.Lote.Revision Then Throw ConflictoOperacion()
        Else
            'Gate sin retorno fiable nunca prueba ausencia, aunque no haya filas.
            Throw ConflictoOperacion()
        End If
        Return documento
    End Function

    Private Shared Sub VincularRecuperacionV2(vista As VistaRecuperacionDocumentoV2Importacion, documento As DocumentoAlmacenamientoV2Importacion)
        Dim o = vista.Operacion, inicio = documento.Inicio, contexto = o.Lote.Contexto, a = o.Archivo, r = o.Raiz, i = o.Intento
        If inicio.IntentoId <> i.IntentoId OrElse inicio.LoteId <> o.Lote.LoteId OrElse inicio.RaizId <> r.RaizId OrElse inicio.ArchivoId <> a.ArchivoId OrElse inicio.UsuarioRef <> contexto.UsuarioRef OrElse inicio.AmbitoRef <> contexto.AmbitoRef OrElse inicio.NivelRef <> contexto.NivelRef OrElse inicio.SolicitudHash <> i.SolicitudHash Then Throw ConflictoOperacion()
        If r.IdentidadExpediente Is Nothing OrElse inicio.ExpedienteId <> r.IdentidadExpediente.ExpedienteId OrElse inicio.RelacionNivelId <> r.IdentidadExpediente.RelacionNivelId OrElse inicio.OrigenGabinete <> o.Lote.Snapshot.Configuracion.OrigenGabinete OrElse inicio.GabineteId <> o.Lote.Snapshot.Configuracion.GabineteId.Value.ToString(CultureInfo.InvariantCulture) Then Throw ConflictoOperacion()
        If inicio.ContextoHash <> HuellaContenidoRecuperacionV2(New With {contexto.UsuarioRef, contexto.AmbitoRef, contexto.NivelRef, contexto.RutaCanonica, contexto.SelloDestino}) OrElse inicio.SnapshotHash <> o.Lote.Snapshot.Huella OrElse inicio.RecepcionHash <> HuellaContenidoRecuperacionV2(New With {a.StagingRef, a.BytesRecibidos, a.ContenidoHash}) OrElse inicio.ValoresHash <> HuellaContenidoRecuperacionV2(a.ValoresEfectivos) Then Throw ConflictoOperacion()
        If inicio.RevisionRaiz <> r.Revision OrElse inicio.RevisionArchivo <> a.Revision OrElse inicio.RevisionLote >= o.Lote.Revision OrElse inicio.Original.ContenidoHash <> a.ContenidoHash OrElse inicio.Original.Bytes <> a.BytesRecibidos Then Throw ConflictoOperacion()
    End Sub
    Private Shared Function HuellaContenidoRecuperacionV2(valor As Object) As String
        Using sha = SHA256.Create()
            Return BitConverter.ToString(sha.ComputeHash(Encoding.ASCII.GetBytes(PerfilImportacionJson.Escribir(valor)))).Replace("-", "").ToLowerInvariant()
        End Using
    End Function

    Friend Shared Function LeerHistorialRecuperacionV2(vista As VistaRecuperacionDocumentoV2Importacion) As ResultadoEfectoImportacion
        Dim o = vista.Operacion, i = o.Intento
        If i.Estado <> "RECHAZADO" AndAlso i.Estado <> "CONFIRMADO" Then Return Nothing
        Dim recuperaciones = o.Diario.Rows.Cast(Of DataRow)().Where(Function(r) Texto(r, "accion") = RecuperarV2).ToArray()
        Dim checkpoints = o.Diario.Rows.Cast(Of DataRow)().Where(Function(r) Texto(r, "accion") = ResultadoV2).ToArray()
        If recuperaciones.Length = 0 AndAlso checkpoints.Length = 0 Then Return Nothing
        If recuperaciones.Length + checkpoints.Length <> 1 OrElse i.FinUtc Is Nothing OrElse i.Resultado Is Nothing OrElse o.Evidencia <> PerfilImportacionJson.Escribir(i.Resultado) OrElse i.EjecutorRef <> "WEB_DOCUMENTO" Then Throw ConflictoOperacion()
        Dim fila = If(recuperaciones.Length = 1, recuperaciones(0), checkpoints(0))
        If o.Diario.Rows(o.Diario.Rows.Count - 1) IsNot fila Then Throw ConflictoOperacion()
        If Texto(fila, "detalle_doc") <> o.Evidencia OrElse Texto(fila, "codigo") <> i.Resultado.Codigo OrElse Texto(fila, "lote_id") <> o.Lote.LoteId OrElse Texto(fila, "intento_id") <> i.IntentoId OrElse Texto(fila, "recurso_id") <> o.Archivo.ArchivoId OrElse Texto(fila, "recurso_tipo") <> "ARCHIVO" OrElse Texto(fila, "actor_ref") <> o.Lote.Contexto.UsuarioRef Then Throw ConflictoOperacion()
        Dim pruebaFila = o.Diario.Rows.Cast(Of DataRow)().Where(Function(r) Texto(r, "accion") = If(i.Estado = "RECHAZADO", InicioV2, RetornoV2)).ToArray()
        If pruebaFila.Length <> 1 Then Throw ConflictoOperacion()
        Dim documento = DocumentosAlmacenamientoV2Importacion.Leer(Texto(pruebaFila(0), "detalle_doc"))
        Dim temporal As New VistaPersistenciaDocumentoV2Importacion With {.Lote = o.Lote, .Raiz = o.Raiz, .Archivo = o.Archivo, .Intento = i, .Evidencia = o.Evidencia, .Diario = o.Diario.Copy()}
        If recuperaciones.Length = 1 Then
            For Each borrar In temporal.Diario.Rows.Cast(Of DataRow)().Where(Function(r) Texto(r, "accion") = RecuperarV2).ToArray()
                temporal.Diario.Rows.Remove(borrar)
            Next
        End If
        Dim cantidades = ValidarDiarioDocumentoV2(temporal, documento, If(checkpoints.Length = 1, i.Resultado, Nothing))
        Dim inicio = documento.Inicio, contexto = o.Lote.Contexto
        If inicio.IntentoId <> i.IntentoId OrElse inicio.LoteId <> o.Lote.LoteId OrElse inicio.RaizId <> o.Raiz.RaizId OrElse inicio.ArchivoId <> o.Archivo.ArchivoId OrElse inicio.SolicitudHash <> i.SolicitudHash OrElse inicio.UsuarioRef <> contexto.UsuarioRef OrElse inicio.AmbitoRef <> contexto.AmbitoRef OrElse inicio.NivelRef <> contexto.NivelRef OrElse inicio.SnapshotHash <> o.Lote.Snapshot.Huella OrElse inicio.ContextoHash <> HuellaContenidoRecuperacionV2(New With {contexto.UsuarioRef, contexto.AmbitoRef, contexto.NivelRef, contexto.RutaCanonica, contexto.SelloDestino}) Then Throw ConflictoOperacion()
        If i.Estado = "RECHAZADO" Then
            If cantidades(1) <> 1 OrElse cantidades(2) <> 0 OrElse cantidades(3) <> 0 OrElse cantidades(4) <> 0 OrElse i.Resultado.Estado <> "RECHAZADO" OrElse i.Resultado.Codigo <> "EJECUCION_CERRADA_SIN_INVOCACION" OrElse i.Resultado.DocumentoId IsNot Nothing OrElse i.Resultado.InventarioId IsNot Nothing Then Throw ConflictoOperacion()
        ElseIf cantidades(1) <> 1 OrElse cantidades(2) <> 1 OrElse cantidades(3) <> 1 OrElse documento.Fase <> "RETORNO_OBSERVADO" OrElse i.Resultado.Estado <> "CONFIRMADO" OrElse i.Resultado.Codigo <> "DOCUMENTO_CONFIRMADO" OrElse Not ValidadorPerfilImportacion.ReferenciaValida(i.Resultado.DocumentoId) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(i.Resultado.InventarioId) OrElse i.Resultado.DocumentoId <> documento.Retorno.DocumentoId OrElse i.Resultado.InventarioId <> documento.Retorno.InventarioId Then
            Throw ConflictoOperacion()
        End If
        If i.Resultado.Expediente Is Nothing OrElse PerfilImportacionJson.Escribir(i.Resultado.Expediente) <> PerfilImportacionJson.Escribir(o.Raiz.IdentidadExpediente) Then Throw ConflictoOperacion()
        Return PerfilImportacionJson.Leer(Of ResultadoEfectoImportacion)(o.Evidencia)
    End Function

    Friend Shared Function PersistirRecuperacionDocumentoV2(esperada As VistaRecuperacionDocumentoV2Importacion, actual As VistaRecuperacionDocumentoV2Importacion, resultado As ResultadoEfectoImportacion, guardia As IGuardiaOriginalV2Importacion, instante As DateTime, ejecutar As Func(Of String, Object(), Integer), consolidar As Action) As ResultadoEfectoImportacion
        If guardia Is Nothing OrElse ejecutar Is Nothing OrElse consolidar Is Nothing Then Throw SolicitudInvalida()
        Dim historico = LeerHistorialRecuperacionV2(actual)
        If historico IsNot Nothing Then
            If PerfilImportacionJson.Escribir(historico) <> PerfilImportacionJson.Escribir(resultado) Then Throw ConflictoOperacion()
            Return historico
        End If
        If CoordinadorImportacionCarpetas.HuellaVistaRecuperacionV2(esperada) <> CoordinadorImportacionCarpetas.HuellaVistaRecuperacionV2(actual) Then Throw ConflictoOperacion()
        Dim documento = ClasificarRecuperacionV2(actual)
        guardia.Validar()
        If PerfilImportacionJson.Escribir(guardia.Original) <> PerfilImportacionJson.Escribir(documento.Inicio.Original) Then Throw ConflictoOperacion()
        Dim normal = CoordinadorImportacionCarpetas.ResultadoRecuperacionV2(actual, documento)
        If PerfilImportacionJson.Escribir(resultado) <> PerfilImportacionJson.Escribir(normal) Then Throw ConflictoOperacion()
        Dim o = actual.Operacion, i = o.Intento, a = o.Archivo, negativo = documento.Fase = "PREPARADO"
        If Not negativo Then
            CoordinadorImportacionCarpetas.ValidarTransicion("INTENTO", i.Estado, "CONFIRMADO")
            CoordinadorImportacionCarpetas.ValidarTransicion("EFECTO", a.EstadoEfecto, "COMPLETADO")
        End If
        Dim evidencia = PerfilImportacionJson.Escribir(normal)
        UnaFila(ejecutar("UPDATE ra_integracion_intento SET estado=@estado,evidencia_doc=@evidencia,fin_utc=@ahora,ultimo_contacto_utc=@ahora WHERE intento_id=@intento AND lote_id=@lote AND raiz_id=@raiz AND archivo_id=@archivo AND fase='ALMACENAR' AND estado='EN_CURSO' AND fin_utc IS NULL AND token_reclamacion=@token AND solicitud_hash=@hash AND evidencia_doc=@anterior", New Object() {"@estado", normal.Estado, "@evidencia", evidencia, "@ahora", instante, "@intento", IdOperacion(i.IntentoId), "@lote", IdOperacion(o.Lote.LoteId), "@raiz", IdOperacion(o.Raiz.RaizId), "@archivo", IdOperacion(a.ArchivoId), "@token", i.TokenReclamacion, "@hash", BytesHuella(i.SolicitudHash), "@anterior", o.Evidencia}))
        UnaFila(ejecutar("UPDATE ra_integracion_archivo SET estado_efecto=@estado,resultado_doc=@resultado,intento_activo_id=NULL,revision=revision+1,actualizado_utc=@ahora WHERE archivo_id=@archivo AND raiz_id=@raiz AND revision=@revision AND estado_efecto='EN_CURSO' AND intento_activo_id=@intento AND resultado_doc IS NULL", New Object() {"@estado", If(negativo, "NO_INICIADO", "COMPLETADO"), "@resultado", If(negativo, DirectCast(DBNull.Value, Object), evidencia), "@ahora", instante, "@archivo", IdOperacion(a.ArchivoId), "@raiz", IdOperacion(o.Raiz.RaizId), "@revision", a.Revision, "@intento", IdOperacion(i.IntentoId)}))
        UnaFila(ejecutar("UPDATE ra_integracion_lote SET revision=revision+1,actualizado_utc=@ahora WHERE lote_id=@lote AND revision=@revision AND estado='EN_CURSO'" & If(negativo, " AND detencion_solicitada=0", ""), New Object() {"@ahora", instante, "@lote", IdOperacion(o.Lote.LoteId), "@revision", o.Lote.Revision}))
        UnaFila(ejecutar("INSERT INTO ra_integracion_evento (lote_id,intento_id,recurso_tipo,recurso_id,accion,codigo,actor_ref,ocurrido_utc,detalle_doc) VALUES (@lote,@intento,'ARCHIVO',@archivo,'RECUPERAR_DOCUMENTO_V2',@codigo,@actor,@ahora,@detalle)", New Object() {"@lote", IdOperacion(o.Lote.LoteId), "@intento", IdOperacion(i.IntentoId), "@archivo", IdOperacion(a.ArchivoId), "@codigo", normal.Codigo, "@actor", o.Lote.Contexto.UsuarioRef, "@ahora", instante, "@detalle", evidencia}))
        guardia.Validar()
        consolidar()
        Return normal
    End Function
End Class
