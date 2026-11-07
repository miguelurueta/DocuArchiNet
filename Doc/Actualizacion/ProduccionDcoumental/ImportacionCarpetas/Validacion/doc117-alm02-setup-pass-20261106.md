# DOC-117 — Preparación ALM-02 real, 2026-11-06

**PASS dentro del alcance autorizado**: plan 37/lote 21, dos originales recibidos, dos expedientes creados y dos materializaciones. **100 SELECT acumulados de 160**, cero solicitudes ALMACENAR, cero documentos definitivos y cero instalaciones de DLL. Los recursos se conservan.

Ambiente local, actor 141/empresa 2/nivel 7/versión 38/gabinete 10, según la propuesta autorizada por el usuario mediante «autorizado». Credenciales capturadas exclusivamente por consola enmascarada y descartadas por proceso; no se guardaron cuentas, contraseñas, cookies, conexiones, contexto completo ni filas SQL.

| Raíz / archivo | Expediente / relación de nivel | Original |
| --- | --- | --- |
| 33 / 34 | 664 / 1089 | PDF de 677 bytes verificado |
| 34 / 35 | 665 / 1090 | PDF de 677 bytes verificado |

Los originales `DOC117-ALM02-20261106.pdf` conservan SHA256 `a5f1924afbae4bd631de893c49a56d9f09219075113e6da2ec983602cff84c6c`, bajo las raíces propuestas A/B. Código, nombre, actor, empresa y nivel de los expedientes se contrastaron por sus IDs. Los valores quedaron materializados con tipología 79 e índice 14/TEXTO/`EEE`. Los archivos permanecen RECIBIDO/NO_INICIADO, sin intento documental ni documento/inventario/incorporación.

## Continuación del mismo plan

La primera ejecución realizó 24 SELECT y creó el plan 37. Se detuvo antes de confirmar por `DOC117_SETUP_PLAN_CHANGED`: el comparador local no contemplaba el orden canónico de las reglas y sus representaciones nulas admitidas. Se corrigió el soporte para reutilizar `comparePublishedSnapshot`, conservando IDs, valores y demás diferencias significativas.

La corrección tiene 13/13 pruebas, incluyendo orden diferente, continuación del plan conocido, cero preparaciones adicionales, bloqueo si ya se intentó confirmar y presupuesto SELECT acumulado. No se modificó producto, publicación, perfil, configuración o permisos.

Con la misma autorización de efectos se capturaron credenciales nuevas, dado que las anteriores se habían descartado. La continuación quedó ligada al diario detenido, plan 37/revisión 1, manifiesto/contexto exactos y ausencia de un lote previo para ese plan. Releyó sus datos por SELECT y confirmó **ese mismo plan**, sin volver a llamar a PrepararPlan. Consumió otros 76 SELECT: total 100/160.

La preservación de planes/lotes/raíces/archivos/intentos/eventos anteriores se comprobó antes y después de la continuación, excluyendo el plan nuevo 37 de ese conjunto. Incluye lote 18/intento 41 y lotes 19/20. No se atribuye una huella persistida que cubra el intervalo entre los dos procesos: el primer proceso no conservó su baseline en disco.

## Candidata cerrada compilada localmente

Con los IDs y revisiones reales se calcularon las huellas de las dos solicitudes futuras mediante la consola .NET revisada. El manifiesto privado conserva únicamente el alcance y comandos necesarios, sin secretos de autenticación ni contexto completo. Ninguna de esas solicitudes se envió.

Se compiló la variante específica en `.tmp/doc117-alm02-lote21-candidate-20261106`: `COMPILED_SCOPED_NOT_INSTALLED`, sin errores. Alcance SHA256 `0878b656ded3919a31b956e3378d5b94b2b4560dc9bd4503f09648583b88b323`; candidata `58674b31eedc6cc7d60cc5fe04d731d9273e5ee6fc047ad7938637ce3dfa93ec`.

La DLL real conserva `4134f57f66fc259b5f33a553577c9a07e6bf176b2da027aaeeb8576325c282c5` y los 912 archivos de entrada del proyecto mantienen sus hashes. La variante se ensayó **solo como copia de bytes** en un sitio temporal `.tmp`: guardián independiente restauró el original tras terminar el dueño sin restauración propia; ambos respaldos resultaron idénticos y las ACL se preservaron. No se cargó la DLL ni se activó en IIS.

Evidencia completa y procedencia: `doc117-alm02-setup-pass-20261106.json`. La autorización de preparación queda consumida para nuevas preparaciones; no repetir el comando ni crear otro plan/lote para recuperar estos recursos.

Regresión conjunta del soporte corregido: **51/51 PASS**, sin conexión; incluye los 13 casos de preparación/continuación, los comandos reales del harness .NET y la preservación/restauración. OpenSpec estricto PASS. No se suman a esta cuenta corridas previas de los mismos tests.

## Límites

Este PASS acredita preparación, preservación y construcción local. Las ventanas internas documentales ALM-02 y el solapamiento ALM-03 siguen **NO_EJECUTADO**. La candidata no está instalada y no existe autorización de almacenamiento/inyección. Las tareas 16.1, 17.1 y 18.1 siguen pendientes; no se archiva ni cierra DOC-117.
