# AppButton

`AppButton` es el boton estandar reusable del proyecto. Existe para encapsular `Button` y `Tooltip` de Ant Design detras de una API propia de la aplicacion, manteniendo consistencia visual, semantica y de accesibilidad sin acoplar a las vistas consumidoras al proveedor UI.

## Importacion

```tsx
import { AppButton } from "src/app/Components/UI";
```

## Props principales

| Prop | Tipo | Default | Descripcion |
| --- | --- | --- | --- |
| `children` | `ReactNode` | `undefined` | Texto o contenido principal del boton. |
| `variant` | `"primary" \| "secondary" \| "success" \| "warning" \| "danger" \| "ghost" \| "link"` | `"primary"` | Semantica visual del design system. |
| `size` | `"sm" \| "md" \| "lg"` | `"md"` | Tamano visual y de espaciado. |
| `loading` | `boolean` | `false` | Muestra estado de carga, bloquea interacciones y propaga semantica de deshabilitado. |
| `disabled` | `boolean` | `false` | Bloquea interacciones manualmente. |
| `htmlType` | `"button" \| "submit" \| "reset"` | `"button"` | Tipo HTML del boton. |
| `leftIcon` | `ReactNode` | `undefined` | Icono antes del texto. |
| `rightIcon` | `ReactNode` | `undefined` | Icono despues del texto. |
| `icon` | `ReactNode` | `undefined` | Icono principal; si no hay `children`, activa modo `icon-only`. |
| `fullWidth` | `boolean` | `false` | Hace que el boton ocupe el ancho disponible. |
| `tooltip` | `string` | `undefined` | Ayuda contextual mostrada con `Tooltip`. |
| `className` | `string` | `undefined` | Permite componer clases externas con CSS Modules internos. |

`AppButton` hereda props utiles de Ant Design a traves de un `Omit<ComponentProps<typeof AntButton>, ...>` controlado, de modo que puede recibir atributos compatibles como `id`, `data-*`, `aria-*` y handlers soportados, sin exponer directamente la API visual base de Ant Design.

## Reglas de iconografia

- `icon` + sin `children` => modo `icon-only`.
- En `icon-only`, `aria-label` es obligatorio.
- `leftIcon` y `rightIcon` se usan para botones con texto.
- Si hay `children`, el icono visual principal se resuelve con `leftIcon`; `rightIcon` queda al final del contenido.

## Ejemplos

### Primario

```tsx
<AppButton variant="primary">Guardar cambios</AppButton>
```

### Con icono

```tsx
<AppButton
  variant="secondary"
  leftIcon={<EditOutlined />}
>
  Editar documento
</AppButton>
```

### Loading

```tsx
<AppButton loading>Guardando...</AppButton>
```

### Tooltip

```tsx
<AppButton tooltip="Ejecuta la sincronizacion manual">
  Sincronizar
</AppButton>
```

### Full width

```tsx
<AppButton fullWidth variant="success">
  Confirmar envio
</AppButton>
```

### Icon-only

```tsx
<AppButton
  icon={<DeleteOutlined />}
  aria-label="Eliminar documento"
  variant="danger"
/>
```

### Accion en tabla

```tsx
<AppButton
  icon={<EyeOutlined />}
  aria-label="Ver detalle"
  variant="ghost"
  tooltip="Abrir detalle del registro"
/>
```

## Integracion con operaciones asincronicas

`AppButton` no depende de `OperationBlockerContext`, pero puede componerse con ese contexto o con cualquier hook de mutacion externo:

```tsx
const { block, unblock } = useOperationBlocker();

async function handleSubmit() {
  block("Guardando cambios...");
  try {
    await saveDocument();
  } finally {
    unblock();
  }
}

<AppButton loading={isPending} onClick={handleSubmit}>
  Guardar
</AppButton>
```

Este patron mantiene al componente desacoplado de dominio e infraestructura.

## Buenas practicas

- Usa `primary` para la accion principal de la vista.
- Usa `loading` cuando la accion dispara una operacion asincronica que no debe duplicarse.
- Usa `disabled` para estados no disponibles por reglas de negocio.
- Usa `icon-only` solo cuando el contexto visual sea obvio y siempre con `aria-label`.
- Prefiere `tooltip` para explicar acciones secundarias o restringidas.

## Ejemplo de integracion en una vista

```tsx
<section>
  <AppButton
    variant="primary"
    leftIcon={<SaveOutlined />}
    loading={isSaving}
    onClick={handleSave}
  >
    Guardar solicitud
  </AppButton>

  <AppButton
    variant="ghost"
    icon={<InfoCircleOutlined />}
    aria-label="Ver ayuda"
    tooltip="Muestra ayuda contextual del formulario"
  />
</section>
```
