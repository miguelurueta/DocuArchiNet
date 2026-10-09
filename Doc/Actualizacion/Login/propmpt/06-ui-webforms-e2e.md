# 06 — Interfaz WebForms y E2E del recorrido 2FA

## ROL ESPERADO

Actúa como arquitecto full stack senior y especialista en accesibilidad/WebForms.

## OBJETIVO

Integrar, dentro del OpenSpec de esta Jira, la captura OTP en `gestor.aspx` sobre el ASMX ya probado y validar el recorrido de usuario en la misma unidad de entrega.

## CONTEXTO

Lee la exploración, el markup/code-behind actual y el contrato ASMX implementado. Inspecciona los estilos/scripts existentes antes de agregar recursos.

## PRECONDICIONES DE RUTAS

El `design.md` debe inventariar los archivos exactos y declarar cada control, handler y recurso afectado. Esta tarea se limita a:

- `gestor.aspx` para markup del panel OTP y referencias de recursos.
- `gestor.aspx.vb` y `gestor.aspx.designer.vb` para el postback/controles estrictamente necesarios.
- `js/login/segundo-factor.js` para el cliente del ASMX; no agregar lógica 2FA al genérico `js/workflow/gestor.js`.
- `Styles/login/segundo-factor.css` solo si los estilos existentes no cubren el panel; no insertar un framework CSS nuevo.
- `DTOs/Login/SegundoFactor/` o `webservice/Login/SegundoFactor/` únicamente ante una corrección contractual demostrada del ASMX.
- `tests/`, `tools/e2e/` y `GestionDocumental-Docuarchi.net.vbproj` para verificación y registro de nuevos recursos.

No crear `Login2FA.aspx`, otra pantalla, otro master page, otro ASMX, otro cliente HTTP ni código SQL/SMTP en la página.

## REQUISITOS POSITIVOS

1. Conservar el postback actual para empresa, módulo, usuario y contraseña. No crear un login JavaScript paralelo.
2. Cuando el servidor retorne preautenticación pendiente, ocultar/deshabilitar el formulario de credenciales y mostrar un panel accesible de seis dígitos con destino enmascarado, expiración, validar, reenviar y cancelar.
3. Consumir el ASMX con mismo origen y Session/cookies vigentes. El cliente envía únicamente el OTP a `Verificar`; `ObtenerEstado`, `Reenviar` y `Cancelar` no reciben identidad ni challenge.
4. Evitar doble submit, bloquear botones durante requests, mostrar cooldown real del servidor y restaurar estado tras refresh mediante `ObtenerEstado`.
5. Navegar solo a la ruta local retornada tras `COMPLETED`. Ante cancelación/expiración/bloqueo/fallo, limpiar UI y regresar al login sin cookie.
6. No exponer en DOM, URL, storage o logs OTP, challenge, correo completo, IDs, contraseña o detalles internos.
7. Mantener la interfaz y navegación actuales cuando 2FA esté `0`/`NULL`.

## RESTRICCIONES CRITICAS

- No crear otro login, endpoint de credenciales, SPA, almacenamiento local de identidad ni bypass ante fallos.
- No modificar recuperación de contraseña, `ClassCorreo`, permisos o navegación legacy con 2FA apagado.
- No aceptar identidad, challenge, correo o return URL desde JavaScript.
- No cambiar feature flags/configuración ni datos reales para hacer pasar pruebas.

## CASOS FUNCIONALES OBLIGATORIOS

- Credenciales inválidas no muestran OTP.
- Módulo sin 2FA mantiene el login y destino actuales.
- Módulo con 2FA muestra correo enmascarado y no navega antes de verificar.
- OTP inválido conserva el panel y mensaje genérico; el quinto intento lo cierra/bloquea.
- OTP expirado requiere reiniciar login.
- Reenvío temprano no envía; reenvío permitido invalida el código anterior; después de dos reenvíos se rechaza.
- Refresh recupera el pendiente; back/cancel no autentican.
- Doble click o respuestas concurrentes no finalizan dos veces.
- Fallo de correo/configuración/finalización nunca degrada al login sin 2FA.

## PRUEBAS OBLIGATORIAS Y E2E

Código, pruebas unitarias, E2E y evidencia forman una única unidad de entrega y cierre. Amplía y reutiliza exclusivamente `tools/e2e`, su autenticación, configuración, validadores, utilidades y evidencias; prohíbe otro arnés, login, proyecto Playwright o `.env`. Antes de cualquier autenticación real lee `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`. Exige autorización explícita vigente para ambiente, cuentas, buzón y datos/tareas descartables; no reutilices autorizaciones anteriores. Usa secretos efímeros y nunca imprimas ni guardes credenciales, cookies, OTP, correo, tokens o conexiones.

Automatiza, como mínimo, un módulo con 2FA apagado y uno activado; diseña la matriz para los cuatro módulos. Las consultas de control serán exclusivamente `SELECT` y la evidencia será sanitizada. Verifica autorización/control de acceso, lectura sin mutación, escrituras autorizadas del challenge, navegación/permisos después del OTP, reenvío, concurrencia y regresión relacionada. Respetar feature flags, gates, usuarios y grupos sin habilitarlos arbitrariamente. No cerrar sin validación autorizada: si el ambiente no permite activar/configurar, reporta bloqueo explícito y prohíbe mocks, simulaciones o evidencia ficticia para sustituir el E2E.

El E2E no puede depender de sleeps largos, credenciales embebidas ni un `.env` nuevo. Captura evidencias sanitizadas y falla con mensajes que identifiquen etapa, no secretos.

## CRITERIOS DE ACEPTACION

Ejecuta pruebas de cliente/contrato, MSBuild, regresiones previas y E2E autorizado. La tarea solo está completa cuando código, pruebas y evidencia coherente están juntos; si falta autorización real, el estado es implementado pero validación E2E bloqueada, no “pasó”.

Aceptación:

- UI accesible y resiliente consume únicamente el contrato ASMX aprobado.
- No hay bypass, open redirect, doble finalización ni secretos del lado cliente.
- El login desactivado conserva el comportamiento existente.
- El OpenSpec y `Doc/Actualizacion/Login/Implementacion/<JIRA>/` incluyen trazabilidad, comandos, códigos de salida, evidencia y bloqueos.

## DOCUMENTACION TECNICA

Actualiza la documentación existente de UI, contrato, accesibilidad y flujo; registra matriz/evidencia saneada en `Doc/Actualizacion/Login/Implementacion/<JIRA>/`.

## ENTREGABLE FINAL

Entrega archivos, pruebas unitarias con ruta/caso, E2E y resultados reales, build, evidencia, limitaciones y bloqueos. No declares cierre si el código incorporado carece de la validación autorizada exigida; informa el estado exacto.
