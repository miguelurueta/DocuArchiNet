<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08 -->
# Diseño — Fundación de backend y contratos de Notas de Workflow

## Contexto

La inspección estática de `workflow/Class_anotacion_tarea.vb`, `webservice/WebServiceWorkflow.asmx.vb`, `webservice/WorkflowPreviewSessionContextGate.vb`, `Modelo/Workflow/Terminar/WorkflowModernInterfaces.vb` e `Infrastructure/Repositories/Workflow/MySqlWorkflowPreviewRepositories.vb` muestra dos referencias reutilizables: el gate resuelve el contexto autenticado en servidor y el puerto de tarea recibe `contexto` e `idTarea` explícitos. También confirma que la ruta legacy toma la tarea de `Session("ID_TAREA_SELECCIONDA")` para crear, actualizar y eliminar notas; dicha fuente queda excluida de todo contrato moderno.

En este diseño, “ruta” significa la ruta de negocio de Workflow, no una URL. `ContextoModuloWorkflow` contiene `IdRutaWorkflow` y `TareaWorkflow` contiene el `IdRuta` obtenido desde el estado actual de la tarea. El patrón existente resuelve metadatos con `rutas_workflow` y parámetros, y solo acepta nombres de ruta seguros antes de formar identificadores técnicos derivados.

DOC-40 crea la línea arquitectónica y sus contratos internos, no un recorrido expuesto. El transporte ASMX queda para una fase posterior; el gate de contexto, servicio de aplicación, puerto de tarea y repositorio inactivo ya respetan el límite de capas. Solo un transporte futuro y el gate podrán conocer sesión o `HttpContext`.

## Objetivos

- Aislar el dominio de Notas dentro de Workflow sin alterar consumidores ni contratos legacy.
- Hacer explícito el recurso objetivo en cada operación mediante `idTarea`; para una nota concreta, exigir además `idNota`.
- Hacer que la autorización y el estado de tarea se resuelvan y validen en servidor antes de ejecutar una operación de dominio.
- Proveer resultados funcionales seguros y estables para las siguientes fases.

## Decisiones de arquitectura

| ID | Decisión | Justificación y aplicación posterior |
| --- | --- | --- |
| D-01 | DOC-40 se limita a fundación interna de Notas en Workflow. | No se modifica UI, consumidor, feature gate, endpoint publicado, esquema ni datos; esto preserva los recorridos legacy. |
| D-02 | La tarea objetivo es un argumento `idTarea` de cada contrato moderno. | `ITareaWorkflowRepository.ObtenerTarea(contexto, idTarea)` demuestra el patrón existente para resolver una tarea del actor sin compartir la selección mutable de sesión. |
| D-03 | El gate de Notas deriva identidad, grupo y permiso en servidor y falla cerrado. | `WorkflowPreviewSessionContextGate` es la referencia de composición; ningún DTO acepta actor, autor, grupo ni permiso desde el navegador. |
| D-04 | El dominio usa DTOs, modelos y resultados funcionales tipados. | Las salidas se normalizan a `Forbidden`, `TaskNotActive`, `NoteNotFound`, `NotOwner`, `VersionConflict`, `InvalidContent` o `Unavailable`, sin SQL ni excepciones de infraestructura. |
| D-05 | El acceso a datos moderno es un repositorio Workflow parametrizado y aislado de la clase legacy. | Se reutiliza el estilo de `MySqlWorkflowPreviewRepositoryBase`, `IModuleConnectionFactory` e `IDataExecutor`; no se adapta `Class_anotacion_tarea`. |
| D-06 | La política de escritura queda definida, pero su ejecución y migración siguen fuera de DOC-40. | Borrado físico con auditoría, solo propietario, histórico de solo lectura autorizado, contenido de máximo 16.000 unidades UTF-16, auditoría por huella y longitud, e idempotencia de 30 días. Cada esquema requiere preflight de motor, charset, auditoría, índices y tabla de idempotencia antes de una mutación. |
| D-07 | La verificación de DOC-40 es local y no productiva. | Cuando se autorice código se agregan pruebas unitarias de gate, contratos y rechazos; no procede E2E, base real ni activación mientras no haya endpoint o recorrido visible. |
| D-08 | La ruta Workflow forma parte del contexto autorizado y del snapshot de tarea. | El servidor obtiene `IdRutaWorkflow` y `IdRuta`; el cliente no entrega ruta, nombre de tabla ni metadatos. Si una operación necesita metadatos, se usa `rutas_workflow` parametrizada y se rechaza una ruta inválida o incoherente. |

