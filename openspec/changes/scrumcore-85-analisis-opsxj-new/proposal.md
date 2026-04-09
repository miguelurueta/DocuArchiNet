## Why

`opsxj:new` ya crea propuestas OpenSpec desde Jira, pero el resultado todavia puede quedar en un estado demasiado generico para continuar `design`, `specs` y `tasks` sin correccion manual. Eso degrada el valor del flujo automatico y produce cambios con capabilities artificiales o impactos poco utiles para refinement posterior.

## What Changes

- Se analiza el comportamiento actual de `opsxj:new` y del generador de propuestas para identificar por que algunas propuestas quedan con texto heredado o capabilities no alineadas al resumen real del ticket.
- Se documentan los patrones de falla, sus causas probables y las zonas de codigo implicadas para preparar un ticket posterior de correccion.
- Se deja explicitamente fuera de alcance cualquier ajuste funcional en `proposalGenerator`, `jiraProposalService` o el comando `opsxj:new`.

## Capabilities

### Modified Capabilities
- `jira-proposal-generator`: Generacion automatica de propuestas OpenSpec desde Jira, con inferencia de capability e impacto alineada al contexto funcional real del ticket.

## Impact

- Analisis sobre `scripts/lib/proposalGenerator.*`, `scripts/lib/jiraProposalService.*` y la documentacion operativa de `opsxj:new`.
- Evidencia para un cambio posterior, pero sin modificaciones funcionales en el tooling durante este ticket.
