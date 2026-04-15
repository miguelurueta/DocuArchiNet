## ADDED Requirements

### Requirement: Medicion del contenido paginado en AppEditor
El sistema SHALL medir la altura renderizada del contenido de `AppEditor` en modo `paginationMode="visual"` usando la superficie editable de ProseMirror, sin fragmentar el documento ni modificar su HTML serializado.

#### Scenario: Medicion basada en scrollHeight
- **WHEN** `AppEditor` se renderiza con `paginationMode="visual"`
- **THEN** el sistema SHALL usar `scrollHeight` de `.ProseMirror` como base para calcular la altura renderizada del contenido

#### Scenario: Sin medicion en modo continuo
- **WHEN** `AppEditor` se renderiza sin activar `paginationMode="visual"`
- **THEN** el sistema SHALL omitir la logica de metricas de paginacion y mantener el comportamiento actual del editor

### Requirement: Calculo de paginas estimadas a partir del area util
El sistema SHALL calcular la altura util de pagina y el numero total de paginas estimadas en funcion del formato, orientacion y margenes configurados para el modo visual.

#### Scenario: Altura util por pagina
- **WHEN** se define `pageHeight` y margenes `top` y `bottom`
- **THEN** el sistema SHALL calcular `pageContentHeight = pageHeight - (top + bottom)`

#### Scenario: Total de paginas estimadas
- **WHEN** el contenido medido supera una pagina util
- **THEN** el sistema SHALL calcular `totalPages = ceil(contentHeight / pageContentHeight)`

### Requirement: Guias visuales de pagina desacopladas del documento
El sistema SHALL dibujar guias visuales de pagina mediante un overlay absoluto fuera de `ProseMirror`, preservando el documento como flujo continuo editable.

#### Scenario: Overlay no interactivo
- **WHEN** `AppEditor` dibuja guias visuales de pagina
- **THEN** las guias SHALL renderizarse en una capa absoluta con `pointer-events: none`

#### Scenario: Guias alineadas al calculo de pagina
- **WHEN** el sistema detecta que el contenido ocupa multiples paginas estimadas
- **THEN** SHALL renderizar una guia visual por cada limite de pagina calculado

### Requirement: Recalculo estable por cambios de contenido y layout
El sistema SHALL recalcular metricas de paginacion cuando cambie el contenido o el tamaño del contenedor, usando estrategias de sincronizacion estables para evitar jitter visual.

#### Scenario: Recalculo por escritura o cambio del layout
- **WHEN** cambia el contenido del editor o el tamaño del contenedor paginado
- **THEN** el sistema SHALL recalcular metricas y guias usando `requestAnimationFrame`, `useLayoutEffect` o un mecanismo equivalente de sincronizacion visual

#### Scenario: Debounce de medicion
- **WHEN** se producen multiples cambios en secuencia corta
- **THEN** el sistema SHALL agrupar mediciones con debounce entre `16ms` y `50ms` para evitar trabajo excesivo en cada `keypress`

### Requirement: Integracion sin regresion sobre edicion y serializacion
El sistema SHALL mantener intacta la experiencia de edicion, toolbar y serializacion del contenido mientras incorpora metricas y guias visuales de pagina.

#### Scenario: Edicion sin ruptura
- **WHEN** el usuario escribe, selecciona contenido, usa la toolbar o inserta imagenes en modo visual
- **THEN** el editor SHALL seguir funcionando como documento continuo sin perdida de foco ni bloqueo de interaccion

#### Scenario: HTML sin metadata de guias
- **WHEN** `AppEditor` serializa contenido mediante `value` y `onChange`
- **THEN** el HTML resultante SHALL permanecer libre de nodos o atributos relacionados con las guias visuales de pagina
