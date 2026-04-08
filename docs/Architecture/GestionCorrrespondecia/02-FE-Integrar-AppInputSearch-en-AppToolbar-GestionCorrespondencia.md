# PROMPT ARQUITECTONICO Ticket 02 FE

# Integrar AppInputSearch en AppToolbar de GestionCorrespondencia

## Rol esperado

Arquitecto de software senior frontend (React, composition patterns, componentes UI enterprise, testing).

## Objetivo

Integrar `AppInputSearch` dentro de `AppToolbar.actionContent` en:

```txt
src/modules/gestionCorrespondencia/pages/GestionCorrespondencia.tsx
```

conectandolo al estado de consulta existente de la tabla, sin duplicar el buscador de `AppTableQueryWrapper` y sin mover logica de request o de backend a la pagina.

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

## Dependencias

- Depende de `01-FE-Implementar-AppInputSearch-core.md` si `AppInputSearch` aun no expone:
  - `className`
  - `onChange?: (value: string) => void`

### Regla de compatibilidad obligatoria

Si `02-FE` se implementa antes de completar esa migracion:

- la integracion puede adaptarse temporalmente al contrato actual del componente
- no debe inventar una firma alternativa permanente
- debe mantenerse alineada con el contrato final esperado: `onChange(value: string)`

## Restricciones obligatorias

- no mover logica de request al componente de pagina
- no consumir APIs desde `AppInputSearch`
- no duplicar el buscador en toolbar y wrapper al mismo tiempo
- no romper acciones existentes del toolbar
- no modificar exportacion, seleccion, paginacion ni contratos de `AppTable`
- no introducir estilos globales
- no ejecutar busqueda manual desde la pagina fuera del flujo ya existente de `queryState`
- no alterar estilos internos de `AppInputSearch`
- no acoplar `GestionCorrespondencia.tsx` al endpoint workflow

## Precondicion obligatoria

- validar que `AppInputSearch` soporte `className`
- validar que `AppInputSearch` soporte `onChange(value)`
- si aun no lo soporta, ajustar el ticket correspondiente o resolver la compatibilidad temporal sin romper el objetivo final

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
2. Reutilizar `table.queryState.search` como unica fuente del valor.
3. Reutilizar `table.onQueryChange({ search })` como unica salida de cambios.
4. La pagina no debe disparar busqueda manual adicional; solo actualiza `queryState`.
5. Mantener `showSearch={false}` en `AppTableQueryWrapper` de forma obligatoria para evitar duplicidad.
6. Mantener controles existentes de descarga, exportacion y acciones de toolbar.
7. Agregar clase local `toolbarSearch` solo para layout, ancho, flex y separacion.
8. `toolbarSearch` no debe alterar semantica, foco, estados ni accesibilidad de `AppInputSearch`.
9. No alterar estilos internos de `AppInputSearch`.
10. No usar clases globales ni selectores de Ant Design fuera del scope del modulo.
11. No introducir hook nuevo en la pantalla si el hook existente ya expone `queryState` y `onQueryChange`.
12. Reutilizar el hook existente del modulo como unica fuente de verdad para consulta.
13. Mantener la pagina desacoplada del endpoint workflow.
14. Si el usuario limpia el valor del buscador, la pantalla debe seguir el mismo flujo unico de estado:
    - `table.onQueryChange({ search: "" })`
    - sin agregar busqueda manual adicional
    - sin introducir logica paralela de reset en la pagina

## Reglas de consistencia

- debe existir un unico buscador visible en la pantalla
- el wrapper no debe renderizar buscador cuando la pantalla use el buscador del toolbar
- la pagina solo compone UI y conecta `queryState`
- la busqueda real debe seguir dependiendo del flujo ya existente del hook / query layer
- no debe existir doble wiring entre input, pagina y query layer

## Riesgos a evitar

- dos inputs de busqueda con el mismo estado
- perdida de acciones existentes en `AppToolbar`
- acoplar `GestionCorrespondencia.tsx` al endpoint workflow
- romper paginacion al escribir
- estilos que afecten otros toolbars
- `AppInputSearch` sin label accesible
- doble wiring entre input y query layer
- integrar `onSearch` manual desde la pagina y duplicar el flujo del hook
- adaptacion temporal que se vuelva permanente y rompa la firma final del componente

## Pruebas unitarias obligatorias

- renderiza `AppInputSearch` dentro de la pantalla
- el input usa el valor de `table.queryState.search`
- al escribir llama `table.onQueryChange({ search })`
- al limpiar el valor mantiene el flujo via `queryState`
- `AppTableQueryWrapper` permanece con `showSearch={false}`
- las acciones existentes del toolbar siguen renderizando
- el buscador tiene `aria-label`
- existe exactamente un buscador visible con ese `aria-label`
- la pagina no pasa callbacks que consuman API ni endpoints al buscador

## Pruebas QT / calidad

- usuario escribe en el buscador del toolbar y la tabla recibe cambio de consulta
- no aparecen dos buscadores en la pantalla
- el layout del toolbar no rompe acciones existentes
- el buscador conserva foco durante escritura
- el buscador se adapta al ancho disponible sin desbordar
- limpiar el valor mantiene el flujo correcto mediante `queryState`
- la integracion no rompe exportacion, paginacion ni acciones existentes

## Criterios de aceptacion

- `GestionCorrespondencia` renderiza el buscador en `AppToolbar.actionContent`
- la pantalla usa un unico buscador visible
- el buscador reutiliza `queryState.search` y `onQueryChange`
- `AppTableQueryWrapper` queda con `showSearch={false}`
- no se mueve logica de conexion a backend a la pagina
- no se rompe la exportacion ni la paginacion
- el ticket queda alineado con la firma final `onChange(value)` de `01-FE`
- la adaptacion temporal, si existe, no deja deuda contractual permanente

## Instruccion final

Antes de implementar:

- validar contratos de `AppInputSearch`
- validar hook existente del modulo
- validar `AppToolbar`
- validar `AppTableQueryWrapper`
- validar como se resuelve actualmente el flujo de busqueda desde `queryState`

Luego:

- implementar sin romper capas
- mantener TypeScript estricto
- mantener desacoplamiento
- mantener la pantalla como capa de composicion

Finalmente reportar:

- decisiones de diseno
- integracion con `queryState`
- tratamiento de compatibilidad temporal del contrato
- preservacion de accesibilidad
- preservacion de acciones del toolbar
- como queda preparada la pantalla para fases siguientes
