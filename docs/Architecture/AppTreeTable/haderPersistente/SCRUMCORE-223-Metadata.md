# SCRUMCORE-223 - Metadata

## Identificacion

- Ticket: `SCRUMCORE-223`
- Tema: Header persistente y estrategia de scroll interno en `DocumentosWorkbench`
- Modulo: `gestionCorrespondencia`
- Componente principal: `AppTreeTable` (uso localizado desde `DocumentosWorkbench`)

## Autor y control

- Autor tecnico: Codex (asistente de implementacion)
- Fecha de documento: `2026-05-22`
- Version: `v1.0.0`
- Estado: En progreso (implementacion y documentacion listas; commit pendiente)

## Control de cambios

| Version | Fecha | Cambio | Autor |
|---|---|---|---|
| v1.0.0 | 2026-05-22 | Creacion de paquete de arquitectura, implementacion detallada, integracion backend y pruebas | Codex |

## Referencias cruzadas

- OpenSpec:
  - `openspec/changes/scrumcore-223-implementacion-header-perisitente/proposal.md`
  - `openspec/changes/scrumcore-223-implementacion-header-perisitente/design.md`
  - `openspec/changes/scrumcore-223-implementacion-header-perisitente/specs/implementacion-header-perisitente/spec.md`
  - `openspec/changes/scrumcore-223-implementacion-header-perisitente/tasks.md`
- Arquitectura:
  - `docs/Architecture/AppTreeTable/haderPersistente/SCRUMCORE-223-Arquitectura.md`
  - `docs/Architecture/AppTreeTable/haderPersistente/SCRUMCORE-223-Implementacion-Detallada.md`
  - `docs/Architecture/AppTreeTable/haderPersistente/SCRUMCORE-223-Integracion-BackEnd.md`
  - `docs/Architecture/AppTreeTable/haderPersistente/SCRUMCORE-223-Pruebas.md`

## Confirmaciones de alcance

- Backend: no modificado.
- Endpoints: no modificados.
- Contratos backend: no modificados.
- Dynamic UI: sin cambios.
- Seleccion/documento activo: sin cambios de logica.
- AppTable global: sin impacto en comportamiento por defecto.
