# Revisión de evidencia DOC-117 — 2026-11-05

Esta matriz registra lo inspeccionado durante refinamiento. No certifica una nueva ejecución ni reemplaza la futura matriz de aceptación. Los reportes históricos mantienen sus SHAs y límites originales.

| Caso/afirmación | Evidencia inspeccionada | Alcance y trabajo restante |
| --- | --- | --- |
| ALM-01: índices, tipología, nombre, PDF, vínculo e inventario | Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-rec-lote-pass-20261102.md; plan 6/lote 4, documentos 16208/16209, inventarios 23297/23298 | REAL histórico, código 96c3fafdd5e6254da8a6f3fbcb994908078cdd8ebff30c1ef1809ea4a6bf8e3d; no ejecución del commit actual |
| ALM-01: ceros, DECIMAL, fecha y visibilidad de NOMBRE | tests/ImportacionCarpetasBehaviorTests.cs DocumentVerificationCases; Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-adaptador-documento-local-20261030.md | Núcleo productivo con SELECT sintéticos; relacionar cada caso con la evidencia y ejecutar regresión local |
| ALM-01: formatos A/B | Snapshot Jira DOC-117 y soporte DOC-114 de preparación | La preparación no acredita almacenamiento final; proponer únicamente combinaciones finales no acreditadas |
| Original retenido y copia consumible | AdaptadorDocumentoImportacion.Almacenar; AlmacenStagingImportacion.CrearCopiaTrabajo; DocumentAdapterCases | Código/local histórico: retención, tamaño/hash y copia con nombre original; no exclusión global de escritores externos |
| ALM-02: lectura final del documento conocido | Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-lote2-verificacion-pass-20261102.md; documentos 16204/16205 | REAL histórico SOLO LECTURA y reapertura; no demuestra un nuevo fallo interno ni replay mutante |
| ALM-02: fallos de writer/verificador/checkpoint y retorno perdido | DocumentAdapterCases/DocumentPhaseCases; Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-fase-documento-local-20261030.md | LOCAL histórico con dobles explícitos; ventanas internas documentales reales no acreditadas por estas pruebas |
| ALM-02: reconciliación | CoordinadorImportacionCarpetas.Reconciliacion.vb; ReconciliationCases; Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-reconciliacion-local-20261030.md | Verificación por IDs; INCIERTO con prueba suficiente. EN_CURSO, falta de IDs o incorporación discordante no autorizan reparar ni repetir |
| ALM-03: carga individual en degradación | Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-degradacion-individual-pass-20261102.md; documento 16212/inventario 23301 | REAL histórico durante esquema degradado; no importación simultánea. Código be095c78d774dc7f2d8b5fafb7f7dfb68c5eee0fe6623c8a9c8c94728e9a9a1e |
| ALM-03: solapamiento real | Sin evidencia suficiente en las fuentes inspeccionadas | NO_ACREDITADO. Requiere presupuesto autorizado y prueba de ejecución solapada; no afirmar XML/SII concurrente |
| Negativos | DocumentAdapterCases/DocumentCatalogCases/DocumentVerificationCases | LOCAL histórico con dobles: recurso ajeno, nombres, relación, TIFF y metadatos. Reutilizar antes de añadir pruebas |

La revisión no reproduce fallos y no ha identificado todavía un defecto que justifique modificar producto. Las variantes de laboratorio de DOC-116 instrumentaban creación, no almacenamiento: sus PASS no se trasladan a ALM-02 documental. El lote 18/intento 41 queda fuera del alcance de mutación de DOC-117.
