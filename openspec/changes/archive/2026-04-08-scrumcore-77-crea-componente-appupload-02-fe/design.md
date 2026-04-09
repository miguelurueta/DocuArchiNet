## Context

- Cambio OpenSpec para `SCRUMCORE-77` enfocado en la segunda fase del componente `AppUpload` (UI/UX y responsive).
- Existe especificacion detallada en `docs/Architecture/AppUpload/AppUpload-Architecture.md` y tickets FE.
- La fase anterior (SCRUMCORE-75) ya definio core/estrategias; esta fase enfoca UI, UX, accesibilidad y performance visual.

## Goals / Non-Goals

**Goals:**
- Implementar mejoras visuales de `AppUpload`: cards 1:1, hover elevation, preview galeria, acciones overlay.
- Asegurar layout responsive (Desktop 46 columnas, Tablet 23, Mobile 2) y estados visuales drag & drop.
- Mejorar accesibilidad (teclado, focus visible, aria-label) sin romper contrato existente.

**Non-Goals:**
- Cambiar contratos core o estrategias de carga ya definidos.
- Implementar endpoints backend o integraciones de negocio.
- Migrar pantallas consumidoras existentes.

## Decisions

1. **UI como capa separable del core**
   - Se mantiene el contrato base y se mejora la capa visual en CSS Modules.
   - Razon: evitar regresiones en el core y permitir mejoras iterativas.

2. **Responsive con grilla basada en columnas**
   - Desktop 46, Tablet 23, Mobile 2.
   - Razon: coherencia con arquitectura y densidad controlada.

3. **Accesibilidad y focus visible**
   - Teclado soportado y focus visible en items/acciones.
   - Razon: cumplimiento de accesibilidad enterprise.

## Risks / Trade-offs

- **Riesgo de romper estilos previos** -> Mitigacion: CSS encapsulado por modulo.
- **Sobrepeso visual en mobile** -> Mitigacion: padding reducido y previews compactos.
- **Acciones ocultas en hover** -> Mitigacion: focus visible y aria-labels.

## Migration Plan

- No requiere migracion: los cambios son internos al componente.
- Validar que el layout responsivo no afecte consumidores.

## Open Questions

- ¿Se requiere modo compacto adicional para tablas?
- ¿Se desean tokens de color centralizados para estados drag?
