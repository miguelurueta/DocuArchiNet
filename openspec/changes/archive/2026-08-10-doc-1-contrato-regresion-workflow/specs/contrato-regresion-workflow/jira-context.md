# Jira Context - DOC-1

## Summary

CONTRATO-REGRESION-WORKFLOW

## Description

> # JIRA-01 — Línea base y contrato de regresión Workflow
> 
> ## Prompt para Jira
> 
> **Rol:** Actúa como arquitecto de software senior especializado en ASP.NET WebForms, `UpdatePanel`, regresión funcional y documentación de sistemas legacy.
> 
> Analiza y documenta la interfaz WebForms que implementa el centro de trabajo Workflow antes de cualquier cambio visual. El resultado debe permitir demostrar que la modernización no altera negocio, permisos, postbacks ni selección documental.
> 
> ### Alcance
> 
> - Consumir el inventario y la decisión de corte aprobados en JIRA-00; fijar expresamente qué recursos y versiones componen la línea base funcional.
> - Identificar los `.aspx`, `.ascx`, JavaScript y CSS que emiten o controlan `div_content_general_wf`, `UpdatePanel_menu_cab`, `contenido_imagen`, `contenido_indice`, la lista de tareas y documentos relacionados.
> - Inventariar ID, selector, evento, fuente de permisos, tipo de postback y dependencia de cada acción: Opciones, Detalle, Servicios, Documentos, Notas, Autorizar, Devolver, Pendiente, Enviar, Cerrar, adjuntar, firmar, visor e índice.
> - Identificar los campos ocultos y valores de DOM que participan en tarea/documento activo. Para cada flujo, distinguir campo canónico, campo derivado, formato del valor, valor vacío y mecanismo que lo actualiza; no asumir que un único hidden input representa todos los contextos.
> - Mapear los `UpdatePanel` que pueden reemplazar menú, selección, documentos, visor e índice y los contenedores existentes que JIRA-04 puede usar como zonas Grid sin reubicar nodos.
> - Registrar capturas en 1366, 1024, 768 y 375 px para: sin tarea, tarea seleccionada, documento seleccionado, menú abierto y postback parcial.
> 
> ### Restricciones no negociables
> 
> - No modificar HTML, JavaScript ni CSS funcionales.
> - No cambiar `onclick`, nombres ASP.NET, IDs, `UpdatePanel`, hidden inputs, permisos, rutas del visor ni servicios.
> 
> ### Entregables técnicos
> 
> 1. `01-ContratoControles.md`: tabla de controles, IDs, eventos y riesgos.
> 2. `02-ContratoEstadoSeleccion.md`: contrato por flujo de tarea y documento, con fuente canónica, campos derivados, limpieza e invariantes visuales.
> 3. `03-FlujoSeleccionWebForms.md`: secuencia tarea → documento → visor/índice → postback, incluidos los paneles que se vuelven a renderizar.
> 4. `04-MapaContenedoresLayout.md`: selectores permitidos, jerarquía y restricciones para Grid/Flex.
> 5. `05-MatrizRegresionBase.md`: casos, datos necesarios, navegador, cuenta de prueba y resultado esperado.
> 6. Carpeta de evidencias visuales con nomenclatura estable, fecha, versión de recursos y entorno.
> 
> ### Criterios de aceptación
> 
> - Todos los controles críticos tienen propietario técnico y evento identificado.
> - Cada flujo identifica una fuente canónica de tarea/documento y sus campos derivados, sin crear una nueva fuente de verdad en el adaptador visual.
> - La matriz separa selección activa de documento y selección masiva por checkbox.
> - El mapa de layout identifica los contenedores que pueden participar en Grid/Flex y los `UpdatePanel` que no pueden moverse ni colapsarse.
> - La matriz permite repetir los flujos críticos sin inferencias manuales.
> 
> ### Pruebas requeridas
> 
> - Repetir tres actualizaciones parciales consecutivas tras elegir tarea/documento.
> - Verificar con un usuario sin permiso y uno con permiso de transición.
> - Ejecutar los casos con: sin tarea, tarea sin documento, documento de título largo, lista extensa, selección masiva y permisos restringidos.
> 
> ### Reversión
> 
> No aplica: ticket exclusivamente documental.

## Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: CONTRATO, REGRESION, WORKFLOW
