<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06 -->
# Diseño: lectura, listado y contador seguros de Notas Workflow

## Contexto

DOC-40 dejó los contratos de Notas y un repositorio que falla cerrado. DOC-41 habilita el primer tramo de uso: exclusivamente lectura operativa. El código legacy en `workflow/Class_anotacion_tarea.vb` se usa solo como evidencia de esquema y comportamiento anterior; no se reutiliza como adaptador ni se modifica.

## Objetivos y límites

- Devolver notas, contenido y conteo solo para una tarea autorizada y activa.
- Aislar cada consulta por tarea, usuario, grupo y ruta de Workflow.
- Mantener el orden estable, la paginación acotada y las consultas parametrizadas.
- Conservar sin cambios las mutaciones, las páginas `.aspx`, los endpoints legacy y los gates.

## Decisiones

### D-01 — Límite de transporte de solo lectura

`webservice/WebServiceWorkflowNotesModern.asmx.vb` contendrá los tres métodos de lectura y mapeará únicamente DTOs de Notas a contratos de aplicación. Cada método recibe `idTarea`; no recibe identidad, permisos, grupo, ruta, tabla ni tarea seleccionada desde el navegador. No se agregan métodos de escritura en este cambio.

### D-02 — Autorización y tarea desde el servidor

Cada método invoca `WorkflowPreviewSessionContextGate.AsegurarContextoNotas`. El servicio mantiene `ServicioNotasWorkflow.PrepararOperacion`, que valida permiso, tarea activa y coincidencia de ruta mediante `MySqlTareaWorkflowRepository.ObtenerTarea`. La sesión solo aporta el contexto autenticado; `ID_TAREA_SELECCIONDA` no interviene en el camino moderno.

### D-03 — Cursor y orden deterministas

El contrato de esta fase no expone un orden alternativo: usa `FECHA_ANOTACION DESC, ID_ANOTACION DESC`. Se agrega un codec de cursor de Notas basado en el patrón `MachineKey` de `DevolverActividadCursorCodec`; protege tarea, `TokenVersion`, usuario, grupo, ruta, orden y la clave del último registro. El servicio normaliza el tamaño a 25 y rechaza tamaños superiores a 50. Un cursor inválido se resuelve como respuesta segura sin datos.

### D-04 — Persistencia parametrizada y consistente

`MySqlNotasWorkflowRepository` implementa `Listar`, `Consultar` y `Contar` usando `IDataExecutor` y `MySqlParameter`. El listado proyecta únicamente los campos del contrato, solicita una fila adicional para calcular `TieneMas` y no interpola filtros ni orden. El contador usa `COUNT(*)`, y la consulta de contenido exige `ID_ANOTACION` e `INICIO_TAREAS_WORKFLOW_ID_TAREA` en la misma sentencia.

### D-05 — Visibilidad operativa y no filtración

Las tres consultas aplican la misma visibilidad operativa de `ANOTACION_TAREA.ESTADO_TAREA = 1`. No se crea ningún modo histórico. El contenido que no pertenezca a la tarea, una tarea no disponible y un cursor cruzado no devuelven filas, conteos, cursores ni metadatos de otro contexto.

### D-06 — Compatibilidad, evidencia y operación segura

No se modifica `Class_anotacion_tarea`, los ASMX legacy ni `WorkflowCentroTrabajoModernActive`. Las pruebas de servicio y repositorio usan fakes de puertos y ejecutores, sin base de datos. La E2E se integra exclusivamente en `tools/e2e`; toda corrida autenticada depende de autorización expresa y deja los gates apagados al terminar. Precarga únicamente raíz local de Gestión, módulo y ambiente no sensibles. Sus huellas de estado y auditoría reutilizan el DSN ODBC no sensible `workflowconta` y capturan únicamente usuario y contraseña MySQL de solo lectura mediante TTY; no aceptan una URL o cadena de conexión MySQL. El arnés registra los `SELECT` de metadatos de `ANOTACION_TAREA` y de `wf_log_workflow`, ambos ligados a la tarea y sin proyectar contenido de notas ni `datos_operacion`.

## Flujo de lectura

```text
ASMX moderno -> gate de sesión -> ServicioNotasWorkflow -> tarea autorizada
      -> validación de cursor/tamaño -> repositorio MySQL parametrizado -> DTO seguro
```

## Componentes afectados

- `Modelo/Workflow/Notas/`: contratos de cursor y resultados de lectura.
- `Services/Workflow/Notas/ServicioNotasWorkflow.vb`: normalización de página, cursor y respuestas seguras.
- `Infrastructure/Repositories/Workflow/MySqlNotasWorkflowRepository.vb`: listado, contenido y contador.
- `Infrastructure/Workflow/Notas/`: codec de cursor protegido.
- `webservice/WebServiceWorkflowNotesModern.asmx.vb`: transporte de las tres lecturas.
- `tests/` y `tools/e2e/`: pruebas locales y política de validación autorizada.
- `Doc/Actualizacion/workflow/Notas/`: contrato, límites, riesgos y matriz de pruebas.

## Verificación y reversión

Las pruebas locales verifican que los puertos se bloqueen antes de la lectura cuando falla permiso o tarea, que el cursor no cruce contexto y que el contador use la misma visibilidad que el listado. La reversión consiste en retirar los nuevos métodos modernos sin tocar los consumidores ni el código legacy, que permanece operativo e independiente.
