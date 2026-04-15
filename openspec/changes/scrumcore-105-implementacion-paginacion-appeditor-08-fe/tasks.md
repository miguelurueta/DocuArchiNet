## 1. Contexto de pagina actual

- [x] 1.1 Crear el hook o helper de `application` para calcular la pagina actual (`usePageContext`)
- [x] 1.2 Resolver `totalPages` reutilizando las metricas de paginacion ya disponibles
- [x] 1.3 Calcular `currentPage` con `pageIndex = floor(offset / pageContentHeight) + 1`
- [x] 1.4 Acotar `currentPage` entre `1` y `totalPages`
- [x] 1.5 Evitar `setState` innecesario cuando la pagina actual no cambie

## 2. Prioridad cursor y fallback scroll

- [x] 2.1 Resolver pagina actual por cursor cuando el editor tenga foco y exista seleccion valida
- [x] 2.2 Usar `editor.view.coordsAtPos(selection.from)` como fuente primaria para la coordenada del cursor
- [x] 2.3 Implementar fallback por scroll cuando no sea posible resolver la pagina desde el cursor
- [x] 2.4 Proteger la resolucion por cursor frente a errores o coordenadas invalidas

## 3. Presentacion del contador

- [x] 3.1 Ajustar `presentation/AppEditor.tsx` para renderizar el contador solo en `paginationMode="visual"`
- [x] 3.2 Extender `AppEditor.module.css` con estilos discretos para el contador en la esquina inferior derecha
- [x] 3.3 Confirmar que el contador no compita visualmente con la toolbar ni bloquee interaccion sobre el editor

## 4. Performance e integracion

- [x] 4.1 Aplicar debounce o sincronizacion estable para eventos de scroll del contenedor paginado
- [x] 4.2 Validar que escritura, seleccion, toolbar y scroll sigan funcionando sin regresion
- [x] 4.3 Confirmar que el HTML serializado permanezca libre de metadata del contador

## 5. Pruebas y evidencia

- [x] 5.1 Agregar o ajustar pruebas para el calculo de `currentPage`
- [x] 5.2 Agregar o ajustar pruebas para prioridad cursor / fallback scroll
- [x] 5.3 Agregar o ajustar pruebas para el render del contador `Pagina X de Y`
- [x] 5.4 Ejecutar pruebas focalizadas de `AppEditor` y registrar resultados
- [x] 5.5 Ejecutar validacion TypeScript o equivalente y registrar residuos ajenos si aparecen
- [x] 5.6 Registrar evidencia final en este archivo

## Evidencia

- Se agrego `usePageContext` en `application` para calcular la pagina actual reutilizando `totalPages` y `pageContentHeight` ya disponibles desde las metricas de paginacion.
- El hook prioriza la resolucion por cursor con `editor.view.coordsAtPos(selection.from)` cuando el editor tiene foco y usa scroll como fallback cuando la coordenada no es valida o no hay foco.
- `AppEditor.tsx` ahora renderiza un contador discreto `Pagina X de Y` solo en `paginationMode="visual"`.
- `AppEditor.module.css` incorpora estilos del contador en la esquina inferior derecha del shell paginado sin bloquear interaccion del editor.
- Se agrego `usePageContext.test.tsx` para validar calculo de pagina actual y fallback por scroll.
- Se ajusto `AppEditor.test.tsx` para validar el render del contador `Pagina X de Y` en modo visual.
- El HTML serializado del editor se mantiene libre de metadata del contador porque el contador vive fuera de `.ProseMirror` y fuera del flujo serializado de Tiptap.
- Pruebas ejecutadas:
  - `npm test -- AppEditor.test.tsx AppEditorToolbar.test.tsx useAppEditor.test.tsx AppEditor.integration.test.tsx usePaginationMetrics.test.tsx usePageContext.test.tsx` -> `6 files passed`, `25 tests passed`
  - `npx tsc -p tsconfig.app.json --noEmit` -> mantiene errores preexistentes ajenos al cambio en `src/app/Components/UI/AppTabs/AppTabs.tsx` y `src/setupTests.ts`
