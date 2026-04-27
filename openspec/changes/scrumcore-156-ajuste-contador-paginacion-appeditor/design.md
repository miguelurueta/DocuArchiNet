# Design: SCRUMCORE-156 ajuste contador paginacion AppEditor

## Contexto

`AppEditor` ya expone un contador de pagina cuando `paginationMode="visual"`,
pero hoy ese contador depende del mismo circuito reactivo que sostiene la
repaginacion visual estricta:

- `useAppEditor` muta el flujo visual durante autopaginacion
- `usePaginationMetrics` vuelve a medir DOM y recalcula paginas
- `usePageContext` deriva la pagina actual desde `scrollTop`, `offsetTop`,
  boundaries visuales y `zoomLevel`

Ese acoplamiento hace que el contador sea correcto en escenarios simples, pero
fragil cuando:

- el scroll cambia mientras se esta escribiendo
- la repaginacion dispara nuevas mediciones
- el zoom altera la escala visual
- el contenido cruza el final de una hoja

El ticket `SCRUMCORE-156` no busca rehacer toda la paginacion visual. Su foco
es estabilizar el calculo y presentacion del contador de pagina para que deje
de amplificar bugs de scroll y deje de depender de sincronizaciones frágiles.

## Problema actual

### Acoplamiento excesivo

El contador actual depende de varias capas simultaneas:

1. metricas visuales recalculadas en `usePaginationMetrics`
2. eventos de repaginacion emitidos por `useAppEditor`
3. scroll del `canvas`
4. `sheet.offsetTop`
5. `zoomLevel`

Cuando cualquiera de estas capas cambia de orden o con timing distinto, el
contador puede:

- cambiar tarde
- cambiar antes de tiempo
- “rebotar” entre dos paginas
- quedar correcto solo despues de una correccion adicional

### Mezcla de responsabilidades

Hoy el contador no es solo una capa de lectura UX. En la practica termina
siguiendo demasiado de cerca el motor de layout. Eso hace mas dificil razonar
si un bug pertenece a:

- scroll
- zoom
- boundaries visuales
- page context
- o repaginacion correctiva

## Objetivos de diseño

1. Separar el contador de pagina del mecanismo de repaginacion destructiva.
2. Mantener hojas visibles y experiencia paginada.
3. Derivar la pagina actual desde una fuente de verdad mas estable.
4. Mantener compatibilidad con zoom visual.
5. Evitar regresiones en modo continuo.
6. Preservar el contrato reusable de `AppEditor`.

## Decision arquitectonica principal

Se propone convertir el contador en una capa de lectura estable del layout
visual, no en una extension reactiva del motor de repaginacion.

### Principio

La pagina actual debe resolverse a partir de un modelo simple y coherente:

- geometria visible del `canvas`
- geometria visible del `sheet`
- stride de pagina ya calculado
- boundaries ya normalizados para la escala efectiva

El contador no debe depender de:

- remapeo de seleccion
- restauracion de cursor
- `pageBreak` automaticos como evento principal
- efectos secundarios del ciclo correctivo del editor

## Modelo propuesto

### 1. Fuente de verdad del contador

El contador debe basarse en una sola lectura estable:

- offset visible dentro del `canvas`
- boundaries visuales derivados del layout actual

Representacion conceptual:

```text
canvas scrollTop
  -> offset interno visible del sheet
  -> boundaries visuales normalizados
  -> page index calculado
  -> contador UI
```

### 2. Separacion entre layout y page context

`usePaginationMetrics` sigue calculando:

- `totalPages`
- `visualPageBoundaries`
- `pageStride`

`usePageContext` debe limitarse a:

- leer scroll
- transformar offset visible a pagina actual
- publicar `currentPage`

La clave es que `usePageContext` no intente “perseguir” cada detalle del motor
de repaginacion, sino solo reaccionar cuando el layout visible ya esta
estabilizado.

### 3. Estabilidad frente a zoom

El zoom debe tratarse como parte del modelo geometrico del contador, no como un
caso especial distribuido en varios lugares.

