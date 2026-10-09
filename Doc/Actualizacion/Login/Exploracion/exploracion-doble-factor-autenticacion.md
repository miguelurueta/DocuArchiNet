# Exploración técnica: doble factor de autenticación para DocuArchiNet

Fecha de revisión inicial: 2026-10-08
Última actualización arquitectónica: 2026-10-09
Estado: exploración arquitectónica cerrada para descomposición Spec-Driven; no implementado
Repositorio destino: `D:\imagenesda\DocuachiNet\DocuArchiNet`

## 1. Objetivo

Determinar si la implementación de doble factor de autenticación existente en el workspace `D:\imagenesda\GestorDocumental\DocuArchiCore` puede incorporarse a DocuArchiNet, conservando el login WebForms y las tablas compartidas.

La solución deberá funcionar para todos los módulos autenticables de DocuArchiNet:

- `DOCUARCHI CONTENEDOR`.
- `GESTOR DOCUMENTAL`.
- `RADICACION DOCUMENTAL`.
- `WORKFLOW DOCUMENTAL`.

El envío del código utilizará una clase SMTP dedicada para 2FA, reutilizando la conexión existente y la configuración de `Config_Smpt_Side` marcada con `ESTADO_ENVIO = 1`. `ClassCorreo.Envio_Correo_recuperacion_pasword` y sus recorridos legacy permanecerán intactos y no serán invocados por el flujo nuevo. No se incorporará `EmailSenderStub` del Core.

## 2. Alcance inspeccionado

### 2.1. Repositorio destino: DocuArchiNet

- `gestor.aspx.vb`.
- `Defaul/ClassGestorSesion.vb`.
- `Defaul/GestorModuleSesion.vb`.
- `Defaul/conect.vb`.
- `radicador/ClassCorreo.vb`.
- `radicador/ClassRaEnvioCorrespondencia.vb`.
- `Infrastructure/Shared/Data/ModuleDataContracts.vb`.
- `Infrastructure/Shared/Data/AdoNetDataInfrastructure.vb`.
- `Infrastructure/Shared/Data/WorkflowModuleConnectionFactory.vb`.
- `Infrastructure/Shared/Data/ModuleSessionConnectionStringResolver.vb`.
- `webservice/WebServiceWorkflowModern.asmx.vb`.
- `Global.asax.vb` y `Web.config`.
- `GestionDocumental-Docuarchi.net.vbproj`.
- `packages.config`.

DocuArchiNet utiliza VB.NET, ASP.NET WebForms, Forms Authentication y .NET Framework 4.6.1.

### 2.2. Repositorios fuente revisados

| Repositorio | Rama observada | Revisión observada | Participación |
|---|---|---|---|
| `DocuArchi.Api` | `scrum-412-implementacion-api-sub-series` | `16cf793` | Endpoints de login, verificación y recuperación |
| `DocuArchiCore.Web` | `main` | `355b357` | Cliente JavaScript del login |
| `MiApp.Services` | `scrum-412-implementacion-api-sub-series` | `8ef784f` | Orquestación, proveedor de correo y validación OTP |
| `MiApp.Repository` | `scrum-412-implementacion-api-sub-series` | `86c4f17` | Persistencia del challenge |
| `MiApp.Models` | `scrum-390-implementacion-preview` | `64283ff` | Modelos del challenge, módulo y contexto |
| `MiApp.DTOs` | `scrum-412-workspace-aligned` | `0cbdf7c` | Contratos de autenticación |

Los repositorios fuente no estaban en una única rama coordinada. Los hallazgos corresponden a los working trees y revisiones indicados, no a una versión integrada certificada.

Los proyectos de backend inspeccionados usan .NET 10 y `DocuArchiCore.Web` usa .NET 9. Por esta diferencia de plataforma, sus ensamblados y clases no pueden copiarse directamente al proyecto WebForms .NET Framework 4.6.1.

## 3. Conclusión

La funcionalidad puede incorporarse, pero la implementación actual del Core no debe trasladarse literalmente.

La inspección física de solo lectura ejecutada el 2026-10-08 confirmó que la base configurada `docuarchi` es compatible con el contrato de persistencia que hoy espera el Core: contiene las tres columnas de configuración en `gestor_modulos` y la tabla `ra_auth_second_factor_challenge` con sus columnas e índices actuales. Esta compatibilidad estructural no corrige los defectos de seguridad y concurrencia descritos en esta exploración.

Se deben reutilizar el concepto de challenge y, después de corregir su contrato, la tabla compartida. DocuArchiNet debe conservar:

- su validación de credenciales por módulo;
- su inicialización de permisos y sesiones relacionadas;
- Forms Authentication;
- su tabla y conexión de configuración SMTP, sin reutilizar ni modificar la clase legacy de correo.

No se debe trasladar a DocuArchiNet la emisión de JWT, la inicialización de sesión del Core, `EmailSenderStub` ni la serialización completa de `UserAuthContext`.

## 4. Implementación existente en DocuArchiCore

