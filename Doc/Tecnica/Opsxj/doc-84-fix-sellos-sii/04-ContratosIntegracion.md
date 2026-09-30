# FIX-SELLOS-SII

- Ticket: DOC-84
- Cambio OpenSpec: doc-84-fix-sellos-sii
- Clasificacion: cross_cutting (Transversal)
## Contratos e integraciones

`ImportItemResultDto` incorpora `WorkflowProjection` con `CabinetName`, `DocumentId`, `Radicado`, `StorageType`, `DocumentTypeName`, `TaskId`, `SignatureStatus` e `IconClass`. Es aditivo y no cambia autenticación, endpoints ni esquema. `EnlaseProjection` permanece separado. La frontera JavaScript serializa el contrato delimitado solo al invocar el inserter existente.
