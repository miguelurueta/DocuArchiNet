## ADDED Requirements

### Requirement: Precondición obligatoria de AppTreeTable (API pública + wrapper AppTable)
El sistema MUST garantizar que `AppTreeTable` permanece como wrapper sobre `AppTable` internamente y expone eventos para integración enterprise sin hacks:
- `onSelectRow(rowId)`
- `onCellClicked(args)` para click principal (o equivalente del wrapper)
- `onActionTriggered(actionId, ctx)` para acciones secundarias (menú/cell/menu actions)

#### Scenario: AppTreeTable cumple precondición del prompt
- **WHEN** `DocumentosWorkbench` integra el panel documental usando `AppTreeTable`
- **THEN** `AppTreeTable` MUST renderizar internamente usando `AppTable`
- **THEN** `AppTreeTable` MUST permitir wiring de `onSelectRow`, `onCellClicked` y `onActionTriggered` sin romper otros consumidores

#### Scenario: Precondición no se cumple (bloqueante)
- **WHEN** `AppTreeTable` no expone los eventos requeridos o deja de renderizar con `AppTable` internamente
- **THEN** la implementación MUST detenerse y reportar blocker técnico
- **AND** el sistema MUST NOT introducir hacks temporales para simular esos eventos

### Requirement: DocumentosWorkbench carga listado backend-driven en AppTreeTable
El sistema MUST permitir que `DocumentosWorkbench` renderice un listado jerárquico de documentos consumiendo datos desde backend y presentándolos mediante `AppTreeTable` (wrapper sobre `AppTable`).

#### Scenario: Carga inicial exitosa
- **WHEN** `DocumentosWorkbench` inicializa el listado
- **THEN** el sistema MUST invocar `load()` para obtener las filas raíz
- **THEN** el sistema MUST renderizar las filas raíz en `AppTreeTable`

#### Scenario: Carga inicial falla
- **WHEN** `load()` responde con `{ ok: false }` o lanza error
- **THEN** `AppTreeTable` MUST mostrar un estado de error con un mensaje en español
- **THEN** el sistema MUST permitir reintentar la carga si el retry está habilitado

### Requirement: Carga incremental de hijos por nodo (lazy children)
El sistema MUST permitir carga incremental de hijos por nodo mediante `loadChildren(row)` para nodos con hijos.

#### Scenario: Expandir nodo con hijos sin children cargados
- **WHEN** el usuario expande un nodo que reporta `hasChildren=true` y aún no tiene `children` cargados
- **THEN** el sistema MUST invocar `loadChildren(row)` una sola vez para ese nodo mientras esté en progreso
- **THEN** el sistema MUST actualizar el árbol y mostrar las filas hijas bajo el nodo expandido

#### Scenario: Fallo al cargar hijos
- **WHEN** `loadChildren(row)` responde `{ ok: false }` o lanza error
- **THEN** el sistema MUST mantener el árbol consistente sin duplicar filas
- **THEN** el sistema MUST permitir que el usuario colapse/expanda sin que se rompa el render

### Requirement: Columnas backend-driven sin hardcode en cliente
El sistema MUST soportar columnas provenientes de backend (metadata) para renderizar valores de filas sin hardcodear columnas en el cliente.

#### Scenario: Backend entrega columnas
- **WHEN** el backend entrega `columns` (lista de claves/encabezados)
- **THEN** el sistema MUST pasar esas columnas a `AppTreeTable`
- **THEN** `AppTreeTable` MUST renderizar cada columna en orden, usando `values[columnKey]` cuando aplique

### Requirement: Contrato SCRUM-205 obligatorio (request/response/action)
El sistema MUST consumir `SCRUM-205 ListaDocumentosRadicados` cumpliendo los campos requeridos por el contrato y sin inventar un contrato alterno.

#### Scenario: Query request contiene campos obligatorios
- **WHEN** el sistema construye el request de query (raíz o children)
- **THEN** el payload MUST incluir: `ViewMode`, `Page`, `PageSize`, `SortDir`, `ParentRowId`, `ParentNodeType`, `Level`, `IncludeConfig`, `EnablePagination`, `EnableColumnFilters`

