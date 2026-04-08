## Context

`SCRUMCORE-69` implementa la capa frontend de autocomplete para Workflow Inbox en `GestionCorrespondencia`.

El estado actual ya tiene piezas base:

- `AppInputSearch` existe como componente presentacional basado en `AutoComplete + Input` de Ant Design.
- `GestionCorrespondencia` renderiza `AppInputSearch` dentro de `AppToolbar.actionContent`.
- `AppTableQueryWrapper` se mantiene con `showSearch={false}` para evitar doble buscador.
- `mapGestionCorrespondenciaTableRequest` fuerza `SearchType = 2` cuando hay texto simple efectivo y conserva `SearchType = 3`.
- La búsqueda real de tabla ya fluye por `table.onQueryChange({ search })`.

El nuevo alcance no debe volver a mover búsqueda a `AppTableQueryWrapper`, no debe acoplar `AppInputSearch` a endpoints y no debe convertir la pantalla en una capa de datos. La conexión de sugerencias debe quedar en un hook de dominio y un servicio HTTP específico.

El contrato backend sugerido para el frontend es:

```txt
POST /api/workflowInboxgestion/inboxgestion/autocomplete
```

El servicio frontend debe encapsular cualquier diferencia entre el contrato backend real y el contrato interno del hook, de modo que `AppInputSearch` solo reciba `options`, `loading` y callbacks.

## Goals / Non-Goals

**Goals:**

- Crear `workflowInboxAutocomplete.service.ts` para encapsular el request de sugerencias.
- Crear `useWorkflowInboxAutocomplete` para manejar `minLength`, `limit`, debounce, loading, error y respuestas obsoletas.
- Integrar `GestionCorrespondencia` con el hook sin consumir el servicio directamente desde la pantalla.
- Pasar sugerencias a `AppInputSearch.options` y estado `loading`.
- Mantener texto libre por Enter/click como búsqueda real de tabla mediante `table.onQueryChange({ search })`.
- Ejecutar búsqueda real de tabla al seleccionar una sugerencia.
- Evitar doble debounce entre `AppInputSearch` y el hook de autocomplete.
- Mantener TypeScript estricto sin `any`.

**Non-Goals:**

- No modificar `AppInputSearch` para que conozca endpoints, DTOs o `SearchType`.
- No cambiar el endpoint de listado `POST /api/workflowInboxgestion/inboxgestion`.
- No mover lógica de request paginado a `GestionCorrespondencia.tsx`.
- No reimplementar el mapper de búsqueda simple de `GestionCorrespondencia`.
- No cambiar exportación, selección, paginación ni contratos de `AppTable`.
- No implementar correcciones backend del endpoint o SQL en este repo frontend.
- No retornar o manejar filas completas como sugerencias.

## Decisions

### 1. Hook de dominio para autocomplete

Se creará `src/modules/gestionCorrespondencia/hooks/useWorkflowInboxAutocomplete.ts`.

Alternativa considerada: usar directamente `AppInputSearch.debounceMs` y llamar al servicio desde la pantalla.

Decisión: rechazada. La pantalla quedaría acoplada al endpoint y se duplicaría la responsabilidad de debounce. El hook debe ser la única fuente de debounce para sugerencias.

Contrato interno esperado:

```ts
type WorkflowInboxAutocompleteItem = {
  value: string;
  label?: string;
};

type UseWorkflowInboxAutocompleteParams = {
  minLength: number;
  limit: number;
};

type UseWorkflowInboxAutocompleteResult = {
  items: WorkflowInboxAutocompleteItem[];
  loading: boolean;
  error: Error | null;
  setSearchText: (value: string) => void;
  clear: () => void;
};
```

### 2. Servicio HTTP desacoplado de UI

Se creará `src/modules/gestionCorrespondencia/services/workflowInboxAutocomplete.service.ts`.

El servicio debe:

- llamar el endpoint aprobado
- mapear la respuesta backend al contrato interno `WorkflowInboxAutocompleteItem[]`
- controlar que `limit` viaje explícito
- no importar `AppInputSearch`
- no conocer el componente de pantalla

Alternativa considerada: reutilizar el endpoint paginado de tabla para sugerencias.

Decisión: rechazada. El autocomplete requiere respuestas pequeñas y limitadas; mezclarlo con el listado paginado aumenta costo, ruido de datos y riesgo de acoplamiento.

### 3. Dos flujos separados: sugerencias vs búsqueda real

