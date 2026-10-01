<!-- opsxj:refinement-traceability version=1 artifact=spec decisions=D-01,D-02,D-03,D-04,D-05,D-06 -->
## Purpose

Garantizar que Radicación Simplificada adjunte documentos con una identidad de radicado autorizada y consistente, preserve los consumidores legacy y actualice la interfaz sin recargas.

## ADDED Requirements

### Requirement: RQ-01 — Radicado autoritativo del registro de estado

Por D-01, el sistema SHALL resolver el radicado de `ADJUNTARADICACION` desde `ra_rad_estados_modulo_radicacion.consecutivo_radicado` usando el identificador del registro validado en servidor.

#### Scenario: DAT_ADIC_TAR no entrega radicado

- **GIVEN** un registro autorizado con `consecutivo_radicado` no vacío y una tarea cuya tabla `DAT_ADIC_TAR` no contiene un radicado utilizable
- **WHEN** el usuario adjunta un documento desde Radicación Simplificada
- **THEN** plantilla, índices, almacenamiento y respuesta usan el radicado del registro de estado

#### Scenario: Identidad inconsistente

- **WHEN** el registro no existe, no pertenece al contexto, tiene radicado vacío o el valor informativo no vacío difiere del persistido
- **THEN** la operación falla antes del almacenamiento y no crea una fila documental

### Requirement: RQ-02 — Resolución única de contexto

Por D-02, el sistema SHALL materializar una vez por carga lógica un `ContextoAdjuntoRadicacion` inmutable mediante `IContextoAdjuntoRadicacionRepository.ObtenerAutorizado` y una consulta parametrizada.

#### Scenario: Contexto válido

- **WHEN** `ServicioAdjuntoRadicacion.Adjuntar` recibe un identificador autorizado
- **THEN** obtiene radicado, tarea, trámite, plantilla y destino desde el repository y transporta el mismo contexto hasta el almacenamiento

#### Scenario: Aislamiento entre cargas

- **WHEN** se procesan varios archivos o pestañas
- **THEN** cada POST construye su propio contexto sin usar `Session`, variables estáticas ni estado de página como autoridad

### Requirement: RQ-03 — Índices construidos con radicado explícito

Por D-03, el sistema SHALL construir plantilla e índices con un radicado obligatorio ya validado y SHALL NOT volver a resolverlo mediante `SolicitaRadicadoTareaWorkflow`.

#### Scenario: Preparación específica

- **WHEN** `PreAlmacenaDocumentosRadicacionConContexto` prepara el documento
- **THEN** invoca una vez `ConstruirDatosCamposIndiceGabineteConRadicado` y reutiliza `AlmacenaDocumentosRadicacion`

#### Scenario: Radicado vacío

- **WHEN** el constructor recibe un radicado vacío
- **THEN** rechaza la operación antes de consultar la plantilla

### Requirement: RQ-04 — Compatibilidad de sobrecargas y eventos legacy

Por D-04, el sistema SHALL conservar una sobrecarga legacy de diez argumentos y SHALL usar una sobrecarga de doce argumentos exclusivamente para `ADJUNTARADICACION`, sin parámetros opcionales ambiguos.

#### Scenario: Consumidor legacy

- **WHEN** `GESTION_RESPUESTA`, `WORKFLOWSELECCION`, `PRODUCCION` u otro consumidor histórico invoca diez argumentos
- **THEN** conserva su recorrido, firma, retorno y resolución histórica

#### Scenario: Radicación Simplificada

- **WHEN** la llamada existente del handler entrega doce argumentos
- **THEN** la sobrecarga específica valida el evento y delega en `ServicioAdjuntoRadicacion.Adjuntar`

### Requirement: RQ-05 — Contrato de interfaz sin recarga

Por D-05, el sistema SHALL preservar sin modificaciones el handler y el JavaScript propio de Radicación Simplificada, además del contrato `stru_datos_image_lista`/`uploadFiles`. La proyección compartida SHALL distinguir `ADJUNTARADICACION` para insertar una fila nueva y SHALL conservar el recorrido histórico de actualización para los demás eventos.

#### Scenario: Carga válida

- **WHEN** el backend almacena correctamente el documento
- **THEN** el handler mapea todos los campos actuales y la interfaz inserta o actualiza la fila mediante JavaScript sin postback, `DataBind` ni recarga parcial o completa

#### Scenario: Fronteras protegidas

- **WHEN** se revisa el diff final
- **THEN** `generic_control/FileUploadHandler_.ashx.vb` y `js/RadicadorSimplificado/Web_form_radicacion_simpilificada.js` no presentan cambios, y el cambio de `generic_control/FileUploadHandler.js` está delimitado por `evento_adjunta == "ADJUNTARADICACION"` con el comportamiento legacy preservado en `else`

### Requirement: RQ-06 — Evidencia de no regresión y E2E controlada

Por D-06, el sistema SHALL contar con pruebas locales deterministas, compilación completa y una E2E real positiva y negativa integrada en `tools/e2e`.

#### Scenario: Validación local

- **WHEN** se valida el cambio antes de desplegar
- **THEN** aprueban las pruebas del defecto, autorización, resolución única, sobrecargas, contratos compartidos, consumidores legacy, respuesta y compilación

#### Scenario: E2E autorizada

- **GIVEN** autorización explícita de ambiente, cuenta, registro descartable, fixture, tipología y acceso de evidencia de solo lectura
- **WHEN** se ejecutan los escenarios positivo y de discrepancia informativa
- **THEN** se verifica persistencia única o rechazo sin persistencia, inserción JavaScript sin recarga y gate `WorkflowCentroTrabajoModernActive` en `false` con listas vacías

#### Scenario: E2E sin autorización

- **WHEN** falta cualquiera de los recursos o autorizaciones requeridos
- **THEN** no se ejecuta la E2E, se registra el bloqueo operacional y el ticket no se declara cerrado

### Requirement: RQ-07 — Infraestructura existente como única composición técnica

Por D-07, `ADJUNTARADICACION` SHALL reutilizar las abstracciones, la factoría de conexión de Radicación, el ejecutor ADO.NET y las utilidades E2E existentes, sin introducir fábricas privadas ni reconstruir credenciales dentro del repository.

#### Scenario: Composición productiva

- **WHEN** `ClassAlmacenamiento` compone el servicio de adjuntos
- **THEN** obtiene la cadena mediante `ModuleSessionConnectionStringResolver` e inyecta `RadicacionModuleConnectionFactory` y `AdoNetDataExecutor`
- **AND** `MySqlContextoAdjuntoRadicacionRepository` no dispone de constructor productivo implícito ni de una fábrica de conexión anidada
