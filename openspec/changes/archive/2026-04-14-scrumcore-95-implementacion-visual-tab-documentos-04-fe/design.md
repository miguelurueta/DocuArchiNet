## Context

El ticket SCRUMCORE-95 continua la implementacion visual del tab **Documentos**
para GestionCorrespondencia. Se parte del workbench definido previamente y se
busca reforzar consistencia visual, responsive y accesibilidad, sin logica de
negocio ni cambios en el tab **Gestion**.

## Goals / Non-Goals

**Goals:**
- Consolidar la experiencia visual del tab **Documentos** con layout tipo workbench.
- Mantener comportamiento responsive (desktop/tablet/mobile) del panel colapsable.
- Preservar el contrato de tabs de `GestionRespuesta.tsx`.

**Non-Goals:**
- Integrar APIs, servicios o logica de dominio.
- Modificar componentes shared (`AppTabs`, `AppToolbar`, `AppCollapseRail`).
- Alterar el tab **Gestion** o el routing del modulo.

## Decisions

- Reutilizar la estructura del workbench con `AppToolbar` y `AppCollapseRail`.
- Mantener componentes desacoplados y estilos locales via CSS Modules.
- Controlar el estado `collapsed` desde el contenedor del tab **Documentos**.
- Mantener contenido del panel montado aun cuando se colapsa.

## Risks / Trade-offs

- [Riesgo] Desalineacion visual con otros tabs.
  -> Mitigacion: usar referencias del workbench existente en `gestionRespuestaMainTab`.
- [Riesgo] Inconsistencias en breakpoints.
  -> Mitigacion: centralizar reglas responsive en el CSS Module del workbench.
- [Riesgo] Accesibilidad incompleta en toggles.
  -> Mitigacion: mantener atributos ARIA y foco visible.
