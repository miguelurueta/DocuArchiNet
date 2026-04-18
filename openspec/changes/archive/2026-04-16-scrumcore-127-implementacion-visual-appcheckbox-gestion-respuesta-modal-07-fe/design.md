# Design

## Context

`SCRUMCORE-127` implementa la integracion visual de la familia shared
`AppCheckbox` dentro del modal `GestionDocumentoModal` del modulo de gestion de
correspondencia.

Actualmente, el modal usa tres controles `AppInput` con `type="checkbox"` en
su bloque superior. Esa solucion funciona visualmente, pero ya no es la opcion
correcta para el design system despues de la creacion de la familia shared
`AppCheckbox` en `SCRUMCORE-124`, `SCRUMCORE-125` y `SCRUMCORE-126`.

El objetivo de esta FE es migrar ese bloque a `AppCheckbox` sin alterar la
semantica general del modal ni introducir logica de negocio.

## Scope Of This FE

Esta FE cubre:

- reemplazo de `AppInput type="checkbox"` por `AppCheckbox`
- conservacion del estado local del modal
- ajustes de layout del bloque superior para que los labels largos sigan siendo legibles
- mantenimiento de accesibilidad y comportamiento del modal

No cubre:

- backend
- submit real
- validaciones de negocio
- uso de `AppCheckboxGroup`
- uso de `AppCheckboxCheckAll`

## Design Decisions

### 1. Se usara `AppCheckbox` individual, no `Group` ni `CheckAll`

El modal presenta tres decisiones booleanas independientes. Aunque ya existe la
familia completa de `AppCheckbox`, este flujo no necesita composicion grupal ni
patron `check all`.

La integracion correcta es:

- `AppCheckbox` individual para cada item del bloque superior

Esto evita sobredisenar la pantalla y mantiene la semantica actual del
formulario.

### 2. El modal mantiene su estado local y su contrato actual

`GestionDocumentoModal` seguira siendo un componente controlado por:

- `open`
- `onClose`

Y los valores internos del formulario seguiran siendo estado local visual. No
se incorporaran side effects, adaptadores ni integracion remota.

### 3. El cambio debe consolidar el design system

Despues de esta FE, el modal ya no debe depender de:

- `AppInput type="checkbox"`

para este caso de uso. El bloque superior debe quedar alineado con la familia
shared correcta:

- `AppCheckbox`

Esto reduce deuda de UI y elimina patrones duplicados para checkboxes.

### 4. Los textos largos deben priorizar legibilidad sobre compacidad

Los tres labels del modal son largos. Por eso:

- el bloque debe mantenerse en columna
- el label debe wrappear de forma natural
- el click target debe seguir siendo claro
- no se deben forzar distribuciones horizontales agresivas

### 5. Los ajustes visuales deben quedar en el modulo, no en el shared

Si el bloque superior necesita spacing o alineacion adicional, eso debe
resolverse en `GestionDocumentoModal.module.css`, no reconfigurando la familia
shared `AppCheckbox` para un caso particular del modulo.

## Implementation Notes

- Importar `AppCheckbox` desde `src/app/Components/UI/AppCheckbox`
- Eliminar `AppInput` como dependencia para el bloque de checks del modal
- Mantener `AppInputSelect`, `AppInputTags` y acciones sin cambio funcional
- Preferir nombres de estado booleano mas semanticos si el cambio no genera ruido
- Mantener `size="md"` en los tres checkboxes

## Risks

- dejar estilos del modulo demasiado acoplados a una estructura previa basada en `AppInput`
- romper el wrap de labels largos en resoluciones estrechas
- mezclar en la misma FE cambios ajenos al bloque superior del modal

## Result

Al finalizar esta FE, `GestionDocumentoModal` debe quedar visualmente
consistente con el design system shared del proyecto, usando `AppCheckbox` en
el bloque superior y eliminando el uso de `AppInput type="checkbox"` para este
flujo.
