<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09 -->
## Context

DOC-91 crea la fundación interna de segundo factor para el login WebForms VB.NET sobre .NET Framework 4.6.1. La fuente arquitectónica es `Doc/Actualizacion/Login/Exploracion/exploracion-doble-factor-autenticacion.md`; se verificaron los patrones de modelos y puertos en `Modelo/Workflow/Terminar/`, DTO serializable en `DTOs/Workflow/Terminar/TransicionWorkflowDtos.vb`, frontera de Session en `webservice/WorkflowPreviewSessionContextGate.vb` e inclusión explícita en `GestionDocumental-Docuarchi.net.vbproj`.

La entrega permanece inactiva: no modifica ni es invocada por `gestor.aspx`, `ClassGestorSesion`, `ClassCorreo`, Forms Authentication, recuperación de contraseña, MySQL o SMTP.

## Goals / Non-Goals

**Goals**

- Definir modelos, resultados y puertos independientes de WebForms, SMTP y SQL.
- Implementar OTP criptográfico, HMAC-SHA256 y lectura inyectable de llaves compatibles con net461.
- Encapsular el contexto mínimo pendiente en Session, en la frontera WebForms.
- Incorporar pruebas focales y registrar los fuentes VB en el proyecto existente.

**Non-Goals**

- Implementar repositorios MySQL, migraciones, servicios de orquestación, SMTP, ASMX o UI.
- Activar 2FA, finalizar el login o copiar JWT, `UserAuthContext`, `EmailSenderStub` o contratos HTTP del Core.

## Inventario de archivos y símbolos

Se conserva el patrón de clases globales bajo el `RootNamespace`; no se agrega un bloque `Namespace` explícito.

| Archivo | Símbolos previstos | Responsabilidad |
| --- | --- | --- |
| `Modelo/Login/SegundoFactor/SegundoFactorModels.vb` | `SegundoFactorPurpose`, `SegundoFactorChallengeState`, `SegundoFactorIdentity`, `SegundoFactorConfiguration`, `SegundoFactorChallenge`, `SecondFactorProtectionContext`, `SecondFactorKeyMaterial`, `SecondFactorRecipient`, `SecondFactorEmailMessage`, `SecondFactorDeliveryResult`, `LegacyLoginFinalizationResult`, `PendingSecondFactorContext` | Modelo tipado e invariantes sin infraestructura. |
| `Modelo/Login/SegundoFactor/SegundoFactorInterfaces.vb` | `ISecondFactorClock`, `ISecondFactorOtpGenerator`, `ISecondFactorCodeProtector`, `ISecondFactorKeyProvider`, `ISecondFactorConfigurationRepository`, `ISecondFactorChallengeRepository`, `ISecondFactorRecipientResolver`, `ISecondFactorEmailSender`, `ILegacyLoginFinalizer`, `IPendingSecondFactorContextStore` | Puertos pequeños; los de repositorio no implementan SQL en DOC-91. |
| `DTOs/Login/SegundoFactor/SegundoFactorDtos.vb` | `SegundoFactorEstadoDto`, `SegundoFactorResultadoDto` | Respuesta pública sanitizada prevista para una frontera futura. |
| `Infrastructure/Login/SegundoFactor/Security/SystemSecondFactorClock.vb` | `SystemSecondFactorClock` | UTC detrás de un reloj inyectable. |
| `Infrastructure/Login/SegundoFactor/Security/CryptographicSecondFactorOtpGenerator.vb` | `CryptographicSecondFactorOtpGenerator` | OTP uniforme de seis dígitos mediante RNG y rechazo. |
| `Infrastructure/Login/SegundoFactor/Security/HmacSecondFactorCodeProtector.vb` | `HmacSecondFactorCodeProtector` | Protección HMAC-SHA256 y comparación XOR constante. |
| `Infrastructure/Login/SegundoFactor/Security/AppSettingsSecondFactorKeyProvider.vb` | `ISecondFactorSettings`, `ConfigurationManagerSecondFactorSettings`, `AppSettingsSecondFactorKeyProvider` | Adaptar configuración externa y anillo de llaves. |
| `webservice/Login/SegundoFactor/SessionPendingSecondFactorContextStore.vb` | `SessionPendingSecondFactorContextStore` | Guardar, recuperar y limpiar contexto mínimo en `HttpSessionStateBase`. |
| `tests/login-second-factor-foundation.test.cjs` | Suite CJS y arnés compilable | Verificar seguridad, invariantes, Session y límites. |
| `GestionDocumental-Docuarchi.net.vbproj` | Entradas `Compile` | Registrar explícitamente los archivos VB. |

## Contratos de métodos

