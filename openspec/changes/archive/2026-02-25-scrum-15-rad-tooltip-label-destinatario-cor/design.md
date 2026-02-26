## Context

En `RadicacionForm.tsx` el campo de destinatario (`data-ident="pl-radicacion-spe-Destinatario_Cor"`) hoy usa una configuracion estática y no toma de forma consistente la metadata del registro de `camposPlantilla` con `name_campo = "Destinatario_Cor"`. El requerimiento de `SCRUM-15` exige resolver ese registro desde plantilla y aplicar atributos declarativos (`required`, `disabled`, `title`, `tooltipAyuda`) para homogenizar comportamiento con otros campos dinámicos de radicación.

## Goals / Non-Goals

**Goals:**
- Localizar el campo `Destinatario_Cor` desde `camposPlantilla` por `name_campo` (comparación normalizada).
- Aplicar en UI de destinatario atributos derivados de plantilla: `required`, `disabled`, `title_control` como `title`, y `tooltipAyuda` con `span.tooltip-ayuda`.
- Mantener la experiencia de selección token del destinatario sin romper acciones existentes.
- Cubrir el comportamiento con pruebas del formulario.

**Non-Goals:**
- Rediseñar el flujo completo de destinatarios/remitentes.
- Cambiar contratos backend o modelos de datos de terceros.
- Modificar campos no relacionados de la pantalla de radicación.

## Decisions

### Decision 1: Resolver metadata por `name_campo` normalizado
- **Decision:** Buscar el campo de destinatario comparando `name_campo` normalizado (trim + case-insensitive) contra `Destinatario_Cor`.
- **Rationale:** Evita dependencias frágiles con texto de label o posición del arreglo.
- **Alternatives considered:** usar mapeo por índice fijo (descartado por frágil).

### Decision 2: Reutilizar componente de selección existente para destinatario
- **Decision:** Mantener `SelectDestinatario`/token behavior y solo inyectar metadata resuelta desde plantilla.
- **Rationale:** Menor riesgo de regresión funcional y visual.
- **Alternatives considered:** reemplazar por nuevo componente dedicado (descartado por mayor impacto).

### Decision 3: Tooltip y title desde metadata de plantilla
- **Decision:** `title_control` se mapeará al atributo `title` del label y `tooltipAyuda` se renderizará mediante `span.tooltip-ayuda` con ícono de ayuda.
- **Rationale:** cumple requerimiento funcional y conserva consistencia con patrón existente en el formulario.
- **Alternatives considered:** usar solo tooltip nativo sin ícono (descartado por no cumplir UX requerida).

## Risks / Trade-offs

- **[Risk] Variaciones en el nombre del campo (`Destinatario_Cor`)** -> **Mitigation:** normalización de nombre antes de comparar.
- **[Risk] Regresión en validaciones del selector destinatario** -> **Mitigation:** pruebas de formulario para required/disabled y re-render.
- **[Risk] Inconsistencia visual del tooltip** -> **Mitigation:** reutilizar clase existente `tooltip-ayuda` y patrón de label del módulo.

## Migration Plan

1. Identificar el registro `Destinatario_Cor` en `camposPlantilla`.
2. Inyectar metadata al selector destinatario (`required`, `disabled`, `title`, `tooltipAyuda`).
3. Ajustar render de label con `span.tooltip-ayuda` e ícono.
4. Ejecutar tests de radicación y documentar evidencia en `tasks.md`.

Rollback: revertir commit de integración de metadata; no hay migración de datos.

## Open Questions

- Confirmar si `Destinatario_Cor` debe soportar selección múltiple o mantenerse de selección única visual (token único).
- Confirmar si el tooltip debe mostrarse siempre que exista `tooltipAyuda`, incluso con `disabled`.
