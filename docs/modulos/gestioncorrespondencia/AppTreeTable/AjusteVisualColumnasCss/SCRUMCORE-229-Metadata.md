# SCRUMCORE-229 — Metadata

- Ticket: `SCRUMCORE-229`
- Módulo: Gestión Correspondencia / Documentos Workbench
- Autor: (completar)
- Fecha: 2026-05-22
- Tipo: Ajuste visual + UX de selección (Workbench) + tooltip headers

## Resumen del cambio
- Visual refresh enterprise del `AppTreeTable` (AG Grid Quartz) **scopeado** al Workbench.
- Sin separadores verticales de columnas; solo separador horizontal por fila.
- Header más “título” (uppercase/weight) y con tooltips.
- Click en fila selecciona la fila completa (estado `aria-selected="true"`).
- Botón colapsar junto al contador “Documentos (N)” y mayor área útil para el listado.

## Archivos relevantes
- `src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.module.css`
- `src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.tsx`
- `src/app/Components/UI/AppTreeTable/AppTreeTable.tsx`

## Referencias cruzadas
- `docs/modulos/gestioncorrespondencia/AppTreeTable/AjusteVisualColumnasCss/SCRUMCORE-229-Arquitectura.md`
- `docs/modulos/gestioncorrespondencia/AppTreeTable/AjusteVisualColumnasCss/SCRUMCORE-229-Implementacion-Detallada.md`
- `docs/modulos/gestioncorrespondencia/AppTreeTable/AjusteVisualColumnasCss/SCRUMCORE-229-Pruebas.md`

## Historial de cambios
- 2026-05-22: Creación de documentación y consolidación de cambios visuales/UX.

## PR / Commit
- PR: (completar)
- Commit: (completar)

