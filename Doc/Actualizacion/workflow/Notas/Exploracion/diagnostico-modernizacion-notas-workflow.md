# Exploración arquitectónica — Modernización de notas de Workflow

**Fecha:** 2026-08-22  
**Alcance:** revisión estática del código fuente. No se modificó código, configuración, datos ni se ejecutaron pruebas autenticadas/E2E.

## Conclusión ejecutiva

La modernización es técnicamente viable y recomendable, pero no debe limitarse a una capa visual. Notas de Workflow es un dominio pequeño y aislable, con deuda crítica de seguridad, autorización, integridad y mantenibilidad.

| Aspecto | Diagnóstico |
|---|---|
| Viabilidad técnica | Alta: hay un modelo de datos y operaciones CRUD acotadas, y el proyecto ya incorpora un patrón moderno reutilizable. |
| Riesgo actual | Alto: SQL concatenado, autorización situada solo en la interfaz, exposición por identificadores y auditoría que puede desalinearse de la tarea real. |
| Enfoque recomendado | Sustitución gradual del backend detrás de contratos nuevos; WebForms queda como adaptador temporal. |
| Condición para modernizar UI | Antes debe existir un backend que valide permiso, acceso a tarea, pertenencia de nota y concurrencia en el servidor. |

## Mapa actual

La misma lógica de notas es consumida por cuatro superficies:

```text
Centro de Trabajo Workflow ─┐
Radicación Entrante ────────┼─ JavaScript duplicado ──► WebServiceWorkflow.asmx
Gestión de Correspondencia ─┤                                  │
Consulta histórica ─────────┘                                  ▼
                                                   Class_anotacion_tarea
                                                           │          │
                                                           ▼          ▼
                                                ANOTACION_TAREA  wf_log_workflow
```

### Componentes identificados

- `workflow/Webworkflow.aspx` contiene el acceso directo a Notas y los modales principales.
- `workflow/Class_anotacion_tarea.vb` concentra consultas, creación, edición, borrado y auditoría.
- `webservice/WebServiceWorkflow.asmx.vb` expone listar, crear, actualizar, borrar, recuperar contenido y contar notas.
- `js/workflow/Webworkflow.js` realiza el CRUD AJAX y actualiza la grilla en navegador.
- `radicador` y `Gestion_correspondencia` duplican los clientes AJAX para el mismo ASMX.
- `workflow/WebFormConsultaTareasWorkflow` consume un listado de notas en modo histórico.
- `workflow/WebFormAnotacion.aspx` parece ser una pantalla aislada sin consumidores; además, su guardar y actualizar están comentados.

La conclusión es que no conviene modernizar una sola pantalla de forma independiente: se debe extraer el backend compartido y luego migrar cada consumidor.

## Reglas y modelo de dominio que deben preservarse

La tabla `ANOTACION_TAREA` representa una nota asociada a una tarea, actividad, usuario creador, fecha y estado. La aplicación registra operaciones en `wf_log_workflow`.

La política legacy observable es:

1. El permiso de interacción con anotaciones viene de `MatriPermisos(9)` y se guarda como `Session("Interactuar_Anotaciones")`.
2. La creación asocia la nota a la tarea seleccionada, el usuario de sesión y la actividad resuelta para el grupo actual.
3. La actualización y el borrado pretenden permitir operación solo al propietario de la nota.
4. La lista operativa muestra únicamente notas con `ESTADO_TAREA = 1`.
5. La eliminación actual es física aunque exista la columna de estado.

Estas reglas deben convertirse en políticas explícitas de servidor. No deben seguir dependiendo de que el navegador o un postback haya abierto previamente un modal.

## Hallazgos priorizados

### P0 — SQL concatenado

El texto de la nota se concatena en sentencias `INSERT`, `UPDATE`, `DELETE` y en el log de auditoría. Esto permite que una comilla rompa la operación y abre una superficie de inyección SQL para un actor autenticado.

También el cliente construye manualmente JSON concatenando el contenido de la nota; comillas, saltos de línea o caracteres de escape pueden invalidar la solicitud antes de que llegue al servidor.

