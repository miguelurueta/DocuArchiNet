export interface TomPParameterTomSelelect {
  [key: string]: unknown;
}

export interface CampoPlantillaDTO {
  Tupcae_label: string;
  label_input_class_font: string;
  Place_Holder: string;
  control_input_class: string;
  name_campo: string;
  tipo_control: string;
  value_campo: string;
  obligatorio_campo: number;
  disable_campo: number;
  tipo_campo: string;
  max_leng_campo: number;
  error_gestion: string;
  tooltipAyuda: string;
  onChangeAction: string;
  serviceName: string;
  apiMethod: string;
  placeholder: string;
  TagSesion: string;
  ComportamientoCampo: string;
  dataClear: string;
  event_control: ReadonlyArray<unknown>;
  ilist_row_drowlist: ReadonlyArray<unknown>;
  config_service_drowlis: ReadonlyArray<unknown>;
  config_service_controls_error: ReadonlyArray<unknown>;
  Item_Tom_Select: ReadonlyArray<unknown>;
  Item_Tom_row: ReadonlyArray<unknown>;
  CamposUpdateIndiceBach: ReadonlyArray<unknown>;
  TomPParameterTomSelelect: TomPParameterTomSelelect;
}
