import clienteApi from "../../../../api/Clienteaxios";
import type { VisorPdfPermissionsResponse } from "./AppVisorEmbedPdf.permissions";

type ApiEnvelope<T> = {
  success: boolean;
  message: string;
  data: T;
  meta?: {
    Status?: string;
    Total?: number;
  };
  errors?: Array<{
    Type?: string;
    Field?: string;
    Message?: string;
  }>;
};

function buildEnvelopeErrorMessage(envelope?: ApiEnvelope<VisorPdfPermissionsResponse>): string {
  const detail = envelope?.errors?.find((item) => typeof item?.Message === "string" && item.Message.trim())?.Message;
  if (typeof detail === "string" && detail.trim()) return detail.trim();
  if (envelope?.success) return "Permisos visor PDF: contrato invalido.";
  if (typeof envelope?.message === "string" && envelope.message.trim()) return envelope.message.trim();
  return "Permisos visor PDF: contrato invalido.";
}

export async function fetchMisPermisosVisorPdf(params: {
  codigoImpl: string;
  signal?: AbortSignal;
}): Promise<VisorPdfPermissionsResponse> {
  const { codigoImpl, signal } = params;
  const res = await clienteApi.get<ApiEnvelope<VisorPdfPermissionsResponse>>(
    `/api/gestor-documental/permisos-visorpdf/implementaciones/${encodeURIComponent(codigoImpl)}/mis-permisos`,
    { signal },
  );
  const envelope = res.data;
  if (!envelope?.success || !envelope?.data?.Permissions) {
    throw new Error(buildEnvelopeErrorMessage(envelope));
  }
  return envelope.data;
}

