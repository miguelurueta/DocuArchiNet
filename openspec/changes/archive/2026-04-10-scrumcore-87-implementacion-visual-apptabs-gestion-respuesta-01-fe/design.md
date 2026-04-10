## Context

`GestionRespuesta.tsx` necesita reemplazar su contenido actual por un layout basado en `AppTabs`, manteniendo el `AppButton` de "Volver a la bandeja" visible y fuera del contenido de tabs. La arquitectura base de `AppTabs` ya existe en `docs/Architecture/AppTabs/AppTabs-Architecture.md` y el contexto especifico para esta vista esta documentado en `docs/Architecture/ImplementacionVisualGestionCorrespo/GestionRespuesta-AppTabs-Architecture.md`.

El cambio es visual/estructural dentro del modulo `gestionCorrespondencia` y no debe introducir logica de negocio ni cambios de routing.

## Goals / Non-Goals

**Goals:**
- Reemplazar el contenido de `GestionRespuesta.tsx` por `AppTabs` como contenedor principal.
- Mantener el `AppButton` de "Volver a la bandeja" siempre visible y fuera de los `children` de tabs.
- Preservar estilos locales del modulo y el comportamiento existente.
- Garantizar layout responsive con tabs y boton visibles en desktop y mobile.

**Non-Goals:**
- Modificar el componente `AppTabs` o su contrato.
- Crear o cambiar endpoints/API.
- Refactor del router o del layout global.
- Introducir dependencias nuevas.

## Decisions

- **AppTabs como layout principal en la vista**: Se adopta `AppTabs` para dividir la UI en secciones, reutilizando el wrapper enterprise ya definido para mantener consistencia visual.
- **Boton fuera de tabs**: El `AppButton` de "Volver a la bandeja" se mantiene en un wrapper superior a `AppTabs` para evitar que cambie al navegar entre tabs y preservar el comportamiento actual.
- **Estilos locales**: Cualquier ajuste visual se hace en CSS Modules del modulo `gestionCorrespondencia` para evitar impactos globales.
- **Items definidos en la vista**: La lista de `AppTabItem[]` se define en `GestionRespuesta.tsx` para mantener separacion de responsabilidades y control de estado en el contenedor.

## Risks / Trade-offs

- [Riesgo] Tabs con keys duplicadas o estructura incompleta -> Mitigacion: definir `AppTabItem[]` con keys unicas y validar render en tests.
- [Riesgo] El boton se vuelva parte del contenido del tab -> Mitigacion: mantener estructura con wrapper fijo fuera de `AppTabs`.
- [Riesgo] Ajustes de estilos rompan layout en mobile -> Mitigacion: limitar cambios a CSS Modules del modulo y validar responsive.
- [Trade-off] Cambio visual puede requerir ajustes menores de spacing -> Mitigacion: revisar solo estilos locales necesarios.
