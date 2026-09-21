# Pruebas y evidencia

Fecha: 2026-09-21.

Comando focal:

```text
node --test tests/importar-servicio-web-core.test.cjs tests/importar-servicio-web-provider-registry-ui.test.cjs tests/importar-servicio-web-accessibility.test.cjs tests/importar-servicio-web-gate-regression.test.cjs tests/importar-servicio-web-legacy-regression.test.cjs
```

Resultado: PASS, 18 pruebas aprobadas y 0 fallidas. La ejecución fue local con adaptadores falsos; no usó red, credenciales ni cambió el gate.

Validaciones adicionales: los cuatro módulos pasaron `node --check`; MSBuild de `GestionDocumental-Docuarchi.net.vbproj` finalizó con código 0 y produjo el ensamblado. Permanecen advertencias históricas del proyecto sobre referencias y código legacy, sin errores de compilación.

No se ejecutó E2E autenticado: no existe autorización explícita de ambiente y cuentas para esta sesión.
