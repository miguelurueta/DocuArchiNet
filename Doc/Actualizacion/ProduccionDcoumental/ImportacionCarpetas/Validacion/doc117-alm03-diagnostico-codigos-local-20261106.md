# DOC-117 — Diagnóstico local de la pérdida de error individual

Se reprodujo y corrigió una pérdida de información del ejecutor de pruebas. El helper oficial de UI genera un código cerrado en `error.code`; `dispatchPair` esperaba ambos resultados y luego sustituía cualquier rechazo por `DOC117_ALM03_PAIR_RESULT_UNCERTAIN`, descartando el motivo de cada operación. El fallo histórico conserva únicamente ese código genérico. Su causa individual exacta **continúa sin determinar**; esta corrección no reconstruye la respuesta anterior.

## Cambio implementado y probado

El ejecutor conserva `PAIR_OPERATIONS_SETTLED` con los resultados separados de `IMPORT_A` e `INDIVIDUAL`. Una promesa FULFILLED indica que terminó su función; la confirmación de negocio sigue dependiendo de respuesta, checkpoint y controles físicos. Una operación rechazada conserva únicamente un código de lista cerrada. Un código desconocido se convierte en `UNCLASSIFIED_FAILURE`; el envoltorio no conserva el error original como causa, mensaje o stack.

La UI emite un diagnóstico opcional de preparación, guarda previa, Guardar, respuesta HTTP, envelope, resultado servidor, identidades o proyección. Contiene fase, código cerrado, HTTP numérico, conteo observado de POST y banderas de resultado/IDs/proyección. No contiene cuerpo HTTP, mensaje del servidor, credenciales, cookies, URL, rutas, nombre de archivo o token. El error original conserva su precedencia si falla la escritura del diagnóstico.

Se corrigió también un camino de rechazo sin manejar: si fallaba el clic de Guardar, la espera HTTP creada antes del clic podía rechazar después del cierre de la página. Ahora esa espera tiene manejador desde su creación. La pareja sigue esperando ambos envíos, se detiene ante incertidumbre y no reintenta; B depende todavía de ambos resultados e integridad. Un diario asíncrono o fallido se rechaza sin dejar otra promesa sin manejar.

Los IDs de respuesta se validan antes del callback: solo referencias UInt32 positivas expresadas como número o cadena decimal. Se rechazan NaN textual, desbordamientos, null, arrays y objetos; no se registra un ID inválido como conocido.

## Reproducción local

Con el helper real de UI y dobles de página/respuesta, un rechazo sintético `PRODUCCION_CARGA_ALMACENAMIENTO_RECHAZADO` produce `PRODUCCION_DOCUMENTAL_E2E_STORAGE_EXCEPTION`. El coordinador corregido conserva ese código para INDIVIDUAL, junto al resultado de A, en vez de perderlo al construir el error de pareja. Otro ensayo distingue HTTP 403 de rechazo de negocio con HTTP 200; otro conserva IDs conocidos si falla la proyección. Son ensayos locales, sin servidor, login o base de datos.

Ejemplo **sintético** de información que quedaría disponible ante un rechazo de negocio:

```json
{
  "phase": "SERVER_RESULT",
  "code": "PRODUCCION_DOCUMENTAL_E2E_STORAGE_EXCEPTION",
  "httpStatus": 200,
  "serverResult": "REJECTED",
  "uploadRequestsObserved": 1,
  "identitiesObserved": false,
  "projectionVerified": false
}
```

La regresión conjunta final pasó **270/270**. Incluye el helper de UI ejecutado con dobles, códigos forjados/desconocidos, causas y cuerpos privados descartados, rechazo antes de Guardar, respuesta perdida, proyección, diario fallido/asíncrono, espera de ambos resultados y detención sin retry. También cubre los consumidores anteriores DOC-88/DOC-89/DOC-111 y los controles ODBC locales. Registro `.tmp/doc117-alm03-failure-diagnostic-final-regression-20261106.tap`; archivos y huellas en el certificado JSON adjunto.

## Alcance y procedencia

Solo cambia soporte JavaScript de pruebas y sus tests. Las 912 entradas originales del proyecto, el escritor compartido, la candidata histórica, la DLL instalada, los respaldos y los artefactos de la corrida anterior se verificaron intactos. No hubo autenticación, SELECT real, almacenamiento, instalación de DLL, replay o modificación de los recursos de A/B durante este avance. Las concesiones de ejecución anteriores quedan rechazadas por la huella de código nueva; no se creó otra concesión.

La evidencia real anterior sigue siendo `doc117-alm03-detenido-verificado-20261106.md/.json`: **146/320 SELECT**, A/intento 58 CONFIRMADO (16224/23322), individual sin documento, B NO_INICIADO, originales/recursos previos conservados y DLL original restaurada. Sus archivos y SHAs se conservan, incluida la limitación de causa no retenida.

El siguiente diagnóstico deberá usar un caso aislado y conservar el fallo individual desde el primer intento. Si el código público termina en `STORAGE_EXCEPTION`, habrá que distinguir la excepción de las guardas del preparador en una copia de laboratorio: el catch legacy puede convertirlas en un mismo código público. Este cambio no añade esa instrumentación nativa ni autoriza una instalación o carga nueva. No se identifica por inferencia qué guarda falló en la corrida anterior.

**DOC-117 permanece en 16/18, con 17.1 y 18.1 abiertas.** No se declara aceptación de concurrencia, serialización, XML/SII, cierre o archivo.
