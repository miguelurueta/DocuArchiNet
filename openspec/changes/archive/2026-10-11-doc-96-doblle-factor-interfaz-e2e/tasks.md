<!-- opsxj:refinement-traceability version=1 artifact=tasks decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09,D-10,D-11,D-12,D-13 -->
## 1. Integración Web Forms

- [x] 1.1 [S] Agregar panel OTP HTML accesible y referencias locales sin controles de identidad. Área/archivos: `gestor.aspx`. Origen: D-02, RQ-02. Verificación: prueba estructural confirma panel oculto, label, input de seis dígitos, regiones vivas y tres botones.
- [x] 1.2 [S] Interceptar solo `SECOND_FACTOR_REQUIRED`, limpiar contraseña y registrar bootstrap estático tras postback. Área/archivos: `gestor.aspx.vb`. Origen: D-01, RQ-01. Verificación: prueba focal confirma rama exacta y preserva `YES` y errores.
- [x] 1.3 [S] Registrar JS y CSS una vez como contenido Web Application. Área/archivos: `GestionDocumental-Docuarchi.net.vbproj`. Origen: D-10, RQ-02. Verificación: exactamente una entrada `Content` por recurso y sin cambio de designer innecesario.

## 2. Cliente OTP aislado

- [x] 2.1 [M] Implementar adaptador ASMX estricto, mismo origen, sobre `{d}` y payload mínimo. Área/archivos: `js/login/segundo-factor.js`. Origen: D-04, RQ-03. Verificación: pruebas Node inspeccionan URL, método, cookies, cuerpo y respuesta inválida.
- [x] 2.2 [M] Implementar estados idempotentes, exclusión mutua y contadores desde segundos servidor. Área/archivos: `js/login/segundo-factor.js`. Origen: D-05, RQ-04. Verificación: reloj/transporte inyectados prueban doble click, cooldown, expiración y reconsulta.
- [x] 2.3 [M] Restaurar por `ObtenerEstado`, alternar vistas, foco y anuncios en carga/postback parcial. Área/archivos: `js/login/segundo-factor.js`, `gestor.aspx`. Origen: D-06, RQ-05. Verificación: cubre `PENDIENTE`, `NO_DISPONIBLE`, refresh e inicio repetido sin handlers duplicados.
- [x] 2.4 [M] Clasificar recuperables/terminales y limpiar OTP, contraseña, intervalos y estado. Área/archivos: `js/login/segundo-factor.js`. Origen: D-08, RQ-07. Verificación: todos los códigos públicos del mapper quedan cubiertos y ningún terminal navega.
- [x] 2.5 [S] Validar completado y resolver únicamente `RutaLocal` `~/` segura. Área/archivos: `js/login/segundo-factor.js`. Origen: D-07, RQ-06. Verificación: acepta ruta legacy y rechaza host, esquema, traversal, barra inversa, query y fragmento.
- [x] 2.6 [S] Aplicar estilos aislados, responsive y de foco/disabled. Área/archivos: `Styles/login/segundo-factor.css`. Origen: D-10, RQ-02. Verificación: prefijo propio, `[hidden]`, foco visible y sin dependencia externa.

## 3. Seguridad y regresión automatizada

- [x] 3.1 [M] Crear pruebas del cliente con dobles de transporte, reloj, navegación y vista, sin nueva dependencia DOM. Área/archivos: `tests/login-second-factor-ui.test.cjs`. Origen: D-11, RQ-09. Verificación: `node --test tests/login-second-factor-ui.test.cjs` termina en 0.
- [x] 3.2 [S] Crear política Web Forms para límites, accesibilidad, rama exacta, secretos/storage/logs y backend congelado. Área/archivos: `tests/doc96-login-second-factor-ui-policy.test.cjs`. Origen: D-09, RQ-08. Verificación: `node --test tests/doc96-login-second-factor-ui-policy.test.cjs` termina en 0.
- [x] 3.3 [S] Ejecutar regresiones DOC-91 a DOC-95 y compilación sin alterar persistencia/configuración. Área/archivos: `tests/login-second-factor-*.test.cjs`, política DOC-95 y solución. Origen: D-13, RQ-11. Verificación: comandos/códigos registrados y fallos diagnosticados sin falso éxito.

## 4. E2E de interfaz

- [x] 4.1 [M] Ampliar helper para conservar opcionalmente página/postback manteniendo compatible `createAuthenticatedWorkflowSession`. Área/archivos: `tools/e2e/tests/support/authenticated-workflow-session.cjs` y pruebas. Origen: D-11, RQ-09. Verificación: cubre API legacy y variante UI sin fuga de credenciales.
- [x] 4.2 [M] Incorporar runner DOC-96 y validador reutilizando perfil, autorización y sanitización existentes. Área/archivos: `tools/e2e/scripts/`, `tools/e2e/tests/`, `tools/e2e/package.json`. Origen: D-12, RQ-10. Verificación: política/configuración falla por ausencia y nunca imprime secretos.
- [x] 4.3 [M] Automatizar módulo OFF y recorrido ON con panel, enmascarado, refresh, OTP y navegación/permisos. Área/archivos: runner DOC-96 en `tools/e2e`. Origen: D-01, RQ-01. Verificación: corridas autorizadas informan resultados ON saneados y el control OFF confirma login legacy, cookie, restauración ON y comprobación independiente por `SELECT`.
- [x] 4.4 [M] Añadir inválido/bloqueo, reenvío/cooldown/límite, expiración, cancelación y doble activación sin sleeps largos. Área/archivos: runner/matriz DOC-96. Origen: D-08, RQ-07. Verificación: corridas reales autorizadas `invalid`, `blocked`, `resend`, `expiry`, `cancel` y `double-submit` pasan sobre `GESTOR`, con esperas por estado y sin cookie antes de completar.

## 5. Documentación y cierre

- [x] 5.1 [M] Documentar arquitectura, contrato, estados, accesibilidad, seguridad, matriz de cuatro módulos y rollback. Área/archivos: `Doc/Actualizacion/Login/Implementacion/DOC-96/`. Origen: D-03, RQ-03. Verificación: enlaza símbolos reales y distingue implementado, validado y bloqueado.
- [x] 5.2 [S] Registrar comandos, códigos y evidencia E2E saneada o bloqueo exacto. Área/archivos: `Doc/Actualizacion/Login/Implementacion/DOC-96/`. Origen: D-09, RQ-08. Verificación: ausencia de OTP, correos, usuarios, cookies, credenciales y conexiones.
- [x] 5.3 [S] Ejecutar OpenSpec estricto y auditoría OPSXJ antes de implementar/cerrar. Área/archivos: `openspec/changes/doc-96-doblle-factor-interfaz-e2e/`. Origen: D-13, RQ-11. Verificación: `openspec validate doc-96-doblle-factor-interfaz-e2e --strict` y `opsxj:refine -- DOC-96 --sync` terminan en 0.
