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
5. asigne cada documento al expediente correcto;
6. almacene y vincule cada documento sin duplicar efectos;
7. actualice realmente sus índices SII;
8. registre las cachés de creación y vinculación;
9. reconcilie documento, tarea, expediente, índices y caché antes de declarar el item disponible y la intención completada.

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

La exploración indicada es contexto normativo de esta entrega: contiene el diagnóstico comprobado, las brechas, el diagrama de flujo propuesto, riesgos heredados y decisiones pendientes. No implementar una versión simplificada que ignore esas conclusiones.

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

## Compuerta de investigación obligatoria

Antes de escribir código mutador, documentar con archivo, función, sentencia o tabla la respuesta a estas preguntas:

1. ¿Qué tablas y registros modifica `AutoRegistraExpedienteTramite` directa e indirectamente?
2. ¿Qué tablas y registros modifica `VinculaDocumentoExpediente`?
3. ¿Qué tablas modifican las cachés de creación, vinculación e inscripción SII?
4. ¿Qué claves primarias, índices únicos o validaciones funcionales evitan duplicar expedientes, relaciones y cachés?
5. ¿Qué condición exacta define que un expediente existente corresponde a matrícula, proponente, gabinete y trámite?
6. ¿Qué documentos exige `CreaExpedienteIntegracionSII` que existan antes de crear el expediente?
7. ¿Los sellos importados deben vincularse al expediente o solamente los documentos previamente asociados al trámite?
8. ¿Qué regla de negocio aplica cuando el trámite exige expediente y no es posible resolverlo o crearlo?
9. ¿Cómo se distribuyen las tipologías entre expediente primario y expedientes secundarios?
10. ¿Qué efectos son reintentables y cuáles requieren reconciliación previa?

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
└── ServicioReconciliacionImportacion.vb

Infrastructure/Workflow/ImportarServicioWeb/Expedients/
├── LegacyImportExpedientAdapter.vb
├── LegacyImportExpedientRelationAdapter.vb
└── LegacyImportDocumentIndexAdapter.vb

Infrastructure/Repositories/Workflow/ImportarServicioWeb/
├── MySqlImportExpedientConfigurationRepository.vb
├── MySqlImportExpedientCacheRepository.vb
├── MySqlImportIntentRepository.vb
└── MySqlImportReconciliationRepository.vb

Infrastructure/Workflow/ImportarServicioWeb/Sii/
└── SiiImportContractMapper.vb

Tests/
├── importar-servicio-web-expedient-configuration.test.cjs
├── importar-servicio-web-expedient-planning.test.cjs
├── importar-servicio-web-expedient-creation.test.cjs
├── importar-servicio-web-expedient-relation.test.cjs
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
- persista un plan `inscripción/documento → expediente` antes de almacenar documentos.

### Puertos modernos

Definir interfaces modernas equivalentes a:

```text
IImportExpedientConfigurationRepository
ISiiExpedientSubjectResolver
IImportExpedientRepository
IImportDocumentExpedientRelationPort
IImportExpedientCacheRepository
IImportDocumentIndexUpdater
```

Los servicios modernos dependen de estas interfaces. Las referencias a clases legacy quedan confinadas a adaptadores de infraestructura.

No invocar ASMX legacy mediante HTTP interno.

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
```

No adoptar literalmente estos nombres sin revisar las convenciones y migraciones actuales. Documentar la decisión final.

Proteger como identidades idempotentes:

```text
matrícula/proponente + gabinete + trámite → expediente
documentId + expedientId → relación
radicado + matrícula + gabinete + expedientId → caché
```

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
RecursoObtenido
  ↓
ArchivoTemporalPreparado
  ↓
DocumentoAlmacenado
  ↓
DocumentoVinculado
  ↓
IndicesActualizados
  ↓
CacheActualizado
  ↓
Reconciliada
  ↓
Completada
```

- `ExpedientePreparado` no puede continuar significando solamente “archivo temporal escrito”. Renombrar o redefinir mediante una migración contractual compatible.
- `IndicesActualizados` solo se confirma después de una actualización real.
- `CacheActualizado` solo se confirma después de verificar las cachés requeridas.
- `Completada` solo se alcanza después de reconciliación autoritativa.
- Todo estado nuevo debe versionar DTO, fixture, mapeo frontend/backend y documentación.

