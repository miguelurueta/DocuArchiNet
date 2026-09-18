<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-67-creacion-vinculacion-expediente

## Fuente y alcance

- Ticket: `DOC-67` — CREACION-VINCULACION-EXPEDIENTE.
- Cambio: `doc-67-creacion-vinculacion-expediente`.
- Jira: `specs/creacion-vinculacion-expediente/jira-context.md`.
- Fuente normativa: `Doc/Actualizacion/workflow/ImportarServicioWeb/PromptBackend/08-creacion-vinculacion-expedientes-sii.md`.
- Evidencia arquitectónica: `Doc/Actualizacion/workflow/ImportarServicioWeb/Exploracion/implementacion-creacion-vinculacion-expedientes-sii.md`, especialmente `Flujo consolidado vigente`.
- Contexto heredado: prompts backend 01 a 07 y `CONTRATO-COMPARTIDO-FRONTEND-BACKEND.md`.
- Perfil: `legacy-webforms-vb`; Web Forms/ASMX, VB.NET, .NET Framework 4.6.1, MySQL y modernización incremental bajo gate.

Jira resume el resultado. El prompt completo y la exploración gobiernan comportamiento, fronteras, restricciones, pruebas y migración. Ante omisión del resumen prevalecen las fuentes versionadas.

## Contexto inspeccionado

### Brecha moderna comprobada

- `Services/Workflow/ImportarServicioWeb/ImportExecutionSteps.vb`: `PrepareImportExecutionStep` prepara un archivo temporal aunque confirma `ExpedientePreparado`; `PrepareImportIndicesExecutionStep` es un marcador sin actualización real.
- `Services/Workflow/ImportarServicioWeb/ImportServiceOrchestrator.vb`: coordina principalmente por item; DOC-67 requiere coordinación por intención e inscripción.
- `Infrastructure/Workflow/ImportarServicioWeb/LegacyImportDocumentStorageAdapter.vb`: delega en `ClassAlmacenamiento.AlmacenaDocumentoTareaWorkflow`; puede heredar expediente, pero no sustituye creación, caché, vinculación e indexación SII explícitas.
- El flujo actual demuestra intención, item, almacenamiento y tarea, pero no todavía `documento → expediente SII`, índices SQL/XML ni caché incremental por documento.

### Núcleo legacy y disposición objetivo

| Responsabilidad | Evidencia de código/símbolo | Disposición |
| --- | --- | --- |
| Configuración | `SolicitaEstructuraTramite`; `Webworkflow.js::GuardarConstanciaIncripcionSII` | Migración directa a repositorio |
| Localización | `SolicitaRegistroExpedienteMatricula`, `SolicitaCacheCreacionExpedienteSII`; `JSExpediente.js::CreaExpedienteVinculaDocumentoSII` | Migrar consulta y verificar físicamente |
| Sujeto SII | `SolicitaEstructuraExpedienteSII` | Adaptador transitorio sin autoridad de sesión |
| Creación física | `ClassGaExpediente.vb::AutoRegistraExpedienteTramite` | Adaptador caracterizado; migración progresiva |
| Coordinación único/múltiple | `ClassGaExpediente.vb::CreaExpedienteIntegracionSII` | Migrar reglas al coordinador |
| Descubrimiento | `SolicitaListaImagenesGabineteEnlace` | Repositorio parametrizado por gabinete/`ENLASE` |
| Plan documental | `SolicitaDocumentosTareaWorkflowVinculacionUnicoExpedientesSII`, `SolicitaDocumentosTareaWorkflowVinculacionMultipleExpedientesSII` | Migrar con destino único por `IdImagen` |
| Vinculación | `VinculaDocumentoExpediente` | Adaptador con precheck/postcheck; `YES` no basta |
| Caché creación | `RegistraCacheCreacionExpedienteSII` | Compatibilidad; no fuente de verdad |
| Caché vinculación | `RegistraCahcheVinculacionSII` | Reemplazo por caché moderna por documento |
| Índices | `ActualizaIndiceDocumentosSII`, `ActualizaIndiceDocumentoCacheExpediente`, `ActualizaIndiceDocumentoIntegracionSII` | Migrar tres campos y verificar SQL/XML |
| Almacenamiento compatible | `SolicitaEstructuraExpedienteDocumentoVinculante` | Mantener consumidores legacy |

### Compatibilidad preservada

