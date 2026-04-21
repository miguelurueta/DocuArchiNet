# Spec — SCRUMCORE-146: `opsxj:new` bloquea cuando el repo no está limpio

## Objetivo

Evitar iniciar un change nuevo con un repositorio en estado “sucio”, bloqueando
`opsxj:new` antes de consultar Jira cuando existan cambios sin commit o staging.

## Requerimientos

1. `opsxj:new` debe ejecutar un preflight Git antes de consultar Jira.
2. Si hay cambios sin commit (`git status --porcelain` no vacío), debe:
   - fallar con exit code != 0
   - imprimir mensaje accionable
   - NO consultar Jira
3. Si hay cambios staged (`git diff --cached --name-only` no vacío), debe:
   - fallar con exit code != 0
   - imprimir mensaje accionable
   - NO consultar Jira
4. Si el repo está limpio, `opsxj:new` se comporta igual que antes.

## Criterios de aceptación

- Existen tests que demuestran que en repo sucio no se invoca la consulta Jira.
- El flujo normal permanece sin regresión en repo limpio.

