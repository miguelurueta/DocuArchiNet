# Corrección de adjuntos de Radicación Simplificada

## Purpose

Garantizar que Radicación Simplificada adjunte documentos con una identidad de radicado autorizada y consistente, preserve los consumidores legacy y actualice la interfaz sin recargas.

## Requirements

### Requirement: RQ-01 — Radicado autoritativo del registro de estado

El sistema SHALL resolver el radicado de `ADJUNTARADICACION` desde `ra_rad_estados_modulo_radicacion.consecutivo_radicado` usando el identificador del registro validado en servidor.

#### Scenario: DAT_ADIC_TAR no entrega radicado

- **GIVEN** un registro autorizado con `consecutivo_radicado` no vacío y una tarea cuya tabla `DAT_ADIC_TAR` no contiene un radicado utilizable
- **WHEN** el usuario adjunta un documento desde Radicación Simplificada
- **THEN** plantilla, índices, almacenamiento y respuesta usan el radicado del registro de estado

#### Scenario: Identidad inconsistente

- **WHEN** el registro no existe, no pertenece al contexto, tiene radicado vacío o el valor informativo no vacío difiere del persistido
- **THEN** la operación falla antes del almacenamiento y no crea una fila documental

### Requirement: RQ-02 — Resolución única de contexto

El sistema SHALL materializar una vez por carga lógica un contexto inmutable mediante una consulta parametrizada y autorizada.

#### Scenario: Contexto válido

- **WHEN** el servicio de adjuntos recibe un identificador autorizado
- **THEN** obtiene radicado, tarea, trámite, plantilla y destino desde el registro confiable y transporta el mismo contexto durante el almacenamiento

#### Scenario: Aislamiento entre cargas

- **WHEN** se procesan varios archivos o pestañas
- **THEN** cada solicitud construye su propio contexto sin usar estado web mutable como autoridad

### Requirement: RQ-03 — Índices construidos con radicado explícito

El sistema SHALL construir plantilla e índices con un radicado obligatorio ya validado y SHALL NOT volver a resolverlo desde los datos adicionales de la tarea.

#### Scenario: Preparación específica

- **WHEN** se prepara un documento de Radicación Simplificada
- **THEN** los índices se construyen una vez con el radicado autorizado y se reutiliza el almacenamiento documental existente

#### Scenario: Radicado vacío

- **WHEN** la preparación recibe un radicado vacío
- **THEN** rechaza la operación antes de consultar la plantilla

### Requirement: RQ-04 — Compatibilidad de sobrecargas y eventos legacy

El sistema SHALL usar la ruta autoritativa exclusivamente para `ADJUNTARADICACION` y SHALL conservar el comportamiento histórico de los demás eventos de carga.

#### Scenario: Consumidor legacy

- **WHEN** Gestión de respuestas, Workflow seleccionado, Producción u otro consumidor histórico adjunta un documento
- **THEN** conserva su recorrido, retorno y resolución histórica

#### Scenario: Radicación Simplificada

- **WHEN** el handler recibe el contexto específico de `ADJUNTARADICACION`
- **THEN** valida la identidad autoritativa antes de almacenar

### Requirement: RQ-05 — Contrato de interfaz sin recarga

El sistema SHALL preservar el contrato HTTP de carga y SHALL proyectar el documento en la interfaz sin recarga parcial o completa.

#### Scenario: Carga válida

- **WHEN** el backend almacena correctamente el documento
- **THEN** la respuesta mantiene tipología, formato, icono y acciones, y la interfaz inserta la fila mediante JavaScript sin postback ni recarga

#### Scenario: Fronteras protegidas

- **WHEN** se ejecuta `ADJUNTARADICACION`
- **THEN** la inserción nueva queda limitada a ese evento y los demás consumidores conservan su comportamiento de actualización

### Requirement: RQ-06 — Evidencia de no regresión y E2E controlada

El sistema SHALL contar con pruebas locales deterministas, compilación completa y una E2E real positiva y negativa integrada en el arnés oficial.

#### Scenario: Validación local

- **WHEN** se valida el cambio antes de desplegar
- **THEN** aprueban las pruebas del defecto, autorización, resolución única, contratos compartidos, consumidores legacy, respuesta y compilación

#### Scenario: E2E autorizada

- **GIVEN** autorización explícita de ambiente, cuenta, registro descartable, fixture, tipología y acceso de evidencia de solo lectura
- **WHEN** se ejecutan los escenarios positivo y de discrepancia informativa
- **THEN** se verifica persistencia única o rechazo sin persistencia, inserción JavaScript sin recarga y ausencia de un gate activado

#### Scenario: E2E sin autorización

- **WHEN** falta cualquiera de los recursos o autorizaciones requeridos
- **THEN** no se ejecuta la E2E, se registra el bloqueo operacional y el cambio no se declara cerrado

### Requirement: RQ-07 — Infraestructura existente como única composición técnica

`ADJUNTARADICACION` SHALL reutilizar la infraestructura compartida de conexión y ejecución de datos, sin introducir fábricas privadas ni reconstruir credenciales en el repositorio.

#### Scenario: Composición productiva

- **WHEN** se compone el servicio de adjuntos
- **THEN** la conexión se obtiene desde el contexto autenticado y las dependencias compartidas se inyectan en el repositorio
- **AND** el repositorio no conoce controles web, credenciales ni una fábrica de conexión privada
