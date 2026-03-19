# AppInput

`AppInput` es el input estandar reusable del proyecto. Encapsula el control base de entrada de la libreria UI detras de una API propia para mantener consistencia visual, accesibilidad y bajo acoplamiento en formularios y filtros.

## Importacion

```tsx
import { AppInput } from "src/app/Components/UI";
```

## Props principales

| Prop | Tipo | Default | Descripcion |
| --- | --- | --- | --- |
| `label` | `ReactNode` | `undefined` | Label visible asociado al input. |
| `helperText` | `ReactNode` | `undefined` | Texto de ayuda o feedback debajo del campo. |
| `error` | `boolean` | `false` | Activa semantica y estilo visual de error. |
| `state` | `"default" \| "error"` | `"default"` | Estado visual alternativo del campo. |
| `value` | `string` | `undefined` | Valor controlado por estado externo. |
| `defaultValue` | `string` | `undefined` | Valor inicial en modo no controlado. |
| `onChange` | `(event) => void` | `undefined` | Callback de cambio propagado por el control base. |
| `placeholder` | `string` | `undefined` | Texto de ayuda inicial dentro del campo. |
| `disabled` | `boolean` | `false` | Deshabilita la interaccion del usuario. |
| `className` | `string` | `undefined` | Permite combinar clases externas con estilos internos. |

## Ejemplos

### Basico

```tsx
<AppInput label="Nombre" placeholder="Escribe tu nombre" />
```

### Controlado

```tsx
<AppInput
  label="Correo"
  value={email}
  onChange={(event) => setEmail(event.target.value)}
/>
```

### Con ayuda y error

```tsx
<AppInput
  label="Telefono"
  error
  helperText="El telefono es obligatorio"
/>
```

### Deshabilitado

```tsx
<AppInput
  label="Numero de radicado"
  defaultValue="RAD-2026-001"
  disabled
/>
```

## Buenas practicas

- Usa `label` siempre que el campo participe en captura principal de formularios.
- Usa `helperText` para instrucciones cortas o feedback de validacion.
- Usa `error` solo cuando exista una condicion de validacion clara para el usuario.
- Mantiene el estado controlado fuera del componente cuando el formulario requiere trazabilidad o validacion coordinada.
