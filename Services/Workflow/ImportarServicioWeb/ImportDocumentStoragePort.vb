Imports System.Collections.Generic

Public Class ComandoAlmacenamientoImportacion
    Public Property RutaArchivo As String
    Public Property NombreGabinete As String
    Public Property Radicado As String
    Public Property NombreRutaWorkflow As String
    Public Property IdRutaWorkflow As Integer
    Public Property IdTareaWorkflow As Long
    Public Property DescripcionTipo As String
    Public Property IdTipoListaChequeo As Integer
    Public Property TipoAlmacenamiento As Integer
    Public Property NombreCaso As String
    Public Property NombreClaseFormatoDocumento As String
    Public Property NombreArchivoOrigen As String
    Public Property FormatoProveedor As String
    Public Property TipoContenidoOrigen As String
    Public Property Campos As IList(Of CampoAlmacenamientoImportacion)

    Public Sub New()
        Campos = New List(Of CampoAlmacenamientoImportacion)()
        NombreCaso = "SII"
    End Sub
End Class

Public Class CampoAlmacenamientoImportacion
    Public Property Nombre As String
    Public Property Valor As String
End Class

Public Interface IImportDocumentStoragePort
    Function Almacenar(ByVal comando As ComandoAlmacenamientoImportacion) As ResultadoFaseImportacion
End Interface
