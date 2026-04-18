## 1. Metricas de paginacion

- [x] 1.1 Crear el hook o helper de `application` para calcular metricas de paginacion visual (`usePaginationMetrics`)
- [x] 1.2 Medir `scrollHeight` de `.ProseMirror` solo cuando `paginationMode="visual"` este activo
- [x] 1.3 Calcular `pageContentHeight` a partir de la altura de pagina y los margenes superior e inferior
- [x] 1.4 Calcular `totalPages` con `ceil(contentHeight / pageContentHeight)`
- [x] 1.5 Evitar recalculo innecesario cuando las metricas no cambian realmente

## 2. Overlay y guias visuales

- [x] 2.1 Ajustar `presentation/AppEditor.tsx` para renderizar el overlay de guias solo en modo visual
- [x] 2.2 Extender `AppEditor.module.css` con una capa absoluta de guias fuera de `ProseMirror`
- [x] 2.3 Dibujar una guia visual por cada limite de pagina calculado
- [x] 2.4 Asegurar `pointer-events: none` para no bloquear interaccion ni seleccion

## 3. Recalculo y performance

- [x] 3.1 Sincronizar mediciones con `requestAnimationFrame`, `useLayoutEffect` o mecanismo equivalente
- [x] 3.2 Aplicar debounce entre `16ms` y `50ms` para evitar trabajo excesivo por `keypress`
- [x] 3.3 Recalcular metricas cuando cambie el contenido o el tamaño del contenedor paginado
- [x] 3.4 Confirmar que modo continuo no incurre en costo de medicion

## 4. Integracion y regresion

- [x] 4.1 Verificar que toolbar, escritura, seleccion, links e imagenes sigan funcionando en modo visual con guias
- [x] 4.2 Confirmar que el HTML serializado por `value` y `onChange` siga libre de metadata de guias
- [x] 4.3 Validar que las guias no rompan foco, scroll ni experiencia de edicion

## 5. Pruebas y evidencia

- [x] 5.1 Agregar o ajustar pruebas para el calculo de paginas estimadas
- [x] 5.2 Agregar o ajustar pruebas para el render del overlay y de las guias visuales
- [x] 5.3 Ejecutar pruebas focalizadas de `AppEditor` y registrar resultados
- [x] 5.4 Ejecutar validacion TypeScript o equivalente y registrar residuos ajenos si aparecen
- [x] 5.5 Registrar evidencia final en este archivo

## Evidencia

- Se agrego `usePaginationMetrics` en `application` para medir `scrollHeight` de `.ProseMirror`, calcular `pageContentHeight`, `totalPages` y `guideOffsets`, con actualizacion solo en modo `paginationMode="visual"`.
- `AppEditor.tsx` ahora consume esas metricas y renderiza un overlay absoluto de guias visuales dentro de la hoja paginada sin tocar el documento editable.
- `AppEditor.module.css` incorpora `pageGuides` y `pageGuide` con `pointer-events: none`, manteniendo la interaccion del editor intacta.
- Se ajusto el toolbar del editor con comportamiento `sticky` dentro del `frame` para evitar que se pierda visualmente al navegar documentos largos en modo paginado.
- Se agrego `usePaginationMetrics.test.tsx` para validar el calculo puro de paginas estimadas.
- Se ajusto `AppEditor.test.tsx` para validar que aparecen guias visuales cuando el contenido medido supera multiples paginas.
- La serializacion HTML del editor se mantiene libre de metadata de guias porque el overlay vive fuera de `.ProseMirror` y fuera del flujo serializado de Tiptap.
- Pruebas ejecutadas:
  - `npm test -- AppEditor.test.tsx AppEditorToolbar.test.tsx useAppEditor.test.tsx AppEditor.integration.test.tsx usePaginationMetrics.test.tsx` -> `5 files passed`, `22 tests passed`
  - `npx tsc -p tsconfig.app.json --noEmit` -> mantiene errores preexistentes ajenos al cambio en `src/app/Components/UI/AppTabs/AppTabs.tsx` y `src/setupTests.ts`
