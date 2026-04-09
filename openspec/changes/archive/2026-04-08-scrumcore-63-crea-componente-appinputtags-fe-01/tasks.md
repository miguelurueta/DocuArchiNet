## 1. Preparacion

- [x] 1.1 Revisar `docs/Architecture/SelectDestinatario-Reusable/AppInputTags-reqs.md` y `Ticket-01-FE-AppInputTags.md` antes de implementar.
- [x] 1.2 Confirmar el naming funcional `AppInputTags` y no usar `AppAppinputtagsFe01` en rutas, exports ni nombres de componente.
- [x] 1.3 Revisar `AppInput` y `AppInputSearch` para mantener consistencia visual, accesibilidad y estilo de pruebas.

## 2. Estructura y contrato

- [x] 2.1 Crear `src/app/Components/UI/AppInputTags/` con `AppInputTags.tsx`, `AppInputTags.module.css`, `AppInputTags.test.tsx` e `index.ts`.
- [x] 2.2 Definir tipos estrictos para `AppInputTagsProps`, `AppInputTagsOption`, `AppInputTagsMode` y `AppInputTagsSize` sin usar `any`.
- [x] 2.3 Implementar props core: `value`, `defaultValue`, `mode`, `options`, `minLength`, `debounceMs`, `loading`, `clearOnEscape`, `disabled`/`selectDisabled`, `size`, `label`, `aria-label`, `aria-labelledby`.
- [x] 2.4 Implementar callbacks `onAddTag`, `onRemoveTag`, `onRemoveAll` y `onSearch`.

## 3. Comportamiento del componente

- [x] 3.1 Implementar modo controlado y no controlado con `value` como fuente de verdad cuando exista.
- [x] 3.2 Implementar `mode="single"` para reemplazar el tag visible al agregar un nuevo valor.
- [x] 3.3 Implementar `mode="multiple"` para acumular tags sin mutar props.
- [x] 3.4 Implementar adicion por seleccion de sugerencia y por confirmacion manual sin usar `KeyPress`.
- [x] 3.5 Implementar eliminacion individual de tags con accion accesible.
- [x] 3.6 Implementar eliminacion masiva con `onRemoveAll`.
- [x] 3.7 Implementar busqueda con `minLength`, `debounceMs`, cancelacion de debounce pendiente en Enter/click y limpieza de timers al desmontar.
- [x] 3.8 Implementar clear y `clearOnEscape` sin disparar automaticamente `onSearch("")`.
- [x] 3.9 Implementar `loading` como indicador visual sin bloquear input salvo que `disabled`/`selectDisabled` este activo.

## 4. UI, estilos y export

- [x] 4.1 Renderizar el control con primitives de Ant Design adecuadas, por ejemplo `AutoComplete`, `Input`, `Tag` y `Spin`, sin usar `Input.Search`.
- [x] 4.2 Aplicar estilos locales con CSS module alineados a `AppInput`: border radius 12px, focus, hover, error, disabled y spacing.
- [x] 4.3 Implementar clases de size `sm`, `md` y `lg` para altura, padding e iconos.
- [x] 4.4 Integrar acciones secundarias mediante `toolbar` o `AppDropdown` cuando aplique, sin acoplar logica de dominio.
- [x] 4.5 Exportar `AppInputTags` y sus tipos desde `src/app/Components/UI/AppInputTags/index.ts`.
- [x] 4.6 Exportar `AppInputTags` desde `src/app/Components/UI/index.ts`.

## 5. Accesibilidad

- [x] 5.1 Asegurar que el input expone nombre accesible mediante `label`, `aria-label` o `aria-labelledby`.
- [x] 5.2 Asegurar que cada accion de remover tag tiene nombre accesible que identifica el tag.
- [x] 5.3 Asegurar que la accion de remover todos expone `aria-label="Eliminar todos"` o equivalente.
- [x] 5.4 Validar navegacion por teclado para sugerencias de autocomplete.
- [x] 5.5 Asegurar que `loading` no rompe foco ni navegacion.

## 6. Pruebas

- [x] 6.1 Agregar tests `[SPEC:app-input-tags]` para disponibilidad del componente y export compartido.
- [x] 6.2 Agregar tests de modo controlado y no controlado.
- [x] 6.3 Agregar tests de `single` y `multiple`.
- [x] 6.4 Agregar tests de `onAddTag`, `onRemoveTag` y `onRemoveAll`.
- [x] 6.5 Agregar tests de `onSearch`, `minLength`, `debounceMs`, Enter/click inmediato y cancelacion de debounce pendiente.
- [x] 6.6 Agregar tests de options vacio, render de sugerencias y seleccion por teclado.
- [x] 6.7 Agregar tests de clear, `clearOnEscape`, loading y disabled.
- [x] 6.8 Agregar tests de clases de size, estilos base observables y accesibilidad.

## 7. Validacion

- [x] 7.1 Ejecutar tests focales de `AppInputTags`.
- [x] 7.2 Ejecutar lint focal sobre archivos creados/modificados.
- [x] 7.3 Ejecutar `npx.cmd tsc -b` o documentar cualquier fallo preexistente no relacionado.
- [x] 7.4 Ejecutar `npx.cmd openspec validate scrumcore-63-crea-componente-appinputtags-fe-01 --strict`.
- [x] 7.5 Actualizar `tasks.md` con evidencia de validacion antes de cerrar implementacion.

## Evidencia de validacion

- `npm.cmd test -- src/app/Components/UI/AppInputTags/AppInputTags.test.tsx`: paso, 14 tests.
- `npx.cmd eslint src/app/Components/UI/AppInputTags/AppInputTags.tsx src/app/Components/UI/AppInputTags/AppInputTags.test.tsx src/app/Components/UI/AppInputTags/index.ts src/app/Components/UI/index.ts`: paso.
- `npx.cmd tsc -b`: paso.
- `npx.cmd openspec validate scrumcore-63-crea-componente-appinputtags-fe-01 --strict`: paso.
