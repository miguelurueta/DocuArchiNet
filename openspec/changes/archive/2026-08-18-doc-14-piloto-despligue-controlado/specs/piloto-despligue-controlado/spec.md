<!-- opsxj:refinement-traceability version=1 artifact=spec decisions=D-01,D-02,D-03,D-04,D-05,D-06 -->
## ADDED Requirements

### Requirement: RQ-01 — El gate de piloto falla hacia legacy

El sistema SHALL evaluar `WorkflowCentroTrabajoModernActive` mediante `ConfiguracionWorkflowModernFeatureGate` con precedencia de exclusión. El modo piloto SHALL requerir inclusión explícita; el modo oficial SHALL requerir `WorkflowCentroTrabajoModernOfficialMode=true`, listas de piloto vacías y metadatos válidos, sin habilitación global implícita por la bandera base.

#### Scenario: Activación para un perfil incluido

- **WHEN** la bandera es verdadera, los metadatos del piloto son válidos y el contexto coincide con un usuario o grupo incluido no excluido
- **THEN** el gate devuelve `activo` y `WORKFLOW_MODERN_ACTIVE`.

#### Scenario: Configuración de piloto sin lista autorizada

- **WHEN** la bandera es verdadera, el modo oficial está desactivado y no existen usuarios ni grupos incluidos
- **THEN** el gate devuelve `fallback-legacy` sin habilitar la experiencia moderna para toda la población.

#### Scenario: Activación oficial explícita

- **WHEN** la bandera y `WorkflowCentroTrabajoModernOfficialMode` son verdaderos, las listas de usuario y grupo están vacías y los metadatos son válidos
- **THEN** el gate devuelve `activo` para todo contexto Workflow válido que no esté excluido.

#### Scenario: Modo oficial con alcance piloto simultáneo

- **WHEN** el modo oficial está activo y existe un usuario o grupo incluido
- **THEN** el gate devuelve `fallback-legacy` con código `WORKFLOW_MODERN_OFFICIAL_SCOPE_CONFLICT`.

#### Scenario: Exclusión explícita

- **WHEN** un perfil incluido también aparece en una lista de exclusión
- **THEN** el gate devuelve `excluido` y no expone la experiencia moderna.

### Requirement: RQ-02 — Página y ASMX respetan la misma decisión

Presentation SHALL consultar únicamente el bootstrap permitido del gate. `PreviewEnviarTarea` y `EjecutarEnvioTarea` SHALL conservar la revalidación en servidor antes de consultar o ejecutar una transición.

#### Scenario: Perfil fuera del piloto invoca un ASMX moderno

- **WHEN** un perfil inactivo, excluido o en fallback llama preview o ejecución directamente
- **THEN** el servicio devuelve un bloqueo funcional de gate y no invoca el motor legacy.

#### Scenario: Cambio de gate entre apertura y envío

- **WHEN** la página moderna se abrió con gate activo y la configuración se desactiva antes del envío
- **THEN** la ejecución ASMX se bloquea de forma segura y la siguiente apertura usa la interfaz legacy.

#### Scenario: Selección de conector completa la transición moderna

- **WHEN** el preview moderno devuelve un conector válido y el usuario lo selecciona
- **THEN** la página abre una confirmación con el contexto de la tarea y solo la confirmación invoca `EjecutarEnvioTarea` con `idTarea`, `idConector` y `tokenVersion`.

- **WHEN** `EjecutarEnvioTarea` responde éxito con el token esperado
- **THEN** la interfaz elimina la tarea completada, limpia el contexto de selección y muestra la confirmación funcional sin ejecutar el flujo legacy.

- **WHEN** la respuesta es bloqueo funcional o error técnico controlado
- **THEN** la interfaz conserva el contexto y permite al usuario cancelar o reintentar únicamente cuando el servicio lo habilita.

### Requirement: RQ-03 — La auditoría de piloto es mínima y sanitizada

Cada intento moderno relevante SHALL registrar correlación, identidad autorizada, tarea, ruta o flujo, conector, destino, canal, duración, resultado, código funcional y referencia de auditoría mediante `IAuditoriaTransicionRepository`.

#### Scenario: Resultado de ejecución moderno

- **WHEN** una ejecución termina en éxito, bloqueo o error
- **THEN** la bitácora registra un resultado estructurado sin SQL, credenciales, Session, token, documento ni payload sensible.

#### Scenario: Falla de auditoría

- **WHEN** el adaptador de auditoría no puede persistir la entrada
- **THEN** el resultado funcional no se reemplaza ni se reintenta automáticamente y se comunica una advertencia segura.

### Requirement: RQ-04 — El rollback no revierte datos ni transiciones confirmadas

El rollback SHALL desactivar `WorkflowCentroTrabajoModernActive` y `WorkflowCentroTrabajoModernOfficialMode`, vaciar el alcance piloto por configuración, registrar responsable, motivo, hora y correlación, y dejar legacy como ruta para nuevos intentos.

#### Scenario: Rollback durante operación no exitosa

- **WHEN** se ordena rollback mientras una operación moderna termina bloqueada o con error
- **THEN** no se cambia el estado legacy, las llamadas posteriores quedan bloqueadas por gate y el usuario vuelve a legacy al abrir de nuevo.

#### Scenario: Transición ya confirmada

- **WHEN** una transición ya fue confirmada por el servidor antes del rollback
- **THEN** no se ejecuta SQL, JavaScript ni una nueva llamada a `Cambia_Estado` para revertirla.

### Requirement: RQ-05 — La promoción exige métricas y aprobación explícita

El sistema SHALL producir un reporte de piloto por canal que compare volumen, éxito, bloqueos, errores, duración, abandonos y divergencias. La activación y promoción SHALL requerir un alcance de piloto o modo oficial explícito, responsable, motivo, umbrales y aprobación documentados.

#### Scenario: Evento crítico

- **WHEN** se evidencia transición duplicada, pérdida de datos/contexto, filtración sensible, incumplimiento de autorización o fallo de rollback
- **THEN** el reporte marca el piloto como bloqueado y la configuración permanece o vuelve a legacy.

### Requirement: RQ-06 — La evidencia es reproducible y no activa ambientes sin autorización

La entrega SHALL incluir pruebas focales, compilación o limitación comprobada, matriz manual, resoluciones requeridas y paquete documental en `Doc/Actualizacion/workflow/Terminar/06-piloto-pruebas-rollout/`.

#### Scenario: Prueba autenticada, carga o activación oficial

- **WHEN** se proponga ejecutar E2E o carga en un ambiente
- **THEN** se exige la autorización explícita, el runbook aplicable y la restauración final de `WorkflowCentroTrabajoModernActive` y `WorkflowCentroTrabajoModernOfficialMode` en `false`, con usuarios y grupos vacíos.

#### Scenario: Activación oficial explícita

- **WHEN** se aprueba activar la interfaz moderna para todos los contextos Workflow válidos
- **THEN** se registran responsable, motivo, fecha y umbrales; se activa `WorkflowCentroTrabajoModernOfficialMode` con las listas piloto vacías y queda disponible el rollback por configuración.

## Trazabilidad de decisiones

| Decisión | Requisito cubierto |
| --- | --- |
| D-01 | RQ-01 |
| D-02 | RQ-02 |
| D-03 | RQ-03 |
| D-04 | RQ-04 |
| D-05 | RQ-05 |
| D-06 | RQ-06 |