## Contratos internos propuestos

La fase autorizada incorporó modelos y DTOs en las ubicaciones coherentes de Workflow, sin alterar el alcance de estas firmas:

| Operación | Solicitud mínima | Resultado mínimo |
| --- | --- | --- |
| Listar | `idTarea`, cursor y tamaño controlados | Colección visible, cursor siguiente y código funcional. |
| Contar | `idTarea` | Cantidad visible o código funcional. |
| Crear | `idTarea`, contenido, identificador de intención | Nota creada o resultado funcional. |
| Consultar | `idTarea`, `idNota` | Contenido y metadatos permitidos o código funcional. |
| Actualizar | `idTarea`, `idNota`, contenido, versión | Nota actualizada, conflicto o rechazo funcional. |
| Eliminar | `idTarea`, `idNota`, versión | Confirmación funcional, conflicto o rechazo funcional. |

Ninguna firma toma la tarea objetivo desde sesión. La versión, el cursor y el identificador de intención se mantienen en el diseño sin implementar aún una mutación. La versión será un ETag SHA-256 opaco calculado en servidor a partir de los valores persistidos de la nota; se valida junto con tarea, propietario y estado dentro de la sentencia mutante. Esta decisión evita añadir una columna que las rutas legacy no actualizarían mientras coexistan consumidores.

## Política y preflight de la primera escritura

La inspección de metadatos de siete esquemas Workflow confirma `ANOTACION_TAREA` en MyISAM, contenido `TEXT utf8` y un índice individual por tarea. Solo tres de esos esquemas exponen `wf_log_workflow`, que es InnoDB y conserva `datos_operacion` en `latin1`. Una transacción de nota MyISAM y auditoría InnoDB no es atómica; por ello la fase de escritura debe quedar bloqueada hasta que, para cada esquema que vaya a activarse, una migración aprobada y revisable cumpla:

1. Convertir `ANOTACION_TAREA` a InnoDB y conservar `Dato_Anotacion` como `TEXT CHARACTER SET utf8`, compatible con MySQL 5.1. Las inserciones modernas asignan servidor, actividad y autor no nulos aunque el legado tolere nulos. El servicio acepta solo Unicode BMP y rechaza pares sustitutos; `utf8mb4` requiere MySQL 5.5.3 o posterior y queda para un cambio de plataforma separado.
2. Conservar la clave primaria `(Id_Anotacion, Inicio_Tareas_Workflow_id_Tarea)` y agregar índices portables `(Inicio_Tareas_Workflow_id_Tarea, Estado_Tarea, Fecha_Anotacion, Id_Anotacion)` para operación y `(Inicio_Tareas_Workflow_id_Tarea, Fecha_Anotacion, Id_Anotacion)` para histórico. El índice individual existente se conserva hasta comprobar planes y referencias.
3. Garantizar `wf_log_workflow` InnoDB. Si el esquema no lo posee, la migración crea su equivalente compatible antes de habilitar mutaciones; agrega el índice `(ID_TAREA_WORKFLOW, fecha_hora, id_log_workflow)` para trazabilidad por tarea.
4. Crear una tabla InnoDB de idempotencia con unicidad de `(idTarea, idAutorWorkflow, clientRequestId)`, referencia a `idNota`, resultado original y expiración a 30 días. El valor es un UUID opaco validado por servidor; un reintento antes de expirar devuelve el resultado original y no crea una segunda nota ni una segunda auditoría.
5. Ejecutar un preflight no mutante de motor, charset, índices y tablas. Cualquier desviación devuelve `Unavailable`; no se habilita una escritura parcial. La reversa del consumidor puede volver a legacy; una futura adopción de `utf8mb4` no pertenece a esta migración MySQL 5.1.

