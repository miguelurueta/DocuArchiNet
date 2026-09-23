# Gate y alternancia

## Regla de autorización

La importación moderna está disponible únicamente si:

1. `WorkflowCentroTrabajoModernActive=true`.
2. La sesión Workflow produce un contexto válido.
3. El login está en `WorkflowCentroTrabajoModernUsers` o el grupo está en `WorkflowCentroTrabajoModernGroups`.

Una audiencia vacía cierra el feature. El ASMX valida la misma regla que la página; ocultar la UI no concede acceso backend.

## Estados

| Estado | UI moderna | UI legacy | ASMX moderno |
| --- | --- | --- | --- |
| Gate apagado | No se registra | Disponible | `FEATURE_DISABLED` |
| Gate activo, fuera de audiencia | No se registra | Disponible | Rechazo seguro |
| Gate activo, audiencia autorizada | Una entrada/handler | Oculta, no eliminada | Disponible con contexto válido |

No pueden coexistir dos handlers efectivos para iniciar la misma importación.
