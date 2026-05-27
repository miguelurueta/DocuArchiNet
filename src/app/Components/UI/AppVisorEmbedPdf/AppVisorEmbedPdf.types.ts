export type ViewerEffectivePermissions = {
  allowSignaturePlacement: boolean;
  allowSignatureDelete: boolean;
  allowSignatureLockToggle: boolean;
  allowAnnotationEdit: boolean;
  allowExport: boolean;
  allowPrint: boolean;
};

export type AppVisorLoadInput = {
  url: string;
  isElectronicallySigned: boolean;
  idImagen: number;
  nombreGabinete: string;
  idTareaWorkflow: number;
  radicado: string;
  nombre_modulo: string;
  metadata?: Record<string, unknown>;
};

export type AppVisorLoadResult = {
  ok: boolean;
  fileUrl: string | null;
  permissionsRaw: Record<string, boolean>;
  permissionsEffective: ViewerEffectivePermissions;
  isElectronicallySigned: boolean;
  permissionStatus: "resolved" | "failed" | "not_required";
  errors: string[];
};

export type AppVisorEmbedPdfRef = {
  load(input: AppVisorLoadInput): Promise<AppVisorLoadResult>;
  reset(): void;
  cancelCurrentLoad(): void;
};

