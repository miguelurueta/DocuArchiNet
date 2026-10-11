# DOBLE-FACTOR-ORQUESTACION

- Ticket: DOC-95
- Cambio OpenSpec: doc-95-doble-factor-orquestacion
- Clasificacion: cross_cutting (Transversal)
## Servicios y reglas

`SecondFactorPreAuthenticationService.Execute` conserva la rama sin 2FA y, cuando la configuración lo exige, delega la creación del reto en `SecondFactorAuthenticationService.Start`. Este servicio también implementa `GetStatus`, `Verify`, `Resend` y `Cancel` sobre puertos de reloj, OTP, HMAC, challenge, destinatario, correo, sesión, finalizador, cookie, ruta y diagnóstico.

El OTP debe contener seis dígitos, expira según `SegundoFactorTiempoExpira`, admite como máximo cinco intentos y dos reenvíos con cooldown de 60 segundos. La verificación adquiere el challenge de forma exclusiva antes de finalizar el login; solo un ganador puede emitir la cookie. Los fallos internos de HMAC no consumen intentos.

`LoginSecondFactorPresentationMapper` limita los códigos y descripciones públicas y colapsa errores no reconocidos a `SECOND_FACTOR_UNAVAILABLE`. `ISecondFactorDiagnosticSink` registra correlación, etapa, resultado y código cerrado sin identidad, OTP, challenge, correo, conexión ni texto de excepción; Event Log 9501 usa `Trace` como fallback y nunca cambia el resultado funcional.
