Imports System
Imports System.Collections.Generic

' Único límite nuevo autorizado para invocar AlmacenaDocumentoTareaWorkflow.
Public NotInheritable Class LegacyImportDocumentStorageAdapter
    Implements IImportDocumentStoragePort

    Public Function Almacenar(ByVal comando As ComandoAlmacenamientoImportacion) As ResultadoFaseImportacion Implements IImportDocumentStoragePort.Almacenar
        If comando Is Nothing OrElse String.IsNullOrWhiteSpace(comando.RutaArchivo) OrElse comando.IdTareaWorkflow <= 0 Then
            Return Fallo("INVALID_STORAGE_COMMAND", "No fue posible preparar el documento.", True)
        End If
        Try
            Dim idImagen As Integer = 0
            Dim imagen As stru_datos_image_lista = Nothing
            Dim campos As New List(Of CDcamposAsignaAlmacenamiento)()
            Dim respuesta = New ClassAlmacenamiento().AlmacenaDocumentoTareaWorkflow(
                1, comando.NombreGabinete, comando.Radicado, comando.RutaArchivo,
                comando.NombreRutaWorkflow, comando.IdRutaWorkflow, comando.IdTareaWorkflow,
                comando.DescripcionTipo, comando.IdTipoListaChequeo, comando.TipoAlmacenamiento,
                campos, Nothing, comando.NombreCaso, comando.NombreClaseFormatoDocumento,
                idImagen, imagen)
            If String.Equals(respuesta, "YES", StringComparison.OrdinalIgnoreCase) Then
                Return New ResultadoFaseImportacion With {.Exitoso = True, .PersistenciaConocida = True, .IdDocumento = idImagen}
            End If
            Return Fallo("DOCUMENT_STORAGE_REJECTED", "No fue posible almacenar el documento.", True)
        Catch
            Return Fallo("DOCUMENT_STORAGE_UNCERTAIN", "No fue posible confirmar el almacenamiento del documento.", False)
        End Try
    End Function

    Private Shared Function Fallo(ByVal codigo As String, ByVal mensaje As String, ByVal conocida As Boolean) As ResultadoFaseImportacion
        Return New ResultadoFaseImportacion With {.Codigo = codigo, .MensajeVisible = mensaje, .PersistenciaConocida = conocida, .Reintentable = conocida}
    End Function
End Class
