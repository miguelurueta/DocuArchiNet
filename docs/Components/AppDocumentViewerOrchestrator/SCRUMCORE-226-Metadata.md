# SCRUMCORE-226 - Metadata

- Componente: `AppDocumentViewerOrchestrator`
- Ruta: `src/app/Components/UI/AppDocumentViewerOrchestrator/`
- Consumidor esperado: `AppVisorEmbedPdf` (consume estado runtime consolidado)
- Restricciones: sin backend changes, sin UI/permissions, sin persistencia de URLs temporales

## Artefactos OpenSpec actualizados

- `openspec/changes/scrumcore-226-implementacion-orquestador-documento-visor/proposal.md`
- `openspec/changes/scrumcore-226-implementacion-orquestador-documento-visor/design.md`
- `openspec/changes/scrumcore-226-implementacion-orquestador-documento-visor/specs/implementacion-orquestador-documento-visor/spec.md`
- `openspec/changes/scrumcore-226-implementacion-orquestador-documento-visor/tasks.md`

## Código agregado/modificado

Agregado:
- `src/app/Components/UI/AppDocumentViewerOrchestrator/AppDocumentViewerOrchestrator.types.ts`
- `src/app/Components/UI/AppDocumentViewerOrchestrator/AppDocumentViewerOrchestrator.adapter.ts`
- `src/app/Components/UI/AppDocumentViewerOrchestrator/AppDocumentViewerOrchestrator.service.ts`
- `src/app/Components/UI/AppDocumentViewerOrchestrator/useDocumentViewerOrchestrator.ts`
- `src/app/Components/UI/AppDocumentViewerOrchestrator/index.ts`
- `src/app/Components/UI/AppDocumentViewerOrchestrator/tests/AppDocumentViewerOrchestrator.adapter.test.ts`
- `src/app/Components/UI/AppDocumentViewerOrchestrator/tests/useDocumentViewerOrchestrator.test.tsx`

Modificado:
- `src/app/Components/UI/index.ts` (export del nuevo módulo)

## Metadata JIRA

- Ticket: `SCRUMCORE-226`
- Resumen: `IMPLEMENTACION-ORQUESTADOR-DOCUMENTO-VISOR`
- Autor: Miguel Angel Urueta Miranda (usuario Jira reportado por `jira:test` en este entorno)
- Fecha: 2026-05-25

## Control de cambios (resumen)

- Se añadió el módulo `AppDocumentViewerOrchestrator` (core sin UI) con soporte de cancelación y stale protection.
- Se añadieron unit tests para adapter + hook.
- Se añadieron docs en `docs/Components/AppDocumentViewerOrchestrator/`.

## Restricciones (confirmación explícita)

- Backend/endpoints: **NO** modificados.
- TypeScript: diseñado para uso estricto y sin `any` dentro del módulo.
- Persistencia: **NO** persistencia de `UrlTemporal*` en storage/caches.
- Permisos/UI: el orquestador **NO** contiene lógica de permisos, toolbar ni edición/anotaciones.
