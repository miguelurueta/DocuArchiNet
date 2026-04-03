# Ticket 01 BE

## Titulo

Normalizar `workflowInboxgestion` para paginacion consistente y claims reales

## Objetivo

Eliminar el hardcode actual del endpoint `POST /api/workflowInboxgestion/inboxgestion` y dejar el flujo listo para operar con contexto real de usuario, sin romper el contrato dinamico actual.

## Problema actual

- el controller usa valores hardcodeados para `idUsuarioGestion` y `defaultDbAlias`
- el endpoint no debe depender de esos valores para operar en entornos reales
- la respuesta vacia no siempre retorna una estructura de tabla consistente

## Alcance

- restaurar validacion real de claims en controller
- eliminar hardcodes temporales
- mantener el flujo actual:
  - controller
  - service
  - context resolver
  - repository
  - query builder
- devolver respuesta vacia estructurada y consistente con `DynamicUiTableDto`

## No alcance

- no rediseñar `DynamicUiTableDto`
- no cambiar la arquitectura del servicio
- no implementar aun el conteo total real de la consulta filtrada
- no rediseñar acciones dinamicas

## Dependencias

- ninguna funcional previa en frontend

## Archivos backend esperados

- `DocuArchi.Api/Controllers/WorkflowInboxGestion/WorkflowInboxController.cs`
- `MiApp.Services/Service/Workflow/BandejaCorrespondencia/WorkflowInboxService.cs`
- tests de controller y service

## Reglas de implementacion

- validar claim `defaulalias`
- validar claim `usuarioid`
- convertir `usuarioid` a entero
- si `usuarioid` no es valido, retornar error controlado consistente con el estandar actual del API
- si `defaulalias` no existe, retornar error controlado consistente con el estandar actual del API
- el controller solo debe leer y validar claims, luego delegar
- no resolver contexto workflow en controller
- no resolver en controller:
  - `IdUsuarioWorkflow`
  - `NombreRuta`
  - `IdActividad`
- si no hay filas, devolver `DynamicUiTableDto` con:
  - `Columns`
  - `Pagination`
  - `Sorting`
  - `Rows = []`
- en empty state, `Columns` deben mantenerse
- en empty state, `Pagination.Total = 0`
- en empty state, `Pagination.Page` debe ser consistente con el request o fallback actual
- en empty state, `Pagination.PageSize` debe ser consistente con el request o fallback actual
- en empty state, `Sorting` debe ser consistente con el request o fallback actual

## Riesgos a evitar

- volver a dejar hardcodes temporales en el controller
- devolver `data = null` en empty state si frontend ya espera tabla estructurada
- mover logica de negocio al controller
- romper el flujo actual de resolucion de contexto

## Pruebas obligatorias

- claim `defaulalias` ausente
- claim `usuarioid` ausente
- claim `usuarioid` invalido
- success con claims validos
- success con claims validos y respuesta vacia estructurada
- sin regresion en columnas, actions y metadata actual

## Criterios de aceptacion

- el endpoint deja de depender de hardcodes
- usa claims reales
- el empty state devuelve tabla valida y consistente
- no se rompe el contrato actual consumido por frontend
- el controller queda limitado a validacion de claims y delegacion del flujo
