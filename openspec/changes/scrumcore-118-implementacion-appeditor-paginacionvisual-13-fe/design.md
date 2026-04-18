## Context

`SCRUMCORE-118` corresponde a la fase 13 FE de `AppEditor`, enfocada en
reemplazar la visualizacion actual basada en lineas guia por una representacion
de hojas reales con margenes visibles.

`AppEditor` ya soporta:
- modo paginado visual con `paginationMode="visual"`;
- metricas de paginacion mediante `usePaginationMetrics`;
- contador de pagina actual mediante `usePageContext`;
- `PageBreak` manual;
- zoom visual;
- compatibilidad con imagenes locales, remotas, resize y alineacion.

El problema actual es de representacion:
- el documento sigue percibiendose como una superficie continua;
- las guias visibles atraviesan visualmente el contenido;
- la caja util del documento no se percibe con claridad;
- la UX sigue lejos de una hoja tipo Word/Docs.

La referencia principal para esta fase es
`docs/Architecture/AppEditor/13-FE-AppEditor-hojas-reales-margenes-visuales.md`.

## Goals / Non-Goals

**Goals:**
- Reemplazar guias visibles por hojas visuales reales tipo A4.
- Hacer que los margenes top/right/bottom/left sean claramente visibles.
- Diferenciar visualmente la hoja del workspace exterior.
- Mantener una unica instancia de `ProseMirror`.
- Reutilizar la logica actual de metricas y paginacion como base interna.
- Conservar compatibilidad con contador, zoom, `PageBreak` e imagenes.

**Non-Goals:**
- No implementar todavia salto automatico real por contenido.
- No dividir el documento en multiples editores o multiples arboles de contenido.
- No alterar HTML persistido.
- No introducir nodos artificiales adicionales en el documento.
- No rehacer la arquitectura de paginacion existente.

## Decisions

1. **Mantener una sola capa editable y cambiar solo la visualizacion**
   - **Decision:** Conservar una unica instancia de `.ProseMirror` y reestructurar
     solo el layout visual alrededor de ella.
   - **Rationale:** El cambio es visual; tocar el modelo del documento o dividir
     el editor generaria demasiado riesgo sobre cursor, seleccion y persistencia.
   - **Alternatives considered:** Multiples contenedores o editores por pagina.
     Se descarta por alto riesgo y por violar el requerimiento.

2. **Convertir `sheet` en una hoja real con workspace exterior**
   - **Decision:** Reforzar la jerarquia `editorWrapper -> canvas -> sheet ->
     surface/content`, haciendo que `sheet` represente visualmente la hoja A4 y
     que `canvas` represente el workspace exterior.
   - **Rationale:** Esa estructura ya existe y permite evolucionar la UX sin
     rehacer el componente.
   - **Alternatives considered:** Crear wrappers extra o una segunda capa de
     paginas fake. Se descarta porque este ticket no persigue segmentacion real.

3. **Representar margenes como caja visual del documento**
   - **Decision:** Mostrar los margenes a traves del layout de la hoja y del area
     de contenido, sin meter `padding` estructural persistido en `.ProseMirror`.
   - **Rationale:** Los margenes deben sentirse como limites del documento, pero
     no deben alterar la semantica del HTML guardado.
   - **Alternatives considered:** Aplicar padding directo sobre el contenido
     editable. Se descarta por mezclar persistencia visual con representacion UI.

4. **Ocultar las guias visibles pero mantener su calculo**
   - **Decision:** Las guias seguiran existiendo como metricas internas
     provenientes de `usePaginationMetrics`, pero dejaran de renderizarse como
     lineas visibles.
   - **Rationale:** El contador y la futura segmentacion todavia dependen de esas
     metricas. Quitarlas de la logica romperia mas de lo que este ticket pretende.
   - **Alternatives considered:** Eliminar completamente guias y metricas.
     Se descarta por introducir deuda para la siguiente fase.

5. **Mantener compatibilidad explicita con zoom e imagenes**
   - **Decision:** El nuevo layout debe respetar la coexistencia con zoom visual,
     `PageBreak`, `data-width`, `data-align` y el flujo de imagenes locales.
   - **Rationale:** Estas capacidades ya estan integradas al editor compartido y
     no deben verse como optionales o de segundo nivel.
   - **Alternatives considered:** Ajustar solo el layout base y tratar el resto
     como deuda posterior. Se descarta por alto riesgo de regresion inmediata.

## Risks / Trade-offs

- [Riesgo] Hacer mas marcada la hoja puede desalinear visualmente overlays como
  contador o guias internas si no se recalibra bien el contenedor.
  Mitigacion: mantener una sola fuente de medidas basada en `sheet` y `canvas`.

- [Riesgo] El contenido podria percibirse recortado si la caja util se renderiza
  de forma demasiado agresiva.
  Mitigacion: este ticket solo redefine la percepcion visual, no la segmentacion
  del contenido.

- [Riesgo] Cambios de CSS pueden afectar modo continuo o toolbar sticky.
  Mitigacion: aislar reglas al modo `paginationMode="visual"` y validar regresion.

- [Riesgo] El usuario puede esperar salto automatico real al ver hojas reales.
  Mitigacion: documentar que esta fase es visual y deja preparada la siguiente
  capa de segmentacion automatica.

## Migration Plan

- Revisar el layout actual de `AppEditor` en modo visual.
- Ajustar `AppEditor.tsx` para que la hoja A4 tenga representacion visual mas
  fuerte y los margenes se lean como caja interna del documento.
- Ajustar `AppEditor.module.css` para:
  - diferenciar `canvas` como workspace;
  - reforzar `sheet` como hoja real;
  - ocultar guias visibles;
  - mantener compatibilidad con contador, zoom e imagenes.
- Actualizar pruebas del modo visual para reflejar hojas reales con margenes.

## Open Questions

- ¿Conviene exponer visualmente la caja util del documento con contraste suave o
  debe mantenerse completamente blanca y solo sugerida por el layout?
- ¿La sombra y el fondo del workspace deben mantenerse sobrios tipo Word o mas
  neutros para no competir con el contenido?
