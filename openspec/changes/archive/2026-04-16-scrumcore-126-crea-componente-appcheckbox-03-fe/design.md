# Design

## Context

`SCRUMCORE-126` corresponde a la tercera FE de la familia `AppCheckbox`.
No crea un componente nuevo distinto de `AppCheckbox`; completa la familia
shared ya iniciada en `SCRUMCORE-124` y extendida en `SCRUMCORE-125`.

El naming automatico proveniente de Jira (`AppAppcheckbox03Fe`) no debe
traducirse en una carpeta o wrapper nuevo. La implementacion debe vivir en
`src/app/Components/UI/AppCheckbox/` y mantener una sola API compartida.

## Scope Of This FE

Esta FE completa el patron reusable de seleccion total/parcial mediante:

- `AppCheckboxCheckAll`
- sincronizacion con `AppCheckboxGroup`
- estado `indeterminate`
- documentacion y ejemplos avanzados
- pruebas del flujo `check all`

## Design Decisions

### 1. `AppCheckboxCheckAll` sera un contrato controlado

El patron `check all` debe ser totalmente controlado por el consumidor. El
ownership del estado queda fuera del componente:

- `value: TValue[]`
- `onChange: (value: TValue[]) => void`
- `options: AppCheckboxOption<TValue>[]`

Esto mantiene la API consistente con `AppCheckboxGroup` y evita estados
duplicados o comportamiento ambiguo.

### 2. `CheckAll` se construye sobre la misma base del `Group`

`AppCheckboxCheckAll` no debe implementar una logica paralela ni aislada.
Debe reutilizar `AppCheckboxGroup` o un helper interno comun para calcular:

- todos seleccionados
- seleccion parcial
- limpieza total
- seleccion total

Esto mantiene un solo origen de verdad para el comportamiento de seleccion.

### 3. `indeterminate` es parte obligatoria del contrato visual

Cuando exista una seleccion parcial, el checkbox maestro debe reflejar el
estado `indeterminate`. No se permite resolver este caso solo con texto o con
conteo auxiliar; el estado visual debe verse directamente en el control.

### 4. La documentacion forma parte del entregable

Esta FE no termina solo con el componente. Debe incluir:

- README con API real
- ejemplos simples
- ejemplo con `AppCheckboxGroup`
- ejemplo con `AppCheckboxCheckAll`
- ejemplo de integracion con `Form.Item`, `name` y `rules`

La documentacion debe quedar alineada con los contratos tipados y con el
comportamiento real probado en tests.

## Implementation Notes

- Mantener Ant Design como base de comportamiento visual
- Conservar `CSS Modules` y evitar estilos globales
- No acoplar la logica a casos de negocio
- No usar `any`
- Mantener compatibilidad con el export shared de `src/app/Components/UI/index.ts`

## Risks

- duplicar logica entre `Group` y `CheckAll`
- dejar `indeterminate` inconsistente con `value`
- documentar ejemplos que no coincidan con la API final

## Result

Al finalizar esta FE, la familia `AppCheckbox` debe quedar lista para uso real
en flujos enterprise con:

- checkbox individual
- grupo controlado
- patron `check all`
- documentacion y ejemplos completos
