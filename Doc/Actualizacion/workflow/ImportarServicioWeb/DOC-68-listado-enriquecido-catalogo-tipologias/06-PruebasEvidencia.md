# Pruebas y evidencia

Ejecutado localmente:

```text
node --test Tests/importar-servicio-web-query-presentation.test.cjs Tests/importar-servicio-web-document-type-catalog.test.cjs Tests/importar-servicio-web-query-no-extra-sii-calls.test.cjs Tests/importar-servicio-web-contracts.test.cjs Tests/importar-servicio-web-sii-contract-mapping.test.cjs Tests/importar-servicio-web-production-composition.test.cjs
Resultado actual: 17/17 aprobadas.

MSBuild GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug
Resultado: compilación correcta; advertencias legacy preexistentes.
```

## E2E real autorizada — 2026-09-18

Se ejecutó la plataforma compartida, sin runner ni autenticación paralelos:

```text
Escenario: import-sii-read
Ambiente: CERTIFICACION
Módulo: WORKFLOW REGISTRO
Perfil no sensible: doc67-esal-220573-read.runtime.json
Resultado: FAILED
Código: E2E_PLATFORM_STAGE_FAILED
Detalle saneado: page.waitForFunction excedió 30000 ms
```

El fallo ocurrió durante `initializeWorkflowContext`, antes de invocar los contratos modernos. Tras accionar la tarea esperada, el campo de contexto `#Hidden_id_tarea_selecionada` no reflejó el identificador dentro del presupuesto de 30 segundos. En consecuencia, esta corrida no validó `ResolveCapabilities`, `QueryItems`, el catálogo ni la presentación DOC-68.

Controles de seguridad y no mutación observados:

| Control | Resultado |
| --- | --- |
| Controles `SELECT` | 7 ejecutados, 0 cambios |
| Intenciones, ítems, relaciones, índice, caché, expediente y auditoría | Sin cambios |
| Recursos reservados/consumidos | 0 |
| Gate al cierre | `false`, usuarios y grupos vacíos |
| Evidencia saneada | `tools/e2e/artifacts/workflow-e2e-platform-import-sii-read.json` |

La E2E real permanece pendiente de una tarea activa, visible y seleccionable por la cuenta autorizada. Las pruebas locales demuestran correspondencia estructural y reglas focales, pero no sustituyen la evidencia aprobatoria del ambiente.

## E2E real MERCANTIL aprobada — 2026-09-18

Se repitió el escenario de lectura con una tarea activa y seleccionable:

```text
Escenario: import-sii-read
Ambiente: CERTIFICACION
Módulo: WORKFLOW REGISTRO
Registro: MERCANTIL
Tarea: 220562
Perfil no sensible: doc68-mercantil-220562-read.runtime.json
Resultado: PASSED
```

Resultados saneados del contrato:

| Operación | Código funcional | Latencia |
| --- | --- | ---: |
| `ResolveCapabilities` | Sin error | 71 ms |
| `QueryItems` | Sin error | 654 ms |
| `PreviewImport` | Sin error | 863 ms |
| `PreflightImport` | Sin error | 35 ms |

La corrida validó las invariantes DOC-68 integradas en el adaptador: catálogo de tipologías con identificadores y nombres válidos, y elementos con metadatos, estado de importación y acciones disponibles. No se persisten cuerpos de respuesta ni datos personales como evidencia.

| Control | Resultado |
| --- | --- |
| Operaciones contractuales verificadas | 4 |
| Controles `SELECT` | 7 ejecutados, 0 cambios |
| Recursos reservados/consumidos | 0 |
| Gate al cierre | `false`, usuarios y grupos vacíos |
| Páginas legacy modificadas | Ninguna |
| Evidencia saneada | `tools/e2e/artifacts/workflow-e2e-platform-import-sii-read.json` |

Esta evidencia aprueba el flujo real de lectura MERCANTIL. La matriz completa de 5.3 continúa pendiente para los registros ESAL y RUP que dispongan de una tarea activa autorizada.

## E2E real RUP no concluyente — 2026-09-18

```text
Escenario: import-sii-read
Ambiente: CERTIFICACION
Módulo: WORKFLOW REGISTRO
Registro: RUP
Tarea: 220578
Perfil no sensible: doc68-rup-220578-read.runtime.json
Resultado: FAILED
Código: IMPORT_E2E_QUERY_FAILED_EXTERNAL_TIMEOUT
Mensaje público: La operación no está disponible.
```

La sesión y el contexto de tarea fueron resueltos, pero `QueryItems` recibió un timeout del proveedor SII antes de obtener el universo documental. Por tanto, esta corrida no permite aprobar ni rechazar el contrato de presentación RUP y debe repetirse cuando el proveedor esté disponible.

| Control | Resultado |
| --- | --- |
| Controles `SELECT` | 7 ejecutados, 0 cambios |
| Recursos reservados/consumidos | 0 |
| Gate al cierre | `false`, usuarios y grupos vacíos |
| Páginas legacy modificadas | Ninguna |
| Clasificación | Dependencia externa; reintentable |

### Reintento RUP aprobado

El mismo recurso y perfil se reintentaron sin modificar datos ni expectativas. El segundo intento finalizó correctamente:

