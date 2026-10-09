<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento — DOC-93 SMTP dedicado para segundo factor

## Fuente y alcance

- Ticket: `DOC-93` — `SEGUNDO-FACTOR-SMTP`.
- Cambio: `doc-93-segundo-factor-smtp`.
- Perfil: VB.NET WebForms sobre .NET Framework 4.6.1, ADO.NET/MySQL y `System.Net.Mail`.
- Alcance: infraestructura interna inactiva; no se conecta todavía al login, ASMX o UI.

## Contexto inspeccionado

- `radicador/ClassCorreo.vb`: `Config_Smtp`, `Obtener_Datos_ConfigSmtp`, `MailMessage`, `SmtpClient`, SSL, credenciales y conversión `SMTP_TIEMPO * 100000`.
- `radicador/ClassRaEnvioCorrespondencia.vb`: consumidor legacy de correo fuera del diff.
- `Modelo/Login/SegundoFactor/SegundoFactorModels.vb`: `SecondFactorEmailMessage` y `SecondFactorDeliveryResult` existentes.
- `Modelo/Login/SegundoFactor/SegundoFactorInterfaces.vb`: puerto `ISecondFactorEmailSender.Send`.
- `Infrastructure/Shared/Data/ModuleDataContracts.vb` y `AdoNetDataInfrastructure.vb`: `IModuleConnectionFactory` e `IDataExecutor`.
- `Infrastructure/Repositories/Login/SegundoFactor/MySqlSecondFactorChallengeRepository.vb`: patrón de inyección y snapshot defensivo de `ContextoModulo`.
- `GestionDocumental-Docuarchi.net.vbproj`: altas explícitas de fuentes VB.NET.
- Exploración 2FA: cardinalidad, fail-closed, cuerpo OTP y límites aprobados.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia de código | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | Entregar infraestructura SMTP inactiva sin cambiar login, UI, ASMX ni challenges. | Puertos DOC-91 sin consumidores productivos. | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | Conservar `ISecondFactorEmailSender.Send(SecondFactorEmailMessage) As SecondFactorDeliveryResult`. | `SegundoFactorInterfaces.vb`. | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | Introducir modelos SMTP inmutables y estados sanitizados independientes de `System.Net.Mail`. | `SegundoFactorModels.vb`. | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | Leer `Config_Smpt_Side` con fábrica/ejecutor compartidos, contexto inyectado y columnas explícitas. | Infraestructura compartida y patrón DOC-92. | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | Exigir una fila `ESTADO_ENVIO = 1`: cero `Disabled`, múltiples `AmbiguousConfiguration`. | Legacy elige la primera; exploración exige fail-closed. | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | Validar host, puerto, remitente, banderas, credenciales y timeout antes de crear recursos SMTP. | Campos de `Config_Smtp`. | D-06 | RQ-06 | Origen: D-06, RQ-06 |
| D-07 | Conservar `SMTP_TIEMPO * 100000` con `Checked` y máximo 120000 ms; leer dominio sin aplicarlo. | `ClassCorreo.vb` y exploración §8. | D-07 | RQ-07 | Origen: D-07, RQ-07 |
| D-08 | Encapsular `MailMessage`/`SmtpClient` en transporte síncrono con `Using` y cuerpo OTP mínimo. | Jira DOC-93. | D-08 | RQ-08 | Origen: D-08, RQ-08 |
| D-09 | Retornar solo estados/códigos públicos; nunca secretos ni texto del servidor. | Contratos sanitizados DOC-91. | D-09 | RQ-09 | Origen: D-09, RQ-09 |
| D-10 | Probar con dobles sin red, caracterizar `ClassCorreo` intacto y requerir autorización para SMTP real. | Política Jira y AGENTS. | D-10 | RQ-10 | Origen: D-10, RQ-10 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptación | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | Despliegue sin activación funcional. | Ningún recorrido HTTP o login cambia. | Evita activar 2FA prematuramente. |
| RQ-02 | Puerto DOC-91 conservado. | Firma existente compila y retorna su tipo heredado. | Sin regresión contractual. |
| RQ-03 | Configuración y estados tipados. | Objetos inmutables; resultados sin detalles técnicos. | Contraseña solo en memoria. |
| RQ-04 | Consulta explícita con contexto inyectado. | Sin `SELECT *`, `conect`, `HttpContext`, Session ni cadenas embebidas. | No crea fuente paralela. |
| RQ-05 | Cardinalidad estricta. | 0/1/>1 filas producen Disabled/validación/Ambiguous. | No elige cuenta arbitraria. |
| RQ-06 | Validación previa a red. | Campo inválido retorna InvalidConfiguration sin crear cliente. | Falla cerrada. |
| RQ-07 | Timeout compatible y acotado. | Overflow/no positivo inválido; efectivo <=120000 ms. | Evita bloqueos prolongados. |
| RQ-08 | Envío OTP mínimo y determinístico. | SSL/credenciales según tabla y recursos siempre liberados. | Correo legacy intacto. |
| RQ-09 | Error público sanitizado. | Excepción produce Failed sin secretos ni mensaje del servidor. | Evita filtración. |
| RQ-10 | Evidencia honesta y regresión. | Dobles, regresiones y MSBuild; SMTP real no ejecutado sin autorización. | No atribuye integración falsa. |

## Reglas de trazabilidad obligatorias

Cada D-XX se desarrolla en `design.md`, corresponde a RQ-XX en la especificación y origina al menos una tarea atómica. No se autorizan endpoints, UI, cambios de esquema, configuración paralela ni cambios al correo legacy.

## Resultado del refinamiento

- Estado: aprobado.
- Implementación limitada a las rutas inventariadas.
- SMTP real requiere autorización nueva para ambiente, cuenta y buzón descartable.
