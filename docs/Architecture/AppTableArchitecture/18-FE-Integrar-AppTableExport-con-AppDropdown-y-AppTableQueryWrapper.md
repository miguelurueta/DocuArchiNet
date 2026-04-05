# PROMPT ARQUITECTÓNICO
Integrar `AppTableExport` con `AppDropdown` y `AppTableQueryWrapper`

## Rol esperado

Arquitecto de software senior y desarrollador frontend React
(React 19 + TypeScript estricto + arquitectura enterprise)

## Objetivo

Integrar visualmente `AppTableExport` al ecosistema reusable de `AppTable`, aprovechando `AppDropdown` y el wrapper de consulta sin mover logica de negocio al renderer base.

## Problema actual

La exportacion puede terminar dispersa entre:

- toolbar de pagina
- wrappers puntuales
- botones ad hoc

Eso rompe consistencia visual y reusabilidad.

## Objetivo funcional

Ofrecer una experiencia visual integrada para exportacion dentro del sistema de tablas.

La implementacion grafica debe usar `AppDropdown` como componente estandar para mostrar las opciones de descarga.
Ademas, el trigger de descarga debe convivir en la misma fila funcional donde viven los controles de paginacion de la tabla.

## Alcance

- integrar `AppTableExport` con `AppDropdown`
- permitir uso en `headerActions` o zona equivalente
- mantener `AppTable.tsx` libre de logica de exportacion
- definir la estructura visual del menu de descarga
- acoplar exportacion al mismo entorno visual de la paginacion
- asegurar comportamiento responsive coherente

## No alcance

- no conectar aun backend de `allMatching`
- no acoplar la UI a `GestionCorrespondencia`
- no redefinir `AppDropdown`

## Reglas funcionales

- el trigger visual debe ser reusable
- el menu debe reflejar solo formatos y modos disponibles
- estados `loading` o `disabled` deben ser claros
- `AppDropdown` debe ser el patron visual oficial para las opciones de descarga
- el menu debe poder agrupar por formato y luego por alcance de exportacion
- el trigger de descarga debe vivir en la misma fila o bloque funcional de la paginacion
- en desktop debe verse integrado a la barra de consulta/paginacion
- en responsive debe reacomodarse sin separarse del entorno visual de la tabla
- durante la descarga no debe activarse `Skeleton Screen` de la tabla
- durante la descarga debe mantenerse visible el contenido actual de la tabla

Ejemplo esperado:

- `Exportar en Excel`
- `Exportar en PDF`
- `Pagina actual`
- `Seleccionados`
- `Todo cargado`
- `Todos los resultados`

## Reglas tecnicas

- `AppTableExport` debe poder vivir dentro o junto a `AppTableQueryWrapper`
- `AppTable` no debe instanciar exportacion internamente
- la integracion debe soportar futuras pantallas sin wiring duplicado
- `AppDropdown` no debe contener logica de exportacion; solo actua como trigger/menu visual
- la disponibilidad de opciones debe depender de capacidades reales del datasource
- `AppTableQueryWrapper` debe ofrecer una composicion que permita ubicar paginacion y descarga en la misma banda visual
- el layout responsive no debe degradar en toolbars separadas o desalineadas
- el estado de descarga debe expresarse en el trigger `Exportar` y/o en la banda de controles, no en el renderer de filas

Refactor minimo recomendado en `AppTableQueryWrapper`:

- mantener `header` para busqueda, refresh y acciones de cabecera reales
- crear una banda de controles de tabla compartida para:
  - rango visible
  - page size
  - prev / next
  - exportacion
- no mover esta logica a `AppTable.tsx`

Contrato minimo sugerido:

