## Why

CONFIRMACION-ESPECIALIZADA. Ver detalle funcional completo del ticket en la seccion Jira Details.

## What Changes

- Se genera automaticamente una propuesta OpenSpec basada en el issue DOC-13.
- Se formaliza una propuesta OpenSpec inicial derivada del ticket Jira.
- Se captura el resumen y la descripcion del ticket como punto de partida para refinement posterior.
- Se deja lista una base coherente para continuar con design, specs y tasks.

## Jira Details

> Prompt 05 — Confirmación especializada y envío asíncrono
> Rol esperado:
> Especialista UI/UX y desarrollador senior de ASP.NET Web Forms .NET Framework 4.6.1, JavaScript progresivo, accesibilidad, concurrencia e integración segura con workflows legacy.
>
> Contexto:
> - Repositorio: `D:\imagenesda\GestorDocumental\Desarrollo\old\oldanterior\GestionDocumental-Docuarchi.net`.
> - La lista moderna entrega un destino seleccionado desde `PreviewEnviarTarea`; la ejecución paralela se realiza exclusivamente por `webservice/WebServiceWorkflowModern.asmx` mediante `EjecutarEnvioTarea`.
> - La interfaz legacy de `workflow/Webworkflow.aspx`, sus modales, controles ocultos y el flujo de autorización vigente deben mantenerse funcionales.
> - Esta fase implementa Presentation en JavaScript/CSS e integra contratos existentes; no modifica el núcleo `Terminar_Tarea_Workflow`, `Cambia_Estado` ni reglas de autorización.
>
> Objetivo:
> Implementar un componente de confirmación especializado, reutilizable, accesible y reversible que presente un destino validado, solicite un único envío asíncrono al servidor y actualice la interfaz solo después de un éxito funcional confirmado.
>
> Restricciones críticas:
> - No debe ejecutarse ninguna regla crítica, autorización, validación de permisos, cambio de estado, firma, expediente, copia documental, balanceo ni evento dinámico solo en cliente.
> - No debe llamar controles ocultos, `Terminar_Tarea_Workflow`, `Cambia_Estado`, repositorios, SQL ni Session desde JavaScript o desde el componente Presentation.
> - No debe retirar tareas, limpiar visor, actualizar contador ni cerrar el modal antes de una respuesta `Exito` real y correlacionada del servidor.
> - No debe permitir doble envío, reintentos paralelos ni ejecutar con un destino, token o contexto diferente al recibido de la lista moderna.
> - No debe exponer HTML, SQL, credenciales, Session, excepciones internas, trazas ni datos personales innecesarios en mensajes visibles.
> - No debe retirar ni alterar el modal, autorización o envío legacy; no agregar bibliotecas UI nuevas ni recargar la página para confirmar o enviar.
> - No debe acoplarse a `Webworkflow.aspx`, IDs de controles legacy, `GridView_envia_flujo`, variables globales de una página, selectores particulares de Workflow ni texto fijo de una actividad.
> - No debe duplicarse el componente para otros módulos: las diferencias de caso de uso se resuelven mediante configuración, DTOs y callbacks tipados/documentados.
>
> Entrada:
> Destino seleccionado desde la lista moderna.
>
> Contrato técnico:
> - Entrada del componente: `idTarea`, `idConector`, `tokenVersion`, radicado/trámite, actividad origen, resumen visible de `DestinoTransicionDto`, requisitos y advertencias provenientes exclusivamente del preview del servidor.
> - Solicitud de ejecución: el adaptador Workflow llama async/await a `WebServiceWorkflowModern.EjecutarEnvioTarea(idTarea, idConector, tokenVersion)`; el ASMX delega a `ServicioTransicionTarea` en Application. El cliente no agrega ni infiere usuario, grupo, ruta, actividad, permisos o requisitos.
> - El endpoint revalida `IWorkflowModernFeatureGate`; si el piloto no está activo devuelve `WORKFLOW_MODERN_INACTIVE` como bloqueo funcional, sin transición ni fallback automático.
> - Respuesta: `ResultadoTransicionDto` con `Exito`, `EstadoFinal`, `MensajeFuncional`, `CodigoBloqueo`, `Advertencias`, `ActividadDestino`, `Destino`, `TokenVersion`, `ReferenciaAuditoria` y `EsReintentable`.
> - Estados Presentation: `confirmando`, `enviando`, `exito`, `bloqueo-funcional` y `error-tecnico-controlado`; cada uno define mensaje visible, foco, acciones habilitadas y recuperación.
> - Correlación: ignorar respuestas obsoletas, cancelar o inutilizar solicitudes anteriores cuando se cierre el modal y permitir el resultado visual solo si coincide con la tarea, conector y `tokenVersion` actualmente confirmados.
>
> Reutilización obligatoria:
> - Esta es una migración incremental del código Web Forms existente: no crear módulos ES, bundler, framework nuevo ni carpeta basada en convenciones de una aplicación nueva.
> - Crear el componente reusable en la ruta genérica legacy `js/java_general/ConfirmationDialog.js` y sus estilos aislados en `Styles/confirmation-dialog.css`.
> - Exponer una única API pública documentada en el namespace global existente: `ConfirmationDialog.open(config)` y `ConfirmationDialog.close()`.
> - `config` debe recibir únicamente datos y callbacks genéricos: `title`, `primaryLabel`, `cancelLabel`, `summaryFields`, `requirements`, `warnings`, `confirmationNotice`, `executionContext`, `execute`, `normalizeResult`, `onSuccess`, `onBlocked`, `onTechnicalError`, `onCancel`, `labels` y opciones de accesibilidad. No recibe ni consulta controles, Session ni globals legacy.
> - `executionContext` es opaco para el componente. `execute(executionContext)` retorna una promesa con el resultado propio del consumidor y `normalizeResult` lo convierte al contrato visual genérico `{ status: success|blocked|technical-error, message, warnings, canRetry, reference }`.
> - El componente administra renderizado, foco, Escape, foco atrapado, estado de envío, prevención de doble clic y mensajes; el consumidor administra integración de página, refresco de lista, visor, contador y reglas propias del módulo a través de callbacks.
> - Crear un adaptador de integración específico de Workflow en `js/workflow/workflow-transition-confirmation-integration.js`, responsable de convertir `DestinoTransicionDto`, `{ idTarea, idConector, tokenVersion }` y `ResultadoTransicionDto` al `config` y contrato visual genérico. No incorporar esa conversión dentro del componente.
> - El componente debe poder utilizarse por otro módulo que entregue el mismo contrato, sin copiar código ni depender de rutas, selectores o reglas de Workflow.
> - El componente no contiene los textos “Enviar tarea”, “La tarea actual quedará finalizada”, nombres de actividades ni referencias a tareas; esos textos y campos los entrega el adaptador Workflow mediante `config`.
>
> Contenido obligatorio:
> - La integración Workflow configura título: “Enviar tarea”.
> - La integración Workflow entrega campos: radicado, trámite, actividad origen, actividad destino, destinatario/grupo y mecanismo.
> - La integración Workflow entrega requisitos, advertencias y aviso visible: “La tarea actual quedará finalizada”.
> - La integración Workflow configura botón secundario: “Cancelar” y botón primario: “Enviar a [actividad destino]”.
>
> Comportamiento:
> 1. El botón primario llama async/await a EjecutarEnvioTarea.
> 2. Durante el envío, deshabilitar acciones, evitar doble clic y mostrar “Enviando tarea…”.
> 3. Si el servidor responde éxito, cerrar modales, retirar solo esa tarea, limpiar visor/contexto, actualizar contador y mostrar confirmación no intrusiva.
> 4. Si responde bloqueo funcional, mantener modal abierto, mostrar causa legible y restaurar acciones.
> 5. Si hay fallo técnico, no retirar tarea y mostrar mensaje seguro al usuario.
> 6. Si exige autorización adicional, integrar el flujo de autorización vigente o un adaptador moderno equivalente, sin alterar la regla de negocio.
>
> No implementar autorizaciones ni validaciones críticas solo en cliente.
>
> La confirmación es Presentation: utiliza DTOs del servicio y el adaptador Workflow invoca el ASMX; el ASMX es quien solicita la ejecución a Application. No puede llamar controles ocultos, Terminar_Tarea_Workflow, Cambia_Estado ni repositorios.
>
> Pruebas obligatorias:
> - Ejecutar compilación del proyecto o solución afectada con MSBuild/.NET Framework y registrar comando, resultado y limitaciones reales.
> - Agregar o ajustar pruebas JavaScript focales donde la infraestructura actual lo permita para el mapeo de DTOs, estados del modal, bloqueo de doble clic, correlación de respuesta y contrato de actualización visual.
> - Probar el componente reusable con un `config` simulado ajeno a Workflow para verificar que no depende de selectores, globals o textos del módulo.
> - Probar el adaptador de Workflow por separado para verificar la conversión de `DestinoTransicionDto`, callbacks y el contrato `EjecutarEnvioTarea`.
> - Ejecutar QA manual reproducible: cancelar, enviar con éxito, bloqueo funcional, error técnico, doble clic, cierre mientras hay solicitud, foco inicial, Escape, foco atrapado, teclado, ARIA, contraste y vista móvil/escritorio.
> - Confirmar mediante QA que la tarea solo se retira después del éxito real y que el modal/flujo legacy sigue disponible con la bandera moderna desactivada.
> - E2E automatizada no aplica si el repositorio no cuenta con infraestructura compatible para Web Forms; registrar la justificación y la evidencia de QA manual. Si existe infraestructura disponible, ejecutar el recorrido hasta éxito o bloqueo sin alterar el camino legacy.
>
> Documentación técnica:
> - Este prompt es autosuficiente: no depende de README ni de documentación externa para conocer su convención documental.
> - Raíz documental obligatoria, relativa a la raíz del repositorio: `Doc/Actualizacion/workflow/Terminar/05-confirmacion-especializada/`.
> - Estructura obligatoria del paquete:
>     `Doc/Actualizacion/workflow/Terminar/05-confirmacion-especializada/`
>     - `00-indice.md`
>     - `01-arquitectura.md`
>     - `02-contrato.md`
>     - `03-flujo-y-seguridad.md`
>     - `04-pruebas-y-evidencia.md`
>     - `Diagramas/`
> - `00-indice.md`: ticket, fecha, estado, alcance, archivos relacionados y resumen de cambios.
> - `01-arquitectura.md`: frontera Presentation/Application, componente genérico bajo `js/java_general/`, adaptador Workflow, bandera de activación, responsabilidades, dependencias y alternativas descartadas.
> - `02-contrato.md`: API `ConfirmationDialog.open/close`, configuración, `executionContext`, `normalizeResult`, callbacks, solicitud `EjecutarEnvioTarea`, `ResultadoTransicionDto`, estados UI, mensajes y correlación de solicitudes.
> - `03-flujo-y-seguridad.md`: secuencia selección → confirmación → envío → éxito/bloqueo/error → actualización visual; doble envío, autorización vigente, límites legacy, riesgos y rollback.
> - `04-pruebas-y-evidencia.md`: comandos, compilación, pruebas focales, QA accesible/responsive, E2E o justificación, resultados, limitaciones y evidencia.
> - `Diagramas/`: diagramas Mermaid o fuentes estructuradas de estados del modal, secuencia de envío y recuperación ante errores cuando correspondan.
> - Incluir una tabla con: función o selector, ruta, parámetros/DTO, responsabilidad, estado UI y dependencia legacy permitida.
> - El prompt fuente `05-confirmacion-especializada.md` permanece en `Doc/Actualizacion/workflow/Terminar/`; no crear documentación de implementación junto a él, en la raíz del repositorio ni en rutas alternativas sin justificarlo expresamente en el entregable.
>
> Criterios de aceptación:
> - La confirmación muestra datos provenientes del preview y una acción primaria contextual, sin inferir permisos ni reglas en JavaScript.
> - `ConfirmationDialog` funciona con un `config` ajeno a Workflow y no depende de selectores, globals, controles o textos fijos del módulo; la conversión de contexto queda en el adaptador de Workflow.
> - Solo se realiza una solicitud efectiva por combinación de tarea, conector y versión; doble clic, respuestas obsoletas o cierre del modal no producen transición duplicada.
> - Ante éxito real, se cierra la confirmación, se actualiza exclusivamente la tarea afectada y se muestra confirmación no intrusiva; ante bloqueo o error se mantiene contexto y se restauran acciones según el contrato.
> - El componente es accesible: foco inicial, foco atrapado, Escape, teclado, ARIA, contraste y mensajes de estado perceptibles.
> - La autorización adicional reutiliza el flujo vigente o su adaptador autorizado sin trasladar la regla de negocio al cliente.
> - Interfaz, envío y autorización legacy se preservan sin regresiones; compilación, pruebas focales y QA manual quedan registrados con evidencia.
>
> Entregable final:
> - Entregar el componente reusable en `js/java_general/ConfirmationDialog.js`, su CSS aislado en `Styles/confirmation-dialog.css`, el adaptador Workflow y los cambios de integración, indicando rutas, responsabilidades, estados UI, contrato de datos y dependencias.
> - Entregar tabla de funciones/selectores, documentación del paquete obligatorio, diagramas aplicables y declaración de compatibilidad legacy preservada.
> - Entregar comandos ejecutados, resultados de compilación/pruebas, evidencia QA de accesibilidad/responsive, E2E o justificación, riesgos y limitaciones.
> - Declarar explícitamente qué no se modificó y cómo desactivar la experiencia moderna para rollback inmediato.

## Jira Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: CONFIRMACION, ESPECIALIZADA

## Capabilities

### New Capabilities
- `confirmacion-especializada`: Capacidad derivada del ticket Jira para continuar el refinamiento funcional en OpenSpec.

### Modified Capabilities
-

## Impact

- Nueva propuesta inicial en `openspec/changes/<changeName>/proposal.md`.
- Impacto funcional pendiente de refinamiento en los siguientes artefactos OpenSpec.
