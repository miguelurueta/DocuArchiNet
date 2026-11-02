# DOC-111: REC-01/02 LOTE PASS

Aceptación real por LOTE: **PASS**. La sesión 22 terminó el 2026-11-02 a las 20:32:35.151 UTC sin fallos. Se cierra la tarea 13.13; avance **125/130**. Evidencia saneada: `tools/e2e/artifacts/doc111-lote-session-22.json`. Autorización específica: `tools/e2e/artifacts/doc111-lote-authorization-22.json`.

## Alcance y resultados

Ambiente local autorizado, nivel 7, perfil 24/versión 38, gabinete 10, tipología 79 e índice 14/TIPO=EEE. Dos raíces DOC111-20261102-LOTE-01/02 y dos PDF de prueba de 677 bytes. Credenciales solo por consola; controles RA/DA mediante MembershipUsers con cuenta exclusivamente SELECT. Recursos conservados.

| Recurso | Primera raíz | Segunda raíz |
|---|---|---|
| Plan / lote | 6 / 4 | 6 / 4 |
| Raíz / archivo | 7 / 7 | 8 / 8 |
| Expediente | 652 | 653 |
| Relación con nivel 7 | 1077 | 1078 |
| Intento de almacenamiento | 18 | 19 |
| Documento | 16208 | 16209 |
| Inventario | 23297 | 23298 |

La corrida acredita `rec01`, `rec02`, `barrierPreserved`, `idsAndIndicesVerified` y `replayAndReopenUnchanged`, todos true. La barrera global bloqueó creación antes de recibir ambos archivos sin cambiar el snapshot. La confirmación y los comandos originales de creación/materialización/almacenamiento se repitieron en la misma ejecución; los IDs se conservaron y el fingerprint SELECT permaneció idéntico tras repetición y reapertura real de la UI. Los valores efectivos se comprobaron antes de almacenar y los índices físicos, clasificación, gabinete, inventario e incorporación se contrastaron al finalizar.

SELECT posterior del 2026-11-02 a las 20:33:56.719 UTC confirma lote 4 COMPLETADO/revisión 21, raíces COMPLETADA sin reclamaciones y seis intentos CONFIRMADO: dos recepciones, dos creaciones y dos almacenamientos. Evidencia: `tools/e2e/artifacts/doc111-lote-lot4-readonly.json`. Esta lectura posterior complementa la corrida; no sustituye sus controles con cuenta exclusivamente SELECT.

## Huellas y límites

- HEAD: `b2e93b726f221868cf2b82d43200a8e568871117`.
- Código: `96c3fafdd5e6254da8a6f3fbcb994908078cdd8ebff30c1ef1809ea4a6bf8e3d`.
- Driver: `0f7e4378d438eaf462be85303425c8c0c688f0d7f2a25ad5f8b6110dc9ef8865`.

El driver comprueba las huellas antes y después del recorrido. No se atribuye este PASS al replay pendiente de la antigua sesión del lote 2: la aceptación LOTE corresponde a la nueva corrida completa plan 6/lote 4. Los lotes 2 y 3 se conservaron; no se invocaron operaciones sobre ellos en esta corrida. PARCIAL, degradación de esquema, regresión individual real y revisión final siguen pendientes. No se declara completado todo DOC-111.
