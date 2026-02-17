import type { CampoPlantillaDTO, ItemDrowlistDTO } from "../models/CampoPlantillaDTO";
import type {
  DetallePlantillaRadicadoDTO,
  PlantillaRadicadoDTO,
} from "../models/PlantillaRadicadoDTO";

function mapOptions(items: ReadonlyArray<ItemDrowlistDTO> | null): ReadonlyArray<string> {
  if (!items || items.length === 0) {
    return [];
  }

  return items
    .map((item) => item.value_campo)
    .filter((value) => value.trim().length > 0);
}

function mapDetalleCampo(
  campo: CampoPlantillaDTO,
  index: number,
): DetallePlantillaRadicadoDTO {
  return {
    IdDetallePlantillaRadicado: index + 1,
    NombreCampo: campo.name_campo,
    Etiqueta: campo.aleas_campo || campo.Tupcae_label || campo.name_campo,
    TipoCampo: campo.ComportamientoCampo || campo.tipo_control || campo.tipo_campo,
    Requerido: campo.obligatorio_campo === 1,
    Orden: index + 1,
    Placeholder: campo.placeholder || campo.Place_Holder || undefined,
    ValorDefecto: campo.value_campo,
    Opciones: mapOptions(campo.ilist_row_drowlist),
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
