import { AxiosError } from "axios";

export default function crearAxiosErrorContrato(mensajes: string[]): AxiosError {
  return new AxiosError(
    "Respuesta de autenticación inválida",
    "CONTRACT_ERROR",
    undefined,
    undefined,
    {
      status: 500,
      statusText: "Error de contrato",
      headers: {},
      config: {},
      data: {
        message: "La API respondió pero el contrato de respuesta de la api no es orrecto",
        detalles: mensajes
      }
    } as any
  );
}
