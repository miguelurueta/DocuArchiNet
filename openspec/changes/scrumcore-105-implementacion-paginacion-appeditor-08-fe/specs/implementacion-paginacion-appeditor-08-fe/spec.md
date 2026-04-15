## ADDED Requirements

### Requirement: Contador de pagina actual en AppEditor
El sistema SHALL mostrar un contador visual de pagina actual dentro de `AppEditor` cuando el modo `paginationMode="visual"` este activo, indicando `Pagina X de Y` sin alterar el contenido serializado.

#### Scenario: Documento de una sola pagina
- **WHEN** el contenido estimado ocupa una unica pagina
- **THEN** el contador SHALL mostrar `Pagina 1 de 1`

#### Scenario: Documento multipagina
- **WHEN** el contenido estimado ocupa varias paginas
- **THEN** el contador SHALL mostrar la pagina actual estimada y el total de paginas calculadas

### Requirement: Resolucion de pagina actual por cursor con fallback a scroll
El sistema SHALL determinar la pagina actual con prioridad en la posicion del cursor cuando el editor tiene foco, y usar el scroll como fallback cuando no sea posible resolverla desde la seleccion activa.

#### Scenario: Cursor activo en el editor
- **WHEN** el editor tiene foco y existe una seleccion activa valida
- **THEN** el sistema SHALL calcular la pagina actual a partir de `editor.view.coordsAtPos(selection.from)`

#### Scenario: Fallback por scroll
- **WHEN** el editor no tiene foco o no se puede resolver una coordenada valida desde la seleccion
- **THEN** el sistema SHALL calcular la pagina actual a partir del offset de scroll del contenedor paginado

### Requirement: Calculo de pagina actual basado en altura util
El sistema SHALL calcular el indice de pagina actual usando la altura util de pagina previamente determinada por las metricas de paginacion visual.

#### Scenario: Formula de pagina actual
- **WHEN** el sistema cuenta con un `offset` vertical y un `pageContentHeight`
- **THEN** SHALL resolver `pageIndex = floor(offset / pageContentHeight) + 1`

#### Scenario: Limites del contador
- **WHEN** el offset calculado cae fuera de rango
- **THEN** el sistema SHALL acotar el valor entre `1` y `totalPages`

### Requirement: Presentacion discreta del contador
El sistema SHALL renderizar el contador como un elemento visual discreto dentro del shell paginado del editor, sin competir con la toolbar ni con el contenido editable.

#### Scenario: Ubicacion del contador
- **WHEN** el contador esta visible en modo paginado
- **THEN** SHALL renderizarse en la esquina inferior derecha del shell del editor o en una posicion equivalente de baja interferencia visual

#### Scenario: Sin impacto en interaccion
- **WHEN** el usuario navega, escribe o selecciona contenido
- **THEN** el contador SHALL mantenerse visible sin bloquear foco, seleccion o scroll del editor

### Requirement: Actualizacion estable y performante
El sistema SHALL actualizar el contador de pagina actual de forma estable durante scroll, escritura y cambios de seleccion, evitando `setState` innecesario.

#### Scenario: Debounce de scroll
- **WHEN** el usuario realiza scroll continuo en el contenedor paginado
- **THEN** el sistema SHALL agrupar actualizaciones del contador con debounce o sincronizacion equivalente

#### Scenario: Sin metadata adicional en HTML
- **WHEN** `AppEditor` serializa su contenido mediante `value` y `onChange`
- **THEN** el HTML resultante SHALL permanecer libre de datos asociados al contador de pagina actual
