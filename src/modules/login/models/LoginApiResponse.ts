import type { ApiResponse } from "../../../api/ApiResponse";
import type RespuestaAutenticacion from "./RespuestaAutenticacionDTO";
import type SegundoFactorResultado from "./SegundoFactorResultado";
export type LoginApiResponse =
  | ApiResponse<RespuestaAutenticacion>
  | ApiResponse<SegundoFactorResultado>;
