# gestion-correspondencia Specification

## Purpose

Definir la estructura del modulo Gestion Correspondencia dentro del dashboard, incluyendo layout, paginas base, routing anidado, busqueda desacoplada y un shell persistente para vistas secundarias contextuales.

## ADDED Requirements

### Requirement: El modulo Gestion Correspondencia SHALL exponer una estructura base desacoplada
El sistema SHALL incorporar un modulo `gestionCorrespondencia` dentro de `src/modules/` con separacion explicita entre `layout`, `pages`, `routes` y documentacion, de modo que la estructura inicial pueda crecer sin mezclar navegacion, composicion visual y futuras reglas del dominio.

#### Scenario: Estructura minima del modulo disponible
- **WHEN** el repositorio integra el modulo Gestion Correspondencia
- **THEN** el sistema SHALL incluir `GestionCorrespondenciaLayout`, `GestionCorrespondencia`, `GestionRespuesta`, `GestionCorrespondenciaRoute` y `README.md` dentro del arbol del modulo

#### Scenario: Capas con responsabilidades separadas
- **WHEN** un desarrollador revisa la estructura del modulo
- **THEN** el layout SHALL contener solo estructura compartida, las pages SHALL contener composicion visual y la capa routes SHALL orquestar la navegacion del modulo

### Requirement: El modulo Gestion Correspondencia SHALL integrarse como ruta hija del dashboard
El sistema SHALL registrar Gestion Correspondencia como una ruta hija del arbol protegido bajo `/dashboard`, reutilizando `DashboardLayout` y el patron de rutas anidadas actual sin crear un shell paralelo ni romper la navegacion existente.

#### Scenario: Acceso a la ruta base del modulo
- **WHEN** un usuario autenticado navega a la ruta configurada del modulo bajo `/dashboard`
- **THEN** el sistema SHALL renderizar el contenido base de Gestion Correspondencia dentro del `Outlet` del dashboard

#### Scenario: Integracion sin alterar otras rutas del dashboard
- **WHEN** el nuevo modulo se agrega al arbol de rutas
- **THEN** las rutas existentes del dashboard SHALL seguir resolviendose sin cambios de comportamiento atribuibles al modulo Gestion Correspondencia

### Requirement: El layout del modulo SHALL renderizar encabezado, contenedor principal y Outlet
El sistema SHALL implementar `GestionCorrespondenciaLayout` como shell visual agnostico al negocio, usando componentes de Ant Design para presentar el titulo del modulo, descripcion contextual, contenedor de contenido y un `Outlet` para las rutas hijas.

#### Scenario: Render del layout base
- **WHEN** la ruta del modulo se renderiza por primera vez
- **THEN** el usuario SHALL ver un encabezado del modulo, una descripcion y un contenedor principal preparados para alojar la pagina principal y vistas hijas

#### Scenario: Layout sin logica de negocio
- **WHEN** el layout se usa como contenedor del modulo
- **THEN** su responsabilidad SHALL limitarse a la estructura comun y no SHALL depender de llamadas API, reglas funcionales ni estado de negocio del dominio

### Requirement: La pagina principal SHALL mantener contexto visual y placeholders profesionales
El sistema SHALL mostrar `GestionCorrespondencia` como pagina principal del modulo con contenido inicial corporativo, placeholders utiles y jerarquia visual consistente, evitando una pantalla vacia y sirviendo como punto de entrada para futuras iteraciones.

#### Scenario: Vista principal del modulo
- **WHEN** el usuario entra a la ruta base de Gestion Correspondencia
- **THEN** el sistema SHALL renderizar una pagina principal con contenido base visible, secciones informativas y placeholders preparados para evolucionar

#### Scenario: Sin integracion real de negocio
- **WHEN** la pagina principal se renderiza en esta iteracion inicial
- **THEN** el sistema SHALL presentar solo estructura y contenido placeholder sin ejecutar integraciones backend ni acciones funcionales del dominio

