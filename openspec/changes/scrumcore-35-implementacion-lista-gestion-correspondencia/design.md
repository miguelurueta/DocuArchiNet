## Context

`SCRUMCORE-35` no crea nueva infraestructura de tabla dinámica. Continúa sobre fases ya implementadas en `AppTable`:

- Fase 1B: contratos y adapters dinámicos
- Fase 2: query layer dinámica
- Fase 3: action layer dinámica

El objetivo real ahora es integrar la bandeja `workflowInboxgestion` dentro del módulo `gestionCorrespondencia` usando el endpoint ya validado `POST /api/workflowInboxgestion/inboxgestion`, pero manteniendo `AppTable` como renderer visual final.

El módulo `gestionCorrespondencia` hoy conserva una estructura deliberadamente simple:

```text
layout/
pages/
routes/
```

y un patrón `Outlet + Drawer` que no debe romperse. La página principal [GestionCorrespondencia.tsx](D:/imagenesda/GestorDocumental/DocuArchiCore.react/src/modules/gestionCorrespondencia/pages/GestionCorrespondencia.tsx) todavía usa datos mock y renderiza [AppTable.tsx](D:/imagenesda/GestorDocumental/DocuArchiCore.react/src/app/Components/UI/AppTable/AppTable.tsx) con columnas `ColDef[]` y filas planas.

La infraestructura dinámica ya existente no entrega directamente ese contrato visual final. `useDynamicUiTableQuery` hoy termina en:

- `AppGridColumn[]`
- `AppGridRow[]`

pero `AppTable` espera:

- `columns: ColDef<T>[]`
- `rows: T[]`

Eso obliga a cerrar el gap con una capa adicional de adaptación, sin duplicar la lógica ya creada ni crear un grid alterno.

## Goals / Non-Goals

**Goals**

- Consumir `POST /api/workflowInboxgestion/inboxgestion` en la carga inicial del módulo `gestionCorrespondencia`.
- Reutilizar `AppTable` como componente visual final.
- Reemplazar los mocks actuales de la pantalla por datos reales.
- Agregar un wrapper de ruta para manejar `loading`, `error` y `success` sin contaminar la página principal.
- Reutilizar el patrón de `Skeleton` de la aplicación para la primera carga.
- Crear adapters reutilizables desde `AppGridColumn[]` / `AppGridRow[]` hacia el contrato visual de `AppTable`.
- Mantener intacto el patrón `Outlet + Drawer`.

**Non-Goals**

- No crear una nueva tabla específica para `gestionCorrespondencia`.
- No renderizar AG Grid directamente desde el módulo.
- No mover infraestructura dinámica fuera de `src/app/Components/UI/AppTable/`.
- No acoplar `AppTable` al endpoint `workflowInboxgestion`.
- No resolver aquí toda la integración visual final de `CellActions`, `RowActions`, toolbar actions o bulk actions, salvo que se limite explícitamente a preservación de metadata.

## Decisions

### 1. `AppTable` sigue siendo el renderer final

La pantalla debe seguir renderizando [AppTable.tsx](D:/imagenesda/GestorDocumental/DocuArchiCore.react/src/app/Components/UI/AppTable/AppTable.tsx). No se debe crear un grid paralelo en `gestionCorrespondencia` ni consumir `AgGridReact` directamente desde el módulo.

Esto protege el esquema actual del frontend y mantiene un único componente base de render.

### 2. Los adapters finales entre modelo dinámico y modelo visual viven en `AppTable`

El gap entre:

- `AppGridColumn[]` / `AppGridRow[]`

y

- `ColDef<T>[]` / `rows: T[]`

no es un concern de dominio del módulo. Es un concern genérico del propio `AppTable`.

Por esa razón, los nuevos adapters deben ubicarse en:

```text
src/app/Components/UI/AppTable/adapters/
  appGridToAppTableColumns.ts
  appGridToAppTableRows.ts
```

Esto evita acoplar `gestionCorrespondencia` a shapes internos de `AppTable` y deja esa traducción disponible para futuras pantallas.

### 3. El módulo encapsula request mapping y wiring de pantalla

El módulo `gestionCorrespondencia` sí debe ser dueño de:

- cómo se construye el request del endpoint
- cómo se orquesta la carga de pantalla
- cómo se presenta `loading`, `error`, `empty` y `success`

Por eso se espera agregar:

```text
src/modules/gestionCorrespondencia/
  adapters/
    gestionCorrespondenciaTableRequestMapper.ts
  hooks/
    useGestionCorrespondenciaTable.ts
  pages/
    GestionCorrespondenciaRoutePage.tsx
  components/
    GestionCorrespondenciaTableSkeleton.tsx
```

La página principal [GestionCorrespondencia.tsx](D:/imagenesda/GestorDocumental/DocuArchiCore.react/src/modules/gestionCorrespondencia/pages/GestionCorrespondencia.tsx) debe permanecer enfocada en composición visual y props ya resueltas.

