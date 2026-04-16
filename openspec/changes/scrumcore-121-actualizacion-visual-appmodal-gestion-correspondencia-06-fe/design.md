## Context

`SCRUMCORE-121` implementa un modal reusable de gestión de documento dentro del
tab **Gestion** de `GestionRespuesta`, disparado desde el botón
`Solicitud de Aprobacion` ya existente en el `AppToolbar` del primer tab.

La referencia arquitectónica del cambio está documentada en:

- `docs/Architecture/ImplementacionVisualGestionCorrespo/06-FE-Modal-Gestion-Documento-Solicitud-Aprobacion.md`

Actualmente el botón existe dentro de
`src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaMainTabContent.tsx`,
pero no abre ninguna superficie adicional. El objetivo de esta FE es introducir
un `AppModal` controlado con una composición moderna tipo formulario/workbench,
utilizando exclusivamente componentes shared ya disponibles.

## Goals / Non-Goals

**Goals:**
- Abrir un `AppModal` desde el botón `Solicitud de Aprobacion`.
- Encapsular el contenido del modal en
  `gestionRespuestaMainTab/modalGestionDocumento/`.
- Construir una UI moderna con `infoBox`, `formGrid` y `actions`.
- Usar exclusivamente `AppModal`, `AppInputSelect`, `AppInput` y `AppInputTags`
  para el contenido del formulario.
- Mantener el modal controlado mediante `open` y `onClose`.
- Implementar solo UI y estado local del formulario.

**Non-Goals:**
- Integrar servicios, backend o submit real.
- Introducir lógica de negocio del proceso de aprobación.
- Reemplazar el `AppToolbar` o alterar el flujo del resto del workbench.
- Crear nuevos componentes shared para resolver esta FE.

## Decisions

- El trigger seguirá viviendo en `GestionRespuestaMainTabContent.tsx`.
- El contenido del modal se implementará en:
  `src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/modalGestionDocumento/`.
- El modal usará `AppModal` como shell principal, no `Modal` directo de Ant Design.
- El formulario interno manejará solo estado local para:
  - select principal
  - checkbox
  - tags
- La composición visual seguirá esta jerarquía:
  - `infoBox`
  - `formGrid`
  - `actions`
- Las acciones del modal serán `Cancelar` y `Guardar`, con alineación a la derecha.
- CSS Modules será obligatorio para encapsular el layout del modal.

## Risks / Trade-offs

- [Riesgo] El modal puede terminar acoplado a lógica de negocio del workflow.
  -> Mitigacion: limitar la implementación a UI y estado local, sin servicios ni
  handlers de dominio.

- [Riesgo] El contenido del modal puede degradarse visualmente en mobile si el
  grid no colapsa bien.
  -> Mitigacion: diseñar `formGrid` con stack claro en breakpoints pequeños.

- [Riesgo] Rehacer controles con estilos custom puede romper consistencia con el
  Design System.
  -> Mitigacion: usar exclusivamente componentes shared ya existentes y CSS
  Modules solo para layout y composición.

- [Riesgo] El trigger del botón puede quedar mezclado con lógica de presentación
  del modal dentro del mismo archivo.
  -> Mitigacion: mantener en el contenedor solo el estado `isModalOpen` y la
  apertura/cierre, delegando el contenido a `GestionDocumentoModal`.
