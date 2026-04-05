# PROMPT ARQUITECTÓNICO
Definir estrategia backend de exportacion total `allMatching`

## Rol esperado

Arquitecto de software senior y desarrollador backend/frontend
(arquitectura enterprise + contratos API + integracion con frontend)

## Objetivo

Definir la estrategia backend para exportar todos los resultados de una consulta activa, respetando filtros, busqueda y ordenamiento.

## Problema actual

En `server mode`, el frontend solo conoce la pagina actual.
Por eso no puede exportar correctamente “todo” usando solo `rows`.

## Objetivo funcional

Permitir exportar todos los resultados que hacen match con la consulta actual:

- `search`
- `searchType`
- `structuredFilters`
- `sortField`
- `sortDir`

La estrategia backend tambien debe contemplar el encabezado formal del reporte y sus metadatos corporativos.

## Alcance

- definir contrato backend de exportacion total
- decidir si devuelve archivo final o dataset completo
- alinear semantica con query state actual

## No alcance

- no implementar todavia la UI frontend
- no limitarse a un modulo si se busca estrategia reusable
- no usar paginado incremental desde navegador como solucion principal

## Opciones a evaluar

### Opcion 1. Endpoint de exportacion directa
- recibe query state
- responde archivo final

### Opcion 2. Endpoint de lectura total
- recibe query state
- responde dataset completo
- frontend genera archivo

## Recomendacion

Preferir endpoint de exportacion directa para:

- `xlsx`
- `pdf`

Porque:

- reduce carga en frontend
- evita grandes payloads en memoria
- facilita formatos complejos

## Reglas obligatorias

- la exportacion debe respetar exactamente la consulta activa
- no debe recalcularse con otra semantica
- no debe depender del page actual
- debe ser consistente con `Pagination.Total`
- debe poder incorporar metadata del reporte en el archivo final
- debe poder resolver la imagen corporativa desde un recurso estable y controlado
- debe incrustar la imagen corporativa dentro del archivo final y no dejar una referencia URL
- la ruta corporativa por defecto debe alinearse con la convencion del repo:
  - `public/branding/reports/company-report-logo.png`

Regla por formato backend:

- `xlsx`
  - soportar encabezado ejecutivo con logo embebido
- `pdf`
  - soportar encabezado ejecutivo con logo embebido
- `csv`
  - no requiere embebido de imagen; la estrategia debe priorizar compatibilidad del formato

## Archivos esperados

- documento tecnico backend o contrato API
- eventual endpoint/documentacion asociada

## Riesgos a evitar

- exportar un conjunto distinto al visible
- depender del page actual
- exponer un endpoint no reusable
- formatos inconsistentes entre modulos

## Pruebas obligatorias

- exportacion con filtros
- exportacion con sort
- exportacion con busqueda simple
- exportacion con busqueda avanzada
- encabezado del reporte con metadata obligatoria
- inclusion correcta de la imagen corporativa

## Criterios de aceptación

- existe estrategia backend clara para `allMatching`
- respeta el mismo query state de la tabla
- no depende del front paginado

## Conclusión

Sin este ticket, `allMatching` sera ambiguo o incorrecto en tablas server-side.
