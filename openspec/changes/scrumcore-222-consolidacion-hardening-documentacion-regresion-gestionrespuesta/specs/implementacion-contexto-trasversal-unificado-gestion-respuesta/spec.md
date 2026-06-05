## MODIFIED Requirements

### Requirement: Contexto documental transversal estable y acotado
El sistema SHALL mantener `GestionRespuestaDocumentosContext` como estado transversal documental, sin ampliar responsabilidades a UI local.

#### Scenario: Estabilidad de contrato de contexto
- **GIVEN** un consumidor existente usa `files` y `setFiles`
- **WHEN** se ejecuta cualquier validación de regresión del ticket
- **THEN** la firma y comportamiento de `files`/`setFiles` permanecen sin cambios funcionales
- **AND** no se introducen estados de UI no transversales en el proveedor

#### Scenario: Estado transversal consistente
- **GIVEN** cambios de render y recarga normal
- **WHEN** se actualizan estados transversales (`idTareaWf`, `radicado`, `idRespuestaRadicado`, `nombreGabinete`)
- **THEN** el proveedor actualiza únicamente estado documental y efectos asociados
- **AND** no rompe la integración de adjuntos ni de acciones actuales.

### Requirement: Fallback seguro de gabinete
- **WHEN** falla la resolución de gabinete o no existe `idTareaWf`
- **THEN** se mantiene render estable
- **AND** `gabineteError` reporta error explícito sin bloquear el árbol/visor
- **AND** `gabineteLoading` vuelve a `false` en estado final.
