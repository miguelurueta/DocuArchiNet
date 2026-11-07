# DOC-117 — ALM-02 interno real PASS, 2026-11-06

El usuario autorizó expresamente el alcance revisado mediante «autorizado». Ejecución real finalizada con PASS a las `2026-11-06T12:10:09.600Z`, en `https://localhost/GestionDocumental-Docuarchi.net/`, actor 141, empresa 2, nivel 7, versión 38, gabinete 10. Autorización propia `8acc6c159c162668302fb82905560118`; no se heredó la de setup ni DOC-116.

## Resultado por ventana

| Ventana | Recursos | Resultado acreditado |
| --- | --- | --- |
| A, fallo posterior al checkpoint confirmado | Plan 37/lote 21, raíz 33/archivo 34, expediente 664/relación 1089; intento 52, documento 16222, inventario 23320 | Checkpoint persistido CONFIRMADO, archivo COMPLETADO y respuesta confirmada pese a la excepción inyectada antes del retorno al adaptador. Replay idéntico una vez: mismos IDs, snapshot, eventos, prueba física y marcas; sin otra entrada al escritor. |
| B, fallo después de verificar el efecto y antes del checkpoint | Mismo plan/lote, raíz 34/archivo 35, expediente 665/relación 1090; intento 53, documento 16223, inventario 23321 | Documento/inventario/incorporación existentes por IDs, resultado público INCIERTO, intento y archivo persistidos EN_CURSO sin checkpoint final. B no se repitió, reconcilió ni reparó. El lote permanece EN_CURSO. |

Se efectuaron exactamente **tres solicitudes ALMACENAR y dos entradas al escritor**, una por archivo. El inventario del actor/empresa pasó de 8811 a 8813: dos documentos nuevos. Se usaron **191/240 SELECT**, incluyendo los controles previos y posteriores a restaurar. En esta fase no se crearon planes, lotes, recepciones, expedientes ni materializaciones; se usaron los recursos preparados anteriormente.

La verificación física correlacionó gabinete, expediente/relación, documento, inventario, incorporación, nombre/extensión PDF, tipología 79 e índice 14 TEXTO `EEE`. Los originales de los archivos 34/35 permanecen con 677 bytes cada uno y SHA-256 `a5f1924afbae4bd631de893c49a56d9f09219075113e6da2ec983602cff84c6c`. Todos los recursos se conservaron, sin limpieza.

## Runtime y preservación

Candidata exclusiva de laboratorio instalada temporalmente: SHA-256 `85824b255a826664a8395754d8903e51218ba1a8303595ee6e181c8c0dc05a95`. Guardián independiente armado antes de activar, límite configurado de 600 segundos y doble respaldo verificado. DLL original restaurada, comprobada en finally y tras reabrir sesión: `4134f57f66fc259b5f33a553577c9a07e6bf176b2da027aaeeb8576325c282c5`. Ambas copias de respaldo conservan ese mismo SHA.

La sesión nueva con el runtime original volvió a comprobar A/B, las identidades físicas, ambos originales y el conteo documental. Las huellas de planes hasta 36 y recursos de lotes hasta 20 coincidieron antes/después de la corrida, incluidos lotes 18/19/20 e intento 41. No se modificaron permisos, configuración, esquema, almacenamiento compartido, XML o SII.

## Procedencia y límites

Evidencia estructurada con huellas de autorización, propuesta, soporte, candidata, marcadores, respaldos y preservación: `doc117-alm02-interno-pass-20261106.json`. Artefacto de ejecución saneado: `tools/e2e/artifacts/doc117-alm02-internal-8acc6c159c162668302fb82905560118.json`. No contiene credenciales, cookies, conexiones ni claves de operación. La revisión previa local (134/134 pruebas y ensayo aislado de restauración) mantiene su alcance histórico en `doc117-alm02-interno-local-20261106.json`.

Se acreditan estas dos ventanas mediante una variante instrumentada, sin presentarlas como corte de red, caída de servidor o ejecución del binario ordinario. La ausencia de checkpoint de B no implica ausencia de incorporación ni habilita repetir/reparar. No se afirma coexistencia ALM-03 ni aislamiento XML/SII. Tarea 16.1 completa; progreso 16/18. Permanecen pendientes 17.1 y 18.1, sin cierre o archivo de DOC-117.
