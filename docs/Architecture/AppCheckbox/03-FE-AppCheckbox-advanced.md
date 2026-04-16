# Ticket 03 FE

## Titulo

Completar `AppCheckbox` con `check all`, documentacion y ejemplos avanzados

## Objetivo

Cerrar la implementacion reusable de `AppCheckbox` agregando patron `check all`,
estado `indeterminate`, documentacion completa y ejemplos listos para equipos
frontend.

## Contexto existente

- Arquitectura de referencia: `docs/Architecture/AppCheckbox/AppCheckbox-Architecture.md`
- Shared esperado: `src/app/Components/UI/AppCheckbox/`
- Base de interaccion: `Checkbox.Group` y estado `indeterminate` de Ant Design

## Alcance de esta FE

- `AppCheckboxCheckAll`
- sincronizacion de seleccion total/parcial
- ejemplos de uso
- README
- pruebas completas del contrato reusable

## Reglas de implementacion (obligatorio)

- `check all` no debe acoplarse a un caso de negocio concreto
- el estado parcial debe reflejarse con `indeterminate`
- el flujo debe mantenerse controlado mediante `value` + `onChange`
- no usar logica de dominio
- README debe incluir ejemplos simples y grupales
- incluir ejemplo de integracion con `Form.Item`, `name` y `rules`
- `CheckAll` debe construirse sobre `Group` o sobre un hook/helper comun, no como logica aislada

## Estructura sugerida

```tsx
const allValues = options.map((option) => option.value);
const checkedAll = value.length === allValues.length;
const indeterminate = value.length > 0 && !checkedAll;
```

## Entregables de esta FE

- `AppCheckboxCheckAll.tsx` o variante integrada bien documentada
- README de uso
- ejemplos basicos, grupo y check all
- pruebas del flujo `check all`

## Pruebas obligatorias

- check all selecciona todas
- check all limpia todas
- indeterminate cuando hay seleccion parcial
- disabled no permite cambios
- README alineado con API real

## Criterios de aceptacion

- patron `check all` reusable y claro
- documentacion util para frontend
- contrato estable y mantenible
- cobertura suficiente del comportamiento avanzado
