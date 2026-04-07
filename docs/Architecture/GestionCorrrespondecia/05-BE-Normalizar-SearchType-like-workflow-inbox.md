# PROMPT ARQUITECTONICO Ticket 05 BE

# Normalizar contrato SearchType para busqueda LIKE en Workflow Inbox

## Rol esperado

Arquitecto de software senior backend (.NET, C#, APIs enterprise, contratos DTO, seguridad en queries).

## Objetivo

Normalizar el contrato de busqueda simple de `SolicitaBandejaWorkflow` para que `SearchType = 2` active de forma explicita la busqueda global tipo `LIKE` sobre campos textuales visibles y filtrables, conservando el comportamiento legacy de `SearchType = 1` y la busqueda avanzada `SearchType = 3`.

## Contexto existente

- Documento tecnico:
  - `docs/Architecture/GestionCorrrespondecia/WorkflowInbox-Busqueda-Autocomplete-Architecture.md`
- Controller backend:
  - `DocuArchi.Api/Controllers/WorkflowInboxGestion/WorkflowInboxController.cs`
- Servicio backend:
  - `MiApp.Services/Service/Workflow/BandejaCorrespondencia/WorkflowInboxService.cs`
- Query builder:
  - `MiApp.Services/Service/Workflow/BandejaCorrespondencia/WorkflowInboxQueryBuilder.cs`

## Ubicacion esperada

```txt
DocuArchi.Api/Controllers/WorkflowInboxGestion/WorkflowInboxController.cs
MiApp.Services/Service/Workflow/BandejaCorrespondencia/WorkflowInboxService.cs
MiApp.Services/Service/Workflow/BandejaCorrespondencia/WorkflowInboxQueryBuilder.cs
DTOs relacionados con WorkflowInboxApiRequestDto
Tests backend relacionados con WorkflowInboxQueryBuilder y WorkflowInboxService
```

## Restricciones obligatorias

- no cambiar la ruta existente `POST /api/workflowInboxgestion/inboxgestion`
- no romper clientes que envian `SearchType = 1`
- no alterar el contrato de busqueda avanzada `SearchType = 3`
- no buscar sobre columnas no visibles o no filtrables
- no buscar sobre columnas no textuales salvo decision tecnica documentada
- no mover logica SQL al controller
- no omitir validaciones de claims existentes

## Contrato obligatorio

```txt
SearchType = 1 -> comportamiento legacy/default sin busqueda global LIKE
SearchType = 2 -> busqueda global LIKE sobre columnas textuales visibles/filtrables
SearchType = 3 -> busqueda avanzada por expresion controlada
```

Cuando el request incluya:

```json
{
  "Search": "abc",
  "SearchType": 2
}
```

el backend debe aplicar `LIKE` global en las columnas dinamicas elegibles.

## Reglas de implementacion obligatorias

1. Mantener `SolicitaBandejaWorkflow` como endpoint de listado paginado.
2. Delegar toda la logica de busqueda al servicio/query builder.
3. Asegurar que `SearchType = 2` invoque la ruta `ApplyLikeSearch`.
4. Mantener `SearchType = 1` como comportamiento legacy documentado.
5. Mantener `SearchType = 3` para busqueda avanzada.
6. Si `Search` esta vacio o whitespace, no aplicar `LIKE`.
7. Si no existen columnas elegibles, no romper la consulta.
8. Registrar o documentar el comportamiento de valores `SearchType` desconocidos.

## Riesgos a evitar

- activar `LIKE` accidentalmente para `SearchType = 1`
- romper busqueda avanzada
- aplicar filtros sobre columnas no autorizadas
- concatenar nombres de columnas desde el cliente
- retornar errores cuando no hay columnas filtrables
- cambiar paginacion o conteo por accidente

## Pruebas unitarias obligatorias

- `SearchType = 2` con `Search` aplica condicion `LIKE`
- `SearchType = 2` con `Search` vacio no aplica condicion
- `SearchType = 2` sin columnas elegibles no rompe la consulta
- `SearchType = 1` conserva comportamiento legacy
- `SearchType = 3` conserva busqueda avanzada
- columnas no visibles no participan
- columnas no filtrables no participan
- columnas no textuales no participan

## Pruebas de integracion / calidad

- request paginado con `SearchType = 2` retorna filas filtradas
- total de registros respeta el filtro textual
- paginacion conserva el filtro al cambiar de pagina
- claims requeridos siguen siendo obligatorios
- respuesta conserva forma `AppResponses<DynamicUiTableDto>`

## Criterios de aceptacion

- `SearchType = 2` activa `ApplyLikeSearch`
- `SearchType = 1` y `SearchType = 3` no cambian su semantica
- busqueda global se limita a columnas textuales visibles/filtrables
- conteo y paginacion respetan el filtro
- cobertura backend valida el contrato

