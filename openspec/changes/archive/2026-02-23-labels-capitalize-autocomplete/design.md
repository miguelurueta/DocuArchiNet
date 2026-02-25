## Context

Los campos `AUTOCOMPLETE` ya se renderizan con labels basados en `camposPlantilla`, pero la capitalizacion puede variar segun la data. Se requiere aplicar el efecto de letra capital exclusivamente para estos campos, sin alterar estructura ni atributos existentes.

## Goals / Non-Goals

**Goals:**
- Aplicar capitalizacion a labels de campos `AUTOCOMPLETE` con `campo_tip = 1`.
- Mantener atributos existentes (data-ident, required, disabled, title, tooltipAyuda) y accesibilidad.

**Non-Goals:**
- Cambiar textos originales o la logica de renderizado de otros tipos.
- Introducir nuevas dependencias.

## Decisions

- **CSS como opcion preferida**: usar `text-transform: capitalize` para no mutar el texto fuente y mantener i18n.
- **Aplicacion localizada**: aplicar el estilo solo a labels de `AUTOCOMPLETE` para minimizar impacto visual.

## Risks / Trade-offs

- [Riesgo] Capitalizar via CSS puede afectar idiomas con reglas especiales. -> Mitigacion: permitir ajuste futuro por render si fuese necesario.
- [Riesgo] Estilo aplicado fuera del alcance deseado. -> Mitigacion: usar clases locales en el componente.
