## 1. Inventario

- [x] 1.1 Ejecutar `rg -n "AppEditorPdf" src` y listar archivos afectados en el PR (o en un comentario corto del ticket)
- [x] 1.2 Identificar consumidores reales en `src/modules/**` y en `src/modules/gestionCorrespondencia/components/documentosWorkbench/`

## 2. Eliminar componente y exports

- [x] 2.1 Eliminar `src/app/Components/UI/AppEditorPdf/` completo
- [x] 2.2 Remover export en `src/app/Components/UI/index.ts`
- [x] 2.3 Remover re-exports adicionales (si existen) de `AppEditorPdf*`

## 3. Limpiar consumidores

- [x] 3.1 Eliminar imports/usos de `AppEditorPdf`, `AppEditorPdfSaveAction`, `useAppEditorPdfDirtyState`, `useAppEditorPdfSaveState`, tipos asociados
- [x] 3.2 Limpiar `src/modules/gestionCorrespondencia/components/documentosWorkbench/` para que no quede UI rota ni paneles vacíos
- [x] 3.3 Remover CSS/props/handlers/estado muerto relacionado al editor

## 4. Tests y validación

- [x] 4.1 Eliminar/ajustar tests bajo `src/app/Components/UI/AppEditorPdf/` (y otros que fallen)
- [x] 4.2 Validar `rg -n "AppEditorPdf" src` retorna 0 matches
- [ ] 4.3 Validar `npm.cmd test` pasa
- [ ] 4.4 (Si aplica) Validar `npm.cmd run build` pasa
