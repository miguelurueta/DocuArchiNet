# Ticket 01 FE

## Titulo

Implementar `AppCheckbox` core como componente reusable sobre Ant Design

## Objetivo

Crear el componente real `AppCheckbox` en la capa shared de UI, basado en
`Checkbox` de Ant Design, con soporte para casilla individual controlada y no
controlada. El entregable de esta FE es la implementacion del componente, no la
creacion de nuevos documentos de arquitectura.

## Contexto existente

- Arquitectura de referencia: `docs/Architecture/AppCheckbox/AppCheckbox-Architecture.md`
- Referencia de filosofia shared: `src/app/Components/UI/AppButton/`, `AppInput/`
- Base visual y de interaccion: `Checkbox` de Ant Design

## Restricciones (obligatorio)

- No usar `any`
- Mantener `Checkbox` de Ant Design como base principal
- No embutir logica de negocio del dominio
- Tipado estricto para `checked`, `defaultChecked` y `onChange`

## Ubicacion (obligatoria)

```txt
src/app/Components/UI/AppCheckbox/
```

## Contratos (obligatorios)

```ts
export type AppCheckboxSize = "sm" | "md" | "lg";

export type AppCheckboxProps = {
  checked?: boolean;
  defaultChecked?: boolean;
  disabled?: boolean;
  indeterminate?: boolean;
  size?: AppCheckboxSize;
  label?: ReactNode;
  helperText?: ReactNode;
  error?: boolean;
  className?: string;
  onChange?: (checked: boolean, event: CheckboxChangeEvent) => void;
};

export type AppCheckboxGroupProps<TValue extends string | number = string> = {
  value: TValue[];
  options: AppCheckboxOption<TValue>[];
  disabled?: boolean;
  size?: AppCheckboxSize;
  direction?: "vertical" | "horizontal";
  label?: ReactNode;
  helperText?: ReactNode;
  error?: boolean;
  name?: string;
  rules?: Rule[];
  onChange: (value: TValue[]) => void;
};

export type AppCheckboxCheckAllProps<TValue extends string | number = string> = {
  options: AppCheckboxOption<TValue>[];
  value: TValue[];
  disabled?: boolean;
  size?: AppCheckboxSize;
  checkAllLabel?: ReactNode;
  name?: string;
  rules?: Rule[];
  onChange: (value: TValue[]) => void;
};
```

## Control de estado (obligatorio)

Para `AppCheckboxGroup` y `AppCheckboxCheckAll`, esta FE debe dejar explicito y
obligatorio el contrato controlado:

```ts
value: string[]
onChange: (value: string[]) => void
```

`defaultValue` no debe formar parte del contrato base de grupo ni de `check all`.

## Integracion con formularios (obligatorio)

La API debe contemplar compatibilidad con `Form.Item`:

- `name`
- `rules`
- paso limpio de validaciones externas
- `rules` con tipado fuerte alineado a Ant Design, no `unknown[]`

## Reglas de implementacion (obligatorio)

- Implementar `AppCheckbox.tsx` como wrapper real sobre `Checkbox`.
- Permitir modo controlado y no controlado.
- Soportar `disabled` e `indeterminate`.
- Exponer label visible y helperText.
- Dejar definidos tambien los contratos publicos de `AppCheckboxGroup` y `AppCheckboxCheckAll`.
- Dejar explicita la relacion: `CheckAll` usa `Group` o comparte un helper/hook interno unico.
- Exportar el componente desde el indice shared correspondiente.

## Estructura sugerida

```tsx
<Checkbox
  checked={checked}
  disabled={disabled}
  indeterminate={indeterminate}
  onChange={(event) => onChange?.(event.target.checked, event)}
>
  {label}
</Checkbox>
```

## Entregables de esta FE

- `AppCheckbox.tsx`
- `AppCheckbox.module.css`
- `index.ts`
- tipos exportados del componente
- implementacion funcional individual

## Pruebas obligatorias

- Renderiza label y helperText
- Respeta `checked` y `defaultChecked`
- Dispara `onChange`
- Respeta `disabled`
- Renderiza `indeterminate`

## Criterios de aceptacion

- Componente reusable creado en UI shared
- Carpeta del componente creada con implementacion real y exports correspondientes
- API estable y tipada
- Compatible con Ant Design y formularios del proyecto
