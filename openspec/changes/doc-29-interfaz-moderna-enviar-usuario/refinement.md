<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-29-interfaz-moderna-enviar-usuario

## Fuente y alcance

- Ticket: `DOC-29` — INTERFAZ-MODERNA-ENVIAR-USUARIO
- Cambio OpenSpec: `doc-29-interfaz-moderna-enviar-usuario`
- Fuente Jira: `specs/interfaz-moderna-enviar-usuario/jira-context.md`
- Perfil tecnológico confirmado: ASP.NET Web Forms con VB.NET, JavaScript legacy sin bundler y pruebas CJS con `node --test`.

DOC-29 es la etapa 02: entrega solo la interfaz oficial de **Enviar a usuario** en `workflow/Webworkflow.aspx`. DOC-28 ya aprobó los endpoints, contratos, revalidación, lock, token y auditoría; este cambio no los altera.

## Contexto inspeccionado

- `workflow/Webworkflow.aspx:681` todavía activa `ImageButtonEnviarUsuario`; el control oculto, su handler y la cadena Web Forms son la ruta legacy que se retira para esta acción.
- `workflow/Webworkflow.aspx.vb:258-390` registra Grupo y Continuar flujo detrás de `WorkflowModernPresentationBootstrap`. El bootstrap de usuario debe ser uniforme, pero independiente del gate.
- `webservice/WebServiceWorkflowModern.asmx.vb:87-160` ya publica `PreviewEnviarUsuario(idTarea, consulta, cursor, tamanoPagina)` y `EjecutarEnvioUsuario(idTarea, idUsuarioWorkflowDestino, idActividadDestino, tokenVersion)` como JSON autenticado.
- Los adaptadores de grupo/transición, `ConfirmationDialog` y `WorkflowTransitionPagePresentation` aportan patrones reutilizables, pero los selectores, eventos, estado y payload de Continuar flujo no se pueden compartir.
- La exploración y el paquete técnico de `Doc/Actualizacion/workflow/TerminarUsuario/` muestran que DOC-28 cerró servidor y habilitó esta etapa.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia de código | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | Sustituir el enlace por un `button` moderno exclusivo y registrar su bootstrap aun con `WorkflowCentroTrabajoModernActive=false`; retirar el postback de usuario. | `Webworkflow.aspx:681,783`; `Webworkflow.aspx.vb:258-390` | D-01 | RQ-01 | 2.1, 2.2, 2.3 |
| D-02 | Consumir solo los endpoints existentes y paginar por `consulta`, `cursor` y `tamanoPagina`; el destino es usuario–actividad y token. | `WebServiceWorkflowModern.asmx.vb:87-160`; `WorkflowModernModels.vb:107-138` | D-02 | RQ-02 | 2.4, 2.5 |
| D-03 | Aislar selectores, eventos, estado y payload de usuario de `WorkflowTransitionUi`; reutilizar solo componentes genéricos de estilo y diálogo. | `workflow-transition-ui.js`; `workflow-group-send-ui.js`; `ConfirmationDialog.js` | D-03 | RQ-03 | 2.4, 2.5, 3.1, 3.2 |
| D-04 | Actualizar únicamente fila, visor, contador y un mensaje de éxito propio de usuario. | `workflow-transition-page-presentation.js:112-132`; `Webworkflow.aspx:4666` | D-04 | RQ-04 | 2.6, 3.2 |
| D-05 | Verificar con CJS, compilación y recorrido manual autorizado; no ejecutar E2E autenticado, carga ni cambios de configuración sin autorización. | `tests/workflow-group-send.test.cjs`; `tools/e2e/AGENT-RUNBOOK.md` | D-05 | RQ-05 | 3.1, 3.2, 3.3, 4.1, 4.2 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptación | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | El único comando visible es `workflow-user-send-trigger`; no usa gate ni postback legacy. | Con gate apagado el bootstrap enlaza el botón y no existe una ruta Web Forms de usuario. | Grupo y Continuar flujo permanecen sin cambios; se revierte el cambio versionado completo. |
| RQ-02 | Preview y ejecución son JSON `same-origin`, paginados y con el par usuario–actividad más token, sin `IdConector`. | Buscar, avanzar, retroceder o cambiar texto descarta respuestas obsoletas e invalida selección anterior. | El servidor sigue siendo la única autoridad de destino. |
| RQ-03 | Modal, eventos, estado y confirmación son propios; foco, Tab, Escape y cancelar no activan el flujo legacy. | Selección, cancelación o cambio de búsqueda no comparten listeners ni estado con Continuar flujo. | Se permite solo el componente genérico `ConfirmationDialog`. |
| RQ-04 | Un éxito elimina una sola fila, limpia visor/contexto, decrementa una vez el contador y anuncia mensaje de usuario. | No se refresca la lista completa ni se afecta el mensaje de Grupo o Continuar flujo. | La presentación actual de las otras operaciones conserva su comportamiento. |
| RQ-05 | CJS focal y MSBuild documentan cobertura y límites sin operación autenticada no autorizada. | Las pruebas pasan sin red; la documentación separa evidencia local de E2E/QA no ejecutada. | No se activa gate ni se ejecuta transición real. |

## Resultado del refinamiento

- Estado: aprobado tras revisar DOC-28, la ruta legacy, los bootstraps modernos y los contratos ASMX vigentes.
- La implementación inicia por el punto de entrada oficial y conserva Grupo/Continuar flujo como superficies independientes.
- Comando de control: `npm.cmd --prefix tools/opsxj run opsxj:refine -- DOC-29 --sync`.
