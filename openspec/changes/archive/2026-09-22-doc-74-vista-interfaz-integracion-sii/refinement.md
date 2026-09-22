<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-74-vista-interfaz-integracion-sii

## Fuente y alcance

- Ticket: `DOC-74` — VISTA-INTERFAZ-INTEGRACION-SII
- Cambio: `doc-74-vista-interfaz-integracion-sii`
- Perfil: JavaScript ES5 sobre ASP.NET WebForms/VB.
- Alcance: vista segura de un recurso SII aún no importado, aditiva al modal moderno.

## Contexto inspeccionado

- `js/workflow/importar-servicio-web/importar-servicio-web-api.js`: cliente ASMX único con `GetPreview`.
- `js/workflow/importar-servicio-web/importar-servicio-web-ui.js`: modal, consulta, foco y renderizado actuales.
- `js/workflow/importar-servicio-web/sii/importar-servicio-web-sii-list.js`: lista y selección SII.
- `webservice/WebServiceImportarServicioWebModern.asmx.vb`: crea el descriptor mediado.
- `workflow/ImportarServicioWebPreview.ashx.vb`: streaming, expiración y cabeceras defensivas.
- `Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiPreviewResponseFactory.vb`: política de seguridad.
- `workflow/Webworkflow.aspx`, `.vb`, CSS y `.vbproj`: superficie y registro de recursos.
- `Tests/importar-servicio-web-preview-*.test.cjs`: contrato y seguridad existentes.
- `workflow/ClassAlmacenamiento.vb`: fuera de alcance y protegido por invariancias.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia de código | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | Solicitar preview por proveedor e identidad externa; consumir solo el handler derivado del `DescriptorId`. La URL de fila nunca es autoridad. | `importar-servicio-web-api.js:create`; `WebServiceImportarServicioWebModern.asmx.vb:GetPreview` | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | Un estado independiente controla una solicitud por apertura y los estados de carga, éxito y error; foco o layout no repiten la descarga. | `importar-servicio-web-core.js`; `importar-servicio-web-preview-single-fetch.test.cjs` | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | Panel lateral en escritorio y subvista en ancho reducido, preservando selección, filtros, scroll y foco. | `Webworkflow.aspx#importar-servicio-web-modal`; `importar-servicio-web-ui.js:open/close` | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | Preview externo y documento importado son acciones distintas; el visor vigente solo recibe una identidad interna reconciliada y autorizada. | `MySqlImportReconciliationRepository.vb`; visor vigente | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | Si B10, el gate o el proveedor no están disponibles, mostrar estado bloqueado sin simulación, URL externa ni fuga de detalles. | `WebServiceImportarServicioWebModern.asmx.vb:FeatureEnabled`; `SiiPreviewResponseFactory.vb:Validate` | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | Limitar el cambio a scripts canónicos, integración aditiva, CSS existente, `.vbproj`, pruebas focales y documentación DOC-74. | `GestionDocumental-Docuarchi.net.vbproj`; rutas de Jira | D-06 | RQ-06 | Origen: D-06, RQ-06 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptación | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | Preview exclusivamente mediado. | WHEN se abre un recurso THEN se usa `GetPreview` y el handler seguro, nunca la URL externa de la fila. | Evita autoridad del cliente y exposición de tokens. |
| RQ-02 | Estados y expiración deterministas. | WHEN vence el descriptor THEN se solicita uno nuevo sin mutación; foco o layout no repiten la descarga. | Evita consumir dos veces un descriptor. |
| RQ-03 | Interacción accesible que conserva contexto. | WHEN se cierra o se usa `Volver a la lista` THEN se restauran selección, filtros, scroll y foco. | Preserva el modal existente. |
| RQ-04 | Recurso externo distinto del importado. | WHEN existe identidad interna autorizada THEN aparece `Ver documento importado` usando el visor vigente. | Evita confundir preview con almacenamiento. |
| RQ-05 | Dependencias ausentes fallan cerradas. | WHEN B10, gate o proveedor fallan THEN no hay URL externa, base64 ni simulación productiva. | Rollout bloqueado hasta confirmar B10. |
| RQ-06 | Código y evidencia en rutas aprobadas. | WHEN se revisa el diff THEN solo hay integración aditiva, pruebas focales y documentación DOC-74. | Evita un segundo cliente o visor. |

## Reglas de trazabilidad obligatorias

1. Cada `D-XX` aparece en design, spec y tareas con `Origen: D-XX, RQ-XX`.
2. No modificar almacenamiento, `AlmacenaDocumentoTareaWorkflow(...)`, `ClassAlmacenamiento` ni visores existentes.
3. E2E real o activación de gates exige autorización explícita y el runbook aplicable.
4. Registrar evidencia sin secretos, cookies ni datos sensibles.

## Resultado del refinamiento

- Estado: aprobado técnicamente contra código y Jira.
- Rollout: bloqueado hasta confirmar B10 y el mediador en el ambiente objetivo.
- Comando: `npm.cmd --prefix tools/opsxj run opsxj:refine -- DOC-74 --sync`.
