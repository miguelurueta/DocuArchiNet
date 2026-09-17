<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09,D-10,D-11,D-12,D-13,D-14,D-15 -->
## Context

DOC-67 extiende el flujo moderno de `ImportarServicioWeb` construido por los prompts backend 01–07. El flujo actual consulta SII, persiste una intención, procesa items secuencialmente y almacena documentos mediante `LegacyImportDocumentStorageAdapter`, pero no coordina explícitamente expedientes por inscripción, no demuestra la relación documento–expediente y mantiene pasos nominales para índices/caché.

La capacidad equivalente existe distribuida entre JavaScript, ASMX y clases VB legacy. Sus funciones contienen reglas válidas y también dependencias de sesión, retornos ambiguos, ausencia de unicidad suficiente y efectos SQL/XML no atómicos. La modernización será aditiva bajo gate; véanse `proposal.md`, `refinement.md`, el prompt backend 08 y su exploración normativa.

Restricciones determinantes:

- ASP.NET Web Forms/ASMX y VB.NET sobre .NET Framework 4.6.1.
- MySQL, almacenamiento físico, índice electrónico SQL y XML sin transacción distribuida.
- `workflow/ClassAlmacenamiento.vb`, `AlmacenaDocumentoTareaWorkflow(...)` y consumidores legacy protegidos no se modifican.
- El contexto persistido gobierna; frontend y sesión no deciden expediente.
- Ejecución secuencial documento por documento.
- `ENLASE` es la única pertenencia documental demostrable disponible.

## Goals / Non-Goals

**Goals:**

- Coordinar resolución/creación de expedientes a nivel de intención e inscripción.
- Preservar el agregado inscripción–documentos y soportar expediente único, primario y secundarios.
- Procesar incrementalmente todo `IdImagen` descubierto por gabinete y `ENLASE`.
- Hacer idempotentes y reconciliables creación, vínculo, caché, índices SQL y XML.
- Migrar las reglas legacy comprobadas detrás de contratos modernos con trazabilidad por función.
- Reutilizar la plataforma E2E DOC-56 existente.

**Non-Goals:**

- Reescribir o retirar el recorrido legacy.
- Alterar `ClassAlmacenamiento` o redirigir sus consumidores.
- Crear un ASMX interno, runner E2E, autenticación, gate o reporte paralelo.
- Introducir paralelismo/multiplex, compensación destructiva de expedientes o una transacción distribuida ficticia.
- Codificar una lista universal de campos únicos o aceptar el expediente enviado por cliente.

## Architecture

```text
API/ASMX moderno
  ↓ contexto autorizado e inmutable
ImportServiceOrchestrator
  ├─ agregado de inscripciones
  ├─ ImportExpedientCoordinator
  │    ├─ configuración/identidad
  │    ├─ búsqueda/creación/verificación
  │    └─ plan inscripción-tipología → expediente
  ├─ almacenamiento secuencial → IdImagen
  ├─ ImportRelatedDocumentCoordinator
  │    ├─ consulta gabinete + ENLASE
  │    ├─ plan físico IdImagen → expediente
  │    ├─ relación + caché
  │    └─ índices SQL/XML
  └─ ServicioReconciliacionImportacion
       └─ postcondiciones autoritativas
```

Los servicios de aplicación dependen de puertos modernos. Adaptadores de infraestructura encapsulan llamadas directas a clases legacy; nunca se invoca ASMX legacy por loopback HTTP.

Puertos previstos, sujetos a las convenciones comprobadas del repositorio:

- `IImportExpedientConfigurationRepository`
- `ISiiExpedientSubjectResolver`
- `IImportExpedientRepository`
- `IImportDocumentExpedientRelationPort`
- `IImportExpedientCacheRepository`
- `IImportRelatedDocumentRepository`
- `IImportDocumentLinkCacheRepository`
- `IImportDocumentIndexUpdater`
- `IImportElectronicIndexVerifier`

## Decisions

### D-01 — Expediente como precondición obligatoria

El coordinador busca y verifica un expediente existente o crea solamente el faltante antes de almacenar items. Si la configuración, identidad, creación o verificación no puede resolverse, la intención no avanza al almacenamiento.