### 4.1. Flujo observado

```text
POST api/accout/ValidaUserAplicacion
    -> AutenticacionApplicationService.ValidarLogin
    -> InicioSesionL.ValidaUsuarioAplicacion
    -> EmailSecondFactorProvider.CreateTokenSegundoFactorAutenticacion
    -> INSERT ra_auth_second_factor_challenge
    -> IEmailSender.SendAsync

POST api/accout/VerificarSegundoFactor
    -> SecondFactorService.ValidaTokenSegundoFactorL
    -> consulta challenge
    -> valida expiración, consumo, intentos y hash
    -> consume challenge
    -> inicia sesión y permisos
    -> emite JWT
```

### 4.2. Componentes principales

| Componente | Repositorio | Ruta relativa |
|---|---|---|
| `AccountController` | `DocuArchi.Api` | `Controllers/Account/AccountController.cs` |
| `AutenticacionApplicationService` | `MiApp.Services` | `Service/Account/AutenticacionApplicationService.cs` |
| `EmailSecondFactorProvider` | `MiApp.Services` | `Service/Autenticacion/Providers/EmailSecondFactorProvider.cs` |
| `SecondFactorService` | `MiApp.Services` | `Service/Autenticacion/SecondFactor/SecondFactorService.cs` |
| `SecondFactorChallengeRepository` | `MiApp.Repository` | `Repositorio/Autenticacion/SecondFactorChallengeRepository.cs` |
| `AuthSecondFactorChallengeDA` | `MiApp.Models` | `Models/Auntetnicacion/AuthSecondFactorChallengeDA.cs` |
| `UserAuthContext` | `MiApp.Models` | `Models/Auntetnicacion/UserAuthContext.cs` |
| `GestorModulo` | `MiApp.Models` | `Models/Account/GestorModulo.cs` |
| `HashHelper` | `MiApp.Services` | `Service/SessionHelper/HashHelper.cs` |
| `AuthOrchestrator` | `MiApp.Services` | `Service/Autenticacion/Recovery/AuthOrchestrator.cs` |
| Cliente de login | `DocuArchiCore.Web` | `wwwroot/frontend/modules/Account/Login.js` |

### 4.3. Cobertura funcional real

- El único proveedor implementado es correo electrónico.
- Existe una enumeración para TOTP, pero no se encontró un proveedor TOTP implementado.
- La selección del proveedor configurado en `GestorModulo.SecondFactorProviderType` no se respeta: el código selecciona EMAIL directamente.
- `InicioSesionL` contempla Gestor, Workflow y DocuArchi Contenedor, pero no Radicación Documental.
- El cliente `Login.js` no implementa la captura ni la validación del OTP y navega directamente a `/Home/Home` después del primer llamado.
- El proveedor registrado para `IEmailSender` es `EmailSenderStub`, cuyo método no envía mensajes.

## 5. Riesgos encontrados en la implementación fuente

### 5.1. Exposición de secretos en el challenge

`EmailSecondFactorProvider` serializa el objeto completo `UserAuthContext` en `AuthPayloadJson`. Este contexto contiene `GestorModulo`, que a su vez incluye usuario y contraseña de base de datos, contraseña de web service y datos de conexión.

Consecuencia: las credenciales técnicas pueden quedar almacenadas como JSON legible dentro de `ra_auth_second_factor_challenge`.

El challenge nuevo deberá almacenar únicamente identificadores mínimos y nunca el objeto de módulo completo, contraseñas, permisos o cadenas de conexión.

### 5.2. Código OTP expuesto en logs

El código de seis dígitos se registra mediante `LogInformation`. Un OTP nunca debe escribirse en logs, trazas, errores ni auditorías.

### 5.3. Hash susceptible a fuerza bruta offline

El hash observado es SHA-256 sobre `EMAIL-OTP|authUserId|code`, sin una clave secreta. Si alguien obtiene lectura de la tabla puede probar las 1.000.000 combinaciones posibles fuera de la aplicación.

La solución deberá usar HMAC-SHA256 con un secreto externo a la base de datos y comparación en tiempo constante.

### 5.4. Operaciones no atómicas

El incremento de intentos y el consumo realizan primero una lectura y después una actualización. Solicitudes concurrentes pueden:

- consumir el mismo código más de una vez;
- perder incrementos de intentos;
- superar el máximo configurado.

La verificación y el consumo deberán resolverse con una operación condicional atómica o una transacción con bloqueo apropiado.

### 5.5. Challenge consumido antes de completar el flujo

El Core marca el challenge como consumido antes de deserializar el payload, inicializar permisos y emitir el token. Un error posterior invalida un código correcto sin finalizar la autenticación.

### 5.6. Recuperación y login acoplados

`RecoveryPaswVerifyOtp` llama a `ValidaTokenSegundoFactorL`. Ese método consume el challenge, inicializa el login y genera internamente un token de acceso antes de que recuperación valide `Purpose=RECOVERY` y genere su token limitado.

Login y recuperación deberán usar propósitos separados y una validación común que no cree sesiones ni tokens por sí misma.

