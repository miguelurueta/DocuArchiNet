# DOC-92 — Pruebas y evidencia

Fecha: 2026-10-09.

| Comando | Código | Resultado |
| --- | ---: | --- |
| `msbuild.exe GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug /m:1 /v:minimal` | 0 | Compilación correcta; advertencias legacy preexistentes |
| `node --test tests/login-second-factor-persistence.test.cjs tests/login-second-factor-foundation.test.cjs` | 0 | 6/6 pruebas aprobadas |
| `npm.cmd --prefix tools/e2e run test:doc92:persistence` | 0 | Harness localizado; integración omitida por falta de autorización vigente |

Las pruebas locales validan contrato, SQL emitido, parámetros, bloqueo declarado, quinto intento, rollback del reenvío y regresión DOC-91 con dobles. No demuestran el aislamiento real de MySQL.

La integración real está automatizada para una base descartable cuyo nombre debe comenzar por `doc92_`; exige `DOC92_MYSQL_AUTHORIZED=SI` y secretos efímeros. En esta ejecución quedó `SKIP`. No es una prueba E2E WebForms porque DOC-92 no tiene recorrido de usuario.
