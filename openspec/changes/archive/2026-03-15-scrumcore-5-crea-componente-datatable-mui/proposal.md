## Why

El proyecto necesita un componente de tabla reusable sobre MUI Data Grid para desacoplar a las vistas consumidoras de la API directa del proveedor y estandarizar paginacion, estados vacios, carga y estilo. Esto extiende la capa `UI` compartida ya construida con `AppButton`, `AppInput`, `AppModal` y `AppTabs`.

## What Changes

- Crear el componente compartido `AppDataTableMui` en `src/app/Components/UI`.
- Definir una API tipada propia del proyecto para columnas, filas, seleccion, carga y paginacion.
- Estandarizar estados visuales de tabla, overlay vacio, loading y altura adaptable sobre `@mui/x-data-grid`.
- Agregar estilos encapsulados, pruebas focalizadas y README de uso.
- Incorporar el componente al barrel de la capa UI y formalizar la capacidad `app-datatable-mui` en OpenSpec.

## Capabilities

### New Capabilities
- `app-datatable-mui`: componente reusable para tablas de datos sobre MUI Data Grid con contrato tipado, estados comunes y desacoplamiento del proveedor.

### Modified Capabilities
- Ninguna.

## Impact

- Nuevo componente en `src/app/Components/UI/AppDataTableMui/`.
- Actualizacion del barrel `src/app/Components/UI/index.ts`.
- Nuevas pruebas unitarias y documentacion del componente.
- Nuevo spec principal `openspec/specs/app-datatable-mui/spec.md` al sincronizar el cambio.
