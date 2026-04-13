# Ticket 01 FE

## Titulo

Implementar `AppCollapseRail` core (estructura + estado controlado + accesibilidad)

## Objetivo

Crear el componente reusable que encapsule el panel lateral colapsable con rail,
replicando el comportamiento de GestionRespuesta pero sin acoplarlo al modulo.

## Contexto existente

- Arquitectura: `docs/Architecture/AppCollapseRail/AppCollapseRail-Architecture.md`
- Referencia visual/behavior: `GestionRespuestaRightToolsPanel` + CSS de
  `GestionRespuestaMainTabContent.module.css`

## Restricciones (obligatorio)

- No usar `any`
- No incluir logica de negocio
- No desmontar contenido al colapsar
- No estilos globales; solo CSS Modules
- Control de estado desde el contenedor (`collapsed` + `onToggle`)

## Ubicacion (obligatoria)

```txt
src/app/Components/UI/AppCollapseRail/
```

## Contratos (obligatorios)

```ts
export type AppCollapseRailPlacement = "right" | "left";
export type AppCollapseRailVariant = "inline" | "overlay";

export type AppCollapseRailProps = {
  title: string;
  collapsed: boolean;
  onToggle: () => void;
  children: ReactNode;
  panelId?: string;
  placement?: AppCollapseRailPlacement;
  variant?: AppCollapseRailVariant;
  railLabel?: string;
  railIcon?: ReactNode;
  headerActions?: ReactNode;
  className?: string;
};
```

## Reglas de implementacion (obligatorio)

- `aria-expanded` en boton de toggle.
- `aria-controls` apunta a `panelId` si existe.
- Rail visible solo cuando `collapsed = true`.
- Panel visible cuando `collapsed = false`.
- Panel mantiene contenido montado siempre.
- Toggle disponible en header y en rail.
- `placement` controla si el panel va a derecha o izquierda (clases).

## Estructura sugerida

```tsx
<aside className={styles.panel} data-collapsed={collapsed}>
  <header className={styles.header}>
    <h5>{title}</h5>
    <AppButton ... onClick={onToggle} aria-expanded={!collapsed} />
  </header>
  <div id={panelId} className={styles.surface}>
    {children}
  </div>
</aside>
<div className={styles.rail} data-collapsed={collapsed}>
  {collapsed ? <AppButton ... onClick={onToggle} /> : null}
</div>
```

## Pruebas obligatorias

- Renderiza titulo y children.
- `aria-expanded` alterna segun `collapsed`.
- Rail visible cuando colapsado.
- Contenido persiste al colapsar (no se desmonta).
- Toggle dispara `onToggle` en header y rail.

## Criterios de aceptacion

- Componente reusable creado en UI shared.
- API estable y tipada.
- Accesibilidad basica cubierta.
- Sin acoplamientos a modulos.
