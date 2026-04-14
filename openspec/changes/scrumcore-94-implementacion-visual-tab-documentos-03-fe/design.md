## Context

El ticket SCRUMCORE-94 busca continuar la implementacion visual del tab **Documentos**
para GestionCorrespondencia. Se parte de la arquitectura existente del modulo y de
los componentes shared (`AppToolbar`, `AppCollapseRail`, `AppTabs`) para construir un
workbench consistente, responsive y accesible, sin logica de negocio.

## Goals / Non-Goals

**Goals:**
- Consolidar la experiencia visual del tab **Documentos** con layout tipo workbench.
- Asegurar comportamiento responsive (desktop/tablet/mobile) y panel colapsable.
- Mantener el contrato de tabs de `GestionRespuesta.tsx` y consistencia visual.

**Non-Goals:**
- Integrar APIs, servicios o logica de dominio.
- Modificar componentes shared o la arquitectura del modulo.
- Cambiar el comportamiento del tab **Gestion**.

## Decisions

- Reutilizar el esquema de workbench con `AppToolbar` y `AppCollapseRail`.
- Mantener componentes desacoplados y estilos locales (CSS Modules).
- Controlar el estado colapsado desde el contenedor del tab **Documentos**.
- Mantener contenido del panel montado aun cuando se colapsa.

## Risks / Trade-offs

- [Riesgo] Desalineacion visual con el resto del modulo.
  -> Mitigacion: usar referencias de `gestionRespuestaMainTab` y design system.
- [Riesgo] Inconsistencias responsive entre breakpoints.
  -> Mitigacion: centralizar reglas en el CSS Module del workbench.
- [Riesgo] Accesibilidad incompleta en toggles.
  -> Mitigacion: mantener ARIA y foco visible en el rail y header.
