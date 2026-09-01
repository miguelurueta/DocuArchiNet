<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07 -->
# Diseño técnico — DOC-42 Transacciones de Notas

## Contexto actual

`WebServiceWorkflowNotesModern.asmx.vb` publica sólo `ListarNotas`, `ConsultarNota` y `ContarNotas`. `ServicioNotasWorkflow.vb` ya valida contenido BMP de hasta 16.000 unidades UTF-16, versión, contexto, ruta y tarea explícita. `MySqlNotasWorkflowRepository.vb` implementa las lecturas, mientras `Crear`, `Actualizar` y `Eliminar` retornan `Unavailable`. Los DTOs, modelos e interfaces ya reservan las tres solicitudes de escritura.

Las decisiones de negocio provienen de DP-01, DP-03, DP-04, DP-05 y DP-07 de `Doc/Actualizacion/workflow/Notas/Exploracion/modelo-requerimientos-modernizacion-notas-workflow.md`.

## Flujo de la operación

1. El ASMX recibe sólo campos del contrato y obtiene el contexto mediante `WorkflowPreviewSessionContextGate`.
2. `ServicioNotasWorkflow` valida forma, contenido, versión cuando corresponde, permiso y tarea explícita; no consulta una tarea desde una selección mutable de sesión.
3. El repositorio ejecuta el preflight del esquema y abre una transacción sólo si todas las precondiciones son verdaderas.
4. La mutación condicionada comprueba atributos persistidos de autorización y estado; no se basa en una lectura de propietario separada.
5. La misma transacción registra dominio, idempotencia cuando aplica y auditoría de metadatos. Un error revierte toda la unidad.
6. El servicio mapea el resultado a un DTO seguro, sin serializar excepciones, SQL, contenido de auditoría ni datos de infraestructura.

## Decisions

### D-01 — Frontera moderna aislada

Se añaden métodos al ASMX `WebServiceWorkflowNotesModern` y se reutilizan los DTOs, modelos, `ServicioNotasWorkflow` e `INotasWorkflowRepository`. No hay cambio en WebForms, flags, consumidores ni `Class_anotacion_tarea`. Toda solicitud contiene `idTarea`; la sesión sólo entrega identidad y contexto de servidor.

### D-02 — Contexto y autorización atómicos

El servicio conserva su validación fail-closed de contexto, ruta y tarea. El repositorio recibe sólo contexto autorizado y tarea validada. La escritura efectiva condiciona en la propia unidad de persistencia nota, `idTarea`, actor, estado aplicable y, para editar/eliminar, versión esperada. Si una condición no coincide, no hay modificación ni auditoría de éxito. El mapeo devuelve únicamente los códigos funcionales seguros permitidos, sin revelar contenido ni existencia.

### D-03 — Idempotencia de creación

`IdSolicitudCliente` se valida como UUID no vacío. La persistencia de idempotencia tendrá una clave única por `idTarea`, autor e identificador de solicitud, y conservará durante 30 días resultado original, referencia de nota y versión. La reserva/lectura de esa clave, la inserción de nota y auditoría ocurren en la misma transacción InnoDB. Un duplicado recupera respuesta original; no crea ni audita una segunda nota.

### D-04 — Versión, actualización y eliminación

La versión expuesta es un ETag SHA-256 calculado por .NET sobre una representación canónica de los valores persistidos que identifican nota, estado y SHA-256 de contenido. `workflow_notas_version` conserva la versión vigente en InnoDB, separada de la respuesta original de `workflow_notas_idempotencia`; no se invoca `SHA2()` de MySQL. Actualizar y eliminar unen nota y libro de versiones en una sola sentencia condicionada por tarea, autor, estado, versión y tarea operativa. Eliminar es físico para ambas filas; no hay borrado lógico, recuperación ni exposición posterior del contenido. Un conflicto no incluye versión ni contenido actuales.

### D-05 — Unidad transaccional y auditoría privada

El repositorio abre conexión, comandos, lectores y transacción con `Using` y manejo determinista. Crear, actualizar y eliminar incluyen cambio de dominio y auditoría en la misma transacción. La auditoría registra actor autenticado, tarea y actividad autorizadas, nota, operación, fecha de servidor, correlación, resultado, versión, longitud y SHA-256; nunca texto completo ni contenido de cliente como supuesto valor anterior. Cualquier error hace rollback y devuelve resultado seguro.

### D-06 — Preflight y migración controlada

Antes de la primera escritura por esquema, el repositorio exige preflight para `ANOTACION_TAREA` InnoDB, `Dato_Anotacion TEXT utf8`, auditoría `wf_log_workflow` InnoDB, índices de operación/histórico, almacén InnoDB de idempotencia y `workflow_notas_version` InnoDB con su índice de tarea/autor. MySQL 5.1 no admite `utf8mb4` y algunas instalaciones no habilitan `SHA2()` SQL, por lo que el servicio mantiene rechazo BMP y calcula SHA-256 sólo en .NET. La implementación contiene verificación no destructiva y migración con rollback, pero una inspección `SELECT` y autorización específica son requisito para aplicarla a un ambiente.

### D-07 — Pruebas y E2E integrada

Se agregan dobles de servicio/repositorio para reintento, doble pestaña, nota/tarea cruzada, cambio de estado, conflicto, error de auditoría y rollback. `tools/e2e/tests/notes-workflow.spec.cjs` y runners existentes son la única base E2E. No se ejecutan con tareas o ambiente de lectura ya autorizados: se requiere autorización nueva de cuenta y tarea descartable, controles sólo `SELECT`, evidencia saneada y gate apagado al finalizar.

## Riesgos y compatibilidad

- La base de lectura de DOC-41 sigue compatible; añadir mutaciones no altera sus métodos ni cursores.
- El preflight bloquea esquemas MyISAM; no se degrada a una escritura no transaccional.
- La migración por esquema es dependencia de despliegue, no una acción implícita del endpoint.
