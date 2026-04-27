# Proposal: scrumcore-150

## Why
Hoy existen estados de carga inline implementados por pantalla con temporización local (delay), lo que genera inconsistencias visuales, duplicación de lógica y parpadeo cuando las cargas son rápidas. Necesitamos un componente shared único que estandarice el patrón de “loading inline con delay” y su accesibilidad.

## What Changes
- Se crea un componente shared `AppLoadingState` (inline/card) con soporte de `delayMs`, accesible y reutilizable.
- Se migra el estado `loading` del panel master-detail de Gestión Correspondencia (`GestionCorrespondenciaRoute`) para usar `AppLoadingState` y eliminar lógica ad-hoc (`setTimeout`/`clearTimeout` en vistas).
- Se agregan pruebas unitarias e integración UI para validar el delay, limpieza de timers y render dentro de contenedores/paneles.

## Capabilities
### New Capabilities
- `app-loading-state`: Componente shared de estado de carga inline con delay, accesibilidad uniforme y API estable para consumidores.

### Modified Capabilities
- `crea-bloqueo-por-estructura-gestion-respuesta-id-tarea-wf`: Ajuste de la experiencia de carga en el panel de detalle (loading) para estandarizar el loader (sin cambiar reglas de bloqueo/ready).

## Impact
- Código afectado (previsto):
  - `src/app/Components/UI/AppLoadingState/**` (nuevo)
  - `src/modules/gestionCorrespondencia/routes/GestionCorrespondenciaRoute.tsx` (migración de loader)
- Testing:
  - Nuevos tests del componente shared y actualización/validación de tests del route.
- UX:
  - Menos flicker en cargas rápidas (delay encapsulado).
  - Consistencia visual y accesible para estados de carga inline (no overlay global).

