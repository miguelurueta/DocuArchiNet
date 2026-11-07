# DOC-117 ALM-03C: ejecución documental restaurada; cierre posterior pendiente

Este informe fija el resultado histórico de la corrida ec82e738600489f9ec7837a32eca1525, terminada el 2026-11-06T23:51:41.124Z. No declara PASS integral: el proceso salió 2 por DOC111_LOGIN_INPUT_REQUIRED durante la reapertura con DLL original, después de restaurarla. Su resultado interno de documentos y observación fue PASS, pero businessEvidenceVerified y alm03ConcurrentPass permanecieron false al no completarse los controles posteriores. Una comprobación independiente posterior debe aportar su propio certificado, sin modificar este artefacto.

El usuario autorizó expresamente hasta tres documentos, candidata temporal por600s y hasta320 SELECT adicionales desde339, con techo659. Se usó lote23/plan39 y los expedientes ya preparados668/669, sin repetir preparación o casos anteriores. Se enviaron A/individual en sesiones independientes; ambos resultados y sus controles físicos pasaron antes de enviar B una sola vez.

| Recurso | Documento | Inventario | Intento | Expediente |
| --- | --- | --- | --- | --- |
| Importación A | 16225 | 23323 | 63 CONFIRMADO | 668 |
| Individual | 16226 | 23324 | No aplica | 467 |
| Importación B | 16227 | 23325 | 64 CONFIRMADO en respuesta | 669 |

Las marcas internas completas corresponden a tres entradas y tres salidas exitosas, con recursos/solicitudes e intentos correlacionados. La pareja tuvo997347 ticks a10MHz de solapamiento, equivalentes a99,7347ms dentro del método instrumentado. Esto no acredita commits simultáneos, rendimiento del binario ordinario ni XML/SII. No hubo registro nativo de rechazo de guardas.

El fallo posterior conservado tiene postback HTTP200, cuerpo disponible y postedFields=DIFFERENT: el servidor pidió un dato obligatorio y el formulario enviado no coincidió con las entradas efímeras esperadas. No se determina qué campo cambió ni se afirma contraseña inválida, pérdida de secretos o causa de UI concreta. No se conserva formulario, contraseña, cookie o cuerpo en la evidencia. failedAt=RESTORATION_FINALLY_VERIFIED refleja la última etapa del finally, no un fallo de restauración: el diario muestra ORIGINAL_RUNTIME_RESTORED y RESTORATION_FINALLY_VERIFIED/restored=true antes de terminar.

La DLL instalada y ambos respaldos nuevos están verificados byte a byte como el original4134f57f…. Las912 entradas originales y las fuentes nativas revisadas permanecen intactas. **104 SELECT de ejecución,443/659 acumulados**; no se reinicia contador. No se han repetido cargas ni se ha ejecutado reparación, reconciliación, limpieza o consolidación del agregado.

Se preparó un cierre independiente más acotado con la autorización existente: una reapertura autenticada con original y solo SELECT, cero documentos o cambios de DLL, máximo215 consultas adicionales desde443 y techo659. Requiere reingresar secretos en consola porque la corrida anterior terminó. La base individual solo se aceptará si su reconstrucción por SELECT, excluyendo exclusivamente16226/23324 identificado por la salida nativa, coincide con la huella completa previa; ningún valor se fabricará o guardará. Cuatro pruebas locales del filtro cerrado PASS, separadas de la regresión anterior355/355.

Fuente inmutable: tools/e2e/artifacts/doc117-alm03c-coexistence-ec82e738600489f9ec7837a32eca1525.json. Autorización .tmp/doc117-alm03c-coexistence-authorization.json. Revisión previa doc117-alm03c-coexistencia-local-20261106.json. Este snapshot conserva **16/18**,17.1/18.1 abiertas hasta la evidencia posterior; no se archiva ni se cierra Jira.
