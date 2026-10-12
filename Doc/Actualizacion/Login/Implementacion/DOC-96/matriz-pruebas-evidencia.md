# Matriz de pruebas y evidencia DOC-96

## Resultados automatizados locales

| Fecha | Comando | Resultado | Alcance |
| --- | --- | --- | --- |
| 2026-10-11 | `node --test tests/login-second-factor-ui.test.cjs tests/doc96-login-second-factor-ui-policy.test.cjs` | PASS, 13/13 | Cliente, payload, estado, carrera, ruta y markup. |
| 2026-10-11 | `npm.cmd --prefix tools/e2e run test:doc96:policy` | PASS, 21/21 | Helper compatible, perfil, escenarios, navegador visible, sanitización, códigos de etapa y control temporal restaurable. |
| 2026-10-11 | `node --test --test-concurrency=1 ...DOC-91 a DOC-95...` | PASS, 34/34 | Regresión de fundación, persistencia, SMTP, preautenticación y ASMX. |
| 2026-10-11 | `msbuild.exe GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug /m:1 /v:minimal` | PASS, código 0 | Compilación Web Application. |
| 2026-10-11 | `npm.cmd --prefix tools/e2e run test:doc96:login-ui -- --scenario matrix --profile profiles/doc96-login-second-factor-ui.profile.example.json --check` | PASS; ejecución real `false` | Valida estructura de configuración sin navegador ni TTY. |
| 2026-10-11 | `npm.cmd --prefix tools/e2e run test:doc96:login-ui -- --scenario verify --profile <perfil-local-no-versionado>` | PASS; `DOC96_E2E_RESULT=PASSED` | `GESTOR` ON: panel visible, destino enmascarado, OTP automatizado, navegación local y cookie posterior a completar. |
| 2026-10-11 | `npm.cmd --prefix tools/e2e run test:doc96:login-ui -- --scenario refresh --profile <perfil-local-no-versionado>` | PASS; `DOC96_E2E_RESULT=PASSED` | `GESTOR` ON: el estado pendiente se restaura después de recargar, sin autenticación anticipada. |
| 2026-10-11 | `npm.cmd --prefix tools/e2e run test:doc96:login-ui -- --scenario cancel --profile <perfil-local-no-versionado>` | PASS; `DOC96_E2E_RESULT=PASSED` | `GESTOR` ON: cancelar limpia la transición visual y vuelve al login sin cookie autenticada. |
| 2026-10-11 | `npm.cmd --prefix tools/e2e run test:doc96:login-ui -- --scenario invalid --profile <perfil-local-no-versionado>` | PASS; `DOC96_E2E_RESULT=PASSED` | `GESTOR` ON: un OTP inválido conserva el estado pendiente y no crea cookie autenticada. |
| 2026-10-11 | `npm.cmd --prefix tools/e2e run test:doc96:login-ui -- --scenario double-submit --profile <perfil-local-no-versionado>` | PASS; `DOC96_E2E_RESULT=PASSED` | `GESTOR` ON: dos activaciones inmediatas producen una sola solicitud `Verificar` y autentican una vez. |
| 2026-10-11 | `npm.cmd --prefix tools/e2e run test:doc96:login-ui -- --scenario blocked --profile <perfil-local-no-versionado>` | PASS; `DOC96_E2E_RESULT=PASSED` | `GESTOR` ON: cinco códigos inválidos cierran el challenge, vuelven al login y no autentican. |
| 2026-10-11 | `npm.cmd --prefix tools/e2e run test:doc96:login-ui -- --scenario resend --profile <perfil-local-no-versionado>` | PASS; `DOC96_E2E_RESULT=PASSED` | `GESTOR` ON: respeta cooldown, invalida el OTP anterior y autentica únicamente con el OTP nuevo. |
| 2026-10-11 | `npm.cmd --prefix tools/e2e run test:doc96:login-ui -- --scenario expiry --profile <perfil-local-no-versionado>` | PASS; `DOC96_E2E_RESULT=PASSED` | `GESTOR` ON: vence sin usar OTP, vuelve al login y no emite solicitud de verificación ni cookie autenticada. |
| 2026-10-11 | `node tools/e2e/scripts/run-doc96-gestor-off-temporary-interactive.cjs --db-profile <perfil-local-no-versionado> --e2e-profile <perfil-local-no-versionado>` | PASS; `DOC96_GESTOR_OFF_CONTROL=PASSED` | Con autorización explícita, cambia únicamente `RequiereSegundoFactor` de `GESTOR` a OFF durante la corrida, valida login legacy sin panel OTP y cookie autenticada, restaura ON en `finally` y verifica `DOC96_GESTOR_RESTORED_ON=true`. |
| 2026-10-11 | `node tools/e2e/scripts/inspect-doc96-module-second-factor-interactive.cjs --db-profile <perfil-local-no-versionado>` | PASS; `GESTOR|2FA=ON` | Confirmación independiente posterior, exclusivamente con `SELECT`, de la restauración del módulo. |