**Corrección requerida:** consultas parametrizadas de extremo a extremo, DTOs tipados y `JSON.stringify` en los clientes temporales.

### P0 — Autorización únicamente en la interfaz

`Webworkflow.aspx.vb` bloquea la apertura del modal cuando `Interactuar_Anotaciones` vale `0`. Sin embargo, los endpoints ASMX de notas no validan ese permiso. Una llamada AJAX directa evita el chequeo visual.

En Gestión de Correspondencia el chequeo de permiso además está comentado.

**Corrección requerida:** resolver el permiso en el endpoint a partir de la sesión autenticada y fallar cerrado. No aceptar identidad, permisos ni tarea confiando en valores del navegador.

### P0 — Acceso por identificador sin comprobación de tarea

- El endpoint de lista recibe desde el cliente un identificador de tarea y no demuestra que esa tarea sea visible para el actor.
- El endpoint de contenido recibe únicamente `ID_ANOTACION`.
- La comprobación de propiedad de actualizar/borrar usa `ID_ANOTACION + ID_USUARIO`, no la tarea.

Esto permite revelar una nota por identificador o modificar una nota de otra tarea del mismo propietario. En el último caso, el log usa la tarea de sesión y puede quedar auditado contra una tarea equivocada.

**Corrección requerida:** toda mutación y lectura debe recibir `idTarea` e `idNota`, y validar en una misma consulta que:

```text
nota pertenece a tarea
AND tarea es accesible/activa para el contexto actual
AND actor tiene permiso de notas
AND (para editar/borrar) actor es propietario, mientras negocio no defina otra regla
```

### P1 — XSS almacenado

El cliente agrega y actualiza el contenido de la nota usando `innerHTML`. Un texto persistido como HTML puede ejecutarse al renderizar la grilla.

**Corrección requerida:** tratar las notas como texto plano, codificar la salida en servidor y usar `textContent` en JavaScript. Si negocio requiere formato enriquecido, debe usarse una lista de etiquetas permitidas y un sanitizador de servidor.

### P1 — Auditoría y borrado no son consistentes

La eliminación es física, y el log almacena operación y valor entregado por cliente. No hay registro explícito del valor anterior en una actualización, ni garantía de que tarea, nota y actor correspondan entre sí.

La existencia de `ESTADO_TAREA` sugiere una posible intención de borrado lógico, pero el endpoint histórico no filtra ese estado. Cambiar el borrado a lógico sin actualizar todos los lectores podría hacer reaparecer notas eliminadas.

**Corrección requerida:** mantener inicialmente la semántica actual de borrado mientras se documenta el comportamiento real. Luego decidir si se adopta baja lógica y actualizar todos los lectores de manera atómica.

### P1 — Sin concurrencia ni límites de contenido

Actualizar no usa versión, fecha ni valor previo en la condición `WHERE`; dos usuarios o dos pestañas del propietario pueden sobrescribir cambios sin aviso. Tampoco se observó validación de longitud en el contrato.

**Corrección requerida:** agregar `version` o token de concurrencia y rechazar una actualización desactualizada; definir máximo de longitud, obligatoriedad y normalización Unicode.

### P2 — Rendimiento del contador

El Centro de Trabajo ejecuta `alarma_nota_actividad()` cada 600 ms. Cada llamada consulta todas las columnas y materializa todas las notas activas para contar filas.

Con 100 sesiones activas equivale aproximadamente a 167 solicitudes por segundo, antes de otras actualizaciones de la bandeja.

**Corrección requerida:** actualizar el contador después de crear/editar/borrar y al cambiar de tarea. Si se necesita refresco periódico, usar un intervalo razonable y solo con una tarea seleccionada; la consulta debe ser `COUNT(*)` parametrizado.

### P2 — Duplicación y deuda de presentación

El CRUD JavaScript está duplicado en Workflow, Radicación y Gestión de Correspondencia. Cambios de contrato implicarían tres modificaciones equivalentes. La antigua `WebFormAnotacion` agrega un cuarto artefacto que no parece participar en el flujo vivo.

