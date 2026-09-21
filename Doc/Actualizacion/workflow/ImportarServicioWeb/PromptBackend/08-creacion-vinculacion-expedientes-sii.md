# Prompt backend 08 — Creación y vinculación de expedientes SII

Implementa en el flujo moderno de `ImportarServicioWeb` la resolución, creación idempotente, vinculación, actualización de índices, caché y reconciliación de expedientes SII, preservando la convivencia con el recorrido legacy.

La entrega extiende los contratos, la intención, la orquestación y la reconciliación construidos por los Prompts backend 01 a 07. No reemplaza ni modifica la lógica legacy existente.

## Rol esperado

Actúa como arquitecto y desarrollador senior en ASP.NET Web Forms, ASMX, VB.NET sobre .NET Framework 4.6.1, MySQL, gestión documental, flujos idempotentes y modernización incremental de sistemas legacy.

Antes de cambiar código, reconstruye el comportamiento real desde el repositorio. No conviertas nombres de métodos, comentarios o fases existentes en supuestos funcionales.

## Objetivo

Conseguir que una intención moderna de importación SII:

1. conserve la relación entre inscripciones y documentos;
2. resuelva desde backend la configuración de expedientes del trámite;
3. reutilice expedientes existentes;
4. cree de forma idempotente expedientes primarios o secundarios cuando corresponda;
5. almacene los items SII seleccionados y obtenga sus `IdImagen`;
6. consulte después del almacenamiento todos los documentos actuales mediante `NombreGabinete + ENLASE = RadicadoSII`;
7. asigne cada `IdImagen` a exactamente un expediente único, primario o secundario;
8. vincule solamente las relaciones ausentes y controle la idempotencia mediante caché persistente por documento;
9. actualice realmente `NITCEDULA`, `RAZONSOCIAL` y `MATRICULA`, además del índice electrónico SQL y su archivo XML;
10. reconcilie documento, expediente, relación, caché, índices y XML antes de declarar la intención completada.

## Contexto obligatorio

Leer completamente antes de diseñar o implementar:

```text
Doc/Actualizacion/workflow/ImportarServicioWeb/
├── Exploracion/
│   └── implementacion-creacion-vinculacion-expedientes-sii.md
├── PromptBackend/
│   ├── 01-contratos-contexto-registro-multiproveedor.md
│   ├── 02-clientes-http-asincronos-seguridad.md
│   ├── 03-preflight-intencion-idempotencia.md
│   ├── 04-orquestacion-secuencial-estados-compensacion.md
│   ├── 05-reconciliacion-lista-documentos.md
│   ├── 06-adaptador-sii-compatibilidad-asmx.md
│   └── 07-pruebas-backend-evidencia.md
└── CONTRATO-COMPARTIDO-FRONTEND-BACKEND.md
```

La exploración indicada es contexto normativo de esta entrega. El apartado `Flujo consolidado vigente` sustituye el diagrama preliminar y gobierna la implementación. No implementar una versión simplificada que ignore sus decisiones funcionales consolidadas.

Inspeccionar además, sin modificar salvo autorización posterior explícita:

```text
Gestion/ClassGaExpediente.vb
Integracionccv/ClassConsultaExpedienteSII.vb
Integracionccv/ClassRaSIiCacheExpediente.vb
Integracionccv/ClassRaSiiCacheVinculacion.vb
Integracionccv/ClassRaSIICacheActualizaIndice.vb
Docuarchi/ClassDaGabinete.vb
workflow/ClassAlmacenamiento.vb
webservice/WebServiceGaExpediente.asmx.vb
webservice/WebService_integracion_sii.asmx.vb
js/java_general/JSExpediente.js
js/workflow/Webworkflow.js
radicador/Class_tipo_doc_entrante.vb
```

## Decisiones funcionales obligatorias

- Resolver expediente es obligatorio: reutilizar uno válido o crear el faltante. Si no puede resolverse, no almacenar ni completar.
- La única forma disponible de descubrir documentos relacionados es `NombreGabinete + ENLASE = RadicadoSII`; no exigir otra relación física con la tarea.
- Conservar el sello anterior cuando se almacene uno corregido. El nuevo `IdImagen` se procesa como documento adicional.
- Incluir expediente único y múltiples expedientes con primario y secundarios.
- Las tablas legacy no aportan restricciones únicas suficientes para esta idempotencia.
- La localización SII usa matrícula normalizada más gabinete. La identidad física se verifica con todos los campos configurados `estado_unico=1`; no codificar una lista fija.
- Actualizar obligatoriamente `NITCEDULA`, `RAZONSOCIAL` y `MATRICULA` según gabinete.
- El índice electrónico SQL y el archivo XML son obligatorios para completar.
- Una caché nunca sustituye la consulta y reconciliación del efecto físico.
- El servicio no se tratará como multiplex: aunque una consulta o un radicado exponga varios items, su almacenamiento, vinculación, indexación y confirmación se ejecutan secuencialmente, documento por documento.

## Compuerta de investigación técnica obligatoria

