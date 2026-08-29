# Modelo de requerimientos — Modernización de Notas de Workflow

| Atributo | Valor |
|---|---|
| Estado | Base técnica resuelta; implementación mutante condicionada a preflight por esquema |
| Versión | 0.3 |
| Fecha | 2026-08-28 |
| Producto | DocuArchi — Workflow |
| Capacidad | Notas de tarea Workflow |
| Artefactos relacionados | `diagnostico-modernizacion-notas-workflow.md` y `modelo-ui-notas-workflow-moderno.html` |

## 1. Propósito

Definir los requerimientos para modernizar la consulta y gestión de notas asociadas a tareas Workflow. La solución debe conservar la continuidad operativa de las superficies WebForms existentes, eliminar las vulnerabilidades observadas en el flujo legacy y entregar una experiencia de lista cronológica aprobada para usuarios de operación.

El documento establece **qué** debe cumplir la capacidad. No define aún el detalle de clases, tablas nuevas o el plan de despliegue ejecutable.

## 2. Problema a resolver

La funcionalidad actual de notas está distribuida entre varios módulos, combina interfaz, sesión, persistencia y auditoría, y expone operaciones mediante endpoints legacy. El flujo actual presenta, entre otros, estos riesgos:

- La autorización se aplica en la interfaz, pero no de manera uniforme en los endpoints.
- La nota y sus identificadores se procesan con SQL y JSON concatenados.
- Una nota puede consultarse o mutarse sin validar de forma completa su pertenencia a la tarea accesible.
- El borrado, auditoría y control de concurrencia no tienen una semántica explícita y uniforme.
- La tarea objetivo se toma de un valor de sesión compartido entre pestañas, y la validación de propietario no queda ligada atómicamente a la escritura.
- Existen rutas de salida después de iniciar transacciones que no garantizan liberación uniforme de recursos, y crear no es idempotente ante reintentos.
- La interfaz usa una grilla para contenido conversacional y un sondeo de contador de alta frecuencia.

La modernización debe resolver estas condiciones sin interrumpir el Centro de Trabajo, Radicación Entrante, Gestión de Correspondencia ni la consulta histórica.

## 3. Objetivos

1. Garantizar que las notas solo puedan verse o modificarse dentro de una tarea autorizada.
2. Consolidar un contrato de backend único, tipado y seguro para las operaciones de notas.
3. Preservar la política de propiedad existente mientras negocio no apruebe una política diferente.
4. Mantener auditoría transaccional y trazable de las operaciones.
5. Presentar una lista cronológica clara, accesible y responsive.
6. Reducir duplicación de clientes y eliminar el sondeo de 600 ms.
7. Permitir una adopción gradual y reversible desde las pantallas WebForms actuales.

## 4. Alcance

### 4.1 Incluido

- Listar, crear, consultar, editar y eliminar notas de una tarea Workflow.
- Contador de notas de la tarea seleccionada.
- Autorización, pertenencia tarea-nota, propiedad y estado de tarea en servidor.
- Auditoría de las operaciones de notas.
- Manejo de concurrencia para edición y eliminación.
- Interfaz moderna de lista cronológica y editor de nota.
- Integración gradual de Centro de Trabajo, Radicación Entrante, Gestión de Correspondencia y consulta histórica.
- Pruebas funcionales, seguridad, accesibilidad, regresión y reversión.

### 4.2 Excluido

- Mensajería en tiempo real, menciones, reacciones, adjuntos o conversaciones por hilo.
- Edición colaborativa simultánea.
- Cambio de reglas de asignación, transición, cierre o devolución de tareas.
- Cambio de la política organizacional de retención documental.
- Reemplazo completo de WebForms o de la base de datos Workflow.
- Inferir o habilitar privilegios de supervisor sin decisión expresa de negocio.

## 5. Actores y permisos

| Actor | Responsabilidad | Acciones previstas |
|---|---|---|
| Usuario Workflow autorizado | Trabaja una tarea asignada o accesible. | Ver notas, crear nota; editar/eliminar únicamente sus propias notas según política vigente. |
| Propietario de nota | Usuario que creó la nota. | Editar y eliminar su nota, mientras la tarea y permiso sigan vigentes. |
| Usuario sin permiso de anotaciones | Usuario autenticado sin privilegio para notas. | No ve contenido ni puede ejecutar ninguna operación de notas. |
| Usuario de consulta histórica | Usuario Workflow con autorización de consulta de una tarea, aunque no la trabaje. | Puede ver las notas históricas de esa tarea; no puede crear, editar ni eliminar desde el modo histórico. |
| Administrador/a funcional | Consulta la trazabilidad bajo los controles del sistema. | No recibe excepción de edición ni eliminación en la política inicial. |

### 5.1 Política base obligatoria

