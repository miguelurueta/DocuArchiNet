# JIRA-03 — Adaptador UpdatePanel y selección única

## Prompt para Jira

**Rol:** Actúa como ingeniero senior de JavaScript legacy y ASP.NET AJAX, experto en `Sys.WebForms.PageRequestManager`, `UpdatePanel` y consistencia de estado en DOM.

Implementa el adaptador visual resiliente al ciclo de vida de WebForms. Debe sincronizar el layout moderno después de cada actualización parcial sin duplicar selección, menús, eventos o controles.

### Alcance

- Ejecutar inicialización en carga inicial y en `Sys.WebForms.PageRequestManager.endRequest`.
- Leer la tarea/documento activo desde los campos ocultos definidos en JIRA-01.
- Eliminar estados visuales anteriores antes de aplicar `gridview-documento-seleccionado` al documento activo.
- Cerrar dropdowns obsoletos, limpiar atributos ARIA desfasados y restaurar foco cuando el panel actualizado lo requiera.
- Recalcular layout en cambio de tarea, documento y menú.

### Restricciones no negociables

- No clonar controles.
- No mover nodos que contengan `UpdatePanel`, hidden inputs, validadores o eventos inline.
- Usar clases y CSS; solo permitir `order`/Grid/Flex para composición.
- No crear una segunda fuente de selección en JavaScript.

### Entregables técnicos

1. `01-CicloVidaUpdatePanel.md` con secuencia de inicialización y limpieza.
2. `02-ContratoSeleccion.md` con fuente de verdad y estados permitidos.
3. Pruebas unitarias o de navegador del adaptador para estados consecutivos.

### Criterios de aceptación

- Tras tres postbacks parciales consecutivos existe como máximo una tarea y un documento visualmente seleccionados.
- El menú abierto no sobrevive si su control fue reemplazado por UpdatePanel.
- La selección visual coincide con el valor de los hidden inputs.

### Pruebas requeridas

- Selección rápida de dos tareas y tres documentos.
- Actualización parcial con menú abierto.
- Sesión vencida/error de respuesta parcial sin excepción no controlada.

### Reversión

Desactivar el inicializador visual; no modificar la lógica original de selección.
