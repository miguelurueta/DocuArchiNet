import type { ApiResponse } from "../../../api/ApiResponse";
import clienteApi from "../../../api/Clienteaxios";
import type RespuestaAutenticacion from "../../login/models/RespuestaAutenticacionDTO";
import type { VerificarOtpRequest } from "../models/VerificarOtpRequest";

export async function seVerificarSegundoFactor(
  data: VerificarOtpRequest
): Promise<RespuestaAutenticacion> {
  const response = await clienteApi.post<ApiResponse<RespuestaAutenticacion>>(
    "/api/accout/VerificarSegundoFactor",
    data
  );
  const resp = response.data;
  // error de negocio (success=false)
  if (!resp?.success) {
    throw response; // para que lo procese useAxiosErrorNotifier
  }

  // aquí retornas lo que espera React Query: RespuestaAutenticacion
  return resp.data;
}
