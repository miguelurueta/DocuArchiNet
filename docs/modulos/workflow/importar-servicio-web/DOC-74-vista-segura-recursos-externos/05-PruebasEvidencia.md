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

La validación 40/40 anterior fue local y no autenticada. La corrida real usa `test:workflow:platform` y requiere autorizaciones interactivas de ambiente, gate y TLS local cuando aplique.

## E2E real autorizada

```text
npm.cmd --prefix tools/e2e run test:workflow:platform -- --scenario import-sii-read --profile doc74-import-sii-preview-220585.runtime.json --authorize environment,gate,local-tls
Resultado: PASS; 2026-09-22 12:20:33.
```

La evidencia saneada confirmó `uiPreview`, `uiFocus` y `uiSingleFetch`; los siete controles de intención, ítems, transiciones, expediente, relación, caché e índices permanecieron sin cambios. El gate regresó a `false`, el proveedor temporal se restauró a vacío y no quedaron diferencias en las páginas Workflow.

## Limitaciones

- La E2E real de lectura fue ejecutada con autorización explícita; no se realizó ejecución mutante ni carga.
- El gate y el proveedor se habilitaron únicamente durante la corrida y fueron restaurados al finalizar.
- La disponibilidad productiva de B10 sigue pendiente de confirmación externa.
