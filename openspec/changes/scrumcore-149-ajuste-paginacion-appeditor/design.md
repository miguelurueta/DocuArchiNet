# Design: SCRUMCORE-149 ajuste paginacion AppEditor

## Contexto

`AppEditor` ya soporta un modo `paginationMode="visual"` con hojas visibles,
zoom, contador de paginas y page breaks automaticos. Sin embargo, el modelo
actual sigue siendo correctivo: el contenido se renderiza en un flujo continuo,
se mide en DOM y luego se insertan `pageBreak` automaticos para recomponer el
layout. Ese enfoque permite casos donde el contenido llega al borde inferior de
la hoja y solo despues se corrige.

Para un comportamiento tipo Word, ese modelo no es suficiente. El sistema debe
anticipar el corte antes del desborde visible y aplicar reglas coherentes por
tipo de bloque.

## Problema actual

### Arquitectura vigente

- `useAppEditor` ejecuta una rutina de autopaginacion que elimina
  `pageBreak` automaticos, vuelve a medir y reinserta cortes.
- `autoPagination.ts` decide entre:
  - mover un bloque completo antes de la siguiente pagina (`before`)
  - partir un `textblock` (`split`)
- `usePaginationMetrics` calcula paginas visuales sobre el flujo existente.

### Limitaciones detectadas

1. El flujo principal sigue siendo continuo.
2. El corte ocurre despues del render medido, no antes.
3. Solo los `textblock` tienen estrategia de split fino.
4. Los bloques no textuales dependen de mover el bloque completo.
5. El usuario puede percibir correccion tardia en borde inferior.
6. La experiencia no es equivalente a Word aunque visualmente simule hojas.

## Objetivos de diseño

1. Endurecer la paginacion visual para que el borde inferior sea un limite
   funcional real.
2. Definir un motor incremental de layout que opere sobre bloques logicos.
3. Separar claramente:
   - medicion
   - plan de layout
   - aplicacion de page breaks
4. Mantener compatibilidad con TipTap, zoom, page counter y modo continuo.
5. Evitar re-render completo del editor.

## Decision arquitectonica principal

Se propone evolucionar de un modelo correctivo basado en reflujo completo a un
modelo de layout incremental por bloques top-level.

### Nuevo enfoque

El motor de paginacion visual se organiza en tres etapas:

1. **Snapshot estructural**
   - leer nodos top-level del documento TipTap
   - clasificarlos por tipo estructural
   - identificar bloques divisibles y no divisibles

2. **Plan de layout**
   - medir solo los bloques relevantes del flujo actual
   - calcular espacio restante por pagina
   - decidir antes del desborde si el bloque:
     - cabe
     - debe moverse completo
     - debe dividirse

3. **Aplicacion incremental**
   - insertar, ajustar o remover `pageBreak` automaticos solo donde cambie el
     plan
   - preservar seleccion, foco, scroll y node selections de imagenes

## Modelo logico de pagina

Cada pagina visual debe tratarse como una entidad logica con:

- `pageIndex`
- `contentTop`
- `contentBottom`
- `remainingHeight`
- `blocks[]`

### Representacion conceptual

```text
Documento
└─ Bloques top-level
   ├─ paragraph
   ├─ heading
   ├─ bulletList
   ├─ orderedList
   ├─ taskList
   ├─ image
   └─ pageBreak(auto/manual)

Motor de layout
└─ Paginas logicas
   ├─ Pagina 1 -> bloques asignados / cortes
   ├─ Pagina 2 -> bloques asignados / cortes
   └─ Pagina N -> bloques asignados / cortes
```

## Estrategia de corte por tipo de bloque

### 1. TextBlocks

Aplica a:
- `paragraph`
- `heading`

Regla:
- medir lineas/caret positions reales
- encontrar ultimo punto legible antes del limite inferior
- partir solo si se conserva continuidad estable

### 2. Listas

Aplica a:
- `bulletList`
- `orderedList`
- `taskList`

Regla:
- no tratar la lista como texto plano
- preferir corte por `listItem` cuando el bloque completo no cabe
- si el item actual es divisible y la estructura interna lo permite, cortar el
  contenido del item
- si no es seguro, mover el item o subbloque completo

### 3. Imagenes y bloques indivisibles

Aplica a:
- `image`
- cualquier bloque atomico/no textblock no divisible

Regla:
- si no cabe completo en el espacio restante, moverlo a la siguiente pagina
- nunca permitir overflow visible parcial

## Clasificacion estructural requerida

Se necesita una capa de clasificacion de bloques con al menos estas categorias:

- `text-divisible`
- `list-structured`
- `atomic-indivisible`
- `manual-break`

Esto permite que el motor de layout no dependa solo de `node.isTextblock`.

## Integracion con TipTap / ProseMirror

### Requisitos

- no romper transactions existentes
- mantener `pageBreak` como representacion de corte aplicado
- no duplicar el estado del documento en una fuente paralela de verdad
- usar DOM measurement como soporte, no como unica logica de control

### Requisito de consistencia

El estado fuente sigue siendo el documento TipTap.

El motor de layout:
- observa
- planifica
- aplica cortes

Pero no redefine el modelo del documento por fuera de ProseMirror.

## Estrategia de medicion

### No permitido

- medir todo el documento completo en cada keystroke
- invalidar toda la paginacion en cada cambio minimo

### Propuesta

