# Manual de requerimientos — Devolver a usuario anterior

## Propósito

Modernizar la operación **Devolver → Usuario anterior** del Centro de trabajo. La operación devuelve una tarea únicamente al usuario Workflow histórico inmediatamente anterior de esa misma tarea.

## Alcance

Incluye:

- Consulta moderna y de solo lectura del usuario histórico anterior.
- Confirmación y ejecución moderna de la devolución.
- Validación de historial, usuario, tarea, permiso, token y concurrencia.
- Auditoría sanitizada y restauración de la bandeja después del éxito.

Excluye:

- **Devolver → Elegir actividad anterior**.
- Devolver a grupos, a una actividad sin usuario o a un destino elegido por el navegador.
- Continuar flujo, Enviar a usuario, Enviar a grupo y sus contratos actuales.
- Postbacks Web Forms, `GridView`, `UpdatePanel`, `ModalPopupExtender` y campos ocultos como mecanismo de autorización o ejecución.
- Tratamiento de respuestas, radicados o confirmaciones de respuesta.

## Requisitos funcionales

### RF-01 — Disponibilidad de la operación

El sistema debe mostrar **Devolver a usuario anterior** para una tarea seleccionada y con contexto Workflow válido.

La opción debe estar disponible solo si el servidor puede resolver un usuario histórico anterior válido. La interfaz no debe abrir ni proponer la devolución a actividad anterior como alternativa automática.

### RF-02 — Previsualización del usuario anterior

El sistema debe exponer un endpoint de solo lectura equivalente a `PreviewDevolverUsuarioAnterior(idTarea)`.

Antes de devolver datos, el servidor debe validar:

- Sesión y contexto Workflow autenticados.
- Tarea válida, activa y accesible al contexto actual.
- Permiso de devolución del usuario autenticado.
- Existencia de al menos un estado histórico anterior para la misma tarea.
- Que el registro histórico anterior tenga `Id_Usuario > 0` y represente un usuario elegible.
- Consistencia del historial con la Ruta o Flujo de la tarea.

El preview debe devolver únicamente actividad anterior, usuario anterior resumido, token de versión y bloqueo funcional cuando aplique.

No debe devolver grupos, actividades alternativas, conectores, datos de respuestas ni datos de otros historiales.

### RF-03 — Bloqueos de historial

El sistema debe bloquear la operación, sin cambiar la tarea, cuando:

- No exista un usuario histórico anterior.
- El registro anterior represente un grupo o no tenga usuario.
- El usuario histórico ya no sea elegible.
- El usuario histórico coincida con el usuario Workflow autenticado.
- La tarea, Ruta o Flujo ya no correspondan al historial consultado.

El bloqueo debe ser público, estable y comprensible. No debe redirigir al selector de actividades anteriores.

### RF-04 — Confirmación moderna

El modal moderno debe presentar la actividad y el usuario anteriores resueltos por el servidor, y solicitar confirmación explícita.

Debe soportar cancelación, Escape, foco inicial, trampa de foco, retorno al disparador, teclado, mensajes accesibles y prevención de doble clic.

No requiere búsqueda ni paginación: la operación tiene un único destino determinado por el historial.

### RF-05 — Ejecución segura

El sistema debe exponer un endpoint equivalente a `EjecutarDevolverUsuarioAnterior(idTarea, tokenVersion)`.

La ejecución no debe recibir usuario, actividad, grupo, Ruta, Flujo ni conector desde el navegador.

Dentro de `GET_LOCK`, el servidor debe releer y validar:

- Contexto autenticado y permiso de devolución.
- Tarea activa y token de versión vigente.
- Registro histórico anterior de la misma tarea.
- Usuario histórico con identificador positivo, elegible y distinto del usuario Workflow autenticado actual.
- Consistencia de la actividad, Ruta o Flujo del registro histórico con la tarea actual.

La transición efectiva debe usar un adaptador exclusivo hacia `Terminar_Tarea_Workflow`, sin `Page` ni handlers Web Forms.

### RF-06 — Corrección de auto-devolución

