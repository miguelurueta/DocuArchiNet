## Why

ACTUALIZACION-VISUAL-UI-PAGE. PROMPT PROFESIONAL — Ajustar estilos en .page (GestionCorrespondencia.module.css)(React 19 + CSS Modules)

## What Changes

- Se ajusta la clase `.page` en `GestionCorrespondencia.module.css` para aplicar estilos visuales enterprise sin alterar el layout existente.
- Se agrega `gap: 2px` para compactar la separacion entre `AppToolbar` y `AppContent`.
- Se agrega `border-radius: 24px` y `background-color: #f5f5f5` para reforzar el contenedor visual de la pagina.
- Se alinean las acciones de `GestionCorrespondencia` al estilo `ghost` y se fija texto negro para `.variantGhost` en `AppButton`.
- Se mantiene el comportamiento consistente en desktop, tablet y mobile sin tocar logica ni JSX.

## Capabilities

### New Capabilities
- `refinamiento-visual-page-gestion-correspondencia`: Ajuste visual del wrapper `.page` con fondo, radio y separacion consistentes.

### Modified Capabilities
- 

## Impact

- Ajustes en `src/modules/gestionCorrespondencia/style/GestionCorrespondencia.module.css`.
- Ajustes en `src/modules/gestionCorrespondencia/pages/GestionCorrespondencia.tsx`.
- Ajustes visuales en `src/app/Components/UI/AppButton/AppButton.module.css`.