```ts
type AppTableQueryWrapperProps = {
  queryState: AppTableQueryState;
  onQueryChange: (patch: Partial<AppTableQueryState>) => void;
  onRefresh?: () => void;
  total: number;
  loading?: boolean;
  exportLoading?: boolean;
  headerActions?: ReactNode;
  paginationActions?: ReactNode;
  children: ReactNode;
  className?: string;
  pageSizeOptions?: number[];
  searchPlaceholder?: string;
  showSearch?: boolean;
};
```

Semantica:

- `headerActions`
  - acciones del bloque superior
  - uso secundario y no recomendado para acciones operativas de tabla
- `paginationActions`
  - acciones acopladas al entorno visual de la tabla
  - `AppTableExport` debe montarse aqui

Regla de prioridad:

- si una accion pertenece al flujo operativo de la tabla, debe preferirse `paginationActions`
- `headerActions` no debe reutilizarse para exportacion, paginacion o acciones que deban convivir visualmente con el bloque de tabla

Estructura JSX sugerida:

```tsx
<section className={styles.root}>
  <div className={styles.header}>
    <div className={styles.searchGroup}>...</div>
    {headerActions ? <div className={styles.headerActions}>{headerActions}</div> : null}
  </div>

  <div className={styles.controlsBand}>
    <div className={styles.paginationInfo}>
      <span className={styles.range}>...</span>
      <div className={styles.paginationActions}>...</div>
    </div>

    {paginationActions ? (
      <div className={styles.paginationSideActions}>{paginationActions}</div>
    ) : null}
  </div>

  <div className={styles.tableContainer}>{children}</div>
</section>
```

Regla de estado:

- `loading`
  - reservado para carga/refetch de datos de tabla
- `exportLoading`
  - reservado para la generacion de archivos
- `exportLoading` no debe activar skeleton, overlays ni loading veil de filas

Mock contractual de layout:

```txt
Desktop
+----------------------------------------------------------------------------------+
| Buscar | Actualizar | Total/Rango | Page size | Prev | Next | Exportar [v]      |
+----------------------------------------------------------------------------------+
| Tabla / Cards                                                                   |
+----------------------------------------------------------------------------------+

Responsive
+--------------------------------------------------+
| Buscar | Actualizar | Exportar [v]               |
| Total/Rango | Page size | Prev | Next            |
+--------------------------------------------------+
| Tabla / Cards                                    |
+--------------------------------------------------+
```

Variante equivalente aceptada:

```txt
Desktop
+----------------------------------------------------------------------------------+
| Buscar | Actualizar                                                              |
| Total/Rango | Page size | Prev | Next | Exportar [v]                             |
+----------------------------------------------------------------------------------+
| Tabla / Cards                                                                   |
+----------------------------------------------------------------------------------+
```

## Archivos esperados

- `src/app/Components/UI/AppTable/AppTableExport.tsx`
- `src/app/Components/UI/AppTable/AppTableQueryWrapper.tsx` si requiere slots
- pruebas de integracion visual

## Riesgos a evitar

- meter logica de exportacion en `AppTable.tsx`
- acoplar el menu a una sola pantalla
- duplicar triggers de exportacion por modulo

## Pruebas obligatorias

- render del trigger
- render condicional de modos
- integracion con wrapper
- estados disabled/loading
- layout compartido entre paginacion y descarga
- reflow responsive sin desacoplar visualmente la descarga de la tabla
- descarga en curso mantiene tabla visible
- descarga en curso no activa skeleton de `AppTable`

## Criterios de aceptación

- exportacion se integra visualmente al ecosistema `AppTable`
- el renderer base sigue limpio
- la pieza queda reusable para cualquier modulo
- `AppDropdown` queda establecido como componente oficial del menu de descarga
- descarga y paginacion comparten el mismo entorno visual y responden bien en responsive
- el layout implementado respeta el mock contractual o una variante equivalente con la misma semantica visual
- la descarga usa un estado de carga no destructivo y semantica visual distinta al loading de datos de la tabla

## Conclusión

La integracion visual debe consolidarse aqui, no dentro de una pantalla concreta.