Antes de escribir código mutador, documentar con archivo, función, sentencia o tabla la respuesta a estas preguntas:

1. ¿Qué tablas y registros modifica `AutoRegistraExpedienteTramite` directa e indirectamente?
2. ¿Qué tablas y registros modifica `VinculaDocumentoExpediente`?
3. ¿Qué tablas modifican las cachés de creación, vinculación e inscripción SII?
4. ¿Cómo se obtienen para cada trámite los campos de auto-registro marcados `estado_unico=1` y cómo se verifica con ellos el expediente físico?
5. ¿Cómo se normaliza la identidad de matrícula para `MERCANTIL`, `ESAL`, `RUP`, primario y secundarios sin reproducir la inconsistencia legacy entre creación y recuperación?
6. ¿Qué documentos exige `CreaExpedienteIntegracionSII` que existan antes de crear el expediente?
7. ¿Cómo se distribuyen las tipologías entre expediente primario y expedientes secundarios garantizando un único destino por `IdImagen`?
8. ¿Cómo se verifican de manera independiente el índice electrónico SQL y el archivo XML?
9. ¿Qué efectos son reintentables y cuáles requieren reconciliación previa?
10. ¿Qué escritura o respuesta perdida puede dejar discrepancia entre SQL, XML y cachés?

Si una respuesta no puede demostrarse desde código, configuración o consultas `SELECT`, registrarla como bloqueo. Está prohibido completar el comportamiento con una suposición silenciosa.

## Rutas canónicas de implementación

Extender las rutas modernas existentes y crear responsabilidades equivalentes a:

```text
DTOs/Workflow/ImportarServicioWeb/
└── ImportarServicioWebDtos.vb

Modelo/Workflow/ImportarServicioWeb/
├── ImportarServicioWebModels.vb
└── ImportarServicioWebInterfaces.vb

Services/Workflow/ImportarServicioWeb/
├── ImportServiceOrchestrator.vb
├── ImportIntentStateMachine.vb
├── ImportExecutionSteps.vb
├── ImportExpedientCoordinator.vb
├── ImportExpedientPlan.vb
├── ImportRelatedDocumentCoordinator.vb
├── ImportRelatedDocumentPlan.vb
└── ServicioReconciliacionImportacion.vb

Infrastructure/Workflow/ImportarServicioWeb/Expedients/
├── LegacyImportExpedientAdapter.vb
├── LegacyImportExpedientRelationAdapter.vb
└── LegacyImportDocumentIndexAdapter.vb

Infrastructure/Repositories/Workflow/ImportarServicioWeb/
├── MySqlImportExpedientConfigurationRepository.vb
├── MySqlImportExpedientCacheRepository.vb
├── MySqlImportRelatedDocumentRepository.vb
├── MySqlImportDocumentLinkCacheRepository.vb
├── MySqlImportIntentRepository.vb
└── MySqlImportReconciliationRepository.vb

Infrastructure/Workflow/ImportarServicioWeb/Sii/
└── SiiImportContractMapper.vb

Tests/
├── importar-servicio-web-expedient-configuration.test.cjs
├── importar-servicio-web-expedient-planning.test.cjs
├── importar-servicio-web-expedient-creation.test.cjs
├── importar-servicio-web-expedient-relation.test.cjs
├── importar-servicio-web-related-documents.test.cjs
├── importar-servicio-web-document-link-cache.test.cjs
├── importar-servicio-web-electronic-index.test.cjs
├── importar-servicio-web-expedient-idempotency.test.cjs
├── importar-servicio-web-expedient-reconciliation.test.cjs
├── importar-servicio-web-expedient-failure-injection.test.cjs
└── importar-servicio-web-expedient-legacy-invariance.test.cjs
```

Los nombres pueden ajustarse a las convenciones demostradas del repositorio, pero deben conservarse las fronteras de responsabilidad.

Agregar todo archivo VB nuevo al `.vbproj` en el orden correcto de compilación.

## Diseño obligatorio

### Agregado de inscripción

No reducir la respuesta SII únicamente a una lista plana de imágenes. Mantener un modelo interno equivalente a:

```text
Inscripción
├── inscriptionKey estable
├── libro
├── registro
├── matrícula
├── proponente
├── identificación
├── razón social
├── propietario
└── documentos[]
```

El frontend puede continuar consumiendo documentos seleccionables, pero la autoridad y los datos necesarios para expediente deben obtenerse y validarse nuevamente en servidor.

### Coordinación a nivel de intención

La resolución y creación de expedientes ocurre una vez por intención o grupo de inscripción. No ejecutarla ciegamente una vez por imagen.

Implementar un coordinador que:

- cargue todas las inscripciones seleccionadas;
- resuelva configuración de trámite y gabinete;
- consulte `util_Estado_Crea_ExpedienteSII`;
- consulte `util_Estado_Multiple_expedienteSII`;
- cargue reglas de expedientes secundarios;
- busque expedientes y cachés existentes;
- cree únicamente los faltantes;
- persista antes del almacenamiento un plan `inscripción/tipología → expediente`;
- después del almacenamiento consulte todos los documentos por `NombreGabinete + ENLASE = RadicadoSII`;
- persista entonces el plan físico definitivo `IdImagen → expediente`.

