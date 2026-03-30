## 1. Definicion del contrato AppDropdown

- [x] 1.1 Alinear los artefactos del cambio para que usen el nombre canonico `AppDropdown` y la capability `app-dropdown`
- [x] 1.2 Revisar el spec `app-dropdown` y cerrar el alcance inicial sobre items planos, metadata visual y modo controlado/no controlado de apertura

## 2. Implementacion del componente compartido

- [x] 2.1 Crear `src/app/Components/UI/AppDropdown/` con componente, estilos, tipos publicos e `index.ts`
- [x] 2.2 Exportar `AppDropdown` desde `src/app/Components/UI/index.ts` sin romper los wrappers ya existentes
- [x] 2.3 Implementar la API tipada de `trigger`, `items`, callbacks de seleccion y estados `disabled` desacoplada de tipos crudos de Ant Design
- [x] 2.4 Implementar soporte para metadata visual por item, incluyendo iconografia y acciones destructivas, manteniendo accesibilidad base del trigger y del menu
- [x] 2.5 Implementar soporte consistente para apertura no controlada y controlada segun el contrato del componente

## 3. Pruebas del comportamiento reusable

- [x] 3.1 Crear pruebas con Vitest + Testing Library para validar apertura del dropdown, render de items y ejecucion de callbacks segun el spec `app-dropdown`
- [x] 3.2 Agregar pruebas para estados `disabled`, trigger icon-only con nombre accesible y comportamiento observable del control de apertura
- [x] 3.3 Ejecutar las pruebas relevantes del componente y registrar evidencia util para la validacion del cambio

## 4. Adopcion inicial y verificacion

- [x] 4.1 Integrar `AppDropdown` en un consumidor real o en un punto actual del repo donde hoy se use `Dropdown` directamente, preferiblemente `AppToolbar`
- [x] 4.2 Ajustar pruebas del consumidor para verificar que la adopcion preserva el comportamiento esperado sin exponer tipos del vendor UI
- [x] 4.3 Ejecutar validaciones finales relevantes y dejar el cambio listo para implementacion o verificacion OpenSpec
