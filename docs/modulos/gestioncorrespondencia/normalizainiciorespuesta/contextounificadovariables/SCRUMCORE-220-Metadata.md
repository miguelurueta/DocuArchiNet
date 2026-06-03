# SCRUMCORE-220 - Metadata

## Ticket

- ID: SCRUMCORE-220
- Nombre: Contexto transversal unificado de GestionRespuesta
- Fecha: 2026-06-03
- Autor: Codex
- Version: 1.0.2

## Alcance

Refactor del contexto documental de `GestionRespuesta` para centralizar estado transversal compartido:

- `idTareaWf`
- `radicado`
- `idRespuestaRadicado`
- `nombreGabinete`
- `gabineteLoading`
- `gabineteError`
- `reloadGabinete`
- `files/setFiles`

## Control de cambios

| Version | Fecha | Cambio |
| --- | --- | --- |
| 1.0.0 | 2026-06-03 | Implementacion inicial de contexto transversal documental, gabinete idempotente, wiring desde estructura por tarea, pruebas y documentacion enterprise. |
| 1.0.1 | 2026-06-03 | Cierre de revision final: se confirma diff sin cambios UI ni logica de negocio fuera de alcance, el contexto no importa axios y la interpretacion de errores queda estructural sin exponer detalles HTTP a consumidores. |
| 1.0.2 | 2026-06-03 | Commit/push de implementacion y apertura de PR `#269` con resumen tecnico, pruebas ejecutadas, riesgos y documentacion generada. |

## Referencias cruzadas

OpenSpec:

- `openspec/changes/scrumcore-220-implementacion-contexto-trasversal-unificado-gestion-respuesta/proposal.md`
- `openspec/changes/scrumcore-220-implementacion-contexto-trasversal-unificado-gestion-respuesta/design.md`
- `openspec/changes/scrumcore-220-implementacion-contexto-trasversal-unificado-gestion-respuesta/specs/implementacion-contexto-trasversal-unificado-gestion-respuesta/spec.md`
- `openspec/changes/scrumcore-220-implementacion-contexto-trasversal-unificado-gestion-respuesta/tasks.md`

Codigo:

- `src/modules/gestionCorrespondencia/routes/GestionCorrespondenciaRoute.tsx`
- `src/modules/gestionCorrespondencia/pages/GestionRespuesta.tsx`
- `src/modules/gestionCorrespondencia/context/GestionRespuestaDocumentosContext.tsx`
- `src/modules/gestionCorrespondencia/hooks/useGestionRespuestaDocumentos.ts`
- `src/modules/gestionCorrespondencia/hooks/useGestionRespuestaDocumentosTable.ts`
- `src/modules/gestionCorrespondencia/services/solicitaGabineteRadicadoWorkflow.service.ts`

Tests:

- `src/modules/gestionCorrespondencia/tests/useGestionRespuestaDocumentos.test.tsx`
- `src/modules/gestionCorrespondencia/tests/useGestionRespuestaDocumentosTable.test.tsx`

Docs:

- `SCRUMCORE-220-Arquitectura.md`
- `SCRUMCORE-220-Implementacion-Detallada.md`
- `SCRUMCORE-220-Integracion-BackEnd.md`
- `SCRUMCORE-220-Pruebas.md`
- `SCRUMCORE-220-Metadata.md`

Pull request:

- `https://github.com/miguelurueta/DocuArchiCore.react/pull/269`

## Resultado de pruebas

- `npx vitest run src/modules/gestionCorrespondencia/tests/useGestionRespuestaDocumentos.test.tsx src/modules/gestionCorrespondencia/tests/useGestionRespuestaDocumentosTable.test.tsx`: OK.
- `npx vitest run src/modules/gestionCorrespondencia/tests/DocumentosWorkbench.test.tsx`: OK.
- `npx vitest run src/modules/gestionCorrespondencia/routes/GestionCorrespondenciaRoute.spec.test.tsx src/modules/gestionCorrespondencia/tests/GestionRespuestaMainTabContent.test.tsx src/modules/gestionCorrespondencia/tests/DocumentosWorkbench.test.tsx`: parcial; `DocumentosWorkbench` OK, ruta/main-tab conservan fallos de test/mocks no vinculados a errores TypeScript del cambio.
- `npm run build`: OK.
- `npx openspec validate scrumcore-220-implementacion-contexto-trasversal-unificado-gestion-respuesta --strict`: OK.

## Observaciones

- `radicado` e `idRespuestaRadicado` se cablean desde `GestionCorrespondenciaRoute`, usando `estrucTuraRespuesta` ya normalizado por `useEstructuraRespuestaIdTarea`.
- El contexto queda limitado a estado documental transversal para evitar god context.
- `EstadoExistenciaRadicado` se usa solo como criterio interno para `gabineteError`; no se expone como contrato publico del contexto.
- El provider no importa axios ni llama endpoints directamente; usa el service tipado y clasifica errores como `unknown` con guards estructurales.
