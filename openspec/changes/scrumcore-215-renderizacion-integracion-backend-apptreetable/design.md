## Context

En `SCRUMCORE-214` se introdujo `AppTreeTable` como componente reutilizable y se integró en `DocumentosWorkbench` dentro del rail de “Listado”, garantizando no interferir con `AppVisorEmbedPdf` ni otros plugins.

`SCRUMCORE-215` busca evolucionar ese componente para soportar renderización e integración “backend-driven” basada en contratos de API (query/action) sin romper el comportamiento actual del Workbench ni afectar otros módulos.

Restricciones clave del producto / repo:
- TypeScript estricto, ESM y Vite.
- Tests con Vitest/Testing Library.
- No introducir lógica “hacky” (por ejemplo, ocultar UI con CSS en vez de corregir el pipeline).
- Evitar impactos en otros plugins/operaciones (especialmente `AppVisorEmbedPdf`).

## Goals / Non-Goals

**Goals:**
- Permitir que `AppTreeTable` consuma una API “query” y renderice filas/columnas desde `Rows[].Values` + `Rows[].Meta`.
- Soportar modo jerárquico (lazy-load de hijos al expandir) sin degradar el modo plano.
- Integrar acciones por fila (“action”) que disparen operaciones (por ejemplo, ver documento) de manera desacoplada.
- Mantener separación limpia: `UI` (AppTreeTable) vs. `data access` (services/hooks por módulo consumidor).
- Asegurar que la integración en `DocumentosWorkbench` no afecte el visor ni otros componentes existentes.

**Non-Goals:**
- No rediseñar `AppTreeTable` como reemplazo total de `AppTable` ni introducir un framework nuevo de grids.
- No implementar consumo API->API (si hay “resolve” u otros endpoints, el frontend los invoca directamente).
- No cambiar el layout general del Workbench fuera de lo estrictamente necesario.
- No introducir cache persistente o almacenamiento durable; el estado es por sesión de UI.

## Decisions

### 1) Mantener `AppTreeTable` como componente “headless-ish”
**Decisión:** `AppTreeTable` seguirá siendo un componente UI que puede operar con:
- `rows` (modo controlado por el consumidor), o
- `load()` (modo de carga inicial), y se extenderá para soportar “fetch children” al expandir nodos.

**Rationale:** evita acoplar el componente UI a endpoints específicos o a un dominio (Gestor Documental). La lógica de consumo y tipado de contratos vive en `services/hooks` del módulo consumidor.

**Alternativas consideradas:**
- (A) Meter axios/servicios dentro del componente: descartado por acoplamiento y dificultad de testear/reusar.
- (B) Duplicar componente “TreeTableGD”: descartado por deuda y divergencia.

### 2) Adaptador de contrato: `Rows[]` -> `AppTreeTableRow[]`
**Decisión:** el consumidor implementa un adaptador tipado que convierte el response del backend a filas del componente.

**Rationale:** el backend define columnas dinámicas vía `Values`, y meta funcional vía `Meta`. El adaptador es el lugar correcto para:
- mapear `RowId`
- derivar `label`/`cells`
- mapear `hasChildren` desde `Meta.HasChildren`
- conservar payload de dominio (por ejemplo `Meta.DocumentId`, `Meta.NombreGabinete`, `Meta.NodeType`) para acciones.

### 3) Lazy-load jerárquico con “contract safe inputs”
**Decisión:** el expand/collapse dispara una función `loadChildren(row)` provista por el consumidor, la cual ejecuta el query con:
- `ViewMode="hierarchical"`
- `ParentRowId=row.rowId`
- `ParentNodeType=row.meta.nodeType` (o equivalente)
- `Level=nextLevel`

**Rationale:** el componente no debe conocer cómo se calcula `Level` ni qué `NodeType` aplica; el contrato es del dominio.

### 4) Acciones: `action` retorna contrato y el frontend ejecuta el “resolve”
**Decisión:** para “ver_documento”, el flujo recomendado es:
1) `POST .../action` con `{ActionId:"ver_documento", RowId, NodeType, Payload{IdDocumento, NombreGabinete}}`
2) si el backend retorna `DocumentResolveRequest`, el frontend invoca directamente `POST /api/.../visualizacion/resolve`.

**Rationale:** mantiene el frontend como orquestador, evita patrones API->API, y desacopla la evolución del backend.

### 5) Manejo de error funcional vs. error HTTP
**Decisión:** diferenciar:
- HTTP no-2xx: error técnico (mostrar mensaje genérico + reintentar).
- `200` con `success=false`: error funcional/validación (mostrar `errors[0].errorMessage` / `message`).

**Rationale:** consistente con contratos existentes (wrapper `success/message/errors`) y evita UX confuso.

## Risks / Trade-offs

- **[Riesgo] Columnas dinámicas cambian en backend** → Mitigación: renderizar `Values` por claves/orden provisto por configuración (si existe) y fallback a orden estable; pruebas con respuestas mock.
- **[Riesgo] Jerarquía inconsistente (HasChildren true pero no retorna hijos)** → Mitigación: tratar como leaf tras primer fetch vacío y no romper UI.
- **[Riesgo] Regresiones en Workbench layout** → Mitigación: mantener cambios CSS aislados al rail de Listado y tests de montaje.
- **[Riesgo] Acoplamiento accidental a Gestor Documental** → Mitigación: tipos/servicios del contrato viven en el módulo consumidor; `AppTreeTable` expone contratos UI genéricos.

## Migration Plan

1) Extender `AppTreeTable` (sin breaking changes) para:
   - soportar carga de hijos al expandir (callbacks)
   - exponer estado `expanded/loadingChildren`
2) Implementar servicios/hooks en el módulo consumidor para:
   - `query` inicial
   - `query` hijos
   - `action`
   - `resolve` (si aplica)
3) Conectar `DocumentosWorkbench` para usar `load()` real (dejar de pasar `rows={[]}`).
4) Agregar tests:
   - `AppTreeTable`: expand con lazy-load y render de hijos
   - `DocumentosWorkbench`: integra hook mock sin afectar el visor
5) Documentar en OpenSpec:
   - Spec de contrato (request/response + reglas)
   - Evidencia de tests ejecutados

Rollback:
- Revert commit(s) del ticket en `feature/SCRUMCORE-215` o deshabilitar el loader del Workbench volviendo a `rows` estático.

## Open Questions

- ¿Qué contrato exacto se usará en esta iteración (por ejemplo SCRUM-205 `ListaDocumentosRadicados`), y cuál es el `TableId` estándar para acciones?
- ¿Cómo se obtiene `jwt`/claims en el cliente para este dominio (manejador actual vs. wrapper central)?
- ¿Se requiere soporte de “EnablePagination/EnableColumnFilters” en UI o solo respetar flags (off) inicialmente?