El plan anterior al almacenamiento no puede considerarse el plan físico final porque los nuevos `IdImagen` todavía no existen y el universo incluye documentos previamente almacenados.

### Puertos modernos

Definir interfaces modernas equivalentes a:

```text
IImportExpedientConfigurationRepository
ISiiExpedientSubjectResolver
IImportExpedientRepository
IImportDocumentExpedientRelationPort
IImportExpedientCacheRepository
IImportRelatedDocumentRepository
IImportDocumentLinkCacheRepository
IImportDocumentIndexUpdater
IImportElectronicIndexVerifier
```

Los servicios modernos dependen de estas interfaces. Las referencias a clases legacy quedan confinadas a adaptadores de infraestructura.

No invocar ASMX legacy mediante HTTP interno.

### Estrategia obligatoria para funciones legacy

Esta sección es el eje de la entrega, no material de referencia. La implementación debe partir de las funciones existentes y migrar su comportamiento comprobado. Está prohibido construir en paralelo una solución “desde cero” que ignore sus consultas, reglas por gabinete, efectos laterales, orden de operaciones o casos de expediente único/primario/secundario.

| Responsabilidad | Función legacy | Tratamiento |
|---|---|---|
| Configuración | `SolicitaEstructuraTramite` | Migrar a repositorio moderno |
| Localización de expediente | `SolicitaRegistroExpedienteMatricula`, `SolicitaCacheCreacionExpedienteSII` | Migrar; verificar siempre el expediente físico |
| Datos del sujeto SII | `SolicitaEstructuraExpedienteSII` | Reutilización temporal detrás de puerto |
| Creación física | `AutoRegistraExpedienteTramite` | Adaptador transitorio; caracterizar y migrar progresivamente |
| Coordinación único/múltiple | `CreaExpedienteIntegracionSII` | Migrar sus reglas; no invocar ASMX ni JavaScript |
| Descubrimiento por enlace | `SolicitaListaImagenesGabineteEnlace` | Migrar a repositorio parametrizado |
| Planificación documental | `SolicitaDocumentosTareaWorkflowVinculacionUnicoExpedientesSII`, `SolicitaDocumentosTareaWorkflowVinculacionMultipleExpedientesSII` | Migrar reglas garantizando un destino por `IdImagen` |
| Vinculación física e índice electrónico | `VinculaDocumentoExpediente` | Adaptador transitorio con precheck y postcheck; no confiar solo en `YES` |
| Caché de creación | `RegistraCacheCreacionExpedienteSII` | Compatibilidad; no fuente de verdad |
| Caché de vinculación | `RegistraCahcheVinculacionSII` | Sustituir como control moderno por caché por documento |
| Índices SII | `ActualizaIndiceDocumentosSII`, `ActualizaIndiceDocumentoCacheExpediente`, `ActualizaIndiceDocumentoIntegracionSII` | Migrar para los tres campos obligatorios y verificar efectos |

No modificar las funciones legacy protegidas en esta entrega. “Migrar” significa implementar su comportamiento requerido detrás de contratos modernos y mantener adaptadores transitorios únicamente donde la complejidad física lo exija.

#### Método obligatorio de migración por función

Antes de implementar cada responsabilidad moderna:

1. localizar la función legacy y todos sus llamadores directos;
2. documentar parámetros efectivos, datos de sesión implícitos, consultas, tablas, archivos, retorno y efectos laterales;
3. crear pruebas de caracterización para los comportamientos reutilizados y para sus respuestas ambiguas;
4. separar reglas puras, lecturas y mutaciones;
5. migrar las reglas y lecturas a servicios/repositorios modernos sin cambiar su semántica demostrada;
6. encapsular temporalmente las mutaciones físicas complejas detrás de un puerto cuando una migración directa no sea segura;
7. agregar precondición, postcondición, idempotencia y reconciliación modernas alrededor del efecto;
8. demostrar equivalencia o documentar expresamente la diferencia aprobada.

La migración no consiste en copiar literalmente el cuerpo de una función. Tampoco consiste en llamarla ciegamente. Se debe conservar la regla válida, retirar dependencias de sesión o UI cuando corresponda y controlar los defectos conocidos mediante contratos modernos.

#### Matriz de trazabilidad obligatoria

Crear y mantener durante la implementación una matriz con una fila por **cada función** de la tabla anterior:

```text
función legacy
→ archivo y líneas verificadas
→ llamadores y orden dentro del flujo actual
→ entradas explícitas e implícitas
→ consultas/tablas/archivos afectados
→ regla reutilizada
→ defecto o ambigüedad conocida
→ destino moderno: migración directa | adaptador transitorio | reemplazo controlado
→ contrato/puerto/clase moderna
→ prueba de caracterización
→ prueba moderna equivalente
→ estado: pendiente | caracterizada | migrada | adaptada | verificada
```

