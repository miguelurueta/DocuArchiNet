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

## Cobertura E2E DOC-75

Se extendió el inspector UI no mutante del escenario compartido `import-sii-read`; no se creó un login, transporte ni runner paralelo. La extensión valida en navegador real:

- popup y tabla contenidos dentro del viewport de `760 x 900`;
- región de tabla accesible y con scroll horizontal/vertical propio;
- preparación individual, catálogo autorizado, preflight y habilitación posterior del botón;
- cancelación y restauración del foco al botón de la fila;
- preparación múltiple cuando el perfil solicita al menos dos elementos importables;
- ausencia total de solicitudes `CreateImportIntent` y `ExecuteImportIntent` durante la inspección.

```text
node --test tools/e2e/tests/importar-servicio-web-modern.spec.cjs tools/e2e/tests/workflow-e2e-platform.test.cjs
Resultado de política/plataforma: PASS, 27/27.
```

La E2E autenticada real continúa pendiente: requiere autorización explícita para el ambiente y la cuenta, un perfil no sensible con `sampleSize >= 2`, controles de solo lectura y restauración del gate. La evidencia solo podrá marcar preparación múltiple como confirmada cuando existan dos filas importables en la respuesta real.

## Limitaciones

La disponibilidad productiva continúa condicionada a B11 en el ambiente objetivo.
