## Why

INTERFAZ-MODERNA-DEVOLVER-USUARIO-ANTERIOR. Ver detalle funcional completo del ticket en la seccion Jira Details.

## What Changes

- Se genera automaticamente una propuesta OpenSpec basada en el issue DOC-37.
- Se formaliza una propuesta OpenSpec inicial derivada del ticket Jira.
- Se captura el resumen y la descripcion del ticket como punto de partida para refinement posterior.
- Se deja lista una base coherente para continuar con design, specs y tasks.

## Jira Details

> # 03 — Interfaz moderna oficial
> 
> ## ROL ESPERADO
> 
> Actúa como desarrollador senior de ASP.NET Web Forms y JavaScript accesible.
> 
> ## OBJETIVO
> 
> Conectar **Devolver a usuario anterior** a los endpoints modernos mediante una confirmación accesible y una única experiencia moderna para todo contexto Workflow válido.
> 
> ## CONTEXTO OBLIGATORIO
> 
> - Requiere 02 aprobado y endpoints de preview/ejecución disponibles.
> - Leer `00-contexto-obligatorio.md`, evidencia de 02, decisiones de 01 y componentes modernos existentes.
> - Habilita 04 únicamente si no comparte listeners, estado o payload con otras operaciones y no existe ruta legacy alcanzable.
> 
> ## REQUISITOS POSITIVOS
> 
> - Registrar la presentación de esta operación por contexto Workflow válido, sin evaluar `WorkflowCentroTrabajoModernActive` ni cambiar la política de feature gate de otras operaciones modernas.
> - Reemplazar el enlace legacy de **Usuario anterior** por un trigger con selector y adaptador JavaScript exclusivos. No debe invocar `inicializa_tipo_adjunto_documento`, controles ocultos ni `Button_tool_devolver_a_usuario`.
> - Desconectar o retirar el handler/postback legacy de esta operación y cualquier listener que abra actividades anteriores. El comando se ofrece en contexto válido; `PreviewDevolverUsuarioAnterior` determina elegibilidad y presenta bloqueo funcional cuando no existe destino.
> - Consumir `PreviewDevolverUsuarioAnterior` y `EjecutarDevolverUsuarioAnterior`; presentar exclusivamente el usuario y actividad históricos resueltos por servidor y conservar el token opaco recibido.
> - Reutilizar modal, foco, trampa de foco, teclado, Escape, ARIA, responsive, cancelación, doble clic y mensajes correlacionados.
> - Mientras ejecuta, deshabilitar confirmación y cierre que pueda abandonar un resultado pendiente; aplicar la política de timeout y recuperación aprobada en 01.
> - Tras éxito, actualizar solo tarea afectada, visor, contador, listado y scroll horizontal mediante componentes modernos existentes.
> 
> ## RESTRICCIONES CRÍTICAS
> 
> - No crear framework, bundler, selector de destinos, búsqueda, paginación, modal paralelo, banderas de habilitación ni autorización JavaScript.
> - No usar postbacks, `GridView`, `UpdatePanel`, `ModalPopupExtender`, SQL, handlers Web Forms, campos ocultos ni endpoints/payloads/selectores de Devolver a actividad anterior, Continuar flujo, Enviar a usuario o Enviar a grupo.
> - No incluir ni mostrar datos de respuestas.
> - No ejecutar E2E autenticada sin autorización explícita de ambiente y cuentas de prueba.
> 
> ## REGLAS DE ANTIRREGRESIÓN
> 
> - La devolución a usuario anterior y las demás operaciones no comparten selectores, eventos, estado ni requests.
> - La desactivación de la ruta legacy afecta únicamente Usuario anterior; Devolver a actividad anterior y las demás operaciones conservan sus contratos y triggers.
> 
> ## CRITERIOS DE ACEPTACIÓN
> 
> - El modal representa solo JSON autorizado con un único destino histórico de usuario.
> - Historial ausente, grupo, usuario retirado, token/historial desactualizado o auto-devolución muestran bloqueo y no proponen actividades alternativas.
> - No existe un recorrido de postback o fallback Web Forms alcanzable desde el comando Usuario anterior.
> - Éxito, bloqueo, timeout, cancelación y error mantienen la bandeja en estado consistente, accesible y con foco/restauración de scroll definidos.
> 
> ## PRUEBAS OBLIGATORIAS
> 
> Agregar pruebas CJS de bootstrap sin feature gate, trigger exclusivo, ausencia de postback legacy, contratos, eventos aislados, confirmación, historial ausente, grupo, usuario retirado, auto-devolución, token/historial cambiado, error, éxito, bloqueo, timeout, cancelación, doble clic, teclado, foco, Escape, responsive y restauración de bandeja. Ejecutar MSBuild y pruebas focales; registrar evidencia.  E2E reutilizando patron de DOC-36. Todas estas preubas son de caracter obligatorio.
> 
> ## DOCUMENTACIÓN TÉCNICA
> 
> Actualizar arquitectura, contrato, flujo, evidencia y diagramas necesarios con registro de presentación, selectores, ruta sustituida, UI, accesibilidad y relevo a 04.
> 
> ## ENTREGABLE FINAL
> 
> Reportar ticket, archivos UI, pruebas, compilación y evidencia de ruta moderna única/no regresión. No cambiar configuración de ambiente ni realizar QA autenticada sin autorización.

## Jira Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: ANTERIOR, DEVOLVER, INTERFAZ, MODERNA, USUARIO

## Capabilities

### New Capabilities
- `interfaz-moderna-devolver-usuario-anterior`: Capacidad derivada del ticket Jira para continuar el refinamiento funcional en OpenSpec.

### Modified Capabilities
- 

## Impact

- Nueva propuesta inicial en `openspec/changes/<changeName>/proposal.md`.
- Impacto funcional pendiente de refinamiento en los siguientes artefactos OpenSpec.

