# DOC-120 — preparación de medición real,2026-11-17

**PREPARADO; SIN EJECUCIÓN REAL.** La solicitud autoriza medir tres documentos existentes del lote50; instalación de la instrumentación pendiente de aprobación y credenciales de consola pendientes. [Plan, alcance y límites](doc120-medicion-tres-plan-real-20261117.md). No hay tiempos reales nuevos ni documentos guardados por este corte.

La captura usa Server-Timing por solicitud ALMACENAR después de las guardas existentes y cuando se pide X-DOC120-Medicion. Mantiene el JSON, las comprobaciones físicas, los permisos, las reservas y el escritor compartido. Incluye costo de autoridad/metadatos/comprobación física en ambas pasadas, original/copia/verificaciones V2, persistencia intermedia, writer, confirmación y lectura final. Son intervalos anidados y conteos de llamadas; no tiempos SQL individuales ni bytes físicos exactos.

El ejecutor limita a tres IDs originales recibidos/pendientes con valores ya fijados. Espera una respuesta antes del siguiente; timeout0 únicamente en ALMACENAR y sin retry. Preserva huella de los997 restantes y resultados confirmados, comprueba los IDs conocidos en DA y exige binario/runner sin cambios. No confirma planes, recibe PDF, crea expedientes, reanuda, reconcilia o borra. La existencia de otra ventana requiere revisión, sin reset de presupuestos.

**8/8 pruebas Node únicas:**6 del ejecutor y2 wrappers Framework/DDL; los casos internos no se suman. Cubren clasificación y saneamiento de métricas, nombres V2, selección/huellas, campos y operaciones ajenas, replay, transporte pendiente, respuesta perdida, padre/revisión/actividad y aislamiento de HttpContext. Framework prueba además emisión de Server-Timing al transporte y conservación del retorno/excepción. Comandos:

```text
node --test tests/doc120-medicion-tres.test.cjs
node --test tests/doc110-perfil-importacion-behavior.test.cjs
```

Proyecto VB compilado con targetCompile y salidas aisladas en `.tmp/doc120-three-build/`:0 errores, advertencias del proyecto conservadas. OpenSpec estricto y auditoría de refinamiento15/15.3.165–3.168 completas localmente;3.169 abierta:177/189 tareas y67 escenarios, conservando las once abiertas anteriores.

DLL instrumentada SHA-256 `f667ea0ffe1b6f2ef111ac5afd52f50bab92c5edcd340a0421b9ea0931aef689`; la activa observada sigue `234268c3dddabc2996b871718af508be8ab4a86842a4af770aa44535b78a319a`. No se instaló ni cambió Web.config, listeners globales, tablas, política, writer compartido o interfaz. Se preparó la aprobación del despliegue con respaldo y una comprobación HTTP anónima; no equivale a haberlo ejecutado.

[Manifiesto de preparación y huellas](doc120-medicion-tres-preparacion-20261117.json).0 HTTP reales/0 SELECT reales/0 sesiones autenticadas/0 efectos en este corte. El video previo conserva su valor de observación visual, sin atribuirle tiempos internos del servidor. El informe de tres archivos solo se producirá a partir de la ejecución real y sus controles; no se inventan filas ni promedios.
