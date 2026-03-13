## 1. Estructura base del componente

- [x] 1.1 Crear la carpeta `src/app/Components/UI/AppButton/` con `AppButton.tsx`, `AppButton.module.css`, `AppButton.test.tsx`, `index.ts` y `README.md`.
- [x] 1.2 Definir `AppButtonProps` con TypeScript estricto, `forwardRef<HTMLButtonElement, AppButtonProps>` y herencia controlada desde Ant Design usando `Omit<ComponentProps<typeof AntButton>, ...>`.
- [x] 1.3 Exponer `AppButton` desde el barrel correspondiente de la capa UI para habilitar su consumo compartido.

## 2. Implementacion del contrato visual y funcional

- [x] 2.1 Implementar el render base de `AppButton` como wrapper sobre `Button` y `Tooltip` de Ant Design sin renderizar un `<button>` nativo directo.
- [x] 2.2 Mapear `variant` (`primary`, `secondary`, `success`, `warning`, `danger`, `ghost`, `link`) y `size` (`sm`, `md`, `lg`) a estilos con CSS Modules y semantica visual consistente.
- [x] 2.3 Implementar soporte para `loading`, `disabled`, `htmlType="button"` por defecto y `fullWidth`, evitando ejecuciones duplicadas de `onClick`.
- [x] 2.4 Resolver la precedencia entre `icon`, `leftIcon`, `rightIcon` y `children`, incluyendo modo `icon-only` con `aria-label` obligatorio.
- [x] 2.5 Envolver el boton con `Tooltip` de forma segura para mantener ayuda contextual tambien en estados `disabled` o `loading`.

## 3. Documentacion y pruebas

- [x] 3.1 Crear pruebas con Vitest + Testing Library para render, `onClick`, bloqueo por `disabled/loading`, `htmlType` por defecto, `aria-disabled`, iconos, `icon-only`, tooltip, variantes, `fullWidth` y tamanos.
- [x] 3.2 Agregar identificadores `[SPEC:<SPEC_ID>]` en los tests que cubren los requisitos del nuevo spec `app-button`.
- [x] 3.3 Redactar `README.md` del componente con descripcion, importacion, API de props, ejemplos de uso, integracion asincronica y buenas practicas.

## 4. Verificacion final

- [x] 4.1 Ejecutar la suite de pruebas relevante del componente y registrar evidencia de resultados para la documentacion OpenSpec.
- [x] 4.2 Revisar exports, tipado y estilos finales para confirmar que `AppButton` queda desacoplado de dominio y alineado con la arquitectura UI del proyecto.

## Evidencia

- `2026-03-13`: `npm.cmd test -- src/app/Components/UI/AppButton/AppButton.test.tsx` -> `10 passed`.
- `2026-03-13`: `npx.cmd tsc -p tsconfig.app.json --noEmit` -> errores preexistentes fuera de `AppButton`; sin errores locales nuevos en este cambio.
