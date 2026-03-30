# AppDropdown

## Proposito

`AppDropdown` es un componente reusable de la capa UI compartida para encapsular menus desplegables y acciones contextuales sin acoplar a los modulos consumidores al contrato crudo de `Dropdown` de Ant Design.

Su primera adopcion real ocurre dentro de `AppToolbar`, donde resuelve el overflow de acciones secundarias, y el contrato funcional esta respaldado por el cambio OpenSpec `scrumcore-15-crea-componente-appdropdown`.

## Ubicacion

- Implementacion: `src/app/Components/UI/AppDropdown/AppDropdown.tsx`
- Estilos: `src/app/Components/UI/AppDropdown/AppDropdown.module.css`
- Tests: `src/app/Components/UI/AppDropdown/AppDropdown.test.tsx`
- Export: `src/app/Components/UI/AppDropdown/index.ts`

## API publica

### `AppDropdownProps`

- `trigger: ReactElement`
  Trigger obligatorio que abre el menu contextual.
- `items: AppDropdownItem[]`
  Lista tipada de items renderizados dentro del overlay.
- `disabled?: boolean`
  Impide la apertura del dropdown y bloquea interacciones derivadas del trigger.
- `open?: boolean`
  Permite controlar externamente el estado visible del overlay.
- `defaultOpen?: boolean`
  Define el estado inicial cuando el componente se usa en modo no controlado.
- `onOpenChange?: (open: boolean) => void`
  Notifica cambios de visibilidad del dropdown.
- `placement?: "bottom" | "bottomLeft" | "bottomRight" | "top" | "topLeft" | "topRight"`
  Controla la posicion del overlay.
- `ariaLabel?: string`
  Nombre accesible opcional para el trigger, obligatorio en triggers icon-only si el propio trigger no lo define.
- `className?: string`
  Clase CSS opcional para el wrapper del trigger.

### `AppDropdownItem`

- `key: string`
- `label: ReactNode`
- `icon?: ReactNode`
- `danger?: boolean`
- `disabled?: boolean`
- `href?: string`
- `onSelect?: () => void`

## Ejemplo de uso

```tsx
import { AppButton } from "../../../app/Components/UI/AppButton";
import { AppDropdown } from "../../../app/Components/UI/AppDropdown";

export function ActionsMenu() {
  return (
    <AppDropdown
      ariaLabel="Mas acciones"
      trigger={<AppButton icon={<span aria-hidden="true">...</span>} variant="ghost" />}
      items={[
        { key: "refresh", label: "Actualizar", onSelect: () => console.log("refresh") },
        { key: "share", label: "Compartir", icon: <span aria-hidden="true">#</span> },
        { key: "delete", label: "Eliminar", danger: true, disabled: true },
      ]}
    />
  );
}
```

## Comportamiento

- Soporta modo no controlado mediante `defaultOpen`.
- Soporta modo controlado mediante `open` y `onOpenChange`.
- Mantiene visible la metadata visual de cada item, incluyendo iconografia y estado `danger`.
- Los items `disabled` permanecen visibles pero no ejecutan su accion.
- El trigger admite apertura por click y conserva soporte de teclado en el wrapper.

## Accesibilidad

- El trigger expone `aria-haspopup="menu"` y `aria-expanded`.
- Los triggers icon-only deben tener nombre accesible mediante `ariaLabel` o props equivalentes en el trigger original.
- El componente delega la navegacion del menu al patron accesible del proveedor UI, manteniendo un contrato observable del proyecto.
- Los iconos de items se renderizan como decorativos con `aria-hidden`.

## Cobertura de pruebas

Se validan al menos estos escenarios:

- render de items y ejecucion de callbacks al seleccionar acciones
- preservacion de metadata visual y bloqueo de items `disabled`
- bloqueo de apertura cuando el dropdown esta deshabilitado
- soporte de modo controlado con `open` y `onOpenChange`
- exigencia de nombre accesible para triggers icon-only
- integracion del dropdown en `AppToolbar` para overflow de acciones

## Notas

- El barrel `src/app/Components/UI/index.ts` exporta `AppDropdown`, aunque en tests puede ser util importar desde `src/app/Components/UI/AppDropdown`.
- La primera version cubre menus de acciones y overflow contextual; no implementa busqueda, multiseleccion ni virtualizacion.
- `AppToolbar` ya consume `AppDropdown` como wrapper estable para el menu de overflow de acciones secundarias.
