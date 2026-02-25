# Ayuda de usuario: comandos Jira Proposal Generator

Este documento explica cómo usar los comandos del flujo `jira-proposal-generator`, incluyendo parámetros, variables de entorno y ejemplos.

## Requisitos previos

Definir estas variables en la sesión de terminal:

- `JIRA_BASE_URL`: URL de Jira Cloud.
- `JIRA_EMAIL`: correo de la cuenta Jira.
- `JIRA_API_TOKEN`: token API de Atlassian.

Ejemplo PowerShell:

```powershell
$env:JIRA_BASE_URL = "https://tu-organizacion.atlassian.net"
$env:JIRA_EMAIL = "usuario@empresa.com"
$env:JIRA_API_TOKEN = "TU_TOKEN"
```

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
4. Muestra confirmaciones y ruta final del proposal.

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

Uso interno:

```bash
node scripts/opsxj.js new <ISSUE-KEY>
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

## Flujo recomendado para usuarios

1. Crear/abrir ticket en Jira.
2. Ejecutar:
   - `npm run opsxj:new -- <ISSUE-KEY>`
3. Revisar proposal generado en `openspec/changes/.../proposal.md`.
4. Continuar flujo OpenSpec (`design`, `specs`, `tasks`, implementación, verify, archive).