### 5.7. Respuesta incorrecta ante fallo de creación

`AutenticacionApplicationService.ValidarLogin` comprueba que `secondFactorResult` no sea nulo, pero no valida correctamente su indicador `success`. Puede responder `SECOND_FACTOR_REQUIRED` aunque el proveedor haya retornado un error, por ejemplo por ausencia de correo.

### 5.8. Ausencia de artefactos de madurez

No se encontraron:

- pruebas automáticas específicas del segundo factor;
- migración o DDL versionado para `ra_auth_second_factor_challenge`;
- migración de las columnas de segundo factor de `gestor_modulos`;
- límites de generación o reenvío;
- invalidación de challenges anteriores;
- proceso de limpieza de challenges expirados.

### 5.9. Verificación física del esquema compartido

Se ejecutó el inspector autorizado y exclusivamente de lectura:

```text
npm.cmd --prefix tools/e2e run inspect:login:2fa-schema -- --profile profiles/doc90-radicacion-classic-attachment.local.runtime.json
```

La inspección consulta únicamente `information_schema`. No crea ni modifica tablas, índices, filas, tareas, estados ni auditoría. El contrato del inspector tiene además tres pruebas automáticas, ejecutadas satisfactoriamente con:

```text
node --test tools/e2e/tests/login-second-factor-schema-inspector.test.cjs
```

Resultado comprobado en el ambiente autorizado:

| Hallazgo | Resultado | Implicación |
|---|---|---|
| Base configurada | `docuarchi` | Es la ubicación central que debe usarse para configuración y challenges. |
| `gestor_modulos.ID_MODULO` | `int(10) unsigned`, PK, no nulo | Compatible con el identificador consultado por el Core. |
| `RequiereSegundoFactor` | `int(11)`, nullable | Existe, pero requiere una regla explícita para `NULL`. |
| `SecondFactorProviderType` | `int(11)`, nullable | Existe; el Core actual no respeta realmente esta selección. |
| `SegundoFactorTiempoExpira` | `int(11)`, nullable | Existe, pero necesita validación de rango y tratamiento de `NULL`/cero. |
| `ra_auth_second_factor_challenge` | Todas las columnas esperadas existen | Se puede reutilizar como punto de partida, no sin endurecimiento. |
| `ChallengeId` | Índice único `uq_challengeid` | Impide duplicar el identificador público del challenge. |
| `AuthUserId` | Índice no único `IX_ra_auth_sfc_authuserid` | Ayuda a localizar challenges por usuario, pero no garantiza consumo atómico. |
| Tablas de usuario | Las columnas de identidad y correo existen en los esquemas inspeccionados | Es viable resolver el destinatario para los cuatro módulos mediante adaptadores por módulo. |

Las copias de `gestor_modulos` encontradas en esquemas distintos de `docuarchi` no contienen uniformemente las columnas 2FA; algunas tampoco contienen `ID_MODULO`. Esto no obliga a migrarlas. Tanto DocuArchiNet como el Core consultan el catálogo de módulos mediante la conexión central, mientras que las conexiones específicas se usan para resolver el usuario de cada módulo. En consecuencia:

```text
Configuración 2FA y challenge -> docuarchi central
Identidad y correo           -> base del módulo/empresa autenticados
```

La implementación deberá hacer explícita esta separación y no depender de cuál conexión haya quedado activa previamente en Session.

La tabla actual es compatible con el modelo existente, pero no cubre por sí sola el contrato seguro propuesto. Antes de implementar se deberá decidir y versionar el endurecimiento mínimo:

- propósito explícito del challenge (`LOGIN`, y recuperación únicamente en un incremento separado);
- enlace no reversible con la sesión WebForms que inició la autenticación;
- consumo e incremento de intentos mediante actualización condicional atómica;
- invalidación del challenge anterior al reenviar;
- fecha de consumo o revocación y estrategia de limpieza;
- restricción para impedir `CodeHash` nulo en challenges activos;
- índices compuestos acordes con las consultas atómicas y de limpieza.

No se reutilizará `AuthPayloadJson` para guardar el contexto completo. Si la columna se conserva por compatibilidad, su contenido deberá limitarse a identificadores no secretos o permanecer nulo.

Las diferencias de mayúsculas y longitudes observadas en nombres de login y correo deberán manejarse con mapeo explícito por módulo. Los correos de Gestor, Radicación y Workflow admiten `NULL`; cuando 2FA esté activado, la ausencia de correo debe cerrar el acceso con un error controlado y no omitir el segundo factor.

## 6. Login existente en DocuArchiNet

El evento de `gestor.aspx.vb` llama a:

```vb
ClassGestorSesion.InicioAplicacionWebGestorDocumental(modulo, usuario, contraseña, empresa)
```

El método actual realiza en una sola operación:

