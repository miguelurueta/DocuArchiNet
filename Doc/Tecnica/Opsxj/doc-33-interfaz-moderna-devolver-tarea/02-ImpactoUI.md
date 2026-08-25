# INTERFAZ-MODERNA-DEVOLVER-TAREA

- Ticket: DOC-33
- Cambio OpenSpec: doc-33-interfaz-moderna-devolver-tarea
- Clasificacion: cross_cutting (Transversal)
## Superficies UI

- `workflow-return-activity-trigger` abre un modal aislado para **Elegir actividad anterior**. El trigger no tiene `onclick` ni fallback de postback.
- `workflow-return-activity-modern-modal` muestra el contexto resumido, búsqueda debounced, paginación por cursor, tabla de escritorio y tarjetas móviles desde el mismo resultado normalizado.
- El diálogo implementa `role="dialog"`, `aria-modal`, encabezado/descripción propios, foco inicial, trampa de foco, Escape, cancelar, cierre y fondo. Toda búsqueda invalida la selección y descarta respuestas obsoletas.
- La confirmación usa `ConfirmationDialog`; mientras ejecuta, evita repetición y cierre que pueda perder el resultado. El éxito se muestra en una región exclusiva y actualiza solo la tarea afectada mediante `WorkflowTransitionPagePresentation`.

## Validacion visual

La validación local cubre declarativamente selectores, ARIA, teclado, responsive y aislamiento. El recorrido visual autenticado autorizado completó preview no mutante, devolución UI y bloqueo mientras la respuesta permanece pendiente. Las futuras corridas requieren autorización independiente de ambiente, cuenta y tareas descartables.
