# Resultados de rendimiento DOC-105

## Workflow focal con preview prioritario — 2026-10-18

Evidencia capturada 12:33:40.576 UTC: PASSED, 34 páginas; tres aperturas 2.266/350/347 ms, mediana 350 ms. Antes del primer canvas, recursos completados de 390.850 bytes en cada muestra (preview); principal de 9.646.444 bytes. Transporte RANGE, feedback, scroll y zoom aprobados, sin errores clasificados. Cierre de 11.12/11.13, sin sustituir FAILED históricos. No hay emulación explícita 10/20/50/100 Mbps en esta corrida focal y no completa 9.6. La primera apertura es más lenta que la mediana: no se afirma mejora uniforme frente a las corridas anteriores.

## Conciliación de tarea 9.6 — 2026-10-18

No completar 9.6 con las ocho celdas de Producción: corresponden a otro PDF y a primera página `preview`. Workflow tiene baseline STREAM en cuatro redes, pero la comparación documentada del derivado completo RANGE solo incluye remote-100 frío/caliente. Faltan muestras equivalentes del derivado Workflow a 10/20/50 Mbps o recuperar evidencia histórica no localizada. La limpieza y desactivación están documentadas en despliegue-rollback.md; ello no acredita su ensayo (8.6). No se repite Producción ya aprobada ni se mezclan presupuestos entre muestras distintas. Revisión documental, sin nueva ejecución.

Estado: Workflow y Producción Documental `PASSED` en 10/20/50/100 Mbps; cobertura transversal `BLOCKED` por consumidores sin perfil autorizado.

## Workflow remoto autorizado

| Red | Caché | Primer canvas | `domInteractive` | Tareas largas | HTTP/modo | Bytes físicos | Solicitudes |
| --- | --- | ---: | ---: | ---: | --- | ---: | ---: |
| 10 Mbps | Fría | 32.618 ms | 504 ms | 1 | 200 / `STREAM` | 33.592.820 | 1 |
| 10 Mbps | Caliente | 29.183 ms | 201 ms | 1 | 200 / `STREAM` | 33.592.820 | 1 |
| 20 Mbps | Fría | 17.726 ms | 606 ms | 1 | 200 / `STREAM` | 33.592.820 | 1 |
| 20 Mbps | Caliente | 15.305 ms | 181 ms | 1 | 200 / `STREAM` | 33.592.820 | 1 |
| 50 Mbps | Fría | 9.691 ms | 716 ms | 1 | 200 / `STREAM` | 33.592.820 | 1 |
| 50 Mbps | Caliente | 7.129 ms | 163 ms | 1 | 200 / `STREAM` | 33.592.820 | 1 |
| 100 Mbps | Fría | 6.612 ms | 648 ms | 1 | 200 / `STREAM` | 33.592.820 | 1 |
| 100 Mbps | Caliente | 4.341 ms | 158 ms | 1 | 200 / `STREAM` | 33.592.820 | 1 |

Las ocho celdas pasaron transporte e interacción. Scroll y zoom no produjeron bloqueos medibles por encima de los límites. El PDF se sirvió completo mediante `STREAM`: no hubo respuestas 206 y, por tanto, no se afirma descarga parcial. `uniqueBytes=0` representa que esa métrica solo se deriva de intervalos `Content-Range`; los bytes físicos observados fueron 33.592.820 en cada carga.

## Corrección focal: derivado linealizado temporal

Forzar `RANGE` sobre el original no linealizado fue rechazado: produjo 63 solicitudes, cubrió prácticamente los 33.592.820 bytes y degradó el primer canvas a 8.003/5.828 ms. La implementación aceptada conserva `STREAM` en la primera apertura, prepara un derivado en segundo plano y usa ese derivado únicamente para visualización posterior.

| Red | Caché | Primer canvas | `domInteractive` | Tareas largas | HTTP/modo | Derivado total | Bytes antes del canvas | Bytes físicos | Solicitudes | Parcial |
| --- | --- | ---: | ---: | ---: | --- | ---: | ---: | ---: | ---: | --- |
| 100 Mbps | Fría | 5.432 ms | 395 ms | 1 | 206 / `RANGE` | 9.646.444 | 8.073.580 | 8.083.180 | 38 | Sí |
| 100 Mbps | Caliente | 4.072 ms | 159 ms | 1 | 206 / `RANGE` | 9.646.444 | 8.073.580 | 8.083.180 | 38 | Sí |