- El permiso de interacción con notas se resuelve en el servidor desde la identidad autenticada y la matriz de permisos Workflow.
- La interfaz no concede permisos: únicamente representa una autorización recibida del backend.
- Toda operación exige una tarea válida y accesible para el contexto del actor.
- La política base de mutación es **solo propietario**. Cualquier excepción requiere requisito nuevo y evidencia de auditoría adicional.
- La consulta histórica es solo lectura y está disponible para todo usuario Workflow autorizado a consultar la tarea histórica; no depende de ser propietario ni del usuario que actualmente trabaja la tarea.
- El contenido es texto plano no vacío, sin NUL y con máximo de **16.000 unidades UTF-16**. En MySQL 5.1 se admite Unicode BMP (`utf8` de tres bytes); pares sustitutos, incluidos emojis, se rechazan en servidor. El máximo usa como máximo 48.000 bytes del `TEXT` de 65.535 bytes confirmado.
- La eliminación moderna es física. Retira el contenido de las lecturas operativas e históricas y conserva únicamente la auditoría de metadatos de la operación.
- No existe excepción de mutación para supervisor, administrador ni rol equivalente: crear, editar y eliminar se limitan al propietario autorizado mientras la tarea sea operable.
- La nota hereda la clasificación y retención de su tarea o documento padre; no tiene calendario autónomo ni se replica íntegra en auditoría.

## 6. Glosario

| Término | Definición |
|---|---|
| Tarea | Unidad de trabajo Workflow identificada por `idTarea`. |
| Nota | Comentario de texto asociado a una única tarea, actividad, autor y momento de creación. |
| Propietario | Usuario Workflow que creó la nota. |
| Contexto Workflow | Identidad, grupo, ruta y permisos resueltos desde la sesión autenticada. |
| Versión | Valor de concurrencia que identifica el estado conocido de una nota al editar o eliminar. |
| Idempotencia | Propiedad por la que el reintento de una misma intención de creación no genera más de una nota. |
| `clientRequestId` | Identificador opaco y único de una intención de crear nota, generado por cliente y validado por servidor. |
| Borrado | `DELETE` físico condicionado por nota, tarea, propietario y versión, dentro de la misma transacción de auditoría. |
| Consulta histórica | Vista de solo lectura de notas bajo una política explícita de acceso a tareas no activas. |

## 7. Requerimientos funcionales

| ID | Requerimiento | Prioridad | Criterio de aceptación resumido |
|---|---|---|---|
| RF-01 | El sistema debe listar las notas de una tarea autorizada en orden cronológico descendente por defecto. | Obligatorio | Solo retorna notas pertenecientes a `idTarea`; la respuesta incluye autor, rol, actividad, fecha, contenido, id y versión. |
| RF-02 | El sistema debe permitir paginación por cursor o mecanismo equivalente. | Obligatorio | La primera carga tiene tamaño configurable; no carga todas las notas sin límite. |
| RF-03 | El usuario autorizado debe poder crear una nota de texto para la tarea activa. | Obligatorio | El backend determina autor, actividad y fecha; el navegador no envía ni controla estos valores. |
| RF-04 | El sistema debe permitir recuperar el contenido de una nota únicamente cuando esta pertenece a una tarea autorizada. | Obligatorio | Una solicitud con nota de otra tarea recibe rechazo funcional sin exponer contenido. |
| RF-05 | El propietario debe poder editar una nota propia usando una versión vigente. | Obligatorio | Si la versión no coincide, se informa conflicto y no se sobrescribe el cambio existente. |
| RF-06 | El propietario debe poder solicitar la eliminación de una nota propia usando una versión vigente. | Obligatorio | La interfaz solicita confirmación explícita; el backend valida propiedad, tarea y versión. |
| RF-07 | El sistema debe rechazar crear, editar o eliminar para usuario sin permiso de notas. | Obligatorio | La invocación directa al endpoint devuelve código de autorización y no cambia datos. |
| RF-08 | El sistema debe validar que la tarea exista, esté en un estado operable para la acción y sea accesible para el contexto del actor. | Obligatorio | Un id de tarea alterado o ajeno no devuelve notas ni permite mutación. |
| RF-09 | El sistema debe mostrar un contador de notas para la tarea seleccionada. | Obligatorio | Se actualiza tras operaciones confirmadas y cambio de tarea; no hace sondeo cada 600 ms. |
| RF-10 | El sistema debe registrar auditoría de creación, actualización y eliminación en la misma unidad transaccional de la operación. | Obligatorio | La auditoría conserva actor, tarea real, nota, operación, fecha, resultado, correlación, versión, longitudes y huellas SHA-256, sin texto completo. |
| RF-11 | El sistema debe soportar consulta histórica de solo lectura bajo una política de visibilidad explícita. | Obligatorio | Un usuario habilitado puede listar; ninguna acción de mutación se expone ni se acepta. |
| RF-12 | El sistema debe devolver errores funcionales estables y seguros. | Obligatorio | No se devuelven consultas SQL, trazas, excepciones ni detalles de infraestructura. |
| RF-13 | El sistema debe presentar estado vacío, carga, error, éxito y conflicto de concurrencia. | Obligatorio | La interfaz comunica cada estado sin dejar controles ambiguos o bloqueados. |
| RF-14 | El sistema debe conservar la compatibilidad temporal con las cuatro superficies consumidoras. | Obligatorio | Cada consumidor puede migrar de forma independiente sin doble escritura ni doble acción visible. |
| RF-15 | Cada operación debe recibir `idTarea` explícitamente y no debe tomar la tarea objetivo de un valor mutable de sesión. | Obligatorio | Dos pestañas de la misma sesión y tareas distintas operan siempre sobre el `idTarea` de su solicitud validada. |
| RF-16 | Crear una nota debe ser idempotente para una misma intención de usuario. | Obligatorio | Un reintento dentro de 30 días con igual `clientRequestId`, tarea y autor devuelve el resultado original y no crea ni audita una segunda nota. |
| RF-17 | Actualizar y eliminar deben aplicar autorización, pertenencia, estado y versión como condición atómica de la mutación. | Obligatorio | Ninguna escritura se ejecuta si no coinciden nota, tarea, actor, estado aplicable y versión esperada. |
| RF-18 | La actividad de origen debe derivarse del snapshot autorizado y vigente de la tarea. | Obligatorio | El backend no combina una tarea solicitada con una actividad obtenida solo desde grupo o navegador. |
| RF-19 | El listado operativo e histórico debe aplicar orden estable y una política de estados explícita. | Obligatorio | Las respuestas se ordenan por fecha e identificador; no exponen estados fuera de la política del modo consultado. |
| RF-20 | El backend debe retirar las rutas duplicadas de borrado cuando no tengan referencias consumidoras. | Obligatorio | Existe una única implementación moderna de borrado y evidencia de regresión antes de retirar el código legacy sin uso. |

