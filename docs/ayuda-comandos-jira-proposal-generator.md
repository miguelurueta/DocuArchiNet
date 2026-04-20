# Ayuda de usuario: comandos Jira Proposal Generator

Este documento explica cómo usar los comandos del flujo `jira-proposal-generator`, incluyendo parámetros, variables de entorno y ejemplos.

## Requisitos previos

Definir estas variables en la sesión de terminal:

- `JIRA_BASE_URL`: URL de Jira Cloud.
- `JIRA_EMAIL`: correo de la cuenta Jira.
- `JIRA_API_TOKEN`: token API de Atlassian.
- `GITHUB_TOKEN`: token GitHub con permiso para crear PR.
- `GITHUB_REPO`: repositorio destino en formato `owner/repo`.

Ejemplo PowerShell:

```powershell
$env:JIRA_BASE_URL = "https://tu-organizacion.atlassian.net"
$env:JIRA_EMAIL = "usuario@empresa.com"
$env:JIRA_API_TOKEN = "TU_TOKEN"
$env:GITHUB_TOKEN = "ghp_xxx"
$env:GITHUB_REPO = "owner/repo"
```

## Regla de bloqueo para `opsxj:new`

`opsxj:new` requiere lectura exitosa del ticket en Jira.

Si Jira falla por:
- credenciales
- conectividad
- permisos
- issue inexistente

entonces el comando debe terminar con error y NO debe:
- generar `proposal.md`
- crear rama `feature/<ISSUE-KEY>`
- crear commit inicial
- hacer push

## 1) `opsxj:new` (comando recomendado)

Comando de un paso para crear proposal OpenSpec desde Jira.

### Sintaxis

```bash
npm run opsxj:new -- <ISSUE-KEY>
```

o directo:

```bash
node scripts/opsxj.js opsxj:new <ISSUE-KEY>
```

### Parámetros

- `<ISSUE-KEY>` (requerido): clave del ticket Jira, por ejemplo `SCRUM-8`.

### Qué hace

1. Consulta el ticket en Jira (`summary`, `description`).
2. Genera `proposal.md`.
3. Crea carpeta OpenSpec con base en `issueKey + resumen` (slug kebab-case).
4. Crea rama `feature/<ISSUE-KEY>`, commit inicial del `proposal.md` y push.
5. Muestra confirmaciones y ruta final del proposal.

### Qué NO hace si Jira falla

- no genera proposal parcial
- no intenta fallback local
- no avanza a git

### Ejemplo

```bash
npm run opsxj:new -- SCRUM-8
```

Salida esperada (resumen):

```text
[opsxj:new] Ticket: SCRUM-8
[opsxj:new] Carpeta OpenSpec: openspec\changes\scrum-8-...
[opsxj:new] Proposal creado: openspec\changes\...\proposal.md
[opsxj:new] Proceso finalizado correctamente.
```

## Pre-check manual antes de `opsxj:archive` (recomendado)

Antes de archivar, valide que NO hay cambios sin commit y que la rama esta sincronizada con upstream:

```bash
git status -sb
git diff --name-only
git diff --cached --name-only
git rev-list --left-right --count @{upstream}...HEAD
```

- Debe estar limpio (sin `M`, `D`, `??`) y el conteo debe ser `0 0`.

## Utilidades Git (opcional)

- `npm run git:verify`: valida que la rama local este limpia y sincronizada con su upstream.
- `npm run git:update`: hace `git add -A`, commit con mensaje fijo, push y crea PR automaticamente.
  - Requiere `GITHUB_TOKEN` y `GITHUB_REPO` en el entorno.

## 2) `generate-proposal-from-jira.js` (modo clásico)

Genera proposal usando carpeta basada en `issueKey` (sin slug del resumen).

### Sintaxis

```bash
node scripts/generate-proposal-from-jira.js <ISSUE-KEY>
```

### Parámetros

- `<ISSUE-KEY>` (requerido): clave del ticket Jira.

### Ejemplo

```bash
node scripts/generate-proposal-from-jira.js SCRUM-8
```

## 3) `fetch-jira.js` (solo consulta)

Consulta Jira y devuelve JSON del issue (summary/description), sin crear proposal.

### Sintaxis

```bash
node scripts/fetch-jira.js <ISSUE-KEY>
```

### Parámetros

- `<ISSUE-KEY>` (requerido): clave del ticket Jira.

### Ejemplo

```bash
node scripts/fetch-jira.js SCRUM-8
```

## Parámetros y opciones internas de `opsxj:new`

El runner actual soporta alias:

- `opsxj:new`
- `new` (alias)

## 4) `opsxj:archive` (archive + PR + Jira)

Ejecuta archive del cambio asociado al issue, abre PR y comenta Jira con el enlace.

