# AppTabs

`AppTabs` es el componente estandar reusable del proyecto para navegacion por pestañas. Encapsula el control base de Ant Design detras de una API propia para mantener consistencia visual, accesibilidad y bajo acoplamiento en vistas con contenido seccionado.

## Importacion

```tsx
import { AppTabs } from "src/app/Components/UI";
```

## Props principales

| Prop | Tipo | Default | Descripcion |
| --- | --- | --- | --- |
| `items` | `AppTabsItem[]` | requerido | Lista de tabs con `key`, `label`, `children` y `disabled` opcional. |
| `activeKey` | `string` | `undefined` | Tab activa en modo controlado. |
| `defaultActiveKey` | `string` | `undefined` | Tab inicial en modo no controlado. |
| `onChange` | `(activeKey: string) => void` | `undefined` | Callback ejecutado cuando cambia la seleccion. |
| `orientation` | `"horizontal" \| "vertical"` | `"horizontal"` | Define la disposicion del tabset. |
| `variant` | `"default" \| "card"` | `"default"` | Variante visual del control. |
| `fullWidth` | `boolean` | `false` | Hace que el tabset ocupe todo el ancho disponible. |
| `className` | `string` | `undefined` | Permite combinar clases externas con estilos internos. |

## Ejemplos

### Basico

```tsx
<AppTabs
  defaultActiveKey="general"
  items={[
    { key: "general", label: "General", children: <GeneralSection /> },
    { key: "historial", label: "Historial", children: <HistorySection /> },
  ]}
/>
```

### Controlado

```tsx
<AppTabs
  activeKey={activeTab}
  onChange={setActiveTab}
  items={items}
/>
```

### Vertical

```tsx
<AppTabs
  orientation="vertical"
  fullWidth
  items={items}
/>
```

### Variante card

```tsx
<AppTabs
  variant="card"
  defaultActiveKey="resumen"
  items={items}
/>
```

## Buenas practicas

- Usa `key` estables y semanticamente significativas para cada tab.
- Prefiere `activeKey` cuando el estado de la tab deba sincronizarse con routing o estado externo.
- Usa `defaultActiveKey` para flujos simples donde la vista no necesita controlar la seleccion.
- Marca tabs no disponibles con `disabled` en lugar de ocultarlas si el usuario debe entender que existen pero no aplican aun.