- Gate apagado mantiene el recorrido legacy sin doble efecto.
- No modificar `workflow/ClassAlmacenamiento.vb`, `AlmacenaDocumentoTareaWorkflow(...)` ni funciones legacy protegidas.
- No invocar ASMX legacy por HTTP interno ni usar frontend/sesión como autoridad.
- Mantener .NET Framework 4.6.1, orden de compilación VB y contratos compatibles/versionados.
- Procesar secuencialmente; no multiplex ni paralelismo en esta entrega.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | Expediente obligatorio: reutilizar uno válido o crear el faltante antes de almacenar; si no se resuelve, detener. | Prompt §Decisiones; exploración §Decisiones consolidadas | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | Coordinar por intención/inscripción conservando el agregado inscripción-documentos, no ciegamente por imagen. | `CreaExpedienteIntegracionSII`; exploración §Pérdida de agregación | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | Cubrir modo único y múltiple, primario/secundarios, con exactamente un destino por `IdImagen`. | `util_Estado_Multiple_expedienteSII`; planificadores legacy | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | Localizar por matrícula normalizada+gabinete y verificar todos los campos dinámicos `estado_unico=1`. | Exploración §Identidad; `SolicitaRegistroExpedienteMatricula` | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | Tras almacenar, descubrir exclusivamente por `NombreGabinete + ENLASE = RadicadoSII` y deduplicar por imagen. | `SolicitaListaImagenesGabineteEnlace`; decisión confirmada | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | Conservar sellos anteriores; un sello corregido con nuevo `IdImagen` es adicional e incremental. | Ticket; exploración §Comportamiento incremental | D-06 | RQ-06 | Origen: D-06, RQ-06 |
| D-07 | Caché única por `IdTarea+IdImagen+NombreGabinete`, con expediente/radicado, nunca fuente de verdad. | Prompt §Persistencia e idempotencia | D-07 | RQ-07 | Origen: D-07, RQ-07 |
| D-08 | Vincular solo ausentes; reutilizar correcta y detener duplicada/cruzada; cada efecto mutador exige postcondición física. | `VinculaDocumentoExpediente`; prompt §Reconciliación | D-08 | RQ-08 | Origen: D-08, RQ-08 |
| D-09 | Actualizar `NITCEDULA`, `RAZONSOCIAL`, `MATRICULA`; SQL y XML son condiciones independientes de finalización. | Funciones `ActualizaIndice*`; marcador moderno | D-09 | RQ-09 | Origen: D-09, RQ-09 |
| D-10 | Saga persistente: continuar desde último efecto confirmado y usar `ResultadoIncierto` si no es demostrable. | Ausencia de transacción global; prompt §Fallos | D-10 | RQ-10 | Origen: D-10, RQ-10 |
| D-11 | Cada función legacy tendrá caracterización y disposición explícita; no reescritura desde cero ni SQL/reglas duplicados. | Prompt §Estrategia legacy | D-11 | RQ-11 | Origen: D-11, RQ-11 |
| D-12 | Reutilizar E2E DOC-56 (`execution`, `retry`, `recovery`, `concurrency`); no crear infraestructura paralela. | `tools/e2e`; prompt §E2E | D-12 | RQ-12 | Origen: D-12, RQ-12 |
| D-13 | Contexto persistido es autoridad; sesión solo autoriza y toda dependencia legacy se encapsula/contrasta. | Dependencias `HttpContext.Session`; prompt §Contexto | D-13 | RQ-13 | Origen: D-13, RQ-13 |
| D-14 | Implementación aditiva bajo gate con invariancia de funciones y consumidores legacy protegidos. | Prompt §Restricciones | D-14 | RQ-14 | Origen: D-14, RQ-14 |
| D-15 | Consultar el sujeto por transporte moderno seguro y conservar la función legacy como fallback configurable sin modificarla. | E2E ESAL `SII_SUBJECT_UNAVAILABLE`; auditoría de `ConsultaExpedienteMercantilEsal` | D-15 | RQ-15 | Origen: D-15, RQ-15 |

## Requisitos verificables

| ID | Resultado observable | Escenario o aceptación | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | Toda intención resuelve/verifica expedientes antes de almacenar. | Existente se reutiliza; faltante se crea una vez; irresoluble no almacena/completa. | Respuesta perdida puede duplicar sin consulta previa. |
| RQ-02 | Inscripción conserva libro, registro, matrícula/proponente, sujeto y documentos. | Varios documentos permanecen asociados durante planificación, ejecución y recuperación. | Mapper actual aplana imágenes. |
| RQ-03 | Plan único/múltiple es inequívoco. | Primario/secundarios según configuración; ninguna imagen sin destino o con dos. | Inconsistencia de matrícula secundaria legacy. |
| RQ-04 | Búsqueda e identidad usan reglas consistentes/configurables. | MERCANTIL, ESAL y RUP normalizan; coinciden todos los `estado_unico=1`. | No codificar lista fija ni confiar solo en caché. |
| RQ-05 | Tras almacenar se descubre universo completo por gabinete/`ENLASE`. | Documentos previos/nuevos se deduplican por `IdImagen` y entran al plan físico. | No inventar otra pertenencia con tarea. |
| RQ-06 | Reejecución incremental. | Sin cambios no repite; nuevo sello procesa solo nuevo `IdImagen` y conserva anteriores. | Caché global puede ocultar posteriores. |
| RQ-07 | Caché documental persistente, única y reconciliable. | `UNIQUE(task_id,image_id,cabinet_name)`; conflicto no se sobrescribe; registrar tras verificar. | DDL requiere migración/rollback. |
| RQ-08 | Documento vinculado exactamente al expediente esperado. | Ausente se crea; correcta se conserva; duplicada/cruzada bloquea con código seguro. | `YES` no prueba destino. |
| RQ-09 | Índices documentales/electrónicos materialmente actualizados. | Tres campos por gabinete/sujeto; SQL y XML verificados por separado. | XML fuera de transacción SQL. |
| RQ-10 | Fallos parciales no repiten efectos confirmados. | Continuar tras fallos en expediente, documento, vínculo, índices o caché. | Saga y reconciliación obligatorias. |
| RQ-11 | Cada función legacy tiene trazabilidad y prueba equivalente. | Matriz función→efecto→destino→caracterización→equivalencia. | Efectos ocultos de sesión/archivo. |
| RQ-12 | Las 18 aserciones E2E reutilizan activos DOC-56. | Evidencias `passed/failed/blocked`, consultas `SELECT`, sin runner/escenario paralelo. | Requiere autorización y muestras descartables. |
| RQ-13 | Frontend/sesión no deciden expediente. | Reintento usa contexto persistido; sesión solo valida actor. | Globales pueden apuntar a otra tarea/ruta. |
| RQ-14 | Gate apagado conserva legacy y evita doble operación. | Archivos invariantes; gate restaurado a false/listas vacías aun ante fallo. | Activación incompleta afecta terceros. |
| RQ-15 | Consulta de sujeto tipada, acotada y observable, con fallback legacy conservado. | MERCANTIL/ESAL/RUP usan transporte moderno; respuesta incompleta bloquea creación; fallback es configurable. | Dependencia externa y fallback temporal legacy. |