```text
validación de entrada
    -> inicialización de conexiones y variables Session
    -> resolución del tipo de módulo
    -> validación de usuario y contraseña
    -> carga de usuarios relacionados
    -> inicialización de sesiones y permisos por módulo
    -> registros de auditoría de inicio
    -> FormsAuthentication.RedirectFromLoginPage
```

El punto correcto para exigir el OTP está después de validar las credenciales y antes de inicializar identidades autenticadas, permisos, auditoría y Forms Authentication.

Agregar el OTP al final del método no sería seguro porque para ese momento la aplicación ya habría construido el contexto autenticado.

La inspección del método completo introduce dos precisiones obligatorias:

- `InicioAplicacionWebGestorDocumental` ejecuta `InicializaconexionesModulos` y guarda metadatos de conexión en `Session` antes de validar la contraseña. Esa preparación técnica existe hoy y no equivale a una sesión autenticada; debe preservarse para no romper la selección de empresa/módulo.
- La regla de “usuario activo” no es homogénea: Gestor y Workflow consultan su estado vigente; Radicación y DocuArchi Contenedor no aplican hoy una validación equivalente dentro de `ValidaUserAplicacion`. El incremento 2FA no puede agregar silenciosamente una regla de bloqueo que cambie el login legacy.

El límite seguro queda definido así:

```text
Preparación de conexiones de módulo en Session (comportamiento legacy permitido)
    -> validación legacy exacta de credenciales
    -> ¿2FA desactivado? -> finalización legacy inmediata
    -> ¿2FA activado?    -> contexto pendiente mínimo, sin identidad autenticada
                            -> challenge + correo + OTP
                            -> finalización legacy una sola vez
```

Antes del OTP no se podrán establecer usuarios autenticados de módulo, permisos, relaciones, auditoría de ingreso ni cookie Forms Authentication. Tampoco se deberá reinterpretar o endurecer la consulta legacy de credenciales dentro de este incremento.

## 7. Cobertura requerida para todos los módulos

| Tipo de módulo | Tabla | Identificador real requerido | Campo de login | Campo de correo |
|---|---|---|---|---|
| `DOCUARCHI CONTENEDOR` | `usuarios_da` | `Clave_Usuario` | `idusuario` | `correo` |
| `GESTOR DOCUMENTAL` | `remit_dest_interno` | `Id_Remit_Dest_Int` | `Login_Usuario` | `Correo_Electronico` |
| `RADICACION DOCUMENTAL` | `usuario_radicador` | `id_usuario` | `Login_usuario` | `Correo_Usuario` |
| `WORKFLOW DOCUMENTAL` | `usuario_workflow` | `idU_suario` | `login_Usuario` | `Correo_Usuario` |

La función actual `ValidaUserAplicacion` recibe `id_user` por referencia, pero solamente lo llena para DocuArchi Contenedor. Para habilitar 2FA en todos los módulos, la fase de preautenticación deberá obtener de forma inequívoca el ID real y el correo del usuario autenticado en cada tabla.

Esa resolución se implementará mediante adaptadores tipados y consultas parametrizadas por tipo de módulo. Reutilizará la misma conexión ya resuelta para empresa/módulo, pero no leerá `HttpContext` o `Session` dentro del repositorio. La capa de presentación deberá entregar un snapshot de conexión mediante las abstracciones existentes `IModuleConnectionFactory`, `IDataExecutor` y `ModuleSessionConnectionStringResolver`.

Un mismo nombre de usuario puede existir en empresas o módulos diferentes. Por tanto, la identidad del challenge deberá componerse como mínimo de:

```text
empresa + módulo + tipo de módulo + ID real del usuario + propósito
```

No se debe confiar en el nombre del módulo, usuario o correo enviados nuevamente por el navegador durante la verificación.

## 8. Infraestructura SMTP dedicada para 2FA

La inspección de `radicador/ClassCorreo.vb` confirma que el sistema ya obtiene host, puerto, SSL, credenciales y demás parámetros desde `Config_Smpt_Side`, y que la configuración activa se identifica actualmente con `ESTADO_ENVIO = 1`. También confirma que `ClassCorreo` mezcla varios recorridos legacy, incluido recuperar contraseña. Por compatibilidad, esa clase se conserva completamente intacta.

El flujo 2FA deberá incorporar dos fronteras nuevas y pequeñas:

- un repositorio de solo lectura que use la conexión vigente y obtenga explícitamente la configuración de `Config_Smpt_Side`;
- un transporte SMTP dedicado que reciba un modelo tipado y sanitizado.

La resolución de configuración deberá exigir exactamente una fila con `ESTADO_ENVIO = 1`. Si no existe o hay varias, el envío falla de forma cerrada y el login no puede completarse. No se agregará una columna `DEFAULT`, no se escogerá la primera fila y no se creará una fuente de configuración paralela.

La contraseña SMTP solo podrá existir en memoria durante la creación de la credencial. No deberá aparecer en logs, excepciones, DTOs, respuestas ni evidencia. `MailMessage` y `SmtpClient` deberán liberarse determinísticamente. La lógica de challenge, reenvío, intentos e invalidación permanecerá fuera del transporte.

