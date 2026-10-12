# Validación y evidencia

## Comandos locales

```powershell
node --test tests/doc98-deployment-readiness.test.cjs
node --test tests/login-second-factor-foundation.test.cjs tests/login-second-factor-persistence.test.cjs tests/login-second-factor-smtp.test.cjs tests/login-second-factor-preauthentication.test.cjs tests/login-second-factor-authentication.test.cjs tests/login-second-factor-asmx.test.cjs tests/login-second-factor-ui.test.cjs tests/login-second-factor-regression.test.cjs tests/doc98-deployment-readiness.test.cjs
npm.cmd --prefix tools/e2e run test:doc95:policy
npm.cmd --prefix tools/e2e run test:doc96:policy
npm.cmd --prefix tools/e2e run test:doc97:policy
msbuild GestionDocumental-Docuarchi.net.sln /t:Build /p:Configuration=Debug /m
npx --yes @fission-ai/openspec@1.7.0 validate doc-98-doble-factor-despliegue --strict
```

Estas pruebas no acceden a SMTP, MySQL ni cuentas reales. La prueba DOC-98 valida inventario, límites, sintaxis estática y guardas; no demuestra que un script sea correcto para datos no inspeccionados ni que un ambiente esté activado.

## Evidencia por ambiente

Registrar solo:

| Campo | Ejemplo permitido |
|---|---|
| Ambiente | alias no secreto |
| Autorización | referencia y vigencia |
| Paquete | versión/hash |
| Manifiesto | cantidad de filas y hash del archivo protegido |
| Preflight | PASS/BLOCKED y regla |
| Oleada | número, esperadas, afectadas, postcondiciones |
| E2E | marcadores booleanos saneados |
| Rollback | requerido/no requerido y conteos |
| Estado | `ACTIVADO` o `BLOQUEADO` |

Prohibido registrar: credenciales, conexiones, llaves, key IDs, cookies, OTP, logins, correos, `AuthUserId`, `CodeHash` o payloads.

## Estado de esta entrega

- Prueba contractual DOC-98: PASS, 7/7.
- Regresión estable 2FA: PASS, 42/42.
- Políticas E2E sin infraestructura: DOC-95 7/7, DOC-96 21/21 y DOC-97 3/3.
- Build de solución: PASS, cero errores; conserva dos advertencias preexistentes de caché/binding de ensamblados.
- OpenSpec estricto: PASS.
- OPSXJ refine sincronizado: PASS.
- Mutación de base de datos: no ejecutada por la preparación documental.
- E2E real: no ejecutada sin autorización específica vigente para ambiente, cuentas y datos descartables.
- Activación transversal: `BLOQUEADA` hasta completar una operación autorizada con manifiesto real.

Preparar el mecanismo no equivale a dejar un ambiente activo.
