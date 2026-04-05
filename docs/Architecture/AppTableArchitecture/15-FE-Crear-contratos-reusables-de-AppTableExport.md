# PROMPT ARQUITECTÓNICO
Crear contratos reusables de `AppTableExport`

## Rol esperado

Arquitecto de software senior y desarrollador frontend React
(React 19 + TypeScript estricto + arquitectura enterprise)

## Objetivo

Definir los contratos base de exportacion reutilizable para el ecosistema `AppTable`, sin acoplarlos a `GestionCorrespondencia` ni a un backend especifico.

## Problema actual

Hoy existen triggers visuales de exportacion en algunas pantallas, pero no existe un contrato comun para:

- tipos de exportacion
- formatos soportados
- datasource de filas
- capacidades locales vs server-side

Eso obliga a cada modulo a inventar su propio modelo.

## Objetivo funcional

Crear una base tipada y reusable para futuras implementaciones de exportacion en tablas.

## Alcance

- definir `AppTableExportMode`
- definir `AppTableExportFormat`
- definir `AppTableExportDataSource`
- definir `AppTableExportProps`
- definir metadata reusable del reporte exportado
- definir reglas de disponibilidad por modo

## No alcance

- no generar archivos aun
- no integrar UI final
- no conectar backend real
- no migrar pantallas consumidoras

## Contratos esperados

```ts
type AppTableExportMode =
  | "currentPage"
  | "selectedRows"
  | "allLoaded"
  | "allMatching";

type AppTableExportFormat = "csv" | "xlsx" | "pdf";

type AppTableExportDataSource<T> = {
  getCurrentPageRows: () => T[];
  getSelectedRows?: () => T[];
  getAllLoadedRows?: () => T[];
  getAllMatchingRows?: () => Promise<T[]>;
};

type AppTableExportReportMeta = {
  reportName: string;
  generatedBy: string;
  moduleName: string;
  reportType: string;
  generatedAt: string;
  rowCount: number;
  description: string;
  companyImageAsset: string;
};
```

La metadata del reporte debe permitir construir un encabezado ejecutivo consistente entre formatos.
El asset corporativo debe insertarse dentro del archivo exportado y no quedar como una URL referenciada.

Convencion recomendada del asset corporativo:

- ruta estable en repo: `public/branding/reports/company-report-logo.png`
- el contrato debe permitir override controlado, pero ese asset debe ser el default institucional
- el flujo de exportacion debe convertir ese asset en contenido embebido dentro del reporte final

## Reglas funcionales

### `currentPage`
- siempre representa filas visibles actualmente

### `selectedRows`
- requiere seleccion activa

### `allLoaded`
- solo aplica a datasets cargados en memoria

### `allMatching`
- requiere estrategia server-side o datasource adicional

## Archivos esperados

- `src/app/Components/UI/AppTable/AppTableExport.types.ts`
- `src/app/Components/UI/AppTable/` si se requiere indexado adicional
- pruebas unitarias de contratos o helpers si aplica

## Riesgos a evitar

- acoplar contratos a un modulo concreto
- confundir `allLoaded` con `allMatching`
- asumir backend obligatorio para todos los casos
- dejar ambigua la semantica de cada modo
- dejar el encabezado del reporte como detalle informal por pantalla
- depender de logos o imagenes fuera del repo

## Pruebas obligatorias

- disponibilidad de tipos
- helpers de resolucion de modos si se crean
- compatibilidad con `AppTableRow`

## Criterios de aceptación

- existe contrato reusable para exportacion
- no depende de una pantalla concreta
- distingue claramente modos locales y server-side
- queda listo para integracion posterior
- incluye metadata formal del reporte y referencia corporativa desde el repo

## Conclusión

Este ticket debe hacerse primero.
Sin este contrato, la exportacion terminara fragmentada por pantalla.
