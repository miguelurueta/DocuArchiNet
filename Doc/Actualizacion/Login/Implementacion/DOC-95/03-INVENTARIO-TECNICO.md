# Inventario técnico

Repositorio: `DocuArchiNet`. Todas las rutas son relativas a su raíz.

## APIs y entrada legacy

| Tipo | Ruta del archivo | Clase o interfaz | Nombre exacto | Parámetros y tipos | Retorno | Descripción | Relaciones o dependencias |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Postback WebForms | `gestor.aspx.vb` | `gestor` | `Button1_Click` | `sender As Object`, `e As EventArgs` | `Void` | Recibe módulo/usuario/contraseña por controles existentes. Autorización: No aplica, es el inicio. | `ClassGestorSesion` |
| Método legacy | `Defaul/ClassGestorSesion.vb` | `ClassGestorSesion` | `InicioAplicacionWebGestorDocumental` | `modulo As String`, `user As String`, `passs As String`, `nombre_empresa As String` | `String` | Valida credenciales, vacía contraseña e inicia/finaliza preautenticación. | Preauth, composition factory, Forms Authentication solo en rama sin 2FA |
| ASMX POST | `webservice/WebServiceLoginSegundoFactor.asmx.vb` | `WebServiceLoginSegundoFactor` | `/webservice/WebServiceLoginSegundoFactor.asmx/ObtenerEstado` | JSON `{}` | `SegundoFactorEstadoDto` | Session habilitada; requiere contexto pendiente server-side, no identidad cliente. | gate, factory, service, mapper |
| ASMX POST | misma ruta | misma clase | `/webservice/WebServiceLoginSegundoFactor.asmx/Verificar` | JSON `codigo As String` | `SegundoFactorResultadoDto` | Verifica OTP. Autorización: Session pendiente; cookie todavía ausente. | mismos adaptadores |
| ASMX POST | misma ruta | misma clase | `/webservice/WebServiceLoginSegundoFactor.asmx/Reenviar` | JSON `{}` | `SegundoFactorResultadoDto` | Reenvía con cooldown/límite. | mismos adaptadores |
| ASMX POST | misma ruta | misma clase | `/webservice/WebServiceLoginSegundoFactor.asmx/Cancelar` | JSON `{}` | `SegundoFactorResultadoDto` | Revoca/limpia idempotentemente. | mismos adaptadores |

Los cuatro métodos ASMX tienen `WebMethod(EnableSession:=True)` y `ScriptMethod(ResponseFormat:=Json)`. No aceptan usuario, módulo, ID, challenge, nonce, correo o `returnUrl`.

## Servicios, interfaces y adaptadores

