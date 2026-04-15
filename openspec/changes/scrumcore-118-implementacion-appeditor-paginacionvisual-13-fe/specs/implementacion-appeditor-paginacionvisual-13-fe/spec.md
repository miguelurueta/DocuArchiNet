## ADDED Requirements

### Requirement: Hojas visuales reales en modo paginado
El sistema SHALL reemplazar la representacion basada en lineas guia por hojas visuales reales tipo A4 cuando `AppEditor` se renderice con `paginationMode="visual"`.

#### Scenario: Hoja A4 visible
- **WHEN** `AppEditor` usa `paginationMode="visual"`
- **THEN** el usuario SHALL percibir una hoja delimitada visualmente con fondo blanco, separacion respecto al workspace y presencia de bordes/sombra sutil

#### Scenario: Sin impacto en modo continuo
- **WHEN** `AppEditor` se renderiza con `paginationMode="none"`
- **THEN** el componente SHALL mantener el comportamiento visual continuo existente

### Requirement: Margenes visuales por los cuatro lados
El sistema SHALL representar margenes visuales claros en los cuatro lados de la hoja sin modificar la estructura persistida del documento.

#### Scenario: Caja util de documento
- **WHEN** el editor se muestra en modo paginado visual
- **THEN** el contenido SHALL percibirse dentro de una caja util interna con margenes top, right, bottom y left claramente visibles

#### Scenario: Sin padding estructural sobre ProseMirror
- **WHEN** el layout de hoja se aplica
- **THEN** los margenes SHALL resolverse como parte del layout visual del contenedor y no mediante cambios persistentes sobre el HTML serializado

### Requirement: Separacion limpia entre hojas
El sistema SHALL mostrar separacion vertical clara entre hojas sin introducir lineas visibles que atraviesen el texto o el contenido editable.

#### Scenario: Sin cruce visual sobre texto
- **WHEN** el documento supera una pagina visual
- **THEN** la separacion entre hojas SHALL percibirse como espacio limpio entre paginas y no como una linea que cruza el contenido

#### Scenario: Workspace diferenciado
- **WHEN** el usuario observa el area externa del documento
- **THEN** el sistema SHALL mostrar un workspace visualmente distinto de la hoja

### Requirement: Compatibilidad con calculo de paginacion existente
El sistema SHALL mantener la logica interna actual de metricas y paginacion como base de calculo, aunque las guias dejen de ser visibles.

#### Scenario: Guias internas no visibles
- **WHEN** el modo de hojas reales esta activo
- **THEN** las guias de pagina ya no se renderizaran como lineas visibles, pero la logica de `usePaginationMetrics` SHALL seguir alimentando la paginacion

#### Scenario: Compatibilidad con contador
- **WHEN** el contador de pagina calcula `Pagina X de Y`
- **THEN** SHALL seguir funcionando correctamente con la nueva representacion visual de hojas

### Requirement: Compatibilidad con capacidades actuales del editor
El sistema SHALL introducir hojas reales y margenes visibles sin romper toolbar, zoom, `PageBreak`, imagenes ni el modelo controlled/uncontrolled del editor.

#### Scenario: Compatibilidad con `PageBreak`
- **WHEN** existen saltos de pagina manuales en el documento
- **THEN** la nueva representacion visual SHALL seguir siendo compatible con ellos

#### Scenario: Compatibilidad con imagenes y zoom
- **WHEN** el documento contiene imagenes con `data-width` y `data-align`, o cuando se aplique zoom visual
- **THEN** la nueva visualizacion SHALL mantener compatibilidad con esas capacidades sin alterar el HTML persistido

### Requirement: Documento unico y continuo internamente
El sistema SHALL mantener una unica instancia de editor Tiptap y un documento continuo, aun cuando la representacion visual cambie a hojas reales.

#### Scenario: Sin multiples editores
- **WHEN** el modo paginado visual se renderiza
- **THEN** el componente SHALL mantener una unica instancia de `.ProseMirror`

#### Scenario: Sin mutacion del HTML persistido
- **WHEN** el usuario edita y serializa el documento
- **THEN** el HTML persistido SHALL permanecer semanticamente igual, sin nodos extra o cambios estructurales introducidos por la visualizacion de hojas
