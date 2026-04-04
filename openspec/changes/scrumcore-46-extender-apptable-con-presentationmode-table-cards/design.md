## Contexto

`AppTable` hoy renderiza exclusivamente sobre AG Grid. Eso cubre bien los escenarios tabulares clásicos, pero no resuelve de forma reusable pantallas responsive o casos donde una fila debe leerse mejor como bloque de información.

La necesidad no es “convertir celdas en cards dentro del grid”, sino reutilizar el mismo pipeline de datos en dos presentaciones distintas:

- tabla
- cards

## Objetivos

- Permitir dos renderizadores del mismo dataset sin duplicar query state ni request mapper.
- Mantener compatibilidad hacia atrás con la vista tabular actual.
- Hacer posible una adopción progresiva por pantalla o por breakpoint.

## Decisiones

### 1. `AppTable` expone `presentationMode`

Se introduce `presentationMode?: "table" | "cards"` como contrato visual del componente.

- `table` mantiene AG Grid como renderer.
- `cards` renderiza una card por fila.

### 2. El modelo de datos no cambia

La presentación `cards` debe consumir el mismo modelo que hoy ya alimenta al grid:

- `rows`
- `columns`
- `total`
- `queryState`
- acciones dinámicas

No se duplica:

- backend request
- hooks de consulta
- paginación
- sort

### 3. La unidad visual en cards es la fila

Cada fila se convierte en una card. No se deben crear “cards por celda” dentro del grid.

### 4. La configuración de cards debe ser explícita

No todas las columnas del grid deben mostrarse igual en cards. La solución debe contemplar una forma de elegir:

- campos principales
- campos secundarios
- orden visual

Eso puede resolverse con metadata reusable o con mapping por pantalla en una primera fase.

### 5. Primera pantalla candidata

La primera candidata natural para adopción controlada es `GestionCorrespondencia`, porque:

- ya usa la infraestructura reusable de `AppTable`
- ya opera con server pagination
- ya fue el módulo piloto para `QueryWrapper`, `layoutMode` y ajustes de estabilidad visual

La adopción no es obligatoria en este ticket, pero la pantalla queda identificada como primer consumidor real cuando se decida activar `presentationMode="cards"` por breakpoint o por configuración explícita.
