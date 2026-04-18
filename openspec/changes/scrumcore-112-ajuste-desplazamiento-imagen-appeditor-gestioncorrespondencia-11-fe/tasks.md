## 1. Extension de imagen con alineacion persistida

- [x] 1.1 Extender `resizable-image.extension.ts` con el atributo `align`
- [x] 1.2 Implementar `parseHTML` para `data-align`
- [x] 1.3 Implementar `renderHTML` preservando `data-align` y `data-width`
- [x] 1.4 Mantener `left` como valor por defecto para imagenes sin atributo

## 2. Comando e interaccion de editor

- [x] 2.1 Crear comando `setImageAlign('left' | 'center' | 'right')`
- [x] 2.2 Hacer que el comando actue solo sobre imagen activa o nodo imagen seleccionado
- [x] 2.3 Confirmar que el cambio de alineacion no rompe foco ni seleccion

## 3. Integracion con toolbar

- [x] 3.1 Mostrar controles de alineacion de imagen solo cuando la imagen este activa
- [x] 3.2 Agregar botones `left`, `center`, `right` en la toolbar contextual
- [x] 3.3 Confirmar que la toolbar no sufre regresion visual ni funcional

## 4. Render visual y compatibilidad

- [x] 4.1 Implementar CSS para `img[data-align="left|center|right"]`
- [x] 4.2 Confirmar compatibilidad con estilos de seleccion de imagen
- [x] 4.3 Confirmar que resize y alineacion conviven sin perder `data-width`

## 5. Pruebas y evidencia

- [x] 5.1 Agregar pruebas de extension para atributo `align`
- [x] 5.2 Agregar pruebas de serializacion y rehidratacion
- [x] 5.3 Agregar pruebas de comando `setImageAlign`
- [x] 5.4 Agregar pruebas de integracion en `AppEditor`
- [x] 5.5 Ejecutar pruebas focalizadas y registrar resultados
- [x] 5.6 Ejecutar validacion TypeScript o equivalente y registrar residuos ajenos si aparecen
- [x] 5.7 Registrar evidencia final en este archivo

## Evidencia

- Se extendio `ResizableImage` con el atributo persistido `align` y parsing/render de `data-align`.
- La extension ahora expone el comando `setImageAlign('left' | 'center' | 'right')`.
- La serializacion preserva simultaneamente `data-align` y `data-width`.
- `AppEditorToolbar` ahora muestra controles de alineacion de imagen solo cuando la imagen esta activa o seleccionada.
- `AppEditor.module.css` resuelve la posicion horizontal via `img[data-align="left|center|right"]` sin estilos inline de alineacion.
- Se agregaron pruebas en `resizableImage.extension.test.ts` para serializacion, rehidratacion y comando.
- Se actualizaron `AppEditorToolbar.test.tsx` y `AppEditor.test.tsx` para cubrir controles contextuales y rehidratacion de imagen alineada.
- Pruebas ejecutadas:
  - `npm test -- AppEditor.test.tsx AppEditorToolbar.test.tsx useAppEditor.test.tsx AppEditor.integration.test.tsx resizableImage.extension.test.ts` -> `5 files passed`, `27 tests passed`
  - `npx tsc -p tsconfig.app.json --noEmit` -> mantiene solo errores preexistentes ajenos al cambio en `src/app/Components/UI/AppTabs/AppTabs.tsx` y `src/setupTests.ts`
