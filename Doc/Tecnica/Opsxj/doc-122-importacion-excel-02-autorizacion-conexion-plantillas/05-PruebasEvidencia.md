# DOC-122 — IMPORTACION-EXCEL-02-AUTORIZACION-CONEXION-PLANTILLAS

- Ticket: DOC-122
- Cambio OpenSpec: doc-122-importacion-excel-02-autorizacion-conexion-plantillas
- Clasificacion: cross_cutting

## Evidencia requerida
Build MSBuild aislado en .tmp/doc122-project-build, DeployOnBuild=false y Disable_CopyWebApplication=true; no sobrescribe DLL del sitio ni Web.config. Suite nativa DOC122 usa el harness existente DOC110 y ejecuta su regresión. Fixtures sintéticos ADO.NET prueban rollback tras escritura parcial de campos, revisión concurrente, cambio legacy, metadatos, preservación de ruta/IDs/NULL, configuración protegida, autoridad viva, guardia CSRF y reflexión SOAP. Vector independiente SHA256 de snapshot v1 en tests/Fixtures/ProduccionDocumental/doc122-snapshot-v1.json. Informe saneado en Doc/Actualizacion/Importacion/Validacion/doc122-validacion-local-20261121.md/json.

## QA/E2E WebForms
El runner DOC110 existente añade --excel-read/--excel-write y reutiliza su perfil local DOC88 y cuentas efímeras DOC110. No exige configurar URL/módulo otra vez ni crear infraestructura de ejecución. Seis pruebas locales en tests/doc122-e2e-reuse.test.cjs verifican la adaptación y los rechazos de alcance antes de autenticar. El permiso o una autorización anterior de DOC110 no habilitan DOC122 automáticamente.

Dos recorridos preparados en tools/e2e/tests/importacion-excel.spec.cjs sobre autenticación y safePost existentes. Descubrimiento local no inicia navegador ni sesiones. Ejecución real, permisos efectivos, enrutamiento RA/DA y despliegue siguen sin acreditar: requieren cuenta dedicada configurada y autorización de ambiente/cuentas/plantilla descartable. La prueba de concurrencia HTTP usa una sesión ASP.NET y no demuestra dos escritores MySQL paralelos. No se registra manual_qa como aprobada ni se cierra DOC122.
