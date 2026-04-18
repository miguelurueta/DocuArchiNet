# PROMPT ARQUITECTONICO Ticket 12 FE

# Revision de performance enfocada en rerenders innecesarios en GestionCorrespondencia

## Rol esperado

Arquitecto de software senior frontend
(React, performance rendering, estabilidad referencial, profiling, tablas enterprise, hooks, patrones de composicion)

## Archivo objetivo

- `src/modules/gestionCorrespondencia/pages/GestionCorrespondencia.tsx`

## Objetivo

Realizar una revision de performance enfocada exclusivamente en rerenders innecesarios en `GestionCorrespondencia.tsx` y su arbol inmediato.

La revision debe:

- detectar si existen rerenders evitables en zonas sensibles de la pantalla
- identificar props, estados locales, callbacks, objetos, arrays o hooks que cambian de identidad en cada render
- distinguir entre rerender esperado y rerender innecesario
- proponer cambios concretos, minimos y seguros para reducir rerenders sin sobre-optimizar

## Alcance

Limitar el analisis a:

- el componente objetivo
- hijos directos
- props que fluyen desde este componente

Revisar especificamente este componente y los hijos directos que reciben props desde aqui:

- `AppToolbar`
- `AppInputSearch`
- `AppButton`
- `AppContent`
- `AppTableQueryWrapper`
- `AppTableExport`
- `AppTable`

Revisar tambien los hooks o contratos que alimentan este componente si afectan estabilidad referencial:

- `useWorkflowInboxAutocomplete`
- `GestionCorrespondenciaTableResult`
- `table.queryState`
- `table.rows`
- `table.columns`
- `table.onQueryChange`
- `table.refetch`
- `table.getAllMatchingRows`
- `table.getBackendExportFile`

## Focos obligatorios de analisis

1. Validar si `selectedRows` provoca rerenders amplios cuando solo cambia la seleccion.
2. Validar si `searchDraft` provoca rerenders costosos de toda la pagina en cada tecla.
3. Revisar si `exportReportMeta` sigue siendo realmente estable o si contiene valores que deberian recalcularse en otro momento.
4. Revisar si los objetos inline estan rompiendo memoizacion:
   - `actionContent`
   - `paginationActions`
   - `dataSource`
   - `responsivePresentation`
5. Revisar si las callbacks recreadas en cada render estan forzando rerenders en hijos:
   - `applySearch`
   - `handleSearchChange`
   - `handleSearchClear`
   - `navigateToRowDetail`
   - `handleTableAction`
   - `handleTableCellClick`
   - `setSelectedRows` como prop de `onSelectionChanged`
6. Revisar si `table.loading && table.hasLoadedOnce` y otros derivados deberian estabilizarse.
7. Revisar si `table.columns as ColDef<T>[]` o `table.rows` pueden cambiar de referencia aunque sus datos no cambien.
8. Revisar si `AppTableExport` y `AppTable` estan recibiendo props nuevas en cada render aunque semanticamente no hayan cambiado.
9. Revisar si hay riesgo de rerender por navegacion, autocomplete o cambios en `queryState`.
10. Evaluar si conviene usar `React.memo`, `useMemo`, `useCallback`, o si eso seria ruido innecesario.

## Medicion obligatoria

La respuesta debe indicar explicitamente:

- que hallazgos pueden confirmarse solo leyendo codigo
- que hallazgos requieren profiling

Para los casos que requieren profiling, debe:

- sugerir uso de React DevTools Profiler
- sugerir uso de `why-did-you-render` si aplica
- explicar que comportamiento se esperaria observar

## Contexto existente

- Componente objetivo:
  - `src/modules/gestionCorrespondencia/pages/GestionCorrespondencia.tsx`
- Ruta de carga:
  - `src/modules/gestionCorrespondencia/pages/GestionCorrespondenciaRoutePage.tsx`
- Hook principal de datos:
  - `src/modules/gestionCorrespondencia/hooks/useGestionCorrespondenciaTable.ts`
- Hook de autocomplete:
  - `src/modules/gestionCorrespondencia/hooks/useWorkflowInboxAutocomplete.ts`
- Pruebas asociadas:
  - `src/modules/gestionCorrespondencia/tests/GestionCorrespondencia.test.tsx`

