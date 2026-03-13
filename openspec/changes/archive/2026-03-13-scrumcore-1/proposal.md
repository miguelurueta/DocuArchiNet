## Why

La interfaz actual no tiene un boton estandar reusable que abstraiga Ant Design y preserve consistencia visual, semantica y de accesibilidad en toda la SPA. El ticket `SCRUMCORE-1` busca cerrar esa brecha ahora para evitar que nuevas vistas sigan acoplandose directamente a componentes base del framework UI.

## What Changes

- Crear un componente reusable `AppButton` en `src/app/Components/UI` como wrapper tipado sobre `Button` y `Tooltip` de Ant Design.
- Definir una API publica estable para variantes, tamanos, estados (`loading`, `disabled`), iconografia, `fullWidth`, `tooltip` y modo `icon-only` con reglas explicitas de accesibilidad.
- Estandarizar estilos mediante CSS Modules para mapear `variant` y `size` a una presentacion enterprise coherente con la UI actual sin exponer directamente la API visual nativa de Ant Design.
- Incorporar pruebas con Vitest + Testing Library para comportamiento, accesibilidad y semantica visual del componente.
- Documentar el componente con ejemplos de uso, contrato publico y lineamientos de integracion para vistas consumidoras.

## Capabilities

### New Capabilities
- `app-button`: componente base reutilizable para acciones UI con abstraccion sobre Ant Design, variantes visuales estandarizadas y reglas de accesibilidad consistentes.

### Modified Capabilities

## Impact

- Nuevo componente UI compartido en `src/app/Components/UI/AppButton/`.
- Nuevos estilos CSS Modules, tests y barrel export asociados al componente.
- Posible adopcion progresiva en vistas que hoy dependan directamente de botones de Ant Design.
- Nueva documentacion tecnica local para guiar el uso consistente del boton estandar del proyecto.
