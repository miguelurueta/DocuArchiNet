<!-- opsxj:refinement-traceability version=1 artifact=spec decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07 -->
## ADDED Requirements

### Requirement: RQ-01 Contexto de confirmación verificable (D-01)

El sistema SHALL construir el resumen de confirmación exclusivamente con el preview normalizado y el destino previamente seleccionado.

#### Scenario: Datos no publicados por el preview

- **WHEN** el preview no contiene trámite, actividad origen legible, requisitos o advertencias
- **THEN** la confirmación omite esos campos
- **AND THEN** no los deriva de IDs, controles ocultos, HTML legacy, Session ni reglas de negocio de cliente

### Requirement: RQ-02 Diálogo reutilizable y libre de Workflow (D-02)

El sistema SHALL exponer `ConfirmationDialog.open(config)` y `ConfirmationDialog.close()` como una API genérica, accesible y aislada.

#### Scenario: Consumidor ajeno a Workflow

- **WHEN** otro módulo abre el diálogo con campos, etiquetas, callbacks y un contexto opaco simulado
- **THEN** el diálogo representa el contenido y procesa sus callbacks sin requerir rutas, selectores, globals ni textos del módulo Workflow
- **AND THEN** usa APIs DOM seguras y no consulta Session, SQL, repositorios ni controles Web Forms

### Requirement: RQ-03 Integración tipada de Workflow (D-03)

El sistema SHALL mantener en el adaptador Workflow la conversión entre la selección moderna, el contrato del ASMX y el contrato visual genérico.

#### Scenario: Apertura desde un destino seleccionado

- **WHEN** `WorkflowTransitionUi` publica una selección válida con tarea, conector, token y resumen normalizado
- **THEN** el adaptador abre la confirmación con los textos y campos propios de Workflow
- **AND THEN** el componente genérico no conoce `Webworkflow.aspx`, sus IDs, el nombre de la actividad ni reglas de envío

### Requirement: RQ-04 Envío asíncrono único y correlacionado (D-04)

El sistema SHALL solicitar `EjecutarEnvioTarea` solo desde el adaptador y con la terna validada `{ idTarea, idConector, tokenVersion }`.

#### Scenario: Doble clic, cierre o navegación durante el envío

- **WHEN** la persona intenta enviar dos veces, cerrar con `X`, Cancelar, fondo, Escape o `close()`, abrir otra confirmación, o cerrar/recargar la pestaña durante el envío
- **THEN** el diálogo no inicia una segunda ejecución útil ni se cierra hasta obtener resultado; la navegación solicita la confirmación nativa del navegador
- **AND THEN** éxito, bloqueo o error técnico recuperan las acciones de acuerdo con su contrato
- **AND THEN** el navegador no agrega usuario, grupo, ruta, permisos, requisitos ni datos derivados a la solicitud

### Requirement: RQ-05 Resultado funcional y recuperación segura (D-05)

El sistema SHALL actualizar la página únicamente después de un resultado exitoso y correlacionado del servidor.

#### Scenario: Éxito, bloqueo y error técnico

- **WHEN** la respuesta normalizada es éxito
- **THEN** el callback actualiza solo la tarea afectada, restablece la lista, limpia su contexto y acciones, actualiza el contador y muestra una confirmación no intrusiva durante seis segundos
- **WHEN** la respuesta es bloqueo funcional o error técnico controlado
- **THEN** la tarea y el contexto permanecen disponibles, el diálogo muestra una causa segura y restaura acciones conforme al contrato
- **AND THEN** una excepción o fallo de red usa el mensaje técnico seguro configurado y no expone `Error.message` ni detalles del navegador

### Requirement: RQ-06 Convivencia y reversa legacy (D-06)

El sistema SHALL habilitar la confirmación moderna solamente con el bootstrap respaldado por `IWorkflowModernFeatureGate` y conservar el flujo legacy fuera del piloto.

#### Scenario: Bandera inactiva durante la navegación

- **WHEN** el bootstrap no está activo o la ejecución devuelve `WORKFLOW_MODERN_INACTIVE`
- **THEN** la interfaz moderna no sustituye ni ejecuta fallback sobre los modales, controles y postbacks legacy
- **AND THEN** la desactivación de la bandera permite volver al comportamiento previo sin migración ni cambio de estado

### Requirement: RQ-07 Evidencia reproducible (D-07)

La entrega SHALL incluir pruebas focales, build, QA accesible y responsive, y documentación técnica en la ruta establecida.

#### Scenario: Verificación previa al cierre

- **WHEN** se valida DOC-13 antes de archivar
- **THEN** la evidencia cubre el diálogo genérico, el adaptador, estados, correlación, cancelación, teclado, ARIA, contraste y vistas móvil y escritorio
- **AND THEN** el paquete `Doc/Actualizacion/workflow/Terminar/05-confirmacion-especializada/` registra comandos, resultados, límites y rollback sin exponer información sensible
