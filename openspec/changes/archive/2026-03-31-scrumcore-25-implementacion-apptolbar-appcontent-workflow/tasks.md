## 1. Composicion de UI en Workflow

- [x] 1.1 Actualizar `Workflow.tsx` para renderizar `AppToolbar` y `AppContent` en el orden obligatorio
- [x] 1.2 Integrar `AppButton` y `AppDropdown` con acciones superiores requeridas
- [x] 1.3 Implementar placeholder de tabla dentro de `AppContent` preparado para AG Grid / Ant Design

## 2. Estilos responsivos y overflow

- [x] 2.1 Ajustar `Workflow.module.css` con reglas de layout, wrap y spacing responsive
- [x] 2.2 Asegurar que el contenedor ocupa todo el alto disponible y evita overflow horizontal inesperado
- [x] 2.3 Ajustar altura minima del toolbar a `55px` para mejor alineacion horizontal

## 3. Layout y pruebas

- [x] 3.1 Verificar que `WorkflowLayout.tsx` mantiene solo estructura y delega a `Workflow.tsx`
- [x] 3.2 Agregar pruebas para toolbar, content y orden de render `[SPEC:IMPLEMENTACION-APPTOLBAR-APPCONTENT-WORKFLOW]`
- [x] 3.3 Validar comportamiento responsive y manejo de contenido grande en tests o evidencia documentada

## Evidencia de pruebas

- `npm.cmd test -- --run src/modules/Workflow/pages/Workflow.spec.test.tsx`
- `npm.cmd test -- --run src/modules/Workflow/routes/WorkflowRoute.spec.test.tsx`
