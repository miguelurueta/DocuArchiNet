# Verificación OpenSpec — DOC-104

Fecha: 2026-10-14.

## Resumen

| Dimensión | Estado |
| --- | --- |
| Completitud | 23/24 tareas; 10/10 requisitos con implementación |
| Corrección | 19/20 escenarios verificados; documento firmado pendiente por dato externo |
| Coherencia | D-01 a D-10 seguidas; sin contradicciones detectadas |

## CRITICAL

### Fixture firmado no disponible

La tarea 2.3 y el escenario «Documento firmado» no tienen evidencia de ejecución real porque el ambiente autorizado no dispone de un PDF descartable registrado como firmado.

Acción requerida antes de archivar: proporcionar una tarea/PDF firmado descartable y ejecutar el escenario `signed` descrito en `06-BloqueoFixtureFirmado.md`. No marcar la tarea, archivar el cambio ni declarar cobertura 20/20 antes de obtener `DOC104_SERVER_PROCESSING_RESULT=PASSED`.

## WARNING

Ninguno adicional.

## SUGGESTION

Ninguna adicional.

## Trazabilidad observada

- RQ-01: cardinalidad/tipos en `WebService_itext.asmx.vb:88`; certificado en `WebService_itext.asmx.vb:108`; mutación posterior en `WebService_itext.asmx.vb:123`.
- RQ-02: límites binarios/dimensiones en `itextshare/Class_ItexShare.vb:297` y `itextshare/Class_ItexShare.vb:316`; página antes del stamper en `itextshare/Class_ItexShare.vb:195`.
- RQ-03: contexto GUID en `itextshare/Class_ItexShare.vb:259`; lock exclusivo en `itextshare/Class_ItexShare.vb:170`; limpieza propia en `itextshare/Class_ItexShare.vb:400`.
- RQ-04: `LockBits` y alfa en `workflow/Class_transparent.vb:30` y `workflow/Class_transparent.vb:51`.
- RQ-05: tamaño 90 x 90 y recursos en `itextshare/Class_ItexShare.vb:223` y `itextshare/Class_ItexShare.vb:230`.
- RQ-06: staged, validación, reemplazo y restauración en `itextshare/Class_ItexShare.vb:216`, `itextshare/Class_ItexShare.vb:236`, `itextshare/Class_ItexShare.vb:372` y `itextshare/Class_ItexShare.vb:387`.
- RQ-07: URL vigente con cache-buster en `itextshare/Class_ItexShare.vb:248`; página `i` en `pdfjs/pdf_anotate_view/pdf_viewer_anotate.js:1019`.
- RQ-08: clasificación posterior al reemplazo en `itextshare/Class_ItexShare.vb:247`.
- RQ-09: auditoría mínima en `webservice/WebService_itext.asmx.vb:133`; estados cliente cerrados en `pdfjs/pdf_anotate_view/pdf_viewer_anotate.js:1310`.
- RQ-10: autorizaciones y evidencia saneada en `tools/e2e/scripts/run-doc104-server-processing-interactive.cjs:90`; E2E normal real aprobada y suites focales documentadas.

## Evaluación final

Existe un único problema crítico externo. No se detectaron defectos adicionales de implementación, divergencias del diseño ni escenarios pendientes distintos del documento firmado. El cambio está listo para revisión, pero no para archivo definitivo.
