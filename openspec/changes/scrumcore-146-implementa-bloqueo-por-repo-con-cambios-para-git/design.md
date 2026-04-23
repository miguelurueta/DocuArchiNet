## Context

`SCRUMCORE-146` fortalece el flujo `opsxj:new` agregando un **preflight** para
validar que el repositorio esté limpio (sin cambios sin commit / staging
pendiente) **antes** de consultar Jira.

Motivación:

- Evitar mezclar trabajo de tickets distintos en el mismo change/commit.
- Reducir riesgo de commits inesperados (incluyendo archivos no deseados).
- Asegurar trazabilidad: cada ticket inicia en un repo estable.

## Goals / Non-Goals

**Goals**

- `opsxj:new` debe bloquear si el workspace tiene cambios (tracked o staged).
- El bloqueo debe ocurrir **antes** de cualquier request a Jira.
- Mensaje de error accionable (sugiere commit/stash/limpieza).
- Mantener el comportamiento actual cuando el repo está limpio.
- Agregar pruebas que aseguren “no se consulta Jira si el repo no está limpio”.

**Non-Goals**

- No cambiar el comportamiento de `opsxj:archive` o `opsxj:close` (ya tienen guardrails).
- No introducir verificación de “synced con upstream” en `opsxj:new` (solo clean).
- No manejar interactivamente prompts (modo non-interactive).

## Decisions

1. **Validación Git en Node (no invocar `npm run git:verify`)**
   - **Decision:** implementar el check directamente en `scripts/lib/gitClient.js`
     usando `git status --porcelain` y `git diff --cached --name-only`.
   - **Rationale:** evita anidar npm y reduce fricción con PowerShell.

2. **`assertGitClean` como primitiva reusable**
   - **Decision:** extraer `assertGitClean({ baseDir, commandLabel })` y
     llamarlo desde `opsxj:new` antes de Jira.
   - **Rationale:** consistencia, testabilidad, separación de responsabilidades.

3. **Inyección de dependencias para tests**
   - **Decision:** `runOpsxjCommand` debe permitir inyectar `assertGitCleanFn`
     (y `assertGitCleanAndSyncedFn`) para pruebas herméticas.
   - **Rationale:** evita depender de un repo real en tests.

## Risks / Trade-offs

- [Riesgo] Bloquear por cambios puede frustrar si el usuario olvida stashear.
  - Mitigación: mensaje claro + preview de cambios.
- [Riesgo] Diferencias de Git en entornos sin `git` disponible.
  - Mitigación: error explícito al no poder ejecutar Git.

## Migration Plan

- Implementar `assertGitClean` en `scripts/lib/gitClient.js`.
- Ejecutar `assertGitClean` en `opsxj:new` antes de Jira.
- Agregar tests en `scripts/lib/opsxjCommandRunner.test.js`.
- Validar que `opsxj:new` sigue funcionando cuando el repo está limpio.

