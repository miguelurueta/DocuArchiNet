## Why

INTERFAZ-MODERNA-DEVOLVER-TAREA. Ver detalle funcional completo del ticket en la seccion Jira Details.

## What Changes

- Se genera automaticamente una propuesta OpenSpec basada en el issue DOC-33.
- Se formaliza una propuesta OpenSpec inicial derivada del ticket Jira.
- Se captura el resumen y la descripcion del ticket como punto de partida para refinement posterior.
- Se deja lista una base coherente para continuar con design, specs y tasks.

## Jira Details

> # 02 — Interfaz moderna oficial
> 
> ## ROL ESPERADO
> 
> Actúa como desarrollador senior de ASP.NET Web Forms y JavaScript accesible.
> 
> ## OBJETIVO
> 
> Conectar **Devolver a actividad anterior** a los endpoints modernos, con búsqueda paginada, confirmación accesible y una única experiencia moderna para todo contexto Workflow válido.
> 
> ## CONTEXTO OBLIGATORIO
> 
> - Requiere 01 aprobado y endpoints de preview/ejecución disponibles.
> - Leer `00-contexto-obligatorio.md`, `../Exploracion/`, evidencia de 01 y componentes modernos existentes.
> - Habilita 03 únicamente si no comparte listeners, estado o payload con otras operaciones y no existe recorrido legacy alcanzable.
> 
> ## REQUISITOS POSITIVOS
> 
> - Registrar la presentación de esta operación por contexto Workflow válido, sin evaluar `WorkflowCentroTrabajoModernActive` ni cambiar la política de feature gate de otras operaciones modernas.
> - Reemplazar el enlace legacy `D-TASK-ANT` por un trigger con selector y adaptador JavaScript exclusivos. No debe invocar `inicializa_tipo_adjunto_documento`, controles ocultos ni `Button_tool_devolver_a_actividades_anterior`.
> - Desconectar o retirar el handler/postback legacy y cualquier listener que abra el modal Web Forms de actividades anteriores. Devolver a Usuario anterior conserva su propio trigger y ruta.
> - Consumir `PreviewDevolverActividad` y `EjecutarDevolverActividad`; aplicar término mínimo, debounce, páginas, descarte de respuesta obsoleta e invalidación de selección antigua.
> - Representar solo JSON autorizado, incluido `IdConector` contextual; la UI no deduce ni transforma una identidad de Ruta en una de Flujo.
> - Reutilizar modal, foco, trampa de foco, teclado, Escape, ARIA, responsive, cancelación, doble clic y mensajes correlacionados.
> - Mientras ejecuta, deshabilitar confirmación y cierre que pueda abandonar un resultado pendiente; aplicar política de timeout y recuperación documentada en la evidencia backend.
> - Tras éxito, actualizar solo tarea afectada, visor, contador, listado y scroll horizontal mediante componentes modernos existentes.
> 
> ## RESTRICCIONES CRÍTICAS
> 
> - No crear framework, bundler, modal paralelo, banderas de habilitación ni autorización JavaScript.
> - No usar postbacks, `GridView`, `UpdatePanel`, `ModalPopupExtender`, SQL, handlers Web Forms, campos ocultos ni endpoints/payloads/selectores de Continuar flujo, Enviar a usuario, Enviar a grupo o Usuario anterior.
> - No incluir ni mostrar datos de respuestas.
> -NO  ejecutar E2E autenticada sin autorización explícita de ambiente y cuentas de prueba.
> 
> ## REGLAS DE ANTIRREGRESIÓN
> 
> - La devolución y las demás operaciones no comparten selectores, eventos, estado ni requests.
> - La sustitución de la ruta legacy afecta solo Devolver a actividad anterior; las demás operaciones conservan contratos y triggers.
> 
> ## CRITERIOS DE ACEPTACIÓN
> 
> - El modal representa solo JSON autorizado, nunca materializa la lista completa y no expone actividades o conectores de otro contexto.
> - Búsqueda, paginación, vacío, error, cursor inválido, respuesta obsoleta, bloqueo, timeout y cancelación restauran estado sin iniciar una transición.
> - No existe recorrido de postback o fallback Web Forms alcanzable desde el comando Devolver a actividad anterior.
> - Éxito, bloqueo y error mantienen la bandeja en estado consistente, accesible y con foco/restauración de scroll definidos.
> 
> ## PRUEBAS OBLIGATORIAS
> 
> Agregar pruebas CJS de bootstrap sin feature gate, trigger exclusivo, ausencia de postback legacy, contratos Ruta/Flujo aislados, búsqueda, debounce, páginas, respuesta obsoleta, vacío, error, selección, éxito, bloqueo, timeout, cancelación, doble clic, teclado, foco, Escape, responsive y bloqueo durante ejecución. Ejecutar MSBuild y pruebas focales; registrar evidencia. No E2E sin autorización.
> 
> ## DOCUMENTACIÓN TÉCNICA
> 
> Actualizar arquitectura, contrato, flujo, evidencia y diagramas necesarios con registro de presentación, selectores, ruta sustituida, UI, accesibilidad y relevo a 03.
> 
> ## ENTREGABLE FINAL
> 
> Reportar ticket, archivos UI, pruebas, compilación y evidencia de ruta moderna única/no regresión. No cambiar configuración de ambiente ni realizar QA autenticada sin autorización.

## Jira Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: ACTIVIDAD, ANTERIOR, DEVOLVER, INTERFAZ

## Capabilities

### New Capabilities
- `interfaz-moderna-devolver-tarea`: Capacidad derivada del ticket Jira para continuar el refinamiento funcional en OpenSpec.

### Modified Capabilities
- 

## Impact

- Nueva propuesta inicial en `openspec/changes/<changeName>/proposal.md`.
- Impacto funcional pendiente de refinamiento en los siguientes artefactos OpenSpec.

