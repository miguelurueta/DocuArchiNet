## ADDED Requirements

### Requirement: Adaptador de tabla soporta shape legacy y shape actual
El sistema SHALL interpretar la respuesta de `query` aceptando ambos formatos de `DynamicUiTableDto`: `data.Config` (legacy) y `data` directo (actual), sin degradar la inferencia actual de columnas ni el mapeo de documentos planos.

#### Scenario: Configuracion en data.Config
- **WHEN** la respuesta de `query` contiene `data.Config` como objeto de tabla valido
- **THEN** el adaptador utiliza `data.Config` como fuente de `DynamicUiTableDto` y genera `columns` y `flatDocuments` equivalentes al comportamiento esperado

#### Scenario: Configuracion en data directo
- **WHEN** la respuesta de `query` contiene `Columns` y `Rows` directamente en `data`
- **THEN** el adaptador utiliza `data` como `DynamicUiTableDto` y genera `columns` y `flatDocuments` sin requerir `data.Config`

### Requirement: Flattening conserva acciones para render y disparo
El sistema SHALL preservar en `flatDocuments` las acciones requeridas por la tabla de UI, priorizando acciones de `CellActions` y `MenuActions`, con compatibilidad para `RowActions` como fallback.

#### Scenario: RowActions vacio con acciones en celda/menu
- **WHEN** una fila tiene `RowActions` vacio y acciones disponibles en `CellActions` o `MenuActions`
- **THEN** el documento plano conserva dichas acciones para que el menu de fila se renderice y permita disparar `onActionTriggered`

#### Scenario: Compatibilidad legacy de acciones
- **WHEN** una fila solo incluye acciones en `RowActions`
- **THEN** el documento plano mantiene esas acciones y el comportamiento existente permanece compatible

### Requirement: Hook mantiene TableId efectivo para query y action
El sistema SHALL mantener un `TableId` efectivo en el hook de documentos usando el `tableId` del modelo cuando llegue en respuesta y el fallback `InboxListaDocumentosRadicado` cuando no llegue.

#### Scenario: TableId dinamico en respuesta
- **WHEN** `load()` procesa una respuesta que incluye `model.tableId`
- **THEN** el hook actualiza `tableIdRef` con ese valor antes de ejecutar acciones posteriores

#### Scenario: TableId ausente en respuesta
- **WHEN** `load()` procesa una respuesta sin `model.tableId`
- **THEN** el hook conserva como `TableId` efectivo el fallback `InboxListaDocumentosRadicado`

### Requirement: Contrato de action usa identificadores y payload normalizados
El sistema MUST construir el request de `action` con `TableId` efectivo, `RowId` y payload de documento normalizado, priorizando `IdDocumento` sobre `DocumentId` cuando ambos existan y preservando `NombreGabinete` cuando esté disponible.

#### Scenario: Prioridad de IdDocumento
- **WHEN** la fila seleccionada contiene simultaneamente `IdDocumento` y `DocumentId`
- **THEN** el payload de `action` envia `IdDocumento` como identificador principal

#### Scenario: Fallback a DocumentId
- **WHEN** la fila seleccionada no contiene `IdDocumento` pero si `DocumentId`
- **THEN** el payload de `action` envia `DocumentId` para ejecutar la accion

#### Scenario: Conservacion de NombreGabinete
- **WHEN** la fila seleccionada contiene `NombreGabinete`
- **THEN** el payload de `action` incluye `NombreGabinete` junto al identificador del documento

### Requirement: Query mantiene solicitud de configuracion
El sistema MUST mantener `IncludeConfig: true` en el root de la solicitud `query` para asegurar que la configuracion de tabla siga disponible en todos los entornos compatibles.

#### Scenario: Construccion de request query
- **WHEN** el cliente construye el body de `POST /ListaDocumentosRadicados/query`
- **THEN** el root del request contiene `IncludeConfig: true`
