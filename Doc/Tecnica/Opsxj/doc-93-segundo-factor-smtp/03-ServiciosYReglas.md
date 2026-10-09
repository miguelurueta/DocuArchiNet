# DOC-93 — Servicios y reglas

- Ticket: DOC-93
- Cambio OpenSpec: doc-93-segundo-factor-smtp
- Clasificacion: cross_cutting

## Servicios y reglas

`SecondFactorSmtpEmailSender` conserva `ISecondFactorEmailSender.Send(SecondFactorEmailMessage) As SecondFactorDeliveryResult` y coordina `ISecondFactorSmtpConfigurationRepository` con `ISecondFactorSmtpTransport`.

La configuración exige exactamente una fila activa. Cero filas produce `Disabled`; más de una, `AmbiguousConfiguration`; campos inválidos, `InvalidConfiguration`; aceptación SMTP, `Submitted`; excepción técnica, `Failed`. El timeout mantiene `SMTP_TIEMPO * 100000` con overflow comprobado y máximo 120000 ms. `DOMINIO_SMTP` se lee pero no forma parte de `NetworkCredential`.

No hay fallback al correo legacy, decisiones de challenge, autenticación, cooldown, reenvío o fire-and-forget.
