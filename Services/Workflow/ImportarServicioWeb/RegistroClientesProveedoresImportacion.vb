Imports System
Imports System.Collections.Generic

' Registro aditivo para la ruta moderna; no reemplaza el registro síncrono existente.
Public NotInheritable Class RegistroClientesProveedoresImportacion
    Implements IRegistroClientesProveedoresImportacion

    Private ReadOnly _clientes As IDictionary(Of String, IExternalImportProviderClient)

    Public Sub New(ByVal clientes As IEnumerable(Of IExternalImportProviderClient))
        If clientes Is Nothing Then Throw New ArgumentNullException("clientes")
        _clientes = New Dictionary(Of String, IExternalImportProviderClient)(StringComparer.OrdinalIgnoreCase)
        For Each cliente In clientes
            If cliente Is Nothing Then Throw New ArgumentException("La colección no puede contener clientes nulos.", "clientes")
            Dim identidad = Normalizar(cliente.ProviderId)
            If identidad.Length = 0 Then Throw New ArgumentException("La identidad canónica es obligatoria.", "clientes")
            If _clientes.ContainsKey(identidad) Then Throw New ArgumentException("La identidad canónica está duplicada.", "clientes")
            _clientes.Add(identidad, cliente)
        Next
    End Sub

    Public Function Resolver(ByVal providerId As String) As ResultadoResolucionClienteProveedorImportacion Implements IRegistroClientesProveedoresImportacion.Resolver
        Dim cliente As IExternalImportProviderClient = Nothing
        If _clientes.TryGetValue(Normalizar(providerId), cliente) Then
            Return New ResultadoResolucionClienteProveedorImportacion With {.Cliente = cliente}
        End If
        Return New ResultadoResolucionClienteProveedorImportacion With {
            .Codigo = "PROVIDER_NOT_SUPPORTED",
            .MensajeVisible = "El proveedor solicitado no está disponible."
        }
    End Function

    Private Shared Function Normalizar(ByVal providerId As String) As String
        Return If(providerId, String.Empty).Trim()
    End Function
End Class
