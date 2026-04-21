## 1. Consolidar estado de carga del detalle

- [x] 1.1 Inventariar fuentes actuales de estado para el detalle (`idTareaWf`, estructura, loading/error/isEmpty, metadata) en `GestionCorrespondenciaRoute` y `GestionRespuesta`.
- [x] 1.2 Implementar estado derivado unico del detalle (`loading`, `ready`, `blocked`) en el contenedor principal.
- [x] 1.3 Tipar y exponer el estado derivado para consumo de componentes hijos sin duplicar consultas.

## 2. Orquestacion por fases de dependencias

- [x] 2.1 Ajustar el flujo para que metadata de cabecera y contexto se resuelvan antes de habilitar la superficie operativa.
- [x] 2.2 Asegurar que `GestionRespuestaMainTabContent` y componentes dependientes solo se rendericen en estado `ready`.
- [x] 2.3 Mantener estados de carga/bloqueo con UI consistente en desktop y mobile overlay.

## 3. Cobertura de pruebas del flujo de carga

- [x] 3.1 Actualizar pruebas de ruta/detalle para validar transiciones `loading -> ready` y casos `blocked`.
- [x] 3.2 Agregar pruebas que verifiquen ausencia de contenido operativo durante `loading/blocked`.
- [x] 3.3 Verificar que tabs y panel secundario conservan comportamiento esperado al cambiar estado del detalle.

## 4. Validacion tecnica y trazabilidad

- [x] 4.1 Ejecutar `npx.cmd vitest run` sobre suites impactadas de `gestionCorrespondencia` y corregir regresiones del cambio.
- [x] 4.2 Ejecutar `npm.cmd run spec:validate` para asegurar coherencia entre specs y tags de tests.
- [x] 4.3 Ejecutar `npx.cmd tsc -b` y documentar evidencia de validacion en este `tasks.md`.

## Evidencia de validacion (2026-04-21)

- `npx.cmd vitest run src/modules/gestionCorrespondencia/routes/GestionCorrespondenciaRoute.spec.test.tsx` -> OK (12 tests).
- `npm.cmd run spec:validate` -> OK.
- `npx.cmd tsc -b` -> OK.
