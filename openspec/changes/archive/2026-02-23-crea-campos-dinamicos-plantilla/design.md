## Context

Se necesita un componente que renderice campos dinámicos a partir de `camposPlantilla` para evitar hardcode y asegurar consistencia en validaciones, accesibilidad y metadata (labels, tooltips, data-*). La solución debe integrarse en el módulo de radicación sin modificar la arquitectura ni introducir dependencias nuevas.

## Goals / Non-Goals

**Goals:**
- Renderizar campos con `campo_tip = 1` dentro de un `<Card data-ident="pl-radicacion-card-spe">`.
- Soportar mapeo de `ComportamientoCampo` a controles (`select`/`input`) con atributos `maxLength`, `required`, `disabled`, `type/pattern`, `data-api-method` y `data-ident`.
- Incluir accesibilidad (`aria-label`/`aria-describedby`), eventos (`onChange`, `onBlur`, `onFocus`) y metadatos de UI (`aleas_campo`, `title_control`, `tooltipAyuda`), con preparación para i18n.

**Non-Goals:**
- Implementar la carga real de opciones/autocompletado desde APIs.
- Rediseñar el formulario de radicación existente o su layout.
- Añadir nuevas dependencias o cambiar la estrategia de validación global.

## Decisions

- **Crear un componente dedicado en radicación.** Ubicado en `src/modules/radicacion/components` para aislar la lógica de renderizado dinámico y facilitar tests.
- **Mapeo declarativo de propiedades de plantilla a atributos HTML.** Traducir `max_leng_campo`, `obligatorio_campo`, `disable_campo`, `control_tip_correo` y `apiMethod` a atributos nativos (`maxLength`, `required`, `disabled`, `type`, `pattern`, `data-api-method`).
- **i18n listo pero no acoplado.** Exponer textos (`aleas_campo`, `title_control`, `tooltipAyuda`) a través de una función/adapter de traducción opcional para evitar dependencia directa a una librería de i18n.

## Risks / Trade-offs

- **[Riesgo] Campos con reglas no cubiertas (números/fechas) podrían renderizarse como texto.** → Mitigación: incluir mapeo de validaciones por metadatos cuando existan (type/pattern), y documentar extensibilidad.
- **[Riesgo] Diferencias en nomenclatura de propiedades en plantillas históricas.** → Mitigación: defensivo en lectura de propiedades y fallback razonables.

## Migration Plan

- Sin migración de datos. Cambio solo en frontend.
- Rollback: revertir el componente y retornar al renderizado estático actual.

## Open Questions

- ¿Existe un catálogo oficial de validaciones (número, fecha, etc.) para mapear `type/pattern`?
- ¿Hay una función de traducción estándar ya disponible en el módulo de radicación?
