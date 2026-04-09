## 1. Contrato shared

- [x] 1.1 Agregar `rowClickAffordance?: boolean` al contrato publico de `AppTable`
- [x] 1.2 Asegurar default `false` sin cambios de comportamiento en consumidores existentes
- [x] 1.3 Mantener tipado estricto sin usar `any`

## 2. Grid renderer

- [x] 2.1 Revisar `AppTableGridRenderer` y el flujo actual de `onRowClicked` / `onCellClicked`
- [x] 2.2 Implementar affordance reusable con `cellClass` o `cellClassRules`
- [x] 2.3 Excluir columna `acciones`
- [x] 2.4 Excluir columna de seleccion
- [x] 2.5 Excluir superficies explicitamente no navegables si existen flags o metadata compatibles
- [x] 2.6 Asegurar que la implementacion no manipula DOM directamente

## 3. Estilos shared

- [x] 3.1 Agregar clase reusable en `AppTable.module.css` para cursor navegable
- [x] 3.2 Agregar hover ligero consistente con el design system
- [x] 3.3 Agregar transicion suave sin modificar layout
- [x] 3.4 Verificar que controles internos no pierden su cursor o interaccion propia

## 4. Teclado y accesibilidad

- [x] 4.1 Revisar el comportamiento actual de keyboard navigation en AG Grid
- [x] 4.2 Implementar soporte de `Enter` solo cuando `rowClickAffordance` este activo
- [x] 4.3 Reutilizar el flujo observable del consumidor sin hardcodear navegacion
- [x] 4.4 Evitar activacion por `Enter` en acciones, seleccion y controles interactivos internos
- [x] 4.5 Confirmar que no se rompe Tab ni foco existente del grid

## 5. No interferencia

- [x] 5.1 Verificar que `onRowClicked` mantiene su contrato
- [x] 5.2 Verificar que `onCellClicked` mantiene su contrato
- [x] 5.3 Verificar que `onActionTriggered` mantiene su contrato
- [x] 5.4 Verificar que no cambia bubbling ni prioridad de eventos actuales
- [x] 5.5 Verificar que la seleccion de filas sigue intacta

## 6. Pruebas

- [x] 6.1 Agregar prueba: sin `rowClickAffordance` no hay affordance visual
- [x] 6.2 Agregar prueba: con `rowClickAffordance` hay affordance en celdas de datos
- [x] 6.3 Agregar prueba: columna `acciones` no recibe affordance
- [x] 6.4 Agregar prueba: columna de seleccion no recibe affordance
- [x] 6.5 Agregar prueba: controles internos mantienen comportamiento
- [x] 6.6 Agregar prueba: `Enter` dispara la accion primaria esperada
- [x] 6.7 Agregar prueba: `Enter` no activa acciones, seleccion ni controles internos
- [x] 6.8 Agregar prueba: la seleccion de filas no se rompe
- [x] 6.9 Ejecutar validacion focal de tests de `AppTable`

## 7. Verificacion final

- [x] 7.1 Ejecutar `openspec validate scrumcore-79-implementa-affordance-apptable --strict`
- [x] 7.2 Confirmar que el componente queda listo para adopcion posterior en `GestionCorrespondencia`