La matrícula física es un entero positivo canónico. El normalizador compartido elimina letras y demás caracteres no numéricos, quita ceros iniciales y rechaza entradas sin dígitos significativos. Su resultado se reutiliza sin divergencias en localización, materialización de campos únicos, creación legacy, caché e índice documental.

Alternativa descartada: almacenar primero y “completar después” el expediente. Deja documentos disponibles sin destino obligatorio y dificulta recuperación segura.

### D-02 — Coordinación agregada por intención e inscripción

Se introduce un agregado interno que conserva `inscriptionKey`, libro, registro, matrícula/proponente, identificación, razón social, propietario y documentos. `ImportExpedientCoordinator` trabaja una vez por intención/grupo de inscripción y persiste el plan lógico antes de almacenar.

Alternativa descartada: ejecutar creación por imagen. Duplica consultas/efectos y pierde la semántica primario/secundario.

### D-03 — Dos niveles de planificación

Antes del almacenamiento se persiste `inscripción/tipología → expediente`. Después, con los identificadores físicos disponibles, se construye `IdImagen → expediente`. Tanto en modo único como múltiple cada imagen debe tener exactamente un destino; ambigüedad o ausencia bloquea el documento.

Alternativa descartada: considerar definitivo el plan previo. No incluye nuevos `IdImagen` ni documentos ya existentes descubiertos por enlace.

### D-04 — Localización funcional separada de identidad física

La localización SII usa matrícula normalizada más gabinete. La confirmación del expediente compara dinámicamente todos los campos configurados con `estado_unico=1`. La normalización mantiene reglas distintas para MERCANTIL, ESAL y RUP y debe ser simétrica en creación/recuperación, incluidos secundarios.

Alternativa descartada: identidad fija codificada o caché como autoridad. La configuración cambia por trámite/gabinete y el legacy no ofrece unicidad suficiente.

### D-05 — Universo documental posterior por ENLASE

Tras almacenar los items seleccionados, `IImportRelatedDocumentRepository` consulta nuevamente `NombreGabinete + ENLASE = RadicadoSII` y deduplica por `IdImagen`. Esa consulta constituye el universo a planificar y reconciliar; `IdTarea` permanece como contexto/caché, no como relación física adicional inventada.

Alternativa descartada: procesar únicamente los items recién almacenados. Omite documentos previos y correcciones posteriores.

### D-06 — Correcciones aditivas

Un sello corregido obtiene otro `IdImagen`; no anula ni sustituye físicamente el anterior. Una nueva ejecución reutiliza expediente y efectos confirmados y procesa únicamente documentos nuevos o pendientes.

Alternativa descartada: borrar o marcar sustituido el sello previo, porque la decisión funcional exige conservar ambos.

### D-07 — Caché documental como diario, no autoridad

La persistencia moderna impone unicidad por `(task_id,image_id,cabinet_name)` y conserva `expected_expedient_id`, `sii_radicado`, estado y fechas de verificación. Toda lectura de caché se contrasta con la relación física; una discrepancia es conflicto y no se sobrescribe silenciosamente. Se escribe solo después de verificar el vínculo.

Alternativa descartada: reutilizar la caché global por radicado. Puede ocultar documentos agregados en ejecuciones posteriores.

### D-08 — Vinculación con precheck y postcheck

Por documento se consulta primero la relación física. Una relación correcta se conserva, una ausente se crea mediante adaptador y se verifica, y una duplicada/cruzada detiene la finalización con código seguro. El retorno textual `YES` nunca confirma por sí solo el destino.

Alternativa descartada: invocar siempre el mutador o confiar en su texto de retorno; ambos permiten dobles relaciones o falsos positivos.

### D-09 — Índices reales y finalización SQL/XML

`IImportDocumentIndexUpdater` materializa `NITCEDULA`, `RAZONSOCIAL` y `MATRICULA` según gabinete. `IImportElectronicIndexVerifier` comprueba de forma independiente índice electrónico SQL y archivo XML. `IndicesYXmlActualizados` solo se confirma si ambos efectos coinciden.

Alternativa descartada: conservar `PrepareImportIndicesExecutionStep` como marcador o aceptar SQL sin XML.

### D-10 — Saga persistente y ResultadoIncierto

No se intenta una transacción global. Después de cada efecto se verifica su postcondición y se persiste el estado. Un reintento continúa desde el último efecto demostrado. Expedientes ya creados no se compensan destructivamente. Una respuesta perdida o discrepancia no demostrable produce `ResultadoIncierto` y reconciliación antes de mutar nuevamente.

