## Why

Se requiere estandarizar la vista del tab **Documentos** con un workbench tipo workspace para alinear la experiencia con el resto del modulo de GestionCorrespondencia y habilitar una visualizacion clara de documentos sin introducir logica de negocio.

## What Changes

- Se define e implementa un layout visual tipo workbench para el tab **Documentos** con `AppToolbar`, area principal scrollable y `AppCollapseRail` para visualizar documentos.
- Se agrega una estructura de componentes desacoplados en `src/modules/gestionCorrespondencia/components/documentosWorkbench/`.
- Se establece comportamiento responsive (desktop/tablet/mobile) y reglas de accesibilidad (ARIA, foco visible) sin alterar el tab **Gestion**.

## Capabilities

### New Capabilities
- `documentos-workbench-tab`: Layout visual del tab **Documentos** con toolbar, area principal y panel colapsable, incluyendo responsive y accesibilidad.

### Modified Capabilities
- (sin cambios)

## Impact

- Nuevos componentes en `src/modules/gestionCorrespondencia/components/documentosWorkbench/`.
- Actualizacion del contenido renderizado del tab **Documentos** en `GestionRespuesta.tsx`.
- Estilos locales via CSS Modules para el workbench.
- Pruebas unitarias/QA relacionadas al layout y toggle del panel.
