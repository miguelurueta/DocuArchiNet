# DOC-67 — Auditoría de confiabilidad de funciones legacy

Fecha de corte: 2026-09-16.

## Dictamen ejecutivo

Ninguna de las quince funciones legacy inventariadas debe operar como autoridad definitiva del flujo moderno. Esto no significa que todas deban reescribirse nuevamente: diez ya tienen un reemplazo moderno suficiente, una posee reemplazo moderno primario con fallback temporal y cuatro todavía requieren migración física obligatoria.

La función original debe conservarse para el recorrido con el gate apagado hasta completar rollout y rollback. “Conservar” no equivale a “seguir invocando desde la ruta moderna”.

## Criterios de aceptación misional

Una implementación definitiva debe cumplir simultáneamente:

1. autoridad calculada en backend, sin confiar en sesión o datos del cliente para gabinete, tarea o expediente;
2. SQL parametrizado y nombres dinámicos limitados por lista/configuración validada;
3. idempotencia persistente y comportamiento determinista ante concurrencia;
4. precondición y postcondición autoritativas, sin interpretar `YES` como evidencia suficiente;
5. atomicidad o saga recuperable para efectos SQL, relaciones y XML;
6. códigos tipados y seguros, sin excepciones o SQL expuestos;
7. tratamiento explícito de cero, uno y múltiples resultados;
8. observabilidad, timeout y reconciliación por cada efecto físico.

## Evaluación individual

| # | Función | Confiabilidad legacy | Riesgo determinante | Dictamen definitivo | Estado moderno |
|---:|---|---|---|---|---|
| 1 | `SolicitaEstructuraTramite` | Baja como dependencia moderna | SQL concatenado, DTO de 38 campos, protocolo textual y ausencia/conflicto no tipados. Aunque el id es entero, el contrato sigue siendo frágil y sobredimensionado. | **No reutilizar en moderno.** La migración necesaria ya está hecha. | `MySqlImportExpedientConfigurationRepository.Obtener` consulta únicamente configuración requerida, con parámetros y resultado tipado. |
| 2 | `SolicitaRegistroExpedienteMatricula` | Crítica | Decide usando solo `Item(0)`, mezcla consulta de sujeto y caché, propaga textos y no valida lista vacía. No soporta correctamente el agregado multiinscripción. | **Retirar de la ruta moderna.** La migración del orquestador ya está hecha. | `ImportExpedientCoordinator.Resolver` procesa cada inscripción y exige expediente verificado. |
| 3 | `SolicitaCacheCreacionExpedienteSII` | Crítica | Consulta concatenada por matrícula/gabinete, toma `Rows(0)` sin orden ni unicidad y devuelve `YES` también cuando no encuentra datos. Puede ocultar duplicados o escoger un expediente arbitrario. | **Migración física obligatoria.** No es aceptable como localizador definitivo. | Todavía es llamada por `LegacyPhysicalExpedientGateway` y `LegacyImportExpedientCacheRepository`. |
| 4 | `SolicitaEstructuraExpedienteSII` | Baja–media | Transporte y errores legacy, normalizaciones divergentes por gabinete y defecto RUP en campos de propietario. Su resultado depende de contratos externos poco tipados. | **No usar como primario.** Mantener solo fallback temporal hasta E2E completa de los tres registros. | `ModernSiiExpedientSubjectResolver` y `SiiExternalImportProviderClient` son primarios; fallback configurable. |
| 5 | `AutoRegistraExpedienteTramite` | Crítica | Usa sesión implícita, SQL/campos dinámicos, múltiples tablas y XML; el commit no está seguido por verificación autoritativa y una respuesta perdida puede duplicar o dejar efectos parciales. | **Migración física obligatoria y prioritaria.** | Aún se invoca en `LegacyPhysicalExpedientGateway.Crear`; el repositorio moderno mitiga con pre/postcheck, pero no elimina el riesgo interno. |
| 6 | `CreaExpedienteIntegracionSII` | Crítica | Macro-orquestador acoplado a sesión, `Item(0)`, cachés y bucles inscripción×imagen sin deduplicación ni garantía de destino único. | **No reutilizar.** Su migración funcional ya está hecha. | Reemplazado por `ImportExpedientCoordinator`, `ImportExpedientPlan` e `ImportRelatedDocumentPlan`. |
| 7 | `SolicitaListaImagenesGabineteEnlace` | Baja | Concatena tabla y `ENLASE`, no ordena, no deduplica y representa cero filas como salida implícita. Riesgo de inyección y universo ambiguo. | **No reutilizar.** Migración terminada. | `MySqlImportRelatedDocumentRepository.ObtenerPorEnlace` valida gabinete, parametriza valor, ordena y deduplica. |
| 8 | `SolicitaDocumentosTareaWorkflowVinculacionUnicoExpedientesSII` | Media–baja | Hereda descubrimiento inseguro; el plan no persiste evidencia fuerte ni protege por sí mismo duplicidad/concurrencia. | **No reutilizar.** Migración terminada. | `ImportRelatedDocumentPlan` asigna exactamente un destino por `IdImagen`. |
| 9 | `SolicitaDocumentosTareaWorkflowVinculacionMultipleExpedientesSII` | Crítica | Depende de caché como autoridad, normalización ESAL asimétrica y bucles que pueden generar destinos repetidos o cruzados. | **No reutilizar.** Migración terminada. | Planificadores modernos combinan inscripción, tipología y destino único, rechazando ambigüedad. |
| 10 | `VinculaDocumentoExpediente` | Crítica | Si ya existe relación devuelve `YES` sin verificar el expediente solicitado; coordina varias escrituras y XML con ventanas de inconsistencia y sesión implícita. No distingue relación correcta, cruzada o duplicada. | **Migración física obligatoria y prioritaria.** | `DocumentExpedientRelationAdapter` agrega pre/postcheck, pero el mutador aún se invoca mediante `LegacyDocumentExpedientPhysicalGateway`. |
| 11 | `RegistraCacheCreacionExpedienteSII` | Crítica | Inserta sin precheck ni clave idempotente demostrada, puede duplicar en reintento, no retorna correctamente el id y deja ventana entre creación física y caché. | **Migración física obligatoria.** | Todavía se invoca desde `LegacyImportExpedientCacheRepository`, con relectura mitigadora posterior. |
| 12 | `RegistraCahcheVinculacionSII` | Crítica | La clave legacy no identifica tarea+imagen+gabinete, no demuestra la relación física y puede ocultar duplicados por radicado. | **No reutilizar.** Migración terminada. | `MySqlImportDocumentLinkCacheRepository` usa clave persistente por documento, escritura idempotente y relectura. |
| 13 | `ActualizaIndiceDocumentosSII` | Crítica | Una caché previa evita postcheck, limita expedientes procesados, usa el primer elemento y mezcla metadatos, caché e índice sin demostrar SQL/XML. | **No reutilizar.** Migración terminada. | Coordinador moderno procesa todo el universo secuencialmente y separa estados de campos, índice SQL y XML. |
| 14 | `ActualizaIndiceDocumentoCacheExpediente` | Baja | SQL concatenado y actualización masiva por expediente; `YES` no acredita filas ni valores y no confirma índice SQL/XML. | **No reutilizar.** Migración terminada. | Gateway moderno actualiza por `ID + ID_EXPEDIENTE`, descubre campos dinámicos, ajusta longitudes/tipos y relee valores. |
| 15 | `ActualizaIndiceDocumentoIntegracionSII` | Crítica | `UPDATE` masivo por `ENLASE` puede afectar cero o demasiadas filas y tampoco confirma valores ni XML. | **No reutilizar.** Migración terminada. | Gateway moderno opera documento por documento sobre el universo autoritativo. |

