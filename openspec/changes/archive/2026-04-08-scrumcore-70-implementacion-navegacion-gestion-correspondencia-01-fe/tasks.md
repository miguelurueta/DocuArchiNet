## 1. Shell Layout

- [x] 1.1 Redefinir `GestionCorrespondenciaRoute` para que renderice un shell persistente del módulo en lugar del patrón actual basado únicamente en `Drawer`
- [x] 1.2 Mantener `GestionCorrespondenciaRoutePage` como región principal estable del shell
- [x] 1.3 Renderizar la vista secundaria de la ruta hija dentro de una región persistente del layout cuando exista `drawerContent` o contenido equivalente
- [x] 1.4 Conservar el cierre de la región secundaria mediante navegación a `/dashboard/gestion-correspondencia`

## 2. Secondary View Integration

- [x] 2.1 Adaptar `GestionRespuesta` para que funcione correctamente dentro de la nueva región secundaria persistente
- [x] 2.2 Mantener `GestionRespuesta` desacoplada de la lógica de rutas y del mecanismo de apertura/cierre
- [x] 2.3 Preservar deep-linking para `/dashboard/gestion-correspondencia/respuesta`
- [x] 2.4 Preparar la composición para futuras vistas secundarias sin acoplarla a un único caso funcional

## 3. Styling and Responsive Behavior

- [x] 3.1 Crear o ajustar estilos del módulo para soportar el shell principal + detalle secundario
- [x] 3.2 Asegurar que el listado principal siga siendo legible y usable con la región secundaria visible
- [x] 3.3 Definir comportamiento responsivo para pantallas reducidas sin romper la navegación gobernada por routing
- [x] 3.4 Evitar que el layout del shell invada responsabilidades de `AppTable`, `AppToolbar` o `AppTableQueryWrapper`

## 4. Tests

- [x] 4.1 Actualizar pruebas de `GestionCorrespondenciaRoute` para validar el shell persistente en lugar del `Drawer`
- [x] 4.2 Verificar que la vista principal permanece visible cuando la subruta secundaria está activa
- [x] 4.3 Verificar que la región secundaria se abre por routing y se cierra navegando a la ruta base
- [x] 4.4 Verificar deep-linking a la subruta secundaria
- [x] 4.5 Verificar que `GestionRespuesta` renderiza dentro de la región secundaria sin reemplazar la principal
- [x] 4.6 Agregar o ajustar pruebas para el comportamiento responsivo observable si aplica

## 5. Documentation and Validation

- [x] 5.1 Actualizar `src/modules/gestionCorrespondencia/README.md` para documentar el shell persistente gobernado por routing
- [x] 5.2 Ejecutar Vitest focal sobre rutas y páginas afectadas
- [x] 5.3 Ejecutar ESLint focal sobre archivos tocados
- [x] 5.4 Ejecutar validación TypeScript si el cambio toca tipos o composición compartida
- [x] 5.5 Ejecutar `openspec validate scrumcore-70-implementacion-navegacion-gestion-correspondencia-01-fe --strict`
- [x] 5.6 Ejecutar `git diff --check`
- [x] 5.7 Registrar evidencia de validación en este checklist antes del archive

## Evidencia

- `npm.cmd test -- src/modules/gestionCorrespondencia/routes/GestionCorrespondenciaRoute.spec.test.tsx src/modules/gestionCorrespondencia/tests/GestionCorrespondenciaRoutePage.test.tsx src/modules/gestionCorrespondencia/tests/GestionCorrespondencia.test.tsx` -> `9 files`, `28 tests` OK
- `npx.cmd eslint src/modules/gestionCorrespondencia/routes/GestionCorrespondenciaRoute.tsx src/modules/gestionCorrespondencia/routes/GestionCorrespondenciaRoute.spec.test.tsx src/modules/gestionCorrespondencia/pages/GestionRespuesta.tsx src/app/routes/routes.tsx src/modules/gestionCorrespondencia/tests/GestionCorrespondenciaRoutePage.test.tsx src/modules/gestionCorrespondencia/tests/GestionCorrespondencia.test.tsx` -> OK
- `npx.cmd tsc -b` -> OK
- `npx.cmd openspec validate scrumcore-70-implementacion-navegacion-gestion-correspondencia-01-fe --strict` -> OK
- `git diff --check` -> OK
