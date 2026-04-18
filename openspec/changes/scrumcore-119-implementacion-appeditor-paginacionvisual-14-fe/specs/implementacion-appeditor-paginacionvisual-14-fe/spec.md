## ADDED Requirements

### Requirement: Segmentacion visual automatica por area util de pagina
El sistema SHALL segmentar visualmente el contenido de `AppEditor` por hojas cuando `paginationMode="visual"` este activo, respetando el area util derivada de `pageHeight - top - bottom` sin dividir internamente el documento de Tiptap.

#### Scenario: Documento multipagina segmentado visualmente
- **WHEN** el contenido renderizado supera el alto util de la hoja
- **THEN** el sistema SHALL percibir el documento distribuido hoja a hoja con cortes visuales coherentes

#### Scenario: Documento corto permanece en una sola hoja
- **WHEN** el contenido renderizado cabe dentro del alto util de una pagina
- **THEN** el sistema SHALL mantener una unica hoja visual sin introducir cortes adicionales

### Requirement: Calculo de paginas basado en medicion DOM acumulada
El sistema SHALL medir el contenido renderizado dentro de `.ProseMirror` y calcular un modelo de segmentacion visual por acumulacion de alturas, sin clonar ni mover nodos del documento.

#### Scenario: Segmentacion por bloques renderizados
- **WHEN** se recalculan las metricas del editor
- **THEN** el sistema SHALL usar medicion DOM de bloques renderizados para determinar puntos de corte visual

#### Scenario: Recalculo eficiente de metricas
- **WHEN** cambie el tamano del contenido, del contenedor o del viewport relevante
- **THEN** el sistema SHALL recalcular la segmentacion usando mecanismos de observacion y sincronizacion visual que eviten trabajo excesivo en cada input

### Requirement: PageBreak como corte forzado de nueva hoja
El sistema SHALL integrar `PageBreak` manual como punto de corte obligatorio dentro del modelo de segmentacion visual.

#### Scenario: Salto manual fuerza nueva pagina
- **WHEN** el documento contiene un nodo `PageBreak`
- **THEN** el sistema SHALL iniciar una nueva hoja visual a partir de ese punto aunque aun exista espacio util restante en la pagina anterior

#### Scenario: Reinicio de acumulacion tras PageBreak
- **WHEN** el algoritmo encuentra un `PageBreak`
- **THEN** el calculo de altura acumulada SHALL reiniciarse para la nueva hoja

### Requirement: Documento unico y continuo internamente
El sistema SHALL mantener una unica instancia de `ProseMirror` y un documento continuo aunque la presentacion visual se distribuya en multiples hojas.

#### Scenario: Sin multiples editores
- **WHEN** el usuario interactua con el editor en modo visual multipagina
- **THEN** el sistema SHALL conservar una sola instancia editable de `ProseMirror`

#### Scenario: Sin cambios en HTML persistido
- **WHEN** el contenido se serializa mediante `value` y `onChange`
- **THEN** el sistema SHALL mantener el HTML persistido sin nodos de pagina adicionales ni cambios estructurales provocados por la segmentacion visual

### Requirement: Compatibilidad con zoom, contador y scroll continuo
El sistema SHALL mantener compatibilidad con `zoomLevel`, contador de pagina actual y scroll continuo del canvas al introducir segmentacion visual por hojas.

#### Scenario: Recalculo coherente con zoom
- **WHEN** el usuario cambia el zoom visual del editor
- **THEN** el sistema SHALL recalcular la segmentacion y las metricas de pagina sin desalinear la experiencia visual

#### Scenario: Contador coherente con segmentacion
- **WHEN** el usuario navega o edita un documento multipagina
- **THEN** el contador `Pagina X de Y` SHALL seguir reflejando el contexto de pagina correcto

#### Scenario: Scroll continuo preservado
- **WHEN** el documento se distribuye en varias hojas visuales
- **THEN** el sistema SHALL mantener un unico scroll continuo dentro del `canvas`

### Requirement: Manejo resiliente de contenido grande y capacidades existentes
El sistema SHALL introducir segmentacion visual sin romper imagenes, seleccion, undo/redo, toolbar ni los modos controlled/uncontrolled de `AppEditor`.

#### Scenario: Imagen mayor al alto util
- **WHEN** una imagen renderizada excede el alto util de la pagina
- **THEN** el sistema SHALL mantener la imagen integra aunque desborde visualmente la hoja sin romper el editor

#### Scenario: Regresion cero sobre capacidades existentes
- **WHEN** el editor contiene imagenes con `data-width` y `data-align`, usa toolbar, o se integra en modo controlled/uncontrolled
- **THEN** la segmentacion visual SHALL mantener compatibilidad con esas capacidades sin romper cursor, seleccion ni undo/redo
