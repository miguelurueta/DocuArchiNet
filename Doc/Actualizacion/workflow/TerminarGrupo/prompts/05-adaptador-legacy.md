# 05 — Adaptador legacy de reenvío directo

## ROL ESPERADO

Actúa como desarrollador senior responsable del límite entre la arquitectura moderna y el motor Workflow legacy.

## OBJETIVO

Crear un adaptador exclusivo para `ENVIO_GRUPO_DIRECTO` que reutilice `ClassWorkflow.Terminar_Tarea_Workflow` sin exponer el motor al ASMX, Application o JavaScript.

## RESTRICCIONES CRITICAS

- Leer y aplicar `prompts/00-contexto-obligatorio.md`.
- Solo el adaptador puede invocar `Terminar_Tarea_Workflow`; no trasladar SQL, cambios de estado, firma, balanceo, correo o eventos dinámicos a capas nuevas.
- No modificar ni relajar la exigencia `IdConector > 0` de `WorkflowLegacyExecutorAdapter` usado por continuar flujo.
- No manipular controles Web Forms ni requerir `Page`; el ASMX no puede ejecutar handlers de página.
- No crear un segundo motor, transacción o implementación de `Cambia_Estado`.

## REQUISITOS POSITIVOS

1. Aceptar solo tarea y destino directo previamente reautorizados por el servicio.
2. Invocar `Terminar_Tarea_Workflow` sin conector y con identificadores de flujo en cero, preservando la semántica legacy de enviar a grupo.
3. Usar `Page = Nothing` y deshabilitar actualización de controles, manteniendo eventos dinámicos legacy requeridos por el motor.
4. Normalizar resultado, advertencias de evento/correo, bloqueo funcional y excepción técnica a contratos públicos.

## CONTRATO DETALLADO

- Entrada: `ContextoModuloWorkflow` válido, `TareaWorkflow` activa y `DestinoEjecucionWorkflow` con `TipoTransicion=ENVIO_GRUPO_DIRECTO`, `IdActividadDestino>0` y notificación resuelta en servidor.
- Prohibido en entrada: `IdConector` como requisito, `Page`, controles Web Forms, Session o datos de destino enviados sin revalidación.
- Salida: `ResultadoEjecucionWorkflow` con `Exito`, `EstadoFinal`, `CodigoBloqueo`, `MensajeFuncional`, `EsReintentable` y advertencias sanitizadas.
- Invariante: el adaptador de grupo no comparte ni modifica el contrato de `IWorkflowLegacyExecutor` usado por continuar flujo; si se requiere un puerto nuevo, debe ser específico y documentado.

## CRITERIOS DE ACEPTACION

- El adaptador no cambia las firmas ni el comportamiento de la transición por conector.
- No hay dependencia de `GridView`, `UpdatePanel`, `ModalPopupExtender` ni HTML.
- El motor legacy sigue siendo el único autor de cambios de estado y trazabilidad base.
- El resultado no filtra excepción, SQL, credenciales o Session.

## PRUEBAS OBLIGATORIAS

Agregar y ejecutar pruebas focales de éxito, bloqueo legacy, advertencia de correo/evento y excepción técnica. Ejecutar `msbuild .\GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug`, registrar código de salida y comandos de prueba; si no está disponible, registrar limitación/QA reproducible. No ejecutar tareas reales ni E2E autenticado sin autorización.

## DOCUMENTACION TECNICA

Actualizar exclusivamente `Doc/Actualizacion/workflow/TerminarGrupo/01-implementacion-envio-grupo/`: registrar frontera legacy, parámetros y normalización de retorno en `01-arquitectura.md` y `03-flujo-y-seguridad.md`, y riesgos y pruebas en `04-pruebas-y-evidencia.md`. No crear una carpeta documental para esta etapa.

## ENTREGABLE FINAL

Entregar adaptador, pruebas, resultado de compilación, documentación y declaración de que `WorkflowLegacyExecutorAdapter` de continuar flujo no fue alterado.