Frente al mismo PDF servido por `STREAM`, el primer canvas mejoró 17,8 % en frío y 6,2 % en caliente. Los bytes físicos observados bajaron aproximadamente 75,9 % frente a la transferencia completa del original. Scroll y zoom permanecieron dentro del presupuesto y ambas filas Workflow terminaron `PASSED`. El resultado transversal continúa `BLOCKED` exclusivamente por cuatro consumidores sin perfil autorizado.

La activación productiva requiere configurar un ejecutable absoluto, permisos de caché y revisión de licencia/distribución. Si se desactiva o falla, la ruta vuelve automáticamente a `STREAM` del original.

## Validación posterior del estado de carga

La E2E autenticada de solo lectura se repitió después de añadir la retroalimentación visual y retirar la llamada inicial a `pdf.getData()`. Las tres iteraciones terminaron `PASSED`, con mediana de primer canvas de 630 ms, 34 páginas, una tarea larga, modo `RANGE`, transporte aprobado, scroll completo y zoom completo.

La instrumentación saneada confirmó en cada iteración este orden: overlay visible antes del primer canvas, progreso indeterminado mientras no existía total, progreso determinado al recibir total y overlay oculto después del canvas. No se observó estado de error. La muestra inmediatamente anterior fue de 686 ms; la diferencia de 56 ms (8,2 %) es favorable, pero no se atribuye íntegramente al cambio porque red y servidor pueden variar entre corridas.

## Límites históricos de seguridad

| Operación | Límite que causa fallo |
| --- | ---: |
| Scroll bloqueante | 3.548 ms o más |
| Zoom bloqueante | 435 ms o más |

Estos valores provienen de DOC-100 y funcionan como límites iniciales de regresión. No son todavía los presupuestos finales de 10/20/50/100 Mbps.

## Cobertura pendiente

Correspondencia pasó `remote-100` en frío y caliente mediante su handler nativo de solo lectura; sus métricas de canvas/scroll/zoom PDF.js no aplican. Producción pasó las ocho celdas 10/20/50/100 Mbps con navegación automática y el mismo PDF pesado. Aprobación/externo pasó las ocho celdas sobre su fallback autenticado con HTTP 200, 339.878 bytes y una solicitud por celda. Continúan pendientes los perfiles 10/20/50 Mbps de Correspondencia y cada clase de handler simple. También faltan el único guardado de grafo y el ensayo operativo de rollback.

## Regresión observada en Producción Documental

La E2E `remote-100` de Producción Documental abrió el visor autorizado con HTTP 206 y modo `RANGE`, pero necesitó 231.079.058 de 289.799.314 bytes únicos antes del primer canvas (79,74 %) y 223 solicitudes. El primer canvas tardó 48.774 ms en frío y 56.662 ms en caliente. No hubo mutaciones.

Aunque la evidencia histórica quedó etiquetada `PASSED`, el resultado funcional correcto es `FAILED`: `uniqueBytesBeforeFirstCanvas < documentLengthBytes` solo demuestra parcialidad nominal. No demuestra que la primera página deje de depender de casi todo el archivo. DOC-105 exige desde D-13/RQ-13 presupuestos explícitos de tiempo, bytes y solicitudes antes de aceptar una celda enriquecida.

## Corrección implementada y repetición `remote-100`

El runner aplica presupuestos cerrados por red y el caso histórico de 48.774 ms, 231.079.058 bytes y 223 solicitudes queda rechazado. El cronómetro comienza en la acción documental y solo acepta un canvas temporal marcado como completamente renderizado.

En producto, la primera apertura prepara una representación temporal de una página y la sirve con una referencia autorizada distinta; el PDF completo comienza a cargar en paralelo y reemplaza esa vista al renderizar su primera página. El derivado completo se genera en segundo plano. `/L` y `/E` se validan antes de considerar eficaz una representación linealizada, con límites configurables de 32 MiB y 50 %. No se reemplaza el original ni se cambia `urimage_format`.

La repetición autorizada y completamente automática terminó `PASSED` para Producción en `remote-100`: 1.929 ms en frío y 1.174 ms en caliente hasta la primera página temporal; 1.629.367 bytes y 6 solicitudes antes de ese canvas en ambas celdas. El transporte fue `RANGE` con HTTP 206, parcialidad respecto del PDF original de 289.802.461 bytes, permisos conservados y cero mutaciones. El resultado global permanece `BLOCKED` exclusivamente porque la ejecución aislada no autorizó los otros consumidores.

