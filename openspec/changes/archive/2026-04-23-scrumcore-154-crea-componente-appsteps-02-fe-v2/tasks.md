## 1. Extensión del contrato y estructura de AppSteps

- [x] 1.1 Revisar y ajustar tipos públicos de `AppSteps` para cubrir `variant="progress"` y `variant="timeline"` sin romper compatibilidad con `default/form`.
- [x] 1.2 Confirmar soporte tipado para `progressPercent`, `timestamp`, `size` (`sm/md/lg`) y props de orientación responsive.
- [x] 1.3 Mantener componente único `AppSteps` y consolidar helpers para evitar duplicación de lógica por variante.

## 2. Implementación de variantes progress y timeline

- [x] 2.1 Implementar bloque visual de progreso global en `variant="progress"` que renderice solo cuando `progressPercent` esté presente.
- [x] 2.2 Garantizar que `AppSteps` no calcule porcentaje internamente y consuma el valor externo como fuente única.
- [x] 2.3 Implementar `variant="timeline"` con render de `timestamp` por step y separación visual tipo historial.
- [x] 2.4 Forzar orientación vertical en `timeline` incluso cuando se reciba preferencia horizontal desde props.
- [x] 2.5 Preservar composición de `title`, `description` e `icon` en las nuevas variantes.

## 3. Responsive y accesibilidad

- [x] 3.1 Implementar reglas responsive para `default/form/progress` con fallback vertical en anchos reducidos.
- [x] 3.2 Mantener `timeline` vertical en todos los breakpoints (desktop/tablet/mobile).
- [x] 3.3 Asegurar foco visible y navegación utilizable por teclado en interacción de steps.
- [x] 3.4 Verificar que el paso activo conserve `aria-current="step"` en todas las variantes.
- [x] 3.5 Garantizar que estados de proceso/error no dependan únicamente de color (señales semánticas/estructurales).

## 4. Pruebas y validación

- [x] 4.1 Añadir pruebas de `variant="progress"` para render condicional de progreso y consistencia visual con steps.
- [x] 4.2 Añadir pruebas de `variant="timeline"` para orientación vertical forzada y render de `timestamp`.
- [x] 4.3 Añadir pruebas de comportamiento responsive en viewport reducido para fallback vertical.
- [x] 4.4 Añadir pruebas de accesibilidad para semántica del paso activo y señales de estado observables.
- [x] 4.5 Ejecutar `npx.cmd vitest --run src/app/Components/UI/AppSteps/AppSteps.test.tsx` y corregir fallas.
- [x] 4.6 Ejecutar `npm.cmd run spec:validate` y registrar evidencia de resultado en el cambio.