## 8. Reglas de negocio

| ID | Regla |
|---|---|
| RN-01 | Una nota pertenece a una única tarea Workflow y su relación no puede cambiarse después de crearla. |
| RN-02 | Autor, fecha de creación, actividad de origen y grupo se derivan del contexto validado por servidor. |
| RN-03 | El contenido es texto plano. El formato HTML ejecutable no es parte del dominio de notas. |
| RN-04 | El autor es el único actor que puede modificar o eliminar su propia nota; no existe excepción inicial para supervisor o administrador. |
| RN-05 | Editar y eliminar requieren que la nota pertenezca a la tarea solicitada y que la tarea continúe autorizada para el actor. |
| RN-06 | Ninguna mutación se completa si la auditoría asociada no se registra satisfactoriamente. |
| RN-07 | La eliminación requiere confirmación en interfaz, pero la confirmación no reemplaza la autorización ni la validación de servidor. |
| RN-08 | El contador representa notas visibles bajo la misma política de la lista. |
| RN-09 | La consulta histórica no altera la tarea, estado, auditoría ni el contenido de notas. |
| RN-10 | El borrado moderno es físico, se ejecuta con tarea, propietario y versión como condición atómica, y la nota deja de estar disponible tanto en lectura operativa como histórica. |
| RN-11 | `idTarea` identifica el recurso objetivo de cada comando; la sesión no puede suplirlo ni sobrescribirlo. |
| RN-12 | La validación de acceso a tarea, propiedad, estado y versión forma parte de la condición de persistencia de editar o eliminar, no de una comprobación previa aislada. |
| RN-13 | La actividad registrada para una nota pertenece al estado y recorrido de la tarea autorizada en el momento de crear. |
| RN-14 | Un `clientRequestId` solo puede materializar una creación por combinación de tarea y autor. |
| RN-15 | El orden predeterminado es `fechaCreacion DESC, idNota DESC`; otros órdenes solo se habilitan si están definidos en el contrato. |
| RN-16 | Cursores, filtros y orden no permiten trasladar el contexto de listado entre tareas o actores. |
| RN-17 | Las validaciones que no requieren datos se resuelven antes de abrir conexión; cualquier recurso abierto se libera en todos los resultados y excepciones. |
| RN-18 | La ruta de negocio de Workflow forma parte del contexto autorizado y del snapshot de tarea; no procede de URL, campo oculto ni payload de Notas. |
| RN-19 | La consulta histórica permite leer a cualquier usuario Workflow autorizado para consultar la tarea histórica, pero nunca habilita mutaciones. |
| RN-20 | El contenido de una nota es texto plano no vacío, sin carácter NUL, no supera 16.000 unidades UTF-16 y se limita a Unicode BMP mientras el motor sea MySQL 5.1. |
| RN-21 | La versión es un ETag SHA-256 opaco calculado por servidor a partir de los valores persistidos de la nota; editar y eliminar lo comparan dentro de la condición de persistencia. |
| RN-22 | La auditoría no registra contenido completo ni valores anterior/nuevo completos; registra huella SHA-256 y longitud de cada valor relevante. |
| RN-23 | `clientRequestId` es un UUID opaco y único por tarea y autor durante 30 días; la misma combinación devuelve la primera respuesta y no añade otra auditoría. |
| RN-24 | Antes de una mutación en MySQL 5.1, el esquema debe confirmar InnoDB para nota y auditoría, `TEXT utf8` para contenido, índices de listado y tabla InnoDB de idempotencia; sin ello responde `Unavailable`. |

