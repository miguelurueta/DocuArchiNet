# SCRUM-241 Metadata

## Ticket

- Jira: SCRUMCORE-241
- Titulo: MODULO-REUSABLE-DIGITALIZACIONDOCUMENTAL- WORKBENCH-GRAFICO-CAPTURA-03-FE
- Fecha: 2026-06-10
- Version: FE-03

## Cambio

Se implemento el workbench grafico de captura dentro de `DigitalizacionDocumentalModal`, integrado con los hooks existentes de estado documental y scanner.

## Archivos Principales

- `src/modules/digitalizacion/components/DigitalizacionDocumentalModal/DigitalizacionDocumentalModal.tsx`
- `src/modules/digitalizacion/components/DigitalizacionDocumentalModal/DigitalizacionDocumentalModal.module.css`
- `src/modules/digitalizacion/components/DigitalizacionDocumentalModal/DigitalizacionDocumentalModal.test.tsx`
- `src/modules/digitalizacion/types/digitalizacion.types.ts`

## Referencias Cruzadas

- SCRUMCORE-239: contrato inicial del modulo reusable.
- SCRUMCORE-240: adapter Dynamsoft y hook scanner.
- OpenSpec: `openspec/changes/scrumcore-241-modulo-reusable-digitalizaciondocumental-workbench-grafico-captura-03-fe`.

## Evidencia

- ESLint modulo digitalizacion: PASS.
- Vitest modulo digitalizacion: PASS.
- OpenSpec CLI: no disponible localmente.
