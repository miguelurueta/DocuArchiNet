## Context

Hoy `AppEditor` ya tiene piezas parciales para paginas reales, pero no son la
base del modo paginado:

- `AppEditor.tsx` sigue renderizando `paginationMode="visual"` como un
  `contentFlow` continuo sobre `pageShells` visuales.
- `useAppEditor.ts` solo activa `PageDocument` + `PageNode` cuando el valor de
  entrada ya viene con `data-app-editor-page="true"` o con `pageBreak`
  manuales.
- La repaginacion principal sigue dependiendo de `autoPagination.ts` y
  `autoPageBreak.ts`, que insertan `pageBreak` automaticos, calculan
  `spacerHeight` y corrigen el flujo despues de medir el DOM.
- `usePaginationMetrics.ts` aun mantiene una ruta heuristica basada en
  `scrollHeight`, offsets y `pageBreaks`, aunque ya puede leer wrappers reales
  cuando existen.

Eso significa que el proyecto ya posee un esquema `doc -> page`, pero hoy se
usa como compatibilidad de rehidratacion, no como motor principal del editor.

`SCRUMCORE-180` debe convertir esa capacidad parcial en la arquitectura base de
`paginationMode="visual"` sin romper toolbar, serializacion, zoom, imagenes
locales ni modo continuo.

## Goals / Non-Goals

**Goals:**
- Hacer que `paginationMode="visual"` use siempre un documento real
  `doc -> page -> blocks`.
- Reemplazar la dependencia estructural de `pageBreak` automaticos,
  `spacerHeight` y shells visuales como fuente de verdad.
- Mantener una ruta estable de migracion desde HTML plano o HTML con
  `pageBreak` manual.
- Garantizar continuidad basica hacia la pagina siguiente en esta fase.
- Preservar el contrato reusable actual de `AppEditor` y sus extensiones
  existentes.

**Non-Goals:**
- No resolver en esta fase reflow fino intraparrafo, recomposicion inversa
  completa, viudas/huerfanas ni hardening editorial avanzado.
- No introducir dos motores paginados activos al mismo tiempo.
- No cambiar el contrato publico de `value` / `onChange` mas alla de la
  normalizacion necesaria.
- No imponer paginas reales sobre `paginationMode="none"`.

## Decisions

### 1. `paginationMode="visual"` activara siempre el schema paginado real

La decision principal es dejar de tratar `PageDocument` y `PageNode` como una
via opcional de rehidratacion. En modo visual, el editor siempre se inicializara
con schema paginado real.

Implicaciones:
- `useAppEditor.ts` dejara de decidir `paginatedDocument` a partir de si el
  HTML entrante trae wrappers o `pageBreaks`.
- El contenido externo plano se migrara a una sola pagina real inicial.
- El contenido externo con `pageBreak` manual se convertira a multiples nodos
  `page`.
- El contenido ya envuelto con `data-app-editor-page="true"` seguira siendo una
  entrada valida y estable.

Esto convierte a `doc -> page -> blocks` en el modelo canonico en memoria para
todo el modo visual.

### 2. Las paginas reales seran la fuente de verdad para render y layout

El modo visual dejara de depender de una columna continua superpuesta a hojas
decorativas. La hoja real sera el propio nodo `page` renderizado por
ProseMirror.

Implicaciones:
- `AppEditor.tsx` debera dejar de tratar `pageShell` / `contentFlow` como la
  estructura primaria del layout paginado.
- Los estilos de `AppEditor.module.css` pasaran a apoyar la visualizacion de
  `.ProseMirror > [data-app-editor-page="true"]` como hojas reales, con su gap
  y margenes utiles.
- La representacion visual y la estructura del documento dejaran de divergir.

Esto elimina la situacion actual donde el usuario ve hojas, pero el contenido
real sigue existiendo como un flujo continuo corregido despues.

### 3. `pageBreak` automatico y `spacerHeight` salen del camino principal

`pageBreak` seguira existiendo solo como mecanismo de compatibilidad para
persistencia o migracion, no como base del motor paginado.

Implicaciones:
- La rama principal de `useAppEditor.ts` para modo visual no debera ejecutar la
  repaginacion basada en `removeAutoPageBreaks`, `resolveAutoPageBreakActions`,
  `syncAutoPageBreakSpacerHeights` ni helpers equivalentes.
- `autoPagination.ts` y `autoPageBreak.ts` dejaran de gobernar el layout base
  del modo visual.
- `PageBreak` manual podra mantenerse como representacion externa de frontera
  entre paginas al serializar, si eso evita romper contratos consumidores.

Esto permite retirar el acoplamiento actual entre layout, mutacion del
documento y metadata transitoria.

### 4. La continuidad basica de Fase 1 se resuelve a nivel de pagina y bloque

Fase 1 no va a resolver todavia el reflow fino de cualquier posicion textual
interna. La estrategia base sera trabajar con redistribucion entre paginas
reales desde el final del contenido afectado.

Reglas base:
- Si un bloque indivisible no cabe, se mueve completo a la siguiente pagina.
- Si el usuario escribe al final de la ultima linea disponible y la continuidad
  natural crea un nuevo bloque, ese bloque nace en la siguiente pagina.
- Si un pegado agrega varios bloques, los excedentes se redistribuyen a paginas
  siguientes.
