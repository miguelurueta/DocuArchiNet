<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07 -->
# Diseño - DOC-13: confirmación especializada

## Contexto

DOC-12 termina al seleccionar un destino desde una lista de solo lectura. La selección contiene la tarea, conector, versión y resumen normalizado. El endpoint paralelo `EjecutarEnvioTarea` ya preserva la autoridad de Application: obtiene sesión en servidor, verifica el feature gate, protege concurrencia, vuelve a resolver destino y requisitos, y usa los adaptadores legacy autorizados. DOC-13 agrega la confirmación de Presentation entre esa selección y el endpoint; no reemplaza el flujo Web Forms preexistente.

## Arquitectura propuesta

```text
Lista moderna (preview validado)
          |
          +-- workflow:destination-selected (detalle normalizado)
                         |
                         v
        Adaptador Workflow -------------------> ConfirmationDialog.open(config)
                         |                                  |
                         |                                  +-- render, foco, Escape,
                         |                                      trap, estado y doble envío
                         v
 WebServiceWorkflowModern.EjecutarEnvioTarea(idTarea, idConector, tokenVersion)
                         |
                         v
 ServicioTransicionTarea (gate, concurrencia, requisitos, autorización y legado)
                         |
                         v
 ResultadoTransicionDto --> adaptador --> callback visual de página
```

## Decisiones

### D-01 — Contexto limitado al contrato disponible

La lista moderna ampliará su evento de selección solo con el resumen normalizado del preview actual. El adaptador mostrará radicado, tipo de decisión, grupo actual, destino, destinatario o grupo y mecanismo. `PrevisualizacionTransicionDto` no tiene trámite; `ActividadOrigen` contiene un identificador y el preview no obtiene requisitos ni advertencias. Esos valores se omiten de la confirmación en lugar de inferirlos desde controles ocultos, HTML, Session o reglas en JavaScript. No se modifica el DTO, Application ni el endpoint para agregarlos.

### D-02 — Componente genérico y aislado

`js/java_general/ConfirmationDialog.js` publica solamente `ConfirmationDialog.open(config)` y `ConfirmationDialog.close()`. Recibe datos, etiquetas, `summaryFields`, requisitos y advertencias opcionales, `confirmationNotice`, `executionContext`, `execute`, `normalizeResult` y callbacks. El contexto es opaco: el componente no cambia ni interpreta tarea, grupo, permisos o destino. Crea nodos con API DOM, no inserta HTML no confiable y no contiene texto fijo del workflow. `Styles/confirmation-dialog.css` se limita a sus clases propias y a estados declarados.

### D-03 — Adaptador de Workflow como frontera

`js/workflow/workflow-transition-confirmation-integration.js` escucha la selección de DOC-12 y convierte el detalle publicado por `WorkflowTransitionUi` al `config` genérico. Proporciona “Enviar tarea”, “Cancelar”, “La tarea actual quedará finalizada” y la etiqueta primaria con la actividad destino. También serializa únicamente la terna existente `{ idTarea, idConector, tokenVersion }` al ASMX y normaliza `ResultadoTransicionDto` al contrato visual. La conversión de DTOs, textos y callbacks de refresco pertenece al adaptador, no a `ConfirmationDialog`.

### D-04 — Ejecución única y correlacionada

El componente bloquea el primario al iniciar `execute`, muestra `Enviando tarea…` y conserva una identidad local de la apertura. Mientras esa operación está en curso, deshabilita `X` y Cancelar, bloquea fondo, Escape, `close()` y una nueva apertura, y publica un mensaje de espera; también activa la advertencia nativa ante `beforeunload`. Al recibir éxito, bloqueo o error técnico recupera las acciones correspondientes. Solo procesa una respuesta cuyo token y contexto coincidan con la apertura vigente. Si la persona cierra la pestaña pese al aviso, el servidor sigue siendo la fuente de verdad y la lista debe consultarse de nuevo. Estas defensas visuales complementan, pero no sustituyen, el `MySqlTransicionConcurrencyGuard`, la revalidación de token, destino y requisitos que realiza `ServicioTransicionTarea`.

### D-05 — Estados y actualización después del éxito

El componente modela `confirmando`, `enviando`, `exito`, `bloqueo-funcional` y `error-tecnico-controlado`. En éxito no realiza por sí mismo operaciones de página: entrega el resultado normalizado a `onSuccess`. El adaptador invoca `WorkflowTransitionPagePresentation.applySuccess`, que trabaja exclusivamente con atributos `data-workflow-*` emitidos por la página para retirar la fila exacta si aún está representada, restablecer la lista, limpiar contexto y visor, ocultar acciones de la tarea, ajustar el contador visible y comunicar una confirmación no intrusiva que se oculta a los seis segundos. No consulta controles ocultos ni funciones legacy. En bloqueo mantiene el diálogo con causa legible y acciones restauradas; en fallo técnico conserva contexto, no retira tarea y muestra el `technicalError` configurado, nunca `Error.message` ni detalles de transporte del navegador. Los callbacks de autorización conservan el flujo vigente y no implementan la regla en cliente.

### D-06 — Activación, coexistencia y rollback

Los nuevos recursos se registran mediante el bootstrap existente, que usa el mismo `IWorkflowModernFeatureGate` que el ASMX. La página no registra listener, diálogo ni assets de confirmación si el bootstrap no está activo. El ASMX permanece como segunda barrera y devuelve bloqueo funcional si el piloto se desactiva entre preview y envío. La UI legacy, sus modales, postbacks y controles no se modifican; el rollback es desactivar la bandera de piloto y cargar nuevamente la página.

### D-07 — Verificación y documentación

Las pruebas JavaScript aíslan `ConfirmationDialog` con un config ajeno a Workflow y el adaptador con respuestas ASMX simuladas. La verificación final incluye build de la solución, pruebas focales, QA manual de escritorio y móvil y, solo con autorización, E2E autenticada. La documentación se crea exclusivamente bajo `Doc/Actualizacion/workflow/Terminar/05-confirmacion-especializada/`, con los diagramas Mermaid requeridos y evidencia sin secretos ni datos personales.

## Riesgos y compatibilidad

- El modal actual de DOC-12 debe conservar su foco y selección hasta que el adaptador abra la confirmación; el cambio no puede reintroducir postback durante la experiencia moderna.
- El preview actual no contiene todos los campos solicitados originalmente. El diseño los omite explícitamente y no los rellena desde fuentes legacy.
- Un resultado técnico o tardío no puede limpiar el contexto visual. La correlación de cliente se combina con el guard y la versión protegidos por servidor.
- La autorización complementaria se delega al mecanismo existente; DOC-13 no crea una segunda implementación ni altera su decisión.
