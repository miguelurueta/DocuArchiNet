## Why

ACTUALIZACION-VISUAL-UI-APPTOOLBAR. PROMPT PROFESIONAL — Ajuste de ALTURA RESPONSIVE en AppToolbar (SIN modificar botones)(React 19 + TypeScript estricto + Ant Design)

## What Changes

- Se genera automaticamente una propuesta OpenSpec basada en el issue SCRUMCORE-17.
- Se ajusta unicamente el comportamiento responsive de altura del `AppToolbar` consumido por `GestionCorrespondencia`.
- Se corrigen reglas de contenedor para permitir crecimiento vertical automatico cuando las acciones hacen wrap en tablet y mobile.
- Se incorpora una correccion adicional para el breakpoint de `1100px`, donde el toolbar reservaba alto artificial por el `flex-basis` de las regiones internas y dejaba una altura incorrecta en vez de ajustarse al contenido.
- Se mantiene intacta la logica de botones, `AppDropdown`, `AppButton` y la estructura JSX existente.
- Se formaliza la validacion del cambio como refinamiento visual de layout y no como cambio funcional del modulo.

## Capabilities

### New Capabilities
- `altura-responsive-toolbar-gestion-correspondencia`: Ajuste visual del contenedor `AppToolbar` en `GestionCorrespondencia` para crecimiento vertical automatico y responsive.

### Modified Capabilities
- 

## Impact

- Ajustes de CSS en `src/modules/gestionCorrespondencia/style/GestionCorrespondencia.module.css`.
- Ajuste puntual en `src/app/Components/UI/AppToolbar/AppToolbar.module.css` para neutralizar `flex-basis` en layout de columna a `<=1100px`.
- Sin cambios en logica de negocio, navegacion ni estructura de botones.
- Validacion orientada a comportamiento visual y continuidad del flujo responsive del modulo.
