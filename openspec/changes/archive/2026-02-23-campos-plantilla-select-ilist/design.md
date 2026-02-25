## Context

Los campos `SELECCION` ya se renderizan en la plantilla de radicacion, pero no siempre se pueblan con las opciones declaradas en `ilist_row_drowlist`. Se requiere estandarizar el llenado del `<select>` para evitar controles vacios y asegurar consistencia con la configuracion de `camposPlantilla`.

## Goals / Non-Goals

**Goals:**
- Poblar cada `<select>` de campos `SELECCION` con las opciones de `ilist_row_drowlist`.
- Incluir siempre la opcion inicial "Seleccionar".
- Mantener atributos existentes (required, disabled, title, tooltipAyuda) y la estructura actual.

**Non-Goals:**
- Cambiar la fuente de datos de `camposPlantilla` o el modelo de API.
- Introducir nuevas dependencias o rediseñar el componente.

## Decisions

- **Usar `ilist_row_drowlist` como fuente unica de opciones**: se mapearan opciones como `<option value={idValue}>{Value}</option>` y se antepondra la opcion inicial con `idValue: null` y texto "Seleccionar".
- **Mantener estructura actual del render**: se modifica solo el mapeo de opciones para evitar regresiones en estilos o accesibilidad.

## Risks / Trade-offs

- [Riesgo] Campos con `ilist_row_drowlist` vacio podrian mostrar solo "Seleccionar". -> Mitigacion: se preserva el placeholder y se evita render vacio.
- [Riesgo] Diferencias de nombres (`id_value` vs `idValue`, `value_campo` vs `Value`). -> Mitigacion: normalizar valores con fallback al mapear.
