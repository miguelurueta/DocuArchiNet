# SCRUMCORE-206 — Información técnica del componente (AppVisorEmbedPdf)

## Ubicación
- `src/app/Components/UI/AppVisorEmbedPdf/`

## Rotación
- Plugin: `@embedpdf/plugin-rotate`
- Registro: `createPluginRegistration(RotatePluginPackage)`
- Render: wrapper `<Rotate>` por página cuando `rotation !== 0`.

## Toolbar (presentacional)
- Iconos: `@ant-design/icons`
  - Thumbnails: `MenuOutlined`
  - Zoom: `ZoomInOutlined` / `ZoomOutOutlined`
  - Reset zoom: `FileSyncOutlined`
  - Rotate: `RotateLeftOutlined` / `RotateRightOutlined`
- Tooltips en todos los botones (`title`).

## FAB “Ir arriba”
- Overlay en esquina inferior derecha (tipo WhatsApp).
- Icono: `UpOutlined`
- Muestra/oculta por scroll y hace `scrollTo({ x:0, y:0 })`.

## Estabilidad (decisión)
- Zoom se deshabilita cuando `rotation !== 0` para evitar “jump” (90°/180°/270°).