#### Scenario: Query response se mapea según SCRUM-205
- **WHEN** el backend retorna `Rows`
- **THEN** el adapter MUST mapear:
  - `Rows[].RowId` -> `AppTreeTableRow.id`
  - `Rows[].Values` -> `AppTreeTableRow.values`
  - `Rows[].Meta.HasChildren` -> `AppTreeTableRow.hasChildren`
  - `Rows[].Meta.NodeType/ParentId/DocumentId/NombreGabinete` -> `AppTreeTableRow.meta`

#### Scenario: Action request contiene campos obligatorios
- **WHEN** el usuario dispara una acción de backend (`ActionId`) desde el menú o click principal (`ver_documento`)
- **THEN** el request MUST incluir: `ActionId`, `RowId`, `NodeType`, `Payload.IdDocumento`, `Payload.NombreGabinete`

### Requirement: Compatibilidad SCRUM-209 (flatDocuments + label + no legacy columns)
El sistema MUST mantener compatibilidad con `SCRUM-205` pero aplicar el delta de `SCRUM-209` en el consumo frontend.

#### Scenario: Rutas y envelope no cambian (compatibilidad)
- **WHEN** el frontend consume `ListaDocumentosRadicados`
- **THEN** MUST mantenerse las rutas:
  - `POST /api/GestorDocumental/Documentos/ListaDocumentosRadicados/query`
  - `POST /api/GestorDocumental/Documentos/ListaDocumentosRadicados/action`
  - `POST /api/gestor-documental/documentos/visualizacion/resolve`
- **AND** el response MUST mantener envelope estable tipo `AppResponses<T>`: `success`, `message`, `data`, `meta`, `errors`

#### Scenario: `flatDocuments` usa reglas de request oficiales
- **WHEN** el frontend consulta `ViewMode="flatDocuments"`
- **THEN** el request SHOULD enviar `ParentRowId=null`, `ParentNodeType=null`, `Level=1`
- **AND** el frontend MUST NOT depender de columnas legacy no garantizadas en este modo

#### Scenario: `hierarchical` mantiene semántica heredada sin regresión
- **WHEN** el frontend consulta `ViewMode="hierarchical"` y el usuario expande nodos
- **THEN** el sistema MUST usar `ParentRowId` y `Level` para cargar children sin romper el árbol

#### Scenario: Label documental provisto por backend (sin recalcular fallback)
- **WHEN** el backend entrega el valor de la columna documental principal (p.ej. `TIPODOCUMENTO`)
- **THEN** la UI MUST mostrar el label tal como viene en `Values`
- **AND** la UI MUST NOT recalcular el fallback `DOC {ID}` (se asume ya resuelto por backend)

#### Scenario: `flatDocuments` no renderiza columnas irrelevantes/legacy
- **WHEN** el backend retorna `flatDocuments`
- **THEN** el adapter SHOULD limitar el render a label principal + acciones backend-driven (según `Config`/acciones)
- **AND** el cliente MUST NOT asumir presencia de columnas removidas (p.ej. `PAG`, `ESTADO_FIRMA_DIGITAL`, etc.)

#### Scenario: Seguridad (Authorization + claims) y errores controlados
- **WHEN** el frontend invoca los endpoints SCRUM-209
- **THEN** MUST enviar header `Authorization: Bearer {jwt}`
- **AND** SHOULD manejar errores esperados:
  - `400` por claims faltantes/invalidas (error controlado)
  - `200` con `success=false` (validación funcional) mostrando `message` y priorizando `errors[0].errorMessage`
  - errores técnicos controlados (sin romper pantalla)

#### Scenario: `flatDocuments` usa payload recomendado (cuando aplique al contexto pantalla)
- **WHEN** el frontend integra la vista simplificada por radicado/tarea
- **THEN** el request SHOULD permitir enviar campos opcionales frecuentes (sin romper compatibilidad):
  - `ColumnMode`, `SearchType`, `Search`, `SortField`, `StructuredFilters`, `IncludeConfig`, `TableId`, `NombreGabinete`, `Radicado`, `AplicaTrd`

### Requirement: Selección de fila notifica al consumidor
El sistema MUST notificar al consumidor cuando el usuario selecciona una fila para habilitar la integración con el visor/documento.

