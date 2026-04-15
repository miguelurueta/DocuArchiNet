## Context

`SCRUMCORE-103` corresponde a la fase 06 FE de `AppEditor`, enfocada en
introducir una base open source de paginacion visual sin depender de `Tiptap
Pages Pro` y sin convertir el documento en un modelo paginado real. El
componente ya existe en `src/app/Components/UI/AppEditor/` con soporte
controlled/uncontrolled, toolbar, tema visual y scroll interno del contenido,
pero hoy se renderiza unicamente como flujo continuo.

La referencia principal para esta fase es
`docs/Architecture/AppEditor/06-FE-AppEditor-paginacion-visual-base.md`. El
alcance real del ticket no es medir contenido ni insertar saltos de pagina;
esta fase solo debe crear el shell visual tipo hoja y la API minima para
activarlo sin romper la integracion actual del componente shared.

## Goals / Non-Goals

**Goals:**
- Introducir un modo `paginationMode="visual"` en `AppEditor`.
- Agregar API tipada para formato, orientacion y margenes de pagina.
- Renderizar el editor dentro de una estructura visual tipo documento:
  `editorWrapper -> canvas -> sheet -> content`.
- Mantener el documento como flujo continuo editable.
- Ubicar el scroll en el canvas del modo paginado sin doble scroll interno de hoja.
- Preservar compatibilidad con consumidores existentes que no usen paginacion.

**Non-Goals:**
- No implementar medicion de contenido ni guias de pagina en esta fase.
- No implementar contador de pagina actual.
- No insertar nodos `PageBreak` ni saltos persistidos en el HTML.
- No mover logica de paginacion a extensiones Tiptap.
- No cambiar el modelo de serializacion del contenido ni el comportamiento del hook base.

## Decisions

1. **Paginacion visual como layout, no como feature de Tiptap**
   - **Decision:** Resolver esta fase exclusivamente desde `presentation` y estilos, sin agregar extensiones Tiptap ni nodos nuevos al documento.
   - **Rationale:** El ticket pide una base visual open source y prohibe modificar la estructura del documento. La forma menos riesgosa es tratar la paginacion como layout de shell.
   - **Alternatives considered:** Implementar desde ahora una extension Tiptap de pagina o `PageBreak`. Se descarta porque mezcla alcance de fases posteriores y aumenta el riesgo sobre cursor, serializacion y undo/redo.

2. **API opt-in sin romper consumidores existentes**
   - **Decision:** Agregar props nuevas de paginacion (`paginationMode`, `pageFormat`, `pageOrientation`, `pageMargins`) con defaults que mantengan el comportamiento actual cuando no se usan.
   - **Rationale:** `AppEditor` ya es shared UI y tiene consumidores reales; la nueva capacidad debe ser incremental y no obligar migraciones.
   - **Alternatives considered:** Cambiar el layout por defecto de `AppEditor` a modo hoja. Se descarta por regresion visual potencial sobre integraciones actuales.

3. **Canvas externo con hoja centrada**
   - **Decision:** Introducir un `canvas` con fondo de workspace y una `sheet` centrada como superficie de documento, manteniendo el contenido editable dentro de esa hoja.
   - **Rationale:** Esto permite separar claramente el contenedor de scroll del area tipo papel y prepara la estructura para futuras fases de guias, contador y page breaks.
   - **Alternatives considered:** Pintar un borde o fondo tipo hoja directamente sobre `.ProseMirror`. Se descarta por acoplar excesivamente la presentacion al DOM interno del editor y dificultar las siguientes fases.

4. **Scroll en canvas, no en hoja**
   - **Decision:** En modo paginado visual, el desplazamiento del documento debe ocurrir sobre el `canvas`; la hoja no debe tener su propio scroll interno independiente.
   - **Rationale:** Evita la experiencia de doble scroll y alinea mejor con la nocion de documento continuo dentro de una superficie tipo papel.
   - **Alternatives considered:** Mantener el scroll actual en la superficie editable aun dentro de la hoja. Se descarta porque rompe la ilusion de documento y complica futuras guias de pagina.

5. **Dimensiones base declarativas y preparadas para extenderse**
   - **Decision:** Definir dimensiones base de referencia para `A4 portrait` (`794px` x `1123px`) y modelar formato, orientacion y margenes como datos de layout.
   - **Rationale:** Aunque esta fase no necesita medicion, si necesita una base estable y tipada para futuras fases de calculo de altura y cortes visuales.
   - **Alternatives considered:** Hardcodear solo una hoja A4 sin API configurable. Se descarta porque obliga refactor inmediato en la siguiente fase.

## Risks / Trade-offs

- [Riesgo] El cambio de scroll desde la superficie editable al canvas puede alterar percepcion de foco o experiencia de escritura si no se ajustan correctamente alturas y `min-height`.
  Mitigacion: limitar el cambio al modo `paginationMode="visual"` y validar escritura, foco y scroll en ambos modos.

- [Riesgo] La hoja visual puede introducir overflow horizontal o padding inconsistente en viewports intermedios.
  Mitigacion: mantener hoja centrada con ancho controlado y permitir que el canvas administre overflow de forma explicita.

- [Riesgo] La nueva API puede quedar adelantada respecto a funcionalidades no implementadas aun.
  Mitigacion: documentar claramente que esta fase solo resuelve layout base y que guias, contador y `PageBreak` pertenecen a fases posteriores.

- [Riesgo] El DOM adicional del modo paginado puede volver mas compleja la integracion con clases actuales como `surfaceClassName` o `minHeight`.
  Mitigacion: preservar el contrato actual y aislar el wrapper paginado como una capa optativa sin romper personalizacion existente.

## Migration Plan

- Extender `editor.types.ts` con las props nuevas de paginacion visual.
- Ajustar `presentation/AppEditor.tsx` para renderizar condicionalmente la estructura `editorWrapper -> canvas -> sheet -> content`.
- Actualizar `AppEditor.module.css` para introducir tokens y reglas de layout del modo visual.
- Mantener intactos `useAppEditor` y la configuracion Tiptap en esta fase.
- Extender pruebas focalizadas del componente para cubrir modo por defecto y modo `visual`.

## Open Questions

- ¿El modo visual debe quedar limitado a desktop en esta fase o tambien debe degradar con elegancia en mobile desde el primer release?
- ¿Conviene exponer desde ahora un `pageFormat` adicional aparte de `A4`, o es mejor dejar solo `A4` como baseline aunque la API ya quede preparada?
