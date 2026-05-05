## 1. Descubrimiento y limpieza de referencias

- [x] 1.1 Identificar entry-points del visor (`rg` por `AppVisorEmbedPdf|VisorEmbedPdf|embedpdf|@embedpdf`) y listar archivos afectados
- [x] 1.2 Remover imports/exports/wiring del visor en UI/rutas/barrels/tests (sin dejar stubs rotos)

## 2. Eliminación de código del componente

- [x] 2.1 Eliminar carpeta `src/app/Components/UI/AppVisorEmbedPdf` y ajustar cualquier `index.ts`/export asociado
- [x] 2.2 Asegurar que pantallas que lo referenciaban degradan de forma controlada (remover acción o alternativa existente)

## 3. Eliminación de dependencias y lockfile

- [x] 3.1 Remover `@embedpdf/*` del `package.json` (solo si ya no hay usos)
- [x] 3.2 Ejecutar `npm install` para actualizar `package-lock.json`
- [x] 3.3 Verificar que no quedan referencias a `@embedpdf`/`embedpdf` en `src/**`, `package.json` y `package-lock.json`

## 4. Validación (build/tests) y evidencia

- [x] 4.1 Ejecutar `npm run build` y corregir errores de compilación por referencias residuales
- [x] 4.2 Ejecutar `npm test` (o `npm.cmd test`) y ajustar tests afectados por la eliminación
- [x] 4.3 Documentar evidencia de validación (comandos ejecutados y resultado) en el change log o notes del cambio