### Requirement: El modulo SHALL usar un shell de navegacion persistente controlado por routing
El sistema SHALL implementar `GestionCorrespondenciaRoute` como un shell de navegacion tipo Gmail para el modulo, manteniendo visible la vista principal y renderizando las vistas secundarias dentro de una region persistente del layout gobernada por la URL y no por un overlay modal acoplado a estado local.

#### Scenario: Ruta secundaria abre la region persistente
- **WHEN** el usuario navega a una subruta secundaria configurada del modulo
- **THEN** el sistema SHALL conservar visible la pagina principal y SHALL renderizar la vista secundaria dentro de una region persistente del shell del modulo

#### Scenario: Cierre navega a la ruta base
- **WHEN** el usuario cierra la vista secundaria desde la UI del modulo
- **THEN** el sistema SHALL navegar a la ruta base del modulo y SHALL ocultar la region secundaria sin desmontar el contexto principal

#### Scenario: Deep link a la vista secundaria
- **WHEN** el usuario entra directamente a la URL de la subruta secundaria
- **THEN** el sistema SHALL resolver la ruta, mostrar la vista principal y renderizar la vista secundaria dentro del shell persistente del modulo

#### Scenario: Comportamiento responsivo del shell
- **WHEN** el modulo se renderiza en pantallas reducidas
- **THEN** el sistema SHALL preservar la navegacion gobernada por routing y SHALL adaptar la region secundaria sin romper la experiencia principal del listado

#### Scenario: Retorno visible desde el shell secundario
- **WHEN** la vista secundaria esta activa dentro del shell
- **THEN** el sistema SHALL exponer una accion de retorno o cierre claramente visible y consistente con el contexto master-detail del modulo

#### Scenario: El shell secundario no se presenta como dialog modal
- **WHEN** la subruta secundaria del modulo esta activa
- **THEN** el sistema MUST renderizar una region secundaria observable del shell y MUST NOT depender de `role="dialog"` ni del patron `Drawer`

#### Scenario: La bandeja principal permanece montada bajo el panel secundario
- **WHEN** la vista secundaria esta activa en el shell del modulo
- **THEN** la region principal del listado MUST permanecer montada y visible como contexto base del patron master-detail

#### Scenario: La subruta secundaria incluye el identificador del registro
- **WHEN** el usuario abre el detalle contextual desde una accion de fila
- **THEN** la navegacion MUST resolverse mediante una ruta tipo `respuesta/:id` usando el identificador del registro seleccionado

### Requirement: GestionRespuesta SHALL renderizarse como vista secundaria desacoplada dentro del shell del modulo
El sistema SHALL implementar `GestionRespuesta` como una pagina secundaria preparada para mostrarse dentro de la region persistente del shell de `GestionCorrespondencia`, con estructura visual profesional y sin conocimiento directo del mecanismo de routing o de logica de negocio.

#### Scenario: Render de la vista secundaria
- **WHEN** la ruta secundaria del modulo esta activa
- **THEN** el sistema SHALL mostrar el contenido de `GestionRespuesta` dentro de la region secundaria del shell con titulo, descripcion y placeholders visibles

#### Scenario: Vista secundaria sin control de navegacion
- **WHEN** `GestionRespuesta` se renderiza como contenido del shell
- **THEN** la pagina SHALL depender de la capa routes para apertura, cierre y navegacion, y no SHALL gestionar directamente el flujo de rutas

#### Scenario: Retorno contextual sin acoplar la pagina al router
- **WHEN** el usuario interactua con el flujo de retorno visible desde la experiencia secundaria
- **THEN** `GestionRespuesta` SHALL seguir desacoplada del router y la resolucion de navegacion SHALL permanecer en la capa del shell del modulo

#### Scenario: La vista secundaria mantiene copy contextual consistente
- **WHEN** `GestionRespuesta` se renderiza dentro del shell
- **THEN** el contenido visible MUST reforzar que se trata de una respuesta contextual dentro de la bandeja y no de una navegacion modal independiente

### Requirement: El modulo SHALL incluir pruebas de comportamiento del flujo estructural
El sistema SHALL cubrir con Vitest y Testing Library el render del shell, la presencia de la pagina principal y la integracion entre rutas anidadas y la region secundaria persistente, enfocandose en comportamiento observable del modulo.

