# BUSQUEDA-ESCALABLE-ENVIAR-GRUPOS

- Ticket: DOC-26
- Cambio OpenSpec: doc-26-busqueda-escalable-enviar-grupos
- Clasificacion: cross_cutting

## Contratos e integraciones

El método ASMX BuscarDestinosEnvioGrupo recibe { idTarea, termino, pagina, tamanoPagina }. Devuelve IdTarea, TokenVersion, Pagina, TamanoPagina, TieneMas, Destinos y Error. Cada destino se limita a IdActividadDestino, NombreActividad y GrupoDestino resumido.

PreviewEnviarGrupo mantiene { idTarea } y añade los metadatos de primera página. EjecutarEnvioGrupo permanece con { idTarea, idActividadDestino, tokenVersion }. La búsqueda nunca envía IdConector, sesión, permisos, SQL, secretos o datos de otras rutas. Los errores de longitud de término usan WORKFLOW_GROUP_SEARCH_TERM_INVALID y los demás bloqueos conservan los códigos públicos del módulo.
