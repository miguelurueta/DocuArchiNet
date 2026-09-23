# Gate y alternancia

- Ticket: DOC-79
- Cambio OpenSpec: doc-79-pruebas-retiro-gate
- Clasificacion: cross_cutting

## Regla de autorización

La importación moderna está disponible únicamente si:

1. `WorkflowCentroTrabajoModernActive=true`.
2. La sesión Workflow produce un contexto válido.

No se aplican listas adicionales de usuarios o grupos: la funcionalidad es transversal para todos los usuarios que ya tienen acceso autenticado al módulo. El ASMX valida la misma bandera y sesión que la página; ocultar la UI no concede acceso backend.

Después de la certificación E2E, el responsable autorizó la activación versionada del gate con el proveedor `INTEGRACIONSII`. La interfaz queda disponible para toda sesión Workflow válida, sin habilitación manual por cuenta ni listas de audiencia. Las corridas E2E conservan su obligación independiente de partir y terminar con el gate apagado.

`QueryItems` resuelve el código de barras en servidor a partir de la tarea validada. El navegador no necesita publicarlo ni puede sustituirlo por otro valor. Si la tarea no tiene código de barras, la UI presenta el código seguro `SERVER_BARCODE_UNAVAILABLE` como error y no como un resultado vacío.

## Estados

| Estado | UI moderna | UI legacy | ASMX moderno |
| --- | --- | --- | --- |
| Gate apagado | No se registra | Disponible | `FEATURE_DISABLED` |
| Gate activo, sesión inválida | No se registra | Disponible | Rechazo seguro |
| Gate activo, sesión válida | Una entrada/handler | Oculta, no eliminada | Disponible |

No pueden coexistir dos handlers efectivos para iniciar la misma importación.
