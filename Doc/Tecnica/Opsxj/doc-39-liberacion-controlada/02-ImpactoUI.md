# Impacto UI de la liberación controlada

- Ticket: DOC-39
- Cambio OpenSpec: doc-39-liberacion-controlada
- Clasificacion: cross_cutting

## Superficies UI

La superficie funcional cubierta por la liberación es el Centro de trabajo en `workflow/Webworkflow.aspx`. Devolver → Usuario anterior conserva su disparador, preview y modal propios; el navegador solicita el preview y solo puede enviar `idTarea` y `tokenVersion` a la ejecución vigente. La autorización, el usuario histórico, la actividad y el historial se reconstruyen en servidor.

El cambio de tarea debe volver a enlazar las acciones modernas y asociar el preview con la tarea recién seleccionada. Cancelar o pulsar `Escape` cierra el modal sin transición. Cuando el antecedente no identifica a un usuario individual elegible, se muestra un bloqueo funcional y no se inicia el flujo de Actividad anterior.

DOC-39 no altera HTML, JavaScript, estilos, foco, navegación ni tamaños de la bandeja. Tampoco cambia `general_code_java.js`; la evidencia DOC-38 registró compatibilidad con el recurso `compatible-events5` y ausencia del error manual observado previamente.

## Validacion visual

La QA manual saneada de DOC-38, `QA-MANUAL-DOC38-20260828`, aprobó preview, cancelación, `Escape`, cambio de tarea, bloqueos por historial, exclusividad frente a Actividad anterior, responsive, foco y compatibilidad JavaScript. La observación de doble clic durante ejecución no se realizó porque exige una transición real.

Antes de una futura ventana autorizada, el responsable de QA debe comprobar que el artefacto aprobado conserva esos comportamientos en el ambiente correspondiente. Esa comprobación no se inicia con este documento: requiere autorización explícita de ambiente y una cuenta de prueba autorizada. No se permiten rutas visuales alternativas, postback ni reutilización del modal o destino de Actividad anterior.