Ninguna función puede quedar agrupada bajo expresiones genéricas como “lógica legacy reutilizada”. Si una función listada deja de ser necesaria, demostrar qué responsabilidad la reemplaza y por qué su omisión no elimina un efecto requerido.

#### Prioridad de reutilización segura

- Migrar primero consultas y reglas deterministas: estructura del trámite, identidad, descubrimiento por `ENLASE` y clasificación único/múltiple.
- Reutilizar detrás de puertos los efectos físicos de alto riesgo mientras se caracterizan: creación, vinculación e índice electrónico.
- Sustituir solamente los controles legacy insuficientes, como la caché global de vinculación, preservando compatibilidad donde existan consumidores comprobados.
- No duplicar SQL o reglas en coordinadores. Toda regla migrada debe tener un único dueño moderno.
- No considerar confiable un retorno textual (`YES`, identificador o mensaje) sin postcondición física verificable.

### Contexto explícito

No utilizar `ID_TAREA_SELECCIONDA`, ruta, usuario o empresa de sesión como autoridad mutable durante la ejecución.

El contexto persistido en la intención es la fuente primaria. La sesión se utiliza solamente para comprobar que el actor sigue autorizado.

Si una función legacy requiere sesión, encapsular la compatibilidad y demostrar que los valores coinciden con el contexto inmutable. No alterar silenciosamente la sesión para fabricar contexto.

## Persistencia e idempotencia

Persistir explícitamente el agregado de inscripción y los efectos de expediente. El diseño debe cubrir, como mínimo:

```text
workflow_import_inscription
├── intent_id
├── inscription_key
├── book / registration
├── enrollment / proponent
├── expedient_id
├── expedient_role
├── expedient_status
└── cache_status

workflow_import_intent_item
├── inscription_key
├── document_id
├── expedient_id
├── storage_status
├── relation_status
├── index_status
└── cache_status

workflow_import_related_document
├── intent_id
├── task_id
├── image_id
├── cabinet_name
├── sii_radicado
├── expected_expedient_id
├── relation_status
├── index_status
├── xml_index_status
└── reconciliation_status

workflow_import_document_link_cache
├── task_id
├── image_id
├── cabinet_name
├── expected_expedient_id
├── sii_radicado
├── relation_status
├── created_utc
└── verified_utc
```

No adoptar literalmente estos nombres sin revisar las convenciones y migraciones actuales. Documentar la decisión final.

Proteger como identidades idempotentes:

```text
matrícula normalizada + gabinete → localización SII del expediente
campos configurados estado_unico=1 → identidad física del expediente
IdTarea + IdImagen + NombreGabinete → caché moderna de vinculación
```

La migración moderna de la caché documental debe imponer `UNIQUE(task_id, image_id, cabinet_name)` y guardar `expected_expedient_id` y `sii_radicado`. Si una entrada cacheada apunta a otro expediente, representar conflicto; no sobrescribirla silenciosamente.

La caché es un diario persistente y no una fuente de verdad. Tanto si existe como si falta, consultar la relación física antes de decidir vincular o confirmar. Registrar o actualizar la caché solamente después de demostrar que el documento apunta al expediente esperado.

La protección debe existir en persistencia o mediante una combinación demostrable de consulta autoritativa y escritura condicionada. Un `If` en memoria no es suficiente para concurrencia.

No borrar ni compensar automáticamente un expediente creado si posteriormente falla un documento. Conservar el efecto y permitir recuperación.

## Máquina de estados

Corregir la semántica de fases. Implementar o mapear estados equivalentes a:

```text
Creada
  ↓
Validada
  ↓
ExpedientesPlanificados
  ↓
ExpedientesResueltos
  ↓
ItemsSiiAlmacenados
  ↓
UniversoDocumentalConsultado
  ↓
VinculacionesProcesadas
  ↓
IndicesYXmlActualizados
  ↓
Reconciliada
  ↓
Completada
```

Mantener estados por item SII para descarga, archivo temporal y almacenamiento. Persistir además por cada documento relacionado estados equivalentes a `Descubierto`, `DestinoResuelto`, `RelacionConsultada`, `Vinculado` o `YaVinculado`, `IndicesActualizados`, `XmlConfirmado` y `Reconciliado`. No confundir estados agregados de intención con estados de item o de documento relacionado.

- `ExpedientePreparado` no puede continuar significando solamente “archivo temporal escrito”. Renombrar o redefinir mediante una migración contractual compatible.
- `IndicesActualizados` solo se confirma después de una actualización real.
- `CacheActualizado` solo se confirma después de verificar las cachés requeridas.
- `Completada` solo se alcanza después de reconciliación autoritativa.
- Todo estado nuevo debe versionar DTO, fixture, mapeo frontend/backend y documentación.

## Secuencia funcional

Implementar la siguiente secuencia consolidada:

