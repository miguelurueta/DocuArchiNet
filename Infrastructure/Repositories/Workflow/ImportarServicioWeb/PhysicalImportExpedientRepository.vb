Imports System
Imports System.Collections.Generic

' Repositorio idempotente: siempre demuestra la postcondición física antes de confirmar.
Public NotInheritable Class PhysicalImportExpedientRepository
    Implements IImportExpedientRepository

    Private ReadOnly _normalizer As IImportExpedientIdentityNormalizer
    Private ReadOnly _gateway As IPhysicalExpedientGateway

    Public Sub New(ByVal normalizer As IImportExpedientIdentityNormalizer,
                   ByVal gateway As IPhysicalExpedientGateway)
        If normalizer Is Nothing OrElse gateway Is Nothing Then Throw New ArgumentNullException("dependency")
        _normalizer = normalizer
        _gateway = gateway
    End Sub

    Public Function Buscar(ByVal contexto As ContextoImportacionServicio,
                           ByVal inscripcion As InscripcionImportacion,
                           ByVal configuracion As ConfiguracionExpedienteImportacion) As ResultadoEfectoExpedienteImportacion Implements IImportExpedientRepository.Buscar
        Dim identidad = Normalizar(contexto, inscripcion, configuracion)
        If Not identidad.Valida Then Return Result(EstadoEfectoExpedienteImportacion.Fallido, identidad.Codigo, False)

        Dim encontrados = _gateway.Localizar(contexto, identidad.NombreGabinete, identidad.ValorConsulta)
        If encontrados Is Nothing OrElse encontrados.Count = 0 Then
            Return Result(EstadoEfectoExpedienteImportacion.Ausente, "EXPEDIENT_NOT_FOUND", False)
        End If
        If encontrados.Count <> 1 Then
            Return Result(EstadoEfectoExpedienteImportacion.Conflicto, "EXPEDIENT_IDENTITY_CONFLICT", False)
        End If
        Return VerifySnapshot(encontrados(0), identidad, configuracion)
    End Function

    Public Function Crear(ByVal contexto As ContextoImportacionServicio,
                          ByVal inscripcion As InscripcionImportacion,
                          ByVal configuracion As ConfiguracionExpedienteImportacion) As ResultadoEfectoExpedienteImportacion Implements IImportExpedientRepository.Crear
        Dim existente = Buscar(contexto, inscripcion, configuracion)
        If existente.Estado <> EstadoEfectoExpedienteImportacion.Ausente Then Return existente
        If configuracion Is Nothing OrElse Not configuracion.CreacionAutomaticaHabilitada Then
            Return Result(EstadoEfectoExpedienteImportacion.Fallido, "EXPEDIENT_CREATION_DISABLED", False)
        End If

        Dim identidad = Normalizar(contexto, inscripcion, configuracion)
        Dim creado = _gateway.Crear(contexto, inscripcion, configuracion, identidad)

        If creado IsNot Nothing AndAlso String.Equals(creado.Codigo, "EXPEDIENT_IDENTITY_CONFLICT", StringComparison.Ordinal) Then
            Return Result(EstadoEfectoExpedienteImportacion.Conflicto, creado.Codigo, False)
        End If

        If creado IsNot Nothing AndAlso creado.IdExpediente.HasValue Then
            Dim snapshot = _gateway.Obtener(contexto, creado.IdExpediente.Value)
            Dim verifiedCreated = VerifySnapshot(snapshot, identidad, configuracion)
            If verifiedCreated.Estado = EstadoEfectoExpedienteImportacion.Confirmado OrElse
               verifiedCreated.Estado = EstadoEfectoExpedienteImportacion.Conflicto Then Return verifiedCreated
        End If

        ' Incluso con respuesta perdida, una segunda localización evita duplicar el expediente.
        Dim confirmado = Buscar(contexto, inscripcion, configuracion)
        If confirmado.Estado = EstadoEfectoExpedienteImportacion.Confirmado OrElse
           confirmado.Estado = EstadoEfectoExpedienteImportacion.Conflicto Then Return confirmado
        If creado Is Nothing OrElse Not creado.RespuestaRecibida Then
            Return Result(EstadoEfectoExpedienteImportacion.ResultadoIncierto, "EXPEDIENT_CREATE_RESULT_UNKNOWN", True)
        End If
        Return Result(EstadoEfectoExpedienteImportacion.Fallido, "EXPEDIENT_CREATE_NOT_CONFIRMED", True)
    End Function

    Public Function Verificar(ByVal contexto As ContextoImportacionServicio,
                              ByVal idExpediente As Long,
                              ByVal configuracion As ConfiguracionExpedienteImportacion) As ResultadoEfectoExpedienteImportacion Implements IImportExpedientRepository.Verificar
        If contexto Is Nothing OrElse idExpediente <= 0 OrElse configuracion Is Nothing Then
            Return Result(EstadoEfectoExpedienteImportacion.Fallido, "EXPEDIENT_VERIFY_INVALID", False)
        End If
        Dim snapshot = _gateway.Obtener(contexto, idExpediente)
        If snapshot Is Nothing Then Return Result(EstadoEfectoExpedienteImportacion.Ausente, "EXPEDIENT_NOT_FOUND", False)
        Return VerifyFields(snapshot, configuracion)
    End Function

    Private Function Normalizar(ByVal contexto As ContextoImportacionServicio,
                                ByVal inscripcion As InscripcionImportacion,
                                ByVal configuracion As ConfiguracionExpedienteImportacion) As IdentidadExpedienteNormalizada
        If contexto Is Nothing OrElse inscripcion Is Nothing OrElse configuracion Is Nothing Then
            Return New IdentidadExpedienteNormalizada With {.Valida = False, .Codigo = "EXPEDIENT_INPUT_INVALID"}
        End If
        Dim identidad = _normalizer.Normalizar(contexto, configuracion.NombreGabinete, inscripcion.Matricula, inscripcion.Proponente)
        If identidad.Valida Then inscripcion.MatriculaNormalizada = identidad.ValorPersistencia
        Return identidad
    End Function

    Private Shared Function VerifySnapshot(ByVal snapshot As ExpedienteFisicoImportacion,
                                           ByVal identidad As IdentidadExpedienteNormalizada,
                                           ByVal configuracion As ConfiguracionExpedienteImportacion) As ResultadoEfectoExpedienteImportacion
        If snapshot Is Nothing Then Return Result(EstadoEfectoExpedienteImportacion.Ausente, "EXPEDIENT_SNAPSHOT_UNAVAILABLE", False)
        If Not String.Equals(snapshot.NombreGabinete, identidad.NombreGabinete, StringComparison.OrdinalIgnoreCase) Then
            Return Result(EstadoEfectoExpedienteImportacion.Conflicto, "EXPEDIENT_CABINET_CONFLICT", False)
        End If
        If Not String.Equals(snapshot.ValorIdentidad, identidad.ValorPersistencia, StringComparison.OrdinalIgnoreCase) Then
            Return Result(EstadoEfectoExpedienteImportacion.Conflicto, "EXPEDIENT_PRIMARY_IDENTITY_CONFLICT", False)
        End If
        Return VerifyFields(snapshot, configuracion)
    End Function

    Private Shared Function VerifyFields(ByVal snapshot As ExpedienteFisicoImportacion,
                                         ByVal configuracion As ConfiguracionExpedienteImportacion) As ResultadoEfectoExpedienteImportacion
        If Not String.Equals(snapshot.NombreGabinete, configuracion.NombreGabinete, StringComparison.OrdinalIgnoreCase) Then
            Return Result(EstadoEfectoExpedienteImportacion.Conflicto, "EXPEDIENT_CABINET_CONFLICT", False)
        End If
        For Each field In configuracion.CamposIdentidad
            Dim actual As String = Nothing
            If field.Obligatorio AndAlso String.IsNullOrWhiteSpace(field.Valor) Then
                Return Result(EstadoEfectoExpedienteImportacion.Fallido, IdentityExpectationCode(field.NombreCampo), False)
            End If
            If snapshot.Campos Is Nothing OrElse Not snapshot.Campos.TryGetValue(field.NombreCampo, actual) Then Return Result(EstadoEfectoExpedienteImportacion.Conflicto, IdentityFieldCode("EXPEDIENT_IDENTITY_FIELD_MISSING", field.NombreCampo), False)
            If Not String.Equals(If(actual, String.Empty).Trim(), If(field.Valor, String.Empty).Trim(), StringComparison.OrdinalIgnoreCase) Then Return Result(EstadoEfectoExpedienteImportacion.Conflicto, "EXPEDIENT_IDENTITY_FIELD_CONFLICT", False)
        Next
        Return Result(EstadoEfectoExpedienteImportacion.Confirmado, "EXPEDIENT_CONFIRMED", False, snapshot.IdExpediente)
    End Function

    Private Shared Function IdentityExpectationCode(ByVal fieldName As String) As String
        Return IdentityFieldCode("EXPEDIENT_IDENTITY_EXPECTATION_UNRESOLVED", fieldName)
    End Function

    Private Shared Function IdentityFieldCode(ByVal prefix As String, ByVal fieldName As String) As String
        Dim safeName = If(fieldName, String.Empty).Trim().ToUpperInvariant()
        If safeName.Length > 36 Then safeName = safeName.Substring(0, 36)
        If safeName.Length = 0 Then Return prefix
        Return prefix & "_" & safeName
    End Function

    Private Shared Function Result(ByVal state As EstadoEfectoExpedienteImportacion,
                                  ByVal code As String,
                                  ByVal retryable As Boolean,
                                  Optional ByVal id As Nullable(Of Long) = Nothing) As ResultadoEfectoExpedienteImportacion
        Return New ResultadoEfectoExpedienteImportacion With {
            .Estado = state, .Codigo = code, .Reintentable = retryable, .IdExpediente = id
        }
    End Function
End Class
