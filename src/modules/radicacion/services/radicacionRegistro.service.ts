import clienteApi from "../../../api/Clienteaxios";
import type {
  AppResponses,
  RegistrarRadicacionEntranteRequestDto,
  RegistrarRadicacionEntranteResponseDto,
} from "../types/radicacionRegistro.types";

export const RADICACION_REGISTRAR_ENTRANTE_ENDPOINT =
  "/api/radicacion/registrar-entrante";
export const RADICACION_TIPO_MODULO_REGISTRO = 1;

export async function registrarRadicacionEntrante(
  request: RegistrarRadicacionEntranteRequestDto,
  tipoModuloRadicacion = RADICACION_TIPO_MODULO_REGISTRO,
): Promise<AppResponses<RegistrarRadicacionEntranteResponseDto>> {
  const response = await clienteApi.post<
    AppResponses<RegistrarRadicacionEntranteResponseDto>
  >(RADICACION_REGISTRAR_ENTRANTE_ENDPOINT, request, {
    params: { tipoModuloRadicacion },
  });

  return response.data;
}
