Option Strict On
Option Compare Binary
Imports System
Imports System.Linq

Public Partial Class CoordinadorImportacionCarpetas
    Private ReadOnly _vistaReconciliacion As Func(Of ContextoImportacionCarpetas, String, String, VistaReconciliacionImportacion)
    Private ReadOnly _verificarReconciliacion As Func(Of VistaReconciliacionImportacion, ResultadoEfectoImportacion)
    Private ReadOnly _guardarReconciliacion As Func(Of VistaReconciliacionImportacion, ResultadoEfectoImportacion, ResultadoEfectoImportacion)
    Private ReadOnly _cerrarReservaSinInicio As Func(Of VistaReconciliacionImportacion, ResultadoEfectoImportacion)

    Public Sub New(identidad As Func(Of ContextoImportacionCarpetas), repositorio As IControlLoteImportacionRepository, destino As Func(Of String, ContextoImportacionCarpetas), autorizar As Action(Of ContextoImportacionCarpetas, ConfiguracionPerfilImportacion), vista As Func(Of ContextoImportacionCarpetas, String, String, VistaReconciliacionImportacion), verificar As Func(Of VistaReconciliacionImportacion, ResultadoEfectoImportacion), guardar As Func(Of VistaReconciliacionImportacion, ResultadoEfectoImportacion, ResultadoEfectoImportacion), Optional cerrarReserva As Func(Of VistaReconciliacionImportacion, ResultadoEfectoImportacion) = Nothing)
        Me.New(identidad, repositorio, destino, autorizar)
        If vista Is Nothing OrElse verificar Is Nothing OrElse guardar Is Nothing Then Throw New ArgumentNullException()
        _vistaReconciliacion = vista : _verificarReconciliacion = verificar : _guardarReconciliacion = guardar
        _cerrarReservaSinInicio = cerrarReserva
    End Sub

    'Operacion explicita: confirma el cierre del control, nunca un documento.
    'No tiene puertos de almacenamiento ni concede una nueva reserva.
    Public Function CerrarReservaSinInicio(solicitud As ReconciliarRecursoImportacionSolicitud) As ReconciliacionRecursoImportacionResultado
        If _cerrarReservaSinInicio Is Nothing Then Throw New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "El cierre no esta compuesto.")
        If solicitud Is Nothing OrElse Not ValidadorPerfilImportacion.ReferenciaValida(solicitud.loteId) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(solicitud.intentoId) OrElse solicitud.revisionEsperada = 0 Then Throw EntradaOperacionInvalida()
        Dim identidad = _identidad()
        If identidad Is Nothing OrElse Not identidad.PuedeImportar Then Throw Denegado()
        Dim referencia = _repositorio.ObtenerContextoLote(identidad, solicitud.loteId)
        If referencia Is Nothing OrElse referencia.UsuarioRef <> identidad.UsuarioRef OrElse referencia.AmbitoRef <> identidad.AmbitoRef Then Throw Denegado()
        Dim contexto = _destino(referencia.NivelRef)
        If Not MismoContextoControl(contexto, referencia) Then Throw Denegado()
        Dim vista = _vistaReconciliacion(contexto, solicitud.loteId, solicitud.intentoId)
        ValidarVistaReconciliacion(vista, contexto, solicitud.loteId, solicitud.intentoId)
        ReautorizarFase(contexto, vista.Lote.Snapshot.Configuracion)
        Dim repeticion = vista.Intento.Estado = "RECHAZADO"
        If Not repeticion AndAlso vista.Lote.Revision <> solicitud.revisionEsperada Then Throw NoPermitida()
        _cerrarReservaSinInicio(CopiarReconciliacion(vista))
        Return New ReconciliacionRecursoImportacionResultado With {.loteId = solicitud.loteId, .intentoId = solicitud.intentoId, .raizId = vista.Raiz.RaizId, .archivoId = vista.Archivo.ArchivoId, .fase = "ALMACENAR", .estado = "CONFIRMADO", .codigo = "RESERVA_CERRADA_SIN_INICIO", .resuelta = True, .repeticion = repeticion}
    End Function

    'Una comprobacion de un intento. No contiene puertos de ejecucion/staging,
    'no renueva reclamaciones ni deduce terminacion por el tiempo transcurrido.
    Public Function ReconciliarIntento(loteId As String, intentoId As String, revisionEsperada As UInteger) As ResultadoReconciliacionImportacion
        If _vistaReconciliacion Is Nothing Then Throw New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "La reconciliacion no esta compuesta.")
        If Not ValidadorPerfilImportacion.ReferenciaValida(loteId) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(intentoId) OrElse revisionEsperada = 0 Then Throw New PerfilImportacionException("SOLICITUD_INVALIDA", "La referencia de reconciliacion no es valida.")
        Dim identidad = _identidad()
        If identidad Is Nothing OrElse Not identidad.PuedeImportar Then Throw Denegado()
        Dim referencia = _repositorio.ObtenerContextoLote(identidad, loteId)
        If referencia Is Nothing OrElse referencia.UsuarioRef <> identidad.UsuarioRef OrElse referencia.AmbitoRef <> identidad.AmbitoRef Then Throw Denegado()
        Dim contexto = _destino(referencia.NivelRef)
        If Not MismoContextoControl(contexto, referencia) Then Throw Denegado()
        Dim vista = _vistaReconciliacion(contexto, loteId, intentoId)
        ValidarVistaReconciliacion(vista, contexto, loteId, intentoId)
        ReautorizarFase(contexto, vista.Lote.Snapshot.Configuracion)
        If ConfirmacionReconciliacionPersistida(vista) Then Return ProyectarReconciliacion(vista, vista.Intento.Resultado, True, True)
        If vista.Lote.Revision <> revisionEsperada Then Throw NoPermitida()
        If Not PuedeReconciliar(vista) Then Return ProyectarReconciliacion(vista, vista.Intento.Resultado, False, False)
        Dim comprobado As ResultadoEfectoImportacion
        Try
            comprobado = _verificarReconciliacion(CopiarReconciliacion(vista))
        Catch ex As Exception
            Return ProyectarReconciliacion(vista, vista.Intento.Resultado, False, False)
        End Try
        If comprobado Is Nothing OrElse comprobado.Estado <> "CONFIRMADO" Then Return ProyectarReconciliacion(vista, vista.Intento.Resultado, False, False)
        ReautorizarFase(contexto, vista.Lote.Snapshot.Configuracion)
        'El repositorio compara de nuevo la vista y la prueba ya registrada;
        'el verificador no puede sustituir IDs o valores mediante su respuesta.
        Try
            _guardarReconciliacion(CopiarReconciliacion(vista), comprobado)
        Catch ex As PerfilImportacionException
            Throw
        Catch ex As Exception
            'Una respuesta perdida no autoriza repetir escrituras ni efectos.
            'La lectura siguiente determina si el checkpoint quedo confirmado.
        End Try
        Dim ultima = _vistaReconciliacion(contexto, loteId, intentoId)
        ValidarVistaReconciliacion(ultima, contexto, loteId, intentoId)
        Return ProyectarReconciliacion(ultima, ultima.Intento.Resultado, ConfirmacionReconciliacionPersistida(ultima), False)
    End Function

    Friend Shared Sub ValidarVistaReconciliacion(vista As VistaReconciliacionImportacion, contexto As ContextoImportacionCarpetas, loteId As String, intentoId As String)
        If vista Is Nothing OrElse vista.Lote Is Nothing OrElse vista.Plan Is Nothing OrElse vista.Raiz Is Nothing OrElse vista.Intento Is Nothing OrElse Not MismoContextoControl(contexto, vista.Lote.Contexto) OrElse Not contexto.PuedeImportar OrElse vista.Lote.LoteId <> loteId OrElse vista.Intento.IntentoId <> intentoId OrElse vista.Raiz.LoteId <> loteId OrElse vista.Intento.LoteId <> loteId OrElse vista.Intento.RaizId <> vista.Raiz.RaizId OrElse vista.Lote.Revision = 0 OrElse vista.Raiz.Revision = 0 Then Throw Denegado()
        If vista.Plan.Estado <> "CONFIRMADO" OrElse vista.Plan.PlanId <> vista.Lote.PlanId OrElse Not MismoContextoControl(contexto, vista.Plan.Contexto) OrElse vista.Lote.Snapshot Is Nothing OrElse vista.Lote.Snapshot.Configuracion Is Nothing OrElse PerfilImportacionJson.Escribir(vista.Plan.Snapshot) <> PerfilImportacionJson.Escribir(vista.Lote.Snapshot) OrElse vista.Plan.Resultado Is Nothing OrElse vista.Plan.Resultado.Elementos Is Nothing Then Throw NoPermitida()
        If vista.Plan.Manifiesto Is Nothing OrElse vista.Plan.Manifiesto.Raices Is Nothing OrElse vista.Plan.Manifiesto.Archivos Is Nothing Then Throw NoPermitida()
        Dim raices = vista.Plan.Manifiesto.Raices.Where(Function(r) r IsNot Nothing AndAlso r.IdCliente = vista.Raiz.ClienteRaizId).ToList()
        If raices.Count <> 1 OrElse raices(0).Nombre <> vista.Raiz.NombreOriginal Then Throw NoPermitida()
        ExigirValoresPlanConsolidacion(vista.Plan, "RAIZ", vista.Raiz.ClienteRaizId, Nothing, vista.Raiz.ValoresPlan)
        If vista.Intento.Fase = "CREAR_EXPEDIENTE" Then
            If vista.Archivo IsNot Nothing OrElse vista.Intento.ArchivoId IsNot Nothing Then Throw NoPermitida()
        Else
            If vista.Archivo Is Nothing OrElse vista.Archivo.ArchivoId <> vista.Intento.ArchivoId OrElse vista.Archivo.RaizId <> vista.Raiz.RaizId OrElse vista.Archivo.Revision = 0 Then Throw NoPermitida()
            Dim originales = vista.Plan.Manifiesto.Archivos.Where(Function(a) a IsNot Nothing AndAlso a.RaizIdCliente = vista.Raiz.ClienteRaizId AndAlso a.IdCliente = vista.Archivo.ClienteArchivoId).ToList()
            If originales.Count <> 1 OrElse originales(0).Nombre <> vista.Archivo.NombreOriginal OrElse originales(0).RutaRelativa <> vista.Archivo.RutaRelativa OrElse originales(0).Bytes <> vista.Archivo.BytesPrevistos OrElse originales(0).UltimaModificacionLocal <> vista.Archivo.UltimaModificacionLocal Then Throw NoPermitida()
            ExigirValoresPlanConsolidacion(vista.Plan, "ARCHIVO", vista.Raiz.ClienteRaizId, vista.Archivo.ClienteArchivoId, vista.Archivo.ValoresPlan)
        End If
    End Sub
    Friend Shared Function PuedeReconciliar(vista As VistaReconciliacionImportacion) As Boolean
        Dim intento = vista.Intento, prueba = intento.Resultado
        If intento.Estado <> "INCIERTO" OrElse intento.FinUtc IsNot Nothing OrElse intento.EsNuevaReserva OrElse prueba Is Nothing OrElse prueba.Estado <> "INCIERTO" OrElse {"COMPLETADO", "FALLIDO"}.Contains(vista.Lote.Estado) Then Return False
        If intento.Fase = "CREAR_EXPEDIENTE" Then
            Return vista.Raiz.Estado = "INCIERTO" AndAlso vista.Raiz.IntentoActivoId = intento.IntentoId AndAlso vista.Raiz.IdentidadExpediente Is Nothing AndAlso prueba.Expediente IsNot Nothing AndAlso prueba.Expediente.OrigenLogico = "RA" AndAlso prueba.Expediente.NivelRef = vista.Lote.Contexto.NivelRef AndAlso ValidadorPerfilImportacion.ReferenciaValida(prueba.Expediente.ExpedienteId) AndAlso ValidadorPerfilImportacion.ReferenciaValida(prueba.Expediente.RelacionNivelId) AndAlso prueba.Creacion IsNot Nothing AndAlso prueba.Creacion.IntentoId = intento.IntentoId AndAlso prueba.Creacion.CicloArchivoNombre = "Archivo Gestión" AndAlso ValidadorPerfilImportacion.ReferenciaValida(prueba.Creacion.CicloArchivoId)
        End If
        If intento.Fase <> "ALMACENAR" Then Return False
        Return vista.Archivo IsNot Nothing AndAlso vista.Archivo.EstadoEfecto = "INCIERTO" AndAlso vista.Archivo.IntentoActivoId = intento.IntentoId AndAlso vista.Raiz.IntentoActivoId Is Nothing AndAlso {"INCIERTO", "EXPEDIENTE_CONFIRMADO"}.Contains(vista.Raiz.Estado) AndAlso vista.Raiz.IdentidadExpediente IsNot Nothing AndAlso ValidadorPerfilImportacion.ReferenciaValida(prueba.DocumentoId) AndAlso ValidadorPerfilImportacion.ReferenciaValida(prueba.InventarioId) AndAlso PerfilImportacionJson.Escribir(vista.Archivo.Resultado) = PerfilImportacionJson.Escribir(prueba)
    End Function
    Friend Shared Function ConfirmacionReconciliacionPersistida(vista As VistaReconciliacionImportacion) As Boolean
        Dim i = vista.Intento, resultado = i.Resultado
        If i.Estado <> "CONFIRMADO" OrElse i.FinUtc Is Nothing OrElse resultado Is Nothing OrElse resultado.Estado <> "CONFIRMADO" OrElse vista.Raiz.IdentidadExpediente Is Nothing OrElse PerfilImportacionJson.Escribir(resultado.Expediente) <> PerfilImportacionJson.Escribir(vista.Raiz.IdentidadExpediente) Then Return False
        If resultado.Expediente.OrigenLogico <> "RA" OrElse resultado.Expediente.NivelRef <> vista.Lote.Contexto.NivelRef OrElse Not ValidadorPerfilImportacion.ReferenciaValida(resultado.Expediente.ExpedienteId) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(resultado.Expediente.RelacionNivelId) Then Return False
        If i.Fase = "CREAR_EXPEDIENTE" Then Return vista.Raiz.IntentoActivoId Is Nothing AndAlso resultado.Creacion IsNot Nothing AndAlso resultado.Creacion.IntentoId = i.IntentoId AndAlso resultado.Creacion.CicloArchivoNombre = "Archivo Gestión" AndAlso ValidadorPerfilImportacion.ReferenciaValida(resultado.Creacion.CicloArchivoId) AndAlso resultado.Codigo = "EXPEDIENTE_CONFIRMADO"
        Return i.Fase = "ALMACENAR" AndAlso vista.Archivo IsNot Nothing AndAlso vista.Archivo.EstadoEfecto = "COMPLETADO" AndAlso vista.Archivo.IntentoActivoId Is Nothing AndAlso resultado.Codigo = "DOCUMENTO_CONFIRMADO" AndAlso ValidadorPerfilImportacion.ReferenciaValida(resultado.DocumentoId) AndAlso ValidadorPerfilImportacion.ReferenciaValida(resultado.InventarioId) AndAlso PerfilImportacionJson.Escribir(vista.Archivo.Resultado) = PerfilImportacionJson.Escribir(resultado)
    End Function
    Private Shared Function ProyectarReconciliacion(vista As VistaReconciliacionImportacion, prueba As ResultadoEfectoImportacion, resuelta As Boolean, repeticion As Boolean) As ResultadoReconciliacionImportacion
        Dim resultado = If(prueba Is Nothing, New ResultadoEfectoImportacion(), CopiarReconciliacion(prueba))
        resultado.Estado = If(resuelta, "CONFIRMADO", "INCIERTO")
        resultado.Codigo = If(resuelta, If(vista.Intento.Fase = "CREAR_EXPEDIENTE", "EXPEDIENTE_CONFIRMADO", "DOCUMENTO_CONFIRMADO"), "RESULTADO_INCIERTO")
        resultado.Mensaje = If(resuelta, "La evidencia persistida y sus relaciones fueron comprobadas.", "La evidencia no permite resolver este intento; se conserva la reclamacion sin repetir efectos.")
        Return New ResultadoReconciliacionImportacion With {.LoteId = vista.Lote.LoteId, .IntentoId = vista.Intento.IntentoId, .Fase = vista.Intento.Fase, .Resuelta = resuelta, .Repeticion = repeticion, .Resultado = resultado}
    End Function
    Private Shared Function CopiarReconciliacion(Of T)(valor As T) As T
        Return PerfilImportacionJson.Leer(Of T)(PerfilImportacionJson.Escribir(valor))
    End Function
End Class
