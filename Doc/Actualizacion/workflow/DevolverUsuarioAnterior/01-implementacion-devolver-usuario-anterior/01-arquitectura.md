# Arquitectura y decisiones aprobadas

## Capacidad aislada

`Devolver a usuario anterior` se implementa como una capacidad distinta de `Devolver a actividad anterior`, `Enviar a usuario`, `Enviar a grupo` y `Continuar flujo`. Sus contratos no contienen `IdConector`, usuario destino, actividad destino, grupo, Ruta, Flujo ni historial enviados por el navegador.

| Capa | Responsabilidad DOC-36 |
| --- | --- |
| ASMX | Reconstruye el contexto autenticado y publica preview y ejecución con sesión. |
| Application | Resuelve preview, valida el token, adquiere lock, relee el historial y normaliza resultado. |
| Domain | Define modelos, puertos, token, códigos y reglas exclusivas de usuario anterior. |
| Infrastructure | Ejecuta `SELECT` parametrizados, autorización específica, `GET_LOCK`, adaptador del motor y auditoría. |
| Motor legacy | Solo recibe la mutación final desde el adaptador exclusivo. |

## Componentes DOC-36

- `WebServiceWorkflowModern.asmx.vb` y `WorkflowPreviewSessionContextGate.vb`: endpoints autenticados y permiso específico fail-closed.
- `DTOs/Workflow/DevolverUsuarioAnterior/` y `Modelo/Workflow/DevolverUsuarioAnterior/`: contratos exclusivos.
- `ServicioDevolverUsuarioAnterior`: preview, revalidación dentro del lock, resultado y auditoría posterior.
- `MySqlDevolverUsuarioAnteriorRepository`, token codec y guard: historial de solo lectura, token de cinco minutos y exclusión por tarea.
- Adaptadores exclusivos de motor y auditoría: único punto mutante, sin UI, correo, eventos dinámicos ni componentes de respuestas.

## Decisiones de diseño

1. **Último usuario histórico.** El estado actual se revalida y el repositorio busca el registro anterior más reciente con `Id_Usuario > 0`, `id_Estado < id_EstadoActual` y `ORDER BY id_Estado DESC LIMIT 1`. Así los snapshots grupales no se convierten en destino ni obligan al cliente a indicar un usuario. No se usa fecha para ordenar.
2. **Elegibilidad.** El antecedente debe tener `Id_Usuario > 0`, usuario Workflow existente y habilitado, actividad válida, y atributos de Ruta/Flujo consistentes con el snapshot activo. En un flujo, `ID_USUARIO_WORKFLOW_FLUJO_TRABAJO` positivo se preserva aunque sea distinto de `Id_Usuario`; solo se completa con el usuario histórico cuando viene en cero, como hace el motor legado. Una fila grupal, retirada o inconsistente bloquea; jamás se convierte en destino alternativo.
3. **Token.** El servidor protege con `MachineKey` un sobre exclusivo de la operación que contiene `IdTarea`, `IdEstadoActual`, `IdEstadoHistorico` y vencimiento corto de cinco minutos. El cliente recibe el valor opaco como `TokenVersion`; el identificador histórico nunca se expone por separado.
4. **Exclusión.** La ejecución adquiere `GET_LOCK('workflow-return-user-' + IdTarea, 0)` y lo libera sobre la misma conexión en `Finally`. El lock no depende del token; por ello serializa intentos con tokens distintos.
5. **Autorización, recuperación y auto-devolución.** El permiso específico se recalcula en servidor a partir del contexto autenticado y de la autorización existente de devolver tareas. Una tarea cuyo snapshot activo tenga `ESTADO_RECUPERACION_FLUJO_TRABAJO = 1` puede ser retomada por el usuario autenticado aunque el `ID_USUARIO` de ese snapshot sea otro; una tarea no marcada sigue exigiendo coincidencia de usuario. El antecedente se compara con `IdUsuarioWorkflow` autenticado, nunca con `Id_Ruta_Workflow` ni con un valor del navegador.
6. **Punto mutante único.** Solo el adaptador exclusivo invoca `Terminar_Tarea_Workflow`; no se invoca `Devolver_tarea_workflow_usuario_anterior`, Web Forms, postbacks ni controles de página.
7. **Política sin respuestas.** La transición no solicita correo ni eventos dinámicos. Los componentes nuevos no construyen ni invocan componentes de respuestas; las pruebas deben impedir referencias a esos tipos y demostrar los parámetros inhibidores del motor.

DOC-36 no cambia UI ni el registro de presentación. Esa sustitución del postback legado corresponde a la etapa posterior.