El mensaje OTP deberá contener solamente:

- el código de verificación;
- su tiempo de expiración;
- una indicación de no compartir el código;
- una indicación para ignorar el mensaje si el usuario no inició el acceso.

No deberá incluir contraseña, hash, identificadores internos, cookies, datos de conexión ni información completa del challenge.

La función es síncrona, por lo que su latencia formará parte de la solicitud inicial del login. Esto deberá considerarse en timeout, experiencia de usuario y pruebas.

`SMTP_TIEMPO` se usa actualmente como `SMTP_TIEMPO * 100000`; no existe evidencia suficiente para reinterpretar su unidad. La implementación 2FA conservará esa conversión observable, con aritmética comprobada y un máximo efectivo de 120.000 ms para impedir esperas ilimitadas. `DOMINIO_SMTP` se leerá por compatibilidad de configuración, pero no se aplicará a `NetworkCredential` porque ningún recorrido inspeccionado lo usa y hacerlo cambiaría la semántica existente.

## 9. Arquitectura objetivo propuesta

```text
gestor.aspx
    |
    v
Preautenticar empresa + módulo + credenciales
    |
    +-- módulo sin 2FA --------------------------+
    |                                            |
    +-- módulo con 2FA                           |
            |                                    |
            v                                    |
      crear challenge mínimo                     |
            |                                    |
            v                                    |
      SecondFactorEmailSender                    |
            |                                    |
            +--> SecondFactorSmtpConfigRepository
            |         -> Config_Smpt_Side        |
            |            ESTADO_ENVIO = 1        |
            v                                    |
      SecondFactorSmtpTransport -> SMTP           |
            |                                    |
            v                                    |
      mostrar formulario OTP                     |
            |                                    |
            v                                    |
      verificar y consumir atómicamente          |
            |                                    |
            +------------------------------------+
                             |
                             v
                  Finalizar autenticación actual
                             |
                             v
             permisos + auditoría + FormsAuthentication
```

### 9.1. Preautenticación

Debe validar:

- empresa y módulo existentes y habilitados;
- correspondencia entre empresa y módulo;
- usuario y contraseña mediante la validación legacy exacta del tipo de módulo, incluido el estado únicamente donde hoy se valida;
- ID real del usuario;
- correo registrado, cuando el módulo exige segundo factor.

El resultado interno debe ser tipado y mínimo. No debe conservar la contraseña. La configuración 2FA se leerá del catálogo central de módulos mediante una consulta parametrizada y se copiará al contexto pendiente como snapshot; no se volverá a confiar en campos de identidad enviados por el navegador.

### 9.2. Creación del challenge

Debe:

- usar un identificador aleatorio criptográficamente seguro;
- generar un OTP de seis dígitos con RNG criptográfico;
- almacenar HMAC del OTP, no el OTP;
- registrar propósito `LOGIN`;
- registrar expiración UTC, intentos y estado de consumo;
- ligar el challenge a la sesión WebForms que inició el login;
- invalidar challenges anteriores equivalentes;
- aplicar cooldown y límites por identidad canónica y vínculo de sesión; la IP solo será telemetría sanitizada y no una identidad estable;
- enviar el código mediante la infraestructura SMTP dedicada para 2FA.

Si el correo no existe o el envío falla, no se deberá crear la sesión autenticada ni omitir el segundo factor.

### 9.3. Verificación

Debe validar de forma conjunta:

- challenge existente;
- propósito `LOGIN`;
- empresa, módulo y usuario esperados;
- vínculo con la sesión WebForms;
- challenge no consumido;
- challenge no expirado;
- intentos inferiores al máximo;
- HMAC coincidente mediante comparación en tiempo constante.

El incremento de intentos y el consumo deberán ser atómicos.

### 9.4. Finalización del login

Solo después de verificar y consumir correctamente el challenge se debe ejecutar la lógica actual de:

- inicialización específica del módulo;
- relaciones con otros usuarios o módulos;
- permisos;
- auditoría de inicio;
- establecimiento de Forms Authentication y navegación local segura.

La lógica posterior a credenciales se extraerá a un único finalizador legacy reutilizable. El recorrido sin 2FA continuará invocando `FormsAuthentication.RedirectFromLoginPage` como hoy. El recorrido ASMX con OTP invocará el mismo finalizador sin redirección, usará `FormsAuthentication.SetAuthCookie` y retornará únicamente una ruta local calculada y permitida por el servidor. No se aceptará un `returnUrl` remoto ni se finalizará más de una vez.

Si el módulo no exige segundo factor, este bloque se ejecutará inmediatamente después de validar las credenciales, preservando el comportamiento actual.

### 9.5. Frontera HTTP y estado pendiente

La entrada inicial seguirá siendo el postback de `gestor.aspx`; no se creará un endpoint alterno para recibir usuario y contraseña. Cuando se requiera 2FA, el servidor guardará en la `Session` InProc vigente únicamente:

