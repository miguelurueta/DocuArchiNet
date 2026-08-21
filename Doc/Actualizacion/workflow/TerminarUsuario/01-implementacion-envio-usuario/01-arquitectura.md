# Arquitectura y componentes

`Enviar a usuario` es una operación directa a un usuario y actividad de la ruta. No es una transición por conector y, por tanto, no extiende Continuar flujo.

- Ticket: DOC-28
- Cambio OpenSpec: doc-28-backend-enviar-usuario-workflow
- Clasificación: cross_cutting

| Capa | Componentes implementados o reutilizados | Responsabilidad |
| --- | --- | --- |
| Presentation | Sin entrega en DOC-28. La UI corresponde a la etapa 02. | El navegador no autoriza el destino ni recibe lista global de usuarios. |
| ASMX | `WebServiceWorkflowModern`, `WorkflowPreviewSessionContextGate` | Reconstruye contexto autenticado, calcula permiso y compone dependencias específicas. |
| Application | `ServicioEnvioUsuarioTarea`, `ValidadorEnvioUsuarioTarea` | Normaliza la solicitud, coordina preview, lock, relectura, requisitos, resultado y auditoría. |
| Domain | Modelos, DTOs, códigos y puertos de Enviar a usuario | Define el destino directo usuario–actividad, sin `Page`, `Session` ni `IdConector`. |
| Infrastructure | `MySqlEnvioUsuarioRepository`, guard y adaptadores de usuario | Ejecuta solo lecturas de destinos y concentra la autorización, requisitos y única mutación legacy. |

El destino real es `(IdUsuarioWorkflowDestino, IdActividadDestino)`. `MySqlEnvioUsuarioRepository` lo resuelve contra la ruta de la tarea, usuario activo y `UTIL_ASIGNA_TAREA=1`; `ServicioEnvioUsuarioTarea` lo vuelve a validar dentro del lock antes de llamar al motor.

Continuar flujo conserva `PreviewEnviarTarea`, `EjecutarEnvioTarea`, `SolicitudTransicionWorkflow`, `IdConector`, sus DTOs y su adaptador existentes.

## Diagramas de arquitectura

- [Componentes y fronteras](Diagramas/03-arquitectura-componentes.md): separa la frontera ASMX, aplicación, contratos, infraestructura y motor legacy.
- [Validación y ejecución](Diagramas/04-validacion-y-ejecucion.md): muestra la secuencia de reautorización y el único punto mutante.
- [Secuencia del envío](Diagramas/01-secuencia-envio-usuario.md): muestra preview, selección y confirmación como contratos de servidor.
- [Alcance sin gate y relevo](Diagramas/02-alcance-y-relevo.md): delimita explícitamente lo que DOC-28 no modifica y la responsabilidad de la etapa 02.
