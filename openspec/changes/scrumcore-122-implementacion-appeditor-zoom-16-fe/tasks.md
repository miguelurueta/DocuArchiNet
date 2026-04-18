## 1. API y estado de zoom

- [x] 1.1 Extender `editor.types.ts` con props de zoom (`zoomLevel`, `defaultZoomLevel`, `minZoomLevel`, `maxZoomLevel`, `onZoomChange`)
- [x] 1.2 Resolver modo controlado y no controlado en `AppEditor` sin breaking changes
- [x] 1.3 Definir valor por defecto `100%` y limites operativos del zoom visual
- [x] 1.4 Confirmar que `paginationMode="none"` no adopta comportamiento ni UI de zoom

## 2. Control UI y presentacion del zoom

- [x] 2.1 Renderizar el control de zoom solo en `paginationMode="visual"`
- [x] 2.2 Exponer UI minima con decremento, valor actual y aumento
- [x] 2.3 Integrar el control en el frame del editor sin recargar la toolbar principal
- [x] 2.4 Ajustar `AppEditor.module.css` para materializar el zoom visual sin desalinear hoja, overlays ni contador

## 3. Integracion con paginacion, scroll y page context

- [x] 3.1 Revisar `usePaginationMetrics.ts` para integrar `zoomLevel` al recalculo del layout visual
- [x] 3.2 Revisar `usePageContext.ts` para mantener estable `Pagina X de Y` bajo cambios de zoom
- [x] 3.3 Confirmar que el scroll continuo sigue ocurriendo en el `canvas`
- [x] 3.4 Verificar que el cambio de zoom no introduce jitter, flicker ni desalineacion perceptible

## 4. Compatibilidad con interaccion y contenido existente

- [x] 4.1 Confirmar que cursor y seleccion siguen estables tras variar el zoom
- [x] 4.2 Confirmar compatibilidad con `PageBreak` manual
- [x] 4.3 Confirmar compatibilidad con imagenes locales/remotas, resize y `data-align`
- [x] 4.4 Confirmar que HTML serializado y atributos persistidos no cambian por efecto del zoom

## 5. Pruebas y evidencia

- [x] 5.1 Agregar o ajustar pruebas del control UI de zoom en modo visual
- [x] 5.2 Agregar pruebas de limites min/max y valor por defecto
- [x] 5.3 Agregar pruebas de no regresion para `paginationMode="none"`
- [x] 5.4 Agregar pruebas de compatibilidad con contador, `PageBreak` e imagenes
- [x] 5.5 Ejecutar pruebas focalizadas del editor y registrar resultados
- [x] 5.6 Ejecutar validacion TypeScript, lint o equivalente y registrar residuos ajenos si aparecen
- [x] 5.7 Registrar evidencia final en este archivo

## Evidencia

- `src/app/Components/UI/AppEditor/domain/editor.types.ts`: se agregaron props de zoom visual (`zoomLevel`, `defaultZoomLevel`, `minZoomLevel`, `maxZoomLevel`, `onZoomChange`) sin romper la API previa del editor.
- `src/app/Components/UI/AppEditor/presentation/AppEditor.tsx`: se resolvio estado controlado/no controlado de zoom, se agrego el control UI solo para `paginationMode="visual"` y se integro una etapa visual escalada para la hoja paginada.
- `src/app/Components/UI/AppEditor/AppEditor.module.css`: se agregaron estilos del control de zoom y de la capa `zoomStage` para escalar la experiencia paginada completa sin tocar el HTML persistido.
- `src/app/Components/UI/AppEditor/application/usePaginationMetrics.ts`: se integró `zoomLevel` al ciclo de recálculo del layout visual para mantener sincronía con el modo paginado.
- `src/app/Components/UI/AppEditor/application/usePageContext.ts`: se ajustó el cálculo de `Pagina X de Y` para interpretar límites visuales escalados bajo zoom.
- `src/app/Components/UI/AppEditor/AppEditor.test.tsx`: se agregaron pruebas para visibilidad del control, valor por defecto, límites min/max y modo controlado sin mutación de contenido.
- `src/app/Components/UI/AppEditor/usePageContext.test.tsx`: se agregó cobertura para límites visuales escalados por zoom.
- `node .\\node_modules\\vitest\\vitest.mjs --run src/app/Components/UI/AppEditor/AppEditor.test.tsx src/app/Components/UI/AppEditor/usePaginationMetrics.test.tsx src/app/Components/UI/AppEditor/usePageContext.test.tsx src/app/Components/UI/AppEditor/AppEditorToolbar.test.tsx src/app/Components/UI/AppEditor/useAppEditor.test.tsx src/app/Components/UI/AppEditor/resizableImage.extension.test.ts src/app/Components/UI/AppEditor/pageBreak.extension.test.ts` -> `7 files passed`, `46 tests passed`.
- `npx.cmd tsc -p tsconfig.app.json --noEmit` -> persisten solo errores preexistentes fuera del alcance del cambio en `src/app/Components/UI/AppTabs/AppTabs.tsx` y `src/setupTests.ts`.
