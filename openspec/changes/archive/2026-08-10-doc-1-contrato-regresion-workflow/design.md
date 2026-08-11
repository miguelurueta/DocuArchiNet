## Contexto

DOC-1 establece la línea base técnica del centro de trabajo Workflow antes de la modernización visual. El módulo es ASP.NET WebForms y combina estado de servidor, campos ocultos, JavaScript y actualizaciones parciales `UpdatePanel`.

## Objetivos y exclusiones

**Objetivos**

- Documentar el contrato de controles, selección, postbacks y layout existente.
- Definir una matriz repetible de regresión y un formato de evidencia visual.
- Dejar un límite seguro para los cambios posteriores de JIRA-04 y tickets dependientes.

**Fuera de alcance**

- Cambiar `.aspx`, code-behind, JavaScript, CSS, permisos, rutas del visor o servicios.
- Crear fuentes de verdad nuevas para tarea o documento seleccionado.
- Aplicar la política `AppResponses<T>`: DOC-1 es exclusivamente documental y el módulo analizado es WebForms, sin consumidores de esa API modificados por este cambio.

## Decisiones arquitectónicas

1. La tarea canónica después de un postback es `Session("ID_TAREA_SELECCIONDA")`; `Hidden_id_tarea_selecionada` es su espejo de cliente. `Hidden_id_tarea_sel` es el candidato que JavaScript envía al servidor y no sustituye a la sesión.
2. La selección activa de documento usa el descriptor `hiden_seleccion_documento_wf` y el identificador de fila `hiden_seleccion_documento_id_wf`. La selección masiva por checkbox es un contexto distinto.
3. Los `UpdatePanel` existentes son límites de renderizado. JIRA-04 podrá aplicar estilos Grid/Flex a contenedores, pero no mover, colapsar ni sustituir esos límites ni sus controles con ID generado por WebForms.
4. La evidencia de ejecución solo se considerará válida si referencia la decisión de corte de JIRA-00, versión/commit, entorno, cuenta y datos de prueba. Este repositorio permite la línea base estática; la captura final requiere un ambiente ejecutable autorizado.

## Riesgos y mitigaciones

| Riesgo | Mitigación contractual |
| --- | --- |
| El re-render parcial elimina listeners o estilos dinámicos | Probar tres postbacks consecutivos y rehidratar únicamente en el ciclo ASP.NET existente. |
| Se confunde tarea candidata con tarea consolidada | Mantener separados `Hidden_id_tarea_sel`, sesión y espejo seleccionado. |
| Se confunde documento activo con selección masiva | Casos y campos separados en la matriz de regresión. |
| Un cambio de layout mueve un `UpdatePanel` | El mapa identifica los nodos que solo admiten estilos, no reubicación. |
| Evidencia sin línea base aprobada | Bloquear el cierre de validación hasta disponer de JIRA-00 y un ambiente de prueba. |

## Entregables

Los contratos se publican en `Doc/Tecnica/Opsxj/doc-1-contrato-regresion-workflow/`: controles, estado de selección, secuencia WebForms, mapa de contenedores, matriz de regresión y convención de evidencias.

## Plan de validación

1. Revisar los IDs, eventos y paneles contra `workflow/Webworkflow.aspx`, `.vb` y `js/workflow/Webworkflow.js`.
2. Validar la estructura OpenSpec y el whitespace del cambio.
3. Ejecutar la matriz cuando estén disponibles el inventario/corte JIRA-00, URL, cuentas con y sin permiso y datos controlados.

## Preguntas abiertas

- ¿Cuál es el artefacto aprobado de JIRA-00 que fija recursos y versión de corte?
- ¿Cuál es el ambiente autorizado, URL, navegador objetivo y cuentas de prueba para las capturas?
