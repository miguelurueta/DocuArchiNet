# interfaz-moderna-enviar-usuario Specification

## Purpose

Define la experiencia oficial, accesible y aislada para enviar una tarea Workflow a un usuario autorizado mediante preview paginado y ejecución controlada.

## Requirements

### Requirement: RQ-01 / D-01 Entrada moderna oficial de Enviar a usuario

La página `workflow/Webworkflow.aspx` SHALL exponer `workflow-user-send-trigger` como única entrada de **Enviar a usuario** para todo contexto Workflow válido, sin depender de `WorkflowCentroTrabajoModernActive`.

#### Scenario: El gate de otras operaciones está apagado

- **WHEN** el contexto Workflow es válido y el gate de Grupo/Continuar flujo está apagado
- **THEN** el bootstrap de usuario enlaza su disparador moderno y no consulta ni modifica el gate.

#### Scenario: No existe ruta legacy de usuario

- **WHEN** el usuario activa el comando moderno
- **THEN** no se invoca `ImageButtonEnviarUsuario`, un postback, un handler o un modal Web Forms de envío a usuario.

### Requirement: RQ-02 / D-02 Búsqueda y ejecución con contrato directo paginado

El adaptador de usuario SHALL consumir exclusivamente los endpoints JSON de DOC-28 y SHALL conservar el destino directo usuario–actividad y el token de versión.

#### Scenario: Búsqueda de destinos por cursor

- **WHEN** el usuario abre el modal, escribe una búsqueda o navega de página
- **THEN** el cliente envía `{ idTarea, consulta, cursor, tamanoPagina }` a `PreviewEnviarUsuario`, muestra solo la página recibida y descarta respuestas obsoletas.

#### Scenario: Ejecución de una selección vigente

- **WHEN** el usuario confirma un destino de la página vigente
- **THEN** el cliente envía únicamente `{ idTarea, idUsuarioWorkflowDestino, idActividadDestino, tokenVersion }` a `EjecutarEnvioUsuario` y nunca incluye `IdConector`.

### Requirement: RQ-03 / D-03 Operación accesible y aislada de Continuar flujo

La interfaz SHALL usar modal, selectores, eventos, estado de solicitudes y confirmación propios para Enviar a usuario.

#### Scenario: Navegación accesible y cancelación

- **WHEN** se abre el modal o la confirmación
- **THEN** foco, Tab/Shift+Tab, Escape, fondo y cancelar permiten abandonar la operación y restituir el foco sin postback legacy.

#### Scenario: Cierre bloqueado durante el envío

- **WHEN** el usuario confirma el destino y `EjecutarEnvioUsuario` continúa pendiente
- **THEN** los controles de confirmar, cancelar y cerrar quedan deshabilitados y X, fondo, Escape o un nuevo intento de abrir confirmación no cierran ni reemplazan el diálogo; tras una respuesta controlada se recupera el cierre según el resultado.
- **AND** un intento de cerrar o recargar la pestaña solicita la confirmación nativa del navegador mientras haya una operación pendiente.

#### Scenario: Cambio de contexto y antirregresión

- **WHEN** llega una respuesta obsoleta, cambia el término/página o se cancela la confirmación
- **THEN** la selección se invalida y no se ejecuta un destino de otro contexto ni se registran listeners, estado o payload de `WorkflowTransitionUi`.

### Requirement: RQ-04 / D-04 Actualización parcial correlacionada

Tras éxito de Enviar a usuario, la presentación SHALL actualizar exclusivamente tarea afectada, visor, contador y mensaje de éxito propio.

#### Scenario: Envío exitoso

- **WHEN** `EjecutarEnvioUsuario` devuelve éxito para tarea y token seleccionados
- **THEN** la fila afectada se elimina una vez, visor/contexto se limpia, contador disminuye una vez y se anuncia éxito de usuario sin refrescar la lista completa.

### Requirement: RQ-05 / D-05 Verificación y límites operativos

La entrega SHALL aportar pruebas focales y compilación reproducibles, sin operaciones autenticadas no autorizadas.

#### Scenario: Evidencia local

- **WHEN** se valida DOC-29 localmente
- **THEN** CJS focal y MSBuild terminan sin errores y la documentación registra comando, resultado y cobertura.

#### Scenario: E2E no autorizado

- **WHEN** no existe autorización explícita de ambiente y cuentas
- **THEN** no se activa gate, no se ejecuta E2E/carga ni transición real y la evidencia consigna la limitación.
