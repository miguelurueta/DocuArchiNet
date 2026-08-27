<!-- opsxj:refinement-traceability version=1 artifact=spec decisions=D-01,D-02,D-03,D-04,D-05,D-06 -->
## ADDED Requirements

### Requirement: Presentación moderna independiente de gate

La página SHALL registrar la presentación de Devolver a usuario anterior para cada contexto Workflow válido, sin evaluar `WorkflowCentroTrabajoModernActive` ni modificar la política de otras operaciones. (D-01, RQ-01)

#### Scenario: Registro sin gate de transición

- **WHEN** `Webworkflow.aspx` prepara una solicitud con contexto Workflow válido
- **THEN** el trigger exclusivo recibe su bootstrap aunque el gate de transición de otras operaciones esté desactivado.

### Requirement: Sustitución exclusiva de la ruta heredada

El menú SHALL reemplazar solo Usuario anterior por un trigger sin postback y SHALL retirar los símbolos heredados asociados a esa operación. (D-02, RQ-02)

#### Scenario: Comando Usuario anterior

- **WHEN** una persona activa Usuario anterior en el menú Devolver
- **THEN** se abre únicamente el modal moderno propio y no se invocan `D-TWU-ANT`, `Button_tool_devolver_a_usuario`, controles ocultos ni handlers Web Forms.

#### Scenario: Operaciones vecinas

- **WHEN** se activa Devolver a actividad anterior u otra operación Workflow
- **THEN** conserva sus triggers, eventos, estado y contratos existentes.

### Requirement: Contrato de usuario histórico mínimo

La interfaz SHALL consumir solo `PreviewDevolverUsuarioAnterior` y `EjecutarDevolverUsuarioAnterior`, usando la identidad mínima definida por DOC-36. (D-03, RQ-03)

#### Scenario: Preview elegible o bloqueado

- **WHEN** el preview devuelve un usuario histórico elegible
- **THEN** el modal muestra exclusivamente `ActividadAnterior`, `UsuarioAnterior` y el contexto autorizado por servidor; conserva `TokenVersion` como valor opaco.

- **WHEN** el preview devuelve un bloqueo por historial, grupo, usuario retirado o auto-devolución
- **THEN** se muestra el mensaje funcional y no se presentan actividades alternativas.

### Requirement: Confirmación accesible y aislada

La interfaz SHALL reutilizar el diálogo de confirmación, foco, teclado, Escape y bloqueo de ejecución sin compartir listeners, estado ni requests con otras operaciones. (D-04, RQ-04)

#### Scenario: Ejecución en curso y recuperación

- **WHEN** la ejecución está en curso
- **THEN** confirmar, cancelar, cierre, backdrop, Escape y doble clic no pueden iniciar ni abandonar una segunda ejecución.

- **WHEN** ocurre timeout o error técnico
- **THEN** la bandeja permanece intacta, se informa un mensaje seguro y la persona puede solicitar un preview nuevo.

### Requirement: Actualización localizada de bandeja

La interfaz SHALL aplicar el resultado exitoso solo a la tarea confirmada y SHALL preservar la bandeja ante bloqueo o error. (D-05, RQ-05)

#### Scenario: Resultado de ejecución

- **WHEN** `EjecutarDevolverUsuarioAnterior` confirma éxito
- **THEN** se actualizan fila, visor, contador, listado y scroll mediante la presentación existente.

- **WHEN** el servidor bloquea o rechaza la operación
- **THEN** la tarea no se retira ni se altera el estado de las demás operaciones.

### Requirement: Arquitectura E2E reutilizable y segura

El repositorio SHALL registrar una corrida `doc37` que reutilice el patrón de sesión efímera, ODBC de solo lectura y ciclo de recursos de DOC-36, con perfil no sensible y dos tareas de interfaz distintas. (D-06, RQ-06)

#### Scenario: Perfil y etapas aislados

- **WHEN** se prepara un perfil DOC-37 desde los recursos descartables de DOC-36
- **THEN** el perfil conserva únicamente configuración operativa no sensible, separa tarea de ejecución y bloqueo UI, y registra las etapas `preview`, `execution` y `ui-lock` con autorización explícita.

#### Scenario: Una tarea seleccionada por prueba

- **WHEN** se invoca el runner DOC-37
- **THEN** acepta exactamente una etapa; cada etapa abre una sesión propia, selecciona solo la tarea autorizada mediante el comando oficial de la bandeja, obtiene su preview vigente y opera solo esa tarea.

#### Scenario: Precondición de selección segura

- **WHEN** una sesión E2E nueva no tiene tarea seleccionada
- **THEN** la prueba activa únicamente el comando oficial de selección de la tarea autorizada y espera su confirmación antes de capturar las huellas de control.
- **THEN** no escribe campos ocultos, sesión ni invoca endpoints internos para simular la selección; si la tarea no está disponible, falla antes de invocar preview o ejecución.

#### Scenario: Validación de interfaz autorizada

- **WHEN** una persona autorizada ejecuta una etapa DOC-37 contra el ambiente y las cuentas de prueba aprobados
- **THEN** el preview controla estado y auditoría mediante consultas `SELECT`, la ejecución usa solo los endpoints de DOC-36 y el bloqueo UI conserva una única solicitud mutante sin registrar valores sensibles.
