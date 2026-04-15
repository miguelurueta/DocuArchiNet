## 1. Infraestructura IndexedDB reusable

- [x] 1.1 Crear carpeta `src/app/Components/UI/AppEditor/infrastructure/indexeddb/`
- [x] 1.2 Implementar inicializacion de base y versionado
- [x] 1.3 Implementar object store para imagenes locales
- [x] 1.4 Implementar operaciones `saveImage`, `getImage`, `deleteImage` y `clearByScope`
- [x] 1.5 Asegurar que el adaptador sea puro, testeable y sin dependencia de React

## 2. Modelo temporal de imagen

- [x] 2.1 Definir tipo `LocalImage` con metadata minima y `blob`
- [x] 2.2 Implementar generador de ids `img_local_<uuid>`
- [x] 2.3 Incorporar soporte opcional para `documentDraftId` y `sessionId`

## 3. Integracion con AppEditor

- [x] 3.1 Orquestar en `application` el flujo de insercion de imagen local
- [x] 3.2 Guardar imagen local en `IndexedDB` al seleccionar archivo
- [x] 3.3 Generar `blob:` URL valida para render en el editor
- [x] 3.4 Insertar nodo imagen con `data-local-image-id` y `data-source="local"`
- [x] 3.5 Mantener intacta la insercion por URL remota

## 4. Gestion de Object URLs

- [x] 4.1 Registrar las `blob:` URLs activas creadas para imagenes locales
- [x] 4.2 Revocar `Object URLs` al eliminar o reemplazar imagenes
- [x] 4.3 Revocar `Object URLs` al desmontar el editor

## 5. Rehidratacion basica en sesion

- [x] 5.1 Detectar imagenes con `data-local-image-id` al cargar HTML
- [x] 5.2 Resolver blobs desde `IndexedDB` para regenerar `blob:` URLs
- [x] 5.3 Fallar de forma segura cuando la imagen temporal ya no exista

## 6. Compatibilidad y pruebas

- [x] 6.1 Confirmar compatibilidad con resize persistido (`data-width`)
- [x] 6.2 Confirmar compatibilidad con alineacion horizontal (`data-align`)
- [x] 6.3 Agregar pruebas del adaptador `IndexedDB` (CRUD)
- [x] 6.4 Agregar pruebas de generacion de ids locales
- [x] 6.5 Agregar pruebas de insercion de imagen local y serializacion HTML
- [x] 6.6 Agregar pruebas de liberacion de `Object URLs`
- [x] 6.7 Ejecutar pruebas focalizadas y registrar resultados
- [x] 6.8 Ejecutar validacion TypeScript o equivalente y registrar residuos ajenos si aparecen

## Evidencia

- Se creo el adaptador `appEditorImageStore` en `infrastructure/indexeddb/` con soporte para inicializacion, versionado, object store y operaciones `saveImage`, `getImage`, `deleteImage` y `clearByScope`.
- Se definio el modelo `LocalImage` y el generador `generateLocalImageId()` con prefijo `img_local_`.
- `ResizableImage` ahora preserva `data-local-image-id` y `data-source` junto con `data-width` y `data-align`.
- `useAppEditor` orquesta la insercion local: guarda el archivo en `IndexedDB`, crea `blob:` URL, inserta la imagen en Tiptap y rehidrata imagenes locales al cargar HTML.
- `useAppEditor` mantiene un registro de Object URLs activas y las revoca cuando desaparecen del documento o al desmontar el editor.
- `AppEditorToolbar` delega la carga de imagen local al hook, manteniendo intacta la insercion por URL remota.
- Se agregaron pruebas en:
  - `appEditorImageStore.test.ts`
  - `localImageIds.test.ts`
  - `resizableImage.extension.test.ts`
  - `AppEditorToolbar.test.tsx`
  - `AppEditor.test.tsx`
- Pruebas ejecutadas:
  - `npm test -- AppEditor.test.tsx AppEditorToolbar.test.tsx useAppEditor.test.tsx resizableImage.extension.test.ts appEditorImageStore.test.ts localImageIds.test.ts` -> `6 files passed`, `35 tests passed`
  - `npx tsc -p tsconfig.app.json --noEmit` -> mantiene solo errores preexistentes ajenos al cambio en `src/app/Components/UI/AppTabs/AppTabs.tsx` y `src/setupTests.ts`
