<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-55-adaptador-sii-asmx

## Fuente y alcance

- Ticket: `DOC-55` — ADAPTADOR-SII-ASMX.
- Cambio OpenSpec: `doc-55-adaptador-sii-asmx`.
- Perfil: ASP.NET WebForms/ASMX en VB.NET, con servicios e infraestructura moderna paralela.
- Alcance: proveedor SII, mapeo contractual, compatibilidad legacy localizada, preview mediado, ASMX moderno, pruebas y documentación.

## Contexto inspeccionado

- Contratos y puertos B01-B05 en `DTOs/Workflow/ImportarServicioWeb/`, `Modelo/Workflow/ImportarServicioWeb/`, `Services/Workflow/ImportarServicioWeb/` e `Infrastructure/Workflow/ImportarServicioWeb/`.
- Comportamiento histórico en `webservice/WebService_integracion_sii.asmx.vb`, `webservice/WebServiceGaExpediente.asmx.vb`, `Integracionccv/Class_consultarInformacionSello.vb`, `Integracionccv/Class_ClassResfull.vb` y `workflow/ClassAlmacenamiento.vb`.
- El almacenamiento moderno ya dispone de `LegacyImportDocumentStorageAdapter`, única frontera nueva autorizada para invocar `AlmacenaDocumentoTareaWorkflow(...)`.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia de código | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | Registrar únicamente la identidad exacta `INTEGRACIONSII`; un proveedor desconocido nunca usa SII como fallback. | `Services/Workflow/ImportarServicioWeb/RegistroProveedoresImportacion.vb` | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | Implementar el cliente SII sobre el transporte HTTP común, sin red real en pruebas. | `Infrastructure/Workflow/ImportarServicioWeb/Http/` | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | Traducir identidad y datos registrales exclusivamente dentro del mapper SII. | `DTOs/Workflow/ImportarServicioWeb/ImportarServicioWebDtos.vb` | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | Confinar `YES`, `CTRL`, `CTRLRETURN` y `dato_lista` al adaptador legacy nuevo. | `webservice/WebService_integracion_sii.asmx.vb` | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | Mantener el ASMX moderno como frontera delgada: contexto, gate, validación, servicio y serialización. | `webservice/` y `Services/Workflow/ImportarServicioWeb/` | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | Mediar preview en servidor con autorización, expiración y encabezados seguros. | `Modelo/Workflow/ImportarServicioWeb/ImportarServicioWebInterfaces.vb` | D-06 | RQ-06 | Origen: D-06, RQ-06 |
| D-07 | Evaluar `WorkflowCentroTrabajoModernActive` en servidor; apagado retorna `FEATURE_DISABLED` sin efectos. | Configuración y servicios modernos de workflow | D-07 | RQ-07 | Origen: D-07, RQ-07 |
| D-08 | Persistir solo mediante el orquestador y el puerto moderno; no modificar `ClassAlmacenamiento`. | `Infrastructure/Workflow/ImportarServicioWeb/Storage/LegacyImportDocumentStorageAdapter.vb` | D-08 | RQ-08 | Origen: D-08, RQ-08 |
| D-09 | Preservar rutas legacy y validar el adaptador con fixtures saneados, build y evidencia reproducible. | ASMX/Integracionccv/workflow existentes y `Tests/` | D-09 | RQ-09 | Origen: D-09, RQ-09 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptación | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | Solo `INTEGRACIONSII` resuelve el proveedor SII. | Proveedor distinto produce error explícito. | Evita fallback accidental. |
| RQ-02 | Consulta SII usa el transporte común y respuestas acotadas. | Fixtures reproducen éxito/error sin red. | No expone token ni transporte. |
| RQ-03 | Query y capacidades exponen contratos comunes saneados. | Campos SII se normalizan con clave externa estable. | El núcleo no conoce semántica registral. |
| RQ-04 | Compatibilidad conserva códigos y forma legacy. | Cada resultado estructurado tiene traducción determinista. | Solo el adaptador manipula delimitadores. |
| RQ-05 | ASMX moderno delega casos de uso sin almacenar directamente. | Auditoría de código prueba ausencia de negocio y `ClassAlmacenamiento`. | No sustituye endpoints vigentes. |
| RQ-06 | Preview autorizado entrega contenido y encabezados seguros. | Expiración, tipo o tamaño inválido se rechazan. | No revela ruta física ni secretos. |
| RQ-07 | Gate apagado bloquea rutas modernas sin efectos. | Respuesta `FEATURE_DISABLED` y cero invocaciones downstream. | Rollback inmediato por configuración. |
| RQ-08 | Documento SII se normaliza y persiste una sola vez por el puerto moderno. | Orquestador conserva correlación e idempotencia. | No se modifica almacenamiento legacy. |
| RQ-09 | Pruebas y documentación demuestran compatibilidad. | Suite focal, build y diff protegido quedan registrados. | No ejecutar SII real ni E2E sin autorización. |

## Reglas de trazabilidad obligatorias

1. Cada decisión D-XX aparece en design, spec y al menos una tarea con `Origen: D-XX, RQ-XX`.
2. Los ASMX existentes, `Integracionccv/`, `ServiciosIntegracion/`, `workflow/ClassAlmacenamiento.vb` y `js/java_general/JSProgresBar.js` son superficies de solo lectura para DOC-55.
3. No se activará el gate ni se ejecutarán E2E autenticados, carga o llamadas reales a SII sin autorización explícita.

## Resultado del refinamiento

- Estado: aprobado.
- Alcance, decisiones, compatibilidad, trazabilidad y verificaciones quedaron definidos para implementación.
