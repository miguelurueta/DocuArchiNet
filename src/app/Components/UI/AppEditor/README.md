# AppEditor

`AppEditor` es el editor enriquecido reusable de la capa shared UI.

## Ubicacion

- `src/app/Components/UI/AppEditor/`

## Caracteristicas

- Basado en Tiptap con extensiones MIT
- Soporta modo controlado y no controlado
- Encabezado contextual opcional
- Toolbar con formato, listas, headings, alineacion, links e imagenes
- Estados `disabled`, `readOnly`, `error` y `helperText`

## Ejemplo

```tsx
import { useState } from "react";
import { AppEditor } from "../UI";

export function Example() {
  const [value, setValue] = useState("<p>Contenido inicial</p>");

  return (
    <AppEditor
      title="Contenido"
      description="Editor reusable para documentos y respuestas"
      label="Cuerpo"
      value={value}
      onChange={setValue}
      placeholder="Escribe aqui..."
      helperText="Puedes usar formato enriquecido"
    />
  );
}
```
