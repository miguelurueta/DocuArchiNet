# SCRUMCORE-295 - UI Y Componentes

## Composicion

`DocumentosWorkbench` renderiza:

1. Visor documental.
2. Rail/listado de documentos.
3. `AppTableQueryWrapper` como contenedor de busqueda.
4. `AppTreeTable` como renderer del listado.

## `AppTableQueryWrapper`

Cambio agregado:

```ts
showPagination?: boolean;
```

Reglas:

- Default: `true`.
- Otros consumidores conservan paginacion sin cambios.
- `DocumentosWorkbench` pasa `showPagination={false}`.
- Si `showPagination=false`, no se renderizan:
  - rango
  - selector page size
  - pagina anterior
  - pagina siguiente
  - `paginationActions`

## `DocumentosWorkbench`

Uso implementado:

```tsx
<AppTableQueryWrapper
  queryState={documentosTable.queryState}
  onQueryChange={documentosTable.onQueryChange}
  total={documentosTable.totalDocumentsCount}
  loading={documentosTable.loading || isReplacingAnnotatedPages}
  showSearch
  showPagination={false}
  searchPlaceholder="Buscar documento"
  className={styles.listQueryWrapper}
>
  <AppTreeTable ... />
</AppTableQueryWrapper>
```

## `AppTreeTable`

No se agregaron reglas de negocio al tree.

Sigue recibiendo:

- `load`
- `loadChildren`
- `tableColumns`
- `columns`
- callbacks de seleccion y acciones
- `tableLayoutMode="fill"`

No conoce:

- `DocumentRelationScope`
- `Radicado`
- `NombreGabinete`
- `EnablePagination`
- `Search`
- anexos de respuesta
- offsets o limits

## CSS Scoped

El layout del listado se ajusto solo en `DocumentosWorkbench.module.css`:

- wrapper full height
- grid con header de busqueda y tabla
- paginacion no visible porque no se renderiza
- input compacto

Reglas relevantes:

```css
.listQueryWrapper {
  display: grid;
  grid-template-rows: auto minmax(0, 1fr);
  padding: 0.35rem;
  gap: 0.35rem;
  border-radius: 8px;
}

.listQueryWrapper :global(.ant-input-affix-wrapper) {
  min-height: 2rem;
  padding-block: 0.15rem;
  border-radius: 8px;
}
```

## Estados UI

| Estado | Condicion | Comportamiento |
|---|---|---|
| `loading` | Request activo | Wrapper recibe `loading=true`. |
| `success` | Filas visibles > 0 | Renderiza `AppTreeTable`. |
| `empty` | Filas visibles = 0 | `AppTreeTable` muestra `emptyMessage`. |
| `validation` | Backend valida contra request | Se muestra mensaje funcional, sin fallback silencioso. |
| `error` | Error tecnico/red | Mensaje generico de carga. |

## Delete Restringido

Cuando backend responde restriccion funcional para `eliminar_item`:

- se usa `toast.warning`
- no se deja alerta inline persistente
- se preserva estado visual del workbench
