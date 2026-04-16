## Context

`SCRUMCORE-122` corresponde a la fase 16 FE de `AppEditor`, enfocada en
exponer un control UI de zoom visual sobre el modo paginado ya existente.

`AppEditor` ya soporta:
- hoja visual A4 con margenes visibles;
- segmentacion visual automatica por pagina;
- contador `Pagina X de Y`;
- `PageBreak` manual como corte forzado;
- imagenes locales y remotas con resize y alineacion horizontal;
- hardening reciente de scroll, seleccion y page context;
- una unica instancia de `ProseMirror` sobre un flujo continuo de contenido.

El problema actual no es la ausencia total de compatibilidad con zoom, sino la
falta de una interfaz publica y estable para controlarlo desde la UI del
editor. Esa nueva exposicion no puede degradar la arquitectura multi-hoja ya
endurecida en las fases 14 y 15.

La referencia principal para esta fase es
`docs/Architecture/AppEditor/16-FE-AppEditor-zoom-visual-ui.md`.

## Goals / Non-Goals

**Goals:**
- Exponer un control UI de zoom solo en `paginationMode="visual"`.
- Introducir props de zoom sin breaking changes para modo controlado y no controlado.
- Mantener el zoom como comportamiento puramente visual.
- Preservar coherencia entre zoom, paginacion visual, scroll, overlays y page context.
- Mantener compatibilidad plena con `PageBreak`, imagenes, seleccion y HTML persistido.
- Evitar flicker o desalineaciones perceptibles al cambiar zoom.

**Non-Goals:**
- No rehacer la arquitectura multi-hoja introducida en fases previas.
- No cambiar el modelo continuo del documento ni dividir el editor.
- No persistir el zoom dentro del HTML serializado.
- No modificar atributos existentes como `data-width`, `data-align` o `data-page-break`.
- No mezclar este cambio con exportacion, impresion ni nuevas features ajenas al zoom UI.

## Decisions

1. **Exponer zoom como estado del componente, no como atributo del documento**
   - **Decision:** El zoom vivira en la API y estado de `AppEditor`, con soporte
     controlado y no controlado, y no se reflejara en el contenido persistido.
   - **Rationale:** El ticket exige zoom puramente visual. Llevarlo al documento
     rompería separacion entre presentacion y persistencia.
   - **Alternatives considered:** Persistir zoom en HTML o en nodos del editor.
     Se descarta por introducir semantica que no pertenece al documento.

2. **Aplicar zoom sobre la capa paginada como presentacion derivada**
   - **Decision:** La capa visual del modo paginado debe consumir `zoomLevel`
     como parte de su layout derivado, manteniendo alineadas `sheet`,
     `pageStack`, `contentFlow` y overlays.
   - **Rationale:** El usuario percibe el zoom sobre la hoja completa, no solo
     sobre el contenido tipografico. El escalado debe abarcar la experiencia
     paginada completa.
   - **Alternatives considered:** Escalar solo `.ProseMirror` o solo la capa de
     fondo. Se descarta porque produciria desalineacion entre contenido y hojas.

3. **No asumir `transform: scale(...)` como solucion aislada**
   - **Decision:** Si el layout usa `transform`, debe hacerlo integrado con las
     metricas visuales y con el `canvas`, no como una transformacion cosmetica
     desconectada del scroll y del contexto de pagina.
   - **Rationale:** Las fases 14 y 15 ya mostraron que offsets y scroll pueden
     volverse inconsistentes cuando la presentacion visual y las metricas reales
     divergen.
   - **Alternatives considered:** Aplicar solo `transform: scale(...)` sobre la
     hoja sin compensacion adicional. Se descarta por alto riesgo de jitter,
     contador incorrecto y desalineacion de overlays.

4. **Tratar zoom como input de metricas y page context**
   - **Decision:** `usePaginationMetrics` y `usePageContext` deben recibir o
     derivar el `zoomLevel` como parte del calculo relevante del modo visual.
   - **Rationale:** El zoom altera la percepcion y geometria del layout paginado.
     Si las metricas lo ignoran, el contador y el scroll dejaran de representar
     la hoja activa correctamente.
   - **Alternatives considered:** Mantener las metricas existentes sin conocer
     el zoom. Se descarta por incoherencia entre UI y page context.

5. **Mantener el control de zoom fuera de la toolbar principal**
   - **Decision:** El control de zoom debe integrarse en la capa de presentacion
     del editor sin recargar la toolbar principal de formato.
   - **Rationale:** La toolbar ya fue simplificada y endurecida en fases
     previas. El zoom pertenece mas al contexto de vista del documento que a
     las acciones semanticas de edicion.
   - **Alternatives considered:** Inyectar el zoom dentro de la toolbar de
     formato. Se descarta para evitar saturacion visual y acoplamiento
     innecesario.

6. **Preservar la unica instancia editable y el flujo continuo**
   - **Decision:** El zoom no debe introducir wrappers interactivos ni capas que
     interfieran con foco, seleccion o mapping de posiciones de ProseMirror.
   - **Rationale:** El editor ya fue endurecido para mantener estabilidad de
     interaccion. Cualquier nueva capa invasiva reabriria riesgos ya resueltos.
   - **Alternatives considered:** Envolver bloques o paginas con capas extras
     para manejar zoom por separado. Se descarta por fragilidad sobre cursor y
     seleccion.

## Risks / Trade-offs

- [Riesgo] Un escalado visual mal integrado puede desalinear `sheet`,
  contenido, contador y page context.
  Mitigacion: tratar `zoomLevel` como input explicito del layout paginado y de
  las metricas derivadas.

- [Riesgo] El cambio de zoom puede introducir flicker o reposicionamientos
  bruscos en documentos largos.
  Mitigacion: mantener recalculo acotado y coordinado con el mecanismo de
  medicion ya endurecido en fases previas.

- [Riesgo] La API de zoom podria introducir ambiguedad entre modo controlado y
  no controlado si no se define con claridad.
  Mitigacion: seguir el patron ya usado por otras props del componente y cubrir
  ambos modos con pruebas especificas.

- [Riesgo] Integrar el zoom dentro de la toolbar podria degradar la UX en
  mobile/tablet.
  Mitigacion: ubicar el control como affordance de vista, separado de la
  toolbar de formato.

## Migration Plan

- Extender `editor.types.ts` con props de zoom (`zoomLevel`,
  `defaultZoomLevel`, `minZoomLevel`, `maxZoomLevel`, `onZoomChange`) sin
  romper la API existente.
- Ajustar `AppEditor.tsx` para resolver el estado de zoom y renderizar el
  control UI solo en `paginationMode="visual"`.
- Ajustar `AppEditor.module.css` para materializar la experiencia de zoom en la
  capa paginada sin desalinear `sheet`, `pageStack`, `contentFlow` ni el
  contador.
- Revisar `usePaginationMetrics.ts` para integrar `zoomLevel` en el recalculo
  del layout visual y sus offsets relevantes.
- Revisar `usePageContext.ts` para asegurar que el calculo de `Pagina X de Y`
  siga siendo coherente bajo cambios de zoom.
- Ampliar pruebas focalizadas de `AppEditor` para cubrir control UI, limites,
  compatibilidad con `PageBreak`, imagenes y no regresion de HTML.

## Open Questions

- ¿Conviene expresar el zoom visual principalmente como escala derivada de CSS
  variables con compensacion de layout, o hay un enfoque mas estable sobre la
  geometria paginada actual sin duplicar logica?
- ¿El control UI de zoom debe vivir cerca del contador de pagina o en otra zona
  del frame del editor para mantener mejor claridad visual en mobile y desktop?
