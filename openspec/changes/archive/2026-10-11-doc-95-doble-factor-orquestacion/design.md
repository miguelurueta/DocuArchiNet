<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09,D-10,D-11,D-12,D-13 -->
# Diseño técnico — DOC-95

## Contexto verificado

El código actual contiene `SegundoFactorChallenge`, `PendingSecondFactorContext`, los puertos 2FA, `MySqlSecondFactorChallengeRepository`, `SecondFactorSmtpEmailSender`, `SecondFactorPreAuthenticationService`, `SessionPendingSecondFactorContextStore` y `ClassGestorSesion.FinalizeLogin`. No existe orquestador de challenge ni `WebServiceLoginSegundoFactor`.

DOC-92 usa las diez columnas compartidas de `ra_auth_second_factor_challenge`: `Consumed`, `Attempts` y `ExpiresAtUtc` son las señales persistidas. `TryBeginFinalization` cambia atómicamente `Consumed=0` a `1`; `Complete` y `FailFinalization` no pueden distinguir estados físicos. `AuthPayloadJson` permanece `NULL`. Esta restricción, decidida previamente para compatibilidad sin DDL, prevalece sobre cualquier diagrama ideal de estados.

## Precondiciones de rutas e inventario previsto

| Responsabilidad | Acción | Ruta exacta | Símbolos previstos |
| --- | --- | --- | --- |
| Contratos y modelos | Modificar compatible | `Modelo/Login/SegundoFactor/SegundoFactorInterfaces.vb` | `ISecondFactorAuthenticationService`, `ISecondFactorAuthenticationTicketIssuer`, `ILoginLocalRouteResolver` |
| Contratos y modelos | Modificar compatible | `Modelo/Login/SegundoFactor/SegundoFactorModels.vb` | extensión de `PendingSecondFactorContext`; solicitudes, estados y resultados de orquestación |
| DTO ASMX | Modificar compatible | `DTOs/Login/SegundoFactor/SegundoFactorDtos.vb` | `SegundoFactorEstadoDto`, `SegundoFactorResultadoDto` con ruta local opcional |
| Aplicación | Crear | `Services/Login/SegundoFactor/SecondFactorAuthenticationService.vb` | `SecondFactorAuthenticationService`: `Start`, `GetStatus`, `Verify`, `Resend`, `Cancel` |
| Preautenticación | Modificar | `Services/Login/SegundoFactor/SecondFactorPreAuthenticationService.vb` | delegación de inicio solo para configuración activa |
| Persistencia | Modificar solo si una prueba demuestra la brecha | `Infrastructure/Repositories/Login/SegundoFactor/MySqlSecondFactorChallengeRepository.vb` | conservar firmas y semántica DOC-92; sin DDL ni payload JSON |
| Gate Session | Crear | `webservice/Login/SegundoFactor/LoginSecondFactorSessionGate.vb` | resolución exclusiva desde `HttpContext.Current.Session` |
| Composición | Crear | `webservice/Login/SegundoFactor/LoginSecondFactorCompositionFactory.vb` | composición de repositorio, HMAC, SMTP, servicio, finalizador y adaptadores WebForms |
| Presentación | Crear | `webservice/Login/SegundoFactor/LoginSecondFactorPresentationMapper.vb` | mapeo a DTO sanitizado |
| Cookie/ruta | Crear | `webservice/Login/SegundoFactor/FormsAuthenticationTicketIssuer.vb` y `LegacyLoginLocalRouteResolver.vb` | emisión de cookie y allowlist local |
| Frontera HTTP | Crear | `webservice/WebServiceLoginSegundoFactor.asmx` y `webservice/WebServiceLoginSegundoFactor.asmx.vb` | `ObtenerEstado()`, `Verificar(codigo As String)`, `Reenviar()`, `Cancelar()` |
| Entrada legacy | Modificar | `Defaul/ClassGestorSesion.vb` | composición e inicio; `FinalizeLogin` sigue siendo el único finalizador |
| Registro | Modificar | `GestionDocumental-Docuarchi.net.vbproj` | una entrada por cada fuente y contenido ASMX |
| Pruebas | Crear/modificar | `tests/` | pruebas conductuales y contrato ASMX aisladas de infraestructura real |
| E2E | Crear mediante registro común | `tools/e2e/` | escenario DOC-95 sin login/configuración/Playwright paralelo |
| Documentación | Crear | `Doc/Actualizacion/Login/Implementacion/DOC-95/` | arquitectura, casos, inventario, diagramas y evidencia |

## D-01 — Reutilización y compatibilidad física

Se reutilizan los contratos e implementaciones DOC-91 a DOC-94. No se crean tablas, columnas, índices ni procedimientos; `AuthPayloadJson` continúa `NULL`. `Consumed=1` es la adquisición exclusiva e irreversible. El servicio puede expresar estados lógicos durante una operación, pero no afirmará que `FINALIZING`, `COMPLETED` o `FINALIZATION_FAILED` quedan diferenciados en la tabla.