| Operación | Código funcional | Latencia |
| --- | --- | ---: |
| `ResolveCapabilities` | Sin error | 27 ms |
| `QueryItems` | Sin error | 1260 ms |
| `PreviewImport` | Sin error | 1495 ms |
| `PreflightImport` | Sin error | 44 ms |

```text
Resultado: PASSED
Operaciones contractuales verificadas: 4
Controles SELECT: 7, sin cambios
Recursos reservados/consumidos: 0
Gate al cierre: false; usuarios y grupos vacíos
```

La evidencia aprueba el flujo real RUP y confirma que el fallo anterior fue transitorio en la dependencia externa.

## E2E real ESAL aprobada — 2026-09-19

```text
Escenario: import-sii-read
Ambiente: CERTIFICACION
Módulo: WORKFLOW REGISTRO
Registro: ESAL
Tarea: 220572
Perfil no sensible: doc68-esal-220572-read.runtime.json
Resultado: PASSED
```

El contrato público saneado confirmó una inscripción y un único sello `tipoanexo=505`, sin paginación pendiente. El elemento presentó libro `RE51`, registro `51183`, fecha, naturaleza y noticia correlacionados bajo el mismo `ExternalKey`; su estado local fue `Importado` y la única acción permitida fue `View`.

| Operación | Código funcional | Latencia |
| --- | --- | ---: |
| `ResolveCapabilities` | Sin error | 40 ms |
| `QueryItems` | Sin error | 882 ms |
| `PreviewImport` | Sin error | 1022 ms |
| `PreflightImport` | Sin error | 38 ms |

```text
inscriptionCount: 1
imageCount: 1
items: 1
continuationToken: null
importStatus: Importado
allowedActions: View
Controles SELECT: 7, sin cambios
Recursos reservados/consumidos: 0
Gate al cierre: false; usuarios y grupos vacíos
```

Con esta corrida quedan aprobados los tres registros disponibles exigidos por 5.3: MERCANTIL, ESAL y RUP.

## Corrección de universo documental SII

La inspección real demostró que una inscripción puede incluir simultáneamente sellos `tipoanexo=505` y soportes SIPREF `tipoanexo=518`. Se corrigió el mapper para publicar solo 505 y el resolvedor físico para rechazar claves de otros tipos.

```text
Suite focal actual de contratos, catálogo, presentación, conteo, mapper y composición: 17/17 aprobadas
Compilación .NET Framework: correcta
Compilación final: 0 errores, 1 advertencia agregada por MSBuild
E2E posterior al filtro: aprobada en RUP
```

Resultado contractual real posterior a la corrección:

```text
inscriptionCount: 1
imageCount: 1
continuationToken: null
items: 1
importStatus: Importado
allowedActions: View
```

La misma respuesta SII había reportado cuatro anexos antes del filtro. El resultado confirma que `tipoanexo=505` conserva un único sello y excluye los otros tres anexos. La plataforma completó `ResolveCapabilities`, `QueryItems`, `GetPreview` y `PreflightImport`; ejecutó 7 controles `SELECT` sin cambios y restauró el gate a `false` con alcance vacío.

### E2E multiinscripción MERCANTIL

La tarea 220580 y el código de barras 18341190 validaron cardinalidad real múltiple:

```text
inscriptionCount: 3
imageCount: 3
items: 3
continuationToken: null
Estados: 3 Disponible
Acciones: 3 Preview + Import
Resultado E2E: PASSED
```

Cada inscripción produjo exactamente un sello 505: `RM09/122222`, `RM09/122223` y `RM15/930482`. Las cuatro operaciones contractuales finalizaron sin error (34, 706, 871 y 38 ms), los 7 controles `SELECT` permanecieron sin cambios y el gate fue restaurado.

### E2E de importación de los tres sellos

La tarea descartable 220580 se ejecutó con `sampleSize=3`, una sola intención y los tres sellos 505. La plataforma reportó `success=true`; `Create`, `Execute`, `Get` y `Reconcile` finalizaron sin código funcional, los siete controles mutantes cambiaron y el recurso quedó consumido.

La auditoría posterior, exclusivamente `SELECT`, confirmó:

```text
ITEM_COUNT=3
RELATED_COUNT=4
ITEM_STORAGE_CONFIRMED=3
ITEM_RELATION_CONFIRMED=3
ITEM_INDEX_CONFIRMED=3
ITEM_CACHE_CONFIRMED=3
INTENT_COMPLETED=TRUE
ITEMS_COMPLETED=TRUE
ITEMS_STORED=TRUE
ITEMS_HAVE_EXPEDIENT=TRUE
ITEM_EFFECTS_CONFIRMED=TRUE
INSCRIPTIONS_CONFIRMED=TRUE
RELATED_UNIVERSE_UNIQUE=TRUE
RELATED_EFFECTS_CONFIRMED=TRUE
CACHE_MATCHES_UNIVERSE=TRUE
NO_ERRORS=TRUE
RECONCILIATION_TRANSITION=TRUE
VERDICT=PASSED
```

Los cuatro relacionados corresponden al universo documental de la tarea reconciliado por DOC-67; no representan cuatro sellos importados. Los sellos incorporados fueron exactamente tres.
