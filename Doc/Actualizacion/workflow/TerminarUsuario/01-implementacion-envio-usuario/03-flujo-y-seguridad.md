# Flujo, seguridad y relevo

- Ticket: DOC-28
- Cambio OpenSpec: doc-28-backend-enviar-usuario-workflow
- Clasificación: cross_cutting

## Preview de solo lectura

El ASMX reconstruye el contexto autenticado y calcula `PuedeCambioUsuario` desde `SolicitaPermisosUsuarioWorkflow`; solo el valor `1` del índice 18 habilita el permiso. Contexto, permiso o consulta inválidos se deniegan por defecto.

Después, `ServicioEnvioUsuarioTarea.Previsualizar` normaliza la solicitud, relee la tarea y pide al repositorio únicamente el conjunto usuario–actividad permitido. El repositorio valida tarea activa, ruta abierta y, cuando aplica, flujo y actividad de flujo abiertos; restringe a la ruta actual, `ESTADO_USUARIO=1` y `UTIL_ASIGNA_TAREA=1`, y solo entonces aplica filtro, cursor y límite parametrizados.

Preview no toma lock, no audita, no invoca el motor legacy y no escribe tarea, estado ni respuesta.

## Ejecución y revalidación

El servicio valida la forma de la solicitud y toma `GET_LOCK` por tarea y token. Dentro del lease, en este orden, vuelve a comprobar:

1. `CAMBIO_USUARIO` efectivo desde servidor.
2. Tarea activa y coincidencia de `TokenVersion`.
3. Ruta, flujo y actividad de flujo abiertos.
4. Par usuario–actividad vigente, usuario activo, pertenencia a ruta y `UTIL_ASIGNA_TAREA=1`.
5. Política de respuesta con `Verifica_respuesta_radicado_sin_respuesta = YES`.
6. Configuración de notificación ya resuelta junto con el destino.

Solo `WorkflowLegacyEnvioUsuarioExecutorAdapter` cruza la frontera mutante. Llama una vez a `ClassWorkflow.Terminar_Tarea_Workflow` con `Page = Nothing`, conector `0` y actualización de interfaz desactivada. No usa `After_envio_usuario_workflow`, `Reasigna_respuesta_envia_tarea_usuario`, `Cambia_Estado`, handlers Web Forms, Pendientes/batch ni ejecutores por conector.

## Auditoría, compatibilidad y relevo

La auditoría conserva el formato histórico y agrega el mecanismo `ASMX_ENVIO_USUARIO`. Si esa escritura adicional falla después de un envío exitoso, se agrega una advertencia sanitizada sin revertir la transición.

DOC-28 no consulta ni modifica `WorkflowCentroTrabajoModernActive`; no existe fallback por gate en este corte de servidor. Tampoco intercepta `ImageButtonEnviarUsuario` ni actualiza la presentación. La etapa 02 debe consumir los endpoints con el payload mínimo, mantener el navegador como expresión de intención y definir su propia experiencia accesible sin alterar Continuar flujo.

## DOC-29 — Recorrido de interfaz, accesibilidad y aislamiento

1. El bootstrap de usuario se registra aun cuando `WorkflowCentroTrabajoModernActive` está en `false`; no lo lee ni lo modifica.
2. `workflow-user-send-trigger` abre un modal con foco inicial en búsqueda. El modal atrapa Tab/Shift+Tab, Escape, cierre y fondo, y devuelve el foco al disparador.
3. Preview usa el cursor opaco emitido por servidor. Debounce, `AbortController` cuando está disponible y un contador monotónico descartan resultados tardíos.
4. Elegir un destino emite `workflow:user-destination-selected`; la confirmación muestra únicamente datos JSON del preview y evita doble ejecución mediante `ConfirmationDialog`.
5. Cancelar, bloquear, fallar o invalidar una búsqueda no invoca postback, campos ocultos, reasignación de respuesta ni `After_envio_usuario_workflow`.
6. Solo una ejecución exitosa correlacionada remueve la fila de la tarea, limpia visor/contexto, decrementa contador y anuncia `workflow-user-send-success-message`.

`workflow-user-send-*` y `WorkflowUserSendUi` no comparten listeners, selectores, estado de solicitudes ni payload de `WorkflowTransitionUi`/`workflow:destination-selected`. Se comparten únicamente estilos y el componente genérico de confirmación; los temporizadores de mensaje están asociados a cada elemento de éxito.

La reversión es restaurar el cambio versionado completo. No requiere migración de datos ni modificación de appSettings; tampoco revierte transiciones ya confirmadas por el servidor.
