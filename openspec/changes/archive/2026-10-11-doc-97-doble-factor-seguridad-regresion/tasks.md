<!-- opsxj:refinement-traceability version=1 artifact=tasks decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07 -->
## 1. Auditoría reproducible

- [x] 1.1 [M] Crear el manifiesto DOC-97 del flujo, símbolos y pruebas heredadas DOC-91..96. Área/archivos: `tests/`, `Doc/Actualizacion/Login/Implementacion/DOC-97/`. Origen: D-01, RQ-01. Verificación: falla indicando ruta/símbolo cuando una referencia no existe.
- [x] 1.2 [M] Implementar matriz parametrizada para los cuatro módulos, incluidos OFF/NULL, activo, correo y regla legacy de estado. Área/archivos: `tests/LoginSecondFactor*Tests.cs`, `tests/login-second-factor-*.test.cjs`, fixtures Login. Origen: D-02, RQ-02. Verificación: harness informa cuatro módulos/casos con código 0.
- [x] 1.3 [M] Extender harness con OTP correcto/incorrecto/expirado/consumido, quinto intento, cooldown, dos reenvíos, cancelación, replay y doble submit. Área/archivos: `tests/LoginSecondFactorAuthenticationBehaviorTests.cs`, wrapper CJS. Origen: D-03, RQ-03. Verificación: terminalidad y exactamente un finalizador.
- [x] 1.4 [M] Probar aislamiento entre sesiones, empresas, módulos y usuarios con login coincidente. Área/archivos: harness y fixtures `tests/`. Origen: D-02, RQ-03. Verificación: ningún contexto acepta challenge ajeno.

## 2. Seguridad y antirregresión

- [x] 2.1 [M] Consolidar verificaciones de secretos, DTO/ASMX mínimo, `AuthPayloadJson=NULL`, ruta local y autoridad de identidad. Área/archivos: `tests/login-second-factor-*.test.cjs`, `tools/validation/` si requiere AST. Origen: D-03, RQ-04. Verificación: prueba negativa por superficie con diagnóstico.
- [x] 2.2 [M] Verificar orden de efectos, comparación HMAC, fail-closed y ausencia de cookie/auditoría/permisos antes de OTP. Área/archivos: `tests/LoginSecondFactor*Tests.cs`, wrappers CJS. Origen: D-03, RQ-05. Verificación: dobles registran orden y cero efectos tempranos.
- [x] 2.3 [S] Añadir guardas para `ClassCorreo`, recuperación, firmas públicas y registro único del proyecto. Área/archivos: `tests/login-second-factor-*.test.cjs`, `GestionDocumental-Docuarchi.net.vbproj`. Origen: D-04, RQ-06. Verificación: contrato falla ante cambios no autorizados.
- [x] 2.4 [M] Corregir solo defectos reproducidos por 1.x/2.x en el propietario y agregar regresión causal. Área/archivos: rutas canónicas Login 2FA de `design.md`. Origen: D-04, RQ-06. Verificación: no aplicó cambio productivo: la auditoría no reprodujo un defecto del contrato aprobado.

## 3. Automatización y E2E

- [x] 3.1 [S] Registrar paths DOC-96/DOC-97 y ejecutar política DOC-96 y matriz DOC-97 en el workflow existente. Área/archivos: `.github/workflows/opsxj-validation.yml`. Origen: D-05, RQ-07. Verificación: YAML y comandos locales con código 0.
- [x] 3.2 [S] Registrar un único comando/política DOC-97 en infraestructura E2E compartida, sin secretos ni runner alterno. Área/archivos: `tools/e2e/package.json`, `tools/e2e/tests/`, `tools/e2e/scripts/`, perfil example si aplica. Origen: D-06, RQ-08. Verificación: policy/check no abre navegador ni solicita secretos.
- [x] 3.3 [M] Ejecutar matriz E2E viable con autorización nueva, flags inmutables y controles `SELECT`. Área/archivos: `tools/e2e/`, evidencia DOC-97. Origen: D-06, RQ-08. Verificación: GESTOR activo/concurrencia PASSED; combinaciones que exigirían cambiar flags quedaron BLOCKED sin ejecución.

## 4. Documentación y cierre

- [x] 4.1 [M] Documentar arquitectura, matriz, hallazgos, correcciones y riesgo no transaccional. Área/archivos: `Doc/Actualizacion/Login/Implementacion/DOC-97/`. Origen: D-07, RQ-09. Verificación: enlaza pruebas/comandos y no afirma casos bloqueados.
- [x] 4.2 [S] Ejecutar suites 2FA, validación documental/Roslyn aplicable y MSBuild. Área/archivos: repositorio y evidencia DOC-97. Origen: D-03, RQ-07. Verificación: 63/63 pruebas y MSBuild código 0; advertencias legacy registradas.
- [x] 4.3 [S] Validar/sincronizar OpenSpec y cerrar trazabilidad D/RQ/tareas. Área/archivos: `openspec/changes/doc-97-doble-factor-seguridad-regresion/`. Origen: D-07, RQ-09. Verificación: OpenSpec strict y OPSXJ código 0.
