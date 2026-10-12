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

- Prueba contractual DOC-98: PASS, 10/10.
- Regresión estable 2FA: PASS, 42/42.
- Políticas E2E sin infraestructura: DOC-95 7/7, DOC-96 21/21 y DOC-97 3/3.
- Build de solución: PASS, cero errores; conserva una advertencia preexistente de binding de ensamblados.
- OpenSpec estricto: PASS.
- OPSXJ refine sincronizado: PASS.
- Preflight autorizado y saneado: PASS; esquema, tipos objetivo, expiración, cardinalidad SMTP y ausencia de ambigüedad conformes.
- Activación global autorizada: cinco filas en el manifiesto, una previamente activa y cuatro filas afectadas; postcondición global y filas externas conformes.
- Ensayo de reversa: PASS; tras una E2E transversal fallida se restauraron cuatro filas y se conservaron las filas externas.
- Corrección del arnés: se eliminó una doble normalización del snapshot. El `WHERE` exacto impidió una escritura con precondición divergente; las pruebas ahora comparan filas crudas y normalizadas.
- E2E real RADICACION: PASS; estado pendiente, OTP válido y cookie autenticada.
- E2E real WORKFLOW REGISTRO: PASS; estado pendiente, OTP válido y cookie autenticada.
- E2E Docuarchi.net: descartada por indicación del operador; no se declara ejecutada.
- Verificación final autorizada y solo lectura: las cinco filas objetivo están activas, ninguna fila externa cambió y `DOC98_VERIFY_READY=true`.
- Activación transversal del ambiente autorizado: `ACTIVADO`. El operador confirmó que WORKFLOW REGISTRO era la única prueba pendiente y descartó repetir Docuarchi.net; DOC-98 no atribuye a esta ronda una E2E que no se ejecutó.

La evidencia confirma configuración y correspondencia estructural. No demuestra por sí sola que todos los usuarios tengan identidad y correo válidos; esos datos continúan fallando de forma cerrada durante cada login.
