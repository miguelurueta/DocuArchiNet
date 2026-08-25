# INTERFAZ-MODERNA-DEVOLVER-TAREA

- Ticket: DOC-33
- Cambio OpenSpec: doc-33-interfaz-moderna-devolver-tarea
- Clasificacion: cross_cutting (Transversal)
## Objetivo

DOC-33 reemplaza el acceso Web Forms legado para devolver una tarea a una actividad anterior por una interfaz moderna oficial. Consume exclusivamente `PreviewDevolverActividad` y `EjecutarDevolverActividad`, ambos entregados por DOC-32, y no duplica permisos, consultas de Ruta/Flujo, cursor, concurrencia ni la llamada al motor legacy.

## Alcance y compatibilidad

- Afecta `workflow/Webworkflow.aspx`, su code-behind/diseñador, `Webworkflow.js`, los assets exclusivos `workflow-return-activity-ui.js` y `workflow-return-activity-confirmation.js`, y los estilos de transición modernos.
- Retira únicamente `D-TASK-ANT`, `Button_tool_devolver_a_actividades_anterior`, su handler y callback de postback. `Usuario anterior`, Enviar a usuario, Enviar a grupo y Continuar flujo conservan sus contratos y módulos propios.
- No consulta ni cambia `WorkflowCentroTrabajoModernActive`; no introduce flags, configuración, migraciones ni cambios de esquema.
- La reversa es por código fuente. No requiere datos compensatorios ni revierte una devolución ya confirmada.
