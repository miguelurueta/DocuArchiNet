# SCRUMCORE-221 - Metadata

## Ticket

- ID: SCRUMCORE-221
- Nombre: Refactor de useListaDocumentosRadicadosTreeTable para consumir contexto transversal de GestionRespuesta
- Fecha: 2026-06-03
- Autor: Codex
- Version: 1.0.0

## Alcance

El ticket refactoriza `useListaDocumentosRadicadosTreeTable` para eliminar la resolución local de gabinete y consumir `nombreGabinete` desde `GestionRespuestaDocumentosContext` (transversal).  
Mantiene contratos de AppTreeTable y no modifica acciones ni layout de workbench/visor.

## Control de cambios

| Version | Fecha | Cambio |
| --- | --- | --- |
| 1.0.0 | 2026-06-03 | Implementación inicial: consumo de contexto en `useListaDocumentosRadicadosTreeTable`, retirada de `getSolicitaGabinetePorTareaWorkflow` local, bloqueos funcionales por estado de gabinete y actualización de tests. |

## Referencias cruzadas

OpenSpec (pendiente de sincronizar si aplica al bloque completo):

- `openspec/changes` (si se genera nuevo change para SCRUMCORE-221)

Codigo:

- `src/modules/gestionCorrespondencia/hooks/useListaDocumentosRadicadosTreeTable.ts`
- `src/modules/gestionCorrespondencia/tests/useListaDocumentosRadicadosTreeTable.test.tsx`
- `docs/modulos/gestioncorrespondencia/normalizainiciorespuesta/actualizalistadocumentosradicadovargabinetecontrexto/SCRUMCORE-221-Arquitectura.md`
- `docs/modulos/gestioncorrespondencia/normalizainiciorespuesta/actualizalistadocumentosradicadovargabinetecontrexto/SCRUMCORE-221-Implementacion-Detallada.md`
- `docs/modulos/gestioncorrespondencia/normalizainiciorespuesta/actualizalistadocumentosradicadovargabinetecontrexto/SCRUMCORE-221-Integracion-BackEnd.md`
- `docs/modulos/gestioncorrespondencia/normalizainiciorespuesta/actualizalistadocumentosradicadovargabinetecontrexto/SCRUMCORE-221-Pruebas.md`

Git:

- Rama: `feature/SCRUMCORE-221` (cuando se ejecute el siguiente ciclo de merge)
- Commit implementacion código: `b106237` (`SCRUMCORE-221`).
- Estado del working tree tras documentación: limpio (previo a commit/push adicional de metadata).

## Resultado de pruebas

- `npx vitest run src/modules/gestionCorrespondencia/tests/useListaDocumentosRadicadosTreeTable.test.tsx`: OK (1 archivo, 4 tests).
- Validación adicional sugerida en cierre:
  - `npx vitest run src/modules/gestionCorrespondencia/tests/DocumentosWorkbench.test.tsx`
  - `npm run build`

## Observaciones

- Esta implementación depende de la existencia del contexto transversal completo realizado en SCRUMCORE-220 (`GestionRespuestaDocumentosProvider` y `useGestionRespuestaDocumentos`).
- Se respeta explícitamente el scope:
  - no se tocó UI,
  - no se cambió endpoint ni contratos de acción/query,
  - no se convirtió el contexto en god context.
