## 1. Preflight Git (antes de Jira)

- [x] 1.1 Definir política: bloquear si hay cambios sin commit o staged.
- [x] 1.2 Implementar `assertGitClean` (mensaje accionable + preview).
- [x] 1.3 Invocar `assertGitClean` al inicio de `opsxj:new` (antes de consultar Jira).

## 2. Pruebas

- [x] 2.1 Test: cuando Git está sucio, `opsxj:new` falla y NO llama Jira (`createProposalFn`).
- [x] 2.2 Ajustar tests existentes para inyectar el preflight (evitar dependencia de repo real).

## 3. Documentación (operativa)

- [ ] 3.1 Actualizar documentación de `opsxj:new` indicando que requiere repo limpio.
- [ ] 3.2 Documentar ejemplos de corrección (commit/stash) y mensaje esperado.

## Evidencia

- Código:
  - `scripts/lib/gitClient.js` (nuevo `assertGitClean`)
  - `scripts/lib/opsxjCommandRunner.js` (preflight en `runNew`)
- Tests:
  - `scripts/lib/opsxjCommandRunner.test.js` (caso “repo sucio”)

