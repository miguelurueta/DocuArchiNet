# DOC-93 — Contratos e integración

- Ticket: DOC-93
- Cambio OpenSpec: doc-93-segundo-factor-smtp
- Clasificacion: cross_cutting

## Contratos e integraciones

No se creó endpoint, handler, DTO HTTP ni autorización web. La integración interna implementada es:

`ISecondFactorEmailSender → SecondFactorSmtpEmailSender → ISecondFactorSmtpConfigurationRepository → MySqlSecondFactorSmtpConfigurationRepository → IModuleConnectionFactory/IDataExecutor → Config_Smpt_Side → ISecondFactorSmtpTransport → SecondFactorSmtpTransport → ISecondFactorSmtpClientFactory → FrameworkSmtpClientAdapter`.

La consulta enumera `SERV_SMTP`, `PUERTO_SERV_SMTP`, `USUARIO_SMTP`, `PASW_SMTP`, `DOMINIO_SMTP`, `SMTP_TIEMPO`, `ESTADO_SSL`, `ESTADO_ENVIO`, `ESTADO_BODY` y `ESTADO_CREDENCIAL`, filtrando con parámetro `ESTADO_ENVIO=@enabled`.

No se modifica la base ni se introduce configuración paralela. Contraseña, OTP, destinatario completo, conexión y excepción quedan fuera de resultados, logs y evidencia.
