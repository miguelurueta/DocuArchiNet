Imports System

Public NotInheritable Class ServicioAdjuntoRadicacion
    Private ReadOnly _repository As IContextoAdjuntoRadicacionRepository

    Public Sub New(ByVal repository As IContextoAdjuntoRadicacionRepository)
        If repository Is Nothing Then Throw New ArgumentNullException(NameOf(repository))
        _repository = repository
    End Sub

    Public Function Adjuntar(ByVal contextoModulo As ContextoModulo,
                             ByVal idRegistroEstado As Long,
                             ByVal radicadoInformativo As String,
                             ByVal preparar As Func(Of ContextoAdjuntoRadicacion, ResultadoAdjuntoRadicacion)) As ResultadoAdjuntoRadicacion
        If preparar Is Nothing Then
            Return ResultadoAdjuntoRadicacion.Fallo("No fue posible preparar el almacenamiento del documento.")
        End If

        Dim contexto As ContextoAdjuntoRadicacion = Nothing
        Dim result As String = _repository.ObtenerAutorizado(contextoModulo, idRegistroEstado, contexto)
        If Not String.Equals(result, "YES", StringComparison.OrdinalIgnoreCase) Then
            Return ResultadoAdjuntoRadicacion.Fallo(result)
        End If
        If contexto Is Nothing OrElse Not contexto.EsValido() Then
            Return ResultadoAdjuntoRadicacion.Fallo("El registro seleccionado no contiene un contexto válido para adjuntar el documento.")
        End If

        Dim informado As String = If(radicadoInformativo, String.Empty).Trim()
        If Not String.IsNullOrWhiteSpace(informado) AndAlso
           Not String.Equals(informado, contexto.Radicado, StringComparison.Ordinal) Then
            Return ResultadoAdjuntoRadicacion.Fallo(
                "El radicado enviado no corresponde al registro de estado seleccionado. Actualice la pantalla e intente nuevamente.",
                contexto)
        End If

        Try
            Dim resultado As ResultadoAdjuntoRadicacion = preparar(contexto)
            If resultado Is Nothing Then
                Return ResultadoAdjuntoRadicacion.Fallo("No fue posible preparar el almacenamiento del documento.", contexto)
            End If
            If resultado.Contexto Is Nothing Then resultado.Contexto = contexto
            Return resultado
        Catch
            Return ResultadoAdjuntoRadicacion.Fallo("No fue posible adjuntar el documento al radicado seleccionado.", contexto)
        End Try
    End Function
End Class
