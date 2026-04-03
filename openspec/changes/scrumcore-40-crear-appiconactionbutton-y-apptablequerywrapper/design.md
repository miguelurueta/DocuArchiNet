## Context

La siguiente fase de la arquitectura de `AppTable` necesita una capa visual reusable que acople controles y tabla sin invadir el query layer ni la action layer. Después de `SCRUMCORE-39`, el proyecto ya cuenta con un `AppTableQueryState` compartido; ahora falta una composición UI consistente para búsqueda, refresh, paginación externa y acciones iconográficas.

En el frontend actual, `AppButton`, `AppDropdown` y `AppTable` existen como piezas separadas. Eso deja a cada pantalla la responsabilidad de inventar sus propios botones icon-only, su propia barra de consulta y sus propios contenedores visuales. El resultado es riesgo de divergencia visual, wiring duplicado y una integración frágil entre toolbar, acciones de fila y dropdowns dinámicos.

Este cambio debe introducir dos piezas reutilizables y desacopladas:

- `AppIconActionButton` como base oficial de acciones iconográficas compactas
- `AppTableQueryWrapper` como contenedor de composición UI para controles + tabla + paginación externa

Ambas deben quedar alineadas con la arquitectura maestra de `AppTableArchitecture`, sin consumir APIs, sin serializar requests y sin reimplementar la lógica de `AppTableQueryState`.

## Goals / Non-Goals

**Goals:**
- Crear `AppIconActionButton` reutilizando `AppButton` como base de sistema UI.
- Crear `AppTableQueryWrapper` para componer búsqueda, refresh, acciones adicionales, tabla y paginación externa.
- Permitir que `AppDropdown` use un trigger basado en la misma familia visual sin romper compatibilidad actual.
- Mantener desacoplamiento entre composición UI y lógica de datos o query state.
- Dejar una base visual reutilizable para las siguientes fases de paginación híbrida y migración de `GestionCorrespondencia`.

**Non-Goals:**
- No introducir fetch ni lógica backend en el wrapper.
- No diseñar aún filtros avanzados de dominio ni bulk actions finales.
- No convertir `AppTable` en un grid nuevo ni en un layout rígido obligatorio.
- No duplicar la lógica de reset, merge o serialización del query state.
- No obligar a `AppDropdown` a depender estructuralmente de `AppIconActionButton`.

## Decisions

### 1. `AppIconActionButton` será la base oficial de acciones iconográficas compactas

El proyecto necesita una sola pieza base para acciones compactas icon-only. Esa pieza reutilizará `AppButton` internamente y fijará el contrato visual y funcional para refresh, acciones de celda y triggers iconográficos de dropdown.

La decisión evita que cada módulo o renderer vuelva a crear botones ad hoc con estados o tamaños inconsistentes.

**Alternativas consideradas**
- Crear solo `AppRefreshButton`: se descarta porque no cubre acciones de celda ni dropdowns.
- Seguir usando directamente `AppButton` en cada integración: se descarta porque deja repetición de variantes y divergencia visual.

### 2. `AppTableQueryWrapper` será un contenedor de composición UI, no un contenedor de datos

`AppTableQueryWrapper` debe renderizar un bloque visual único con:

- controles de búsqueda
- refresh
- acciones adicionales de header
- tabla renderizada como `children`
- navegación prev/next y page size

El wrapper no debe ejecutar queries ni aplicar reglas de reset. Solo emite patches hacia el owner del `queryState` y dispara `onRefresh` cuando exista.

**Alternativas consideradas**
- Meter la barra Gmail dentro de `AppTable`: se descarta porque acopla demasiado el renderer base.
- Hacer un wrapper por pantalla: se descarta porque rompe reutilización.

### 3. La integración con `AppDropdown` será no intrusiva

`AppDropdown` podrá recibir `AppIconActionButton` como trigger compatible, pero no debe reescribirse alrededor de ese componente ni perder triggers actuales. La relación correcta es de interoperabilidad visual, no de dependencia estructural.

**Alternativas consideradas**
- Requerir `AppIconActionButton` como trigger único de dropdown: se descarta porque rompería usos existentes.
- Ignorar `AppDropdown` en esta fase: se descarta porque dejaría inconsistencia visual en acciones `icon_button`.

### 4. El wrapper emite patches simples y delega al owner del estado

El contrato `onQueryChange(patch)` se mantiene intencionalmente pequeño. `AppTableQueryWrapper` no debe hacer merge ni reset de `AppTableQueryState`; esas reglas ya viven en la capa reusable creada en `SCRUMCORE-39`.

La decisión mantiene la separación entre:

- UI composition
- estado reusable
- query/fetch

**Alternativas consideradas**
- Dejar que el wrapper administre su propio estado complejo: se descarta porque duplicaría la capa de `AppTableQueryState`.
- Hacer que el wrapper reciba callbacks separados por control (`onSearchChange`, `onNext`, etc.): se descarta porque fragmenta la API.

## Risks / Trade-offs

- [Risk] El wrapper termine creciendo demasiado y mezclando layout con estado.  
  Mitigation: limitarlo a composición UI y emisión de patches, sin fetch ni merge interno.

- [Risk] `AppIconActionButton` no quede visualmente alineado con `AppButton` o con los triggers actuales de dropdown.  
  Mitigation: reutilizar `AppButton` internamente y validar integración con `AppDropdown`.

- [Risk] Se dupliquen estilos entre refresh, acciones de celda y toolbar.  
  Mitigation: usar `AppIconActionButton` como base única y dejar especializaciones como wrappers ligeros.

- [Risk] `AppTableQueryWrapper` termine acoplado a `GestionCorrespondencia`.  
  Mitigation: no introducir props ni naming de dominio y aceptar `children` + `headerActions` genéricos.

## Migration Plan

1. Revisar `AppButton`, `AppDropdown` y `AppTable` para ubicar la mejor integración sin romper consumidores actuales.
2. Implementar `AppIconActionButton` y sus pruebas de accesibilidad, loading y disabled.
3. Extender la integración de `AppDropdown` para aceptar ese trigger sin romper comportamiento existente.
4. Implementar `AppTableQueryWrapper` con el contrato basado en `AppTableQueryState`.
5. Cubrir la composición visual con pruebas antes de que las fases de paginación híbrida y migración de módulo lo consuman.

Rollback: si aparece regresión, se puede retirar el wrapper nuevo y el botón iconográfico sin tocar aún la infraestructura base de `AppTableQueryState`.

## Open Questions

- Si conviene extraer inmediatamente un `AppRefreshButton` ligero encima de `AppIconActionButton` o dejarlo para la fase de integración en módulo.
- Si `AppTableQueryWrapper` debe renderizar el rango visible internamente desde `queryState` y `total` o aceptar algún override futuro para formatos más especializados.
