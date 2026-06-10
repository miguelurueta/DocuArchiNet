# SCRUMCORE-225 — Design: Ajuste visual de columnas (AppTreeTable / Workbench)

## Context
El `DocumentosWorkbench` consume `AppTreeTable` como wrapper sobre `AppTable`, y para columnas utiliza configuración backend-driven (Dynamic UI) que se transforma a `ColDef` (AG Grid) y luego a columnas consumibles por `AppTable`.

En este flujo, el Workbench puede renderizar más de dos columnas (por `Columns`/config) o dejar una columna secundaria con baja visibilidad por sizing no consistente. Este cambio busca una actualización **visual** enterprise: **renderizar solo 2 columnas y garantizar sizing estable**, sin cambiar comportamiento funcional (selección, acciones, events, integración visor) ni contratos backend.

Archivos relevantes hoy:
- `src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.tsx`
- `src/app/Components/UI/AppTreeTable/AppTreeTable.tsx`
- `src/app/Components/UI/AppTable/adapters/dynamicUiToAgGridColumns.ts`
- `src/app/Components/UI/AppTable/adapters/appGridToAppTableColumns.ts`
- `src/modules/gestionCorrespondencia/adapters/documentosWorkbenchResponseAdapter.ts`

Restricciones clave:
- No romper `AppTable`/`AppTreeTable` ni otros módulos.
- No hardcodear columnas en `DocumentosWorkbench`.
- Mantener compatibilidad SCRUM-205/SCRUM-209 (`flatDocuments` vista simplificada; label ya resuelto por backend).

## Goals / Non-Goals
**Goals**
- En el Workbench, renderizar exactamente 2 columnas visibles (primaria + secundaria).
- Aplicar sizing enterprise (flex/minWidth/truncado) para que la columna secundaria no quede invisibilizada.
- Mantener intactos eventos y comportamiento: `onSelectRow`, `onCellClicked`, `onActionTriggered`, selección múltiple, y actualización del visor PDF.
- No introducir cambios de contrato backend ni dependencia nueva.
- Acompañar con pruebas unitarias + Playwright de regresión.

**Non-Goals**
- Rediseño de UI del Workbench (layout visor/rail).
- Cambiar la lógica de selección/acciones o la semántica de `client_event`.
- Alterar el conjunto de acciones backend-driven.

## Decisions
### 1) Scoping del ajuste (evitar side-effects)
**Decisión**: aplicar el “two-column shaping + sizing preset” solo para el contexto Workbench mediante `tableId`.

**Rationale**: el mismo pipeline Dynamic UI es reutilizado por múltiples pantallas; el ajuste debe ser acotado. El Workbench utiliza `tableId` asociado a `ListaDocumentosRadicados` (ej. `InboxListaDocumentosRadicado`). Se implementará una regla del tipo:
- Si `tableId` pertenece al conjunto permitido (Workbench), entonces:
  - seleccionar 2 columnas,
  - aplicar sizing preset,
  - ocultar el resto.
- En otros `tableId`, no cambiar nada.

**Alternativas consideradas**
- (B) Override desde `DocumentosWorkbench` post-procesando `tableColumns`: descartado como opción principal por aumentar acoplamiento UI→config y duplicar lógica de columnas en capa de componentes.
- (C) Nuevo prop en `AppTreeTable` para “two-column mode”: descartado por impacto a API pública y superficie de regresión.

**Alcance explícito (para evitar ambigüedad)**
- El preset se aplica a `ViewMode=flatDocuments` del Workbench.
- `ViewMode=hierarchical` NO se ajusta en este ticket salvo que el backend entregue explícitamente un modo de “vista simplificada” equivalente (si no existe, se mantiene comportamiento actual).

### 2) Selección determinística de 2 columnas (backend-driven)
**Decisión**: seleccionar columnas en este orden:
1. **Primaria**: `TIPODOCUMENTO` si existe en el conjunto de columnas disponibles (SCRUM-209).
2. **Secundaria**: primera columna “válida” distinta a la primaria, derivada de la config real (sin inventar), aplicando reglas anti-legacy para `flatDocuments`.

Reglas de “columna válida”:
- existe en el set de columnas entregadas por Dynamic UI/config,
- no está en lista de columnas legacy prohibidas en `flatDocuments` (ej. `PAG`, `ESTADO_FIRMA_DIGITAL`, etc. según guía SCRUM-209),
- no es una columna técnica interna no visible (si existiera).

Lista inicial anti-legacy (flatDocuments) (se valida contra config real y puede crecer con evidencia):
- `PAG`
- `ESTADO_FIRMA_DIGITAL`
- `DBT`
- `TIPODOCUMENTO` (solo para evitar duplicar en secundaria)

Fallbacks:
- Si no existe secundaria válida, mantener solo primaria (y documentar el caso como limitación controlada; Playwright debe reflejar expectativa según config real).

### 3) Sizing preset enterprise (2 columnas visibles)
**Decisión**: aplicar `flex` + `minWidth` en las 2 columnas:
- Primaria: `flex: 2`, `minWidth: 280` (ajustable por evidencia en QA).
- Secundaria: `flex: 1`, `minWidth: 200`.

**Notas**
- Se mantendrá truncado/ellipsis en celdas para ancho estrecho.
- No se cambia sorting/filtering salvo que el backend-driven config lo requiera.

**Definición de “siempre visible”**
- Desktop/tablet (viewport estándar): 2 headers visibles sin requerir scroll horizontal.
- Viewports estrechos: se permite truncado; si el contenedor es físicamente menor que la suma de `minWidth`, el comportamiento permitido es mantener ambas columnas con truncado y/o permitir scroll horizontal sin romper el layout (esto se valida explícitamente en Playwright con viewport objetivo).

## Risks / Trade-offs
- [Config backend cambia columnas] → Mitigación: selección por reglas y fallback; pruebas unitarias cubren selector + lista anti-legacy.
- [Pantallas fuera de Workbench afectadas] → Mitigación: scoping estricto por `tableId`.
- [Viewport estrecho no permite 2 columnas sin scroll] → Mitigación: sizing con flex/minWidth y truncado; Playwright valida “2 headers visibles” en viewport estándar; documentar comportamiento en móvil si aplica.
- [Regresión en acciones/selección] → Mitigación: pruebas UI + Playwright de interacción (click primario y acción secundaria).

## Migration Plan
1. Implementar selector de 2 columnas + sizing preset con scoping por `tableId`.
2. Añadir tests unitarios del selector y del preset.
3. Añadir Playwright tests (Workbench) validando 2 columnas visibles + interacción básica.
4. Validar manualmente en responsive (desktop/tablet/móvil) sin cambios de layout.

Rollback:
- Revertir la regla por `tableId` (cambio acotado y aislado).

## Testing Strategy
- Unit (Vitest):
  - Selector de columnas: primaria `TIPODOCUMENTO` + secundaria válida + fallback.
  - Sizing preset aplicado solo cuando `tableId` corresponde a Workbench.
- Integration (Testing Library):
  - `DocumentosWorkbench` renderiza tabla con 2 headers (mock de config/columns) y mantiene estados `loading/error/empty`.
- Playwright:
  - 2 headers visibles en listado (Workbench).
  - Click primario actualiza visor (mock/fixture según suite existente).
  - Acción secundaria dispara handler sin romper selección/visor.
