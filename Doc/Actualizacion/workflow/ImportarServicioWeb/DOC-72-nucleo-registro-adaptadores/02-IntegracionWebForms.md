# Integración WebForms y gate

## Ciclo de carga

`Webworkflow.Page_Load` llama `ConfigureWorkflowTransitionModernPresentation`. Este método registra presentación compartida y retorna inmediatamente cuando `WorkflowTransitionModernActive` es falso. Solo después del retorno condicional ejecuta `RegisterImportarServicioWebModernAssets` y `RegisterImportarServicioWebModernBootstrap`.

Los cuatro scripts se agregan dinámicamente a `Page.Header` con IDs estables para evitar duplicación. El bootstrap usa `ScriptManager.RegisterStartupScript`, localiza `ctw-document-action-service` y agrega:

- `data-import-modern-active="true"`;
- `data-import-task-input-id` con el `ClientID` de `Hidden_id_tarea_selecionada`;
- `data-import-provider-id` leído de `ImportarServicioWebProviderId` y codificado con `JavaScriptStringEncode`.

## Coexistencia

| Condición | Resultado implementado |
| --- | --- |
| Gate falso | No se cargan assets DOC-72; `btnloadservice` conserva listener legacy |
| Gate verdadero y proveedor vacío | Se abre modal y core termina en `error/PROVIDER_NOT_CONFIGURED`; no hay llamada HTTP |
| Gate verdadero y proveedor configurado | UI registra el adaptador, oculta `btnloadservice`, reemplaza `onclick` del disparador moderno y consulta |
| Bootstrap repetido por UpdatePanel | `data-import-modern-bound="true"` evita doble enlace del mismo nodo; si el nodo es reemplazado se vuelve a enlazar |

## Reversa

La reversa operativa es `WorkflowCentroTrabajoModernActive=false`, usuarios/grupos vacíos. No requiere retirar archivos para restablecer legacy. DOC-72 no autoriza activar el gate automáticamente ni modifica autorización backend.

## Elementos no verificados en navegador

No se ejecutó una sesión WebForms autenticada ni UpdatePanel real porque no hubo autorización de ambiente/cuentas. La idempotencia de bootstrap y el DOM se verifican estructuralmente y por pruebas focales, no mediante E2E.
