## Context

Se requiere un componente base `AppTable` usando AG Grid Community para estandarizar tablas reutilizables en DocuArchiCore. Debe ser 100% presentacional, tipado y desacoplado del backend.

## Goals / Non-Goals

**Goals:**
- Implementar `AppTable<T>` con props tipadas para rows, columns y callbacks.
- Centralizar configuracion base en `agGridDefaultConfig` y `useAgGridBaseConfig`.
- Soportar loading, empty state, seleccion y renderers externos.
- Documentar API y limites del componente en README.

**Non-Goals:**
- Integraciones con backend, React Query o DTOs.
- Transformacion de datos dentro del componente.
- Funcionalidades avanzadas de paginacion o CRUD.

## Decisions

- **Separacion por capas**: `AppTable` (render), `useAgGridBaseConfig` (composicion), `agGridDefaultConfig` (defaults) para mantener cohesion.
- **Tipado generico**: `AppTable<T extends Record<string, unknown>>` para garantizar contratos fuertes sin `any`.
- **Estados de UX**: overlays de loading/empty configurados en defaults para reutilizacion.
- **Callbacks tipados**: eventos expuestos como callbacks simples y tipados para integracion futura.

## Risks / Trade-offs

- [Sobrecarga de configuracion inicial] -> Mitigacion: defaults centralizados y configurables por props.
- [Uso incorrecto de tipos genericos] -> Mitigacion: ejemplos claros en README.

## Migration Plan

- Componentes nuevos aislados; no requiere migracion.
- Rollback: eliminar carpeta `src/app/Components/UI/AppTable` y README asociado.

## Open Questions

- Confirmar lineamientos visuales para empty/loading (texto y estilos).
- Definir estrategia de theming si se requiere en fases futuras.
