## ADDED Requirements

### Requirement: AppEditor usa paginas reales como base del modo paginado
El sistema SHALL representar `AppEditor` en `paginationMode="visual"` a partir
de paginas reales dentro del modelo del editor, en lugar de depender de un
flujo continuo con correcciones posteriores como base estructural.

#### Scenario: El documento paginado se normaliza a un arbol con paginas reales
- **WHEN** `AppEditor` se inicializa en `paginationMode="visual"`
- **THEN** el documento SHALL quedar estructurado como `doc -> page -> blocks`
- **AND** los bloques editables SHALL vivir dentro de nodos `page` reales y no
  directamente sobre una sola columna continua

#### Scenario: Cada pagina define un area util real
- **WHEN** el editor renderiza una pagina con formato, orientacion y margenes
- **THEN** la hoja SHALL exponer un area util real coherente con esa
  configuracion
- **AND** el contenido SHALL medirse y distribuirse respecto a esa area util

### Requirement: La base vieja deja de ser el mecanismo principal del modo paginado
El sistema SHALL dejar de depender de `pageBreak` automaticos,
`spacerHeight`, simulacion visual de hojas y correccion posterior al
desborde como base primaria del modo paginado.

#### Scenario: El limite de pagina no nace de espaciadores legacy
- **WHEN** `AppEditor` opera en `paginationMode="visual"`
- **THEN** el sistema MUST NOT usar `data-page-break-spacer` ni
  `spacerHeight` como fuente principal de los limites entre hojas

#### Scenario: La continuidad entre hojas no se corrige despues del overflow
- **WHEN** el contenido alcanza el final del area util de una pagina
- **THEN** el sistema SHALL determinar la continuidad hacia la siguiente hoja
  desde la estructura de paginas reales
- **AND** MUST NOT esperar a que exista desborde visible para corregirlo

### Requirement: Fase 1 garantiza continuidad basica hacia la pagina siguiente
El sistema SHALL crear o reutilizar una pagina siguiente cuando el contenido
ya no cabe en la hoja actual, preservando una continuidad basica sin montar el
contenido sobre el borde inferior.

#### Scenario: Escritura al final de pagina crea continuidad en la siguiente hoja
- **WHEN** el usuario escribe en el ultimo espacio disponible de una pagina
- **THEN** el contenido nuevo SHALL continuar en la pagina siguiente
- **AND** la pagina actual SHALL mantener su borde inferior libre de overflow

#### Scenario: Un bloque que ya no cabe se mueve o continua en la siguiente hoja
- **WHEN** un bloque agregado o pegado excede la capacidad restante de la hoja
  actual
- **THEN** el editor SHALL repartirlo o moverlo a la pagina siguiente segun su
  naturaleza basica
- **AND** el usuario SHALL seguir viendo una continuidad valida entre hojas

### Requirement: La migracion y serializacion siguen siendo coherentes con paginas reales
El sistema SHALL ofrecer una ruta de migracion desde documentos existentes y
mantener una serializacion estable al introducir la nueva estructura de
paginas reales.

#### Scenario: Documento existente se migra al modelo paginado real
- **WHEN** `AppEditor` abre contenido existente proveniente del flujo continuo
- **THEN** el sistema SHALL normalizarlo a la estructura `doc -> page -> blocks`
- **AND** el contenido funcional existente SHALL conservarse sin depender de la
  metadata transitoria del motor viejo

#### Scenario: Guardado y recarga no preservan residuos del motor legacy
- **WHEN** el contenido del editor se serializa y luego se vuelve a cargar
- **THEN** el HTML SHALL permanecer libre de metadata espuria derivada de
  `data-page-break-spacer`, `spacerHeight` o equivalentes transitorios

### Requirement: La fase base mantiene compatibilidad con capacidades actuales del editor
El sistema SHALL introducir paginas reales sin romper toolbar, links,
imagenes locales, zoom, serializacion, ni el uso controlado y no controlado en
los flujos basicos de esta fase.

#### Scenario: Modo continuo sigue estable
- **WHEN** `AppEditor` se usa con `paginationMode="none"`
- **THEN** la experiencia SHALL seguir funcionando como flujo continuo
- **AND** MUST NOT imponer la estructura paginada sobre ese modo

#### Scenario: Capacidades basicas siguen operativas sobre paginas reales
- **WHEN** el usuario edita un documento basico con toolbar, links, imagenes
  locales o zoom en `paginationMode="visual"`
- **THEN** esas capacidades SHALL seguir operando sobre la nueva base de
  paginas reales sin romper la experiencia de escritura y apertura

