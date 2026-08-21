# BACKEND-ENVIAR-USUARIO-WORKFLOW

- Ticket: DOC-28
- Cambio OpenSpec: doc-28-backend-enviar-usuario-workflow
- Clasificacion: cross_cutting (Transversal)
## Objetivo

Implementar el backend seguro de Enviar a usuario para una tarea Workflow. El destino es el par usuario–actividad y se resuelve nuevamente en servidor antes de tocar el motor legacy.

La solución incorpora los endpoints ASMX `PreviewEnviarUsuario` y `EjecutarEnvioUsuario`, contratos dedicados sin `IdConector`, permiso `CAMBIO_USUARIO` fail-closed, preview de solo lectura, lock por tarea/token y auditoría `ASMX_ENVIO_USUARIO`.

## Alcance y compatibilidad

- [x] Servicios afectados: `WebServiceWorkflowModern`, contexto de sesión, repositorio de usuario, servicio de aplicación, adaptadores legacy y auditoría.
- [x] No hay páginas, controles ni scripts afectados; no se activa ni modifica un feature gate.
- [x] Se preservan `PreviewEnviarTarea`, `EjecutarEnvioTarea`, `ServicioTransicionTarea` y el contrato de Continuar flujo por conector.
- [x] Reversa: retirar los componentes y endpoints exclusivos de DOC-28; no requiere migración de datos ni cambios de configuración.
