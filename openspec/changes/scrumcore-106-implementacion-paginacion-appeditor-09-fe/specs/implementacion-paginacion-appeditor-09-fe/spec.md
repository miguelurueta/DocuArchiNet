## ADDED Requirements

### Requirement: Salto de pagina manual persistido en AppEditor
El sistema SHALL permitir insertar un salto de pagina manual persistido dentro de `AppEditor` cuando el editor opere en modo de paginacion visual, sin convertir el documento en una estructura de paginas automaticas.

#### Scenario: Insercion manual de salto de pagina
- **WHEN** el usuario ejecuta el comando `editor.commands.insertPageBreak()`
- **THEN** el sistema SHALL insertar un nodo `PageBreak` en una posicion valida del documento

#### Scenario: Persistencia del salto en el contenido
- **WHEN** el editor serializa el contenido a HTML
- **THEN** el salto manual SHALL persistirse como un elemento reconocible y rehidratable

### Requirement: Nodo PageBreak atomico y aislado
El sistema SHALL modelar el salto manual como un nodo de bloque atomico, seleccionable e isolating para evitar que el contenido editable se mezcle con la representacion visual del corte.

#### Scenario: Contrato estructural del nodo
- **WHEN** la extension `PageBreak` se registra en Tiptap
- **THEN** el nodo SHALL declararse como `block`, `atom: true`, `selectable: true` e `isolating: true`

#### Scenario: Interaccion alrededor del nodo
- **WHEN** el cursor navega antes o despues del `PageBreak`
- **THEN** la escritura SHALL seguir siendo posible sin bloquear la seleccion ni el flujo del documento

### Requirement: Representacion HTML estable del salto de pagina
El sistema SHALL serializar y parsear el salto manual usando una representacion HTML estable basada en `data-page-break`.

#### Scenario: Serializacion HTML
- **WHEN** el contenido con un `PageBreak` se convierte a HTML
- **THEN** el sistema SHALL emitir `<div data-page-break="true"></div>` o una estructura equivalente compatible con rehidratacion

#### Scenario: Parsing de HTML persistido
- **WHEN** el editor recibe contenido HTML que incluye `data-page-break="true"`
- **THEN** el sistema SHALL mapearlo nuevamente al nodo `PageBreak`

### Requirement: Render visual no editable del PageBreak
El sistema SHALL renderizar el `PageBreak` como una separacion visual clara dentro del editor, distinta del contenido editable y no modificable directamente como texto.

#### Scenario: Apariencia visual del salto
- **WHEN** el `PageBreak` esta presente en el documento
- **THEN** SHALL mostrarse como una linea horizontal con separacion visual entre bloques

#### Scenario: Nodo no editable
- **WHEN** el usuario interactua con el `PageBreak`
- **THEN** el sistema SHALL impedir la edicion textual directa del nodo mientras conserva su seleccion y navegacion

### Requirement: Insercion valida sin duplicados consecutivos
El sistema SHALL evitar la insercion de multiples `PageBreak` consecutivos y restringir su colocacion a posiciones validas del documento.

#### Scenario: Prevencion de saltos duplicados
- **WHEN** el usuario intenta insertar un `PageBreak` inmediatamente adyacente a otro `PageBreak`
- **THEN** el sistema SHALL rechazar la insercion adicional

#### Scenario: Insercion en posicion permitida
- **WHEN** el usuario solicita un `PageBreak` en una posicion valida entre bloques
- **THEN** el sistema SHALL insertarlo sin corromper la estructura del documento

### Requirement: Integracion del salto manual con la paginacion visual
El sistema SHALL tratar cada `PageBreak` manual como un limite duro para el calculo y la representacion de la paginacion visual.

#### Scenario: Recalculo despues de un salto manual
- **WHEN** existe uno o varios `PageBreak` en el contenido
- **THEN** las metricas y guias de paginacion visual SHALL reiniciar el calculo de pagina despues de cada salto manual

#### Scenario: Compatibilidad con el modo visual existente
- **WHEN** `AppEditor` esta en `paginationMode="visual"` y contiene `PageBreak`
- **THEN** el shell paginado SHALL reflejar esos limites sin alterar el resto del comportamiento del editor