```text
1. Consultar SII preservando inscripciones e imágenes.
2. Validar selección, contexto y tipologías.
3. Crear o recuperar la intención idempotente.
4. Resolver configuración, gabinete, roles y campos `estado_unico=1`.
5. Buscar cada expediente por matrícula normalizada y gabinete y verificarlo físicamente.
6. Crear únicamente los expedientes faltantes, incluidos primario y secundarios; verificarlos y registrar la caché de creación.
7. Persistir el plan `inscripción/tipología → expediente`.
8. Descargar, preparar y almacenar secuencialmente cada item SII; obtener cada `IdImagen`.
9. Consultar nuevamente todos los documentos del gabinete mediante `ENLASE = RadicadoSII` y deduplicar por `IdImagen`.
10. Construir y persistir el plan físico `IdImagen → expediente`, con exactamente un destino por documento.
11. Por cada documento, consultar la caché moderna y la relación física.
12. No revincular una relación correcta; vincular una ausente; detener una conflictiva o duplicada.
13. Registrar la caché documental solo después de verificar la relación correcta.
14. Actualizar `NITCEDULA`, `RAZONSOCIAL` y `MATRICULA` según gabinete.
15. Confirmar el índice electrónico SQL y actualizar obligatoriamente el archivo XML.
16. Reconciliar el universo documental completo.
17. Completar únicamente cuando todos los documentos estén reconciliados.
```

La primera implementación continúa procesando documentos secuencialmente.

## Fallos y recuperación

Implementar y probar estas reglas:

| Falla | Comportamiento requerido |
|---|---|
| Antes de crear expediente | Reintento sin efectos previos |
| Expediente creado y respuesta perdida | Consultar antes de crear; no duplicar |
| Expediente creado y documento fallido | Conservar expediente; reintentar documento |
| Documento almacenado y vinculación fallida | No almacenar nuevamente; reintentar vinculación |
| Vinculación confirmada e índices fallidos | Reintentar solo índices |
| Caché fallida | Reintentar caché sin recrear expediente ni documento |
| Índice SQL confirmado y XML fallido o desconocido | `ResultadoIncierto`; verificar ambos antes de reintentar |
| Segunda ejecución con nuevo `IdImagen` | Reutilizar expediente y efectos confirmados; procesar el documento nuevo |
| Efecto no demostrable | `ResultadoIncierto`; reconciliar antes de reintentar |
| Algunos items completados | Mantener confirmados y declarar intención `Parcial` |

No prometer una transacción distribuida entre SII, archivos, gabinete, expedientes y cachés.

## Reconciliación autoritativa

Extender `ReconcileImportIntent` para comprobar cada documento descubierto por `NombreGabinete + ENLASE = RadicadoSII`:

```text
documento existe exactamente una vez
AND expediente relacionado = expediente esperado
AND no existe relación con otro expediente
AND NITCEDULA corresponde al gabinete y sujeto SII
AND RAZONSOCIAL corresponde al gabinete y sujeto SII
AND MATRICULA corresponde a la regla del gabinete
AND caché apunta al mismo expediente
AND índice electrónico SQL es consistente
AND archivo XML es consistente
```

`IdTarea` es contexto persistido y forma parte de la caché documental, pero no exigir otra relación física de pertenencia: se confirmó que `ENLASE` es la única forma disponible de descubrir el universo documental.

Detectar y representar explícitamente:

- expediente ausente;
- relación ausente;
- relación duplicada;
- relación con otro expediente;
- expediente de otro gabinete;
- índices incompletos;
- caché ausente o contradictoria;
- expediente creado con documento pendiente;
- documento almacenado con resultado de vinculación desconocido.
- índice SQL presente con XML ausente o desactualizado;
- caché documental presente con relación física ausente;
- documento nuevo descubierto en una ejecución posterior.

No declarar un item `Disponible` utilizando solamente `DocumentId` y `ID_TAREA_WF`.

## Contratos de resultado

Agregar códigos seguros equivalentes a:

```text
EXPEDIENT_CONFIGURATION_UNAVAILABLE
EXPEDIENT_REQUIRED
EXPEDIENT_REUSED
EXPEDIENT_CREATED
EXPEDIENT_CREATION_FAILED
EXPEDIENT_CREATION_UNCERTAIN
EXPEDIENT_RELATION_MISSING
EXPEDIENT_RELATION_DUPLICATE
EXPEDIENT_RELATION_CONFLICT
EXPEDIENT_INDEX_UPDATE_FAILED
EXPEDIENT_CACHE_UPDATE_FAILED
RELATED_DOCUMENT_QUERY_FAILED
DOCUMENT_LINK_CACHE_CONFLICT
ELECTRONIC_INDEX_SQL_FAILED
ELECTRONIC_INDEX_XML_FAILED
ELECTRONIC_INDEX_RESULT_UNCERTAIN
```

No exponer mensajes SQL, rutas físicas, respuestas SII crudas, datos personales innecesarios, cookies, credenciales ni excepciones internas.

## Restricciones críticas

