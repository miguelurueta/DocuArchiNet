## 1. AppCheckboxCheckAll

- [x] 1.1 Implementar `AppCheckboxCheckAll` dentro de `src/app/Components/UI/AppCheckbox/`
- [x] 1.2 Mantener el contrato controlado con `value`, `onChange` y `options`
- [x] 1.3 Resolver seleccion total, limpieza total y estado `checked`
- [x] 1.4 Reflejar `indeterminate` cuando exista seleccion parcial

## 2. Relacion con Group

- [x] 2.1 Reutilizar `AppCheckboxGroup` o un helper interno comun para la logica de seleccion
- [x] 2.2 Evitar duplicacion de logica entre `Group` y `CheckAll`
- [x] 2.3 Mantener compatibilidad con `disabled`, `size` y contratos tipados existentes

## 3. Documentacion y ejemplos

- [x] 3.1 Actualizar el README de `AppCheckbox` con API real
- [x] 3.2 Agregar ejemplos de checkbox simple, grupo y `check all`
- [x] 3.3 Incluir ejemplo de integracion con `Form.Item`, `name` y `rules`

## 4. Validacion

- [x] 4.1 Ampliar pruebas de `AppCheckbox` para cubrir `check all`
- [x] 4.2 Validar seleccion total, limpieza total e `indeterminate`
- [x] 4.3 Validar comportamiento `disabled`
- [x] 4.4 Ejecutar la suite del componente y registrar evidencia en este archivo

## Evidencia

- 2026-04-16: `node .\node_modules\vitest\vitest.mjs --run src\app\Components\UI\AppCheckbox\AppCheckbox.test.tsx`
- Resultado: `21 tests passed`
