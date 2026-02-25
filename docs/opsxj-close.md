# Comando `opsxj:close` (GitHub merge -> Jira Done)

`opsxj:close` cierra manualmente un issue en Jira **solo si** valida antes que el PR de `feature/<ISSUE-KEY>` ya fue mergeado en GitHub.

## Objetivo

- Evitar cierres manuales incorrectos en Jira.
- Validar trazabilidad contra GitHub antes de cerrar.
- Ejecutar transición a estado `done` (`Finalizado`, `Done`, etc.) usando transiciones disponibles del workflow Jira.

## Requisitos previos

Variables Jira:

- `JIRA_BASE_URL`
- `JIRA_EMAIL`
- `JIRA_API_TOKEN`

Variables GitHub:

- `GITHUB_TOKEN`
- `GITHUB_REPO` (formato `owner/repo`) o `GITHUB_OWNER` + `GITHUB_REPO_NAME`
- `GITHUB_BASE_BRANCH` (opcional, default `main`)

## Invocación

Desde `npm`:

```bash
npm run opsxj:close -- SCRUM-12
```

Directo con Node:

```bash
node scripts/opsxj.js opsxj:close SCRUM-12
```

Alias soportados:

- `close`
- `opesxj:close`

## Flujo funcional

1. Resuelve la rama esperada: `feature/<ISSUE-KEY>`.
2. Consulta PRs cerrados en GitHub para esa rama contra `main` (o `GITHUB_BASE_BRANCH`).
3. Valida que exista al menos un PR con `merged_at` (merge efectivo).
4. Si no hay merge, aborta con error y **no toca Jira**.
5. Si hay merge, ejecuta transición Jira a categoría `done`.
6. Agrega comentario en Jira con enlace del PR mergeado.

## Ejemplo de salida

```text
[opsxj:close] Ticket: SCRUM-12
[opsxj:close] PR mergeado validado: https://github.com/owner/repo/pull/24
[opsxj:close] Jira actualizado a: Finalizado
[opsxj:close] Proceso finalizado correctamente.
```

## Manejo de errores

Errores se reportan con prefijo:

- `[opsxj:error] ...`

Casos comunes:

- `No existe un PR mergeado para <ISSUE>`: el PR aún no fue mergeado.
- `Falta GITHUB_TOKEN`: falta configuración GitHub.
- `No se encontro una transicion Jira compatible con target='done'`: revisar workflow/transiciones de Jira.
