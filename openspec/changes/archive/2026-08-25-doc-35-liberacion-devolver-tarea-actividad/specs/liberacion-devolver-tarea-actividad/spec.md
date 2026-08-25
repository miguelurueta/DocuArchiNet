<!-- opsxj:refinement-traceability version=1 artifact=spec decisions=D-01,D-02,D-03,D-04,D-05 -->
## Purpose

Define la decisión, controles y límites para una liberación futura y autorizada de Devolver a actividad anterior, sin convertir la evidencia técnica en un despliegue implícito.

## ADDED Requirements

### Requirement: Decisión de liberación única (D-01)

El paquete de liberación MUST basarse en la evidencia técnica aprobada de DOC-34 y declarar exactamente una decisión operativa para la versión de referencia.

#### Scenario: Sin autorización de ambiente

- **WHEN** no existen ambiente, ventana, aprobador y responsables autorizados para la versión de referencia
- **THEN** la decisión es solicitar aprobación operativa y no se infiere un despliegue

### Requirement: Matriz aislada por ambiente (D-02)

El paquete MUST contener una matriz que identifique, sin secretos, autorización, versión, alcance, ventana, responsables por rol, evidencia y continuación para cada ambiente.

#### Scenario: Ambiente no incluido

- **WHEN** un ambiente no tiene una solicitud que complete todos los campos requeridos
- **THEN** queda fuera de operación y la autorización de otro ambiente, versión o ventana no se reutiliza

### Requirement: Runbook de controles autorizados (D-03)

El runbook MUST permitir únicamente comprobaciones documentales y consultas SELECT parametrizadas y saneadas después de una autorización explícita para el ambiente.

#### Scenario: Autorización incompleta

- **WHEN** falta autorización, versión, alcance, ventana o responsables
- **THEN** el operador aborta antes de desplegar y no ejecuta E2E, carga, cambios de configuración ni cambios de gate

### Requirement: Reversión e invariantes de la capacidad (D-04)

La liberación MUST conservar la ruta moderna oficial de devolución, los contratos de operaciones vecinas, conectores entrantes Ruta/Flujo, lock por tarea y aislamiento de respuestas.

#### Scenario: Reversión aprobada

- **WHEN** la gestión de despliegue ordena regresar al paquete previamente acordado
- **THEN** solo se afectan intentos nuevos y no se alteran tareas, auditoría o transiciones confirmadas ni se reactiva una ruta Web Forms alternativa

### Requirement: Registro sano de resultado (D-05)

El resultado de una operación autorizada MUST registrar decisión, ambiente, versión y referencias saneadas, sin secretos, cookies, cadenas de conexión, datos de tarea ni cuerpos de respuesta.

#### Scenario: Control no conforme

- **WHEN** aparece una diferencia de versión o contrato, un control no conforme o retiro de aprobación
- **THEN** el resultado se registra como abortado con referencias saneadas y no inicia el despliegue
