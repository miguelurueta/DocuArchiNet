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

## Infraestructura E2E reutilizada

Se amplió quirúrgicamente el escenario existente `import-sii-read` de la plataforma DOC-56. La corrida conserva una sola sesión autenticada, selección autorizada de tarea, activación temporal del gate y de `INTEGRACIONSII`, restauración exacta de ambos valores, controles ODBC exclusivamente `SELECT`, evidencia saneada y cierre en `finally`.

El inspector UI agregado verifica apertura del modal, preview mediado, foco inicial, una sola llamada a `GetPreview` ante foco/resize, retorno a la lista, restauración del foco y cierre. Para aislar la dependencia DOC-73, completa el `codigoBarras` vacío en el POST real `QueryItems` con el valor no sensible del perfil y continúa la misma petición al ASMX; no simula respuestas ni crea otro cliente. No se creó otro runner ni otro perfil.

```text
node --test tools/e2e/tests/workflow-e2e-platform*.test.cjs tools/e2e/tests/importar-servicio-web-modern.spec.cjs
Resultado: PASS, 40/40.
```

Esta validación fue local y no autenticada. La corrida real preparada sigue siendo `test:workflow:platform` con el perfil existente `doc56-import-sii-read.profile.example.json` y requiere autorizaciones interactivas de ambiente, gate y TLS local cuando aplique.

## Limitaciones

- No se ejecutó E2E real: la infraestructura quedó preparada, pero requiere autorización explícita de ambiente, cuentas y tarea.
- No se activó el gate ni se hizo carga.
- La disponibilidad productiva de B10 sigue pendiente de confirmación externa.