La tarea 11.5 queda completa. Una repetición posterior `remote-10`, ejecutada sin selección manual y con la ruta operativa confirmada, también terminó `PASSED`: 4.256/4.167 ms en frío/caliente, 1.629.367 bytes físicos antes del canvas, 2.492.637 bytes únicos, 5 solicitudes, HTTP 206, `RANGE`, primera página de fuente `preview` y cero mutaciones.

## Matriz final de Producción Documental

La corrida completa posterior usó el mismo consumidor, PDF pesado, navegador y ambiente; solo variaron la red emulada y el estado de caché. Las ocho celdas conservaron HTTP 206, `RANGE`, fuente inicial `preview`, permisos y cero mutaciones.

| Red | Caché | Primer canvas | `domInteractive` | Tareas largas | Únicos antes del canvas | Físicos antes del canvas | Solicitudes antes del canvas | Estado |
| --- | --- | ---: | ---: | ---: | ---: | ---: | ---: | --- |
| 10 Mbps | Fría | 4.284 ms | 518 ms | 2 | 2.492.637 | 1.629.367 | 5 | `PASSED` |
| 10 Mbps | Caliente | 4.156 ms | 515 ms | 3 | 2.492.637 | 1.629.367 | 5 | `PASSED` |
| 20 Mbps | Fría | 2.790 ms | 373 ms | 2 | 2.492.637 | 1.629.367 | 5 | `PASSED` |
| 20 Mbps | Caliente | 2.659 ms | 360 ms | 2 | 2.492.637 | 1.629.367 | 5 | `PASSED` |
| 50 Mbps | Fría | 1.991 ms | 299 ms | 2 | 2.492.637 | 1.629.367 | 5 | `PASSED` |
| 50 Mbps | Caliente | 1.877 ms | 442 ms | 2 | 2.492.637 | 1.629.367 | 5 | `PASSED` |
| 100 Mbps | Fría | 1.724 ms | 262 ms | 2 | 3.541.213 | 1.629.367 | 6 | `PASSED` |
| 100 Mbps | Caliente | 1.479 ms | 279 ms | 2 | 3.541.213 | 1.629.367 | 6 | `PASSED` |

Los 232.130.781 bytes únicos y 224 solicitudes acumulados por celda ocurrieron después del primer canvas durante la medición completa del visor; no forman parte del presupuesto de primera página. La primera página dejó de esperar la transferencia total del PDF de 289.802.461 bytes.

## Presupuestos finales de primera página

Los límites quedan ligados a la regresión de Producción (`remote-100`: 48.774/56.662 ms, 231.079.058 bytes únicos y 223 solicitudes antes del canvas) y a las ocho muestras corregidas de la tabla anterior. Se aplican por igual a frío y caliente porque ambos estados deben aprobar de forma independiente; no se usa la muestra caliente para relajar la fría.

| Red | Caché | Máximo de tiempo | Máximo de bytes antes del canvas | Fracción máxima si la fuente es `full` | Máximo de solicitudes | Muestra corregida tiempo/bytes físicos/solicitudes |
| --- | --- | ---: | ---: | ---: | ---: | --- |
| 10 Mbps | Fría | 60.000 ms | 33.554.432 | 50 % | 64 | 4.284 ms / 1.629.367 / 5 |
| 10 Mbps | Caliente | 60.000 ms | 33.554.432 | 50 % | 64 | 4.156 ms / 1.629.367 / 5 |
| 20 Mbps | Fría | 35.000 ms | 33.554.432 | 50 % | 64 | 2.790 ms / 1.629.367 / 5 |
| 20 Mbps | Caliente | 35.000 ms | 33.554.432 | 50 % | 64 | 2.659 ms / 1.629.367 / 5 |
| 50 Mbps | Fría | 20.000 ms | 33.554.432 | 50 % | 64 | 1.991 ms / 1.629.367 / 5 |
| 50 Mbps | Caliente | 20.000 ms | 33.554.432 | 50 % | 64 | 1.877 ms / 1.629.367 / 5 |
| 100 Mbps | Fría | 15.000 ms | 33.554.432 | 50 % | 64 | 1.724 ms / 1.629.367 / 6 |
| 100 Mbps | Caliente | 15.000 ms | 33.554.432 | 50 % | 64 | 1.479 ms / 1.629.367 / 6 |

El gate usa bytes contabilizados por unión de rangos; la evidencia conserva además bytes físicos para no confundir cobertura lógica con transferencia real. La validación local del overlay (mediana 630 ms) se informa aparte y no se mezcla con estos presupuestos remotos. Los límites históricos de scroll/zoom continúan siendo 3.548/435 ms.
