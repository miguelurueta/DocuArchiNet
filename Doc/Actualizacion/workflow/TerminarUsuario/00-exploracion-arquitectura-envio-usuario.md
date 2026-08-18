# Exploracion arquitectonica — Enviar a usuario

## Alcance y decision

Esta exploracion cubre exclusivamente la accion **Enviar a usuario** del Centro de trabajo, expuesta en `workflow/Webworkflow.aspx` mediante `ImageButtonEnviarUsuario`.

La modernizacion debe ejecutar unicamente `ClassWorkflow.Terminar_Tarea_Workflow`. Quedan fuera del alcance la reasignacion de respuesta, el envio a grupo, Continuar flujo, Pendientes y las pantallas de Gestion de Correspondencia.

Cuando la tarea requiera confirmacion o radicado de respuesta, la experiencia moderna debe bloquear la operacion con un mensaje funcional. No debe invocar `After_envio_usuario_workflow` ni `Classgestionrespuesta.Reasigna_respuesta_envia_tarea_usuario`.

## Conclusion

La modernizacion es viable con complejidad media. El flujo no es una transicion por conector: el destino se identifica por el par `idUsuarioWorkflowDestino` e `idActividadDestino`. Por ello debe ser una operacion moderna especifica y no una extension de `ServicioTransicionTarea`.

Se pueden reutilizar el gate actual, el contexto autenticado, la proteccion de concurrencia, el token de version, la auditoria y componentes visuales de confirmacion. No se deben reutilizar directamente los contratos ni el ejecutor de Continuar flujo porque exigen `idConector`.

## Flujo legacy observado

```text
Centro de trabajo / Enviar a usuario
  -> ImageButtonEnviarUsuario_Click
  -> Valida_lista_usuarios_workflow_para_envio_tarea
     - tarea seleccionada
     - permiso CAMBIO_USUARIO
     - flujo/ruta abiertos
     - estado de respuesta
  -> Solicita_listado_usuarios_workflow_ruta
  -> seleccion usuario + actividad destino
  -> Button_tool_enviar_usuario_Click
  -> After_envio_usuario_workflow
  -> Terminar_Tarea_Workflow o reasignacion de respuesta
```

Referencias principales:

| Responsabilidad | Ubicacion |
| --- | --- |
| Comando de menu | `workflow/Webworkflow.aspx:683` |
| Apertura y validacion legacy | `workflow/Webworkflow.aspx.vb:2413` |
| Envio legacy | `workflow/Webworkflow.aspx.vb:2477` |
| Permisos, estado y respuesta | `workflow/Class_usuario_workflow.vb:527` |
| Consulta de usuarios destino | `workflow/Class_usuario_workflow.vb:629` |
| Bifurcacion legacy normal/reasignacion | `workflow/ClassWorkflow.vb:5386` |

La lista legacy devuelve usuarios de la ruta actual que cumplen `ESTADO_USUARIO = 1` y `UTIL_ASIGNA_TAREA = 1`, junto con la actividad asociada a cada usuario. Ese par define el destino real y se debe validar en servidor en cada ejecucion.

## Flujo moderno objetivo

```text
Enviar a usuario
  -> PreviewEnviarUsuario(idTarea) [solo lectura]
  -> muestra destinos validos y tokenVersion
  -> usuario confirma un destino
  -> EjecutarEnvioUsuario(idTarea, idUsuarioDestino, idActividadDestino, tokenVersion)
  -> adquiere lock y revalida todo el estado
  -> Terminar_Tarea_Workflow
  -> audita y actualiza la presentacion
```

La previsualizacion debe clasificar el estado de respuesta sin modificarlo:

| Estado | Comportamiento moderno |
| --- | --- |
| `YES` | Permite elegir y enviar al usuario. |
| Requiere confirmacion o radicado de respuesta | Bloquea con codigo funcional; no reasigna. |
| Error al consultar | Bloquea sin filtrar detalle tecnico. |

## Contrato propuesto

No se debe reutilizar `SolicitudTransicionWorkflow` porque requiere `idConector`. El contrato nuevo debe expresar el destino directo:

```text
SolicitudEnvioUsuarioWorkflow
  - IdTarea: Long
  - IdUsuarioWorkflowDestino: Integer
  - IdActividadDestino: Integer
  - TokenVersion: String
```

Endpoints ASMX paralelos en `WebServiceWorkflowModern.asmx`:

| Endpoint | Tipo | Proposito |
| --- | --- | --- |
| `PreviewEnviarUsuario(idTarea)` | Lectura | Valida contexto, tarea y requisitos; devuelve destinos y token. |
| `EjecutarEnvioUsuario(idTarea, idUsuarioDestino, idActividadDestino, tokenVersion)` | Escritura | Revalida, termina la tarea y devuelve resultado publico. |

## Componentes reutilizables

