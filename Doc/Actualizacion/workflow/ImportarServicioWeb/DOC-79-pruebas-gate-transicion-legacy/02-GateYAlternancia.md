# Gate y alternancia

- Ticket: DOC-79
- Cambio OpenSpec: doc-79-pruebas-retiro-gate
- Clasificacion: cross_cutting

## Regla de autorización

La importación moderna está disponible únicamente si:

1. `WorkflowCentroTrabajoModernActive=true`.
2. La sesión Workflow produce un contexto válido.

No se aplican listas adicionales de usuarios o grupos: la funcionalidad es transversal para todos los usuarios que ya tienen acceso autenticado al módulo. El ASMX valida la misma bandera y sesión que la página; ocultar la UI no concede acceso backend.

La configuración versionada activa el gate y selecciona `INTEGRACIONSII` como proveedor, por lo que la interfaz queda visible y operativa sin una habilitación manual por cuenta.

## Estados

| Estado | UI moderna | UI legacy | ASMX moderno |
| --- | --- | --- | --- |
| Gate apagado | No se registra | Disponible | `FEATURE_DISABLED` |
| Gate activo, sesión inválida | No se registra | Disponible | Rechazo seguro |
| Gate activo, sesión válida | Una entrada/handler | Oculta, no eliminada | Disponible |

No pueden coexistir dos handlers efectivos para iniciar la misma importación.
