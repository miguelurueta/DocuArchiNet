## 1. Preparacion y alineacion

- [x] 1.1 Revisar `docs/Architecture/SelectDestinatario-Reusable/Ticket-02-FE-AutoComplete.md` y `AppInputTags-reqs.md`.
- [x] 1.2 Confirmar que el cambio modifica `app-input-tags` y no crea `AppAppinputtagsFe02` ni una capability nueva.
- [x] 1.3 Revisar `src/app/Components/UI/AppInputTags/` y la spec principal `openspec/specs/app-input-tags/spec.md`.
- [x] 1.4 Revisar `useAutocompleteCamposPlantilla` como referencia de hook consumidor que normaliza opciones.

## 2. Contrato de opciones y metadata

- [x] 2.1 Extender `AppInputTagsOption` para soportar metadata opcional sin usar `any`.
- [x] 2.2 Mantener compatibilidad con opciones existentes `{ value, label, id }`.
- [x] 2.3 Asegurar que `AppInputTags` renderiza sugerencias solo desde `label` y `value` sin interpretar metadata de dominio.
- [x] 2.4 Verificar que `options` se trata como prop inmutable.

## 3. Autocomplete y eventos

- [x] 3.1 Verificar que `onSearch(query)` se dispara solo cuando el texto cumple `minLength`.
- [x] 3.2 Verificar que `debounceMs` controla el retraso de busqueda por escritura.
- [x] 3.3 Verificar que Enter cancela debounce pendiente y dispara busqueda inmediata.
- [x] 3.4 Verificar que click en icono de busqueda cancela debounce pendiente y dispara busqueda inmediata.
- [x] 3.5 Verificar que `loading` muestra indicador visual sin bloquear el input salvo `disabled` o `selectDisabled`.

## 4. Acciones secundarias

- [x] 4.1 Revisar el slot actual `toolbar` de `AppInputTags` y documentar si cubre la composicion requerida.
- [x] 4.2 Ajustar el contrato de acciones secundarias solo si el slot actual no permite integrar `AppDropdown` de forma accesible.
- [x] 4.3 Mantener la accion de eliminar todos accesible y no dependiente de logica de dominio.
- [x] 4.4 Asegurar que acciones secundarias y loading no bloquean autocomplete ni edicion de texto.

## 5. Pruebas

- [x] 5.1 Agregar o actualizar tests `[SPEC:app-input-tags]` para metadata opcional en `options`.
- [x] 5.2 Agregar o actualizar tests de desacoplamiento: no se importan hooks ni servicios de dominio en `AppInputTags`.
- [x] 5.3 Agregar o actualizar tests de `minLength`, `debounceMs`, Enter/click inmediato y cancelacion de debounce pendiente.
- [x] 5.4 Agregar o actualizar tests de `loading` no bloqueante.
- [x] 5.5 Agregar o actualizar tests de slot/acciones secundarias accesibles.
- [x] 5.6 Si se toca un consumidor real, actualizar sus pruebas focales. No aplica: no se modifico consumidor real.

## 6. Validacion

- [x] 6.1 Ejecutar tests focales de `AppInputTags`.
- [x] 6.2 Ejecutar lint focal sobre archivos creados/modificados.
- [x] 6.3 Ejecutar `npx.cmd tsc -b` o documentar cualquier fallo preexistente no relacionado.
- [x] 6.4 Ejecutar `npx.cmd openspec validate scrumcore-64-crea-componente-appinputtags-fe-02 --strict`.
- [x] 6.5 Actualizar `tasks.md` con evidencia de validacion antes de cerrar implementacion.

## Evidencia de validacion

- `npm.cmd test -- src/app/Components/UI/AppInputTags/AppInputTags.test.tsx`: paso, 17 tests.
- `npx.cmd eslint src/app/Components/UI/AppInputTags/AppInputTags.tsx src/app/Components/UI/AppInputTags/AppInputTags.test.tsx src/app/Components/UI/AppInputTags/index.ts src/app/Components/UI/index.ts`: paso.
- `npx.cmd tsc -b`: paso.
- `npx.cmd openspec validate scrumcore-64-crea-componente-appinputtags-fe-02 --strict`: paso.
- `npm.cmd run build`: paso fuera del sandbox; Vite reporto el warning existente de chunk grande.
