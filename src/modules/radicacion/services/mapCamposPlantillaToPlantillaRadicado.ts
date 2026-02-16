import type { CampoPlantillaDTO } from "../models/CampoPlantillaDTO";
import type {
  DetallePlantillaRadicadoDTO,
  PlantillaRadicadoDTO,
} from "../models/PlantillaRadicadoDTO";

function mapDetalleCampo(
  campo: CampoPlantillaDTO,
  index: number,
): DetallePlantillaRadicadoDTO {
  return {
    IdDetallePlantillaRadicado: index + 1,
    NombreCampo: campo.name_campo,
    Etiqueta: campo.Tupcae_label,
    TipoCampo: campo.tipo_control || campo.tipo_campo,
    Requerido: campo.obligatorio_campo === 1,
    Orden: index + 1,
    Placeholder: campo.placeholder || campo.Place_Holder,
    ValorDefecto: campo.value_campo,
    Opciones: [],
  };
}

export function mapCamposPlantillaToPlantillaRadicado(
  campos: ReadonlyArray<CampoPlantillaDTO>,
): PlantillaRadicadoDTO {
  return {
    IdPlantillaRadicado: 0,
    NombrePlantilla: "Plantilla Radicación",
    DetallePlantillaRadicadoDTO: campos.map(mapDetalleCampo),
    CamposPlantillaValidacionDTO: [],
    RelCamposValRadicDTO: [],
  };
}
