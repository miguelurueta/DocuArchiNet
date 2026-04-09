## 1. Router sync

- [x] 1.1 Implementar `syncWithRouter` con query param `?tab=` y path segment
- [x] 1.2 Resolver fallback cuando el `key` del URL no existe en `items`
- [x] 1.3 Resolver conflicto `activeKey` vs `syncWithRouter` (router gana)
- [x] 1.4 Diferenciar sync inicial vs cambios de ruta para evitar side effects

## 2. Lazy rendering y telemetry

- [x] 2.1 Implementar `lazy` para renderizar contenido solo al activar tab
- [x] 2.2 Cachear contenido activado para evitar remount
- [x] 2.3 Implementar `onTabVisible` cuando una tab se vuelve visible

## 3. Documentacion y pruebas

- [x] 3.1 Actualizar README con props avanzadas y ejemplos
- [x] 3.2 Test: sincronizacion con router
- [x] 3.3 Test: fallback cuando `activeKey` no existe
- [x] 3.4 Test: conflicto `activeKey` vs `syncWithRouter`
- [x] 3.5 Test: lazy rendering con cache
- [x] 3.6 Test: `onTabVisible`
- [x] 3.7 Ejecutar pruebas `npx vitest --run src/app/Components/UI/AppTabs/AppTabs.test.tsx`
- [x] 3.8 Registrar evidencia de tests en `tasks.md`

Evidencia: `npx vitest --run src/app/Components/UI/AppTabs/AppTabs.test.tsx` (2026-04-09).
