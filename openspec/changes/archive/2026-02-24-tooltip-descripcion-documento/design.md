## Context

`RadicacionForm` ya renderiza el campo `Descripcion_Documento` usando datos de `camposPlantilla`, pero no aplica los metadatos de plantilla para `title` y `tooltipAyuda`. Se requiere respetar estos atributos para mantener consistencia con otros campos dinámicos y mejorar la ayuda contextual sin alterar el flujo existente de selección ni el layout.

## Goals / Non-Goals

**Goals:**
- Localizar `Descripcion_Documento` en `camposPlantilla` y usar `title_control` como atributo `title`.
- Mostrar `tooltipAyuda` como icono con clase `tooltip-ayuda` junto al label.
- Mantener `required` y `disabled` existentes del campo.

**Non-Goals:**
- No se cambia el comportamiento de consulta o carga de opciones del select.
- No se rediseña el formulario ni se reemplaza el componente `Select`.
- No se agregan nuevas dependencias UI.

## Decisions

- **Usar metadatos del registro `Descripcion_Documento`**: el label y tooltip se derivan del objeto localizado en `camposPlantilla` para mantener coherencia con otros campos dinámicos.
  - Alternativas: hardcodear el tooltip en el formulario. Se descarta por perder consistencia con la plantilla.

- **Renderizar tooltip junto al label**: seguir el patrón existente de `tooltip-ayuda` y `InfoCircleOutlined` para mantener accesibilidad y estilos.
  - Alternativas: usar tooltip solo en el input. Se descarta porque el requerimiento indica tooltip en el label.

## Risks / Trade-offs

- [Riesgo] Campo sin `title_control` o `tooltipAyuda` → Mitigación: renderizar el label sin tooltip cuando esos valores estén vacíos.
- [Trade-off] Pequeña lógica adicional en el render del campo para construir el label con tooltip.
