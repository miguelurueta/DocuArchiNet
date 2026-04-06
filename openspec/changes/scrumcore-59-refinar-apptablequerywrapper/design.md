## Context

`AppTableQueryWrapper` concentra los controles reutilizables de busqueda, refresco y paginacion que consumen las tablas basadas en `AppTable`. Actualmente usa `AppIconActionButton` para refrescar, pagina anterior y pagina siguiente, y usa `AppInput type="select"` para elegir el tamano de pagina.

El ticket SCRUMCORE-59 refina ese wrapper a partir del documento arquitectonico `25-FE-Refinar-controles-AppTableQueryWrapper`. El objetivo es mantener `AppTable` con paginacion default `25`, alinear `GestionCorrespondencia` con ese valor inicial y hacer que los controles del wrapper expresen mejor su intencion visual sin introducir variantes globales nuevas.

El CSS requerido para botones `ghost` ya existe en `AppButton` mediante `variant="ghost"`. Por eso el cambio no necesita modificar `AppIconActionButton` globalmente ni duplicar estilos. El ajuste de `AppInput` debe quedar limitado al contexto local de `AppTableQueryWrapper`.

## Goals / Non-Goals

**Goals:**

- Mantener el default global de `AppTable` y query state en `25`.
- Alinear la inicializacion de `GestionCorrespondencia` para que use `pageSize = 25`.
- Reemplazar los usos de `AppIconActionButton` dentro de `AppTableQueryWrapper` por `AppButton` explicito.
- Preservar accesibilidad, tooltip, loading, disabled y handlers de los botones de refresco y navegacion.
- Cambiar el control de tamano de pagina del wrapper de `AppInput type="select"` a `AppDropdown` con trigger `AppButton`.
- Mantener el input de busqueda como `AppInput`, aplicando cualquier ajuste visual solo desde `AppTableQueryWrapper.module.css`.

**Non-Goals:**

- No cambiar `DEFAULT_APP_TABLE_CLIENT_PAGE_SIZE` ni el default reusable de query state si ya estan en `25`.
- No eliminar, redisenar ni reemplazar `AppIconActionButton` fuera de `AppTableQueryWrapper`.
- No agregar una variante global nueva a `AppInput`.
- No cambiar la arquitectura de paginacion server-side o client-side de `AppTable`.
- No modificar contratos backend ni DTOs de tabla dinamica.
- No introducir dependencias nuevas.

## Decisions

1. Usar `AppButton` directamente para botones de accion del wrapper.

   `AppIconActionButton` es un wrapper conveniente, pero en este caso el componente contenedor necesita expresar de forma explicita `variant="ghost"` y conservar control local sobre clases y layout. Se reemplazaran los tres usos actuales por `AppButton` con `icon`, `aria-label`, `tooltip`, `variant="ghost"` y `size="md"`.

   Alternativa considerada: ajustar `AppIconActionButton`. Se descarta porque el CSS `variantGhost` ya vive en `AppButton` y cambiar el wrapper global ampliaria el alcance sin necesidad.

2. Usar `AppDropdown` para el selector de tamano de pagina.

   El tamano de pagina funciona como una accion de paginacion dentro de la banda de controles, no como un campo de formulario editable. `AppDropdown` permite modelarlo como menu de opciones con trigger `AppButton variant="ghost"`, manteniendo las opciones actuales `10`, `25`, `50` y `100`.

   Alternativa considerada: mantener `AppInput type="select"` y solo cambiar CSS. Se descarta porque mantiene la semantica visual de input y no aprovecha el componente reusable de dropdown ya existente.

3. Mantener `AppInput` solo para busqueda dentro del wrapper.

   La busqueda sigue siendo un campo textual, por lo que debe permanecer como `AppInput`. Si requiere estilo tipo `ghost`, ese ajuste se hara con clases locales del wrapper, por ejemplo sobre `styles.searchInput`, sin agregar una prop global a `AppInput`.

   Alternativa considerada: agregar `variant="ghost"` a `AppInput`. Se descarta porque el requerimiento aplica solo a `AppTableQueryWrapper` y una variante global aumentaria la superficie de pruebas y regresiones.

4. Alinear `GestionCorrespondencia` con `pageSize = 25` sin cambiar defaults globales.

   El default compartido ya es `25`; si `GestionCorrespondencia` inicializa el hook con `pageSize = 10`, debe actualizarse a `25` o remover el override si el hook puede usar el default de forma segura. La decision final de implementacion debe elegir la opcion con menor impacto en el contrato local del hook y sus pruebas.

   Alternativa considerada: cambiar el default global de `AppTable`. Se descarta porque el default requerido ya es `25` y el riesgo seria introducir cambios innecesarios en otros consumidores.

## Risks / Trade-offs

- Cambiar un `select` por `dropdown` puede modificar expectativas de tests y consultas por rol accesible -> actualizar pruebas para validar comportamiento observable: label del trigger, apertura y seleccion de opcion.
- El trigger de `AppDropdown` requiere nombre accesible -> usar texto visible como `25 por pagina` y/o `ariaLabel="Cantidad de registros por pagina"`.
- `AppButton` icon-only lanza error si no tiene `aria-label` -> conservar `aria-label` en refrescar, anterior y siguiente.
- Al cambiar `GestionCorrespondencia` de `10` a `25`, tests existentes que esperan `PageSize: 10` fallaran -> actualizar expectativas a `25` y verificar que el request siga usando el resto de filtros y ordenamiento sin cambios.
- Estilos locales sobre `AppInput` pueden no alcanzar wrappers internos de Ant Design -> limitar el ajuste a clases del wrapper y, si hace falta, usar selectores scoped contra los nodos internos renderizados dentro de `searchInput`, sin tocar `AppInput.module.css`.
- El cambio no debe afectar usos externos de `AppIconActionButton` -> mantener el reemplazo acotado a `AppTableQueryWrapper.tsx`.

## Migration Plan

1. Actualizar `AppTableQueryWrapper.tsx` para importar `AppButton` y `AppDropdown`.
2. Reemplazar los tres `AppIconActionButton` por `AppButton` con las mismas acciones y estados.
3. Reemplazar el `AppInput type="select"` de page size por `AppDropdown` con trigger `AppButton`.
4. Mantener `AppInput` para busqueda y ajustar estilos locales en `AppTableQueryWrapper.module.css` si es necesario.
5. Cambiar la inicializacion de `GestionCorrespondencia` para que use `pageSize = 25` o delegue en el default reusable.
6. Actualizar pruebas de `AppTableQueryWrapper` y `GestionCorrespondencia` afectadas por page size y controles.

Rollback: revertir los cambios en `AppTableQueryWrapper`, estilos locales y `useGestionCorrespondenciaTable`; no requiere migracion de datos ni cambios backend.

## Open Questions

- Confirmar durante implementacion si `GestionCorrespondencia` debe declarar explicitamente `pageSize: 25` o si es preferible remover el override para delegar en `useAppTableQueryState`.
- Confirmar si el dropdown de page size debe reiniciar la pagina a `1` al cambiar el tamano, o conservar exactamente la convencion actual de `onQueryChange({ pageSize })`.
