# IMPORTACION-EXCEL-01-ESQUEMA-CONTRATOS-CONFIGURACION

- Ticket: DOC-121
- Cambio OpenSpec: doc-121-importacion-excel-01-esquema-contratos-configuracion
- Clasificacion: cross_cutting (Transversal)
## Evidencia requerida

Evidencia local de fase 01 en Doc/Actualizacion/Importacion/Validacion/doc121-revision-fase01-20261120.md/JSON. Auditor reproducible de SQL y contratos; OpenSpec estricto y refinement. Suites existentes de gobierno: npm.cmd --prefix tools/opsxj test -- scripts/lib/refinementService.test.js scripts/lib/legacyGovernanceService.test.js, 19/19 aprobadas. Son pruebas del tooling reutilizado, no del escritor Excel aún inexistente.

La revisión manual técnica comprende correspondencia de scripts con destinos RA/DA, tipos MySQL 5.1, claves, nulabilidad inicial y contrato de recuperación. Se vincula la evidencia al SHA comprometido en registros locales OPSXJ. La instalación sigue PENDIENTE_EJECUCION_MANUAL; no se ejecutó SQL ni se consultaron datos reales.

## QA/E2E WebForms

E2E no aplica a la fase sin endpoint o interfaz ejecutable, conforme al prompt 01. Build de producto tampoco aplica sin cambios compilables. Las fases posteriores reutilizan tools/e2e, autorización, datos descartables y controles SELECT; los tests no instalan o reparan bases. La fase 07 acredita XLS/XLSX, 1000/5000/10000 filas, fronteras, rollback, concurrencia, acuse perdido, CREATE y regresión SII/carpetas. Ningún resultado local de DOC-121 acredita esos recorridos.
