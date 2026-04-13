# Ticket 02 FE

## Titulo

Diseño UI/UX + responsive de `AppCollapseRail` (desktop/tablet/mobile)

## Objetivo

Alinear el look&feel del `AppCollapseRail` con la apariencia del panel de herramientas
de GestionRespuesta y garantizar buen comportamiento responsive.

## Contexto existente

- Arquitectura: `docs/Architecture/AppCollapseRail/AppCollapseRail-Architecture.md`
- Referencia visual: estilos de `GestionRespuestaMainTabContent.module.css`

## Restricciones (obligatorio)

- CSS Modules obligatorio
- Sin estilos globales
- Transiciones suaves (opacity + transform)
- No romper layout del contenedor

## Ubicacion (obligatoria)

```txt
src/app/Components/UI/AppCollapseRail/AppCollapseRail.module.css
```

## Reglas de UI (obligatorio)

- Panel con fondo claro, borde sutil y radius 12-20px.
- Rail flotante tipo chip con sombra suave.
- Iconos consistentes con `RightOutlined` / `LeftOutlined`.
- Hover y focus visibles.
- Panel con scroll interno si overflow.

## Responsive (obligatorio)

Desktop (>= 1025px)

- Panel inline dentro del layout.
- Colapso desplaza fuera del viewport lateral.

Tablet (769px - 1024px)

- Panel colapsado por defecto (contendor lo decide).
- Rail visible siempre.
- Panel con ancho reducido.

Mobile (<= 768px)

- Panel overlay tipo bottom-sheet.
- Altura maxima 70% - 80% del viewport.
- Rail flotante abajo derecha con label visible.
- Handle visual superior (barra horizontal).

## Tokens sugeridos

- `--rail-bg`, `--rail-border`, `--rail-shadow`
- `--panel-bg`, `--panel-border`, `--panel-shadow`

## Pruebas visuales (obligatorio)

- Desktop: panel abierto/colapsado sin saltos.
- Tablet: rail visible + panel colapsado por defecto.
- Mobile: overlay + rail tipo chip.

## Criterios de aceptacion

- Estilos alineados a GestionRespuesta.
- Responsive consistente en 3 breakpoints.
- Transiciones suaves y sin parpadeos.