- detectar rango afectado por la transaccion actual
- medir el bloque editado y un vecindario corto
- recalcular desde la pagina impactada hacia adelante hasta estabilizar layout
- reutilizar el resultado previo para arrancar desde el primer bloque top-level
  invalidado, en vez de reescanear siempre desde el inicio del documento
- mantener la medicion global completa solo para casos de resize, zoom,
  rehidratacion de imagenes o cambios que invaliden la geometria total

### Beneficio

Reduce:
- costo de medicion
- parpadeo
- recomputacion innecesaria

## Impacto esperado en typing y estrategia de cache

### Impacto esperado

- escritura normal dentro de un bloque ya estabilizado:
  - la repaginacion preventiva arranca desde el bloque afectado
  - se evita el reescaneo completo del documento en la mayoria de keystrokes
- escritura cerca del final de hoja:
  - puede requerir varias iteraciones locales hasta estabilizar cortes
  - aun asi el trabajo se limita al tramo invalido hacia adelante
- resize, zoom o carga tardia de imagenes:
  - siguen siendo escenarios de invalidacion amplia porque cambian la geometria
    base de varias paginas

### Estrategia de cache / invalidez

- cache implicita:
  - el documento TipTap actual y los `pageBreak` automaticos vigentes actuan
    como base reutilizable del layout ya estabilizado
- punto de invalidez:
  - cada transaccion con `docChanged` resuelve el bloque top-level afectado y
    registra un `dirtyStartChildIndex`
- alcance de recomputacion:
  - el planificador vuelve a medir desde `dirtyStartChildIndex` hacia adelante
  - los bloques previos se consideran estables mientras no cambie la geometria
    global
- caida controlada a recomputacion amplia:
  - si hay resize, zoom, cambios estructurales mayores o imagenes pendientes,
    el sistema reinicia desde el principio para evitar inconsistencia visual

## Flujo de ejecucion propuesto

```text
edicion del usuario
→ identificar bloque afectado
→ medir bloque y contexto cercano
→ calcular capacidad restante de la pagina actual
→ decidir:
   - cabe
   - mover completo
   - dividir estructuralmente
→ aplicar pageBreaks minimos necesarios
→ sincronizar spacer heights / overlays visuales
→ preservar cursor, foco y scroll
```

## Cambios esperados por modulo

### `application/autoPagination.ts`

Debe evolucionar de:
- detector de overflow + accion correctiva

A:
- planificador de layout por bloque con estrategia estructural

Responsabilidades nuevas:
- clasificar nodos top-level
- calcular acciones preventivas
- definir estrategia por listas e imagenes

### `application/useAppEditor.ts`

Debe evolucionar de:
- loop correctivo de repaginacion

A:
- coordinador incremental de layout

Responsabilidades nuevas:
- invalidacion parcial
- scheduling mas fino
- preservacion estable de seleccion durante layout

### `application/usePaginationMetrics.ts`

Debe mantenerse como capa de metricas visuales, pero alineada al nuevo motor:
- menos dependencia de “flujo largo corregido despues”
- mayor dependencia del layout ya planificado

## Riesgos

1. Aumentar complejidad de coordinacion entre layout y estado del editor.
2. Introducir regresiones en seleccion de imagenes o scroll.
3. Generar cortes inconsistentes en listas anidadas.
4. Impactar rendimiento si la medicion incremental no queda bien limitada.

## Mitigaciones

- introducir clasificacion estructural antes de tocar estrategia de corte
- cubrir listas, imagenes y texto con pruebas dedicadas
- mantener modo continuo fuera de este motor
- validar zoom y page counter con documentos multipagina reales

## Alternativas consideradas

### A. Ajustar solo CSS

Descartada.

No resuelve el problema de fondo y solo maquilla el desborde.

### B. Mantener motor actual con mas thresholds

Descartada.

Seguiria siendo correctivo y dependiente del desborde previo.

### C. Reemplazar el editor por un motor de paginacion externo

Descartada por alcance actual.

Romperia demasiado la integracion existente con TipTap.

### D. Evolucion incremental del motor actual

Seleccionada.

Permite conservar TipTap y endurecer la logica hacia un comportamiento tipo
Word sin rehacer todo el editor.

## Fases de implementacion propuestas

### Fase 1
- clasificacion estructural de bloques top-level
- acciones preventivas para bloques indivisibles

### Fase 2
- corte estructural de listas y task lists
- refinamiento de split real por item o subbloque

### Fase 3
- invalidacion incremental por rango afectado
- optimizacion de scheduling y medicion

### Fase 4
- pruebas de no regresion visual y funcional
- validacion fuerte de edge cases en borde inferior

## Criterio de exito

La solucion se considera aceptable solo si:

- no existe overflow visible en borde inferior
- el corte ocurre antes del desborde
- listas e imagenes respetan la hoja
- el usuario no percibe correccion tardia
- la experiencia se siente consistente y estable en modo visual

## Honestidad tecnica

Con la arquitectura actual, el editor todavia no es equivalente a Word.

La ruta viable no es maquillar el problema, sino endurecer el motor actual
hacia un modelo incremental preventivo. Si durante implementacion se detecta que
las listas complejas o ciertos node views no pueden alcanzar el mismo nivel de
fidelidad sin mayor refactor, eso debera declararse explicitamente en lugar de
presentar una aproximacion como equivalencia total.
