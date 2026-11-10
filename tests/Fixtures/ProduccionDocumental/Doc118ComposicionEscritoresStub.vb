Option Strict On
Imports System.Web

'El dispatcher y composición documental reales se compilan. La fábrica de
'creación y el constructor de puertos legacy quedan fuera de este arnés.
'No incluir este archivo en el proyecto productivo.
Public NotInheritable Class ComposicionCreacionImportacion
    Public Shared Function Crear(http As HttpContext) As CoordinadorImportacionCarpetas
        Throw New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "El arnes no compone el escritor legacy.")
    End Function
End Class

Public Partial Class AdaptadorDocumentoImportacion
    Public Sub New(http As HttpContext, staging As IAlmacenStagingImportacion)
        Throw New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "El arnes no construye los puertos legacy.")
    End Sub
End Class
