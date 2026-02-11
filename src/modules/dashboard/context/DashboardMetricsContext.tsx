import { createContext, useContext } from "react";

export type MetricMap = Map<number, number>;

interface DashboardMetricsContextType {
  metricMap: MetricMap;
}

const DashboardMetricsContext =
  createContext<DashboardMetricsContextType | null>(null);

export function useDashboardMetricsContext() {
  const ctx = useContext(DashboardMetricsContext);
  if (!ctx) {
    throw new Error("Must be used inside DashboardMetricsProvider");
  }
  return ctx;
}

export default DashboardMetricsContext;
