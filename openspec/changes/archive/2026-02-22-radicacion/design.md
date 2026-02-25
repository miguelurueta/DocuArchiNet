## Context

Actualmente `RadicacionForm.tsx` usa opciones hardcodeadas para el campo `ra_tipo_tramite`. La fuente oficial de configuración está en `useCamposPlantilla`, que entrega la lista de campos y sus opciones (por ejemplo `ilist_row_drowlist`). Esta discrepancia genera inconsistencias entre la plantilla de radicación y la UI.

**Restricción adicional:** si los tests obligatorios no pasan, los cambios no se aplican. Además, es obligatorio dejar evidencia de la ejecución de tests en la documentación OpenSpec.

## Goals / Non-Goals

**Goals:**
- Consumir la lista de tipos de trámite desde `useCamposPlantilla` y renderizarla en el `Select` de `ra_tipo_tramite`.
- Mantener compatibilidad con la UI existente y el resto del formulario.

**Non-Goals:**
- Reestructurar la arquitectura de formularios dinámicos.
- Cambiar la fuente de datos del resto de campos.
- Introducir nuevas dependencias.

## Decisions

- **Ubicar la lógica en `RadicacionForm.tsx`.** Se usará `useCamposPlantilla` para obtener los campos y extraer las opciones del campo `ra_tipo_tramite`. Esto cumple el requerimiento sin reestructurar rutas o tabs.
- **Mapeo directo de opciones.** Se mapearán los items de `ilist_row_drowlist` a `{ value, label }`, priorizando `id_value` como `value` y `value_campo` como `label`.
- **Fallback seguro.** Si la plantilla no trae `ra_tipo_tramite` o no hay opciones, se mantendrá una lista mínima de opciones por defecto para evitar un `Select` vacío.

## Risks / Trade-offs

- **[Riesgo] Doble consulta de plantilla si otros componentes ya usan `useCamposPlantilla`.** → Mitigación: mantener el hook sólo en `RadicacionForm` para esta necesidad puntual. Si en el futuro se consolida, se puede elevar el dato y compartirlo por props.
- **[Riesgo] Datos de plantilla incompletos.** → Mitigación: fallback de opciones por defecto.

## Migration Plan

- No requiere migración. Cambios sólo en frontend.
- Rollback: revertir a opciones estáticas del `Select`.

## Open Questions

- ¿La API garantiza que `ra_tipo_tramite` siempre existe en la plantilla? En caso negativo, confirmar si el fallback es aceptado o debe mostrarse vacío.
