Option Strict On
Imports System.Web

'Solo arnes sintetico; las pruebas deben inyectar el coordinador explicitamente.
'No se incluye en el vbproj productivo ni sustituye una llamada legacy real.
Public Partial Class WebServiceImportacionCarpetas
    Private Shared Function CrearCoordinacionOperativa(http As HttpContext, autoridad As ImportacionCarpetasContextAdapter, accion As String) As CoordinadorImportacionCarpetas
        Throw New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "El arnes no compone efectos productivos.")
    End Function
End Class
