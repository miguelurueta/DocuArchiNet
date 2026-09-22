# Pruebas y evidencia

## Focales y regresión

```text
node --test Tests/importar-servicio-web-preparation.test.cjs Tests/importar-servicio-web-preflight-contract.test.cjs Tests/importar-servicio-web-intent-client.test.cjs Tests/importar-servicio-web-preview.test.cjs Tests/importar-servicio-web-preview-security.test.cjs Tests/importar-servicio-web-preview-accessibility.test.cjs Tests/importar-servicio-web-provider-registry-ui.test.cjs Tests/importar-servicio-web-sii-adapter.test.cjs Tests/importar-servicio-web-sii-contract-mapping.test.cjs
Resultado focal inicial: PASS, 28/28.
```

La regresión ampliada con contratos B09/B11, superficies legacy y almacenamiento aprobó 46/46. Cobertura: cardinalidad, catálogo, estados, preflight, plan, deduplicación, stale, foco e invariancias.

```text
msbuild GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug /m:1 /v:minimal /nologo
Resultado: PASS; se conservaron advertencias históricas del proyecto legacy.

openspec.cmd validate doc-75-importacion-interfaz-integracion-sii --strict
Resultado: PASS.
```

La E2E real no se ejecutó en este corte: requiere autorización explícita, ambiente/cuenta controlados y restauración del gate.

## Limitaciones

La disponibilidad productiva continúa condicionada a B11 en el ambiente objetivo.
