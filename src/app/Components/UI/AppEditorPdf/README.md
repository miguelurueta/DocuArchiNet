# AppEditorPdf

`AppEditorPdf` es el alias canonico de editor PDF en la capa shared UI.

## Ubicacion

- `src/app/Components/UI/AppEditorPdf/`

## Decision de arquitectura

- Reutiliza `AppEditor` como engine shared para evitar duplicacion.
- Mantiene contrato tipado compatible para modo controlado/no controlado.
- Permite evolucion incremental por tickets posteriores sin acoplar dominio.
- Incluye un shell CSS propio (`AppEditorPdf.module.css`) para ajustes visuales
  responsive sin romper el contrato funcional del editor.

## Guardar + Dirty State (FE-17)

`AppEditorPdf` expone herramientas reutilizables para manejar dirty state y un boton Guardar.

Ejemplo (consumidor):

```tsx
import { useState } from "react";
import {
  AppEditorPdf,
  AppEditorPdfSaveAction,
  useAppEditorPdfDirtyState,
} from "@/app/Components/UI/AppEditorPdf";

export function EditorRespuesta() {
  const [value, setValue] = useState("<p>Inicial</p>");
  const [saved, setSaved] = useState("<p>Inicial</p>");
  const { saveStatus, isDirty } = useAppEditorPdfDirtyState({
    currentValue: value,
    savedValue: saved,
  });

  return (
    <AppEditorPdf
      value={value}
      onChange={setValue}
      toolbarActions={
        <AppEditorPdfSaveAction
          saveStatus={saveStatus}
          onSave={() => setSaved(value)}
        />
      }
    />
  );
}
```
