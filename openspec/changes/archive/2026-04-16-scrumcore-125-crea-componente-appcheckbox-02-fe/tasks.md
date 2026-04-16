## 1. Composicion del grupo

- [x] 1.1 Implementar `AppCheckboxGroup` dentro de `src/app/Components/UI/AppCheckbox/`
- [x] 1.2 Componer el grupo exclusivamente sobre `AppCheckbox`
- [x] 1.3 Exponer contrato controlado con `value` + `onChange`
- [x] 1.4 Confirmar que `AppCheckboxGroup` se implementa en el shared y no desde modulos de dominio

## 2. UI y responsive

- [x] 2.1 Soportar `direction="vertical" | "horizontal"`
- [x] 2.2 Propagar `size="sm" | "md" | "lg"` a los items con spacing consistente entre checkbox y texto
- [x] 2.3 Soportar `disabled`, `helperText` y `error` en el grupo
- [x] 2.4 Validar responsive en desktop, tablet y mobile sin romper legibilidad ni interaccion
- [x] 2.5 Mantener area clicable comoda y labels bien alineados

## 3. Formularios y export shared

- [x] 3.1 Contemplar compatibilidad con `Form.Item`
- [x] 3.2 Soportar `name` y `rules` tipadas fuertemente como `Rule[]`
- [x] 3.3 Exportar `AppCheckboxGroup` desde el indice shared correspondiente
- [x] 3.4 Implementar estilos solo con CSS Modules, sin estilos globales, manteniendo Ant Design como base visual principal

## 4. Validacion

- [x] 4.1 Crear pruebas para layout vertical y horizontal
- [x] 4.2 Validar estado controlado del grupo
- [x] 4.3 Validar `disabled`, `helperText` y `error`
- [x] 4.4 Registrar evidencia de tests ejecutados para el change

## Evidencia

- 2026-04-16: `node .\\node_modules\\vitest\\vitest.mjs --run src\\app\\Components\\UI\\AppCheckbox\\AppCheckbox.test.tsx`
- Resultado: `15 tests passed`
