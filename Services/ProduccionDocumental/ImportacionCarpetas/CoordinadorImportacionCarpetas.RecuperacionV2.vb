Option Strict On
Option Compare Binary
Imports System
Imports System.Data
Imports System.Globalization
Imports System.Linq
Imports System.Security.Cryptography
Imports System.Text

Public Partial Class CoordinadorImportacionCarpetas
    Private ReadOnly _recuperacionDocumentoV2 As RecuperacionDocumentoV2Importacion

    Friend Sub New(identidad As Func(Of ContextoImportacionCarpetas), repositorio As IControlLoteImportacionRepository, destino As Func(Of String, ContextoImportacionCarpetas), autorizar As Action(Of ContextoImportacionCarpetas, ConfiguracionPerfilImportacion), recuperacion As RecuperacionDocumentoV2Importacion)
        Me.New(identidad, repositorio, destino, autorizar)
        If recuperacion Is Nothing Then Throw New ArgumentNullException("recuperacion")
        _recuperacionDocumentoV2 = recuperacion
    End Sub

    Public Function RecuperarDocumento(solicitud As RecuperarDocumentoV2ImportacionSolicitud) As RecuperacionDocumentoImportacionResultado
        ValidarSolicitudRecuperacionV2(solicitud)
        Dim r = RecuperarDocumentoV2(solicitud.loteId, solicitud.intentoId, solicitud.revisionLoteEsperada, solicitud.revisionArchivoEsperada, _recuperacionDocumentoV2)
        Dim resultado = r.Resultado
        If Not r.Resuelta OrElse resultado Is Nothing OrElse resultado.Expediente Is Nothing Then Throw NoPermitida()
        Dim cierre = resultado.Estado = "RECHAZADO" AndAlso resultado.Codigo = "EJECUCION_CERRADA_SIN_INVOCACION" AndAlso resultado.DocumentoId Is Nothing AndAlso resultado.InventarioId Is Nothing
        Dim confirmado = resultado.Estado = "CONFIRMADO" AndAlso resultado.Codigo = "DOCUMENTO_CONFIRMADO" AndAlso ValidadorPerfilImportacion.ReferenciaValida(resultado.DocumentoId) AndAlso ValidadorPerfilImportacion.ReferenciaValida(resultado.InventarioId)
        If Not cierre AndAlso Not confirmado Then Throw NoPermitida()
        Return New RecuperacionDocumentoImportacionResultado With {.loteId = r.LoteId, .intentoId = r.IntentoId, .fase = r.Fase,
            .estado = resultado.Estado, .codigo = resultado.Codigo, .resuelta = True, .repeticion = r.Repeticion,
            .cierreSinInvocacion = cierre, .documentoConfirmado = confirmado, .expedienteId = resultado.Expediente.ExpedienteId,
            .relacionNivelId = resultado.Expediente.RelacionNivelId, .gabineteId = resultado.GabineteId,
            .documentoId = resultado.DocumentoId, .inventarioId = resultado.InventarioId}
    End Function

    Friend Shared Sub ValidarSolicitudRecuperacionV2(solicitud As RecuperarDocumentoV2ImportacionSolicitud)
        If solicitud Is Nothing OrElse Not ValidadorPerfilImportacion.ReferenciaValida(solicitud.loteId) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(solicitud.intentoId) OrElse solicitud.revisionLoteEsperada = 0 OrElse solicitud.revisionArchivoEsperada = 0 Then Throw EntradaOperacionInvalida()
    End Sub

    Friend Function RecuperarDocumentoV2(loteId As String, intentoId As String, revisionLote As UInteger, revisionArchivo As UInteger, puerto As RecuperacionDocumentoV2Importacion) As ResultadoReconciliacionImportacion
        If puerto Is Nothing Then Throw New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "La recuperación V2 no está compuesta.")
        If Not ValidadorPerfilImportacion.ReferenciaValida(loteId) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(intentoId) OrElse revisionLote = 0 OrElse revisionArchivo = 0 Then Throw EntradaOperacionInvalida()
        Dim identidad = _identidad()
        If identidad Is Nothing OrElse Not identidad.PuedeImportar Then Throw Denegado()
        Dim referencia = _repositorio.ObtenerContextoLote(identidad, loteId)
        If referencia Is Nothing OrElse referencia.UsuarioRef <> identidad.UsuarioRef OrElse referencia.AmbitoRef <> identidad.AmbitoRef Then Throw Denegado()
        Dim contexto = _destino(referencia.NivelRef)
        If Not MismoContextoControl(contexto, referencia) OrElse Not contexto.PuedeImportar Then Throw Denegado()
        Dim vista = CopiarVistaRecuperacionV2(puerto.Leer(contexto, loteId, intentoId))
        ValidarVistaRecuperacionV2(vista, contexto, loteId, intentoId)
        ReautorizarFase(contexto, vista.Operacion.Lote.Snapshot.Configuracion)
        Dim historico = MySqlImportacionCarpetasRepository.LeerHistorialRecuperacionV2(vista)
        If historico IsNot Nothing Then Return ProyectarRecuperacionV2(vista, historico, True)
        If vista.Operacion.Lote.Revision <> revisionLote OrElse vista.Operacion.Archivo.Revision <> revisionArchivo Then Throw NoPermitida()
        Dim documento = MySqlImportacionCarpetasRepository.ClasificarRecuperacionV2(vista)
        Dim operacion = vista.Operacion
        Dim comando = ComandoAlmacenamientoCarpetas.PrepararRecuperacionV2(contexto, operacion.Raiz, operacion.Archivo, operacion.Lote.Snapshot, operacion.Intento, documento)
        Dim resultado = ResultadoRecuperacionV2(vista, documento)
        Using guardia = puerto.Guardia(CopiarVistaRecuperacionV2(vista), documento)
            If guardia Is Nothing Then Throw NoPermitida()
            guardia.Validar()
            If PerfilImportacionJson.Escribir(guardia.Original) <> PerfilImportacionJson.Escribir(documento.Inicio.Original) Then Throw NoPermitida()
            ReautorizarFase(contexto, operacion.Lote.Snapshot.Configuracion)
            If documento.Fase = "PREPARADO" Then
                If Not puerto.Ausencia(comando) Then Throw NoPermitida()
            ElseIf Not puerto.Comprobar(comando, PerfilImportacionJson.Leer(Of ResultadoEfectoImportacion)(PerfilImportacionJson.Escribir(resultado))) Then
                Throw NoPermitida()
            End If
            guardia.Validar()
            ReautorizarFase(contexto, operacion.Lote.Snapshot.Configuracion)
            Dim guardado = puerto.Guardar(CopiarVistaRecuperacionV2(vista), resultado, guardia)
            If guardado Is Nothing OrElse PerfilImportacionJson.Escribir(guardado) <> PerfilImportacionJson.Escribir(resultado) Then Throw NoPermitida()
            Return ProyectarRecuperacionV2(vista, guardado, False)
        End Using
    End Function

    Friend Shared Sub ValidarVistaRecuperacionV2(vista As VistaRecuperacionDocumentoV2Importacion, contexto As ContextoImportacionCarpetas, loteId As String, intentoId As String)
        If vista Is Nothing OrElse vista.Operacion Is Nothing OrElse vista.Operacion.Intento Is Nothing OrElse vista.Operacion.Archivo Is Nothing Then Throw Denegado()
        Dim o = vista.Operacion
        ValidarVistaReconciliacion(New VistaReconciliacionImportacion With {.Lote = o.Lote, .Plan = vista.Plan, .Raiz = o.Raiz, .Archivo = o.Archivo, .Intento = o.Intento}, contexto, loteId, intentoId)
    End Sub
    Friend Shared Function CopiarVistaRecuperacionV2(vista As VistaRecuperacionDocumentoV2Importacion) As VistaRecuperacionDocumentoV2Importacion
        If vista Is Nothing OrElse vista.Operacion Is Nothing OrElse vista.Operacion.Diario Is Nothing Then Throw Denegado()
        Dim o = vista.Operacion
        Return New VistaRecuperacionDocumentoV2Importacion With {.Plan = PerfilImportacionJson.Leer(Of PlanImportacionCarpetas)(PerfilImportacionJson.Escribir(vista.Plan)), .Operacion = New VistaPersistenciaDocumentoV2Importacion With {
            .Lote = PerfilImportacionJson.Leer(Of LoteImportacionCarpetas)(PerfilImportacionJson.Escribir(o.Lote)), .Raiz = PerfilImportacionJson.Leer(Of RaizImportacionCarpetas)(PerfilImportacionJson.Escribir(o.Raiz)), .Archivo = PerfilImportacionJson.Leer(Of ArchivoImportacionCarpetas)(PerfilImportacionJson.Escribir(o.Archivo)), .Intento = PerfilImportacionJson.Leer(Of IntentoImportacionCarpetas)(PerfilImportacionJson.Escribir(o.Intento)), .Evidencia = o.Evidencia, .Diario = o.Diario.Copy()}}
    End Function
    Friend Shared Function HuellaVistaRecuperacionV2(vista As VistaRecuperacionDocumentoV2Importacion) As String
        Dim o = vista.Operacion
        Dim eventos = o.Diario.Rows.Cast(Of DataRow)().Select(Function(r) New With {
            .id = Convert.ToString(r("evento_id"), CultureInfo.InvariantCulture), .utc = Convert.ToDateTime(r("ocurrido_utc"), CultureInfo.InvariantCulture).ToString("o", CultureInfo.InvariantCulture),
            .accion = Convert.ToString(r("accion"), CultureInfo.InvariantCulture), .codigo = Convert.ToString(r("codigo"), CultureInfo.InvariantCulture), .actor = Convert.ToString(r("actor_ref"), CultureInfo.InvariantCulture), .tipo = Convert.ToString(r("recurso_tipo"), CultureInfo.InvariantCulture), .recurso = Convert.ToString(r("recurso_id"), CultureInfo.InvariantCulture), .lote = Convert.ToString(r("lote_id"), CultureInfo.InvariantCulture), .intento = Convert.ToString(r("intento_id"), CultureInfo.InvariantCulture), .detalle = Convert.ToString(r("detalle_doc"), CultureInfo.InvariantCulture)}).ToArray()
        'Cada documento conserva su límite individual; no duplicar un plan
        'grande en un JSON agregado que exceda el límite del serializador.
        Dim partes = New String() {PerfilImportacionJson.Escribir(o.Lote), PerfilImportacionJson.Escribir(o.Raiz), PerfilImportacionJson.Escribir(o.Archivo), PerfilImportacionJson.Escribir(o.Intento), If(o.Evidencia, "null"), PerfilImportacionJson.Escribir(vista.Plan), PerfilImportacionJson.Escribir(eventos)}
        Return HuellaTextoRecuperacionV2(String.Join(":", partes.Select(Function(p) HuellaTextoRecuperacionV2(p))))
    End Function
    Private Shared Function HuellaTextoRecuperacionV2(texto As String) As String
        Using sha = SHA256.Create()
            Return BitConverter.ToString(sha.ComputeHash(Encoding.ASCII.GetBytes(texto))).Replace("-", "").ToLowerInvariant()
        End Using
    End Function
    Friend Shared Function ResultadoRecuperacionV2(vista As VistaRecuperacionDocumentoV2Importacion, documento As DocumentoAlmacenamientoV2Importacion) As ResultadoEfectoImportacion
        Dim o = vista.Operacion, negativo = documento.Fase = "PREPARADO"
        Return New ResultadoEfectoImportacion With {.Estado = If(negativo, "RECHAZADO", "CONFIRMADO"), .Codigo = If(negativo, "EJECUCION_CERRADA_SIN_INVOCACION", "DOCUMENTO_CONFIRMADO"), .Mensaje = If(negativo, "Ejecución V2 cerrada sin invocación; no confirma un documento.", "Documento, gabinete e incorporacion comprobados en el expediente confirmado."), .Expediente = o.Raiz.IdentidadExpediente, .OrigenGabinete = o.Lote.Snapshot.Configuracion.OrigenGabinete, .GabineteId = o.Lote.Snapshot.Configuracion.GabineteId.Value.ToString(CultureInfo.InvariantCulture), .DocumentoId = If(negativo, Nothing, documento.Retorno.DocumentoId), .InventarioId = If(negativo, Nothing, documento.Retorno.InventarioId), .ValoresEfectivos = o.Archivo.ValoresEfectivos}
    End Function
    Private Shared Function ProyectarRecuperacionV2(vista As VistaRecuperacionDocumentoV2Importacion, resultado As ResultadoEfectoImportacion, repeticion As Boolean) As ResultadoReconciliacionImportacion
        Return New ResultadoReconciliacionImportacion With {.LoteId = vista.Operacion.Lote.LoteId, .IntentoId = vista.Operacion.Intento.IntentoId, .Fase = "ALMACENAR", .Resuelta = True, .Repeticion = repeticion, .Resultado = PerfilImportacionJson.Leer(Of ResultadoEfectoImportacion)(PerfilImportacionJson.Escribir(resultado))}
    End Function
End Class
