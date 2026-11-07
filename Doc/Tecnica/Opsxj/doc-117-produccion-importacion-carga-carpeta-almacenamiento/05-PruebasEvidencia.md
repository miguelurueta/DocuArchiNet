# DOC-117 — 05-PruebasEvidencia

- Ticket: DOC-117
- Cambio OpenSpec: doc-117-produccion-importacion-carga-carpeta-almacenamiento
- Clasificacion: cross_cutting

## Evidencia requerida

Matriz canónica Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc117-matriz-almacenamiento.md. ALM-01: doc117-alm01-pass-20261105.md/.json. ALM-02: doc117-alm02-interno-pass-20261106.md/.json. ALM-03C y sus límites: doc117-alm03c-coexistencia-pass-20261106.md/.json en el mismo directorio. La evidencia histórica conserva sus hashes y no se traslada entre binarios o escenarios.

Regresión conjunta final365/365 PASS,43archivos, con log .tmp/doc117-alm03c-closure-regression-20261106.tap y lista de archivos asociada. Incluye núcleo .NET, guardas/catálogos/UI, pareja incierta, preservación, reconstrucción por SHA, límites SELECT y estado consolidado por la transacción exterior. Compilación completa sin errores y ensayo de restauración aislado con la candidata exacta PASS.

## QA/E2E WebForms

Ejecución real autorizada mediante consola y conductor automatizado: A16225/23323/intento63, individual16226/23324 y B16227/23325/intento64. Solapamiento99,7347ms dentro del método instrumentado, tres incorporaciones físicas, índices/clasificación y preservación confirmados. El cierre verifica loteCOMPLETADO y raícesCOMPLETADA; runtime original restaurado/reabierto, seis respaldos verificados y912 entradas originales conservadas.

La ejecución inicial y el primer cierre mantienen sus salidas2 e informes propios; el cierre final solo SELECT termina0. Consumo acumulado581/659, sin repetir cargas. Esta evidencia de QA describe una ejecución automatizada real supervisada, no una revisión visual humana adicional. No se guardan credenciales, cookies, cadenas de conexión ni cuerpos de respuesta.
