## Context

SCRUMCORE-91 corresponde a la fase 02 del componente `AppCollapseRail`. La fase 01
ya introdujo el componente base con colapso, rail y responsive. Esta fase profundiza
en refinamientos visuales, accesibilidad avanzada y consistencia con el panel de
herramientas de GestionRespuesta, sin modificar su API publica.

## Goals / Non-Goals

**Goals:**
- Refinar estilos para alineacion completa con GestionRespuesta.
- Mejorar accesibilidad (focus visible y labels consistentes).
- Consolidar responsive en desktop/tablet/mobile con variantes.
- Agregar pruebas de UI/UX adicionales si aplican.

**Non-Goals:**
- Reescribir API del componente.
- Persistencia de estado en backend.
- Integraciones con modulos consumidores.

## Decisions

1. **Mantener API existente**
   - Decision: mantener props actuales y evitar cambios breaking.
   - Razon: fase 01 ya expuso el contrato, evitar re-trabajo.
   - Alternativas: redefinir props (rechazada por riesgo).

2. **Refinar estilos con tokens locales**
   - Decision: ajustes en CSS Modules con tokens y breakpoints claros.
   - Razon: consistencia visual sin afectar otros componentes.
   - Alternativas: estilos globales (rechazada).

3. **Accesibilidad consistente**
   - Decision: estandarizar labels de toggle y rail.
   - Razon: coherencia con AppButton y lectores de pantalla.
   - Alternativas: dejar labels implicitos (rechazada).

## Risks / Trade-offs

- [Cambios visuales pueden alterar layouts existentes] -> Mitigacion: mantener
  tamaños compatibles y revisar en breakpoints.
- [Overlays en mobile] -> Mitigacion: limitar altura y asegurar rail visible.

## Migration Plan

- No requiere migracion.
- Aplicar ajustes incrementales en el componente existente.

## Open Questions

- Confirmar si se requiere soporte para `placement="left"` en overlay mobile.
