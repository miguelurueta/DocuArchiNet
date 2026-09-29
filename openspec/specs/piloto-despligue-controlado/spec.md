# Disponibilidad oficial y operación controlada Specification

## Purpose

Definir el comportamiento verificable de la experiencia moderna oficial del centro de trabajo, sin configuración de rollout ni audiencias piloto, y conservar una reversa por control de versiones sin alterar transiciones confirmadas.

## Requirements

### Requirement: RQ-01 — La disponibilidad oficial depende del contexto

El sistema SHALL ofrecer la experiencia moderna a toda sesión autenticada con contexto Workflow válido. La decisión SHALL NOT leer configuración de rollout, modo oficial, inclusiones, exclusiones ni audiencias piloto.

#### Scenario: Contexto Workflow válido

- **WHEN** una sesión autenticada aporta identidad, grupo y ruta Workflow válidos
- **THEN** la política devuelve `activo` y `WORKFLOW_MODERN_OFFICIAL`.

#### Scenario: Configuración de rollout residual

- **WHEN** existen claves residuales de rollout o listas de audiencia en un ambiente
- **THEN** la política las ignora y decide únicamente con el contexto autenticado.

#### Scenario: Contexto inválido

- **WHEN** falta autenticación o el contexto Workflow es inválido
- **THEN** la política devuelve `WORKFLOW_CONTEXT_INVALID` sin habilitar operaciones.

#### Scenario: Usuario o grupo sin audiencia configurada

- **WHEN** una sesión Workflow válida no aparece en ninguna lista de audiencia
- **THEN** la experiencia oficial permanece disponible.

#### Scenario: Configuración de exclusión residual

- **WHEN** un usuario o grupo aparece en una lista residual de exclusión
- **THEN** esa lista no afecta la disponibilidad oficial.

### Requirement: RQ-02 — Página y ASMX respetan el mismo contexto autenticado

Presentation SHALL consultar únicamente el bootstrap permitido del gate. `PreviewEnviarTarea` y `EjecutarEnvioTarea` SHALL conservar la revalidación en servidor antes de consultar o ejecutar una transición.

#### Scenario: Sesión inválida invoca un ASMX moderno

- **WHEN** una sesión sin contexto válido llama preview o ejecución directamente
- **THEN** el servicio devuelve un rechazo de autorización y no invoca el motor legacy.

#### Scenario: Cambio de sesión entre apertura y envío

- **WHEN** la página moderna se abrió con contexto válido y la sesión deja de ser válida antes del envío
- **THEN** la ejecución ASMX se bloquea de forma segura y no ofrece una ruta alternativa sin autorización.

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

### Requirement: RQ-04 — La reversa por versión no revierte datos ni transiciones confirmadas

La reversa SHALL realizarse por control de versiones y despliegue, registrar responsable, motivo, hora y correlación, y no SHALL reintroducir configuración de rollout como mecanismo operativo.

#### Scenario: Rollback durante operación no exitosa

- **WHEN** se ordena una reversa mientras una operación moderna termina bloqueada o con error
- **THEN** no se cambia el estado de la tarea ni se reintenta automáticamente.

#### Scenario: Transición ya confirmada

- **WHEN** una transición ya fue confirmada por el servidor antes del rollback
- **THEN** no se ejecuta SQL, JavaScript ni una nueva llamada a `Cambia_Estado` para revertirla.

### Requirement: RQ-05 — La promoción exige métricas y aprobación explícita

El sistema SHALL producir un reporte operativo por canal que compare volumen, éxito, bloqueos, errores, duración, abandonos y divergencias. Un cambio de versión SHALL requerir responsable, motivo, umbrales y aprobación documentados.

#### Scenario: Evento crítico

- **WHEN** se evidencia transición duplicada, pérdida de datos/contexto, filtración sensible, incumplimiento de autorización o fallo de rollback
- **THEN** el reporte marca la versión como bloqueada y se aplica el procedimiento de reversa aprobado.

### Requirement: RQ-06 — La evidencia es reproducible y no activa ambientes sin autorización

La entrega SHALL incluir pruebas focales, compilación o limitación comprobada, matriz manual, resoluciones requeridas y paquete documental en `Doc/Actualizacion/workflow/Terminar/06-piloto-pruebas-rollout/`.

#### Scenario: Prueba autenticada o carga

- **WHEN** se proponga ejecutar E2E o carga en un ambiente
- **THEN** se exige la autorización explícita, el runbook aplicable y la confirmación de que no se creó ni modificó configuración de rollout.

#### Scenario: Despliegue de la versión oficial

- **WHEN** se aprueba desplegar la interfaz moderna oficial
- **THEN** se registran responsable, motivo, fecha y umbrales; queda disponible la reversa por control de versiones.
