# Casos de uso implementados

## UC-01 — Interpretar configuración 2FA

| Campo | Detalle |
| --- | --- |
| Actor | `EXT: consumidor interno futuro` |
| Precondiciones | Valores nullable de requerido, proveedor y expiración ya obtenidos por una capa externa. |
| Flujo principal | Construir `SegundoFactorConfiguration`; con requerido=1, provider=1 y expiración 1..10 queda activa. |
| Alternativos | Requerido 0 o `NULL` produce configuración desactivada e ignora proveedor/expiración. |
| Errores | Otro requerido, proveedor distinto de 1 o expiración fuera de rango lanza `ArgumentOutOfRangeException`. |
| Resultado | Configuración inmutable activa o desactivada. |
| Endpoint / Service / Repository | No aplica / No aplica / solo puerto `ISecondFactorConfigurationRepository.GetConfiguration(Int32)`. |

## UC-02 — Generar un OTP

| Campo | Detalle |
| --- | --- |
| Actor | `EXT: consumidor interno futuro` |
| Precondiciones | Instancia vigente de `CryptographicSecondFactorOtpGenerator`. |
| Flujo principal | `GenerateCode()` obtiene cuatro bytes, acepta una muestra menor a 4.294.000.000 y devuelve módulo 1.000.000 con formato `D6`. |
| Alternativos | Una muestra fuera del límite se descarta y se solicita otra. |
| Errores | Una instancia eliminada lanza `ObjectDisposedException`. |
| Resultado | `String` de seis dígitos. |
| Endpoint / Service / Repository | No aplica / No aplica / No aplica. |

## UC-03 — Proteger un OTP

| Campo | Detalle |
| --- | --- |
| Actor | `EXT: consumidor interno futuro` |
| Precondiciones | Contexto válido, OTP de seis dígitos y llave activa configurada. |
| Flujo principal | `Protect` obtiene la llave activa, codifica propósito, challenge, identidad, vínculo y OTP, calcula HMAC-SHA256 y devuelve `v1:keyId:base64mac`. |
| Alternativos | No aplica. |
| Errores | Contexto nulo, OTP inválido o llave ausente/insegura producen excepción controlada. |
| Resultado | Hash versionado; el OTP no se retorna dentro del hash. |
| Endpoint / Service / Repository | No aplica / `ISecondFactorCodeProtector` / No aplica. |

## UC-04 — Verificar un OTP protegido

| Campo | Detalle |
| --- | --- |
| Actor | `EXT: consumidor interno futuro` |
| Precondiciones | Contexto y código candidatos; valor protegido previamente. |
| Flujo principal | `Verify` analiza tres segmentos, exige versión v1, resuelve la llave por `keyId`, recalcula HMAC y compara bytes de igual longitud mediante XOR. |
| Alternativos | Una llave anterior todavía configurada permite validar un challenge emitido antes de rotación. |
| Errores | Contexto/código/formato/Base64/versión/llave/longitud/MAC inválidos retornan `False`; no se exponen detalles. |
| Resultado | `Boolean`. |
| Endpoint / Service / Repository | No aplica / `ISecondFactorCodeProtector` / No aplica. |

## UC-05 — Resolver material HMAC

| Campo | Detalle |
| --- | --- |
| Actor | `HmacSecondFactorCodeProtector` u otro consumidor interno. |
| Precondiciones | Fuente `ISecondFactorSettings`; no implica que `Web.config` contenga valores en el repositorio. |
| Flujo principal | `GetActiveKey` lee `LoginSecondFactorHmacActiveKeyId`; `TryGetKey` lee `LoginSecondFactorHmacKey.<keyId>`, decodifica Base64 y exige 32 bytes. |
| Alternativos | `TryGetKey` puede recuperar una llave anterior por identificador. |
| Errores | Ausencia o material inválido retorna `False`; para la activa, `GetActiveKey` lanza `InvalidOperationException`. |
| Resultado | `SecondFactorKeyMaterial` que entrega copia de los bytes. |
| Endpoint / Service / Repository | No aplica / No aplica / No aplica. |

## UC-06 — Administrar contexto pendiente en Session

| Campo | Detalle |
| --- | --- |
| Actor | `EXT: consumidor WebForms futuro` |
| Precondiciones | `HttpSessionStateBase`, reloj y `PendingSecondFactorContext` vigentes. |
| Flujo principal | `Save` almacena; `GetCurrent` recupera; `Clear` elimina la clave privada. |
| Alternativos | Si no existe o el tipo no coincide, `GetCurrent` retorna `Nothing`; si expiró, lo elimina y retorna `Nothing`. |
| Errores | Session/reloj/contexto nulos o contexto ya expirado producen excepción de argumento. |
| Resultado | Contexto vigente o `Nothing`; nunca una sesión autenticada. |
| Endpoint / Service / Repository | No aplica / `IPendingSecondFactorContextStore` / No aplica. |

## UC-07 — Validar datos auxiliares

| Campo | Detalle |
| --- | --- |
| Actor | `EXT: consumidor interno futuro` |
| Precondiciones | Código o dirección de correo candidatos. |
| Flujo principal | `IsSixDigitCode` valida seis caracteres ASCII; `MaskEmailAddress` conserva un carácter local cuando aplica, agrega asteriscos y conserva el dominio; `IsMaskedEmailDestination` exige un único `@`, asterisco solo en la parte local y ausencia de espacios. |
| Alternativos | Un local de un carácter se convierte en `*`. |
| Errores | Correo vacío, sin `@`, con dominio vacío o con más de un `@` lanza `ArgumentException`. |
| Resultado | `Boolean` o correo enmascarado. |
| Endpoint / Service / Repository | No aplica / No aplica / No aplica. |

## Funcionalidades no implementadas

No son casos de uso de DOC-91: consultar MySQL, persistir/consumir challenges, resolver destinatarios reales, enviar correo, finalizar login, publicar ASMX, presentar formulario OTP, reenviar códigos o autenticar usuarios. Solo existen puertos para algunas de esas entregas futuras.
