# Pruebas y evidencia

## Ejecutado localmente

| Validación | Comando | Resultado |
| --- | --- | --- |
| Compilación .NET Framework | `msbuild GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug /m /nologo` | Correcto el 2026-08-14: 0 errores; 277 advertencias históricas del proyecto. |
| Verificación focal | `powershell.exe -NoProfile -ExecutionPolicy Bypass -File tools\validation\Verify-Doc10Preview.ps1` | Correcto: contrato ASMX, gate/contexto, aislamiento Workflow/Docuarchi, bootstrap Gestión → Workflow sin auditoría, tarea inválida/no disponible, flujo, ruta, destino vacío, conector ajeno, tipo desconocido y repositorios sin escritura. |
| OpenSpec estricto | `openspec validate doc-10-previsualizacion-ruta-segura-flujo --strict` | Correcto el 2026-08-14. |
| OpsXJ de cierre | `npm.cmd --prefix tools\opsxj run opsxj:validate -- DOC-10` | Correcto el 2026-08-14: gobierno, documentación, evidencia focal y QA manual validados. Estado final del flujo: `READY`. |
| Sintaxis E2E | Analizador PowerShell sobre `tools/e2e/Invoke-Doc10PreviewE2E.ps1` | Correcto. |
| ASMX sin sesión con Playwright | `DOC10_E2E_BASE_URL=http://localhost/GestionDocumental-Docuarchi.net/`, `npm.cmd --prefix tools\e2e run test:anonymous` | Correcto el 2026-08-14: 1/1. El ASMX respondió `WORKFLOW_CONTEXT_INVALID` y cero destinos. |
| Sesión Gestión con Playwright | `npm.cmd --prefix tools\e2e run test:session` | Correcto el 2026-08-14: 1/1. El postback de GESTOR creó sesión y el ASMX no devolvió `WORKFLOW_CONTEXT_INVALID`. |
| Autorización piloto/no piloto con Playwright | `npm.cmd --prefix tools\e2e run test:authorization` | Correcto el 2026-08-14: 1/1. El piloto superó el gate; el usuario no piloto recibió `WORKFLOW_MODERN_INACTIVE` y cero destinos. |
| E2E completa con estado/auditoría | `npm.cmd --prefix tools\e2e run test:e2e` | Correcto el 2026-08-14: 1/1 para tarea 879. Huellas de estado y `wf_log_estados_workflow` idénticas antes/después. Evidencia: [qa-preview-879.json](evidencias/qa-preview-879.json). |
| E2E completa de ruta con estado/auditoría | `npm.cmd --prefix tools\e2e run test:e2e` | Correcto el 2026-08-14: 1/1 para tarea 922. El piloto obtuvo dos destinos `RUTA`; el no piloto recibió `WORKFLOW_MODERN_INACTIVE`. Huellas de estado y `wf_log_estados_workflow` idénticas antes/después. Evidencia: [qa-preview-922.json](evidencias/qa-preview-922.json). |
| QA manual mínima de ruta | Navegador autenticado en GESTOR + POST al ASMX | Correcto el 2026-08-14: tarea 922 devolvió HTTP 200, `RUTA`, dos destinos y `Error: null`. Evidencia: [qa-manual-922.json](evidencias/qa-manual-922.json). |
| Carga autenticada del ASMX | `npm.cmd --prefix tools\e2e run test:load` | El ASMX respondió 20/20 con p95 119,54 ms y 30/30 con p95 68,71 ms, sin errores ni mutación de estado/auditoría. El bootstrap Web Forms tuvo timeouts intermitentes en corridas mixtas; se registra como riesgo separado. Evidencias: [carga inicial](evidencias/qa-preview-load-922.json) y [carga validada](evidencias/qa-preview-load-922-validada.json). |
| Integridad legacy | `git diff --name-only -- workflow/Webworkflow.aspx workflow/Webworkflow.aspx.vb` | Sin salida: ambos archivos continúan sin modificación. |

## E2E automatizada real

La automatización PowerShell se conserva en `tools/e2e/Invoke-Doc10PreviewE2E.ps1`. La alternativa recomendada e integrada para ejecución continua es `tools/e2e/tests/doc10-preview.spec.cjs` con Playwright: realiza login real en `gestor.aspx` mediante Chromium, reutiliza la cookie de esa sesión para invocar el ASMX y ejecuta dos consultas `SELECT` antes/después. Nunca escribe en base de datos ni guarda contraseñas, cookies o cadena de conexión en la evidencia.

