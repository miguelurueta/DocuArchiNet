# SCRUMCORE-195 — Eliminación de `AppEditorPdf`

## Goal
Eliminar completamente `AppEditorPdf` de la capa UI shared y remover su uso en consumidores, dejando el proyecto con build/tests verdes y sin referencias colgantes.

## Scope

### In-scope
- Eliminar `src/app/Components/UI/AppEditorPdf/` (código, styles, tests, README).
- Eliminar exports del componente (barrels), incluyendo `src/app/Components/UI/index.ts`.
- Eliminar o refactorizar consumidores que importen/usen `AppEditorPdf` o helpers asociados.
- Limpieza explícita en `src/modules/gestionCorrespondencia/components/documentosWorkbench/` (si hay integración real).
- Ajustar UI para quedar estable sin paneles vacíos.

### Out-of-scope
- Migrar funcionalidades de `AppEditorPdf` a otro componente.
- Rediseñar UX más allá de remover el editor y limpiar el layout.

## Strategy
1. Hacer inventario de referencias con búsqueda global (rg) y listar archivos afectados.
2. Remover componente y exports.
3. Limpiar consumidores (incluyendo workbench) y eliminar código muerto.
4. Ajustar/retirar tests asociados.
5. Validar: `rg` sin matches, tests y build.