- empresa, módulo y tipo de módulo ya resueltos;
- ID interno y login normalizado;
- destino enmascarado;
- identificador del challenge y nonce aleatorio de preautenticación;
- instante de creación y estado pendiente.

No se guardarán contraseña, OTP, correo completo, credenciales de base de datos ni el objeto `GestorModulo` en el challenge. La tabla persistirá solo un HMAC del vínculo de sesión, nunca el `SessionID` ni el nonce en texto claro.

Se creará un ASMX dedicado, siguiendo el patrón de `WebServiceWorkflowModern.asmx.vb`, con `EnableSession:=True`. Sus operaciones usarán exclusivamente el contexto pendiente del servidor:

```text
ObtenerEstado()                  -> estado público + destino enmascarado
Verificar(codigo As String)      -> resultado público + ruta local si finalizó
Reenviar()                       -> resultado público + nuevo tiempo de espera
Cancelar()                       -> revoca el pendiente y limpia Session
```

El cliente no enviará empresa, módulo, login, correo, ID de usuario ni ID de challenge.

### 9.6. Criptografía y llave HMAC

- OTP: seis dígitos generados con RNG criptográfico y muestreo sin sesgo compatible con .NET Framework 4.6.1.
- Hash almacenado: `v1:<keyId>:<base64mac>`.
- Entrada autenticada: propósito, challenge, identidad canónica, vínculo de sesión y OTP.
- Comparación: recorrido XOR de longitud constante; no se depende de APIs ausentes en .NET Framework 4.6.1.
- Configuración externa: `LoginSecondFactorHmacActiveKeyId` y `LoginSecondFactorHmacKey.<keyId>` en `appSettings` transformable por ambiente; la llave será Base64 de al menos 32 bytes y nunca se versionará con valor real.
- Rotación: la llave activa firma challenges nuevos y las llaves identificadas anteriores pueden verificar únicamente durante su vigencia. Una llave desconocida falla de forma cerrada.

### 9.7. Semántica cerrada de configuración

| Campo | Regla |
|---|---|
| `RequiereSegundoFactor` | `1` activa 2FA. `0` o `NULL` preservan el login legacy. Cualquier otro valor falla de forma cerrada como configuración inválida. |
| `SecondFactorProviderType` | Con 2FA activo, solo `1 = EMAIL` es válido. `NULL`, `2 = TOTP` u otro valor retornan configuración no soportada; no existe fallback. |
| `SegundoFactorTiempoExpira` | Minutos enteros entre 1 y 10. `NULL`, cero o fuera de rango fallan de forma cerrada cuando 2FA está activo. |
| Intentos | Máximo 5 por challenge; el quinto error lo bloquea. |
| Reenvío | Espera mínima de 60 segundos; máximo dos reenvíos además del envío inicial. Cada reenvío revoca el challenge anterior. |
| Correo ausente | Error público genérico; no se omite 2FA y no se crea sesión autenticada. |

El límite superior de 10 minutos corresponde al timeout InProc actual de `Session`; si el ambiente cambia ese timeout, ambos valores deberán revisarse juntos.

## 10. Estados y concurrencia

```text
CREATED -> SENT -> FINALIZING -> COMPLETED
   |        |          |
   |        |          +------> FINALIZATION_FAILED
   |        +-----------------> BLOCKED | EXPIRED | REVOKED
   +--------------------------> DELIVERY_FAILED
```

La creación inserta `CREATED`, envía por SMTP y solo después cambia a `SENT`; un fallo marca `DELIVERY_FAILED`. La verificación abre transacción, lee con bloqueo, valida estado/expiración/vínculo, incrementa intentos atómicamente y cambia un código correcto de `SENT` a `FINALIZING`. Solo una solicitud puede adquirir esa transición. La finalización satisfactoria marca `COMPLETED` y `Consumed = 1`; un fallo posterior queda en `FINALIZATION_FAILED`, limpia la preautenticación y no crea cookie.

Se extenderá la tabla de manera aditiva y compatible con el Core con columnas versionadas para propósito, vínculo de sesión, estado, llave HMAC, conteo/fecha de envíos, fechas terminales, actualización y versión de esquema. Los registros legacy con esos campos nulos no serán elegibles para el flujo nuevo. `AuthPayloadJson` permanecerá `NULL` en nuevos challenges.

La identidad guardada en `AuthUserId` será una clave canónica `empresa:módulo:tipo:idInterno`, no el login ni el correo. Los índices deberán soportar challenge por identidad/propósito/estado, vínculo de sesión/estado y limpieza por estado/expiración. El DDL será idempotente, versionado, con verificación previa y rollback documentado; no se ejecutará en un ambiente sin autorización expresa.

Los estados terminales se conservarán 30 días. La limpieza será una operación de mantenimiento versionada y separadamente autorizada; no se inventará un scheduler dentro de WebForms.

La interfaz deberá impedir que refrescar, volver atrás o repetir el POST finalice el login sin un challenge válido. Existe un riesgo residual: las inicializaciones legacy posteriores a `FINALIZING` no forman una única transacción con la base central. Se mitiga con la transición exclusiva y estado `FINALIZATION_FAILED`; no se afirmará atomicidad distribuida.

