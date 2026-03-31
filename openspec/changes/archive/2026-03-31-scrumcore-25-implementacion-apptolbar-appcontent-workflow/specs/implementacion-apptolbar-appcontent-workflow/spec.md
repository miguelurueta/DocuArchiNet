## ADDED Requirements

### Requirement: Composicion de UI con AppToolbar y AppContent
El sistema MUST renderizar `AppToolbar` arriba y `AppContent` abajo dentro de `Workflow.tsx`, respetando el orden obligatorio.

#### Scenario: Orden de renderizado correcto
- **WHEN** el componente `Workflow` se renderiza
- **THEN** `AppToolbar` aparece antes de `AppContent`

### Requirement: Toolbar con acciones y controles reutilizables
El sistema SHALL incluir `AppButton` y `AppDropdown` dentro de `AppToolbar` para acciones superiores definidas por el modulo.

#### Scenario: Toolbar contiene controles
- **WHEN** el usuario visualiza `Workflow`
- **THEN** la toolbar muestra controles de acciones superiores

### Requirement: Contenido preparado para datos extensos
El sistema MUST ofrecer un contenedor de contenido que soporte tablas grandes (AG Grid / Ant Design) sin romper el layout.

#### Scenario: Contenido amplio no rompe el layout
- **WHEN** el contenido crece vertical u horizontalmente
- **THEN** `AppContent` mantiene el layout y permite scroll segun sea necesario

### Requirement: Estilos responsivos con CSS Modules
El sistema SHALL aplicar estilos responsivos en `Workflow.module.css` para desktop, tablet y mobile, evitando overflow horizontal inesperado.

#### Scenario: Toolbar adaptable en mobile
- **WHEN** la pantalla es menor a 768px
- **THEN** la toolbar se adapta y mantiene botones accesibles sin overflow

### Requirement: Layout sin logica de negocio
El sistema MUST mantener `WorkflowLayout` como estructura y delegar la composicion UI a `Workflow.tsx`.

#### Scenario: Layout estructural
- **WHEN** se renderiza `WorkflowLayout`
- **THEN** solo se encarga de la estructura y contiene el `Outlet`
