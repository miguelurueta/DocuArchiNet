# PROMPT ARQUITECTONICO Ticket 07 BE

# Crear endpoint de autocomplete para Workflow Inbox

## Rol esperado

Arquitecto de software senior backend (.NET, C#, APIs REST, seguridad, performance).

## Objetivo

Crear un endpoint backend dedicado para autocomplete de tareas workflow de gestion de correspondencia, retornando sugerencias limitadas y seguras sin reutilizar el endpoint paginado completo de listado.

## Contexto existente

- Documento tecnico:
  - `docs/Architecture/GestionCorrrespondecia/WorkflowInbox-Busqueda-Autocomplete-Architecture.md`
- Controller:
  - `DocuArchi.Api/Controllers/WorkflowInboxGestion/WorkflowInboxController.cs`
- Servicio:
  - `MiApp.Services/Service/Workflow/BandejaCorrespondencia/WorkflowInboxService.cs`
- Repository/query builder:
  - componentes backend de `WorkflowInbox`

## Ubicacion esperada

```txt
DocuArchi.Api/Controllers/WorkflowInboxGestion/WorkflowInboxController.cs
MiApp.Services/Service/Workflow/BandejaCorrespondencia/WorkflowInboxService.cs
MiApp.Services/Service/Workflow/BandejaCorrespondencia/WorkflowInboxRepository.cs
MiApp.Services/Service/Workflow/BandejaCorrespondencia/WorkflowInboxQueryBuilder.cs
DTOs de request/response para autocomplete
Tests backend relacionados
```

## Ruta sugerida

```txt
POST /api/workflowInboxgestion/inboxgestion/autocomplete
```

## Contrato sugerido

Request:

```csharp
public sealed class WorkflowInboxAutocompleteRequestDto
{
    public string? Search { get; set; }
    public int? Limit { get; set; }
}
```

Response:

```csharp
public sealed class WorkflowInboxAutocompleteResponseDto
{
    public IReadOnlyList<WorkflowInboxAutocompleteItemDto> Items { get; set; } = [];
}

public sealed class WorkflowInboxAutocompleteItemDto
{
    public string Value { get; set; } = string.Empty;
    public string? Label { get; set; }
    public string? Field { get; set; }
}
```

## Restricciones obligatorias

- no retornar filas completas de tareas workflow
- no exponer campos no visibles o no filtrables
- no omitir validacion de claims
- no permitir `limit` sin maximo
- no consultar si `Search` es menor al minimo definido
- no duplicar logica de resolucion de contexto workflow
- no reemplazar el endpoint de listado paginado

## Reglas de implementacion obligatorias

1. Agregar accion nueva en `WorkflowInboxController`.
2. Reutilizar o extraer validacion comun de claims del endpoint existente.
3. Reutilizar resolucion de contexto workflow del servicio.
4. Crear DTOs especificos para autocomplete.
5. Aplicar `minLength` backend, recomendado 2 o 3.
6. Aplicar `limit` maximo, recomendado 10 o 20.
7. Buscar solo en columnas textuales visibles y filtrables.
8. Retornar sugerencias distintas.
9. Ordenar sugerencias de forma estable.
10. Retornar `AppResponses<WorkflowInboxAutocompleteResponseDto>`.

## Riesgos a evitar

- usar autocomplete como exportacion accidental
- filtrar informacion de campos no visibles
- duplicar reglas de claims
- no limitar resultados
- consultas pesadas por cada tecla
- acoplar response a estructura interna de tabla dinamica

## Pruebas unitarias obligatorias

- request menor a `minLength` retorna lista vacia o respuesta controlada
- `limit` mayor al maximo se recorta
- campos no visibles no generan sugerencias
- campos no filtrables no generan sugerencias
- campos no textuales no generan sugerencias
- sugerencias retornan `Value` no vacio
- sugerencias se retornan sin duplicados
- errores de claims mantienen comportamiento del controller

## Pruebas de integracion / calidad

- endpoint responde con `AppResponses<WorkflowInboxAutocompleteResponseDto>`
- endpoint respeta claims requeridos
- endpoint no devuelve filas completas
- endpoint respeta limite maximo
- endpoint mantiene tiempo de respuesta aceptable con metadata representativa

## Criterios de aceptacion

- existe endpoint dedicado de autocomplete
- autocomplete usa mismo contexto y claims del workflow inbox
- response contiene sugerencias limitadas y seguras
- no se reutiliza el endpoint paginado para sugerencias
- pruebas backend cubren contrato, seguridad y limites