## 11. Estrategia Spec-Driven por Jira/OpenSpec

Esta exploración es la fuente arquitectónica común. No debe convertirse en una tarea Jira adicional ni repetirse en un OpenSpec global. Cada prompt de `../propmpt/` representa una tarea Jira independiente y deberá crear su propio cambio OpenSpec, con propuesta, diseño, especificación delta, requisitos `RQ-XX`, decisiones `D-XX`, tareas atómicas y trazabilidad completa.

La secuencia implementable queda cerrada así:

1. Fundación de contratos, criptografía y preautenticación en Session.
2. Migración compatible y repositorio transaccional de challenges.
3. Infraestructura SMTP 2FA dedicada sobre `Config_Smpt_Side`.
4. Separación del login legacy y adaptadores de identidad de los cuatro módulos.
5. Orquestación del challenge y ASMX dedicado, probado directamente antes de UI.
6. Interfaz WebForms y E2E del recorrido incorporado en la misma tarea que la UI.
7. Verificación transversal de seguridad y no regresión.
8. Despliegue, activación gradual, observabilidad y rollback.

Cada tarea debe terminar con sus propias pruebas verdes y corregir allí mismo los defectos que introduzca. El E2E no queda relegado a una tarea tardía aislada: aparece en la integración HTTP cuando sea autorizable, en la UI y nuevamente como regresión transversal.

## 12. Matriz mínima de validación

Cada uno de los cuatro módulos deberá cubrir:

- credenciales inválidas;
- usuario bloqueado o inactivo;
- módulo sin 2FA;
- módulo con 2FA y correo válido;
- módulo con 2FA sin correo;
- fallo de envío de correo;
- OTP correcto;
- OTP incorrecto;
- OTP expirado;
- OTP consumido;
- máximo de intentos;
- reenvío e invalidación del código anterior;
- intentos concurrentes de consumo;
- aislamiento entre empresas, módulos y usuarios con el mismo login;
- ausencia de sesión autenticada antes de verificar el OTP;
- conservación de permisos y navegación actuales después del login.

## 13. Recursos de implementación y prueba existentes

| Recurso | Uso previsto |
|---|---|
| `Infrastructure/Shared/Data/ModuleDataContracts.vb` | Contratos existentes de conexión, ejecución y transacción. |
| `AdoNetDataInfrastructure.vb` | SQL parametrizado y transacciones sin introducir otro stack de datos. |
| `Infrastructure/Shared/Data/WorkflowModuleConnectionFactory.vb` | Precedente de factories por snapshot de conexión. |
| `ModuleSessionConnectionStringResolver.vb` | Única adaptación permitida de Session hacia snapshots en Presentation. |
| `WebServiceWorkflowModern.asmx.vb` | Patrón de ASMX con sesión y DTOs, no destino para métodos de Login. |
| `tests/*.cjs` | Pruebas de contratos/estructura y arneses de compilación existentes. |
| `tools/validation/Doc72SourceValidator` | Precedente Roslyn para validaciones estructurales de VB.NET. |
| `tools/e2e` | Único arnés Playwright permitido para integración/E2E autorizado. |
| `.github/workflows/opsxj-validation.yml` | CI Windows/Node/OpenSpec que deberá ampliarse sin crear un pipeline paralelo. |

No se introducirá un proyecto de aplicación nuevo, un framework de pruebas JavaScript distinto ni un segundo arnés Playwright. Cuando sea necesario probar comportamiento .NET, se reutilizará MSBuild y el patrón de harness C# contra el ensamblado compilado; para estructura VB se podrá ampliar el validador Roslyn existente.

### 13.1. Convención vinculante de rutas

La implementación seguirá la separación ya utilizada por las funcionalidades modernas del repositorio. Las rutas no son sugerencias genéricas:

