import { useQuery } from "@tanstack/react-query";
import { fetchMenuItems } from "../services/menuService";

/**
 * Hook de React Query para cargar el menú del dashboard.
 */
export const useMenuItems = () =>
  useQuery({
    queryKey: ["dashboard-menu"],
    queryFn: fetchMenuItems,
  });
