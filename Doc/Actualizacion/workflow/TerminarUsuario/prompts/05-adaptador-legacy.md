# 05 — Adaptador legacy de envío directo a usuario

## ROL ESPERADO

Actúa como desarrollador senior responsable del límite entre arquitectura moderna y motor Workflow legacy.

## OBJETIVO

Crear un adaptador exclusivo para `ENVIO_USUARIO_DIRECTO` que reutilice `ClassWorkflow.Terminar_Tarea_Workflow` sin exponer el motor al ASMX, Application o JavaScript.

## RESTRICCIONES CRÍTICAS

- Lee y aplica `prompts/00-contexto-obligatorio.md`.
- Solo este adaptador puede invocar `Terminar_Tarea_Workflow` para la operación moderna de usuario.
- No trasladar SQL, cambios de estado, firma, balanceo, correo o eventos dinámicos a capas nuevas.
- No modificar ni relajar `IdConector > 0` de `WorkflowLegacyExecutorAdapter`, usado por Continuar flujo.
- No manipular controles Web Forms ni requerir `Page`; el ASMX no ejecuta handlers de página.
- No crear motor, transacción o implementación nueva de `Cambia_Estado`.
- Prohibido invocar `After_envio_usuario_workflow`, `Reasigna_respuesta_envia_tarea_usuario` o métodos batch de Pendientes.

## REQUISITOS POSITIVOS

1. Aceptar solo contexto, tarea y destino directo previamente reautorizados por el servicio.
2. Invocar `Terminar_Tarea_Workflow` con usuario y actividad destino, sin conector ni identificadores de flujo.
3. Usar `Page = Nothing` y deshabilitar actualización de controles de interfaz; preservar los eventos dinámicos que el motor requiera.
4. Usar el estado de notificación resuelto por el servidor, nunca por el navegador.
5. Normalizar éxito, advertencias de correo/evento, bloqueo funcional y excepción técnica en contratos públicos seguros.

## CONTRATO DETALLADO

- Entrada: `ContextoModuloWorkflow` válido, `TareaWorkflow` activa y `DestinoEjecucionWorkflow` con `TipoTransicion=ENVIO_USUARIO_DIRECTO`, usuario y actividad positivos, notificación ya resuelta.
- Prohibido en entrada: `IdConector` como requisito, `Page`, controles Web Forms, Session o destino no revalidado.
- Salida: `ResultadoEjecucionWorkflow` con `Exito`, `EstadoFinal`, `CodigoBloqueo`, `MensajeFuncional`, `EsReintentable` y advertencias sanitizadas.
- Invariante: este adaptador no modifica el contrato de `IWorkflowLegacyExecutor` de Continuar flujo; si requiere puerto nuevo, debe ser exclusivo y documentado.

## CRITERIOS DE ACEPTACIÓN

- El adaptador no cambia firmas ni comportamiento de transición por conector.
- No hay dependencia de `GridView`, `UpdatePanel`, `ModalPopupExtender` ni HTML.
- El motor legacy conserva autoridad única sobre cambios de estado y trazabilidad base.
- Respuesta pendiente no llega al adaptador: se bloquea antes en el servicio.
- El resultado no filtra excepción, SQL, credenciales ni Session.

## PRUEBAS OBLIGATORIAS

Agregar y ejecutar pruebas focales de éxito, bloqueo legacy, advertencia de correo/evento, excepción técnica y no invocación de los métodos de reasignación. Ejecutar MSBuild y registrar comandos; no ejecutar tareas reales ni E2E autenticado sin autorización.

## DOCUMENTACIÓN TÉCNICA

Crear o actualizar `Doc/Actualizacion/workflow/TerminarUsuario/05-adaptador-legacy/` con frontera legacy, parámetros permitidos, eventos, normalización de retorno, exclusiones y pruebas.

## ENTREGABLE FINAL

Entregar adaptador, pruebas, resultado de compilación, documentación y declaración de que `WorkflowLegacyExecutorAdapter` de Continuar flujo no fue alterado.

