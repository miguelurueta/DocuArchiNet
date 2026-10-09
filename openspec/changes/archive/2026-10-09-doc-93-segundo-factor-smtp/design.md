<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09,D-10 -->
# Diseño técnico — DOC-93

## Context

DOC-93 agrega un adaptador SMTP exclusivo para OTP sobre la configuración existente de Radicación. Permanece desacoplado del login productivo y conserva intactos `ClassCorreo`, `ClassRaEnvioCorrespondencia`, recuperación de contraseña y sus llamadores.

## D-01 — Alcance inactivo

No se modifica Presentation, ASMX, Session, Forms Authentication ni persistencia de challenges. Ningún archivo productivo previo referenciará las clases nuevas salvo contratos compartidos extendidos de forma compatible.

## D-02 — Fachada compatible con DOC-91

`SecondFactorSmtpEmailSender` implementará `ISecondFactorEmailSender.Send(message As SecondFactorEmailMessage) As SecondFactorDeliveryResult`. Resolverá configuración, delegará el envío y mapeará el estado tipado a `Success`/`PublicCode` sin retirar firmas.

## D-03 — Modelos y resultados

En `Modelo/Login/SegundoFactor/SegundoFactorSmtpModels.vb`:

- `SecondFactorSmtpDeliveryStatus`: `Submitted`, `Disabled`, `InvalidConfiguration`, `AmbiguousConfiguration`, `Failed`.
- `SecondFactorSmtpConfiguration`: host, puerto, remitente, usuario, contraseña, dominio, timeout efectivo, SSL, credenciales predeterminadas y cuerpo HTML; propiedades `ReadOnly`.
- `SecondFactorSmtpConfigurationResolution`: estado y configuración opcional; nunca excepción.
- `SecondFactorSmtpDelivery`: estado y código público sanitizado.

En `SegundoFactorInterfaces.vb` se agregarán:

- `ISecondFactorSmtpConfigurationRepository.Resolve() As SecondFactorSmtpConfigurationResolution`.
- `ISecondFactorSmtpTransport.Send(configuration As SecondFactorSmtpConfiguration, message As SecondFactorEmailMessage) As SecondFactorSmtpDelivery`.

## D-04 — Repositorio MySQL

`MySqlSecondFactorSmtpConfigurationRepository` estará en `Infrastructure/Repositories/Login/SegundoFactor/`. Constructor: `New(connections As IModuleConnectionFactory, executor As IDataExecutor, radicacionContext As ContextoModulo)`. Copiará defensivamente el contexto.

SQL: `SELECT SERV_SMTP, PUERTO_SERV_SMTP, USUARIO_SMTP, PASW_SMTP, DOMINIO_SMTP, SMTP_TIEMPO, ESTADO_SSL, ESTADO_ENVIO, ESTADO_BODY, ESTADO_CREDENCIAL FROM Config_Smpt_Side WHERE ESTADO_ENVIO=@enabled` con `@enabled=1`. Quedan prohibidos `SELECT *`, `LIMIT 1`, `conect`, Session y `HttpContext`.

## D-05 — Cardinalidad fail-closed

El projector contará hasta dos filas. Cero devuelve `Disabled`; una se mapea y valida; una segunda devuelve `AmbiguousConfiguration`. Nunca expone una fila parcial ni escoge la primera.

## D-06 — Validación previa

Son inválidos: host/remitente vacíos; remitente sin formato; puerto fuera de 1..65535; banderas fuera de 0/1; `SMTP_TIEMPO <= 0`; credenciales explícitas sin usuario o contraseña. La validación ocurre antes de `MailMessage`/`SmtpClient`. Credencial 0 usa defaults y 1 usa usuario/contraseña.

## D-07 — Timeout y dominio

Se calcula `Checked(SMTP_TIEMPO * 100000)` y después `Math.Min(valor, 120000)`. Overflow produce `InvalidConfiguration`. `DOMINIO_SMTP` se conserva, pero no se pasa a `NetworkCredential`.

## D-08 — Transporte síncrono

`SecondFactorSmtpTransport` y el adaptador de framework estarán en `Infrastructure/Login/SegundoFactor/Smtp/`. Asunto fijo y cuerpo mínimo: OTP, expiración, no compartir e ignorar si no inició el acceso. Sin challenge, login, IDs ni trazas.

`MailMessage` y `SmtpClient` estarán dentro de `Using`; no habrá `Task.Run`, fire-and-forget ni colas. Una fábrica `ISecondFactorSmtpClientFactory` permitirá dobles sin sockets; el adaptador real encapsulará `SmtpClient`.

## D-09 — Sanitización

La fachada captura fallos y retorna solo `OTP_SUBMITTED`, `OTP_DISABLED`, `OTP_CONFIGURATION_INVALID`, `OTP_CONFIGURATION_AMBIGUOUS` u `OTP_DELIVERY_FAILED`. Nunca retorna o registra excepción, texto SMTP, correo completo, OTP, cuerpo, contraseña o conexión.

## D-10 — Verificación y autorización

Pruebas locales cubren cardinalidad, nulos, rangos, banderas, credenciales, SSL, timeout, cuerpo, excepciones y disposición. Una prueba estructural confirma que `ClassCorreo.vb` no cambia y que archivos nuevos no lo invocan. Se ejecutan regresiones DOC-91/DOC-92 y MSBuild. SMTP real requiere autorización vigente.

## Inventario de cambios previsto

| Acción | Ruta |
| --- | --- |
| Modificar compatible | `Modelo/Login/SegundoFactor/SegundoFactorInterfaces.vb` |
| Crear | `Modelo/Login/SegundoFactor/SegundoFactorSmtpModels.vb` |
| Crear | `Infrastructure/Repositories/Login/SegundoFactor/MySqlSecondFactorSmtpConfigurationRepository.vb` |
| Crear | `Infrastructure/Login/SegundoFactor/Smtp/SecondFactorSmtpEmailSender.vb` |
| Crear | `Infrastructure/Login/SegundoFactor/Smtp/SecondFactorSmtpTransport.vb` |
| Crear | `Infrastructure/Login/SegundoFactor/Smtp/FrameworkSmtpClientAdapter.vb` |
| Modificar | `GestionDocumental-Docuarchi.net.vbproj` |
| Crear | `tests/login-second-factor-smtp.test.cjs` y runner conductual sin red |
| Crear/actualizar | `Doc/Actualizacion/Login/Implementacion/DOC-93/` y documentación OPSXJ |

Fuera del diff funcional: `radicador/ClassCorreo.vb`, `radicador/ClassRaEnvioCorrespondencia.vb`, `Services/`, `DTOs/`, `webservice/`, login, UI y esquema MySQL.

## Risks / Trade-offs

- `SmtpClient` síncrono agrega latencia; el timeout acotado limita el impacto.
- La tabla contiene contraseña reversible; DOC-93 no cambia esquema y limita su exposición a memoria.
- Dos filas activas bloquean deliberadamente 2FA; corregir datos requiere operación externa.
- Pruebas sin red no acreditan entrega real.

## Migration Plan

No existe migración. Rollback: retirar fuentes nuevas y entradas del `.vbproj`; correo legacy permanece intacto.

## Open Questions

Ninguna dentro del alcance interno. Integración con login y SMTP real pertenecen a fases posteriores.
