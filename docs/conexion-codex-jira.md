# Conectar Codex con JIRA (sin cambios en la aplicacion)

Este proyecto **no necesita** integrar JIRA dentro del frontend para que Codex trabaje con tickets.
La conexion recomendada es externa (CLI/entorno del agente), usando credenciales de Atlassian.

## 1) Variables de entorno requeridas

Define estas variables en la terminal donde ejecutes Codex:

### Opcion 1 (Windows PowerShell)

```powershell
$env:JIRA_BASE_URL="https://tu-organizacion.atlassian.net"
$env:JIRA_EMAIL="tu-usuario@empresa.com"
$env:JIRA_API_TOKEN="<token-atlassian>"
```

### Opcion 2 (Bash: Linux/macOS/Git Bash)

```bash
export JIRA_BASE_URL="https://tu-organizacion.atlassian.net"
export JIRA_EMAIL="tu-usuario@empresa.com"
export JIRA_API_TOKEN="<token-atlassian>"
```

> Genera el token en Atlassian Account (API tokens): https://id.atlassian.com/manage-profile/security/api-tokens y usa una cuenta con permisos sobre los proyectos/tickets que Codex consultara.

## 2) Validar conexion desde CLI

Primero valida variables y config general:

```bash
npm run opsxj:doctor
```

Si usas PowerShell con `ExecutionPolicy` restrictivo y falla `npm.ps1`, usa:

```powershell
npm.cmd run opsxj:doctor
```

Luego valida conectividad real a Jira (`GET /rest/api/3/myself`):

```bash
npm run jira:test
```

En PowerShell (mismo caso de `ExecutionPolicy`):

```powershell
npm.cmd run jira:test
```

Si esta bien configurado, veras usuario y `accountId`.

## 3) Uso operativo con Codex

Al pedir tareas a Codex, pasa el contexto JIRA directamente en el prompt:

- clave del ticket (ej. `AUTH-123`),
- URL del ticket,
- objetivo tecnico,
- criterios de aceptacion.

Ejemplo:

```text
Implementa AUTH-123 usando openspec/auth.behavior.yaml.
Ticket: https://tu-organizacion.atlassian.net/browse/AUTH-123
Criterios: ...
```

Con esto, Codex puede trabajar trazabilidad JIRA -> codigo -> PR sin acoplar JIRA a la UI.
