# Arquitectura y componentes

`Enviar a grupo` es una operación directa a una actividad de ruta, independiente de la transición por conector de Continuar flujo.

- Ticket: DOC-15
- Cambio OpenSpec: doc-15-base-enviar-grupo
- Clasificacion: cross_cutting

| Capa | Componentes implementados | Responsabilidad |
| --- | --- | --- |
| Presentation | `Webworkflow.aspx`, `Webworkflow.aspx.vb`, `workflow-group-send-ui.js`, `workflow-group-send-confirmation.js` | Modal accesible, confirmación, payload directo y fallback legacy. |
| ASMX | `WebServiceWorkflowModern.asmx.vb` | Contexto autenticado, composición de dependencias y respuesta pública. |
| Application | `ServicioEnvioGrupoTarea`, `ValidadorEnvioGrupoTarea` | Validación, gate, lock, relectura, requisitos, resultado y auditoría. |
| Domain | Modelos, DTOs e interfaces de grupo | Contratos sin `Page`, `Session` ni `IdConector`. |
| Infrastructure | `MySqlEnvioGrupoRepository`, adaptadores legacy | SELECT de destinos, aprobaciones y única frontera con el motor legacy. |

Continuar flujo conserva `PreviewEnviarTarea`, `EjecutarEnvioTarea`, `IdConector`, sus DTOs y adaptador existentes.

## Diagramas de arquitectura

- [Componentes y fronteras](Diagramas/03-arquitectura-componentes.md): separa la UI, ASMX, aplicación, dominio, infraestructura y el motor legacy.
- [Validación y ejecución](Diagramas/04-validacion-y-ejecucion.md): muestra los controles previos y la única ruta mutante.
- [Secuencia de grupo](Diagramas/01-secuencia-envio-grupo.md) y [gate con fallback](Diagramas/02-fallback-gate.md): muestran la interacción del usuario y el mecanismo de reversión.
