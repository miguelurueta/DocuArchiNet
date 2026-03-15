## 1. Estructura base del componente

- [x] 1.1 Crear la carpeta `src/app/Components/UI/AppTabs/` con `AppTabs.tsx`, `AppTabs.module.css`, `AppTabs.test.tsx`, `index.ts` y `README.md`.
- [x] 1.2 Definir `AppTabsProps` y `AppTabsItem` con TypeScript estricto y herencia controlada desde el control base del proveedor UI usando `Omit<ComponentProps<...>, ...>`.
- [x] 1.3 Exponer `AppTabs` desde el barrel correspondiente de la capa UI para habilitar su consumo compartido.

## 2. Implementacion del contrato visual y funcional

- [x] 2.1 Implementar el render base de `AppTabs` como wrapper sobre el control de tabs del proveedor UI sin acoplar a las vistas a su API completa.
- [x] 2.2 Implementar soporte para `items`, tab activa controlada/no controlada, `onChange`, orientacion y tabs deshabilitadas.
- [x] 2.3 Mapear variantes y estilos de la raiz, lista de tabs y panel activo mediante CSS Modules para mantener consistencia con el design system interno.
- [x] 2.4 Garantizar accesibilidad de `tablist`, `tab`, `tabpanel` y navegacion por teclado delegada al control base cuando aplique.
- [x] 2.5 Permitir composicion segura con labels y contenido React enriquecido sin romper la experiencia base del componente.

## 3. Documentacion y pruebas

- [x] 3.1 Crear pruebas con Vitest + Testing Library para render de tabs, cambio de seleccion, estado deshabilitado, orientacion y contrato accesible.
- [x] 3.2 Agregar identificadores `[SPEC:<SPEC_ID>]` en los tests que cubren los requisitos del nuevo spec `app-tabs`.
- [x] 3.3 Redactar `README.md` del componente con descripcion, importacion, API de props, ejemplos de uso y buenas practicas.

## 4. Verificacion final

- [x] 4.1 Ejecutar la suite de pruebas relevante del componente y registrar evidencia de resultados para la documentacion OpenSpec.
- [x] 4.2 Revisar exports, tipado y estilos finales para confirmar que `AppTabs` queda desacoplado de dominio y alineado con la arquitectura UI del proyecto.

## Evidencia

- `npm.cmd test -- src/app/Components/UI/AppTabs/AppTabs.test.tsx` -> `6 passed`
- `npx.cmd tsc -p tsconfig.app.json --noEmit` -> sin errores nuevos en `AppTabs`; persisten errores preexistentes del repositorio en `src/api`, `src/modules/dashboard` y `src/modules/radicacion`
