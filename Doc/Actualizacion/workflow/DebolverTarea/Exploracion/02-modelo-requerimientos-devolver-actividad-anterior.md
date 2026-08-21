# Modelo de requerimientos — Devolver a actividad anterior

## Propósito

Modernizar la operación **Devolver → Elegir actividad anterior** del Centro de trabajo. La operación permite devolver una tarea activa únicamente a una actividad predecesora válida de su Ruta o Flujo actual.

## Alcance

Incluye:

- Consulta moderna y de solo lectura de actividades anteriores autorizadas.
- Confirmación y ejecución moderna de la devolución a la actividad seleccionada.
- Soporte para contextos de Ruta y de Flujo.
- Revalidación en servidor, concurrencia, auditoría sanitizada y restauración de la bandeja.

Excluye:

- La opción **Devolver → Usuario anterior**.
- Continuar flujo, Enviar a usuario, Enviar a grupo y sus contratos actuales.
- Postbacks Web Forms, `GridView`, `UpdatePanel`, `ModalPopupExtender` y campos ocultos como mecanismo de ejecución.
- Cualquier tratamiento de respuestas.

## Requisitos funcionales

### RF-01 — Punto de entrada moderno

El sistema debe mostrar la acción **Devolver a actividad anterior** para una tarea seleccionada y con contexto Workflow válido.

La acción debe abrir una experiencia moderna accesible y no debe iniciar un postback Web Forms. Su registro depende del contexto Workflow válido, no de `WorkflowCentroTrabajoModernActive`; esto no altera el gate de las demás operaciones.

### RF-02 — Previsualización de actividades anteriores

El sistema debe exponer un endpoint de solo lectura equivalente a `PreviewDevolverActividad(idTarea)`.

Antes de devolver destinos, el servidor debe validar:

- Sesión y contexto Workflow autenticados.
- Identificador de tarea válido, activa y accesible al contexto actual.
- Permiso de devolución del usuario autenticado.
- Consistencia entre la tarea y su Ruta o Flujo.

Para Flujo, la previsualización debe resolver solamente conectores entrantes cuya actividad destino sea la actividad actual del flujo. En este contexto `IdConector` identifica solo el registro de conector de Flujo.

Para Ruta, la previsualización debe resolver solamente las configuraciones `actividades_disponibles_envio` cuya actividad siguiente sea la actividad actual, actividad origen sea predecesora y `id_Ruta` coincida con la tarea. En este contexto `IdConector` identifica solo `id_actividades_disponibles_envio`.

Cada destino debe contener solo los datos mínimos: identificador de conector, nombre de actividad, usuario o grupo resumido cuando aplique, tipo de contexto y token de versión.

### RF-03 — Confirmación de devolución

El usuario debe seleccionar una actividad anterior y confirmar explícitamente la operación.

La interfaz debe informar el destino elegido, mantener foco accesible, permitir cancelar, responder a Escape y evitar doble envío.

### RF-04 — Ejecución segura

El sistema debe exponer un endpoint equivalente a `EjecutarDevolverActividad(idTarea, idConector, tokenVersion)`.

La ejecución debe adquirir el lock de concurrencia y, dentro de él, releer y validar:

- Tarea activa y token de versión vigente.
- Contexto autenticado y permiso de devolución.
- Ruta o Flujo vigente.
- Conector entrante solicitado, su origen y su pertenencia al contexto real de la tarea.

El identificador del conector recibido desde el navegador se debe considerar no confiable. El servidor debe resolverlo nuevamente dentro del tipo de contexto deducido de la tarea y no debe usar datos de destino publicados por el cliente.

La transición efectiva debe pasar por un adaptador exclusivo de devolución hacia `Terminar_Tarea_Workflow`, con `Page = Nothing`, actualización de interfaz legacy desactivada y un perfil aprobado de notificación/eventos que no ejecute métodos de componentes de respuestas.

### RF-05 — Resultado y presentación

El resultado debe devolver éxito, bloqueo funcional, advertencias, estado final, token cuando corresponda y referencia de auditoría sanitizada.

En éxito, la interfaz debe cerrar el modal, quitar o actualizar únicamente la tarea afectada, restablecer el listado y el desplazamiento horizontal de la bandeja, y mostrar un mensaje correlacionado.

En error o bloqueo, debe conservar el contexto necesario para reintentar o cancelar sin iniciar otra transición.

## Regla inviolable: respuestas fuera de alcance

La capacidad no debe consultar, validar, bloquear, crear, actualizar, reasignar ni auditar respuestas, radicados o confirmaciones de respuesta.

No debe referenciar `Classgestionrespuesta`, `Verifica_respuesta_*` ni `Reasigna_respuesta_envia_tarea_usuario` en endpoints, servicios, repositorios, adaptadores, DTO, JavaScript o pruebas.

## Requisitos no funcionales

### RNF-01 — Seguridad

- El preview solo puede ejecutar `SELECT` parametrizados.
- La autorización y la integridad del destino se resuelven en servidor.
- Los mensajes públicos no exponen SQL, credenciales, sesión, controles Web Forms ni excepciones internas.
- La auditoría no contiene secretos ni datos innecesarios.

### RNF-02 — Concurrencia y resiliencia

- Dos solicitudes simultáneas no pueden devolver dos veces la misma tarea. El lock es exclusivo por `IdTarea`, no por token; token y conector se revalidan dentro de él.
- Un token vencido, un lock ocupado o un conector retirado deben producir un bloqueo funcional estable.
- Mientras la ejecución esté pendiente, la interfaz no debe permitir cerrar o confirmar nuevamente la operación.

### RNF-03 — Accesibilidad y responsive

- El modal debe soportar teclado, foco inicial, trampa de foco, Escape y retorno de foco al disparador.
- La lista de destinos debe ser usable en escritorio y móvil.
- Los estados de carga, error y éxito deben anunciarse de forma accesible.

## Criterios de aceptación

1. Una tarea de Flujo solo presenta conectores entrantes válidos de su actividad actual.
2. Una tarea de Ruta solo presenta actividades anteriores válidas de su ruta actual.
3. Un usuario sin permiso de devolución no recibe destinos ni puede ejecutar la operación.
4. Un conector manipulado, ajeno o retirado se bloquea en servidor sin cambiar la tarea.
5. Un token vencido, lock ocupado o doble clic no produce una segunda transición.
6. Una devolución exitosa restablece correctamente la bandeja y su scroll horizontal.
7. Ninguna capa de la capacidad moderna contiene tratamiento de respuestas.
8. Las acciones modernas existentes conservan sus contratos y comportamientos.

## Pruebas requeridas

- Pruebas unitarias de preview sin escritura, permiso, Ruta, Flujo y destinos vacíos.
- Pruebas de ejecución con token vencido, lock ocupado, conector manipulado, concurrencia, éxito, error y advertencia.
- Prueba de aislamiento que falle si la nueva capacidad referencia componentes o métodos de respuesta.
- Pruebas CJS de selección, confirmación, cancelación, bloqueo durante envío, error, éxito, teclado, foco, Escape y responsive.
- Compilación del proyecto y pruebas focales del área modificada.
- E2E autenticada o pruebas con tarea real solo con autorización explícita de ambiente y cuentas de prueba.

## Trazabilidad

Este modelo se fundamenta en `01-exploracion-modernizacion-actividad-anterior.md` del mismo directorio. Antes del ticket backend se deben aprobar las decisiones de identidad Ruta/Flujo, cursor, notificación/eventos y sustitución del postback legacy; después se asignan los cuatro tickets Jira de ejecución.