La escritura del usuario alimenta sugerencias:

```txt
onChange(value) -> autocomplete.setSearchText(value)
```

La búsqueda real de tabla se mantiene por confirmación:

```txt
onSearch(value) -> table.onQueryChange({ search: value })
onSelect(value) -> table.onQueryChange({ search: value })
```

`AppInputSearch` debe usarse con `debounceMs={0}` en esta integración para que el debounce de sugerencias viva solo en `useWorkflowInboxAutocomplete`.

Alternativa considerada: seguir enviando `table.onQueryChange({ search })` en cada `onChange`.

Decisión: no aplica para el modo autocomplete. Ese patrón ya servía para búsqueda simple, pero con sugerencias generaría requests de tabla por cada escritura además de requests de autocomplete.

### 4. Estado visual controlado en la pantalla sin endpoint en la pantalla

`GestionCorrespondencia` puede mantener el valor visual/controlado necesario para el input o derivarlo de `table.queryState.search`, pero no debe llamar el servicio.

La integración debe garantizar:

- el texto escrito puede consultar sugerencias sin filtrar tabla inmediatamente
- Enter/click/selección sí actualizan `table.onQueryChange({ search })`
- limpiar texto limpia sugerencias y aplica el flujo de limpieza de tabla definido para la pantalla

Si se requiere un texto de borrador separado del filtro aplicado, se permite un estado local simple en la pantalla para el valor del input, siempre que no contenga lógica de endpoint, DTO ni mapping backend.

### 5. Manejo de respuestas obsoletas

El hook debe evitar que una respuesta anterior sobrescriba sugerencias más recientes.

Alternativas aceptables:

- contador incremental de request en `useRef`
- `AbortController` si el servicio lo soporta de forma limpia

Decisión: cualquiera de las dos es válida, pero debe quedar cubierta por pruebas. El criterio funcional es que la respuesta tardía de una búsqueda antigua no reemplace `items` de una búsqueda nueva.

## Risks / Trade-offs

- Doble debounce entre `AppInputSearch` y el hook -> usar `debounceMs={0}` en la integración con autocomplete y centralizar timers en `useWorkflowInboxAutocomplete`.
- Requests de tabla por cada tecla -> no conectar `onChange` directamente a `table.onQueryChange` en el modo autocomplete.
- Respuestas obsoletas sobrescriben sugerencias recientes -> invalidar request pendiente con id incremental o abortar request anterior.
- Endpoint backend difiere del contrato sugerido -> adaptar solo en el servicio y mantener estable el contrato interno del hook.
- Error de autocomplete rompe tabla -> el hook debe exponer `error` sin lanzar excepción al componente y mantener `items = []` si corresponde.
- Loading bloquea escritura -> pasar `loading` a `AppInputSearch` solo como estado visual; no deshabilitar input.
- Sugerencias demasiado amplias -> exigir `minLength` y `limit` obligatorios en el hook.
- Acoplamiento de UI a dominio -> `AppInputSearch` solo recibe `options`, `loading`, `onChange`, `onSearch`, `onClear` y callbacks de selección existentes.

## Migration Plan

1. Crear tipos internos de autocomplete para request, response e item.
2. Crear el servicio HTTP con mapping hacia `WorkflowInboxAutocompleteItem`.
3. Crear `useWorkflowInboxAutocomplete` con `minLength`, `limit`, debounce, loading, error, cleanup y protección contra respuestas obsoletas.
4. Integrar el hook en `GestionCorrespondencia` sin mover lógica de backend a la pantalla.
5. Configurar `AppInputSearch` con `options`, `loading` y `debounceMs={0}` para evitar doble debounce.
6. Mantener `AppTableQueryWrapper showSearch={false}`.
7. Agregar pruebas de hook, servicio e integración de pantalla.

Rollback:

- La integración debe poder revertirse removiendo `options/loading` y el hook de autocomplete sin tocar el mapper de búsqueda simple, exportación ni paginación.

## Open Questions

- Confirmar si el endpoint backend definitivo es `POST /api/workflowInboxgestion/inboxgestion/autocomplete`.
- Confirmar valores de producto para `minLength` y `limit`. Valores recomendados iniciales: `minLength = 2`, `limit = 10`.
- Confirmar si limpiar el input debe filtrar la tabla inmediatamente con `search: ""` o solo limpiar sugerencias hasta que el usuario confirme.
