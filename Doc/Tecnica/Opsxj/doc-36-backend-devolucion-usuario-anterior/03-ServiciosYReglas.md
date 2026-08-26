# BACKEND-DEVOLUCION-USUARIO-ANTERIOR

- Ticket: DOC-36
- Cambio OpenSpec: doc-36-backend-devolucion-usuario-anterior
- Clasificacion: cross_cutting (Transversal)
## Servicios y reglas

| Componente | Responsabilidad |
| --- | --- |
| `ServicioDevolverUsuarioAnterior` | Preview de solo lectura, revalidación dentro del lock, resultado saneado y auditoría posterior. |
| `MySqlDevolverUsuarioAnteriorRepository` | Lee tarea activa y dos snapshots por `id_Estado DESC`; valida elegibilidad y Ruta/Flujo con `SELECT` parametrizados. |
| `DevolverUsuarioAnteriorTokenCodec` | Protege tarea, estado actual, estado histórico, contexto y vencimiento de cinco minutos con `MachineKey`. |
| `MySqlDevolverUsuarioAnteriorConcurrencyGuard` | Usa `GET_LOCK` exclusivo por tarea, independiente del token. |
| Adaptadores `WorkflowLegacyDevolverUsuarioAnterior*` | Concentración del único punto mutante y auditoría `ASMX_DEVOLVER_USUARIO_ANTERIOR`. |

Reglas: no hay fallback a actividad anterior; usuario ausente, grupo, retiro, inconsistencia, auto-devolución, token inválido o lock ocupado bloquean antes del motor. El adaptador usa `Page = Nothing`, correo, interfaz legacy, eventos y reasignaciones en `0`; no trata respuestas.
