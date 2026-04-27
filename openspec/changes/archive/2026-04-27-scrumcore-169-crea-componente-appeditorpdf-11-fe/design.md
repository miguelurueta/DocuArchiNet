## Context

El ticket `SCRUMCORE-169` agrega la capability `app-appeditorpdf-11-fe` enfocada en **alineacion horizontal de imagen** dentro de la experiencia reusable `AppEditorPdf`.

El editor base (`AppEditor`) ya cuenta con soporte de alineacion persistida:

- atributo `data-align` con valores `left|center|right`;
- comando `editor.commands.setImageAlign(...)` (extension de imagen);
- estilos CSS que materializan el alineado;
- pruebas que validan serializacion/rehidratacion y que el cambio de alineacion preserva `data-width`.

El objetivo de FE-11 es asegurar que este comportamiento forme parte del contrato reusable consumible desde `AppEditorPdf`, sin duplicar logica ni romper paginacion visual.

## Goals / Non-Goals

**Goals**

- Exponer/garantizar el control de alineacion horizontal de imagen en el contrato de `AppEditorPdf`.
- Persistir `data-align` y mantener compatibilidad de round-trip HTML.
- Asegurar que cambiar alineacion no pierde `data-width`.
- Mantener estabilidad en modo `paginationMode="visual"`.

**Non-Goals**

- Crear una segunda implementacion de alineacion fuera del editor base.
- Cambiar el formato de persistencia (se mantiene `data-align`).
- Reescribir la extension de imagen o el pipeline de render.

## Decisions

1. Reutilizar el comando existente `setImageAlign` del editor base.
Rationale: mantiene una sola fuente de verdad y aprovecha pruebas ya existentes.

2. La persistencia se mantiene via `data-align` (y `data-width` intacto).
Rationale: ya hay compatibilidad de serializacion/rehidratacion en la extension actual.

3. El contrato en `AppEditorPdf` no introduce dependencias nuevas ni logica duplicada.
Rationale: `AppEditorPdf` debe actuar como wrapper que expone superficie reusable.

## Implementation Sketch

- Verificar que `AppEditorToolbar` ya expone la UI de "Alineacion de imagen" cuando corresponde; de ser necesario, asegurar que no quede bloqueada por el wrapper `AppEditorPdf`.
- Agregar pruebas FE-11 que validen desde el punto de vista del contrato:
  - existe el control de alineacion cuando una imagen esta seleccionada (o cuando el editor indica que puede alinearse),
  - `data-align` persiste y no rompe `data-width` (cubierto por pruebas del editor base, pero se agrega al menos 1 smoke de integracion).

## Risks / Trade-offs

- [Riesgo] Doble cobertura de pruebas (base editor + wrapper).
Mitigacion: mantener pruebas FE-11 de integracion ligeras y dejar la logica pesada en pruebas del editor base.

- [Riesgo] Dependencia de comportamiento interno del editor al simular seleccion de imagen en tests de React.
Mitigacion: mock del editor o pruebas a nivel de extension donde corresponda.

