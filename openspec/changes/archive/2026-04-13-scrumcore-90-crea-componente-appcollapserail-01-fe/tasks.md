## 1. Estructura base del componente

- [x] 1.1 Crear carpeta `src/app/Components/UI/AppCollapseRail/` con archivo principal `AppCollapseRail.tsx`
- [x] 1.2 Definir tipos `AppCollapseRailProps`, `AppCollapseRailPlacement`, `AppCollapseRailVariant`
- [x] 1.3 Incluir props `railLabel`, `railIcon`, `headerActions` y `className` en el contrato
- [x] 1.4 Exportar componente desde `src/app/Components/UI/index.ts`

## 2. Comportamiento de colapso

- [x] 2.1 Implementar panel con header, surface y rail de restauracion
- [x] 2.2 Aplicar `aria-expanded` y `aria-controls` en el toggle
- [x] 2.3 Soportar `placement="left|right"` mediante clases y data-attrs
- [x] 2.4 Mantener contenido montado cuando el panel se colapsa

## 3. Estilos y responsive

- [x] 3.1 Crear `AppCollapseRail.module.css` con estilos base (panel/rail)
- [x] 3.2 Implementar variantes inline/overlay segun `variant`
- [x] 3.3 Ajustar breakpoints desktop/tablet/mobile segun arquitectura

## 4. Pruebas

- [x] 4.1 Tests de render y accesibilidad ([SPEC:APP-COLLAPSE-RAIL-001])
- [x] 4.2 Tests de toggle/rail visible ([SPEC:APP-COLLAPSE-RAIL-002])
- [x] 4.3 Tests de persistencia de contenido ([SPEC:APP-COLLAPSE-RAIL-003])
- [x] 4.4 Tests de `placement` y rail label/icon cuando aplique

## 5. Documentacion y evidencia

- [x] 5.1 README breve con ejemplos desktop y mobile
- [x] 5.2 Registrar evidencia de ejecucion de tests en este archivo

## Evidencia de pruebas

- `node .\\node_modules\\vitest\\vitest.mjs --run src/app/Components/UI/AppCollapseRail/AppCollapseRail.test.tsx` (2026-04-13)
