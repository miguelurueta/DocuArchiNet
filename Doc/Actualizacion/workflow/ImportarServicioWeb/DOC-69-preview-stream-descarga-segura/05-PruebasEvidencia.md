# Pruebas y evidencia

Pruebas focales:

```powershell
node --test tests/importar-servicio-web-preview-authorization.test.cjs tests/importar-servicio-web-preview-content.test.cjs tests/importar-servicio-web-preview-single-fetch.test.cjs
npm.cmd --prefix tools/e2e run test:doc56:policy
msbuild GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug /m:1
```

La E2E real reutiliza `import-sii-read`: obtiene descriptor desde el ASMX y, en la misma sesión, valida descriptor alterado, HEAD, GET y reutilización. Conserva solo estados, longitudes y headers; nunca el contenido o el descriptor.

La ejecución real requiere autorización expresa de ambiente, gate, cuenta y certificado cuando aplique. Los controles son SELECT y al finalizar el gate debe quedar apagado con usuarios/grupos vacíos.

Resultados locales del 2026-09-19:

- Suite transversal `tests/importar-servicio-web-*.test.cjs`: 355/355 aprobadas.
- Suite de política E2E `test:doc56:policy`: 17/17 aprobadas.
- Suite focal DOC-69 y regresiones inmediatas: 24/24 aprobadas.
- MSBuild .NET Framework 4.6.1: código de salida 0, sin errores; conserva advertencias heredadas.

La migración se aplicó y auditó en `workflowdocument`: 16 columnas, 4 índices, cero llaves foráneas y veredicto `PASS`.

E2E real autenticada del 2026-09-19, escenario `import-sii-read`, tarea descartable `220580`:

- `success=true`, sin código de fallo.
- Descriptor alterado rechazado de forma opaca.
- HEAD autorizado confirmado sin consumo.
- Dos GET concurrentes: una sola entrega y un rechazo; contenido íntegro y headers confirmados.
- Reutilización posterior rechazada.
- `previewContent=CONFIRMED`; capacidades, consulta, preview y preflight completados.
- Siete controles documentales verificados mediante SELECT, todos sin cambios.
- Evidencia saneada: `tools/e2e/artifacts/workflow-e2e-platform-import-sii-read.json`; no contiene descriptor, contenido ni credenciales.

E2E real de expiración del 2026-09-19:

- TTL reducido temporalmente de 5 a 1 minuto mediante el runner y restaurado en `finally`.
- HEAD previo al vencimiento respondió correctamente.
- Después de 62 segundos, el mismo descriptor fue rechazado con `404`.
- La corrida cerró con `success=true` y los siete controles SELECT sin cambios.
- Al finalizar se verificó `WorkflowCentroTrabajoModernActive=false`, usuarios/grupos vacíos y TTL nuevamente en 5 minutos.

Auditoría SELECT de telemetría del mismo recurso:

- Grupo de operación más reciente con descarga: 3 intentos totales.
- `SOLICITAR_TOKEN=1`, `CONSULTAR_SELLO=1`, `DESCARGAR_ANEXO=1`.
- Tres intentos exitosos; no hubo consumo SII desde HEAD ni desde los GET del handler.
- El identificador opaco de operación no se conserva en esta documentación.

Prueba real de migración y rollback en tabla temporal aislada:

- Creación compatible con MySQL 5.1: 16 columnas, 4 índices y cero FK.
- Rollback ejecutado exclusivamente sobre `workflow_import_preview_descriptor_doc69_test`.
- Verificación posterior: cero tablas temporales restantes y veredicto `PASS`.
- La tabla real `workflow_import_preview_descriptor` no fue eliminada ni modificada por esta prueba.

Validación final:

- `openspec.cmd validate doc-69-lista-preview-incripciones --strict`: `PASS`.
- `opsxj:refine -- DOC-69 --sync`: `PASS`, con D-01…D-08 trazadas en diseño, especificación y tareas.
- `git diff --check`: sin errores; únicamente avisos de normalización LF/CRLF.

Durante la corrida se detectaron y corrigieron tres diferencias que las pruebas simuladas no cubrían: namespace incompleto del handler `.ashx`, lectura del `MEDIUMBLOB` dependiente del conector y vaciado del BLOB con sintaxis no portable a MySQL 5.1.

Fuentes: `tests/importar-servicio-web-preview-*.test.cjs`, `tools/e2e/tests/importar-servicio-web-modern.spec.cjs`, `tools/e2e/AGENT-RUNBOOK.md`.
