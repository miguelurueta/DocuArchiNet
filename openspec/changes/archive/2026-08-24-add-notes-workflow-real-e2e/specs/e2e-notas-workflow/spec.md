## Purpose

Definir una validación E2E real, reproducible y protegida de Notas de Workflow, que use sesiones autorizadas y evidencia saneada sin confundir pruebas aisladas con comportamiento integrado.

## ADDED Requirements

### Requirement: Validación de Notas integrada y ejecución E2E por DOC-32

La automatización SHALL proporcionar validaciones contractuales de Notas dentro del arnés Workflow existente y SHALL reutilizar la sesión autenticada, configuración de navegador y controles de configuración comunes. La ejecución E2E de transición Workflow SHALL realizarse exclusivamente mediante los comandos DOC-32 existentes. La suite MUST NOT crear un login, almacenamiento de secretos, harness ni ejecutor de transición paralelo.

#### Scenario: Captura interactiva y efímera de configuración

- **WHEN** una persona ejecuta un comando `test:notes:*` o `test:doc32:*` desde una consola interactiva
- **THEN** el arnés solicita únicamente los valores requeridos para ese modo, oculta contraseña y URL MySQL, y entrega los valores solo al proceso hijo de validación y prueba, sin crear `.env`, usar `setx` ni imprimir valores.
- **AND WHEN** el comando se ejecuta sin TTY
- **THEN** falla antes de iniciar sesión, abrir navegador o enviar solicitudes.

#### Scenario: Ejecución real de transición por DOC-32

- **WHEN** se requiere evidencia E2E real del flujo Workflow
- **THEN** se ejecutan `test:doc32:preview`, `test:doc32:execute` y `test:doc32:concurrency` sobre las tareas descartables autorizadas.
- **AND THEN** las comprobaciones CRUD de Notas no se anuncian ni sustituyen la transición DOC-32.

#### Scenario: Borde sin sesión

- **WHEN** una solicitud de lectura de notas se ejecuta sin una sesión Gestión válida
- **THEN** el sistema devuelve un bloqueo funcional seguro, no expone notas ni detalles internos y no modifica estado ni auditoría.

#### Scenario: Sesión autenticada reutilizada

- **WHEN** un caso E2E de Notas requiere autenticación
- **THEN** utiliza el mecanismo de sesión Workflow compartido y no transmite usuario, grupo, permisos, autor ni tarea como sustitutos de contexto de servidor.

### Requirement: Lectura contractual real sin mutación

La automatización SHALL ejecutar lectura de notas contra un ambiente de pruebas autorizado con una cuenta válida, una tarea autorizada y controles MySQL exclusivamente de lectura. Antes y después de la lectura SHALL comparar huellas de estado y auditoría asociadas a la tarea.

#### Scenario: Listado autorizado estable

- **WHEN** una sesión autorizada lista o consulta una nota perteneciente a una tarea accesible
- **THEN** recibe únicamente datos autorizados con orden y paginación del contrato, y las huellas de estado y auditoría permanecen iguales.

#### Scenario: Aislamiento de tarea y cursor

- **WHEN** la prueba intenta usar una nota, cursor o identificador de tarea fuera del contexto autorizado
- **THEN** recibe un resultado funcional seguro sin contenido cruzado y sin cambios en estado ni auditoría.

### Requirement: Escrituras contractuales explícitamente autorizadas

La automatización SHALL mantener las escrituras de Notas deshabilitadas por defecto y SHALL exigir autorización explícita de ambiente, cuenta y tarea descartable antes de crear, actualizar o eliminar. Las consultas de control MUST ser una sola sentencia `SELECT` con exactamente un parámetro para la tarea y usar una cuenta MySQL de solo lectura.

#### Scenario: Falta autorización de escritura

- **WHEN** no se declara autorización explícita para una tarea descartable
- **THEN** el comando de escritura falla antes de abrir sesión autenticada o enviar una solicitud mutante.

#### Scenario: Escritura autorizada e idempotente

- **WHEN** una tarea descartable autorizada recibe dos solicitudes de creación con la misma intención idempotente
- **THEN** la prueba confirma una sola nota materializada, una sola auditoría de creación y un resultado estable para el reintento.

#### Scenario: Conflicto y eliminación conforme a política

- **WHEN** dos contextos de prueba intentan mutar la misma nota con una versión desactualizada o se elimina una nota en una tarea descartable
- **THEN** el conflicto no sobrescribe el cambio vigente y la eliminación produce únicamente la semántica y auditoría aprobadas.

### Requirement: Evidencia saneada y cierre de corrida

La automatización SHALL conservar únicamente evidencia resumida de códigos, conteos, latencias y huellas. MUST NOT persistir o mostrar credenciales, cookies, cadenas de conexión, contenido de notas, cuerpos de respuesta ni identificadores sensibles fuera de los controles autorizados.

#### Scenario: Cierre E2E autorizado

- **WHEN** finaliza una corrida DOC-32 real de preview, ejecución o concurrencia
- **THEN** la evidencia queda saneada, el gate de Centro de Trabajo permanece en `false` con listas vacías y se aplican los controles de integridad establecidos por el runbook E2E.
