## Context

El ticket `SCRUMCORE-166` introduce la capability `app-appeditorpdf-09-fe`, enfocada en salto de pagina manual dentro de la experiencia reusable `AppEditorPdf`. El repositorio ya cuenta con soporte tecnico previo para `PageBreak` en `AppEditor`:

- extension ProseMirror/Tiptap persistida con `data-page-break="true"`;
- comando `editor.commands.insertPageBreak()`;
- reglas de no duplicar saltos consecutivos;
- integracion con la paginacion visual existente.

El objetivo de FE-09 no es reimplementar ese soporte, sino exponerlo de forma consistente en la capa shared UI para consumidores de `AppEditorPdf` y mantener compatibilidad con el comportamiento ya existente del editor base.

Restricciones relevantes:
- Mantener React 19 + TypeScript estricto + ESM.
- No duplicar la logica de `PageBreak`; debe reutilizar la implementacion base de `AppEditor`.
- No romper la experiencia de lectura/paginacion visual ya entregada en FE-07 y FE-08.
- Evitar dependencias nuevas solo para el salto manual.

Stakeholders: equipo FE que usa `AppEditorPdf` para flujos de gestion documental, QA para pruebas de persistencia del salto manual, y arquitectura para consistencia entre editores reusables.

## Goals / Non-Goals

**Goals:**
- Definir un contrato reusable para insertar saltos de pagina manuales desde `AppEditorPdf`.
- Reutilizar la extension existente de `PageBreak` sin introducir una segunda fuente de verdad.
- Mantener la compatibilidad con el modo visual paginado y con el contador/contexto entregados en FE-08.
- Dejar trazabilidad testable del salto manual persistido en HTML y del comportamiento de cursor alrededor del corte.

**Non-Goals:**
- Reescribir el motor de paginacion visual.
- Cambiar la estructura interna del documento o fragmentarlo en paginas fisicas.
- Introducir un sistema nuevo de atajos globales o toolbars externas.
- Mover la logica de `PageBreak` fuera del editor base.

## Decisions

1. Reutilizar el comando `insertPageBreak` existente en `AppEditor` y exponerlo desde `AppEditorPdf` como parte del contrato reusable.
Rationale: evita duplicacion de logica y preserva el comportamiento probado de la extension.
Alternativa considerada: implementar un nuevo comando dentro de `AppEditorPdf`.
Por que no: crearia dos caminos para la misma accion y aumentaria el riesgo de divergencia.

2. Mantener el salto manual como nodo atomico persistido con `data-page-break="true"`.
Rationale: el formato ya esta soportado por parsing/render actual y es compatible con serializacion HTML estable.
Alternativa considerada: usar una marca de estilo o separador visual no persistido.
Por que no: no garantiza round-trip ni interoperabilidad entre edicion y render.

3. Exponer la accion manual mediante un control reusable en la capa `AppEditorPdf` sin obligar a consumidores a renderizarlo.
Rationale: permite adopcion incremental; los consumidores avanzados pueden añadir o retirar el control segun su flujo.
Alternativa considerada: forzar el boton en toda instancia del componente.
Por que no: reduce flexibilidad y puede introducir ruido en consumidores que solo requieren lectura.

4. Validar el comportamiento con pruebas centradas en el contrato visible: insercion, persistencia HTML, no duplicados consecutivos y navegacion del cursor alrededor del salto.
Rationale: el valor del cambio esta en el comportamiento observable; las pruebas deben capturar el round-trip del contrato.
Alternativa considerada: pruebas internas de implementacion.
Por que no: acoplan el test a detalles del editor base y dificultan evolucion.

## Risks / Trade-offs

- [Riesgo] Duplicar el control de salto manual entre `AppEditor` y `AppEditorPdf` -> Mitigacion: delegar el comando al editor base y mantener un unico punto de implementacion.
- [Riesgo] El control pueda interferir con consumidores que no necesitan insercion manual -> Mitigacion: exponerlo como accion opcional/slot, no como requisito obligatorio.
- [Riesgo] Inserciones repetidas generen marcas consecutivas no deseadas -> Mitigacion: respetar la regla existente de no multiples consecutivos y cubrirla con tests.
- [Riesgo] Inconsistencias entre HTML persistido y estado visual tras rehidratar contenido -> Mitigacion: validar parsing/render del nodo `data-page-break` en pruebas de round-trip.

## Migration Plan

1. Definir el contrato FE-09 en specs para `app-appeditorpdf-09-fe`.
2. Exponer el comando/control reusable de salto manual en la superficie de `AppEditorPdf`.
3. Reutilizar la extension/comando existente de `AppEditor` para insertar el nodo `PageBreak`.
4. Agregar pruebas de persistencia y cursor alrededor del salto manual.
5. Validar `npm.cmd run test -- --run` y `npm.cmd run spec:validate`.

Rollback:
- Remover el control/contrato FE-09 desde `AppEditorPdf` y dejar intacto el soporte base de `AppEditor`.
- Conservar el comportamiento de `PageBreak` existente sin cambios en la extension.

## Open Questions

- El control de salto manual debe aparecer en la toolbar de `AppEditorPdf` o como accion expuesta para que el consumidor lo coloque?
- Se requiere atajo de teclado para insertar el salto, o por ahora solo boton/accion programatica?
- FE-09 debe limitarse a insercion manual o tambien debe incluir mejora de navegacion alrededor del salto?
- El contrato debe exponer `insertPageBreak` directo o una abstraccion mas general de comandos del documento?