### 4. La carga inicial debe resolverse en un wrapper de ruta

La primera carga no debe depender solo del overlay de AG Grid. La app ya usa `Skeleton` de `@mui/material` en páginas de carga como [RadicacionRoutePage.tsx](D:/imagenesda/GestorDocumental/DocuArchiCore.react/src/modules/radicacion/pages/RadicacionRoutePage.tsx), y ese patrón debe reutilizarse.

Por tanto, la integración debe seguir esta secuencia:

```text
GestionCorrespondenciaRoute
  -> GestionCorrespondenciaRoutePage
      -> loading inicial => Skeleton de pantalla
      -> error => estado de error
      -> success => GestionCorrespondencia(props)
```

Esto evita mezclar concerns de fetch con el patrón `Drawer`.

### 5. `GestionCorrespondenciaRoute` debe preservar `Outlet + Drawer`

El `Drawer` contextual existente en [GestionCorrespondenciaRoute.tsx](D:/imagenesda/GestorDocumental/DocuArchiCore.react/src/modules/gestionCorrespondencia/routes/GestionCorrespondenciaRoute.tsx) no debe cambiar de responsabilidad.

La integración de datos debe incorporarse de forma que:

- la pantalla principal siga visible en la ruta base
- el drawer siga abriéndose por subruta
- cerrar el drawer siga regresando a la ruta base

La nueva `RoutePage` debe convivir con ese comportamiento, no reemplazarlo.

### 6. El `Skeleton` vive en el módulo, no en `AppTable`

El `Skeleton` de primera carga es concern de pantalla, no del componente base.

El comportamiento esperado es:

- primera carga: `GestionCorrespondenciaTableSkeleton`
- recargas/refetch: `AppTable loading={...}`

Eso permite una UX alineada con la aplicación sin sobrecargar `AppTable` con estructura específica de una página.

### 7. La metadata dinámica de acciones se preserva, pero la renderización visual completa no se promete en esta fase

La respuesta backend ya trae `CellActions`, y la infraestructura dinámica actual ya puede modelar esa metadata. Sin embargo, esta fase de integración de pantalla no debe prometer automáticamente el render funcional completo de:

- `icon_button`
- menús dinámicos
- toolbar actions
- bulk actions

si esa capa visual no existe todavía en `AppTable`.

La fase sí debe dejar preparada la pantalla para no perder metadata relevante y para soportar una siguiente fase visual.

## Risks / Trade-offs

- [Se cree una tabla paralela en el módulo] -> Mitigación: dejar explícito que `AppTable` es el renderer final obligatorio.
- [Se mezcle el adapter final dentro del módulo] -> Mitigación: ubicar `AppGrid -> AppTable` dentro de `AppTable/adapters`.
- [La primera carga quede resuelta solo con overlay de grid] -> Mitigación: usar `RoutePage` + `Skeleton`.
- [El módulo pierda su patrón `Outlet + Drawer`] -> Mitigación: mantener `GestionCorrespondenciaRoute` como orquestador del drawer y mover la carga a un wrapper compatible.
- [El ticket prometa actions visuales completas] -> Mitigación: dejar ese alcance fuera salvo soporte explícito adicional.

## Migration Plan

1. Crear el request mapper del módulo para `workflowInboxgestion`.
2. Crear un hook del módulo que componga:
   - query dinámica existente
   - request mapper del módulo
   - adapters finales hacia `AppTable`
3. Crear adapters genéricos en `AppTable` para:
   - `AppGridColumn[] -> ColDef<T>[]`
   - `AppGridRow[] -> T[]`
4. Crear `GestionCorrespondenciaRoutePage.tsx` para manejar:
   - loading inicial
   - error
   - success
5. Crear `GestionCorrespondenciaTableSkeleton.tsx` usando `@mui/material/Skeleton`.
6. Refactorizar [GestionCorrespondencia.tsx](D:/imagenesda/GestorDocumental/DocuArchiCore.react/src/modules/gestionCorrespondencia/pages/GestionCorrespondencia.tsx) para recibir props y dejar de usar mocks.
7. Ajustar tests del módulo y agregar cobertura de adapters/hook/ruta.

Rollback: como esta fase agrega wiring y adapters sin migraciones persistentes, el rollback consiste en revertir los nuevos archivos del módulo y los adapters finales de `AppTable`, restaurando la página actual con mocks.

## Open Questions

- Si la navegación de página completa entra en esta fase o si se limita a sincronizar `pageSize` y `total`.
- Cómo se debe tratar visualmente la columna `acciones` mientras no exista integración visual final de actions dinámicas en `AppTable`.
- Si el filtro `Categoria` debe quedar activo sobre query backend en esta fase o seguir como placeholder hasta que exista soporte real en el request del endpoint.
