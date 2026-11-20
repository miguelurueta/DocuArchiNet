# DOC-120 — continuación local del gobierno y revisión de pendientes

Fecha: 2026-11-20. Base inspeccionada: `256ceee2de30c9b52327700b73d9bd29574e5140`, rama `feature/DOC-120` de la carpeta principal. Origen: D-08/RQ-08 y D-10/RQ-10; avance parcial de 6.4–6.6. Estado: **LOCAL_PASS; ACEPTACIÓN INTEGRAL ABIERTA**. Permanecen **259/270 tareas completas y 11 abiertas**.

## Defectos de gobierno reproducidos y corrección

`opsxj:validate DOC-120` detectó seis comprobaciones fallidas. Dos eran resolubles localmente:

- `refinement:tasks:origins`: 39 tareas usaban pares `D-XX/RQ-XX` en su origen, mientras el contrato exige `Origen: D-XX, RQ-XX`. Se normalizaron los separadores conservando todos los identificadores, descripciones, verificaciones y casillas.
- `refinement:no_placeholders`: el validador trataba como plantilla incompleta cualquier aparición de «pendiente», incluidas una intención de navegación y el estado de un archivo. La reproducción falla antes de corregirlo. El control general ahora distingue prosa operativa de marcadores explícitos, campos vacíos y líneas sin desarrollar. Las filas de decisiones mantienen el control estricto anterior.

La regresión conserva el rechazo de `[PENDIENTE: evidencia]`, `TBD`, `TODO`, `por definir`, `por confirmar`, `Evidencia: pendiente`, una línea `Pendiente` y decisiones incompletas. No se modificaron contratos de importación, código de la aplicación, DLL, datos, esquema ni permisos.

## Verificación local

| Selección ejecutada | Resultado y alcance |
| --- | --- |
| `node --test tests/doc120-preparacion-segmentada.test.cjs tests/doc120-completed-check.test.cjs tests/doc120-importacion-arbol.test.cjs` | 32/32; tres archivos, respuestas sintéticas y Chromium local. Partes/recuperación, controles finales con datos simulados y buscador real sobre DOM local. |
| `npm.cmd --prefix tools/opsxj test -- scripts/lib/refinementService.test.js scripts/lib/legacyGovernanceService.test.js scripts/lib/opsxjCommandRunner.test.js scripts/lib/runChecklistService.test.js scripts/lib/opsxjStatusService.test.js` | 59/59; cinco archivos. Ocho casos nuevos del validador. La primera reproducción fue 10/11; tras corregir, las dos suites focales pasaron 19/19 y están incluidas en estos 59. |
| `openspec.cmd validate doc-120-produccion-importacion-carga-carpeta-verificacion --strict` | PASS de estructura y especificación; no concede aceptación operativa. |
| `opsxj:validate DOC-120` después de la corrección | Los controles de refinamiento pasan. Antes de renovar la evidencia del commit quedan tareas, revisión del SHA, evidencia unit antigua y aceptación manual ausente. |

Son **91 casos únicos de esta selección**, sin sumar las ejecuciones focales repetidas ni los cortes históricos. Cero autenticaciones, HTTP de aplicación, SELECT reales o efectos de negocio. El flujo sintético de 1.000 archivos no acredita capacidad real ni rollback MySQL. No se necesita compilación .NET porque este corte modifica únicamente documentación y herramientas Node de gobierno.

## Pendientes y decisión de salida

| Tareas | Evidencia disponible y brecha | Responsable y siguiente acción |
| --- | --- | --- |
| 3.87 | QA real de cuatro guardados y control final del lote 52, sin recorrido completo de inicio/creación/árbol con la versión actual. | Solicitante y operador: delimitar una ventana nueva de UI; desarrollo: preparar y contrastar el ejecutor. Mantener abierta. |
| 3.118 y 3.187 | Pruebas locales en memoria y QA HTTP histórica; faltan carrera y fallo/rollback nativos de MySQL en los contratos indicados. | Operador y desarrollo: identificar BD/cuentas/recursos de laboratorio, preparar casos y topes y obtener autorización específica antes de conectar. Mantener abiertas. |
| 3.126, 3.127 y 5.2 | Lote 52 completado con metadatos contrastados; falta acreditar la ventana completa de capacidad, sus métricas/umbrales y la continuación dentro de ese contrato. | Solicitante y operador: revisar el plan de capacidad frente al estado actual y autorizar recursos/efectos/recuperación/topes. No reutilizar el lote completado como carga nueva. Mantener abiertas. |
| 3.130 y 3.131 | Medición de BENEFICIARIO disponible; aplicación detenida por `PUBLICATION_CHANGED` antes de escribir. La versión 45 del lote 52 no prueba que estas herramientas aplicaran el cambio. | Operador y administrador de perfiles: observar publicación/revisiones/capacidad actuales en una ventana nueva antes de proponer cualquier escritura. Mantener abiertas; no repetir ALTER/publicación por inferencia. |
| 6.4 | Límites reales/locales diferenciados; faltan decisiones expresas de salida y aceptación visual actual. La intermitencia contextual histórica conserva su límite. | Solicitante y responsable de aceptación: resolver las brechas o aceptar límites expresamente, según su impacto. Mantener abierta. |
| 6.5 | Se corrigen dos defectos locales y se renueva evidencia unit del commit; no se confirma revisión OpenSpec ni QA integral por esta selección. | Desarrollo y revisor: completar gobierno sobre el SHA de entrega tras las decisiones anteriores. Mantener abierta. |
| 6.6 | Entrega parcial local verificable; archivo, PR integrado y cierre global conservan sus dependencias. | Desarrollo: registrar/sincronizar este corte. Responsable de entrega: completar revisión/PR/merge solo con las compuertas satisfechas. Mantener abierta. |

La autorización requerida procede de D-08/RQ-08, la tarea 5.2 y el runbook. La continuación actual permite trabajo local; no identifica una nueva instancia/cuenta ni autoriza efectos reales o inyección de fallos. Primero se necesita concretar el laboratorio y revisar su ejecutor/topes; no se solicita aprobación de una prueba todavía indefinida.

## Fuentes preservadas

- [Control final del lote 52](doc120-lote52-control-final-resultado-20261120.md): diez SELECT, 1.000 documentos e inventarios distintos, una creación confirmada y cero intentos activos/inciertos; ventana cerrada.
- [Guardado real, pausa y continuación](doc120-guardado-real-resultado-20261119.md): cuatro documentos, alcance y versión propios; ventana cerrada.
- [Preparación segmentada](doc120-segmentada-20261117.md): 3.118 explícitamente parcial por falta de ensayo transaccional MySQL.
- [Longitud de Egresos](doc120-egresos-longitud-20261117.md): aplicación detenida y ninguna publicación atribuible a esas corridas.
- [Corrección del acceso al expediente](doc120-ver-expediente-video001055-20261120.md): regresión local; acceso visual actual al expediente 696 sin acreditar.

No se reabren ventanas cerradas, se repiten efectos ni se transforman metadatos finales en verificación binaria o capacidad de inicio completo. El registro unit local se vincula al commit que contiene este informe; no se registra `manual_qa` como aprobada.