## Compuerta técnica previa a mutaciones

Bloquean el efecto correspondiente hasta aportar evidencia de código, configuración o `SELECT`:

1. Caracterizar tablas/archivos/efectos de `AutoRegistraExpedienteTramite`.
2. Caracterizar `VinculaDocumentoExpediente`, significado de `YES`, relación física y SQL/XML.
3. Inventariar tablas/claves de cachés de creación, vinculación e inscripción.
4. Localizar carga por trámite de campos `estado_unico=1`.
5. Demostrar normalización MERCANTIL, ESAL, RUP, primario/secundarios.
6. Confirmar documentos previos exigidos por `CreaExpedienteIntegracionSII`.
7. Demostrar reglas de tipología para destino único por `IdImagen`.
8. Definir verificación `SELECT` y de archivo para SQL/XML.
9. Clasificar efectos reintentables y sujetos a reconciliación previa.
10. Identificar ventanas de respuesta perdida y discrepancias SQL/XML/caché.

Una respuesta no demostrable se registra como bloqueo; nunca se completa con suposición.

## Secuencia contractual

1. Consultar SII preservando inscripciones/imágenes; validar selección, contexto y tipologías.
2. Crear o recuperar intención idempotente.
3. Resolver configuración, gabinete, roles y `estado_unico=1`.
4. Buscar por matrícula normalizada/gabinete y verificar expediente físico.
5. Crear solo faltantes (primario/secundarios), verificar y cachear creación.
6. Persistir plan `inscripción/tipología → expediente`.
7. Almacenar items secuencialmente y obtener `IdImagen`.
8. Consultar nuevamente por gabinete+`ENLASE`; deduplicar.
9. Persistir plan físico con exactamente un `IdImagen → expediente`.
10. Consultar caché y relación física; reutilizar correcta, crear ausente, detener conflicto.
11. Cachear solo relación verificada; actualizar tres índices y confirmar SQL/XML.
12. Reconciliar universo completo y completar solo si todos los efectos están confirmados.

## Estados mínimos

- Intención: `Creada → Validada → ExpedientesPlanificados → ExpedientesResueltos → ItemsSiiAlmacenados → UniversoDocumentalConsultado → VinculacionesProcesadas → IndicesYXmlActualizados → Reconciliada → Completada`.
- Item SII: estados de descarga, preparación y almacenamiento.
- Documento relacionado: `Descubierto → DestinoResuelto → RelacionConsultada → Vinculado|YaVinculado → IndicesActualizados → XmlConfirmado → Reconciliado`.
- Agregados alternos: `Parcial` y `ResultadoIncierto`.

## Reglas de trazabilidad

1. Cada D-XX debe desarrollarse en `design.md`, reflejarse en un requirement/scenario y vincularse a tareas con `Origen: D-XX, RQ-XX`.
2. Cada función legacy debe tener fila propia de migración y caracterización/equivalencia.
3. Cada tarea debe indicar complejidad, área/archivos, una entrega y verificación observable.
4. E2E reutiliza runner, adaptador, escenarios, perfiles, autenticación, gate, controles y reporte existentes.
5. No marcar sincronización hasta que las referencias existan realmente.

## Resultado del refinamiento

- Contenido consolidado desde Jira, prompt completo y exploración.
- Estado: `approved`; D-01 a D-15 están desarrolladas en `design.md`, RQ-01 a RQ-15 tienen escenarios verificables en `spec.md` y las tareas atómicas declaran su origen.
- Auditoría: sin marcadores abiertos y con cobertura bidireccional de decisiones y requisitos en diseño, especificación y tareas.
- Próximo: sincronizar encabezados OPSXJ, validar la compuerta y comenzar por la caracterización legacy; ninguna tarea mutadora puede adelantarse a su evidencia correspondiente.
