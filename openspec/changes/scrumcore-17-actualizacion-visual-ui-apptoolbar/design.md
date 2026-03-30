## Context

El cambio `SCRUMCORE-17` no introduce nuevas acciones ni nuevas primitives compartidas. El ticket es un refinamiento visual acotado al contenedor `AppToolbar` ya consumido por `GestionCorrespondencia`, cuyo problema actual aparece cuando las acciones del toolbar hacen wrap en resoluciones tablet o mobile y la altura del contenedor no acompaña correctamente ese crecimiento.

El alcance descrito en Jira es explícito: no modificar botones, no cambiar `AppDropdown`, no cambiar `AppButton` y no alterar la estructura interna del JSX salvo que fuese estrictamente necesario. La solucion debe concentrarse en CSS Modules del modulo consumidor, ajustando layout del contenedor y del wrapper inmediato para que el alto del toolbar sea automatico, crezca con el contenido y no corte ni esconda controles al pasar a multiples lineas.

Como `SCRUMCORE-16` ya definio la toolbar con dos acciones visibles (`Exportar` y `Abrir respuesta contextual`), este cambio debe preservar exactamente esa composicion y limitarse a mejorar el comportamiento responsive del contenedor donde esas acciones viven. Durante la validacion surgio un hallazgo adicional: en el breakpoint de `1100px`, `AppToolbar` cambiaba a `flex-direction: column` pero mantenia `flex-basis` heredados en `.context` y `.actions`, lo que podia inflar la altura del contenedor con un valor artificial en lugar de ajustarla al contenido real.

## Goals / Non-Goals

**Goals:**
- Corregir la altura del `AppToolbar` consumido por `GestionCorrespondencia` para que sea automatica y crezca dinamicamente con el contenido.
- Garantizar wrap correcto de acciones en desktop estrecho, tablet y mobile sin corte visual.
- Asegurar que el contenedor padre del modulo no impida el crecimiento vertical del toolbar.
- Mantener intacta la logica de acciones y la estructura JSX ya aprobada del ticket anterior.

**Non-Goals:**
- No cambiar la API de `AppButton`, `AppDropdown` ni la composicion de acciones del toolbar.
- No agregar ni quitar botones.
- No introducir cambios funcionales de navegacion, drawer o exportacion.
- No rediseñar globalmente `AppToolbar.module.css` para todos los consumidores del proyecto.

## Decisions

### 1. Resolver primero el ajuste en el CSS Module del modulo consumidor

La decision es aplicar el refinamiento en `src/modules/gestionCorrespondencia/style/GestionCorrespondencia.module.css`, usando la clase `toolbar` que ya se inyecta via `className` al `AppToolbar`, y reforzando tambien el contenedor padre del modulo.

Rationale:
- El ticket pide explicitamente modificar solo estilos del toolbar consumidor.
- Evita que un ajuste de layout puntual impacte otros modulos que usan `AppToolbar`.
- Mantiene el cambio reversible y acotado.

Alternatives considered:
- Modificar `AppToolbar.module.css` globalmente: descartado por alcance excesivo.
- Alterar JSX para insertar wrappers extra: descartado porque el problema es de layout CSS, no estructural.

### 2. Tratar el toolbar como contenedor flexible con altura automatica

La toolbar debe usar `display: flex`, `flex-wrap: wrap`, `height: auto`, `min-height: fit-content`, `align-items: center`, `align-content: flex-start` y `overflow: visible`, de forma que su caja se expanda cuando el grupo de acciones ocupe varias lineas.

Rationale:
- Resuelve exactamente el problema descrito por Jira.
- Evita alturas fijas y clipping visual.
- Permite que los botones sigan su flujo natural sin hacks de JS.

Alternatives considered:
- Mantener un alto fijo y compensar con padding: descartado porque sigue fallando cuando el contenido crece.
- Forzar una sola linea con `nowrap`: descartado porque contradice el requerimiento responsive.

