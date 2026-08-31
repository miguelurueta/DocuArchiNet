# Contrato de lectura de Notas Workflow

## Autorización común

Los tres métodos se publican en `WebServiceWorkflowNotesModern.asmx` con sesión habilitada. El servidor obtiene actor, grupo y ruta desde la sesión autenticada, calcula el permiso de anotaciones y comprueba que `idTarea` esté activa y asignada a la ruta del actor.

Una respuesta bloqueada devuelve `Exito=false`, `Codigo` y `CodigoBloqueo`, con colecciones vacías y sin detalles internos. No diferencia entre una nota inexistente y una nota perteneciente a otro contexto.

## ListarNotas

Entrada JSON:

```json
{ "idTarea": 843, "cursor": "", "tamanoPagina": 25 }
```

La primera página puede usar `cursor` vacío. El servidor convierte `tamanoPagina=0` en 25 y rechaza tamaños negativos o superiores a 50. El cursor protegido contiene tarea, versión, usuario, grupo, ruta y la clave de continuación; no puede reutilizarse entre sesiones o tareas. La respuesta expone `Notas`, `TieneMas` y `CursorSiguiente`, sin contenido de la nota.

## ConsultarNota

Entrada JSON:

```json
{ "idTarea": 843, "idNota": 17 }
```

La consulta parametrizada exige simultáneamente `ID_ANOTACION`, `INICIO_TAREAS_WORKFLOW_ID_TAREA` y la visibilidad operativa. La respuesta solo contiene `Nota` si todas las condiciones se cumplen.

## ContarNotas

Entrada JSON:

```json
{ "idTarea": 843 }
```

El resultado usa `COUNT(*)` y los mismos predicados de tarea y visibilidad del listado; no materializa filas. Los consumidores deben espaciar el sondeo al menos 30 segundos hasta que exista el mecanismo de eventos de una fase posterior.

## Riesgos y reversión

El principal riesgo es que el esquema legacy almacene fechas sin zona horaria. El cursor conserva la misma clave leída por el repositorio y no convierte un valor de negocio en entrada de SQL. La reversión se limita a retirar el ASMX especializado y sus piezas modernas: los endpoints y páginas legacy no se modifican.
