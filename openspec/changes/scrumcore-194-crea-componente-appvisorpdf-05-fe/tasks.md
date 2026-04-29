## 1. Toolbar responsive

- [x] 1.1 Agregar modo compacto <= 768px (class o `data-compact="true"`) en `AppVisorPdfToolbar`
- [x] 1.2 Mover acciones secundarias a `AppDropdown` en modo compacto
- [x] 1.3 Asegurar hit-target >= 40px en botones (CSS)

## 2. Thumbnails UI

- [x] 2.1 Implementar thumbnails (UI scaffold) en `src/app/Components/UI/AppVisorPdf/presentation/` (lista simple por p\u00e1gina)
- [x] 2.2 Mobile: drawer/overlay colapsable (backdrop + close)
- [x] 2.3 Tablet/Desktop: rail lateral colapsable (no fijo por altura; usar `min-height: 0`)
- [x] 2.4 Agregar control toggle visible en toolbar (o header) para abrir/cerrar thumbnails

## 3. A11y

- [x] 3.1 Toggle thumbnails con `aria-expanded` + `aria-controls`
- [x] 3.2 Focus management: al abrir -> focus dentro; al cerrar -> focus vuelve al trigger
- [x] 3.3 Focus visible consistente en controles principales

## 4. Tests

- [x] 4.1 Test: en viewport mobile aplica toolbar compacta (assert clase/data-attr)
- [x] 4.2 Test: toggle thumbnails accesible (button + aria-expanded cambia)
- [x] 4.3 Test: controles principales accesibles por teclado (enter/space)

## 5. Documentation

- [x] 5.1 Actualizar `src/app/Components/UI/AppVisorPdf/README.md` con comportamiento responsive (mobile/tablet/desktop)
- [x] 5.2 Documentar consideraciones A11y (roles/aria/focus) y touch targets
