# AppToolbar

## Proposito

`AppToolbar` es un componente reusable de la capa UI compartida para construir encabezados contextuales con titulo, descripcion, breadcrumbs, acciones y contenido auxiliar sin acoplar a los modulos consumidores a primitives directos de Ant Design o MUI.

Su primera adopcion real esta en el modulo de gestion de correspondencia y el contrato funcional esta respaldado por el cambio OpenSpec `scrumcore-12-crea-componente-toolbar`.

## Ubicacion

- Implementacion: `src/app/Components/UI/AppToolbar/AppToolbar.tsx`
- Estilos: `src/app/Components/UI/AppToolbar/AppToolbar.module.css`
- Tests: `src/app/Components/UI/AppToolbar/AppToolbar.test.tsx`
- Export: `src/app/Components/UI/AppToolbar/index.ts`

## API publica

### `AppToolbarProps`

- `title: ReactNode`
  Contexto principal obligatorio del encabezado.
- `subtitle?: ReactNode`
  Texto superior opcional para clasificacion o contexto secundario.
- `description?: ReactNode`
  Descripcion de apoyo debajo del titulo.
- `breadcrumbs?: AppToolbarBreadcrumbItem[]`
  Lista opcional de breadcrumbs renderizada como navegacion contextual.
- `primaryAction?: AppToolbarAction`
  Accion principal prioritaria de la toolbar.
- `actions?: AppToolbarAction[]`
  Acciones visibles de soporte.
- `secondaryActions?: AppToolbarAction[]`
  Acciones secundarias que pueden colapsar en overflow en vista compacta.
- `extra?: ReactNode`
  Contenido auxiliar, por ejemplo tags, filtros o indicadores.
- `children?: ReactNode`
  Region adicional para composicion flexible.
- `className?: string`
  Clase CSS opcional para el contenedor raiz.
- `collapseBreakpoint?: "sm" | "md"`
  Punto de quiebre usado para activar el modo compacto.
- `maxVisibleSecondaryActions?: number`
  Cantidad maxima de acciones secundarias visibles antes de enviarlas a overflow.
- `overflowLabel?: string`
  Nombre accesible del disparador de overflow.
- `sticky?: boolean`
  Activa comportamiento sticky del contenedor.

### `AppToolbarBreadcrumbItem`

- `key?: string`
- `label: ReactNode`
- `to?: string`
- `href?: string`
- `onClick?: () => void`
- `current?: boolean`

### `AppToolbarAction`

- `key: string`
- `label: ReactNode`
- `onClick?: AppButtonProps["onClick"]`
- `href?: string`
- `variant?: AppButtonVariant`
- `size?: AppButtonSize`
- `icon?: ReactNode`
- `disabled?: boolean`
- `loading?: boolean`
- `tooltip?: string`
- `ariaLabel?: string`

## Ejemplo de uso

```tsx
import { Space, Tag } from "antd";
import { useNavigate } from "react-router-dom";
import { AppToolbar } from "../../../app/Components/UI/AppToolbar";

export default function GestionCorrespondencia() {
  const navigate = useNavigate();

  return (
    <AppToolbar
      title="Centro operativo del modulo"
      subtitle="AppToolbar enterprise"
      description="Esta vista deja lista la composicion base para incorporar bandejas, detalle de correspondencia y acciones de respuesta."
      breadcrumbs={[
        { key: "dashboard", label: "Dashboard", to: "/dashboard" },
        { key: "gestion-correspondencia", label: "Gestion de correspondencia", current: true },
      ]}
      extra={
        <Space wrap>
          <Tag color="blue">React Router anidado</Tag>
          <Tag color="cyan">Ant Design</Tag>
        </Space>
      }
      actions={[{ key: "refresh", label: "Actualizar resumen", variant: "secondary" }]}
      secondaryActions={[
        { key: "share", label: "Compartir contexto", variant: "ghost" },
        { key: "export", label: "Exportar vista", variant: "ghost" },
      ]}
      primaryAction={{
        key: "open-response",
        label: "Abrir respuesta contextual",
        variant: "primary",
        onClick: () => navigate("respuesta"),
      }}
    />
  );
}
```

## Comportamiento responsive

- En desktop, el contexto visual se muestra a la izquierda y las acciones a la derecha.
- En modo compacto, la toolbar reorganiza sus regiones en vertical.
- Las `secondaryActions` pueden colapsar en un menu de overflow.
- La `primaryAction` se mantiene visible como accion prioritaria.

## Accesibilidad

- La toolbar usa una region con `aria-labelledby` asociada al titulo.
- Los breadcrumbs se renderizan dentro de un `nav` con nombre accesible `Breadcrumb`.
- Las acciones icon-only deben declarar `ariaLabel`.
- El orden de foco sigue la jerarquia visual del encabezado y las acciones.

## Cobertura de pruebas

Se validan al menos estos escenarios:

- renderizado de encabezado, breadcrumbs y accion primaria
- regiones opcionales y acciones visibles en desktop
- colapso de acciones secundarias en overflow en vista compacta
- soporte para acciones icon-only con nombre accesible
- integracion del componente en `gestionCorrespondencia`

## Notas

- El barrel `src/app/Components/UI/index.ts` exporta `AppToolbar`, pero en tests puede ser preferible importar desde `src/app/Components/UI/AppToolbar` para evitar efectos colaterales de otros exports compartidos.
- La primera version no implementa buscador/filtros propios ni variante sticky por defecto de producto; solo deja el contrato preparado para esos casos.
