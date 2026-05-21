## Context

`DocumentosWorkbench` debe consumir un listado jerárquico (tree) de documentos impulsado por backend (backend-driven), sin reimplementar una tabla ni acoplarse a detalles de `AppTable`.

En el repo ya existe `AppTreeTable` como wrapper reusable sobre `AppTable` (refactor de `SCRUMCORE-216`). Este ticket integra `DocumentosWorkbench` con **SCRUM-205 ListaDocumentosRadicados** para:
- eliminar hardcode/listas vacías
- soportar acción primaria `ver_documento` (click principal)
- soportar menú secundario **dinámico** por fila (backend-driven)
- integrar con `AppVisorEmbedPdf`
- preservar totalmente layout visor + rail + responsive/overlay

Compatibilidad obligatoria (SCRUM-209):
- Mantener compatibilidad con SCRUM-205 (sin ruptura de rutas ni envelope).
- Consumir `flatDocuments` como vista simplificada (label principal + acciones), sin depender de columnas legacy no garantizadas.
- El label documental se considera **resuelto por backend** (TIPODOCUMENTO o fallback oficial `DOC {ID}`); el frontend no recalcula el fallback.

## Goals / Non-Goals

### Goals
- `DocumentosWorkbench` orquesta UI (layout + wiring + estado documento activo) sin lógica de negocio.
- Query jerárquica SCRUM-205 (root + lazy children) vía hook/service.
- Mapping DTO backend -> modelos UI vía adapters (sin consumir DTOs en el componente).
- Columnas/acciones backend-driven (sin hardcode en Workbench).
- Integración visor: actualizar documento activo **solo** si `ver_documento` OK; si falla, no cambiar.
- TypeScript estricto (sin `any`), cambios aislados y sin breaking changes.
- Tests con tag `[SPEC:APPTREETABLE-217]` + evidencia documentada.

### Non-Goals
- Rediseñar el layout del workbench (se preserva).
- Reescribir `AppTable`/`AppTreeTable`.
- Introducir state-management nuevo o librerías grandes.
- Cambiar el contrato SCRUM-205 desde frontend.

## Decisions

### 0) Precondición obligatoria (bloqueante)
`AppTreeTable` debe soportar (sin hacks):
- acción primaria: `onSelectRow`, `onCellClicked`
- acción secundaria: `onActionTriggered`
- render interno basado en `AppTable`

Si no se cumple: detener implementación, reportar blocker, no introducir workaround temporal.

### 1) Clean Architecture (separación estricta)
Responsabilidades:
- `DocumentosWorkbench`: layout + wiring + estado documento activo + integración visor/tree.
- `hooks`: loading/error/empty + orquestación query/actions + caching.
- `services`: HTTP (NO axios directo en `DocumentosWorkbench`).
- `adapters`: request mapper + response adapter + action mapper (DTO -> UI, metadata, acciones).

### 2) Contrato SCRUM-205 como fuente de verdad
Contrato obligatorio (resumen):
- Query request: `ViewMode`, `Page`, `PageSize`, `SortDir`, `ParentRowId`, `ParentNodeType`, `Level`, `IncludeConfig`, `EnablePagination`, `EnableColumnFilters`.
- Query response: `Rows[].RowId`, `Rows[].Values`, `Rows[].Meta.{NodeType,ParentId,HasChildren,DocumentId,NombreGabinete}` (+ `Config/Columns` cuando aplique).
- Action request: `ActionId`, `RowId`, `NodeType`, `Payload.{IdDocumento,NombreGabinete}`.

Compatibilidad SCRUM-209 (delta vs SCRUM-205, obligatoria):
- `ViewMode` puede ser `hierarchical` o `flatDocuments`.
- Regla por modo:
  - `flatDocuments`: enviar `ParentRowId=null`, `ParentNodeType=null`, `Level=1` (vista simplificada).
  - `hierarchical`: usar `ParentRowId` y `Level` para expansión de nodos.
- En `flatDocuments` la UI MUST NOT asumir columnas legacy removidas (p.ej. `PAG`, `ESTADO_FIRMA_DIGITAL`, etc.).
- El label documental (columna principal) debe mostrarse tal como lo resuelve backend:
  - si `TIPODOCUMENTO` no vacío -> usar ese valor
  - si `TIPODOCUMENTO` vacío -> backend envía fallback oficial `DOC {ID}`

### 3) Estrategia de carga incremental
`DocumentosWorkbench` usa:
- `load()` para filas raíz
- `loadChildren(row)` para hijos bajo demanda (cuando `HasChildren=true`)

### 4) Columnas/acciones 100% backend-driven (sin hardcode)
Prioridad de metadata:
1) `Config` backend (Dynamic UI) -> columnas/acciones para tabla (sin duplicar lógica existente; reutilizar mapeos donde aplique).
2) `Columns` backend -> `columns` (keys) para render de `Values`.
3) Fallback: inferencia desde `Values` solo si no hay metadata.

Regla adicional (SCRUM-209):
- En `flatDocuments`, el adapter SHOULD limitar la UI a la columna principal (label) + acciones backend-driven (evitar render de columnas irrelevantes/legacy incluso si el backend envía `Values` extensos).

### 5) Integración del visor PDF (resiliente)
- `ver_documento` resuelve `fileUrl` (y metadata necesaria) vía hook/service.
- `DocumentosWorkbench` solo mantiene estado normalizado (p.ej. `activeFileUrl`) y lo pasa a `AppVisorEmbedPdf`.
- En error de `ver_documento`: no se cambia el documento activo.

## Risks / Trade-offs
- Variabilidad de columnas/acciones backend-driven: mitigar con tests de integración y mocks tipados.
- Árbol grande: evitar recomputaciones; memoizar handlers y mantener estabilidad de filas.
- Desincronización árbol/visor: mutar estado activo solo en OK; mantener selección estable.
- Acciones dinámicas: centralizar mapping en adapters; no duplicar Dynamic UI.

## Migration Plan
1) Confirmar precondición de `AppTreeTable` (eventos + wrapper `AppTable`).
2) Implementar service SCRUM-205 (query + action + resolve).
3) Implementar adapters: request/response/action.
4) Implementar hook de orquestación (root + children + actions).
5) Reemplazar hardcode en `DocumentosWorkbench` preservando layout.
6) Agregar tests unitarios + integración UI; documentar evidencia.

Rollback: revertir cambios del módulo `gestionCorrespondencia` sin afectar `AppTable/AppTreeTable`.

## Open Questions
- Shape exacto de `Config`/`Columns` del backend en producción (¿keys, headers, orden?).
- Reglas de permisos para acciones secundarias (¿cómo se habilitan/ocultan por fila?).
- ¿Modo por defecto en `DocumentosWorkbench` para SCRUMCORE-217?: `flatDocuments` vs `hierarchical` (recomendado: `flatDocuments` cuando la UI sea “listado simplificado por radicado/tarea”).
