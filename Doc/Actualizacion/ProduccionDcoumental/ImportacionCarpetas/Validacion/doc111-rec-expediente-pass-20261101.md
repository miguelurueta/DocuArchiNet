# DOC-111: REC-01/02 EXPEDIENTE PASS

**Aceptación real EXPEDIENTE: PASS. Tarea 13.12 completada, avance 124/130.** La sesión 18 terminó el 2026-11-01 a las 17:44:47 UTC, sin fallos y con resultado completo del driver existente. No se acredita este resultado a los otros modos ni a pruebas locales anteriores.

## Alcance autorizado y resultados

Ambiente local ya autorizado, cuenta operativa 141/empresa 2, nivel 7, perfil 24/versión publicada 37, gabinete 10, tipología 79 e índice 14/TIPO=EEE. Raíces DOC111-20261101-EXP-01/02 y dos PDF de prueba de 677 bytes, conservados con sus recursos. Autorización específica `tools/e2e/artifacts/doc111-expediente-authorization-15.json`; credenciales introducidas solo por TTY enmascarado, controles RA/DA con cuenta exclusivamente SELECT.

| Recurso | Primera raíz | Segunda raíz |
|---|---|---|
| Plan / lote | 5 / 3 | 5 / 3 |
| Raíz / archivo | 5 / 5 | 6 / 6 |
| Expediente | 650 | 651 |
| Relación con nivel 7 | 1075 | 1076 |
| Intento de almacenamiento | 12 | 13 |
| Documento | 16206 | 16207 |
| Inventario | 23295 | 23296 |

Lote 3 COMPLETADO/revisión 21, ambas raíces COMPLETADA y archivos RECIBIDO/COMPLETADO, sin reclamaciones activas. Seis intentos confirmados: dos recepciones, dos creaciones y dos almacenamientos. El diario termina en evento 52/LOTE_COMPLETADO.

## Comprobaciones acreditadas por la corrida

- `rec01` y `rec02`: dos raíces y dos documentos reales en el destino autorizado.
- `barrierPreserved`: una raíz incompleta no inicia creación y la solicitud bloqueada no cambia el snapshot de datos. En este modo la primera raíz completa puede avanzar antes de completar la recepción de la segunda.
- `idsAndIndicesVerified`: valores fijados contrastados antes del almacenamiento, SELECT de gabinete/documento/inventario/incorporación, campos e identidades, clasificación, Archivo Gestión, nombres, PDF e índices.
- `replayAndReopenUnchanged`: repetición de confirmación y de comandos ya confirmados conservando IDs; reapertura real de UI y comparación SELECT/fingerprint sin cambios, más intentos o eventos. No fue necesario recuperar resultados inciertos en esta corrida.

El runner exige todos esos indicadores, dos raíces/documentos/originales y huellas válidas para aceptar PASS; no hay retries automáticos, screenshots, traces o video de la sesión. Evidencia saneada: `tools/e2e/artifacts/doc111-expediente-session-18.json`.

## Código y contraste posterior

- HEAD: `b2e93b726f221868cf2b82d43200a8e568871117`.
- `codeSha256` de la corrida: `96c3fafdd5e6254da8a6f3fbcb994908078cdd8ebff30c1ef1809ea4a6bf8e3d`.
- `driverSha256`: `156cb2042c8c17334537970cb065360812c670c1f391efd6f08d627ee5455e2e`.
- El driver comprueba las huellas antes/después de la corrida. La lectura posterior del estado saneado y `captureCodeEvidence` confirma que las fuentes actuales coinciden con `codeSha256`. Estas huellas usan su ámbito propio, distinto del manifiesto local de 110 archivos del trabajo previo.
- La consola comprobó antes de ejecutar la DLL activa `4134f57f66fc259b5f33a553577c9a07e6bf176b2da027aaeeb8576325c282c5`; no se desplegó otra DLL para esta corrida.

SELECT posterior a las 18:34:39 UTC confirma los IDs/estados del lote 3 en `tools/e2e/artifacts/doc111-expediente-lot3-readonly.json`. SELECT a las 18:34:42 UTC confirma que el lote 2 permanece COMPLETADO/revisión 25, sin alterar sus expedientes/documentos aprobados. Estas inspecciones posteriores no ejecutaron efectos, reconciliación o limpieza.

Los recursos y originales se conservan. La aceptación integral LOTE, PARCIAL, degradación de esquema/carga individual y revisión/gobierno finales siguen pendientes; no se marca todo DOC-111 como terminado.
