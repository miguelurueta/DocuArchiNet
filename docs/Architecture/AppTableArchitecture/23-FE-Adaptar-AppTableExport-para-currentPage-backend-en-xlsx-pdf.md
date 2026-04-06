# PROMPT ARQUITECTÓNICO
Adaptar `AppTableExport` para usar backend en `currentPage` con `xlsx` y `pdf`

## Rol esperado

Arquitecto de software senior y desarrollador frontend React
(React 19 + TypeScript estricto + integracion reusable con backend)

## Resumen Jira sugerido

`[APPTABLE_EXPORT_23] Adaptar AppTableExport para currentPage backend en xlsx y pdf`

## Objetivo

Adaptar `AppTableExport` para que los formatos ejecutivos `xlsx` y `pdf` puedan ejecutarse tambien sobre `currentPage`, delegando la generacion de archivo al backend.

## Problema actual

La integracion actual de `AppTableExport` ya soporta estrategia hibrida:

- `csv` local para modos locales
- backend para `allMatching`

Pero `currentPage` en `xlsx` y `pdf` queda deshabilitado porque el contrato backend actual solo se usa para `allMatching`.

## Objetivo funcional

Permitir que el usuario exporte la pagina actual en `xlsx` y `pdf` desde el mismo menu reusable de `AppTableExport`, manteniendo:

- `csv` local cuando corresponda
- UX no destructiva
- contrato desacoplado del modulo concreto

## Alcance

- adaptar reglas de disponibilidad en `AppTableExport`
- extender el datasource reusable para soportar `currentPage` backend
- conectar el primer consumidor real con el nuevo modo
- mantener `allMatching` funcionando sin regresion

## No alcance

- no rediseñar el menu visual
- no migrar todos los modulos del sistema en este mismo ticket
- no mover `AppTableExport` dentro de `AppTable.tsx`
- no reemplazar `csv` local de `currentPage` si no aporta valor

## Reglas funcionales

- `currentPage` en `csv` puede seguir siendo local
- `currentPage` en `xlsx` y `pdf` debe preferir backend
- `allMatching` debe seguir soportando backend
- `selectedRows` puede mantenerse local mientras no exista contrato backend especifico
- si el datasource no soporta export backend para `currentPage`, la opcion no debe ejecutarse
- la tabla debe permanecer visible durante la exportacion
- el loading de exportacion debe seguir separado del loading de la tabla

## Reglas tecnicas

- la decision local/backend debe seguir siendo declarativa desde el datasource o estrategia inyectada
- `AppTableExport` no debe hardcodear `/api/AppTable/export`
- el componente shared solo debe decidir entre:
  - exportacion local
  - exportacion backend
- la estrategia backend debe poder recibir:
  - `format`
  - `mode`
  - columnas exportables
  - metadata del reporte
  - `queryState` contextual si el consumidor lo necesita
- la disponibilidad de subopciones en el dropdown debe reflejar capacidades reales por combinacion `formato + modo`

## Reglas de decision sugeridas

```txt
csv + currentPage       -> local
csv + selectedRows      -> local
csv + allLoaded         -> local
csv + allMatching       -> backend o estrategia ya definida
xlsx + currentPage      -> backend
pdf + currentPage       -> backend
xlsx + allMatching      -> backend
pdf + allMatching       -> backend
```

## Contrato sugerido

El datasource reusable debe poder declarar una estrategia backend no solo por existencia global, sino por capacidad efectiva segun `mode` y `format`.

Ejemplo conceptual:

```ts
type AppTableBackendExportRequest<T> = {
  columns: AppTableExportColumn<T>[];
  format: AppTableExportFormat;
  mode: AppTableExportMode;
  reportMeta: AppTableExportReportMeta;
  fileName?: string;
};

type AppTableExportDataSource<T> = {
  getCurrentPageRows: () => T[];
  getSelectedRows?: () => T[];
  getAllLoadedRows?: () => T[];
  getAllMatchingRows?: () => Promise<T[]>;
  getBackendExportFile?: (
    request: AppTableBackendExportRequest<T>,
  ) => Promise<AppTableBackendExportFile>;
  canExportWithBackend?: (
    request: Pick<AppTableBackendExportRequest<T>, "format" | "mode">,
  ) => boolean;
};
```

Nota:

- si el contrato actual ya permite inferirlo sin agregar `canExportWithBackend`, la implementacion puede resolverlo desde el adapter
- lo importante es que `currentPage` en `xlsx/pdf` no vuelva a quedar gris por una regla global incorrecta

## Primer consumidor esperado

`GestionCorrespondencia` debe ser el primer modulo de referencia para validar:

- `currentPage` en `xlsx`
- `currentPage` en `pdf`
- `allMatching` en `csv/xlsx/pdf`

El adapter del modulo debe mapear el `queryState` activo hacia el request backend respetando:

- `Page`
- `PageSize`
- `Search`
- `SearchType`
- `StructuredFilters`
- `SortField`
- `SortDir`
- `ReportTitle`
- `ExportMode`
- `Format`

Regla de semantica:

- para `currentPage`, el adapter debe enviar el `Page` actual visible y su `PageSize`
- para `allMatching`, el adapter debe usar la estrategia total ya definida

## Archivos esperados

- `src/app/Components/UI/AppTable/AppTableExport.tsx`
- `src/app/Components/UI/AppTable/AppTableExport.types.ts`
- servicios/adapters de exportacion backend
- hook o adapter del modulo consumidor real
- pruebas de `AppTableExport`
- pruebas de integracion del consumidor

## Riesgos a evitar

- introducir una regla global que habilite `xlsx/pdf` localmente sin soporte real
- mezclar `currentPage` con `allMatching`
- acoplar el shared component al endpoint concreto del modulo
- romper `csv` local que hoy ya funciona
- dejar visibles opciones ejecutivas que no sean ejecutables

## Pruebas obligatorias

- `currentPage` en `xlsx` usa backend
- `currentPage` en `pdf` usa backend
- `csv` en `currentPage` sigue funcionando localmente
- `allMatching` sigue funcionando en backend
- error backend recupera estado interactivo
- loading de exportacion no activa skeleton ni overlay de tabla
- dropdown refleja correctamente capacidades por formato y modo

## Criterios de aceptación

- `AppTableExport` permite `currentPage` en `xlsx/pdf` cuando el datasource real lo soporta
- `csv` local no se rompe
- el primer consumidor real queda conectado end-to-end con el contrato backend extendido
- la UX sigue siendo reusable y no destructiva

## Conclusión

Este ticket completa la matriz funcional de exportacion ejecutiva sin sacrificar la abstraccion reusable que ya se construyo en `AppTableExport`.