## 9. Requerimientos de experiencia de usuario

El diseño aprobado se documenta en `modelo-ui-notas-workflow-moderno.html`.

### 9.1 Lista de notas

- La lista debe ser cronológica, vertical y semántica; no una tabla como diseño objetivo.
- Cada ítem presenta: autor, rol o cargo, fecha/hora, actividad de origen, contenido y acciones disponibles.
- La nota más reciente aparece primero por defecto; debe existir orden ascendente opcional.
- El contenido puede ocupar varias líneas y debe conservar saltos de línea como texto.
- La cabecera muestra título, contador, ordenamiento y la acción `Nueva nota`.
- El estado vacío debe explicar que aún no existen notas y ofrecer `Nueva nota` solo si el usuario puede crear.
- La edición abre el editor con el contenido vigente; la eliminación abre un diálogo de confirmación con texto explícito.

### 9.2 Componentes y estilo

Los estilos se encapsulan en `.workflow-centro-trabajo-moderno` y reutilizan el contrato del Centro de Trabajo:

| Elemento | Componente |
|---|---|
| Panel y título | `.ctw-panel`, `.ctw-pane-head` |
| Contador y actividad | `.ctw-badge` |
| Barra de acciones | `.ctw-action-bar` |
| Nueva nota y cancelar | `.ctw-btn` |
| Editar/eliminar | `.ctw-icon-btn` con nombre accesible |
| Confirmación de eliminación | `.ctw-btn--danger` con texto visible |
| Lista y composición | `.ctw-note-list`, `.ctw-note-item`, `.ctw-note-composer` |

`Enviar` permanece como la única acción candidata a `.ctw-btn--primary`; guardar una nota no debe aparentar ser una transición Workflow principal.

### 9.3 Accesibilidad y responsive

- Cumplir WCAG 2.1 nivel AA para la interfaz incorporada.
- Todo control de icono tiene `aria-label`; `title` no sustituye nombre accesible.
- La interacción funciona con Tab, Shift+Tab, Enter, Espacio y Escape.
- La confirmación y editor gestionan foco de forma coherente y regresan foco al disparador al cerrar.
- Las acciones de icono tienen objetivo táctil mínimo de 40 × 40 px en móvil.
- En 375 px, la cabecera y acciones se apilan sin ocultar operaciones esenciales.
- El color no es el único indicador de estado o peligro.

## 10. Requerimientos de seguridad

| ID | Requerimiento |
|---|---|
| RS-01 | El endpoint debe resolver identidad y permisos desde sesión autenticada; no acepta usuario, grupo, permiso, autor ni propiedad desde cliente. |
| RS-02 | Cada lectura y mutación debe comprobar acceso a tarea, pertenencia nota-tarea y estado aplicable antes de procesar contenido. |
| RS-03 | Toda consulta de persistencia usa parámetros; queda prohibida la concatenación de contenido o identificadores no validados en SQL. |
| RS-04 | Los DTOs deben ser tipados; el cliente serializa solicitudes con JSON real, no concatenando cadenas. |
| RS-05 | El contenido se codifica al renderizar y se inserta con APIs de texto (`textContent`); nunca se interpreta como HTML. |
| RS-06 | Los errores visibles son funcionales; diagnóstico técnico completo queda en trazabilidad protegida. |
| RS-07 | Las operaciones mutantes deben estar protegidas contra solicitudes forjadas conforme al mecanismo compatible con Forms Authentication y ASMX definido por el proyecto. |
| RS-08 | La autorización falla cerrada si no se puede resolver sesión, permiso, tarea o propiedad. |
| RS-09 | El log de seguridad no debe registrar credenciales, cookies, cadenas de conexión ni contenido sensible fuera de la política de auditoría aprobada. |
| RS-10 | El cursor debe estar protegido y ligado a tarea, actor o contexto, filtros y orden; el orden se resuelve mediante lista blanca, nunca desde SQL dinámico recibido del cliente. |
| RS-11 | `IdRutaWorkflow` del contexto y `IdRuta` de la tarea se validan en servidor. Los metadatos de ruta se consultan con parámetros desde `rutas_workflow`; el cliente no puede aportar nombres de ruta, tablas ni campos dinámicos. |

## 11. Requerimientos de datos e integridad

### 11.1 Modelo lógico mínimo

