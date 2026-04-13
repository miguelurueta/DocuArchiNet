## ADDED Requirements

### Requirement: Workbench de Documentos con layout definido
El sistema SHALL renderizar el tab **Documentos** con un layout tipo workbench:
`AppToolbar` arriba, area principal a la izquierda y `AppCollapseRail` a la derecha,
respetando el contrato de tabs de `GestionRespuesta.tsx`.

#### Scenario: Render del workbench
- **WHEN** el usuario navega al tab **Documentos**
- **THEN** se visualiza la barra superior, el area principal y el rail lateral

### Requirement: Panel colapsable persistente
El sistema SHALL mantener el contenido del panel montado al colapsar el
`AppCollapseRail`, mostrando solo el rail cuando esta colapsado.

#### Scenario: Contenido persistente
- **WHEN** el usuario colapsa el panel lateral
- **THEN** el contenido interno permanece montado y el rail sigue visible

### Requirement: Responsive consistente
El sistema SHALL aplicar comportamiento responsive:
- Desktop: `inline` y expandido por defecto
- Tablet: colapsado por defecto
- Mobile: `overlay` con rail visible como chip

#### Scenario: Cambio de viewport
- **WHEN** el viewport cambia entre desktop, tablet y mobile
- **THEN** el workbench ajusta su variant y estado segun el breakpoint

### Requirement: Accesibilidad del toggle
El sistema SHALL exponer `aria-expanded` y `aria-controls` en los toggles
relacionados con el panel y mantener foco visible.

#### Scenario: Toggle accesible
- **WHEN** el usuario navega con teclado al toggle del panel
- **THEN** el estado es anunciado y el foco es visible

### Requirement: Scroll independiente por seccion
El sistema SHALL permitir scroll independiente en el area principal y en el
contenido interno del panel lateral.

#### Scenario: Scroll separado
- **WHEN** el contenido de ambas secciones excede el alto disponible
- **THEN** cada seccion scrollea de forma independiente
