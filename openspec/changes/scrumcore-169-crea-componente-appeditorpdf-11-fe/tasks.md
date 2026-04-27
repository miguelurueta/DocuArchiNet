## 1. Contrato FE-11 (spec) y reutilizacion de soporte existente

- [x] 1.1 Definir el delta spec `app-appeditorpdf-11-fe` para alineacion horizontal de imagen.
- [x] 1.2 Confirmar soporte existente en `AppEditor` (extension de imagen + `data-align` + comandos).

## 2. Integracion en AppEditorPdf (sin duplicar logica)

- [x] 2.1 Verificar que `AppEditorPdf` no bloquee la UI de "Alineacion de imagen" del toolbar del editor base.
- [x] 2.2 Asegurar que la alineacion persiste como `data-align="left|center|right"` y conserva `data-width`.
- [x] 2.3 Validar compatibilidad con `paginationMode="visual"`.

## 3. Pruebas y validacion

- [x] 3.1 Agregar pruebas FE-11 (integracion) para confirmar contrato: control de alineacion disponible y no rompe wrapper.
- [x] 3.2 Reusar/ajustar pruebas de extension base si falta cobertura de `data-align`/`data-width` o round-trip.
- [ ] 3.3 Ejecutar `npm.cmd run test -- --run` y `npm.cmd run spec:validate`, dejando evidencia en el cambio.

## Evidencia (2026-04-27)

- Soporte base: `ResizableImage extension` persiste `data-align` + preserva `data-width`.
- UI: `AppEditorToolbar` expone alineacion horizontal de imagen y ejecuta `setImageAlign`.
- `npm.cmd run spec:validate`: OK.
- `npm.cmd run test -- --run`: falla en este entorno por `esbuild spawn EPERM` al cargar `vite.config.ts`.
