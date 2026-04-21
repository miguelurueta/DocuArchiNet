## 1. Modelo de bloqueo en la ruta de detalle

- [x] 1.1 Derivar estado de detalle (`loading`, `ready`, `blocked-empty`, `blocked-error`, `blocked-invalid-id`) en `GestionCorrespondenciaRoute` usando `idTareaWf`, `error` e `isEmpty` del hook `useEstructuraRespuestaIdTarea`.
- [x] 1.2 Ajustar render condicional para que `detailContent` solo se muestre en estado `ready`.
- [x] 1.3 Mantener metadata de cabecera consistente: placeholders en carga y valores de estructura solo cuando exista payload valido.

## 2. UI de bloqueo y navegacion de salida

- [x] 2.1 Implementar superficie de bloqueo dentro de `detailBody` con mensaje contextual para `blocked-empty`, `blocked-error` y `blocked-invalid-id`.
- [x] 2.2 Reutilizar la accion existente de retorno a bandeja (`navigate('/dashboard/gestion-correspondencia')`) desde el estado bloqueado.
- [x] 2.3 Validar comportamiento responsive del estado bloqueado (desktop y mobile overlay) sin romper el layout del panel secundario.

## 3. Cobertura de pruebas

- [x] 3.1 Actualizar `GestionCorrespondenciaRoute.spec.test.tsx` para cubrir `ready` (renderiza detalle) y `blocked-empty` (bloquea detalle).
- [x] 3.2 Agregar pruebas para `blocked-error` y `blocked-invalid-id` verificando que no se renderiza contenido editable y que existe accion de retorno.
- [x] 3.3 Asegurar que los escenarios nuevos usan tag de spec correspondiente y mocks de hook sin red real.

## 4. Validacion y cierre tecnico

- [x] 4.1 Ejecutar `npm.cmd run test -- --run` y corregir fallas de regresion relacionadas con la ruta de gestion correspondencia.
- [x] 4.2 Ejecutar `npm.cmd run spec:validate` para confirmar coherencia entre specs y tests.
- [x] 4.3 Documentar evidencia de validacion (comandos y resultado) en el change antes de pasar a fase de implementacion/aplicacion.

## Evidencia de validacion (2026-04-21)

- `npx.cmd vitest run src/modules/gestionCorrespondencia/routes/GestionCorrespondenciaRoute.spec.test.tsx` -> OK (10 tests, 1 file).
- `npx.cmd vitest run src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaMainTabContent.test.tsx src/modules/gestionCorrespondencia/routes/GestionCorrespondenciaRoute.spec.test.tsx` -> OK (12 tests, 2 files).
- `npm.cmd run spec:validate` -> OK (16 specs / 16 tags, sin faltantes ni unknown tags).
- `npx.cmd tsc -b` -> OK.
- `npm.cmd run test -- --run --silent` -> ejecutado; fallo global por suites no relacionadas (`radicacion` timeouts y `GestionCorrespondencia.profiling` sin tests). Se corrigio la unica regresion detectada en alcance cercano (`GestionRespuestaMainTabContent.test.tsx` por duplicidad de boton de colapso).
