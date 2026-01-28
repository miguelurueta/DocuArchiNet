import clienteApi from "../../../api/Clienteaxios";
import type LoginRequestDTO from "../models/LoginRequestDTO.model";

export async function seLoginUsuario(data: LoginRequestDTO) {
  const response = await clienteApi.post(
    "/api/accout/ValidaUserAplicacion",
    data
  );

  const resp = response.data;

  // ❌ Error real de negocio (NO contrato)
  if (!resp.success) {
    throw response;
  }

  return resp;
}
