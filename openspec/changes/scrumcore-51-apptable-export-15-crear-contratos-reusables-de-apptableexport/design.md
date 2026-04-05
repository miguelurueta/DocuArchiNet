## Context

`AppTable` ya evoluciono hacia un modelo reusable de consulta con:

- `paginationMode = "none" | "client" | "server"`
- `AppTableQueryWrapper` como contenedor visual de busqueda, refresh y paginacion
- soporte de seleccion y presentacion `table/cards`
- reglas claras de loading para first load y refetch

El siguiente paso de la serie es agregar una capacidad reusable de exportacion. El problema actual no es de rendering sino de contratos: hoy no existe una forma tipada y transversal de expresar:

- que se exporta
- desde donde salen las filas
- que formatos soporta la pantalla
- como se describe el reporte exportado
- cuando una opcion aplica a datos locales y cuando requiere backend

Si esa capa no se define primero, cada modulo termina resolviendo exportacion con contratos distintos y semanticas inconsistentes, especialmente en escenarios server-side.

Este ticket solo define la base contractual. No implementa generacion de archivos ni integra una UI final.

## Goals / Non-Goals

**Goals:**

- Definir contratos reutilizables para `AppTableExport`.
- Separar claramente los modos de exportacion `currentPage`, `selectedRows`, `allLoaded` y `allMatching`.
- Formalizar un datasource extensible que soporte tanto datos locales como estrategias server-side.
- Definir metadata institucional del reporte para encabezados ejecutivos y branding.
- Dejar una base de tipos que permita implementar frontend y backend sin rehacer la API.

**Non-Goals:**

- No generar archivos `csv`, `xlsx` o `pdf` en este ticket.
- No implementar el componente visual final de exportacion.
- No acoplar la solucion a `GestionCorrespondencia` ni a un endpoint concreto.
- No resolver todavia la estrategia backend definitiva de `allMatching`.
- No modificar `AppTable.tsx` ni `AppTableQueryWrapper.tsx` en esta fase.

## Decisions

### 1. Crear una capa contractual separada de `AppTable`

Se definira una capa de tipos dedicada, por ejemplo en:

- `src/app/Components/UI/AppTable/AppTableExport.types.ts`

Razon:

- `AppTable` es renderer y no debe absorber semantica de exportacion.
- Los contratos de exportacion deben poder reutilizarse desde componentes, hooks y adaptadores server-side.

Alternativas consideradas:

- Reutilizar `AppTable.types.ts`
  - descartado por mezclar contratos de rendering con contratos de exportacion
- Definir tipos por modulo
  - descartado por romper reusabilidad

### 2. Modelar el alcance de exportacion con un enum string explicito

Se definira:

```ts
type AppTableExportMode =
  | "currentPage"
  | "selectedRows"
  | "allLoaded"
  | "allMatching";
```

Razon:

- fuerza una semantica consistente entre pantallas
- evita ambiguedad entre datos visibles, datos cargados y datos totales

Alternativas consideradas:

- booleanos dispersos como `exportAll`, `exportSelected`, `exportPage`
  - descartado por crecer mal y ser ambiguo

### 3. Separar el origen de datos mediante un datasource inyectable

Se definira un contrato tipo:

```ts
type AppTableExportDataSource<T> = {
  getCurrentPageRows: () => T[];
  getSelectedRows?: () => T[];
  getAllLoadedRows?: () => T[];
  getAllMatchingRows?: () => Promise<T[]>;
};
```

Razon:

- permite usar el mismo `AppTableExport` en client-side y server-side
- evita que el componente conozca endpoints, hooks de dominio o `React Query`
- habilita capacidades opt-in segun lo que cada pantalla pueda resolver realmente

Alternativas consideradas:

- pasar directamente `rows` y `selectedRows` como props sueltas
  - descartado porque `allMatching` requiere una estrategia distinta
- hacer que `AppTableExport` consulte backend por su cuenta
  - descartado por acoplarlo a infraestructura y a casos de negocio

### 4. Formalizar metadata ejecutiva del reporte desde el contrato

Se definira un contrato como:

```ts
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

Razon:

- el encabezado ejecutivo no debe quedar hardcodeado por pantalla
- backend y frontend deben compartir una misma semantica institucional del reporte
- la referencia corporativa debe salir del repo y luego embebirse en el archivo final

Alternativas consideradas:

- construir encabezados ad hoc segun cada formato
  - descartado por inconsistencia
- resolver metadata solo en backend
  - descartado porque tambien se necesita en exportacion local

### 5. Diferenciar contrato de asset corporativo de la representacion final del logo

El contrato usara `companyImageAsset`, no `companyImageUrl`.

Razon:

- el origen del logo debe ser un asset versionado del repo
- el archivo final debe incrustar la imagen, no referenciarla por URL

Convencion institucional base:

- `public/branding/reports/company-report-logo.png`

Alternativas consideradas:

- guardar URL externa en Jira o en el modulo
  - descartado por fragilidad y dependencia externa

## Risks / Trade-offs

- [Riesgo] Diseñar contratos demasiado amplios desde el inicio
  → Mitigacion: mantener esta fase enfocada en tipos minimos y capacidades reales de la serie `15-21`.

- [Riesgo] Confundir `allLoaded` con `allMatching`
  → Mitigacion: documentar ambos modos como conceptos distintos y reflejarlo desde los nombres del contrato.

- [Riesgo] Introducir metadata institucional que despues no sea util para todos los formatos
  → Mitigacion: mantener la metadata comun y permitir que cada formato degrade de forma controlada, especialmente `csv`.

- [Riesgo] Que el frontend asuma backend obligatorio para exportar
  → Mitigacion: el datasource mantiene capacidades opcionales y soporta exportacion puramente local.

- [Riesgo] Que el contrato se acople a una sola tabla o modulo
  → Mitigacion: no incluir endpoints, DTOs de dominio ni referencias a `GestionCorrespondencia` en esta fase.

## Migration Plan

1. Crear el archivo de tipos base para exportacion en `AppTable`.
2. Exponer los contratos desde el barrel correspondiente si hace falta.
3. Agregar pruebas unitarias de tipos/helpers si se introducen funciones de resolucion.
4. Usar estos contratos como base del ticket `SCRUMCORE-52`, que implementa `currentPage` y `selectedRows`.
5. Mantener el resto del ecosistema sin cambios de runtime hasta la siguiente fase.

Rollback:

- al ser una fase contractual y sin impacto de runtime esperado, el rollback consiste en revertir el archivo de tipos y su export.

## Open Questions

- Si `AppTableExportFormat` debe incluir desde el inicio solo `csv | xlsx | pdf` o contemplar extensiones futuras.
- Si `generatedAt` debe normalizarse como ISO string o permitir formato pre-renderizado.
- Si `rowCount` debe venir siempre desde el emisor o permitir resolverse en el propio flujo de exportacion.
- Si el contrato de metadata debe incluir nombre de archivo sugerido o si eso queda para la fase de implementacion.
