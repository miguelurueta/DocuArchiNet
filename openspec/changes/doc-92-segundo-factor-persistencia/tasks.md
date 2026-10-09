<!-- opsxj:refinement-traceability version=1 artifact=tasks decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09 -->
# Tareas atómicas — DOC-92

## 1. Contrato compatible

- [ ] 1.1 [S] Agregar `SecondFactorChallengeVerificationData` con `Challenge As SegundoFactorChallenge` y `ProtectedCode As String`, sin modificar modelos existentes. Área/archivos: `Modelo/Login/SegundoFactor/SegundoFactorModels.vb`. Origen: D-05, RQ-05. Trazabilidad adicional: D-01, RQ-01. Verificación: prueba estructural confirma el tipo y la compilación no reporta ruptura en consumidores DOC-91.
- [ ] 1.2 [M] Extender `ISecondFactorChallengeRepository` con `GetVerificationData`, `MarkSent`, `MarkDeliveryFailed`, `ReplaceForResend` y `Expire`, conservando literalmente las firmas existentes. Área/archivos: `Modelo/Login/SegundoFactor/SegundoFactorInterfaces.vb`. Origen: D-05, RQ-05. Verificación: comparación automática del contrato previo y compilación del proyecto.

## 2. Paquete de esquema

- [ ] 2.1 [M] Crear preflight y postflight idempotentes que validen tabla, columnas base, InnoDB, columnas v1 e índices ordenados. Área/archivos: `Doc/Actualizacion/Login/Implementacion/DOC-92/Sql/00-preflight.sql`, `02-postflight.sql`. Origen: D-03, RQ-03. Trazabilidad adicional: D-08, RQ-08. Verificación: validación estructural de `information_schema` y, solo con autorización vigente, ejecución en esquema descartable.
- [ ] 2.2 [M] Crear el apply idempotente para las nueve columnas v1, sin backfill destructivo ni alteración de `AuthPayloadJson`. Área/archivos: `Doc/Actualizacion/Login/Implementacion/DOC-92/Sql/01-apply.sql`. Origen: D-02, RQ-02. Trazabilidad adicional: D-08, RQ-08. Verificación: harness confirma columnas, tipos, guards de existencia y ausencia de DML sobre filas legacy.
- [ ] 2.3 [M] Incorporar al apply los tres índices compuestos y preservar `uq_challengeid` e `IX_ra_auth_sfc_authuserid`. Área/archivos: `Doc/Actualizacion/Login/Implementacion/DOC-92/Sql/01-apply.sql`. Origen: D-03, RQ-03. Verificación: prueba estructural compara nombre/secuencia; integración autorizada ejecuta apply dos veces.
- [ ] 2.4 [M] Crear rollback idempotente que elimine solo índices y columnas DOC-92 y explicite el riesgo de pérdida v1. Área/archivos: `Doc/Actualizacion/Login/Implementacion/DOC-92/Sql/03-rollback.sql`. Origen: D-08, RQ-08. Verificación: prueba coteja inventario inverso; integración autorizada ejecuta apply/rollback en esquema descartable.
- [ ] 2.5 [S] Crear limpieza manual parametrizada para terminales mayores a 30 días y documentar secuencia, permisos, respaldo y ausencia de scheduler. Área/archivos: `Doc/Actualizacion/Login/Implementacion/DOC-92/Sql/04-cleanup-terminal.sql`, `README.md`. Origen: D-08, RQ-08. Verificación: prueba confirma filtro por estado/fecha y ausencia de `CREATE EVENT`, jobs o ejecución automática.

## 3. Repositorio transaccional