#### Scenario: Cobertura del layout y pagina principal
- **WHEN** se ejecutan las pruebas del modulo
- **THEN** el conjunto de tests SHALL verificar que el layout y la pagina principal renderizan sin errores con el contenido base esperado

#### Scenario: Cobertura del shell gobernado por rutas
- **WHEN** se ejecutan las pruebas sobre la ruta secundaria del modulo
- **THEN** el conjunto de tests SHALL verificar que la region secundaria se abre por routing y que `GestionRespuesta` se renderiza sin reemplazar la pagina principal

#### Scenario: Cobertura del retorno visible
- **WHEN** se ejecutan las pruebas del flujo secundario del modulo
- **THEN** el conjunto de tests SHALL verificar que el usuario dispone de una accion observable de retorno/cierre y que esta lo devuelve a la ruta base sin desmontar la bandeja principal antes del cambio de ruta

#### Scenario: Cobertura contra regresion a Drawer o modal
- **WHEN** se ejecutan las pruebas del shell secundario
- **THEN** la suite MUST detectar una regresion si la implementacion vuelve a un `Drawer`, a un `dialog` modal o a un reemplazo total de la region principal

#### Scenario: Cobertura de deep link con shell completo
- **WHEN** se ejecutan las pruebas entrando directamente a la subruta secundaria
- **THEN** la suite MUST verificar simultaneamente shell, region principal y region secundaria observables en la misma navegacion

#### Scenario: La accion de toolbar ya no abre el detalle contextual
- **WHEN** `GestionCorrespondencia` renderiza las acciones principales del toolbar
- **THEN** el flujo de apertura de `GestionRespuesta` MUST depender de la accion contextual de fila y MUST NOT exponerse tambien como boton global redundante

### Requirement: Gestion Correspondencia adopta affordance navegable reusable de AppTable

The system SHALL adopt the shared `AppTable` row click affordance contract in `GestionCorrespondencia`, replacing the module-local CSS implementation without changing the current navigation flow.

#### Scenario: GestionCorrespondencia activates shared affordance contract
- **WHEN** `GestionCorrespondencia` renders its main table
- **THEN** it MUST pass `rowClickAffordance` to `AppTable`
- **AND** it MUST preserve `onCellClicked` and `onActionTriggered` as the module-owned navigation layer

#### Scenario: Module-local navigable grid class is removed
- **WHEN** the shared affordance contract is active in `GestionCorrespondencia`
- **THEN** the page MUST NOT depend on `gridClassName={styles.navigableGrid}` for navigable cell affordance

#### Scenario: Local affordance CSS is removed
- **WHEN** `GestionCorrespondencia` adopts the shared affordance contract
- **THEN** `GestionCorrespondencia.module.css` MUST NOT keep cursor, hover, or equivalent visual rules for navigable table cells

#### Scenario: Data-cell navigation remains unchanged
- **WHEN** the user clicks a navigable data cell in `GestionCorrespondencia`
- **THEN** the module MUST keep navigating to `respuesta/:id` through its existing cell-click flow

#### Scenario: Actions column still does not trigger accidental navigation
- **WHEN** the user clicks the `acciones` column cell surface in `GestionCorrespondencia`
- **THEN** the module MUST NOT navigate through the generic cell-click flow
- **AND** the contextual action menu MUST remain usable

#### Scenario: Selection behavior remains unchanged
- **WHEN** the user interacts with row selection in `GestionCorrespondencia`
- **THEN** the module MUST preserve the existing selection behavior and MUST NOT add navigable affordance to the selection column

#### Scenario: Keyboard support comes from AppTable
- **WHEN** the user uses `Enter` on a navigable data cell in `GestionCorrespondencia`
- **THEN** the module MUST preserve the expected row action behavior through the shared `AppTable` implementation
- **AND** `GestionCorrespondencia` MUST NOT implement a second keyboard-specific navigation layer

