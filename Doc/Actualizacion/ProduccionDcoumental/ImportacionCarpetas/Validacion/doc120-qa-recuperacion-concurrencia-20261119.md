# DOC-120: QA real de recuperación, concurrencia y privacidad

Resultado: **PASS_QA_WITH_LIMITS**, corrida5f7af8f5354a4fca5e2eb5c7a53b4b4d, terminada2026-11-19 11:50:54 UTC (06:50:54 Colombia). Fuente local cerrada, credenciales eliminadas y sin fallos visuales tolerados. Ambiente localhost/GestionDocumental-Docuarchi.net/, propietario141/empresa2. DLL8b27a942 sin cambios de producto, esquema o publicación durante esta QA. Ejecutor del commit2c37beef; hashes de todas sus fuentes coinciden con la carpeta principal.

[Evidencia JSON](doc120-qa-recuperacion-concurrencia-20261119.json) y [cuatro guardados en CSV](doc120-qa-recuperacion-concurrencia-20261119.csv). JSON conserva resultados, tiempos, hashes de estados/fuentes y manifiesto de capturas privadas; no contiene claves, cookies, tokens, cadenas de conexión o valores documentales.

## Resultados reales

| Caso | Resultado observado |
| --- | --- |
| Plan90 ya confirmado | Dos reconfirmaciones de95b22a23 convergieron al lote50; cero planes/lotes nuevos. |
| A50 y C52 del lote29 | Documentos16282/16283, inventarios23412/23413, expedientes681/694. Nueva relación1118 para694 en95b22a23; al consultar y recargar conservan exactamente sus identidades, sin repetir efectos. |
| Original faltante B51 | El diálogo identificó archivo/ruta/fase, esperó la decisión y permitió Pausar. Continúa PENDIENTE/NO_INICIADO, sin recibirlo. Continuar con los demás permitió guardar C. |
| Pausa esperando acuse | Captura95b22a23 muestra Aplicando pausa. El código sellado del commit420af6a7 coincide por hash LF: await paused(page) precede a verifyFile(C). Las12 SELECT alcanzadas y el control posterior independiente conservan C. Se mantiene el STOPPED original, cuya etiqueta de fase era demasiado amplia; no se reescribe como PASS. |
| Árbol del expediente694 | Una relación lógica y etiqueta igual al código previsto. Dos enlaces HTML representan imagen/texto, no dos expedientes; count1/matchingAnchors2/labelMatchestrue. |
| Respuesta perdida de120 | ALMACENAR200 confirmó16284/inventario23414/expediente693. Se suprimió la entrega al navegador solo después del checkpoint RA/DA; la consulta y recarga conservaron ese resultado, con una sola solicitud sobre120. |
| Dos sesiones sobre121 | Dos HTTP solapados: un200 confirmó16285/inventario23415/expediente693 y un409/OPERACION_EN_CURSO. Un solo resultado documental. |
| Revisión antigua de121 | Tercera solicitud rechazada409/CONFLICTO_REVISION; mismo documento16285. Es un caso negativo deliberado, no un reintento automático. |
| Privacidad | Carolina recibió403/ACCESO_DENEGADO sin datos al consultar y exportar cada uno de los lotes29/50; cuatro rechazos comprobados. |
| Cierre | Lote29 conserva2 guardados; lote50 pasa23→25. Huellas de25 y998 archivos ajenos a los efectos preservadas en las columnas RA observadas. Cero actividad/incertidumbre o señal de detención al control final. |

El faltante es un original local sin vincular: no se retiró ni borró un PDF recibido del servidor. Las comprobaciones DA se limitaron a los documentos/expedientes conocidos y su propietario/empresa; las huellas RA cubren las columnas seleccionadas del control, no toda la base.

## Consumo y preservación

| Ventanas | HTTP | SELECT externos | Sesiones |
| --- | ---: | ---: | ---: |
| Última5f7af8f5 | 758 de1200 | 22 de94 | 3 de3 |
| Desde95b22a23, con sus continuaciones de lectura | 1901 de2343 | 48 de120 | 6 de6 |
| Incluidos los dos preflights anteriores detenidos sin efectos | 2179 | 59 | 7 |

