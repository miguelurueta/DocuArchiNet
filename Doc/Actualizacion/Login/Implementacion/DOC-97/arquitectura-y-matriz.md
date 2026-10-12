# Arquitectura y matriz DOC-97

## Flujo verificado

`gestor.Button1_Click` → `SecondFactorPreAuthenticationService.Execute` → resolución central y principal → `SecondFactorAuthenticationService.Start` → repositorio de challenge → SMTP → Session pendiente → `WebServiceLoginSegundoFactor`/UI → `Verify` → adquisición exclusiva → finalizador legacy → confirmación → cookie y ruta local.

No se emiten cookie, permisos ni finalización antes del OTP. El cliente solo aporta `codigo` a `Verificar`; la identidad procede de Session y repositorios.

## Convención de evidencia

- `PASSED-LOCAL`: harness determinista sin infraestructura real.
- `PASSED-E2E`: recorrido real autorizado con salida saneada.
- `BLOCKED-E2E`: el ambiente no ofrece la precondición sin cambiar flags o datos no autorizados.

## Matriz por módulo

| Módulo | OFF/NULL local | ON/correo local | Correo/configuración/fallo local | OTP/límites/concurrencia local | E2E DOC-97 |
| --- | --- | --- | --- | --- | --- |
| DocuArchi | PASSED-LOCAL | PASSED-LOCAL | PASSED-LOCAL | PASSED-LOCAL | BLOCKED-E2E: no se alteró el flag |
| Gestor | PASSED-LOCAL | PASSED-LOCAL | PASSED-LOCAL | PASSED-LOCAL | PASSED-E2E: pendiente, enmascarado, ganador único y cookie final |
| Radicación | PASSED-LOCAL | PASSED-LOCAL | PASSED-LOCAL | PASSED-LOCAL | BLOCKED-E2E: no se alteró el flag |
| Workflow | PASSED-LOCAL | PASSED-LOCAL | PASSED-LOCAL | PASSED-LOCAL | BLOCKED-E2E: no se alteró el flag |

La regla legacy de estado permanece únicamente en Gestor y Workflow. DocuArchi y Radicación conservan sus contratos históricos.

## Seguridad verificada

- Identidad canónica: empresa, módulo, tipo de usuario e ID interno.
- Binding de Session y replay consumido rechazados.
- Máximo cinco intentos, cooldown de 60 segundos y dos reenvíos.
- `AuthPayloadJson` se inserta `NULL`; OTP solo se persiste protegido.
- Comparación HMAC de tiempo constante.
- DTO, DOM y evidencia sin challenge, nonce, correo completo, conexión o credenciales.
- Ruta final fija y local.
- `ClassCorreo` y recuperación permanecen invariantes.

## Riesgo residual

La adquisición/confirmación del challenge y los efectos del finalizador legacy no participan en una transacción distribuida. Un fallo parcial del legado puede exigir reiniciar el login y no admite compensación automática completa.
