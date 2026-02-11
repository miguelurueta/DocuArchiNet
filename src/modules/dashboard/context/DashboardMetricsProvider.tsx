import { useDashboardMetrics } from "../hooks/useDashboardMetrics";
import DashboardMetricsContext from "./DashboardMetricsContext";

interface Props {
  children: React.ReactNode;
}

export default function DashboardMetricsProvider({ children }: Props) {
  const { metricMap } = useDashboardMetrics();

  return (
    <DashboardMetricsContext.Provider value={{ metricMap }}>
      {children}
    </DashboardMetricsContext.Provider>
  );
}

