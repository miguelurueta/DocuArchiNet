# VISTA-INTERFAZ-INTEGRACION-SII

- Ticket: DOC-74
- Cambio OpenSpec: doc-74-vista-interfaz-integracion-sii
- Clasificacion: cross_cutting (Transversal)

## Evidencia requerida

- [x] unit: `node --test Tests/importar-servicio-web-preview.test.cjs Tests/importar-servicio-web-preview-security.test.cjs Tests/importar-servicio-web-preview-accessibility.test.cjs`; PASS 12/12; 2026-09-22; detalle en `Doc/Actualizacion/workflow/ImportarServicioWeb/DOC-74-vista-segura-recursos-externos/05-PruebasEvidencia.md`.
- [x] manual_qa: E2E real `import-sii-read`; PASS; tarea autorizada 220585; evidencia saneada en `tools/e2e/artifacts/workflow-e2e-platform-import-sii-read.json`; 2026-09-22.

## Regresión local

Suite relacionada de API, adaptador SII, mediación, handler, invariancia legacy y almacenamiento: PASS 27/27 el 2026-09-22.

## QA/E2E WebForms

Infraestructura preparada reutilizando el escenario `import-sii-read` de DOC-56: sesión autenticada, tarea autorizada, gate y proveedor `INTEGRACIONSII` temporales con restauración exacta en `finally`, controles ODBC `SELECT` y evidencia saneada. El inspector añadido cubre apertura, consulta visual con el código de barras no sensible del perfil, preview, foco, no repetición por foco/resize, retorno y cierre. Pruebas de plataforma locales: PASS 40/40.

E2E real ejecutado con autorización explícita para ambiente, cuenta, tarea, gate y TLS local. Resultado PASS: preview UI, foco y descarga única confirmados; siete controles ODBC sin cambios. El gate quedó apagado, el proveedor temporal vacío y no se realizó carga ni mutación. No se guardaron secretos, cookies ni cadenas de conexión.

Al recibir autorización se debe validar: apertura, foco, cierre, retorno, layout reducido, fallback MIME, renovación tras expiración y delegación al visor con una identidad interna autorizada; al finalizar, confirmar gate `false` y alcance vacío.
