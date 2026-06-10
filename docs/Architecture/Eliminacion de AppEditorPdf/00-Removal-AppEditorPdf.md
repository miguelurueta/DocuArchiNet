# Eliminaci\u00f3n de `AppEditorPdf` (cleanup arquitect\u00f3nico)

## Objetivo
Eliminar completamente el componente `AppEditorPdf` de la capa UI compartida y remover cualquier integraci\u00f3n/uso visual asociada en el proyecto, dejando la interfaz limpia y el build/tests verdes.

## Alcance

### In-scope
- Eliminar la carpeta del componente:
  - `src/app/Components/UI/AppEditorPdf/`
- Eliminar exportaciones p\u00fablicas del componente:
  - `src/app/Components/UI/index.ts` (y cualquier barrel adicional que lo exporte)
- Eliminar referencias de consumidores:
  - Buscar y remover imports/usos de `AppEditorPdf`, `AppEditorPdfSaveAction`, `useAppEditorPdfDirtyState`, etc.
- Limpiar implementaci\u00f3n visual en:
  - `src/modules/gestionCorrespondencia/components/documentosWorkbench/`
  - (y cualquier otro m\u00f3dulo que tenga uso real de `AppEditorPdf`)
- Ajustar UI para quedar funcional sin el componente (sin dejar placeholders rotos).
- Eliminar/actualizar tests que dependan de `AppEditorPdf`.

### Out-of-scope
- Migrar funcionalidad de `AppEditorPdf` a otro componente (solo eliminar).
- Redise\u00f1ar UX m\u00e1s all\u00e1 de remover el editor donde estaba incrustado.

## Estrategia

1. **Inventario de referencias**
   - Buscar en `src/` ocurrencias:
     - `AppEditorPdf`
     - `AppEditorPdfSaveAction`
     - `useAppEditorPdfDirtyState`
     - `useAppEditorPdfSaveState`
     - `editor-pdf.types`
   - Registrar archivos y decidir para cada uno: eliminar uso, reemplazar por otro viewer (si ya existe), o remover secci\u00f3n UI.

2. **Remoci\u00f3n del componente**
   - Borrar `src/app/Components/UI/AppEditorPdf/`.
   - Eliminar su export en `src/app/Components/UI/index.ts`.

3. **Consumidores**
   - En `src/modules/gestionCorrespondencia/components/documentosWorkbench/`:
     - Remover imports/usos relacionados a `AppEditorPdf`.
     - Eliminar estados/handlers/props que solo exist\u00edan para el editor.
     - Ajustar layout para que la vista quede limpia (sin paneles vac\u00edos).

4. **Tests y build**
   - Quitar o actualizar tests del componente eliminado.
   - Correr pruebas relevantes (al menos `npm.cmd test` o subset relacionado).

## Criterios de aceptaci\u00f3n
- No existe la ruta `src/app/Components/UI/AppEditorPdf/` en el repo.
- No hay referencias a `AppEditorPdf*` en `src/` (b\u00fasqueda global retorna 0 matches).
- `src/modules/gestionCorrespondencia/components/documentosWorkbench/` no contiene imports/uso de `AppEditorPdf` y la UI queda estable.
- `npm.cmd test` pasa (o suite relevante seg\u00fan convenciones del repo).
- `npm.cmd run build` (si aplica) no falla por imports rotos.

## Checklist t\u00e9cnico sugerido (para el ticket)
- [ ] Buscar referencias globales y listarlas en el ticket
- [ ] Eliminar exports barrel
- [ ] Eliminar componente y tests
- [ ] Limpiar `documentosWorkbench` (UI + CSS)
- [ ] Verificar build/tests