## D-02 — Servicio único de aplicación

`SecondFactorAuthenticationService` coordina cinco casos de uso: `Start`, `GetStatus`, `Verify`, `Resend` y `Cancel`. Depende solo de puertos del modelo: reloj, OTP, protector HMAC, repositorio, correo, contexto pendiente, finalizador, emisor de ticket y resolvedor de ruta. No conoce `HttpContext`, `Session`, `FormsAuthentication`, SQL ni `SmtpClient`.

## D-03 — Inicio conectado a preautenticación

Cuando `SecondFactorPreAuthenticationService` obtiene configuración activa e identidad con correo, delega `Start`. El servicio genera `Guid`, nonce criptográfico y OTP; construye identidad canónica; protege el OTP con propósito, challenge, identidad y nonce; crea la fila; envía mediante `ISecondFactorEmailSender`; confirma `MarkSent` y guarda `PendingSecondFactorContext`. Si cualquier paso falla, consume mediante `MarkDeliveryFailed` cuando corresponda, limpia Session y retorna un código público genérico. Con 2FA apagado no se invoca ninguna dependencia nueva y el finalizador legacy conserva su recorrido.

## D-04 — Session como única autoridad HTTP

El contexto pendiente contiene empresa, módulo, tipo de módulo, usuario interno, login normalizado, destino enmascarado, challenge, nonce, contador de reenvío y tiempos UTC. Solo `SessionPendingSecondFactorContextStore` lo conserva. El cliente nunca entrega empresa, módulo, login, ID, challenge, nonce, destino ni URL de retorno. Una sesión ausente, expirada o reemplazada limpia el contexto y obtiene respuesta genérica.

## D-05 — Verificación y concurrencia

`Verify` rechaza antes del repositorio todo valor que no tenga exactamente seis dígitos. Reconstruye identidad y `SecondFactorProtectionContext` desde Session, obtiene `GetVerificationData`, exige igualdad de identidad canónica y verifica HMAC con el nonce actual. Un código incorrecto usa `RegisterFailedAttemptData`; el quinto intento deja el challenge bloqueado y limpia Session. Un código correcto debe ganar `TryBeginFinalization`; dos solicitudes simultáneas solo permiten un ganador porque la transición física es `Consumed=0 -> 1`.

## D-06 — Finalización, cookie y riesgo residual

El ganador llama una sola vez a `ILegacyLoginFinalizer.FinalizeLogin`. Si falla, invoca `FailFinalization`, limpia Session y no emite cookie. Si tiene éxito, exige `Complete=True`; solo entonces `ISecondFactorAuthenticationTicketIssuer` llama `FormsAuthentication.SetAuthCookie`, se limpia Session y se retorna la ruta de `ILoginLocalRouteResolver`.

La ruta permitida es exclusivamente `~/Defaul/WebFormInicioDocuarchiGestion.aspx`, que coincide con `Web.config`. No se lee ni refleja `returnUrl`. Los efectos de Session/auditoría ejecutados por el finalizador legacy no son compensables si luego falla `Complete` o la emisión de cookie; el challenge permanece consumido y el usuario reinicia el login. Esta limitación se documenta y prueba, sin afirmar atomicidad distribuida.

## D-07 — Reenvío y cancelación

`Resend` exige contexto elegible, 60 segundos desde `CreatedAtUtc` y menos de dos reenvíos. Re-resuelve el correo desde repositorios autorizados; nunca lo toma de Session o cliente. Genera challenge/nonce/OTP nuevos y llama `ReplaceForResend`, que revoca e inserta atómicamente. Solo tras envío y `MarkSent` sustituye el contexto pendiente con contador incrementado. Si falla SMTP, consume el reemplazo y limpia Session. `Cancel` revoca el challenge activo, limpia Session de forma idempotente y nunca autentica.

## D-08 — Frontera ASMX exclusiva

`WebServiceLoginSegundoFactor` sigue el patrón de `WebServiceWorkflowModern`: `ScriptService`, `WebService`, `WebServiceBinding`, `ToolboxItem(False)`, `ScriptMethod(ResponseFormat:=Json)` y `WebMethod(EnableSession:=True)`. Expone exactamente:

- `ObtenerEstado() As SegundoFactorEstadoDto`
- `Verificar(codigo As String) As SegundoFactorResultadoDto`
- `Reenviar() As SegundoFactorResultadoDto`
- `Cancelar() As SegundoFactorResultadoDto`

El code-behind solo aplica gate, composición y mapper. No contiene SQL, SMTP ni reglas del challenge y no modifica otros ASMX.

## D-09 — Contrato público mínimo

