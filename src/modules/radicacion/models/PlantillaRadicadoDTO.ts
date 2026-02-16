export interface PlantillaRadicadoDTO {
  IdPlantillaRadicado: number;
  NombrePlantilla: string;
  DetallePlantillaRadicadoDTO: ReadonlyArray<DetallePlantillaRadicadoDTO>;
  CamposPlantillaValidacionDTO: ReadonlyArray<CamposPlantillaValidacionDTO>;
  RelCamposValRadicDTO: ReadonlyArray<RelCamposValRadicDTO>;
}

export interface DetallePlantillaRadicadoDTO {
  IdDetallePlantillaRadicado: number;
  NombreCampo: string;
  Etiqueta: string;
  TipoCampo: string;
  Requerido: boolean;
  Orden: number;
  Placeholder?: string;
  ValorDefecto?: string;
  Opciones?: ReadonlyArray<string>;
}

export interface CamposPlantillaValidacionDTO {
  IdCampoPlantillaValidacion: number;
  TipoValidacion: string;
  MensajeValidacion: string;
  Parametro?: string;
}

export interface RelCamposValRadicDTO {
  IdDetallePlantillaRadicado: number;
  IdCampoPlantillaValidacion: number;
}
