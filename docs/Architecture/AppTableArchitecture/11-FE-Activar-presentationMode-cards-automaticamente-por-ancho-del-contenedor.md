# PROMPT ARQUITECTÓNICO
Activar `presentationMode="cards"` automáticamente por ancho del contenedor

## Rol esperado

Arquitecto de software senior y desarrollador frontend React  
(React 19 + TypeScript estricto + arquitectura enterprise)

## Objetivo

Permitir que `AppTable` cambie automáticamente entre:

- `table`
- `cards`

según el ancho disponible del contenedor, sin obligar a cada pantalla a activar manualmente el modo card.

## Problema actual

La infraestructura base de `presentationMode` ya existe, pero hoy:

- `cards` solo se activa si una pantalla lo solicita explícitamente
- no existe activación responsive automática
- al probar varias pantallas no se ve el cambio porque ninguna delega esa decisión al componente

## Decisión arquitectónica

La activación automática no debe depender de:

- nombre de la pantalla
- ruta específica
- módulo fijo

Debe depender de:

- el ancho disponible del contenedor de `AppTable`

Como fallback, puede considerar el ancho del viewport, pero la fuente principal debe ser el contenedor.

## Contrato sugerido

```ts
presentationMode?: "table" | "cards"
responsivePresentation?: {
  enabled?: boolean
  cardsBelow?: number
}
```

## Reglas funcionales

### 1. Prioridad del modo explícito

Si `presentationMode` se informa explícitamente:

- ese valor tiene prioridad total

### 2. Activación automática

Si no existe `presentationMode` explícito y `responsivePresentation.enabled === true`:

- el componente debe calcular el modo automáticamente

### 3. Umbral de cards

Si el ancho disponible del contenedor es menor a `cardsBelow`:

- usar `cards`

Si es mayor o igual:

- usar `table`

## Técnica recomendada

- usar `ResizeObserver` sobre el contenedor de `AppTable`
- evitar depender solo de `window.innerWidth`
- usar viewport solo como fallback si el contenedor no puede medirse

## Ventajas

- reusable para cualquier pantalla
- más preciso que una regla por ruta
- compatible con:
  - pantallas angostas
  - drawers
  - layouts partidos
  - contenedores embebidos

## Alcance

- activar `cards` automáticamente por ancho del contenedor
- mantener override manual
- dejar el comportamiento reusable para cualquier pantalla

## No alcance

- no rediseñar la UI de cards
- no cambiar backend
- no cambiar query state
- no reescribir el renderer tabular

## Riesgos a evitar

- depender solo del viewport
- generar saltos visuales excesivos al cambiar de modo
- romper pantallas que requieren `table` fijo
- quitar el control manual del modo

## Pruebas obligatorias

- `presentationMode="table"` sigue forzando tabla
- `presentationMode="cards"` sigue forzando cards
- con `responsivePresentation.enabled`, el modo cambia por ancho del contenedor
- al cruzar el umbral, el renderer cambia correctamente
- no se rompe paginación ni query state

## Criterios de aceptación

- `AppTable` puede activar `cards` automáticamente
- el cálculo depende del ancho del contenedor
- sigue existiendo override manual
- no se rompe la compatibilidad actual

## Conclusión

La base de `presentationMode` ya quedó implementada.

La siguiente evolución correcta es:

- activar `cards` automáticamente por ancho del contenedor
- no por nombre de pantalla
- no por ruta fija

Eso deja el comportamiento verdaderamente reusable y responsive.
