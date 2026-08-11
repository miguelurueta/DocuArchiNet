# JIRA-01 — Línea base y contrato de regresión Workflow

## Prompt para Jira

**Rol:** Actúa como arquitecto de software senior especializado en ASP.NET WebForms, `UpdatePanel`, regresión funcional y documentación de sistemas legacy.

Analiza y documenta la interfaz WebForms que implementa el centro de trabajo Workflow antes de cualquier cambio visual. El resultado debe permitir demostrar que la modernización no altera negocio, permisos, postbacks ni selección documental.

### Alcance

- Identificar los `.aspx`, `.ascx`, JavaScript y CSS que emiten o controlan `div_content_general_wf`, `UpdatePanel_menu_cab`, `contenido_imagen`, `contenido_indice`, la lista de tareas y documentos relacionados.
- Inventariar ID, selector, evento, fuente de permisos, tipo de postback y dependencia de cada acción: Opciones, Detalle, Servicios, Documentos, Notas, Autorizar, Devolver, Pendiente, Enviar, Cerrar, adjuntar, firmar, visor e índice.
- Identificar los campos ocultos que representan tarea/documento activo y el mecanismo que los actualiza.
- Registrar capturas en 1366, 1024, 768 y 375 px para: sin tarea, tarea seleccionada, documento seleccionado, menú abierto y postback parcial.

### Restricciones no negociables

- No modificar HTML, JavaScript ni CSS funcionales.
- No cambiar `onclick`, nombres ASP.NET, IDs, `UpdatePanel`, hidden inputs, permisos, rutas del visor ni servicios.

### Entregables técnicos

1. `01-ContratoControles.md`: tabla de controles, IDs, eventos y riesgos.
2. `02-FlujoSeleccionWebForms.md`: secuencia tarea → documento → visor/índice → postback.
3. `03-MatrizRegresionBase.md`: casos, datos necesarios y resultado esperado.
4. Carpeta de evidencias visuales con nomenclatura estable.

### Criterios de aceptación

- Todos los controles críticos tienen propietario técnico y evento identificado.
- Se identifica una única fuente de verdad para tarea y documento seleccionados.
- La matriz permite repetir los flujos críticos sin inferencias manuales.

### Pruebas requeridas

- Repetir tres actualizaciones parciales consecutivas tras elegir tarea/documento.
- Verificar con un usuario sin permiso y uno con permiso de transición.

### Reversión

No aplica: ticket exclusivamente documental.
