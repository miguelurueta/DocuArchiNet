## Context

`SCRUMCORE-71` parte del shell persistente ya implementado en `SCRUMCORE-70` para `GestionCorrespondencia`. El modulo ya no usa `Drawer`; ahora mantiene la bandeja visible y renderiza `GestionRespuesta` dentro de una region secundaria persistente controlada por routing.

El nuevo ticket no debe reabrir esa decision. El alcance sugerido por Jira es ajustar `GestionRespuesta` para que el cierre y el retorno se comporten de forma mas cercana a una experiencia tipo Gmail: el usuario debe percibir que esta trabajando dentro de un detalle contextual con una accion de regreso clara, sin acoplar la pagina secundaria a la logica de rutas ni mover la responsabilidad de navegacion fuera del shell del modulo.

Estado actual relevante:

- `GestionCorrespondenciaRoute` ya posee header, accion de cierre y region secundaria persistente.
- `GestionRespuesta` sigue siendo un placeholder visual simple.
- El cierre actual depende solo del control del shell, no de una experiencia de retorno visible dentro del contenido secundario.
- La URL sigue siendo la fuente de verdad y eso debe preservarse.

Este cambio toca principalmente:

- `pages/GestionRespuesta.tsx`
- `routes/GestionCorrespondenciaRoute.tsx`
- estilos del shell o de la pagina secundaria
- pruebas de routing y render observable

## Goals / Non-Goals

**Goals:**

- Refinar `GestionRespuesta` para que tenga una experiencia de retorno/cierre coherente con el shell persistente del modulo.
- Mantener la navegacion gobernada por routing.
- Hacer mas evidente la relacion entre bandeja principal y detalle secundario.
- Mantener la pagina secundaria desacoplada del mecanismo concreto de cierre.
- Dejar el shell listo para futuras vistas secundarias con el mismo patron.

**Non-Goals:**

- No reemplazar el shell persistente por overlay o `Drawer`.
- No mover la fuente de verdad de navegacion a estado local.
- No convertir `GestionRespuesta` en una pantalla funcional de negocio completa.
- No tocar `AppTable`, `AppToolbar`, `AppTableQueryWrapper` ni el flujo de datos de la bandeja.
- No introducir contexto global ni dependencia nueva para navegacion secundaria.

## Decisions

### 1. El ticket refina la vista secundaria, no reemplaza el shell

Alternativas consideradas:

- volver a un overlay por encima de la bandeja
- dejar el shell igual y solo ajustar copy
- refinar la presentacion de `GestionRespuesta` dentro del shell existente

Decision:

- mantener el shell persistente actual y refinar la experiencia interna de `GestionRespuesta`.

Razonamiento:

- `SCRUMCORE-70` ya formalizo el patron arquitectonico del modulo.
- Revertirlo en este ticket generaria ruido en specs, pruebas y UX.
- El ajuste pedido encaja mejor como refinamiento de detalle secundario.

### 2. La accion de retorno debe seguir gobernada por la capa de rutas

Alternativas consideradas:

- hacer que `GestionRespuesta` use `useNavigate` directamente
- pasar callbacks de cierre a `GestionRespuesta`
- mantener la navegacion en `GestionCorrespondenciaRoute` y exponer una composicion visual mas clara

Decision:

- la capa routes sigue controlando la navegacion; `GestionRespuesta` no decide por si sola como cerrar.

Razonamiento:

- preserva desacoplamiento entre pagina secundaria y router.
- permite reutilizar el mismo patron para otras vistas secundarias.
- evita que cada vista hija implemente su propia logica de retorno.

### 3. El retorno tipo Gmail se resuelve con jerarquia visual y affordance explicita

Alternativas consideradas:

- depender solo del boton de cierre del shell
- agregar un “volver” dentro de `GestionRespuesta` con navegacion propia
- reorganizar el header del panel y el contenido secundario para que el retorno sea visible y contextual

Decision:

- la experiencia debe reforzarse desde el shell y la composicion visual del detalle: header contextual, copy de retorno y una accion principal de cierre/volver claramente visible.

Razonamiento:

- en una experiencia tipo Gmail el usuario identifica rapido como volver al listado sin perder contexto.
- eso puede lograrse sin acoplar la pagina secundaria al router.
- el comportamiento observable sigue siendo el mismo: volver a la ruta base.

### 4. La spec debe modificar `gestion-correspondencia`, no crear una capability nueva artificial

Alternativas consideradas:

- dejar la capability derivada del nombre Jira
- modificar la spec existente del modulo

Decision:

- el cambio debe vivir en `gestion-correspondencia`.

Razonamiento:

- no existe un capability independiente para “navegacion 02”.
- el ticket extiende el comportamiento del modulo ya especificado.

## Risks / Trade-offs

- [Refinar cierre/retorno sin cambiar arquitectura puede parecer un cambio menor] -> Mitigacion: dejar la spec enfocada en comportamiento observable y no en maquillaje visual.
- [Demasiada UI de retorno dentro del panel puede duplicar controles] -> Mitigacion: definir una sola accion dominante de volver/cerrar y evitar duplicacion confusa.
- [Acoplar `GestionRespuesta` a routing por comodidad] -> Mitigacion: mantener navegacion en la capa route y testear esa separacion.
- [El cambio puede interpretarse como reposicionar la vista por encima de la bandeja] -> Mitigacion: documentar explicitamente que la bandeja sigue visible y que el patron sigue siendo shell persistente.

## Migration Plan

1. Corregir el `proposal.md` para que modifique `gestion-correspondencia`.
2. Crear delta spec enfocada en cierre/retorno observable dentro del shell persistente.
3. Ajustar `GestionRespuesta` y, si hace falta, el header del shell para clarificar el retorno.
4. Actualizar pruebas del modulo sobre el flujo de cierre y persistencia de la bandeja.
5. Validar README/specs y archivar el cambio cuando este implementado.

Rollback:

- si el refinamiento introduce confusion o regresiones, se puede volver a la version actual del shell persistente sin afectar la decision estructural tomada en `SCRUMCORE-70`.

## Open Questions

- Confirmar si el ticket espera un boton “volver” dentro del contenido secundario o solo una mejor jerarquia visual de cierre en el shell.
- Confirmar si el copy “tipo Gmail” implica solo patron de retorno o tambien cambios de densidad/espaciado del panel.
- Confirmar si futuras vistas secundarias deben reutilizar exactamente el mismo header o solo el mismo comportamiento de navegacion.
