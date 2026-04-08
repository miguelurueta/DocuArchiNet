# gestion-correspondencia Specification

## Purpose

Definir la estructura inicial del modulo Gestion Correspondencia dentro del dashboard, incluyendo layout, paginas base, routing anidado y el patron `Outlet + Drawer` para vistas secundarias contextuales.

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

### Requirement: El modulo SHALL usar un patron Outlet + Drawer controlado por routing
El sistema SHALL implementar `GestionCorrespondenciaRoute` para mantener visible la pagina principal mientras una ruta hija secundaria se renderiza dentro de un `Drawer` contextual, de forma que la apertura y cierre del overlay dependan de la URL y no solo de estado local.

#### Scenario: Ruta secundaria abre el Drawer
- **WHEN** el usuario navega a la subruta secundaria configurada del modulo
- **THEN** el sistema SHALL conservar visible la pagina principal y SHALL abrir un `Drawer` con el contenido de la vista secundaria

#### Scenario: Cierre del Drawer vuelve a la ruta base
- **WHEN** el usuario cierra el `Drawer` desde la UI del modulo
- **THEN** el sistema SHALL navegar a la ruta base del modulo y SHALL ocultar la vista secundaria sin desmontar el contexto principal

#### Scenario: Deep link a la vista secundaria
- **WHEN** el usuario entra directamente a la URL de la subruta secundaria
- **THEN** el sistema SHALL resolver la ruta, mostrar la pagina principal de fondo y renderizar la vista secundaria dentro del `Drawer`

### Requirement: GestionRespuesta SHALL renderizarse como vista secundaria desacoplada
El sistema SHALL implementar `GestionRespuesta` como una pagina secundaria preparada para mostrarse dentro del `Drawer`, con estructura visual profesional y sin conocimiento directo del mecanismo de routing o de logica de negocio.

#### Scenario: Render de la vista secundaria
- **WHEN** la ruta secundaria del modulo esta activa
- **THEN** el sistema SHALL mostrar el contenido de `GestionRespuesta` dentro del `Drawer` con titulo, descripcion y placeholders visibles

#### Scenario: Vista secundaria sin control de navegacion
- **WHEN** `GestionRespuesta` se renderiza como contenido del `Drawer`
- **THEN** la pagina SHALL depender de la capa routes para apertura, cierre y navegacion, y no SHALL gestionar directamente el flujo de rutas

### Requirement: El modulo SHALL incluir pruebas de comportamiento del flujo estructural
El sistema SHALL cubrir con Vitest y Testing Library el render del layout, la presencia de la pagina principal y la integracion entre rutas anidadas y `Drawer`, enfocandose en comportamiento observable del modulo.

#### Scenario: Cobertura del layout y pagina principal
- **WHEN** se ejecutan las pruebas del modulo
- **THEN** el conjunto de tests SHALL verificar que el layout y la pagina principal renderizan sin errores con el contenido base esperado

#### Scenario: Cobertura del Drawer gobernado por rutas
- **WHEN** se ejecutan las pruebas sobre la ruta secundaria del modulo
- **THEN** el conjunto de tests SHALL verificar que el `Drawer` se abre por routing y que `GestionRespuesta` se renderiza dentro del overlay sin reemplazar la pagina principal

### Requirement: El modulo SHALL documentar su arquitectura inicial
El sistema SHALL incluir un `README.md` dentro del modulo que documente el proposito de Gestion Correspondencia, su estructura de carpetas, la responsabilidad de cada capa y el flujo `Outlet + Drawer` previsto para futuras ampliaciones.

#### Scenario: Documentacion disponible para futuras iteraciones
- **WHEN** un desarrollador consulte el modulo por primera vez
- **THEN** el `README.md` SHALL describir el flujo de navegacion, las capas del modulo y la forma recomendada de escalar la implementacion
## Requirements
### Requirement: Gestion Correspondencia toolbar search
The system SHALL render `AppInputSearch` inside `AppToolbar.actionContent` in `GestionCorrespondencia` and keep the table wrapper search disabled to avoid duplicate search controls.

#### Scenario: Search is rendered in toolbar
- **WHEN** `GestionCorrespondencia` renders with a table query state
- **THEN** the toolbar MUST include exactly one visible `AppInputSearch` with accessible name `Buscar tareas workflow`

#### Scenario: Search uses query state
- **WHEN** the toolbar search renders
- **THEN** its value MUST come from `table.queryState.search`

#### Scenario: Search changes update query state
- **WHEN** the user types in the toolbar search
- **THEN** the page MUST call `table.onQueryChange({ search: value })` and MUST NOT call backend services directly

#### Scenario: Search clear updates query state
- **WHEN** the user clears the toolbar search
- **THEN** the page MUST update the search through `table.onQueryChange({ search: "" })` without adding a parallel reset flow in the page

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

#### Scenario: Pagination and filters are preserved
- **WHEN** `mapGestionCorrespondenciaTableRequest` maps search input with page, page size, sort, include config, and structured filters
- **THEN** the mapped request MUST preserve `Page`, `PageSize`, `SortField`, `SortDir`, `IncludeConfig`, and `StructuredFilters`

#### Scenario: Shared mapper remains generic
- **WHEN** other tables use the shared dynamic UI request mapper
- **THEN** they MUST NOT receive `SearchType = 2` automatically because of this Gestion Correspondencia behavior