Las advertencias de conflictos de ensamblado de .NET Framework permanecen preexistentes; no impidieron la compilación.

## Matriz de módulos

| Tipo de módulo | 2FA | Caso mínimo | Estado de evidencia real |
| --- | --- | --- | --- |
| Gestión Documental (`GESTOR`) | ON en el ambiente autorizado | `verify` y control temporal `off` autorizado | PASS: recorridos ON/OFF completos, restauración ON verificada. |
| Workflow Documental | Configurable | OFF u ON según ambiente aprobado | No ejecutado en DOC-96. |
| Radicación Documental | Configurable | OFF u ON según ambiente aprobado | No ejecutado en DOC-96. |
| Administrador Documental | Configurable | OFF u ON según ambiente aprobado | No ejecutado en DOC-96. |

La cobertura funcional usa la configuración real ON de `GESTOR`. Debido a que las cuentas suministradas para módulos OFF no completaron el login legacy, el control OFF final se ejecutó sobre el mismo módulo mediante un cambio temporal y explícitamente autorizado de `RequiereSegundoFactor`; el arnés limitó la escritura a esa columna, restauró el valor original en `finally` y confirmó después el estado ON con una consulta independiente de solo lectura. No hubo cambio persistente de configuración.

## Escenarios del runner

| Escenario | Resultado esperado |
| --- | --- |
| `off` | Login legacy, sin panel OTP y con autenticación normal. |
| `verify` | Panel enmascarado, sin cookie previa, OTP válido y navegación local. |
| `matrix` | Ejecuta `off` y después `verify` en una autorización. |
| `refresh` | Recarga y recupera el panel desde Session. |
| `invalid` | Código inválido mantiene panel y no autentica. |
| `blocked` | Cinco intentos autorizados cierran el panel sin cookie. |
| `resend` | Respeta cooldown, invalida código anterior y acepta el nuevo. |
| `cancel` | Cancela y vuelve a credenciales sin cookie. |
| `expiry` | Espera por condición observable hasta que servidor cierre el estado. |
| `double-submit` | Dos activaciones cliente producen una sola solicitud `Verificar`. |

## Comandos

Política sin autenticación:

```powershell
npm.cmd --prefix tools/e2e run test:doc96:policy
```

Comprobación de perfil sin navegador:

```powershell
npm.cmd --prefix tools/e2e run test:doc96:login-ui -- --scenario matrix --profile <perfil-local-no-versionado> --check
```

E2E real, solo después de leer `tools/e2e/AGENT-RUNBOOK.md` y obtener autorización nueva:

```powershell
npm.cmd --prefix tools/e2e run test:doc96:login-ui -- --scenario matrix --profile <perfil-local-no-versionado>
```

## Estado de validación real

Las corridas reales autorizadas `verify`, `refresh`, `cancel`, `invalid`, `double-submit`, `blocked`, `resend` y `expiry` sobre `GESTOR`, único módulo activo para doble factor, terminaron con `DOC96_E2E_RESULT=PASSED`. El control OFF terminó con `DOC96_OFF_LOGIN_LEGACY_OK=true`, `DOC96_OFF_AUTH_COOKIE_OK=true`, `DOC96_E2E_RESULT=PASSED`, `DOC96_GESTOR_RESTORED_ON=true` y `DOC96_GESTOR_OFF_CONTROL=PASSED`. Las tareas OpenSpec 4.3 y 4.4 quedan validadas.

## Protección de evidencia

Los resultados admitidos son banderas `DOC96_*` y códigos de etapa. No se registran OTP, correos, usuarios, contraseñas, cookies, challenges, conexiones ni respuestas completas.
