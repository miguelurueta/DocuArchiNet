## ADDED Requirements

### Requirement: Panel de miniaturas colapsable
El sistema SHALL permitir ocultar y mostrar el panel de Miniaturas desde el toolbar de digitalizacion documental.

#### Scenario: Ocultar miniaturas
- **GIVEN** el workspace de digitalizacion esta activo
- **WHEN** el usuario ejecuta `Ocultar Miniaturas`
- **THEN** la columna de miniaturas se contrae a `0`
- **AND** el Preview PDF se expande usando el espacio disponible
- **AND** el panel de miniaturas permanece montado para preservar paginas, seleccion, drag and drop y scroll

#### Scenario: Mostrar miniaturas
- **GIVEN** el panel de miniaturas esta oculto
- **WHEN** el usuario ejecuta `Mostrar Miniaturas`
- **THEN** la columna de miniaturas vuelve a estar visible
- **AND** el estado previo de paginas y seleccion se conserva

### Requirement: Panel de configuracion colapsable
El sistema SHALL permitir ocultar y mostrar el panel de Configuracion de escaneo desde el toolbar de digitalizacion documental.

#### Scenario: Ocultar configuracion
- **GIVEN** el workspace de digitalizacion esta activo
- **WHEN** el usuario ejecuta `Ocultar Configuracion`
- **THEN** la columna de configuracion se contrae a `0`
- **AND** el Preview PDF se expande usando el espacio disponible
- **AND** los valores de configuracion de captura se conservan en memoria

#### Scenario: Mostrar configuracion
- **GIVEN** el panel de configuracion esta oculto
- **WHEN** el usuario ejecuta `Mostrar Configuracion`
- **THEN** la columna de configuracion vuelve a estar visible
- **AND** los valores seleccionados de scanner, modo, color, resolucion y procesamiento siguen disponibles

### Requirement: Preview PDF responsivo
El sistema SHALL expandir automaticamente el Preview PDF cuando uno o ambos paneles laterales estan ocultos.

#### Scenario: Ambos paneles ocultos
- **GIVEN** el panel de miniaturas esta oculto
- **AND** el panel de configuracion esta oculto
- **WHEN** se renderiza el layout
- **THEN** el Preview PDF ocupa el ancho disponible entre ambas columnas colapsadas

### Requirement: Persistencia de paneles
El sistema SHALL persistir `showThumbnails` y `showConfiguration` en `localStorage`.

#### Scenario: Restaurar preferencias
- **GIVEN** existen preferencias guardadas para paneles de digitalizacion
- **WHEN** el workspace se monta
- **THEN** el layout inicial respeta `showThumbnails` y `showConfiguration`

#### Scenario: Storage no disponible
- **GIVEN** `localStorage` falla o contiene datos corruptos
- **WHEN** el workspace se monta o actualiza preferencias
- **THEN** la UI sigue operando sin bloquear el flujo
- **AND** ambos paneles visibles son el estado por defecto

### Requirement: No regresion scanner y miniaturas
El sistema SHALL alternar paneles sin reinicializar scanner, recargar miniaturas ni limpiar paginas capturadas.

#### Scenario: Toggle sin operaciones scanner
- **GIVEN** el workspace esta montado
- **WHEN** el usuario oculta o muestra paneles laterales
- **THEN** no se ejecutan operaciones de captura, limpieza, dispose o generacion PDF por el toggle
