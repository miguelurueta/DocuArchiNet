# IMPORTAR-SERVICIO-ENLACE-SII

- Ticket: DOC-80
- Cambio OpenSpec: doc-80-importar-servicio-enlace-sii
- Clasificacion: cross_cutting (Transversal)
## Evidencia requerida

- [x] unit: `node --test --test-reporter=dot tests/importar-servicio-web-*.test.cjs`; PASS, 2026-09-24. Focal DOC-80: 6/6 PASS.
- [x] manual_qa: revisión de diff, `git diff --check`, OpenSpec estricto y MSBuild Compile; PASS, 2026-09-24.

## QA/E2E WebForms

Las pruebas E2E automatizadas no se suponen disponibles. Cuando aplique, registrar ambiente, pasos manuales, resultado y limitacion; si hay automatizacion real, adjuntar comando y reporte.

Escenario real `import-sii-enlase-read`: PASS, 2026-09-25. Confirmó contexto ENLASE, capacidad, consulta SII, descriptor y consumo GET; 7 controles de solo lectura quedaron sin cambios. Evidencia saneada disponible. El gate terminó en `false`, con usuarios y grupos vacíos.

- [x] e2e: `import-sii-enlase-read`; PASS, controles=7, sinCambios=SI, 2026-09-25.
