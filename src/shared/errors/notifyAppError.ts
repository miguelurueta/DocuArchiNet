
import { toast } from "react-toastify";
import type { AppError } from "./AppError";

export function notifyAppError(error: AppError) {
  if (error.severity === "warning") {
    toast.warning(error.message);
  } else {
    toast.error(error.message);
  }
}
