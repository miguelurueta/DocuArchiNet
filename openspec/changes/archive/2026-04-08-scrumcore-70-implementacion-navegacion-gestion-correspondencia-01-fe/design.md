## Context

`SCRUMCORE-70` busca evolucionar la navegación de `GestionCorrespondencia` hacia un shell de trabajo más cercano a una experiencia tipo Gmail, sin romper la arquitectura actual del módulo ni el patrón de rutas anidadas del dashboard.

Estado actual del módulo:

- `GestionCorrespondenciaRoute` renderiza `GestionCorrespondenciaRoutePage` como vista principal y, cuando existe ruta hija, abre un `Drawer` lateral con `GestionRespuesta`.
- El listado principal ya usa `AppToolbar`, `AppTableQueryWrapper`, `AppTable`, exportación y búsqueda desacoplada con autocomplete.
- La navegación secundaria depende del routing y preserva el fondo del listado.

El problema es que el `Drawer` funciona bien como overlay contextual, pero no representa un shell persistente de navegación de trabajo. Si el ticket pide una navegación tipo Gmail, la implementación debe decidir dónde vive esa experiencia:

- en un contenedor propio del módulo
- sin embutir la lógica visual en `AppTable`
- sin romper deep-linking, historial ni la carga inicial del módulo

Además, el cambio toca varias capas:

- `routes/` para el patrón de navegación
- `pages/` para la composición visual
- `style/` para layout persistente
- pruebas de routing y de comportamiento observable

## Goals / Non-Goals

**Goals:**

- Definir un shell de navegación persistente para `GestionCorrespondencia` alineado con una experiencia tipo Gmail.
- Mantener navegación gobernada por la URL.
- Preservar la vista principal del listado mientras se navega a vistas secundarias.
- Evitar que `AppTable` asuma responsabilidades de layout o navegación.
- Mantener el módulo desacoplado de otros dashboards o shells globales.
- Permitir que el layout escale a más vistas secundarias además de `respuesta`.

**Non-Goals:**

- No rediseñar `AppTable` ni `AppTableQueryWrapper`.
- No mover lógica de datos, filtros o exportación al router.
- No reemplazar el patrón de rutas del dashboard.
- No introducir dependencias nuevas de estado global.
- No convertir este ticket en rediseño completo del detalle funcional de `GestionRespuesta`.
- No replicar Gmail de forma literal; solo tomarlo como referencia de navegación persistente.

## Decisions

### 1. La experiencia tipo Gmail vive en el shell del módulo, no en `AppTable`

Se implementará el cambio en `GestionCorrespondenciaRoute` y/o un contenedor de layout del módulo, no dentro de `AppTable`.

Alternativa considerada:

- meter la navegación persistente dentro del wrapper de tabla o sobre el propio `AppTable`

Decisión:

- rechazada. `AppTable` debe seguir siendo reusable y agnóstico al dominio. El shell de navegación pertenece al módulo `gestionCorrespondencia`.

### 2. Se mantiene navegación por routing como fuente de verdad

La navegación secundaria debe seguir gobernada por la URL.

Alternativas consideradas:

- manejar apertura/cierre solo con estado local
- usar contexto global para la vista activa

Decisión:

- mantener routing como fuente de verdad. Esto preserva deep-linking, historial del navegador y consistencia con el patrón actual `Outlet + Drawer`.

### 3. El `Drawer` actual evoluciona hacia una región persistente del shell

La referencia tipo Gmail sugiere un área principal y un panel secundario persistente o semipersistente. La decisión recomendada es reemplazar el `Drawer` por una región layout propia del módulo cuando la ruta hija esté activa, en lugar de seguir usando un overlay modal clásico.

Alternativas consideradas:

- conservar `Drawer` y solo cambiar estilos
- reemplazarlo por `Layout/Sider` fijo del módulo
- usar un panel lateral persistente renderizado por rutas

Decisión:

- preferir un panel lateral persistente renderizado condicionalmente por routing. Esto mantiene la navegación del módulo visible y reduce la sensación modal del `Drawer`.

Razonamiento:

- el `Drawer` está bien para acciones contextuales rápidas
- un shell tipo Gmail requiere continuidad visual y jerarquía de navegación más estable
- un panel persistente dentro del layout del módulo permite transiciones futuras a master-detail sin rehacer el router

### 4. `GestionCorrespondenciaRoutePage` sigue aislando carga y errores

El shell no debe absorber lógica de `loading` o `error`.

Alternativas consideradas:

- mover manejo de skeleton/error al route shell

Decisión:

- mantener `GestionCorrespondenciaRoutePage` como frontera de carga. El shell solo decide composición y navegación.

### 5. El módulo debe soportar más de una vista secundaria

Aunque hoy existe `respuesta`, el diseño debe anticipar nuevas vistas secundarias.

Decisión:

- modelar el shell para recibir `drawerContent`/`detailContent` por routing sin asumir un único caso. La implementación concreta puede seguir usando la estructura actual de `GestionCorrespondenciaRoute`, pero con nombres y layout preparados para panel secundario persistente.

## Risks / Trade-offs

- [Cambiar `Drawer` por panel persistente altera UX actual] -> Mitigación: mantener cierre por navegación a la ruta base y cubrir con pruebas de routing.
- [El shell puede invadir responsabilidades de tabla] -> Mitigación: limitar cambios a `routes/`, `pages/` y CSS de módulo.
- [Ruptura de mobile si el panel persistente ocupa demasiado espacio] -> Mitigación: usar comportamiento responsivo que permita colapso o fallback modal en pantallas reducidas.
- [Ambigüedad entre shell persistente y overlay contextual] -> Mitigación: documentar explícitamente cuándo se usa panel persistente y cuándo fallback overlay.
- [Más complejidad visual antes de tener detalle funcional real] -> Mitigación: mantener `GestionRespuesta` como placeholder desacoplado y no mezclar el ticket con lógica de negocio.

## Migration Plan

1. Ajustar `proposal.md` y `specs` para apuntar a la capability real del módulo, no a una capability artificial generada por Jira.
2. Implementar un shell de navegación del módulo que reemplace o encapsule el patrón actual `Drawer`.
3. Mantener la vista principal del listado como región estable.
4. Renderizar la vista secundaria por routing dentro de una región lateral o detalle persistente.
5. Añadir reglas responsivas para conservar usabilidad en pantallas pequeñas.
6. Actualizar pruebas de routing y navegación observable.

Rollback:

- si el shell persistente genera regresiones, se puede volver temporalmente al `Drawer` porque la fuente de verdad de navegación seguirá siendo la URL.

## Open Questions

- Confirmar si el ticket exige reemplazar totalmente el `Drawer` o si acepta un primer paso híbrido: panel persistente en desktop y `Drawer` en mobile.
- Confirmar si la referencia “Gmail” aplica solo al patrón master-detail o también a toolbar, densidad y distribución visual del contenido.
- Confirmar si `respuesta` es la única ruta secundaria del ticket o si se espera dejar preparada una navegación lateral para múltiples vistas secundarias.
