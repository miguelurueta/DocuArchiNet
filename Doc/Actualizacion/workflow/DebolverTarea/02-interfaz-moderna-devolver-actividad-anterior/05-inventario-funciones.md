# Inventario de funciones implementadas y reutilizadas

| Componente | Tipo | Uso en DOC-33 | Aislamiento |
| --- | --- | --- | --- |
| `workflow-return-activity-trigger` | Trigger | Abre la elección de actividad anterior. | No tiene `onclick` ni postback legacy. |
| `WorkflowReturnActivityUi` | Módulo UI | Preview, búsqueda, páginas, foco, cancelación y evento de selección. | Estado y eventos con prefijo exclusivo. |
| `WorkflowReturnActivityConfirmation` | Módulo UI | Ejecuta la terna seleccionada y muestra resultado. | No invoca módulos de usuario, grupo o transición. |
| `ConfirmationDialog` | Reutilizado | Bloquea doble envío y cierre durante operación pendiente. | Contrato compartido sin ampliar payload. |
| `WorkflowTransitionPagePresentation` | Reutilizado | Refresca solo la representación de la tarea exitosa. | Identidad correlacionada por `IdTarea`. |
| `workflow-transition-modern.css` | Reutilizado/extensión | Diálogo, estados y cuerpo responsive. | Selectores `workflow-return-activity-*` exclusivos. |
| `PreviewDevolverActividad` | DOC-32 reutilizado | Lista opciones autorizadas. | Solo lectura y contexto reconstruido. |
| `EjecutarDevolverActividad` | DOC-32 reutilizado | Ejecuta destino elegido bajo lock. | Revalidación servidor y respuesta saneada. |

Fuera del inventario quedan `D-TASK-ANT`, `Button_tool_devolver_a_actividades_anterior`, su handler Web Forms y su declaración de diseñador: no deben volver a ser alcanzables desde la acción de actividad anterior.
