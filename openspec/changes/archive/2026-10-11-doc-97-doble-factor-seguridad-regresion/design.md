<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07 -->
## Context

DOC-97 audita transversalmente el Login 2FA implementado por DOC-91 a DOC-96. La línea base local está verde, pero las pruebas están separadas por entrega y no existe una matriz única que demuestre todos los casos por módulo. El CI tampoco observa archivos ni ejecuta las pruebas estables de DOC-96.

## Inventario implementado inspeccionado

| Capa | Archivos reales |
| --- | --- |
| Modelo/puertos | `Modelo/Login/SegundoFactor/SegundoFactorModels.vb`, `SegundoFactorSmtpModels.vb`, `SegundoFactorInterfaces.vb` |
| DTO público | `DTOs/Login/SegundoFactor/SegundoFactorDtos.vb` |
| Aplicación | `Services/Login/SegundoFactor/SecondFactorPreAuthenticationService.vb`, `SecondFactorAuthenticationService.vb`, `SecondFactorRecipientResolver.vb`, `NullSecondFactorDiagnosticSink.vb` |
| Repositorios | `Infrastructure/Repositories/Login/SegundoFactor/OdbcSecondFactorLoginModuleRepository.vb`, base y cuatro `MySql*SecondFactorPrincipalRepository.vb`, `SecondFactorPrincipalRepositoryResolver.vb`, `MySqlSecondFactorChallengeRepository.vb`, `MySqlSecondFactorSmtpConfigurationRepository.vb` |
| Infraestructura | `Infrastructure/Login/SegundoFactor/Security/*.vb`, `Smtp/*.vb`, `Diagnostics/*.vb` |
| Web/Session | `webservice/Login/SegundoFactor/SessionPendingSecondFactorContextStore.vb`, `LoginSecondFactorSessionGate.vb`, `FormsAuthenticationTicketIssuer.vb`, `LegacyLoginLocalRouteResolver.vb`, `LoginSecondFactorPresentationMapper.vb`, `LoginSecondFactorCompositionFactory.vb` |
| Endpoint/UI | `webservice/WebServiceLoginSegundoFactor.asmx(.vb)`, `gestor.aspx`, `gestor.aspx.vb`, `js/login/segundo-factor.js`, `Styles/login/segundo-factor.css` |
| Registro | `GestionDocumental-Docuarchi.net.vbproj`, `Web.config` |
| Pruebas | `tests/LoginSecondFactor*Tests.cs`, `tests/login-second-factor-*.test.cjs`, pruebas documentales y políticas/runners DOC-95/DOC-96 en `tools/e2e/` |

No se detectó modificación pendiente en `ClassCorreo` ni se autoriza tocar recuperación de contraseña.

## Flujo real bajo auditoría

`gestor.Button1_Click` valida credenciales legacy y delega en preautenticación. `SecondFactorPreAuthenticationService.Execute` resuelve empresa/módulo y principal. Con flag `0/NULL`, invoca el finalizador legacy; con flag `1`, `SecondFactorAuthenticationService.Start` crea, protege y persiste el challenge, resuelve destinatario y envía por SMTP dedicado. Session conserva el contexto pendiente. La UI consume `ObtenerEstado`, `Verificar(codigo)`, `Reenviar` y `Cancelar`. `Verify` adquiere el challenge antes del finalizador y emite cookie/ruta local solo después de completar. Los errores se mapean a catálogo público.

## Goals / Non-Goals

**Goals**

- Consolidar garantías dispersas en una matriz reproducible de cuatro módulos.
- Probar ausencia de efectos autenticados antes de OTP y equivalencia OFF/NULL.
- Probar límites, aislamiento, replay/concurrencia, confidencialidad y rutas locales.
- Integrar pruebas estables al único workflow de CI.
- Corregir solo defectos observados por una prueba fallida.

**Non-Goals**

- Crear esquema, endpoints, DTOs, proveedores, correo o UI nuevos.
- Cambiar firmas públicas o mover símbolos.
- Alterar flags/datos reales para fabricar cobertura.
- Modificar `ClassCorreo`, recuperación de contraseña o módulos ajenos.

## Decisions

### D-01 — Auditar el flujo existente

La matriz se ancla en símbolos reales y puntos de composición. Una falla identifica módulo, escenario, símbolo y expectativa.

### D-02 — Matriz determinista parametrizada

El harness reutiliza dobles y parametriza los cuatro módulos. Incluye credenciales inválidas, OFF/NULL, activo, correo ausente, proveedor/configuración inválidos, SMTP fallido, OTP correcto/incorrecto/expirado/consumido, quinto intento, reenvío/cooldown/límite, cancelación, refresh, doble submit, dos sesiones y mismo login en empresas distintas. Solo Gestor/Workflow esperan regla de estado.

### D-03 — Prueba en capas

- Harness .NET: decisiones y efectos.
- CJS: límites, proyecto, ASMX, JavaScript, secretos y registro.
- Roslyn: pertenencia/firmas VB cuando texto no baste.
- E2E: integración real autorizada; no sustituye la matriz determinista.

### D-04 — Corrección condicionada por evidencia

Solo una prueba roja habilita cambio en el propietario mínimo. Se conserva contrato público y se repiten prueba causal y suite 2FA. DDL o cambio legacy se eleva como bloqueo.

### D-05 — CI único

`.github/workflows/opsxj-validation.yml` añadirá paths DOC-96/DOC-97 y pasos de política DOC-96 y matriz DOC-97. No ejecutará E2E interactiva, SMTP ni base real.

### D-06 — E2E inmutable y gobernada

DOC-97 usa `tools/e2e` y el runbook, exige autorización específica y controles `SELECT`. El runner temporal que cambia GESTOR no sirve como evidencia DOC-97. Sin precondición real, el caso queda bloqueado.

### D-07 — Evidencia honesta

`Doc/Actualizacion/Login/Implementacion/DOC-97/` contendrá arquitectura, matriz y validación, distinguiendo prueba local, E2E y bloqueo. Mantendrá el riesgo de que finalización legacy y challenge no comparten transacción distribuida.

## Risks / Trade-offs

- La matriz E2E puede bloquearse por falta de cuentas/configuración; la cobertura determinista no se presenta como sustituto.
- Una prueba estructural no demuestra por sí sola comportamiento.
- Un fallo parcial del finalizador tras adquirir challenge no es compensable transaccionalmente.
- La búsqueda de secretos se combina con dobles y revisión de DTO/DOM; no garantiza ausencia absoluta en runtime.

## Migration and rollback

No hay migración. Pruebas/CI son aditivos. Una eventual corrección usa el flag existente para rollback operativo y reversión del código, nunca SQL nuevo. Sin defecto funcional demostrado, producción no cambia.

## Validation baseline

El 2026-10-11 se ejecutaron con código 0: 35 pruebas de contratos/comportamiento, 7 políticas DOC-95 y 21 políticas DOC-96. No equivalen a cobertura DOC-97 completa.
