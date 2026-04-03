## 1. Tipos y propagación de MenuActions

- [x] 1.1 Extender `src/app/Components/UI/AppTable/types/dynamicUiTable.types.ts` para modelar `MenuActions`, `Children` e `IsDivider`
- [x] 1.2 Propagar `MenuActions` al modelo interno compartido consumido por `AppTable`
- [x] 1.3 Ajustar `src/app/Components/UI/AppTable/types/dynamicUiTableQuery.types.ts` y `src/app/Components/UI/AppTable/hooks/useDynamicUiTableQuery.ts` para no perder la metadata necesaria del menú

## 2. Resolución de menuItems en la capa compartida

- [x] 2.1 Ajustar `src/app/Components/UI/AppTable/utils/dynamicUiActionMapper.ts` para preservar metadata suficiente de menú
- [x] 2.2 Implementar la resolución `menuItems -> MenuActions -> acciones completas` en la capa compartida de `AppTable`
- [x] 2.3 Mantener fallback controlado cuando falte `MenuActions` o falten ids resolubles

## 3. Integración con AppDropdown

- [x] 3.1 Extender `src/app/Components/UI/AppDropdown/AppDropdown.tsx` para soportar `type: "divider"` sin romper consumidores actuales
- [x] 3.2 Mapear `Children` recursivamente a `children` de `AppDropdownItem`
- [x] 3.3 Mapear `IsDivider` a un separador visual no ejecutable
- [x] 3.4 Mantener acciones directas existentes cuando no haya `menuItems` válidos

## 4. Ejecución y reglas de validez

- [x] 4.1 Ejecutar solo items resolubles y válidos mediante la action layer existente
- [x] 4.2 Evitar ejecución de divisores e items inválidos
- [x] 4.3 Mantener inmutabilidad de `MenuActions`, `Children` y `BehaviorConfig`

## 5. Verificación y documentación

- [x] 5.1 Crear o ajustar pruebas para resolución de `menuItems`
- [x] 5.2 Crear o ajustar pruebas para submenús (`Children`)
- [x] 5.3 Crear o ajustar pruebas para divisores (`IsDivider`)
- [x] 5.4 Verificar que `AppDropdown` no tenga regresiones fuera de tablas dinámicas
- [x] 5.5 Verificar que `GestionCorrespondencia` no tenga regresiones
- [x] 5.6 Documentar el nuevo comportamiento de `MenuActions`, `Children` e `IsDivider`
