import clienteApi from "../../../../api/Clienteaxios";
import type { VisorPdfPermissionsResponse } from "./AppVisorEmbedPdf.permissions";

export async function fetchMisPermisosVisorPdf(params: {
  codigoImpl: string;
  signal?: AbortSignal;
}): Promise<VisorPdfPermissionsResponse> {
  const { codigoImpl, signal } = params;
  const res = await clienteApi.get<VisorPdfPermissionsResponse>(
    `/api/gestor-documental/permisos-visorpdf/implementaciones/${encodeURIComponent(codigoImpl)}/mis-permisos`,
    { signal },
  );
  return res.data;
}

