# Pruebas y evidencia

Fecha local: 2026-09-22. Rama: `feature/DOC-74`.

## Pruebas focales

```text
node --test Tests/importar-servicio-web-preview.test.cjs Tests/importar-servicio-web-preview-security.test.cjs Tests/importar-servicio-web-preview-accessibility.test.cjs
Resultado final: PASS, 12/12.
```

Cobertura: estados, deduplicación, renovación, fallback, descriptor seguro, prohibición de transporte/URL externa, foco, retorno y responsive.

## Regresión relacionada

```text
node --test Tests/importar-servicio-web-provider-registry-ui.test.cjs Tests/importar-servicio-web-sii-adapter.test.cjs Tests/importar-servicio-web-sii-contract-mapping.test.cjs Tests/importar-servicio-web-preview-mediation.test.cjs Tests/importar-servicio-web-preview-content.test.cjs Tests/importar-servicio-web-preview-single-fetch.test.cjs Tests/importar-servicio-web-legacy-surface-invariance.test.cjs Tests/importar-servicio-web-storage-invariance.test.cjs
Resultado: PASS, 27/27.
```

## Limitaciones

- No se ejecutó E2E real: requiere autorización explícita de ambiente y cuentas.
- No se activó el gate ni se hizo carga.
- La disponibilidad productiva de B10 sigue pendiente de confirmación externa.
