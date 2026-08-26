# Flujo y seguridad

## Preview

1. ASMX reconstruye contexto Workflow autenticado y verifica el permiso específico.
   La consulta de tarea acepta al usuario de la sesión o un snapshot activo explícitamente recuperable (`ESTADO_RECUPERACION_FLUJO_TRABAJO = 1`), para soportar la opción Recuperar tarea desde otra actividad.
2. El repositorio lee la tarea activa y accesible y, mediante `SELECT` parametrizados, obtiene el snapshot de usuario real más reciente con `id_Estado` menor al estado activo, ordenado por `id_Estado DESC LIMIT 1`; los registros grupales quedan fuera de la selección.
3. Si no existe usuario histórico, o no es elegible y consistente, devuelve el bloqueo correspondiente sin escribir estado, tarea, auditoría, evento ni dato de negocio.
4. El servidor emite el token opaco que vincula tarea, estado actual y `id_Estado` del antecedente exacto.

## Ejecución

1. Valida forma y vencimiento del token y adquiere el lock exclusivo de la tarea.
2. Dentro del lock relee contexto, permiso, tarea activa, ambos snapshots, elegibilidad y Ruta/Flujo. En Flujo exige coincidencia de flujo y actividad de flujo; preserva `ID_USUARIO_WORKFLOW_FLUJO_TRABAJO` histórico cuando sea positivo, incluso si es distinto del usuario destino, y solo lo completa con este último cuando sea cero.
3. Exige que el token coincida con el estado actual y con el antecedente exacto. Un cambio bloquea; nunca se elige otro historial.
4. Bloquea si el usuario histórico es igual al usuario Workflow autenticado real.
5. El adaptador ejecuta una sola vez el motor. La auditoría se intenta después de una transición; su falla se convierte en advertencia y no deshace un éxito ya confirmado.

## Parámetros aprobados del único punto mutante

El adaptador exclusivo pasará al motor los datos reconstruidos del historial, no valores del navegador:

| Parámetro `Terminar_Tarea_Workflow` | Valor decidido |
| --- | --- |
| `Id_Usuario_Destino`, `Id_Actividad_Destino` | Usuario y actividad del antecedente revalidado. |
| `id_Tarea` | Tarea bloqueada y revalidada. |
| Ruta/Flujo | Identificadores consistentes del antecedente; cero solo cuando el contexto Ruta lo requiere. |
| `pag` | `Nothing`. |
| `notifica`, `notifica_envio_correo` | `0`; DOC-36 no envía notificación. |
| `id_conector` | `0`; no es una transición por conector. |
| Usuario y actividad que envían | Contexto Workflow autenticado y actividad actual revalidada, para trazabilidad del motor. |
| `activa_actualizacion_paramtros_interface` | `0`. |
| `activa_evento_dinamicos` | `0`. |
| `activa_reasigna_sii`, `activa_reasigna_tarea_workflow` | `0`. |

La desactivación de notificación y eventos es deliberada: evita que esta capacidad dispare flujos dinámicos o mecanismos de respuestas internos. Si una necesidad futura exige alguno, debe aprobarse una nueva política y prueba antes de activarlo.

## Auditoría

La auditoría funcional usa la acción `ASMX_DEVOLVER_USUARIO_ANTERIOR`, tarea, usuario autenticado, resultado, código público, duración y referencia opaca. No registra el token ni datos sensibles.
