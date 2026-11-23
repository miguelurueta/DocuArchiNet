# DOC-125 — Confirmación y recuperación

- Ticket: DOC-125
- Cambio OpenSpec: doc-125-importacion-excel-05-confirmacion-recuperacion
- Clasificacion: cross_cutting (Transversal)
- Estado: refinamiento técnico, sin implementación ni E2E escritora ejecutada.

## Evidencia requerida

El manifiesto exige unit, e2e y manual_qa. Ampliar el harness compilado DOC122 con pruebas nativas de modos, transacciones, scopes, replay, CREATE, rollback e incertidumbre; conservar regresiones DOC122–124/carpetas y build net461. Las pruebas del nuevo comportamiento todavía no se ejecutaron porque el producto aún no está implementado. La auditoría del refinamiento se registra por separado y no sustituye estas evidencias.

## QA/E2E WebForms

Reutilizar tools/e2e/tests/importacion-excel.spec.cjs, runner DOC110, autenticación privada y controles SELECT DOC32. El nuevo modo de confirmación exige scope DOC125 con ambiente, cuentas, destinos descartables y efectos autorizados. No heredar el permiso de sólo lectura DOC124 ni reabrir sus ventanas. Los 625 contratos y cargas históricas se conservan.

La matriz acreditará CREAR/AGREGAR/REEMPLAZAR, dos sesiones, replay, acuse perdido, rollback y carrera UNIQUE frente a otro escritor. La E2E no instala, siembra ni limpia bases; fixtures/plantillas/permisos se configuran por archivos manuales si hacen falta. Mantener autorización específica y distinguir datos reales, dobles locales y mediciones de capacidad. SQL18 fue entregado como SELECT, no ejecutado.
