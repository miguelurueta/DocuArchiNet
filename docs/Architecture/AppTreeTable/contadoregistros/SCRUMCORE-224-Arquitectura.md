# SCRUMCORE-224 - Arquitectura

## 1. Resumen arquitectonico

### Objetivo tecnico
Implementar un contador documental contextual en `DocumentosWorkbench` derivado automaticamente del estado actual de la lista y de la seleccion, sin logica mutable manual y sin impacto global en `AppTable`/`AppTreeTable`.

### Decisiones
- Se deriva `totalDocumentsCount` y `selectedDocumentsCount` en `useGestionRespuestaDocumentosTable`.
- Se prioriza `Total -> TotalRecords -> rows.length` en carga inicial.
- Tras mutaciones runtime (`agregar_item`, `eliminar_item`) se prioriza conteo por `rows/treeRows` actuales.
- Se agrega callback opcional `onSelectionChanged` en `AppTreeTable` (backward-compatible).

### Restricciones
- Sin cambios backend, endpoints ni contratos.
- Sin cambios globales de comportamiento en AppTable/AppTreeTable.
- Sin cambios de flujo Dynamic UI, documento activo ni acciones.

## 2. Vista estatica

Capas:
- `components`: `DocumentosWorkbench` renderiza contador contextual.
- `hooks`: `useGestionRespuestaDocumentosTable` calcula estado derivado.
- `AppTreeTable`: expone seleccion actual al consumidor mediante callback opcional.
- `AppTable`: conserva logica de seleccion AG Grid existente.
- `styles`: estilos locales del panel de lista.

## 3. Diagrama de clases

```mermaid
classDiagram
  class DocumentosWorkbench {
    +render()
    -documentsCounter: string
  }

  class useGestionRespuestaDocumentosTable {
    +load()
    +loadChildren()
    +onActionTriggered()
    +onSelectionChanged(rowIds)
    +totalDocumentsCount: number
    +selectedDocumentsCount: number
  }

  class AppTreeTable {
    +onSelectionChanged(rowIds)
  }

  class AppTable {
    +onSelectionChanged(rows)
  }

  DocumentosWorkbench --> useGestionRespuestaDocumentosTable
  DocumentosWorkbench --> AppTreeTable
  AppTreeTable --> AppTable
```

## 4. Diagrama de secuencia

```mermaid
sequenceDiagram
  participant UI as DocumentosWorkbench
  participant Hook as useGestionRespuestaDocumentosTable
  participant Tree as AppTreeTable
  participant API as ListaDocumentosRadicados API

  UI->>Hook: load()
  Hook->>API: queryListaDocumentosRadicados
  API-->>Hook: Rows + (Total|TotalRecords)
  Hook-->>UI: totalDocumentsCount

  UI->>Tree: render tabla
  Tree-->>UI: onSelectionChanged(rowIds)
  UI->>Hook: onSelectionChanged(rowIds)
  Hook-->>UI: selectedDocumentsCount

  UI->>Hook: onActionTriggered(agregar_item/eliminar_item)
  Hook->>API: actionListaDocumentosRadicados
  Hook->>API: queryListaDocumentosRadicados (reload)
  Hook-->>UI: totalDocumentsCount basado en rows actuales
```

## 5. Diagrama de estados

```mermaid
stateDiagram-v2
  [*] --> loading
  loading --> ready: load ok
  loading --> empty: rows=0
  ready --> selection_active: rowIds seleccionados > 0
  selection_active --> ready: seleccion vacia
  ready --> runtime_mutation: agregar_item/eliminar_item
  runtime_mutation --> ready: reload completo
  empty --> ready: nueva carga con datos
```

## 6. ADRs resumidas
- ADR-1: Derivacion automatica en hook para evitar contadores mutables.
- ADR-2: Prioridad de fallback backend en carga inicial, con preferencia runtime post-mutacion.
- ADR-3: Extension minima y opcional de `AppTreeTable` para exponer seleccion sin romper consumidores.

## 7. Riesgos tecnicos y mitigaciones
- Riesgo: desincronizacion entre seleccion y datos recargados.
  - Mitigacion: filtrado automatico de `selectedRowIds` contra `latestRowRef`.
- Riesgo: impacto global en componentes tabla.
  - Mitigacion: API nueva opcional (`onSelectionChanged`) y sin cambios default.
- Riesgo: regresion en documento activo.
  - Mitigacion: no se modifica flujo `onSelectRow` ni `onActionTriggered` existente.

## 8. Trazabilidad a codigo
- `src/modules/gestionCorrespondencia/hooks/useGestionRespuestaDocumentosTable.ts`
- `src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.tsx`
- `src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.module.css`
- `src/app/Components/UI/AppTreeTable/AppTreeTable.tsx`
- `src/app/Components/UI/AppTreeTable/types.ts`
