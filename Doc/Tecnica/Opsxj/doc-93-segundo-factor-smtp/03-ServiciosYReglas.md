# DOC-93 — Servicios y reglas

- Ticket: DOC-93
- Cambio OpenSpec: doc-93-segundo-factor-smtp
- Clasificacion: cross_cutting

## Servicios y reglas

`SecondFactorSmtpEmailSender` conserva `ISecondFactorEmailSender.Send(SecondFactorEmailMessage) As SecondFactorDeliveryResult` y coordina `ISecondFactorSmtpConfigurationRepository` con `ISecondFactorSmtpTransport`.

La configuración exige exactamente una fila activa. Cero filas produce `Disabled`; más de una, `AmbiguousConfiguration`; campos inválidos, `InvalidConfiguration`; aceptación SMTP, `Submitted`; excepción técnica, `Failed`. El timeout mantiene `SMTP_TIEMPO * 100000` con overflow comprobado y máximo 120000 ms. `DOMINIO_SMTP` se lee pero no forma parte de `NetworkCredential`.

El OTP se construye como texto plano. La vigencia se recibe y conserva en UTC, pero el texto del correo convierte exclusivamente su presentación a hora Colombia (`UTC-5`) y formato de 12 horas con `a. m.`/`p. m.`. El código aparece sin puntuación adyacente para facilitar su lectura y copia.

El remitente conserva la dirección configurada en `USUARIO_SMTP` y presenta el nombre visible legacy `Sistema de gestión Docuarchi.net web`, igual que `ClassCorreo`. Este nombre no proviene de `DOMINIO_SMTP` ni cambia según el módulo.

No hay fallback al correo legacy, decisiones de challenge, autenticación, cooldown, reenvío o fire-and-forget.
