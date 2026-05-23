# SCRUMCORE-229 — Pruebas

## Objetivo de pruebas
Validar que los cambios:

- Están **scopeados** al Workbench (no regresión en otras pantallas).
- Mantienen performance/scroll estable.
- Mantienen accesibilidad visual (focus visible).
- Mantienen layout y columnas (sin tocar sizing desde código).

## Ejecutadas
### Unit / Component
- Se ejecutaron pruebas focales del Workbench:
  - `npm.cmd test -- --run src/modules/gestionCorrespondencia/tests/DocumentosWorkbench.test.tsx`

## Pendientes / Recomendadas
### Playwright (recomendado para regresión UI)
1. Workbench renderiza headers esperados:
   - Deben existir al menos “Documento” y “acciones” cuando el backend las provee.
2. Click fila:
   - Click sobre fila marca `aria-selected="true"` y el color de selección se aplica a toda la fila.
3. Focus visible:
   - Navegación con teclado sobre celdas debe mostrar `outline` (no perder accesibilidad).
4. Líneas:
   - Verificar ausencia de separadores verticales (column lines) y presencia de separadores horizontales (row lines).

## Evidencia / comandos
- Los comandos exactos deben registrarse en el PR o en la ejecución local del pipeline (si aplica).

## Riesgos residuales
- CSS specificity: si AG Grid Quartz cambia su estructura/clases, puede requerir ajuste de overrides.
- Tooltip header: depende de implementación de AG Grid; en cambios de versión podría variar el trigger visual.

