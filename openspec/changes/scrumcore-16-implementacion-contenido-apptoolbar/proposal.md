## Why

IMPLEMENTACION-CONTENIDO-APPTOOLBAR. PROMPT PROFESIONAL — Ajustar AppToolbar en GestionCorrespondencia(React 19 + TypeScript estricto + Ant Design)

## What Changes

- Se genera automaticamente una propuesta OpenSpec basada en el issue SCRUMCORE-16.
- Se refactoriza el area de acciones de `AppToolbar` dentro de `GestionCorrespondencia` para dejar exactamente dos acciones principales.
- Se introduce un patron de `AppDropdown` jerarquico basado en `AppButton` para la accion `Exportar`.
- Se reconstruye la accion `Abrir respuesta contextual` con `AppButton` manteniendo la navegacion relativa hacia `respuesta`.
- Se ajustan estilos enterprise y comportamiento responsive de la toolbar consumidora en el modulo.

## Capabilities

### New Capabilities
- `toolbar-acciones-gestion-correspondencia`: Integracion refinada de acciones enterprise en `GestionCorrespondencia` usando `AppToolbar`, `AppDropdown` y `AppButton`.

### Modified Capabilities
- 

## Impact

- Refactor en `src/modules/gestionCorrespondencia/pages/GestionCorrespondencia.tsx`.
- Ajustes de estilos en `src/modules/gestionCorrespondencia/style/GestionCorrespondencia.module.css`.
- Extension de `AppDropdown` para soportar submenus jerarquicos con iconografia.
- Actualizacion de pruebas de comportamiento sobre `AppDropdown`, `AppToolbar` y la ruta consumidora del modulo.
