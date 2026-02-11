
import { useQuery } from "@tanstack/react-query";
import type { RawMenuItem, MenuNode } from "../types/menu";

import { setMenuItems } from "../services/menuService";
import { buildMenuTree } from "../utils/buildMenuTree";

export function useMenuItems() {
  return useQuery<RawMenuItem[], Error, MenuNode[]>({
    queryKey: ["dashboard-menu"],
    queryFn: async () => {
      // 👈 aquí llamas directamente tu API
      const rawData = await setMenuItems({ userId: 0 });
      return rawData;
    },
    select: (data) => buildMenuTree(data), // transforma RawMenuItem[] → MenuNode[]
    staleTime: 1000 * 60 * 5, // cache opcional: 5 minutos
  });
}



