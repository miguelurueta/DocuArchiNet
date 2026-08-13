# Plan de migración controlada — Centro de trabajo Workflow

## Objetivo

Aplicar la propuesta aprobada de centro de trabajo sin bandeja: una tarea ya seleccionada presenta sus menús, acciones, documentos relacionados, visor e índice en una interfaz consistente. Se conservan IDs, eventos JavaScript, postbacks, permisos y reglas de negocio vigentes.

## Principios de seguridad

- La primera entrega es una capa CSS/JavaScript cargada al final; no se reemplaza el HTML funcional.
- Cada etapa debe poder desactivarse retirando una referencia de recurso o una clase de activación.
- No se cambian `onclick`, nombres de controles ASP.NET, `UpdatePanel`, campos ocultos ni rutas del visor.
- La selección de tarea sigue siendo el único disparador del contexto; la bandeja existente permanece disponible fuera del centro de trabajo.

## Etapa 0 — Línea base y respaldo

1. Identificar el `.aspx`/`.ascx` que emite `div_content_general_wf`, `UpdatePanel_menu_cab`, `contenido_imagen` y `contenido_indice`.
2. Registrar capturas de escritorio y móvil de los estados: sin tarea, tarea seleccionada, documento seleccionado, menú abierto y actualización parcial.
3. Inventariar los controles existentes por grupo: Opciones, Detalle, Servicios, Documentos, Notas, Autorizar, devolver, pendiente, enviar, cerrar, visor e índice.
4. Guardar una lista de IDs y eventos actuales como contrato de regresión.

**Criterio de salida:** la interfaz actual queda reproducible y sus eventos clave están documentados.

## Etapa 1 — Recursos aislados y activación opt-in

1. Crear un CSS específico, por ejemplo `Styles/workflow-centro-trabajo-moderno.css`.
2. Crear un adaptador JavaScript, por ejemplo `js/workflow/centro-trabajo-visual.js`.
3. Cargar ambos recursos después de los estilos y scripts actuales.
4. Activarlos únicamente cuando un contenedor reciba la clase `workflow-centro-trabajo-moderno`.

**Validación:** sin la clase, el Workflow debe verse y funcionar exactamente igual que hoy.

**Reversión:** retirar la clase o las dos referencias de recursos.

## Etapa 2 — Estructura visual sin mover controles

1. Aplicar el layout de centro de trabajo al contenido de tarea seleccionada: cabecera de contexto, barra de acciones, documentos, visor, índice y pie de estado.
2. Ocultar la bandeja de tareas solo dentro de este modo; no eliminar su HTML ni lógica.
3. Convertir el área de documento a tres columnas: documentos relacionados, visor e índice/metadatos.
4. Mantener versión móvil en una columna, en el orden: contexto, acciones, documentos, visor, índice.

**Validación:** abrir una tarea y comprobar visor, documentos e índice; cambiar de documento debe seguir actualizando el visor sin pérdida de contexto.

**Reversión:** desactivar la clase del modo moderno.

## Etapa 3 — Normalización de menús y botones

1. Conservar cada panel existente, pero presentarlo en grupos: `Opciones`, `Detalle`, `Servicios` y `Documentos`.
2. Mantener accesos directos para `Notas` y `Autorizar`.
3. Presentar `Devolver`, `Pendiente`, `Enviar` y `Cerrar tarea` como acciones de transición al extremo derecho.
4. Distinguir acciones de documento en la barra del visor: cargar, metadatos, versiones y más acciones.
5. Agregar `title`, `aria-label` y texto visible donde hoy exista solo un icono.

**Validación:** cada menú invoca exactamente el mismo evento que antes; no se duplican acciones ni se cambian permisos visibles.

**Reversión:** retirar solo las reglas de agrupación del adaptador, manteniendo el layout de la etapa 2 si se desea.

## Etapa 4 — Sincronización con UpdatePanel

1. Ejecutar el adaptador al cargar la página y después de `endRequest` de `Sys.WebForms.PageRequestManager`.
2. Recalcular la disposición al abrir/cerrar menús y al cambiar documento o tarea.
3. Nunca clonar controles: se reubican visualmente o se les añaden clases, preservando su identidad DOM y eventos.
4. Verificar que un postback parcial no deje dos estados seleccionados ni menús abiertos desfasados.

**Validación:** repetir selección de tareas/documentos y cada acción de menú tras al menos tres actualizaciones parciales consecutivas.

## Etapa 5 — Estados, riesgo y accesibilidad

1. Aplicar colores de estado solo como complemento del texto: pendiente, en proceso, cerrado y seleccionado.
2. Reservar el estilo de peligro para cerrar/eliminar y solicitar confirmación conforme a la lógica existente.
3. Garantizar foco visible, navegación por teclado, objetivos táctiles mínimos de 34 px y etiquetas para iconos.
4. Probar 1366 px, 1024 px, 768 px y 375 px; verificar truncamiento, menú emergente y lector de contexto.

**Criterio de salida:** no hay controles sin nombre accesible, solapamientos ni acciones críticas ambiguas.

## Etapa 6 — Piloto funcional y despliegue

1. Habilitar el modo moderno para un entorno local o un perfil de prueba mediante una bandera de configuración o clase de contenedor.
2. Ejecutar una matriz de regresión: abrir tarea, abrir documento, cambiar documento, editar índice, guardar, adjuntar, firmar, devolver, poner pendiente, enviar y cerrar.
3. Recoger validación de usuarios de operación sobre rapidez de localización de acciones y lectura del contexto.
4. Promover a producción únicamente al aprobar la matriz y las capturas de los estados principales.

## Etapa 7 — Estabilización y limpieza

1. Mantener la bandera de reversión durante una versión posterior al despliegue.
2. Corregir únicamente defectos visuales o de sincronización detectados.
3. Cuando el piloto esté estable, consolidar reglas duplicadas y documentar el contrato de componentes reutilizables: menú de tarea, barra de documento, panel de documentos, visor, índice y pie de contexto.

## Matriz mínima de regresión

| Flujo | Resultado esperado |
|---|---|
| Seleccionar tarea | Se actualizan contexto, acciones y documentos asociados. |
| Abrir documento | El visor cambia y el documento queda destacado. |
| Abrir índice/metadatos | La acción afecta el documento activo. |
| Actualización parcial | Layout y eventos se restablecen sin controles duplicados. |
| Acción de tarea | Enviar, devolver, pendiente y cerrar conservan su comportamiento actual. |
| Móvil | El contenido se apila sin ocultar acciones esenciales. |

