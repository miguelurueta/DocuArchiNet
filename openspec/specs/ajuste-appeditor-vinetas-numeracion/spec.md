## ADDED Requirements

### Requirement: Listas respetan la margen visual del editor
El sistema SHALL renderizar listas con viñetas y numeracion en `AppEditor`
sin romper la margen visual ya establecida por la superficie editable en modo
continuo y en modo `paginationMode="visual"`.

#### Scenario: Lista con viñetas dentro de la margen esperada
- **WHEN** el usuario aplica `bullet list` sobre contenido del editor
- **THEN** la lista SHALL conservar una sangria controlada y alineada con la
  margen visual del documento, sin desplazarse excesivamente hacia la derecha

#### Scenario: Lista numerada dentro de la margen esperada
- **WHEN** el usuario aplica `ordered list` sobre contenido del editor
- **THEN** la numeracion SHALL quedar alineada con la misma referencia visual
  de margen sin introducir una segunda sangria dominante

### Requirement: Items multilinea mantienen legibilidad estable
El sistema SHALL conservar legibilidad correcta para items de lista que ocupen
mas de una linea.

#### Scenario: Item multilinea con viñeta
- **WHEN** un item de `ul` se extiende a multiples lineas
- **THEN** el texto de continuidad SHALL alinearse de forma estable respecto al
  marcador y al bloque del item

#### Scenario: Item multilinea con numeracion
- **WHEN** un item de `ol` se extiende a multiples lineas
- **THEN** la continuidad del texto SHALL mantenerse legible y sin colisiones
  con el marcador numerado

### Requirement: Simplificacion del DOM sin romper estructura necesaria
El sistema SHALL remover el wrapper intermedio redundante del contenido de
`AppEditor` sin romper la estructura necesaria del modo continuo ni la del modo
visual paginado.

#### Scenario: Modo continuo sin wrapper intermedio redundante
- **WHEN** `AppEditor` se renderiza en modo continuo
- **THEN** `EditorContent` SHALL colgar directamente de la estructura principal
  del frame sin una capa visual intermedia redundante

#### Scenario: Modo visual conserva capas necesarias
- **WHEN** `AppEditor` se renderiza con `paginationMode="visual"`
- **THEN** la estructura SHALL preservar `editorWrapper`, `canvas`, `sheet` y
  `contentFlow`, removiendo solo la capa intermedia no necesaria entre
  `contentFlow` y `EditorContent`

### Requirement: Compatibilidad con paginacion, zoom y contador
El sistema SHALL introducir el ajuste visual de listas y la simplificacion del
DOM sin romper la experiencia de paginacion visual ya existente.

#### Scenario: Zoom y hojas siguen alineados
- **WHEN** el usuario usa zoom en modo visual sobre un documento con listas
- **THEN** hojas, contenido y overlays SHALL permanecer alineados

#### Scenario: Contador de pagina sigue estable
- **WHEN** el documento contiene listas y el usuario navega en modo visual
- **THEN** `Pagina X de Y` SHALL seguir calculandose de forma coherente

### Requirement: Sin regresion funcional del editor
El sistema SHALL mantener estables foco, scroll, toolbar y serializacion HTML
tras el ajuste de listas y layout.

#### Scenario: Toolbar y foco estables
- **WHEN** el usuario aplica viñetas o numeracion desde la toolbar
- **THEN** el editor SHALL mantener una experiencia estable sin perder foco ni
  recrear la instancia funcional del editor

#### Scenario: HTML sin metadata espuria del layout
- **WHEN** el contenido del editor se serializa tras usar listas
- **THEN** el HTML SHALL seguir representando listas estandares sin metadata
  adicional derivada del ajuste visual del wrapper o de la sangria
