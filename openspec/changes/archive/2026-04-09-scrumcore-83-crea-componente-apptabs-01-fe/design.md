## Context

- Cambio OpenSpec para `SCRUMCORE-83` enfocado en crear `AppTabs` core (wrapper + contrato controlado).
- Arquitectura base en `docs/Architecture/AppTabs/AppTabs-Architecture.md` y orquestacion `AppTabs-Orquestacion.md`.
- Debe ser wrapper desacoplado de Ant Design Tabs con tipado estricto y bloqueo por disabled.

## Goals / Non-Goals

**Goals:**
- Implementar `AppTabs` core en `src/app/Components/UI/AppTabs/`.
- Soportar controlado/no controlado con reglas criticas.
- Implementar mapper interno `mapToAntdItems`.
- Bloqueo por `disabled` y `beforeChange` con prioridad correcta.

**Non-Goals:**
- Implementar estilos enterprise avanzados (Ticket 02).
- Implementar sync con router, lazy o telemetry (Ticket 03).
- Integraciones backend.

## Decisions

1. **Wrapper desacoplado de AntD Tabs**
   - `AppTabs` abstrae AntD y prohibe uso directo en vistas.

2. **Mapper interno obligatorio**
   - `mapToAntdItems(items)` evita interpretaciones inconsistentes.

3. **Controlado vs no controlado**
   - `activeKey` domina, `defaultActiveKey` solo en no controlado.

4. **Bloqueo por disabled/beforeChange**
   - prioridad: disabled -> beforeChange -> router -> onChange.

## Risks / Trade-offs

- **Errores de bloqueo** -> Mitigacion: pruebas unitarias para disabled/beforeChange.
- **Uso incorrecto de activeKey** -> Mitigacion: reglas estrictas de controlado/no controlado.

## Migration Plan

- Crear AppTabs sin migrar consumidores.
- Migracion progresiva posterior.

## Open Questions

- ¿Se requiere fallback de activeKey no existente al primer tab habilitado en core o en behavior?
