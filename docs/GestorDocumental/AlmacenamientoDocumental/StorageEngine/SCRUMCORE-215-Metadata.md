# SCRUMCORE-215 - Metadata (Enterprise)

## Resumen
Evoluciona `AppTreeTable` para soportar renderización e integración backend-driven (contrato tipo SCRUM-205 `ListaDocumentosRadicados`) y habilita su consumo en el rail de “Listado” dentro de `DocumentosWorkbench`, sin afectar `AppVisorEmbedPdf` ni otros componentes/plugins.

Además integra la resolución de `NombreGabinete` por **tarea workflow** para evitar errores `400 validation` al consumir `ListaDocumentosRadicados/query` cuando el backend exige `NombreGabinete` no vacío.

## Ajuste UI (alto/scroll)
Se corrigiÃƒÂ³ un recorte de altura en el visor + listado causado por el wrapper `workbenchBody`:
- Se eliminÃƒÂ³ el `<div class="workbenchBody">` extra en `DocumentosWorkbench` y el layout se moviÃƒÂ³ al `<section>` para evitar un contenedor que limitaba el alto disponible.
- Se actualizÃƒÂ³ el CSS para que el workbench use `height: 100%` + `min-height: 0` (sin `clamp()`), y el rail/listado queden a `height: 100%`, manteniendo scroll horizontal/vertical del listado.
- Archivos: `src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.tsx`, `src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.module.css`.

## Rama
- `feature/SCRUMCORE-215`

## Commits relevantes
- `892d14f` - Alineación del OpenSpec con contrato SCRUM-205 (orden de columnas).

## Contratos backend consumidos
- `POST /api/GestorDocumental/Documentos/ListaDocumentosRadicados/query`
- `POST /api/GestorDocumental/Documentos/ListaDocumentosRadicados/action`
- `POST /api/gestor-documental/documentos/visualizacion/resolve`
- `GET /api/workflow/ruta-trabajo/tareas/{idTareaWorkflow}/gabinete` (resuelve `NombreGabinete` para query/action según reglas de backend).

## Tests ejecutados (Vitest)
- `npm test -- --run src/app/Components/UI/AppTreeTable/AppTreeTable.test.tsx`
- `npm test -- --run src/modules/gestionCorrespondencia/tests/DocumentosWorkbench.test.tsx`

## Artefactos OpenSpec
- `openspec/changes/scrumcore-215-renderizacion-integracion-backend-apptreetable/proposal.md`
- `openspec/changes/scrumcore-215-renderizacion-integracion-backend-apptreetable/design.md`
- `openspec/changes/scrumcore-215-renderizacion-integracion-backend-apptreetable/specs/renderizacion-integracion-backend-apptreetable/spec.md`
- `openspec/changes/scrumcore-215-renderizacion-integracion-backend-apptreetable/tasks.md`
