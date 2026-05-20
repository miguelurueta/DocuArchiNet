## Context

En `SCRUMCORE-214` se introdujo `AppTreeTable` como componente reutilizable (UI) y se integró en `DocumentosWorkbench` dentro del rail de “Listado”, garantizando no interferir con `AppVisorEmbedPdf` ni otros plugins.

`SCRUMCORE-215` evoluciona `AppTreeTable` para soportar renderización e integración “backend-driven” basada en un contrato `query/action` (tipo SCRUM-205 ListaDocumentosRadicados), manteniendo compatibilidad hacia atrás y evitando impactos colaterales en el Workbench.

Contrato objetivo (referencia funcional):
- `POST /api/GestorDocumental/Documentos/ListaDocumentosRadicados/query`
- `POST /api/GestorDocumental/Documentos/ListaDocumentosRadicados/action`
- `POST /api/gestor-documental/documentos/visualizacion/resolve` (frontend directo)

Restricciones clave:
- TypeScript estricto, ESM y Vite.
- Tests con Vitest/Testing Library.
- No “mitigar” problemas ocultando elementos con CSS si la causa está en pipeline/composición.
- No afectar comportamiento de plugins y operaciones existentes (especialmente `AppVisorEmbedPdf`).

## Goals / Non-Goals

**Goals:**
- Permitir que `AppTreeTable` renderice datos provenientes de `query` (wrapper `success/message/data/errors`) mediante un adaptador del consumidor.
- Soportar modo jerárquico (`ViewMode=hierarchical`) con lazy-load de hijos al expandir nodos.
- Habilitar acciones por fila mediante `action` (ej. `ver_documento`) y orquestación frontend de `visualizacion/resolve`.
- Mantener separación limpia: `AppTreeTable` (UI genérica) vs. `services/hooks` del módulo consumidor (Gestor Documental / Gestión Correspondencia).
- Integrar el listado en `DocumentosWorkbench` sin modificar el visor ni otros rails.

**Non-Goals:**
- No reescribir `AppTreeTable` como un “AppTable v2”.
- No implementar consumo API->API para el flujo `ver_documento`.
- No añadir dependencias grandes ni un nuevo grid system.
- No introducir persistencia de cache; el estado es en memoria del componente/sesión.

## Decisions

### 1) `AppTreeTable` permanece agnóstico al dominio
**Decisión:** `AppTreeTable` no conoce endpoints ni DTOs de negocio. El consumo se hace en hooks/servicios del módulo consumidor, que entrega al componente:
- `load()` para carga inicial
- `loadChildren(row)` (o equivalente) para lazy-load jerárquico
- `onRowAction(actionId, row)` para acciones (opcional)

**Rationale:** reusabilidad, testeo y evita acoplamiento a Gestor Documental.

**Alternativas consideradas:**
- Servicios axios dentro de `AppTreeTable`: descartado por acoplamiento y dificultad de pruebas.
- Duplicar un componente específico para Gestor Documental: descartado por deuda técnica.

### 2) Adaptador explícito: `Rows[]` -> `AppTreeTableRow[]`
**Decisión:** el consumidor implementa un adaptador tipado para transformar:
- `RowId` -> id técnico de UI
- `Values` -> celdas/columnas renderizadas
- `Meta` -> `hasChildren`, `nodeType`, payload para acciones (DocumentId, NombreGabinete, etc.)

**Rationale:** `Values` y `Meta` pueden variar por configuración/contrato; el adaptador aisla cambios.

### 3) Lazy-load jerárquico via callback (sin lógica de dominio en UI)
**Decisión:** al expandir una fila con `HasChildren=true`, `AppTreeTable` invoca `loadChildren(row)` provisto por el consumidor. Ese callback ejecuta `query` con:
- `ViewMode="hierarchical"`
- `ParentRowId=<RowId del padre>`
- `ParentNodeType=<NodeType del padre>`
- `Level=<nivel siguiente>`

**Rationale:** el cálculo de `Level` y la semántica de `NodeType` pertenecen al dominio, no al componente UI.

### 4) Acciones: `action` -> (opcional) `visualizacion/resolve`
**Decisión:** para `ver_documento`:
1) frontend llama `action` con `RowId`, `NodeType`, `Payload.{IdDocumento, NombreGabinete}`
2) si `action` retorna `DocumentResolveRequest`, frontend llama `visualizacion/resolve` directamente

**Rationale:** sin API->API, y compatible con el contrato (backend devuelve “qué resolver”, frontend ejecuta).

### 5) Error funcional vs. error técnico
**Decisión:** distinguir:
- HTTP no-2xx / network: error técnico (mensaje genérico + reintento)
- HTTP 200 con `success=false`: error funcional (mostrar `errors[0].errorMessage` o `message`)

## Risks / Trade-offs

- **[Riesgo] Columnas dinámicas cambian en backend** → Mitigación: adaptador controla mapping; tests con respuestas mock.
- **[Riesgo] Inconsistencia HasChildren** → Mitigación: si hijos retornan vacío, tratar nodo como hoja sin loops.
- **[Riesgo] Regresiones de layout en Workbench** → Mitigación: estilos aislados al rail Listado; test de montaje.
- **[Riesgo] Acoplamiento accidental a Gestor Documental** → Mitigación: contratos/DTOs viven en módulo consumidor, no en UI.

## Migration Plan

1) Extender `AppTreeTable` sin breaking changes:
   - expand/collapse
   - lazy-load children (callbacks)
   - estados: `loading`, `error`, `empty`, `loadingChildren`
2) Implementar servicios/hooks del contrato en el módulo consumidor:
   - `query` inicial
   - `query` hijos
   - `action`
   - `visualizacion/resolve`
3) Conectar `DocumentosWorkbench` “Listado” al hook (dejar `rows={[]}` solo como fallback).
4) Tests focales (Vitest/RTL) + evidencia en documentación.

Rollback:
- Revertir commits del ticket en `feature/SCRUMCORE-215`, o deshabilitar el loader volviendo temporalmente a `rows` estático.

## Open Questions

- Confirmar `TableId` estándar (ej. `InboxListaRadicados`) y conjunto de `ActionId` soportadas.
- Confirmar si `IncludeConfig=true` es requerido siempre en primera carga.
- Definir orden/selección de columnas cuando `Values` es dinámico (si existe `config` en response).

