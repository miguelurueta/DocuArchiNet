## Context

Los campos dinamicos `SELECCION` y `AUTOCOMPLETE` ya se renderizan con labels basados en `camposPlantilla`, pero la capitalizacion puede variar segun la data. Se requiere aplicar un efecto de letra capital para uniformidad visual, sin alterar la estructura ni los atributos existentes.

## Goals / Non-Goals

**Goals:**
- Aplicar capitalizacion a los labels de campos `SELECCION` y `AUTOCOMPLETE` con `campo_tip = 1`.
- Mantener atributos existentes (data-ident, required, disabled, title, tooltipAyuda) y accesibilidad.

**Non-Goals:**
- Cambiar textos originales en datos o internacionalizacion.
- Modificar la logica de renderizado de campos o tipos de control.

## Decisions

- **CSS como opcion preferida**: usar `text-transform: capitalize` para evitar mutar el texto fuente y mantener i18n sin efectos colaterales.
- **Aplicacion localizada**: aplicar el estilo solo a labels de campos dinamicos `SELECCION`/`AUTOCOMPLETE` para minimizar impacto visual global.

## Risks / Trade-offs

- [Riesgo] Capitalizar via CSS puede afectar idiomas con reglas especiales. -> Mitigacion: mantener la opcion de transformacion por render si se requiere ajuste futuro.
- [Riesgo] Estilo global accidental. -> Mitigacion: usar clases locales en el componente.
