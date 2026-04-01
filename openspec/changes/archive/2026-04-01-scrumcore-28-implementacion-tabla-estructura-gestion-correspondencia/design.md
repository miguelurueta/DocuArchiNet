## Context

GestionCorrespondencia requiere una vista de tabla con filtros y paginacion usando AppTable y AppInput existentes. Debe mantenerse la arquitectura modular y evitar logica de backend.

## Goals / Non-Goals

**Goals:**
- Reemplazar el contenido dentro de AppContent por la estructura requerida (filtros, paginacion, tabla).
- Usar AppInput controlado para search, categoria y pageSize.
- Renderizar AppTable con datos mock y columnas basicas.
- Ajustar CSS Modules para layout responsive sin overflow.

**Non-Goals:**
- Implementar filtros reales o paginacion con backend.
- Integrar logica de negocio o consumo de APIs.

## Decisions

- **Estructura fija por filas**: se mantiene el layout exacto solicitado para filtros, paginacion y tabla.
- **Estado local**: search, category y pageSize se controlan con `useState` sin efectos secundarios.
- **Mock data**: filas y columnas basicas para validar render y layout.

## Risks / Trade-offs

- [AppInput select no estaba soportado] -> Mitigacion: extender AppInput para manejar `type="select"` con opciones.
- [Diferencias de layout en mobile] -> Mitigacion: reglas responsive en CSS para columnas y controles.

## Migration Plan

- Cambios localizados en GestionCorrespondencia y AppInput.
- Rollback: revertir estructura de AppContent y estilos asociados.

## Open Questions

- Definir opciones reales de categoria y paginacion en fases posteriores.
