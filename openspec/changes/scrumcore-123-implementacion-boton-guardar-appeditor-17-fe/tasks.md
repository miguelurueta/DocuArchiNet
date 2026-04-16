## 1. Normalizacion y baseline de contenido

- [x] 1.1 Definir una funcion compartida para normalizar HTML serializado del editor
- [x] 1.2 Cubrir equivalencias de contenido vacio (`""`, `<p></p>`, `<p><br></p>`)
- [x] 1.3 Definir comparacion real entre `currentValue` y `savedValue` basada en contenido normalizado
- [x] 1.4 Confirmar que no se usan heuristicas simplistas para el dirty state

## 2. Dirty state fuera de `AppEditor`

- [x] 2.1 Implementar el estado de guardado en el contenedor consumidor o en un hook de application del modulo
- [x] 2.2 Gestionar `currentValue`, `savedValue` e `isDirty` fuera del componente shared
- [x] 2.3 Sincronizar baseline cuando el `value` controlado cambie externamente
- [x] 2.4 Confirmar que `AppEditor` permanece agnostico a persistencia real

## 3. Presentacion del boton `Guardar`

- [x] 3.1 Integrar el boton `Guardar` en `headerActions` o en el shell inmediato del editor
- [x] 3.2 Renderizar estado gris cuando no existan cambios pendientes
- [x] 3.3 Renderizar estado negro cuando el contenido difiera del ultimo baseline guardado
- [x] 3.4 Confirmar que `Guardar` no aparece dentro de la toolbar de formato

## 4. Guardado simulado y preparacion para backend

- [x] 4.1 Implementar guardado simulado actualizando solo `savedValue`
- [x] 4.2 Confirmar que al guardar el estado vuelve a no dirty
- [x] 4.3 Confirmar que una nueva edicion tras guardar vuelve a marcar dirty
- [x] 4.4 Dejar clara una extension futura compatible con un contrato tipo `saveDraft` sin implementarlo aun

## 5. Pruebas y evidencia

- [x] 5.1 Agregar pruebas para estado inicial gris
- [x] 5.2 Agregar pruebas para cambio a negro tras edicion
- [x] 5.3 Agregar pruebas para guardado simulado y re-edicion posterior
- [x] 5.4 Agregar pruebas especificas de normalizacion de HTML vacio
- [x] 5.5 Ejecutar pruebas focalizadas del editor y registrar resultados
- [x] 5.6 Ejecutar validacion TypeScript o equivalente y registrar residuos ajenos si aparecen
- [x] 5.7 Registrar evidencia final en este archivo

## Evidencia

- Se agrego `normalizeEditorHtml` como utilidad compartida para comparar contenido serializado sin falsos positivos por variantes equivalentes de editor vacio.
- Se agrego `useAppEditorSaveState` y el tipo `AppEditorSaveStatus` para encapsular dirty state real fuera de `AppEditor`.
- Se agrego `AppEditorSaveAction` como companion reutilizable para `headerActions`, con estilo gris en `idle` y negro en `dirty`.
- `AppEditor` permanece agnostico a persistencia; el flujo de guardado queda delegado al consumidor mediante `value`, `onChange` y `headerActions`.
- Se documento el patron de consumo en `README.md` y se expusieron los companions desde el barrel shared UI.
- Pruebas focalizadas ejecutadas con Vitest:
  - `9 files passed`
  - `52 tests passed`
- Validacion TypeScript ejecutada:
  - sin errores nuevos atribuibles a `SCRUMCORE-123`
  - persisten errores preexistentes en `src/app/Components/UI/AppTabs/AppTabs.tsx`
  - persisten errores preexistentes en `src/setupTests.ts`
