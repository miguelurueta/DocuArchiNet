## Context

`SCRUMCORE-163` corresponde al `06-FE` del roadmap de `AppEditorPdf` con foco
en **paginacion visual base**. El motor `AppEditor` ya implementa esta
capacidad (modo `visual`, formato/orientacion/margenes), pero `AppEditorPdf`
como wrapper shared todavia no define un baseline explicito de ese contrato.

El objetivo de esta fase en `AppEditorPdf` es formalizar una configuracion base
de paginacion visual manteniendo compatibilidad de API y capacidad de override.

## Goals

- Definir baseline por defecto de paginacion visual en `AppEditorPdf`.
- Preservar comportamiento opt-in/override desde consumidores.
- Fortalecer pruebas unitarias del wrapper para el contrato `06-FE`.

## Non-Goals

- No modificar el motor interno de `AppEditor` ni su layout CSS.
- No introducir page-breaks estructurales ni metadata persistida en HTML.
- No alterar integraciones de `GestionRespuesta` fuera del contrato ya existente.

## Decisions

### Decision 1: Defaults de paginacion visual en wrapper

**Decision:** `AppEditorPdf` aplicara por defecto:
- `paginationMode: "visual"`
- `pageFormat: "A4"`
- `pageOrientation: "portrait"`
- `pageMargins: { top: 96, right: 72, bottom: 96, left: 72 }`

**Rationale:** `AppEditorPdf` representa la variante orientada a experiencia
tipo documento/PDF; por ello el baseline debe salir activado sin repetir props
en cada consumidor.

### Decision 2: Override explicito preservado

**Decision:** El wrapper mantendra la posibilidad de sobrescribir cualquier
prop de paginacion (`paginationMode`, orientacion, margenes) desde el consumo.

**Rationale:** Evita acoplar a un unico modo y mantiene compatibilidad con
escenarios que requieran desactivar paginacion visual.

### Decision 3: Validacion por contrato unitario

**Decision:** Las pruebas de `AppEditorPdf` verificaran tanto defaults como
overrides, ademas del contrato de accesibilidad previo.

**Rationale:** Garantiza que el wrapper no rompa su responsabilidad de
adaptador entre consumidores y `AppEditor`.

## Risks & Mitigations

- [Riesgo] Cambio visual inesperado en consumidores que no pasaban props de paginacion.
  - Mitigacion: permitir override inmediato con `paginationMode="none"` y dejar contrato claro en pruebas.
- [Riesgo] Regresion de API en forwarding de props.
  - Mitigacion: mantener pruebas existentes de contrato controlado/accesibilidad y ampliar con `06-FE`.

## Implementation Plan

1. Completar artefactos OpenSpec (`design/spec/tasks`) para `app-appeditorpdf-06-fe`.
2. Implementar defaults de paginacion visual base en `AppEditorPdf`.
3. Extender pruebas unitarias del wrapper para defaults y overrides.
4. Ejecutar suite focal de `AppEditorPdf`.
