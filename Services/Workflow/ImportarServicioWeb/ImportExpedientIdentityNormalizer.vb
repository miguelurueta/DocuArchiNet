Imports System
Imports System.Text.RegularExpressions

' Regla única usada por localización, creación y recuperación, sin distinguir principal/secundario.
Public NotInheritable Class ImportExpedientIdentityNormalizer
    Implements IImportExpedientIdentityNormalizer

    Public Function Normalizar(ByVal contexto As ContextoImportacionServicio,
                               ByVal nombreGabinete As String,
                               ByVal matricula As String,
                               ByVal proponente As String) As IdentidadExpedienteNormalizada Implements IImportExpedientIdentityNormalizer.Normalizar
        Dim cabinet = If(nombreGabinete, String.Empty).Trim().ToUpperInvariant()
        Select Case cabinet
            Case "MERCANTIL"
                Dim canonical = NormalizeEnrollment(matricula)
                Return Build(cabinet, canonical, canonical)
            Case "ESAL"
                Dim canonical = NormalizeEnrollment(matricula)
                Return Build(cabinet, canonical, canonical)
            Case "RUP"
                Dim canonical = NormalizeEnrollment(proponente)
                Return Build(cabinet, canonical, canonical)
            Case Else
                Return Invalid("EXPEDIENT_CABINET_NOT_SUPPORTED", cabinet)
        End Select
    End Function

    Private Shared Function NormalizeEnrollment(ByVal value As String) As String
        Dim canonical = Regex.Replace(Clean(value), "[^0-9]", String.Empty)
        canonical = canonical.TrimStart("0"c)
        Return canonical
    End Function

    Private Shared Function Clean(ByVal value As String) As String
        Return If(value, String.Empty).Trim()
    End Function

    Private Shared Function Build(ByVal cabinet As String,
                                  ByVal queryValue As String,
                                  ByVal persistedValue As String) As IdentidadExpedienteNormalizada
        If String.IsNullOrWhiteSpace(queryValue) OrElse String.IsNullOrWhiteSpace(persistedValue) Then
            Return Invalid("EXPEDIENT_IDENTITY_INVALID", cabinet)
        End If
        Return New IdentidadExpedienteNormalizada With {
            .Valida = True,
            .NombreGabinete = cabinet,
            .ValorConsulta = queryValue,
            .ValorPersistencia = persistedValue
        }
    End Function

    Private Shared Function Invalid(ByVal code As String, ByVal cabinet As String) As IdentidadExpedienteNormalizada
        Return New IdentidadExpedienteNormalizada With {
            .Valida = False,
            .Codigo = code,
            .NombreGabinete = cabinet
        }
    End Function
End Class
