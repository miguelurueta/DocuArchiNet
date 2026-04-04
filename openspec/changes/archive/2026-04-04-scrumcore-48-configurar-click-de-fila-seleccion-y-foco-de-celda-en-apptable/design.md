## Context

`AppTable` ya soporta:

- `onRowClicked`
- `rowSelection: "single" | "multiple"`
- configuración base de selección en AG Grid

Pero hoy el click de fila y el foco visual de celda quedan mezclados por el default del grid. Eso genera una UX ambigua: la fila se selecciona, el evento puede dispararse, y además queda una celda visualmente activa.

## Decision

Separar explícitamente tres dimensiones del comportamiento:

1. Evento de click de fila
2. Selección de fila
3. Foco visual de celda

La corrección no debe quitar la selección de fila por click. Debe introducir un control explícito del foco de celda, con default orientado a listados.

## Contract

`AppTable` debe exponer:

- `rowSelection?: "single" | "multiple"`
- `onRowClicked?: (row) => void`
- `suppressCellFocus?: boolean`

Default recomendado:

- `suppressCellFocus = true`

## Implementation notes

- El contrato vive en `AppTable.types.ts`
- La configuración de AG Grid vive en `useAgGridBaseConfig.ts`
- `AppTableGridRenderer.tsx` mantiene el wiring de `onRowClicked`
- No se cambia backend ni pipeline de datos

## Compatibility

- La selección única debe seguir funcionando
- La selección múltiple debe seguir funcionando
- El click de fila debe seguir disponible
- Pantallas que necesiten foco de celda pueden desactivarlo explícitamente
