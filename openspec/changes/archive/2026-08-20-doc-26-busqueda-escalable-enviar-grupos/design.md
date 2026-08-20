<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09 -->
## Context

DOC-26 amplía únicamente la presentación moderna de Enviar a grupo cuando una ruta tiene muchos destinos o grupos relacionados. La implementación actual obtiene todos los destinos con PreviewEnviarGrupo y MySqlEnvioGrupoRepository; la consulta une grupos_workflow sin agrupar, por lo que su cardinalidad no escala y puede duplicar una actividad.

La solución mantiene dos límites separados:

1. PreviewEnviarGrupo continúa abriendo el modal y entregando el contexto con una primera página máxima de 25 destinos.
2. BuscarDestinosEnvioGrupo obtiene una página limitada para el filtro remoto.
3. EjecutarEnvioGrupo conserva el contrato mutante existente y vuelve a autorizar el destino dentro del lock.

### Objetivos

- Encontrar una actividad por su nombre o por el nombre de un grupo asociado.
- Mostrar a lo sumo una fila o tarjeta por IdActividadDestino.
- Limitar la transferencia, permitir navegación y no calcular COUNT en cada búsqueda.
- Mantener la autorización del servidor, el gate fail-closed, el fallback Web Forms y la experiencia accesible actual.

### Fuera de alcance

- Cambiar PreviewEnviarTarea, EjecutarEnvioTarea, IdConector, ServicioTransicionTarea o Continuar flujo.
- Crear gates, modificar su configuración, añadir índices o migrar la base de datos.
- Ejecutar E2E autenticado, pruebas de carga o activar el gate.

## Decisiones

### D-01 — Método de búsqueda aislado

WebServiceWorkflowModern.asmx.vb incorporará BuscarDestinosEnvioGrupo. Construirá el mismo contexto autenticado y ServicioEnvioGrupoTarea que usa PreviewEnviarGrupo, pero invocará una operación de lectura dedicada. PreviewEnviarGrupo conserva su firma y su contexto, pero obtiene la primera página limitada y publica los metadatos de página; así nunca descarga una lista ilimitada al abrir el modal.

### D-02 — Contrato paginado mínimo

El método recibe los argumentos ASMX idTarea, termino, pagina y tamanoPagina. Su respuesta BusquedaDestinosEnvioGrupoDto contiene IdTarea, TokenVersion, Pagina, TamanoPagina, TieneMas, Destinos y Error. PrevisualizacionEnvioGrupoDto publica los mismos metadatos de primera página sin cambiar sus argumentos. Cada destino expone IdActividadDestino, NombreActividad y GrupoDestino resumido. No expone grupos completos, conteos globales, ruta, permisos, Session, SQL ni IdConector.

### D-03 — Normalización antes de Infrastructure

Application normaliza Trim(termino), página uno-basada y tamaño. El término vacío solicita la primera página sin filtro; un término no vacío debe medir entre 2 y 80 caracteres. La página se ajusta al mínimo 1 y el tamaño al intervalo 1..50, con 25 como valor por defecto. Se devuelven Pagina y TamanoPagina aplicados. Los términos de longitud inválida devuelven un error público específico sin tocar Infrastructure.

### D-04 — Autorización y lectura

BuscarDestinosEnvioGrupo llamará el control de sesión y gate existente y ServicioEnvioGrupoTarea reutilizará la verificación de Cambio_Ruta, tarea activa, ruta y flujo antes de consultar. El repositorio solo ejecutará SELECT con parámetros para ruta, término, límite y desplazamiento. Un filtro remoto no autoriza EjecutarEnvioGrupo; esa operación mantiene su relectura y validación dentro del lock.

### D-05 — Unicidad de actividad y resumen de grupos

La consulta parte de LISTADO_ACTIVIDADES_WORKFLOW restringida por RUTAS_WORKFLOW_ID_RUTA. El filtro de grupo usa EXISTS correlacionado con grupos_workflow, y la proyección agrupa por actividad. Para una asociación se muestra el nombre del grupo; para varias se muestra una etiqueta de cantidad calculada en la misma fila. El valor seleccionable siempre es IdActividadDestino.

