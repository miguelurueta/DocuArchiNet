# Pruebas y evidencia

Fecha de evidencia: 2026-09-06. Todos los comandos focales se ejecutan sin sesión, red ni credenciales.

| Verificación | Comando | Resultado | Estado |
| --- | --- | --- | --- |
| Suites DOC-50 | `node --test Tests/importar-servicio-web-contracts.test.cjs Tests/importar-servicio-web-provider-registry.test.cjs Tests/importar-servicio-web-context.test.cjs` | 11 pruebas aprobadas, 0 fallidas | PASS |
| Build .NET Framework | `MSBuild.exe GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug /m /nologo /verbosity:minimal` | Código 0; generó `bin/GestionDocumental-Docuarchi.net.dll`; advertencias legacy preexistentes, sin errores | PASS |
| OpenSpec estricto | `openspec.cmd validate doc-50-contratos-multi-proveedor --strict` | Cambio válido | PASS |
| Refinamiento OPSXJ | `npm.cmd --prefix Tools/opsxj run opsxj:refine -- DOC-50` | Refinamiento aprobado y trazable | PASS |

Las pruebas cubren ocho operaciones y contratos auxiliares, versión y fixtures, rutas canónicas y no duplicación, registro conocido/inválido/duplicado/desconocido, inmutabilidad, orden de autorización y fachada sin efectos.

## Corridas excluidas

- E2E autenticado: **NO EJECUTADO — requiere autorización explícita para ambiente y cuentas de prueba**.
- Carga: **NO EJECUTADA — requiere autorización explícita**.
- Activación de `WorkflowCentroTrabajoModernActive`: **NO EJECUTADA — requiere autorización explícita**.

No se alteraron gates y no hubo efectos de importación. Si una corrida autenticada fuera autorizada posteriormente, deberá seguir `tools/e2e/AGENT-RUNBOOK.md` y terminar con el gate desactivado y listas de usuarios/grupos vacías.
