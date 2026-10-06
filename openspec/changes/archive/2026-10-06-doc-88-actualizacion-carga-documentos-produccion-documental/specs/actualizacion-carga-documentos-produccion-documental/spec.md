<!-- opsxj:refinement-traceability version=1 artifact=spec decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07 -->
## ADDED Requirements

### Requirement: Caracterización segura de la carga

El sistema SHALL caracterizar las fronteras de preparación, handler, prealmacenamiento y almacenamiento antes de seleccionar la corrección. Trazabilidad: D-01, RQ-01.

#### Scenario: Contexto incompleto

- **WHEN** una frontera recibe una selección ausente, caducada o incompatible
- **THEN** identifica la etapa y devuelve un rechazo funcional sin `NullReferenceException` ni persistencia

#### Scenario: Recorrido válido

- **WHEN** el usuario autorizado proporciona expediente, configuración, tipología y archivo válidos
- **THEN** el recorrido alcanza el almacenamiento y entrega un resultado determinista

### Requirement: Validación de contexto y materialización

El sistema SHALL validar en servidor el contexto autorizado y toda salida antes de acceder a referencias o índices. Trazabilidad: D-02, RQ-02.

#### Scenario: Resolver textual con salida vacía

- **WHEN** un resolver retorna `YES` pero su objeto, arreglo o lista es nulo, vacío o no corresponde a la solicitud
- **THEN** el consumidor rechaza la operación antes de acceder a `(0)`, `.Item(0)`, `.Length`, `.Count` o propiedades

#### Scenario: Identificador enviado por el cliente

- **WHEN** el navegador envía un identificador de expediente
- **THEN** el servidor vuelve a resolver autorización y pertenencia desde la sesión autenticada antes de guardar

### Requirement: Compatibilidad del cargador compartido

El sistema SHALL delimitar a `PRODUCCION` cualquier comportamiento nuevo y preservar los contratos de los demás eventos. Trazabilidad: D-03, RQ-03.

#### Scenario: Evento de producción

- **WHEN** `evento_adjunta` es `PRODUCCION`
- **THEN** se aplican las validaciones y respuestas específicas de DOC-88

#### Scenario: Otro evento soportado

- **WHEN** el handler recibe `ADJUNTARADICACION`, Workflow, SII, ENLASE, versiones, PQRS u otro evento existente
- **THEN** conserva firmas, campos, semántica y recorrido previo

### Requirement: Separación entre persistencia y proyección

El sistema SHALL confirmar la persistencia independientemente de la construcción de la fila visual. Trazabilidad: D-04, RQ-04.

#### Scenario: Persistencia y proyección exitosas

- **WHEN** el almacenamiento confirma el documento y la respuesta contiene los datos de producción
- **THEN** el cliente inserta exactamente una fila correcta

#### Scenario: Proyección fallida después de guardar

- **WHEN** el documento fue persistido pero falla la actualización visual
- **THEN** el sistema informa la falla de proyección y no repite el almacenamiento automáticamente

### Requirement: Errores públicos saneados

El sistema SHALL devolver códigos funcionales por etapa y SHALL NOT exponer excepciones internas o información sensible. Trazabilidad: D-05, RQ-05.

#### Scenario: Excepción controlada del servidor

- **WHEN** falla contexto, configuración, tipología, almacenamiento, confirmación o proyección
- **THEN** la respuesta contiene un código `PRODUCCION_CARGA_*` y un mensaje saneado sin `ex.Message`, rutas ni secretos

#### Scenario: Excepción JavaScript

- **WHEN** el cliente captura una excepción en el recorrido de producción
- **THEN** usa `ex.message` y conserva un diagnóstico funcional legible

### Requirement: Resultado terminal y no repetición automática

El sistema SHALL tratar el resultado de la llamada existente de almacenamiento como autoridad y SHALL NOT consultar existencia ni repetir automáticamente la escritura. Trazabilidad: D-06, RQ-06.

#### Scenario: Doble clic o concurrencia

- **WHEN** el usuario activa dos veces la carga mientras la primera solicitud continúa en curso
- **THEN** el cliente de `PRODUCCION` mantiene una sola solicitud activa sin modificar el comportamiento de otros eventos

#### Scenario: Almacenamiento rechazado

- **WHEN** `Almacenamiento` retorna un valor diferente de `YES`
- **THEN** el flujo termina con un error funcional saneado, sin consulta de existencia y sin segundo intento de escritura

#### Scenario: Respuesta perdida

- **WHEN** el cliente no recibe confirmación después de una posible persistencia
- **THEN** informa un resultado incierto y no vuelve a llamar automáticamente al almacenamiento

### Requirement: Evidencia y no regresión

El cambio SHALL demostrar pruebas focales, compatibilidad compartida, compilación y cierre E2E seguro. Trazabilidad: D-07, RQ-07.

#### Scenario: Validación local

- **WHEN** finaliza la implementación
- **THEN** aprueban caracterización, contratos, múltiples archivos, reintentos, suites compartidas, compilación, `git diff --check` y OpenSpec estricto

#### Scenario: E2E sin autorización

- **WHEN** no existe autorización explícita de ambiente, cuentas y datos descartables
- **THEN** no se ejecuta carga real y se registra un bloqueo explícito sin mocks, simulaciones ni evidencia ficticia

#### Scenario: E2E autorizada

- **WHEN** ambiente, cuenta, expediente y archivos descartables están autorizados
- **THEN** el preview prepara localmente el archivo y la tipología sin activar `Guardar`, y una ejecución separada con autorización mutante activa `Guardar` una sola vez, confirma persistencia por `SELECT` y valida una única fila visual

#### Scenario: Rechazo durante la E2E mutante

- **WHEN** el almacenamiento real retorna un resultado diferente de `YES`
- **THEN** la E2E termina con rechazo saneado, no reintenta la carga y conserva evidencia de si el control de persistencia cambió