- Implementación paralela y aditiva; el gate apagado conserva íntegramente el recorrido legacy.
- No modificar `workflow/ClassAlmacenamiento.vb` ni `AlmacenaDocumentoTareaWorkflow(...)`.
- No modificar las funciones legacy de creación, vinculación, índices o caché dentro de esta entrega.
- No redirigir consumidores legacy hacia la implementación moderna.
- No invocar ASMX legacy por loopback HTTP.
- No duplicar la lógica legacy completa dentro de servicios modernos.
- No utilizar datos del frontend como autoridad de expediente.
- No utilizar la sesión como destino mutable.
- No procesar items en paralelo en esta primera versión.
- No reintentar efectos mutadores sin idempotencia y persistencia conocida.
- No marcar fases semánticas como exitosas cuando solo sean marcadores.
- No introducir DDL productivo sin migración versionada, rollback documentado y autorización correspondiente.
- Mantener .NET Framework 4.6.1 y compatibilidad con el proyecto VB existente.

## Pruebas obligatorias

### Caracterización legacy

- una prueba focal por cada función legacy declarada en la estrategia de migración, o una justificación verificable cuando varias funciones solo puedan caracterizarse como una unidad transaccional;
- expediente existente reutilizado;
- expediente único creado;
- expediente primario y secundarios;
- clasificación de documentos por tipología;
- cachés creadas y reutilizadas;
- retorno `YES` de vinculación cuando ya existe una relación y validación del expediente real;
- escritura de índice electrónico SQL y archivo XML;
- invariancia de archivos y funciones legacy.

### Unitarias y de integración local

- planificación de expediente único y múltiple;
- preservación inscripción-documentos;
- idempotencia de creación y relación;
- concurrencia de dos intentos sobre la misma identidad;
- fallo inyectado antes y después de cada efecto;
- continuación desde el último estado confirmado;
- transición inválida rechazada;
- índices y caché no confirmados sin efecto real;
- reconciliación de relación ausente, duplicada y cruzada.
- consulta del universo actual mediante `ENLASE` después del almacenamiento;
- segunda ejecución con el mismo radicado y un nuevo `IdImagen`;
- caché documental existente con relación física ausente o contradictoria;
- sello corregido conservando el anterior;
- reglas de matrícula para `MERCANTIL`, `ESAL` y `RUP`;
- índice SQL correcto con XML ausente o desactualizado;
- expediente secundario recuperado con la misma identidad usada al crearlo.

### E2E autorizada

Reutilizar y ampliar quirúrgicamente la plataforma E2E DOC-56 existente. No crear un runner, adaptador o escenario monolítico nuevo mientras los siguientes activos puedan expresar el comportamiento:

| Activo existente | Responsabilidad que debe conservar o ampliar |
|---|---|
| `scripts/run-workflow-e2e-platform.cjs` | CLI, autorizaciones, secretos efímeros, sesión autenticada, gate, TLS, ciclo del recurso y evidencia saneada |
| `scripts/adapters/importar-servicio-web-e2e-adapter.cjs` | Invocación real de `QueryItems`, `PreflightImport`, intención, ejecución, consulta y reconciliación |
| `import-sii-execution` | creación/reutilización, documentos seleccionados, vinculación, índices y reconciliación |
| `import-sii-retry` | continuación de una intención reintentable sin repetir efectos confirmados |
| `import-sii-recovery` | lectura y reconciliación de una intención existente sin mutación |
| `import-sii-concurrency` | exclusión concurrente e idempotencia de intención |
| `doc56-import-sii-execution.profile.example.json` | muestra de un documento |
| `doc56-import-sii-multidocument.profile.example.json` | varios items procesados secuencialmente dentro de `import-sii-execution` |
| registro y controles de `workflow-e2e-platform-registry.cjs` | huellas `SELECT` antes/después y expectativas de mutación |

Agregar únicamente campos de perfil, controles `SELECT`, verificadores y datos de evidencia que falten para expedientes, relaciones, caché documental e índices SQL/XML. Si `import-sii-retry` o `import-sii-recovery` requieren un perfil que aún no existe, crear solo ese perfil sobre el esquema y cargador vigentes. La ausencia de una aserción no autoriza duplicar escenario, autenticación, manejo del gate, transporte ASMX, ciclo de recursos ni escritura de reportes.

La suite DOC-56 reutilizada debe demostrar en conjunto:

1. reutilización de expediente existente;
2. creación de expediente único;
3. creación de primario/secundarios;
4. varios documentos en una intención;
5. cada documento relacionado con el expediente esperado;
6. reejecución sin duplicar expediente, documento ni relación;
7. estado parcial y recuperación;
8. segundo intento con el mismo radicado incorpora un nuevo `IdImagen` sin repetir relaciones anteriores;
9. sello corregido se conserva junto con el anterior;
10. `NITCEDULA`, `RAZONSOCIAL`, `MATRICULA`, índice SQL y XML quedan verificados;
11. gate restaurado al finalizar.

La prueba E2E de esta entrega es **obligatoria para cerrar la implementación**. Debe ejecutar selectivamente los escenarios DOC-56 existentes mediante `test:workflow:platform`, argumentos `--scenario`, `--profile` y sus autorizaciones vigentes. El perfil multidocumento existente sirve como entrada real de `import-sii-execution`; ampliar sus verificaciones no significa crear otro flujo de prueba.

