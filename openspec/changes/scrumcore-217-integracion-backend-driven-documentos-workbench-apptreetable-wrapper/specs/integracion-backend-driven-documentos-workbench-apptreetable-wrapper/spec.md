## ADDED Requirements

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

### Requirement: Selección de fila notifica al consumidor
El sistema MUST notificar al consumidor cuando el usuario selecciona una fila para habilitar la integración con el visor/documento.

#### Scenario: Click en fila selecciona documento
- **WHEN** el usuario hace click en una fila (o en el label) dentro del listado
- **THEN** `AppTreeTable` MUST invocar `onSelectRow(rowId)`
- **THEN** `DocumentosWorkbench` MUST poder reaccionar (p.ej. seleccionar/visualizar el documento) sin requerir cambios en `AppTreeTable`

### Requirement: Trazabilidad de tests a Spec
Los tests que cubren este cambio MUST incluir el tag de spec en su nombre o `describe`.

#### Scenario: Suite de tests etiquetada
- **WHEN** se agregan o actualizan tests para este cambio
- **THEN** dichos tests MUST incluir el tag `[SPEC:APPTREETABLE-217]`
