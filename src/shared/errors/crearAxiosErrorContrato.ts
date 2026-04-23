import { AxiosError } from "axios";
import type { AxiosResponse } from "axios";

export default function crearAxiosErrorContrato(mensajes: string[]): AxiosError {
  const response = {
    status: 500,
    statusText: "Error de contrato",
    headers: {},
    config: {},
    data: {
      message:
        "La API respondio pero el contrato de respuesta de la API no es correcto.",
      detalles: mensajes,
    },
  } as unknown as AxiosResponse;

  return new AxiosError(
    "Respuesta de autenticacion invalida",
    "CONTRACT_ERROR",
    undefined,
    undefined,
    response,
  );
}

