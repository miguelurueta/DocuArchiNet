import type { ApiResponse } from "../../../api/ApiResponse";
import clienteApi from "../../../api/Clienteaxios";
import type { OptRecoveryPaswReguest } from "../models/OptRecoveryPaswReguest";
import type { OptRecoveryPaswResponse } from "../models/OptRecoveryPaswResponse";

export async function seVerificarOtpPaswRecovery(
  data: OptRecoveryPaswReguest
): Promise<OptRecoveryPaswResponse> {
  const response = await clienteApi.post<ApiResponse<OptRecoveryPaswResponse>>(
    "/api/accout/recovery/verify-otp",
    data
  );
  if (!response.data.success) {
    throw response;
  }
  return response.data.data; // 👈 MUY IMPORTANTE
}
