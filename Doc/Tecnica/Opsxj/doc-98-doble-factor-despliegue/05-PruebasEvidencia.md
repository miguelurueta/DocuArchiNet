# DOBLE-FACTOR-DESPLIEGUE

- Ticket: DOC-98
- Cambio OpenSpec: doc-98-doble-factor-despliegue
- Clasificacion: cross_cutting (Transversal)
## Evidencia requerida

- unit: `node --test tests/doc98-deployment-readiness.test.cjs`; 7/7 PASS el 2026-10-12.
- suite 2FA estable: 42/42 PASS.
- politicas E2E sin infraestructura: DOC-95 7/7, DOC-96 21/21 y DOC-97 3/3 PASS.
- build: PASS con cero errores y dos advertencias preexistentes de cache/binding.
- OpenSpec estricto y `opsxj:refine -- DOC-98 --sync`: PASS.
- manual_qa/operacion real: `BLOQUEADA`; falta autorizacion especifica de ambiente, manifiesto, ventana y cuentas descartables.

## QA/E2E WebForms

Se reutilizan las politicas y E2E DOC-95/DOC-96/DOC-97. No se ejecuta E2E real ni SQL mutable sin autorizacion expresa. Una futura evidencia debe ser saneada y no incluir OTP, correo, login, cookie, llave, `AuthUserId`, `CodeHash`, credenciales o conexiones.

La prueba local DOC-98 valida inventario, trazabilidad y guardas estaticas; no demuestra por si sola una activacion correcta en un ambiente real.
