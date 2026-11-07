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

## Regresión local de la entrega reconciliada

La entrega en `feature/DOC-117` dispone del comando `npm.cmd --prefix tools/e2e run test:doc117:local`: 454/454 PASS en 49 archivos, sin fallos ni omisiones. Incluye la regresión DOC-117 y cobertura compartida DOC-111/DOC-114. Los loaders y concesiones se comprueban con historia sintética en RAM; las pruebas nativas compilan el núcleo y las guardas en procesos y directorios temporales propios. No se requieren los archivos privados de las corridas históricas, ni se activa un runtime o consulta una base real.

La regresión histórica 365/365 conserva su procedencia. El resultado nuevo no acredita una nueva ejecución ALM-01/02/03C ni una nueva revisión visual humana. Ver `reconciliacion-20261107.md` para alcance de transferencia, conservación de antecedentes y separación de las compuertas.
