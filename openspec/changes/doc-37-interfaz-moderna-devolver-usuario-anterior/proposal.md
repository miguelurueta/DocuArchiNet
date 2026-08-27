## Why

DOC-36 expone el preview y la ejecución seguros para devolver una tarea al usuario histórico inmediatamente anterior, pero `Webworkflow.aspx` conserva un enlace que activa la ruta Web Forms heredada. DOC-37 sustituye solo ese enlace por una experiencia moderna y accesible, sin habilitar rutas alternativas ni cambiar las demás operaciones Workflow.

## What Changes

- Registrar una presentación exclusiva de **Devolver a usuario anterior** para todo contexto Workflow válido, independiente de `WorkflowCentroTrabajoModernActive`.
- Reemplazar el trigger heredado y retirar su postback, control oculto, handler y JavaScript asociados.
- Agregar un adaptador JavaScript exclusivo que consulte `PreviewDevolverUsuarioAnterior`, conserve el token opaco y ejecute únicamente `EjecutarDevolverUsuarioAnterior` después de confirmación.
- Reutilizar `ConfirmationDialog`, las convenciones de foco, teclado, Escape, bloqueo durante ejecución y la actualización localizada de la bandeja existentes.
- Añadir pruebas focales CJS y documentación técnica de contratos, aislamiento y no regresión.
- Extender el orquestador E2E local de DOC-36 con un perfil DOC-37 no sensible, recursos de UI aislados y etapas autorizadas. Cada invocación valida una sola tarea seleccionada. La extensión no aprovisiona ni configura un ambiente autenticado.

## Scope

La modificación se limita a `workflow/Webworkflow.aspx`, su code-behind y diseñador, `js/workflow/`, estilos existentes, `tools/e2e/`, pruebas focales y documentación DOC-37. Los endpoints, contratos y capas de servidor de DOC-36 se consumen sin modificación.

## Capabilities

### New Capabilities

- `interfaz-moderna-devolver-usuario-anterior`: presentación única, accesible y aislada para la devolución al usuario histórico.

### Modified Capabilities

- Ninguna.

## Impact

- La única ruta alcanzable desde el menú **Devolver → Usuario anterior** deja de ser un postback Web Forms.
- Devolver a actividad anterior, Enviar a usuario, Enviar a grupo y Continuar flujo conservan sus selectores, eventos, payloads y contratos.
- La futura E2E de interfaz reutiliza recursos desechables ya definidos por DOC-36, pero exige autorización explícita de ambiente y cuentas antes de cualquier corrida autenticada.
