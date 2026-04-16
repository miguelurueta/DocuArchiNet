## Contexto

`SCRUMCORE-125` corresponde a la segunda FE de `AppCheckbox`. El naming
automatico de Jira vuelve a sugerir un componente incorrecto (`AppAppcheckbox02Fe`),
pero el objetivo real de este change es refinar la FE visual y de composicion de
la familia `AppCheckbox` ya iniciada en `SCRUMCORE-124`.

La FE 01 ya dejo resuelto:

- `AppCheckbox` base sobre `Checkbox` de Ant Design
- soporte controlado y no controlado individual
- estados `disabled` e `indeterminate`
- `label`, `helperText`, `aria` y `size`
- contratos publicos para `AppCheckboxGroup` y `AppCheckboxCheckAll`

Esta FE 02 no debe crear un componente paralelo ni redefinir la unidad base. Su
responsabilidad es completar la capa UI/UX y de composicion de grupo.

## Decision

La FE 02 implementa y refina:

- `AppCheckboxGroup` como composicion shared sobre `AppCheckbox`
- layout vertical y horizontal
- responsive
- consistencia visual de spacing, labels, helperText y estados de error

La implementacion sigue en:

```txt
src/app/Components/UI/AppCheckbox/
```

No se crea un folder separado para group porque forma parte de la misma familia
shared y comparte tokens visuales, naming y contrato base.

## Relacion entre componentes

La relacion interna queda asi:

- `AppCheckbox` sigue siendo la unidad base
- `AppCheckboxGroup` compone multiples `AppCheckbox`
- `AppCheckboxCheckAll` queda reservado para FE posterior, construido sobre grupo
  o sobre un helper/hook comun

Esta FE debe respetar esa jerarquia para evitar implementaciones paralelas.

## API objetivo de esta FE

La API principal nueva es `AppCheckboxGroup`:

```ts
type AppCheckboxGroupProps<TValue extends string | number = string> = {
  value: TValue[];
  options: AppCheckboxOption<TValue>[];
  disabled?: boolean;
  size?: "sm" | "md" | "lg";
  direction?: "vertical" | "horizontal";
  label?: ReactNode;
  helperText?: ReactNode;
  error?: boolean;
  name?: string;
  rules?: Rule[];
  onChange: (value: TValue[]) => void;
};
```

El ownership de estado es controlado de forma obligatoria:

```ts
value: TValue[]
onChange: (value: TValue[]) => void
```

No se introduce `defaultValue` para el grupo en esta FE.

## Consideraciones visuales

La FE visual debe mantener Ant Design como base, pero alinearse con el resto de
shared UI:

- spacing consistente entre casilla y texto
- group con gap legible
- variantes `vertical` y `horizontal`
- `helperText` y `error` consistentes con otros wrappers
- responsive para que el layout horizontal pueda reorganizarse en viewports estrechos

## Integracion con formularios

`AppCheckboxGroup` debe contemplar compatibilidad con `Form.Item`:

- soporte de `name`
- soporte de `rules` tipadas como `Rule[]`
- sin depender de una instancia concreta de formulario
- sin interceptar validaciones externas

## Riesgos y mitigaciones

### 1. Duplicar la logica de AppCheckbox

Riesgo:
- que `Group` implemente su propio checkbox interno y se desalineen estilos y accesibilidad

Mitigacion:
- `AppCheckboxGroup` compone exclusivamente `AppCheckbox`

### 2. Layout horizontal rigido

Riesgo:
- que el grupo se rompa o desborde en responsive

Mitigacion:
- definir un layout horizontal flexible con wrap controlado o degradacion a columna

### 3. Estado ambiguo del grupo

Riesgo:
- mezcla entre controlado y no controlado

Mitigacion:
- mantener `value + onChange` como contrato obligatorio
