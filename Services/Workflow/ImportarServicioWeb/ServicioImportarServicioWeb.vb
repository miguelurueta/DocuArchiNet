Imports System

Public Class ServicioImportarServicioWeb
    Private ReadOnly _registro As IRegistroProveedoresImportacion
    Private ReadOnly _validador As ValidadorContextoImportacion

    Public Sub New(ByVal registro As IRegistroProveedoresImportacion,
                   ByVal validador As ValidadorContextoImportacion)
        If registro Is Nothing Then Throw New ArgumentNullException("registro")
        If validador Is Nothing Then Throw New ArgumentNullException("validador")
        _registro = registro
        _validador = validador
    End Sub

    Public Function ResolverCapacidades(ByVal contexto As ContextoImportacionServicio,
                                        ByVal operationId As String,
                                        ByVal correlationId As String) As ResolveCapabilitiesResponseDto
        Dim respuesta As New ResolveCapabilitiesResponseDto With {
            .OperationId = operationId,
            .CorrelationId = correlationId,
            .ProviderId = If(contexto Is Nothing, Nothing, contexto.ProviderId)
        }
        Dim validacion As ResultadoValidacionContextoImportacion = _validador.Validar(contexto)
        If Not validacion.Valido Then
            respuesta.Error = CrearError(validacion)
            Return respuesta
        End If
        Dim resolucion As ResultadoResolucionProveedorImportacion = _registro.Resolver(contexto.ProviderId)
        If Not resolucion.Encontrado Then
            respuesta.Error = CrearError(resolucion.Codigo, resolucion.MensajeVisible)
            Return respuesta
        End If
        For Each capacidad As CapacidadProveedorImportacion In resolucion.Proveedor.ResolverCapacidades(contexto)
            respuesta.Capabilities.Add(New ProviderCapabilityDto With {
                .Codigo = capacidad.Codigo,
                .Habilitada = capacidad.Habilitada,
                .TimeoutSeconds = capacidad.TimeoutSeconds
            })
        Next
        respuesta.ContextAllowed = True
        Return respuesta
    End Function

    Public Function ConsultarElementos(ByVal contexto As ContextoImportacionServicio,
                                       ByVal operationId As String,
                                       ByVal correlationId As String,
                                       ByVal continuationToken As String,
                                       ByVal pageSize As Nullable(Of Integer)) As QueryItemsResponseDto
        Dim respuesta As New QueryItemsResponseDto With {
            .OperationId = operationId,
            .CorrelationId = correlationId
        }
        Dim validacion As ResultadoValidacionContextoImportacion = _validador.Validar(contexto)
        If Not validacion.Valido Then
            respuesta.Error = CrearError(validacion)
            Return respuesta
        End If
        Dim resolucion As ResultadoResolucionProveedorImportacion = _registro.Resolver(contexto.ProviderId)
        If Not resolucion.Encontrado Then
            respuesta.Error = CrearError(resolucion.Codigo, resolucion.MensajeVisible)
            Return respuesta
        End If
        For Each elemento As ElementoExternoImportacion In resolucion.Proveedor.ConsultarElementos(contexto, continuationToken, pageSize)
            respuesta.Items.Add(New ExternalItemDto With {
                .ExternalKey = elemento.Identidad.ExternalKey,
                .DisplayName = elemento.NombreVisible,
                .ContentType = elemento.TipoContenido,
                .Length = elemento.Longitud,
                .PreviewAvailable = elemento.PermitePreview
            })
        Next
        Return respuesta
    End Function

    Private Shared Function CrearError(ByVal validacion As ResultadoValidacionContextoImportacion) As ErrorImportacionServicioDto
        Return CrearError(validacion.Codigo, validacion.MensajeVisible)
    End Function

    Private Shared Function CrearError(ByVal codigo As String,
                                       ByVal mensajeVisible As String) As ErrorImportacionServicioDto
        Return New ErrorImportacionServicioDto With {
            .Codigo = codigo,
            .MensajeVisible = mensajeVisible,
            .EsReintentable = False
        }
    End Function
End Class
