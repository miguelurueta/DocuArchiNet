<!-- opsxj:refinement-traceability version=1 artifact=spec decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08 -->
<!-- Decisiones aplicadas: D-01, D-02, D-03, D-04, D-05, D-06, D-07 y D-08. -->
## ADDED Requirements

### Requirement: Presentación moderna exclusiva de devolución

La página Workflow SHALL presentar **Elegir actividad anterior** mediante un trigger, modal y bootstrap exclusivos para todo contexto Workflow válido, sin evaluar `WorkflowCentroTrabajoModernActive` ni crear un gate nuevo. MUST NOT usar postback, controles ocultos, `UpdatePanel`, `ModalPopupExtender` ni los selectores, eventos o payloads de otras transiciones.

#### Scenario: Apertura con tarea seleccionada

- **WHEN** una persona activa el trigger moderno sobre una tarea válida
- **THEN** abre el modal exclusivo sin invocar `inicializa_tipo_adjunto_documento` ni una ruta Web Forms.

### Requirement: Preview paginado y aislado de actividades anteriores

La interfaz SHALL solicitar `PreviewDevolverActividad` con tarea, término, cursor y tamaño permitidos. SHALL aplicar término mínimo, debounce, páginas, cancelación, descarte de respuesta obsoleta e invalidación de toda selección que deje de corresponder al preview vigente. SHALL mostrar solamente los campos autorizados, incluido `IdConector` como referencia opaca contextual.

#### Scenario: Búsqueda o página reemplaza el preview vigente

- **WHEN** cambia la tarea, búsqueda, cursor o página mientras hay una solicitud pendiente o una actividad seleccionada
- **THEN** descarta el resultado anterior, invalida la selección y no permite ejecutar una transición con datos obsoletos.

### Requirement: Confirmación accesible y ejecución mínima

La interfaz SHALL confirmar la actividad elegida mediante el diálogo accesible existente y SHALL invocar `EjecutarDevolverActividad` solo con `idTarea`, `idConector` y `tokenVersion` procedentes del preview vigente. Mientras ejecuta SHALL impedir doble confirmación y cierre inseguro; los bloqueos, errores y timeout SHALL mantener un estado funcional para cancelar o reintentar sin fallback legacy.

#### Scenario: Resultado de ejecución bloqueado

- **WHEN** el servidor devuelve bloqueo de token, conector, concurrencia o autorización
- **THEN** la interfaz no modifica la bandeja, no inicia otra ejecución y presenta el mensaje funcional asociado.

#### Scenario: Respuesta de backend retenida

- **WHEN** la ejecución ya fue enviada al servidor y su respuesta permanece pendiente
- **THEN** la confirmación y el modal de devolución no permiten cancelar, cerrar, usar Escape ni abandonar la página hasta recibir un resultado.

### Requirement: Éxito puntual y accesible

La interfaz SHALL actualizar únicamente la tarea afectada, visor, contador y scroll mediante la presentación moderna compartida después de un éxito. SHALL restaurar foco, teclado, Escape, cancelación, ARIA y diseño responsive; ningún estado no exitoso modificará otra tarea o acción Workflow.

#### Scenario: Devolución confirmada

- **WHEN** `EjecutarDevolverActividad` responde éxito
- **THEN** cierra la confirmación, refleja el éxito correlacionado y actualiza solo la tarea que se devolvió.

### Requirement: Retiro y aislamiento del recorrido legacy

La acción moderna SHALL retirar el enlace `D-TASK-ANT`, el botón, handler y listener de postback de actividad anterior. SHALL preservar los triggers, contratos y recorridos de Usuario anterior, Continuar flujo, Enviar a usuario y Enviar a grupo.

#### Scenario: Inspección de no regresión

- **WHEN** se inspeccionan markup, code-behind y módulos de la acción modernizada
- **THEN** no existe una ruta alcanzable de actividad anterior hacia `Activa_devolver_actividades_anteriores` y las otras transiciones conservan sus identificadores propios.
