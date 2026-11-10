Option Strict On
Imports System

'Puerto adicional interno. Los proveedores/dobles V1 no adquieren capacidad V2.
Friend Interface IAlmacenStagingV2Importacion
    Function ConservarOriginalV2(contexto As ContextoImportacionCarpetas) As IRetencionOriginalV2Importacion
    Function AbrirGuardiaV2(contexto As ContextoImportacionCarpetas, documento As DocumentoAlmacenamientoV2Importacion) As IGuardiaOriginalV2Importacion
End Interface

Friend Interface IRetencionOriginalV2Importacion
    Inherits IDisposable
    ReadOnly Property Original As OriginalAlmacenamientoV2Importacion
    Sub Validar()
End Interface

Friend Interface IGuardiaOriginalV2Importacion
    Inherits IRetencionOriginalV2Importacion
    'El futuro consumidor mantiene commit/rollback dentro de esta sección.
    'El puerto no decide recuperación ni conoce una transacción de datos.
    Sub ConservarDurante(operacion As Action)
End Interface
