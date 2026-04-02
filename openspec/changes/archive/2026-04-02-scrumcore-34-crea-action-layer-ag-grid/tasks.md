## 1. Contracts and file placement

- [x] 1.1 Crear `src/app/Components/UI/AppTable/types/dynamicUiTableAction.types.ts` con contratos compatibles con `AppGridCellAction`, `AppGridRow` y `ApiResponse<unknown>`
- [x] 1.2 Definir `DynamicUiActionContext` incluyendo `userClaims?` para soportar evaluación de claims sin dependencias de dominio
- [x] 1.3 Mantener toda la implementación de la fase dentro de `src/app/Components/UI/AppTable/`, sin mover la capability a `src/features/dynamic-ui-table/`

## 2. Action execution service

- [x] 2.1 Implementar `src/app/Components/UI/AppTable/services/dynamicUiAction.service.ts` sobre `clienteApi`
- [x] 2.2 Soportar endpoint default, endpoint inyectable por invocación y factory ligada a endpoint
- [x] 2.3 Validar que el servicio preserve el contrato backend y no transforme la respuesta a estado visual

## 3. Pure action helpers

- [x] 3.1 Implementar `src/app/Components/UI/AppTable/utils/dynamicUiActionPayloadBuilder.ts` como función pura con precedencia correcta entre payload derivado, metadata request y payload manual
- [x] 3.2 Implementar `src/app/Components/UI/AppTable/utils/dynamicUiActionGuard.ts` evaluando `RequiredClaimsAny`, `RequiredClaimsAll`, `ClaimKey` y solo reglas seguras en frontend
- [x] 3.3 Implementar `src/app/Components/UI/AppTable/utils/dynamicUiActionBehaviorResolver.ts` con clasificación extensible y sin side effects
- [x] 3.4 Implementar `src/app/Components/UI/AppTable/utils/dynamicUiActionPresentationResolver.ts` con clasificación extensible y sin render UI
- [x] 3.5 Reutilizar la normalización de Fase 1B sin duplicar la lógica de `dynamicUiActionMapper.ts`

## 4. React Query orchestration

- [x] 4.1 Implementar `src/app/Components/UI/AppTable/hooks/useDynamicUiTableActions.ts` como única capa con React Query para acciones dinámicas
- [x] 4.2 Integrar el action service, payload builder, guard y resolvers dentro del hook sin acoplarlo a navegación, modales o dominio
- [x] 4.3 Exponer desde el hook `executeAction`, helpers reutilizables, `isExecutingAction`, `actionError` y `lastActionResult`

## 5. Verification and documentation

- [x] 5.1 Crear `src/app/Components/UI/AppTable/tests/dynamicUiAction.service.test.ts` cubriendo ejecución HTTP y endpoint default/inyectable
- [x] 5.2 Crear `src/app/Components/UI/AppTable/tests/dynamicUiActionPayloadBuilder.test.ts` cubriendo precedencia, `rowId`, `selectedRowIds` y manual override
- [x] 5.3 Crear `src/app/Components/UI/AppTable/tests/dynamicUiActionGuard.test.ts` cubriendo claims, `ClaimKey` y reglas no resolubles de frontend
- [x] 5.4 Crear `src/app/Components/UI/AppTable/tests/dynamicUiActionBehaviorResolver.test.ts` y `dynamicUiActionPresentationResolver.test.ts` cubriendo valores conocidos y futuros
- [x] 5.5 Crear `src/app/Components/UI/AppTable/tests/useDynamicUiTableActions.test.ts` cubriendo mutación, estados del hook y salida estructurada
- [x] 5.6 Documentar la separación metadata vs ejecución, los límites del guard y la preparación para Fase 4
- [x] 5.7 Ejecutar la suite relevante de `AppTable` y dejar evidencia de validación en el cambio OpenSpec

## Evidencia de validación

Comando ejecutado:

```bash
npm.cmd test -- src/app/Components/UI/AppTable/tests/dynamicUiToAgGridColumns.test.ts src/app/Components/UI/AppTable/tests/dynamicUiToAgGridRows.test.ts src/app/Components/UI/AppTable/tests/dynamicUiActionMapper.test.ts src/app/Components/UI/AppTable/tests/dynamicUiTable.service.test.ts src/app/Components/UI/AppTable/tests/useDynamicUiTableQuery.test.ts src/app/Components/UI/AppTable/tests/dynamicUiAction.service.test.ts src/app/Components/UI/AppTable/tests/dynamicUiActionPayloadBuilder.test.ts src/app/Components/UI/AppTable/tests/dynamicUiActionGuard.test.ts src/app/Components/UI/AppTable/tests/dynamicUiActionBehaviorResolver.test.ts src/app/Components/UI/AppTable/tests/dynamicUiActionPresentationResolver.test.ts src/app/Components/UI/AppTable/tests/useDynamicUiTableActions.test.ts
```

Resultado:

- `11` archivos de test
- `39` tests en verde
