# AppDataTableMui

`AppDataTableMui` es la tabla de datos estandar reusable del proyecto sobre `@mui/x-data-grid`. Encapsula el proveedor detras de una API propia para mantener consistencia visual, estados comunes y bajo acoplamiento en listados administrativos y tablas operativas.

## Importacion

```tsx
import { AppDataTableMui } from "src/app/Components/UI";
```

## Props principales

| Prop | Tipo | Default | Descripcion |
| --- | --- | --- | --- |
| `rows` | `GridRowsProp` | requerido | Filas a renderizar en la grilla. |
| `columns` | `GridColDef[]` | requerido | Definicion de columnas de la tabla. |
| `loading` | `boolean` | `false` | Activa el feedback visual de carga. |
| `emptyMessage` | `ReactNode` | `"No hay registros para mostrar."` | Mensaje mostrado en el overlay vacio. |
| `label` | `string` | `undefined` | Nombre accesible programatico para la grilla. |
| `initialPageSize` | `number` | `10` | Tamano de pagina inicial. |
| `pageSizeOptions` | `number[]` | `[5, 10, 20, 50]` | Opciones de tamanos de pagina visibles para el usuario. |
| `rowSelectionModel` | `GridRowSelectionModel` | `undefined` | Modelo de seleccion controlado. |
| `onRowSelectionModelChange` | `(model) => void` | `undefined` | Callback de cambio del modelo de seleccion. |
| `checkboxSelection` | `boolean` | `false` | Habilita seleccion por checkbox. |

## Ejemplos

### Basico

```tsx
<AppDataTableMui
  label="Usuarios"
  rows={rows}
  columns={columns}
/>
```

### Con seleccion

```tsx
<AppDataTableMui
  label="Usuarios"
  rows={rows}
  columns={columns}
  checkboxSelection
  onRowSelectionModelChange={setSelectionModel}
/>
```

### Con estado vacio personalizado

```tsx
<AppDataTableMui
  label="Usuarios"
  rows={[]}
  columns={columns}
  emptyMessage="No se encontraron usuarios para este filtro."
/>
```

## Buenas practicas

- Mantiene la definicion de columnas fuera del render cuando la vista las reutiliza entre renders.
- Usa `label` siempre que la tabla no tenga un encabezado visible que ya describa claramente su contenido.
- Usa `checkboxSelection` solo en flujos donde la seleccion batch tenga una accion posterior clara.
- Prefiere este wrapper sobre `DataGrid` directo para conservar consistencia visual y desacoplamiento del proveedor.
