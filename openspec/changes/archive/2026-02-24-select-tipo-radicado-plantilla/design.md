## Context

El select `TipoRadicado` existe en `RadicacionForm.tsx`, pero no se alimenta con opciones desde `camposPlantilla`. Se requiere vincularlo con el registro cuyo `name_campo = "TipoRadicado"` y usar `ilist_row_drowlist` para poblar el `<select>` manteniendo atributos existentes.

## Goals / Non-Goals

**Goals:**
- Identificar el registro de `camposPlantilla` por `name_campo = "TipoRadicado"`.
- Poblar el `<select data-ident="pl-radicacion-spe-TipoRadicado">` con opciones de `ilist_row_drowlist` y opcion inicial "Seleccionar".
- Mantener `required`, `title` y `tooltipAyuda`.

**Non-Goals:**
- Cambiar la arquitectura del formulario o los tipos de controles.
- Alterar el origen de `camposPlantilla`.

## Decisions

- **Mapeo directo por `name_campo`**: usar coincidencia exacta para localizar el registro y evitar ambiguedad.
- **Opcion inicial consistente**: siempre incluir "Seleccionar" antes del listado.
- **Tooltip y title**: reutilizar la misma estructura de tooltip usada en campos dinamicos para consistencia visual.

## Risks / Trade-offs

- [Riesgo] Si `camposPlantilla` no incluye `TipoRadicado`, el select quedara sin opciones. -> Mitigacion: manejar fallback y no romper UI.
- [Riesgo] Diferencias de claves (`idValue` vs `id_value`). -> Mitigacion: normalizar valores al mapear.
