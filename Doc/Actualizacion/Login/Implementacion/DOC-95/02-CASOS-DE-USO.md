# Casos de uso implementados

## UC-01 — Iniciar challenge después de credenciales válidas

**Actor:** usuario de cualquiera de los cuatro módulos soportados.

**Precondiciones:** credenciales válidas; configuración de módulo resuelta; 2FA activo; principal único con correo; conexiones de Session, HMAC y SMTP disponibles.

**Entrada:** postback de `gestor.aspx`; no existe endpoint alterno de credenciales.

**Participantes:** `ClassGestorSesion.InicioAplicacionWebGestorDocumental` → `SecondFactorPreAuthenticationService.Execute` → `SecondFactorAuthenticationService.Start` → `ISecondFactorChallengeRepository`/`ISecondFactorEmailSender`/`IPendingSecondFactorContextStore`.

Flujo principal: crea challenge/nonce/OTP, protege el OTP, persiste, envía, marca enviado, guarda contexto pendiente y retorna `SECOND_FACTOR_REQUIRED`. No emite cookie.

Alternos y errores: módulo/principal/correo ausente, fallo HMAC, creación, SMTP o `MarkSent` producen rechazo saneado; se consume el challenge cuando corresponde y se limpia Session. Con 2FA desactivado, se ejecuta `FinalizeLogin` y el comportamiento previo continúa.

**Resultado:** contexto pendiente o login legacy finalizado. Endpoint participante: No aplica en el inicio; es postback WebForms.

## UC-02 — Consultar estado pendiente

**Actor:** cliente con la misma Session.

**Precondición:** challenge iniciado.

**Endpoint:** `POST /webservice/WebServiceLoginSegundoFactor.asmx/ObtenerEstado`.

**Participantes:** `WebServiceLoginSegundoFactor.ObtenerEstado` → `LoginSecondFactorSessionGate.AsegurarContexto` → `ISecondFactorAuthenticationService.GetStatus` → `LoginSecondFactorPresentationMapper.MapState`.

Flujo principal: obtiene el contexto solo de Session y calcula destino enmascarado, segundos restantes y cooldown. Sesión ausente, expirada o sin `ModuleType` se limpia y retorna `NO_DISPONIBLE`, sin identidad interna.

**Resultado:** `SegundoFactorEstadoDto` saneado.

## UC-03 — Verificar OTP y completar login

**Actor:** cliente con Session pendiente.

**Precondiciones:** OTP de seis dígitos, challenge vigente y no consumido.

**Endpoint:** `POST /webservice/WebServiceLoginSegundoFactor.asmx/Verificar`, cuerpo `{"codigo":"......"}`.

**Participantes:** ASMX → `Verify` → challenge repository/protector → `ClassGestorSesion.FinalizeLogin` → `Complete` → route resolver → ticket issuer.

Flujo principal: valida formato, reconstruye identidad, verifica HMAC, gana la adquisición exclusiva, finaliza el login legacy, confirma persistencia, resuelve ruta local, emite una cookie no persistente y limpia Session.

Alternos y errores:

- formato inválido: rechazo antes del repositorio;
- OTP incorrecto: incrementa intento; el quinto bloquea y limpia Session;
- llave HMAC ausente, versión o valor protegido inválido: falla interna cerrada, no incrementa intentos y se registra la etapa `CODE_VERIFICATION`;
- identidad/nonce/expiración/consumo incompatibles: fallo cerrado;
- dos verificaciones: un solo ganador de `TryBeginFinalization`;
- fallo de finalizador, `Complete`, ruta o cookie: no se retorna éxito; la Session pendiente se limpia.

**Resultado:** `SECOND_FACTOR_COMPLETED` con ruta local, o resultado genérico sin secretos.

## UC-04 — Reenviar OTP

**Actor:** cliente con Session pendiente.

**Precondiciones:** pasaron 60 segundos; menos de dos reenvíos; principal/correo aún resolubles.

**Endpoint:** `POST /webservice/WebServiceLoginSegundoFactor.asmx/Reenviar`.

**Participantes:** ASMX → `Resend` → `SecondFactorRecipientResolver.Resolve` → `ReplaceForResend` → sender → `MarkSent` → store.

El servicio reconsulta el correo; no usa un correo aportado por cliente o Session. Genera challenge, nonce y OTP nuevos; revoca/reemplaza atómicamente el anterior y solo cambia Session después del envío confirmado. Cooldown y límite conservan estado pendiente. Fallo de resolución, SMTP o confirmación consume el reemplazo y limpia Session.

**Resultado:** `SECOND_FACTOR_RESENT`, cooldown/límite, o fallo cerrado.

## UC-05 — Cancelar challenge

**Actor:** cliente con o sin Session pendiente.

**Endpoint:** `POST /webservice/WebServiceLoginSegundoFactor.asmx/Cancelar`.

**Participantes:** ASMX → `Cancel` → `ISecondFactorChallengeRepository.Revoke` → store.

Si existe contexto, intenta revocar el challenge. Siempre limpia Session y retorna cancelación de manera idempotente; nunca autentica ni emite cookie. Errores internos de revocación no se exponen.

**Resultado:** `SECOND_FACTOR_CANCELLED`.

## UC-06 — Mapear respuestas públicas

**Actor:** cliente ASMX.

**Participantes:** `LoginSecondFactorPresentationMapper.MapState`, `MapResult`, `Failure`.

Expone solo estado público, destino enmascarado, tiempos, código/mensaje controlados y ruta local en éxito completo. Cada código aprobado tiene una descripción fija y segura; cualquier código desconocido se transforma en `SECOND_FACTOR_UNAVAILABLE`. Challenge, IDs, login, correo completo, nonce, intentos, HMAC, excepción, SQL y SMTP nunca aparecen en DTO.
