# SCRUMCORE-263 - Metadata

## Jira

- Ticket: SCRUMCORE-263
- Nombre: CREA-COMPONENTE-APPPROGRESSBATCH
- Tipo: Tarea
- Prioridad: Medium
- Labels: APPPROGRESSBATCH, COMPONENTE, CREA

## Git

- Rama: `feature/SCRUMCORE-263`
- Commits previos de refinamiento:
  - `2f86069 feat(SCRUMCORE-263): proposal inicial OpenSpec`
  - `0750498 docs(SCRUMCORE-263): refine AppProgressBatch OpenSpec`
  - `a130fa7 docs(SCRUMCORE-263): require enterprise documentation`
- Commit de implementacion:
  - `0242cd2 feat(SCRUMCORE-263): implement AppProgressBatch`
- Push:
  - `feature/SCRUMCORE-263` actualizado en remoto.
- PR: pendiente si el flujo requiere abrirlo despues de revision.

## Archivos creados

- `src/app/Components/UI/AppProgressBatch/AppProgressBatch.tsx`
- `src/app/Components/UI/AppProgressBatch/AppProgressBatch.types.ts`
- `src/app/Components/UI/AppProgressBatch/AppProgressBatch.module.css`
- `src/app/Components/UI/AppProgressBatch/AppProgressBatch.test.tsx`
- `src/app/Components/UI/AppProgressBatch/README.md`
- `src/app/Components/UI/AppProgressBatch/index.ts`
- `docs/Architecture/AppProgressBatch/SCRUMCORE-263-Arquitectura.md`
- `docs/Architecture/AppProgressBatch/SCRUMCORE-263-Implementacion-Detallada.md`
- `docs/Architecture/AppProgressBatch/SCRUMCORE-263-Pruebas.md`
- `docs/Architecture/AppProgressBatch/SCRUMCORE-263-Metadata.md`

## Archivos modificados

- `src/app/Components/UI/index.ts`
- `openspec/changes/scrumcore-263-crea-componente-appprogressbatch/tasks.md` cuando se marque avance.

## Decisiones

- El componente no integra consumidores.
- La operacion concreta se inyecta por `processItem`.
- La cancelacion se limita a `AbortController`.
- Los resultados invalidos se tratan como error fatal.
- El resumen cancelado queda visible si el modal permanece abierto.
- `closeOnComplete` solo aplica a finalizacion exitosa, no a errores ni cancelaciones.
- El preview de items en cola es generico y usa `getItemLabel`; no conoce documentos ni upload.

## Confirmaciones de alcance

- Backend NO modificado.
- Endpoints NO modificados.
- Consumidores de negocio NO modificados.
- `AppUpload` NO modificado.
- Componente reusable sin dominio.
- Cancelacion con `AbortController`.
- Resultados stale ignorados mediante `runId`.
- Sin `any` nuevo en el contrato publico.

## Riesgos residuales

- El consumidor debe honrar `context.signal` para cancelacion efectiva de operaciones externas.
- El componente no puede garantizar rollback transaccional de servicios.
- La integracion futura con `AppUploadDocumental` debe tener su propio ticket y pruebas.

## Estado de tareas

Las tareas de OpenSpec fueron marcadas como completadas en:

```txt
openspec/changes/scrumcore-263-crea-componente-appprogressbatch/tasks.md
```

## Evidencia de validacion

- Tests especificos: `npx.cmd vitest run src/app/Components/UI/AppProgressBatch/AppProgressBatch.test.tsx` OK, 19 tests passed.
- TypeScript: `npx.cmd tsc --noEmit --pretty false` OK.
- OpenSpec: `npx.cmd openspec validate scrumcore-263-crea-componente-appprogressbatch --strict` OK.
- Diff check: `git diff --check` OK.
- Lint acotado: `npx.cmd eslint src/app/Components/UI/AppProgressBatch src/app/Components/UI/index.ts` OK.
- Lint global: ejecutado, falla por deuda previa fuera del alcance.
- Build global: ejecutado, falla por errores previos en `src/modules/digitalizacion`.
- Preview generico posterior: `AppProgressBatch` renderiza items en cola antes de iniciar.
- Typecheck posterior: `npx.cmd tsc --noEmit --pretty false` OK.
- Lint acotado posterior: `npx.cmd eslint src/app/Components/UI/AppProgressBatch/AppProgressBatch.tsx src/app/Components/UI/AppProgressBatch/AppProgressBatch.test.tsx` OK.
- Reversion de integracion Gestion: se retiro `AppProgressBatch` de `GestionRespuestaMainTabContent`; verificacion posterior `npx.cmd vitest run src/app/Components/UI/AppProgressBatch/AppProgressBatch.test.tsx src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaMainTabContent.test.tsx src/modules/gestionCorrespondencia/tests/GestionRespuestaMainTabContent.test.tsx --reporter verbose` OK, 3 archivos y 24 tests.
