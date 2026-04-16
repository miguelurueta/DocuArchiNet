## ADDED Requirements

### Requirement: Apertura del modal desde Solicitud de Aprobacion
El sistema SHALL abrir un modal reusable de gestión de documento desde el botón
`Solicitud de Aprobacion` del tab **Gestion** en `GestionRespuesta`.

#### Scenario: Trigger del modal
- **WHEN** el usuario hace click en `Solicitud de Aprobacion`
- **THEN** se abre un `AppModal` controlado
- **AND** el resto del workbench del tab **Gestion** permanece estable

### Requirement: Modal reusable basado en AppModal
El sistema SHALL implementar el modal usando `AppModal` como shell principal y
encapsular su contenido en un componente propio del módulo.

#### Scenario: Shell del modal
- **WHEN** el modal se renderiza
- **THEN** utiliza `AppModal` y no un contenedor modal alternativo
- **AND** su contenido vive dentro de `modalGestionDocumento/`

### Requirement: Composicion moderna tipo formulario/workbench
El sistema SHALL renderizar una composición visual moderna dentro del modal con
las zonas `infoBox`, `formGrid` y `actions`.

#### Scenario: Layout del modal
- **WHEN** el modal se abre
- **THEN** muestra un bloque superior de información
- **AND** muestra un grid de formulario principal
- **AND** muestra acciones alineadas a la derecha

### Requirement: Uso exclusivo de componentes shared del formulario
El sistema SHALL construir el contenido del modal usando exclusivamente
`AppInputSelect`, `AppInput` tipo checkbox y `AppInputTags`.

#### Scenario: Render de controles internos
- **WHEN** el modal se renderiza
- **THEN** los controles internos usan componentes shared del proyecto
- **AND** no se introducen inputs alternativos fuera del Design System

### Requirement: Estado local sin logica de negocio
El sistema SHALL manejar únicamente estado local del formulario dentro del flujo
del modal, sin integrar backend ni lógica de negocio.

#### Scenario: Interaccion local del formulario
- **WHEN** el usuario interactúa con el select, checkbox o tags
- **THEN** el modal actualiza su estado local
- **AND** no ejecuta lógica remota ni submit de negocio real

### Requirement: Accesibilidad y cierre consistente
El sistema SHALL ofrecer foco inicial adecuado, navegación por teclado y cierre
consistente del modal.

#### Scenario: Accesibilidad del modal
- **WHEN** el modal se abre
- **THEN** el usuario puede navegarlo por teclado
- **AND** los botones y controles internos tienen labels claros

#### Scenario: Cierre del modal
- **WHEN** el usuario acciona cancelar o cerrar
- **THEN** el modal se cierra correctamente mediante `onClose`

### Requirement: Responsive del layout interno
El sistema SHALL mantener el layout interno del modal legible y estable en
desktop, tablet y mobile.

#### Scenario: Render responsive
- **WHEN** el modal se renderiza en diferentes breakpoints
- **THEN** el grid y las acciones no rompen el layout inmediato
- **AND** el contenido mantiene jerarquía visual clara
