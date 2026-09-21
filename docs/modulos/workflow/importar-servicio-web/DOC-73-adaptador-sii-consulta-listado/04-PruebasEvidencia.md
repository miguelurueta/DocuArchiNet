# Pruebas y evidencia

Comando:

```powershell
node --test tests/importar-servicio-web-sii-query-contract.test.cjs tests/importar-servicio-web-sii-list.test.cjs tests/importar-servicio-web-sii-adapter.test.cjs tests/importar-servicio-web-core.test.cjs tests/importar-servicio-web-provider-registry-ui.test.cjs tests/importar-servicio-web-accessibility.test.cjs tests/importar-servicio-web-gate-regression.test.cjs tests/importar-servicio-web-legacy-regression.test.cjs
```

Resultado: 24 aprobadas, 0 fallidas. No se contactó SII, no se usaron credenciales y no se activó el gate. La validación visual autenticada queda pendiente de autorización operativa.

