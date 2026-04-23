## 1. Cobertura de pruebas obligatorias de AppSteps

- [x] 1.1 Revisar `src/app/Components/UI/AppSteps/AppSteps.test.tsx` contra la matriz minima del ticket (base, form sync/async, progress, timeline, controlado/no controlado).
- [x] 1.2 Completar/ajustar casos faltantes para `disabled`, `validateStep` async, render de `progressPercent`, render de `timestamp` y `timeline` vertical.
- [x] 1.3 Asegurar convencion `[SPEC:APP-APPSTEPS-03-FE]` en los bloques de prueba nuevos o actualizados.

## 2. Integracion real en modulo consumidor

- [x] 2.1 Integrar `AppSteps` en `src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaMainTabContent.tsx` como flujo visible del workbench.
- [x] 2.2 Orquestar el flujo desde el consumidor usando `items`, `current`, `onChange` y `validateStep` (si aplica) sin duplicar motor de navegacion.
- [x] 2.3 Ajustar estilos/modificadores locales del modulo para asegurar coherencia visual desktop/mobile sin introducir theming global nuevo.

## 3. Pruebas de integracion del consumidor

- [x] 3.1 Crear o ampliar pruebas del modulo consumidor para validar wiring de `AppSteps` (render de paso activo y transiciones observables).
- [x] 3.2 Verificar en pruebas que la validacion de negocio/formulario permanece en el consumidor y no en internals de `AppSteps`.
- [x] 3.3 Verificar en pruebas que el consumidor no mantiene una segunda logica paralela de navegacion por pasos.

## 4. Documentacion y evidencia operativa

- [x] 4.1 Actualizar `src/app/Components/UI/AppSteps/README.md` con ejemplo real de integracion en `gestionCorrespondencia` y limites de responsabilidad.
- [x] 4.2 Documentar en el change la evidencia requerida para PR (capturas/video corto desktop+mobile y referencia del ticket/variante).

## 5. Validacion final y cierre tecnico

- [x] 5.1 Ejecutar `npx.cmd vitest --run src/app/Components/UI/AppSteps/AppSteps.test.tsx`.
- [x] 5.2 Ejecutar pruebas del modulo integrado de `gestionCorrespondencia` y corregir fallas de regresion.
- [x] 5.3 Ejecutar `npm.cmd run spec:validate` y registrar resultado como evidencia del cambio.
