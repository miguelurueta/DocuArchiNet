# DOC-120 — revisión focal de cinco tareas dadas por probadas

El usuario confirma que las cinco primeras tareas de la lista pendiente ya se probaron y solicita revisión quirúrgica. Se revisan exclusivamente 3.87, 3.126, 3.127, 3.130 y 3.131, sin nuevas pruebas reales, conexiones o cambios de producto. Se contrastaron los primarios publicados, 27 estados locales DOC-120 y los registros de publicación de Egresos. Los estados locales permanecen excluidos de Git; no se consultan claves ni se muestran valores documentales.

| Tarea | Evidencia efectiva | Conciliación |
| --- | --- | --- |
| 3.87, recorrido funcional | Confirmación manual actual del usuario, resultado final lote52 COMPLETADO/1.000/1.000/una creación, pruebas reales de pausa/cierre/retoma y corrección local del acceso al árbol. | Completa por cobertura mixta y aceptación manual reportada. No se exige repetir el recorrido por conservar una casilla anterior. La aceptación global y privacidad/capacidad mantienen sus fuentes propias. |
| 3.126, capacidad completa | El control real7ace0d25 correlaciona1.000 documentos/inventarios distintos,44.859.102 bytes, expediente696/relación1120 y cero intentos inciertos. El video001055 muestra1.000/1.000 completados. | La importación funcional de1.000 **ya ocurrió**. Se sustituye el título desactualizado «ejecutar la importación» por el único remanente: consolidar o aceptar expresamente límites de tiempo/memoria/disco del recorrido completo. No se inventan esas métricas ni se propone otra carga para volver a acreditar los1.000. |
| 3.127, continuidad del mismo lote | E2E08bc33cc: pausa espera, cancelar cierre continúa, aceptar espera, recargar y continuar conserva cuatro documentos sin replay. Video230607: consulta52, vinculación, continuación, cierre y nueva consulta. Control final7ace0d25:1.000 resultados únicos y1.000 intentos de recepción/guardado confirmados, sin duplicación. Confirmación manual actual del usuario. | Completa. La grabación era parcial al terminar; el control final posterior acredita la terminación. Se consumen ambas fuentes sin reinterpretar la grabación como si ya mostrara el final. |
| 3.130, BENEFICIARIO250 | El usuario la da por probada. Los únicos registros automáticos del cambio encontrados son d605ad7e y fa226614: ambos se detuvieron antes de escribir. La medición original de60 caracteres es del17/11 y no describe necesariamente el estado posterior. | Cambio reportado como probado; conciliar su procedencia/estado final. No concluir que falta ejecutar el ALTER ni repetirlo. No afirmar que esos dos scripts lo aplicaron. Se solicitó únicamente aclarar si se aplicó manualmente o mediante otra ejecución. |
| 3.131, publicación Egresos | El usuario la da por probada. Existe versión45 en lotes50/52 y resultado real de1.000 documentos; la publicación original43 se conserva en las fuentes. Los scripts de cambio encontrados no llegaron a publicar. | La versión45 existe; esos datos por sí solos no muestran la regla17 a250 ni que sea el único cambio respecto de43. Conciliar el registro posterior o la aplicación manual, sin publicar otra versión por defecto. |

## Fuentes que resuelven estados históricos

- [Guardado real de cuatro documentos](doc120-guardado-real-resultado-20261119.md), run08bc33cc cerrado: pasos de pausa/cancelación/cierre/recarga y controles RA/DA.
- [Video manual230607](doc120-validacion-manual-video230607-20261119.md): continuidad del lote52; su límite de126 guardados al acabar la grabación permanece histórico.
- [Control final del lote52](doc120-lote52-control-final-resultado-20261120.md) y JSON/CSV: cierre posterior,1.000 identidades distintas y ausencia de intentos pendientes. No midió el rendimiento completo ni descargó binarios.
- [Video001055 y corrección del árbol](doc120-ver-expediente-video001055-20261120.md): finalización visible y defecto corregido localmente; la nueva aceptación visual procede del reporte actual del usuario, no de una captura nueva del agente.
- [Medición e intentos de ampliación](doc120-egresos-longitud-20261117.md): estados anteriores a escritura, sin atribuirlos al cambio posterior reportado por el usuario.

## Resultado del corte

3.87 y3.127 pasan a completas.3.126 conserva únicamente el remanente de métricas/aceptación;3.130/3.131 se tratan como conciliación de evidencia de cambios reportados como probados, no como órdenes de ejecutar DDL o publicación otra vez. **262/270**, ocho tareas abiertas al corte de esta conciliación. Se conserva por separado la prueba nativa3.118 ya completada.

No se reduce el criterio de capacidad de D-07/RQ-07 ni se presenta una muestra de tres/diez guardados como medición integral. La aceptación explícita de límites puede resolver un remanente documental; la falta de un registro no justifica repetir efectos reales. No se cierra la aceptación global ni se archiva DOC-120 durante esta revisión.
