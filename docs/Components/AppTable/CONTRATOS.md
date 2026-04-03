# Contratos Dynamic UI para AppTable

Esta fase agrega la capa transversal que adapta contratos dinámicos del backend hacia estructuras internas consumibles por AG Grid, sin modificar el componente base `AppTable`.

## Alcance

- Modela contratos backend con soporte para variantes `PascalCase` y `camelCase`.
- Separa DTOs remotos de los contratos internos del grid.
- Mantiene adapters puros, sin React, sin HTTP y sin lógica de dominio.
- Prepara la base para fases posteriores de integración en módulos consumidores.

## Contrato backend asumido

Archivos base:

- `src/app/Components/UI/AppTable/types/dynamicUiTable.types.ts`
- `src/app/Components/UI/AppTable/adapters/dynamicUiToAgGridColumns.ts`
- `src/app/Components/UI/AppTable/adapters/dynamicUiToAgGridRows.ts`
- `src/app/Components/UI/AppTable/utils/dynamicUiActionMapper.ts`

DTOs modelados:

- `ApiResponse<T>`
- `ApiError`
- `DynamicUiTableDto`
- `DynamicUiRowsOnlyDto`
- `UiColumnDto`
- `UiRowDto`
- `UiActionDto`
- `UiCellActionDto`
- `DynamicUiPaginationDto`
- `DynamicUiSortingDto`

La normalización se resuelve por lectura tolerante de propiedades equivalentes, por ejemplo:

- `DataIndex` o `dataIndex`
- `Rows` o `rows`
- `CellActions` o `cellActions`
- `SortField` o `field`
- `SortDir` o `direction`
- `meta` o `metadata`

## Separación backend vs grid

Los DTOs representan el shape remoto tal como puede venir del backend. La salida interna del grid usa contratos distintos:

- `AppGridColumn`
- `AppGridRow`
- `AppGridCellAction`

Esta separación evita acoplar el render al backend y permite que futuras capas HTTP o hooks consuman adapters ya estabilizados.

## Estrategia de field mapping

La resolución del `field` de columna sigue este orden estricto:

1. `DataIndex`
2. `Field`
3. `ColumnName`
4. `Key`
5. `ColumnKey`
6. `Id`
7. fallback sintético `column-{index}`

La columna se considera visible salvo que `Visible = false`.
Cuando existe `Order`, las columnas visibles se ordenan por ese valor.

## Soporte de pinning dinámico

La línea dinámica ahora puede preservar metadata de columnas fijas sin cambiar la API pública de `AppTable`.

Campos soportados en `UiColumnDto`:

- `Pinned` / `pinned`
- `LockPinned` / `lockPinned`

Propagación:

1. `UiColumnDto`
2. `AppGridColumn`
3. `ColDef`

Reglas:

- `Pinned` se mapea a `AppGridColumn.pinned` y luego a `ColDef.pinned`
- `LockPinned` se mapea a `AppGridColumn.lockPinned` y luego a `ColDef.lockPinned`
- si la metadata no existe, no se aplica pinning por defecto
- en esta fase no se impone una convención automática para `isActionColumn`

## Shape final de filas

El adapter de filas retorna:

```ts
type AppGridRow = {
  id: string;
  data: Record<string, unknown>;
  meta?: Record<string, unknown>;
};
```

Reglas aplicadas:

- `Values` se aplana a `data`
- `Meta` se preserva por separado
- nunca se mezcla `meta` dentro de `data`
- si faltan `id` e identificadores alternos, se genera `row-{index}`
- `rows = null` o `rows = undefined` produce `[]`

## Manejo de actions

Las acciones se mapean a `AppGridCellAction` sin ejecutar comportamiento.

Prioridad:

- Si existe columna con `IsActionColumn = true`, se buscan `CellActions` por `ColumnKey`
- Si no hay acciones de celda para esa columna, se usan `RowActions`

El payload real también puede entregar acciones anidadas:

```ts
{
  ColumnKey: "acciones",
  Action: {
    ActionId: "gestionar_tramite",
    Presentation: "icon_button",
    Behavior: "client_event"
  }
}
```

La metadata se preserva completa:

- `behavior`
- `behaviorConfig`
- `presentation`
- `request`
- `icon`
- `tone`
- `requiresConfirm`
- `confirmTitle`
- `confirmMessage`
- `requiredClaimsAny`
- `requiredClaimsAll`
- `claimKey`
- `rules`
- `payload`
- `metadata`

## Extensibilidad de behavior y presentation

No se usan enums cerrados para `behavior` ni `presentation`. Ambos se mantienen como `string` para soportar contratos futuros sin rigidizar la infraestructura.

## Ejemplos

### Columna dinámica backend

```ts
const column = {
  DataIndex: "subject",
  Title: "Asunto",
  Visible: true,
};
```

Salida:

```ts
{
  field: "subject",
  headerName: "Asunto",
  visible: true,
  sortable: true,
  filterable: true,
}
```

### Fila dinámica backend

```ts
const row = {
  Id: "wf-1",
  Values: { subject: "Contrato", priority: "Alta" },
  Meta: { source: "backend" },
};
```

Salida:

```ts
{
  id: "wf-1",
  data: { subject: "Contrato", priority: "Alta" },
  meta: { source: "backend" },
}
```

## Integración

La integración esperada en fases siguientes es:

1. Capa HTTP obtiene `DynamicUiTableDto`
2. Adapters transforman columnas, filas y acciones
3. Un contenedor de dominio compone `rows` y `columns`
4. `AppTable` renderiza sin conocer backend ni reglas de negocio

Esta fase deja lista la infraestructura para Fase 2 y Fase 3 sin alterar el contrato público del componente base.
