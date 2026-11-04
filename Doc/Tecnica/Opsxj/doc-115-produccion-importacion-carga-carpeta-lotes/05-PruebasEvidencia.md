# DOC-115 — PruebasEvidencia

Ticket: DOC-115. Cambio: doc-115-produccion-importacion-carga-carpeta-lotes. Impacto: cross_cutting. Estado: implementación y validación completas, entrega pendiente de revisión/publicación.

## Evidencia requerida

93/93 pruebas Node y 2/2 del harness Framework PASS. Los escenarios RECIB-01/02/03 y 07-A cuentan con ejecución real autorizada. Evidencia por fase: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc115-07a-resultados.md y doc115-07b-resultados.md. Los intentos fallidos conservan sus códigos y no se sustituyen por éxito retroactivo.

## QA/E2E WebForms

RECIB-01: planes 24/25, lote 6. RECIB-02: planes 26/27, lotes 7/8. RECIB-03: plan 28, lote 9, transferencia multipart parcial y descarte deliberado del resultado en cliente, seguido de lectura/reenvío. 07-A: planes 29–31, lotes 10/11, archivo operativo 16, intento 30; cuatro rollbacks reales y trece grupos protegidos sin cambios. Sin expedientes/documentos definitivos nuevos. No cambió VB/proyecto de producto: el arnés compila Framework, sin despliegue ni MSBuild de producto adicional por este delta de pruebas.

## Límites

No se simula caída del servidor ni pérdida de respuesta de red al descartar el resultado en cliente. La cuota acumulada finita no se presenta como configurada en producción. Concurrencia HTTP puede serializarse; la comprobación independiente se hizo en 07-A. Las huellas se conservan por ejecución y no se atribuye todo a una sola revisión final.
