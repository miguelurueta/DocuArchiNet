# SCRUMCORE-215 - Metadata (Enterprise)

## Resumen
Evoluciona `AppTreeTable` para soportar renderización e integración backend-driven (contrato tipo SCRUM-205 `ListaDocumentosRadicados`) y habilita su consumo en el rail de “Listado” dentro de `DocumentosWorkbench`, sin afectar `AppVisorEmbedPdf` ni otros componentes/plugins.

## Rama
- `feature/SCRUMCORE-215`

## Commits relevantes
- `892d14f` - Alineación del OpenSpec con contrato SCRUM-205 (orden de columnas).

## Tests ejecutados (Vitest)
- `npm test -- --run src/app/Components/UI/AppTreeTable/AppTreeTable.test.tsx`
- `npm test -- --run src/modules/gestionCorrespondencia/tests/DocumentosWorkbench.test.tsx`

## Artefactos OpenSpec
- `openspec/changes/scrumcore-215-renderizacion-integracion-backend-apptreetable/proposal.md`
- `openspec/changes/scrumcore-215-renderizacion-integracion-backend-apptreetable/design.md`
- `openspec/changes/scrumcore-215-renderizacion-integracion-backend-apptreetable/specs/renderizacion-integracion-backend-apptreetable/spec.md`
- `openspec/changes/scrumcore-215-renderizacion-integracion-backend-apptreetable/tasks.md`

