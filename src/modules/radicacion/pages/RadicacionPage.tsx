import TabsDocu from "../hooks/RadicacionTabs";
import type { PlantillaRadicadoDTO } from "../models/PlantillaRadicadoDTO";
import type { RadicacionPayloadDTO } from "../services/radicacionPayloadSerializer";
//import { useRadicacionDynamicForm } from "../hooks/useRadicacionDynamicForm";


interface RadicacionPageProps {
  plantilla: PlantillaRadicadoDTO;
  onSubmit?: (payload: RadicacionPayloadDTO) => void;
}
export function RadicacionPage({ plantilla, onSubmit }: RadicacionPageProps) {
  //const { fields, values, onInputChange, serialize } =
    //useRadicacionDynamicForm(plantilla);
    
  return (
    <>
      <TabsDocu/>
    </>
  );
}
