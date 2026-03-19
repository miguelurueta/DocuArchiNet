import TabsDocu from "../hooks/RadicacionTabs";
import type { PlantillaRadicadoDTO } from "../models/PlantillaRadicadoDTO";
import type { RadicacionPayloadDTO } from "../services/radicacionPayloadSerializer";

interface RadicacionPageProps {
  plantilla: PlantillaRadicadoDTO;
  onSubmit?: (payload: RadicacionPayloadDTO) => void;
}
export function RadicacionPage({ plantilla, onSubmit }: RadicacionPageProps) {
  return (
    <>
      <TabsDocu camposPlantilla={plantilla.DetallePlantillaRadicadoDTO} />
    </>
  );
}
