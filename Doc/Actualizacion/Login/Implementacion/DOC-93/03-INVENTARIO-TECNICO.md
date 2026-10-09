# Inventario técnico verificado

Las rutas son relativas a `DocuArchiNet`.

## APIs, servicios, repositorios y adaptadores

| Tipo | Ruta del archivo | Clase o interfaz | Nombre exacto | Parámetros y tipos | Retorno | Descripción | Relaciones o dependencias |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Endpoint | No aplica | No aplica | No aplica | Verbo/ruta/autorización/DTO: No aplica | No aplica | DOC-93 no publica HTTP o ASMX. | No aplica |
| Controller | No aplica | No aplica | No aplica | No aplica | No aplica | No existe Controller DOC-93. | No aplica |
| Service de aplicación | No aplica | No aplica | No aplica | No aplica | No aplica | La activación del login queda fuera de alcance. | No aplica |
| Puerto | `Modelo/Login/SegundoFactor/SegundoFactorInterfaces.vb` | `ISecondFactorEmailSender` | `Send` | `message As SecondFactorEmailMessage` | `SecondFactorDeliveryResult` | Contrato DOC-91 preservado. | Implementado por `SecondFactorSmtpEmailSender`. |
| Puerto | misma ruta | `ISecondFactorSmtpConfigurationRepository` | `Resolve` | Sin parámetros | `SecondFactorSmtpConfigurationResolution` | Resuelve exactamente una configuración activa. | Implementado por repositorio MySQL. |
| Puerto | misma ruta | `ISecondFactorSmtpTransport` | `Send` | `configuration As SecondFactorSmtpConfiguration`, `message As SecondFactorEmailMessage` | `SecondFactorSmtpDelivery` | Entrega interna tipada. | Implementado por `SecondFactorSmtpTransport`. |
| Repositorio | `Infrastructure/Repositories/Login/SegundoFactor/MySqlSecondFactorSmtpConfigurationRepository.vb` | `MySqlSecondFactorSmtpConfigurationRepository` | `New` | `connections As IModuleConnectionFactory`, `executor As IDataExecutor`, `radicacionContext As ContextoModulo` | Instancia | Copia el contexto defensivamente. | Fábrica/ejecutor compartidos. |
| Repositorio | misma ruta | misma clase | `Resolve` | Sin parámetros | `SecondFactorSmtpConfigurationResolution` | Consulta, cuenta y valida; captura errores. | `Config_Smpt_Side`. |
| Transporte | `Infrastructure/Login/SegundoFactor/Smtp/SecondFactorSmtpTransport.vb` | `SecondFactorSmtpTransport` | `Send` | `configuration As SecondFactorSmtpConfiguration`, `message As SecondFactorEmailMessage` | `SecondFactorSmtpDelivery` | Construye mensaje mínimo y dispone recursos. | `ISecondFactorSmtpClientFactory`. |
| Fachada | `Infrastructure/Login/SegundoFactor/Smtp/SecondFactorSmtpEmailSender.vb` | `SecondFactorSmtpEmailSender` | `Send` | `message As SecondFactorEmailMessage` | `SecondFactorDeliveryResult` | Mapea estados a códigos públicos. | Repositorio y transporte. |
| Fábrica | `Infrastructure/Login/SegundoFactor/Smtp/FrameworkSmtpClientAdapter.vb` | `ISecondFactorSmtpClientFactory` | `Create` | `configuration As SecondFactorSmtpConfiguration` | `ISecondFactorSmtpClient` | Puerto sustituible sin sockets. | Implementada por `FrameworkSmtpClientFactory`. |
| Fábrica | misma ruta | `FrameworkSmtpClientFactory` | `Create` | `configuration As SecondFactorSmtpConfiguration` | `ISecondFactorSmtpClient` | Configura `SmtpClient`; no usa dominio. | `FrameworkSmtpClientAdapter`. |
| Adaptador | misma ruta | `ISecondFactorSmtpClient` | `Send` | `message As MailMessage` | `Void` | Puerto del cliente framework. | `IDisposable`. |
| Adaptador | misma ruta | `FrameworkSmtpClientAdapter` | `Send` | `message As MailMessage` | `Void` | Envío síncrono. | `SmtpClient.Send`. |
| Adaptador | misma ruta | misma clase | `Dispose` | Sin parámetros | `Void` | Libera `SmtpClient` una vez. | `IDisposable`. |
| Datos compartidos | `Infrastructure/Shared/Data/ModuleDataContracts.vb` | `IModuleConnectionFactory` | `CreateOpenConnection` | `contexto As ContextoModulo` | `IDbConnection` | Abre conexión ya configurada. | Implementación existente. |
| Datos compartidos | misma ruta | `IDataExecutor` | `ExecuteReader(Of T)` | `connection As IDbConnection`, `transaction As IDbTransaction`, `commandText As String`, `parameters As IEnumerable(Of IDataParameter)`, `projector As Func(Of IDataReader,T)` | `T` | Ejecuta lectura y dispone comando/reader. | `AdoNetDataExecutor`. |