```powershell
npm.cmd --prefix tools/e2e run test:workflow:platform -- --scenario import-sii-execution --profile doc56-import-sii-multidocument.profile.example.json --authorize environment,gate,execution,discardable-resource,local-tls
```

No ejecutar toda la matriz para cada cambio. Durante desarrollo, usar primero las pruebas Node focales del adaptador, perfil, registro y plataforma; reservar las corridas autenticadas para la validación autorizada final y ejecutar solo los escenarios que aporten evidencia distinta. No ejecutar directamente ASMX, clases internas o SQL mutador para simular el flujo.

Los escenarios existentes deben conducir y verificar, mediante la API y el recorrido moderno reales, estas 18 aserciones:

1. obtiene del proveedor una inscripción con múltiples documentos y conserva la asociación inscripción-documentos;
2. resuelve en backend la configuración vigente del trámite y no acepta del cliente una autoridad de expediente;
3. reutiliza el expediente cuando ya existe y satisface la identidad funcional comprobada;
4. crea idempotentemente un expediente cuando no existe y la configuración exige crearlo;
5. crea y distribuye entre expediente primario y secundarios cuando la configuración y las tipologías así lo exigen;
6. almacena todos los documentos esperados exactamente una vez, no solamente el primero;
7. vincula cada documento al expediente que le corresponde;
8. actualiza realmente `NITCEDULA`, `RAZONSOCIAL`, `MATRICULA`, índice electrónico SQL y XML, y registra las cachés requeridas;
9. confirma la secuencia de estados persistidos sin aceptar fases nominales o marcadores vacíos;
10. reconcilia intención, inscripción, documentos, tarea, expedientes, relaciones, índices y cachés antes de completar;
11. repite controladamente la misma intención y demuestra que no duplica expedientes, documentos, relaciones ni cachés;
12. inyecta un fallo recuperable después de al menos un efecto confirmado y demuestra continuación sin repetirlo;
13. rechaza una transición o relación inconsistente sin declarar el documento disponible;
14. mantiene sin cambios el recorrido legacy cuando el gate está apagado;
15. restaura `WorkflowCentroTrabajoModernActive=false`, usuarios vacíos y grupos vacíos, tanto en éxito como en fallo.
16. repite el radicado con un nuevo `IdImagen`, reutiliza el expediente, conserva los documentos anteriores y procesa únicamente la relación nueva;
17. demuestra que una caché por documento no sustituye la comprobación de la relación física;
18. conserva un sello anterior cuando se agrega uno corregido y reconcilia ambos.

Los perfiles reutilizados o mínimos perfiles adicionales deben declarar muestras descartables separadas para los casos de reutilización, creación única y creación primaria/secundaria. No copiar perfiles solo para cambiar datos: parametrizar o aportar muestras compatibles con el cargador existente. Si el ambiente no dispone de uno de esos casos, la suite debe reportarlo como bloqueo explícito y no convertir un caso distinto en evidencia equivalente.

El reporte debe incluir identificadores técnicos saneados, conteos esperados/observados, estados recorridos y el resultado de las consultas de reconciliación exclusivamente `SELECT`. Cada una de las 18 aserciones anteriores debe aparecer individualmente como `passed`, `failed` o `blocked`; un simple código de salida `0` no basta.

Si un recurso fue consumido, la muestra no contiene los datos requeridos, falta autorización o una dependencia externa impide ejecutarla, registrar el bloqueo y conseguir una nueva muestra descartable. La implementación permanece abierta mientras cualquier aserción obligatoria esté `failed` o `blocked`.

Crear o preparar E2E no autoriza ejecutarla. Antes de cualquier corrida autenticada leer `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`. Solicitar autorización explícita para ambiente, cuenta, mutación, tarea descartable, creación de expediente y concurrencia cuando corresponda.

Las consultas de verificación serán exclusivamente `SELECT`. La evidencia debe estar saneada y no incluir credenciales, cookies, tokens, cadenas de conexión, URLs SII firmadas ni cuerpos externos completos.

## Criterios de aceptación

