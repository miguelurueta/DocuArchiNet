# Pruebas y evidencia

## Resultado actual

| Verificación | Resultado | Evidencia |
| --- | --- | --- |
| Compilación .NET Framework Debug | PASS | MSBuild finalizó con código 0; permanecen advertencias históricas de binding/nullabilidad. |
| Prueba focal DOC-11 | PASS | `Verify-Doc11Transition.ps1` ejerció con dobles validador, gate, token, conector, requisitos, resultado y guard. |
| Sintaxis de utilidades DOC-11 | PASS | `node --check` para configuración, prueba E2E y concurrencia. |
| Revisión de límite legacy | PASS | Solo `WorkflowLegacyExecutorAdapter` contiene la invocación nueva a `Terminar_Tarea_Workflow`. |
| Preservación de Web Forms | PASS | No hay cambios en `workflow/Webworkflow.aspx`, `workflow/Webworkflow.aspx.vb` ni `workflow/ClassWorkflow.vb`. |
| E2E anónima | PASS | 2026-08-15: `EjecutarEnvioTarea` sin sesión devolvió `WORKFLOW_CONTEXT_INVALID`; no hubo transición. |
| E2E de validación autenticada | PASS | 2026-08-15: login real de GESTOR y `idTarea=0` devolvieron `WORKFLOW_TASK_INVALID`; no hubo transición. |
| Preview autorizado de tareas de control | Parcial | 2026-08-15: 922 y 879 devolvieron `WORKFLOW_TASK_UNAVAILABLE`; 920 resolvió dos destinos RUTA. Ninguna se envió. |
| E2E positiva RUTA | PASS | 2026-08-15: tareas 922 y 895, conector 18 (`CONTADOR`), HTTP 200, `Exito=true`, `EstadoFinal=completada`; 895 devolvió referencia `WF-MOD-*`. |
| E2E positiva FLUJO | PASS | 2026-08-15: tarea 879, conector 250 (`CONTADOR`), HTTP 200, `Exito=true`, `EstadoFinal=completada`. |
| E2E concurrencia real | PASS | 2026-08-15: tarea 920, dos solicitudes simultáneas al conector 18; 1 completó y 1 recibió `WORKFLOW_TRANSITION_IN_PROGRESS`. |

La concurrencia aún no se ejecuta porque el endpoint puede cambiar una tarea y auditoría, y requiere una tarea nueva. La prueba positiva 922 sí se autorizó y completó; su confirmación de estado/auditoría queda pendiente de la consulta posterior en la misma base del IIS.

## Comandos ejecutados

```powershell
& 'C:\Program Files\Microsoft Visual Studio\18\Enterprise\MSBuild\Current\Bin\amd64\MSBuild.exe' GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug /m:1 /v:minimal
node --check tools\e2e\scripts\assert-doc11-execution-config.cjs
node --check tools\e2e\tests\doc11-execution.spec.cjs
node --check tools\e2e\scripts\run-doc11-concurrency.cjs
powershell.exe -NoProfile -ExecutionPolicy Bypass -File tools\validation\Verify-Doc11Transition.ps1
git diff --check
```

Además, con el gate habilitado de forma temporal y restringido al piloto autorizado, se ejecutó `npm.cmd run test:doc11:validation` desde `tools/e2e`; terminó correctamente. El gate se dejó de nuevo desactivado y sin usuarios/grupos configurados.

## E2E reutilizable

Leer primero `tools/e2e/AGENT-RUNBOOK.md`. Los secretos se reciben solo en variables de sesión/CI; no se crean `.env` ni se guardan cookies, cuerpos o cadenas de conexión.

| Objetivo | Comando | Cambia estado |
| --- | --- | --- |
| Sin sesión | `npm.cmd --prefix tools/e2e run test:doc11:anonymous` | No |
| Validación de parámetros con piloto | `npm.cmd --prefix tools/e2e run test:doc11:validation` | No |
| Ejecución de tarea descartable | `npm.cmd --prefix tools/e2e run test:doc11:execute` | Sí, con confirmación explícita |
| Dos solicitudes simultáneas | `npm.cmd --prefix tools/e2e run test:doc11:concurrency` | Sí, con una tarea nueva descartable |

La ejecución y concurrencia exigen `DOC11_E2E_EXECUTION_AUTHORIZED=true`, ID de tarea, conector, token de preview, consultas `SELECT` aprobadas para estado/auditoría y expectativa `success` o `blocked`. Cada concurrencia consume una tarea distinta; se espera un solo envío efectivo y una respuesta concurrente controlada.

## QA manual mínima

1. Con gate apagado, invocar el endpoint con sesión piloto y confirmar `WORKFLOW_MODERN_INACTIVE`.
2. Habilitar temporalmente el gate solo para la cuenta piloto y abrir preview de una tarea descartable RUTA; conservar su token y un conector disponible.
3. Enviar mediante `EjecutarEnvioTarea`; verificar `Exito=true`, destino y estado/auditoría en las consultas aprobadas.
4. Repetir con tarea descartable FLUJO y confirmar la actividad real destino.
5. Alterar el conector, token o usar respuesta/aprobación pendiente; confirmar bloqueo funcional y estado intacto.
6. Con una tarea nueva, disparar dos solicitudes a la vez; confirmar una ejecución efectiva como máximo.
7. Restaurar el gate apagado y comprobar que el diff de las tres pantallas/clases legacy está vacío.

## Limitaciones

No se pudo adjuntar aún evidencia de transición o concurrencia: 922 y 879 no están disponibles y 920 no se usó sin control previo/posterior. La configuración dinámica de `gestor_modulos` sí localiza el módulo Workflow, pero no produjo una conexión de control utilizable desde el arnés E2E; no se expusieron sus valores. Para completar la prueba se requiere una tarea descartable disponible por escenario y las consultas de evidencia, cada una una única sentencia `SELECT` con un parámetro `?` para tarea, ejecutadas con un usuario MySQL de solo lectura.
