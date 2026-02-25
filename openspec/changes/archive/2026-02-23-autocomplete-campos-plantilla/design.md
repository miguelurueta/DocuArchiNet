## Context

Se requiere un componente reutilizable que renderice autocompletados a partir de `camposPlantilla` y centralice el consumo de la API `/api/PlantillaRadicado/solicitaAutoCompleteCampos`. Hoy no existe un flujo unificado para estos campos ni manejo consistente de errores, loading y accesibilidad.

## Goals / Non-Goals

**Goals:**
- Renderizar solo campos con `campo_tip = 1` y `ComportamientoCampo = "AUTOCOMPLETE"`.
- Usar un componente de autocompletado (AntD o MUI) con loading, accesibilidad y eventos (`onChange`, `onBlur`, `onFocus`).
- Centralizar errores en Axios y mostrar mensajes amigables.
- Soportar `className` dinámico, `data-group`, `data-ident` y valores iniciales.
- Dejar documentación clara para extender a nuevos endpoints o validaciones.

**Non-Goals:**
- Cambiar la arquitectura del formulario o la fuente de datos principal.
- Implementar cache avanzado o prefetching global.
- Cambiar el contrato de la API existente.

## Decisions

- **Usar Ant Design AutoComplete.** Se alinea con el stack UI existente y mantiene consistencia visual.
- **Separar lógica de datos y UI.** Crear un hook/servicio para la consulta de autocompletado y un componente presentacional para renderizar campos.
- **Normalizar respuestas de API.** Mapear `data` a opciones `{ value, label }` usando `texValue` como label/valor visible.
- **Manejo central de errores.** Reutilizar el cliente Axios existente y exponer errores con mensajes amigables en el componente.

## Risks / Trade-offs

- **[Riesgo] Llamadas excesivas por cada tecla.** → Mitigación: debounce controlado en el componente/hook.
- **[Riesgo] Dependencia de `ComportamientoCampo` para `tbl_control`.** → Mitigación: validación defensiva y fallback a string vacío.

## Migration Plan

- Sin migración de datos. Integración gradual del componente en el formulario.
- Rollback: volver al renderizado estático o a la versión anterior del componente.

## Open Questions

- ¿Se requiere autenticación adicional o headers específicos para el endpoint?
- ¿Se necesita paginación o límite de resultados configurable?