Los DTO exponen únicamente éxito, código/mensaje controlados, estado público, destino enmascarado, segundos de expiración/cooldown y ruta local tras completar. No retornan challenge, IDs, login, email, nonce, intentos internos, HMAC, excepción, SQL o detalle SMTP. Todos los fallos sensibles se colapsan a códigos públicos estables.

## D-10 — Composición WebForms y secretos

`LoginSecondFactorCompositionFactory` reutiliza `AdoNetDataExecutor`, `DbTransactionFactory`, factorías de conexión ya resueltas por Session, repositorios DOC-92/DOC-94, HMAC/appSettings y SMTP DOC-93. La contraseña validada no entra al orquestador. Infrastructure no accede a Session. No se modifica `ClassCorreo`, recuperación de contraseña, configuración productiva ni datos reales.

## D-11 — Pruebas y E2E autorizado

Las pruebas locales compilan fuentes aisladas y cubren inicio, estado, formato inválido, OTP erróneo/correcto, expiración, quinto intento, fallo SMTP, fallo de finalizador, doble adquisición, cooldown, dos reenvíos, cancelación, cookie/ruta y contrato ASMX. Se ejecutan regresiones DOC-91 a DOC-94, validación documental, OpenSpec y MSBuild.

La E2E reutiliza exclusivamente `tools/e2e` y su TTY. Requiere autorización nueva y explícita para ambiente, cuenta, buzón, escritura de challenge, concurrencia y datos descartables. Las consultas de control son solo `SELECT`; no se guardan secretos, OTP, cookies o conexiones. Sin esa autorización el cambio no se presenta como cerrado: se registra el bloqueo real.

## D-12 — Documentación y rollback

La documentación DOC-95 representa código implementado, cuatro endpoints, estados lógicos frente a señales físicas, errores y efectos no compensables. Diagramas y firmas se validan estructuralmente. Rollback: desactivar 2FA por la configuración existente y revertir el artefacto; no hay rollback SQL.

## D-13 — Observabilidad saneada y estabilidad de arranque

La preautenticación genera un `CorrelationId` aleatorio y lo propaga internamente hasta `Start`. `ISecondFactorDiagnosticSink` solo acepta ese GUID y enums cerrados de etapa/resultado; no admite texto libre, identidad, correo, OTP, challenge, nonce, HMAC, conexión o excepción. `SecondFactorPreAuthenticationService` y `SecondFactorAuthenticationService.Start` registran los límites críticos. `EventLogSecondFactorDiagnosticSink` persiste únicamente rechazo, falla o inicio completado como evento 9501 en la fuente existente `Application`; no crea fuentes. Si Event Log no está disponible delega a `TraceSecondFactorDiagnosticSink`, mecanismo ya presente en el repositorio. Todo acceso al sink está aislado para que una falla diagnóstica nunca cambie autenticación.

La escritura y retención de Event Log con la identidad real del AppPool deben verificarse como precondición operativa. También debe ejecutarse una corrida controlada con sesiones nuevas después de reciclar el AppPool; el cambio no autoriza por sí mismo esa acción disruptiva. No se agregan reintentos SMTP automáticos porque podrían duplicar códigos enviados y ocultar el estado real de entrega.

## D-14 — Catálogo público y diagnóstico posterior al inicio

`LoginSecondFactorPresentationMapper` conserva el DTO existente y convierte únicamente códigos aprobados en `Codigo` y descripciones fijas en `MensajeVisible`; cualquier valor ajeno al catálogo se reemplaza por `SECOND_FACTOR_UNAVAILABLE`. El límite legacy devuelve `LOGIN_UNAVAILABLE` en vez de concatenar `Exception.Message`.

`Verify`, `GetStatus`, `Resend` y `Cancel` generan correlaciones internas por operación y registran etapas cerradas mediante el sink existente. La causa SMTP se conserva como categoría diagnóstica cuando corresponde a configuración deshabilitada, inválida o ambigua, mientras el cliente continúa recibiendo el error genérico de entrega. El protector HMAC lanza una falla interna ante llave ausente o valor protegido inválido; solo una comparación HMAC válida con resultado negativo representa OTP incorrecto y puede consumir un intento.

## Riesgos

- La tabla compartida no diferencia estados terminales; el diagnóstico persistente es limitado.
- SMTP síncrono añade latencia antes de mostrar el reto.
- Session InProc invalida el contexto ante reciclado o cambio de nodo sin afinidad.
- El finalizador legacy produce efectos no transaccionales con la tabla central.
- La UI del challenge pertenece a otra entrega; DOC-95 valida la frontera HTTP antes de esa UI.

## Preguntas cerradas por evidencia

- Ruta posterior: `~/Defaul/WebFormInicioDocuarchiGestion.aspx` de `Web.config`.
- Persistencia: tabla y columnas existentes, sin DDL.
- SMTP: fila `ESTADO_ENVIO=1` mediante el repositorio DOC-93.
- Identidad HTTP: exclusivamente Session; ningún endpoint recibe identidad o `returnUrl`.