| Campo | Origen / regla |
|---|---|
| `idNota` | Identificador inmutable asignado por persistencia. |
| `idTarea` | Asociado al crear; validado desde el contrato y contexto. |
| `idActividad` | Resuelto por servidor desde el contexto vigente. |
| `idAutorWorkflow` | Resuelto por servidor; inmutable. |
| `contenido` | Texto plano validado y limitado por la política de negocio. |
| `fechaCreacion` | Asignada por servidor. |
| `fechaActualizacion` | Asignada por servidor si aplica. |
| `estado` | Debe reflejar la semántica de eliminación aprobada. |
| `version` | ETag SHA-256 opaco calculado por servidor desde los valores persistidos; cambia cuando cambia el estado relevante de la nota. |
| `clientRequestId` | UUID requerido al crear; permite detectar reintentos y correlacionar una única intención de usuario durante 30 días. |

### 11.2 Validación de contenido

- La nota no puede estar vacía ni contener solo espacio en blanco.
- La longitud máxima es 16.000 unidades UTF-16 y se valida en cliente y servidor; el servidor prevalece.
- Se aceptan caracteres Unicode BMP, comillas y saltos de línea como texto; se rechazan pares sustitutos y caracteres suplementarios.
- El contenido no se sanitiza mediante eliminación silenciosa de texto; se almacena y muestra como texto plano.
- En MySQL 5.1 la columna se conserva como `utf8`; el contrato restringe el contenido a Unicode BMP. `utf8mb4` exige MySQL 5.5.3 o posterior y no se aplica en esta fase.

### 11.3 Concurrencia

- Crear no requiere versión.
- Editar y eliminar exigen el ETag SHA-256 observado por el usuario.
- Ante conflicto, el sistema no sobrescribe; informa que la nota cambió y ofrece recargar el contenido vigente.
- La respuesta de conflicto no revela contenido de una nota que dejó de ser autorizada.

### 11.4 Auditoría

Para crear, editar y eliminar se registra como mínimo:

- Identificador de correlación.
- Actor autenticado y contexto Workflow efectivo.
- Tarea real y nota afectada.
- Tipo de operación y momento de servidor.
- Resultado, motivo de rechazo cuando aplique y versión resultante.
- Huella SHA-256 y longitud de valor anterior/nuevo, sin texto completo.

### 11.5 Persistencia, atomicidad y recursos

- Crear, actualizar o eliminar y su auditoría se ejecutan en una única transacción InnoDB.
- Editar y eliminar usan una sentencia condicionada por nota, tarea, actor, estado y versión; no se acepta una autorización previa como sustituto de esa condición.
- Conexiones, comandos, lectores y transacciones se liberan de forma determinista ante éxito, rechazo funcional y excepción.
- La migración previa por esquema convierte `ANOTACION_TAREA` de MyISAM a InnoDB, verifica `Dato_Anotacion TEXT utf8`, agrega índices de operación `(tarea, estado, fecha, nota)` e histórico `(tarea, fecha, nota)`, y garantiza `wf_log_workflow` InnoDB con índice `(tarea, fecha, log)` y tabla InnoDB de idempotencia. El servidor rechaza caracteres fuera de Unicode BMP. Su aprobación y aplicación quedan fuera de DOC-40.

## 12. Requerimientos no funcionales

| ID | Categoría | Requerimiento verificable |
|---|---|---|
| RNF-01 | Rendimiento | El listado se pagina y la consulta de contador utiliza agregación, no materializa todas las notas. |
| RNF-02 | Rendimiento | La interfaz no programa sondeo de notas menor a 30 segundos; la opción preferida es actualización por eventos de usuario y cambio de tarea. |
| RNF-03 | Disponibilidad | Un error de notas no debe bloquear la carga ni las transiciones de la tarea. |
| RNF-04 | Compatibilidad | La primera liberación preserva WebForms, `UpdatePanel`, permisos y eventos existentes; no clona controles operativos. |
| RNF-05 | Reversión | Cada consumidor se puede devolver al flujo legacy mediante una bandera o adaptador, sin migración irreversible de datos en la misma liberación. |
| RNF-06 | Observabilidad | Se registran métricas de operación, latencia, autorización denegada, conflicto y error, sin exponer datos sensibles. |
| RNF-07 | Mantenibilidad | El cliente de notas y el contrato backend son únicos; no se copian funciones CRUD entre módulos. |
| RNF-08 | Pruebas | Las reglas de autorización, pertenencia, concurrencia y auditoría tienen pruebas automatizadas y E2E reales antes de activar escrituras modernas. La transición E2E Workflow reutiliza exclusivamente DOC-32 dentro de `tools/e2e`; las pruebas CRUD de Notas son cobertura contractual complementaria y no la sustituyen. Los comandos capturan en la misma consola los valores del modo requerido de manera efímera, ocultan secretos y no requieren `.env` ni carga manual persistente. |
| RNF-09 | Integridad | No se dejan transacciones, conexiones o auditorías parciales ante rechazos funcionales, errores ni reintentos. |
| RNF-10 | Mantenibilidad | La capa de dominio y repositorios no depende de `Page`, `GridView`, `UpdatePanel` ni `HttpContext`; solo el transporte y gate conocen ASMX/sesión. |

