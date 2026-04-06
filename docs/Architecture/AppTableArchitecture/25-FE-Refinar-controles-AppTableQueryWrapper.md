# Ticket 25 FE

## Titulo

Refinar controles de `AppTableQueryWrapper` y paginacion inicial de `GestionCorrespondencia`

## Objetivo

Mejorar la consistencia visual y funcional de los controles superiores de `AppTableQueryWrapper`, manteniendo el default global de paginacion en `25`, reemplazando los botones de accion del wrapper por `AppButton` explicito y evaluando el selector de tamano de pagina como `AppDropdown`.

## Problema actual

- `AppTable` ya define `25` como tamano de pagina default, pero `GestionCorrespondencia` puede sobreescribirlo con un valor distinto
- los botones de refrescar, pagina anterior y pagina siguiente usan `AppIconActionButton`, aunque el wrapper necesita controlar de forma explicita el estilo `ghost`
- el CSS `variantGhost` requerido ya existe en `AppButton`, por lo que duplicarlo en `AppIconActionButton` no aporta valor
- el control de tamano de pagina se renderiza como `AppInput type="select"` y puede verse mas como campo de formulario que como control de paginacion
- el ajuste visual de `AppInput` debe limitarse a los inputs usados dentro de `AppTableQueryWrapper`, sin crear una variante global nueva

## Alcance

- mantener el default global de `AppTable` en `25`
- alinear la inicializacion de `GestionCorrespondencia` con `pageSize = 25` si actualmente sobreescribe el valor con otro tamano
- reemplazar los usos de `AppIconActionButton` dentro de `AppTableQueryWrapper` por `AppButton`
- preservar acciones de refrescar, pagina anterior y pagina siguiente
- preservar `aria-label`, `tooltip`, `disabled`, `loading` y handlers actuales de navegacion
- mantener `variant="ghost"` en los botones de accion del wrapper
- reemplazar el selector de tamano de pagina de `AppInput type="select"` por `AppDropdown`, usando `AppButton` como trigger
- mantener opciones de tamano de pagina actuales: `10`, `25`, `50`, `100`
- aplicar estilos locales tipo `ghost` solo a los `AppInput` usados por `AppTableQueryWrapper`, especialmente el input de busqueda si sigue usando `AppInput`

## No alcance

- no cambiar el default global de `AppTable` porque ya debe permanecer en `25`
- no eliminar ni redisenar `AppIconActionButton` en todo el sistema
- no cambiar el CSS global de `AppButton` si `variantGhost` ya cumple con el estilo requerido
- no agregar una prop global `variant="ghost"` a `AppInput`
- no cambiar la estructura del contrato backend
- no modificar la paginacion server-side ni client-side fuera de `GestionCorrespondencia` y `AppTableQueryWrapper`
- no redisenar `AppTableQueryWrapper` completo

## Dependencias

- componente shared `AppTableQueryWrapper`
- componente shared `AppButton`
- componente shared `AppDropdown`
- componente shared `AppInput`
- hook consumidor de `GestionCorrespondencia`
- estilos locales de `AppTableQueryWrapper`

## Archivos frontend esperados

- `src/app/Components/UI/AppTable/AppTableQueryWrapper.tsx`
- `src/app/Components/UI/AppTable/AppTableQueryWrapper.module.css`
- `src/modules/gestionCorrespondencia/hooks/useGestionCorrespondenciaTable.ts`
- tests de `AppTableQueryWrapper`
- tests de `GestionCorrespondencia` relacionados con `pageSize`

## Reglas de implementacion

- no modificar `DEFAULT_APP_TABLE_CLIENT_PAGE_SIZE` si ya permanece en `25`
- no modificar el `DEFAULT_PAGE_SIZE` de query state si ya permanece en `25`
- `GestionCorrespondencia` debe iniciar con `pageSize = 25` o delegar en el default reusable cuando sea compatible con el hook actual
- los botones de refrescar, anterior y siguiente deben usar `AppButton` directamente dentro de `AppTableQueryWrapper`
- los botones reemplazados deben conservar comportamiento, accesibilidad y estados visuales
- el boton de refrescar debe conservar `loading` y `onRefresh`
- el boton de pagina anterior debe conservar la validacion de `canGoPrevious`
- el boton de pagina siguiente debe conservar la validacion de `canGoNext`
- el selector de tamano de pagina debe usar `AppDropdown` con un trigger basado en `AppButton`
- el trigger debe mostrar el valor actual, por ejemplo `25 por pagina`
- cada opcion del dropdown debe llamar `onQueryChange({ pageSize: option })`
- si el cambio de tamano de pagina debe reiniciar pagina, debe seguir la convencion existente del query state o del handler actual
- el input de busqueda debe seguir usando `AppInput` y su estilo especial debe vivir en `AppTableQueryWrapper.module.css`
- no introducir estilos globales para `AppInput` por este ajuste

## Riesgos a evitar

- cambiar el default global de `AppTable` por accidente
- romper la paginacion inicial de `GestionCorrespondencia`
- perder `aria-label` o `tooltip` al reemplazar `AppIconActionButton`
- cambiar el estado disabled de botones de navegacion
- duplicar estilos ya existentes de `AppButton.variantGhost`
- convertir todos los `AppInput` del sistema a estilo `ghost`
- degradar la seleccion de page size al pasar de `select` a `dropdown`
- romper tests existentes que validan `PageSize`

## Pruebas obligatorias

- `GestionCorrespondencia` inicia la consulta con `pageSize = 25`
- el request esperado de `GestionCorrespondencia` envia `PageSize = 25`
- `AppTableQueryWrapper` renderiza el boton de refrescar con `AppButton` y conserva `onRefresh`
- `AppTableQueryWrapper` conserva navegacion anterior y siguiente con los mismos estados disabled
- el selector de tamano de pagina muestra el valor actual, por ejemplo `25 por pagina`
- seleccionar una opcion del dropdown emite `onQueryChange({ pageSize: option })`
- el input de busqueda sigue emitiendo cambios de busqueda
- los estilos locales del wrapper no afectan otros usos de `AppInput`

## Criterios de aceptacion

- `AppTable` conserva `25` como default global de paginacion
- `GestionCorrespondencia` queda alineado con `25` como tamano inicial de pagina
- `AppTableQueryWrapper` usa `AppButton` para acciones de refrescar y navegacion
- los botones mantienen estilo `ghost`, accesibilidad y comportamiento previo
- el tamano de pagina se controla mediante `AppDropdown` sin perder las opciones actuales
- el ajuste visual de `AppInput` queda limitado al wrapper de `AppTable`
- las pruebas relacionadas pasan sin regresion funcional
