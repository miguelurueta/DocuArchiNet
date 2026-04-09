## Context

El proyecto ya cuenta con `AppTabs` (fases 01 y 02) como wrapper reusable de Ant Design. El ticket `SCRUMCORE-86` agrega capacidades avanzadas: sincronizacion con router, lazy rendering, telemetry de visibilidad y documentacion profesional. El router SPA ya existe y el componente debe integrarse sin acoplarse a modulos concretos.

## Goals / Non-Goals

**Goals:**
- Implementar `syncWithRouter` para sincronizar la tab activa con el URL (path segment y `?tab=`).
- Implementar `lazy` para renderizar contenido solo al activar tab, con cache de contenido.
- Disparar `onTabVisible(key)` cuando una tab se vuelve visible.
- Completar README con props y ejemplos para estas capacidades.

**Non-Goals:**
- No modificar el router global ni las rutas de modulos existentes.
- No introducir dependencias nuevas.
- No acoplar `AppTabs` a endpoints o flujo de negocio especifico.

## Decisions

1. **Router gana en conflicto con `activeKey`**
   - **Decision:** Cuando `syncWithRouter` esta activo, el valor de la ruta domina sobre `activeKey`.
   - **Rationale:** Evita inconsistencias entre URL y estado controlado; el URL es fuente de verdad.
   - **Alternatives considered:** Dar prioridad a `activeKey`. Se descarta por riesgo de desincronizacion y recargas no estables.

2. **Soportar `?tab=` y path segment**
   - **Decision:** Resolver tab desde `?tab=` y, si no existe, desde el ultimo segmento del path.
   - **Rationale:** Permite integracion flexible sin imponer un unico esquema de rutas.
   - **Alternatives considered:** Solo query param o solo path. Se descarta por menor compatibilidad.

3. **Lazy rendering con cache interno**
   - **Decision:** Renderizar children solo al activar una tab y mantener cache para evitar remount.
   - **Rationale:** Reduce costo inicial y evita re-render innecesario.
   - **Alternatives considered:** Desmontar al salir. Se descarta por perdida de estado interno.

4. **Telemetry solo al volverse visible**
   - **Decision:** Ejecutar `onTabVisible` cuando una tab pasa a visible por primera vez o por cambio de tab.
   - **Rationale:** Evita eventos redundantes y facilita observabilidad.
   - **Alternatives considered:** Emitir en cada render. Se descarta por ruido de telemetria.

## Risks / Trade-offs

- [Risk] Ambiguedad en reglas de resolucion de ruta -> Mitigacion: documentar el orden de prioridad y agregar tests de conflicto.
- [Risk] Cache de lazy puede retener memoria -> Mitigacion: cache solo por key activa y evitar almacenar nodos no usados.
- [Risk] Cambios de ruta no esperados pueden forzar tab invalida -> Mitigacion: fallback a primer tab habilitado.
