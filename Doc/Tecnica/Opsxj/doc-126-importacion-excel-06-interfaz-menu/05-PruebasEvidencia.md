# DOC-126 — Interfaz Excel y menú

- Ticket: DOC-126
- Cambio OpenSpec: doc-126-importacion-excel-06-interfaz-menu
- Clasificacion: webforms_ui (WebForms UI)
- Estado: refinamiento técnico; implementación y aceptación aún sin ejecutar.

## Evidencia requerida

Compilación aislada net461 y regresiones DOC122–125, pruebas nuevas de cliente/browser local y navegación DOC119/110 afectada. Verificación de permiso exclusivo, CAS, selección obsoleta, double click, preview tipado, errores, foco/visibilidad y referencias sin secretos. Registrar SHA/comandos/resultados y límites; el refinamiento sólo acredita inspección/trazabilidad, no ejecución del producto.

## QA/E2E WebForms

Ampliar importacion-excel.spec.cjs y runner DOC110 con scope DOC126 lectura/escritura. Reutilizar login actual, controles DOC32 SELECT, generador y actor DOC125. La aceptación UI usa clicks y controles reales, con cuenta exclusiva y cuenta restringida, fixtures y efectos específicamente autorizados. Nuevos positivos no reutilizan CREAR antiguo ni scripts aplicados; adecuaciones se entregan como SQL manual y se verifican después. Conservar documentos/filas/resultados y huellas de fuentes ajenas. E2E física anterior1000por modo no certifica esta interfaz ni20000filas; no recuperar credenciales de ventanas cerradas.
