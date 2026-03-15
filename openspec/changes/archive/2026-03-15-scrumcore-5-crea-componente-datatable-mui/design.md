# Design: app-datatable-mui

## Contexto

`SCRUMCORE-5` introduce un componente `AppDataTableMui` reusable en la capa `src/app/Components/UI` para encapsular `@mui/x-data-grid` detras de una API propia del proyecto. La meta es centralizar estilos, estados comunes y una superficie publica mas acotada para vistas que hoy o en el futuro necesiten tablas de datos.

## Decision

Se implementara `AppDataTableMui` como wrapper tipado sobre `DataGrid` de MUI X, ubicado en `src/app/Components/UI/AppDataTableMui/`, con:

- `AppDataTableMuiProps` basado en `Omit<ComponentProps<typeof DataGrid>, ...>` para controlar las props expuestas.
- Reexport de tipos relevantes como `AppDataTableMuiColumn` y `AppDataTableMuiRowId` para facilitar adopcion sin importar directamente desde MUI en las vistas.
- Props de alto nivel para `rows`, `columns`, `loading`, `emptyMessage`, `checkboxSelection`, `onRowSelectionModelChange`, `pageSizeOptions`, `initialPageSize`, `autoHeight` y `label`.
- Estado vacio estandar mediante un overlay custom simple del proyecto.
- Estilos via `sx` y `className` encapsulados, con apoyo de CSS Modules para el contenedor y el overlay vacio.

## API propuesta

```ts
type AppDataTableMuiColumn = GridColDef;

type AppDataTableMuiProps = Omit<
  ComponentProps<typeof DataGrid>,
  | "rows"
  | "columns"
  | "loading"
  | "pageSizeOptions"
  | "onRowSelectionModelChange"
  | "rowSelectionModel"
> & {
  rows: GridRowsProp;
  columns: GridColDef[];
  loading?: boolean;
  emptyMessage?: ReactNode;
  label?: string;
  initialPageSize?: number;
};
```

## Estructura

- `AppDataTableMui.tsx`: wrapper principal y adaptacion de props.
- `AppDataTableMui.module.css`: estilos del contenedor y overlay vacio.
- `AppDataTableMui.test.tsx`: pruebas focalizadas del contrato base.
- `README.md`: descripcion, props y ejemplos.
- `index.ts`: export local del componente.

## Estilos

El componente tendra un contenedor estable con borde, fondo y altura minima consistentes con la capa UI ya creada. Se evitara depender de temas globales complejos; en su lugar:

- CSS Modules para el shell del componente y el estado vacio;
- `sx` para overrides locales del `DataGrid` que no exijan cascada global;
- soporte de `className` externa para composicion adicional.

## Accesibilidad

Se reutilizara la semantica accesible de `DataGrid` y se verificara por pruebas:

- render del rol de grid;
- presencia de nombre accesible cuando se configure `label`;
- mantenimiento de headers y filas;
- overlay vacio legible cuando no haya resultados.

## Riesgos y trade-offs

- `DataGrid` tiene una API extensa; el wrapper reducira la superficie publica pero seguira permitiendo `...restProps` en props seguras.
- La personalizacion visual profunda de MUI puede depender de clases internas; por eso se privilegiaran overrides por `sx` y slots soportados.
- La seleccion y la paginacion de MUI pueden cambiar entre versiones; el contrato del wrapper debe quedarse en el nivel de comportamiento observable.

## Migracion

No requiere migracion inmediata. `AppDataTableMui` quedara disponible para adopcion progresiva en modulos que hoy usan tablas ad hoc o deban incorporar listados nuevos.
