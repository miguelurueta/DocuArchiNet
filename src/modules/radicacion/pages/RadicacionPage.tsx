import type { PlantillaRadicadoDTO } from "../models/PlantillaRadicadoDTO";
import type { RadicacionPayloadDTO } from "../services/radicacionPayloadSerializer";
import { useRadicacionDynamicForm } from "../hooks/useRadicacionDynamicForm";
import { RadicacionDynamicRenderer } from "../components/RadicacionDynamicRenderer";

interface RadicacionPageProps {
  plantilla: PlantillaRadicadoDTO;
  onSubmit?: (payload: RadicacionPayloadDTO) => void;
}

export function RadicacionPage({ plantilla, onSubmit }: RadicacionPageProps) {
  const { fields, values, onInputChange, serialize } =
    useRadicacionDynamicForm(plantilla);

  return (
    <form
      onSubmit={(event) => {
        event.preventDefault();
        onSubmit?.(serialize());
      }}
    >
      <RadicacionDynamicRenderer
        fields={fields}
        values={values}
        onChange={onInputChange}
      />
      <button type="submit">Radicar</button>
    </form>
  );
}
