import type { ViewerEffectivePermissions } from "./AppVisorEmbedPdf.types";

export type VisorPdfPermissionsResponse = {
  CodigoImplementacion: string;
  IdUsuario: number;
  Permissions: VisorPdfPermissionsMap;
  Sources?: Record<string, string>;
  GeneratedAt: string;
};

export type VisorPdfPermissionCode =
  | "pdf.view"
  | "pdf.print"
  | "pdf.download"
  | "pdf.annotate.open_signature_modal"
  | "pdf.annotate.signature.draw"
  | "pdf.annotate.signature.upload"
  | "pdf.annotate.signature.personal"
  | "pdf.annotate.signature.place"
  | "pdf.annotate.signature.delete"
  | "pdf.annotate.signature.lock"
  | "pdf.annotate.signature.unlock"
  | "pdf.rotate"
  | "pdf.zoom";

export type VisorPdfPermissionsMap = Partial<Record<VisorPdfPermissionCode, boolean>> & Record<string, boolean>;

const DEFAULT_EFFECTIVE: ViewerEffectivePermissions = {
  allowSignaturePlacement: false,
  allowSignatureDelete: false,
  allowSignatureLockToggle: false,
  allowAnnotationEdit: false,
  allowExport: false,
  allowPrint: false,
};

export function resolveCodigoImplementacion(nombreModulo: string): string | null {
  const key = nombreModulo.trim().toLowerCase();
  const map: Record<string, string> = {
    // Gestión de Correspondencia (módulo consumidor real actual)
    gestioncorrespondencia: "gestion_correspondencia",
    "gestión de correspondencia": "gestion_correspondencia",
  };
  return map[key] ?? null;
}

export function mapPermisosVisorPdfToEffectivePermissions(
  permissionsRaw: Record<string, boolean>,
): ViewerEffectivePermissions {
  const raw = permissionsRaw ?? {};
  // Mapping centralizado: keys backend oficiales -> capacidades del visor.
  const allowSignaturePlacement = Boolean(raw["pdf.annotate.signature.place"]);
  const allowSignatureDelete = Boolean(raw["pdf.annotate.signature.delete"]);
  const allowSignatureLockToggle = Boolean(
    raw["pdf.annotate.signature.lock"] || raw["pdf.annotate.signature.unlock"],
  );
  const allowAnnotationEdit = Boolean(
    raw["pdf.annotate.open_signature_modal"] ||
      raw["pdf.annotate.signature.draw"] ||
      raw["pdf.annotate.signature.upload"] ||
      raw["pdf.annotate.signature.personal"] ||
      raw["pdf.annotate.signature.place"],
  );
  const allowExport = Boolean(raw["pdf.download"]);
  const allowPrint = Boolean(raw["pdf.print"]);

  return {
    allowSignaturePlacement,
    allowSignatureDelete,
    allowSignatureLockToggle,
    allowAnnotationEdit,
    allowExport,
    allowPrint,
  };
}

export function applySignedOverride(params: {
  effective: ViewerEffectivePermissions;
  isElectronicallySigned: boolean;
}): ViewerEffectivePermissions {
  const { effective, isElectronicallySigned } = params;
  if (!isElectronicallySigned) return effective;
  // Override duro por firma: edición/firmas quedan bloqueadas.
  return {
    ...effective,
    allowSignaturePlacement: false,
    allowSignatureDelete: false,
    allowSignatureLockToggle: false,
    allowAnnotationEdit: false,
  };
}

export function failClosedEffectivePermissions(): ViewerEffectivePermissions {
  return { ...DEFAULT_EFFECTIVE };
}

