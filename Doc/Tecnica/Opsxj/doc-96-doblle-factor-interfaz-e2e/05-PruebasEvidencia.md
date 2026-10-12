# DOBLLE-FACTOR-INTERFAZ-E2E

- Ticket: DOC-96
- Cambio OpenSpec: doc-96-doblle-factor-interfaz-e2e
- Clasificacion: cross_cutting (Transversal)

## Evidencia requerida

- [x] unit — 2026-10-11: `node --test tests/login-second-factor-ui.test.cjs tests/doc96-login-second-factor-ui-policy.test.cjs`, PASS 13/13; `npm.cmd --prefix tools/e2e run test:doc96:policy`, PASS 21/21.
- [x] manual_qa — 2026-10-11: recorridos reales autorizados ON/OFF y escenarios de refresh, cancelación, inválido, bloqueo, reenvío, expiración y doble activación, todos con `DOC96_E2E_RESULT=PASSED`; restauración final `GESTOR|2FA=ON` confirmada por `SELECT`.
- [x] regression — 2026-10-11: regresiones DOC-91 a DOC-95, PASS 44/44.
- [x] build — 2026-10-11: `msbuild.exe GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug`, código 0; conserva advertencias preexistentes de versiones de ensamblados.
- [x] specification — 2026-10-11: OpenSpec estricto, 74 elementos aprobados después del archivo y sincronización.

## QA/E2E WebForms

El runner reutiliza `tools/e2e`, abre Chromium visible, exige autorización interactiva y mantiene secretos solo en memoria. La matriz ON verificó que no existe cookie antes del OTP, que la navegación posterior es local y que los terminales no autentican. El control OFF temporal autorizado cambió únicamente `gestor_modulos.RequiereSegundoFactor`, validó login legacy, restauró el snapshot ON en `finally` y confirmó la restauración mediante diagnóstico independiente de solo lectura.

La evidencia detallada y saneada está en `Doc/Actualizacion/Login/Implementacion/DOC-96/matriz-pruebas-evidencia.md`. No se almacenan OTP, correo completo, usuario, contraseña, cookies, tokens ni cadenas de conexión.
