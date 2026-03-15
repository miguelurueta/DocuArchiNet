## ADDED Requirements

### Requirement: AppDataTableMui abstrae la tabla estandar del proyecto sobre MUI
El sistema SHALL exponer un componente `AppDataTableMui` reusable en `src/app/Components/UI` como wrapper tipado sobre `@mui/x-data-grid`, de forma que las vistas consumidoras no dependan directamente del proveedor para tablas de datos, listados administrativos o grillas operativas.

#### Scenario: Vista consume la tabla desde la capa UI
- **WHEN** una vista necesita mostrar una lista tabular de registros
- **THEN** la implementacion SHALL poder importar `AppDataTableMui` desde la capa UI compartida sin acoplarse a `DataGrid` directamente

### Requirement: AppDataTableMui normaliza columnas, filas y estados comunes
El sistema SHALL permitir configurar columnas, filas, carga, estado vacio y seleccion mediante una API propia del proyecto, preservando consistencia visual y conductual sin exponer toda la superficie del proveedor UI.

#### Scenario: Tabla con filas y columnas definidas
- **WHEN** una vista renderiza `AppDataTableMui` con `rows` y `columns`
- **THEN** el componente SHALL presentar la grilla con encabezados y celdas segun el contrato configurado

#### Scenario: Estado de carga visible
- **WHEN** una vista marca `AppDataTableMui` con `loading=true`
- **THEN** el componente SHALL mostrar feedback visual de carga sin romper la estructura de la tabla

#### Scenario: Estado vacio sin registros
- **WHEN** una vista renderiza `AppDataTableMui` sin filas
- **THEN** el componente SHALL mostrar un estado vacio estandar del proyecto o el texto configurado para ausencia de resultados

#### Scenario: Seleccion de filas controlada
- **WHEN** una vista habilita seleccion y provee callback de cambio
- **THEN** el componente SHALL propagar el modelo de seleccion resultante sin exponer a la vista a detalles internos del proveedor

### Requirement: AppDataTableMui soporta paginacion y layout reutilizable
El sistema SHALL soportar paginacion, tamanos de pagina y ajuste de altura mediante una API estable del proyecto para facilitar reutilizacion en vistas administrativas y modulos operativos.

#### Scenario: Paginacion inicial configurable
- **WHEN** una vista define tamano de pagina inicial u opciones de pagina en `AppDataTableMui`
- **THEN** el componente SHALL inicializar la tabla con esa configuracion y permitir navegacion de paginas segun el control base

#### Scenario: Altura adaptable de la grilla
- **WHEN** una vista requiere que la tabla ocupe el alto disponible o una altura minima consistente
- **THEN** el componente SHALL mantener un contenedor estable sin colapsar visualmente

### Requirement: AppDataTableMui conserva accesibilidad y composicion segura
El sistema SHALL mantener semantica accesible de grilla, soporte de teclado, labels configurables y composicion segura con renderers de celdas o columnas definidos por la vista.

#### Scenario: Tabla accesible con etiqueta visible o programatica
- **WHEN** una vista renderiza `AppDataTableMui` con identificacion accesible
- **THEN** el componente SHALL exponer nombre accesible compatible con lectores de pantalla

#### Scenario: Columnas con render personalizado
- **WHEN** una vista define renderers o formato personalizado en las columnas
- **THEN** el componente SHALL permitir esa composicion sin romper el contrato base del wrapper
