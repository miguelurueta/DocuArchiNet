## Purpose

Proporciona un contrato uniforme, seguro y verificable para administrar recursos descartables utilizados por cualquier escenario E2E autorizado.

## ADDED Requirements

### Requirement: Contrato registrado de recursos E2E

La plataforma SHALL aceptar recursos E2E únicamente mediante un contrato registrado por tipo de escenario y ambiente. El contrato SHALL declarar identificador no sensible, clase de recurso, prerrequisitos verificables, política de restauración y evidencia permitida; SHALL rechazar perfiles que intenten introducir secretos, SQL, comandos, rutas ejecutables o políticas no registradas.

#### Scenario: Recurso declarado por un escenario registrado

- **WHEN** un escenario E2E recibe un perfil con un recurso y contrato registrados para su ambiente
- **THEN** la plataforma usa únicamente los campos no sensibles declarados por ese contrato.

#### Scenario: Perfil intenta alterar el proveedor de recursos

- **WHEN** un perfil contiene un campo o valor que no pertenece al contrato registrado del recurso
- **THEN** la plataforma lo rechaza antes de iniciar una operación E2E y no revela el valor rechazado.

### Requirement: Preflight no mutante y explicable

La plataforma SHALL verificar los prerrequisitos registrados de un recurso antes de cualquier endpoint mutante. El preflight SHALL usar solo controles permitidos de lectura y SHALL producir un resultado saneado que identifique disponibilidad, falta de prerrequisito, ambigüedad o reserva vigente, sin exponer credenciales, cadenas de conexión, SQL ni datos de negocio innecesarios.

#### Scenario: Recurso disponible

- **WHEN** el recurso existe, es único y satisface los prerrequisitos declarados
- **THEN** el preflight lo marca disponible para reserva sin modificar su estado ni auditoría.

#### Scenario: Prerrequisito de negocio ausente

- **WHEN** el recurso no cumple una regla declarada, como una ruta o destinatario habilitado
- **THEN** el preflight falla antes de la operación mutante y registra únicamente un código saneado de prerrequisito.

### Requirement: Reserva exclusiva y ciclo de liberación

La plataforma SHALL reservar de forma exclusiva un recurso disponible antes de una etapa E2E mutante y SHALL impedir que otra corrida autorizada use ese mismo recurso mientras la reserva esté vigente. Al terminar, fallar o interrumpirse la corrida, SHALL registrar el resultado saneado y liberar el recurso o ejecutar la restauración registrada, dejando trazabilidad de cierre sin secretos.

#### Scenario: Dos corridas solicitan el mismo recurso

- **WHEN** una segunda corrida solicita un recurso que ya posee una reserva vigente
- **THEN** la segunda corrida falla antes de la etapa mutante con un código de recurso reservado.

#### Scenario: Corrida interrumpida después de reservar

- **WHEN** una corrida finaliza con error o interrupción después de obtener un recurso
- **THEN** el ciclo de cierre libera o restaura el recurso según su contrato y conserva evidencia saneada del cierre.

### Requirement: Recursos independientes por escenario

La plataforma SHALL permitir que distintos tipos de pruebas registren sus propios recursos y prerrequisitos sin modificar el núcleo común ni reutilizar los supuestos funcionales de otro escenario.

#### Scenario: Se registra una prueba no Workflow

- **WHEN** un nuevo tipo de prueba declara su contrato de recursos y adaptador registrado
- **THEN** obtiene el mismo preflight, reserva, liberación y evidencia sin heredar tareas, actividades ni consultas propias de Workflow.