| Responsabilidad | Ruta canónica | Contenido permitido |
|---|---|---|
| Modelos de dominio y puertos | `Modelo/Login/SegundoFactor/` | Estados, value objects, resultados e interfaces independientes de WebForms, SQL y SMTP. |
| DTOs de la frontera | `DTOs/Login/SegundoFactor/` | Requests/responses del ASMX; no entidades de persistencia ni secretos. |
| Servicios de aplicación | `Services/Login/SegundoFactor/` | Casos de uso y orquestación; no SQL, `HttpContext`, `SmtpClient` ni acceso directo a Session. |
| Repositorios MySQL | `Infrastructure/Repositories/Login/SegundoFactor/` | Configuración central, challenges, identidades por módulo y consultas parametrizadas. |
| Adaptadores técnicos | `Infrastructure/Login/SegundoFactor/` | HMAC/RNG, SMTP y composición técnica no HTTP; organizables en subcarpetas `Security/` y `Smtp/`. |
| Adaptación de Session/HTTP | `webservice/Login/SegundoFactor/` | Gate/contexto pendiente y mapeo de Presentation; es la única capa nueva que puede usar `HttpContext`/Session. |
| Servicio web | `webservice/WebServiceLoginSegundoFactor.asmx` y `webservice/WebServiceLoginSegundoFactor.asmx.vb` | Frontera delgada con `WebMethod`, DTOs y composición; nunca SQL. |
| Página de login | `gestor.aspx`, `gestor.aspx.vb` y recursos ya asociados a esa página | Postback vigente, panel OTP y cliente del ASMX; no una segunda pantalla de login. |
| Integración legacy | `Defaul/ClassGestorSesion.vb` y, solo si la evidencia lo exige, `Defaul/GestorModuleSesion.vb` | Extracción del finalizador y lectura de configuración sin reubicar código ajeno. |
| SQL versionado | `Doc/Actualizacion/Login/Implementacion/<JIRA>/Sql/` | Preflight, apply y rollback de la Jira propietaria; nunca scripts sueltos fuera del paquete. |
| Pruebas estructurales | `tests/` y, cuando corresponda, `tools/validation/` | Contratos CJS y validación Roslyn según patrones existentes. |
| E2E | `tools/e2e/` | Único arnés Playwright, perfiles, utilidades y evidencia autorizada. |

Cada OpenSpec deberá inventariar antes de implementar los archivos exactos que creará o modificará dentro de estas rutas. Una desviación solo es válida si el código vigente demuestra otra convención y el `design.md` registra la evidencia y la decisión. Todos los `.vb`, `.asmx`, scripts y contenidos compilables deberán agregarse explícitamente a `GestionDocumental-Docuarchi.net.vbproj` con el tipo de elemento utilizado por archivos equivalentes.

Queda prohibido colocar repositorios en `Services`, DTOs en `Modelo`, lógica de aplicación dentro del ASMX, acceso a Session dentro de Infrastructure o SQL dentro de Presentation. No se crearán árboles alternos como `src/`, `Application/`, `Data/` o un segundo proyecto para este incremento.

## 14. Verificaciones operativas que no bloquean la especificación

Las decisiones arquitectónicas ya están cerradas. Durante cada tarea todavía se deberá comprobar, sin reinterpretar el diseño:

- nombres físicos exactos de columnas antes de emitir DDL o SQL;
- valores configurados por ambiente antes de activar 2FA;
- disponibilidad de la llave HMAC inyectada y configuración SMTP válida;
- equivalencia del esquema en ambientes diferentes al inspeccionado;
- rutas locales reales permitidas después del login por tipo de módulo;
- autorización vigente antes de consultar bases, enviar correo, migrar o ejecutar E2E real.

Una diferencia física que impida cumplir el contrato debe detener la tarea correspondiente y actualizar la exploración con evidencia; no autoriza un fallback inseguro.

## 15. Decisiones registradas

- El segundo factor deberá funcionar para todos los módulos de DocuArchiNet.
- Radicación Documental es un módulo explícito del alcance; no se tratará como una variante implícita de Gestor Documental.
- La autenticación final continuará usando Forms Authentication y Session de WebForms.
- El envío del OTP usará una clase SMTP nueva y exclusiva para 2FA; reutilizará la conexión actual y la fila única de `Config_Smpt_Side` con `ESTADO_ENVIO = 1`.
- `ClassCorreo`, recuperación de contraseña y los demás recorridos de correo legacy permanecerán sin modificaciones y fuera del flujo 2FA.
- No se reutilizarán `EmailSenderStub`, JWT ni el payload completo del Core.
- No se persistirán credenciales técnicas, contraseña de usuario ni el OTP en texto claro.
- `docuarchi.gestor_modulos` y `docuarchi.ra_auth_second_factor_challenge` serán las fuentes centrales; no se migrarán copias homónimas de otros esquemas sin evidencia de uso.
- La compatibilidad física observada no autoriza reutilizar sin cambios la lógica insegura del Core.
- El recorrido sin 2FA conservará exactamente la validación legacy; no se añadirán reglas de estado a módulos que hoy no las aplican.
- La preparación técnica de conexiones en Session puede ocurrir antes del OTP; identidad autenticada, permisos, auditoría y cookie no.
- Solo EMAIL (`1`) está soportado; TOTP y valores de proveedor inválidos fallan de forma cerrada.
- La vigencia es configurable entre 1 y 10 minutos, con 5 intentos, cooldown de 60 segundos y máximo de 2 reenvíos.
- La llave HMAC se inyectará por ambiente, tendrá identificador versionado y permitirá rotación controlada.
- El ASMX de Login no recibirá identidad o challenge desde el cliente y nunca contendrá SQL.
- La UI y su E2E pertenecen a una misma unidad de entrega; no se considerará terminado el cambio con una UI sin recorrido autorizado o bloqueo explícito.
- La exploración es fuente común; cada prompt crea un OpenSpec autónomo para su Jira.
- Esta exploración autoriza únicamente la preparación documental de prompts; cada implementación requiere ejecutar su Jira/OpenSpec y sus controles.