## 13. Interfaces y contratos esperados

| Operación | Solicitud mínima | Respuesta mínima |
|---|---|---|
| Listar | `idTarea`, cursor protegido, tamaño, orden permitido | notas visibles, siguiente cursor, total/contador visible, autorización/errores funcionales |
| Crear | `idTarea`, contenido, `clientRequestId` | nota creada o resultado idempotente, versión, fecha, estado funcional |
| Consultar | `idTarea`, `idNota` | contenido, metadatos visibles, versión |
| Actualizar | `idTarea`, `idNota`, contenido, versión | nota actualizada o conflicto/rechazo funcional |
| Eliminar | `idTarea`, `idNota`, versión | confirmación de resultado o conflicto/rechazo funcional |
| Contar | `idTarea` | cantidad de notas visibles o rechazo funcional |

No se autoriza la exposición de endpoints que reciban una nota sin tarea asociada, ni respuestas con excepciones sin normalizar.

## 14. Criterios de aceptación end-to-end

### CA-01 — Lectura autorizada

**Dado** un usuario con permiso de notas y una tarea accesible,  
**cuando** solicita el listado,  
**entonces** recibe únicamente notas de esa tarea, ordenadas y paginadas según la solicitud.

### CA-02 — Denegación segura

**Dado** un usuario sin permiso o una tarea no accesible,  
**cuando** invoca cualquier endpoint de notas directamente,  
**entonces** recibe un código funcional de denegación y no obtiene contenido ni modifica datos.

### CA-03 — Creación controlada

**Dado** un usuario autorizado con una tarea activa,  
**cuando** crea una nota válida,  
**entonces** el servidor asigna autor, actividad, fecha y versión, persiste la nota y auditoría de forma atómica, y el contador se actualiza.

### CA-04 — Protección por pertenencia

**Dado** un identificador de nota que pertenece a otra tarea,  
**cuando** el usuario intenta leer, editar o borrar desde la tarea actual,  
**entonces** la operación se rechaza sin revelar datos de la nota objetivo.

### CA-05 — Propiedad

**Dado** una nota creada por otro usuario,  
**cuando** un usuario autorizado intenta editarla o eliminarla,  
**entonces** la operación se rechaza de acuerdo con la política base de solo propietario.

### CA-06 — Concurrencia

**Dado** dos pestañas abiertas sobre una misma nota propia,  
**cuando** una guarda un cambio y la otra intenta guardar con versión previa,  
**entonces** la segunda recibe conflicto y no sobrescribe el contenido.

### CA-07 — Seguridad de contenido

**Dado** una nota que contiene comillas, Unicode, saltos de línea o texto similar a HTML,  
**cuando** se crea, lista o edita,  
**entonces** se conserva como texto y no ejecuta código ni rompe el contrato JSON/SQL.

### CA-08 — Borrado y trazabilidad

**Dado** una eliminación autorizada,  
**cuando** se confirma la operación,  
**entonces** la nota deja de estar visible según la semántica aprobada y existe una única auditoría vinculada a tarea, nota y actor correctos.

### CA-11 — Aislamiento de tarea entre pestañas

**Dado** un usuario con dos pestañas abiertas sobre tareas distintas,  
**cuando** crea, edita o elimina una nota desde cualquiera de ellas,  
**entonces** el backend usa el `idTarea` de esa solicitud validada y no una tarea seleccionada previamente en sesión.

### CA-12 — Mutación atómica

**Dado** una nota propia autorizada con una versión conocida,  
**cuando** se intenta editar o eliminar,  
**entonces** nota, tarea, actor, estado y versión se comprueban en la mutación; si cualquiera no coincide, no hay cambio ni auditoría de éxito.

### CA-13 — Reintento idempotente de creación

**Dado** una creación cuya respuesta se pierde en tránsito,  
**cuando** el cliente reintenta con el mismo `clientRequestId`,  
**entonces** recibe la nota originalmente creada y no se duplica la operación ni su auditoría.

### CA-14 — Liberación de recursos y reversión

**Dado** un rechazo funcional o una falla durante una operación mutante,  
**cuando** termina la solicitud,  
**entonces** no persiste una nota ni auditoría parcial y la conexión/transacción queda liberada o revertida.

### CA-15 — Listado aislado y determinista

**Dado** un cursor emitido para una tarea, actor, filtros y orden,  
**cuando** se reutiliza con otro contexto o se solicita un orden no permitido,  
**entonces** el sistema rechaza o reinicia la consulta sin exponer notas de otro contexto.

### CA-16 — E2E real con arnés existente

**Dado** un ambiente, cuentas y tareas descartables expresamente autorizados,  
**cuando** se ejecuta la validación E2E de Notas,  
**entonces** reutiliza `tools/e2e/tests/support/authenticated-workflow-session.cjs` y los controles de configuración/evidencia existentes; solicita la configuración necesaria desde la misma consola de forma efímera, oculta secretos y falla sin TTY antes de autenticar; la lectura preserva las huellas de estado y auditoría y las escrituras autorizadas prueban los resultados esperados sin revelar secretos ni contenido sensible.

