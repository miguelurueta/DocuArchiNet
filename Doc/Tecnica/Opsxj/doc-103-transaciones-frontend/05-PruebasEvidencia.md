# TRANSACIONES-FRONTEND

- Ticket: DOC-103
- Cambio OpenSpec: doc-103-transaciones-frontend
- Clasificacion: cross_cutting (Transversal)
## Evidencia requerida

## Evidencia ejecutada — 2026-10-14

- `node --check pdfjs/pdf_anotate_view/pdf_viewer_anotate.js`: PASS.
- Suite focal DOC-103: 17/17 PASS.
- Regresión visor DOC-100–DOC-102 más DOC-103: 55/55 PASS.
- `npm.cmd --prefix tools/e2e run test:doc103:graph-save -- --mode double`: PASS; sin login, red ni escritura PDF.
- E2E real mutadora autorizada: PASS; código 0, un POST, resultado normalizado `committed`, efecto único y control `SELECT` antes/después completado sobre `workflowdocument`.
- Evidencia saneada ignorada: `tools/e2e/artifacts/doc103-graph-save.json`.

La matriz cubre éxito, advertencia, rechazo funcional, 401, 403, 404, 500, parser, timeout, aborto, doble clic, error de imagen y fallo de refresco. Verifica un POST máximo, no retry, overlay, mensajes saneados y navegación estable.

## QA/E2E WebForms

## Resultado autorizado

La E2E real se ejecutó el 2026-10-14 después de recibir autorización específica para ambiente, cuenta y PDF descartable. Terminó con código 0 y registró `postCount=1`, `normalizedResult=committed`, retiro del overlay, efecto único y preservación de página activa, zoom y scroll. El control ODBC `SELECT` antes/después sobre `workflowdocument` se completó sin consultar el origen documental ni su auditoría. Comando ejecutado:

```powershell
npm.cmd --prefix tools/e2e run test:doc103:graph-save -- --mode real --profile profiles/<perfil-runtime.json>
```

MSBuild no aplica mientras el diff permanezca limitado a JavaScript, Node, OpenSpec y documentación; no se modificó código compilado ni contrato ASMX.
