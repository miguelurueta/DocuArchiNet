# DOC-122 — IMPORTACION-EXCEL-02-AUTORIZACION-CONEXION-PLANTILLAS

- Ticket: DOC-122
- Cambio OpenSpec: doc-122-importacion-excel-02-autorizacion-conexion-plantillas
- Clasificacion: cross_cutting

## Evidencia requerida
Build MSBuild aislado en .tmp/doc122-project-build, DeployOnBuild=false y Disable_CopyWebApplication=true; no sobrescribe DLL del sitio ni Web.config. Suite nativa DOC122 usa el harness existente DOC110 y ejecuta su regresión. Fixtures sintéticos ADO.NET prueban rollback tras escritura parcial de campos, revisión concurrente, cambio legacy, metadatos, preservación de ruta/IDs/NULL, conexión DA del módulo y cambios de ámbito, autoridad viva, guardia CSRF y reflexión SOAP. Vector independiente SHA256 de snapshot v1 en tests/Fixtures/ProduccionDocumental/doc122-snapshot-v1.json. Evidencia inicial en Doc/Actualizacion/Importacion/Validacion/doc122-validacion-local-20261121.md/json; revisión vigente de conexión en doc122-conexion-reutilizada-20261121.md/json del mismo directorio. La evidencia inicial de configuración protegida es histórica, no un requisito actual.

## QA/E2E WebForms
La corrida real2026-11-22 pasó2/2 recorridos, sin omisiones/reintentos, con14 llamadas ASMX y23 controles SELECT. Creó una plantilla propia inactiva y dos mapeos mediante el API, sin tabla nueva ni filas de negocio. Usuario autorizado, rechazo CSRF, cuenta restringida y un ganador/un conflicto por revisión acreditados. EGRESOS, CONTRATOS y demás cabeceras/campos conservaron sus huellas. Evidencia vigente: Doc/Actualizacion/Importacion/Validacion/doc122-e2e-real-20261122.md/json.

El runner DOC110 existente añade --excel-read/--excel-write y reutiliza su perfil local DOC88 y cuentas efímeras DOC110. No exige configurar URL/módulo otra vez ni crear infraestructura de ejecución. Seis pruebas locales en tests/doc122-e2e-reuse.test.cjs verifican la adaptación y los rechazos de alcance antes de autenticar. El permiso o una autorización anterior de DOC110 no habilitan DOC122 automáticamente.

Dos recorridos preparados en tools/e2e/tests/importacion-excel.spec.cjs sobre autenticación y safePost existentes. Descubrimiento local no inicia navegador ni sesiones. El despliegue y el enrutamiento DA quedaron acreditados para estas operaciones; no se acreditan mínimo privilegio ni DDL de fases futuras. La prueba de concurrencia HTTP usa una sesión ASP.NET y no demuestra dos escritores MySQL paralelos. manual_qa de este alcance queda aprobada; DOC12214/14 tareas completas. El archivo, merge o cambio Jira no forman parte de esta ejecución.
