## Why

ACTUALIZACION-VISUAL-UI-APPBUTTON. PROMPT PROFESIONAL — Agregar botón "Actualizar" en AppToolbar (Design System Enterprise)

## What Changes

- Se agrega un nuevo `AppButton` con etiqueta `Actualizar` dentro del `AppToolbar` de `GestionCorrespondencia`.
- El nuevo botón usa `variant="ghost"`, `size="sm"` y `leftIcon={<UndoOutlined />}` siguiendo el design system existente.
- Se mantiene el layout actual del toolbar y su comportamiento responsive en desktop, tablet y mobile.
- Se ajusta `.page` en `GestionCorrespondencia.module.css` para usar `background-color: white`.

## Capabilities

### New Capabilities
- `boton-actualizar-toolbar-gestion-correspondencia`: Acción visual adicional en `AppToolbar` sin lógica de negocio real.

### Modified Capabilities
- 

## Impact

- Ajustes en `src/modules/gestionCorrespondencia/pages/GestionCorrespondencia.tsx`.
- Ajustes en `src/modules/gestionCorrespondencia/style/GestionCorrespondencia.module.css`.
- Sin cambios en la lógica interna de `AppToolbar` ni en el contrato de `AppButton`.
