# Prompt de implementación — Plataforma reutilizable E2E Workflow

Antes de comenzar, leer la guía operativa para agentes en [AGENTS.md](AGENTS.md).

## Objetivo

Implementa una plataforma E2E reutilizable para pruebas Workflow en este repositorio. Debe permitir que un nuevo DOC declare su escenario y expectativas sin volver a implementar login, captura de secretos, TLS local, consultas de control, evidencia, limpieza ni controles de cierre.

La plataforma debe aprovechar los componentes existentes de `tools/e2e`, especialmente:

- `tests/support/authenticated-workflow-session.cjs` para la sesión Gestión → Workflow.
- `scripts/support/interactive-e2e-console.cjs` para TTY y secretos efímeros.
- `scripts/support/doc32-e2e-odbc.cjs` para controles ODBC de solo lectura.
- `scripts/support/workflow-e2e-orchestrator.cjs` como referencia de perfiles, etapas y autorizaciones.

No reemplaces los arneses DOC existentes en una primera entrega. Agrega la plataforma y migra como prueba piloto el escenario de lectura de Notas; mantén compatibilidad con los comandos actuales.

## Implementación disponible

El kernel aditivo ya se encuentra bajo `tools/e2e`:

- `scripts/support/workflow-e2e-platform-registry.cjs`: único registro versionado de escenarios, controles y adaptadores.
- `scripts/support/workflow-e2e-platform-profile.cjs`: carga perfiles JSON desde `profiles/` y rechaza campos o valores sensibles.
- `scripts/support/workflow-e2e-platform.cjs`: preflight, autorizaciones, sesión, controles, ciclo de recursos, cierre y evidencia segura.
- `scripts/adapters/notes-read-e2e-adapter.cjs`: piloto DOC-41 que solo conoce sus operaciones y expectativas.
- `scripts/run-workflow-e2e-platform.cjs`: punto de entrada interactivo limitado a `--scenario`, `--profile` y `--authorize`.

El perfil de referencia es `profiles/workflow-e2e-platform.profile.example.json`. No es una autorización de ejecución ni contiene secretos.

### Uso del piloto

Para una lectura real previamente autorizada, desde `tools/e2e` se usa:

```powershell
npm.cmd run test:workflow:platform -- --scenario notes-read --profile workflow-e2e-platform.profile.example.json --authorize environment
```

El iniciador confirma la autorización y solicita en TTY únicamente las cuentas Workflow y de lectura ODBC. Si el perfil habilita temporalmente `ignoreHttpsErrors: true` para un certificado local autofirmado autorizado, el comando debe incluir `--authorize environment,local-tls` y la TTY exige ambas confirmaciones. El comando existente `test:notes:read` no cambia durante el piloto.

La implementación no ejecuta una E2E real por sí misma. Esa acción requiere una autorización nueva y explícita de ambiente, cuentas y tarea.

## Restricciones obligatorias

1. Nunca guardar, imprimir ni registrar contraseñas, cookies, tokens, cadenas de conexión, cuerpos HTTP, contenido de notas, usuarios destino o identificadores sensibles.
2. No crear `.env`, no usar `setx` y no persistir secretos en perfiles, archivos, procesos posteriores o evidencia.
3. Las consultas de control deben ser una única sentencia `SELECT`, con exactamente un parámetro `?`, validada antes de abrir navegador o conexión ODBC.
4. Las etapas mutantes requieren autorización explícita para ambiente y recurso; una lectura/preview no puede cambiar tarea, estado ni auditoría.
5. Al cierre de toda corrida comprobar:
   - `WorkflowCentroTrabajoModernActive=false`;
   - usuarios y grupos del gate vacíos;
   - ausencia de cambios en `workflow/Webworkflow.aspx` y `workflow/Webworkflow.aspx.vb`.
6. TLS se valida por defecto. `ignoreHTTPSErrors` solo puede habilitarse con una bandera efímera no sensible y autorización explícita para un certificado local autofirmado.
7. La salida de procesos y artefactos de Playwright se deben sanear; los directorios temporales se eliminan al terminar, incluso si una prueba falla.

## Diseño requerido

### 1. Registro declarativo de escenarios

Crear un registro común de escenarios, sin secretos, con una estructura equivalente a:

```js
{
  id: 'notes-read',
  doc: 'doc41',
  mode: 'read',
  requiredAuthorizations: ['environment'],
  requiredSecrets: ['workflowAccount', 'workflowPassword', 'readOnlyDbUser', 'readOnlyDbPassword'],
  resource: { kind: 'workflow-task', role: 'read', variable: 'TASK_ID' },
  controls: ['notes-task-state', 'notes-audit'],
  transport: { session: 'workflow', service: 'notes-modern' },
  expectations: ['no-state-change', 'no-audit-change', 'sanitized-evidence']
}
```