La eliminación moderna será física, igual que la operación legacy, y deja únicamente una auditoría de metadatos. Una nota eliminada no aparece en lectura operativa ni histórica y no se recupera su contenido desde la API. La auditoría registra actor, tarea, nota, ruta, actividad, operación, fecha de servidor, correlación, resultado, versión anterior/resultante, longitudes y huellas SHA-256; `datos_operacion` almacena JSON ASCII de esos metadatos, nunca texto completo de la nota. La clasificación y retención del contenido heredan la tarea o documento padre, sin calendario autónomo de Notas.

## Patrón de rutas de Workflow

1. El gate obtiene `IdRutaWorkflow` desde la sesión autenticada y confirma que la ruta existe antes de completar el contexto de servidor.
2. El puerto de tarea obtiene el `IdRuta` real desde `estados_tarea_workflow`; una solicitud solo continúa cuando su tarea y ruta son válidas para el contexto autenticado.
3. Un repositorio que requiera metadatos de ruta los consulta con `@idRuta` en `rutas_workflow`. No acepta de la solicitud el nombre de ruta, nombres de campo ni tablas.
4. Si el patrón requiere un identificador técnico derivado —por ejemplo, una tabla `dat_adic_tar` asociada a una ruta— este procede exclusivamente de metadatos confiables y pasa la validación de identificador seguro. Los valores de contenido e identificadores de negocio siguen siendo parámetros ADO.NET.
5. El repositorio de Notas no debe introducir una dependencia de ruta dinámica si `ANOTACION_TAREA` no la necesita; conserva la ruta dentro del snapshot para autorización, trazabilidad y coherencia de la tarea.

## Límites de capas

```text
ASMX futuro
  -> Gate de contexto de Notas
  -> Servicio de aplicación de Notas
  -> ITareaWorkflowRepository + repositorio de Notas
  -> MySQL parametrizado
```

El gate obtiene identidad, grupo, permiso y conexiones desde la sesión del servidor. El servicio recibe un contexto ya validado y solicitudes tipadas. Los repositorios reciben contexto y parámetros, y no conocen controles WebForms ni la sesión. La persistencia posterior debe usar parámetros y condiciones atómicas de nota, tarea, actor, estado y versión cuando una decisión de escritura sea aprobada.

## Compatibilidad, seguridad y reversa

Los endpoints y clientes legacy no cambian en DOC-40. Si una implementación futura se revierte, el consumidor de Workflow se mantiene en la ruta legacy sin doble escritura porque DOC-40 todavía no registra ni publica operaciones. El contenido de notas se tratará como texto y los resultados expuestos en etapas posteriores evitarán detalles técnicos.

## Verificación prevista

Las pruebas locales cubren estáticamente contratos, permiso de Notas, tarea explícita, coherencia de ruta, aislamiento de capas y repositorio fail-closed. La compilación local confirma que los nuevos archivos pertenecen al proyecto. No hay E2E aplicable en esta fundación sin endpoint ni cambio de usuario.

## Entrada de la fase de lectura

La primera exposición queda diseñada por `Doc/Actualizacion/workflow/Notas/Prompt/02-lectura-listado-y-contador.md`. Esa fase reutilizará los contratos y el gate de DOC-40 para publicar solo `Listar`, `Consultar` y `Contar`; crear, actualizar y eliminar continuarán sin exposición. El endpoint, si la fase lo requiere, se ubicará en `webservice/` y no trasladará lógica a WebForms.

La E2E pertenece al mismo cambio que exponga esa lectura, conforme a `bloque-e2e-integrado-en-modernizacion.md`: rechazo anónimo, lectura operativa e histórica autorizada, aislamiento entre tareas y ausencia de mutación. Requiere ambiente, cuentas y tarea descartable autorizados, reutiliza exclusivamente `tools/e2e` y no se crea ni ejecuta dentro de DOC-40. El histórico aplica la política aprobada de acceso autorizado a la tarea y permanece sin mutaciones.
