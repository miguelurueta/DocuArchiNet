# PROMPT ARQUITECTONICO Ticket 02 FE

# Integrar AppInputSearch en AppToolbar de GestionCorrespondencia

## Rol esperado

Arquitecto de software senior frontend (React, composition patterns, componentes UI enterprise, testing).

## Objetivo

Integrar `AppInputSearch` dentro de `AppToolbar.actionContent` en `src/modules/gestionCorrespondencia/pages/GestionCorrespondencia.tsx`, conectandolo al estado de consulta existente de la tabla sin duplicar el buscador de `AppTableQueryWrapper`.

## Contexto existente

- Documento tecnico:
  - `docs/Architecture/GestionCorrrespondecia/WorkflowInbox-Busqueda-Autocomplete-Architecture.md`
- Componente de busqueda:
  - `src/app/Components/UI/AppInputSearch/`
- Pantalla objetivo:
  - `src/modules/gestionCorrespondencia/pages/GestionCorrespondencia.tsx`
- Toolbar existente:
  - `src/app/Components/UI/AppToolbar`
- Wrapper existente:
  - `src/app/Components/UI/AppTable/AppTableQueryWrapper.tsx`

## Ubicacion obligatoria

```txt
src/modules/gestionCorrespondencia/pages/GestionCorrespondencia.tsx
src/modules/gestionCorrespondencia/pages/GestionCorrespondencia.module.css
```

## Restricciones obligatorias

- no mover logica de request al componente de pagina
- no consumir APIs desde `AppInputSearch`
- no duplicar el buscador en toolbar y wrapper al mismo tiempo
- no romper acciones existentes del toolbar
- no modificar exportacion, seleccion, paginacion ni contratos de `AppTable`
- no introducir estilos globales

## Contrato de integracion obligatorio

```tsx
<AppToolbar
  className={styles.toolbar}
  actionContent={
    <div className={styles.toolbarActionGroup}>
      <AppInputSearch
        className={styles.toolbarSearch}
        aria-label="Buscar tareas workflow"
        placeholder="Buscar tareas workflow"
        value={table.queryState.search}
        onChange={(value) => table.onQueryChange({ search: value })}
      />
      {/* acciones existentes */}
    </div>
  }
/>
```

Si el buscador vive en `AppToolbar`, el wrapper debe mantenerse sin buscador:

```tsx
<AppTableQueryWrapper showSearch={false} />
```

## Reglas de implementacion obligatorias

1. Ubicar `AppInputSearch` dentro de `AppToolbar.actionContent`.
2. Reutilizar `table.queryState.search` como fuente del valor.
3. Reutilizar `table.onQueryChange({ search })` como salida de cambios.
4. Mantener `showSearch={false}` en `AppTableQueryWrapper` para evitar duplicidad.
5. Mantener los controles existentes de descarga, exportacion y acciones de toolbar.
6. Agregar clase local `toolbarSearch` solo para layout y ancho.
7. No usar clases globales ni selectores de Ant Design fuera del scope del modulo.
8. No introducir hook nuevo en la pantalla si el hook existente ya expone `queryState` y `onQueryChange`.

## Riesgos a evitar

- dos inputs de busqueda con el mismo estado
- perdida de acciones existentes en `AppToolbar`
- acoplar `GestionCorrespondencia.tsx` al endpoint workflow
- romper paginacion al escribir
- estilos que afecten otros toolbars
- `AppInputSearch` sin label accesible

## Pruebas unitarias obligatorias

- renderiza `AppInputSearch` dentro de la pantalla
- el input usa el valor de `table.queryState.search`
- al escribir llama `table.onQueryChange({ search })`
- `AppTableQueryWrapper` permanece con `showSearch={false}`
- las acciones existentes del toolbar siguen renderizando
- el buscador tiene `aria-label`

## Pruebas QT / calidad

- usuario escribe en el buscador del toolbar y la tabla recibe cambio de consulta
- no aparecen dos buscadores en la pantalla
- el layout del toolbar no rompe acciones existentes
- el buscador conserva foco durante escritura
- el buscador se adapta al ancho disponible sin desbordar

## Criterios de aceptacion

- `GestionCorrespondencia` renderiza el buscador en `AppToolbar.actionContent`
- la pantalla usa un unico buscador visible
- el buscador reutiliza `queryState.search` y `onQueryChange`
- no se mueve logica de conexion a backend a la pagina
- no se rompe la exportacion ni la paginacion

