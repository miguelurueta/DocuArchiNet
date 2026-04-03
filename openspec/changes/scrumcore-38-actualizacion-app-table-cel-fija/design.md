## Context

`SCRUMCORE-38` continúa sobre la línea dinámica existente de `AppTable`, donde hoy el componente base ya soporta columnas fijas si recibe `ColDef` manual con propiedades nativas de AG Grid como `pinned` y `lockPinned`.

El gap actual está en la línea dinámica:

`DynamicUiTableDto -> AppGridColumn -> ColDef -> AppTable`

La metadata de pinning no forma parte del contrato dinámico ni del modelo intermedio, por lo que una tabla que llegue desde backend no puede declarar columnas fijas aunque `AppTable` sí sea capaz de renderizarlas.

Esta fase debe cerrar ese gap sin cambiar la API manual actual de `AppTable`, sin acoplar la solución a una pantalla específica y sin introducir lógica de dominio.

## Goals / Non-Goals

**Goals:**

- Extender el contrato dinámico para soportar `Pinned` y `LockPinned`
- Preservar esa metadata en `AppGridColumn`
- Mapear la metadata al `ColDef` final que consume `AppTable`
- Mantener compatibilidad con tablas actuales
- Dejar la capability reusable para cualquier tabla dinámica

**Non-Goals:**

- No rediseñar `AppTable`
- No cambiar la API manual `columns: ColDef<T>[]`
- No introducir lógica específica de `gestionCorrespondencia`
- No cambiar la semántica de actions, selección o paginación
- No definir pinning por pantalla fuera del contrato dinámico

## Decisions

### 1. `AppTable` no cambia su API pública

La decisión principal es mantener `AppTable` tal como está: recibe `ColDef<T>[]` y deja que AG Grid resuelva el pinning de forma nativa.

Rationale:

- `AppTable` ya es compatible con `pinned` manual
- agregar una nueva prop del componente para algo que AG Grid ya soporta duplicaría semántica
- el problema real está en la línea dinámica, no en el renderer final

Alternativa descartada:

- introducir props propias como `fixedColumns` o `pinnedColumns` en `AppTable`

### 2. El contrato dinámico se extiende en `UiColumnDto`

La metadata de pinning debe entrar por el mismo contrato dinámico que ya describe ancho, alineación, filtros y visibilidad.

Se agregan:

- `Pinned?: "left" | "right" | null`
- `pinned?: "left" | "right" | null`
- `LockPinned?: boolean | null`
- `lockPinned?: boolean | null`

Rationale:

- mantiene el patrón actual de aliases PascalCase/camelCase del DTO
- evita inventar un canal paralelo de configuración
- deja el backend como fuente de verdad de la estructura visual de columnas

### 3. `AppGridColumn` preserva el pinning como metadata intermedia

El modelo interno `AppGridColumn` debe extenderse con:

- `pinned?: "left" | "right"`
- `lockPinned?: boolean`

Rationale:

- evita perder metadata entre el DTO y el adapter final a AG Grid
- mantiene la transformación en capas coherentes

### 4. El mapping final se hace en `appGridToAppTableColumns.ts`

El adapter final debe mapear:

- `AppGridColumn.pinned -> ColDef.pinned`
- `AppGridColumn.lockPinned -> ColDef.lockPinned`

Rationale:

- `appGridToAppTableColumns.ts` ya es la frontera entre el modelo intermedio compartido y el `ColDef`
- concentra la adaptación visual final en un solo lugar

### 5. No habrá defaults implícitos para columnas normales

Si una columna no trae metadata de pinning, no debe aplicarse ningún valor por defecto.

Rationale:

- evita alterar comportamiento de tablas existentes
- reduce regresiones

Alternativa descartada:

- fijar columnas por heurísticas como “la primera siempre a la izquierda”

### 6. Convención opcional para `isActionColumn`

Se admite como opción de diseño que una columna de acción pueda quedar fijada a la derecha con:

- `pinned = "right"`
- `lockPinned = true`
- `suppressMovable = true`

pero solo si esa convención se implementa de forma explícita, reusable y documentada.

Rationale:

- suele ser una convención útil en grids de producción
- pero no debe imponerse de forma silenciosa si el contrato backend no la pide

## Risks / Trade-offs

- [El backend puede no enviar `Pinned` y esperar defaults implícitos] -> Mitigación: documentar que el pinning solo se aplica cuando la metadata existe o cuando se adopta explícitamente una convención para `isActionColumn`
- [El pinning puede interferir con configuraciones manuales si se mezcla mal] -> Mitigación: mantener separada la línea dinámica de la API manual
- [Se puede perder metadata entre adapters] -> Mitigación: cubrir ambos pasos con tests (`DTO -> AppGridColumn` y `AppGridColumn -> ColDef`)
- [La convención de `isActionColumn` puede introducir comportamiento inesperado] -> Mitigación: dejarla opcional y probarla explícitamente si se adopta

## Migration Plan

1. Extender `UiColumnDto` y `AppGridColumn` con metadata de pinning
2. Ajustar `dynamicUiToAgGridColumns.ts` para preservar `Pinned` y `LockPinned`
3. Ajustar `appGridToAppTableColumns.ts` para mapear a `ColDef`
4. Agregar pruebas de mapeo y compatibilidad
5. Documentar la decisión tomada sobre `isActionColumn` si se implementa la convención opcional

Rollback:

- quitar `Pinned` y `LockPinned` del modelo dinámico
- revertir el mapping adicional en los adapters
- mantener solo el soporte manual actual vía `ColDef`

## Open Questions

- Si la columna de acciones debe fijarse a la derecha por convención o solo cuando el backend lo indique explícitamente
- Si el backend ya tiene una fuente de configuración de columnas que vaya a poblar `Pinned`/`LockPinned`, o si esta fase es preparatoria del frontend
