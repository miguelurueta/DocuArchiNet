export interface ItemDrowlistDTO {
  id_value: string;
  value_campo: string;
}

export interface ConfigServiceDrowlisDTO {
  defaultAlias: string;
  name_dbs_auto: string;
  name_table_auto: string;
  name_campo_value: string;
  name_campo_condicion: string;
  name_campo_orden: string;
  tipo_orden: string;
  value_condicion: string | null;
  value_default: string | null;
  limit_rows: number;
  name_campo_primary: string;
  addd_seleccion: number;
  value_seleccion: string | null;
  campo_estado_auto_lista: number;
}

export interface CampoTomSelectDTO {
  NombreCampoTomSlect: string;
}

export interface CampoTomListDTO {
  NameCampo: string | null;
  NombreCampoTomList: string;
}

export interface TomPParameterTomSelelect {
  defaultDbAlias: string;
  NombrePlantillaValidacion: string;
  NombreCampo: string;
  NameCampoPrimary: string;
  ValueAuto: string | null;
  id_escript: number;
  CamposTomSelect: ReadonlyArray<CampoTomSelectDTO>;
  CamposTomList: ReadonlyArray<CampoTomListDTO>;
  onItemAddAction: string;
  onItemRemoveAction: string;
  IdTipoRestriccion: number;
  IdRestriccion: number;
}

export interface CampoPlantillaDTO {
  Tupcae_label: string;
  label_input_class_font: string | null;
  Place_Holder: string | null;
  control_input_class: string | null;
  name_campo: string;
  aleas_campo: string | null;
  title?: string | null;
  title_control?: string | null;
  tipo_control: string | null;
  value_campo: string;
  obligatorio_campo: number;
  disable_campo: number;
  tipo_campo: string;
  max_leng_campo: number;
  campo_tip?: number | null;
  control_tip_correo?: number | null;
  tbl_control?: string | null;
  error_gestion: string;
  tooltipAyuda: string;
  onChangeAction: string | null;
  serviceName: string | null;
  apiMethod: string | null;
  placeholder: string | null;
  TagSesion: string;
  ComportamientoCampo: string | null;
  dataClear: string | null;
  event_control: ReadonlyArray<unknown> | null;
  ilist_row_drowlist: ReadonlyArray<ItemDrowlistDTO> | null;
  config_service_drowlis: ReadonlyArray<ConfigServiceDrowlisDTO> | null;
  config_service_controls_error: ReadonlyArray<unknown> | null;
  Item_Tom_Select: ReadonlyArray<unknown> | null;
  Item_Tom_row: ReadonlyArray<unknown> | null;
  CamposUpdateIndiceBach: ReadonlyArray<unknown> | null;
  TomPParameterTomSelelect: TomPParameterTomSelelect | null;
}

export interface PlantillaCamposApiResponseDTO {
  Data: ReadonlyArray<CampoPlantillaDTO>;
  Success: boolean;
  Message: string;
  Errors: ReadonlyArray<unknown>;
}