### D-06 — Paginación sin conteo

El repositorio pide TamanoPagina más uno mediante parámetros y conserva solo TamanoPagina en la respuesta. La fila adicional establece TieneMas. La ordenación estable es NombreActividad e IdActividadDestino. No se agregan índices ni cambios de esquema: antes de proponerlos se registrará una consulta representativa y su plan de ejecución en evidencia técnica aprobada.

### D-07 — Estado asíncrono y accesible

workflow-group-send-ui.js añadirá campo visible Buscar actividad o grupo, instrucción de mínimo dos caracteres, controles anterior y siguiente y estado aria-live. Tras 300 ms sin pulsaciones se consulta la primera página; limpiar el campo restaura página uno. AbortController cuando exista y una secuencia monotónica descartan respuestas obsoletas. Todo filtro, página, reintento o nuevo preview invalida la selección visual y la posibilidad de confirmar una selección de generación anterior. Tabla y tarjetas se derivan del mismo modelo.

### D-08 — Invariantes de ejecución y regresión

El diálogo de confirmación sigue recibiendo solo idTarea, idActividadDestino y tokenVersion. No reutiliza IdConector ni cambia WorkflowLegacyEnvioGrupoExecutorAdapter. Escape, trampa de foco, foco de retorno, teclado, doble clic y fallback conservan el comportamiento actual. Con gate inactivo el enlace legacy no se intercepta.

### D-09 — Retorno estable a la bandeja después del éxito

Tras una ejecución moderna confirmada, WorkflowTransitionPagePresentation conserva la retirada local de la fila y el cierre del contexto de tarea. Después restablece a cero el desplazamiento horizontal del contenedor marcado de la bandeja y llama a auto_zise_popup_workflow existente cuando está disponible, una vez que el listado ya es visible. No se agrega una petición, un postback, un cambio de payload ni una regla de gate; el atributo de datos identifica el contenedor visual sin acoplar el presentador a controles Web Forms.

## Arquitectura y flujo

    Trigger moderno
      -> PreviewEnviarGrupo(idTarea) para abrir contexto
      -> BuscarDestinosEnvioGrupo(idTarea, termino, pagina, tamanoPagina)
         -> sesión y gate existente
         -> Cambio_Ruta, tarea, ruta y flujo
         -> SELECT parametrizado, agrupado y limitado
      -> selección IdActividadDestino
      -> EjecutarEnvioGrupo(idTarea, idActividadDestino, tokenVersion)
         -> relectura, GET_LOCK y revalidación existentes
         -> éxito confirmado: retirar fila, cerrar contexto y restaurar bandeja

La búsqueda usa una interfaz de lectura dedicada para que MySqlEnvioGrupoRepository conserve separados ObtenerDestinos y ResolverDestino de la nueva consulta paginada. Los DTOs y el servicio modelan explícitamente la página para que la UI no sintetice autorización ni límites.

## Riesgos y mitigaciones

- Un filtro con coincidencia inicial amplia puede requerir revisión de rendimiento; el límite, la fila adicional y el plan de consulta reducen el impacto sin alterar esquema.
- Una respuesta tardía puede representar otra búsqueda; la secuencia y la invalidación de selección impiden usarla.
- La agregación de grupos puede revelar más de lo necesario; se limita al nombre único o a la cantidad del destino ya autorizado en la ruta.
- El preview antiguo puede seguir devolviendo listas grandes durante la transición; la UI debe preferir la búsqueda paginada para rutas potencialmente extensas sin cambiar los endpoints actuales.

## Plan de pruebas

Las pruebas CJS y VB focales cubren límites, filtros, agrupación, SELECT, sesión y gate denegados, resultados obsoletos, accesibilidad, fallback, ejecución con token o destino inválido y la restauración de la bandeja tras éxito. Se ejecutan la suite afectada y la compilación disponible; no se sustituyen por E2E autenticado ni carga.
