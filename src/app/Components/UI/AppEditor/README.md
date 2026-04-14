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
- Tokens CSS preparados para light/dark mode y refinamiento visual responsive

## Props principales

- `value` y `onChange`: modo controlado con HTML serializado
- `defaultValue`: modo no controlado
- `placeholder`: texto inicial del editor vacio
- `disabled` y `readOnly`: bloqueo funcional y visual
- `label`, `helperText`, `error`: semantica accesible y feedback de campo
- `title`, `description`, `headerActions`: contexto del shell del editor
- `className`, `surfaceClassName`, `minHeight`, `aria-label`: personalizacion de layout

## Ejemplo basico

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

## Ejemplo controlled

```tsx
import { useState } from "react";
import { AppEditor } from "../UI";

export function ControlledEditor() {
  const [html, setHtml] = useState("<p>Descripcion inicial</p>");

  return (
    <AppEditor
      label="Descripcion"
      value={html}
      onChange={setHtml}
      helperText="El valor se serializa como HTML"
    />
  );
}
```

## Ejemplo disabled y readOnly

```tsx
import { AppEditor } from "../UI";

export function LockedExamples() {
  return (
    <>
      <AppEditor
        label="Editor deshabilitado"
        disabled
        defaultValue="<p>No editable temporalmente</p>"
      />

      <AppEditor
        label="Editor solo lectura"
        readOnly
        defaultValue="<p>Contenido auditado</p>"
      />
    </>
  );
}
```

## Buenas practicas

- Usa modo controlado cuando el contenido forme parte de un formulario o flujo con submit.
- Usa `readOnly` para vistas de consulta y `disabled` cuando la accion este bloqueada.
- Provee `label` o `aria-label` para no depender solo del contexto visual.
- Mantiene `headerActions` para acciones contextuales del shell, no para comandos del editor.
- Trata el valor como HTML serializado y sanitiza en backend o al renderizar fuera del editor si aplica.

## Limitaciones conocidas

- La insercion de links e imagenes usa `window.prompt`, suficiente para shared UI base pero no para una UX avanzada.
- No existe aun integracion real en un modulo consumidor del producto; la validacion actual es focalizada y de integracion representativa.
- Los shortcuts avanzados dependen del comportamiento de Tiptap y no se documentan como API publica estable del componente.
