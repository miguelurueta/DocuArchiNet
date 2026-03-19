## 1. Estructura base del componente

- [x] 1.1 Crear la carpeta `src/app/Components/UI/AppInput/` con `AppInput.tsx`, `AppInput.module.css`, `AppInput.test.tsx`, `index.ts` y `README.md`.
- [x] 1.2 Definir `AppInputProps` con TypeScript estricto y herencia controlada desde el control base del proveedor UI usando `Omit<ComponentProps<...>, ...>`.
- [x] 1.3 Exponer `AppInput` desde el barrel correspondiente de la capa UI para habilitar su consumo compartido.

## 2. Implementacion del contrato visual y funcional

- [x] 2.1 Implementar el render base de `AppInput` como wrapper sobre el control de entrada del proveedor UI sin acoplar a las vistas a su API visual directa.
- [x] 2.2 Implementar soporte para `value`, `defaultValue`, `onChange`, `placeholder`, `label`, `helperText`, `disabled` y `error`.
- [x] 2.3 Mapear estados visuales y presentacion base mediante CSS Modules para mantener consistencia con el design system interno.
- [x] 2.4 Garantizar accesibilidad en foco visible, asociacion label-control, helper text y semantica de error/deshabilitado.
- [x] 2.5 Permitir composicion segura entre estilos propios del componente y `className` externa.

## 3. Documentacion y pruebas

- [x] 3.1 Crear pruebas con Vitest + Testing Library para render, `onChange`, sincronizacion de valor, `disabled`, `error`, `label`, `helperText`, `placeholder` y composicion de clases.
- [x] 3.2 Agregar identificadores `[SPEC:<SPEC_ID>]` en los tests que cubren los requisitos del nuevo spec `app-input`.
- [x] 3.3 Redactar `README.md` del componente con descripcion, importacion, API de props, ejemplos de uso y buenas practicas.

## 4. Verificacion final

- [x] 4.1 Ejecutar la suite de pruebas relevante del componente y registrar evidencia de resultados para la documentacion OpenSpec.
- [x] 4.2 Revisar exports, tipado y estilos finales para confirmar que `AppInput` queda desacoplado de dominio y alineado con la arquitectura UI del proyecto.

## Evidencia

- `2026-03-13`: `npm.cmd test -- src/app/Components/UI/AppInput/AppInput.test.tsx` -> `6 passed`.
- `2026-03-13`: `npx.cmd tsc -p tsconfig.app.json --noEmit` -> errores preexistentes fuera de `AppInput`; sin errores locales nuevos de este cambio.
