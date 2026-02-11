// hooks/useDashboardMetrics.ts
// import { useQuery } from "@tanstack/react-query";
// import type { DashboardMetric } from "../types/dashboardMetrics";

// async function fetchDashboardMetrics(): Promise<DashboardMetric[]> {
//   // 🔴 AQUÍ VAN TUS APIS REALES
//   // Ejemplo mockeado
//   return [
//     { nodeId: 2, pendingCount: 5 },   // Workflow
//     { nodeId: 20, pendingCount: 12 },  // Correspondencia
//   ];
// }

// export function useDashboardMetrics() {
//   return useQuery({
//     queryKey: ["dashboard-metrics"],
//     queryFn: fetchDashboardMetrics,
//     staleTime: 1000 * 30,
//   });
// }
import { useMemo } from "react";
import { useQuery } from "@tanstack/react-query";
import type { DashboardMetric } from "../types/dashboardMetrics";

async function fetchDashboardMetrics(): Promise<DashboardMetric[]> {
  return [
    { nodeId: 2, pendingCount: 5 },
    { nodeId: 20, pendingCount: 12 },
  ];
}

export function useDashboardMetrics() {
  const query = useQuery({
    queryKey: ["dashboard-metrics"],
    queryFn: fetchDashboardMetrics,
    staleTime: 1000 * 30,
  });

  const metricMap = useMemo(() => {
    const map = new Map<number, number>();
    (query.data ?? []).forEach((m) =>
      map.set(m.nodeId, m.pendingCount)
    );
    return map;
  }, [query.data]);

  return {
    ...query,
    metricMap,
  };
}
