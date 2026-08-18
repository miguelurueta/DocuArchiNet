# 07 — Gate existente, auditoría y rollback

## ROL ESPERADO

Actúa como arquitecto de release y calidad para Workflow legacy, habilitación fail-closed y despliegues reversibles.

## OBJETIVO

Integrar Enviar a usuario al gobierno, auditoría y rollback de la modernización existente sin crear configuración paralela ni cambiar el estado operativo del gate.

## RESTRICCIONES CRÍTICAS

- Lee y aplica `prompts/00-contexto-obligatorio.md`.
- Reutilizar exclusivamente `IWorkflowModernFeatureGate`, `WorkflowCentroTrabajoModernActive` y bootstrap de Presentation existentes.
- No crear opt-in, bandera, appSetting, fuente de configuración, ASMX ni evaluación paralela.
- No activar, desactivar ni editar configuración. Una operación futura autorizada termina con `WorkflowCentroTrabajoModernActive=false` y usuarios/grupos vacíos.
- No guardar en auditoría SQL, Session, token, payloads, documentos, credenciales ni datos personales innecesarios.
- No revertir transiciones confirmadas con SQL, JavaScript o llamadas directas a `Cambia_Estado`.

## REQUISITOS POSITIVOS

1. Asegurar que preview, ejecución y bootstrap de usuario consuman y revaliden el gate existente de forma fail-closed.
2. Registrar auditoría sanitizada con `Canal=MODERNO` y `Mecanismo=ASMX_ENVIO_USUARIO`, incluyendo tarea, ruta, origen, actividad destino, resultado, código y duración; conector en cero cuando aplique.
3. Diferenciar auditoría de usuario de la transición por conector sin cambiar su contrato.
4. Registrar bloqueo de respuesta pendiente con código público sin datos de la respuesta.
5. Documentar rollback: gate inactivo hace que nuevos intentos usen el postback legacy, sin migración ni reversión de datos.

## SECUENCIA FUNCIONAL

1. El bootstrap consulta la evaluación existente y solo enlaza la experiencia moderna de usuario si está activa.
2. `PreviewEnviarUsuario` y `EjecutarEnvioUsuario` reevalúan el mismo gate antes de resolver o ejecutar.
3. Si está inactivo, el cliente conserva postback legacy y ASMX devuelve bloqueo funcional sin fallback automático.
4. Con rollback autorizado, las transiciones confirmadas permanecen; nuevos intentos retornan a Web Forms.

## CRITERIOS DE ACEPTACIÓN

- Existe una sola fuente de habilitación para página y endpoints.
- Configuración ausente o inválida bloquea los endpoints sin fallback ASMX automático.
- El rollback no necesita migración ni altera tareas ya terminadas.
- Auditoría útil para soporte sin información sensible ni registro de reasignación.

## PRUEBAS OBLIGATORIAS

Agregar y ejecutar pruebas de gate activo/inactivo/inválido, bloqueo de llamada directa, serialización sanitizada de auditoría, bloqueo por respuesta pendiente y rollback visual al postback legacy. Ejecutar MSBuild y pruebas focales; no activar gate ni ejecutar E2E/carga sin autorización.

## DOCUMENTACIÓN TÉCNICA

Crear o actualizar `Doc/Actualizacion/workflow/TerminarUsuario/07-gate-auditoria/` con fuente única de gate, alcance, auditoría, rollback, responsable operativo, matriz de pruebas y diagrama de decisión.

## ENTREGABLE FINAL

Entregar cambios de integración, pruebas, compilación, documentación de rollback y confirmación de que no se creó ni activó una segunda configuración.

