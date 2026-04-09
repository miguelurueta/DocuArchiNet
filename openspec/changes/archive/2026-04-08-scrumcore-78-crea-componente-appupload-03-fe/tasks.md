## 1. Telemetry

- [x] 1.1 Exponer callback `onTelemetry(event)` con payload estandar
- [x] 1.2 Emitir eventos minimos: select, upload_start, upload_success, upload_error, remove, preview_open, cancel

## 2. Documentacion

- [x] 2.1 Crear `README.md` en AppUpload con props y eventos
- [x] 2.2 Agregar ejemplos: auto, manual, customRequest

## 3. Accesibilidad

- [x] 3.1 Validar aria-labels y focus visible en acciones
- [x] 3.2 Tests de accesibilidad (teclado, aria)
- [x] 3.3 Ejecutar tests y registrar evidencia

## Evidencia

- `vitest.cmd --run src/app/Components/UI/AppUpload/AppUpload.test.tsx` -> `10 passed`
