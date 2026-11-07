# DOC-117 ALM-03: diagnóstico autorizado detenido en preparación inicial

La autorización específica del usuario se registró con ID `188de3a60bbbc41d97cd4ccf3fa04266`, vinculada a la propuesta revisada, código `901f59d2…`, alcance `364bd144…` y candidata `8952bd88…`. La consola comenzó el 2026-11-06 a las 19:56:19 UTC y terminó a las 19:58:42 UTC, salida 2 y estado `STOPPED_REVIEW_REQUIRED`.

**No se alcanzó el diagnóstico nativo.** Se consumieron cero SELECT adicionales, por lo que el acumulado permanece en **146**. No se preparó la ventana de runtime, no se instaló la candidata y no se envió el PDF temporal. La DLL instalada sigue siendo la original; los directorios de lease, diagnóstico y reserva de esta corrida y su temporal nuevo no existen. No hubo segundo intento, replay, reconciliación, reparación o limpieza.

El registro conservó `DOC117_ALM03_PROBE_STOPPED` / `UNCLASSIFIED_FAILURE`, con `runtimeOriginalVerified=true` e historial vacío. El código permite acotar el fallo al preflight inicial, antes de sus consultas de control, pero no distinguir apertura de sesión, selección de nivel o lectura de lote. No se afirma que las credenciales fueran incorrectas ni que la sesión llegara a establecerse. No se recuperó el detalle del error y no se inventa su causa.

La última comprobación real de negocio sigue siendo la lectura anterior: A confirmado, documento 16224/inventario 23322/intento 58; individual sin documento del caso y B sin iniciar. Esta corrida no volvió a consultar esos datos y no acredita una nueva comprobación de preservación en MySQL. Sus artefactos anteriores permanecen intactos por SHA.

## Corrección local posterior

El catch del ejecutor no conservaba códigos DOC111 de sesión ni DOC116 de selección de nivel, aunque el helper ya los producía. Se agregó una clasificación de lista cerrada y etapas durables para apertura, selección, lectura del lote y baseline. La observación de login conserva exclusivamente HTTP y enumeraciones validadas del helper existente. Los únicos metadatos operacionales adicionales admitidos son los de `ConsultarLote`, con HTTP válido y código público de lista cerrada. Se descartan mensajes libres, cuerpos, stack, causa, credenciales, cookies, tokens y rutas.

Regresión conjunta posterior: **289/289 PASS**, sin sumar las pruebas enfocadas o regresiones históricas. OpenSpec estricto y `--check` PASS. Las 912 entradas originales de compilación/recursos, DLL instalada, ambos respaldos históricos, candidata nativa y evidencia anterior siguen intactos. La candidata no se recompiló ni se instaló por esta corrección JavaScript.

Código corregido: `89d2145c3891caa691f50f96bb91dc30ed8ec76cf7a7a5209c8cab8615f7f405`. La concesión anterior queda como evidencia de la corrida terminada y se rechaza con este código; no se creó otra ni se volvió a autenticar. La corrección permitirá conservar el fallo de una futura apertura expresamente indicada, pero no recupera retroactivamente el ocurrido.

## Evidencia

- Corrida original: `tools/e2e/artifacts/doc117-alm03-guard-probe-188de3a60bbbc41d97cd4ccf3fa04266.json`.
- Autorización, sin secretos: `.tmp/doc117-alm03-guard-probe-authorization.json`.
- Salida de consola: `.tmp/doc117-alm03-guard-probe-console-exit.json`.
- Regresión: `.tmp/doc117-alm03-preflight-evidence-regression-20261106.tap`.
- Certificado de verificación independiente: `doc117-alm03-guard-diagnostic-detenido-20261106.json`.

**DOC-117 permanece 16/18, tareas 17.1 y 18.1 abiertas.** El diagnóstico individual y la coexistencia real siguen pendientes; esta detención no aporta PASS de guardas, escritor, serialización o concurrencia, ni permite cierre/archivo o aceptación XML/SII.
