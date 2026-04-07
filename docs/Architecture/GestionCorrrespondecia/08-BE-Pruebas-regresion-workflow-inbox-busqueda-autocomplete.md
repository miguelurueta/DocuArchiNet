# PROMPT ARQUITECTONICO Ticket 08 BE

# Pruebas de regresion backend para busqueda y autocomplete Workflow Inbox

## Rol esperado

Arquitecto de software senior backend / QA tecnico (.NET, pruebas unitarias, pruebas de integracion, contratos API).

## Objetivo

Consolidar pruebas backend que validen el comportamiento de busqueda global `LIKE`, conteo filtrado, paginacion, exportacion filtrada y autocomplete de Workflow Inbox Gestion Correspondencia.

## Contexto existente

- Documento tecnico:
  - `docs/Architecture/GestionCorrrespondecia/WorkflowInbox-Busqueda-Autocomplete-Architecture.md`
- Tickets relacionados:
  - `05-BE-Normalizar-SearchType-like-workflow-inbox.md`
  - `06-BE-Endurecer-like-seguridad-rendimiento-workflow-inbox.md`
  - `07-BE-Endpoint-autocomplete-workflow-inbox.md`

## Ubicacion esperada

```txt
tests backend de DocuArchi.Api
tests backend de MiApp.Services
tests de WorkflowInboxController
tests de WorkflowInboxService
tests de WorkflowInboxQueryBuilder
```

## Restricciones obligatorias

- no depender de datos productivos
- no usar fixtures con informacion sensible
- no validar solo strings exactos de SQL cuando exista una abstraccion mas estable
- no dejar pruebas fragiles por orden no deterministico
- no omitir casos de seguridad por caracteres especiales

## Matriz de pruebas obligatoria

### SearchType

- `SearchType = 1` conserva comportamiento legacy
- `SearchType = 2` aplica busqueda global `LIKE`
- `SearchType = 3` conserva busqueda avanzada
- `SearchType` desconocido tiene comportamiento controlado

### Columnas elegibles

- columnas visibles y filtrables textuales participan
- columnas invisibles no participan
- columnas no filtrables no participan
- columnas no textuales no participan
- ausencia de columnas elegibles no rompe la consulta

### Texto de busqueda

- texto normal aplica filtro
- texto vacio no aplica filtro
- whitespace no aplica filtro
- `%` se maneja de forma segura
- `_` se maneja de forma segura
- corchetes se manejan de forma segura cuando aplique
- comillas no rompen la consulta

### Conteo y paginacion

- total filtrado corresponde a la misma condicion de rows
- cambiar de pagina conserva filtro
- cambiar page size conserva filtro
- rows y count no divergen

### Exportacion

- exportacion filtrada usa la misma condicion que listado
- `allMatching` respeta `Search` y `SearchType = 2` si aplica
- exportacion no ignora filtros estructurados existentes

### Autocomplete

- menor a `minLength` no consulta o retorna lista controlada
- respeta `limit` maximo
- retorna sugerencias distintas
- no retorna filas completas
- respeta columnas visibles/filtrables/textuales
- respeta claims y contexto workflow

## Riesgos a evitar

- cobertura centrada solo en caso feliz
- pruebas que pasen aunque el filtro no se aplique al count
- no detectar duplicados en autocomplete
- no detectar busqueda sobre columnas no autorizadas
- no detectar doble semantica entre `SearchType = 1` y `2`

## Criterios de aceptacion

- pruebas cubren `SearchType`, columnas, texto, conteo, paginacion, exportacion y autocomplete
- casos de seguridad con caracteres especiales estan cubiertos
- autocomplete tiene pruebas de limites y autorizacion
- pruebas documentan comportamiento legacy y nuevo
- suite backend puede ejecutarse sin datos productivos

