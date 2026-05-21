# SCRUMCORE-216 - Metadata

## Ticket
- `SCRUMCORE-216`

## Autor
- `gerencia@contasoftcompany.com`

## Fecha
- 2026-05-21

## Resumen tecnico
`AppTreeTable` se refactoriza para usar `AppTable` internamente (wrapper/adaptador), preservando compatibilidad de consumidores actuales y manteniendo expansion/indentacion y estados legacy.

## Entregables
- Change OpenSpec: `openspec/changes/scrumcore-216-actualizacion-componente-apptreetable/`
- Documentacion: `docs/Components/AppTreeTable/`

## Evidencia de tests
- `npm test -- --run src/app/Components/UI/AppTreeTable/AppTreeTable.test.tsx`
- `npm test -- --run src/modules/gestionCorrespondencia/tests/DocumentosWorkbench.test.tsx`

## Incidencias y correcciones
### Error: orden de Hooks (runtime)
- Sintoma: React lanza `React has detected a change in the order of Hooks called by AppTreeTable` y `Rendered more hooks than during the previous render`.
- Causa raiz: `AppTreeTable` definia `const tableColumns = useMemo(...)` despues de returns condicionales (`loading/error/empty`). Al cambiar el estado entre renders, el componente ejecutaba un numero diferente de hooks.
- Correccion: mover el `useMemo` de `tableColumns` antes de cualquier `return` condicional para mantener el orden de hooks estable.
- Referencia: `src/app/Components/UI/AppTreeTable/AppTreeTable.tsx`.

## Archivos relevantes
- `src/app/Components/UI/AppTreeTable/AppTreeTable.tsx`
- `src/app/Components/UI/AppTreeTable/adapters/*`
- `src/app/Components/UI/AppTreeTable/hooks/*`

## Riesgos residuales
- Validar visualmente en navegador el comportamiento de expand/collapse en escenarios reales con AG Grid.
