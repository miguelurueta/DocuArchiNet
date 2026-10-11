# DOBLE-FACTOR-ORQUESTACION

- Ticket: DOC-95
- Cambio OpenSpec: doc-95-doble-factor-orquestacion
- Clasificacion: cross_cutting (Transversal)
## Evidencia requerida

- [x] unit: `node --test tests/login-second-factor-smtp.test.cjs tests/doc93-technical-documentation.test.cjs tools/e2e/tests/doc95-login-second-factor-policy.test.cjs`; 12/12 aprobadas; 2026-10-11.
- [x] manual_qa: runner E2E DOC-95 en TTY autorizado; resultado saneado `DOC95_E2E_RESULT=PASSED`; 2026-10-11.

## QA/E2E WebForms

Se ejecutó `npm.cmd --prefix tools/e2e run test:doc95:second-factor -- --profile profiles/doc95-login-second-factor.local.runtime.json` en TTY autorizado. La prueba confirmó postback de login, estado pendiente, destino enmascarado, exactamente un ganador entre dos verificaciones concurrentes y cookie únicamente después de completar el segundo factor. Resultado: `PASSED`. La evidencia excluye credenciales, OTP, correo, cookie, challenge y configuración de conexión.

Se ejecutaron además `--scenario expiry` y `--scenario verify` en corridas independientes. Expiración confirmó limpieza del estado y ausencia de cookie; verificación confirmó aceptación de un OTP vigente y cookie posterior. Ambos resultados fueron `PASSED`. El correo presenta vencimiento en hora Colombia y formato de 12 horas, aunque toda decisión de vigencia continúa basada en UTC.

La corrida completa `--scenario concurrency` se repitió después de ajustar la presentación del remitente y del vencimiento. Confirmó nuevamente `DOC95_CONCURRENCY_WINNERS=1`, cookie posterior y `DOC95_E2E_RESULT=PASSED`, sin almacenar el OTP.

Una corrida anterior no observó Session pendiente y tampoco creó challenge. El control saneado confirmó configuración, módulo, principal y correo válidos, pero el código de entonces no conservaba la etapa de la excepción. Se incorporó `ISecondFactorDiagnosticSink` con correlación y catálogos cerrados para ubicar recurrencias sin registrar identidad, OTP, challenge, conexiones ni excepciones. Las pruebas locales confirman propagación de la misma correlación y que un fallo del sink no altera el login. El adaptador primario escribe solo resultados terminales saneados como evento 9501 en Windows `Application` y usa `Trace` como fallback. Quedan como puertas externas verificar escritura/retención con la identidad del AppPool y repetir varias sesiones nuevas después de un reciclado autorizado.

El runner dispone de `--scenario stability`: tres sesiones secuenciales independientes, cada una con challenge, correo y OTP nuevo. Después de autorización explícita se recicló solo el AppDomain de la aplicación, no el `DefaultAppPool` compartido, y se ejecutó el escenario en TTY visible. Las tres iteraciones aprobaron y finalizaron con `DOC95_STABILITY_SESSIONS=3` y `DOC95_STABILITY_RESULT=PASSED`.

La consulta posterior, de solo lectura y saneada, encontró 6 eventos 9501, 3 correlaciones distintas, 6 resultados `SUCCEEDED`, 0 `FAILED` y 0 `REJECTED`. Los GUID y mensajes no se imprimieron ni conservaron. Esto valida escritura y lectura en el host de pruebas; otros ambientes deben comprobar sus propios permisos y retención.
