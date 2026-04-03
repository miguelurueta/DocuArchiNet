## 1. Botón base iconográfico

- [x] 1.1 Revisar `AppButton` para definir la integración correcta de un botón icon-only reusable
- [x] 1.2 Implementar `AppIconActionButton` reutilizando `AppButton` y soportando `icon`, `loading`, `disabled`, `aria-label`, `tooltip` y `size`
- [x] 1.3 Validar consistencia visual para acciones de toolbar, refresh y acciones de celda

## 2. Integración con AppDropdown

- [x] 2.1 Revisar el contrato actual de `AppDropdown` para identificar la integración no intrusiva del trigger iconográfico
- [x] 2.2 Ajustar `AppDropdown` para aceptar `AppIconActionButton` como trigger compatible sin romper triggers actuales
- [x] 2.3 Verificar que las acciones `icon_button` puedan reutilizar la misma familia visual

## 3. Wrapper reusable de consulta

- [x] 3.1 Implementar `AppTableQueryWrapper` con `queryState`, `onQueryChange`, `onRefresh`, `total`, `loading`, `headerActions` y `children`
- [x] 3.2 Renderizar búsqueda, refresh opcional, acciones adicionales, contenedor de tabla y navegación externa en un mismo bloque visual
- [x] 3.3 Garantizar que el wrapper emita patches simples y no resuelva merge/reset de `AppTableQueryState`

## 4. Pruebas y validación

- [x] 4.1 Agregar pruebas de `AppIconActionButton` para render, loading, disabled, accesibilidad y tooltip
- [x] 4.2 Agregar pruebas de integración de `AppDropdown` con trigger basado en `AppIconActionButton`
- [x] 4.3 Agregar pruebas de `AppTableQueryWrapper` para estructura, emisión de `onQueryChange` y `onRefresh`
- [x] 4.4 Ejecutar la suite asociada y dejar evidencia del resultado en este cambio OpenSpec

Evidencia:
- `npm.cmd test -- src/app/Components/UI/AppButton/AppButton.test.tsx src/app/Components/UI/AppButton/AppIconActionButton.test.tsx src/app/Components/UI/AppDropdown/AppDropdown.test.tsx src/app/Components/UI/AppTable/tests/AppTableQueryWrapper.test.tsx src/app/Components/UI/AppTable/tests/AppTableActionCellRenderer.test.tsx`
- Resultado: `5` archivos, `36` tests en verde