**Corrección requerida:** crear un único cliente de notas reutilizable o integrar los consumidores contra el mismo contrato moderno. Retirar la pantalla antigua solo después de comprobar referencias y regresión.

## Hallazgos adicionales — revisión quirúrgica del backend

La revisión inicial identificó los riesgos de autorización, SQL, concurrencia y auditoría. La inspección detallada del recorrido de escritura descubre además deuda de ciclo de vida, consistencia transaccional y contexto compartido. Estos puntos son condición de salida para el backend moderno; no deben trasladarse como compatibilidad del adaptador legacy.

### P0 — Transacciones y conexiones con salida anticipada

`Eliminar_nota_service_workflow` y `Actualizar_datos_anotacion` abren conexión y transacción antes de validar propietario y antes de formatear la fecha. Varias ramas hacen `Exit Function` sin `Rollback`, `Close` ni `Dispose`.

Esto puede dejar recursos abiertos o transacciones abandonadas ante errores funcionales. El problema no se resuelve solamente con una transacción de auditoría: el ciclo de vida completo debe ser seguro aun cuando la validación falle.

**Corrección requerida:** usar bloques `Using` para conexión, comando y lector; encapsular la transacción con `Try/Catch/Finally`; hacer rollback ante cualquier excepción o resultado no exitoso. Las validaciones sintácticas se ejecutan antes de abrir la transacción y las validaciones que dependen de datos se hacen dentro de la unidad de trabajo cuando condicionan la escritura.

### P0 — Tarea seleccionada compartida por sesión

Los endpoints legacy de crear, editar y borrar obtienen la tarea desde `Session("ID_TAREA_SELECCIONDA")`. Una sesión ASP.NET se comparte entre pestañas del mismo navegador. Si el usuario abre dos tareas, la última selección puede reemplazar ese valor para ambas pestañas.

El riesgo es distinto del conflicto de versión: una nota puede crearse o auditarse contra una tarea distinta de la que el usuario veía. En edición o borrado, el identificador de nota procede de una pestaña mientras la tarea usada para auditoría procede de otra.

**Corrección requerida:** cada contrato moderno recibe `idTarea` explícito. La sesión aporta exclusivamente identidad y contexto base, nunca la tarea objetivo. Antes de cada lectura o mutación se vuelve a resolver la tarea solicitada para el actor y su estado actual.

### P0 — Autorización separada de la escritura (TOCTOU)

La validación de propietario se realiza en una consulta separada. Después, la actualización legacy filtra por `ID_ANOTACION` sin incluir tarea, actor ni versión; el borrado tampoco liga todas las dimensiones de autorización.

Entre la comprobación y la escritura existe una ventana temporal, y la condición de persistencia no garantiza por sí misma que la nota, tarea y actor sean los ya autorizados.

**Corrección requerida:** realizar actualización o eliminación con predicados atómicos de `idNota`, `idTarea`, `idAutor`, estado y `version`. Para actualizar, el patrón objetivo es equivalente a:

```text
UPDATE ...
WHERE idNota = @idNota
  AND idTarea = @idTarea
  AND idAutor = @actor
  AND version = @versionEsperada
  AND estado = @estadoVisible
```

La auditoría debe leer el valor necesario y persistirse en la misma transacción. Las respuestas distinguen de forma segura entre conflicto, tarea no operable, no autorizado y nota inexistente, sin revelar información ajena.

### P1 — Actividad de origen no ligada al snapshot de tarea

La creación legacy resuelve actividad desde el grupo de sesión y la combina con la tarea seleccionada. No comprueba que la actividad resuelta corresponda al estado actual de la tarea.

**Corrección requerida:** el servicio moderno obtiene tarea, ruta, actividad vigente y estado desde el repositorio de acceso autorizado. La actividad de la nota se deriva de ese snapshot; nunca de un valor del navegador ni de un grupo de sesión aislado.

### P1 — Semántica desigual y no determinista en el listado histórico

El listado histórico consulta por tarea sin `ORDER BY` y sin aplicar el filtro de estado que utiliza el listado operativo. Por ello el orden puede variar y una futura baja lógica podría reaparecer en una superficie histórica sin decisión explícita.