## Modelos y propiedades

| Tipo | Ruta | Propiedades exactas | Obligatoriedad y validación |
| --- | --- | --- | --- |
| Enum | `Modelo/Login/SegundoFactor/SegundoFactorSmtpModels.vb` | `SecondFactorSmtpDeliveryStatus`: `Submitted`, `Disabled`, `InvalidConfiguration`, `AmbiguousConfiguration`, `Failed` | Estado obligatorio. |
| Modelo | misma ruta | `SecondFactorSmtpConfiguration`: `Host As String`, `Port As Integer`, `SenderAddress As String`, `Username As String`, `Password As String`, `Domain As String`, `TimeoutMilliseconds As Integer`, `EnableSsl As Boolean`, `UseDefaultCredentials As Boolean`, `IsBodyHtml As Boolean` | Propiedades `ReadOnly`; el repositorio valida antes de construir. |
| Resultado | misma ruta | `SecondFactorSmtpConfigurationResolution`: `Status`, `Configuration` | `Submitted` exige configuración; otros estados la prohíben. |
| Resultado | misma ruta | `SecondFactorSmtpDelivery`: `Status`, `PublicCode As String` | Código no vacío y propiedades `ReadOnly`. |
| Entrada existente | `Modelo/Login/SegundoFactor/SegundoFactorModels.vb` | `SecondFactorEmailMessage`: `Recipient`, `Code`, `ExpiresAtUtc` | Destinatario obligatorio, OTP de seis dígitos y UTC. |
| Salida existente | misma ruta | `SecondFactorDeliveryResult`: `Success As Boolean`, `PublicCode As String` | Mapeada por la fachada. |

No existen DTOs HTTP DOC-93.

## Tabla y columnas leídas

| Tabla | Columna | Uso | Validación |
| --- | --- | --- | --- |
| `Config_Smpt_Side` | `SERV_SMTP` | Host | No nulo/vacío. |
| misma | `PUERTO_SERV_SMTP` | Puerto | Entero 1..65535. |
| misma | `USUARIO_SMTP` | Remitente y usuario explícito | Dirección de correo válida. |
| misma | `PASW_SMTP` | Contraseña | Obligatoria solo con credencial `1`; nunca se retorna. |
| misma | `DOMINIO_SMTP` | Compatibilidad de lectura | Se conserva, no se usa en `NetworkCredential`. |
| misma | `SMTP_TIEMPO` | Timeout legacy | Positivo, sin overflow al multiplicar por 100000, máximo efectivo 120000 ms. |
| misma | `ESTADO_SSL` | SSL | Solo 0/1. |
| misma | `ESTADO_ENVIO` | Selección/confirmación | Parámetro `@enabled=1`; fila debe conservar valor 1. |
| misma | `ESTADO_BODY` | HTML/texto | Solo 0/1. |
| misma | `ESTADO_CREDENCIAL` | Default/explícita | Solo 0/1. |

## Códigos públicos

`OTP_SUBMITTED`, `OTP_DISABLED`, `OTP_CONFIGURATION_INVALID`, `OTP_CONFIGURATION_AMBIGUOUS` y `OTP_DELIVERY_FAILED`. Ningún código contiene OTP, buzón, contraseña, conexión, cuerpo o texto de excepción.
