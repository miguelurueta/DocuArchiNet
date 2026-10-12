<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-96-doblle-factor-interfaz-e2e

## Fuente y alcance

- Ticket: `DOC-96` — DOBLLE-FACTOR-INTERFAZ-E2E.
- Cambio: `doc-96-doblle-factor-interfaz-e2e`.
- Fuente: `specs/doblle-factor-interfaz-e2e/jira-context.md` y exploración técnica Login 2FA.
- Perfil: ASP.NET Web Forms/.NET Framework, VB.NET, ASMX JSON, JavaScript/CSS y E2E Node.js/Playwright en `tools/e2e`.

DOC-96 incorpora la interfaz OTP al login existente y valida el recorrido real. Reutiliza el backend DOC-91 a DOC-95; no agrega tablas, columnas, SMTP, DTOs, endpoints de credenciales ni otro login.

## Contexto inspeccionado

- `gestor.aspx` contiene `UpdatePanel1`, `Panel_sesion_privado_login`, `DropDownListmodulos`, `TextBoxuser`, `TextBoxpasw` y `Buttonaceptar`.
- `gestor.aspx.vb:Button1_Click` llama `ClassGestorSesion.InicioAplicacionWebGestorDocumental(modulo As String, user As String, passs As String, nombre_empresa As String) As String`.
- `Defaul/ClassGestorSesion.vb` retorna `SECOND_FACTOR_REQUIRED` al crear una preautenticación activa; con 2FA apagado finaliza el login legacy y retorna `YES`.
- `webservice/WebServiceLoginSegundoFactor.asmx.vb` expone con Session `ObtenerEstado()`, `Verificar(codigo As String)`, `Reenviar()` y `Cancelar()`.
- `SegundoFactorDtos.vb` y `LoginSecondFactorPresentationMapper.vb` entregan estado público, destino enmascarado, segundos restantes, mensajes seguros y ruta local solo al completar.
- `LegacyLoginLocalRouteResolver.DefaultRoute` es `~/Defaul/WebFormInicioDocuarchiGestion.aspx`.
- `js/workflow/gestor.js` es compartido y no recibirá lógica 2FA.
- El helper E2E `authenticated-workflow-session.cjs` cierra la página tras el postback; DOC-96 necesita una variante compatible que preserve la página para observar la UI.
- El runner DOC-95 ya valida contrato, OTP, expiración, concurrencia y autorización; DOC-96 lo reutiliza sin duplicar arnés ni secretos.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | El postback vigente sigue siendo la única entrada de credenciales; `Button1_Click` intercepta solo `SECOND_FACTOR_REQUIRED`. | `gestor.aspx(.vb)`, `ClassGestorSesion.vb`. | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | El panel OTP vive en `gestor.aspx`, oculto por defecto, accesible y sin controles de identidad. | Markup actual y DTO público. | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | Toda lógica cliente 2FA reside en `js/login/segundo-factor.js`; no se agrega a `js/workflow/gestor.js`. | Límite del script legacy. | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | El cliente usa los cuatro métodos ASMX con mismo origen/Session; solo `Verificar` envía `{ codigo }`. | `WebServiceLoginSegundoFactor.asmx.vb`. | D-04 | RQ-03 | Origen: D-04, RQ-03 |
| D-05 | El estado visual deriva del servidor; una operación mutante en vuelo y contadores desde los segundos retornados. | DTOs y mapper. | D-05 | RQ-04 | Origen: D-05, RQ-04 |
| D-06 | `ObtenerEstado` restaura la preautenticación activa en carga/refresh; ausencia o término vuelve al login sin autenticar. | `ObtenerEstado`, Session gate. | D-06 | RQ-05 | Origen: D-06, RQ-05 |
| D-07 | Solo `SECOND_FACTOR_COMPLETED` navega y solo a `~/...` sin traversal, esquema, query o fragmento. | Mapper y route resolver. | D-07 | RQ-06 | Origen: D-07, RQ-06 |
| D-08 | Cancelación, expiración, bloqueo, límite y fallos terminales limpian UI y nunca degradan a login autenticado. | Códigos públicos del mapper. | D-08 | RQ-07 | Origen: D-08, RQ-07 |
| D-09 | DOM, URL, storage, consola y evidencia no contienen secretos ni identificadores internos. | DTO público y política E2E. | D-09 | RQ-08 | Origen: D-09, RQ-08 |
| D-10 | CSS aislado en `Styles/login/segundo-factor.css`; JS/CSS se registran una vez en el proyecto. | CSS actual y `.vbproj`. | D-10 | RQ-02 | Origen: D-10, RQ-02 |
| D-11 | Pruebas focales ejercitan estados/políticas; E2E reutiliza `tools/e2e` preservando la página tras postback. | Tests Node, helper de sesión y runner DOC-95. | D-11 | RQ-09 | Origen: D-11, RQ-09 |
| D-12 | E2E cubre OFF y ON y prepara matriz de cuatro; el control temporal excepcional exige autorización, escritura guardada de un solo flag, restauración y `SELECT` independiente. | `AGENTS.md`, runbook, política DOC-95 y evidencia real del ambiente. | D-12 | RQ-10 | Origen: D-12, RQ-10 |
| D-13 | El contrato DOC-95 no cambia salvo incompatibilidad demostrada; actualmente no se autoriza modificar backend. | Firmas ASMX/DTO inspeccionadas. | D-13 | RQ-11 | Origen: D-13, RQ-11 |

