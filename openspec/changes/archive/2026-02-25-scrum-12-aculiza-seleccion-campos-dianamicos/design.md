## Context

El formulario de radicación combina campos estáticos y campos dinámicos provenientes de `camposPlantilla`.
Para campos dinámicos con comportamiento `SELECCION`, hoy la apariencia puede diferir del estilo visual de los controles estáticos del formulario (bordes, alto, estados focus/hover, consistencia general).

El ticket `SCRUM-12` solicita que los campos dinámicos de selección tengan el mismo lenguaje visual que los campos estáticos para mejorar coherencia de UI y experiencia de usuario.

## Goals / Non-Goals

**Goals:**
- Unificar el estilo visual de campos dinámicos `SELECCION` con los campos estáticos del formulario.
- Mantener intacto el comportamiento funcional actual (opciones, validaciones, disabled/required, data attributes).
- Incorporar pruebas de no-regresión para validar estructura/estilo esperado.

**Non-Goals:**
- Cambiar contratos de backend o estructura de `camposPlantilla`.
- Rediseñar el layout completo de `RadicacionForm`.
- Modificar componentes no relacionados con selección dinámica.

## Decisions

### Decisión 1: Aplicar estilos desde clase local en el renderer de selección dinámica
Se utilizará una clase de estilo dedicada (en `FormRadicacion.module.css`) para el `<select>` nativo dinámico en `CamposPlantillaAutoCompleteRenderer`.

Rationale:
- Controla consistencia visual sin acoplarse a clases internas de librerías.
- Mantiene alcance del cambio dentro del módulo de radicación.

Alternativa descartada:
- Ajustar estilos globales de `select`.
  - Se descarta por riesgo de impacto transversal en otros módulos.

### Decisión 2: Preservar atributos funcionales y de accesibilidad
El ajuste será solo visual; se preservan `data-ident`, `aria-*`, `required`, `disabled`, `maxLength` y opciones de `ilist_row_drowlist`.

Rationale:
- Evita regresiones en pruebas automatizadas y comportamiento de negocio.

Alternativa descartada:
- Reemplazar `<select>` por componente AntD `Select`.
  - Se descarta por ser un cambio funcional mayor fuera del alcance.

### Decisión 3: Validar con pruebas de componente
Se actualizarán pruebas para confirmar que los campos `SELECCION` dinámicos mantienen atributos y usan la clase visual esperada.

Rationale:
- Reduce regresiones visuales/estructurales en iteraciones futuras.

## Risks / Trade-offs

- [Riesgo] Ajustes CSS pueden afectar selectores de pruebas visuales existentes.
  → Mitigación: conservar `data-ident` y actualizar pruebas necesarias.

- [Riesgo] Diferencias entre navegadores al renderizar `<select>` nativo.
  → Mitigación: usar estilos compatibles y validar en navegadores objetivo.

- [Trade-off] Mayor especificidad de CSS local.
  → Mitigación: mantener reglas encapsuladas en el módulo y evitar cascadas globales.

## Migration Plan

1. Ajustar clases/estilos para campos dinámicos `SELECCION`.
2. Verificar visualmente que se alinean con controles estáticos del formulario.
3. Ejecutar pruebas del módulo radicación.
4. Registrar evidencia en `tasks.md`.
5. Si hay regresión visual crítica, rollback de reglas CSS nuevas.

## Open Questions

- ¿El alineamiento visual debe ser exacto al pixel con controles estáticos o basta con consistencia de estados (base/hover/focus)?
- ¿Hay un token corporativo obligatorio para bordes/sombras/focus de selects dinámicos?