### CA-17 — Aislamiento por ruta Workflow

**Dado** un contexto autenticado y una tarea solicitada,
**cuando** el servidor no puede validar la ruta del contexto, la ruta de la tarea o la coherencia entre ambas,
**entonces** rechaza la operación sin usar una ruta, tabla o campo recibido desde el navegador.

### CA-09 — Accesibilidad

**Dado** un usuario de teclado o lector de pantalla,  
**cuando** navega la lista, abre el editor o confirma eliminación,  
**entonces** puede completar las acciones con foco visible, etiquetas accesibles y Escape para cerrar diálogos.

### CA-10 — Compatibilidad y reversión

**Dado** la adopción gradual por consumidor,  
**cuando** se desactiva la capa moderna de un consumidor,  
**entonces** su flujo legacy continúa operando sin doble escritura ni pérdida de notas.

## 15. Estrategia de migración y liberación

1. **Base y seguridad:** contratos con tarea explícita, autorización, persistencia parametrizada, mutaciones condicionales, ciclo de vida seguro de recursos, auditoría e idempotencia.
2. **Lectura en paralelo:** listado y contador modernos detrás de activación reversible, con comparación controlada contra legacy.
3. **Escritura por consumidor:** Centro de Trabajo, luego Radicación, Gestión de Correspondencia y consulta histórica de solo lectura.
4. **Presentación:** aplicar el modelo UX/UI aprobado sin duplicar controles ni sustituir reglas de servidor.
5. **Retiro:** inventariar y retirar la ruta duplicada de borrado, scripts y endpoints legacy solamente cuando todos los consumidores estén migrados y la matriz de regresión esté aprobada.

La activación debe iniciar deshabilitada. No se habilitan usuarios, grupos, E2E ni pruebas de carga sin autorización explícita del ambiente y cuentas de prueba.

## 16. Matriz mínima de pruebas

| Área | Casos mínimos |
|---|---|
| Autorización | Sin permiso, permiso vigente, sesión incompleta, tarea ajena, tarea inactiva, consulta histórica. |
| Propiedad | Nota propia, ajena, misma nota en diferente tarea, identificadores manipulados. |
| Contenido | Vacía, máximo permitido, excedida, comillas, Unicode, saltos de línea, texto HTML. |
| Concurrencia | Edición simultánea, eliminación tras edición, reintento posterior a conflicto. |
| Contexto y atomicidad | Dos pestañas con tareas distintas, nota/tarea cruzada, actor no propietario, estado no operable y versión no vigente en una sola mutación. |
| Idempotencia | Doble clic, reintento tras pérdida de respuesta, mismo `clientRequestId` en distinta tarea o distinto autor. |
| Recursos y consistencia | Rechazo antes y durante escritura, error de auditoría, rollback y liberación de conexión/transacción. |
| Listado | Orden estable, cursor manipulado, cursor de otro actor/tarea y estados operativo/histórico. |
| E2E real | Sesión autenticada reutilizada, anónimo, lectura sin mutación, escritura autorizada sobre tarea descartable, idempotencia, conflicto y evidencia de estado/auditoría con `SELECT` de solo lectura. |
| Auditoría | Crear/editar/eliminar exitosos; falla de auditoría; tarea y actor correctos. |
| Rendimiento | Paginación, `COUNT(*)`, ausencia de sondeo de 600 ms, lista grande. |
| UI | Vacío, carga, error, conflicto, móvil 375 px, escritorio 1366 px, zoom 200 %. |
| Regresión | Centro de Trabajo, Radicación Entrante, Gestión de Correspondencia y consulta histórica. |
| Reversión | Desactivar consumidor moderno, comprobar continuidad legacy y ausencia de doble operación. |

## 17. Dependencias y supuestos

| Tipo | Elemento |
|---|---|
| Dependencia técnica | Contexto de sesión Workflow validado y matriz de permisos disponible en servidor. |
| Dependencia técnica | Acceso a persistencia MySQL mediante repositorio parametrizado y transacciones InnoDB. |
| Dependencia técnica | Patrón moderno existente para context gate, DTOs, servicios y repositorios. |
| Dependencia técnica | Ruta Workflow validada mediante `IdRutaWorkflow`, `IdRuta` de la tarea y metadatos de `rutas_workflow` consultados en servidor. |
| Dependencia técnica | Verificación del esquema e índices MySQL mediante consultas de solo lectura antes de definir migración de índices. |
| Dependencia de despliegue | Migración autorizada y preflight por esquema de motor InnoDB, `TEXT utf8` compatible con MySQL 5.1, auditoría, índices e idempotencia antes de la primera escritura. |
| Evidencia de esquema | En 2026-08-28, siete esquemas Workflow inspeccionados exponen `ANOTACION_TAREA` MyISAM con `Dato_Anotacion TEXT utf8` nullable, 65.535 bytes y solo índice individual por tarea; tres exponen `wf_log_workflow` InnoDB con `datos_operacion LONGTEXT latin1`. |
| Supuesto | La política inicial de mutación es solo propietario. |
| Supuesto | Las notas son texto plano, sin formato enriquecido ni adjuntos. |
| Supuesto | La activación moderna se mantiene reversible por consumidor. |

