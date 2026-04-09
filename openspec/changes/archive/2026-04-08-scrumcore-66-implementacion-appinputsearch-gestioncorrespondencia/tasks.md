## 1. AppInputSearch Core

- [x] 1.1 Revisar los usos actuales de `AppInputSearch` para planear la migracion de `onChange(event)` a `onChange(value)`.
- [x] 1.2 Reimplementar `AppInputSearch` con composicion `AutoComplete` + `Input` sin usar `Input.Search` ni consumir APIs.
- [x] 1.3 Definir el contrato tipado con `value`, `defaultValue`, `options`, `loading`, `clearOnEscape`, `debounceMs`, `minLength`, `onChange(value)`, `onSearch(value)`, `onClear`, `size`, `aria-label`, `aria-labelledby`, `className`, `error`, `state` y `helperText`.
- [x] 1.4 Implementar modo controlado y no controlado sin mantener fuentes de verdad duplicadas.
- [x] 1.5 Implementar `onChange(value)` en cada cambio de texto.
- [x] 1.6 Implementar `onSearch(value)` por Enter, click en icono y debounce de escritura.
- [x] 1.7 Cancelar o neutralizar debounce pendiente cuando Enter o click en icono ejecuten busqueda inmediata.
- [x] 1.8 Aplicar `minLength` a busqueda por debounce, Enter, click en icono y seleccion de opcion.
- [x] 1.9 Implementar clear con `onChange("")`, `onClear()` y sin `onSearch("")` automatico.
- [x] 1.10 Implementar `clearOnEscape` respetando `disabled`.
- [x] 1.11 Implementar `options` de autocomplete sin mutar props y con seleccion deterministica `onChange(selectedValue)` + `onSearch(selectedValue)`.
- [x] 1.12 Implementar loading visual sin bloquear escritura ni perder foco, con prioridad de `disabled` sobre `loading`.
- [x] 1.13 Implementar variantes `size="sm" | "md" | "lg"` con default `md`.
- [x] 1.14 Ajustar estilos CSS module de `AppInputSearch` para mantener consistencia visual con `AppInput` y evitar estilos globales.
- [x] 1.15 Actualizar el barrel export si el contrato publico cambia.

## 2. Consumidores De AppInputSearch

- [x] 2.1 Migrar `AppTableQueryWrapper` al contrato `onChange(value)` sin cambiar su comportamiento de `onQueryChange({ search })`.
- [x] 2.2 Validar que `AppTableQueryWrapper showSearch={false}` siga evitando el render del buscador.
- [x] 2.3 Actualizar la documentacion tecnica de `docs/Components/AppInputSearch/README.md` para reflejar el contrato final.

## 3. Gestion Correspondencia

- [x] 3.1 Importar y renderizar `AppInputSearch` dentro de `AppToolbar.actionContent` en `GestionCorrespondencia.tsx`.
- [x] 3.2 Conectar `value={table.queryState.search}` como fuente unica del valor.
- [x] 3.3 Conectar `onChange={(value) => table.onQueryChange({ search: value })}` sin disparar servicios ni endpoints desde la pagina.
- [x] 3.4 Mantener `AppTableQueryWrapper` con `showSearch={false}` para evitar duplicidad.
- [x] 3.5 Preservar acciones existentes del toolbar, exportacion, seleccion y paginacion.
- [x] 3.6 Agregar `toolbarSearch` en el CSS module de Gestion Correspondencia solo para layout, ancho, flex y separacion.

## 4. Mapper De Busqueda LIKE

- [x] 4.1 Cambiar `mapGestionCorrespondenciaTableRequest` para envolver `mapDynamicUiServerTableRequest` en lugar de reexportarlo directamente.
- [x] 4.2 Resolver `SearchType = 2` cuando `search.trim().length > 0` y no exista override avanzado.
- [x] 4.3 Preservar `SearchType = 3` cuando la busqueda avanzada lo defina explicitamente.
- [x] 4.4 No forzar `SearchType = 2` cuando `search` este vacio o solo tenga espacios.
- [x] 4.5 Preservar `Page`, `PageSize`, `StructuredFilters`, `SortField`, `SortDir` e `IncludeConfig` generados por el mapper shared.
- [x] 4.6 Evitar cambios en el mapper shared de `AppTable` para no afectar otras tablas.

## 5. Pruebas

- [x] 5.1 Actualizar o crear pruebas de `AppInputSearch` para controlado, no controlado, `onChange(value)`, clear, Escape, loading, options, tamanos y accesibilidad.
- [x] 5.2 Cubrir `onSearch` por Enter, click en icono, debounce, `minLength` y cancelacion de debounce pendiente.
- [x] 5.3 Actualizar pruebas de `AppTableQueryWrapper` para el nuevo contrato de `AppInputSearch` y `showSearch={false}`.
- [x] 5.4 Actualizar pruebas de `GestionCorrespondencia` para validar el buscador en toolbar, un unico buscador visible, `aria-label` y acciones existentes.
- [x] 5.5 Actualizar pruebas del mapper de Gestion Correspondencia para `SearchType = 2`, `SearchType = 3`, search vacio, paginacion, filtros, sort e `IncludeConfig`.

## 6. Validacion

- [x] 6.1 Ejecutar pruebas focales de `AppInputSearch`, `AppTableQueryWrapper` y Gestion Correspondencia.
- [x] 6.2 Ejecutar ESLint focal sobre los archivos tocados.
- [x] 6.3 Ejecutar `npx.cmd tsc -b`.
- [x] 6.4 Ejecutar `npx.cmd openspec validate scrumcore-66-implementacion-appinputsearch-gestioncorrespondencia --strict`.
- [x] 6.5 Ejecutar `git diff --check`.
- [x] 6.6 Registrar en este archivo la evidencia de validacion antes de archivar.

## 7. Evidencia De Validacion

- [x] 7.1 `npm.cmd test -- src/app/Components/UI/AppInputSearch/AppInputSearch.test.tsx src/app/Components/UI/AppTable/tests/AppTableQueryWrapper.test.tsx src/modules/gestionCorrespondencia/tests/GestionCorrespondencia.test.tsx src/modules/gestionCorrespondencia/tests/useGestionCorrespondenciaTable.test.tsx src/modules/gestionCorrespondencia/tests/gestionCorrespondenciaTableRequestMapper.test.ts` paso: 11 files, 42 tests.
- [x] 7.2 `npx.cmd eslint ...` sobre archivos tocados paso sin errores.
- [x] 7.3 `npx.cmd tsc -b` paso.
- [x] 7.4 `npx.cmd openspec validate scrumcore-66-implementacion-appinputsearch-gestioncorrespondencia --strict` paso.
- [x] 7.5 `git diff --check` paso.
