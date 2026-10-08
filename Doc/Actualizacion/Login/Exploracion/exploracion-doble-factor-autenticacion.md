# Exploración técnica: doble factor de autenticación para DocuArchiNet

Fecha de revisión: 2026-10-08
Estado: exploración arquitectónica; no implementado
Repositorio destino: `D:\imagenesda\DocuachiNet\DocuArchiNet`

## 1. Objetivo

Determinar si la implementación de doble factor de autenticación existente en el workspace `D:\imagenesda\GestorDocumental\DocuArchiCore` puede incorporarse a DocuArchiNet, conservando el login WebForms y las tablas compartidas.

La solución deberá funcionar para todos los módulos autenticables de DocuArchiNet:

- `DOCUARCHI CONTENEDOR`.
- `GESTOR DOCUMENTAL`.
- `RADICACION DOCUMENTAL`.
- `WORKFLOW DOCUMENTAL`.

El envío del código deberá reutilizar la función de correo existente `ClassCorreo.Envio_Correo_recuperacion_pasword`. No se incorporará otro cliente SMTP ni el `EmailSenderStub` del Core.

## 2. Alcance inspeccionado

### 2.1. Repositorio destino: DocuArchiNet

- `gestor.aspx.vb`.
- `Defaul/ClassGestorSesion.vb`.
- `radicador/ClassCorreo.vb`.
- `radicador/ClassRaEnvioCorrespondencia.vb`.
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
- su configuración SMTP y la función existente de envío de correo.

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

El punto correcto para exigir el OTP está después de validar las credenciales y antes de inicializar definitivamente permisos, auditoría y Forms Authentication.

Agregar el OTP al final del método no sería seguro porque para ese momento la aplicación ya habría construido el contexto autenticado.

## 7. Cobertura requerida para todos los módulos

| Tipo de módulo | Tabla | Identificador real requerido | Campo de login | Campo de correo |
|---|---|---|---|---|
| `DOCUARCHI CONTENEDOR` | `usuarios_da` | `Clave_Usuario` | `idusuario` | `correo` |
| `GESTOR DOCUMENTAL` | `remit_dest_interno` | `Id_Remit_Dest_Int` | `Login_Usuario` | `Correo_Electronico` |
| `RADICACION DOCUMENTAL` | `usuario_radicador` | `id_usuario` | `Login_usuario` | `Correo_Usuario` |
| `WORKFLOW DOCUMENTAL` | `usuario_workflow` | `idU_suario` | `login_Usuario` | `Correo_Usuario` |

La función actual `ValidaUserAplicacion` recibe `id_user` por referencia, pero solamente lo llena para DocuArchi Contenedor. Para habilitar 2FA en todos los módulos, la fase de preautenticación deberá obtener de forma inequívoca el ID real y el correo del usuario autenticado en cada tabla.

Un mismo nombre de usuario puede existir en empresas o módulos diferentes. Por tanto, la identidad del challenge deberá componerse como mínimo de:

```text
empresa + módulo + tipo de módulo + ID real del usuario + propósito
```

No se debe confiar en el nombre del módulo, usuario o correo enviados nuevamente por el navegador durante la verificación.

## 8. Reutilización obligatoria del correo existente

La función seleccionada es:

```vb
ClassCorreo.Envio_Correo_recuperacion_pasword(
    Adic_mensaje() As String,
    Corre_dest As String,
    subyect As String
) As String
```

Ubicación: `radicador/ClassCorreo.vb`.

Esta función:

- obtiene la configuración mediante `Obtener_Datos_ConfigSmtp`;
- consulta la configuración activa en `Config_Smpt_Side`;
- utiliza `SmtpClient` con el host, puerto, SSL y credenciales existentes;
- retorna `YES` cuando finaliza el envío;
- ya se utiliza desde `ClassGestorSesion.Recuperar_pasword_usuario` para los cuatro tipos de módulo.

El componente de segundo factor deberá invocar esta función mediante una frontera pequeña de aplicación para evitar que la lógica del challenge conozca detalles SMTP. No se deberá duplicar la construcción de `SmtpClient`.

El mensaje OTP deberá contener solamente:

- el código de verificación;
- su tiempo de expiración;
- una indicación de no compartir el código;
- una indicación para ignorar el mensaje si el usuario no inició el acceso.