**Corrección requerida:** definir un orden estable, como `fechaCreacion DESC, idNota DESC`, y una política explícita de visibilidad de estados para cada modo: operativo e histórico. El contrato debe indicar cuándo puede solicitarse contenido eliminado o archivado y con qué permiso.

### P1 — Operación duplicada de borrado

Existen dos rutinas legacy de eliminación. La segunda no aparece como consumidor activo y contiene una construcción de SQL de auditoría inconsistente, además de devolver el resultado a una variable local en vez del nombre de la función.

**Corrección requerida:** no migrar ni adaptar ambas variantes. Inventariar referencias, mantener un único camino moderno y retirar la rutina sin referencias como parte del retiro controlado, con prueba de regresión de todos los consumidores.

### P1 — Reintentos que pueden duplicar creación

Crear una nota no tiene una clave de idempotencia. Un doble clic, una reconexión o un reintento del cliente después de perder la respuesta puede generar más de una nota válida.

**Corrección requerida:** incluir un `clientRequestId` opaco en la creación, generado una vez por intención del usuario. Persistirlo o registrarlo con unicidad por tarea y autor, y devolver el resultado original ante un reintento. Debe incluirse en la correlación de auditoría y métricas.

### P2 — Cursor, orden e índices requieren contrato de integridad

La paginación propuesta no debe aceptar cursores opacos sin contexto ni interpolar el campo de orden. Un cursor reutilizado para otra tarea o usuario podría causar errores de aislamiento; una orden dinámica sin lista permitida reintroduciría riesgo de SQL.

**Corrección requerida:** el cursor se protege y liga al menos a tarea, actor/contexto, filtros y sentido de orden. El orden se selecciona por lista blanca. Antes de liberar, validar el esquema con consultas de solo lectura y preparar una migración revisable de índices que cubra listados por tarea/estado/fecha y mutaciones por nota/tarea/autor/versión.

### P2 — Límites de capa y resultados funcionales

La clase legacy acopla acceso a datos, `HttpContext`, controles WebForms y generación de presentación. También retorna mensajes técnicos directamente en algunos caminos.

**Corrección requerida:** el backend moderno se divide en transporte, gate de contexto, servicio de aplicación y repositorios. Solo la capa de transporte conoce ASMX y sesión; repositorios y modelos no conocen `Page`, `GridView`, `UpdatePanel` ni `HttpContext`. Los casos de salida son códigos funcionales estables (`Forbidden`, `TaskNotActive`, `NoteNotFound`, `NotOwner`, `VersionConflict`, `InvalidContent`, `Unavailable`).

## Arquitectura objetivo

Se recomienda aplicar el patrón ya existente para transiciones modernas: contexto de sesión validado, contratos que no dependen de `Page` ni `Session`, repositorios parametrizados y errores funcionales seguros.

```text
WebForms existentes
        │
        ▼
Endpoint de Notas Workflow
        │  ← contexto autenticado y permiso calculado en servidor
        ▼
ServicioNotasWorkflow
        ├─ tarea explícita y autorización de tarea/nota
        ├─ validación de contenido e idempotencia
        ├─ concurrencia optimista y mutación condicional
        └─ transacción de dominio y auditoría
        ▼
RepositorioNotasMySql parametrizado
        ├─ ANOTACION_TAREA
        └─ wf_log_workflow
```

### Estructura sugerida

```text
Modelo/Workflow/Notas/
  NotasWorkflowModels.vb
  NotasWorkflowInterfaces.vb

DTOs/Workflow/Notas/
  NotasWorkflowDtos.vb

Services/Workflow/Notas/
  ServicioNotasWorkflow.vb

Infrastructure/Repositories/Workflow/
  MySqlNotasWorkflowRepository.vb

webservice/
  WebServiceWorkflowNotesModern.asmx(.vb)
```

Se prefiere un ASMX específico para no mezclar sin límite el dominio de transiciones de tarea con el de colaboración/anotaciones. Puede reutilizar `WorkflowPreviewSessionContextGate` extendiéndolo con `AsegurarContextoNotas`.

### Contratos mínimos

