## 1. API y contrato del componente

- [x] 1.1 Extender `editor.types.ts` con las nuevas props de paginacion visual (`paginationMode`, `pageFormat`, `pageOrientation`, `pageMargins`)
- [x] 1.2 Definir valores por defecto compatibles con el comportamiento actual cuando la paginacion no se activa
- [x] 1.3 Verificar que el contrato controlado/no controlado de `AppEditor` no cambie con la nueva API

## 2. Layout visual paginado

- [x] 2.1 Ajustar `presentation/AppEditor.tsx` para renderizar condicionalmente la estructura `editorWrapper -> canvas -> sheet -> content`
- [x] 2.2 Implementar en `AppEditor.module.css` el canvas de workspace y la hoja centrada para `paginationMode="visual"`
- [x] 2.3 Aplicar las dimensiones base de `A4 portrait` (`794px` x `1123px`) como referencia visual del modo paginado
- [x] 2.4 Asegurar que el scroll del modo visual ocurra en el `canvas` y no en una hoja con scroll interno independiente
- [x] 2.5 Preservar el comportamiento actual del editor cuando `paginationMode` no este activo

## 3. Integracion y compatibilidad

- [x] 3.1 Confirmar que `surfaceClassName`, `className` y `minHeight` sigan siendo compatibles con el nuevo wrapper visual
- [x] 3.2 Validar que el editor siga funcionando con toolbar, links, listas, headings e imagenes en modo visual
- [x] 3.3 Confirmar que el HTML serializado por `value` y `onChange` no incorpora metadata de paginacion

## 4. Pruebas y evidencia

- [x] 4.1 Agregar o ajustar pruebas para cubrir el modo por defecto sin paginacion
- [x] 4.2 Agregar o ajustar pruebas para cubrir `paginationMode="visual"` y la hoja centrada
- [x] 4.3 Ejecutar pruebas focalizadas de `AppEditor` y registrar resultados
- [x] 4.4 Ejecutar validacion TypeScript o la verificacion equivalente del repo y registrar residuos ajenos si aparecen
- [x] 4.5 Registrar evidencia final en este archivo

## Evidencia

- Se extendio `AppEditorProps` con `paginationMode`, `pageFormat`, `pageOrientation` y `pageMargins`, dejando valores por defecto compatibles con el comportamiento previo.
- `AppEditor.tsx` ahora renderiza condicionalmente la estructura `editorWrapper -> canvas -> sheet -> content` cuando `paginationMode="visual"`, sin modificar `useAppEditor` ni la infraestructura Tiptap.
- `AppEditor.module.css` incorpora el canvas de workspace, la hoja centrada, dimensiones base de `A4 portrait` y scroll en `canvas`, manteniendo el modo continuo cuando la paginacion no se activa.
- Se preservo compatibilidad con `className`, `surfaceClassName` y `minHeight`, y el contenido sigue serializandose como HTML sin metadata de paginacion.
- Se ajusto `AppEditor.test.tsx` para verificar el modo por defecto y el modo `visual` con canvas y hoja.
- Pruebas ejecutadas:
  - `npm test -- AppEditor.test.tsx AppEditorToolbar.test.tsx useAppEditor.test.tsx AppEditor.integration.test.tsx` -> `4 files passed`, `19 tests passed`
  - `npx tsc -p tsconfig.app.json --noEmit` -> mantiene errores preexistentes ajenos al cambio en `src/app/Components/UI/AppTabs/AppTabs.tsx` y `src/setupTests.ts`
