# CONTRATOS-MULTI-PROVEEDOR

- Ticket: DOC-50
- Cambio OpenSpec: doc-50-contratos-multi-proveedor
- Clasificacion: cross_cutting (Transversal)
## Evidencia requerida

- [x] unit: `node --test Tests/importar-servicio-web-contracts.test.cjs Tests/importar-servicio-web-provider-registry.test.cjs Tests/importar-servicio-web-context.test.cjs`; 11/11 PASS; 2026-09-06; suites y fixtures versionados en la rama DOC-50.
- [x] manual_qa: revisión estructural de rutas, enlaces, ausencia de duplicados y frontera legacy; 8 documentos + 7 diagramas, 0 enlaces rotos; 2026-09-06. Build MSBuild Debug PASS y OpenSpec estricto 41/41 PASS.

## QA/E2E WebForms

E2E autenticado, carga y activación de gates: **NO EJECUTADOS — requieren autorización explícita de ambiente y cuentas de prueba**. DOC-50 no cambia UI ni activa `WorkflowCentroTrabajoModernActive`.