### Requirement: El modulo SHALL documentar su arquitectura inicial
El sistema SHALL incluir un `README.md` dentro del modulo que documente el proposito de Gestion Correspondencia, su estructura de carpetas, la responsabilidad de cada capa y el flujo de shell persistente gobernado por routing previsto para futuras ampliaciones.

#### Scenario: Documentacion disponible para futuras iteraciones
- **WHEN** un desarrollador consulte el modulo por primera vez
- **THEN** el `README.md` SHALL describir el flujo de navegacion, las capas del modulo y la forma recomendada de escalar la implementacion
## Requirements
### Requirement: Gestion Correspondencia toolbar search
The system SHALL render `AppInputSearch` inside `AppToolbar.actionContent` in `GestionCorrespondencia` and keep the table wrapper search disabled to avoid duplicate search controls.

#### Scenario: Search is rendered in toolbar
- **WHEN** `GestionCorrespondencia` renders with a table query state
- **THEN** the toolbar MUST include exactly one visible `AppInputSearch` with accessible name `Buscar tareas workflow`

#### Scenario: Search uses a controlled visible value
- **WHEN** the toolbar search renders with autocomplete enabled
- **THEN** its visible value MUST be controlled by the page composition while preserving the applied table search value through `table.queryState.search`

#### Scenario: Typing updates autocomplete text without applying table search
- **WHEN** the user types in the toolbar search
- **THEN** the page MUST update the autocomplete search text through `useWorkflowInboxAutocomplete.setSearchText(value)` and MUST NOT call backend services directly from the page

#### Scenario: Manual search applies table query
- **WHEN** the user confirms a free text search with Enter or the search icon
- **THEN** the page MUST call `table.onQueryChange({ search: value })` and the request mapper MUST continue applying the simple search mapping contract

#### Scenario: Search clear updates query state
- **WHEN** the user clears the toolbar search
- **THEN** the page MUST clear autocomplete suggestions and update the search through `table.onQueryChange({ search: "" })` without adding a parallel reset flow in the page

#### Scenario: Wrapper search remains disabled
- **WHEN** the toolbar search is present in `GestionCorrespondencia`
- **THEN** `AppTableQueryWrapper` MUST be rendered with `showSearch={false}`

#### Scenario: Existing toolbar actions remain available
- **WHEN** the toolbar search is present
- **THEN** the existing refresh and contextual response actions MUST remain rendered and usable

#### Scenario: Export and pagination remain delegated
- **WHEN** the toolbar search is present
- **THEN** export and pagination MUST remain delegated to the existing `AppTableQueryWrapper` and related table controls without changing their public contracts

#### Scenario: Toolbar search styling remains local
- **WHEN** `GestionCorrespondencia` applies layout styles to the toolbar search
- **THEN** the styles MUST be scoped to the module CSS and MUST NOT alter the internal semantics, focus behavior, states, or accessibility of `AppInputSearch`

### Requirement: Gestion Correspondencia search request mapping
The system SHALL map effective simple search text from `GestionCorrespondencia` to `SearchType = 2` in the module request mapper without changing shared `AppTable` request mapping.

#### Scenario: Effective simple search uses LIKE search type
- **WHEN** `mapGestionCorrespondenciaTableRequest` receives input with `search` whose trimmed length is greater than zero and no advanced search override
- **THEN** the mapped request MUST include the trimmed `Search` value and `SearchType = 2`

#### Scenario: Empty search does not force LIKE
- **WHEN** `mapGestionCorrespondenciaTableRequest` receives empty or whitespace-only `search`
- **THEN** the mapped request MUST NOT force `SearchType = 2`

#### Scenario: Advanced search type is preserved
- **WHEN** `mapGestionCorrespondenciaTableRequest` receives `searchType = 3`
- **THEN** the mapped request MUST preserve `SearchType = 3`

#### Scenario: Explicit non-advanced search type without text is preserved
- **WHEN** `mapGestionCorrespondenciaTableRequest` receives an explicit `searchType` different from `3` and no effective search text
- **THEN** the mapped request MUST preserve the explicit `SearchType` instead of forcing `SearchType = 2`

