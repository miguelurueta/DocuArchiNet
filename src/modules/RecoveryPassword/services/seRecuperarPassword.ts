import type { ApiResponse } from "../../../api/ApiResponse";
import clienteApi from "../../../api/Clienteaxios";
import type { RecuperarPasswordRequest } from "../Models/RecuperarPasswordRequest";
import type { RecuperarPasswordResponse } from "../Models/RecuperarPasswordResponse";

export async function setRecuperarPassword(
  data: RecuperarPasswordRequest
): Promise<RecuperarPasswordResponse> {
  const response = await clienteApi.post<ApiResponse<RecuperarPasswordResponse>>(
    "/api/accout/recovery/start",
    data
  );

  if (!response.data.success) {
    throw response;
  }

  return response.data.data;
}
