# lista-moderna-destino Specification

## Purpose

Proporciona una lista moderna, accesible y reversible de destinos de workflow,
sin sustituir el recorrido Web Forms legacy ni ejecutar una transición desde la
previsualización.

## Requirements

### Requirement: RQ-01 Convivencia de la lista moderna y legacy (D-01)

El sistema SHALL mantener `GridView_envia_flujo`, su modal y el comportamiento Web Forms existente cuando la experiencia moderna no esté activada.

#### Scenario: Gate ausente o desactivado

- **WHEN** el atributo bootstrap moderno falta o su valor no es `true`
- **THEN** la página no carga ni inicializa la lista moderna
- **AND THEN** el enlace Continuar conserva el flujo legacy actual sin llamadas a `PreviewEnviarTarea`

### Requirement: RQ-02 Bootstrap con el gate de servidor (D-02)

El sistema SHALL habilitar la lista solo con un atributo emitido en servidor después de evaluar `IWorkflowModernFeatureGate` para el contexto autenticado.

#### Scenario: Usuario fuera del piloto del ASMX

- **WHEN** `IWorkflowModernFeatureGate` no devuelve un estado activo
- **THEN** el atributo no habilita la UI moderna
- **AND THEN** el navegador no consulta el ASMX ni intenta modificar la bandera

### Requirement: RQ-03 Representación fiel del DTO (D-03)

El sistema SHALL representar solo los campos realmente publicados por `PrevisualizacionTransicionDto` y `DestinoTransicionDto`, usando radicado, tipo y grupo actual como contexto de la primera versión.

#### Scenario: Campo no entregado por el endpoint

- **WHEN** el contrato no contiene trámite ni actividad actual legible
- **THEN** la UI omite esos campos y no los deriva desde identificadores, campos ocultos, HTML legacy, sesión ni reglas de negocio
- **AND THEN** presenta `Contexto.Radicado`, `TipoDecision` y `Contexto.GrupoActual` como contexto disponible

### Requirement: RQ-04 Estados accesibles y selección sin envío (D-04)

El sistema SHALL ofrecer estados visuales recuperables y seleccionar un destino sin ejecutar una transición.

#### Scenario: Selección de destino

- **WHEN** una persona selecciona un destino disponible desde tabla o tarjeta
- **THEN** la UI publica un callback con `idTarea`, identificador de conector, `tokenVersion` y resumen visible
- **AND THEN** no llama a `EjecutarEnvioTarea`, `Terminar_Tarea_Workflow`, `Cambia_Estado` ni a un botón Web Forms invisible

### Requirement: RQ-05 Validación y rollback verificables (D-05)

La entrega SHALL documentar y verificar la compilación, pruebas focales, QA visual y la reversa por gate.

#### Scenario: Desactivación de la experiencia moderna

- **WHEN** se desactiva el gate para el piloto
- **THEN** la siguiente carga usa la interfaz legacy sin migración ni cambio de estado
- **AND THEN** la evidencia no expone credenciales, cookies, SQL ni cadenas de conexión
