## Contexto

`SCRUMCORE-124` corresponde a la primera FE de implementacion de `AppCheckbox`
como componente reusable en la capa shared de UI. La propuesta generada desde
Jira trae naming automatico defectuoso (`AppAppcheckbox01Fe`), pero el objetivo
real del change es crear `AppCheckbox` sobre `Checkbox` de Ant Design.

La arquitectura de referencia ya define tres piezas relacionadas:

- `AppCheckbox` como unidad base
- `AppCheckboxGroup` como composicion grupal
- `AppCheckboxCheckAll` como variante avanzada construida sobre grupo o helper comun

Esta primera FE no debe resolver toda la familia completa, pero si debe dejar
correctamente fijado el contrato base y la direccion de implementacion para no
generar wrappers paralelos ni APIs ambiguas.

## Decision

La FE 01 implementa el componente shared base `AppCheckbox` y deja tipados los
contratos publicos relacionados para `AppCheckboxGroup` y `AppCheckboxCheckAll`,
sin introducir aun la logica completa de grupo/check-all.

La carpeta de implementacion correcta sera:

```txt
src/app/Components/UI/AppCheckbox/
```

La solucion se apoya directamente en `Checkbox` de Ant Design y mantiene estos
criterios:

- `AppCheckbox` soporta `checked`, `defaultChecked`, `disabled`, `indeterminate`
  y `onChange`
- el shared expone label visible, helperText y atributos de accesibilidad
- `AppCheckboxGroup` y `AppCheckboxCheckAll` quedan definidos a nivel de tipos
  y documentacion con contrato controlado obligatorio
- la compatibilidad con `Form.Item` se contempla via `name` y `rules`, sin
  acoplar el shared a una instancia concreta de formulario

## API base

La API de esta FE se centra en `AppCheckbox`:

```ts
type AppCheckboxProps = {
  checked?: boolean;
  defaultChecked?: boolean;
  disabled?: boolean;
  indeterminate?: boolean;
  size?: "sm" | "md" | "lg";
  label?: ReactNode;
  helperText?: ReactNode;
  error?: boolean;
  className?: string;
  onChange?: (checked: boolean, event: CheckboxChangeEvent) => void;
  "aria-label"?: string;
  "aria-labelledby"?: string;
  "aria-describedby"?: string;
};
```

Los contratos de `AppCheckboxGroup` y `AppCheckboxCheckAll` se preservan en la
documentacion y en el diseño del shared como contratos controlados:

```ts
value: TValue[]
onChange: (value: TValue[]) => void
```

## Integracion con formularios

La implementacion base no depende de `Form.Item`, pero debe ser compatible con
ese ecosistema:

- permitir paso limpio de `name`
- permitir `rules` fuertemente tipadas en contratos grupales posteriores
- no interceptar validaciones de formulario
- no esconder el input base de Ant Design

## Consideraciones visuales

Se mantiene apariencia nativa de Ant Design con refinamiento shared minimo:

- tamanos `sm`, `md`, `lg`
- label y helperText consistentes con otros componentes shared
- estado `error` reflejado en wrapper/helper, no sustituyendo la semantica del checkbox
- area clicable clara y alineacion correcta entre casilla y texto

## Riesgos y mitigaciones

### 1. Mezclar el checkbox dentro de `AppInput`

Riesgo:
- contaminar la API de `AppInput` con responsabilidades de otro tipo de control

Mitigacion:
- `AppCheckbox` vive como shared independiente

### 2. Dejar contratos grupales ambiguos

Riesgo:
- que futuras FE implementen `Group` y `CheckAll` con ownership de estado distinto

Mitigacion:
- el diseño deja explicito desde ahora que `Group` y `CheckAll` son controlados
  por `value` + `onChange`

### 3. Duplicar logica entre Group y CheckAll

Riesgo:
- tener dos implementaciones independientes para seleccion parcial/total

Mitigacion:
- se deja establecido que `CheckAll` debe construirse sobre `Group` o compartir
  un helper/hook interno unico