La reutilización por agentes está definida en [tools/e2e/AGENT-RUNBOOK.md](../../../../../tools/e2e/AGENT-RUNBOOK.md) y se anuncia desde `AGENTS.md`. El runbook exige autorización explícita, secretos fuera del repositorio, restauración del gate y una comprobación final de integridad legacy.

La QA humana mínima quedó ejecutada para la ruta 922; su paso a paso reutilizable está en [07-checklist-qa-manual.md](07-checklist-qa-manual.md). La ausencia de mutación se demuestra con la E2E automatizada antes/después de esa misma tarea, no con una operación manual de escritura.

La prueba `test:anonymous` permite comprobar directamente el ASMX **sin login**. Por diseño solo puede validar `WORKFLOW_CONTEXT_INVALID` y cero destinos; no sustituye `test:e2e`, que conserva la autorización real de piloto y no piloto.

```powershell
npm.cmd --prefix tools\e2e install
npm.cmd --prefix tools\e2e run install:browsers
$env:DOC10_E2E_BASE_URL = 'https://qa.example/app/'
$env:DOC10_E2E_BROWSER_CHANNEL = 'msedge' # opcional si el agente ya tiene Edge
npm.cmd --prefix tools\e2e run test:anonymous
```

Para la E2E completa, se configuran en el secreto del agente de CI las variables `DOC10_E2E_MODULE`, `DOC10_E2E_AUTHORIZED_USER`, `DOC10_E2E_AUTHORIZED_PASSWORD`, `DOC10_E2E_UNAUTHORIZED_USER`, `DOC10_E2E_UNAUTHORIZED_PASSWORD`, `DOC10_E2E_TASK_ID`, `DOC10_E2E_MYSQL_URL` y `DOC10_E2E_AUDIT_SQL`; después se ejecuta `npm.cmd --prefix tools\e2e run test:e2e`. El detalle reproducible está en `tools/e2e/README.md`.

Ejemplo, solo en ambiente de pruebas y con un usuario MySQL de lectura:

```powershell
$autorizado = Get-Credential
$noAutorizado = Get-Credential
$conexionLectura = $env:DOC10_E2E_MYSQL
powershell.exe -ExecutionPolicy Bypass -File tools\e2e\Invoke-Doc10PreviewE2E.ps1 `
  -BaseUri 'https://qa.example/app/' -ModuleValue 'WORKFLOW' `
  -AuthorizedCredential $autorizado -UnauthorizedCredential $noAutorizado -IdTarea 123 `
  -ReadOnlyConnectionString $conexionLectura `
  -AuditProbeSql 'SELECT COUNT(*) AS total FROM <tabla_auditoria> WHERE <campo_tarea> = @idTarea' `
  -EvidencePath 'Doc\Actualizacion\workflow\Terminar\02-preview-ruta-flujo\evidencias\qa-preview-123.json'
