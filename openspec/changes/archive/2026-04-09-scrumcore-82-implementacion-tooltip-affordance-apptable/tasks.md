## 1. Contrato reusable en AppTable
- [x] 1.1 Agregar `rowClickTooltip?: string` al contrato tipado de `AppTable` sin alterar props ni callbacks existentes.
- [x] 1.2 Hacer que el tooltip solo sea elegible cuando `rowClickAffordance` este activo y exista texto configurado.
- [x] 1.3 Mantener el comportamiento por defecto sin tooltip ni affordance textual cuando el consumidor no configure la nueva prop.

## 2. Integracion en grid y cards
- [x] 2.1 Implementar soporte de tooltip en `cards` usando una primitiva reusable del design system sobre la superficie navegable valida.
- [x] 2.2 Implementar soporte de tooltip en `table` con una estrategia liviana compatible con AG Grid, evitando un wrapper React costoso por cada celda navegable.
- [x] 2.3 Reutilizar la misma semantica de elegibilidad ya usada por `rowClickAffordance` para excluir columna de acciones, columna de seleccion y superficies no navegables.
- [x] 2.4 Excluir controles interactivos internos (`button`, `a`, `input`, `textarea`, `select`, `[role="button"]` y menus) para no competir con su foco ni con sus eventos.

## 3. Preservacion de contratos y accesibilidad
- [x] 3.1 Verificar que el tooltip siga siendo presentacional y no altere `onCellClicked`, `onRowClicked` ni `onActionTriggered`.
- [x] 3.2 Mantener navegacion por click y por `Enter` sin introducir bubbling inesperado ni interceptar interaccion primaria.
- [x] 3.3 Validar hover y foco cuando apliquen, sin ruido redundante para accesibilidad ni regresiones en controles internos.

## 4. Pruebas y validacion
- [x] 4.1 Agregar pruebas de `AppTable` para confirmar que el tooltip no se renderiza por defecto.
- [x] 4.2 Agregar pruebas para confirmar que el tooltip solo se activa con `rowClickAffordance` + `rowClickTooltip`.
- [x] 4.3 Cubrir exclusiones de acciones, seleccion y controles interactivos internos.
- [x] 4.4 Cubrir soporte de tooltip en `cards` y preservacion de la accion primaria existente.
- [x] 4.5 Validar que la estrategia de grid no introduce regresiones funcionales ni altera eventos observables.
- [x] 4.6 Ejecutar pruebas focales de `AppTable`, validacion de tipos y `openspec validate` del cambio antes de preparar el PR.
