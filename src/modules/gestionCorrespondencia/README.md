# GestionCorrespondencia

Modulo base para la gestion de correspondencia dentro del dashboard de `DocuArchiCore.react`.

## Proposito

Definir una estructura inicial desacoplada y escalable para futuras bandejas, detalle documental y flujos de respuesta, sin introducir aun logica de negocio ni integracion real con backend.

## Estructura

```text
src/modules/gestionCorrespondencia/
  adapters/
    gestionCorrespondenciaTableRequestMapper.ts
  components/
    GestionCorrespondenciaTableSkeleton.tsx
  hooks/
    useGestionCorrespondenciaTable.ts
    useWorkflowInboxAutocomplete.ts
  layout/
    GestionCorrespondenciaLayout.tsx
  pages/
    GestionCorrespondencia.tsx
    GestionCorrespondenciaRoutePage.tsx
    GestionRespuesta.tsx
  routes/
    GestionCorrespondenciaRoute.tsx
  services/
    workflowInboxAutocomplete.service.ts
  types/
    workflowInboxAutocomplete.types.ts
  README.md
```

## Responsabilidad por capa

- `layout/`: shell visual del modulo. Define header, descripcion, contenedor principal y `Outlet`.
- `adapters/`: mapeo del input del modulo al request real del backend.
- `components/`: piezas visuales auxiliares del modulo como skeletons de pantalla.
- `hooks/`: orquestacion de la carga de datos del modulo usando la infraestructura compartida de `AppTable` y features desacopladas como autocomplete.
- `pages/`: composicion de UI y pantallas visibles de la vista principal y la vista secundaria.
- `routes/`: orquestacion del shell persistente del modulo controlado por la URL.
- `services/`: acceso HTTP del dominio del modulo sin acoplar componentes UI a endpoints.
- `types/`: contratos tipados del modulo para requests, responses e items auxiliares.

## Shell persistente gobernado por routing

- `GestionCorrespondenciaLayout` renderiza el `Outlet` del modulo.
- `GestionCorrespondenciaRoute` mantiene visible la pagina principal y superpone un panel secundario cuando la ruta hija `respuesta` esta activa.
- `GestionRespuesta` se renderiza dentro del panel secundario como vista contextual desacoplada.
- `GestionCorrespondenciaRoutePage` envuelve la carga inicial de datos, el estado de error y la pantalla principal sin romper el shell.
- la accion dominante de retorno vive en el shell del modulo y devuelve al listado sin acoplar `GestionRespuesta` al router.

Este patron permite deep-linking, navegacion con historial y preserva el contexto de la pantalla principal mientras la vista secundaria vive en el mismo shell.

## Flujo de navegacion

1. El usuario entra a `/dashboard/gestion-correspondencia`.
2. Se renderiza la pagina principal `GestionCorrespondencia`.
3. El usuario navega a `/dashboard/gestion-correspondencia/respuesta`.
4. Se superpone el panel secundario del shell y se muestra `GestionRespuesta` encima del listado, sin desmontarlo.
5. El usuario usa la accion visible `Volver a la bandeja`.
6. La aplicacion vuelve a la ruta base del modulo sin romper el contexto principal.

## Como escalar el modulo

- Agregar nuevas rutas hijas dentro del adapter de rutas para futuros paneles contextuales o vistas secundarias persistentes.
- Incorporar hooks, services y modelos solo cuando entren funcionalidades reales del dominio.
- Mantener las reglas de negocio fuera del layout y de las paginas placeholder.

## Integracion actual de bandeja

La pantalla principal ya consume la bandeja dinamica `workflowInboxgestion` usando la infraestructura compartida de `AppTable`:

- query layer dinamica
- adapters del modelo intermedio `AppGrid -> AppTable`
- `Skeleton` de primera carga a nivel de ruta

El renderer visual final sigue siendo `AppTable`.

## Busqueda y autocomplete

La pantalla principal mantiene dos flujos separados:

- busqueda real de tabla:
  - Enter
  - click en el icono de busqueda
  - seleccion de sugerencia
- autocomplete de sugerencias:
  - escritura del usuario
  - `minLength`
  - debounce
  - request limitado al endpoint dedicado

La integracion actual sigue estas reglas:

- `GestionCorrespondencia` renderiza `AppInputSearch` en el toolbar.
- `AppTableQueryWrapper` conserva `showSearch={false}`.
- `useWorkflowInboxAutocomplete` maneja debounce, loading, error y respuestas obsoletas.
- `workflowInboxAutocomplete.service.ts` encapsula el endpoint `/api/workflowInboxgestion/inboxgestion/autocomplete`.
- `AppInputSearch` sigue siendo presentacional y no conoce endpoints.
- `table.onQueryChange({ search })` sigue siendo el unico puente para aplicar filtro real a la tabla.

## Limites actuales

- la columna `acciones` se preserva en la metadata dinamica, pero su render visual final dentro de `AppTable` sigue siendo basico hasta una fase visual posterior
- la pantalla sincroniza total y `pageSize`, pero la navegacion completa entre paginas puede requerir una iteracion adicional segun backlog
- el panel secundario actual usa una composicion persistente simple y queda preparado para futuras vistas secundarias, no como detalle funcional final
- la accion de retorno es unica y visible, pero el detalle sigue siendo placeholder hasta fases funcionales posteriores
