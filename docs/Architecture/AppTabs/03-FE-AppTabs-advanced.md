# PROMPT ARQUITECTONICO  Ticket 03 FE
# Router sync, lazy, telemetry y documentacion AppTabs

Rol esperado:
Arquitecto de software senior frontend (React, componentes UI enterprise, accesibilidad, testing)


OBJETIVO

Completar funcionalidades avanzadas de AppTabs: sincronizacion con router, lazy rendering, telemetry y documentacion profesional.


CONTEXTO EXISTENTE

- arquitectura: `docs/Architecture/AppTabs/AppTabs-Architecture.md`
- router SPA existente


UBICACION (OBLIGATORIA)

```
src/app/Components/UI/AppTabs/
```


REGLAS DE IMPLEMENTACION (OBLIGATORIAS)

1. SYNC WITH ROUTER
   - `syncWithRouter` basado en URL
   - soportar path segment y query param `?tab=`
   - mantener estado al recargar
   - fallback si `activeKey` no existe en items
   - resolver conflicto entre `activeKey` y `syncWithRouter` (router gana)
   - diferenciar sync inicial vs cambios de ruta

2. LAZY RENDERING
   - `lazy` renderiza contenido solo al activar tab
   - no renderizar contenido innecesario
   - cachear contenido para no re-mount

3. TELEMETRY
   - `onTabVisible(key)` cuando un tab se vuelve visible

4. DOCUMENTACION
   - README con descripcion, importacion, props y ejemplos


PRUEBAS UNITARIAS (OBLIGATORIAS)

- test de sincronizacion con router
- test de lazy rendering
- test de `onTabVisible`
 - test de fallback cuando `activeKey` no existe
 - test de conflicto `activeKey` vs `syncWithRouter`


CRITERIOS DE ACEPTACION

- syncWithRouter funcional y estable
- lazy rendering sin render extra
- README profesional con ejemplos
