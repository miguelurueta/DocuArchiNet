# ANALISIS TECNICO

# Flujo `opsxj:new` y calidad de propuestas OpenSpec

## Objetivo

Documentar el diagnostico tecnico del flujo `opsxj:new` para dejar claro:

- donde aparece el problema observado
- que capas del repo estan implicadas
- que evidencia ya existe
- que restricciones debe respetar un ticket posterior de correccion

Este documento es solo de analisis. No propone implementar cambios funcionales dentro de este artefacto.

## Contexto

`opsxj:new` resuelve hoy un flujo compuesto:

1. lee `summary` y `description` desde Jira
2. genera una carpeta OpenSpec
3. construye `proposal.md`
4. crea rama, commit y push inicial

El problema observado no esta en la orquestacion Git/Jira en si, sino en la calidad del `proposal.md` resultante cuando la inferencia falla. En esos casos, el cambio arranca con texto demasiado generico, capabilities artificiales o impactos poco utiles para continuar `design`, `specs` y `tasks`.

## Sintoma principal

En ciertos tickets, `opsxj:new` deja un `proposal.md` que:

- no refleja la intencion funcional real del issue
- usa texto heredado del tooling
- inventa una capability artificial o demasiado generica
- obliga a corregir manualmente el artifacto antes de continuar el refinement

## Capas implicadas

Las capas involucradas en el diagnostico son:

- comando:
  - `scripts/lib/opsxjCommandRunner.js`
- generacion de proposal:
  - `scripts/lib/proposalGenerator.js`
  - `scripts/lib/jiraProposalService.js`
- pruebas existentes:
  - `scripts/lib/proposalGenerator.test.js`
  - `scripts/lib/jiraProposalService.test.js`
- documentacion operativa:
  - `docs/opsxj-new.md`
  - `docs/ayuda-comandos-jira-proposal-generator.md`

## Hipotesis tecnica

La falla probable vive en la capa que transforma `summary` y `description` en contenido de `proposal.md`, no en la lectura Jira ni en la creacion de rama.

En particular, el punto de observacion correcto es:

- inferencia de capability
- inferencia de impacto
- redaccion base de `Why`, `What Changes` e `Impact`

Esto apunta a `proposalGenerator` y/o `jiraProposalService` como zonas principales de analisis.

## Evidencia existente en el repo

El repo ya contiene evidencia de la intencion correcta del flujo:

### 1. Documentacion

[opsxj-new.md](D:/imagenesda/GestorDocumental/DocuArchiCore.react/docs/opsxj-new.md) deja claro que:

- `opsxj:new` consulta Jira
- usa el resumen del ticket como base del cambio
- genera un `proposal.md` util para iniciar el flujo OpenSpec

### 2. Tests

[proposalGenerator.test.js](D:/imagenesda/GestorDocumental/DocuArchiCore.react/scripts/lib/proposalGenerator.test.js) y [jiraProposalService.test.js](D:/imagenesda/GestorDocumental/DocuArchiCore.react/scripts/lib/jiraProposalService.test.js) ya expresan una expectativa importante:

- el proposal debe alinearse al ticket Jira
- el contenido no debe quedar centrado en `jira-proposal-generator`
- la capability debe salir del contexto funcional del issue cuando exista señal suficiente

### 3. Cambios archivados

Hay cambios archivados donde el `proposal.md` inicial debio corregirse manualmente para reemplazar texto generico o una capability mal inferida. Eso confirma que el problema no es teorico: ya ocurrio en el flujo real.

## Delimitacion del problema

Este analisis separa dos cosas que no deben mezclarse:

### A. Orquestacion `opsxj:new`

Incluye:

- lectura Jira
- nombre de carpeta
- creacion de rama
- commit inicial
- push

### B. Calidad del `proposal.md`

Incluye:

- capability inferida
- impacto inferido
- redaccion base
- utilidad real del proposal para continuar `design/specs/tasks`

El defecto observado cae en **B**, no necesariamente en **A**.

## Riesgos de una correccion mal enfocada

Si el ticket posterior se implementa sin esta separacion, hay riesgos claros:

- tocar `opsxj:new` completo cuando el bug esta solo en la inferencia
- introducir postprocesamientos de markdown en vez de corregir la fuente
- mezclar reglas de negocio del ticket con reglas genericas del generador
- romper tickets que hoy si generan proposals razonables

## Restricciones para un ticket posterior de correccion

Un ticket posterior deberia respetar estas restricciones:

- no crear una capability nueva para `opsxj:new`
- tratar el problema como parte de `jira-proposal-generator`
- corregir la inferencia antes que postprocesar el markdown
- usar tickets reales del repo como casos de prueba
- no modificar Git/Jira orchestration si no hay evidencia de que ahi vive el problema

## Recomendacion

El siguiente ticket no deberia ser de analisis. Deberia ser un ticket de correccion acotado a:

- `proposalGenerator`
- `jiraProposalService`
- pruebas unitarias del generador
- documentacion de `opsxj:new` solo si cambia la expectativa publica del proposal

## Fuera de alcance de este analisis

Este documento no:

- cambia scripts
- cambia tests
- cambia comportamiento de `opsxj:new`
- redefine el flujo Jira/Git

Su salida es solo diagnostica y sirve como base para abrir un ticket posterior de correccion bien acotado.
