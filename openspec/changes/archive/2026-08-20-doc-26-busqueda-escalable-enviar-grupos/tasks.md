<!-- opsxj:refinement-traceability version=1 artifact=tasks decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08 -->
## 1. Contrato y dominio

- [x] 1.1 [S] Añadir DTOs de request y respuesta para BuscarDestinosEnvioGrupo, metadatos de primera página en PreviewEnviarGrupo y una interfaz de lectura paginada, sin modificar las firmas de PreviewEnviarGrupo ni EjecutarEnvioGrupo. Área/archivos: modelos, interfaces y ServicioEnvioGrupoTarea. Origen: D-01, RQ-01.
- [x] 1.2 [S] Normalizar término, página y tamaño en Application antes de Infrastructure; devolver valores aplicados y errores públicos seguros. Área/archivos: ServicioEnvioGrupoTarea y validador de grupo. Origen: D-03, RQ-03.
- [x] 1.3 [S] Añadir BuscarDestinosEnvioGrupo al ASMX existente con el mismo contexto autenticado, gate y respuestas seguras del módulo de grupo. Área/archivos: webservice/WebServiceWorkflowModern.asmx.vb. Origen: D-02, RQ-02.

## 2. Consulta segura y escalable

- [x] 2.1 [S] Implementar en MySqlEnvioGrupoRepository la consulta SELECT parametrizada por ruta, término, límite y desplazamiento, después de validar sesión efectiva, Cambio_Ruta, tarea, ruta y flujo. Área/archivos: Infrastructure/Repositories/Workflow/MySqlEnvioGrupoRepository.vb. Origen: D-04, RQ-04.
- [x] 2.2 [S] Filtrar por actividad o grupo y agrupar por IdActividadDestino para devolver una sola fila con resumen de grupo acotado. Área/archivos: Infrastructure/Repositories/Workflow/MySqlEnvioGrupoRepository.vb. Origen: D-05, RQ-05.
- [x] 2.3 [S] Solicitar una fila adicional, calcular TieneMas y ordenar de manera estable sin COUNT ni cambios de esquema. Registrar la consulta representativa y su plan antes de proponer índices. Área/archivos: repositorio y documentación técnica del paquete único. Origen: D-06, RQ-06.

## 3. Presentación accesible

- [x] 3.1 Añadir el campo Buscar actividad o grupo, instrucción visible, controles de página y estados aria-live al modal de grupo. Área/archivos: workflow/Webworkflow.aspx y js/workflow/workflow-group-send-ui.js. Origen: D-07, RQ-07.
- [x] 3.2 Implementar demora de 300 ms, mínimo de caracteres, limpieza a página uno, cancelación o descarte obsoleto y sincronía tabla/tarjetas. Área/archivos: js/workflow/workflow-group-send-ui.js. Origen: D-07, RQ-07.
- [x] 3.3 Invalidar la selección y la confirmación de generación anterior al cambiar filtro, página, reintento o preview; conservar Escape, foco, teclado y doble clic. Área/archivos: workflow-group-send-ui.js y workflow-group-send-confirmation.js. Origen: D-08, RQ-08.

## 4. Pruebas y documentación

- [x] 4.1 Añadir pruebas de contrato ASMX, filtros, normalización, límites, página, TieneMas, SELECT y denegaciones de autorización. Área/archivos: pruebas VB o estáticas focales y tests/workflow-group-send.test.cjs. Origen: D-04, RQ-04.
- [x] 4.2 Añadir pruebas de actividad con varios grupos, respuesta obsoleta, reintento, móvil/escritorio, teclado, Escape y selección invalidada. Área/archivos: tests/workflow-group-send.test.cjs. Origen: D-05, RQ-05.
- [x] 4.3 Confirmar con pruebas que el payload de ejecución, token, destino retirado, concurrencia, fallback legacy y Continuar flujo no cambian. Área/archivos: suites de grupo y transición existentes. Origen: D-08, RQ-08.
- [x] 4.4 Actualizar exclusivamente Doc/Actualizacion/workflow/TerminarGrupo/01-implementacion-envio-grupo/ con contrato, seguridad, secuencia de búsqueda y evidencia de pruebas. Área/archivos: 02-contrato.md, 03-flujo-y-seguridad.md, 04-pruebas-y-evidencia.md y Diagramas/. Origen: D-02, RQ-02.
- [x] 4.5 Ejecutar las pruebas focales y la compilación disponible, registrar código de salida y limitaciones reproducibles; no ejecutar E2E autenticado, carga ni activar el gate. Área/archivos: evidencia técnica del cambio. Origen: D-06, RQ-06.
