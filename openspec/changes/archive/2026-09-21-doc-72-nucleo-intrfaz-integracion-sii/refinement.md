<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-72-nucleo-intrfaz-integracion-sii

## Fuente y alcance

- Ticket: `DOC-72` — NUCLEO-INTRFAZ-INTEGRACION-SII.
- Fuente funcional: `specs/nucleo-intrfaz-integracion-sii/jira-context.md`.
- Perfil verificado: ASP.NET WebForms VB.NET, JavaScript compatible con el navegador vigente y CSS propio del feature.
- Contrato consumido: `Doc/Actualizacion/workflow/ImportarServicioWeb/CONTRATO-COMPARTIDO-FRONTEND-BACKEND.md`; las ocho operaciones modernas existen en `webservice/WebServiceImportarServicioWebModern.asmx.vb`.

## Contexto inspeccionado

- `workflow/Webworkflow.aspx` contiene `btnloadservice` y `ctw-document-action-service`; el segundo ya pertenece a la presentación moderna.
- `workflow/Webworkflow.aspx.vb` resuelve el gate con `WorkflowModernPresentationBootstrap.EstaActivaParaSolicitudActual()` y registra assets modernos desde code-behind.
- `Web.config` conserva `WorkflowCentroTrabajoModernActive=false`, usuarios y grupos vacíos; no se modificarán para desarrollar ni probar localmente.
- `js/workflow/workflow-transition-ui.js`, `js/workflow/workflow-transition-page-presentation.js` y `Styles/workflow-transition-modern.css` son referencias de encapsulación y accesibilidad, sin acoplar este feature con Terminar.
- Las rutas frontend y las tres pruebas focales solicitadas por DOC-72 todavía no existen.
- El contrato exige una intención, una llamada síncrona a `ExecuteImportIntent`, espera global indeterminada y resultados finales asociados a la tarea visible.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia de código | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | Encapsular el núcleo en cuatro módulos; `api.js` será el único cliente ASMX y `ui.js` no hará AJAX directo. | Contrato compartido; `webservice/WebServiceImportarServicioWebModern.asmx.vb` | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | Resolver proveedores mediante registro explícito e identidad canónica; proveedor ausente, no migrado o desconocido falla de forma diferenciada y nunca se redirige a SII. | `Services/Workflow/ImportarServicioWeb/RegistroClientesProveedoresImportacion.vb`; `tests/importar-servicio-web-provider-registry.test.cjs` | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | Modelar estados cerrados sin lógica mutadora: una intención produce exactamente una llamada a `ExecuteImportIntent` y espera global indeterminada. | Contrato compartido, secciones 1 y 4; `Services/Workflow/ImportarServicioWeb/ImportServiceOrchestrator.vb` | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | Integrar desde `ctw-document-action-service` bajo el gate canónico, conservando `btnloadservice` como puente con gate apagado. | `workflow/Webworkflow.aspx`; `workflow/Webworkflow.aspx.vb` | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | Implementar modal accesible, foco inicial/restaurado, teclado, `aria-live` y estilos aislados; no reutilizar `JSProgresBar`. | Scripts y CSS modernos de Workflow; contrato compartido, secciones 1 y 5 | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | Validar con pruebas Node y dobles locales; cualquier E2E real exige autorización y restauración del gate apagado. | `AGENTS.md`; `tools/e2e/AGENT-RUNBOOK.md`; prueba de regresión del gate | D-06 | RQ-06 | Origen: D-06, RQ-06 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptación | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | Solo `api.js` conoce las rutas ASMX. | WHEN la UI consulta o ejecuta THEN delega en el cliente inyectado y no contiene AJAX directo. | Evita duplicar contratos y permite pruebas sin red. |
| RQ-02 | El registro devuelve adaptador y capacidades por identidad canónica. | WHEN el proveedor es vacío, no migrado o desconocido THEN devuelve error seguro específico y no selecciona SII. | Impide importar con proveedor incorrecto. |
| RQ-03 | El orquestador acepta transiciones declaradas y ejecuta una intención completa una sola vez. | WHEN hay uno o varios seleccionados THEN ocurre una llamada a `ExecuteImportIntent`, espera indeterminada y proyección final. | Evita doble mutación y fragmentación. |
| RQ-04 | Gate apagado preserva legacy; gate autorizado muestra una entrada moderna. | WHEN cambia el gate THEN se conserva el recorrido correspondiente sin duplicar entradas visibles. | Rollback por configuración sin tocar endpoints legacy. |
| RQ-05 | El modal cumple teclado y manejo de foco. | WHEN abre, cierra o cambia de estado THEN enfoca, restaura y anuncia de forma accesible. | Reduce regresiones WebForms/UpdatePanel. |
| RQ-06 | Las pruebas cubren estados, registro, gate y accesibilidad sin red ni secretos. | WHEN corren las suites DOC-72 THEN usan adaptadores falsos y no activan el gate real. | E2E autenticado requiere autorización separada. |

## Reglas de trazabilidad obligatorias

1. Cada decisión está desarrollada en `design.md`, reflejada en `spec.md` y vinculada a tareas con `Origen`.
2. El código nuevo queda limitado a las rutas canónicas y puntos aditivos declarados.
3. No se modifican almacenamiento legacy, endpoints mutadores, `js/Webworkflow.js`, `JSProgresBar.js` ni `App_Code/`.
4. El gate permanece `false` con usuarios y grupos vacíos durante esta entrega.

## Resultado del refinamiento

- Estado aprobado para sincronizar trazabilidad e iniciar implementación.
- La integración productiva consume el contrato existente; las pruebas usan adaptadores falsos.
- E2E real no está autorizado en esta sesión.
