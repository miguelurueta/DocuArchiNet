## Context

`SCRUMCORE-112` corresponde a la fase 11 FE de `AppEditor`, enfocada en
agregar alineacion horizontal persistida para imagenes dentro del editor.

Actualmente `AppEditor` ya soporta:
- insercion de imagen;
- resize persistido mediante `data-width`;
- seleccion de imagen y actualizacion de atributos;
- toolbar reusable con controles contextuales.

El hueco funcional es que la imagen aun no tiene una forma estable y persistida
de ubicarse horizontalmente. El cambio debe resolver esa necesidad sin entrar en
posicionamiento libre ni comprometer la serializacion actual.

La referencia principal para esta fase es
`docs/Architecture/AppEditor/11-FE-AppEditor-alineacion-horizontal-imagen.md`.

## Goals / Non-Goals

**Goals:**
- Persistir alineacion horizontal de imagen con `data-align`.
- Rehidratar correctamente `left`, `center` y `right`.
- Exponer un comando `setImageAlign`.
- Mostrar controles de alineacion solo cuando la imagen este activa.
- Mantener compatibilidad con `data-width` y con resize existente.

**Non-Goals:**
- No implementar posicionamiento libre.
- No usar canvas ni drag horizontal arbitrario.
- No representar alineacion con estilos inline.
- No modificar la estructura base del documento mas alla del atributo del nodo imagen.

## Decisions

1. **Extender el nodo imagen actual**
   - **Decision:** Agregar el atributo `align` a `ResizableImage` en lugar de
     crear un nodo nuevo o wrapper adicional.
   - **Rationale:** La imagen ya concentra resize, serializacion y render. La
     alineacion debe vivir en el mismo contrato para evitar desincronizacion.
   - **Alternatives considered:** Crear wrapper alrededor de la imagen. Se
     descarta porque introduce mas complejidad estructural y riesgo de regresion.

2. **Persistencia con `data-align`**
   - **Decision:** Serializar y parsear la alineacion mediante
     `data-align="left|center|right"`.
   - **Rationale:** Es explicito, estable y desacoplado del CSS. Facilita
     compatibilidad con HTML guardado y rehidratacion futura.
   - **Alternatives considered:** Clases CSS o estilos inline. Se descarta por
     ir contra el requerimiento y por ser menos robusto.

3. **Render visual resuelto por CSS**
   - **Decision:** Mantener el HTML limpio y resolver el layout horizontal con
     selectores `img[data-align=...]` en `AppEditor.module.css`.
   - **Rationale:** El comportamiento visual debe quedar separado de la
     persistencia y no mezclarse con atributos de estilo.
   - **Alternatives considered:** Injectar `margin-left/right` inline desde la
     extension. Se descarta por romper la regla tecnica del ticket.

4. **Comando dedicado `setImageAlign`**
   - **Decision:** Exponer `setImageAlign('left' | 'center' | 'right')` desde la
     extension de imagen.
   - **Rationale:** La actualizacion de atributos debe pertenecer al nodo imagen
     y no quedar incrustada en `presentation`.
   - **Alternatives considered:** Llamar directamente `updateAttributes` desde
     toolbar sin comando semantico. Se descarta por menor claridad y menor reuso.

5. **Controles contextuales solo con imagen activa**
   - **Decision:** Mostrar los botones de alineacion de imagen solo cuando la
     imagen este activa o seleccionada como nodo.
   - **Rationale:** Evita ruido en la toolbar y mantiene el patron contextual ya
     usado para interacciones de imagen.
   - **Alternatives considered:** Mostrar siempre esos controles. Se descarta
     por saturar la barra con acciones irrelevantes la mayor parte del tiempo.

## Risks / Trade-offs

- [Riesgo] La extension actual construye `style` para width y puede mezclar
  responsabilidades con la nueva alineacion.
  Mitigacion: mantener `style` solo para width/max-width/height y mover la
  alineacion completamente a CSS basado en `data-align`.

- [Riesgo] La deteccion de imagen activa puede no cubrir todos los estados si la
  seleccion no es estrictamente `editor.isActive('image')`.
  Mitigacion: reutilizar la logica ya existente que detecta imagen activa o nodo seleccionado.

- [Riesgo] Cambios de alineacion podrian afectar resize o el outline de imagen seleccionada.
  Mitigacion: validar que `data-width` siga persistiendo y que los estilos de
  seleccion continúen aplicando sobre `img`.

- [Riesgo] Controles extra en toolbar pueden volverla mas cargada.
  Mitigacion: renderizarlos solo cuando la imagen este activa.

## Migration Plan

- Extender `ResizableImage` con atributo `align`.
- Agregar parsing/render de `data-align`.
- Crear comando `setImageAlign`.
- Integrar controles de alineacion en la toolbar para imagen activa.
- Agregar estilos CSS por `data-align`.
- Actualizar pruebas de serializacion, rehidratacion, comando e integracion.

## Open Questions

- ¿Los botones de alineacion de imagen deben reutilizar los iconos de alineacion
  de texto o conviene diferenciarlos visualmente?
- ¿Cuando una imagen no tiene `data-align`, conviene serializar explicitamente
  `left` o dejarlo ausente y tratarlo como default al parsear/renderizar?
