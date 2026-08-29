# BACKEND-CONTRATOS-NOTAS

- Ticket: DOC-40
- Cambio OpenSpec: doc-40-backend-contratos-notas
- Clasificacion: cross_cutting (Transversal)

## Servicios y reglas

El modelo futuro se compone de un gate de contexto de Notas, un servicio de aplicación, DTOs y modelos de Workflow, un puerto de tarea y un repositorio parametrizado. El gate será el único componente de esta cadena que conozca la sesión; servicio, modelos y repositorios no conocerán `Page`, `GridView`, `UpdatePanel` ni `HttpContext`.

Cada contrato exige `idTarea`; consultar, actualizar y eliminar exigen además `idNota`. El gate determina identidad, grupo, permiso e `IdRutaWorkflow` en servidor y falla cerrado. El puerto de tarea valida acceso, estado e `IdRuta` usando el patrón de `ITareaWorkflowRepository.ObtenerTarea(contexto, idTarea)`. Una ruta ausente, inválida o incoherente bloquea la operación. Los resultados admitidos son `Forbidden`, `TaskNotActive`, `NoteNotFound`, `NotOwner`, `VersionConflict`, `InvalidContent` y `Unavailable`, sin exponer SQL ni excepciones.

La ruta es un dato de negocio, no una ruta HTTP. Ninguna solicitud de Notas puede suministrar nombre de ruta, tabla o campo dinámico. Si un repositorio requiere metadatos, consulta `rutas_workflow` con parámetros y emplea solo identificadores validados en servidor; el repositorio de Notas conserva la ruta en el snapshot para autorización y trazabilidad.

El código crea `ServicioNotasWorkflow`, `INotasWorkflowRepository` y `MySqlNotasWorkflowRepository`. El repositorio responde `Unavailable` en sus seis operaciones y no contiene SQL ni acceso a conexión; por tanto la fundación no puede leer ni escribir datos hasta una autorización posterior. El servicio rechaza texto vacío, NUL o superior a 16.000 unidades UTF-16. Histórico se aprobó como lectura para todo usuario Workflow autorizado a consultar la tarea histórica, sin mutación.

## Política resuelta para la siguiente fase

| Área | Regla | Evidencia y efecto técnico |
|---|---|---|
| Borrado | Físico, condicionado y auditado. | El legacy ya ejecuta `DELETE`; la nota eliminada no se lista ni se consulta, y queda solo auditoría de metadatos. |
| Mutación | Solo propietario; no hay excepción inicial de supervisor/admin. | No existe regla legacy que otorgue esa excepción. Crear, editar y eliminar requieren permiso, tarea operable y actor propietario donde aplique. |
| Histórico | Lectura para cualquier usuario Workflow autorizado a consultar esa tarea. | No depende de ser trabajador actual; nunca habilita crear, editar ni eliminar. |
| Contenido | Texto plano, no vacío, sin NUL, máximo 16.000 UTF-16 y Unicode BMP. | MySQL 5.1 `utf8` usa hasta tres bytes; el máximo consume hasta 48.000 de los 65.535 bytes de `TEXT` y el servicio rechaza pares sustitutos. |
| Retención | Hereda tarea/documento padre; no hay calendario propio de notas. | Evita crear una serie documental o una copia de contenido en el log. |
| Versión | ETag SHA-256 opaco generado por servidor desde valores persistidos. | Detecta cambios hechos por rutas legacy mientras coexisten consumidores; la mutación lo compara en su condición atómica. |
| Auditoría | Metadatos, longitudes y SHA-256, sin contenido completo. | El JSON ASCII cabe en `datos_operacion latin1` sin copiar texto Unicode a un log de propósito general. |
| Idempotencia | UUID único por tarea y autor, retenido 30 días. | Una tabla InnoDB guarda nota y resultado original; el reintento no duplica nota ni auditoría. |
| Rendimiento | Página inicial 50, máximo 100, `COUNT(*)` y refresco por evento/cambio de tarea. | El mayor esquema tiene 17.048 notas; se elimina el sondeo legacy de 600 ms. |

## Preflight obligatorio antes de escribir

Los metadatos leídos el 2026-08-28 muestran siete `ANOTACION_TAREA` MyISAM con `Dato_Anotacion TEXT utf8`, clave compuesta `(Id_Anotacion, Inicio_Tareas_Workflow_id_Tarea)` e índice individual por tarea. Solo tres esquemas tienen `wf_log_workflow`, que es InnoDB, con `datos_operacion LONGTEXT latin1` y sin índice por tarea/fecha. Esa mezcla impide prometer una transacción nota-auditoría.

La fase de escritura debe ejecutar, con autorización independiente y por cada esquema objetivo, una migración revisable que convierta la tabla de notas a InnoDB y conserve `Dato_Anotacion TEXT utf8`, compatible con MySQL 5.1; agregue índices `(tarea, estado, fecha, nota)` y `(tarea, fecha, nota)`; asegure una auditoría InnoDB e índice `(tarea, fecha, log)`; y cree la tabla InnoDB de idempotencia con unicidad `(tarea, autor, clientRequestId)`. Un preflight de solo lectura verifica esas condiciones antes de abrir transacción. Si una falla, el servicio devuelve `Unavailable`, sin crear auditoría parcial ni tocar la nota. `utf8mb4` requiere MySQL 5.5.3 o posterior; mientras el motor sea 5.1, el servicio rechaza pares sustitutos.
