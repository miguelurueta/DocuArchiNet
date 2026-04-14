## Context

`SCRUMCORE-97` corresponde a la fase 02 FE de `AppEditor`, enfocada en
consolidar su UI/UX responsive y su comportamiento visual en la capa de
`presentation`. El repo ya cuenta con una implementacion base del componente en
`src/app/Components/UI/AppEditor/` derivada de `SCRUMCORE-96`, por lo que esta
fase debe refinar la experiencia visual sin reabrir decisiones del core ni
mezclar cambios de infraestructura.

La documentacion de referencia principal es
`docs/Architecture/AppEditor/02-FE-AppEditor-ui-ux.md`, complementada por
`AppEditor-Architecture.md` y la implementacion base ya existente.

## Goals / Non-Goals

**Goals:**
- Consolidar `AppEditor.module.css` como capa visual oficial del componente.
- Mejorar toolbar, superficie editable y estados visuales para mobile, tablet y desktop.
- Dejar tokens CSS claros para light/dark mode y futura tematizacion.
- Reforzar accesibilidad visual, contraste y focus visible.
- Alinear la presentacion del componente con el design system shared del proyecto.

**Non-Goals:**
- No modificar `useAppEditor`, extensiones Tiptap ni la logica de sincronizacion controlada/no controlada.
- No incorporar reglas de negocio ni integraciones con modulos consumidores.
- No introducir estilos globales ni dependencias nuevas de UI.
- No tratar en esta fase la integracion funcional con un modulo real.

## Decisions

1. **Limitar el alcance a la capa de presentation**
   - **Decision:** La fase 02 actuara sobre `AppEditor.tsx`, `AppEditorToolbar.tsx` y `AppEditor.module.css`.
   - **Rationale:** El ticket y la documentacion delimitan esta fase como visual/UI-UX; tocar `application` o `infrastructure` ampliaria el alcance y aumentaria el riesgo.
   - **Alternatives considered:** Ajustar el hook o la configuracion Tiptap para resolver detalles visuales. Se descarta porque mezcla responsabilidades.

2. **Mantener tokens CSS como contrato visual del componente**
   - **Decision:** La presentacion de `AppEditor` se basara en custom properties locales que puedan ser sobreescritas por el entorno consumidor.
   - **Rationale:** Esto permite light/dark mode y refinamientos visuales sin acoplar el componente a un tema global especifico.
   - **Alternatives considered:** Hardcodear colores y estados en clases CSS. Se descarta por rigidez y mala capacidad de tematizacion.

3. **Diseño mobile-first con toolbar flexible**
   - **Decision:** La toolbar priorizara wrapping, tamanos tactiles y distribucion por grupos desde mobile, expandiendose progresivamente en tablet y desktop.
   - **Rationale:** El punto mas fragil del editor es la densidad de controles; disenar desde mobile reduce riesgo de overflow y friccion tactil.
   - **Alternatives considered:** Mantener una sola distribucion desktop y degradarla en mobile. Se descarta porque genera saturacion visual y usabilidad pobre.

4. **Refuerzo de estados visuales accesibles**
   - **Decision:** `hover`, `focus`, `active`, `disabled`, `readOnly` y `error` se expresaran visualmente con contraste claro y sin depender solo de color u hover.
   - **Rationale:** La documentacion exige accesibilidad visual y WCAG AA; el editor tiene suficientes elementos interactivos para que el focus y los estados sean parte del contrato.
   - **Alternatives considered:** Limitarse a los estilos actuales del proveedor UI. Se descarta porque no garantiza coherencia ni visibilidad suficiente.

## Risks / Trade-offs

- [Riesgo] La toolbar puede volverse demasiado densa en mobile.
  Mitigacion: priorizar wrap, spacing reducido y area tactil minima sin overflow horizontal.

- [Riesgo] El soporte de dark mode puede quedar superficial si se depende de colores fijos.
  Mitigacion: usar tokens visuales y validar contraste con overrides previsibles.

- [Riesgo] Cambios visuales en `AppEditor.module.css` pueden afectar el equilibrio logrado en la fase core.
  Mitigacion: preservar la estructura DOM base y limitar ajustes a presentation.

- [Riesgo] La interfaz del editor puede quedar inconsistente entre toolbar AntD y superficie ProseMirror.
  Mitigacion: unificar espaciado, bordes, radios y foco visible en todo el shell del componente.

## Migration Plan

- Partir de la implementacion base actual de `AppEditor`.
- Refinar presentation y estilos sin cambiar la API publica del componente.
- Validar visualmente y por pruebas de comportamiento los estados principales.
- Mantener el componente listo para una futura adopcion en un modulo consumidor sin migracion inmediata.

## Open Questions

- ¿La fase 02 debe incluir una apariencia dark mode completa por defecto o basta con garantizar tokens compatibles?
- ¿Conviene introducir una variante visual de toolbar compacta adicional o basta con responsive por CSS sobre la estructura actual?
