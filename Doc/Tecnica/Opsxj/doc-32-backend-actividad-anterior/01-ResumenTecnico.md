# BACKEND-ACTIVIDAD-ANTERIOR

- Ticket: DOC-32
- Cambio OpenSpec: doc-32-backend-actividad-anterior
- Clasificacion: cross_cutting (Transversal)
## Objetivo

Entregar el corte servidor de **Devolver a actividad anterior** para Workflow sin reutilizar los contratos de envío. La solución expone un preview de solo lectura y una ejecución autorizada, ambos con identidad contextual de la arista y resultado público saneado.

## Alcance y compatibilidad

- [x] Servicios afectados: capacidad exclusiva `Devolver`, DTOs, puertos, repositorio MySQL, cursor, guard de concurrencia, adaptador legacy, auditoría y `WebServiceWorkflowModern`.
- [x] Se agregaron `PreviewDevolverActividad` y `EjecutarDevolverActividad`; el navegador solo entrega identificadores mínimos y el servidor vuelve a resolver contexto, permiso y destino.
- [x] Se preservan `PreviewEnviarTarea`, `EjecutarEnvioTarea`, servicios de envío, Usuario anterior, el guard tokenizado existente y el gate `WorkflowCentroTrabajoModernActive`.
- [x] No se modificaron páginas, controles ni scripts WebForms, ni configuración de ambiente.
- [x] Reversa: retirar los componentes y endpoints exclusivos de DOC-32; no requiere migración ni cambios de datos o configuración.