Estados de intención:

```text
Creada → Validada → ExpedientesPlanificados → ExpedientesResueltos
→ ItemsSiiAlmacenados → UniversoDocumentalConsultado
→ VinculacionesProcesadas → IndicesYXmlActualizados
→ Reconciliada → Completada
```

Se conservan estados por item para descarga/preparación/almacenamiento y por documento relacionado para descubrimiento, destino, consulta de relación, vínculo, índices, XML y reconciliación. `Parcial` representa resultados mixtos confirmados.

### D-11 — Migración gobernada por función legacy

Cada función enumerada en `refinement.md` debe registrar archivo/llamadores, entradas explícitas e implícitas, SQL/tablas/archivos, efectos, defecto conocido, disposición, componente moderno, caracterización y prueba equivalente.

Orden:

1. migrar lecturas/reglas deterministas (configuración, identidad, ENLASE, clasificación);
2. encapsular temporalmente mutaciones complejas (creación, vínculo, SQL/XML);
3. reemplazar controles insuficientes (caché global) manteniendo compatibilidad demostrada;
4. migrar progresivamente adaptadores cuando exista equivalencia comprobada.

Alternativas descartadas: copiar cuerpos completos, llamarlos ciegamente o reimplementar desde cero. Las tres pierden control sobre sesión, efectos laterales o reglas duplicadas.

### D-12 — Extensión mínima de E2E DOC-56

Las 18 aserciones se distribuyen entre `import-sii-execution`, `import-sii-retry`, `import-sii-recovery` e `import-sii-concurrency`. Se reutilizan runner, adaptador, perfiles, sesión, gate, TLS, ciclo de recursos, controles y evidencia. Solo se agregan campos/controles/verificadores faltantes para expedientes, vínculos, caché e índices SQL/XML.

Alternativa descartada: `import-sii-expedient-execution` o infraestructura paralela; aumenta costo y duplica seguridad ya resuelta.

### D-13 — Contexto explícito e inmutable

Tarea, ruta, gabinete, empresa y actor se resuelven en servidor y se persisten con la intención. Los reintentos usan ese contexto; la sesión valida que el actor siga autorizado. Si un adaptador legacy requiere sesión, compara sus valores con el contexto y nunca altera sesión para fabricar coincidencia.

Alternativa descartada: usar `ID_TAREA_SELECCIONDA` u otras globales como autoridad mutable.

### D-14 — Convivencia aditiva bajo gate

El recorrido moderno se habilita con el gate vigente y el fallback legacy permanece intacto. No se modifican funciones/consumidores protegidos. Estados, DTO, fixtures y mapeos se versionan cuando cambie su semántica. El gate debe restaurarse a `false`, con usuarios y grupos vacíos, incluso ante fallo de E2E.

Alternativa descartada: redirigir consumidores legacy al coordinador nuevo; amplía el radio de regresión y puede duplicar operaciones.

### D-15 — Consulta moderna de sujeto con fallback legacy conservado

`ModernSiiExpedientSubjectResolver` es la ruta primaria del flujo moderno. Reutiliza `SiiExternalImportProviderClient` y `ExternalImportHttpTransport` para solicitar token y consultar `consultarExpedienteMercantil` o `consultarExpedienteProponente` con TLS del sistema, timeout por solicitud, cancelación, UTF-8, límite de cuerpo, contrato JSON tipado, telemetría y códigos seguros. No instala callbacks TLS globales ni depende de `HttpContext.Session`.

La identidad ESAL se deriva eliminando caracteres no numéricos, retirando el prefijo histórico `9000` únicamente si ocupa el inicio y anteponiendo exactamente una vez `S0`. La respuesta solo se acepta con NIT y razón social; luego se aplica el normalizador compartido y la materialización dinámica de campos únicos.

La función legacy `SolicitaEstructuraExpedienteSII` y `ConsultaExpedienteMercantilEsal` permanecen intactas. `LegacySiiExpedientSubjectResolver` se inyecta como fallback configurable mediante `ImportarServicioWebSiiSubjectLegacyFallback`, inicialmente habilitado para rollout y desactivable sin cambiar binarios. Sus demás consumidores no se redirigen.

