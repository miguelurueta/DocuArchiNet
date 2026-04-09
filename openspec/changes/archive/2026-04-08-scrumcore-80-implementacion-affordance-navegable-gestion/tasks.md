## 1. Adopcion del contrato shared

- [x] 1.1 Revisar la implementacion actual de `GestionCorrespondencia.tsx`
- [x] 1.2 Reemplazar `gridClassName={styles.navigableGrid}` por `rowClickAffordance`
- [x] 1.3 Mantener intactos `onCellClicked` y `onActionTriggered`
- [x] 1.4 No agregar una capa adicional de teclado en el modulo

## 2. Limpieza de CSS local

- [x] 2.1 Identificar reglas del modulo usadas solo para affordance navegable
- [x] 2.2 Eliminar reglas de cursor local para celdas navegables
- [x] 2.3 Eliminar reglas locales equivalentes de hover o affordance si existen
- [x] 2.4 Eliminar clases residuales no utilizadas relacionadas con este patron

## 3. Verificaciones funcionales

- [x] 3.1 Verificar que click en celda de datos sigue navegando a `respuesta/:id`
- [x] 3.2 Verificar que la columna `acciones` no dispara navegacion accidental por click de celda
- [x] 3.3 Verificar que el menu contextual de acciones sigue funcionando
- [x] 3.4 Verificar que la columna de seleccion no adquiere affordance navegable
- [x] 3.5 Verificar que `Enter` sigue funcionando a traves de `AppTable`

## 4. Pruebas

- [x] 4.1 Ajustar pruebas de `GestionCorrespondencia` para validar `rowClickAffordance`
- [x] 4.2 Ajustar pruebas para confirmar que ya no se usa `gridClassName={styles.navigableGrid}`
- [x] 4.3 Cubrir que la navegacion por celda de datos sigue intacta
- [x] 4.4 Cubrir que la columna `acciones` no navega por click de celda
- [x] 4.5 Ejecutar tests focales del modulo

## 5. Verificacion final

- [x] 5.1 Ejecutar `openspec validate scrumcore-80-implementacion-affordance-navegable-gestion --strict`
- [x] 5.2 Confirmar ausencia de CSS local duplicado para este patron
