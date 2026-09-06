Imports System
Imports System.Collections.Generic

Public Class RegistroProveedoresImportacion
    Implements IRegistroProveedoresImportacion

    Private ReadOnly _proveedores As IDictionary(Of String, IExternalImportProvider)

    Public Sub New(ByVal proveedores As IEnumerable(Of IExternalImportProvider))
        If proveedores Is Nothing Then
            Throw New ArgumentNullException("proveedores")
        End If

        _proveedores = New Dictionary(Of String, IExternalImportProvider)(StringComparer.OrdinalIgnoreCase)
        For Each proveedor As IExternalImportProvider In proveedores
            If proveedor Is Nothing Then
                Throw New ArgumentException("La colección no puede contener proveedores nulos.", "proveedores")
            End If

            Dim providerId As String = NormalizarIdentidad(proveedor.ProviderId)
            If providerId.Length = 0 Then
                Throw New ArgumentException("La identidad canónica del proveedor es obligatoria.", "proveedores")
            End If
            If _proveedores.ContainsKey(providerId) Then
                Throw New ArgumentException("La identidad canónica del proveedor está duplicada.", "proveedores")
            End If
            _proveedores.Add(providerId, proveedor)
        Next
    End Sub

    Public Function Resolver(ByVal providerId As String) As ResultadoResolucionProveedorImportacion Implements IRegistroProveedoresImportacion.Resolver
        Dim identidad As String = NormalizarIdentidad(providerId)
        Dim proveedor As IExternalImportProvider = Nothing
        If identidad.Length > 0 AndAlso _proveedores.TryGetValue(identidad, proveedor) Then
            Return New ResultadoResolucionProveedorImportacion With {.Proveedor = proveedor}
        End If

        Return New ResultadoResolucionProveedorImportacion With {
            .Codigo = "PROVIDER_NOT_SUPPORTED",
            .MensajeVisible = "El proveedor solicitado no está disponible."
        }
    End Function

    Private Shared Function NormalizarIdentidad(ByVal providerId As String) As String
        Return If(providerId, String.Empty).Trim()
    End Function
End Class