#### Scenario: Click en fila selecciona documento
- **WHEN** el usuario hace click en una fila (o en el label) dentro del listado
- **THEN** `AppTreeTable` MUST invocar `onSelectRow(rowId)`
- **THEN** `DocumentosWorkbench` MUST poder reaccionar (p.ej. seleccionar/visualizar el documento) sin requerir cambios en `AppTreeTable`

### Requirement: Acción primaria `ver_documento` integra visor PDF (resiliente)
El sistema MUST ejecutar `ver_documento` como acción primaria del listado y actualizar el visor PDF cuando sea exitoso.

#### Scenario: Click principal ejecuta `ver_documento` y actualiza visor
- **WHEN** el usuario hace click principal sobre una fila que representa un documento
- **THEN** el sistema MUST ejecutar `ver_documento` (vía `onSelectRow` y/o `onCellClicked`, según aplique)
- **THEN** el sistema MUST resolver `fileUrl` (vía hook/service) y actualizar `AppVisorEmbedPdf`

#### Scenario: Error en `ver_documento` no cambia documento activo
- **WHEN** `ver_documento` falla (error HTTP, `ok:false`, o `success=false`)
- **THEN** el sistema MUST NOT cambiar el documento activo del visor
- **AND** el sistema MUST mostrar el error en el panel (en español), priorizando `errors[0].errorMessage` cuando exista

#### Scenario: Sin documento activo muestra fallback
- **WHEN** no hay documento seleccionado
- **THEN** el visor MUST mostrar un estado/fallback claro (sin romper layout)

### Requirement: Menú secundario dinámico backend-driven por fila
El sistema MUST soportar acciones secundarias por fila disparadas desde un menú/cell actions backend-driven y notificarlas mediante `onActionTriggered`.

#### Scenario: Render de menú dinámico provisto por backend
- **WHEN** el backend entrega metadata de acciones (p.ej. `Config` Dynamic UI o equivalente)
- **THEN** el sistema MUST renderizar menú/acciones sin hardcode en `DocumentosWorkbench`
- **AND** el wiring MUST reutilizar el flujo existente (`client_event`/mapeos) sin duplicar lógica Dynamic UI

#### Scenario: Disparar acción secundaria ejecuta ActionId
- **WHEN** el usuario selecciona una acción secundaria del menú
- **THEN** `AppTreeTable` MUST invocar `onActionTriggered(actionId, ctx)`
- **THEN** el sistema MUST construir el action request SCRUM-205 y ejecutar la acción en backend

### Requirement: Restricciones enterprise (Clean Architecture + no regressions)
El sistema MUST cumplir guardrails arquitectónicos:
- `DocumentosWorkbench` MUST NOT consumir Axios ni DTOs backend directamente
- `services/hooks/adapters` MUST ser los responsables de HTTP y mapping
- MUST NOT romper `AppTable`, `AppTreeTable`, `AppVisorEmbedPdf`, `AppCollapseRail`, responsive ni overlay

### Requirement: Estados UX enterprise (loading/empty/error/retry) en español
El sistema MUST mostrar estados de carga y resiliencia en el panel documental.

#### Scenario: Loading visible
- **WHEN** hay una query en progreso (root o children)
- **THEN** el panel MUST mostrar un estado de loading perceptible

#### Scenario: Empty state
- **WHEN** la query retorna 0 filas
- **THEN** el panel MUST mostrar un mensaje claro en español indicando que no hay documentos

#### Scenario: Error state con retry
- **WHEN** ocurre un error
- **THEN** el panel MUST mostrar un mensaje en español y un botón de reintento visible

### Requirement: Accesibilidad/teclado y performance
El sistema SHOULD ser keyboard-friendly y mantener estabilidad visual/performance:
- focus visible
- acciones accesibles (ARIA)
- evitar re-render masivo del workbench; handlers memoizados; no jitter

### Requirement: Trazabilidad de tests a Spec
Los tests que cubren este cambio MUST incluir el tag de spec en su nombre o `describe`.

#### Scenario: Suite de tests etiquetada
- **WHEN** se agregan o actualizan tests para este cambio
- **THEN** dichos tests MUST incluir el tag `[SPEC:APPTREETABLE-217]`
