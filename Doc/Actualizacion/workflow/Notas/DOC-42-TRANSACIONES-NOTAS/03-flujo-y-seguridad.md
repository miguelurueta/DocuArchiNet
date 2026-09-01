# Flujo y seguridad

1. El servicio comprueba contexto Workflow, permiso y tarea operativa.
2. El repositorio ejecuta preflight no destructivo de motores, columnas, índices, idempotencia y ledger.
3. Crear reserva `(tarea, usuario, clientRequestId)` con unicidad, inserta la nota, registra su versión, audita y confirma en una transacción.
4. Actualizar une `ANOTACION_TAREA` con `workflow_notas_version` y condiciona tarea, propietario, actividad, estado y versión esperada.
5. Eliminar borra físicamente ambas filas en una sentencia condicionada y registra auditoría en la misma transacción.

Un conflicto no devuelve contenido ni versión vigente. Los fallos de auditoría revierten la mutación. Las notas históricas sin fila de ledger permanecen legibles, pero se bloquean para escritura hasta un backfill separado, revisado y autorizado.

La migración es manual y reversible por esquema; no se ejecuta desde el endpoint ni desde las pruebas.
