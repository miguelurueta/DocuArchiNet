## ADDED Requirements

### Requirement: Exportar paginas PDF anotadas desde AppVisorEmbedPdf

El sistema SHALL permitir que `AppVisorEmbedPdf` exporte un PDF anotado de una sola pagina por cada pagina con anotaciones, sin exponer tipos ni APIs internas de EmbedPDF a consumidores externos.

#### Scenario: Sin anotaciones
- **GIVEN** un PDF cargado sin paginas anotadas
- **WHEN** el consumidor solicita exportar paginas anotadas
- **THEN** el visor retorna `hasAnnotations = false`
- **AND** retorna listas vacias de paginas anotadas y blobs
- **AND** no llama servicios HTTP

#### Scenario: Paginas anotadas base 1
- **GIVEN** `annotation.state.pages` contiene anotaciones para indices internos base 0
- **WHEN** el visor calcula las paginas anotadas
- **THEN** retorna `pageNumber` base 1
- **AND** deduplica y ordena las paginas ascendentemente

#### Scenario: Exportacion PDF real
- **GIVEN** un PDF cargado con una o mas paginas anotadas
- **WHEN** se exportan paginas anotadas
- **THEN** el visor ejecuta `commit()` antes de exportar
- **AND** usa `saveAsCopy()` para obtener el PDF anotado materializado
- **AND** genera un blob `application/pdf` por cada pagina anotada
- **AND** cada blob contiene exactamente una pagina

#### Scenario: Prohibicion de rasterizacion
- **WHEN** se implementa la exportacion de paginas
- **THEN** no se usa canvas, PNG, JPEG, Base64 ni `pdfjs-dist` para generar el contrato de reemplazo
- **AND** si no hay mecanismo real para extraer una pagina PDF, la implementacion queda bloqueada y documentada

### Requirement: Subir temporales de paginas PDF anotadas

El sistema SHALL subir cada PDF anotado de una sola pagina mediante el upload temporal del backend usando `clienteApi`, chunks binarios y envelope `AppResponses<T>`.

#### Scenario: Init por pagina
- **GIVEN** una pagina anotada exportada como PDF
- **WHEN** el frontend inicializa upload temporal
- **THEN** llama `POST /api/gestor-documental/documentos/reemplazopdf/upload-temporal/init`
- **AND** envia nombre original, tamano, extension `.PDF`, numero de chunks y hash opcional
- **AND** conserva `RutaTemporalId`, `ArchivoTemporalId` y `ChunkSizeBytes` para esa pagina

#### Scenario: Chunk binario
- **GIVEN** un temporal inicializado
- **WHEN** el frontend sube un chunk
- **THEN** llama `PUT /api/gestor-documental/documentos/reemplazopdf/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/chunk/{chunkIndex}`
- **AND** envia body binario puro
- **AND** envia `Content-Type: application/octet-stream`
- **AND** envia `X-Total-Chunks`
- **AND** no usa JSON, Base64 ni `FormData`

#### Scenario: Content-Length en browser
- **WHEN** el frontend sube chunks desde navegador
- **THEN** no intenta setear manualmente `Content-Length`
- **AND** envia `Blob`, `File` o `ArrayBuffer` para que el runtime calcule el header restringido
- **AND** los tests no requieren presencia manual de `Content-Length`

#### Scenario: Complete antes de reemplazar
- **GIVEN** todos los chunks de una pagina fueron enviados
- **WHEN** el frontend termina la subida
- **THEN** llama `POST /complete`
- **AND** valida que el temporal quede `COMPLETED` antes de usarlo en `paginas-anotadas`

#### Scenario: Cancelacion best-effort
- **GIVEN** existen temporales creados
- **WHEN** el usuario cancela o la operacion falla antes del reemplazo final
- **THEN** el frontend intenta llamar `DELETE` por cada temporal creado
- **AND** fallos de cancelacion no bloquean la limpieza local

### Requirement: Reemplazar paginas PDF anotadas

El sistema SHALL llamar el endpoint final de reemplazo enviando la lista de paginas anotadas, cada una con su propio temporal completado.

