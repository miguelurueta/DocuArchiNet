# pruebas-integracion Specification

## Purpose

TBD: Consolidar el propósito funcional de las pruebas de integración backend y su evidencia controlada.

## Requirements

### Requirement: RQ-01 Pirámide reutilizable
El sistema SHALL consolidar cobertura usando suites y fixtures B01–B06 en rutas canónicas, según D-01.

#### Scenario: Activos únicos
- **WHEN** se inspecciona la suite
- **THEN** reutiliza fixtures y no crea arnés, configuración o proyecto paralelo

#### Scenario: Documentación derivada del código
- **WHEN** se inspecciona el paquete técnico
- **THEN** cada flujo identifica funciones, componentes, parámetros y tablas existentes, y los diagramas están segmentados para reproducir el orden y las decisiones implementadas sin sustituirlos por un resumen conceptual

### Requirement: RQ-02 Integración determinista
El sistema SHALL validar contratos, seguridad, concurrencia, idempotencia, estados, fallos y reconciliación sin red ni secretos, según D-02.

#### Scenario: Matriz local
- **WHEN** corre la validación local
- **THEN** éxito, rechazo, timeout, cancelación, carreras, reintentos y fallos producen resultados reproducibles

### Requirement: RQ-03 Invariancia legacy
El sistema SHALL proteger fronteras legacy mediante caracterización y verificación del diff, según D-03.

#### Scenario: Persistencia única
- **WHEN** se inspecciona el código
- **THEN** sólo el adaptador contiene una invocación nueva a `AlmacenaDocumentoTareaWorkflow(...)`

#### Scenario: Superficies protegidas
- **WHEN** se valida DOC-56
- **THEN** `ClassAlmacenamiento.vb`, rutas legacy y `JSProgresBar.js` permanecen sin cambios

### Requirement: RQ-04 Contratos y gate
El sistema SHALL validar ocho operaciones y bloquear efectos modernos con el gate apagado, según D-04.

#### Scenario: Gate apagado
- **WHEN** `WorkflowCentroTrabajoModernActive` es `false`
- **THEN** cada endpoint moderno devuelve `FEATURE_DISABLED` sin invocar dependencias

#### Scenario: Contrato compartido
- **WHEN** se validan solicitudes, respuestas y estados
- **THEN** coinciden con los fixtures contractuales compartidos

### Requirement: RQ-05 Validador canónico
El sistema SHALL ofrecer un único script local no autenticado en `tools/validation/`, según D-05.

#### Scenario: Resultado propagado
- **WHEN** corre `Verify-ImportarServicioWebModern.ps1`
- **THEN** ejecuta la suite, resume y retorna código distinto de cero ante fallos

### Requirement: RQ-06 E2E controlado
El sistema SHALL preparar E2E sólo en `tools/e2e` y separar evidencia local de autorizada, según D-06.

#### Scenario: Sin autorización
- **WHEN** no hay autorización explícita
- **THEN** no ejecuta E2E, no activa gate y declara la prueba pendiente

#### Scenario: Restauración
- **WHEN** una corrida futura sea autorizada
- **THEN** usa controles `SELECT`, no expone secretos y restaura gate `false`, usuarios y grupos vacíos

### Requirement: RQ-07 Operaciones productivas completas
El sistema SHALL publicar las ocho operaciones con composición productiva, autorización existente y metadatos confiables, según D-07.

#### Scenario: Autorización SII
- **WHEN** una operación prepara o almacena documentos
- **THEN** exige `ADJUNTAR_IMAGENES_PREDETERMINADA` y falla cerrada si el permiso o el contexto no son vigentes

#### Scenario: Metadatos de almacenamiento
- **WHEN** se ejecuta una intención
- **THEN** el servidor deriva gabinete, ruta y clase documental de la tarea y acepta únicamente el radicado desde la solicitud

#### Scenario: Traducción segura de tipología
- **WHEN** el frontend envía `DocumentTypeId` y `DocumentTypeName`
- **THEN** interpreta el ID como `tipo_doc_series.Id_Tipo_Doc_Series`, exige una única relación de lista de chequeo para el trámite vigente, valida el nombre canónico y entrega al legacy exclusivamente el `ID_TIPO_DOCUMENTAL_CHEQUEO` resuelto

#### Scenario: Tipología inexistente, ambigua o discordante
- **WHEN** la pareja no pertenece al trámite, produce varias relaciones o el nombre no coincide
- **THEN** preflight y almacenamiento fallan cerrados sin invocar `AlmacenaDocumentoTareaWorkflow`

### Requirement: RQ-08 Consulta y descarga SII reales
El sistema SHALL consultar las inscripciones y documentos mediante el contrato SII productivo demostrado en el repositorio, según D-08.

#### Scenario: Consulta por código de barras
- **WHEN** `QueryItems` recibe un código de barras autorizado de hasta 15 caracteres
- **THEN** obtiene token, invoca `consultarInformacionSello` enviándolo en el campo externo `radicado`, transforma cada elemento de `inscripciones[].imagenes[]` y devuelve una clave opaca que distingue código de barras, libro, registro e `idanexo`, sin exponer URLs ni credenciales

#### Scenario: Resolución y descarga de anexo
- **WHEN** preview o ejecución recibe una clave opaca válida
- **THEN** el servidor vuelve a consultar `consultarInformacionSello`, resuelve exactamente el anexo por código de barras, libro, registro e `idanexo`, descarga su URL SII con el transporte seguro y rechaza claves, URLs, formatos o respuestas inconsistentes

#### Scenario: Concordancia de índices por gabinete
- **WHEN** una inscripción SII se almacena en `mercantil`, `rup` o `esal`
- **THEN** el servidor separa código de barras y recibo, normaliza libro/fecha/campos numéricos, usa proponente como matrícula RUP, aplica los nombres de columna propios del gabinete y delega los metadatos documentales comunes al almacenamiento existente

#### Scenario: Trazabilidad de negocio independiente del proveedor
- **WHEN** se registra una llamada a cualquier proveedor externo
- **THEN** conserva como snapshots opcionales la tarea, el radicado, el código de barras y una referencia genérica del proveedor, sin exigir código de barras ni crear FK hacia Workflow
