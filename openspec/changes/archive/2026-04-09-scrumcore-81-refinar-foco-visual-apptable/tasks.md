## 1. Analisis del foco visual actual

- [x] 1.1 Revisar en `AppTableGridRenderer` y `useAgGridBaseConfig` como se habilita el foco funcional cuando `rowClickAffordance` esta activo
- [x] 1.2 Identificar en `AppTable.module.css` y en los estados de AG Grid la clase o combinacion visual que produce el recuadro de celda
- [x] 1.3 Confirmar que el soporte de `Enter` depende del foco funcional actual y no de un workaround alterno

## 2. Implementacion visual scoped

- [x] 2.1 Aplicar o reutilizar una clase raiz scoped cuando `rowClickAffordance` este activo
- [x] 2.2 Refinar visualmente `ag-cell-focus` y estados relacionados sin eliminar el foco funcional del grid
- [x] 2.3 Mantener diferenciados hover navegable, foco tecnico y seleccion real de fila
- [x] 2.4 Verificar que la solucion no afecte tablas que no usan `rowClickAffordance`

## 3. Proteccion de superficies especiales

- [x] 3.1 Verificar que columnas de acciones y seleccion sigan excluidas del affordance navegable
- [x] 3.2 Verificar que controles interactivos internos conserven su foco visible y accesibilidad
- [x] 3.3 Confirmar que el ajuste no altere el comportamiento de `presentationMode=\"cards\"`

## 4. Pruebas

- [x] 4.1 Ajustar pruebas de `AppTable` para cubrir el scope visual del foco cuando `rowClickAffordance` esta activo
- [x] 4.2 Cubrir que `Enter` sigue funcionando y que `onCellClicked` conserva su contrato
- [x] 4.3 Cubrir que columnas excluidas no disparan regresiones funcionales
- [x] 4.4 Ejecutar tests focales de `AppTable`
- [x] 4.5 Ejecutar validacion de tipos si el cambio toca props, renderer o estilos compartidos relevantes

## 5. Verificacion final

- [x] 5.1 Confirmar visualmente que el foco de celda ya no se percibe como una segunda seleccion
- [x] 5.2 Confirmar que seleccion de fila sigue siendo el estado visual dominante
- [x] 5.3 Ejecutar `openspec validate scrumcore-81-refinar-foco-visual-apptable --strict`
- [x] 5.4 Ejecutar `git diff --check`