- La decisión de crear o reutilizar expediente se toma únicamente en backend con configuración vigente.
- El modelo conserva la relación inscripción-documentos.
- Expediente único y expedientes múltiples reproducen las reglas legacy demostradas.
- Cada función legacy enumerada tiene disposición explícita (`migración directa`, `adaptador transitorio` o `reemplazo controlado`), trazabilidad al componente moderno y pruebas de caracterización/equivalencia.
- No existe una reimplementación paralela de reglas de estructura, identidad, clasificación, vinculación o índices sin demostrar su correspondencia con las funciones existentes.
- Después del almacenamiento se consultan todos los documentos mediante `NombreGabinete + ENLASE = RadicadoSII`.
- La caché documental impone `UNIQUE(task_id, image_id, cabinet_name)`, conserva el expediente esperado y nunca sustituye la verificación física.
- Una ejecución repetida no duplica expedientes, documentos, relaciones ni cachés.
- Una ejecución posterior con un `IdImagen` nuevo lo vincula e indexa sin repetir los documentos anteriores.
- Un sello corregido se conserva junto con el anterior.
- Cada item persiste su expediente esperado y el estado de cada efecto.
- Un documento no llega a `Disponible` sin haber sido descubierto por `NombreGabinete + ENLASE = RadicadoSII` y sin que su relación con el expediente esperado esté reconciliada.
- `NITCEDULA`, `RAZONSOCIAL` y `MATRICULA` respetan la regla de `MERCANTIL`, `ESAL` y `RUP`.
- El índice electrónico SQL y el archivo XML están verificados antes de completar.
- Un fallo parcial permite continuar sin repetir efectos confirmados.
- El gate apagado mantiene intacto el comportamiento legacy.
- `AlmacenaDocumentoTareaWorkflow(...)` y sus consumidores legacy permanecen sin cambios.
- Las pruebas focales, integración local y validaciones del proyecto pasan.
- La suite DOC-56 existente (`import-sii-execution`, `import-sii-retry`, `import-sii-recovery` e `import-sii-concurrency`, solo donde aplique) finaliza con sus 18 aserciones obligatorias aprobadas y conserva evidencia saneada; cualquier aserción pendiente, fallida o bloqueada impide cerrar la implementación.

## Ruta documental obligatoria

```text
Doc/Actualizacion/workflow/ImportarServicioWeb/SCRUMCORE-000-creacion-vinculacion-expedientes-sii/
```

Sustituir `SCRUMCORE-000` por el ticket real. Crear el paquete `00-Indice.md` a `07-Metadata.md`, `Diagramas/` y `Evidencias/` siguiendo la estructura documental vigente.

Documentar:

- radiografía legacy y sentencias/tablas afectadas;
- matriz función legacy → regla → destino moderno → pruebas → evidencia;
- decisiones cerradas de la compuerta de investigación;
- agregado de inscripción;
- contratos y puertos;
- modelo persistente y migraciones;
- máquina de estados;
- idempotencia y recuperación;
- reconciliación;
- seguridad y contexto;
- matriz requisito-prueba-evidencia;
- invariancia del recorrido legacy.

## Entregable final

Entregar como una sola unidad:

- código moderno;
- migraciones versionadas aplicables;
- pruebas de caracterización, unitarias e integración;
- E2E preparada y evidencia autorizada cuando exista autorización;
- documentación técnica y diagramas;
- lista exacta de archivos modificados;
- comandos y resultados reales;
- decisiones cerradas y riesgos residuales;
- inventario final de funciones migradas, adaptadas, reemplazadas y pendientes, sin agrupaciones genéricas;
- pruebas no ejecutadas y motivo;
- evidencia de gate restaurado;
- evidencia de que los archivos legacy protegidos no cambiaron.

No declarar la implementación completa mientras alguna fase siga siendo un marcador o la reconciliación no demuestre la relación documento-expediente.

## Correcciones opsxj:prompt-review

Las siguientes reglas de cierre incorporan los controles exigidos por `opsxj:prompt-review` y son obligatorias para esta implementación:

- Compilar la solución/proyectos ASP.NET y VB.NET afectados con MSBuild usando la configuración disponible en el repositorio. Ejecutar además la validación Node de `tools/e2e` cuando sus archivos cambien. Registrar comandos, versión de herramienta, código de salida y resultado; no sustituir la compilación .NET Framework 4.6.1 por `npm run build` o `tsc`.
- Entregar código, migraciones, E2E especializada, validación autorizada y evidencia saneada dentro del mismo cambio. Está prohibido diferir la E2E a otra tarea para declarar terminado este prompt.
- Reutilizar exclusivamente `tools/e2e`, incluyendo su runner de plataforma, autenticación, perfiles, validadores, reportes y utilidades. No crear login, arnés, proyecto Playwright, configuración de credenciales ni archivo `.env` paralelos.
- Recibir los secretos de forma efímera durante la corrida. No imprimir, registrar ni persistir credenciales, cookies, tokens, cadenas de conexión o URLs SII firmadas. Todas las verificaciones de base de datos deben ser `SELECT` y toda evidencia debe quedar saneada.
- Extender la suite DOC-56 existente para cubrir autenticación y autorización, resolución de contexto, lectura SII con múltiples items procesados secuencialmente, universo documental por `ENLASE`, escrituras autorizadas, creación/reutilización/vinculación, caché por documento, índices SQL/XML, idempotencia incremental, recuperación, reconciliación y regresión del recorrido legacy. Distribuir las 18 aserciones entre `import-sii-execution`, `import-sii-retry`, `import-sii-recovery` e `import-sii-concurrency`; no crear `import-sii-expedient-execution` ni repetir una corrida cuando otra evidencia existente ya demuestra la misma propiedad.
- Respetar feature flags, gates, usuarios, grupos y controles de seguridad. Solo activar temporalmente el gate con autorización explícita y restaurarlo en un bloque de cierre aun ante error. No cerrar la entrega sin validación autorizada; registrar como bloqueo cualquier prueba pendiente y prohibir mocks, simulaciones, resultados inventados o evidencia ficticia.