- `ListarNotas(idTarea, cursor, tamañoPagina)`
- `CrearNota(idTarea, contenido, clientRequestId)`
- `ActualizarNota(idTarea, idNota, contenido, version)`
- `EliminarNota(idTarea, idNota, version)`
- `ContarNotas(idTarea)`

Todos devuelven DTOs tipados con códigos funcionales. Nunca devuelven SQL, excepciones internas, cadenas de conexión ni mensajes del motor.

## Decisión aprobada — Diseño moderno de lista de notas

Se aprueba una interfaz de notas tipo **conversación cronológica de tarea**, en lugar de una grilla tabular. Las notas son contenido variable y contextual; el formato de lista facilita lectura, escaneo de autoría y acciones por elemento.

El prototipo navegable aprobado está disponible en [modelo-ui-notas-workflow-moderno.html](modelo-ui-notas-workflow-moderno.html). Es una demostración autocontenida: no se conecta a servicios ni persiste datos.

### Composición aprobada

```text
Contexto de tarea
        │
        ▼
Panel de notas: título + contador + ordenar + nueva nota
        │
        ▼
Lista cronológica de notas
  ├─ autor, cargo y fecha
  ├─ actividad de origen
  ├─ contenido en texto plano
  └─ acciones por nota: editar / eliminar, si el servidor autoriza
        │
        ▼
Editor de nota + confirmación explícita de eliminación
```

| Zona | Componente visual | Decisión |
|---|---|---|
| Contenedor | `.ctw-panel` | Agrupa la lista y conserva la jerarquía del Centro de Trabajo. |
| Cabecera | `.ctw-pane-head` | Muestra título, contexto y contador mediante `.ctw-badge`. |
| Acciones globales | `.ctw-action-bar` | Contiene ordenamiento y `Nueva nota`. |
| Nueva nota | `.ctw-btn` | Acción secundaria; `.ctw-btn--primary` se reserva para la transición principal `Enviar`. |
| Lista | `.ctw-note-list` | Componente nuevo, vertical, paginado por cursor y semántico (`ol`/artículos). |
| Nota | `.ctw-note-item` | Autor, rol, fecha, actividad, contenido y acciones locales. |
| Acciones locales | `.ctw-icon-btn` | Editar/eliminar con nombre accesible; solo se muestran si el endpoint confirma autorización. |
| Eliminación | `.ctw-btn--danger` | Se usa en la confirmación textual de eliminación, no como fondo permanente de la fila. |
| Editor | `.ctw-note-composer` | Área de texto, contador, cancelar y guardar. |
| Estados | `.ctw-note-empty` / `.ctw-note-feedback` | Ausencia de notas, carga, error o resultado de operación. |

### Restricciones de implementación

1. Durante la migración, el `GridView_lista_notas` y sus eventos se conservan como soporte legacy; la nueva lista no debe duplicar controles ni habilitar acciones paralelas.
2. La versión objetivo usa una lista semántica y renderiza contenido exclusivamente como texto, nunca mediante `innerHTML`.
3. Las acciones editar y eliminar son una representación de la autorización del servidor: el frontend no decide permisos.
4. Los estilos quedan encapsulados bajo `.workflow-centro-trabajo-moderno` y reutilizan el contrato CSS del Centro de Trabajo.
5. En móvil la cabecera se apila, las acciones mantienen objetivos táctiles de al menos 40 px y el contenido no pierde acciones esenciales.

## Plan de migración incremental

### Fase 1 — Blindaje del contrato

1. Documentar roles de ver, crear, editar y borrar.
2. Implementar gate de contexto y autorización tarea-nota en servidor.
3. Implementar repositorio parametrizado y DTOs de respuesta seguros.
4. Agregar auditoría transaccional y concurrencia.
5. Establecer mutaciones condicionales, ciclo de vida seguro de transacciones e idempotencia de creación.
6. Validar por consulta de solo lectura el esquema e índices necesarios antes de diseñar una migración de datos o índices.
7. Crear pruebas unitarias y de integración no autenticada contra contratos y repositorios simulados.

### Fase 2 — Lectura en paralelo

