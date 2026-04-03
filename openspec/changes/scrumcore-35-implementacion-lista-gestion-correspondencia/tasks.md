## 1. Reusable AppTable integration adapters

- [x] 1.1 Crear `src/app/Components/UI/AppTable/adapters/appGridToAppTableColumns.ts` para convertir `AppGridColumn[]` a `ColDef<T>[]`
- [x] 1.2 Crear `src/app/Components/UI/AppTable/adapters/appGridToAppTableRows.ts` para convertir `AppGridRow[]` a filas planas compatibles con `AppTable`
- [x] 1.3 Definir el tratamiento de columnas de acción sin romper el contrato visual actual de `AppTable`

## 2. GestionCorrespondencia module wiring

- [x] 2.1 Crear `src/modules/gestionCorrespondencia/adapters/gestionCorrespondenciaTableRequestMapper.ts` para mapear el input de pantalla al request real de `workflowInboxgestion`
- [x] 2.2 Crear `src/modules/gestionCorrespondencia/hooks/useGestionCorrespondenciaTable.ts` componiendo query dinámica + adapters finales a `AppTable`
- [x] 2.3 Refactorizar `src/modules/gestionCorrespondencia/pages/GestionCorrespondencia.tsx` para eliminar mocks y consumir props/estado integrados

## 3. Route-level loading, error and success handling

- [x] 3.1 Crear `src/modules/gestionCorrespondencia/pages/GestionCorrespondenciaRoutePage.tsx` como wrapper de carga de pantalla
- [x] 3.2 Crear `src/modules/gestionCorrespondencia/components/GestionCorrespondenciaTableSkeleton.tsx` reutilizando el patrón de `Skeleton` usado por la aplicación
- [x] 3.3 Integrar el wrapper de ruta sin romper el patrón `Outlet + Drawer` actual en `GestionCorrespondenciaRoute.tsx`
- [x] 3.4 Definir y renderizar estado de error estable para fallas de carga del inbox

## 4. Screen behavior integration

- [x] 4.1 Conectar la carga inicial de `/dashboard/gestion-correspondencia` al endpoint `POST /api/workflowInboxgestion/inboxgestion`
- [x] 4.2 Conectar el botón `Actualizar` a `refetch`
- [x] 4.3 Sincronizar `pageSize` y total visible con `Pagination.PageSize` y `Pagination.Total`
- [x] 4.4 Mantener compatibilidad con metadata dinámica de acciones sin crear una tabla paralela

## 5. Verification and documentation

- [x] 5.1 Crear pruebas para `appGridToAppTableColumns.ts`
- [x] 5.2 Crear pruebas para `appGridToAppTableRows.ts`
- [x] 5.3 Crear pruebas para `useGestionCorrespondenciaTable.ts`
- [x] 5.4 Crear pruebas para `GestionCorrespondenciaRoutePage.tsx` cubriendo loading, error y success
- [x] 5.5 Ajustar pruebas existentes del módulo para reflejar la integración real
- [x] 5.6 Documentar cualquier decisión necesaria sobre paginación y tratamiento visual de la columna `acciones`
- [x] 5.7 Ejecutar la suite relevante y dejar evidencia de validación

## Evidencia de validación

Comando ejecutado:

```bash
npm.cmd test -- src/app/Components/UI/AppTable/tests/appGridToAppTableColumns.test.ts src/app/Components/UI/AppTable/tests/appGridToAppTableRows.test.ts src/app/Components/UI/AppTable/tests/dynamicUiToAgGridColumns.test.ts src/app/Components/UI/AppTable/tests/dynamicUiToAgGridRows.test.ts src/app/Components/UI/AppTable/tests/dynamicUiActionMapper.test.ts src/app/Components/UI/AppTable/tests/dynamicUiTable.service.test.ts src/app/Components/UI/AppTable/tests/useDynamicUiTableQuery.test.ts src/app/Components/UI/AppTable/tests/dynamicUiAction.service.test.ts src/app/Components/UI/AppTable/tests/dynamicUiActionPayloadBuilder.test.ts src/app/Components/UI/AppTable/tests/dynamicUiActionGuard.test.ts src/app/Components/UI/AppTable/tests/dynamicUiActionBehaviorResolver.test.ts src/app/Components/UI/AppTable/tests/dynamicUiActionPresentationResolver.test.ts src/app/Components/UI/AppTable/tests/useDynamicUiTableActions.test.ts src/modules/gestionCorrespondencia/routes/GestionCorrespondenciaRoute.spec.test.tsx src/modules/gestionCorrespondencia/tests/useGestionCorrespondenciaTable.test.tsx src/modules/gestionCorrespondencia/tests/GestionCorrespondenciaRoutePage.test.tsx
```

Resultado:

- `16` archivos de test
- `48` tests en verde
