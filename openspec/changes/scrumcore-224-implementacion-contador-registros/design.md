## Context

`DocumentosWorkbench` ya tiene:
- Documento activo por click.
- Seleccion multiple por checkbox.
- Lista documental dinamica con mutaciones runtime (acciones como `agregar_item` / `eliminar_item`, con recargas posteriores).

Actualmente no existe un contador documental contextual sincronizado automaticamente con el estado real del listado.

## Goals / Non-Goals

**Goals**
- Mostrar contador contextual de documentos (`totalDocumentsCount`).
- Mostrar contador de seleccionados (`selectedDocumentsCount`).
- Garantizar sincronizacion automatica con cambios en `rows/treeRows/selectedRows`.
- Mantener comportamiento actual de documento activo, seleccion, acciones y Dynamic UI.

**Non-Goals**
- Cambios backend/endpoints/contratos.
- Cambios en scroll/header (ya resuelto en SCRUMCORE-223).
- Cambios globales en `AppTable`/`AppTreeTable`.
- Lógica manual de incremento/decremento (`contador++`, `contador--`) o dependencia en `actionId`.

## Architectural Rules

1. Derivacion automatica obligatoria
- El contador se calcula desde estado normalizado (`rows`/`treeRows` y seleccion actual).
- Implementacion via estado derivado (`useMemo`/selectors), no via eventos de mutacion.

2. Source of truth
- Prioridad inicial de total:
  1. `Total` (si backend lo entrega en payload conocido)
  2. `TotalRecords` (si backend lo entrega)
  3. `rows.length`
- Despues de mutaciones runtime, fuente principal: `rows/treeRows` actuales.

3. Separacion de responsabilidades
- `DocumentosWorkbench`: renderiza contador contextual.
- Hook(s) de `gestionCorrespondencia`: exponen estado derivado de conteo y seleccion.
- `AppTreeTable`/`AppTable`: no incorporan logica de negocio del contador.

4. Alcance local
- Cambios limitados a:
  - `src/modules/gestionCorrespondencia/components/documentosWorkbench/*`
  - `src/modules/gestionCorrespondencia/hooks/*`
  - tests del modulo.
- Sin efectos colaterales en otros consumidores de tabla.

## Technical Design

### 1) Modelo derivado de conteo

Se añadira en el hook de workbench un modelo derivado:
- `totalDocumentsCount: number`
- `selectedDocumentsCount: number`

Ambos se calculan automaticamente desde estado actual:
- `totalDocumentsCount`: fallback backend/runtime según regla de prioridad.
- `selectedDocumentsCount`: cardinalidad de seleccion actual (no manual).

### 2) Wiring de seleccion

`DocumentosWorkbench` consumira un callback/estado de seleccion desde `AppTreeTable` (ya soportado por `AppTable`) para recalcular seleccionados de forma reactiva sin acoplarse a action ids.

### 3) Presentacion

Formato objetivo:
- `Documentos (N)`
- Opcional extendido: `Documentos (N) · Seleccionados (M)`

El contador debe ser:
- No invasivo.
- Estable en loading/error (sin flicker).
- Accesible y coherente con el estilo del panel.

## Risks / Mitigations

- Riesgo: desincronizacion por mezclar fuentes backend y runtime.
  - Mitigacion: politicas claras de prioridad y predominio de estado runtime tras mutacion.

- Riesgo: rerenders innecesarios del workbench.
  - Mitigacion: memoizacion de derivados, evitar estado duplicado mutable.

- Riesgo: romper semantica de documento activo o seleccion.
  - Mitigacion: no tocar flujos existentes; solo observar estado.

## Migration Plan

1. Extender hook con derivados `totalDocumentsCount` / `selectedDocumentsCount`.
2. Integrar contador en cabecera contextual de `DocumentosWorkbench`.
3. Conectar seleccion actual para derivar `selectedDocumentsCount` automatico.
4. Ajustar tests unitarios/integracion.
5. Generar paquete documental en `docs/Architecture/AppTreeTable/contadoregistros`.

## Open Questions

- Confirmar copy final visible:
  - `Documentos (N)` o `Documentos (N) · Seleccionados (M)`.
- Confirmar si contador de seleccionados debe mostrarse siempre o solo cuando `M > 0`.
