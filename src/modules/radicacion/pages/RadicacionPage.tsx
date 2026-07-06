import TabsDocu from "../hooks/RadicacionTabs";
import type { CampoPlantillaDTO } from "../models/CampoPlantillaDTO";
import type { PlantillaRadicadoDTO } from "../models/PlantillaRadicadoDTO";
import type { RadicacionPayloadDTO } from "../services/radicacionPayloadSerializer";
//import { useRadicacionDynamicForm } from "../hooks/useRadicacionDynamicForm";


interface RadicacionPageProps {
  plantilla: PlantillaRadicadoDTO;
  camposPlantilla: CampoPlantillaDTO[];
  onSubmit?: (payload: RadicacionPayloadDTO) => void;
}
export function RadicacionPage({
  plantilla,
  camposPlantilla,
  onSubmit,
}: RadicacionPageProps) {
  //const { fields, values, onInputChange, serialize } =
    //useRadicacionDynamicForm(plantilla);
  void onSubmit;
    
  return (
    <>
      <TabsDocu plantilla={plantilla} camposPlantilla={camposPlantilla} />
    </>
  );
}
