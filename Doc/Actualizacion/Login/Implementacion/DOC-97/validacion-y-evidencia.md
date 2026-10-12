# Validación y evidencia DOC-97

## Línea base y pruebas locales

| Comando | Resultado |
| --- | --- |
| `node --test tests/login-second-factor-regression.test.cjs tests/login-second-factor-preauthentication.test.cjs tests/login-second-factor-authentication.test.cjs` | PASS, 17/17 |
| `npm.cmd --prefix tools/e2e run test:doc96:policy` | PASS, 21/21 |
| `npm.cmd --prefix tools/e2e run test:doc97:policy` | PASS, 3/3 |
| Suite completa de contratos, comportamiento y documentación 2FA | PASS, 63/63 |
| `msbuild.exe GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug /m /nologo /verbosity:minimal` | PASS, código 0; advertencias legacy de binding de ensamblados |

Las pruebas nuevas validan manifiesto, cuatro módulos, OFF/NULL, activo, errores, límites, binding de Session, aislamiento empresa/módulo/usuario, replay, concurrencia, secretos, orden de cierre e invariancia legacy.

## E2E real autorizada

El primer intento terminó `DOC95_PENDING_STATE_NOT_OBSERVED`. El diagnóstico `SELECT` saneado mostró que la cuenta escrita no coincidía con el principal. Repetido con la misma cuenta del login, confirmó módulo, SMTP, identidad y correo válidos.

La siguiente corrida produjo:

- postback correcto;
- estado pendiente y destino enmascarado;
- exactamente un ganador entre dos verificaciones;
- cookie presente solo después de completar;
- resultado final aprobado.

No se almacenaron OTP, credenciales, cookies, respuestas ni conexiones.

## Bloqueos declarados

No se ejecutó E2E activa en DocuArchi, Radicación o Workflow porque hacerlo habría requerido alterar flags/configuración o disponer de precondiciones adicionales. DOC-97 prohíbe fabricar esos casos. La cobertura conductual local sí los incluye.

## Límite de la evidencia

Las comprobaciones estructurales demuestran existencia y correspondencia de símbolos; no prueban por sí solas fidelidad completa del comportamiento. La E2E aprobada demuestra el recorrido activo de GESTOR, no todos los estados reales de los cuatro módulos.
