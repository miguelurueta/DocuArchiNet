## Context

`SCRUMCORE-116` corresponde a la segunda FE de `AppInputSelect`, centrada en la
capa UI/UX, responsive y sizing del componente shared ya creado en
`SCRUMCORE-114`. La propuesta automática generada desde Jira vuelve a introducir
un nombre técnico derivado del ticket (`AppAppinputselect02Fe`), pero el diseño
real debe aterrizarse como evolución de `AppInputSelect`, no como un componente
nuevo paralelo.

La arquitectura de referencia para esta FE está en
`docs/Architecture/AppInputSelect/02-FE-AppInputSelect-ui-ux.md` y fija como
objetivo la consolidación visual del wrapper sobre Ant Design `Select`:
estados, tamaños `sm/md/lg`, empty state, loading, responsive y border radius
leve y moderno.

## Goals / Non-Goals

**Goals:**
- Refinar la capa visual real de `AppInputSelect` sin romper su contrato actual.
- Alinear `sm`, `md` y `lg` con el lenguaje visual de `AppButton`.
- Consolidar estados visuales `idle`, `focused`, `disabled`, `loading`, `empty`,
  `error`, `warning` y `selected`.
- Mejorar la experiencia responsive en desktop, tablet y mobile.
- Mantener apariencia nativa de Ant Design con ajustes minimos por CSS Modules.
- Validar visualmente el comportamiento en select simple, remote y multiple.

**Non-Goals:**
- Rediseñar el contrato funcional del componente.
- Introducir lógica de dominio o endpoints concretos.
- Crear un componente distinto a `AppInputSelect`.
- Reemplazar consumidores reales en modulos ajenos dentro de esta FE.

## Decisions

- La FE trabajará sobre `src/app/Components/UI/AppInputSelect/AppInputSelect.module.css`
  y, si hace falta, sobre pequeños ajustes del wrapper en `AppInputSelect.tsx`.
- Se preservará la base visual de Ant Design; los overrides solo cubrirán sizing,
  spacing, radius, estados de soporte y comportamiento responsive.
- El `border-radius` seguirá siendo discreto, moderno y sobrio.
- `notFoundContent` seguirá siendo el punto oficial para empty state.
- Las mejoras visuales deben soportar modo simple, `multiple` y `tags`.
- Las pruebas se enfocarán en sizing, estados y cobertura del contrato visual ya
  existente.

## Risks / Trade-offs

- [Riesgo] Ajustar demasiado el CSS puede romper el look nativo de Ant Design.
  -> Mitigacion: limitar overrides a selector, spacing, radius y estados ya
  definidos por la arquitectura.

- [Riesgo] Los modos `multiple` y `tags` pueden degradarse en tablet o mobile.
  -> Mitigacion: validar wrap, altura minima y legibilidad en breakpoints chicos.

- [Riesgo] Cambios visuales en foco, error o warning pueden entrar en conflicto
  con los estados internos de Ant Design.
  -> Mitigacion: reaprovechar `status` del componente base y complementar solo
  lo necesario desde CSS Modules.

- [Riesgo] El naming automático de la proposal puede inducir a crear otro
  componente shared distinto.
  -> Mitigacion: dejar explícito en design/spec/tasks que esta FE refina el
  `AppInputSelect` existente.