### 3. Ajustar tambien el wrapper de acciones y el contenedor padre

No basta con poner `height: auto` en la toolbar si el wrapper de acciones o el contenedor padre siguen limitando el crecimiento. La decision es alinear `toolbarActionGroup` con wrap, `overflow: visible` y `min-width: 0`, y mover `.page` a un contenedor columnar con `min-height: 0`.

Rationale:
- Reduce el riesgo de que el crecimiento del toolbar sea bloqueado por un ancestro.
- Sigue la advertencia expresa del ticket sobre contenedores padres.
- Hace el comportamiento mas estable en layouts complejos.

Alternatives considered:
- Tocar solo `.toolbar`: descartado porque puede dejar cuellos de botella en wrappers internos.

### 4. Corregir el breakpoint base de `AppToolbar` cuando el layout cambia a columna

Durante la validacion surgio un problema adicional no completamente resuelto desde el modulo: al entrar al breakpoint `@media (max-width: 1100px)` en `AppToolbar.module.css`, el componente pasaba a columna pero conservaba `flex: 1 1 34rem` en `.context` y `flex: 1 1 22rem` en `.actions`. En direccion columna, esos `flex-basis` dejan de comportarse como ancho y empiezan a empujar la altura del bloque.

La decision es ajustar el CSS base del componente en ese breakpoint para forzar `flex: 0 1 auto` en `.context` y `.actions`, permitiendo que el toolbar retome un alto determinado por contenido real.

Rationale:
- Corrige el comportamiento observado alrededor de `1100px`, donde el toolbar quedaba con una altura artificial.
- No cambia la API ni la estructura JSX del componente.
- Mantiene el layout de columna, pero elimina la reserva de espacio vertical no deseada.

Alternatives considered:
- Compensar solo desde el CSS del modulo consumidor: descartado porque el problema nace dentro del propio breakpoint base del componente.
- Eliminar completamente el `flex` de las regiones en todos los breakpoints: descartado porque afectaria el layout normal en desktop.

## Risks / Trade-offs

- [Risk] Un ajuste local de `display` en el contenedor del modulo puede alterar ligeramente espaciados existentes. -> Mitigation: mantener el gap actual y limitar los cambios a layout/altura.
- [Risk] El comportamiento visual en desktop amplio puede variar minimamente por el nuevo wrap. -> Mitigation: preservar alineacion horizontal y gap reducido para que el estado de una sola linea siga compacto.
- [Risk] Tocar `AppToolbar.module.css` puede impactar otros consumidores del componente. -> Mitigation: limitar el cambio al breakpoint de `1100px` y solo a la neutralizacion del `flex-basis` al pasar a columna.
- [Risk] El ticket habla de validacion visual pero no de pruebas automatizadas de CSS. -> Mitigation: conservar pruebas de ruta para asegurar que no hubo regresiones funcionales y documentar el alcance visual en OpenSpec.

## Migration Plan

1. Refinar proposal/specs/tasks para describir el ajuste visual real del ticket.
2. Aplicar cambios de CSS en `GestionCorrespondencia.module.css` sobre toolbar, wrapper de acciones y contenedor padre.
3. Corregir el breakpoint base de `AppToolbar.module.css` para que `.context` y `.actions` no conserven `flex-basis` vertical artificial en `<=1100px`.
4. Verificar que `GestionCorrespondencia` sigue renderizando la toolbar y el drawer sin cambios funcionales.
5. Ejecutar la prueba de ruta del modulo como smoke test de no regresion.

Rollback:
- Si el ajuste visual genera regresiones de layout no deseadas, revertir el CSS del modulo a su estado anterior sin necesidad de tocar componentes compartidos.

## Open Questions

- Si en una iteracion futura el ajuste de altura responsive debe generalizarse al `AppToolbar` base para todos los consumidores o seguir local al modulo.
