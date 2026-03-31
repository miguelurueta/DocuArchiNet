## Context

El modulo Workflow ya existe y cuenta con layout y rutas. El ticket SCRUMCORE-25 exige que `Workflow.tsx` componga la UI con `AppToolbar` y `AppContent`, manteniendo el layout como estructura sin logica de negocio. Se requiere un comportamiento responsive enterprise y soporte para contenido amplio (AG Grid/Ant Design).

## Goals / Non-Goals

**Goals:**
- Componer `Workflow.tsx` usando `AppToolbar`, `AppContent`, `AppButton` y `AppDropdown`.
- Mantener `WorkflowLayout` como contenedor estructural (sin logica de negocio).
- Asegurar responsividad y manejo de overflow para contenido grande.
- Respetar CSS Modules y convenciones del repositorio.

**Non-Goals:**
- Implementar logica de negocio, integraciones reales o datos.
- Introducir nuevas dependencias o cambios globales en el layout de la app.

## Decisions

- **Toolbar + Content en la pagina**: `Workflow.tsx` controla `AppToolbar` y `AppContent` para cumplir el orden obligatorio. Alternativa: mover a layout, descartada por las reglas del ticket.
- **CSS Modules dedicados**: `Workflow.module.css` define contenedor, toolbar y wrapper con reglas de wrap/spacing. Alternativa: estilos inline, descartada por consistencia.
- **Placeholders enterprise**: se deja un placeholder de tabla dentro de `AppContent` para futuras integraciones (AG Grid/Ant Design). Alternativa: contenido vacio, descartada por requerimiento visual.
- **Overflow controlado**: `AppContent` con scroll y wrapper con `min-height: 0` para evitar overflow global.
- **Altura del toolbar**: se ajusta la altura minima del bloque de acciones a `55px` para mantener alineacion horizontal y evitar desbordes.

## Risks / Trade-offs

- [Desfase con estilos existentes] -> Mitigacion: reutilizar AppToolbar/AppContent y tokens visuales existentes.
- [Overflow horizontal en mobile] -> Mitigacion: usar wrap en toolbar y limitar anchos en placeholders.
- [Confusion de responsabilidades] -> Mitigacion: mantener layout solo como estructura y pagina como composicion.

## Migration Plan

- Cambios solo en modulo Workflow, sin migraciones.
- Rollback: revertir `Workflow.tsx` y `Workflow.module.css` si fuese necesario.

## Open Questions

- Validacion exacta de variantes de `AppToolbar` y `AppContent` preferidas por UX.
- Alcance de pruebas de responsive y overflow (unitarias vs visuales).