## Requisitos verificables

| ID | Resultado observable | Criterio | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | 2FA apagado e ingreso inválido conservan comportamiento. | `YES` navega como hoy y solo `SECOND_FACTOR_REQUIRED` abre OTP. | Evita regresión transversal. |
| RQ-02 | La preautenticación activa muestra panel accesible de seis dígitos y deshabilita credenciales. | Label, foco, inputmode, autocomplete, región viva y botones observables. | Evita doble interacción y barreras. |
| RQ-03 | Operaciones usan solo contrato ASMX aprobado. | Estado/reenvío/cancelación `{}`; verificar solo `{ codigo }`. | Evita suplantación cliente. |
| RQ-04 | Doble click no duplica y tiempos reflejan servidor. | Request único; al llegar a cero se reconsulta estado. | Evita carreras y reloj autoritativo cliente. |
| RQ-05 | Refresh recupera la preautenticación activa. | El estado activo abre panel; `NO_DISPONIBLE` deja login. | Session es fuente de verdad. |
| RQ-06 | Navegación solo tras completar y a ruta local validada. | Ruta inválida o respuesta incompleta no navega. | Impide open redirect. |
| RQ-07 | Terminales vuelven a credenciales sin cookie. | Cancelar, expirar, bloquear o fallar limpian campos. | Fail-closed. |
| RQ-08 | No hay secretos en superficie cliente/evidencia. | Pruebas inspeccionan DOM, storage, URL y logs. | Evita fuga sensible. |
| RQ-09 | Pruebas focales validan estados, contrato, accesibilidad y assets. | `node --test` falla ante divergencias. | Detecta regresión previa a E2E. |
| RQ-10 | E2E autorizada prueba OFF/ON y escenarios activos. | Evidencia real saneada; si OFF requiere control temporal, restaura ON y lo verifica independientemente. | No sustituye integración por mocks ni deja configuración alterada. |
| RQ-11 | DOC-95 permanece compatible y sin cambios de datos/configuración. | Diff sin tablas/columnas/ASMX/DTOs. | Aísla presentación. |

## Reglas de trazabilidad

1. Cada D-01 a D-13 aparece en diseño, especificación y tareas.
2. Cada tarea declara área, complejidad, origen y verificación.
3. Cada E2E real requiere autorización nueva y protege secretos.
4. Esta aprobación habilita implementación; no declara código ni E2E completados.

## Resultado

Se aprueba el corte de presentación, pruebas y documentación. No quedan decisiones funcionales abiertas; backend, persistencia y configuración permanecen inalterados.
