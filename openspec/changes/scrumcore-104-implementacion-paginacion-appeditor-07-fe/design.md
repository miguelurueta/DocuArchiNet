## Context

`SCRUMCORE-104` corresponde a la fase 07 FE de `AppEditor`, enfocada en medir
el contenido renderizado del modo `paginationMode="visual"` y dibujar guias
visuales de pagina sobre la base introducida en `SCRUMCORE-103`. El editor ya
puede mostrarse como hoja visual A4 dentro de un `canvas`, pero aun no indica
cuando el contenido supera una pagina ni cuantos cortes visuales estimados
existen.

La referencia principal para esta fase es
`docs/Architecture/AppEditor/07-FE-AppEditor-paginacion-guias-metricas.md`. El
alcance no es implementar paginacion real del documento: el contenido sigue
siendo un flujo continuo dentro de ProseMirror. La nueva responsabilidad es
medir esa superficie editable, calcular paginas estimadas y dibujar guias
visuales fuera del documento.

## Goals / Non-Goals

**Goals:**
- Medir la altura renderizada del contenido usando `.ProseMirror`.
- Calcular `pageContentHeight` y `totalPages` a partir de formato y margenes.
- Dibujar guias visuales de pagina en un overlay absoluto fuera de ProseMirror.
- Recalcular metricas cuando cambie el contenido o el tamaño del contenedor.
- Mantener escritura, seleccion, toolbar y serializacion sin regresion.

**Non-Goals:**
- No insertar saltos de pagina reales ni nodos persistidos.
- No dividir contenido en paginas estructurales.
- No modificar el HTML serializado con metadata de guias.
- No implementar aun contador de pagina actual.
- No resolver aun `PageBreak` manual.

## Decisions

1. **Medicion basada en DOM renderizado, no en el documento ProseMirror**
   - **Decision:** Usar `scrollHeight` de `.ProseMirror` como fuente primaria de la altura del contenido.
   - **Rationale:** La paginacion visual depende del layout real despues de estilos, imagenes, headings y padding; el arbol del documento por si solo no entrega esa medida efectiva.
   - **Alternatives considered:** Calcular altura a partir del contenido estructural del documento o de nodos Tiptap. Se descarta porque no refleja el alto visual real y complica el soporte de contenido enriquecido.

2. **Hook dedicado para metricas de paginacion**
   - **Decision:** Encapsular el calculo de metricas en `application` mediante un hook tipo `usePaginationMetrics`.
   - **Rationale:** El ticket define claramente que `application` debe encargarse de las metricas y `presentation` del overlay. Separarlo evita mezclar calculo DOM con la composicion visual del componente.
   - **Alternatives considered:** Resolver toda la logica dentro de `AppEditor.tsx`. Se descarta por acoplar demasiado el calculo al render y dificultar pruebas focalizadas.

3. **Overlay absoluto desacoplado del contenido editable**
   - **Decision:** Dibujar las guias en una capa absoluta, posicionada sobre la hoja o el canvas, fuera de `.ProseMirror` y con `pointer-events: none`.
   - **Rationale:** Las guias son affordances visuales, no contenido editable. Deben poder superponerse sin afectar seleccion, foco ni eventos del editor.
   - **Alternatives considered:** Insertar lineas dentro del contenido del editor o como pseudo-elementos de `.ProseMirror`. Se descarta porque interfiere con la experiencia de edicion y mezcla decoracion con documento.

4. **Recalculo sincronizado con layout usando `requestAnimationFrame` y debounce**
   - **Decision:** Recalcular metricas tras cambios relevantes usando un pipeline de sincronizacion visual (`requestAnimationFrame`, `useLayoutEffect`) mas debounce corto de `16ms` a `50ms`.
   - **Rationale:** Medir el DOM en cada pulsacion sin control es costoso y puede provocar jitter visual. El debounce corto mantiene fluidez sin volver el recalculo perceptiblemente lento.
   - **Alternatives considered:** Medir de forma inmediata en cada transaccion del editor. Se descarta por costo y por riesgo de layout thrashing.

5. **Metricas solo en modo `visual`**
   - **Decision:** Activar la medicion y las guias unicamente cuando `paginationMode="visual"`.
   - **Rationale:** El modo continuo no necesita esta logica y cualquier medicion extra seria costo innecesario. Ademas preserva el comportamiento y rendimiento actuales para consumidores que no usan paginacion.
   - **Alternatives considered:** Medir siempre y ocultar el overlay segun modo. Se descarta por overhead innecesario.

## Risks / Trade-offs

- [Riesgo] `scrollHeight` puede variar por carga tardia de fuentes o imagenes y generar desalineacion temporal de guias.
  Mitigacion: recalcular despues del render visible y reaccionar a cambios del contenedor o del contenido con observadores o sincronizacion adicional.

- [Riesgo] Un overlay mal posicionado podria tapar seleccion o interaccion del usuario.
  Mitigacion: mantener `pointer-events: none`, z-index controlado y guias fuera del flujo editable.

- [Riesgo] El recalculo frecuente puede afectar rendimiento en documentos largos.
  Mitigacion: limitar medicion al modo visual, usar debounce corto y evitar `setState` si las metricas no cambiaron realmente.

- [Riesgo] Contenido muy alto, imagenes redimensionables o cambios de viewport pueden exigir multiples recalculos y producir flicker.
  Mitigacion: centralizar metricas en un hook y actualizar el overlay solo cuando los offsets finales cambien.

## Migration Plan

- Crear hook o helper de metricas de paginacion en `application`.
- Exponer desde `presentation/AppEditor.tsx` el overlay visual de guias cuando `paginationMode="visual"`.
- Extender `AppEditor.module.css` con clases de overlay y guias sin alterar el contrato de hoja ya introducido en la fase 103.
- Agregar pruebas focalizadas para:
  - calculo de paginas estimadas;
  - render del overlay;
  - ausencia de metadata de guias en el HTML.
- Mantener intactos Tiptap, serializacion y toolbar.

## Open Questions

- ¿Conviene usar `ResizeObserver` en esta fase para detectar cambios del canvas/hoja, o basta con recalculo por cambios de contenido y resize del viewport?
- ¿Las guias deben dibujarse desde la primera pagina como referencia visual completa, o solo a partir del primer corte adicional?