Regla:

- toda comparacion entre `offset` y `pageBoundaries` debe usar una escala unica
  y explicita
- el contador no debe aplicar compensaciones implícitas distintas segun el
  origen del evento

## Estrategia tecnica

### A. Normalizacion geometrica

El calculo de pagina actual debe normalizar:

- `canvas.scrollTop`
- `sheet.offsetTop`
- `zoomLevel`
- `pageBoundaries`

Objetivo:

- tener una sola interpretacion de “donde empieza una hoja”
- evitar que el contador mezcle offsets sin normalizar

### B. Scheduling simple y predecible

El contador debe reaccionar a dos fuentes principales:

1. `scroll`
2. `app-editor-pagination-updated`

Pero con una regla clara:

- `scroll` actualiza en frame
- `pagination-updated` actualiza de forma inmediata

Sin agregar mas fuentes de sincronizacion salvo que exista una justificacion
tecnica clara.

### C. UI desacoplada y discreta

El contador debe mantenerse como una pieza UX compacta:

- visible solo en `paginationMode="visual"`
- discreta
- sin bloquear interaccion
- compatible con toolbar y zoom

No debe convertirse en un control activo ni en un overlay invasivo.

## Cambios esperados por modulo

### `application/usePageContext.ts`

Debe concentrar la mayor parte del ajuste.

Responsabilidades:

- resolver pagina actual de forma geometrica y estable
- normalizar el offset visible
- mantener un scheduling predecible
- reducir rebotes y cambios espurios

### `application/usePaginationMetrics.ts`

Debe conservar su rol de metricas visuales, pero idealmente:

- exponer boundaries mas coherentes para consumo del contador
- evitar trabajo innecesario que solo exista para el page context

### `presentation/AppEditor.tsx`

Debe conservar:

- render condicional del contador en modo visual
- integracion con `zoom`
- integracion con `totalPages`

Pero sin sumar mas acoplamiento entre toolbar, canvas y page context.

## Alternativas consideradas

### A. Quitar el contador

Descartada.

El usuario necesita referencia clara de pagina mientras existan hojas visibles.

### B. Mover el contador a la toolbar sin tocar logica

Descartada.

Solo cambia la ubicacion visual y no resuelve la inestabilidad del calculo.

### C. Rehacer toda la paginacion visual dentro de este ticket

Descartada por alcance.

Ese trabajo es mayor y merece un cambio arquitectonico distinto. Aqui el foco
es estabilizar el contador actual, no reemplazar el motor completo.

### D. Separar el contador del motor de repaginacion

Seleccionada.

Es el mejor equilibrio entre:

- estabilidad
- alcance del ticket
- riesgo de regresion
- valor UX inmediato

## Riesgos

1. Que el contador siga heredando inconsistencias si las boundaries visuales
   llegan ya desfasadas desde la capa de metricas.
2. Que el zoom siga introduciendo errores si el escalado no se normaliza de
   forma unica.
3. Que el modo continuo herede listeners o calculos innecesarios.

## Mitigaciones

- mantener el ajuste concentrado en `usePageContext`
- no mezclar calculo de pagina con seleccion o cursor
- cubrir scroll, zoom y notificaciones de layout con pruebas focalizadas
- validar que `paginationMode="none"` quede libre de logica paginada

## Criterio de exito

La solucion se considera aceptable solo si:

- el contador deja de rebotar o cambiar erraticamente durante scroll y typing
- la pagina actual coincide con la hoja visible activa
- zoom y contador siguen alineados
- el modo continuo no presenta regresiones
- el contador sigue siendo compacto, legible y no invasivo

## Honestidad tecnica

Este ticket no resuelve por si solo todos los problemas del motor de paginacion
visual de `AppEditor`.

Si durante la implementacion se confirma que parte de la inestabilidad del
contador proviene del diseño global de repaginacion, eso debe declararse de
forma explicita. Aun asi, este cambio debe dejar el contador claramente mejor
aislado, mas estable y mas facil de mantener que en el estado actual.
