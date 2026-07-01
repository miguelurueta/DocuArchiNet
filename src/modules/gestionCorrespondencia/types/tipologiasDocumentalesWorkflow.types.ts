export type TipologiaDocumentalWorkflowQuery = {
  idTareaWf: number;
  idRutaWf: number;
};

export type TipologiaDocumentalWorkflowDto = {
  Id: number;
  Descripcion: string;
};

export type TipologiaDocumentalWorkflowOption = {
  value: number;
  label: string;
  idTipoDocumento: number;
  nombreTipoDocumento: string;
};

export type TipologiasDocumentalesWorkflowResponse = {
  success: boolean;
  message: string;
  data: TipologiaDocumentalWorkflowDto[];
  meta?: {
    Status?: string;
    RequestId?: string;
    Total?: number;
  };
  errors?: unknown[];
};
