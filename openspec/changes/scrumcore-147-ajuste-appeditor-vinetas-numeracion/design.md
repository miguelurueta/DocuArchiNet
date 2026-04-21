## Context

`SCRUMCORE-147` corresponde a la fase 19 FE de `AppEditor`, enfocada en
corregir el comportamiento visual de listas con viñetas y numeracion dentro del
editor, respetar la margen ya establecida del documento y simplificar la
estructura renderizada removiendo el wrapper intermedio redundante del contenido.

`AppEditor` ya soporta:
- toolbar enriquecida con `bullet list`, `ordered list` y `task list`;
- modo continuo y modo `paginationMode="visual"`;
- estructura paginada `editorWrapper -> canvas -> zoomStage -> sheet -> contentFlow`;
- margenes de pagina derivados desde `pageMargins`;
- una unica instancia editable de `ProseMirror`;
- zoom visual, contador de pagina y overlays asociados al modo paginado.

El problema actual no esta en la ausencia de soporte de listas, sino en la
composicion visual:
- `.ProseMirror` ya aplica padding base para el area editable;
- `ul` y `ol` agregan una sangria adicional que empuja el contenido mas de lo
  necesario respecto a la margen visual vigente;
- la jerarquia JSX mantiene una capa `surface` / `surfacePaged` entre el shell
  del editor y `EditorContent`, aun cuando esa capa ya no aporta valor
  estructural suficiente en este flujo.

La referencia principal para esta fase es
`docs/Architecture/AppEditor/19-FE-AppEditor-ajuste-listas-margenes-y-remocion-wrapper.md`.

## Goals / Non-Goals

**Goals:**
- Corregir la sangria de listas `ul/ol` para que respete la margen visual ya
  resuelta por el editor.
- Mantener legibilidad correcta de viñetas y numeracion en items multilinea.
- Simplificar la estructura del contenido removiendo el wrapper intermedio
  redundante en modo continuo y paginado.
- Preservar `frame`, `editorWrapper`, `canvas`, `sheet` y `contentFlow` como
  capas necesarias de la arquitectura paginada.
- Evitar breaking changes en el contrato publico de `AppEditor`.
- Mantener foco, scroll, accesibilidad y serializacion HTML sin regresiones.

**Non-Goals:**
- No rediseñar la paginacion visual completa ni rehacer el layout multi-hoja.
- No cambiar comandos de Tiptap ni el modelo del documento serializado.
- No modificar `pageMargins`, `zoomLevel` ni `PageBreak` como features.
- No introducir una nueva API publica para listas o indentation.
- No mezclar este cambio con backend, persistencia remota ni nuevas acciones de
  toolbar.

## Decisions

1. **Mantener la margen del documento como fuente de verdad**
   - **Decision:** La margen visible del editor seguira resolviendose desde las
     capas actuales (`contentFlow` en modo visual y padding base del editor en
     modo continuo), y las listas deben adaptarse a esa base en lugar de
     introducir una segunda sangria dominante.
   - **Rationale:** El problema reportado no exige una nueva definicion de
     margenes, sino evitar que `ul/ol` dupliquen desplazamiento visual sobre la
     superficie ya calibrada.
   - **Alternatives considered:** Aumentar o mover la margen general del editor.
     Se descarta porque corregiria el sintoma rompiendo la base ya validada del
     layout.

2. **Reducir la sangria propia de `ul/ol` y normalizar contenido interno**
   - **Decision:** Ajustar los selectores de listas en `AppEditor.module.css`
     para usar una sangria menor y estable, con `list-style-position: outside`
     y normalizacion de margenes internos en `li > p`.
   - **Rationale:** El render actual combina padding del editor con
     `padding-left` de listas, lo que desplaza demasiado el texto y empeora la
     alineacion percibida en bullets y numeracion.
   - **Alternatives considered:** Quitar todo padding de listas o usar
     `list-style-position: inside`. Se descarta porque degrada multilinea,
     numeracion y semantica visual del listado.

3. **Eliminar la capa `surface` / `surfacePaged` como wrapper intermedio**
   - **Decision:** El contenido editable debe colgar directamente de la
     estructura principal necesaria del editor, trasladando al nodo de
     `editorContent` las responsabilidades de layout visual que hoy recaen en
     `surface` o `surfacePaged`.
   - **Rationale:** Esa capa agrega complejidad DOM y pruebas accesorias sin ser
     indispensable para toolbar, scroll, zoom ni page context.
   - **Alternatives considered:** Mantener la capa actual y retocar solo CSS.
     Se descarta porque deja intacta una jerarquia redundante que el ticket pide
     simplificar.

