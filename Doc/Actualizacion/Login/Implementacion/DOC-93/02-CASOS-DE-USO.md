# Casos de uso implementados

## UC-01 — Resolver SMTP deshabilitado

| Campo | Detalle |
| --- | --- |
| Actor | `EXT: consumidor interno futuro` |
| Precondiciones | Contexto de Radicación válido y acceso de solo lectura disponible. |
| Flujo principal | `Resolve` abre conexión, ejecuta el `SELECT` parametrizado y el proyector no encuentra filas. |
| Alternativos | No aplica. |
| Errores | Una excepción técnica se sanitiza como `Failed`. |
| Resultado | `SecondFactorSmtpConfigurationResolution(Disabled, Nothing)`. |
| Endpoint / Controller / Service / Repository | No aplica / No aplica / No aplica / `MySqlSecondFactorSmtpConfigurationRepository.Resolve`. |

## UC-02 — Resolver configuración única válida

| Campo | Detalle |
| --- | --- |
| Actor | Consumidor interno futuro. |
| Precondiciones | Exactamente una fila con `ESTADO_ENVIO=1`. |
| Flujo principal | Lee diez columnas; valida host, remitente, puerto, flags, credenciales y timeout; crea modelo inmutable. |
| Alternativos | Credencial `0` usa credenciales predeterminadas; `1` exige usuario y contraseña. Timeout superior a 120000 ms efectivos se acota. |
| Errores | Campo nulo, formato/rango inválido, tiempo no positivo u overflow retorna `InvalidConfiguration`. |
| Resultado | Estado `Submitted` como señal interna de configuración utilizable y `SecondFactorSmtpConfiguration`. |
| Endpoint / Controller / Service / Repository | No aplica / No aplica / No aplica / `Resolve`. |

## UC-03 — Rechazar configuración ambigua

| Campo | Detalle |
| --- | --- |
| Actor | Consumidor interno futuro. |
| Precondiciones | Al menos dos filas activas. |
| Flujo principal | El proyector lee la primera y detecta una segunda sin mapearla. |
| Alternativos | No existe selección de “primera fila”. |
| Errores | No aplica. |
| Resultado | `AmbiguousConfiguration`, sin configuración y sin cliente SMTP. |
| Endpoint / Controller / Service / Repository | No aplica / No aplica / No aplica / `Resolve`. |

## UC-04 — Entregar OTP mediante transporte

| Campo | Detalle |
| --- | --- |
| Actor | `CODE: SecondFactorSmtpEmailSender`. |
| Precondiciones | Configuración resuelta y `SecondFactorEmailMessage` válido. |
| Flujo principal | Construye `MailMessage` con asunto fijo, OTP, expiración y advertencias; crea cliente configurado; envía sincrónicamente; libera recursos. |
| Alternativos | El cuerpo respeta `ESTADO_BODY`; SSL y credenciales reflejan la tabla. |
| Errores | Remitente/destinatario inválido, fallo de fábrica o envío produce `Failed/OTP_DELIVERY_FAILED`; no retorna detalle técnico. |
| Resultado | `Submitted/OTP_SUBMITTED` o fallo sanitizado. |
| Endpoint / Controller / Service / Repository | No aplica / No aplica / `SecondFactorSmtpTransport.Send` / No aplica. |

## UC-05 — Adaptar entrega al contrato DOC-91

| Campo | Detalle |
| --- | --- |
| Actor | Consumidor de `ISecondFactorEmailSender`. |
| Precondiciones | Mensaje no nulo. |
| Flujo principal | `SecondFactorSmtpEmailSender.Send` resuelve configuración y delega únicamente si el estado es `Submitted`. |
| Alternativos | Mapea `Disabled`, `InvalidConfiguration`, `AmbiguousConfiguration` y `Failed` a códigos públicos fijos. |
| Errores | Nulos, resultado inesperado o excepción se convierten en `OTP_DELIVERY_FAILED`. |
| Resultado | `SecondFactorDeliveryResult`; solo `OTP_SUBMITTED` tiene `Success=True`. |
| Endpoint / Controller / Service / Repository | No aplica / No aplica / `SecondFactorSmtpEmailSender.Send` / puerto de configuración. |
