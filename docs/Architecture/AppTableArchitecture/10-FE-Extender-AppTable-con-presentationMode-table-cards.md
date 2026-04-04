# PROMPT ARQUITECTÓNICO
Extender `AppTable` con `presentationMode: "table" | "cards"`

## Rol esperado

Arquitecto de software senior y desarrollador frontend React  
(React 19 + TypeScript estricto + arquitectura enterprise)

## Objetivo

Permitir que `AppTable` sea compatible con más tipos de pantallas mediante dos modos de presentación reutilizables:

- `table`
- `cards`

La solución debe reutilizar el mismo modelo de datos, query state, paginación, acciones y contrato backend, cambiando únicamente la capa visual.

## Problema actual

Hoy `AppTable` está orientado exclusivamente a AG Grid y a un render tabular clásico.

Eso funciona bien para:

- escritorio
- tablas con muchas columnas
- interacciones tipo grid

Pero se vuelve menos adecuado para:

- pantallas angostas
- layouts responsive
- vistas donde una fila necesita leerse mejor como bloque de información

## Decisión arquitectónica

No se debe intentar “convertir celdas individuales en cards” dentro del grid.

La solución correcta es soportar dos renderizadores del mismo dataset:

- `table`: renderer basado en AG Grid
- `cards`: renderer basado en cards por fila

## Arquitectura propuesta

```txt
AppTableQueryWrapper
  ├─ controls
  ├─ pagination
  └─ AppTable
       ├─ AppTableGridRenderer
       └─ AppTableCardRenderer
```

## Contrato sugerido

```ts
presentationMode?: "table" | "cards"
```

Evolución opcional:

```ts
responsivePresentation?: {
  mobile?: "table" | "cards"
  tablet?: "table" | "cards"
  desktop?: "table" | "cards"
}
```

## Regla funcional clave

- `table`
  - usa AG Grid
- `cards`
  - cada fila se representa como una card
- ambos modos deben compartir:
  - `queryState`
  - `page`
  - `pageSize`
  - `total`
  - `sort`
  - `actions`
  - `backend request`

## Regla de modelado

La unidad visual de `cards` debe ser:

- una fila = una card

No:

- una celda = una card

## Ejemplo conceptual

```txt
┌─────────────────────────────┐
│ Radicado: 2500466700035     │
│ Beneficiario: Karina...     │
│ Trámite: Oficio             │
│ Vence: 2025-06-30           │
│ [Acciones]                  │
└─────────────────────────────┘
```

## Alcance

- definir `presentationMode`
- desacoplar renderer tabular del renderer card
- reutilizar acción dinámica, paginación y query state
- permitir adopción selectiva por pantalla

## No alcance

- no rediseñar backend
- no duplicar hooks de consulta
- no rehacer `AppTable` desde cero
- no resolver en este ticket todas las reglas visuales de cada pantalla

## Reglas de implementación

### 1. Mismo pipeline de datos

La vista `cards` debe consumir exactamente la misma entrada que la vista `table`.

### 2. Reutilización de acciones

Las acciones de fila y menú deben seguir funcionando en ambas vistas.

### 3. Selección controlada

Si una pantalla requiere selección, debe definirse explícitamente cómo se representa en cards.

### 4. Configuración de campos visibles

La vista card no debe mostrar necesariamente todas las columnas.

Debe existir una forma de definir:

- campos principales
- campos secundarios
- orden visual en card

Esto puede resolverse con metadata reusable o mapping por pantalla.

## Riesgos a evitar

- mezclar lógica AG Grid con layout card dentro del mismo renderer
- duplicar query state o request mapper
- perder soporte de acciones dinámicas
- crear una vista card ad hoc por módulo
- intentar renderizar “cards” dentro de celdas del grid

## Pruebas obligatorias

- `presentationMode="table"` preserva comportamiento actual
- `presentationMode="cards"` renderiza una card por fila
- paginación y total funcionan igual en ambos modos
- acciones dinámicas funcionan en cards
- el query state sigue siendo único

## Criterios de aceptación

- `AppTable` soporta `presentationMode`
- la vista `cards` reutiliza el mismo pipeline de datos
- no se rompe la vista tabular actual
- la solución sirve para pantallas responsive o especializadas

## Recomendación de adopción

No migrar todas las pantallas al tiempo.

Primera adopción recomendada:

- una pantalla con necesidad real de vista móvil o card
- validar allí el patrón antes de extenderlo a otros módulos

## Conclusión

Sí, es viable hacer `AppTable` compatible con pantallas tipo card.

La forma correcta es:

- agregar `presentationMode`
- mantener el modelo de datos compartido
- renderizar `row -> card`

No es recomendable intentar forzar este patrón dentro del grid actual.
