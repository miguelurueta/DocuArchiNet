# 05 — Orquestación del challenge y servicio ASMX

## ROL ESPERADO

Actúa como arquitecto de aplicación y desarrollador senior WebForms/ASMX.

## OBJETIVO

Integrar, en el OpenSpec independiente de esta Jira, contratos, repositorio, SMTP, preautenticación y finalizador mediante un servicio de aplicación y un ASMX exclusivo de Login 2FA. Esta tarea entrega una frontera HTTP comprobable antes de construir la UI.

## CONTEXTO

Lee completa la exploración y verifica los artefactos implementados en las tareas 01–04 contra el código actual; no asumas que sus nombres o firmas son correctos sin compilar.

## PRECONDICIONES DE RUTAS

Antes de editar, el `design.md` debe inventariar endpoints, DTOs, servicios, interfaces implementadas y archivos exactos. Usa:

- `Services/Login/SegundoFactor/` para inicio, verificación, reenvío, cancelación y coordinación de finalización.
- `DTOs/Login/SegundoFactor/` para requests/responses ASMX tipados y sanitizados.
- `webservice/Login/SegundoFactor/` para gate de Session, composition factory y mapeadores de Presentation.
- `webservice/WebServiceLoginSegundoFactor.asmx` y `webservice/WebServiceLoginSegundoFactor.asmx.vb` como única frontera HTTP nueva.
- `Modelo/Login/SegundoFactor/`, `Infrastructure/Repositories/Login/SegundoFactor/` e `Infrastructure/Login/SegundoFactor/` solo para completar contratos/implementaciones previamente definidos, sin duplicarlos.
- `tests/`, `tools/e2e/` y `GestionDocumental-Docuarchi.net.vbproj` para pruebas, integración y registro explícito.

El ASMX no puede contener SQL, SMTP, acceso directo a tablas ni reglas de challenge. Infrastructure no puede leer Session. No agregar métodos 2FA a `WebServiceWorkflowModern`, `WebServiceGestorDocumental` u otro ASMX existente.

## REQUISITOS POSITIVOS

1. Implementar el orquestador de inicio: credenciales ya validadas + configuración activa -> identidad canónica -> contexto pendiente -> `CREATED` -> envío SMTP -> `SENT`; ante fallo, `DELIVERY_FAILED`, contexto limpiado y ninguna autenticación.
2. Implementar verificación transaccional: obtener contexto pendiente de Session, adquirir `SENT -> FINALIZING`, comparar HMAC, incrementar/bloquear intento o ejecutar una sola vez el finalizador legacy.
3. Tras ejecutar correctamente el finalizador legacy, marcar `COMPLETED`; solo después usar `FormsAuthentication.SetAuthCookie`, limpiar contexto y retornar una ruta local calculada por servidor. Ante error previo a la cookie, marcar `FINALIZATION_FAILED`, limpiar y no autenticar. Documentar expresamente los efectos legacy que no puedan compensarse si falla el cierre persistente.
4. Implementar reenvío: validar 60 segundos, máximo dos reenvíos, revocar anterior, crear/enviar uno nuevo. No reutilizar OTP.
5. Implementar cancelación: revocar pendiente elegible, limpiar Session y no autenticar.
6. Crear exactamente `webservice/WebServiceLoginSegundoFactor.asmx` y `webservice/WebServiceLoginSegundoFactor.asmx.vb`, siguiendo el patrón real de `WebServiceWorkflowModern.asmx.vb`, con `ScriptService` y `WebMethod(EnableSession:=True)`.
7. Exponer exactamente operaciones sin identidad aportada por cliente:

```text
ObtenerEstado()             -> estado público, destino enmascarado y tiempos
Verificar(codigo As String) -> resultado y ruta local únicamente al completar
Reenviar()                  -> resultado y próximo instante permitido
Cancelar()                  -> resultado genérico
```

8. Registrar `.asmx`/`.vb` en el proyecto y mantener composición/SQL fuera de la frontera HTTP.

## RESTRICCIONES CRITICAS

- Sin contexto pendiente, sesión cambiada, challenge revocado/expirado/bloqueado o llave desconocida: error público genérico, ninguna cookie.
- Código vacío o formato distinto de seis dígitos: rechazo antes del repositorio sin revelar información.
- Dos verificaciones correctas simultáneas: una sola entra en `FINALIZING`; la otra recibe estado no elegible.
- No aceptar ni reflejar `returnUrl` externo. La ruta se obtiene de una lista/localización server-side ya usada por los módulos.
- No retornar excepción, SQL, challenge, IDs, email completo, intentos internos, hash o detalles SMTP.
- No convertir el ASMX en endpoint alterno de credenciales: usuario/contraseña siguen llegando solo al postback de `gestor.aspx`.
- No modificar recuperación, `ClassCorreo`, configuración productiva, datos reales ni contratos ajenos al Login 2FA.

## PRUEBAS OBLIGATORIAS

Prueba el servicio de aplicación con dobles y el ASMX localmente: sesión ausente, estado pendiente, OTP inválido/correcto, expiración, quinto intento, resend temprano/límite, cancelación, fallo SMTP/finalizador y doble submit. Verifica atributos `WebMethod`, Session habilitada, DTO JSON estable, cookie solo tras completar y ruta local.

La prueba E2E/integrada de la frontera es parte integral de este mismo cambio y de su cierre, no una entrega separada. Si existe autorización explícita vigente para ambiente, cuentas, buzón y datos descartables, ejecútala a través del ASMX y SMTP/base reales antes de UI. Lee primero `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`; reutiliza exclusivamente `tools/e2e`, su autenticación, configuración, validadores y evidencias. No crear login, arnés, Playwright, configuración ni `.env` paralelos.

No imprimas ni persistas credenciales, cookies, tokens, OTP o conexiones; usa secretos efímeros, consultas de control exclusivamente `SELECT` y evidencia sanitizada. Cubre autorización/control de acceso, lectura sin mutación, escrituras autorizadas del challenge, doble submit/concurrencia y regresión del login apagado. Respetar feature flags, gates, usuarios y grupos sin habilitarlos arbitrariamente. No cerrar sin validación autorizada: registra bloqueo explícito y prohíbe mocks, simulaciones o evidencia ficticia para sustituirla; las pruebas locales siguen siendo obligatorias.

Ejecuta MSBuild y todas las regresiones previas.

## CRITERIOS DE ACEPTACION

- El ciclo de estados y compensaciones coincide con la exploración.
- No hay identidad controlada por cliente ni SQL en ASMX.
- Una única solicitud puede finalizar y emitir cookie.
- Existe evidencia directa del contrato HTTP antes de la UI.
- El OpenSpec traza `D-XX`/`RQ-XX`, código y pruebas.

## DOCUMENTACION TECNICA

Actualiza contratos HTTP, estados, errores, secuencia y documentación existente del login. Registra evidencia en `Doc/Actualizacion/Login/Implementacion/<JIRA>/`.

## ENTREGABLE FINAL

Entrega archivos, firmas ASMX/DTO, pruebas unitarias con ruta/caso, integración/E2E real o bloqueo explícito, build, comandos, códigos de salida y evidencia saneada.
