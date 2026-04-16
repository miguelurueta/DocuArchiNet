# Ticket 02 FE

## Titulo

Refinar UI/UX de `AppCheckbox` y `AppCheckboxGroup`

## Objetivo

Implementar la FE visual del componente reusable `AppCheckbox`, incluyendo
spacing, tamanos, estados visuales y grupo de casillas, manteniendo consistencia
con Ant Design y el Design System del proyecto.

## Contexto existente

- Arquitectura de referencia: `docs/Architecture/AppCheckbox/AppCheckbox-Architecture.md`
- Componente base esperado: `src/app/Components/UI/AppCheckbox/`
- Referencia visual shared: `AppInputSelect`, `AppInputTags`

## Alcance de esta FE

- tamanos `sm`, `md`, `lg`
- label y helperText consistentes
- estado `disabled`
- estado `error`
- `AppCheckboxGroup`
- layout vertical y horizontal
- responsive

## Reglas de UI (obligatorio)

- usar Ant Design como base visual principal
- mantener area clicable comoda
- labels legibles y bien alineados
- spacing consistente entre checkbox y texto
- helperText discreto y claro
- responsive en desktop, tablet y mobile
- `border-radius` leve y moderno en wrappers si aplica

## Reglas de implementacion (obligatorio)

- No usar estilos globales
- CSS Modules obligatorio
- No duplicar variantes de dominio
- El grupo debe componerse sobre el shared, no desde modulos

## Estructura sugerida

```tsx
<div className={styles.group}>
  {options.map((option) => (
    <AppCheckbox
      key={option.value}
      label={option.label}
      checked={selected.includes(option.value)}
      disabled={option.disabled}
    />
  ))}
</div>
```

## Entregables de esta FE

- `AppCheckbox.module.css`
- soporte de `size`
- soporte de `error`
- composicion `AppCheckboxGroup`
- layout responsive

## Pruebas obligatorias

- tamanos `sm`, `md`, `lg`
- group vertical
- group horizontal
- disabled en grupo
- helperText y error

## Criterios de aceptacion

- `AppCheckbox` y `AppCheckboxGroup` visualmente consistentes
- responsive correcto
- legibilidad y jerarquia clara
- comportamiento alineado con Ant Design