#### Scenario: Request multipagina
- **GIVEN** dos o mas paginas anotadas fueron subidas y completadas
- **WHEN** el frontend llama `POST /api/gestor-documental/documentos/reemplazopdf/paginas-anotadas`
- **THEN** envia `NombreGabinete` e `IdDocumento`
- **AND** envia `Paginas` con `PageNumber`, `RutaTemporalId`, `ArchivoTemporalId`, `ContentType = application/pdf` y hash opcional
- **AND** cada item de `Paginas` usa su propio `RutaTemporalId`
- **AND** `RutaTemporalId` raiz se usa solo como fallback compatible

#### Scenario: Documento firmado
- **GIVEN** el documento activo esta firmado electronicamente
- **WHEN** el usuario intenta guardar paginas anotadas
- **THEN** el frontend bloquea el flujo
- **AND** no exporta, no sube temporales y no llama `paginas-anotadas`

#### Scenario: Password de PDF original
- **GIVEN** el PDF original requirio password para visualizar/anotar
- **WHEN** el frontend llama el reemplazo final
- **THEN** envia `OriginalPdfPassword` solo en ese request final
- **AND** nunca persiste ni loguea la password
- **AND** limpia la password en reset, cambio de documento, cancelacion, cierre o desmontaje

#### Scenario: Anti-desfase opcional
- **GIVEN** el frontend tiene hash/version/geometria/fingerprint confiable de la version renderizada
- **WHEN** llama el reemplazo final
- **THEN** envia la metadata anti-desfase correspondiente
- **AND** si no tiene fuente real, no inventa ni envia esos campos

#### Scenario: Success
- **GIVEN** el backend responde success
- **WHEN** el reemplazo final termina
- **THEN** el frontend muestra paginas reemplazadas y `RequestId`
- **AND** refresca el documento visible
- **AND** no llama `DELETE` sobre temporales consumidos por el backend

### Requirement: Orquestacion en DocumentosWorkbench

El sistema SHALL integrar el flujo desde `DocumentosWorkbench` preservando la carga gestionada actual, cancelacion, latest-wins y no-regresion.

#### Scenario: Validaciones previas
- **GIVEN** el usuario solicita guardar paginas anotadas
- **WHEN** falta documento activo, no es PDF, faltan permisos, el visor no esta listo o no hay anotaciones
- **THEN** el frontend bloquea el flujo con mensaje funcional
- **AND** no llama APIs backend

#### Scenario: Latest-wins
- **GIVEN** existe una operacion de reemplazo en progreso
- **WHEN** el usuario cambia el documento activo o inicia una nueva operacion
- **THEN** la operacion anterior se aborta
- **AND** sus resultados no se aplican al documento nuevo
- **AND** se limpian temporales creados best-effort

#### Scenario: Error recuperable
- **GIVEN** falla init, chunk, complete o reemplazo final
- **WHEN** el frontend maneja el error
- **THEN** conserva el documento visible actual
- **AND** muestra error funcional
- **AND** preserva `Field` y `RequestId` cuando esten disponibles

#### Scenario: No regresion
- **WHEN** se implementa SCRUMCORE-238
- **THEN** siguen funcionando exportacion, impresion, firma, visualizacion de imagenes, `load/reset/cancelCurrentLoad` y carga gestionada de `DocumentosWorkbench`

### Requirement: Separacion de responsabilidades

El sistema SHALL mantener separadas las responsabilidades entre visor, toolbar, workbench y servicios HTTP.

#### Scenario: Visor sin negocio HTTP
- **WHEN** se revisa `AppVisorEmbedPdf`
- **THEN** el visor no importa `clienteApi`
- **AND** no conoce endpoints de reemplazo ni workflow

#### Scenario: Toolbar presentacional
- **WHEN** se revisa `AppPdfToolbar`
- **THEN** la toolbar no importa `clienteApi`
- **AND** no conoce endpoints, workflow ni tipos EmbedPDF

#### Scenario: Service HTTP dedicado
- **WHEN** se revisan llamadas al modulo de reemplazo
- **THEN** estan encapsuladas en un service dedicado
- **AND** usan `clienteApi`, `AbortSignal` y desempaquetado `AppResponses<T>`
