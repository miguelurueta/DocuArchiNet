Option Strict On
Option Compare Binary
Imports System
Imports System.Text.RegularExpressions

Public Partial Class CoordinadorImportacionCarpetas
    'Fachada acotada, sin bucles o seleccion de otro recurso/fase tras el resultado.
    Public Shared Function AvanzarLote(solicitud As AvanzarLoteImportacionSolicitud, componer As Func(Of String, CoordinadorImportacionCarpetas)) As AvanceLoteImportacionResultado
        If solicitud Is Nothing OrElse componer Is Nothing OrElse Not ValidadorPerfilImportacion.ReferenciaValida(solicitud.loteId) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(solicitud.recursoId) OrElse solicitud.revisionEsperada = 0 OrElse solicitud.revisionEsperada >= UInteger.MaxValue - 1UI OrElse Not Regex.IsMatch(If(solicitud.claveOperacion, ""), "\A[A-Za-z0-9_-]{1,64}\z", RegexOptions.CultureInvariant) Then Throw EntradaOperacionInvalida()
        Select Case solicitud.fase
            Case "CREAR_EXPEDIENTE", "MATERIALIZAR_VALORES", "ALMACENAR"
            Case Else : Throw EntradaOperacionInvalida()
        End Select
        Dim coordinador = componer(solicitud.fase)
        If coordinador Is Nothing Then Throw New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "La fase no esta disponible.")
        Dim salida As AvanceLoteImportacionResultado
        Select Case solicitud.fase
            Case "CREAR_EXPEDIENTE"
                Dim r = coordinador.CrearExpediente(solicitud.loteId, solicitud.recursoId, solicitud.revisionEsperada, solicitud.claveOperacion)
                salida = ProyectarEfectoPublico(r.Resultado)
                salida.raizId = r.RaizId : salida.intentoId = r.IntentoId : salida.repeticion = r.Repeticion
            Case "ALMACENAR"
                Dim r = coordinador.AlmacenarArchivo(solicitud.loteId, solicitud.recursoId, solicitud.revisionEsperada, solicitud.claveOperacion)
                salida = ProyectarEfectoPublico(r.Resultado)
                salida.raizId = r.RaizId : salida.archivoId = r.ArchivoId : salida.intentoId = r.IntentoId : salida.repeticion = r.Repeticion
            Case Else
                Dim archivo = coordinador.MaterializarArchivo(solicitud.loteId, solicitud.recursoId, solicitud.revisionEsperada)
                salida = New AvanceLoteImportacionResultado With {.raizId = archivo.RaizId, .archivoId = archivo.ArchivoId,
                    .estado = If(archivo.ValoresEfectivosFijados, "CONFIRMADO", "RECHAZADO"), .codigo = If(archivo.ValoresEfectivosFijados, "VALORES_FIJADOS", "ARCHIVO_RECHAZADO")}
        End Select
        salida.loteId = solicitud.loteId : salida.fase = solicitud.fase
        Return salida
    End Function

    Public Function ReconciliarRecurso(solicitud As ReconciliarRecursoImportacionSolicitud) As ReconciliacionRecursoImportacionResultado
        If solicitud Is Nothing Then Throw EntradaOperacionInvalida()
        Dim r = ReconciliarIntento(solicitud.loteId, solicitud.intentoId, solicitud.revisionEsperada)
        Dim publico = ProyectarEfectoPublico(r.Resultado)
        Return New ReconciliacionRecursoImportacionResultado With {.loteId = r.LoteId, .intentoId = r.IntentoId, .fase = r.Fase, .resuelta = r.Resuelta,
            .estado = publico.estado, .codigo = publico.codigo, .repeticion = r.Repeticion, .expedienteId = publico.expedienteId,
            .relacionNivelId = publico.relacionNivelId, .gabineteId = publico.gabineteId, .documentoId = publico.documentoId, .inventarioId = publico.inventarioId}
    End Function

    Private Shared Function ProyectarEfectoPublico(resultado As ResultadoEfectoImportacion) As AvanceLoteImportacionResultado
        If resultado Is Nothing OrElse (resultado.Estado <> "CONFIRMADO" AndAlso resultado.Estado <> "INCIERTO") Then Throw NoPermitida()
        Return New AvanceLoteImportacionResultado With {.estado = resultado.Estado,
            .codigo = If(resultado.Estado = "INCIERTO", "RESULTADO_INCIERTO", If(resultado.Creacion Is Nothing, "DOCUMENTO_CONFIRMADO", "EXPEDIENTE_CONFIRMADO")),
            .expedienteId = If(resultado.Expediente Is Nothing, Nothing, resultado.Expediente.ExpedienteId),
            .relacionNivelId = If(resultado.Expediente Is Nothing, Nothing, resultado.Expediente.RelacionNivelId),
            .gabineteId = resultado.GabineteId, .documentoId = resultado.DocumentoId, .inventarioId = resultado.InventarioId}
    End Function
    Private Shared Function EntradaOperacionInvalida() As PerfilImportacionException
        Return New PerfilImportacionException("SOLICITUD_INVALIDA", "La fase y las referencias de la operacion no son validas.")
    End Function
End Class
