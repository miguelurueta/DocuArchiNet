# Arquitectura

## Frontera de responsabilidades

DOC-12 agrega una capa de Presentation. El servidor mantiene la autoridad sobre sesión, permisos, piloto, tarea, ruta y destinos.

| Función o selector | Ruta | Responsabilidad | Parámetros / DTO | Estado UI | Dependencia legacy permitida |
| --- | --- | --- | --- | --- | --- |
| `WorkflowModernPresentationBootstrap` | `workflow/WorkflowModernPresentationBootstrap.vb` | Obtiene el contexto de sesión y evalúa `IWorkflowModernFeatureGate`. | `ResultadoContextoSesionWorkflow`, `HabilitacionWorkflowModern` | Bootstrap activo/inactivo | `WorkflowPreviewSessionContextGate` y `ConfiguracionWorkflowModernFeatureGate`. |
| `#workflow-transition-trigger` | `workflow/Webworkflow.aspx` y `.vb` | El code-behind señaliza activación y origen de `idTarea`; conserva el `onclick` legacy hasta que el JS moderno lo sustituye con gate activo. | `data-workflow-modern-active`, id de input de tarea | Inactivo / cargando | Enlace Continuar e `ImageButtonterminar`. |
| `#workflow-transition-modern-modal` | `workflow/Webworkflow.aspx` | Host semántico del modal; no es `UpdatePanel`. | Ninguno | Todos | Ninguna operación legacy. |
| `WorkflowTransitionUi` | `js/workflow/workflow-transition-ui.js` | Solicita preview, construye DOM y publica selección. | `PrevisualizacionTransicionDto`, `DestinoTransicionDto` | Cargando, sin destinos, error, lista, seleccionado | Solo lee el input de tarea; el ASMX vuelve a validar. |
| `.workflow-transition-modal` | `Styles/workflow-transition-modern.css` | Estilos encapsulados de contraste, foco y adaptación. | Clases CSS | Todos | Ninguna. |

## Activación por bandera

La página ya tenía `WorkflowCentroTrabajoModernActive`, basado en otro conjunto de claves y perfiles. DOC-12 no lo usa para la lista. El bootstrap instancia `ConfiguracionWorkflowModernFeatureGate` a través de `IWorkflowModernFeatureGate` y usa el contexto de `WorkflowPreviewSessionContextGate`, igual que `WebServiceWorkflowModern.asmx`.

El atributo visual no es una autorización: si un navegador cambia el DOM, `PreviewEnviarTarea` vuelve a validar contexto y feature gate en el servidor.

Para que `AjaxControlToolkit.ToolkitResourceManager` pueda registrar sus recursos, el marcado dentro de `<head runat="server">` no contiene bloques `<% ... %>`. Cuando el gate está activo, `Webworkflow.aspx.vb` agrega los recursos DOC-12 al `Page.Header` y registra el atributo bootstrap al final de la página; cuando está inactivo no agrega recursos ni cambia el `onclick` legacy.

## Alternativas descartadas

- Reutilizar `WorkflowCentroTrabajoModernActive`: podía divergir del piloto que protege el ASMX.
- Generar destinos en VB o reutilizar `GridView_envia_flujo`: duplicaría el flujo legacy y requeriría postback.
- Llamar la operación de ejecución desde la lista: queda fuera de DOC-12; la lista solo publica un contrato para la confirmación posterior.
