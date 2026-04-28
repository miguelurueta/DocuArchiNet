# Design: SCRUMCORE-178 (GestionRespuesta - Tab Documentos)

## Goal

Definir la integracion visual del tab **Documentos** en `GestionRespuesta` manteniendo:

- Reuso de UI shared.
- Separacion por capas (modulo consume, shared provee).
- Estabilidad (sin flicker/regresiones de navegacion).

## Approach

- Identificar el contenedor actual del tab Documentos y su layout.
- Reusar:
  - `AppToolbar`/acciones existentes del modulo.
  - componentes shared para tablas/upload/acciones si ya existen.
- Si el tab muestra un editor/preview, preferir `AppEditorPdf` configurado por el modulo.

## Non-Goals

- No crear un nuevo componente shared si la necesidad es especifica del modulo.
- No alterar contratos de `AppEditorPdf` salvo que se detecte un gap real.

