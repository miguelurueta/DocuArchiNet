# SCRUMCORE-194 \u2014 AppVisorPdf responsive + a11y (05-FE)

## Goal
Hacer `AppVisorPdf` usable y consistente en **mobile/tablet/desktop**, con foco en:

- Toolbar adaptativa (modo compacto <= 768px).
- Thumbnails colapsables (overlay en mobile, rail colapsable en tablet/desktop).
- Accesibilidad (labels, roles, focus visible, soporte teclado; shortcuts opcionales).

## Non-goals

- NO integrar `AppVisorPdf` en pantallas/m\u00f3dulos consumidores.
- NO fijar alturas; evitar layouts fr\u00e1giles (usar `min-height: 0` y overflow controlado).

## Proposed UI pieces

1. **Toolbar responsiva**
   - Desktop: botones + dropdown para herramientas.
   - Mobile: reduce controles visibles y mueve secundarios a `AppDropdown`.
   - Hit targets: >= 40px.

2. **Thumbnails**
   - Mobile: drawer/overlay que tapa viewport, con backdrop y cierre.
   - Tablet/Desktop: rail lateral (izquierda) colapsable.
   - El contenido del viewport debe mantener `min-height: 0` para permitir scroll interno.

3. **A11y**
   - `aria-label` en acciones.
   - Thumbnails toggle: `aria-expanded`, `aria-controls`.
   - Focus management: al abrir thumbnails, focus al primer elemento; al cerrar, volver al trigger.
   - Teclado: tab/shift+tab y enter/space en triggers; shortcuts opcionales documentadas.

## File touch points

```
src/app/Components/UI/AppVisorPdf/presentation/
src/app/Components/UI/AppVisorPdf/AppVisorPdf.module.css
```

