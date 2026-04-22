## 1. Scaffold del componente UI

- [x] 1.1 Crear carpeta `src/app/Components/UI/AppSteps/` con archivos base `AppSteps.tsx`, `AppSteps.types.ts`, `AppSteps.module.css`, `AppSteps.test.tsx` e `index.ts`.
- [x] 1.2 Definir contrato público del componente (`AppStepsProps`, `AppStepItem`, `AppStepsVariant`, `AppStepStatus`).
- [x] 1.3 Exportar `AppSteps` en `src/app/Components/UI/index.ts` para importación desde barrel compartido.

## 2. Implementación funcional

- [x] 2.1 Implementar render de secuencia de pasos en orden con resaltado del paso activo controlado por props.
- [x] 2.2 Implementar manejo de interacción para cambio de paso, respetando pasos deshabilitados y callback de notificación al contenedor.
- [x] 2.3 Implementar `variant="default"` y `variant="form"` con validación externa (`validateStep`) sync/async y bloqueo en fallo con estado `error`.
- [x] 2.4 Incluir helpers internos para evitar duplicación (`guardStepChange`, `resolveIsControlled`, `normalizeItems`).
- [x] 2.5 Incluir semántica accesible base en el DOM para identificar progreso y paso actual.

## 3. Integración en consumidor piloto

- [x] 3.1 Deferido por alcance: no integrar en consumidor en este ticket (instrucción explícita), se moverá a ticket de integración.
- [x] 3.2 Deferido por alcance: no ajustar contenedor piloto en esta implementación.
- [x] 3.3 Deferido por alcance: validación manual de integración quedará para el ticket de adopción.

## 4. Pruebas y trazabilidad SPEC

- [x] 4.1 Crear pruebas en `AppSteps.test.tsx` para render de pasos, paso activo y transición controlada.
- [x] 4.2 Agregar pruebas para comportamiento de pasos deshabilitados y semántica de accesibilidad observable en DOM.
- [x] 4.3 Etiquetar pruebas relacionadas con `[SPEC:APP-APPSTEPS-01-FE]` en `describe` o nombre del test.
- [x] 4.4 Agregar pruebas para `variant="form"` con `validateStep` sync/async y escenarios de bloqueo/avance.
- [x] 4.5 Ejecutar `npx.cmd vitest --run src/app/Components/UI/AppSteps/AppSteps.test.tsx` y corregir fallas antes de finalizar.
- [x] 4.6 Ejecutar `npm.cmd run spec:validate` y dejar evidencia de resultado en artefactos del cambio.
