# Jira Context - DOC-15

## Summary

BASE-ENVIAR-GRUPO

## Description

> # Prompt base obligatorio — Modernización de "Enviar a grupo"
> 
> Adjuntar este prompt al inicio de cada una de las etapas de implementación de esta carpeta.
> 
> ## ROL ESPERADO
> 
> Actúa como arquitecto y desarrollador senior de .NET Framework, VB.NET, ASP.NET Web Forms, MySQL y JavaScript legado. Trabaja de forma incremental, conserva la compatibilidad del sistema y no amplíes el alcance sin documentar antes la decisión.
> 
> ## OBJETIVO
> 
> Modernizar únicamente el comando **Enviar a grupo** de `workflow/Webworkflow.aspx` como una operación de envío directo a una actividad de la ruta.
> 
> La operación moderna debe recibir y ejecutar con `IdTarea`, `IdActividadDestino` y `TokenVersion`; debe conservar la semántica legacy del reenvío a grupo y coexistir con **Continuar flujo**.
> 
> ## ALCANCE
> 
> - Implementar solo la etapa solicitada por el prompt complementario en `prompts/`.
> - Reutilizar componentes transversales seguros: contexto autenticado, feature gate, token de versión, `GET_LOCK`, auditoría, motor legacy, confirmación y actualización de presentación.
> - Reutilizar el `IWorkflowModernFeatureGate`, `WorkflowModernPresentationBootstrap` y `WebServiceWorkflowModern.asmx` existentes; la capacidad de grupo se integra como una operación del mismo límite moderno.
> - Mantener el fallback Web Forms cuando la capacidad moderna esté inhabilitada.
> - Usar el documento `00-exploracion-arquitectura-envio-grupo.md` como referencia de decisiones técnicas.
> 
> ## RESTRICCIONES CRITICAS
> 
> - `Enviar a grupo` es un reenvío directo a actividad (`IdActividadDestino`), no una transición por conector (`IdConector`).
> - No crear conectores ficticios ni relajar la regla `IdConector > 0` del flujo existente **Continuar flujo**.
> - No crear una segunda configuración, bandera, fuente de evaluación ni gate para `Enviar a grupo`; cualquier evaluación de habilitación usa la fuente existente y conserva el comportamiento fail-closed.
> - No modificar contratos, endpoints, destinos ni comportamiento de `PreviewEnviarTarea`, `EjecutarEnvioTarea` o `ServicioTransicionTarea`.
> - No usar un destino recibido del navegador como autorización; revalidar en servidor permiso, tarea, token, ruta y destino.
> - El preview solo puede ejecutar consultas `SELECT`; no puede modificar tarea, estado, auditoría ni eventos del motor.
> - El ASMX no puede manipular controles Web Forms ni invocar handlers de página; la mutación final sigue en `Terminar_Tarea_Workflow` mediante un adaptador específico.
> - No añadir la validación de respuesta radicada sin aprobación funcional explícita: el flujo legacy actual de envío a grupo no la aplica.
> - No ejecutar E2E autenticado, pruebas de carga ni activar gates sin autorización explícita del ambiente y las cuentas de prueba.
> - Ninguna etapa puede cambiar la configuración de habilitación. Si una prueba autorizada llegara a modificarla, se debe restaurar `WorkflowCentroTrabajoModernActive=false` y listas de usuarios/grupos vacías antes de terminar.
> - No imprimir ni guardar credenciales, cookies ni cadenas de conexión.
> - Mantener los cambios acotados al alcance solicitado; no refactorizar componentes no relacionados.
> 
> ## CRITERIOS DE ACEPTACION
> 
> - La nueva operación usa `IdActividadDestino` y nunca requiere un conector artificial.
> - Con gate inactivo, el botón conserva el postback legacy de `Enviar a grupo`.
> - Con gate activo y autorización válida, preview y ejecución validan `Cambio_Ruta`, tarea activa, token, ruta/flujo/actividad abiertos y pertenencia del destino a la ruta.
> - Una ejecución concurrente o con token vencido no produce una segunda transición.
> - La operación registra auditoría sanitizada con mecanismo distinguible `ASMX_ENVIO_GRUPO`.
> - **Continuar flujo** conserva sus endpoints, payload `IdConector`, validaciones y pruebas actuales sin regresión.
> - Los errores públicos no exponen SQL, Session, credenciales ni excepciones internas.
> 
> ## PRUEBAS OBLIGATORIAS
> 
> - Agregar o actualizar pruebas automatizadas de contratos y JavaScript para el área afectada.
> - Ejecutar las pruebas unitarias/CJS afectadas y reportar comando, resultado y archivos cubiertos.
> - Ejecutar la compilación MSBuild del proyecto afectado cuando esté disponible; si la solución no puede compilarse localmente, documentar causa y una verificación manual reproducible.
> - Cubrir, cuando corresponda a la etapa: permiso denegado, ruta/flujo/actividad cerrados, destino fuera de ruta, aprobación pendiente, token vencido, concurrencia, fallback legacy y no regresión de continuar flujo.
> - No sustituir estas evidencias por E2E autenticado o carga no autorizados.
> 
> ## DOCUMENTACION TECNICA
> 
> - Actualizar el documento de exploración o el artefacto OpenSpec aplicable cuando cambie una decisión, contrato o requisito.
> - Cada etapa implementada crea o actualiza exclusivamente su paquete en `Doc/Actualizacion/workflow/TerminarGrupo/<NN>-<slug>/` con `00-indice.md`, `01-arquitectura.md`, `02-contrato.md`, `03-flujo-y-seguridad.md`, `04-pruebas-y-evidencia.md` y `Diagramas/` cuando corresponda.
> - Documentar endpoints, payloads, códigos de bloqueo, mecanismos de auditoría, configuración de gate y rollback cuando sean introducidos.
> - Registrar archivos modificados, supuestos y riesgos residuales en el resultado de la etapa.
> 
> ## ENTREGABLE FINAL
> 
> Entregar una respuesta breve y verificable con:
> 
> 1. Cambios implementados y su relación con el objetivo.
> 2. Archivos modificados.
> 3. Pruebas y compilación ejecutadas, con resultados.
> 4. Documentación actualizada.
> 5. Riesgos, limitaciones o decisiones pendientes.
> 
> No continuar hacia una etapa posterior si la etapa actual no cumple sus criterios de aceptación o si falta una decisión funcional.

## Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: BASE, ENVIAR, GRUPO