1. Activar el nuevo listado y contador para Centro de Trabajo detrás de una bandera reversible.
2. Comparar resultado con la lectura legacy en ambiente controlado.
3. Validar permisos, tarea sin acceso, tarea cerrada y nota inexistente.

### Fase 3 — Escritura gradual

1. Migrar crear, editar y borrar en Centro de Trabajo.
2. Migrar Radicación Entrante y Gestión de Correspondencia al mismo contrato.
3. Migrar consulta histórica en modo solo lectura, con su política de visibilidad definida.

### Fase 4 — Modernización visual

1. Modernizar el modal reutilizando controles existentes o un componente aislado.
2. Conservar identidad de controles, accesibilidad y mecanismos de reversión.
3. Eliminar polling agresivo y actualizar estado desde operaciones confirmadas.

### Fase 5 — Retiro controlado

1. Desactivar endpoints y scripts legacy solo cuando los cuatro consumidores estén migrados.
2. Revisar referencias de `WebFormAnotacion` y retirarla si permanece sin uso.
3. Consolidar pruebas de regresión y evidencias de auditoría.

## Matriz mínima de pruebas

| Caso | Resultado esperado |
|---|---|
| Usuario sin permiso | El endpoint rechaza aun si se invoca directamente. |
| Tarea ajena o inactiva | No revela notas ni permite mutación. |
| Nota de otra tarea | No se puede leer, editar ni borrar por identificador. |
| Nota ajena de misma tarea | Respeta la política de propietario. |
| Comillas, Unicode, saltos de línea | Se persisten como texto sin romper JSON ni SQL. |
| Texto HTML o script | Se muestra como texto, sin ejecutar contenido. |
| Dos ediciones concurrentes | La segunda recibe conflicto y no sobreescribe. |
| Dos pestañas en tareas distintas | Cada operación usa el `idTarea` recibido; ninguna depende de la última tarea guardada en sesión. |
| Error funcional antes de escribir | Conexión y transacción se liberan; no queda cambio ni auditoría parcial. |
| Reintento de creación | El mismo `clientRequestId` devuelve una sola nota y una sola auditoría. |
| Cursor de otra tarea o usuario | Se rechaza o se reinicia de forma segura; no expone notas cruzadas. |
| Histórico y operativo | Aplican orden estable y la política de estados correspondiente. |
| Error de auditoría | La operación completa revierte. |
| Eliminación | Todos los lectores respetan la semántica elegida. |
| Contador | No hace sondeo cada 600 ms ni carga todas las filas. |

## Decisiones de negocio pendientes

1. Quién puede ver notas de tareas cerradas, reasignadas o históricas.
2. Si un supervisor puede editar o borrar una nota ajena.
3. Si eliminar significa ocultar, conservar marcada como eliminada o borrar físicamente.
4. Longitud máxima, retención y clasificación de contenido de la nota.
5. Si auditoría debe retener texto completo, valor anterior/nuevo o solo huellas.

## Evidencia de código revisada

- `workflow/Class_anotacion_tarea.vb`: persistencia y auditoría legacy.
- `webservice/WebServiceWorkflow.asmx.vb`: endpoints ASMX de notas.
- `workflow/Webworkflow.aspx(.vb)` y `js/workflow/Webworkflow.js`: Centro de Trabajo.
- `radicador/WebFormRadicacionEntrante.aspx.vb` y `js/radicacion/WebFormRadicacionEntrante.js`.
- `Gestion_correspondencia/WebForm_interface_gestion_tramite.aspx.vb` y su JavaScript.
- `workflow/WebFormConsultaTareasWorkflow.aspx` y JavaScript histórico.
- `workflow/InicioWorkflow.vb`: origen del permiso de anotaciones.
- `webservice/WorkflowPreviewSessionContextGate.vb`, `Modelo/Workflow`, `Services/Workflow` e `Infrastructure/Workflow`: patrón moderno reutilizable.

## Estado de seguridad operativa

No se ejecutaron consultas a base de datos, E2E, carga ni activaciones de gate. La bandera `WorkflowCentroTrabajoModernActive` se mantuvo en `false`, con usuarios y grupos de piloto vacíos.
