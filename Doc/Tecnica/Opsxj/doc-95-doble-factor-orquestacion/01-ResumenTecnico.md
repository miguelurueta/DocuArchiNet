# DOBLE-FACTOR-ORQUESTACION

- Ticket: DOC-95
- Cambio OpenSpec: doc-95-doble-factor-orquestacion
- Clasificacion: cross_cutting (Transversal)
## Objetivo

Implementar el ciclo completo de segundo factor por correo después de validar las credenciales legacy y antes de emitir la cookie de Forms Authentication. La solución orquesta inicio, consulta de estado, verificación, reenvío y cancelación mediante `SecondFactorAuthenticationService`, y publica una frontera ASMX que obtiene la identidad exclusivamente de la sesión del servidor.

## Alcance y compatibilidad

- Se incorporaron modelos, puertos, servicio de aplicación, adaptadores WebForms, diagnóstico saneado y `WebServiceLoginSegundoFactor.asmx`.
- `ClassGestorSesion` conserva la autenticación existente cuando el módulo no exige 2FA y usa `FinalizeLogin` como única finalización legacy después de validar el OTP.
- Se reutilizan `gestor_modulos`, los repositorios de principales, `Config_Smpt_Side` con `ESTADO_ENVIO = 1` y `ra_auth_second_factor_challenge`; no se agregan tablas ni columnas.
- Los módulos Docuarchi, Radicación, Workflow y Gestor siguen resolviendo su principal mediante las implementaciones existentes.
- La reversa funcional consiste en desactivar `RequiereSegundoFactor` por módulo o retirar los artefactos DOC-95; no requiere reversa SQL.
- El riesgo residual documentado es Session InProc: requiere afinidad o una sesión compartida si el despliegue usa más de un proceso.
