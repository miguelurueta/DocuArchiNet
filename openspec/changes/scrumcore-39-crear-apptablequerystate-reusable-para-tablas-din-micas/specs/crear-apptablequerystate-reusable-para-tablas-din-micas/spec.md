## ADDED Requirements

### Requirement: AppTableQueryState centraliza el estado reusable de consulta
El sistema SHALL exponer un contrato compartido `AppTableQueryState` en la capa de `AppTable` para modelar `page`, `pageSize`, `search`, `searchType`, `structuredFilters`, `sortField` y `sortDir` sin acoplarlo a una pantalla específica.

#### Scenario: Estado inicial reusable con defaults
- **WHEN** un consumidor solicita el estado inicial de `AppTableQueryState`
- **THEN** el sistema devuelve un objeto con `page = 1`, `pageSize = 25`, `search = ""`, `structuredFilters = []`, `sortField = undefined`, `sortDir = undefined` y `searchType = undefined`

#### Scenario: Contrato estructurado compatible con backend
- **WHEN** un consumidor define filtros estructurados para `AppTableQueryState`
- **THEN** el sistema acepta un contrato tipado que soporta `field`, `operator`, `value`, `valueFrom` y `valueTo`, incluyendo operadores como `between`, `isNull` e `isNotNull`

### Requirement: Las reglas de reset de pagina se encapsulan en la infraestructura reusable
El sistema SHALL resolver los resets de `page` dentro del helper compartido de actualización de `AppTableQueryState`, sin delegar esa responsabilidad a componentes visuales o módulos consumidores.

#### Scenario: Cambio efectivo de búsqueda o filtros reinicia pagina
- **WHEN** cambia efectivamente alguno de estos campos: `search`, `searchType`, `structuredFilters`, `sortField`, `sortDir` o `pageSize`
- **THEN** el sistema devuelve un nuevo `AppTableQueryState` con `page = 1`

#### Scenario: Cambio solo de pagina no altera el resto del estado
- **WHEN** el consumidor actualiza únicamente `page`
- **THEN** el sistema conserva `search`, `searchType`, `structuredFilters`, `sortField`, `sortDir` y `pageSize` sin modificaciones adicionales

#### Scenario: Cambio por referencia sin cambio efectivo no reinicia pagina
- **WHEN** el consumidor envía una actualización con arrays u objetos cuyo contenido efectivo no cambió
- **THEN** el sistema no ejecuta un reset espurio de `page`

### Requirement: La serialización del query state hacia backend es única y reusable
El sistema SHALL centralizar en un único helper reusable la transformación de `AppTableQueryState` al shape consumido por el query layer backend-compatible.

#### Scenario: Serialización compatible con consulta server
- **WHEN** un consumidor serializa un `AppTableQueryState`
- **THEN** el sistema produce un payload reutilizable que incluye `page`, `pageSize`, `search`, `searchType`, `structuredFilters`, `sortField` y `sortDir` sin requerir mappers manuales por pantalla

#### Scenario: Serialización de filtro between preserva ambos extremos
- **WHEN** `structuredFilters` contiene un operador `between`
- **THEN** la serialización preserva `valueFrom` y `valueTo` como parte del contrato del filtro

### Requirement: El hook compartido reutiliza helpers puros sin mezclar fetch ni refresh
El sistema SHALL exponer `useAppTableQueryState` como un hook ligero de ergonomía React que reutiliza los helpers compartidos y no incorpora lógica de backend, fetch ni refresh.

#### Scenario: Hook inicializa el estado con un override parcial
- **WHEN** un consumidor invoca `useAppTableQueryState` con un estado inicial parcial
- **THEN** el hook compone ese override sobre los defaults del contrato compartido

#### Scenario: Refresh no forma parte del query state
- **WHEN** un flujo de UI ejecuta una acción de refresh externa
- **THEN** el `AppTableQueryState` permanece sin cambios salvo que exista un patch explícito sobre sus campos
