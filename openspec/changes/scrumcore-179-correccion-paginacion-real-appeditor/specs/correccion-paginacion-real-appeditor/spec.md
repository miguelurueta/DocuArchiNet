## ADDED Requirements

### Requirement: AppEditor usa paginas reales como base del modo paginado
El sistema SHALL representar `AppEditor` en modo paginado a partir de paginas
reales dentro del modelo del editor, en lugar de depender de un flujo continuo
con `pageBreak` automaticos y espaciadores visuales como mecanismo principal.

#### Scenario: El documento paginado se estructura por hojas reales
- **WHEN** `AppEditor` se renderiza con `paginationMode="visual"`
- **THEN** el contenido SHALL vivir dentro de una estructura real de paginas y
  no en una sola columna continua corregida posteriormente con separadores
  visuales

#### Scenario: La paginacion ya no depende del spacer automatico
- **WHEN** el modo paginado esta activo
- **THEN** el sistema MUST NOT depender de `data-page-break-spacer` ni de
  `spacerHeight` como base primaria para representar el salto entre hojas

### Requirement: El contenido respeta el area util de cada hoja
El sistema SHALL impedir que texto, listas o imagenes invadan visual o
estructuralmente el borde inferior de la hoja al usar paginas reales.

#### Scenario: Escritura al final de pagina continua en la siguiente hoja
- **WHEN** el usuario escribe al final del area util de una pagina
- **THEN** el contenido nuevo SHALL continuar en la siguiente pagina sin quedar
  montado sobre el borde inferior de la hoja actual

#### Scenario: Pegado multiparrafo se distribuye por hojas reales
- **WHEN** el usuario pega contenido que excede la capacidad de la hoja actual
- **THEN** el sistema SHALL repartir el contenido resultante entre paginas
  reales sin mostrar overflow visible dentro de la hoja activa

### Requirement: Los parrafos pueden continuar entre paginas sin corromperse
El sistema SHALL permitir que un parrafo crezca, se parta y continúe en la
pagina siguiente conservando su continuidad logica y sin duplicar ni perder
texto.

#### Scenario: Edicion de un parrafo ya partido
- **WHEN** el usuario modifica un parrafo cuya continuacion ya esta en la
  pagina siguiente
- **THEN** el editor SHALL recomponer ese parrafo y redistribuir su contenido
  entre paginas manteniendo una sola continuidad logica del bloque

#### Scenario: Borrado trae contenido hacia arriba
- **WHEN** el usuario elimina contenido de un parrafo o bloque en una pagina
- **THEN** el sistema SHALL volver a ocupar el espacio disponible trayendo
  contenido desde la pagina siguiente cuando corresponda

### Requirement: Los bloques indivisibles se mueven completos a la siguiente hoja
El sistema SHALL tratar imagenes y otros bloques no divisibles como unidades
atomicas dentro del reflow de paginas reales.

#### Scenario: Imagen que no cabe en la hoja actual
- **WHEN** una imagen o bloque indivisible ya no cabe dentro del espacio
  restante de la pagina actual
- **THEN** el sistema SHALL mover el bloque completo a la siguiente pagina en
  lugar de cortarlo visualmente

#### Scenario: Bloque grande preserva integridad visual
- **WHEN** un bloque indivisible participa en el flujo multipagina
- **THEN** el editor SHALL mantener su integridad visual y estructural durante
  el reflow

### Requirement: La transicion a paginas reales no rompe capacidades existentes
El sistema SHALL introducir la nueva base de paginacion sin romper toolbar,
links, imagenes locales, zoom, modo controlado, modo continuo ni
serializacion HTML del editor.

#### Scenario: Modo continuo sigue intacto
- **WHEN** `AppEditor` se usa con `paginationMode="none"`
- **THEN** la experiencia SHALL seguir funcionando como flujo continuo sin
  imponer la estructura de paginas reales sobre ese modo

#### Scenario: Zoom y contador siguen siendo compatibles
- **WHEN** el usuario usa zoom o consulta el contexto de pagina en modo
  paginado
- **THEN** el sistema SHALL mantener esos comportamientos alineados con la nueva
  estructura de hojas reales

#### Scenario: HTML serializado no conserva metadata espuria del motor viejo
- **WHEN** el contenido del editor se guarda o vuelve a cargarse
- **THEN** el HTML SHALL permanecer libre de metadata transitoria heredada del
  mecanismo anterior de `pageBreak` automaticos y espaciadores visuales
