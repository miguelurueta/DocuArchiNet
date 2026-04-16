## Context

`SCRUMCORE-120` corresponde a la fase 15 FE de `AppEditor`, enfocada en
estabilizar el modo multi-hoja introducido en las fases previas de paginacion
visual.

`AppEditor` ya soporta:
- hoja visual A4 con margenes visibles;
- segmentacion visual automatica por pagina;
- `PageBreak` manual como corte forzado;
- contador `Pagina X de Y`;
- zoom visual;
- imagenes locales y remotas con resize y alineacion horizontal;
- modo controlled/uncontrolled sobre una unica instancia de `ProseMirror`.

El problema de esta fase ya no es agregar capacidades, sino endurecer la
implementacion actual frente a comportamientos inestables:
- saltos o jitter de scroll;
- desalineacion entre cursor, seleccion y layout multi-hoja;
- calculo inestable de pagina actual;
- regressions al combinar zoom, imagenes y `PageBreak`;
- recalculos excesivos y flicker en documentos largos.

La referencia principal para esta fase es
`docs/Architecture/AppEditor/15-FE-AppEditor-hardening-modo-multi-hoja.md`.

## Goals / Non-Goals

**Goals:**
- Estabilizar cursor y seleccion en el modo multi-hoja.
- Garantizar scroll continuo y `scrollIntoView` coherente.
- Mantener contador de pagina estable y sin jitter visible.
- Preservar compatibilidad plena con `PageBreak`, imagenes y zoom.
- Reducir recalculos innecesarios y flicker de la capa visual paginada.
- Mantener intacto el documento persistido y la API publica del componente.

**Non-Goals:**
- No introducir nuevas features funcionales de paginacion.
- No rehacer la arquitectura base de paginas de la fase 14.
- No dividir el documento en multiples editores o subarboles.
- No alterar el HTML persistido ni el modelo de datos del editor.
- No implementar exportacion, impresion o persistencia estructural de paginas.

## Decisions

1. **Endurecer la arquitectura actual en lugar de reemplazarla**
   - **Decision:** La fase 15 debe partir del modelo multi-hoja ya existente y
     corregir sus puntos fragiles en vez de redisenar otra vez la arquitectura.
   - **Rationale:** El ticket es de hardening. Un nuevo redisenio volveria a
     abrir riesgos sobre scroll, seleccion, page context e integraciones ya
     recuperadas en la fase 14.
   - **Alternatives considered:** Replantear toda la segmentacion visual. Se
     descarta porque violaria el alcance del ticket.

2. **Alinear metricas, layout y navegacion sobre una misma fuente**
   - **Decision:** La pagina actual, el scroll y la capa visual deben depender
     de una fuente de metricas coherente, evitando que cada subsistema infiera
     paginas por su cuenta.
   - **Rationale:** La mayor fuente de inestabilidad en modo multi-hoja aparece
     cuando layout, scroll y contador se calculan con offsets distintos.
   - **Alternatives considered:** Mantener logicas separadas para page context y
     segmentacion. Se descarta por propagar jitter y desalineaciones.

3. **Preservar la semantica del documento y del foco**
   - **Decision:** Las correcciones deben mantener una unica instancia editable
     de `ProseMirror` y evitar overlays o wrappers que interfieran con foco,
     seleccion o mapping de posiciones.
   - **Rationale:** El editor enriquecido depende de que cursor y seleccion
     sigan referenciando el mismo flujo continuo de contenido.
   - **Alternatives considered:** Interponer capas visuales interactivas o
     wrappers por pagina. Se descarta por riesgo alto sobre interaccion.

4. **Tratar `PageBreak` como compatibilidad critica**
   - **Decision:** `PageBreak` debe mantenerse como parte central de la prueba
     de robustez del sistema, no como un caso especial secundario.
   - **Rationale:** El nodo manual es uno de los puntos con mas riesgo de romper
     page context, scroll y seleccion al cruzar paginas.
   - **Alternatives considered:** Validarlo solo de forma superficial. Se
     descarta porque ocultaria fallos reales del modo multi-hoja.

5. **Validar zoom e imagenes como casos de primer nivel**
   - **Decision:** Zoom e imagenes deben formar parte explicita del hardening y
     de la evidencia de regresion.
   - **Rationale:** Ambos alteran el layout real y son escenarios donde mas
     facilmente se desincronizan medicion, foco y scroll.
   - **Alternatives considered:** Centrar el hardening solo en texto puro. Se
     descarta por dar una falsa sensacion de estabilidad.

6. **Reducir jitter antes que perseguir micro-optimizaciones prematuras**
   - **Decision:** La prioridad es eliminar recalculos innecesarios, flicker y
     reposicionamientos erraticos antes que optimizar casos marginales.
   - **Rationale:** La experiencia del usuario en un editor se degrada primero
     por inestabilidad perceptible, no por una micro-mejora teorica de costo.
   - **Alternatives considered:** Introducir memoizacion y caches complejos como
     primera respuesta. Se descarta hasta aislar bien los puntos reales de
     inestabilidad.

## Risks / Trade-offs

- [Riesgo] Corregir scroll o page context puede reabrir desalineaciones entre
  layout y seleccion.
  Mitigacion: validar cualquier ajuste contra cursor, scroll y contador juntos.

- [Riesgo] Reducir recalculos puede dejar metricas obsoletas en ciertos
  escenarios de zoom o imagenes.
  Mitigacion: conservar triggers claros para cambios reales de layout.

- [Riesgo] Ajustes de foco o estilos pueden ocultar affordances utiles del
  editor si se eliminan sin criterio.
  Mitigacion: diferenciar bien entre ruido visual y senales necesarias para
  seleccion o accesibilidad.

- [Riesgo] Documentos largos pueden seguir exponiendo casos no visibles en
  pruebas cortas.
  Mitigacion: incluir evidencia con escenarios multipagina prolongados.

## Migration Plan

- Revisar `usePaginationMetrics.ts` para asegurar que las metricas y offsets del
  modo multi-hoja no se retroalimenten ni generen jitter.
- Revisar `usePageContext.ts` para consolidar el calculo de pagina actual con
  prioridad coherente entre scroll y cursor.
- Ajustar `AppEditor.tsx` y `AppEditor.module.css` solo donde sea necesario para
  evitar interferencias entre layout paginado, foco y overlays.
- Validar explicitamente compatibilidad con `PageBreak`, imagenes y zoom.
- Ampliar pruebas focalizadas del editor para cubrir casos de seleccion,
  scroll, contador, imagenes y documentos largos.

## Open Questions

- ¿Conviene derivar la pagina actual prioritariamente desde el cursor cuando el
  editor esta enfocado, o seguir priorizando scroll reciente bajo ciertas
  ventanas temporales?
- ¿Existen affordances visuales del modo multi-hoja que deban mantenerse por
  accesibilidad aunque se reduzca el ruido visual del hardening?
