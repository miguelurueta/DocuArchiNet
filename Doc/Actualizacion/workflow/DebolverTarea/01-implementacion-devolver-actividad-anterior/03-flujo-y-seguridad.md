# Flujo, seguridad y relevo

- Ticket: DOC-32
- Cambio OpenSpec: `doc-32-backend-actividad-anterior`
- Clasificación: `cross_cutting`

## Preview de solo lectura

1. `WebServiceWorkflowModern` reconstruye el contexto de sesión y calcula en servidor el permiso `DEVOLVER_TAREA_WORKFLOW`; un contexto inválido o permiso ausente se deniega por defecto.
2. `ServicioDevolverActividad.Previsualizar` normaliza solicitud, término y página; relee el snapshot activo de tarea asignado al usuario.
3. `MySqlDevolverActividadRepository` identifica Ruta o Flujo y construye únicamente las aristas entrantes autorizadas con `SELECT` parametrizados.
4. Solo después aplica término, límite, orden determinista y continuación; si existe otra página, `DevolverActividadCursorCodec` protege la última clave.

Preview no toma lock, no llama al motor legacy, no registra auditoría y no escribe tarea, estado, eventos ni datos de negocio. Las huellas antes/después de la E2E autorizada confirman esta propiedad.

## Ejecución, exclusión y revalidación

1. El endpoint recibe únicamente tarea, conector y token de la respuesta vigente de preview.
2. `MySqlDevolverActividadConcurrencyGuard` solicita `GET_LOCK` con una clave derivada exclusivamente de `IdTarea`; los tokens distintos de la misma tarea compiten por el mismo recurso.
3. Dentro del lease se vuelven a leer tarea activa, permiso específico, token, tipo Ruta/Flujo y conector entrante. Un conector ajeno, retirado o de semántica distinta, token vencido, permiso retirado o tarea no disponible se bloquea antes del motor.
4. Solo `WorkflowLegacyDevolverActividadExecutorAdapter` cruza la frontera mutante e invoca una vez `ClassWorkflow.Terminar_Tarea_Workflow` con `Page = Nothing`, actualización de interfaz y reasignaciones desactivadas, y eventos/notificación determinados por el destino reconstruido.
5. La auditoría `ASMX_DEVOLVER_ACTIVIDAD` registra resultado, código funcional, duración y referencia saneada. Si falla después de una transición exitosa, se conserva la transición y se agrega una advertencia pública.

La carrera de dos solicitudes se limita a una tarea descartable distinta: una transición efectiva es válida y la otra solicitud debe recibir un bloqueo controlado. No se permite doble mutación.

## Aislamiento, compatibilidad y reversa

Los componentes nuevos no invocan `Classgestionrespuesta`, verificaciones o reasignaciones de respuesta, `Activa_devolver_actividades_anteriores`, `Enviar_actividad_por_conector_flujo_de_trabajo_anterior`, postbacks, handlers WebForms ni modales legacy. Los endpoints de envío y el guard tokenizado previo no cambian.

DOC-32 no lee ni modifica `WorkflowCentroTrabajoModernActive`; al cierre de una corrida autorizada el gate sigue en `false`, con usuarios y grupos vacíos. La reversa consiste en retirar los componentes y endpoints exclusivos de DOC-32; no requiere migración de datos ni revertir transiciones ya confirmadas.

La siguiente etapa de UI debe tratar preview como información de selección, no como autorización. Debe enviar el payload mínimo, aceptar bloqueos de revalidación y conservar el aislamiento con Continuar flujo.