#### Scenario: Pagination and filters are preserved
- **WHEN** `mapGestionCorrespondenciaTableRequest` maps search input with page, page size, sort, include config, and structured filters
- **THEN** the mapped request MUST preserve `Page`, `PageSize`, `SortField`, `SortDir`, `IncludeConfig`, and `StructuredFilters`

#### Scenario: Shared mapper remains generic
- **WHEN** other tables use the shared dynamic UI request mapper
- **THEN** they MUST NOT receive `SearchType = 2` automatically because of this Gestion Correspondencia behavior

#### Scenario: All matching rows reuse module mapper
- **WHEN** `GestionCorrespondencia` requests all matching rows after a simple search
- **THEN** the request MUST be built through `mapGestionCorrespondenciaTableRequest` and MUST preserve the active `Search` and `SearchType`

#### Scenario: Backend export reuses module mapper
- **WHEN** `GestionCorrespondencia` requests a backend export after a simple search
- **THEN** the export request MUST be built through `mapGestionCorrespondenciaTableRequest` and MUST preserve the active `Search` and `SearchType`

### Requirement: Gestion Correspondencia workflow inbox autocomplete
The system SHALL provide a frontend autocomplete layer for Workflow Inbox suggestions through a domain hook and service while keeping `AppInputSearch` presentational.

#### Scenario: Autocomplete hook does not query below minLength
- **WHEN** `useWorkflowInboxAutocomplete` receives search text shorter than its configured `minLength`
- **THEN** it MUST NOT call the backend service and MUST expose an empty `items` array

#### Scenario: Autocomplete hook queries with search and limit
- **WHEN** `useWorkflowInboxAutocomplete` receives search text that satisfies `minLength`
- **THEN** it MUST call the autocomplete service with the current `search` text and the configured `limit`

#### Scenario: Autocomplete hook exposes loading and items
- **WHEN** a suggestion request is in progress and then resolves
- **THEN** the hook MUST expose `loading = true` during the request and MUST expose mapped `items` with only `value` and optional `label` after success

#### Scenario: Autocomplete hook handles errors without breaking table search
- **WHEN** the autocomplete service rejects
- **THEN** the hook MUST expose an `error`, MUST NOT throw to the component tree, and MUST keep free text table search usable

#### Scenario: Autocomplete hook ignores obsolete responses
- **WHEN** an older request resolves after a newer request has already been issued
- **THEN** the hook MUST NOT let the older response overwrite the current `items`

#### Scenario: Autocomplete debounce is centralized in the hook
- **WHEN** `GestionCorrespondencia` integrates autocomplete with `AppInputSearch`
- **THEN** suggestion debounce MUST live in `useWorkflowInboxAutocomplete` and `AppInputSearch` MUST be configured without its own typing debounce for this flow

#### Scenario: AppInputSearch receives presentational autocomplete inputs
- **WHEN** `GestionCorrespondencia` renders the toolbar search with autocomplete
- **THEN** it MUST pass only `options`, `loading`, value, and callbacks to `AppInputSearch`, and MUST NOT pass endpoint URLs, DTOs, or service functions

#### Scenario: Selecting suggestion applies table search
- **WHEN** the user selects an autocomplete suggestion
- **THEN** `AppInputSearch` MUST emit the selected value and `GestionCorrespondencia` MUST apply it through `table.onQueryChange({ search: selectedValue })`

#### Scenario: Free text search works without suggestions
- **WHEN** there are no autocomplete suggestions and the user confirms free text search
- **THEN** `GestionCorrespondencia` MUST still apply the table search through `table.onQueryChange({ search: value })`

#### Scenario: Autocomplete service adapts backend response
- **WHEN** the backend autocomplete response includes fields beyond `value` and `label`
- **THEN** `workflowInboxAutocomplete.service` MUST adapt the response to the hook contract without leaking backend-specific fields into `AppInputSearch.options`

#### Scenario: Autocomplete does not alter AppTable contracts
- **WHEN** autocomplete is enabled in `GestionCorrespondencia`
- **THEN** export, pagination, selection, `AppTableQueryWrapper`, and `AppTable` public contracts MUST remain unchanged
