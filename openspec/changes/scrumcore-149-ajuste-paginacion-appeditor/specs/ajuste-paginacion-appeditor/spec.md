## ADDED Requirements

### Requirement: Paginacion visual estricta dentro de la hoja
El sistema SHALL impedir que cualquier contenido de `AppEditor` con
`paginationMode="visual"` se vea o se escriba por fuera de la hoja o de la
margen util de la pagina.

#### Scenario: Escritura al final de la hoja sin overflow visible
- **WHEN** el usuario escribe en el ultimo renglon disponible de una pagina
- **THEN** el contenido SHALL continuar en la siguiente hoja antes de que exista
  desborde visible

#### Scenario: Contenido pegado que supera el espacio restante
- **WHEN** el usuario pega contenido cuyo alto excede el espacio restante de la
  pagina actual
- **THEN** el sistema SHALL redistribuir el contenido sin mostrar overflow
  visible fuera de la hoja

### Requirement: Corte preventivo antes del desborde
El sistema SHALL decidir el punto de corte antes del desborde visible, evitando
correcciones tardias perceptibles para el usuario.

#### Scenario: El salto de pagina ocurre antes del borde inferior
- **WHEN** un bloque textual se acerca al limite inferior de la pagina
- **THEN** el motor de layout SHALL determinar el corte antes de invadir el
  borde inferior

#### Scenario: No hay flicker ni rebote visual
- **WHEN** el contenido cruza de una pagina a otra durante edicion continua
- **THEN** la transicion SHALL ocurrir sin efecto de contenido que se sale y
  luego se corrige

### Requirement: Corte estructural por tipo de contenido
El sistema SHALL aplicar reglas de corte coherentes con la estructura del
contenido en vez de usar una unica estrategia para todos los nodos.

#### Scenario: Texto se divide por lineas reales
- **WHEN** un parrafo debe continuar en la siguiente pagina
- **THEN** el sistema SHALL cortar en una posicion legible basada en layout
  real, evitando cortes arbitrarios

#### Scenario: Bloques indivisibles se mueven completos
- **WHEN** una imagen o bloque no divisible no cabe en el espacio restante
- **THEN** el sistema SHALL moverlo completo a la siguiente hoja

#### Scenario: Bloques divisibles conservan continuidad legible
- **WHEN** una lista o bloque textual puede dividirse sin romper estructura
- **THEN** el sistema SHALL mantener continuidad visual y semantica entre
  paginas

### Requirement: Continuidad correcta para listas y task lists
El sistema SHALL mantener integridad estructural y continuidad visual correcta
para `bullet list`, `ordered list` y `task list` al final de la hoja.

#### Scenario: Lista con viñetas en el limite inferior
- **WHEN** una `bullet list` alcanza el borde inferior de la pagina
- **THEN** su continuidad SHALL preservarse en la siguiente hoja sin romper
  sangria, marcador ni estructura

#### Scenario: Lista numerada en el limite inferior
- **WHEN** una `ordered list` alcanza el borde inferior de la pagina
- **THEN** la numeracion SHALL mantenerse estable y ordenada a traves del corte

#### Scenario: Task list en el limite inferior
- **WHEN** una `task list` alcanza el borde inferior de la pagina
- **THEN** el sistema SHALL preservar checkbox, estructura y continuidad visual

### Requirement: Imagenes respetan estrictamente el limite de pagina
El sistema SHALL impedir que las imagenes invadan el borde inferior o se vean
parcialmente fuera de la hoja en modo visual paginado.

#### Scenario: Imagen que no cabe en el espacio restante
- **WHEN** una imagen no cabe completa en el espacio restante de la pagina
- **THEN** el sistema SHALL moverla a la siguiente hoja antes del desborde
  visible

#### Scenario: Imagen cerca del final con zoom activo
- **WHEN** el usuario trabaja con zoom en modo visual y una imagen se aproxima
  al final de la pagina
- **THEN** el corte SHALL seguir respetando la hoja sin perder alineacion visual

### Requirement: Coherencia entre modelo de layout y estado TipTap
El sistema SHALL mantener coherencia entre el motor de layout paginado y el
estado del editor TipTap/ProseMirror.

#### Scenario: Transacciones siguen siendo consistentes
- **WHEN** el motor de paginacion ajusta el flujo entre paginas
- **THEN** el estado del editor SHALL permanecer sincronizado con el layout sin
  romper transactions ni seleccion

#### Scenario: Serializacion HTML permanece estable
- **WHEN** el contenido se guarda o se vuelve a cargar
- **THEN** el HTML SHALL seguir representando el documento sin metadata espuria
  derivada de la paginacion visual

### Requirement: Compatibilidad con zoom, contador y modo continuo
El sistema SHALL introducir la paginacion estricta sin romper las capacidades
ya existentes del editor.

#### Scenario: Zoom y page counter siguen correctos
- **WHEN** el usuario edita un documento multipagina con zoom visual activo
- **THEN** zoom, hojas y contador de paginas SHALL mantenerse alineados

#### Scenario: Modo continuo no se degrada
- **WHEN** `AppEditor` se usa en modo continuo
- **THEN** el comportamiento SHALL mantenerse estable y sin regresiones por el
  nuevo motor de paginacion visual
