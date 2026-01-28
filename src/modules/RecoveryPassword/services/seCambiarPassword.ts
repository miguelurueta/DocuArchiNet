import clienteApi from "../../../api/Clienteaxios";
import type { ApiResponse } from "../../../api/ApiResponse";
import type { CambiarPasswordRequest } from "../Models/CambiarPasswordRequest";
export async function seResetPassword(
  data: CambiarPasswordRequest
): Promise<void> {
  const response = await clienteApi.post<ApiResponse<void>>(
    "/api/accout/recovery/reset-password",
    data
  );

  // ❌ error de negocio
  if (!response.data?.success) {
    throw response; // lo procesa useAxiosErrorNotifier
  }

  // ✔ no retorna payload (solo éxito)
  return;
}