```

La consulta de auditoría debe ser aprobada por el responsable del ambiente, usar `@idTarea`, tener orden determinista si retorna filas y no contener otro comando que `SELECT`.

### Ejecución E2E autenticada — 2026-08-14

El ASMX está desplegado localmente. La E2E completa `test:e2e` se ejecutó con el piloto, el usuario no piloto y las tareas 879 (`FLUJO`) y 922 (`RUTA`). La conexión al módulo Workflow y las consultas de estado/auditoría se obtuvieron sólo en memoria desde la configuración dinámica del módulo; todas las consultas usadas para las huellas fueron `SELECT` parametrizados.

La reejecución posterior a la corrección de semántica devolvió un destino `FLUJO` al piloto y `WORKFLOW_MODERN_INACTIVE` con cero destinos al usuario no piloto. Las huellas de `estados_tarea_workflow` y `wf_log_estados_workflow` fueron iguales antes/después, por lo cual se confirma que el preview no mutó esos datos. La evidencia versionable y sin secretos está en [qa-preview-879.json](evidencias/qa-preview-879.json). La ruta 922 se reejecutó después de separar catálogos: devolvió dos destinos `RUTA`, conservó las mismas huellas y su evidencia segura está en [qa-preview-922.json](evidencias/qa-preview-922.json).

### Carga autenticada y decisión de asincronía — 2026-08-14

La utilidad `tools/e2e/scripts/run-doc10-concurrency.cjs` mide el POST de `PreviewEnviarTarea` después de crear sesiones Gestión independientes. Usa una solicitud por sesión, estado y auditoría antes/después mediante `SELECT`, y conserva únicamente métricas, códigos públicos y huellas. El login se dosifica para no mezclar su costo con la ráfaga del ASMX.

- Nivel 20: una corrida completó 20/20 solicitudes de preview, sin errores ni mutación; p50 108,47 ms y p95 119,54 ms.
- Nivel 30: la corrida validada completó 30/30 solicitudes de preview, sin errores ni mutación; p50 51,20 ms y p95 68,71 ms.
- En dos corridas mixtas existieron `LOGIN_TIMEOUT` durante el bootstrap de Web Forms (3 de 30 y 10 de 20, respectivamente). Las sesiones que sí se autenticaron completaron todas sus solicitudes de preview. Por tanto, el hallazgo es de inicio de sesión/capacidad del host, no un timeout del ASMX ni una mutación de Workflow.

Con esta carga no hay evidencia para justificar convertir `PreviewEnviarTarea` a asíncrono: el ASMX respondió correctamente a 30 solicitudes simultáneas. La asincronía no resolvería por sí sola los timeouts del login ni la saturación de conexiones; antes de una decisión de modernización se debe repetir la prueba en un ambiente representativo y observar IIS, pool MySQL, CPU, memoria y latencia de login.

#### Comprobación de recursos locales del repositorio — 2026-08-14

- `MyDbContext` continúa siendo una configuración estática heredada y no se usa en el recorrido válido del ASMX DOC-10. Su prueba histórica de apertura fue rechazada por el servidor; no se ejecutó ninguna sentencia sobre esa conexión.
- `OdbcServices` y `OdbcServicesGestor` abren conexión de solo lectura contra el catálogo `docuarchi`. La E2E los usa únicamente para resolver en memoria la configuración del módulo Workflow; las huellas de tarea y auditoría se consultan contra el módulo 8, que sí contiene las tablas Workflow. El ASMX obtiene el snapshot Docuarchi desde la sesión autenticada para el estado documental de ruta.
- El gate específico del preview está en `WorkflowCentroTrabajoModernActive=false` y sus listas de usuarios/grupos están vacías. La configuración heredada de perfiles piloto no habilita este gate nuevo.
- `PreviewEnviarTarea` recibe `idTarea`, no `idRuta`. Los identificadores de ruta abierta/cerrada necesitan asociarse primero a tareas activas y autorizadas en el esquema Workflow correcto.

#### Contraste con `gestor_modulos` — 2026-08-14

- La fila activa de `GESTOR` es el catálogo (`docuarchi`); la fila Workflow asociada por `WF_DEFAULT_GESTOR` es el módulo 8. Su conexión MySQL fue validada solo al abrirse y sí contiene las tablas Workflow.
- `Defaul/GestorModuleSesion.vb` consulta `gestor_modulos`, copia host, base, usuario, clave, pool y máximo de conexiones a la sesión, y `Defaul/conect.vb` construye la conexión MySQL dinámica con esos valores.
- El ASMX válido ya no usa la cadena estática: crea `WorkflowModuleConnectionFactory` con el snapshot Workflow contenido en la sesión autenticada para tarea, flujo y destinos, y `DocuarchiModuleConnectionFactory` para el estado documental de ruta. Los constructores por defecto con `ModuleConnectionFactory("MyDbContext")` permanecen exclusivamente para la respuesta fail-closed, que no abre conexión.
- La identidad de Gestión no se compara directamente por `usuario_workflow.Nombre_Usuario`. `Gestion/ClassGestionDocumental.vb` obtiene `Relacion_Workflow` desde `docuarchi.remit_dest_interno` y lo asigna a la sesión. La relación activa del usuario de prueba apunta al usuario Workflow que tiene asignadas ambas tareas consultadas.
- Las consultas de solo lectura sobre el módulo 8 muestran que la tarea 879 es `FLUJO`, está asignada al usuario Workflow relacionado y tiene un conector saliente autorizado configurado. El flujo 22 y su actividad 172 tienen valor `1` en los campos de libertad de asignación; esos valores no representan disponibilidad de envío y no se usan para ocultar el conector. La reejecución E2E confirmó un destino. La 922 es `RUTA`, está asignada al usuario relacionado, tiene tipo documental abierto en Docuarchi y dos destinos configurados en Workflow.
- El primer intento de la 922 expuso que `tipo_doc_entrante` no existe en el catálogo Workflow: la consulta se convertía en `WORKFLOW_TRANSITION_INCONSISTENT`. La corrección separó el estado documental hacia `DocuarchiModuleConnectionFactory`; la tarea y los destinos se mantienen en `WorkflowModuleConnectionFactory`.
- El gate `webservice/WorkflowPreviewSessionContextGate.vb` vuelve a validar la relación Gestión → Workflow en cada llamada Gestión y establece únicamente usuario, grupo, ruta y login Workflow relacionados. No llama `InicializaSesionModuloWorkflow`, por lo que no registra auditoría, carga permisos ni compila scripts. Entrega snapshots de Workflow y Docuarchi a Presentation, sin que los repositorios lean `Session`.

#### Intento de sesión Gestión → Workflow — 2026-08-14

- Se ejecutó `npm.cmd --prefix tools/e2e run test:session` contra el ASMX local con el módulo `GESTOR` y la tarea `879`. La prueba usa el enlace visible del formulario y espera su postback Web Forms; ya no depende erróneamente de que desaparezcan los controles de login.
- El primer intento omitió un carácter final de la clave. La comprobación posterior descifró el valor en memoria con la rutina legacy y confirmó la variante corregida, sin imprimir ni almacenar el valor cifrado o descifrado.
- La repetición `test:session` pasó 1/1: el postback dejó una sesión Gestión válida y el ASMX resolvió el contexto Workflow; la aserción confirmó que la respuesta no fue `WORKFLOW_CONTEXT_INVALID`.
- La cuenta candidata a no piloto también pasó `test:session` 1/1. Esto confirma autenticación y contexto, pero no sustituye la prueba de autorización: con el gate apagado todos reciben el mismo bloqueo.
- Con autorización explícita, se activó temporalmente el gate local y se limitó al piloto. `test:authorization` pasó 1/1: el piloto superó el gate y el usuario no piloto recibió `WORKFLOW_MODERN_INACTIVE` sin destinos. Después de la corrección, la tarea 879 devuelve su conector autorizado. Al terminar se restauraron `WorkflowCentroTrabajoModernActive=false` y las listas de alcance vacías.
- `test:e2e` se reejecutó 1/1 con la tarea 879 y comparó las huellas de estado y de `wf_log_estados_workflow` antes/después; ambas permanecieron iguales. El piloto obtuvo un destino y el no piloto permaneció bloqueado por gate. La evidencia segura está en `evidencias/qa-preview-879.json`. El gate se restauró al estado apagado inmediatamente después.
- Después de corregir la separación de catálogos, `test:e2e` pasó 1/1 con la tarea 922: el piloto obtuvo dos destinos `RUTA`, el no piloto recibió `WORKFLOW_MODERN_INACTIVE` y las huellas de estado/auditoría no cambiaron. La evidencia segura está en `evidencias/qa-preview-922.json`. El gate se restauró inmediatamente a apagado y las listas de alcance quedaron vacías.
- Se implementó `test:load` para carga autenticada. La evidencia inicial registró 20/20 previews correctos (p95 119,54 ms) y la evidencia validada registró 30/30 previews correctos (p95 68,71 ms), siempre sin mutación. Los `LOGIN_TIMEOUT` de corridas mixtas quedan separados como riesgo del bootstrap Web Forms; no se atribuyen al endpoint ni se compensan con asincronía. El gate se restauró apagado al finalizar cada corrida.
- La QA manual mínima se ejecutó con sesión GESTOR del piloto y tarea 922. La respuesta HTTP 200 fue `RUTA`, con dos destinos (`CONTADOR` y `SUPERVISOR`), sin error, requisitos ni notificación. La evidencia resumida sin secretos está en [qa-manual-922.json](evidencias/qa-manual-922.json); las capturas fueron entregadas por QA.
- La garantía de no mutación para la 922 permanece en la E2E [qa-preview-922.json](evidencias/qa-preview-922.json): compara estado y `wf_log_estados_workflow` antes/después mediante `SELECT`. La QA manual no ejecutó terminación, correo ni transición.

Se deben conservar por cada corrida: ambiente, usuario/rol sin contraseña, identificador de tarea, configuración del gate, respuesta resumida, huellas antes/después de estado y auditoría, resultado y fecha.
