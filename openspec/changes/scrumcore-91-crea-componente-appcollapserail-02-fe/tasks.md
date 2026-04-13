## 1. Refinamientos visuales

- [x] 1.1 Ajustar tokens de color, sombras y bordes para igualar GestionRespuesta
- [x] 1.2 Revisar estados hover/focus en rail y toggle
- [x] 1.3 Validar consistencia de radios en panel y rail

## 2. Responsive avanzado

- [x] 2.1 Asegurar comportamiento en tablet (colapsado por defecto desde contenedor)
- [x] 2.2 Revisar overlay mobile y rail chip con label visible
- [x] 2.3 Verificar placement left/right en mobile y desktop

## 3. Accesibilidad y UX

- [x] 3.1 Normalizar labels en toggle y rail para lector de pantalla
- [x] 3.2 Validar foco visible en todos los controles

## 4. Pruebas

- [x] 4.1 Tests de estilos/clases aplicadas por variant/placement
- [x] 4.2 Tests de accesibilidad (aria-expanded, aria-controls)
- [x] 4.3 Tests de comportamiento responsive (mock viewport si aplica)

## 5. Evidencia

- [x] 5.1 Registrar evidencia de ejecucion de tests en este archivo

## Evidencia de pruebas

- `node .\\node_modules\\vitest\\vitest.mjs --run src/app/Components/UI/AppCollapseRail/AppCollapseRail.test.tsx` (2026-04-13)
