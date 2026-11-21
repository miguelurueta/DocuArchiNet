# IMPORTACION-EXCEL-01-ESQUEMA-CONTRATOS-CONFIGURACION

- Ticket: DOC-121
- Cambio OpenSpec: doc-121-importacion-excel-01-esquema-contratos-configuracion
- Clasificacion: cross_cutting (Transversal)
## Evidencia requerida

Estado vigente de alcance: fase 01 contractual completa, 16 tareas, 8 requisitos y 24 escenarios documentados. El usuario aclaró «egresos solo es de pruebas»; las incidencias de esa tabla no bloquean esta entrega y no se propone modificarla. La cuenta dedicada tiene contrato/plantilla entregados, con comprobación efectiva reservada a la fase de conexión. No se confunde cobertura documental de escenarios con ejecución de 24 recorridos reales. Los cortes inferiores mantienen su evidencia histórica y sus límites.

Aplicación autorizada de 08, 2026-11-21: usuario «aplica por mi», excepción posterior y acotada al índice de CONTRATOS. `Doc/Actualizacion/Importacion/Validacion/doc121-contratos-indice-aplicado-20261121.md`/JSON registra un ALTER reconocido, 15 SELECT, un SET SESSION estricto y cero DML/HTTP. NCONTRATO NOT NULL y UNIQUE completo verificados; 625 filas y huella de las cuatro columnas preservadas. Los otros campos conservan su definición. EGRESOS y concesiones 05 no se modifican ni se acreditan. Los cortes inferiores son históricos.

Continuación de claves 2026-11-21: `Doc/Actualizacion/Importacion/Validacion/doc121-preparacion-indices-20261121.md`/JSON conserva 11 SELECT y agregados saneados. CONTRATOS: 625 filas y cero incidencias de clave, con ALTER 08 preparado para ejecución manual. EGRESOS: 25153 filas, 2 claves vacías, 738 grupos duplicados y 4479 filas excedentes; no se prepara ALTER ejecutable para su clave. Tres archivos nuevos 07–09 pasan 12 controles estáticos; los siete originales se preservan. Cuenta/host/DA del paso 05 y destino efectivo de fuentes requieren confirmación del usuario. No hay DDL/DML, limpieza o permisos aplicados por el agente.

Corte de instalación 2026-11-21: el usuario informa «achivos aplicados». `Doc/Actualizacion/Importacion/Validacion/doc121-verificacion-instalacion-20261121.md`/JSON registra 8 SELECT y 37 comprobaciones aprobadas del esquema 01–03 en la base física local de referencia, sin DDL/DML/HTTP. Índices de fuente todavía no acreditados en esa base y cuenta dedicada sin identificar. La correspondencia autenticada RA/DA y el alcance exacto aplicado siguen por precisar; no se transforma la comprobación parcial en aceptación completa. Los párrafos siguientes conservan el corte contractual del 20/11.

Evidencia local de fase 01 en Doc/Actualizacion/Importacion/Validacion/doc121-revision-fase01-20261120.md/JSON. Auditor reproducible de SQL y contratos; OpenSpec estricto y refinement. Suites existentes de gobierno: npm.cmd --prefix tools/opsxj test -- scripts/lib/refinementService.test.js scripts/lib/legacyGovernanceService.test.js, 19/19 aprobadas. Son pruebas del tooling reutilizado, no del escritor Excel aún inexistente.

La revisión manual técnica comprende correspondencia de scripts con destinos RA/DA, tipos MySQL 5.1, claves, nulabilidad inicial y contrato de recuperación. Se vincula la evidencia al SHA comprometido en registros locales OPSXJ. La instalación sigue PENDIENTE_EJECUCION_MANUAL; no se ejecutó SQL ni se consultaron datos reales.

## QA/E2E WebForms

E2E no aplica a la fase sin endpoint o interfaz ejecutable, conforme al prompt 01. Build de producto tampoco aplica sin cambios compilables. Las fases posteriores reutilizan tools/e2e, autorización, datos descartables y controles SELECT; los tests no instalan o reparan bases. La fase 07 acredita XLS/XLSX, 1000/5000/10000 filas, fronteras, rollback, concurrencia, acuse perdido, CREATE y regresión SII/carpetas. Ningún resultado local de DOC-121 acredita esos recorridos.
