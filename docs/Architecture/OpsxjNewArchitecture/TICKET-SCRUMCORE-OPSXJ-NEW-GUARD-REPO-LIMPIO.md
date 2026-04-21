# Ticket — Guardrail en `opsxj:new`: bloquear si el repo no está limpio (preflight Git antes de Jira)

## Contexto

El comando `opsxj:new` inicia un cambio OpenSpec desde un ticket Jira (consulta Jira, crea carpeta, genera `proposal.md`, crea rama, commit y push).

Hoy es posible ejecutar `opsxj:new` con el repositorio en estado “sucio” (cambios sin commit, staging pendiente o archivos no rastreados relevantes), lo que puede:

- mezclar trabajo de distintos tickets en el mismo cambio/commit
- generar propuestas incompletas o commits inesperados
- degradar la trazabilidad (ramas/tickets) y provocar errores de flujo

## Objetivo

Agregar un **preflight guardrail** a `opsxj:new` para que **valide el estado del repositorio Git antes de consultar Jira**. Si el repositorio no está limpio, el comando debe fallar explícitamente y no iniciar un ticket nuevo.

## Requerimiento funcional

1. `opsxj:new` debe ejecutar un chequeo Git **antes** de cualquier request a Jira.
2. Si el repo NO está limpio:
   - `opsxj:new` debe terminar con error (exit code != 0)
   - Debe imprimir un mensaje claro con:
     - resumen del problema (“workspace no está limpio”)
     - recomendación de acción (commit/stash/limpiar)
3. Si el repo está limpio:
   - `opsxj:new` continúa con el flujo actual sin cambios

## Alcance propuesto

- Implementar la validación en Node dentro del flujo `opsxj:new` (preferido), para evitar anidar `npm` dentro de `npm`.
- La verificación debe ser rápida y determinista.

### Regla sugerida de “repo limpio”

Definir explícitamente qué bloquea:

- Bloquear si hay cambios *tracked* sin commit (`git status --porcelain` contiene líneas de `M`, `A`, `D`, `R`, etc.)
- Bloquear si hay cambios staged (`--porcelain` con index != limpio)
- Evaluar política para untracked:
  - Opción A (estricta): bloquear si hay untracked no ignorados
  - Opción B (recomendada): permitir untracked ignorables típicos (`node_modules`, `.npm-cache`, artefactos de build) y bloquear el resto

Nota: `git status --porcelain --untracked-files=normal` + filtros por `.gitignore` suele ser suficiente.

## Diseño / Decisiones

### A. No invocar `npm run git:verify` desde `opsxj:new` (recomendado)

Motivo:

- evita recursión/encadenamiento de npm
- reduce dependencias de PowerShell en el core del flujo
- mejora observabilidad de errores (un solo comando)

### B. Validación directa con Git

Estrategia:

- Ejecutar `git status --porcelain` (y opcionalmente `git diff --name-only --cached`)
- Si hay salida relevante → bloquear

### C. Mensajes operativos

El error debe ser accionable. Ejemplo:

```text
[opsxj:error] Repo no está limpio. Termina tu trabajo actual antes de iniciar otro ticket.
Sugerencia: git status; luego commit/stash o descarta cambios. Reintenta opsxj:new.
```

## Cambios esperados (referencias)

- `package.json` (solo si se agrega flag/opción; idealmente no cambia)
- `scripts/opsxj.js` / `scripts/lib/opsxjCommandRunner.js` (punto de orquestación)
- `Tools/git-verify-synced.ps1` (no modificar; solo referencia si se quiere reutilizar)

## Pruebas obligatorias

Unitarias (Node):

- Caso 1: repo limpio → `opsxj:new` pasa el preflight
- Caso 2: repo con cambios tracked → `opsxj:new` falla antes de Jira
- Caso 3: repo con staged changes → falla antes de Jira
- Caso 4: repo con untracked ignorables → no falla (si se adopta opción B)

Regla crítica:

- En los casos que fallan, **no** debe llamarse al servicio de Jira (no requests).

## Criterios de aceptación

- `opsxj:new` bloquea cuando el workspace no está limpio (según política acordada)
- El bloqueo ocurre **antes** de consultar Jira
- El mensaje es claro y guía la corrección
- No hay regresión en ejecución normal cuando el repo está limpio

## Notas

Este ticket nace como mejora de robustez del flujo `opsxj:new` para asegurar aislamiento por ticket/cambio.

