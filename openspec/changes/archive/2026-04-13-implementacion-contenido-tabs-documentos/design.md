## Context

El tab **Documentos** en GestionRespuesta requiere una estructura visual tipo workbench
que estandarice la experiencia con el resto del modulo. Existe el componente compartido
`AppCollapseRail` y `AppToolbar`, pero falta su integracion en el tab **Documentos**.
La implementacion debe ser solo visual, sin logica de negocio y respetando el contrato
existente de `GestionRespuesta.tsx` y los tabs.

## Goals / Non-Goals

**Goals:**
- Definir el acomodo de componentes para el workbench del tab **Documentos**.
- Establecer comportamiento responsive (desktop/tablet/mobile) del panel lateral.
- Mantener contenido del panel montado al colapsar.
- Garantizar accesibilidad basica en toggles y rail.

**Non-Goals:**
- Integrar APIs, servicios o logica de negocio.
- Modificar `AppTabs`, `AppToolbar` o `AppCollapseRail`.
- Reestructurar el tab **Gestion** o romper el contrato del layout.

## Decisions

- Ubicar el workbench en `src/modules/gestionCorrespondencia/components/documentosWorkbench/`
  para mantener separacion con `gestionRespuestaMainTab` sin cambiar el modulo.
- Usar `AppCollapseRail` controlado por estado local (`collapsed`) con props exactas:
  `title`, `collapsed`, `onToggle`, `placement="right"`, `variant` responsive.
- Mantener el contenido del panel renderizado en DOM aun cuando esta colapsado,
  garantizando persistencia de estado visual.
- Implementar scroll independiente en area principal y panel lateral mediante CSS Modules.
- Aplicar `variant="overlay"` en mobile para el panel tipo bottom-sheet con rail visible.

## Risks / Trade-offs

- [Riesgo] Divergencia visual con el workbench del tab Gestion.
  -> Mitigacion: usar referencias visuales existentes en `gestionRespuestaMainTab`.
- [Riesgo] Inconsistencias en breakpoints.
  -> Mitigacion: centralizar reglas responsive en el CSS Module del workbench.
- [Riesgo] Regresiones de accesibilidad por falta de ARIA.
  -> Mitigacion: asegurar `aria-expanded`, `aria-controls` y foco visible.