4. **Preservar las capas estructurales de paginacion visual**
   - **Decision:** `frame`, `editorWrapper`, `canvas`, `zoomStage`, `sheet` y
     `contentFlow` permanecen como parte de la arquitectura estable del modo
     paginado.
   - **Rationale:** Esas capas si son necesarias para zoom, scroll, page shells,
     page counter y medicion visual; removerlas mezclaria este ticket con una
     refactorizacion mucho mayor.
   - **Alternatives considered:** Simplificar toda la estructura del modo
     visual. Se descarta por alto riesgo de regresion sobre fases 14, 15 y 16.

5. **Tratar el cambio como ajuste de presentacion, no de infraestructura Tiptap**
   - **Decision:** La correccion debe concentrarse en `presentation/AppEditor.tsx`,
     `AppEditor.module.css` y pruebas del componente, sin alterar
     `useAppEditor`, extensiones ni serializacion HTML.
   - **Rationale:** El comportamiento funcional de listas ya existe; el defecto
     esta en layout y composicion visual, no en comandos del editor.
   - **Alternatives considered:** Tocar extensiones de StarterKit o interceptar
     nodos list en Tiptap. Se descarta por sobreingenieria y por no atacar la
     causa real.

6. **Actualizar pruebas para reflejar la nueva jerarquia y proteger regresiones**
   - **Decision:** Ajustar `AppEditor.test.tsx` para dejar de depender de la
     presencia de `surface` / `surfacePaged` y validar la estructura resultante
     junto con el comportamiento esperado en modo visual.
   - **Rationale:** Hoy hay pruebas que verifican wrappers intermedios; si se
     simplifica el DOM sin actualizar test coverage, el cambio quedaria
     artificialmente bloqueado o mal protegido.
   - **Alternatives considered:** Mantener clases vacias solo para satisfacer
     tests existentes. Se descarta por esconder deuda estructural.

## Risks / Trade-offs

- [Riesgo] Reducir demasiado la sangria de listas puede hacer que viñetas o
  numeracion queden demasiado cerca del borde del texto.
  Mitigacion: ajustar de forma incremental y validar `ul`, `ol` y multilinea en
  pruebas y render real.

- [Riesgo] Quitar `surface` o `surfacePaged` puede afectar estilos heredados del
  modo continuo y del modo visual.
  Mitigacion: migrar al nodo correcto las responsabilidades de `min-height`,
  `display`, `overflow` y `box-sizing`, en vez de simplemente eliminar clases.

- [Riesgo] El modo visual podria perder alineacion entre contenido y hoja si la
  refactorizacion mueve responsabilidades a la capa equivocada.
  Mitigacion: conservar intacta la cadena `editorWrapper -> canvas -> zoomStage
  -> sheet -> contentFlow`.

- [Riesgo] Los tests actuales pueden depender de clases concretas de la
  jerarquia vieja.
  Mitigacion: reescribir expectativas hacia estructura y semantica util, no
  hacia wrappers cosméticos.

## Migration Plan

- Ajustar `presentation/AppEditor.tsx` para remover la capa `surface` /
  `surfacePaged` y conectar `TiptapEditorContent` directamente con la jerarquia
  necesaria del editor.
- Reasignar en `AppEditor.module.css` las reglas de layout requeridas para que
  `editorContent` y `editorContentPaged` absorban el comportamiento visual que
  siga siendo necesario tras remover el wrapper.
- Corregir estilos de listas en `AppEditor.module.css`:
  - sangria de `ul/ol`
  - posicion del marcador
  - normalizacion de `li > p`
- Verificar que modo continuo y modo visual mantengan scroll, altura minima y
  consistencia de contenido.
- Actualizar `AppEditor.test.tsx` para reflejar la nueva estructura y cubrir no
  regresion de modo paginado, zoom y contador.

## Open Questions

- ¿Conviene dejar una clase semantica en `editorContentPaged` para representar
  la antigua responsabilidad visual de `surfacePaged`, o es preferible colapsar
  completamente ese concepto en una sola capa?
- ¿Las listas de tareas (`task list`) necesitan un ajuste visual adicional
  independiente de `ul/ol`, o el cambio de sangria general las deja alineadas
  de forma suficiente?
