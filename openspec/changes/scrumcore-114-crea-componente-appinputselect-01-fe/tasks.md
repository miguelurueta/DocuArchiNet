## 1. Core del componente

- [x] 1.1 Crear `src/app/Components/UI/AppInputSelect/`
- [x] 1.2 Implementar `AppInputSelect.tsx` como wrapper reusable sobre `Select` de Ant Design
- [x] 1.3 Definir y exportar tipos `AppInputSelectProps`, `AppInputSelectOption` y `AppInputSelectSize`
- [x] 1.4 Soportar opciones locales via `options`
- [x] 1.5 Soportar modo remoto via `fetchOptions`

## 2. UI, estados y responsive

- [x] 2.1 Crear `AppInputSelect.module.css` con ajustes minimos sobre Ant Design
- [x] 2.2 Implementar tamaños `sm`, `md`, `lg` alineados al sistema UI
- [x] 2.3 Implementar estado visual `loading`
- [x] 2.4 Implementar estado visual `no data` con `notFoundContent`
- [x] 2.5 Validar responsive en desktop, tablet y mobile con border radius leve y moderno

## 3. Integracion shared y documentacion

- [x] 3.1 Exportar `AppInputSelect` desde `src/app/Components/UI/index.ts`
- [x] 3.2 Crear `README.md` dentro de `src/app/Components/UI/AppInputSelect/`
- [x] 3.3 Documentar ejemplos de uso local, remoto y multiple
- [x] 3.4 Documentar el contrato de integracion backend via `fetchOptions`

## 4. Validacion

- [x] 4.1 Crear o ajustar pruebas para render basico y cambio de valor
- [x] 4.2 Validar flujo remoto con loading y estado vacio
- [x] 4.3 Validar sizing `sm`, `md`, `lg`
- [x] 4.4 Registrar evidencia de tests ejecutados para el change

## Evidencia de pruebas

- `node .\\node_modules\\vitest\\vitest.mjs --run src/app/Components/UI/AppInputSelect/AppInputSelect.test.tsx` (2026-04-15)
