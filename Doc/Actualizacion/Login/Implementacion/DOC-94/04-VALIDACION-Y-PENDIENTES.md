# Validación y pendientes

## Evidencia local

| Validación | Comando | Resultado |
| --- | --- | --- |
| Comportamiento DOC-94 | `node --test tests/login-second-factor-preauthentication.test.cjs` | 6 pruebas aprobadas. |
| Regresión de correo condicionado | incluida en `LoginSecondFactorPreAuthenticationBehaviorTests.cs` | Los cuatro módulos finalizan con `0`/`NULL` y correo ausente; con `1`, correo ausente devuelve `SECOND_FACTOR_EMAIL_UNAVAILABLE` y no invoca el finalizador. |
| Regresión DOC-91/92/93/94 | `node --test --test-concurrency=1 tests/login-second-factor-foundation.test.cjs tests/login-second-factor-persistence.test.cjs tests/login-second-factor-smtp.test.cjs tests/login-second-factor-preauthentication.test.cjs tests/doc91-technical-documentation.test.cjs tests/doc92-technical-documentation.test.cjs tests/doc93-technical-documentation.test.cjs tests/doc94-technical-documentation.test.cjs` | 31/31 pruebas aprobadas. DOC-93 requirió incluir el nuevo contexto en su compilación aislada. |
| Documentación DOC-94 | `node --test tests/doc94-technical-documentation.test.cjs` | 5/5 pruebas aprobadas; valida inventario requerido, Mermaid, símbolos y CI. |
| Contrato estructural DOC-94 | `dotnet run --project ./tools/validation/Doc72SourceValidator/Doc72SourceValidator.csproj -- ./Doc/Actualizacion/Login/Implementacion/DOC-94/diagram-contract.json .` | Código 0: 21 declaraciones, 5 relaciones, 1 enum, 11 firmas y 6 tipos con propiedades. |
| Regresión documental DOC-91 | `node --test tests/doc91-technical-documentation.test.cjs` y el validador Roslyn con su manifiesto | 4/4 pruebas aprobadas; Roslyn aprobó 32 declaraciones, 2 enums, 34 firmas y 4 tipos con propiedades. |
| Compilación | `msbuild.exe GestionDocumental-Docuarchi.net.sln /t:Build /p:Configuration=Debug /clp:ErrorsOnly` | Código 0. |
| OpenSpec | `npx.cmd openspec validate doc-94-doble-factor-preautenticacion --strict` | Código 0; cambio válido. |
| Auditoría OPSXJ | `npm.cmd --prefix tools/opsxj run opsxj:refine -- DOC-94 --sync` | Código 0; refinamiento aprobado y trazable con design, spec y tasks. |

No se ejecutaron E2E autenticados, consultas de base reales, envío SMTP ni carga. Las pruebas usan dobles locales.

## Pendientes operativos

- Antes de desplegar, comprobar mediante `SELECT` autorizado que los módulos objetivo tengan `RequiereSegundoFactor` en `0` o `NULL`.
- DOC-94 no ofrece challenge/UI; un valor `1` detiene el login deliberadamente.
- Las Jiras posteriores deben consumir el mismo `ILegacyLoginFinalizer` después de verificar el challenge.

## Rollback

Revertir el artefacto de aplicación a la versión anterior. No existe rollback SQL porque DOC-94 no modifica esquema ni datos.

## Límite de la prueba documental

La prueba valida existencia de archivos, sintaxis Mermaid y correspondencia estructural de declaraciones, implementaciones, métodos, parámetros y retornos. No demuestra por sí sola la fidelidad completa del comportamiento ni la cobertura de todos los escenarios de producción.
