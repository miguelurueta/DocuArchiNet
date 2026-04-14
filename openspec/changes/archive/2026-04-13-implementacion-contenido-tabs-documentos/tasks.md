## 1. Preparacion de estructura

- [x] 1.1 Crear carpeta `src/modules/gestionCorrespondencia/components/documentosWorkbench/`
- [x] 1.2 Definir archivos base del workbench (`DocumentosWorkbench.tsx`, `.module.css`, `index.ts`)
- [x] 1.3 Definir subcomponentes presentacionales (`DocumentosToolbar`, `DocumentosList`, `DocumentosPreview`)

## 2. Layout del workbench

- [x] 2.1 Implementar layout en columna con `AppToolbar` arriba
- [x] 2.2 Implementar zona principal en fila con area principal y `AppCollapseRail`
- [x] 2.3 Aplicar scroll independiente en area principal y panel lateral

## 3. Estado y responsive

- [x] 3.1 Implementar estado controlado `collapsed` y handler `onToggle`
- [x] 3.2 Aplicar `variant="inline"` en desktop y `collapsed=false` por defecto
- [x] 3.3 Aplicar `collapsed=true` en tablet por defecto
- [x] 3.4 Aplicar `variant="overlay"` y rail chip en mobile

## 4. Accesibilidad

- [x] 4.1 Asegurar `aria-expanded` y `aria-controls` en toggles
- [x] 4.2 Garantizar foco visible en rail y acciones principales

## 5. Integracion en tabs

- [x] 5.1 Renderizar `DocumentosWorkbench` dentro del tab **Documentos** en `GestionRespuesta.tsx`
- [x] 5.2 Validar que el tab **Gestion** no se modifica

## 6. Pruebas

- [x] 6.1 Crear pruebas unitarias de render y toggle del panel
- [x] 6.2 Cubrir estados responsive (desktop/tablet/mobile)
- [x] 6.3 Registrar evidencia de tests ejecutados en el change

## Evidencia de pruebas

- `node .\\node_modules\\vitest\\vitest.mjs --run src/modules/gestionCorrespondencia/tests/DocumentosWorkbench.test.tsx` (2026-04-13)
