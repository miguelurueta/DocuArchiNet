import { useMemo } from "react";
import type { MenuNode } from "../types/menu";
import { useMenuItems } from "./useMenuItems";

export type DashboardMenuState = {
  menuTree: MenuNode[];
  isLoading: boolean;
  error: Error | null;
};

export function useDashboardMenu(): DashboardMenuState {
  const q = useMenuItems();
  //console.log(q.data);
  // Normaliza salida (escala fácil: roles, permisos, módulos, etc.)
  return useMemo(
    () => ({
      menuTree: q.data ?? [],
      isLoading: q.isLoading,
      error: q.error ?? null,
    }),
    [q.data, q.isLoading, q.error]
  );
}
