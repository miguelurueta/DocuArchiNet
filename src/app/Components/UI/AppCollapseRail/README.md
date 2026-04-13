# AppCollapseRail

Componente reusable para panel lateral colapsable con rail de restauracion,
alineado al comportamiento del workbench de GestionRespuesta.

## Instalacion

```tsx
import { AppCollapseRail } from "src/app/Components/UI";
```

## Props principales

- `title: string` titulo del panel.
- `collapsed: boolean` estado controlado.
- `onToggle()` callback para alternar estado.
- `placement?: "right" | "left"` posicion del panel.
- `variant?: "inline" | "overlay"` comportamiento responsive.
- `railLabel?: string` texto del rail (mobile).
- `railIcon?: ReactNode` icono del rail.
- `headerActions?: ReactNode` acciones extras en header.

## Ejemplo desktop

```tsx
<AppCollapseRail
  title="Herramientas"
  collapsed={collapsed}
  onToggle={() => setCollapsed((prev) => !prev)}
>
  <ToolsList />
</AppCollapseRail>
```

## Ejemplo mobile (overlay)

```tsx
<AppCollapseRail
  title="Herramientas"
  collapsed={collapsed}
  onToggle={() => setCollapsed((prev) => !prev)}
  variant="overlay"
  railLabel="Herramientas"
>
  <ToolsList />
</AppCollapseRail>
```
