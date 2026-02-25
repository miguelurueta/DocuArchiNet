# Comando `opsxj:new` (Jira -> OpenSpec)

`opsxj:new` es el comando de entrada para crear propuestas OpenSpec desde un ticket Jira con un solo paso.

## Objetivo

- Consultar el ticket actual en Jira.
- Generar `proposal.md` en `openspec/changes/<carpeta>/`.
- Usar el **resumen del ticket** como base del nombre de carpeta para que el flujo de archive de OpenSpec trabaje con un identificador legible y estable.

## Requisitos previos

Definir variables de entorno Jira:

- `JIRA_BASE_URL`
- `JIRA_EMAIL`
- `JIRA_API_TOKEN`

Variables Git/GitHub para trazabilidad automática:

- `GITHUB_TOKEN`
- `GITHUB_REPO` (formato `owner/repo`) o `GITHUB_OWNER` + `GITHUB_REPO_NAME`
- `GITHUB_BASE_BRANCH` (opcional, default `main`)
- `GIT_AUTO_PUSH` (opcional, default `true`)

Opcional:

- `JIRA_ISSUE_KEY` (si no se pasa `<ISSUE-KEY>` por argumento)

## Invocación

Desde `npm`:

```bash
npm run opsxj:new -- SCRUM-8
```

Directo con Node:

```bash
node scripts/opsxj.js opsxj:new SCRUM-8
```

## Flujo funcional

1. `opsxj:new` consulta Jira (`summary`, `description`) del ticket indicado.
2. Genera contenido OpenSpec de `proposal.md`.
3. Construye carpeta de cambio usando `issueKey + slug(summary)` en kebab-case:
   - Ejemplo: `scrum-8-auto-complete-asunto`
4. Escribe:
   - `openspec/changes/<changeName>/proposal.md`
5. Crea/sincroniza rama `feature/<ISSUE-KEY>`.
6. Crea commit inicial con `proposal.md`.
7. Hace push de la rama (si `GIT_AUTO_PUSH=true`).
5. Muestra confirmaciones centralizadas:
   - ticket procesado
   - resumen Jira usado
   - carpeta OpenSpec creada
   - ruta del proposal

## Integración con archive de OpenSpec

OpenSpec archiva basado en el nombre de carpeta del cambio.  
Como `opsxj:new` crea la carpeta a partir del resumen Jira (normalizado), el archive conserva un nombre trazable al contexto funcional del ticket.

Además, la rama queda preparada para `opsxj:archive`, que abre PR y comenta Jira.

## Manejo centralizado de errores

Todos los errores de `opsxj` se reportan con prefijo uniforme:

- `[opsxj:error] ...`

Casos cubiertos:

- comando desconocido
- `issueKey` ausente
- credenciales faltantes
- errores de red/permisos Jira
- errores de escritura de archivos

## Extensibilidad para futuros comandos

El runner de `opsxj` usa un registro de comandos en:

- `scripts/lib/opsxjCommandRunner.js`

Para agregar nuevos comandos (`opsxj:archive`, `opsxj:sync`, etc.), se registra un nuevo handler sin romper `opsxj:new`.
