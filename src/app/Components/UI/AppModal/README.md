# AppModal

`AppModal` es el modal estandar reusable del proyecto. Encapsula el dialogo base del proveedor UI detras de una API propia para mantener consistencia visual, accesibilidad y bajo acoplamiento en overlays, confirmaciones y dialogos.

## Importacion

```tsx
import { AppModal } from "src/app/Components/UI";
```

## Props principales

| Prop | Tipo | Default | Descripcion |
| --- | --- | --- | --- |
| `open` | `boolean` | requerido | Controla si el modal esta visible. |
| `title` | `ReactNode` | `undefined` | Titulo principal del dialogo. |
| `children` | `ReactNode` | `undefined` | Contenido del cuerpo del modal. |
| `primaryAction` | `AppModalAction` | `undefined` | Accion principal del footer. |
| `secondaryAction` | `AppModalAction` | `undefined` | Accion secundaria del footer. |
| `onClose` | `() => void` | `undefined` | Callback de cierre por boton, mascara o teclado segun configuracion. |
| `closeOnEscape` | `boolean` | `true` | Permite cierre con `Escape`. |
| `hideFooter` | `boolean` | `false` | Oculta por completo el footer estandar. |
| `maskClosable` | `boolean` | `false` | Permite cierre al hacer click fuera del dialogo. |

`AppModalAction` soporta:

| Campo | Tipo | Descripcion |
| --- | --- | --- |
| `label` | `ReactNode` | Texto o contenido visible del boton. |
| `onClick` | `() => void` | Callback de la accion. |
| `loading` | `boolean` | Bloquea la accion y muestra estado de carga. |
| `disabled` | `boolean` | Deshabilita la accion. |

## Ejemplos

### Confirmacion basica

```tsx
<AppModal
  open={open}
  title="Eliminar documento"
  primaryAction={{ label: "Eliminar", onClick: handleDelete }}
  secondaryAction={{ label: "Cancelar", onClick: handleClose }}
  onClose={handleClose}
>
  Esta accion no se puede deshacer.
</AppModal>
```

### Modal con carga

```tsx
<AppModal
  open={open}
  title="Guardar cambios"
  primaryAction={{ label: "Guardar", onClick: handleSave, loading: isSaving }}
  secondaryAction={{ label: "Cancelar", onClick: handleClose, disabled: isSaving }}
>
  Revisa la informacion antes de continuar.
</AppModal>
```

### Modal sin footer

```tsx
<AppModal open={open} title="Ayuda" hideFooter onClose={handleClose}>
  <p>Contenido libre del dialogo.</p>
</AppModal>
```

## Buenas practicas

- Usa `primaryAction` y `secondaryAction` para confirmaciones estandar.
- Usa `hideFooter` cuando el contenido ya incorpora sus propias acciones.
- Mantiene el estado `open` y la logica de cierre fuera del componente.
- Habilita `maskClosable` solo cuando el flujo no requiera confirmacion estricta.
