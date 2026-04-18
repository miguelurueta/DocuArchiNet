## Context

`SCRUMCORE-106` corresponde a la fase 09 FE de `AppEditor`, enfocada en
agregar saltos de pagina manuales persistidos sobre la base de paginacion
visual construida en `SCRUMCORE-103`, `SCRUMCORE-104` y `SCRUMCORE-105`.

Hasta este punto, `AppEditor` ya soporta:
- shell visual tipo hoja;
- metricas y guias de paginacion;
- contador de pagina actual.

Sin embargo, toda la paginacion sigue siendo estimada sobre un flujo continuo.
Esta fase introduce por primera vez una representacion persistida dentro del
documento para forzar limites de pagina manuales, sin implementar aun una
paginacion automatica real.

La referencia principal para esta fase es
`docs/Architecture/AppEditor/09-FE-AppEditor-pagebreak-manual.md`.

## Goals / Non-Goals

**Goals:**
- Crear una extension Tiptap `PageBreak` propia de `AppEditor`.
- Permitir insertar el salto manual mediante `editor.commands.insertPageBreak()`.
- Persistir y rehidratar el salto desde HTML usando `data-page-break`.
- Renderizar el salto como un bloque visual claro, no editable.
- Hacer que la paginacion visual trate cada `PageBreak` como limite duro.

**Non-Goals:**
- No implementar paginacion automatica real.
- No partir parrafos o nodos automaticamente por overflow.
- No introducir navegacion por paginas ni exportacion de documentos.
- No convertir el editor a un modelo interno de paginas separadas.

## Decisions

1. **Modelar `PageBreak` como nodo atomico de bloque**
   - **Decision:** Definir `PageBreak` como un nodo `block`, `atom: true`,
     `selectable: true` e `isolating: true`.
   - **Rationale:** El salto debe comportarse como un corte duro entre bloques,
     sin aceptar texto interno ni mezclarse con el contenido adyacente.
   - **Alternatives considered:** Usar un `mark` o un nodo inline. Se descarta
     porque no representa correctamente un limite estructural de pagina.

2. **Persistencia HTML con `data-page-break`**
   - **Decision:** Serializar el nodo como
     `<div data-page-break="true"></div>` y parsearlo desde ese atributo.
   - **Rationale:** Es una forma estable, simple y desacoplada del estilo
     visual, compatible con rehidratacion.
   - **Alternatives considered:** Usar clases CSS o comentarios HTML. Se
     descarta por ser menos explicito y mas fragil para parseo.

3. **Comando dedicado de insercion**
   - **Decision:** Exponer `editor.commands.insertPageBreak()` como entrypoint
     de insercion manual.
   - **Rationale:** Mantiene el comportamiento encapsulado en la extension y
     deja lista una integracion futura con toolbar o atajos.
   - **Alternatives considered:** Insertar HTML directo o manipular el documento
     desde `AppEditor.tsx`. Se descarta por acoplar presentacion y modelo.

4. **Prevencion de duplicados consecutivos**
   - **Decision:** Antes de insertar, validar los nodos adyacentes para evitar
     multiples `PageBreak` consecutivos o posiciones invalidas.
   - **Rationale:** Saltos duplicados degradan la UX y complican el recalculo de
     guias y contador sin aportar valor.
   - **Alternatives considered:** Permitir cualquier secuencia y limpiar luego.
     Se descarta por introducir estados intermedios innecesarios.

5. **Integracion con metricas visuales sin reestructurar Tiptap**
   - **Decision:** Reutilizar el editor continuo actual y ajustar las metricas
     para que calculen paginas por segmentos delimitados por `PageBreak`.
   - **Rationale:** Conserva la arquitectura abierta de las fases previas y
     evita intentar paginacion real dentro de ProseMirror.
   - **Alternatives considered:** Renderizar cada pagina como un editor
     independiente o insertar saltos automaticos. Se descarta por complejidad y
     por alto riesgo de regresion.

## Risks / Trade-offs

- [Riesgo] La navegacion del cursor alrededor de un nodo atomico puede sentirse
  extraña si no se manejan bien los puntos de insercion antes y despues.
  Mitigacion: probar seleccion, flechas, enter y backspace en torno al nodo.

- [Riesgo] Las metricas actuales basadas solo en `scrollHeight` no distinguen
  segmentos delimitados por `PageBreak`.
  Mitigacion: extender `usePaginationMetrics` para detectar offsets del nodo y
  reiniciar el calculo por tramos.

- [Riesgo] Un `PageBreak` persistido puede verse distinto entre modo continuo y
  modo visual.
  Mitigacion: definir una presentacion minima consistente incluso fuera del modo
  visual, sin depender totalmente del shell paginado.

- [Riesgo] La extension puede introducir regresiones en serializacion o en
  documentos existentes sin `PageBreak`.
  Mitigacion: mantener la extension opt-in y cubrir parsing, serializacion y
  rehidratacion con pruebas.

## Migration Plan

- Crear la extension `PageBreak` en `infrastructure`.
- Registrar la extension dentro del set actual de extensiones Tiptap.
- Exponer el comando `insertPageBreak`.
- Renderizar visualmente el nodo dentro del editor.
- Ajustar metricas de paginacion para tratar `PageBreak` como limite duro.
- Agregar pruebas de extension, serializacion, rehidratacion e integracion.

## Open Questions

- ¿Conviene exponer desde ya un boton visible en la toolbar o solo dejar el
  comando listo para una fase posterior?
- ¿El `PageBreak` debe verse tambien en `paginationMode="none"` como separador
  basico, o solo tener estilo completo en modo visual?
