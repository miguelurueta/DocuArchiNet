# Arquitectura

## Decisiones

La UI usa una colección común: el recorrido individual contiene una fila y el múltiple la selección explícita. El cliente de intención reutiliza `ImportarServicioWebApi`; no crea transporte, almacenamiento ni ejecución. El backend conserva autoridad sobre catálogo, requisitos, huella y plan.

## Responsabilidades

- `requirements`: máquina pura, completitud y fallo cerrado.
- `preparation`: normalización y tipología limitada al catálogo.
- `intent-client`: preflight, deduplicación y creación idempotente.
- `ui`: foco, renderizado, resumen de efectos previstos y eventos explícitos.
- Backend: autorización, resolución TRD, configuración B11, huella e intención.

Se descartaron caminos separados por cardinalidad, preflight implícito en **Guardar todas**, fabricación cliente del plan y llamadas a mutadores legacy.
