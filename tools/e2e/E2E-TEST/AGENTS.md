# Guía para agentes — Plataforma E2E reutilizable

Esta carpeta define cómo construir y reutilizar la plataforma E2E Workflow. Antes de modificarla, leer también:

1. `AGENTS.md` de la raíz del repositorio.
2. `tools/e2e/AGENT-RUNBOOK.md` antes de cualquier E2E autenticada.
3. `IMPLEMENTATION-PROMPT.md` para el alcance y criterios de aceptación de la plataforma.

## Propósito

Un nuevo DOC no debe volver a construir login, TTY, TLS, ODBC, consultas de control, limpieza, evidencia ni controles de cierre. Debe añadir solo un adaptador declarativo con sus operaciones y expectativas.

## Flujo de reutilización para un nuevo DOC

1. Identificar si el caso es `anonymous`, `read`, `preview`, `execution`, `concurrency` o `ui-lock`.
2. Registrar el escenario sin secretos: identificador, autorizaciones requeridas, recurso, controles registrados, transporte y expectativas.
3. Agregar o reutilizar un perfil JSON no sensible. Solo puede declarar URL base, módulo, ambiente, DSN, recurso/tarea, presupuestos y la excepción TLS autorizada.
4. Crear un adaptador pequeño para payloads permitidos, operaciones del ASMX/UI y validaciones funcionales.
5. Reutilizar el kernel para sesión, TTY, ODBC, pre/post controles, evidencia y cierre.
6. Agregar pruebas unitarias, de contrato y una E2E real solo si existe autorización explícita.

## Implementación y comando

La plataforma se invoca con `npm.cmd run test:workflow:platform --` y solo admite:

```text
--scenario <id-registrado> --profile <archivo-json-en-profiles> --authorize <autorizaciones-requeridas>
```

Los escenarios piloto disponibles son `notes-anonymous` y `notes-read`. Para `notes-read`, el comando requiere `--authorize environment`; si el perfil local autorizado define `ignoreHttpsErrors: true`, requiere además `local-tls`. Cada autorización se confirma por TTY: el argumento no sustituye la confirmación.

El kernel se ocupa de la sesión, cliente HTTP, controles ODBC, recurso, cierre, TLS, saneamiento y evidencia. El adaptador en `scripts/adapters/` no puede importar ni reimplementar esas piezas. Los comandos legados, incluido `test:notes:read`, se mantienen sin cambios durante el piloto.

## Campos mínimos del perfil

Para una lectura autenticada, el perfil necesita:

```json
{
  "scenarioId": "<doc>-read",
  "baseUrl": "https://ambiente/app/",
  "module": "GESTOR",
  "environment": "<ambiente-autorizado>",
  "odbcDsn": "<dsn-no-sensible>",
  "taskId": 0,
  "budgetMs": 10000,
  "ignoreHttpsErrors": false
}
```

Las consultas SQL se obtienen del registro de controles del repositorio; no se escriben en el perfil. Para un caso anónimo normalmente solo se necesitan `scenarioId`, `baseUrl` e `ignoreHttpsErrors` cuando se autorizó para un certificado local autofirmado.

## Secretos y autorizaciones

- Cuentas y contraseñas solo se capturan ocultas mediante TTY y existen únicamente durante el proceso hijo de la corrida.
- No pedir, imprimir ni almacenar secretos en el chat, perfiles, archivos, `.env`, `setx`, evidencia o comandos.
- El usuario debe autorizar explícitamente ambiente, cuenta y tarea; ejecución, concurrencia y bloqueo UI requieren además su autorización específica y un recurso descartable.
- La excepción TLS se valida en `false` por defecto. Solo habilitarla temporalmente ante autorización explícita para el ambiente local.

## Controles de seguridad y cierre

- Cada consulta de control debe ser un único `SELECT` con exactamente un `?`.
- Las etapas `read` y `preview` deben demostrar que estado y auditoría no cambiaron.
- La evidencia solo puede incluir códigos, conteos, latencias, banderas y huellas; nunca cuerpos HTTP, notas, destinos, credenciales, cookies o tokens.
- En `finally`, eliminar secretos, contextos de navegador, clientes API y directorios temporales.
- Al terminar, verificar gate moderno en `false` con usuarios/grupos vacíos y que las páginas legacy no tengan cambios.

## Lista de verificación antes de entregar

- El nuevo adaptador no implementa login, ODBC, prompts, TLS, redacción ni limpieza.
- El perfil no contiene secretos, URL MySQL, cadena de conexión ni SQL arbitrario.
- Las pruebas de política validan autorizaciones, controles, limpieza y evidencia saneada.
- La E2E real solo se ejecutó si el usuario la autorizó; de otro modo quedó documentada como pendiente.
- Los comandos, resultados y limitaciones están documentados sin datos sensibles.

## Migración de otro DOC

1. Añadir un adaptador declarativo sin `require` de login, ODBC, prompts, TLS ni escritura de evidencia.
2. Registrar escenario, controles `SELECT` y recurso en el registro versionado; nunca aceptar SQL ni scripts desde el perfil.
3. Añadir un perfil de ejemplo no sensible y pruebas de registro, perfil, ciclo, seguridad y contrato.
4. Exponer el escenario mediante el comando común, sin retirar el arnés legado.
5. Solicitar una autorización independiente antes de una corrida real y conservar únicamente evidencia saneada.