- `ISecondFactorClock.UtcNow As DateTime`; `ISecondFactorOtpGenerator.GenerateCode() As String`.
- `ISecondFactorCodeProtector.Protect(context As SecondFactorProtectionContext, code As String) As String` y `Verify(context As SecondFactorProtectionContext, code As String, protectedCode As String) As Boolean`.
- `ISecondFactorKeyProvider.GetActiveKey() As SecondFactorKeyMaterial` y `TryGetKey(keyId As String, ByRef key As SecondFactorKeyMaterial) As Boolean`.
- `ISecondFactorConfigurationRepository.GetConfiguration(moduleId As Integer) As SegundoFactorConfiguration`; `ISecondFactorChallengeRepository` expone crear, leer, registrar intento, adquirir finalización, completar, fallar finalización y revocar sin conexión ni SQL.
- `ISecondFactorRecipientResolver.Resolve(identity As SegundoFactorIdentity) As SecondFactorRecipient`; `ISecondFactorEmailSender.Send(message As SecondFactorEmailMessage) As SecondFactorDeliveryResult`.
- `ILegacyLoginFinalizer.FinalizeLogin(context As PendingSecondFactorContext) As LegacyLoginFinalizationResult`.
- `IPendingSecondFactorContextStore.Save(context As PendingSecondFactorContext)`, `GetCurrent() As PendingSecondFactorContext` y `Clear()`.

## Decisions

### D-01 — Alcance físico cerrado y fundación inactiva

Solo se crean archivos del inventario. No se crean `Services/Login/SegundoFactor/`, `Infrastructure/Repositories/Login/SegundoFactor/` ni `webservice/WebServiceLoginSegundoFactor.asmx(.vb)`. Ningún archivo productivo referencia los tipos nuevos.

### D-02 — Dominio canónico y política cerrada

Los modelos representan propósito `LOGIN`; estados `CREATED`, `SENT`, `FINALIZING`, `COMPLETED`, `DELIVERY_FAILED`, `BLOCKED`, `EXPIRED`, `REVOKED`, `FINALIZATION_FAILED`; 5 intentos, cooldown 60 segundos, un envío inicial y 2 reenvíos. `RequiereSegundoFactor` acepta 1 activo y 0/NULL desactivado; otro valor es inválido. Solo `EMAIL = 1` se soporta y la expiración válida es 1..10 minutos. El login se normaliza de forma determinista.

### D-03 — OTP criptográfico uniforme y reloj inyectable

El OTP contiene seis dígitos. El generador usa `RandomNumberGenerator` y descarta muestras fuera del mayor múltiplo entero del espacio de 1.000.000 para evitar sesgo. `ISecondFactorClock` permite probar expiración sin reloj real.

### D-04 — HMAC versionado y ligado al contexto

La salida es `v1:<keyId>:<base64mac>`. La entrada canónica incluye propósito, challenge, identidad, vínculo de sesión y OTP con delimitación no ambigua. La verificación rechaza formato, versión o llave desconocida y compara bytes de igual longitud mediante acumulación XOR.

### D-05 — Llaves externas, rotables y fallo cerrado

El proveedor consume `LoginSecondFactorHmacActiveKeyId` y `LoginSecondFactorHmacKey.<keyId>` mediante `ISecondFactorSettings`. Cada material es Base64 de al menos 32 bytes. `Protect` usa la activa; `Verify` resuelve la llave indicada en el valor almacenado. Ausencia o material inválido no degrada la seguridad. No se agregan secretos a `Web.config`.

### D-06 — Session como adaptador mínimo de presentación

El adaptador depende de `HttpSessionStateBase` y solo guarda IDs resueltos, login normalizado, destino enmascarado, challenge, nonce y tiempos. No admite contraseña, OTP, correo completo, HMAC, llave, credenciales u objetos de conexión. Dominio y puertos no dependen de `System.Web`.

### D-07 — DTO público sanitizado

Los DTO serializables exponen éxito, código público, mensaje neutro, destino enmascarado y segundos de expiración/reenvío. No exponen IDs internos, login, correo completo, estado persistido, SQL, excepción o secretos. DOC-91 no publica endpoint.

### D-08 — Validación focal y antirregresión estructural

Las pruebas cubren criptografía, invariantes, Session, registro de proyecto y ausencia de referencias productivas o cambios legacy prohibidos. El proyecto compila con MSBuild Debug y la evidencia sanitizada se registra en `Doc/Actualizacion/Login/Implementacion/DOC-91/`.

### D-09 — Documentación ejecutable contra código

Cuatro diagramas Mermaid requeridos se inventarían en `diagram-contract.json` y distinguen `CODE`, `EXT` y `CONCEPT`. Mermaid valida sintaxis real y Roslyn Visual Basic resuelve declaraciones, enums, pertenencia de métodos, parámetros, retornos, sobrecargas y propiedades DTO. La documentación declara expresamente los puertos sin implementación y los límites que estas pruebas no demuestran.

## Risks / Trade-offs

- Los puertos fijan semántica antes del repositorio MySQL; una variación posterior exige una decisión nueva.
- El adaptador simulado no valida aún el ciclo WebForms real.
- Las llaves anteriores deben conservarse hasta vencer sus challenges.
- Estas pruebas no demuestran login completo, entrega SMTP ni concurrencia MySQL.

## Migration Plan

1. Agregar contratos, modelos, DTO y primitivas sin referencias desde producción.
2. Agregar el adaptador Session y entradas `Compile`.
3. Ejecutar pruebas focales, MSBuild y revisión de archivos legacy.
4. Documentar evidencia. La activación corresponde a tickets posteriores.

## Rollback

Retirar entradas `Compile`, archivos nuevos y pruebas. No hay reversa de datos, configuración, autenticación ni UI.

## Open Questions

No hay preguntas abiertas para DOC-91; MySQL, SMTP, endpoints e interfaz pertenecen a cambios posteriores.
