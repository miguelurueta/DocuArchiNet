# Ticket 04 FE

## Titulo

Crear `AppIconActionButton` y `AppTableQueryWrapper`

## Objetivo

Implementar infraestructura visual reusable para tablas dinámicas estilo Gmail, basada en:

- una familia consistente de botones iconográficos (`AppIconActionButton`)
- un contenedor de composición UI (`AppTableQueryWrapper`)

Debe permitir:

- consistencia visual entre acciones de tabla, toolbar y dropdown
- desacoplar UI de módulos específicos
- integrar `AppTableQueryState` sin duplicar lógica

## Contexto existente

Ya existe:

- `AppButton` como base UI
- `AppTable` como renderer de datos
- `AppDropdown` para acciones dinámicas
- `AppTableQueryState` como modelo compartido
- action layer para payload, guard y resolvers

Problema actual:

- controles y tabla viven separados por pantalla
- refresh y acciones no comparten base visual
- triggers `icon_button` pueden divergir visualmente

## Restricciones (obligatorio)

- no crear otro grid
- no acoplar a `GestionCorrespondencia`
- no duplicar lógica de `QueryState`
- no duplicar lógica de action layer
- no introducir lógica de dominio
- no consumir APIs desde el wrapper
- no mutar props ni estado recibido
- no usar `any`

## Ubicación (obligatoria)

```txt
src/app/Components/UI/AppButton/
src/app/Components/UI/AppTable/
src/app/Components/UI/AppDropdown/
```

## Componente 1: `AppIconActionButton` (obligatorio)

### Propósito

Botón base reusable para acciones compactas tipo icon-only, consistente en toda la aplicación.

### Contrato (obligatorio)

```ts
type AppIconActionButtonProps = {
  icon: ReactNode
  loading?: boolean
  disabled?: boolean
  onClick?: () => void
  "aria-label": string
  tooltip?: string
  size?: "sm" | "md" | "lg"
}
```

### Reglas de implementación

- debe usar `AppButton` internamente
- debe operar en modo icon-only
- no debe renderizar `children`
- el icono es el contenido principal
- debe respetar estados:
  - `loading`
  - `disabled`
- debe soportar tooltip opcional
- debe mantener accesibilidad:
  - `aria-label` es obligatorio
  - `tooltip` no reemplaza `aria-label`

### Consistencia visual (obligatorio)

- debe usar tamaños compatibles con `AppButton`
- debe ser consistente con:
  - acciones de tabla
  - triggers de dropdown
  - botones de toolbar

## Integración con `AppDropdown` (obligatorio)

- `AppDropdown` debe poder aceptar `AppIconActionButton` como trigger
- no debe romper usos actuales
- debe permitir trigger personalizado sin acoplar implementación interna
- la integración debe ser no intrusiva:
  - `AppDropdown` no depende estructuralmente de `AppIconActionButton`
  - solo debe poder recibirlo como trigger compatible

## Componente 2: `AppTableQueryWrapper` (obligatorio)

### Propósito

Wrapper reusable que compone:

- controles de consulta
- tabla
- paginación

sin contener lógica de negocio ni lógica de datos.

### Contrato (obligatorio)

```ts
type AppTableQueryWrapperProps = {
  queryState: AppTableQueryState
  onQueryChange: (patch: Partial<AppTableQueryState>) => void
  onRefresh?: () => void

  total: number
  loading?: boolean

  headerActions?: ReactNode
  children: ReactNode
}
```

### Estructura visual (obligatoria)

```txt
AppTableQueryWrapper
  HeaderControls
    search
    refresh (AppIconActionButton)
    acciones adicionales

  TableContainer
    children (AppTable)

  PaginationControls
    range
    prev / next
    pageSize
```

## Integración con `QueryState` (obligatorio)

- debe consumir `AppTableQueryState`
- debe usar `onQueryChange` para emitir cambios
- no debe implementar lógica de reset
- no debe serializar state
- no debe aplicar merge complejo del state internamente

### Regla crítica

`AppTableQueryWrapper` no debe resolver por sí mismo reglas de actualización del estado.  
Solo debe emitir patches y delegar al owner del estado, que debe aplicar la lógica reusable de `updateAppTableQueryState`.

## Reglas de implementación

`AppTableQueryWrapper` no debe:

- consumir APIs
- ejecutar queries
- duplicar lógica de `QueryState`
- manejar estado complejo interno

### Comportamiento esperado

- cambios en controles llaman `onQueryChange`
- paginación usa `queryState.page` y `queryState.pageSize`
- refresh es acción externa y no modifica state
- layout debe ser consistente y reusable

### Reglas explícitas de emisión de patches

- buscar:
  - `onQueryChange({ search: value })`
- página anterior:
  - `onQueryChange({ page: queryState.page - 1 })`
- página siguiente:
  - `onQueryChange({ page: queryState.page + 1 })`
- cambio de page size:
  - `onQueryChange({ pageSize: newValue })`

El wrapper emite patches; no resuelve las reglas finales de merge/reset.

## Reglas de inmutabilidad (obligatorio)

- no mutar `queryState`
- no mutar props
- siempre trabajar con copias o valores derivados

## Riesgos a evitar

- crear botón especial solo para refresh
- duplicar estilos de acciones compactas
- crear wrappers específicos por módulo
- divergencia visual entre tabla y dropdown
- lógica de query en UI

## Pruebas (obligatorio)

Cubrir mínimo:

### `AppIconActionButton`

- render correcto
- `loading`
- `disabled`
- accesibilidad (`aria-label`)
- integración con tooltip

### `AppDropdown`

- uso de `AppIconActionButton` como trigger
- no romper comportamiento existente

### `AppTableQueryWrapper`

- render de estructura completa
- integración con `queryState`
- cambios disparan `onQueryChange`
- `onRefresh` se invoca sin modificar state
- layout no rompe `AppTable`

## Criterios de aceptación

- existe `AppIconActionButton` reusable
- existe `AppTableQueryWrapper`
- acciones, refresh y dropdown usan la misma familia visual
- no hay duplicación de estilos
- el wrapper es reusable y desacoplado
- no rompe componentes existentes
- la integración con `QueryState` es correcta
- el wrapper no invade la capa de datos ni la capa de estado reusable

## Instrucción final

Antes de implementar:

- validar `AppButton`
- validar `AppDropdown`
- validar `AppTableQueryState`
- validar convenciones de estilos

Luego:

- implementar con TypeScript estricto
- mantener separación de capas
- mantener componentes puros

Finalmente reportar:

- decisiones de diseño
- estrategia de composición UI
- integración con `QueryState`
- consistencia visual lograda
- preparación para siguientes fases como toolbar dinámica o bulk actions
