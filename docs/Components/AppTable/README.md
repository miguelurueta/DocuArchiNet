# AppTable

Componente base reusable para renderizar tablas con AG Grid Community. Es 100% presentacional, desacoplado del backend y pensado como infraestructura visual para futuras fases.

## Rol en la arquitectura

- Provee una base consistente de grilla para multiples modulos.
- Centraliza configuraciones comunes (seleccion, columnas, overlays).
- No contiene logica de negocio ni transformacion de datos.

## API (props)

```tsx
type AppTableProps<T extends Record<string, unknown>> = {
  rows: T[];
  columns: ColDef<T>[];
  loading?: boolean;
  total?: number;
  rowSelection?: "single" | "multiple";
  suppressRowClickSelection?: boolean;
  domLayout?: "autoHeight" | "normal" | "print";
  className?: string;
  gridClassName?: string;
  getRowId?: (row: T) => string;
  onRowSelected?: (row: T | null) => void;
  onCellClicked?: (params: { row: T; field?: string | null; value?: unknown }) => void;
  onRowClicked?: (row: T) => void;
  onSelectionChanged?: (rows: T[]) => void;
};
```

## Ejemplo de uso

```tsx
<AppTable
  rows={data}
  columns={columns}
  loading={false}
  onRowClicked={(row) => console.log(row)}
/>;
```

## Que NO hace

- No consume APIs ni conoce DTOs backend.
- No transforma datos ni aplica reglas de negocio.
- No implementa paginacion avanzada ni CRUD.

## Hooks y configuracion

- `hooks/useAgGridBaseConfig.ts`: compone configuracion base con memoizacion.
- `utils/agGridDefaultConfig.ts`: define defaults reutilizables (selection, defaultColDef, overlays).

## Integracion futura (Fase 1B)

El contrato de props permite integrar adaptadores de datos sin cambiar AppTable. La fase 1B puede mapear DTOs externos hacia `rows` y `columns` manteniendo este componente intacto.
