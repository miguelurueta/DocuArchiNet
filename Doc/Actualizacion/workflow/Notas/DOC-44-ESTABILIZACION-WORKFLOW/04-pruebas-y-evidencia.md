# Pruebas y evidencia

| Cobertura | Comando | Estado |
| --- | --- | --- |
| Política DOC-44 | `npm.cmd --prefix tools/e2e run test:doc44:policy` | PASS, 6/6 |
| Regresión DOC-43 | `node --test tools/e2e/tests/doc43-notes-ui-policy.test.cjs` | PASS, 8/8 |
| Política contractual Notas | `node --test tools/e2e/tests/notes-workflow-policy.test.cjs` | PASS, 8/8 |
| Sintaxis E2E DOC-44 | `node --check` sobre spec y runner | PASS |
| Compilación .NET Framework | `MSBuild.exe GestionDocumental-Docuarchi.net.sln /t:Build /p:Configuration=Debug /m` | PASS, 0 errores; 310 advertencias históricas |
| E2E real | `npm.cmd --prefix tools/e2e run test:doc44:workflow-notes` | PASS, 1/1 en 20.0 s; tarea 627; evidencia saneada |

La E2E solicita cuenta/clave por TTY, autorización de ambiente, mutación y gate temporal. No persiste secretos, restaura exactamente `Web.config` en `finally` y comprueba gate `false` con usuarios/grupos vacíos. El certificado autofirmado se admite automáticamente solo para `localhost`/loopback; una URL remota conserva la validación TLS. Cualquier control de datos adicional deberá ser un único `SELECT`; no se autorizan escrituras de control.
