import type { ViewerEffectivePermissions } from "./AppVisorEmbedPdf.types";

export type VisorPdfPermissionsResponse = {
  CodigoImplementacion: string;
  IdUsuario: number;
  Permissions: Record<string, boolean>;
  Sources?: Record<string, string>;
  GeneratedAt: string;
};

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
    gestioncorrespondencia: "GESTION_CORRESPONDENCIA",
    "gestión de correspondencia": "GESTION_CORRESPONDENCIA",
  };
  return map[key] ?? null;
}

export function mapPermisosVisorPdfToEffectivePermissions(
  permissionsRaw: Record<string, boolean>,
): ViewerEffectivePermissions {
  const raw = permissionsRaw ?? {};
  // Mapping centralizado: keys backend -> capacidades del visor
  const allowSignaturePlacement = Boolean(raw["pdf.signature.add"]);
  const allowSignatureDelete = Boolean(raw["pdf.signature.delete"]);
  const allowSignatureLockToggle = Boolean(raw["pdf.signature.lock"]);
  const allowAnnotationEdit = Boolean(raw["pdf.annotation.edit"]);
  const allowExport = Boolean(raw["pdf.export"]);
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

