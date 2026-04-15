## 1. Reforzar layout visual de hoja real

- [x] 1.1 Revisar la estructura actual `editorWrapper -> canvas -> sheet -> surface`
- [x] 1.2 Hacer que `sheet` represente visualmente una hoja A4 real
- [x] 1.3 Diferenciar el `canvas` como workspace exterior del documento
- [x] 1.4 Mantener compatibilidad con `paginationMode="visual"` sin afectar `paginationMode="none"`

## 2. Margenes visuales y caja util

- [x] 2.1 Renderizar margenes visibles top/right/bottom/left como parte del layout
- [x] 2.2 Hacer que el contenido se perciba dentro de una caja util de documento
- [x] 2.3 Evitar padding estructural persistente sobre `.ProseMirror`
- [x] 2.4 Confirmar que no cambia el HTML serializado

## 3. Sustituir guias visibles sin romper la logica interna

- [x] 3.1 Retirar la representacion visible de lineas guia
- [x] 3.2 Mantener `usePaginationMetrics` como base de calculo interno
- [x] 3.3 Confirmar compatibilidad con contador de pagina

## 4. Compatibilidad con capacidades existentes

- [x] 4.1 Confirmar compatibilidad con zoom visual
- [x] 4.2 Confirmar compatibilidad con `PageBreak`
- [x] 4.3 Confirmar compatibilidad con imagenes (`data-width`, `data-align`, locales/remotas)
- [x] 4.4 Confirmar que toolbar y scroll continuo siguen funcionando

## 5. Pruebas y evidencia

- [x] 5.1 Agregar o ajustar pruebas de render en modo visual
- [x] 5.2 Agregar pruebas para margenes visuales y hoja A4 delimitada
- [x] 5.3 Ejecutar pruebas focalizadas del editor
- [x] 5.4 Ejecutar validacion TypeScript o equivalente y registrar residuos ajenos si aparecen
- [x] 5.5 Registrar evidencia final en este archivo

## Evidencia

- `src/app/Components/UI/AppEditor/presentation/AppEditor.tsx`: se introdujo `pageDocument` como caja visual interna de la hoja y se retiraron las lineas guia visibles, manteniendo `usePaginationMetrics` para conteo y limites internos.
- `src/app/Components/UI/AppEditor/AppEditor.module.css`: el workspace exterior y la hoja A4 se reforzaron visualmente; los margenes pasaron al wrapper de documento y `.ProseMirror` dejo de usar padding estructural de pagina.
- `src/app/Components/UI/AppEditor/AppEditor.test.tsx`: se ajustaron pruebas para validar la nueva caja de documento y confirmar que el calculo interno de paginas sigue activo sin overlays visibles.
- `npm test -- AppEditor.test.tsx AppEditorToolbar.test.tsx useAppEditor.test.tsx usePaginationMetrics.test.tsx usePageContext.test.tsx resizableImage.extension.test.ts appEditorImageStore.test.ts localImageIds.test.ts` -> `8 files passed`, `43 tests passed`.
- `npx tsc -p tsconfig.app.json --noEmit` -> persisten solo errores preexistentes en `src/app/Components/UI/AppTabs/AppTabs.tsx` y `src/setupTests.ts`.
