# gestion-correspondencia Specification

## ADDED Requirements

### Requirement: Remount completo del detalle por ruta de tarea
El sistema SHALL resolver la ruta de detalle con una clave determinística derivada de `parsedId` para que el subárbol completo se reconstruya por tarea.

#### Scenario: Navegación entre tareas distintas
- **WHEN** el usuario entra a `/dashboard/gestion-correspondencia/respuesta/:idA` y luego `/dashboard/gestion-correspondencia/respuesta/:idB`
- **THEN** el componente que contiene `GestionRespuesta` SHALL usar una `key` basada en `parsedId` que fuerce remount completo.
- **AND** no SHALL reutilizar estado del detalle anterior.

### Requirement: Estado transversal limpio entre remounts
Al remount de detalle, los providers y estado contextual de `GestionRespuesta` asociados a una tarea anterior SHALL iniciar en estado neutro para la siguiente tarea.

#### Scenario: Providers dentro del subárbol remonteado
- **WHEN** la clave de detalle cambia por `parsedId`
- **THEN** `GestionRespuestaDocumentosProvider`, tabs y componentes dependientes SHALL reconstruirse desde estado inicial.
- **AND** `files`, `activeRowId`, `activeFileUrl` y estado del editor no SHALL persistir entre tareas.

### Requirement: Anti-stale y limpieza en unmount de detalle
El sistema SHALL cancelar o ignorar operaciones asíncronas pendientes al desmontar `GestionRespuesta` para evitar que contaminen el nuevo detalle.

#### Scenario: Requests pendientes al cambiar de tarea
- **WHEN** un request del detalle anterior está en vuelo durante un cambio rápido a otra tarea
- **THEN** el nuevo detalle SHALL permanecer coherente y consistente, descartando estados actualizados por peticiones obsoletas.

## MODIFIED Requirements

### Requirement: El modulo Gestion Correspondencia SHALL integrar una ruta secundaria tipo Gmail con detalle montado en shell persistente
El sistema SHALL implementar `GestionCorrespondenciaRoute` como un shell de navegacion tipo Gmail para el modulo, manteniendo visible la vista principal y renderizando las vistas secundarias dentro de una region persistente del layout gobernada por la URL y no por un overlay modal acoplado a estado local.

#### Scenario: Deep link a la vista secundaria mantiene shell persistente
- **WHEN** el usuario entra directamente a `/dashboard/gestion-correspondencia/respuesta/:id`
- **THEN** el sistema SHALL resolver la ruta y renderizar la vista secundaria en la region persistente.
- **AND** la navegación entre tareas en esa region SHALL usar remount por `parsedId` sin degradar el shell principal.

### Requirement: GestionRespuesta SHALL renderizarse como vista secundaria desacoplada dentro del shell del modulo
El sistema SHALL implementar `GestionRespuesta` como una pagina secundaria preparada para mostrarse dentro de la region persistente del shell de `GestionCorrespondencia`.

#### Scenario: Re-render por cambio de id sin reusar subárbol
- **WHEN** `GestionRespuesta` recibe un `id` distinto
- **THEN** el contenido visible SHALL reiniciarse por remount y mostrar el estado correspondiente a la nueva tarea.