- El split fino de un mismo parrafo en mitad de su contenido queda explicitado
  como endurecimiento de Fase 2.

Esto es coherente con el objetivo del ticket: introducir la base estructural
real y una continuidad funcional minima sin prometer todavia recomposicion
editorial completa.

### 5. La migracion y serializacion conservaran compatibilidad hacia afuera

El modelo interno sera `doc -> page -> blocks`, pero la interfaz externa puede
seguir usando HTML limpio con `pageBreak` manual como frontera persistible si
eso protege a los consumidores actuales.

Implicaciones:
- `pageDocument.ts` se convierte en la capa canonica de migracion entre HTML
  externo y documento paginado interno.
- `wrapHtmlInVisualPages` deja de ser solo un helper para casos especiales y
  pasa a ser la ruta base del modo visual.
- `serializeVisualPageHtml` seguira eliminando wrappers internos del editor y
  emitiendo una representacion persistible estable.
- `normalizeEditorHtml.ts` seguira limpiando `data-page-break-auto`,
  `spacerHeight` y metadata transitoria legacy.

La ventaja es que la arquitectura interna cambia fuerte sin obligar a cambiar
de inmediato el contrato persistido del resto de la aplicacion.

### 6. Metricas, contador y contexto de pagina se derivan de paginas reales

`usePaginationMetrics.ts` ya tiene una rama para wrappers reales. Esa rama pasa
a ser la primaria para `paginationMode="visual"`.

Implicaciones:
- `totalPages` se derivara primero de la cantidad real de nodos `page`.
- `visualPageBoundaries` se construira desde la secuencia real de hojas, no
  desde heuristicas sobre `scrollHeight` y `pageBreaks` auto-insertados.
- `usePageContext` y el contador de pagina quedan desacoplados del motor viejo
  y alineados con el arbol del documento.

Esto reduce jitter y elimina el doble calculo actual entre estructura real,
layout sintetico y metadata de repaginacion.

### 7. Se mantiene un solo editor TipTap/ProseMirror

No se abrira una arquitectura de un editor por pagina. Se conserva una sola
instancia de TipTap/ProseMirror con nodos `page`.

Ventajas:
- toolbar, seleccion, undo/redo, links, imagenes y marks siguen operando sobre
  una sola fuente de verdad.
- el costo de integracion sobre `AppEditorToolbar`, `TiptapEditorContent` y las
  extensiones existentes es mucho menor.
- se evita duplicar foco, historial y sincronizacion entre multiples editores.

La complejidad se concentra en la normalizacion del documento y en la
redistribucion de bloques entre paginas, no en la orquestacion de varias
instancias.

## Risks / Trade-offs

- **[Riesgo] Fase 1 no resuelve todos los casos de split textual fino.**
  -> Mitigacion: dejar documentado que typing al borde, pegado por bloques y
  continuidad basica quedan cubiertos primero; recomposicion fina va a Fase 2.

- **[Riesgo] Seleccion y cursor pueden sufrir regresiones al mover bloques entre paginas.**
  -> Mitigacion: preservar una sola instancia de editor y ajustar tests sobre
  caret, scroll anchor y continuidad al final de pagina.

- **[Riesgo] El CSS actual del modo visual esta acoplado a `pageShell` y `contentFlow`.**
  -> Mitigacion: migrar esos estilos hacia wrappers reales del ProseMirror en
  vez de sumar otra capa visual.

- **[Trade-off] Se conserva `pageBreak` manual como contrato externo temporal.**
  -> Se acepta porque reduce riesgo en guardado/rehidratacion mientras el motor
  interno cambia a paginas reales.

- **[Trade-off] Parte del codigo legacy puede convivir transitoriamente en el repo pero no en la ruta principal.**
  -> Se acepta solo como etapa de migracion tecnica; no debe seguir activo para
  `paginationMode="visual"` una vez completada esta fase.

## Migration Plan

1. Activar `paginatedDocument` siempre que `paginationMode="visual"` este
   activo.
2. Normalizar entradas planas, con `pageBreak` manual o con wrappers reales al
   mismo arbol `doc -> page -> blocks`.
3. Cambiar el render del modo visual para que las hojas reales sean los nodos
   `page` del propio editor.
4. Sacar `autoPagination.ts` y `autoPageBreak.ts` del flujo principal del modo
   visual.
5. Promover en `usePaginationMetrics.ts` la lectura basada en wrappers reales
   como fuente primaria de `totalPages` y `visualPageBoundaries`.
6. Ajustar pruebas para cubrir migracion, continuidad basica, serializacion
   limpia y estabilidad de contador/cursor.

Rollback:
- Si la redistribucion basica entre paginas no alcanza estabilidad minima, se
  puede frenar el cambio antes de retirar completamente la ruta vieja, pero sin
  dejar ambos motores activos sobre el mismo flujo en produccion.

## Open Questions

- Si la persistencia externa de esta fase debe seguir usando `pageBreak`
  manuales o si ya es viable persistir `data-app-editor-page="true"` en toda la
  aplicacion.
- Si la continuidad basica de Fase 1 para typing al final de pagina se
  implementara solo por bloques o tambien con un split controlado del ultimo
  parrafo en el caso mas simple.
- Cuanto codigo legacy conviene dejar encapsulado como compatibilidad de
  migracion antes de su retiro definitivo en la siguiente fase.
