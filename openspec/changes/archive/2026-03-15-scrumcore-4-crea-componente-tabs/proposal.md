## Why

El proyecto necesita un componente de tabs reusable para desacoplar a las vistas consumidoras del proveedor UI y estandarizar navegacion por secciones, estados visuales y accesibilidad. Esto mantiene consistencia con la capa `UI` ya definida para `AppButton`, `AppInput` y `AppModal`.

## What Changes

- Crear el componente compartido `AppTabs` en `src/app/Components/UI`.
- Definir una API tipada propia del proyecto para items, tab activa y cambios de seleccion.
- Estandarizar variantes visuales, orientacion y estados deshabilitados sobre el componente base del proveedor UI.
- Agregar estilos con CSS Modules, pruebas focalizadas y README de uso.
- Incorporar el componente al barrel de la capa UI y formalizar la capacidad `app-tabs` en OpenSpec.

## Capabilities

### New Capabilities
- `app-tabs`: componente reusable para navegacion por pestañas con contrato tipado, estados consistentes y accesibilidad.

### Modified Capabilities
- Ninguna.

## Impact

- Nuevo componente en `src/app/Components/UI/AppTabs/`.
- Actualizacion del barrel `src/app/Components/UI/index.ts`.
- Nuevas pruebas unitarias y documentacion del componente.
- Nuevo spec principal `openspec/specs/app-tabs/spec.md` al sincronizar el cambio.