La validación de auto-devolución debe comparar el usuario histórico anterior contra el usuario Workflow autenticado obtenido en servidor.

No debe usar `Id_Ruta_Workflow` como sustituto de usuario ni aceptar el usuario desde el navegador.

### RF-07 — Resultado y presentación

El resultado debe devolver éxito, bloqueo funcional, advertencias, estado final, token cuando corresponda y referencia de auditoría sanitizada.

En éxito, la interfaz debe cerrar el modal, quitar o actualizar solo la tarea afectada, restaurar el listado y el desplazamiento horizontal de la bandeja y mostrar un mensaje correlacionado.

En bloqueo o error, debe conservar el contexto necesario para reintentar o cancelar sin crear otra transición.

## Regla inviolable: respuestas fuera de alcance

La capacidad no debe consultar, validar, bloquear, crear, actualizar, reasignar ni auditar respuestas, radicados o confirmaciones.

No debe referenciar `Classgestionrespuesta`, `Verifica_respuesta_*` ni `Reasigna_respuesta_envia_tarea_usuario` en endpoints, servicios, repositorios, adaptadores, DTO, JavaScript o pruebas.

## Requisitos no funcionales

### RNF-01 — Seguridad

- El preview solo puede ejecutar `SELECT` parametrizados.
- El usuario destino se deriva exclusivamente del historial validado en servidor.
- Los mensajes públicos no exponen SQL, credenciales, sesión, controles Web Forms ni excepciones internas.
- La auditoría no contiene secretos ni datos innecesarios.

### RNF-02 — Concurrencia y resiliencia

- Dos solicitudes simultáneas no pueden devolver dos veces la misma tarea.
- Un token vencido, lock ocupado, usuario retirado o historial cambiado deben producir un bloqueo funcional estable.
- Mientras la ejecución esté pendiente, la interfaz no debe permitir confirmar nuevamente ni cerrar de forma que abandone el resultado pendiente.

### RNF-03 — Accesibilidad y responsive

- El modal debe operar con teclado, foco, Escape y retorno de foco.
- Debe ser usable en escritorio y móvil.
- Los estados de carga, error, bloqueo y éxito deben anunciarse de forma accesible.

## Criterios de aceptación

1. Una tarea con usuario histórico anterior válido puede mostrarlo y confirmar su devolución.
2. Una tarea sin usuario histórico anterior se bloquea sin mostrar actividades alternativas.
3. Un registro histórico de grupo o sin usuario se bloquea como destino no válido.
4. Un usuario histórico igual al usuario Workflow autenticado se bloquea.
5. La ejecución recibe exclusivamente tarea y token; el destino se resuelve de nuevo dentro del lock.
6. Un token vencido, lock ocupado, doble clic o historial manipulado no produce una segunda transición.
7. Ninguna capa de la capacidad contiene tratamiento de respuestas.
8. Las demás operaciones Workflow conservan sus contratos y comportamientos.

## Pruebas requeridas

- Preview sin escritura con usuario histórico válido, inexistente, grupo y usuario retirado.
- Permiso ausente, tarea no disponible, Ruta o Flujo inconsistente y token vencido.
- Auto-devolución con usuario histórico igual al usuario autenticado.
- Prueba focal que garantice el uso del usuario Workflow autenticado y prohíba `Id_Ruta_Workflow` en esta validación.
- Lock ocupado, concurrencia, éxito, error y advertencia.
- Prueba de aislamiento que falle si la capacidad referencia componentes o métodos de respuesta.
- Pruebas CJS de confirmación, cancelación, bloqueo durante envío, error, éxito, teclado, foco, Escape, responsive y restauración de bandeja.
- Compilación del proyecto y pruebas focales del área modificada.
- E2E autenticada o pruebas con tarea real solo con autorización explícita de ambiente y cuentas de prueba.

## Trazabilidad

Este manual se fundamenta en `01-exploracion-modernizacion-devolver-usuario-anterior.md` del mismo directorio. Antes de implementar se deben crear tareas atómicas, asignar ticket Jira y aprobar las decisiones funcionales de auditoría y presentación.
