# VISTA-INTERFAZ-INTEGRACION-SII

- Ticket: DOC-74
- Cambio OpenSpec: doc-74-vista-interfaz-integracion-sii
- Clasificacion: cross_cutting (Transversal)

## Evidencia requerida

- [x] unit: `node --test Tests/importar-servicio-web-preview.test.cjs Tests/importar-servicio-web-preview-security.test.cjs Tests/importar-servicio-web-preview-accessibility.test.cjs`; PASS 12/12; 2026-09-22; detalle en `docs/modulos/workflow/importar-servicio-web/DOC-74-vista-segura-recursos-externos/05-PruebasEvidencia.md`.
- [ ] manual_qa: pendiente de ambiente, cuenta y tarea de prueba expresamente autorizados.

## Regresión local

Suite relacionada de API, adaptador SII, mediación, handler, invariancia legacy y almacenamiento: PASS 27/27 el 2026-09-22.

## QA/E2E WebForms

No ejecutado. El flujo requiere navegador autenticado y el runbook prohíbe inferir autorización de ambiente o cuentas. No se activó el gate, no se realizó carga y no se guardaron secretos, cookies ni cadenas de conexión.

Al recibir autorización se debe validar: apertura, foco, cierre, retorno, layout reducido, fallback MIME, renovación tras expiración y delegación al visor con una identidad interna autorizada; al finalizar, confirmar gate `false` y alcance vacío.