El registro debe validar campos permitidos, rechazar comandos arbitrarios y no permitir que un perfil seleccione scripts libremente.

### 2. Perfil no sensible

Definir un perfil JSON reutilizable que solo permita configuración no sensible: raíz, módulo, ambiente, DSN, referencias de consultas registradas, tarea, presupuestos, navegador y excepción TLS explícita.

El perfil debe rechazar cuentas, contraseñas, cookies, tokens, URLs/cadenas de conexión MySQL y SQL arbitrario. Las consultas deben residir en un registro versionado del repositorio, no dentro del perfil.

### 3. Ejecutor común

Implementar un único ejecutor que:

1. Cargue y valide perfil, escenario, autorizaciones y recurso.
2. Solicite por TTY solo secretos efímeros y confirmaciones necesarias.
3. Cree una sesión autenticada mediante el helper existente.
4. Ejecute preflight, controles antes/después y la etapa solicitada.
5. Aplique presupuestos, sanee salida y escriba evidencia mínima.
6. Elimine secretos, contextos, clientes API y directorios temporales en `finally`.
7. Ejecute los controles de cierre aunque falle una etapa.

Las etapas mínimas son: `anonymous`, `read`, `preview`, `execution`, `concurrency` y `ui-lock`. Las tres últimas deben estar fallar-cerrado si falta autorización o recurso descartable.

### 4. Adaptadores por DOC

Cada DOC debe aportar únicamente un adaptador pequeño que declare:

- operaciones y payloads permitidos;
- selectores o servicio ASMX autorizado;
- escenarios declarativos;
- expectativas funcionales y de no mutación;
- esquema de evidencia saneada.

El adaptador no puede implementar login, ODBC, prompts, TLS, redacción, limpieza ni comandos de Playwright.

### 5. Gestión de recursos de prueba

Registrar recursos por rol: lectura, ejecución, concurrencia y bloqueo UI. Un recurso mutante debe reservarse antes de usarse, liberarse al fallar y marcarse consumido al completar. No reutilizar la misma tarea para escenarios mutantes incompatibles.

## Migración piloto obligatoria: Notas Workflow

Migrar el flujo `notes-read` como piloto. Debe conservar:

- raíz local, módulo y ambiente GESTOR preconfigurados como valores no sensibles;
- DSN ODBC `workflowconta`;
- captura TTY de cuenta Workflow y cuenta MySQL de solo lectura;
- controles de `ANOTACION_TAREA` y `wf_log_workflow` sin contenido de nota ni datos de operación;
- `ListarNotas`, `ConsultarNota` y cursor inválido;
- comparación de huellas antes/después;
- soporte explícitamente autorizado para TLS local autofirmado;
- evidencia sin notas, IDs de nota, contenido, credenciales, cookies ni respuestas HTTP.

El comando existente `npm.cmd --prefix tools/e2e run test:notes:read` debe continuar funcionando. El nuevo ejecutor puede exponerse con un comando adicional durante la migración.

## Pruebas y criterios de aceptación

1. Pruebas unitarias del validador de perfiles, registro, autorizaciones, consultas y saneamiento.
2. Pruebas de contrato que demuestren que un adaptador no puede introducir login ni persistencia de secretos.
3. Pruebas de orden: un fallo bloquea etapas posteriores y aun así ejecuta el cierre.
4. Pruebas de limpieza: secretos, clientes API y directorios temporales se eliminan al finalizar o fallar.
5. Pruebas de seguridad TLS: por defecto se valida el certificado; la excepción solo opera cuando fue autorizada explícitamente.
6. Prueba E2E anónima y de lectura de Notas con autorización explícita, usando una tarea aprobada y consultas `SELECT` de control.
7. No modificar el flujo legacy ni activar/configurar el gate moderno.
8. Documentar el contrato, un perfil de ejemplo sin secretos, comandos de uso y procedimiento de migración de un nuevo DOC.

## Entregables

- Registro, validador y ejecutor común bajo `tools/e2e`.
- Perfil de ejemplo no sensible.
- Adaptador piloto de Notas.
- Pruebas automatizadas y comandos npm.
- Documentación de arquitectura, seguridad y uso.
- Matriz de migración para DOC-32, DOC-33 y futuros DOC, sin migrarlos todavía salvo que se solicite expresamente.

## Forma de trabajo

Antes de ejecutar una E2E autenticada, leer `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`. No ejecutar escenarios reales sin autorización explícita para ambiente, cuentas y recursos. Informar resultados solo mediante códigos, conteos, latencias, banderas y huellas saneadas.
