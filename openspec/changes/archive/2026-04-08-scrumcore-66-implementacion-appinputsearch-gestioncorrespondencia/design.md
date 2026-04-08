## Context

`SCRUMCORE-66` implementa el ticket frontend para `AppInputSearch` dentro del flujo de busqueda de `GestionCorrespondencia`.

El estado actual del componente compartido es una primera version simple: `src/app/Components/UI/AppInputSearch/AppInputSearch.tsx` compone `AppInput`, agrega `SearchOutlined` como icono decorativo y hereda el contrato de `AppInput`, incluyendo `onChange` como `ChangeEventHandler<HTMLInputElement>`. El ticket requiere evolucionarlo a un control de busqueda reusable con contrato por valor, `AutoComplete` + `Input`, soporte de `onSearch`, clear, loading, `minLength`, debounce y tamanos `sm | md | lg`.

La pantalla `src/modules/gestionCorrespondencia/pages/GestionCorrespondencia.tsx` ya renderiza `AppTableQueryWrapper` con `showSearch={false}`. Esto evita duplicar buscadores y deja el espacio correcto para ubicar el buscador de `GestionCorrespondencia` dentro de `AppToolbar.actionContent`.

El flujo de consulta de tabla ya esta centralizado en `table.queryState` y `table.onQueryChange`. La pantalla no debe conocer endpoints ni mover logica de request. Para que la busqueda simple active el `LIKE` backend existente, el mapper de `GestionCorrespondencia` debe asegurar `SearchType = 2` cuando exista texto efectivo sin romper `SearchType = 3` de busqueda avanzada.

## Goals / Non-Goals

**Goals:**

- Evolucionar `AppInputSearch` hacia el contrato reusable definido por el ticket 01 FE.
- Mantener `AppInputSearch` presentacional, sin consumo de APIs ni conocimiento de modulos.
- Integrar `AppInputSearch` en `AppToolbar.actionContent` de `GestionCorrespondencia`.
- Mantener un unico buscador visible en la pantalla y conservar `AppTableQueryWrapper showSearch={false}`.
- Conectar cambios de texto al flujo existente de `queryState` / `onQueryChange`.
- Centralizar la decision `SearchType = 2` en el mapper de `GestionCorrespondencia`, no en UI.
- Cubrir el comportamiento con pruebas unitarias focales de componente, pantalla y mapper.

**Non-Goals:**

- Implementar autocomplete conectado a backend. Eso queda para el ticket frontend de autocomplete.
- Cambiar el endpoint de listado workflow o modificar claims/autorizacion.
- Modificar contratos globales de `AppTable` o el mapper shared si el comportamiento solo aplica a `GestionCorrespondencia`.
- Mover logica de request a `GestionCorrespondencia.tsx`.
- Cambiar exportacion, seleccion, paginacion o acciones existentes del toolbar.

## Decisions

1. `AppInputSearch` sera el componente dueño de la semantica de busqueda UI.

   Se evoluciona desde wrapper simple sobre `AppInput` hacia composicion `AutoComplete` + `Input` de Ant Design para soportar sugerencias, seleccion por teclado, clear, icono de busqueda interactivo y loading. No se usara `Input.Search` porque el contrato local necesita controlar eventos, estilos y accesibilidad de forma consistente con `AppInput`.

   Alternativa considerada: mantenerlo como wrapper de `AppInput` y resolver eventos en consumidores. Se descarta porque duplicaria debounce, Enter, clear y semantica de `onSearch` entre pantallas.

2. El contrato publico migrara de `onChange(event)` a `onChange(value)`.

   La pantalla de `GestionCorrespondencia` debe consumir `onChange={(value) => table.onQueryChange({ search: value })}`. Esto evita que consumidores dependan de detalles del DOM y alinea el componente con un contrato controlado por valor.

   Alternativa considerada: conservar `ChangeEventHandler` para compatibilidad inmediata. Se descarta como objetivo final porque contradice el prompt arquitectonico. Si aparece una adaptacion temporal, no debe quedar como deuda contractual permanente.

3. El buscador de `GestionCorrespondencia` vive en `AppToolbar.actionContent`.

   `GestionCorrespondencia.tsx` ya usa `showSearch={false}` en `AppTableQueryWrapper`. Se mantiene esa regla y se agrega `AppInputSearch` al grupo de acciones del toolbar, junto con las acciones existentes. La clase local `toolbarSearch` solo debe controlar layout, ancho, flex y separacion.

   Alternativa considerada: habilitar `showSearch` dentro de `AppTableQueryWrapper`. Se descarta porque el requerimiento pide integrarlo en el toolbar y generaria dos lugares posibles para el mismo control.

4. La pantalla solo actualiza `queryState`.

   `GestionCorrespondencia.tsx` no llamara servicios, endpoints ni mappers. La busqueda real seguira fluyendo por el hook de tabla existente a partir de `table.onQueryChange({ search })`.

   Alternativa considerada: disparar una busqueda manual desde `onSearch` en la pagina. Se descarta porque crea doble wiring y acopla la composicion UI a la capa de datos.

5. `SearchType = 2` se resuelve en el mapper de `GestionCorrespondencia`.

   `src/modules/gestionCorrespondencia/adapters/gestionCorrespondenciaTableRequestMapper.ts` debe envolver `mapDynamicUiServerTableRequest` y ajustar `SearchType` solo para este modulo: conservar `3` si la busqueda avanzada lo define explicitamente; usar `2` si `search.trim().length > 0`; no forzar `2` cuando no haya texto efectivo.

   Alternativa considerada: modificar el mapper shared de `AppTable`. Se descarta porque cambiaria el comportamiento de otras tablas sin que el ticket lo pida.

## Risks / Trade-offs

- [Riesgo] Cambiar `onChange` de evento a valor rompe consumidores existentes.
  Mitigacion: buscar todos los usos de `AppInputSearch`, migrarlos en el mismo cambio y cubrir `AppTableQueryWrapper` / `GestionCorrespondencia` con pruebas.

- [Riesgo] Doble ejecucion de `onSearch` por debounce pendiente y Enter/click.
  Mitigacion: cancelar o neutralizar timers pendientes antes de ejecutar busquedas manuales y cubrirlo con fake timers en Vitest.

- [Riesgo] Dos buscadores visibles comparten el mismo estado.
  Mitigacion: mantener `showSearch={false}` en `AppTableQueryWrapper` y agregar prueba de un unico textbox con `aria-label="Buscar tareas workflow"`.

- [Riesgo] `SearchType = 2` se filtra a otras tablas.
  Mitigacion: aplicar la normalizacion solo en `mapGestionCorrespondenciaTableRequest`, envolviendo el mapper shared.

- [Riesgo] Loading bloquea escritura o pierde foco.
  Mitigacion: usar indicador visual de loading sin deshabilitar el input; `disabled` conserva prioridad cuando venga explicitamente.

- [Riesgo] Estilos locales del toolbar alteran semantica o estados del input.
  Mitigacion: limitar `toolbarSearch` a layout; no usar selectores globales ni selectores de Ant Design fuera del CSS module de la pantalla.
