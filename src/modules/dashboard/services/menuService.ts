import type { ApiResponse } from "../../../api/ApiResponse";
import clienteApi from "../../../api/Clienteaxios";
import type { RawMenuItem } from "../types/menu";
//import menuFallback from "../utils/menuFallback";

export async function setMenuItems(
  data: object
): Promise<RawMenuItem[]> {
  const response = await clienteApi.post<ApiResponse<RawMenuItem[]>>(
    "/api/Menu/inicioMenu",
    data
  );

  if (!response.data.success) {
    throw response;
  }

  return response.data.data;
}