- [ ] 3.1 [M] Crear `MySqlSecondFactorChallengeRepository` con constructor inyectado, conexión central y mapeo v1, sin dependencias HTTP ni helper `conect`. Área/archivos: `Infrastructure/Repositories/Login/SegundoFactor/MySqlSecondFactorChallengeRepository.vb`. Origen: D-04, RQ-04. Trazabilidad adicional: D-01, RQ-01. Verificación: prueba estructural inspecciona constructor/imports y dobles validan apertura/cierre.
- [ ] 3.2 [M] Implementar creación, lectura verificable y marcado `SENT`/`DELIVERY_FAILED`, validando identidad, versión, sesión, payload nulo y `v1:keyId:mac`. Área/archivos: `Infrastructure/Repositories/Login/SegundoFactor/MySqlSecondFactorChallengeRepository.vb`. Origen: D-05, RQ-05. Trazabilidad adicional: D-02, RQ-02. Verificación: pruebas validan parámetros, estados, `KeyId`, rechazo legacy/formato inválido y rollback.
- [ ] 3.3 [L] Implementar `RegisterFailedAttempt` y `TryBeginFinalization` con transacción, `SELECT ... FOR UPDATE` y estado condicionado; el quinto fallo bloquea en el mismo commit. Área/archivos: `Infrastructure/Repositories/Login/SegundoFactor/MySqlSecondFactorChallengeRepository.vb`. Origen: D-06, RQ-06. Verificación: dobles validan orden/estados; integración autorizada demuestra exactamente un ganador concurrente.
- [ ] 3.4 [M] Implementar `Complete`, `FailFinalization`, `Revoke` y `Expire` con estados previos, timestamps y `Consumed=1` solo en `COMPLETED`. Área/archivos: `Infrastructure/Repositories/Login/SegundoFactor/MySqlSecondFactorChallengeRepository.vb`. Origen: D-07, RQ-07. Verificación: pruebas cubren cada transición válida/inválida, expiración y rollback.
- [ ] 3.5 [L] Implementar `ReplaceForResend` como revocación/creación atómicas, con cooldown de 60 segundos, máximo dos reenvíos y nunca dos elegibles. Área/archivos: `Infrastructure/Repositories/Login/SegundoFactor/MySqlSecondFactorChallengeRepository.vb`. Origen: D-07, RQ-07. Trazabilidad adicional: D-06, RQ-06. Verificación: pruebas cubren límite, contador, fallo de inserción y concurrencia.

## 4. Integración de compilación

- [ ] 4.1 [S] Registrar en orden los nuevos `.vb` del modelo y repositorio. Área/archivos: `GestionDocumental-Docuarchi.net.vbproj`. Origen: D-01, RQ-01. Trazabilidad adicional: D-05, RQ-05. Verificación: MSBuild muestra los `Compile Include` y compila sin inclusiones duplicadas.

## 5. Pruebas y evidencia

- [ ] 5.1 [L] Crear pruebas focales con dobles para creación, legacy, lectura, estados, quinto intento, expiración, reenvío, identidades y commit/rollback. Área/archivos: `tests/LoginSecondFactorPersistenceBehaviorTests.cs` y harness existente. Origen: D-09, RQ-09. Trazabilidad adicional: D-02, D-06, D-07, RQ-02, RQ-06, RQ-07. Verificación: ejecutar filtro DOC-92 y registrar comando, salida y código 0.
- [ ] 5.2 [M] Crear validación estructural de rutas, contrato, SQL parametrizado, ausencia HTTP, scripts e índices. Área/archivos: `tests/login-second-factor-persistence.test.cjs` o `tools/validation/`, según runner existente. Origen: D-04, RQ-04. Trazabilidad adicional: D-05, D-08, D-09, RQ-05, RQ-08, RQ-09. Verificación: ejecutar Node test y comprobar diagnóstico focal con fixture inválido.
- [ ] 5.3 [L] Incorporar harness MySQL descartable para apply/reapply, legacy, rollback y carrera de un ganador, protegido por autorización y variables no versionadas. Área/archivos: harness MySQL existente bajo `tools/` y documentación DOC-92; no es E2E de navegador. Origen: D-06, RQ-06. Trazabilidad adicional: D-03, D-09, RQ-03, RQ-09. Verificación: autorizado, salida saneada/código 0; no autorizado, caso no ejecutado y bloqueo documentado.
- [ ] 5.4 [M] Ejecutar compilación, pruebas DOC-92, regresiones DOC-91 y OpenSpec estricto sin DDL real no autorizado. Área/archivos: solución, `tests/`, `tools/opsxj/`, `openspec/changes/doc-92-segundo-factor-persistencia/`. Origen: D-09, RQ-09. Verificación: registrar comandos, códigos y cualquier suite bloqueada sin declararla aprobada.

## 6. Documentación y cierre

- [ ] 6.1 [M] Actualizar documentación con esquema, índices, clases, firmas, transacciones, estados, rollout/rollback y exclusiones. Área/archivos: `Doc/Actualizacion/Login/Implementacion/DOC-92/`, `Doc/Tecnica/Opsxj/doc-92-segundo-factor-persistencia/`. Origen: D-08, RQ-08. Trazabilidad adicional: D-01, RQ-01. Verificación: inventario documental coincide con código/scripts por rutas y símbolos.
- [ ] 6.2 [S] Consolidar evidencia, autorización o bloqueo MySQL, riesgos y archivos sin secretos. Área/archivos: `Doc/Actualizacion/Login/Implementacion/DOC-92/README.md`, artefactos OpenSpec DOC-92. Origen: D-09, RQ-09. Verificación: `opsxj:refine --sync`, `openspec validate --strict` y `git diff --check` exitosos.
