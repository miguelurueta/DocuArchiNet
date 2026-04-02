# Query Layer para Dynamic UI Table

Esta fase agrega la capa de consulta transversal para `AppTable` sobre los contratos dinámicos estabilizados en la fase anterior.

## Alcance

- define `DynamicTableQueryInput` como contrato común de entrada
- define `RequestMapper<TRequest>` para proyectar ese input al request real de cada endpoint
- implementa un servicio delgado sobre `clienteApi`
- implementa `useDynamicUiTableQuery` como única capa con React Query
- reutiliza los adapters de fase 1B sin modificarlos

## Archivos

- `src/app/Components/UI/AppTable/types/dynamicUiTableQuery.types.ts`
- `src/app/Components/UI/AppTable/services/dynamicUiTable.service.ts`
- `src/app/Components/UI/AppTable/hooks/useDynamicUiTableQuery.ts`

Endpoint actual usado por el servicio base:

```ts
"/api/workflowInboxgestion/inboxgestion"
```

Ese valor queda como default, no como endpoint único. El servicio también permite:

- usar `getDynamicTable(endpoint, request)` para otro endpoint compatible
- crear una variante ligada con `createDynamicTableService(endpoint)`

## Request y ACL

La ACL y cualquier detalle de dominio viven únicamente en el request. La capa transversal no conoce claims ni filtros específicos del módulo consumidor.

El patrón esperado es:

```ts
type DynamicTableQueryInput = {
  tableId: string;
  page?: number;
  pageSize?: number;
  search?: string;
  sortField?: string;
  sortDirection?: "asc" | "desc";
  includeConfig?: boolean;
};

type RequestMapper<TRequest> = (input: DynamicTableQueryInput) => TRequest;
```

Cada módulo decide cómo transformar ese input en el payload real del backend.

## Response

El servicio preserva el contrato remoto:

```ts
Promise<ApiResponse<DynamicUiTableDto | null>>
```

El hook no devuelve el DTO original. Devuelve el modelo intermedio compartido por `AppTable`:

- `rows: AppGridRow[]`
- `columns: AppGridColumn[]`
- `total`
- `pagination`
- `loading`
- `error`
- `isEmpty`
- `refetch`
- `rawResponse?`

## Manejo de estados

- `success = true` con `data = null` se interpreta como estado vacío válido
- `success = false` se normaliza a `Error`
- errores de transporte también se exponen como `Error`

## Query Key

La key usada por el hook es:

```ts
[
  "dynamic-ui-table",
  input.tableId,
  input.page,
  input.pageSize,
  input.search,
  input.sortField,
  input.sortDirection,
  input.includeConfig,
]
```

## Límite actual de la fase

Esta fase termina en `AppGridRow[]` y `AppGridColumn[]`.

Todavía no resuelve la adaptación final a:

- `ColDef<T>[]`
- filas planas tipo `Record<string, unknown>`

Eso significa que esta fase sí deja lista la consulta y normalización del backend, pero no deja todavía la integración visual directa con `AppTable.tsx` sin una capa adicional.

## Validación

Tests ejecutados para esta fase:

```bash
npm.cmd test -- src/app/Components/UI/AppTable/tests/dynamicUiTable.service.test.ts src/app/Components/UI/AppTable/tests/useDynamicUiTableQuery.test.ts
```

Resultado validado en conjunto con la fase anterior:

- `5` archivos de test
- `19` tests en verde
