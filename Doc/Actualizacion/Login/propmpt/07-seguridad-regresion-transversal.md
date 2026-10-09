# 07 — Seguridad y regresión transversal

## ROL ESPERADO

Actúa como arquitecto de seguridad y líder de calidad.

## OBJETIVO

Auditar, en un OpenSpec propio de esta Jira, y corregir exclusivamente defectos comprobados del incremento Login 2FA, completar la matriz de los cuatro módulos e integrar verificaciones estables en CI.

## CONTEXTO

Lee la exploración, los OpenSpec implementados y la documentación existente. Contrasta cada afirmación con código, configuración versionada y pruebas reales.

## PRECONDICIONES DE RUTAS

Primero inventaría en `design.md` los archivos exactos realmente entregados por las tareas 01–06 y verifica que respeten la matriz de la exploración. Los cambios se limitan a:

- correcciones en las rutas canónicas ya propietarias del símbolo: `Modelo/Login/SegundoFactor/`, `DTOs/Login/SegundoFactor/`, `Services/Login/SegundoFactor/`, `Infrastructure/Repositories/Login/SegundoFactor/`, `Infrastructure/Login/SegundoFactor/` y `webservice/Login/SegundoFactor/`;
- frontera exacta `webservice/WebServiceLoginSegundoFactor.asmx(.vb)` y página/recursos `gestor.aspx*`, `js/login/`, `Styles/login/` solo cuando una prueba demuestre el defecto;
- `tests/`, `tools/validation/`, `tools/e2e/`, `.github/workflows/opsxj-validation.yml` y `GestionDocumental-Docuarchi.net.vbproj` para automatización y para registrar explícitamente cada archivo nuevo.

No mover símbolos entre capas como parte de limpieza, no crear rutas alternativas y no tocar módulos fuera del Login 2FA salvo una regresión demostrada y trazada.

## REQUISITOS POSITIVOS Y FLUJO FUNCIONAL

Traza desde `gestor.aspx` hasta cookie final, incluyendo configuración central, adaptadores por módulo, Session pendiente, challenge, SMTP, ASMX y UI. Verifica contra la exploración:

- ninguna identidad, permiso, auditoría o cookie antes de OTP;
- equivalencia legacy con `RequiereSegundoFactor = 0/NULL`;
- fail-closed para configuración/proveedor/expiración/SMTP inválidos;
- OTP/HMAC/llaves sin exposición y comparación constante;
- estados y transiciones atómicas, máximo 5 intentos, cooldown y 2 reenvíos;
- aislamiento empresa/módulo/usuario/sesión y resistencia a replay/concurrencia;
- rutas locales sin open redirect y ASMX sin identidad controlada por cliente;
- `AuthPayloadJson` nulo y ausencia de contraseñas/conexiones en tabla, Session, DTOs, DOM, logs y evidencia;
- `ClassCorreo` y recuperación de contraseña intactos.

Documenta y verifica la secuencia completa: postback -> validación legacy -> configuración -> preautenticación -> persistencia -> SMTP -> ASMX/UI -> finalizador -> cookie/ruta, con sus ramas de error.

## Matriz obligatoria

Para DocuArchi, Gestor, Radicación y Workflow, cubre credenciales inválidas, 2FA apagado, activo con correo, correo ausente, proveedor inválido, fallo SMTP, OTP correcto/incorrecto/expirado/consumido, quinto intento, reenvío, cancelación, refresh/back, doble submit, dos sesiones y mismo login en empresas distintas. Respeta que solo Gestor/Workflow aplican actualmente la regla legacy de estado.

## PRUEBAS OBLIGATORIAS

- Pruebas .NET mediante MSBuild y harnesses ya usados por el repositorio.
- CJS/Node para contratos y comprobaciones estructurales existentes.
- Roslyn bajo `tools/validation` si se requiere pertenencia/firma VB real.
- `tools/e2e` como único Playwright.
- Integración en `.github/workflows/opsxj-validation.yml` sin pipeline o runner paralelo.

Código, correcciones, pruebas y E2E son una única unidad del cambio. Reutiliza exclusivamente `tools/e2e`; no crees login, configuración, arnés, Playwright o `.env` paralelos. Antes de E2E/autenticación/base real, exige autorización actual para ambiente, cuentas y datos descartables y lee `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`. Usa secretos efímeros, consultas de control solo `SELECT` y evidencia sanitizada; no imprimas credenciales, cookies, OTP, tokens o conexiones. Protege configuración/flags y no los alteres para forzar casos. Cubre acceso, lecturas, escrituras autorizadas del challenge, concurrencia y regresión. Una prueba bloqueada se informa explícitamente y no se cierra como aprobada.

## RESTRICCIONES CRITICAS Y REGLAS DE ANTIRREGRESION

Corrige en esta misma Jira fallas demostradas de la implementación 2FA y repite pruebas afectadas. Debe preservar el comportamiento legacy con 2FA apagado y no romper firmas públicas ni recorridos de correo/recuperación. Si la corrección exige cambiar decisiones, esquema incompatible o comportamiento legacy, detente y eleva el hallazgo: no lo ocultes en una “mejora”. No rediseñes funcionalidades aprobadas, agregues alcance, hagas refactors cosméticos masivos ni modifiques recuperación/ClassCorreo.

## CRITERIOS DE ACEPTACION

- Todas las verificaciones automatizables estables corren en CI y fallan de forma diagnóstica.
- La matriz tiene estado real por caso: pasado, fallido o bloqueado, nunca cobertura declarada sin evidencia.
- No existe regresión conocida en los cuatro módulos con 2FA apagado.
- Los riesgos residuales, incluida la falta de transacción distribuida en finalización legacy, permanecen explícitos.
- OpenSpec, `D-XX`/`RQ-XX`, cambios y resultados están trazados.

## DOCUMENTACION TECNICA

Ubica y actualiza documentación existente afectada; registra matriz, riesgos y evidencia saneada bajo `Doc/Actualizacion/Login/Implementacion/<JIRA>/`. Si falta una referencia, documenta la ruta faltante.

## ENTREGABLE FINAL

Entrega hallazgos, correcciones, pruebas unitarias con ruta/caso, CI, E2E real o bloqueo, comandos, códigos de salida y estado por caso.
