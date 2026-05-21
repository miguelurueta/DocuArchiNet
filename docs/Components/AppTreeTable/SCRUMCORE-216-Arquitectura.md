# SCRUMCORE-216 - Arquitectura

## 1. Resumen arquitectonico
Refactoriza `AppTreeTable` para implementarlo como wrapper/adaptador reusable sobre `AppTable`, preservando compatibilidad con consumidores actuales y evitando duplicacion del engine tabular.

Restricciones:
- No reemplazar `AppTable`.
- No acoplar `AppTreeTable` a dominios (ej. `GestionCorrespondencia`).
- No introducir contratos backend-driven nuevos en este ticket.

## 2. Vista estatica

Capas:
- `components`: `AppTreeTable.tsx`
- `hooks`: estado de expansion y filas visibles
- `adapters`: flattening, indentacion, mapeo Tree -> Table
- `types`: tipos publicos y tipos internos
- `style`: `AppTreeTable.module.css`

Dependencias:
- `AppTreeTable` -> `AppTable` -> AG Grid

## 3. Diagrama de clases

```mermaid
classDiagram
  class AppTreeTable {
    +rows?: AppTreeTableRow[]
    +load?: () => Promise
    +loadChildren?: (row) => Promise
    +onSelectRow?: (id) => void
  }

  class AppTable {
    +rows: AppTableRow[]
    +columns: ColDef[]
  }

  class AppTreeTableRow {
    +id: string
    +label: string
    +values?: Record
    +children?: AppTreeTableRow[]
  }

  class TreeTableRow {
    +id: string
    +level: number
    +expanded: boolean
    +hasChildren: boolean
    +originalNode: AppTreeTableRow
  }

  AppTreeTable --> AppTable : renders via
  AppTreeTableRow <.. TreeTableRow : originalNode
```

## 4. Diagramas de secuencia

### Render inicial
```mermaid
sequenceDiagram
  participant UI as AppTreeTable
  participant A as Adapters/Hooks
  participant T as AppTable
  UI->>A: flattenTree(rows, expandedIds)
  A-->>UI: TreeTableRow[]
  UI->>A: mapTreeRowsToAppTableRows(...)
  A-->>UI: AppTable rows
  UI->>T: render(rows, columns)
```

### Expand/Collapse
```mermaid
sequenceDiagram
  participant UI as AppTreeTable
  participant H as useTreeExpansionState
  participant S as state(rows)
  UI->>H: toggleExpanded(nodeId)
  alt needsChildren
    UI->>UI: loadChildren(node)
    UI->>S: updateRowById(children)
  end
```

## 5. Diagrama de estados
```mermaid
stateDiagram-v2
  [*] --> idle
  idle --> loading: load()
  loading --> ready: ok
  loading --> error: fail
  ready --> ready: expand/collapse
  error --> loading: retry
```

## 6. ADRs resumidas
- Wrapper vs segundo motor: se elige wrapper para evitar duplicacion de logica y divergencia UX.
- Expansion local: `AppTreeTable` controla expansion por ser concern jerarquico.

## 7. Riesgos tecnicos y mitigaciones
- Re-render masivo: memoizacion en hooks/adapters.
- Regresiones en consumidores: pruebas unitarias + integracion en `DocumentosWorkbench`.

## 8. Trazabilidad a codigo
- `src/app/Components/UI/AppTreeTable/AppTreeTable.tsx`
- `src/app/Components/UI/AppTreeTable/hooks/useTreeExpansionState.ts`
- `src/app/Components/UI/AppTreeTable/hooks/useTreeVisibleRows.ts`
- `src/app/Components/UI/AppTreeTable/adapters/*`

