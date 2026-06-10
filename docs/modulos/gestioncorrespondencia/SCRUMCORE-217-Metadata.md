# SCRUMCORE-217 - Metadata

## Ticket
- `SCRUMCORE-217`

## Autor
- `gerencia@contasoftcompany.com`

## Fecha
- 2026-05-21

## Resumen técnico

Integración backend-driven de `DocumentosWorkbench` usando `AppTreeTable` (wrapper sobre `AppTable`) para consumir el contrato `SCRUM-205 ListaDocumentosRadicados`, ejecutar acción principal `ver_documento` y actualizar el visor PDF preservando layout visor + rail.

## Control de cambios (archivos principales)

- `src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.tsx`
- `src/modules/gestionCorrespondencia/hooks/useGestionRespuestaDocumentosTable.ts`
- `src/modules/gestionCorrespondencia/services/listaDocumentosRadicados.service.ts`
- `src/modules/gestionCorrespondencia/adapters/gestionRespuestaDocumentosRequestMapper.ts`
- `src/modules/gestionCorrespondencia/adapters/documentosWorkbenchResponseAdapter.ts`
- `src/modules/gestionCorrespondencia/adapters/documentosWorkbenchActionMapper.ts`
- `src/app/Components/UI/AppTreeTable/types.ts`
- `src/app/Components/UI/AppTreeTable/AppTreeTable.tsx`

## Specs

- OpenSpec change:
  - `openspec/changes/scrumcore-217-integracion-backend-driven-documentos-workbench-apptreetable-wrapper/`
- Spec:
  - `openspec/changes/scrumcore-217-integracion-backend-driven-documentos-workbench-apptreetable-wrapper/specs/integracion-backend-driven-documentos-workbench-apptreetable-wrapper/spec.md`

## Tests

Unit:
- `src/modules/gestionCorrespondencia/adapters/gestionRespuestaDocumentosRequestMapper.test.ts`
- `src/modules/gestionCorrespondencia/adapters/documentosWorkbenchResponseAdapter.test.ts`
- `src/modules/gestionCorrespondencia/adapters/documentosWorkbenchActionMapper.test.ts`

Integración:
- `src/modules/gestionCorrespondencia/tests/DocumentosWorkbench.test.tsx`

## Evidencia de ejecución

Pendiente de registrar salida real de CI/local (ver `SCRUMCORE-217-Pruebas.md`).

## Riesgos residuales

- La respuesta real de `resolveDocumentoVisualizacion` debe exponer `fileUrl` (o mapping equivalente); validar en ambiente real.
- Acciones dinámicas (menú secundario) requieren completar mapping cuando se confirme el catálogo final del backend.
