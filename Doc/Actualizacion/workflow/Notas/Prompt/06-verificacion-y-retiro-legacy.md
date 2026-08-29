# Prompt 06 — Verificar con E2E reales y retirar legacy controladamente

## Prompt para ejecutar

```text
Aplica primero el contexto común de Prompt/00-guia-de-uso-y-contexto-comun.md. Ejecuta esta fase solo cuando el consumidor Centro de Trabajo Workflow tenga adaptación moderna revisada, la política registrada de Notas esté aplicada y el preflight de cada esquema activo haya sido aprobado.

La implementación de E2E forma parte del mismo cambio de modernización de Notas y de su criterio de cierre. Usa `bloque-e2e-integrado-en-modernizacion.md` como bloque incorporado al prompt principal; no abras una tarea, historia ni cambio independiente únicamente para la automatización.

Controles E2E obligatorios: reutiliza exclusivamente `tools/e2e`, su autenticación, configuración, validadores, evidencias y utilidades; no crear login, arnés, proyecto Playwright, configuración ni `.env` paralelos. Antes de una E2E autenticada lee `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`; ejecútala solo con ambiente, cuentas y datos o tareas descartables expresamente autorizados. Usa secretos efímeros y no exponer, imprimir ni persistir credenciales, cookies, tokens ni cadenas de conexión; las verificaciones son solo `SELECT` y toda evidencia saneada. Cubre, cuando aplique, autorización y control de acceso, lectura sin mutación, escrituras autorizadas, concurrencia y regresión. Respeta feature flags, gates, usuarios, grupos y seguridad sin habilitarlos arbitrariamente; la implementación no se considera terminada sin validación autorizada y registra bloqueo explícito sin mocks, simulaciones ni evidencia ficticia.

Objetivo: demostrar con pruebas automatizadas y E2E reales que la modernización de Notas dentro de `workflow/` satisface diagnóstico y requerimientos, y retirar de manera controlada únicamente rutas legacy de ese módulo sin uso, incluida la rutina duplicada de borrado. Las E2E no pueden sustituirse por mocks; se ejecutan únicamente con autorización explícita.

Rol esperado: arquitecto y desarrollador senior de ASP.NET Web Forms/VB.NET, automatización E2E y MySQL de solo lectura, responsable de producir evidencia saneada y de retirar únicamente rutas legacy verificadamente sin consumidores.

Contexto obligatorio: revisa la propuesta OpenSpec, Exploración, requerimientos, matriz de pruebas, `AGENTS.md`, `tools/e2e/AGENT-RUNBOOK.md`, `tools/e2e/package.json`, las suites DOC-28/DOC-32 y las rutas legacy de Notas bajo `workflow/` en páginas `.aspx`, code-behind `.vb`, `webservice/`, scripts y estilos. Ubica E2E y validadores solo bajo `tools/e2e/`; los cambios de retiro se limitan a la ruta legacy inventariada y al Centro de Trabajo confirmado.

Restricciones críticas:
- No ejecutar E2E real, carga, escrituras ni activar gate sin las autorizaciones explícitas, ambiente, cuentas y tareas descartables definidos por el runbook.
- No crear login, `.env`, cliente de autenticación, harness paralelo, usuarios virtuales, mutaciones fuera de las tareas autorizadas ni consultas de control que no sean `SELECT` parametrizados.
- No imprimir ni persistir secretos, cookies, cadenas de conexión, contenido de notas o cuerpos de respuesta; conserva solo evidencia saneada.
- No retirar tablas/datos, cambiar semántica de borrado, ocultar cambios ajenos, modificar módulos externos ni retirar una ruta Workflow con referencias vivas; el gate termina en `false` con usuarios y grupos vacíos.

Reglas de anti-regresión: conserva las rutas legacy de Workflow hasta contar con referencias cero y regresión aprobada; no reemplaza evidencia real por simulaciones, no revierte cambios ajenos y valida antes/después que páginas, eventos y rutas Workflow no afectadas mantienen su comportamiento.

Pruebas obligatorias: ejecuta pruebas focales y de regresión, compila con MSBuild o `dotnet` si cambia código VB, y ejecuta E2E real solo con el gate de autorización aplicable. Registra comandos, resultados, latencias, conteos, huellas y bloqueos en evidencia saneada; cuando falte autorización, documenta la razón y no declares la E2E aprobada ni la sustituyas por un mock.

Documentación técnica: actualiza bajo `Doc/Actualizacion/workflow/Notas/` la trazabilidad, propuesta OpenSpec, requerimientos, inventario legacy de Workflow, matriz de pruebas y evidencia de retiro. El informe debe describir el flujo paso a paso de autorización, lectura, escritura autorizada, control antes/después, rollback y retiro; no crear documentos en la raíz.

Entregable final: entrega trazabilidad requisito-código-prueba, suites/validadores integrados, comandos y resultados, evidencia saneada, inventario de referencias, alcance Workflow efectivamente retirado, rollback por endpoint, riesgos, deuda y confirmación explícita del gate final.

Alcance: RF-14, RF-20; RNF-05, RNF-06, RNF-08, RNF-09 y RNF-10; matriz de pruebas y trazabilidad completas.

1. Construye una matriz de trazabilidad desde los hallazgos P0/P1/P2 aplicables a Workflow hacia RF, RN, RS, RNF, código y pruebas. Identifica cualquier requisito sin evidencia como bloqueo de retiro.
2. Ejecuta las pruebas automatizadas pertinentes dentro de Workflow: autorización, tarea explícita entre pestañas, TOCTOU/mutación condicional, versión, idempotencia, auditoría transaccional, escape de contenido, cursores, rendimiento lógico de COUNT y regresión del Centro de Trabajo.
3. Completa únicamente las brechas E2E que queden para el retiro que este cambio introduce; no conviertas la automatización de fases anteriores en una tarea o entrega independiente. Reutiliza obligatoriamente `tests/support/authenticated-workflow-session.cjs`, `playwright.config.cjs`, el patrón de validación de configuración y los controles de evidencia de las suites DOC-28/DOC-32. No crees login, `.env`, cliente de autenticación ni harness paralelo.
4. Agrega comandos `npm` y validadores de configuración específicos de Notas de Workflow, coherentes con `tools/e2e/package.json`. Deben separar, como mínimo: anónimo sin sesión; lectura real no mutante; escritura real sobre tarea descartable; y, si corresponde a la decisión de negocio, concurrencia controlada de dos solicitudes. No implementar carga ni usuarios virtuales.
5. La suite real debe cubrir: rechazo anónimo; listado/consulta autorizado; ausencia de mutación de estado y auditoría en lectura; crear idempotente; edición con conflicto de versión; aislamiento de dos contextos/tareas; eliminación según semántica aprobada; y evidencia de tarea, actor y auditoría correctos. El test obtiene los valores permitidos desde el contrato/preview vigente, no del navegador ni de variables de entorno que suplanten autorización.
6. Antes de cualquier E2E autenticado, lee y cumple `tools/e2e/AGENT-RUNBOOK.md`. Requiere URL, ambiente, cuentas y MySQL de solo lectura entregados mediante secretos efímeros. Para escritura exige además autorización explícita, tarea descartable y una variable de autorización específica. Las consultas de control son solo `SELECT`, de una única sentencia, con exactamente un parámetro `?` para la tarea.
7. Ejecuta la E2E real contra el ambiente autorizado cuando dichas autorizaciones estén presentes. No imprimas secretos, cookies, cadenas de conexión, contenido de notas ni cuerpos de respuesta. Conserva únicamente evidencia saneada: códigos, conteos, latencias y huellas antes/después. Si la autorización falta, deja el bloqueo explícito; no declares la fase validada ni la reemplaces por simulación.
8. Antes y después de la corrida, verifica que `WorkflowCentroTrabajoModernActive=false` y usuarios/grupos estén vacíos. Ejecuta además los controles de integridad del runbook; si detectan cambios no permitidos en páginas legacy de Workflow, detén la corrida y solicita dirección, sin ocultar ni revertir cambios ajenos.
9. Inventaría con búsquedas estáticas todas las referencias bajo `workflow/` a endpoints, scripts y rutinas legacy de notas, especialmente las dos rutas de borrado y WebFormAnotacion. Diferencia código realmente usado de artefactos no referenciados.
10. Solo después de evidencia de cero referencias dentro de Workflow, E2E real aprobada y una ruta moderna equivalente, retira una pieza legacy por cambio atómico y reversible. No borres tablas ni datos. No cambies semántica de borrado durante el retiro.
11. Confirma que no queden rutas modernas leyendo Session("ID_TAREA_SELECCIONDA"), concatenando SQL/JSON, exponiendo excepciones o usando innerHTML para notas.
12. Conserva una estrategia de rollback por endpoint Workflow hasta la ventana de estabilización aprobada. La bandera WorkflowCentroTrabajoModernActive debe terminar false, con usuarios y grupos vacíos.
13. Actualiza los documentos de Exploración, requerimientos, propuesta OpenSpec y matriz de pruebas con evidencia verificable, decisiones aplicadas y deuda conscientemente aplazada.

Criterios de aceptación:
- Cada hallazgo del diagnóstico tiene mitigación y prueba identificable, o una excepción aprobada explícitamente.
- Existe una suite E2E real de Notas integrada a `tools/e2e`, con sesión reutilizada y validadores de configuración; no hay harness ni autenticación paralela.
- La evidencia E2E de lectura prueba huellas sin cambios; la de escritura autorizada prueba resultados y auditoría esperados sin revelar datos sensibles.
- El borrado duplicado solo se retira después de confirmar referencias y regresión; no se pierde funcionalidad viva.
- El consumidor Centro de Trabajo preserva rollback y no existe doble escritura.
- El gate permanece deshabilitado y no se efectuaron operaciones fuera del ambiente, cuentas y tareas explícitamente autorizados.

Entrega un informe final con alcance Workflow retirado, comandos E2E ejecutados, evidencia saneada, estado del Centro de Trabajo, riesgos residuales, instrucciones de rollback y confirmación explícita del estado del gate.
```
