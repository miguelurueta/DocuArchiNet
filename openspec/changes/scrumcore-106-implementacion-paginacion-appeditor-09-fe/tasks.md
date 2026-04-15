## 1. Extension `PageBreak`

- [x] 1.1 Crear la extension Tiptap `PageBreak` en `infrastructure`
- [x] 1.2 Definir el nodo como `block`, `atom: true`, `selectable: true` e `isolating: true`
- [x] 1.3 Implementar `parseHTML` para detectar `data-page-break="true"`
- [x] 1.4 Implementar `renderHTML` con `<div data-page-break="true"></div>`
- [x] 1.5 Exponer el comando `editor.commands.insertPageBreak()`

## 2. Insercion valida y navegacion

- [x] 2.1 Validar posicion de insercion para permitir el salto solo en puntos seguros del documento
- [x] 2.2 Evitar multiples `PageBreak` consecutivos
- [x] 2.3 Confirmar navegacion funcional del cursor antes y despues del nodo
- [x] 2.4 Confirmar que el salto no bloquee escritura, seleccion ni undo/redo

## 3. Render visual del salto

- [x] 3.1 Definir una representacion visual clara y no editable del `PageBreak`
- [x] 3.2 Integrar el estilo del salto con el shell de paginacion visual existente
- [x] 3.3 Confirmar que el salto siga siendo distinguible sin competir con toolbar, guias o contador

## 4. Integracion con paginacion visual

- [x] 4.1 Ajustar `usePaginationMetrics` o la capa equivalente para tratar `PageBreak` como limite duro
- [x] 4.2 Reiniciar el calculo de paginas despues de cada salto manual
- [x] 4.3 Confirmar compatibilidad entre `PageBreak`, guias visuales y contador de pagina actual
- [x] 4.4 Confirmar que el documento sigue siendo un flujo continuo fuera del calculo visual

## 5. Serializacion, rehidratacion y pruebas

- [x] 5.1 Agregar pruebas de extension `PageBreak`
- [x] 5.2 Agregar pruebas de serializacion HTML
- [x] 5.3 Agregar pruebas de rehidratacion desde `data-page-break`
- [x] 5.4 Agregar pruebas de integracion con `AppEditor` en modo visual
- [x] 5.5 Ejecutar pruebas focalizadas del editor y registrar resultados
- [x] 5.6 Ejecutar validacion TypeScript o equivalente y registrar residuos ajenos si aparecen
- [x] 5.7 Registrar evidencia final en este archivo

## Evidencia

- Se agrego la extension `PageBreak` en `infrastructure/page-break.extension.ts` con nodo `block`, `atom`, `selectable` e `isolating`.
- La extension serializa y rehidrata el salto manual mediante `<div data-page-break="true"></div>`.
- Se expuso el comando `editor.commands.insertPageBreak()` y se conecto un boton visible en la toolbar para permitir prueba manual directa.
- La insercion valida evita `PageBreak` consecutivos inspeccionando el contexto de insercion antes de ejecutar el comando.
- `usePaginationMetrics` ahora detecta offsets de `PageBreak` en el DOM y los trata como limites duros, reiniciando el calculo de paginas por segmentos.
- `usePageContext` ahora resuelve la pagina actual usando `pageBoundaries`, por lo que el contador tambien respeta los saltos manuales.
- `AppEditor.module.css` incorpora el render visual del salto manual con linea, etiqueta y estado seleccionado.
- Pruebas agregadas o ajustadas:
  - `pageBreak.extension.test.ts`
  - `usePaginationMetrics.test.tsx`
  - `usePageContext.test.tsx`
  - `AppEditor.test.tsx`
- Pruebas ejecutadas:
  - `npm test -- AppEditor.test.tsx AppEditorToolbar.test.tsx useAppEditor.test.tsx AppEditor.integration.test.tsx usePaginationMetrics.test.tsx usePageContext.test.tsx pageBreak.extension.test.ts` -> `7 files passed`, `30 tests passed`
  - `npx tsc -p tsconfig.app.json --noEmit` -> mantiene errores preexistentes ajenos al cambio en `src/app/Components/UI/AppTabs/AppTabs.tsx` y `src/setupTests.ts`
