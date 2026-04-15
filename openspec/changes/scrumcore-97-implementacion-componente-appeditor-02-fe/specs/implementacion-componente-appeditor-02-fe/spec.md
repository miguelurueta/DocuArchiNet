## ADDED Requirements

### Requirement: UI responsive de AppEditor fase 02
El sistema SHALL consolidar la capa de presentacion de `AppEditor` con una experiencia visual responsive y reusable, preservando la arquitectura existente y sin introducir logica de negocio en estilos.

#### Scenario: Jerarquia visual estable
- **WHEN** una vista renderiza `AppEditor` con `title`, `description` y toolbar
- **THEN** el componente SHALL mostrar una jerarquia clara entre encabezado, toolbar, superficie editable y estados auxiliares

#### Scenario: Integracion limitada a presentation
- **WHEN** se implementa la fase 02 de `AppEditor`
- **THEN** los cambios SHALL concentrarse en `AppEditor.tsx`, `AppEditorToolbar.tsx` y `AppEditor.module.css` sin modificar la logica del hook ni la infraestructura del editor

### Requirement: Toolbar usable en mobile, tablet y desktop
El sistema SHALL ofrecer una toolbar clara, compacta y usable en los tres breakpoints definidos para la fase 02.

#### Scenario: Toolbar mobile sin overflow horizontal
- **WHEN** el viewport es `<= 768px`
- **THEN** la toolbar SHALL adaptarse a multiples filas o distribucion compacta, manteniendo controles tactiles usables y sin overflow horizontal del editor

#### Scenario: Toolbar desktop completa
- **WHEN** el viewport es `>= 1025px`
- **THEN** la toolbar SHALL aprovechar el ancho disponible sin saturacion visual y con separacion consistente entre grupos de acciones

### Requirement: Tokens visuales y soporte light/dark
El sistema SHALL exponer tokens CSS para fondo, bordes, toolbar, foco, estados mute y error, permitiendo override para light mode y dark mode desde fabrica.

#### Scenario: Tokens base definidos
- **WHEN** se inspecciona `AppEditor.module.css`
- **THEN** existen como minimo `--editor-bg`, `--editor-border`, `--editor-toolbar-bg`, `--editor-toolbar-border`, `--editor-focus`, `--editor-muted` y `--editor-error`

#### Scenario: Dark mode configurable
- **WHEN** el entorno consumidor redefine los tokens visuales del componente
- **THEN** `AppEditor` SHALL mantener contraste y jerarquia visual coherentes sin requerir cambios en la logica del componente

### Requirement: Estados visuales accesibles y consistentes
El sistema SHALL reflejar visualmente `hover`, `focus`, `active`, `disabled`, `readOnly` y `error` con feedback visible y accesible.

#### Scenario: Focus visible en controles interactivos
- **WHEN** el usuario navega por teclado sobre la toolbar y la superficie editable
- **THEN** el componente SHALL mostrar focus visible sin depender unicamente de hover

#### Scenario: Estado de error legible
- **WHEN** una vista provee `error` a `AppEditor`
- **THEN** el borde o la superficie SHALL reflejar el estado de error y el mensaje SHALL conservar contraste adecuado

#### Scenario: Estado disabled distinguible
- **WHEN** `AppEditor` se renderiza con `disabled=true`
- **THEN** la UI SHALL comunicar visualmente el estado bloqueado sin romper la legibilidad del contenido

### Requirement: Legibilidad de la superficie editable
El sistema SHALL mantener una superficie de edicion con tipografia, espaciado, headings, enlaces e imagenes alineados con el design system y con alta legibilidad.

#### Scenario: Contenido enriquecido legible
- **WHEN** el editor contiene parrafos, headings, listas, links e imagenes
- **THEN** la superficie SHALL mantener espaciado consistente, lectura clara y comportamiento estable en distintos tamanos de viewport

#### Scenario: Placeholder visible y no intrusivo
- **WHEN** el editor esta vacio
- **THEN** el placeholder SHALL ser visible, legible y no interferir con la interaccion del usuario
