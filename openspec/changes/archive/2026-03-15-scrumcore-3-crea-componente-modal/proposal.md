## Why

La aplicacion no tiene un modal estandar reusable que abstraiga el proveedor UI y mantenga consistencia visual, semantica y de accesibilidad en dialogos y confirmaciones. El ticket `SCRUMCORE-3` busca cubrir esa base ahora para evitar que nuevas vistas sigan creando modales ad-hoc y acoplados al framework UI.

## What Changes

- Crear un componente reusable `AppModal` en `src/app/Components/UI` como wrapper tipado sobre el componente de dialogo base ya disponible en la libreria UI del proyecto.
- Definir una API publica estable para apertura/cierre, titulo, contenido, acciones primarias/secundarias y estados basicos de interaccion coherentes con el design system interno.
- Estandarizar estilos mediante CSS Modules para que overlays, cabeceras, cuerpo y footer compartan look & feel enterprise sin exponer directamente la API visual nativa del proveedor UI.
- Incorporar pruebas con Vitest + Testing Library para comportamiento, accesibilidad, acciones y semantica visual del componente.
- Documentar el componente con ejemplos de uso, contrato publico y pautas de integracion en flujos de confirmacion y formularios.

## Capabilities

### New Capabilities
- `app-modal`: componente base reutilizable para dialogos, confirmaciones y overlays con abstraccion sobre la libreria UI, estructura estandarizada y reglas de accesibilidad consistentes.

### Modified Capabilities

## Impact

- Nuevo componente UI compartido en `src/app/Components/UI/AppModal/`.
- Nuevos estilos CSS Modules, tests y barrel export asociados al componente.
- Posible adopcion progresiva en pantallas que hoy dependan directamente del modal del proveedor UI.
- Nueva documentacion tecnica local para guiar el uso consistente del modal estandar del proyecto.