## 18. Decisiones de negocio y estado

| ID | Decisión | Estado | Impacto si no se resuelve |
|---|---|---|---|
| DP-01 | Semántica definitiva de eliminación: física, lógica o archivada. | **Resuelta:** borrado físico con auditoría atómica de metadatos; no hay recuperación ni visibilidad operativa o histórica del contenido eliminado. | Preserva la semántica legacy y evita retener contenido borrado. |
| DP-02 | Usuarios que pueden ver notas de tareas cerradas, reasignadas o históricas. | **Resuelta:** todo usuario Workflow autorizado a consultar la tarea histórica puede leer sus notas; el modo es estrictamente de solo lectura. | Se debe implementar con autorización de tarea independiente de la condición operativa de mutación. |
| DP-03 | Excepciones a “solo propietario” para supervisores o administradores. | **Resuelta:** no hay excepción inicial; los roles distintos del propietario no crean, editan ni eliminan. | Evita ampliar privilegios sin evidencia de regla legacy. |
| DP-04 | Longitud máxima, clasificación y retención del contenido. | **Resuelta:** máximo 16.000 UTF-16, texto plano y Unicode BMP; clasificación y retención heredan de tarea/documento padre. MySQL 5.1 conserva `TEXT utf8` y el servidor rechaza caracteres suplementarios. | Es compatible con el motor actual y no crea una política documental autónoma. |
| DP-05 | Nivel de detalle de auditoría: texto completo, valor anterior/nuevo o huellas. | **Resuelta:** metadatos, resultado, correlación, longitudes y SHA-256; nunca contenido completo. | Evita duplicar contenido en `wf_log_workflow.datos_operacion`. |
| DP-06 | Presupuesto de rendimiento y volumen esperado de notas por tarea. | **Resuelta:** página inicial 50, máximo 100, `COUNT(*)` y refresco por evento o cambio de tarea; índices específicos para operación e histórico. | El mayor esquema observado tiene 17.048 notas, por lo que se elimina el sondeo de 600 ms. |
| DP-07 | Política de retención de `clientRequestId` y de respuesta idempotente. | **Resuelta:** unicidad por tarea, autor y UUID; se conserva resultado original durante 30 días y después se limpia mediante proceso controlado. | Evita doble creación/auditoría sin acumulación indefinida. |

## 19. Trazabilidad con el diagnóstico

| Requerimiento | Riesgo que mitiga |
|---|---|
| RS-01, RS-02, RF-07, RF-08 | Autorización aplicada solo en UI e identificadores manipulables. |
| RS-03, RS-04 | SQL y JSON concatenados. |
| RS-05, RF-13 | XSS almacenado y estados de interfaz inseguros. |
| RF-05, RF-06, RN-05, concurrencia | Actualización/borrado sin tarea y sin versión. |
| RF-10, RN-06, auditoría | Auditoría que puede quedar asociada a tarea incorrecta. |
| RF-15, RN-11, CA-11 | Tarea tomada de sesión mutable y compartida entre pestañas. |
| RN-18, RS-11, CA-17 | Ruta de Workflow ausente, incoherente o controlada por el cliente al resolver metadatos técnicos. |
| RF-16, RN-14, CA-13 | Doble clic, reintento de red o pérdida de respuesta que duplica una nota. |
| RF-17, RN-12, CA-12 | Ventana TOCTOU entre comprobar propiedad y mutar por identificador. |
| RF-18, RN-13 | Actividad de la nota derivada de grupo sin validar el estado de la tarea. |
| RF-19, RN-15, RN-16, CA-15 | Histórico no determinista y cursor u orden fuera de contexto. |
| RN-17, 11.5, RNF-09, CA-14 | Salida anticipada con recursos o auditoría parcial. |
| RF-20, RNF-10 | Borrado duplicado y mezcla de WebForms, sesión y persistencia. |
| RF-09, RNF-01, RNF-02 | Sondeo de contador cada 600 ms y consulta no agregada. |
| RF-14, RNF-04, RNF-05 | Clientes duplicados y necesidad de adopción incremental. |

## 20. Criterio de preparación para implementación

La capacidad está lista para abrir una propuesta de escritura cuando el cambio de esa fase reciba autorización y complete la migración y preflight por cada esquema objetivo. Las decisiones de acceso, borrado, contenido, auditoría, rendimiento e idempotencia quedaron definidas mediante código legacy y metadatos MySQL de solo lectura.
