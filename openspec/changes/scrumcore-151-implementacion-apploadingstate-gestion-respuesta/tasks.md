## 1. Especificacion y alineacion

- [ ] 1.1 Verificar que `design.md` y `specs/**/spec.md` reflejan las restricciones del ticket (sin any, inline, sin timers en vistas, accesibilidad)
- [ ] 1.2 Acordar `delayMs` default=500 y copy del loading para el panel (`title`/`message`) segun ticket

## 2. Implementacion: AppLoadingState (shared)

- [ ] 2.1 Revisar/ajustar contrato de props de `AppLoadingState` para que coincida con el ticket (loading, delayMs default 500, title, message, icon, className, children)
- [ ] 2.2 Implementar la logica interna de delay/visibilidad en `AppLoadingState` (sin duplicacion en consumidores)
- [ ] 2.3 Garantizar render inline (card pequena) y sin estilos globales (solo estilos del modulo)
- [ ] 2.4 Implementar accesibilidad: `role=\"status\"` y `aria-live=\"polite\"` cuando este visible
- [ ] 2.5 Asegurar cleanup de timers al desmontar y al cambiar `loading` (sin setState post-unmount)

## 3. Pruebas unitarias: AppLoadingState

- [ ] 3.1 Test: no renderiza antes de `delayMs`
- [ ] 3.2 Test: renderiza despues de `delayMs` si `loading` sigue `true`
- [ ] 3.3 Test: se oculta cuando `loading=false`
- [ ] 3.4 Test: limpia timers correctamente (sin setState tras unmount)

## 4. Migracion: GestionCorrespondenciaRoute

- [ ] 4.1 Identificar y eliminar `showDelayedLoader` en `GestionCorrespondenciaRoute`
- [ ] 4.2 Eliminar `setTimeout / clearTimeout` del consumidor (sin logica de temporizacion en vistas)
- [ ] 4.3 Reemplazar por `AppLoadingState` con `loading={detailState === \"loading\"}`, `delayMs={500}`, `title` y `message` segun ticket
- [ ] 4.4 Mantener `data-testid=\"gestion-correspondencia-loading-state\"`

## 5. Pruebas de integracion UI: panel master-detail

- [ ] 5.1 Test: se renderiza correctamente dentro del panel master-detail cuando el detalle esta en loading
- [ ] 5.2 Test: no rompe layout del panel (render en contenedor existente)
- [ ] 5.3 Test: `children` visibles cuando `loading=false` (si aplica wrapper mode)

## 6. Verificacion

- [ ] 6.1 Ejecutar `npm run test -- --run` y corregir fallas relacionadas al cambio
- [ ] 6.2 Ejecutar `npm run spec:validate` (mapeo specs/tests)
- [ ] 6.3 Confirmar criterios de aceptacion: componente reusable, delay evita flicker, ruta migrada, sin timers duplicados, sin regresiones