| Tipo | Ruta del archivo | Clase o interfaz | Nombre exacto | Parámetros y tipos | Retorno | Descripción | Relaciones o dependencias |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Interfaz | `Modelo/Login/SegundoFactor/SegundoFactorInterfaces.vb` | `ISecondFactorAuthenticationService` | `Start` | `request As SecondFactorAuthenticationStartRequest` | `SecondFactorAuthenticationResult` | Inicia reto. | Implementada por orquestador. |
| Interfaz | misma ruta | misma interfaz | `GetStatus` | No aplica | `SecondFactorAuthenticationResult` | Estado derivado de Session/reloj. | store, clock |
| Interfaz | misma ruta | misma interfaz | `Verify` | `code As String` | `SecondFactorAuthenticationResult` | Valida y finaliza. | protector, repository, finalizer, ticket |
| Interfaz | misma ruta | misma interfaz | `Resend` | No aplica | `SecondFactorAuthenticationResult` | Reemplaza reto. | recipient resolver, sender |
| Interfaz | misma ruta | misma interfaz | `Cancel` | No aplica | `SecondFactorAuthenticationResult` | Revoca y limpia. | repository, store |
| Servicio | `Services/Login/SegundoFactor/SecondFactorAuthenticationService.vb` | `SecondFactorAuthenticationService` | cinco métodos anteriores | firmas de interfaz | mismos retornos | Orquestador puro. | diez puertos funcionales y diagnóstico saneado inyectados |
| Servicio | `Services/Login/SegundoFactor/SecondFactorPreAuthenticationService.vb` | `SecondFactorPreAuthenticationService` | `Execute` | `request As SecondFactorPreAuthenticationRequest` | `SecondFactorPreAuthenticationResult` | Decide rama activa/inactiva y delega `Start` sin contraseña. | módulo, principal, finalizador y orquestador |
| Servicio | `Services/Login/SegundoFactor/SecondFactorRecipientResolver.vb` | `SecondFactorRecipientResolver` | `Resolve` | `identity As SegundoFactorIdentity` | `SecondFactorRecipient` | Re-resuelve principal/correo por tipo e identidad. | resolver DOC-94 |
| Interfaz | `Modelo/Login/SegundoFactor/SegundoFactorInterfaces.vb` | `ISecondFactorRecipientResolver` | `Resolve` | `identity As SegundoFactorIdentity` | `SecondFactorRecipient` | Puerto para re-resolver el destinatario en reenvío. | `SecondFactorRecipientResolver` |
| Interfaz | `Modelo/Login/SegundoFactor/SegundoFactorInterfaces.vb` | `ISecondFactorAuthenticationTicketIssuer` | `Issue` | `normalizedLogin As String` | `Void` | Puerto de cookie. | adaptador Forms Authentication |
| Adaptador | `webservice/Login/SegundoFactor/FormsAuthenticationTicketIssuer.vb` | `FormsAuthenticationTicketIssuer` | `Issue` | `normalizedLogin As String` | `Void` | Llama `SetAuthCookie(login, False)`. | `FormsAuthentication` |
| Interfaz | `Modelo/Login/SegundoFactor/SegundoFactorInterfaces.vb` | `ILoginLocalRouteResolver` | `Resolve` | No aplica | `String` | Puerto de ruta local. | adaptador fijo |
| Adaptador | `webservice/Login/SegundoFactor/LegacyLoginLocalRouteResolver.vb` | `LegacyLoginLocalRouteResolver` | `Resolve` | No aplica | `String` | Devuelve exclusivamente la ruta default legacy. | No aplica |
| Gate | `webservice/Login/SegundoFactor/LoginSecondFactorSessionGate.vb` | `LoginSecondFactorSessionGate` | `AsegurarContexto` | No aplica | `PendingSecondFactorContext` | Materializa/limpia contexto desde Session. | store DOC-91 |
| Mapper | `webservice/Login/SegundoFactor/LoginSecondFactorPresentationMapper.vb` | `LoginSecondFactorPresentationMapper` | `MapState`, `MapResult`, `Failure` | resultado interno o código | DTO público | Aplica allowlist de códigos y descripciones fijas; colapsa desconocidos. | DTO |
| Factoría | `webservice/Login/SegundoFactor/LoginSecondFactorCompositionFactory.vb` | `LoginSecondFactorCompositionFactory` | `Create` | `HttpContext`, `ILegacyLoginFinalizer`, `Long`, `String`, `String` | `ISecondFactorAuthenticationService` | Compone conexiones/repositorios/HMAC/SMTP/Session. | implementaciones DOC-91–94 |
| Factoría | misma ruta | misma clase | `CreateFromPending` | `HttpContext`, `ILegacyLoginFinalizer`, `PendingSecondFactorContext` | `ISecondFactorAuthenticationService` | Variante usada por ASMX. | `Create` |
| Interfaz | `Modelo/Login/SegundoFactor/SegundoFactorInterfaces.vb` | `ISecondFactorDiagnosticSink` | `Record` | `correlationId As Guid`, `stage As SecondFactorDiagnosticStage`, `outcome As SecondFactorDiagnosticOutcome` | `Void` | Puerto cerrado de observabilidad; no acepta texto o datos de identidad. | preautenticación y orquestador |
| Adaptador | `Infrastructure/Login/SegundoFactor/Diagnostics/EventLogSecondFactorDiagnosticSink.vb` | `EventLogSecondFactorDiagnosticSink` | `Record` | misma firma | `Void` | Persiste solo resultados terminales saneados como evento 9501 en `Application`; no crea fuentes. | Windows Event Log; fallback Trace |
| Adaptador fallback | `Infrastructure/Login/SegundoFactor/Diagnostics/TraceSecondFactorDiagnosticSink.vb` | `TraceSecondFactorDiagnosticSink` | `Record` | misma firma | `Void` | Emite correlación/etapa/resultado por `TraceInformation`; nunca altera el login. | `System.Diagnostics.Trace` |
| Adaptador nulo | `Services/Login/SegundoFactor/NullSecondFactorDiagnosticSink.vb` | `NullSecondFactorDiagnosticSink` | `Record` | misma firma | `Void` | Conserva compatibilidad de constructores y pruebas sin infraestructura. | No aplica |

