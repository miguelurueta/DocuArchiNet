## Context

`SCRUMCORE-162` corresponde al `05-FE` del roadmap de `AppEditorPdf` y se
enfoca en consolidar la integracion visual tipo **full surface** dentro de
`GestionRespuestaMainTab`.

El ticket `04-FE` ya movio el consumidor a `AppEditorPdf`, pero el contenedor
del modulo conserva reglas de shell que siguen agregando una capa visual extra
(padding/borde/estructura redundante) y evita que el editor sea la superficie
dominante del panel principal.

## Scope

- Mantener `AppEditorPdf` como editor principal del workbench.
- Simplificar `GestionRespuestaEditorContainer` a wrapper neutro.
- Ajustar estilos del contenedor para ocupar toda la superficie util sin shell
  adicional.
- Conservar comportamiento funcional existente: steps, adjuntos, toolbar y
  panel lateral.

## Non-Goals

- No modificar logica interna de `AppEditorPdf` o `AppEditor`.
- No alterar reglas de negocio del flujo de envio.
- No cambiar contratos publicos de `gestionCorrespondencia`.

## Decisions

### Decision 1: Wrapper neutro con contrato explicito

**Decision:** `GestionRespuestaEditorContainer` mantiene el `aria-label`
existente y publica `data-editor-shell="neutral"` para declarar contrato de
integracion sin capa visual intermedia.

**Rationale:** Preserva accesibilidad y agrega un punto estable de validacion en
pruebas sin acoplarse a nombres de clases CSS modules.

### Decision 2: Full surface desde estilos del modulo consumidor

**Decision:** `editorContainer` deja de aplicar border, padding y estructura de
header interno; pasa a `display: flex` + `min-height: 0` + `overflow: hidden`
para que `AppEditorPdf` ocupe el 100% de la superficie del panel.

**Rationale:** El shell visual pertenece al componente shared (`AppEditorPdf`),
no al contenedor de integracion del modulo.

### Decision 3: Limpieza de reglas CSS obsoletas

**Decision:** eliminar reglas duplicadas o huérfanas asociadas al shell previo
(`editorSurface`, duplicados de `height/display`).

**Rationale:** Reduce deuda visual, evita overrides contradictorios y mejora la
trazabilidad del layout actual.

## Risks & Mitigations

- [Riesgo] Regresion de layout vertical en el panel principal.
  - Mitigacion: mantener `embeddedAppEditor` con `flex: 1 1 auto` y `min-height: 0`.
- [Riesgo] Pruebas frágiles por acoplamiento a CSS modules.
  - Mitigacion: validar contrato via atributo semantico `data-editor-shell`.

## Implementation Plan

1. Crear artefactos OpenSpec (`design/spec/tasks`) para `app-appeditorpdf-05-fe`.
2. Ajustar `GestionRespuestaEditorContainer` a contrato neutro full-surface.
3. Limpiar estilos de `GestionRespuestaMainTabContent.module.css`.
4. Actualizar pruebas del tab para contrato `05-FE`.
5. Ejecutar suite focal de pruebas del modulo y `AppEditorPdf`.
