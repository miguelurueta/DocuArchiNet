## Context

Actualmente `AppEditor` representa hojas en `paginationMode="visual"` a partir de
un flujo continuo de contenido editable y corrige el layout mediante
`pageBreak` automaticos, `spacerHeight` y medicion DOM reactiva. Ese enfoque
permite aproximar una experiencia paginada, pero no garantiza hojas reales ni
respeto estructural del area util de cada pagina.

El problema funcional reportado en `SCRUMCORE-179` es que el contenido puede
llegar al borde de la hoja, superponer el limite inferior o depender de una
correccion posterior. Eso rompe la expectativa de comportamiento tipo Word y
afecta especialmente:

- escritura al final de pagina
- edicion de parrafos ya partidos
- paste largo
- bloques indivisibles como imagenes

La correccion debe reemplazar la base del sistema paginado, no maquillarla con
CSS o con mas heuristicas sobre el motor actual.

## Goals / Non-Goals

**Goals:**
- Migrar `AppEditor` a una arquitectura de paginas reales como base del modo
  paginado.
- Eliminar la dependencia estructural del sistema actual de `pageBreak`
  automaticos y `spacerHeight`.
- Definir un modelo de reflow real para texto y bloques entre paginas.
- Mantener compatibilidad con toolbar, zoom, links, imagenes locales, modo
  continuo, modo controlado y serializacion HTML.
- Permitir evolucionar el editor por fases sin convivir con dos motores
  paginados competidores.

**Non-Goals:**
- No buscar equivalencia total con Microsoft Word en una sola entrega.
- No implementar reglas editoriales avanzadas como viudas/huérfanas,
  keep-with-next o tablas complejas.
- No rediseñar toda la toolbar ni las capacidades de formato no relacionadas
  con el layout paginado.
- No imponer paginas reales sobre `paginationMode="none"`.

## Decisions

### 1. Reemplazar la base paginada por paginas reales dentro del modelo del editor

El modo paginado dejara de depender de un flujo continuo con separadores
artificiales y pasara a trabajar con una estructura de paginas reales. La
decision base es introducir un nodo `page` real en el schema del editor para
que el documento paginado quede modelado como una secuencia de hojas y no como
una sola columna editable.

Esto permite:
- delimitar el area util de cada hoja de forma estructural
- separar correctamente contenido por pagina
- reflow real entre hojas
- retirar `pageBreak + spacer` como mecanismo base

Alternativas descartadas:
- seguir parcheando el sistema actual de `pageBreak` automaticos: descartado
  porque la base sigue siendo una hoja falsa con correcciones posteriores
- usar solo shells visuales con flujo continuo por debajo: descartado porque no
  cumple el requisito de hojas reales

### 2. Mantener un solo editor con paginas reales en el schema

La implementacion se apoyara en un solo editor TipTap/ProseMirror con nodos
`page`, en lugar de instancias separadas por hoja. Eso mantiene una sola fuente
de verdad para:

- seleccion
- historial
- comandos de toolbar
- serializacion
- integracion con extensiones existentes

Alternativas descartadas:
- un editor independiente por pagina: descartado por complejidad en copy/paste,
  undo/redo, seleccion y normalizacion global

### 3. Reflow incremental desde el bloque afectado hacia adelante

El rendimiento y la estabilidad dependen de no recomputar todo el documento en
cada edicion. La estrategia elegida es recalcular solo desde el bloque afectado
hacia adelante, estabilizando el reflow cuando las paginas posteriores dejan de
cambiar.

Esto cubre los escenarios clave:
- escribir al final de una hoja
- hacer crecer un parrafo ya partido
- borrar y traer contenido desde la hoja siguiente
- paste multipagina

Alternativas descartadas:
- repaginar todo el documento en cada tecla: descartado por costo, jitter y
  riesgo de cursor inestable

### 4. Distinguir bloques divisibles de bloques indivisibles

El sistema tratara:
- parrafos y texto enriquecido como bloques divisibles
- imagenes y otros bloques atomicos como bloques indivisibles

Las reglas base seran:
- un bloque indivisible que no cabe pasa completo a la siguiente hoja
- un parrafo puede partirse por posicion real de texto
- el reflow preserva continuidad logica del mismo parrafo entre paginas

Esto reduce corrupcion de contenido y mantiene integridad visual.

### 5. Preservar cursor y seleccion por posicion logica, no por posicion visual

Al crecer o reducir contenido, el punto editado puede cambiar de pagina. La
seleccion no debe reconstruirse por coordenadas visuales antiguas, sino por
posiciones logicas del documento despues del reflow.

Esta decision es critica para:
- evitar saltos del cursor
- mantener typing estable
- soportar parrafos partidos y links inline

### 6. Retirar la arquitectura vieja una vez que la nueva base quede activa

No deben convivir dos motores paginados. Cuando el nuevo flujo de paginas
reales quede operativo, la logica vieja basada en `autoPagination.ts`,
`autoPageBreak.ts`, `pageBreak` automaticos y `spacerHeight` debe eliminarse o
quedar completamente fuera del camino principal del modo paginado.

## Risks / Trade-offs

- **[Riesgo] Complejidad alta en el cambio de schema.**
  -> Mitigacion: ejecutar por fases, empezando por base de paginas reales y
  luego reflow/hardening.

- **[Riesgo] Regresiones en cursor y seleccion.**
  -> Mitigacion: separar una fase explicita para mapping de seleccion, links y
  formato inline.

- **[Riesgo] Degradacion de rendimiento en documentos largos.**
  -> Mitigacion: reflow incremental desde bloque afectado, corte temprano cuando
  la distribucion se estabiliza y pruebas con documentos multipagina reales.

- **[Riesgo] Compatibilidad con HTML ya guardado.**
  -> Mitigacion: definir una ruta de migracion desde contenido serializado con
  metadata vieja a la nueva estructura de paginas.

- **[Trade-off] Mayor complejidad interna a cambio de una experiencia mucho mas
  estable.**
  -> Se acepta porque el modelo actual ya no satisface el requerimiento de
  hojas reales y margen util estricta.

## Migration Plan

1. Introducir el nodo `page` y adaptar el schema paginado.
2. Definir la capa de render de hojas reales y area util por pagina.
3. Incorporar reflow incremental para texto y bloques.
4. Ajustar seleccion/cursor y marks inline.
5. Hardening de paste, listas basicas, imagenes y rendimiento.
6. Retirar la logica vieja del camino principal del modo paginado.

Rollback:
- volver temporalmente al motor actual de `paginationMode="visual"` si una fase
  intermedia no alcanza estabilidad minima
- evitar rollback parcial donde convivan ambos motores sobre el mismo flujo

## Open Questions

- Como se representara la migracion exacta del HTML actual hacia la estructura
  `doc -> page -> blocks` sin introducir deuda en la serializacion persistida.
- Si el nodo `page` se persistira tal cual en el HTML del editor o si se
  convertira a una forma de persistencia mas limpia durante guardado.
- Si las listas basicas se partiran solo por item o tambien por contenido de
  item en la fase inicial de hardening.
