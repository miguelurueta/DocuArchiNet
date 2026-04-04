## 1. Contrato compartido

- [x] 1.1 Agregar `presentationMode?: "table" | "cards"` a `AppTable`.
- [x] 1.2 Mantener compatibilidad hacia atrás con `table` como comportamiento efectivo por defecto.

## 2. Renderer reusable

- [x] 2.1 Extraer o definir el renderer tabular actual como pieza separada.
- [x] 2.2 Crear un renderer card que consuma el mismo dataset.
- [x] 2.3 Definir cómo se configuran campos principales/secundarios en cards.

## 3. Integración funcional

- [x] 3.1 Reutilizar acciones dinámicas en cards.
- [x] 3.2 Garantizar compatibilidad con paginación, total y query state.
- [x] 3.3 Definir la primera pantalla candidata para adopción.

## 4. Validación

- [x] 4.1 Agregar pruebas para `presentationMode="table"`.
- [x] 4.2 Agregar pruebas para `presentationMode="cards"`.
- [x] 4.3 Validar que no se duplica la capa de consulta.
