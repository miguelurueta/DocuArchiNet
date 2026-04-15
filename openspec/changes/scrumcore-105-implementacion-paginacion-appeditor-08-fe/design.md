## Context

`SCRUMCORE-105` corresponde a la fase 08 FE de `AppEditor`, enfocada en
introducir un contador visual de pagina actual sobre la base de paginacion
visual ya implementada en `SCRUMCORE-103` y `SCRUMCORE-104`. A esta altura el
editor ya puede renderizar una hoja visual, medir el contenido, calcular
paginas estimadas y dibujar guias de corte, pero aun no comunica al usuario en
que pagina esta trabajando.

La referencia principal para esta fase es
`docs/Architecture/AppEditor/08-FE-AppEditor-paginacion-contador-contexto.md`.
El objetivo no es cambiar el modelo de documento ni volver estructural la
paginacion: se trata de un contexto visual calculado a partir de cursor o
scroll, sin introducir metadata adicional en el contenido serializado.

## Goals / Non-Goals

**Goals:**
- Mostrar `Pagina X de Y` dentro del shell paginado del editor.
- Resolver `Y` desde las metricas de paginacion ya calculadas.
- Resolver `X` con prioridad por cursor y fallback por scroll.
- Mantener el contador discreto, visible y desacoplado del documento.
- Actualizar el contador de forma estable durante scroll, escritura y cambios de seleccion.

**Non-Goals:**
- No introducir `PageBreak` ni saltos manuales.
- No cambiar el HTML serializado ni la estructura del documento.
- No mover esta responsabilidad al contenido Tiptap.
- No agregar navegacion por paginas ni botones de ir a pagina.
- No rediseñar la toolbar ni el layout base de paginacion visual.

## Decisions

1. **Contexto de pagina como hook separado**
   - **Decision:** Encapsular el calculo de pagina actual en `application` con un hook dedicado tipo `usePageContext`.
   - **Rationale:** La fase 104 ya separo metricas de paginacion de la presentacion; el contador debe seguir el mismo patron y no sobrecargar `AppEditor.tsx` con logica de estado derivado.
   - **Alternatives considered:** Calcular `currentPage` directamente dentro del render del componente. Se descarta por acoplar medicion, scroll y seleccion a la presentacion.

2. **Prioridad por cursor, fallback por scroll**
   - **Decision:** Usar `editor.view.coordsAtPos(selection.from)` cuando el editor tiene foco y la seleccion es valida; solo si eso no es viable, usar el offset de scroll del contenedor paginado.
   - **Rationale:** El cursor refleja mejor la intencion actual del usuario que el scroll, especialmente en documentos largos donde la vista puede mostrar varias paginas a la vez.
   - **Alternatives considered:** Resolver siempre por scroll. Se descarta porque puede reportar una pagina distinta a la que realmente se esta editando.

3. **Calculo basado en altura util ya conocida**
   - **Decision:** Resolver `pageIndex = floor(offset / pageContentHeight) + 1` y acotar el resultado entre `1` y `totalPages`.
   - **Rationale:** Reutiliza el contrato ya establecido en la fase 104 y mantiene una formula simple, verificable y desacoplada del contenido estructural.
   - **Alternatives considered:** Recalcular pagina actual por coincidencia con guias o por nodos del documento. Se descarta porque agrega complejidad innecesaria para una fase de contexto visual.

4. **Contador como affordance discreta del shell**
   - **Decision:** Renderizar el contador en la esquina inferior derecha del shell paginado, fuera del flujo editable y con bajo peso visual.
   - **Rationale:** El contador debe ayudar a orientarse sin competir con la toolbar superior ni con el contenido del documento.
   - **Alternatives considered:** Insertarlo dentro de la toolbar o como elemento fijo sobre el contenido principal. Se descarta por ruido visual y riesgo de interferencia.

5. **Actualizacion performante y con guardas**
   - **Decision:** Debouncear scroll y evitar `setState` cuando la pagina actual no cambie.
   - **Rationale:** El contador puede reaccionar a eventos de alta frecuencia; sin guardas, provocaria renders innecesarios.
   - **Alternatives considered:** Actualizar en cada evento bruto de scroll o seleccion. Se descarta por costo y por riesgo de jitter visual.

## Risks / Trade-offs

- [Riesgo] `coordsAtPos` puede fallar o devolver coordenadas poco utiles en selecciones especiales, nodos complejos o estados transitorios del editor.
  Mitigacion: proteger la resolucion por cursor con `try/catch` y fallback inmediato a scroll.

- [Riesgo] El contador puede “saltar” entre paginas si el cursor esta cerca del borde o si el usuario escribe en un area visible mientras hace scroll.
  Mitigacion: priorizar cursor solo cuando el editor tenga foco real y mantener el fallback por scroll para estados ambiguos.

- [Riesgo] Una posicion visual inadecuada del contador puede tapar contenido o quedar demasiado cerca de las guias.
  Mitigacion: ubicarlo en una esquina del shell con contraste y padding controlados, fuera del area de texto.

- [Riesgo] El nuevo estado derivado puede generar renders adicionales en documentos grandes.
  Mitigacion: actualizar solo cuando cambie `currentPage` o `totalPages`, reutilizando metricas existentes.

## Migration Plan

- Crear `usePageContext` en `application`.
- Integrarlo en `presentation/AppEditor.tsx` solo para `paginationMode="visual"`.
- Agregar estilos del contador a `AppEditor.module.css`.
- Extender pruebas para:
  - calculo de pagina actual;
  - prioridad cursor/fallback scroll;
  - render discreto del contador.
- Mantener intactas metricas, guias, toolbar y serializacion HTML.

## Open Questions

- ¿Conviene ocultar el contador en documentos de una sola pagina o mantener `Pagina 1 de 1` siempre visible para consistencia?
- ¿La pagina actual debe calcularse sobre el borde superior visible del viewport o sobre la coordenada exacta del cursor cuando hay seleccion activa?
