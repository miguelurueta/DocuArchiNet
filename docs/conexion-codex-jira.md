# Conectar Codex con JIRA (sin cambios en la aplicación)

Este proyecto **no necesita** integrar JIRA dentro del frontend para que Codex trabaje con tickets.
La conexión recomendada es externa (CLI/entorno del agente), usando credenciales de Atlassian.

## 1) Variables de entorno requeridas

Define estas variables en la terminal donde ejecutes Codex:

### Opción 1 (Windows PowerShell)

```powershell
$env:JIRA_BASE_URL="https://tu-organizacion.atlassian.net"
$env:JIRA_EMAIL="tu-usuario@empresa.com"
$env:JIRA_API_TOKEN="<token-atlassian>"
```

### Opción 2 (Bash: Linux/macOS/Git Bash)

```bash
export JIRA_BASE_URL="https://tu-organizacion.atlassian.net"
export JIRA_EMAIL="tu-usuario@empresa.com"
export JIRA_API_TOKEN="<token-atlassian>"
```

> Genera el token en Atlassian Account (API tokens): https://id.atlassian.com/manage-profile/security/api-tokens y usa una cuenta con permisos sobre los proyectos/tickets que Codex consultará.

## 2) Validar conexión desde CLI

Se incluye un script para comprobar acceso a `GET /rest/api/3/myself`:

```bash
./tools/jira/test-jira-connection.sh
```

Si está bien configurado, mostrará usuario y `accountId`.

> En Windows, ejecuta el script desde **Git Bash** o WSL correctamente configurado. Si usas solo PowerShell sin Bash, puedes validar con `Invoke-RestMethod`.

## 3) Uso operativo con Codex

Al pedir tareas a Codex, pasa el contexto JIRA directamente en el prompt:

- clave del ticket (ej. `AUTH-123`),
- URL del ticket,
- objetivo técnico,
- criterios de aceptación.

Ejemplo:

```text
Implementa AUTH-123 usando openspec/auth.behavior.yaml.
Ticket: https://tu-organizacion.atlassian.net/browse/AUTH-123
Criterios: ...
```

Con esto, Codex puede trabajar trazabilidad JIRA ↔ código ↔ PR sin acoplar JIRA a la UI.
