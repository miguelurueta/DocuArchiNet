## Context

El ticket `SCRUMCORE-161` corresponde a `04-FE` y cubre la integracion de
`AppEditorPdf` en `GestionRespuesta` como consumidor principal, reemplazando el
uso directo de `AppEditor` sin cambiar el comportamiento funcional del flujo.

La propuesta autogenerada usa naming inconsistente (`AppAppeditorpdf04Fe`) y
ruta no canonica. Este diseno fija:
- nombre oficial: `AppEditorPdf`
- ubicacion shared: `src/app/Components/UI/AppEditorPdf/`
- modulo consumidor: `src/modules/gestionCorrespondencia/...`

Estado actual:
- `GestionRespuestaMainTabContent` usa `AppEditor` + `AppEditorSaveAction` +
  `useAppEditorSaveState`.
- `AppEditorPdf` ya existe en shared con compatibilidad de contrato.

## Goals / Non-Goals

**Goals:**
- Integrar `AppEditorPdf` en `GestionRespuestaMainTabContent`.
- Mantener UI/UX y contrato de guardado sin regresiones funcionales.
- Conservar separacion de responsabilidades: shared UI vs dominio del modulo.

**Non-Goals:**
- Cambiar reglas de negocio de `gestionCorrespondencia`.
- Introducir nuevas features del editor.
- Alterar serializacion de contenido o backend.

## Decisions

### 1) Sustitucion por alias canonico en consumidor
**Decision:** Reemplazar imports/uso de `AppEditor` por `AppEditorPdf` y sus
companion APIs (`AppEditorPdfSaveAction`, `useAppEditorPdfSaveState`).

**Rationale:** Alinea naming y trazabilidad de tickets `AppEditorPdf`.

### 2) Preservar contrato y layout del workbench
**Decision:** Mantener props y layout actuales en el contenedor del modulo
(`paginationMode`, `pageFormat`, `pageMargins`, `className`, `surfaceClassName`).

**Rationale:** Evita regresiones en experiencia y pruebas existentes.

### 3) Validar integracion con pruebas focalizadas del modulo
**Decision:** Ajustar tests de `GestionRespuestaMainTabContent` para reflejar
`AppEditorPdf` como superficie principal manteniendo escenarios previos.

**Rationale:** Garantiza migracion segura en el consumidor real.

## Risks / Trade-offs

- [Riesgo] Cambio de imports rompe build por exports no alineados
  -> Mitigacion: usar exports oficiales de `AppEditorPdf/index.ts`.

- [Riesgo] Regresion visual en surface del workbench
  -> Mitigacion: preservar clases y props actuales del contenedor.

- [Trade-off] Wrapper `AppEditorPdf` delega motor a `AppEditor`
  -> Aceptado para migracion incremental y bajo riesgo.

## Migration Plan

1. Crear artefactos OpenSpec (`design/specs/tasks`) para `04-FE`.
2. Actualizar `GestionRespuestaMainTabContent` a `AppEditorPdf`.
3. Ajustar pruebas del modulo consumidor.
4. Ejecutar suite focal de tests del modulo.
5. Archivar change y abrir PR con comentario Jira.

Rollback:
- Revertir consumidor a `AppEditor` previo si aparece regresion critica.

## Open Questions

- Se renombraran en tickets futuros las clases CSS `embeddedAppEditor*` a
  `embeddedAppEditorPdf*` o se mantiene para compatibilidad visual?
