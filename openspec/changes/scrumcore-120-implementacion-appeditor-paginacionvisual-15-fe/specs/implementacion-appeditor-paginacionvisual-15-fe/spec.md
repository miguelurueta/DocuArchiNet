## ADDED Requirements

### Requirement: Estabilidad de cursor y seleccion en modo multi-hoja
El sistema SHALL mantener una experiencia estable de cursor y seleccion cuando `AppEditor` opere en `paginationMode="visual"` con multiples hojas visuales.

#### Scenario: Cursor estable tras recalculo visual
- **WHEN** el editor recalcula su segmentacion multi-hoja mientras el usuario escribe o navega
- **THEN** el cursor SHALL mantener una posicion coherente sin saltar inesperadamente entre paginas

#### Scenario: Seleccion continua entre limites de pagina
- **WHEN** el usuario crea o ajusta una seleccion que cruza limites visuales de pagina
- **THEN** la seleccion SHALL seguir funcionando correctamente sin romperse por la capa visual de paginacion

### Requirement: Scroll continuo y navegacion robusta en canvas paginado
El sistema SHALL mantener un scroll continuo, predecible y libre de saltos bruscos en el `canvas` del modo multi-hoja.

#### Scenario: Scroll fluido en documento multipagina
- **WHEN** el usuario navega verticalmente por un documento distribuido en varias hojas
- **THEN** el scroll SHALL mantenerse continuo y estable sin jitter ni desplazamientos inesperados

#### Scenario: `scrollIntoView` coherente con layout paginado
- **WHEN** una accion interna del editor requiera llevar el cursor o la seleccion al viewport visible
- **THEN** el sistema SHALL respetar el offset real del `canvas` paginado sin desalinearse por la segmentacion visual o el zoom

### Requirement: Calculo estable de pagina actual
El sistema SHALL calcular `Pagina X de Y` de forma estable a partir del contexto real del editor multi-hoja.

#### Scenario: Prioridad coherente entre cursor y scroll
- **WHEN** existan simultaneamente una posicion de cursor valida y una posicion reciente de scroll
- **THEN** el sistema SHALL resolver la pagina actual con una prioridad consistente y sin jitter visible

#### Scenario: Contador estable durante edicion
- **WHEN** el usuario edita contenido en un documento multipagina
- **THEN** el contador de pagina SHALL permanecer coherente con la hoja visual activa

### Requirement: Compatibilidad plena con PageBreak e imagenes
El sistema SHALL mantener compatibilidad completa del modo multi-hoja con `PageBreak`, imagenes redimensionables, alineacion horizontal e imagenes locales/remotas.

#### Scenario: PageBreak no rompe navegacion
- **WHEN** el usuario cruza un `PageBreak` manual durante edicion o scroll
- **THEN** el sistema SHALL mantener consistencia de scroll, calculo de pagina y seleccion

#### Scenario: Imagenes no rompen la experiencia multi-hoja
- **WHEN** el documento contiene imagenes con `data-width`, `data-align`, origen local o remoto
- **THEN** la experiencia de seleccion, foco y segmentacion visual SHALL mantenerse estable

### Requirement: Recalculo coherente con zoom visual
El sistema SHALL recalcular metricas y comportamiento multi-hoja correctamente cuando cambie `zoomLevel`.

#### Scenario: Cambio de zoom sin desalineacion
- **WHEN** el usuario modifica el zoom visual del editor
- **THEN** las paginas visuales, el scroll, la seleccion y el contador SHALL seguir alineados con el contenido renderizado

#### Scenario: Zoom sin regresion de interaccion
- **WHEN** el documento se edita tras un cambio de zoom
- **THEN** el sistema SHALL mantener una interaccion consistente sin romper cursor ni seleccion

### Requirement: Performance y ausencia de flicker critico
El sistema SHALL evitar recalculos y repaints innecesarios que degraden la experiencia del modo multi-hoja.

#### Scenario: Recalculo acotado ante cambios de layout
- **WHEN** cambie el contenido, el viewport o una metrica relevante del documento
- **THEN** el sistema SHALL recalcular solo lo necesario con una estrategia que evite reflows excesivos

#### Scenario: Sin parpadeo visual critico
- **WHEN** el usuario interactua con documentos largos en modo multi-hoja
- **THEN** la segmentacion visual SHALL evitar flicker critico o reposicionamientos erraticos del contenido
