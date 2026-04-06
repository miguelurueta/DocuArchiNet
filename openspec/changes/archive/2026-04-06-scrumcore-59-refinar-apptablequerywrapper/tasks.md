## 1. Pruebas de comportamiento

- [x] 1.1 Actualizar o crear pruebas de `AppTableQueryWrapper` para validar que refrescar, pagina anterior y pagina siguiente se renderizan como acciones accesibles y conservan sus handlers.
- [x] 1.2 Actualizar o crear pruebas de `AppTableQueryWrapper` para validar que el selector de tamano de pagina muestra el valor actual y lista las opciones configuradas.
- [x] 1.3 Actualizar o crear pruebas de `AppTableQueryWrapper` para validar que seleccionar una opcion de tamano de pagina emite `onQueryChange({ pageSize: selectedOption })`.
- [x] 1.4 Actualizar o crear pruebas de `AppTableQueryWrapper` para validar que el input de busqueda conserva `onQueryChange({ search })` y se omite cuando `showSearch=false`.
- [x] 1.5 Actualizar pruebas de `GestionCorrespondencia` para esperar `pageSize = 25` y `PageSize = 25` sin cambiar filtros ni ordenamiento existentes.

## 2. Refinamiento de AppTableQueryWrapper

- [x] 2.1 Reemplazar el import y los usos de `AppIconActionButton` por `AppButton` dentro de `AppTableQueryWrapper.tsx`.
- [x] 2.2 Configurar los botones de refrescar, pagina anterior y pagina siguiente con `variant="ghost"`, `size="md"`, `icon`, `aria-label`, `tooltip` y estados `loading` o `disabled` equivalentes.
- [x] 2.3 Reemplazar el `AppInput type="select"` de tamano de pagina por `AppDropdown` con trigger `AppButton`.
- [x] 2.4 Construir los items del dropdown desde `pageSizeOptions` usando etiquetas `<valor> por pagina` y callbacks `onSelect`.
- [x] 2.5 Mantener el `AppInput` de busqueda y limitar cualquier estilo especial a `AppTableQueryWrapper.module.css`.

## 3. Ajuste de GestionCorrespondencia

- [x] 3.1 Revisar la inicializacion de `useGestionCorrespondenciaTable` y decidir si declarar `pageSize: 25` explicitamente o delegar en el default reusable.
- [x] 3.2 Aplicar el ajuste seleccionado para que `GestionCorrespondencia` inicie con `pageSize = 25`.
- [x] 3.3 Verificar que el request inicial mantiene pagina, filtros y ordenamiento previos, cambiando solo el tamano de pagina esperado.

## 4. Validacion

- [x] 4.1 Ejecutar las pruebas focales de `AppTableQueryWrapper` y `GestionCorrespondencia`.
- [x] 4.2 Ejecutar `openspec validate scrumcore-59-refinar-apptablequerywrapper --strict`.
- [x] 4.3 Registrar en la respuesta final la evidencia de pruebas ejecutadas y cualquier limitacion encontrada.
