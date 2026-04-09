## 1. Preparacion y estructura

- [x] 1.1 Crear carpeta base `src/app/Components/UI/AppUpload/`
- [x] 1.2 Definir tipos `AppUploadFile` y `AppUploadProps` en el componente

## 2. Core controlado y contrato

- [x] 2.1 Implementar wrapper base de AntD Upload con estado controlado via `value`
- [x] 2.2 Implementar `onChange`, `onRemove`, `onUpload` y slots/render props
- [x] 2.3 Aplicar limite `maxCount` y mantener orden estable

## 3. Validaciones y state machine

- [x] 3.1 Implementar validaciones `accept`, `maxSize`, `validateFile`
- [x] 3.2 Implementar state machine estricta `queued -> uploading -> done/error -> removed`
- [x] 3.3 Exponer `onProgress`, `onSuccess`, `onError` con `percent` 0-100
- [x] 3.4 Implementar `retry(file)` y `abort(file)`

## 4. Estrategias de carga

- [x] 4.1 Implementar estrategia `auto`
- [x] 4.2 Implementar estrategia `manual`
- [x] 4.3 Implementar `customRequest` (incluye presigned via adaptor)

## 5. UI/UX y responsive

- [x] 5.1 Implementar preview tipo galeria con fallback por tipo
- [x] 5.2 Implementar cards 1:1 con bordes suaves y hover elevation
- [x] 5.3 Implementar layout `grid`/`list` responsive (46/23/2 columnas)
- [x] 5.4 Implementar drag & drop con estados validos/invalidos

## 6. Accesibilidad y performance

- [x] 6.1 Agregar soporte teclado (Enter preview, Delete remove) + focus visible
- [x] 6.2 Agregar `aria-label` en acciones principales
- [x] 6.3 Aplicar `React.memo` en items y evitar re-render completo de lista

## 7. Pruebas y evidencia

- [x] 7.1 Tests unitarios para contrato controlado, validaciones y eventos
- [x] 7.2 Tests unitarios de state machine, retry/cancel y progreso
- [x] 7.3 Tests UI/UX: preview, drag & drop, responsive
- [x] 7.4 Ejecutar tests y registrar evidencia en el cambio OpenSpec

## Evidencia

- `vitest.cmd --run src/app/Components/UI/AppUpload/AppUpload.test.tsx` -> `9 passed`