### Sintaxis

```bash
npm run opsxj:archive -- <ISSUE-KEY>
```

o directo:

```bash
node scripts/opsxj.js opsxj:archive <ISSUE-KEY>
```

### Qué hace

1. Busca el cambio activo OpenSpec que inicia con `<ISSUE-KEY>-`.
2. Ejecuta archive de OpenSpec (con fallback `--skip-specs` si falla sync de specs).
3. Crea/reutiliza PR desde `feature/<ISSUE-KEY>` hacia `main`.
4. Comenta en Jira con URL del PR.
5. Deja el cierre final de Jira atado al resultado del PR (merge/rechazo).

### Ejemplo

```bash
npm run opsxj:archive -- SCRUM-10
```

Salida esperada (resumen):

```text
[opsxj:archive] Ticket: SCRUM-10
[opsxj:archive] Cambio archivado: scrum-10-...
[opsxj:archive] PR creado: https://github.com/.../pull/123
[opsxj:archive] Jira comentado con enlace al PR.
```

## 5) `opsxj:doctor` (pre-check de configuración)

Valida rápidamente si el entorno tiene lo mínimo para correr el flujo.

### Sintaxis

```bash
npm run opsxj:doctor
```

### Qué valida

1. Variables Jira: `JIRA_BASE_URL`, `JIRA_EMAIL`, `JIRA_API_TOKEN`.
2. Variables GitHub: `GITHUB_TOKEN`.
3. Repositorio GitHub: `GITHUB_REPO` o `GITHUB_OWNER` + `GITHUB_REPO_NAME`.

Uso interno:

```bash
node scripts/opsxj.js new <ISSUE-KEY>
```

## 6) `opsxj:close` (cierre manual con validación de merge)

Cierra el issue en Jira únicamente si detecta PR mergeado para `feature/<ISSUE-KEY>`.

### Sintaxis

```bash
npm run opsxj:close -- <ISSUE-KEY>
```

o directo:

```bash
node scripts/opsxj.js opsxj:close <ISSUE-KEY>
```

### Parámetros

- `<ISSUE-KEY>` (requerido): clave del ticket Jira.

### Qué valida antes de cerrar

1. Busca PRs cerrados en GitHub para `feature/<ISSUE-KEY>` contra `main` (o `GITHUB_BASE_BRANCH`).
2. Verifica que exista PR con `merged_at` (merge real).
3. Si no hay merge, falla y no modifica Jira.
4. Si hay merge, transiciona Jira a `done` y comenta el enlace del PR.

### Ejemplo

```bash
npm run opsxj:close -- SCRUM-12
```

Salida esperada (resumen):

```text
[opsxj:close] Ticket: SCRUM-12
[opsxj:close] PR mergeado validado: https://github.com/.../pull/24
[opsxj:close] Jira actualizado a: Finalizado
```

## Errores comunes y solución

- `[opsxj:error] Falta issueKey...`
  - Solución: pasar `<ISSUE-KEY>` en el comando.

- `401 No autorizado`
  - Solución: revisar `JIRA_EMAIL` y `JIRA_API_TOKEN`.

- `404 ... no existe o no tienes permiso`
  - Solución: validar que la key exista y la cuenta tenga permisos.

- `Falta JIRA_BASE_URL...`
  - Solución: exportar variables de entorno antes de ejecutar.

- `fetch failed` o errores de Jira
  - Solución: restaurar acceso real a Jira antes de reintentar; `opsxj:new` no debe continuar sin lectura exitosa del ticket.

## Flujo recomendado para usuarios

1. Crear/abrir ticket en Jira.
2. Ejecutar:
   - `npm run opsxj:new -- <ISSUE-KEY>`
3. Revisar proposal generado en `openspec/changes/.../proposal.md`.
4. Continuar flujo OpenSpec (`design`, `specs`, `tasks`, implementación, verify, archive).
5. Ejecutar:
   - `npm run opsxj:archive -- <ISSUE-KEY>`
6. Si necesitas cierre manual controlado:
   - `npm run opsxj:close -- <ISSUE-KEY>`
7. Jira se sincroniza automáticamente por GitHub Action:
   - PR mergeado -> `Done`
   - PR cerrado sin merge -> `In Progress`

## Recomendación de uso con agentes Codex

Los comandos `opsxj` no seleccionan modelos por sí mismos. La elección de subagente mini o agente principal depende del entorno de Codex que los ejecuta.

Política recomendada:

- `opsxj:new`: mini para artefactos OpenSpec, principal para implementación
- `opsxj:archive`: principal para verify, archive y PR
- `opsxj:close`: principal para cierre final de Jira

Referencia:

- `docs/codex-agent-strategy.md`