## Repositorios y dependencias reutilizadas

| Tipo | Ruta del archivo | Clase o interfaz | Nombre exacto | Parámetros y tipos | Retorno | Descripción | Relaciones o dependencias |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Repositorio | `Infrastructure/Repositories/Login/SegundoFactor/MySqlSecondFactorChallengeRepository.vb` | `MySqlSecondFactorChallengeRepository` | operaciones de `ISecondFactorChallengeRepository` | challenge/Guid/contador/UTC | modelos o `Boolean` | Persistencia y adquisición exclusiva sin DDL. | `ra_auth_second_factor_challenge`, transacción existente |
| Repositorio | `Infrastructure/Repositories/Login/SegundoFactor/MySqlSecondFactorSmtpConfigurationRepository.vb` | `MySqlSecondFactorSmtpConfigurationRepository` | `Resolve` | No aplica | `SecondFactorSmtpConfigurationResolution` | Selecciona configuración SMTP activa/default `ESTADO_ENVIO=1`. | `Config_Smpt_Side` |
| Servicio | `Infrastructure/Login/SegundoFactor/Smtp/SecondFactorSmtpEmailSender.vb` | `SecondFactorSmtpEmailSender` | `Send` | `SecondFactorEmailMessage` | `SecondFactorDeliveryResult` | Envía OTP reutilizando configuración. | repository SMTP, transport |
| Seguridad | `Infrastructure/Login/SegundoFactor/Security/HmacSecondFactorCodeProtector.vb` | `HmacSecondFactorCodeProtector` | `Protect`, `Verify` | contexto y código/hash | `String`/`Boolean` | Protección HMAC con comparación segura; llave/valor protegido inválidos lanzan falla interna para no consumir intentos. | key provider |
| Store | `webservice/Login/SegundoFactor/SessionPendingSecondFactorContextStore.vb` | `SessionPendingSecondFactorContextStore` | `Save`, `GetCurrent`, `Clear` | contexto o No aplica | `Void`/contexto | Session InProc y expiración. | `HttpSessionStateBase`, clock |
| Finalizador | `Defaul/ClassGestorSesion.vb` | `ClassGestorSesion` | `FinalizeLogin` | `LegacyLoginFinalizationContext` | `LegacyLoginFinalizationResult` | Única finalización legacy para los cuatro módulos. | sesiones/permisos/auditoría legacy |

## Modelos y DTO

