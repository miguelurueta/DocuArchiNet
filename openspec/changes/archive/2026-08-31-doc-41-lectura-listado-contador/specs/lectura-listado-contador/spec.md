<!-- opsxj:refinement-traceability version=1 artifact=spec decisions=D-01,D-02,D-03,D-04,D-05,D-06 -->
## ADDED Requirements

### Requirement: Lectura moderna autorizada de notas (RQ-01, D-01, D-02)

El sistema SHALL exponer listado, consulta de contenido y contador de Notas Workflow solo a través del límite moderno, recibiendo `idTarea` explícito y resolviendo permiso, usuario, grupo y ruta exclusivamente en el servidor.

#### Scenario: Actor autorizado consulta una tarea activa

- **WHEN** un actor autenticado con permiso de notas solicita una lectura con una tarea activa asignada a su ruta
- **THEN** el servicio procesa la solicitud con el contexto de sesión validado y no consulta `ID_TAREA_SELECCIONDA`

#### Scenario: Contexto sin permiso o tarea no disponible

- **WHEN** el contexto no puede interactuar con notas o la tarea no está activa para la ruta autenticada
- **THEN** el sistema devuelve una respuesta funcional segura antes de consultar notas

### Requirement: Listado paginado y determinista (RQ-02, D-03, D-04)

El sistema SHALL listar notas operativas en orden `FECHA_ANOTACION DESC, ID_ANOTACION DESC`, con 25 elementos por defecto y un máximo de 50.

#### Scenario: Primera página autorizada

- **WHEN** una solicitud válida no incluye cursor
- **THEN** el sistema devuelve como máximo el tamaño solicitado, ordenado de forma estable y con cursor siguiente solo si existen más registros

#### Scenario: Cursor ligado al contexto

- **WHEN** un cursor se reutiliza con otra tarea, actor, grupo, ruta, versión o posición de orden
- **THEN** el sistema no devuelve filas, conteos ni cursor de la consulta ajena

### Requirement: Contenido aislado por tarea (RQ-03, D-02, D-04, D-05)

El sistema SHALL exigir `idTarea` e `idNota` para consultar contenido y verificará su pertenencia en la misma consulta parametrizada.

#### Scenario: Nota perteneciente a la tarea autorizada

- **WHEN** la nota pertenece a la tarea activa del actor autenticado
- **THEN** el sistema devuelve únicamente el contenido y metadatos definidos por el DTO moderno

#### Scenario: Nota de otra tarea o inexistente

- **WHEN** una nota no pertenece a la tarea indicada
- **THEN** el sistema devuelve una respuesta funcional sin revelar si la nota existe en otro contexto

### Requirement: Contador operativo consistente (RQ-04, D-04, D-05)

El sistema SHALL calcular el contador mediante `COUNT(*)` parametrizado y con la misma visibilidad operativa del listado.

#### Scenario: Conteo de tarea autorizada

- **WHEN** un actor autorizado solicita el contador de una tarea activa
- **THEN** el resultado coincide con la población del listado operativo y no materializa filas para contarlas

#### Scenario: Histórico sin política aprobada

- **WHEN** se intenta obtener información que no cumple la visibilidad operativa
- **THEN** el sistema no habilita ni expone un modo histórico moderno

### Requirement: Camino estrictamente de solo lectura (RQ-05, D-01, D-06)

El sistema SHALL mantener las operaciones modernas de DOC-41 como lecturas sin alterar tarea, estado, auditoría, endpoints legacy ni gates de activación.

#### Scenario: Invocación de cualquier endpoint DOC-41

- **WHEN** se invoca listado, contenido o contador
- **THEN** la operación no ejecuta una mutación ni modifica `WorkflowCentroTrabajoModernActive`

### Requirement: Evidencia local y E2E gobernada (RQ-06, D-06)

El sistema SHALL cubrir autorización, orden, cursor, aislamiento y contador con pruebas focales, y reutilizará `tools/e2e` para la cobertura autenticada cuando exista autorización expresa.

#### Scenario: Ambiente E2E no autorizado

- **WHEN** faltan autorización de ambiente, cuentas o tarea descartable
- **THEN** la validación deja un bloqueo explícito sin conectarse a una base real ni fabricar evidencia

#### Scenario: Control E2E de solo lectura reutilizable

- **WHEN** una corrida autenticada de Notas compara huellas de estado y auditoría
- **THEN** precarga raíz, módulo y ambiente no sensibles, reutiliza el DSN ODBC no sensible `workflowconta`, captura usuario y contraseña MySQL de solo lectura únicamente en TTY, registra los `SELECT` de estado/auditoría sin contenido y no solicita ni acepta una URL o cadena de conexión MySQL
