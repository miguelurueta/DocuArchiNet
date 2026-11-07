# DOC-111: REC-01/02 PARCIAL PASS

Aceptación real PARCIAL: **PASS**. Sesión 25 finalizada el 2026-11-02 a las 20:55:44.054 UTC, sin fallos. Evidencia saneada `tools/e2e/artifacts/doc111-parcial-session-25.json`. Se cierra 13.14: **126/130**.

## Recursos y publicación

Autorización específica registrada en `doc111-parcial-authorization-23.json`, vinculada a la propuesta aprobada. Se creó el perfil separado **25**, código DOC111-PARCIAL-20261102, y publicó/activó su versión **39** mediante DOC-110. La configuración/reglas se contrastaron con el origen 24/38; cambia el modo a PARCIAL_PERMITIDO y el ámbito a NO_APLICA. El helper usa sesión, navegación administrativa y CSRF existentes, sin SQL de escritura. Los intentos anteriores 23/24 se detuvieron antes de crear recursos por dos errores del consumidor E2E, corregidos con 66 pruebas locales PASS.

Nivel 7, gabinete 10, tipología 79, índice 14/TIPO=EEE; raíces DOC111-20261102-PAR-01/02. Dos PDF válidos de 677 bytes y una entrada de manifiesto rechazada, sin recepción ni almacenamiento de esa tercera entrada. Cuenta/contraseñas solo por consola; controles RA/DA MembershipUsers con cuenta exclusivamente SELECT.

| Recurso | Primera raíz | Segunda raíz |
|---|---|---|
| Plan / lote | 7 / 5 | 7 / 5 |
| Raíz / archivo válido | 9 / 9 | 10 / 10 |
| Expediente | 654 | 655 |
| Relación con nivel 7 | 1079 | 1080 |
| Intento de almacenamiento | 24 | 25 |
| Documento | 16210 | 16211 |
| Inventario | 23299 | 23300 |

## Comprobaciones reales

`rec01`, `rec02`, `barrierPreserved`, `idsAndIndicesVerified` y `replayAndReopenUnchanged`: todos true. Dos raíces, dos documentos y tres archivos originales. La primera raíz elegible avanzó antes de recibir el segundo PDF; la segunda se bloqueó mientras faltaba su archivo válido, sin cambiar el snapshot. Se conservaron el rechazado y los totales originales. Valores efectivos antes del almacenamiento e índices físicos, clasificación, gabinete, inventario e incorporación se comprobaron por SELECT. Confirmación y comandos originales se repitieron conservando IDs; reapertura real y fingerprint posterior sin cambios.

SELECT posterior a las 20:56:48.822 UTC confirma lote 5 PARCIAL/revisión 21, primera raíz COMPLETADA, segunda PARCIAL y seis intentos CONFIRMADO. Archivo 11/cliente bad permanece RECHAZADO en recepción y efecto, con 677 bytes previstos y sin bytes recibidos; ambos archivos válidos están RECIBIDO/COMPLETADO y sin reclamaciones. Evidencia `tools/e2e/artifacts/doc111-parcial-lot5-readonly.json`. Esta lectura complementa los controles exclusivamente SELECT de la corrida.

## Huellas y límites

- HEAD: `b2e93b726f221868cf2b82d43200a8e568871117`.
- Código: `96c3fafdd5e6254da8a6f3fbcb994908078cdd8ebff30c1ef1809ea4a6bf8e3d`.
- Driver: `b94ded809fa2d7789089be0960068ca4e0d3d161a0170bf945e56433fbb4f4a3`.

Las huellas se contrastan antes/después del recorrido. Se conservan perfil/publicación, lote, expedientes/documentos y originales válidos. El perfil 24 y lotes 2, 3 y 4 no fueron objetivos de mutación. Los tres modos REC ya tienen aceptación propia; degradación de esquema, carga individual y revisión/gobierno final siguen pendientes. No se acredita QA final ni se declara terminado todo DOC-111.