| Componente | Reutilizacion |
| --- | --- |
| `IWorkflowModernFeatureGate` y `WorkflowModernPresentationBootstrap` | Misma fuente de activacion; no crear un segundo gate. |
| `WorkflowPreviewSessionContextGate` | Contexto autenticado y conexiones del modulo. |
| `MySqlTareaWorkflowRepository` | Verificar que la tarea sigue activa y pertenece al usuario actual. |
| `MySqlTransicionConcurrencyGuard` | Lock por tarea y token contra doble envio. |
| `WorkflowLegacyAuditoriaAdapter` | Trazabilidad de la operacion moderna, con conector en cero. |
| Dialogo y actualizacion visual moderna | Reutilizables si se parametrizan para el nuevo contrato. |

## Componentes que requieren una variante especifica

| Componente existente | Motivo | Variante requerida |
| --- | --- | --- |
| `ServicioTransicionTarea` | Modela transiciones por conector. | `ServicioEnvioUsuarioTarea`. |
| `WorkflowLegacyExecutorAdapter` | Rechaza destinos sin `idConector` positivo. | `WorkflowLegacyEnvioUsuarioAdapter`. |
| `WorkflowLegacyRequisitosAdapter` | Trata respuesta pendiente como bloqueo generico y no representa la politica exclusiva de este flujo. | `WorkflowLegacyEnvioUsuarioRequisitosAdapter`. |
| `workflow-transition-ui.js` | Tiene endpoint y payload de conector codificados. | Adaptador JS propio para Enviar a usuario. |

El adaptador legacy nuevo debe llamar a `Terminar_Tarea_Workflow` con los parametros de actualizacion de interfaz desactivados, de modo que no dependa de `Page` ni manipule controles WebForms. La actualizacion de la pantalla queda a cargo de la capa moderna.

## Revalidaciones obligatorias al ejecutar

El navegador solo expresa la intencion. Dentro del lock, el servidor debe validar nuevamente:

1. Contexto autenticado valido y gate moderno habilitado.
2. Permiso `CAMBIO_USUARIO` calculado desde servidor.
3. Tarea activa, seleccionable y aun perteneciente al usuario actual.
4. Coincidencia del `tokenVersion` con el estado actual de la tarea.
5. Flujo o ruta abierta.
6. Respuesta en estado `YES`; si no, bloquear sin reasignar.
7. Usuario destino existente, activo y con `UTIL_ASIGNA_TAREA = 1`.
8. Relacion vigente del usuario destino con la actividad y ruta actuales.
9. Estado de notificacion de correo del usuario destino.

No se deben aceptar como autorizacion los valores de `Hidden_id_usuario_envio`, `Hidden_id_actividad_envio`, ni controles de la pagina legacy.

## Seguridad y compatibilidad

- La previsualizacion debe ser estrictamente de lectura: no cambia tarea, estado ni auditoria.
- La ejecucion debe usar datos tipados y consultas parametrizadas; no concatenar el criterio de busqueda en SQL.
- Las respuestas de ASMX no deben devolver mensajes internos, SQL, Session ni detalles de infraestructura.
- La auditoria debe registrar origen, usuario, destino, resultado, codigo funcional y referencia.
- La configuracion existente no se modifica. El gate `WorkflowCentroTrabajoModernActive` se conserva apagado salvo autorizacion expresa de prueba y se deja en `false` al finalizar.

## Secuencia recomendada de implementacion

1. Definir DTOs, codigos funcionales, repositorios e interfaces especificas de Enviar a usuario.
2. Implementar `PreviewEnviarUsuario` de solo lectura y su prueba de contrato.
3. Implementar el adaptador legacy directo a `Terminar_Tarea_Workflow` y el servicio de ejecucion con lock, token y auditoria.
4. Crear la interfaz moderna de seleccion y confirmacion sin interceptar otros comandos del Centro de trabajo.
5. Añadir pruebas unitarias, de integracion sin mutacion para preview y, solo con autorizacion, pruebas E2E controladas.

## Matriz minima de pruebas

| Caso | Resultado esperado |
| --- | --- |
| Usuario con permiso, tarea vigente y destino valido | La tarea se termina y se asigna al destino. |
| Sin `CAMBIO_USUARIO` | Bloqueo funcional. |
| Tarea de otro usuario, cerrada o desactualizada | Bloqueo funcional. |
| Token vencido o lock ocupado | Conflicto o operacion en progreso; no hay doble envio. |
| Usuario destino inactivo, fuera de ruta o sin `UTIL_ASIGNA_TAREA` | Destino no disponible. |
| Ruta o flujo cerrado | Bloqueo funcional. |
| Respuesta pendiente | Bloqueo funcional; no se reasigna respuesta. |
| Advertencia de correo | La tarea queda enviada y se informa advertencia sanitizada. |

## Exclusiones explicitas

- Reasignacion de respuesta y cualquier uso de `Reasigna_respuesta_envia_tarea_usuario`.
- Enviar a grupo.
- Continuar flujo y conectores de ruta/flujo.
- `WebFormPendientes.aspx` y su variante batch.
- Cambios funcionales en Gestion de Correspondencia.
