<!-- opsxj:refinement version=1 state=approved -->
<!-- opsxj:refinement-traceability version=1 artifact=refinement decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08 -->

# Refinamiento - doc-101-arranque-carga-diferida

## Fuente y alcance

- Ticket: `DOC-101` — ARRANQUE-CARGA-DIFERIDA
- Cambio OpenSpec: `doc-101-arranque-carga-diferida`
- Fuente Jira: `specs/arranque-carga-diferida/jira-context.md`
- Perfil tecnológico: JavaScript legacy dentro de un visor PDF embebido en WebForms, con PDF.js 3.4.120, jQuery, Bootstrap y recursos estáticos declarados en el proyecto VB.NET.

El alcance reduce el JavaScript crítico de la primera apertura. No cambia el transporte del PDF, los WebMethods, la persistencia de grafos, los permisos ni las firmas públicas consumidas por la página padre.

## Contexto inspeccionado

- `pdfjs/pdf_anotate_view/index.html`: orden de recursos y duplicados de jQuery UI, Popper, PDF.js, Font Awesome, Prettify y Wacom.
- `pdfjs/pdf_anotate_view/script.js`: arranque en `window.load`, `load_pdf_viwer_anotation`, enlace de botones y llamada anticipada a `pdfViewer.config_stamp()`.
- `pdfjs/pdf_anotate_view/pdf_viewer_anotate.js`: `pdfjsLib.getDocument`, `_Add_captureFromSTU`, `_PrinterPdf`, `load_config_stamp_graf`, `config_stamp` y servicios WebForms.
- `pdfjs/pdf_anotate_view/extend_fabric.js` y `pdfjs/wacom/*.js`: consumidores y orden de símbolos opcionales.
- `GestionDocumental-Docuarchi.net.vbproj`: recursos locales ya desplegados para visor, Wacom e impresión.
- `tools/e2e/scripts/run-workflow-pdf-viewer-performance-interactive.cjs` y pruebas asociadas: métricas, clasificación saneada y línea base reutilizable.
- Evidencia DOC-100: `domInteractive` inicial de 1.679 ms frente a 92–98 ms en recargas y error repetible en `pdf.worker.entry.min.js`.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia de código | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | El camino crítico conservará solo los recursos necesarios para construir el visor, obtener el PDF y pintar el primer canvas. | `index.html`; `script.js:load_pdf_viwer_anotation`; `PDFjsViewer.loadDocument` | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | PDF.js cargará `pdf.min.js` en el frame y resolverá el worker mediante `GlobalWorkerOptions.workerSrc`; worker, sandbox y worker-entry no se ejecutarán como scripts normales. | `index.html`; `pdf_viewer_anotate.js:pdfjsLib.getDocument` | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | Cada familia tendrá un propietario único: jQuery UI local 1.12.1, Popper incluido por Bootstrap bundle, un único runtime Font Awesome, un Prettify y cero cargas sin consumidor. | `index.html` líneas de jQuery UI, Popper, Font Awesome, jsPDF y Prettify | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | `script.js` administrará un registro de capacidades opcionales con promesa memoizada, carga ordenada y máximo una inserción por recurso. | Botones de Wacom, impresión, firma y grafo en `configToolbarPdfView` | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | Wacom, impresión, Fabric/Prettify y complementos de grafo se solicitarán únicamente desde la acción que los consume; la configuración de sello no se consultará durante la visualización básica. | `pdfTool_viwer_captureFromSTU`, `pdfTool_viwer_Printer`, `showPdfData`, `pdfTool_parameter_grafo`, `pdfViewer.config_stamp()` | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | El fallo de una capacidad opcional quedará localizado, será comprensible para el usuario y permitirá reintento sin invalidar el PDF ya renderizado. | `PdfViwerAlert`; handlers de toolbar; clasificador E2E DOC-100 | D-06 | RQ-06 | Origen: D-06, RQ-06 |
| D-07 | No se introducirán versiones mayores, bundlers ni recursos remotos nuevos; se conservarán orden, SRI/crossorigin existentes y registro `.vbproj` cuando corresponda. | `index.html`; `GestionDocumental-Docuarchi.net.vbproj` | D-07 | RQ-07 | Origen: D-07, RQ-07 |
| D-08 | La aceptación comparará solicitudes, errores, `domInteractive` y primer canvas contra la línea base mediante el runner saneado existente. | Runner y exploración del visor; línea base 1.679 ms y 92–98 ms | D-08 | RQ-08 | Origen: D-08, RQ-08 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptación | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | La primera página se visualiza sin descargar capacidades opcionales. | Al abrir solo para lectura, aparece el primer canvas y no hay solicitudes Wacom, Fabric, Prettify ni impresión. | Mantener jQuery, PDF.js, toolbar y contratos WebForms necesarios para el núcleo. |
| RQ-02 | PDF.js usa un worker dedicado sin ejecutar artefactos de worker como scripts del frame. | `pdf.worker.entry.min.js` deja de generar `pageerror` y el render sigue usando worker. | Rollback: restaurar configuración previa de worker sin tocar el planificador DOC-100. |
| RQ-03 | No existen cargas duplicadas conocidas y cada familia conserva una versión compatible. | El contrato estático detecta exactamente un propietario por jQuery UI, Popper, Font Awesome, Prettify y Wacom. | Validar draggable, resizable, dropdown e iconos antes de retirar propietarios. |
| RQ-04 | Una capacidad concurrentemente solicitada se carga una vez y respeta el orden declarado. | Dos clics simultáneos comparten promesa y no duplican nodos `script`/`link`. | No exponer estados intermedios ni símbolos incompletos. |
| RQ-05 | Firma personal, Wacom, impresión, grafo y descarga conservan su comportamiento al activarse. | La matriz funcional ejecuta cada acción con sus recursos ya disponibles. | No cambiar permisos, payloads, WebMethods ni persistencia. |
| RQ-06 | Una dependencia opcional ausente no bloquea lectura, scroll, zoom o descarga básica. | Al simular fallo de una capacidad, el visor permanece operativo y solo esa acción informa el error. | Evitar `catch` vacíos y errores globales no clasificados. |
| RQ-07 | La entrega mantiene compatibilidad y política de recursos del sistema legacy. | No hay CDN nuevos ni upgrades mayores y cada recurso local nuevo aparece en `.vbproj`. | Bootstrap JS 5.3.2/CSS 5.1.3 queda documentado, no actualizado dentro de DOC-101. |
| RQ-08 | La mejora y ausencia de regresiones quedan respaldadas con evidencia reproducible. | E2E autorizada registra métricas saneadas, solicitudes y errores frente a la línea base. | Sin autorización se registra bloqueo y no se inventa evidencia. |

## Reglas de trazabilidad obligatorias

1. Cada decisión `D-XX` está desarrollada en `design.md`, reflejada en un requirement de `spec.md` y vinculada a tareas mediante `Origen: D-XX, RQ-XX`.
2. Cada tarea tiene un solo entregable principal, un área concreta y una verificación observable.
3. Las pruebas autenticadas requieren autorización vigente y lectura previa de `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`.
4. No se guardan credenciales, cookies, URLs sensibles, nombres documentales, mensajes libres ni stacks en evidencia.

## Resultado del refinamiento

- Estado: aprobado para plan de implementación.
- Cobertura: D-01..D-08 y RQ-01..RQ-08 trazados en design, spec y tasks.
- Siguiente fase: ejecutar las tareas sin ampliar el alcance hacia transporte, persistencia o actualización mayor de librerías.
