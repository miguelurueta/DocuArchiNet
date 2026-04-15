## ADDED Requirements

### Requirement: Modo de paginacion visual configurable en AppEditor
El sistema SHALL permitir que `AppEditor` se renderice en modo continuo o en un modo de paginacion visual open source, sin fragmentar internamente el documento ni mover la logica de paginacion a Tiptap.

#### Scenario: Modo por defecto sin paginacion
- **WHEN** una vista renderiza `AppEditor` sin configurar `paginationMode`
- **THEN** el componente SHALL mantener el comportamiento actual de documento continuo sin cambios de layout ni regresiones funcionales

#### Scenario: Activacion explicita del modo visual
- **WHEN** una vista renderiza `AppEditor` con `paginationMode="visual"`
- **THEN** el componente SHALL presentar el contenido dentro de una superficie tipo hoja manteniendo un unico documento continuo editable

### Requirement: API tipada para formato, orientacion y margenes
El sistema SHALL exponer una API tipada para configurar formato de pagina, orientacion y margenes del modo visual desde `AppEditor`, manteniendo compatibilidad con el contrato actual del componente.

#### Scenario: Configuracion A4 vertical con margenes explicitos
- **WHEN** una vista renderiza `AppEditor` con `pageFormat="A4"`, `pageOrientation="portrait"` y `pageMargins`
- **THEN** el componente SHALL aplicar esas opciones al layout visual de hoja sin alterar el HTML serializado del contenido

#### Scenario: Compatibilidad con consumidores existentes
- **WHEN** un consumidor actual usa `AppEditor` sin las nuevas props de paginacion
- **THEN** la integracion SHALL seguir funcionando sin requerir cambios en su codigo

### Requirement: Layout tipo hoja desacoplado de Tiptap
El sistema SHALL resolver la experiencia visual paginada desde la capa de `presentation` con una estructura `editorWrapper -> canvas -> sheet -> content`, manteniendo a Tiptap como infraestructura de edicion y no como responsable del layout de pagina.

#### Scenario: Hoja centrada dentro del canvas
- **WHEN** `AppEditor` se renderiza en modo visual
- **THEN** la hoja SHALL mostrarse centrada dentro de un `canvas` con fondo de workspace y jerarquia visual clara de documento

#### Scenario: Dimensiones base A4
- **WHEN** el formato configurado es `A4` en orientacion vertical
- **THEN** la hoja SHALL usar como referencia visual base `794px` de ancho y `1123px` de alto

### Requirement: Scroll del documento en canvas sin scroll interno de hoja
El sistema SHALL ubicar el scroll del modo paginado visual en el `canvas` contenedor, evitando scroll interno de la hoja y preservando la experiencia de edicion continua.

#### Scenario: Contenido largo en modo visual
- **WHEN** el contenido del editor excede la altura visible disponible
- **THEN** el desplazamiento SHALL ocurrir en el `canvas` y no dentro de una hoja con scroll interno independiente

#### Scenario: Edicion estable dentro de una sola hoja visual
- **WHEN** el contenido aun cabe dentro de la superficie visible de la hoja
- **THEN** el usuario SHALL poder editar normalmente sin percibir doble scroll ni perdida de foco

### Requirement: Documento continuo sin cambios estructurales
El sistema SHALL preservar el modelo actual de documento continuo, evitando insertar nodos de pagina, saltos persistidos o modificaciones estructurales del contenido en esta fase.

#### Scenario: Serializacion HTML sin metadata de pagina
- **WHEN** `AppEditor` serializa su contenido mediante `value` y `onChange`
- **THEN** el HTML resultante SHALL permanecer libre de nodos, atributos o marcas de paginacion

#### Scenario: Sin logica de paginacion en keypress
- **WHEN** el usuario escribe dentro del editor en modo visual
- **THEN** la fase base SHALL resolver el layout con CSS y no mediante calculos de paginacion ejecutados en cada `keypress`