No deberá incluir contraseña, hash, identificadores internos, cookies, datos de conexión ni información completa del challenge.

La función es síncrona, por lo que su latencia formará parte de la solicitud inicial del login. Esto deberá considerarse en timeout, experiencia de usuario y pruebas.

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
      ClassCorreo                                |
      .Envio_Correo_recuperacion_pasword         |
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
- usuario activo;
- contraseña correcta;
- ID real del usuario;
- correo registrado, cuando el módulo exige segundo factor.

El resultado interno debe ser tipado y mínimo. No debe conservar la contraseña.

### 9.2. Creación del challenge

Debe:

- usar un identificador aleatorio criptográficamente seguro;
- generar un OTP de seis dígitos con RNG criptográfico;
- almacenar HMAC del OTP, no el OTP;
- registrar propósito `LOGIN`;
- registrar expiración UTC, intentos y estado de consumo;
- ligar el challenge a la sesión WebForms que inició el login;
- invalidar challenges anteriores equivalentes;
- aplicar cooldown y límites por usuario, empresa, módulo e IP;
- enviar el código con la función existente de correo.

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
- `FormsAuthentication.RedirectFromLoginPage`.

Si el módulo no exige segundo factor, este bloque se ejecutará inmediatamente después de validar las credenciales, preservando el comportamiento actual.

## 10. Estados funcionales mínimos

```text
Credenciales pendientes
    -> Credenciales rechazadas
    -> Login sin 2FA
    -> Challenge pendiente
         -> Código incorrecto / intento incrementado
         -> Challenge expirado
         -> Máximo de intentos
         -> Reenvío controlado
         -> Challenge consumido
              -> Login finalizado
```

La interfaz deberá impedir que refrescar, volver atrás o repetir el POST finalice el login sin un challenge válido.

## 11. Estrategia de entrega sugerida

1. Definir y versionar el endurecimiento del esquema ya verificado para challenges y la semántica de los valores nulos de configuración.
2. Separar conceptualmente preautenticación y finalización del login sin cambiar todavía el resultado funcional.
3. Implementar el repositorio atómico del challenge y sus pruebas unitarias/integración.
4. Adaptar `ClassCorreo.Envio_Correo_recuperacion_pasword` para el caso de uso OTP sin duplicar SMTP.
5. Implementar la interfaz de captura, reenvío y validación del código.
6. Probar la matriz completa de los cuatro módulos, tanto con 2FA activado como desactivado.
7. Activar gradualmente por empresa y módulo.

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

## 13. Pendientes de verificación

No se consideran confirmados todavía:

- valores reales y semántica de `RequiereSegundoFactor`, `SecondFactorProviderType` y `SegundoFactorTiempoExpira` por módulo; la inspección confirmó su existencia y tipos, no su configuración funcional;
- DDL definitivo de endurecimiento, índices compuestos y estrategia de limpieza de challenges;
- equivalencia del esquema en otros ambientes distintos del ambiente autorizado inspeccionado;
- política de duración, intentos, reenvío y bloqueo;
- comportamiento deseado cuando la configuración SMTP está deshabilitada;
- mecanismo definitivo para proteger y rotar la clave HMAC;
- alcance de recuperación de contraseña, que deberá permanecer separado del primer incremento de login 2FA.

## 14. Decisiones registradas

- El segundo factor deberá funcionar para todos los módulos de DocuArchiNet.
- Radicación Documental es un módulo explícito del alcance; no se tratará como una variante implícita de Gestor Documental.
- La autenticación final continuará usando Forms Authentication y Session de WebForms.
- El envío del OTP reutilizará `ClassCorreo.Envio_Correo_recuperacion_pasword`.
- No se reutilizarán `EmailSenderStub`, JWT ni el payload completo del Core.
- No se persistirán credenciales técnicas, contraseña de usuario ni el OTP en texto claro.
- `docuarchi.gestor_modulos` y `docuarchi.ra_auth_second_factor_challenge` serán las fuentes centrales; no se migrarán copias homónimas de otros esquemas sin evidencia de uso.
- La compatibilidad física observada no autoriza reutilizar sin cambios la lógica insegura del Core.
- Esta exploración no autoriza todavía cambios en la lógica de aplicación.
