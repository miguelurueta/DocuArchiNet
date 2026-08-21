## Context

DOC-28: BACKEND-ENVIAR-USUARIO-WORKFLOW

## Jira Details

> # 01 — Backend seguro de Enviar a usuario
> 
> ## ROL ESPERADO
> 
> Actúa como desarrollador senior VB.NET de capas Domain, Application e Infrastructure para Workflow, con experiencia en concurrencia MySQL y encapsulación de motores legacy.
> 
> ## OBJETIVO
> 
> Implementar el corte completo de servidor de **Enviar a usuario**: contrato exclusivo de usuario, autorización `CAMBIO_USUARIO`, `PreviewEnviarUsuario` de solo lectura y paginado, y `EjecutarEnvioUsuario` directo, seguro y auditable. Esta etapa integra deliberadamente preview y ejecución porque forman un único contrato y límite de seguridad.
> 
> ## CONTEXTO OBLIGATORIO
> 
> - Requiere que el ticket actual enlace este archivo y que la decisión vigente en `../00-exploracion-arquitectura-envio-usuario.md` esté aprobada, sin decisiones funcionales abiertas.
> - Leer `00-contexto-obligatorio.md`, la exploración y la arquitectura existente de `../Terminar/`.
> - La salida habilita 02; no implementa UI, activación ni liberación.
> 
> ## REQUISITOS POSITIVOS
> 
> - Crear DTOs, modelos, puertos y códigos públicos exclusivos de usuario, sin `IdConector`.
> - Exponer en el ASMX moderno existente `PreviewEnviarUsuario(idTarea, consulta?, cursor?, tamanoPagina?)` y `EjecutarEnvioUsuario({ IdTarea, IdUsuarioWorkflowDestino, IdActividadDestino, TokenVersion })`.
> - Calcular `CAMBIO_USUARIO` exclusivamente en servidor, fail-closed, y validar contexto, tarea activa, ruta/flujo abierto, respuesta permitida, usuario activo, actividad destino, pertenencia a ruta y `UTIL_ASIGNA_TAREA=1`.
> - Para el preview, reducir primero el universo a destinos usuario–actividad autorizados; después filtrar con `SELECT` parametrizado, límite de página en servidor, orden estable, cursor seguro, `hayMas`, token y datos mínimos de selección. El preview no produce escrituras.
> - Para la ejecución, adquirir `GET_LOCK` y releer/revalidar dentro del lock el contexto, permiso, tarea, token, ruta/flujo, respuesta `YES`, destino, `UTIL_ASIGNA_TAREA` y notificación.
> - Crear un puerto y adaptador exclusivos. Solo ese adaptador invoca una vez `ClassWorkflow.Terminar_Tarea_Workflow`, con `Page = Nothing` y sin conector.
> - Normalizar éxito, bloqueo, error reintentable y advertencias; auditar con `ASMX_ENVIO_USUARIO`, sin datos sensibles.
> 
> ## RESTRICCIONES CRÍTICAS
> 
> - No implementar UI, banderas de habilitación, directorio global, listas completas en cliente ni cambios de configuración.
> - No modificar `PreviewEnviarTarea`, `EjecutarEnvioTarea`, `ServicioTransicionTarea`, `SolicitudTransicionWorkflow`, `DestinoTransicionDto`, `IWorkflowLegacyExecutor`, `IdConector` ni Continuar flujo.
> - No invocar `After_envio_usuario_workflow`, `Reasigna_respuesta_envia_tarea_usuario`, `Cambia_Estado`, handlers Web Forms, batch de Pendientes ni ejecutor por conector.
> - Respuesta pendiente, token vencido, lock ocupado o destino retirado bloquean antes del motor legacy.
> - No ejecutar E2E autenticado, carga ni transiciones reales sin autorización explícita de ambiente y cuentas de prueba.
> 
> ## REGLAS DE ANTIRREGRESIÓN
> 
> - El preview solo ejecuta `SELECT`, no revela destinos fuera del conjunto autorizado y no elimina la revalidación de ejecución.
> - Existe un único punto mutante directo para Enviar a usuario; el navegador no autoriza el destino.
> - Continuar flujo conserva endpoints, payload `IdConector`, validaciones, adaptador y pruebas existentes.
> - Una falla de auditoría no revierte una transición ya confirmada y la concurrencia no duplica la transición.
> 
> ## CRITERIOS DE ACEPTACIÓN
> 
> - Contratos de preview y ejecución no reciben ni exponen `IdConector`.
> - Permiso, contexto, tarea, ruta/flujo, respuesta y destino se validan en servidor; la ejecución los revalida bajo lock.
> - Cursor inválido, filtro malformado, lista extensa, token vencido, respuesta pendiente, usuario fuera de ruta o destino inactivo generan resultado público seguro, sin fuga ni escritura indebida.
> - Dos solicitudes concurrentes no terminan dos veces la tarea; los errores públicos no exponen SQL, Session, credenciales ni excepciones internas.
> 
> ## PRUEBAS OBLIGATORIAS
> 
> Agregar y ejecutar pruebas focales para permiso, preview sin escritura, respuesta pendiente, destino inactivo/fuera de ruta, `UTIL_ASIGNA_TAREA=0`, filtro parametrizado, cursor, límite, orden, lista sintética extensa, solicitud inválida, token, lock ocupado, advertencia correo/evento, auditoría fallida y éxito simulado. Ejecutar MSBuild disponible y pruebas afectadas; registrar comando, resultado, cobertura y limitaciones. No E2E.
> 
> ## DOCUMENTACIÓN TÉCNICA
> 
> Actualizar `01-arquitectura.md`, `02-contrato.md`, `03-flujo-y-seguridad.md` y `04-pruebas-y-evidencia.md` bajo `../01-implementacion-envio-usuario/` con endpoints, payloads, límites, privacidad, punto mutante, auditoría, estados/error, evidencia y relevo a 02.
> 
> ## ENTREGABLE FINAL
> 
> Reportar ticket, cambios backend, archivos, pruebas, compilación, evidencia de no escritura del preview, trazabilidad sanitizada, riesgos y confirmación de no regresión de Continuar flujo. No implementar UI ni cambiar configuración de ambiente.

## Goals / Non-Goals

**Goals**
- Refinar alcance tecnico usando el contexto completo de Jira.
- Definir decisiones arquitectonicas, riesgos y plan de migracion.

**Non-Goals**
- Cambios fuera del alcance descrito por el ticket.

## Decisions

1. Las decisiones funcionales y tecnicas se completan durante `opsxj:refine`; no se inyectan politicas de otro perfil tecnologico.


## Risks / Trade-offs

- El refinamiento debe identificar compatibilidad, riesgos y limites del modulo afectado antes de iniciar cambios.

## Migration Plan

1. Completar y aprobar `refinement.md` antes de marcar tareas de implementacion.
2. Sincronizar cada decision con design, spec y tasks mediante `opsxj:refine --sync`.

## Open Questions

- TBD