Alternativa descartada: modificar el helper REST legacy. Sus callbacks TLS globales, sesión, protocolo textual y numerosos consumidores ampliarían el radio de regresión.

## Persistence and consistency

El modelo lógico añade o amplía:

```text
workflow_import_inscription
  intent_id, inscription_key, datos SII,
  expedient_id, expedient_role, expedient_status, cache_status

workflow_import_intent_item
  inscription_key, document_id, expedient_id,
  storage_status, relation_status, index_status, cache_status

workflow_import_related_document
  intent_id, task_id, image_id, cabinet_name, sii_radicado,
  expected_expedient_id, relation_status, index_status,
  xml_index_status, reconciliation_status

workflow_import_document_link_cache
  task_id, image_id, cabinet_name, expected_expedient_id,
  sii_radicado, relation_status, created_utc, verified_utc
  UNIQUE(task_id,image_id,cabinet_name)
```

Los nombres físicos finales deben respetar convenciones/migraciones existentes. Todo DDL se entrega como migración versionada con rollback. La unicidad se implementa en persistencia o mediante escritura condicionada autoritativa demostrable; un `If` en memoria no basta.

## Reconciliation contract

Por cada documento descubierto mediante gabinete+`ENLASE`, la reconciliación confirma:

```text
documento existe exactamente una vez
AND expediente relacionado = esperado
AND no hay relación con otro expediente
AND NITCEDULA, RAZONSOCIAL y MATRICULA corresponden
AND caché apunta al expediente esperado
AND índice electrónico SQL es consistente
AND archivo XML es consistente
```

Los resultados seguros distinguen configuración/expediente ausente, creación fallida o incierta, relación ausente/duplicada/conflictiva, consulta documental fallida, conflicto de caché e índice SQL/XML fallido o incierto. Nunca exponen SQL, rutas, respuestas SII crudas, secretos ni excepciones internas.

## Risks / Trade-offs

- [Efectos legacy no caracterizados] → bloquear la mutación afectada hasta documentar SQL, archivos, retornos y postcondiciones.
- [Dependencia de `HttpContext.Session`] → puertos libres de sesión y adaptador que contrasta contexto persistido.
- [Creación y respuesta perdida] → búsqueda por identidad/caché y verificación física antes de recrear.
- [Relación `YES` ambigua] → precheck/postcheck autoritativos.
- [SQL confirmado y XML fallido] → estados separados, `ResultadoIncierto` y reconciliación.
- [Reglas distintas de matrícula secundaria] → un normalizador compartido para creación y recuperación, probado por gabinete.
- [Sin unicidad legacy] → índice único moderno y manejo explícito de conflicto concurrente.
- [E2E costosa] → pruebas Node focales durante desarrollo y escenarios autenticados solo para evidencia distinta y autorizada.
- [Cambio semántico de estados] → versionar contratos/fixtures y conservar compatibilidad con consumidores existentes.

## Migration Plan

1. Completar caracterización y matriz por función antes de código mutador.
2. Introducir migraciones persistentes versionadas y sus rollbacks.
3. Extender modelos, DTO y repositorios conservando compatibilidad.
4. Implementar coordinador de expedientes y plan lógico bajo gate.
5. Mantener almacenamiento secuencial existente y capturar `IdImagen`.
6. Implementar descubrimiento por `ENLASE`, plan físico y vinculación verificada.
7. Implementar caché documental e índices SQL/XML con postcondiciones.
8. Extender reconciliación y recuperación de la saga.
9. Ejecutar caracterización, unitarias, integración y compilación.
10. Extender/reutilizar E2E DOC-56; ejecutar únicamente con autorización y evidencia saneada.
11. Desplegar inicialmente con gate apagado; habilitar alcance controlado y observar reconciliación.
12. Observar la consulta moderna de sujeto, retirar progresivamente el fallback configurable y conservar el legacy para el recorrido con gate apagado.

Rollback:

- Deshabilitar el gate restaura el recorrido legacy sin borrar expedientes/documentos ya confirmados.
- Revertir binarios y contratos modernos compatibles.
- Aplicar rollback versionado de nuevas tablas/índices solo después de conservar/exportar evidencia necesaria y confirmar que no hay ejecución activa.
- No borrar automáticamente expedientes, relaciones o documentos como compensación.
