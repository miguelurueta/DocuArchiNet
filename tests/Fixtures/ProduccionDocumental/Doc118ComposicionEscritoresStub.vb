Option Strict On
Imports System.Web

'El dispatcher HTTP real se compila. Solo los dos escritores legacy quedan
'fuera de este arnés; recuperación/reconciliación usan su composición real.
'No incluir este archivo en el proyecto productivo.
Public NotInheritable Class ComposicionCreacionImportacion
    Public Shared Function Crear(http As HttpContext) As CoordinadorImportacionCarpetas
        Throw New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "El arnes no compone el escritor legacy.")
    End Function
End Class

Public NotInheritable Class ComposicionDocumentoImportacion
    Public Shared Function Crear(http As HttpContext) As CoordinadorImportacionCarpetas
        Throw New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "El arnes no compone el escritor legacy.")
    End Function
End Class
