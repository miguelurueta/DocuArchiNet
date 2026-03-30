## Context

El cambio `SCRUMCORE-18` busca corregir un problema clásico de layouts SPA: `AppContent` crece libremente, arrastra scroll al body o a la pagina completa y no respeta el espacio ocupado por `AppToolbar`. En `GestionCorrespondencia`, la estructura actual ya sigue el patron `Layout -> Page -> AppToolbar + AppContent`, pero el layout no estaba configurado para que el contenido restante fuera absorbido por `AppContent` como region scrollable interna.

La descripcion de Jira es explicita sobre la estrategia: usar flexbox, evitar `calc(100vh - Xpx)`, evitar JS para calcular alturas y asegurar `min-height: 0` en los contenedores flex relevantes. Tambien pide que el toolbar permanezca siempre visible y que el scroll ocurra solo dentro de `AppContent` cuando el contenido sea largo.

Como el cambio es puramente estructural/visual, debe preservar `AppToolbar`, `AppDropdown`, `AppButton`, el drawer contextual y la navegacion existente del modulo.

## Goals / Non-Goals

**Goals:**
- Hacer que `AppContent` ocupe automaticamente el alto restante del viewport debajo del toolbar.
- Evitar scroll del body o de la pagina completa cuando el contenido sea largo.
- Mantener scroll interno vertical solo dentro de `AppContent`.
- Hacer el layout estable y responsive en desktop, tablet y mobile.
- Resolver el problema con flexbox puro y propiedades de overflow adecuadas.

**Non-Goals:**
- No introducir calculos de altura con JS ni `calc(100vh - Xpx)`.
- No cambiar logica de negocio, routing ni comportamiento del drawer.
- No rediseñar visualmente `AppToolbar` o las acciones del modulo.
- No introducir dependencias nuevas ni hacks de medicion de DOM.

## Decisions

### 1. Convertir el layout del modulo a una cadena flex vertical completa

La solucion no puede quedarse solo en `AppContent`. Para que el contenido restante sea calculado correctamente, `GestionCorrespondenciaLayout`, el contenedor `.page` y el propio `AppContent` deben formar una cadena flex vertical con `min-height: 0` en los nodos intermedios.

Rationale:
- `flex: 1` solo funciona bien cuando todos los ancestros relevantes permiten contraccion y expansion.
- Evita que un padre con alto libre rompa el scroll interno del hijo.
- Resuelve el layout sin cálculos manuales.

Alternatives considered:
- Tocar solo `AppContent.module.css`: descartado porque no basta si el layout ancestro no participa del modelo flex correctamente.

### 2. Usar `overflow: hidden` en el contenedor padre y `overflow-y: auto` en `AppContent`

El body no debe scrollear; por eso el contenedor principal del modulo debe ocultar overflow y delegar el scroll al bloque interno del contenido. La decision es usar `overflow: hidden` en el wrapper/page y `overflow-y: auto` en el cuerpo de `AppContent`.

Rationale:
- Cumple exactamente el comportamiento esperado del ticket.
- Mantiene la toolbar visible mientras el contenido se desplaza.
- Es el patron correcto para shells SPA con paneles internos scrollables.

Alternatives considered:
- Mantener scroll en el body: descartado por mala UX y por incumplir el ticket.
- Aplicar overflow al root completo de `AppContent` sin estructurar el body: descartado porque dificulta preservar header/footer internos de forma limpia.

### 3. Hacer que `AppContent` sea un contenedor flex con body expandible

`AppContent` actualmente usa `display: grid` y bloques visuales independientes. Para soportar “altura restante + scroll interno”, la decision es convertir el root a un contenedor flex vertical y el `.body` a una region expandible con `flex: 1 1 auto` y `min-height: 0`.

Rationale:
- Permite que `header`, `body` y `footer` convivan sin perder el patron visual del componente.
- Hace que el scroll interno viva en la zona correcta.
- Mantiene la API publica del componente sin necesidad de props nuevas.

Alternatives considered:
- Agregar una prop de altura dinamica a `AppContent`: descartado por sobrecomplicar un comportamiento que puede resolverse estructuralmente.

## Risks / Trade-offs

- [Risk] Cambiar `AppContent` de grid a flex puede afectar otros consumidores. -> Mitigation: mantener la misma API y la misma estructura visual de header/body/footer, cambiando solo el motor de layout necesario para el scroll interno.
- [Risk] `overflow: hidden` en el padre puede ocultar contenido si algun hijo no tiene `min-height: 0`. -> Mitigation: asegurar `min-height: 0` en layout, page y content.
- [Risk] La validacion automatizada cubre flujo, no todas las propiedades CSS. -> Mitigation: usar el test de ruta como smoke test y dejar el comportamiento visual explicitado en specs/tasks.

## Migration Plan

1. Refinar proposal/specs/tasks para describir el ajuste real de alto restante y scroll interno.
2. Ajustar `GestionCorrespondenciaLayout` y `.page` para usar flex vertical con `min-height: 0` y `overflow: hidden`.
3. Ajustar `AppContent` para comportarse como panel expandible con scroll interno.
4. Ejecutar la prueba de ruta del modulo como smoke test de no regresion.

Rollback:
- Si el cambio genera regresiones en otros consumidores, revertir el layout de `AppContent` y limitar el comportamiento al modulo hasta aislar una variante mas segura.

## Open Questions

- Si otros modulos del proyecto deberian adoptar el mismo patron de “toolbar fija + content scrollable” usando `AppContent`.
