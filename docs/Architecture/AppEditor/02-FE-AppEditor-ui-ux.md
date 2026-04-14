# Ticket 02 FE

## Título

Diseño UI/UX responsive de `AppEditor` con modo claro/oscuro

## Rol

Desarrollador Frontend Senior especializado en:

- React 19 + TypeScript estricto
- Clean Architecture
- Diseño de sistemas UI (Design Systems)
- Accesibilidad (a11y)
- CSS Modules y arquitectura visual escalable

## Objetivo

Diseñar la experiencia visual y responsive de `AppEditor` con enfoque
mobile-first, garantizando usabilidad táctil, consistencia visual,
accesibilidad y compatibilidad nativa con tema claro y oscuro.

## Contexto existente

- Arquitectura: `docs/Architecture/AppEditor/AppEditor-Architecture.md`
- Implementación base: `docs/Architecture/AppEditor/01-FE-AppEditor-core.md`
- Referencia estructural: `docs/Architecture/AppCollapseRail/`

## Restricciones (obligatorio)

- CSS Modules obligatorio
- Sin estilos globales
- Sin romper layout del contenedor
- Toolbar usable en mobile
- Light/Dark mode obligatorio
- No introducir lógica de negocio en estilos

## Ubicación (obligatoria)

`src/app/Components/UI/AppEditor/AppEditor.module.css`

## Regla arquitectónica (obligatoria)

Este ticket afecta exclusivamente la capa de `presentation`.

- No modificar `application`
- No modificar `infrastructure`
- No introducir lógica en CSS
- Mantener desacoplamiento total del core del editor

## Integración (obligatorio)

El archivo `AppEditor.module.css` debe ser consumido únicamente por:

- `AppEditor.tsx`
- `AppEditorToolbar.tsx`

No debe ser reutilizado fuera del componente.

## Reglas de UI (obligatorio)

- Toolbar clara, compacta y reusable
- Superficie del editor con alta legibilidad
- Estados `hover`, `focus` y `active` visibles
- Dropdown de headings usable y consistente
- Controles de enlaces e imágenes claros para el usuario
- Estados visuales para `disabled`, `readOnly` y `error`
- Jerarquía visual clara entre toolbar, editor y estados
- Consistencia con el design system existente

## Responsive (obligatorio)

### Mobile (<= 768px)

- Diseño optimizado por defecto
- Toolbar compacta o adaptable a múltiples filas
- Controles táctiles con área suficiente (mínimo 40px)
- Sin overflow horizontal del editor
- Interacciones claras sin necesidad de zoom

### Tablet (769px - 1024px)

- Toolbar parcialmente expandida
- Mejor distribución de acciones frecuentes
- Espaciado equilibrado en el editor

### Desktop (>= 1025px)

- Toolbar completa
- Mejor aprovechamiento horizontal
- Jerarquía visual clara y estable
- Espaciado consistente

## Tokens (obligatorio)

Los estilos deben usar variables CSS (`custom properties`), ya sea definidas en
el módulo o integradas con el sistema de temas global.

Variables mínimas:

- `--editor-bg`
- `--editor-border`
- `--editor-toolbar-bg`
- `--editor-toolbar-border`
- `--editor-focus`
- `--editor-muted`
- `--editor-error`

Debe soportar override para dark mode.

## Reglas de performance visual (obligatorio)

- Evitar layout shift (CLS)
- Evitar reflows innecesarios
- No usar animaciones pesadas
- Transiciones suaves (`< 200ms`)
- No bloquear interacción del usuario

## Accesibilidad (obligatorio)

- Focus visible en todos los controles interactivos
- Contraste mínimo WCAG AA
- Estados disabled claramente distinguibles
- Hover no debe ser el único indicador visual
- Navegación coherente entre elementos

## Pruebas visuales (obligatorio)

- Mobile: toolbar usable sin overflow
- Tablet: distribución estable sin solapamientos
- Desktop: toolbar completa sin saturación
- Light mode: contraste adecuado
- Dark mode: contraste y jerarquía mantenidos
- No solapamiento de botones
- Estados hover/focus visibles en todos los elementos
- No ruptura visual en cambios de estado

## Criterios de aceptación

- UI responsive consistente en los 3 breakpoints
- Light/Dark mode funcional desde fábrica
- Usabilidad correcta en dispositivos táctiles y desktop
- Sin parpadeos, saltos o quiebres visuales
- Sin overflow horizontal
- Accesibilidad visual validada
- Integración limpia con `AppEditor`
- Alineación con design system

## Instrucción final

Implementar respetando estrictamente:

- Clean Architecture (solo capa `presentation`)
- Uso exclusivo de CSS Modules
- Uso de tokens visuales
- Mobile-first
- Accesibilidad (WCAG)
- Performance visual

El resultado debe ser una UI estable, reusable, accesible y lista para
producción en un entorno SaaS.