## Secuencia funcional

Implementar la siguiente secuencia, ajustada únicamente cuando la investigación demuestre una dependencia legacy diferente:

```text
1. Consultar SII preservando inscripciones e imágenes.
2. Validar selección, contexto y tipologías.
3. Crear intención idempotente.
4. Resolver configuración de expediente del trámite.
5. Buscar expediente y cachés existentes.
6. Crear de forma idempotente los expedientes faltantes.
7. Persistir el plan de asignación.
8. Descargar y preparar cada documento secuencialmente.
9. Almacenar el documento y obtener DocumentId.
10. Vincular DocumentId al expediente planificado.
11. Actualizar índices SII reales.
12. Registrar cachés de creación, vinculación e inscripción.
13. Reconciliar todos los efectos.
14. Completar cada item y agregar el estado de la intención.
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
| Efecto no demostrable | `ResultadoIncierto`; reconciliar antes de reintentar |
| Algunos items completados | Mantener confirmados y declarar intención `Parcial` |

No prometer una transacción distribuida entre SII, archivos, gabinete, expedientes y cachés.

## Reconciliación autoritativa

Extender `ReconcileImportIntent` para comprobar por item:

```text
documento existe exactamente una vez
AND tarea relacionada = tarea esperada
AND expediente relacionado = expediente esperado
AND no existe relación con otro expediente
AND índices corresponden a la inscripción
AND caché apunta al mismo expediente
```

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

- trámite sin creación de expediente;
- expediente existente reutilizado;
- expediente único creado;
- expediente primario y secundarios;
- clasificación de documentos por tipología;
- cachés creadas y reutilizadas;
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

### E2E autorizada

Preparar en `tools/e2e` escenarios que demuestren:

1. reutilización de expediente existente;
2. creación de expediente único;
3. creación de primario/secundarios;
4. varios documentos en una intención;
5. cada documento relacionado con el expediente esperado;
6. reejecución sin duplicar expediente, documento ni relación;
7. estado parcial y recuperación;
8. gate restaurado al finalizar.

La prueba E2E de esta entrega es **obligatoria para cerrar la implementación**. Debe reutilizar únicamente la estructura técnica del runner existente (`test:workflow:platform`, argumentos `--scenario`, `--profile` y autorizaciones); el escenario y el perfil deben especializarse para validar lo implementado por este prompt. La antigua prueba multidocumento sirve como base técnica, pero su resultado no constituye por sí solo evidencia de creación y vinculación de expedientes.

```powershell
npm.cmd --prefix tools/e2e run test:workflow:platform -- --scenario import-sii-expedient-execution --profile doc56-import-sii-expedient-execution.profile.example.json --authorize environment,gate,execution,discardable-resource,local-tls
```

Implementar `import-sii-expedient-execution` sobre la infraestructura del escenario `import-sii-execution`, reutilizando su arranque de plataforma, autenticación, activación/restauración del gate, manejo TLS, captura de evidencia y consultas MySQL de solo lectura. No crear un segundo runner ni ejecutar directamente ASMX, clases internas o SQL mutador para simular el flujo.

El escenario especializado y su perfil deben conducir y verificar, mediante la API y el recorrido moderno reales, todos estos comportamientos:

1. obtiene del proveedor una inscripción con múltiples documentos y conserva la asociación inscripción-documentos;
2. resuelve en backend la configuración vigente del trámite y no acepta del cliente una autoridad de expediente;
3. reutiliza el expediente cuando ya existe y satisface la identidad funcional comprobada;
4. crea idempotentemente un expediente cuando no existe y la configuración exige crearlo;
5. crea y distribuye entre expediente primario y secundarios cuando la configuración y las tipologías así lo exigen;
6. almacena todos los documentos esperados exactamente una vez, no solamente el primero;
7. vincula cada documento al expediente que le corresponde;
8. actualiza realmente los índices SII y las cachés de creación/vinculación requeridas;
9. confirma la secuencia de estados persistidos sin aceptar fases nominales o marcadores vacíos;
10. reconcilia intención, inscripción, documentos, tarea, expedientes, relaciones, índices y cachés antes de completar;
11. repite controladamente la misma intención y demuestra que no duplica expedientes, documentos, relaciones ni cachés;
12. inyecta un fallo recuperable después de al menos un efecto confirmado y demuestra continuación sin repetirlo;
13. rechaza una transición o relación inconsistente sin declarar el documento disponible;
14. mantiene sin cambios el recorrido legacy cuando el gate está apagado;
15. restaura `WorkflowCentroTrabajoModernActive=false`, usuarios vacíos y grupos vacíos, tanto en éxito como en fallo.

El perfil debe declarar muestras descartables separadas para los casos de reutilización, creación única y creación primaria/secundaria. Si el ambiente no dispone de uno de esos casos, el escenario debe reportarlo como bloqueo explícito y no convertir un caso distinto en evidencia equivalente.

El reporte debe incluir identificadores técnicos saneados, conteos esperados/observados, estados recorridos y el resultado de las consultas de reconciliación exclusivamente `SELECT`. Cada aserción anterior debe aparecer individualmente como `passed`, `failed` o `blocked`; un simple código de salida `0` no basta.

Si un recurso fue consumido, la muestra no contiene los datos requeridos, falta autorización o una dependencia externa impide ejecutarla, registrar el bloqueo y conseguir una nueva muestra descartable. La implementación permanece abierta mientras cualquier aserción obligatoria esté `failed` o `blocked`.

Crear o preparar E2E no autoriza ejecutarla. Antes de cualquier corrida autenticada leer `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`. Solicitar autorización explícita para ambiente, cuenta, mutación, tarea descartable, creación de expediente y concurrencia cuando corresponda.

Las consultas de verificación serán exclusivamente `SELECT`. La evidencia debe estar saneada y no incluir credenciales, cookies, tokens, cadenas de conexión, URLs SII firmadas ni cuerpos externos completos.

## Criterios de aceptación

- La decisión de crear o reutilizar expediente se toma únicamente en backend con configuración vigente.
- El modelo conserva la relación inscripción-documentos.
- Expediente único y expedientes múltiples reproducen las reglas legacy demostradas.
- Una ejecución repetida no duplica expedientes, documentos, relaciones ni cachés.
- Cada item persiste su expediente esperado y el estado de cada efecto.
- Un documento no llega a `Disponible` sin relación de tarea y expediente reconciliadas cuando el trámite exige expediente.
- `IndicesActualizados` y `CacheActualizado` corresponden a operaciones reales verificadas.
- Un fallo parcial permite continuar sin repetir efectos confirmados.
- El gate apagado mantiene intacto el comportamiento legacy.
- `AlmacenaDocumentoTareaWorkflow(...)` y sus consumidores legacy permanecen sin cambios.
- Las pruebas focales, integración local y validaciones del proyecto pasan.
- La E2E especializada `import-sii-expedient-execution` finaliza con sus 15 aserciones obligatorias aprobadas y conserva evidencia saneada; cualquier aserción pendiente, fallida o bloqueada impide cerrar la implementación.

## Ruta documental obligatoria

```text
docs/Architecture/Workflow/ImportarServicioWeb/SCRUMCORE-000-creacion-vinculacion-expedientes-sii/
```

Sustituir `SCRUMCORE-000` por el ticket real. Crear el paquete `00-Indice.md` a `07-Metadata.md`, `Diagramas/` y `Evidencias/` siguiendo la estructura documental vigente.

Documentar:

- radiografía legacy y sentencias/tablas afectadas;
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
- Cubrir en `import-sii-expedient-execution` autenticación y autorización, resolución de contexto, lectura SII multidocumento, escrituras autorizadas, creación/reutilización/vinculación, idempotencia, recuperación, reconciliación y regresión del recorrido legacy. La cobertura se demuestra con las 15 aserciones definidas en este prompt.
- Respetar feature flags, gates, usuarios, grupos y controles de seguridad. Solo activar temporalmente el gate con autorización explícita y restaurarlo en un bloque de cierre aun ante error. No cerrar la entrega sin validación autorizada; registrar como bloqueo cualquier prueba pendiente y prohibir mocks, simulaciones, resultados inventados o evidencia ficticia.
