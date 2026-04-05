## Context

`AppTableExport` ya existe como pieza reusable para exportacion local y actualmente construye un menu basado en `AppDropdown`, filtrando formatos y modos segun las capacidades reales del datasource. En paralelo, `AppTableQueryWrapper` ya concentra busqueda, refresh, rango visible, page size y navegacion de pagina, con un slot `paginationActions` que permite montar acciones operativas junto a la paginacion.

El riesgo de este cambio no es funcional sino de consolidacion arquitectonica: si cada pantalla vuelve a ubicar exportacion en toolbars o wrappers ad hoc, la experiencia de tabla se fragmenta y el renderer base termina absorbiendo responsabilidades que no le corresponden. El ticket busca cerrar esa decision y declarar un patron reusable para todas las tablas del ecosistema `AppTable`.

El prompt arquitectonico del ticket 18 tambien impone dos restricciones fuertes: `AppTable.tsx` debe permanecer libre de logica de exportacion y la experiencia de descarga debe convivir visualmente con los controles de consulta y paginacion sin disparar estados destructivos sobre la tabla visible.

## Goals / Non-Goals

**Goals:**
- Consolidar `AppDropdown` como patron visual oficial para las opciones de exportacion de `AppTableExport`.
- Formalizar que la banda `paginationActions` de `AppTableQueryWrapper` es el punto de integracion preferido para exportacion y futuras acciones operativas de tabla.
- Mantener la separacion entre estado de carga de datos de tabla (`loading`) y estado de descarga (`exportLoading`) para evitar overlays o skeletons durante la exportacion.
- Dejar una composicion reusable que permita adoptar `AppTableExport` en otros modulos sin wiring especifico por pantalla.

**Non-Goals:**
- Conectar en este cambio la estrategia backend de `allMatching`; ese flujo pertenece al ticket posterior de datasource server-side.
- Acoplar la solucion a `GestionCorrespondencia` como caso especial; la pantalla actual solo valida el patron reusable.
- Redefinir `AppDropdown`, `AppTable` o el modelo de query state fuera de lo necesario para la integracion visual.

## Decisions

### 1. La exportacion vive fuera de `AppTable` y se integra por composicion en `AppTableQueryWrapper`

Se mantiene `AppTable` como renderer puro de datos y se usa `AppTableQueryWrapper` como contenedor de experiencia operativa. La razon es que la exportacion depende del contexto de consulta, seleccion y datasource, no del renderer de filas. Llevar la logica a `AppTable` introduciria acoplamiento con concerns de toolbar, descarga y permisos de accion.

Alternativa considerada: instanciar `AppTableExport` dentro de `AppTable`. Se descarta porque forzaria props y estados de exportacion en todas las tablas, incluso donde la funcionalidad no aplica.

### 2. `paginationActions` se declara como el slot canonico para exportacion

La banda de controles inferiores ya expone rango, page size y navegacion, por lo que `paginationActions` es el punto correcto para montar acciones que pertenecen al flujo operativo de la tabla. Esta decision alinea exportacion con paginacion en desktop y responsive sin reciclar `headerActions` para algo que no es una accion global de cabecera.

Alternativa considerada: montar exportacion en `headerActions`. Se descarta porque visualmente separa una accion operativa de tabla de los controles con los que comparte contexto y favorece layouts inconsistentes entre pantallas.

### 3. `AppDropdown` solo expresa menu y trigger; `AppTableExport` conserva la logica de capacidades y descarga

`AppTableExport` calcula formatos soportados, modos disponibles y estados disabled a partir del datasource y delega el render del menu a `AppDropdown`. Asi el dropdown sigue siendo una pieza visual reusable y la inteligencia de exportacion permanece encapsulada en el componente del dominio `AppTable`.

Alternativa considerada: mover parte de la logica de habilitacion a `AppDropdown`. Se descarta porque el dropdown no conoce semantica de exportacion y terminaria contaminado con reglas de negocio ajenas.

### 4. El estado de exportacion es no destructivo y separado del `loading` de tabla

Durante una descarga se bloquean solo las acciones del trigger y del menu correspondiente mediante `exportLoading`. La tabla visible y sus datos actuales permanecen montados. Esta decision evita mezclar una operacion de archivo local con un refetch de datos, que son dos experiencias distintas para el usuario.

Alternativa considerada: reutilizar `loading` del wrapper o de la tabla para exportacion. Se descarta porque generaria skeletons, overlays o deshabilitaciones globales no justificadas por una descarga local.

### 5. El contrato actual prioriza exportacion local y deja el backend extensible

El componente limita los modos ejecutables a `currentPage`, `selectedRows` y `allLoaded`, aunque el contrato general ya contempla capacidades mas amplias como `allMatching`. La decision es mantener el contrato abierto pero restringir la ejecucion a lo que el datasource local puede resolver hoy, evitando prometer una opcion que aun depende de backend.

Alternativa considerada: ocultar completamente modos futuros del contrato. Se descarta porque el modelo reusable ya necesita contemplar la evolucion a server-side; lo correcto es diferenciar contrato de implementacion activa.

## Risks / Trade-offs

- [Riesgo] `headerActions` siga usandose para exportacion en pantallas nuevas y reaparezca la fragmentacion visual. → Mitigacion: dejar este design y el spec del capability como fuente normativa del punto de integracion esperado.
- [Riesgo] el datasource exponga capacidades incompletas o inconsistentes y el menu muestre opciones invalidas. → Mitigacion: mantener la construccion del menu basada estrictamente en `getAvailableAppTableExportModes` y en validaciones de seleccion.
- [Riesgo] responsive degrade la banda de controles y separe exportacion del contexto de tabla. → Mitigacion: conservar una estructura unica de `controlsBand` con reflow CSS, en lugar de duplicar triggers por breakpoint.
- [Riesgo] futuras exportaciones server-side intenten reutilizar la ruta local actual sin diferenciar tiempos y errores remotos. → Mitigacion: preservar `AppTableExport` como orquestador y extender el datasource con estrategias especificas cuando se implemente `allMatching`.
- [Trade-off] usar composicion en wrapper exige que cada pantalla conecte explícitamente `paginationActions`. → Mitigacion: esa pequena integracion explicita evita acoplar `AppTable` a una responsabilidad opcional y mantiene el patron reusable.

## Migration Plan

1. Formalizar el capability en spec para que la ubicacion de exportacion y la separacion de estados queden gobernadas por OpenSpec.
2. Mantener `AppTableExport` como trigger basado en `AppDropdown` y usar `AppTableQueryWrapper.paginationActions` como integracion de referencia.
3. Validar el patron en la pantalla consumidora actual sin introducir especializaciones por modulo.
4. Extender en un cambio posterior el datasource para `allMatching` server-side sin alterar la ubicacion visual decidida aqui.

Rollback: si la integracion visual introduce regresiones, la reversa debe limitarse a retirar el montaje desde `paginationActions` manteniendo intactos `AppTableExport` y `AppTableQueryWrapper`; no se debe migrar la logica al renderer base como solucion temporal.

## Open Questions

- Cuando se implemente `allMatching`, si el trigger debe mostrar progreso remoto diferenciado por formato o un estado generico de exportacion.
- Si futuras tablas en modo cards necesitan una variante visual de la misma banda de controles o solo ajustes de CSS conservando `paginationActions`.
- Si conviene elevar a spec comun del ecosistema `AppTable` la regla de que acciones operativas de tabla no deben montarse en `headerActions`.
