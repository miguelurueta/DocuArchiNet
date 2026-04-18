## Context

`SCRUMCORE-119` corresponde a la fase 14 FE de `AppEditor`, enfocada en
introducir una segmentacion visual automatica por area util de pagina sobre el
modo `paginationMode="visual"` ya existente.

`AppEditor` ya soporta:
- una hoja visual A4 con workspace exterior;
- margenes visibles;
- metricas de paginacion mediante `usePaginationMetrics`;
- contador de pagina mediante `usePageContext`;
- `PageBreak` manual;
- zoom visual;
- compatibilidad con imagenes locales, remotas, resize y alineacion.

El problema actual es que el documento sigue siendo visualmente continuo:
- el contenido todavia puede cruzar los limites de hoja;
- no existe una percepcion real de salto entre paginas;
- `PageBreak` no participa aun como corte visual obligado del layout;
- la experiencia todavia no se acerca a un editor tipo Word/Docs.

La referencia principal para esta fase es
`docs/Architecture/AppEditor/14-FE-AppEditor-segmentacion-visual-area-util.md`.

## Goals / Non-Goals

**Goals:**
- Calcular el area util real de cada hoja a partir de formato y margenes.
- Medir el contenido renderizado dentro de `.ProseMirror`.
- Construir un modelo visual de paginas por acumulacion de alturas.
- Integrar `PageBreak` como corte forzado de nueva pagina.
- Mantener una sola instancia de `ProseMirror` y un documento continuo.
- Conservar compatibilidad con zoom, contador, scroll continuo e imagenes.

**Non-Goals:**
- No dividir realmente parrafos o nodos entre paginas.
- No crear multiples editores o multiples arboles de contenido.
- No clonar contenido para simular una pagina por separado.
- No modificar el HTML persistido ni la semantica del documento.
- No introducir exportacion, impresion ni persistencia estructural de paginas.

## Decisions

1. **Modelar la paginacion como una capa derivada de medicion**
   - **Decision:** La segmentacion se calculara a partir del DOM renderizado de
     `.ProseMirror`, generando un modelo de paginas derivado y efimero en lugar
     de mutar el documento fuente.
   - **Rationale:** El requisito central del ticket es mantener el documento
     unico en Tiptap. Un modelo derivado permite pagina visual sin tocar
     cursor, persistencia ni undo/redo.
   - **Alternatives considered:** Persistir metadatos de pagina en el documento
     o insertar nodos artificiales. Se descarta por acoplar presentacion y
     estructura.

2. **Usar area util como limite real de corte visual**
   - **Decision:** El algoritmo tomara como referencia el alto util calculado a
     partir de `pageHeight - top - bottom`, y no la altura completa de la hoja.
   - **Rationale:** La fase 13 ya hizo visibles los margenes; esta fase debe
     hacer que esos margenes impacten la percepcion real del flujo del contenido.
   - **Alternatives considered:** Cortar por altura total de la hoja. Se
     descarta porque contradice el objetivo de segmentacion por area util.

3. **Integrar `PageBreak` como breakpoint explicito**
   - **Decision:** `PageBreak` actuara como reinicio obligatorio del acumulado de
     altura para abrir una nueva hoja visual, incluso si aun queda espacio.
   - **Rationale:** El salto manual ya existe como capacidad del editor; esta
     fase debe volverlo parte del layout paginado y no solo un elemento aislado.
   - **Alternatives considered:** Tratar `PageBreak` solo como sugerencia
     visual. Se descarta por ambiguedad y por no cumplir la expectativa del
     usuario.

4. **Mantener scroll continuo y hoja unica editable**
   - **Decision:** El `canvas` seguira siendo un contenedor de scroll continuo y
     la capa de paginas se resolvera como presentacion alrededor del mismo flujo
     editable.
   - **Rationale:** Separar cada pagina en su propio contenedor editable elevaria
     demasiado el riesgo sobre seleccion, mapping de posiciones y foco.
   - **Alternatives considered:** Renderizar una instancia editable por pagina.
     Se descarta por violar la restriccion principal del ticket.

5. **Aceptar overflow controlado para bloques mas altos que la pagina**
   - **Decision:** Si una imagen o bloque excede el alto util de pagina, se
     mantendra integro y podra desbordar visualmente la hoja sin romper el
     editor ni fragmentar el nodo.
   - **Rationale:** El ticket prohibe split real de contenido. Forzar un corte
     interno dentro de imagenes o nodos complejos seria inconsistente y riesgoso.
   - **Alternatives considered:** Escalar automaticamente el contenido o
     fragmentarlo. Se descarta porque altera semantica y comportamiento esperado.

6. **Recalcular con sincronizacion controlada**
   - **Decision:** La medicion y el recalculo de segmentos deben apoyarse en
     `ResizeObserver`, `requestAnimationFrame` y sincronizacion acotada para no
     disparar trabajo costoso en cada cambio minimo.
   - **Rationale:** La paginacion visual depende del DOM real; sin control de
     recalc se introducirian flicker y reflows innecesarios.
   - **Alternatives considered:** Recalcular sincronicamente en cada input. Se
     descarta por impacto negativo en performance y experiencia de edicion.

## Risks / Trade-offs

- [Riesgo] Medir el DOM por bloques puede introducir jitter visual durante
  cambios rapidos de contenido.
  Mitigacion: agrupar recalculos en frames y evitar lecturas/escrituras de DOM
  intercaladas.

- [Riesgo] La segmentacion visual puede desalinear el contador de pagina si el
  contexto sigue basandose en metricas previas.
  Mitigacion: usar el modelo de segmentos como fuente coherente para layout y
  page context.

- [Riesgo] Imagenes grandes o bloques atipicos pueden romper la percepcion de
  hoja si se fuerzan cortes agresivos.
  Mitigacion: privilegiar integridad del nodo y aceptar overflow controlado.

- [Riesgo] El zoom puede invalidar offsets y hacer que el usuario perciba
  paginas equivocadas.
  Mitigacion: recalcular segmentos ante cambios de zoom con una sola fuente de
  medidas derivadas.

- [Riesgo] La capa visual de paginas puede afectar cursor, seleccion o
  interaccion si introduce overlays invasivos.
  Mitigacion: mantener overlays sin bloquear eventos y sin interferir con la
  superficie editable.

## Migration Plan

- Revisar `usePaginationMetrics.ts` para extenderlo desde metricas simples de
  guia hacia un modelo de segmentos visuales por area util.
- Ajustar `AppEditor.tsx` para consumir el modelo de paginas y representar la
  separacion visual hoja a hoja sin dividir la instancia editable.
- Ajustar `AppEditor.module.css` para materializar el salto entre hojas y la
  caja util del documento dentro del `canvas`.
- Integrar `PageBreak` al algoritmo como corte duro de nueva pagina.
- Revisar `usePageContext.ts` para asegurar coherencia entre segmento actual,
  contador y scroll/zoom.
- Actualizar pruebas del editor para cubrir documento corto, multipagina,
  `PageBreak`, zoom e imagenes grandes.

## Open Questions

- ¿Conviene que el modelo de segmentos viva dentro de `usePaginationMetrics` o
  debe extraerse a una capa nueva para no sobrecargar ese hook?
- ¿La pagina actual debe priorizar la posicion del cursor, la del scroll o una
  combinacion de ambas una vez exista segmentacion visual real?
