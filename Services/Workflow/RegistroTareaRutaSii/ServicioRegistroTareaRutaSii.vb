Imports System
Imports System.Text.RegularExpressions

Public NotInheritable Class LegacyConsultaAutoritativaRutaSii
    Implements IConsultaAutoritativaRutaSii
    Public Function Consultar(ByVal recibo As String, ByRef datos As ConsultaAutoritativaRegistroRutaSii) As Boolean Implements IConsultaAutoritativaRutaSii.Consultar
        Dim receiptData As Class_parram_consultarRecibo = Nothing
        Dim radicadoData As Class_parram_consultarRadicado = Nothing
        Dim result = New Class_ConSultaRecibo().Solicita_datos_estructura_consulta_recibo_ruta_interfaz_SII(recibo, receiptData, radicadoData)
        If Not String.Equals(result, "YES", StringComparison.Ordinal) OrElse receiptData Is Nothing OrElse radicadoData Is Nothing Then
            datos = Nothing
            Return False
        End If
        datos = New ConsultaAutoritativaRegistroRutaSii With {.Recibo = receiptData, .Radicado = radicadoData}
        Return True
    End Function
End Class

Public NotInheritable Class ServicioRegistroTareaRutaSii
    Private ReadOnly _repository As IRegistroTareaRutaSiiRepository
    Private ReadOnly _relation As IRelacionRutaSiiGateway
    Private ReadOnly _sii As IConsultaAutoritativaRutaSii

    Public Sub New(ByVal repository As IRegistroTareaRutaSiiRepository,
                   ByVal relation As IRelacionRutaSiiGateway,
                   ByVal sii As IConsultaAutoritativaRutaSii)
        If repository Is Nothing OrElse relation Is Nothing OrElse sii Is Nothing Then Throw New ArgumentNullException("dependency")
        _repository = repository : _relation = relation : _sii = sii
    End Sub

    Public Function Ejecutar(ByVal contexto As ContextoModulo, ByVal solicitud As SolicitudRegistroTareaRutaSii) As ResultadoRegistroTareaRutaSii
        If contexto Is Nothing OrElse Not contexto.EsValido() Then Return Failure("UNAUTHORIZED", "La sesión Workflow no es válida.")
        If solicitud Is Nothing OrElse Not Regex.IsMatch(If(solicitud.recibo, String.Empty), "^[SR][0-9]{9}$") OrElse solicitud.id_tramite <= 0 OrElse solicitud.id_actividad <= 0 Then
            Return Failure("INVALID_REQUEST", "El recibo, trámite o actividad no son válidos.")
        End If
        If Not _repository.TienePermiso(contexto) Then Return Failure("FORBIDDEN", "El usuario no tiene permiso para registrar tareas de ruta SII.")

        Dim siiData As ConsultaAutoritativaRegistroRutaSii = Nothing
        If Not _sii.Consultar(solicitud.recibo, siiData) Then Return Failure("SII_UNAVAILABLE", "No fue posible validar nuevamente el recibo en SII.")
        If siiData Is Nothing OrElse siiData.Recibo Is Nothing OrElse siiData.Radicado Is Nothing OrElse
           Not String.Equals(Convert.ToString(siiData.Radicado.recibo).Trim().ToUpperInvariant(), solicitud.recibo, StringComparison.Ordinal) OrElse
           String.IsNullOrWhiteSpace(Convert.ToString(siiData.Radicado.radicado)) OrElse
           String.IsNullOrWhiteSpace(Convert.ToString(siiData.Radicado.nombre)) Then
            Return Failure("SII_CONTEXT_MISMATCH", "La validación SII no corresponde de forma completa al recibo solicitado.")
        End If
        Dim authoritative = _repository.ResolverContexto(contexto, solicitud, siiData)
        If authoritative Is Nothing Then Return Failure("CATALOG_MISMATCH", "El trámite, actividad, sede o ruta no tienen una configuración única y vigente.")
        Dim persisted = _repository.RegistrarTareaConOutbox(contexto, authoritative)
        Dim alreadyRegistered = persisted IsNot Nothing AndAlso String.Equals(persisted.Codigo, "ALREADY_REGISTERED", StringComparison.Ordinal)
        If alreadyRegistered AndAlso persisted.Evento Is Nothing Then
            Return Failure("ALREADY_REGISTERED", "El recibo ya se encuentra registrado en la ruta de trabajo.")
        End If
        If persisted Is Nothing OrElse persisted.Evento Is Nothing Then Return Failure(If(persisted Is Nothing, "PERSISTENCE_ERROR", persisted.Codigo), "No fue posible registrar la tarea.")
        If Not alreadyRegistered AndAlso Not String.Equals(persisted.Codigo, "REGISTERED", StringComparison.Ordinal) Then
            Return Failure(persisted.Codigo, "No fue posible registrar la tarea.")
        End If

        Try
            Dim relationResult = _relation.Materializar(contexto, persisted.Evento)
            If relationResult = "CONFIRMED" OrElse relationResult = "NO_APLICA" Then
                _repository.ConfirmarEvento(contexto, persisted.Evento.OperationId, relationResult)
                If alreadyRegistered Then
                    Return New ResultadoRegistroTareaRutaSii With {.Codigo = "ALREADY_REGISTERED", .Mensaje = "El recibo ya se encuentra registrado en la ruta de trabajo.", .Evento = persisted.Evento}
                End If
                Return New ResultadoRegistroTareaRutaSii With {.Codigo = "YES", .Mensaje = "Registro confirmado.", .Evento = persisted.Evento}
            End If
            _repository.ConfirmarEvento(contexto, persisted.Evento.OperationId, "REVIEW_REQUIRED")
            Return New ResultadoRegistroTareaRutaSii With {.Codigo = If(alreadyRegistered, "ALREADY_REGISTERED_RELATION_PENDING", "REGISTERED_RELATION_PENDING"), .Mensaje = If(alreadyRegistered, "El recibo ya estaba registrado y la relación requiere revisión.", "La tarea fue registrada y la relación requiere revisión."), .Evento = persisted.Evento}
        Catch
            Try
                _repository.MarcarEventoPendiente(contexto, persisted.Evento.OperationId, "DOCUARCHI_UNAVAILABLE")
            Catch
            End Try
            Return New ResultadoRegistroTareaRutaSii With {.Codigo = If(alreadyRegistered, "ALREADY_REGISTERED_RELATION_PENDING", "REGISTERED_RELATION_PENDING"), .Mensaje = If(alreadyRegistered, "El recibo ya estaba registrado y la relación quedó pendiente.", "La tarea fue registrada y la relación quedó pendiente."), .Evento = persisted.Evento}
        End Try
    End Function

    Private Shared Function Failure(ByVal code As String, ByVal message As String) As ResultadoRegistroTareaRutaSii
        Return New ResultadoRegistroTareaRutaSii With {.Codigo = code, .Mensaje = message}
    End Function
End Class
