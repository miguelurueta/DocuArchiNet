## 1. Modelo de segmentacion visual por area util

- [x] 1.1 Revisar `usePaginationMetrics.ts` y la medicion actual de hojas visuales
- [x] 1.2 Calcular el area util real de pagina a partir de formato y margenes
- [x] 1.3 Construir un modelo derivado de segmentos/paginas por acumulacion de alturas
- [x] 1.4 Evitar que el modelo de segmentacion mutile el documento o introduzca nodos persistidos

## 2. Integracion de `PageBreak` y contenido especial

- [x] 2.1 Integrar `PageBreak` como corte obligatorio de nueva hoja
- [x] 2.2 Reiniciar el acumulado de altura cuando aparezca un `PageBreak`
- [x] 2.3 Definir manejo de bloques o imagenes mas altos que el area util sin romper el editor
- [x] 2.4 Confirmar que no se fragmentan nodos, imagenes ni parrafos para forzar cortes

## 3. Presentacion multi-hoja sin dividir ProseMirror

- [x] 3.1 Ajustar `AppEditor.tsx` para consumir el modelo de segmentos y materializar saltos visuales entre hojas
- [x] 3.2 Ajustar `AppEditor.module.css` para representar separacion clara entre paginas y caja util del documento
- [x] 3.3 Mantener una sola instancia de `.ProseMirror` y scroll continuo en `canvas`
- [x] 3.4 Confirmar que `paginationMode="none"` no sufre regresiones visuales

## 4. Coherencia con zoom, contador e interaccion

- [x] 4.1 Recalcular segmentos correctamente al cambiar `zoomLevel`
- [x] 4.2 Revisar `usePageContext.ts` para mantener coherencia del contador `Pagina X de Y`
- [x] 4.3 Confirmar que cursor, seleccion y undo/redo no se degradan con la segmentacion visual
- [x] 4.4 Confirmar que toolbar, scroll y overlays no bloquean la interaccion editable

## 5. Pruebas y evidencia

- [x] 5.1 Agregar o ajustar pruebas para documento de una sola pagina
- [x] 5.2 Agregar o ajustar pruebas para documento multipagina con salto visual claro
- [x] 5.3 Agregar pruebas de `PageBreak` como corte forzado
- [x] 5.4 Agregar pruebas de compatibilidad con zoom e imagenes grandes
- [x] 5.5 Ejecutar pruebas focalizadas del editor y registrar resultados
- [x] 5.6 Ejecutar validacion TypeScript o equivalente y registrar residuos ajenos si aparecen
- [x] 5.7 Registrar evidencia final en este archivo

## Evidencia

- `src/app/Components/UI/AppEditor/application/usePaginationMetrics.ts`: se extendio el modelo de metricas para calcular `visualPageBoundaries`, `pageStride` y `visualContentHeight`, y para aplicar desplazamiento vertical por bloques sin modificar el documento persistido.
- `src/app/Components/UI/AppEditor/presentation/AppEditor.tsx`: el modo `paginationMode="visual"` ahora renderiza una pila de hojas (`pageStack` / `pageShell`) y usa los limites visuales para el contador de pagina.
- `src/app/Components/UI/AppEditor/AppEditor.module.css`: se materializo el modo multi-hoja con separacion visual real entre paginas y flujo de contenido segmentado sobre una sola instancia de `.ProseMirror`.
- `src/app/Components/UI/AppEditor/AppEditor.test.tsx`: se ajustaron pruebas del modo visual para validar la nueva estructura multi-hoja y el contador con offsets visuales.
- `src/app/Components/UI/AppEditor/usePaginationMetrics.test.tsx`: se ampliaron expectativas para cubrir `visualPageBoundaries` y `visualContentHeight`.
- `node .\\node_modules\\vitest\\vitest.mjs --run src/app/Components/UI/AppEditor/AppEditor.test.tsx src/app/Components/UI/AppEditor/usePaginationMetrics.test.tsx src/app/Components/UI/AppEditor/usePageContext.test.tsx src/app/Components/UI/AppEditor/AppEditorToolbar.test.tsx src/app/Components/UI/AppEditor/useAppEditor.test.tsx src/app/Components/UI/AppEditor/resizableImage.extension.test.ts src/app/Components/UI/AppEditor/pageBreak.extension.test.ts` -> `7 files passed`, `42 tests passed`.
- `npx.cmd tsc -p tsconfig.app.json --noEmit` -> persisten errores preexistentes fuera del alcance del cambio en `src/app/Components/UI/AppTabs/AppTabs.tsx` y `src/setupTests.ts`.
