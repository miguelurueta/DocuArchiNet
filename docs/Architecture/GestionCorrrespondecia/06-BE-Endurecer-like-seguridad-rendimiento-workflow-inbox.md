# PROMPT ARQUITECTONICO Ticket 06 BE

# Endurecer busqueda LIKE en WorkflowInboxQueryBuilder

## Rol esperado

Arquitecto de software senior backend (.NET, SQL, seguridad, performance, query builders).

## Objetivo

Endurecer la implementacion de busqueda `LIKE` en `WorkflowInboxQueryBuilder` para reducir riesgos de inyeccion, degradacion de rendimiento y exposicion de campos no autorizados, manteniendo compatibilidad con el contrato dinamico de columnas.

## Contexto existente

- Documento tecnico:
  - `docs/Architecture/GestionCorrrespondecia/WorkflowInbox-Busqueda-Autocomplete-Architecture.md`
- Query builder:
  - `MiApp.Services/Service/Workflow/BandejaCorrespondencia/WorkflowInboxQueryBuilder.cs`
- Metodo actual:
  - `ApplyLikeSearch(search, dynamicColumns)`

## Ubicacion esperada

```txt
MiApp.Services/Service/Workflow/BandejaCorrespondencia/WorkflowInboxQueryBuilder.cs
MiApp.Services/Service/Workflow/BandejaCorrespondencia/*
Tests backend de WorkflowInboxQueryBuilder
```

## Restricciones obligatorias

- no usar columnas enviadas por el cliente
- no aplicar `LIKE` sobre todos los campos indiscriminadamente
- no aplicar `CAST` masivo sobre fechas o numeros sin aprobacion explicita
- no deshabilitar paginacion
- no generar SQL invalido cuando no hay columnas filtrables
- no degradar exportacion o conteo

## Reglas de implementacion obligatorias

1. Resolver columnas solo desde metadata validada.
2. Filtrar columnas por:
   - `IsVisible = true`
   - `IsFilterable = true`
   - tipo textual segun politica existente
3. Escapar caracteres especiales de `LIKE`:
   - `%`
   - `_`
   - `[`
   - `]`
   - comillas cuando aplique
4. Preferir parametros SQL si `QueryOptions` y el motor de datos lo soportan.
5. Si no hay soporte de parametros, encapsular el escape en una utilidad testeada.
6. Aplicar limite defensivo de longitud de `Search` si el contrato backend lo permite.
7. Mantener la misma condicion de filtro para rows, count y export.
8. Documentar cualquier comportamiento que quede como deuda por limitaciones del engine actual.

## Riesgos a evitar

- SQL injection por texto de busqueda
- SQL injection por nombres de columnas
- `LIKE '%%'` accidental
- condiciones `OR` sobre demasiadas columnas
- busqueda en campos no visibles
- divergencia entre consulta de filas y conteo
- escape inconsistente entre pruebas y runtime

## Pruebas unitarias obligatorias

- escapa `%` correctamente
- escapa `_` correctamente
- escapa corchetes correctamente si aplica a SQL Server
- no incluye columnas invisibles
- no incluye columnas no filtrables
- no incluye columnas no textuales
- no genera condicion cuando `Search` es whitespace
- no genera SQL invalido sin columnas elegibles
- rows y count reciben condiciones equivalentes

## Pruebas de integracion / calidad

- busqueda con caracteres especiales no rompe la consulta
- busqueda con texto largo aplica limite o manejo documentado
- busqueda mantiene paginacion y total filtrado
- exportacion filtrada usa la misma condicion
- rendimiento se mantiene aceptable con metadata representativa

## Criterios de aceptacion

- busqueda `LIKE` queda protegida contra caracteres especiales
- columnas se resuelven solo desde metadata segura
- filas, conteo y exportacion usan filtros consistentes
- no se aplica busqueda sobre campos no autorizados
- pruebas backend cubren casos de seguridad y borde

