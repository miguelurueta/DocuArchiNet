# LISTA-MODERNA-DESTINO

- Ticket: DOC-12
- Cambio OpenSpec: doc-12-lista-moderna-destino
- Clasificacion: cross_cutting (Transversal)
## Servicios y reglas

`WorkflowModernPresentationBootstrap` llama `WorkflowPreviewSessionContextGate` para obtener el contexto de servidor y evalúa `IWorkflowModernFeatureGate` mediante `ConfiguracionWorkflowModernFeatureGate`. Su resultado se cachea por solicitud y se reduce a `true` o `false`; no es una autorización adicional.

El navegador solo llama `PreviewEnviarTarea(idTarea)` con credenciales de mismo origen. El ASMX conserva la autoridad sobre contexto, permisos, piloto, tarea, ruta y destinos. Los errores de red o contrato se muestran como mensajes controlados; las excepciones internas no se exponen. La selección no llama ninguna operación de ejecución, evento dinámico, correo, auditoría ni cambio de estado.
