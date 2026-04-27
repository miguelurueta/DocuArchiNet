## 1. Activacion del documento paginado real

- [x] 1.1 Forzar `paginatedDocument` cuando `paginationMode="visual"` este activo en `useAppEditor.ts`
- [x] 1.2 Asegurar que `PageDocument` y `PageNode` sean el schema canonico del modo visual en `tiptap.config.ts` y `tiptap.extensions.ts`
- [x] 1.3 Mantener `paginationMode="none"` como flujo continuo sin wrappers de pagina ni regresiones de contrato

## 2. Migracion de entrada y serializacion estable

- [x] 2.1 Normalizar HTML plano, HTML con `pageBreak` manual y HTML con `data-app-editor-page="true"` al arbol `doc -> page -> blocks` en `pageDocument.ts`
- [x] 2.2 Mantener `serializeVisualPageHtml` y `normalizeEditorHtml` libres de `data-page-break-auto`, `spacerHeight` y metadata legacy equivalente
- [x] 2.3 Verificar roundtrip estable de `value` y `onChange` para modo controlado y no controlado en el camino paginado real

## 3. Render de hojas reales y metricas de pagina

- [x] 3.1 Reemplazar el camino principal de `AppEditor.tsx` basado en `contentFlow` y `pageShells` por hojas reales renderizadas desde los nodos `page`
- [x] 3.2 Ajustar `AppEditor.module.css` para que `.ProseMirror > [data-app-editor-page="true"]` represente la hoja real, su gap y su area util
- [x] 3.3 Promover en `usePaginationMetrics.ts` la lectura basada en paginas reales como fuente primaria de `totalPages`, `visualPageBoundaries` y `visualContentHeight`
- [x] 3.4 Validar que `usePageContext` y el contador de pagina sigan alineados con la estructura real de hojas y con el zoom

## 4. Continuidad basica entre paginas reales

- [x] 4.1 Implementar la creacion o reutilizacion de pagina siguiente cuando el contenido excede la hoja actual en escenarios basicos de escritura
- [x] 4.2 Implementar redistribucion basica por bloques para contenido agregado o pegado que ya no cabe en la pagina activa
- [x] 4.3 Mover bloques indivisibles completos a la siguiente hoja cuando no quepan en el espacio restante
- [x] 4.4 Mantener el borde inferior libre de overflow visible sin depender de correccion posterior por espaciadores legacy

## 5. Retiro del motor legacy del camino principal

- [x] 5.1 Sacar `autoPagination.ts` y `autoPageBreak.ts` de la ruta principal de `paginationMode="visual"`
- [x] 5.2 Mantener `PageBreak` manual solo como compatibilidad de migracion o persistencia externa mientras siga siendo necesario
- [x] 5.3 Evitar convivencia de dos motores paginados activos sobre el mismo flujo del editor

## 6. Compatibilidad funcional minima

- [x] 6.1 Verificar que toolbar, links e imagenes locales sigan operando sobre el documento paginado real
- [x] 6.2 Verificar que serializacion, apertura de contenido existente y zoom sigan funcionando con la nueva base
- [x] 6.3 Verificar que el cursor y la seleccion permanezcan estables en los escenarios basicos cubiertos por esta fase

## 7. Pruebas y evidencia

- [x] 7.1 Actualizar pruebas de `pageDocument`, `useAppEditor` y `usePaginationMetrics` para cubrir activacion permanente del schema paginado real
- [x] 7.2 Cubrir migracion desde HTML plano, desde `pageBreak` manual y desde wrappers reales sin residuos legacy
- [x] 7.3 Cubrir escritura al final de pagina, continuidad a la hoja siguiente y redistribucion basica de bloques
- [x] 7.4 Cubrir estabilidad de contador de pagina, zoom y modo continuo despues del cambio
- [x] 7.5 Ejecutar las pruebas relevantes y dejar evidencia del resultado en el flujo OpenSpec del cambio

Evidencia 7.5:
`npx vitest --run src/app/Components/UI/AppEditor/useAppEditor.test.tsx src/app/Components/UI/AppEditor/pageDocument.test.ts src/app/Components/UI/AppEditor/AppEditorSaveAction.test.tsx src/app/Components/UI/AppEditor/usePaginationMetrics.test.tsx src/app/Components/UI/AppEditor/AppEditorToolbar.test.tsx src/app/Components/UI/AppEditor/AppEditor.test.tsx src/app/Components/UI/AppEditor/usePageContext.test.tsx`
