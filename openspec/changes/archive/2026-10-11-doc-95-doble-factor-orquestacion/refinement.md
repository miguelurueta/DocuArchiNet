<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento — doc-95-doble-factor-orquestacion

## Fuente y alcance

- Ticket: `DOC-95` — DOBLE-FACTOR-ORQUESTACION.
- Perfil tecnológico: `legacy-webforms-vb`, .NET Framework 4.6.1, ASMX, Session InProc y MySQL/ODBC existentes.
- Alcance: orquestador de challenge y frontera HTTP previa a la UI.
- Restricción prioritaria confirmada: sin tablas, columnas, índices o configuración productiva nueva.

## Contexto inspeccionado

- Fundación: `Modelo/Login/SegundoFactor/SegundoFactorModels.vb`, `SegundoFactorInterfaces.vb`, `DTOs/Login/SegundoFactor/SegundoFactorDtos.vb`.
- Persistencia: `MySqlSecondFactorChallengeRepository`; semántica física `Consumed`/`Attempts`/`ExpiresAtUtc`, payload nulo y adquisición `0 -> 1`.
- SMTP: repositorio de `Config_Smpt_Side`, `SecondFactorSmtpEmailSender` y transporte síncrono.
- Preautenticación/finalizador: `SecondFactorPreAuthenticationService` y `ClassGestorSesion.FinalizeLogin` para cuatro módulos.
- WebForms: `SessionPendingSecondFactorContextStore`, patrón `WebServiceWorkflowModern.asmx.vb`, Forms Authentication y ruta por defecto de `Web.config`.
- E2E: `tools/e2e/AGENT-RUNBOOK.md`; ninguna autorización DOC-95 vigente se presume.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia de código | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | Reutilizar tabla exacta, payload nulo y consumo exclusivo sin fingir estados físicos. | `MySqlSecondFactorChallengeRepository.vb`; diseño DOC-92 | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | Un servicio de aplicación coordina cinco casos sin dependencias HTTP/SQL/SMTP concretas. | Interfaces existentes y límites de `SecondFactorPreAuthenticationService.vb` | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | La rama 2FA activa crea/envía/guarda challenge; la apagada no cambia. | `SecondFactorPreAuthenticationService.Execute`; `ClassGestorSesion` | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | Session es la única autoridad de identidad y nonce para ASMX. | `SessionPendingSecondFactorContextStore.vb` | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | Formato, HMAC, intentos y adquisición atómica preceden al finalizador. | protector HMAC; repositorio `RegisterFailedAttemptData`/`TryBeginFinalization` | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | Finalizador y `Complete` preceden a cookie; fallos no compensables reinician login. | `ILegacyLoginFinalizer`; semántica DOC-92; ruta `Web.config` | D-06 | RQ-06 | Origen: D-06, RQ-06 |
| D-07 | Reenvío genera valores nuevos, respeta 60 s/dos veces; cancelar revoca y limpia. | constantes de `SegundoFactorConfiguration`; `ReplaceForResend`/`Revoke` | D-07 | RQ-07 | Origen: D-07, RQ-07 |
| D-08 | Un ASMX exclusivo expone exactamente cuatro operaciones con Session. | patrón `WebServiceWorkflowModern.asmx.vb` | D-08 | RQ-08 | Origen: D-08, RQ-08 |
| D-09 | DTO mínimo y ruta local server-side; no `returnUrl` ni datos internos. | DTO existentes; `Web.config` defaultUrl | D-09 | RQ-09 | Origen: D-09, RQ-09 |
| D-10 | La composición reutiliza implementaciones DOC-91 a DOC-94 y mantiene secretos en sus adaptadores. | factorías, repositorios, HMAC y SMTP existentes | D-10 | RQ-10 | Origen: D-10, RQ-10 |
| D-11 | Pruebas locales obligatorias y E2E real solo con autorización nueva; sin sustitución ficticia. | suites login existentes; `tools/e2e/AGENT-RUNBOOK.md` | D-11 | RQ-11 | Origen: D-11, RQ-11 |
| D-12 | Documentar código real, riesgo residual y rollback sin SQL. | documentación DOC-91 a DOC-94 y CI documental | D-12 | RQ-12 | Origen: D-12, RQ-12 |
| D-13 | Correlacionar etapas cerradas sin secretos y verificar estabilidad post-reciclado. | incidente transitorio E2E; patrón Event Log existente y fallback `Trace` | D-13 | RQ-13 | Origen: D-13, RQ-13 |
| D-14 | Exponer solo errores públicos catalogados, conservar diagnóstico interno por etapa y no penalizar al usuario por fallos criptográficos internos. | `LoginSecondFactorPresentationMapper.vb`; `SecondFactorAuthenticationService.vb`; `HmacSecondFactorCodeProtector.vb`; `ClassGestorSesion.vb` | D-14 | RQ-14 | Origen: D-14, RQ-14 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | Challenge exclusivo con esquema existente. | Un ganador cambia `Consumed=0 -> 1`; payload nulo. | Estados terminales no distinguibles físicamente. |
| RQ-02 | Cinco operaciones en servicio aislado. | Inspección no encuentra APIs concretas de infraestructura. | Evita reglas duplicadas en ASMX. |
| RQ-03 | Inicio 2FA entrega reto o falla cerrado. | SMTP exitoso guarda contexto; fallo limpia; bandera apagada preserva login. | No regresión de cuatro módulos. |
| RQ-04 | Ninguna identidad llega del cliente. | Sesión/nonce inválido rechaza genéricamente. | Session InProc requiere afinidad. |
| RQ-05 | OTP y concurrencia son seguras. | Formato previo, quinto intento y doble submit con un ganador. | Challenge consumido no reintentable. |
| RQ-06 | Cookie posterior a finalización confirmada. | Solo éxito total emite cookie/ruta fija. | Efectos legacy previos pueden no compensarse. |
| RQ-07 | Reenvío/cancelación acotados. | 60 s, máximo dos, OTP nuevo, revocación y limpieza. | Contador reside en Session. |
| RQ-08 | ASMX exacto y delgado. | Cuatro métodos, Session, JSON, sin SQL/SMTP. | UI todavía fuera de alcance. |
| RQ-09 | DTO saneado. | Ningún secreto/ID/challenge; ruta solo local. | Mensajes deben permanecer genéricos. |
| RQ-10 | Composición reutiliza dependencias. | No cambia `ClassCorreo`, recovery ni otros ASMX. | Conexiones siguen resolviéndose por sesión legacy. |
| RQ-11 | Evidencia honesta. | Suites locales; E2E autorizada o bloqueo explícito. | No cerrar únicamente con mocks. |
| RQ-12 | Documentación y rollback comprobables. | CI valida diagramas/símbolos; rollback sin SQL. | Prueba estructural no acredita la fidelidad conductual completa. |
| RQ-13 | Inicio localizable sin datos sensibles. | Correlación/etapas cerradas; sink no bloqueante; prueba post-reciclado. | Permiso y retención de Event Log dependen del host. |
| RQ-14 | Errores seguros y operables durante el ciclo completo de 2FA. | Allowlist pública; códigos desconocidos colapsados; etapas cerradas en estado, verificación, reenvío, cancelación y compensaciones; fallos HMAC internos no consumen intentos. | El diagnóstico informa categoría y etapa, no texto libre ni secretos. |

## Resultado del refinamiento

- Estado aprobado para planificación e implementación.
- La desviación frente al modelo ideal de estados está resuelta por la restricción de compatibilidad sin DDL y queda trazada en D-01/RQ-01.
- La verificación autorizada del listener y la estabilidad post-reciclado quedaron aprobadas el 2026-10-11; el cierre incorpora además el endurecimiento de errores trazado por D-14/RQ-14.
