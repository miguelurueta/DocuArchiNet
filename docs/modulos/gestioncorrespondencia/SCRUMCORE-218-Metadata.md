# SCRUMCORE-218 - Metadata

## Ticket
- `SCRUMCORE-218`

## Autor
- `gerencia@contasoftcompany.com`

## Fecha
- 2026-05-21

## Resumen técnico

Normalización del contrato frontend para `DocumentosWorkbench` asegurando compatibilidad con respuestas backend en `data.Config` y `data` directo, preservación de acciones (`CellActions` + `MenuActions`) y ejecución consistente de `onActionTriggered` con `TableId`/payload efectivos.

## Control de cambios (archivos principales)

- `src/modules/gestionCorrespondencia/adapters/documentosWorkbenchResponseAdapter.ts`
- `src/modules/gestionCorrespondencia/hooks/useGestionRespuestaDocumentosTable.ts`
- `src/modules/gestionCorrespondencia/adapters/documentosWorkbenchActionMapper.ts`
- `src/modules/gestionCorrespondencia/adapters/documentosWorkbenchResponseAdapter.test.ts`
- `src/modules/gestionCorrespondencia/adapters/documentosWorkbenchActionMapper.test.ts`

## Specs

- OpenSpec change:
  - `openspec/changes/scrumcore-218-normalizacion-contrato-action/`
- Design:
  - `openspec/changes/scrumcore-218-normalizacion-contrato-action/design.md`
- Spec:
  - `openspec/changes/scrumcore-218-normalizacion-contrato-action/specs/normalizacion-contrato-action/spec.md`
- Tasks:
  - `openspec/changes/scrumcore-218-normalizacion-contrato-action/tasks.md`

## Tests

Unit:
- `src/modules/gestionCorrespondencia/adapters/documentosWorkbenchResponseAdapter.test.ts`
- `src/modules/gestionCorrespondencia/adapters/documentosWorkbenchActionMapper.test.ts`

## Evidencia de ejecución

- `npm.cmd test -- src/modules/gestionCorrespondencia/adapters/documentosWorkbenchResponseAdapter.test.ts` (PASS)
- `npm.cmd test -- src/modules/gestionCorrespondencia/adapters/documentosWorkbenchActionMapper.test.ts` (PASS)

## Riesgos residuales

- Validar en ambiente integrado que todas las variantes reales de `CellActions/MenuActions` del backend sigan el mismo shape esperado por Dynamic UI.
- Si aparece un nuevo alias de identificador de documento, extender la lectura en hook manteniendo prioridad explícita.
