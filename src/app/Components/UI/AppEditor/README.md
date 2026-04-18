# AppEditor

`AppEditor` es el editor enriquecido reusable de la capa shared UI.

## Ubicacion

- `src/app/Components/UI/AppEditor/`

## Caracteristicas

- Basado en Tiptap con extensiones MIT
- Soporta modo controlado y no controlado
- Encabezado contextual opcional
- Soporte para `headerActions` con acciones contextuales del shell
- Toolbar con formato, listas, headings, alineacion, links e imagenes
- La alineacion de texto se resuelve desde un dropdown compacto en la toolbar
- Boton visible para alternar entre tema claro y oscuro
- UI visible para enlaces e imagenes
- Insercion de imagen por URL o archivo
- Control persistido de tamaño de imagen
- Estados `disabled`, `readOnly`, `error` y `helperText`
- Tokens CSS preparados para light/dark mode y refinamiento visual responsive
- Scroll vertical interno del contenido con scrollbar adaptado al tema

## Props principales

- `value` y `onChange`: modo controlado con HTML serializado
- `defaultValue`: modo no controlado
- `placeholder`: texto inicial del editor vacio
- `disabled` y `readOnly`: bloqueo funcional y visual
- `label`, `helperText`, `error`: semantica accesible y feedback de campo
- `title`, `description`, `headerActions`: contexto del shell del editor
- `className`, `surfaceClassName`, `minHeight`, `aria-label`: personalizacion de layout
- `showThemeToggle`, `themeMode`, `defaultThemeMode`, `onThemeModeChange`: control visual del tema del editor
- `AppEditorSaveAction`, `useAppEditorSaveState`, `normalizeEditorHtml`: piezas companion para dirty state y boton Guardar fuera del editor

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

## Ejemplo con boton Guardar

```tsx
import { useState } from "react";
import {
  AppEditor,
  AppEditorSaveAction,
  useAppEditorSaveState,
} from "../UI";

export function EditorWithSave() {
  const [currentValue, setCurrentValue] = useState("<p>Contenido inicial</p>");
  const [savedValue, setSavedValue] = useState("<p>Contenido inicial</p>");
  const { saveStatus } = useAppEditorSaveState({
    currentValue,
    savedValue,
  });

  return (
    <AppEditor
      title="Contenido"
      label="Cuerpo"
      value={currentValue}
      onChange={setCurrentValue}
      headerActions={
        <AppEditorSaveAction
          saveStatus={saveStatus}
          onSave={() => setSavedValue(currentValue)}
        />
      }
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
- Usa dirty state y baseline guardado fuera de `AppEditor`; el shared component no gestiona persistencia.
- Trata el valor como HTML serializado y sanitiza en backend o al renderizar fuera del editor si aplica.

## Limitaciones conocidas

- El resize de imagen actual se resuelve por atributos persistidos y controles de toolbar; no existen drag handles visuales para resize directo sobre la imagen.
- Ya existe integracion embebida en `gestionCorrespondencia`, ademas de la validacion focalizada del componente shared.
- Los shortcuts avanzados dependen del comportamiento de Tiptap y no se documentan como API publica estable del componente.
