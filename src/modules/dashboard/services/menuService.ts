import clienteApi from "../../../api/Clienteaxios";
import type { RawMenuItem } from "../types/menu";
import menuFallback from "../utils/menuFallback";

/**
 * Obtiene el menú desde la API usando Clienteaxios, con fallback local si falla.
 */
export const fetchMenuItems = async (): Promise<RawMenuItem[]> => {
  try {
    const menuUrl =
      typeof window !== "undefined"
        ? new URL("/mock/menu.json", window.location.origin).toString()
        : "/mock/menu.json";
    const { data } = await clienteApi.get<RawMenuItem[]>(menuUrl);
    return data;
  } catch (error) {
    console.warn("No se pudo cargar el menú remoto, usando fallback.", error);
    return menuFallback;
  }
};