## Observaciones iniciales

Estas observaciones son contexto de partida y no deben tomarse como conclusion:

- `searchDraft` y `selectedRows` viven en el page component, por lo que cualquier cambio rerenderiza toda la pagina
- existen objetos y JSX inline que cambian referencia por render: `actionContent`, `paginationActions`, `dataSource`, `responsivePresentation`
- las funciones `applySearch`, `handleSearchChange`, `handleSearchClear`, `handleTableAction` y `handleTableCellClick` se recrean en cada render
- `exportReportMeta` esta memoizado, pero incluye `new Date().toISOString()`, lo cual merece revision conceptual

## Restricciones obligatorias

- NO asumir que todo rerender es un problema
- NO recomendar memoizacion indiscriminada
- NO proponer sobre-optimizacion que aumente complejidad sin impacto real
- NO salir del alcance definido
- NO acoplar la evaluacion a componentes nietos salvo cuando el problema nazca en un hijo directo o contrato inmediato
- NO usar conclusiones categoricas cuando solo exista sospecha y falte profiling

## Reglas de evaluacion

- priorizar cambios con evidencia y bajo riesgo
- si un rerender no puede confirmarse solo leyendo codigo, indicarlo explicitamente
- si se detecta que el problema viene de un hook o componente hijo, declararlo de forma explicita
- si no hay problema en una zona, indicarlo claramente
- si existe buen diseno actual, reconocerlo
- diferenciar claramente problema real, sospecha razonable y falso positivo comun en React

## Formato obligatorio de respuesta

La respuesta debe incluir exactamente estas secciones:

### 1. Tabla de hallazgos

Con las columnas:

- Severidad
- Zona
- Causa del rerender
- Impacto esperado
- Costo de implementacion (`bajo`, `medio`, `alto`)
- Beneficio esperado (`bajo`, `medio`, `alto`)
- Recomendacion

### 2. Mapa de flujo de render del componente

Debe cubrir:

- que cambios de estado lo rerenderizan
- que props nuevas genera en cada render
- que hijos probablemente se rerenderizan por eso

### 3. Clasificacion de rerenders

Separar en:

- rerenders esperados
- rerenders sospechosos
- rerenders claramente evitables

### 4. Clasificacion adicional de cada hallazgo

Separar cada hallazgo como:

- problema real
- sospecha razonable
- falso positivo comun en React

### 5. Plan de refactor minimo en orden de prioridad

### 6. Riesgos de regresion por cada optimizacion

### 7. Estrategia de validacion

Debe incluir:

- que validar con tests existentes
- que tests nuevos sugerir
- que medir antes y despues con profiling

## Guia de analisis sugerida

El analisis debe revisar al menos:

```tsx
const [selectedRows, setSelectedRows] = useState<T[]>([]);
const [searchDraft, setSearchDraft] = useState(table.queryState.search);

const exportReportMeta = useMemo(
  () => ({
    reportName: "Bandeja de gestion correspondencia",
    generatedBy: "DocuArchiCore",
    moduleName: "Gestion Correspondencia",
    reportType: "Operativo",
    generatedAt: new Date().toISOString(),
    rowCount: table.rows.length,
    description: "Exportacion desde la bandeja operativa",
    companyImageAsset: "public/branding/reports/company-report-logo.png",
  }),
  [table.rows.length],
);
```

y tambien props inline como:

```tsx
actionContent={...}
paginationActions={...}
dataSource={{ ... }}
responsivePresentation={{ enabled: true, cardsBelow: 768 }}
```

## Resultado esperado

El resultado final debe permitir decidir con rigor:

- si existe un problema real de rerender
- donde esta el origen
- cuales optimizaciones tienen mejor relacion costo/beneficio
- que optimizaciones deben rechazarse por ser ruido
- que puntos deben validarse con profiling antes de tocar codigo

## Instruccion final

Antes de concluir:

- leer `GestionCorrespondencia.tsx`
- revisar contratos inmediatos usados por el componente
- separar observaciones confirmables por lectura de codigo vs observaciones que requieren medicion

Finalmente reportar:

- hallazgos priorizados
- zonas sanas donde no conviene intervenir
- plan de refactor minimo
- riesgos
- estrategia de validacion y profiling
