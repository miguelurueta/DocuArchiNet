# SCRUMCORE-XX — Integración UI: `AppLoadingState`

## Objetivo
Definir cómo se adopta `AppLoadingState` en pantallas existentes sin romper el comportamiento actual.

## Estrategia de migración (progresiva)
1. Introducir el componente shared sin modificar consumidores.
2. Migrar pantallas que hoy tienen loader con delay local:
   - reemplazar lógica local por `AppLoadingState`.
3. Mantener compatibilidad visual durante la migración.

## Guía de uso
- Usar `AppLoadingState` en regiones/paneles (master-detail, cards, secciones).
- No usar `AppLoadingState` como bloqueador global full-screen.
- Para operaciones globales usar `OperationBlockerContext`.

## N/A (si no aplica en el ticket)
Si este ticket solo crea el componente base sin integrarlo en una pantalla, esta sección queda como guía y la integración se hace en tickets posteriores.

