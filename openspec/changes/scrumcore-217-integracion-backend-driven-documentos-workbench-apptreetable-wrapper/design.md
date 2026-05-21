## Context

`DocumentosWorkbench` necesita consumir un listado jerárquico (tree) de documentos impulsado por backend (backend-driven), sin reimplementar una tabla ni acoplarse a detalles de `AppTable`. En el repo ya existe `AppTreeTable` como wrapper sobre `AppTable` (refactor de `SCRUMCORE-216`), con:

- API pública simple (`rows`, `load`, `loadChildren`, `onSelectRow`, `columns`).
- Adaptadores `Tree -> Table` y hooks de expansión/visibilidad.
- Render/selección delegada a `AppTable`.

Este ticket integra `DocumentosWorkbench` con backend-driven para poblar `AppTreeTable` usando contratos claros (DTOs + mapping) y asegurando que el cambio quede confinado al módulo (sin afectar otros consumidores/componentes).

## Goals / Non-Goals

### Goals
- Integrar `DocumentosWorkbench` para construir el árbol de documentos desde backend (carga inicial + carga por nodo).
- Mantener `AppTreeTable` como wrapper (sin duplicar lógica de `AppTable`).
- Mantener TypeScript estricto (sin `any`) y contratos tipados para DTOs/mapping.
- No afectar otros consumidores de `AppTreeTable` / `AppTable` (cambios aislados).
- Agregar pruebas que cubran el Spec asociado y documentar evidencia.

### Non-Goals
- Reemplazar `AppTable` o reescribir `AppTreeTable`.
- Cambiar routing/layout/arquitectura SPA.
- Introducir una nueva librería grande para state-management o data-fetching.
- Rediseñar UI/UX completa del workbench; el foco es integración backend-driven.
- Cambiar el contrato o comportamiento de `AppTreeTable` (fuera del alcance).

## Decisions

### 1) Contrato backend-driven (DTOs) + mapping a `AppTreeTableRow`
**Decision:** Definir tipos DTO para la respuesta del backend (lista raíz y/o children por nodo) y mapearlos a `AppTreeTableRow`.

**Rationale:** El workbench debe ser backend-driven sin filtrar tipos internos de UI a la capa services. El mapping permite:
- desacoplar cambios del backend del componente UI
- preservar la API estable de `AppTreeTable`

**Alternativas consideradas:**
- Consumir la respuesta del backend directamente como `AppTreeTableRow` (rechazado: acopla contrato backend a UI y dificulta evolución).

**Contrato propuesto (refinable):**
- Root: retorna `{ ok: true, columns: string[], rows: BackendDocumentNodeDto[] }` (o shape equivalente).
- Children: retorna `{ ok: true, rows: BackendDocumentNodeDto[] }`.
- `BackendDocumentNodeDto` MUST contener:
  - `id: string`
  - `label: string`
  - `hasChildren: boolean` (o `childCount > 0`)
  - `values?: Record<string, string | number | boolean | null>`
  - `meta?: Record<string, unknown>` (opcional)

### 2) Estrategia de carga: `load()` + `loadChildren(row)`
**Decision:** En `DocumentosWorkbench`, usar `load()` para carga inicial y `loadChildren(row)` para cargar children bajo demanda.

**Rationale:** Encaja con la API de `AppTreeTable` y soporta crecimiento sin cargar todo el árbol de una vez.

**Alternativas consideradas:**
- Cargar el árbol completo en un solo endpoint (posible, pero penaliza performance y memoria; se deja como opción backend).

### 3) Columnas desde backend (sin hardcode)
**Decision:** Resolver columnas de tabla desde metadata del backend (`columns: string[]`) y pasarlas a `AppTreeTable`.

**Rationale:** Backend-driven implica que columnas/labels pueden variar por contexto, sin requerir cambios de frontend.

**Alternativas consideradas:**
- Inferir columnas desde `values` en el cliente (ya existe fallback en `AppTreeTable`; se mantiene, pero se prioriza metadata backend cuando esté disponible).

### 4) Aislamiento del cambio
**Decision:** Colocar DTOs/mapping/services/hook dentro de `src/modules/gestionCorrespondencia/**` (o el módulo dueño de `DocumentosWorkbench`), evitando tocar `AppTreeTable` salvo que el contrato lo requiera.

**Rationale:** Minimiza riesgo de regresión en otros consumidores y mantiene responsabilidades por dominio.

## Risks / Trade-offs

- [Riesgo] Variabilidad de columnas por backend puede romper snapshots/expectativas visuales → Mitigación: tests de integración validan que se rendericen columnas esperadas para un mock.
- [Riesgo] Carga incremental (children) puede generar estados intermedios confusos → Mitigación: usar estados legacy del wrapper (`loadingChildren`) y mensajes consistentes.
- [Riesgo] Performance con árbol grande → Mitigación: virtualización la provee `AppTable`/AG Grid; evitar recomputaciones masivas (memoización ya existente en wrapper).

## Migration Plan

1. Implementar servicios backend-driven (DTOs + llamada HTTP) para obtener raíz e hijos.
2. Implementar mapping DTO -> `AppTreeTableRow`.
3. Conectar `DocumentosWorkbench` a `AppTreeTable` usando `load/loadChildren`.
4. Agregar pruebas unitarias (mapping) y de integración (workbench render + expand).
5. Documentar evidencia de tests y actualizar metadata del cambio.

Rollback:
- Revertir cambios del módulo `DocumentosWorkbench` y dejar el listado en modo mock/local si aplica.

## Open Questions

- ¿Cuál es el contrato exacto del backend (shape de `columns`, keys de `values`, paginación, permisos)?
- ¿La jerarquía de documentos es por gabinete, por tipo, por carpeta, o por relación documental?
- ¿Se requiere caché local de children por nodo o siempre se reconsulta?