| Tipo | Ruta del archivo | Clase o interfaz | Nombre exacto | Parámetros y tipos | Retorno | Descripción | Relaciones o dependencias |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Modelo | `Modelo/Login/SegundoFactor/SegundoFactorModels.vb` | `PendingSecondFactorContext` | constructor completo | `Integer, Integer, String, Long, String, String, Guid, String, Integer, DateTime, DateTime` | No aplica | Autoridad temporal en Session. | store/gate/service |
| Modelo | misma ruta | `SecondFactorAuthenticationStartRequest` | constructores | `SecondFactorLoginModule`, `SecondFactorPrincipal`; sobrecarga con `Guid` | No aplica | Entrada posterior a credenciales; sin contraseña; transporta correlación aleatoria interna. | preauth/service |
| Modelo | misma ruta | `SecondFactorAuthenticationResult` | constructor | `Boolean, String, SecondFactorAuthenticationState, String, Integer, Integer, String` | No aplica | Resultado interno saneado. | service/mapper |
| Enum | misma ruta | `SecondFactorAuthenticationState` | `NONE`, `PENDING`, `COMPLETED`, `REJECTED` | No aplica | No aplica | Estado lógico interno. | result/mapper |
| Enum | misma ruta | `SecondFactorDiagnosticStage` | etapas cerradas de preautenticación, inicio, estado, verificación, reenvío, cancelación y compensación | No aplica | No aplica | Localiza el último límite atravesado sin datos sensibles. | diagnostic sink |
| Enum | misma ruta | `SecondFactorDiagnosticOutcome` | `STARTED`, `SUCCEEDED`, `REJECTED`, `FAILED` | No aplica | No aplica | Resultado diagnóstico cerrado. | diagnostic sink |
| DTO | `DTOs/Login/SegundoFactor/SegundoFactorDtos.vb` | `SegundoFactorEstadoDto` | propiedades | No aplica | No aplica | Estado público. | ASMX |
| DTO | misma ruta | `SegundoFactorResultadoDto` | propiedades | No aplica | No aplica | Resultado público. | ASMX |

## Propiedades, obligatoriedad y validaciones

| DTO/modelo | Propiedad | Tipo | Obligatoria | Validación |
| --- | --- | --- | --- | --- |
| `PendingSecondFactorContext` | `EmpresaId`, `ModuloId`, `InternalUserId` | `Integer`, `Integer`, `Long` | Sí | mayores que cero |
| mismo | `ModuleType`, `NormalizedLogin`, `SessionNonce` | `String` | Sí en contexto nuevo | no nulos/vacíos; normalización; gate rechaza módulo vacío legado |
| mismo | `MaskedDestination` | `String` | Sí | patrón enmascarado |
| mismo | `ChallengeId` | `Guid` | Sí | distinto de vacío |
| mismo | `ResendCount` | `Integer` | Sí | 0 a 2 |
| mismo | `CreatedAtUtc`, `ExpiresAtUtc` | `DateTime` UTC | Sí | UTC y expiración posterior |
| `SecondFactorAuthenticationStartRequest` | `LoginModule`, `Principal`, `CorrelationId` | modelos, `Guid` | Sí | modelos no nulos y GUID distinto de vacío |
| `SecondFactorAuthenticationResult` | `PublicCode`, `State` | `String`, enum | Sí | código no vacío; enum definido |
| mismo | tiempos | `Integer` | Sí | no negativos |
| `SegundoFactorEstadoDto` | `RequiereCodigo` | `Boolean` | Sí | verdadero solo para pendiente |
| mismo | `EstadoPublico` | `String` | Sí | `PENDIENTE`, `COMPLETADO` o `NO_DISPONIBLE` |
| mismo | `DestinoEnmascarado` | `String` | Condicional | nunca correo completo |
| mismo | `ExpiraEnSegundos`, `ReenvioDisponibleEnSegundos` | `Integer` | Sí | calculados, no negativos |
| `SegundoFactorResultadoDto` | `Exito`, `Codigo`, `MensajeVisible`, `Estado` | varios | Sí | mapper controla valores |
| mismo | `RutaLocal` | `String` | Solo éxito completo | ruta fija local; nunca `returnUrl` |
