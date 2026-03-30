# Design

## Context

`SCRUMCORE-21` pide un refinamiento visual pequeño pero explícito sobre la clase `.page` de `GestionCorrespondencia`. El layout actual ya organiza la página como una columna principal que contiene `AppToolbar` y `AppContent`, con reglas específicas para desktop y mobile. El objetivo del ticket no es rediseñar la estructura ni tocar componentes compartidos, sino mejorar la presentación del wrapper visual que encapsula ambos bloques.

La descripción Jira fija tres estilos obligatorios:

- `gap: 2px`
- `border-radius: 24px`
- `background-color: #f5f5f5`

El cambio debe respetar CSS Modules, preservar el layout existente y funcionar de forma consistente en todos los breakpoints.

Durante la implementación también se añadió un ajuste complementario sobre las acciones de toolbar de `GestionCorrespondencia`: ambos `AppButton` pasan a `variant="ghost"` y el estilo base de `.variantGhost` se normaliza a texto negro para mantener coherencia visual con el nuevo wrapper de página.

## Goals / Non-Goals

### Goals

- Aplicar los tres estilos requeridos a `.page`.
- Mantener la composición actual entre toolbar y content.
- Garantizar consistencia visual en desktop, tablet y mobile.
- Mantener el cambio estrictamente en `GestionCorrespondencia.module.css`.
- Alinear los botones visibles de la toolbar con una variante visual consistente con la nueva superficie.

### Non-Goals

- Rediseñar `AppToolbar` o `AppContent`.
- Cambiar JSX o lógica en `GestionCorrespondencia.tsx`.
- Introducir ocultamiento visual, cambios de navegación o comportamientos nuevos.
- Alterar el shell global del dashboard.

## Decisions

### 1. El cambio se limita al wrapper `.page`

La clase `.page` ya es el contenedor semántico y visual de la pantalla, por lo que es el punto correcto para aplicar un fondo y radio unificados sin invadir componentes hijos. Esto mantiene el cambio local y coherente con el ticket.

Alternativas descartadas:

- Aplicar fondo/radio en `.toolbar` o `.content`: produciría una composición fragmentada y no responde al requerimiento literal de Jira.

### 2. El `gap` se normaliza a `2px` en todos los breakpoints

Jira pide explícitamente consistencia del `gap` en desktop, tablet y mobile. Por eso el diseño asume que cualquier regla responsive existente que cambie el `gap` debe converger a `2px` en `.page`.

Alternativas descartadas:

- Mantener gaps distintos por breakpoint: contradice la instrucción funcional del ticket.

### 3. No se eliminan estilos existentes fuera del mínimo necesario

El ticket pide extender y no sobrescribir propiedades críticas sin validar. La decisión es conservar `display`, `flex/grid`, alturas y overflow actuales, limitando la modificación al `gap`, `border-radius` y `background-color`.

### 4. Las acciones visibles adoptan `ghost` con texto negro

Como refinamiento visual adicional solicitado durante la implementación, las acciones `Exportar` y `Abrir respuesta contextual` se renderizan con `variant="ghost"` en `GestionCorrespondencia.tsx`. Para que esa variante sea legible sobre la nueva superficie clara, `.variantGhost` en `AppButton.module.css` usa `color: black`.

## Risks / Trade-offs

- Un `gap` tan pequeño puede compactar mucho la separación visual entre toolbar y content; se acepta porque el ticket lo exige explícitamente.
- El fondo aplicado a `.page` puede ser menos perceptible si los bloques hijos ya usan superficies propias; aun así el wrapper quedará alineado con el diseño pedido.
- Cambiar el color base de `.variantGhost` puede impactar otros consumidores del componente; se acepta como decisión visual explícita mientras siga siendo consistente con el design system actual.
