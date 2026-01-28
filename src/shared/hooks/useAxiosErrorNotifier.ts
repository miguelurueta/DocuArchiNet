import axios, { AxiosError } from "axios";
import { toast } from "react-toastify";
import type { ValidationErrorDTO } from "../Modules/ValidationErrorDTO";
import type { ApiResponse } from "../../api/ApiResponse";

/**
 * Mapa extensible de constructores de mensajes
 * Cada Type se resuelve aquí
 */
const validationMessageBuilders: Record<
  string,
  (error: ValidationErrorDTO) => string
> = {
  Required: (e) => `${e.Message}`,
  MaxLength: (e) => `${e.Field}: ${e.Message}`,
  MinLength: (e) => `${e.Field}: ${e.Message}`,
  Email: (e) => `Formato inválido para ${e.Field}`,
  Business:(e)=> `${e.Field}: ${e.Message}`
};

/**
 * Type guard: valida si un objeto es ValidationErrorDTO
 */
function isValidationErrorDTO(value: unknown): value is ValidationErrorDTO {
  if (typeof value !== "object" || value === null) return false;

  const v = value as Record<string, unknown>;

  return (
    typeof v.Field === "string" &&
    typeof v.Message === "string" &&
    typeof v.Type === "string"
  );
}

/**
 * Hook centralizado para notificación de errores Axios
 */
export function useAxiosErrorNotifier() {
  function notifyAxiosError(error: unknown) {

    // 🚫 No es Axios
    if (!axios.isAxiosError(error)) {
      toast.error("Error inesperado en la aplicación.");
      return;
    }

    const axiosError = error as AxiosError<ApiResponse<unknown>>;

    // 🚫 Sin respuesta del backend
    if (!axiosError.response) {
      toast.error("No fue posible conectar con el servidor.");
      return;
    }

    const { status, data } = axiosError.response;

    // 🧠 Errores con estructura
    if (status === 400 && Array.isArray(data?.errors)) {
      const typedErrors = data.errors.filter(isValidationErrorDTO);

      // 🔴 Error de negocio
      const businessError = typedErrors.find(e => e.Type === "Business");
      if (businessError) {
        toast.warning(businessError.Message);
        return;
      }

      // 🟡 Validaciones de formulario
      const validationErrors = typedErrors.filter(e => e.Type !== "Business");
      if (validationErrors.length > 0) {
        handleValidationErrors(validationErrors);
        return;
      }

      // ⚠️ Fallback defensivo
      if (data?.message) {
        toast.error(data.message);
        return;
      }
    }

    // 🔐 Autorización
    if (status === 401 && data?.message) {
      toast.warning(data.message);
      return;
    }

    if (status === 401) {
      toast.warning("Sesión expirada. Por favor inicie sesión nuevamente.");
      return;
    }

    if (status === 403) {
      toast.error("No tiene permisos para realizar esta acción.");
      return;
    }

    // 💥 Error genérico controlado por backend
    if (data?.message) {
      toast.error(data.message);
      return;
    }

    // 💣 Fallback total
    toast.error("Error inesperado al procesar la solicitud.");
  }

  function handleValidationErrors(errors: ValidationErrorDTO[]) {
    errors.forEach((error) => {
      const builder = validationMessageBuilders[error.Type];
      const message = builder
        ? builder(error)
        : `${error.Field}: ${error.Message}`;

      toast.warning(message);
    });
  }

  return notifyAxiosError;
}

