## 1. Estructura base del componente

- [x] 1.1 Crear `src/app/Components/UI/AppEditor/` con subcarpetas `domain`, `application`, `infrastructure` y `presentation`
- [x] 1.2 Definir tipos base en `domain/editor.types.ts` y modelo auxiliar en `domain/editor.model.ts`
- [x] 1.3 Configurar export publico del componente en `src/app/Components/UI/index.ts`

## 2. Core de edicion con Tiptap

- [x] 2.1 Implementar `tiptap.extensions.ts` usando solo extensiones/licencias MIT requeridas por el ticket
- [x] 2.2 Implementar `tiptap.config.ts` para centralizar configuracion del editor
- [x] 2.3 Implementar `useAppEditor.ts` para orquestar la instancia del editor sin usar `any`
- [x] 2.4 Soportar modo controlado (`value` + `onChange`) y no controlado con sincronizacion estable
- [x] 2.5 Garantizar que cambios externos no rompan cursor, seleccion ni `undo/redo`

## 3. Presentacion y layout reusable

- [x] 3.1 Implementar `presentation/AppEditor.tsx` con encabezado contextual, superficie principal y scroll interno
- [x] 3.2 Implementar `presentation/AppEditorToolbar.tsx` desacoplado de logica de negocio
- [x] 3.3 Soportar acciones de toolbar: `bold`, `italic`, `underline`, listas, headings, alineacion, `undo`, `redo`
- [x] 3.4 Implementar UI minima para enlaces e insercion de imagenes
- [x] 3.5 Soportar `placeholder`, `disabled`, `readOnly`, `label`, `helperText`, `error`, `className` y `aria-label`

## 4. Integracion y documentacion local

- [x] 4.1 Crear estilos/modulo visual del componente sin acoplarlo a `GestionRespuesta`
- [x] 4.2 Agregar `README.md` del componente con contrato y ejemplo de uso
- [x] 4.3 Verificar coexistencia con `GestionRespuestaEditorContainer` sin reemplazo inmediato

## 5. Pruebas

- [x] 5.1 Agregar pruebas de `presentation` para render, toolbar e interacciones visibles
- [x] 5.2 Agregar pruebas de `application` para `useAppEditor`
- [x] 5.3 Validar escenarios del spec: controlled, uncontrolled, disabled, readOnly, helper/error, accesibilidad
- [x] 5.4 Validar formato, listas, headings, alineacion, links, imagenes, `undo` y `redo`
- [x] 5.5 Ejecutar pruebas del componente y registrar evidencia en este archivo

## Evidencia

- `node .\\node_modules\\vitest\\vitest.mjs --run src/app/Components/UI/AppEditor/AppEditor.test.tsx src/app/Components/UI/AppEditor/AppEditorToolbar.test.tsx src/app/Components/UI/AppEditor/useAppEditor.test.tsx` -> `9 passed` (2026-04-14)
- `npx tsc -p tsconfig.app.json --noEmit` -> AppEditor sin errores propios; quedan errores preexistentes en `src/app/Components/UI/AppTabs/AppTabs.tsx`, `src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaMainTabContent.tsx` y `src/setupTests.ts` (2026-04-14)
