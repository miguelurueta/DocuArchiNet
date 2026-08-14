## Why

OPSXJ valida compuertas en cada comando, pero no conserva una bitácora única de ejecución por ticket. Esto hace que una revisión ya realizada pueda aparecer como `UNKNOWN` en `opsxj:status` y obliga a reconstruir manualmente el progreso entre `new`, `validate`, `archive` y `close`.

## What Changes

- Registrar de forma persistente el resultado verificable de cada etapa OPSXJ por ticket y SHA.
- Exponer en `opsxj:status` un checklist ordenado del ciclo, con estado, fecha, responsable, referencia de evidencia y motivo de bloqueo cuando corresponda.
- Sustituir la dependencia exclusiva de variables de entorno para la confirmación de revisión por una confirmación persistida y vinculada al SHA, conservando compatibilidad con la variable temporal actual.
- Mantener los comandos actuales y sus efectos remotos; el registro de ejecución no alterará el código WebForms ni las reglas de archivo, PR o Jira.

## Capabilities

### New Capabilities

Ninguna.

### Modified Capabilities

- `legacy-opsxj-governance`: el gobierno OPSXJ deberá conservar y mostrar el checklist persistente del ciclo de vida por ticket y SHA.

## Impact

- `tools/opsxj/scripts/lib/`: servicios de estado, validación, archivo, cierre y runner de comandos.
- `.opsxj/`: nuevo formato de registro local por ticket, sin secretos ni cambios en la aplicación WebForms.
- `tools/opsxj/README.md` y pruebas Vitest para el contrato, compatibilidad y escenarios de recuperación.
