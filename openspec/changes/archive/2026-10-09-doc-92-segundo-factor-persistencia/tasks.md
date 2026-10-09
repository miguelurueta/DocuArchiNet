<!-- opsxj:refinement-traceability version=1 artifact=tasks decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09,D-10 -->
# Tareas atómicas — DOC-92

- [x] 1.1 [S] Mantener proyección persistida sin login. Área/archivos: `Modelo/Login/SegundoFactor/SegundoFactorModels.vb`. Origen: D-05, RQ-05. Verificación: compilación y prueba de identidad canónica.
- [x] 1.2 [S] Extender el puerto sin retirar firmas DOC-91. Área/archivos: `Modelo/Login/SegundoFactor/SegundoFactorInterfaces.vb`. Origen: D-05, RQ-05. Verificación: prueba estructural de firmas.
- [x] 2.1 [M] Implementar inserción y lectura usando solo las diez columnas existentes. Área/archivos: repositorio MySQL 2FA. Origen: D-02, RQ-02. Verificación: inspección de SQL y prueba con dobles.
- [x] 2.1a [S] Preservar los índices actuales sin crear, eliminar ni alterar índices. Área/archivos: repositorio y documentación SQL DOC-92. Origen: D-03, RQ-03. Verificación: ausencia de DDL y acceso parametrizado por `ChallengeId`.
- [x] 2.2 [L] Implementar intentos y adquisición mediante `FOR UPDATE` y condiciones existentes. Área/archivos: repositorio MySQL 2FA. Origen: D-06, RQ-06. Verificación: quinto intento y ganador único.
- [x] 2.3 [M] Implementar consumo, expiración y reemplazo sin estados físicos adicionales. Área/archivos: repositorio MySQL 2FA. Origen: D-07, RQ-07. Verificación: transiciones derivadas y rollback.
- [x] 2.4 [S] Registrar el repositorio en el proyecto. Área/archivos: `GestionDocumental-Docuarchi.net.vbproj`. Origen: D-01, RQ-01. Verificación: MSBuild.
- [x] 3.1 [S] Eliminar scripts DDL y documentar que no aplica migración. Área/archivos: `Doc/Actualizacion/Login/Implementacion/DOC-92/Sql/`. Origen: D-08, RQ-08. Verificación: solo existe README sin SQL ejecutable.
- [x] 3.2 [M] Validar ausencia de columnas nuevas, contexto HTTP y SQL no parametrizado. Área/archivos: `tests/login-second-factor-persistence.test.cjs`. Origen: D-04, RQ-04. Verificación: Node test.
- [x] 3.3 [L] Cubrir comportamiento transaccional con dobles. Área/archivos: `tests/LoginSecondFactorPersistenceBehaviorTests.cs`. Origen: D-06, RQ-06. Verificación: runner DOC-92.
- [x] 3.4 [L] Mantener harness MySQL exacto y protegido. Área/archivos: `tools/e2e/tests/login-second-factor-persistence.integration.test.cjs`. Origen: D-09, RQ-09. Verificación: `SKIP` sin autorización o código 0 autorizado.
- [x] 4.1 [M] Actualizar documentación técnica y limitaciones. Área/archivos: documentación DOC-92. Origen: D-07, RQ-07. Verificación: consistencia con código.
- [x] 4.2 [M] Ejecutar MSBuild y regresiones DOC-91/DOC-92. Área/archivos: proyecto y pruebas. Origen: D-09, RQ-09. Verificación: códigos de salida registrados.
- [x] 4.3 [S] Validar OpenSpec, trazabilidad y diff. Área/archivos: cambio OpenSpec DOC-92. Origen: D-09, RQ-09. Verificación: validadores en código 0.
- [x] 5.1 [L] Documentar arquitectura, diagramas, casos de uso, inventario y pendientes contra el código existente. Área/archivos: `Doc/Actualizacion/Login/Implementacion/DOC-92/`. Origen: D-10, RQ-10. Verificación: revisión cruzada de rutas, firmas y comportamiento.
- [x] 5.2 [M] Crear manifiesto explícito y prueba Mermaid para documentos, diagramas y referencias. Área/archivos: `diagram-contract.json`, `tests/doc92-technical-documentation.test.cjs`. Origen: D-10, RQ-10. Verificación: Node test.
- [x] 5.3 [M] Extender el validador Roslyn para relaciones interfaz/implementación e integrar DOC-92 en CI. Área/archivos: `tools/validation/Doc72SourceValidator/Program.cs`, `.github/workflows/opsxj-validation.yml`. Origen: D-10, RQ-10. Verificación: dotnet run y contrato CI.
- [x] 5.4 [S] Ejecutar pruebas documentales, OpenSpec estricto y registrar resultados honestos. Área/archivos: evidencia DOC-92. Origen: D-10, RQ-10. Verificación: comandos y códigos de salida.
