## 1. Core del componente

- [x] 1.1 Crear `src/app/Components/UI/AppCheckbox/`
- [x] 1.2 Implementar `AppCheckbox.tsx` como wrapper sobre `Checkbox` de Ant Design
- [x] 1.3 Definir y exportar tipos base del componente
- [x] 1.4 Exponer `checked`, `defaultChecked`, `disabled`, `indeterminate` y `onChange`

## 2. API shared y formularios

- [x] 2.1 Agregar soporte para `label`, `helperText` y atributos aria
- [x] 2.2 Soportar `size="sm" | "md" | "lg"`
- [x] 2.3 Definir contratos publicos controlados de `AppCheckboxGroup` y `AppCheckboxCheckAll` con `value` + `onChange` obligatorios
- [x] 2.4 Contemplar compatibilidad con `Form.Item`, `name` y `rules` tipadas fuertemente como `Rule[]`
- [x] 2.5 Dejar explicito que `AppCheckboxCheckAll` usa `AppCheckboxGroup` o un helper/hook interno comun

## 3. UI y export shared

- [x] 3.1 Implementar `AppCheckbox.module.css` con CSS Modules
- [x] 3.2 Mantener apariencia base de Ant Design con refinamiento shared minimo
- [x] 3.3 Exportar el componente desde el indice shared correspondiente

## 4. Validacion

- [x] 4.1 Crear pruebas para render basico, controlado y no controlado
- [x] 4.2 Validar estados `disabled` e `indeterminate`
- [x] 4.3 Validar `onChange`
- [x] 4.4 Registrar evidencia de tests ejecutados para el change

## Evidencia

- 2026-04-16: `node .\\node_modules\\vitest\\vitest.mjs --run src\\app\\Components\\UI\\AppCheckbox\\AppCheckbox.test.tsx`
- Resultado: `8 tests passed`