Desde95b22a23: una recepción677 bytes, una creación de expediente, dos materializaciones, seis solicitudes ALMACENAR (cuatro positivas/dos rechazos) y cuatro documentos nuevos. Las dos reconfirmaciones exitosas reutilizan el lote50. El primer preflight86709c05 tuvo además una reconfirmación rechazada sin efectos. No se amplían presupuestos ni se vuelve a abrir una ventana cerrada por este resultado.

Los hashes de95b22a23,c4afaca2 y9bad855f coinciden con las referencias revisadas. Los estados STOPPED anteriores permanecen intactos. La última corrida contiene cero recepciones/creaciones/confirmaciones y exactamente cuatro ALMACENAR sobre120/121. No hay ejecución nueva en esta revisión documental.

## Tiempos y comparación

| Lote/archivo | Documento | HTTP de almacenamiento |
| --- | --- | ---: |
| 29/50 (PDF677 bytes) | 16282 | 1,020954s |
| 29/52 (PDF677 bytes) | 16283 | 0,797384s |
| 50/120 (entrega suprimida) | 16284 | 1,813745s |
| 50/121 (ganador concurrente) | 16285 | 1,326951s |

Media de los dos últimos:1,570348s. El cronómetro se captura al recibir la respuesta del servidor, antes de retenerla o suprimirla; el control SQL externo no está incluido. Categoría sql_archivo:11 llamadas por guardado; sql_esquema:120, según la instrumentación. No son un conteo global de SELECT del sistema, y las categorías anidadas no deben sumarse como etapas independientes.

La comparación histórica de3.196 usa el mismo corpus Egresos/lote50/configuración y guardados siguientes, sin repetir completados: [tres corregidos](doc120-lote50-correccion-medida-20261118.md), media1,058143s frente79,432043s anteriores, y [diez siguientes](doc120-lote50-diez-20261118.md), media0,969465s. Conserva límites de corpus preparado y prueba histórica no pareada; no promete ese tiempo para PDF grandes o sellado en frío. Los dos últimos con pérdida/concurrencia y los PDF677 bytes se presentan por separado, sin atribuir su diferencia a una regresión de rendimiento.

## Tareas y límites restantes

Se verifican3.196 (independencia A/C/B y comparación histórica acotada),3.202 (decisión/pausa/recarga/árbol) y3.203 (convergencia/pérdida/carrera HTTP/revisión/privacidad). Resultado documental212/223 tareas,11 pendientes; no es aceptación integral.

3.87 sigue pendiente del recorrido actual completo Seleccionar→Preparar→Confirmar e importar y aceptación operativa: esta QA reutilizó planes/lotes confirmados.3.187 conserva rollback nativo de incidencia/cierre, contención de lote y demás condiciones no ensayadas: el solapamiento de sesiones HTTP no prueba solapamiento físico de conexiones SQL. La pérdida se inyectó después del checkpoint, no como pérdida de commit ni fallo del escritor.3.118/3.126/3.127/5.2 conservan los escenarios segmentados/capacidad total sin ejecutar.3.130/3.131 y6.4/6.5/6.6 mantienen sus condiciones administrativas/publicación/aceptación. No se modifica Jira ni se archiva DOC-120.

## Prueba manual del usuario

Puedes abrir Producción Documental→Importación→Consultar importación e indicar50. Debe mostrar25 documentos guardados y975 pendientes. Para probar la pausa visual, Continuar importación procesa los pendientes secuencialmente hasta pausar o terminar: no limita la muestra a diez. Pausar espera el acuse de la solicitud iniciada. Después recarga y consulta el mismo lote para verificar conteos/continuación; los resultados confirmados se conservan. El lote29 mantiene a51 pendiente por falta de original, por lo que sirve para revisar ese diálogo.
