Option Strict On
Imports System

Friend Class VistaRecuperacionDocumentoV2Importacion
    Public Property Operacion As VistaPersistenciaDocumentoV2Importacion
    Public Property Plan As PlanImportacionCarpetas
End Class

'Puerto interno de comprobación/recuperación: no contiene invocación legacy.
Friend NotInheritable Class RecuperacionDocumentoV2Importacion
    Friend ReadOnly Leer As Func(Of ContextoImportacionCarpetas, String, String, VistaRecuperacionDocumentoV2Importacion)
    Friend ReadOnly Guardia As Func(Of VistaRecuperacionDocumentoV2Importacion, DocumentoAlmacenamientoV2Importacion, IGuardiaOriginalV2Importacion)
    Friend ReadOnly Ausencia As Func(Of ComandoAlmacenamientoCarpetas, Boolean)
    Friend ReadOnly Comprobar As Func(Of ComandoAlmacenamientoCarpetas, ResultadoEfectoImportacion, Boolean)
    Friend ReadOnly Guardar As Func(Of VistaRecuperacionDocumentoV2Importacion, ResultadoEfectoImportacion, IGuardiaOriginalV2Importacion, ResultadoEfectoImportacion)
    Friend Sub New(lectura As Func(Of ContextoImportacionCarpetas, String, String, VistaRecuperacionDocumentoV2Importacion), guardiaOriginal As Func(Of VistaRecuperacionDocumentoV2Importacion, DocumentoAlmacenamientoV2Importacion, IGuardiaOriginalV2Importacion), sinDocumentos As Func(Of ComandoAlmacenamientoCarpetas, Boolean), verificacion As Func(Of ComandoAlmacenamientoCarpetas, ResultadoEfectoImportacion, Boolean), persistencia As Func(Of VistaRecuperacionDocumentoV2Importacion, ResultadoEfectoImportacion, IGuardiaOriginalV2Importacion, ResultadoEfectoImportacion))
        If lectura Is Nothing OrElse guardiaOriginal Is Nothing OrElse sinDocumentos Is Nothing OrElse verificacion Is Nothing OrElse persistencia Is Nothing Then Throw New ArgumentNullException()
        Leer = lectura : Guardia = guardiaOriginal : Ausencia = sinDocumentos : Comprobar = verificacion : Guardar = persistencia
    End Sub
End Class
