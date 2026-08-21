# Exploración técnica — Modernización de devolver a usuario anterior

## Decisión inicial

Es viable modernizar **Devolver → Usuario anterior** como una capacidad independiente.

Su alcance es exclusivo: devuelve una tarea al usuario histórico anterior de esa misma tarea. No muestra, invoca, reutiliza ni redirige a **Devolver → Elegir actividad anterior**.

## Comportamiento actual

La opción se encuentra en el menú **Devolver** de `workflow/Webworkflow.aspx`. El navegador muestra una confirmación JavaScript y activa el postback oculto `Button_tool_devolver_a_usuario`.

El handler Web Forms llama a `ClassWorkflow.Devolver_tarea_workflow_usuario_anterior`.

El método consulta `ESTADOS_TAREA_WORKFLOW` por la tarea, ordena de forma descendente por `id_Estado`, limita el resultado a dos registros y toma el penúltimo registro como origen de:

- Usuario Workflow anterior.
- Actividad anterior.
- Ruta o Flujo y sus identificadores asociados.

Luego delega la transición a `Terminar_Tarea_Workflow`.

## Regla funcional obligatoria

La opción solo está disponible cuando el historial resuelve un **usuario anterior válido**.

- Si la tarea no tiene un registro histórico anterior, debe devolver un bloqueo funcional: no existe usuario anterior disponible.
- Si el registro histórico no tiene usuario (`Id_Usuario = 0`), debe devolver un bloqueo funcional: el registro anterior no corresponde a un usuario.
- No debe abrir el selector de actividades anteriores, ni ejecutar una devolución a grupo, ni convertir la operación en una devolución por conector.
- **Devolver a actividad anterior** conserva su capacidad y contrato independientes.

El recorrido legado mezcla ambas operaciones: cuando no tiene un registro anterior, abre la lista de actividades anteriores. Ese comportamiento no debe trasladarse a la versión moderna.

## Hallazgo de corrección obligatoria

`Devolver_tarea_workflow_usuario_anterior` recibe el parámetro `id_usuario_workflow` para impedir que un usuario se devuelva la tarea a sí mismo.

Sin embargo, el handler actual le entrega `Session.Item("Id_Ruta_Workflow")` en lugar del usuario Workflow autenticado. La validación de auto-devolución puede comparar contra un identificador de Ruta y no contra el usuario real.

La futura implementación debe corregirlo de forma explícita:

- Obtener el usuario Workflow desde el contexto autenticado del servidor.
- Compararlo con el usuario histórico anterior dentro de la revalidación de ejecución.
- Bloquear la operación si ambos identificadores coinciden.
- No aceptar identificadores de usuario desde el navegador.

## Limitaciones del recorrido legado

- Depende de `Page`, `Hidden_*`, `GridView`, `UpdatePanel`, `ModalPopupExtender` y postbacks.
- La consulta histórica concatena el identificador de tarea en SQL y no usa el contrato moderno de token o lock.
- El permiso de devolución se consulta antes de la mutación, pero no existe una revalidación moderna uniforme dentro de un lock.
- No existen pruebas automatizadas ni una especificación específica de esta capacidad.

## Diseño moderno recomendado

Crear contratos exclusivos:

1. `PreviewDevolverUsuarioAnterior(idTarea)`
   - Solo lectura y `SELECT` parametrizado.
   - Valida contexto autenticado, tarea activa, permiso de devolución y existencia de un usuario histórico anterior.
   - Devuelve datos mínimos para confirmar: actividad y usuario anteriores, token de versión y bloqueos funcionales.

2. `EjecutarDevolverUsuarioAnterior(idTarea, tokenVersion)`
   - No recibe usuario, actividad ni conector desde el navegador.
   - Adquiere `GET_LOCK` y relee tarea, token, permiso y registro histórico anterior.
   - Valida que el usuario anterior sea distinto del usuario Workflow autenticado y continúe siendo elegible.
   - Invoca un adaptador exclusivo hacia `Terminar_Tarea_Workflow`, sin controles Web Forms.
   - Normaliza éxito, bloqueo, error y auditoría sin filtrar detalles técnicos.

La interfaz moderna solo necesita presentar una confirmación del usuario y actividad resueltos por el servidor; no requiere búsqueda ni paginación.

## Seguridad y consistencia

- El historial es fuente de destino, pero debe revalidarse dentro del lock antes de ejecutar.
- La tarea debe estar activa y pertenecer al contexto autenticado actual.
- El permiso de devolución debe resolverse en servidor, no mediante valores de Session o navegador no revalidados.
- Token vencido, lock ocupado, usuario retirado, historial ausente o auto-devolución deben devolver códigos funcionales estables.
- Los mensajes públicos no exponen SQL, sesión, controles Web Forms ni excepciones internas.

## Pruebas requeridas

- Preview sin escritura, historial con usuario, historial inexistente e historial con `Id_Usuario = 0`.
- Permiso ausente, tarea no disponible, Ruta o Flujo inconsistente y usuario histórico no elegible.
- Auto-devolución: usuario histórico igual al usuario autenticado debe bloquearse.
- Prueba focal que confirme que la ejecución usa el usuario Workflow autenticado, no `Id_Ruta_Workflow`.
- Token vencido, lock ocupado, doble solicitud, éxito, error y advertencia.
- Confirmación, cancelación, teclado, foco, Escape, bloqueo durante ejecución, responsive y restauración de la bandeja.

No se ejecutará E2E autenticada, carga ni una tarea real sin autorización explícita de ambiente y cuentas de prueba.

## Conclusión

La modernización es factible. Debe ser un flujo determinista por historial de usuario, sin mezcla ni fallback hacia actividades anteriores, y debe corregir la validación de auto-devolución usando el usuario Workflow autenticado.
