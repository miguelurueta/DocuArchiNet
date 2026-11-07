# Verificación DOC-117 — 2026-11-07

Cambio: doc-117-produccion-importacion-carga-carpeta-almacenamiento. Perfil legacy-webforms-vb. Revisión: Codex. HEAD observado: b2e93b726f221868cf2b82d43200a8e568871117. La revisión incluye archivos locales todavía sin commit; el SHA de HEAD no representa por sí solo esa implementación.

| Dimensión | Resultado |
| --- | --- |
| Completitud | 18/18 tareas; ocho requisitos RQ-01–RQ-08 trazados a D-01–D-08 |
| Corrección | Cobertura local del adaptador, verificación, checkpoint, repetición, reconciliación y negativos; evidencia real ALM-01/02/03C con sus límites propios |
| Coherencia | Reutilización del almacenamiento existente; fuentes documentales conservadas; instrumentación de laboratorio separada |

## Comprobaciones de esta revisión

- `npm.cmd --prefix tools/opsxj run opsxj:refine -- DOC-117`: PASS, aprobación y trazabilidad completas.
- `openspec.cmd validate doc-117-produccion-importacion-carga-carpeta-almacenamiento --strict`: PASS.
- `node .tmp/doc117-opsxj-verification-20261107.cjs`: PASS. Ejecuta los 43 archivos del inventario `.tmp/doc117-alm03c-closure-test-files-20261106.json`; resultado 365/365, cero fallos, cancelaciones u omisiones. Log: `.tmp/doc117-opsxj-verification-20261107-1794063697585.tap`.
- Se contrastaron por SHA-256 las 63 referencias de `sourceEvidence` y `finalReadbackReviewedFiles` del certificado `doc117-alm03c-coexistencia-pass-20261106.json`: todas coinciden.
- De diez archivos en `source-review.json`, las nueve fuentes documentales conservan su SHA normalizado. La diferencia del test `tests/ImportacionCarpetasBehaviorTests.cs` está explicada en `doc117-local-20261105.md`: aserción de bytes originales y negativos TIFF. La captura histórica permanece intacta.
- La primera validación OPSXJ detectó la frase inicial «Completar la evidencia» del resumen como instrucción de plantilla. Se sustituyó por una descripción declarativa del resultado acreditado, sin cambiar su alcance; la validación final se ejecuta sobre esa corrección documental.

## Cobertura de requisitos

| Requisito | Evidencia contrastada |
| --- | --- |
| RQ-01 | source-review.json, matriz canónica y certificados con procedencia y SHA |
| RQ-02 | AdaptadorDocumentoImportacion.Almacenar, DocumentAdapterCases y DocumentPhaseCases: copia privada, original íntegro y repetición sin nuevo escritor |
| RQ-03 | PerfilImportacionCatalogAdapter.Documento.vb, DocumentVerificationCases/DocumentCatalogCases y ALM-01 real |
| RQ-04 | CoordinadorImportacionCarpetas.Documento.vb, DocumentPhaseCases y ALM-02 interno: retorno perdido, checkpoint y conservación de incertidumbre |
| RQ-05 | CoordinadorImportacionCarpetas.Reconciliacion.vb y ReconciliationCases: identidades concordantes; rechazo de EN_CURSO e IDs insuficientes sin escritor |
| RQ-06 | ALM-03C real, observación interna y negativos locales de serialización y evidencia incompleta |
| RQ-07 | ComandoAlmacenamientoCarpetas y grupos DocumentAdapterCases/DocumentCatalogCases: nombres, TIFF, recursos y staging |
| RQ-08 | Matriz, certificados y pruebas de concesiones, controles SELECT, presupuestos y restauración |

La evidencia real revisada pertenece a las corridas previas y sus variantes, no a una ejecución nueva de HEAD. ALM-03C confirma documentos 16225/16226/16227, inventarios 23323/23324/23325 y solapamiento interno de 99,7347 ms. El certificado conserva 581/659 SELECT acumulados y la restauración/reapertura del original. No acredita XML/SII concurrente, commits simultáneos ni rendimiento ordinario. Esta revisión no ejecutó login, consultas reales, cargas o instalación de DLL.

## Hallazgos y entrega

Sin hallazgos críticos de implementación o escenarios faltantes en el alcance revisado. No se omitieron dimensiones de verificación.

Advertencia de entrega: rama actual `feature/DOC-105`, con cambios locales de varios tickets. El comando `opsxj:archive` exige la rama esperada para DOC-117 limpia y sincronizada y después ejecuta commit, push, PR y comentario Jira. Antes de esa operación, preparar una entrega aislada y revisar su contenido; no incluir por arrastre los cambios de otros tickets o archivos generados. Una revisión registrada para este HEAD debe renovarse si cambia el SHA y comprobarse si cambia el contenido local.

La evidencia de tipo `manual_qa` exigida por el manifiesto referencia la ejecución automatizada real supervisada ya documentada en `05-PruebasEvidencia.md`; no constituye una nueva inspección visual humana. El registro local de compuertas no publica ni cierra el ticket.
