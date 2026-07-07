# SCRUMCORE-295 - Busqueda Y Lista Completa

## Decision Final

El listado documental de `DocumentosWorkbench` opera como lista completa:

- Backend: `EnablePagination=false`.
- UI: sin controles de pagina.
- Busqueda: local sobre todas las filas recibidas.
- Request backend en modo full-list: `Search=""`.

## Motivo

La busqueda backend no estaba entregando el comportamiento esperado para la pantalla. Como el flujo ya trae todas las filas, el filtro local es mas deterministico y evita que una interpretacion backend de `Search` oculte filas antes de que la UI pueda mostrarlas.

## Algoritmo De Busqueda

1. Recibir todas las filas del scope base.
2. Consolidar texto de cada fila usando:
   - `RowId`
   - valores de `Values`
   - valores de `Meta`
3. Normalizar texto:
   - convertir a string
   - eliminar acentos con Unicode NFD
   - convertir a minusculas con locale `es-CO`
4. Dividir el termino de busqueda por espacios.
5. Una fila coincide si todos los tokens aparecen en el texto consolidado.

## Campos Incluidos

| Campo | Incluido |
|---|---|
| `RowId` | Si |
| `Values.*` | Si |
| `Meta.*` | Si |
| Labels renderizados no presentes en datos | No |
| Nombre/extension inferidos fuera del payload | No |

## Comportamiento

| Caso | Resultado |
|---|---|
| Search vacio | Se muestran todas las filas. |
| Search con texto | Se muestran filas que contienen todos los tokens. |
| Acentos | Se comparan de forma accent-insensitive. |
| Mayusculas/minusculas | Se comparan de forma case-insensitive. |
| Numeros | Se convierten a string y participan en la busqueda. |
| Booleanos | Se convierten a string. |
| Null/undefined | Se ignoran como string vacio. |

## Total Durante Busqueda

Cuando hay busqueda activa, el total visible es `model.rows.length` despues del filtro local.

Cuando no hay busqueda, el total vuelve a resolverse desde backend:

1. `meta.total`
2. `meta.Total`
3. `data.pagination.total`
4. `data.Pagination.Total`
5. `rows.length`

## Reglas De Request Durante Busqueda

Aunque el usuario escriba texto, en modo full-list el request backend mantiene:

```json
{
  "EnablePagination": false,
  "Search": "",
  "Page": 1
}
```

El texto se conserva en `queryState.search` y se aplica despues de recibir las filas.

## Impacto UX

- El usuario ve resultados consistentes con los datos disponibles.
- No aparecen controles de `10/25/50 por pagina`.
- No hay botones de pagina anterior/siguiente.
- El contador refleja el resultado real del filtro local.
