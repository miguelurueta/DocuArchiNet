# IMPORTACION-INTERFAZ-INTEGRACION-SII

- Ticket: DOC-75
- Cambio OpenSpec: doc-75-importacion-interfaz-integracion-sii
- Clasificacion: cross_cutting (Transversal)
## Evidencia requerida

- [x] unit: 2026-09-22; `node --test` focal/regresión PASS 46/46; MSBuild Debug PASS; OpenSpec estricto PASS. Referencia: paquete canónico `05-PruebasEvidencia.md`.
- [x] manual_qa: 2026-09-22; revisión estática del popup, foco, catálogo/plan autoritativos, gate y ausencia de ejecución/persistencia; PASS local no autenticado.

## QA/E2E WebForms

La E2E real aplica al flujo completo, pero no se ejecutó sin autorización explícita para DOC-75. Una corrida futura debe usar recursos controlados, confirmar B11 y restaurar gate/proveedor en `finally`.