## Migraciones todavía obligatorias

### 1. Repositorio moderno de caché de creación

Sustituir conjuntamente `SolicitaCacheCreacionExpedienteSII` y `RegistraCacheCreacionExpedienteSII`. La lectura y escritura deben quedar en un mismo repositorio moderno con consultas parametrizadas, discriminador `NoEncontrado/Encontrado/Conflicto`, relectura y protección única persistente. Mientras la tabla legacy no pueda garantizar unicidad, debe tratarse como evidencia secundaria y nunca escoger `Rows(0)`.

### 2. Creación física moderna de expediente

Sustituir `AutoRegistraExpedienteTramite` detrás de `IPhysicalExpedientGateway`. La nueva implementación debe conservar las reglas comprobadas, no copiar el método monolítico. Debe separar: resolución de plantilla y campos, precheck por todos los `estado_unico=1`, inserción transaccional de estructuras SQL, creación/publicación segura del XML y postcheck completo. Un fallo después del commit debe producir `ResultadoIncierto` recuperable, no un segundo insert ciego.

Precondición detectada al iniciar la tarea 10.3: `ContextoImportacionServicio` no transporta la empresa de Gestión Documental. El método legacy obtiene `GA_IDEMPRESA` de sesión y lo usa para generar consecutivos/códigos e insertar `ID_EMPRESA_EXPEDIENTE`; por tanto, el gateway moderno debe recibir un `IdEmpresaGestion` resuelto autoritativamente en la frontera autenticada o mediante repositorio. Está prohibido inferirlo del cliente, fijarlo como constante o volver a consultar `HttpContext.Session` dentro de infraestructura.

### 3. Vinculación física moderna

Sustituir `VinculaDocumentoExpediente` detrás del puerto actual. La clave autoritativa es `(gabinete, IdImagen)` y el destino esperado debe compararse siempre. La operación debe detectar relación correcta, ausente, cruzada y múltiple; escribir únicamente si está ausente; y confirmar de forma independiente producción documental, relación, índice SQL y XML.

## Orden recomendado

1. Migrar primero la caché de creación: reduce ambigüedad al localizar y verificar expedientes.
2. Migrar la creación física: es la frontera con mayor radio de impacto y dependencia de sesión/XML.
3. Migrar la vinculación física: comparte riesgos SQL/XML y debe apoyarse en identidad de expediente ya estable.
4. Ejecutar E2E MERCANTIL, ESAL y RUP, incluidos reintento y respuesta perdida.
5. Deshabilitar el fallback de `SolicitaEstructuraExpedienteSII` y conservarlo únicamente para rollback del gate durante el periodo acordado.

## Criterio de cierre arquitectónico

DOC-67 puede operar de manera controlada durante transición gracias a los adaptadores, prechecks, postchecks y reconciliación. Sin embargo, no debe declararse **modernización física completa** mientras la composición productiva siga construyendo `LegacyPhysicalExpedientGateway`, `LegacyImportExpedientCacheRepository` o `LegacyDocumentExpedientPhysicalGateway`.

La retirada de esas tres dependencias de `WebServiceImportarServicioWebModern.CreateService`, seguida de pruebas equivalentes y E2E de los tres tipos de registro, constituye el criterio objetivo de terminación.
