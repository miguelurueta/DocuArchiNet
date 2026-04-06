## ADDED Requirements

### Requirement: AppTableQueryWrapper usa botones ghost explicitos para acciones de tabla
El sistema SHALL renderizar las acciones de refrescar, pagina anterior y pagina siguiente de `AppTableQueryWrapper` mediante `AppButton` con variante `ghost`, sin depender de `AppIconActionButton` dentro de este wrapper.

#### Scenario: Accion de refrescar conserva comportamiento
- **WHEN** `AppTableQueryWrapper` recibe `onRefresh`
- **THEN** el sistema SHALL renderizar un boton accesible para actualizar la tabla que ejecute `onRefresh`, conserve tooltip, use `variant="ghost"` y refleje `loading` cuando la tabla este cargando

#### Scenario: Navegacion anterior respeta estado disponible
- **WHEN** `AppTableQueryWrapper` se renderiza en una pagina mayor a `1`
- **THEN** el sistema SHALL permitir navegar a la pagina anterior con un `AppButton` ghost que emita `onQueryChange({ page: currentPage - 1 })`

#### Scenario: Navegacion anterior se bloquea en primera pagina
- **WHEN** `AppTableQueryWrapper` se renderiza en la pagina `1`
- **THEN** el sistema MUST deshabilitar el boton de pagina anterior y MUST NOT emitir cambios de query al intentar activarlo

#### Scenario: Navegacion siguiente respeta total de paginas
- **WHEN** `AppTableQueryWrapper` se renderiza con pagina actual menor que el total de paginas calculado
- **THEN** el sistema SHALL permitir navegar a la pagina siguiente con un `AppButton` ghost que emita `onQueryChange({ page: currentPage + 1 })`

#### Scenario: Navegacion siguiente se bloquea en ultima pagina
- **WHEN** `AppTableQueryWrapper` se renderiza en la ultima pagina calculada
- **THEN** el sistema MUST deshabilitar el boton de pagina siguiente y MUST NOT emitir cambios de query al intentar activarlo

### Requirement: AppTableQueryWrapper usa AppDropdown para seleccionar tamano de pagina
El sistema SHALL renderizar el selector de tamano de pagina de `AppTableQueryWrapper` mediante `AppDropdown` con un trigger basado en `AppButton`, manteniendo las opciones configuradas por `pageSizeOptions`.

#### Scenario: Trigger muestra tamano de pagina actual
- **WHEN** `AppTableQueryWrapper` se renderiza con `queryState.pageSize = 25`
- **THEN** el sistema SHALL mostrar un trigger accesible con el texto `25 por pagina` o equivalente y SHALL asociarlo al selector de cantidad de registros por pagina

#### Scenario: Dropdown conserva opciones configuradas
- **WHEN** `AppTableQueryWrapper` recibe `pageSizeOptions` con valores numericos
- **THEN** el sistema SHALL renderizar una opcion por cada valor recibido, usando etiquetas de formato `<valor> por pagina`

#### Scenario: Seleccion de tamano actualiza query state
- **WHEN** el usuario selecciona una opcion habilitada del dropdown de tamano de pagina
- **THEN** el sistema SHALL emitir `onQueryChange({ pageSize: selectedOption })` sin exigir que el consumidor manipule eventos internos del proveedor UI

### Requirement: AppTableQueryWrapper mantiene AppInput acotado a busqueda
El sistema SHALL mantener `AppInput` como control de busqueda textual dentro de `AppTableQueryWrapper` y SHALL limitar cualquier ajuste visual especial de ese input a estilos locales del wrapper.

#### Scenario: Busqueda conserva emision de cambios
- **WHEN** el usuario escribe en el input de busqueda de `AppTableQueryWrapper`
- **THEN** el sistema SHALL emitir `onQueryChange({ search: typedValue })`

#### Scenario: Estilos ghost no se aplican globalmente a AppInput
- **WHEN** se ajuste la presentacion del input de busqueda de `AppTableQueryWrapper`
- **THEN** el sistema SHALL aplicar ese ajuste mediante `AppTableQueryWrapper.module.css` o clases locales equivalentes y MUST NOT agregar una variante global nueva a `AppInput`

#### Scenario: Wrapper sin busqueda no renderiza AppInput
- **WHEN** `AppTableQueryWrapper` recibe `showSearch=false`
- **THEN** el sistema SHALL omitir el input de busqueda sin afectar los controles de refresco, paginacion ni acciones externas

### Requirement: GestionCorrespondencia inicia paginacion con el default esperado de AppTable
El sistema SHALL iniciar la tabla de `GestionCorrespondencia` con `pageSize = 25`, manteniendo el default global de `AppTable` en `25` y sin modificar los contratos backend.

#### Scenario: Query state inicial usa veinticinco registros
- **WHEN** se inicializa el hook de tabla de `GestionCorrespondencia`
- **THEN** el sistema SHALL usar `pageSize = 25` como tamano inicial de pagina

#### Scenario: Request de GestionCorrespondencia envia PageSize veinticinco
- **WHEN** `GestionCorrespondencia` solicita datos iniciales al servicio de correspondencia
- **THEN** el sistema SHALL enviar `PageSize = 25` y SHALL conservar los filtros, ordenamiento y pagina inicial existentes

#### Scenario: Defaults globales permanecen sin cambio
- **WHEN** se implemente el refinamiento de `AppTableQueryWrapper`
- **THEN** el sistema MUST mantener `DEFAULT_APP_TABLE_CLIENT_PAGE_SIZE` y el default reusable de query state en `25`
