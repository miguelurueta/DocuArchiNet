# BUSQUEDA-ESCALABLE-ENVIAR-GRUPOS

- Ticket: DOC-26
- Cambio OpenSpec: doc-26-busqueda-escalable-enviar-grupos
- Clasificacion: cross_cutting

## Servicios y reglas

BuscarDestinosEnvioGrupo compone el contexto autenticado existente y ServicioEnvioGrupoTarea. El validador normaliza página, tamaño y término antes de Infrastructure: término vacío o de 2 a 80 caracteres, página mínima uno y tamaño entre uno y cincuenta con inicio de veinticinco.

MySqlEnvioGrupoRepository vuelve a validar tarea, ruta y flujo, ejecuta exclusivamente SELECT parametrizados y agrupa por actividad. El filtro de grupo se resuelve con EXISTS y la proyección entrega nombre único o cantidad de grupos asociados. Se consulta una fila adicional para TieneMas, sin COUNT por interacción ni cambios de esquema. El endpoint no audita, no toma lock y no modifica la tarea; la ejecución existente conserva esas responsabilidades.
