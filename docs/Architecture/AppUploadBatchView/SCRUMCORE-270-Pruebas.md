# SCRUMCORE-270 - Pruebas

## Matriz

| Tipo | Cobertura |
| --- | --- |
| Unitarias | lista vacia, resumen, contador, render de archivos, estados, callbacks, slots, warning/error |
| Integracion | composicion con `AppUpload`, seleccion de archivos, preview por `selectedUid`, object URL cleanup |
| Responsive | CSS module con layout desktop/mobile |
| Regresion | `AppUpload` no se modifica y se consume por contrato existente |

## Casos cubiertos por test

- Export publico desde `AppUploadBatchView/index.ts`.
- Lista vacia y mensaje configurable.
- Nombre, tamano, estado, progreso y fila activa.
- Acciones globales y por archivo.
- `onFilesSelected` desde `AppUpload`.
- Estados disabled/loading/can*.
- Slots de metadata, preview, nombre y footer.
- Warning y error por archivo.
- Preview default PDF, imagen y fallback.
- Revocacion de object URL.

## Comandos Ejecutados

```txt
npx.cmd vitest run src/app/Components/UI/AppUploadBatchView/AppUploadBatchView.test.tsx --environment jsdom --isolate=false --reporter verbose
npx.cmd tsc --noEmit --pretty false
npx.cmd eslint src/app/Components/UI/AppUploadBatchView/AppUploadBatchView.tsx src/app/Components/UI/AppUploadBatchView/AppUploadBatchView.test.tsx
npx.cmd openspec validate scrumcore-270-crea-componente-appuploadbatchview --strict
```

## Resultados

- `vitest`: passed, 1 archivo, 10 tests.
- `tsc --noEmit`: passed.
- `eslint` enfocado: passed.
- `openspec validate --strict`: passed. El CLI imprimio warnings de telemetria PostHog por red restringida; no afectan la validez del cambio.

## Riesgos residuales

- La vista no valida negocio; el consumidor debe mantener validaciones, persistencia y progreso real.
- `AppUpload` conserva su comportamiento interno. Este ticket solo lo compone.
- La seleccion nativa de archivos se canaliza mediante `beforeUpload` para usar `AppUpload` como selector controlado y evitar lista interna duplicada.
