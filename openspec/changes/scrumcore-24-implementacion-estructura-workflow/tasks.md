## 1. Estructura del modulo Workflow

- [x] 1.1 Crear `src/modules/Workflow/layout/WorkflowLayout.tsx` con Ant Design y `Outlet`
- [x] 1.2 Crear `src/modules/Workflow/pages/Workflow.tsx` con placeholders profesionales
- [x] 1.3 Crear `src/modules/Workflow/pages/WorkflowAsignacion.tsx` placeholder para Drawer
- [x] 1.4 Crear `src/modules/Workflow/pages/WorkflowEnlace.tsx` placeholder para Drawer
- [x] 1.5 Crear `src/modules/Workflow/routes/WorkflowRoute.tsx` con patron Outlet + Drawer
- [x] 1.6 Crear `src/modules/Workflow/README.md` con proposito, estructura y flujo

## 2. Integracion de routing

- [x] 2.1 Integrar rutas del modulo Workflow en `src/app/routes/routes.tsx`
- [x] 2.2 Asegurar que la ruta principal mantiene contexto cuando se abren drawers por rutas hijas

## 3. Pruebas

- [x] 3.1 Agregar pruebas de layout y pagina principal `[SPEC:IMPLEMENTACION-ESTRUCTURA-WORKFLOW]`
- [x] 3.2 Agregar pruebas de Drawer para `WorkflowAsignacion` y `WorkflowEnlace`
- [x] 3.3 Ejecutar pruebas relevantes y documentar evidencia en OpenSpec

## Evidencia de pruebas

- `npm.cmd test -- --run src/modules/Workflow/routes/WorkflowRoute.spec.test.tsx`
