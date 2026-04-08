## 1. Secondary Shell Experience

- [x] 1.1 Revisar `GestionCorrespondenciaRoute` para decidir dónde vive la acción dominante de retorno/cierre sin romper el shell persistente existente
- [x] 1.2 Ajustar la composición del header o región secundaria para que el retorno al listado sea más evidente y consistente con el patrón master-detail
- [x] 1.3 Mantener el cierre gobernado por navegación a `/dashboard/gestion-correspondencia`

## 2. GestionRespuesta Refinement

- [x] 2.1 Refinar `GestionRespuesta` para que el contenido secundario exprese mejor el contexto de detalle y retorno
- [x] 2.2 Mantener `GestionRespuesta` desacoplada del router y sin `useNavigate` propio
- [x] 2.3 Evitar duplicación confusa entre acciones del shell y acciones internas del contenido

## 3. Tests

- [x] 3.1 Actualizar pruebas de `GestionCorrespondenciaRoute` para validar la acción observable de retorno/cierre
- [x] 3.2 Verificar que la bandeja principal sigue visible cuando la vista secundaria está activa
- [x] 3.3 Verificar que el retorno lleva a la ruta base sin reintroducir `Drawer` ni overlay modal
- [x] 3.4 Verificar que `GestionRespuesta` sigue renderizando dentro del shell persistente sin acoplarse al router

## 4. Documentation and Validation

- [x] 4.1 Ajustar `README.md` del módulo si cambia la narrativa del flujo de retorno/cierre
- [x] 4.2 Ejecutar Vitest focal sobre rutas y páginas afectadas
- [x] 4.3 Ejecutar ESLint focal sobre archivos tocados
- [x] 4.4 Ejecutar validación TypeScript si el refinamiento toca composición o tipos
- [x] 4.5 Ejecutar `openspec validate scrumcore-71-implementacion-navegacion-gestion-correspondencia-02-fe --strict`
- [x] 4.6 Ejecutar `git diff --check`
- [x] 4.7 Registrar evidencia de validación en este checklist antes del archive

## Evidencia

- `npm.cmd test -- src/modules/gestionCorrespondencia/routes/GestionCorrespondenciaRoute.spec.test.tsx src/modules/gestionCorrespondencia/tests/GestionCorrespondenciaRoutePage.test.tsx src/modules/gestionCorrespondencia/tests/GestionCorrespondencia.test.tsx` -> `9 files`, `29 tests` OK
- `npx.cmd eslint src/modules/gestionCorrespondencia/routes/GestionCorrespondenciaRoute.tsx src/modules/gestionCorrespondencia/routes/GestionCorrespondenciaRoute.spec.test.tsx src/modules/gestionCorrespondencia/pages/GestionRespuesta.tsx` -> OK
- `npx.cmd tsc -b` -> OK
- `npx.cmd openspec validate scrumcore-71-implementacion-navegacion-gestion-correspondencia-02-fe --strict` -> OK
- `git diff --check` -> OK
