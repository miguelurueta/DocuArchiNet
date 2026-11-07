# DOC-118 — coordinación y recuperación

- Ticket: DOC-118
- Cambio OpenSpec: doc-118-produccion-importacion-carga-carpeta-lote-recuperacion
- Clasificación: cross_cutting
- Estado: propuesta de refinamiento para revisión humana; sin despliegue ni integración real nueva.

## Evidencia requerida

unit: el 2026-11-07 se ejecutó node --test tests/doc110-perfil-importacion-behavior.test.cjs; 2/2 pruebas Node, cero fallos, VB/.NET Framework y DDL locales sin HTTP ni conexión. No se suman grupos internos C# al contador Node.

manual_qa: no se ejecutó QA real nuevo para DOC-118. La matriz Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-matriz-coordinacion.md distingue REC reales históricos, dobles actuales y las integraciones COORD-02/03 NO_EJECUTADAS. source-review.json registra hashes de fuentes y reportes y la base de revisión. Los resultados previos no se trasladan a los binarios actuales.

## QA/E2E WebForms

Antes de ejecutar: resolver observación compatible dentro de llamada legacy real, documentar ambiente/cuentas/recursos nuevos, puntos de fallo, límites y corte, leer AGENTS.md y el runbook, y obtener autorización específica. No se concede por este documento. Intentos41/lote18 y53/lote21 permanecen intactos. No repetir cargas acreditadas ni efectos inciertos. La revisión editorial, pruebas locales y validación OpenSpec no completan las brechas reales ni autorizan su ejecución.
