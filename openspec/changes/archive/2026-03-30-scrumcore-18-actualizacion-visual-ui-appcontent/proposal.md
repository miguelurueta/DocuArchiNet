## Why

ACTUALIZACION-VISUAL-UI-APPCONTENT. PROMPT PROFESIONAL — Ajustar AppContent con altura dinámica descontando Toolbar(React 19 + TypeScript estricto + CSS Modules)

## What Changes

- Se genera automaticamente una propuesta OpenSpec basada en el issue SCRUMCORE-18.
- Se ajusta `AppContent` para ocupar el espacio vertical restante debajo del `AppToolbar` sin depender de calculos manuales.
- Se convierte el layout consumidor a una columna flex con `overflow: hidden` para evitar scroll en el body o en la pagina completa.
- Se configura `AppContent` con `flex: 1`, `min-height: 0` y scroll interno vertical cuando el contenido excede el espacio disponible.
- Se mantiene estable el layout de `GestionCorrespondencia` en desktop, tablet y mobile.

## Capabilities

### New Capabilities
- `altura-dinamica-appcontent-gestion-correspondencia`: Ajuste visual y estructural para que `AppContent` ocupe el alto restante con scroll interno controlado.

### Modified Capabilities
- 

## Impact

- Ajustes de layout en `src/modules/gestionCorrespondencia/style/GestionCorrespondenciaLayout.module.css`.
- Ajustes de page container en `src/modules/gestionCorrespondencia/style/GestionCorrespondencia.module.css`.
- Ajustes de `AppContent` en `src/app/Components/UI/AppContent/AppContent.module.css`.
- Sin cambios en logica de negocio, navegacion ni acciones de toolbar.
